using System.Drawing;

namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the pattern.
/// </summary>
public sealed class PdfPatternPainter : PdfTemplate
{
    internal BaseColor defaultColor;
    internal bool Stencil;
    internal float Xstep, Ystep;

    /// <summary>
    ///     Creates a  PdfPattern .
    /// </summary>
    internal PdfPatternPainter(PdfWriter wr) : base(wr) => type = TYPE_PATTERN;

    /// <summary>
    ///     Creates new PdfPattern
    /// </summary>
    internal PdfPatternPainter(PdfWriter wr, BaseColor defaultColor) : this(wr)
    {
        Stencil = true;
        if (defaultColor == null)
        {
            this.defaultColor = new BaseColor(Color.Gray);
        }
        else
        {
            this.defaultColor = defaultColor;
        }
    }

    private PdfPatternPainter() => type = TYPE_PATTERN;

    public BaseColor DefaultColor => defaultColor;

    public override PdfContentByte Duplicate
    {
        get
        {
            var tpl = new PdfPatternPainter();
            tpl.Writer = Writer;
            tpl.Pdf = Pdf;
            tpl.ThisReference = ThisReference;
            tpl.pageResources = pageResources;
            tpl.BBox = new Rectangle(BBox);
            tpl.Xstep = Xstep;
            tpl.Ystep = Ystep;
            tpl.matrix = matrix;
            tpl.Stencil = Stencil;
            tpl.defaultColor = defaultColor;
            return tpl;
        }
    }

    public float XStep
    {
        get => Xstep;

        set => Xstep = value;
    }

    public float YStep
    {
        get => Ystep;

        set => Ystep = value;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#addImage(com.lowagie.text.Image, float, float, float, float, float, float)
    /// </summary>
    public override void AddImage(Image image, float a, float b, float c, float d, float e, float f)
    {
        if (Stencil && !image.IsMask())
        {
            CheckNoColor();
        }

        base.AddImage(image, a, b, c, d, e, f);
    }

    public bool IsStencil() => Stencil;

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#resetCMYKColorFill()
    /// </summary>
    public override void ResetCmykColorFill()
    {
        CheckNoColor();
        base.ResetCmykColorFill();
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#resetCMYKColorStroke()
    /// </summary>
    public override void ResetCmykColorStroke()
    {
        CheckNoColor();
        base.ResetCmykColorStroke();
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#resetGrayFill()
    /// </summary>
    public override void ResetGrayFill()
    {
        CheckNoColor();
        base.ResetGrayFill();
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#resetGrayStroke()
    /// </summary>
    public override void ResetGrayStroke()
    {
        CheckNoColor();
        base.ResetGrayStroke();
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#resetRGBColorFill()
    /// </summary>
    public override void ResetRgbColorFill()
    {
        CheckNoColor();
        base.ResetRgbColorFill();
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#resetRGBColorStroke()
    /// </summary>
    public override void ResetRgbColorStroke()
    {
        CheckNoColor();
        base.ResetRgbColorStroke();
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setCMYKColorFill(int, int, int, int)
    /// </summary>
    public override void SetCmykColorFill(int cyan, int magenta, int yellow, int black)
    {
        CheckNoColor();
        base.SetCmykColorFill(cyan, magenta, yellow, black);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setCMYKColorFillF(float, float, float, float)
    /// </summary>
    public override void SetCmykColorFillF(float cyan, float magenta, float yellow, float black)
    {
        CheckNoColor();
        base.SetCmykColorFillF(cyan, magenta, yellow, black);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setCMYKColorStroke(int, int, int, int)
    /// </summary>
    public override void SetCmykColorStroke(int cyan, int magenta, int yellow, int black)
    {
        CheckNoColor();
        base.SetCmykColorStroke(cyan, magenta, yellow, black);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setCMYKColorStrokeF(float, float, float, float)
    /// </summary>
    public override void SetCmykColorStrokeF(float cyan, float magenta, float yellow, float black)
    {
        CheckNoColor();
        base.SetCmykColorStrokeF(cyan, magenta, yellow, black);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setColorFill(java.awt.Color)
    /// </summary>
    public override void SetColorFill(BaseColor value)
    {
        CheckNoColor();
        base.SetColorFill(value);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setColorFill(com.lowagie.text.pdf.PdfSpotColor, float)
    /// </summary>
    public override void SetColorFill(PdfSpotColor sp, float tint)
    {
        CheckNoColor();
        base.SetColorFill(sp, tint);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setColorStroke(java.awt.Color)
    /// </summary>
    public override void SetColorStroke(BaseColor value)
    {
        CheckNoColor();
        base.SetColorStroke(value);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setColorStroke(com.lowagie.text.pdf.PdfSpotColor, float)
    /// </summary>
    public override void SetColorStroke(PdfSpotColor sp, float tint)
    {
        CheckNoColor();
        base.SetColorStroke(sp, tint);
    }

    /// <summary>
    ///     Gets a duplicate of this  PdfPatternPainter . All
    ///     the members are copied by reference but the buffer stays different.
    /// </summary>
    /// <returns>a copy of this  PdfPatternPainter </returns>
    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setGrayFill(float)
    /// </summary>
    public override void SetGrayFill(float value)
    {
        CheckNoColor();
        base.SetGrayFill(value);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setGrayStroke(float)
    /// </summary>
    public override void SetGrayStroke(float value)
    {
        CheckNoColor();
        base.SetGrayStroke(value);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setPatternFill(com.lowagie.text.pdf.PdfPatternPainter)
    /// </summary>
    public override void SetPatternFill(PdfPatternPainter p)
    {
        CheckNoColor();
        base.SetPatternFill(p);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setPatternFill(com.lowagie.text.pdf.PdfPatternPainter, java.awt.Color,
    ///     float)
    /// </summary>
    public override void SetPatternFill(PdfPatternPainter p, BaseColor color, float tint)
    {
        CheckNoColor();
        base.SetPatternFill(p, color, tint);
    }

    public void SetPatternMatrix(float a, float b, float c, float d, float e, float f)
    {
        SetMatrix(a, b, c, d, e, f);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setPatternStroke(com.lowagie.text.pdf.PdfPatternPainter, java.awt.Color,
    ///     float)
    /// </summary>
    public override void SetPatternStroke(PdfPatternPainter p, BaseColor color, float tint)
    {
        CheckNoColor();
        base.SetPatternStroke(p, color, tint);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setPatternStroke(com.lowagie.text.pdf.PdfPatternPainter)
    /// </summary>
    public override void SetPatternStroke(PdfPatternPainter p)
    {
        CheckNoColor();
        base.SetPatternStroke(p);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setRGBColorFill(int, int, int)
    /// </summary>
    public override void SetRgbColorFill(int red, int green, int blue)
    {
        CheckNoColor();
        base.SetRgbColorFill(red, green, blue);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setRGBColorFillF(float, float, float)
    /// </summary>
    public override void SetRgbColorFillF(float red, float green, float blue)
    {
        CheckNoColor();
        base.SetRgbColorFillF(red, green, blue);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setRGBColorStroke(int, int, int)
    /// </summary>
    public override void SetRgbColorStroke(int red, int green, int blue)
    {
        CheckNoColor();
        base.SetRgbColorStroke(red, green, blue);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfContentByte#setRGBColorStrokeF(float, float, float)
    /// </summary>
    public override void SetRgbColorStrokeF(float red, float green, float blue)
    {
        CheckNoColor();
        base.SetRgbColorStrokeF(red, green, blue);
    }

    internal void CheckNoColor()
    {
        if (Stencil)
        {
            throw new ArgumentException("Colors are not allowed in uncolored tile patterns.");
        }
    }

    /// <summary>
    ///     Gets the stream representing this pattern
    /// </summary>
    /// <returns>the stream representing this pattern</returns>
    internal PdfPattern GetPattern() => new(this);

    /// <summary>
    ///     Gets the stream representing this pattern
    ///     @since   2.1.3
    /// </summary>
    /// <param name="compressionLevel">the compression level of the stream</param>
    /// <returns>the stream representing this pattern</returns>
    internal PdfPattern GetPattern(int compressionLevel) => new(this, compressionLevel);
}