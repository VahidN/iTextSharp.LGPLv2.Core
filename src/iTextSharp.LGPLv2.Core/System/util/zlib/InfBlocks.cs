namespace System.util.zlib;

internal sealed class InfBlocks
{
    private const int Bad = 9;

    private const int Btree = 4;

    // get length, distance trees for a dynamic block
    private const int Codes = 6;

    private const int Done = 8;

    private const int Dry = 7;

    // get bit lengths tree for a dynamic block
    private const int Dtree = 5;

    private const int Lens = 1;
    private const int Many = 1440;

    // get lengths for stored
    private const int Stored = 2;

    // get type bits (3, including end bit)
    // processing stored block
    private const int Table = 3;

    private const int Type = 0;

    private const int ZBufError = -5;

    private const int ZDataError = -3;

    private const int ZErrno = -1;

    private const int ZMemError = -4;

    private const int ZNeedDict = 2;

    private const int ZOk = 0;

    private const int ZStreamEnd = 1;

    private const int ZStreamError = -2;

    private const int ZVersionError = -6;

    /// <summary>
    ///     Table for deflate from PKZIP's appnote.txt.
    /// </summary>
    private static readonly int[] _border =
    {
        // Order of the bit length code lengths
        16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15,
    };

    /// <summary>
    ///     And'ing with mask[n] masks the lower n bits
    /// </summary>
    private static readonly int[] _inflateMask =
    {
        0x00000000, 0x00000001, 0x00000003, 0x00000007, 0x0000000f,
        0x0000001f, 0x0000003f, 0x0000007f, 0x000000ff, 0x000001ff,
        0x000003ff, 0x000007ff, 0x00000fff, 0x00001fff, 0x00003fff,
        0x00007fff, 0x0000ffff,
    };
    // get table lengths
    // processing fixed or dynamic block
    // output remaining window bytes
    // finished last block, done
    // ot a data error--stuck here

    // current inflate_block mode

    // if STORED, bytes left to copy

    // bit length decoding tree

    // if CODES, current state

    private int _last; // true if this block is the last block
    internal readonly int[] Bb = new int[1];
    internal int Bitb;

    /// <summary>
    ///     mode independent information
    /// </summary>
    internal int Bitk;

    internal int[] Blens;
    internal long Check;
    internal readonly object Checkfn;
    internal readonly InfCodes codes = new();

    internal readonly int End;

    // bits in bit buffer
    // bit buffer
    internal int[] Hufts;

    internal int Index;
    internal readonly InfTree Inftree = new();
    internal int Left;

    internal int Mode;

    // one byte after sliding window
    internal int Read;
    internal int table;

    // table lengths (14 bits)
    // index into blens (or border)
    // bit lengths of codes
    // bit length tree depth
    internal readonly int[] Tb = new int[1];

    // single malloc for tree space
    internal byte[] Window;

    // sliding window
    // window read pointer
    internal int Write;

    // window write pointer
    // check function
    // check on output
    internal InfBlocks(ZStream z, object checkfn, int w)
    {
        Hufts = new int[Many * 3];
        Window = new byte[w];
        End = w;
        Checkfn = checkfn;
        Mode = Type;
        Reset(z, null);
    }

    internal void Free(ZStream z)
    {
        Reset(z, null);
        Window = null;
        Hufts = null;
        //ZFREE(z, s);
    }

    /// <summary>
    ///     copy as much as possible from the sliding window to the output area
    /// </summary>
    internal int inflate_flush(ZStream z, int r)
    {
        int n;
        int p;
        int q;

        // local copies of source and destination pointers
        p = z.NextOutIndex;
        q = Read;

        // compute number of bytes to copy as far as end of window
        n = (q <= Write ? Write : End) - q;
        if (n > z.AvailOut)
        {
            n = z.AvailOut;
        }

        if (n != 0 && r == ZBufError)
        {
            r = ZOk;
        }

        // update counters
        z.AvailOut -= n;
        z.TotalOut += n;

        // update check information
        if (Checkfn != null)
        {
            z.Adler = Check = Adler32.adler32(Check, Window, q, n);
        }

        // copy as far as end of window
        Array.Copy(Window, q, z.NextOut, p, n);
        p += n;
        q += n;

        // see if more to copy at beginning of window
        if (q == End)
        {
            // wrap pointers
            q = 0;
            if (Write == End)
            {
                Write = 0;
            }

            // compute bytes to copy
            n = Write - q;
            if (n > z.AvailOut)
            {
                n = z.AvailOut;
            }

            if (n != 0 && r == ZBufError)
            {
                r = ZOk;
            }

            // update counters
            z.AvailOut -= n;
            z.TotalOut += n;

            // update check information
            if (Checkfn != null)
            {
                z.Adler = Check = Adler32.adler32(Check, Window, q, n);
            }

            // copy
            Array.Copy(Window, q, z.NextOut, p, n);
            p += n;
            q += n;
        }

        // update pointers
        z.NextOutIndex = p;
        Read = q;

        // done
        return r;
    }

    internal int Proc(ZStream z, int r)
    {
        int t; // temporary storage
        int b; // bit buffer
        int k; // bits in bit buffer
        int p; // input data pointer
        int n; // bytes available there
        int q; // output window write pointer
        int m;
        {
            // bytes to end of window or read pointer

            // copy input/output information to locals (UPDATE macro restores)
            p = z.NextInIndex;
            n = z.AvailIn;
            b = Bitb;
            k = Bitk;
        }
        {
            q = Write;
            m = q < Read ? Read - q - 1 : End - q;
        }

        // process input based on current state
        while (true)
        {
            switch (Mode)
            {
                case Type:

                    while (k < 3)
                    {
                        if (n != 0)
                        {
                            r = ZOk;
                        }
                        else
                        {
                            Bitb = b;
                            Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            Write = q;
                            return inflate_flush(z, r);
                        }

                        n--;
                        b |= (z.NextIn[p++] & 0xff) << k;
                        k += 8;
                    }

                    t = b & 7;
                    _last = t & 1;

                    switch (t >> 1)
                    {
                        case 0:
                        {
                            // stored
                            b >>= 3;
                            k -= 3;
                        }
                            t = k & 7;
                        {
                            // go to byte boundary

                            b >>= t;
                            k -= t;
                        }
                            Mode = Lens; // get length of stored block
                            break;
                        case 1:
                        {
                            // fixed
                            var bl = new int[1];
                            var bd = new int[1];
                            var tl = new int[1][];
                            var td = new int[1][];

                            InfTree.inflate_trees_fixed(bl, bd, tl, td, z);
                            codes.Init(bl[0], bd[0], tl[0], 0, td[0], 0, z);
                        }
                        {
                            b >>= 3;
                            k -= 3;
                        }

                            Mode = Codes;
                            break;
                        case 2:
                        {
                            // dynamic

                            b >>= 3;
                            k -= 3;
                        }

                            Mode = Table;
                            break;
                        case 3:
                        {
                            // illegal

                            b >>= 3;
                            k -= 3;
                        }
                            Mode = Bad;
                            z.Msg = "invalid block type";
                            r = ZDataError;

                            Bitb = b;
                            Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            Write = q;
                            return inflate_flush(z, r);
                    }

                    break;
                case Lens:

                    while (k < 32)
                    {
                        if (n != 0)
                        {
                            r = ZOk;
                        }
                        else
                        {
                            Bitb = b;
                            Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            Write = q;
                            return inflate_flush(z, r);
                        }

                        ;
                        n--;
                        b |= (z.NextIn[p++] & 0xff) << k;
                        k += 8;
                    }

                    if (((~b >> 16) & 0xffff) != (b & 0xffff))
                    {
                        Mode = Bad;
                        z.Msg = "invalid stored block lengths";
                        r = ZDataError;

                        Bitb = b;
                        Bitk = k;
                        z.AvailIn = n;
                        z.TotalIn += p - z.NextInIndex;
                        z.NextInIndex = p;
                        Write = q;
                        return inflate_flush(z, r);
                    }

                    Left = b & 0xffff;
                    b = k = 0; // dump bits
                    Mode = Left != 0 ? Stored : _last != 0 ? Dry : Type;
                    break;
                case Stored:
                    if (n == 0)
                    {
                        Bitb = b;
                        Bitk = k;
                        z.AvailIn = n;
                        z.TotalIn += p - z.NextInIndex;
                        z.NextInIndex = p;
                        Write = q;
                        return inflate_flush(z, r);
                    }

                    if (m == 0)
                    {
                        if (q == End && Read != 0)
                        {
                            q = 0;
                            m = q < Read ? Read - q - 1 : End - q;
                        }

                        if (m == 0)
                        {
                            Write = q;
                            r = inflate_flush(z, r);
                            q = Write;
                            m = q < Read ? Read - q - 1 : End - q;
                            if (q == End && Read != 0)
                            {
                                q = 0;
                                m = q < Read ? Read - q - 1 : End - q;
                            }

                            if (m == 0)
                            {
                                Bitb = b;
                                Bitk = k;
                                z.AvailIn = n;
                                z.TotalIn += p - z.NextInIndex;
                                z.NextInIndex = p;
                                Write = q;
                                return inflate_flush(z, r);
                            }
                        }
                    }

                    r = ZOk;

                    t = Left;
                    if (t > n)
                    {
                        t = n;
                    }

                    if (t > m)
                    {
                        t = m;
                    }

                    Array.Copy(z.NextIn, p, Window, q, t);
                    p += t;
                    n -= t;
                    q += t;
                    m -= t;
                    if ((Left -= t) != 0)
                    {
                        break;
                    }

                    Mode = _last != 0 ? Dry : Type;
                    break;
                case Table:

                    while (k < 14)
                    {
                        if (n != 0)
                        {
                            r = ZOk;
                        }
                        else
                        {
                            Bitb = b;
                            Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            Write = q;
                            return inflate_flush(z, r);
                        }

                        ;
                        n--;
                        b |= (z.NextIn[p++] & 0xff) << k;
                        k += 8;
                    }

                    table = t = b & 0x3fff;
                    if ((t & 0x1f) > 29 || ((t >> 5) & 0x1f) > 29)
                    {
                        Mode = Bad;
                        z.Msg = "too many length or distance symbols";
                        r = ZDataError;

                        Bitb = b;
                        Bitk = k;
                        z.AvailIn = n;
                        z.TotalIn += p - z.NextInIndex;
                        z.NextInIndex = p;
                        Write = q;
                        return inflate_flush(z, r);
                    }

                    t = 258 + (t & 0x1f) + ((t >> 5) & 0x1f);
                    if (Blens == null || Blens.Length < t)
                    {
                        Blens = new int[t];
                    }
                    else
                    {
                        for (var i = 0; i < t; i++)
                        {
                            Blens[i] = 0;
                        }
                    }

                {
                    b >>= 14;
                    k -= 14;
                }

                    Index = 0;
                    Mode = Btree;
                    goto case Btree;
                case Btree:
                    while (Index < 4 + (table >> 10))
                    {
                        while (k < 3)
                        {
                            if (n != 0)
                            {
                                r = ZOk;
                            }
                            else
                            {
                                Bitb = b;
                                Bitk = k;
                                z.AvailIn = n;
                                z.TotalIn += p - z.NextInIndex;
                                z.NextInIndex = p;
                                Write = q;
                                return inflate_flush(z, r);
                            }

                            n--;
                            b |= (z.NextIn[p++] & 0xff) << k;
                            k += 8;
                        }

                        Blens[_border[Index++]] = b & 7;
                        {
                            b >>= 3;
                            k -= 3;
                        }
                    }

                    while (Index < 19)
                    {
                        Blens[_border[Index++]] = 0;
                    }

                    Bb[0] = 7;
                    t = Inftree.inflate_trees_bits(Blens, Bb, Tb, Hufts, z);
                    if (t != ZOk)
                    {
                        r = t;
                        if (r == ZDataError)
                        {
                            Blens = null;
                            Mode = Bad;
                        }

                        Bitb = b;
                        Bitk = k;
                        z.AvailIn = n;
                        z.TotalIn += p - z.NextInIndex;
                        z.NextInIndex = p;
                        Write = q;
                        return inflate_flush(z, r);
                    }

                    Index = 0;
                    Mode = Dtree;
                    goto case Dtree;
                case Dtree:
                    while (true)
                    {
                        t = table;
                        if (!(Index < 258 + (t & 0x1f) + ((t >> 5) & 0x1f)))
                        {
                            break;
                        }

                        int i, j, c;

                        t = Bb[0];

                        while (k < t)
                        {
                            if (n != 0)
                            {
                                r = ZOk;
                            }
                            else
                            {
                                Bitb = b;
                                Bitk = k;
                                z.AvailIn = n;
                                z.TotalIn += p - z.NextInIndex;
                                z.NextInIndex = p;
                                Write = q;
                                return inflate_flush(z, r);
                            }

                            n--;
                            b |= (z.NextIn[p++] & 0xff) << k;
                            k += 8;
                        }

                        if (Tb[0] == -1)
                        {
                            //System.err.println("null...");
                        }

                        t = Hufts[(Tb[0] + (b & _inflateMask[t])) * 3 + 1];
                        c = Hufts[(Tb[0] + (b & _inflateMask[t])) * 3 + 2];

                        if (c < 16)
                        {
                            b >>= t;
                            k -= t;
                            Blens[Index++] = c;
                        }
                        else
                        {
                            // c == 16..18
                            i = c == 18 ? 7 : c - 14;
                            j = c == 18 ? 11 : 3;

                            while (k < t + i)
                            {
                                if (n != 0)
                                {
                                    r = ZOk;
                                }
                                else
                                {
                                    Bitb = b;
                                    Bitk = k;
                                    z.AvailIn = n;
                                    z.TotalIn += p - z.NextInIndex;
                                    z.NextInIndex = p;
                                    Write = q;
                                    return inflate_flush(z, r);
                                }

                                n--;
                                b |= (z.NextIn[p++] & 0xff) << k;
                                k += 8;
                            }

                            b >>= t;
                            k -= t;

                            j += b & _inflateMask[i];

                            b >>= i;
                            k -= i;

                            i = Index;
                            t = table;
                            if (i + j > 258 + (t & 0x1f) + ((t >> 5) & 0x1f) ||
                                (c == 16 && i < 1))
                            {
                                Blens = null;
                                Mode = Bad;
                                z.Msg = "invalid bit length repeat";
                                r = ZDataError;

                                Bitb = b;
                                Bitk = k;
                                z.AvailIn = n;
                                z.TotalIn += p - z.NextInIndex;
                                z.NextInIndex = p;
                                Write = q;
                                return inflate_flush(z, r);
                            }

                            c = c == 16 ? Blens[i - 1] : 0;
                            do
                            {
                                Blens[i++] = c;
                            } while (--j != 0);

                            Index = i;
                        }
                    }

                    Tb[0] = -1;
                {
                    var bl = new int[1];
                    var bd = new int[1];
                    var tl = new int[1];
                    var td = new int[1];
                    bl[0] = 9; // must be <= 9 for lookahead assumptions
                    bd[0] = 6; // must be <= 9 for lookahead assumptions

                    t = table;
                    t = Inftree.inflate_trees_dynamic(257 + (t & 0x1f),
                                                      1 + ((t >> 5) & 0x1f),
                                                      Blens, bl, bd, tl, td, Hufts, z);

                    if (t != ZOk)
                    {
                        if (t == ZDataError)
                        {
                            Blens = null;
                            Mode = Bad;
                        }

                        r = t;

                        Bitb = b;
                        Bitk = k;
                        z.AvailIn = n;
                        z.TotalIn += p - z.NextInIndex;
                        z.NextInIndex = p;
                        Write = q;
                        return inflate_flush(z, r);
                    }

                    codes.Init(bl[0], bd[0], Hufts, tl[0], Hufts, td[0], z);
                }
                    Mode = Codes;
                    goto case Codes;
                case Codes:
                    Bitb = b;
                    Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    Write = q;

                    if ((r = codes.Proc(this, z, r)) != ZStreamEnd)
                    {
                        return inflate_flush(z, r);
                    }

                    r = ZOk;
                    InfCodes.Free(z);

                    p = z.NextInIndex;
                    n = z.AvailIn;
                    b = Bitb;
                    k = Bitk;
                    q = Write;
                    m = q < Read ? Read - q - 1 : End - q;

                    if (_last == 0)
                    {
                        Mode = Type;
                        break;
                    }

                    Mode = Dry;
                    goto case Dry;
                case Dry:
                    Write = q;
                    r = inflate_flush(z, r);
                    q = Write;
                    m = q < Read ? Read - q - 1 : End - q;
                    if (Read != Write)
                    {
                        Bitb = b;
                        Bitk = k;
                        z.AvailIn = n;
                        z.TotalIn += p - z.NextInIndex;
                        z.NextInIndex = p;
                        Write = q;
                        return inflate_flush(z, r);
                    }

                    Mode = Done;
                    goto case Done;
                case Done:
                    r = ZStreamEnd;

                    Bitb = b;
                    Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    Write = q;
                    return inflate_flush(z, r);
                case Bad:
                    r = ZDataError;

                    Bitb = b;
                    Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    Write = q;
                    return inflate_flush(z, r);

                default:
                    r = ZStreamError;

                    Bitb = b;
                    Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    Write = q;
                    return inflate_flush(z, r);
            }
        }
    }

    internal void Reset(ZStream z, long[] c)
    {
        if (c != null)
        {
            c[0] = Check;
        }

        if (Mode == Btree || Mode == Dtree)
        {
        }

        if (Mode == Codes)
        {
            InfCodes.Free(z);
        }

        Mode = Type;
        Bitk = 0;
        Bitb = 0;
        Read = Write = 0;

        if (Checkfn != null)
        {
            z.Adler = Check = Adler32.adler32(0L, null, 0, 0);
        }
    }

    internal void set_dictionary(byte[] d, int start, int n)
    {
        Array.Copy(d, start, Window, 0, n);
        Read = Write = n;
    }

    /// <summary>
    ///     Returns true if inflate is currently at the end of a block generated
    /// </summary>
    /// <summary>
    ///     by Z_SYNC_FLUSH or Z_FULL_FLUSH.
    /// </summary>
    internal int sync_point() => Mode == Lens ? 1 : 0;
}