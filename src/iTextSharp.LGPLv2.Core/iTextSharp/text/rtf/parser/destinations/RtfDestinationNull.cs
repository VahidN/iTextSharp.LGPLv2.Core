using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationNull  is for discarded entries. They go nowhere.
///     If a control word destination is unknown or ignored, this is the destination
///     that should be set.
///     All methods return true indicating they were handled.
///     This is a unique destination in that it is a singleton.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public sealed class RtfDestinationNull : RtfDestination
{
    private static readonly RtfDestinationNull _instance = new();

    /// <summary>
    ///     Constructs a new RtfDestinationNull.
    ///     This constructor is hidden for internal use only.
    /// </summary>
    private RtfDestinationNull()
    {
    }

    /// <summary>
    ///     Constructs a new RtfDestinationNull.
    ///     This constructor is hidden for internal use only.
    /// </summary>
    /// <param name="parser">Unused value</param>
    private RtfDestinationNull(RtfParser parser) : base(null)
    {
    }

    /// <summary>
    ///     Get the singleton instance of RtfDestinationNull object.
    /// </summary>
    public static RtfDestinationNull GetInstance() => _instance;

    public static string GetName() => typeof(RtfDestinationNull).Name;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
    /// </summary>
    public override bool CloseDestination() => true;

    public override int GetNewTokeniserState() => RtfParser.TOKENISER_SKIP_GROUP;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
    /// </summary>
    public override bool HandleCharacter(int ch) => true;

    /// <summary>
    ///     Interface definitions
    /// </summary>
    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
    /// </summary>
    public override bool HandleCloseGroup() => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see
    ///     com.lowagie.text.rtf.parser.destinations.RtfDestination#handleControlWord(com.lowagie.text.rtf.parser.ctrlwords.RtfCtrlWordData)
    /// </summary>
    public override bool HandleControlWord(RtfCtrlWordData ctrlWordData) => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
    /// </summary>
    public override bool HandleOpenGroup() => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
    /// </summary>
    public override bool HandleOpeningSubGroup() => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#setDefaults()
    /// </summary>
    public override void SetToDefaults()
    {
    }
}