using iTextSharp.text.pdf.events;

namespace iTextSharp.text.pdf;

/// <summary>
///     A cell in a PdfPTable.
/// </summary>
public class PdfPCell : Rectangle
{
    /// <summary>
    ///     Holds value of property cellEvent.
    /// </summary>
    private IPdfPCellEvent _cellEvent;

    /// <summary>
    ///     Holds value of property colspan.
    /// </summary>
    private int _colspan = 1;

    /// <summary>
    ///     Holds value of property fixedHeight.
    /// </summary>
    private float _fixedHeight;

    /// <summary>
    ///     Holds value of property image.
    /// </summary>
    private Image _image;

    /// <summary>
    ///     Holds value of property minimumHeight.
    /// </summary>
    private float _minimumHeight;

    /// <summary>
    ///     Holds value of property noWrap.
    /// </summary>
    private bool _noWrap;

    /// <summary>
    ///     Holds value of property paddingBottom.
    /// </summary>
    private float _paddingBottom = 2;

    /// <summary>
    ///     Holds value of property paddingLeft.
    /// </summary>
    private float _paddingLeft = 2;

    /// <summary>
    ///     Holds value of property paddingLeft.
    /// </summary>
    private float _paddingRight = 2;

    /// <summary>
    ///     Holds value of property paddingTop.
    /// </summary>
    private float _paddingTop = 2;

    /// <summary>
    ///     The rotation of the cell. Possible values are
    ///     0, 90, 180 and 270.
    /// </summary>
    private int _rotation;

    /// <summary>
    ///     Holds value of property rowspan.
    ///     @since    2.1.6
    /// </summary>
    private int _rowspan = 1;

    /// <summary>
    ///     Holds value of property table.
    /// </summary>
    private PdfPTable _table;

    /// <summary>
    ///     Increases padding to include border if true
    /// </summary>
    private bool _useBorderPadding;

    /// <summary>
    ///     Holds value of property useDescender.
    /// </summary>
    private bool _useDescender;

    /// <summary>
    ///     Holds value of property verticalAlignment.
    /// </summary>
    private int _verticalAlignment = ALIGN_TOP;

    /// <summary>
    ///     The text in the cell.
    /// </summary>
    protected Phrase phrase;

    /// <summary>
    ///     Constructs an empty  PdfPCell .
    ///     The default padding is 2.
    /// </summary>
    public PdfPCell() : base(0, 0, 0, 0)
    {
        borderWidth = 0.5f;
        border = BOX;
        Column.SetLeading(0, 1);
    }

    /// <summary>
    ///     Constructs a  PdfPCell  with a  Phrase .
    ///     The default padding is 2.
    /// </summary>
    /// <param name="phrase">the text</param>
    public PdfPCell(Phrase phrase) : base(0, 0, 0, 0)
    {
        borderWidth = 0.5f;
        border = BOX;
        Column.AddText(this.phrase = phrase);
        Column.SetLeading(0, 1);
    }

    /// <summary>
    ///     Constructs a  PdfPCell  with an  Image .
    ///     The default padding is 0.
    /// </summary>
    /// <param name="image">the  Image </param>
    public PdfPCell(Image image) : this(image, false)
    {
    }

    /// <summary>
    ///     Constructs a  PdfPCell  with an  Image .
    ///     The default padding is 0.25 for a border width of 0.5.
    /// </summary>
    /// <param name="image">the  Image </param>
    /// <param name="fit"> true  to fit the image to the cell</param>
    public PdfPCell(Image image, bool fit) : base(0, 0, 0, 0)
    {
        borderWidth = 0.5f;
        border = BOX;
        if (fit)
        {
            _image = image;
            Column.SetLeading(0, 1);
            Padding = borderWidth / 2;
        }
        else
        {
            Column.AddText(phrase = new Phrase(new Chunk(image, 0, 0)));
            Column.SetLeading(0, 1);
            Padding = 0;
        }
    }

    /// <summary>
    ///     Constructs a  PdfPCell  with a  PdfPtable .
    ///     This constructor allows nested tables.
    ///     The default padding is 0.
    /// </summary>
    /// <param name="table">The  PdfPTable </param>
    public PdfPCell(PdfPTable table) : this(table, null)
    {
    }

    /// <summary>
    ///     Constructs a  PdfPCell  with a  PdfPtable .
    ///     This constructor allows nested tables.
    ///     @since 2.1.0
    /// </summary>
    /// <param name="table">The  PdfPTable </param>
    /// <param name="style">The style to apply to the cell (you could use getDefaultCell())</param>
    public PdfPCell(PdfPTable table, PdfPCell style) : base(0, 0, 0, 0)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        borderWidth = 0.5f;
        border = BOX;
        Column.SetLeading(0, 1);
        _table = table;
        table.WidthPercentage = 100;
        table.ExtendLastRow = true;
        Column.AddElement(table);
        if (style != null)
        {
            CloneNonPositionParameters(style);
            _verticalAlignment = style._verticalAlignment;
            _paddingLeft = style._paddingLeft;
            _paddingRight = style._paddingRight;
            _paddingTop = style._paddingTop;
            _paddingBottom = style._paddingBottom;
            _colspan = style._colspan;
            _rowspan = style._rowspan;
            _cellEvent = style._cellEvent;
            _useDescender = style._useDescender;
            _useBorderPadding = style._useBorderPadding;
            _rotation = style._rotation;
        }
        else
        {
            Padding = 0;
        }
    }

    /// <summary>
    ///     Constructs a deep copy of a  PdfPCell .
    /// </summary>
    /// <param name="cell">the  PdfPCell  to duplicate</param>
    public PdfPCell(PdfPCell cell) : base(cell?.Llx ?? throw new ArgumentNullException(nameof(cell)),
                                          cell.Lly,
                                          cell.Urx,
                                          cell.Ury)
    {
        CloneNonPositionParameters(cell);
        _verticalAlignment = cell._verticalAlignment;
        _paddingLeft = cell._paddingLeft;
        _paddingRight = cell._paddingRight;
        _paddingTop = cell._paddingTop;
        _paddingBottom = cell._paddingBottom;
        phrase = cell.phrase;
        _fixedHeight = cell._fixedHeight;
        _minimumHeight = cell._minimumHeight;
        _noWrap = cell._noWrap;
        _colspan = cell._colspan;
        _rowspan = cell._rowspan;
        if (cell._table != null)
        {
            _table = new PdfPTable(cell._table);
        }

        _image = Image.GetInstance(cell._image);
        _cellEvent = cell._cellEvent;
        _useDescender = cell._useDescender;
        Column = ColumnText.Duplicate(cell.Column);
        _useBorderPadding = cell._useBorderPadding;
        _rotation = cell._rotation;
    }

    /// <summary>
    ///     Gets the arabic shaping options.
    /// </summary>
    /// <returns>the arabic shaping options</returns>
    public int ArabicOptions
    {
        get => Column.ArabicOptions;
        set => Column.ArabicOptions = value;
    }

    /// <summary>
    ///     Gets the cell event for this cell.
    /// </summary>
    /// <returns>the cell event</returns>
    public IPdfPCellEvent CellEvent
    {
        get => _cellEvent;
        set
        {
            if (value == null)
            {
                _cellEvent = null;
            }
            else if (_cellEvent == null)
            {
                _cellEvent = value;
            }
            else if (_cellEvent is PdfPCellEventForwarder)
            {
                ((PdfPCellEventForwarder)_cellEvent).AddCellEvent(value);
            }
            else
            {
                var forward = new PdfPCellEventForwarder();
                forward.AddCellEvent(_cellEvent);
                forward.AddCellEvent(value);
                _cellEvent = forward;
            }
        }
    }

    /// <summary>
    ///     Getter for property colspan.
    /// </summary>
    /// <returns>Value of property colspan.</returns>
    public int Colspan
    {
        get => _colspan;
        set => _colspan = value;
    }

    /// <summary>
    ///     Gets the ColumnText with the content of the cell.
    /// </summary>
    /// <returns>a columntext object</returns>
    public ColumnText Column { get; set; } = new(null);

    /// <summary>
    ///     Returns the list of composite elements of the column.
    ///     @since    2.1.1
    /// </summary>
    /// <returns>a List object.</returns>
    public IList<IElement> CompositeElements => Column.CompositeElements;

    /// <summary>
    ///     /** Gets the effective bottom padding.  This will include
    ///     the bottom border width if {@link #UseBorderPadding} is true.
    /// </summary>
    /// <returns>effective value of property paddingBottom.</returns>
    public float EffectivePaddingBottom
    {
        get
        {
            if (UseBorderPadding)
            {
                var localBorder = BorderWidthBottom / (UseVariableBorders ? 1f : 2f);
                return _paddingBottom + localBorder;
            }

            return _paddingBottom;
        }
    }

    /// <summary>
    ///     Gets the effective left padding.  This will include
    ///     the left border width if {@link #UseBorderPadding} is true.
    /// </summary>
    /// <returns>effective value of property paddingLeft.</returns>
    public float EffectivePaddingLeft
    {
        get
        {
            if (UseBorderPadding)
            {
                var localBorder = BorderWidthLeft / (UseVariableBorders ? 1f : 2f);
                return _paddingLeft + localBorder;
            }

            return _paddingLeft;
        }
    }

    /// <summary>
    ///     Gets the effective right padding.  This will include
    ///     the right border width if {@link #UseBorderPadding} is true.
    /// </summary>
    /// <returns>effective value of property paddingRight.</returns>
    public float EffectivePaddingRight
    {
        get
        {
            if (UseBorderPadding)
            {
                var localBorder = BorderWidthRight / (UseVariableBorders ? 1f : 2f);
                return _paddingRight + localBorder;
            }

            return _paddingRight;
        }
    }

    /// <summary>
    ///     Gets the effective top padding.  This will include
    ///     the top border width if {@link #isUseBorderPadding()} is true.
    /// </summary>
    /// <returns>effective value of property paddingTop.</returns>
    public float EffectivePaddingTop
    {
        get
        {
            if (UseBorderPadding)
            {
                var localBorder = BorderWidthTop / (UseVariableBorders ? 1f : 2f);
                return _paddingTop + localBorder;
            }

            return _paddingTop;
        }
    }

    /// <summary>
    ///     Gets the extra space between paragraphs.
    /// </summary>
    /// <returns>the extra space between paragraphs</returns>
    public float ExtraParagraphSpace
    {
        get => Column.ExtraParagraphSpace;
        set => Column.ExtraParagraphSpace = value;
    }

    /// <summary>
    ///     Getter for property fixedHeight.
    /// </summary>
    /// <returns>Value of property fixedHeight.</returns>
    public float FixedHeight
    {
        get => _fixedHeight;
        set
        {
            _fixedHeight = value;
            _minimumHeight = 0;
        }
    }

    /// <summary>
    ///     Gets the following paragraph lines indent.
    /// </summary>
    /// <returns>the indent</returns>
    public float FollowingIndent
    {
        get => Column.FollowingIndent;
        set => Column.FollowingIndent = value;
    }

    /// <summary>
    ///     Gets the horizontal alignment for the cell.
    /// </summary>
    /// <returns>the horizontal alignment for the cell</returns>
    public int HorizontalAlignment
    {
        get => Column.Alignment;
        set => Column.Alignment = value;
    }

    /// <summary>
    ///     Getter for property image.
    /// </summary>
    /// <returns>Value of property image.</returns>
    public Image Image
    {
        get => _image;
        set
        {
            Column.SetText(null);
            _table = null;
            _image = value;
        }
    }

    /// <summary>
    ///     Gets the first paragraph line indent.
    /// </summary>
    /// <returns>the indent</returns>
    public float Indent
    {
        get => Column.Indent;
        set => Column.Indent = value;
    }

    /// <summary>
    ///     Gets the fixed leading
    /// </summary>
    /// <returns>the leading</returns>
    public float Leading => Column.Leading;

    /// <summary>
    ///     Getter for property minimumHeight.
    /// </summary>
    /// <returns>Value of property minimumHeight.</returns>
    public float MinimumHeight
    {
        get => _minimumHeight;
        set
        {
            _minimumHeight = value;
            _fixedHeight = 0;
        }
    }

    /// <summary>
    ///     Gets the variable leading
    /// </summary>
    /// <returns>the leading</returns>
    public float MultipliedLeading => Column.MultipliedLeading;

    /// <summary>
    ///     Setter for property noWrap.
    /// </summary>
    public bool NoWrap
    {
        set => _noWrap = value;
        get => _noWrap;
    }

    /// <summary>
    ///     Sets the padding of the contents in the cell (space between content and border).
    /// </summary>
    public float Padding
    {
        set
        {
            _paddingBottom = value;
            _paddingTop = value;
            _paddingLeft = value;
            _paddingRight = value;
        }
    }

    /// <summary>
    ///     Getter for property paddingBottom.
    /// </summary>
    /// <returns>Value of property paddingBottom.</returns>
    public float PaddingBottom
    {
        get => _paddingBottom;
        set => _paddingBottom = value;
    }

    /// <summary>
    /// </summary>
    /// <returns>Value of property paddingLeft.</returns>
    public float PaddingLeft
    {
        get => _paddingLeft;
        set => _paddingLeft = value;
    }

    /// <summary>
    ///     Getter for property paddingRight.
    /// </summary>
    /// <returns>Value of property paddingRight.</returns>
    public float PaddingRight
    {
        get => _paddingRight;
        set => _paddingRight = value;
    }

    /// <summary>
    ///     Getter for property paddingTop.
    /// </summary>
    /// <returns>Value of property paddingTop.</returns>
    public float PaddingTop
    {
        get => _paddingTop;
        set => _paddingTop = value;
    }

    /// <summary>
    ///     Gets the  Phrase  from this cell.
    /// </summary>
    /// <returns>the  Phrase </returns>
    public Phrase Phrase
    {
        get => phrase;
        set
        {
            _table = null;
            _image = null;
            Column.SetText(phrase = value);
        }
    }

    /// <summary>
    ///     Gets the right paragraph lines indent.
    /// </summary>
    /// <returns>the indent</returns>
    public float RightIndent
    {
        get => Column.RightIndent;
        set => Column.RightIndent = value;
    }

    /// <summary>
    ///     Sets the rotation of the cell. Possible values are
    ///     0, 90, 180 and 270.
    /// </summary>
    public new int Rotation
    {
        set
        {
            var rot = value % 360;
            if (rot < 0)
            {
                rot += 360;
            }

            if (rot % 90 != 0)
            {
                throw new ArgumentException("Rotation must be a multiple of 90.");
            }

            _rotation = rot;
        }
        get => _rotation;
    }

    /// <summary>
    ///     Getter for property rowspan.
    /// </summary>
    /// <returns>Value of property rowspan.</returns>
    public int Rowspan
    {
        get => _rowspan;
        set => _rowspan = value;
    }

    /// <summary>
    ///     Gets the run direction of the text content in the cell
    /// </summary>
    /// <returns>
    ///     One of the following values: PdfWriter.RUN_DIRECTION_DEFAULT, PdfWriter.RUN_DIRECTION_NO_BIDI,
    ///     PdfWriter.RUN_DIRECTION_LTR or PdfWriter.RUN_DIRECTION_RTL.
    /// </returns>
    public int RunDirection
    {
        get => Column.RunDirection;
        set => Column.RunDirection = value;
    }

    /// <summary>
    ///     Gets the space/character extra spacing ratio for
    ///     fully justified text.
    /// </summary>
    /// <returns>the space/character extra spacing ratio</returns>
    public float SpaceCharRatio
    {
        get => Column.SpaceCharRatio;
        set => Column.SpaceCharRatio = value;
    }

    /// <summary>
    ///     Getter for property table.
    /// </summary>
    /// <returns>Value of property table.</returns>
    public PdfPTable Table
    {
        get => _table;
        set
        {
            _table = value;
            Column.SetText(null);
            _image = null;
            if (_table != null)
            {
                _table.ExtendLastRow = _verticalAlignment == ALIGN_TOP;
                Column.AddElement(_table);
                _table.WidthPercentage = 100;
            }
        }
    }

    /// <summary>
    ///     Gets state of first line height based on max ascender
    /// </summary>
    /// <returns>true if an ascender is to be used.</returns>
    public bool UseAscender
    {
        get => Column.UseAscender;
        set => Column.UseAscender = value;
    }

    /// <summary>
    ///     Adjusts effective padding to include border widths.
    /// </summary>
    public bool UseBorderPadding
    {
        set => _useBorderPadding = value;
        get => _useBorderPadding;
    }

    /// <summary>
    ///     Getter for property useDescender.
    /// </summary>
    /// <returns>Value of property useDescender.</returns>
    public bool UseDescender
    {
        get => _useDescender;
        set => _useDescender = value;
    }

    /// <summary>
    ///     Gets the vertical alignment for the cell.
    /// </summary>
    /// <returns>the vertical alignment for the cell</returns>
    public int VerticalAlignment
    {
        get => _verticalAlignment;
        set
        {
            _verticalAlignment = value;
            if (_table != null)
            {
                _table.ExtendLastRow = _verticalAlignment == ALIGN_TOP;
            }
        }
    }

    /// <summary>
    ///     Adds an iText element to the cell.
    /// </summary>
    /// <param name="element"></param>
    public void AddElement(IElement element)
    {
        if (_table != null)
        {
            _table = null;
            Column.SetText(null);
        }

        Column.AddElement(element);
    }

    /// <summary>
    ///     Returns the height of the cell.
    ///     @since   3.0.0
    /// </summary>
    /// <returns>the height of the cell</returns>
    public float GetMaxHeight()
    {
        var pivoted = Rotation == 90 || Rotation == 270;
        var img = Image;
        if (img != null)
        {
            img.ScalePercent(100);
            var refWidth = pivoted ? img.ScaledHeight : img.ScaledWidth;
            var scale = (Right - EffectivePaddingRight
                               - EffectivePaddingLeft - Left) / refWidth;
            img.ScalePercent(scale * 100);
            var refHeight = pivoted ? img.ScaledWidth : img.ScaledHeight;
            Bottom = Top - EffectivePaddingTop - EffectivePaddingBottom - refHeight;
        }
        else
        {
            if ((pivoted && HasFixedHeight()) || Column == null)
            {
                Bottom = Top - FixedHeight;
            }
            else
            {
                var ct = ColumnText.Duplicate(Column);
                float right, top, left, bottom;
                if (pivoted)
                {
                    right = PdfPRow.RIGHT_LIMIT;
                    top = Right - EffectivePaddingRight;
                    left = 0;
                    bottom = Left + EffectivePaddingLeft;
                }
                else
                {
                    right = NoWrap ? PdfPRow.RIGHT_LIMIT : Right - EffectivePaddingRight;
                    top = Top - EffectivePaddingTop;
                    left = Left + EffectivePaddingLeft;
                    bottom = HasFixedHeight() ? top + EffectivePaddingBottom - FixedHeight : PdfPRow.BOTTOM_LIMIT;
                }

                PdfPRow.SetColumn(ct, left, bottom, right, top);
                ct.Go(true);
                if (pivoted)
                {
                    Bottom = Top - EffectivePaddingTop - EffectivePaddingBottom - ct.FilledWidth;
                }
                else
                {
                    var yLine = ct.YLine;
                    if (UseDescender)
                    {
                        yLine += ct.Descender;
                    }

                    Bottom = yLine - EffectivePaddingBottom;
                }
            }
        }

        var height = Height;
        if (HasFixedHeight())
        {
            height = FixedHeight;
        }
        else if (height < MinimumHeight)
        {
            height = MinimumHeight;
        }

        return height;
    }

    /// <summary>
    ///     Tells you whether the cell has a fixed height.
    ///     @since 2.1.5
    /// </summary>
    /// <returns>true is a fixed height was set.</returns>
    public bool HasFixedHeight() => FixedHeight > 0;

    /// <summary>
    ///     Tells you whether the cell has a minimum height.
    ///     @since 2.1.5
    /// </summary>
    /// <returns>true if a minimum height was set.</returns>
    public bool HasMinimumHeight() => MinimumHeight > 0;

    /// <summary>
    ///     Sets the leading fixed and variable. The resultant leading will be
    ///     fixedLeading+multipliedLeading*maxFontSize where maxFontSize is the
    ///     size of the bigest font in the line.
    /// </summary>
    /// <param name="fixedLeading">the fixed leading</param>
    /// <param name="multipliedLeading">the variable leading</param>
    public void SetLeading(float fixedLeading, float multipliedLeading)
    {
        Column.SetLeading(fixedLeading, multipliedLeading);
    }

    /// <summary>
    ///     Consumes part of the content of the cell.
    ///     @since   2.1.6
    /// </summary>
    /// <param name="height">the hight of the part that has to be consumed</param>
    internal void ConsumeHeight(float height)
    {
        var rightLimit = Right - EffectivePaddingRight;
        var leftLimit = Left + EffectivePaddingLeft;
        var bry = height - EffectivePaddingTop - EffectivePaddingBottom;
        if (Rotation != 90 && Rotation != 270)
        {
            Column.SetSimpleColumn(leftLimit, bry + 0.001f, rightLimit, 0);
        }
        else
        {
            Column.SetSimpleColumn(0, leftLimit, bry + 0.001f, rightLimit);
        }

        try
        {
            Column.Go(true);
        }
        catch (DocumentException)
        {
            // do nothing
        }
    }
}