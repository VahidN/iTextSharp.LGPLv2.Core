using System.Collections;
using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text.html.simpleparser
{
    /// <summary>
    /// @author  psoares
    /// </summary>
    public class IncCell : ITextElementArray
    {
        /// <summary>
        /// Creates a new instance of IncCell
        /// </summary>
        public IncCell(string tag, ChainedProperties props)
        {
            Cell = new PdfPCell();
            string value = props["colspan"];
            if (value != null)
                Cell.Colspan = int.Parse(value);
            value = props["align"];
            if (tag.Equals("th"))
                Cell.HorizontalAlignment = Element.ALIGN_CENTER;
            if (value != null)
            {
                if (Util.EqualsIgnoreCase(value, "center"))
                    Cell.HorizontalAlignment = Element.ALIGN_CENTER;
                else if (Util.EqualsIgnoreCase(value, "right"))
                    Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                else if (Util.EqualsIgnoreCase(value, "left"))
                    Cell.HorizontalAlignment = Element.ALIGN_LEFT;
                else if (Util.EqualsIgnoreCase(value, "justify"))
                    Cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            }
            value = props["valign"];
            Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            if (value != null)
            {
                if (Util.EqualsIgnoreCase(value, "top"))
                    Cell.VerticalAlignment = Element.ALIGN_TOP;
                else if (Util.EqualsIgnoreCase(value, "bottom"))
                    Cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            }
            value = props["border"];
            float border = 0;
            if (value != null)
                border = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            Cell.BorderWidth = border;
            value = props["cellpadding"];
            if (value != null)
                Cell.Padding = float.Parse(value, System.Globalization.NumberFormatInfo.InvariantInfo);
            Cell.UseDescender = true;
            value = props["bgcolor"];
            Cell.BackgroundColor = Markup.DecodeColor(value);
        }

        public PdfPCell Cell { get; }

        public ArrayList Chunks { get; } = new ArrayList();

        public int Type
        {
            get
            {
                return Element.RECTANGLE;
            }
        }

        public bool Add(object o)
        {
            if (!(o is IElement))
                return false;
            Cell.AddElement((IElement)o);
            return true;
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

        public bool Process(IElementListener listener)
        {
            return true;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}