using System.Net;
using iTextSharp.text;

namespace iTextSharp.LGPLv2.Core.System.NetUtils;

public static class WebUtils
{
    public static Stream GetResponseStream(this Uri url)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        //CoreFx doesn't support file: or ftp: schemes for WebRequest classes.
        if (url.IsFile)
        {
            return new FileStream(url.LocalPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        var w = WebRequest.Create(url);
#if NET40
            return w.GetResponse().GetResponseStream();
#else
        return w.GetResponseAsync().GetAwaiter().GetResult().GetResponseStream();
#endif
    }

    public static Stream GetResponseStream(this string url) => GetResponseStream(Utilities.ToUrl(url));
}