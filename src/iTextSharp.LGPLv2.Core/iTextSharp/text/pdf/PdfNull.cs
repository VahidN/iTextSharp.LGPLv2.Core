namespace iTextSharp.text.pdf;

/// <summary>
///     PdfNull  is the Null object represented by the keyword <VAR>null</VAR>.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.9 (page 53).
///     @see        PdfObject
/// </summary>
public class PdfNull : PdfObject
{
    /// <summary>
    ///     static membervariables
    /// </summary>
    /// <summary>
    ///     This is an instance of the  PdfNull -object.
    /// </summary>
    public static PdfNull Pdfnull = new();

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfNull -object.
    ///     You never need to do this yourself, you can always use the static object <VAR>PDFNULL</VAR>.
    /// </summary>
    public PdfNull() : base(NULL, "null")
    {
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => "null";
}