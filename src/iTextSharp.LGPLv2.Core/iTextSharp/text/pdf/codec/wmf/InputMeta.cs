namespace iTextSharp.text.pdf.codec.wmf;

/// <summary>
///     Summary description for InputMeta.
/// </summary>
public class InputMeta
{
    private readonly Stream _sr;

    public InputMeta(Stream istr) => _sr = istr;

    public int Length { get; private set; }

    public int ReadByte()
    {
        ++Length;
        return _sr.ReadByte() & 0xff;
    }

    public BaseColor ReadColor()
    {
        var red = ReadByte();
        var green = ReadByte();
        var blue = ReadByte();
        ReadByte();
        return new BaseColor(red, green, blue);
    }

    public int ReadInt()
    {
        Length += 4;
        var k1 = _sr.ReadByte();
        if (k1 < 0)
        {
            return 0;
        }

        var k2 = _sr.ReadByte() << 8;
        var k3 = _sr.ReadByte() << 16;
        return k1 + k2 + k3 + (_sr.ReadByte() << 24);
    }

    public int ReadShort()
    {
        var k = ReadWord();
        if (k > 0x7fff)
        {
            k -= 0x10000;
        }

        return k;
    }

    public int ReadWord()
    {
        Length += 2;
        var k1 = _sr.ReadByte();
        if (k1 < 0)
        {
            return 0;
        }

        return (k1 + (_sr.ReadByte() << 8)) & 0xffff;
    }

    public void Skip(int len)
    {
        Length += len;
        Utilities.Skip(_sr, len);
    }
}