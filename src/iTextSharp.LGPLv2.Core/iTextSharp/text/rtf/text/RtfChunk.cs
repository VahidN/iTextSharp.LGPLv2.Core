using System.util;
using iTextSharp.text.rtf.document;
using ST = iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfChunk contains one piece of text. The smallest text element available
///     in iText.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfChunk : RtfElement
{
    /// <summary>
    ///     Constant for the subscript flag
    /// </summary>
    private static readonly byte[] _fontSubscript = DocWriter.GetIsoBytes("\\sub");

    /// <summary>
    ///     Constant for the superscript flag
    /// </summary>
    private static readonly byte[] _fontSuperscript = DocWriter.GetIsoBytes("\\super");

    /// <summary>
    ///     Constant for the end of sub / superscript flag
    /// </summary>
    private static readonly byte[] _fontEndSuperSubscript = DocWriter.GetIsoBytes("\\nosupersub");

    /// <summary>
    ///     Constant for background colour.
    /// </summary>
    private static readonly byte[] _backgroundColor = DocWriter.GetIsoBytes("\\chcbpat");

    /// <summary>
    ///     An optional background colour.
    /// </summary>
    private readonly ST.RtfColor _background;

    /// <summary>
    ///     The actual content of this RtfChunk
    /// </summary>
    private readonly string _content = "";

    /// <summary>
    ///     The font of this RtfChunk
    /// </summary>
    private readonly ST.RtfFont _font;

    /// <summary>
    ///     The super / subscript of this RtfChunk
    /// </summary>
    private readonly float _superSubScript;

    /// <summary>
    ///     Whether to use soft line breaks instead of hard ones.
    /// </summary>
    private bool _softLineBreaks;

    /// <summary>
    ///     Constructs a RtfChunk based on the content of a Chunk
    /// </summary>
    /// <param name="doc">The RtfDocument that this Chunk belongs to</param>
    /// <param name="chunk">The Chunk that this RtfChunk is based on</param>
    public RtfChunk(RtfDocument doc, Chunk chunk) : base(doc)
    {
        if (chunk == null)
        {
            return;
        }

        if (chunk.Attributes != null && chunk.Attributes[Chunk.SUBSUPSCRIPT] != null)
        {
            _superSubScript = (float)chunk.Attributes[Chunk.SUBSUPSCRIPT];
        }

        if (chunk.Attributes != null && chunk.Attributes[Chunk.BACKGROUND] != null)
        {
            _background = new ST.RtfColor(Document, (BaseColor)((object[])chunk.Attributes[Chunk.BACKGROUND])[0]);
        }

        _font = new ST.RtfFont(doc, chunk.Font);
        _content = chunk.Content;
    }

    /// <summary>
    ///     Writes the content of this RtfChunk. First the font information
    ///     is written, then the content, and then more font information
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        if (_background != null)
        {
            outp.Write(OpenGroup, 0, OpenGroup.Length);
        }

        _font.WriteBegin(outp);
        if (_superSubScript < 0)
        {
            outp.Write(_fontSubscript, 0, _fontSubscript.Length);
        }
        else if (_superSubScript > 0)
        {
            outp.Write(_fontSuperscript, 0, _fontSuperscript.Length);
        }

        if (_background != null)
        {
            outp.Write(_backgroundColor, 0, _backgroundColor.Length);
            outp.Write(t = IntToByteArray(_background.GetColorNumber()), 0, t.Length);
        }

        outp.Write(Delimiter, 0, Delimiter.Length);

        Document.FilterSpecialChar(outp, _content, false,
                                   _softLineBreaks || Document.GetDocumentSettings().IsAlwaysGenerateSoftLinebreaks());

        if (_superSubScript.ApproxNotEqual(0))
        {
            outp.Write(_fontEndSuperSubscript, 0, _fontEndSuperSubscript.Length);
        }

        _font.WriteEnd(outp);

        if (_background != null)
        {
            outp.Write(CloseGroup, 0, CloseGroup.Length);
        }
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfChunk belongs to.
    /// </summary>
    /// <param name="doc">The RtfDocument to use</param>
    public override void SetRtfDocument(RtfDocument doc)
    {
        base.SetRtfDocument(doc);
        _font.SetRtfDocument(Document);
    }

    /// <summary>
    ///     Sets whether to use soft line breaks instead of default hard ones.
    /// </summary>
    /// <param name="softLineBreaks">whether to use soft line breaks instead of default hard ones.</param>
    public void SetSoftLineBreaks(bool softLineBreaks)
    {
        _softLineBreaks = softLineBreaks;
    }

    /// <summary>
    ///     Gets whether to use soft line breaks instead of default hard ones.
    /// </summary>
    /// <returns>whether to use soft line breaks instead of default hard ones.</returns>
    public bool GetSoftLineBreaks() => _softLineBreaks;
}