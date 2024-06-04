using iTextSharp.text.pdf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf.table;

/// <summary>
///     The RtfTable wraps a Table.
///     INTERNAL USE ONLY
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Steffen Stundzig
///     @author Benoit WIART
/// </summary>
public class RtfTable : RtfElement
{
    /// <summary>
    ///     The alignment of this RtfTable
    /// </summary>
    private int _alignment = Element.ALIGN_CENTER;

    /// <summary>
    ///     The border style of this RtfTable
    /// </summary>
    private RtfBorderGroup _borders;

    /// <summary>
    ///     The cell padding
    /// </summary>
    private float _cellPadding;

    /// <summary>
    ///     Whether the cells in this RtfTable must fit in a page
    /// </summary>
    private bool _cellsFitToPage;

    /// <summary>
    ///     The cell spacing
    /// </summary>
    private float _cellSpacing;

    /// <summary>
    ///     The number of header rows in this RtfTable
    /// </summary>
    private int _headerRows;

    /// <summary>
    ///     The offset from the previous text
    /// </summary>
    private int _offset = -1;

    /// <summary>
    ///     An array with the proportional widths of the cells in each row
    /// </summary>
    private float[] _proportionalWidths;

    /// <summary>
    ///     The rows of this RtfTable
    /// </summary>
    private List<RtfElement> _rows;

    /// <summary>
    ///     Whether the whole RtfTable must fit in a page
    /// </summary>
    private bool _tableFitToPage;

    /// <summary>
    ///     The percentage of the page width that this RtfTable covers
    /// </summary>
    private float _tableWidthPercent = 80;

    /// <summary>
    ///     Constructs a RtfTable based on a Table for a RtfDocument.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfTable belongs to</param>
    /// <param name="table">The Table that this RtfTable wraps</param>
    public RtfTable(RtfDocument doc, Table table) : base(doc)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        table.Complete();
        importTable(table);
    }

    /// <summary>
    ///     Constructs a RtfTable based on a PdfTable for a RtfDocument.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfTable belongs to</param>
    /// <param name="table">The PdfPTable that this RtfTable wraps</param>
    public RtfTable(RtfDocument doc, PdfPTable table) : base(doc)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        importTable(table);
    }

    /// <summary>
    ///     Writes the content of this RtfTable
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        if (!InHeader)
        {
            if (_offset != -1)
            {
                outp.Write(RtfFont.FontSize, 0, RtfFont.FontSize.Length);
                byte[] t;
                outp.Write(t = IntToByteArray(_offset), 0, t.Length);
            }

            outp.Write(RtfParagraph.Paragraph, 0, RtfParagraph.Paragraph.Length);
        }

        for (var i = 0; i < _rows.Count; i++)
        {
            var re = _rows[i];
            re.WriteContent(outp);
        }

        outp.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
    }

    /// <summary>
    ///     Gets the alignment of this RtfTable
    /// </summary>
    /// <returns>The alignment of this RtfTable.</returns>
    protected internal int GetAlignment() => _alignment;

    /// <summary>
    ///     Gets the borders of this RtfTable
    /// </summary>
    /// <returns>The borders of this RtfTable.</returns>
    protected internal RtfBorderGroup GetBorders() => _borders;

    /// <summary>
    ///     Gets the cell padding of this RtfTable
    /// </summary>
    /// <returns>The cell padding of this RtfTable.</returns>
    protected internal float GetCellPadding() => _cellPadding;

    /// <summary>
    ///     Gets the cellsFitToPage setting of this RtfTable.
    /// </summary>
    /// <returns>The cellsFitToPage setting of this RtfTable.</returns>
    protected internal bool GetCellsFitToPage() => _cellsFitToPage;

    /// <summary>
    ///     Gets the cell spacing of this RtfTable
    /// </summary>
    /// <returns>The cell spacing of this RtfTable.</returns>
    protected internal float GetCellSpacing() => _cellSpacing;

    /// <summary>
    ///     Gets the number of header rows of this RtfTable
    /// </summary>
    /// <returns>The number of header rows</returns>
    protected internal int GetHeaderRows() => _headerRows;

    /// <summary>
    ///     Gets the proportional cell widths of this RtfTable
    /// </summary>
    /// <returns>The proportional widths of this RtfTable.</returns>
    protected internal float[] GetProportionalWidths() => (float[])_proportionalWidths.Clone();

    /// <summary>
    ///     Gets the rows of this RtfTable
    /// </summary>
    /// <returns>The rows of this RtfTable</returns>
    protected internal IList<RtfElement> GetRows() => _rows;

    /// <summary>
    ///     Gets the tableFitToPage setting of this RtfTable.
    /// </summary>
    /// <returns>The tableFitToPage setting of this RtfTable.</returns>
    protected internal bool GetTableFitToPage() => _tableFitToPage;

    /// <summary>
    ///     Gets the percentage of the page width this RtfTable covers
    /// </summary>
    /// <returns>The percentage of the page width.</returns>
    protected internal float GetTableWidthPercent() => _tableWidthPercent;

    /// <summary>
    ///     Imports the rows and settings from the Table into this
    ///     RtfTable.
    /// </summary>
    /// <param name="table">The source Table</param>
    private void importTable(Table table)
    {
        _rows = new List<RtfElement>();
        _tableWidthPercent = table.Width;
        _proportionalWidths = table.ProportionalWidths;
        _cellPadding = (float)(table.Cellpadding * TWIPS_FACTOR);
        _cellSpacing = (float)(table.Cellspacing * TWIPS_FACTOR);
        _borders = new RtfBorderGroup(Document, RtfBorder.ROW_BORDER, table.Border, table.BorderWidth,
                                      table.BorderColor);
        _alignment = table.Alignment;

        var i = 0;
        foreach (var row in table)
        {
            _rows.Add(new RtfRow(Document, this, row, i));
            i++;
        }

        for (i = 0; i < _rows.Count; i++)
        {
            ((RtfRow)_rows[i]).HandleCellSpanning();
            ((RtfRow)_rows[i]).CleanRow();
        }

        _headerRows = table.LastHeaderRow;
        _cellsFitToPage = table.CellsFitPage;
        _tableFitToPage = table.TableFitsPage;
        if (!float.IsNaN(table.Offset))
        {
            _offset = (int)(table.Offset * 2);
        }
    }

    /// <summary>
    ///     Imports the rows and settings from the Table into this
    ///     RtfTable.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="table">The source PdfPTable</param>
    private void importTable(PdfPTable table)
    {
        _rows = new List<RtfElement>();
        _tableWidthPercent = table.WidthPercentage;
        _proportionalWidths = table.AbsoluteWidths;
        _cellPadding = (float)(table.SpacingAfter * TWIPS_FACTOR);
        _cellSpacing = (float)(table.SpacingAfter * TWIPS_FACTOR);
        _alignment = table.HorizontalAlignment;

        var i = 0;
        foreach (var row in table.Rows)
        {
            _rows.Add(new RtfRow(Document, this, row, i));
            i++;
        }

        foreach (RtfRow row in _rows)
        {
            row.HandleCellSpanning();
            row.CleanRow();
        }

        _headerRows = table.HeaderRows;
        _cellsFitToPage = table.KeepTogether;
    }
}