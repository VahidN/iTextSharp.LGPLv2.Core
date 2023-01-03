namespace iTextSharp.text.pdf.interfaces;

/// <summary>
///     A PDF document can have an open action and other additional actions.
/// </summary>
public interface IPdfDocumentActions
{
    /// <summary>
    ///     Additional-actions defining the actions to be taken in
    ///     response to various trigger events affecting the document
    ///     as a whole. The actions types allowed are:  DOCUMENT_CLOSE ,
    ///     WILL_SAVE ,  DID_SAVE ,  WILL_PRINT
    ///     and  DID_PRINT .
    ///     @throws DocumentException on invalid action type
    /// </summary>
    /// <param name="actionType">the action type</param>
    /// <param name="action">the action to execute in response to the trigger</param>
    void SetAdditionalAction(PdfName actionType, PdfAction action);

    /// <summary>
    ///     When the document opens it will jump to the destination with
    ///     this name.
    /// </summary>
    /// <param name="name">the name of the destination to jump to</param>
    void SetOpenAction(string name);

    /// <summary>
    ///     When the document opens this  action  will be
    ///     invoked.
    /// </summary>
    /// <param name="action">the action to be invoked</param>
    void SetOpenAction(PdfAction action);
}