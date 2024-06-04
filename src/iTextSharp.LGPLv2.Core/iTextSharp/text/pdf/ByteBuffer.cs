using System.Text;

namespace iTextSharp.text.pdf;

/// <summary>
///     Acts like a  StringBuilder  but works with  byte  arrays.
///     floating point is converted to a format suitable to the PDF.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class ByteBuffer : Stream
{
    public const byte ZERO = (byte)'0';

    /// <summary>
    ///     If  true  always output floating point numbers with 6 decimal digits.
    ///     If  false  uses the faster, although less precise, representation.
    /// </summary>
    public static bool HighPrecision = false;

    private static readonly byte[] _bytes = { 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 97, 98, 99, 100, 101, 102 };
    private static readonly char[] _chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
    private static byte[][] _byteCache;
    private static int _byteCacheSize;

    /// <summary>
    ///     The buffer where the bytes are stored.
    /// </summary>
    protected byte[] Buf;

    /// <summary>
    ///     The count of bytes in the buffer.
    /// </summary>
    protected int Count;

    static ByteBuffer() => _byteCache = new byte[_byteCacheSize][];

    /// <summary>
    ///     Creates new ByteBuffer with capacity 128
    /// </summary>
    public ByteBuffer() : this(128)
    {
    }

    /// <summary>
    ///     Creates a byte buffer with a certain capacity.
    /// </summary>
    /// <param name="size">the initial capacity</param>
    public ByteBuffer(int size)
    {
        if (size < 1)
        {
            size = 128;
        }

        Buf = new byte[size];
    }

    /// <summary>
    ///     Sets the cache size.
    ///     This can only be used to increment the size.
    ///     If the size that is passed through is smaller than the current size, nothing happens.
    /// </summary>
    public byte[] Buffer => Buf;

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => Count;

    public override long Position
    {
        get => Count;
        set { }
    }

    /// <summary>
    ///     Returns the current size of the buffer.
    /// </summary>
    /// <returns>the value of the  count  field, which is the number of valid bytes in this byte buffer.</returns>
    public int Size
    {
        get => Count;
        set
        {
            if (value > Count || value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value),
                                                      "The new size must be positive and <= of the current size");
            }

            Count = value;
        }
    }

    public static void FillCache(int decimals)
    {
        var step = 1;
        switch (decimals)
        {
            case 0:
                step = 100;
                break;
            case 1:
                step = 10;
                break;
        }

        for (var i = 1; i < _byteCacheSize; i += step)
        {
            if (_byteCache[i] != null)
            {
                continue;
            }

            _byteCache[i] = convertToBytes(i);
        }
    }

    /// <summary>
    ///     Outputs a  double  into a format suitable for the PDF.
    /// </summary>
    /// <param name="d">a double</param>
    /// <returns>the  string  representation of the  double </returns>
    public static string FormatDouble(double d) => FormatDouble(d, null);

    /// <summary>
    ///     Outputs a  double  into a format suitable for the PDF.
    ///     buf  is  null . If  buf  is <B>not</B>  null ,
    ///     then the double is appended directly to the buffer and this methods returns  null .
    /// </summary>
    /// <param name="d">a double</param>
    /// <param name="buf">a ByteBuffer</param>
    /// <returns>the  String  representation of the  double  if</returns>
    public static string FormatDouble(double d, ByteBuffer buf)
    {
        if (HighPrecision)
        {
            var sform = d.ToString("0.######", CultureInfo.InvariantCulture);
            if (buf == null)
            {
                return sform;
            }

            buf.Append(sform);
            return null;
        }

        var negative = false;
        if (Math.Abs(d) < 0.000015)
        {
            if (buf != null)
            {
                buf.Append(ZERO);
                return null;
            }

            return "0";
        }

        if (d < 0)
        {
            negative = true;
            d = -d;
        }

        if (d < 1.0)
        {
            d += 0.000005;
            if (d >= 1)
            {
                if (negative)
                {
                    if (buf != null)
                    {
                        buf.Append((byte)'-');
                        buf.Append((byte)'1');
                        return null;
                    }

                    return "-1";
                }

                if (buf != null)
                {
                    buf.Append((byte)'1');
                    return null;
                }

                return "1";
            }

            if (buf != null)
            {
                var v = (int)(d * 100000);

                if (negative)
                {
                    buf.Append((byte)'-');
                }

                buf.Append((byte)'0');
                buf.Append((byte)'.');

                buf.Append((byte)(v / 10000 + ZERO));
                if (v % 10000 != 0)
                {
                    buf.Append((byte)(v / 1000 % 10 + ZERO));
                    if (v % 1000 != 0)
                    {
                        buf.Append((byte)(v / 100 % 10 + ZERO));
                        if (v % 100 != 0)
                        {
                            buf.Append((byte)(v / 10 % 10 + ZERO));
                            if (v % 10 != 0)
                            {
                                buf.Append((byte)(v % 10 + ZERO));
                            }
                        }
                    }
                }

                return null;
            }
            else
            {
                var x = 100000;
                var v = (int)(d * x);

                var res = new StringBuilder();
                if (negative)
                {
                    res.Append('-');
                }

                res.Append("0.");

                while (v < x / 10)
                {
                    res.Append('0');
                    x /= 10;
                }

                res.Append(v);
                var cut = res.Length - 1;
                while (res[cut] == '0')
                {
                    --cut;
                }

                res.Length = cut + 1;
                return res.ToString();
            }
        }

        if (d <= 32767)
        {
            d += 0.005;
            var v = (int)(d * 100);

            if (v < _byteCacheSize && _byteCache[v] != null)
            {
                if (buf != null)
                {
                    if (negative)
                    {
                        buf.Append((byte)'-');
                    }

                    buf.Append(_byteCache[v]);
                    return null;
                }

                var tmp = PdfEncodings.ConvertToString(_byteCache[v], null);
                if (negative)
                {
                    tmp = "-" + tmp;
                }

                return tmp;
            }

            if (buf != null)
            {
                if (v < _byteCacheSize)
                {
                    //create the cachebyte[]
                    byte[] cache;
                    var size = 0;
                    if (v >= 1000000)
                    {
                        //the original number is >=10000, we need 5 more bytes
                        size += 5;
                    }
                    else if (v >= 100000)
                    {
                        //the original number is >=1000, we need 4 more bytes
                        size += 4;
                    }
                    else if (v >= 10000)
                    {
                        //the original number is >=100, we need 3 more bytes
                        size += 3;
                    }
                    else if (v >= 1000)
                    {
                        //the original number is >=10, we need 2 more bytes
                        size += 2;
                    }
                    else if (v >= 100)
                    {
                        //the original number is >=1, we need 1 more bytes
                        size += 1;
                    }

                    //now we must check if we have a decimal number
                    if (v % 100 != 0)
                    {
                        //yes, do not forget the "."
                        size += 2;
                    }

                    if (v % 10 != 0)
                    {
                        size++;
                    }

                    cache = new byte[size];
                    var add = 0;
                    if (v >= 1000000)
                    {
                        cache[add++] = _bytes[v / 1000000];
                    }

                    if (v >= 100000)
                    {
                        cache[add++] = _bytes[v / 100000 % 10];
                    }

                    if (v >= 10000)
                    {
                        cache[add++] = _bytes[v / 10000 % 10];
                    }

                    if (v >= 1000)
                    {
                        cache[add++] = _bytes[v / 1000 % 10];
                    }

                    if (v >= 100)
                    {
                        cache[add++] = _bytes[v / 100 % 10];
                    }

                    if (v % 100 != 0)
                    {
                        cache[add++] = (byte)'.';
                        cache[add++] = _bytes[v / 10 % 10];
                        if (v % 10 != 0)
                        {
                            cache[add++] = _bytes[v % 10];
                        }
                    }

                    _byteCache[v] = cache;
                }

                if (negative)
                {
                    buf.Append((byte)'-');
                }

                if (v >= 1000000)
                {
                    buf.Append(_bytes[v / 1000000]);
                }

                if (v >= 100000)
                {
                    buf.Append(_bytes[v / 100000 % 10]);
                }

                if (v >= 10000)
                {
                    buf.Append(_bytes[v / 10000 % 10]);
                }

                if (v >= 1000)
                {
                    buf.Append(_bytes[v / 1000 % 10]);
                }

                if (v >= 100)
                {
                    buf.Append(_bytes[v / 100 % 10]);
                }

                if (v % 100 != 0)
                {
                    buf.Append((byte)'.');
                    buf.Append(_bytes[v / 10 % 10]);
                    if (v % 10 != 0)
                    {
                        buf.Append(_bytes[v % 10]);
                    }
                }

                return null;
            }

            var res = new StringBuilder();
            if (negative)
            {
                res.Append('-');
            }

            if (v >= 1000000)
            {
                res.Append(_chars[v / 1000000]);
            }

            if (v >= 100000)
            {
                res.Append(_chars[v / 100000 % 10]);
            }

            if (v >= 10000)
            {
                res.Append(_chars[v / 10000 % 10]);
            }

            if (v >= 1000)
            {
                res.Append(_chars[v / 1000 % 10]);
            }

            if (v >= 100)
            {
                res.Append(_chars[v / 100 % 10]);
            }

            if (v % 100 != 0)
            {
                res.Append('.');
                res.Append(_chars[v / 10 % 10]);
                if (v % 10 != 0)
                {
                    res.Append(_chars[v % 10]);
                }
            }

            return res.ToString();
        }
        else
        {
            var res = new StringBuilder();
            if (negative)
            {
                res.Append('-');
            }

            d += 0.5;
            var v = (long)d;
            return res.Append(v).ToString();
        }
    }

    public static void SetCacheSize(int size)
    {
        if (size > 3276700)
        {
            size = 3276700;
        }

        if (size <= _byteCacheSize)
        {
            return;
        }

        var tmpCache = new byte[size][];
        Array.Copy(_byteCache, 0, tmpCache, 0, _byteCacheSize);
        _byteCache = tmpCache;
        _byteCacheSize = size;
    }


    /// <summary>
    ///     Appends the subarray of the  byte  array. The buffer will grow by
    ///     len  bytes.
    /// </summary>
    /// <param name="b">the array to be appended</param>
    /// <param name="off">the offset to the start of the array</param>
    /// <param name="len">the length of bytes to Append</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append(byte[] b, int off, int len)
    {
        if (b == null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        if (off < 0 || off > b.Length || len < 0 ||
            off + len > b.Length || off + len < 0 || len == 0)
        {
            return this;
        }

        var newcount = Count + len;
        if (newcount > Buf.Length)
        {
            var newbuf = new byte[Math.Max(Buf.Length << 1, newcount)];
            Array.Copy(Buf, 0, newbuf, 0, Count);
            Buf = newbuf;
        }

        Array.Copy(b, off, Buf, Count, len);
        Count = newcount;
        return this;
    }

    /// <summary>
    ///     Appends an array of bytes.
    /// </summary>
    /// <param name="b">the array to be appended</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append(byte[] b)
    {
        if (b == null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        return Append(b, 0, b.Length);
    }

    /// <summary>
    ///     Appends a  string  to the buffer. The  string  is
    ///     converted according to the encoding ISO-8859-1.
    /// </summary>
    /// <param name="str">the  string  to be appended</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append(string str)
    {
        if (str != null)
        {
            return Append(DocWriter.GetIsoBytes(str));
        }

        return this;
    }

    /// <summary>
    ///     Appends a  char  to the buffer. The  char  is
    ///     converted according to the encoding ISO-8859-1.
    /// </summary>
    /// <param name="c">the  char  to be appended</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append(char c) => Append_i(c);

    /// <summary>
    ///     Appends another  ByteBuffer  to this buffer.
    /// </summary>
    /// <param name="buf">the  ByteBuffer  to be appended</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append(ByteBuffer buf)
    {
        if (buf == null)
        {
            throw new ArgumentNullException(nameof(buf));
        }

        return Append(buf.Buf, 0, buf.Count);
    }

    /// <summary>
    ///     Appends the string representation of an  int .
    /// </summary>
    /// <param name="i">the  int  to be appended</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append(int i) => Append((double)i);

    public ByteBuffer Append(byte b) => Append_i(b);

    /// <summary>
    ///     Appends a string representation of a  float  according
    ///     to the Pdf conventions.
    /// </summary>
    /// <param name="i">the  float  to be appended</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append(float i) => Append((double)i);

    /// <summary>
    ///     Appends a string representation of a  double  according
    ///     to the Pdf conventions.
    /// </summary>
    /// <param name="d">the  double  to be appended</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append(double d)
    {
        Append(FormatDouble(d, this));
        return this;
    }

    /// <summary>
    ///     Appends an  int . The size of the array will grow by one.
    /// </summary>
    /// <param name="b">the int to be appended</param>
    /// <returns>a reference to this  ByteBuffer  object</returns>
    public ByteBuffer Append_i(int b)
    {
        var newcount = Count + 1;
        if (newcount > Buf.Length)
        {
            var newbuf = new byte[Math.Max(Buf.Length << 1, newcount)];
            Array.Copy(Buf, 0, newbuf, 0, Count);
            Buf = newbuf;
        }

        Buf[Count] = (byte)b;
        Count = newcount;
        return this;
    }

    public ByteBuffer AppendHex(byte b)
    {
        Append(_bytes[(b >> 4) & 0x0f]);
        return Append(_bytes[b & 0x0f]);
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count) => 0;

    /// <summary>
    ///     Sets the size to zero.
    /// </summary>
    public void Reset()
    {
        Count = 0;
    }

    public override long Seek(long offset, SeekOrigin origin) => 0;

    public override void SetLength(long value)
    {
    }

    /// <summary>
    ///     Creates a newly allocated byte array. Its size is the current
    ///     size of this output stream and the valid contents of the buffer
    ///     have been copied into it.
    /// </summary>
    /// <returns>the current contents of this output stream, as a byte array.</returns>
    public byte[] ToByteArray()
    {
        var newbuf = new byte[Count];
        Array.Copy(Buf, 0, newbuf, 0, Count);
        return newbuf;
    }

    /// <summary>
    ///     Converts the buffer's contents into a string, translating bytes into
    ///     characters according to the platform's default character encoding.
    /// </summary>
    /// <returns>string translated from the buffer's contents.</returns>
    public override string ToString()
    {
        var tmp = convertToChar(Buf);
        return new string(tmp, 0, Count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Append(buffer, offset, count);
    }

    public override void WriteByte(byte value)
    {
        Append(value);
    }

    /// <summary>
    ///     Writes the complete contents of this byte buffer output to
    ///     the specified output stream argument, as if by calling the output
    ///     stream's write method using  out.Write(buf, 0, count) .
    ///     @exception  IOException  if an I/O error occurs.
    /// </summary>
    /// <param name="str">the output stream to which to write the data.</param>
    public void WriteTo(Stream str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        str.Write(Buf, 0, Count);
    }

    private static byte[] convertToBytes(int i)
    {
        var size = (int)Math.Floor(Math.Log(i) / Math.Log(10));
        if (i % 100 != 0)
        {
            size += 2;
        }

        if (i % 10 != 0)
        {
            size++;
        }

        if (i < 100)
        {
            size++;
            if (i < 10)
            {
                size++;
            }
        }

        size--;
        var cache = new byte[size];
        size--;
        if (i < 100)
        {
            cache[0] = (byte)'0';
        }

        if (i % 10 != 0)
        {
            cache[size--] = _bytes[i % 10];
        }

        if (i % 100 != 0)
        {
            cache[size--] = _bytes[i / 10 % 10];
            cache[size--] = (byte)'.';
        }

        size = (int)Math.Floor(Math.Log(i) / Math.Log(10)) - 1;
        var add = 0;
        while (add < size)
        {
            cache[add] = _bytes[i / (int)Math.Pow(10, size - add + 1) % 10];
            add++;
        }

        return cache;
    }

    /// <summary>
    ///     Converts the buffer's contents into a string, translating bytes into
    ///     characters according to the specified character encoding.
    ///     @throws UnsupportedEncodingException
    ///     If the named encoding is not supported.
    /// </summary>
    private char[] convertToChar(byte[] buf)
    {
        var retVal = new char[Count + 1];
        for (var i = 0; i <= Count; i++)
        {
            retVal[i] = (char)buf[i];
        }

        return retVal;
    }
}