namespace iTextSharp.text.pdf.draw;

/// <summary>
///     Helper class implementing the DrawInterface. Can be used to add
///     horizontal or vertical separators. Won't draw anything unless
///     you implement the draw method.
///     @since   2.1.2
/// </summary>
public class VerticalPositionMark : IDrawInterface, IElement
{
    /// <summary>
    ///     Creates a vertical position mark that won't draw anything unless
    ///     you define a DrawInterface.
    /// </summary>
    public VerticalPositionMark()
    {
    }

    /// <summary>
    ///     Creates a vertical position mark that won't draw anything unless
    ///     you define a DrawInterface.
    /// </summary>
    /// <param name="drawInterface">the drawInterface for this vertical position mark.</param>
    /// <param name="offset">the offset for this vertical position mark.</param>
    public VerticalPositionMark(IDrawInterface drawInterface, float offset)
    {
        DrawInterface = drawInterface;
        Offset = offset;
    }

    /// <summary>
    ///     Setter for the interface with the overruling Draw() method.
    /// </summary>
    public virtual IDrawInterface DrawInterface { set; get; }

    /// <summary>
    ///     Setter for the offset. The offset is relative to the current
    ///     Y position. If you want to underline something, you have to
    ///     choose a negative offset.
    /// </summary>
    public virtual float Offset { set; get; }

    /// <summary>
    ///     @see com.lowagie.text.pdf.draw.DrawInterface#draw(com.lowagie.text.pdf.PdfContentByte, float, float, float, float,
    ///     float)
    /// </summary>
    public virtual void Draw(PdfContentByte canvas, float llx, float lly, float urx, float ury, float y)
    {
        if (DrawInterface != null)
        {
            DrawInterface.Draw(canvas, llx, lly, urx, ury, y + Offset);
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.Element#getChunks()
    /// </summary>
    public IList<Chunk> Chunks
    {
        get
        {
            List<Chunk> list = new() { new Chunk(this, true) };
            return list;
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.Element#type()
    /// </summary>
    public int Type { get; } = Element.YMARK;

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    /// </summary>
    public bool IsNestable() => false;

    /// <summary>
    ///     @see com.lowagie.text.Element#process(com.lowagie.text.ElementListener)
    /// </summary>
    public bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            return listener.Add(this);
        }
        catch (DocumentException)
        {
            return false;
        }
    }
}