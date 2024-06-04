namespace iTextSharp.text.pdf;

/// <summary>
///     PdfNumber  provides two types of numbers, int and real.
///     ints may be specified by signed or unsigned constants. Reals may only be
///     in decimal format.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.3 (page 37).
///     @see        PdfObject
///     @see        BadPdfFormatException
/// </summary>
public class PdfNumber : PdfObject
{
    /// <summary>
    ///     actual value of this  PdfNumber , represented as a  double
    /// </summary>
    private double _value;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfNumber -object.
    /// </summary>
    /// <param name="content">value of the new  PdfNumber -object</param>
    public PdfNumber(string content) : base(NUMBER)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        _value = double.Parse(content.Trim(), NumberFormatInfo.InvariantInfo);
        Content = content;
    }

    /// <summary>
    ///     Constructs a new int  PdfNumber -object.
    /// </summary>
    /// <param name="value">value of the new  PdfNumber -object</param>
    public PdfNumber(int value) : base(NUMBER)
    {
        _value = value;
        Content = value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Constructs a new REAL  PdfNumber -object.
    /// </summary>
    /// <param name="value">value of the new  PdfNumber -object</param>
    public PdfNumber(double value) : base(NUMBER)
    {
        _value = value;
        Content = ByteBuffer.FormatDouble(value);
    }

    /// <summary>
    ///     Constructs a new REAL  PdfNumber -object.
    /// </summary>
    /// <param name="value">value of the new  PdfNumber -object</param>
    public PdfNumber(float value) : this((double)value)
    {
    }

    /// <summary>
    ///     methods returning the value of this object
    /// </summary>
    /// <summary>
    ///     Returns the primitive  int  value of this object.
    /// </summary>
    /// <returns>a value</returns>

    public double DoubleValue => _value;

    /// <summary>
    ///     Returns the primitive  double  value of this object.
    /// </summary>
    /// <returns>a value</returns>
    public float FloatValue => (float)_value;

    public int IntValue => (int)_value;

    /// <summary>
    ///     other methods
    /// </summary>
    /// <summary>
    ///     Increments the value of the  PdfNumber -object with 1.
    /// </summary>
    public void Increment()
    {
        _value += 1.0;
        Content = ByteBuffer.FormatDouble(_value);
    }
}