namespace iTextSharp.text.rtf.parser.exceptions;

/// <summary>
///     Signals that an error has occurred in a  RtfParser .
/// </summary>
/// <summary>
///     RtfParserException  is the exception object thrown by
///     the parser
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfParserException : Exception
{
    /// <summary>
    ///     Creates a RtfParserException object.
    /// </summary>
    /// <param name="ex">an exception that has to be turned into a RtfParserException</param>
    public RtfParserException(Exception ex) : base("", ex)
    {
    }

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  RtfParserException  whithout a message.
    /// </summary>
    public RtfParserException()
    {
    }

    /// <summary>
    ///     Constructs a  RtfParserException  with a message.
    /// </summary>
    /// <param name="message">a message describing the exception</param>
    public RtfParserException(string message) : base(message)
    {
    }

    public RtfParserException(string message, Exception innerException) : base(message, innerException)
    {
    }
}