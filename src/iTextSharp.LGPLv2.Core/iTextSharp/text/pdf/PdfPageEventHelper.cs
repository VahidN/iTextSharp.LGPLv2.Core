namespace iTextSharp.text.pdf;

/// <summary>
///     Helps the use of  PdfPageEvent  by implementing all the interface methods.
///     A class can extend  PdfPageEventHelper  and only implement the
///     needed methods.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfPageEventHelper : IPdfPageEvent
{
    /// <summary>
    ///     Called when a Chapter is written.
    ///     position  will hold the height at which the
    ///     chapter will be written to.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    /// <param name="paragraphPosition">the position the chapter will be written to</param>
    /// <param name="title">the title of the Chapter</param>
    public virtual void OnChapter(PdfWriter writer, Document document, float paragraphPosition, Paragraph title)
    {
    }

    /// <summary>
    ///     Called when the end of a Chapter is reached.
    ///     position  will hold the height of the end of the chapter.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    /// <param name="paragraphPosition">the position of the end of the chapter.</param>
    public virtual void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition)
    {
    }

    /// <summary>
    ///     Called when the document is closed.
    ///     Note that this method is called with the page number equal
    ///     to the last page plus one.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    public virtual void OnCloseDocument(PdfWriter writer, Document document)
    {
    }

    /// <summary>
    ///     Called when a page is finished, just before being written to the document.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    public virtual void OnEndPage(PdfWriter writer, Document document)
    {
    }

    /// <summary>
    ///     Called when a  Chunk  with a generic tag is written.
    ///     It is usefull to pinpoint the  Chunk  location to generate
    ///     bookmarks, for example.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    /// <param name="rect">the  Rectangle  containing the  Chunk </param>
    /// <param name="text">the text of the tag</param>
    public virtual void OnGenericTag(PdfWriter writer, Document document, Rectangle rect, string text)
    {
    }

    /// <summary>
    ///     Called when the document is opened.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    public virtual void OnOpenDocument(PdfWriter writer, Document document)
    {
    }

    /// <summary>
    ///     Called when a Paragraph is written.
    ///     paragraphPosition  will hold the height at which the
    ///     paragraph will be written to. This is useful to insert bookmarks with
    ///     more control.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    /// <param name="paragraphPosition">the position the paragraph will be written to</param>
    public virtual void OnParagraph(PdfWriter writer, Document document, float paragraphPosition)
    {
    }

    /// <summary>
    ///     Called when a Paragraph is written.
    ///     paragraphPosition  will hold the height of the end of the paragraph.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    /// <param name="paragraphPosition">the position of the end of the paragraph</param>
    public virtual void OnParagraphEnd(PdfWriter writer, Document document, float paragraphPosition)
    {
    }

    /// <summary>
    ///     Called when a Section is written.
    ///     position  will hold the height at which the
    ///     section will be written to.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    /// <param name="paragraphPosition">the position the chapter will be written to</param>
    /// <param name="depth"></param>
    /// <param name="title">the title of the Chapter</param>
    public virtual void OnSection(PdfWriter writer, Document document, float paragraphPosition, int depth,
                                  Paragraph title)
    {
    }

    /// <summary>
    ///     Called when the end of a Section is reached.
    ///     position  will hold the height of the section end.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    /// <param name="paragraphPosition">the position of the end of the section</param>
    public virtual void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition)
    {
    }

    /// <summary>
    ///     Called when a page is initialized.
    ///     Note that if even if a page is not written this method is still
    ///     called. It is preferable to use  onEndPage  to avoid
    ///     infinite loops.
    /// </summary>
    /// <param name="writer">the  PdfWriter  for this document</param>
    /// <param name="document">the document</param>
    public virtual void OnStartPage(PdfWriter writer, Document document)
    {
    }
}