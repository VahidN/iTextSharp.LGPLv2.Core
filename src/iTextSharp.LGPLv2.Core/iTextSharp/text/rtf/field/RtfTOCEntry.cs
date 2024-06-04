namespace iTextSharp.text.rtf.field;

/// <summary>
///     The RtfTOCEntry is used together with the RtfTableOfContents to generate a table of
///     contents. Add the RtfTOCEntry in those locations in the document where table of
///     contents entries should link to
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author <a href="mailto:Steffen.Stundzig@smb-tec.com">Steffen.Stundzig@smb-tec.com</a>
/// </summary>
public class RtfTocEntry : RtfField
{
    /// <summary>
    ///     Constant for the end of hidden text
    /// </summary>
    private static readonly byte[] _textHiddenOff = DocWriter.GetIsoBytes("\\v0");

    /// <summary>
    ///     Constant for the beginning of hidden text
    /// </summary>
    private static readonly byte[] _textHiddenOn = DocWriter.GetIsoBytes("\\v");

    /// <summary>
    ///     Constant for a TOC entry without page numbers
    /// </summary>
    private static readonly byte[] _tocEntryNoPageNumber = DocWriter.GetIsoBytes("\\tcn");

    /// <summary>
    ///     Constant for a TOC entry with page numbers
    /// </summary>
    private static readonly byte[] _tocEntryPageNumber = DocWriter.GetIsoBytes("\\tc");

    /// <summary>
    ///     The entry text of this RtfTOCEntry
    /// </summary>
    private readonly string _entry = "";

    /// <summary>
    ///     Whether to show page numbers in the table of contents
    /// </summary>
    private bool _showPageNumber = true;

    /// <summary>
    ///     Constructs a RtfTOCEntry with a certain entry text.
    /// </summary>
    /// <param name="entry">The entry text to display</param>
    public RtfTocEntry(string entry) : base(null, new Font())
    {
        if (entry != null)
        {
            _entry = entry;
        }
    }

    /// <summary>
    ///     Sets whether to display a page number in the table of contents, or not
    /// </summary>
    /// <param name="showPageNumber">Whether to display a page number or not</param>
    public void SetShowPageNumber(bool showPageNumber)
    {
        _showPageNumber = showPageNumber;
    }

    /// <summary>
    ///     Writes the content of the RtfTOCEntry
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(_textHiddenOn, 0, _textHiddenOn.Length);
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        if (_showPageNumber)
        {
            outp.Write(_tocEntryPageNumber, 0, _tocEntryPageNumber.Length);
        }
        else
        {
            outp.Write(_tocEntryNoPageNumber, 0, _tocEntryNoPageNumber.Length);
        }

        outp.Write(Delimiter, 0, Delimiter.Length);
        Document.FilterSpecialChar(outp, _entry, true, false);
        outp.Write(CloseGroup, 0, CloseGroup.Length);
        outp.Write(_textHiddenOff, 0, _textHiddenOff.Length);
    }

    /// <summary>
    ///     unused
    /// </summary>
    protected override void WriteFieldInstContent(Stream oupt)
    {
    }

    /// <summary>
    ///     unused
    ///     @see com.lowagie.text.rtf.field.RtfField#writeFieldResultContent(java.io.OutputStream)
    /// </summary>
    protected override void WriteFieldResultContent(Stream oupt)
    {
    }
}