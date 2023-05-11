using System.Drawing;
using System.Text;
using iTextSharp.LGPLv2.Core.System.Drawing;
using SkiaSharp;

namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the code interleaved 2 of 5. The text can include
///     non numeric characters that are printed but do not generate bars.
///     The default parameters are:
///     x = 0.8f;
///     n = 2;
///     font = BaseFont.CreateFont("Helvetica", "winansi", false);
///     size = 8;
///     baseline = size;
///     barHeight = size * 3;
///     textint= Element.ALIGN_CENTER;
///     generateChecksum = false;
///     checksumText = false;
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class BarcodeInter25 : Barcode
{
    /// <summary>
    ///     The bars to generate the code.
    /// </summary>
    private static readonly byte[][] _bars =
    {
        new byte[] { 0, 0, 1, 1, 0 },
        new byte[] { 1, 0, 0, 0, 1 },
        new byte[] { 0, 1, 0, 0, 1 },
        new byte[] { 1, 1, 0, 0, 0 },
        new byte[] { 0, 0, 1, 0, 1 },
        new byte[] { 1, 0, 1, 0, 0 },
        new byte[] { 0, 1, 1, 0, 0 },
        new byte[] { 0, 0, 0, 1, 1 },
        new byte[] { 1, 0, 0, 1, 0 },
        new byte[] { 0, 1, 0, 1, 0 },
    };

    /// <summary>
    ///     Creates new BarcodeInter25
    /// </summary>
    public BarcodeInter25()
    {
        x = 0.8f;
        n = 2;
        font = BaseFont.CreateFont("Helvetica", "winansi", false);
        size = 8;
        baseline = size;
        barHeight = size * 3;
        textAlignment = Element.ALIGN_CENTER;
        generateChecksum = false;
        checksumText = false;
    }

    /// <summary>
    ///     Gets the maximum area that the barcode and the text, if
    ///     any, will occupy. The lower left corner is always (0, 0).
    /// </summary>
    /// <returns>the size the barcode occupies.</returns>
    public override Rectangle BarcodeSize
    {
        get
        {
            float fontX = 0;
            float fontY = 0;
            if (font != null)
            {
                if (baseline > 0)
                {
                    fontY = baseline - font.GetFontDescriptor(BaseFont.DESCENT, size);
                }
                else
                {
                    fontY = -baseline + size;
                }

                var fullCode = code;
                if (generateChecksum && checksumText)
                {
                    fullCode += GetChecksum(fullCode);
                }

                fontX = font.GetWidthPoint(altText != null ? altText : fullCode, size);
            }

            var fCode = KeepNumbers(code);
            var len = fCode.Length;
            if (generateChecksum)
            {
                ++len;
            }

            var fullWidth = len * (3 * x + 2 * x * n) + (6 + n) * x;
            fullWidth = Math.Max(fullWidth, fontX);
            var fullHeight = barHeight + fontY;
            return new Rectangle(fullWidth, fullHeight);
        }
    }

    /// <summary>
    ///     Creates the bars for the barcode.
    /// </summary>
    /// <param name="text">the text. It can contain non numeric characters</param>
    /// <returns>the barcode</returns>
    public static byte[] GetBarsInter25(string text)
    {
        text = KeepNumbers(text);
        if ((text.Length & 1) != 0)
        {
            throw new ArgumentException("The text length must be even.");
        }

        var bars = new byte[text.Length * 5 + 7];
        var pb = 0;
        bars[pb++] = 0;
        bars[pb++] = 0;
        bars[pb++] = 0;
        bars[pb++] = 0;
        var len = text.Length / 2;
        for (var k = 0; k < len; ++k)
        {
            var c1 = text[k * 2] - '0';
            var c2 = text[k * 2 + 1] - '0';
            var b1 = _bars[c1];
            var b2 = _bars[c2];
            for (var j = 0; j < 5; ++j)
            {
                bars[pb++] = b1[j];
                bars[pb++] = b2[j];
            }
        }

        bars[pb++] = 1;
        bars[pb++] = 0;
        bars[pb++] = 0;
        return bars;
    }

    /// <summary>
    ///     Calculates the checksum.
    /// </summary>
    /// <param name="text">the numeric text</param>
    /// <returns>the checksum</returns>
    public static char GetChecksum(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var mul = 3;
        var total = 0;
        for (var k = text.Length - 1; k >= 0; --k)
        {
            var n = text[k] - '0';
            total += mul * n;
            mul ^= 2;
        }

        return (char)((10 - total % 10) % 10 + '0');
    }

    /// <summary>
    ///     Deletes all the non numeric characters from  text .
    /// </summary>
    /// <param name="text">the text</param>
    /// <returns>a  string  with only numeric characters</returns>
    public static string KeepNumbers(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var sb = new StringBuilder();
        for (var k = 0; k < text.Length; ++k)
        {
            var c = text[k];
            if (c >= '0' && c <= '9')
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }

    public override SKBitmap CreateDrawingImage(Color foreground, Color background)
    {
        var bCode = KeepNumbers(code);
        if (generateChecksum)
        {
            bCode += GetChecksum(bCode);
        }

        var len = bCode.Length;
        var nn = (int)n;
        var fullWidth = len * (3 + 2 * nn) + 6 + nn;
        var bars = GetBarsInter25(bCode);
        var height = (int)barHeight;
        var bmp = new SKBitmap(fullWidth, height);
        for (var h = 0; h < height; ++h)
        {
            var print = true;
            var ptr = 0;
            for (var k = 0; k < bars.Length; ++k)
            {
                var w = bars[k] == 0 ? 1 : nn;
                var c = background;
                if (print)
                {
                    c = foreground;
                }

                print = !print;
                for (var j = 0; j < w; ++j)
                {
                    bmp.SetPixel(ptr++, h, c.ToSKColor());
                }
            }
        }

        return bmp;
    }

    /// <summary>
    ///     Places the barcode in a  PdfContentByte . The
    ///     barcode is always placed at coodinates (0, 0). Use the
    ///     translation matrix to move it elsewhere.
    ///     The bars and text are written in the following colors:
    ///     barColor
    ///     textColor
    ///     Result
    ///     null
    ///     null
    ///     bars and text painted with current fill color
    ///     barColor
    ///     null
    ///     bars and text painted with  barColor
    ///     null
    ///     textColor
    ///     bars painted with current color text painted with  textColor
    ///     barColor
    ///     textColor
    ///     bars painted with  barColor  text painted with  textColor
    /// </summary>
    /// <param name="cb">the  PdfContentByte  where the barcode will be placed</param>
    /// <param name="barColor">the color of the bars. It can be  null </param>
    /// <param name="textColor">the color of the text. It can be  null </param>
    /// <returns>the dimensions the barcode occupies</returns>
    public override Rectangle PlaceBarcode(PdfContentByte cb, BaseColor barColor, BaseColor textColor)
    {
        if (cb == null)
        {
            throw new ArgumentNullException(nameof(cb));
        }

        var fullCode = code;
        float fontX = 0;
        if (font != null)
        {
            if (generateChecksum && checksumText)
            {
                fullCode += GetChecksum(fullCode);
            }

            fontX = font.GetWidthPoint(fullCode = altText != null ? altText : fullCode, size);
        }

        var bCode = KeepNumbers(code);
        if (generateChecksum)
        {
            bCode += GetChecksum(bCode);
        }

        var len = bCode.Length;
        var fullWidth = len * (3 * x + 2 * x * n) + (6 + n) * x;
        float barStartX = 0;
        float textStartX = 0;
        switch (textAlignment)
        {
            case Element.ALIGN_LEFT:
                break;
            case Element.ALIGN_RIGHT:
                if (fontX > fullWidth)
                {
                    barStartX = fontX - fullWidth;
                }
                else
                {
                    textStartX = fullWidth - fontX;
                }

                break;
            default:
                if (fontX > fullWidth)
                {
                    barStartX = (fontX - fullWidth) / 2;
                }
                else
                {
                    textStartX = (fullWidth - fontX) / 2;
                }

                break;
        }

        float barStartY = 0;
        float textStartY = 0;
        if (font != null)
        {
            if (baseline <= 0)
            {
                textStartY = barHeight - baseline;
            }
            else
            {
                textStartY = -font.GetFontDescriptor(BaseFont.DESCENT, size);
                barStartY = textStartY + baseline;
            }
        }

        var bars = GetBarsInter25(bCode);
        var print = true;
        if (barColor != null)
        {
            cb.SetColorFill(barColor);
        }

        for (var k = 0; k < bars.Length; ++k)
        {
            var w = bars[k] == 0 ? x : x * n;
            if (print)
            {
                cb.Rectangle(barStartX, barStartY, w - inkSpreading, barHeight);
            }

            print = !print;
            barStartX += w;
        }

        cb.Fill();
        if (font != null)
        {
            if (textColor != null)
            {
                cb.SetColorFill(textColor);
            }

            cb.BeginText();
            cb.SetFontAndSize(font, size);
            cb.SetTextMatrix(textStartX, textStartY);
            cb.ShowText(fullCode);
            cb.EndText();
        }

        return BarcodeSize;
    }
}