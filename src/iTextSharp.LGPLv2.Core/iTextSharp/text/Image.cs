using iTextSharp.LGPLv2.Core.System.Drawing;
using iTextSharp.LGPLv2.Core.System.NetUtils;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec;
using SkiaSharp;

namespace iTextSharp.text;

/// <summary>
///     An Image is the representation of a graphic element (JPEG, PNG or GIF)
///     that has to be inserted into the document
/// </summary>
public abstract class Image : Rectangle
{
    /// <summary>
    ///     static membervariables (concerning the presence of borders)
    /// </summary>
    /// <summary> This represents a coordinate in the transformation matrix. </summary>
    public const int AX = 0;

    /// <summary> This represents a coordinate in the transformation matrix. </summary>
    public const int AY = 1;

    /// <summary> This represents a coordinate in the transformation matrix. </summary>
    public const int BX = 2;

    /// <summary> This represents a coordinate in the transformation matrix. </summary>
    public const int BY = 3;

    /// <summary> This represents a coordinate in the transformation matrix. </summary>
    public const int CX = 4;

    /// <summary> This represents a coordinate in the transformation matrix. </summary>
    public const int CY = 5;

    /// <summary> this is a kind of image Element. </summary>
    public const int DEFAULT = 0;

    /// <summary> This represents a coordinate in the transformation matrix. </summary>
    public const int DX = 6;

    /// <summary> This represents a coordinate in the transformation matrix. </summary>
    public const int DY = 7;

    /// <summary> this is a kind of image Element. </summary>
    public const int LEFT_ALIGN = 0;

    /// <summary> this is a kind of image Element. </summary>
    public const int MIDDLE_ALIGN = 1;

    /// <summary>
    ///     type of image
    /// </summary>
    public const int ORIGINAL_BMP = 4;

    /// <summary>
    ///     type of image
    /// </summary>
    public const int ORIGINAL_GIF = 3;

    /// <summary>
    ///     type of image
    ///     @since	2.1.5
    /// </summary>
    public const int ORIGINAL_JBIG2 = 9;

    /// <summary>
    ///     type of image
    /// </summary>
    public const int ORIGINAL_JPEG = 1;

    /// <summary>
    ///     type of image
    /// </summary>
    public const int ORIGINAL_JPEG2000 = 8;

    /// <summary>
    ///     type of image
    /// </summary>
    public const int ORIGINAL_NONE = 0;

    /// <summary>
    ///     type of image
    /// </summary>
    public const int ORIGINAL_PNG = 2;

    /// <summary>
    ///     type of image
    /// </summary>
    public const int ORIGINAL_TIFF = 5;

    /// <summary>
    ///     type of image
    /// </summary>
    public const int ORIGINAL_WMF = 6;

    /// <summary> this is a kind of image Element. </summary>
    public const int RIGHT_ALIGN = 2;

    /// <summary> this is a kind of image Element. </summary>
    public const int TEXTWRAP = 4;

    /// <summary> this is a kind of image Element. </summary>
    public const int UNDERLYING = 8;

    /// <summary>
    ///     serial stamping
    /// </summary>
    private static object _serialId = 0L;

    private static readonly object _mutex = new();

    /// <summary>
    ///     Holds value of property initialRotation.
    /// </summary>
    private float _initialRotation;

    /// <summary> This is the absolute X-position of the image. </summary>
    protected float absoluteX = float.NaN;

    /// <summary> This is the absolute Y-position of the image. </summary>
    protected float absoluteY = float.NaN;

    /// <summary> The alignment of the Image. </summary>
    protected int alignment;

    /// <summary> Text that can be shown instead of the image. </summary>
    protected string alt;

    /// <summary> if the annotation is not null the image will be clickable. </summary>
    protected Annotation annotation;

    /// <summary> this is the bits per component of the raw image. It also flags a CCITT image.</summary>
    protected int bpc = 1;

    /// <summary> this is the colorspace of a jpeg-image. </summary>
    protected int colorspace = -1;

    /// <summary>
    ///     The compression level of the content streams.
    ///     @since   2.1.3
    /// </summary>
    protected int compressionLevel = PdfStream.DEFAULT_COMPRESSION;

    /// <summary>
    ///     Holds value of property deflated.
    /// </summary>
    protected bool deflated;

    /// <summary> Holds value of property dpiX. </summary>
    protected int dpiX;

    /// <summary> Holds value of property dpiY. </summary>
    protected int dpiY;

    protected Image imageMask;

    /// <summary>
    ///     for the moment these variables are only used for Images in class Table
    /// </summary>
    /// <summary>
    ///     code contributed by Pelikan Stephan
    /// </summary>
    /// <summary>
    ///     the indentation to the left.
    /// </summary>
    protected float indentationLeft;

    /// <summary>
    ///     the indentation to the right.
    /// </summary>
    protected float indentationRight;

    /// <summary> Holds value of property interpolation. </summary>
    protected bool interpolation;

    /// <summary>
    ///     Image color inversion
    /// </summary>
    protected bool Invert;

    protected IPdfOcg layer;

    protected bool Mask;

    protected long mySerialId = GetSerialId();

    /// <summary>
    ///     Holds value of property originalData.
    /// </summary>
    protected byte[] originalData;

    /// <summary>
    ///     Holds value of property originalType.
    /// </summary>
    protected int originalType = ORIGINAL_NONE;

    /// <summary> This is the width of the image without rotation. </summary>
    protected float plainHeight;

    /// <summary> This is the width of the image without rotation. </summary>
    protected float plainWidth;

    /// <summary> ICC Profile attached </summary>
    protected IccProfile Profile;

    /// <summary> The raw data of the image. </summary>
    protected byte[] rawData;

    /// <summary> This is the rotation of the image. </summary>
    protected float RotationRadians;

    /// <summary> This is the original height of the image taking rotation into account. </summary>
    protected float scaledHeight;

    /// <summary> This is the scaled width of the image taking rotation into account. </summary>
    protected float scaledWidth;

    /// <summary>
    ///     The spacing after the image.
    /// </summary>
    protected float spacingAfter;

    /// <summary>
    ///     The spacing before the image.
    /// </summary>
    protected float spacingBefore;

    /// <summary> The template to be treated as an image. </summary>
    protected PdfTemplate[] Template = new PdfTemplate[1];

    /// <summary> this is the transparency information of the raw image</summary>
    protected int[] transparency;

    /// <summary> The imagetype. </summary>
    protected int type;

    /// <summary> The URL of the image. </summary>
    protected Uri url;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs an Image-object, using an url.
    /// </summary>
    /// <param name="url">the URL where the image can be found.</param>
    protected Image(Uri url) : base(0, 0)
    {
        this.url = url;
        alignment = DEFAULT;
        RotationRadians = 0;
    }

    /// <summary>
    ///     Constructs an Image object duplicate.
    /// </summary>
    /// <param name="image">another Image object.</param>
    protected Image(Image image) : base(image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        type = image.type;
        url = image.url;
        alignment = image.alignment;
        alt = image.alt;
        absoluteX = image.absoluteX;
        absoluteY = image.absoluteY;
        plainWidth = image.plainWidth;
        plainHeight = image.plainHeight;
        scaledWidth = image.scaledWidth;
        scaledHeight = image.scaledHeight;
        RotationRadians = image.RotationRadians;
        indentationLeft = image.indentationLeft;
        indentationRight = image.indentationRight;
        colorspace = image.colorspace;
        rawData = image.rawData;
        Template = image.Template;
        bpc = image.bpc;
        transparency = image.transparency;
        mySerialId = image.mySerialId;
        Invert = image.Invert;
        dpiX = image.dpiX;
        dpiY = image.dpiY;
        Mask = image.Mask;
        imageMask = image.imageMask;
        interpolation = image.interpolation;
        annotation = image.annotation;
        Profile = image.Profile;
        deflated = image.deflated;
        Additional = image.Additional;
        Smask = image.Smask;
        XyRatio = image.XyRatio;
        originalData = image.originalData;
        originalType = image.originalType;
        spacingAfter = image.spacingAfter;
        spacingBefore = image.spacingBefore;
        WidthPercentage = image.WidthPercentage;
        layer = image.layer;
        _initialRotation = image._initialRotation;
        DirectReference = image.DirectReference;
    }

    /// <summary>
    ///     Returns the absolute X position.
    /// </summary>
    /// <value>a position</value>
    public float AbsoluteX => absoluteX;

    /// <summary>
    ///     Returns the absolute Y position.
    /// </summary>
    /// <value>a position</value>
    public float AbsoluteY => absoluteY;

    public PdfDictionary Additional { get; set; }

    /// <summary>
    ///     Get/set the alignment for the image.
    /// </summary>
    /// <value>a value</value>
    public int Alignment
    {
        get => alignment;

        set => alignment = value;
    }

    /// <summary>
    ///     Get/set the alternative text for the image.
    /// </summary>
    /// <value>a string</value>
    public string Alt
    {
        get => alt;

        set => alt = value;
    }

    /// <summary>
    ///     Get/set the annotation.
    /// </summary>
    /// <value>the Annotation</value>
    public Annotation Annotation
    {
        get => annotation;

        set => annotation = value;
    }

    /// <summary>
    ///     Gets the bpc for the image.
    /// </summary>
    /// <remarks>
    ///     this only makes sense for Images of the type RawImage.
    /// </remarks>
    /// <value>a bpc value</value>
    public int Bpc => bpc;

    /// <summary>
    ///     Gets the colorspace for the image.
    /// </summary>
    /// <remarks>
    ///     this only makes sense for Images of the type Jpeg.
    /// </remarks>
    /// <value>a colorspace value</value>
    public int Colorspace => colorspace;

    /// <summary>
    ///     Sets the compression level to be used if the image is written as a compressed stream.
    ///     @since   2.1.3
    /// </summary>
    public int CompressionLevel
    {
        set
        {
            if (value < PdfStream.NO_COMPRESSION || value > PdfStream.BEST_COMPRESSION)
            {
                compressionLevel = PdfStream.DEFAULT_COMPRESSION;
            }
            else
            {
                compressionLevel = value;
            }
        }
        get => compressionLevel;
    }

    public bool Deflated
    {
        get => deflated;
        set => deflated = value;
    }

    public PdfIndirectReference DirectReference { set; get; }

    /// <summary>
    ///     Gets the dots-per-inch in the X direction. Returns 0 if not available.
    /// </summary>
    /// <value>the dots-per-inch in the X direction</value>
    public int DpiX => dpiX;

    /// <summary>
    ///     Gets the dots-per-inch in the Y direction. Returns 0 if not available.
    /// </summary>
    /// <value>the dots-per-inch in the Y direction</value>
    public int DpiY => dpiY;

    /// <summary>
    ///     Get/set the explicit masking.
    /// </summary>
    /// <value>the explicit masking</value>
    public Image ImageMask
    {
        get => imageMask;

        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (Mask)
            {
                throw new DocumentException("An image mask cannot contain another image mask.");
            }

            if (!value.Mask)
            {
                throw new DocumentException("The image mask is not a mask. Did you do MakeMask()?");
            }

            imageMask = value;
            Smask = value.bpc > 1 && value.bpc <= 8;
        }
    }

    public float IndentationLeft
    {
        get => indentationLeft;
        set => indentationLeft = value;
    }

    public float IndentationRight
    {
        get => indentationRight;
        set => indentationRight = value;
    }

    /// <summary>
    ///     Some image formats, like TIFF may present the images rotated that have
    ///     to be compensated.
    /// </summary>
    public float InitialRotation
    {
        get => _initialRotation;
        set
        {
            var oldRot = RotationRadians - _initialRotation;
            _initialRotation = value;
            Rotation = oldRot;
        }
    }

    /// <summary>
    ///     Sets the image interpolation. Image interpolation attempts to
    ///     produce a smooth transition between adjacent sample values.
    /// </summary>
    /// <value>New value of property interpolation.</value>
    public bool Interpolation
    {
        set => interpolation = value;
        get => interpolation;
    }

    /// <summary>
    ///     Inverts the meaning of the bits of a mask.
    /// </summary>
    /// <value>true to invert the meaning of the bits of a mask</value>
    public bool Inverted
    {
        set => Invert = value;
        get => Invert;
    }

    public IPdfOcg Layer
    {
        get => layer;
        set => layer = value;
    }

    /// <summary>
    ///     Returns the transformation matrix of the image.
    /// </summary>
    /// <value>an array [AX, AY, BX, BY, CX, CY, DX, DY]</value>
    public float[] Matrix
    {
        get
        {
            var matrix = new float[8];
            var cosX = (float)Math.Cos(RotationRadians);
            var sinX = (float)Math.Sin(RotationRadians);
            matrix[AX] = plainWidth * cosX;
            matrix[AY] = plainWidth * sinX;
            matrix[BX] = -plainHeight * sinX;
            matrix[BY] = plainHeight * cosX;
            if (RotationRadians < Math.PI / 2f)
            {
                matrix[CX] = matrix[BX];
                matrix[CY] = 0;
                matrix[DX] = matrix[AX];
                matrix[DY] = matrix[AY] + matrix[BY];
            }
            else if (RotationRadians < Math.PI)
            {
                matrix[CX] = matrix[AX] + matrix[BX];
                matrix[CY] = matrix[BY];
                matrix[DX] = 0;
                matrix[DY] = matrix[AY];
            }
            else if (RotationRadians < Math.PI * 1.5f)
            {
                matrix[CX] = matrix[AX];
                matrix[CY] = matrix[AY] + matrix[BY];
                matrix[DX] = matrix[BX];
                matrix[DY] = 0;
            }
            else
            {
                matrix[CX] = 0;
                matrix[CY] = matrix[AY];
                matrix[DX] = matrix[AX] + matrix[BX];
                matrix[DY] = matrix[BY];
            }

            return matrix;
        }
    }

    /// <summary>
    ///     returns serial id for this object
    /// </summary>
    public long MySerialId => mySerialId;

    public byte[] OriginalData
    {
        get => originalData;
        set => originalData = value;
    }

    public int OriginalType
    {
        get => originalType;
        set => originalType = value;
    }

    /// <summary>
    ///     Gets the plain height of the image.
    /// </summary>
    /// <value>a value</value>
    public float PlainHeight => plainHeight;

    /// <summary>
    ///     Gets the plain width of the image.
    /// </summary>
    /// <value>a value</value>
    public float PlainWidth => plainWidth;

    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Gets the raw data for the image.
    /// </summary>
    /// <remarks>
    ///     this only makes sense for Images of the type RawImage.
    /// </remarks>
    /// <value>the raw data</value>
    public byte[] RawData => rawData;

    /// <summary>
    ///     Sets the rotation of the image in radians.
    /// </summary>
    public new float Rotation
    {
        set
        {
            var d = Math.PI; //__IDS__
            RotationRadians = (float)((value + _initialRotation) % (2.0 * d)); //__IDS__
            if (RotationRadians < 0)
            {
                RotationRadians += (float)(2.0 * d); //__IDS__
            }

            var matrix = Matrix;
            scaledWidth = matrix[DX] - matrix[CX];
            scaledHeight = matrix[DY] - matrix[CY];
        }
    }

    /// <summary>
    ///     Sets the rotation of the image in degrees.
    /// </summary>
    public float RotationDegrees
    {
        set => Rotation = value / 180 * (float)Math.PI; //__IDS__
    }

    /// <summary>
    ///     Gets the scaled height of the image.
    /// </summary>
    /// <value>a value</value>
    public float ScaledHeight => scaledHeight;

    /// <summary>
    ///     Gets the scaled width of the image.
    /// </summary>
    /// <value>a value</value>
    public float ScaledWidth => scaledWidth;

    /// <summary>
    ///     Holds value of property smask.
    /// </summary>
    public bool Smask { get; set; }

    public float SpacingAfter
    {
        get => spacingAfter;
        set => spacingAfter = value;
    }

    public float SpacingBefore
    {
        get => spacingBefore;
        set => spacingBefore = value;
    }

    /// <summary>
    ///     Tags this image with an ICC profile.
    /// </summary>
    public IccProfile TagIcc
    {
        get => Profile;
        set => Profile = value;
    }

    /// <summary>
    ///     Get/set the template to be used as an image.
    /// </summary>
    /// <remarks>
    ///     this only makes sense for Images of the type ImgTemplate.
    /// </remarks>
    /// <value>the template</value>
    public PdfTemplate TemplateData
    {
        get => Template[0];

        set => Template[0] = value;
    }

    /// <summary>
    ///     Returns the transparency.
    /// </summary>
    /// <value>the transparency</value>
    public int[] Transparency
    {
        get => transparency;
        set => transparency = value;
    }

    /// <summary>
    ///     Returns the type.
    /// </summary>
    /// <value>a type</value>
    public override int Type => type;

    /// <summary>
    ///     Gets the string-representation of the reference to the image.
    /// </summary>
    /// <value>a string</value>
    public Uri Url
    {
        get => url;
        set => url = value;
    }

    /// <summary>
    ///     Holds value of property widthPercentage.
    /// </summary>
    public float WidthPercentage { get; set; } = 100;

    /// <summary>
    ///     Holds value of property XYRatio.
    /// </summary>
    public float XyRatio { get; set; }

    /// <summary>
    ///     Gets an instance of an Image.
    /// </summary>
    /// <param name="image">an Image</param>
    /// <returns>an object of type Gif, Jpeg or Png</returns>
    public static Image GetInstance(Image image) => image; //todo: Clone the image

    /// <summary>
    ///     Gets an instance of an Image.
    /// </summary>
    /// <param name="url">an URL</param>
    /// <returns>an object of type Gif, Jpeg or Png</returns>
    public static Image GetInstance(Uri url)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        // Add support for base64 encoded images.
        if (url.Scheme == "data")
        {
            var src = url.AbsoluteUri;
            if (IsBase64EncodedImage(src))
            {
                return GetBase64EncodedImage(src);
            }
        }

        using var istr = url.GetResponseStream();
        var c1 = istr.ReadByte();
        var c2 = istr.ReadByte();
        var c3 = istr.ReadByte();
        var c4 = istr.ReadByte();
        // jbig2
        var c5 = istr.ReadByte();
        var c6 = istr.ReadByte();
        var c7 = istr.ReadByte();
        var c8 = istr.ReadByte();

        if (c1 == 'G' && c2 == 'I' && c3 == 'F')
        {
            var gif = new GifImage(url);
            var img = gif.GetImage(1);
            return img;
        }

        if (c1 == 0xFF && c2 == 0xD8)
        {
            return new Jpeg(url);
        }

        if (c1 == 0x00 && c2 == 0x00 && c3 == 0x00 && c4 == 0x0c)
        {
            return new Jpeg2000(url);
        }

        if (c1 == 0xff && c2 == 0x4f && c3 == 0xff && c4 == 0x51)
        {
            return new Jpeg2000(url);
        }

        if (c1 == PngImage.Pngid[0] && c2 == PngImage.Pngid[1]
                                    && c3 == PngImage.Pngid[2] && c4 == PngImage.Pngid[3])
        {
            var img = PngImage.GetImage(url);
            return img;
        }

        if (c1 == 0xD7 && c2 == 0xCD)
        {
            Image img = new ImgWmf(url);
            return img;
        }

        if (c1 == 'B' && c2 == 'M')
        {
            var img = BmpImage.GetImage(url);
            return img;
        }

        if ((c1 == 'M' && c2 == 'M' && c3 == 0 && c4 == 42)
            || (c1 == 'I' && c2 == 'I' && c3 == 42 && c4 == 0))
        {
            RandomAccessFileOrArray ra = null;
            try
            {
                if (url.IsFile)
                {
                    var file = url.LocalPath;
                    ra = new RandomAccessFileOrArray(file);
                }
                else
                {
                    ra = new RandomAccessFileOrArray(url);
                }

                var img = TiffImage.GetTiffImage(ra, 1);
                img.url = url;
                return img;
            }
            finally
            {
                if (ra != null)
                {
                    ra.Close();
                }
            }
        }

        if (c1 == 0x97 && c2 == 'J' && c3 == 'B' && c4 == '2' &&
            c5 == '\r' && c6 == '\n' && c7 == 0x1a && c8 == '\n')
        {
            RandomAccessFileOrArray ra = null;
            try
            {
                if (url.IsFile)
                {
                    var file = url.LocalPath;
                    ra = new RandomAccessFileOrArray(file);
                }
                else
                {
                    ra = new RandomAccessFileOrArray(url);
                }

                var img = Jbig2Image.GetJbig2Image(ra, 1);
                img.url = url;
                return img;
            }
            finally
            {
                if (ra != null)
                {
                    ra.Close();
                }
            }
        }

        throw new IOException(url + " is not a recognized image format.");
    }

    private static Image GetBase64EncodedImage(string src)
    {
        // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
        var base64Data = src.Substring(src.IndexOf(",", StringComparison.OrdinalIgnoreCase) + 1);
        var imageData = Convert.FromBase64String(base64Data);
        return GetInstance(imageData);
    }

    private static bool IsBase64EncodedImage(string src) =>
        src.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase);

    public static Image GetInstance(Stream s)
    {
        var a = RandomAccessFileOrArray.InputStreamToArray(s);
        s.Dispose();
        return GetInstance(a);
    }

    /// <summary>
    ///     Creates a JBIG2 Image.
    ///     @since   2.1.5
    /// </summary>
    /// <param name="width">the width of the image</param>
    /// <param name="height">the height of the image</param>
    /// <param name="data">the raw image data</param>
    /// <param name="globals">JBIG2 globals</param>
    public static Image GetInstance(int width, int height, byte[] data, byte[] globals)
    {
        Image img = new ImgJbig2(width, height, data, globals);
        return img;
    }

    /// <summary>
    ///     Gets an instance of an Image.
    /// </summary>
    /// <param name="imgb">a byte array</param>
    /// <returns>an object of type Gif, Jpeg or Png</returns>
    public static Image GetInstance(byte[] imgb)
    {
        if (imgb == null)
        {
            throw new ArgumentNullException(nameof(imgb));
        }

        int c1 = imgb[0];
        int c2 = imgb[1];
        int c3 = imgb[2];
        int c4 = imgb[3];

        if (c1 == 'G' && c2 == 'I' && c3 == 'F')
        {
            var gif = new GifImage(imgb);
            return gif.GetImage(1);
        }

        if (c1 == 0xFF && c2 == 0xD8)
        {
            return new Jpeg(imgb);
        }

        if (c1 == 0x00 && c2 == 0x00 && c3 == 0x00 && c4 == 0x0c)
        {
            return new Jpeg2000(imgb);
        }

        if (c1 == 0xff && c2 == 0x4f && c3 == 0xff && c4 == 0x51)
        {
            return new Jpeg2000(imgb);
        }

        if (c1 == PngImage.Pngid[0] && c2 == PngImage.Pngid[1]
                                    && c3 == PngImage.Pngid[2] && c4 == PngImage.Pngid[3])
        {
            return PngImage.GetImage(imgb);
        }

        if (c1 == 0xD7 && c2 == 0xCD)
        {
            return new ImgWmf(imgb);
        }

        if (c1 == 'B' && c2 == 'M')
        {
            return BmpImage.GetImage(imgb);
        }

        if ((c1 == 'M' && c2 == 'M' && c3 == 0 && c4 == 42)
            || (c1 == 'I' && c2 == 'I' && c3 == 42 && c4 == 0))
        {
            RandomAccessFileOrArray ra = null;
            try
            {
                ra = new RandomAccessFileOrArray(imgb);
                var img = TiffImage.GetTiffImage(ra, 1);
                if (img.OriginalData == null)
                {
                    img.OriginalData = imgb;
                }

                return img;
            }
            finally
            {
                if (ra != null)
                {
                    ra.Close();
                }
            }
        }

        throw new IOException("The byte array is not a recognized imageformat.");
    }

    /// <summary>
    ///     Converts a .NET image to a Native(PNG, JPG, GIF, WMF) image
    /// </summary>
    /// <param name="image"></param>
    /// <param name="format"></param>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static Image GetInstance(SKBitmap image, SKEncodedImageFormat format, int quality = 100)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        using var data = image.Encode(format, quality);
        using var stream = new MemoryStream();
        data.SaveTo(stream);
        var imageArray = stream.ToArray();
        return GetInstance(imageArray);
    }

    /// <summary>
    ///     Gets an instance of an Image from a SkiaSharp.SKBitmap.
    /// </summary>
    /// <param name="image">the System.Drawing.Image to convert</param>
    /// <param name="color">
    ///     if different from null the transparency
    ///     pixels are replaced by this color
    /// </param>
    /// <param name="forceBw">if true the image is treated as black and white</param>
    /// <returns>an object of type ImgRaw</returns>
    public static Image GetInstance(SKBitmap image, BaseColor color, bool forceBw)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        var bm = image;
        var w = bm.Width;
        var h = bm.Height;
        var pxv = 0;
        if (forceBw)
        {
            var byteWidth = w / 8 + ((w & 7) != 0 ? 1 : 0);
            var pixelsByte = new byte[byteWidth * h];

            var index = 0;
            var size = h * w;
            var transColor = 1;
            if (color != null)
            {
                transColor = color.R + color.G + color.B < 384 ? 0 : 1;
            }

            int[] transparency = null;
            var cbyte = 0x80;
            var wMarker = 0;
            var currByte = 0;
            if (color != null)
            {
                for (var j = 0; j < h; j++)
                {
                    for (var i = 0; i < w; i++)
                    {
                        int alpha = bm.GetPixel(i, j).Alpha;
                        if (alpha < 250)
                        {
                            if (transColor == 1)
                            {
                                currByte |= cbyte;
                            }
                        }
                        else
                        {
                            if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
                            {
                                currByte |= cbyte;
                            }
                        }

                        cbyte >>= 1;
                        if (cbyte == 0 || wMarker + 1 >= w)
                        {
                            pixelsByte[index++] = (byte)currByte;
                            cbyte = 0x80;
                            currByte = 0;
                        }

                        ++wMarker;
                        if (wMarker >= w)
                        {
                            wMarker = 0;
                        }
                    }
                }
            }
            else
            {
                for (var j = 0; j < h; j++)
                {
                    for (var i = 0; i < w; i++)
                    {
                        if (transparency == null)
                        {
                            int alpha = bm.GetPixel(i, j).Alpha;
                            if (alpha == 0)
                            {
                                transparency = new int[2];
                                transparency[0] = transparency[1] = (bm.GetPixel(i, j).ToArgb() & 0x888) != 0 ? 1 : 0;
                            }
                        }

                        if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
                        {
                            currByte |= cbyte;
                        }

                        cbyte >>= 1;
                        if (cbyte == 0 || wMarker + 1 >= w)
                        {
                            pixelsByte[index++] = (byte)currByte;
                            cbyte = 0x80;
                            currByte = 0;
                        }

                        ++wMarker;
                        if (wMarker >= w)
                        {
                            wMarker = 0;
                        }
                    }
                }
            }

            return GetInstance(w, h, 1, 1, pixelsByte, transparency);
        }
        else
        {
            var pixelsByte = new byte[w * h * 3];
            byte[] smask = null;

            var index = 0;
            var size = h * w;
            var red = 255;
            var green = 255;
            var blue = 255;
            if (color != null)
            {
                red = color.R;
                green = color.G;
                blue = color.B;
            }

            int[] transparency = null;
            if (color != null)
            {
                for (var j = 0; j < h; j++)
                {
                    for (var i = 0; i < w; i++)
                    {
                        var alpha = (bm.GetPixel(i, j).ToArgb() >> 24) & 0xff;
                        if (alpha < 250)
                        {
                            pixelsByte[index++] = (byte)red;
                            pixelsByte[index++] = (byte)green;
                            pixelsByte[index++] = (byte)blue;
                        }
                        else
                        {
                            pxv = bm.GetPixel(i, j).ToArgb();
                            pixelsByte[index++] = (byte)((pxv >> 16) & 0xff);
                            pixelsByte[index++] = (byte)((pxv >> 8) & 0xff);
                            pixelsByte[index++] = (byte)(pxv & 0xff);
                        }
                    }
                }
            }
            else
            {
                var transparentPixel = 0;
                smask = new byte[w * h];
                var shades = false;
                var smaskPtr = 0;
                for (var j = 0; j < h; j++)
                {
                    for (var i = 0; i < w; i++)
                    {
                        pxv = bm.GetPixel(i, j).ToArgb();
                        var alpha = smask[smaskPtr++] = (byte)((pxv >> 24) & 0xff);
                        /* bugfix by Chris Nokleberg */
                        if (!shades)
                        {
                            if (alpha != 0 && alpha != 255)
                            {
                                shades = true;
                            }
                            else if (transparency == null)
                            {
                                if (alpha == 0)
                                {
                                    transparentPixel = pxv & 0xffffff;
                                    transparency = new int[6];
                                    transparency[0] = transparency[1] = (transparentPixel >> 16) & 0xff;
                                    transparency[2] = transparency[3] = (transparentPixel >> 8) & 0xff;
                                    transparency[4] = transparency[5] = transparentPixel & 0xff;
                                }
                            }
                            else if ((pxv & 0xffffff) != transparentPixel)
                            {
                                shades = true;
                            }
                        }

                        pixelsByte[index++] = (byte)((pxv >> 16) & 0xff);
                        pixelsByte[index++] = (byte)((pxv >> 8) & 0xff);
                        pixelsByte[index++] = (byte)(pxv & 0xff);
                    }
                }

                if (shades)
                {
                    transparency = null;
                }
                else
                {
                    smask = null;
                }
            }

            var img = GetInstance(w, h, 3, 8, pixelsByte, transparency);
            if (smask != null)
            {
                var sm = GetInstance(w, h, 1, 8, smask);
                sm.MakeMask();
                img.ImageMask = sm;
            }

            return img;
        }
    }

    /// <summary>
    ///     Gets an instance of an Image from a System.Drawing.Image.
    /// </summary>
    /// <param name="image">the System.Drawing.Image to convert</param>
    /// <param name="color">
    ///     if different from null the transparency
    ///     pixels are replaced by this color
    /// </param>
    /// <returns>an object of type ImgRaw</returns>
    public static Image GetInstance(SKBitmap image, BaseColor color) => GetInstance(image, color, false);

    /// <summary>
    ///     Gets an instance of an Image.
    /// </summary>
    /// <param name="filename">a filename</param>
    /// <returns>an object of type Gif, Jpeg or Png</returns>
    public static Image GetInstance(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        return IsBase64EncodedImage(filename)
                   ? GetBase64EncodedImage(filename)
                   : GetInstance(Utilities.ToUrl(filename));
    }

    /// <summary>
    ///     Gets an instance of an Image in raw mode.
    /// </summary>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
    /// <param name="bpc">bits per component</param>
    /// <param name="data">the image data</param>
    /// <returns>an object of type ImgRaw</returns>
    public static Image GetInstance(int width, int height, int components, int bpc, byte[] data) =>
        GetInstance(width, height, components, bpc, data, null);

    /// <summary>
    ///     Reuses an existing image.
    ///     @throws BadElementException on error
    /// </summary>
    /// <param name="iref">the reference to the image dictionary</param>
    /// <returns>the image</returns>
    public static Image GetInstance(PrIndirectReference iref)
    {
        var dic = (PdfDictionary)PdfReader.GetPdfObjectRelease(iref);
        var width = ((PdfNumber)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Width))).IntValue;
        var height = ((PdfNumber)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Height))).IntValue;
        Image imask = null;
        var obj = dic.Get(PdfName.Smask);
        if (obj != null && obj.IsIndirect())
        {
            imask = GetInstance((PrIndirectReference)obj);
        }
        else
        {
            obj = dic.Get(PdfName.Mask);
            if (obj != null && obj.IsIndirect())
            {
                var obj2 = PdfReader.GetPdfObjectRelease(obj);
                if (obj2 is PdfDictionary)
                {
                    imask = GetInstance((PrIndirectReference)obj);
                }
            }
        }

        Image img = new ImgRaw(width, height, 1, 1, null);
        img.imageMask = imask;
        img.DirectReference = iref;
        return img;
    }

    /// <summary>
    ///     Gets an instance of an Image in raw mode.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    public static Image GetInstance(PdfTemplate template) => new ImgTemplate(template);

    /// <summary>
    ///     Gets an instance of an Image in raw mode.
    /// </summary>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <param name="reverseBits"></param>
    /// <param name="typeCcitt"></param>
    /// <param name="parameters"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static Image
        GetInstance(int width, int height, bool reverseBits, int typeCcitt, int parameters, byte[] data) =>
        GetInstance(width, height, reverseBits, typeCcitt, parameters, data, null);

    /// <summary>
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="reverseBits"></param>
    /// <param name="typeCcitt"></param>
    /// <param name="parameters"></param>
    /// <param name="data"></param>
    /// <param name="transparency"></param>
    /// <returns></returns>
    public static Image GetInstance(int width,
                                    int height,
                                    bool reverseBits,
                                    int typeCcitt,
                                    int parameters,
                                    byte[] data,
                                    int[] transparency)
    {
        if (transparency != null && transparency.Length != 2)
        {
            throw new BadElementException("Transparency length must be equal to 2 with CCITT images");
        }

        Image img = new ImgCcitt(width, height, reverseBits, typeCcitt, parameters, data);
        img.transparency = transparency;
        return img;
    }

    /// <summary>
    ///     Gets an instance of an Image in raw mode.
    /// </summary>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
    /// <param name="bpc">bits per component</param>
    /// <param name="data">the image data</param>
    /// <param name="transparency">
    ///     transparency information in the Mask format of the
    ///     image dictionary
    /// </param>
    /// <returns>an object of type ImgRaw</returns>
    public static Image GetInstance(int width, int height, int components, int bpc, byte[] data, int[] transparency)
    {
        if (transparency != null && transparency.Length != components * 2)
        {
            throw new BadElementException("Transparency length must be equal to (componentes * 2)");
        }

        if (components == 1 && bpc == 1)
        {
            var g4 = Ccittg4Encoder.Compress(data, width, height);
            return GetInstance(width, height, false, CCITTG4, CCITT_BLACKIS1, g4, transparency);
        }

        Image img = new ImgRaw(width, height, components, bpc, data);
        img.transparency = transparency;
        return img;
    }

    /// <summary>
    ///     methods to set information
    /// </summary>
    /// <summary>
    ///     Gets the current image rotation in radians.
    /// </summary>
    /// <returns>the current image rotation in radians</returns>
    public float GetImageRotation()
    {
        var rot = (float)((RotationRadians - _initialRotation) % (2.0 * Math.PI));
        if (rot < 0)
        {
            rot += (float)(2.0 * Math.PI);
        }

        return rot;
    }

    /// <summary>
    ///     Checks if the Images has to be added at an absolute position.
    /// </summary>
    /// <returns>a bool</returns>
    public bool HasAbsolutePosition() => !float.IsNaN(absoluteY);

    /// <summary>
    ///     Checks if the Images has to be added at an absolute X position.
    /// </summary>
    /// <returns>a bool</returns>
    public bool HasAbsoluteX() => !float.IsNaN(absoluteX);

    /// <summary>
    ///     Checks is the image has an ICC profile.
    /// </summary>
    /// <returns>the ICC profile or null</returns>
    public bool HasIccProfile() => Profile != null;

    /// <summary>
    ///     Returns true if the image is a ImgRaw-object.
    /// </summary>
    /// <returns>a bool</returns>
    public bool IsImgRaw() => type == IMGRAW;

    /// <summary>
    ///     Returns true if the image is an ImgTemplate-object.
    /// </summary>
    /// <returns>a bool</returns>
    public bool IsImgTemplate() => type == IMGTEMPLATE;

    /// <summary>
    ///     Returns true if the image is a Jpeg-object.
    /// </summary>
    /// <returns>a bool</returns>
    public bool IsJpeg() => type == JPEG;

    /// <summary>
    ///     Returns true if this Image is a mask.
    /// </summary>
    /// <returns>true if this Image is a mask</returns>
    public bool IsMask() => Mask;

    /// <summary>
    ///     Returns true if this Image has the
    ///     requisites to be a mask.
    /// </summary>
    /// <returns>true if this Image can be a mask</returns>
    public bool IsMaskCandidate()
    {
        if (type == IMGRAW)
        {
            if (bpc > 0xff)
            {
                return true;
            }
        }

        return colorspace == 1;
    }

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public override bool IsNestable() => true;

    /// <summary>
    ///     Make this Image a mask.
    /// </summary>
    public void MakeMask()
    {
        if (!IsMaskCandidate())
        {
            throw new DocumentException("This image can not be an image mask.");
        }

        Mask = true;
    }

    /// <summary>
    ///     Scale the image to an absolute width and an absolute height.
    /// </summary>
    /// <param name="newWidth">the new width</param>
    /// <param name="newHeight">the new height</param>
    public void ScaleAbsolute(float newWidth, float newHeight)
    {
        plainWidth = newWidth;
        plainHeight = newHeight;
        var matrix = Matrix;
        scaledWidth = matrix[DX] - matrix[CX];
        scaledHeight = matrix[DY] - matrix[CY];
        WidthPercentage = 0;
    }

    /// <summary>
    ///     Scale the image to an absolute height.
    /// </summary>
    /// <param name="newHeight">the new height</param>
    public void ScaleAbsoluteHeight(float newHeight)
    {
        plainHeight = newHeight;
        var matrix = Matrix;
        scaledWidth = matrix[DX] - matrix[CX];
        scaledHeight = matrix[DY] - matrix[CY];
        WidthPercentage = 0;
    }

    /// <summary>
    ///     Scale the image to an absolute width.
    /// </summary>
    /// <param name="newWidth">the new width</param>
    public void ScaleAbsoluteWidth(float newWidth)
    {
        plainWidth = newWidth;
        var matrix = Matrix;
        scaledWidth = matrix[DX] - matrix[CX];
        scaledHeight = matrix[DY] - matrix[CY];
        WidthPercentage = 0;
    }

    /// <summary>
    ///     Scale the image to a certain percentage.
    /// </summary>
    /// <param name="percent">the scaling percentage</param>
    public void ScalePercent(float percent)
    {
        ScalePercent(percent, percent);
    }

    /// <summary>
    ///     Scale the width and height of an image to a certain percentage.
    /// </summary>
    /// <param name="percentX">the scaling percentage of the width</param>
    /// <param name="percentY">the scaling percentage of the height</param>
    public void ScalePercent(float percentX, float percentY)
    {
        plainWidth = Width * percentX / 100f;
        plainHeight = Height * percentY / 100f;
        var matrix = Matrix;
        scaledWidth = matrix[DX] - matrix[CX];
        scaledHeight = matrix[DY] - matrix[CY];
        WidthPercentage = 0;
    }

    /// <summary>
    ///     Scales the image so that it fits a certain width and height.
    /// </summary>
    /// <param name="fitWidth">the width to fit</param>
    /// <param name="fitHeight">the height to fit</param>
    public void ScaleToFit(float fitWidth, float fitHeight)
    {
        ScalePercent(100);
        var percentX = fitWidth * 100 / ScaledWidth;
        var percentY = fitHeight * 100 / ScaledHeight;
        ScalePercent(percentX < percentY ? percentX : percentY);
        WidthPercentage = 0;
    }

    /// <summary>
    ///     Sets the absolute position of the Image.
    /// </summary>
    /// <param name="absoluteX"></param>
    /// <param name="absoluteY"></param>
    public void SetAbsolutePosition(float absoluteX, float absoluteY)
    {
        this.absoluteX = absoluteX;
        this.absoluteY = absoluteY;
    }

    /// <summary>
    ///     Sets the dots per inch value
    ///     dpi for x coordinates
    ///     dpi for y coordinates
    /// </summary>
    /// <param name="dpiX"></param>
    /// <param name="dpiY"></param>
    public void SetDpi(int dpiX, int dpiY)
    {
        this.dpiX = dpiX;
        this.dpiY = dpiY;
    }

    /// <summary>
    ///     Replaces CalRGB and CalGray colorspaces with DeviceRGB and DeviceGray.
    /// </summary>
    public void SimplifyColorspace()
    {
        if (Additional == null)
        {
            return;
        }

        var value = Additional.GetAsArray(PdfName.Colorspace);
        if (value == null)
        {
            return;
        }

        var cs = simplifyColorspace(value);
        PdfObject newValue;
        if (cs.IsName())
        {
            newValue = cs;
        }
        else
        {
            newValue = value;
            var first = value.GetAsName(0);
            if (PdfName.Indexed.Equals(first))
            {
                if (value.Size >= 2)
                {
                    var second = value.GetAsArray(1);
                    if (second != null)
                    {
                        value[1] = simplifyColorspace(second);
                    }
                }
            }
        }

        Additional.Put(PdfName.Colorspace, value);
    }

    /// <summary>
    ///     generates new serial id
    /// </summary>
    protected static long GetSerialId()
    {
        lock (_mutex)
        {
            _serialId = (long)_serialId + 1L;
            return (long)_serialId;
        }
    }

    private static PdfObject simplifyColorspace(PdfArray obj)
    {
        if (obj == null)
        {
            return null;
        }

        PdfObject first = obj.GetAsName(0);
        if (PdfName.Calgray.Equals(first))
        {
            return PdfName.Devicegray;
        }

        if (PdfName.Calrgb.Equals(first))
        {
            return PdfName.Devicergb;
        }

        return obj;
    }
}