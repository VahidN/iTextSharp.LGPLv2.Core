namespace iTextSharp.text.rtf.parser.ctrlwords;

/// <summary>
///     RtfCtrlWordType  indicates the type of control word.
///     RTF control words are divided up into:
///     Destination, Flag, Value, Toggle, Symbol.
///     Destination: The current destination for values and text to be sent.
///     Flag: 0/1 value types. Represents true/false, on/off value types.
///     Toggle: Flips a Flag value on/off.
///     Value: an Integer value data type. (Exception: Some control words this is a long data value type)
///     Symbol: Special RTF characters such as \{, \} and others.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public sealed class RtfCtrlWordType
{
    /// <summary>
    ///     Control word is a destination.
    /// </summary>
    public const int DESTINATION = 0;

    /// <summary>
    ///     Control word is a newer destination.
    /// </summary>
    public const int DESTINATION_EX = 1;

    /// <summary>
    ///     Control word is a flag.
    /// </summary>
    public const int FLAG = 2;

    /// <summary>
    ///     Control word is a special symbol.
    /// </summary>
    public const int SYMBOL = 5;

    /// <summary>
    ///     Control word is a flag toggle.
    /// </summary>
    public const int TOGGLE = 4;

    /// <summary>
    ///     Control word is unidentified.
    /// </summary>
    public const int UNIDENTIFIED = -1;

    /// <summary>
    ///     Control word is a value.
    /// </summary>
    public const int VALUE = 3;
}