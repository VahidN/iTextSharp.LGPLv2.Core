using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationListener  interface for handling events.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public interface IRtfDestinationListener : IEventListener
{
    /// <summary>
    /// </summary>
    int AfterCharacter(int ch);

    /// <summary>
    /// </summary>
    RtfCtrlWordData AfterCtrlWord(RtfCtrlWordData ctrlWordData);

    /// <summary>
    /// </summary>
    int BeforeCharacter(int ch);

    /// <summary>
    /// </summary>
    RtfCtrlWordData BeforeCtrlWord(RtfCtrlWordData ctrlWordData);

    /// <summary>
    /// </summary>
    int OnCharacter(int ch);

    /// <summary>
    /// </summary>
    /// <returns></returns>
    bool OnCloseGroup();

    /// <summary>
    /// </summary>
    RtfCtrlWordData OnCtrlWord(RtfCtrlWordData ctrlWordData);

    /// <summary>
    /// </summary>
    /// <returns></returns>
    bool OnOpenGroup();
}