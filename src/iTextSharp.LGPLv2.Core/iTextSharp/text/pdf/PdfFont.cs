using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfFont  is the Pdf Font object.
///     Limitation: in this class only base 14 Type 1 fonts (courier, courier bold, courier oblique,
///     courier boldoblique, helvetica, helvetica bold, helvetica oblique, helvetica boldoblique,
///     symbol, times roman, times bold, times italic, times bolditalic, zapfdingbats) and their
///     standard encoding (standard, MacRoman, (MacExpert,) WinAnsi) are supported.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 7.7 (page 198-203).
///     @see        PdfName
///     @see        PdfDictionary
///     @see        BadPdfFormatException
/// </summary>
public class PdfFont : IComparable
{
    /// <summary>
    ///     the size.
    /// </summary>
    private readonly float _size;

    protected float HScale = 1;

    /// <summary>
    ///     an image.
    /// </summary>
    protected Image image;

    /// <summary>
    ///     constructors
    /// </summary>
    internal PdfFont(BaseFont bf, float size)
    {
        _size = size;
        Font = bf;
    }

    internal static PdfFont DefaultFont
    {
        get
        {
            var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, false);
            return new PdfFont(bf, 12);
        }
    }

    /// <summary>
    ///     the font metrics.
    /// </summary>
    internal BaseFont Font { get; }

    internal float HorizontalScaling
    {
        set => HScale = value;
    }

    internal Image Image
    {
        set => image = value;
    }

    internal float Size
    {
        get
        {
            if (image == null)
            {
                return _size;
            }

            return image.ScaledHeight;
        }
    }

    public int CompareTo(object obj)
    {
        if (image != null)
        {
            return 0;
        }

        if (obj == null)
        {
            return -1;
        }

        PdfFont pdfFont;
        try
        {
            pdfFont = (PdfFont)obj;
            if (Font != pdfFont.Font)
            {
                return 1;
            }

            if (Size.ApproxNotEqual(pdfFont.Size))
            {
                return 2;
            }

            return 0;
        }
        catch (InvalidCastException)
        {
            return -2;
        }
    }

    /// <summary>
    ///     Returns the size of this font.
    /// </summary>
    /// <returns>a size</returns>
    /// <summary>
    ///     Returns the approximative width of 1 character of this font.
    /// </summary>
    /// <returns>a width in Text Space</returns>
    internal float Width() => Width(' ');

    /// <summary>
    ///     Returns the width of a certain character of this font.
    /// </summary>
    /// <param name="character">a certain character</param>
    /// <returns>a width in Text Space</returns>
    internal float Width(int character)
    {
        if (image == null)
        {
            return Font.GetWidthPoint(character, _size) * HScale;
        }

        return image.ScaledWidth;
    }

    internal float Width(string s)
    {
        if (image == null)
        {
            return Font.GetWidthPoint(s, _size) * HScale;
        }

        return image.ScaledWidth;
    }
}