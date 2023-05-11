using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Formats content into one or more columns bounded by a
///     rectangle.  The columns may be simple rectangles or
///     more complicated shapes. Add all of the columns before
///     adding content. Column continuation is supported. A MultiColumnText object may be added to
///     a document using  Document.add .
///     @author Steve Appling
/// </summary>
public class MultiColumnText : IElement
{
    /// <summary>
    ///     special constant for automatic calculation of height
    /// </summary>
    public const float AUTOMATIC = -1f;

    /// <summary>
    ///     Array of  ColumnDef  objects used to define the columns
    /// </summary>
    private readonly List<ColumnDef> _columnDefs;

    /// <summary>
    ///     ColumnText object used to do all the real work.  This same object is used for all columns
    /// </summary>
    private readonly ColumnText _columnText;

    /// <summary>
    ///     total desiredHeight of columns.  If  AUTOMATIC , this means fill pages until done.
    ///     This may be larger than one page
    /// </summary>
    private readonly float _desiredHeight;

    private bool _columnsRightToLeft;

    private int _currentColumn;

    private PdfDocument _document;

    private float _nextY = AUTOMATIC;

    /// <summary>
    ///     true if all the text could not be written out due to height restriction
    /// </summary>
    private bool _overflow;

    /// <summary>
    ///     true if all columns are simple (rectangular)
    /// </summary>
    private bool _simple = true;

    /// <summary>
    ///     Top of the columns - y position on starting page.
    ///     If  AUTOMATIC , it means current y position when added to document
    /// </summary>
    private float _top;

    /// <summary>
    ///     total height of element written out so far
    /// </summary>
    private float _totalHeight;

    /// <summary>
    ///     Default constructor.  Sets height to  AUTOMATIC .
    ///     Columns will repeat on each page as necessary to accomodate content length.
    /// </summary>
    public MultiColumnText() : this(AUTOMATIC)
    {
    }

    /// <summary>
    ///     Construct a MultiColumnText container of the specified height.
    ///     If height is  AUTOMATIC , fill complete pages until done.
    ///     If a specific height is used, it may span one or more pages.
    /// </summary>
    /// <param name="height"></param>
    public MultiColumnText(float height)
    {
        _columnDefs = new List<ColumnDef>();
        _desiredHeight = height;
        _top = AUTOMATIC;
        // canvas will be set later
        _columnText = new ColumnText(null);
        _totalHeight = 0f;
    }

    /// <summary>
    ///     Construct a MultiColumnText container of the specified height
    ///     starting at the specified Y position.
    /// </summary>
    /// <param name="height"></param>
    /// <param name="top"></param>
    public MultiColumnText(float top, float height)
    {
        _columnDefs = new List<ColumnDef>();
        _desiredHeight = height;
        _top = top;
        _nextY = top;
        // canvas will be set later
        _columnText = new ColumnText(null);
        _totalHeight = 0f;
    }

    /// <summary>
    ///     Gets the current column.
    /// </summary>
    /// <returns>the current column</returns>
    public int CurrentColumn
    {
        get
        {
            if (_columnsRightToLeft)
            {
                return _columnDefs.Count - _currentColumn - 1;
            }

            return _currentColumn;
        }
    }

    /// <summary>
    ///     Sets the ratio between the extra word spacing and the extra character spacing
    ///     when the text is fully justified.
    ///     Extra word spacing will grow  spaceCharRatio  times more than extra character spacing.
    ///     If the ratio is  PdfWriter.NO_SPACE_CHAR_RATIO  then the extra character spacing
    ///     will be zero.
    /// </summary>
    public float SpaceCharRatio
    {
        set => _columnText.SpaceCharRatio = value;
    }

    /// <summary>
    ///     Sets the run direction.
    /// </summary>
    public int RunDirection
    {
        set => _columnText.RunDirection = value;
    }

    /// <summary>
    ///     Sets the arabic shaping options. The option can be AR_NOVOWEL,
    ///     AR_COMPOSEDTASHKEEL and AR_LIG.
    /// </summary>
    public int ArabicOptions
    {
        set => _columnText.ArabicOptions = value;
    }

    /// <summary>
    ///     Sets the default alignment
    /// </summary>
    public int Alignment
    {
        set => _columnText.Alignment = value;
    }


    /// <summary>
    ///     Processes the element by adding it to an
    ///     ElementListener .
    /// </summary>
    /// <param name="listener">an  ElementListener </param>
    /// <returns> true  if the element was processed successfully</returns>
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
    ///     Gets the type of the text element.
    /// </summary>
    /// <returns>a type</returns>

    public int Type => Element.MULTI_COLUMN_TEXT;

    /// <summary>
    ///     Returns null - not used
    /// </summary>
    /// <returns>null</returns>

    public IList<Chunk> Chunks => null;

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
    ///     Indicates that all of the text did not fit in the
    ///     specified height. Note that isOverflow will return
    ///     false before the MultiColumnText object has been
    ///     added to the document.  It will always be false if
    ///     the height is AUTOMATIC.
    /// </summary>
    /// <returns>true if there is still space left in the column</returns>
    public bool IsOverflow() => _overflow;

    /// <summary>
    ///     Copy the parameters from the specified ColumnText to use
    ///     when rendering.  Parameters like  setArabicOptions
    ///     must be set in this way.
    /// </summary>
    /// <param name="sourceColumn"></param>
    public void UseColumnParams(ColumnText sourceColumn)
    {
        // note that canvas will be overwritten later
        _columnText.SetSimpleVars(sourceColumn);
    }

    /// <summary>
    ///     Add a new column.  The parameters are limits for each column
    ///     wall in the format of a sequence of points (x1,y1,x2,y2,...).
    /// </summary>
    /// <param name="left">limits for left column</param>
    /// <param name="right">limits for right column</param>
    public void AddColumn(float[] left, float[] right)
    {
        var nextDef = new ColumnDef(left, right, this);
        if (!nextDef.IsSimple())
        {
            _simple = false;
        }

        _columnDefs.Add(nextDef);
    }

    /// <summary>
    ///     Add a simple rectangular column with specified left
    ///     and right x position boundaries.
    /// </summary>
    /// <param name="left">left boundary</param>
    /// <param name="right">right boundary</param>
    public void AddSimpleColumn(float left, float right)
    {
        var newCol = new ColumnDef(left, right, this);
        _columnDefs.Add(newCol);
    }

    /// <summary>
    ///     Add the specified number of evenly spaced rectangular columns.
    ///     Columns will be seperated by the specified gutterWidth.
    /// </summary>
    /// <param name="left">left boundary of first column</param>
    /// <param name="right">right boundary of last column</param>
    /// <param name="gutterWidth">width of gutter spacing between columns</param>
    /// <param name="numColumns">number of columns to add</param>
    public void AddRegularColumns(float left, float right, float gutterWidth, int numColumns)
    {
        var currX = left;
        var width = right - left;
        var colWidth = (width - gutterWidth * (numColumns - 1)) / numColumns;
        for (var i = 0; i < numColumns; i++)
        {
            AddSimpleColumn(currX, currX + colWidth);
            currX += colWidth + gutterWidth;
        }
    }

    /// <summary>
    ///     Adds a  Phrase  to the current text array.
    ///     Will not have any effect if addElement() was called before.
    ///     @since	2.1.5
    /// </summary>
    /// <param name="phrase">the text</param>
    public void AddText(Phrase phrase)
    {
        _columnText.AddText(phrase);
    }

    /// <summary>
    ///     Adds a  Chunk  to the current text array.
    ///     Will not have any effect if addElement() was called before.
    ///     @since	2.1.5
    /// </summary>
    /// <param name="chunk">the text</param>
    public void AddText(Chunk chunk)
    {
        _columnText.AddText(chunk);
    }

    /// <summary>
    ///     Add an element to be rendered in a column.
    ///     Note that you can only add a  Phrase
    ///     or a  Chunk  if the columns are
    ///     not all simple.  This is an underlying restriction in
    ///     {@link com.lowagie.text.pdf.ColumnText}
    ///     @throws DocumentException if element can't be added
    /// </summary>
    /// <param name="element">element to add</param>
    public void AddElement(IElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (_simple)
        {
            _columnText.AddElement(element);
        }
        else if (element is Phrase)
        {
            _columnText.AddText((Phrase)element);
        }
        else if (element is Chunk)
        {
            _columnText.AddText((Chunk)element);
        }
        else
        {
            throw new DocumentException("Can't add " + element.GetType() + " to MultiColumnText with complex columns");
        }
    }


    /// <summary>
    ///     Write out the columns.  After writing, use
    ///     {@link #isOverflow()} to see if all text was written.
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="canvas">PdfContentByte to write with</param>
    /// <param name="document">document to write to (only used to get page limit info)</param>
    /// <param name="documentY">starting y position to begin writing at</param>
    /// <returns>the current height (y position) after writing the columns</returns>
    public float Write(PdfContentByte canvas, PdfDocument document, float documentY)
    {
        _document = document ?? throw new ArgumentNullException(nameof(document));
        _columnText.Canvas = canvas;
        if (_columnDefs.Count == 0)
        {
            throw new DocumentException("MultiColumnText has no columns");
        }

        _overflow = false;
        float currentHeight = 0;
        var done = false;
        while (!done)
        {
            if (_top.ApproxEquals(AUTOMATIC))
            {
                _top = document.GetVerticalPosition(true);
            }
            else if (_nextY.ApproxEquals(AUTOMATIC))
            {
                _nextY = document
                    .GetVerticalPosition(true); // RS - 07/07/2005 - - Get current doc writing position for top of columns on new page.
            }

            var currentDef = _columnDefs[CurrentColumn];
            _columnText.YLine = _top;

            var left = currentDef.ResolvePositions(Rectangle.LEFT_BORDER);
            var right = currentDef.ResolvePositions(Rectangle.RIGHT_BORDER);
            if (document.IsMarginMirroring() && document.PageNumber % 2 == 0)
            {
                var delta = document.RightMargin - document.Left;
                left = (float[])left.Clone();
                right = (float[])right.Clone();
                for (var i = 0; i < left.Length; i += 2)
                {
                    left[i] -= delta;
                }

                for (var i = 0; i < right.Length; i += 2)
                {
                    right[i] -= delta;
                }
            }

            currentHeight = Math.Max(currentHeight, getHeight(left, right));

            if (currentDef.IsSimple())
            {
                _columnText.SetSimpleColumn(left[2], left[3], right[0], right[1]);
            }
            else
            {
                _columnText.SetColumns(left, right);
            }

            var result = _columnText.Go();
            if ((result & ColumnText.NO_MORE_TEXT) != 0)
            {
                done = true;
                _top = _columnText.YLine;
            }
            else if (ShiftCurrentColumn())
            {
                _top = _nextY;
            }
            else
            {
                // check if we are done because of height
                _totalHeight += currentHeight;

                if (_desiredHeight.ApproxNotEqual(AUTOMATIC) && _totalHeight >= _desiredHeight)
                {
                    _overflow = true;
                    break;
                } // need to start new page and reset the columns

                documentY = _nextY;
                newPage();
                currentHeight = 0;
            }
        }

        if (_desiredHeight.ApproxEquals(AUTOMATIC) && _columnDefs.Count == 1)
        {
            currentHeight = documentY - _columnText.YLine;
        }

        return currentHeight;
    }

    private void newPage()
    {
        ResetCurrentColumn();
        if (_desiredHeight.ApproxEquals(AUTOMATIC))
        {
            _top = _nextY = AUTOMATIC;
        }
        else
        {
            _top = _nextY;
        }

        _totalHeight = 0;
        if (_document != null)
        {
            _document.NewPage();
        }
    }

    /// <summary>
    ///     Figure out the height of a column from the border extents
    /// </summary>
    /// <param name="left">left border</param>
    /// <param name="right">right border</param>
    /// <returns>height</returns>
    private static float getHeight(float[] left, float[] right)
    {
        var max = float.MinValue;
        var min = float.MaxValue;
        for (var i = 0; i < left.Length; i += 2)
        {
            min = Math.Min(min, left[i + 1]);
            max = Math.Max(max, left[i + 1]);
        }

        for (var i = 0; i < right.Length; i += 2)
        {
            min = Math.Min(min, right[i + 1]);
            max = Math.Max(max, right[i + 1]);
        }

        return max - min;
    }

    /// <summary>
    ///     Calculates the appropriate y position for the bottom
    ///     of the columns on this page.
    /// </summary>
    /// <returns>the y position of the bottom of the columns</returns>
    private float getColumnBottom()
    {
        if (_desiredHeight.ApproxEquals(AUTOMATIC))
        {
            return _document.Bottom;
        }

        return Math.Max(_top - (_desiredHeight - _totalHeight), _document.Bottom);
    }

    /// <summary>
    ///     Moves the text insertion point to the beginning of the next column, issuing a page break if
    ///     needed.
    ///     @throws DocumentException on error
    /// </summary>
    public void NextColumn()
    {
        _currentColumn = (_currentColumn + 1) % _columnDefs.Count;
        _top = _nextY;
        if (_currentColumn == 0)
        {
            newPage();
        }
    }

    /// <summary>
    ///     Resets the current column.
    /// </summary>
    public void ResetCurrentColumn()
    {
        _currentColumn = 0;
    }

    /// <summary>
    ///     Shifts the current column.
    /// </summary>
    /// <returns>true if the currentcolumn has changed</returns>
    public bool ShiftCurrentColumn()
    {
        if (_currentColumn + 1 < _columnDefs.Count)
        {
            _currentColumn++;
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Sets the direction of the columns.
    /// </summary>
    /// <param name="direction">true = right2left; false = left2right</param>
    public void SetColumnsRightToLeft(bool direction)
    {
        _columnsRightToLeft = direction;
    }

    /// <summary>
    ///     Inner class used to define a column
    /// </summary>
    internal class ColumnDef
    {
        private readonly float[] _left;
        private readonly MultiColumnText _mc;
        private readonly float[] _right;

        internal ColumnDef(float[] newLeft, float[] newRight, MultiColumnText mc)
        {
            _mc = mc;
            _left = newLeft;
            _right = newRight;
        }

        internal ColumnDef(float leftPosition, float rightPosition, MultiColumnText mc)
        {
            _mc = mc;
            _left = new float[4];
            _left[0] = leftPosition; // x1
            _left[1] = mc._top; // y1
            _left[2] = leftPosition; // x2
            if (mc._desiredHeight.ApproxEquals(AUTOMATIC) || mc._top.ApproxEquals(AUTOMATIC))
            {
                _left[3] = AUTOMATIC;
            }
            else
            {
                _left[3] = mc._top - mc._desiredHeight;
            }

            _right = new float[4];
            _right[0] = rightPosition; // x1
            _right[1] = mc._top; // y1
            _right[2] = rightPosition; // x2
            if (mc._desiredHeight.ApproxEquals(AUTOMATIC) || mc._top.ApproxEquals(AUTOMATIC))
            {
                _right[3] = AUTOMATIC;
            }
            else
            {
                _right[3] = mc._top - mc._desiredHeight;
            }
        }

        /// <summary>
        ///     Resolves the positions for the specified side of the column
        ///     into real numbers once the top of the column is known.
        ///     or  Rectangle.RIGHT_BORDER
        /// </summary>
        /// <param name="side">either  Rectangle.LEFT_BORDER </param>
        /// <returns>the array of floats for the side</returns>
        internal float[] ResolvePositions(int side)
        {
            if (side == Rectangle.LEFT_BORDER)
            {
                return ResolvePositions(_left);
            }

            return ResolvePositions(_right);
        }

        internal float[] ResolvePositions(float[] positions)
        {
            if (!IsSimple())
            {
                positions[1] = _mc._top;
                return positions;
            }

            if (_mc._top.ApproxEquals(AUTOMATIC))
            {
                // this is bad - must be programmer error
                throw new InvalidOperationException("resolvePositions called with top=AUTOMATIC (-1).  " +
                                                    "Top position must be set befure lines can be resolved");
            }

            positions[1] = _mc._top;
            positions[3] = _mc.getColumnBottom();
            return positions;
        }

        /// <summary>
        ///     Checks if column definition is a simple rectangle
        /// </summary>
        /// <returns>true if it is a simple column</returns>
        internal bool IsSimple() => _left.Length == 4 && _right.Length == 4 && _left[0].ApproxEquals(_left[2]) &&
                                    _right[0].ApproxEquals(_right[2]);
    }
}