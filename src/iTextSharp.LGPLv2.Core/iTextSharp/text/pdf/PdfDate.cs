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

        Value = d.ToString("\\D\\:yyyyMMddHHmmss", DateTimeFormatInfo.InvariantInfo);
        // bug fix for .NET Framework - see https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings#zzzSpecifier
        var timezone = d.Kind == DateTimeKind.Utc ? "+00:00" : d.ToString("zzz", DateTimeFormatInfo.InvariantInfo);
        timezone = timezone.Replace(":", "'");
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

        if (date.StartsWith("D:", StringComparison.Ordinal))
        {
            date = date.Substring(2);
        }

        int year, month = 1, day = 1, hour = 0, minute = 0, second = 0;
        int offsetHour = 0, offsetMinute = 0;
        var variation = '\0';
        year = int.Parse(date.Substring(0, 4), CultureInfo.InvariantCulture);
        if (date.Length >= 6)
        {
            month = int.Parse(date.Substring(4, 2), CultureInfo.InvariantCulture);
            if (date.Length >= 8)
            {
                day = int.Parse(date.Substring(6, 2), CultureInfo.InvariantCulture);
                if (date.Length >= 10)
                {
                    hour = int.Parse(date.Substring(8, 2), CultureInfo.InvariantCulture);
                    if (date.Length >= 12)
                    {
                        minute = int.Parse(date.Substring(10, 2), CultureInfo.InvariantCulture);
                        if (date.Length >= 14)
                        {
                            second = int.Parse(date.Substring(12, 2), CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
        }

        var d = new DateTime(year, month, day, hour, minute, second);
        if (date.Length <= 14)
        {
            return d;
        }

        variation = date[14];
        if (variation == 'Z')
        {
            return d.ToLocalTime();
        }

        if (date.Length >= 17)
        {
            offsetHour = int.Parse(date.Substring(15, 2), CultureInfo.InvariantCulture);
            if (date.Length >= 20)
            {
                offsetMinute = int.Parse(date.Substring(18, 2), CultureInfo.InvariantCulture);
            }
        }

        var span = new TimeSpan(offsetHour, offsetMinute, 0);
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

        if (d.StartsWith("D:", StringComparison.Ordinal))
        {
            d = d.Substring(2);
        }

        var sb = new StringBuilder();
        if (d.Length < 4)
        {
            return "0000";
        }

        sb.Append(d.Substring(0, 4)); //year
        d = d.Substring(4);
        if (d.Length < 2)
        {
            return sb.ToString();
        }

        sb.Append('-').Append(d.Substring(0, 2)); //month
        d = d.Substring(2);
        if (d.Length < 2)
        {
            return sb.ToString();
        }

        sb.Append('-').Append(d.Substring(0, 2)); //day
        d = d.Substring(2);
        if (d.Length < 2)
        {
            return sb.ToString();
        }

        sb.Append('T').Append(d.Substring(0, 2)); //hour
        d = d.Substring(2);
        if (d.Length < 2)
        {
            sb.Append(":00Z");
            return sb.ToString();
        }

        sb.Append(':').Append(d.Substring(0, 2)); //minute
        d = d.Substring(2);
        if (d.Length < 2)
        {
            sb.Append('Z');
            return sb.ToString();
        }

        sb.Append(':').Append(d.Substring(0, 2)); //second
        d = d.Substring(2);
        if (d.StartsWith("-", StringComparison.Ordinal) ||
            d.StartsWith("+", StringComparison.Ordinal))
        {
            var sign = d.Substring(0, 1);
            d = d.Substring(1);
            var h = "00";
            var m = "00";
            if (d.Length >= 2)
            {
                h = d.Substring(0, 2);
                if (d.Length > 2)
                {
                    d = d.Substring(3);
                    if (d.Length >= 2)
                    {
                        m = d.Substring(0, 2);
                    }
                }

                sb.Append(sign).Append(h).Append(':').Append(m);
                return sb.ToString();
            }
        }

        sb.Append('Z');
        return sb.ToString();
    }

    /// <summary>
    ///     Gives the W3C format of the PdfDate.
    /// </summary>
    /// <returns>a formatted date</returns>
    public string GetW3CDate() => GetW3CDate(Value);

    private static string setLength(int i, int length) => i.ToString(CultureInfo.InvariantCulture)
                                                           .PadLeft(length, '0');
}