namespace iTextSharp.text.pdf;

/// <summary>
///     Signals that a bad PDF format has been used to construct a  PdfObject .
///     @see        PdfException
///     @see        PdfBoolean
///     @see        PdfNumber
///     @see        PdfString
///     @see        PdfName
///     @see        PdfDictionary
///     @see        PdfFont
/// </summary>
public class BadPdfFormatException : Exception
{
    public BadPdfFormatException()
    {
    }

    public BadPdfFormatException(string message) : base(message)
    {
    }

    public BadPdfFormatException(string message, Exception innerException) : base(message, innerException)
    {
    }
}