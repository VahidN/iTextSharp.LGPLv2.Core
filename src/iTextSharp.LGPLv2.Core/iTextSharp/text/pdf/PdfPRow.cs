using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     A row in a PdfPTable.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfPRow
{
    /// <summary>
    ///     the bottom limit (bottom right y)
    /// </summary>
    public const float BOTTOM_LIMIT = -(1 << 30);

    /// <summary>
    ///     the right limit
    ///     @since    2.1.5
    /// </summary>
    public const float RIGHT_LIMIT = 20000;

    private int[] _canvasesPos;

    protected bool Calculated;
    protected PdfPCell[] Cells;

    /// <summary>
    ///     extra heights that needs to be added to a cell because of rowspans.
    ///     @since    2.1.6
    /// </summary>
    protected float[] ExtraHeights;

    protected float MaxHeight;
    protected float[] Widths;

    /// <summary>
    ///     Constructs a new PdfPRow with the cells in the array that was passed
    ///     as a parameter.
    /// </summary>
    /// <param name="cells"></param>
    public PdfPRow(PdfPCell[] cells)
    {
        Cells = cells ?? throw new ArgumentNullException(nameof(cells));
        Widths = new float[cells.Length];
        InitExtraHeights();
    }

    /// <summary>
    ///     Makes a copy of an existing row.
    /// </summary>
    /// <param name="row"></param>
    public PdfPRow(PdfPRow row)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row));
        }

        MaxHeight = row.MaxHeight;
        Calculated = row.Calculated;
        Cells = new PdfPCell[row.Cells.Length];
        for (var k = 0; k < Cells.Length; ++k)
        {
            if (row.Cells[k] != null)
            {
                Cells[k] = new PdfPCell(row.Cells[k]);
            }
        }

        Widths = new float[Cells.Length];
        Array.Copy(row.Widths, 0, Widths, 0, Cells.Length);
        InitExtraHeights();
    }

    /// <summary>
    ///     Gets the maximum height of the row (i.e. of the 'highest' cell).
    /// </summary>
    /// <returns>the maximum height of the row</returns>
    public float MaxHeights
    {
        get
        {
            if (Calculated)
            {
                return MaxHeight;
            }

            return CalculateHeights();
        }
        set => MaxHeight = value;
    }

    /// <summary>
    ///     @since	3.0.0 protected is now public static
    /// </summary>
    public static float SetColumn(ColumnText ct, float left, float bottom, float right, float top)
    {
        if (ct == null)
        {
            throw new ArgumentNullException(nameof(ct));
        }

        if (left > right)
        {
            right = left;
        }

        if (bottom > top)
        {
            top = bottom;
        }

        ct.SetSimpleColumn(left, bottom, right, top);
        return top;
    }

    /// <summary>
    ///     Calculates the heights of each cell in the row.
    /// </summary>
    /// <returns>the maximum height of the row.</returns>
    public float CalculateHeights()
    {
        MaxHeight = 0;
        for (var k = 0; k < Cells.Length; ++k)
        {
            var cell = Cells[k];
            float height = 0;
            if (cell == null)
            {
                continue;
            }

            height = cell.GetMaxHeight();
            if (height > MaxHeight && cell.Rowspan == 1)
            {
                MaxHeight = height;
            }
        }

        Calculated = true;
        return MaxHeight;
    }

    /// <summary>
    ///     Returns the array of cells in the row.
    ///     Please be extremely careful with this method.
    ///     Use the cells as read only objects.
    ///     @since    2.1.1
    /// </summary>
    /// <returns>an array of cells</returns>
    public PdfPCell[] GetCells() => Cells;

    /// <summary>
    ///     Initializes the extra heights array.
    ///     @since    2.1.6
    /// </summary>
    public void InitExtraHeights()
    {
        ExtraHeights = new float[Cells.Length];
        for (var i = 0; i < ExtraHeights.Length; i++)
        {
            ExtraHeights[i] = 0;
        }
    }

    /// <summary>
    ///     Checks if the dimensions of the columns were calculated.
    /// </summary>
    /// <returns>true if the dimensions of the columns were calculated</returns>
    public bool IsCalculated() => Calculated;

    /// <summary>
    ///     Sets an extra height for a cell.
    ///     @since    2.1.6
    /// </summary>
    /// <param name="cell">the index of the cell that needs an extra height</param>
    /// <param name="height">the extra height</param>
    public void SetExtraHeight(int cell, float height)
    {
        if (cell < 0 || cell >= Cells.Length)
        {
            return;
        }

        ExtraHeights[cell] = height;
    }

    /// <summary>
    ///     Sets the widths of the columns in the row.
    /// </summary>
    /// <param name="widths"></param>
    /// <returns>true if everything went right</returns>
    public bool SetWidths(float[] widths)
    {
        if (widths == null)
        {
            throw new ArgumentNullException(nameof(widths));
        }

        if (widths.Length != Cells.Length)
        {
            return false;
        }

        Array.Copy(widths, 0, Widths, 0, Cells.Length);
        float total = 0;
        Calculated = false;
        for (var k = 0; k < widths.Length; ++k)
        {
            var cell = Cells[k];

            if (cell == null)
            {
                total += widths[k];
                continue;
            }

            cell.Left = total;
            var last = k + cell.Colspan;
            for (; k < last; ++k)
            {
                total += widths[k];
            }

            --k;
            cell.Right = total;
            cell.Top = 0;
        }

        return true;
    }

    /// <summary>
    ///     Splits a row to newHeight.
    ///     The returned row is the remainder. It will return null if the newHeight
    ///     was so small that only an empty row would result.
    ///     an empty row would result
    /// </summary>
    /// <returns>the remainder row or null if the newHeight was so small that only</returns>
    public PdfPRow SplitRow(PdfPTable table, int rowIndex, float new_height)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        var newCells = new PdfPCell[Cells.Length];
        var fixHs = new float[Cells.Length];
        var minHs = new float[Cells.Length];
        var allEmpty = true;
        for (var k = 0; k < Cells.Length; ++k)
        {
            var newHeight = new_height;
            var cell = Cells[k];
            if (cell == null)
            {
                var index = rowIndex;
                if (table.RowSpanAbove(index, k))
                {
                    newHeight += table.GetRowHeight(index);
                    while (table.RowSpanAbove(--index, k))
                    {
                        newHeight += table.GetRowHeight(index);
                    }

                    var row = table.GetRow(index);
                    if (row != null && row.GetCells()[k] != null)
                    {
                        newCells[k] = new PdfPCell(row.GetCells()[k]);
                        newCells[k].ConsumeHeight(newHeight);
                        newCells[k].Rowspan = row.GetCells()[k].Rowspan - rowIndex + index;
                        allEmpty = false;
                    }
                }

                continue;
            }

            fixHs[k] = cell.FixedHeight;
            minHs[k] = cell.MinimumHeight;
            var img = cell.Image;
            var newCell = new PdfPCell(cell);
            if (img != null)
            {
                if (newHeight > cell.EffectivePaddingBottom + cell.EffectivePaddingTop + 2)
                {
                    newCell.Phrase = null;
                    allEmpty = false;
                }
            }
            else
            {
                float y;
                var ct = ColumnText.Duplicate(cell.Column);
                var left = cell.Left + cell.EffectivePaddingLeft;
                var bottom = cell.Top + cell.EffectivePaddingBottom - newHeight;
                var right = cell.Right - cell.EffectivePaddingRight;
                var top = cell.Top - cell.EffectivePaddingTop;
                switch (cell.Rotation)
                {
                    case 90:
                    case 270:
                        y = SetColumn(ct, bottom, left, top, right);
                        break;
                    default:
                        y = SetColumn(ct, left, bottom, cell.NoWrap ? RIGHT_LIMIT : right, top);
                        break;
                }

                int status;
                status = ct.Go(true);
                var thisEmpty = ct.YLine.ApproxEquals(y);
                if (thisEmpty)
                {
                    newCell.Column = ColumnText.Duplicate(cell.Column);
                    ct.FilledWidth = 0;
                }
                else if ((status & ColumnText.NO_MORE_TEXT) == 0)
                {
                    newCell.Column = ct;
                    ct.FilledWidth = 0;
                }
                else
                {
                    newCell.Phrase = null;
                }

                allEmpty = allEmpty && thisEmpty;
            }

            newCells[k] = newCell;
            cell.FixedHeight = newHeight;
        }

        if (allEmpty)
        {
            for (var k = 0; k < Cells.Length; ++k)
            {
                var cell = Cells[k];
                if (cell == null)
                {
                    continue;
                }

                if (fixHs[k] > 0)
                {
                    cell.FixedHeight = fixHs[k];
                }
                else
                {
                    cell.MinimumHeight = minHs[k];
                }
            }

            return null;
        }

        CalculateHeights();
        var split = new PdfPRow(newCells);
        split.Widths = (float[])Widths.Clone();
        split.CalculateHeights();
        return split;
    }

    /// <summary>
    ///     Writes the border and background of one cell in the row.
    ///     @since    2.1.6   extra parameter currentMaxHeight
    /// </summary>
    /// <param name="xPos">The x-coordinate where the table starts on the canvas</param>
    /// <param name="yPos">The y-coordinate where the table starts on the canvas</param>
    /// <param name="currentMaxHeight">The height of the cell to be drawn.</param>
    /// <param name="cell"></param>
    /// <param name="canvases"></param>
    public static void WriteBorderAndBackground(float xPos, float yPos, float currentMaxHeight, PdfPCell cell,
                                                PdfContentByte[] canvases)
    {
        if (cell == null)
        {
            throw new ArgumentNullException(nameof(cell));
        }

        if (canvases == null)
        {
            throw new ArgumentNullException(nameof(canvases));
        }

        var background = cell.BackgroundColor;
        if (background != null || cell.HasBorders())
        {
            // Add xPos resp. yPos to the cell's coordinates for absolute coordinates
            var right = cell.Right + xPos;
            var top = cell.Top + yPos;
            var left = cell.Left + xPos;
            var bottom = top - currentMaxHeight;

            if (background != null)
            {
                var backgr = canvases[PdfPTable.BACKGROUNDCANVAS];
                backgr.SetColorFill(background);
                backgr.Rectangle(left, bottom, right - left, top - bottom);
                backgr.Fill();
            }

            if (cell.HasBorders())
            {
                var newRect = new Rectangle(left, bottom, right, top);
                // Clone non-position parameters except for the background color
                newRect.CloneNonPositionParameters(cell);
                newRect.BackgroundColor = null;
                // Write the borders on the line canvas
                var lineCanvas = canvases[PdfPTable.LINECANVAS];
                lineCanvas.Rectangle(newRect);
            }
        }
    }

    /// <summary>
    ///     Writes a number of cells (not necessarily all cells).
    ///     Remember that the column index starts with 0.
    ///     Remember that the column index starts with 0.
    ///     If -1, all the columns to the end are written.
    /// </summary>
    /// <param name="colStart">The first column to be written.</param>
    /// <param name="colEnd">The last column to be written.</param>
    /// <param name="xPos">The x-coordinate where the table starts on the canvas</param>
    /// <param name="yPos">The y-coordinate where the table starts on the canvas</param>
    /// <param name="canvases"></param>
    public void WriteCells(int colStart, int colEnd, float xPos, float yPos, PdfContentByte[] canvases)
    {
        if (!Calculated)
        {
            CalculateHeights();
        }

        if (colEnd < 0)
        {
            colEnd = Cells.Length;
        }
        else
        {
            colEnd = Math.Min(colEnd, Cells.Length);
        }

        if (colStart < 0)
        {
            colStart = 0;
        }

        if (colStart >= colEnd)
        {
            return;
        }

        int newStart;
        for (newStart = colStart; newStart >= 0; --newStart)
        {
            if (Cells[newStart] != null)
            {
                break;
            }

            if (newStart > 0)
            {
                xPos -= Widths[newStart - 1];
            }
        }

        if (newStart < 0)
        {
            newStart = 0;
        }

        if (Cells[newStart] != null)
        {
            xPos -= Cells[newStart].Left;
        }

        for (var k = newStart; k < colEnd; ++k)
        {
            var cell = Cells[k];
            if (cell == null)
            {
                continue;
            }

            var currentMaxHeight = MaxHeight + ExtraHeights[k];

            WriteBorderAndBackground(xPos, yPos, currentMaxHeight, cell, canvases);

            var img = cell.Image;

            var tly = cell.Top + yPos - cell.EffectivePaddingTop;
            if (cell.Height <= currentMaxHeight)
            {
                switch (cell.VerticalAlignment)
                {
                    case Element.ALIGN_BOTTOM:
                        tly = cell.Top + yPos - currentMaxHeight + cell.Height
                              - cell.EffectivePaddingTop;
                        break;
                    case Element.ALIGN_MIDDLE:
                        tly = cell.Top + yPos + (cell.Height - currentMaxHeight) / 2
                              - cell.EffectivePaddingTop;
                        break;
                }
            }

            if (img != null)
            {
                if (cell.Rotation != 0)
                {
                    img = Image.GetInstance(img);
                    img.Rotation = img.GetImageRotation() + (float)(cell.Rotation * Math.PI / 180.0);
                }

                var vf = false;
                if (cell.Height > currentMaxHeight)
                {
                    img.ScalePercent(100);
                    var scale = (currentMaxHeight - cell.EffectivePaddingTop - cell
                                     .EffectivePaddingBottom)
                                / img.ScaledHeight;
                    img.ScalePercent(scale * 100);
                    vf = true;
                }

                var left = cell.Left + xPos
                                     + cell.EffectivePaddingLeft;
                if (vf)
                {
                    switch (cell.HorizontalAlignment)
                    {
                        case Element.ALIGN_CENTER:
                            left = xPos
                                   + (cell.Left + cell.EffectivePaddingLeft
                                                + cell.Right
                                      - cell.EffectivePaddingRight - img
                                          .ScaledWidth) / 2;
                            break;
                        case Element.ALIGN_RIGHT:
                            left = xPos + cell.Right
                                   - cell.EffectivePaddingRight
                                   - img.ScaledWidth;
                            break;
                    }

                    tly = cell.Top + yPos - cell.EffectivePaddingTop;
                }

                img.SetAbsolutePosition(left, tly - img.ScaledHeight);
                canvases[PdfPTable.TEXTCANVAS].AddImage(img);
            }
            else
            {
                // rotation sponsored by Connection GmbH
                if (cell.Rotation == 90 || cell.Rotation == 270)
                {
                    var netWidth = currentMaxHeight - cell.EffectivePaddingTop - cell.EffectivePaddingBottom;
                    var netHeight = cell.Width - cell.EffectivePaddingLeft - cell.EffectivePaddingRight;
                    var ct = ColumnText.Duplicate(cell.Column);
                    ct.Canvases = canvases;
                    ct.SetSimpleColumn(0, 0, netWidth + 0.001f, -netHeight);
                    ct.Go(true);
                    var calcHeight = -ct.YLine;
                    if (netWidth <= 0 || netHeight <= 0)
                    {
                        calcHeight = 0;
                    }

                    if (calcHeight > 0)
                    {
                        if (cell.UseDescender)
                        {
                            calcHeight -= ct.Descender;
                        }

                        ct = ColumnText.Duplicate(cell.Column);
                        ct.Canvases = canvases;
                        ct.SetSimpleColumn(-0.003f, -0.001f, netWidth + 0.003f, calcHeight);
                        float pivotX;
                        float pivotY;
                        if (cell.Rotation == 90)
                        {
                            pivotY = cell.Top + yPos - currentMaxHeight + cell.EffectivePaddingBottom;
                            switch (cell.VerticalAlignment)
                            {
                                case Element.ALIGN_BOTTOM:
                                    pivotX = cell.Left + xPos + cell.Width - cell.EffectivePaddingRight;
                                    break;
                                case Element.ALIGN_MIDDLE:
                                    pivotX = cell.Left + xPos +
                                             (cell.Width + cell.EffectivePaddingLeft - cell.EffectivePaddingRight +
                                              calcHeight) / 2;
                                    break;
                                default: //top
                                    pivotX = cell.Left + xPos + cell.EffectivePaddingLeft + calcHeight;
                                    break;
                            }

                            SaveAndRotateCanvases(canvases, 0, 1, -1, 0, pivotX, pivotY);
                        }
                        else
                        {
                            pivotY = cell.Top + yPos - cell.EffectivePaddingTop;
                            switch (cell.VerticalAlignment)
                            {
                                case Element.ALIGN_BOTTOM:
                                    pivotX = cell.Left + xPos + cell.EffectivePaddingLeft;
                                    break;
                                case Element.ALIGN_MIDDLE:
                                    pivotX = cell.Left + xPos +
                                             (cell.Width + cell.EffectivePaddingLeft - cell.EffectivePaddingRight -
                                              calcHeight) / 2;
                                    break;
                                default: //top
                                    pivotX = cell.Left + xPos + cell.Width - cell.EffectivePaddingRight - calcHeight;
                                    break;
                            }

                            SaveAndRotateCanvases(canvases, 0, -1, 1, 0, pivotX, pivotY);
                        }

                        try
                        {
                            ct.Go();
                        }
                        finally
                        {
                            RestoreCanvases(canvases);
                        }
                    }
                }
                else
                {
                    var fixedHeight = cell.FixedHeight;
                    var rightLimit = cell.Right + xPos
                                     - cell.EffectivePaddingRight;
                    var leftLimit = cell.Left + xPos
                                              + cell.EffectivePaddingLeft;
                    if (cell.NoWrap)
                    {
                        switch (cell.HorizontalAlignment)
                        {
                            case Element.ALIGN_CENTER:
                                rightLimit += 10000;
                                leftLimit -= 10000;
                                break;
                            case Element.ALIGN_RIGHT:
                                if (cell.Rotation == 180)
                                {
                                    rightLimit += RIGHT_LIMIT;
                                }
                                else
                                {
                                    leftLimit -= RIGHT_LIMIT;
                                }

                                break;
                            default:
                                if (cell.Rotation == 180)
                                {
                                    leftLimit -= RIGHT_LIMIT;
                                }
                                else
                                {
                                    rightLimit += RIGHT_LIMIT;
                                }

                                break;
                        }
                    }

                    var ct = ColumnText.Duplicate(cell.Column);
                    ct.Canvases = canvases;
                    var bry = tly
                              - (currentMaxHeight
                                 - cell.EffectivePaddingTop - cell.EffectivePaddingBottom);
                    if (fixedHeight > 0)
                    {
                        if (cell.Height > currentMaxHeight)
                        {
                            tly = cell.Top + yPos - cell.EffectivePaddingTop;
                            bry = cell.Top + yPos - currentMaxHeight + cell.EffectivePaddingBottom;
                        }
                    }

                    if ((tly > bry || ct.ZeroHeightElement()) && leftLimit < rightLimit)
                    {
                        ct.SetSimpleColumn(leftLimit, bry - 0.001f, rightLimit, tly);
                        if (cell.Rotation == 180)
                        {
                            var shx = leftLimit + rightLimit;
                            var shy = yPos + yPos - currentMaxHeight + cell.EffectivePaddingBottom -
                                      cell.EffectivePaddingTop;
                            SaveAndRotateCanvases(canvases, -1, 0, 0, -1, shx, shy);
                        }

                        try
                        {
                            ct.Go();
                        }
                        finally
                        {
                            if (cell.Rotation == 180)
                            {
                                RestoreCanvases(canvases);
                            }
                        }
                    }
                }
            }

            var evt = cell.CellEvent;
            if (evt != null)
            {
                var rect = new Rectangle(cell.Left + xPos, cell.Top
                                                           + yPos - currentMaxHeight, cell.Right + xPos, cell.Top
                                             + yPos);
                evt.CellLayout(cell, rect, canvases);
            }
        }
    }

    internal float[] GetEventWidth(float xPos)
    {
        var n = 0;
        for (var k = 0; k < Cells.Length; ++k)
        {
            if (Cells[k] != null)
            {
                ++n;
            }
        }

        var width = new float[n + 1];
        n = 0;
        width[n++] = xPos;
        for (var k = 0; k < Cells.Length; ++k)
        {
            if (Cells[k] != null)
            {
                width[n] = width[n - 1] + Cells[k].Width;
                ++n;
            }
        }

        return width;
    }

    /// <summary>
    ///     @since    2.1.6 private is now protected
    /// </summary>
    protected void RestoreCanvases(PdfContentByte[] canvases)
    {
        if (canvases == null)
        {
            throw new ArgumentNullException(nameof(canvases));
        }

        var last = PdfPTable.TEXTCANVAS + 1;
        for (var k = 0; k < last; ++k)
        {
            var bb = canvases[k].InternalBuffer;
            var p1 = bb.Size;
            canvases[k].RestoreState();
            if (p1 == _canvasesPos[k * 2 + 1])
            {
                bb.Size = _canvasesPos[k * 2];
            }
        }
    }

    /// <summary>
    ///     @since    2.1.6 private is now protected
    /// </summary>
    protected void SaveAndRotateCanvases(PdfContentByte[] canvases, float a, float b, float c, float d, float e,
                                         float f)
    {
        if (canvases == null)
        {
            throw new ArgumentNullException(nameof(canvases));
        }

        var last = PdfPTable.TEXTCANVAS + 1;
        if (_canvasesPos == null)
        {
            _canvasesPos = new int[last * 2];
        }

        for (var k = 0; k < last; ++k)
        {
            var bb = canvases[k].InternalBuffer;
            _canvasesPos[k * 2] = bb.Size;
            canvases[k].SaveState();
            canvases[k].ConcatCtm(a, b, c, d, e, f);
            _canvasesPos[k * 2 + 1] = bb.Size;
        }
    }
}