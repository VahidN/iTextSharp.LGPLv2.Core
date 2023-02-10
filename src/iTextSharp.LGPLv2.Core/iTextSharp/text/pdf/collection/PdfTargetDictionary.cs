namespace iTextSharp.text.pdf.collection;

public class PdfTargetDictionary : PdfDictionary
{
    /// <summary>
    ///     Creates dictionary referring to a target document that is the parent of the current document.
    /// </summary>
    /// <param name="nested">null if this is the actual target, another target if this is only an intermediate target.</param>
    public PdfTargetDictionary(PdfTargetDictionary nested)
    {
        Put(PdfName.R, PdfName.P);
        if (nested != null)
        {
            AdditionalPath = nested;
        }
    }

    /// <summary>
    ///     Creates a dictionary referring to a target document.
    /// </summary>
    /// <param name="child">
    ///     if false, this refers to the parent document; if true, this refers to a child document, and you'll
    ///     have to specify where to find the child using the other methods of this class
    /// </param>
    public PdfTargetDictionary(bool child)
    {
        if (child)
        {
            Put(PdfName.R, PdfName.C);
        }
        else
        {
            Put(PdfName.R, PdfName.P);
        }
    }

    /// <summary>
    ///     If this dictionary refers to an intermediate target, you can
    ///     add the next target in the sequence.
    /// </summary>
    public PdfTargetDictionary AdditionalPath
    {
        set => Put(PdfName.T, value);
    }

    /// <summary>
    ///     If this dictionary refers to a child that is a document level attachment,
    ///     you need to specify the name that was used to attach the document.
    /// </summary>
    public string EmbeddedFileName
    {
        set => Put(PdfName.N, new PdfString(value, null));
    }

    /// <summary>
    ///     If this dictionary refers to a child that is a file attachment added to a page,
    ///     you need to specify the page with setFileAttachmentPage or setFileAttachmentPageName,
    ///     and then specify the index of the attachment added to this page (or use setFileAttachmentName).
    /// </summary>
    public int FileAttachmentIndex
    {
        set => Put(PdfName.A, new PdfNumber(value));
    }

    /// <summary>
    ///     If this dictionary refers to a child that is a file attachment added to a page,
    ///     you need to specify the page with setFileAttachmentPage or setFileAttachmentPageName,
    ///     and then specify the name of the attachment added to this page (or use setFileAttachmentIndex).
    /// </summary>
    public string FileAttachmentName
    {
        set => Put(PdfName.A, new PdfString(value, TEXT_UNICODE));
    }

    /// <summary>
    ///     If this dictionary refers to a child that is a file attachment added to a page,
    ///     you need to specify the page number (or use setFileAttachmentPagename to specify a named destination).
    ///     Once you have specified the page, you still need to specify the attachment using another method.
    /// </summary>
    public int FileAttachmentPage
    {
        set => Put(PdfName.P, new PdfNumber(value));
    }

    /// <summary>
    ///     If this dictionary refers to a child that is a file attachment added to a page,
    ///     you need to specify the name of the page (or use setFileAttachmentPage to specify the page number).
    ///     Once you have specified the page, you still need to specify the attachment using another method.
    /// </summary>
    public string FileAttachmentPagename
    {
        set => Put(PdfName.P, new PdfString(value, null));
    }
}