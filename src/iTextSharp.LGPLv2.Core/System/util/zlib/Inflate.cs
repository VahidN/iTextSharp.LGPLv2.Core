namespace System.util.zlib;

internal sealed class Inflate
{
    internal const int Z_FINISH = 4;
    internal const int Z_FULL_FLUSH = 3;
    internal const int Z_NO_FLUSH = 0;
    internal const int Z_PARTIAL_FLUSH = 1;
    internal const int Z_SYNC_FLUSH = 2;

    private const int Bad = 13;

    // waiting for flag byte
    // four dictionary check bytes to go
    // three dictionary check bytes to go
    // two dictionary check bytes to go
    // one dictionary check byte to go
    // waiting for inflateSetDictionary
    private const int Blocks = 7;

    private const int Check1 = 11;
    private const int Check2 = 10;
    private const int Check3 = 9;
    private const int Check4 = 8;
    private const int Dict0 = 6;
    private const int Dict1 = 5;
    private const int Dict2 = 4;
    private const int Dict3 = 3;

    private const int Dict4 = 2;

    // four check bytes to go
    // three check bytes to go
    // two check bytes to go
    // one check byte to go
    private const int Done = 12;

    private const int Flag = 1;
    private const int MaxWbits = 15; // 32K LZ77 window
    private const int Method = 0;

    /// <summary>
    ///     preset dictionary flag in zlib header
    /// </summary>
    private const int PresetDict = 0x20;

    private const int ZBufError = -5;
    private const int ZDataError = -3;
    private const int ZDeflated = 8;

    private const int ZErrno = -1;
    private const int ZMemError = -4;
    private const int ZNeedDict = 2;
    private const int ZOk = 0;
    private const int ZStreamEnd = 1;
    private const int ZStreamError = -2;
    private const int ZVersionError = -6;

    // waiting for method byte
    // decompressing blocks
    // finished check, done
    // got an error--stay here

    // current inflate mode

    // if FLAGS, method byte

    // log2(window size)  (8..15, defaults to 15)

    // current inflate_blocks state

    private static readonly byte[] _mark = { 0, 0, 0xff, 0xff };
    internal InfBlocks blocks;

    /// <summary>
    ///     if BAD, inflateSync's marker bytes count
    /// </summary>
    internal int Marker;

    /// <summary>
    ///     mode dependent information
    /// </summary>
    internal int method;

    internal int Mode;

    internal long Need;

    // stream check value
    /// <summary>
    ///     mode independent information
    /// </summary>
    internal int Nowrap;

    /// <summary>
    ///     if CHECK, check values to compare
    /// </summary>
    internal readonly long[] Was = new long[1];

    // computed check value
    // flag for no wrapper
    internal int Wbits;

    internal static int inflate(ZStream z, int f)
    {
        int r;
        int b;

        if (z == null || z.Istate == null || z.NextIn == null)
        {
            return ZStreamError;
        }

        f = f == Z_FINISH ? ZBufError : ZOk;
        r = ZBufError;
        while (true)
        {
            //System.out.println("mode: "+z.istate.mode);
            switch (z.Istate.Mode)
            {
                case Method:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    if (((z.Istate.method = z.NextIn[z.NextInIndex++]) & 0xf) != ZDeflated)
                    {
                        z.Istate.Mode = Bad;
                        z.Msg = "unknown compression method";
                        z.Istate.Marker = 5; // can't try inflateSync
                        break;
                    }

                    if ((z.Istate.method >> 4) + 8 > z.Istate.Wbits)
                    {
                        z.Istate.Mode = Bad;
                        z.Msg = "invalid window size";
                        z.Istate.Marker = 5; // can't try inflateSync
                        break;
                    }

                    z.Istate.Mode = Flag;
                    goto case Flag;
                case Flag:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    b = z.NextIn[z.NextInIndex++] & 0xff;

                    if (((z.Istate.method << 8) + b) % 31 != 0)
                    {
                        z.Istate.Mode = Bad;
                        z.Msg = "incorrect header check";
                        z.Istate.Marker = 5; // can't try inflateSync
                        break;
                    }

                    if ((b & PresetDict) == 0)
                    {
                        z.Istate.Mode = Blocks;
                        break;
                    }

                    z.Istate.Mode = Dict4;
                    goto case Dict4;
                case Dict4:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    z.Istate.Need = ((z.NextIn[z.NextInIndex++] & 0xff) << 24) & 0xff000000L;
                    z.Istate.Mode = Dict3;
                    goto case Dict3;
                case Dict3:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    z.Istate.Need += ((z.NextIn[z.NextInIndex++] & 0xff) << 16) & 0xff0000L;
                    z.Istate.Mode = Dict2;
                    goto case Dict2;
                case Dict2:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    z.Istate.Need += ((z.NextIn[z.NextInIndex++] & 0xff) << 8) & 0xff00L;
                    z.Istate.Mode = Dict1;
                    goto case Dict1;
                case Dict1:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    z.Istate.Need += z.NextIn[z.NextInIndex++] & 0xffL;
                    z.Adler = z.Istate.Need;
                    z.Istate.Mode = Dict0;
                    return ZNeedDict;
                case Dict0:
                    z.Istate.Mode = Bad;
                    z.Msg = "need dictionary";
                    z.Istate.Marker = 0; // can try inflateSync
                    return ZStreamError;
                case Blocks:

                    r = z.Istate.blocks.Proc(z, r);
                    if (r == ZDataError)
                    {
                        z.Istate.Mode = Bad;
                        z.Istate.Marker = 0; // can try inflateSync
                        break;
                    }

                    if (r == ZOk)
                    {
                        r = f;
                    }

                    if (r != ZStreamEnd)
                    {
                        return r;
                    }

                    r = f;
                    z.Istate.blocks.Reset(z, z.Istate.Was);
                    if (z.Istate.Nowrap != 0)
                    {
                        z.Istate.Mode = Done;
                        break;
                    }

                    z.Istate.Mode = Check4;
                    goto case Check4;
                case Check4:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    z.Istate.Need = ((z.NextIn[z.NextInIndex++] & 0xff) << 24) & 0xff000000L;
                    z.Istate.Mode = Check3;
                    goto case Check3;
                case Check3:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    z.Istate.Need += ((z.NextIn[z.NextInIndex++] & 0xff) << 16) & 0xff0000L;
                    z.Istate.Mode = Check2;
                    goto case Check2;
                case Check2:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    z.Istate.Need += ((z.NextIn[z.NextInIndex++] & 0xff) << 8) & 0xff00L;
                    z.Istate.Mode = Check1;
                    goto case Check1;
                case Check1:

                    if (z.AvailIn == 0)
                    {
                        return r;
                    }

                    r = f;

                    z.AvailIn--;
                    z.TotalIn++;
                    z.Istate.Need += z.NextIn[z.NextInIndex++] & 0xffL;

                    if ((int)z.Istate.Was[0] != (int)z.Istate.Need)
                    {
                        z.Istate.Mode = Bad;
                        z.Msg = "incorrect data check";
                        z.Istate.Marker = 5; // can't try inflateSync
                        break;
                    }

                    z.Istate.Mode = Done;
                    goto case Done;
                case Done:
                    return ZStreamEnd;
                case Bad:
                    return ZDataError;
                default:
                    return ZStreamError;
            }
        }
    }

    internal int InflateEnd(ZStream z)
    {
        if (blocks != null)
        {
            blocks.Free(z);
        }

        blocks = null;
        //    ZFREE(z, z->state);
        return ZOk;
    }

    internal int InflateInit(ZStream z, int w)
    {
        z.Msg = null;
        blocks = null;

        // handle undocumented nowrap option (no zlib header or check)
        Nowrap = 0;
        if (w < 0)
        {
            w = -w;
            Nowrap = 1;
        }

        // set window size
        if (w < 8 || w > 15)
        {
            InflateEnd(z);
            return ZStreamError;
        }

        Wbits = w;

        z.Istate.blocks = new InfBlocks(z,
                                        z.Istate.Nowrap != 0 ? null : this,
                                        1 << w);

        // reset state
        InflateReset(z);
        return ZOk;
    }

    internal static int InflateReset(ZStream z)
    {
        if (z == null || z.Istate == null)
        {
            return ZStreamError;
        }

        z.TotalIn = z.TotalOut = 0;
        z.Msg = null;
        z.Istate.Mode = z.Istate.Nowrap != 0 ? Blocks : Method;
        z.Istate.blocks.Reset(z, null);
        return ZOk;
    }

    internal static int InflateSetDictionary(ZStream z, byte[] dictionary, int dictLength)
    {
        var index = 0;
        var length = dictLength;
        if (z == null || z.Istate == null || z.Istate.Mode != Dict0)
        {
            return ZStreamError;
        }

        if (Adler32.adler32(1L, dictionary, 0, dictLength) != z.Adler)
        {
            return ZDataError;
        }

        z.Adler = Adler32.adler32(0, null, 0, 0);

        if (length >= 1 << z.Istate.Wbits)
        {
            length = (1 << z.Istate.Wbits) - 1;
            index = dictLength - length;
        }

        z.Istate.blocks.set_dictionary(dictionary, index, length);
        z.Istate.Mode = Blocks;
        return ZOk;
    }

    internal static int InflateSync(ZStream z)
    {
        int n; // number of bytes to look at
        int p; // pointer to bytes
        int m; // number of marker bytes found in a row
        long r, w; // temporaries to save total_in and total_out

        // set up
        if (z == null || z.Istate == null)
        {
            return ZStreamError;
        }

        if (z.Istate.Mode != Bad)
        {
            z.Istate.Mode = Bad;
            z.Istate.Marker = 0;
        }

        if ((n = z.AvailIn) == 0)
        {
            return ZBufError;
        }

        p = z.NextInIndex;
        m = z.Istate.Marker;

        // search
        while (n != 0 && m < 4)
        {
            if (z.NextIn[p] == _mark[m])
            {
                m++;
            }
            else if (z.NextIn[p] != 0)
            {
                m = 0;
            }
            else
            {
                m = 4 - m;
            }

            p++;
            n--;
        }

        // restore
        z.TotalIn += p - z.NextInIndex;
        z.NextInIndex = p;
        z.AvailIn = n;
        z.Istate.Marker = m;

        // return no joy or set up to restart on a new block
        if (m != 4)
        {
            return ZDataError;
        }

        r = z.TotalIn;
        w = z.TotalOut;
        InflateReset(z);
        z.TotalIn = r;
        z.TotalOut = w;
        z.Istate.Mode = Blocks;
        return ZOk;
    }

    /// <summary>
    ///     Returns true if inflate is currently at the end of a block generated
    /// </summary>
    /// <summary>
    ///     by Z_SYNC_FLUSH or Z_FULL_FLUSH. This function is used by one PPP
    /// </summary>
    /// <summary>
    ///     implementation to provide an additional safety check. PPP uses Z_SYNC_FLUSH
    /// </summary>
    /// <summary>
    ///     but removes the length bytes of the resulting empty stored block. When
    /// </summary>
    /// <summary>
    ///     decompressing, PPP checks that at the end of input packet, inflate is
    /// </summary>
    /// <summary>
    ///     waiting for these length bytes.
    /// </summary>
    internal static int InflateSyncPoint(ZStream z)
    {
        if (z == null || z.Istate == null || z.Istate.blocks == null)
        {
            return ZStreamError;
        }

        return z.Istate.blocks.sync_point();
    }
}