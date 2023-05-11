namespace iTextSharp.text.rtf.document.output;

/// <summary>
///     The RtfNilOutputStream is a dummy output stream that sends all
///     bytes to the big byte bucket in the sky. It is used to improve
///     speed in those situations where processing is required, but
///     the results are not needed.
///     @author Thomas Bickel (tmb99@inode.at)
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfNilOutputStream : Stream
{
    private long _size;

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
    }

    /// <summary>
    ///     Returns the number of bytes that have been written to this buffer so far.
    /// </summary>
    /// <returns>number of bytes written to this buffer</returns>
    public long GetSize() => _size;

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

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

        _size += count;
    }

    public override void WriteByte(byte value)
    {
        ++_size;
    }
}