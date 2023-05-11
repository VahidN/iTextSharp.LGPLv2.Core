namespace iTextSharp.text.exceptions;

/// <summary>
///     Typed exception used when opening an existing PDF document.
///     Gets thrown when the document isn't a valid PDF document.
/// </summary>
public class InvalidPdfException : IOException
{
    /// <summary>
    ///     Creates an instance of a NoPdfException.
    /// </summary>
    /// <param name="message">the reason why the document isn't a PDF document according to iText.</param>
    public InvalidPdfException(string message) : base(message)
    {
    }

    public InvalidPdfException()
    {
    }

    public InvalidPdfException(string message, Exception innerException) : base(message, innerException)
    {
    }
}