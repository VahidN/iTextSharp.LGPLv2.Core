namespace iTextSharp.text.pdf;

/// <summary>
///     a Literal
/// </summary>
public class PdfLiteral : PdfObject
{
    public PdfLiteral(string text) : base(0, text)
    {
    }

    public PdfLiteral(byte[] b) : base(0, b)
    {
    }

    public PdfLiteral(int type, string text) : base(type, text)
    {
    }

    public PdfLiteral(int type, byte[] b) : base(type, b)
    {
    }

    public PdfLiteral(int size) : base(0, (byte[])null)
    {
        Bytes = new byte[size];
        for (var k = 0; k < size; ++k)
        {
            Bytes[k] = 32;
        }
    }

    public int Position { get; private set; }

    public int PosLength
    {
        get
        {
            if (Bytes != null)
            {
                return Bytes.Length;
            }

            return 0;
        }
    }

    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (os is OutputStreamCounter)
        {
            Position = ((OutputStreamCounter)os).Counter;
        }

        base.ToPdf(writer, os);
    }
}