namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the shading pattern dictionary.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfShadingPattern : PdfDictionary
{
    protected float[] matrix = { 1, 0, 0, 1, 0, 0 };
    protected PdfName patternName;
    protected PdfIndirectReference patternReference;
    protected PdfShading shading;

    protected PdfWriter Writer;

    /// <summary>
    ///     Creates new PdfShadingPattern
    /// </summary>
    public PdfShadingPattern(PdfShading shading)
    {
        if (shading == null)
        {
            throw new ArgumentNullException(nameof(shading));
        }

        Writer = shading.Writer;
        Put(PdfName.Patterntype, new PdfNumber(2));
        this.shading = shading;
    }

    public float[] Matrix
    {
        get => matrix;

        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length != 6)
            {
                throw new ArgumentException("The matrix size must be 6.");
            }

            matrix = value;
        }
    }

    public PdfShading Shading => shading;

    internal ColorDetails ColorDetails => shading.ColorDetails;

    internal int Name
    {
        set => patternName = new PdfName("P" + value);
    }

    internal PdfName PatternName => patternName;

    internal PdfIndirectReference PatternReference
    {
        get
        {
            if (patternReference == null)
            {
                patternReference = Writer.PdfIndirectReference;
            }

            return patternReference;
        }
    }

    internal PdfName ShadingName => shading.ShadingName;

    internal PdfIndirectReference ShadingReference => shading.ShadingReference;

    internal void AddToBody()
    {
        Put(PdfName.Shading, ShadingReference);
        Put(PdfName.Matrix, new PdfArray(matrix));
        Writer.AddToBody(this, PatternReference);
    }
}