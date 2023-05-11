namespace iTextSharp.text.pdf.codec.wmf;

public class MetaPen : MetaObject
{
    public const int PS_DASH = 1;
    public const int PS_DASHDOT = 3;
    public const int PS_DASHDOTDOT = 4;
    public const int PS_DOT = 2;
    public const int PS_INSIDEFRAME = 6;
    public const int PS_NULL = 5;
    public const int PS_SOLID = 0;

    public MetaPen() => Type = META_PEN;

    public BaseColor Color { get; private set; } = BaseColor.Black;

    public int PenWidth { get; private set; } = 1;

    public int Style { get; private set; } = PS_SOLID;

    public void Init(InputMeta meta)
    {
        if (meta == null)
        {
            throw new ArgumentNullException(nameof(meta));
        }

        Style = meta.ReadWord();
        PenWidth = meta.ReadShort();
        meta.ReadWord();
        Color = meta.ReadColor();
    }
}