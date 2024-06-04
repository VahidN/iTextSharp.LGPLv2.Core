namespace iTextSharp.text.pdf;

public class BadPasswordException : IOException
{
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