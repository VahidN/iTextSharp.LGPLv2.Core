namespace iTextSharp.text.pdf;

/// <summary>
///     PdfBoolean  is the bool object represented by the keywords <VAR>true</VAR> or <VAR>false</VAR>.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.2 (page 37).
///     @see        PdfObject
///     @see        BadPdfFormatException
/// </summary>
public class PdfBoolean : PdfObject
{
    /// <summary>
    ///     A possible value of  PdfBoolean
    /// </summary>
    public const string FALSE = "false";

    /// <summary>
    ///     A possible value of  PdfBoolean
    /// </summary>
    public const string TRUE = "true";

    public static readonly PdfBoolean Pdffalse = new(false);

    /// <summary>
    ///     static membervariables (possible values of a bool object)
    /// </summary>
    public static readonly PdfBoolean Pdftrue = new(true);

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     the bool value of this object
    /// </summary>
    private readonly bool _value;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfBoolean -object.
    /// </summary>
    /// <param name="value">the value of the new  PdfObject </param>
    public PdfBoolean(bool value) : base(BOOLEAN)
    {
        if (value)
        {
            Content = TRUE;
        }
        else
        {
            Content = FALSE;
        }

        _value = value;
    }

    /// <summary>
    ///     Constructs a  PdfBoolean -object.
    ///     @throws        BadPdfFormatException    thrown if the <VAR>value</VAR> isn't ' true ' or ' false '
    /// </summary>
    /// <param name="value">the value of the new  PdfObject , represented as a  string </param>
    public PdfBoolean(string value) : base(BOOLEAN, value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (value.Equals(TRUE, StringComparison.Ordinal))
        {
            _value = true;
        }
        else if (value.Equals(FALSE, StringComparison.Ordinal))
        {
            _value = false;
        }
        else
        {
            throw new BadPdfFormatException("The value has to be 'true' of 'false', instead of '" + value + "'.");
        }
    }

    /// <summary>
    ///     methods returning the value of this object
    /// </summary>
    /// <summary>
    ///     Returns the primitive value of the  PdfBoolean -object.
    /// </summary>
    /// <returns>the actual value of the object.</returns>
    public bool BooleanValue => _value;

    public override string ToString() => _value ? TRUE : FALSE;
}