namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfDashPattern  defines a dash pattern as described in
///     the PDF Reference Manual version 1.3 p 325 (section 8.4.3).
///     @see        PdfArray
/// </summary>
public class PdfDashPattern : PdfArray
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     This is the length of a dash.
    /// </summary>
    private readonly float _dash = -1;

    /// <summary>
    ///     This is the length of a gap.
    /// </summary>
    private readonly float _gap = -1;

    /// <summary>
    ///     This is the phase.
    /// </summary>
    private readonly float _phase = -1;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a new  PdfDashPattern .
    /// </summary>
    public PdfDashPattern()
    {
    }

    /// <summary>
    ///     Constructs a new  PdfDashPattern .
    /// </summary>
    public PdfDashPattern(float dash) : base(new PdfNumber(dash)) => _dash = dash;

    /// <summary>
    ///     Constructs a new  PdfDashPattern .
    /// </summary>
    public PdfDashPattern(float dash, float gap) : base(new PdfNumber(dash))
    {
        Add(new PdfNumber(gap));
        _dash = dash;
        _gap = gap;
    }

    /// <summary>
    ///     Constructs a new  PdfDashPattern .
    /// </summary>
    public PdfDashPattern(float dash, float gap, float phase) : base(new PdfNumber(dash))
    {
        Add(new PdfNumber(gap));
        _dash = dash;
        _gap = gap;
        _phase = phase;
    }

    public void Add(float n)
    {
        Add(new PdfNumber(n));
    }

    /// <summary>
    ///     Returns the PDF representation of this  PdfArray .
    /// </summary>
    /// <returns>an array of  byte s</returns>
    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        os.WriteByte((byte)'[');

        if (_dash >= 0)
        {
            new PdfNumber(_dash).ToPdf(writer, os);
            if (_gap >= 0)
            {
                os.WriteByte((byte)' ');
                new PdfNumber(_gap).ToPdf(writer, os);
            }
        }

        os.WriteByte((byte)']');
        if (_phase >= 0)
        {
            os.WriteByte((byte)' ');
            new PdfNumber(_phase).ToPdf(writer, os);
        }
    }
}