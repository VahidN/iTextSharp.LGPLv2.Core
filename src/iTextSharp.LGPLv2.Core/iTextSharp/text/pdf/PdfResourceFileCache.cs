using System.Collections.Concurrent;

namespace iTextSharp.text.pdf;

public static class PdfResourceFileCache
{
    private static readonly ConcurrentDictionary<string, byte[]> _cache = new();

    public static bool Set(string key, byte[] value) => _cache.TryAdd(key, value);

    public static byte[] Get(string key)
    {
        _cache.TryGetValue(key, out var ret);
        return ret;
    }

    public static bool ContainsKey(string key) => _cache.ContainsKey(key);
}