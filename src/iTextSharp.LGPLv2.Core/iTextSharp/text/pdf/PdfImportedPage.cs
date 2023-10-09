namespace iTextSharp.text.pdf;

/// <summary>
///     Represents an imported page.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfImportedPage : PdfTemplate
{
    internal readonly int pageNumber;
    internal readonly PdfReaderInstance ReaderInstance;

    internal PdfImportedPage(PdfReaderInstance readerInstance, PdfWriter writer, int pageNumber)
    {
        ReaderInstance = readerInstance;
        this.pageNumber = pageNumber;
        Writer = writer;
        BBox = readerInstance.Reader.GetPageSize(pageNumber);
        SetMatrix(1, 0, 0, 1, -BBox.Left, -BBox.Bottom);
        type = TYPE_IMPORTED;
    }

    /// <summary>
    ///     Always throws an error. This operation is not allowed.
    /// </summary>
    /// <returns>dummy</returns>
    public override PdfContentByte Duplicate
    {
        get
        {
            ThrowError();
            return null;
        }
    }

    /// <summary>
    ///     Reads the content from this  PdfImportedPage -object from a reader.
    /// </summary>
    /// <returns>self</returns>
    public PdfImportedPage FromReader => this;

    public override PdfTransparencyGroup Group
    {
        set => ThrowError();
    }

    public int PageNumber => pageNumber;

    internal PdfReaderInstance PdfReaderInstance => ReaderInstance;

    internal override PdfObject Resources => ReaderInstance.GetResources(pageNumber);

    /// <summary>
    ///     Always throws an error. This operation is not allowed.
    ///     @throws DocumentException  dummy
    /// </summary>
    /// <param name="image">dummy</param>
    /// <param name="a">dummy</param>
    /// <param name="b">dummy</param>
    /// <param name="c">dummy</param>
    /// <param name="d">dummy</param>
    /// <param name="e">dummy</param>
    /// <param name="f">dummy</param>
    public override void AddImage(Image image, float a, float b, float c, float d, float e, float f)
    {
        ThrowError();
    }

    /// <summary>
    ///     Always throws an error. This operation is not allowed.
    /// </summary>
    /// <param name="pdfTemplate">dummy</param>
    /// <param name="a">dummy</param>
    /// <param name="b">dummy</param>
    /// <param name="c">dummy</param>
    /// <param name="d">dummy</param>
    /// <param name="e">dummy</param>
    /// <param name="f">dummy</param>
    public override void AddTemplate(PdfTemplate pdfTemplate, float a, float b, float c, float d, float e, float f)
    {
        ThrowError();
    }

    public override void SetColorFill(PdfSpotColor sp, float tint)
    {
        ThrowError();
    }

    public override void SetColorStroke(PdfSpotColor sp, float tint)
    {
        ThrowError();
    }

    /// <summary>
    ///     Always throws an error. This operation is not allowed.
    /// </summary>
    /// <param name="bf">dummy</param>
    /// <param name="size">dummy</param>
    public override void SetFontAndSize(BaseFont bf, float size)
    {
        ThrowError();
    }

    /// <summary>
    ///     Gets the stream representing this page.
    ///     @since   2.1.3   (replacing the method without param compressionLevel)
    /// </summary>
    /// <param name="compressionLevel">the compressionLevel</param>
    /// <returns>the stream representing this page</returns>
    internal override PdfStream GetFormXObject(int compressionLevel) =>
        ReaderInstance.GetFormXObject(pageNumber, compressionLevel);

    internal static void ThrowError()
    {
        throw new InvalidOperationException("Content can not be added to a PdfImportedPage.");
    }
}