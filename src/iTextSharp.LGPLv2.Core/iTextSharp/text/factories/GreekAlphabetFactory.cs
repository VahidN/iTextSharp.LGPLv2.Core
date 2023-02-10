namespace iTextSharp.text.factories;

/// <summary>
///     This class can produce String combinations representing a number built with
///     Greek letters (from alpha to omega, then alpha alpha, alpha beta, alpha gamma).
///     We are aware of the fact that the original Greek numbering is different;
///     See http://www.cogsci.indiana.edu/farg/harry/lan/grknum.htm#ancient
///     but this isn't implemented yet; the main reason being the fact that we
///     need a font that has the obsolete Greek characters qoppa and sampi.
/// </summary>
public static class GreekAlphabetFactory
{
    /// <summary>
    ///     Changes an int into a lower case Greek letter combination.
    /// </summary>
    /// <param name="index">the original number</param>
    /// <returns>the letter combination</returns>
    public static string GetString(int index) => GetString(index, true);

    /// <summary>
    ///     Changes an int into a lower case Greek letter combination.
    /// </summary>
    /// <param name="index">the original number</param>
    /// <returns>the letter combination</returns>
    public static string GetLowerCaseString(int index) => GetString(index);

    /// <summary>
    ///     Changes an int into a upper case Greek letter combination.
    /// </summary>
    /// <param name="index">the original number</param>
    /// <returns>the letter combination</returns>
    public static string GetUpperCaseString(int index) => GetString(index).ToUpper(CultureInfo.InvariantCulture);

    /// <summary>
    ///     Changes an int into a Greek letter combination.
    /// </summary>
    /// <param name="index">the original number</param>
    /// <param name="lowercase"></param>
    /// <returns>the letter combination</returns>
    public static string GetString(int index, bool lowercase)
    {
        if (index < 1)
        {
            return "";
        }

        index--;

        var bytes = 1;
        var start = 0;
        var symbols = 24;
        while (index >= symbols + start)
        {
            bytes++;
            start += symbols;
            symbols *= 24;
        }

        var c = index - start;
        var value = new char[bytes];
        while (bytes > 0)
        {
            bytes--;
            value[bytes] = (char)(c % 24);
            if (value[bytes] > 16)
            {
                value[bytes]++;
            }

            value[bytes] += (char)(lowercase ? 945 : 913);
            value[bytes] = SpecialSymbol.GetCorrespondingSymbol(value[bytes]);
            c /= 24;
        }

        return new string(value);
    }
}