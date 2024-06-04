namespace iTextSharp.text.pdf;

/// <summary>
///     @author  psoares
/// </summary>
public class SpotColor : ExtendedColor
{
    public SpotColor(PdfSpotColor spot, float tint) :
        base(TYPE_SEPARATION,
             ((spot?.AlternativeCs.R ?? throw new ArgumentNullException(nameof(spot))) / 255f - 1f) * tint + 1,
             (spot.AlternativeCs.G / 255f - 1f) * tint + 1,
             (spot.AlternativeCs.B / 255f - 1f) * tint + 1)
    {
        PdfSpotColor = spot;
        Tint = tint;
    }

    public SpotColor(PdfSpotColor spot) : this(spot, spot?.Tint ?? throw new ArgumentNullException(nameof(spot)))
    {
    }

    public PdfSpotColor PdfSpotColor { get; }

    public float Tint { get; }

    public override bool Equals(object obj) => this == obj;

    public override int GetHashCode() => PdfSpotColor.GetHashCode() ^ Tint.GetHashCode();
}