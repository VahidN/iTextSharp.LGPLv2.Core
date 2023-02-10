namespace iTextSharp.text.pdf.interfaces;

/// <summary>
///     A PDF page can have an open and/or close action.
/// </summary>
public interface IPdfPageActions
{
    /// <summary>
    ///     Sets the display duration for the page (for presentations)
    /// </summary>
    int Duration { set; }

    /// <summary>
    ///     Sets the transition for the page
    /// </summary>
    PdfTransition Transition { set; }

    /// <summary>
    ///     Sets the open and close page additional action.
    ///     or  PdfWriter.PAGE_CLOSE
    ///     @throws DocumentException if the action type is invalid
    /// </summary>
    /// <param name="actionType">the action type. It can be  PdfWriter.PAGE_OPEN </param>
    /// <param name="action">the action to perform</param>
    void SetPageAction(PdfName actionType, PdfAction action);
}