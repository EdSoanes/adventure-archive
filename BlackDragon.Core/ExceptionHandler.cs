using System;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace BlackDragon.Core
{
    public class ExceptionHandler
    {
		public static StatusCode HttpDownload(Exception ex)
		{
			StatusCode res = StatusCode.UnknownErrorOccured;

			if (ex is WebException)
				res = StatusCode.HttpFileNotFound;
			else if (ex is JsonSerializationException)
				res = StatusCode.JsonDeserializeFailed;
			else if (ex is UriFormatException)
				res = StatusCode.HttpInvalidUrl;

			Console.WriteLine(ex.ToString());
			return res;
		}

		public static StatusCode LoadFile(Exception ex)
		{
			StatusCode res = StatusCode.UnknownErrorOccured;

			if (ex is IOException)
				res = StatusCode.FileAccessError;
			else if (ex is JsonSerializationException)
				res = StatusCode.JsonDeserializeFailed;
			else if (ex is UriFormatException)
				res = StatusCode.HttpInvalidUrl;

			Console.WriteLine(ex.ToString());
			return res;
		}
    }
}

