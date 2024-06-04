using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfAnnotation provides support for adding Annotations to the rtf document.
///     Only simple Annotations with Title / Content are supported.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfAnnotation : RtfElement
{
    /// <summary>
    ///     Constant for the actual annotation
    /// </summary>
    private static readonly byte[] _annotation = DocWriter.GetIsoBytes("\\*\\annotation");

    /// <summary>
    ///     Constant for the author of the annotation
    /// </summary>
    private static readonly byte[] _annotationAuthor = DocWriter.GetIsoBytes("\\*\\atnauthor");

    /// <summary>
    ///     Constant for the id of the annotation
    /// </summary>
    private static readonly byte[] _annotationId = DocWriter.GetIsoBytes("\\*\\atnid");

    /// <summary>
    ///     The content of this RtfAnnotation
    /// </summary>
    private readonly string _content = "";

    /// <summary>
    ///     The title of this RtfAnnotation
    /// </summary>
    private readonly string _title = "";

    /// <summary>
    ///     Constructs a RtfAnnotation based on an Annotation.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfAnnotation belongs to</param>
    /// <param name="annotation">The Annotation this RtfAnnotation is based off</param>
    public RtfAnnotation(RtfDocument doc, Annotation annotation) : base(doc)
    {
        if (annotation == null)
        {
            throw new ArgumentNullException(nameof(annotation));
        }

        _title = annotation.Title;
        _content = annotation.Content;
    }

    /// <summary>
    ///     Writes the content of the RtfAnnotation
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_annotationId, 0, _annotationId.Length);
        outp.Write(Delimiter, 0, Delimiter.Length);
        outp.Write(t = IntToByteArray(Document.GetRandomInt()), 0, t.Length);
        outp.Write(CloseGroup, 0, CloseGroup.Length);
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_annotationAuthor, 0, _annotationAuthor.Length);
        outp.Write(Delimiter, 0, Delimiter.Length);
        outp.Write(t = DocWriter.GetIsoBytes(_title), 0, t.Length);
        outp.Write(CloseGroup, 0, CloseGroup.Length);
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_annotation, 0, _annotation.Length);
        outp.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
        outp.Write(Delimiter, 0, Delimiter.Length);
        outp.Write(t = DocWriter.GetIsoBytes(_content), 0, t.Length);
        outp.Write(CloseGroup, 0, CloseGroup.Length);
    }
}