using System.Text;
using System.util;
using iTextSharp.text.factories;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.xml;

/// <summary>
///     The  iTextHandler -class maps several XHTML-tags to iText-objects.
/// </summary>
public class TextHandler : ParserBase
{
    /// <summary>
    ///     current margin of a page.
    /// </summary>
    private float _bottomMargin = 36;

    /// <summary>
    ///     current margin of a page.
    /// </summary>
    private float _leftMargin = 36;

    /// <summary>
    ///     current margin of a page.
    /// </summary>
    private float _rightMargin = 36;

    /// <summary>
    ///     current margin of a page.
    /// </summary>
    private float _topMargin = 36;

    /// <summary> Counts the number of chapters in this document. </summary>
    protected int Chapters = 0;

    /// <summary> This is a flag that can be set, if you want to open and close the Document-object yourself. </summary>
    protected bool ControlOpenClose = true;

    /// <summary> This is the current chunk to which characters can be added. </summary>
    protected Chunk CurrentChunk;

    /// <summary> This is the resulting document. </summary>
    protected IDocListener Document;

    /// <summary> This is the current chunk to which characters can be added. </summary>
    protected bool Ignore;

    protected NullValueDictionary<string, XmlPeer> MyTags;

    /// <summary> This is a  Stack  of objects, waiting to be added to the document. </summary>
    protected Stack<IElement> Stack;

    /// <summary>
    ///     Constructs a new iTextHandler that will translate all the events
    ///     triggered by the parser to actions on the  Document -object.
    /// </summary>
    /// <param name="document">this is the document on which events must be triggered</param>
    public TextHandler(IDocListener document)
    {
        Document = document;
        Stack = new Stack<IElement>();
    }

    /// <summary>
    ///     @throws DocumentException
    ///     @throws IOException
    /// </summary>
    /// <param name="document"></param>
    /// <param name="myTags"></param>
    public TextHandler(IDocListener document, TagMap myTags) : this(document) => MyTags = myTags;

    /// <summary>
    ///     @throws DocumentException
    ///     @throws IOException
    /// </summary>
    /// <param name="document"></param>
    /// <param name="myTags"></param>
    public TextHandler(IDocListener document, NullValueDictionary<string, XmlPeer> myTags) :
        this(document) => MyTags = myTags;

    /// <summary>
    ///     @throws DocumentException
    ///     @throws IOException
    /// </summary>
    /// <param name="document"></param>
    /// <param name="myTags"></param>
    /// <param name="bf"></param>
    public TextHandler(IDocListener document, TagMap myTags, BaseFont bf) : this(document, myTags) =>
        DefaultFont = bf;

    public BaseFont DefaultFont { set; get; }

    /// <summary>
    ///     This method gets called when characters are encountered.
    /// </summary>
    /// <param name="content">an array of characters</param>
    /// <param name="start">the start position in the array</param>
    /// <param name="length">the number of characters to read from the array</param>
    public override void Characters(string content, int start, int length)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        if (Ignore)
        {
            return;
        }

        if (content.Trim().Length == 0 && content.IndexOf(" ", StringComparison.Ordinal) < 0)
        {
            return;
        }

        var buf = new StringBuilder();
        var len = content.Length;
        char character;
        var newline = false;
        for (var i = 0; i < len; i++)
        {
            switch (character = content[i])
            {
                case ' ':
                    if (!newline)
                    {
                        buf.Append(character);
                    }

                    break;
                case '\n':
                    if (i > 0)
                    {
                        newline = true;
                        buf.Append(' ');
                    }

                    break;
                case '\r':
                    break;
                case '\t':
                    break;
                default:
                    newline = false;
                    buf.Append(character);
                    break;
            }
        }

        var tmp = buf.ToString();
        var rline = new string('\r', 1);
        var nline = new string('\n', 1);
        var tline = new string('\t', 1);
        tmp = tmp.Replace("\\n", nline);
        tmp = tmp.Replace("\\t", tline);
        tmp = tmp.Replace("\\r", rline);

        if (CurrentChunk == null)
        {
            if (DefaultFont == null)
            {
                CurrentChunk = new Chunk(buf.ToString());
            }
            else
            {
                CurrentChunk = new Chunk(buf.ToString(), new Font(DefaultFont));
            }
        }
        else
        {
            CurrentChunk.Append(buf.ToString());
        }
    }

    /// <summary>
    ///     This method gets called when an end tag is encountered.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="lname"></param>
    /// <param name="name">the name of the tag that ends</param>
    public override void EndElement(string uri, string lname, string name)
    {
        HandleEndingTags(name);
    }

    /// <summary>
    ///     This method deals with the starting tags.
    /// </summary>
    /// <param name="name">the name of the tag</param>
    public void HandleEndingTags(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }
        //System.err.Println("Stop: " + name);

        if (ElementTags.IGNORE.Equals(name, StringComparison.Ordinal))
        {
            Ignore = false;
            return;
        }

        if (Ignore)
        {
            return;
        }

        // tags that don't have any content
        if (isNewpage(name) || ElementTags.ANNOTATION.Equals(name, StringComparison.Ordinal) ||
            ElementTags.IMAGE.Equals(name, StringComparison.Ordinal) || isNewline(name))
        {
            return;
        }

        // titles of sections and chapters
        if (ElementTags.TITLE.Equals(name, StringComparison.Ordinal))
        {
            var current = (Paragraph)Stack.Pop();
            if (CurrentChunk != null)
            {
                current.Add(CurrentChunk);
                CurrentChunk = null;
            }

            var previous = (Section)Stack.Pop();
            previous.Title = current;
            Stack.Push(previous);
            return;
        }

        // all other endtags
        if (CurrentChunk != null)
        {
            ITextElementArray current;
            try
            {
                current = (ITextElementArray)Stack.Pop();
            }
            catch
            {
                current = new Paragraph();
            }

            current.Add(CurrentChunk);
            Stack.Push(current);
            CurrentChunk = null;
        }

        // chunks
        if (ElementTags.CHUNK.Equals(name, StringComparison.Ordinal))
        {
            return;
        }

        // phrases, anchors, lists, tables
        if (ElementTags.PHRASE.Equals(name, StringComparison.Ordinal) ||
            ElementTags.ANCHOR.Equals(name, StringComparison.Ordinal) ||
            ElementTags.LIST.Equals(name, StringComparison.Ordinal)
            || ElementTags.PARAGRAPH.Equals(name, StringComparison.Ordinal))
        {
            var current = Stack.Pop();
            try
            {
                var previous = (ITextElementArray)Stack.Pop();
                previous.Add(current);
                Stack.Push(previous);
            }
            catch
            {
                Document.Add(current);
            }

            return;
        }

        // listitems
        if (ElementTags.LISTITEM.Equals(name, StringComparison.Ordinal))
        {
            var listItem = (ListItem)Stack.Pop();
            var list = (List)Stack.Pop();
            list.Add(listItem);
            Stack.Push(list);
        }

        // tables
        if (ElementTags.TABLE.Equals(name, StringComparison.Ordinal))
        {
            var table = (Table)Stack.Pop();
            try
            {
                var previous = (ITextElementArray)Stack.Pop();
                previous.Add(table);
                Stack.Push(previous);
            }
            catch
            {
                Document.Add(table);
            }

            return;
        }

        // rows
        if (ElementTags.ROW.Equals(name, StringComparison.Ordinal))
        {
            var cells = new List<Cell>();
            var columns = 0;
            Table table;
            Cell cell;
            while (true)
            {
                var element = Stack.Pop();
                if (element.Type == Element.CELL)
                {
                    cell = (Cell)element;
                    columns += cell.Colspan;
                    cells.Add(cell);
                }
                else
                {
                    table = (Table)element;
                    break;
                }
            }

            if (table.Columns < columns)
            {
                table.AddColumns(columns - table.Columns);
            }

            cells.Reverse(0, cells.Count);
            string width;
            var cellWidths = new float[columns];
            var cellNulls = new bool[columns];
            for (var i = 0; i < columns; i++)
            {
                cellWidths[i] = 0;
                cellNulls[i] = true;
            }

            float total = 0;
            var j = 0;
            foreach (var c in cells)
            {
                cell = c;
                width = cell.GetWidthAsString();
                if (cell.Width.ApproxEquals(0))
                {
                    if (cell.Colspan == 1 && cellWidths[j].ApproxEquals(0))
                    {
                        try
                        {
                            cellWidths[j] = 100f / columns;
                            total += cellWidths[j];
                        }
                        catch
                        {
                            // empty on purpose
                        }
                    }
                    else if (cell.Colspan == 1)
                    {
                        cellNulls[j] = false;
                    }
                }
                else if (cell.Colspan == 1 && width.EndsWith("%", StringComparison.Ordinal))
                {
                    try
                    {
                        cellWidths[j] = float.Parse(width.Substring(0, width.Length - 1),
                                                    NumberFormatInfo.InvariantInfo);
                        total += cellWidths[j];
                    }
                    catch
                    {
                        // empty on purpose
                    }
                }

                j += cell.Colspan;
                table.AddCell(cell);
            }

            var widths = table.ProportionalWidths;
            if (widths.Length == columns)
            {
                var left = 0.0f;
                for (var i = 0; i < columns; i++)
                {
                    if (cellNulls[i] && widths[i].ApproxNotEqual(0))
                    {
                        left += widths[i];
                        cellWidths[i] = widths[i];
                    }
                }

                if (100.0 >= total)
                {
                    for (var i = 0; i < widths.Length; i++)
                    {
                        if (cellWidths[i].ApproxEquals(0) && widths[i].ApproxNotEqual(0))
                        {
                            cellWidths[i] = widths[i] / left * (100.0f - total);
                        }
                    }
                }

                table.Widths = cellWidths;
            }

            Stack.Push(table);
        }

        // registerfont
        if (name.Equals("registerfont", StringComparison.Ordinal))
        {
            return;
        }

        // header
        if (ElementTags.HEADER.Equals(name, StringComparison.Ordinal))
        {
            Document.Header = (HeaderFooter)Stack.Pop();
            return;
        }

        // footer
        if (ElementTags.FOOTER.Equals(name, StringComparison.Ordinal))
        {
            Document.Footer = (HeaderFooter)Stack.Pop();
            return;
        }

        // before
        if (name.Equals("before", StringComparison.Ordinal))
        {
            return;
        }

        // after
        if (name.Equals("after", StringComparison.Ordinal))
        {
            return;
        }

        // cells
        if (ElementTags.CELL.Equals(name, StringComparison.Ordinal))
        {
            return;
        }

        // sections
        if (ElementTags.SECTION.Equals(name, StringComparison.Ordinal))
        {
            Stack.Pop();
            return;
        }

        // chapters
        if (ElementTags.CHAPTER.Equals(name, StringComparison.Ordinal))
        {
            Document.Add(Stack.Pop());
            return;
        }

        // the documentroot
        if (IsDocumentRoot(name))
        {
            try
            {
                while (true)
                {
                    var element = Stack.Pop();
                    try
                    {
                        var previous = (ITextElementArray)Stack.Pop();
                        previous.Add(element);
                        Stack.Push(previous);
                    }
                    catch
                    {
                        Document.Add(element);
                    }
                }
            }
            catch
            {
                // empty on purpose
            }

            if (ControlOpenClose)
            {
                Document.Close();
            }
        }
    }

    /// <summary>
    ///     This method deals with the starting tags.
    /// </summary>
    /// <param name="name">the name of the tag</param>
    /// <param name="attributes">the list of attributes</param>
    public void HandleStartingTags(string name, Properties attributes)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        //System.err.Println("Start: " + name);
        if (Ignore || ElementTags.IGNORE.Equals(name, StringComparison.Ordinal))
        {
            Ignore = true;
            return;
        }

        // maybe there is some meaningful data that wasn't between tags
        if (CurrentChunk != null)
        {
            ITextElementArray current;
            try
            {
                current = (ITextElementArray)Stack.Pop();
            }
            catch
            {
                if (DefaultFont == null)
                {
                    current = new Paragraph("", new Font());
                }
                else
                {
                    current = new Paragraph("", new Font(DefaultFont));
                }
            }

            current.Add(CurrentChunk);
            Stack.Push(current);
            CurrentChunk = null;
        }

        // registerfont
        if (name.Equals("registerfont", StringComparison.Ordinal))
        {
            FontFactory.Register(attributes);
        }

        // header
        if (ElementTags.HEADER.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(new HeaderFooter(attributes));
            return;
        }

        // footer
        if (ElementTags.FOOTER.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(new HeaderFooter(attributes));
            return;
        }

        // before
        if (name.Equals("before", StringComparison.Ordinal))
        {
            var tmp = (HeaderFooter)Stack.Pop();

            tmp.Before = ElementFactory.GetPhrase(attributes);
            Stack.Push(tmp);
            return;
        }

        // after
        if (name.Equals("after", StringComparison.Ordinal))
        {
            var tmp = (HeaderFooter)Stack.Pop();

            tmp.After = ElementFactory.GetPhrase(attributes);
            Stack.Push(tmp);
            return;
        }

        // chunks
        if (ElementTags.CHUNK.Equals(name, StringComparison.Ordinal))
        {
            CurrentChunk = ElementFactory.GetChunk(attributes);
            if (DefaultFont != null)
            {
                CurrentChunk.Font = new Font(DefaultFont);
            }

            return;
        }

        // symbols
        if (ElementTags.ENTITY.Equals(name, StringComparison.Ordinal))
        {
            var f = new Font();
            if (CurrentChunk != null)
            {
                HandleEndingTags(ElementTags.CHUNK);
                f = CurrentChunk.Font;
            }

            CurrentChunk = EntitiesToSymbol.Get(attributes[ElementTags.ID], f);
            return;
        }

        // phrases
        if (ElementTags.PHRASE.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(ElementFactory.GetPhrase(attributes));
            return;
        }

        // anchors
        if (ElementTags.ANCHOR.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(ElementFactory.GetAnchor(attributes));
            return;
        }

        // paragraphs and titles
        if (ElementTags.PARAGRAPH.Equals(name, StringComparison.Ordinal) ||
            ElementTags.TITLE.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(ElementFactory.GetParagraph(attributes));
            return;
        }

        // lists
        if (ElementTags.LIST.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(ElementFactory.GetList(attributes));
            return;
        }

        // listitems
        if (ElementTags.LISTITEM.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(ElementFactory.GetListItem(attributes));
            return;
        }

        // cells
        if (ElementTags.CELL.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(ElementFactory.GetCell(attributes));
            return;
        }

        // tables
        if (ElementTags.TABLE.Equals(name, StringComparison.Ordinal))
        {
            var table = ElementFactory.GetTable(attributes);
            var widths = table.ProportionalWidths;
            for (var i = 0; i < widths.Length; i++)
            {
                if (widths[i].ApproxEquals(0))
                {
                    widths[i] = 100.0f / widths.Length;
                }
            }

            table.Widths = widths;
            Stack.Push(table);
            return;
        }

        // sections
        if (ElementTags.SECTION.Equals(name, StringComparison.Ordinal))
        {
            var previous = Stack.Pop();
            Section section;
            section = ElementFactory.GetSection((Section)previous, attributes);
            Stack.Push(previous);
            Stack.Push(section);
            return;
        }

        // chapters
        if (ElementTags.CHAPTER.Equals(name, StringComparison.Ordinal))
        {
            Stack.Push(ElementFactory.GetChapter(attributes));
            return;
        }

        // images
        if (ElementTags.IMAGE.Equals(name, StringComparison.Ordinal))
        {
            var img = ElementFactory.GetImage(attributes);
            try
            {
                AddImage(img);
                return;
            }
            catch
            {
                // if there is no element on the stack, the Image is added to the document
                Document.Add(img);
                return;
            }
        }

        // annotations
        if (ElementTags.ANNOTATION.Equals(name, StringComparison.Ordinal))
        {
            var annotation = ElementFactory.GetAnnotation(attributes);
            ITextElementArray current;
            try
            {
                current = (ITextElementArray)Stack.Pop();
                try
                {
                    current.Add(annotation);
                }
                catch
                {
                    Document.Add(annotation);
                }

                Stack.Push(current);
            }
            catch
            {
                Document.Add(annotation);
            }

            return;
        }

        // newlines
        if (isNewline(name))
        {
            ITextElementArray current;
            try
            {
                current = (ITextElementArray)Stack.Pop();
                current.Add(Chunk.Newline);
                Stack.Push(current);
            }
            catch
            {
                if (CurrentChunk == null)
                {
                    Document.Add(Chunk.Newline);
                }
                else
                {
                    CurrentChunk.Append("\n");
                }
            }

            return;
        }

        // newpage
        if (isNewpage(name))
        {
            ITextElementArray current;
            try
            {
                current = (ITextElementArray)Stack.Pop();
                var newPage = new Chunk("");
                newPage.SetNewPage();
                if (DefaultFont != null)
                {
                    newPage.Font = new Font(DefaultFont);
                }

                current.Add(newPage);
                Stack.Push(current);
            }
            catch
            {
                Document.NewPage();
            }

            return;
        }

        if (ElementTags.HORIZONTALRULE.Equals(name, StringComparison.Ordinal))
        {
            ITextElementArray current;
            var hr = new LineSeparator(1.0f, 100.0f, null, Element.ALIGN_CENTER, 0);
            try
            {
                current = (ITextElementArray)Stack.Pop();
                current.Add(hr);
                Stack.Push(current);
            }
            catch (InvalidOperationException)
            {
                Document.Add(hr);
            }

            return;
        }

        // documentroot
        if (IsDocumentRoot(name))
        {
            string value;
            // pagesize and orientation specific code suggested by Samuel Gabriel
            // Updated by Ricardo Coutinho. Only use if set in html!
            Rectangle pageSize = null;
            string orientation = null;
            foreach (var key in attributes.Keys)
            {
                value = attributes[key];
                // margin specific code suggested by Reza Nasiri
                if (Util.EqualsIgnoreCase(ElementTags.LEFT, key))
                {
                    _leftMargin = float.Parse(value, NumberFormatInfo.InvariantInfo);
                }

                if (Util.EqualsIgnoreCase(ElementTags.RIGHT, key))
                {
                    _rightMargin = float.Parse(value, NumberFormatInfo.InvariantInfo);
                }

                if (Util.EqualsIgnoreCase(ElementTags.TOP, key))
                {
                    _topMargin = float.Parse(value, NumberFormatInfo.InvariantInfo);
                }

                if (Util.EqualsIgnoreCase(ElementTags.BOTTOM, key))
                {
                    _bottomMargin = float.Parse(value, NumberFormatInfo.InvariantInfo);
                }

                if (ElementTags.PAGE_SIZE.Equals(key, StringComparison.Ordinal))
                {
                    pageSize = (Rectangle)typeof(PageSize).GetField(value).GetValue(null);
                }
                else if (ElementTags.ORIENTATION.Equals(key, StringComparison.Ordinal))
                {
                    if ("landscape".Equals(value, StringComparison.Ordinal))
                    {
                        orientation = "landscape";
                    }
                }
                else
                {
                    Document.Add(new Meta(key, value));
                }
            }

            if (pageSize != null)
            {
                if ("landscape".Equals(orientation, StringComparison.Ordinal))
                {
                    pageSize = pageSize.Rotate();
                }

                Document.SetPageSize(pageSize);
            }

            Document.SetMargins(_leftMargin, _rightMargin, _topMargin,
                                _bottomMargin);
            if (ControlOpenClose)
            {
                Document.Open();
            }
        }
    }

    /// <summary>
    ///     This method gets called when ignorable white space encountered.
    /// </summary>
    /// <param name="ch">an array of characters</param>
    /// <param name="start">the start position in the array</param>
    /// <param name="length">the number of characters to read from the array</param>
    public static void IgnorableWhitespace(char[] ch, int start, int length)
    {
        // do nothing: we handle white space ourselves in the characters method
    }

    /// <summary>
    ///     Sets the parameter that allows you to enable/disable the control over the Document.Open() and Document.Close()
    ///     method.
    /// </summary>
    /// <remarks>
    ///     If you set this parameter to true (= default), the parser will open the Document object when the start-root-tag is
    ///     encountered
    ///     and close it when the end-root-tag is met. If you set it to false, you have to open and close the Document object
    ///     yourself.
    /// </remarks>
    /// <param name="controlOpenClose">set this to false if you plan to open/close the Document yourself</param>
    public void SetControlOpenClose(bool controlOpenClose)
    {
        ControlOpenClose = controlOpenClose;
    }

    /// <summary>
    ///     This method gets called when a start tag is encountered.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="lname"></param>
    /// <param name="name">the name of the tag that is encountered</param>
    /// <param name="attrs">the list of attributes</param>
    public override void StartElement(string uri, string lname, string name,
                                      INullValueDictionary<string, string> attrs)
    {
        var attributes = new Properties();
        if (attrs != null)
        {
            foreach (var key in attrs.Keys)
            {
                attributes.Add(key, attrs[key]);
            }
        }

        HandleStartingTags(name, attributes);
    }

    protected internal void AddImage(Image img)
    {
        if (img == null)
        {
            throw new ArgumentNullException(nameof(img));
        }

        // if there is an element on the stack...
        var current = Stack.Pop();
        // ...and it's a Chapter or a Section, the Image can be
        // added directly
        if (current is Chapter
            || current is Section
            || current is Cell)
        {
            ((ITextElementArray)current).Add(img);
            Stack.Push(current);
            return;
        }
        // ...if not, we need to to a lot of stuff

        var newStack = new Stack<IElement>();
        while (!(current is Chapter
                 || current is Section || current is Cell))
        {
            newStack.Push(current);
            if (current is Anchor)
            {
                img.Annotation = new Annotation(0, 0, 0,
                                                0, ((Anchor)current).Reference);
            }

            current = Stack.Pop();
        }

        ((ITextElementArray)current).Add(img);
        Stack.Push(current);
        while (newStack.Count != 0)
        {
            Stack.Push(newStack.Pop());
        }
    }

    /// <summary>
    ///     Checks if a certain tag corresponds with the roottag.
    /// </summary>
    /// <param name="tag">a presumed tagname</param>
    /// <returns> true  if <VAR>tag</VAR> equals  itext ,  false  otherwise.</returns>
    protected static bool IsDocumentRoot(string tag) => ElementTags.ITEXT.Equals(tag, StringComparison.Ordinal);

    /// <summary>
    ///     Checks if a certain tag corresponds with the newpage-tag.
    /// </summary>
    /// <param name="tag">a presumed tagname</param>
    /// <returns> true  or  false </returns>
    private static bool isNewline(string tag) => ElementTags.NEWLINE.Equals(tag, StringComparison.Ordinal);

    /// <summary>
    ///     Checks if a certain tag corresponds with the newpage-tag.
    /// </summary>
    /// <param name="tag">a presumed tagname</param>
    /// <returns> true  or  false </returns>
    private static bool isNewpage(string tag) => ElementTags.NEWPAGE.Equals(tag, StringComparison.Ordinal);
}