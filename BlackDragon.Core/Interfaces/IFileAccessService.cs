using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BlackDragon.Core
{
	public interface IFileAccessService
	{
		void Initialize(bool deleteAllFiles = false);
		void Clear();
		void CreateCacheFolder(string subFolder);

		void Request(string url, string subFolder, Action<FileCacheEntry> postOpAction = null);
		void RequestSingle(string url, Action<FileCacheEntry> postOpAction = null);
		void RequestMultiple(List<string> urls, string subFolder, Action<Dictionary<string, FileCacheEntry>> postOpAction = null);
		void Request(string url, Action<FileCacheEntry> postOpAction = null);
		void RequestMultiple(List<string> urls, Action<Dictionary<string, FileCacheEntry>> postOpAction = null);

		object ReadFromCache(string url, string subFolder = "");
		void WriteToCache(string url, object decodedObject);
		void WriteToCache(string url, string subFolder, object decodedObject);
		void WriteToCache(string url, string subFolder, Stream stream);
		void DeleteFromCache(string url, string subFolder = "");
		bool ExistsInCache(string url, string subFolder = "");

		object ReadFromLocalStorage(string fileName);
		T DeserializeFromLocalStorage<T>(string localFileName);
		void SerializeToLocalStorage(string localFileName, object data);
	}
}
