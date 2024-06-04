using System.Drawing;
using iTextSharp.LGPLv2.Core.System.Drawing;
using SkiaSharp;

namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the code 39 and code 39 extended. The default parameters are:
///     x = 0.8f;
///     n = 2;
///     font = BaseFont.CreateFont("Helvetica", "winansi", false);
///     size = 8;
///     baseline = size;
///     barHeight = size * 3;
///     textint= Element.ALIGN_CENTER;
///     generateChecksum = false;
///     checksumText = false;
///     startStopText = true;
///     extended = false;
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class Barcode39 : Barcode
{
    /// <summary>
    ///     The index chars to  BARS .
    /// </summary>
    private const string Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%*";

    /// <summary>
    ///     The character combinations to make the code 39 extended.
    /// </summary>
    private const string EXTENDED = "%U" +
                                    "$A$B$C$D$E$F$G$H$I$J$K$L$M$N$O$P$Q$R$S$T$U$V$W$X$Y$Z" +
                                    "%A%B%C%D%E  /A/B/C/D/E/F/G/H/I/J/K/L - ./O" +
                                    " 0 1 2 3 4 5 6 7 8 9/Z%F%G%H%I%J%V" +
                                    " A B C D E F G H I J K L M N O P Q R S T U V W X Y Z" +
                                    "%K%L%M%N%O%W" +
                                    "+A+B+C+D+E+F+G+H+I+J+K+L+M+N+O+P+Q+R+S+T+U+V+W+X+Y+Z" +
                                    "%P%Q%R%S%T";

    /// <summary>
    ///     The bars to generate the code.
    /// </summary>
    private static readonly byte[][] _bars =
    {
        new byte[] { 0, 0, 0, 1, 1, 0, 1, 0, 0 },
        new byte[] { 1, 0, 0, 1, 0, 0, 0, 0, 1 },
        new byte[] { 0, 0, 1, 1, 0, 0, 0, 0, 1 },
        new byte[] { 1, 0, 1, 1, 0, 0, 0, 0, 0 },
        new byte[] { 0, 0, 0, 1, 1, 0, 0, 0, 1 },
        new byte[] { 1, 0, 0, 1, 1, 0, 0, 0, 0 },
        new byte[] { 0, 0, 1, 1, 1, 0, 0, 0, 0 },
        new byte[] { 0, 0, 0, 1, 0, 0, 1, 0, 1 },
        new byte[] { 1, 0, 0, 1, 0, 0, 1, 0, 0 },
        new byte[] { 0, 0, 1, 1, 0, 0, 1, 0, 0 },
        new byte[] { 1, 0, 0, 0, 0, 1, 0, 0, 1 },
        new byte[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 },
        new byte[] { 1, 0, 1, 0, 0, 1, 0, 0, 0 },
        new byte[] { 0, 0, 0, 0, 1, 1, 0, 0, 1 },
        new byte[] { 1, 0, 0, 0, 1, 1, 0, 0, 0 },
        new byte[] { 0, 0, 1, 0, 1, 1, 0, 0, 0 },
        new byte[] { 0, 0, 0, 0, 0, 1, 1, 0, 1 },
        new byte[] { 1, 0, 0, 0, 0, 1, 1, 0, 0 },
        new byte[] { 0, 0, 1, 0, 0, 1, 1, 0, 0 },
        new byte[] { 0, 0, 0, 0, 1, 1, 1, 0, 0 },
        new byte[] { 1, 0, 0, 0, 0, 0, 0, 1, 1 },
        new byte[] { 0, 0, 1, 0, 0, 0, 0, 1, 1 },
        new byte[] { 1, 0, 1, 0, 0, 0, 0, 1, 0 },
        new byte[] { 0, 0, 0, 0, 1, 0, 0, 1, 1 },
        new byte[] { 1, 0, 0, 0, 1, 0, 0, 1, 0 },
        new byte[] { 0, 0, 1, 0, 1, 0, 0, 1, 0 },
        new byte[] { 0, 0, 0, 0, 0, 0, 1, 1, 1 },
        new byte[] { 1, 0, 0, 0, 0, 0, 1, 1, 0 },
        new byte[] { 0, 0, 1, 0, 0, 0, 1, 1, 0 },
        new byte[] { 0, 0, 0, 0, 1, 0, 1, 1, 0 },
        new byte[] { 1, 1, 0, 0, 0, 0, 0, 0, 1 },
        new byte[] { 0, 1, 1, 0, 0, 0, 0, 0, 1 },
        new byte[] { 1, 1, 1, 0, 0, 0, 0, 0, 0 },
        new byte[] { 0, 1, 0, 0, 1, 0, 0, 0, 1 },
        new byte[] { 1, 1, 0, 0, 1, 0, 0, 0, 0 },
        new byte[] { 0, 1, 1, 0, 1, 0, 0, 0, 0 },
        new byte[] { 0, 1, 0, 0, 0, 0, 1, 0, 1 },
        new byte[] { 1, 1, 0, 0, 0, 0, 1, 0, 0 },
        new byte[] { 0, 1, 1, 0, 0, 0, 1, 0, 0 },
        new byte[] { 0, 1, 0, 1, 0, 1, 0, 0, 0 },
        new byte[] { 0, 1, 0, 1, 0, 0, 0, 1, 0 },
        new byte[] { 0, 1, 0, 0, 0, 1, 0, 1, 0 },
        new byte[] { 0, 0, 0, 1, 0, 1, 0, 1, 0 },
        new byte[] { 0, 1, 0, 0, 1, 0, 1, 0, 0 },
    };

    /// <summary>
    ///     Creates a new Barcode39.
    /// </summary>
    public Barcode39()
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
        startStopText = true;
        extended = false;
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
            var fCode = code;
            if (extended)
            {
                fCode = GetCode39Ex(code);
            }

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
                    fullCode += GetChecksum(fCode);
                }

                if (startStopText)
                {
                    fullCode = "*" + fullCode + "*";
                }

                fontX = font.GetWidthPoint(altText != null ? altText : fullCode, size);
            }

            var len = fCode.Length + 2;
            if (generateChecksum)
            {
                ++len;
            }

            var fullWidth = len * (6 * x + 3 * x * n) + (len - 1) * x;
            fullWidth = Math.Max(fullWidth, fontX);
            var fullHeight = barHeight + fontY;
            return new Rectangle(fullWidth, fullHeight);
        }
    }

    /// <summary>
    ///     Creates the bars.
    ///     stop characters
    /// </summary>
    /// <param name="text">the text to create the bars. This text does not include the start and</param>
    /// <returns>the bars</returns>
    public static byte[] GetBarsCode39(string text)
    {
        text = "*" + text + "*";
        var bars = new byte[text.Length * 10 - 1];
        for (var k = 0; k < text.Length; ++k)
        {
            var idx = Chars.IndexOf(text[k].ToString(), StringComparison.Ordinal);
            if (idx < 0)
            {
                throw new ArgumentException($"The character \'{text[k]}\' is illegal in code 39.");
            }

            Array.Copy(_bars[idx], 0, bars, k * 10, 9);
        }

        return bars;
    }

    /// <summary>
    ///     Converts the extended text into a normal, escaped text,
    ///     ready to generate bars.
    /// </summary>
    /// <param name="text">the extended text</param>
    /// <returns>the escaped text</returns>
    public static string GetCode39Ex(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var ret = "";
        for (var k = 0; k < text.Length; ++k)
        {
            var c = text[k];
            if (c > 127)
            {
                throw new ArgumentException($"The character \'{c}\' is illegal in code 39 extended.");
            }

            var c1 = EXTENDED[c * 2];
            var c2 = EXTENDED[c * 2 + 1];
            if (c1 != ' ')
            {
                ret += c1;
            }

            ret += c2;
        }

        return ret;
    }

    public override SKBitmap CreateDrawingImage(Color foreground, Color background)
    {
        var bCode = code;
        if (extended)
        {
            bCode = GetCode39Ex(code);
        }

        if (generateChecksum)
        {
            bCode += GetChecksum(bCode);
        }

        var len = bCode.Length + 2;
        var nn = (int)n;
        var fullWidth = len * (6 + 3 * nn) + (len - 1);
        var height = (int)barHeight;
        var bmp = new SKBitmap(fullWidth, height);
        var bars = GetBarsCode39(bCode);
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
        var bCode = code;
        if (extended)
        {
            bCode = GetCode39Ex(code);
        }

        if (font != null)
        {
            if (generateChecksum && checksumText)
            {
                fullCode += GetChecksum(bCode);
            }

            if (startStopText)
            {
                fullCode = "*" + fullCode + "*";
            }

            fontX = font.GetWidthPoint(fullCode = altText != null ? altText : fullCode, size);
        }

        if (generateChecksum)
        {
            bCode += GetChecksum(bCode);
        }

        var len = bCode.Length + 2;
        var fullWidth = len * (6 * x + 3 * x * n) + (len - 1) * x;
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

        var bars = GetBarsCode39(bCode);
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

    /// <summary>
    ///     Calculates the checksum.
    /// </summary>
    /// <param name="text">the text</param>
    /// <returns>the checksum</returns>
    internal static char GetChecksum(string text)
    {
        var chk = 0;
        for (var k = 0; k < text.Length; ++k)
        {
            var idx = Chars.IndexOf(text[k].ToString(), StringComparison.Ordinal);
            if (idx < 0)
            {
                throw new ArgumentException($"The character \'{text[k]}\' is illegal in code 39.");
            }

            chk += idx;
        }

        return Chars[chk % 43];
    }
}