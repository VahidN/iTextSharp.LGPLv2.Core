namespace iTextSharp.text.rtf.document.output;

/// <summary>
/// </summary>
public static class RtfDataCache
{
    /// <summary>
    ///     Constant for caching efficently into memory.
    /// </summary>
    public const int CACHE_MEMORY_EFFICIENT = 3;

    /// <summary>
    ///     Constant for caching into memory.
    /// </summary>
    public const int CACHE_MEMORY = 2;

    /// <summary>
    ///     Constant for caching to the disk.
    /// </summary>
    public const int CACHE_DISK = 1;
}