using System.util;
using iTextSharp.LGPLv2.Core.System.NetUtils;

namespace iTextSharp.text.pdf.codec;

/// <summary>
///     Reads a BMP image. All types of BMP can be read.
///     It is based in the JAI codec.
///     @author  Paulo Soares (psoares@consiste.pt)
/// </summary>
public class BmpImage
{
    private const int BiBitfields = 3;

    /// <summary>
    ///     Compression Types
    /// </summary>
    private const int BiRgb = 0;

    private const int BiRle4 = 2;

    private const int BiRle8 = 1;

    /// <summary>
    ///     Color space types
    /// </summary>
    private const int LcsCalibratedRgb = 0;

    private const int LcsCmyk = 2;

    private const int Lcs_SRgb = 1;

    /// <summary>
    ///     BMP Image types
    /// </summary>
    private const int Version21Bit = 0;

    private const int Version224Bit = 3;

    private const int Version24Bit = 1;

    private const int Version28Bit = 2;

    private const int Version31Bit = 4;

    private const int Version324Bit = 7;

    private const int Version34Bit = 5;

    private const int Version38Bit = 6;

    private const int Version3Nt16Bit = 8;

    private const int Version3Nt32Bit = 9;

    private const int Version41Bit = 10;

    private const int Version416Bit = 13;

    private const int Version424Bit = 14;

    private const int Version432Bit = 15;

    private const int Version44Bit = 11;

    private const int Version48Bit = 12;

    private long _bitmapFileSize;

    private long _bitmapOffset;

    private int _bitsPerPixel;

    private long _compression;

    private int _height;

    private long _imageSize;

    private int _imageType;

    /// <summary>
    ///     BMP variables
    /// </summary>
    private Stream _inputStream;

    private bool _isBottomUp;
    private int _numBands;
    private byte[] _palette;
    private int _redMask, _greenMask, _blueMask, _alphaMask;
    private int _width;
    private long _xPelsPerMeter;
    private long _yPelsPerMeter;

    public INullValueDictionary<string, object> Properties = new NullValueDictionary<string, object>();

    internal BmpImage(Stream isp, bool noHeader, int size)
    {
        _bitmapFileSize = size;
        _bitmapOffset = 0;
        Process(isp, noHeader);
    }

    /// <summary>
    ///     Reads a BMP from an url.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="url">the url</param>
    /// <returns>the image</returns>
    public static Image GetImage(Uri url)
    {
        using var isp = url.GetResponseStream();
        var img = GetImage(isp);
        img.Url = url;

        return img;
    }

    /// <summary>
    ///     Reads a BMP from a stream. The stream is not closed.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="isp">the stream</param>
    /// <returns>the image</returns>
    public static Image GetImage(Stream isp)
        => GetImage(isp, false, 0);

    /// <summary>
    ///     Reads a BMP from a stream. The stream is not closed.
    ///     The BMP may not have a header and be considered as a plain DIB.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="isp">the stream</param>
    /// <param name="noHeader">true to process a plain DIB</param>
    /// <param name="size">the size of the DIB. Not used for a BMP</param>
    /// <returns>the image</returns>
    public static Image GetImage(Stream isp, bool noHeader, int size)
    {
        var bmp = new BmpImage(isp, noHeader, size);
        var img = bmp.getImage();
        img.SetDpi((int)(bmp._xPelsPerMeter * 0.0254 + 0.5), (int)(bmp._yPelsPerMeter * 0.0254 + 0.5));
        img.OriginalType = Image.ORIGINAL_BMP;

        return img;
    }

    /// <summary>
    ///     Reads a BMP from a file.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="file">the file</param>
    /// <returns>the image</returns>
    public static Image GetImage(string file)
        => GetImage(Utilities.ToUrl(file));

    /// <summary>
    ///     Reads a BMP from a byte array.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="data">the byte array</param>
    /// <returns>the image</returns>
    public static Image GetImage(byte[] data)
    {
        Stream isp = new MemoryStream(data);
        var img = GetImage(isp);
        img.OriginalData = data;

        return img;
    }

    protected void Process(Stream stream, bool noHeader)
    {
        if (noHeader || stream is BufferedStream)
        {
            _inputStream = stream;
        }
        else
        {
            _inputStream = new BufferedStream(stream);
        }

        if (!noHeader)
        {
            // Start File Header
            if (!(readUnsignedByte(_inputStream) == 'B' && readUnsignedByte(_inputStream) == 'M'))
            {
                throw new InvalidOperationException("Invalid magic value for BMP file.");
            }

            // Read file size
            _bitmapFileSize = readDWord(_inputStream);

            // Read the two reserved fields
            readWord(_inputStream);
            readWord(_inputStream);

            // Offset to the bitmap from the beginning
            _bitmapOffset = readDWord(_inputStream);

            // End File Header
        }

        // Start BitmapCoreHeader
        var size = readDWord(_inputStream);

        if (size == 12)
        {
            _width = readWord(_inputStream);
            _height = readWord(_inputStream);
        }
        else
        {
            _width = readLong(_inputStream);
            _height = readLong(_inputStream);
        }

        var planes = readWord(_inputStream);
        _bitsPerPixel = readWord(_inputStream);

        Properties["color_planes"] = planes;
        Properties["bits_per_pixel"] = _bitsPerPixel;

        // As BMP always has 3 rgb bands, except for Version 5,
        // which is bgra
        _numBands = 3;

        if (_bitmapOffset == 0)
        {
            _bitmapOffset = size;
        }

        if (size == 12)
        {
            // Windows 2.x and OS/2 1.x
            Properties["bmp_version"] = "BMP v. 2.x";

            // Classify the image type
            if (_bitsPerPixel == 1)
            {
                _imageType = Version21Bit;
            }
            else if (_bitsPerPixel == 4)
            {
                _imageType = Version24Bit;
            }
            else if (_bitsPerPixel == 8)
            {
                _imageType = Version28Bit;
            }
            else if (_bitsPerPixel == 24)
            {
                _imageType = Version224Bit;
            }

            // Read in the palette
            var numberOfEntries = (int)((_bitmapOffset - 14 - size) / 3);
            var sizeOfPalette = numberOfEntries * 3;

            if (_bitmapOffset == size)
            {
                switch (_imageType)
                {
                    case Version21Bit:
                        sizeOfPalette = 2 * 3;

                        break;
                    case Version24Bit:
                        sizeOfPalette = 16 * 3;

                        break;
                    case Version28Bit:
                        sizeOfPalette = 256 * 3;

                        break;
                    case Version224Bit:
                        sizeOfPalette = 0;

                        break;
                }

                _bitmapOffset = size + sizeOfPalette;
            }

            readPalette(sizeOfPalette);
        }
        else
        {
            _compression = readDWord(_inputStream);
            _imageSize = readDWord(_inputStream);
            _xPelsPerMeter = readLong(_inputStream);
            _yPelsPerMeter = readLong(_inputStream);
            var colorsUsed = readDWord(_inputStream);
            var colorsImportant = readDWord(_inputStream);

            switch ((int)_compression)
            {
                case BiRgb:
                    Properties["compression"] = "BI_RGB";

                    break;

                case BiRle8:
                    Properties["compression"] = "BI_RLE8";

                    break;

                case BiRle4:
                    Properties["compression"] = "BI_RLE4";

                    break;

                case BiBitfields:
                    Properties["compression"] = "BI_BITFIELDS";

                    break;
            }

            Properties["x_pixels_per_meter"] = _xPelsPerMeter;
            Properties["y_pixels_per_meter"] = _yPelsPerMeter;
            Properties["colors_used"] = colorsUsed;
            Properties["colors_important"] = colorsImportant;

            if (size == 40)
            {
                // Windows 3.x and Windows NT
                switch ((int)_compression)
                {
                    case BiRgb: // No compression
                    case BiRle8: // 8-bit RLE compression
                    case BiRle4: // 4-bit RLE compression

                        if (_bitsPerPixel == 1)
                        {
                            _imageType = Version31Bit;
                        }
                        else if (_bitsPerPixel == 4)
                        {
                            _imageType = Version34Bit;
                        }
                        else if (_bitsPerPixel == 8)
                        {
                            _imageType = Version38Bit;
                        }
                        else if (_bitsPerPixel == 24)
                        {
                            _imageType = Version324Bit;
                        }
                        else if (_bitsPerPixel == 16)
                        {
                            _imageType = Version3Nt16Bit;
                            _redMask = 0x7C00;
                            _greenMask = 0x3E0;
                            _blueMask = 0x1F;
                            Properties["red_mask"] = _redMask;
                            Properties["green_mask"] = _greenMask;
                            Properties["blue_mask"] = _blueMask;
                        }
                        else if (_bitsPerPixel == 32)
                        {
                            _imageType = Version3Nt32Bit;
                            _redMask = 0x00FF0000;
                            _greenMask = 0x0000FF00;
                            _blueMask = 0x000000FF;
                            Properties["red_mask"] = _redMask;
                            Properties["green_mask"] = _greenMask;
                            Properties["blue_mask"] = _blueMask;
                        }

                        // Read in the palette
                        var numberOfEntries = (int)((_bitmapOffset - 14 - size) / 4);
                        var sizeOfPalette = numberOfEntries * 4;

                        if (_bitmapOffset == size)
                        {
                            switch (_imageType)
                            {
                                case Version31Bit:
                                    sizeOfPalette = (int)(colorsUsed == 0 ? 2 : colorsUsed) * 4;

                                    break;
                                case Version34Bit:
                                    sizeOfPalette = (int)(colorsUsed == 0 ? 16 : colorsUsed) * 4;

                                    break;
                                case Version38Bit:
                                    sizeOfPalette = (int)(colorsUsed == 0 ? 256 : colorsUsed) * 4;

                                    break;
                                default:
                                    sizeOfPalette = 0;

                                    break;
                            }

                            _bitmapOffset = size + sizeOfPalette;
                        }

                        readPalette(sizeOfPalette);
                        Properties["bmp_version"] = "BMP v. 3.x";

                        break;

                    case BiBitfields:

                        if (_bitsPerPixel == 16)
                        {
                            _imageType = Version3Nt16Bit;
                        }
                        else if (_bitsPerPixel == 32)
                        {
                            _imageType = Version3Nt32Bit;
                        }

                        // BitsField encoding
                        _redMask = (int)readDWord(_inputStream);
                        _greenMask = (int)readDWord(_inputStream);
                        _blueMask = (int)readDWord(_inputStream);

                        Properties["red_mask"] = _redMask;
                        Properties["green_mask"] = _greenMask;
                        Properties["blue_mask"] = _blueMask;

                        if (colorsUsed != 0)
                        {
                            // there is a palette
                            sizeOfPalette = (int)colorsUsed * 4;
                            readPalette(sizeOfPalette);
                        }

                        Properties["bmp_version"] = "BMP v. 3.x NT";

                        break;

                    default:
                        throw new InvalidOperationException("Invalid compression specified in BMP file.");
                }
            }
            else if (size == 108)
            {
                // Windows 4.x BMP

                Properties["bmp_version"] = "BMP v. 4.x";

                // rgb masks, valid only if comp is BI_BITFIELDS
                _redMask = (int)readDWord(_inputStream);
                _greenMask = (int)readDWord(_inputStream);
                _blueMask = (int)readDWord(_inputStream);

                // Only supported for 32bpp BI_RGB argb
                _alphaMask = (int)readDWord(_inputStream);
                var csType = readDWord(_inputStream);
                var redX = readLong(_inputStream);
                var redY = readLong(_inputStream);
                var redZ = readLong(_inputStream);
                var greenX = readLong(_inputStream);
                var greenY = readLong(_inputStream);
                var greenZ = readLong(_inputStream);
                var blueX = readLong(_inputStream);
                var blueY = readLong(_inputStream);
                var blueZ = readLong(_inputStream);
                var gammaRed = readDWord(_inputStream);
                var gammaGreen = readDWord(_inputStream);
                var gammaBlue = readDWord(_inputStream);

                if (_bitsPerPixel == 1)
                {
                    _imageType = Version41Bit;
                }
                else if (_bitsPerPixel == 4)
                {
                    _imageType = Version44Bit;
                }
                else if (_bitsPerPixel == 8)
                {
                    _imageType = Version48Bit;
                }
                else if (_bitsPerPixel == 16)
                {
                    _imageType = Version416Bit;

                    if ((int)_compression == BiRgb)
                    {
                        _redMask = 0x7C00;
                        _greenMask = 0x3E0;
                        _blueMask = 0x1F;
                    }
                }
                else if (_bitsPerPixel == 24)
                {
                    _imageType = Version424Bit;
                }
                else if (_bitsPerPixel == 32)
                {
                    _imageType = Version432Bit;

                    if ((int)_compression == BiRgb)
                    {
                        _redMask = 0x00FF0000;
                        _greenMask = 0x0000FF00;
                        _blueMask = 0x000000FF;
                    }
                }

                Properties["red_mask"] = _redMask;
                Properties["green_mask"] = _greenMask;
                Properties["blue_mask"] = _blueMask;
                Properties["alpha_mask"] = _alphaMask;

                // Read in the palette
                var numberOfEntries = (int)((_bitmapOffset - 14 - size) / 4);
                var sizeOfPalette = numberOfEntries * 4;

                if (_bitmapOffset == size)
                {
                    switch (_imageType)
                    {
                        case Version41Bit:
                            sizeOfPalette = (int)(colorsUsed == 0 ? 2 : colorsUsed) * 4;

                            break;
                        case Version44Bit:
                            sizeOfPalette = (int)(colorsUsed == 0 ? 16 : colorsUsed) * 4;

                            break;
                        case Version48Bit:
                            sizeOfPalette = (int)(colorsUsed == 0 ? 256 : colorsUsed) * 4;

                            break;
                        default:
                            sizeOfPalette = 0;

                            break;
                    }

                    _bitmapOffset = size + sizeOfPalette;
                }

                readPalette(sizeOfPalette);

                switch ((int)csType)
                {
                    case LcsCalibratedRgb:
                        // All the new fields are valid only for this case
                        Properties["color_space"] = "LCS_CALIBRATED_RGB";
                        Properties["redX"] = redX;
                        Properties["redY"] = redY;
                        Properties["redZ"] = redZ;
                        Properties["greenX"] = greenX;
                        Properties["greenY"] = greenY;
                        Properties["greenZ"] = greenZ;
                        Properties["blueX"] = blueX;
                        Properties["blueY"] = blueY;
                        Properties["blueZ"] = blueZ;
                        Properties["gamma_red"] = gammaRed;
                        Properties["gamma_green"] = gammaGreen;
                        Properties["gamma_blue"] = gammaBlue;

                        // break;
                        throw new NotImplementedException("Not implemented yet.");

                    case Lcs_SRgb:
                        // Default Windows color space
                        Properties["color_space"] = "LCS_sRGB";

                        break;

                    case LcsCmyk:
                        Properties["color_space"] = "LCS_CMYK";

                        //		    break;
                        throw new NotImplementedException("Not implemented yet.");
                }
            }
            else
            {
                Properties["bmp_version"] = "BMP v. 5.x";

                throw new NotImplementedException("BMP version 5 not implemented yet.");
            }
        }

        if (_height > 0)
        {
            // bottom up image
            _isBottomUp = true;
        }
        else
        {
            // top down image
            _isBottomUp = false;
            _height = Math.Abs(_height);
        }

        // When number of bitsPerPixel is <= 8, we use IndexColorModel.
        if (_bitsPerPixel == 1 || _bitsPerPixel == 4 || _bitsPerPixel == 8)
        {
            _numBands = 1;

            // Create IndexColorModel from the palette.
            byte[] r;
            byte[] g;
            byte[] b;
            int sizep;

            if (_imageType == Version21Bit || _imageType == Version24Bit || _imageType == Version28Bit)
            {
                sizep = _palette.Length / 3;

                if (sizep > 256)
                {
                    sizep = 256;
                }

                int off;
                r = new byte[sizep];
                g = new byte[sizep];
                b = new byte[sizep];

                for (var i = 0; i < sizep; i++)
                {
                    off = 3 * i;
                    b[i] = _palette[off];
                    g[i] = _palette[off + 1];
                    r[i] = _palette[off + 2];
                }
            }
            else
            {
                sizep = _palette.Length / 4;

                if (sizep > 256)
                {
                    sizep = 256;
                }

                int off;
                r = new byte[sizep];
                g = new byte[sizep];
                b = new byte[sizep];

                for (var i = 0; i < sizep; i++)
                {
                    off = 4 * i;
                    b[i] = _palette[off];
                    g[i] = _palette[off + 1];
                    r[i] = _palette[off + 2];
                }
            }
        }
        else if (_bitsPerPixel == 16)
        {
            _numBands = 3;
        }
        else if (_bitsPerPixel == 32)
        {
            _numBands = _alphaMask == 0 ? 3 : 4;
        }
        else
        {
            _numBands = 3;
        }
    }

    private byte[] decodeRle(bool is8, byte[] values)
    {
        var val = new byte[_width * _height];

        try
        {
            var ptr = 0;
            var x = 0;
            var q = 0;

            for (var y = 0; y < _height && ptr < values.Length;)
            {
                var count = values[ptr++] & 0xff;

                if (count != 0)
                {
                    // encoded mode
                    var bt = values[ptr++] & 0xff;

                    if (is8)
                    {
                        for (var i = count; i != 0; --i)
                        {
                            val[q++] = (byte)bt;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < count; ++i)
                        {
                            val[q++] = (byte)((i & 1) == 1 ? bt & 0x0f : (bt >> 4) & 0x0f);
                        }
                    }

                    x += count;
                }
                else
                {
                    // escape mode
                    count = values[ptr++] & 0xff;

                    if (count == 1)
                    {
                        break;
                    }

                    switch (count)
                    {
                        case 0:
                            x = 0;
                            ++y;
                            q = y * _width;

                            break;
                        case 2:
                            // delta mode
                            x += values[ptr++] & 0xff;
                            y += values[ptr++] & 0xff;
                            q = y * _width + x;

                            break;
                        default:
                            // absolute mode
                            if (is8)
                            {
                                for (var i = count; i != 0; --i)
                                {
                                    val[q++] = (byte)(values[ptr++] & 0xff);
                                }
                            }
                            else
                            {
                                var bt = 0;

                                for (var i = 0; i < count; ++i)
                                {
                                    if ((i & 1) == 0)
                                    {
                                        bt = values[ptr++] & 0xff;
                                    }

                                    val[q++] = (byte)((i & 1) == 1 ? bt & 0x0f : (bt >> 4) & 0x0f);
                                }
                            }

                            x += count;

                            // read pad byte
                            if (is8)
                            {
                                if ((count & 1) == 1)
                                {
                                    ++ptr;
                                }
                            }
                            else
                            {
                                if ((count & 3) == 1 || (count & 3) == 2)
                                {
                                    ++ptr;
                                }
                            }

                            break;
                    }
                }
            }
        }
        catch
        {
            //empty on purpose
        }

        return val;
    }

    private static int findMask(int mask)
    {
        var k = 0;

        for (; k < 32; ++k)
        {
            if ((mask & 1) == 1)
            {
                break;
            }

            mask = Util.Usr(mask, 1);
        }

        return mask;
    }

    private static int findShift(int mask)
    {
        var k = 0;

        for (; k < 32; ++k)
        {
            if ((mask & 1) == 1)
            {
                break;
            }

            mask = Util.Usr(mask, 1);
        }

        return k;
    }

    private Image getImage()
    {
        byte[] bdata = null; // buffer for byte data

        //short[] sdata = null; // buffer for short data
        //int[] idata = null; // buffer for int data

        //	if (sampleModel.GetDataType() == DataBuffer.TYPE_BYTE)
        //	    bdata = (byte[])((DataBufferByte)tile.GetDataBuffer()).GetData();
        //	else if (sampleModel.GetDataType() == DataBuffer.TYPE_USHORT)
        //	    sdata = (short[])((DataBufferUShort)tile.GetDataBuffer()).GetData();
        //	else if (sampleModel.GetDataType() == DataBuffer.TYPE_INT)
        //	    idata = (int[])((DataBufferInt)tile.GetDataBuffer()).GetData();

        // There should only be one tile.
        switch (_imageType)
        {
            case Version21Bit:
                // no compression
                return read1Bit(3);

            case Version24Bit:
                // no compression
                return read4Bit(3);

            case Version28Bit:
                // no compression
                return read8Bit(3);

            case Version224Bit:
                // no compression
                bdata = new byte[_width * _height * 3];
                read24Bit(bdata);

                return new ImgRaw(_width, _height, 3, 8, bdata);

            case Version31Bit:
                // 1-bit images cannot be compressed.
                return read1Bit(4);

            case Version34Bit:
                switch ((int)_compression)
                {
                    case BiRgb:
                        return read4Bit(4);

                    case BiRle4:
                        return readRle4();

                    default:
                        throw new InvalidOperationException("Invalid compression specified for BMP file.");
                }

            case Version38Bit:
                switch ((int)_compression)
                {
                    case BiRgb:
                        return read8Bit(4);

                    case BiRle8:
                        return readRle8();

                    default:
                        throw new InvalidOperationException("Invalid compression specified for BMP file.");
                }

            case Version324Bit:
                // 24-bit images are not compressed
                bdata = new byte[_width * _height * 3];
                read24Bit(bdata);

                return new ImgRaw(_width, _height, 3, 8, bdata);

            case Version3Nt16Bit:
                return read1632Bit(false);

            case Version3Nt32Bit:
                return read1632Bit(true);

            case Version41Bit:
                return read1Bit(4);

            case Version44Bit:
                switch ((int)_compression)
                {
                    case BiRgb:
                        return read4Bit(4);

                    case BiRle4:
                        return readRle4();

                    default:
                        throw new InvalidOperationException("Invalid compression specified for BMP file.");
                }

            case Version48Bit:
                switch ((int)_compression)
                {
                    case BiRgb:
                        return read8Bit(4);

                    case BiRle8:
                        return readRle8();

                    default:
                        throw new InvalidOperationException("Invalid compression specified for BMP file.");
                }

            case Version416Bit:
                return read1632Bit(false);

            case Version424Bit:
                bdata = new byte[_width * _height * 3];
                read24Bit(bdata);

                return new ImgRaw(_width, _height, 3, 8, bdata);

            case Version432Bit:
                return read1632Bit(true);
        }

        return null;
    }

    private byte[] getPalette(int group)
    {
        if (_palette == null)
        {
            return Array.Empty<byte>();
        }

        var np = new byte[_palette.Length / group * 3];
        var e = _palette.Length / group;

        for (var k = 0; k < e; ++k)
        {
            var src = k * group;
            var dest = k * 3;
            np[dest + 2] = _palette[src++];
            np[dest + 1] = _palette[src++];
            np[dest] = _palette[src];
        }

        return np;
    }

    private Image indexedModel(byte[] bdata, int bpc, int paletteEntries)
    {
        Image img = new ImgRaw(_width, _height, 1, bpc, bdata);
        var colorspace = new PdfArray();
        colorspace.Add(PdfName.Indexed);
        colorspace.Add(PdfName.Devicergb);
        var np = getPalette(paletteEntries);
        var len = np.Length;
        colorspace.Add(new PdfNumber(len / 3 - 1));
        colorspace.Add(new PdfString(np));
        var ad = new PdfDictionary();
        ad.Put(PdfName.Colorspace, colorspace);
        img.Additional = ad;

        return img;
    }

    private ImgRaw read1632Bit(bool is32)
    {
        var red_mask = findMask(_redMask);
        var redShift = findShift(_redMask);
        var redFactor = red_mask + 1;
        var green_mask = findMask(_greenMask);
        var greenShift = findShift(_greenMask);
        var greenFactor = green_mask + 1;
        var blue_mask = findMask(_blueMask);
        var blueShift = findShift(_blueMask);
        var blueFactor = blue_mask + 1;
        var bdata = new byte[_width * _height * 3];

        // Padding bytes at the end of each scanline
        var padding = 0;

        if (!is32)
        {
            // width * bitsPerPixel should be divisible by 32
            var bitsPerScanline = _width * 16;

            if (bitsPerScanline % 32 != 0)
            {
                padding = (bitsPerScanline / 32 + 1) * 32 - bitsPerScanline;
                padding = (int)Math.Ceiling(padding / 8.0);
            }
        }

        var imSize = (int)_imageSize;

        if (imSize == 0)
        {
            imSize = (int)(_bitmapFileSize - _bitmapOffset);
        }

        var l = 0;
        int v;

        if (_isBottomUp)
        {
            for (var i = _height - 1; i >= 0; --i)
            {
                l = _width * 3 * i;

                for (var j = 0; j < _width; j++)
                {
                    if (is32)
                    {
                        v = (int)readDWord(_inputStream);
                    }
                    else
                    {
                        v = readWord(_inputStream);
                    }

                    bdata[l++] = (byte)((Util.Usr(v, redShift) & red_mask) * 256 / redFactor);
                    bdata[l++] = (byte)((Util.Usr(v, greenShift) & green_mask) * 256 / greenFactor);
                    bdata[l++] = (byte)((Util.Usr(v, blueShift) & blue_mask) * 256 / blueFactor);
                }

                for (var m = 0; m < padding; m++)
                {
                    _inputStream.ReadByte();
                }
            }
        }
        else
        {
            for (var i = 0; i < _height; i++)
            {
                for (var j = 0; j < _width; j++)
                {
                    if (is32)
                    {
                        v = (int)readDWord(_inputStream);
                    }
                    else
                    {
                        v = readWord(_inputStream);
                    }

                    bdata[l++] = (byte)((Util.Usr(v, redShift) & red_mask) * 256 / redFactor);
                    bdata[l++] = (byte)((Util.Usr(v, greenShift) & green_mask) * 256 / greenFactor);
                    bdata[l++] = (byte)((Util.Usr(v, blueShift) & blue_mask) * 256 / blueFactor);
                }

                for (var m = 0; m < padding; m++)
                {
                    _inputStream.ReadByte();
                }
            }
        }

        return new ImgRaw(_width, _height, 3, 8, bdata);
    }

    /// <summary>
    ///     Deal with 1 Bit images using IndexColorModels
    /// </summary>
    private Image read1Bit(int paletteEntries)
    {
        var bdata = new byte[(_width + 7) / 8 * _height];
        var padding = 0;
        var bytesPerScanline = (int)Math.Ceiling(_width / 8.0);

        var remainder = bytesPerScanline % 4;

        if (remainder != 0)
        {
            padding = 4 - remainder;
        }

        var imSize = (bytesPerScanline + padding) * _height;

        // Read till we have the whole image
        var values = new byte[imSize];
        var bytesRead = 0;

        while (bytesRead < imSize)
        {
            bytesRead += _inputStream.Read(values, bytesRead, imSize - bytesRead);
        }

        if (_isBottomUp)
        {
            // Convert the bottom up image to a top down format by copying
            // one scanline from the bottom to the top at a time.

            for (var i = 0; i < _height; i++)
            {
                Array.Copy(values, imSize - (i + 1) * (bytesPerScanline + padding), bdata, i * bytesPerScanline,
                    bytesPerScanline);
            }
        }
        else
        {
            for (var i = 0; i < _height; i++)
            {
                Array.Copy(values, i * (bytesPerScanline + padding), bdata, i * bytesPerScanline, bytesPerScanline);
            }
        }

        return indexedModel(bdata, 1, paletteEntries);
    }

    /// <summary>
    ///     Method to read 24 bit BMP image data
    /// </summary>
    private void read24Bit(byte[] bdata)
    {
        // Padding bytes at the end of each scanline
        var padding = 0;

        // width * bitsPerPixel should be divisible by 32
        var bitsPerScanline = _width * 24;

        if (bitsPerScanline % 32 != 0)
        {
            padding = (bitsPerScanline / 32 + 1) * 32 - bitsPerScanline;
            padding = (int)Math.Ceiling(padding / 8.0);
        }

        var imSize = (_width * 3 + 3) / 4 * 4 * _height;

        // Read till we have the whole image
        var values = new byte[imSize];
        var bytesRead = 0;

        while (bytesRead < imSize)
        {
            var r = _inputStream.Read(values, bytesRead, imSize - bytesRead);

            if (r < 0)
            {
                break;
            }

            bytesRead += r;
        }

        int l = 0, count;

        if (_isBottomUp)
        {
            var max = _width * _height * 3 - 1;

            count = -padding;

            for (var i = 0; i < _height; i++)
            {
                l = max - (i + 1) * _width * 3 + 1;
                count += padding;

                for (var j = 0; j < _width; j++)
                {
                    bdata[l + 2] = values[count++];
                    bdata[l + 1] = values[count++];
                    bdata[l] = values[count++];
                    l += 3;
                }
            }
        }
        else
        {
            count = -padding;

            for (var i = 0; i < _height; i++)
            {
                count += padding;

                for (var j = 0; j < _width; j++)
                {
                    bdata[l + 2] = values[count++];
                    bdata[l + 1] = values[count++];
                    bdata[l] = values[count++];
                    l += 3;
                }
            }
        }
    }

    /// <summary>
    ///     Method to read a 4 bit BMP image data
    /// </summary>
    private Image read4Bit(int paletteEntries)
    {
        var bdata = new byte[(_width + 1) / 2 * _height];

        // Padding bytes at the end of each scanline
        var padding = 0;

        var bytesPerScanline = (int)Math.Ceiling(_width / 2.0);
        var remainder = bytesPerScanline % 4;

        if (remainder != 0)
        {
            padding = 4 - remainder;
        }

        var imSize = (bytesPerScanline + padding) * _height;

        // Read till we have the whole image
        var values = new byte[imSize];
        var bytesRead = 0;

        while (bytesRead < imSize)
        {
            bytesRead += _inputStream.Read(values, bytesRead, imSize - bytesRead);
        }

        if (_isBottomUp)
        {
            // Convert the bottom up image to a top down format by copying
            // one scanline from the bottom to the top at a time.
            for (var i = 0; i < _height; i++)
            {
                Array.Copy(values, imSize - (i + 1) * (bytesPerScanline + padding), bdata, i * bytesPerScanline,
                    bytesPerScanline);
            }
        }
        else
        {
            for (var i = 0; i < _height; i++)
            {
                Array.Copy(values, i * (bytesPerScanline + padding), bdata, i * bytesPerScanline, bytesPerScanline);
            }
        }

        return indexedModel(bdata, 4, paletteEntries);
    }

    /// <summary>
    ///     Method to read 8 bit BMP image data
    /// </summary>
    private Image read8Bit(int paletteEntries)
    {
        var bdata = new byte[_width * _height];

        // Padding bytes at the end of each scanline
        var padding = 0;

        // width * bitsPerPixel should be divisible by 32
        var bitsPerScanline = _width * 8;

        if (bitsPerScanline % 32 != 0)
        {
            padding = (bitsPerScanline / 32 + 1) * 32 - bitsPerScanline;
            padding = (int)Math.Ceiling(padding / 8.0);
        }

        var imSize = (_width + padding) * _height;

        // Read till we have the whole image
        var values = new byte[imSize];
        var bytesRead = 0;

        while (bytesRead < imSize)
        {
            bytesRead += _inputStream.Read(values, bytesRead, imSize - bytesRead);
        }

        if (_isBottomUp)
        {
            // Convert the bottom up image to a top down format by copying
            // one scanline from the bottom to the top at a time.
            for (var i = 0; i < _height; i++)
            {
                Array.Copy(values, imSize - (i + 1) * (_width + padding), bdata, i * _width, _width);
            }
        }
        else
        {
            for (var i = 0; i < _height; i++)
            {
                Array.Copy(values, i * (_width + padding), bdata, i * _width, _width);
            }
        }

        return indexedModel(bdata, 8, paletteEntries);
    }

    /// <summary>
    ///     Unsigned 4 bytes
    /// </summary>
    private static long readDWord(Stream stream)
        => readUnsignedInt(stream);

    /// <summary>
    ///     Signed 4 bytes
    /// </summary>
    private static int readInt(Stream stream)
    {
        var b1 = readUnsignedByte(stream);
        var b2 = readUnsignedByte(stream);
        var b3 = readUnsignedByte(stream);
        var b4 = readUnsignedByte(stream);

        return (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;
    }

    /// <summary>
    ///     32 bit signed value
    /// </summary>
    private static int readLong(Stream stream)
        => readInt(stream);

    private void readPalette(int sizeOfPalette)
    {
        if (sizeOfPalette == 0)
        {
            return;
        }

        _palette = new byte[sizeOfPalette];
        var bytesRead = 0;

        while (bytesRead < sizeOfPalette)
        {
            var r = _inputStream.Read(_palette, bytesRead, sizeOfPalette - bytesRead);

            if (r <= 0)
            {
                throw new IOException("incomplete palette");
            }

            bytesRead += r;
        }

        Properties["palette"] = _palette;
    }

    private Image readRle4()
    {
        // If imageSize field is not specified, calculate it.
        var imSize = (int)_imageSize;

        if (imSize == 0)
        {
            imSize = (int)(_bitmapFileSize - _bitmapOffset);
        }

        // Read till we have the whole image
        var values = new byte[imSize];
        var bytesRead = 0;

        while (bytesRead < imSize)
        {
            bytesRead += _inputStream.Read(values, bytesRead, imSize - bytesRead);
        }

        // Decompress the RLE4 compressed data.
        var val = decodeRle(false, values);

        // Invert it as it is bottom up format.
        if (_isBottomUp)
        {
            var inverted = val;
            val = new byte[_width * _height];
            int l = 0, index, lineEnd;

            for (var i = _height - 1; i >= 0; i--)
            {
                index = i * _width;
                lineEnd = l + _width;

                while (l != lineEnd)
                {
                    val[l++] = inverted[index++];
                }
            }
        }

        var stride = (_width + 1) / 2;
        var bdata = new byte[stride * _height];
        var ptr = 0;
        var sh = 0;

        for (var h = 0; h < _height; ++h)
        {
            for (var w = 0; w < _width; ++w)
            {
                if ((w & 1) == 0)
                {
                    bdata[sh + w / 2] = (byte)(val[ptr++] << 4);
                }
                else
                {
                    bdata[sh + w / 2] |= (byte)(val[ptr++] & 0x0f);
                }
            }

            sh += stride;
        }

        return indexedModel(bdata, 4, 4);
    }

    private Image readRle8()
    {
        // If imageSize field is not provided, calculate it.
        var imSize = (int)_imageSize;

        if (imSize == 0)
        {
            imSize = (int)(_bitmapFileSize - _bitmapOffset);
        }

        // Read till we have the whole image
        var values = new byte[imSize];
        var bytesRead = 0;

        while (bytesRead < imSize)
        {
            bytesRead += _inputStream.Read(values, bytesRead, imSize - bytesRead);
        }

        // Since data is compressed, decompress it
        var val = decodeRle(true, values);

        // Uncompressed data does not have any padding
        imSize = _width * _height;

        if (_isBottomUp)
        {
            // Convert the bottom up image to a top down format by copying
            // one scanline from the bottom to the top at a time.
            // int bytesPerScanline = (int)Math.Ceil((double)width/8.0);
            var temp = new byte[val.Length];
            var bytesPerScanline = _width;

            for (var i = 0; i < _height; i++)
            {
                Array.Copy(val, imSize - (i + 1) * bytesPerScanline, temp, i * bytesPerScanline, bytesPerScanline);
            }

            val = temp;
        }

        return indexedModel(val, 8, 4);
    }

    /// <summary>
    ///     Windows defined data type reading methods - everything is little endian
    /// </summary>
    /// <summary>
    ///     Signed 16 bits
    /// </summary>
    private static int readShort(Stream stream)
    {
        var b1 = readUnsignedByte(stream);
        var b2 = readUnsignedByte(stream);

        return (b2 << 8) | b1;
    }

    /// <summary>
    ///     Unsigned 8 bits
    /// </summary>
    private static int readUnsignedByte(Stream stream)
        => stream.ReadByte() & 0xff;

    /// <summary>
    ///     Unsigned 4 bytes
    /// </summary>
    private static long readUnsignedInt(Stream stream)
    {
        var b1 = readUnsignedByte(stream);
        var b2 = readUnsignedByte(stream);
        var b3 = readUnsignedByte(stream);
        var b4 = readUnsignedByte(stream);
        long l = (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;

        return l & 0xffffffff;
    }

    /// <summary>
    ///     Unsigned 2 bytes
    /// </summary>
    private static int readUnsignedShort(Stream stream)
    {
        var b1 = readUnsignedByte(stream);
        var b2 = readUnsignedByte(stream);

        return ((b2 << 8) | b1) & 0xffff;
    }

    /// <summary>
    ///     Unsigned 16 bits
    /// </summary>
    private static int readWord(Stream stream)
        => readUnsignedShort(stream);
}