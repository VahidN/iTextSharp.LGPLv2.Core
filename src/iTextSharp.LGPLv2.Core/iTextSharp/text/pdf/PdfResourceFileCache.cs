using System.Collections.Concurrent;

namespace iTextSharp.text.pdf
{
    public static class PdfResourceFileCache
    {
        private static readonly ConcurrentDictionary<string, byte[]> _cache = new ConcurrentDictionary<string, byte[]>();

        public static bool Set(string key, byte[] value)
        {
            return _cache.TryAdd(key, value);
        }

        public static byte[] Get(string key)
        {
            byte[] ret;
            _cache.TryGetValue(key, out ret);
            return ret;
        }

        public static bool ContainsKey(string key)
        {
            return _cache.ContainsKey(key);
        }
    }
}