namespace iTextSharp.text.pdf.draw;

/// <summary>
///     Element that draws a dotted line from left to right.
///     Can be added directly to a document or column.
///     Can also be used to create a separator chunk.
///     @since   2.1.2
/// </summary>
public class DottedLineSeparator : LineSeparator
{
    /// <summary>
    ///     Setter for the gap between the center of the dots of the dotted line.
    /// </summary>
    public float Gap { get; set; } = 5;

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
        canvas.SetLineWidth(LineWidth);
        canvas.SetLineCap(PdfContentByte.LINE_CAP_ROUND);
        canvas.SetLineDash(0, Gap, Gap / 2);
        DrawLine(canvas, llx, urx, y);
        canvas.RestoreState();
    }
}