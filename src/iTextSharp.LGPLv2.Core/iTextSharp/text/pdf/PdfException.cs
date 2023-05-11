namespace iTextSharp.text.pdf;

/// <summary>
///     Signals that an unspecified problem while constructing a PDF document.
///     @see        BadPdfFormatException
/// </summary>
public class PdfException : DocumentException
{
    public PdfException()
    {
    }

    public PdfException(string message) : base(message)
    {
    }

    public PdfException(string message, Exception innerException) : base(message, innerException)
    {
    }
}