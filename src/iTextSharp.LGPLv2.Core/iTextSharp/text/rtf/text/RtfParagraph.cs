using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.graphic;
using ST = iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfParagraph is an extension of the RtfPhrase that adds alignment and
///     indentation properties. It wraps a Paragraph.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfParagraph : RtfPhrase
{
    /// <summary>
    ///     Constant for the end of a paragraph
    /// </summary>
    public static readonly byte[] Paragraph = DocWriter.GetIsoBytes("\\par");

    /// <summary>
    ///     An optional RtfParagraphStyle to use for styling.
    /// </summary>
    protected ST.RtfParagraphStyle ParagraphStyle;

    /// <summary>
    ///     Constructs a RtfParagraph belonging to a RtfDocument based on a Paragraph.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfParagraph belongs to</param>
    /// <param name="paragraph">The Paragraph that this RtfParagraph is based on</param>
    public RtfParagraph(RtfDocument doc, Paragraph paragraph) : base(doc)
    {
        if (doc == null)
        {
            throw new ArgumentNullException(nameof(doc));
        }

        if (paragraph == null)
        {
            throw new ArgumentNullException(nameof(paragraph));
        }

        ST.RtfFont baseFont = null;
        if (paragraph.Font is ST.RtfParagraphStyle)
        {
            ParagraphStyle = Document.GetDocumentHeader()
                                     .GetRtfParagraphStyle(((ST.RtfParagraphStyle)paragraph.Font).GetStyleName());
            baseFont = ParagraphStyle;
        }
        else
        {
            baseFont = new ST.RtfFont(Document, paragraph.Font);
            ParagraphStyle =
                new ST.RtfParagraphStyle(Document, Document.GetDocumentHeader().GetRtfParagraphStyle("Normal"));
            ParagraphStyle.SetAlignment(paragraph.Alignment);
            ParagraphStyle.SetFirstLineIndent((int)(paragraph.FirstLineIndent * TWIPS_FACTOR));
            ParagraphStyle.SetIndentLeft((int)(paragraph.IndentationLeft * TWIPS_FACTOR));
            ParagraphStyle.SetIndentRight((int)(paragraph.IndentationRight * TWIPS_FACTOR));
            ParagraphStyle.SetSpacingBefore((int)(paragraph.SpacingBefore * TWIPS_FACTOR));
            ParagraphStyle.SetSpacingAfter((int)(paragraph.SpacingAfter * TWIPS_FACTOR));
            if (paragraph.HasLeading())
            {
                ParagraphStyle.SetLineLeading((int)(paragraph.Leading * TWIPS_FACTOR));
            }

            ParagraphStyle.SetKeepTogether(paragraph.KeepTogether);
        }

        for (var i = 0; i < paragraph.Count; i++)
        {
            var chunk = paragraph[i];
            if (chunk is Chunk)
            {
                ((Chunk)chunk).Font = baseFont.Difference(((Chunk)chunk).Font);
            }
            else if (chunk is RtfImage)
            {
                ((RtfImage)Chunks[i]).SetAlignment(ParagraphStyle.GetAlignment());
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
    ///     Gets the left indentation of this RtfParagraph.
    /// </summary>
    /// <returns>The left indentation.</returns>
    public int GetIndentLeft() => ParagraphStyle.GetIndentLeft();

    /// <summary>
    ///     Gets the right indentation of this RtfParagraph.
    /// </summary>
    /// <returns>The right indentation.</returns>
    public int GetIndentRight() => ParagraphStyle.GetIndentRight();

    /// <summary>
    ///     Sets the left indentation of this RtfParagraph.
    /// </summary>
    /// <param name="indentLeft">The left indentation to use.</param>
    public void SetIndentLeft(int indentLeft)
    {
        ParagraphStyle.SetIndentLeft(indentLeft);
    }

    /// <summary>
    ///     Sets the right indentation of this RtfParagraph.
    /// </summary>
    /// <param name="indentRight">The right indentation to use.</param>
    public void SetIndentRight(int indentRight)
    {
        ParagraphStyle.SetIndentRight(indentRight);
    }

    /// <summary>
    ///     Set whether this RtfParagraph must stay on the same page as the next one.
    /// </summary>
    /// <param name="keepTogetherWithNext">Whether this RtfParagraph must keep together with the next.</param>
    public void SetKeepTogetherWithNext(bool keepTogetherWithNext)
    {
        ParagraphStyle.SetKeepTogetherWithNext(keepTogetherWithNext);
    }

    /// <summary>
    ///     Writes the content of this RtfParagraph. First paragraph specific data is written
    ///     and then the RtfChunks of this RtfParagraph are added.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(ParagraphDefaults, 0, ParagraphDefaults.Length);
        outp.Write(Plain, 0, Plain.Length);
        if (((RtfElement)this).InTable)
        {
            outp.Write(InTable, 0, InTable.Length);
        }

        if (ParagraphStyle != null)
        {
            ParagraphStyle.WriteBegin(outp);
        }

        outp.Write(Plain, 0, Plain.Length);
        for (var i = 0; i < Chunks.Count; i++)
        {
            var rbe = Chunks[i];
            rbe.WriteContent(outp);
        }

        if (ParagraphStyle != null)
        {
            ParagraphStyle.WriteEnd(outp);
        }

        if (!((RtfElement)this).InTable)
        {
            outp.Write(Paragraph, 0, Paragraph.Length);
        }

        Document.OutputDebugLinebreak(outp);
    }
}