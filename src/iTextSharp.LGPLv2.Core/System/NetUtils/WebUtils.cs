using System;
using System.IO;
using System.Net;
using iTextSharp.text;

namespace iTextSharp.LGPLv2.Core.System.NetUtils
{
    public static class WebUtils
    {
        public static Stream GetResponseStream(this Uri url)
        {
            //CoreFx doesn't support file: or ftp: schemes for WebRequest classes.
            if (url.IsFile)
            {
                return new FileStream(url.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            var w = WebRequest.Create(url);
            return w.GetResponseAsync().Result.GetResponseStream();
        }

        public static Stream GetResponseStream(this string url)
        {
            return GetResponseStream(Utilities.ToUrl(url));
        }
    }
}