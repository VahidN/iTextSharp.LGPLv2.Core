namespace iTextSharp.text.exceptions;

/// <summary>
///     Typed exception used when creating PDF syntax that isn't valid.
/// </summary>
public class IllegalPdfSyntaxException : ArgumentException
{
    /// <summary>
    ///     Creates an exception saying the PDF syntax isn't correct.
    /// </summary>
    /// <param name="message">message	some extra info about the exception</param>
    public IllegalPdfSyntaxException(string message) : base(message)
    {
    }

    public IllegalPdfSyntaxException()
    {
    }

    public IllegalPdfSyntaxException(string message, Exception innerException) : base(message, innerException)
    {
    }
}