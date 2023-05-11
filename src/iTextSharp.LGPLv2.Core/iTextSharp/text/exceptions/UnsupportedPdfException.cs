namespace iTextSharp.text.exceptions;

/// <summary>
///     Typed exception used when opening an existing PDF document.
///     Gets thrown when the document isn't a valid PDF document according to iText,
///     but it's different from the InvalidPdfException in the sense that it may
///     be an iText limitation(most of the times it isn't but you might have
///     bumped into something that has been added to the PDF specs, but that isn't
///     supported in iText yet).
/// </summary>
public class UnsupportedPdfException : InvalidPdfException
{
    /// <summary>
    ///     Creates an instance of an UnsupportedPdfException.
    /// </summary>
    /// <param name="message">the reason why the document isn't a PDF document according to iText.</param>
    public UnsupportedPdfException(string message) : base(message)
    {
    }

    public UnsupportedPdfException()
    {
    }

    public UnsupportedPdfException(string message, Exception innerException) : base(message, innerException)
    {
    }
}