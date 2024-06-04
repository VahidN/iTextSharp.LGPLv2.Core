namespace iTextSharp.text.pdf;

/// <summary>
///     The default class that is used to determine whether or not a character
///     is a split character. You can subclass this class to define your own
///     split characters.
///     @since	2.1.2
/// </summary>
public class DefaultSplitCharacter : ISplitCharacter
{
    /// <summary>
    ///     An instance of the default SplitCharacter.
    /// </summary>
    public static readonly ISplitCharacter Default = new DefaultSplitCharacter();

    /// <summary>
    ///     Checks if a character can be used to split a  PdfString .
    ///     for the moment every character less than or equal to SPACE, the character '-'
    ///     and some specific unicode ranges are 'splitCharacters'.
    /// </summary>
    /// <param name="start">start position in the array</param>
    /// <param name="current">current position in the array</param>
    /// <param name="end">end position in the array</param>
    /// <param name="cc">array that has to be checked</param>
    /// <param name="ck">chunk array</param>
    /// <returns>if the character can be used to split a string,  false  otherwise</returns>
    public bool IsSplitCharacter(int start, int current, int end, char[] cc, PdfChunk[] ck)
    {
        var c = GetCurrentCharacter(current, cc, ck);
        if (c <= ' ' || c == '-' || c == '\u2010')
        {
            return true;
        }

        if (c < 0x2002)
        {
            return false;
        }

        return (c >= 0x2002 && c <= 0x200b)
               || (c >= 0x2e80 && c < 0xd7a0)
               || (c >= 0xf900 && c < 0xfb00)
               || (c >= 0xfe30 && c < 0xfe50)
               || (c >= 0xff61 && c < 0xffa0);
    }

    /// <summary>
    ///     Returns the current character
    /// </summary>
    /// <param name="current">current position in the array</param>
    /// <param name="cc">array that has to be checked</param>
    /// <param name="ck">chunk array</param>
    /// <returns>current character</returns>
    protected static char GetCurrentCharacter(int current, char[] cc, PdfChunk[] ck)
    {
        if (cc == null)
        {
            throw new ArgumentNullException(nameof(cc));
        }

        if (ck == null)
        {
            return cc[current];
        }

        return (char)ck[Math.Min(current, ck.Length - 1)].GetUnicodeEquivalent(cc[current]);
    }
}