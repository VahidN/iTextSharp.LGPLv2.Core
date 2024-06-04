namespace iTextSharp.text.pdf.draw;

/// <summary>
///     Element that draws a solid line from left to right.
///     Can be added directly to a document or column.
///     Can also be used to create a separator chunk.
///     @author   Paulo Soares
///     @since    2.1.2
/// </summary>
public class LineSeparator : VerticalPositionMark
{
    /// <summary>
    ///     Creates a new instance of the LineSeparator class.
    /// </summary>
    /// <param name="lineWidth">the thickness of the line</param>
    /// <param name="percentage">the width of the line as a percentage of the available page width</param>
    /// <param name="lineColor">the color of the line</param>
    /// <param name="align">the alignment</param>
    /// <param name="offset">the offset of the line relative to the current baseline (negative = under the baseline)</param>
    public LineSeparator(float lineWidth, float percentage, BaseColor lineColor, int align, float offset)
    {
        LineWidth = lineWidth;
        Percentage = percentage;
        LineColor = lineColor;
        Alignment = align;
        Offset = offset;
    }

    /// <summary>
    ///     Creates a new instance of the LineSeparator class with
    ///     default values: lineWidth 1 user unit, width 100%, centered with offset 0.
    /// </summary>
    public LineSeparator()
    {
    }

    /// <summary>
    ///     Setter for the alignment of the line.
    /// </summary>
    public int Alignment { get; set; } = Element.ALIGN_CENTER;

    public BaseColor LineColor { get; set; }

    /// <summary>
    ///     Setter for the line width.
    /// </summary>
    public float LineWidth { get; set; } = 1;

    /// <summary>
    ///     Setter for the width as a percentage of the available width.
    /// </summary>
    /// <returns>a width percentage</returns>
    public float Percentage { get; set; } = 100;

    /// <summary>
    ///     @see com.lowagie.text.pdf.draw.DrawInterface#draw(com.lowagie.text.pdf.PdfContentByte, float, float, float, float,
    ///     float)
    /// </summary>
    public override void Draw(PdfContentByte canvas, float llx, float lly, float urx, float ury, float y)
    {
        if (canvas == null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        canvas.SaveState();
        DrawLine(canvas, llx, urx, y);
        canvas.RestoreState();
    }

    /// <summary>
    ///     Draws a horizontal line.
    /// </summary>
    /// <param name="canvas">the canvas to draw on</param>
    /// <param name="leftX">the left x coordinate</param>
    /// <param name="rightX">the right x coordindate</param>
    /// <param name="y">the y coordinate</param>
    public void DrawLine(PdfContentByte canvas, float leftX, float rightX, float y)
    {
        if (canvas == null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        float w;
        if (Percentage < 0)
        {
            w = -Percentage;
        }
        else
        {
            w = (rightX - leftX) * Percentage / 100.0f;
        }

        float s;
        switch (Alignment)
        {
            case Element.ALIGN_LEFT:
                s = 0;
                break;
            case Element.ALIGN_RIGHT:
                s = rightX - leftX - w;
                break;
            default:
                s = (rightX - leftX - w) / 2;
                break;
        }

        canvas.SetLineWidth(LineWidth);
        if (LineColor != null)
        {
            canvas.SetColorStroke(LineColor);
        }

        canvas.MoveTo(s + leftX, y + Offset);
        canvas.LineTo(s + w + leftX, y + Offset);
        canvas.Stroke();
    }
}