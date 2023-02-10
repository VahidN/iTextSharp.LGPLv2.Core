namespace iTextSharp.text.rtf.document.output;

/// <summary>
///     The RtfMemoryCache is an RtfDataCache that keeps the whole rtf document
///     data in memory. Fast but memory intensive.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfMemoryCache : IRtfDataCache
{
    /// <summary>
    ///     The buffer for the rtf document data.
    /// </summary>
    private readonly MemoryStream _data;

    /// <summary>
    ///     Constructs a RtfMemoryCache.
    /// </summary>
    public RtfMemoryCache() => _data = new MemoryStream();

    /// <summary>
    ///     Gets the MemoryStream.
    /// </summary>
    public Stream GetOutputStream() => _data;

    /// <summary>
    ///     Writes the content of the MemoryStream into the Stream.
    /// </summary>
    public void WriteTo(Stream target)
    {
        _data.WriteTo(target);
    }
}