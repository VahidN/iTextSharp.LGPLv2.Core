using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfPage  is the PDF Page-object.
///     A Page object is a dictionary whose keys describe a single page containing text,
///     graphics, and images. A Page onjects is a leaf of the Pages tree.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 6.4 (page 73-81)
///     @see     PdfPageElement
///     @see     PdfPages
/// </summary>
public class PdfPage : PdfDictionary
{
    /// <summary>
    ///     value of the <B>Rotate</B> key for a page in INVERTEDPORTRAIT
    /// </summary>
    public static PdfNumber Invertedportrait = new(180);

    /// <summary>
    ///     value of the <B>Rotate</B> key for a page in LANDSCAPE
    /// </summary>
    public static PdfNumber Landscape = new(90);

    /// <summary>
    ///     value of the <B>Rotate</B> key for a page in PORTRAIT
    /// </summary>
    public static PdfNumber Portrait = new(0);

    /// <summary>
    ///     value of the <B>Rotate</B> key for a page in SEASCAPE
    /// </summary>
    public static PdfNumber Seascape = new(270);

    private static readonly PdfName[] _boxNames =
        { PdfName.Cropbox, PdfName.Trimbox, PdfName.Artbox, PdfName.Bleedbox };

    /// <summary>
    ///     membervariables
    /// </summary>
    private static readonly string[] _boxStrings = { "crop", "trim", "art", "bleed" };

    /// <summary>
    ///     value of the <B>MediaBox</B> key
    /// </summary>
    private PdfRectangle _mediaBox;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfPage .
    /// </summary>
    /// <param name="mediaBox">a value for the <B>MediaBox</B> key</param>
    /// <param name="boxSize"></param>
    /// <param name="resources">an indirect reference to a  PdfResources -object</param>
    /// <param name="rotate">a value for the <B>Rotate</B> key</param>
    internal PdfPage(PdfRectangle mediaBox, INullValueDictionary<string, PdfRectangle> boxSize,
                     PdfDictionary resources, int rotate) : base(Page)
    {
        _mediaBox = mediaBox;
        Put(PdfName.Mediabox, mediaBox);
        Put(PdfName.Resources, resources);
        if (rotate != 0)
        {
            Put(PdfName.Rotate, new PdfNumber(rotate));
        }

        for (var k = 0; k < _boxStrings.Length; ++k)
        {
            PdfObject rect = boxSize[_boxStrings[k]];
            if (rect != null)
            {
                Put(_boxNames[k], rect);
            }
        }
    }

    /// <summary>
    ///     Constructs a  PdfPage .
    /// </summary>
    /// <param name="mediaBox">a value for the <B>MediaBox</B> key</param>
    /// <param name="boxSize"></param>
    /// <param name="resources">an indirect reference to a  PdfResources -object</param>
    internal PdfPage(PdfRectangle mediaBox, INullValueDictionary<string, PdfRectangle> boxSize,
                     PdfDictionary resources) : this(mediaBox, boxSize, resources, 0)
    {
    }

    /// <summary>
    ///     Checks if this page element is a tree of pages.
    ///     This method allways returns  false .
    /// </summary>
    /// <returns> false  because this is a single page</returns>

    internal PdfRectangle MediaBox => _mediaBox;

    public static bool IsParent() => false;

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Adds an indirect reference pointing to a  PdfContents -object.
    /// </summary>
    /// <param name="contents">an indirect reference to a  PdfContents -object</param>
    internal void Add(PdfIndirectReference contents)
    {
        Put(PdfName.Contents, contents);
    }

    /// <summary>
    ///     Rotates the mediabox, but not the text in it.
    /// </summary>
    /// <returns>a  PdfRectangle </returns>
    internal PdfRectangle RotateMediaBox()
    {
        _mediaBox = _mediaBox.Rotate;
        Put(PdfName.Mediabox, _mediaBox);
        return _mediaBox;
    }
}