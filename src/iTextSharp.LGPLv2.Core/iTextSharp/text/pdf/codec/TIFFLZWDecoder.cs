namespace iTextSharp.text.pdf.codec;

/// <summary>
///     A class for performing LZW decoding.
/// </summary>
public class TifflzwDecoder
{
    private readonly int[] _andTable =
    {
        511,
        1023,
        2047,
        4095,
    };

    private readonly int _predictor;
    private readonly int _samplesPerPixel;
    private readonly int _w;
    private int _bytePointer;
    private byte[] _data;
    private int _dstIndex;
    private int _h;
    private int _nextBits;
    private int _nextData;

    private byte[][] _stringTable;
    private int _tableIndex, _bitsToGet = 9;
    private byte[] _uncompData;

    public TifflzwDecoder(int w, int predictor, int samplesPerPixel)
    {
        _w = w;
        _predictor = predictor;
        _samplesPerPixel = samplesPerPixel;
    }

    /// <summary>
    ///     Method to decode LZW compressed data.
    /// </summary>
    /// <param name="data">The compressed data.</param>
    /// <param name="uncompData">Array to return the uncompressed data in.</param>
    /// <param name="h">The number of rows the compressed data contains.</param>
    public byte[] Decode(byte[] data, byte[] uncompData, int h)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (uncompData == null)
        {
            throw new ArgumentNullException(nameof(uncompData));
        }

        if (data[0] == 0x00 && data[1] == 0x01)
        {
            throw new InvalidOperationException("TIFF 5.0-style LZW codes are not supported.");
        }

        InitializeStringTable();

        _data = data;
        _h = h;
        _uncompData = uncompData;

        // Initialize pointers
        _bytePointer = 0;
        _dstIndex = 0;


        _nextData = 0;
        _nextBits = 0;

        int code, oldCode = 0;
        byte[] strn;

        while ((code = GetNextCode()) != 257 &&
               _dstIndex < uncompData.Length)
        {
            if (code == 256)
            {
                InitializeStringTable();
                code = GetNextCode();

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
                    strn = _stringTable[code];

                    WriteString(strn);
                    AddStringToTable(_stringTable[oldCode], strn[0]);
                    oldCode = code;
                }
                else
                {
                    strn = _stringTable[oldCode];
                    strn = ComposeString(strn, strn[0]);
                    WriteString(strn);
                    AddStringToTable(strn);
                    oldCode = code;
                }
            }
        }

        // Horizontal Differencing Predictor
        if (_predictor == 2)
        {
            int count;
            for (var j = 0; j < h; j++)
            {
                count = _samplesPerPixel * (j * _w + 1);

                for (var i = _samplesPerPixel; i < _w * _samplesPerPixel; i++)
                {
                    uncompData[count] += uncompData[count - _samplesPerPixel];
                    count++;
                }
            }
        }

        return uncompData;
    }


    /// <summary>
    ///     Initialize the string table.
    /// </summary>
    public void InitializeStringTable()
    {
        _stringTable = new byte[4096][];

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
    public void WriteString(byte[] strn)
    {
        if (strn == null)
        {
            throw new ArgumentNullException(nameof(strn));
        }

        // Fix for broken tiff files
        var max = _uncompData.Length - _dstIndex;
        if (strn.Length < max)
        {
            max = strn.Length;
        }

        Array.Copy(strn, 0, _uncompData, _dstIndex, max);
        _dstIndex += max;
    }

    /// <summary>
    ///     Add a new string to the string table.
    /// </summary>
    public void AddStringToTable(byte[] oldString, byte newString)
    {
        if (oldString == null)
        {
            throw new ArgumentNullException(nameof(oldString));
        }

        var length = oldString.Length;
        var strn = new byte[length + 1];
        Array.Copy(oldString, 0, strn, 0, length);
        strn[length] = newString;

        // Add this new String to the table
        _stringTable[_tableIndex++] = strn;

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
    public void AddStringToTable(byte[] strn)
    {
        // Add this new String to the table
        _stringTable[_tableIndex++] = strn;

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
    ///     Append  newString  to the end of  oldString .
    /// </summary>
    public static byte[] ComposeString(byte[] oldString, byte newString)
    {
        if (oldString == null)
        {
            throw new ArgumentNullException(nameof(oldString));
        }

        var length = oldString.Length;
        var strn = new byte[length + 1];
        Array.Copy(oldString, 0, strn, 0, length);
        strn[length] = newString;

        return strn;
    }

    /// <summary>
    ///     Returns the next 9, 10, 11 or 12 bits
    /// </summary>
    public int GetNextCode()
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
                (_nextData >> (_nextBits - _bitsToGet)) & _andTable[_bitsToGet - 9];
            _nextBits -= _bitsToGet;

            return code;
        }
        catch (IndexOutOfRangeException)
        {
            // Strip not terminated as expected: return EndOfInformation code.
            return 257;
        }
    }
}