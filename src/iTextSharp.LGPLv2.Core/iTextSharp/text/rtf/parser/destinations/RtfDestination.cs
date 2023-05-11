using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestination  is the base class for destinations according
///     to the RTF Specification. All destinations must extend from this class.
///     @author Howard Shank (hgshank@yahoo.com
///     @since 2.0.8
/// </summary>
public abstract class RtfDestination
{
    /// <summary>
    ///     The  RtfDestinationListener .
    /// </summary>
    private static readonly List<IRtfDestinationListener> _listeners = new();

    /// <summary>
    ///     The last control word handled by this destination
    /// </summary>
    protected RtfCtrlWordData LastCtrlWord = null;

    /// <summary>
    ///     Is data in destination modified?
    /// </summary>
    protected bool Modified = false;

    /// <summary>
    ///     Parser object
    /// </summary>
    protected RtfParser RtfParser;

    /// <summary>
    ///     Constructor.
    /// </summary>
    protected RtfDestination() => RtfParser = null;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="parser"> RtfParser  object.</param>
    protected RtfDestination(RtfParser parser) => RtfParser = parser;

    /// <summary>
    ///     Adds a  RtfDestinationListener  to the  RtfDestinationMgr .
    ///     the new RtfDestinationListener.
    /// </summary>
    /// <param name="listener"></param>
    public static bool AddListener(IRtfDestinationListener listener)
    {
        _listeners.Add(listener);
        return true;
    }

    /// <summary>
    ///     Clean up when destination is closed.
    /// </summary>
    /// <returns>true if handled, false if not handled</returns>
    public abstract bool CloseDestination();

    public virtual int GetNewTokeniserState() => RtfParser.TOKENISER_IGNORE_RESULT;

    /// <summary>
    ///     Handle text for this destination
    /// </summary>
    /// <returns>true if handled, false if not handled</returns>
    public abstract bool HandleCharacter(int ch);

    /// <summary>
    ///     Clean up when group is closed.
    /// </summary>
    /// <returns>true if handled, false if not handled</returns>
    public abstract bool HandleCloseGroup();

    /// <summary>
    ///     Handle control word for this destination
    /// </summary>
    /// <param name="ctrlWordData">The control word and parameter information object</param>
    /// <returns>true if handled, false if not handled</returns>
    public abstract bool HandleControlWord(RtfCtrlWordData ctrlWordData);

    /// <summary>
    ///     Setup when group is opened.
    /// </summary>
    /// <returns>true if handled, false if not handled</returns>
    public abstract bool HandleOpenGroup();

    /// <summary>
    ///     Handle a new subgroup contained within this group
    /// </summary>
    /// <returns>true if handled, false if not handled</returns>
    public abstract bool HandleOpeningSubGroup();

    /// <summary>
    ///     Method to indicate if data in this destination has changed.
    /// </summary>
    /// <returns>true if modified, false if not modified.</returns>
    public bool IsModified() => Modified;

    /// <summary>
    ///     listener methods
    /// </summary>
    /// <summary>
    ///     Removes a  RtfDestinationListener  from the  RtfDestinationMgr .
    ///     the RtfCtrlWordListener that has to be removed.
    /// </summary>
    /// <param name="listener"></param>
    public static bool RemoveListener(IRtfDestinationListener listener)
    {
        var i = _listeners.IndexOf(listener);
        if (i >= 0)
        {
            _listeners.RemoveAt(i);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Set the parser to use with the RtfDestination object.
    /// </summary>
    /// <param name="parser">The RtfParser object.</param>
    public virtual void SetParser(RtfParser parser)
    {
        if (RtfParser != null && RtfParser.Equals(parser))
        {
            return;
        }

        RtfParser = parser;
    }

    /// <summary>
    ///     Method to set this object to the default values. Must be implemented in child class.
    /// </summary>
    public abstract void SetToDefaults();

    /// <summary>
    /// </summary>
    protected static int AfterCharacter(int ch)
    {
        foreach (var listener in _listeners)
        {
            listener.AfterCharacter(ch);
        }

        return 0;
    }

    /// <summary>
    /// </summary>
    protected static RtfCtrlWordData AfterCtrlWord(RtfCtrlWordData ctrlWordData)
    {
        foreach (var listener in _listeners)
        {
            listener.AfterCtrlWord(ctrlWordData);
        }

        return null;
    }

    /// <summary>
    /// </summary>
    protected static int BeforeCharacter(int ch)
    {
        foreach (var listener in _listeners)
        {
            listener.BeforeCharacter(ch);
        }

        return 0;
    }

    protected static RtfCtrlWordData BeforeCtrlWord(RtfCtrlWordData ctrlWordData)
    {
        foreach (var listener in _listeners)
        {
            listener.BeforeCtrlWord(ctrlWordData);
        }

        return null;
    }

    /// <summary>
    /// </summary>
    protected static int OnCharacter(int ch)
    {
        foreach (var listener in _listeners)
        {
            listener.OnCharacter(ch);
        }

        return 0;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    protected static bool OnCloseGroup()
    {
        foreach (var listener in _listeners)
        {
            listener.OnCloseGroup();
        }

        return true;
    }

    /// <summary>
    /// </summary>
    protected static RtfCtrlWordData OnCtrlWord(RtfCtrlWordData ctrlWordData)
    {
        foreach (var listener in _listeners)
        {
            listener.OnCtrlWord(ctrlWordData);
        }

        return null;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    protected static bool OnOpenGroup()
    {
        foreach (var listener in _listeners)
        {
            listener.OnOpenGroup();
        }

        return true;
    }
}