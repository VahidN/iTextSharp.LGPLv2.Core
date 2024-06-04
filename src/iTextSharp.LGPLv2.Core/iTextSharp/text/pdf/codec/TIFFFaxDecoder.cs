namespace iTextSharp.text.pdf.codec;

public class TiffFaxDecoder
{
    /// <summary>
    ///     Table to be used when fillOrder = 2, for flipping bytes.
    /// </summary>
    internal static readonly byte[] FlipTable =
    {
        0, 256 - 128, 64, 256 - 64, 32, 256 - 96, 96, 256 - 32,
        16, 256 - 112, 80, 256 - 48, 48, 256 - 80, 112, 256 - 16,
        8, 256 - 120, 72, 256 - 56, 40, 256 - 88, 104, 256 - 24,
        24, 256 - 104, 88, 256 - 40, 56, 256 - 72, 120, 256 - 8,
        4, 256 - 124, 68, 256 - 60, 36, 256 - 92, 100, 256 - 28,
        20, 256 - 108, 84, 256 - 44, 52, 256 - 76, 116, 256 - 12,
        12, 256 - 116, 76, 256 - 52, 44, 256 - 84, 108, 256 - 20,
        28, 256 - 100, 92, 256 - 36, 60, 256 - 68, 124, 256 - 4,
        2, 256 - 126, 66, 256 - 62, 34, 256 - 94, 98, 256 - 30,
        18, 256 - 110, 82, 256 - 46, 50, 256 - 78, 114, 256 - 14,
        10, 256 - 118, 74, 256 - 54, 42, 256 - 86, 106, 256 - 22,
        26, 256 - 102, 90, 256 - 38, 58, 256 - 70, 122, 256 - 6,
        6, 256 - 122, 70, 256 - 58, 38, 256 - 90, 102, 256 - 26,
        22, 256 - 106, 86, 256 - 42, 54, 256 - 74, 118, 256 - 10,
        14, 256 - 114, 78, 256 - 50, 46, 256 - 82, 110, 256 - 18,
        30, 256 - 98, 94, 256 - 34, 62, 256 - 66, 126, 256 - 2,
        1, 256 - 127, 65, 256 - 63, 33, 256 - 95, 97, 256 - 31,
        17, 256 - 111, 81, 256 - 47, 49, 256 - 79, 113, 256 - 15,
        9, 256 - 119, 73, 256 - 55, 41, 256 - 87, 105, 256 - 23,
        25, 256 - 103, 89, 256 - 39, 57, 256 - 71, 121, 256 - 7,
        5, 256 - 123, 69, 256 - 59, 37, 256 - 91, 101, 256 - 27,
        21, 256 - 107, 85, 256 - 43, 53, 256 - 75, 117, 256 - 11,
        13, 256 - 115, 77, 256 - 51, 45, 256 - 83, 109, 256 - 19,
        29, 256 - 99, 93, 256 - 35, 61, 256 - 67, 125, 256 - 3,
        3, 256 - 125, 67, 256 - 61, 35, 256 - 93, 99, 256 - 29,
        19, 256 - 109, 83, 256 - 45, 51, 256 - 77, 115, 256 - 13,
        11, 256 - 117, 75, 256 - 53, 43, 256 - 85, 107, 256 - 21,
        27, 256 - 101, 91, 256 - 37, 59, 256 - 69, 123, 256 - 5,
        7, 256 - 121, 71, 256 - 57, 39, 256 - 89, 103, 256 - 25,
        23, 256 - 105, 87, 256 - 41, 55, 256 - 73, 119, 256 - 9,
        15, 256 - 113, 79, 256 - 49, 47, 256 - 81, 111, 256 - 17,
        31, 256 - 97, 95, 256 - 33, 63, 256 - 65, 127, 256 - 1,
    };

    /// <summary>
    ///     Additional make up codes for both White and Black runs
    /// </summary>
    private static readonly short[] _additionalMakeup =
    {
        28679, 28679, 31752, unchecked((short)32777),
        unchecked((short)33801), unchecked((short)34825),
        unchecked((short)35849), unchecked((short)36873),
        29703, 29703, 30727, 30727,
        unchecked((short)37897), unchecked((short)38921),
        unchecked((short)39945), unchecked((short)40969),
    };

    /// <summary>
    ///     Main black run table, using the last 9 bits of possible 13 bit code
    /// </summary>
    private static readonly short[] _black =
    {
        // 0 - 7
        62, 62, 30, 30, 0, 0, 0, 0,
        // 8 - 15
        0, 0, 0, 0, 0, 0, 0, 0,
        // 16 - 23
        0, 0, 0, 0, 0, 0, 0, 0,
        // 24 - 31
        0, 0, 0, 0, 0, 0, 0, 0,
        // 32 - 39
        3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225,
        // 40 - 47
        3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225,
        // 48 - 55
        3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225,
        // 56 - 63
        3225, 3225, 3225, 3225, 3225, 3225, 3225, 3225,
        // 64 - 71
        588, 588, 588, 588, 588, 588, 588, 588,
        // 72 - 79
        1680, 1680, 20499, 22547, 24595, 26643, 1776, 1776,
        // 80 - 87
        1808, 1808, -24557, -22509, -20461, -18413, 1904, 1904,
        // 88 - 95
        1936, 1936, -16365, -14317, 782, 782, 782, 782,
        // 96 - 103
        814, 814, 814, 814, -12269, -10221, 10257, 10257,
        // 104 - 111
        12305, 12305, 14353, 14353, 16403, 18451, 1712, 1712,
        // 112 - 119
        1744, 1744, 28691, 30739, -32749, -30701, -28653, -26605,
        // 120 - 127
        2061, 2061, 2061, 2061, 2061, 2061, 2061, 2061,
        // 128 - 135
        424, 424, 424, 424, 424, 424, 424, 424,
        // 136 - 143
        424, 424, 424, 424, 424, 424, 424, 424,
        // 144 - 151
        424, 424, 424, 424, 424, 424, 424, 424,
        // 152 - 159
        424, 424, 424, 424, 424, 424, 424, 424,
        // 160 - 167
        750, 750, 750, 750, 1616, 1616, 1648, 1648,
        // 168 - 175
        1424, 1424, 1456, 1456, 1488, 1488, 1520, 1520,
        // 176 - 183
        1840, 1840, 1872, 1872, 1968, 1968, 8209, 8209,
        // 184 - 191
        524, 524, 524, 524, 524, 524, 524, 524,
        // 192 - 199
        556, 556, 556, 556, 556, 556, 556, 556,
        // 200 - 207
        1552, 1552, 1584, 1584, 2000, 2000, 2032, 2032,
        // 208 - 215
        976, 976, 1008, 1008, 1040, 1040, 1072, 1072,
        // 216 - 223
        1296, 1296, 1328, 1328, 718, 718, 718, 718,
        // 224 - 231
        456, 456, 456, 456, 456, 456, 456, 456,
        // 232 - 239
        456, 456, 456, 456, 456, 456, 456, 456,
        // 240 - 247
        456, 456, 456, 456, 456, 456, 456, 456,
        // 248 - 255
        456, 456, 456, 456, 456, 456, 456, 456,
        // 256 - 263
        326, 326, 326, 326, 326, 326, 326, 326,
        // 264 - 271
        326, 326, 326, 326, 326, 326, 326, 326,
        // 272 - 279
        326, 326, 326, 326, 326, 326, 326, 326,
        // 280 - 287
        326, 326, 326, 326, 326, 326, 326, 326,
        // 288 - 295
        326, 326, 326, 326, 326, 326, 326, 326,
        // 296 - 303
        326, 326, 326, 326, 326, 326, 326, 326,
        // 304 - 311
        326, 326, 326, 326, 326, 326, 326, 326,
        // 312 - 319
        326, 326, 326, 326, 326, 326, 326, 326,
        // 320 - 327
        358, 358, 358, 358, 358, 358, 358, 358,
        // 328 - 335
        358, 358, 358, 358, 358, 358, 358, 358,
        // 336 - 343
        358, 358, 358, 358, 358, 358, 358, 358,
        // 344 - 351
        358, 358, 358, 358, 358, 358, 358, 358,
        // 352 - 359
        358, 358, 358, 358, 358, 358, 358, 358,
        // 360 - 367
        358, 358, 358, 358, 358, 358, 358, 358,
        // 368 - 375
        358, 358, 358, 358, 358, 358, 358, 358,
        // 376 - 383
        358, 358, 358, 358, 358, 358, 358, 358,
        // 384 - 391
        490, 490, 490, 490, 490, 490, 490, 490,
        // 392 - 399
        490, 490, 490, 490, 490, 490, 490, 490,
        // 400 - 407
        4113, 4113, 6161, 6161, 848, 848, 880, 880,
        // 408 - 415
        912, 912, 944, 944, 622, 622, 622, 622,
        // 416 - 423
        654, 654, 654, 654, 1104, 1104, 1136, 1136,
        // 424 - 431
        1168, 1168, 1200, 1200, 1232, 1232, 1264, 1264,
        // 432 - 439
        686, 686, 686, 686, 1360, 1360, 1392, 1392,
        // 440 - 447
        12, 12, 12, 12, 12, 12, 12, 12,
        // 448 - 455
        390, 390, 390, 390, 390, 390, 390, 390,
        // 456 - 463
        390, 390, 390, 390, 390, 390, 390, 390,
        // 464 - 471
        390, 390, 390, 390, 390, 390, 390, 390,
        // 472 - 479
        390, 390, 390, 390, 390, 390, 390, 390,
        // 480 - 487
        390, 390, 390, 390, 390, 390, 390, 390,
        // 488 - 495
        390, 390, 390, 390, 390, 390, 390, 390,
        // 496 - 503
        390, 390, 390, 390, 390, 390, 390, 390,
        // 504 - 511
        390, 390, 390, 390, 390, 390, 390, 390,
    };

    /// <summary>
    ///     Initial black run look up table, uses the first 4 bits of a code
    /// </summary>
    private static readonly short[] _initBlack =
    {
        // 0 - 7
        3226, 6412, 200, 168, 38, 38, 134, 134,
        // 8 - 15
        100, 100, 100, 100, 68, 68, 68, 68,
    };

    private static readonly int[] _table1 =
    {
        0x00, // 0 bits are left in first byte - SHOULD NOT HAPPEN
        0x01, // 1 bits are left in first byte
        0x03, // 2 bits are left in first byte
        0x07, // 3 bits are left in first byte
        0x0f, // 4 bits are left in first byte
        0x1f, // 5 bits are left in first byte
        0x3f, // 6 bits are left in first byte
        0x7f, // 7 bits are left in first byte
        0xff, // 8 bits are left in first byte
    };

    private static readonly int[] _table2 =
    {
        0x00, // 0
        0x80, // 1
        0xc0, // 2
        0xe0, // 3
        0xf0, // 4
        0xf8, // 5
        0xfc, // 6
        0xfe, // 7
        0xff, // 8
    };

    /// <summary>
    /// </summary>
    private static readonly short[] _twoBitBlack = { 292, 260, 226, 226 };

    // 0 - 3
    private static readonly byte[] _twoDCodes =
    {
        // 0 - 7
        80, 88, 23, 71, 30, 30, 62, 62,
        // 8 - 15
        4, 4, 4, 4, 4, 4, 4, 4,
        // 16 - 23
        11, 11, 11, 11, 11, 11, 11, 11,
        // 24 - 31
        11, 11, 11, 11, 11, 11, 11, 11,
        // 32 - 39
        35, 35, 35, 35, 35, 35, 35, 35,
        // 40 - 47
        35, 35, 35, 35, 35, 35, 35, 35,
        // 48 - 55
        51, 51, 51, 51, 51, 51, 51, 51,
        // 56 - 63
        51, 51, 51, 51, 51, 51, 51, 51,
        // 64 - 71
        41, 41, 41, 41, 41, 41, 41, 41,
        // 72 - 79
        41, 41, 41, 41, 41, 41, 41, 41,
        // 80 - 87
        41, 41, 41, 41, 41, 41, 41, 41,
        // 88 - 95
        41, 41, 41, 41, 41, 41, 41, 41,
        // 96 - 103
        41, 41, 41, 41, 41, 41, 41, 41,
        // 104 - 111
        41, 41, 41, 41, 41, 41, 41, 41,
        // 112 - 119
        41, 41, 41, 41, 41, 41, 41, 41,
        // 120 - 127
        41, 41, 41, 41, 41, 41, 41, 41,
    };

    /// <summary>
    ///     The main 10 bit white runs lookup table
    /// </summary>
    private static readonly short[] _white =
    {
        // 0 - 7
        6430, 6400, 6400, 6400, 3225, 3225, 3225, 3225,
        // 8 - 15
        944, 944, 944, 944, 976, 976, 976, 976,
        // 16 - 23
        1456, 1456, 1456, 1456, 1488, 1488, 1488, 1488,
        // 24 - 31
        718, 718, 718, 718, 718, 718, 718, 718,
        // 32 - 39
        750, 750, 750, 750, 750, 750, 750, 750,
        // 40 - 47
        1520, 1520, 1520, 1520, 1552, 1552, 1552, 1552,
        // 48 - 55
        428, 428, 428, 428, 428, 428, 428, 428,
        // 56 - 63
        428, 428, 428, 428, 428, 428, 428, 428,
        // 64 - 71
        654, 654, 654, 654, 654, 654, 654, 654,
        // 72 - 79
        1072, 1072, 1072, 1072, 1104, 1104, 1104, 1104,
        // 80 - 87
        1136, 1136, 1136, 1136, 1168, 1168, 1168, 1168,
        // 88 - 95
        1200, 1200, 1200, 1200, 1232, 1232, 1232, 1232,
        // 96 - 103
        622, 622, 622, 622, 622, 622, 622, 622,
        // 104 - 111
        1008, 1008, 1008, 1008, 1040, 1040, 1040, 1040,
        // 112 - 119
        44, 44, 44, 44, 44, 44, 44, 44,
        // 120 - 127
        44, 44, 44, 44, 44, 44, 44, 44,
        // 128 - 135
        396, 396, 396, 396, 396, 396, 396, 396,
        // 136 - 143
        396, 396, 396, 396, 396, 396, 396, 396,
        // 144 - 151
        1712, 1712, 1712, 1712, 1744, 1744, 1744, 1744,
        // 152 - 159
        846, 846, 846, 846, 846, 846, 846, 846,
        // 160 - 167
        1264, 1264, 1264, 1264, 1296, 1296, 1296, 1296,
        // 168 - 175
        1328, 1328, 1328, 1328, 1360, 1360, 1360, 1360,
        // 176 - 183
        1392, 1392, 1392, 1392, 1424, 1424, 1424, 1424,
        // 184 - 191
        686, 686, 686, 686, 686, 686, 686, 686,
        // 192 - 199
        910, 910, 910, 910, 910, 910, 910, 910,
        // 200 - 207
        1968, 1968, 1968, 1968, 2000, 2000, 2000, 2000,
        // 208 - 215
        2032, 2032, 2032, 2032, 16, 16, 16, 16,
        // 216 - 223
        10257, 10257, 10257, 10257, 12305, 12305, 12305, 12305,
        // 224 - 231
        330, 330, 330, 330, 330, 330, 330, 330,
        // 232 - 239
        330, 330, 330, 330, 330, 330, 330, 330,
        // 240 - 247
        330, 330, 330, 330, 330, 330, 330, 330,
        // 248 - 255
        330, 330, 330, 330, 330, 330, 330, 330,
        // 256 - 263
        362, 362, 362, 362, 362, 362, 362, 362,
        // 264 - 271
        362, 362, 362, 362, 362, 362, 362, 362,
        // 272 - 279
        362, 362, 362, 362, 362, 362, 362, 362,
        // 280 - 287
        362, 362, 362, 362, 362, 362, 362, 362,
        // 288 - 295
        878, 878, 878, 878, 878, 878, 878, 878,
        // 296 - 303
        1904, 1904, 1904, 1904, 1936, 1936, 1936, 1936,
        // 304 - 311
        -18413, -18413, -16365, -16365, -14317, -14317, -10221, -10221,
        // 312 - 319
        590, 590, 590, 590, 590, 590, 590, 590,
        // 320 - 327
        782, 782, 782, 782, 782, 782, 782, 782,
        // 328 - 335
        1584, 1584, 1584, 1584, 1616, 1616, 1616, 1616,
        // 336 - 343
        1648, 1648, 1648, 1648, 1680, 1680, 1680, 1680,
        // 344 - 351
        814, 814, 814, 814, 814, 814, 814, 814,
        // 352 - 359
        1776, 1776, 1776, 1776, 1808, 1808, 1808, 1808,
        // 360 - 367
        1840, 1840, 1840, 1840, 1872, 1872, 1872, 1872,
        // 368 - 375
        6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157,
        // 376 - 383
        6157, 6157, 6157, 6157, 6157, 6157, 6157, 6157,
        // 384 - 391
        -12275, -12275, -12275, -12275, -12275, -12275, -12275, -12275,
        // 392 - 399
        -12275, -12275, -12275, -12275, -12275, -12275, -12275, -12275,
        // 400 - 407
        14353, 14353, 14353, 14353, 16401, 16401, 16401, 16401,
        // 408 - 415
        22547, 22547, 24595, 24595, 20497, 20497, 20497, 20497,
        // 416 - 423
        18449, 18449, 18449, 18449, 26643, 26643, 28691, 28691,
        // 424 - 431
        30739, 30739, -32749, -32749, -30701, -30701, -28653, -28653,
        // 432 - 439
        -26605, -26605, -24557, -24557, -22509, -22509, -20461, -20461,
        // 440 - 447
        8207, 8207, 8207, 8207, 8207, 8207, 8207, 8207,
        // 448 - 455
        72, 72, 72, 72, 72, 72, 72, 72,
        // 456 - 463
        72, 72, 72, 72, 72, 72, 72, 72,
        // 464 - 471
        72, 72, 72, 72, 72, 72, 72, 72,
        // 472 - 479
        72, 72, 72, 72, 72, 72, 72, 72,
        // 480 - 487
        72, 72, 72, 72, 72, 72, 72, 72,
        // 488 - 495
        72, 72, 72, 72, 72, 72, 72, 72,
        // 496 - 503
        72, 72, 72, 72, 72, 72, 72, 72,
        // 504 - 511
        72, 72, 72, 72, 72, 72, 72, 72,
        // 512 - 519
        104, 104, 104, 104, 104, 104, 104, 104,
        // 520 - 527
        104, 104, 104, 104, 104, 104, 104, 104,
        // 528 - 535
        104, 104, 104, 104, 104, 104, 104, 104,
        // 536 - 543
        104, 104, 104, 104, 104, 104, 104, 104,
        // 544 - 551
        104, 104, 104, 104, 104, 104, 104, 104,
        // 552 - 559
        104, 104, 104, 104, 104, 104, 104, 104,
        // 560 - 567
        104, 104, 104, 104, 104, 104, 104, 104,
        // 568 - 575
        104, 104, 104, 104, 104, 104, 104, 104,
        // 576 - 583
        4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107,
        // 584 - 591
        4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107,
        // 592 - 599
        4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107,
        // 600 - 607
        4107, 4107, 4107, 4107, 4107, 4107, 4107, 4107,
        // 608 - 615
        266, 266, 266, 266, 266, 266, 266, 266,
        // 616 - 623
        266, 266, 266, 266, 266, 266, 266, 266,
        // 624 - 631
        266, 266, 266, 266, 266, 266, 266, 266,
        // 632 - 639
        266, 266, 266, 266, 266, 266, 266, 266,
        // 640 - 647
        298, 298, 298, 298, 298, 298, 298, 298,
        // 648 - 655
        298, 298, 298, 298, 298, 298, 298, 298,
        // 656 - 663
        298, 298, 298, 298, 298, 298, 298, 298,
        // 664 - 671
        298, 298, 298, 298, 298, 298, 298, 298,
        // 672 - 679
        524, 524, 524, 524, 524, 524, 524, 524,
        // 680 - 687
        524, 524, 524, 524, 524, 524, 524, 524,
        // 688 - 695
        556, 556, 556, 556, 556, 556, 556, 556,
        // 696 - 703
        556, 556, 556, 556, 556, 556, 556, 556,
        // 704 - 711
        136, 136, 136, 136, 136, 136, 136, 136,
        // 712 - 719
        136, 136, 136, 136, 136, 136, 136, 136,
        // 720 - 727
        136, 136, 136, 136, 136, 136, 136, 136,
        // 728 - 735
        136, 136, 136, 136, 136, 136, 136, 136,
        // 736 - 743
        136, 136, 136, 136, 136, 136, 136, 136,
        // 744 - 751
        136, 136, 136, 136, 136, 136, 136, 136,
        // 752 - 759
        136, 136, 136, 136, 136, 136, 136, 136,
        // 760 - 767
        136, 136, 136, 136, 136, 136, 136, 136,
        // 768 - 775
        168, 168, 168, 168, 168, 168, 168, 168,
        // 776 - 783
        168, 168, 168, 168, 168, 168, 168, 168,
        // 784 - 791
        168, 168, 168, 168, 168, 168, 168, 168,
        // 792 - 799
        168, 168, 168, 168, 168, 168, 168, 168,
        // 800 - 807
        168, 168, 168, 168, 168, 168, 168, 168,
        // 808 - 815
        168, 168, 168, 168, 168, 168, 168, 168,
        // 816 - 823
        168, 168, 168, 168, 168, 168, 168, 168,
        // 824 - 831
        168, 168, 168, 168, 168, 168, 168, 168,
        // 832 - 839
        460, 460, 460, 460, 460, 460, 460, 460,
        // 840 - 847
        460, 460, 460, 460, 460, 460, 460, 460,
        // 848 - 855
        492, 492, 492, 492, 492, 492, 492, 492,
        // 856 - 863
        492, 492, 492, 492, 492, 492, 492, 492,
        // 864 - 871
        2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059,
        // 872 - 879
        2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059,
        // 880 - 887
        2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059,
        // 888 - 895
        2059, 2059, 2059, 2059, 2059, 2059, 2059, 2059,
        // 896 - 903
        200, 200, 200, 200, 200, 200, 200, 200,
        // 904 - 911
        200, 200, 200, 200, 200, 200, 200, 200,
        // 912 - 919
        200, 200, 200, 200, 200, 200, 200, 200,
        // 920 - 927
        200, 200, 200, 200, 200, 200, 200, 200,
        // 928 - 935
        200, 200, 200, 200, 200, 200, 200, 200,
        // 936 - 943
        200, 200, 200, 200, 200, 200, 200, 200,
        // 944 - 951
        200, 200, 200, 200, 200, 200, 200, 200,
        // 952 - 959
        200, 200, 200, 200, 200, 200, 200, 200,
        // 960 - 967
        232, 232, 232, 232, 232, 232, 232, 232,
        // 968 - 975
        232, 232, 232, 232, 232, 232, 232, 232,
        // 976 - 983
        232, 232, 232, 232, 232, 232, 232, 232,
        // 984 - 991
        232, 232, 232, 232, 232, 232, 232, 232,
        // 992 - 999
        232, 232, 232, 232, 232, 232, 232, 232,
        // 1000 - 1007
        232, 232, 232, 232, 232, 232, 232, 232,
        // 1008 - 1015
        232, 232, 232, 232, 232, 232, 232, 232,
        // 1016 - 1023
        232, 232, 232, 232, 232, 232, 232, 232,
    };

    private readonly int _fillOrder;
    private readonly int _w;
    private int _bitPointer, _bytePointer;

    /// <summary>
    ///     Data structures needed to store changing elements for the previous
    /// </summary>
    /// <summary>
    ///     and the current scanline
    /// </summary>
    private int _changingElemSize;

    private int _compression = 2;
    private int[] _currChangingElems;
    private byte[] _data;
    private int _fillBits;
    private readonly int _h;

    /// <summary>
    ///     Element at which to start search in getNextChangingElement
    /// </summary>
    private int _lastChangingElement;

    private int _oneD;
    private int[] _prevChangingElems;

    /// <summary>
    ///     Variables set by T4Options
    /// </summary>
    private int _uncompressedMode;

    /// <summary>
    /// </summary>
    /// <param name="fillOrder">The fill order of the compressed data bytes.</param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public TiffFaxDecoder(int fillOrder, int w, int h)
    {
        _fillOrder = fillOrder;
        _w = w;
        _h = h;

        _bitPointer = 0;
        _bytePointer = 0;
        _prevChangingElems = new int[w];
        _currChangingElems = new int[w];
    }

    public static void ReverseBits(byte[] b)
    {
        if (b == null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        for (var k = 0; k < b.Length; ++k)
        {
            b[k] = FlipTable[b[k] & 0xff];
        }
    }


    /// <summary>
    ///     One-dimensional decoding methods
    /// </summary>
    public void Decode1D(byte[] buffer, byte[] compData,
                         int startX, int height)
    {
        _data = compData;

        var lineOffset = 0;
        var scanlineStride = (_w + 7) / 8;

        _bitPointer = 0;
        _bytePointer = 0;

        for (var i = 0; i < height; i++)
        {
            DecodeNextScanline(buffer, lineOffset, startX);
            lineOffset += scanlineStride;
        }
    }

    public void Decode2D(byte[] buffer,
                         byte[] compData,
                         int startX,
                         int height,
                         long tiffT4Options)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        _data = compData;
        _compression = 3;

        _bitPointer = 0;
        _bytePointer = 0;

        var scanlineStride = (_w + 7) / 8;

        int a0, a1, b1, b2;
        var b = new int[2];
        int entry, code, bits;
        bool isWhite;
        var currIndex = 0;
        int[] temp;

        // fillBits - dealt with this in readEOL
        // 1D/2D encoding - dealt with this in readEOL

        // uncompressedMode - haven't dealt with this yet.


        _oneD = (int)(tiffT4Options & 0x01);
        _uncompressedMode = (int)((tiffT4Options & 0x02) >> 1);
        _fillBits = (int)((tiffT4Options & 0x04) >> 2);

        // The data must start with an EOL code
        if (readEol(true) != 1)
        {
            throw new InvalidOperationException("First scanline must be 1D encoded.");
        }

        var lineOffset = 0;
        int bitOffset;

        // Then the 1D encoded scanline data will occur, changing elements
        // array gets set.
        DecodeNextScanline(buffer, lineOffset, startX);
        lineOffset += scanlineStride;

        for (var lines = 1; lines < height; lines++)
        {
            // Every line must begin with an EOL followed by a bit which
            // indicates whether the following scanline is 1D or 2D encoded.
            if (readEol(false) == 0)
            {
                // 2D encoded scanline follows

                // Initialize previous scanlines changing elements, and
                // initialize current scanline's changing elements array
                temp = _prevChangingElems;
                _prevChangingElems = _currChangingElems;
                _currChangingElems = temp;
                currIndex = 0;

                // a0 has to be set just before the start of this scanline.
                a0 = -1;
                isWhite = true;
                bitOffset = startX;

                _lastChangingElement = 0;

                while (bitOffset < _w)
                {
                    // Get the next changing element
                    getNextChangingElement(a0, isWhite, b);

                    b1 = b[0];
                    b2 = b[1];

                    // Get the next seven bits
                    entry = nextLesserThan8Bits(7);

                    // Run these through the 2DCodes table
                    entry = _twoDCodes[entry] & 0xff;

                    // Get the code and the number of bits used up
                    code = (entry & 0x78) >> 3;
                    bits = entry & 0x07;

                    if (code == 0)
                    {
                        if (!isWhite)
                        {
                            setToBlack(buffer, lineOffset, bitOffset,
                                       b2 - bitOffset);
                        }

                        bitOffset = a0 = b2;

                        // Set pointer to consume the correct number of bits.
                        updatePointer(7 - bits);
                    }
                    else if (code == 1)
                    {
                        // Horizontal
                        updatePointer(7 - bits);

                        // identify the next 2 codes.
                        int number;
                        if (isWhite)
                        {
                            number = decodeWhiteCodeWord();
                            bitOffset += number;
                            _currChangingElems[currIndex++] = bitOffset;

                            number = decodeBlackCodeWord();
                            setToBlack(buffer, lineOffset, bitOffset, number);
                            bitOffset += number;
                            _currChangingElems[currIndex++] = bitOffset;
                        }
                        else
                        {
                            number = decodeBlackCodeWord();
                            setToBlack(buffer, lineOffset, bitOffset, number);
                            bitOffset += number;
                            _currChangingElems[currIndex++] = bitOffset;

                            number = decodeWhiteCodeWord();
                            bitOffset += number;
                            _currChangingElems[currIndex++] = bitOffset;
                        }

                        a0 = bitOffset;
                    }
                    else if (code <= 8)
                    {
                        // Vertical
                        a1 = b1 + (code - 5);

                        _currChangingElems[currIndex++] = a1;

                        // We write the current color till a1 - 1 pos,
                        // since a1 is where the next color starts
                        if (!isWhite)
                        {
                            setToBlack(buffer, lineOffset, bitOffset,
                                       a1 - bitOffset);
                        }

                        bitOffset = a0 = a1;
                        isWhite = !isWhite;

                        updatePointer(7 - bits);
                    }
                    else
                    {
                        throw new
                            InvalidOperationException("Invalid code encountered while decoding 2D group 3 compressed data.");
                    }
                }

                // Add the changing element beyond the current scanline for the
                // other color too
                _currChangingElems[currIndex++] = bitOffset;
                _changingElemSize = currIndex;
            }
            else
            {
                // 1D encoded scanline follows
                DecodeNextScanline(buffer, lineOffset, startX);
            }

            lineOffset += scanlineStride;
        }
    }

    public void DecodeNextScanline(byte[] buffer,
                                   int lineOffset, int bitOffset)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        int bits = 0, code = 0, isT = 0;
        int current, entry, twoBits;
        var isWhite = true;

        // Initialize starting of the changing elements array
        _changingElemSize = 0;

        // While scanline not complete
        while (bitOffset < _w)
        {
            while (isWhite)
            {
                // White run
                current = nextNBits(10);
                entry = _white[current];

                // Get the 3 fields from the entry
                isT = entry & 0x0001;
                bits = (entry >> 1) & 0x0f;

                if (bits == 12)
                {
                    // Additional Make up code
                    // Get the next 2 bits
                    twoBits = nextLesserThan8Bits(2);
                    // Consolidate the 2 new bits and last 2 bits into 4 bits
                    current = ((current << 2) & 0x000c) | twoBits;
                    entry = _additionalMakeup[current];
                    bits = (entry >> 1) & 0x07; // 3 bits 0000 0111
                    code = (entry >> 4) & 0x0fff; // 12 bits
                    bitOffset += code; // Skip white run

                    updatePointer(4 - bits);
                }
                else if (bits == 0)
                {
                    // ERROR
                    throw new InvalidOperationException("Invalid code encountered.");
                }
                else if (bits == 15)
                {
                    // EOL
                    throw new InvalidOperationException("EOL code word encountered in White run.");
                }
                else
                {
                    // 11 bits - 0000 0111 1111 1111 = 0x07ff
                    code = (entry >> 5) & 0x07ff;
                    bitOffset += code;

                    updatePointer(10 - bits);
                    if (isT == 0)
                    {
                        isWhite = false;
                        _currChangingElems[_changingElemSize++] = bitOffset;
                    }
                }
            }

            // Check whether this run completed one width, if so
            // advance to next byte boundary for compression = 2.
            if (bitOffset == _w)
            {
                if (_compression == 2)
                {
                    advancePointer();
                }

                break;
            }

            while (isWhite == false)
            {
                // Black run
                current = nextLesserThan8Bits(4);
                entry = _initBlack[current];

                // Get the 3 fields from the entry
                isT = entry & 0x0001;
                bits = (entry >> 1) & 0x000f;
                code = (entry >> 5) & 0x07ff;

                if (code == 100)
                {
                    current = nextNBits(9);
                    entry = _black[current];

                    // Get the 3 fields from the entry
                    isT = entry & 0x0001;
                    bits = (entry >> 1) & 0x000f;
                    code = (entry >> 5) & 0x07ff;

                    if (bits == 12)
                    {
                        // Additional makeup codes
                        updatePointer(5);
                        current = nextLesserThan8Bits(4);
                        entry = _additionalMakeup[current];
                        bits = (entry >> 1) & 0x07; // 3 bits 0000 0111
                        code = (entry >> 4) & 0x0fff; // 12 bits

                        setToBlack(buffer, lineOffset, bitOffset, code);
                        bitOffset += code;

                        updatePointer(4 - bits);
                    }
                    else if (bits == 15)
                    {
                        // EOL code
                        throw new InvalidOperationException("EOL code word encountered in Black run.");
                    }
                    else
                    {
                        setToBlack(buffer, lineOffset, bitOffset, code);
                        bitOffset += code;

                        updatePointer(9 - bits);
                        if (isT == 0)
                        {
                            isWhite = true;
                            _currChangingElems[_changingElemSize++] = bitOffset;
                        }
                    }
                }
                else if (code == 200)
                {
                    // Is a Terminating code
                    current = nextLesserThan8Bits(2);
                    entry = _twoBitBlack[current];
                    code = (entry >> 5) & 0x07ff;
                    bits = (entry >> 1) & 0x0f;

                    setToBlack(buffer, lineOffset, bitOffset, code);
                    bitOffset += code;

                    updatePointer(2 - bits);
                    isWhite = true;
                    _currChangingElems[_changingElemSize++] = bitOffset;
                }
                else
                {
                    // Is a Terminating code
                    setToBlack(buffer, lineOffset, bitOffset, code);
                    bitOffset += code;

                    updatePointer(4 - bits);
                    isWhite = true;
                    _currChangingElems[_changingElemSize++] = bitOffset;
                }
            }

            // Check whether this run completed one width
            if (bitOffset == _w)
            {
                if (_compression == 2)
                {
                    advancePointer();
                }

                break;
            }
        }

        _currChangingElems[_changingElemSize++] = bitOffset;
    }

    /// <summary>
    ///     Two-dimensional decoding methods
    /// </summary>
    public void DecodeT6(byte[] buffer,
                         byte[] compData,
                         int startX,
                         int height,
                         long tiffT6Options)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }

        _data = compData;
        _compression = 4;

        _bitPointer = 0;
        _bytePointer = 0;

        var scanlineStride = (_w + 7) / 8;

        int a0, a1, b1, b2;
        int entry, code, bits;
        bool isWhite;
        int currIndex;
        int[] temp;

        // Return values from getNextChangingElement
        var b = new int[2];

        // uncompressedMode - have written some code for this, but this
        // has not been tested due to lack of test images using this optional

        _uncompressedMode = (int)((tiffT6Options & 0x02) >> 1);

        // Local cached reference
        var cce = _currChangingElems;

        // Assume invisible preceding row of all white pixels and insert
        // both black and white changing elements beyond the end of this
        // imaginary scanline.
        _changingElemSize = 0;
        cce[_changingElemSize++] = _w;
        cce[_changingElemSize++] = _w;

        var lineOffset = 0;
        int bitOffset;

        for (var lines = 0; lines < height; lines++)
        {
            // a0 has to be set just before the start of the scanline.
            a0 = -1;
            isWhite = true;

            // Assign the changing elements of the previous scanline to
            // prevChangingElems and start putting this new scanline's
            // changing elements into the currChangingElems.
            temp = _prevChangingElems;
            _prevChangingElems = _currChangingElems;
            cce = _currChangingElems = temp;
            currIndex = 0;

            // Start decoding the scanline at startX in the raster
            bitOffset = startX;

            // Reset search start position for getNextChangingElement
            _lastChangingElement = 0;

            // Till one whole scanline is decoded
            while (bitOffset < _w)
            {
                // Get the next changing element
                getNextChangingElement(a0, isWhite, b);
                b1 = b[0];
                b2 = b[1];

                // Get the next seven bits
                entry = nextLesserThan8Bits(7);
                // Run these through the 2DCodes table
                entry = _twoDCodes[entry] & 0xff;

                // Get the code and the number of bits used up
                code = (entry & 0x78) >> 3;
                bits = entry & 0x07;

                if (code == 0)
                {
                    // Pass
                    // We always assume WhiteIsZero format for fax.
                    if (!isWhite)
                    {
                        setToBlack(buffer, lineOffset, bitOffset,
                                   b2 - bitOffset);
                    }

                    bitOffset = a0 = b2;

                    // Set pointer to only consume the correct number of bits.
                    updatePointer(7 - bits);
                }
                else if (code == 1)
                {
                    // Horizontal
                    // Set pointer to only consume the correct number of bits.
                    updatePointer(7 - bits);

                    // identify the next 2 alternating color codes.
                    int number;
                    if (isWhite)
                    {
                        // Following are white and black runs
                        number = decodeWhiteCodeWord();
                        bitOffset += number;
                        cce[currIndex++] = bitOffset;

                        number = decodeBlackCodeWord();
                        setToBlack(buffer, lineOffset, bitOffset, number);
                        bitOffset += number;
                        cce[currIndex++] = bitOffset;
                    }
                    else
                    {
                        // First a black run and then a white run follows
                        number = decodeBlackCodeWord();
                        setToBlack(buffer, lineOffset, bitOffset, number);
                        bitOffset += number;
                        cce[currIndex++] = bitOffset;

                        number = decodeWhiteCodeWord();
                        bitOffset += number;
                        cce[currIndex++] = bitOffset;
                    }

                    a0 = bitOffset;
                }
                else if (code <= 8)
                {
                    // Vertical
                    a1 = b1 + (code - 5);
                    cce[currIndex++] = a1;

                    // We write the current color till a1 - 1 pos,
                    // since a1 is where the next color starts
                    if (!isWhite)
                    {
                        setToBlack(buffer, lineOffset, bitOffset,
                                   a1 - bitOffset);
                    }

                    bitOffset = a0 = a1;
                    isWhite = !isWhite;

                    updatePointer(7 - bits);
                }
                else if (code == 11)
                {
                    if (nextLesserThan8Bits(3) != 7)
                    {
                        throw new
                            InvalidOperationException("Invalid code encountered while decoding 2D group 4 compressed data.");
                    }

                    var zeros = 0;
                    var exit = false;

                    while (!exit)
                    {
                        while (nextLesserThan8Bits(1) != 1)
                        {
                            zeros++;
                        }

                        if (zeros > 5)
                        {
                            // Exit code

                            // Zeros before exit code
                            zeros = zeros - 6;

                            if (!isWhite && zeros > 0)
                            {
                                cce[currIndex++] = bitOffset;
                            }

                            // Zeros before the exit code
                            bitOffset += zeros;
                            if (zeros > 0)
                            {
                                // Some zeros have been written
                                isWhite = true;
                            }

                            // Read in the bit which specifies the color of
                            // the following run
                            if (nextLesserThan8Bits(1) == 0)
                            {
                                if (!isWhite)
                                {
                                    cce[currIndex++] = bitOffset;
                                }

                                isWhite = true;
                            }
                            else
                            {
                                if (isWhite)
                                {
                                    cce[currIndex++] = bitOffset;
                                }

                                isWhite = false;
                            }

                            exit = true;
                        }

                        if (zeros == 5)
                        {
                            if (!isWhite)
                            {
                                cce[currIndex++] = bitOffset;
                            }

                            bitOffset += zeros;

                            // Last thing written was white
                            isWhite = true;
                        }
                        else
                        {
                            bitOffset += zeros;

                            cce[currIndex++] = bitOffset;
                            setToBlack(buffer, lineOffset, bitOffset, 1);
                            ++bitOffset;

                            // Last thing written was black
                            isWhite = false;
                        }
                    }
                }
                else
                {
                    //micah_tessler@yahoo.com
                    //Microsoft TIFF renderers seem to treat unknown codes as line-breaks
                    //That is, they give up on the current line and move on to the next one
                    //set bitOffset to w to move on to the next scan line.
                    bitOffset = _w;
                    updatePointer(7 - bits);
                }
            }

            // Add the changing element beyond the current scanline for the
            // other color too
            //make sure that the index does not exceed the bounds of the array
            if (currIndex < cce.Length)
            {
                cce[currIndex++] = bitOffset;
            }

            // Number of changing elements in this scanline.
            _changingElemSize = currIndex;

            lineOffset += scanlineStride;
        }
    }

    /// <summary>
    ///     Move to the next byte boundary
    /// </summary>
    private bool advancePointer()
    {
        if (_bitPointer != 0)
        {
            _bytePointer++;
            _bitPointer = 0;
        }

        return true;
    }

    /// <summary>
    ///     Returns run length
    /// </summary>
    private int decodeBlackCodeWord()
    {
        int current, entry, bits, isT, code = -1;
        var runLength = 0;
        var isWhite = false;

        while (!isWhite)
        {
            current = nextLesserThan8Bits(4);
            entry = _initBlack[current];

            // Get the 3 fields from the entry
            isT = entry & 0x0001;
            bits = (entry >> 1) & 0x000f;
            code = (entry >> 5) & 0x07ff;

            if (code == 100)
            {
                current = nextNBits(9);
                entry = _black[current];

                // Get the 3 fields from the entry
                isT = entry & 0x0001;
                bits = (entry >> 1) & 0x000f;
                code = (entry >> 5) & 0x07ff;

                if (bits == 12)
                {
                    // Additional makeup codes
                    updatePointer(5);
                    current = nextLesserThan8Bits(4);
                    entry = _additionalMakeup[current];
                    bits = (entry >> 1) & 0x07; // 3 bits 0000 0111
                    code = (entry >> 4) & 0x0fff; // 12 bits
                    runLength += code;

                    updatePointer(4 - bits);
                }
                else if (bits == 15)
                {
                    // EOL code
                    throw new InvalidOperationException("EOL code word encountered in Black run.");
                }
                else
                {
                    runLength += code;
                    updatePointer(9 - bits);
                    if (isT == 0)
                    {
                        isWhite = true;
                    }
                }
            }
            else if (code == 200)
            {
                // Is a Terminating code
                current = nextLesserThan8Bits(2);
                entry = _twoBitBlack[current];
                code = (entry >> 5) & 0x07ff;
                runLength += code;
                bits = (entry >> 1) & 0x0f;
                updatePointer(2 - bits);
                isWhite = true;
            }
            else
            {
                // Is a Terminating code
                runLength += code;
                updatePointer(4 - bits);
                isWhite = true;
            }
        }

        return runLength;
    }

    /// <summary>
    ///     Returns run length
    /// </summary>
    private int decodeWhiteCodeWord()
    {
        int current, entry, bits, isT, twoBits, code = -1;
        var runLength = 0;
        var isWhite = true;

        while (isWhite)
        {
            current = nextNBits(10);
            entry = _white[current];

            // Get the 3 fields from the entry
            isT = entry & 0x0001;
            bits = (entry >> 1) & 0x0f;

            if (bits == 12)
            {
                // Additional Make up code
                // Get the next 2 bits
                twoBits = nextLesserThan8Bits(2);
                // Consolidate the 2 new bits and last 2 bits into 4 bits
                current = ((current << 2) & 0x000c) | twoBits;
                entry = _additionalMakeup[current];
                bits = (entry >> 1) & 0x07; // 3 bits 0000 0111
                code = (entry >> 4) & 0x0fff; // 12 bits
                runLength += code;
                updatePointer(4 - bits);
            }
            else if (bits == 0)
            {
                // ERROR
                throw new InvalidOperationException("Invalid code encountered.");
            }
            else if (bits == 15)
            {
                // EOL
                throw new InvalidOperationException("EOL code word encountered in White run.");
            }
            else
            {
                // 11 bits - 0000 0111 1111 1111 = 0x07ff
                code = (entry >> 5) & 0x07ff;
                runLength += code;
                updatePointer(10 - bits);
                if (isT == 0)
                {
                    isWhite = false;
                }
            }
        }

        return runLength;
    }

    private void getNextChangingElement(int a0, bool isWhite, int[] ret)
    {
        // Local copies of instance variables
        var pce = _prevChangingElems;
        var ces = _changingElemSize;

        // If the previous match was at an odd element, we still
        // have to search the preceeding element.
        // int start = lastChangingElement & ~0x1;
        var start = _lastChangingElement > 0 ? _lastChangingElement - 1 : 0;
        if (isWhite)
        {
            start &= ~0x1; // Search even numbered elements
        }
        else
        {
            start |= 0x1; // Search odd numbered elements
        }

        var i = start;
        for (; i < ces; i += 2)
        {
            var temp = pce[i];
            if (temp > a0)
            {
                _lastChangingElement = i;
                ret[0] = temp;
                break;
            }
        }

        if (i + 1 < ces)
        {
            ret[1] = pce[i + 1];
        }
    }

    private int nextLesserThan8Bits(int bitsToGet)
    {
        byte b, next;
        var l = _data.Length - 1;
        var bp = _bytePointer;

        if (_fillOrder == 1)
        {
            b = _data[bp];
            if (bp == l)
            {
                next = 0x00;
            }
            else
            {
                next = _data[bp + 1];
            }
        }
        else if (_fillOrder == 2)
        {
            b = FlipTable[_data[bp] & 0xff];
            if (bp == l)
            {
                next = 0x00;
            }
            else
            {
                next = FlipTable[_data[bp + 1] & 0xff];
            }
        }
        else
        {
            throw new InvalidOperationException("TIFF_FILL_ORDER tag must be either 1 or 2.");
        }

        var bitsLeft = 8 - _bitPointer;
        var bitsFromNextByte = bitsToGet - bitsLeft;

        var shift = bitsLeft - bitsToGet;
        int i1, i2;
        if (shift >= 0)
        {
            i1 = (b & _table1[bitsLeft]) >> shift;
            _bitPointer += bitsToGet;
            if (_bitPointer == 8)
            {
                _bitPointer = 0;
                _bytePointer++;
            }
        }
        else
        {
            i1 = (b & _table1[bitsLeft]) << -shift;
            i2 = (next & _table2[bitsFromNextByte]) >> (8 - bitsFromNextByte);

            i1 |= i2;
            _bytePointer++;
            _bitPointer = bitsFromNextByte;
        }

        return i1;
    }

    private int nextNBits(int bitsToGet)
    {
        byte b, next, next2Next;
        var l = _data.Length - 1;
        var bp = _bytePointer;

        if (_fillOrder == 1)
        {
            b = _data[bp];

            if (bp == l)
            {
                next = 0x00;
                next2Next = 0x00;
            }
            else if (bp + 1 == l)
            {
                next = _data[bp + 1];
                next2Next = 0x00;
            }
            else
            {
                next = _data[bp + 1];
                next2Next = _data[bp + 2];
            }
        }
        else if (_fillOrder == 2)
        {
            b = FlipTable[_data[bp] & 0xff];

            if (bp == l)
            {
                next = 0x00;
                next2Next = 0x00;
            }
            else if (bp + 1 == l)
            {
                next = FlipTable[_data[bp + 1] & 0xff];
                next2Next = 0x00;
            }
            else
            {
                next = FlipTable[_data[bp + 1] & 0xff];
                next2Next = FlipTable[_data[bp + 2] & 0xff];
            }
        }
        else
        {
            throw new InvalidOperationException("TIFF_FILL_ORDER tag must be either 1 or 2.");
        }

        var bitsLeft = 8 - _bitPointer;
        var bitsFromNextByte = bitsToGet - bitsLeft;
        var bitsFromNext2NextByte = 0;
        if (bitsFromNextByte > 8)
        {
            bitsFromNext2NextByte = bitsFromNextByte - 8;
            bitsFromNextByte = 8;
        }

        _bytePointer++;

        var i1 = (b & _table1[bitsLeft]) << (bitsToGet - bitsLeft);
        var i2 = (next & _table2[bitsFromNextByte]) >> (8 - bitsFromNextByte);

        var i3 = 0;
        if (bitsFromNext2NextByte != 0)
        {
            i2 <<= bitsFromNext2NextByte;
            i3 = (next2Next & _table2[bitsFromNext2NextByte]) >>
                 (8 - bitsFromNext2NextByte);
            i2 |= i3;
            _bytePointer++;
            _bitPointer = bitsFromNext2NextByte;
        }
        else
        {
            if (bitsFromNextByte == 8)
            {
                _bitPointer = 0;
                _bytePointer++;
            }
            else
            {
                _bitPointer = bitsFromNextByte;
            }
        }

        var i = i1 | i2;
        return i;
    }

    private int readEol(bool isFirstEol)
    {
        if (_fillBits == 0)
        {
            var next12Bits = nextNBits(12);
            if (isFirstEol && next12Bits == 0)
            {
                // Might have the case of EOL padding being used even
                // though it was not flagged in the T4Options field.
                // This was observed to be the case in TIFFs produced
                // by a well known vendor who shall remain nameless.

                if (nextNBits(4) == 1)
                {
                    // EOL must be padded: reset the fillBits flag.

                    _fillBits = 1;
                    return 1;
                }
            }

            if (next12Bits != 1)
            {
                throw new InvalidOperationException("Scanline must begin with EOL code word.");
            }
        }
        else if (_fillBits == 1)
        {
            // First EOL code word xxxx 0000 0000 0001 will occur
            // As many fill bits will be present as required to make
            // the EOL code of 12 bits end on a byte boundary.

            var bitsLeft = 8 - _bitPointer;

            if (nextNBits(bitsLeft) != 0)
            {
                throw new InvalidOperationException("All fill bits preceding EOL code must be 0.");
            }

            // If the number of bitsLeft is less than 8, then to have a 12
            // bit EOL sequence, two more bytes are certainly going to be
            // required. The first of them has to be all zeros, so ensure
            // that.
            if (bitsLeft < 4)
            {
                if (nextNBits(8) != 0)
                {
                    throw new InvalidOperationException("All fill bits preceding EOL code must be 0.");
                }
            }

            // There might be a random number of fill bytes with 0s, so
            // loop till the EOL of 0000 0001 is found, as long as all
            // the bytes preceding it are 0's.
            int n;
            while ((n = nextNBits(8)) != 1)
            {
                // If not all zeros
                if (n != 0)
                {
                    throw new InvalidOperationException("All fill bits preceding EOL code must be 0.");
                }
            }
        }

        // If one dimensional encoding mode, then always return 1
        if (_oneD == 0)
        {
            return 1;
        }

        // Otherwise for 2D encoding mode,
        // The next one bit signifies 1D/2D encoding of next line.
        return nextLesserThan8Bits(1);
    }

    private static void setToBlack(byte[] buffer,
                                   int lineOffset, int bitOffset,
                                   int numBits)
    {
        var bitNum = 8 * lineOffset + bitOffset;
        var lastBit = bitNum + numBits;

        var byteNum = bitNum >> 3;

        // Handle bits in first byte
        var shift = bitNum & 0x7;
        if (shift > 0)
        {
            var maskVal = 1 << (7 - shift);
            var val = buffer[byteNum];
            while (maskVal > 0 && bitNum < lastBit)
            {
                val |= (byte)maskVal;
                maskVal >>= 1;
                ++bitNum;
            }

            buffer[byteNum] = val;
        }

        // Fill in 8 bits at a time
        byteNum = bitNum >> 3;
        while (bitNum < lastBit - 7)
        {
            buffer[byteNum++] = 255;
            bitNum += 8;
        }

        // Fill in remaining bits
        while (bitNum < lastBit)
        {
            byteNum = bitNum >> 3;
            buffer[byteNum] |= (byte)(1 << (7 - (bitNum & 0x7)));
            ++bitNum;
        }
    }

    /// <summary>
    ///     Move pointer backwards by given amount of bits
    /// </summary>
    private void updatePointer(int bitsToMoveBack)
    {
        var i = _bitPointer - bitsToMoveBack;

        if (i < 0)
        {
            _bytePointer--;
            _bitPointer = 8 + i;
        }
        else
        {
            _bitPointer = i;
        }
    }
}