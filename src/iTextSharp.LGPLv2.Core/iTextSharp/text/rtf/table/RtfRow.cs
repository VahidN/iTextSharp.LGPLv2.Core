using iTextSharp.text.pdf;
using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.table;

/// <summary>
///     The RtfRow wraps one Row for a RtfTable.
///     INTERNAL USE ONLY
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Steffen Stundzig
///     @author Lorenz Maierhofer
/// </summary>
public class RtfRow : RtfElement
{
    /// <summary>
    ///     Constant for center alignment of this RtfRow
    /// </summary>
    private static readonly byte[] _rowAlignCenter = DocWriter.GetIsoBytes("\\trqc");

    /// <summary>
    ///     Constant for justified alignment of this RtfRow
    /// </summary>
    private static readonly byte[] _rowAlignJustified = DocWriter.GetIsoBytes("\\trqj");

    /// <summary>
    ///     Constant for left alignment of this RtfRow
    /// </summary>
    private static readonly byte[] _rowAlignLeft = DocWriter.GetIsoBytes("\\trql");

    /// <summary>
    ///     Constant for right alignment of this RtfRow
    /// </summary>
    private static readonly byte[] _rowAlignRight = DocWriter.GetIsoBytes("\\trqr");

    /// <summary>
    ///     Constant for the RtfRow beginning
    /// </summary>
    private static readonly byte[] _rowBegin = DocWriter.GetIsoBytes("\\trowd");

    /// <summary>
    ///     Constant for the cell left padding
    /// </summary>
    private static readonly byte[] _rowCellPaddingLeft = DocWriter.GetIsoBytes("\\trpaddl");

    /// <summary>
    ///     Constant for the cell left padding style
    /// </summary>
    private static readonly byte[] _rowCellPaddingLeftStyle = DocWriter.GetIsoBytes("\\trpaddfl3");

    /// <summary>
    ///     Constant for the cell right padding
    /// </summary>
    private static readonly byte[] _rowCellPaddingRight = DocWriter.GetIsoBytes("\\trpaddr");

    /// <summary>
    ///     Constant for the cell right padding style
    /// </summary>
    private static readonly byte[] _rowCellPaddingRightStyle = DocWriter.GetIsoBytes("\\trpaddfr3");

    /// <summary>
    ///     Constant for the cell bottom spacing
    /// </summary>
    private static readonly byte[] _rowCellSpacingBottom = DocWriter.GetIsoBytes("\\trspdb");

    /// <summary>
    ///     Constant for the cell bottom spacing style
    /// </summary>
    private static readonly byte[] _rowCellSpacingBottomStyle = DocWriter.GetIsoBytes("\\trspdfb3");

    /// <summary>
    ///     Constant for the cell left spacing
    /// </summary>
    private static readonly byte[] _rowCellSpacingLeft = DocWriter.GetIsoBytes("\\trspdl");

    /// <summary>
    ///     Constant for the cell left spacing style
    /// </summary>
    private static readonly byte[] _rowCellSpacingLeftStyle = DocWriter.GetIsoBytes("\\trspdfl3");

    /// <summary>
    ///     Constant for the cell right spacing
    /// </summary>
    private static readonly byte[] _rowCellSpacingRight = DocWriter.GetIsoBytes("\\trspdr");

    /// <summary>
    ///     Constant for the cell right spacing style
    /// </summary>
    private static readonly byte[] _rowCellSpacingRightStyle = DocWriter.GetIsoBytes("\\trspdfr3");

    /// <summary>
    ///     Constant for the cell top spacing
    /// </summary>
    private static readonly byte[] _rowCellSpacingTop = DocWriter.GetIsoBytes("\\trspdt");

    /// <summary>
    ///     Constant for the cell top spacing style
    /// </summary>
    private static readonly byte[] _rowCellSpacingTopStyle = DocWriter.GetIsoBytes("\\trspdft3");

    /// <summary>
    ///     Constant for the end of a row
    /// </summary>
    private static readonly byte[] _rowEnd = DocWriter.GetIsoBytes("\\row");

    /// <summary>
    ///     Constant for the graph style of this RtfRow
    /// </summary>
    private static readonly byte[] _rowGraph = DocWriter.GetIsoBytes("\\trgaph10");

    /// <summary>
    ///     Constant to specify that this is a header RtfRow
    /// </summary>
    private static readonly byte[] _rowHeaderRow = DocWriter.GetIsoBytes("\\trhdr");

    /// <summary>
    ///     Constant to specify that this RtfRow are not to be broken across pages
    /// </summary>
    private static readonly byte[] _rowKeepTogether = DocWriter.GetIsoBytes("\\trkeep");

    /// <summary>
    ///     Constant for the RtfRow width
    /// </summary>
    private static readonly byte[] _rowWidth = DocWriter.GetIsoBytes("\\trwWidth");

    /// <summary>
    ///     Constant for the RtfRow width style
    /// </summary>
    private static readonly byte[] _rowWidthStyle = DocWriter.GetIsoBytes("\\trftsWidth3");

    /// <summary>
    ///     The RtfTable this RtfRow belongs to
    /// </summary>
    private readonly RtfTable _parentTable;

    /// <summary>
    ///     The row number
    /// </summary>
    private readonly int _rowNumber;

    /// <summary>
    ///     The cells of this RtfRow
    /// </summary>
    private List<RtfCell> _cells;

    /// <summary>
    ///     The width of this row
    /// </summary>
    private int _width;

    /// <summary>
    ///     Constructs a RtfRow for a Row.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfRow belongs to</param>
    /// <param name="rtfTable">The RtfTable this RtfRow belongs to</param>
    /// <param name="row">The Row this RtfRow is based on</param>
    /// <param name="rowNumber">The number of this row</param>
    protected internal RtfRow(RtfDocument doc, RtfTable rtfTable, Row row, int rowNumber) : base(doc)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row));
        }

        _parentTable = rtfTable;
        _rowNumber = rowNumber;
        importRow(row);
    }

    /// <summary>
    ///     Constructs a RtfRow for a Row.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfRow belongs to</param>
    /// <param name="rtfTable">The RtfTable this RtfRow belongs to</param>
    /// <param name="row">The Row this RtfRow is based on</param>
    /// <param name="rowNumber">The number of this row</param>
    protected internal RtfRow(RtfDocument doc, RtfTable rtfTable, PdfPRow row, int rowNumber) : base(doc)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row));
        }

        _parentTable = rtfTable;
        _rowNumber = rowNumber;
        importRow(row);
    }

    /// <summary>
    ///     Writes the content of this RtfRow
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        writeRowDefinition(outp);

        for (var i = 0; i < _cells.Count; i++)
        {
            var rtfCell = _cells[i];
            rtfCell.WriteContent(outp);
        }

        outp.Write(Delimiter, 0, Delimiter.Length);

        if (Document.GetDocumentSettings().IsOutputTableRowDefinitionAfter())
        {
            writeRowDefinition(outp);
        }

        outp.Write(_rowEnd, 0, _rowEnd.Length);
        Document.OutputDebugLinebreak(outp);
    }

    /// <summary>
    ///     Cleans the deleted RtfCells from the total RtfCells.
    /// </summary>
    protected internal void CleanRow()
    {
        var i = 0;
        while (i < _cells.Count)
        {
            if (_cells[i].IsDeleted())
            {
                _cells.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    /// <summary>
    ///     Gets the cells of this RtfRow
    /// </summary>
    /// <returns>The cells of this RtfRow</returns>
    protected internal IList<RtfCell> GetCells() => _cells;

    /// <summary>
    ///     Gets the parent RtfTable of this RtfRow
    /// </summary>
    /// <returns>The parent RtfTable of this RtfRow</returns>
    protected internal RtfTable GetParentTable() => _parentTable;

    /// <summary>
    ///     Performs a second pass over all cells to handle cell row/column spanning.
    /// </summary>
    protected internal void HandleCellSpanning()
    {
        var deletedCell = new RtfCell(true);
        for (var i = 0; i < _cells.Count; i++)
        {
            var rtfCell = _cells[i];
            if (rtfCell.Colspan > 1)
            {
                var cSpan = rtfCell.Colspan;
                for (var j = i + 1; j < i + cSpan; j++)
                {
                    if (j < _cells.Count)
                    {
                        var rtfCellMerge = _cells[j];
                        rtfCell.SetCellRight(rtfCell.GetCellRight() + rtfCellMerge.GetCellWidth());
                        rtfCell.SetCellWidth(rtfCell.GetCellWidth() + rtfCellMerge.GetCellWidth());
                        _cells[j] = deletedCell;
                    }
                }
            }

            if (rtfCell.Rowspan > 1)
            {
                var rows = _parentTable.GetRows();
                for (var j = 1; j < rtfCell.Rowspan; j++)
                {
                    var mergeRow = (RtfRow)rows[_rowNumber + j];
                    if (_rowNumber + j < rows.Count)
                    {
                        var rtfCellMerge = mergeRow.GetCells()[i];
                        rtfCellMerge.SetCellMergeChild(rtfCell);
                    }

                    if (rtfCell.Colspan > 1)
                    {
                        var cSpan = rtfCell.Colspan;
                        for (var k = i + 1; k < i + cSpan; k++)
                        {
                            if (k < mergeRow.GetCells().Count)
                            {
                                mergeRow.GetCells()[k] = deletedCell;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Imports a Row and copies all settings
    /// </summary>
    /// <param name="row">The Row to import</param>
    private void importRow(Row row)
    {
        _cells = new List<RtfCell>();
        _width = Document.GetDocumentHeader().GetPageSetting().GetPageWidth() -
                 Document.GetDocumentHeader().GetPageSetting().GetMarginLeft() -
                 Document.GetDocumentHeader().GetPageSetting().GetMarginRight();
        _width = (int)(_width * _parentTable.GetTableWidthPercent() / 100);

        var cellRight = 0;
        var cellWidth = 0;
        for (var i = 0; i < row.Columns; i++)
        {
            cellWidth = (int)(_width * _parentTable.GetProportionalWidths()[i] / 100);
            cellRight = cellRight + cellWidth;

            var cell = (Cell)row.GetCell(i);
            var rtfCell = new RtfCell(Document, this, cell);
            rtfCell.SetCellRight(cellRight);
            rtfCell.SetCellWidth(cellWidth);
            _cells.Add(rtfCell);
        }
    }

    /// <summary>
    ///     Imports a PdfPRow and copies all settings
    ///     @since 2.1.3
    /// </summary>
    /// <param name="row">The PdfPRow to import</param>
    private void importRow(PdfPRow row)
    {
        _cells = new List<RtfCell>();
        _width = Document.GetDocumentHeader().GetPageSetting().GetPageWidth() -
                 Document.GetDocumentHeader().GetPageSetting().GetMarginLeft() -
                 Document.GetDocumentHeader().GetPageSetting().GetMarginRight();
        _width = (int)(_width * _parentTable.GetTableWidthPercent() / 100);

        var cellRight = 0;
        var cellWidth = 0;
        var cells = row.GetCells();
        for (var i = 0; i < cells.Length; i++)
        {
            cellWidth = (int)(_width * _parentTable.GetProportionalWidths()[i] / 100);
            cellRight = cellRight + cellWidth;

            var cell = cells[i];
            var rtfCell = new RtfCell(Document, this, cell);
            rtfCell.SetCellRight(cellRight);
            rtfCell.SetCellWidth(cellWidth);
            _cells.Add(rtfCell);
        }
    }

    /// <summary>
    ///     Writes the row definition/settings.
    /// </summary>
    /// <param name="result">The  Stream  to write the definitions to.</param>
    private void writeRowDefinition(Stream result)
    {
        byte[] t;
        result.Write(_rowBegin, 0, _rowBegin.Length);
        Document.OutputDebugLinebreak(result);
        result.Write(_rowWidthStyle, 0, _rowWidthStyle.Length);
        result.Write(_rowWidth, 0, _rowWidth.Length);
        result.Write(t = IntToByteArray(_width), 0, t.Length);
        if (_parentTable.GetCellsFitToPage())
        {
            result.Write(_rowKeepTogether, 0, _rowKeepTogether.Length);
        }

        if (_rowNumber <= _parentTable.GetHeaderRows())
        {
            result.Write(_rowHeaderRow, 0, _rowHeaderRow.Length);
        }

        switch (_parentTable.GetAlignment())
        {
            case Element.ALIGN_LEFT:
                result.Write(_rowAlignLeft, 0, _rowAlignLeft.Length);
                break;
            case Element.ALIGN_RIGHT:
                result.Write(_rowAlignRight, 0, _rowAlignRight.Length);
                break;
            case Element.ALIGN_CENTER:
                result.Write(_rowAlignCenter, 0, _rowAlignCenter.Length);
                break;
            case Element.ALIGN_JUSTIFIED:
            case Element.ALIGN_JUSTIFIED_ALL:
                result.Write(_rowAlignJustified, 0, _rowAlignJustified.Length);
                break;
        }

        result.Write(_rowGraph, 0, _rowGraph.Length);

        var borders = _parentTable.GetBorders();
        if (borders != null)
        {
            borders.WriteContent(result);
        }

        if (_parentTable.GetCellSpacing() > 0)
        {
            result.Write(_rowCellSpacingLeft, 0, _rowCellSpacingLeft.Length);
            result.Write(t = IntToByteArray((int)(_parentTable.GetCellSpacing() / 2)), 0, t.Length);
            result.Write(_rowCellSpacingLeftStyle, 0, _rowCellSpacingLeftStyle.Length);
            result.Write(_rowCellSpacingTop, 0, _rowCellSpacingTop.Length);
            result.Write(t = IntToByteArray((int)(_parentTable.GetCellSpacing() / 2)), 0, t.Length);
            result.Write(_rowCellSpacingTopStyle, 0, _rowCellSpacingTopStyle.Length);
            result.Write(_rowCellSpacingRight, 0, _rowCellSpacingRight.Length);
            result.Write(t = IntToByteArray((int)(_parentTable.GetCellSpacing() / 2)), 0, t.Length);
            result.Write(_rowCellSpacingRightStyle, 0, _rowCellSpacingRightStyle.Length);
            result.Write(_rowCellSpacingBottom, 0, _rowCellSpacingBottom.Length);
            result.Write(t = IntToByteArray((int)(_parentTable.GetCellSpacing() / 2)), 0, t.Length);
            result.Write(_rowCellSpacingBottomStyle, 0, _rowCellSpacingBottomStyle.Length);
        }

        result.Write(_rowCellPaddingLeft, 0, _rowCellPaddingLeft.Length);
        result.Write(t = IntToByteArray((int)(_parentTable.GetCellPadding() / 2)), 0, t.Length);
        result.Write(_rowCellPaddingRight, 0, _rowCellPaddingRight.Length);
        result.Write(t = IntToByteArray((int)(_parentTable.GetCellPadding() / 2)), 0, t.Length);
        result.Write(_rowCellPaddingLeftStyle, 0, _rowCellPaddingLeftStyle.Length);
        result.Write(_rowCellPaddingRightStyle, 0, _rowCellPaddingRightStyle.Length);

        Document.OutputDebugLinebreak(result);

        for (var i = 0; i < _cells.Count; i++)
        {
            var rtfCell = _cells[i];
            rtfCell.WriteDefinition(result);
        }
    }
}