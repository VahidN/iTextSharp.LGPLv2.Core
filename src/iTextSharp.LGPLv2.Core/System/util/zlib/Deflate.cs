namespace System.util.zlib;

public sealed class Deflate
{
    private const int BlCodes = 19;

    /// <summary>
    ///     block flush performed
    /// </summary>
    private const int BlockDone = 1;

    private const int BufSize = 8 * 2;
    private const int BusyState = 113;
    private const int DCodes = 30;
    private const int DefMemLevel = 8;
    private const int DynTrees = 2;
    private const int EndBlock = 256;
    private const int Fast = 1;

    /// <summary>
    ///     finish done, accept no more input or output
    /// </summary>
    private const int FinishDone = 3;

    /// <summary>
    ///     finish started, need only more output at next deflate
    /// </summary>
    private const int FinishStarted = 2;

    private const int FinishState = 666;
    private const int HeapSize = 2 * LCodes + 1;
    private const int InitState = 42;
    private const int LCodes = Literals + 1 + LengthCodes;
    private const int LengthCodes = 29;
    private const int Literals = 256;
    private const int MaxBits = 15;
    private const int MaxMatch = 258;
    private const int MaxMemLevel = 9;

    private const int MaxWbits = 15;
    private const int MinLookahead = MaxMatch + MinMatch + 1;
    private const int MinMatch = 3;

    /// <summary>
    ///     block not completed, need more input or more output
    /// </summary>
    private const int NeedMore = 0;

    /// <summary>
    ///     preset dictionary flag in zlib header
    /// </summary>
    private const int PresetDict = 0x20;

    /// <summary>
    ///     repeat previous bit length 3-6 times (2 bits of repeat count)
    /// </summary>
    private const int Rep36 = 16;

    /// <summary>
    ///     repeat a zero length 11-138 times  (7 bits of repeat count)
    /// </summary>
    private const int Repz11138 = 18;

    /// <summary>
    ///     repeat a zero length 3-10 times  (3 bits of repeat count)
    /// </summary>
    private const int Repz310 = 17;

    private const int Slow = 2;
    private const int StaticTrees = 1;
    private const int Stored = 0;
    private const int StoredBlock = 0;
    private const int ZAscii = 1;

    /// <summary>
    ///     The three kinds of block type
    /// </summary>
    private const int ZBinary = 0;

    private const int ZBufError = -5;
    private const int ZDataError = -3;
    private const int ZDefaultCompression = -1;

    private const int ZDefaultStrategy = 0;

    /// <summary>
    ///     The deflate compression method
    /// </summary>
    private const int ZDeflated = 8;

    private const int ZErrno = -1;

    private const int ZFiltered = 1;

    private const int ZFinish = 4;

    private const int ZFullFlush = 3;

    private const int ZHuffmanOnly = 2;

    private const int ZMemError = -4;

    private const int ZNeedDict = 2;

    private const int ZNoFlush = 0;

    private const int ZOk = 0;

    private const int ZPartialFlush = 1;

    private const int ZStreamEnd = 1;

    private const int ZStreamError = -2;

    private const int ZSyncFlush = 2;

    private const int ZUnknown = 2;

    private const int ZVersionError = -6;

    private static readonly Config[] _configTable;

    private static readonly string[] _zErrmsg =
    {
        "need dictionary", // Z_NEED_DICT       2
        "stream end", // Z_STREAM_END      1
        "", // Z_OK              0
        "file error", // Z_ERRNO         (-1)
        "stream error", // Z_STREAM_ERROR  (-2)
        "data error", // Z_DATA_ERROR    (-3)
        "insufficient memory", // Z_MEM_ERROR     (-4)
        "buffer error", // Z_BUF_ERROR     (-5)
        "incompatible version", // Z_VERSION_ERROR (-6)
        "",
    };

    /// <summary>
    ///     Output buffer. bits are inserted starting at the bottom (least
    /// </summary>
    /// <summary>
    ///     significant bits).
    /// </summary>
    internal uint BiBuf;

    /// <summary>
    ///     Number of valid bits in bi_buf.  All bits above the last valid bit
    /// </summary>
    /// <summary>
    ///     are always zero.
    /// </summary>
    internal int BiValid;

    /// <summary>
    ///     number of codes at each bit length for an optimal tree
    /// </summary>
    internal readonly short[] BlCount = new short[MaxBits + 1];

    internal readonly Tree BlDesc = new();
    internal int BlockStart;
    internal readonly short[] BlTree;
    internal byte DataType;
    internal int DBuf;
    internal readonly Tree DDesc = new();

    /// <summary>
    ///     Depth of each subtree used as tie breaker for trees of equal frequency
    /// </summary>
    internal readonly byte[] Depth = new byte[2 * LCodes + 1];

    internal readonly short[] DynDtree;
    internal readonly short[] DynLtree;

    /// <summary>
    ///     Use a faster search when the previous match is longer than this
    /// </summary>
    internal int GoodMatch;

    internal int HashBits;

    // log2(hash_size)
    internal int HashMask;

    /// <summary>
    ///     Number of bits by which ins_h must be shifted at each input
    /// </summary>
    /// <summary>
    ///     step. It must be such that after MIN_MATCH steps, the oldest
    /// </summary>
    /// <summary>
    ///     byte no longer takes part in the hash key, that is:
    /// </summary>
    /// <summary>
    ///     hash_shift * MIN_MATCH >= hash_bits
    /// </summary>
    internal int HashShift;

    internal int HashSize;

    internal short[] Head;

    // desc for distance tree
    // desc for bit length tree
    /// <summary>
    ///     heap used to build the Huffman trees
    /// </summary>
    internal readonly int[] Heap = new int[2 * LCodes + 1];

    internal int HeapLen;

    // number of elements in the heap
    internal int HeapMax;

    internal int InsH;
    internal int LastEobLen;
    internal int LastFlush;

    internal int LastLit;

    // element of largest frequency
    /// <summary>
    ///     The sons of heap[n] are heap[2*n] and heap[2*n+1]. heap[0] is not used.
    /// </summary>
    /// <summary>
    ///     The same heap array is used to build all trees.
    /// </summary>
    internal int LBuf;

    internal readonly Tree LDesc = new();
    internal int Level;

    /// <summary>
    ///     Size of match buffer for literals/lengths.  There are 4 reasons for
    /// </summary>
    /// <summary>
    ///     limiting lit_bufsize to 64K:
    /// </summary>
    /// <summary>
    ///     - frequencies can be kept in 16 bit counters
    /// </summary>
    /// <summary>
    ///     - if compression is not successful for the first block, all input
    /// </summary>
    /// <summary>
    ///     data is still in the window so we can still emit a stored block even
    /// </summary>
    /// <summary>
    ///     when input comes from standard input.  (This can also be done for
    /// </summary>
    /// <summary>
    ///     all blocks if lit_bufsize is not greater than 32K.)
    /// </summary>
    /// <summary>
    ///     - if compression is not successful for a file smaller than 64K, we can
    /// </summary>
    /// <summary>
    ///     even emit a stored file instead of a stored block (saving 5 bytes).
    /// </summary>
    /// <summary>
    ///     This is applicable only for zip (not gzip or zlib).
    /// </summary>
    /// <summary>
    ///     - creating new Huffman trees less frequently may not provide fast
    /// </summary>
    /// <summary>
    ///     adaptation to changes in the input data statistics. (Take for
    /// </summary>
    /// <summary>
    ///     example a binary file with poorly compressible code followed by
    /// </summary>
    /// <summary>
    ///     a highly compressible string table.) Smaller buffer sizes give
    /// </summary>
    /// <summary>
    ///     fast adaptation but have of course the overhead of transmitting
    /// </summary>
    /// <summary>
    ///     trees more frequently.
    /// </summary>
    /// <summary>
    ///     - I can't count above 4
    /// </summary>
    internal int LitBufsize;

    internal int Lookahead;
    internal int MatchAvailable;

    internal int Matches;

    // hash index of string to be inserted
    // number of elements in hash table
    // hash_size-1
    /// <summary>
    ///     Window position at the beginning of the current output block. Gets
    /// </summary>
    /// <summary>
    ///     negative when the window is moved backwards.
    /// </summary>
    internal int MatchLength;

    internal int MatchStart;

    /// <summary>
    ///     To speed up deflation, hash chains are never searched beyond this
    /// </summary>
    /// <summary>
    ///     length.  A higher limit improves compression ratio but degrades the speed.
    /// </summary>
    internal int MaxChainLength;

    /// <summary>
    ///     Attempt to find a better match only when the current match is strictly
    /// </summary>
    /// <summary>
    ///     smaller than this value. This mechanism is used only for compression
    /// </summary>
    /// <summary>
    ///     levels >= 4.
    /// </summary>
    internal int MaxLazyMatch;

    // UNKNOWN, BINARY or ASCII
    internal byte Method;

    /// <summary>
    ///     Stop searching when current match exceeds this
    /// </summary>
    internal int NiceMatch;

    internal int Noheader;
    internal int OptLen;
    internal int Pending;

    internal byte[] PendingBuf;

    // output still pending
    internal int PendingBufSize;

    // size of pending_buf
    internal int PendingOut;

    internal short[] Prev;

    /// <summary>
    ///     Length of the best match at previous step. Matches not greater than this
    /// </summary>
    /// <summary>
    ///     are discarded. This is used in the lazy match evaluation.
    /// </summary>
    internal int PrevLength;

    // length of best match
    internal int PrevMatch;

    // bit length of current block with optimal trees
    internal int StaticLen;

    internal int Status;

    // compression level (1..9)
    internal int Strategy;

    internal ZStream Strm;

    // previous match
    // set if previous match exists
    internal int Strstart;

    internal int WBits;
    internal byte[] Window;

    internal int WindowSize;

    // log2(w_size)  (8..16)
    internal int WMask;

    internal int WSize;

    static Deflate()
    {
        _configTable = new Config[10];
        //                         good  lazy  nice  chain
        _configTable[0] = new Config(0, 0, 0, 0, Stored);
        _configTable[1] = new Config(4, 4, 8, 4, Fast);
        _configTable[2] = new Config(4, 5, 16, 8, Fast);
        _configTable[3] = new Config(4, 6, 32, 32, Fast);

        _configTable[4] = new Config(4, 4, 16, 16, Slow);
        _configTable[5] = new Config(8, 16, 32, 32, Slow);
        _configTable[6] = new Config(8, 16, 128, 128, Slow);
        _configTable[7] = new Config(8, 32, 128, 256, Slow);
        _configTable[8] = new Config(32, 128, 258, 1024, Slow);
        _configTable[9] = new Config(32, 258, 258, 4096, Slow);
    }

    // bit length of current block with static trees
    // number of string matches in current block
    // bit length of EOB code for last block
    internal Deflate()
    {
        DynLtree = new short[HeapSize * 2];
        DynDtree = new short[(2 * DCodes + 1) * 2]; // distance tree
        BlTree = new short[(2 * BlCodes + 1) * 2]; // Huffman tree for bit lengths
    }

    internal static bool Smaller(short[] tree, int n, int m, byte[] depth)
    {
        var tn2 = tree[n * 2];
        var tm2 = tree[m * 2];
        return tn2 < tm2 ||
               (tn2 == tm2 && depth[n] <= depth[m]);
    }

    /// <summary>
    ///     Send one empty static block to give enough lookahead for inflate.
    /// </summary>
    /// <summary>
    ///     This takes 10 bits, of which 7 may remain in the bit buffer.
    /// </summary>
    /// <summary>
    ///     The current inflate code requires 9 bits of lookahead. If the
    /// </summary>
    /// <summary>
    ///     last two codes for the previous block (real code plus EOB) were coded
    /// </summary>
    /// <summary>
    ///     on 5 bits or less, inflate may have only 5+3 bits of lookahead to decode
    /// </summary>
    /// <summary>
    ///     the last real code. In this case we send two empty static blocks instead
    /// </summary>
    /// <summary>
    ///     of one. (There are no problems if the previous block is stored or fixed.)
    /// </summary>
    /// <summary>
    ///     To simplify the code, we assume the worst case of last real code encoded
    /// </summary>
    /// <summary>
    ///     on one bit only.
    /// </summary>
    internal void _tr_align()
    {
        send_bits(StaticTrees << 1, 3);
        send_code(EndBlock, StaticTree.StaticLtree);

        bi_flush();

        // Of the 10 bits for the empty block, we have already sent
        // (10 - bi_valid) bits. The lookahead for the last real code (before
        // the EOB of the previous block) was thus at least one plus the length
        // of the EOB plus what we have just sent of the empty static block.
        if (1 + LastEobLen + 10 - BiValid < 9)
        {
            send_bits(StaticTrees << 1, 3);
            send_code(EndBlock, StaticTree.StaticLtree);
            bi_flush();
        }

        LastEobLen = 7;
    }

    /// <summary>
    ///     Determine the best encoding for the current block: dynamic trees, static
    /// </summary>
    /// <summary>
    ///     trees or store, and output the encoded block to the zip file.
    /// </summary>
    internal void _tr_flush_block(int buf, // input block, or NULL if too old
                                  int storedLen, // length of input block
                                  bool eof // true if this is the last block for a file
    )
    {
        int optLenb, staticLenb; // opt_len and static_len in bytes
        var maxBlindex = 0; // index of last bit length code of non zero freq

        // Build the Huffman trees unless a stored block is forced
        if (Level > 0)
        {
            // Check if the file is ascii or binary
            if (DataType == ZUnknown)
            {
                set_data_type();
            }

            // Construct the literal and distance trees
            LDesc.build_tree(this);

            DDesc.build_tree(this);

            // At this point, opt_len and static_len are the total bit lengths of
            // the compressed block data, excluding the tree representations.

            // Build the bit length tree for the above two trees, and get the index
            // in bl_order of the last bit length code to send.
            maxBlindex = build_bl_tree();

            // Determine the best encoding. Compute first the block length in bytes
            optLenb = (OptLen + 3 + 7) >> 3;
            staticLenb = (StaticLen + 3 + 7) >> 3;

            if (staticLenb <= optLenb)
            {
                optLenb = staticLenb;
            }
        }
        else
        {
            optLenb = staticLenb = storedLen + 5; // force a stored block
        }

        if (storedLen + 4 <= optLenb && buf != -1)
        {
            // 4: two words for the lengths
            // The test buf != NULL is only necessary if LIT_BUFSIZE > WSIZE.
            // Otherwise we can't have processed more than WSIZE input bytes since
            // the last block flush, because compression would have been
            // successful. If LIT_BUFSIZE <= WSIZE, it is never too late to
            // transform a block into a stored block.
            _tr_stored_block(buf, storedLen, eof);
        }
        else if (staticLenb == optLenb)
        {
            send_bits((StaticTrees << 1) + (eof ? 1 : 0), 3);
            compress_block(StaticTree.StaticLtree, StaticTree.StaticDtree);
        }
        else
        {
            send_bits((DynTrees << 1) + (eof ? 1 : 0), 3);
            send_all_trees(LDesc.MaxCode + 1, DDesc.MaxCode + 1, maxBlindex + 1);
            compress_block(DynLtree, DynDtree);
        }

        // The above check is made mod 2^32, for files larger than 512 MB
        // and uLong implemented on 32 bits.

        init_block();

        if (eof)
        {
            bi_windup();
        }
    }

    /// <summary>
    ///     Send a stored block
    /// </summary>
    internal void _tr_stored_block(int buf, // input block
                                   int storedLen, // length of input block
                                   bool eof // true if this is the last block for a file
    )
    {
        send_bits((StoredBlock << 1) + (eof ? 1 : 0), 3); // send block type
        copy_block(buf, storedLen, true); // with header
    }

    /// <summary>
    ///     Save the match info and tally the frequency counts. Return true if
    /// </summary>
    /// <summary>
    ///     the current block must be flushed.
    /// </summary>
    internal bool _tr_tally(int dist, // distance of matched string
                            int lc // match length-MIN_MATCH or unmatched char (if dist==0)
    )
    {
        PendingBuf[DBuf + LastLit * 2] = (byte)(dist >> 8);
        PendingBuf[DBuf + LastLit * 2 + 1] = (byte)dist;

        PendingBuf[LBuf + LastLit] = (byte)lc;
        LastLit++;

        if (dist == 0)
        {
            // lc is the unmatched char
            DynLtree[lc * 2]++;
        }
        else
        {
            Matches++;
            // Here, lc is the match length - MIN_MATCH
            dist--; // dist = match distance - 1
            DynLtree[(Tree.LengthCode[lc] + Literals + 1) * 2]++;
            DynDtree[Tree.d_code(dist) * 2]++;
        }

        if ((LastLit & 0x1fff) == 0 && Level > 2)
        {
            // Compute an upper bound for the compressed length
            var outLength = LastLit * 8;
            var inLength = Strstart - BlockStart;
            int dcode;
            for (dcode = 0; dcode < DCodes; dcode++)
            {
                outLength += (int)(DynDtree[dcode * 2] *
                                   (5L + Tree.ExtraDbits[dcode]));
            }

            outLength >>= 3;
            if (Matches < LastLit / 2 && outLength < inLength / 2)
            {
                return true;
            }
        }

        return LastLit == LitBufsize - 1;
        // We avoid equality with lit_bufsize because of wraparound at 64K
        // on 16 bit machines and because stored blocks are restricted to
        // 64K-1 bytes.
    }

    /// <summary>
    ///     Flush the bit buffer, keeping at most 7 bits in it.
    /// </summary>
    internal void bi_flush()
    {
        if (BiValid == 16)
        {
            PendingBuf[Pending++] = (byte)BiBuf /*&0xff*/;
            PendingBuf[Pending++] = (byte)(BiBuf >> 8);
            BiBuf = 0;
            BiValid = 0;
        }
        else if (BiValid >= 8)
        {
            PendingBuf[Pending++] = (byte)BiBuf;
            BiBuf >>= 8;
            BiBuf &= 0x00ff;
            BiValid -= 8;
        }
    }

    /// <summary>
    ///     Flush the bit buffer and align the output on a byte boundary
    /// </summary>
    internal void bi_windup()
    {
        if (BiValid > 8)
        {
            PendingBuf[Pending++] = (byte)BiBuf;
            PendingBuf[Pending++] = (byte)(BiBuf >> 8);
        }
        else if (BiValid > 0)
        {
            PendingBuf[Pending++] = (byte)BiBuf;
        }

        BiBuf = 0;
        BiValid = 0;
    }

    /// <summary>
    ///     Construct the Huffman tree for the bit lengths and return the index in
    /// </summary>
    /// <summary>
    ///     bl_order of the last bit length code to send.
    /// </summary>
    internal int build_bl_tree()
    {
        int maxBlindex; // index of last bit length code of non zero freq

        // Determine the bit length frequencies for literal and distance trees
        scan_tree(DynLtree, LDesc.MaxCode);
        scan_tree(DynDtree, DDesc.MaxCode);

        // Build the bit length tree:
        BlDesc.build_tree(this);
        // opt_len now includes the length of the tree representations, except
        // the lengths of the bit lengths codes and the 5+5+4 bits for the counts.

        // Determine the number of bit length codes to send. The pkzip format
        // requires that at least 4 bit length codes be sent. (appnote.txt says
        // 3 but the actual value used is 4.)
        for (maxBlindex = BlCodes - 1; maxBlindex >= 3; maxBlindex--)
        {
            if (BlTree[Tree.BlOrder[maxBlindex] * 2 + 1] != 0)
            {
                break;
            }
        }

        // Update opt_len to include the bit length tree and counts
        OptLen += 3 * (maxBlindex + 1) + 5 + 5 + 4;

        return maxBlindex;
    }

    /// <summary>
    ///     Send the block data compressed using the given Huffman trees
    /// </summary>
    internal void compress_block(short[] ltree, short[] dtree)
    {
        int dist; // distance of matched string
        int lc; // match length or unmatched char (if dist == 0)
        var lx = 0; // running index in l_buf
        int code; // the code to send
        int extra; // number of extra bits to send

        if (LastLit != 0)
        {
            do
            {
                dist = ((PendingBuf[DBuf + lx * 2] << 8) & 0xff00) |
                       (PendingBuf[DBuf + lx * 2 + 1] & 0xff);
                lc = PendingBuf[LBuf + lx] & 0xff;
                lx++;

                if (dist == 0)
                {
                    send_code(lc, ltree); // send a literal byte
                }
                else
                {
                    // Here, lc is the match length - MIN_MATCH
                    code = Tree.LengthCode[lc];

                    send_code(code + Literals + 1, ltree); // send the length code
                    extra = Tree.ExtraLbits[code];
                    if (extra != 0)
                    {
                        lc -= Tree.BaseLength[code];
                        send_bits(lc, extra); // send the extra length bits
                    }

                    dist--; // dist is now the match distance - 1
                    code = Tree.d_code(dist);

                    send_code(code, dtree); // send the distance code
                    extra = Tree.ExtraDbits[code];
                    if (extra != 0)
                    {
                        dist -= Tree.BaseDist[code];
                        send_bits(dist, extra); // send the extra distance bits
                    }
                } // literal or match pair ?

                // Check that the overlay between pending_buf and d_buf+l_buf is ok:
            } while (lx < LastLit);
        }

        send_code(EndBlock, ltree);
        LastEobLen = ltree[EndBlock * 2 + 1];
    }

    /// <summary>
    ///     Copy a stored block, storing first the length and its
    /// </summary>
    /// <summary>
    ///     one's complement if requested.
    /// </summary>
    internal void copy_block(int buf, // the input data
                             int len, // its length
                             bool header // true if block header must be written
    )
    {
        //int index=0;
        bi_windup(); // align on byte boundary
        LastEobLen = 8; // enough lookahead for inflate

        if (header)
        {
            put_short((short)len);
            put_short((short)~len);
        }

        //  while(len--!=0) {
        //    put_byte(window[buf+index]);
        //    index++;
        //  }
        put_byte(Window, buf, len);
    }

    internal int deflate(ZStream strm, int flush)
    {
        int oldFlush;

        if (flush > ZFinish || flush < 0)
        {
            return ZStreamError;
        }

        if (strm.NextOut == null ||
            (strm.NextIn == null && strm.AvailIn != 0) ||
            (Status == FinishState && flush != ZFinish))
        {
            strm.Msg = _zErrmsg[ZNeedDict - ZStreamError];
            return ZStreamError;
        }

        if (strm.AvailOut == 0)
        {
            strm.Msg = _zErrmsg[ZNeedDict - ZBufError];
            return ZBufError;
        }

        Strm = strm; // just in case
        oldFlush = LastFlush;
        LastFlush = flush;

        // Write the zlib header
        if (Status == InitState)
        {
            var header = (ZDeflated + ((WBits - 8) << 4)) << 8;
            var levelFlags = ((Level - 1) & 0xff) >> 1;

            if (levelFlags > 3)
            {
                levelFlags = 3;
            }

            header |= levelFlags << 6;
            if (Strstart != 0)
            {
                header |= PresetDict;
            }

            header += 31 - header % 31;

            Status = BusyState;
            PutShortMsb(header);


            // Save the adler32 of the preset dictionary:
            if (Strstart != 0)
            {
                PutShortMsb((int)(strm.Adler >> 16));
                PutShortMsb((int)(strm.Adler & 0xffff));
            }

            strm.Adler = Adler32.adler32(0, null, 0, 0);
        }

        // Flush as much pending output as possible
        if (Pending != 0)
        {
            strm.flush_pending();
            if (strm.AvailOut == 0)
            {
                //System.out.println("  avail_out==0");
                // Since avail_out is 0, deflate will be called again with
                // more output space, but possibly with both pending and
                // avail_in equal to zero. There won't be anything to do,
                // but this is not an error situation so make sure we
                // return OK instead of BUF_ERROR at next call of deflate:
                LastFlush = -1;
                return ZOk;
            }

            // Make sure there is something to do and avoid duplicate consecutive
            // flushes. For repeated and useless calls with Z_FINISH, we keep
            // returning Z_STREAM_END instead of Z_BUFF_ERROR.
        }
        else if (strm.AvailIn == 0 && flush <= oldFlush &&
                 flush != ZFinish)
        {
            strm.Msg = _zErrmsg[ZNeedDict - ZBufError];
            return ZBufError;
        }

        // User must not provide more input after the first FINISH:
        if (Status == FinishState && strm.AvailIn != 0)
        {
            strm.Msg = _zErrmsg[ZNeedDict - ZBufError];
            return ZBufError;
        }

        // Start a new block or continue the current one.
        if (strm.AvailIn != 0 || Lookahead != 0 ||
            (flush != ZNoFlush && Status != FinishState))
        {
            var bstate = -1;
            switch (_configTable[Level].Func)
            {
                case Stored:
                    bstate = deflate_stored(flush);
                    break;
                case Fast:
                    bstate = deflate_fast(flush);
                    break;
                case Slow:
                    bstate = deflate_slow(flush);
                    break;
            }

            if (bstate == FinishStarted || bstate == FinishDone)
            {
                Status = FinishState;
            }

            if (bstate == NeedMore || bstate == FinishStarted)
            {
                if (strm.AvailOut == 0)
                {
                    LastFlush = -1; // avoid BUF_ERROR next call, see above
                }

                return ZOk;
                // If flush != Z_NO_FLUSH && avail_out == 0, the next call
                // of deflate should use the same flush parameter to make sure
                // that the flush is complete. So we don't have to output an
                // empty block here, this will be done at next call. This also
                // ensures that for a very small output buffer, we emit at most
                // one empty block.
            }

            if (bstate == BlockDone)
            {
                if (flush == ZPartialFlush)
                {
                    _tr_align();
                }
                else
                {
                    // FULL_FLUSH or SYNC_FLUSH
                    _tr_stored_block(0, 0, false);
                    // For a full flush, this empty block will be recognized
                    // as a special marker by inflate_sync().
                    if (flush == ZFullFlush)
                    {
                        //state.head[s.hash_size-1]=0;
                        for (var i = 0; i < HashSize /*-1*/; i++) // forget history
                        {
                            Head[i] = 0;
                        }
                    }
                }

                strm.flush_pending();
                if (strm.AvailOut == 0)
                {
                    LastFlush = -1; // avoid BUF_ERROR at next call, see above
                    return ZOk;
                }
            }
        }

        if (flush != ZFinish)
        {
            return ZOk;
        }

        if (Noheader != 0)
        {
            return ZStreamEnd;
        }

        // Write the zlib trailer (adler32)
        PutShortMsb((int)(strm.Adler >> 16));
        PutShortMsb((int)(strm.Adler & 0xffff));
        strm.flush_pending();

        // If avail_out is zero, the application will call deflate again
        // to flush the rest.
        Noheader = -1; // write the trailer only once!
        return Pending != 0 ? ZOk : ZStreamEnd;
    }

    /// <summary>
    ///     Compress as much as possible from the input stream, return the current
    /// </summary>
    /// <summary>
    ///     block state.
    /// </summary>
    /// <summary>
    ///     This function does not perform lazy evaluation of matches and inserts
    /// </summary>
    /// <summary>
    ///     new strings in the dictionary only for unmatched strings or for short
    /// </summary>
    /// <summary>
    ///     matches. It is used only for the fast compression options.
    /// </summary>
    internal int deflate_fast(int flush)
    {
        //    short hash_head = 0; // head of the hash chain
        var hashHead = 0; // head of the hash chain
        bool bflush; // set if current block must be flushed

        while (true)
        {
            // Make sure that we always have enough lookahead, except
            // at the end of the input file. We need MAX_MATCH bytes
            // for the next match, plus MIN_MATCH bytes to insert the
            // string following the next match.
            if (Lookahead < MinLookahead)
            {
                fill_window();
                if (Lookahead < MinLookahead && flush == ZNoFlush)
                {
                    return NeedMore;
                }

                if (Lookahead == 0)
                {
                    break; // flush the current block
                }
            }

            // Insert the string window[strstart .. strstart+2] in the
            // dictionary, and set hash_head to the head of the hash chain:
            if (Lookahead >= MinMatch)
            {
                InsH = ((InsH << HashShift) ^ (Window[Strstart + (MinMatch - 1)] & 0xff)) & HashMask;

                //  prev[strstart&w_mask]=hash_head=head[ins_h];
                hashHead = Head[InsH] & 0xffff;
                Prev[Strstart & WMask] = Head[InsH];
                Head[InsH] = (short)Strstart;
            }

            // Find the longest match, discarding those <= prev_length.
            // At this point we have always match_length < MIN_MATCH

            if (hashHead != 0L &&
                ((Strstart - hashHead) & 0xffff) <= WSize - MinLookahead
               )
            {
                // To simplify the code, we prevent matches with the string
                // of window index 0 (in particular we have to avoid a match
                // of the string with itself at the start of the input file).
                if (Strategy != ZHuffmanOnly)
                {
                    MatchLength = longest_match(hashHead);
                }
                // longest_match() sets match_start
            }

            if (MatchLength >= MinMatch)
            {
                //        check_match(strstart, match_start, match_length);

                bflush = _tr_tally(Strstart - MatchStart, MatchLength - MinMatch);

                Lookahead -= MatchLength;

                // Insert new strings in the hash table only if the match length
                // is not too large. This saves time but degrades compression.
                if (MatchLength <= MaxLazyMatch &&
                    Lookahead >= MinMatch)
                {
                    MatchLength--; // string at strstart already in hash table
                    do
                    {
                        Strstart++;

                        InsH = ((InsH << HashShift) ^ (Window[Strstart + (MinMatch - 1)] & 0xff)) & HashMask;
                        //      prev[strstart&w_mask]=hash_head=head[ins_h];
                        hashHead = Head[InsH] & 0xffff;
                        Prev[Strstart & WMask] = Head[InsH];
                        Head[InsH] = (short)Strstart;

                        // strstart never exceeds WSIZE-MAX_MATCH, so there are
                        // always MIN_MATCH bytes ahead.
                    } while (--MatchLength != 0);

                    Strstart++;
                }
                else
                {
                    Strstart += MatchLength;
                    MatchLength = 0;
                    InsH = Window[Strstart] & 0xff;

                    InsH = ((InsH << HashShift) ^ (Window[Strstart + 1] & 0xff)) & HashMask;
                    // If lookahead < MIN_MATCH, ins_h is garbage, but it does not
                    // matter since it will be recomputed at next deflate call.
                }
            }
            else
            {
                // No match, output a literal byte

                bflush = _tr_tally(0, Window[Strstart] & 0xff);
                Lookahead--;
                Strstart++;
            }

            if (bflush)
            {
                flush_block_only(false);
                if (Strm.AvailOut == 0)
                {
                    return NeedMore;
                }
            }
        }

        flush_block_only(flush == ZFinish);
        if (Strm.AvailOut == 0)
        {
            if (flush == ZFinish)
            {
                return FinishStarted;
            }

            return NeedMore;
        }

        return flush == ZFinish ? FinishDone : BlockDone;
    }

    /// <summary>
    ///     Same as above, but achieves better compression. We use a lazy
    /// </summary>
    /// <summary>
    ///     evaluation for matches: a match is finally adopted only if there is
    /// </summary>
    /// <summary>
    ///     no better match at the next window position.
    /// </summary>
    internal int deflate_slow(int flush)
    {
        //    short hash_head = 0;    // head of hash chain
        var hashHead = 0; // head of hash chain
        bool bflush; // set if current block must be flushed

        // Process the input block.
        while (true)
        {
            // Make sure that we always have enough lookahead, except
            // at the end of the input file. We need MAX_MATCH bytes
            // for the next match, plus MIN_MATCH bytes to insert the
            // string following the next match.

            if (Lookahead < MinLookahead)
            {
                fill_window();
                if (Lookahead < MinLookahead && flush == ZNoFlush)
                {
                    return NeedMore;
                }

                if (Lookahead == 0)
                {
                    break; // flush the current block
                }
            }

            // Insert the string window[strstart .. strstart+2] in the
            // dictionary, and set hash_head to the head of the hash chain:

            if (Lookahead >= MinMatch)
            {
                InsH = ((InsH << HashShift) ^ (Window[Strstart + (MinMatch - 1)] & 0xff)) & HashMask;
                //  prev[strstart&w_mask]=hash_head=head[ins_h];
                hashHead = Head[InsH] & 0xffff;
                Prev[Strstart & WMask] = Head[InsH];
                Head[InsH] = (short)Strstart;
            }

            // Find the longest match, discarding those <= prev_length.
            PrevLength = MatchLength;
            PrevMatch = MatchStart;
            MatchLength = MinMatch - 1;

            if (hashHead != 0 && PrevLength < MaxLazyMatch &&
                ((Strstart - hashHead) & 0xffff) <= WSize - MinLookahead
               )
            {
                // To simplify the code, we prevent matches with the string
                // of window index 0 (in particular we have to avoid a match
                // of the string with itself at the start of the input file).

                if (Strategy != ZHuffmanOnly)
                {
                    MatchLength = longest_match(hashHead);
                }
                // longest_match() sets match_start

                if (MatchLength <= 5 && (Strategy == ZFiltered ||
                                         (MatchLength == MinMatch &&
                                          Strstart - MatchStart > 4096)))
                {
                    // If prev_match is also MIN_MATCH, match_start is garbage
                    // but we will ignore the current match anyway.
                    MatchLength = MinMatch - 1;
                }
            }

            // If there was a match at the previous step and the current
            // match is not better, output the previous match:
            if (PrevLength >= MinMatch && MatchLength <= PrevLength)
            {
                var maxInsert = Strstart + Lookahead - MinMatch;
                // Do not insert strings in hash table beyond this.

                //          check_match(strstart-1, prev_match, prev_length);

                bflush = _tr_tally(Strstart - 1 - PrevMatch, PrevLength - MinMatch);

                // Insert in hash table all strings up to the end of the match.
                // strstart-1 and strstart are already inserted. If there is not
                // enough lookahead, the last two strings are not inserted in
                // the hash table.
                Lookahead -= PrevLength - 1;
                PrevLength -= 2;
                do
                {
                    if (++Strstart <= maxInsert)
                    {
                        InsH = ((InsH << HashShift) ^ (Window[Strstart + (MinMatch - 1)] & 0xff)) & HashMask;
                        //prev[strstart&w_mask]=hash_head=head[ins_h];
                        hashHead = Head[InsH] & 0xffff;
                        Prev[Strstart & WMask] = Head[InsH];
                        Head[InsH] = (short)Strstart;
                    }
                } while (--PrevLength != 0);

                MatchAvailable = 0;
                MatchLength = MinMatch - 1;
                Strstart++;

                if (bflush)
                {
                    flush_block_only(false);
                    if (Strm.AvailOut == 0)
                    {
                        return NeedMore;
                    }
                }
            }
            else if (MatchAvailable != 0)
            {
                // If there was no match at the previous position, output a
                // single literal. If there was a match but the current match
                // is longer, truncate the previous match to a single literal.

                bflush = _tr_tally(0, Window[Strstart - 1] & 0xff);

                if (bflush)
                {
                    flush_block_only(false);
                }

                Strstart++;
                Lookahead--;
                if (Strm.AvailOut == 0)
                {
                    return NeedMore;
                }
            }
            else
            {
                // There is no previous match to compare with, wait for
                // the next step to decide.

                MatchAvailable = 1;
                Strstart++;
                Lookahead--;
            }
        }

        if (MatchAvailable != 0)
        {
            bflush = _tr_tally(0, Window[Strstart - 1] & 0xff);
            MatchAvailable = 0;
        }

        flush_block_only(flush == ZFinish);

        if (Strm.AvailOut == 0)
        {
            if (flush == ZFinish)
            {
                return FinishStarted;
            }

            return NeedMore;
        }

        return flush == ZFinish ? FinishDone : BlockDone;
    }

    /// <summary>
    ///     Copy without compression as much as possible from the input stream, return
    /// </summary>
    /// <summary>
    ///     the current block state.
    /// </summary>
    /// <summary>
    ///     This function does not insert new strings in the dictionary since
    /// </summary>
    /// <summary>
    ///     uncompressible data is probably not useful. This function is used
    /// </summary>
    /// <summary>
    ///     only for the level=0 compression option.
    /// </summary>
    /// <summary>
    ///     NOTE: this function should be optimized to avoid extra copying from
    /// </summary>
    /// <summary>
    ///     window to pending_buf.
    /// </summary>
    internal int deflate_stored(int flush)
    {
        // Stored blocks are limited to 0xffff bytes, pending_buf is limited
        // to pending_buf_size, and each stored block has a 5 byte header:

        var maxBlockSize = 0xffff;
        int maxStart;

        if (maxBlockSize > PendingBufSize - 5)
        {
            maxBlockSize = PendingBufSize - 5;
        }

        // Copy as much as possible from input to output:
        while (true)
        {
            // Fill the window as much as possible:
            if (Lookahead <= 1)
            {
                fill_window();
                if (Lookahead == 0 && flush == ZNoFlush)
                {
                    return NeedMore;
                }

                if (Lookahead == 0)
                {
                    break; // flush the current block
                }
            }

            Strstart += Lookahead;
            Lookahead = 0;

            // Emit a stored block if pending_buf will be full:
            maxStart = BlockStart + maxBlockSize;
            if (Strstart == 0 || Strstart >= maxStart)
            {
                // strstart == 0 is possible when wraparound on 16-bit machine
                Lookahead = Strstart - maxStart;
                Strstart = maxStart;

                flush_block_only(false);
                if (Strm.AvailOut == 0)
                {
                    return NeedMore;
                }
            }

            // Flush if we may have to slide, otherwise block_start may become
            // negative and the data will be gone:
            if (Strstart - BlockStart >= WSize - MinLookahead)
            {
                flush_block_only(false);
                if (Strm.AvailOut == 0)
                {
                    return NeedMore;
                }
            }
        }

        flush_block_only(flush == ZFinish);
        if (Strm.AvailOut == 0)
        {
            return flush == ZFinish ? FinishStarted : NeedMore;
        }

        return flush == ZFinish ? FinishDone : BlockDone;
    }

    internal int DeflateEnd()
    {
        if (Status != InitState && Status != BusyState && Status != FinishState)
        {
            return ZStreamError;
        }

        // Deallocate in reverse order of allocations:
        PendingBuf = null;
        Head = null;
        Prev = null;
        Window = null;
        // free
        // dstate=null;
        return Status == BusyState ? ZDataError : ZOk;
    }

    internal int DeflateInit(ZStream strm, int level, int bits) =>
        DeflateInit2(strm, level, ZDeflated, bits, DefMemLevel,
                     ZDefaultStrategy);

    internal int DeflateInit(ZStream strm, int level) => DeflateInit(strm, level, MaxWbits);

    internal int DeflateInit2(ZStream strm, int level, int method, int windowBits,
                              int memLevel, int strategy)
    {
        var noheader = 0;
        //    byte[] my_version=ZLIB_VERSION;

        //
        //  if (version == null || version[0] != my_version[0]
        //  || stream_size != sizeof(z_stream)) {
        //  return Z_VERSION_ERROR;
        //  }

        strm.Msg = null;

        if (level == ZDefaultCompression)
        {
            level = 6;
        }

        if (windowBits < 0)
        {
            // undocumented feature: suppress zlib header
            noheader = 1;
            windowBits = -windowBits;
        }

        if (memLevel < 1 || memLevel > MaxMemLevel ||
            method != ZDeflated ||
            windowBits < 9 || windowBits > 15 || level < 0 || level > 9 ||
            strategy < 0 || strategy > ZHuffmanOnly)
        {
            return ZStreamError;
        }

        strm.Dstate = this;

        Noheader = noheader;
        WBits = windowBits;
        WSize = 1 << WBits;
        WMask = WSize - 1;

        HashBits = memLevel + 7;
        HashSize = 1 << HashBits;
        HashMask = HashSize - 1;
        HashShift = (HashBits + MinMatch - 1) / MinMatch;

        Window = new byte[WSize * 2];
        Prev = new short[WSize];
        Head = new short[HashSize];

        LitBufsize = 1 << (memLevel + 6); // 16K elements by default

        // We overlay pending_buf and d_buf+l_buf. This works since the average
        // output size for (length,distance) codes is <= 24 bits.
        PendingBuf = new byte[LitBufsize * 4];
        PendingBufSize = LitBufsize * 4;

        DBuf = LitBufsize / 2;
        LBuf = (1 + 2) * LitBufsize;

        Level = level;

        //System.out.println("level="+level);

        Strategy = strategy;
        Method = (byte)method;

        return DeflateReset(strm);
    }

    internal int DeflateParams(ZStream strm, int _level, int _strategy)
    {
        var err = ZOk;

        if (_level == ZDefaultCompression)
        {
            _level = 6;
        }

        if (_level < 0 || _level > 9 ||
            _strategy < 0 || _strategy > ZHuffmanOnly)
        {
            return ZStreamError;
        }

        if (_configTable[Level].Func != _configTable[_level].Func &&
            strm.TotalIn != 0)
        {
            // Flush the last buffer:
            err = strm.Deflate(ZPartialFlush);
        }

        if (Level != _level)
        {
            Level = _level;
            MaxLazyMatch = _configTable[Level].MaxLazy;
            GoodMatch = _configTable[Level].GoodLength;
            NiceMatch = _configTable[Level].NiceLength;
            MaxChainLength = _configTable[Level].MaxChain;
        }

        Strategy = _strategy;
        return err;
    }

    internal int DeflateReset(ZStream strm)
    {
        strm.TotalIn = strm.TotalOut = 0;
        strm.Msg = null; //
        strm.DataType = ZUnknown;

        Pending = 0;
        PendingOut = 0;

        if (Noheader < 0)
        {
            Noheader = 0; // was set to -1 by deflate(..., Z_FINISH);
        }

        Status = Noheader != 0 ? BusyState : InitState;
        strm.Adler = Adler32.adler32(0, null, 0, 0);

        LastFlush = ZNoFlush;

        tr_init();
        lm_init();
        return ZOk;
    }

    internal int DeflateSetDictionary(ZStream strm, byte[] dictionary, int dictLength)
    {
        var length = dictLength;
        var index = 0;

        if (dictionary == null || Status != InitState)
        {
            return ZStreamError;
        }

        strm.Adler = Adler32.adler32(strm.Adler, dictionary, 0, dictLength);

        if (length < MinMatch)
        {
            return ZOk;
        }

        if (length > WSize - MinLookahead)
        {
            length = WSize - MinLookahead;
            index = dictLength - length; // use the tail of the dictionary
        }

        Array.Copy(dictionary, index, Window, 0, length);
        Strstart = length;
        BlockStart = length;

        // Insert all strings in the hash table (except for the last two bytes).
        // s->lookahead stays null, so s->ins_h will be recomputed at the next
        // call of fill_window.

        InsH = Window[0] & 0xff;
        InsH = ((InsH << HashShift) ^ (Window[1] & 0xff)) & HashMask;

        for (var n = 0; n <= length - MinMatch; n++)
        {
            InsH = ((InsH << HashShift) ^ (Window[n + (MinMatch - 1)] & 0xff)) & HashMask;
            Prev[n & WMask] = Head[InsH];
            Head[InsH] = (short)n;
        }

        return ZOk;
    }

    /// <summary>
    ///     Fill the window when the lookahead becomes insufficient.
    /// </summary>
    /// <summary>
    ///     Updates strstart and lookahead.
    /// </summary>
    /// <summary>
    /// </summary>
    /// <summary>
    ///     At least one byte has been read, or avail_in == 0; reads are
    /// </summary>
    /// <summary>
    ///     performed for at least two bytes (required for the zip translate_eol
    /// </summary>
    /// <summary>
    ///     option -- not supported here).
    /// </summary>
    internal void fill_window()
    {
        int n, m;
        int p;
        int more; // Amount of free space at the end of the window.

        do
        {
            more = WindowSize - Lookahead - Strstart;

            // Deal with !@#$% 64K limit:
            if (more == 0 && Strstart == 0 && Lookahead == 0)
            {
                more = WSize;
            }
            else if (more == -1)
            {
                // Very unlikely, but possible on 16 bit machine if strstart == 0
                // and lookahead == 1 (input done one byte at time)
                more--;

                // If the window is almost full and there is insufficient lookahead,
                // move the upper half to the lower one to make room in the upper half.
            }
            else if (Strstart >= WSize + WSize - MinLookahead)
            {
                Array.Copy(Window, WSize, Window, 0, WSize);
                MatchStart -= WSize;
                Strstart -= WSize; // we now have strstart >= MAX_DIST
                BlockStart -= WSize;

                // Slide the hash table (could be avoided with 32 bit values
                // at the expense of memory usage). We slide even when level == 0
                // to keep the hash table consistent if we switch back to level > 0
                // later. (Using level 0 permanently is not an optimal usage of
                // zlib, so we don't care about this pathological case.)

                n = HashSize;
                p = n;
                do
                {
                    m = Head[--p] & 0xffff;
                    Head[p] = (short)(m >= WSize ? m - WSize : 0);
                } while (--n != 0);

                n = WSize;
                p = n;
                do
                {
                    m = Prev[--p] & 0xffff;
                    Prev[p] = (short)(m >= WSize ? m - WSize : 0);
                    // If n is not on any hash chain, prev[n] is garbage but
                    // its value will never be used.
                } while (--n != 0);

                more += WSize;
            }

            if (Strm.AvailIn == 0)
            {
                return;
            }

            // If there was no sliding:
            //    strstart <= WSIZE+MAX_DIST-1 && lookahead <= MIN_LOOKAHEAD - 1 &&
            //    more == window_size - lookahead - strstart
            // => more >= window_size - (MIN_LOOKAHEAD-1 + WSIZE + MAX_DIST-1)
            // => more >= window_size - 2*WSIZE + 2
            // In the BIG_MEM or MMAP case (not yet supported),
            //   window_size == input_size + MIN_LOOKAHEAD  &&
            //   strstart + s->lookahead <= input_size => more >= MIN_LOOKAHEAD.
            // Otherwise, window_size == 2*WSIZE so more >= 2.
            // If there was sliding, more >= WSIZE. So in all cases, more >= 2.

            n = Strm.read_buf(Window, Strstart + Lookahead, more);
            Lookahead += n;

            // Initialize the hash value now that we have some input:
            if (Lookahead >= MinMatch)
            {
                InsH = Window[Strstart] & 0xff;
                InsH = ((InsH << HashShift) ^ (Window[Strstart + 1] & 0xff)) & HashMask;
            }
            // If the whole input has less than MIN_MATCH bytes, ins_h is garbage,
            // but this is not important since only literal bytes will be emitted.
        } while (Lookahead < MinLookahead && Strm.AvailIn != 0);
    }

    internal void flush_block_only(bool eof)
    {
        _tr_flush_block(BlockStart >= 0 ? BlockStart : -1,
                        Strstart - BlockStart,
                        eof);
        BlockStart = Strstart;
        Strm.flush_pending();
    }

    internal void init_block()
    {
        // Initialize the trees.
        for (var i = 0; i < LCodes; i++)
        {
            DynLtree[i * 2] = 0;
        }

        for (var i = 0; i < DCodes; i++)
        {
            DynDtree[i * 2] = 0;
        }

        for (var i = 0; i < BlCodes; i++)
        {
            BlTree[i * 2] = 0;
        }

        DynLtree[EndBlock * 2] = 1;
        OptLen = StaticLen = 0;
        LastLit = Matches = 0;
    }

    // index of pendig_buf
    internal void lm_init()
    {
        WindowSize = 2 * WSize;

        Head[HashSize - 1] = 0;
        for (var i = 0; i < HashSize - 1; i++)
        {
            Head[i] = 0;
        }

        // Set the default configuration parameters:
        MaxLazyMatch = _configTable[Level].MaxLazy;
        GoodMatch = _configTable[Level].GoodLength;
        NiceMatch = _configTable[Level].NiceLength;
        MaxChainLength = _configTable[Level].MaxChain;

        Strstart = 0;
        BlockStart = 0;
        Lookahead = 0;
        MatchLength = PrevLength = MinMatch - 1;
        MatchAvailable = 0;
        InsH = 0;
    }

    internal int longest_match(int curMatch)
    {
        var chainLength = MaxChainLength; // max hash chain length
        var scan = Strstart; // current string
        int match; // matched string
        int len; // length of current match
        var bestLen = PrevLength; // best match length so far
        var limit = Strstart > WSize - MinLookahead ? Strstart - (WSize - MinLookahead) : 0;
        var niceMatch = NiceMatch;

        // Stop when cur_match becomes <= limit. To simplify the code,
        // we prevent matches with the string of window index 0.

        var wmask = WMask;

        var strend = Strstart + MaxMatch;
        var scanEnd1 = Window[scan + bestLen - 1];
        var scanEnd = Window[scan + bestLen];

        // The code is optimized for HASH_BITS >= 8 and MAX_MATCH-2 multiple of 16.
        // It is easy to get rid of this optimization if necessary.

        // Do not waste too much time if we already have a good match:
        if (PrevLength >= GoodMatch)
        {
            chainLength >>= 2;
        }

        // Do not look for matches beyond the end of the input. This is necessary
        // to make deflate deterministic.
        if (niceMatch > Lookahead)
        {
            niceMatch = Lookahead;
        }

        do
        {
            match = curMatch;

            // Skip to next match if the match length cannot increase
            // or if the match length is less than 2:
            if (Window[match + bestLen] != scanEnd ||
                Window[match + bestLen - 1] != scanEnd1 ||
                Window[match] != Window[scan] ||
                Window[++match] != Window[scan + 1])
            {
                continue;
            }

            // The check at best_len-1 can be removed because it will be made
            // again later. (This heuristic is not always a win.)
            // It is not necessary to compare scan[2] and match[2] since they
            // are always equal when the other bytes match, given that
            // the hash keys are equal and that HASH_BITS >= 8.
            scan += 2;
            match++;

            // We check for insufficient lookahead only every 8th comparison;
            // the 256th check will be made at strstart+258.
            do
            {
            } while (Window[++scan] == Window[++match] &&
                     Window[++scan] == Window[++match] &&
                     Window[++scan] == Window[++match] &&
                     Window[++scan] == Window[++match] &&
                     Window[++scan] == Window[++match] &&
                     Window[++scan] == Window[++match] &&
                     Window[++scan] == Window[++match] &&
                     Window[++scan] == Window[++match] &&
                     scan < strend);

            len = MaxMatch - (strend - scan);
            scan = strend - MaxMatch;

            if (len > bestLen)
            {
                MatchStart = curMatch;
                bestLen = len;
                if (len >= niceMatch)
                {
                    break;
                }

                scanEnd1 = Window[scan + bestLen - 1];
                scanEnd = Window[scan + bestLen];
            }
        } while ((curMatch = Prev[curMatch & wmask] & 0xffff) > limit
                 && --chainLength != 0);

        if (bestLen <= Lookahead)
        {
            return bestLen;
        }

        return Lookahead;
    }

    /// <summary>
    ///     Restore the heap property by moving down the tree starting at node k,
    /// </summary>
    /// <summary>
    ///     exchanging a node with the smallest of its two sons if necessary, stopping
    /// </summary>
    /// <summary>
    ///     when the heap property is re-established (each father smaller than its
    /// </summary>
    /// <summary>
    ///     two sons).
    /// </summary>
    internal void Pqdownheap(short[] tree, // the tree to restore
                             int k // node to move down
    )
    {
        var v = Heap[k];
        var j = k << 1; // left son of k
        while (j <= HeapLen)
        {
            // Set j to the smallest of the two sons:
            if (j < HeapLen &&
                Smaller(tree, Heap[j + 1], Heap[j], Depth))
            {
                j++;
            }

            // Exit if v is smaller than both sons
            if (Smaller(tree, v, Heap[j], Depth))
            {
                break;
            }

            // Exchange v with the smallest son
            Heap[k] = Heap[j];
            k = j;
            // And continue down the tree, setting j to the left son of k
            j <<= 1;
        }

        Heap[k] = v;
    }

    /// <summary>
    ///     Output a byte on the stream.
    /// </summary>
    /// <summary>
    ///     IN assertion: there is enough room in pending_buf.
    /// </summary>
    internal void put_byte(byte[] p, int start, int len)
    {
        Array.Copy(p, start, PendingBuf, Pending, len);
        Pending += len;
    }

    internal void put_byte(byte c)
    {
        PendingBuf[Pending++] = c;
    }

    internal void put_short(int w)
    {
        PendingBuf[Pending++] = (byte)w /*&0xff*/;
        PendingBuf[Pending++] = (byte)(w >> 8);
    }

    internal void PutShortMsb(int b)
    {
        PendingBuf[Pending++] = (byte)(b >> 8);
        PendingBuf[Pending++] = (byte)b /*&0xff*/;
    }

    /// <summary>
    ///     Scan a literal or distance tree to determine the frequencies of the codes
    /// </summary>
    /// <summary>
    ///     in the bit length tree.
    /// </summary>
    internal void scan_tree(short[] tree, // the tree to be scanned
                            int maxCode // and its largest code of non zero frequency
    )
    {
        int n; // iterates over all tree elements
        var prevlen = -1; // last emitted length
        int curlen; // length of current code
        int nextlen = tree[0 * 2 + 1]; // length of next code
        var count = 0; // repeat count of the current code
        var maxCount = 7; // max repeat count
        var minCount = 4; // min repeat count

        if (nextlen == 0)
        {
            maxCount = 138;
            minCount = 3;
        }

        tree[(maxCode + 1) * 2 + 1] = -1; // guard

        for (n = 0; n <= maxCode; n++)
        {
            curlen = nextlen;
            nextlen = tree[(n + 1) * 2 + 1];
            if (++count < maxCount && curlen == nextlen)
            {
                continue;
            }

            if (count < minCount)
            {
                BlTree[curlen * 2] += (short)count;
            }
            else if (curlen != 0)
            {
                if (curlen != prevlen)
                {
                    BlTree[curlen * 2]++;
                }

                BlTree[Rep36 * 2]++;
            }
            else if (count <= 10)
            {
                BlTree[Repz310 * 2]++;
            }
            else
            {
                BlTree[Repz11138 * 2]++;
            }

            count = 0;
            prevlen = curlen;
            if (nextlen == 0)
            {
                maxCount = 138;
                minCount = 3;
            }
            else if (curlen == nextlen)
            {
                maxCount = 6;
                minCount = 3;
            }
            else
            {
                maxCount = 7;
                minCount = 4;
            }
        }
    }

    /// <summary>
    ///     Send the header for a block using dynamic Huffman trees: the counts, the
    /// </summary>
    /// <summary>
    ///     lengths of the bit length codes, the literal tree and the distance tree.
    /// </summary>
    /// <summary>
    ///     IN assertion: lcodes >= 257, dcodes >= 1, blcodes >= 4.
    /// </summary>
    internal void send_all_trees(int lcodes, int dcodes, int blcodes)
    {
        int rank; // index in bl_order

        send_bits(lcodes - 257, 5); // not +255 as stated in appnote.txt
        send_bits(dcodes - 1, 5);
        send_bits(blcodes - 4, 4); // not -3 as stated in appnote.txt
        for (rank = 0; rank < blcodes; rank++)
        {
            send_bits(BlTree[Tree.BlOrder[rank] * 2 + 1], 3);
        }

        send_tree(DynLtree, lcodes - 1); // literal tree
        send_tree(DynDtree, dcodes - 1); // distance tree
    }

    internal void send_bits(int val, int length)
    {
        if (BiValid > BufSize - length)
        {
            BiBuf |= (uint)(val << BiValid);
            PendingBuf[Pending++] = (byte)BiBuf /*&0xff*/;
            PendingBuf[Pending++] = (byte)(BiBuf >> 8);
            BiBuf = (uint)val >> (BufSize - BiValid);
            BiValid += length - BufSize;
        }
        else
        {
            BiBuf |= (uint)(val << BiValid);
            BiValid += length;
        }
        //            int len = length;
        //            if (bi_valid > (int)Buf_size - len) {
        //                int val = value;
        //                //      bi_buf |= (val << bi_valid);
        //                bi_buf = (short)((ushort)bi_buf | (ushort)((val << bi_valid)&0xffff));
        //                put_short(bi_buf);
        //                bi_buf = (short)(((uint)val) >> (Buf_size - bi_valid));
        //                bi_valid += len - Buf_size;
        //            } else {
        //                //      bi_buf |= (value) << bi_valid;
        //                bi_buf = (short)((ushort)bi_buf | (ushort)(((value) << bi_valid)&0xffff));
        //                bi_valid += len;
        //            }
    }

    internal void send_code(int c, short[] tree)
    {
        var c2 = c * 2;
        send_bits(tree[c2] & 0xffff, tree[c2 + 1] & 0xffff);
    }

    /// <summary>
    ///     Send a literal or distance tree in compressed form, using the codes in
    /// </summary>
    /// <summary>
    ///     bl_tree.
    /// </summary>
    internal void send_tree(short[] tree, // the tree to be sent
                            int maxCode // and its largest code of non zero frequency
    )
    {
        int n; // iterates over all tree elements
        var prevlen = -1; // last emitted length
        int curlen; // length of current code
        int nextlen = tree[0 * 2 + 1]; // length of next code
        var count = 0; // repeat count of the current code
        var maxCount = 7; // max repeat count
        var minCount = 4; // min repeat count

        if (nextlen == 0)
        {
            maxCount = 138;
            minCount = 3;
        }

        for (n = 0; n <= maxCode; n++)
        {
            curlen = nextlen;
            nextlen = tree[(n + 1) * 2 + 1];
            if (++count < maxCount && curlen == nextlen)
            {
                continue;
            }

            if (count < minCount)
            {
                do
                {
                    send_code(curlen, BlTree);
                } while (--count != 0);
            }
            else if (curlen != 0)
            {
                if (curlen != prevlen)
                {
                    send_code(curlen, BlTree);
                    count--;
                }

                send_code(Rep36, BlTree);
                send_bits(count - 3, 2);
            }
            else if (count <= 10)
            {
                send_code(Repz310, BlTree);
                send_bits(count - 3, 3);
            }
            else
            {
                send_code(Repz11138, BlTree);
                send_bits(count - 11, 7);
            }

            count = 0;
            prevlen = curlen;
            if (nextlen == 0)
            {
                maxCount = 138;
                minCount = 3;
            }
            else if (curlen == nextlen)
            {
                maxCount = 6;
                minCount = 3;
            }
            else
            {
                maxCount = 7;
                minCount = 4;
            }
        }
    }

    /// <summary>
    ///     frequencies does not exceed 64K (to fit in an int on 16 bit machines).
    /// </summary>
    internal void set_data_type()
    {
        var n = 0;
        var asciiFreq = 0;
        var binFreq = 0;
        while (n < 7)
        {
            binFreq += DynLtree[n * 2];
            n++;
        }

        while (n < 128)
        {
            asciiFreq += DynLtree[n * 2];
            n++;
        }

        while (n < Literals)
        {
            binFreq += DynLtree[n * 2];
            n++;
        }

        DataType = (byte)(binFreq > asciiFreq >> 2 ? ZBinary : ZAscii);
    }

    /// <summary>
    ///     Buffer for distances. To simplify the code, d_buf and l_buf have
    /// </summary>
    /// <summary>
    ///     the same number of elements. To use different lengths, an extra flag
    /// </summary>
    /// <summary>
    ///     array would be necessary.
    /// </summary>
    /// <summary>
    ///     Initialize the tree data structures for a new zlib stream.
    /// </summary>
    internal void tr_init()
    {
        LDesc.DynTree = DynLtree;
        LDesc.StatDesc = StaticTree.StaticLDesc;

        DDesc.DynTree = DynDtree;
        DDesc.StatDesc = StaticTree.StaticDDesc;

        BlDesc.DynTree = BlTree;
        BlDesc.StatDesc = StaticTree.StaticBlDesc;

        BiBuf = 0;
        BiValid = 0;
        LastEobLen = 8; // enough lookahead for inflate

        // Initialize the first block of the first file:
        init_block();
    }

    // 32K LZ77 window
    internal class Config
    {
        internal readonly int Func;
        internal readonly int GoodLength; // reduce lazy search above this match length
        internal readonly int MaxChain;
        internal readonly int MaxLazy; // do not perform lazy search above this match length
        internal readonly int NiceLength; // quit search above this match length

        internal Config(int goodLength, int maxLazy,
                        int niceLength, int maxChain, int func)
        {
            GoodLength = goodLength;
            MaxLazy = maxLazy;
            NiceLength = niceLength;
            MaxChain = maxChain;
            Func = func;
        }
    }
}