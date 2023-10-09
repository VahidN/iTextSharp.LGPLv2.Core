using System.util.collections;

namespace iTextSharp.text.pdf.codec;

/// <summary>
///     Class to read a JBIG2 file at a basic level: understand all the segments,
///     understand what segments belong to which pages, how many pages there are,
///     what the width and height of each page is, and global segments if there
///     are any.  Or: the minimum required to be able to take a normal sequential
///     or random-access organized file, and be able to embed JBIG2 pages as images
///     in a PDF.
///     TODO: the indeterminate-segment-size value of dataLength, else?
///     @since 2.1.5
/// </summary>
public class Jbig2SegmentReader
{
    public const int END_OF_FILE = 51;

    public const int END_OF_PAGE = 49;

    //see 7.4.9.
    public const int END_OF_STRIPE = 50;

    public const int EXTENSION = 62;
    public const int IMMEDIATE_GENERIC_REFINEMENT_REGION = 42;
    public const int IMMEDIATE_GENERIC_REGION = 38;

    public const int IMMEDIATE_HALFTONE_REGION = 22;

    //see 7.4.7.
    public const int IMMEDIATE_LOSSLESS_GENERIC_REFINEMENT_REGION = 43;

    //see 7.4.6.
    public const int IMMEDIATE_LOSSLESS_GENERIC_REGION = 39;

    //see 7.4.5.
    public const int IMMEDIATE_LOSSLESS_HALFTONE_REGION = 23;

    public const int IMMEDIATE_LOSSLESS_TEXT_REGION = 7;

    public const int IMMEDIATE_TEXT_REGION = 6;

    //see 7.4.6.
    public const int INTERMEDIATE_GENERIC_REFINEMENT_REGION = 40;

    //see 7.4.5.
    public const int INTERMEDIATE_GENERIC_REGION = 36;

    public const int INTERMEDIATE_HALFTONE_REGION = 20;
    public const int INTERMEDIATE_TEXT_REGION = 4;

    public const int PAGE_INFORMATION = 48;

    //see 7.4.3.
    //see 7.4.3.
    //see 7.4.3.
    public const int PATTERN_DICTIONARY = 16;

    //see 7.4.8.
    //see 7.4.10.
    //see 7.4.11.
    public const int PROFILES = 52;

    public const int SYMBOL_DICTIONARY = 0; //see 7.4.2.

    //see 7.4.4.
    //see 7.4.5.
    //see 7.4.6.
    //see 7.4.7.
    //see 7.4.7.

    //see 7.4.12.
    public const int TABLES = 53; //see 7.4.13.
    //see 7.4.14.

    private readonly OrderedTree _globals = new();
    private readonly OrderedTree _pages = new();
    private readonly RandomAccessFileOrArray _ra;
    private readonly OrderedTree _segments = new();
    private int _numberOfPages = -1;
    private bool _numberOfPagesKnown;
    private bool _read;
    private bool _sequential;


    public Jbig2SegmentReader(RandomAccessFileOrArray ra) => _ra = ra;

    public static byte[] CopyByteArray(byte[] b)
    {
        if (b == null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        var bc = new byte[b.Length];
        Array.Copy(b, 0, bc, 0, b.Length);
        return bc;
    }

    public byte[] GetGlobal(bool forEmbedding)
    {
        using var os = new MemoryStream();
        try
        {
            foreach (Jbig2Segment s in _globals.Keys)
            {
                if (forEmbedding &&
                    (s.Type == END_OF_FILE || s.Type == END_OF_PAGE))
                {
                    continue;
                }

                os.Write(s.HeaderData, 0, s.HeaderData.Length);
                os.Write(s.Data, 0, s.Data.Length);
            }
        }
        catch
        {
        }

        if (os.Length <= 0)
        {
            return null;
        }

        return os.ToArray();
    }

    public Jbig2Page GetPage(int page) => (Jbig2Page)_pages[page];

    public int GetPageHeight(int i) => ((Jbig2Page)_pages[i]).PageBitmapHeight;

    public int GetPageWidth(int i) => ((Jbig2Page)_pages[i]).PageBitmapWidth;

    public int NumberOfPages() => _pages.Count;

    public void Read()
    {
        if (_read)
        {
            throw new InvalidOperationException("already attempted a Read() on this Jbig2 File");
        }

        _read = true;

        readFileHeader();
        // Annex D
        if (_sequential)
        {
            // D.1
            do
            {
                var tmp = readHeader();
                readSegment(tmp);
                _segments[tmp.SegmentNumber] = tmp;
            } while (_ra.FilePointer < _ra.Length);
        }
        else
        {
            // D.2
            Jbig2Segment tmp;
            do
            {
                tmp = readHeader();
                _segments[tmp.SegmentNumber] = tmp;
            } while (tmp.Type != END_OF_FILE);

            foreach (int ss in _segments.Keys)
            {
                readSegment((Jbig2Segment)_segments[ss]);
            }
        }
    }

    public override string ToString()
    {
        if (_read)
        {
            return "Jbig2SegmentReader: number of pages: " + NumberOfPages();
        }

        return "Jbig2SegmentReader in indeterminate state.";
    }

    private void readFileHeader()
    {
        _ra.Seek(0);
        var idstring = new byte[8];
        _ra.Read(idstring);

        byte[] refidstring = { 0x97, 0x4A, 0x42, 0x32, 0x0D, 0x0A, 0x1A, 0x0A };

        for (var i = 0; i < idstring.Length; i++)
        {
            if (idstring[i] != refidstring[i])
            {
                throw new InvalidOperationException("file header idstring not good at byte " + i);
            }
        }

        var fileheaderflags = _ra.Read();

        _sequential = (fileheaderflags & 0x1) == 0x1;
        _numberOfPagesKnown = (fileheaderflags & 0x2) == 0x0;

        if ((fileheaderflags & 0xfc) != 0x0)
        {
            throw new InvalidOperationException("file header flags bits 2-7 not 0");
        }

        if (_numberOfPagesKnown)
        {
            _numberOfPages = _ra.ReadInt();
        }
    }

    private Jbig2Segment readHeader()
    {
        var ptr = _ra.FilePointer;
        // 7.2.1
        var segmentNumber = _ra.ReadInt();
        var s = new Jbig2Segment(segmentNumber);

        // 7.2.3
        var segmentHeaderFlags = _ra.Read();
        var deferredNonRetain = (segmentHeaderFlags & 0x80) == 0x80;
        s.DeferredNonRetain = deferredNonRetain;
        var pageAssociationSize = (segmentHeaderFlags & 0x40) == 0x40;
        var segmentType = segmentHeaderFlags & 0x3f;
        s.Type = segmentType;

        //7.2.4
        var referredToByte0 = _ra.Read();
        var countOfReferredToSegments = (referredToByte0 & 0xE0) >> 5;
        int[] referredToSegmentNumbers;
        bool[] segmentRetentionFlags = null;

        if (countOfReferredToSegments == 7)
        {
            // at least five bytes
            _ra.Seek(_ra.FilePointer - 1);
            countOfReferredToSegments = _ra.ReadInt() & 0x1fffffff;
            segmentRetentionFlags = new bool[countOfReferredToSegments + 1];
            var i = 0;
            var referredToCurrentByte = 0;
            do
            {
                var j = i % 8;
                if (j == 0)
                {
                    referredToCurrentByte = _ra.Read();
                }

                segmentRetentionFlags[i] = ((0x1 << j) & referredToCurrentByte) >> j == 0x1;
                i++;
            } while (i <= countOfReferredToSegments);
        }
        else if (countOfReferredToSegments <= 4)
        {
            // only one byte
            segmentRetentionFlags = new bool[countOfReferredToSegments + 1];
            referredToByte0 &= 0x1f;
            for (var i = 0; i <= countOfReferredToSegments; i++)
            {
                segmentRetentionFlags[i] = ((0x1 << i) & referredToByte0) >> i == 0x1;
            }
        }
        else if (countOfReferredToSegments == 5 || countOfReferredToSegments == 6)
        {
            throw new InvalidOperationException("count of referred-to segments had bad value in header for segment " +
                                                segmentNumber + " starting at " + ptr);
        }

        s.SegmentRetentionFlags = segmentRetentionFlags;
        s.CountOfReferredToSegments = countOfReferredToSegments;

        // 7.2.5
        referredToSegmentNumbers = new int[countOfReferredToSegments + 1];
        for (var i = 1; i <= countOfReferredToSegments; i++)
        {
            if (segmentNumber <= 256)
            {
                referredToSegmentNumbers[i] = _ra.Read();
            }
            else if (segmentNumber <= 65536)
            {
                referredToSegmentNumbers[i] = _ra.ReadUnsignedShort();
            }
            else
            {
                referredToSegmentNumbers[i] = (int)_ra.ReadUnsignedInt(); // TODO wtf ack
            }
        }

        s.ReferredToSegmentNumbers = referredToSegmentNumbers;

        // 7.2.6
        int segmentPageAssociation;
        var pageAssociationOffset = _ra.FilePointer - ptr;
        if (pageAssociationSize)
        {
            segmentPageAssociation = _ra.ReadInt();
        }
        else
        {
            segmentPageAssociation = _ra.Read();
        }

        if (segmentPageAssociation < 0)
        {
            throw new InvalidOperationException("page " + segmentPageAssociation + " invalid for segment " +
                                                segmentNumber + " starting at " + ptr);
        }

        s.Page = segmentPageAssociation;
        // so we can change the page association at embedding time.
        s.PageAssociationSize = pageAssociationSize;
        s.PageAssociationOffset = pageAssociationOffset;

        if (segmentPageAssociation > 0 && !_pages.ContainsKey(segmentPageAssociation))
        {
            _pages[segmentPageAssociation] = new Jbig2Page(segmentPageAssociation, this);
        }

        if (segmentPageAssociation > 0)
        {
            ((Jbig2Page)_pages[segmentPageAssociation]).AddSegment(s);
        }
        else
        {
            _globals[s] = null;
        }

        // 7.2.7
        var segmentDataLength = _ra.ReadUnsignedInt();
        // TODO the 0xffffffff value that might be here, and how to understand those afflicted segments
        s.DataLength = segmentDataLength;

        var endPtr = _ra.FilePointer;
        _ra.Seek(ptr);
        var headerData = new byte[endPtr - ptr];
        _ra.Read(headerData);
        s.HeaderData = headerData;

        return s;
    }

    private void readSegment(Jbig2Segment s)
    {
        var ptr = _ra.FilePointer;

        if (s.DataLength == 0xffffffffL)
        {
            // TODO figure this bit out, 7.2.7
            return;
        }

        var data = new byte[(int)s.DataLength];
        _ra.Read(data);
        s.Data = data;

        if (s.Type == PAGE_INFORMATION)
        {
            var last = _ra.FilePointer;
            _ra.Seek(ptr);
            var pageBitmapWidth = _ra.ReadInt();
            var pageBitmapHeight = _ra.ReadInt();
            _ra.Seek(last);
            var p = (Jbig2Page)_pages[s.Page];
            if (p == null)
            {
                throw new InvalidOperationException("referring to widht/height of page we havent seen yet? " + s.Page);
            }

            p.PageBitmapWidth = pageBitmapWidth;
            p.PageBitmapHeight = pageBitmapHeight;
        }
    }

    /// <summary>
    ///     Inner class that holds information about a JBIG2 page.
    ///     @since   2.1.5
    /// </summary>
    public class Jbig2Page
    {
        private readonly OrderedTree _segs = new();
        private readonly Jbig2SegmentReader _sr;
        public int Page;
        public int PageBitmapHeight = -1;
        public int PageBitmapWidth = -1;

        public Jbig2Page(int page, Jbig2SegmentReader sr)
        {
            Page = page;
            _sr = sr;
        }

        public void AddSegment(Jbig2Segment s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            _segs[s.SegmentNumber] = s;
        }

        /// <summary>
        ///     return as a single byte array the header-data for each segment in segment number
        ///     order, EMBEDDED organization, but i am putting the needed segments in SEQUENTIAL organization.
        ///     if for_embedding, skip the segment types that are known to be not for acrobat.
        ///     @throws IOException
        /// </summary>
        /// <param name="forEmbedding"></param>
        /// <returns>a byte array</returns>
        public byte[] GetData(bool forEmbedding)
        {
            using var os = new MemoryStream();
            foreach (int sn in _segs.Keys)
            {
                var s = (Jbig2Segment)_segs[sn];

                // pdf reference 1.4, section 3.3.6 JBIG2Decode Filter
                // D.3 Embedded organisation
                if (forEmbedding &&
                    (s.Type == END_OF_FILE || s.Type == END_OF_PAGE))
                {
                    continue;
                }

                if (forEmbedding)
                {
                    // change the page association to page 1
                    var headerDataEmb = CopyByteArray(s.HeaderData);
                    if (s.PageAssociationSize)
                    {
                        headerDataEmb[s.PageAssociationOffset] = 0x0;
                        headerDataEmb[s.PageAssociationOffset + 1] = 0x0;
                        headerDataEmb[s.PageAssociationOffset + 2] = 0x0;
                        headerDataEmb[s.PageAssociationOffset + 3] = 0x1;
                    }
                    else
                    {
                        headerDataEmb[s.PageAssociationOffset] = 0x1;
                    }

                    os.Write(headerDataEmb, 0, headerDataEmb.Length);
                }
                else
                {
                    os.Write(s.HeaderData, 0, s.HeaderData.Length);
                }

                os.Write(s.Data, 0, s.Data.Length);
            }

            return os.ToArray();
        }
    }

    /// <summary>
    ///     Inner class that holds information about a JBIG2 segment.
    ///     @since   2.1.5
    /// </summary>
    public class Jbig2Segment : IComparable
    {
        public int CountOfReferredToSegments = -1;
        public byte[] Data;
        public long DataLength = -1;
        public bool DeferredNonRetain;
        public byte[] HeaderData;
        public int Page = -1;
        public int PageAssociationOffset = -1;
        public bool PageAssociationSize;
        public int[] ReferredToSegmentNumbers;
        public int SegmentNumber;
        public bool[] SegmentRetentionFlags;
        public int Type = -1;

        public Jbig2Segment(int segment_number) => SegmentNumber = segment_number;

        /// <summary>
        ///     for the globals treeset
        /// </summary>
        public int CompareTo(object obj) => CompareTo((Jbig2Segment)obj);

        public int CompareTo(Jbig2Segment s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return SegmentNumber - s.SegmentNumber;
        }
    }
}