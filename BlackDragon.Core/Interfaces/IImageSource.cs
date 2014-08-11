//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//
//namespace BlackDragon.Core
//{
//    public interface IImageSource<T> where T : class
//    {
//        void RequestImage(string url, Action<string, T> postLoadAction, Func<T, T> processImage = null);
//        void RequestImages(IEnumerable<string> urls, Action<ConcurrentDictionary<string, T>> postLoadAction, Func<T, T> processImage = null);
//
//        void SaveToCache(string id, T img);
//        T LoadFromCache(string url);
//
//        void DeleteCachedFiles();
//    }
//}
