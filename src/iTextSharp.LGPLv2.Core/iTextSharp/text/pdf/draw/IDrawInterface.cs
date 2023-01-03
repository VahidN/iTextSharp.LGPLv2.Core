namespace iTextSharp.text.pdf.draw;

/// <summary>
///     Interface for an Element that allows you to draw something at the current
///     vertical position. Trivial implementations are LineSeparator and VerticalPositionMark.
///     It is also used to define what has to be drawn by a separator chunk.
///     @since 2.1.2
/// </summary>
public interface IDrawInterface
{
    /// <summary>
    ///     Implement this method if you want to draw something at the current Y position
    ///     (for instance a line).
    /// </summary>
    /// <param name="canvas">the canvas on which you can draw</param>
    /// <param name="llx">the x coordinate of the left page margin</param>
    /// <param name="lly">the y coordinate of the bottom page margin</param>
    /// <param name="urx">the x coordinate of the right page margin</param>
    /// <param name="ury">the y coordinate of the top page margin</param>
    /// <param name="y">the current y position on the page</param>
    void Draw(PdfContentByte canvas, float llx, float lly, float urx, float ury, float y);
}