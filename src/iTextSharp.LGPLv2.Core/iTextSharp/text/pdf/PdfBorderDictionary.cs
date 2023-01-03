namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfBorderDictionary  define the appearance of a Border (Annotations).
///     @see     PdfDictionary
/// </summary>
public class PdfBorderDictionary : PdfDictionary
{
    public const int STYLE_BEVELED = 2;
    public const int STYLE_DASHED = 1;
    public const int STYLE_INSET = 3;
    public const int STYLE_SOLID = 0;
    public const int STYLE_UNDERLINE = 4;

    /// <summary>
    ///     Constructs a  PdfBorderDictionary .
    /// </summary>
    public PdfBorderDictionary(float borderWidth, int borderStyle, PdfDashPattern dashes)
    {
        Put(PdfName.W, new PdfNumber(borderWidth));
        switch (borderStyle)
        {
            case STYLE_SOLID:
                Put(PdfName.S, PdfName.S);
                break;
            case STYLE_DASHED:
                if (dashes != null)
                {
                    Put(PdfName.D, dashes);
                }

                Put(PdfName.S, PdfName.D);
                break;
            case STYLE_BEVELED:
                Put(PdfName.S, PdfName.B);
                break;
            case STYLE_INSET:
                Put(PdfName.S, PdfName.I);
                break;
            case STYLE_UNDERLINE:
                Put(PdfName.S, PdfName.U);
                break;
            default:
                throw new ArgumentException("Invalid border style.");
        }
    }

    public PdfBorderDictionary(float borderWidth, int borderStyle) : this(borderWidth, borderStyle, null)
    {
    }
}