using iTextSharp.text;

namespace System.util;

/// <summary>
///     Summary description for Util.
/// </summary>
public static class Util
{
    public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (list is List<T> asList)
        {
            asList.AddRange(items);
        }
        else
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }
    }

    public static int Usr(int op1, int op2)
    {
        if (op2 < 1)
        {
            return op1;
        }

        return unchecked((int)((uint)op1 >> op2));
    }

    public static bool EqualsIgnoreCase(string s1, string s2) =>
        CultureInfo.InvariantCulture.CompareInfo.Compare(s1, s2, CompareOptions.IgnoreCase) == 0;

    public static int CompareToIgnoreCase(string s1, string s2) =>
        CultureInfo.InvariantCulture.CompareInfo.Compare(s1, s2, CompareOptions.IgnoreCase);

    /// <summary>
    ///     Converts a string into a Byte array
    ///     according to the ISO-8859-1 codepage.
    /// </summary>
    /// <param name="text">the text to be converted</param>
    /// <returns>the conversion result</returns>
    public static byte[] GetIsoBytes(this string text) => DocWriter.GetIsoBytes(text);

    public static byte[] CopyOfRange(this byte[] src, int start, int end)
    {
        var len = end - start;
        var dest = new byte[len];
        Array.Copy(src, start, dest, 0, len);
        return dest;
    }

    public static byte[] CopyOf(this byte[] src, int newLength)
    {
        if (src == null)
        {
            throw new ArgumentNullException(nameof(src));
        }

        var dest = new byte[newLength];
        var len = newLength > src.Length ? src.Length : newLength;
        Array.Copy(src, 0, dest, 0, len);
        return dest;
    }
}