using System.Drawing;
using System.util;
using iTextSharp.LGPLv2.Core.System.Drawing;
using iTextSharp.LGPLv2.Core.System.Encodings;
using iTextSharp.text.pdf.codec;
using SkiaSharp;

namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public class BarcodeDatamatrix
{
    /// <summary>
    ///     ASCII encodation.
    /// </summary>
    public const int DM_ASCII = 1;

    /// <summary>
    ///     The best encodation will be used.
    /// </summary>
    public const int DM_AUTO = 0;

    /// <summary>
    ///     Binary encodation.
    /// </summary>
    public const int DM_B256 = 4;

    /// <summary>
    ///     C40 encodation.
    /// </summary>
    public const int DM_C40 = 2;

    /// <summary>
    ///     EDIFACT encodation.
    /// </summary>
    public const int DM_EDIFACT = 6;

    /// <summary>
    ///     An error while parsing an extension.
    /// </summary>
    public const int DM_ERROR_EXTENSION = 5;

    /// <summary>
    ///     The dimensions given for the symbol are illegal.
    /// </summary>
    public const int DM_ERROR_INVALID_SQUARE = 3;

    /// <summary>
    ///     The text is too big for the symbology capabilities.
    /// </summary>
    public const int DM_ERROR_TEXT_TOO_BIG = 1;

    /// <summary>
    ///     Allows extensions to be embedded at the start of the text.
    /// </summary>
    public const int DM_EXTENSION = 32;

    /// <summary>
    ///     No error.
    /// </summary>
    public const int DM_NO_ERROR = 0;

    /// <summary>
    ///     No encodation needed. The bytes provided are already encoded.
    /// </summary>
    public const int DM_RAW = 7;

    /// <summary>
    ///     Doesn't generate the image but returns all the other information.
    /// </summary>
    public const int DM_TEST = 64;

    /// <summary>
    ///     TEXT encodation.
    /// </summary>
    public const int DM_TEXT = 3;

    /// <summary>
    ///     X21 encodation.
    /// </summary>
    public const int DM_X21 = 5;

    private const string X12 = "\r*> 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private static readonly DmParams[] _dmSizes =
    {
        new(10, 10, 10, 10, 3, 3, 5),
        new(12, 12, 12, 12, 5, 5, 7),
        new(8, 18, 8, 18, 5, 5, 7),
        new(14, 14, 14, 14, 8, 8, 10),
        new(8, 32, 8, 16, 10, 10, 11),
        new(16, 16, 16, 16, 12, 12, 12),
        new(12, 26, 12, 26, 16, 16, 14),
        new(18, 18, 18, 18, 18, 18, 14),
        new(20, 20, 20, 20, 22, 22, 18),
        new(12, 36, 12, 18, 22, 22, 18),
        new(22, 22, 22, 22, 30, 30, 20),
        new(16, 36, 16, 18, 32, 32, 24),
        new(24, 24, 24, 24, 36, 36, 24),
        new(26, 26, 26, 26, 44, 44, 28),
        new(16, 48, 16, 24, 49, 49, 28),
        new(32, 32, 16, 16, 62, 62, 36),
        new(36, 36, 18, 18, 86, 86, 42),
        new(40, 40, 20, 20, 114, 114, 48),
        new(44, 44, 22, 22, 144, 144, 56),
        new(48, 48, 24, 24, 174, 174, 68),
        new(52, 52, 26, 26, 204, 102, 42),
        new(64, 64, 16, 16, 280, 140, 56),
        new(72, 72, 18, 18, 368, 92, 36),
        new(80, 80, 20, 20, 456, 114, 48),
        new(88, 88, 22, 22, 576, 144, 56),
        new(96, 96, 24, 24, 696, 174, 68),
        new(104, 104, 26, 26, 816, 136, 56),
        new(120, 120, 20, 20, 1050, 175, 68),
        new(132, 132, 22, 22, 1304, 163, 62),
        new(144, 144, 24, 24, 1558, 156, 62),
    };

    private int _extOut;
    private short[] _place;

    /// <summary>
    ///     Gets the generated image. The image is represented as a stream of bytes, each byte representing
    ///     8 pixels, 0 for white and 1 for black, with the high-order bit of each byte first. Each row
    ///     is aligned at byte boundaries. The dimensions of the image are defined by height and width
    ///     plus 2 * ws.
    /// </summary>
    /// <returns>the generated image</returns>
    public byte[] BitImage { get; private set; }

    /// <summary>
    ///     Gets/sets the height of the barcode. If the height is zero it will be calculated. This height doesn't include the
    ///     whitespace border, if any.
    ///     The allowed dimensions are (height, width):
    ///     10, 10
    ///     12, 12
    ///     8, 18
    ///     14, 14
    ///     8, 32
    ///     16, 16
    ///     12, 26
    ///     18, 18
    ///     20, 20
    ///     12, 36
    ///     22, 22
    ///     16, 36
    ///     24, 24
    ///     26, 26
    ///     16, 48
    ///     32, 32
    ///     36, 36
    ///     40, 40
    ///     44, 44
    ///     48, 48
    ///     52, 52
    ///     64, 64
    ///     72, 72
    ///     80, 80
    ///     88, 88
    ///     96, 96
    ///     104, 104
    ///     120, 120
    ///     132, 132
    ///     144, 144
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    ///     Gets/sets the options for the barcode generation. The options can be:
    ///     One of:
    ///     DM_AUTO  - the best encodation will be used
    ///     DM_ASCII  - ASCII encodation
    ///     DM_C40  - C40 encodation
    ///     DM_TEXT  - TEXT encodation
    ///     DM_B256  - binary encodation
    ///     DM_X21  - X21 encodation
    ///     DM_EDIFACT  - EDIFACT encodation
    ///     DM_RAW  - no encodation. The bytes provided are already encoded and will be added directly to the barcode, using
    ///     padding if needed. It assumes that the encodation state is left at ASCII after the last byte.
    ///     One of:
    ///     DM_EXTENSION  - allows extensions to be embedded at the start of the text:
    ///     exxxxxx - ECI number xxxxxx
    ///     m5 - macro 5
    ///     m6 - macro 6
    ///     f - FNC1
    ///     saabbccccc - Structured Append, aa symbol position (1-16), bb total number of symbols (2-16), ccccc file
    ///     identification (0-64515)
    ///     p - Reader programming
    ///     . - extension terminator
    ///     Example for a structured append, symbol 2 of 6, with FNC1 and ECI 000005. The actual text is "Hello".
    ///     s020600075fe000005.Hello
    ///     One of:
    ///     DM_TEST  - doesn't generate the image but returns all the other information.
    /// </summary>
    public int Options { get; set; }

    /// <summary>
    ///     Gets/sets the width of the barcode. If the width is zero it will be calculated. This width doesn't include the
    ///     whitespace border, if any.
    ///     The allowed dimensions are (height, width):
    ///     10, 10
    ///     12, 12
    ///     8, 18
    ///     14, 14
    ///     8, 32
    ///     16, 16
    ///     12, 26
    ///     18, 18
    ///     20, 20
    ///     12, 36
    ///     22, 22
    ///     16, 36
    ///     24, 24
    ///     26, 26
    ///     16, 48
    ///     32, 32
    ///     36, 36
    ///     40, 40
    ///     44, 44
    ///     48, 48
    ///     52, 52
    ///     64, 64
    ///     72, 72
    ///     80, 80
    ///     88, 88
    ///     96, 96
    ///     104, 104
    ///     120, 120
    ///     132, 132
    ///     144, 144
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    ///     Gets/sets the whitespace border around the barcode.
    /// </summary>
    public int Ws { get; set; }

    /// <summary>
    ///     Creates a  java.awt.Image . A successful call to the method  generate()
    ///     before calling this method is required.
    /// </summary>
    /// <param name="foreground">the color of the bars</param>
    /// <param name="background">the color of the background</param>
    /// <returns>the image</returns>
    public virtual SKBitmap CreateDrawingImage(Color foreground, Color background)
    {
        if (BitImage == null)
        {
            return null;
        }

        var h = Height + 2 * Ws;
        var w = Width + 2 * Ws;
        var stride = (w + 7) / 8;
        var bmp = new SKBitmap(w, h);
        for (var k = 0; k < h; ++k)
        {
            var p = k * stride;
            for (var j = 0; j < w; ++j)
            {
                var b = BitImage[p + j / 8] & 0xff;
                b <<= j % 8;
                bmp.SetPixel(j, k, (b & 0x80) == 0 ? background.ToSKColor() : foreground.ToSKColor());
            }
        }

        return bmp;
    }

    /// <summary>
    ///     Gets an  Image  with the barcode. A successful call to the method  generate()
    ///     before calling this method is required.
    ///     @throws BadElementException on error
    /// </summary>
    /// <returns>the barcode  Image </returns>
    public Image CreateImage()
    {
        if (BitImage == null)
        {
            return null;
        }

        var g4 = Ccittg4Encoder.Compress(BitImage, Width + 2 * Ws, Height + 2 * Ws);
        return Image.GetInstance(Width + 2 * Ws, Height + 2 * Ws, false, Element.CCITTG4, 0, g4, null);
    }

    /// <summary>
    ///     Creates a barcode. The  String  is interpreted with the ISO-8859-1 encoding
    ///     DM_NO_ERROR  - no error.
    ///     DM_ERROR_TEXT_TOO_BIG  - the text is too big for the symbology capabilities.
    ///     DM_ERROR_INVALID_SQUARE  - the dimensions given for the symbol are illegal.
    ///     DM_ERROR_EXTENSION  - an error was while parsing an extension.
    ///     @throws java.io.UnsupportedEncodingException on error
    /// </summary>
    /// <param name="text">the text</param>
    /// <returns>the status of the generation. It can be one of this values:</returns>
    public int Generate(string text)
    {
        var t = EncodingsRegistry.GetEncoding(1252).GetBytes(text);
        return Generate(t, 0, t.Length);
    }

    /// <summary>
    ///     Creates a barcode.
    ///     DM_NO_ERROR  - no error.
    ///     DM_ERROR_TEXT_TOO_BIG  - the text is too big for the symbology capabilities.
    ///     DM_ERROR_INVALID_SQUARE  - the dimensions given for the symbol are illegal.
    ///     DM_ERROR_EXTENSION  - an error was while parsing an extension.
    /// </summary>
    /// <param name="text">the text</param>
    /// <param name="textOffset">the offset to the start of the text</param>
    /// <param name="textSize">the text size</param>
    /// <returns>the status of the generation. It can be one of this values:</returns>
    public int Generate(byte[] text, int textOffset, int textSize)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        int extCount, e, k, full;
        DmParams dm, last;
        var data = new byte[2500];
        _extOut = 0;
        extCount = processExtensions(text, textOffset, textSize, data);
        if (extCount < 0)
        {
            return DM_ERROR_EXTENSION;
        }

        e = -1;
        if (Height == 0 || Width == 0)
        {
            last = _dmSizes[_dmSizes.Length - 1];
            e = getEncodation(text, textOffset + _extOut, textSize - _extOut, data, extCount, last.DataSize - extCount,
                              Options, false);
            if (e < 0)
            {
                return DM_ERROR_TEXT_TOO_BIG;
            }

            e += extCount;
            for (k = 0; k < _dmSizes.Length; ++k)
            {
                if (_dmSizes[k].DataSize >= e)
                {
                    break;
                }
            }

            dm = _dmSizes[k];
            Height = dm.height;
            Width = dm.width;
        }
        else
        {
            for (k = 0; k < _dmSizes.Length; ++k)
            {
                if (Height == _dmSizes[k].height && Width == _dmSizes[k].width)
                {
                    break;
                }
            }

            if (k == _dmSizes.Length)
            {
                return DM_ERROR_INVALID_SQUARE;
            }

            dm = _dmSizes[k];
            e = getEncodation(text, textOffset + _extOut, textSize - _extOut, data, extCount, dm.DataSize - extCount,
                              Options, true);
            if (e < 0)
            {
                return DM_ERROR_TEXT_TOO_BIG;
            }

            e += extCount;
        }

        if ((Options & DM_TEST) != 0)
        {
            return DM_NO_ERROR;
        }

        BitImage = new byte[(dm.width + 2 * Ws + 7) / 8 * (dm.height + 2 * Ws)];
        makePadding(data, e, dm.DataSize - e);
        _place = Placement.DoPlacement(dm.height - dm.height / dm.HeightSection * 2,
                                       dm.width - dm.width / dm.WidthSection * 2);
        full = dm.DataSize + (dm.DataSize + 2) / dm.DataBlock * dm.ErrorBlock;
        ReedSolomon.GenerateEcc(data, dm.DataSize, dm.DataBlock, dm.ErrorBlock);
        draw(data, full, dm);
        return DM_NO_ERROR;
    }

    private static int asciiEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset,
                                       int dataLength)
    {
        int ptrIn, ptrOut, c;
        ptrIn = textOffset;
        ptrOut = dataOffset;
        textLength += textOffset;
        dataLength += dataOffset;
        while (ptrIn < textLength)
        {
            if (ptrOut >= dataLength)
            {
                return -1;
            }

            c = text[ptrIn++] & 0xff;
            if (isDigit(c) && ptrIn < textLength && isDigit(text[ptrIn] & 0xff))
            {
                data[ptrOut++] = (byte)((c - '0') * 10 + (text[ptrIn++] & 0xff) - '0' + 130);
            }
            else if (c > 127)
            {
                if (ptrOut + 1 >= dataLength)
                {
                    return -1;
                }

                data[ptrOut++] = 235;
                data[ptrOut++] = (byte)(c - 128 + 1);
            }
            else
            {
                data[ptrOut++] = (byte)(c + 1);
            }
        }

        return ptrOut - dataOffset;
    }

    private static int b256Encodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset,
                                      int dataLength)
    {
        int k, j, prn, tv, c;
        if (textLength == 0)
        {
            return 0;
        }

        if (textLength < 250 && textLength + 2 > dataLength)
        {
            return -1;
        }

        if (textLength >= 250 && textLength + 3 > dataLength)
        {
            return -1;
        }

        data[dataOffset] = 231;
        if (textLength < 250)
        {
            data[dataOffset + 1] = (byte)textLength;
            k = 2;
        }
        else
        {
            data[dataOffset + 1] = (byte)(textLength / 250 + 249);
            data[dataOffset + 2] = (byte)(textLength % 250);
            k = 3;
        }

        Array.Copy(text, textOffset, data, k + dataOffset, textLength);
        k += textLength + dataOffset;
        for (j = dataOffset + 1; j < k; ++j)
        {
            c = data[j] & 0xff;
            prn = 149 * (j + 1) % 255 + 1;
            tv = c + prn;
            if (tv > 255)
            {
                tv -= 256;
            }

            data[j] = (byte)tv;
        }

        return k - dataOffset;
    }

    private static int c40OrTextEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset,
                                           int dataLength, bool c40)
    {
        int ptrIn, ptrOut, encPtr, last0, last1, i, a, c;
        string basic, shift2, shift3;
        if (textLength == 0)
        {
            return 0;
        }

        ptrIn = 0;
        ptrOut = 0;
        if (c40)
        {
            data[dataOffset + ptrOut++] = 230;
        }
        else
        {
            data[dataOffset + ptrOut++] = 239;
        }

        shift2 = "!\"#$%&'()*+,-./:;<=>?@[\\]^_";
        if (c40)
        {
            basic = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            shift3 = "`abcdefghijklmnopqrstuvwxyz{|}~\u007f";
        }
        else
        {
            basic = " 0123456789abcdefghijklmnopqrstuvwxyz";
            shift3 = "`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~\u007f";
        }

        var enc = new int[textLength * 4 + 10];
        encPtr = 0;
        last0 = 0;
        last1 = 0;
        while (ptrIn < textLength)
        {
            if (encPtr % 3 == 0)
            {
                last0 = ptrIn;
                last1 = encPtr;
            }

            c = text[textOffset + ptrIn++] & 0xff;
            if (c > 127)
            {
                c -= 128;
                enc[encPtr++] = 1;
                enc[encPtr++] = 30;
            }

            var idx = basic.IndexOf(((char)c).ToString(), StringComparison.Ordinal);
            if (idx >= 0)
            {
                enc[encPtr++] = idx + 3;
            }
            else if (c < 32)
            {
                enc[encPtr++] = 0;
                enc[encPtr++] = c;
            }
            else if ((idx = shift2.IndexOf(((char)c).ToString(), StringComparison.Ordinal)) >= 0)
            {
                enc[encPtr++] = 1;
                enc[encPtr++] = idx;
            }
            else if ((idx = shift3.IndexOf(((char)c).ToString(), StringComparison.Ordinal)) >= 0)
            {
                enc[encPtr++] = 2;
                enc[encPtr++] = idx;
            }
        }

        if (encPtr % 3 != 0)
        {
            ptrIn = last0;
            encPtr = last1;
        }

        if (encPtr / 3 * 2 > dataLength - 2)
        {
            return -1;
        }

        i = 0;
        for (; i < encPtr; i += 3)
        {
            a = 1600 * enc[i] + 40 * enc[i + 1] + enc[i + 2] + 1;
            data[dataOffset + ptrOut++] = (byte)(a / 256);
            data[dataOffset + ptrOut++] = (byte)a;
        }

        data[ptrOut++] = 254;
        i = asciiEncodation(text, ptrIn, textLength - ptrIn, data, ptrOut, dataLength - ptrOut);
        if (i < 0)
        {
            return i;
        }

        return ptrOut + i;
    }

    private static int edifactEncodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset,
                                         int dataLength)
    {
        int ptrIn, ptrOut, edi, pedi, c;
        if (textLength == 0)
        {
            return 0;
        }

        ptrIn = 0;
        ptrOut = 0;
        edi = 0;
        pedi = 18;
        var ascii = true;
        for (; ptrIn < textLength; ++ptrIn)
        {
            c = text[ptrIn + textOffset] & 0xff;
            if (((c & 0xe0) == 0x40 || (c & 0xe0) == 0x20) && c != '_')
            {
                if (ascii)
                {
                    if (ptrOut + 1 > dataLength)
                    {
                        break;
                    }

                    data[dataOffset + ptrOut++] = 240;
                    ascii = false;
                }

                c &= 0x3f;
                edi |= c << pedi;
                if (pedi == 0)
                {
                    if (ptrOut + 3 > dataLength)
                    {
                        break;
                    }

                    data[dataOffset + ptrOut++] = (byte)(edi >> 16);
                    data[dataOffset + ptrOut++] = (byte)(edi >> 8);
                    data[dataOffset + ptrOut++] = (byte)edi;
                    edi = 0;
                    pedi = 18;
                }
                else
                {
                    pedi -= 6;
                }
            }
            else
            {
                if (!ascii)
                {
                    edi |= ('_' & 0x3f) << pedi;
                    if (ptrOut + (3 - pedi / 8) > dataLength)
                    {
                        break;
                    }

                    data[dataOffset + ptrOut++] = (byte)(edi >> 16);
                    if (pedi <= 12)
                    {
                        data[dataOffset + ptrOut++] = (byte)(edi >> 8);
                    }

                    if (pedi <= 6)
                    {
                        data[dataOffset + ptrOut++] = (byte)edi;
                    }

                    ascii = true;
                    pedi = 18;
                    edi = 0;
                }

                if (c > 127)
                {
                    if (ptrOut >= dataLength)
                    {
                        break;
                    }

                    data[dataOffset + ptrOut++] = 235;
                    c -= 128;
                }

                if (ptrOut >= dataLength)
                {
                    break;
                }

                data[dataOffset + ptrOut++] = (byte)(c + 1);
            }
        }

        if (ptrIn != textLength)
        {
            return -1;
        }

        if (!ascii)
        {
            edi |= ('_' & 0x3f) << pedi;
            if (ptrOut + (3 - pedi / 8) > dataLength)
            {
                return -1;
            }

            data[dataOffset + ptrOut++] = (byte)(edi >> 16);
            if (pedi <= 12)
            {
                data[dataOffset + ptrOut++] = (byte)(edi >> 8);
            }

            if (pedi <= 6)
            {
                data[dataOffset + ptrOut++] = (byte)edi;
            }
        }

        return ptrOut;
    }

    private static int getEncodation(byte[] text, int textOffset, int textSize, byte[] data, int dataOffset,
                                     int dataSize, int options, bool firstMatch)
    {
        int e, j, k;
        var e1 = new int[6];
        if (dataSize < 0)
        {
            return -1;
        }

        e = -1;
        options &= 7;
        if (options == 0)
        {
            e1[0] = asciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
            if (firstMatch && e1[0] >= 0)
            {
                return e1[0];
            }

            e1[1] = c40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
            if (firstMatch && e1[1] >= 0)
            {
                return e1[1];
            }

            e1[2] = c40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
            if (firstMatch && e1[2] >= 0)
            {
                return e1[2];
            }

            e1[3] = b256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
            if (firstMatch && e1[3] >= 0)
            {
                return e1[3];
            }

            e1[4] = x12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
            if (firstMatch && e1[4] >= 0)
            {
                return e1[4];
            }

            e1[5] = edifactEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
            if (firstMatch && e1[5] >= 0)
            {
                return e1[5];
            }

            if (e1[0] < 0 && e1[1] < 0 && e1[2] < 0 && e1[3] < 0 && e1[4] < 0 && e1[5] < 0)
            {
                return -1;
            }

            j = 0;
            e = 99999;
            for (k = 0; k < 6; ++k)
            {
                if (e1[k] >= 0 && e1[k] < e)
                {
                    e = e1[k];
                    j = k;
                }
            }

            if (j == 0)
            {
                e = asciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
            }
            else if (j == 1)
            {
                e = c40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
            }
            else if (j == 2)
            {
                e = c40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
            }
            else if (j == 3)
            {
                e = b256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
            }
            else if (j == 4)
            {
                e = x12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
            }

            return e;
        }

        switch (options)
        {
            case DM_ASCII:
                return asciiEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
            case DM_C40:
                return c40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, true);
            case DM_TEXT:
                return c40OrTextEncodation(text, textOffset, textSize, data, dataOffset, dataSize, false);
            case DM_B256:
                return b256Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
            case DM_X21:
                return x12Encodation(text, textOffset, textSize, data, dataOffset, dataSize);
            case DM_EDIFACT:
                return edifactEncodation(text, textOffset, textSize, data, dataOffset, dataSize);
            case DM_RAW:
                if (textSize > dataSize)
                {
                    return -1;
                }

                Array.Copy(text, textOffset, data, dataOffset, textSize);
                return textSize;
        }

        return -1;
    }

    private static int getNumber(byte[] text, int ptrIn, int n)
    {
        int v, j, c;
        v = 0;
        for (j = 0; j < n; ++j)
        {
            c = text[ptrIn++] & 0xff;
            if (c < '0' || c > '9')
            {
                return -1;
            }

            v = v * 10 + c - '0';
        }

        return v;
    }

    private static bool isDigit(int c) => c >= '0' && c <= '9';

    private static void makePadding(byte[] data, int position, int count)
    {
        //already in ascii mode
        if (count <= 0)
        {
            return;
        }

        data[position++] = 129;
        while (--count > 0)
        {
            var t = 129 + (position + 1) * 149 % 253 + 1;
            if (t > 254)
            {
                t -= 254;
            }

            data[position++] = (byte)t;
        }
    }

    private static int x12Encodation(byte[] text, int textOffset, int textLength, byte[] data, int dataOffset,
                                     int dataLength)
    {
        int ptrIn, ptrOut, count, k, n, ci;
        byte c;
        if (textLength == 0)
        {
            return 0;
        }

        ptrIn = 0;
        ptrOut = 0;
        var x = new byte[textLength];
        count = 0;
        for (; ptrIn < textLength; ++ptrIn)
        {
            var i = X12.IndexOf(((char)text[ptrIn + textOffset]).ToString(), StringComparison.Ordinal);
            if (i >= 0)
            {
                x[ptrIn] = (byte)i;
                ++count;
            }
            else
            {
                x[ptrIn] = 100;
                if (count >= 6)
                {
                    count -= count / 3 * 3;
                }

                for (k = 0; k < count; ++k)
                {
                    x[ptrIn - k - 1] = 100;
                }

                count = 0;
            }
        }

        if (count >= 6)
        {
            count -= count / 3 * 3;
        }

        for (k = 0; k < count; ++k)
        {
            x[ptrIn - k - 1] = 100;
        }

        ptrIn = 0;
        c = 0;
        for (; ptrIn < textLength; ++ptrIn)
        {
            c = x[ptrIn];
            if (ptrOut >= dataLength)
            {
                break;
            }

            if (c < 40)
            {
                if (ptrIn == 0 || (ptrIn > 0 && x[ptrIn - 1] > 40))
                {
                    data[dataOffset + ptrOut++] = 238;
                }

                if (ptrOut + 2 > dataLength)
                {
                    break;
                }

                n = 1600 * x[ptrIn] + 40 * x[ptrIn + 1] + x[ptrIn + 2] + 1;
                data[dataOffset + ptrOut++] = (byte)(n / 256);
                data[dataOffset + ptrOut++] = (byte)n;
                ptrIn += 2;
            }
            else
            {
                if (ptrIn > 0 && x[ptrIn - 1] < 40)
                {
                    data[dataOffset + ptrOut++] = 254;
                }

                ci = text[ptrIn + textOffset] & 0xff;
                if (ci > 127)
                {
                    data[dataOffset + ptrOut++] = 235;
                    ci -= 128;
                }

                if (ptrOut >= dataLength)
                {
                    break;
                }

                data[dataOffset + ptrOut++] = (byte)(ci + 1);
            }
        }

        c = 100;
        if (textLength > 0)
        {
            c = x[textLength - 1];
        }

        if (ptrIn != textLength || (c < 40 && ptrOut >= dataLength))
        {
            return -1;
        }

        if (c < 40)
        {
            data[dataOffset + ptrOut++] = 254;
        }

        return ptrOut;
    }

    private void draw(byte[] data, int dataSize, DmParams dm)
    {
        int i, j, p, x, y, xs, ys, z;
        var xByte = (dm.width + Ws * 2 + 7) / 8;
        for (var k = 0; k < BitImage.Length; ++k)
        {
            BitImage[k] = 0;
        }

        //alignment patterns
        //dotted horizontal line
        for (i = Ws; i < dm.height + Ws; i += dm.HeightSection)
        {
            for (j = Ws; j < dm.width + Ws; j += 2)
            {
                setBit(j, i, xByte);
            }
        }

        //solid horizontal line
        for (i = dm.HeightSection - 1 + Ws; i < dm.height + Ws; i += dm.HeightSection)
        {
            for (j = Ws; j < dm.width + Ws; ++j)
            {
                setBit(j, i, xByte);
            }
        }

        //solid vertical line
        for (i = Ws; i < dm.width + Ws; i += dm.WidthSection)
        {
            for (j = Ws; j < dm.height + Ws; ++j)
            {
                setBit(i, j, xByte);
            }
        }

        //dotted vertical line
        for (i = dm.WidthSection - 1 + Ws; i < dm.width + Ws; i += dm.WidthSection)
        {
            for (j = 1 + Ws; j < dm.height + Ws; j += 2)
            {
                setBit(i, j, xByte);
            }
        }

        p = 0;
        for (ys = 0; ys < dm.height; ys += dm.HeightSection)
        {
            for (y = 1; y < dm.HeightSection - 1; ++y)
            {
                for (xs = 0; xs < dm.width; xs += dm.WidthSection)
                {
                    for (x = 1; x < dm.WidthSection - 1; ++x)
                    {
                        z = _place[p++];
                        if (z == 1 || (z > 1 && (data[z / 8 - 1] & 0xff & (128 >> (z % 8))) != 0))
                        {
                            setBit(x + xs + Ws, y + ys + Ws, xByte);
                        }
                    }
                }
            }
        }
    }

    private int processExtensions(byte[] text, int textOffset, int textSize, byte[] data)
    {
        int order, ptrIn, ptrOut, eci, fn, ft, fi, c;
        if ((Options & DM_EXTENSION) == 0)
        {
            return 0;
        }

        order = 0;
        ptrIn = 0;
        ptrOut = 0;
        while (ptrIn < textSize)
        {
            if (order > 20)
            {
                return -1;
            }

            c = text[textOffset + ptrIn++] & 0xff;
            ++order;
            switch (c)
            {
                case '.':
                    _extOut = ptrIn;
                    return ptrOut;
                case 'e':
                    if (ptrIn + 6 > textSize)
                    {
                        return -1;
                    }

                    eci = getNumber(text, textOffset + ptrIn, 6);
                    if (eci < 0)
                    {
                        return -1;
                    }

                    ptrIn += 6;
                    data[ptrOut++] = 241;
                    if (eci < 127)
                    {
                        data[ptrOut++] = (byte)(eci + 1);
                    }
                    else if (eci < 16383)
                    {
                        data[ptrOut++] = (byte)((eci - 127) / 254 + 128);
                        data[ptrOut++] = (byte)((eci - 127) % 254 + 1);
                    }
                    else
                    {
                        data[ptrOut++] = (byte)((eci - 16383) / 64516 + 192);
                        data[ptrOut++] = (byte)((eci - 16383) / 254 % 254 + 1);
                        data[ptrOut++] = (byte)((eci - 16383) % 254 + 1);
                    }

                    break;
                case 's':
                    if (order != 1)
                    {
                        return -1;
                    }

                    if (ptrIn + 9 > textSize)
                    {
                        return -1;
                    }

                    fn = getNumber(text, textOffset + ptrIn, 2);
                    if (fn <= 0 || fn > 16)
                    {
                        return -1;
                    }

                    ptrIn += 2;
                    ft = getNumber(text, textOffset + ptrIn, 2);
                    if (ft <= 1 || ft > 16)
                    {
                        return -1;
                    }

                    ptrIn += 2;
                    fi = getNumber(text, textOffset + ptrIn, 5);
                    if (fi < 0 || fn >= 64516)
                    {
                        return -1;
                    }

                    ptrIn += 5;
                    data[ptrOut++] = 233;
                    data[ptrOut++] = (byte)(((fn - 1) << 4) | (17 - ft));
                    data[ptrOut++] = (byte)(fi / 254 + 1);
                    data[ptrOut++] = (byte)(fi % 254 + 1);
                    break;
                case 'p':
                    if (order != 1)
                    {
                        return -1;
                    }

                    data[ptrOut++] = 234;
                    break;
                case 'm':
                    if (order != 1)
                    {
                        return -1;
                    }

                    if (ptrIn + 1 > textSize)
                    {
                        return -1;
                    }

                    c = text[textOffset + ptrIn++] & 0xff;
                    if (c != '5' && c != '5')
                    {
                        return -1;
                    }

                    data[ptrOut++] = 234;
                    data[ptrOut++] = 236;
                    break;
                case 'f':
                    if (order != 1 && (order != 2 || (text[textOffset] != 's' && text[textOffset] != 'm')))
                    {
                        return -1;
                    }

                    data[ptrOut++] = 232;
                    break;
            }
        }

        return -1;
    }

    private void setBit(int x, int y, int xByte)
    {
        BitImage[y * xByte + x / 8] |= (byte)(128 >> (x & 7));
    }

    internal class Placement
    {
        private static readonly INullValueDictionary<int, short[]> _cache = new NullValueDictionary<int, short[]>();
        private short[] _array;
        private int _ncol;
        private int _nrow;

        private Placement()
        {
        }

        internal static short[] DoPlacement(int nrow, int ncol)
        {
            var key = nrow * 1000 + ncol;
            var pc = _cache[key];
            if (pc != null)
            {
                return pc;
            }

            var p = new Placement();
            p._nrow = nrow;
            p._ncol = ncol;
            p._array = new short[nrow * ncol];
            p.ecc200();
            _cache[key] = p._array;
            return p._array;
        }

        /// <summary>
        ///     "cornerN" places 8 bits of the four special corner cases in ECC200
        /// </summary>
        private void corner1(int chr)
        {
            module(_nrow - 1, 0, chr, 0);
            module(_nrow - 1, 1, chr, 1);
            module(_nrow - 1, 2, chr, 2);
            module(0, _ncol - 2, chr, 3);
            module(0, _ncol - 1, chr, 4);
            module(1, _ncol - 1, chr, 5);
            module(2, _ncol - 1, chr, 6);
            module(3, _ncol - 1, chr, 7);
        }

        private void corner2(int chr)
        {
            module(_nrow - 3, 0, chr, 0);
            module(_nrow - 2, 0, chr, 1);
            module(_nrow - 1, 0, chr, 2);
            module(0, _ncol - 4, chr, 3);
            module(0, _ncol - 3, chr, 4);
            module(0, _ncol - 2, chr, 5);
            module(0, _ncol - 1, chr, 6);
            module(1, _ncol - 1, chr, 7);
        }

        private void corner3(int chr)
        {
            module(_nrow - 3, 0, chr, 0);
            module(_nrow - 2, 0, chr, 1);
            module(_nrow - 1, 0, chr, 2);
            module(0, _ncol - 2, chr, 3);
            module(0, _ncol - 1, chr, 4);
            module(1, _ncol - 1, chr, 5);
            module(2, _ncol - 1, chr, 6);
            module(3, _ncol - 1, chr, 7);
        }

        private void corner4(int chr)
        {
            module(_nrow - 1, 0, chr, 0);
            module(_nrow - 1, _ncol - 1, chr, 1);
            module(0, _ncol - 3, chr, 2);
            module(0, _ncol - 2, chr, 3);
            module(0, _ncol - 1, chr, 4);
            module(1, _ncol - 3, chr, 5);
            module(1, _ncol - 2, chr, 6);
            module(1, _ncol - 1, chr, 7);
        }

        /// <summary>
        ///     "ECC200" fills an nrow x ncol array with appropriate values for ECC200
        /// </summary>
        private void ecc200()
        {
            int row, col, chr;
            /* First, fill the array[] with invalid entries */
            for (var k = 0; k < _array.Length; ++k)
            {
                _array[k] = 0;
            }

            /* Starting in the correct location for character #1, bit 8,... */
            chr = 1;
            row = 4;
            col = 0;
            do
            {
                /* repeatedly first check for one of the special corner cases, then... */
                if (row == _nrow && col == 0)
                {
                    corner1(chr++);
                }

                if (row == _nrow - 2 && col == 0 && _ncol % 4 != 0)
                {
                    corner2(chr++);
                }

                if (row == _nrow - 2 && col == 0 && _ncol % 8 == 4)
                {
                    corner3(chr++);
                }

                if (row == _nrow + 4 && col == 2 && _ncol % 8 == 0)
                {
                    corner4(chr++);
                }

                /* sweep upward diagonally, inserting successive characters,... */
                do
                {
                    if (row < _nrow && col >= 0 && _array[row * _ncol + col] == 0)
                    {
                        utah(row, col, chr++);
                    }

                    row -= 2;
                    col += 2;
                } while (row >= 0 && col < _ncol);

                row += 1;
                col += 3;
                /* & then sweep downward diagonally, inserting successive characters,... */

                do
                {
                    if (row >= 0 && col < _ncol && _array[row * _ncol + col] == 0)
                    {
                        utah(row, col, chr++);
                    }

                    row += 2;
                    col -= 2;
                } while (row < _nrow && col >= 0);

                row += 3;
                col += 1;
                /* ... until the entire array is scanned */
            } while (row < _nrow || col < _ncol);

            /* Lastly, if the lower righthand corner is untouched, fill in fixed pattern */
            if (_array[_nrow * _ncol - 1] == 0)
            {
                _array[_nrow * _ncol - 1] = _array[_nrow * _ncol - _ncol - 2] = 1;
            }
        }

        /// <summary>
        ///     "module" places "chr+bit" with appropriate wrapping within array[]
        /// </summary>
        private void module(int row, int col, int chr, int bit)
        {
            if (row < 0)
            {
                row += _nrow;
                col += 4 - (_nrow + 4) % 8;
            }

            if (col < 0)
            {
                col += _ncol;
                row += 4 - (_ncol + 4) % 8;
            }

            _array[row * _ncol + col] = (short)(8 * chr + bit);
        }

        /// <summary>
        ///     "utah" places the 8 bits of a utah-shaped symbol character in ECC200
        /// </summary>
        private void utah(int row, int col, int chr)
        {
            module(row - 2, col - 2, chr, 0);
            module(row - 2, col - 1, chr, 1);
            module(row - 1, col - 2, chr, 2);
            module(row - 1, col - 1, chr, 3);
            module(row - 1, col, chr, 4);
            module(row, col - 2, chr, 5);
            module(row, col - 1, chr, 6);
            module(row, col, chr, 7);
        }
    }

    internal class ReedSolomon
    {
        private static readonly int[] _alog =
        {
            1, 2, 4, 8, 16, 32, 64, 128, 45, 90, 180, 69, 138, 57, 114, 228,
            229, 231, 227, 235, 251, 219, 155, 27, 54, 108, 216, 157, 23, 46, 92,
            184,
            93, 186, 89, 178, 73, 146, 9, 18, 36, 72, 144, 13, 26, 52, 104, 208,
            141, 55, 110, 220, 149, 7, 14, 28, 56, 112, 224, 237, 247, 195, 171,
            123,
            246, 193, 175, 115, 230, 225, 239, 243, 203, 187, 91, 182, 65, 130,
            41, 82,
            164, 101, 202, 185, 95, 190, 81, 162, 105, 210, 137, 63, 126, 252,
            213, 135,
            35, 70, 140, 53, 106, 212, 133, 39, 78, 156, 21, 42, 84, 168, 125,
            250,
            217, 159, 19, 38, 76, 152, 29, 58, 116, 232, 253, 215, 131, 43, 86,
            172,
            117, 234, 249, 223, 147, 11, 22, 44, 88, 176, 77, 154, 25, 50, 100,
            200,
            189, 87, 174, 113, 226, 233, 255, 211, 139, 59, 118, 236, 245, 199,
            163, 107,
            214, 129, 47, 94, 188, 85, 170, 121, 242, 201, 191, 83, 166, 97, 194,
            169,
            127, 254, 209, 143, 51, 102, 204, 181, 71, 142, 49, 98, 196, 165, 103,
            206,
            177, 79, 158, 17, 34, 68, 136, 61, 122, 244, 197, 167, 99, 198, 161,
            111,
            222, 145, 15, 30, 60, 120, 240, 205, 183, 67, 134, 33, 66, 132, 37,
            74,
            148, 5, 10, 20, 40, 80, 160, 109, 218, 153, 31, 62, 124, 248, 221,
            151,
            3, 6, 12, 24, 48, 96, 192, 173, 119, 238, 241, 207, 179, 75, 150, 1,
        };

        private static readonly int[] _log =
        {
            0, 255, 1, 240, 2, 225, 241, 53, 3, 38, 226, 133, 242, 43, 54, 210,
            4, 195, 39, 114, 227, 106, 134, 28, 243, 140, 44, 23, 55, 118, 211,
            234,
            5, 219, 196, 96, 40, 222, 115, 103, 228, 78, 107, 125, 135, 8, 29, 162,
            244, 186, 141, 180, 45, 99, 24, 49, 56, 13, 119, 153, 212, 199, 235,
            91,
            6, 76, 220, 217, 197, 11, 97, 184, 41, 36, 223, 253, 116, 138, 104,
            193,
            229, 86, 79, 171, 108, 165, 126, 145, 136, 34, 9, 74, 30, 32, 163, 84,
            245, 173, 187, 204, 142, 81, 181, 190, 46, 88, 100, 159, 25, 231, 50,
            207,
            57, 147, 14, 67, 120, 128, 154, 248, 213, 167, 200, 63, 236, 110, 92,
            176,
            7, 161, 77, 124, 221, 102, 218, 95, 198, 90, 12, 152, 98, 48, 185, 179,
            42, 209, 37, 132, 224, 52, 254, 239, 117, 233, 139, 22, 105, 27, 194,
            113,
            230, 206, 87, 158, 80, 189, 172, 203, 109, 175, 166, 62, 127, 247, 146,
            66,
            137, 192, 35, 252, 10, 183, 75, 216, 31, 83, 33, 73, 164, 144, 85, 170,
            246, 65, 174, 61, 188, 202, 205, 157, 143, 169, 82, 72, 182, 215, 191,
            251,
            47, 178, 89, 151, 101, 94, 160, 123, 26, 112, 232, 21, 51, 238, 208,
            131,
            58, 69, 148, 18, 15, 16, 68, 17, 121, 149, 129, 19, 155, 59, 249, 70,
            214, 250, 168, 71, 201, 156, 64, 60, 237, 130, 111, 20, 93, 122, 177,
            150,
        };

        private static readonly int[] _poly10 =
        {
            28, 24, 185, 166, 223, 248, 116, 255, 110, 61,
        };

        private static readonly int[] _poly11 =
        {
            175, 138, 205, 12, 194, 168, 39, 245, 60, 97, 120,
        };

        private static readonly int[] _poly12 =
        {
            41, 153, 158, 91, 61, 42, 142, 213, 97, 178, 100, 242,
        };

        private static readonly int[] _poly14 =
        {
            156, 97, 192, 252, 95, 9, 157, 119, 138, 45, 18, 186, 83, 185,
        };

        private static readonly int[] _poly18 =
        {
            83, 195, 100, 39, 188, 75, 66, 61, 241, 213, 109, 129, 94, 254, 225,
            48,
            90, 188,
        };

        private static readonly int[] _poly20 =
        {
            15, 195, 244, 9, 233, 71, 168, 2, 188, 160, 153, 145, 253, 79, 108,
            82,
            27, 174, 186, 172,
        };

        private static readonly int[] _poly24 =
        {
            52, 190, 88, 205, 109, 39, 176, 21, 155, 197, 251, 223, 155, 21, 5,
            172,
            254, 124, 12, 181, 184, 96, 50, 193,
        };

        private static readonly int[] _poly28 =
        {
            211, 231, 43, 97, 71, 96, 103, 174, 37, 151, 170, 53, 75, 34, 249,
            121,
            17, 138, 110, 213, 141, 136, 120, 151, 233, 168, 93, 255,
        };

        private static readonly int[] _poly36 =
        {
            245, 127, 242, 218, 130, 250, 162, 181, 102, 120, 84, 179, 220, 251,
            80, 182,
            229, 18, 2, 4, 68, 33, 101, 137, 95, 119, 115, 44, 175, 184, 59, 25,
            225, 98, 81, 112,
        };

        private static readonly int[] _poly42 =
        {
            77, 193, 137, 31, 19, 38, 22, 153, 247, 105, 122, 2, 245, 133, 242,
            8,
            175, 95, 100, 9, 167, 105, 214, 111, 57, 121, 21, 1, 253, 57, 54,
            101,
            248, 202, 69, 50, 150, 177, 226, 5, 9, 5,
        };

        private static readonly int[] _poly48 =
        {
            245, 132, 172, 223, 96, 32, 117, 22, 238, 133, 238, 231, 205, 188,
            237, 87,
            191, 106, 16, 147, 118, 23, 37, 90, 170, 205, 131, 88, 120, 100, 66,
            138,
            186, 240, 82, 44, 176, 87, 187, 147, 160, 175, 69, 213, 92, 253,
            225, 19,
        };

        private static readonly int[] _poly5 =
        {
            228, 48, 15, 111, 62,
        };

        private static readonly int[] _poly56 =
        {
            175, 9, 223, 238, 12, 17, 220, 208, 100, 29, 175, 170, 230, 192,
            215, 235,
            150, 159, 36, 223, 38, 200, 132, 54, 228, 146, 218, 234, 117, 203,
            29, 232,
            144, 238, 22, 150, 201, 117, 62, 207, 164, 13, 137, 245, 127, 67,
            247, 28,
            155, 43, 203, 107, 233, 53, 143, 46,
        };

        private static readonly int[] _poly62 =
        {
            242, 93, 169, 50, 144, 210, 39, 118, 202, 188, 201, 189, 143, 108,
            196, 37,
            185, 112, 134, 230, 245, 63, 197, 190, 250, 106, 185, 221, 175, 64,
            114, 71,
            161, 44, 147, 6, 27, 218, 51, 63, 87, 10, 40, 130, 188, 17, 163, 31,
            176, 170, 4, 107, 232, 7, 94, 166, 224, 124, 86, 47, 11, 204,
        };

        private static readonly int[] _poly68 =
        {
            220, 228, 173, 89, 251, 149, 159, 56, 89, 33, 147, 244, 154, 36, 73,
            127,
            213, 136, 248, 180, 234, 197, 158, 177, 68, 122, 93, 213, 15, 160,
            227, 236,
            66, 139, 153, 185, 202, 167, 179, 25, 220, 232, 96, 210, 231, 136,
            223, 239,
            181, 241, 59, 52, 172, 25, 49, 232, 211, 189, 64, 54, 108, 153, 132,
            63,
            96, 103, 82, 186,
        };

        private static readonly int[] _poly7 =
        {
            23, 68, 144, 134, 240, 92, 254,
        };

        internal static void GenerateEcc(byte[] wd, int nd, int datablock, int nc)
        {
            var blocks = (nd + 2) / datablock;
            int b;
            var buf = new byte[256];
            var ecc = new byte[256];
            var c = getPoly(nc);
            for (b = 0; b < blocks; b++)
            {
                int n, p = 0;
                for (n = b; n < nd; n += blocks)
                {
                    buf[p++] = wd[n];
                }

                reedSolomonBlock(buf, p, ecc, nc, c);
                p = 0;
                for (n = b; n < nc * blocks; n += blocks)
                {
                    wd[nd + n] = ecc[p++];
                }
            }
        }

        private static int[] getPoly(int nc)
        {
            switch (nc)
            {
                case 5:
                    return _poly5;
                case 7:
                    return _poly7;
                case 10:
                    return _poly10;
                case 11:
                    return _poly11;
                case 12:
                    return _poly12;
                case 14:
                    return _poly14;
                case 18:
                    return _poly18;
                case 20:
                    return _poly20;
                case 24:
                    return _poly24;
                case 28:
                    return _poly28;
                case 36:
                    return _poly36;
                case 42:
                    return _poly42;
                case 48:
                    return _poly48;
                case 56:
                    return _poly56;
                case 62:
                    return _poly62;
                case 68:
                    return _poly68;
            }

            return null;
        }

        private static void reedSolomonBlock(byte[] wd, int nd, byte[] ncout, int nc, int[] c)
        {
            int i, j, k;

            for (i = 0; i <= nc; i++)
            {
                ncout[i] = 0;
            }

            for (i = 0; i < nd; i++)
            {
                k = (ncout[0] ^ wd[i]) & 0xff;
                for (j = 0; j < nc; j++)
                {
                    ncout[j] = (byte)(ncout[j + 1] ^ (k == 0 ? 0 : (byte)_alog[(_log[k] + _log[c[nc - j - 1]]) % 255]));
                }
            }
        }
    }

    private class DmParams
    {
        internal readonly int DataBlock;

        internal readonly int DataSize;

        internal readonly int ErrorBlock;

        internal readonly int height;

        internal readonly int HeightSection;

        internal readonly int width;

        internal readonly int WidthSection;

        internal DmParams(int height, int width, int heightSection, int widthSection, int dataSize, int dataBlock,
                          int errorBlock)
        {
            this.height = height;
            this.width = width;
            HeightSection = heightSection;
            WidthSection = widthSection;
            DataSize = dataSize;
            DataBlock = dataBlock;
            ErrorBlock = errorBlock;
        }
    }
}