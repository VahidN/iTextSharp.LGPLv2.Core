using System;
using System.Collections;
using System.util;
using iTextSharp.text.pdf.events;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// This is a table that can be put at an absolute position but can also
    /// be added to the document as the class  Table .
    /// In the last case when crossing pages the table always break at full rows; if a
    /// row is bigger than the page it is dropped silently to avoid infinite loops.
    ///
    /// A PdfPTableEvent can be associated to the table to do custom drawing
    /// when the table is rendered.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>

    public class PdfPTable : ILargeElement
    {

        /// <summary>
        /// The index of the duplicate  PdfContentByte  where the background will be drawn.
        /// </summary>
        public const int BACKGROUNDCANVAS = 1;

        /// <summary>
        /// The index of the original  PdfcontentByte .
        /// </summary>
        public const int BASECANVAS = 0;
        /// <summary>
        /// The index of the duplicate  PdfContentByte  where the border lines will be drawn.
        /// </summary>
        public const int LINECANVAS = 2;
        /// <summary>
        /// The index of the duplicate  PdfContentByte  where the text will be drawn.
        /// </summary>
        public const int TEXTCANVAS = 3;

        protected float[] absoluteWidths;
        /// <summary>
        /// Indicates if the PdfPTable is complete once added to the document.
        /// @since	iText 2.0.8
        /// </summary>
        protected bool Complete = true;

        protected PdfPCell[] CurrentRow;
        protected int CurrentRowIdx;
        protected PdfPCell defaultCell = new PdfPCell((Phrase)null);
        /// <summary>
        /// Holds value of property headerRows.
        /// </summary>
        protected int headerRows;

        protected bool IsColspan;
        protected float[] RelativeWidths;
        /// <summary>
        /// Keeps track of the completeness of the current row.
        /// @since    2.1.6
        /// </summary>
        protected bool RowCompleted = true;

        protected ArrayList rows = new ArrayList();
        protected int runDirection = PdfWriter.RUN_DIRECTION_DEFAULT;
        /// <summary>
        /// The spacing after the table.
        /// </summary>
        protected float spacingAfter;

        /// <summary>
        /// The spacing before the table.
        /// </summary>
        protected float spacingBefore;

        protected IPdfPTableEvent tableEvent;
        protected float totalHeight;
        protected float totalWidth;
        /// <summary>
        /// Holds value of property widthPercentage.
        /// </summary>
        protected float widthPercentage = 80;

        /// <summary>
        /// Holds value of property extendLastRow.
        /// </summary>
        private bool _extendLastRow;

        private int _footerRows;

        /// <summary>
        /// Holds value of property headersInEvent.
        /// </summary>
        private bool _headersInEvent;

        /// <summary>
        /// Holds value of property horizontalAlignment.
        /// </summary>
        private int _horizontalAlignment = Element.ALIGN_CENTER;

        /// <summary>
        /// Defines if the table should be kept
        /// on one page if possible
        /// </summary>
        private bool _keepTogether;

        /// <summary>
        /// Holds value of property lockedWidth.
        /// </summary>
        private bool _lockedWidth;

        /// <summary>
        /// Holds value of property skipFirstHeader.
        /// </summary>
        private bool _skipFirstHeader;

        /// <summary>
        /// Holds value of property skipLastFooter.
        /// @since    2.1.6
        /// </summary>
        private bool _skipLastFooter;
        /// <summary>
        /// Holds value of property splitLate.
        /// </summary>
        private bool _splitLate = true;

        /// <summary>
        /// Holds value of property splitRows.
        /// </summary>
        private bool _splitRows = true;
        /// <summary>
        /// Constructs a  PdfPTable  with the relative column widths.
        /// </summary>
        /// <param name="relativeWidths">the relative column widths</param>
        public PdfPTable(float[] relativeWidths)
        {
            if (relativeWidths == null)
                throw new ArgumentNullException("The widths array in PdfPTable constructor can not be null.");
            if (relativeWidths.Length == 0)
                throw new ArgumentException("The widths array in PdfPTable constructor can not have zero length.");
            RelativeWidths = new float[relativeWidths.Length];
            Array.Copy(relativeWidths, 0, RelativeWidths, 0, relativeWidths.Length);
            absoluteWidths = new float[relativeWidths.Length];
            CalculateWidths();
            CurrentRow = new PdfPCell[absoluteWidths.Length];
            _keepTogether = false;
        }

        /// <summary>
        /// Constructs a  PdfPTable  with  numColumns  columns.
        /// </summary>
        /// <param name="numColumns">the number of columns</param>
        public PdfPTable(int numColumns)
        {
            if (numColumns <= 0)
                throw new ArgumentException("The number of columns in PdfPTable constructor must be greater than zero.");
            RelativeWidths = new float[numColumns];
            for (int k = 0; k < numColumns; ++k)
                RelativeWidths[k] = 1;
            absoluteWidths = new float[RelativeWidths.Length];
            CalculateWidths();
            CurrentRow = new PdfPCell[absoluteWidths.Length];
            _keepTogether = false;
        }

        /// <summary>
        /// Constructs a copy of a  PdfPTable .
        /// </summary>
        /// <param name="table">the  PdfPTable  to be copied</param>
        public PdfPTable(PdfPTable table)
        {
            CopyFormat(table);
            for (int k = 0; k < CurrentRow.Length; ++k)
            {
                if (table.CurrentRow[k] == null)
                    break;
                CurrentRow[k] = new PdfPCell(table.CurrentRow[k]);
            }
            for (int k = 0; k < table.rows.Count; ++k)
            {
                PdfPRow row = (PdfPRow)(table.rows[k]);
                if (row != null)
                    row = new PdfPRow(row);
                rows.Add(row);
            }
        }

        protected PdfPTable()
        {
        }
        /// <summary>
        /// Gets the absolute sizes of each column width.
        /// </summary>
        /// <returns>he absolute sizes of each column width</returns>
        public float[] AbsoluteWidths
        {
            get
            {
                return absoluteWidths;
            }
        }

        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <returns>an  ArrayList </returns>
        public ArrayList Chunks
        {
            get
            {
                return new ArrayList();
            }
        }

        /// <summary>
        /// Gets the default  PdfPCell  that will be used as
        /// reference for all the  addCell  methods except
        ///  addCell(PdfPCell) .
        /// </summary>
        /// <returns>default  PdfPCell </returns>
        public PdfPCell DefaultCell
        {
            get
            {
                return defaultCell;
            }
        }

        /// <summary>
        /// @since   iText 2.0.8
        /// @see com.lowagie.text.LargeElement#isComplete()
        /// </summary>
        public bool ElementComplete
        {
            get
            {
                return Complete;
            }
            set
            {
                Complete = value;
            }
        }

        public bool ExtendLastRow
        {
            get
            {
                return _extendLastRow;
            }
            set
            {
                _extendLastRow = value;
            }
        }

        /// <summary>
        /// Gets the height of the rows that constitute the header as defined by
        ///  setFooterRows() .
        /// @since 2.1.1
        /// </summary>
        /// <returns>the height of the rows that constitute the footer</returns>
        public float FooterHeight
        {
            get
            {
                float total = 0;
                int start = Math.Max(0, headerRows - _footerRows);
                int size = Math.Min(rows.Count, headerRows);
                for (int k = start; k < size; ++k)
                {
                    PdfPRow row = (PdfPRow)rows[k];
                    if (row != null)
                        total += row.MaxHeights;
                }
                return total;
            }
        }

        public int FooterRows
        {
            get
            {
                return _footerRows;
            }
            set
            {
                _footerRows = value;
                if (_footerRows < 0)
                    _footerRows = 0;
            }
        }

        /// <summary>
        /// Gets the height of the rows that constitute the header as defined by
        ///  setHeaderRows() .
        /// </summary>
        /// <returns>the height of the rows that constitute the header and footer</returns>
        public float HeaderHeight
        {
            get
            {
                float total = 0;
                int size = Math.Min(rows.Count, headerRows);
                for (int k = 0; k < size; ++k)
                {
                    PdfPRow row = (PdfPRow)rows[k];
                    if (row != null)
                        total += row.MaxHeights;
                }
                return total;
            }
        }

        public int HeaderRows
        {
            get
            {
                return headerRows;
            }
            set
            {
                headerRows = value;
                if (headerRows < 0)
                    headerRows = 0;
            }
        }

        public bool HeadersInEvent
        {
            get
            {
                return _headersInEvent;
            }
            set
            {
                _headersInEvent = value;
            }
        }

        public int HorizontalAlignment
        {
            get
            {
                return _horizontalAlignment;
            }
            set
            {
                _horizontalAlignment = value;
            }
        }

        /// <summary>
        /// If true the table will be kept on one page if it fits, by forcing a
        /// new page if it doesn't fit on the current page. The default is to
        /// split the table over multiple pages.
        /// </summary>
        public bool KeepTogether
        {
            set
            {
                _keepTogether = value;
            }
            get
            {
                return _keepTogether;
            }
        }

        public bool LockedWidth
        {
            get
            {
                return _lockedWidth;
            }
            set
            {
                _lockedWidth = value;
            }
        }

        /// <summary>
        /// Returns the number of columns.
        /// @since   2.1.1
        /// </summary>
        /// <returns>the number of columns.</returns>
        public int NumberOfColumns
        {
            get
            {
                return RelativeWidths.Length;
            }
        }

        /// <summary>
        /// Gets an arraylist with all the rows in the table.
        /// </summary>
        /// <returns>an arraylist</returns>
        public ArrayList Rows
        {
            get
            {
                return rows;
            }
        }

        public int RunDirection
        {
            get
            {
                return runDirection;
            }
            set
            {
                switch (value)
                {
                    case PdfWriter.RUN_DIRECTION_DEFAULT:
                    case PdfWriter.RUN_DIRECTION_NO_BIDI:
                    case PdfWriter.RUN_DIRECTION_LTR:
                    case PdfWriter.RUN_DIRECTION_RTL:
                        runDirection = value;
                        break;
                    default:
                        throw new ArgumentException("Invalid run direction: " + runDirection);
                }
            }
        }

        /// <summary>
        /// Gets the number of rows in this table.
        /// </summary>
        /// <returns>the number of rows in this table</returns>
        public int Size
        {
            get
            {
                return rows.Count;
            }
        }

        public bool SkipFirstHeader
        {
            get
            {
                return _skipFirstHeader;
            }
            set
            {
                _skipFirstHeader = value;
            }
        }

        /// <summary>
        /// Tells you if the last footer needs to be skipped
        /// (for instance if the footer says "continued on the next page")
        /// @since   2.1.6
        /// </summary>
        /// <returns>Value of property skipLastFooter.</returns>
        public bool SkipLastFooter
        {
            get
            {
                return _skipLastFooter;
            }
            set
            {
                _skipLastFooter = value;
            }
        }

        public float SpacingAfter
        {
            get
            {
                return spacingAfter;
            }
            set
            {
                spacingAfter = value;
            }
        }

        public float SpacingBefore
        {
            get
            {
                return spacingBefore;
            }
            set
            {
                spacingBefore = value;
            }
        }

        public bool SplitLate
        {
            get
            {
                return _splitLate;
            }
            set
            {
                _splitLate = value;
            }
        }

        public bool SplitRows
        {
            get
            {
                return _splitRows;
            }
            set
            {
                _splitRows = value;
            }
        }

        public IPdfPTableEvent TableEvent
        {
            get
            {
                return tableEvent;
            }
            set
            {
                if (value == null) tableEvent = null;
                else if (tableEvent == null) tableEvent = value;
                else if (tableEvent is PdfPTableEventForwarder) ((PdfPTableEventForwarder)tableEvent).AddTableEvent(value);
                else
                {
                    PdfPTableEventForwarder forward = new PdfPTableEventForwarder();
                    forward.AddTableEvent(tableEvent);
                    forward.AddTableEvent(value);
                    tableEvent = forward;
                }
            }
        }

        /// <summary>
        /// Gets the total height of the table.
        /// </summary>
        /// <returns>the total height of the table</returns>
        public float TotalHeight
        {
            get
            {
                return totalHeight;
            }
        }

        /// <summary>
        /// Gets the full width of the table.
        /// </summary>
        /// <returns>the full width of the table</returns>
        public float TotalWidth
        {
            get
            {
                return totalWidth;
            }
            set
            {
                if (totalWidth.ApproxEquals(value))
                    return;
                totalWidth = value;
                totalHeight = 0;
                CalculateWidths();
                CalculateHeights(true);
            }
        }

        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <returns>a type</returns>
        public int Type
        {
            get
            {
                return Element.PTABLE;
            }
        }

        public float WidthPercentage
        {
            get
            {
                return widthPercentage;
            }
            set
            {
                widthPercentage = value;
            }
        }

        /// <summary>
        /// Gets and initializes the 4 layers where the table is written to. The text or graphics are added to
        /// one of the 4  PdfContentByte  returned with the following order:
        ///
        ///   PdfPtable.BASECANVAS  - the original  PdfContentByte . Anything placed here
        /// will be under the table.
        ///   PdfPtable.BACKGROUNDCANVAS  - the layer where the background goes to.
        ///   PdfPtable.LINECANVAS  - the layer where the lines go to.
        ///   PdfPtable.TEXTCANVAS  - the layer where the text go to. Anything placed here
        /// will be over the table.
        ///
        /// The layers are placed in sequence on top of each other.
        /// be written to
        /// @see #writeSelectedRows(int, int, float, float, PdfContentByte[])
        /// </summary>
        /// <param name="canvas">the  PdfContentByte  where the rows will</param>
        /// <returns>an array of 4  PdfContentByte </returns>
        public static PdfContentByte[] BeginWritingRows(PdfContentByte canvas)
        {
            return new[]{
                canvas,
                canvas.Duplicate,
                canvas.Duplicate,
                canvas.Duplicate,
            };
        }

        /// <summary>
        /// Finishes writing the table.
        /// </summary>
        /// <param name="canvases">the array returned by  beginWritingRows() </param>
        public static void EndWritingRows(PdfContentByte[] canvases)
        {
            PdfContentByte canvas = canvases[BASECANVAS];
            canvas.SaveState();
            canvas.Add(canvases[BACKGROUNDCANVAS]);
            canvas.RestoreState();
            canvas.SaveState();
            canvas.SetLineCap(2);
            canvas.ResetRgbColorStroke();
            canvas.Add(canvases[LINECANVAS]);
            canvas.RestoreState();
            canvas.Add(canvases[TEXTCANVAS]);
        }

        /// <summary>
        /// Makes a shallow copy of a table (format without content).
        /// </summary>
        /// <param name="table"></param>
        /// <returns>a shallow copy of the table</returns>
        public static PdfPTable ShallowCopy(PdfPTable table)
        {
            PdfPTable nt = new PdfPTable();
            nt.CopyFormat(table);
            return nt;
        }

        /// <summary>
        /// Adds a cell element.
        /// </summary>
        /// <param name="cell">the cell element</param>
        public void AddCell(PdfPCell cell)
        {
            RowCompleted = false;
            PdfPCell ncell = new PdfPCell(cell);

            int colspan = ncell.Colspan;
            colspan = Math.Max(colspan, 1);
            colspan = Math.Min(colspan, CurrentRow.Length - CurrentRowIdx);
            ncell.Colspan = colspan;

            if (colspan != 1)
                IsColspan = true;
            int rdir = ncell.RunDirection;
            if (rdir == PdfWriter.RUN_DIRECTION_DEFAULT)
                ncell.RunDirection = runDirection;

            skipColsWithRowspanAbove();

            bool cellAdded = false;
            if (CurrentRowIdx < CurrentRow.Length)
            {
                CurrentRow[CurrentRowIdx] = ncell;
                CurrentRowIdx += colspan;
                cellAdded = true;
            }

            skipColsWithRowspanAbove();

            if (CurrentRowIdx >= CurrentRow.Length)
            {
                int numCols = NumberOfColumns;
                if (runDirection == PdfWriter.RUN_DIRECTION_RTL)
                {
                    PdfPCell[] rtlRow = new PdfPCell[numCols];
                    int rev = CurrentRow.Length;
                    for (int k = 0; k < CurrentRow.Length; ++k)
                    {
                        PdfPCell rcell = CurrentRow[k];
                        int cspan = rcell.Colspan;
                        rev -= cspan;
                        rtlRow[rev] = rcell;
                        k += cspan - 1;
                    }
                    CurrentRow = rtlRow;
                }
                PdfPRow row = new PdfPRow(CurrentRow);
                if (totalWidth > 0)
                {
                    row.SetWidths(absoluteWidths);
                    totalHeight += row.MaxHeights;
                }
                rows.Add(row);
                CurrentRow = new PdfPCell[numCols];
                CurrentRowIdx = 0;
                RowCompleted = true;
            }

            if (!cellAdded)
            {
                CurrentRow[CurrentRowIdx] = ncell;
                CurrentRowIdx += colspan;
            }
        }

        /// <summary>
        /// Adds a cell element.
        /// </summary>
        /// <param name="text">the text for the cell</param>
        public void AddCell(string text)
        {
            AddCell(new Phrase(text));
        }

        /// <summary>
        /// Adds a nested table.
        /// </summary>
        /// <param name="table">the table to be added to the cell</param>
        public void AddCell(PdfPTable table)
        {
            defaultCell.Table = table;
            AddCell(defaultCell);
            defaultCell.Table = null;
        }

        /// <summary>
        /// Adds an Image as Cell.
        /// </summary>
        /// <param name="image">the  Image  to add to the table. This image will fit in the cell</param>
        public void AddCell(Image image)
        {
            defaultCell.Image = image;
            AddCell(defaultCell);
            defaultCell.Image = null;
        }

        /// <summary>
        /// Adds a cell element.
        /// </summary>
        /// <param name="phrase">the  Phrase  to be added to the cell</param>
        public void AddCell(Phrase phrase)
        {
            defaultCell.Phrase = phrase;
            AddCell(defaultCell);
            defaultCell.Phrase = null;
        }

        /// <summary>
        /// Calculates the heights of the table.
        /// This takes time; normally the heights of the rows are already calcultated,
        /// so in most cases, it's save to use false as parameter.
        /// specify the width of the table with SetTotalWidth().
        /// @since    2.1.5   added a parameter and a return type to an existing method,
        /// and made it public
        /// </summary>
        /// <param name="firsttime">if true, the heights of the rows will be recalculated.</param>
        /// <returns>the total height of the table. Note that it will be 0 if you didn't</returns>
        public float CalculateHeights(bool firsttime)
        {
            if (totalWidth <= 0)
                return 0;
            totalHeight = 0;
            for (int k = 0; k < rows.Count; ++k)
            {
                totalHeight += GetRowHeight(k, firsttime);
            }
            return totalHeight;
        }

        /// <summary>
        /// Calculates the heights of the table.
        /// </summary>
        public void CalculateHeightsFast()
        {
            CalculateHeights(false);
        }

        /// <summary>
        /// Completes the current row with the default cell. An incomplete row will be dropped
        /// but calling this method will make sure that it will be present in the table.
        /// </summary>
        public void CompleteRow()
        {
            while (!RowCompleted)
            {
                AddCell(defaultCell);
            }
        }

        /// <summary>
        /// Removes all of the rows except headers
        /// </summary>
        public void DeleteBodyRows()
        {
            ArrayList rows2 = new ArrayList();
            for (int k = 0; k < headerRows; ++k)
                rows2.Add(rows[k]);
            rows = rows2;
            totalHeight = 0;
            if (totalWidth > 0)
                totalHeight = HeaderHeight;
        }

        /// <summary>
        /// Deletes the last row in the table.
        /// </summary>
        /// <returns> true  if the last row was deleted</returns>
        public bool DeleteLastRow()
        {
            return DeleteRow(rows.Count - 1);
        }

        /// <summary>
        /// Deletes a row from the table.
        /// </summary>
        /// <param name="rowNumber">the row to be deleted</param>
        /// <returns> true  if the row was deleted</returns>
        public bool DeleteRow(int rowNumber)
        {
            if (rowNumber < 0 || rowNumber >= rows.Count)
            {
                return false;
            }
            if (totalWidth > 0)
            {
                PdfPRow row = (PdfPRow)rows[rowNumber];
                if (row != null)
                    totalHeight -= row.MaxHeights;
            }
            rows.RemoveAt(rowNumber);
            if (rowNumber < headerRows)
            {
                --headerRows;
                if (rowNumber >= (headerRows - _footerRows))
                    --_footerRows;
            }
            return true;
        }

        /// <summary>
        /// @since   iText 2.0.8
        /// @see com.lowagie.text.LargeElement#flushContent()
        /// </summary>
        public void FlushContent()
        {
            DeleteBodyRows();
            SkipFirstHeader = true;
        }

        /// <summary>
        /// Gets a row with a given index
        /// (added by Jin-Hsia Yang).
        /// </summary>
        /// <param name="idx"></param>
        /// <returns>the row at position idx</returns>
        public PdfPRow GetRow(int idx)
        {
            return (PdfPRow)rows[idx];
        }

        /// <summary>
        /// Gets the height of a particular row.
        /// </summary>
        /// <param name="idx">the row index (starts at 0)</param>
        /// <returns>the height of a particular row</returns>
        public float GetRowHeight(int idx)
        {
            return GetRowHeight(idx, false);
        }

        /// <summary>
        /// Gets the height of a particular row.
        /// @since    3.0.0
        /// </summary>
        /// <param name="idx">the row index (starts at 0)</param>
        /// <param name="firsttime">is this the first time the row heigh is calculated?</param>
        /// <returns>the height of a particular row</returns>
        public float GetRowHeight(int idx, bool firsttime)
        {
            if (totalWidth <= 0 || idx < 0 || idx >= rows.Count)
                return 0;
            PdfPRow row = (PdfPRow)rows[idx];
            if (row == null)
                return 0;
            if (firsttime)
                row.SetWidths(absoluteWidths);
            float height = row.MaxHeights;
            PdfPCell cell;
            PdfPRow tmprow;
            for (int i = 0; i < RelativeWidths.Length; i++)
            {
                if (!RowSpanAbove(idx, i))
                    continue;
                int rs = 1;
                while (RowSpanAbove(idx - rs, i))
                {
                    rs++;
                }
                tmprow = (PdfPRow)rows[idx - rs];
                cell = tmprow.GetCells()[i];
                float tmp = 0;
                if (cell.Rowspan == rs + 1)
                {
                    tmp = cell.GetMaxHeight();
                    while (rs > 0)
                    {
                        tmp -= GetRowHeight(idx - rs);
                        rs--;
                    }
                }
                if (tmp > height)
                    height = tmp;
            }
            row.MaxHeights = height;
            return height;
        }

        /// <summary>
        /// Gets an arraylist with a selection of rows.
        /// @since    2.1.6
        /// </summary>
        /// <param name="start">the first row in the selection</param>
        /// <param name="end">the first row that isn't part of the selection</param>
        /// <returns>a selection of rows</returns>
        public ArrayList GetRows(int start, int end)
        {
            ArrayList list = new ArrayList();
            if (start < 0 || end > Size)
            {
                return list;
            }
            PdfPRow firstRow = AdjustCellsInRow(start, end);
            int colIndex = 0;
            PdfPCell cell;
            while (colIndex < NumberOfColumns)
            {
                int rowIndex = start;
                while (RowSpanAbove(rowIndex--, colIndex))
                {
                    PdfPRow row = GetRow(rowIndex);
                    if (row != null)
                    {
                        PdfPCell replaceCell = row.GetCells()[colIndex];
                        if (replaceCell != null)
                        {
                            firstRow.GetCells()[colIndex] = new PdfPCell(replaceCell);
                            float extra = 0;
                            int stop = Math.Min(rowIndex + replaceCell.Rowspan, end);
                            for (int j = start + 1; j < stop; j++)
                            {
                                extra += GetRowHeight(j);
                            }
                            firstRow.SetExtraHeight(colIndex, extra);
                            float diff = GetRowspanHeight(rowIndex, colIndex)
                                - GetRowHeight(start) - extra;
                            firstRow.GetCells()[colIndex].ConsumeHeight(diff);
                        }
                    }
                }
                cell = firstRow.GetCells()[colIndex];
                if (cell == null)
                    colIndex++;
                else
                    colIndex += cell.Colspan;
            }
            list.Add(firstRow);
            for (int i = start + 1; i < end; i++)
            {
                list.Add(AdjustCellsInRow(i, end));
            }
            return list;
        }

        /// <summary>
        /// Gets the maximum height of a cell in a particular row (will only be different
        /// from getRowHeight is one of the cells in the row has a rowspan > 1).
        /// @since    2.1.6
        /// </summary>
        /// <param name="rowIndex">the row index</param>
        /// <param name="cellIndex">the cell index</param>
        /// <returns>the height of a particular row including rowspan</returns>
        public float GetRowspanHeight(int rowIndex, int cellIndex)
        {
            if (totalWidth <= 0 || rowIndex < 0 || rowIndex >= rows.Count)
                return 0;
            PdfPRow row = (PdfPRow)rows[rowIndex];
            if (row == null || cellIndex >= row.GetCells().Length)
                return 0;
            PdfPCell cell = row.GetCells()[cellIndex];
            if (cell == null)
                return 0;
            float rowspanHeight = 0;
            for (int j = 0; j < cell.Rowspan; j++)
            {
                rowspanHeight += GetRowHeight(rowIndex + j);
            }
            return rowspanHeight;
        }

        /// <summary>
        /// @see com.lowagie.text.Element#isContent()
        /// @since   iText 2.0.8
        /// </summary>
        public bool IsContent()
        {
            return true;
        }

        /// <summary>
        /// @see com.lowagie.text.Element#isNestable()
        /// @since   iText 2.0.8
        /// </summary>
        public bool IsNestable()
        {
            return true;
        }

        /// <summary>
        /// Processes the element by adding it (or the different parts) to an
        ///  ElementListener .
        /// </summary>
        /// <param name="listener">an  ElementListener </param>
        /// <returns> true  if the element was processed successfully</returns>
        public bool Process(IElementListener listener)
        {
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
        /// Sets the full width of the table from the absolute column width.
        /// @throws DocumentException if the number of widths is different than the number
        /// of columns
        /// </summary>
        /// <param name="columnWidth">the absolute width of each column</param>
        public void SetTotalWidth(float[] columnWidth)
        {
            if (columnWidth.Length != NumberOfColumns)
                throw new DocumentException("Wrong number of columns.");
            totalWidth = 0;
            for (int k = 0; k < columnWidth.Length; ++k)
                totalWidth += columnWidth[k];
            SetWidths(columnWidth);
        }

        /// <summary>
        /// Sets the percentage width of the table from the absolute column width.
        /// @throws DocumentException
        /// </summary>
        /// <param name="columnWidth">the absolute width of each column</param>
        /// <param name="pageSize">the page size</param>
        public void SetWidthPercentage(float[] columnWidth, Rectangle pageSize)
        {
            if (columnWidth.Length != NumberOfColumns)
                throw new ArgumentException("Wrong number of columns.");
            float localTotalWidth = 0;
            for (int k = 0; k < columnWidth.Length; ++k)
                localTotalWidth += columnWidth[k];
            widthPercentage = localTotalWidth / (pageSize.Right - pageSize.Left) * 100f;
            SetWidths(columnWidth);
        }

        /// <summary>
        /// Sets the relative widths of the table.
        /// @throws DocumentException if the number of widths is different than the number
        /// of columns
        /// </summary>
        /// <param name="relativeWidths">the relative widths of the table.</param>
        public void SetWidths(float[] relativeWidths)
        {
            if (relativeWidths.Length != NumberOfColumns)
                throw new DocumentException("Wrong number of columns.");
            RelativeWidths = new float[relativeWidths.Length];
            Array.Copy(relativeWidths, 0, RelativeWidths, 0, relativeWidths.Length);
            absoluteWidths = new float[relativeWidths.Length];
            totalHeight = 0;
            CalculateWidths();
            CalculateHeights(true);
        }

        /// <summary>
        /// Sets the relative widths of the table.
        /// @throws DocumentException if the number of widths is different than the number
        /// of columns
        /// </summary>
        /// <param name="relativeWidths">the relative widths of the table.</param>
        public void SetWidths(int[] relativeWidths)
        {
            float[] tb = new float[relativeWidths.Length];
            for (int k = 0; k < relativeWidths.Length; ++k)
                tb[k] = relativeWidths[k];
            SetWidths(tb);
        }

        /// <summary>
        /// Writes the selected rows to the document.
        ///
        ///  canvases  is obtained from  beginWritingRows() .
        /// rows to the end are written
        ///  beginWrittingRows()
        /// @see #beginWritingRows(com.lowagie.text.pdf.PdfContentByte)
        /// </summary>
        /// <param name="rowStart">the first row to be written, zero index</param>
        /// <param name="rowEnd">the last row to be written + 1. If it is -1 all the</param>
        /// <param name="xPos">the x write coodinate</param>
        /// <param name="yPos">the y write coodinate</param>
        /// <param name="canvases">an array of 4  PdfContentByte  obtained from</param>
        /// <returns>the y coordinate position of the bottom of the last row</returns>
        public float WriteSelectedRows(int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte[] canvases)
        {
            return WriteSelectedRows(0, -1, rowStart, rowEnd, xPos, yPos, canvases);
        }

        /// <summary>
        /// Writes the selected rows and columns to the document.
        /// This method does not clip the columns; this is only important
        /// if there are columns with colspan at boundaries.
        ///
        ///  canvases  is obtained from  beginWritingRows() .
        ///
        /// The table event is only fired for complete rows.
        /// columns to the end are written
        /// rows to the end are written
        ///  beginWrittingRows()
        /// @see #beginWritingRows(com.lowagie.text.pdf.PdfContentByte)
        /// </summary>
        /// <param name="colStart">the first column to be written, zero index</param>
        /// <param name="colEnd">the last column to be written + 1. If it is -1 all the</param>
        /// <param name="rowStart">the first row to be written, zero index</param>
        /// <param name="rowEnd">the last row to be written + 1. If it is -1 all the</param>
        /// <param name="xPos">the x write coodinate</param>
        /// <param name="yPos">the y write coodinate</param>
        /// <param name="canvases">an array of 4  PdfContentByte  obtained from</param>
        /// <returns>the y coordinate position of the bottom of the last row</returns>
        public float WriteSelectedRows(int colStart, int colEnd, int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte[] canvases)
        {
            if (totalWidth <= 0)
                throw new ArgumentException("The table width must be greater than zero.");
            int totalRows = rows.Count;
            if (rowStart < 0)
                rowStart = 0;
            if (rowEnd < 0)
                rowEnd = totalRows;
            else
                rowEnd = Math.Min(rowEnd, totalRows);
            if (rowStart >= rowEnd)
                return yPos;

            int totalCols = NumberOfColumns;
            if (colStart < 0)
                colStart = 0;
            else
                colStart = Math.Min(colStart, totalCols);
            if (colEnd < 0)
                colEnd = totalCols;
            else
                colEnd = Math.Min(colEnd, totalCols);
            float yPosStart = yPos;
            for (int k = rowStart; k < rowEnd; ++k)
            {
                PdfPRow row = (PdfPRow)rows[k];
                if (row != null)
                {
                    row.WriteCells(colStart, colEnd, xPos, yPos, canvases);
                    yPos -= row.MaxHeights;
                }
            }
            if (tableEvent != null && colStart == 0 && colEnd == totalCols)
            {
                float[] heights = new float[rowEnd - rowStart + 1];
                heights[0] = yPosStart;
                for (int k = rowStart; k < rowEnd; ++k)
                {
                    PdfPRow row = (PdfPRow)rows[k];
                    float hr = 0;
                    if (row != null)
                        hr = row.MaxHeights;
                    heights[k - rowStart + 1] = heights[k - rowStart] - hr;
                }
                tableEvent.TableLayout(this, GetEventWidths(xPos, rowStart, rowEnd, _headersInEvent), heights, _headersInEvent ? headerRows : 0, rowStart, canvases);
            }
            return yPos;
        }

        /// <summary>
        /// Writes the selected rows to the document.
        /// rows to the end are written
        /// be written to
        /// </summary>
        /// <param name="rowStart">the first row to be written, zero index</param>
        /// <param name="rowEnd">the last row to be written + 1. If it is -1 all the</param>
        /// <param name="xPos">the x write coodinate</param>
        /// <param name="yPos">the y write coodinate</param>
        /// <param name="canvas">the  PdfContentByte  where the rows will</param>
        /// <returns>the y coordinate position of the bottom of the last row</returns>
        public float WriteSelectedRows(int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte canvas)
        {
            return WriteSelectedRows(0, -1, rowStart, rowEnd, xPos, yPos, canvas);
        }

        /// <summary>
        /// Writes the selected rows to the document.
        /// This method clips the columns; this is only important
        /// if there are columns with colspan at boundaries.
        ///
        /// The table event is only fired for complete rows.
        /// rows to the end are written
        /// be written to
        /// </summary>
        /// <param name="colStart">the first column to be written, zero index</param>
        /// <param name="colEnd">the last column to be written + 1. If it is -1 all the</param>
        /// <param name="rowStart">the first row to be written, zero index</param>
        /// <param name="rowEnd">the last row to be written + 1. If it is -1 all the</param>
        /// <param name="xPos">the x write coodinate</param>
        /// <param name="yPos">the y write coodinate</param>
        /// <param name="canvas">the  PdfContentByte  where the rows will</param>
        /// <returns>the y coordinate position of the bottom of the last row</returns>
        public float WriteSelectedRows(int colStart, int colEnd, int rowStart, int rowEnd, float xPos, float yPos, PdfContentByte canvas)
        {
            int totalCols = NumberOfColumns;
            if (colStart < 0)
                colStart = 0;
            else
                colStart = Math.Min(colStart, totalCols);

            if (colEnd < 0)
                colEnd = totalCols;
            else
                colEnd = Math.Min(colEnd, totalCols);

            bool clip = (colStart != 0 || colEnd != totalCols);

            if (clip)
            {
                float w = 0;
                for (int k = colStart; k < colEnd; ++k)
                    w += absoluteWidths[k];
                canvas.SaveState();
                float lx = (colStart == 0) ? 10000 : 0;
                float rx = (colEnd == totalCols) ? 10000 : 0;
                canvas.Rectangle(xPos - lx, -10000, w + lx + rx, PdfPRow.RIGHT_LIMIT);
                canvas.Clip();
                canvas.NewPath();
            }

            PdfContentByte[] canvases = BeginWritingRows(canvas);
            float y = WriteSelectedRows(colStart, colEnd, rowStart, rowEnd, xPos, yPos, canvases);
            EndWritingRows(canvases);

            if (clip)
                canvas.RestoreState();

            return y;
        }

        internal float[][] GetEventWidths(float xPos, int firstRow, int lastRow, bool includeHeaders)
        {
            if (includeHeaders)
            {
                firstRow = Math.Max(firstRow, headerRows);
                lastRow = Math.Max(lastRow, headerRows);
            }
            float[][] widths = new float[(includeHeaders ? headerRows : 0) + lastRow - firstRow][];
            if (IsColspan)
            {
                int n = 0;
                if (includeHeaders)
                {
                    for (int k = 0; k < headerRows; ++k)
                    {
                        PdfPRow row = (PdfPRow)rows[k];
                        if (row == null)
                            ++n;
                        else
                            widths[n++] = row.GetEventWidth(xPos);
                    }
                }
                for (; firstRow < lastRow; ++firstRow)
                {
                    PdfPRow row = (PdfPRow)rows[firstRow];
                    if (row == null)
                        ++n;
                    else
                        widths[n++] = row.GetEventWidth(xPos);
                }
            }
            else
            {
                int numCols = NumberOfColumns;
                float[] width = new float[numCols + 1];
                width[0] = xPos;
                for (int k = 0; k < numCols; ++k)
                    width[k + 1] = width[k] + absoluteWidths[k];
                for (int k = 0; k < widths.Length; ++k)
                    widths[k] = width;
            }
            return widths;
        }

        /// <summary>
        /// Checks if there are rows above belonging to a rowspan.
        /// @since    2.1.6
        /// </summary>
        /// <param name="currRow">the current row to check</param>
        /// <param name="currCol">the current column to check</param>
        /// <returns>true if there's a cell above that belongs to a rowspan</returns>
        internal bool RowSpanAbove(int currRow, int currCol)
        {

            if ((currCol >= NumberOfColumns)
                    || (currCol < 0)
                    || (currRow == 0))
                return false;

            int row = currRow - 1;
            PdfPRow aboveRow = (PdfPRow)rows[row];
            if (aboveRow == null)
                return false;
            PdfPCell aboveCell = aboveRow.GetCells()[currCol];
            while ((aboveCell == null) && (row > 0))
            {
                aboveRow = (PdfPRow)rows[--row];
                aboveCell = aboveRow.GetCells()[currCol];
            }

            int distance = currRow - row;

            if (aboveCell == null)
            {
                int col = currCol - 1;
                aboveCell = aboveRow.GetCells()[col];
                while ((aboveCell == null) && (row > 0))
                    aboveCell = aboveRow.GetCells()[--col];
                return aboveCell != null && aboveCell.Rowspan > distance;
            }

            if ((aboveCell.Rowspan == 1) && (distance > 1))
            {
                int col = currCol - 1;
                aboveRow = (PdfPRow)rows[row + 1];
                distance--;
                aboveCell = aboveRow.GetCells()[col];
                while ((aboveCell == null) && (col > 0))
                    aboveCell = aboveRow.GetCells()[--col];
            }

            return aboveCell != null && aboveCell.Rowspan > distance;
        }

        protected internal void CalculateWidths()
        {
            if (totalWidth <= 0)
                return;
            float total = 0;
            int numCols = NumberOfColumns;
            for (int k = 0; k < numCols; ++k)
                total += RelativeWidths[k];
            for (int k = 0; k < numCols; ++k)
                absoluteWidths[k] = totalWidth * RelativeWidths[k] / total;
        }

        /// <summary>
        /// Copies the format of the sourceTable without copying the content.
        /// </summary>
        /// <param name="sourceTable"></param>
        protected internal void CopyFormat(PdfPTable sourceTable)
        {
            RelativeWidths = new float[sourceTable.NumberOfColumns];
            absoluteWidths = new float[sourceTable.NumberOfColumns];
            Array.Copy(sourceTable.RelativeWidths, 0, RelativeWidths, 0, NumberOfColumns);
            Array.Copy(sourceTable.absoluteWidths, 0, absoluteWidths, 0, NumberOfColumns);
            totalWidth = sourceTable.totalWidth;
            totalHeight = sourceTable.totalHeight;
            CurrentRowIdx = 0;
            tableEvent = sourceTable.tableEvent;
            runDirection = sourceTable.runDirection;
            defaultCell = new PdfPCell(sourceTable.defaultCell);
            CurrentRow = new PdfPCell[sourceTable.CurrentRow.Length];
            IsColspan = sourceTable.IsColspan;
            _splitRows = sourceTable._splitRows;
            spacingAfter = sourceTable.spacingAfter;
            spacingBefore = sourceTable.spacingBefore;
            headerRows = sourceTable.headerRows;
            _footerRows = sourceTable._footerRows;
            _lockedWidth = sourceTable._lockedWidth;
            _extendLastRow = sourceTable._extendLastRow;
            _headersInEvent = sourceTable._headersInEvent;
            widthPercentage = sourceTable.widthPercentage;
            _splitLate = sourceTable._splitLate;
            _skipFirstHeader = sourceTable._skipFirstHeader;
            _skipLastFooter = sourceTable._skipLastFooter;
            _horizontalAlignment = sourceTable._horizontalAlignment;
            _keepTogether = sourceTable._keepTogether;
            Complete = sourceTable.Complete;
        }
        /// <summary>
        /// Calculates the extra height needed in a row because of rowspans.
        /// @since    2.1.6
        /// </summary>
        /// <param name="start">the index of the start row (the one to adjust)</param>
        /// <param name="end">the index of the end row on the page</param>
        protected PdfPRow AdjustCellsInRow(int start, int end)
        {
            PdfPRow row = new PdfPRow(GetRow(start));
            row.InitExtraHeights();
            PdfPCell cell;
            PdfPCell[] cells = row.GetCells();
            for (int i = 0; i < cells.Length; i++)
            {
                cell = cells[i];
                if (cell == null || cell.Rowspan == 1)
                    continue;
                int stop = Math.Min(end, start + cell.Rowspan);
                float extra = 0;
                for (int k = start + 1; k < stop; k++)
                {
                    extra += GetRowHeight(k);
                }
                row.SetExtraHeight(i, extra);
            }
            return row;
        }

        /// <summary>
        /// When updating the row index, cells with rowspan should be taken into account.
        /// This is what happens in this method.
        /// @since    2.1.6
        /// </summary>
        private void skipColsWithRowspanAbove()
        {
            int direction = 1;
            if (runDirection == PdfWriter.RUN_DIRECTION_RTL)
                direction = -1;
            while (RowSpanAbove(rows.Count, CurrentRowIdx))
                CurrentRowIdx += direction;
        }
    }
}