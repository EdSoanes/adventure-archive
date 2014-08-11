using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using BlackDragon.Core;
using BlackDragon.Core.TPL;

namespace BlackDragon.Core
{
	/// <summary>
	/// Threadsafe class for loading remote files. Will prevent multiple downloads of the same file
	/// and cache the results using an IFileCacheService instance
	/// </summary>
	public class FileAccessService : IFileAccessService
	{
		private ILogService _log;

		private LimitedConcurrencyLevelTaskScheduler _taskScheduler = null;
		private TaskFactory _taskFactory = null;

		private object _filesPendingLock = new object();
		private object _localStorageAccessLock = new object();

		private string LocalStoragePath
		{
			get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "CacheFiles"); }
		}

		/// <summary>
		/// Files pending download object. Once the file is downloaded, all client requests for the file
		///  are informed.
		/// </summary>
		private Dictionary<string, PendingFileLoadData> _filesPending = new Dictionary<string, PendingFileLoadData>();

		/// <summary>
		/// Internal class for keeping track of pending file loads.
		/// </summary>
		private class PendingFileLoadData
		{
			public PendingFileLoadData(string remoteFileName, string subFolder = "")
			{
				RemoteFileName = remoteFileName;
				SubFolder = subFolder;
				PostOpActions = new List<Action<FileCacheEntry>>();
			}

			public string RemoteFileName { get; private set; }
			public string SubFolder { get; private set; }
			public List<Action<FileCacheEntry>> PostOpActions { get; private set; }
		}

		public FileAccessService(ILogService log)
		{
			_log = log;
			_taskScheduler= new LimitedConcurrencyLevelTaskScheduler(5);
			_taskFactory = new TaskFactory(_taskScheduler);
		}

		/// <summary>
		/// Empties the cache and pending files list.
		/// </summary>
		public void Clear()
		{
			try
			{
				lock (_filesPendingLock)
					_filesPending.Clear();

				lock (_localStorageAccessLock)
					FlushDiskCacheFolder(LocalStoragePath);
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.Clear", ex);
			}
		}

		public void Initialize(bool deleteAllFiles = false)
		{
			try
			{
				if (deleteAllFiles)
					Clear();

				bool cacheDirExists = Directory.Exists(LocalStoragePath);
				if (!cacheDirExists)
					Directory.CreateDirectory(LocalStoragePath);
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.Initialize", ex);
			}
		}

		public void CreateCacheFolder(string subFolder)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(subFolder))
				{
					var path = Path.Combine(LocalStoragePath, subFolder);
					if (!Directory.Exists(path))
					{
						lock (_localStorageAccessLock)
							Directory.CreateDirectory(path);
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.CreateCacheFolder", ex);
			}
		}

		#region Requests

		/// <summary>
		/// Loads a specified file from a remote source. Will first check the cache to
		/// see if the file is already downloaded. If not it will check for a pending
		/// download of the file and use that. If not then a new pending file download is
		/// created and queued.
		/// </summary>
		/// <param name="remoteFileName"></param>
		/// <param name="postOpAction"></param>
		public void Request(string url, string subFolder, Action<FileCacheEntry> postOpAction = null)
		{
			try
			{
				lock (_filesPendingLock)
				{
					if (_filesPending.ContainsKey(url))
					{
						if (postOpAction != null)
							_filesPending[url].PostOpActions.Add(postOpAction);
					}
					else
					{
						var fileLoadTaskData = new PendingFileLoadData(url, subFolder);

						if (postOpAction != null)
						{
							_log.Info("Waiting on load for: " + url);
							fileLoadTaskData.PostOpActions.Add(postOpAction);
						}
						_filesPending.Add(url, fileLoadTaskData);

						_log.Info("Queueing task to load: " + url);
						_taskFactory.StartNew(LoadAction(fileLoadTaskData));
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.Request", ex);
			}
		}

		public void Request(string url, Action<FileCacheEntry> postOpAction = null)
		{
			Request(url, string.Empty, postOpAction);
		}

		/// <summary>
		/// Request a file from a remote source. If there is already a pending request then don't make another.
		/// This method will never call the post op action in that case.
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="postOpAction">Post op action.</param>
		public void RequestSingle(string url, Action<FileCacheEntry> postOpAction = null)
		{
			try
			{
				lock (_filesPendingLock)
				{
					if (!_filesPending.ContainsKey(url))
						Request(url, postOpAction);
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.RequestSingle", ex);
			}
		}

		public void RequestMultiple(List<string> urls, string subFolder, Action<Dictionary<string, FileCacheEntry>> postOpAction = null)
		{
			try
			{
				if (urls != null && urls.Any())
				{
					var tasks = new List<Task<FileCacheEntry>>();

					for (int i = 0; i < urls.Count(); i++)
					{
						var url = urls[i];
						var tcs = new TaskCompletionSource<FileCacheEntry>();
						Request(url, (fileCacheEntry) =>
						{
							tcs.SetResult(fileCacheEntry);
						});

						tasks.Add(tcs.Task);
					}

					var res = new Dictionary<string, FileCacheEntry>();
					Task.Factory.ContinueWhenAll(tasks.ToArray(), (completedTasks) =>
					{
						foreach (var completedTask in completedTasks)
							res.Add(((Task<FileCacheEntry>)completedTask).Result.Url, ((Task<FileCacheEntry>)completedTask).Result);

						if (postOpAction != null)
								postOpAction.BeginInvoke(res, null, null);
					});
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.RequestMultiple", ex);
			}

		}

		public void RequestMultiple(List<string> urls, Action<Dictionary<string, FileCacheEntry>> postOpAction = null)
		{
			RequestMultiple(urls, string.Empty, postOpAction);
		}

		#endregion Requests

		#region Read/write from cache

		/// <summary>
		/// Loads a file from local storage
		/// </summary>
		/// <returns>Null if not found</returns>
		/// <param name="remoteFileName">Remote file name.</param>
		public object ReadFromCache(string url, string subFolder = "")
		{
			try
			{
				if (!string.IsNullOrEmpty(url))
				{
					var safeFileName = string.IsNullOrEmpty(subFolder) ? Path.Combine(LocalStoragePath, ValidCacheFileName(url)) : Path.Combine(Path.Combine(LocalStoragePath, subFolder), ValidCacheFileName(url));
					var fileType = GetFileType(url);
					object fileData = null;

					lock (_localStorageAccessLock)
					{
						if (File.Exists(safeFileName))
						{
							if (fileType == FileCacheTypes.Jpg || fileType == FileCacheTypes.Png)
								fileData = File.ReadAllBytes(safeFileName);
							else
								fileData = File.ReadAllText(safeFileName);

							return fileData;
						}
					}
				}

				return null;
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.ReadFromCache", ex);
				return null;
			}
		}

		/// <summary>
		/// Writes an object directly into the cache with the given name
		/// </summary>
		/// <param name="remoteFileName">Remote file name.</param>
		/// <param name="subFolder"></param> 
		/// <param name="decodedObject">Decoded object.</param>
		public void WriteToCache(string url, string subFolder, object fileData)
		{
			try
			{
				if (!string.IsNullOrEmpty(url) && fileData != null)
				{
					var safeFileName = string.IsNullOrEmpty(subFolder) ? Path.Combine(LocalStoragePath, ValidCacheFileName(url)) : Path.Combine(Path.Combine(LocalStoragePath, subFolder), ValidCacheFileName(url));
					var fileType = GetFileType(url);

					lock (_localStorageAccessLock)
					{
						if ((fileType == FileCacheTypes.Jpg || fileType == FileCacheTypes.Png) && fileData != null && fileData is byte[])
							File.WriteAllBytes(safeFileName, (byte[])fileData);
						else
							File.WriteAllText(safeFileName, fileData.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.WriteToCache", ex);
			}
		}

		/// <summary>
		/// Writes an object directly into the cache with the given name
		/// </summary>
		/// <param name="remoteFileName">Remote file name.</param>
		/// <param name="decodedObject">Decoded object.</param>
		public void WriteToCache(string url, object fileData)
		{
			WriteToCache(url, string.Empty, fileData);
		}

		public void WriteToCache(string url, string subFolder, Stream stream)
		{
			try
			{
				if (!string.IsNullOrEmpty(url) && stream != null)
				{
					var safeFileName = string.IsNullOrEmpty(subFolder) ? Path.Combine(LocalStoragePath, ValidCacheFileName(url)) : Path.Combine(Path.Combine(LocalStoragePath, subFolder), ValidCacheFileName(url));
					//var fileType = GetFileType(url);

					lock (_localStorageAccessLock)
					{
						using (Stream output = File.Create(safeFileName))
							stream.CopyTo(output);
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.WriteToCache", ex);
			}
		}

		/// <summary>
		/// Delete a file from the cache
		/// </summary>
		/// <param name="url">URL.</param>
		/// <param name="subFolder">Sub folder.</param>
		public void DeleteFromCache(string url, string subFolder = "")
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(url))
				{
					var safeFileName = string.IsNullOrEmpty(subFolder) ? Path.Combine(LocalStoragePath, ValidCacheFileName(url)) : Path.Combine(Path.Combine(LocalStoragePath, subFolder), ValidCacheFileName(url));
					lock (_localStorageAccessLock)
					{
						if (File.Exists(safeFileName))
							File.Delete(safeFileName);
					}
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.DeleteFromCache", ex);
			}
		}

		public bool ExistsInCache(string url, string subFolder = "")
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(url))
				{
					var safeFileName = string.IsNullOrEmpty(subFolder) ? Path.Combine(LocalStoragePath, ValidCacheFileName(url)) : Path.Combine(Path.Combine(LocalStoragePath, subFolder), ValidCacheFileName(url));
					lock (_localStorageAccessLock)
						return File.Exists(safeFileName);
				}

				return false;
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.ExistsInCache", ex);
				return false;
			}
		}

		#endregion Read/write from cache

		#region Read/write from local storage

		/// <summary>
		/// Loads a resource from local storage, not the cache
		/// </summary>
		/// <returns>The from local storage.</returns>
		/// <param name="fileName">File name.</param>
		public object ReadFromLocalStorage(string fileName)
		{
			try
			{
				_log.Info("Loading direct from local storage: " + fileName);
				var fileType = GetFileType(fileName);
				if (fileType == FileCacheTypes.Jpg || fileType == FileCacheTypes.Png)
					return File.ReadAllBytes(fileName);
				else
					return File.ReadAllText(fileName);
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.ReadFromLocalStorage", ex);
				return null;
			}
		}

		public T DeserializeFromLocalStorage<T>(string localFileName)
		{
			try
			{
				if (File.Exists(localFileName))
				{
					using (var rdr = File.OpenText(localFileName))
					{
						var json = rdr.ReadToEnd();
						var data = JsonConvert.DeserializeObject<T>(json);

						return data;
					}
				}

				return default(T);
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.DeserializeFromLocalStorage", ex);
				return default(T);
			}
		}

		public void SerializeToLocalStorage(string localFileName, object data)
		{
			try
			{
				using (var file = File.CreateText(localFileName))
				{
					var json = JsonConvert.SerializeObject(data);
					file.Write(json);
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.SerializeToLocalStorage", ex);
			}
		}

		#endregion Read/write from local storage

		/// <summary>
		/// Internal task that load the file from a remote source, adds it to the cache and 
		/// notifies pending client calls to Load that the file is available.
		/// </summary>
		/// <param name="state"></param>
		private Action LoadAction(PendingFileLoadData fileLoadTaskData)
		{
			return () =>
			{
				try
				{
					if (fileLoadTaskData != null)
					{
						_log.Info("Loading: " + fileLoadTaskData.RemoteFileName);

						var safeFileName = ValidCacheFileName(fileLoadTaskData.RemoteFileName);
						var fileType = GetFileType(fileLoadTaskData.RemoteFileName);
						var decodedObject = ReadFromCache(safeFileName);
						FileCacheEntry fileCacheEntry = null;

						if (decodedObject == null)
						{
							if (fileType == FileCacheTypes.Jpg || fileType == FileCacheTypes.Png)
								decodedObject = GetBinaryFileSync(fileLoadTaskData.RemoteFileName);
							else
								decodedObject = GetTextFileSync(fileLoadTaskData.RemoteFileName);

							fileCacheEntry = new FileCacheEntry(fileLoadTaskData.RemoteFileName, fileType, decodedObject);
							if (decodedObject != null)
							{
								//Add the file to local storage
								WriteToCache(fileLoadTaskData.RemoteFileName, fileCacheEntry.GetData<object>());
								_log.Info("Loaded from url: " + fileLoadTaskData.RemoteFileName);
							}
						}
						else
						{
							_log.Info("Loaded from cache: " + fileLoadTaskData.RemoteFileName);
							fileCacheEntry = new FileCacheEntry(fileLoadTaskData.RemoteFileName, fileType, decodedObject);
						}

						lock (_filesPendingLock)
						{
							foreach (var postOpAction in fileLoadTaskData.PostOpActions)
								postOpAction.BeginInvoke(fileCacheEntry, null, null);

							if (_filesPending.ContainsKey(fileLoadTaskData.RemoteFileName))
								_filesPending.Remove(fileLoadTaskData.RemoteFileName);
						}
					}
				}
				catch (Exception ex)
				{
					_log.Error("FileAccessService.LoadAction", ex);
				}
			};
		}

		#region Private methods

		private byte[] GetBinaryFileSync(string remoteFilename)
		{
			try
			{
				WebClient client = new WebClient();
				client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

				return client.DownloadData(new Uri(remoteFilename));
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.LoadAction", ex);
				return null;
			}
		}

		private string GetTextFileSync(string remoteFilename)
		{
			try
			{
				WebClient client = new WebClient();
				client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

				return client.DownloadString(new Uri(remoteFilename));
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.GetTextFileSync", ex);
				return string.Empty;
			}
		}

		private void FlushDiskCacheFolder(string folder)
		{
			try
			{
				if (Directory.Exists(folder))
				{
					var subFolders = Directory.GetDirectories(folder);
					foreach (var subFolder in subFolders)
						FlushDiskCacheFolder(subFolder);

					var files = Directory.GetFiles(folder);
					foreach (var file in files)
						File.Delete(file);

					Directory.Delete(folder);
				}
			}
			catch (Exception ex)
			{
				_log.Error("FileAccessService.FlushDiskCacheFolder", ex);
			}
		}

		private string ValidCacheFileName(string name)
		{
			name = name.ToLower();
			var ext = GetFileExtension(name);
			if (!string.IsNullOrEmpty(ext))
				name = name.TrimEnd(("." + ext).ToArray());

			name = GenerateMD5(name);

			if (!string.IsNullOrEmpty(ext))
				name = name + "." + ext;

			return name;
		}

		private string GenerateMD5(string str)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

			byte[] byteArray = Encoding.ASCII.GetBytes(str.ToLower());

			byteArray = md5.ComputeHash(byteArray);

			string hashedValue = "";

			foreach (byte b in byteArray)
			{
				hashedValue += b.ToString("x2");
			}

			return hashedValue;
		}

		private string GetFileExtension(string fileName)
		{
			if (!string.IsNullOrWhiteSpace(fileName))
			{
				var posDot = fileName.LastIndexOf('.');
				var posSlash = fileName.LastIndexOf('/');
				if (posSlash < posDot && posDot > -1 && posDot < fileName.Length - 1)
				{
					var ext = fileName.Substring(posDot + 1);
					return ext;
				}
			}

			return string.Empty;
		}

		private FileCacheTypes GetFileType(string fileName)
		{
			fileName = fileName.ToLower();
			var ext = GetFileExtension(fileName);
			switch (ext.ToLower())
			{
				case "jpg":
				case "jpeg":
					return FileCacheTypes.Jpg;
				case "png":
					return FileCacheTypes.Png;
				case "json":
					return FileCacheTypes.Json;
				case "xml":
					return FileCacheTypes.Xml;
				case "txt":
					return FileCacheTypes.Txt;
				default :
					return FileCacheTypes.Unknown;
			}
		}

		#endregion Private methods
	}
}
