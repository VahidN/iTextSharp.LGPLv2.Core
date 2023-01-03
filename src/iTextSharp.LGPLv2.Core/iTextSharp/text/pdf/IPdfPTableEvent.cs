namespace iTextSharp.text.pdf;

/// <summary>
///     An interface that can be used to retrieve the position of cells in  PdfPTable .
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public interface IPdfPTableEvent
{
    /// <summary>
    ///     This method is called at the end of the table rendering. The text or graphics are added to
    ///     one of the 4  PdfContentByte  contained in
    ///     canvases .
    ///     The indexes to  canvases  are:
    ///     PdfPTable.BASECANVAS  - the original  PdfContentByte . Anything placed here
    ///     will be under the table.
    ///     PdfPTable.BACKGROUNDCANVAS  - the layer where the background goes to.
    ///     PdfPTable.LINECANVAS  - the layer where the lines go to.
    ///     PdfPTable.TEXTCANVAS  - the layer where the text go to. Anything placed here
    ///     will be over the table.
    ///     The layers are placed in sequence on top of each other.
    ///     The  widths  and  heights  have the coordinates of the cells.
    ///     The size of the  widths  array is the number of rows.
    ///     Each sub-array in  widths  corresponds to the x column border positions where
    ///     the first element is the x coordinate of the left table border and the last
    ///     element is the x coordinate of the right table border.
    ///     If colspan is not used all the sub-arrays in  widths
    ///     are the same.
    ///     For the  heights  the first element is the y coordinate of the top table border and the last
    ///     element is the y coordinate of the bottom table border.
    ///     of rows
    ///     of rows + 1
    /// </summary>
    /// <param name="table">the  PdfPTable  in use</param>
    /// <param name="widths">an array of arrays with the cells' x positions. It has the length of the number</param>
    /// <param name="heights">an array with the cells' y positions. It has a length of the number</param>
    /// <param name="headerRows">the number of rows defined for the header.</param>
    /// <param name="rowStart">the first row number after the header</param>
    /// <param name="canvases">an array of  PdfContentByte </param>
    void TableLayout(PdfPTable table, float[][] widths, float[] heights, int headerRows, int rowStart,
                     PdfContentByte[] canvases);
}