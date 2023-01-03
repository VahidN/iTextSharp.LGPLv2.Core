namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfBorderArray  defines the border of a  PdfAnnotation .
///     @see        PdfArray
/// </summary>
public class PdfBorderArray : PdfArray
{
    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a new  PdfBorderArray .
    /// </summary>
    public PdfBorderArray(float hRadius, float vRadius, float width) : this(hRadius, vRadius, width, null)
    {
    }

    /// <summary>
    ///     Constructs a new  PdfBorderArray .
    /// </summary>
    public PdfBorderArray(float hRadius, float vRadius, float width, PdfDashPattern dash) : base(new PdfNumber(hRadius))
    {
        Add(new PdfNumber(vRadius));
        Add(new PdfNumber(width));
        if (dash != null)
        {
            Add(dash);
        }
    }
}