namespace iTextSharp.text.rtf.parser.properties;

/// <summary>
///     RtfPropertyListener  interface for handling events.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public interface IRtfPropertyListener : IEventListener
{
    /// <summary>
    /// </summary>
    void BeforePropertyChange(string propertyName);

    /// <summary>
    /// </summary>
    void AfterPropertyChange(string propertyName);
}