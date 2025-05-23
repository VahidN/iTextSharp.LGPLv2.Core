namespace iTextSharp.text.pdf;

/// <summary>
///     Implements a shading pattern as a  Color .
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class ShadingColor : ExtendedColor
{
    public ShadingColor(PdfShadingPattern shadingPattern) : base(TYPE_SHADING, red: .5f, green: .5f, blue: .5f)
        => PdfShadingPattern = shadingPattern;

    public PdfShadingPattern PdfShadingPattern { get; }

    public override bool Equals(object obj)
        => obj is ShadingColor color && color.PdfShadingPattern.Equals(PdfShadingPattern);

    public override int GetHashCode() => PdfShadingPattern.GetHashCode();
}