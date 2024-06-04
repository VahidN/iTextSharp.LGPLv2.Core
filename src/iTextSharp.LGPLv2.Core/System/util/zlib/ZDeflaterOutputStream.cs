namespace System.util.zlib;

/// <summary>
///     Summary description for DeflaterOutputStream.
/// </summary>
public class ZDeflaterOutputStream : Stream
{
    protected byte[] Buf = new byte[Bufsize];
    protected int FlushLevel = JZlib.Z_NO_FLUSH;
    protected Stream Outp;
    protected ZStream Z = new();
    private const int Bufsize = 4192;
    private readonly byte[] _buf1 = new byte[1];

    public ZDeflaterOutputStream(Stream outp) : this(outp, 6, false)
    {
    }

    public ZDeflaterOutputStream(Stream outp, int level) : this(outp, level, false)
    {
    }

    public ZDeflaterOutputStream(Stream outp, int level, bool nowrap)
    {
        Outp = outp;
        Z.DeflateInit(level, nowrap);
    }


    public override bool CanRead =>
        // TODO:  Add DeflaterOutputStream.CanRead getter implementation
        false;

    public override bool CanSeek =>
        // TODO:  Add DeflaterOutputStream.CanSeek getter implementation
        false;

    public override bool CanWrite =>
        // TODO:  Add DeflaterOutputStream.CanWrite getter implementation
        true;

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

#if NETSTANDARD1_3
        public void Close()
#else
    public override void Close()
#endif
    {
        try
        {
            try
            {
                Finish();
            }
            catch (IOException)
            {
            }
        }
        finally
        {
            End();
            Outp.Dispose();
            Outp = null;
        }
    }

    public void End()
    {
        if (Z == null)
        {
            return;
        }

        Z.DeflateEnd();
        Z.Free();
        Z = null;
    }

    public void Finish()
    {
        int err;
        do
        {
            Z.NextOut = Buf;
            Z.NextOutIndex = 0;
            Z.AvailOut = Bufsize;
            err = Z.Deflate(JZlib.Z_FINISH);
            if (err != JZlib.Z_STREAM_END && err != JZlib.Z_OK)
            {
                throw new IOException("deflating: " + Z.Msg);
            }

            if (Bufsize - Z.AvailOut > 0)
            {
                Outp.Write(Buf, 0, Bufsize - Z.AvailOut);
            }
        } while (Z.AvailIn > 0 || Z.AvailOut == 0);

        Flush();
    }

    public override void Flush()
    {
        Outp.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count) =>
        // TODO:  Add DeflaterOutputStream.Read implementation
        0;

    public override long Seek(long offset, SeekOrigin origin) =>
        // TODO:  Add DeflaterOutputStream.Seek implementation
        0;

    public override void SetLength(long value)
    {
        // TODO:  Add DeflaterOutputStream.SetLength implementation
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (count == 0)
        {
            return;
        }

        int err;
        Z.NextIn = buffer;
        Z.NextInIndex = offset;
        Z.AvailIn = count;
        do
        {
            Z.NextOut = Buf;
            Z.NextOutIndex = 0;
            Z.AvailOut = Bufsize;
            err = Z.Deflate(FlushLevel);
            if (err != JZlib.Z_OK)
            {
                throw new IOException("deflating: " + Z.Msg);
            }

            if (Z.AvailOut < Bufsize)
            {
                Outp.Write(Buf, 0, Bufsize - Z.AvailOut);
            }
        } while (Z.AvailIn > 0 || Z.AvailOut == 0);
    }

    public override void WriteByte(byte value)
    {
        _buf1[0] = value;
        Write(_buf1, 0, 1);
    }
}