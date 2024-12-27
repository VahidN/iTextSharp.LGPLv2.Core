#if NET40 || NETSTANDARD2_0
namespace System;

/// <summary>
///     Missing NET4_6_2 exts
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    ///     Returns a new string in which all occurrences of a specified string in the current instance are replaced with
    ///     another specified string, using the provided comparison type.
    /// </summary>
    public static bool Contains(this string commandText, string value, StringComparison comparisonType)
        => !string.IsNullOrWhiteSpace(commandText) && commandText.IndexOf(value, comparisonType) >= 0;

    /// <summary>
    ///     Returns a new string in which all occurrences of a specified string in the current instance are replaced with
    ///     another specified string, using the provided comparison type.
    /// </summary>
    public static string Replace(this string str, string oldValue, string newValue, StringComparison comparisonType)
    {
        newValue ??= string.Empty;

        if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(oldValue) || oldValue.Equals(newValue, comparisonType))
        {
            return str;
        }

        int foundAt;

        while ((foundAt = str.IndexOf(oldValue, startIndex: 0, comparisonType)) != -1)
        {
            str = str.Remove(foundAt, oldValue.Length).Insert(foundAt, newValue);
        }

        return str;
    }

    /// <summary>
    ///     Returns the hash code for this string using the specified rules.
    /// </summary>
    public static int GetHashCode(this string str, StringComparison comparisonType)
        => comparisonType switch
        {
            StringComparison.CurrentCulture => StringComparer.CurrentCulture.GetHashCode(str),
            StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase.GetHashCode(str),
            StringComparison.InvariantCulture => StringComparer.InvariantCulture.GetHashCode(str),
            StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase.GetHashCode(str),
            StringComparison.Ordinal => StringComparer.Ordinal.GetHashCode(str),
            StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase.GetHashCode(str),
            _ => throw new NotSupportedException()
        };

    /// <summary>
    ///     Reports the zero-based index of the first occurrence of the specified string in the current String object. A
    ///     parameter specifies the type of search to use for the specified string.
    /// </summary>
    public static int IndexOf(this string text, char value, StringComparison comparisonType)
        => text.IndexOf(value.ToString(CultureInfo.InvariantCulture), comparisonType);

    /// <summary>
    ///     Determines whether the end of this string instance matches the specified string when compared using the specified
    ///     comparison option.
    /// </summary>
    public static bool EndsWith(this string text,
        char value,
        StringComparison comparisonType = StringComparison.Ordinal)
        => text.EndsWith(value.ToString(CultureInfo.InvariantCulture), comparisonType);

    /// <summary>
    ///     Determines whether the beginning of this string instance matches the specified string when compared using the
    ///     specified comparison option.
    /// </summary>
    public static bool StartsWith(this string text,
        char value,
        StringComparison comparisonType = StringComparison.Ordinal)
        => text.StartsWith(value.ToString(CultureInfo.InvariantCulture), comparisonType);
}
#endif