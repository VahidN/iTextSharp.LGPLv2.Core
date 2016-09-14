using System;
using System.IO;
using System.Text;
using System.util.zlib;
using System.util;
using iTextSharp.LGPLv2.Core.System.NetUtils;

namespace iTextSharp.text.pdf.codec
{
    /// <summary>
    /// Reads a PNG image. All types of PNG can be read.
    ///
    /// It is based in part in the JAI codec.
    /// @author  Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class PngImage
    {
        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string cHRM = "cHRM";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string gAMA = "gAMA";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string iCCP = "iCCP";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string IDAT = "IDAT";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string IEND = "IEND";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string IHDR = "IHDR";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string pHYs = "pHYs";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string PLTE = "PLTE";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string sRGB = "sRGB";

        /// <summary>
        /// A PNG marker.
        /// </summary>
        public const string tRNS = "tRNS";

        /// <summary>
        /// Some PNG specific values.
        /// </summary>
        public static int[] Pngid = { 137, 80, 78, 71, 13, 10, 26, 10 };
        private const int PngFilterAverage = 3;
        private const int PngFilterNone = 0;
        private const int PngFilterPaeth = 4;
        private const int PngFilterSub = 1;
        private const int PngFilterUp = 2;
        private const int Transfersize = 4096;
        private static readonly PdfName[] _intents = {PdfName.Perceptual,
            PdfName.Relativecolorimetric,PdfName.Saturation,PdfName.Absolutecolorimetric};

        readonly PdfDictionary _additional = new PdfDictionary();
        readonly MemoryStream _idat = new MemoryStream();
        readonly Stream _isp;
        int _bitDepth;
        int _bytesPerPixel;
        // number of bytes per input pixel
        byte[] _colorTable;

        int _colorType;
        int _compressionMethod;
        Stream _dataStream;
        int _dpiX;
        int _dpiY;
        int _filterMethod;
        float _gamma = 1f;
        bool _genBwMask;
        bool _hasChrm;
        int _height;
        IccProfile _iccProfile;
        byte[] _image;
        int _inputBands;
        PdfName _intent;
        int _interlaceMethod;
        bool _palShades;
        byte[] _smask;
        byte[] _trans;
        int _transBlue = -1;
        int _transGreen = -1;
        int _transRedGray = -1;
        int _width;
        float _xW, _yW, _xR, _yR, _xG, _yG, _xB, _yB;
        float _xyRatio;
        /// <summary>
        /// Creates a new instance of PngImage
        /// </summary>
        PngImage(Stream isp)
        {
            _isp = isp;
        }

        /// <summary>
        /// Reads a PNG from an url.
        /// @throws IOException on error
        /// </summary>
        /// <param name="url">the url</param>
        /// <returns>the image</returns>
        public static Image GetImage(Uri url)
        {
            Stream isp = null;
            try
            {
                isp = url.GetResponseStream();
                Image img = GetImage(isp);
                img.Url = url;
                return img;
            }
            finally
            {
                if (isp != null)
                {
                    isp.Dispose();
                }
            }
        }

        /// <summary>
        /// Reads a PNG from a stream.
        /// @throws IOException on error
        /// </summary>
        /// <param name="isp">the stream</param>
        /// <returns>the image</returns>
        public static Image GetImage(Stream isp)
        {
            PngImage png = new PngImage(isp);
            return png.getImage();
        }

        /// <summary>
        /// Reads a PNG from a file.
        /// @throws IOException on error
        /// </summary>
        /// <param name="file">the file</param>
        /// <returns>the image</returns>
        public static Image GetImage(string file)
        {
            return GetImage(Utilities.ToUrl(file));
        }

        /// <summary>
        /// Reads a PNG from a byte array.
        /// @throws IOException on error
        /// </summary>
        /// <param name="data">the byte array</param>
        /// <returns>the image</returns>
        public static Image GetImage(byte[] data)
        {
            Stream isp = new MemoryStream(data);
            Image img = GetImage(isp);
            img.OriginalData = data;
            return img;
        }

        public static int GetInt(Stream isp)
        {
            return (isp.ReadByte() << 24) + (isp.ReadByte() << 16) + (isp.ReadByte() << 8) + isp.ReadByte();
        }

        public static string GetString(Stream isp)
        {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                buf.Append((char)isp.ReadByte());
            }
            return buf.ToString();
        }

        public static int GetWord(Stream isp)
        {
            return (isp.ReadByte() << 8) + isp.ReadByte();
        }

        static bool checkMarker(string s)
        {
            if (s.Length != 4)
                return false;
            for (int k = 0; k < 4; ++k)
            {
                char c = s[k];
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
                    return false;
            }
            return true;
        }

        private static void decodeAverageFilter(byte[] curr, byte[] prev,
        int count, int bpp)
        {
            int raw, priorPixel, priorRow;

            for (int i = 0; i < bpp; i++)
            {
                raw = curr[i] & 0xff;
                priorRow = prev[i] & 0xff;

                curr[i] = (byte)(raw + priorRow / 2);
            }

            for (int i = bpp; i < count; i++)
            {
                raw = curr[i] & 0xff;
                priorPixel = curr[i - bpp] & 0xff;
                priorRow = prev[i] & 0xff;

                curr[i] = (byte)(raw + (priorPixel + priorRow) / 2);
            }
        }

        private static void decodePaethFilter(byte[] curr, byte[] prev,
        int count, int bpp)
        {
            int raw, priorPixel, priorRow, priorRowPixel;

            for (int i = 0; i < bpp; i++)
            {
                raw = curr[i] & 0xff;
                priorRow = prev[i] & 0xff;

                curr[i] = (byte)(raw + priorRow);
            }

            for (int i = bpp; i < count; i++)
            {
                raw = curr[i] & 0xff;
                priorPixel = curr[i - bpp] & 0xff;
                priorRow = prev[i] & 0xff;
                priorRowPixel = prev[i - bpp] & 0xff;

                curr[i] = (byte)(raw + paethPredictor(priorPixel,
                priorRow,
                priorRowPixel));
            }
        }

        private static void decodeSubFilter(byte[] curr, int count, int bpp)
        {
            for (int i = bpp; i < count; i++)
            {
                int val;

                val = curr[i] & 0xff;
                val += curr[i - bpp] & 0xff;

                curr[i] = (byte)val;
            }
        }

        private static void decodeUpFilter(byte[] curr, byte[] prev,
        int count)
        {
            for (int i = 0; i < count; i++)
            {
                int raw = curr[i] & 0xff;
                int prior = prev[i] & 0xff;

                curr[i] = (byte)(raw + prior);
            }
        }

        static int getPixel(byte[] image, int x, int y, int bitDepth, int bytesPerRow)
        {
            if (bitDepth == 8)
            {
                int pos = bytesPerRow * y + x;
                return image[pos] & 0xff;
            }
            else
            {
                int pos = bytesPerRow * y + x / (8 / bitDepth);
                int v = image[pos] >> (8 - bitDepth * (x % (8 / bitDepth)) - bitDepth);
                return v & ((1 << bitDepth) - 1);
            }
        }

        private static int paethPredictor(int a, int b, int c)
        {
            int p = a + b - c;
            int pa = Math.Abs(p - a);
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);

            if ((pa <= pb) && (pa <= pc))
            {
                return a;
            }
            else if (pb <= pc)
            {
                return b;
            }
            else
            {
                return c;
            }
        }

        /// <summary>
        /// Gets an  int  from an  Stream .
        /// </summary>
        private static void readFully(Stream inp, byte[] b, int offset, int count)
        {
            while (count > 0)
            {
                int n = inp.Read(b, offset, count);
                if (n <= 0)
                    throw new IOException("Insufficient data.");
                count -= n;
                offset += n;
            }
        }

        static void setPixel(byte[] image, int[] data, int offset, int size, int x, int y, int bitDepth, int bytesPerRow)
        {
            if (bitDepth == 8)
            {
                int pos = bytesPerRow * y + size * x;
                for (int k = 0; k < size; ++k)
                    image[pos + k] = (byte)data[k + offset];
            }
            else if (bitDepth == 16)
            {
                int pos = bytesPerRow * y + size * x;
                for (int k = 0; k < size; ++k)
                    image[pos + k] = (byte)(data[k + offset] >> 8);
            }
            else
            {
                int pos = bytesPerRow * y + x / (8 / bitDepth);
                int v = data[offset] << (8 - bitDepth * (x % (8 / bitDepth)) - bitDepth);
                image[pos] |= (byte)v;
            }
        }

        void decodeIdat()
        {
            int nbitDepth = _bitDepth;
            if (nbitDepth == 16)
                nbitDepth = 8;
            int size = -1;
            _bytesPerPixel = (_bitDepth == 16) ? 2 : 1;
            switch (_colorType)
            {
                case 0:
                    size = (nbitDepth * _width + 7) / 8 * _height;
                    break;
                case 2:
                    size = _width * 3 * _height;
                    _bytesPerPixel *= 3;
                    break;
                case 3:
                    if (_interlaceMethod == 1)
                        size = (nbitDepth * _width + 7) / 8 * _height;
                    _bytesPerPixel = 1;
                    break;
                case 4:
                    size = _width * _height;
                    _bytesPerPixel *= 2;
                    break;
                case 6:
                    size = _width * 3 * _height;
                    _bytesPerPixel *= 4;
                    break;
            }
            if (size >= 0)
                _image = new byte[size];
            if (_palShades)
                _smask = new byte[_width * _height];
            else if (_genBwMask)
                _smask = new byte[(_width + 7) / 8 * _height];
            _idat.Position = 0;
            _dataStream = new ZInflaterInputStream(_idat);

            if (_interlaceMethod != 1)
            {
                decodePass(0, 0, 1, 1, _width, _height);
            }
            else
            {
                decodePass(0, 0, 8, 8, (_width + 7) / 8, (_height + 7) / 8);
                decodePass(4, 0, 8, 8, (_width + 3) / 8, (_height + 7) / 8);
                decodePass(0, 4, 4, 8, (_width + 3) / 4, (_height + 3) / 8);
                decodePass(2, 0, 4, 4, (_width + 1) / 4, (_height + 3) / 4);
                decodePass(0, 2, 2, 4, (_width + 1) / 2, (_height + 1) / 4);
                decodePass(1, 0, 2, 2, _width / 2, (_height + 1) / 2);
                decodePass(0, 1, 1, 2, _width, _height / 2);
            }

        }

        void decodePass(int xOffset, int yOffset,
        int xStep, int yStep,
        int passWidth, int passHeight)
        {
            if ((passWidth == 0) || (passHeight == 0))
            {
                return;
            }

            int bytesPerRow = (_inputBands * passWidth * _bitDepth + 7) / 8;
            byte[] curr = new byte[bytesPerRow];
            byte[] prior = new byte[bytesPerRow];

            // Decode the (sub)image row-by-row
            int srcY, dstY;
            for (srcY = 0, dstY = yOffset;
            srcY < passHeight;
            srcY++, dstY += yStep)
            {
                // Read the filter type byte and a row of data
                int filter = 0;
                try
                {
                    filter = _dataStream.ReadByte();
                    readFully(_dataStream, curr, 0, bytesPerRow);
                }
                catch
                {
                    // empty on purpose
                }

                switch (filter)
                {
                    case PngFilterNone:
                        break;
                    case PngFilterSub:
                        decodeSubFilter(curr, bytesPerRow, _bytesPerPixel);
                        break;
                    case PngFilterUp:
                        decodeUpFilter(curr, prior, bytesPerRow);
                        break;
                    case PngFilterAverage:
                        decodeAverageFilter(curr, prior, bytesPerRow, _bytesPerPixel);
                        break;
                    case PngFilterPaeth:
                        decodePaethFilter(curr, prior, bytesPerRow, _bytesPerPixel);
                        break;
                    default:
                        // Error -- uknown filter type
                        throw new Exception("PNG filter unknown.");
                }

                processPixels(curr, xOffset, xStep, dstY, passWidth);

                // Swap curr and prior
                byte[] tmp = prior;
                prior = curr;
                curr = tmp;
            }
        }

        PdfObject getColorspace()
        {
            if (_iccProfile != null)
            {
                if ((_colorType & 2) == 0)
                    return PdfName.Devicegray;
                else
                    return PdfName.Devicergb;
            }
            if (_gamma.ApproxEquals(1f) && !_hasChrm)
            {
                if ((_colorType & 2) == 0)
                    return PdfName.Devicegray;
                else
                    return PdfName.Devicergb;
            }
            else
            {
                PdfArray array = new PdfArray();
                PdfDictionary dic = new PdfDictionary();
                if ((_colorType & 2) == 0)
                {
                    if (_gamma.ApproxEquals(1f))
                        return PdfName.Devicegray;
                    array.Add(PdfName.Calgray);
                    dic.Put(PdfName.Gamma, new PdfNumber(_gamma));
                    dic.Put(PdfName.Whitepoint, new PdfLiteral("[1 1 1]"));
                    array.Add(dic);
                }
                else
                {
                    PdfObject wp = new PdfLiteral("[1 1 1]");
                    array.Add(PdfName.Calrgb);
                    if (_gamma.ApproxNotEqual(1f))
                    {
                        PdfArray gm = new PdfArray();
                        PdfNumber n = new PdfNumber(_gamma);
                        gm.Add(n);
                        gm.Add(n);
                        gm.Add(n);
                        dic.Put(PdfName.Gamma, gm);
                    }
                    if (_hasChrm)
                    {
                        float z = _yW * ((_xG - _xB) * _yR - (_xR - _xB) * _yG + (_xR - _xG) * _yB);
                        float ya = _yR * ((_xG - _xB) * _yW - (_xW - _xB) * _yG + (_xW - _xG) * _yB) / z;
                        float xa = ya * _xR / _yR;
                        float za = ya * ((1 - _xR) / _yR - 1);
                        float yb = -_yG * ((_xR - _xB) * _yW - (_xW - _xB) * _yR + (_xW - _xR) * _yB) / z;
                        float xb = yb * _xG / _yG;
                        float zb = yb * ((1 - _xG) / _yG - 1);
                        float yc = _yB * ((_xR - _xG) * _yW - (_xW - _xG) * _yW + (_xW - _xR) * _yG) / z;
                        float xc = yc * _xB / _yB;
                        float zc = yc * ((1 - _xB) / _yB - 1);
                        float xw = xa + xb + xc;
                        float yw = 1;//YA+YB+YC;
                        float zw = za + zb + zc;
                        PdfArray wpa = new PdfArray();
                        wpa.Add(new PdfNumber(xw));
                        wpa.Add(new PdfNumber(yw));
                        wpa.Add(new PdfNumber(zw));
                        wp = wpa;
                        PdfArray matrix = new PdfArray();
                        matrix.Add(new PdfNumber(xa));
                        matrix.Add(new PdfNumber(ya));
                        matrix.Add(new PdfNumber(za));
                        matrix.Add(new PdfNumber(xb));
                        matrix.Add(new PdfNumber(yb));
                        matrix.Add(new PdfNumber(zb));
                        matrix.Add(new PdfNumber(xc));
                        matrix.Add(new PdfNumber(yc));
                        matrix.Add(new PdfNumber(zc));
                        dic.Put(PdfName.Matrix, matrix);
                    }
                    dic.Put(PdfName.Whitepoint, wp);
                    array.Add(dic);
                }
                return array;
            }
        }

        Image getImage()
        {
            readPng();
            int pal0 = 0;
            int palIdx = 0;
            _palShades = false;
            if (_trans != null)
            {
                for (int k = 0; k < _trans.Length; ++k)
                {
                    int n = _trans[k] & 0xff;
                    if (n == 0)
                    {
                        ++pal0;
                        palIdx = k;
                    }
                    if (n != 0 && n != 255)
                    {
                        _palShades = true;
                        break;
                    }
                }
            }
            if ((_colorType & 4) != 0)
                _palShades = true;
            _genBwMask = (!_palShades && (pal0 > 1 || _transRedGray >= 0));
            if (!_palShades && !_genBwMask && pal0 == 1)
            {
                _additional.Put(PdfName.Mask, new PdfLiteral("[" + palIdx + " " + palIdx + "]"));
            }
            bool needDecode = (_interlaceMethod == 1) || (_bitDepth == 16) || ((_colorType & 4) != 0) || _palShades || _genBwMask;
            switch (_colorType)
            {
                case 0:
                    _inputBands = 1;
                    break;
                case 2:
                    _inputBands = 3;
                    break;
                case 3:
                    _inputBands = 1;
                    break;
                case 4:
                    _inputBands = 2;
                    break;
                case 6:
                    _inputBands = 4;
                    break;
            }
            if (needDecode)
                decodeIdat();
            int components = _inputBands;
            if ((_colorType & 4) != 0)
                --components;
            int bpc = _bitDepth;
            if (bpc == 16)
                bpc = 8;
            Image img;
            if (_image != null)
            {
                if (_colorType == 3)
                    img = new ImgRaw(_width, _height, components, bpc, _image);
                else
                    img = Image.GetInstance(_width, _height, components, bpc, _image);
            }
            else
            {
                img = new ImgRaw(_width, _height, components, bpc, _idat.ToArray());
                img.Deflated = true;
                PdfDictionary decodeparms = new PdfDictionary();
                decodeparms.Put(PdfName.Bitspercomponent, new PdfNumber(_bitDepth));
                decodeparms.Put(PdfName.Predictor, new PdfNumber(15));
                decodeparms.Put(PdfName.Columns, new PdfNumber(_width));
                decodeparms.Put(PdfName.Colors, new PdfNumber((_colorType == 3 || (_colorType & 2) == 0) ? 1 : 3));
                _additional.Put(PdfName.Decodeparms, decodeparms);
            }
            if (_additional.Get(PdfName.Colorspace) == null)
                _additional.Put(PdfName.Colorspace, getColorspace());
            if (_intent != null)
                _additional.Put(PdfName.Intent, _intent);
            if (_additional.Size > 0)
                img.Additional = _additional;
            if (_iccProfile != null)
                img.TagIcc = _iccProfile;
            if (_palShades)
            {
                Image im2 = Image.GetInstance(_width, _height, 1, 8, _smask);
                im2.MakeMask();
                img.ImageMask = im2;
            }
            if (_genBwMask)
            {
                Image im2 = Image.GetInstance(_width, _height, 1, 1, _smask);
                im2.MakeMask();
                img.ImageMask = im2;
            }
            img.SetDpi(_dpiX, _dpiY);
            img.XyRatio = _xyRatio;
            img.OriginalType = Image.ORIGINAL_PNG;
            return img;
        }

        int[] getPixel(byte[] curr)
        {
            switch (_bitDepth)
            {
                case 8:
                    {
                        int[] outp = new int[curr.Length];
                        for (int k = 0; k < outp.Length; ++k)
                            outp[k] = curr[k] & 0xff;
                        return outp;
                    }
                case 16:
                    {
                        int[] outp = new int[curr.Length / 2];
                        for (int k = 0; k < outp.Length; ++k)
                            outp[k] = ((curr[k * 2] & 0xff) << 8) + (curr[k * 2 + 1] & 0xff);
                        return outp;
                    }
                default:
                    {
                        int[] outp = new int[curr.Length * 8 / _bitDepth];
                        int idx = 0;
                        int passes = 8 / _bitDepth;
                        int mask = (1 << _bitDepth) - 1;
                        for (int k = 0; k < curr.Length; ++k)
                        {
                            for (int j = passes - 1; j >= 0; --j)
                            {
                                outp[idx++] = Util.Usr(curr[k], _bitDepth * j) & mask;
                            }
                        }
                        return outp;
                    }
            }
        }

        void processPixels(byte[] curr, int xOffset, int step, int y, int width)
        {
            int srcX, dstX;

            int[] outp = getPixel(curr);
            int sizes = 0;
            switch (_colorType)
            {
                case 0:
                case 3:
                case 4:
                    sizes = 1;
                    break;
                case 2:
                case 6:
                    sizes = 3;
                    break;
            }
            if (_image != null)
            {
                dstX = xOffset;
                int yStride = (sizes * _width * (_bitDepth == 16 ? 8 : _bitDepth) + 7) / 8;
                for (srcX = 0; srcX < width; srcX++)
                {
                    setPixel(_image, outp, _inputBands * srcX, sizes, dstX, y, _bitDepth, yStride);
                    dstX += step;
                }
            }
            if (_palShades)
            {
                if ((_colorType & 4) != 0)
                {
                    if (_bitDepth == 16)
                    {
                        for (int k = 0; k < width; ++k)
                        {
                            int t = k * _inputBands + sizes;
                            outp[t] = Util.Usr(outp[t], 8);
                        }
                    }
                    int yStride = _width;
                    dstX = xOffset;
                    for (srcX = 0; srcX < width; srcX++)
                    {
                        setPixel(_smask, outp, _inputBands * srcX + sizes, 1, dstX, y, 8, yStride);
                        dstX += step;
                    }
                }
                else
                { //colorType 3
                    int yStride = _width;
                    int[] v = new int[1];
                    dstX = xOffset;
                    for (srcX = 0; srcX < width; srcX++)
                    {
                        int idx = outp[srcX];
                        if (idx < _trans.Length)
                            v[0] = _trans[idx];
                        else
                            v[0] = 255; // Patrick Valsecchi
                        setPixel(_smask, v, 0, 1, dstX, y, 8, yStride);
                        dstX += step;
                    }
                }
            }
            else if (_genBwMask)
            {
                switch (_colorType)
                {
                    case 3:
                        {
                            int yStride = (_width + 7) / 8;
                            int[] v = new int[1];
                            dstX = xOffset;
                            for (srcX = 0; srcX < width; srcX++)
                            {
                                int idx = outp[srcX];
                                v[0] = ((idx < _trans.Length && _trans[idx] == 0) ? 1 : 0);
                                setPixel(_smask, v, 0, 1, dstX, y, 1, yStride);
                                dstX += step;
                            }
                            break;
                        }
                    case 0:
                        {
                            int yStride = (_width + 7) / 8;
                            int[] v = new int[1];
                            dstX = xOffset;
                            for (srcX = 0; srcX < width; srcX++)
                            {
                                int g = outp[srcX];
                                v[0] = (g == _transRedGray ? 1 : 0);
                                setPixel(_smask, v, 0, 1, dstX, y, 1, yStride);
                                dstX += step;
                            }
                            break;
                        }
                    case 2:
                        {
                            int yStride = (_width + 7) / 8;
                            int[] v = new int[1];
                            dstX = xOffset;
                            for (srcX = 0; srcX < width; srcX++)
                            {
                                int markRed = _inputBands * srcX;
                                v[0] = (outp[markRed] == _transRedGray && outp[markRed + 1] == _transGreen
                                    && outp[markRed + 2] == _transBlue ? 1 : 0);
                                setPixel(_smask, v, 0, 1, dstX, y, 1, yStride);
                                dstX += step;
                            }
                            break;
                        }
                }
            }
        }

        void readPng()
        {
            for (int i = 0; i < Pngid.Length; i++)
            {
                if (Pngid[i] != _isp.ReadByte())
                {
                    throw new IOException("File is not a valid PNG.");
                }
            }
            byte[] buffer = new byte[Transfersize];
            while (true)
            {
                int len = GetInt(_isp);
                string marker = GetString(_isp);
                if (len < 0 || !checkMarker(marker))
                    throw new IOException("Corrupted PNG file.");
                if (IDAT.Equals(marker))
                {
                    int size;
                    while (len != 0)
                    {
                        size = _isp.Read(buffer, 0, Math.Min(len, Transfersize));
                        if (size <= 0)
                            return;
                        _idat.Write(buffer, 0, size);
                        len -= size;
                    }
                }
                else if (tRNS.Equals(marker))
                {
                    switch (_colorType)
                    {
                        case 0:
                            if (len >= 2)
                            {
                                len -= 2;
                                int gray = GetWord(_isp);
                                if (_bitDepth == 16)
                                    _transRedGray = gray;
                                else
                                    _additional.Put(PdfName.Mask, new PdfLiteral("[" + gray + " " + gray + "]"));
                            }
                            break;
                        case 2:
                            if (len >= 6)
                            {
                                len -= 6;
                                int red = GetWord(_isp);
                                int green = GetWord(_isp);
                                int blue = GetWord(_isp);
                                if (_bitDepth == 16)
                                {
                                    _transRedGray = red;
                                    _transGreen = green;
                                    _transBlue = blue;
                                }
                                else
                                    _additional.Put(PdfName.Mask, new PdfLiteral("[" + red + " " + red + " " + green + " " + green + " " + blue + " " + blue + "]"));
                            }
                            break;
                        case 3:
                            if (len > 0)
                            {
                                _trans = new byte[len];
                                for (int k = 0; k < len; ++k)
                                    _trans[k] = (byte)_isp.ReadByte();
                                len = 0;
                            }
                            break;
                    }
                    Utilities.Skip(_isp, len);
                }
                else if (IHDR.Equals(marker))
                {
                    _width = GetInt(_isp);
                    _height = GetInt(_isp);

                    _bitDepth = _isp.ReadByte();
                    _colorType = _isp.ReadByte();
                    _compressionMethod = _isp.ReadByte();
                    _filterMethod = _isp.ReadByte();
                    _interlaceMethod = _isp.ReadByte();
                }
                else if (PLTE.Equals(marker))
                {
                    if (_colorType == 3)
                    {
                        PdfArray colorspace = new PdfArray();
                        colorspace.Add(PdfName.Indexed);
                        colorspace.Add(getColorspace());
                        colorspace.Add(new PdfNumber(len / 3 - 1));
                        ByteBuffer colortable = new ByteBuffer();
                        while ((len--) > 0)
                        {
                            colortable.Append_i(_isp.ReadByte());
                        }
                        colorspace.Add(new PdfString(_colorTable = colortable.ToByteArray()));
                        _additional.Put(PdfName.Colorspace, colorspace);
                    }
                    else
                    {
                        Utilities.Skip(_isp, len);
                    }
                }
                else if (pHYs.Equals(marker))
                {
                    int dx = GetInt(_isp);
                    int dy = GetInt(_isp);
                    int unit = _isp.ReadByte();
                    if (unit == 1)
                    {
                        _dpiX = (int)(dx * 0.0254f + 0.5f);
                        _dpiY = (int)(dy * 0.0254f + 0.5f);
                    }
                    else
                    {
                        if (dy != 0)
                            _xyRatio = dx / (float)dy;
                    }
                }
                else if (cHRM.Equals(marker))
                {
                    _xW = GetInt(_isp) / 100000f;
                    _yW = GetInt(_isp) / 100000f;
                    _xR = GetInt(_isp) / 100000f;
                    _yR = GetInt(_isp) / 100000f;
                    _xG = GetInt(_isp) / 100000f;
                    _yG = GetInt(_isp) / 100000f;
                    _xB = GetInt(_isp) / 100000f;
                    _yB = GetInt(_isp) / 100000f;
                    _hasChrm = !(Math.Abs(_xW) < 0.0001f || Math.Abs(_yW) < 0.0001f || Math.Abs(_xR) < 0.0001f || Math.Abs(_yR) < 0.0001f || Math.Abs(_xG) < 0.0001f || Math.Abs(_yG) < 0.0001f || Math.Abs(_xB) < 0.0001f || Math.Abs(_yB) < 0.0001f);
                }
                else if (sRGB.Equals(marker))
                {
                    int ri = _isp.ReadByte();
                    _intent = _intents[ri];
                    _gamma = 2.2f;
                    _xW = 0.3127f;
                    _yW = 0.329f;
                    _xR = 0.64f;
                    _yR = 0.33f;
                    _xG = 0.3f;
                    _yG = 0.6f;
                    _xB = 0.15f;
                    _yB = 0.06f;
                    _hasChrm = true;
                }
                else if (gAMA.Equals(marker))
                {
                    int gm = GetInt(_isp);
                    if (gm != 0)
                    {
                        _gamma = 100000f / gm;
                        if (!_hasChrm)
                        {
                            _xW = 0.3127f;
                            _yW = 0.329f;
                            _xR = 0.64f;
                            _yR = 0.33f;
                            _xG = 0.3f;
                            _yG = 0.6f;
                            _xB = 0.15f;
                            _yB = 0.06f;
                            _hasChrm = true;
                        }
                    }
                }
                else if (iCCP.Equals(marker))
                {
                    do
                    {
                        --len;
                    } while (_isp.ReadByte() != 0);
                    _isp.ReadByte();
                    --len;
                    byte[] icccom = new byte[len];
                    int p = 0;
                    while (len > 0)
                    {
                        int r = _isp.Read(icccom, p, len);
                        if (r < 0)
                            throw new IOException("Premature end of file.");
                        p += r;
                        len -= r;
                    }
                    byte[] iccp = PdfReader.FlateDecode(icccom, true);
                    icccom = null;
                    try
                    {
                        _iccProfile = IccProfile.GetInstance(iccp);
                    }
                    catch
                    {
                        _iccProfile = null;
                    }
                }
                else if (IEND.Equals(marker))
                {
                    break;
                }
                else
                {
                    Utilities.Skip(_isp, len);
                }
                Utilities.Skip(_isp, 4);
            }
        }
    }
}
