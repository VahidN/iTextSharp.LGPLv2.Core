using iTextSharp.text.rtf.document;
using ST = iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfPhrase contains multiple RtfChunks
///     @version $Id: RtfPhrase.cs,v 1.10 2008/05/16 19:31:24 psoares33 Exp $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfPhrase : RtfElement
{
    /// <summary>
    ///     Constant for phrase in a table indication
    /// </summary>
    public new static readonly byte[] InTable = DocWriter.GetIsoBytes("\\intbl");

    /// <summary>
    ///     Constant for the line spacing.
    /// </summary>
    public static readonly byte[] LineSpacing = DocWriter.GetIsoBytes("\\sl");

    /// <summary>
    ///     Constant for the resetting of the paragraph defaults
    /// </summary>
    public static readonly byte[] ParagraphDefaults = DocWriter.GetIsoBytes("\\pard");

    /// <summary>
    ///     Constant for resetting of font settings to their defaults
    /// </summary>
    public static readonly byte[] Plain = DocWriter.GetIsoBytes("\\plain");

    /// <summary>
    ///     The height of each line.
    /// </summary>
    private readonly int _lineLeading;

    /// <summary>
    ///     ArrayList containing the RtfChunks of this RtfPhrase
    /// </summary>
    protected List<IRtfBasicElement> Chunks = new();

    /// <summary>
    ///     Constructs a new RtfPhrase for the RtfDocument with the given Phrase
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfPhrase belongs to</param>
    /// <param name="phrase">The Phrase this RtfPhrase is based on</param>
    public RtfPhrase(RtfDocument doc, Phrase phrase) : base(doc)
    {
        if (doc == null)
        {
            throw new ArgumentNullException(nameof(doc));
        }

        if (phrase == null)
        {
            return;
        }

        if (phrase.HasLeading())
        {
            _lineLeading = (int)(phrase.Leading * TWIPS_FACTOR);
        }
        else
        {
            _lineLeading = 0;
        }

        var phraseFont = new ST.RtfFont(null, phrase.Font);
        for (var i = 0; i < phrase.Count; i++)
        {
            var chunk = phrase[i];
            if (chunk is Chunk)
            {
                ((Chunk)chunk).Font = phraseFont.Difference(((Chunk)chunk).Font);
            }

            try
            {
                var rtfElements = doc.GetMapper().MapElement(chunk);
                for (var j = 0; j < rtfElements.Length; j++)
                {
                    Chunks.Add(rtfElements[j]);
                }
            }
            catch (DocumentException)
            {
            }
        }
    }

    /// <summary>
    ///     A basically empty constructor that is used by the RtfParagraph.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfPhrase belongs to.</param>
    protected internal RtfPhrase(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     Sets whether this RtfPhrase is in a header. Sets the correct inTable setting for all
    ///     child elements.
    /// </summary>
    /// <param name="inHeader"> True  if this RtfPhrase is in a header,  false  otherwise</param>
    public override void SetInHeader(bool inHeader)
    {
        base.SetInHeader(inHeader);
        for (var i = 0; i < Chunks.Count; i++)
        {
            Chunks[i].SetInHeader(inHeader);
        }
    }

    /// <summary>
    ///     Sets whether this RtfPhrase is in a table. Sets the correct inTable setting for all
    ///     child elements.
    /// </summary>
    /// <param name="inTable"> True  if this RtfPhrase is in a table,  false  otherwise</param>
    public override void SetInTable(bool inTable)
    {
        base.SetInTable(inTable);
        for (var i = 0; i < Chunks.Count; i++)
        {
            Chunks[i].SetInTable(inTable);
        }
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfPhrase belongs to. Also sets the RtfDocument for all child
    ///     elements.
    /// </summary>
    /// <param name="doc">The RtfDocument to use</param>
    public override void SetRtfDocument(RtfDocument doc)
    {
        base.SetRtfDocument(doc);
        for (var i = 0; i < Chunks.Count; i++)
        {
            Chunks[i].SetRtfDocument(Document);
        }
    }

    /// <summary>
    ///     Write the content of this RtfPhrase. First resets to the paragraph defaults
    ///     then if the RtfPhrase is in a RtfCell a marker for this is written and finally
    ///     the RtfChunks of this RtfPhrase are written.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(ParagraphDefaults, 0, ParagraphDefaults.Length);
        outp.Write(Plain, 0, Plain.Length);
        if (base.InTable)
        {
            outp.Write(InTable, 0, InTable.Length);
        }

        if (_lineLeading > 0)
        {
            outp.Write(LineSpacing, 0, LineSpacing.Length);
            outp.Write(t = IntToByteArray(_lineLeading), 0, t.Length);
        }

        foreach (var rbe in Chunks)
        {
            rbe.WriteContent(outp);
        }
    }
}