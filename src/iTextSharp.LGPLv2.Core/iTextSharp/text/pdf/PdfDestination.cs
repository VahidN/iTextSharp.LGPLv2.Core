namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfColor  defines a Color (it's a  PdfArray  containing 3 values).
///     @see        PdfDictionary
/// </summary>
public class PdfDestination : PdfArray
{
    /// <summary>
    ///     public static member-variables
    /// </summary>
    /// <summary>
    ///     This is a possible destination type
    /// </summary>
    public const int FIT = 1;

    /// <summary>
    ///     This is a possible destination type
    /// </summary>
    public const int FITB = 5;

    /// <summary>
    ///     This is a possible destination type
    /// </summary>
    public const int FITBH = 6;

    /// <summary>
    ///     This is a possible destination type
    /// </summary>
    public const int FITBV = 7;

    /// <summary>
    ///     This is a possible destination type
    /// </summary>
    public const int FITH = 2;

    /// <summary>
    ///     This is a possible destination type
    /// </summary>
    public const int FITR = 4;

    /// <summary>
    ///     This is a possible destination type
    /// </summary>
    public const int FITV = 3;

    /// <summary>
    ///     This is a possible destination type
    /// </summary>
    public const int XYZ = 0;

    /// <summary>
    ///     member variables
    /// </summary>
    /// <summary>
    ///     Is the indirect reference to a page already added?
    /// </summary>
    private bool _status;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a new  PdfDestination .
    ///     If <VAR>type</VAR> equals <VAR>FITB</VAR>, the bounding box of a page
    ///     will fit the window of the Reader. Otherwise the type will be set to
    ///     <VAR>FIT</VAR> so that the entire page will fit to the window.
    /// </summary>
    /// <param name="type">The destination type</param>
    public PdfDestination(int type)
    {
        if (type == FITB)
        {
            Add(PdfName.Fitb);
        }
        else
        {
            Add(PdfName.Fit);
        }
    }

    /// <summary>
    ///     Constructs a new  PdfDestination .
    ///     If <VAR>type</VAR> equals <VAR>FITBH</VAR> / <VAR>FITBV</VAR>,
    ///     the width / height of the bounding box of a page will fit the window
    ///     of the Reader. The parameter will specify the y / x coordinate of the
    ///     top / left edge of the window. If the <VAR>type</VAR> equals <VAR>FITH</VAR>
    ///     or <VAR>FITV</VAR> the width / height of the entire page will fit
    ///     the window and the parameter will specify the y / x coordinate of the
    ///     top / left edge. In all other cases the type will be set to <VAR>FITH</VAR>.
    /// </summary>
    /// <param name="type">the destination type</param>
    /// <param name="parameter">a parameter to combined with the destination type</param>
    public PdfDestination(int type, float parameter) : base(new PdfNumber(parameter))
    {
        switch (type)
        {
            default:
                AddFirst(PdfName.Fith);
                break;
            case FITV:
                AddFirst(PdfName.Fitv);
                break;
            case FITBH:
                AddFirst(PdfName.Fitbh);
                break;
            case FITBV:
                AddFirst(PdfName.Fitbv);
                break;
        }
    }

    /// <summary>
    ///     Constructs a new  PdfDestination .
    ///     Display the page, with the coordinates (left, top) positioned
    ///     at the top-left corner of the window and the contents of the page magnified
    ///     by the factor zoom. A negative value for any of the parameters left or top, or a
    ///     zoom value of 0 specifies that the current value of that parameter is to be retained unchanged.
    /// </summary>
    /// <param name="type">must be a <VAR>PdfDestination.XYZ</VAR></param>
    /// <param name="left">the left value. Negative to place a null</param>
    /// <param name="top">the top value. Negative to place a null</param>
    /// <param name="zoom">The zoom factor. A value of 0 keeps the current value</param>
    public PdfDestination(int type, float left, float top, float zoom) : base(PdfName.Xyz)
    {
        if (left < 0)
        {
            Add(PdfNull.Pdfnull);
        }
        else
        {
            Add(new PdfNumber(left));
        }

        if (top < 0)
        {
            Add(PdfNull.Pdfnull);
        }
        else
        {
            Add(new PdfNumber(top));
        }

        Add(new PdfNumber(zoom));
    }

    /// <summary>
    ///     Constructs a new  PdfDestination .
    ///     Display the page, with its contents magnified just enough
    ///     to fit the rectangle specified by the coordinates left, bottom, right, and top
    ///     entirely within the window both horizontally and vertically. If the required
    ///     horizontal and vertical magnification factors are different, use the smaller of
    ///     the two, centering the rectangle within the window in the other dimension.
    ///     @since iText0.38
    /// </summary>
    /// <param name="type">must be PdfDestination.FITR</param>
    /// <param name="left">a parameter</param>
    /// <param name="bottom">a parameter</param>
    /// <param name="right">a parameter</param>
    /// <param name="top">a parameter</param>
    public PdfDestination(int type, float left, float bottom, float right, float top) : base(PdfName.Fitr)
    {
        Add(new PdfNumber(left));
        Add(new PdfNumber(bottom));
        Add(new PdfNumber(right));
        Add(new PdfNumber(top));
    }

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Checks if an indirect reference to a page has been added.
    /// </summary>
    /// <returns> true  or  false </returns>
    public bool AddPage(PdfIndirectReference page)
    {
        if (!_status)
        {
            AddFirst(page);
            _status = true;
            return true;
        }

        return false;
    }

    public bool HasPage() => _status;
}