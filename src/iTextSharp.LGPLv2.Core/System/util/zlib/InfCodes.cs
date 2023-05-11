namespace System.util.zlib;

internal sealed class InfCodes
{
    private const int Badcode = 9;

    private const int Copy = 5;

    private const int Dist = 3;

    // i: get distance next
    private const int Distext = 4;

    private const int End = 8;

    private const int Len = 1;

    // i: get length/literal/eob next
    private const int Lenext = 2;

    // i: getting length extra (have base)
    // i: getting distance extra
    // o: copying bytes in window, waiting for space
    private const int Lit = 6;

    /// <summary>
    ///     waiting for "i:"=input,
    /// </summary>
    /// <summary>
    ///     "o:"=output,
    /// </summary>
    /// <summary>
    ///     "x:"=nothing
    /// </summary>
    private const int Start = 0;

    // x: set up for LEN
    // o: got literal, waiting for output space
    private const int Wash = 7;

    private const int ZBufError = -5;

    private const int ZDataError = -3;

    private const int ZErrno = -1;

    private const int ZMemError = -4;

    private const int ZNeedDict = 2;

    private const int ZOk = 0;

    private const int ZStreamEnd = 1;

    private const int ZStreamError = -2;

    private const int ZVersionError = -6;

    private static readonly int[] _inflateMask =
    {
        0x00000000, 0x00000001, 0x00000003, 0x00000007, 0x0000000f,
        0x0000001f, 0x0000003f, 0x0000007f, 0x000000ff, 0x000001ff,
        0x000003ff, 0x000007ff, 0x00000fff, 0x00001fff, 0x00003fff,
        0x00007fff, 0x0000ffff,
    };
    // o: got eob, possibly still output waiting
    // x: got eob and all data flushed
    // x: got error

    private byte _dbits;
    private int _dist;

    private int[] _dtree;

    // distance tree
    private int _dtreeIndex;

    /// <summary>
    ///     if EXT or COPY, where and how much
    /// </summary>
    private int _get;

    private byte _lbits;

    /// <summary>
    ///     mode dependent information
    /// </summary>
    private int _len;

    private int _lit;

    // ltree bits decoded per branch
    // dtree bits decoder per branch
    private int[] _ltree;

    // literal/length/eob tree
    private int _ltreeIndex;

    private int _mode; // current inflate_codes mode
    private int _need;
    private int[] _tree; // pointer into tree

    private int _treeIndex;
    // bits needed
    // bits to get for extra
    // distance back to copy from

    // literal/length/eob tree
    // distance tree

    internal static void Free(ZStream z)
    {
        //  ZFREE(z, c);
    }

    internal static int inflate_fast(int bl, int bd,
                                     int[] tl, int tlIndex,
                                     int[] td, int tdIndex,
                                     InfBlocks s, ZStream z)
    {
        int t; // temporary pointer
        int[] tp; // temporary pointer
        int tpIndex; // temporary pointer
        int e; // extra bits or operation
        int b; // bit buffer
        int k; // bits in bit buffer
        int p; // input data pointer
        int n; // bytes available there
        int q; // output window write pointer
        int m; // bytes to end of window or read pointer
        int ml; // mask for literal/length tree
        int md; // mask for distance tree
        int c; // bytes to copy
        int d; // distance back to copy from
        int r; // copy source pointer

        int tpIndexT3; // (tp_index+t)*3

        // load input, output, bit values
        p = z.NextInIndex;
        n = z.AvailIn;
        b = s.Bitb;
        k = s.Bitk;
        q = s.Write;
        m = q < s.Read ? s.Read - q - 1 : s.End - q;

        // initialize masks
        ml = _inflateMask[bl];
        md = _inflateMask[bd];

        // do until not enough input or output space for fast loop
        do
        {
            // assume called with m >= 258 && n >= 10
            // get literal/length code
            while (k < 20)
            {
                // max bits for literal/length code
                n--;
                b |= (z.NextIn[p++] & 0xff) << k;
                k += 8;
            }

            t = b & ml;
            tp = tl;
            tpIndex = tlIndex;
            tpIndexT3 = (tpIndex + t) * 3;
            if ((e = tp[tpIndexT3]) == 0)
            {
                b >>= tp[tpIndexT3 + 1];
                k -= tp[tpIndexT3 + 1];

                s.Window[q++] = (byte)tp[tpIndexT3 + 2];
                m--;
                continue;
            }

            do
            {
                b >>= tp[tpIndexT3 + 1];
                k -= tp[tpIndexT3 + 1];

                if ((e & 16) != 0)
                {
                    e &= 15;
                    c = tp[tpIndexT3 + 2] + (b & _inflateMask[e]);

                    b >>= e;
                    k -= e;

                    // decode distance base of block to copy
                    while (k < 15)
                    {
                        // max bits for distance code
                        n--;
                        b |= (z.NextIn[p++] & 0xff) << k;
                        k += 8;
                    }

                    t = b & md;
                    tp = td;
                    tpIndex = tdIndex;
                    tpIndexT3 = (tpIndex + t) * 3;
                    e = tp[tpIndexT3];

                    do
                    {
                        b >>= tp[tpIndexT3 + 1];
                        k -= tp[tpIndexT3 + 1];

                        if ((e & 16) != 0)
                        {
                            // get extra bits to add to distance base
                            e &= 15;
                            while (k < e)
                            {
                                // get extra bits (up to 13)
                                n--;
                                b |= (z.NextIn[p++] & 0xff) << k;
                                k += 8;
                            }

                            d = tp[tpIndexT3 + 2] + (b & _inflateMask[e]);

                            b >>= e;
                            k -= e;

                            // do the copy
                            m -= c;
                            if (q >= d)
                            {
                                // offset before dest
                                //  just copy
                                r = q - d;
                                if (q - r > 0 && 2 > q - r)
                                {
                                    s.Window[q++] = s.Window[r++]; // minimum count is three,
                                    s.Window[q++] = s.Window[r++]; // so unroll loop a little
                                    c -= 2;
                                }
                                else
                                {
                                    Array.Copy(s.Window, r, s.Window, q, 2);
                                    q += 2;
                                    r += 2;
                                    c -= 2;
                                }
                            }
                            else
                            {
                                // else offset after destination
                                r = q - d;
                                do
                                {
                                    r += s.End; // force pointer in window
                                } while (r < 0); // covers invalid distances

                                e = s.End - r;
                                if (c > e)
                                {
                                    // if source crosses,
                                    c -= e; // wrapped copy
                                    if (q - r > 0 && e > q - r)
                                    {
                                        do
                                        {
                                            s.Window[q++] = s.Window[r++];
                                        } while (--e != 0);
                                    }
                                    else
                                    {
                                        Array.Copy(s.Window, r, s.Window, q, e);
                                        q += e;
                                        r += e;
                                        e = 0;
                                    }

                                    r = 0; // copy rest from start of window
                                }
                            }

                            // copy all or what's left
                            if (q - r > 0 && c > q - r)
                            {
                                do
                                {
                                    s.Window[q++] = s.Window[r++];
                                } while (--c != 0);
                            }
                            else
                            {
                                Array.Copy(s.Window, r, s.Window, q, c);
                                q += c;
                                r += c;
                                c = 0;
                            }

                            break;
                        }

                        if ((e & 64) == 0)
                        {
                            t += tp[tpIndexT3 + 2];
                            t += b & _inflateMask[e];
                            tpIndexT3 = (tpIndex + t) * 3;
                            e = tp[tpIndexT3];
                        }
                        else
                        {
                            z.Msg = "invalid distance code";

                            c = z.AvailIn - n;
                            c = k >> 3 < c ? k >> 3 : c;
                            n += c;
                            p -= c;
                            k -= c << 3;

                            s.Bitb = b;
                            s.Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            s.Write = q;

                            return ZDataError;
                        }
                    } while (true);

                    break;
                }

                if ((e & 64) == 0)
                {
                    t += tp[tpIndexT3 + 2];
                    t += b & _inflateMask[e];
                    tpIndexT3 = (tpIndex + t) * 3;
                    if ((e = tp[tpIndexT3]) == 0)
                    {
                        b >>= tp[tpIndexT3 + 1];
                        k -= tp[tpIndexT3 + 1];

                        s.Window[q++] = (byte)tp[tpIndexT3 + 2];
                        m--;
                        break;
                    }
                }
                else if ((e & 32) != 0)
                {
                    c = z.AvailIn - n;
                    c = k >> 3 < c ? k >> 3 : c;
                    n += c;
                    p -= c;
                    k -= c << 3;

                    s.Bitb = b;
                    s.Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    s.Write = q;

                    return ZStreamEnd;
                }
                else
                {
                    z.Msg = "invalid literal/length code";

                    c = z.AvailIn - n;
                    c = k >> 3 < c ? k >> 3 : c;
                    n += c;
                    p -= c;
                    k -= c << 3;

                    s.Bitb = b;
                    s.Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    s.Write = q;

                    return ZDataError;
                }
            } while (true);
        } while (m >= 258 && n >= 10);

        // not enough input or output--restore pointers and return
        c = z.AvailIn - n;
        c = k >> 3 < c ? k >> 3 : c;
        n += c;
        p -= c;
        k -= c << 3;

        s.Bitb = b;
        s.Bitk = k;
        z.AvailIn = n;
        z.TotalIn += p - z.NextInIndex;
        z.NextInIndex = p;
        s.Write = q;

        return ZOk;
    }

    internal void Init(int bl, int bd,
                       int[] tl, int tlIndex,
                       int[] td, int tdIndex, ZStream z)
    {
        _mode = Start;
        _lbits = (byte)bl;
        _dbits = (byte)bd;
        _ltree = tl;
        _ltreeIndex = tlIndex;
        _dtree = td;
        _dtreeIndex = tdIndex;
        _tree = null;
    }

    internal int Proc(InfBlocks s, ZStream z, int r)
    {
        int j; // temporary storage
        int tindex; // temporary pointer
        int e; // extra bits or operation
        var b = 0; // bit buffer
        var k = 0; // bits in bit buffer
        var p = 0; // input data pointer
        int n; // bytes available there
        int q; // output window write pointer
        int m; // bytes to end of window or read pointer
        int f; // pointer to copy strings from

        // copy input/output information to locals (UPDATE macro restores)
        p = z.NextInIndex;
        n = z.AvailIn;
        b = s.Bitb;
        k = s.Bitk;
        q = s.Write;
        m = q < s.Read ? s.Read - q - 1 : s.End - q;

        // process input and output based on current state
        while (true)
        {
            switch (_mode)
            {
                // waiting for "i:"=input, "o:"=output, "x:"=nothing
                case Start: // x: set up for LEN
                    if (m >= 258 && n >= 10)
                    {
                        s.Bitb = b;
                        s.Bitk = k;
                        z.AvailIn = n;
                        z.TotalIn += p - z.NextInIndex;
                        z.NextInIndex = p;
                        s.Write = q;
                        r = inflate_fast(_lbits, _dbits,
                                         _ltree, _ltreeIndex,
                                         _dtree, _dtreeIndex,
                                         s, z);

                        p = z.NextInIndex;
                        n = z.AvailIn;
                        b = s.Bitb;
                        k = s.Bitk;
                        q = s.Write;
                        m = q < s.Read ? s.Read - q - 1 : s.End - q;

                        if (r != ZOk)
                        {
                            _mode = r == ZStreamEnd ? Wash : Badcode;
                            break;
                        }
                    }

                    _need = _lbits;
                    _tree = _ltree;
                    _treeIndex = _ltreeIndex;

                    _mode = Len;
                    goto case Len;
                case Len: // i: get length/literal/eob next
                    j = _need;

                    while (k < j)
                    {
                        if (n != 0)
                        {
                            r = ZOk;
                        }
                        else
                        {
                            s.Bitb = b;
                            s.Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            s.Write = q;
                            return s.inflate_flush(z, r);
                        }

                        n--;
                        b |= (z.NextIn[p++] & 0xff) << k;
                        k += 8;
                    }

                    tindex = (_treeIndex + (b & _inflateMask[j])) * 3;

                    b >>= _tree[tindex + 1];
                    k -= _tree[tindex + 1];

                    e = _tree[tindex];

                    if (e == 0)
                    {
                        // literal
                        _lit = _tree[tindex + 2];
                        _mode = Lit;
                        break;
                    }

                    if ((e & 16) != 0)
                    {
                        // length
                        _get = e & 15;
                        _len = _tree[tindex + 2];
                        _mode = Lenext;
                        break;
                    }

                    if ((e & 64) == 0)
                    {
                        // next table
                        _need = e;
                        _treeIndex = tindex / 3 + _tree[tindex + 2];
                        break;
                    }

                    if ((e & 32) != 0)
                    {
                        // end of block
                        _mode = Wash;
                        break;
                    }

                    _mode = Badcode; // invalid code
                    z.Msg = "invalid literal/length code";
                    r = ZDataError;

                    s.Bitb = b;
                    s.Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    s.Write = q;
                    return s.inflate_flush(z, r);

                case Lenext: // i: getting length extra (have base)
                    j = _get;

                    while (k < j)
                    {
                        if (n != 0)
                        {
                            r = ZOk;
                        }
                        else
                        {
                            s.Bitb = b;
                            s.Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            s.Write = q;
                            return s.inflate_flush(z, r);
                        }

                        n--;
                        b |= (z.NextIn[p++] & 0xff) << k;
                        k += 8;
                    }

                    _len += b & _inflateMask[j];

                    b >>= j;
                    k -= j;

                    _need = _dbits;
                    _tree = _dtree;
                    _treeIndex = _dtreeIndex;
                    _mode = Dist;
                    goto case Dist;
                case Dist: // i: get distance next
                    j = _need;

                    while (k < j)
                    {
                        if (n != 0)
                        {
                            r = ZOk;
                        }
                        else
                        {
                            s.Bitb = b;
                            s.Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            s.Write = q;
                            return s.inflate_flush(z, r);
                        }

                        n--;
                        b |= (z.NextIn[p++] & 0xff) << k;
                        k += 8;
                    }

                    tindex = (_treeIndex + (b & _inflateMask[j])) * 3;

                    b >>= _tree[tindex + 1];
                    k -= _tree[tindex + 1];

                    e = _tree[tindex];
                    if ((e & 16) != 0)
                    {
                        // distance
                        _get = e & 15;
                        _dist = _tree[tindex + 2];
                        _mode = Distext;
                        break;
                    }

                    if ((e & 64) == 0)
                    {
                        // next table
                        _need = e;
                        _treeIndex = tindex / 3 + _tree[tindex + 2];
                        break;
                    }

                    _mode = Badcode; // invalid code
                    z.Msg = "invalid distance code";
                    r = ZDataError;

                    s.Bitb = b;
                    s.Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    s.Write = q;
                    return s.inflate_flush(z, r);

                case Distext: // i: getting distance extra
                    j = _get;

                    while (k < j)
                    {
                        if (n != 0)
                        {
                            r = ZOk;
                        }
                        else
                        {
                            s.Bitb = b;
                            s.Bitk = k;
                            z.AvailIn = n;
                            z.TotalIn += p - z.NextInIndex;
                            z.NextInIndex = p;
                            s.Write = q;
                            return s.inflate_flush(z, r);
                        }

                        n--;
                        b |= (z.NextIn[p++] & 0xff) << k;
                        k += 8;
                    }

                    _dist += b & _inflateMask[j];

                    b >>= j;
                    k -= j;

                    _mode = Copy;
                    goto case Copy;
                case Copy: // o: copying bytes in window, waiting for space
                    f = q - _dist;
                    while (f < 0)
                    {
                        // modulo window size-"while" instead
                        f += s.End; // of "if" handles invalid distances
                    }

                    while (_len != 0)
                    {
                        if (m == 0)
                        {
                            if (q == s.End && s.Read != 0)
                            {
                                q = 0;
                                m = q < s.Read ? s.Read - q - 1 : s.End - q;
                            }

                            if (m == 0)
                            {
                                s.Write = q;
                                r = s.inflate_flush(z, r);
                                q = s.Write;
                                m = q < s.Read ? s.Read - q - 1 : s.End - q;

                                if (q == s.End && s.Read != 0)
                                {
                                    q = 0;
                                    m = q < s.Read ? s.Read - q - 1 : s.End - q;
                                }

                                if (m == 0)
                                {
                                    s.Bitb = b;
                                    s.Bitk = k;
                                    z.AvailIn = n;
                                    z.TotalIn += p - z.NextInIndex;
                                    z.NextInIndex = p;
                                    s.Write = q;
                                    return s.inflate_flush(z, r);
                                }
                            }
                        }

                        s.Window[q++] = s.Window[f++];
                        m--;

                        if (f == s.End)
                        {
                            f = 0;
                        }

                        _len--;
                    }

                    _mode = Start;
                    break;
                case Lit: // o: got literal, waiting for output space
                    if (m == 0)
                    {
                        if (q == s.End && s.Read != 0)
                        {
                            q = 0;
                            m = q < s.Read ? s.Read - q - 1 : s.End - q;
                        }

                        if (m == 0)
                        {
                            s.Write = q;
                            r = s.inflate_flush(z, r);
                            q = s.Write;
                            m = q < s.Read ? s.Read - q - 1 : s.End - q;

                            if (q == s.End && s.Read != 0)
                            {
                                q = 0;
                                m = q < s.Read ? s.Read - q - 1 : s.End - q;
                            }

                            if (m == 0)
                            {
                                s.Bitb = b;
                                s.Bitk = k;
                                z.AvailIn = n;
                                z.TotalIn += p - z.NextInIndex;
                                z.NextInIndex = p;
                                s.Write = q;
                                return s.inflate_flush(z, r);
                            }
                        }
                    }

                    r = ZOk;

                    s.Window[q++] = (byte)_lit;
                    m--;

                    _mode = Start;
                    break;
                case Wash: // o: got eob, possibly more output
                    if (k > 7)
                    {
                        // return unused byte, if any
                        k -= 8;
                        n++;
                        p--; // can always return one
                    }

                    s.Write = q;
                    r = s.inflate_flush(z, r);
                    q = s.Write;
                    m = q < s.Read ? s.Read - q - 1 : s.End - q;

                    if (s.Read != s.Write)
                    {
                        s.Bitb = b;
                        s.Bitk = k;
                        z.AvailIn = n;
                        z.TotalIn += p - z.NextInIndex;
                        z.NextInIndex = p;
                        s.Write = q;
                        return s.inflate_flush(z, r);
                    }

                    _mode = End;
                    goto case End;
                case End:
                    r = ZStreamEnd;
                    s.Bitb = b;
                    s.Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    s.Write = q;
                    return s.inflate_flush(z, r);

                case Badcode: // x: got error

                    r = ZDataError;

                    s.Bitb = b;
                    s.Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    s.Write = q;
                    return s.inflate_flush(z, r);

                default:
                    r = ZStreamError;

                    s.Bitb = b;
                    s.Bitk = k;
                    z.AvailIn = n;
                    z.TotalIn += p - z.NextInIndex;
                    z.NextInIndex = p;
                    s.Write = q;
                    return s.inflate_flush(z, r);
            }
        }
    }
}