using iTextSharp.LGPLv2.Core.System.NetUtils;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec.wmf;

namespace iTextSharp.text;

/// <summary>
///     An ImgWMF is the representation of a windows metafile
///     that has to be inserted into the document
///     @see        Element
///     @see        Image
///     @see        Gif
///     @see        Png
/// </summary>
/// <summary>
///     An ImgWMF is the representation of a windows metafile
///     that has to be inserted into the document
/// </summary>
public class ImgWmf : Image
{
    /// <summary>
    ///     Constructors
    /// </summary>
    /// <summary>
    ///     Constructs an ImgWMF-object
    /// </summary>
    /// <param name="image">a Image</param>
    public ImgWmf(Image image) : base(image)
    {
    }

    /// <summary>
    ///     Constructs an ImgWMF-object, using an url.
    /// </summary>
    /// <param name="url">the URL where the image can be found</param>
    public ImgWmf(Uri url) : base(url)
    {
        processParameters();
    }

    /// <summary>
    ///     Constructs an ImgWMF-object, using a filename.
    /// </summary>
    /// <param name="filename">a string-representation of the file that contains the image.</param>
    public ImgWmf(string filename) : this(Utilities.ToUrl(filename))
    {
    }

    /// <summary>
    ///     Constructs an ImgWMF-object from memory.
    /// </summary>
    /// <param name="img">the memory image</param>
    public ImgWmf(byte[] img) : base((Uri)null)
    {
        rawData = img;
        originalData = img;
        processParameters();
    }

    /// <summary>
    ///     Reads the WMF into a template.
    /// </summary>
    /// <param name="template">the template to read to</param>
    public void ReadWmf(PdfTemplate template)
    {
        TemplateData = template ?? throw new ArgumentNullException(nameof(template));
        template.Width = Width;
        template.Height = Height;
        Stream istr = null;
        try
        {
            if (rawData == null)
            {
                istr = url.GetResponseStream();
            }
            else
            {
                istr = new MemoryStream(rawData);
            }

            var meta = new MetaDo(istr, template);
            meta.ReadAll();
        }
        finally
        {
            if (istr != null)
            {
                istr.Dispose();
            }
        }
    }

    /// <summary>
    ///     This method checks if the image is a valid WMF and processes some parameters.
    /// </summary>
    private void processParameters()
    {
        type = IMGTEMPLATE;
        originalType = ORIGINAL_WMF;
        Stream istr = null;
        try
        {
            string errorId;
            if (rawData == null)
            {
                istr = url.GetResponseStream();
                errorId = url.ToString();
            }
            else
            {
                istr = new MemoryStream(rawData);
                errorId = "Byte array";
            }

            var im = new InputMeta(istr);
            if (im.ReadInt() != unchecked((int)0x9AC6CDD7))
            {
                throw new BadElementException(errorId + " is not a valid placeable windows metafile.");
            }

            im.ReadWord();
            var left = im.ReadShort();
            var top = im.ReadShort();
            var right = im.ReadShort();
            var bottom = im.ReadShort();
            var inch = im.ReadWord();
            dpiX = 72;
            dpiY = 72;
            scaledHeight = (float)(bottom - top) / inch * 72f;
            Top = scaledHeight;
            scaledWidth = (float)(right - left) / inch * 72f;
            Right = scaledWidth;
        }
        finally
        {
            if (istr != null)
            {
                istr.Dispose();
            }

            plainWidth = Width;
            plainHeight = Height;
        }
    }
}