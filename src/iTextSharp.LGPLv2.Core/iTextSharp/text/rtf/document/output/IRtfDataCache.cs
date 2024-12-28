namespace iTextSharp.text.rtf.document.output;

/// <summary>
///     The RtfDataCache interface must be implemented by classes wishing to
///     act as caches for the rtf document data.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public interface IRtfDataCache
{
    /// <summary>
    ///     Get the OutputStream that the RtfDocument can write to.
    /// </summary>
    /// <returns>The OutputStream the RtfDocument can use.</returns>
    Stream GetOutputStream();

    /// <summary>
    ///     Write the content of the cache into the OutputStream.
    ///     @throws IOException If an error occurs reading/writing.
    /// </summary>
    /// <param name="target">The OutputStream to write the content into.</param>
    void WriteTo(Stream target);
}