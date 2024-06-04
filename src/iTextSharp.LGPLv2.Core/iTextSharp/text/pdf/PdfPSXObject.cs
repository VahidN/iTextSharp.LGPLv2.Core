namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the PostScript XObject.
/// </summary>
public class PdfPsxObject : PdfTemplate
{
    /// <summary>
    ///     Constructs a PSXObject
    /// </summary>
    /// <param name="wr"></param>
    public PdfPsxObject(PdfWriter wr) : base(wr ?? throw new ArgumentNullException(nameof(wr)))
    {
    }

    /// <summary>
    ///     Creates a new instance of PdfPSXObject
    /// </summary>
    protected PdfPsxObject()
    {
    }

    /// <summary>
    /// </summary>
    public override PdfContentByte Duplicate
    {
        get
        {
            var tpl = new PdfPsxObject
                      {
                          Writer = Writer,
                          Pdf = Pdf,
                          ThisReference = ThisReference,
                          pageResources = pageResources,
                          Separator = Separator,
                      };
            return tpl;
        }
    }

    /// <summary>
    ///     Gets the stream representing this object.
    ///     @since   2.1.3   (replacing the method without param compressionLevel)
    ///     @throws IOException
    /// </summary>
    /// <param name="compressionLevel">the compressionLevel</param>
    /// <returns>the stream representing this template</returns>
    internal override PdfStream GetFormXObject(int compressionLevel)
    {
        var s = new PdfStream(Content.ToByteArray());
        s.Put(PdfName.TYPE, PdfName.Xobject);
        s.Put(PdfName.Subtype, PdfName.Ps);
        s.FlateCompress(compressionLevel);
        return s;
    }
}