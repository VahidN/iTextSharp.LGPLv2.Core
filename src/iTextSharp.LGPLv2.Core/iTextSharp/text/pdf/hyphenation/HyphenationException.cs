namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     @author Carlos Villegas
/// </summary>
public class HyphenationException : Exception
{
    public HyphenationException(string msg) : base(msg)
    {
    }

    public HyphenationException()
    {
    }

    public HyphenationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}