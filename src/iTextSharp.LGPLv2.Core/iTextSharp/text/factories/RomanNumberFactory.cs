using System.Text;

namespace iTextSharp.text.factories;

/// <summary>
///     This class can produce String combinations representing a roman number.
/// </summary>
public static class RomanNumberFactory
{
    /// <summary>
    ///     Array with Roman digits.
    /// </summary>
    private static readonly RomanDigit[] _roman =
    {
        new('m', 1000, false),
        new('d', 500, false),
        new('c', 100, true),
        new('l', 50, false),
        new('x', 10, true),
        new('v', 5, false),
        new('i', 1, true),
    };

    /// <summary>
    ///     Changes an int into a lower case roman number.
    /// </summary>
    /// <param name="index">the original number</param>
    /// <returns>the roman number (lower case)</returns>
    public static string GetLowerCaseString(int index) => GetString(index);

    /// <summary>
    ///     Changes an int into a lower case roman number.
    /// </summary>
    /// <param name="index">the original number</param>
    /// <returns>the roman number (lower case)</returns>
    public static string GetString(int index)
    {
        var buf = new StringBuilder();

        // lower than 0 ? Add minus
        if (index < 0)
        {
            buf.Append('-');
            index = -index;
        }

        // greater than 3000
        if (index > 3000)
        {
            buf.Append('|');
            buf.Append(GetString(index / 1000));
            buf.Append('|');
            // remainder
            index = index - index / 1000 * 1000;
        }

        // number between 1 and 3000
        var pos = 0;
        while (true)
        {
            // loop over the array with values for m-d-c-l-x-v-i
            var dig = _roman[pos];
            // adding as many digits as we can
            while (index >= dig.Value)
            {
                buf.Append(dig.Digit);
                index -= dig.Value;
            }

            // we have the complete number
            if (index <= 0)
            {
                break;
            }

            // look for the next digit that can be used in a special way
            var j = pos;
            while (!_roman[++j].Pre)
            {
                ;
            }

            // does the special notation apply?
            if (index + _roman[j].Value >= dig.Value)
            {
                buf.Append(_roman[j].Digit).Append(dig.Digit);
                index -= dig.Value - _roman[j].Value;
            }

            pos++;
        }

        return buf.ToString();
    }

    /// <summary>
    ///     Changes an int into a roman number.
    /// </summary>
    /// <param name="index">the original number</param>
    /// <param name="lowercase"></param>
    /// <returns>the roman number (lower case)</returns>
    public static string GetString(int index, bool lowercase)
    {
        if (lowercase)
        {
            return GetLowerCaseString(index);
        }

        return GetUpperCaseString(index);
    }

    /// <summary>
    ///     Changes an int into an upper case roman number.
    /// </summary>
    /// <param name="index">the original number</param>
    /// <returns>the roman number (lower case)</returns>
    public static string GetUpperCaseString(int index) => GetString(index).ToUpper(CultureInfo.InvariantCulture);

    /// <summary>
    ///     Helper class for Roman Digits
    /// </summary>
    internal class RomanDigit
    {
        /// <summary>
        ///     part of a roman number
        /// </summary>
        public readonly char Digit;

        /// <summary>
        ///     can the digit be used as a prefix
        /// </summary>
        public readonly bool Pre;

        /// <summary>
        ///     value of the roman digit
        /// </summary>
        public readonly int Value;

        /// <summary>
        ///     Constructs a roman digit
        /// </summary>
        /// <param name="digit">the roman digit</param>
        /// <param name="value">the value</param>
        /// <param name="pre">can it be used as a prefix</param>
        internal RomanDigit(char digit, int value, bool pre)
        {
            Digit = digit;
            Value = value;
            Pre = pre;
        }
    }
}