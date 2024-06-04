using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     Summary description for SimpleTable.
/// </summary>
public class SimpleTable : Rectangle, IPdfPTableEvent, ITextElementArray
{
    /// <summary>
    ///     the content of a Table.
    /// </summary>
    private readonly List<IElement> _content = new();

    /// <summary>
    ///     the alignment of the table.
    /// </summary>
    private int _alignment;

    /// <summary>
    ///     the padding of the Cells.
    /// </summary>
    private float _cellpadding;

    /// <summary>
    ///     the spacing of the Cells.
    /// </summary>
    private float _cellspacing;

    /// <summary>
    ///     the width of the Table.
    /// </summary>
    private float _width;

    /// <summary>
    ///     the widthpercentage of the Table.
    /// </summary>
    private float _widthpercentage;

    /// <summary>
    ///     A RectangleCell is always constructed without any dimensions.
    ///     Dimensions are defined after creation.
    /// </summary>
    public SimpleTable() : base(0f, 0f, 0f, 0f)
    {
        Border = BOX;
        BorderWidth = 2f;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the alignment.</returns>
    public int Alignment
    {
        get => _alignment;
        set => _alignment = value;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the cellpadding.</returns>
    public float Cellpadding
    {
        get => _cellpadding;
        set => _cellpadding = value;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the cellspacing.</returns>
    public float Cellspacing
    {
        get => _cellspacing;
        set => _cellspacing = value;
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns the width.</returns>
    public override float Width
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
    ///     @see com.lowagie.text.pdf.PdfPTableEvent#tableLayout(com.lowagie.text.pdf.PdfPTable, float[][], float[], int, int,
    ///     com.lowagie.text.pdf.PdfContentByte[])
    /// </summary>
    public void TableLayout(PdfPTable table, float[][] widths, float[] heights, int headerRows, int rowStart,
                            PdfContentByte[] canvases)
    {
        if (widths == null)
        {
            throw new ArgumentNullException(nameof(widths));
        }

        if (heights == null)
        {
            throw new ArgumentNullException(nameof(heights));
        }

        if (canvases == null)
        {
            throw new ArgumentNullException(nameof(canvases));
        }

        var width = widths[0];
        var rect = new Rectangle(width[0], heights[heights.Length - 1], width[width.Length - 1], heights[0]);
        rect.CloneNonPositionParameters(this);
        var bd = rect.Border;
        rect.Border = NO_BORDER;
        canvases[PdfPTable.BACKGROUNDCANVAS].Rectangle(rect);
        rect.Border = bd;
        rect.BackgroundColor = null;
        canvases[PdfPTable.LINECANVAS].Rectangle(rect);
    }

    /// <summary>
    ///     @see com.lowagie.text.Element#type()
    /// </summary>
    public override int Type => TABLE;

    /// <summary>
    ///     @see com.lowagie.text.TextElementArray#add(java.lang.Object)
    /// </summary>
    public bool Add(IElement o)
    {
        try
        {
            AddElement((SimpleCell)o);
            return true;
        }
        catch (InvalidCastException)
        {
            return false;
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public override bool IsNestable() => true;

    /// <summary>
    ///     Adds content to this object.
    ///     @throws BadElementException
    /// </summary>
    /// <param name="element"></param>
    public void AddElement(SimpleCell element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (!element.Cellgroup)
        {
            throw new BadElementException("You can't add cells to a table directly, add them to a row first.");
        }

        _content.Add(element);
    }

    /// <summary>
    ///     Creates a PdfPTable object based on this TableAttributes object.
    ///     @throws DocumentException
    /// </summary>
    /// <returns>a com.lowagie.text.pdf.PdfPTable object</returns>
    public PdfPTable CreatePdfPTable()
    {
        if (_content.Count == 0)
        {
            throw new BadElementException("Trying to create a table without rows.");
        }

        var rowx = (SimpleCell)_content[0];
        var columns = 0;
        foreach (SimpleCell cell in rowx.Content)
        {
            columns += cell.Colspan;
        }

        var widths = new float[columns];
        var widthpercentages = new float[columns];
        var table = new PdfPTable(columns);
        table.TableEvent = this;
        table.HorizontalAlignment = _alignment;
        int pos;
        foreach (SimpleCell row in _content)
        {
            pos = 0;
            foreach (SimpleCell cell in row.Content)
            {
                if (float.IsNaN(cell.SpacingLeft))
                {
                    cell.SpacingLeft = _cellspacing / 2f;
                }

                if (float.IsNaN(cell.SpacingRight))
                {
                    cell.SpacingRight = _cellspacing / 2f;
                }

                if (float.IsNaN(cell.SpacingTop))
                {
                    cell.SpacingTop = _cellspacing / 2f;
                }

                if (float.IsNaN(cell.SpacingBottom))
                {
                    cell.SpacingBottom = _cellspacing / 2f;
                }

                cell.Padding = _cellpadding;
                table.AddCell(cell.CreatePdfPCell(row));
                if (cell.Colspan == 1)
                {
                    if (cell.Width > 0)
                    {
                        widths[pos] = cell.Width;
                    }

                    if (cell.Widthpercentage > 0)
                    {
                        widthpercentages[pos] = cell.Widthpercentage;
                    }
                }

                pos += cell.Colspan;
            }
        }

        var sumWidths = 0f;
        for (var i = 0; i < columns; i++)
        {
            if (widths[i].ApproxEquals(0))
            {
                sumWidths = 0;
                break;
            }

            sumWidths += widths[i];
        }

        if (sumWidths > 0)
        {
            table.TotalWidth = sumWidths;
            table.SetWidths(widths);
        }
        else
        {
            for (var i = 0; i < columns; i++)
            {
                if (widthpercentages[i].ApproxEquals(0))
                {
                    sumWidths = 0;
                    break;
                }

                sumWidths += widthpercentages[i];
            }

            if (sumWidths > 0)
            {
                table.SetWidths(widthpercentages);
            }
        }

        if (_width > 0)
        {
            table.TotalWidth = _width;
        }

        if (_widthpercentage > 0)
        {
            table.WidthPercentage = _widthpercentage;
        }

        return table;
    }

    /// <summary>
    ///     Creates a Table object based on this TableAttributes object.
    ///     @throws BadElementException
    /// </summary>
    /// <returns>a com.lowagie.text.Table object</returns>
    public Table CreateTable()
    {
        if (_content.Count == 0)
        {
            throw new BadElementException("Trying to create a table without rows.");
        }

        var rowx = (SimpleCell)_content[0];
        var columns = 0;
        foreach (SimpleCell cell in rowx.Content)
        {
            columns += cell.Colspan;
        }

        var widths = new float[columns];
        var widthpercentages = new float[columns];
        var table = new Table(columns);
        table.Alignment = _alignment;
        table.Spacing = _cellspacing;
        table.Padding = _cellpadding;
        table.CloneNonPositionParameters(this);
        int pos;
        foreach (SimpleCell row in _content)
        {
            pos = 0;
            foreach (SimpleCell cell in row.Content)
            {
                table.AddCell(cell.CreateCell(row));
                if (cell.Colspan == 1)
                {
                    if (cell.Width > 0)
                    {
                        widths[pos] = cell.Width;
                    }

                    if (cell.Widthpercentage > 0)
                    {
                        widthpercentages[pos] = cell.Widthpercentage;
                    }
                }

                pos += cell.Colspan;
            }
        }

        var sumWidths = 0f;
        for (var i = 0; i < columns; i++)
        {
            if (widths[i].ApproxEquals(0))
            {
                sumWidths = 0;
                break;
            }

            sumWidths += widths[i];
        }

        if (sumWidths > 0)
        {
            table.Width = sumWidths;
            table.Locked = true;
            table.Widths = widths;
        }
        else
        {
            for (var i = 0; i < columns; i++)
            {
                if (widthpercentages[i].ApproxEquals(0))
                {
                    sumWidths = 0;
                    break;
                }

                sumWidths += widthpercentages[i];
            }

            if (sumWidths > 0)
            {
                table.Widths = widthpercentages;
            }
        }

        if (_width > 0)
        {
            table.Width = _width;
            table.Locked = true;
        }
        else if (_widthpercentage > 0)
        {
            table.Width = _widthpercentage;
        }

        return table;
    }
}