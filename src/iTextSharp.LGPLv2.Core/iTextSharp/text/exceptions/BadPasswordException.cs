namespace iTextSharp.text.exceptions;

/// <summary>
///     Typed exception used when opening an existing PDF document.
///     Gets thrown when the document isn't a valid PDF document.
/// </summary>
public class BadPasswordException : IOException
{
    /// <summary>
    ///     Creates an exception saying the user password was incorrect.
    /// </summary>
    public BadPasswordException(string message) : base(message)
    {
    }

    public BadPasswordException()
    {
    }

    public BadPasswordException(string message, Exception innerException) : base(message, innerException)
    {
    }
}