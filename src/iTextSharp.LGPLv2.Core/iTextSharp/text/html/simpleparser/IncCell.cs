using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text.html.simpleparser;

/// <summary>
///     @author  psoares
/// </summary>
public class IncCell : ITextElementArray
{
    /// <summary>
    ///     Creates a new instance of IncCell
    /// </summary>
    public IncCell(string tag, ChainedProperties props)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        Cell = new PdfPCell();

        var value = props["colspan"];
        if (value != null)
        {
            Cell.Colspan = int.Parse(value, CultureInfo.InvariantCulture);
        }

        value = props["rowspan"];
        if (value != null)
        {
            Cell.Rowspan = int.Parse(value, CultureInfo.InvariantCulture);
        }

        value = props["align"];
        if (tag.Equals("th", StringComparison.OrdinalIgnoreCase))
        {
            Cell.HorizontalAlignment = Element.ALIGN_CENTER;
        }

        if (value != null)
        {
            if (Util.EqualsIgnoreCase(value, "center"))
            {
                Cell.HorizontalAlignment = Element.ALIGN_CENTER;
            }
            else if (Util.EqualsIgnoreCase(value, "right"))
            {
                Cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            }
            else if (Util.EqualsIgnoreCase(value, "left"))
            {
                Cell.HorizontalAlignment = Element.ALIGN_LEFT;
            }
            else if (Util.EqualsIgnoreCase(value, "justify"))
            {
                Cell.HorizontalAlignment = Element.ALIGN_JUSTIFIED;
            }
        }

        value = props["valign"];
        Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        if (value != null)
        {
            if (Util.EqualsIgnoreCase(value, "top"))
            {
                Cell.VerticalAlignment = Element.ALIGN_TOP;
            }
            else if (Util.EqualsIgnoreCase(value, "bottom"))
            {
                Cell.VerticalAlignment = Element.ALIGN_BOTTOM;
            }
        }

        value = props["border"];
        float border = 0;
        if (value != null)
        {
            border = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        Cell.BorderWidth = border;
        value = props["cellpadding"];
        if (value != null)
        {
            Cell.Padding = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        // Advanced formatting - does not conform to HTML standards
        value = props["bordertop"];
        if (value != null)
        {
            Cell.BorderWidthTop = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = props["borderbottom"];
        if (value != null)
        {
            Cell.BorderWidthBottom = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = props["borderleft"];
        if (value != null)
        {
            Cell.BorderWidthLeft = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = props["borderright"];
        if (value != null)
        {
            Cell.BorderWidthRight = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = props["cellpaddingtop"];
        if (value != null)
        {
            Cell.PaddingTop = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = props["cellpaddingbottom"];
        if (value != null)
        {
            Cell.PaddingBottom = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = props["cellpaddingleft"];
        if (value != null)
        {
            Cell.PaddingLeft = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = props["cellpaddingright"];
        if (value != null)
        {
            Cell.PaddingRight = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = props["bordercolor"];
        if (value != null)
        {
            Cell.BorderColor = Markup.DecodeColor(value);
        }

        Cell.UseDescender = true;
        value = props["bgcolor"];
        Cell.BackgroundColor = Markup.DecodeColor(value);
    }

    public PdfPCell Cell { get; }

    public IList<Chunk> Chunks { get; } = new List<Chunk>();

    public int Type => Element.RECTANGLE;

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsNestable() => true;

    public bool Process(IElementListener listener) => true;

    public bool Add(IElement o)
    {
        Cell.AddElement(o);
        return true;
    }
}