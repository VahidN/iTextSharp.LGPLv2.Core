using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     Summary description for SimpleCell.
/// </summary>
public class SimpleCell : Rectangle, IPdfPCellEvent, ITextElementArray
{
    /// <summary>
    ///     the CellAttributes object represents a cell.
    /// </summary>
    public new const bool CELL = false;

    /// <summary>
    ///     the CellAttributes object represents a row.
    /// </summary>
    public new const bool ROW = true;

    /// <summary>
    ///     the content of the Cell.
    /// </summary>
    private readonly List<IElement> _content = new();

    /// <summary>
    ///     indicates if these are the attributes of a single Cell (false) or a group of Cells (true).
    /// </summary>
    private bool _cellgroup;

    /// <summary>
    ///     the colspan of a Cell
    /// </summary>
    private int _colspan = 1;

    /// <summary>
    ///     horizontal alignment inside the Cell.
    /// </summary>
    private int _horizontalAlignment = ALIGN_UNDEFINED;

    /// <summary>
    ///     an extra padding variable
    /// </summary>
    private float _paddingBottom = float.NaN;

    /// <summary>
    ///     the width of the Cell.
    /// </summary>
    private float _width;

    /// <summary>
    ///     the widthpercentage of the Cell.
    /// </summary>
    private float _widthpercentage;

    /// <summary>
    ///     Indicates that the largest ascender height should be used to determine the
    ///     height of the first line.  Note that this only has an effect when rendered
    ///     to PDF.  Setting this to true can help with vertical alignment problems.
    /// </summary>
    protected bool useAscender;

    /// <summary>
    ///     Adjusts the cell contents to compensate for border widths.  Note that
    ///     this only has an effect when rendered to PDF.
    /// </summary>
    protected bool useBorderPadding;

    /// <summary>
    ///     Indicates that the largest descender height should be added to the height of
    ///     the last line (so characters like y don't dip into the border).   Note that
    ///     this only has an effect when rendered to PDF.
    /// </summary>
    protected bool useDescender;

    /// <summary>
    ///     A CellAttributes object is always constructed without any dimensions.
    ///     Dimensions are defined after creation.
    /// </summary>
    /// <param name="row">only true if the CellAttributes object represents a row.</param>
    public SimpleCell(bool row) : base(0f, 0f, 0f, 0f)
    {
        _cellgroup = row;
        Border = BOX;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the cellgroup.</returns>
    public bool Cellgroup
    {
        get => _cellgroup;
        set => _cellgroup = value;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the colspan.</returns>
    public int Colspan
    {
        get => _colspan;
        set
        {
            if (value > 0)
            {
                _colspan = value;
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the horizontal alignment.</returns>
    public int HorizontalAlignment
    {
        get => _horizontalAlignment;
        set => _horizontalAlignment = value;
    }

    /// <summary>
    ///     Sets the padding parameters if they are undefined.
    /// </summary>
    public float Padding
    {
        set
        {
            if (float.IsNaN(PaddingRight))
            {
                PaddingRight = value;
            }

            if (float.IsNaN(PaddingLeft))
            {
                PaddingLeft = value;
            }

            if (float.IsNaN(_paddingBottom))
            {
                PaddingBottom = value;
            }

            if (float.IsNaN(PaddingTop))
            {
                PaddingTop = value;
            }
        }
    }

    /// <summary>
    /// </summary>
    public float PaddingBottom
    {
        get => _paddingBottom;
        set => _paddingBottom = value;
    }

    /// <summary>
    ///     an extra padding variable
    /// </summary>
    public float PaddingLeft { get; set; } = float.NaN;

    /// <summary>
    ///     an extra padding variable
    /// </summary>
    public float PaddingRight { get; set; } = float.NaN;

    /// <summary>
    ///     an extra padding variable
    /// </summary>
    public float PaddingTop { get; set; } = float.NaN;

    /// <summary>
    /// </summary>
    /// <returns>Returns the spacing.</returns>
    public float Spacing
    {
        set
        {
            SpacingLeft = value;
            SpacingRight = value;
            SpacingTop = value;
            SpacingBottom = value;
        }
    }

    /// <summary>
    ///     an extra spacing variable
    /// </summary>
    public float SpacingBottom { get; set; } = float.NaN;

    /// <summary>
    ///     an extra spacing variable
    /// </summary>
    public float SpacingLeft { get; set; } = float.NaN;

    /// <summary>
    ///     an extra spacing variable
    /// </summary>
    public float SpacingRight { get; set; } = float.NaN;

    /// <summary>
    ///     an extra spacing variable
    /// </summary>
    public float SpacingTop { get; set; } = float.NaN;

    /// <summary>
    /// </summary>
    /// <returns>Returns the useAscender.</returns>
    public bool UseAscender
    {
        get => useAscender;
        set => useAscender = value;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the useBorderPadding.</returns>
    public bool UseBorderPadding
    {
        get => useBorderPadding;
        set => useBorderPadding = value;
    }

    public bool UseDescender
    {
        get => useDescender;
        set => useDescender = value;
    }

    /// <summary>
    ///     vertical alignment inside the Cell.
    /// </summary>
    public int VerticalAlignment { get; set; } = ALIGN_UNDEFINED;

    /// <summary>
    /// </summary>
    /// <returns>Returns the width.</returns>
    public new float Width
    {
        get => _width;
        set => _width = value;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the widthpercentage.</returns>
    public float Widthpercentage
    {
        get => _widthpercentage;
        set => _widthpercentage = value;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the content.</returns>
    internal IList<IElement> Content => _content;

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfPCellEvent#cellLayout(com.lowagie.text.pdf.PdfPCell, com.lowagie.text.Rectangle,
    ///     com.lowagie.text.pdf.PdfContentByte[])
    /// </summary>
    public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
    {
        if (position == null)
        {
            throw new ArgumentNullException(nameof(position));
        }

        if (canvases == null)
        {
            throw new ArgumentNullException(nameof(canvases));
        }

        var spLeft = SpacingLeft;
        if (float.IsNaN(spLeft))
        {
            spLeft = 0f;
        }

        var spRight = SpacingRight;
        if (float.IsNaN(spRight))
        {
            spRight = 0f;
        }

        var spTop = SpacingTop;
        if (float.IsNaN(spTop))
        {
            spTop = 0f;
        }

        var spBottom = SpacingBottom;
        if (float.IsNaN(spBottom))
        {
            spBottom = 0f;
        }

        var rect = new Rectangle(position.GetLeft(spLeft), position.GetBottom(spBottom), position.GetRight(spRight),
                                 position.GetTop(spTop));
        rect.CloneNonPositionParameters(this);
        canvases[PdfPTable.BACKGROUNDCANVAS].Rectangle(rect);
        rect.BackgroundColor = null;
        canvases[PdfPTable.LINECANVAS].Rectangle(rect);
    }

    public override int Type => Element.CELL;

    /// <summary>
    ///     @see com.lowagie.text.TextElementArray#add(java.lang.Object)
    /// </summary>
    public bool Add(IElement o)
    {
        try
        {
            AddElement(o);
            return true;
        }
        catch (InvalidCastException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Adds content to this object.
    ///     @throws BadElementException
    /// </summary>
    /// <param name="element"></param>
    public void AddElement(IElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (_cellgroup)
        {
            if (element is SimpleCell)
            {
                if (((SimpleCell)element).Cellgroup)
                {
                    throw new BadElementException("You can't add one row to another row.");
                }

                _content.Add(element);
                return;
            }

            throw new BadElementException("You can only add cells to rows, no objects of type " + element.GetType());
        }

        if (element.Type == PARAGRAPH
            || element.Type == PHRASE
            || element.Type == ANCHOR
            || element.Type == CHUNK
            || element.Type == LIST
            || element.Type == MARKED
            || element.Type == JPEG
            || element.Type == JPEG2000
            || element.Type == JBIG2
            || element.Type == IMGRAW
            || element.Type == IMGTEMPLATE)
        {
            _content.Add(element);
        }
        else
        {
            throw new BadElementException("You can't add an element of type " + element.GetType() +
                                          " to a SimpleCell.");
        }
    }

    /// <summary>
    ///     Creates a Cell with these attributes.
    ///     @throws BadElementException
    /// </summary>
    /// <param name="rowAttributes"></param>
    /// <returns>a cell based on these attributes.</returns>
    public Cell CreateCell(SimpleCell rowAttributes)
    {
        var cell = new Cell();
        cell.CloneNonPositionParameters(rowAttributes);
        cell.SoftCloneNonPositionParameters(this);
        cell.Colspan = _colspan;
        cell.HorizontalAlignment = _horizontalAlignment;
        cell.VerticalAlignment = VerticalAlignment;
        cell.UseAscender = useAscender;
        cell.UseBorderPadding = useBorderPadding;
        cell.UseDescender = useDescender;
        foreach (var element in _content)
        {
            cell.AddElement(element);
        }

        return cell;
    }

    /// <summary>
    ///     Creates a PdfPCell with these attributes.
    /// </summary>
    /// <param name="rowAttributes"></param>
    /// <returns>a PdfPCell based on these attributes.</returns>
    public PdfPCell CreatePdfPCell(SimpleCell rowAttributes)
    {
        if (rowAttributes == null)
        {
            throw new ArgumentNullException(nameof(rowAttributes));
        }

        var cell = new PdfPCell();
        cell.Border = NO_BORDER;
        var tmp = new SimpleCell(CELL);
        tmp.SpacingLeft = SpacingLeft;
        tmp.SpacingRight = SpacingRight;
        tmp.SpacingTop = SpacingTop;
        tmp.SpacingBottom = SpacingBottom;
        tmp.CloneNonPositionParameters(rowAttributes);
        tmp.SoftCloneNonPositionParameters(this);
        cell.CellEvent = tmp;
        cell.HorizontalAlignment = rowAttributes._horizontalAlignment;
        cell.VerticalAlignment = rowAttributes.VerticalAlignment;
        cell.UseAscender = rowAttributes.useAscender;
        cell.UseBorderPadding = rowAttributes.useBorderPadding;
        cell.UseDescender = rowAttributes.useDescender;
        cell.Colspan = _colspan;
        if (_horizontalAlignment != ALIGN_UNDEFINED)
        {
            cell.HorizontalAlignment = _horizontalAlignment;
        }

        if (VerticalAlignment != ALIGN_UNDEFINED)
        {
            cell.VerticalAlignment = VerticalAlignment;
        }

        if (useAscender)
        {
            cell.UseAscender = useAscender;
        }

        if (useBorderPadding)
        {
            cell.UseBorderPadding = useBorderPadding;
        }

        if (useDescender)
        {
            cell.UseDescender = useDescender;
        }

        float p;
        var spLeft = SpacingLeft;
        if (float.IsNaN(spLeft))
        {
            spLeft = 0f;
        }

        var spRight = SpacingRight;
        if (float.IsNaN(spRight))
        {
            spRight = 0f;
        }

        var spTop = SpacingTop;
        if (float.IsNaN(spTop))
        {
            spTop = 0f;
        }

        var spBottom = SpacingBottom;
        if (float.IsNaN(spBottom))
        {
            spBottom = 0f;
        }

        p = PaddingLeft;
        if (float.IsNaN(p))
        {
            p = 0f;
        }

        cell.PaddingLeft = p + spLeft;
        p = PaddingRight;
        if (float.IsNaN(p))
        {
            p = 0f;
        }

        cell.PaddingRight = p + spRight;
        p = PaddingTop;
        if (float.IsNaN(p))
        {
            p = 0f;
        }

        cell.PaddingTop = p + spTop;
        p = _paddingBottom;
        if (float.IsNaN(p))
        {
            p = 0f;
        }

        cell.PaddingBottom = p + spBottom;
        foreach (var element in _content)
        {
            cell.AddElement(element);
        }

        return cell;
    }
}