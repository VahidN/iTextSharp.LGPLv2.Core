namespace iTextSharp.text.rtf.parser.ctrlwords;

/// <summary>
///     The control word and parameter information as parsed by the parser.
///     Contains the control word,
///     Flag indicating if there is a parameter.
///     The parameter value as a string.
///     Flag indicating the parameter is positive or negative.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfCtrlWordData
{
    /// <summary>
    ///     The control word found by the parser
    /// </summary>
    public string CtrlWord = "";

    public int CtrlWordType = RtfCtrlWordType.UNIDENTIFIED;

    /// <summary>
    ///     Flag indicating if this keyword has a parameter.
    /// </summary>
    public bool HasParam = false;

    /// <summary>
    ///     Flag indicating if parameter is positive or negative.
    /// </summary>
    public bool IsNeg = false;

    /// <summary>
    ///     Flag indicating if this object has been modified.
    /// </summary>
    public bool Modified = false;

    /// <summary>
    ///     Flag indicating a new group
    /// </summary>
    public bool NewGroup = false;

    /// <summary>
    ///     The parameter for the control word.
    /// </summary>
    public string Param = "";

    public string Prefix = "";
    public string SpecialHandler = "";
    public string Suffix = "";

    /// <summary>
    ///     Return the parameter value as an integer (int) value.
    ///     Returns the parameter value as an int vlaue.
    /// </summary>
    /// <returns></returns>
    public int IntValue()
    {
        int value;
        value = int.Parse(Param, CultureInfo.InvariantCulture);
        if (IsNeg)
        {
            value = -value;
        }

        return value;
    }

    /// <summary>
    ///     Return the parameter value as a long value
    ///     Returns the parameter value as a long value
    /// </summary>
    /// <returns></returns>
    public long LongValue()
    {
        long value;
        value = long.Parse(Param, CultureInfo.InvariantCulture);
        if (IsNeg)
        {
            value = -value;
        }

        return value;
    }

    public override string ToString()
    {
        var outp = "";
        outp = Prefix + CtrlWord;
        if (HasParam)
        {
            if (IsNeg)
            {
                outp += "-";
            }

            outp += Param;
        }

        outp += Suffix;
        return outp;
    }
}