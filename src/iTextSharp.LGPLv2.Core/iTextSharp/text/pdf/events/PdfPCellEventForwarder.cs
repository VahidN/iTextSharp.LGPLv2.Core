namespace iTextSharp.text.pdf.events;

/// <summary>
///     If you want to add more than one event to a cell,
///     you have to construct a PdfPCellEventForwarder, add the
///     different events to this object and add the forwarder to
///     the PdfPCell.
/// </summary>
public class PdfPCellEventForwarder : IPdfPCellEvent
{
    /// <summary>
    ///     ArrayList containing all the PageEvents that have to be executed.
    /// </summary>
    protected List<IPdfPCellEvent> Events = new();

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfPCellEvent#cellLayout(com.lowagie.text.pdf.PdfPCell, com.lowagie.text.Rectangle,
    ///     com.lowagie.text.pdf.PdfContentByte[])
    /// </summary>
    public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
    {
        foreach (var eventa in Events)
        {
            eventa.CellLayout(cell, position, canvases);
        }
    }

    /// <summary>
    ///     Add a page event to the forwarder.
    /// </summary>
    /// <param name="eventa">an event that has to be added to the forwarder.</param>
    public void AddCellEvent(IPdfPCellEvent eventa)
    {
        Events.Add(eventa);
    }
}