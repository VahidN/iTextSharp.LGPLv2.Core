using iTextSharp.LGPLv2.Core.System.NetUtils;

namespace iTextSharp.text;

/// <summary>
///     An  Jpeg2000  is the representation of a graphic element (JPEG)
///     that has to be inserted into the document
///     @see		Element
///     @see		Image
/// </summary>
public class Jpeg2000 : Image
{
    /// <summary>
    ///     public static final membervariables
    /// </summary>
    public const int JP2_BPCC = 0x62706363;

    public const int JP2_COLR = 0x636f6c72;
    public const int JP2_DBTL = 0x6474626c;
    public const int JP2_FTYP = 0x66747970;
    public const int JP2_IHDR = 0x69686472;
    public const int JP2_JP = 0x6a502020;
    public const int JP2_JP2 = 0x6a703220;
    public const int JP2_JP2C = 0x6a703263;
    public const int JP2_JP2H = 0x6a703268;
    public const int JP2_URL = 0x75726c20;
    public const int JPIP_JPIP = 0x6a706970;
    private int _boxLength;
    private int _boxType;
    private Stream _inp;

    /// <summary>
    ///     Constructors
    /// </summary>
    public Jpeg2000(Image image) : base(image)
    {
    }

    /// <summary>
    ///     Constructs a  Jpeg2000 -object, using an <VAR>url</VAR>.
    ///     @throws BadElementException
    ///     @throws IOException
    /// </summary>
    /// <param name="url">where the image can be found</param>
    public Jpeg2000(Uri url) : base(url)
    {
        processParameters();
    }

    /// <summary>
    ///     Constructs a  Jpeg2000 -object from memory.
    ///     @throws BadElementException
    ///     @throws IOException
    /// </summary>
    /// <param name="img">image</param>
    public Jpeg2000(byte[] img) : base((Uri)null)
    {
        rawData = img;
        originalData = img;
        processParameters();
    }

    /// <summary>
    ///     Constructs a  Jpeg2000 -object from memory.
    ///     @throws BadElementException
    ///     @throws IOException
    /// </summary>
    /// <param name="img">image.</param>
    /// <param name="width">you want the image to have</param>
    /// <param name="height">you want the image to have</param>
    public Jpeg2000(byte[] img, float width, float height) : this(img)
    {
        scaledWidth = width;
        scaledHeight = height;
    }

    public void Jp2_read_boxhdr()
    {
        _boxLength = Cio_read(4);
        _boxType = Cio_read(4);
        if (_boxLength == 1)
        {
            if (Cio_read(4) != 0)
            {
                throw new IOException("Cannot handle box sizes higher than 2^32");
            }

            _boxLength = Cio_read(4);
            if (_boxLength == 0)
            {
                throw new IOException("Unsupported box size == 0");
            }
        }
        else if (_boxLength == 0)
        {
            throw new IOException("Unsupported box size == 0");
        }
    }

    private int Cio_read(int n)
    {
        var v = 0;
        for (var i = n - 1; i >= 0; i--)
        {
            v += _inp.ReadByte() << (i << 3);
        }

        return v;
    }

    /// <summary>
    ///     This method checks if the image is a valid JPEG and processes some parameters.
    ///     @throws BadElementException
    ///     @throws IOException
    /// </summary>
    private void processParameters()
    {
        type = JPEG2000;
        originalType = ORIGINAL_JPEG2000;
        _inp = null;
        try
        {
            string errorId;
            if (rawData == null)
            {
                _inp = url.GetResponseStream();
                errorId = url.ToString();
            }
            else
            {
                _inp = new MemoryStream(rawData);
                errorId = "Byte array";
            }

            _boxLength = Cio_read(4);
            if (_boxLength == 0x0000000c)
            {
                _boxType = Cio_read(4);
                if (JP2_JP != _boxType)
                {
                    throw new IOException("Expected JP Marker");
                }

                if (0x0d0a870a != Cio_read(4))
                {
                    throw new IOException("Error with JP Marker");
                }

                Jp2_read_boxhdr();
                if (JP2_FTYP != _boxType)
                {
                    throw new IOException("Expected FTYP Marker");
                }

                Utilities.Skip(_inp, _boxLength - 8);
                Jp2_read_boxhdr();
                do
                {
                    if (JP2_JP2H != _boxType)
                    {
                        if (_boxType == JP2_JP2C)
                        {
                            throw new IOException("Expected JP2H Marker");
                        }

                        Utilities.Skip(_inp, _boxLength - 8);
                        Jp2_read_boxhdr();
                    }
                } while (JP2_JP2H != _boxType);

                Jp2_read_boxhdr();
                if (JP2_IHDR != _boxType)
                {
                    throw new IOException("Expected IHDR Marker");
                }

                scaledHeight = Cio_read(4);
                Top = scaledHeight;
                scaledWidth = Cio_read(4);
                Right = scaledWidth;
                bpc = -1;
            }
            else if ((uint)_boxLength == 0xff4fff51)
            {
                Utilities.Skip(_inp, 4);
                var x1 = Cio_read(4);
                var y1 = Cio_read(4);
                var x0 = Cio_read(4);
                var y0 = Cio_read(4);
                Utilities.Skip(_inp, 16);
                colorspace = Cio_read(2);
                bpc = 8;
                scaledHeight = y1 - y0;
                Top = scaledHeight;
                scaledWidth = x1 - x0;
                Right = scaledWidth;
            }
            else
            {
                throw new IOException("Not a valid Jpeg2000 file");
            }
        }
        finally
        {
            if (_inp != null)
            {
                try
                {
                    _inp.Dispose();
                }
                catch
                {
                }

                _inp = null;
            }
        }

        plainWidth = Width;
        plainHeight = Height;
    }
}