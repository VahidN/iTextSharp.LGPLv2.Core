namespace System.util;

/// <summary>
///     a replacement for the StringTokenizer java class
/// </summary>
/// <summary>
///     it's more or less the same as the one in the GNU classpath
/// </summary>
public class StringTokenizer
{
    private readonly int _len;
    private readonly bool _retDelims;
    private readonly string _str;
    private string _delim;
    private int _pos;

    public StringTokenizer(string str) : this(str, " \t\n\r\f", false)
    {
    }

    public StringTokenizer(string str, string delim) : this(str, delim, false)
    {
    }

    public StringTokenizer(string str, string delim, bool retDelims)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        _len = str.Length;
        _str = str;
        _delim = delim;
        _retDelims = retDelims;
        _pos = 0;
    }

    public int CountTokens()
    {
        var count = 0;
        var delimiterCount = 0;
        var tokenFound = false;
        var tmpPos = _pos;

        while (tmpPos < _len)
        {
            if (_delim.IndexOf(_str[tmpPos++].ToString(), StringComparison.Ordinal) >= 0)
            {
                if (tokenFound)
                {
                    count++;
                    tokenFound = false;
                }

                delimiterCount++;
            }
            else
            {
                tokenFound = true;
                while (tmpPos < _len
                       && _delim.IndexOf(_str[tmpPos].ToString(), StringComparison.Ordinal) < 0)
                {
                    ++tmpPos;
                }
            }
        }

        if (tokenFound)
        {
            count++;
        }

        return _retDelims ? count + delimiterCount : count;
    }

    public bool HasMoreTokens()
    {
        if (!_retDelims)
        {
            while (_pos < _len && _delim.IndexOf(_str[_pos].ToString(), StringComparison.Ordinal) >= 0)
            {
                _pos++;
            }
        }

        return _pos < _len;
    }

    public string NextToken(string delim)
    {
        _delim = delim;
        return NextToken();
    }

    public string NextToken()
    {
        if (_pos < _len && _delim.IndexOf(_str[_pos].ToString(), StringComparison.Ordinal) >= 0)
        {
            if (_retDelims)
            {
                return _str.Substring(_pos++, 1);
            }

            while (++_pos < _len && _delim.IndexOf(_str[_pos].ToString(), StringComparison.Ordinal) >= 0)
            {
                ;
            }
        }

        if (_pos < _len)
        {
            var start = _pos;
            while (++_pos < _len && _delim.IndexOf(_str[_pos].ToString(), StringComparison.Ordinal) < 0)
            {
                ;
            }

            return _str.Substring(start, _pos - start);
        }

        throw new ArgumentOutOfRangeException();
    }
}