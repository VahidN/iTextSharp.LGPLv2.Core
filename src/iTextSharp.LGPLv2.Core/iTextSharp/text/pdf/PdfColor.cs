namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfColor  defines a Color (it's a  PdfArray  containing 3 values).
///     @see        PdfDictionary
/// </summary>
internal class PdfColor : PdfArray
{
    /// <summary>
    ///     Constructs a new  PdfColor .
    /// </summary>
    /// <param name="red">a value between 0 and 255</param>
    /// <param name="green">a value between 0 and 255</param>
    /// <param name="blue">a value between 0 and 255</param>
    internal PdfColor(int red, int green, int blue) : base(new PdfNumber((double)(red & 0xFF) / 0xFF))
    {
        Add(new PdfNumber((double)(green & 0xFF) / 0xFF));
        Add(new PdfNumber((double)(blue & 0xFF) / 0xFF));
    }

    internal PdfColor(BaseColor color) : this(color.R, color.G, color.B)
    {
    }
}