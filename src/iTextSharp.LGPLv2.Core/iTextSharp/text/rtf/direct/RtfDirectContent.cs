namespace iTextSharp.text.rtf.direct;

/// <summary>
///     The RtfDirectContent makes it possible to directly add RTF code into
///     an RTF document. This can be used to directly add RTF fragments that
///     have been created with other RTF editors. One important aspect is that
///     font and color numbers will not be modified. This means that the
///     fonts and colors visible in the final document might not be equivalent
///     with those set on the direct content.
///     For convenience the RtfDirectContent provides a DIRECT_SOFT_LINEBREAK
///     constant that makes it possible to easily add soft line-breaks anywhere in
///     the RTF document.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfDirectContent : RtfAddableElement
{
    /// <summary>
    ///     Add the DIRECT_SOFT_LINEBREAK to the Document to insert
    ///     a soft line-break at that position.
    /// </summary>
    public static readonly RtfDirectContent DirectSoftLinebreak = new("\\line");

    /// <summary>
    ///     The direct content to add.
    /// </summary>
    private readonly string _directContent;

    /// <summary>
    ///     Constructs a new RtfDirectContent with the content to add.
    /// </summary>
    /// <param name="directContent">The content to add.</param>
    public RtfDirectContent(string directContent) => _directContent = directContent;

    /// <summary>
    ///     Writes the element content to the given output stream.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        var contentBytes = DocWriter.GetIsoBytes(_directContent);
        outp.Write(contentBytes, 0, contentBytes.Length);
    }
}