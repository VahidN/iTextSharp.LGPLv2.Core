namespace iTextSharp.text.pdf.codec.wmf;

public class MetaObject
{
    public const int META_BRUSH = 2;
    public const int META_FONT = 3;
    public const int META_NOT_SUPPORTED = 0;
    public const int META_PEN = 1;

    public MetaObject()
    {
    }

    public MetaObject(int type) => Type = type;

    public int Type { get; set; } = META_NOT_SUPPORTED;
}