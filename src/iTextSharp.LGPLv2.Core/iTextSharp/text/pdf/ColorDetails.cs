namespace iTextSharp.text.pdf;

/// <summary>
///     Each spotcolor in the document will have an instance of this class
///     @author Phillip Pan (phillip@formstar.com)
/// </summary>
public class ColorDetails
{
    /// <summary>
    ///     The color name that appears in the document body stream
    /// </summary>
    private readonly PdfName _colorName;

    /// <summary>
    ///     The indirect reference to this color
    /// </summary>
    private readonly PdfIndirectReference _indirectReference;

    /// <summary>
    ///     The color
    /// </summary>
    private readonly PdfSpotColor _spotcolor;

    /// <summary>
    ///     Each spot color used in a document has an instance of this class.
    /// </summary>
    /// <param name="colorName">the color name</param>
    /// <param name="indirectReference">the indirect reference to the font</param>
    /// <param name="scolor">the  PDfSpotColor </param>
    internal ColorDetails(PdfName colorName, PdfIndirectReference indirectReference, PdfSpotColor scolor)
    {
        _colorName = colorName;
        _indirectReference = indirectReference;
        _spotcolor = scolor;
    }

    /// <summary>
    ///     Gets the color name as it appears in the document body.
    /// </summary>
    /// <returns>the color name</returns>
    internal PdfName ColorName => _colorName;

    /// <summary>
    ///     Gets the indirect reference to this color.
    /// </summary>
    /// <returns>the indirect reference to this color</returns>
    internal PdfIndirectReference IndirectReference => _indirectReference;

    /// <summary>
    ///     Gets the  SpotColor  object.
    /// </summary>
    /// <returns>the  PdfSpotColor </returns>
    internal PdfObject GetSpotColor(PdfWriter writer) => _spotcolor.GetSpotObject(writer);
}