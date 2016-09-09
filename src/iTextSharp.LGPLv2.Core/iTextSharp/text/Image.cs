using System;
using System.IO;
using iTextSharp.LGPLv2.Core.System.NetUtils;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec;

namespace iTextSharp.text
{
    /// <summary>
    /// An Image is the representation of a graphic element (JPEG, PNG or GIF)
    /// that has to be inserted into the document
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Rectangle"/>
    public abstract class Image : Rectangle
    {

        /// <summary>
        /// static membervariables (concerning the presence of borders)
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
        /// type of image
        /// </summary>
        public const int ORIGINAL_BMP = 4;

        /// <summary>
        /// type of image
        /// </summary>
        public const int ORIGINAL_GIF = 3;

        /// <summary>
        /// type of image
        /// @since	2.1.5
        /// </summary>
        public const int ORIGINAL_JBIG2 = 9;

        /// <summary>
        /// type of image
        /// </summary>
        public const int ORIGINAL_JPEG = 1;

        /// <summary>
        /// type of image
        /// </summary>
        public const int ORIGINAL_JPEG2000 = 8;

        /// <summary>
        /// type of image
        /// </summary>
        public const int ORIGINAL_NONE = 0;

        /// <summary>
        /// type of image
        /// </summary>
        public const int ORIGINAL_PNG = 2;

        /// <summary>
        /// type of image
        /// </summary>
        public const int ORIGINAL_TIFF = 5;

        /// <summary>
        /// type of image
        /// </summary>
        public const int ORIGINAL_WMF = 6;

        /// <summary> this is a kind of image Element. </summary>
        public const int RIGHT_ALIGN = 2;
        /// <summary> this is a kind of image Element. </summary>
        public const int TEXTWRAP = 4;

        /// <summary> this is a kind of image Element. </summary>
        public const int UNDERLYING = 8;
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
        /// The compression level of the content streams.
        /// @since   2.1.3
        /// </summary>
        protected int compressionLevel = PdfStream.DEFAULT_COMPRESSION;

        /// <summary>
        /// Holds value of property deflated.
        /// </summary>
        protected bool deflated;

        /// <summary> Holds value of property dpiX. </summary>
        protected int dpiX;

        /// <summary> Holds value of property dpiY. </summary>
        protected int dpiY;

        protected Image imageMask;

        /// <summary>
        /// for the moment these variables are only used for Images in class Table
        /// </summary>
        /// <summary>
        /// code contributed by Pelikan Stephan
        /// </summary>
        /// <summary>
        /// the indentation to the left.
        /// </summary>
        protected float indentationLeft;

        /// <summary>
        /// the indentation to the right.
        /// </summary>
        protected float indentationRight;

        /// <summary> Holds value of property interpolation. </summary>
        protected bool interpolation;

        /// <summary>
        /// Image color inversion
        /// </summary>
        protected bool Invert;

        protected IPdfOcg layer;

        protected bool Mask;

        protected long mySerialId = GetSerialId();

        /// <summary>
        /// Holds value of property originalData.
        /// </summary>
        protected byte[] originalData;

        /// <summary>
        /// Holds value of property originalType.
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
        /// The spacing after the image.
        /// </summary>
        protected float spacingAfter;

        /// <summary>
        /// The spacing before the image.
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
        /// serial stamping
        /// </summary>
        static object _serialId = 0L;
        /// <summary>
        /// Holds value of property initialRotation.
        /// </summary>
        private float _initialRotation;

        /// <summary>
        /// Holds value of property smask.
        /// </summary>
        private bool _smask;

        /// <summary>
        /// Holds value of property widthPercentage.
        /// </summary>
        private float _widthPercentage = 100;

        /// <summary>
        /// Holds value of property XYRatio.
        /// </summary>
        private float _xyRatio;
        /// <summary>
        /// constructors
        /// </summary>

        /// <summary>
        /// Constructs an Image-object, using an url.
        /// </summary>
        /// <param name="url">the URL where the image can be found.</param>
        protected Image(Uri url) : base(0, 0)
        {
            this.url = url;
            alignment = DEFAULT;
            RotationRadians = 0;
        }

        /// <summary>
        /// Constructs an Image object duplicate.
        /// </summary>
        /// <param name="image">another Image object.</param>
        protected Image(Image image) : base(image)
        {
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
            _smask = image._smask;
            XyRatio = image.XyRatio;
            originalData = image.originalData;
            originalType = image.originalType;
            spacingAfter = image.spacingAfter;
            spacingBefore = image.spacingBefore;
            _widthPercentage = image._widthPercentage;
            layer = image.layer;
            _initialRotation = image._initialRotation;
            DirectReference = image.DirectReference;
        }

        /// <summary>
        /// Returns the absolute X position.
        /// </summary>
        /// <value>a position</value>
        public float AbsoluteX
        {
            get
            {
                return absoluteX;
            }
        }

        /// <summary>
        /// Returns the absolute Y position.
        /// </summary>
        /// <value>a position</value>
        public float AbsoluteY
        {
            get
            {
                return absoluteY;
            }
        }

        public PdfDictionary Additional { get; set; }

        /// <summary>
        /// Get/set the alignment for the image.
        /// </summary>
        /// <value>a value</value>
        public int Alignment
        {
            get
            {
                return alignment;
            }

            set
            {
                alignment = value;
            }
        }

        /// <summary>
        /// Get/set the alternative text for the image.
        /// </summary>
        /// <value>a string</value>
        public string Alt
        {
            get
            {
                return alt;
            }

            set
            {
                alt = value;
            }
        }

        /// <summary>
        /// Get/set the annotation.
        /// </summary>
        /// <value>the Annotation</value>
        public Annotation Annotation
        {
            get
            {
                return annotation;
            }

            set
            {
                annotation = value;
            }
        }

        /// <summary>
        /// Gets the bpc for the image.
        /// </summary>
        /// <remarks>
        /// this only makes sense for Images of the type RawImage.
        /// </remarks>
        /// <value>a bpc value</value>
        public int Bpc
        {
            get
            {
                return bpc;
            }
        }

        /// <summary>
        /// Gets the colorspace for the image.
        /// </summary>
        /// <remarks>
        /// this only makes sense for Images of the type Jpeg.
        /// </remarks>
        /// <value>a colorspace value</value>
        public int Colorspace
        {
            get
            {
                return colorspace;
            }
        }

        /// <summary>
        /// Sets the compression level to be used if the image is written as a compressed stream.
        /// @since   2.1.3
        /// </summary>
        public int CompressionLevel
        {
            set
            {
                if (value < PdfStream.NO_COMPRESSION || value > PdfStream.BEST_COMPRESSION)
                    compressionLevel = PdfStream.DEFAULT_COMPRESSION;
                else
                    compressionLevel = value;
            }
            get
            {
                return compressionLevel;
            }
        }

        public bool Deflated
        {
            get
            {
                return deflated;
            }
            set
            {
                deflated = value;
            }
        }

        public PdfIndirectReference DirectReference { set; get; }

        /// <summary>
        /// Gets the dots-per-inch in the X direction. Returns 0 if not available.
        /// </summary>
        /// <value>the dots-per-inch in the X direction</value>
        public int DpiX
        {
            get
            {
                return dpiX;
            }
        }

        /// <summary>
        /// Gets the dots-per-inch in the Y direction. Returns 0 if not available.
        /// </summary>
        /// <value>the dots-per-inch in the Y direction</value>
        public int DpiY
        {
            get
            {
                return dpiY;
            }
        }

        /// <summary>
        /// Get/set the explicit masking.
        /// </summary>
        /// <value>the explicit masking</value>
        public Image ImageMask
        {
            get
            {
                return imageMask;
            }

            set
            {
                if (Mask)
                    throw new DocumentException("An image mask cannot contain another image mask.");
                if (!value.Mask)
                    throw new DocumentException("The image mask is not a mask. Did you do MakeMask()?");
                imageMask = value;
                _smask = (value.bpc > 1 && value.bpc <= 8);
            }
        }

        public float IndentationLeft
        {
            get
            {
                return indentationLeft;
            }
            set
            {
                indentationLeft = value;
            }
        }

        public float IndentationRight
        {
            get
            {
                return indentationRight;
            }
            set
            {
                indentationRight = value;
            }
        }

        /// <summary>
        /// Some image formats, like TIFF may present the images rotated that have
        /// to be compensated.
        /// </summary>
        public float InitialRotation
        {
            get
            {
                return _initialRotation;
            }
            set
            {
                float oldRot = RotationRadians - _initialRotation;
                _initialRotation = value;
                Rotation = oldRot;
            }
        }

        /// <summary>
        /// Sets the image interpolation. Image interpolation attempts to
        /// produce a smooth transition between adjacent sample values.
        /// </summary>
        /// <value>New value of property interpolation.</value>
        public bool Interpolation
        {
            set
            {
                interpolation = value;
            }
            get
            {
                return interpolation;
            }
        }

        /// <summary>
        /// Inverts the meaning of the bits of a mask.
        /// </summary>
        /// <value>true to invert the meaning of the bits of a mask</value>
        public bool Inverted
        {
            set
            {
                Invert = value;
            }
            get
            {
                return Invert;
            }
        }

        public IPdfOcg Layer
        {
            get
            {
                return layer;
            }
            set
            {
                layer = value;
            }
        }

        /// <summary>
        /// Returns the transformation matrix of the image.
        /// </summary>
        /// <value>an array [AX, AY, BX, BY, CX, CY, DX, DY]</value>
        public float[] Matrix
        {
            get
            {
                float[] matrix = new float[8];
                float cosX = (float)Math.Cos(RotationRadians);
                float sinX = (float)Math.Sin(RotationRadians);
                matrix[AX] = plainWidth * cosX;
                matrix[AY] = plainWidth * sinX;
                matrix[BX] = (-plainHeight) * sinX;
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
        /// returns serial id for this object
        /// </summary>
        public long MySerialId
        {
            get
            {
                return mySerialId;
            }
        }

        public byte[] OriginalData
        {
            get
            {
                return originalData;
            }
            set
            {
                originalData = value;
            }
        }

        public int OriginalType
        {
            get
            {
                return originalType;
            }
            set
            {
                originalType = value;
            }
        }

        /// <summary>
        /// Gets the plain height of the image.
        /// </summary>
        /// <value>a value</value>
        public float PlainHeight
        {
            get
            {
                return plainHeight;
            }
        }

        /// <summary>
        /// Gets the plain width of the image.
        /// </summary>
        /// <value>a value</value>
        public float PlainWidth
        {
            get
            {
                return plainWidth;
            }
        }

        /// <summary>
        /// methods to retrieve information
        /// </summary>
        /// <summary>
        /// Gets the raw data for the image.
        /// </summary>
        /// <remarks>
        /// this only makes sense for Images of the type RawImage.
        /// </remarks>
        /// <value>the raw data</value>
        public byte[] RawData
        {
            get
            {
                return rawData;
            }
        }

        /// <summary>
        /// Sets the rotation of the image in radians.
        /// </summary>
        public new float Rotation
        {
            set
            {
                double d = Math.PI;                  //__IDS__
                RotationRadians = (float)((value + _initialRotation) % (2.0 * d)); //__IDS__
                if (RotationRadians < 0)
                {
                    RotationRadians += (float)(2.0 * d);           //__IDS__
                }
                float[] matrix = Matrix;
                scaledWidth = matrix[DX] - matrix[CX];
                scaledHeight = matrix[DY] - matrix[CY];
            }
        }

        /// <summary>
        /// Sets the rotation of the image in degrees.
        /// </summary>
        public float RotationDegrees
        {
            set
            {
                Rotation = (value / 180 * (float)Math.PI); //__IDS__
            }
        }

        /// <summary>
        /// Gets the scaled height of the image.
        /// </summary>
        /// <value>a value</value>
        public float ScaledHeight
        {
            get
            {
                return scaledHeight;
            }
        }

        /// <summary>
        /// Gets the scaled width of the image.
        /// </summary>
        /// <value>a value</value>
        public float ScaledWidth
        {
            get
            {
                return scaledWidth;
            }
        }

        public bool Smask
        {
            get
            {
                return _smask;
            }
            set
            {
                _smask = value;
            }
        }

        public float SpacingAfter
        {
            get
            {
                return spacingAfter;
            }
            set
            {
                spacingAfter = value;
            }
        }

        public float SpacingBefore
        {
            get
            {
                return spacingBefore;
            }
            set
            {
                spacingBefore = value;
            }
        }

        /// <summary>
        /// Tags this image with an ICC profile.
        /// </summary>
        public IccProfile TagIcc
        {
            get
            {
                return Profile;
            }
            set
            {
                Profile = value;
            }
        }

        /// <summary>
        /// Get/set the template to be used as an image.
        /// </summary>
        /// <remarks>
        /// this only makes sense for Images of the type ImgTemplate.
        /// </remarks>
        /// <value>the template</value>
        public PdfTemplate TemplateData
        {
            get
            {
                return Template[0];
            }

            set
            {
                Template[0] = value;
            }
        }

        /// <summary>
        /// Returns the transparency.
        /// </summary>
        /// <value>the transparency</value>
        public int[] Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                transparency = value;
            }
        }

        /// <summary>
        /// Returns the type.
        /// </summary>
        /// <value>a type</value>
        public override int Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Gets the string-representation of the reference to the image.
        /// </summary>
        /// <value>a string</value>
        public Uri Url
        {
            get
            {
                return url;
            }
            set
            {
                url = value;
            }
        }

        public float WidthPercentage
        {
            get
            {
                return _widthPercentage;
            }
            set
            {
                _widthPercentage = value;
            }
        }

        public float XyRatio
        {
            get
            {
                return _xyRatio;
            }
            set
            {
                _xyRatio = value;
            }
        }

        /// <summary>
        /// Gets an instance of an Image.
        /// </summary>
        /// <param name="image">an Image</param>
        /// <returns>an object of type Gif, Jpeg or Png</returns>
        public static Image GetInstance(Image image)
        {
            return image; //todo: Clone the image
        }

        /// <summary>
        /// Gets an instance of an Image.
        /// </summary>
        /// <param name="url">an URL</param>
        /// <returns>an object of type Gif, Jpeg or Png</returns>
        public static Image GetInstance(Uri url)
        {
            // Add support for base64 encoded images.
            if (url.Scheme == "data")
            {
                var src = url.AbsoluteUri;
                if (src.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
                {
                    // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
                    var base64Data = src.Substring(src.IndexOf(",", StringComparison.OrdinalIgnoreCase) + 1);
                    var imagedata = Convert.FromBase64String(base64Data);
                    return GetInstance(imagedata);
                }
            }

            Stream istr = null;
            try
            {
                istr = url.GetResponseStream();
                int c1 = istr.ReadByte();
                int c2 = istr.ReadByte();
                int c3 = istr.ReadByte();
                int c4 = istr.ReadByte();
                // jbig2
                int c5 = istr.ReadByte();
                int c6 = istr.ReadByte();
                int c7 = istr.ReadByte();
                int c8 = istr.ReadByte();
                istr.Dispose();

                istr = null;
                if (c1 == 'G' && c2 == 'I' && c3 == 'F')
                {
                    GifImage gif = new GifImage(url);
                    Image img = gif.GetImage(1);
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
                    Image img = PngImage.GetImage(url);
                    return img;
                }
                if (c1 == 0xD7 && c2 == 0xCD)
                {
                    Image img = new ImgWmf(url);
                    return img;
                }
                if (c1 == 'B' && c2 == 'M')
                {
                    Image img = BmpImage.GetImage(url);
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
                            string file = url.LocalPath;
                            ra = new RandomAccessFileOrArray(file);
                        }
                        else
                            ra = new RandomAccessFileOrArray(url);
                        Image img = TiffImage.GetTiffImage(ra, 1);
                        img.url = url;
                        return img;
                    }
                    finally
                    {
                        if (ra != null)
                            ra.Close();
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
                            string file = url.LocalPath;
                            ra = new RandomAccessFileOrArray(file);
                        }
                        else
                            ra = new RandomAccessFileOrArray(url);
                        Image img = Jbig2Image.GetJbig2Image(ra, 1);
                        img.url = url;
                        return img;
                    }
                    finally
                    {
                        if (ra != null)
                            ra.Close();
                    }
                }
                throw new IOException(url
                        + " is not a recognized imageformat.");
            }
            finally
            {
                if (istr != null)
                {
                    istr.Dispose();
                }
            }
        }

        public static Image GetInstance(Stream s)
        {
            byte[] a = RandomAccessFileOrArray.InputStreamToArray(s);
            s.Dispose();
            return GetInstance(a);
        }

        /// <summary>
        /// Creates a JBIG2 Image.
        /// @since   2.1.5
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
        /// Gets an instance of an Image.
        /// </summary>
        /// <param name="imgb">a byte array</param>
        /// <returns>an object of type Gif, Jpeg or Png</returns>
        public static Image GetInstance(byte[] imgb)
        {
            int c1 = imgb[0];
            int c2 = imgb[1];
            int c3 = imgb[2];
            int c4 = imgb[3];

            if (c1 == 'G' && c2 == 'I' && c3 == 'F')
            {
                GifImage gif = new GifImage(imgb);
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
                    Image img = TiffImage.GetTiffImage(ra, 1);
                    if (img.OriginalData == null)
                        img.OriginalData = imgb;

                    return img;
                }
                finally
                {
                    if (ra != null)
                        ra.Close();
                }

            }
            throw new IOException("The byte array is not a recognized imageformat.");
        }
        /// <summary>
        /// Converts a .NET image to a Native(PNG, JPG, GIF, WMF) image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Image GetInstance(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, format);
            return GetInstance(ms.ToArray());
        }

        /// <summary>
        /// Gets an instance of an Image from a System.Drwaing.Image.
        /// </summary>
        /// <param name="image">the System.Drawing.Image to convert</param>
        /// <param name="color">
        /// if different from null the transparency
        /// pixels are replaced by this color
        /// </param>
        /// <param name="forceBw">if true the image is treated as black and white</param>
        /// <returns>an object of type ImgRaw</returns>
        public static Image GetInstance(System.Drawing.Image image, BaseColor color, bool forceBw)
        {
            System.Drawing.Bitmap bm = (System.Drawing.Bitmap)image;
            int w = bm.Width;
            int h = bm.Height;
            int pxv = 0;
            if (forceBw)
            {
                int byteWidth = (w / 8) + ((w & 7) != 0 ? 1 : 0);
                byte[] pixelsByte = new byte[byteWidth * h];

                int index = 0;
                int size = h * w;
                int transColor = 1;
                if (color != null)
                {
                    transColor = (color.R + color.G + color.B < 384) ? 0 : 1;
                }
                int[] transparency = null;
                int cbyte = 0x80;
                int wMarker = 0;
                int currByte = 0;
                if (color != null)
                {
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            int alpha = bm.GetPixel(i, j).A;
                            if (alpha < 250)
                            {
                                if (transColor == 1)
                                    currByte |= cbyte;
                            }
                            else
                            {
                                if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
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
                                wMarker = 0;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            if (transparency == null)
                            {
                                int alpha = bm.GetPixel(i, j).A;
                                if (alpha == 0)
                                {
                                    transparency = new int[2];
                                    transparency[0] = transparency[1] = ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0) ? 1 : 0;
                                }
                            }
                            if ((bm.GetPixel(i, j).ToArgb() & 0x888) != 0)
                                currByte |= cbyte;
                            cbyte >>= 1;
                            if (cbyte == 0 || wMarker + 1 >= w)
                            {
                                pixelsByte[index++] = (byte)currByte;
                                cbyte = 0x80;
                                currByte = 0;
                            }
                            ++wMarker;
                            if (wMarker >= w)
                                wMarker = 0;
                        }
                    }
                }
                return GetInstance(w, h, 1, 1, pixelsByte, transparency);
            }
            else
            {
                byte[] pixelsByte = new byte[w * h * 3];
                byte[] smask = null;

                int index = 0;
                int size = h * w;
                int red = 255;
                int green = 255;
                int blue = 255;
                if (color != null)
                {
                    red = color.R;
                    green = color.G;
                    blue = color.B;
                }
                int[] transparency = null;
                if (color != null)
                {
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            int alpha = (bm.GetPixel(i, j).ToArgb() >> 24) & 0xff;
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
                                pixelsByte[index++] = (byte)((pxv) & 0xff);
                            }
                        }
                    }
                }
                else
                {
                    int transparentPixel = 0;
                    smask = new byte[w * h];
                    bool shades = false;
                    int smaskPtr = 0;
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            pxv = bm.GetPixel(i, j).ToArgb();
                            byte alpha = smask[smaskPtr++] = (byte)((pxv >> 24) & 0xff);
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
                        transparency = null;
                    else
                        smask = null;
                }
                Image img = GetInstance(w, h, 3, 8, pixelsByte, transparency);
                if (smask != null)
                {
                    Image sm = GetInstance(w, h, 1, 8, smask);
                    sm.MakeMask();
                    img.ImageMask = sm;
                }
                return img;
            }
        }

        /// <summary>
        /// Gets an instance of an Image from a System.Drawing.Image.
        /// </summary>
        /// <param name="image">the System.Drawing.Image to convert</param>
        /// <param name="color">
        /// if different from null the transparency
        /// pixels are replaced by this color
        /// </param>
        /// <returns>an object of type ImgRaw</returns>
        public static Image GetInstance(System.Drawing.Image image, BaseColor color)
        {
            return GetInstance(image, color, false);
        }

        /// <summary>
        /// Gets an instance of an Image.
        /// </summary>
        /// <param name="filename">a filename</param>
        /// <returns>an object of type Gif, Jpeg or Png</returns>
        public static Image GetInstance(string filename)
        {
            return GetInstance(Utilities.ToUrl(filename));
        }

        /// <summary>
        /// Gets an instance of an Image in raw mode.
        /// </summary>
        /// <param name="width">the width of the image in pixels</param>
        /// <param name="height">the height of the image in pixels</param>
        /// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
        /// <param name="bpc">bits per component</param>
        /// <param name="data">the image data</param>
        /// <returns>an object of type ImgRaw</returns>
        public static Image GetInstance(int width, int height, int components, int bpc, byte[] data)
        {
            return GetInstance(width, height, components, bpc, data, null);
        }

        /// <summary>
        /// Reuses an existing image.
        /// @throws BadElementException on error
        /// </summary>
        /// <param name="iref">the reference to the image dictionary</param>
        /// <returns>the image</returns>
        public static Image GetInstance(PrIndirectReference iref)
        {
            PdfDictionary dic = (PdfDictionary)PdfReader.GetPdfObjectRelease(iref);
            int width = ((PdfNumber)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Width))).IntValue;
            int height = ((PdfNumber)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Height))).IntValue;
            Image imask = null;
            PdfObject obj = dic.Get(PdfName.Smask);
            if (obj != null && obj.IsIndirect())
            {
                imask = GetInstance((PrIndirectReference)obj);
            }
            else
            {
                obj = dic.Get(PdfName.Mask);
                if (obj != null && obj.IsIndirect())
                {
                    PdfObject obj2 = PdfReader.GetPdfObjectRelease(obj);
                    if (obj2 is PdfDictionary)
                        imask = GetInstance((PrIndirectReference)obj);
                }
            }
            Image img = new ImgRaw(width, height, 1, 1, null);
            img.imageMask = imask;
            img.DirectReference = iref;
            return img;
        }

        /// <summary>
        /// Gets an instance of an Image in raw mode.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static Image GetInstance(PdfTemplate template)
        {
            return new ImgTemplate(template);
        }

        /// <summary>
        /// Gets an instance of an Image in raw mode.
        /// </summary>
        /// <param name="width">the width of the image in pixels</param>
        /// <param name="height">the height of the image in pixels</param>
        /// <param name="reverseBits"></param>
        /// <param name="typeCcitt"></param>
        /// <param name="parameters"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Image GetInstance(int width, int height, bool reverseBits, int typeCcitt, int parameters, byte[] data)
        {
            return GetInstance(width, height, reverseBits, typeCcitt, parameters, data, null);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="reverseBits"></param>
        /// <param name="typeCcitt"></param>
        /// <param name="parameters"></param>
        /// <param name="data"></param>
        /// <param name="transparency"></param>
        /// <returns></returns>
        public static Image GetInstance(int width, int height, bool reverseBits, int typeCcitt, int parameters, byte[] data, int[] transparency)
        {
            if (transparency != null && transparency.Length != 2)
                throw new BadElementException("Transparency length must be equal to 2 with CCITT images");
            Image img = new ImgCcitt(width, height, reverseBits, typeCcitt, parameters, data);
            img.transparency = transparency;
            return img;
        }

        /// <summary>
        /// Gets an instance of an Image in raw mode.
        /// </summary>
        /// <param name="width">the width of the image in pixels</param>
        /// <param name="height">the height of the image in pixels</param>
        /// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
        /// <param name="bpc">bits per component</param>
        /// <param name="data">the image data</param>
        /// <param name="transparency">
        /// transparency information in the Mask format of the
        /// image dictionary
        /// </param>
        /// <returns>an object of type ImgRaw</returns>
        public static Image GetInstance(int width, int height, int components, int bpc, byte[] data, int[] transparency)
        {
            if (transparency != null && transparency.Length != components * 2)
                throw new BadElementException("Transparency length must be equal to (componentes * 2)");
            if (components == 1 && bpc == 1)
            {
                byte[] g4 = Ccittg4Encoder.Compress(data, width, height);
                return GetInstance(width, height, false, CCITTG4, CCITT_BLACKIS1, g4, transparency);
            }
            Image img = new ImgRaw(width, height, components, bpc, data);
            img.transparency = transparency;
            return img;
        }

        /// <summary>
        /// methods to set information
        /// </summary>

        /// <summary>
        /// Gets the current image rotation in radians.
        /// </summary>
        /// <returns>the current image rotation in radians</returns>
        public float GetImageRotation()
        {
            float rot = (float)((RotationRadians - _initialRotation) % (2.0 * Math.PI));
            if (rot < 0)
            {
                rot += (float)(2.0 * Math.PI);
            }
            return rot;
        }

        /// <summary>
        /// Checks if the Images has to be added at an absolute position.
        /// </summary>
        /// <returns>a bool</returns>
        public bool HasAbsolutePosition()
        {
            return !float.IsNaN(absoluteY);
        }

        /// <summary>
        /// Checks if the Images has to be added at an absolute X position.
        /// </summary>
        /// <returns>a bool</returns>
        public bool HasAbsoluteX()
        {
            return !float.IsNaN(absoluteX);
        }

        /// <summary>
        /// Checks is the image has an ICC profile.
        /// </summary>
        /// <returns>the ICC profile or null</returns>
        public bool HasIccProfile()
        {
            return (Profile != null);
        }

        /// <summary>
        /// Returns true if the image is a ImgRaw-object.
        /// </summary>
        /// <returns>a bool</returns>
        public bool IsImgRaw()
        {
            return type == IMGRAW;
        }

        /// <summary>
        /// Returns true if the image is an ImgTemplate-object.
        /// </summary>
        /// <returns>a bool</returns>
        public bool IsImgTemplate()
        {
            return type == IMGTEMPLATE;
        }

        /// <summary>
        /// Returns true if the image is a Jpeg-object.
        /// </summary>
        /// <returns>a bool</returns>
        public bool IsJpeg()
        {
            return type == JPEG;
        }

        /// <summary>
        /// Returns true if this Image is a mask.
        /// </summary>
        /// <returns>true if this Image is a mask</returns>
        public bool IsMask()
        {
            return Mask;
        }

        /// <summary>
        /// Returns true if this Image has the
        /// requisites to be a mask.
        /// </summary>
        /// <returns>true if this Image can be a mask</returns>
        public bool IsMaskCandidate()
        {
            if (type == IMGRAW)
            {
                if (bpc > 0xff)
                    return true;
            }
            return colorspace == 1;
        }

        /// <summary>
        /// @see com.lowagie.text.Element#isNestable()
        /// @since   iText 2.0.8
        /// </summary>
        public override bool IsNestable()
        {
            return true;
        }

        /// <summary>
        /// Make this Image a mask.
        /// </summary>
        public void MakeMask()
        {
            if (!IsMaskCandidate())
                throw new DocumentException("This image can not be an image mask.");
            Mask = true;
        }

        /// <summary>
        /// Scale the image to an absolute width and an absolute height.
        /// </summary>
        /// <param name="newWidth">the new width</param>
        /// <param name="newHeight">the new height</param>
        public void ScaleAbsolute(float newWidth, float newHeight)
        {
            plainWidth = newWidth;
            plainHeight = newHeight;
            float[] matrix = Matrix;
            scaledWidth = matrix[DX] - matrix[CX];
            scaledHeight = matrix[DY] - matrix[CY];
            WidthPercentage = 0;
        }

        /// <summary>
        /// Scale the image to an absolute height.
        /// </summary>
        /// <param name="newHeight">the new height</param>
        public void ScaleAbsoluteHeight(float newHeight)
        {
            plainHeight = newHeight;
            float[] matrix = Matrix;
            scaledWidth = matrix[DX] - matrix[CX];
            scaledHeight = matrix[DY] - matrix[CY];
            WidthPercentage = 0;
        }

        /// <summary>
        /// Scale the image to an absolute width.
        /// </summary>
        /// <param name="newWidth">the new width</param>
        public void ScaleAbsoluteWidth(float newWidth)
        {
            plainWidth = newWidth;
            float[] matrix = Matrix;
            scaledWidth = matrix[DX] - matrix[CX];
            scaledHeight = matrix[DY] - matrix[CY];
            WidthPercentage = 0;
        }

        /// <summary>
        /// Scale the image to a certain percentage.
        /// </summary>
        /// <param name="percent">the scaling percentage</param>
        public void ScalePercent(float percent)
        {
            ScalePercent(percent, percent);
        }

        /// <summary>
        /// Scale the width and height of an image to a certain percentage.
        /// </summary>
        /// <param name="percentX">the scaling percentage of the width</param>
        /// <param name="percentY">the scaling percentage of the height</param>
        public void ScalePercent(float percentX, float percentY)
        {
            plainWidth = (Width * percentX) / 100f;
            plainHeight = (Height * percentY) / 100f;
            float[] matrix = Matrix;
            scaledWidth = matrix[DX] - matrix[CX];
            scaledHeight = matrix[DY] - matrix[CY];
            WidthPercentage = 0;
        }

        /// <summary>
        /// Scales the image so that it fits a certain width and height.
        /// </summary>
        /// <param name="fitWidth">the width to fit</param>
        /// <param name="fitHeight">the height to fit</param>
        public void ScaleToFit(float fitWidth, float fitHeight)
        {
            ScalePercent(100);
            float percentX = (fitWidth * 100) / ScaledWidth;
            float percentY = (fitHeight * 100) / ScaledHeight;
            ScalePercent(percentX < percentY ? percentX : percentY);
            WidthPercentage = 0;
        }

        /// <summary>
        /// Sets the absolute position of the Image.
        /// </summary>
        /// <param name="absoluteX"></param>
        /// <param name="absoluteY"></param>
        public void SetAbsolutePosition(float absoluteX, float absoluteY)
        {
            this.absoluteX = absoluteX;
            this.absoluteY = absoluteY;
        }
        /// <summary>
        /// Sets the dots per inch value
        /// dpi for x coordinates
        /// dpi for y coordinates
        /// </summary>
        /// <param name="dpiX"></param>
        /// <param name="dpiY"></param>
        public void SetDpi(int dpiX, int dpiY)
        {
            this.dpiX = dpiX;
            this.dpiY = dpiY;
        }

        /// <summary>
        /// Replaces CalRGB and CalGray colorspaces with DeviceRGB and DeviceGray.
        /// </summary>
        public void SimplifyColorspace()
        {
            if (Additional == null)
                return;
            PdfArray value = Additional.GetAsArray(PdfName.Colorspace);
            if (value == null)
                return;
            PdfObject cs = simplifyColorspace(value);
            PdfObject newValue;
            if (cs.IsName())
                newValue = cs;
            else
            {
                newValue = value;
                PdfName first = value.GetAsName(0);
                if (PdfName.Indexed.Equals(first))
                {
                    if (value.Size >= 2)
                    {
                        PdfArray second = value.GetAsArray(1);
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
        /// generates new serial id
        /// </summary>
        protected static long GetSerialId()
        {
            lock (_serialId)
            {
                _serialId = (long)_serialId + 1L;
                return (long)_serialId;
            }
        }
        private PdfObject simplifyColorspace(PdfArray obj)
        {
            if (obj == null)
                return null;
            PdfObject first = obj.GetAsName(0);
            if (PdfName.Calgray.Equals(first))
                return PdfName.Devicegray;
            else if (PdfName.Calrgb.Equals(first))
                return PdfName.Devicergb;
            else
                return obj;
        }
    }
}
