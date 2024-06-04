using System.util;
using System.util.zlib;

namespace iTextSharp.text.pdf.codec;

/// <summary>
///     Reads TIFF images
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public static class TiffImage
{
    /// <summary>
    ///     Uncompress packbits compressed image data.
    /// </summary>
    public static void DecodePackbits(byte[] data, byte[] dst)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (dst == null)
        {
            throw new ArgumentNullException(nameof(dst));
        }

        int srcCount = 0, dstCount = 0;
        sbyte repeat, b;

        try
        {
            while (dstCount < dst.Length)
            {
                b = (sbyte)data[srcCount++];
                if (b >= 0 && b <= 127)
                {
                    // literal run packet
                    for (var i = 0; i < b + 1; i++)
                    {
                        dst[dstCount++] = data[srcCount++];
                    }
                }
                else if (b <= -1 && b >= -127)
                {
                    // 2 byte encoded run packet
                    repeat = (sbyte)data[srcCount++];
                    for (var i = 0; i < -b + 1; i++)
                    {
                        dst[dstCount++] = (byte)repeat;
                    }
                }
                else
                {
                    // no-op packet. Do nothing
                    srcCount++;
                }
            }
        }
        catch
        {
        }
    }

    /// <summary>
    ///     Gets the number of pages the TIFF document has.
    /// </summary>
    /// <param name="s">the file source</param>
    /// <returns>the number of pages</returns>
    public static int GetNumberOfPages(RandomAccessFileOrArray s) => TiffDirectory.GetNumDirectories(s);

    /// <summary>
    ///     Reads a page from a TIFF image. Direct mode is not used.
    /// </summary>
    /// <param name="s">the file source</param>
    /// <param name="page">the page to get. The first page is 1</param>
    /// <returns>the  Image </returns>
    public static Image GetTiffImage(RandomAccessFileOrArray s, int page) => GetTiffImage(s, page, false);

    /// <summary>
    ///     Reads a page from a TIFF image.
    ///     by direct byte copying. It's faster but may not work
    ///     every time
    /// </summary>
    /// <param name="s">the file source</param>
    /// <param name="page">the page to get. The first page is 1</param>
    /// <param name="direct">for single strip, CCITT images, generate the image</param>
    /// <returns>the  Image </returns>
    public static Image GetTiffImage(RandomAccessFileOrArray s, int page, bool direct)
    {
        if (page < 1)
        {
            throw new InvalidOperationException("The page number must be >= 1.");
        }

        var dir = new TiffDirectory(s, page - 1);
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_TILEWIDTH))
        {
            throw new InvalidOperationException("Tiles are not supported.");
        }

        var compression = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_COMPRESSION);
        switch (compression)
        {
            case TiffConstants.COMPRESSION_CCITTRLEW:
            case TiffConstants.COMPRESSION_CCITTRLE:
            case TiffConstants.COMPRESSION_CCITTFAX3:
            case TiffConstants.COMPRESSION_CCITTFAX4:
                break;
            default:
                return GetTiffImageColor(dir, s);
        }

        float rotation = 0;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_ORIENTATION))
        {
            var rot = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_ORIENTATION);
            if (rot == TiffConstants.ORIENTATION_BOTRIGHT || rot == TiffConstants.ORIENTATION_BOTLEFT)
            {
                rotation = (float)Math.PI;
            }
            else if (rot == TiffConstants.ORIENTATION_LEFTTOP || rot == TiffConstants.ORIENTATION_LEFTBOT)
            {
                rotation = (float)(Math.PI / 2.0);
            }
            else if (rot == TiffConstants.ORIENTATION_RIGHTTOP || rot == TiffConstants.ORIENTATION_RIGHTBOT)
            {
                rotation = -(float)(Math.PI / 2.0);
            }
        }

        Image img = null;
        long tiffT4Options = 0;
        long tiffT6Options = 0;
        var fillOrder = 1;
        var h = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_IMAGELENGTH);
        var w = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_IMAGEWIDTH);
        var dpiX = 0;
        var dpiY = 0;
        float xyRatio = 0;
        var resolutionUnit = TiffConstants.RESUNIT_INCH;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_RESOLUTIONUNIT))
        {
            resolutionUnit = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_RESOLUTIONUNIT);
        }

        dpiX = getDpi(dir.GetField(TiffConstants.TIFFTAG_XRESOLUTION), resolutionUnit);
        dpiY = getDpi(dir.GetField(TiffConstants.TIFFTAG_YRESOLUTION), resolutionUnit);
        if (resolutionUnit == TiffConstants.RESUNIT_NONE)
        {
            if (dpiY != 0)
            {
                xyRatio = dpiX / (float)dpiY;
            }

            dpiX = 0;
            dpiY = 0;
        }

        var rowsStrip = h;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_ROWSPERSTRIP))
        {
            rowsStrip = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_ROWSPERSTRIP);
        }

        if (rowsStrip <= 0 || rowsStrip > h)
        {
            rowsStrip = h;
        }

        var offset = getArrayLongShort(dir, TiffConstants.TIFFTAG_STRIPOFFSETS);
        var size = getArrayLongShort(dir, TiffConstants.TIFFTAG_STRIPBYTECOUNTS);
        if ((size == null || (size.Length == 1 && (size[0] == 0 || size[0] + offset[0] > s.Length))) && h == rowsStrip)
        {
            // some TIFF producers are really lousy, so...
            size = new long[] { s.Length - (int)offset[0] };
        }

        var reverse = false;
        var fillOrderField = dir.GetField(TiffConstants.TIFFTAG_FILLORDER);
        if (fillOrderField != null)
        {
            fillOrder = fillOrderField.GetAsInt(0);
        }

        reverse = fillOrder == TiffConstants.FILLORDER_LSB2MSB;
        var paramsn = 0;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_PHOTOMETRIC))
        {
            var photo = dir.GetFieldAsLong(TiffConstants.TIFFTAG_PHOTOMETRIC);
            if (photo == TiffConstants.PHOTOMETRIC_MINISBLACK)
            {
                paramsn |= Element.CCITT_BLACKIS1;
            }
        }

        var imagecomp = 0;
        switch (compression)
        {
            case TiffConstants.COMPRESSION_CCITTRLEW:
            case TiffConstants.COMPRESSION_CCITTRLE:
                imagecomp = Element.CCITTG3_1D;
                paramsn |= Element.CCITT_ENCODEDBYTEALIGN | Element.CCITT_ENDOFBLOCK;
                break;
            case TiffConstants.COMPRESSION_CCITTFAX3:
                imagecomp = Element.CCITTG3_1D;
                paramsn |= Element.CCITT_ENDOFLINE | Element.CCITT_ENDOFBLOCK;
                var t4OptionsField = dir.GetField(TiffConstants.TIFFTAG_GROUP3OPTIONS);
                if (t4OptionsField != null)
                {
                    tiffT4Options = t4OptionsField.GetAsLong(0);
                    if ((tiffT4Options & TiffConstants.GROUP3OPT_2DENCODING) != 0)
                    {
                        imagecomp = Element.CCITTG3_2D;
                    }

                    if ((tiffT4Options & TiffConstants.GROUP3OPT_FILLBITS) != 0)
                    {
                        paramsn |= Element.CCITT_ENCODEDBYTEALIGN;
                    }
                }

                break;
            case TiffConstants.COMPRESSION_CCITTFAX4:
                imagecomp = Element.CCITTG4;
                var t6OptionsField = dir.GetField(TiffConstants.TIFFTAG_GROUP4OPTIONS);
                if (t6OptionsField != null)
                {
                    tiffT6Options = t6OptionsField.GetAsLong(0);
                }

                break;
        }

        if (direct && rowsStrip == h)
        {
            //single strip, direct
            var im = new byte[(int)size[0]];
            s.Seek(offset[0]);
            s.ReadFully(im);
            img = Image.GetInstance(w, h, false, imagecomp, paramsn, im);
            img.Inverted = true;
        }
        else
        {
            var rowsLeft = h;
            var g4 = new Ccittg4Encoder(w);
            for (var k = 0; k < offset.Length; ++k)
            {
                var im = new byte[(int)size[k]];
                s.Seek(offset[k]);
                s.ReadFully(im);
                var height = Math.Min(rowsStrip, rowsLeft);
                var decoder = new TiffFaxDecoder(fillOrder, w, height);
                var outBuf = new byte[(w + 7) / 8 * height];
                switch (compression)
                {
                    case TiffConstants.COMPRESSION_CCITTRLEW:
                    case TiffConstants.COMPRESSION_CCITTRLE:
                        decoder.Decode1D(outBuf, im, 0, height);
                        g4.Fax4Encode(outBuf, height);
                        break;
                    case TiffConstants.COMPRESSION_CCITTFAX3:
                        try
                        {
                            decoder.Decode2D(outBuf, im, 0, height, tiffT4Options);
                        }
                        catch (Exception e)
                        {
                            // let's flip the fill bits and try again...
                            tiffT4Options ^= TiffConstants.GROUP3OPT_FILLBITS;
                            try
                            {
                                decoder.Decode2D(outBuf, im, 0, height, tiffT4Options);
                            }
                            catch
                            {
                                throw e;
                            }
                        }

                        g4.Fax4Encode(outBuf, height);
                        break;
                    case TiffConstants.COMPRESSION_CCITTFAX4:
                        decoder.DecodeT6(outBuf, im, 0, height, tiffT6Options);
                        g4.Fax4Encode(outBuf, height);
                        break;
                }

                rowsLeft -= rowsStrip;
            }

            var g4Pic = g4.Close();
            img = Image.GetInstance(w, h, false, Element.CCITTG4, paramsn & Element.CCITT_BLACKIS1, g4Pic);
        }

        img.SetDpi(dpiX, dpiY);
        img.XyRatio = xyRatio;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_ICCPROFILE))
        {
            try
            {
                var fd = dir.GetField(TiffConstants.TIFFTAG_ICCPROFILE);
                var iccProf = IccProfile.GetInstance(fd.GetAsBytes());
                if (iccProf.NumComponents == 1)
                {
                    img.TagIcc = iccProf;
                }
            }
            catch
            {
                //empty
            }
        }

        img.OriginalType = Image.ORIGINAL_TIFF;
        if (rotation.ApproxNotEqual(0))
        {
            img.InitialRotation = rotation;
        }

        return img;
    }

    public static void Inflate(byte[] deflated, byte[] inflated)
    {
        if (inflated == null)
        {
            throw new ArgumentNullException(nameof(inflated));
        }

        var outp = PdfReader.FlateDecode(deflated);
        Array.Copy(outp, 0, inflated, 0, Math.Min(outp.Length, inflated.Length));
    }

    private static Image GetTiffImageColor(TiffDirectory dir, RandomAccessFileOrArray s)
    {
        var predictor = 1;
        TifflzwDecoder lzwDecoder = null;
        var compression = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_COMPRESSION);
        switch (compression)
        {
            case TiffConstants.COMPRESSION_NONE:
            case TiffConstants.COMPRESSION_LZW:
            case TiffConstants.COMPRESSION_PACKBITS:
            case TiffConstants.COMPRESSION_DEFLATE:
            case TiffConstants.COMPRESSION_ADOBE_DEFLATE:
            case TiffConstants.COMPRESSION_OJPEG:
            case TiffConstants.COMPRESSION_JPEG:
                break;
            default:
                throw new InvalidOperationException("The compression " + compression + " is not supported.");
        }

        var photometric = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_PHOTOMETRIC);
        switch (photometric)
        {
            case TiffConstants.PHOTOMETRIC_MINISWHITE:
            case TiffConstants.PHOTOMETRIC_MINISBLACK:
            case TiffConstants.PHOTOMETRIC_RGB:
            case TiffConstants.PHOTOMETRIC_SEPARATED:
            case TiffConstants.PHOTOMETRIC_PALETTE:
                break;
            default:
                if (compression != TiffConstants.COMPRESSION_OJPEG && compression != TiffConstants.COMPRESSION_JPEG)
                {
                    throw new InvalidOperationException("The photometric " + photometric + " is not supported.");
                }

                break;
        }

        float rotation = 0;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_ORIENTATION))
        {
            var rot = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_ORIENTATION);
            if (rot == TiffConstants.ORIENTATION_BOTRIGHT || rot == TiffConstants.ORIENTATION_BOTLEFT)
            {
                rotation = (float)Math.PI;
            }
            else if (rot == TiffConstants.ORIENTATION_LEFTTOP || rot == TiffConstants.ORIENTATION_LEFTBOT)
            {
                rotation = (float)(Math.PI / 2.0);
            }
            else if (rot == TiffConstants.ORIENTATION_RIGHTTOP || rot == TiffConstants.ORIENTATION_RIGHTBOT)
            {
                rotation = -(float)(Math.PI / 2.0);
            }
        }

        if (dir.IsTagPresent(TiffConstants.TIFFTAG_PLANARCONFIG)
            && dir.GetFieldAsLong(TiffConstants.TIFFTAG_PLANARCONFIG) == TiffConstants.PLANARCONFIG_SEPARATE)
        {
            throw new InvalidOperationException("Planar images are not supported.");
        }

        var extraSamples = 0;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_EXTRASAMPLES))
        {
            extraSamples = 1;
        }

        var samplePerPixel = 1;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_SAMPLESPERPIXEL)) // 1,3,4
        {
            samplePerPixel = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_SAMPLESPERPIXEL);
        }

        var bitsPerSample = 1;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_BITSPERSAMPLE))
        {
            bitsPerSample = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_BITSPERSAMPLE);
        }

        switch (bitsPerSample)
        {
            case 1:
            case 2:
            case 4:
            case 8:
                break;
            default:
                throw new InvalidOperationException("Bits per sample " + bitsPerSample + " is not supported.");
        }

        Image img = null;

        var h = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_IMAGELENGTH);
        var w = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_IMAGEWIDTH);
        var dpiX = 0;
        var dpiY = 0;
        var resolutionUnit = TiffConstants.RESUNIT_INCH;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_RESOLUTIONUNIT))
        {
            resolutionUnit = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_RESOLUTIONUNIT);
        }

        dpiX = getDpi(dir.GetField(TiffConstants.TIFFTAG_XRESOLUTION), resolutionUnit);
        dpiY = getDpi(dir.GetField(TiffConstants.TIFFTAG_YRESOLUTION), resolutionUnit);
        var fillOrder = 1;
        var reverse = false;
        var fillOrderField = dir.GetField(TiffConstants.TIFFTAG_FILLORDER);
        if (fillOrderField != null)
        {
            fillOrder = fillOrderField.GetAsInt(0);
        }

        reverse = fillOrder == TiffConstants.FILLORDER_LSB2MSB;
        var rowsStrip = h;
        if (dir.IsTagPresent(TiffConstants.TIFFTAG_ROWSPERSTRIP)) //another hack for broken tiffs
        {
            rowsStrip = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_ROWSPERSTRIP);
        }

        if (rowsStrip <= 0 || rowsStrip > h)
        {
            rowsStrip = h;
        }

        var offset = getArrayLongShort(dir, TiffConstants.TIFFTAG_STRIPOFFSETS);
        var size = getArrayLongShort(dir, TiffConstants.TIFFTAG_STRIPBYTECOUNTS);
        if ((size == null || (size.Length == 1 && (size[0] == 0 || size[0] + offset[0] > s.Length))) && h == rowsStrip)
        {
            // some TIFF producers are really lousy, so...
            size = new long[] { s.Length - (int)offset[0] };
        }

        if (compression == TiffConstants.COMPRESSION_LZW)
        {
            var predictorField = dir.GetField(TiffConstants.TIFFTAG_PREDICTOR);
            if (predictorField != null)
            {
                predictor = predictorField.GetAsInt(0);
                if (predictor != 1 && predictor != 2)
                {
                    throw new InvalidOperationException("Illegal value for Predictor in TIFF file.");
                }

                if (predictor == 2 && bitsPerSample != 8)
                {
                    throw new InvalidOperationException(bitsPerSample +
                                                        "-bit samples are not supported for Horizontal differencing Predictor.");
                }
            }

            lzwDecoder = new TifflzwDecoder(w, predictor,
                                            samplePerPixel);
        }

        var rowsLeft = h;
        MemoryStream stream = null;
        MemoryStream mstream = null;
        ZDeflaterOutputStream zip = null;
        ZDeflaterOutputStream mzip = null;
        
        if (extraSamples > 0) {
            mstream = new MemoryStream();
            mzip = new ZDeflaterOutputStream(mstream);
        }
        
        Ccittg4Encoder g4 = null;
        if (bitsPerSample == 1 && samplePerPixel == 1)
        {
            g4 = new Ccittg4Encoder(w);
        }
        else
        {
            stream = new MemoryStream();
            if (compression != TiffConstants.COMPRESSION_OJPEG && compression != TiffConstants.COMPRESSION_JPEG)
            {
                zip = new ZDeflaterOutputStream(stream);
            }
        }

        if (compression == TiffConstants.COMPRESSION_OJPEG)
        {
            // Assume that the TIFFTAG_JPEGIFBYTECOUNT tag is optional, since it's obsolete and
            // is often missing

            if (!dir.IsTagPresent(TiffConstants.TIFFTAG_JPEGIFOFFSET))
            {
                throw new IOException("Missing tag(s) for OJPEG compression.");
            }

            var jpegOffset = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_JPEGIFOFFSET);
            var jpegLength = s.Length - jpegOffset;

            if (dir.IsTagPresent(TiffConstants.TIFFTAG_JPEGIFBYTECOUNT))
            {
                jpegLength = (int)dir.GetFieldAsLong(TiffConstants.TIFFTAG_JPEGIFBYTECOUNT) +
                             (int)size[0];
            }

            var jpeg = new byte[Math.Min(jpegLength, s.Length - jpegOffset)];

            var posFilePointer = s.FilePointer;
            posFilePointer += jpegOffset;
            s.Seek(posFilePointer);
            s.ReadFully(jpeg);
            img = new Jpeg(jpeg);
        }
        else if (compression == TiffConstants.COMPRESSION_JPEG)
        {
            if (size.Length > 1)
            {
                throw new IOException("Compression JPEG is only supported with a single strip. This image has " +
                                      size.Length + " strips.");
            }

            var jpeg = new byte[(int)size[0]];
            s.Seek(offset[0]);
            s.ReadFully(jpeg);
            img = new Jpeg(jpeg);
        }
        else
        {
            for (var k = 0; k < offset.Length; ++k)
            {
                var im = new byte[(int)size[k]];
                s.Seek(offset[k]);
                s.ReadFully(im);
                var height = Math.Min(rowsStrip, rowsLeft);
                byte[] outBuf = null;
                if (compression != TiffConstants.COMPRESSION_NONE)
                {
                    outBuf = new byte[(w * bitsPerSample * samplePerPixel + 7) / 8 * height];
                }

                if (reverse)
                {
                    TiffFaxDecoder.ReverseBits(im);
                }

                switch (compression)
                {
                    case TiffConstants.COMPRESSION_DEFLATE:
                    case TiffConstants.COMPRESSION_ADOBE_DEFLATE:
                        Inflate(im, outBuf);
                        break;
                    case TiffConstants.COMPRESSION_NONE:
                        outBuf = im;
                        break;
                    case TiffConstants.COMPRESSION_PACKBITS:
                        DecodePackbits(im, outBuf);
                        break;
                    case TiffConstants.COMPRESSION_LZW:
                        lzwDecoder.Decode(im, outBuf, height);
                        break;
                }

                if (bitsPerSample == 1 && samplePerPixel == 1)
                {
                    g4.Fax4Encode(outBuf, height);
                }
                else
                {
                    if (extraSamples > 0)
                        processExtraSamples(zip, mzip, outBuf, samplePerPixel, bitsPerSample, w, height);
                    else
                        zip.Write(outBuf, 0, outBuf.Length);
                }

                rowsLeft -= rowsStrip;
            }

            if (bitsPerSample == 1 && samplePerPixel == 1)
            {
                img = Image.GetInstance(w, h, false, Element.CCITTG4,
                                        photometric == TiffConstants.PHOTOMETRIC_MINISBLACK
                                            ? Element.CCITT_BLACKIS1
                                            : 0, g4.Close());
            }
            else
            {
                zip.Close();
                img = Image.GetInstance(w, h, samplePerPixel - extraSamples, bitsPerSample, stream.ToArray());
                img.Deflated = true;
            }
        }

        img.SetDpi(dpiX, dpiY);
        if (compression != TiffConstants.COMPRESSION_OJPEG && compression != TiffConstants.COMPRESSION_JPEG)
        {
            if (dir.IsTagPresent(TiffConstants.TIFFTAG_ICCPROFILE))
            {
                try
                {
                    var fd = dir.GetField(TiffConstants.TIFFTAG_ICCPROFILE);
                    var iccProf = IccProfile.GetInstance(fd.GetAsBytes());
                    if (samplePerPixel - extraSamples == iccProf.NumComponents)
                    {
                        img.TagIcc = iccProf;
                    }
                }
                catch
                {
                    //empty
                }
            }

            if (dir.IsTagPresent(TiffConstants.TIFFTAG_COLORMAP))
            {
                var fd = dir.GetField(TiffConstants.TIFFTAG_COLORMAP);
                var rgb = fd.GetAsChars();
                var palette = new byte[rgb.Length];
                var gColor = rgb.Length / 3;
                var bColor = gColor * 2;
                for (var k = 0; k < gColor; ++k)
                {
                    palette[k * 3] = (byte)(rgb[k] >> 8);
                    palette[k * 3 + 1] = (byte)(rgb[k + gColor] >> 8);
                    palette[k * 3 + 2] = (byte)(rgb[k + bColor] >> 8);
                }

                var indexed = new PdfArray();
                indexed.Add(PdfName.Indexed);
                indexed.Add(PdfName.Devicergb);
                indexed.Add(new PdfNumber(gColor - 1));
                indexed.Add(new PdfString(palette));
                var additional = new PdfDictionary();
                additional.Put(PdfName.Colorspace, indexed);
                img.Additional = additional;
            }

            img.OriginalType = Image.ORIGINAL_TIFF;
        }

        if (photometric == TiffConstants.PHOTOMETRIC_MINISWHITE)
        {
            img.Inverted = true;
        }

        if (rotation.ApproxNotEqual(0))
        {
            img.InitialRotation = rotation;
        }
        
        if (extraSamples > 0) {
            mzip.Close();
            var mimg = Image.GetInstance(w, h, 1, bitsPerSample, mstream.ToArray());
            mimg.MakeMask();
            mimg.Deflated = true;
            img.ImageMask = mimg;
        }

        return img;
    }

    private static void processExtraSamples(ZDeflaterOutputStream zip, ZDeflaterOutputStream mzip, byte[] outBuf, int samplePerPixel, int bitsPerSample, int width, int height) {
        if (bitsPerSample == 8) {
            var mask = new byte[width * height];
            var mptr = 0;
            var optr = 0;
            var total = width * height * samplePerPixel;
            for (var k = 0; k < total; k += samplePerPixel) {
                for (var s = 0; s < samplePerPixel - 1; ++s) {
                    outBuf[optr++] = outBuf[k + s];
                }
                mask[mptr++] = outBuf[k + samplePerPixel - 1];
            }
            zip.Write(outBuf, 0, optr);
            mzip.Write(mask, 0, mptr);
        }
        else
            throw new InvalidOperationException("Extra samples are not supported.");
    }

    private static long[] getArrayLongShort(TiffDirectory dir, int tag)
    {
        var field = dir.GetField(tag);
        if (field == null)
        {
            return null;
        }

        long[] offset;
        if (field.GetType() == TiffField.TIFF_LONG)
        {
            offset = field.GetAsLongs();
        }
        else
        {
            // must be short
            var temp = field.GetAsChars();
            offset = new long[temp.Length];
            for (var k = 0; k < temp.Length; ++k)
            {
                offset[k] = temp[k];
            }
        }

        return offset;
    }

    private static int getDpi(TiffField fd, int resolutionUnit)
    {
        if (fd == null)
        {
            return 0;
        }

        var res = fd.GetAsRational(0);
        var frac = res[0] / (float)res[1];
        var dpi = 0;
        switch (resolutionUnit)
        {
            case TiffConstants.RESUNIT_INCH:
            case TiffConstants.RESUNIT_NONE:
                dpi = (int)(frac + 0.5);
                break;
            case TiffConstants.RESUNIT_CENTIMETER:
                dpi = (int)(frac * 2.54 + 0.5);
                break;
        }

        return dpi;
    }
}