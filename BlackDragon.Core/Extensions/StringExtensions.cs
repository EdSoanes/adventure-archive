using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BlackDragon.Core
{
    public static class StringExtensions
    {
        public static string AbsoluteHttpUrl(this string url)
        {
            url = url.Trim().ToLower();

            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                return string.Empty;

            int pos = url.IndexOf(@"//");
            if (pos >= 0)
            {
                if (pos + 2 < url.Length)
                    url = url.Substring(pos + 2);
            }

            pos = url.IndexOf("?");
            if (pos >= 0)
                url = url.Substring(0, pos);

            //Ensure the Uri is HTTP
            url = "http://" + url;

            if (!url.EndsWith(@"/"))
                url += "/";

            return url;
        }

        public static string UrlDomain(this string url)
        {
            //strip the protocol
			url = url.WithoutProtocol();

			var pos = url.IndexOf('/');
            if (pos >= 0)
                return Settings.Protocol + url.Substring(0, pos);
            else
                return Settings.Protocol + url;
        }

		public static string WithHttpProtocol(this string url)
		{
			//strip the protocol
			url = Settings.Protocol + url.WithoutProtocol();
			return url;
		}

		public static string WithoutProtocol(this string url)
		{
			//strip the protocol
			var pos = url.IndexOf("//");
			if (pos > -1)
			{
				if (url.Length == 2)
					url = string.Empty;
				else
					url = url.Substring(pos + 2);
			}

			//Strip the root of leading /
			if (url.StartsWith("/"))
			{
				if (url.Length == 1)
					url = string.Empty;
				else
					url = url.Substring(1);
			}

			return url;
		}

        /// <summary>
        /// Returns a url without protocol and ending with / unless the url ends with a file path
        /// </summary>
        /// <returns>The URL.</returns>
        /// <param name="root">Root.</param>
        /// <param name="path">Path.</param>
        public static string CombineUrl(this string root, string path)
        {
            root = root.Trim();
            path = path.Trim();

            //strip the protocol
            var pos = root.IndexOf("//");
            if (pos > -1)
            {
                if (root.Length == 2)
                    root = string.Empty;
                else
                    root = root.Substring(pos + 2);
            }

            //Strip the root of leading /
            if (root.StartsWith("/"))
            {
                if (root.Length == 1)
                    root = string.Empty;
                else
                    root = root.Substring(1);
            }

            //Ensure root ends with /
            if (!root.EndsWith("/"))
                root += "/";

            //Ensure path doesn't start with /
            if (path.StartsWith("/"))
            {
                if (path.Length == 1)
                    path = string.Empty;
                else
                    path = path.Substring(1);
            }

            var res = Settings.Protocol + root + path;
            if (!res.EndsWith("/"))
            {
                var slashPos = res.LastIndexOf('/');
                var extPos = res.LastIndexOf('.');
                if (slashPos < 0 || extPos < slashPos)
                    res += "/";
            }
            return res;
        }

        public static string SafeFileName(this string path, string extension)
        {
            path = path.Trim().ToLower();
            extension = extension.Trim().ToLower();

            var invalid = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder();

            foreach ( var cur in path ) 
            {
                builder.Append(invalid.Contains(cur) ? '_' : cur);
            }

            if (!string.IsNullOrEmpty(extension))
            {
                if (!extension.StartsWith("."))
                    extension = "." + extension;
            }

            return builder.ToString().TrimStart('_').TrimEnd('_') + extension;
        }


    }
}

