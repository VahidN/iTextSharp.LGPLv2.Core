namespace iTextSharp.text;

/// <summary>
///     A Row is part of a Table
///     and contains some Cells.
/// </summary>
/// <remarks>
///     All Rows are constructed by a Table-object.
///     You don't have to construct any Row yourself.
///     In fact you can't construct a Row outside the package.
///     Since a Cell can span several rows and/or columns
///     a row can contain reserved space without any content.
/// </remarks>
public class Row : IElement
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> id of the Cell element in a Row</summary>
    public static int Cell = 1;

    /// <summary> id of a null element in a Row</summary>
    public static int Null = 0;

    /// <summary> id of the Table element in a Row</summary>
    public static int Table = 2;

    /// <summary> This is the array of Objects (Cell or Table). </summary>
    protected object[] Cells;

    /// <summary> This is the number of columns in the Row. </summary>
    protected int columns;

    /// <summary> This is a valid position the Row. </summary>
    protected int CurrentColumn;

    /// <summary> This is the horizontal alignment. </summary>
    protected int horizontalAlignment;

    /// <summary> This is the array that keeps track of reserved cells. </summary>
    protected bool[] Reserved;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a Row with a certain number of columns.
    /// </summary>
    /// <param name="columns">a number of columns</param>
    internal Row(int columns)
    {
        this.columns = columns;
        Reserved = new bool[columns];
        Cells = new object[columns];
        CurrentColumn = 0;
    }

    /// <summary>
    ///     Gets the number of columns.
    /// </summary>
    /// <value>a value</value>
    public int Columns => columns;

    /// <summary>
    ///     Gets the horizontal Element.
    /// </summary>
    /// <value>a value</value>
    public int HorizontalAlignment
    {
        get => horizontalAlignment;
        set => horizontalAlignment = value;
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public IList<Chunk> Chunks => new List<Chunk>();

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public int Type => Element.ROW;

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsNestable() => false;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to a
    ///     IElementListener.
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    public bool Process(IElementListener listener)
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
    ///     Gets a Cell or Table from a certain column.
    /// </summary>
    /// <param name="column">the column the Cell/Table is in.</param>
    /// <returns>
    ///     the Cell,Table or Object if the column was
    ///     reserved or null if empty.
    /// </returns>
    public object GetCell(int column)
    {
        if (column < 0 || column > columns)
        {
            throw new InvalidOperationException("getCell at illegal index :" + column + " max is " + columns);
        }

        return Cells[column];
    }

    /// <summary>
    ///     Checks if the row is empty.
    /// </summary>
    /// <returns>true if none of the columns is reserved.</returns>
    public bool IsEmpty()
    {
        for (var i = 0; i < columns; i++)
        {
            if (Cells[i] != null)
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    ///     Adds a Cell to the Row.
    /// </summary>
    /// <param name="element">the element to add (currently only Cells and Tables supported)</param>
    /// <returns>
    ///     the column position the Cell was added,
    ///     or -1 if the element couldn't be added.
    /// </returns>
    internal int AddElement(object element) => AddElement(element, CurrentColumn);

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Adds an element to the Row at the position given.
    /// </summary>
    /// <param name="element">the element to add. (currently only Cells and Tables supported</param>
    /// <param name="column">the position where to add the cell</param>
    /// <returns>
    ///     the column position the Cell was added,
    ///     or -1 if the Cell couldn't be added.
    /// </returns>
    internal int AddElement(object element, int column)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (column < 0 || column > columns)
        {
            throw new InvalidOperationException("addCell - illegal column argument");
        }

        if (!(getObjectId(element) == Cell || getObjectId(element) == Table))
        {
            throw new ArgumentException("addCell - only Cells or Tables allowed");
        }

        var lColspan = element is Cell ? ((Cell)element).Colspan : 1;

        if (!Reserve(column, lColspan))
        {
            return -1;
        }

        Cells[column] = element;
        CurrentColumn += lColspan - 1;

        return column;
    }

    /// <summary>
    ///     Deletes a certain column has been deleted.
    /// </summary>
    /// <param name="column">the number of the column to delete</param>
    internal void DeleteColumn(int column)
    {
        if (column >= columns || column < 0)
        {
            throw new InvalidOperationException("getCell at illegal index : " + column);
        }

        columns--;
        var newReserved = new bool[columns];
        object[] newCells = new Cell[columns];

        for (var i = 0; i < column; i++)
        {
            newReserved[i] = Reserved[i];
            newCells[i] = Cells[i];
            if (newCells[i] != null && i + ((Cell)newCells[i]).Colspan > column)
            {
                ((Cell)newCells[i]).Colspan = ((Cell)Cells[i]).Colspan - 1;
            }
        }

        for (var i = column; i < columns; i++)
        {
            newReserved[i] = Reserved[i + 1];
            newCells[i] = Cells[i + 1];
        }

        if (Cells[column] != null && ((Cell)Cells[column]).Colspan > 1)
        {
            newCells[column] = Cells[column];
            ((Cell)newCells[column]).Colspan = ((Cell)newCells[column]).Colspan - 1;
        }

        Reserved = newReserved;
        Cells = newCells;
    }

    /// <summary>
    ///     Returns true/false when this position in the Row has been reserved, either filled or through a colspan of an
    ///     Element.
    /// </summary>
    /// <param name="column">the column.</param>
    /// <returns>true if the column was reserved, false if not.</returns>
    internal bool IsReserved(int column) => Reserved[column];

    /// <summary>
    ///     Reserves a Cell in the Row.
    /// </summary>
    /// <param name="column">the column that has to be reserved.</param>
    /// <returns>true if the column was reserved, false if not.</returns>
    internal bool Reserve(int column) => Reserve(column, 1);

    /// <summary>
    ///     Reserves a Cell in the Row.
    /// </summary>
    /// <param name="column">the column that has to be reserved.</param>
    /// <param name="size">the number of columns</param>
    /// <returns>true if the column was reserved, false if not.</returns>
    internal bool Reserve(int column, int size)
    {
        if (column < 0 || column + size > columns)
        {
            throw new InvalidOperationException("reserve - incorrect column/size");
        }

        for (var i = column; i < column + size; i++)
        {
            if (Reserved[i])
            {
                // undo reserve
                for (var j = i; j >= column; j--)
                {
                    Reserved[j] = false;
                }

                return false;
            }

            Reserved[i] = true;
        }

        return true;
    }

    /// <summary>
    ///     Puts Cell to the Row at the position given, doesn't reserve colspan.
    /// </summary>
    /// <param name="aElement">the cell to add.</param>
    /// <param name="column">the position where to add the cell.</param>
    internal void SetElement(object aElement, int column)
    {
        if (Reserved[column])
        {
            throw new ArgumentException("setElement - position already taken");
        }

        Cells[column] = aElement;
        if (aElement != null)
        {
            Reserved[column] = true;
        }
    }

    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Returns the type-id of the element in a Row.
    /// </summary>
    /// <param name="column">the column of which you'd like to know the type</param>
    /// <returns>the element id</returns>
    private int getElementId(int column)
    {
        if (Cells[column] == null)
        {
            return Null;
        }

        if (Cells[column] is Cell)
        {
            return Cell;
        }

        if (Cells[column] is Table)
        {
            return Table;
        }

        return -1;
    }


    /// <summary>
    ///     Returns the type-id of an Object.
    /// </summary>
    /// <param name="element"></param>
    /// <returns>the object of which you'd like to know the type-id, -1 if invalid</returns>
    private static int getObjectId(object element)
    {
        if (element == null)
        {
            return Null;
        }

        if (element is Cell)
        {
            return Cell;
        }

        if (element is Table)
        {
            return Table;
        }

        return -1;
    }
}