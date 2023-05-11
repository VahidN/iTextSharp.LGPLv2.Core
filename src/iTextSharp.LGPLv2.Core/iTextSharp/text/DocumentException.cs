namespace iTextSharp.text;

/// <summary>
///     Signals that an error has occurred in a Document.
/// </summary>
public class DocumentException : Exception
{
    /// <summary>
    ///     Constructs a new DocumentException
    /// </summary>
    /// <overloads>
    ///     Has two overloads.
    /// </overloads>
    public DocumentException()
    {
    }

    /// <summary>
    ///     Construct a new DocumentException
    /// </summary>
    /// <param name="message">error message</param>
    public DocumentException(string message) : base(message)
    {
    }

    public DocumentException(string message, Exception innerException) : base(message, innerException)
    {
    }
}