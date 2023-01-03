namespace iTextSharp.text.pdf;

/// <summary>
///     @author  Paulo Soares (psoares@consiste.pt)
/// </summary>
public abstract class ExtendedColor : BaseColor
{
    internal const int TYPE_CMYK = 2;
    internal const int TYPE_GRAY = 1;
    internal const int TYPE_PATTERN = 4;
    internal const int TYPE_RGB = 0;
    internal const int TYPE_SEPARATION = 3;
    internal const int TYPE_SHADING = 5;

    protected int type;

    protected ExtendedColor(int type) : base(0, 0, 0) => this.type = type;

    protected ExtendedColor(int type, float red, float green, float blue) : base(Normalize(red), Normalize(green),
     Normalize(blue)) => this.type = type;

    public int Type => type;

    public static int GetType(object color)
    {
        if (color is ExtendedColor)
        {
            return ((ExtendedColor)color).Type;
        }

        return TYPE_RGB;
    }

    internal static float Normalize(float value)
    {
        if (value < 0)
        {
            return 0;
        }

        if (value > 1)
        {
            return 1;
        }

        return value;
    }
}