using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.rtf.direct;
using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationShppict  handles data destined for picture destinations
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfDestinationShppict : RtfDestination
{
    public const int FORMAT_BINARY = 1;

    /// <summary>
    ///     data
    /// </summary>
    /// <summary>
    ///     binN
    /// </summary>
    /// <summary>
    ///     0 = HEX, 1 = BINARY
    /// </summary>
    public const int FORMAT_HEXADECIMAL = 0;

    public const int ORIGINAL_GIF = 3;
    public const int ORIGINAL_NONE = 0;
    public const int ORIGINAL_PS = 7;
    public const int ORIGINAL_TIFF = 5;
    private const int Blipuid = 1;

    /// <summary>
    ///     blipupiN
    /// </summary>
    /// <summary>
    ///     private int unitsPerInch = 0;
    /// </summary>
    /// <summary>
    ///     bliptagN
    /// </summary>
    /// <summary>
    ///     private String tag = "";
    /// </summary>
    private const int Normal = 0;

    /// <summary>
    ///     private int state = NORMAL;
    /// </summary>
    /// <summary>
    ///     Constant for converting pixels to twips
    /// </summary>
    private const int PixelTwipsFactor = 15;

    private long _binaryLength;
    private StringBuilder _buffer = new();

    /// <summary>
    ///     piccropbN
    /// </summary>
    private int _cropBottom;

    /// <summary>
    ///     piccroplN
    /// </summary>
    private int _cropLeft;

    /// <summary>
    ///     piccroprN
    /// </summary>
    private int _cropRight;

    /// <summary>
    ///     picscaled - macpict setting
    /// </summary>
    /// <summary>
    ///     private bool scaled = false;
    /// </summary>
    /// <summary>
    ///     picprop
    /// </summary>
    /// <summary>
    ///     private bool inlinePicture = false;
    /// </summary>
    /// <summary>
    ///     defshp
    /// </summary>
    /// <summary>
    ///     private bool wordArt = false;
    /// </summary>
    /// <summary>
    ///     piccroptN
    /// </summary>
    private int _cropTop;

    private ByteBuffer _data;

    /// <summary>
    ///     metafileinfo
    /// </summary>
    /// <summary>
    ///     picbmp
    /// </summary>
    /// <summary>
    ///     private bool bitmap = false;
    /// </summary>
    /// <summary>
    ///     picbppN - Valid 1,4,8,24
    /// </summary>
    /// <summary>
    ///     private int bbp = 1;
    /// </summary>
    private int _dataFormat = FORMAT_HEXADECIMAL;

    private MemoryStream _dataOs;

    /// <summary>
    ///     picgoalN
    /// </summary>
    private long _desiredHeight;

    /// <summary>
    ///     picwgoalN
    /// </summary>
    private long _desiredWidth;

    /// <summary>
    ///     pichN
    /// </summary>
    private long _height;

    private StringBuilder _hexChars = new(0);

    /// <summary>
    ///     picscalexN
    /// </summary>
    private int _scaleX = 100;

    /// <summary>
    ///     picscaleyN
    /// </summary>
    private int _scaleY = 100;

    /// <summary>
    ///     pictsize
    /// </summary>
    /// <summary>
    ///     picwN Ext field if the picture is a Windows metafile; picture width in pixels if the picture is a bitmap or
    /// </summary>
    /// <summary>
    ///     from quickdraw
    /// </summary>
    private long _width;

    /// <summary>
    ///     bitapinfo
    /// </summary>
    /// <summary>
    ///     wbmbitspixelN - number of bits per pixel - 1 monochrome, 4 16 color, 8 256 color, 24 RGB - Default 1
    /// </summary>
    /// <summary>
    ///     private int bitsPerPixel = 1;
    /// </summary>
    /// <summary>
    ///     wbmplanesN - number of color planes - must be 1
    /// </summary>
    /// <summary>
    ///     private int planes = 1;
    /// </summary>
    /// <summary>
    ///     wbmwidthbytesN - number of bytes in each raster line
    /// </summary>
    /// <summary>
    ///     private int widthBytes = 0;
    /// </summary>
    public RtfDestinationShppict() : base(null)
    {
    }

    /// <summary>
    ///     shppict - Destination
    /// </summary>
    /// <summary>
    ///     nonshpict - Destination - SKIP THIS
    /// </summary>
    /// <summary>
    ///     macpict - Mac QuickDraw- NOT HANDLED
    /// </summary>
    /// <summary>
    ///     pmmetafileN - OS/2 Metafile - NOT HANDLED
    /// </summary>
    /// <summary>
    ///     N * Meaning
    /// </summary>
    /// <summary>
    ///     0x0004 PU_ARBITRARY
    /// </summary>
    /// <summary>
    ///     0x0008 PU_PELS
    /// </summary>
    /// <summary>
    ///     0x000C PU_LOMETRIC
    /// </summary>
    /// <summary>
    ///     0x0010 PU_HIMETRIC
    /// </summary>
    /// <summary>
    ///     0x0014 PU_LOENGLISH
    /// </summary>
    /// <summary>
    ///     0x0018 PU_HIENGLISH
    /// </summary>
    /// <summary>
    ///     0x001C PU_TWIPS
    /// </summary>
    /// <summary>
    ///     private int pmmetafile = 0;
    /// </summary>
    /// <summary>
    ///     wmetafileN Image.RIGINAL_WMF = 6;
    /// </summary>
    /// <summary>
    ///     N * Type
    /// </summary>
    /// <summary>
    ///     1 = MM_TEXT
    /// </summary>
    /// <summary>
    ///     2 = M_LOMETRIC
    /// </summary>
    /// <summary>
    ///     3 = MM_HIMETRIC
    /// </summary>
    /// <summary>
    ///     4 = MM_LOENGLISH
    /// </summary>
    /// <summary>
    ///     5 = MM_HIENGLISH
    /// </summary>
    /// <summary>
    ///     6 = MM_TWIPS
    /// </summary>
    /// <summary>
    ///     7 = MM_ISOTROPIC
    /// </summary>
    /// <summary>
    ///     8 = MM_ANISOTROPIC
    /// </summary>
    /// <summary>
    ///     dibitmapN - DIB - Convert to BMP?
    /// </summary>
    /// <summary>
    ///     wbitmapN Image.ORIGINAL_BMP = 4;
    /// </summary>
    /// <summary>
    ///     Constructs a new RtfDestinationShppict.
    /// </summary>
    public RtfDestinationShppict(RtfParser parser) : base(parser)
    {
    }

    /// <summary>
    ///     picttype
    /// </summary>
    public int PictureType { get; private set; } = Image.ORIGINAL_NONE;

    /// <summary>
    ///     emfblip - EMF (nhanced metafile) - NOT HANDLED
    /// </summary>
    /// <summary>
    ///     pngblip int ORIGINAL_PNG = 2;
    /// </summary>
    /// <summary>
    ///     jpegblip Image.ORIGINAL_JPEG = 1; ORIGINAL_JPEG2000 = 8;
    /// </summary>
    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
    /// </summary>
    public override bool CloseDestination()
    {
        if (RtfParser.IsImport())
        {
            if (_buffer.Length > 0)
            {
                writeBuffer();
            }
        }

        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
    /// </summary>
    public override bool HandleCharacter(int ch)
    {
        if (RtfParser.IsImport())
        {
            if (_buffer.Length > 254)
            {
                writeBuffer();
            }
        }

        if (_data == null)
        {
            _data = new ByteBuffer();
        }

        switch (_dataFormat)
        {
            case FORMAT_HEXADECIMAL:
                _hexChars.Append(ch);
                if (_hexChars.Length == 2)
                {
                    try
                    {
                        _data.Append((char)int.Parse(_hexChars.ToString(), NumberStyles.HexNumber,
                                                     CultureInfo.InvariantCulture));
                    }
                    catch
                    {
                    }

                    _hexChars = new StringBuilder();
                }

                break;
            case FORMAT_BINARY:
                if (_dataOs == null)
                {
                    _dataOs = new MemoryStream();
                }

                // HGS - FIX ME IF PROBLEM!
                _dataOs.WriteByte((byte)ch);
                // PNG signature should be.
                //             (decimal)              137  80  78  71  13  10  26  10
                //             (hexadecimal)           89  50  4e  47  0d  0a  1a  0a
                //             (ASCII C notation)    \211   P   N   G  \r  \n \032 \n

                _binaryLength--;
                if (_binaryLength == 0)
                {
                    _dataFormat = FORMAT_HEXADECIMAL;
                }

                break;
        }

        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
    /// </summary>
    public override bool HandleCloseGroup()
    {
        OnCloseGroup(); // event handler

        if (RtfParser.IsImport())
        {
            if (_buffer.Length > 0)
            {
                writeBuffer();
            }

            if (_dataOs != null)
            {
                addImage();
                _dataOs = null;
            }

            writeText("}");
            return true;
        }

        if (RtfParser.IsConvert())
        {
            if (_dataOs != null)
            {
                addImage();
                _dataOs = null;
            }
        }

        return true;
    }

    public override bool HandleControlWord(RtfCtrlWordData ctrlWordData)
    {
        if (ctrlWordData == null)
        {
            throw new ArgumentNullException(nameof(ctrlWordData));
        }

        var result = false;
        var skipCtrlWord = false;
        if (RtfParser.IsImport())
        {
            skipCtrlWord = true;
            if (ctrlWordData.CtrlWord.Equals("shppict", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("nonshppict", StringComparison.Ordinal))
            {
                // never gets here because this is a destination set to null
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("blipuid", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picprop", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("pict", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("emfblip", StringComparison.Ordinal))
            {
                result = true;
                PictureType = Image.ORIGINAL_NONE;
            }

            if (ctrlWordData.CtrlWord.Equals("pngblip", StringComparison.Ordinal))
            {
                result = true;
                PictureType = Image.ORIGINAL_PNG;
            }

            if (ctrlWordData.CtrlWord.Equals("jepgblip", StringComparison.Ordinal))
            {
                result = true;
                PictureType = Image.ORIGINAL_JPEG;
            }

            if (ctrlWordData.CtrlWord.Equals("macpict", StringComparison.Ordinal))
            {
                result = true;
                PictureType = Image.ORIGINAL_NONE;
            }

            if (ctrlWordData.CtrlWord.Equals("pmmetafile", StringComparison.Ordinal))
            {
                result = true;
                PictureType = Image.ORIGINAL_NONE;
            }

            if (ctrlWordData.CtrlWord.Equals("wmetafile", StringComparison.Ordinal))
            {
                result = true;
                PictureType = Image.ORIGINAL_WMF;
            }

            if (ctrlWordData.CtrlWord.Equals("dibitmap", StringComparison.Ordinal))
            {
                result = true;
                PictureType = Image.ORIGINAL_NONE;
            }

            if (ctrlWordData.CtrlWord.Equals("wbitmap", StringComparison.Ordinal))
            {
                result = true;
                PictureType = Image.ORIGINAL_BMP;
            }

            /* bitmap information */
            if (ctrlWordData.CtrlWord.Equals("wbmbitspixel", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("wbmplanes", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("wbmwidthbytes", StringComparison.Ordinal))
            {
                result = true;
            }

            /* picture size, scaling and cropping */
            if (ctrlWordData.CtrlWord.Equals("picw", StringComparison.Ordinal))
            {
                _width = ctrlWordData.LongValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("pich", StringComparison.Ordinal))
            {
                _height = ctrlWordData.LongValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picwgoal", StringComparison.Ordinal))
            {
                _desiredWidth = ctrlWordData.LongValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("pichgoal", StringComparison.Ordinal))
            {
                _desiredHeight = ctrlWordData.LongValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picscalex", StringComparison.Ordinal))
            {
                _scaleX = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picscaley", StringComparison.Ordinal))
            {
                _scaleY = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picscaled", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picprop", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("defshp", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("piccropt", StringComparison.Ordinal))
            {
                _cropTop = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("piccropb", StringComparison.Ordinal))
            {
                _cropBottom = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("piccropl", StringComparison.Ordinal))
            {
                _cropLeft = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("piccropr", StringComparison.Ordinal))
            {
                _cropRight = ctrlWordData.IntValue();
                result = true;
            }

            /* metafile information */
            if (ctrlWordData.CtrlWord.Equals("picbmp", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picbpp", StringComparison.Ordinal))
            {
                result = true;
            }

            /* picture data */
            if (ctrlWordData.CtrlWord.Equals("bin", StringComparison.Ordinal))
            {
                _dataFormat = FORMAT_BINARY;
                // set length to param
                _binaryLength = ctrlWordData.LongValue();
                RtfParser.SetTokeniserStateBinary(_binaryLength);
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("blipupi", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("blipuid", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("bliptag", StringComparison.Ordinal))
            {
                result = true;
            }
        }

        if (RtfParser.IsConvert())
        {
            if (ctrlWordData.CtrlWord.Equals("shppict", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("nonshppict", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("blipuid", StringComparison.Ordinal))
            {
                result = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("pict", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("emfblip", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("pngblip", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("jepgblip", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("macpict", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("pmmetafile", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("wmetafile", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("dibitmap", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("wbitmap", StringComparison.Ordinal))
            {
                result = true;
            }

            /* bitmap information */
            if (ctrlWordData.CtrlWord.Equals("wbmbitspixel", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("wbmplanes", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("wbmwidthbytes", StringComparison.Ordinal))
            {
                result = true;
            }

            /* picture size, scaling and cropping */
            if (ctrlWordData.CtrlWord.Equals("picw", StringComparison.Ordinal))
            {
                _width = ctrlWordData.LongValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("pich", StringComparison.Ordinal))
            {
                _height = ctrlWordData.LongValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picwgoal", StringComparison.Ordinal))
            {
                _desiredWidth = ctrlWordData.LongValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("pichgoal", StringComparison.Ordinal))
            {
                _desiredHeight = ctrlWordData.LongValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picscalex", StringComparison.Ordinal))
            {
                _scaleX = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picscaley", StringComparison.Ordinal))
            {
                _scaleY = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picscaled", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picprop", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("defshp", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("piccropt", StringComparison.Ordinal))
            {
                _cropTop = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("piccropb", StringComparison.Ordinal))
            {
                _cropBottom = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("piccropl", StringComparison.Ordinal))
            {
                _cropLeft = ctrlWordData.IntValue();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("piccropr", StringComparison.Ordinal))
            {
                _cropRight = ctrlWordData.IntValue();
                result = true;
            }

            /* metafile information */
            if (ctrlWordData.CtrlWord.Equals("picbmp", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("picbpp", StringComparison.Ordinal))
            {
                result = true;
            }

            /* picture data */
            if (ctrlWordData.CtrlWord.Equals("bin", StringComparison.Ordinal))
            {
                _dataFormat = FORMAT_BINARY;
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("blipupi", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("blipuid", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("bliptag", StringComparison.Ordinal))
            {
                result = true;
            }
        }

        if (!skipCtrlWord)
        {
            switch (RtfParser.GetConversionType())
            {
                case RtfParser.TYPE_IMPORT_FULL:
                    writeBuffer();
                    writeText(ctrlWordData.ToString());
                    result = true;
                    break;
                case RtfParser.TYPE_IMPORT_FRAGMENT:
                    writeBuffer();
                    writeText(ctrlWordData.ToString());
                    result = true;
                    break;
                case RtfParser.TYPE_CONVERT:
                    result = true;
                    break;
                default: // error because is should be an import or convert
                    result = false;
                    break;
            }
        }

        return result;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
    /// </summary>
    public override bool HandleOpenGroup()
    {
        OnOpenGroup(); // event handler

        if (RtfParser.IsImport())
        {
        }

        if (RtfParser.IsConvert())
        {
        }

        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
    /// </summary>
    public override bool HandleOpeningSubGroup()
    {
        if (RtfParser.IsImport())
        {
            if (_buffer.Length > 0)
            {
                writeBuffer();
            }
        }

        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#setDefaults()
    /// </summary>
    public override void SetToDefaults()
    {
        _buffer = new StringBuilder();
        _data = null;
        _width = 0;
        _height = 0;
        _desiredWidth = 0;
        _desiredHeight = 0;
        _scaleX = 100;
        _scaleY = 100;
        //this.scaled = false;
        //this.inlinePicture = false;
        //this.wordArt = false;
        _cropTop = 0;
        _cropBottom = 0;
        _cropLeft = 0;
        _cropRight = 0;
        //this.bitmap = false;
        //this.bbp = 1;
        _dataFormat = FORMAT_HEXADECIMAL;
        _binaryLength = 0;
        //this.unitsPerInch = 0;
        //this.tag = "";
    }

    private bool addImage()
    {
        Image img = null;
        try
        {
            img = Image.GetInstance(_dataOs.ToArray());
        }
        catch
        {
        }
        //                if (img != null) {
        //                    FileOutputStream out =null;
        //                    try {
        //                        out = new FileOutputStream("c:\\test.png");
        //                        out.Write(img.GetOriginalData());
        //                        out.Close();
        //                    } catch (FileNotFoundException e1) {
        //                        // TODO Auto-generated catch block
        //                        e1.PrintStackTrace();
        //                    } catch (IOException e1) {
        //                        // TODO Auto-generated catch block
        //                        e1.PrintStackTrace();
        //                    }

        if (img != null)
        {
            img.ScaleAbsolute((float)_desiredWidth / PixelTwipsFactor, (float)_desiredHeight / PixelTwipsFactor);
            img.ScaleAbsolute((float)_width / PixelTwipsFactor, (float)_height / PixelTwipsFactor);
            img.ScalePercent(_scaleX, _scaleY);

            try
            {
                if (RtfParser.IsImport())
                {
                    var doc = RtfParser.GetDocument();
                    doc.Add(img);
                }

                if (RtfParser.IsConvert())
                {
                    RtfParser.GetDocument().Add(img);
                }
            }
            catch
            {
            }
        }

        _dataFormat = FORMAT_HEXADECIMAL;
        return true;
    }

    private void writeBuffer()
    {
        writeText(_buffer.ToString());
    }

    private void writeText(string value)
    {
        if (RtfParser.GetState().NewGroup)
        {
            RtfParser.GetRtfDocument().Add(new RtfDirectContent("{"));
            RtfParser.GetState().NewGroup = false;
        }

        if (value.Length > 0)
        {
            RtfParser.GetRtfDocument().Add(new RtfDirectContent(value));
        }
    }
}