namespace iTextSharp.text.pdf;

/// <summary>
///     PdfFormObject  is a type of XObject containing a template-object.
/// </summary>
public class PdfFormXObject : PdfStream
{
    /// <summary>
    ///     public static variables
    /// </summary>
    /// <summary>
    ///     This is the 1 - matrix.
    /// </summary>
    public static PdfLiteral Matrix = new("[1 0 0 1 0 0]");

    /// <summary>
    ///     This is a PdfNumber representing 1.
    /// </summary>
    public static PdfNumber One = new(1);

    /// <summary>
    ///     This is a PdfNumber representing 0.
    /// </summary>
    public static PdfNumber Zero = new(0);

    /// <summary>
    ///     Constructs a  PdfFormXObject -object.
    ///     @since   2.1.3 (Replacing the existing constructor with param compressionLevel)
    /// </summary>
    /// <param name="template">the template</param>
    /// <param name="compressionLevel">the compression level for the stream</param>
    internal PdfFormXObject(PdfTemplate template, int compressionLevel)
    {
        Put(PdfName.TYPE, PdfName.Xobject);
        Put(PdfName.Subtype, PdfName.Form);
        Put(PdfName.Resources, template.Resources);
        Put(PdfName.Bbox, new PdfRectangle(template.BoundingBox));
        Put(PdfName.Formtype, One);
        var matrix = template.Matrix;
        if (template.Layer != null)
        {
            Put(PdfName.Oc, template.Layer.Ref);
        }

        if (template.Group != null)
        {
            Put(PdfName.Group, template.Group);
        }

        if (matrix == null)
        {
            Put(PdfName.Matrix, Matrix);
        }
        else
        {
            Put(PdfName.Matrix, matrix);
        }

        Bytes = template.ToPdf(null);
        Put(PdfName.LENGTH, new PdfNumber(Bytes.Length));
        FlateCompress(compressionLevel);
    }
}