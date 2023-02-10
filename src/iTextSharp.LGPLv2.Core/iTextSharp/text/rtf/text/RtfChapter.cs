using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfChapter wraps a Chapter element.
///     INTERNAL CLASS
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfChapter : RtfSection
{
    /// <summary>
    ///     Constructs a RtfChapter for a given Chapter
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfChapter belongs to</param>
    /// <param name="chapter">The Chapter this RtfChapter is based on</param>
    public RtfChapter(RtfDocument doc, Chapter chapter) : base(doc, chapter)
    {
    }

    /// <summary>
    ///     Writes the RtfChapter and its contents
    /// </summary>
    public override void WriteContent(Stream result)
    {
        byte[] t;
        if (Document.GetLastElementWritten() != null && !(Document.GetLastElementWritten() is RtfChapter))
        {
            result.Write(t = DocWriter.GetIsoBytes("\\page"), 0, t.Length);
        }

        result.Write(t = DocWriter.GetIsoBytes("\\sectd"), 0, t.Length);
        Document.GetDocumentHeader().WriteSectionDefinition(result);
        if (Title != null)
        {
            Title.WriteContent(result);
        }

        for (var i = 0; i < Items.Count; i++)
        {
            var rbe = Items[i];
            rbe.WriteContent(result);
        }

        result.Write(t = DocWriter.GetIsoBytes("\\sect"), 0, t.Length);
    }
}