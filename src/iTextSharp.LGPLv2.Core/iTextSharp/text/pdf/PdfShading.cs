namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the shading dictionary (or stream).
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfShading
{
    /// <summary>
    ///     Holds value of property antiAlias.
    /// </summary>
    protected bool antiAlias;

    /// <summary>
    ///     Holds value of property bBox.
    /// </summary>
    protected float[] bBox;

    protected ColorDetails colorDetails;
    protected PdfDictionary Shading;

    protected PdfName shadingName;
    protected PdfIndirectReference shadingReference;
    protected int ShadingType;
    protected PdfWriter writer;

    /// <summary>
    ///     Creates new PdfShading
    /// </summary>
    protected PdfShading(PdfWriter writer) => this.writer = writer;

    public bool AntiAlias
    {
        set => antiAlias = value;
        get => antiAlias;
    }

    public float[] BBox
    {
        get => bBox;
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length != 4)
            {
                throw new ArgumentException("BBox must be a 4 element array.");
            }

            bBox = value;
        }
    }

    public BaseColor ColorSpace { get; private set; }

    internal ColorDetails ColorDetails => colorDetails;

    internal int Name
    {
        set => shadingName = new PdfName("Sh" + value);
    }

    internal PdfName ShadingName => shadingName;

    internal PdfIndirectReference ShadingReference
    {
        get
        {
            if (shadingReference == null)
            {
                shadingReference = writer.PdfIndirectReference;
            }

            return shadingReference;
        }
    }

    internal PdfWriter Writer => writer;

    public static void CheckCompatibleColors(BaseColor c1, BaseColor c2)
    {
        if (c1 == null)
        {
            throw new ArgumentNullException(nameof(c1));
        }

        if (c2 == null)
        {
            throw new ArgumentNullException(nameof(c2));
        }

        var type1 = ExtendedColor.GetType(c1);
        var type2 = ExtendedColor.GetType(c2);
        if (type1 != type2)
        {
            throw new ArgumentException("Both colors must be of the same type.");
        }

        if (type1 == ExtendedColor.TYPE_SEPARATION && ((SpotColor)c1).PdfSpotColor != ((SpotColor)c2).PdfSpotColor)
        {
            throw new ArgumentException("The spot color must be the same, only the tint can vary.");
        }

        if (type1 == ExtendedColor.TYPE_PATTERN || type1 == ExtendedColor.TYPE_SHADING)
        {
            ThrowColorSpaceError();
        }
    }

    public static float[] GetColorArray(BaseColor color)
    {
        if (color == null)
        {
            throw new ArgumentNullException(nameof(color));
        }

        var type = ExtendedColor.GetType(color);
        switch (type)
        {
            case ExtendedColor.TYPE_GRAY:
            {
                return new[] { ((GrayColor)color).Gray };
            }
            case ExtendedColor.TYPE_CMYK:
            {
                var cmyk = (CmykColor)color;
                return new[] { cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black };
            }
            case ExtendedColor.TYPE_SEPARATION:
            {
                return new[] { ((SpotColor)color).Tint };
            }
            case ExtendedColor.TYPE_RGB:
            {
                return new[] { color.R / 255f, color.G / 255f, color.B / 255f };
            }
        }

        ThrowColorSpaceError();
        return null;
    }

    public static PdfShading SimpleAxial(PdfWriter writer, float x0, float y0, float x1, float y1, BaseColor startColor,
                                         BaseColor endColor, bool extendStart, bool extendEnd)
    {
        CheckCompatibleColors(startColor, endColor);
        var function = PdfFunction.Type2(writer, new float[] { 0, 1 }, null, GetColorArray(startColor),
                                         GetColorArray(endColor), 1);
        return Type2(writer, startColor, new[] { x0, y0, x1, y1 }, null, function, new[] { extendStart, extendEnd });
    }

    public static PdfShading SimpleAxial(PdfWriter writer, float x0, float y0, float x1, float y1, BaseColor startColor,
                                         BaseColor endColor) =>
        SimpleAxial(writer, x0, y0, x1, y1, startColor, endColor, true, true);

    public static PdfShading SimpleRadial(PdfWriter writer, float x0, float y0, float r0, float x1, float y1, float r1,
                                          BaseColor startColor, BaseColor endColor, bool extendStart, bool extendEnd)
    {
        CheckCompatibleColors(startColor, endColor);
        var function = PdfFunction.Type2(writer, new float[] { 0, 1 }, null, GetColorArray(startColor),
                                         GetColorArray(endColor), 1);
        return Type3(writer, startColor, new[] { x0, y0, r0, x1, y1, r1 }, null, function,
                     new[] { extendStart, extendEnd });
    }

    public static PdfShading SimpleRadial(PdfWriter writer, float x0, float y0, float r0, float x1, float y1, float r1,
                                          BaseColor startColor, BaseColor endColor) =>
        SimpleRadial(writer, x0, y0, r0, x1, y1, r1, startColor, endColor, true, true);

    public static void ThrowColorSpaceError()
    {
        throw new ArgumentException("A tiling or shading pattern cannot be used as a color space in a shading pattern");
    }

    public static PdfShading Type1(PdfWriter writer, BaseColor colorSpace, float[] domain, float[] tMatrix,
                                   PdfFunction function)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        var sp = new PdfShading(writer);
        sp.Shading = new PdfDictionary();
        sp.ShadingType = 1;
        sp.Shading.Put(PdfName.Shadingtype, new PdfNumber(sp.ShadingType));
        sp.SetColorSpace(colorSpace);
        if (domain != null)
        {
            sp.Shading.Put(PdfName.Domain, new PdfArray(domain));
        }

        if (tMatrix != null)
        {
            sp.Shading.Put(PdfName.Matrix, new PdfArray(tMatrix));
        }

        sp.Shading.Put(PdfName.Function, function.Reference);
        return sp;
    }

    public static PdfShading Type2(PdfWriter writer, BaseColor colorSpace, float[] coords, float[] domain,
                                   PdfFunction function, bool[] extend)
    {
        if (function == null)
        {
            throw new ArgumentNullException(nameof(function));
        }

        var sp = new PdfShading(writer);
        sp.Shading = new PdfDictionary();
        sp.ShadingType = 2;
        sp.Shading.Put(PdfName.Shadingtype, new PdfNumber(sp.ShadingType));
        sp.SetColorSpace(colorSpace);
        sp.Shading.Put(PdfName.Coords, new PdfArray(coords));
        if (domain != null)
        {
            sp.Shading.Put(PdfName.Domain, new PdfArray(domain));
        }

        sp.Shading.Put(PdfName.Function, function.Reference);
        if (extend != null && (extend[0] || extend[1]))
        {
            var array = new PdfArray(extend[0] ? PdfBoolean.Pdftrue : PdfBoolean.Pdffalse);
            array.Add(extend[1] ? PdfBoolean.Pdftrue : PdfBoolean.Pdffalse);
            sp.Shading.Put(PdfName.Extend, array);
        }

        return sp;
    }

    public static PdfShading Type3(PdfWriter writer, BaseColor colorSpace, float[] coords, float[] domain,
                                   PdfFunction function, bool[] extend)
    {
        var sp = Type2(writer, colorSpace, coords, domain, function, extend);
        sp.ShadingType = 3;
        sp.Shading.Put(PdfName.Shadingtype, new PdfNumber(sp.ShadingType));
        return sp;
    }

    internal void AddToBody()
    {
        if (bBox != null)
        {
            Shading.Put(PdfName.Bbox, new PdfArray(bBox));
        }

        if (antiAlias)
        {
            Shading.Put(PdfName.Antialias, PdfBoolean.Pdftrue);
        }

        writer.AddToBody(Shading, ShadingReference);
    }

    protected void SetColorSpace(BaseColor color)
    {
        ColorSpace = color ?? throw new ArgumentNullException(nameof(color));
        var type = ExtendedColor.GetType(color);
        PdfObject colorSpace = null;
        switch (type)
        {
            case ExtendedColor.TYPE_GRAY:
            {
                colorSpace = PdfName.Devicegray;
                break;
            }
            case ExtendedColor.TYPE_CMYK:
            {
                colorSpace = PdfName.Devicecmyk;
                break;
            }
            case ExtendedColor.TYPE_SEPARATION:
            {
                var spot = (SpotColor)color;
                colorDetails = writer.AddSimple(spot.PdfSpotColor);
                colorSpace = colorDetails.IndirectReference;
                break;
            }
            case ExtendedColor.TYPE_PATTERN:
            case ExtendedColor.TYPE_SHADING:
            {
                ThrowColorSpaceError();
                break;
            }
            default:
                colorSpace = PdfName.Devicergb;
                break;
        }

        Shading.Put(PdfName.Colorspace, colorSpace);
    }
}