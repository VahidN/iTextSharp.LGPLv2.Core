using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     This class expands a string into a list of numbers. The main use is to select a
///     range of pages.
///     The general systax is:
///     [!][o][odd][e][even]start-end
///     You can have multiple ranges separated by commas ','. The '!' modifier removes the
///     range from what is already selected. The range changes are incremental, that is,
///     numbers are added or deleted as the range appears. The start or the end, but not both, can be ommited.
/// </summary>
public class SequenceList
{
    protected const int COMMA = 1;
    protected const int END = 6;
    protected const char EOT = '\uffff';
    protected const int MINUS = 2;
    protected const int NOT = 3;
    protected const int NUMBER = 5;
    protected const int TEXT = 4;
    private const int Digit = 1;
    private const int Digit2 = 3;
    private const int First = 0;
    private const string NotOther = "-,!0123456789";
    private const int Other = 2;
    protected bool Even;
    protected int High;
    protected bool Inverse;
    protected int Low;
    protected int Number;
    protected bool Odd;
    protected string other;
    protected int Ptr;
    protected char[] Text;

    protected SequenceList(string range)
    {
        if (range == null)
        {
            throw new ArgumentNullException(nameof(range));
        }

        Ptr = 0;
        Text = range.ToCharArray();
    }

    protected int Type
    {
        get
        {
            var buf = new StringBuilder();
            var state = First;
            while (true)
            {
                var c = NextChar();
                if (c == EOT)
                {
                    if (state == Digit)
                    {
                        Number = int.Parse(other = buf.ToString(), CultureInfo.InvariantCulture);
                        return NUMBER;
                    }

                    if (state == Other)
                    {
                        other = buf.ToString().ToLower(CultureInfo.InvariantCulture);
                        return TEXT;
                    }

                    return END;
                }

                switch (state)
                {
                    case First:
                        switch (c)
                        {
                            case '!':
                                return NOT;
                            case '-':
                                return MINUS;
                            case ',':
                                return COMMA;
                        }

                        buf.Append(c);
                        if (c >= '0' && c <= '9')
                        {
                            state = Digit;
                        }
                        else
                        {
                            state = Other;
                        }

                        break;
                    case Digit:
                        if (c >= '0' && c <= '9')
                        {
                            buf.Append(c);
                        }
                        else
                        {
                            PutBack();
                            Number = int.Parse(other = buf.ToString(), CultureInfo.InvariantCulture);
                            return NUMBER;
                        }

                        break;
                    case Other:
                        if (NotOther.IndexOf(c.ToString(), StringComparison.Ordinal) < 0)
                        {
                            buf.Append(c);
                        }
                        else
                        {
                            PutBack();
                            other = buf.ToString().ToLower(CultureInfo.InvariantCulture);
                            return TEXT;
                        }

                        break;
                }
            }
        }
    }

    /// <summary>
    ///     Generates a list of numbers from a string.
    /// </summary>
    /// <param name="ranges">the comma separated ranges</param>
    /// <param name="maxNumber">the maximum number in the range</param>
    /// <returns>a list with the numbers as  Integer </returns>
    public static ICollection<int> Expand(string ranges, int maxNumber)
    {
        var parse = new SequenceList(ranges);
        List<int> list = new();
        var sair = false;
        while (!sair)
        {
            sair = parse.GetAttributes();
            if (parse.Low == -1 && parse.High == -1 && !parse.Even && !parse.Odd)
            {
                continue;
            }

            if (parse.Low < 1)
            {
                parse.Low = 1;
            }

            if (parse.High < 1 || parse.High > maxNumber)
            {
                parse.High = maxNumber;
            }

            if (parse.Low > maxNumber)
            {
                parse.Low = maxNumber;
            }

            //System.out.Println("low="+parse.low+",high="+parse.high+",odd="+parse.odd+",even="+parse.even+",inverse="+parse.inverse);
            var inc = 1;
            if (parse.Inverse)
            {
                if (parse.Low > parse.High)
                {
                    var t = parse.Low;
                    parse.Low = parse.High;
                    parse.High = t;
                }

                for (var it = new ListIterator<int>(list); it.HasNext();)
                {
                    var n = it.Next();
                    if (parse.Even && (n & 1) == 1)
                    {
                        continue;
                    }

                    if (parse.Odd && (n & 1) == 0)
                    {
                        continue;
                    }

                    if (n >= parse.Low && n <= parse.High)
                    {
                        it.Remove();
                    }
                }
            }
            else
            {
                if (parse.Low > parse.High)
                {
                    inc = -1;
                    if (parse.Odd || parse.Even)
                    {
                        --inc;
                        if (parse.Even)
                        {
                            parse.Low &= ~1;
                        }
                        else
                        {
                            parse.Low -= (parse.Low & 1) == 1 ? 0 : 1;
                        }
                    }

                    for (var k = parse.Low; k >= parse.High; k += inc)
                    {
                        list.Add(k);
                    }
                }
                else
                {
                    if (parse.Odd || parse.Even)
                    {
                        ++inc;
                        if (parse.Odd)
                        {
                            parse.Low |= 1;
                        }
                        else
                        {
                            parse.Low += (parse.Low & 1) == 1 ? 1 : 0;
                        }
                    }

                    for (var k = parse.Low; k <= parse.High; k += inc)
                    {
                        list.Add(k);
                    }
                }
            }
        }

        return list;
    }

    protected bool GetAttributes()
    {
        Low = -1;
        High = -1;
        Odd = Even = Inverse = false;
        var state = Other;
        while (true)
        {
            var type = Type;
            if (type == END || type == COMMA)
            {
                if (state == Digit)
                {
                    High = Low;
                }

                return type == END;
            }

            switch (state)
            {
                case Other:
                    switch (type)
                    {
                        case NOT:
                            Inverse = true;
                            break;
                        case MINUS:
                            state = Digit2;
                            break;
                        default:
                            if (type == NUMBER)
                            {
                                Low = Number;
                                state = Digit;
                            }
                            else
                            {
                                otherProc();
                            }

                            break;
                    }

                    break;
                case Digit:
                    switch (type)
                    {
                        case NOT:
                            Inverse = true;
                            state = Other;
                            High = Low;
                            break;
                        case MINUS:
                            state = Digit2;
                            break;
                        default:
                            High = Low;
                            state = Other;
                            otherProc();
                            break;
                    }

                    break;
                case Digit2:
                    switch (type)
                    {
                        case NOT:
                            Inverse = true;
                            state = Other;
                            break;
                        case MINUS:
                            break;
                        case NUMBER:
                            High = Number;
                            state = Other;
                            break;
                        default:
                            state = Other;
                            otherProc();
                            break;
                    }

                    break;
            }
        }
    }

    protected char NextChar()
    {
        while (true)
        {
            if (Ptr >= Text.Length)
            {
                return EOT;
            }

            var c = Text[Ptr++];
            if (c > ' ')
            {
                return c;
            }
        }
    }

    protected void PutBack()
    {
        --Ptr;
        if (Ptr < 0)
        {
            Ptr = 0;
        }
    }

    private void otherProc()
    {
        if (other.Equals("odd", StringComparison.Ordinal) || other.Equals("o", StringComparison.Ordinal))
        {
            Odd = true;
            Even = false;
        }
        else if (other.Equals("even", StringComparison.Ordinal) || other.Equals("e", StringComparison.Ordinal))
        {
            Odd = false;
            Even = true;
        }
    }
}