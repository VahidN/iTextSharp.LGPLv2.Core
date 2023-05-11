namespace iTextSharp.text.rtf.parser.exceptions;

public class RtfUnknownCtrlWordException : RtfParserException
{
    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  RtfParserException  whithout a message.
    /// </summary>
    public RtfUnknownCtrlWordException() : base("Unknown control word.")
    {
    }

    public RtfUnknownCtrlWordException(string message) : base(message)
    {
    }

    public RtfUnknownCtrlWordException(string message, Exception innerException) : base(message, innerException)
    {
    }
}