namespace iTextSharp.text.rtf.document;

/// <summary>
///     The RtfCodePage class allows different code pages to be used in the rtf document.
///     Currently always ansi / ansicpg1252
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfCodePage : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     Constant for ansi encoded rtf documents
    /// </summary>
    private static readonly byte[] _ansi = DocWriter.GetIsoBytes("\\ansi");

    /// <summary>
    ///     Constant for the ansi codepage
    /// </summary>
    private static readonly byte[] _ansiCodepage = DocWriter.GetIsoBytes("\\ansicpg");

    /// <summary>
    ///     Construct an RtfCodePage
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfCodePage belongs to</param>
    public RtfCodePage(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the selected codepage
    /// </summary>
    public virtual void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(_ansi, 0, _ansi.Length);
        outp.Write(_ansiCodepage, 0, _ansiCodepage.Length);
        var t = IntToByteArray(1252);
        outp.Write(t, 0, t.Length);
        Document.OutputDebugLinebreak(outp);
    }
}