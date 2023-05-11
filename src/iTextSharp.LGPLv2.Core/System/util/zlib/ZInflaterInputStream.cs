namespace System.util.zlib;

/// <summary>
///     Summary description for DeflaterOutputStream.
/// </summary>
public class ZInflaterInputStream : Stream
{
    private const int Bufsize = 4192;
    private readonly byte[] _buf1 = new byte[1];
    private bool _nomoreinput;
    protected byte[] Buf = new byte[Bufsize];
    protected int FlushLevel = JZlib.Z_NO_FLUSH;
    protected Stream Inp;
    protected ZStream Z = new();

    public ZInflaterInputStream(Stream inp) : this(inp, false)
    {
    }

    public ZInflaterInputStream(Stream inp, bool nowrap)
    {
        Inp = inp;
        Z.InflateInit(nowrap);
        Z.NextIn = Buf;
        Z.NextInIndex = 0;
        Z.AvailIn = 0;
    }

    public override bool CanRead =>
        // TODO:  Add DeflaterOutputStream.CanRead getter implementation
        true;

    public override bool CanSeek =>
        // TODO:  Add DeflaterOutputStream.CanSeek getter implementation
        false;

    public override bool CanWrite =>
        // TODO:  Add DeflaterOutputStream.CanWrite getter implementation
        false;

    public override long Length =>
        // TODO:  Add DeflaterOutputStream.Length getter implementation
        0;

    public override long Position
    {
        get =>
            // TODO:  Add DeflaterOutputStream.Position getter implementation
            0;
        set
        {
            // TODO:  Add DeflaterOutputStream.Position setter implementation
        }
    }

    public override void Flush()
    {
        Inp.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count == 0)
        {
            return 0;
        }

        int err;
        Z.NextOut = buffer;
        Z.NextOutIndex = offset;
        Z.AvailOut = count;
        do
        {
            if (Z.AvailIn == 0 && !_nomoreinput)
            {
                // if buffer is empty and more input is avaiable, refill it
                Z.NextInIndex = 0;
                Z.AvailIn = Inp.Read(Buf, 0, Bufsize); //(BUFSIZE<z.avail_out ? BUFSIZE : z.avail_out));
                if (Z.AvailIn == 0)
                {
                    Z.AvailIn = 0;
                    _nomoreinput = true;
                }
            }

            err = Z.Inflate(FlushLevel);
            if (_nomoreinput && err == JZlib.Z_BUF_ERROR)
            {
                return -1;
            }

            if (err != JZlib.Z_OK && err != JZlib.Z_STREAM_END)
            {
                throw new IOException("inflating: " + Z.Msg);
            }

            if ((_nomoreinput || err == JZlib.Z_STREAM_END) && Z.AvailOut == count)
            {
                return 0;
            }
        } while (Z.AvailOut == count && err == JZlib.Z_OK);

        //System.err.print("("+(len-z.avail_out)+")");
        return count - Z.AvailOut;
    }

    public override int ReadByte()
    {
        if (Read(_buf1, 0, 1) <= 0)
        {
            return -1;
        }

        return _buf1[0] & 0xFF;
    }

    public override long Seek(long offset, SeekOrigin origin) =>
        // TODO:  Add DeflaterOutputStream.Seek implementation
        0;

    public override void SetLength(long value)
    {
        // TODO:  Add DeflaterOutputStream.SetLength implementation
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
    }

    public override void WriteByte(byte value)
    {
    }
}