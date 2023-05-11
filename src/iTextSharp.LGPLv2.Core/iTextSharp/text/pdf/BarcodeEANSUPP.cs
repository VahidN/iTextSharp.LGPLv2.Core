using System.Drawing;
using SkiaSharp;

namespace iTextSharp.text.pdf;

/// <summary>
///     This class takes 2 barcodes, an EAN/UPC and a supplemental
///     and creates a single barcode with both combined in the
///     expected layout. The UPC/EAN should have a positive text
///     baseline and the supplemental a negative one (in the supplemental
///     the text is on the top of the barcode.
///     The default parameters are:
///     n = 8; // horizontal distance between the two barcodes
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class BarcodeEansupp : Barcode
{
    /// <summary>
    ///     The barcode with the EAN/UPC.
    /// </summary>
    protected Barcode Ean;

    /// <summary>
    ///     The barcode with the supplemental.
    /// </summary>
    protected Barcode Supp;

    /// <summary>
    ///     Creates new combined barcode.
    /// </summary>
    /// <param name="ean">the EAN/UPC barcode</param>
    /// <param name="supp">the supplemental barcode</param>
    public BarcodeEansupp(Barcode ean, Barcode supp)
    {
        n = 8; // horizontal distance between the two barcodes
        Ean = ean;
        Supp = supp;
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
            var rect = Ean.BarcodeSize;
            rect.Right = rect.Width + Supp.BarcodeSize.Width + n;
            return rect;
        }
    }

    public override SKBitmap CreateDrawingImage(Color foreground, Color background) =>
        throw new InvalidOperationException("The two barcodes must be composed externally.");

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

        if (Supp.Font != null)
        {
            Supp.BarHeight = Ean.BarHeight + Supp.Baseline - Supp.Font.GetFontDescriptor(BaseFont.CAPHEIGHT, Supp.Size);
        }
        else
        {
            Supp.BarHeight = Ean.BarHeight;
        }

        var eanR = Ean.BarcodeSize;
        cb.SaveState();
        Ean.PlaceBarcode(cb, barColor, textColor);
        cb.RestoreState();
        cb.SaveState();
        cb.ConcatCtm(1, 0, 0, 1, eanR.Width + n, eanR.Height - Ean.BarHeight);
        Supp.PlaceBarcode(cb, barColor, textColor);
        cb.RestoreState();
        return BarcodeSize;
    }
}