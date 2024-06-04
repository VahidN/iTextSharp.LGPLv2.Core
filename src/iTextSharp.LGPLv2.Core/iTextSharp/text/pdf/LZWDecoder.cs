namespace iTextSharp.text.pdf;

/// <summary>
///     A class for performing LZW decoding.
/// </summary>
public class LzwDecoder
{
    private int _bytePointer;
    private byte[] _data;
    private int _nextBits;
    private int _nextData;
    private byte[][] _stringTable;
    private int _tableIndex, _bitsToGet = 9;
    private Stream _uncompData;

    internal readonly int[] AndTable =
    {
        511,
        1023,
        2047,
        4095,
    };

    /// <summary>
    ///     Returns the next 9, 10, 11 or 12 bits
    /// </summary>
    public int NextCode
    {
        get
        {
            // Attempt to get the next code. The exception is caught to make
            // this robust to cases wherein the EndOfInformation code has been
            // omitted from a strip. Examples of such cases have been observed
            // in practice.
            try
            {
                _nextData = (_nextData << 8) | (_data[_bytePointer++] & 0xff);
                _nextBits += 8;

                if (_nextBits < _bitsToGet)
                {
                    _nextData = (_nextData << 8) | (_data[_bytePointer++] & 0xff);
                    _nextBits += 8;
                }

                var code =
                    (_nextData >> (_nextBits - _bitsToGet)) & AndTable[_bitsToGet - 9];
                _nextBits -= _bitsToGet;

                return code;
            }
            catch
            {
                // Strip not terminated as expected: return EndOfInformation code.
                return 257;
            }
        }
    }

    /// <summary>
    ///     Add a new string to the string table.
    /// </summary>
    public void AddStringToTable(byte[] oldstring, byte newstring)
    {
        if (oldstring == null)
        {
            throw new ArgumentNullException(nameof(oldstring));
        }

        var length = oldstring.Length;
        var str = new byte[length + 1];
        Array.Copy(oldstring, 0, str, 0, length);
        str[length] = newstring;

        // Add this new string to the table
        _stringTable[_tableIndex++] = str;

        if (_tableIndex == 511)
        {
            _bitsToGet = 10;
        }
        else if (_tableIndex == 1023)
        {
            _bitsToGet = 11;
        }
        else if (_tableIndex == 2047)
        {
            _bitsToGet = 12;
        }
    }

    /// <summary>
    ///     Add a new string to the string table.
    /// </summary>
    public void AddStringToTable(byte[] str)
    {
        // Add this new string to the table
        _stringTable[_tableIndex++] = str;

        if (_tableIndex == 511)
        {
            _bitsToGet = 10;
        }
        else if (_tableIndex == 1023)
        {
            _bitsToGet = 11;
        }
        else if (_tableIndex == 2047)
        {
            _bitsToGet = 12;
        }
    }

    /// <summary>
    ///     Append  newstring  to the end of  oldstring .
    /// </summary>
    public static byte[] ComposeString(byte[] oldstring, byte newstring)
    {
        if (oldstring == null)
        {
            throw new ArgumentNullException(nameof(oldstring));
        }

        var length = oldstring.Length;
        var str = new byte[length + 1];
        Array.Copy(oldstring, 0, str, 0, length);
        str[length] = newstring;

        return str;
    }

    /// <summary>
    ///     Method to decode LZW compressed data.
    /// </summary>
    /// <param name="data">The compressed data.</param>
    /// <param name="uncompData">Array to return the uncompressed data in.</param>
    public void Decode(byte[] data, Stream uncompData)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (data[0] == 0x00 && data[1] == 0x01)
        {
            throw new NotSupportedException("LZW flavour not supported.");
        }

        InitializeStringTable();

        _data = data;
        _uncompData = uncompData;

        // Initialize pointers
        _bytePointer = 0;

        _nextData = 0;
        _nextBits = 0;

        int code, oldCode = 0;
        byte[] str;

        while ((code = NextCode) != 257)
        {
            if (code == 256)
            {
                InitializeStringTable();
                code = NextCode;

                if (code == 257)
                {
                    break;
                }

                WriteString(_stringTable[code]);
                oldCode = code;
            }
            else
            {
                if (code < _tableIndex)
                {
                    str = _stringTable[code];

                    WriteString(str);
                    AddStringToTable(_stringTable[oldCode], str[0]);
                    oldCode = code;
                }
                else
                {
                    str = _stringTable[oldCode];
                    str = ComposeString(str, str[0]);
                    WriteString(str);
                    AddStringToTable(str);
                    oldCode = code;
                }
            }
        }
    }


    /// <summary>
    ///     Initialize the string table.
    /// </summary>
    public void InitializeStringTable()
    {
        _stringTable = new byte[8192][];

        for (var i = 0; i < 256; i++)
        {
            _stringTable[i] = new byte[1];
            _stringTable[i][0] = (byte)i;
        }

        _tableIndex = 258;
        _bitsToGet = 9;
    }

    /// <summary>
    ///     Write out the string just uncompressed.
    /// </summary>
    public void WriteString(byte[] str)
    {
        if (str == null)
        {
            throw new InvalidOperationException("Tried to write from null location in LZWdecoder, method WriteString.");
        }

        _uncompData.Write(str, 0, str.Length);
    }
}