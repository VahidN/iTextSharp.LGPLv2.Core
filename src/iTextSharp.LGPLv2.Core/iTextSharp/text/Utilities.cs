using System.Text;
using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     A collection of convenience methods that were present in many different iText
///     classes.
/// </summary>
public static class Utilities
{
    private static readonly byte[] _skipBuffer = new byte[4096];

    /// <summary>
    ///     Utility method to extend an array.
    /// </summary>
    /// <param name="original">the original array or  null </param>
    /// <param name="item">the item to be added to the array</param>
    /// <returns>a new array with the item appended</returns>
    public static object[][] AddToArray(object[][] original, object[] item)
    {
        if (original == null)
        {
            original = new object[1][];
            original[0] = item;
            return original;
        }

        var original2 = new object[original.Length + 1][];
        Array.Copy(original, 0, original2, 0, original.Length);
        original2[original.Length] = item;
        return original2;
    }

    /// <summary>
    ///     Checks for a true/false value of a key in a Properties object.
    /// </summary>
    /// <param name="attributes"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool CheckTrueOrFalse(Properties attributes, string key)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        return Util.EqualsIgnoreCase("true", attributes[key]);
    }

    public static string ConvertFromUtf32(int codePoint)
    {
        if (codePoint < 0x10000)
        {
            return char.ToString((char)codePoint);
        }

        codePoint -= 0x10000;
        return new string(new[] { (char)(codePoint / 0x400 + 0xd800), (char)(codePoint % 0x400 + 0xdc00) });
    }

    public static int ConvertToUtf32(char highSurrogate, char lowSurrogate) =>
        (highSurrogate - 0xd800) * 0x400 + (lowSurrogate - 0xdc00) + 0x10000;

    public static int ConvertToUtf32(char[] text, int idx)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        return (text[idx] - 0xd800) * 0x400 + (text[idx + 1] - 0xdc00) + 0x10000;
    }

    public static int ConvertToUtf32(string text, int idx)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        return (text[idx] - 0xd800) * 0x400 + (text[idx + 1] - 0xdc00) + 0x10000;
    }

    /// <summary>
    /// </summary>
    /// <param name="table"></param>
    /// <returns></returns>
    public static ICollection<string> GetKeySet(Properties table) => table == null ? new Properties().Keys : table.Keys;

    /// <summary>
    ///     Measurement conversion from inches to millimeters.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="value">a value in inches</param>
    /// <returns>a value in millimeters</returns>
    public static float InchesToMillimeters(float value) => value * 25.4f;

    /// <summary>
    ///     Measurement conversion from inches to points.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="value">a value in inches</param>
    /// <returns>a value in points</returns>
    public static float InchesToPoints(float value) => value * 72f;

    public static bool IsSurrogateHigh(char c) => c >= '\ud800' && c <= '\udbff';

    public static bool IsSurrogateLow(char c) => c >= '\udc00' && c <= '\udfff';

    public static bool IsSurrogatePair(string text, int idx)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (idx < 0 || idx > text.Length - 2)
        {
            return false;
        }

        return IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
    }

    public static bool IsSurrogatePair(char[] text, int idx)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (idx < 0 || idx > text.Length - 2)
        {
            return false;
        }

        return IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
    }

    /// <summary>
    ///     Measurement conversion from millimeters to inches.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="value">a value in millimeters</param>
    /// <returns>a value in inches</returns>
    public static float MillimetersToInches(float value) => value / 25.4f;

    /// <summary>
    ///     Measurement conversion from millimeters to points.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="value">a value in millimeters</param>
    /// <returns>a value in points</returns>
    public static float MillimetersToPoints(float value) => InchesToPoints(MillimetersToInches(value));

    /// <summary>
    ///     Measurement conversion from points to inches.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="value">a value in points</param>
    /// <returns>a value in inches</returns>
    public static float PointsToInches(float value) => value / 72f;

    /// <summary>
    ///     Measurement conversion from points to millimeters.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="value">a value in points</param>
    /// <returns>a value in millimeters</returns>
    public static float PointsToMillimeters(float value) => InchesToMillimeters(PointsToInches(value));

    /// <summary>
    ///     This method is an alternative for the Stream.Skip()-method
    ///     that doesn't seem to work properly for big values of size.
    /// </summary>
    /// <param name="istr">the stream</param>
    /// <param name="size">the number of bytes to skip</param>
    public static void Skip(Stream istr, int size)
    {
        if (istr == null)
        {
            throw new ArgumentNullException(nameof(istr));
        }

        while (size > 0)
        {
            var r = istr.Read(_skipBuffer, 0, Math.Min(_skipBuffer.Length, size));
            if (r <= 0)
            {
                return;
            }

            size -= r;
        }
    }

    /// <summary>
    ///     This method makes a valid URL from a given filename.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <param name="filename">a given filename</param>
    /// <returns>a valid URL</returns>
    public static Uri ToUrl(string filename)
    {
        try
        {
            return new Uri(filename);
        }
        catch
        {
            return new Uri($"file://{filename}");
        }
    }

    /// <summary>
    ///     Unescapes an URL. All the "%xx" are replaced by the 'xx' hex char value.
    /// </summary>
    /// <param name="src">the url to unescape</param>
    /// <returns>the eunescaped value</returns>
    public static string UnEscapeUrl(string src)
    {
        if (src == null)
        {
            throw new ArgumentNullException(nameof(src));
        }

        var bf = new StringBuilder();
        var s = src.ToCharArray();
        for (var k = 0; k < s.Length; ++k)
        {
            var c = s[k];
            if (c == '%')
            {
                if (k + 2 >= s.Length)
                {
                    bf.Append(c);
                    continue;
                }

                var a0 = PrTokeniser.GetHex(s[k + 1]);
                var a1 = PrTokeniser.GetHex(s[k + 2]);
                if (a0 < 0 || a1 < 0)
                {
                    bf.Append(c);
                    continue;
                }

                bf.Append((char)(a0 * 16 + a1));
                k += 2;
            }
            else
            {
                bf.Append(c);
            }
        }

        return bf.ToString();
    }
}