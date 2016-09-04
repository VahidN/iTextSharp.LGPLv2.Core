using System;
using System.IO;
using System.Collections;
using iTextSharp.LGPLv2.Core.System.NetUtils;

namespace iTextSharp.text.pdf.codec
{
    /// <summary>
    /// Reads gif images of all types. All the images in a gif are read in the constructors
    /// and can be retrieved with other methods.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class GifImage
    {
        protected const int MaxStackSize = 4096;
        protected int BgColor;
        protected int BgIndex;
        protected byte[] Block = new byte[256];
        // current data block
        protected int BlockSize;

        protected int Delay;
        /// <summary>
        /// last graphic control extension info
        /// </summary>
        protected int Dispose;

        protected ArrayList Frames = new ArrayList();
        protected byte[] FromData;
        protected Uri FromUrl;
        protected bool GctFlag;
        protected int Height;
        protected Stream Inp;
        protected bool Interlace;
        protected int Ix, Iy, Iw, Ih;
        protected bool LctFlag;
        // local color table flag
        // interlace flag
        protected int LctSize;

        protected int MBpc;
        protected byte[] MCurrTable;
        protected int MGbpc;
        protected byte[] MGlobalTable;
        protected int MLineStride;
        protected byte[] MLocalTable;
        protected byte[] MOut;
        // background color index
        // background color
        protected int PixelAspect;

        protected byte[] Pixels;
        protected byte[] PixelStack;
        /// <summary>
        /// LZW decoder working arrays
        /// </summary>
        protected short[] Prefix;

        // max decoder pixel stack size
        protected byte[] Suffix;

        // delay in milliseconds
        protected int TransIndex;

        // 0=no action; 1=leave in place; 2=restore to bg; 3=restore to prev
        protected bool Transparency;

        protected int Width;            // full image width


        /// <summary>
        /// Reads gif images from an URL.
        /// @throws IOException on error
        /// </summary>
        /// <param name="url">the URL</param>
        public GifImage(Uri url)
        {
            FromUrl = url;
            Stream isp = null;
            try
            {
                isp = url.GetResponseStream();
                Process(isp);
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
        /// Reads gif images from a file.
        /// @throws IOException on error
        /// </summary>
        /// <param name="file">the file</param>
        public GifImage(string file) : this(Utilities.ToUrl(file))
        {
        }

        /// <summary>
        /// Reads gif images from a byte array.
        /// @throws IOException on error
        /// </summary>
        /// <param name="data">the byte array</param>
        public GifImage(byte[] data)
        {
            FromData = data;
            Stream isp = null;
            try
            {
                isp = new MemoryStream(data);
                Process(isp);
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
        /// Reads gif images from a stream. The stream isp not closed.
        /// @throws IOException on error
        /// </summary>
        /// <param name="isp">the stream</param>
        public GifImage(Stream isp)
        {
            Process(isp);
        }

        /// <summary>
        /// Gets the number of frames the gif has.
        /// </summary>
        /// <returns>the number of frames the gif has</returns>
        public int GetFrameCount()
        {
            return Frames.Count;
        }

        /// <summary>
        /// Gets the [x,y] position of the frame in reference to the
        /// logical screen.
        /// </summary>
        /// <param name="frame">the frame</param>
        /// <returns>the [x,y] position of the frame</returns>
        public int[] GetFramePosition(int frame)
        {
            GifFrame gf = (GifFrame)Frames[frame - 1];
            return new[] { gf.Ix, gf.Iy };

        }

        /// <summary>
        /// Gets the image from a frame. The first frame isp 1.
        /// </summary>
        /// <param name="frame">the frame to get the image from</param>
        /// <returns>the image</returns>
        public Image GetImage(int frame)
        {
            GifFrame gf = (GifFrame)Frames[frame - 1];
            return gf.Image;
        }
        /// <summary>
        /// Gets the logical screen. The images may be smaller and placed
        /// in some position in this screen to playback some animation.
        /// No image will be be bigger that this.
        /// </summary>
        /// <returns>the logical screen dimensions as [x,y]</returns>
        public int[] GetLogicalScreen()
        {
            return new[] { Width, Height };
        }

        internal void Process(Stream isp)
        {
            Inp = new BufferedStream(isp);
            ReadHeader();
            ReadContents();
            if (Frames.Count == 0)
                throw new IOException("The file does not contain any valid image.");
        }

        protected static int NewBpc(int bpc)
        {
            switch (bpc)
            {
                case 1:
                case 2:
                case 4:
                    break;
                case 3:
                    return 4;
                default:
                    return 8;
            }
            return bpc;
        }

        protected bool DecodeImageData()
        {
            int nullCode = -1;
            int npix = Iw * Ih;
            int available, clear, codeMask, codeSize, endOfInformation, inCode, oldCode,
            bits, code, count, i, datum, dataSize, first, top, bi;
            bool skipZero = false;

            if (Prefix == null)
                Prefix = new short[MaxStackSize];
            if (Suffix == null)
                Suffix = new byte[MaxStackSize];
            if (PixelStack == null)
                PixelStack = new byte[MaxStackSize + 1];

            MLineStride = (Iw * MBpc + 7) / 8;
            MOut = new byte[MLineStride * Ih];
            int pass = 1;
            int inc = Interlace ? 8 : 1;
            int line = 0;
            int xpos = 0;

            //  Initialize GIF data stream decoder.

            dataSize = Inp.ReadByte();
            clear = 1 << dataSize;
            endOfInformation = clear + 1;
            available = clear + 2;
            oldCode = nullCode;
            codeSize = dataSize + 1;
            codeMask = (1 << codeSize) - 1;
            for (code = 0; code < clear; code++)
            {
                Prefix[code] = 0;
                Suffix[code] = (byte)code;
            }

            //  Decode GIF pixel stream.

            datum = bits = count = first = top = bi = 0;

            for (i = 0; i < npix;)
            {
                if (top == 0)
                {
                    if (bits < codeSize)
                    {
                        //  Load bytes until there are enough bits for a code.
                        if (count == 0)
                        {
                            // Read a new data block.
                            count = ReadBlock();
                            if (count <= 0)
                            {
                                skipZero = true;
                                break;
                            }
                            bi = 0;
                        }
                        datum += (Block[bi] & 0xff) << bits;
                        bits += 8;
                        bi++;
                        count--;
                        continue;
                    }

                    //  Get the next code.

                    code = datum & codeMask;
                    datum >>= codeSize;
                    bits -= codeSize;

                    //  Interpret the code

                    if ((code > available) || (code == endOfInformation))
                        break;
                    if (code == clear)
                    {
                        //  Reset decoder.
                        codeSize = dataSize + 1;
                        codeMask = (1 << codeSize) - 1;
                        available = clear + 2;
                        oldCode = nullCode;
                        continue;
                    }
                    if (oldCode == nullCode)
                    {
                        PixelStack[top++] = Suffix[code];
                        oldCode = code;
                        first = code;
                        continue;
                    }
                    inCode = code;
                    if (code == available)
                    {
                        PixelStack[top++] = (byte)first;
                        code = oldCode;
                    }
                    while (code > clear)
                    {
                        PixelStack[top++] = Suffix[code];
                        code = Prefix[code];
                    }
                    first = Suffix[code] & 0xff;

                    //  Add a new string to the string table,

                    if (available >= MaxStackSize)
                        break;
                    PixelStack[top++] = (byte)first;
                    Prefix[available] = (short)oldCode;
                    Suffix[available] = (byte)first;
                    available++;
                    if (((available & codeMask) == 0) && (available < MaxStackSize))
                    {
                        codeSize++;
                        codeMask += available;
                    }
                    oldCode = inCode;
                }

                //  Pop a pixel off the pixel stack.

                top--;
                i++;

                SetPixel(xpos, line, PixelStack[top]);
                ++xpos;
                if (xpos >= Iw)
                {
                    xpos = 0;
                    line += inc;
                    if (line >= Ih)
                    {
                        if (Interlace)
                        {
                            do
                            {
                                pass++;
                                switch (pass)
                                {
                                    case 2:
                                        line = 4;
                                        break;
                                    case 3:
                                        line = 2;
                                        inc = 4;
                                        break;
                                    case 4:
                                        line = 1;
                                        inc = 2;
                                        break;
                                    default: // this shouldn't happen
                                        line = Ih - 1;
                                        inc = 0;
                                        break;
                                }
                            } while (line >= Ih);
                        }
                        else
                        {
                            line = Ih - 1; // this shouldn't happen
                            inc = 0;
                        }
                    }
                }
            }
            return skipZero;
        }

        /// <summary>
        /// Reads next variable length block from input.
        /// </summary>
        /// <returns>number of bytes stored in "buffer"</returns>
        protected int ReadBlock()
        {
            BlockSize = Inp.ReadByte();
            if (BlockSize <= 0)
                return BlockSize = 0;
            for (int k = 0; k < BlockSize; ++k)
            {
                int v = Inp.ReadByte();
                if (v < 0)
                {
                    return BlockSize = k;
                }
                Block[k] = (byte)v;
            }
            return BlockSize;
        }

        protected byte[] ReadColorTable(int bpc)
        {
            int ncolors = 1 << bpc;
            int nbytes = 3 * ncolors;
            bpc = NewBpc(bpc);
            byte[] table = new byte[(1 << bpc) * 3];
            readFully(table, 0, nbytes);
            return table;
        }

        protected void ReadContents()
        {
            // read GIF file content blocks
            bool done = false;
            while (!done)
            {
                int code = Inp.ReadByte();
                switch (code)
                {

                    case 0x2C:    // image separator
                        ReadImage();
                        break;

                    case 0x21:    // extension
                        code = Inp.ReadByte();
                        switch (code)
                        {

                            case 0xf9:    // graphics control extension
                                ReadGraphicControlExt();
                                break;

                            case 0xff:    // application extension
                                ReadBlock();
                                Skip();        // don't care
                                break;

                            default:    // uninteresting extension
                                Skip();
                                break;
                        }
                        break;

                    default:
                        done = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Reads Graphics Control Extension values
        /// </summary>
        protected void ReadGraphicControlExt()
        {
            Inp.ReadByte();    // block size
            int packed = Inp.ReadByte();   // packed fields
            Dispose = (packed & 0x1c) >> 2;   // disposal method
            if (Dispose == 0)
                Dispose = 1;   // elect to keep old image if discretionary
            Transparency = (packed & 1) != 0;
            Delay = ReadShort() * 10;   // delay inp milliseconds
            TransIndex = Inp.ReadByte();        // transparent color index
            Inp.ReadByte();                     // block terminator
        }

        /// <summary>
        /// Reads GIF file header information.
        /// </summary>
        protected void ReadHeader()
        {
            string id = "";
            for (int i = 0; i < 6; i++)
                id += (char)Inp.ReadByte();
            if (!id.StartsWith("GIF8"))
            {
                throw new IOException("Gif signature nor found.");
            }

            ReadLsd();
            if (GctFlag)
            {
                MGlobalTable = ReadColorTable(MGbpc);
            }
        }

        /// <summary>
        /// Reads next frame image
        /// </summary>
        protected void ReadImage()
        {
            Ix = ReadShort();    // (sub)image position & size
            Iy = ReadShort();
            Iw = ReadShort();
            Ih = ReadShort();

            int packed = Inp.ReadByte();
            LctFlag = (packed & 0x80) != 0;     // 1 - local color table flag
            Interlace = (packed & 0x40) != 0;   // 2 - interlace flag
            // 3 - sort flag
            // 4-5 - reserved
            LctSize = 2 << (packed & 7);        // 6-8 - local color table size
            MBpc = NewBpc(MGbpc);
            if (LctFlag)
            {
                MCurrTable = ReadColorTable((packed & 7) + 1);   // read table
                MBpc = NewBpc((packed & 7) + 1);
            }
            else
            {
                MCurrTable = MGlobalTable;
            }
            if (Transparency && TransIndex >= MCurrTable.Length / 3)
                Transparency = false;
            if (Transparency && MBpc == 1)
            { // Acrobat 5.05 doesn't like this combination
                byte[] tp = new byte[12];
                Array.Copy(MCurrTable, 0, tp, 0, 6);
                MCurrTable = tp;
                MBpc = 2;
            }
            bool skipZero = DecodeImageData();   // decode pixel data
            if (!skipZero)
                Skip();

            Image img = null;
            img = new ImgRaw(Iw, Ih, 1, MBpc, MOut);
            PdfArray colorspace = new PdfArray();
            colorspace.Add(PdfName.Indexed);
            colorspace.Add(PdfName.Devicergb);
            int len = MCurrTable.Length;
            colorspace.Add(new PdfNumber(len / 3 - 1));
            colorspace.Add(new PdfString(MCurrTable));
            PdfDictionary ad = new PdfDictionary();
            ad.Put(PdfName.Colorspace, colorspace);
            img.Additional = ad;
            if (Transparency)
            {
                img.Transparency = new[] { TransIndex, TransIndex };
            }
            img.OriginalType = Image.ORIGINAL_GIF;
            img.OriginalData = FromData;
            img.Url = FromUrl;
            GifFrame gf = new GifFrame();
            gf.Image = img;
            gf.Ix = Ix;
            gf.Iy = Iy;
            Frames.Add(gf);   // add image to frame list

            //ResetFrame();

        }

        /// <summary>
        /// Reads Logical Screen Descriptor
        /// </summary>
        protected void ReadLsd()
        {

            // logical screen size
            Width = ReadShort();
            Height = ReadShort();

            // packed fields
            int packed = Inp.ReadByte();
            GctFlag = (packed & 0x80) != 0;      // 1   : global color table flag
            MGbpc = (packed & 7) + 1;
            BgIndex = Inp.ReadByte();        // background color index
            PixelAspect = Inp.ReadByte();    // pixel aspect ratio
        }

        /// <summary>
        /// Reads next 16-bit value, LSB first
        /// </summary>
        protected int ReadShort()
        {
            // read 16-bit value, LSB first
            return Inp.ReadByte() | (Inp.ReadByte() << 8);
        }
        /// <summary>
        /// Resets frame state for reading next image.
        /// </summary>
        protected void ResetFrame()
        {
        }

        protected void SetPixel(int x, int y, int v)
        {
            if (MBpc == 8)
            {
                int pos = x + Iw * y;
                MOut[pos] = (byte)v;
            }
            else
            {
                int pos = MLineStride * y + x / (8 / MBpc);
                int vout = v << (8 - MBpc * (x % (8 / MBpc)) - MBpc);
                MOut[pos] |= (byte)vout;
            }
        }
        /// <summary>
        /// Skips variable length blocks up to and including
        /// next zero length block.
        /// </summary>
        protected void Skip()
        {
            do
            {
                ReadBlock();
            } while (BlockSize > 0);
        }

        private void readFully(byte[] b, int offset, int count)
        {
            while (count > 0)
            {
                int n = Inp.Read(b, offset, count);
                if (n <= 0)
                    throw new IOException("Insufficient data.");
                count -= n;
                offset += n;
            }
        }

        internal class GifFrame
        {
            internal Image Image;
            internal int Ix;
            internal int Iy;
        }
    }
}
