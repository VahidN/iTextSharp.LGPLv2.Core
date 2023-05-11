using System.Text;

namespace iTextSharp.text.pdf;

public class PrIndirectReference : PdfIndirectReference
{
    protected PdfReader reader;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfIndirectReference .
    /// </summary>
    /// <param name="reader">a  PdfReader </param>
    /// <param name="number">the object number.</param>
    /// <param name="generation">the generation number.</param>
    internal PrIndirectReference(PdfReader reader, int number, int generation)
    {
        type = INDIRECT;
        this.number = number;
        this.generation = generation;
        this.reader = reader;
    }

    /// <summary>
    ///     Constructs a  PdfIndirectReference .
    /// </summary>
    /// <param name="reader">a  PdfReader </param>
    /// <param name="number">the object number.</param>
    internal PrIndirectReference(PdfReader reader, int number) : this(reader, number, 0)
    {
    }

    /// <summary>
    ///     methods
    /// </summary>

    public PdfReader Reader => reader;

    public void SetNumber(int number, int generation)
    {
        this.number = number;
        this.generation = generation;
    }

    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        var n = writer.GetNewObjectNumber(reader, number, generation);
        var b = PdfEncodings.ConvertToBytes(new StringBuilder().Append(n).Append(" 0 R").ToString(), null);
        os.Write(b, 0, b.Length);
    }
}