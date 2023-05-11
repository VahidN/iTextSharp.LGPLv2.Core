using System.Drawing;
using SkiaSharp;

namespace iTextSharp.text.pdf;

/// <summary>
///     Base class containing properties and methods commom to all
///     barcode types.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public abstract class Barcode
{
    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int CODABAR = 12;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int CODE128 = 9;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int CODE128_RAW = 11;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int CODE128_UCC = 10;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int EAN13 = 1;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int EAN8 = 2;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int PLANET = 8;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int POSTNET = 7;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int SUPP2 = 5;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int SUPP5 = 6;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int UPCA = 3;

    /// <summary>
    ///     A type of barcode
    /// </summary>
    public const int UPCE = 4;

    /// <summary>
    ///     The alternate text to be used, if present.
    /// </summary>
    protected string altText;

    /// <summary>
    ///     The height of the bars.
    /// </summary>
    protected float barHeight;

    /// <summary>
    ///     If positive, the text distance under the bars. If zero or negative,
    ///     the text distance above the bars.
    /// </summary>
    protected float baseline;

    /// <summary>
    ///     Shows the generated checksum in the the text.
    /// </summary>
    protected bool checksumText;

    /// <summary>
    ///     The code to generate.
    /// </summary>
    protected string code = "";

    /// <summary>
    ///     The code type.
    /// </summary>
    protected int codeType;

    /// <summary>
    ///     Generates extended barcode 39.
    /// </summary>
    protected bool extended;

    /// <summary>
    ///     The text font.  null  if no text.
    /// </summary>
    protected BaseFont font;

    /// <summary>
    ///     The optional checksum generation.
    /// </summary>
    protected bool generateChecksum;

    /// <summary>
    ///     Show the guard bars for barcode EAN.
    /// </summary>
    protected bool guardBars;

    /// <summary>
    ///     The ink spreading.
    /// </summary>
    protected float inkSpreading;

    /// <summary>
    ///     The bar multiplier for wide bars or the distance between
    ///     bars for Postnet and Planet.
    /// </summary>
    protected float n;

    /// <summary>
    ///     The size of the text or the height of the shorter bar
    ///     in Postnet.
    /// </summary>
    protected float size;

    /// <summary>
    ///     Show the start and stop character '*' in the text for
    ///     the barcode 39 or 'ABCD' for codabar.
    /// </summary>
    protected bool startStopText;

    /// <summary>
    ///     The text Element. Can be  Element.ALIGN_LEFT ,
    ///     Element.ALIGN_CENTER  or  Element.ALIGN_RIGHT .
    /// </summary>
    protected int textAlignment;

    /// <summary>
    ///     The minimum bar width.
    /// </summary>
    protected float x;

    /// <summary>
    ///     Sets the alternate text. If present, this text will be used instead of the
    ///     text derived from the supplied code.
    /// </summary>
    public string AltText
    {
        set => altText = value;
        get => altText;
    }

    /// <summary>
    ///     Gets the maximum area that the barcode and the text, if
    ///     any, will occupy. The lower left corner is always (0, 0).
    /// </summary>
    /// <returns>the size the barcode occupies.</returns>
    public abstract Rectangle BarcodeSize { get; }

    /// <summary>
    ///     Gets the height of the bars.
    /// </summary>
    /// <returns>the height of the bars</returns>
    public float BarHeight
    {
        get => barHeight;

        set => barHeight = value;
    }

    /// <summary>
    ///     Gets the text baseline.
    ///     If positive, the text distance under the bars. If zero or negative,
    ///     the text distance above the bars.
    /// </summary>
    /// <returns>the baseline.</returns>
    public float Baseline
    {
        get => baseline;

        set => baseline = value;
    }

    /// <summary>
    ///     Sets the property to show the generated checksum in the the text.
    /// </summary>
    public bool ChecksumText
    {
        set => checksumText = value;
        get => checksumText;
    }

    /// <summary>
    ///     Gets the code to generate.
    /// </summary>
    /// <returns>the code to generate</returns>
    public virtual string Code
    {
        get => code;

        set => code = value;
    }

    /// <summary>
    ///     Gets the code type.
    /// </summary>
    /// <returns>the code type</returns>
    public int CodeType
    {
        get => codeType;

        set => codeType = value;
    }

    /// <summary>
    ///     Sets the property to generate extended barcode 39.
    /// </summary>
    public bool Extended
    {
        set => extended = value;
        get => extended;
    }

    /// <summary>
    ///     Gets the text font.  null  if no text.
    /// </summary>
    /// <returns>the text font.  null  if no text</returns>
    public BaseFont Font
    {
        get => font;

        set => font = value;
    }

    /// <summary>
    ///     The property for the optional checksum generation.
    /// </summary>
    public bool GenerateChecksum
    {
        set => generateChecksum = value;
        get => generateChecksum;
    }

    /// <summary>
    ///     Sets the property to show the guard bars for barcode EAN.
    /// </summary>
    public bool GuardBars
    {
        set => guardBars = value;
        get => guardBars;
    }

    public float InkSpreading
    {
        set => inkSpreading = value;
        get => inkSpreading;
    }

    /// <summary>
    ///     Gets the bar multiplier for wide bars.
    /// </summary>
    /// <returns>the bar multiplier for wide bars</returns>
    public float N
    {
        get => n;

        set => n = value;
    }

    /// <summary>
    ///     Gets the size of the text.
    /// </summary>
    /// <returns>the size of the text</returns>
    public float Size
    {
        get => size;

        set => size = value;
    }

    /// <summary>
    ///     Gets the property to show the start and stop character '*' in the text for
    ///     the barcode 39.
    /// </summary>
    public bool StartStopText
    {
        set => startStopText = value;
        get => startStopText;
    }

    /// <summary>
    ///     Gets the text Element. Can be  Element.ALIGN_LEFT ,
    ///     Element.ALIGN_CENTER  or  Element.ALIGN_RIGHT .
    /// </summary>
    /// <returns>the text alignment</returns>
    public int TextAlignment
    {
        get => textAlignment;

        set => textAlignment = value;
    }

    /// <summary>
    ///     Gets the minimum bar width.
    /// </summary>
    /// <returns>the minimum bar width</returns>
    public float X
    {
        get => x;

        set => x = value;
    }

    public abstract SKBitmap CreateDrawingImage(Color foreground, Color background);

    /// <summary>
    ///     Creates an  Image  with the barcode.
    ///     serves no other use
    ///     @see #placeBarcode(PdfContentByte cb, Color barColor, Color textColor)
    /// </summary>
    /// <param name="cb">the  PdfContentByte  to create the  Image . It</param>
    /// <param name="barColor">the color of the bars. It can be  null </param>
    /// <param name="textColor">the color of the text. It can be  null </param>
    /// <returns>the  Image </returns>
    public Image CreateImageWithBarcode(PdfContentByte cb, BaseColor barColor, BaseColor textColor) =>
        Image.GetInstance(CreateTemplateWithBarcode(cb, barColor, textColor));

    /// <summary>
    ///     Creates a template with the barcode.
    ///     serves no other use
    ///     @see #placeBarcode(PdfContentByte cb, Color barColor, Color textColor)
    /// </summary>
    /// <param name="cb">the  PdfContentByte  to create the template. It</param>
    /// <param name="barColor">the color of the bars. It can be  null </param>
    /// <param name="textColor">the color of the text. It can be  null </param>
    /// <returns>the template</returns>
    public PdfTemplate CreateTemplateWithBarcode(PdfContentByte cb, BaseColor barColor, BaseColor textColor)
    {
        if (cb == null)
        {
            throw new ArgumentNullException(nameof(cb));
        }

        var tp = cb.CreateTemplate(0, 0);
        var rect = PlaceBarcode(tp, barColor, textColor);
        tp.BoundingBox = rect;
        return tp;
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
    public abstract Rectangle PlaceBarcode(PdfContentByte cb, BaseColor barColor, BaseColor textColor);
}