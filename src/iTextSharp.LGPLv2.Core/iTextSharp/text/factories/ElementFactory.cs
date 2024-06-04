using System.util;
using iTextSharp.text.html;

namespace iTextSharp.text.factories;

/// <summary>
///     This class is able to create Element objects based on a list of properties.
/// </summary>
public static class ElementFactory
{
    /// <summary>
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static Anchor GetAnchor(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var anchor = new Anchor(GetPhrase(attributes));
        var value = attributes[ElementTags.NAME];
        if (value != null)
        {
            anchor.Name = value;
        }

        value = attributes.Remove(ElementTags.REFERENCE);
        if (value != null)
        {
            anchor.Reference = value;
        }

        return anchor;
    }

    /// <summary>
    ///     Creates an Annotation object based on a list of properties.
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static Annotation GetAnnotation(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        float llx = 0, lly = 0, urx = 0, ury = 0;

        var value = attributes[ElementTags.LLX];
        if (value != null)
        {
            llx = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.LLY];
        if (value != null)
        {
            lly = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.URX];
        if (value != null)
        {
            urx = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.URY];
        if (value != null)
        {
            ury = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        var title = attributes[ElementTags.TITLE];
        var text = attributes[ElementTags.CONTENT];
        if (title != null || text != null)
        {
            return new Annotation(title, text, llx, lly, urx, ury);
        }

        value = attributes[ElementTags.URL];
        if (value != null)
        {
            return new Annotation(llx, lly, urx, ury, value);
        }

        value = attributes[ElementTags.NAMED];
        if (value != null)
        {
            return new Annotation(llx, lly, urx, ury, int.Parse(value, CultureInfo.InvariantCulture));
        }

        var file = attributes[ElementTags.FILE];
        var destination = attributes[ElementTags.DESTINATION];
        var page = attributes.Remove(ElementTags.PAGE);
        if (file != null)
        {
            if (destination != null)
            {
                return new Annotation(llx, lly, urx, ury, file, destination);
            }

            if (page != null)
            {
                return new Annotation(llx, lly, urx, ury, file, int.Parse(page, CultureInfo.InvariantCulture));
            }
        }

        title = "";
        text = "";
        return new Annotation(title, text, llx, lly, urx, ury);
    }

    /// <summary>
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static Cell GetCell(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var cell = new Cell();

        cell.SetHorizontalAlignment(attributes[ElementTags.HORIZONTALALIGN]);
        cell.SetVerticalAlignment(attributes[ElementTags.VERTICALALIGN]);
        var value = attributes[ElementTags.WIDTH];
        if (value != null)
        {
            cell.SetWidth(value);
        }

        value = attributes[ElementTags.COLSPAN];
        if (value != null)
        {
            cell.Colspan = int.Parse(value, CultureInfo.InvariantCulture);
        }

        value = attributes[ElementTags.ROWSPAN];
        if (value != null)
        {
            cell.Rowspan = int.Parse(value, CultureInfo.InvariantCulture);
        }

        value = attributes[ElementTags.LEADING];
        if (value != null)
        {
            cell.Leading = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        cell.Header = Utilities.CheckTrueOrFalse(attributes, ElementTags.HEADER);
        if (Utilities.CheckTrueOrFalse(attributes, ElementTags.NOWRAP))
        {
            cell.MaxLines = 1;
        }

        setRectangleProperties(cell, attributes);
        return cell;
    }

    /// <summary>
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static ChapterAutoNumber GetChapter(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var chapter = new ChapterAutoNumber("");
        setSectionParameters(chapter, attributes);
        return chapter;
    }

    /// <summary>
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static Chunk GetChunk(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var chunk = new Chunk { Font = FontFactory.GetFont(attributes) };

        var value = attributes[ElementTags.ITEXT];
        if (value != null)
        {
            chunk.Append(value);
        }

        value = attributes[ElementTags.Localgoto];
        if (value != null)
        {
            chunk.SetLocalGoto(value);
        }

        value = attributes[ElementTags.Remotegoto];
        if (value != null)
        {
            var page = attributes[ElementTags.PAGE];
            if (page != null)
            {
                chunk.SetRemoteGoto(value, int.Parse(page, CultureInfo.InvariantCulture));
            }
            else
            {
                var destination = attributes[ElementTags.DESTINATION];
                if (destination != null)
                {
                    chunk.SetRemoteGoto(value, destination);
                }
            }
        }

        value = attributes[ElementTags.Localdestination];
        if (value != null)
        {
            chunk.SetLocalDestination(value);
        }

        value = attributes[ElementTags.Subsupscript];
        if (value != null)
        {
            chunk.SetTextRise(float.Parse(value, NumberFormatInfo.InvariantInfo));
        }

        value = attributes[Markup.CSS_KEY_VERTICALALIGN];
        if (value != null && value.EndsWith("%", StringComparison.Ordinal))
        {
            var p = float.Parse(value.Substring(0, value.Length - 1), NumberFormatInfo.InvariantInfo) / 100f;
            chunk.SetTextRise(p * chunk.Font.Size);
        }

        value = attributes[ElementTags.Generictag];
        if (value != null)
        {
            chunk.SetGenericTag(value);
        }

        value = attributes[ElementTags.BACKGROUNDCOLOR];
        if (value != null)
        {
            chunk.SetBackground(Markup.DecodeColor(value));
        }

        return chunk;
    }

    /// <summary>
    ///     Returns an Image that has been constructed taking in account
    ///     the value of some attributes.
    /// </summary>
    /// <param name="attributes">Some attributes</param>
    /// <returns>an Image</returns>
    public static Image GetImage(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var value = attributes[ElementTags.URL];
        if (value == null)
        {
            throw new ArgumentException("The URL of the image is missing.");
        }

        var image = Image.GetInstance(value);

        value = attributes[ElementTags.ALIGN];
        var align = 0;
        if (value != null)
        {
            if (Util.EqualsIgnoreCase(ElementTags.ALIGN_LEFT, value))
            {
                align |= Element.ALIGN_LEFT;
            }
            else if (Util.EqualsIgnoreCase(ElementTags.ALIGN_RIGHT, value))
            {
                align |= Element.ALIGN_RIGHT;
            }
            else if (Util.EqualsIgnoreCase(ElementTags.ALIGN_MIDDLE, value))
            {
                align |= Element.ALIGN_MIDDLE;
            }
        }

        if (Util.EqualsIgnoreCase("true", attributes[ElementTags.UNDERLYING]))
        {
            align |= Image.UNDERLYING;
        }

        if (Util.EqualsIgnoreCase("true", attributes[ElementTags.TEXTWRAP]))
        {
            align |= Image.TEXTWRAP;
        }

        image.Alignment = align;

        value = attributes[ElementTags.ALT];
        if (value != null)
        {
            image.Alt = value;
        }

        var x = attributes[ElementTags.ABSOLUTEX];
        var y = attributes[ElementTags.ABSOLUTEY];
        if (x != null && y != null)
        {
            image.SetAbsolutePosition(float.Parse(x, NumberFormatInfo.InvariantInfo),
                                      float.Parse(y, NumberFormatInfo.InvariantInfo));
        }

        value = attributes[ElementTags.PLAINWIDTH];
        if (value != null)
        {
            image.ScaleAbsoluteWidth(float.Parse(value, NumberFormatInfo.InvariantInfo));
        }

        value = attributes[ElementTags.PLAINHEIGHT];
        if (value != null)
        {
            image.ScaleAbsoluteHeight(float.Parse(value, NumberFormatInfo.InvariantInfo));
        }

        value = attributes[ElementTags.ROTATION];
        if (value != null)
        {
            image.Rotation = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        return image;
    }

    /// <summary>
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static List GetList(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var list = new List
                   {
                       Numbered = Utilities.CheckTrueOrFalse(attributes, ElementTags.NUMBERED),
                       Lettered = Utilities.CheckTrueOrFalse(attributes, ElementTags.LETTERED),
                       Lowercase = Utilities.CheckTrueOrFalse(attributes, ElementTags.LOWERCASE),
                       Autoindent = Utilities.CheckTrueOrFalse(attributes, ElementTags.AUTO_INDENT_ITEMS),
                       Alignindent = Utilities.CheckTrueOrFalse(attributes, ElementTags.ALIGN_INDENTATION_ITEMS),
                   };


        var value = attributes[ElementTags.FIRST];
        if (value != null)
        {
            var character = value[0];
            list.First = char.IsLetter(character) ? character : int.Parse(value, CultureInfo.InvariantCulture);
        }

        value = attributes[ElementTags.LISTSYMBOL];
        if (value != null)
        {
            list.ListSymbol = new Chunk(value, FontFactory.GetFont(attributes));
        }

        value = attributes[ElementTags.INDENTATIONLEFT];
        if (value != null)
        {
            list.IndentationLeft = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.INDENTATIONRIGHT];
        if (value != null)
        {
            list.IndentationRight = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.SYMBOLINDENT];
        if (value != null)
        {
            list.SymbolIndent = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        return list;
    }

    /// <summary>
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static ListItem GetListItem(Properties attributes)
    {
        var item = new ListItem(GetParagraph(attributes));
        return item;
    }

    /// <summary>
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static Paragraph GetParagraph(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var paragraph = new Paragraph(GetPhrase(attributes));
        var value = attributes[ElementTags.ALIGN];
        if (value != null)
        {
            paragraph.SetAlignment(value);
        }

        value = attributes[ElementTags.INDENTATIONLEFT];
        if (value != null)
        {
            paragraph.IndentationLeft = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.INDENTATIONRIGHT];
        if (value != null)
        {
            paragraph.IndentationRight = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        return paragraph;
    }

    /// <summary>
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static Phrase GetPhrase(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var phrase = new Phrase { Font = FontFactory.GetFont(attributes) };
        var value = attributes[ElementTags.LEADING];
        if (value != null)
        {
            phrase.Leading = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[Markup.CSS_KEY_LINEHEIGHT];
        if (value != null)
        {
            phrase.Leading = Markup.ParseLength(value, Markup.DEFAULT_FONT_SIZE);
        }

        value = attributes[ElementTags.ITEXT];
        if (value != null)
        {
            var chunk = new Chunk(value);
            if ((value = attributes[ElementTags.Generictag]) != null)
            {
                chunk.SetGenericTag(value);
            }

            phrase.Add(chunk);
        }

        return phrase;
    }

    /// <summary>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static Section GetSection(Section parent, Properties attributes)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var section = parent.AddSection("");
        setSectionParameters(section, attributes);
        return section;
    }

    /// <summary>
    ///     Creates an Table object based on a list of properties.
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public static Table GetTable(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        Table table;

        var value = attributes[ElementTags.WIDTHS];
        if (value != null)
        {
            var widthTokens = new StringTokenizer(value, ";");
            var values = new List<string>();
            while (widthTokens.HasMoreTokens())
            {
                values.Add(widthTokens.NextToken());
            }

            table = new Table(values.Count);
            var widths = new float[table.Columns];
            for (var i = 0; i < values.Count; i++)
            {
                value = values[i];
                widths[i] = float.Parse(value, NumberFormatInfo.InvariantInfo);
            }

            table.Widths = widths;
        }
        else
        {
            value = attributes[ElementTags.COLUMNS];
            try
            {
                table = new Table(int.Parse(value, CultureInfo.InvariantCulture));
            }
            catch
            {
                table = new Table(1);
            }
        }

        table.Border = Rectangle.BOX;
        table.BorderWidth = 1;
        table.DefaultCell.Border = Rectangle.BOX;

        value = attributes[ElementTags.LASTHEADERROW];
        if (value != null)
        {
            table.LastHeaderRow = int.Parse(value, CultureInfo.InvariantCulture);
        }

        value = attributes[ElementTags.ALIGN];
        if (value != null)
        {
            table.SetAlignment(value);
        }

        value = attributes[ElementTags.CELLSPACING];
        if (value != null)
        {
            table.Spacing = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.CELLPADDING];
        if (value != null)
        {
            table.Padding = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.OFFSET];
        if (value != null)
        {
            table.Offset = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.WIDTH];
        if (value != null)
        {
            if (value.EndsWith("%", StringComparison.Ordinal))
            {
                table.Width = float.Parse(value.Substring(0, value.Length - 1), NumberFormatInfo.InvariantInfo);
            }
            else
            {
                table.Width = float.Parse(value, NumberFormatInfo.InvariantInfo);
                table.Locked = true;
            }
        }

        table.TableFitsPage = Utilities.CheckTrueOrFalse(attributes, ElementTags.TABLEFITSPAGE);
        table.CellsFitPage = Utilities.CheckTrueOrFalse(attributes, ElementTags.CELLSFITPAGE);
        table.Convert2Pdfptable = Utilities.CheckTrueOrFalse(attributes, ElementTags.CONVERT2PDFP);

        setRectangleProperties(table, attributes);
        return table;
    }

    /// <summary>
    ///     Sets some Rectangle properties (for a Cell, Table,...).
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="attributes"></param>
    private static void setRectangleProperties(Rectangle rect, Properties attributes)
    {
        var value = attributes[ElementTags.BORDERWIDTH];
        if (value != null)
        {
            rect.BorderWidth = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        var border = 0;
        if (Utilities.CheckTrueOrFalse(attributes, ElementTags.LEFT))
        {
            border |= Rectangle.LEFT_BORDER;
        }

        if (Utilities.CheckTrueOrFalse(attributes, ElementTags.RIGHT))
        {
            border |= Rectangle.RIGHT_BORDER;
        }

        if (Utilities.CheckTrueOrFalse(attributes, ElementTags.TOP))
        {
            border |= Rectangle.TOP_BORDER;
        }

        if (Utilities.CheckTrueOrFalse(attributes, ElementTags.BOTTOM))
        {
            border |= Rectangle.BOTTOM_BORDER;
        }

        rect.Border = border;

        var r = attributes[ElementTags.RED];
        var g = attributes[ElementTags.GREEN];
        var b = attributes[ElementTags.BLUE];
        if (r != null || g != null || b != null)
        {
            var red = 0;
            var green = 0;
            var blue = 0;
            if (r != null)
            {
                red = int.Parse(r, CultureInfo.InvariantCulture);
            }

            if (g != null)
            {
                green = int.Parse(g, CultureInfo.InvariantCulture);
            }

            if (b != null)
            {
                blue = int.Parse(b, CultureInfo.InvariantCulture);
            }

            rect.BorderColor = new BaseColor(red, green, blue);
        }
        else
        {
            rect.BorderColor = Markup.DecodeColor(attributes[ElementTags.BORDERCOLOR]);
        }

        r = attributes.Remove(ElementTags.BGRED);
        g = attributes.Remove(ElementTags.BGGREEN);
        b = attributes.Remove(ElementTags.BGBLUE);
        value = attributes[ElementTags.BACKGROUNDCOLOR];
        if (r != null || g != null || b != null)
        {
            var red = 0;
            var green = 0;
            var blue = 0;
            if (r != null)
            {
                red = int.Parse(r, CultureInfo.InvariantCulture);
            }

            if (g != null)
            {
                green = int.Parse(g, CultureInfo.InvariantCulture);
            }

            if (b != null)
            {
                blue = int.Parse(b, CultureInfo.InvariantCulture);
            }

            rect.BackgroundColor = new BaseColor(red, green, blue);
        }
        else if (value != null)
        {
            rect.BackgroundColor = Markup.DecodeColor(value);
        }
        else
        {
            value = attributes[ElementTags.GRAYFILL];
            if (value != null)
            {
                rect.GrayFill = float.Parse(value, NumberFormatInfo.InvariantInfo);
            }
        }
    }

    private static void setSectionParameters(Section section, Properties attributes)
    {
        var value = attributes[ElementTags.NUMBERDEPTH];
        if (value != null)
        {
            section.NumberDepth = int.Parse(value, CultureInfo.InvariantCulture);
        }

        value = attributes[ElementTags.INDENT];
        if (value != null)
        {
            section.Indentation = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.INDENTATIONLEFT];
        if (value != null)
        {
            section.IndentationLeft = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        value = attributes[ElementTags.INDENTATIONRIGHT];
        if (value != null)
        {
            section.IndentationRight = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }
    }
}