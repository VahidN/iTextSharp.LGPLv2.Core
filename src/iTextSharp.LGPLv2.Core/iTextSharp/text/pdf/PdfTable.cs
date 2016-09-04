using System;
using System.Collections;

namespace iTextSharp.text.pdf
{

    /// <summary>
    ///  PdfTable  is an object that contains the graphics and text of a table.
    /// @see     iTextSharp.text.Table
    /// @see     iTextSharp.text.Row
    /// @see     iTextSharp.text.Cell
    /// @see     PdfCell
    /// </summary>
    public class PdfTable : Rectangle
    {

        /// <summary>
        /// membervariables
        /// </summary>

        /// <summary>
        /// Cached column widths.
        /// </summary>
        protected float[] Positions;

        /// <summary>
        /// Original table used to build this object
        /// </summary>
        protected Table Table;

        /// <summary>
        /// this is the ArrayList with all the cells in the table.
        /// </summary>
        private readonly ArrayList _cells;

        /// <summary>
        /// this is the number of columns in the table.
        /// </summary>
        private readonly int _columns;

        /// <summary>
        /// this is the ArrayList with all the cell of the table header.
        /// </summary>
        private readonly ArrayList _headercells;
        /// <summary>
        /// constructors
        /// </summary>

        /// <summary>
        /// Constructs a  PdfTable -object.
        /// </summary>
        /// <param name="table">a  Table </param>
        /// <param name="left">the left border on the page</param>
        /// <param name="right">the right border on the page</param>
        /// <param name="top">the start position of the top of the table</param>

        internal PdfTable(Table table, float left, float right, float top) : base(left, top, right, top)
        {
            // constructs a Rectangle (the bottomvalue will be changed afterwards)
            Table = table;
            table.Complete();

            // copying the attributes from class Table
            CloneNonPositionParameters(table);

            _columns = table.Columns;
            Positions = table.GetWidths(left, right - left);

            // initialisation of some parameters
            Left = Positions[0];
            Right = Positions[Positions.Length - 1];

            _headercells = new ArrayList();
            _cells = new ArrayList();

            updateRowAdditionsInternal();
        }

        /// <summary>
        /// methods
        /// </summary>

        /// <summary>
        /// Updates the table row additions in the underlying table object and deletes all table rows,
        /// in order to preserve memory and detect future row additions.
        ///  <b>Pre-requisite</b>: the object must have been built with the parameter  supportUpdateRowAdditions  equals to true.
        /// </summary>

        /// <summary>
        /// Gets the offset of this table.
        /// </summary>
        /// <returns>the space between this table and the previous element.</returns>
        public float Offset
        {
            get
            {
                return Table.Offset;
            }
        }

        /// <summary>
        /// @see com.lowagie.text.Element#type()
        /// </summary>
        public override int Type
        {
            get
            {
                return TABLE;
            }
        }

        internal float Cellpadding
        {
            get
            {
                return Table.Cellpadding;
            }
        }

        internal ArrayList Cells
        {
            get
            {
                return _cells;
            }
        }

        internal float Cellspacing
        {
            get
            {
                return Table.Cellspacing;
            }
        }

        internal int Columns
        {
            get
            {
                return _columns;
            }
        }

        internal ArrayList HeaderCells
        {
            get
            {
                return _headercells;
            }
        }

        internal int Rows
        {
            get
            {
                return _cells.Count == 0 ? 0 : ((PdfCell)_cells[_cells.Count - 1]).Rownumber + 1;
            }
        }

        public bool HasToFitPageCells()
        {
            return Table.CellsFitPage;
        }

        public bool HasToFitPageTable()
        {
            return Table.TableFitsPage;
        }

        internal bool HasHeader()
        {
            return _headercells.Count > 0;
        }

        internal void UpdateRowAdditions()
        {
            Table.Complete();
            updateRowAdditionsInternal();
            Table.DeleteAllRows();
        }

        /// <summary>
        /// Updates the table row additions in the underlying table object
        /// </summary>

        private void updateRowAdditionsInternal()
        {
            // correct table : fill empty cells/ parse table in table
            int prevRows = Rows;
            int rowNumber = 0;
            int groupNumber = 0;
            bool groupChange;
            int firstDataRow = Table.LastHeaderRow + 1;
            Cell cell;
            PdfCell currentCell;
            ArrayList newCells = new ArrayList();
            int rows = Table.Size + 1;
            float[] offsets = new float[rows];
            for (int i = 0; i < rows; i++)
            {
                offsets[i] = Bottom;
            }

            // loop over all the rows
            foreach (Row row in Table)
            {
                groupChange = false;
                if (row.IsEmpty())
                {
                    if (rowNumber < rows - 1 && offsets[rowNumber + 1] > offsets[rowNumber]) offsets[rowNumber + 1] = offsets[rowNumber];
                }
                else
                {
                    for (int i = 0; i < row.Columns; i++)
                    {
                        cell = (Cell)row.GetCell(i);
                        if (cell != null)
                        {
                            currentCell = new PdfCell(cell, rowNumber + prevRows, Positions[i], Positions[i + cell.Colspan], offsets[rowNumber], Cellspacing, Cellpadding);
                            if (rowNumber < firstDataRow)
                            {
                                currentCell.SetHeader();
                                _headercells.Add(currentCell);
                                if (!Table.NotAddedYet)
                                    continue;
                            }
                            try
                            {
                                if (offsets[rowNumber] - currentCell.Height - Cellpadding < offsets[rowNumber + currentCell.Rowspan])
                                {
                                    offsets[rowNumber + currentCell.Rowspan] = offsets[rowNumber] - currentCell.Height - Cellpadding;
                                }
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                if (offsets[rowNumber] - currentCell.Height < offsets[rows - 1])
                                {
                                    offsets[rows - 1] = offsets[rowNumber] - currentCell.Height;
                                }
                            }
                            currentCell.GroupNumber = groupNumber;
                            groupChange |= cell.GroupChange;
                            newCells.Add(currentCell);
                        }
                    }
                }
                rowNumber++;
                if (groupChange) groupNumber++;
            }

            // loop over all the cells
            int n = newCells.Count;
            for (int i = 0; i < n; i++)
            {
                currentCell = (PdfCell)newCells[i];
                try
                {
                    currentCell.Bottom = offsets[currentCell.Rownumber - prevRows + currentCell.Rowspan];
                }
                catch (ArgumentOutOfRangeException)
                {
                    currentCell.Bottom = offsets[rows - 1];
                }
            }
            _cells.AddRange(newCells);
            Bottom = offsets[rows - 1];
        }
    }
}