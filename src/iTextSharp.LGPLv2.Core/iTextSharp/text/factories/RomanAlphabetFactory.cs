namespace iTextSharp.text.factories;

/// <summary>
///     This class can produce String combinations representing a number.
///     "a" to "z" represent 1 to 26, "AA" represents 27, "AB" represents 28,
///     and so on; "ZZ" is followed by "AAA".
/// </summary>
public static class RomanAlphabetFactory
{
    /// <summary>
    ///     Translates a positive integer (not equal to zero)
    ///     into a String using the letters 'a' to 'z';
    ///     1 = a, 2 = b, ..., 26 = z, 27 = aa, 28 = ab,...
    /// </summary>
    public static string GetLowerCaseString(int index) => GetString(index);

    /// <summary>
    ///     Translates a positive integer (not equal to zero)
    ///     into a String using the letters 'a' to 'z';
    ///     1 = a, 2 = b, ..., 26 = z, 27 = aa, 28 = ab,...
    /// </summary>
    public static string GetString(int index)
    {
        if (index < 1)
        {
            throw new FormatException("You can't translate a negative number into an alphabetical value.");
        }

        index--;
        var bytes = 1;
        var start = 0;
        var symbols = 26;
        while (index >= symbols + start)
        {
            bytes++;
            start += symbols;
            symbols *= 26;
        }

        var c = index - start;
        var value = new char[bytes];
        while (bytes > 0)
        {
            value[--bytes] = (char)('a' + c % 26);
            c /= 26;
        }

        return new string(value);
    }

    /// <summary>
    ///     Translates a positive integer (not equal to zero)
    ///     into a String using the letters 'a' to 'z'
    ///     (a = 1, b = 2, ..., z = 26, aa = 27, ab = 28,...).
    /// </summary>
    public static string GetString(int index, bool lowercase)
    {
        if (lowercase)
        {
            return GetLowerCaseString(index);
        }

        return GetUpperCaseString(index);
    }

    /// <summary>
    ///     Translates a positive integer (not equal to zero)
    ///     into a String using the letters 'A' to 'Z';
    ///     1 = A, 2 = B, ..., 26 = Z, 27 = AA, 28 = AB,...
    /// </summary>
    public static string GetUpperCaseString(int index) => GetString(index).ToUpper(CultureInfo.InvariantCulture);
}