namespace iTextSharp.text.rtf.document.output;

public class RtfByteArrayBuffer : Stream
{
    private readonly List<byte[]> _arrays = new();
    private byte[] _buffer;
    private int _pos;
    private int _size;

    public RtfByteArrayBuffer() : this(256)
    {
    }

    /// <summary>
    ///     Creates a new buffer with the given initial size.
    /// </summary>
    /// <param name="bufferSize">desired initial size in bytes</param>
    public RtfByteArrayBuffer(int bufferSize)
    {
        if (bufferSize <= 0 || bufferSize > 1 << 30)
        {
            throw new ArgumentException($"bufferSize {bufferSize}");
        }

        var n = 1 << 5;
        while (n < bufferSize)
        {
            n <<= 1;
        }

        _buffer = new byte[n];
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /// <summary>
    ///     Appends the given array to this buffer without copying (if possible).
    /// </summary>
    /// <param name="a"></param>
    public void Append(byte[] a)
    {
        if (a == null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        if (a.Length == 0)
        {
            return;
        }

        if (a.Length <= 8)
        {
            Write(a, 0, a.Length);
        }
        else if (a.Length <= 16 && _pos > 0 && _buffer.Length - _pos > a.Length)
        {
            Write(a, 0, a.Length);
        }
        else
        {
            flushBuffer();
            _arrays.Add(a);
            _size += a.Length;
        }
    }

    /// <summary>
    ///     Appends all arrays to this buffer without copying (if possible).
    /// </summary>
    /// <param name="a"></param>
    public void Append(byte[][] a)
    {
        if (a == null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        for (var k = 0; k < a.Length; k++)
        {
            Append(a[k]);
        }
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    /// <summary>
    ///     Resets this buffer.
    /// </summary>
    public void Reset()
    {
        _arrays.Clear();
        _pos = 0;
        _size = 0;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    ///     Returns the number of bytes that have been written to this buffer so far.
    /// </summary>
    /// <returns>number of bytes written to this buffer</returns>
    public long Size() => _size;

    /// <summary>
    ///     Allocates a new array and copies all data that has been written to this buffer to the newly allocated array.
    /// </summary>
    /// <returns>a new byte array</returns>
    public byte[] ToArray()
    {
        var r = new byte[_size];
        var off = 0;
        var n = _arrays.Count;
        for (var k = 0; k < n; k++)
        {
            var src = _arrays[k];
            Array.Copy(src, 0, r, off, src.Length);
            off += src.Length;
        }

        if (_pos > 0)
        {
            Array.Copy(_buffer, 0, r, off, _pos);
        }

        return r;
    }

    /// <summary>
    ///     Returns the internal list of byte array buffers without copying the buffer contents.
    /// </summary>
    /// <returns>an byte aray of buffers</returns>
    public byte[][] ToArrayArray()
    {
        flushBuffer();
        var a = new byte[_arrays.Count][];
        _arrays.CopyTo(a);
        return a;
    }

    public override string ToString() =>
        "RtfByteArrayBuffer: size=" + Size() + " #arrays=" + _arrays.Count + " pos=" + _pos;

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        if (offset < 0 || offset > buffer.Length || count < 0 || offset + count > buffer.Length || offset + count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(offset));
        }

        writeLoop(buffer, offset, count);
    }

    /// <summary>
    ///     Writes all bytes available in the given inputstream to this buffer.
    ///     @throws IOException
    /// </summary>
    /// <param name="inp"></param>
    /// <returns>number of bytes written</returns>
    public long Write(Stream inp)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        long sizeStart = _size;
        while (true)
        {
            var n = inp.Read(_buffer, _pos, _buffer.Length - _pos);
            if (n <= 0)
            {
                break;
            }

            _pos += n;
            _size += n;
            if (_pos == _buffer.Length)
            {
                flushBuffer();
            }
        }

        return _size - sizeStart;
    }

    public override void WriteByte(byte value)
    {
        _buffer[_pos] = value;
        _size++;
        if (++_pos == _buffer.Length)
        {
            flushBuffer();
        }
    }

    /// <summary>
    ///     Writes all data that has been written to this buffer to the given output stream.
    ///     @throws IOException
    /// </summary>
    /// <param name="outp"></param>
    public void WriteTo(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        var n = _arrays.Count;
        for (var k = 0; k < n; k++)
        {
            var src = _arrays[k];
            outp.Write(src, 0, src.Length);
        }

        if (_pos > 0)
        {
            outp.Write(_buffer, 0, _pos);
        }
    }

    private void flushBuffer()
    {
        flushBuffer(1);
    }

    private void flushBuffer(int reqSize)
    {
        if (reqSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(reqSize));
        }

        if (_pos == 0)
        {
            return;
        }

        if (_pos == _buffer.Length)
        {
            //add old buffer, alloc new (possibly larger) buffer
            _arrays.Add(_buffer);
            var newSize = _buffer.Length;
            _buffer = null;
            var max = Math.Max(1, _size >> 24) << 16;
            while (newSize < max)
            {
                newSize <<= 1;
                if (newSize >= reqSize)
                {
                    break;
                }
            }

            _buffer = new byte[newSize];
        }
        else
        {
            //copy buffer contents to newly allocated buffer
            var c = new byte[_pos];
            Array.Copy(_buffer, 0, c, 0, _pos);
            _arrays.Add(c);
        }

        _pos = 0;
    }

    private void writeLoop(byte[] src, int off, int len)
    {
        while (len > 0)
        {
            var room = _buffer.Length - _pos;
            var n = len > room ? room : len;
            Array.Copy(src, off, _buffer, _pos, n);
            len -= n;
            off += n;
            _pos += n;
            _size += n;
            if (_pos == _buffer.Length)
            {
                flushBuffer(len);
            }
        }
    }
}