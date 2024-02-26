using System.Drawing;
using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     A Table is a Rectangle that contains Cells,
///     ordered in some kind of matrix.
/// </summary>
/// <remarks>
///     Tables that span multiple pages are cut into different parts automatically.
///     If you want a table header to be repeated on every page, you may not forget to
///     mark the end of the header section by using the method EndHeaders().
///     The matrix of a table is not necessarily an m x n-matrix. It can contain holes
///     or cells that are bigger than the unit. Believe me or not, but it took some serious
///     thinking to make this as userfriendly as possible. I hope you wil find the result
///     quite simple (I love simple solutions, especially for complex problems).
/// </remarks>
/// <example>
///     // Remark: You MUST know the number of columns when constructing a Table.
///     //         The number of rows is not important.
///     Table table = new Table(3);
///     table.SetBorderWidth(1);
///     table.SetBorderColor(new Color(0, 0, 255));
///     table.SetPadding(5);
///     table.SetSpacing(5);
///     Cell cell = new Cell("header");
///     cell.SetHeader(true);
///     cell.SetColspan(3);
///     table.AddCell(cell);
///     table.EndHeaders();
///     cell = new Cell("example cell with colspan 1 and rowspan 2");
///     cell.SetRowspan(2);
///     cell.SetBorderColor(new Color(255, 0, 0));
///     table.AddCell(cell);
///     table.AddCell("1.1");
///     table.AddCell("2.1");
///     table.AddCell("1.2");
///     table.AddCell("2.2");
///     table.AddCell("cell test1");
///     cell = new Cell("big cell");
///     cell.SetRowspan(2);
///     cell.SetColspan(2);
///     table.AddCell(cell);
///     table.AddCell("cell test2");
/// </example>
public class Table : Rectangle, ILargeElement
{
    ///<summary> This is the horizontal Element. </summary>
    private int _alignment = ALIGN_CENTER;

    ///<summary> This is cellpadding. </summary>
    private float _cellpadding;

    ///<summary> If true cells may not be split over two pages. </summary>
    private bool _cellsFitPage;

    ///<summary> This is cellspacing. </summary>
    private float _cellspacing;

    ///<summary> This is the number of columns in the Table. </summary>
    private int _columns;

    /// <summary>
    ///     this is the current Position in the table
    /// </summary>
    private Point _curPosition = new(0, 0);

    ///<summary> This Empty Cell contains the DEFAULT layout of each Cell added with the method AddCell(string content). </summary>
    private Cell _defaultCell = new(true);

    /// <summary>
    ///     these variables contain the layout of the table
    /// </summary>
    /// <summary> This is the number of the last row of the table headers. </summary>
    private int _lastHeaderRow = -1;

    ///<summary> bool to track if a table was inserted (to avoid unnecessary computations afterwards) </summary>
    private bool _mTableInserted;

    ///<summary> This is the offset of the table. </summary>
    private float _offset = float.NaN;

    ///<summary> This is the list of Rows. </summary>
    private List<Row> _rows = new();

    ///<summary> If true this table may not be split over two pages. </summary>
    private bool _tableFitsPage;

    ///<summary> This is the width of the table (in percent of the available space). </summary>
    private float _width = 80;

    ///<summary> This is an array containing the widths (in percentages) of every column. </summary>
    private float[] _widths;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     these variables contain the data of the table
    /// </summary>
    /// <summary>
    ///     Boolean to automatically fill empty cells before a table is rendered
    ///     (takes CPU so may be set to false in case of certainty)
    /// </summary>
    protected internal bool autoFillEmptyCells;

    /// <summary>
    ///     Indicates if the PdfPTable is complete once added to the document.
    ///     @since   iText 2.0.8
    /// </summary>
    protected bool complete = true;

    /// <summary>
    ///     Indicates if this is the first time the section was added.
    ///     @since   iText 2.0.8
    /// </summary>
    protected bool notAddedYet = true;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a Table with a certain number of columns.
    /// </summary>
    /// <param name="columns">The number of columns in the table</param>
    /// <overloads>
    ///     Has three overloads
    /// </overloads>
    public Table(int columns) : this(columns, 1)
    {
    }

    /// <summary>
    ///     Constructs a Table with a certain number of columns
    ///     and a certain number of Rows.
    /// </summary>
    /// <param name="columns">The number of columns in the table</param>
    /// <param name="rows">The number of rows</param>
    /// <overloads>
    ///     Has three overloads
    /// </overloads>
    public Table(int columns, int rows) : base(0, 0, 0, 0)
    {
        Border = BOX;
        BorderWidth = 1;
        _defaultCell.Border = BOX;

        // a table should have at least 1 column
        if (columns <= 0)
        {
            throw new BadElementException("A table should have at least 1 column.");
        }

        _columns = columns;

        // a certain number of rows are created
        for (var i = 0; i < rows; i++)
        {
            _rows.Add(new Row(columns));
        }

        _curPosition = new Point(0, 0);

        // the DEFAULT widths are calculated
        _widths = new float[columns];
        var width = 100f / columns;

        for (var i = 0; i < columns; i++)
        {
            _widths[i] = width;
        }
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Get/set the horizontal Element.
    /// </summary>
    /// <value>a value</value>
    public int Alignment
    {
        get => _alignment;

        set => _alignment = value;
    }

    /// <summary>
    ///     Enables/disables automatic insertion of empty cells before table is rendered. (default = false)
    /// </summary>
    /// <remarks>
    ///     As some people may want to create a table, fill only a couple of the cells and don't bother with
    ///     investigating which empty ones need to be added, this default behaviour may be very welcome.
    ///     Disabling is recommended to increase speed. (empty cells should be added through extra code then)
    /// </remarks>
    /// <value>enable/disable autofill</value>
    public bool AutoFillEmptyCells
    {
        set => autoFillEmptyCells = value;
    }

    public override float Bottom
    {
        get
        {
            errorDimensions();

            return 0;
        }
        set => errorDimensions();
    }

    /// <summary>
    ///     Get/set the cellpadding.
    /// </summary>
    /// <value>the cellpadding</value>
    public float Cellpadding
    {
        get => _cellpadding;

        set => _cellpadding = value;
    }

    /// <summary>
    ///     Allows you to control when a page break occurs.
    /// </summary>
    /// <remarks>
    ///     When a cell doesn't fit a page, it is split in two parts.
    ///     If you want to avoid this, you should set the <VAR>cellsFitPage</VAR> value to true.
    /// </remarks>
    /// <value>a value</value>
    public bool CellsFitPage
    {
        set => _cellsFitPage = value;
        get => _cellsFitPage;
    }

    /// <summary>
    ///     Get/set the cellspacing.
    /// </summary>
    /// <value>the cellspacing</value>
    public float Cellspacing
    {
        get => _cellspacing;

        set => _cellspacing = value;
    }

    /// <summary>
    ///     Gets the number of columns.
    /// </summary>
    /// <value>a value</value>
    public int Columns => _columns;

    /// <summary>
    ///     If set to true, iText will try to convert the Table to a PdfPTable.
    /// </summary>
    public bool Convert2Pdfptable { set; get; }

    /// <summary>
    ///     Sets the default layout of the Table to
    ///     the provided Cell
    /// </summary>
    /// <param name="value">a cell with all the defaults</param>
    public Cell DefaultCell
    {
        set => _defaultCell = value;
        get => _defaultCell;
    }

    /// <summary>
    ///     Changes the backgroundcolor in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    /// <value>the new color</value>
    public BaseColor DefaultCellBackgroundColor
    {
        set => _defaultCell.BackgroundColor = value;
    }

    /// <summary>
    ///     Changes the border in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    /// <value>the new border value</value>
    public int DefaultCellBorder
    {
        set => _defaultCell.Border = value;
    }

    /// <summary>
    ///     Changes the bordercolor in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    public BaseColor DefaultCellBorderColor
    {
        set => _defaultCell.BorderColor = value;
    }

    /// <summary>
    ///     Changes the width of the borders in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    /// <value>the new width</value>
    public float DefaultCellBorderWidth
    {
        set => _defaultCell.BorderWidth = value;
    }

    /// <summary>
    ///     Changes the grayfill in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    /// <value>the new value</value>
    public float DefaultCellGrayFill
    {
        set
        {
            if (value >= 0 && value <= 1)
            {
                _defaultCell.GrayFill = value;
            }
        }
    }

    /// <summary>
    ///     Changes the colspan in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    /// <value>the new colspan value</value>
    public int DefaultColspan
    {
        set => _defaultCell.Colspan = value;
    }

    /// <summary>
    ///     Changes the horizontalalignment in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    /// <value>the new alignment value</value>
    public int DefaultHorizontalAlignment
    {
        set => _defaultCell.HorizontalAlignment = value;
    }

    /// <summary>
    ///     Sets the default layout of the Table to
    ///     the provided Cell
    /// </summary>
    /// <param name="value">a cell with all the defaults</param>
    public Cell DefaultLayout
    {
        set => _defaultCell = value;
        get => _defaultCell;
    }

    /// <summary>
    ///     Changes the rowspan in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    /// <value>the new rowspan value</value>
    public int DefaultRowspan
    {
        set => _defaultCell.Rowspan = value;
    }

    /// <summary>
    ///     Changes the verticalAlignment in the default layout of the Cells
    ///     added with method AddCell(string content).
    /// </summary>
    /// <value>the new alignment value</value>
    public int DefaultVerticalAlignment
    {
        set => _defaultCell.VerticalAlignment = value;
    }

    /// <summary>
    ///     Gets the dimension of this table
    /// </summary>
    /// <value>the dimension</value>
    public Dimension Dimension => new(_columns, _rows.Count);

    /// <summary>
    ///     Sets the horizontal Element.
    /// </summary>
    /// <value>the new value</value>
    public int LastHeaderRow
    {
        set => _lastHeaderRow = value;
        get => _lastHeaderRow;
    }

    public override float Left
    {
        get
        {
            errorDimensions();

            return 0;
        }
        set => errorDimensions();
    }

    /// <summary>
    ///     Is the width a percentage (false) or an absolute width (true)?
    /// </summary>
    public bool Locked { get; set; }

    /// <summary>
    ///     Returns the next column 0-based index where a new cell would be added.
    ///     (contributed by dperezcar@fcc.es)
    /// </summary>
    /// <returns>y coordinate for the next row</returns>
    public int NextColumn => _curPosition.Y;

    /// <summary>
    ///     Returns the next row 0-based index where a new cell would be added.
    ///     (contributed by dperezcar@fcc.es)
    /// </summary>
    /// <returns>x coordinate for the next row</returns>
    public int NextRow => _curPosition.X;

    /// <summary>
    ///     Indicates if this is the first time the section is added.
    ///     @since   iText2.0.8
    /// </summary>
    /// <returns>true if the section wasn't added yet</returns>
    public bool NotAddedYet
    {
        get => notAddedYet;
        set => notAddedYet = value;
    }

    /// <summary>
    ///     Get/set the offset of this table.
    /// </summary>
    /// <value>the space between this table and the previous element.</value>
    public float Offset
    {
        get => _offset;

        set => _offset = value;
    }

    /// <summary>
    ///     Sets the cellpadding.
    /// </summary>
    /// <value>the new value</value>
    public float Padding
    {
        set => _cellpadding = value;
    }

    /// <summary>
    ///     Gets the proportional widths of the columns in this Table.
    /// </summary>
    /// <value>the proportional widths of the columns in this Table</value>
    public float[] ProportionalWidths => _widths;

    public override float Right
    {
        get
        {
            errorDimensions();

            return 0;
        }
        set => errorDimensions();
    }

    /// <summary>
    ///     methods to retrieve the membervariables
    /// </summary>
    /// <summary>
    ///     Gets the number of rows in this Table.
    /// </summary>
    /// <value>the number of rows in this Table</value>
    public int Size => _rows.Count;

    /// <summary>
    ///     Sets the cellspacing.
    /// </summary>
    /// <value>the new value</value>
    public float Spacing
    {
        set => _cellspacing = value;
    }

    /// <summary>
    ///     Allows you to control when a page break occurs.
    /// </summary>
    /// <remarks>
    ///     When a table doesn't fit a page, it is split in two parts.
    ///     If you want to avoid this, you should set the <VAR>tableFitsPage</VAR> value to true.
    /// </remarks>
    /// <value>a value</value>
    public bool TableFitsPage
    {
        set
        {
            _tableFitsPage = value;

            if (value)
            {
                CellsFitPage = true;
            }
        }
        get => _tableFitsPage;
    }

    public override float Top
    {
        get
        {
            errorDimensions();

            return 0;
        }
        set => errorDimensions();
    }

    /// <summary>
    ///     Get/set the table width (a percentage).
    /// </summary>
    /// <value>the table width (a percentage)</value>
    public override float Width
    {
        get => _width;
        set => _width = value;
    }

    /// <summary>
    ///     Sets the widths of the different columns (percentages).
    /// </summary>
    /// <remarks>
    ///     You can give up relative values of borderwidths.
    ///     The sum of these values will be considered 100%.
    ///     The values will be recalculated as percentages of this sum.
    /// </remarks>
    /// <example>
    ///     float[] widths = {2, 1, 1};
    ///     table.SetWidths(widths)
    ///     The widths will be: a width of 50% for the first column,
    ///     25% for the second and third column.
    /// </example>
    /// <value>an array with values</value>
    public float[] Widths
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length != _columns)
            {
                throw new BadElementException("Wrong number of columns.");
            }

            // The sum of all values is 100%
            float hundredPercent = 0;

            for (var i = 0; i < _columns; i++)
            {
                hundredPercent += value[i];
            }

            // The different percentages are calculated
            float width;
            _widths[_columns - 1] = 100;

            for (var i = 0; i < _columns - 1; i++)
            {
                width = 100.0f * value[i] / hundredPercent;
                _widths[i] = width;
                _widths[_columns - 1] -= width;
            }
        }
    }

    /// <summary>
    ///     Sets current col/row to Valid(empty) pos after addCell/Table
    /// </summary>
    /// <value>a System.Drawing.Point</value>
    private Point CurrentLocationToNextValidPosition
    {
        set
        {
            // set latest location to next valid position
            int i, j;
            i = value.X;
            j = value.Y;

            do
            {
                if (j + 1 == _columns)
                {
                    // goto next row
                    i++;
                    j = 0;
                }
                else
                {
                    j++;
                }
            }
            while (i < _rows.Count && j < _columns && _rows[i].IsReserved(j));

            _curPosition = new Point(i, j);
        }
    }

    /// <summary>
    ///     @since   iText 2.0.8
    ///     @see com.lowagie.text.LargeElement#isComplete()
    /// </summary>
    public bool ElementComplete
    {
        get => complete;
        set => complete = value;
    }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public override int Type => TABLE;

    /// <summary>
    ///     @since   iText 2.0.8
    ///     @see com.lowagie.text.LargeElement#flushContent()
    /// </summary>
    public void FlushContent()
    {
        NotAddedYet = false;
        var headerrows = new List<Row>();

        for (var i = 0; i < LastHeaderRow + 1; i++)
        {
            headerrows.Add(_rows[i]);
        }

        _rows = headerrows;
    }

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public override bool IsNestable()
        => true;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     IElementListener.
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    public override bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            return listener.Add(this);
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Adds a Cell to the Table at a certain row and column.
    /// </summary>
    /// <param name="aCell">The Cell to add</param>
    /// <param name="row">The row where the Cell will be added</param>
    /// <param name="column">The column where the Cell will be added</param>
    public void AddCell(Cell aCell, int row, int column)
        => AddCell(aCell, new Point(row, column));

    /// <summary>
    ///     methods to add content to the table
    /// </summary>
    /// <summary>
    ///     Adds a Cell to the Table at a certain location.
    /// </summary>
    /// <param name="aCell">The Cell to add</param>
    /// <param name="aLocation">The location where the Cell will be added</param>
    public void AddCell(Cell aCell, object aLocation)
    {
        if (aCell == null)
        {
            throw new ArgumentNullException(nameof(aCell));
        }

        if (aLocation == null)
        {
            throw new ArgumentNullException(nameof(aLocation));
        }

        Point p;

        p = (Point)aLocation;

        if (aCell.IsTable())
        {
            var i = aCell.Elements.GetEnumerator();
            i.MoveNext();
            InsertTable((Table)i.Current, p);
        }

        if (p.X < 0)
        {
            throw new BadElementException("row coordinate of location must be >= 0");
        }

        if (p.Y <= 0 && p.Y > _columns)
        {
            throw new BadElementException("column coordinate of location must be >= 0 and < nr of columns");
        }

        if (!isValidLocation(aCell, p))
        {
            throw new BadElementException("Adding a cell at the location (" + p.X + "," + p.Y + ") with a colspan of " +
                                          aCell.Colspan + " and a rowspan of " + aCell.Rowspan +
                                          " is illegal (beyond boundaries/overlapping).");
        }

        if (aCell.Border == UNDEFINED)
        {
            aCell.Border = _defaultCell.Border;
        }

        aCell.Fill();
        placeCell(_rows, aCell, p);
        CurrentLocationToNextValidPosition = p;
    }

    /// <summary>
    ///     Adds a Cell to the Table.
    /// </summary>
    /// <param name="cell">a Cell</param>
    public void AddCell(Cell cell)
    {
        try
        {
            AddCell(cell, _curPosition);
        }
        catch (BadElementException)
        {
            // don't add the cell
        }
    }

    /// <summary>
    ///     Adds a Cell to the Table.
    /// </summary>
    /// <remarks>
    ///     This is a shortcut for AddCell(Cell cell).
    ///     The Phrase will be converted to a Cell.
    /// </remarks>
    /// <param name="content">a Phrase</param>
    public void AddCell(Phrase content)
        => AddCell(content, _curPosition);

    /// <summary>
    ///     Adds a Cell to the Table.
    /// </summary>
    /// <param name="content">a Phrase</param>
    /// <param name="location">a System.Drawing.Point</param>
    public void AddCell(Phrase content, Point location)
    {
        var cell = new Cell(content);
        cell.Border = _defaultCell.Border;
        cell.BorderWidth = _defaultCell.BorderWidth;
        cell.BorderColor = _defaultCell.BorderColor;
        cell.BackgroundColor = _defaultCell.BackgroundColor;
        cell.HorizontalAlignment = _defaultCell.HorizontalAlignment;
        cell.VerticalAlignment = _defaultCell.VerticalAlignment;
        cell.Colspan = _defaultCell.Colspan;
        cell.Rowspan = _defaultCell.Rowspan;
        AddCell(cell, location);
    }

    /// <summary>
    ///     Adds a Cell to the Table.
    /// </summary>
    /// <remarks>
    ///     This is a shortcut for AddCell(Cell cell).
    ///     The string will be converted to a Cell.
    /// </remarks>
    /// <param name="content">a string</param>
    public void AddCell(string content)
        => AddCell(new Phrase(content), _curPosition);

    /// <summary>
    ///     Adds a Cell to the Table.
    /// </summary>
    /// <remarks>
    ///     This is a shortcut for AddCell(Cell cell, System.Drawing.Point location).
    ///     The string will be converted to a Cell.
    /// </remarks>
    /// <param name="content">a string</param>
    /// <param name="location">a point</param>
    public void AddCell(string content, Point location)
        => AddCell(new Phrase(content), location);

    /// <summary>
    ///     Gives you the posibility to add columns.
    /// </summary>
    /// <param name="aColumns">the number of columns to add</param>
    public void AddColumns(int aColumns)
    {
        var newRows = new List<Row>(_rows.Count);

        var newColumns = _columns + aColumns;
        Row row;

        for (var i = 0; i < _rows.Count; i++)
        {
            row = new Row(newColumns);

            for (var j = 0; j < _columns; j++)
            {
                row.SetElement(_rows[i].GetCell(j), j);
            }

            for (var j = _columns; j < newColumns && i < _curPosition.X; j++)
            {
                row.SetElement(null, j);
            }

            newRows.Add(row);
        }

        // applied 1 column-fix; last column needs to have a width of 0
        var newWidths = new float[newColumns];
        Array.Copy(_widths, 0, newWidths, 0, _columns);

        for (var j = _columns; j < newColumns; j++)
        {
            newWidths[j] = 0;
        }

        _columns = newColumns;
        _widths = newWidths;
        _rows = newRows;
    }

    /// <summary>
    ///     Will fill empty cells with valid blank Cells
    /// </summary>
    public void Complete()
    {
        if (_mTableInserted)
        {
            mergeInsertedTables(); // integrate tables in the table
            _mTableInserted = false;
        }

        if (autoFillEmptyCells)
        {
            fillEmptyMatrixCells();
        }
    }

    /// <summary>
    ///     Create a PdfPTable based on this Table object.
    ///     @throws BadElementException
    /// </summary>
    /// <returns>a PdfPTable object</returns>
    public PdfPTable CreatePdfPTable()
    {
        if (!Convert2Pdfptable)
        {
            throw new BadElementException("No error, just an old style table");
        }

        AutoFillEmptyCells = true;
        Complete();
        var pdfptable = new PdfPTable(_widths);
        pdfptable.ElementComplete = complete;

        if (NotAddedYet)
        {
            pdfptable.SkipFirstHeader = true;
        }

        var tEvt = new SimpleTable();
        tEvt.CloneNonPositionParameters(this);
        tEvt.Cellspacing = _cellspacing;
        pdfptable.TableEvent = tEvt;
        pdfptable.HeaderRows = _lastHeaderRow + 1;
        pdfptable.SplitLate = _cellsFitPage;
        pdfptable.KeepTogether = _tableFitsPage;

        if (!float.IsNaN(_offset))
        {
            pdfptable.SpacingBefore = _offset;
        }

        pdfptable.HorizontalAlignment = _alignment;

        if (Locked)
        {
            pdfptable.TotalWidth = _width;
            pdfptable.LockedWidth = true;
        }
        else
        {
            pdfptable.WidthPercentage = _width;
        }

        foreach (var row in this)
        {
            IElement cell;
            PdfPCell pcell;

            for (var i = 0; i < row.Columns; i++)
            {
                if ((cell = (IElement)row.GetCell(i)) != null)
                {
                    if (cell is Table)
                    {
                        pcell = new PdfPCell(((Table)cell).CreatePdfPTable());
                    }
                    else if (cell is Cell)
                    {
                        pcell = ((Cell)cell).CreatePdfPCell();
                        pcell.Padding = _cellpadding + _cellspacing / 2f;
                        var cEvt = new SimpleCell(SimpleCell.CELL);
                        cEvt.CloneNonPositionParameters((Cell)cell);
                        cEvt.Spacing = _cellspacing * 2f;
                        pcell.CellEvent = cEvt;
                    }
                    else
                    {
                        pcell = new PdfPCell();
                    }

                    pdfptable.AddCell(pcell);
                }
            }
        }

        return pdfptable;
    }

    public void DeleteAllRows()
    {
        _rows.Clear();
        _rows.Add(new Row(_columns));
        _curPosition.X = 0;
        _curPosition.Y = 0;
        _lastHeaderRow = -1;
    }

    /// <summary>
    ///     Deletes a column in this table.
    /// </summary>
    /// <param name="column">the number of the column that has to be deleted</param>
    public void DeleteColumn(int column)
    {
        var newWidths = new float[--_columns];
        Array.Copy(_widths, 0, newWidths, 0, column);
        Array.Copy(_widths, column + 1, newWidths, column, _columns - column);
        Widths = newWidths;
        Array.Copy(_widths, 0, newWidths, 0, _columns);
        _widths = newWidths;
        Row row;
        var size = _rows.Count;

        for (var i = 0; i < size; i++)
        {
            row = _rows[i];
            row.DeleteColumn(column);
            _rows[i] = row;
        }

        if (column == _columns)
        {
            _curPosition.X++;
            _curPosition.Y = 0;
        }
    }

    /// <summary>
    ///     Deletes all rows in this table.
    ///     (contributed by dperezcar@fcc.es)
    /// </summary>
    /// <summary>
    ///     Deletes the last row in this table.
    /// </summary>
    /// <returns>true if the row was deleted; false if not</returns>
    public bool DeleteLastRow()
        => DeleteRow(_rows.Count - 1);

    /// <summary>
    ///     Deletes a row.
    /// </summary>
    /// <param name="row">the number of the row to delete</param>
    /// <returns>true if the row was deleted; false if not</returns>
    public bool DeleteRow(int row)
    {
        if (row < 0 || row >= _rows.Count)
        {
            return false;
        }

        _rows.RemoveAt(row);
        _curPosition.X--;

        return true;
    }

    /// <summary>
    ///     Marks the last row of the table headers.
    /// </summary>
    /// <returns>the number of the last row of the table headers</returns>
    public int EndHeaders()
    {
        /* patch sep 8 2001 Francesco De Milato */
        _lastHeaderRow = _curPosition.X - 1;

        return _lastHeaderRow;
    }

    public override float GetBottom(float margin)
    {
        errorDimensions();

        return 0;
    }

    /// <summary>
    ///     returns the element at the position row, column
    ///     (Cast to Cell or Table)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <returns>an object</returns>
    public object GetElement(int row, int column)
        => _rows[row].GetCell(column);

    /// <summary>
    ///     Gets an Iterator of all the Rows.
    /// </summary>
    /// <returns>an IEnumerator</returns>
    public List<Row>.Enumerator GetEnumerator()
        => _rows.GetEnumerator();

    public override float GetLeft(float margin)
    {
        errorDimensions();

        return 0;
    }

    public override float GetRight(float margin)
    {
        errorDimensions();

        return 0;
    }

    public override float GetTop(float margin)
    {
        errorDimensions();

        return 0;
    }

    /// <summary>
    ///     Gets an array with the positions of the borders between every column.
    /// </summary>
    /// <remarks>
    ///     This method translates the widths expressed in percentages into the
    ///     x-coordinate of the borders of the columns on a real document.
    /// </remarks>
    /// <param name="left">this is the position of the first border at the left (cellpadding not included)</param>
    /// <param name="totalWidth">
    ///     this is the space between the first border at the left
    ///     and the last border at the right (cellpadding not included)
    /// </param>
    /// <returns>an array with borderpositions</returns>
    public float[] GetWidths(float left, float totalWidth)
    {
        // for x columns, there are x+1 borders
        var w = new float[_columns + 1];
        float wPercentage;

        if (Locked)
        {
            wPercentage = 100 * _width / totalWidth;
        }
        else
        {
            wPercentage = _width;
        }

        // the border at the left is calculated
        switch (_alignment)
        {
            case ALIGN_LEFT:
                w[0] = left;

                break;
            case ALIGN_RIGHT:
                w[0] = left + totalWidth * (100 - wPercentage) / 100;

                break;
            case ALIGN_CENTER:
            default:
                w[0] = left + totalWidth * (100 - wPercentage) / 200;

                break;
        }

        // the total available width is changed
        totalWidth = totalWidth * wPercentage / 100;

        // the inner borders are calculated
        for (var i = 1; i < _columns; i++)
        {
            w[i] = w[i - 1] + _widths[i - 1] * totalWidth / 100;
        }

        // the border at the right is calculated
        w[_columns] = w[0] + totalWidth;

        return w;
    }

    /// <summary>
    ///     To put a table within the existing table at the current position
    ///     generateTable will of course re-arrange the widths of the columns.
    /// </summary>
    /// <param name="aTable">the table you want to insert</param>
    public void InsertTable(Table aTable)
    {
        if (aTable == null)
        {
            throw new ArgumentNullException(nameof(aTable));
        }

        InsertTable(aTable, _curPosition);
    }

    /// <summary>
    ///     To put a table within the existing table at the given position
    ///     generateTable will of course re-arrange the widths of the columns.
    /// </summary>
    /// <param name="aTable">The Table to add</param>
    /// <param name="row">The row where the Cell will be added</param>
    /// <param name="column">The column where the Cell will be added</param>
    public void InsertTable(Table aTable, int row, int column)
    {
        if (aTable == null)
        {
            throw new ArgumentNullException(nameof(aTable));
        }

        InsertTable(aTable, new Point(row, column));
    }

    /// <summary>
    ///     To put a table within the existing table at the given position
    ///     generateTable will of course re-arrange the widths of the columns.
    /// </summary>
    /// <param name="aTable">the table you want to insert</param>
    /// <param name="p">a System.Drawing.Point</param>
    public void InsertTable(Table aTable, Point p)
    {
        if (aTable == null)
        {
            throw new ArgumentNullException(nameof(aTable));
        }

        _mTableInserted = true;
        aTable.Complete();

        if (p.Y > _columns)
        {
            throw new ArgumentException(
                "insertTable -- wrong columnposition(" + p.Y + ") of location; max =" + _columns);
        }

        var rowCount = p.X + 1 - _rows.Count;
        var i = 0;

        if (rowCount > 0)
        {
            //create new rows ?
            for (; i < rowCount; i++)
            {
                _rows.Add(new Row(_columns));
            }
        }

        _rows[p.X].SetElement(aTable, p.Y);

        CurrentLocationToNextValidPosition = p;
    }

    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <returns>an ArrayList</returns>
    /// <summary>
    ///     public ArrayList Chunks {
    /// </summary>
    /// <summary>
    ///     get {
    /// </summary>
    /// <summary>
    ///     return new ArrayList();
    /// </summary>
    /// <summary>
    ///     }
    /// </summary>
    /// <summary>
    ///     }
    /// </summary>
    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     methods to set the membervariables
    /// </summary>
    /// <summary>
    ///     Sets the alignment of this paragraph.
    /// </summary>
    /// <param name="alignment">the new alignment as a string</param>
    public void SetAlignment(string alignment)
    {
        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_LEFT))
        {
            _alignment = ALIGN_LEFT;

            return;
        }

        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_RIGHT))
        {
            _alignment = ALIGN_RIGHT;

            return;
        }

        _alignment = ALIGN_CENTER;
    }

    /// <summary>
    ///     Sets the widths of the different columns (percentages).
    /// </summary>
    /// <remarks>
    ///     You can give up relative values of borderwidths.
    ///     The sum of these values will be considered 100%.
    ///     The values will be recalculated as percentages of this sum.
    /// </remarks>
    /// <param name="widths">an array with values</param>
    public void SetWidths(int[] widths)
    {
        if (widths == null)
        {
            throw new ArgumentNullException(nameof(widths));
        }

        var tb = new float[widths.Length];

        for (var k = 0; k < widths.Length; ++k)
        {
            tb[k] = widths[k];
        }

        Widths = tb;
    }

    /// <summary>
    ///     Sets the unset cell properties to be the table defaults.
    /// </summary>
    /// <param name="aCell">The cell to set to table defaults as necessary.</param>
    private void assumeTableDefaults(Cell aCell)
    {
        if (aCell.Border == UNDEFINED)
        {
            aCell.Border = _defaultCell.Border;
        }

        if (aCell.BorderWidth.ApproxEquals(UNDEFINED))
        {
            aCell.BorderWidth = _defaultCell.BorderWidth;
        }

        if (aCell.BorderColor == null)
        {
            aCell.BorderColor = _defaultCell.BorderColor;
        }

        if (aCell.BackgroundColor == null)
        {
            aCell.BackgroundColor = _defaultCell.BackgroundColor;
        }

        if (aCell.HorizontalAlignment == ALIGN_UNDEFINED)
        {
            aCell.HorizontalAlignment = _defaultCell.HorizontalAlignment;
        }

        if (aCell.VerticalAlignment == ALIGN_UNDEFINED)
        {
            aCell.VerticalAlignment = _defaultCell.VerticalAlignment;
        }
    }

    private static void errorDimensions()
        => throw new InvalidOperationException("Dimensions of a Table can't be calculated. See the FAQ.");

    /// <summary>
    ///     Integrates all added tables and recalculates column widths.
    /// </summary>
    private void fillEmptyMatrixCells()
    {
        for (var i = 0; i < _rows.Count; i++)
        {
            for (var j = 0; j < _columns; j++)
            {
                if (_rows[i].IsReserved(j) == false)
                {
                    AddCell(_defaultCell, new Point(i, j));
                }
            }
        }
    }

    /// <summary>
    ///     check if Cell 'fits' the table.
    /// </summary>
    /// <remarks>
    ///     rowspan/colspan not beyond borders
    ///     spanned cell don't overlap existing cells
    /// </remarks>
    /// <param name="aCell">the cell that has to be checked</param>
    /// <param name="aLocation">the location where the cell has to be placed</param>
    /// <returns></returns>
    private bool isValidLocation(Cell aCell, Point aLocation)
    {
        // rowspan not beyond last column
        if (aLocation.X < _rows.Count)
        {
            // if false : new location is already at new, not-yet-created area so no check
            if (aLocation.Y + aCell.Colspan > _columns)
            {
                return false;
            }

            var difx = _rows.Count - aLocation.X > aCell.Rowspan ? aCell.Rowspan : _rows.Count - aLocation.X;
            var dify = _columns - aLocation.Y > aCell.Colspan ? aCell.Colspan : _columns - aLocation.Y;

            // no other content at cells targetted by rowspan/colspan
            for (var i = aLocation.X; i < aLocation.X + difx; i++)
            {
                for (var j = aLocation.Y; j < aLocation.Y + dify; j++)
                {
                    if (_rows[i].IsReserved(j))
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            if (aLocation.Y + aCell.Colspan > _columns)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Integrates all added tables and recalculates column widths.
    /// </summary>
    private void mergeInsertedTables()
    {
        int i = 0, j = 0;
        float[] lNewWidths = null;
        var lDummyWidths = new int[_columns]; // to keep track in how many new cols this one will be split
        var lDummyColumnWidths = new float[_columns][]; // bugfix Tony Copping
        var lDummyHeights = new int[_rows.Count]; // to keep track in how many new rows this one will be split
        List<Row> newRows = null;
        var isTable = false;
        int lTotalRows = 0, lTotalColumns = 0;
        int lNewMaxRows = 0, lNewMaxColumns = 0;

        Table lDummyTable = null;

        // first we'll add new columns when needed
        // check one column at a time, find maximum needed nr of cols
        // Search internal tables and find one with max columns
        for (j = 0; j < _columns; j++)
        {
            lNewMaxColumns = 1; // value to hold in how many columns the current one will be split
            float[] tmpWidths = null;

            for (i = 0; i < _rows.Count; i++)
            {
                if (_rows[i].GetCell(j) is Table)
                {
                    isTable = true;
                    lDummyTable = (Table)_rows[i].GetCell(j);

                    if (tmpWidths == null)
                    {
                        tmpWidths = lDummyTable._widths;
                        lNewMaxColumns = tmpWidths.Length;
                    }
                    else
                    {
                        var cols = lDummyTable.Dimension.width;
                        var tmpWidthsN = new float[cols * tmpWidths.Length];
                        float tpW = 0, btW = 0, totW = 0;
                        int tpI = 0, btI = 0, totI = 0;
                        tpW += tmpWidths[0];
                        btW += lDummyTable._widths[0];

                        while (tpI < tmpWidths.Length && btI < cols)
                        {
                            if (btW > tpW)
                            {
                                tmpWidthsN[totI] = tpW - totW;
                                tpI++;

                                if (tpI < tmpWidths.Length)
                                {
                                    tpW += tmpWidths[tpI];
                                }
                            }
                            else
                            {
                                tmpWidthsN[totI] = btW - totW;
                                btI++;

                                if (Math.Abs(btW - tpW) < 0.0001)
                                {
                                    tpI++;

                                    if (tpI < tmpWidths.Length)
                                    {
                                        tpW += tmpWidths[tpI];
                                    }
                                }

                                if (btI < cols)
                                {
                                    btW += lDummyTable._widths[btI];
                                }
                            }

                            totW += tmpWidthsN[totI];
                            totI++;
                        }

                        tmpWidths = new float[totI];
                        Array.Copy(tmpWidthsN, 0, tmpWidths, 0, totI);
                        lNewMaxColumns = totI;
                    }
                }
            }

            lDummyColumnWidths[j] = tmpWidths;
            lTotalColumns += lNewMaxColumns;
            lDummyWidths[j] = lNewMaxColumns;
        }

        // next we'll add new rows when needed
        for (i = 0; i < _rows.Count; i++)
        {
            lNewMaxRows = 1; // holds value in how many rows the current one will be split

            for (j = 0; j < _columns; j++)
            {
                if (_rows[i].GetCell(j) is Table)
                {
                    isTable = true;
                    lDummyTable = (Table)_rows[i].GetCell(j);

                    if (lDummyTable.Dimension.height > lNewMaxRows)
                    {
                        lNewMaxRows = lDummyTable.Dimension.height;
                    }
                }
            }

            lTotalRows += lNewMaxRows;
            lDummyHeights[i] = lNewMaxRows;
        }

        if (lTotalColumns != _columns || lTotalRows != _rows.Count || isTable) // NO ADJUSTMENT
        {
            // ** WIDTH
            // set correct width for new columns
            // divide width over new nr of columns
            // Take new max columns of internal table and work out widths for each col
            lNewWidths = new float[lTotalColumns];
            var lDummy = 0;

            for (var tel = 0; tel < _widths.Length; tel++)
            {
                if (lDummyWidths[tel] != 1)
                {
                    // divide
                    for (var tel2 = 0; tel2 < lDummyWidths[tel]; tel2++)
                    {
                        lNewWidths[lDummy] = _widths[tel] * lDummyColumnWidths[tel][tel2] / 100f; // bugfix Tony Copping
                        lDummy++;
                    }
                }
                else
                {
                    lNewWidths[lDummy] = _widths[tel];
                    lDummy++;
                }
            }

            // ** FILL OUR NEW TABLE
            // generate new table
            // set new widths
            // copy old values
            newRows = new List<Row>(lTotalRows);

            for (i = 0; i < lTotalRows; i++)
            {
                newRows.Add(new Row(lTotalColumns));
            }

            int lDummyRow = 0, lDummyColumn = 0; // to remember where we are in the new, larger table
            object lDummyElement = null;

            for (i = 0; i < _rows.Count; i++)
            {
                lDummyColumn = 0;
                lNewMaxRows = 1;

                for (j = 0; j < _columns; j++)
                {
                    if (_rows[i].GetCell(j) is Table) // copy values from embedded table
                    {
                        lDummyTable = (Table)_rows[i].GetCell(j);

                        // Work out where columns in table table correspond to columns in current table
                        var colMap = new int[lDummyTable._widths.Length + 1];
                        int cb = 0, ct = 0;

                        for (; cb < lDummyTable._widths.Length; cb++)
                        {
                            colMap[cb] = lDummyColumn + ct;

                            float wb;
                            wb = lDummyTable._widths[cb];

                            float wt = 0;

                            while (ct < lDummyWidths[j])
                            {
                                wt += lDummyColumnWidths[j][ct++];

                                if (Math.Abs(wb - wt) < 0.0001)
                                {
                                    break;
                                }
                            }
                        }

                        colMap[cb] = lDummyColumn + ct;

                        // need to change this to work out how many cols to span
                        for (var k = 0; k < lDummyTable.Dimension.height; k++)
                        {
                            for (var l = 0; l < lDummyTable.Dimension.width; l++)
                            {
                                lDummyElement = lDummyTable.GetElement(k, l);

                                if (lDummyElement != null)
                                {
                                    var col = lDummyColumn + l;

                                    if (lDummyElement is Cell)
                                    {
                                        var lDummyC = (Cell)lDummyElement;

                                        // Find col to add cell in and set col span
                                        col = colMap[l];
                                        var ot = colMap[l + lDummyC.Colspan];

                                        lDummyC.Colspan = ot - col;
                                    }

                                    newRows[k + lDummyRow]
                                        .AddElement(lDummyElement,
                                            col); // use addElement to set reserved status ok in row
                                }
                            }
                        }
                    }
                    else // copy others values
                    {
                        var aElement = GetElement(i, j);

                        if (aElement is Cell)
                        {
                            // adjust spans for cell
                            ((Cell)aElement).Rowspan = ((Cell)_rows[i].GetCell(j)).Rowspan + lDummyHeights[i] - 1;
                            ((Cell)aElement).Colspan = ((Cell)_rows[i].GetCell(j)).Colspan + lDummyWidths[j] - 1;

                            // most likely this cell covers a larger area because of the row/cols splits : define not-to-be-filled cells
                            placeCell(newRows, (Cell)aElement, new Point(lDummyRow, lDummyColumn));
                        }
                    }

                    lDummyColumn += lDummyWidths[j];
                }

                lDummyRow += lDummyHeights[i];
            }

            // Set our new matrix
            _columns = lTotalColumns;
            _rows = newRows;
            _widths = lNewWidths;
        }
    }

    /// <summary>
    ///     Inserts a Cell in a cell-array and reserves cells defined by row-/colspan.
    /// </summary>
    /// <param name="someRows">some rows</param>
    /// <param name="aCell">the cell that has to be inserted</param>
    /// <param name="aPosition">the position where the cell has to be placed</param>
    private void placeCell(List<Row> someRows, Cell aCell, Point aPosition)
    {
        int i;
        Row row = null;
        var rowCount = aPosition.X + aCell.Rowspan - someRows.Count;
        assumeTableDefaults(aCell);

        if (aPosition.X + aCell.Rowspan > someRows.Count)
        {
            //create new rows ?
            for (i = 0; i < rowCount; i++)
            {
                row = new Row(_columns);
                someRows.Add(row);
            }
        }

        // reserve cell in rows below
        for (i = aPosition.X + 1; i < aPosition.X + aCell.Rowspan; i++)
        {
            if (!someRows[i].Reserve(aPosition.Y, aCell.Colspan))
            {
                // should be impossible to come here :-)
                throw new InvalidOperationException("addCell - error in reserve");
            }
        }

        row = someRows[aPosition.X];
        row.AddElement(aCell, aPosition.Y);
    }
}