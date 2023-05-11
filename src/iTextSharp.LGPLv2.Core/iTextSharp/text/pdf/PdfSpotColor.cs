namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfSpotColor  defines a ColorSpace
///     @see     PdfDictionary
/// </summary>
public class PdfSpotColor
{
    /// <summary>
    ///     The alternative color space
    /// </summary>
    public BaseColor Altcs;

    /// <summary>
    ///     The color name
    /// </summary>
    public PdfName Name;

    /// <summary>
    ///     The tint value
    /// </summary>
    protected float tint;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a new  PdfSpotColor .
    /// </summary>
    /// <param name="name">a string value</param>
    /// <param name="tint">a tint value between 0 and 1</param>
    /// <param name="altcs">a altnative colorspace value</param>
    public PdfSpotColor(string name, float tint, BaseColor altcs)
    {
        Name = new PdfName(name);
        this.tint = tint;
        Altcs = altcs;
    }

    public BaseColor AlternativeCs => Altcs;

    public float Tint => tint;

    protected internal PdfObject GetSpotObject(PdfWriter writer)
    {
        var array = new PdfArray(PdfName.Separation);
        array.Add(Name);
        PdfFunction func = null;
        if (Altcs is ExtendedColor)
        {
            var type = ((ExtendedColor)Altcs).Type;
            switch (type)
            {
                case ExtendedColor.TYPE_GRAY:
                    array.Add(PdfName.Devicegray);
                    func = PdfFunction.Type2(writer, new float[] { 0, 1 }, null, new float[] { 0 },
                                             new[] { ((GrayColor)Altcs).Gray }, 1);
                    break;
                case ExtendedColor.TYPE_CMYK:
                    array.Add(PdfName.Devicecmyk);
                    var cmyk = (CmykColor)Altcs;
                    func = PdfFunction.Type2(writer, new float[] { 0, 1 }, null, new float[] { 0, 0, 0, 0 },
                                             new[] { cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black }, 1);
                    break;
                default:
                    throw new
                        NotSupportedException("Only RGB, Gray and CMYK are supported as alternative color spaces.");
            }
        }
        else
        {
            array.Add(PdfName.Devicergb);
            func = PdfFunction.Type2(writer, new float[] { 0, 1 }, null, new float[] { 1, 1, 1 },
                                     new[] { (float)Altcs.R / 255, (float)Altcs.G / 255, (float)Altcs.B / 255 }, 1);
        }

        array.Add(func.Reference);
        return array;
    }
}