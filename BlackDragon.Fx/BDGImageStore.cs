//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//
//using BlackDragon.Core;
//using BlackDragon.Core.TPL;
//
//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
//
//namespace BlackDragon.Fx
//{
//    public class BDGImageSource : NSObject, IImageSource<UIImage>
//    {
//		public const string MainThreadName = "BDG.MainThread";
//
//        readonly string BaseCacheDirectory;
//        readonly IFileSource _fileSource;
//
//        TaskFactory _taskFactory;
//        LimitedConcurrencyLevelTaskScheduler _taskScheduler;
//
//        const int CONCURRENT_THREADS = 5; // max number of downloading threads
//
//        public BDGImageSource(IFileSource fileSource)
//        {
//            _fileSource = fileSource;
//
//            _taskScheduler = new LimitedConcurrencyLevelTaskScheduler(CONCURRENT_THREADS);
//            _taskFactory = new TaskFactory(_taskScheduler);
//
//            BaseCacheDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Caches/Pictures/");
//
//            if (!Directory.Exists(BaseCacheDirectory))
//                Directory.CreateDirectory(BaseCacheDirectory);
//        }
//
//        #region IImageSource implementation
//
//        public void RequestImage(string url, Action<string, UIImage> postLoadAction, Func<UIImage, UIImage> processImage = null)
//        {
//            UIImage img = null;
//
//            if (!string.IsNullOrEmpty(url))
//                img = LoadFromCache(url);   
//
//            if (img != null)
//            {
//                if (postLoadAction != null)
//                    postLoadAction.Invoke(url, img);
//            }
//            else
//            {
//                _taskFactory.StartNew(delegate
//                {
//                    try
//                    {
//                        img = LoadFromRemoteSource(url, processImage);
//
//                        if (img != null)
//                            this.SaveToCache(url, img);
//
//                        if (postLoadAction != null)
//                            postLoadAction.Invoke(url, img);
//                    }
//                    catch (Exception ex)
//                    {
//                        Console.WriteLine(ex.ToString());
//                    }
//                });
//            }
//        }
//
//        public void RequestImages(IEnumerable<string> urls, Action<ConcurrentDictionary<string, UIImage>> postLoadAction, Func<UIImage, UIImage> processImage = null)
//        {
//            if (urls != null && urls.Any())
//            {
//                ConcurrentDictionary<string, UIImage> res = new ConcurrentDictionary<string, UIImage>();
//                List<Task> tasks = new List<Task>();
//
//                foreach (var url in urls)
//                {
//					tasks.Add(_taskFactory.StartNew(() =>
//                    {
//                        var img = LoadFromCache(url);
//                        if (img == null)
//                            img = LoadFromRemoteSource(url, processImage);
//
//                        if (img != null)
//                            res.TryAdd(url, img);
//					}));
//                }
//
//                Task.WaitAll(tasks.ToArray());
//
//                if (postLoadAction != null)
//                    postLoadAction.BeginInvoke(res, null, null);
//            }
//        }
//
//        public void SaveToCache(string id, UIImage img)
//        {
//            string file = GetSafeFileName(id);
//            if (!File.Exists(file))
//            {
//                //Save it to disk
//                NSError err = null;
//                try
//                {
//                    img.AsPNG().Save(file, false, out err);
//                    if (err != null)
//                        Console.WriteLine(err.Code.ToString() + " - " + err.LocalizedDescription);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
//                }
//            }
//        }
//
//        public UIImage LoadFromCache(string url)
//        {
//            UIImage img = null;
//
//            //Next check for a saved file, and load it into cache and return it if found
//            string picFile = GetSafeFileName(url);
//            if (File.Exists(picFile))
//            {
//                try
//                {
//                    img = UIImage.FromFile(picFile);
//                }
//                catch
//                {
//                }
//
//                //if (img != null)
//                //    Console.WriteLine("FromFile: " + url);
//            }
//
//            return img;
//        }
//
//        public void DeleteCachedFiles()
//        {
//            string[] files = new string[] { };
//
//            try 
//            { 
//                files = Directory.GetFiles(BaseCacheDirectory); 
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.ToString());
//            }
//
//            foreach (string file in files)
//            {
//                try 
//                { 
//                    File.Delete(file); 
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine(ex.ToString());
//                }
//            }
//        }
//
//        #endregion IImageSource implementation
//
//        #region Private methods
//
//        private UIImage LoadFromRemoteSource(string url, Func<UIImage, UIImage> processImage = null)
//        {
//            UIImage img = null;
//            try
//            {
//                NSData data;
//
//				Uri uri = new Uri(url.WithHttpProtocol());
//				var byteData = _fileSource.GetBinaryFile(uri);// NSData.FromUrl(NSUrl.FromString(url));
//                if (byteData == null)
//                {
//                    Console.WriteLine("UrlImageStore: No data for URL: " + url);
//                    return null;
//                }
//                else
//                    data = NSData.FromArray(byteData);
//
//                Console.WriteLine("FromUrl: " + url);
//                img = UIImage.LoadFromData(data);
//
//                if (processImage != null)
//                    img = processImage(img);
//
//                this.SaveToCache(url, img);
//            }
//            catch (Exception ex)
//            {
//				Console.WriteLine("File load failed: " + url);
//                Console.WriteLine(ex.ToString());
//            }
//
//            return img;
//        }
//
//        private string GetSafeFileName(string url)
//        {
//            var fileType = GetFileType(url);
//            string picFile = BaseCacheDirectory + GenerateMD5(url) + (fileType == FileCacheTypes.Png ? ".png" : ".jpg");
//
//            return picFile;
//        }
//
//        private FileCacheTypes GetFileType(string fileName)
//        {
//            if (!string.IsNullOrWhiteSpace(fileName))
//            {
//                var pos = fileName.LastIndexOf('.');
//                if (pos > -1 && pos < fileName.Length - 1)
//                {
//                    var ext = fileName.Substring(pos + 1).ToLower();
//                    if (ext == "jpg" || ext == "jpeg")
//                        return FileCacheTypes.Jpg;
//                    else if (ext == "png")
//                        return FileCacheTypes.Png;
//                }
//
//                return FileCacheTypes.Txt;
//            }
//
//            return FileCacheTypes.Unknown;
//        }
//
//        /// <summary>
//        /// method to generate a MD5 hash of a string
//        /// </summary>
//        /// <param name="strToHash">string to hash</param>
//        /// <returns>hashed string</returns>
//        private string GenerateMD5(string str)
//        {
//            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
//
//            byte[] byteArray = Encoding.ASCII.GetBytes(str);
//
//            byteArray = md5.ComputeHash(byteArray);
//
//            string hashedValue = "";
//
//            foreach (byte b in byteArray)
//            {
//                hashedValue += b.ToString("x2");
//            }
//
//            return hashedValue;
//        }
//
//        #endregion Private methods
//    }
//}
