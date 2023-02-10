namespace iTextSharp.text.rtf.document.output;

/// <summary>
///     The RtfEfficientMemoryCache is an RtfDataCache that keeps the whole rtf document
///     data in memory.
///     More efficient than {@link RtfMemoryCache}.
///     @author Thomas Bickel (tmb99@inode.at)
/// </summary>
public class RtfEfficientMemoryCache : IRtfDataCache
{
    /// <summary>
    ///     The buffer for the rtf document data.
    /// </summary>
    private readonly RtfByteArrayBuffer _bab;

    /// <summary>
    ///     Constructs a RtfMemoryCache.
    /// </summary>
    public RtfEfficientMemoryCache() => _bab = new RtfByteArrayBuffer();

    /// <summary>
    ///     Gets the OutputStream.
    /// </summary>
    public virtual Stream GetOutputStream() => _bab;

    /// <summary>
    ///     Writes the content of the buffer into the OutputStream.
    /// </summary>
    public virtual void WriteTo(Stream target)
    {
        _bab.WriteTo(target);
    }
}