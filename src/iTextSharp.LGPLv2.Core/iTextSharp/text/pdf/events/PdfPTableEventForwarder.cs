namespace iTextSharp.text.pdf.events;

/// <summary>
///     If you want to add more than one page event to a PdfPTable,
///     you have to construct a PdfPTableEventForwarder, add the
///     different events to this object and add the forwarder to
///     the PdfWriter.
/// </summary>
public class PdfPTableEventForwarder : IPdfPTableEvent
{
    /// <summary>
    ///     ArrayList containing all the PageEvents that have to be executed.
    /// </summary>
    protected List<IPdfPTableEvent> Events = new();

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfPTableEvent#tableLayout(com.lowagie.text.pdf.PdfPTable, float[][], float[], int, int,
    ///     com.lowagie.text.pdf.PdfContentByte[])
    /// </summary>
    public void TableLayout(PdfPTable table, float[][] widths, float[] heights, int headerRows, int rowStart,
                            PdfContentByte[] canvases)
    {
        foreach (var eventa in Events)
        {
            eventa.TableLayout(table, widths, heights, headerRows, rowStart, canvases);
        }
    }

    /// <summary>
    ///     Add a page event to the forwarder.
    /// </summary>
    /// <param name="eventa">an event that has to be added to the forwarder.</param>
    public void AddTableEvent(IPdfPTableEvent eventa)
    {
        Events.Add(eventa);
    }
}