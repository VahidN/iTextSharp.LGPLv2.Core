namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public class OutputStreamCounter : Stream
{
    protected int counter;
    protected Stream Outc;

    public OutputStreamCounter(Stream _outc) => Outc = _outc;

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public int Counter => counter;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        Outc.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public void ResetCounter()
    {
        counter = 0;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        counter += count;
        Outc.Write(buffer, offset, count);
    }
}