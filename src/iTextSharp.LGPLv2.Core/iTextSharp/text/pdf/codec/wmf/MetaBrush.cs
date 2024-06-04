namespace iTextSharp.text.pdf.codec.wmf;

public class MetaBrush : MetaObject
{
    public const int BS_DIBPATTERN = 5;
    public const int BS_HATCHED = 2;
    public const int BS_NULL = 1;
    public const int BS_PATTERN = 3;
    public const int BS_SOLID = 0;
    public const int HS_BDIAGONAL = 3;
    public const int HS_CROSS = 4;
    public const int HS_DIAGCROSS = 5;
    public const int HS_FDIAGONAL = 2;
    public const int HS_HORIZONTAL = 0;
    public const int HS_VERTICAL = 1;

    public MetaBrush() => Type = META_BRUSH;

    public BaseColor Color { get; private set; } = BaseColor.White;

    public int Hatch { get; private set; }

    public int Style { get; private set; } = BS_SOLID;

    public void Init(InputMeta meta)
    {
        if (meta == null)
        {
            throw new ArgumentNullException(nameof(meta));
        }

        Style = meta.ReadWord();
        Color = meta.ReadColor();
        Hatch = meta.ReadWord();
    }
}