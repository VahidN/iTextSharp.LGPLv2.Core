using System;
using System.Collections;
using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text
{
    /// <summary>
    /// A Cell is a Rectangle containing other Elements.
    /// </summary>
    /// <remarks>
    /// A Cell is a Rectangle containing other
    /// Elements.
    ///
    /// A Cell must be added to a Table.
    /// The Table will place the Cell in
    /// a Row.
    /// </remarks>
    /// <example>
    ///
    /// Table table = new Table(3);
    /// table.SetBorderWidth(1);
    /// table.SetBorderColor(new Color(0, 0, 255));
    /// table.SetCellpadding(5);
    /// table.SetCellspacing(5);
    ///  Cell cell = new Cell("header");
    /// cell.SetHeader(true);
    /// cell.SetColspan(3);
    /// table.AddCell(cell);
    ///  cell = new Cell("example cell with colspan 1 and rowspan 2");
    /// cell.SetRowspan(2);
    /// cell.SetBorderColor(new Color(255, 0, 0));
    /// table.AddCell(cell);
    /// table.AddCell("1.1");
    /// table.AddCell("2.1");
    /// table.AddCell("1.2");
    /// table.AddCell("2.2");
    ///
    /// </example>
    /// <seealso cref="T:iTextSharp.text.Rectangle"/>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Table"/>
    /// <seealso cref="T:iTextSharp.text.Row"/>
    public class Cell : Rectangle, ITextElementArray
    {

        /// <summary>
        /// static membervariable
        /// </summary>

        ///<summary> This is the ArrayList of Elements. </summary>
        protected ArrayList ArrayList;

        ///<summary> This is the colspan. </summary>
        protected int colspan = 1;

        ///<summary>Does this  Cell  force a group change? </summary>
        protected bool groupChange = true;

        ///<summary> Is this Cell a header? </summary>
        protected bool header;

        /// <summary>
        /// membervariables
        /// </summary>
        ///<summary> This is the horizontal Element. </summary>
        protected int horizontalAlignment = ALIGN_UNDEFINED;

        /// <summary>
        /// Maximum number of lines allowed in the cell.
        /// The default value of this property is not to limit the maximum number of lines
        /// (contributed by dperezcar@fcc.es)
        /// </summary>
        protected int maxLines = int.MaxValue;

        ///<summary> Will the element have to be wrapped? </summary>
        protected bool noWrap;

        protected bool Percentage;

        ///<summary> This is the rowspan. </summary>
        protected int rowspan = 1;

        /// <summary>
        /// Indicates that the largest ascender height should be used to determine the
        /// height of the first line.  Note that this only has an effect when rendered
        /// to PDF.  Setting this to true can help with vertical alignment problems.
        /// </summary>
        protected bool useAscender;

        /// <summary>
        /// Adjusts the cell contents to compensate for border widths.  Note that
        /// this only has an effect when rendered to PDF.
        /// </summary>
        protected bool useBorderPadding;

        /// <summary>
        /// Indicates that the largest descender height should be added to the height of
        /// the last line (so characters like y don't dip into the border).   Note that
        /// this only has an effect when rendered to PDF.
        /// </summary>
        protected bool useDescender;

        ///<summary> This is the vertical Element. </summary>
        protected int verticalAlignment = ALIGN_UNDEFINED;

        ///<summary> This is the vertical Element. </summary>
        protected float width;

        ///<summary> This is the leading. </summary>
        float _leading = float.NaN;

        /// <summary>
        /// If a truncation happens due to the {@link #maxLines} property, then this text will
        /// be added to indicate a truncation has happened.
        /// Default value is null, and means avoiding marking the truncation.
        /// A useful value of this property could be e.g. "..."
        /// (contributed by dperezcar@fcc.es)
        /// </summary>
        private string _showTruncation;

        /// <summary>
        /// Constructs an empty Cell.
        /// </summary>
        /// <summary>
        /// Constructs an empty Cell.
        /// </summary>
        /// <overloads>
        /// Has five overloads.
        /// </overloads>
        public Cell() : base(0, 0, 0, 0)
        {
            // creates a Rectangle with BY DEFAULT a border of 0.5

            Border = UNDEFINED;
            BorderWidth = 0.5F;

            // initializes the arraylist and adds an element
            ArrayList = new ArrayList();
        }

        /// <summary>
        /// constructors
        /// </summary>
        /// <summary>
        /// Constructs an empty Cell (for internal use only).
        /// </summary>
        /// <param name="dummy">a dummy value</param>
        public Cell(bool dummy) : this()
        {
            ArrayList.Add(new Paragraph(0));
        }

        /// <summary>
        /// Constructs a Cell with a certain content.
        /// </summary>
        /// <remarks>
        /// The string will be converted into a Paragraph.
        /// </remarks>
        /// <param name="content">a string</param>
        public Cell(string content) : this()
        {
            AddElement(new Paragraph(content));
        }

        /// <summary>
        /// Constructs a Cell with a certain Element.
        /// </summary>
        /// <remarks>
        /// if the element is a ListItem, Row or
        /// Cell, an exception will be thrown.
        /// </remarks>
        /// <param name="element">the element</param>
        public Cell(IElement element) : this()
        {
            if (element is Phrase)
            {
                Leading = ((Phrase)element).Leading;
            }
            AddElement(element);
        }

        public static Cell DummyCell
        {
            get
            {
                Cell cell = new Cell(true);
                cell.Colspan = 3;
                cell.Border = NO_BORDER;
                return cell;
            }
        }
        /// <summary>
        /// implementation of the Element-methods
        /// </summary>

        /// <summary>
        /// This property throws an Exception.
        /// </summary>
        /// <value>none</value>
        public override float Bottom
        {
            get
            {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }

            set
            {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }
        }

        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <value>an ArrayList</value>
        public override ArrayList Chunks
        {
            get
            {
                ArrayList tmp = new ArrayList();
                foreach (IElement ele in ArrayList)
                {
                    tmp.AddRange(ele.Chunks);
                }
                return tmp;
            }
        }

        /// <summary>
        /// Gets the colspan.
        /// </summary>
        /// <returns>a value</returns>
        /// <summary>
        /// Gets/sets the colspan.
        /// </summary>
        /// <value>a value</value>
        public int Colspan
        {
            get
            {
                return colspan;
            }

            set
            {
                colspan = value;
            }
        }

        /// <summary>
        /// Gets Elements.
        /// </summary>
        /// <value>an ArrayList</value>
        public ArrayList Elements
        {
            get
            {
                return ArrayList;
            }
        }

        /// <summary>
        /// Does this  Cell  force a group change?
        /// </summary>
        public bool GroupChange
        {
            get
            {
                return groupChange;
            }

            set
            {
                groupChange = value;
            }
        }

        /// <summary>
        /// Gets/sets header
        /// </summary>
        /// <value>a value</value>
        public bool Header
        {
            get
            {
                return header;
            }

            set
            {
                header = value;
            }
        }

        /// <summary>
        /// Gets/Sets the horizontal Element.
        /// </summary>
        /// <value>a value</value>
        public int HorizontalAlignment
        {
            get
            {
                return horizontalAlignment;
            }

            set
            {
                horizontalAlignment = value;
            }
        }

        /// <summary>
        /// Gets/sets the leading.
        /// </summary>
        /// <value>a value</value>
        public float Leading
        {
            get
            {
                if (float.IsNaN(_leading))
                {
                    return 16;
                }
                return _leading;
            }

            set
            {
                _leading = value;
            }
        }

        /// <summary>
        /// This property throws an Exception.
        /// </summary>
        /// <value>none</value>
        public override float Left
        {
            get
            {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }

            set
            {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }
        }

        /// <summary>
        /// get/set maxLines value
        /// </summary>
        public int MaxLines
        {
            get
            {
                return maxLines;
            }

            set
            {
                maxLines = value;
            }
        }

        /// <summary>
        /// Get nowrap.
        /// </summary>
        /// <returns>a value</returns>
        /// <summary>
        /// Get/set nowrap.
        /// </summary>
        /// <value>a value</value>
        public bool NoWrap
        {
            get
            {
                return (maxLines == 1);
            }

            set
            {
                maxLines = 1;
            }
        }

        /// <summary>
        /// This property throws an Exception.
        /// </summary>
        /// <value>none</value>
        public override float Right
        {
            get
            {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }

            set
            {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }
        }

        /// <summary>
        /// Gets/sets the rowspan.
        /// </summary>
        /// <value>a value</value>
        public int Rowspan
        {
            get
            {
                return rowspan;
            }

            set
            {
                rowspan = value;
            }
        }

        /// <summary>
        /// get/set showTruncation value
        /// </summary>
        public string ShowTruncation
        {
            get
            {
                return _showTruncation;
            }

            set
            {
                _showTruncation = value;
            }
        }

        /// <summary>
        /// Gets the number of Elements in the Cell.
        /// </summary>
        /// <value>a size</value>
        public int Size
        {
            get
            {
                return ArrayList.Count;
            }
        }

        /// <summary>
        /// This property throws an Exception.
        /// </summary>
        /// <value>none</value>
        public override float Top
        {
            get
            {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }

            set
            {
                throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
            }
        }

        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public override int Type
        {
            get
            {
                return CELL;
            }
        }

        /// <summary>
        /// get/set useAscender value
        /// </summary>
        public bool UseAscender
        {
            get
            {
                return useAscender;
            }

            set
            {
                useAscender = value;
            }
        }

        /// <summary>
        /// get/set useBorderPadding value
        /// </summary>
        public bool UseBorderPadding
        {
            get
            {
                return useBorderPadding;
            }

            set
            {
                useBorderPadding = value;
            }
        }

        /// <summary>
        /// get/set useDescender value
        /// </summary>
        public bool UseDescender
        {
            get
            {
                return useDescender;
            }

            set
            {
                useDescender = value;
            }
        }

        /// <summary>
        /// Gets/sets the vertical Element.
        /// </summary>
        /// <value>a value</value>
        public int VerticalAlignment
        {
            get
            {
                return verticalAlignment;
            }

            set
            {
                verticalAlignment = value;
            }
        }

        /// <summary>
        /// Sets the width.
        /// </summary>
        /// <value>the new value</value>
        public override float Width
        {
            set
            {
                width = value;
            }
            get
            {
                return width;
            }
        }

        /// <summary>
        /// Checks if a given tag corresponds with this object.
        /// </summary>
        /// <param name="tag">the given tag</param>
        /// <returns>true if the tag corresponds</returns>
        public static bool IsTag(string tag)
        {
            return ElementTags.CELL.Equals(tag);
        }

        /// <summary>
        /// Add an Object to this cell.
        /// </summary>
        /// <param name="o">the object to add</param>
        /// <returns>always true</returns>
        public bool Add(object o)
        {
            try
            {
                AddElement((IElement)o);
                return true;
            }
            catch (BadElementException bee)
            {
                throw new Exception(bee.Message);
            }
            catch
            {
                throw new Exception("You can only add objects that implement the Element interface.");
            }
        }

        /// <summary>
        /// Adds an element to this Cell.
        /// </summary>
        /// <remarks>
        /// You can't add ListItems, Rows, Cells,
        /// JPEGs, GIFs or PNGs to a Cell.
        /// </remarks>
        /// <param name="element">the Element to add</param>
        public void AddElement(IElement element)
        {
            if (IsTable())
            {
                Table table = (Table)ArrayList[0];
                Cell tmp = new Cell(element);
                tmp.Border = NO_BORDER;
                tmp.Colspan = table.Columns;
                table.AddCell(tmp);
                return;
            }
            switch (element.Type)
            {
                case LISTITEM:
                case ROW:
                case CELL:
                    throw new BadElementException("You can't add listitems, rows or cells to a cell.");
                case JPEG:
                case IMGRAW:
                case IMGTEMPLATE:
                    ArrayList.Add(element);
                    break;
                case LIST:
                    if (float.IsNaN(Leading))
                    {
                        _leading = ((List)element).TotalLeading;
                    }
                    if (((List)element).IsEmpty()) return;
                    ArrayList.Add(element);
                    return;
                case ANCHOR:
                case PARAGRAPH:
                case PHRASE:
                    if (float.IsNaN(_leading))
                    {
                        _leading = ((Phrase)element).Leading;
                    }
                    if (((Phrase)element).IsEmpty()) return;
                    ArrayList.Add(element);
                    return;
                case CHUNK:
                    if (((Chunk)element).IsEmpty()) return;
                    ArrayList.Add(element);
                    return;
                case TABLE:
                    Table table = new Table(3);
                    float[] widths = new float[3];
                    widths[1] = ((Table)element).Width;

                    switch (((Table)element).Alignment)
                    {
                        case ALIGN_LEFT:
                            widths[0] = 0f;
                            widths[2] = 100f - widths[1];
                            break;
                        case ALIGN_CENTER:
                            widths[0] = (100f - widths[1]) / 2f;
                            widths[2] = widths[0];
                            break;
                        case ALIGN_RIGHT:
                            widths[0] = 100f - widths[1];
                            widths[2] = 0f;
                            break;
                    }
                    table.Widths = widths;
                    Cell tmp;
                    if (ArrayList.Count == 0)
                    {
                        table.AddCell(DummyCell);
                    }
                    else
                    {
                        tmp = new Cell();
                        tmp.Border = NO_BORDER;
                        tmp.Colspan = 3;
                        foreach (IElement ele in ArrayList)
                        {
                            tmp.Add(ele);
                        }
                        table.AddCell(tmp);
                    }
                    tmp = new Cell();
                    tmp.Border = NO_BORDER;
                    table.AddCell(tmp);
                    table.InsertTable((Table)element);
                    tmp = new Cell();
                    tmp.Border = NO_BORDER;
                    table.AddCell(tmp);
                    table.AddCell(DummyCell);
                    Clear();
                    ArrayList.Add(table);
                    return;
                default:
                    ArrayList.Add(element);
                    break;
            }
        }

        /// <summary>
        /// Clears all the Elements of this Cell.
        /// </summary>
        public void Clear()
        {
            ArrayList.Clear();
        }

        /// <summary>
        /// Creates a PdfPCell based on this Cell object.
        /// @throws BadElementException
        /// </summary>
        /// <returns>a PdfPCell</returns>
        public PdfPCell CreatePdfPCell()
        {
            if (rowspan > 1) throw new BadElementException("PdfPCells can't have a rowspan > 1");
            if (IsTable()) return new PdfPCell(((Table)ArrayList[0]).CreatePdfPTable());
            PdfPCell cell = new PdfPCell();
            cell.VerticalAlignment = verticalAlignment;
            cell.HorizontalAlignment = horizontalAlignment;
            cell.Colspan = colspan;
            cell.UseBorderPadding = useBorderPadding;
            cell.UseDescender = useDescender;
            cell.SetLeading(Leading, 0);
            cell.CloneNonPositionParameters(this);
            cell.NoWrap = noWrap;
            foreach (IElement i in Elements)
            {
                if (i.Type == PHRASE || i.Type == PARAGRAPH)
                {
                    Paragraph p = new Paragraph((Phrase)i);
                    p.Alignment = horizontalAlignment;
                    cell.AddElement(p);
                }
                else
                    cell.AddElement(i);
            }
            return cell;
        }

        /// <summary>
        /// This method throws an Exception.
        /// </summary>
        /// <param name="margin">new value</param>
        /// <returns>none</returns>
        public float GetBottom(int margin)
        {
            throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
        }

        /// <summary>
        /// This method throws an Exception.
        /// </summary>
        /// <param name="margin">new value</param>
        /// <returns>none</returns>
        public float GetLeft(int margin)
        {
            throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
        }

        /// <summary>
        /// This method throws an Exception.
        /// </summary>
        /// <param name="margin">new value</param>
        /// <returns>none</returns>
        public float GetRight(int margin)
        {
            throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
        }

        /// <summary>
        /// This method throws an Exception.
        /// </summary>
        /// <param name="margin">new value</param>
        /// <returns>none</returns>
        public float GetTop(int margin)
        {
            throw new Exception("Dimensions of a Cell can't be calculated. See the FAQ.");
        }

        /// <summary>
        /// Gets the width as a String.
        /// </summary>
        /// <returns>a value</returns>
        public string GetWidthAsString()
        {
            string w = width.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (w.EndsWith(".0")) w = w.Substring(0, w.Length - 2);
            if (Percentage) w += "%";
            return w;
        }

        /// <summary>
        /// methods to retrieve information
        /// </summary>
        /// <summary>
        /// Checks if the Cell is empty.
        /// </summary>
        /// <returns>false if there are non-empty Elements in the Cell.</returns>
        public bool IsEmpty()
        {
            switch (Size)
            {
                case 0:
                    return true;
                case 1:
                    IElement element = (IElement)ArrayList[0];
                    switch (element.Type)
                    {
                        case CHUNK:
                            return ((Chunk)element).IsEmpty();
                        case ANCHOR:
                        case PHRASE:
                        case PARAGRAPH:
                            return ((Phrase)element).IsEmpty();
                        case LIST:
                            return ((List)element).IsEmpty();
                    }
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the Cell is empty.
        /// </summary>
        /// <returns>false if there are non-empty Elements in the Cell.</returns>
        public bool IsTable()
        {
            return (Size == 1) && (((IElement)ArrayList[0]).Type == TABLE);
        }

        /// <summary>
        /// Processes the element by adding it (or the different parts) to an
        /// IElementListener.
        /// </summary>
        /// <param name="listener">an IElementListener</param>
        /// <returns>true if the element was processed successfully</returns>
        public override bool Process(IElementListener listener)
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
        /// methods to set the membervariables
        /// </summary>
        /// <summary>
        /// Sets the alignment of this cell.
        /// </summary>
        /// <param name="alignment">the new alignment as a string</param>
        public void SetHorizontalAlignment(string alignment)
        {
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_CENTER))
            {
                HorizontalAlignment = ALIGN_CENTER;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_RIGHT))
            {
                HorizontalAlignment = ALIGN_RIGHT;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED))
            {
                HorizontalAlignment = ALIGN_JUSTIFIED;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED_ALL))
            {
                HorizontalAlignment = ALIGN_JUSTIFIED_ALL;
                return;
            }
            HorizontalAlignment = ALIGN_LEFT;
        }

        /// <summary>
        /// Sets the alignment of this paragraph.
        /// </summary>
        /// <param name="alignment">the new alignment as a string</param>
        public void SetVerticalAlignment(string alignment)
        {
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_MIDDLE))
            {
                VerticalAlignment = ALIGN_MIDDLE;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_BOTTOM))
            {
                VerticalAlignment = ALIGN_BOTTOM;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_BASELINE))
            {
                VerticalAlignment = ALIGN_BASELINE;
                return;
            }
            VerticalAlignment = ALIGN_TOP;
        }
        /// <summary>
        /// Sets the width.
        /// It can be an absolute value "100" or a percentage "20%"
        /// </summary>
        /// <param name="value">the new value</param>
        public void SetWidth(string value)
        {
            if (value.EndsWith("%"))
            {
                value = value.Substring(0, value.Length - 1);
                Percentage = true;
            }
            width = int.Parse(value);
        }
        /// <summary>
        /// Makes sure there is at least 1 object in the Cell.
        /// Otherwise it might not be shown in the table.
        /// </summary>
        internal void Fill()
        {
            if (Size == 0) ArrayList.Add(new Paragraph(0));
        }
    }
}
