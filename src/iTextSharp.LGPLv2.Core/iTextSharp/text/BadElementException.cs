namespace iTextSharp.text;

/// <summary>
///     Signals an attempt to create an Element that hasn't got the right form.
/// </summary>
public class BadElementException : DocumentException
{
    public BadElementException()
    {
    }

    public BadElementException(string message) : base(message)
    {
    }

    public BadElementException(string message, Exception innerException) : base(message, innerException)
    {
    }
}