namespace iTextSharp.text.rtf.parser.ctrlwords;

/// <summary>
///     RtfCtrlWordMgr  handles the dispatching of control words from
///     the table of known control words.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public sealed class RtfCtrlWordMgr
{
    public static bool Debug = false;
    public static bool DebugFound = false;
    public static bool DebugNotFound = true;
    private readonly RtfCtrlWordMap _ctrlWordMap;

    /// <summary>
    ///     The  RtfCtrlWordListener .
    /// </summary>
    private readonly List<IRtfCtrlWordListener> _listeners = new();

    private PushbackStream _reader;
    private RtfParser _rtfParser;

    /// <summary>
    ///     // TIMING DEBUG INFO
    /// </summary>
    /// <summary>
    ///     private long endTime = 0;
    /// </summary>
    /// <summary>
    ///     private Date endDate = null;
    /// </summary>
    /// <summary>
    ///     private long endFree = 0;
    /// </summary>
    /// <summary>
    ///     private DecimalFormat df = new DecimalFormat("#,##0");
    /// </summary>
    /// <summary>
    ///     private Date startDate = new Date();
    /// </summary>
    /// <summary>
    ///     private long startTime = System.CurrentTimeMillis();
    /// </summary>
    /// <summary>
    ///     private long startFree = Runtime.GetRuntime().FreeMemory();
    /// </summary>
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="rtfParser">The parser object this manager works with.</param>
    /// <param name="reader">the PushbackReader from the tokeniser.</param>
    public RtfCtrlWordMgr(RtfParser rtfParser, PushbackStream reader)
    {
        _rtfParser = rtfParser; // set the parser
        _reader = reader; // set the reader value
        _ctrlWordMap = new RtfCtrlWordMap(rtfParser);

        //      // TIMING DEBUG INFO
        //      endFree = Runtime.GetRuntime().FreeMemory();
        //      endTime = System.CurrentTimeMillis();
        //      endDate = new Date();
        //      Console.Out.WriteLine("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
        //      Console.Out.WriteLine("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
        //      Console.Out.WriteLine("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
        //      Console.Out.WriteLine("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
        //      Console.Out.WriteLine("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
        //        Console.Out.WriteLine("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
    }

    /// <summary>
    ///     Adds a  RtfCtrlWordListener  to the  RtfCtrlWordMgr .
    ///     the new RtfCtrlWordListener.
    /// </summary>
    /// <param name="listener"></param>
    public void AddRtfCtrlWordListener(IRtfCtrlWordListener listener)
    {
        _listeners.Add(listener);
    }

    /// <summary>
    ///     Internal to control word manager class.
    /// </summary>
    /// <param name="ctrlWordData">The  RtfCtrlWordData  object with control word and param</param>
    /// <param name="groupLevel">The current document group parsing level</param>
    /// <returns>errOK if ok, otherwise an error code.</returns>
    public int HandleKeyword(RtfCtrlWordData ctrlWordData, int groupLevel)
    {
        //TODO: May be used for event handling.
        var result = RtfParser.errOK;

        // Call before handler event here
        beforeCtrlWord(ctrlWordData);

        result = dispatchKeyword(ctrlWordData, groupLevel);

        // call after handler event here
        afterCtrlWord(ctrlWordData);

        return result;
    }

    /// <summary>
    ///     listener methods
    /// </summary>
    /// <summary>
    ///     Removes a  RtfCtrlWordListener  from the  RtfCtrlWordMgr .
    ///     the RtfCtrlWordListener that has to be removed.
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveRtfCtrlWordListener(IRtfCtrlWordListener listener)
    {
        _listeners.Remove(listener);
    }

    private bool afterCtrlWord(RtfCtrlWordData ctrlWordData)
    {
        foreach (var listener in _listeners)
        {
            listener.AfterCtrlWord(ctrlWordData);
        }

        return true;
    }

    private bool beforeCtrlWord(RtfCtrlWordData ctrlWordData)
    {
        foreach (var listener in _listeners)
        {
            listener.BeforeCtrlWord(ctrlWordData);
        }

        return true;
    }

    /// <summary>
    ///     Dispatch the token to the correct control word handling object.
    /// </summary>
    /// <param name="ctrlWordData">The  RtfCtrlWordData  object with control word and param</param>
    /// <param name="groupLevel">The current document group parsing level</param>
    /// <returns>errOK if ok, otherwise an error code.</returns>
    private int dispatchKeyword(RtfCtrlWordData ctrlWordData, int groupLevel)
    {
        var result = RtfParser.errOK;
        if (ctrlWordData != null)
        {
            var ctrlWord = _ctrlWordMap.GetCtrlWordHandler(ctrlWordData.CtrlWord);
            if (ctrlWord != null)
            {
                ctrlWord.HandleControlword(ctrlWordData);
                if (Debug && DebugFound)
                {
                    Console.Out.WriteLine("Keyword found:" +
                                          " New:" + ctrlWordData.CtrlWord +
                                          " Param:" + ctrlWordData.Param +
                                          " bParam=" + ctrlWordData.HasParam);
                }
            }
            else
            {
                result = RtfParser.errCtrlWordNotFound;
                //result = RtfParser2.errAssertion;
                if (Debug && DebugNotFound)
                {
                    Console.Out.WriteLine("Keyword unknown:" +
                                          " New:" + ctrlWordData.CtrlWord +
                                          " Param:" + ctrlWordData.Param +
                                          " bParam=" + ctrlWordData.HasParam);
                }
            }
        }

        return result;
    }

    private bool onCtrlWord(RtfCtrlWordData ctrlWordData)
    {
        foreach (var listener in _listeners)
        {
            listener.OnCtrlWord(ctrlWordData);
        }

        return true;
    }
}