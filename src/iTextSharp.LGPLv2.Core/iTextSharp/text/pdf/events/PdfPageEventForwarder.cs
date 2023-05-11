namespace iTextSharp.text.pdf.events;

/// <summary>
///     If you want to add more than one page eventa to a PdfWriter,
///     you have to construct a PdfPageEventForwarder, add the
///     different events to this object and add the forwarder to
///     the PdfWriter.
/// </summary>
public class PdfPageEventForwarder : IPdfPageEvent
{
    /// <summary>
    ///     ArrayList containing all the PageEvents that have to be executed.
    /// </summary>
    protected List<IPdfPageEvent> Events = new();

    /// <summary>
    ///     Called when a Chapter is written.
    ///     position  will hold the height at which the chapter will be
    ///     written to.
    ///     the  PdfWriter  for this document
    ///     the document
    ///     the position the chapter will be written to
    ///     the title of the Chapter
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    /// <param name="paragraphPosition"></param>
    /// <param name="title"></param>
    public virtual void OnChapter(PdfWriter writer, Document document,
                                  float paragraphPosition, Paragraph title)
    {
        foreach (var eventa in Events)
        {
            eventa.OnChapter(writer, document, paragraphPosition, title);
        }
    }

    /// <summary>
    ///     Called when the end of a Chapter is reached.
    ///     position  will hold the height of the end of the chapter.
    ///     the  PdfWriter  for this document
    ///     the document
    ///     the position of the end of the chapter.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    /// <param name="paragraphPosition"></param>
    public virtual void OnChapterEnd(PdfWriter writer, Document document, float paragraphPosition)
    {
        foreach (var eventa in Events)
        {
            eventa.OnChapterEnd(writer, document, paragraphPosition);
        }
    }

    /// <summary>
    ///     Called when the document is closed.
    ///     Note that this method is called with the page number equal to the last
    ///     page plus one.
    ///     the  PdfWriter  for this document
    ///     the document
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    public virtual void OnCloseDocument(PdfWriter writer, Document document)
    {
        foreach (var eventa in Events)
        {
            eventa.OnCloseDocument(writer, document);
        }
    }

    /// <summary>
    ///     Called when a page is finished, just before being written to the
    ///     document.
    ///     the  PdfWriter  for this document
    ///     the document
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    public virtual void OnEndPage(PdfWriter writer, Document document)
    {
        foreach (var eventa in Events)
        {
            eventa.OnEndPage(writer, document);
        }
    }

    /// <summary>
    ///     Called when a  Chunk  with a generic tag is written.
    ///     It is usefull to pinpoint the  Chunk  location to generate
    ///     bookmarks, for example.
    ///     the  PdfWriter  for this document
    ///     the document
    ///     the  Rectangle  containing the  Chunk
    ///     the text of the tag
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    /// <param name="rect"></param>
    /// <param name="text"></param>
    public virtual void OnGenericTag(PdfWriter writer, Document document,
                                     Rectangle rect, string text)
    {
        foreach (var eventa in Events)
        {
            eventa.OnGenericTag(writer, document, rect, text);
        }
    }

    /// <summary>
    ///     Called when the document is opened.
    ///     the  PdfWriter  for this document
    ///     the document
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    public virtual void OnOpenDocument(PdfWriter writer, Document document)
    {
        foreach (var eventa in Events)
        {
            eventa.OnOpenDocument(writer, document);
        }
    }

    /// <summary>
    ///     Called when a Paragraph is written.
    ///     paragraphPosition  will hold the height at which the
    ///     paragraph will be written to. This is useful to insert bookmarks with
    ///     more control.
    ///     the  PdfWriter  for this document
    ///     the document
    ///     the position the paragraph will be written to
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    /// <param name="paragraphPosition"></param>
    public virtual void OnParagraph(PdfWriter writer, Document document,
                                    float paragraphPosition)
    {
        foreach (var eventa in Events)
        {
            eventa.OnParagraph(writer, document, paragraphPosition);
        }
    }

    /// <summary>
    ///     Called when a Paragraph is written.
    ///     paragraphPosition  will hold the height of the end of the
    ///     paragraph.
    ///     the  PdfWriter  for this document
    ///     the document
    ///     the position of the end of the paragraph
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    /// <param name="paragraphPosition"></param>
    public virtual void OnParagraphEnd(PdfWriter writer, Document document,
                                       float paragraphPosition)
    {
        foreach (var eventa in Events)
        {
            eventa.OnParagraphEnd(writer, document, paragraphPosition);
        }
    }

    /// <summary>
    ///     Called when a Section is written.
    ///     position  will hold the height at which the section will be
    ///     written to.
    ///     the  PdfWriter  for this document
    ///     the document
    ///     the position the section will be written to
    ///     the number depth of the Section
    ///     the title of the section
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    /// <param name="paragraphPosition"></param>
    /// <param name="depth"></param>
    /// <param name="title"></param>
    public virtual void OnSection(PdfWriter writer, Document document,
                                  float paragraphPosition, int depth, Paragraph title)
    {
        foreach (var eventa in Events)
        {
            eventa.OnSection(writer, document, paragraphPosition, depth, title);
        }
    }

    /// <summary>
    ///     Called when the end of a Section is reached.
    ///     position  will hold the height of the section end.
    ///     the  PdfWriter  for this document
    ///     the document
    ///     the position of the end of the section
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    /// <param name="paragraphPosition"></param>
    public virtual void OnSectionEnd(PdfWriter writer, Document document, float paragraphPosition)
    {
        foreach (var eventa in Events)
        {
            eventa.OnSectionEnd(writer, document, paragraphPosition);
        }
    }

    /// <summary>
    ///     Called when a page is initialized.
    ///     Note that if even if a page is not written this method is still called.
    ///     It is preferable to use  onEndPage  to avoid infinite loops.
    ///     the  PdfWriter  for this document
    ///     the document
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="document"></param>
    public virtual void OnStartPage(PdfWriter writer, Document document)
    {
        foreach (var eventa in Events)
        {
            eventa.OnStartPage(writer, document);
        }
    }

    /// <summary>
    ///     Add a page eventa to the forwarder.
    /// </summary>
    /// <param name="eventa">an eventa that has to be added to the forwarder.</param>
    public void AddPageEvent(IPdfPageEvent eventa)
    {
        Events.Add(eventa);
    }
}