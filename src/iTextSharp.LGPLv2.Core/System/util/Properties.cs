using System.Text;
using iTextSharp.LGPLv2.Core.System.Encodings;

namespace System.util;

/// <summary>
///     Summary description for Properties.
/// </summary>
public class Properties
{
    private const string KeyValueSeparators = "=: \t\r\n\f";
    private const string StrictKeyValueSeparators = "=:";
    private const string WhiteSpaceChars = " \t\r\n\f";
    private readonly INullValueDictionary<string, string> _col;

    public Properties() => _col = new NullValueDictionary<string, string>();

    public int Count => _col.Count;

    public ICollection<string> Keys => _col.Keys;

    public virtual string this[string key]
    {
        get => _col[key];

        set => _col[key] = value;
    }

    public virtual void Add(string key, string value)
    {
        _col[key] = value;
    }

    public void AddAll(Properties col)
    {
        if (col == null)
        {
            throw new ArgumentNullException(nameof(col));
        }

        foreach (var itm in col.Keys)
        {
            _col[itm] = col[itm];
        }
    }

    public void Clear()
    {
        _col.Clear();
    }

    public bool ContainsKey(string key) => _col.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _col.GetEnumerator();

    public void Load(Stream inStream)
    {
        using var inp = new StreamReader(inStream, EncodingsRegistry.GetEncoding(1252));
        while (true)
        {
            // Get next line
            var line = inp.ReadLine();
            if (line == null)
            {
                return;
            }

            if (line.Length > 0)
            {
                // Find start of key
                var len = line.Length;
                int keyStart;
                for (keyStart = 0; keyStart < len; keyStart++)
                {
                    if (WhiteSpaceChars.IndexOf(line[keyStart].ToString(), StringComparison.Ordinal) == -1)
                    {
                        break;
                    }
                }

                // Blank lines are ignored
                if (keyStart == len)
                {
                    continue;
                }

                // Continue lines that end in slashes if they are not comments
                var firstChar = line[keyStart];
                if (firstChar != '#' && firstChar != '!')
                {
                    while (continueLine(line))
                    {
                        var nextLine = inp.ReadLine();
                        if (nextLine == null)
                        {
                            nextLine = "";
                        }

                        var loppedLine = line.Substring(0, len - 1);
                        // Advance beyond whitespace on new line
                        int startIndex;
                        for (startIndex = 0; startIndex < nextLine.Length; startIndex++)
                        {
                            if (WhiteSpaceChars.IndexOf(nextLine[startIndex].ToString(), StringComparison.Ordinal) ==
                                -1)
                            {
                                break;
                            }
                        }

                        nextLine = nextLine.Substring(startIndex, nextLine.Length - startIndex);
                        line = loppedLine + nextLine;
                        len = line.Length;
                    }

                    // Find separation between key and value
                    int separatorIndex;
                    for (separatorIndex = keyStart; separatorIndex < len; separatorIndex++)
                    {
                        var currentChar = line[separatorIndex];
                        if (currentChar == '\\')
                        {
                            separatorIndex++;
                        }
                        else if (KeyValueSeparators.IndexOf(currentChar.ToString(), StringComparison.Ordinal) != -1)
                        {
                            break;
                        }
                    }

                    // Skip over whitespace after key if any
                    int valueIndex;
                    for (valueIndex = separatorIndex; valueIndex < len; valueIndex++)
                    {
                        if (WhiteSpaceChars.IndexOf(line[valueIndex].ToString(), StringComparison.Ordinal) == -1)
                        {
                            break;
                        }
                    }

                    // Skip over one non whitespace key value separators if any
                    if (valueIndex < len)
                    {
                        if (StrictKeyValueSeparators.IndexOf(line[valueIndex].ToString(), StringComparison.Ordinal) !=
                            -1)
                        {
                            valueIndex++;
                        }
                    }

                    // Skip over white space after other separators if any
                    while (valueIndex < len)
                    {
                        if (WhiteSpaceChars.IndexOf(line[valueIndex].ToString(), StringComparison.Ordinal) == -1)
                        {
                            break;
                        }

                        valueIndex++;
                    }

                    var key = line.Substring(keyStart, separatorIndex - keyStart);
                    var value = separatorIndex < len ? line.Substring(valueIndex, len - valueIndex) : "";

                    // Convert then store key and value
                    key = loadConvert(key);
                    value = loadConvert(value);
                    Add(key, value);
                }
            }
        }
    }

    public string Remove(string key)
    {
        var retval = _col[key];
        _col.Remove(key);
        return retval;
    }

    private static bool continueLine(string line)
    {
        var slashCount = 0;
        var index = line.Length - 1;
        while (index >= 0 && line[index--] == '\\')
        {
            slashCount++;
        }

        return slashCount % 2 == 1;
    }

    /// <summary>
    ///     Converts encoded &#92;uxxxx to unicode chars
    ///     and changes special saved chars to their original forms
    /// </summary>
    private static string loadConvert(string theString)
    {
        char aChar;
        var len = theString.Length;
        var outBuffer = new StringBuilder(len);

        for (var x = 0; x < len;)
        {
            aChar = theString[x++];
            if (aChar == '\\')
            {
                aChar = theString[x++];
                if (aChar == 'u')
                {
                    // Read the xxxx
                    var value = 0;
                    for (var i = 0; i < 4; i++)
                    {
                        aChar = theString[x++];
                        switch (aChar)
                        {
                            case '0':
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                                value = (value << 4) + aChar - '0';
                                break;
                            case 'a':
                            case 'b':
                            case 'c':
                            case 'd':
                            case 'e':
                            case 'f':
                                value = (value << 4) + 10 + aChar - 'a';
                                break;
                            case 'A':
                            case 'B':
                            case 'C':
                            case 'D':
                            case 'E':
                            case 'F':
                                value = (value << 4) + 10 + aChar - 'A';
                                break;
                            default:
                                throw new ArgumentException(
                                                            "Malformed \\uxxxx encoding.");
                        }
                    }

                    outBuffer.Append((char)value);
                }
                else
                {
                    if (aChar == 't')
                    {
                        aChar = '\t';
                    }
                    else if (aChar == 'r')
                    {
                        aChar = '\r';
                    }
                    else if (aChar == 'n')
                    {
                        aChar = '\n';
                    }
                    else if (aChar == 'f')
                    {
                        aChar = '\f';
                    }

                    outBuffer.Append(aChar);
                }
            }
            else
            {
                outBuffer.Append(aChar);
            }
        }

        return outBuffer.ToString();
    }
}