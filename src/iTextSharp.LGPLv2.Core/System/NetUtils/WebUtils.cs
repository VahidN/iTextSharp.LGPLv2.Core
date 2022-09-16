using System;
using System.IO;
using System.Net;
using System.Net.Http;
using iTextSharp.text;

namespace iTextSharp.LGPLv2.Core.System.NetUtils
{
    public static class WebUtils
    {
        private static HttpClient _client;
        private static HttpClient Client => _client ??= new();

        public static Stream GetResponseStream(this Uri url)
        {
            //CoreFx doesn't support file: or ftp: schemes for WebRequest classes.
            if (url.IsFile)
            {
                return new FileStream(url.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            return Client.GetStreamAsync(url).Result;
        }

        public static Stream GetResponseStream(this string url)
        {
            return GetResponseStream(Utilities.ToUrl(url));
        }
    }
}