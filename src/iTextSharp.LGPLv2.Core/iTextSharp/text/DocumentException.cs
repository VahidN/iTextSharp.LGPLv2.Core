namespace iTextSharp.text;

/// <summary>
///     Signals that an error has occurred in a Document.
/// </summary>
/// <seealso cref="T:iTextSharp.text.BadElementException" />
/// <seealso cref="T:iTextSharp.text.Document" />
/// <seealso cref="T:iTextSharp.text.DocWriter" />
/// <seealso cref="T:iTextSharp.text.IDocListener" />
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
}