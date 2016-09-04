using System;
using System.Collections;
using iTextSharp.text.pdf;

namespace iTextSharp.text
{
    /// <summary>
    /// Summary description for SimpleCell.
    /// </summary>
    public class SimpleCell : Rectangle, IPdfPCellEvent, ITextElementArray
    {

        /// <summary>
        /// the CellAttributes object represents a cell.
        /// </summary>
        public new const bool CELL = false;

        /// <summary>
        /// the CellAttributes object represents a row.
        /// </summary>
        public new const bool ROW = true;
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

        /// <summary>
        /// the content of the Cell.
        /// </summary>
        private readonly ArrayList _content = new ArrayList();
        /// <summary>
        /// indicates if these are the attributes of a single Cell (false) or a group of Cells (true).
        /// </summary>
        private bool _cellgroup;

        /// <summary>
        /// the colspan of a Cell
        /// </summary>
        private int _colspan = 1;

        /// <summary>
        /// horizontal alignment inside the Cell.
        /// </summary>
        private int _horizontalAlignment = ALIGN_UNDEFINED;

        /// <summary>
        /// an extra padding variable
        /// </summary>
        private float _paddingBottom = float.NaN;

        /// <summary>
        /// an extra padding variable
        /// </summary>
        private float _paddingLeft = float.NaN;

        /// <summary>
        /// an extra padding variable
        /// </summary>
        private float _paddingRight = float.NaN;

        /// <summary>
        /// an extra padding variable
        /// </summary>
        private float _paddingTop = float.NaN;

        /// <summary>
        /// an extra spacing variable
        /// </summary>
        private float _spacingBottom = float.NaN;

        /// <summary>
        /// an extra spacing variable
        /// </summary>
        private float _spacingLeft = float.NaN;

        /// <summary>
        /// an extra spacing variable
        /// </summary>
        private float _spacingRight = float.NaN;

        /// <summary>
        /// an extra spacing variable
        /// </summary>
        private float _spacingTop = float.NaN;

        /// <summary>
        /// vertical alignment inside the Cell.
        /// </summary>
        private int _verticalAlignment = ALIGN_UNDEFINED;

        /// <summary>
        /// the width of the Cell.
        /// </summary>
        private float _width;
        /// <summary>
        /// the widthpercentage of the Cell.
        /// </summary>
        private float _widthpercentage;
        /// <summary>
        /// A CellAttributes object is always constructed without any dimensions.
        /// Dimensions are defined after creation.
        /// </summary>
        /// <param name="row">only true if the CellAttributes object represents a row.</param>
        public SimpleCell(bool row) : base(0f, 0f, 0f, 0f)
        {
            _cellgroup = row;
            Border = BOX;
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the cellgroup.</returns>
        public bool Cellgroup
        {
            get
            {
                return _cellgroup;
            }
            set
            {
                _cellgroup = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the colspan.</returns>
        public int Colspan
        {
            get
            {
                return _colspan;
            }
            set
            {
                if (value > 0) _colspan = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the horizontal alignment.</returns>
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
        /// Sets the padding parameters if they are undefined.
        /// </summary>
        public float Padding
        {
            set
            {
                if (float.IsNaN(_paddingRight))
                {
                    PaddingRight = value;
                }
                if (float.IsNaN(_paddingLeft))
                {
                    PaddingLeft = value;
                }
                if (float.IsNaN(_paddingBottom))
                {
                    PaddingBottom = value;
                }
                if (float.IsNaN(_paddingTop))
                {
                    PaddingTop = value;
                }
            }
        }

        /// <summary>
        /// </summary>
        public float PaddingBottom
        {
            get
            {
                return _paddingBottom;
            }
            set
            {
                _paddingBottom = value;
            }
        }

        public float PaddingLeft
        {
            get
            {
                return _paddingLeft;
            }
            set
            {
                _paddingLeft = value;
            }
        }

        public float PaddingRight
        {
            get
            {
                return _paddingRight;
            }
            set
            {
                _paddingRight = value;
            }
        }

        public float PaddingTop
        {
            get
            {
                return _paddingTop;
            }
            set
            {
                _paddingTop = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the spacing.</returns>
        public float Spacing
        {
            set
            {
                _spacingLeft = value;
                _spacingRight = value;
                _spacingTop = value;
                _spacingBottom = value;
            }

        }

        public float SpacingBottom
        {
            get
            {
                return _spacingBottom;
            }
            set
            {
                _spacingBottom = value;
            }
        }

        public float SpacingLeft
        {
            get
            {
                return _spacingLeft;
            }
            set
            {
                _spacingLeft = value;
            }
        }

        public float SpacingRight
        {
            get
            {
                return _spacingRight;
            }
            set
            {
                _spacingRight = value;
            }
        }

        public float SpacingTop
        {
            get
            {
                return _spacingTop;
            }
            set
            {
                _spacingTop = value;
            }
        }

        public override int Type
        {
            get
            {
                return Element.CELL;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the useAscender.</returns>
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
        /// </summary>
        /// <returns>Returns the useBorderPadding.</returns>
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

        public int VerticalAlignment
        {
            get
            {
                return _verticalAlignment;
            }
            set
            {
                _verticalAlignment = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the width.</returns>
        public new float Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the widthpercentage.</returns>
        public float Widthpercentage
        {
            get
            {
                return _widthpercentage;
            }
            set
            {
                _widthpercentage = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns the content.</returns>
        internal ArrayList Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// @see com.lowagie.text.TextElementArray#add(java.lang.Object)
        /// </summary>
        public bool Add(object o)
        {
            try
            {
                AddElement((IElement)o);
                return true;
            }
            catch (InvalidCastException)
            {
                return false;
            }
        }

        /// <summary>
        /// Adds content to this object.
        /// @throws BadElementException
        /// </summary>
        /// <param name="element"></param>
        public void AddElement(IElement element)
        {
            if (_cellgroup)
            {
                if (element is SimpleCell)
                {
                    if (((SimpleCell)element).Cellgroup)
                    {
                        throw new BadElementException("You can't add one row to another row.");
                    }
                    _content.Add(element);
                    return;
                }
                else
                {
                    throw new BadElementException("You can only add cells to rows, no objects of type " + element.GetType());
                }
            }
            if (element.Type == PARAGRAPH
                    || element.Type == PHRASE
                    || element.Type == ANCHOR
                    || element.Type == CHUNK
                    || element.Type == LIST
                    || element.Type == MARKED
                    || element.Type == JPEG
                    || element.Type == JPEG2000
                    || element.Type == JBIG2
                    || element.Type == IMGRAW
                    || element.Type == IMGTEMPLATE)
            {
                _content.Add(element);
            }
            else
            {
                throw new BadElementException("You can't add an element of type " + element.GetType() + " to a SimpleCell.");
            }
        }

        /// <summary>
        /// @see com.lowagie.text.pdf.PdfPCellEvent#cellLayout(com.lowagie.text.pdf.PdfPCell, com.lowagie.text.Rectangle, com.lowagie.text.pdf.PdfContentByte[])
        /// </summary>
        public void CellLayout(PdfPCell cell, Rectangle position, PdfContentByte[] canvases)
        {
            float spLeft = _spacingLeft;
            if (float.IsNaN(spLeft)) spLeft = 0f;
            float spRight = _spacingRight;
            if (float.IsNaN(spRight)) spRight = 0f;
            float spTop = _spacingTop;
            if (float.IsNaN(spTop)) spTop = 0f;
            float spBottom = _spacingBottom;
            if (float.IsNaN(spBottom)) spBottom = 0f;
            Rectangle rect = new Rectangle(position.GetLeft(spLeft), position.GetBottom(spBottom), position.GetRight(spRight), position.GetTop(spTop));
            rect.CloneNonPositionParameters(this);
            canvases[PdfPTable.BACKGROUNDCANVAS].Rectangle(rect);
            rect.BackgroundColor = null;
            canvases[PdfPTable.LINECANVAS].Rectangle(rect);
        }

        /// <summary>
        /// Creates a Cell with these attributes.
        /// @throws BadElementException
        /// </summary>
        /// <param name="rowAttributes"></param>
        /// <returns>a cell based on these attributes.</returns>
        public Cell CreateCell(SimpleCell rowAttributes)
        {
            Cell cell = new Cell();
            cell.CloneNonPositionParameters(rowAttributes);
            cell.SoftCloneNonPositionParameters(this);
            cell.Colspan = _colspan;
            cell.HorizontalAlignment = _horizontalAlignment;
            cell.VerticalAlignment = _verticalAlignment;
            cell.UseAscender = useAscender;
            cell.UseBorderPadding = useBorderPadding;
            cell.UseDescender = useDescender;
            foreach (IElement element in _content)
            {
                cell.AddElement(element);
            }
            return cell;
        }

        /// <summary>
        /// Creates a PdfPCell with these attributes.
        /// </summary>
        /// <param name="rowAttributes"></param>
        /// <returns>a PdfPCell based on these attributes.</returns>
        public PdfPCell CreatePdfPCell(SimpleCell rowAttributes)
        {
            PdfPCell cell = new PdfPCell();
            cell.Border = NO_BORDER;
            SimpleCell tmp = new SimpleCell(CELL);
            tmp.SpacingLeft = _spacingLeft;
            tmp.SpacingRight = _spacingRight;
            tmp.SpacingTop = _spacingTop;
            tmp.SpacingBottom = _spacingBottom;
            tmp.CloneNonPositionParameters(rowAttributes);
            tmp.SoftCloneNonPositionParameters(this);
            cell.CellEvent = tmp;
            cell.HorizontalAlignment = rowAttributes._horizontalAlignment;
            cell.VerticalAlignment = rowAttributes._verticalAlignment;
            cell.UseAscender = rowAttributes.useAscender;
            cell.UseBorderPadding = rowAttributes.useBorderPadding;
            cell.UseDescender = rowAttributes.useDescender;
            cell.Colspan = _colspan;
            if (_horizontalAlignment != ALIGN_UNDEFINED)
                cell.HorizontalAlignment = _horizontalAlignment;
            if (_verticalAlignment != ALIGN_UNDEFINED)
                cell.VerticalAlignment = _verticalAlignment;
            if (useAscender)
                cell.UseAscender = useAscender;
            if (useBorderPadding)
                cell.UseBorderPadding = useBorderPadding;
            if (useDescender)
                cell.UseDescender = useDescender;
            float p;
            float spLeft = _spacingLeft;
            if (float.IsNaN(spLeft)) spLeft = 0f;
            float spRight = _spacingRight;
            if (float.IsNaN(spRight)) spRight = 0f;
            float spTop = _spacingTop;
            if (float.IsNaN(spTop)) spTop = 0f;
            float spBottom = _spacingBottom;
            if (float.IsNaN(spBottom)) spBottom = 0f;
            p = _paddingLeft;
            if (float.IsNaN(p)) p = 0f;
            cell.PaddingLeft = p + spLeft;
            p = _paddingRight;
            if (float.IsNaN(p)) p = 0f;
            cell.PaddingRight = p + spRight;
            p = _paddingTop;
            if (float.IsNaN(p)) p = 0f;
            cell.PaddingTop = p + spTop;
            p = _paddingBottom;
            if (float.IsNaN(p)) p = 0f;
            cell.PaddingBottom = p + spBottom;
            foreach (IElement element in _content)
            {
                cell.AddElement(element);
            }
            return cell;
        }
    }
}
