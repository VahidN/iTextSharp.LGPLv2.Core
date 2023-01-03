namespace iTextSharp.text.pdf.codec;

/// <summary>
///     Encodes data in the CCITT G4 FAX format.
/// </summary>
public class Ccittg4Encoder
{
    private const int Code = 1;
    private const int Eol = 0x001;
    private const int G3CodeEof = -3;

    /// <summary>
    ///     status values returned instead of a run length
    /// </summary>
    private const int G3CodeEol = -1;

    private const int G3CodeIncomp = -4;
    private const int G3CodeInvalid = -2;
    private const int Length = 0;
    private const int Runlen = 2;

    private static readonly byte[] _oneruns =
    {
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x00 - 0x0f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x10 - 0x1f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x20 - 0x2f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x30 - 0x3f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x40 - 0x4f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x50 - 0x5f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x60 - 0x6f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x70 - 0x7f */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 0x80 - 0x8f */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 0x90 - 0x9f */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 0xa0 - 0xaf */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 0xb0 - 0xbf */
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, /* 0xc0 - 0xcf */
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, /* 0xd0 - 0xdf */
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, /* 0xe0 - 0xef */
        4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8, /* 0xf0 - 0xff */
    };

    private static readonly byte[] _zeroruns =
    {
        8, 7, 6, 6, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4, /* 0x00 - 0x0f */
        3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, /* 0x10 - 0x1f */
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, /* 0x20 - 0x2f */
        2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, /* 0x30 - 0x3f */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 0x40 - 0x4f */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 0x50 - 0x5f */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 0x60 - 0x6f */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 0x70 - 0x7f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x80 - 0x8f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0x90 - 0x9f */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0xa0 - 0xaf */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0xb0 - 0xbf */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0xc0 - 0xcf */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0xd0 - 0xdf */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0xe0 - 0xef */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 0xf0 - 0xff */
    };

    private readonly int[] _horizcode =
        { 3, 0x1, 0 };

    private readonly int[] _msbmask =
        { 0x00, 0x01, 0x03, 0x07, 0x0f, 0x1f, 0x3f, 0x7f, 0xff };

    private readonly ByteBuffer _outBuf = new(1024);

    private readonly int[] _passcode =
        { 4, 0x1, 0 };

    private readonly byte[] _refline;
    private readonly int _rowbytes;
    private readonly int _rowpixels;

    private readonly int[][] _tiffFaxBlackCodes =
    {
        new[] { 10, 0x37, 0 }, /* 0000 1101 11 */
        new[] { 3, 0x2, 1 }, /* 010 */
        new[] { 2, 0x3, 2 }, /* 11 */
        new[] { 2, 0x2, 3 }, /* 10 */
        new[] { 3, 0x3, 4 }, /* 011 */
        new[] { 4, 0x3, 5 }, /* 0011 */
        new[] { 4, 0x2, 6 }, /* 0010 */
        new[] { 5, 0x3, 7 }, /* 0001 1 */
        new[] { 6, 0x5, 8 }, /* 0001 01 */
        new[] { 6, 0x4, 9 }, /* 0001 00 */
        new[] { 7, 0x4, 10 }, /* 0000 100 */
        new[] { 7, 0x5, 11 }, /* 0000 101 */
        new[] { 7, 0x7, 12 }, /* 0000 111 */
        new[] { 8, 0x4, 13 }, /* 0000 0100 */
        new[] { 8, 0x7, 14 }, /* 0000 0111 */
        new[] { 9, 0x18, 15 }, /* 0000 1100 0 */
        new[] { 10, 0x17, 16 }, /* 0000 0101 11 */
        new[] { 10, 0x18, 17 }, /* 0000 0110 00 */
        new[] { 10, 0x8, 18 }, /* 0000 0010 00 */
        new[] { 11, 0x67, 19 }, /* 0000 1100 111 */
        new[] { 11, 0x68, 20 }, /* 0000 1101 000 */
        new[] { 11, 0x6C, 21 }, /* 0000 1101 100 */
        new[] { 11, 0x37, 22 }, /* 0000 0110 111 */
        new[] { 11, 0x28, 23 }, /* 0000 0101 000 */
        new[] { 11, 0x17, 24 }, /* 0000 0010 111 */
        new[] { 11, 0x18, 25 }, /* 0000 0011 000 */
        new[] { 12, 0xCA, 26 }, /* 0000 1100 1010 */
        new[] { 12, 0xCB, 27 }, /* 0000 1100 1011 */
        new[] { 12, 0xCC, 28 }, /* 0000 1100 1100 */
        new[] { 12, 0xCD, 29 }, /* 0000 1100 1101 */
        new[] { 12, 0x68, 30 }, /* 0000 0110 1000 */
        new[] { 12, 0x69, 31 }, /* 0000 0110 1001 */
        new[] { 12, 0x6A, 32 }, /* 0000 0110 1010 */
        new[] { 12, 0x6B, 33 }, /* 0000 0110 1011 */
        new[] { 12, 0xD2, 34 }, /* 0000 1101 0010 */
        new[] { 12, 0xD3, 35 }, /* 0000 1101 0011 */
        new[] { 12, 0xD4, 36 }, /* 0000 1101 0100 */
        new[] { 12, 0xD5, 37 }, /* 0000 1101 0101 */
        new[] { 12, 0xD6, 38 }, /* 0000 1101 0110 */
        new[] { 12, 0xD7, 39 }, /* 0000 1101 0111 */
        new[] { 12, 0x6C, 40 }, /* 0000 0110 1100 */
        new[] { 12, 0x6D, 41 }, /* 0000 0110 1101 */
        new[] { 12, 0xDA, 42 }, /* 0000 1101 1010 */
        new[] { 12, 0xDB, 43 }, /* 0000 1101 1011 */
        new[] { 12, 0x54, 44 }, /* 0000 0101 0100 */
        new[] { 12, 0x55, 45 }, /* 0000 0101 0101 */
        new[] { 12, 0x56, 46 }, /* 0000 0101 0110 */
        new[] { 12, 0x57, 47 }, /* 0000 0101 0111 */
        new[] { 12, 0x64, 48 }, /* 0000 0110 0100 */
        new[] { 12, 0x65, 49 }, /* 0000 0110 0101 */
        new[] { 12, 0x52, 50 }, /* 0000 0101 0010 */
        new[] { 12, 0x53, 51 }, /* 0000 0101 0011 */
        new[] { 12, 0x24, 52 }, /* 0000 0010 0100 */
        new[] { 12, 0x37, 53 }, /* 0000 0011 0111 */
        new[] { 12, 0x38, 54 }, /* 0000 0011 1000 */
        new[] { 12, 0x27, 55 }, /* 0000 0010 0111 */
        new[] { 12, 0x28, 56 }, /* 0000 0010 1000 */
        new[] { 12, 0x58, 57 }, /* 0000 0101 1000 */
        new[] { 12, 0x59, 58 }, /* 0000 0101 1001 */
        new[] { 12, 0x2B, 59 }, /* 0000 0010 1011 */
        new[] { 12, 0x2C, 60 }, /* 0000 0010 1100 */
        new[] { 12, 0x5A, 61 }, /* 0000 0101 1010 */
        new[] { 12, 0x66, 62 }, /* 0000 0110 0110 */
        new[] { 12, 0x67, 63 }, /* 0000 0110 0111 */
        new[] { 10, 0xF, 64 }, /* 0000 0011 11 */
        new[] { 12, 0xC8, 128 }, /* 0000 1100 1000 */
        new[] { 12, 0xC9, 192 }, /* 0000 1100 1001 */
        new[] { 12, 0x5B, 256 }, /* 0000 0101 1011 */
        new[] { 12, 0x33, 320 }, /* 0000 0011 0011 */
        new[] { 12, 0x34, 384 }, /* 0000 0011 0100 */
        new[] { 12, 0x35, 448 }, /* 0000 0011 0101 */
        new[] { 13, 0x6C, 512 }, /* 0000 0011 0110 0 */
        new[] { 13, 0x6D, 576 }, /* 0000 0011 0110 1 */
        new[] { 13, 0x4A, 640 }, /* 0000 0010 0101 0 */
        new[] { 13, 0x4B, 704 }, /* 0000 0010 0101 1 */
        new[] { 13, 0x4C, 768 }, /* 0000 0010 0110 0 */
        new[] { 13, 0x4D, 832 }, /* 0000 0010 0110 1 */
        new[] { 13, 0x72, 896 }, /* 0000 0011 1001 0 */
        new[] { 13, 0x73, 960 }, /* 0000 0011 1001 1 */
        new[] { 13, 0x74, 1024 }, /* 0000 0011 1010 0 */
        new[] { 13, 0x75, 1088 }, /* 0000 0011 1010 1 */
        new[] { 13, 0x76, 1152 }, /* 0000 0011 1011 0 */
        new[] { 13, 0x77, 1216 }, /* 0000 0011 1011 1 */
        new[] { 13, 0x52, 1280 }, /* 0000 0010 1001 0 */
        new[] { 13, 0x53, 1344 }, /* 0000 0010 1001 1 */
        new[] { 13, 0x54, 1408 }, /* 0000 0010 1010 0 */
        new[] { 13, 0x55, 1472 }, /* 0000 0010 1010 1 */
        new[] { 13, 0x5A, 1536 }, /* 0000 0010 1101 0 */
        new[] { 13, 0x5B, 1600 }, /* 0000 0010 1101 1 */
        new[] { 13, 0x64, 1664 }, /* 0000 0011 0010 0 */
        new[] { 13, 0x65, 1728 }, /* 0000 0011 0010 1 */
        new[] { 11, 0x8, 1792 }, /* 0000 0001 000 */
        new[] { 11, 0xC, 1856 }, /* 0000 0001 100 */
        new[] { 11, 0xD, 1920 }, /* 0000 0001 101 */
        new[] { 12, 0x12, 1984 }, /* 0000 0001 0010 */
        new[] { 12, 0x13, 2048 }, /* 0000 0001 0011 */
        new[] { 12, 0x14, 2112 }, /* 0000 0001 0100 */
        new[] { 12, 0x15, 2176 }, /* 0000 0001 0101 */
        new[] { 12, 0x16, 2240 }, /* 0000 0001 0110 */
        new[] { 12, 0x17, 2304 }, /* 0000 0001 0111 */
        new[] { 12, 0x1C, 2368 }, /* 0000 0001 1100 */
        new[] { 12, 0x1D, 2432 }, /* 0000 0001 1101 */
        new[] { 12, 0x1E, 2496 }, /* 0000 0001 1110 */
        new[] { 12, 0x1F, 2560 }, /* 0000 0001 1111 */
        new[] { 12, 0x1, G3CodeEol }, /* 0000 0000 0001 */
        new[] { 9, 0x1, G3CodeInvalid }, /* 0000 0000 1 */
        new[] { 10, 0x1, G3CodeInvalid }, /* 0000 0000 01 */
        new[] { 11, 0x1, G3CodeInvalid }, /* 0000 0000 001 */
        new[] { 12, 0x0, G3CodeInvalid }, /* 0000 0000 0000 */
    };

    private readonly int[][] _tiffFaxWhiteCodes =
    {
        new[] { 8, 0x35, 0 }, /* 0011 0101 */
        new[] { 6, 0x7, 1 }, /* 0001 11 */
        new[] { 4, 0x7, 2 }, /* 0111 */
        new[] { 4, 0x8, 3 }, /* 1000 */
        new[] { 4, 0xB, 4 }, /* 1011 */
        new[] { 4, 0xC, 5 }, /* 1100 */
        new[] { 4, 0xE, 6 }, /* 1110 */
        new[] { 4, 0xF, 7 }, /* 1111 */
        new[] { 5, 0x13, 8 }, /* 1001 1 */
        new[] { 5, 0x14, 9 }, /* 1010 0 */
        new[] { 5, 0x7, 10 }, /* 0011 1 */
        new[] { 5, 0x8, 11 }, /* 0100 0 */
        new[] { 6, 0x8, 12 }, /* 0010 00 */
        new[] { 6, 0x3, 13 }, /* 0000 11 */
        new[] { 6, 0x34, 14 }, /* 1101 00 */
        new[] { 6, 0x35, 15 }, /* 1101 01 */
        new[] { 6, 0x2A, 16 }, /* 1010 10 */
        new[] { 6, 0x2B, 17 }, /* 1010 11 */
        new[] { 7, 0x27, 18 }, /* 0100 111 */
        new[] { 7, 0xC, 19 }, /* 0001 100 */
        new[] { 7, 0x8, 20 }, /* 0001 000 */
        new[] { 7, 0x17, 21 }, /* 0010 111 */
        new[] { 7, 0x3, 22 }, /* 0000 011 */
        new[] { 7, 0x4, 23 }, /* 0000 100 */
        new[] { 7, 0x28, 24 }, /* 0101 000 */
        new[] { 7, 0x2B, 25 }, /* 0101 011 */
        new[] { 7, 0x13, 26 }, /* 0010 011 */
        new[] { 7, 0x24, 27 }, /* 0100 100 */
        new[] { 7, 0x18, 28 }, /* 0011 000 */
        new[] { 8, 0x2, 29 }, /* 0000 0010 */
        new[] { 8, 0x3, 30 }, /* 0000 0011 */
        new[] { 8, 0x1A, 31 }, /* 0001 1010 */
        new[] { 8, 0x1B, 32 }, /* 0001 1011 */
        new[] { 8, 0x12, 33 }, /* 0001 0010 */
        new[] { 8, 0x13, 34 }, /* 0001 0011 */
        new[] { 8, 0x14, 35 }, /* 0001 0100 */
        new[] { 8, 0x15, 36 }, /* 0001 0101 */
        new[] { 8, 0x16, 37 }, /* 0001 0110 */
        new[] { 8, 0x17, 38 }, /* 0001 0111 */
        new[] { 8, 0x28, 39 }, /* 0010 1000 */
        new[] { 8, 0x29, 40 }, /* 0010 1001 */
        new[] { 8, 0x2A, 41 }, /* 0010 1010 */
        new[] { 8, 0x2B, 42 }, /* 0010 1011 */
        new[] { 8, 0x2C, 43 }, /* 0010 1100 */
        new[] { 8, 0x2D, 44 }, /* 0010 1101 */
        new[] { 8, 0x4, 45 }, /* 0000 0100 */
        new[] { 8, 0x5, 46 }, /* 0000 0101 */
        new[] { 8, 0xA, 47 }, /* 0000 1010 */
        new[] { 8, 0xB, 48 }, /* 0000 1011 */
        new[] { 8, 0x52, 49 }, /* 0101 0010 */
        new[] { 8, 0x53, 50 }, /* 0101 0011 */
        new[] { 8, 0x54, 51 }, /* 0101 0100 */
        new[] { 8, 0x55, 52 }, /* 0101 0101 */
        new[] { 8, 0x24, 53 }, /* 0010 0100 */
        new[] { 8, 0x25, 54 }, /* 0010 0101 */
        new[] { 8, 0x58, 55 }, /* 0101 1000 */
        new[] { 8, 0x59, 56 }, /* 0101 1001 */
        new[] { 8, 0x5A, 57 }, /* 0101 1010 */
        new[] { 8, 0x5B, 58 }, /* 0101 1011 */
        new[] { 8, 0x4A, 59 }, /* 0100 1010 */
        new[] { 8, 0x4B, 60 }, /* 0100 1011 */
        new[] { 8, 0x32, 61 }, /* 0011 0010 */
        new[] { 8, 0x33, 62 }, /* 0011 0011 */
        new[] { 8, 0x34, 63 }, /* 0011 0100 */
        new[] { 5, 0x1B, 64 }, /* 1101 1 */
        new[] { 5, 0x12, 128 }, /* 1001 0 */
        new[] { 6, 0x17, 192 }, /* 0101 11 */
        new[] { 7, 0x37, 256 }, /* 0110 111 */
        new[] { 8, 0x36, 320 }, /* 0011 0110 */
        new[] { 8, 0x37, 384 }, /* 0011 0111 */
        new[] { 8, 0x64, 448 }, /* 0110 0100 */
        new[] { 8, 0x65, 512 }, /* 0110 0101 */
        new[] { 8, 0x68, 576 }, /* 0110 1000 */
        new[] { 8, 0x67, 640 }, /* 0110 0111 */
        new[] { 9, 0xCC, 704 }, /* 0110 0110 0 */
        new[] { 9, 0xCD, 768 }, /* 0110 0110 1 */
        new[] { 9, 0xD2, 832 }, /* 0110 1001 0 */
        new[] { 9, 0xD3, 896 }, /* 0110 1001 1 */
        new[] { 9, 0xD4, 960 }, /* 0110 1010 0 */
        new[] { 9, 0xD5, 1024 }, /* 0110 1010 1 */
        new[] { 9, 0xD6, 1088 }, /* 0110 1011 0 */
        new[] { 9, 0xD7, 1152 }, /* 0110 1011 1 */
        new[] { 9, 0xD8, 1216 }, /* 0110 1100 0 */
        new[] { 9, 0xD9, 1280 }, /* 0110 1100 1 */
        new[] { 9, 0xDA, 1344 }, /* 0110 1101 0 */
        new[] { 9, 0xDB, 1408 }, /* 0110 1101 1 */
        new[] { 9, 0x98, 1472 }, /* 0100 1100 0 */
        new[] { 9, 0x99, 1536 }, /* 0100 1100 1 */
        new[] { 9, 0x9A, 1600 }, /* 0100 1101 0 */
        new[] { 6, 0x18, 1664 }, /* 0110 00 */
        new[] { 9, 0x9B, 1728 }, /* 0100 1101 1 */
        new[] { 11, 0x8, 1792 }, /* 0000 0001 000 */
        new[] { 11, 0xC, 1856 }, /* 0000 0001 100 */
        new[] { 11, 0xD, 1920 }, /* 0000 0001 101 */
        new[] { 12, 0x12, 1984 }, /* 0000 0001 0010 */
        new[] { 12, 0x13, 2048 }, /* 0000 0001 0011 */
        new[] { 12, 0x14, 2112 }, /* 0000 0001 0100 */
        new[] { 12, 0x15, 2176 }, /* 0000 0001 0101 */
        new[] { 12, 0x16, 2240 }, /* 0000 0001 0110 */
        new[] { 12, 0x17, 2304 }, /* 0000 0001 0111 */
        new[] { 12, 0x1C, 2368 }, /* 0000 0001 1100 */
        new[] { 12, 0x1D, 2432 }, /* 0000 0001 1101 */
        new[] { 12, 0x1E, 2496 }, /* 0000 0001 1110 */
        new[] { 12, 0x1F, 2560 }, /* 0000 0001 1111 */
        new[] { 12, 0x1, G3CodeEol }, /* 0000 0000 0001 */
        new[] { 9, 0x1, G3CodeInvalid }, /* 0000 0000 1 */
        new[] { 10, 0x1, G3CodeInvalid }, /* 0000 0000 01 */
        new[] { 11, 0x1, G3CodeInvalid }, /* 0000 0000 001 */
        new[] { 12, 0x0, G3CodeInvalid }, /* 0000 0000 0000 */
    };

    private readonly int[][] _vcodes =
    {
        new[] { 7, 0x03, 0 }, /* 0000 011 */
        new[] { 6, 0x03, 0 }, /* 0000 11 */
        new[] { 3, 0x03, 0 }, /* 011 */
        new[] { 1, 0x1, 0 }, /* 1 */
        new[] { 3, 0x2, 0 }, /* 010 */
        new[] { 6, 0x02, 0 }, /* 0000 10 */
        new[] { 7, 0x02, 0 }, /* 0000 010 */
    };

    private int _bit = 8;
    private int _data;
    private byte[] _dataBp;
    private int _offsetData;
    private int _sizeData;

    /// <summary>
    ///     Creates a new encoder.
    /// </summary>
    /// <param name="width">the line width</param>
    public Ccittg4Encoder(int width)
    {
        _rowpixels = width;
        _rowbytes = (_rowpixels + 7) / 8;
        _refline = new byte[_rowbytes];
    }

    /// <summary>
    ///     Encodes a full image.
    /// </summary>
    /// <param name="data">the data to encode</param>
    /// <param name="width">the image width</param>
    /// <param name="height">the image height</param>
    /// <returns>the encoded image</returns>
    public static byte[] Compress(byte[] data, int width, int height)
    {
        var g4 = new Ccittg4Encoder(width);
        g4.Fax4Encode(data, 0, g4._rowbytes * height);
        return g4.Close();
    }

    /// <summary>
    ///     Closes the encoder and returns the encoded data.
    /// </summary>
    /// <returns>the encoded data</returns>
    public byte[] Close()
    {
        fax4PostEncode();
        return _outBuf.ToByteArray();
    }

    /// <summary>
    ///     Encodes a number of lines.
    /// </summary>
    /// <param name="data">the data to be encoded</param>
    /// <param name="offset">the offset into the data</param>
    /// <param name="size">the size of the data to be encoded</param>
    public void Fax4Encode(byte[] data, int offset, int size)
    {
        _dataBp = data;
        _offsetData = offset;
        _sizeData = size;
        while (_sizeData > 0)
        {
            fax3Encode2DRow();
            Array.Copy(_dataBp, _offsetData, _refline, 0, _rowbytes);
            _offsetData += _rowbytes;
            _sizeData -= _rowbytes;
        }
    }

    /// <summary>
    ///     Encodes a number of lines.
    /// </summary>
    /// <param name="data">the data to be encoded</param>
    /// <param name="height">the number of lines to encode</param>
    public void Fax4Encode(byte[] data, int height)
    {
        Fax4Encode(data, 0, _rowbytes * height);
    }

    private static int find0Span(byte[] bp, int offset, int bs, int be)
    {
        var bits = be - bs;
        int n, span;

        var pos = offset + (bs >> 3);
        /*
        * Check partial byte on lhs.
        */
        if (bits > 0 && (n = bs & 7) != 0)
        {
            span = _zeroruns[(bp[pos] << n) & 0xff];
            if (span > 8 - n) /* table value too generous */
            {
                span = 8 - n;
            }

            if (span > bits) /* constrain span to bit range */
            {
                span = bits;
            }

            if (n + span < 8) /* doesn't extend to edge of byte */
            {
                return span;
            }

            bits -= span;
            pos++;
        }
        else
        {
            span = 0;
        }

        /*
        * Scan full bytes for all 1's.
        */
        while (bits >= 8)
        {
            if (bp[pos] != 0) /* end of run */
            {
                return span + _zeroruns[bp[pos] & 0xff];
            }

            span += 8;
            bits -= 8;
            pos++;
        }

        /*
        * Check partial byte on rhs.
        */
        if (bits > 0)
        {
            n = _zeroruns[bp[pos] & 0xff];
            span += n > bits ? bits : n;
        }

        return span;
    }

    private static int find1Span(byte[] bp, int offset, int bs, int be)
    {
        var bits = be - bs;
        int n, span;

        var pos = offset + (bs >> 3);
        /*
        * Check partial byte on lhs.
        */
        if (bits > 0 && (n = bs & 7) != 0)
        {
            span = _oneruns[(bp[pos] << n) & 0xff];
            if (span > 8 - n) /* table value too generous */
            {
                span = 8 - n;
            }

            if (span > bits) /* constrain span to bit range */
            {
                span = bits;
            }

            if (n + span < 8) /* doesn't extend to edge of byte */
            {
                return span;
            }

            bits -= span;
            pos++;
        }
        else
        {
            span = 0;
        }

        /*
        * Scan full bytes for all 1's.
        */
        while (bits >= 8)
        {
            if (bp[pos] != 0xff) /* end of run */
            {
                return span + _oneruns[bp[pos] & 0xff];
            }

            span += 8;
            bits -= 8;
            pos++;
        }

        /*
        * Check partial byte on rhs.
        */
        if (bits > 0)
        {
            n = _oneruns[bp[pos] & 0xff];
            span += n > bits ? bits : n;
        }

        return span;
    }

    private static int finddiff(byte[] bp, int offset, int bs, int be, int color) =>
        bs + (color != 0 ? find1Span(bp, offset, bs, be) : find0Span(bp, offset, bs, be));

    private static int finddiff2(byte[] bp, int offset, int bs, int be, int color) =>
        bs < be ? finddiff(bp, offset, bs, be, color) : be;

    private void fax3Encode2DRow()
    {
        var a0 = 0;
        var a1 = pixel(_dataBp, _offsetData, 0) != 0 ? 0 : finddiff(_dataBp, _offsetData, 0, _rowpixels, 0);
        var b1 = pixel(_refline, 0, 0) != 0 ? 0 : finddiff(_refline, 0, 0, _rowpixels, 0);
        int a2, b2;

        for (;;)
        {
            b2 = finddiff2(_refline, 0, b1, _rowpixels, pixel(_refline, 0, b1));
            if (b2 >= a1)
            {
                var d = b1 - a1;
                if (!(-3 <= d && d <= 3))
                {
                    /* horizontal mode */
                    a2 = finddiff2(_dataBp, _offsetData, a1, _rowpixels, pixel(_dataBp, _offsetData, a1));
                    putcode(_horizcode);
                    if (a0 + a1 == 0 || pixel(_dataBp, _offsetData, a0) == 0)
                    {
                        putspan(a1 - a0, _tiffFaxWhiteCodes);
                        putspan(a2 - a1, _tiffFaxBlackCodes);
                    }
                    else
                    {
                        putspan(a1 - a0, _tiffFaxBlackCodes);
                        putspan(a2 - a1, _tiffFaxWhiteCodes);
                    }

                    a0 = a2;
                }
                else
                {
                    /* vertical mode */
                    putcode(_vcodes[d + 3]);
                    a0 = a1;
                }
            }
            else
            {
                /* pass mode */
                putcode(_passcode);
                a0 = b2;
            }

            if (a0 >= _rowpixels)
            {
                break;
            }

            a1 = finddiff(_dataBp, _offsetData, a0, _rowpixels, pixel(_dataBp, _offsetData, a0));
            b1 = finddiff(_refline, 0, a0, _rowpixels, pixel(_dataBp, _offsetData, a0) ^ 1);
            b1 = finddiff(_refline, 0, b1, _rowpixels, pixel(_dataBp, _offsetData, a0));
        }
    }

    private void fax4PostEncode()
    {
        putBits(Eol, 12);
        putBits(Eol, 12);
        if (_bit != 8)
        {
            _outBuf.Append((byte)_data);
            _data = 0;
            _bit = 8;
        }
    }

    private int pixel(byte[] data, int offset, int bit)
    {
        if (bit >= _rowpixels)
        {
            return 0;
        }

        return ((data[offset + (bit >> 3)] & 0xff) >> (7 - (bit & 7))) & 1;
    }

    private void putBits(int bits, int length)
    {
        while (length > _bit)
        {
            _data |= bits >> (length - _bit);
            length -= _bit;
            _outBuf.Append((byte)_data);
            _data = 0;
            _bit = 8;
        }

        _data |= (bits & _msbmask[length]) << (_bit - length);
        _bit -= length;
        if (_bit == 0)
        {
            _outBuf.Append((byte)_data);
            _data = 0;
            _bit = 8;
        }
    }

    private void putcode(int[] table)
    {
        putBits(table[Code], table[Length]);
    }

    private void putspan(int span, int[][] tab)
    {
        int code, length;

        while (span >= 2624)
        {
            var te = tab[63 + (2560 >> 6)];
            code = te[Code];
            length = te[Length];
            putBits(code, length);
            span -= te[Runlen];
        }

        if (span >= 64)
        {
            var te = tab[63 + (span >> 6)];
            code = te[Code];
            length = te[Length];
            putBits(code, length);
            span -= te[Runlen];
        }

        code = tab[span][Code];
        length = tab[span][Length];
        putBits(code, length);
    }
    /* bit length of g3 code */
    /* g3 code */
    /* run length in bits */

    /* EOL code value - 0000 0000 0000 1 */

    /* NB: ACT_EOL - ACT_WRUNT */
    /* NB: ACT_INVALID - ACT_WRUNT */
    /* end of input data */
    /* incomplete run code */
    /* 001 */
    /* 0001 */
}