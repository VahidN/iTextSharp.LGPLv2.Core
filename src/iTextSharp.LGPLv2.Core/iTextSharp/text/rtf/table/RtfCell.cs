using iTextSharp.text.pdf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf.table;

/// <summary>
///     The RtfCell wraps a Cell, but can also be added directly to a Table.
///     The RtfCell is an extension of Cell, that supports a multitude of different
///     borderstyles.
///     @version $Id: RtfCell.cs,v 1.14 2008/05/16 19:31:18 psoares33 Exp $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Steffen Stundzig
///     @author Benoit WIART
///     @see com.lowagie.text.rtf.table.RtfBorder
/// </summary>
public class RtfCell : Cell, IRtfExtendedElement
{
    /// <summary>
    ///     This cell is not merged
    /// </summary>
    private const int MergeNone = 0;

    /// <summary>
    ///     This cell is a child cell of a vertical merge operation
    /// </summary>
    private const int MergeVertChild = 2;

    /// <summary>
    ///     This cell is the parent cell of a vertical merge operation
    /// </summary>
    private const int MergeVertParent = 1;

    /// <summary>
    ///     Whether this RtfCell is a placeholder for a removed table cell.
    /// </summary>
    private readonly bool _deleted;

    /// <summary>
    ///     The parent RtfRow of this RtfCell
    /// </summary>
    private readonly RtfRow _parentRow;

    /// <summary>
    ///     The background color of this RtfCell
    /// </summary>
    private RtfColor _backgroundColor;

    /// <summary>
    ///     The borders of this RtfCell
    /// </summary>
    private RtfBorderGroup _borders;

    /// <summary>
    ///     The padding of this RtfCell
    /// </summary>
    private int _cellPadding;

    /// <summary>
    ///     Cell padding bottom
    /// </summary>
    private float _cellPaddingBottom;

    /// <summary>
    ///     Whether to use generic padding or individual
    ///     padding values (cellPaddingLeft, cellPaddingTop, cellPaddingBottom, cellPaddingRight)
    /// </summary>
    /// <summary>
    ///     private bool usePadding = false;
    /// </summary>
    /// <summary>
    ///     Cell padding left
    /// </summary>
    private float _cellPaddingLeft;

    /// <summary>
    ///     Cell padding right
    /// </summary>
    private float _cellPaddingRight;

    /// <summary>
    ///     Cell padding top
    /// </summary>
    private float _cellPaddingTop;

    /// <summary>
    ///     The right margin of this RtfCell
    /// </summary>
    private int _cellRight;

    /// <summary>
    ///     The width of this RtfCell
    /// </summary>
    private int _cellWidth;

    /// <summary>
    ///     The content of this RtfCell
    /// </summary>
    private List<IRtfBasicElement> _content;

    /// <summary>
    ///     The RtfDocument this RtfCell belongs to
    /// </summary>
    private RtfDocument _document;

    /// <summary>
    ///     Whether this RtfCell is in a header
    /// </summary>
    private bool _inHeader;

    /// <summary>
    ///     The merge type of this RtfCell
    /// </summary>
    private int _mergeType = MergeNone;

    /// <summary>
    ///     Constructs an empty RtfCell
    /// </summary>
    public RtfCell()
    {
        _borders = new RtfBorderGroup();
        verticalAlignment = ALIGN_MIDDLE;
    }

    /// <summary>
    ///     Constructs a RtfCell based upon a String
    /// </summary>
    /// <param name="content">The String to base the RtfCell on</param>
    public RtfCell(string content) : base(content)
    {
        _borders = new RtfBorderGroup();
        verticalAlignment = ALIGN_MIDDLE;
    }

    /// <summary>
    ///     Constructs a RtfCell based upon an Element
    ///     @throws BadElementException If the Element is not valid
    /// </summary>
    /// <param name="element">The Element to base the RtfCell on</param>
    public RtfCell(IElement element) : base(element)
    {
        _borders = new RtfBorderGroup();
        verticalAlignment = ALIGN_MIDDLE;
    }

    /// <summary>
    ///     Constructs a deleted RtfCell.
    /// </summary>
    /// <param name="deleted">Whether this RtfCell is actually deleted.</param>
    internal protected RtfCell(bool deleted)
    {
        _deleted = deleted;
        verticalAlignment = ALIGN_MIDDLE;
    }

    /// <summary>
    ///     Constructs a RtfCell based on a Cell.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfCell belongs to</param>
    /// <param name="row">The RtfRow this RtfCell lies in</param>
    /// <param name="cell">The Cell to base this RtfCell on</param>
    internal protected RtfCell(RtfDocument doc, RtfRow row, Cell cell)
    {
        _document = doc;
        _parentRow = row;
        importCell(cell);
    }

    /// <summary>
    ///     Constructs a RtfCell based on a Cell.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfCell belongs to</param>
    /// <param name="row">The RtfRow this RtfCell lies in</param>
    /// <param name="cell">The PdfPCell to base this RtfCell on</param>
    internal protected RtfCell(RtfDocument doc, RtfRow row, PdfPCell cell)
    {
        _document = doc;
        _parentRow = row;
        importCell(cell);
    }

    /// <summary>
    ///     Sets whether this RtfCell is in a header
    /// </summary>
    /// <param name="inHeader"> True  if this RtfCell is in a header,  false  otherwise</param>
    public void SetInHeader(bool inHeader)
    {
        _inHeader = inHeader;

        for (var i = 0; i < _content.Count; i++)
        {
            _content[i].SetInHeader(inHeader);
        }
    }

    /// <summary>
    ///     Unused
    /// </summary>
    /// <param name="inTable"></param>
    public void SetInTable(bool inTable)
    {
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfCell belongs to
    /// </summary>
    /// <param name="doc">The RtfDocument to use</param>
    public void SetRtfDocument(RtfDocument doc) => _document = doc;

    /// <summary>
    ///     Write the content of this RtfCell
    /// </summary>
    public virtual void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;

        if (_content.Count == 0)
        {
            outp.Write(RtfPhrase.ParagraphDefaults, offset: 0, RtfPhrase.ParagraphDefaults.Length);

            if (_parentRow.GetParentTable().GetTableFitToPage())
            {
                outp.Write(RtfParagraphStyle.KeepTogetherWithNext, offset: 0,
                    RtfParagraphStyle.KeepTogetherWithNext.Length);
            }

            outp.Write(RtfPhrase.InTable, offset: 0, RtfPhrase.InTable.Length);
        }
        else
        {
            for (var i = 0; i < _content.Count; i++)
            {
                var rtfElement = _content[i];

                if (rtfElement is RtfParagraph paragraph)
                {
                    paragraph.SetKeepTogetherWithNext(_parentRow.GetParentTable().GetTableFitToPage());
                }

                rtfElement.WriteContent(outp);

                if (rtfElement is RtfParagraph && i < _content.Count - 1)
                {
                    outp.Write(RtfParagraph.Paragraph, offset: 0, RtfParagraph.Paragraph.Length);
                }
            }
        }

        outp.Write(t = DocWriter.GetIsoBytes(text: "\\cell"), offset: 0, t.Length);
    }

    /// <summary>
    ///     Write the cell definition part of this RtfCell
    /// </summary>
    /// <returns>A byte array with the cell definition</returns>
    public virtual void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;

        if (_mergeType == MergeVertParent)
        {
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clvmgf"), offset: 0, t.Length);
        }
        else if (_mergeType == MergeVertChild)
        {
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clvmrg"), offset: 0, t.Length);
        }

        switch (verticalAlignment)
        {
            case ALIGN_BOTTOM:
                outp.Write(t = DocWriter.GetIsoBytes(text: "\\clvertalb"), offset: 0, t.Length);

                break;
            case ALIGN_CENTER:
            case ALIGN_MIDDLE:
                outp.Write(t = DocWriter.GetIsoBytes(text: "\\clvertalc"), offset: 0, t.Length);

                break;
            case ALIGN_TOP:
                outp.Write(t = DocWriter.GetIsoBytes(text: "\\clvertalt"), offset: 0, t.Length);

                break;
        }

        _borders.WriteContent(outp);

        if (_backgroundColor != null)
        {
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clcbpat"), offset: 0, t.Length);
            outp.Write(t = intToByteArray(_backgroundColor.GetColorNumber()), offset: 0, t.Length);
        }

        _document.OutputDebugLinebreak(outp);

        outp.Write(t = DocWriter.GetIsoBytes(text: "\\clftsWidth3"), offset: 0, t.Length);
        _document.OutputDebugLinebreak(outp);

        outp.Write(t = DocWriter.GetIsoBytes(text: "\\clwWidth"), offset: 0, t.Length);
        outp.Write(t = intToByteArray(_cellWidth), offset: 0, t.Length);
        _document.OutputDebugLinebreak(outp);

        if (_cellPadding > 0)
        {
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clpadl"), offset: 0, t.Length);
            outp.Write(t = intToByteArray(_cellPadding / 2), offset: 0, t.Length);
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clpadt"), offset: 0, t.Length);
            outp.Write(t = intToByteArray(_cellPadding / 2), offset: 0, t.Length);
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clpadr"), offset: 0, t.Length);
            outp.Write(t = intToByteArray(_cellPadding / 2), offset: 0, t.Length);
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clpadb"), offset: 0, t.Length);
            outp.Write(t = intToByteArray(_cellPadding / 2), offset: 0, t.Length);
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clpadfl3"), offset: 0, t.Length);
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clpadft3"), offset: 0, t.Length);
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clpadfr3"), offset: 0, t.Length);
            outp.Write(t = DocWriter.GetIsoBytes(text: "\\clpadfb3"), offset: 0, t.Length);
        }

        outp.Write(t = DocWriter.GetIsoBytes(text: "\\cellx"), offset: 0, t.Length);
        outp.Write(t = intToByteArray(_cellRight), offset: 0, t.Length);
    }

    /// <summary>
    ///     Checks whether this RtfCell is a placeholder for
    ///     a table cell that has been removed due to col/row spanning.
    /// </summary>
    /// <returns> True  if this RtfCell is deleted,  false  otherwise.</returns>
    public bool IsDeleted() => _deleted;

    /// <summary>
    ///     Gets whether this  RtfCell  is in a header
    /// </summary>
    /// <returns> True  if this  RtfCell  is in a header,  false  otherwise</returns>
    public bool IsInHeader() => _inHeader;

    /// <summary>
    ///     Set the borders of this RtfCell
    /// </summary>
    /// <param name="borderGroup">The RtfBorderGroup to use as borders</param>
    public void SetBorders(RtfBorderGroup borderGroup)
        => _borders = new RtfBorderGroup(_document, RtfBorder.CELL_BORDER, borderGroup);

    /// <summary>
    ///     Gets the borders of this RtfCell
    /// </summary>
    /// <returns>The borders of this RtfCell</returns>
    internal protected RtfBorderGroup GetBorders() => _borders;

    /// <summary>
    ///     Gets the cell padding of this RtfCell
    /// </summary>
    /// <returns>The cell padding of this RtfCell</returns>
    internal protected int GetCellpadding() => _cellPadding;

    /// <summary>
    ///     Gets the right margin of this RtfCell
    /// </summary>
    /// <returns>The right margin of this RtfCell.</returns>
    internal protected int GetCellRight() => _cellRight;

    /// <summary>
    ///     Gets the cell width of this RtfCell
    /// </summary>
    /// <returns>The cell width of this RtfCell</returns>
    internal protected int GetCellWidth() => _cellWidth;

    /// <summary>
    ///     Get the background color of this RtfCell
    /// </summary>
    /// <returns>The background color of this RtfCell</returns>
    internal protected RtfColor GetRtfBackgroundColor() => _backgroundColor;

    /// <summary>
    ///     Merge this cell into the parent cell.
    /// </summary>
    /// <param name="mergeParent">The RtfCell to merge with</param>
    internal protected void SetCellMergeChild(RtfCell mergeParent)
    {
        if (mergeParent == null)
        {
            throw new ArgumentNullException(nameof(mergeParent));
        }

        _mergeType = MergeVertChild;
        _cellWidth = mergeParent.GetCellWidth();
        _cellRight = mergeParent.GetCellRight();
        _cellPadding = mergeParent.GetCellpadding();
        _borders = mergeParent.GetBorders();
        verticalAlignment = mergeParent.VerticalAlignment;
        _backgroundColor = mergeParent.GetRtfBackgroundColor();
    }

    /// <summary>
    ///     Sets the right margin of this cell. Used in merge operations
    /// </summary>
    /// <param name="cellRight">The right margin to use</param>
    internal protected void SetCellRight(int cellRight) => _cellRight = cellRight;

    /// <summary>
    ///     Sets the cell width of this RtfCell. Used in merge operations.
    /// </summary>
    /// <param name="cellWidth">The cell width to use</param>
    internal protected void SetCellWidth(int cellWidth) => _cellWidth = cellWidth;

    /// <summary>
    ///     Imports the Cell properties into the RtfCell
    /// </summary>
    /// <param name="cell">The Cell to import</param>
    private void importCell(Cell cell)
    {
        _content = new List<IRtfBasicElement>();

        if (cell == null)
        {
            _borders = new RtfBorderGroup(_document, RtfBorder.CELL_BORDER, _parentRow.GetParentTable().GetBorders());

            return;
        }

        colspan = cell.Colspan;
        rowspan = cell.Rowspan;

        if (cell.Rowspan > 1)
        {
            _mergeType = MergeVertParent;
        }

        if (cell is RtfCell rtfCell)
        {
            _borders = new RtfBorderGroup(_document, RtfBorder.CELL_BORDER, rtfCell.GetBorders());
        }
        else
        {
            _borders = new RtfBorderGroup(_document, RtfBorder.CELL_BORDER, cell.Border, cell.BorderWidth,
                cell.BorderColor);
        }

        verticalAlignment = cell.VerticalAlignment;

        if (cell.BackgroundColor == null)
        {
            _backgroundColor = new RtfColor(_document, red: 255, green: 255, blue: 255);
        }
        else
        {
            _backgroundColor = new RtfColor(_document, cell.BackgroundColor);
        }

        _cellPadding = (int)_parentRow.GetParentTable().GetCellPadding();

        Paragraph container = null;

        foreach (var element in cell.Elements)
        {
            try
            {
                // should we wrap it in a paragraph
                if (!(element is Paragraph) && !(element is List))
                {
                    if (container != null)
                    {
                        container.Add(element);
                    }
                    else
                    {
                        container = new Paragraph();
                        container.Alignment = cell.HorizontalAlignment;
                        container.Add(element);
                    }
                }
                else
                {
                    if (container != null)
                    {
                        var rtfElements = _document.GetMapper().MapElement(container);

                        for (var i = 0; i < rtfElements.Length; i++)
                        {
                            rtfElements[i].SetInTable(inTable: true);
                            _content.Add(rtfElements[i]);
                        }

                        container = null;
                    }

                    // if horizontal alignment is undefined overwrite
                    // with that of enclosing cell
                    if (element is Paragraph && ((Paragraph)element).Alignment == ALIGN_UNDEFINED)
                    {
                        ((Paragraph)element).Alignment = cell.HorizontalAlignment;
                    }

                    var rtfElements2 = _document.GetMapper().MapElement(element);

                    for (var i = 0; i < rtfElements2.Length; i++)
                    {
                        rtfElements2[i].SetInTable(inTable: true);
                        _content.Add(rtfElements2[i]);
                    }
                }
            }
            catch (DocumentException)
            {
            }
        }

        if (container != null)
        {
            try
            {
                var rtfElements = _document.GetMapper().MapElement(container);

                for (var i = 0; i < rtfElements.Length; i++)
                {
                    rtfElements[i].SetInTable(inTable: true);
                    _content.Add(rtfElements[i]);
                }
            }
            catch (DocumentException)
            {
            }
        }
    }

    /// <summary>
    ///     Imports the Cell properties into the RtfCell
    ///     @since 2.1.3
    /// </summary>
    /// <param name="cell">The PdfPCell to import</param>
    private void importCell(PdfPCell cell)
    {
        _content = new List<IRtfBasicElement>();

        if (cell == null)
        {
            _borders = new RtfBorderGroup(_document, RtfBorder.CELL_BORDER, _parentRow.GetParentTable().GetBorders());

            return;
        }

        // padding
        _cellPadding = (int)_parentRow.GetParentTable().GetCellPadding();
        _cellPaddingBottom = cell.PaddingBottom;
        _cellPaddingTop = cell.PaddingTop;
        _cellPaddingRight = cell.PaddingRight;
        _cellPaddingLeft = cell.PaddingLeft;

        // BORDERS
        _borders = new RtfBorderGroup(_document, RtfBorder.CELL_BORDER, cell.Border, cell.BorderWidth,
            cell.BorderColor);

        // border colors
        border = cell.Border;
        borderColor = cell.BorderColor;
        borderColorBottom = cell.BorderColorBottom;
        borderColorTop = cell.BorderColorTop;
        borderColorLeft = cell.BorderColorLeft;
        borderColorRight = cell.BorderColorRight;

        // border widths
        borderWidth = cell.BorderWidth;
        borderWidthBottom = cell.BorderWidthBottom;
        borderWidthTop = cell.BorderWidthTop;
        borderWidthLeft = cell.BorderWidthLeft;
        borderWidthRight = cell.BorderWidthRight;

        colspan = cell.Colspan;
        rowspan = 1; //cell.GetRowspan();

        //        if (cell.GetRowspan() > 1) {
        //            this.mergeType = MERGE_VERT_PARENT;
        //        }

        verticalAlignment = cell.VerticalAlignment;

        if (cell.BackgroundColor == null)
        {
            _backgroundColor = new RtfColor(_document, red: 255, green: 255, blue: 255);
        }
        else
        {
            _backgroundColor = new RtfColor(_document, cell.BackgroundColor);
        }

        // does it have column composite info?
        var compositeElements = cell.CompositeElements;

        if (compositeElements != null)
        {
            // does it have column info?
            Paragraph container = null;

            foreach (var element in compositeElements)
            {
                try
                {
                    // should we wrap it in a paragraph
                    if (!(element is Paragraph) && !(element is List))
                    {
                        if (container != null)
                        {
                            container.Add(element);
                        }
                        else
                        {
                            container = new Paragraph();
                            container.Alignment = cell.HorizontalAlignment;
                            container.Add(element);
                        }
                    }
                    else
                    {
                        IRtfBasicElement[] rtfElements = null;

                        if (container != null)
                        {
                            rtfElements = _document.GetMapper().MapElement(container);

                            for (var i = 0; i < rtfElements.Length; i++)
                            {
                                rtfElements[i].SetInTable(inTable: true);
                                _content.Add(rtfElements[i]);
                            }

                            container = null;
                        }

                        // if horizontal alignment is undefined overwrite
                        // with that of enclosing cell
                        if (element is Paragraph && ((Paragraph)element).Alignment == ALIGN_UNDEFINED)
                        {
                            ((Paragraph)element).Alignment = cell.HorizontalAlignment;
                        }

                        rtfElements = _document.GetMapper().MapElement(element);

                        for (var i = 0; i < rtfElements.Length; i++)
                        {
                            rtfElements[i].SetInTable(inTable: true);
                            _content.Add(rtfElements[i]);
                        }
                    }
                }
                catch (DocumentException)
                {
                }
            }

            if (container != null)
            {
                try
                {
                    var rtfElements = _document.GetMapper().MapElement(container);

                    for (var i = 0; i < rtfElements.Length; i++)
                    {
                        rtfElements[i].SetInTable(inTable: true);
                        _content.Add(rtfElements[i]);
                    }
                }
                catch (DocumentException)
                {
                }
            }
        }

        // does it have image info?

        var img = cell.Image;

        if (img != null)
        {
            try
            {
                var rtfElements = _document.GetMapper().MapElement(img);

                for (var i = 0; i < rtfElements.Length; i++)
                {
                    rtfElements[i].SetInTable(inTable: true);
                    _content.Add(rtfElements[i]);
                }
            }
            catch (DocumentException)
            {
            }
        }

        // does it have phrase info?
        var phrase = cell.Phrase;

        if (phrase != null)
        {
            try
            {
                var rtfElements = _document.GetMapper().MapElement(phrase);

                for (var i = 0; i < rtfElements.Length; i++)
                {
                    rtfElements[i].SetInTable(inTable: true);
                    _content.Add(rtfElements[i]);
                }
            }
            catch (DocumentException)
            {
            }
        }

        // does it have table info?
        var table = cell.Table;

        if (table != null)
        {
            Add(table);

            //            try {
            //              RtfBasicElement[] rtfElements = this.document.GetMapper().MapElement(table);
            //              for (int i = 0; i < rtfElements.length; i++) {
            //                  rtfElements[i].SetInTable(true);
            //                  this.content.Add(rtfElements[i]);
            //              }
            //          } catch (DocumentException e) {
            //              // TODO Auto-generated catch block
            //              e.PrintStackTrace();
            //          }
        }
    }

    /// <summary>
    ///     Transforms an integer into its String representation and then returns the bytes
    ///     of that string.
    /// </summary>
    /// <param name="i">The integer to convert</param>
    /// <returns>A byte array representing the integer</returns>
    private static byte[] intToByteArray(int i) => DocWriter.GetIsoBytes(i.ToString(CultureInfo.InvariantCulture));
}