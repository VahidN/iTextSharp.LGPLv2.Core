using iTextSharp.text.rtf.parser.destinations;

namespace iTextSharp.text.rtf.parser.ctrlwords;

/// <summary>
///     RtfCtrlWordBase  is the base class for all
///     control word handlers to extend from.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfCtrlWordHandler
{
    /// <summary>
    ///     The control word for this class.
    ///     @since 2.0.8
    /// </summary>
    protected string CtrlWord = "";

    /// <summary>
    ///     The control word as parsed by the parser.
    ///     @since 2.0.8
    /// </summary>
    protected RtfCtrlWordData CtrlWordData;

    /// <summary>
    ///     The prefix for all control words.
    ///     @since 2.0.8
    /// </summary>
    protected string CtrlWordPrefix = "\\";

    /// <summary>
    ///     The prefix for all control words.
    ///     @since 2.0.8
    /// </summary>
    protected string CtrlWordSuffix = " ";

    /// <summary>
    ///     Control Word type. Destination, toggle, value, etc.
    ///     @since 2.0.8
    /// </summary>
    protected int CtrlWordType = RtfCtrlWordType.UNIDENTIFIED;

    /// <summary>
    ///     The default value for this control word.
    ///     Not all control words use a default parameter value.
    ///     @since 2.0.8
    /// </summary>
    protected int DefaultParameterValue;

    /// <summary>
    ///     String containing the value of "{" or "" (blank) depending on if this is the
    ///     first control word in a group.
    ///     @since 2.0.8
    /// </summary>
    protected string GroupPrefix = "";

    /// <summary>
    ///     Does this control word use the default value?
    ///     @since 2.0.8
    /// </summary>
    protected bool PassDefaultParameterValue;

    /// <summary>
    ///     Local variable referencing the parser object.
    ///     @since 2.0.8
    /// </summary>
    protected RtfParser RtfParser;

    /// <summary>
    ///     What version of the RTF spec the control word was introduced.
    ///     @since 2.0.8
    /// </summary>
    protected float RtfVersionSupported = -1.0f;

    /// <summary>
    ///     Class, property, etc.
    ///     @since 2.0.8
    /// </summary>
    protected string SpecialHandler = "";

    // -1.0 unknown. Each class should override this as implemented.
    /// <summary>
    ///     Constructor:
    ///     The parser for this control word.
    ///     The string value of this control word.
    ///     The default value of this control word. Not all control words have values.
    ///     Flag indicating if this control word should use the default value.
    ///     Indicator of the type of control word this is. DESTINATION|DESTINATION_EX|VALUE|FLAG|TOGGLE|SYMBOL
    ///     String to prefix the ctrl word with. "\" or "\*\" are the 2 used values.
    ///     String to add as suffix to the ctrl word. " " and "" are the 2 used values.
    ///     If TOGGLE then the property name as String (propertyGroup.propertyName format ex. "character.bold")
    ///     If FLAG then the property name as String (propertyGroup.propertyName format ex. "character.bold")
    ///     If VALUE then the property name as String (propertyGroup.propertyName format ex. "character.bold")
    ///     If SYMBOL then the character to use for substitution as String
    ///     If DESTINATION|DESTINATION_EX then the RtfDestination class name as String
    ///     @since 2.0.8
    /// </summary>
    /// <param name="rtfParser"></param>
    /// <param name="ctrlWord"></param>
    /// <param name="defaultParameterValue"></param>
    /// <param name="passDefaultParameterValue"></param>
    /// <param name="ctrlWordType"></param>
    /// <param name="prefix"></param>
    /// <param name="suffix"></param>
    /// <param name="specialHandler"></param>
    public RtfCtrlWordHandler(RtfParser rtfParser, string ctrlWord, int defaultParameterValue,
                              bool passDefaultParameterValue,
                              int ctrlWordType, string prefix, string suffix, string specialHandler)
    {
        RtfParser = rtfParser;
        CtrlWord = ctrlWord;
        DefaultParameterValue = defaultParameterValue;
        PassDefaultParameterValue = passDefaultParameterValue;
        CtrlWordType = ctrlWordType;
        CtrlWordPrefix = prefix;
        CtrlWordSuffix = suffix;
        SpecialHandler = specialHandler;

        if (CtrlWordType == RtfCtrlWordType.DESTINATION || CtrlWordType == RtfCtrlWordType.DESTINATION_EX)
        {
            if (SpecialHandler == null)
            {
                SpecialHandler = "RtfDestinationNull";
            }

            var arg1 = ""; // stylesheet value - S, CS, TS
            RtfDestinationMgr.AddDestination(CtrlWord, new object[] { SpecialHandler, arg1 });
        }
        else
        {
            if (CtrlWordType == RtfCtrlWordType.SYMBOL)
            {
            }
            else
            {
                if (SpecialHandler == null)
                {
                    SpecialHandler = CtrlWord; // if null, make the property the name of the ctrl word
                }
                else
                {
                    if (SpecialHandler.Length > 1 && SpecialHandler.EndsWith(".", StringComparison.Ordinal))
                    {
                        SpecialHandler +=
                            CtrlWord; // if string length>1 and ends with a period, it's a group. Add ctrlWord
                    }
                }
            }
        }
    }

    /// <summary>
    ///     The primary control word handler method.
    ///     Called by the parser once it has a control word and parameter if applicable.
    ///     The control word and associated parameter if applicable.
    ///     true  or  false  if the control word was handled.
    ///     @since 2.0.8
    /// </summary>
    /// <param name="ctrlWordDataIn"></param>
    /// <returns></returns>
    public bool HandleControlword(RtfCtrlWordData ctrlWordDataIn)
    {
        var result = false;
        CtrlWordData = ctrlWordDataIn;
        RtfDestination dest = null;
        var handled = false;

        CtrlWordData.Prefix = CtrlWordPrefix;
        CtrlWordData.Suffix = CtrlWordSuffix;
        CtrlWordData.NewGroup = RtfParser.GetState().NewGroup;
        CtrlWordData.CtrlWordType = CtrlWordType;
        CtrlWordData.SpecialHandler = SpecialHandler;

        if (!CtrlWordData.HasParam && PassDefaultParameterValue)
        {
            CtrlWordData.HasParam = true;
            CtrlWordData.Param = DefaultParameterValue.ToString(CultureInfo.InvariantCulture);
        }

        if (CtrlWordData.CtrlWord.Equals("*", StringComparison.Ordinal))
        {
            return true;
        }

        if (!BeforeControlWord())
        {
            return true;
        }

        switch (CtrlWordType)
        {
            case RtfCtrlWordType.FLAG:
            case RtfCtrlWordType.TOGGLE:
            case RtfCtrlWordType.VALUE:
                dest = RtfParser.GetCurrentDestination();
                if (dest != null)
                {
                    handled = dest.HandleControlWord(CtrlWordData);
                }

                break;

            case RtfCtrlWordType.SYMBOL:
                dest = RtfParser.GetCurrentDestination();
                if (dest != null)
                {
                    string data = null;
                    // if doing an import, then put the control word in the output stream through the character handler
                    if (RtfParser.IsImport())
                    {
                        data = CtrlWordPrefix + CtrlWordData.CtrlWord + CtrlWordSuffix;
                    }

                    if (RtfParser.IsConvert())
                    {
                        data = SpecialHandler;
                    }

                    // If there is a substitute character, process the character.
                    // If no substitute character, then provide special handling in the destination for the ctrl word.
                    if (data != null)
                    {
                        foreach (var cc in data)
                        {
                            handled = dest.HandleCharacter(cc);
                        }
                    }
                    else
                    {
                        handled = dest.HandleControlWord(CtrlWordData);
                    }
                }

                break;

            case RtfCtrlWordType.DESTINATION_EX:
            case RtfCtrlWordType.DESTINATION:
                // set the destination
                var x = 0;
                if (CtrlWord == "shppict" || CtrlWord == "nonshppict")
                {
                    x++;
                }

                handled = RtfParser.SetCurrentDestination(CtrlWord);
                // let destination handle the ctrl word now.
                dest = RtfParser.GetCurrentDestination();
                if (dest != null)
                {
                    if (dest.GetNewTokeniserState() == RtfParser.TOKENISER_IGNORE_RESULT)
                    {
                        handled = dest.HandleControlWord(CtrlWordData);
                    }
                    else
                    {
                        RtfParser.SetTokeniserState(dest.GetNewTokeniserState());
                    }
                }

                break;
        }

        AfterControlWord();
        return result;
    }

    /// <summary>
    ///     Post-processing after the control word.
    ///     @since 2.0.8
    /// </summary>
    /// <returns> false  = stop processing,  true  = continue processing</returns>
    protected static bool AfterControlWord() =>
        // TODO: This is where events would be triggered
        true;

    /// <summary>
    ///     Pre-processing before the control word.
    ///     If return value is true, no further processing will be performed on
    ///     this control word.
    ///     @since 2.0.8
    /// </summary>
    /// <returns> false  = stop processing,  true  = continue processing</returns>
    /// <summary>
    ///     Primary purpose is for \* control word and event handling.
    /// </summary>
    protected static bool BeforeControlWord() =>
        // TODO: This is where events would be triggered
        true;

    /// <summary>
    ///     Handle the control word.
    ///     @since 2.0.8
    /// </summary>
    /// <returns> true  if control word was handled,  false  if it was not handled.</returns>
    protected static bool OnControlWord() =>
        // TODO: This is where events would be triggered
        false;

    /// <summary>
    ///     Debug function to print class/method
    ///     @since 2.0.8
    /// </summary>
    /// <param name="txt">The  String  to output.</param>
    private void printDebug(string txt)
    {
        Console.Out.WriteLine(GetType().Name + " : " + txt);
    }
}