using System.Text;
using System.util;

namespace iTextSharp.text.html;

/// <summary>
///     A  DocWriter  class for HTML.
///     An  HtmlWriter  can be added as a  DocListener
///     to a certain  Document  by getting an instance.
///     Every  Element  added to the original  Document
///     will be written to the  Stream  of this  HtmlWriter .
///     Example:
///     // creation of the document with a certain size and certain margins
///     Document document = new Document(PageSize.A4, 50, 50, 50, 50);
///     try {
///     // this will write HTML to the Standard Stream
///     HtmlWriter.GetInstance(document, System.out);
///     // this will write HTML to a file called text.html
///     HtmlWriter.GetInstance(document, new FileOutputStream("text.html"));
///     // this will write HTML to for instance the Stream of a HttpServletResponse-object
///     HtmlWriter.GetInstance(document, response.GetOutputStream());
///     }
///     catch (DocumentException de) {
///     System.err.Println(de.GetMessage());
///     }
///     // this will close the document and all the OutputStreams listening to it
///     document.Close();
/// </summary>
public class HtmlWriter : DocWriter
{
    /// <summary>
    ///     static membervariables (tags)
    /// </summary>
    /// <summary>
    ///     This is a possible HTML-tag.
    /// </summary>
    public const string NBSP = "&nbsp;";

    /// <summary>
    ///     This is a possible HTML-tag.
    /// </summary>
    public static byte[] Begincomment = GetIsoBytes("<!-- ");

    /// <summary>
    ///     This is a possible HTML-tag.
    /// </summary>
    public static byte[] Endcomment = GetIsoBytes(" -->");

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     This is the current font of the HTML.
    /// </summary>
    protected Stack<Font> Currentfont = new();

    /// <summary>
    ///     This is the textual part of the footer
    /// </summary>
    protected HeaderFooter footer;

    /// <summary>
    ///     This is the textual part of a header
    /// </summary>
    protected HeaderFooter header;

    /// <summary>
    ///     This is a path for images.
    /// </summary>
    protected string Imagepath;

    /// <summary>
    ///     Store the markup properties of a MarkedObject.
    /// </summary>
    protected Properties Markup = new();

    /// <summary>
    ///     Stores the page number.
    /// </summary>
    protected int PageN;

    /// <summary>
    ///     This is the standard font of the HTML.
    /// </summary>
    protected Font Standardfont = new();

    /// <summary>
    ///     constructor
    /// </summary>
    /// <summary>
    ///     Constructs a  HtmlWriter .
    /// </summary>
    /// <param name="doc">The  Document  that has to be written as HTML</param>
    /// <param name="os">The  Stream  the writer has to write to.</param>
    protected HtmlWriter(Document doc, Stream os) : base(doc, os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        Document.AddDocListener(this);
        PageN = Document.PageNumber;
        os.WriteByte(LT);
        var tmp = GetIsoBytes(HtmlTags.HTML);
        os.Write(tmp, 0, tmp.Length);
        os.WriteByte(GT);
        os.WriteByte(NEWLINE);
        os.WriteByte(TAB);
        os.WriteByte(LT);
        tmp = GetIsoBytes(HtmlTags.HEAD);
        os.Write(tmp, 0, tmp.Length);
        os.WriteByte(GT);
    }

    /// <summary>
    ///     get an instance of the HtmlWriter
    /// </summary>
    /// <summary>
    ///     Gets an instance of the  HtmlWriter .
    /// </summary>
    /// <param name="document">The  Document  that has to be written</param>
    /// <param name="os">The  Stream  the writer has to write to.</param>
    /// <returns>a new  HtmlWriter </returns>
    public static HtmlWriter GetInstance(Document document, Stream os) => new(document, os);

    /// <summary>
    ///     implementation of the DocListener methods
    /// </summary>
    /// <summary>
    ///     Signals that an new page has to be started.
    ///     @throws  DocumentException when a document isn't open yet, or has been closed
    /// </summary>
    /// <returns> true  if this action succeeded,  false  if not.</returns>
    public override bool Add(IElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (pause)
        {
            return false;
        }

        if (open && !element.IsContent())
        {
            throw new DocumentException("The document is open; you can only add Elements with content.");
        }

        switch (element.Type)
        {
            case Element.HEADER:
                try
                {
                    var h = (Header)element;
                    if (HtmlTags.STYLESHEET.Equals(h.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        WriteLink(h);
                    }
                    else if (HtmlTags.JAVASCRIPT.Equals(h.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        WriteJavaScript(h);
                    }
                    else
                    {
                        WriteHeader(h);
                    }
                }
                catch (InvalidCastException)
                {
                }

                return true;
            case Element.SUBJECT:
            case Element.KEYWORDS:
            case Element.AUTHOR:
                var meta = (Meta)element;
                WriteHeader(meta);
                return true;
            case Element.TITLE:
                AddTabs(2);
                WriteStart(HtmlTags.TITLE);
                Os.WriteByte(GT);
                AddTabs(3);
                Write(HtmlEncoder.Encode(((Meta)element).Content));
                AddTabs(2);
                WriteEnd(HtmlTags.TITLE);
                return true;
            case Element.CREATOR:
                WriteComment("Creator: " + HtmlEncoder.Encode(((Meta)element).Content));
                return true;
            case Element.PRODUCER:
                WriteComment("Producer: " + HtmlEncoder.Encode(((Meta)element).Content));
                return true;
            case Element.CREATIONDATE:
                WriteComment("Creationdate: " + HtmlEncoder.Encode(((Meta)element).Content));
                return true;
            case Element.MARKED:
                if (element is MarkedSection)
                {
                    var ms = (MarkedSection)element;
                    AddTabs(1);
                    WriteStart(HtmlTags.DIV);
                    WriteMarkupAttributes(ms.MarkupAttributes);
                    Os.WriteByte(GT);
                    var mo = ((MarkedSection)element).Title;
                    if (mo != null)
                    {
                        Markup = mo.MarkupAttributes;
                        mo.Process(this);
                    }

                    ms.Process(this);
                    WriteEnd(HtmlTags.DIV);
                    return true;
                }
                else
                {
                    var mo = (MarkedObject)element;
                    Markup = mo.MarkupAttributes;
                    return mo.Process(this);
                }
            default:
                Write(element, 2);
                return true;
        }
    }

    public bool Add(string str)
    {
        if (pause)
        {
            return false;
        }

        Write(str);
        return true;
    }

    public override void Close()
    {
        InitFooter(); // line added by David Freels
        AddTabs(1);
        WriteEnd(HtmlTags.BODY);
        Os.WriteByte(NEWLINE);
        WriteEnd(HtmlTags.HTML);
        base.Close();
    }

    public bool IsOtherFont(Font font)
    {
        try
        {
            var cFont = Currentfont.Peek();
            if (cFont.CompareTo(font) == 0)
            {
                return false;
            }

            return true;
        }
        catch (InvalidOperationException)
        {
            if (Standardfont.CompareTo(font) == 0)
            {
                return false;
            }

            return true;
        }
    }

    public override bool NewPage()
    {
        try
        {
            WriteStart(HtmlTags.DIV);
            Write(" ");
            Write(HtmlTags.STYLE);
            Write("=\"");
            WriteCssProperty(html.Markup.CSS_KEY_PAGE_BREAK_BEFORE, html.Markup.CSS_VALUE_ALWAYS);
            Write("\" /");
            Os.WriteByte(GT);
        }
        catch (IOException ioe)
        {
            throw new DocumentException(ioe.Message);
        }

        return true;
    }

    /// <summary>
    ///     Signals that an  Element  was added to the  Document .
    ///     @throws  DocumentException when a document isn't open yet, or has been closed
    /// </summary>
    /// <returns> true  if the element was added,  false  if not.</returns>
    /// <summary>
    ///     Signals that the  Document  has been opened and that
    ///     Elements  can be added.
    ///     The  HEAD -section of the HTML-document is written.
    /// </summary>
    public override void Open()
    {
        base.Open();
        WriteComment(Document.Version);
        WriteComment("CreationDate: " + DateTime.Now);
        AddTabs(1);
        WriteEnd(HtmlTags.HEAD);
        AddTabs(1);
        WriteStart(HtmlTags.BODY);
        if (Document.LeftMargin > 0)
        {
            Write(HtmlTags.LEFTMARGIN, Document.LeftMargin.ToString(CultureInfo.InvariantCulture));
        }

        if (Document.RightMargin > 0)
        {
            Write(HtmlTags.RIGHTMARGIN, Document.RightMargin.ToString(CultureInfo.InvariantCulture));
        }

        if (Document.TopMargin > 0)
        {
            Write(HtmlTags.TOPMARGIN, Document.TopMargin.ToString(CultureInfo.InvariantCulture));
        }

        if (Document.BottomMargin > 0)
        {
            Write(HtmlTags.BOTTOMMARGIN, Document.BottomMargin.ToString(CultureInfo.InvariantCulture));
        }

        if (PageSize.BackgroundColor != null)
        {
            Write(HtmlTags.BACKGROUNDCOLOR, HtmlEncoder.Encode(PageSize.BackgroundColor));
        }

        if (Document.JavaScriptOnLoad != null)
        {
            Write(HtmlTags.JAVASCRIPT_ONLOAD, HtmlEncoder.Encode(Document.JavaScriptOnLoad));
        }

        if (Document.JavaScriptOnUnLoad != null)
        {
            Write(HtmlTags.JAVASCRIPT_ONUNLOAD, HtmlEncoder.Encode(Document.JavaScriptOnUnLoad));
        }

        if (Document.HtmlStyleClass != null)
        {
            Write(html.Markup.HTML_ATTR_CSS_CLASS, Document.HtmlStyleClass);
        }

        Os.WriteByte(GT);
        InitHeader(); // line added by David Freels
    }

    /// <summary>
    ///     Signals that the  Document  was closed and that no other
    ///     Elements  will be added.
    /// </summary>
    /// <summary>
    ///     some protected methods
    /// </summary>
    /// <summary>
    ///     Adds the header to the top of the  Document
    /// </summary>
    public void ResetImagepath()
    {
        Imagepath = null;
    }

    public void SetFooter(HeaderFooter footer)
    {
        this.footer = footer;
    }

    public void SetHeader(HeaderFooter header)
    {
        this.header = header;
    }

    public void SetImagepath(string imagepath)
    {
        Imagepath = imagepath;
    }

    public void SetStandardFont(Font standardFont)
    {
        Standardfont = standardFont;
    }

    protected void InitFooter()
    {
        if (footer != null)
        {
            // Set the page number. HTML has no notion of a page, so it should always
            // add up to 1
            footer.PageNumber = PageN + 1;
            Add(footer.Paragraph);
        }
    }

    protected void InitHeader()
    {
        if (header != null)
        {
            Add(header.Paragraph);
        }
    }

    /// <summary>
    ///     Writes a Metatag in the header.
    ///     @throws  IOException
    /// </summary>
    protected void Write(IElement element, int indent)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        Properties styleAttributes = null;
        switch (element.Type)
        {
            case Element.MARKED:
            {
                try
                {
                    Add(element);
                }
                catch (DocumentException)
                {
                }

                return;
            }
            case Element.CHUNK:
            {
                var chunk = (Chunk)element;
                // if the chunk contains an image, return the image representation
                var image = chunk.GetImage();
                if (image != null)
                {
                    Write(image, indent);
                    return;
                }

                if (chunk.IsEmpty())
                {
                    return;
                }

                var attributes = chunk.Attributes;
                if (attributes != null && attributes[Chunk.NEWPAGE] != null)
                {
                    return;
                }

                var tag = IsOtherFont(chunk.Font) || Markup.Count > 0;
                if (tag)
                {
                    // start span tag
                    AddTabs(indent);
                    WriteStart(HtmlTags.SPAN);
                    if (IsOtherFont(chunk.Font))
                    {
                        Write(chunk.Font, null);
                    }

                    WriteMarkupAttributes(Markup);
                    Os.WriteByte(GT);
                }

                if (attributes != null && attributes[Chunk.SUBSUPSCRIPT] != null)
                {
                    // start sup or sub tag
                    if ((float)attributes[Chunk.SUBSUPSCRIPT] > 0)
                    {
                        WriteStart(HtmlTags.SUP);
                    }
                    else
                    {
                        WriteStart(HtmlTags.SUB);
                    }

                    Os.WriteByte(GT);
                }

                // contents
                Write(HtmlEncoder.Encode(chunk.Content));
                if (attributes != null && attributes[Chunk.SUBSUPSCRIPT] != null)
                {
                    // end sup or sub tag
                    Os.WriteByte(LT);
                    Os.WriteByte(FORWARD);
                    if ((float)attributes[Chunk.SUBSUPSCRIPT] > 0)
                    {
                        Write(HtmlTags.SUP);
                    }
                    else
                    {
                        Write(HtmlTags.SUB);
                    }

                    Os.WriteByte(GT);
                }

                if (tag)
                {
                    // end tag
                    WriteEnd(html.Markup.HTML_TAG_SPAN);
                }

                return;
            }
            case Element.PHRASE:
            {
                var phrase = (Phrase)element;
                styleAttributes = new Properties();
                if (phrase.HasLeading())
                {
                    styleAttributes[html.Markup.CSS_KEY_LINEHEIGHT] = phrase.Leading + "pt";
                }

                // start tag
                AddTabs(indent);
                WriteStart(html.Markup.HTML_TAG_SPAN);
                WriteMarkupAttributes(Markup);
                Write(phrase.Font, styleAttributes);
                Os.WriteByte(GT);
                Currentfont.Push(phrase.Font);
                // contents
                foreach (var i in phrase)
                {
                    Write(i, indent + 1);
                }

                // end tag
                AddTabs(indent);
                WriteEnd(html.Markup.HTML_TAG_SPAN);
                Currentfont.Pop();
                return;
            }
            case Element.ANCHOR:
            {
                var anchor = (Anchor)element;
                styleAttributes = new Properties();
                if (anchor.HasLeading())
                {
                    styleAttributes[html.Markup.CSS_KEY_LINEHEIGHT] = anchor.Leading + "pt";
                }

                // start tag
                AddTabs(indent);
                WriteStart(HtmlTags.ANCHOR);
                if (anchor.Name != null)
                {
                    Write(HtmlTags.NAME, anchor.Name);
                }

                if (anchor.Reference != null)
                {
                    Write(HtmlTags.REFERENCE, anchor.Reference);
                }

                WriteMarkupAttributes(Markup);
                Write(anchor.Font, styleAttributes);
                Os.WriteByte(GT);
                Currentfont.Push(anchor.Font);
                // contents
                foreach (var i in anchor)
                {
                    Write(i, indent + 1);
                }

                // end tag
                AddTabs(indent);
                WriteEnd(HtmlTags.ANCHOR);
                Currentfont.Pop();
                return;
            }
            case Element.PARAGRAPH:
            {
                var paragraph = (Paragraph)element;
                styleAttributes = new Properties();
                if (paragraph.HasLeading())
                {
                    styleAttributes[html.Markup.CSS_KEY_LINEHEIGHT] = paragraph.TotalLeading + "pt";
                }

                // start tag
                AddTabs(indent);
                WriteStart(HtmlTags.DIV);
                WriteMarkupAttributes(Markup);
                var alignment = HtmlEncoder.GetAlignment(paragraph.Alignment);
                if (!"".Equals(alignment, StringComparison.OrdinalIgnoreCase))
                {
                    Write(HtmlTags.ALIGN, alignment);
                }

                Write(paragraph.Font, styleAttributes);
                Os.WriteByte(GT);
                Currentfont.Push(paragraph.Font);
                // contents
                foreach (var i in paragraph)
                {
                    Write(i, indent + 1);
                }

                // end tag
                AddTabs(indent);
                WriteEnd(HtmlTags.DIV);
                Currentfont.Pop();
                return;
            }
            case Element.SECTION:
            case Element.CHAPTER:
            {
                // part of the start tag + contents
                WriteSection((Section)element, indent);
                return;
            }
            case Element.LIST:
            {
                var list = (List)element;
                // start tag
                AddTabs(indent);
                if (list.Numbered)
                {
                    WriteStart(HtmlTags.ORDEREDLIST);
                }
                else
                {
                    WriteStart(HtmlTags.UNORDEREDLIST);
                }

                WriteMarkupAttributes(Markup);
                Os.WriteByte(GT);
                // contents
                foreach (var i in list.Items)
                {
                    Write(i, indent + 1);
                }

                // end tag
                AddTabs(indent);
                if (list.Numbered)
                {
                    WriteEnd(HtmlTags.ORDEREDLIST);
                }
                else
                {
                    WriteEnd(HtmlTags.UNORDEREDLIST);
                }

                return;
            }
            case Element.LISTITEM:
            {
                var listItem = (ListItem)element;
                styleAttributes = new Properties();
                if (listItem.HasLeading())
                {
                    styleAttributes[html.Markup.CSS_KEY_LINEHEIGHT] = listItem.TotalLeading + "pt";
                }

                // start tag
                AddTabs(indent);
                WriteStart(HtmlTags.LISTITEM);
                WriteMarkupAttributes(Markup);
                Write(listItem.Font, styleAttributes);
                Os.WriteByte(GT);
                Currentfont.Push(listItem.Font);
                // contents
                foreach (var i in listItem)
                {
                    Write(i, indent + 1);
                }

                // end tag
                AddTabs(indent);
                WriteEnd(HtmlTags.LISTITEM);
                Currentfont.Pop();
                return;
            }
            case Element.CELL:
            {
                var cell = (Cell)element;

                // start tag
                AddTabs(indent);
                if (cell.Header)
                {
                    WriteStart(HtmlTags.HEADERCELL);
                }
                else
                {
                    WriteStart(HtmlTags.CELL);
                }

                WriteMarkupAttributes(Markup);
                if (cell.BorderWidth.ApproxNotEqual(Rectangle.UNDEFINED))
                {
                    Write(HtmlTags.BORDERWIDTH, cell.BorderWidth.ToString(CultureInfo.InvariantCulture));
                }

                if (cell.BorderColor != null)
                {
                    Write(HtmlTags.BORDERCOLOR, HtmlEncoder.Encode(cell.BorderColor));
                }

                if (cell.BackgroundColor != null)
                {
                    Write(HtmlTags.BACKGROUNDCOLOR, HtmlEncoder.Encode(cell.BackgroundColor));
                }

                var alignment = HtmlEncoder.GetAlignment(cell.HorizontalAlignment);
                if (!"".Equals(alignment, StringComparison.OrdinalIgnoreCase))
                {
                    Write(HtmlTags.HORIZONTALALIGN, alignment);
                }

                alignment = HtmlEncoder.GetAlignment(cell.VerticalAlignment);
                if (!"".Equals(alignment, StringComparison.OrdinalIgnoreCase))
                {
                    Write(HtmlTags.VERTICALALIGN, alignment);
                }

                if (cell.GetWidthAsString() != null)
                {
                    Write(HtmlTags.WIDTH, cell.GetWidthAsString());
                }

                if (cell.Colspan != 1)
                {
                    Write(HtmlTags.COLSPAN, cell.Colspan.ToString(CultureInfo.InvariantCulture));
                }

                if (cell.Rowspan != 1)
                {
                    Write(HtmlTags.ROWSPAN, cell.Rowspan.ToString(CultureInfo.InvariantCulture));
                }

                if (cell.MaxLines == 1)
                {
                    Write(HtmlTags.STYLE, "white-space: nowrap;");
                }

                Os.WriteByte(GT);
                // contents
                if (cell.IsEmpty())
                {
                    Write(NBSP);
                }
                else
                {
                    foreach (var i in cell.Elements)
                    {
                        Write(i, indent + 1);
                    }
                }

                // end tag
                AddTabs(indent);
                if (cell.Header)
                {
                    WriteEnd(HtmlTags.HEADERCELL);
                }
                else
                {
                    WriteEnd(HtmlTags.CELL);
                }

                return;
            }
            case Element.ROW:
            {
                var row = (Row)element;

                // start tag
                AddTabs(indent);
                WriteStart(HtmlTags.ROW);
                WriteMarkupAttributes(Markup);
                Os.WriteByte(GT);
                // contents
                IElement cell;
                for (var i = 0; i < row.Columns; i++)
                {
                    if ((cell = (IElement)row.GetCell(i)) != null)
                    {
                        Write(cell, indent + 1);
                    }
                }

                // end tag
                AddTabs(indent);
                WriteEnd(HtmlTags.ROW);
                return;
            }
            case Element.TABLE:
            {
                Table table;
                try
                {
                    table = (Table)element;
                }
                catch (InvalidCastException)
                {
                    table = ((SimpleTable)element).CreateTable();
                }

                table.Complete();
                // start tag
                AddTabs(indent);
                WriteStart(HtmlTags.TABLE);
                WriteMarkupAttributes(Markup);
                Os.WriteByte(SPACE);
                Write(HtmlTags.WIDTH);
                Os.WriteByte(EQUALS);
                Os.WriteByte(QUOTE);
                Write(table.Width.ToString(CultureInfo.InvariantCulture));
                if (!table.Locked)
                {
                    Write("%");
                }

                Os.WriteByte(QUOTE);
                var alignment = HtmlEncoder.GetAlignment(table.Alignment);
                if (!"".Equals(alignment, StringComparison.OrdinalIgnoreCase))
                {
                    Write(HtmlTags.ALIGN, alignment);
                }

                Write(HtmlTags.CELLPADDING, table.Cellpadding.ToString(CultureInfo.InvariantCulture));
                Write(HtmlTags.CELLSPACING, table.Cellspacing.ToString(CultureInfo.InvariantCulture));
                if (table.BorderWidth.ApproxNotEqual(Rectangle.UNDEFINED))
                {
                    Write(HtmlTags.BORDERWIDTH, table.BorderWidth.ToString(CultureInfo.InvariantCulture));
                }

                if (table.BorderColor != null)
                {
                    Write(HtmlTags.BORDERCOLOR, HtmlEncoder.Encode(table.BorderColor));
                }

                if (table.BackgroundColor != null)
                {
                    Write(HtmlTags.BACKGROUNDCOLOR, HtmlEncoder.Encode(table.BackgroundColor));
                }

                Os.WriteByte(GT);
                // contents
                foreach (var row in table)
                {
                    Write(row, indent + 1);
                }

                // end tag
                AddTabs(indent);
                WriteEnd(HtmlTags.TABLE);
                return;
            }
            case Element.ANNOTATION:
            {
                var annotation = (Annotation)element;
                WriteComment(annotation.Title + ": " + annotation.Content);
                return;
            }
            case Element.IMGRAW:
            case Element.JPEG:
            case Element.JPEG2000:
            case Element.IMGTEMPLATE:
            {
                var image = (Image)element;
                if (image.Url == null)
                {
                    return;
                }

                // start tag
                AddTabs(indent);
                WriteStart(HtmlTags.IMAGE);
                var path = image.Url.ToString();
                if (Imagepath != null)
                {
                    if (path.IndexOf("/", StringComparison.Ordinal) > 0)
                    {
                        path = Imagepath + path.Substring(path.LastIndexOf("/", StringComparison.Ordinal) + 1);
                    }
                    else
                    {
                        path = Imagepath + path;
                    }
                }

                Write(HtmlTags.URL, path);
                if ((image.Alignment & Image.RIGHT_ALIGN) > 0)
                {
                    Write(HtmlTags.ALIGN, HtmlTags.ALIGN_RIGHT);
                }
                else if ((image.Alignment & Image.MIDDLE_ALIGN) > 0)
                {
                    Write(HtmlTags.ALIGN, HtmlTags.ALIGN_MIDDLE);
                }
                else
                {
                    Write(HtmlTags.ALIGN, HtmlTags.ALIGN_LEFT);
                }

                if (image.Alt != null)
                {
                    Write(HtmlTags.ALT, image.Alt);
                }

                Write(HtmlTags.PLAINWIDTH, image.ScaledWidth.ToString(CultureInfo.InvariantCulture));
                Write(HtmlTags.PLAINHEIGHT, image.ScaledHeight.ToString(CultureInfo.InvariantCulture));
                WriteMarkupAttributes(Markup);
                WriteEnd();
                return;
            }

            default:
                return;
        }
    }

    protected void Write(Font font, Properties styleAttributes)
    {
        if (font == null || !IsOtherFont(font) /*|| styleAttributes == null*/)
        {
            return;
        }

        Write(" ");
        Write(HtmlTags.STYLE);
        Write("=\"");
        if (styleAttributes != null)
        {
            foreach (var key in styleAttributes.Keys)
            {
                WriteCssProperty(key, styleAttributes[key]);
            }
        }

        if (IsOtherFont(font))
        {
            WriteCssProperty(html.Markup.CSS_KEY_FONTFAMILY, font.Familyname);

            if (font.Size.ApproxNotEqual(Font.UNDEFINED))
            {
                WriteCssProperty(html.Markup.CSS_KEY_FONTSIZE, $"{font.Size}pt");
            }

            if (font.Color != null)
            {
                WriteCssProperty(html.Markup.CSS_KEY_COLOR, HtmlEncoder.Encode(font.Color));
            }

            var fontstyle = font.Style;
            var bf = font.BaseFont;
            if (bf != null)
            {
                var ps = bf.PostscriptFontName.ToLower(CultureInfo.InvariantCulture);
                if (ps.IndexOf("bold", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (fontstyle == Font.UNDEFINED)
                    {
                        fontstyle = 0;
                    }

                    fontstyle |= Font.BOLD;
                }

                if (ps.IndexOf("italic", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    ps.IndexOf("oblique", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (fontstyle == Font.UNDEFINED)
                    {
                        fontstyle = 0;
                    }

                    fontstyle |= Font.ITALIC;
                }
            }

            if (fontstyle != Font.UNDEFINED && fontstyle != Font.NORMAL)
            {
                switch (fontstyle & Font.BOLDITALIC)
                {
                    case Font.BOLD:
                        WriteCssProperty(html.Markup.CSS_KEY_FONTWEIGHT, html.Markup.CSS_VALUE_BOLD);
                        break;
                    case Font.ITALIC:
                        WriteCssProperty(html.Markup.CSS_KEY_FONTSTYLE, html.Markup.CSS_VALUE_ITALIC);
                        break;
                    case Font.BOLDITALIC:
                        WriteCssProperty(html.Markup.CSS_KEY_FONTWEIGHT, html.Markup.CSS_VALUE_BOLD);
                        WriteCssProperty(html.Markup.CSS_KEY_FONTSTYLE, html.Markup.CSS_VALUE_ITALIC);
                        break;
                }

                // CSS only supports one decoration tag so if both are specified
                // only one of the two will display
                if ((fontstyle & Font.UNDERLINE) > 0)
                {
                    WriteCssProperty(html.Markup.CSS_KEY_TEXTDECORATION, html.Markup.CSS_VALUE_UNDERLINE);
                }

                if ((fontstyle & Font.STRIKETHRU) > 0)
                {
                    WriteCssProperty(html.Markup.CSS_KEY_TEXTDECORATION, html.Markup.CSS_VALUE_LINETHROUGH);
                }
            }
        }

        Write("\"");
    }

    protected void WriteComment(string comment)
    {
        AddTabs(2);
        Os.Write(Begincomment, 0, Begincomment.Length);
        Write(comment);
        Os.Write(Endcomment, 0, Endcomment.Length);
    }

    /// <summary>
    ///     Writes out a CSS property.
    /// </summary>
    protected void WriteCssProperty(string prop, string value)
    {
        Write(new StringBuilder(prop).Append(": ").Append(value).Append("; ").ToString());
    }

    protected void WriteHeader(Meta meta)
    {
        if (meta == null)
        {
            throw new ArgumentNullException(nameof(meta));
        }

        AddTabs(2);
        WriteStart(HtmlTags.META);
        switch (meta.Type)
        {
            case Element.HEADER:
                Write(HtmlTags.NAME, ((Header)meta).Name);
                break;
            case Element.SUBJECT:
                Write(HtmlTags.NAME, HtmlTags.SUBJECT);
                break;
            case Element.KEYWORDS:
                Write(HtmlTags.NAME, HtmlTags.KEYWORDS);
                break;
            case Element.AUTHOR:
                Write(HtmlTags.NAME, HtmlTags.AUTHOR);
                break;
        }

        Write(HtmlTags.CONTENT, HtmlEncoder.Encode(meta.Content));
        WriteEnd();
    }

    /// <summary>
    ///     Writes a link in the header.
    ///     @throws  IOException
    /// </summary>
    /// <param name="header">the element that has to be written</param>
    protected void WriteJavaScript(Header header)
    {
        if (header == null)
        {
            throw new ArgumentNullException(nameof(header));
        }

        AddTabs(2);
        WriteStart(HtmlTags.SCRIPT);
        Write(HtmlTags.LANGUAGE, HtmlTags.JAVASCRIPT);
        if (Markup.Count > 0)
        {
            /* JavaScript reference example:
             *
             * <script language="JavaScript" src="/myPath/MyFunctions.js"/>
             */
            WriteMarkupAttributes(Markup);
            Os.WriteByte(GT);
            WriteEnd(HtmlTags.SCRIPT);
        }
        else
        {
            /* JavaScript coding convention:
             *
             * <script language="JavaScript" type="text/javascript">
             * <!--
             * // ... JavaScript methods ...
             * //-->
             * </script>
             */
            Write(HtmlTags.TYPE, html.Markup.HTML_VALUE_JAVASCRIPT);
            Os.WriteByte(GT);
            AddTabs(2);
            Write(Encoding.ASCII.GetString(Begincomment) + "\n");
            Write(header.Content);
            AddTabs(2);
            Write("//" + Encoding.ASCII.GetString(Endcomment));
            AddTabs(2);
            WriteEnd(HtmlTags.SCRIPT);
        }
    }

    protected void WriteLink(Header header)
    {
        if (header == null)
        {
            throw new ArgumentNullException(nameof(header));
        }

        AddTabs(2);
        WriteStart(HtmlTags.LINK);
        Write(HtmlTags.REL, header.Name);
        Write(HtmlTags.TYPE, HtmlTags.TEXT_CSS);
        Write(HtmlTags.REFERENCE, header.Content);
        WriteEnd();
    }


    /// <summary>
    ///     Writes the HTML representation of a section.
    /// </summary>
    /// <param name="section">the section to write</param>
    /// <param name="indent">the indentation</param>
    protected void WriteSection(Section section, int indent)
    {
        if (section == null)
        {
            throw new ArgumentNullException(nameof(section));
        }

        if (section.Title != null)
        {
            var depth = section.Depth - 1;
            if (depth > 5)
            {
                depth = 5;
            }

            var styleAttributes = new Properties();
            if (section.Title.HasLeading())
            {
                styleAttributes[html.Markup.CSS_KEY_LINEHEIGHT] = section.Title.TotalLeading + "pt";
            }

            // start tag
            AddTabs(indent);
            WriteStart(HtmlTags.H[depth]);
            Write(section.Title.Font, styleAttributes);
            var alignment = HtmlEncoder.GetAlignment(section.Title.Alignment);
            if (!"".Equals(alignment, StringComparison.OrdinalIgnoreCase))
            {
                Write(HtmlTags.ALIGN, alignment);
            }

            WriteMarkupAttributes(Markup);
            Os.WriteByte(GT);
            Currentfont.Push(section.Title.Font);
            // contents
            foreach (var i in section.Title)
            {
                Write(i, indent + 1);
            }

            // end tag
            AddTabs(indent);
            WriteEnd(HtmlTags.H[depth]);
            Currentfont.Pop();
        }

        foreach (var i in section)
        {
            Write(i, indent);
        }
    }
}