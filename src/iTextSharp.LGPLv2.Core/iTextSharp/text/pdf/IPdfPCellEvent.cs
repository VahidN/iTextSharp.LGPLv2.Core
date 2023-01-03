namespace iTextSharp.text.pdf;

/// <summary>
///     Summary description for IPdfPCellEvent.
/// </summary>
public interface IPdfPCellEvent
{
    /// <summary>
    ///     This method is called at the end of the cell rendering. The text or graphics are added to
    ///     one of the 4  PdfContentByte  contained in
    ///     canvases .
    ///     The indexes to  canvases  are:
    ///     PdfPTable.BASECANVAS  - the original  PdfContentByte . Anything placed here
    ///     will be under the cell.
    ///     PdfPTable.BACKGROUNDCANVAS  - the layer where the background goes to.
    ///     PdfPTable.LINECANVAS  - the layer where the lines go to.
    ///     PdfPTable.TEXTCANVAS  - the layer where the text go to. Anything placed here
    ///     will be over the cell.
    ///     The layers are placed in sequence on top of each other.
    /// </summary>
    /// <param name="cell">the cell</param>
    /// <param name="position">the coordinates of the cell</param>
    /// <param name="canvases">an array of  PdfContentByte </param>
    void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases);
}