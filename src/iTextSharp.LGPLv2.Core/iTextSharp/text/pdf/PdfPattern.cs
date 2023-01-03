namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfPattern  defines a ColorSpace
///     @see     PdfStream
/// </summary>
public class PdfPattern : PdfStream
{
    /// <summary>
    ///     Creates a PdfPattern object.
    /// </summary>
    /// <param name="painter">a pattern painter instance</param>
    internal PdfPattern(PdfPatternPainter painter) : this(painter, DEFAULT_COMPRESSION)
    {
    }

    /// <summary>
    ///     Creates a PdfPattern object.
    ///     @since   2.1.3
    /// </summary>
    /// <param name="painter">a pattern painter instance</param>
    /// <param name="compressionLevel">the compressionLevel for the stream</param>
    internal PdfPattern(PdfPatternPainter painter, int compressionLevel)
    {
        var one = new PdfNumber(1);
        var matrix = painter.Matrix;
        if (matrix != null)
        {
            Put(PdfName.Matrix, matrix);
        }

        Put(PdfName.TYPE, PdfName.Pattern);
        Put(PdfName.Bbox, new PdfRectangle(painter.BoundingBox));
        Put(PdfName.Resources, painter.Resources);
        Put(PdfName.Tilingtype, one);
        Put(PdfName.Patterntype, one);
        if (painter.IsStencil())
        {
            Put(PdfName.Painttype, new PdfNumber(2));
        }
        else
        {
            Put(PdfName.Painttype, one);
        }

        Put(PdfName.Xstep, new PdfNumber(painter.XStep));
        Put(PdfName.Ystep, new PdfNumber(painter.YStep));
        Bytes = painter.ToPdf(null);
        Put(PdfName.LENGTH, new PdfNumber(Bytes.Length));
        FlateCompress(compressionLevel);
    }
}