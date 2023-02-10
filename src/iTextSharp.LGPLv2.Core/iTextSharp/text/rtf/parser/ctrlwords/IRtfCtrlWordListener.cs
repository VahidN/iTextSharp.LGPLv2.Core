namespace iTextSharp.text.rtf.parser.ctrlwords;

/// <summary>
///     RtfCtrlWordListener  interface for handling events.
///     @author Howard Shank (hgshank@yahoo.com)
/// </summary>
public interface IRtfCtrlWordListener : IEventListener
{
    /// <summary>
    /// </summary>
    /// <returns>null or modified copy of the ctrlWordData object</returns>
    RtfCtrlWordData BeforeCtrlWord(RtfCtrlWordData ctrlWordData);

    /// <summary>
    /// </summary>
    /// <returns>null or modified copy of the ctrlWordData object</returns>
    RtfCtrlWordData OnCtrlWord(RtfCtrlWordData ctrlWordData);

    /// <summary>
    /// </summary>
    /// <returns>null or modified copy of the ctrlWordData object</returns>
    RtfCtrlWordData AfterCtrlWord(RtfCtrlWordData ctrlWordData);
}