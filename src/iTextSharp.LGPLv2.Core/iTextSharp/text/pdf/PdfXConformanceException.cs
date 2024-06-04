namespace iTextSharp.text.pdf;

/// <summary>
///     @author  psoares
/// </summary>
public class PdfXConformanceException : Exception
{
    /// <summary>
    ///     Creates a new instance of PdfXConformanceException.
    /// </summary>
    public PdfXConformanceException()
    {
    }

    /// <summary>
    ///     Creates a new instance of PdfXConformanceException.
    /// </summary>
    /// <param name="s"></param>
    public PdfXConformanceException(string s) : base(s)
    {
    }

    public PdfXConformanceException(string message, Exception innerException) : base(message, innerException)
    {
    }
}