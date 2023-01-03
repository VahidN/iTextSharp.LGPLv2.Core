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
}