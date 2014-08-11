//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Net.Cache;
//using System.Text;
//
//namespace BlackDragon.Core
//{
//    public class FileSource : IFileSource
//    {
//		public string GetTextFile(Uri uri)
//        {
//            WebClient client = new WebClient();
//            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
//
//			return client.DownloadString(uri);
//        }
//
//		public byte[] GetBinaryFile(Uri uri)
//        {
//            WebClient client = new WebClient();
//            client.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
//
//			return client.DownloadData(uri);
//        }
//
//        public T DeserializeFromLocal<T>(string localFileName) where T : class
//        {
//            if (File.Exists(localFileName))
//            {
//                using (var rdr = File.OpenText(localFileName))
//                {
//                    var json = rdr.ReadToEnd();
//                    var data = JsonConvert.DeserializeObject<T>(json);
//
//                    return data;
//                }
//            }
//
//            return default(T);
//        }
//
//        public void SerializeToLocal(string localFileName, object data)
//        {
//            using (var file = File.CreateText(localFileName))
//            {
//                var json = JsonConvert.SerializeObject(data);
//                file.Write(json);
//            }
//        }
//    }
//}
