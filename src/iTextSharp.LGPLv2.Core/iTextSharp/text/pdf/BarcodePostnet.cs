using System.Drawing;
using iTextSharp.LGPLv2.Core.System.Drawing;
using SkiaSharp;

namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the Postnet and Planet barcodes. The default parameters are:
///     n = 72f / 22f; // distance between bars
///     x = 0.02f * 72f; // bar width
///     barHeight = 0.125f * 72f; // height of the tall bars
///     size = 0.05f * 72f; // height of the short bars
///     codeType = POSTNET; // type of code
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class BarcodePostnet : Barcode
{
    /// <summary>
    ///     The bars for each character.
    /// </summary>
    private static readonly byte[][] _bars =
    {
        new byte[] { 1, 1, 0, 0, 0 },
        new byte[] { 0, 0, 0, 1, 1 },
        new byte[] { 0, 0, 1, 0, 1 },
        new byte[] { 0, 0, 1, 1, 0 },
        new byte[] { 0, 1, 0, 0, 1 },
        new byte[] { 0, 1, 0, 1, 0 },
        new byte[] { 0, 1, 1, 0, 0 },
        new byte[] { 1, 0, 0, 0, 1 },
        new byte[] { 1, 0, 0, 1, 0 },
        new byte[] { 1, 0, 1, 0, 0 },
    };

    /// <summary>
    ///     Creates new BarcodePostnet
    /// </summary>
    public BarcodePostnet()
    {
        n = 72f / 22f; // distance between bars
        x = 0.02f * 72f; // bar width
        barHeight = 0.125f * 72f; // height of the tall bars
        size = 0.05f * 72f; // height of the short bars
        codeType = POSTNET; // type of code
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
            var width = ((float)(code.Length + 1) * 5 + 1) * n + x;
            return new Rectangle(width, barHeight);
        }
    }

    /// <summary>
    ///     Creates the bars for Postnet.
    /// </summary>
    /// <param name="text">the code to be created without checksum</param>
    /// <returns>the bars</returns>
    public static byte[] GetBarsPostnet(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var total = 0;
        for (var k = text.Length - 1; k >= 0; --k)
        {
            var n = text[k] - '0';
            total += n;
        }

        text += (char)((10 - total % 10) % 10 + '0');
        var bars = new byte[text.Length * 5 + 2];
        bars[0] = 1;
        bars[bars.Length - 1] = 1;
        for (var k = 0; k < text.Length; ++k)
        {
            var c = text[k] - '0';
            Array.Copy(_bars[c], 0, bars, k * 5 + 1, 5);
        }

        return bars;
    }

    public override SKBitmap CreateDrawingImage(Color foreground, Color background)
    {
        var barWidth = (int)x;
        if (barWidth <= 0)
        {
            barWidth = 1;
        }

        var barDistance = (int)n;
        if (barDistance <= barWidth)
        {
            barDistance = barWidth + 1;
        }

        var barShort = (int)size;
        if (barShort <= 0)
        {
            barShort = 1;
        }

        var barTall = (int)barHeight;
        if (barTall <= barShort)
        {
            barTall = barShort + 1;
        }

        var bars = GetBarsPostnet(code);
        var width = bars.Length * barDistance;
        byte flip = 1;
        if (codeType == PLANET)
        {
            flip = 0;
            bars[0] = 0;
            bars[bars.Length - 1] = 0;
        }

        var bmp = new SKBitmap(width, barTall);
        var seg1 = barTall - barShort;
        for (var i = 0; i < seg1; ++i)
        {
            var idx = 0;
            for (var k = 0; k < bars.Length; ++k)
            {
                var dot = bars[k] == flip;
                for (var j = 0; j < barDistance; ++j)
                {
                    bmp.SetPixel(idx++, i, dot && j < barWidth ? foreground.ToSKColor() : background.ToSKColor());
                }
            }
        }

        for (var i = seg1; i < barTall; ++i)
        {
            var idx = 0;
            for (var k = 0; k < bars.Length; ++k)
            {
                for (var j = 0; j < barDistance; ++j)
                {
                    bmp.SetPixel(idx++, i, j < barWidth ? foreground.ToSKColor() : background.ToSKColor());
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

        if (barColor != null)
        {
            cb.SetColorFill(barColor);
        }

        var bars = GetBarsPostnet(code);
        byte flip = 1;
        if (codeType == PLANET)
        {
            flip = 0;
            bars[0] = 0;
            bars[bars.Length - 1] = 0;
        }

        float startX = 0;
        for (var k = 0; k < bars.Length; ++k)
        {
            cb.Rectangle(startX, 0, x - inkSpreading, bars[k] == flip ? barHeight : size);
            startX += n;
        }

        cb.Fill();
        return BarcodeSize;
    }
}