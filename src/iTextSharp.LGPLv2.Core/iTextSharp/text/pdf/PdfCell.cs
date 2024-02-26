namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfCell  is the PDF translation of a  Cell .
///     A  PdfCell  is an  ArrayList  of  PdfLine s.
///     @see     iTextSharp.text.Rectangle
///     @see     iTextSharp.text.Cell
///     @see     PdfLine
///     @see     PdfTable
/// </summary>
public class PdfCell : Rectangle
{
    /// <summary>
    ///     These are the Images in the Cell.
    /// </summary>
    private readonly List<Image> _images;

    private readonly int _verticalAlignment;

    /// <summary>
    ///     This is the total height of the content of the cell.  Note that the actual cell
    ///     height may be larger due to another cell on the row *
    /// </summary>
    private float _contentHeight;

    private PdfLine _firstLine;

    private PdfLine _lastLine;

    /// <summary>
    ///     These are the PdfLines in the Cell.
    /// </summary>
    private PdfLine _line;

    /// <summary>
    ///     These are the PdfLines in the Cell.
    /// </summary>
    private List<PdfLine> _lines;

    /// <summary>
    ///     Indicates that the largest ascender height should be used to
    ///     determine the height of the first line. Setting this to true can help
    ///     with vertical alignment problems.
    /// </summary>
    private bool _useAscender;

    /// <summary>
    ///     Adjusts the cell contents to compensate for border widths.
    /// </summary>
    private bool _useBorderPadding;

    /// <summary>
    ///     Indicates that the largest descender height should be added to the height of
    ///     the last line (so characters like y don't dip into the border).
    /// </summary>
    private bool _useDescender;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfCell -object.
    /// </summary>
    /// <param name="cell">the original  Cell </param>
    /// <param name="rownumber">the number of the  Row  the  Cell  was in.</param>
    /// <param name="left">the left border of the  PdfCell </param>
    /// <param name="right">the right border of the  PdfCell </param>
    /// <param name="top">the top border of the  PdfCell </param>
    /// <param name="cellspacing">the cellspacing of the  Table </param>
    /// <param name="cellpadding">the cellpadding of the  Table </param>
    public PdfCell(Cell cell, int rownumber, float left, float right, float top, float cellspacing, float cellpadding) :
        base(left, top, right, top)
    {
        if (cell == null)
        {
            throw new ArgumentNullException(nameof(cell));
        }

        // copying the other Rectangle attributes from class Cell
        CloneNonPositionParameters(cell);
        Cellpadding = cellpadding;
        Cellspacing = cellspacing;
        _verticalAlignment = cell.VerticalAlignment;
        _useAscender = cell.UseAscender;
        _useDescender = cell.UseDescender;
        _useBorderPadding = cell.UseBorderPadding;

        // initialisation of some parameters
        PdfChunk chunk;
        PdfChunk overflow;
        _lines = new List<PdfLine>();
        _images = new List<Image>();
        Leading = cell.Leading;
        var alignment = cell.HorizontalAlignment;
        left += cellspacing + cellpadding;
        right -= cellspacing + cellpadding;

        left += getBorderWidthInside(LEFT_BORDER);
        right -= getBorderWidthInside(RIGHT_BORDER);
        _contentHeight = 0;
        Rowspan = cell.Rowspan;

        List<PdfAction> allActions;
        int aCounter;

        // we loop over all the elements of the cell
        foreach (var ele in cell.Elements)
        {
            switch (ele.Type)
            {
                case JPEG:
                case JPEG2000:
                case JBIG2:
                case IMGRAW:
                case IMGTEMPLATE:
                    addImage((Image)ele, left, right, 0.4f * Leading, alignment);

                    break;

                // if the element is a list
                case LIST:
                    if (_line != null && _line.Size > 0)
                    {
                        _line.ResetAlignment();
                        addLine(_line);
                    }

                    // we loop over all the listitems
                    addList((List)ele, left, right, alignment);
                    _line = new PdfLine(left, right, alignment, Leading);

                    break;

                // if the element is something else
                default:
                    allActions = new List<PdfAction>();
                    ProcessActions(ele, null, allActions);
                    aCounter = 0;

                    var currentLineLeading = Leading;
                    var currentLeft = left;
                    var currentRight = right;

                    if (ele is Phrase)
                    {
                        currentLineLeading = ((Phrase)ele).Leading;
                    }

                    if (ele is Paragraph)
                    {
                        var p = (Paragraph)ele;
                        currentLeft += p.IndentationLeft;
                        currentRight -= p.IndentationRight;
                    }

                    if (_line == null)
                    {
                        _line = new PdfLine(currentLeft, currentRight, alignment, currentLineLeading);
                    }

                    // we loop over the chunks
                    var chunks = ele.Chunks;

                    if (chunks.Count == 0)
                    {
                        addLine(_line); // add empty line - all cells need some lines even if they are empty
                        _line = new PdfLine(currentLeft, currentRight, alignment, currentLineLeading);
                    }
                    else
                    {
                        foreach (var c in chunks)
                        {
                            chunk = new PdfChunk(c, allActions[aCounter++]);

                            while ((overflow = _line.Add(chunk)) != null)
                            {
                                addLine(_line);
                                _line = new PdfLine(currentLeft, currentRight, alignment, currentLineLeading);
                                chunk = overflow;
                            }
                        }
                    }

                    // if the element is a paragraph, section or chapter, we reset the alignment and add the line
                    switch (ele.Type)
                    {
                        case PARAGRAPH:
                        case SECTION:
                        case CHAPTER:
                            _line.ResetAlignment();
                            flushCurrentLine();

                            break;
                    }

                    break;
            }
        }

        flushCurrentLine();

        if (_lines.Count > cell.MaxLines)
        {
            while (_lines.Count > cell.MaxLines)
            {
                removeLine(_lines.Count - 1);
            }

            if (cell.MaxLines > 0)
            {
                var more = cell.ShowTruncation;

                if (!string.IsNullOrEmpty(more))
                {
                    // Denote that the content has been truncated
                    _lastLine = _lines[_lines.Count - 1];

                    if (_lastLine.Size >= 0)
                    {
                        var lastChunk = _lastLine.GetChunk(_lastLine.Size - 1);
                        var moreWidth = new PdfChunk(more, lastChunk).Width;

                        while (lastChunk.ToString().Length > 0 && lastChunk.Width + moreWidth > right - left)
                        {
                            // Remove characters to leave room for the 'more' indicator
                            lastChunk.Value = lastChunk.ToString().Substring(0, lastChunk.Length - 1);
                        }

                        lastChunk.Value = lastChunk + more;
                    }
                    else
                    {
                        _lastLine.Add(new PdfChunk(new Chunk(more), null));
                    }
                }
            }
        }

        // we set some additional parameters
        if (_useDescender && _lastLine != null)
        {
            _contentHeight -= _lastLine.Descender;
        }

        // adjust first line height so that it touches the top
        if (_lines.Count > 0)
        {
            _firstLine = _lines[0];
            var firstLineRealHeight = FirstLineRealHeight;
            _contentHeight -= _firstLine.Height;
            _firstLine.height = firstLineRealHeight;
            _contentHeight += firstLineRealHeight;
        }

        var newBottom = top - _contentHeight - 2f * Cellpadding - 2f * Cellspacing;
        newBottom -= getBorderWidthInside(TOP_BORDER) + getBorderWidthInside(BOTTOM_BORDER);
        Bottom = newBottom;

        Rownumber = rownumber;
    }

    public override float Bottom
    {
        get => GetBottom(Cellspacing);
        set
        {
            base.Bottom = value;
            var firstLineRealHeight = FirstLineRealHeight;

            var totalHeight = Ury - value; // can't use top (already compensates for cellspacing)
            var nonContentHeight = Cellpadding * 2f + Cellspacing * 2f;
            nonContentHeight += getBorderWidthInside(TOP_BORDER) + getBorderWidthInside(BOTTOM_BORDER);

            var interiorHeight = totalHeight - nonContentHeight;
            var extraHeight = 0.0f;

            switch (_verticalAlignment)
            {
                case ALIGN_BOTTOM:
                    extraHeight = interiorHeight - _contentHeight;

                    break;
                case ALIGN_MIDDLE:
                    extraHeight = (interiorHeight - _contentHeight) / 2.0f;

                    break;
                default: // ALIGN_TOP
                    extraHeight = 0f;

                    break;
            }

            extraHeight += Cellpadding + Cellspacing;
            extraHeight += getBorderWidthInside(TOP_BORDER);

            if (_firstLine != null)
            {
                _firstLine.height = firstLineRealHeight + extraHeight;
            }
        }
    }

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     This is the cellpadding of the cell.
    /// </summary>
    public float Cellpadding { get; }

    /// <summary>
    ///     This is the cellspacing of the cell.
    /// </summary>
    public float Cellspacing { get; }

    /// <summary>
    ///     This is the number of the group the cell is in.
    /// </summary>
    public int GroupNumber { get; set; }

    /// <summary>
    ///     This is the leading of the lines.
    /// </summary>
    public float Leading { get; }

    public override float Left => GetLeft(Cellspacing);

    public float RemainingHeight
    {
        get
        {
            var result = 0f;

            foreach (var image in _images)
            {
                result += image.ScaledHeight;
            }

            return remainingLinesHeight() + Cellspacing + 2 * Cellpadding + result;
        }
    }

    public override float Right => GetRight(Cellspacing);

    /// <summary>
    ///     This is the number of the row the cell is in.
    /// </summary>
    public int Rownumber { get; }

    /// <summary>
    ///     This is the rowspan of the cell.
    /// </summary>
    public int Rowspan { get; }

    public int Size => _lines.Count;

    public override float Top => GetTop(Cellspacing);

    /// <summary>
    ///     Gets the value of {@link #useAscender}
    /// </summary>
    /// <returns>useAscender</returns>
    public bool UseAscender
    {
        get => _useAscender;
        set => _useAscender = value;
    }

    /// <summary>
    ///     Sets the value of {@link #useBorderPadding}.
    /// </summary>
    public bool UseBorderPadding
    {
        set => _useBorderPadding = value;
        get => _useBorderPadding;
    }

    /// <summary>
    ///     Gets the value of {@link #useDescender}
    /// </summary>
    /// <returns>useDescender</returns>
    public bool UseDescender
    {
        get => _useDescender;
        set => _useDescender = value;
    }

    /// <summary>
    ///     Indicates if this cell belongs to the header of a  PdfTable
    /// </summary>
    internal bool Header { get; private set; }

    /// <summary>
    ///     Calculates what the height of the first line should be so that the content will be
    ///     flush with the top.  For text, this is the height of the ascender.  For an image,
    ///     it is the actual height of the image.
    /// </summary>
    /// <returns>the real height of the first line</returns>
    private float FirstLineRealHeight
    {
        get
        {
            var firstLineRealHeight = 0f;

            if (_firstLine != null)
            {
                var chunk = _firstLine.GetChunk(0);

                if (chunk != null)
                {
                    var image = chunk.Image;

                    if (image != null)
                    {
                        firstLineRealHeight = _firstLine.GetChunk(0).Image.ScaledHeight;
                    }
                    else
                    {
                        firstLineRealHeight = _useAscender ? _firstLine.Ascender : Leading;
                    }
                }
            }

            return firstLineRealHeight;
        }
    }

    public IList<Image> GetImages(float top, float bottom)
    {
        // if the bottom of the page is higher than the top of the cell: do nothing
        if (Top < bottom)
        {
            return new List<Image>();
        }

        top = Math.Min(Top, top);

        // initialisations
        float height;
        List<Image> result = new();

        // we loop over the images
        List<Image> remove = new();

        foreach (var image in _images)
        {
            height = image.AbsoluteY;

            // if the currentPosition is higher than the bottom, we add the line to the result
            if (top - height > bottom + Cellpadding)
            {
                image.SetAbsolutePosition(image.AbsoluteX, top - height);
                result.Add(image);
                remove.Add(image);
            }
        }

        foreach (var image in remove)
        {
            _images.Remove(image);
        }

        return result;
    }

    public IList<PdfLine> GetLines(float top, float bottom)
    {
        float lineHeight;
        var currentPosition = Math.Min(Top, top);
        Top = currentPosition + Cellspacing;
        List<PdfLine> result = new();

        // if the bottom of the page is higher than the top of the cell: do nothing
        if (Top < bottom)
        {
            return result;
        }

        // we loop over the lines
        var size = _lines.Count;
        var aboveBottom = true;

        for (var i = 0; i < size && aboveBottom; i++)
        {
            _line = _lines[i];
            lineHeight = _line.Height;
            currentPosition -= lineHeight;

            // if the currentPosition is higher than the bottom, we add the line to the result
            if (currentPosition > bottom + Cellpadding + getBorderWidthInside(BOTTOM_BORDER))
            {
                // bugfix by Tom Ring and Veerendra Namineni
                result.Add(_line);
            }
            else
            {
                aboveBottom = false;
            }
        }

        // if the bottom of the cell is higher than the bottom of the page, the cell is written, so we can remove all lines
        var difference = 0f;

        if (!Header)
        {
            if (aboveBottom)
            {
                _lines = new List<PdfLine>();
                _contentHeight = 0f;
            }
            else
            {
                size = result.Count;

                for (var i = 0; i < size; i++)
                {
                    _line = removeLine(0);
                    difference += _line.Height;
                }
            }
        }

        if (difference > 0)
        {
            foreach (var image in _images)
            {
                image.SetAbsolutePosition(image.AbsoluteX, image.AbsoluteY - difference - Leading);
            }
        }

        return result;
    }

    public Rectangle Rectangle(float top, float bottom)
    {
        var tmp = new Rectangle(Left, Bottom, Right, Top);
        tmp.CloneNonPositionParameters(this);

        if (Top > top)
        {
            tmp.Top = top;
            tmp.Border = border - (border & TOP_BORDER);
        }

        if (Bottom < bottom)
        {
            tmp.Bottom = bottom;
            tmp.Border = border - (border & BOTTOM_BORDER);
        }

        return tmp;
    }

    internal bool MayBeRemoved()
        => Header || (_lines.Count == 0 && _images.Count == 0);

    internal void SetHeader()
        => Header = true;

    protected static void ProcessActions(IElement element, PdfAction action, IList<PdfAction> allActions)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (allActions == null)
        {
            throw new ArgumentNullException(nameof(allActions));
        }

        if (element.Type == ANCHOR)
        {
            var url = ((Anchor)element).Reference;

            if (url != null)
            {
                action = new PdfAction(url);
            }
        }

        switch (element.Type)
        {
            case PHRASE:
            case SECTION:
            case ANCHOR:
            case CHAPTER:
            case LISTITEM:
            case PARAGRAPH:
                foreach (var ele in (ICollection<IElement>)element)
                {
                    ProcessActions(ele, action, allActions);
                }

                break;
            case CHUNK:
                allActions.Add(action);

                break;
            case LIST:
                foreach (var ele in ((List)element).Items)
                {
                    ProcessActions(ele, action, allActions);
                }

                break;
            default:
                var n = element.Chunks.Count;

                while (n-- > 0)
                {
                    allActions.Add(action);
                }

                break;
        }
    }

    private float addImage(Image i, float left, float right, float extraHeight, int alignment)
    {
        var image = Image.GetInstance(i);

        if (image.ScaledWidth > right - left)
        {
            image.ScaleToFit(right - left, float.MaxValue);
        }

        flushCurrentLine();

        if (_line == null)
        {
            _line = new PdfLine(left, right, alignment, Leading);
        }

        var imageLine = _line;

        // left and right in chunk is relative to the start of the line
        right = right - left;
        left = 0f;

        if ((image.Alignment & Image.RIGHT_ALIGN) == Image.RIGHT_ALIGN)
        {
            // fix Uwe Zimmerman
            left = right - image.ScaledWidth;
        }
        else if ((image.Alignment & Image.MIDDLE_ALIGN) == Image.MIDDLE_ALIGN)
        {
            left = left + (right - left - image.ScaledWidth) / 2f;
        }

        var imageChunk = new Chunk(image, left, 0);
        imageLine.Add(new PdfChunk(imageChunk, null));
        addLine(imageLine);

        return imageLine.Height;
    }

    private void addLine(PdfLine line)
    {
        _lines.Add(line);
        _contentHeight += line.Height;
        _lastLine = line;
        _line = null;
    }

    private void addList(List list, float left, float right, int alignment)
    {
        PdfChunk chunk;
        PdfChunk overflow;
        List<PdfAction> allActions = new();
        ProcessActions(list, null, allActions);
        var aCounter = 0;

        foreach (var ele in list.Items)
        {
            switch (ele.Type)
            {
                case LISTITEM:
                    var item = (ListItem)ele;
                    _line = new PdfLine(left + item.IndentationLeft, right, alignment, item.Leading);
                    _line.ListItem = item;

                    foreach (var c in item.Chunks)
                    {
                        chunk = new PdfChunk(c, allActions[aCounter++]);

                        while ((overflow = _line.Add(chunk)) != null)
                        {
                            addLine(_line);
                            _line = new PdfLine(left + item.IndentationLeft, right, alignment, item.Leading);
                            chunk = overflow;
                        }

                        _line.ResetAlignment();
                        addLine(_line);
                        _line = new PdfLine(left + item.IndentationLeft, right, alignment, Leading);
                    }

                    break;
                case LIST:
                    var sublist = (List)ele;
                    addList(sublist, left + sublist.IndentationLeft, right, alignment);

                    break;
            }
        }
    }

    /// <summary>
    ///     overriding of the Rectangle methods
    /// </summary>
    private void flushCurrentLine()
    {
        if (_line != null && _line.Size > 0)
        {
            addLine(_line);
        }
    }

    /// <summary>
    ///     Gets the amount of the border for the specified side that is inside the Rectangle.
    ///     For non-variable width borders this is only 1/2 the border width on that side.  This
    ///     always returns 0 if {@link #useBorderPadding} is false;
    /// </summary>
    /// <param name="side">the side to check. One of the side constants in {@link com.lowagie.text.Rectangle}</param>
    /// <returns>the borderwidth inside the cell</returns>
    private float getBorderWidthInside(int side)
    {
        var width = 0f;

        if (_useBorderPadding)
        {
            switch (side)
            {
                case LEFT_BORDER:
                    width = BorderWidthLeft;

                    break;

                case RIGHT_BORDER:
                    width = BorderWidthRight;

                    break;

                case TOP_BORDER:
                    width = BorderWidthTop;

                    break;

                default: // default and BOTTOM
                    width = BorderWidthBottom;

                    break;
            }

            // non-variable (original style) borders overlap the rectangle (only 1/2 counts)
            if (!UseVariableBorders)
            {
                width = width / 2f;
            }
        }

        return width;
    }

    private float remainingLinesHeight()
    {
        if (_lines.Count == 0)
        {
            return 0;
        }

        float result = 0;
        var size = _lines.Count;
        PdfLine line;

        for (var i = 0; i < size; i++)
        {
            line = _lines[i];
            result += line.Height;
        }

        return result;
    }

    /// <summary>
    ///     Returns the lower left x-coordinaat.
    /// </summary>
    /// <returns>the lower left x-coordinaat</returns>
    /// <summary>
    ///     Returns the upper right x-coordinate.
    /// </summary>
    /// <returns>the upper right x-coordinate</returns>
    /// <summary>
    ///     Returns the upper right y-coordinate.
    /// </summary>
    /// <returns>the upper right y-coordinate</returns>
    /// <summary>
    ///     Returns the lower left y-coordinate.
    /// </summary>
    /// <returns>the lower left y-coordinate</returns>
    /// <summary>
    ///     methods
    /// </summary>
    private PdfLine removeLine(int index)
    {
        var oldLine = _lines[index];
        _lines.RemoveAt(index);
        _contentHeight -= oldLine.Height;

        if (index == 0)
        {
            if (_lines.Count > 0)
            {
                _firstLine = _lines[0];
                var firstLineRealHeight = FirstLineRealHeight;
                _contentHeight -= _firstLine.Height;
                _firstLine.height = firstLineRealHeight;
                _contentHeight += firstLineRealHeight;
            }
        }

        return oldLine;
    }
}