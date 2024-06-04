using System.util;

namespace iTextSharp.text.pdf.events;

/// <summary>
///     Class for an index.
///     @author Michael Niedermair
/// </summary>
public class FieldPositioningEvents : PdfPageEventHelper, IPdfPCellEvent
{
    /// <summary>
    ///     Keeps the form field that is to be positioned in a cellLayout event.
    /// </summary>
    protected PdfFormField CellField;

    /// <summary>
    ///     The PdfWriter to use when a field has to added in a cell event.
    /// </summary>
    protected PdfWriter FieldWriter;

    /// <summary>
    ///     Keeps a map with fields that are to be positioned in inGenericTag.
    /// </summary>
    protected INullValueDictionary<string, PdfFormField> GenericChunkFields =
        new NullValueDictionary<string, PdfFormField>();

    /// <summary>
    ///     Creates a new event. This constructor will be used if you need to position fields with Chunk objects.
    /// </summary>
    public FieldPositioningEvents()
    {
    }

    /// <summary>
    ///     Creates a new event. This constructor will be used if you need to position fields with a Cell Event.
    /// </summary>
    public FieldPositioningEvents(PdfWriter writer, PdfFormField field)
    {
        CellField = field;
        FieldWriter = writer;
    }

    /// <summary>
    ///     Creates a new event. This constructor will be used if you need to position fields with a Cell Event.
    /// </summary>
    public FieldPositioningEvents(PdfFormField parent, PdfFormField field)
    {
        CellField = field;
        Parent = parent;
    }

    /// <summary>
    ///     Creates a new event. This constructor will be used if you need to position fields with a Cell Event.
    ///     @throws DocumentException
    ///     @throws IOException
    /// </summary>
    public FieldPositioningEvents(PdfWriter writer, string text)
    {
        FieldWriter = writer;
        var tf = new TextField(writer, new Rectangle(0, 0), text);
        tf.FontSize = 14;
        CellField = tf.GetTextField();
    }

    /// <summary>
    ///     Creates a new event. This constructor will be used if you need to position fields with a Cell Event.
    ///     @throws DocumentException
    ///     @throws IOException
    /// </summary>
    public FieldPositioningEvents(PdfWriter writer, PdfFormField parent, string text)
    {
        Parent = parent;
        var tf = new TextField(writer, new Rectangle(0, 0), text);
        tf.FontSize = 14;
        CellField = tf.GetTextField();
    }

    /// <summary>
    /// </summary>
    public float Padding { set; get; }

    /// <summary>
    /// </summary>
    public PdfFormField Parent { set; get; }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfPCellEvent#cellLayout(com.lowagie.text.pdf.PdfPCell, com.lowagie.text.Rectangle,
    ///     com.lowagie.text.pdf.PdfContentByte[])
    /// </summary>
    public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
    {
        if (position == null)
        {
            throw new ArgumentNullException(nameof(position));
        }

        if (CellField == null || (FieldWriter == null && Parent == null))
        {
            throw new ArgumentException("You have used the wrong constructor for this FieldPositioningEvents class.");
        }

        CellField.Put(PdfName.Rect,
                      new PdfRectangle(position.GetLeft(Padding), position.GetBottom(Padding),
                                       position.GetRight(Padding),
                                       position.GetTop(Padding)));
        if (Parent == null)
        {
            FieldWriter.AddAnnotation(CellField);
        }
        else
        {
            Parent.AddKid(CellField);
        }
    }

    /// <summary>
    ///     Add a PdfFormField that has to be tied to a generic Chunk.
    /// </summary>
    public void AddField(string text, PdfFormField field)
    {
        GenericChunkFields[text] = field;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfPageEvent#onGenericTag(com.lowagie.text.pdf.PdfWriter, com.lowagie.text.Document,
    ///     com.lowagie.text.Rectangle, java.lang.String)
    /// </summary>
    public override void OnGenericTag(PdfWriter writer, Document document,
                                      Rectangle rect, string text)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (rect == null)
        {
            throw new ArgumentNullException(nameof(rect));
        }

        rect.Bottom = rect.Bottom - 3;
        var field = GenericChunkFields[text];
        if (field == null)
        {
            var tf = new TextField(writer,
                                   new Rectangle(rect.GetLeft(Padding), rect.GetBottom(Padding), rect.GetRight(Padding),
                                                 rect.GetTop(Padding)), text);
            tf.FontSize = 14;
            field = tf.GetTextField();
        }
        else
        {
            field.Put(PdfName.Rect,
                      new PdfRectangle(rect.GetLeft(Padding), rect.GetBottom(Padding), rect.GetRight(Padding),
                                       rect.GetTop(Padding)));
        }

        if (Parent == null)
        {
            writer.AddAnnotation(field);
        }
        else
        {
            Parent.AddKid(field);
        }
    }
}