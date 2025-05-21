namespace iTextSharp.text.pdf;

/// <summary>
///     Represents a pattern. Can be used in high-level constructs (Paragraph, Cell, etc.).
/// </summary>
public class PatternColor : ExtendedColor
{
    /// <summary>
    ///     The actual pattern.
    /// </summary>
    private readonly PdfPatternPainter _painter;

    /// <summary>
    ///     Creates a color representing a pattern.
    /// </summary>
    /// <param name="painter">the actual pattern</param>
    public PatternColor(PdfPatternPainter painter) : base(TYPE_PATTERN, red: .5f, green: .5f, blue: .5f)
        => _painter = painter;

    /// <summary>
    ///     Gets the pattern.
    /// </summary>
    /// <returns>the pattern</returns>
    public PdfPatternPainter Painter => _painter;

    public override bool Equals(object obj) => obj is PatternColor color && color.Painter.Equals(Painter);

    public override int GetHashCode() => _painter.GetHashCode();
}