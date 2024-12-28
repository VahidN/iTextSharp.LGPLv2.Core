using System.Text;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfDate  is the PDF date object.
///     PDF defines a standard date format. The PDF date format closely follows the format
///     defined by the international standard ASN.1 (Abstract Syntax Notation One, defined
///     in CCITT X.208 or ISO/IEC 8824). A date is a  PdfString  of the form:
///     (D:YYYYMMDDHHmmSSOHH'mm')
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 7.2 (page 183-184)
///     @see     PdfString
///     @see     java.util.GregorianCalendar
/// </summary>
public class PdfDate : PdfString
{
    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfDate -object.
    /// </summary>
    /// <param name="d">the date that has to be turned into a  PdfDate -object</param>
    public PdfDate(DateTime d)
    {
        //d = d.ToUniversalTime();

        Value = d.ToString(format: "\\D\\:yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);

        // bug fix for .NET Framework - see https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings#zzzSpecifier
        var timezone = d.Kind == DateTimeKind.Utc
            ? "+00:00"
            : d.ToString(format: "zzz", DateTimeFormatInfo.InvariantInfo);

        timezone = timezone.Replace(oldValue: ":", newValue: "'", StringComparison.Ordinal);
        Value += timezone + "'";
    }

    /// <summary>
    ///     Constructs a  PdfDate -object, representing the current day and time.
    /// </summary>
    public PdfDate() : this(DateTime.Now)
    {
    }

    /// <summary>
    ///     Adds a number of leading zeros to a given  string  in order to get a  string
    ///     of a certain length.
    /// </summary>
    /// <returns>the resulting  string </returns>
    public static DateTime Decode(string date)
    {
        if (date == null)
        {
            throw new ArgumentNullException(nameof(date));
        }

        if (date.StartsWith(value: "D:", StringComparison.Ordinal))
        {
            date = date.Substring(startIndex: 2);
        }

        int year, month = 1, day = 1, hour = 0, minute = 0, second = 0;
        int offsetHour = 0, offsetMinute = 0;
        var variation = '\0';
        year = int.Parse(date.Substring(startIndex: 0, length: 4), CultureInfo.InvariantCulture);

        if (date.Length >= 6)
        {
            month = int.Parse(date.Substring(startIndex: 4, length: 2), CultureInfo.InvariantCulture);

            if (date.Length >= 8)
            {
                day = int.Parse(date.Substring(startIndex: 6, length: 2), CultureInfo.InvariantCulture);

                if (date.Length >= 10)
                {
                    hour = int.Parse(date.Substring(startIndex: 8, length: 2), CultureInfo.InvariantCulture);

                    if (date.Length >= 12)
                    {
                        minute = int.Parse(date.Substring(startIndex: 10, length: 2), CultureInfo.InvariantCulture);

                        if (date.Length >= 14)
                        {
                            second = int.Parse(date.Substring(startIndex: 12, length: 2), CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
        }

        var d = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);

        if (date.Length <= 14)
        {
            return d;
        }

        variation = date[index: 14];

        if (variation == 'Z')
        {
            return d.ToLocalTime();
        }

        if (date.Length >= 17)
        {
            offsetHour = int.Parse(date.Substring(startIndex: 15, length: 2), CultureInfo.InvariantCulture);

            if (date.Length >= 20)
            {
                offsetMinute = int.Parse(date.Substring(startIndex: 18, length: 2), CultureInfo.InvariantCulture);
            }
        }

        var span = new TimeSpan(offsetHour, offsetMinute, seconds: 0);

        if (variation == '-')
        {
            d += span;
        }
        else
        {
            d -= span;
        }

        return d.ToLocalTime();
    }

    /// <summary>
    ///     Gives the W3C format of the PdfDate.
    /// </summary>
    /// <param name="d">the date in the format D:YYYYMMDDHHmmSSOHH'mm'</param>
    /// <returns>a formatted date</returns>
    public static string GetW3CDate(string d)
    {
        if (d == null)
        {
            throw new ArgumentNullException(nameof(d));
        }

        if (d.StartsWith(value: "D:", StringComparison.Ordinal))
        {
            d = d.Substring(startIndex: 2);
        }

        var sb = new StringBuilder();

        if (d.Length < 4)
        {
            return "0000";
        }

        sb.Append(d.Substring(startIndex: 0, length: 4)); //year
        d = d.Substring(startIndex: 4);

        if (d.Length < 2)
        {
            return sb.ToString();
        }

        sb.Append(value: '-').Append(d.Substring(startIndex: 0, length: 2)); //month
        d = d.Substring(startIndex: 2);

        if (d.Length < 2)
        {
            return sb.ToString();
        }

        sb.Append(value: '-').Append(d.Substring(startIndex: 0, length: 2)); //day
        d = d.Substring(startIndex: 2);

        if (d.Length < 2)
        {
            return sb.ToString();
        }

        sb.Append(value: 'T').Append(d.Substring(startIndex: 0, length: 2)); //hour
        d = d.Substring(startIndex: 2);

        if (d.Length < 2)
        {
            sb.Append(value: ":00Z");

            return sb.ToString();
        }

        sb.Append(value: ':').Append(d.Substring(startIndex: 0, length: 2)); //minute
        d = d.Substring(startIndex: 2);

        if (d.Length < 2)
        {
            sb.Append(value: 'Z');

            return sb.ToString();
        }

        sb.Append(value: ':').Append(d.Substring(startIndex: 0, length: 2)); //second
        d = d.Substring(startIndex: 2);

        if (d.StartsWith(value: '-') || d.StartsWith(value: '+'))
        {
            var sign = d.Substring(startIndex: 0, length: 1);
            d = d.Substring(startIndex: 1);
            var h = "00";
            var m = "00";

            if (d.Length >= 2)
            {
                h = d.Substring(startIndex: 0, length: 2);

                if (d.Length > 2)
                {
                    d = d.Substring(startIndex: 3);

                    if (d.Length >= 2)
                    {
                        m = d.Substring(startIndex: 0, length: 2);
                    }
                }

                sb.Append(sign).Append(h).Append(value: ':').Append(m);

                return sb.ToString();
            }
        }

        sb.Append(value: 'Z');

        return sb.ToString();
    }

    /// <summary>
    ///     Gives the W3C format of the PdfDate.
    /// </summary>
    /// <returns>a formatted date</returns>
    public string GetW3CDate() => GetW3CDate(Value);

    private static string setLength(int i, int length)
        => i.ToString(CultureInfo.InvariantCulture).PadLeft(length, paddingChar: '0');
}