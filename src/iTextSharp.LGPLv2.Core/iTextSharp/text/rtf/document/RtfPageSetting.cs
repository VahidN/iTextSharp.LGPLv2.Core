using System.util;

namespace iTextSharp.text.rtf.document;

/// <summary>
///     The RtfPageSetting stores the page size / page margins for a RtfDocument.
///     INTERNAL CLASS - NOT TO BE USED DIRECTLY
///     @version $Id: RtfPageSetting.cs,v 1.5 2008/05/16 19:30:51 psoares33 Exp $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Thomas Bickel (tmb99@inode.at)
/// </summary>
public class RtfPageSetting : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     Constant for landscape
    /// </summary>
    private static readonly byte[] _landscapeBytes = DocWriter.GetIsoBytes("\\lndscpsxn");

    /// <summary>
    ///     Constant for the bottom margin
    /// </summary>
    private static readonly byte[] _marginBottomBytes = DocWriter.GetIsoBytes("\\margb");

    /// <summary>
    ///     Constant for the left margin
    /// </summary>
    private static readonly byte[] _marginLeftBytes = DocWriter.GetIsoBytes("\\margl");

    /// <summary>
    ///     Constant for the right margin
    /// </summary>
    private static readonly byte[] _marginRightBytes = DocWriter.GetIsoBytes("\\margr");

    /// <summary>
    ///     Constant for the top margin
    /// </summary>
    private static readonly byte[] _marginTopBytes = DocWriter.GetIsoBytes("\\margt");

    /// <summary>
    ///     Constant for the page width
    /// </summary>
    private static readonly byte[] _pageHeightBytes = DocWriter.GetIsoBytes("\\paperh");

    /// <summary>
    ///     Constant for the page height
    /// </summary>
    private static readonly byte[] _pageWidthBytes = DocWriter.GetIsoBytes("\\paperw");

    /// <summary>
    ///     Constant for the section bottom margin
    /// </summary>
    private static readonly byte[] _sectionMarginBottomBytes = DocWriter.GetIsoBytes("\\margbsxn");

    /// <summary>
    ///     Constant for the section left margin
    /// </summary>
    private static readonly byte[] _sectionMarginLeftBytes = DocWriter.GetIsoBytes("\\marglsxn");

    /// <summary>
    ///     Constant for the section right margin
    /// </summary>
    private static readonly byte[] _sectionMarginRightBytes = DocWriter.GetIsoBytes("\\margrsxn");

    /// <summary>
    ///     Constant for the section top margin
    /// </summary>
    private static readonly byte[] _sectionMarginTopBytes = DocWriter.GetIsoBytes("\\margtsxn");

    /// <summary>
    ///     Constant for the section page height
    /// </summary>
    private static readonly byte[] _sectionPageHeightBytes = DocWriter.GetIsoBytes("\\pghsxn");

    /// <summary>
    ///     Constant for the section page width
    /// </summary>
    private static readonly byte[] _sectionPageWidthBytes = DocWriter.GetIsoBytes("\\pgwsxn");

    /// <summary>
    ///     Whether the page is portrait or landscape
    /// </summary>
    private bool _landscape;

    /// <summary>
    ///     The bottom margin to use
    /// </summary>
    private int _marginBottom = 1440;

    /// <summary>
    ///     The left margin to use
    /// </summary>
    private int _marginLeft = 1800;

    /// <summary>
    ///     The right margin to use
    /// </summary>
    private int _marginRight = 1800;

    /// <summary>
    ///     The top margin to use
    /// </summary>
    private int _marginTop = 1440;

    /// <summary>
    ///     The page height to use
    /// </summary>
    private int _pageHeight = 16840;

    /// <summary>
    ///     The page width to use
    /// </summary>
    private int _pageWidth = 11906;

    /// <summary>
    ///     Constructs a new RtfPageSetting object belonging to a RtfDocument.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfPageSetting belongs to</param>
    public RtfPageSetting(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the page size / page margin definition
    /// </summary>
    public virtual void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(_pageWidthBytes, 0, _pageWidthBytes.Length);
        outp.Write(t = IntToByteArray(_pageWidth), 0, t.Length);
        outp.Write(_pageHeightBytes, 0, _pageHeightBytes.Length);
        outp.Write(t = IntToByteArray(_pageHeight), 0, t.Length);
        outp.Write(_marginLeftBytes, 0, _marginLeftBytes.Length);
        outp.Write(t = IntToByteArray(_marginLeft), 0, t.Length);
        outp.Write(_marginRightBytes, 0, _marginRightBytes.Length);
        outp.Write(t = IntToByteArray(_marginRight), 0, t.Length);
        outp.Write(_marginTopBytes, 0, _marginTopBytes.Length);
        outp.Write(t = IntToByteArray(_marginTop), 0, t.Length);
        outp.Write(_marginBottomBytes, 0, _marginBottomBytes.Length);
        outp.Write(t = IntToByteArray(_marginBottom), 0, t.Length);
        Document.OutputDebugLinebreak(outp);
    }

    /// <summary>
    ///     Gets the bottom margin
    /// </summary>
    /// <returns>Returns the bottom margin</returns>
    public int GetMarginBottom() => _marginBottom;

    /// <summary>
    ///     Gets the left margin
    /// </summary>
    /// <returns>Returns the left margin</returns>
    public int GetMarginLeft() => _marginLeft;

    /// <summary>
    ///     Gets the right margin
    /// </summary>
    /// <returns>Returns the right margin</returns>
    public int GetMarginRight() => _marginRight;

    /// <summary>
    ///     Gets the top margin
    /// </summary>
    /// <returns>Returns the top margin</returns>
    public int GetMarginTop() => _marginTop;

    /// <summary>
    ///     Gets the page height
    /// </summary>
    /// <returns>Returns the page height</returns>
    public int GetPageHeight() => _pageHeight;

    /// <summary>
    ///     Gets the page width
    /// </summary>
    /// <returns>Returns the page width</returns>
    public int GetPageWidth() => _pageWidth;

    /// <summary>
    ///     Sets the bottom margin
    /// </summary>
    /// <param name="marginBottom">The bottom margin to use</param>
    public void SetMarginBottom(int marginBottom)
    {
        _marginBottom = marginBottom;
    }

    /// <summary>
    ///     Sets the left margin to use
    /// </summary>
    /// <param name="marginLeft">The left margin to use</param>
    public void SetMarginLeft(int marginLeft)
    {
        _marginLeft = marginLeft;
    }

    /// <summary>
    ///     Sets the right margin to use
    /// </summary>
    /// <param name="marginRight">The right margin to use</param>
    public void SetMarginRight(int marginRight)
    {
        _marginRight = marginRight;
    }

    /// <summary>
    ///     Sets the top margin to use
    /// </summary>
    /// <param name="marginTop">The top margin to use</param>
    public void SetMarginTop(int marginTop)
    {
        _marginTop = marginTop;
    }

    /// <summary>
    ///     Sets the page height to use
    /// </summary>
    /// <param name="pageHeight">The page height to use</param>
    public void SetPageHeight(int pageHeight)
    {
        _pageHeight = pageHeight;
    }

    /// <summary>
    ///     Set the page size to use. This method will use guessFormat to try to guess the correct
    ///     page format. If no format could be guessed, the sizes from the pageSize are used and
    ///     the landscape setting is determined by comparing width and height;
    /// </summary>
    /// <param name="pageSize">The pageSize to use</param>
    public void SetPageSize(Rectangle pageSize)
    {
        if (pageSize == null)
        {
            throw new ArgumentNullException(nameof(pageSize));
        }

        if (!guessFormat(pageSize, false))
        {
            _pageWidth = (int)(pageSize.Width * TWIPS_FACTOR);
            _pageHeight = (int)(pageSize.Height * TWIPS_FACTOR);
            _landscape = _pageWidth > _pageHeight;
        }
    }

    /// <summary>
    ///     Sets the page width to use
    /// </summary>
    /// <param name="pageWidth">The page width to use</param>
    public void SetPageWidth(int pageWidth)
    {
        _pageWidth = pageWidth;
    }

    /// <summary>
    ///     Writes the definition part for a new section
    /// </summary>
    /// <returns>A byte array containing the definition for a new section</returns>
    public void WriteSectionDefinition(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        if (_landscape)
        {
            result.Write(_landscapeBytes, 0, _landscapeBytes.Length);
            result.Write(_sectionPageWidthBytes, 0, _sectionPageWidthBytes.Length);
            result.Write(t = IntToByteArray(_pageWidth), 0, t.Length);
            result.Write(_sectionPageHeightBytes, 0, _sectionPageHeightBytes.Length);
            result.Write(t = IntToByteArray(_pageHeight), 0, t.Length);
            Document.OutputDebugLinebreak(result);
        }
        else
        {
            result.Write(_sectionPageWidthBytes, 0, _sectionPageWidthBytes.Length);
            result.Write(t = IntToByteArray(_pageWidth), 0, t.Length);
            result.Write(_sectionPageHeightBytes, 0, _sectionPageHeightBytes.Length);
            result.Write(t = IntToByteArray(_pageHeight), 0, t.Length);
            Document.OutputDebugLinebreak(result);
        }

        result.Write(_sectionMarginLeftBytes, 0, _sectionMarginLeftBytes.Length);
        result.Write(t = IntToByteArray(_marginLeft), 0, t.Length);
        result.Write(_sectionMarginRightBytes, 0, _sectionMarginRightBytes.Length);
        result.Write(t = IntToByteArray(_marginRight), 0, t.Length);
        result.Write(_sectionMarginTopBytes, 0, _sectionMarginTopBytes.Length);
        result.Write(t = IntToByteArray(_marginTop), 0, t.Length);
        result.Write(_sectionMarginBottomBytes, 0, _sectionMarginBottomBytes.Length);
        result.Write(t = IntToByteArray(_marginBottom), 0, t.Length);
    }

    /// <summary>
    ///     This method compares to Rectangles. They are considered equal if width and height are the same
    /// </summary>
    /// <param name="rect1">The first Rectangle to compare</param>
    /// <param name="rect2">The second Rectangle to compare</param>
    /// <returns> True  if the Rectangles equal,  false  otherwise</returns>
    private static bool rectEquals(Rectangle rect1, Rectangle rect2) =>
        rect1.Width.ApproxEquals(rect2.Width) && rect1.Height.ApproxEquals(rect2.Height);

    /// <summary>
    ///     This method tries to fit the  Rectangle pageSize  to one of the predefined PageSize rectangles.
    ///     If a match is found the pageWidth and pageHeight will be set according to values determined from files
    ///     generated by MS Word2000 and OpenOffice 641. If no match is found the method will try to match the rotated
    ///     Rectangle by calling itself with the parameter rotate set to true.
    /// </summary>
    /// <param name="pageSize">the page size for which to guess the correct format</param>
    /// <param name="rotate">Whether we should try to rotate the size befor guessing the format</param>
    /// <returns> True  if the format was guessed,  false/  otherwise</returns>
    private bool guessFormat(Rectangle pageSize, bool rotate)
    {
        if (rotate)
        {
            pageSize = pageSize.Rotate();
        }

        if (rectEquals(pageSize, PageSize.A3))
        {
            _pageWidth = 16837;
            _pageHeight = 23811;
            _landscape = rotate;
            return true;
        }

        if (rectEquals(pageSize, PageSize.A4))
        {
            _pageWidth = 11907;
            _pageHeight = 16840;
            _landscape = rotate;
            return true;
        }

        if (rectEquals(pageSize, PageSize.A5))
        {
            _pageWidth = 8391;
            _pageHeight = 11907;
            _landscape = rotate;
            return true;
        }

        if (rectEquals(pageSize, PageSize.A6))
        {
            _pageWidth = 5959;
            _pageHeight = 8420;
            _landscape = rotate;
            return true;
        }

        if (rectEquals(pageSize, PageSize.B4))
        {
            _pageWidth = 14570;
            _pageHeight = 20636;
            _landscape = rotate;
            return true;
        }

        if (rectEquals(pageSize, PageSize.B5))
        {
            _pageWidth = 10319;
            _pageHeight = 14572;
            _landscape = rotate;
            return true;
        }

        if (rectEquals(pageSize, PageSize.Halfletter))
        {
            _pageWidth = 7927;
            _pageHeight = 12247;
            _landscape = rotate;
            return true;
        }

        if (rectEquals(pageSize, PageSize.Letter))
        {
            _pageWidth = 12242;
            _pageHeight = 15842;
            _landscape = rotate;
            return true;
        }

        if (rectEquals(pageSize, PageSize.Legal))
        {
            _pageWidth = 12252;
            _pageHeight = 20163;
            _landscape = rotate;
            return true;
        }

        if (!rotate && guessFormat(pageSize, true))
        {
            var x = _pageWidth;
            _pageWidth = _pageHeight;
            _pageHeight = x;
            return true;
        }

        return false;
    }
}