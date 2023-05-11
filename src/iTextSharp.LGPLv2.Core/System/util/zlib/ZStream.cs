namespace System.util.zlib;

public sealed class ZStream
{
    private const int DefWbits = MaxWbits;
    private const int MaxMemLevel = 9;
    private const int MaxWbits = 15; // 32K LZ77 window
    private const int ZBufError = -5;
    private const int ZDataError = -3;
    private const int ZErrno = -1;
    private const int ZFinish = 4;
    private const int ZFullFlush = 3;
    private const int ZMemError = -4;
    private const int ZNeedDict = 2;
    private const int ZNoFlush = 0;
    private const int ZOk = 0;
    private const int ZPartialFlush = 1;
    private const int ZStreamEnd = 1;
    private const int ZStreamError = -2;
    private const int ZSyncFlush = 2;
    private const int ZVersionError = -6;

    internal Adler32 _adler = new();

    public long Adler;
    public int AvailIn;
    public int AvailOut;

    internal int DataType;

    // total nb of bytes output so far
    internal Deflate Dstate;

    internal Inflate Istate;
    public string Msg;

    public byte[] NextIn;

    // next input byte
    public int NextInIndex;

    public byte[] NextOut;

    // next output byte should be put there
    public int NextOutIndex;

    // number of bytes available at next_in
    public long TotalIn;

    // total nb of input bytes read so far
    // remaining free space at next_out
    public long TotalOut;

    public int Deflate(int flush)
    {
        if (Dstate == null)
        {
            return ZStreamError;
        }

        return Dstate.deflate(this, flush);
    }

    public int DeflateEnd()
    {
        if (Dstate == null)
        {
            return ZStreamError;
        }

        var ret = Dstate.DeflateEnd();
        Dstate = null;
        return ret;
    }

    public int DeflateInit(int level) => DeflateInit(level, MaxWbits);

    public int DeflateInit(int level, bool nowrap) => DeflateInit(level, MaxWbits, nowrap);

    public int DeflateInit(int level, int bits) => DeflateInit(level, bits, false);

    public int DeflateInit(int level, int bits, bool nowrap)
    {
        Dstate = new Deflate();
        return Dstate.DeflateInit(this, level, nowrap ? -bits : bits);
    }

    public int DeflateParams(int level, int strategy)
    {
        if (Dstate == null)
        {
            return ZStreamError;
        }

        return Dstate.DeflateParams(this, level, strategy);
    }

    public int DeflateSetDictionary(byte[] dictionary, int dictLength)
    {
        if (Dstate == null)
        {
            return ZStreamError;
        }

        return Dstate.DeflateSetDictionary(this, dictionary, dictLength);
    }

    public void Free()
    {
        NextIn = null;
        NextOut = null;
        Msg = null;
        _adler = null;
    }

    public int Inflate(int f)
    {
        if (Istate == null)
        {
            return ZStreamError;
        }

        return zlib.Inflate.inflate(this, f);
    }

    public int InflateEnd()
    {
        if (Istate == null)
        {
            return ZStreamError;
        }

        var ret = Istate.InflateEnd(this);
        Istate = null;
        return ret;
    }

    // best guess about the data type: ascii or binary
    public int InflateInit() => InflateInit(DefWbits);

    public int InflateInit(bool nowrap) => InflateInit(DefWbits, nowrap);

    public int InflateInit(int w) => InflateInit(w, false);

    public int InflateInit(int w, bool nowrap)
    {
        Istate = new Inflate();
        return Istate.InflateInit(this, nowrap ? -w : w);
    }

    public int InflateSetDictionary(byte[] dictionary, int dictLength)
    {
        if (Istate == null)
        {
            return ZStreamError;
        }

        return zlib.Inflate.InflateSetDictionary(this, dictionary, dictLength);
    }

    public int InflateSync()
    {
        if (Istate == null)
        {
            return ZStreamError;
        }

        return zlib.Inflate.InflateSync(this);
    }

    /// <summary>
    ///     Flush as much pending output as possible. All deflate() output goes
    /// </summary>
    /// <summary>
    ///     through this function so some applications may wish to modify it
    /// </summary>
    /// <summary>
    ///     to avoid allocating a large strm->next_out buffer and copying into it.
    /// </summary>
    /// <summary>
    ///     (See also read_buf()).
    /// </summary>
    internal void flush_pending()
    {
        var len = Dstate.Pending;

        if (len > AvailOut)
        {
            len = AvailOut;
        }

        if (len == 0)
        {
            return;
        }

        if (Dstate.PendingBuf.Length <= Dstate.PendingOut ||
            NextOut.Length <= NextOutIndex ||
            Dstate.PendingBuf.Length < Dstate.PendingOut + len ||
            NextOut.Length < NextOutIndex + len)
        {
            //      System.out.println(dstate.pending_buf.length+", "+dstate.pending_out+
            //			 ", "+next_out.length+", "+next_out_index+", "+len);
            //      System.out.println("avail_out="+avail_out);
        }

        Array.Copy(Dstate.PendingBuf, Dstate.PendingOut,
                   NextOut, NextOutIndex, len);

        NextOutIndex += len;
        Dstate.PendingOut += len;
        TotalOut += len;
        AvailOut -= len;
        Dstate.Pending -= len;
        if (Dstate.Pending == 0)
        {
            Dstate.PendingOut = 0;
        }
    }

    /// <summary>
    ///     Read a new buffer from the current input stream, update the adler32
    /// </summary>
    /// <summary>
    ///     and total number of bytes read.  All deflate() input goes through
    /// </summary>
    /// <summary>
    ///     this function so some applications may wish to modify it to avoid
    /// </summary>
    /// <summary>
    ///     allocating a large strm->next_in buffer and copying from it.
    /// </summary>
    /// <summary>
    ///     (See also flush_pending()).
    /// </summary>
    internal int read_buf(byte[] buf, int start, int size)
    {
        var len = AvailIn;

        if (len > size)
        {
            len = size;
        }

        if (len == 0)
        {
            return 0;
        }

        AvailIn -= len;

        if (Dstate.Noheader == 0)
        {
            Adler = Adler32.adler32(Adler, NextIn, NextInIndex, len);
        }

        Array.Copy(NextIn, NextInIndex, buf, start, len);
        NextInIndex += len;
        TotalIn += len;
        return len;
    }
}