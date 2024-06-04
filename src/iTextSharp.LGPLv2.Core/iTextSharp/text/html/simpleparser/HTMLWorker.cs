using System.Text;
using System.util;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.html.simpleparser;

public class HtmlWorker : ISimpleXmlDocHandler, IDocListener
{
    public const string tagsSupportedString =
        "ol ul li a pre font span br p div body table td th tr i b u sub sup em strong s strike"
        + " h1 h2 h3 h4 h5 h6 img hr";

    public static INullValueDictionary<string, string> TagsSupported = new NullValueDictionary<string, string>();

    private readonly ChainedProperties _cprops = new();
    private readonly FactoryProperties _factoryProperties = new();
    private readonly Stack<IElement> _stack = new();
    private readonly Stack<bool[]> _tableState = new();
    private Paragraph _currentParagraph;
    private INullValueDictionary<string, object> _interfaceProps;
    private bool _isPre;
    private bool _pendingLi;
    private bool _pendingTd;
    private bool _pendingTr;
    private bool _skipText;
    protected IDocListener Document;
    protected List<IElement> ObjectList;

    static HtmlWorker()
    {
        var tok = new StringTokenizer(tagsSupportedString);
        while (tok.HasMoreTokens())
        {
            TagsSupported[tok.NextToken()] = null;
        }
    }

    /// <summary>
    ///     Creates a new instance of HTMLWorker
    /// </summary>
    public HtmlWorker(IDocListener document) => Document = document;

    public INullValueDictionary<string, object> InterfaceProps
    {
        set
        {
            _interfaceProps = value;
            FontFactoryImp ff = null;
            if (_interfaceProps != null)
            {
                ff = _interfaceProps["font_factory"] as FontFactoryImp;
            }

            if (ff != null)
            {
                _factoryProperties.FontImp = ff;
            }
        }
        get => _interfaceProps;
    }

    public StyleSheet Style { set; get; } = new();

    public HeaderFooter Footer
    {
        set { }
    }

    public HeaderFooter Header
    {
        set { }
    }

    public int PageCount
    {
        set { }
    }

    public bool Add(IElement element)
    {
        ObjectList.Add(element);
        return true;
    }

    public void Close()
    {
    }

    public bool NewPage() => true;

    public void Open()
    {
    }

    public void ResetFooter()
    {
    }

    public void ResetHeader()
    {
    }

    public void ResetPageCount()
    {
    }

    public bool SetMarginMirroring(bool marginMirroring) => false;

    /// <summary>
    ///     @see com.lowagie.text.DocListener#setMarginMirroring(boolean)
    ///     @since	2.1.6
    /// </summary>
    public bool SetMarginMirroringTopBottom(bool marginMirroringTopBottom) => false;

    public bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom) => true;

    public bool SetPageSize(Rectangle pageSize) => true;

    public void Dispose()
    {
        Close();
    }

    public virtual void EndDocument()
    {
        foreach (var e in _stack)
        {
            Document.Add(e);
        }

        if (_currentParagraph != null)
        {
            Document.Add(_currentParagraph);
        }

        _currentParagraph = null;
    }

    public virtual void EndElement(string tag)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (!TagsSupported.ContainsKey(tag))
        {
            return;
        }

        var follow = FactoryProperties.FollowTags[tag];
        if (follow != null)
        {
            _cprops.RemoveChain(follow);
            return;
        }

        if (tag.Equals("font", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("span", StringComparison.OrdinalIgnoreCase))
        {
            _cprops.RemoveChain(tag);
            return;
        }

        if (tag.Equals("a", StringComparison.OrdinalIgnoreCase))
        {
            if (_currentParagraph == null)
            {
                _currentParagraph = new Paragraph();
            }

            IALink i = null;
            var skip = false;
            if (_interfaceProps != null)
            {
                i = _interfaceProps["alink_interface"] as IALink;
                if (i != null)
                {
                    skip = i.Process(_currentParagraph, _cprops);
                }
            }

            if (!skip)
            {
                var href = _cprops["href"];
                if (href != null)
                {
                    var chunks = _currentParagraph.Chunks;
                    for (var k = 0; k < chunks.Count; ++k)
                    {
                        var ck = chunks[k];
                        ck.SetAnchor(href);
                    }
                }
            }

            var tmp = (Paragraph)_stack.Pop();
            var tmp2 = new Phrase();
            tmp2.Add(_currentParagraph);
            tmp.Add(tmp2);
            _currentParagraph = tmp;
            _cprops.RemoveChain("a");
            return;
        }

        if (tag.Equals("br", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (_currentParagraph != null)
        {
            if (_stack.Count == 0)
            {
                Document.Add(_currentParagraph);
            }
            else
            {
                var obj = _stack.Pop();
                if (obj is ITextElementArray)
                {
                    var current = (ITextElementArray)obj;
                    current.Add(_currentParagraph);
                }

                _stack.Push(obj);
            }
        }

        _currentParagraph = null;
        if (tag.Equals(HtmlTags.UNORDEREDLIST, StringComparison.OrdinalIgnoreCase) ||
            tag.Equals(HtmlTags.ORDEREDLIST, StringComparison.OrdinalIgnoreCase))
        {
            if (_pendingLi)
            {
                EndElement(HtmlTags.LISTITEM);
            }

            _skipText = false;
            _cprops.RemoveChain(tag);
            if (_stack.Count == 0)
            {
                return;
            }

            var obj = _stack.Pop();
            if (!(obj is List))
            {
                _stack.Push(obj);
                return;
            }

            if (_stack.Count == 0)
            {
                Document.Add(obj);
            }
            else
            {
                ((ITextElementArray)_stack.Peek()).Add(obj);
            }

            return;
        }

        if (tag.Equals(HtmlTags.LISTITEM, StringComparison.OrdinalIgnoreCase))
        {
            _pendingLi = false;
            _skipText = true;
            _cprops.RemoveChain(tag);
            if (_stack.Count == 0)
            {
                return;
            }

            var obj = _stack.Pop();
            if (!(obj is ListItem))
            {
                _stack.Push(obj);
                return;
            }

            if (_stack.Count == 0)
            {
                Document.Add(obj);
                return;
            }

            var list = _stack.Pop();
            if (!(list is List))
            {
                _stack.Push(list);
                return;
            }

            var item = (ListItem)obj;
            ((List)list).Add(item);
            var cks = item.Chunks;
            if (cks.Count > 0)
            {
                item.ListSymbol.Font = cks[0].Font;
            }

            _stack.Push(list);
            return;
        }

        if (tag.Equals("div", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("body", StringComparison.OrdinalIgnoreCase))
        {
            _cprops.RemoveChain(tag);
            return;
        }

        if (tag.Equals(HtmlTags.PRE, StringComparison.OrdinalIgnoreCase))
        {
            _cprops.RemoveChain(tag);
            _isPre = false;
            return;
        }

        if (tag.Equals("p", StringComparison.OrdinalIgnoreCase))
        {
            _cprops.RemoveChain(tag);
            return;
        }

        if (tag.Equals("h1", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h2", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h3", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h4", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h5", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h6", StringComparison.OrdinalIgnoreCase))
        {
            _cprops.RemoveChain(tag);
            return;
        }

        if (tag.Equals("table", StringComparison.OrdinalIgnoreCase))
        {
            if (_pendingTr)
            {
                EndElement("tr");
            }

            _cprops.RemoveChain("table");
            var table = (IncTable)_stack.Pop();
            var tb = table.BuildTable();
            tb.SplitRows = true;
            if (_stack.Count == 0)
            {
                Document.Add(tb);
            }
            else
            {
                ((ITextElementArray)_stack.Peek()).Add(tb);
            }

            var state = _tableState.Pop();
            _pendingTr = state[0];
            _pendingTd = state[1];
            _skipText = false;
            return;
        }

        if (tag.Equals("tr", StringComparison.OrdinalIgnoreCase))
        {
            if (_pendingTd)
            {
                EndElement("td");
            }

            _pendingTr = false;
            _cprops.RemoveChain("tr");
            var cells = new List<PdfPCell>();
            IncTable table = null;
            while (true)
            {
                var obj = _stack.Pop();
                if (obj is IncCell)
                {
                    cells.Add(((IncCell)obj).Cell);
                }

                if (obj is IncTable)
                {
                    table = (IncTable)obj;
                    break;
                }
            }

            table.AddCols(cells);
            table.EndRow();
            _stack.Push(table);
            _skipText = true;
            return;
        }

        if (tag.Equals("td", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("th", StringComparison.OrdinalIgnoreCase))
        {
            _pendingTd = false;
            _cprops.RemoveChain("td");
            _skipText = true;
        }
    }

    public virtual void StartDocument()
    {
        var h = new NullValueDictionary<string, string>();
        Style.ApplyStyle("body", h);
        _cprops.AddToChain("body", h);
    }

    public virtual void Text(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (_skipText)
        {
            return;
        }

        var content = str;
        if (_isPre)
        {
            if (_currentParagraph == null)
            {
                _currentParagraph = FactoryProperties.CreateParagraph(_cprops);
            }

            _currentParagraph.Add(FactoryProperties.CreateChunk(content, _cprops));
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

        if (_currentParagraph == null)
        {
            _currentParagraph = FactoryProperties.CreateParagraph(_cprops);
        }

        _currentParagraph.Add(FactoryProperties.CreateChunk(buf.ToString(), _cprops));
    }

    public virtual void StartElement(string tag, INullValueDictionary<string, string> h)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (!TagsSupported.ContainsKey(tag))
        {
            return;
        }

        Style.ApplyStyle(tag, h);
        var follow = FactoryProperties.FollowTags[tag];
        if (follow != null)
        {
            var prop = new NullValueDictionary<string, string>();
            prop[follow] = null;
            _cprops.AddToChain(follow, prop);
            return;
        }

        FactoryProperties.InsertStyle(h, _cprops);
        if (tag.Equals(HtmlTags.ANCHOR, StringComparison.OrdinalIgnoreCase))
        {
            _cprops.AddToChain(tag, h);
            if (_currentParagraph == null)
            {
                _currentParagraph = new Paragraph();
            }

            _stack.Push(_currentParagraph);
            _currentParagraph = new Paragraph();
            return;
        }

        if (tag.Equals(HtmlTags.NEWLINE, StringComparison.OrdinalIgnoreCase))
        {
            if (_currentParagraph == null)
            {
                _currentParagraph = new Paragraph();
            }

            _currentParagraph.Add(FactoryProperties.CreateChunk("\n", _cprops));
            return;
        }

        if (tag.Equals(HtmlTags.HORIZONTALRULE, StringComparison.OrdinalIgnoreCase))
        {
            // Attempting to duplicate the behavior seen on Firefox with
            // http://www.w3schools.com/tags/tryit.asp?filename=tryhtml_hr_test
            // where an initial break is only inserted when the preceding element doesn't
            // end with a break, but a trailing break is always inserted.
            var addLeadingBreak = true;
            if (_currentParagraph == null)
            {
                _currentParagraph = new Paragraph();
                addLeadingBreak = false;
            }

            if (addLeadingBreak)
            {
                // Not a new paragraph
                var numChunks = _currentParagraph.Chunks.Count;
                if (numChunks == 0 ||
                    _currentParagraph.Chunks[numChunks - 1].Content.EndsWith("\n", StringComparison.OrdinalIgnoreCase))
                {
                    addLeadingBreak = false;
                }
            }

            var align = h["align"];
            var hrAlign = Element.ALIGN_CENTER;
            if (align != null)
            {
                if (Util.EqualsIgnoreCase(align, "left"))
                {
                    hrAlign = Element.ALIGN_LEFT;
                }

                if (Util.EqualsIgnoreCase(align, "right"))
                {
                    hrAlign = Element.ALIGN_RIGHT;
                }
            }

            var width = h["width"];
            float hrWidth = 1;
            if (width != null)
            {
                var tmpWidth = Markup.ParseLength(width, Markup.DEFAULT_FONT_SIZE);
                if (tmpWidth > 0)
                {
                    hrWidth = tmpWidth;
                }

                if (!width.EndsWith("%", StringComparison.OrdinalIgnoreCase))
                {
                    hrWidth = 100; // Treat a pixel width as 100% for now.
                }
            }

            var size = h["size"];
            float hrSize = 1;
            if (size != null)
            {
                var tmpSize = Markup.ParseLength(size, Markup.DEFAULT_FONT_SIZE);
                if (tmpSize > 0)
                {
                    hrSize = tmpSize;
                }
            }

            if (addLeadingBreak)
            {
                _currentParagraph.Add(Chunk.Newline);
            }

            _currentParagraph.Add(new LineSeparator(hrSize, hrWidth, null, hrAlign, _currentParagraph.Leading / 2));
            _currentParagraph.Add(Chunk.Newline);
            return;
        }

        if (tag.Equals(HtmlTags.CHUNK, StringComparison.OrdinalIgnoreCase) ||
            tag.Equals(HtmlTags.SPAN, StringComparison.OrdinalIgnoreCase))
        {
            _cprops.AddToChain(tag, h);
            return;
        }

        if (tag.Equals(HtmlTags.IMAGE, StringComparison.OrdinalIgnoreCase))
        {
            var src = h[ElementTags.SRC];
            if (src == null)
            {
                return;
            }

            _cprops.AddToChain(tag, h);
            Image img = null;
            if (_interfaceProps != null)
            {
                if (_interfaceProps["img_provider"] is IImageProvider ip)
                {
                    img = ip.GetImage(src, h, _cprops, Document);
                }

                if (img == null)
                {
                    if (_interfaceProps["img_static"] is INullValueDictionary<string, object> images)
                    {
                        var tim = (Image)images[src];
                        if (tim != null)
                        {
                            img = Image.GetInstance(tim);
                        }
                    }
                    else
                    {
                        if (!src.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                        {
                            // relative src references only
                            if (_interfaceProps["img_baseurl"] is string baseurl)
                            {
                                src = baseurl + src;
                                img = Image.GetInstance(src);
                            }
                        }
                    }
                }
            }

            if (img == null)
            {
                if (!src.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    var path = _cprops["image_path"];
                    if (path == null)
                    {
                        path = "";
                    }

                    src = Path.Combine(path, src);
                }

                img = Image.GetInstance(src);
            }

            var align = h["align"];
            var width = h["width"];
            var height = h["height"];
            var before = _cprops["before"];
            var after = _cprops["after"];
            if (before != null)
            {
                img.SpacingBefore = float.Parse(before, NumberFormatInfo.InvariantInfo);
            }

            if (after != null)
            {
                img.SpacingAfter = float.Parse(after, NumberFormatInfo.InvariantInfo);
            }

            var actualFontSize = Markup.ParseLength(_cprops[ElementTags.SIZE], Markup.DEFAULT_FONT_SIZE);
            if (actualFontSize <= 0f)
            {
                actualFontSize = Markup.DEFAULT_FONT_SIZE;
            }

            var widthInPoints = Markup.ParseLength(width, actualFontSize);
            var heightInPoints = Markup.ParseLength(height, actualFontSize);
            if (widthInPoints > 0 && heightInPoints > 0)
            {
                img.ScaleAbsolute(widthInPoints, heightInPoints);
            }
            else if (widthInPoints > 0)
            {
                heightInPoints = img.Height * widthInPoints / img.Width;
                img.ScaleAbsolute(widthInPoints, heightInPoints);
            }
            else if (heightInPoints > 0)
            {
                widthInPoints = img.Width * heightInPoints / img.Height;
                img.ScaleAbsolute(widthInPoints, heightInPoints);
            }

            img.WidthPercentage = 0;
            if (align != null)
            {
                EndElement("p");
                var ralign = Image.MIDDLE_ALIGN;
                if (Util.EqualsIgnoreCase(align, "left"))
                {
                    ralign = Image.LEFT_ALIGN;
                }
                else if (Util.EqualsIgnoreCase(align, "right"))
                {
                    ralign = Image.RIGHT_ALIGN;
                }

                img.Alignment = ralign;
                IImg i = null;
                var skip = false;
                if (_interfaceProps != null)
                {
                    i = _interfaceProps["img_interface"] as IImg;
                    if (i != null)
                    {
                        skip = i.Process(img, h, _cprops, Document);
                    }
                }

                if (!skip)
                {
                    Document.Add(img);
                }

                _cprops.RemoveChain(tag);
            }
            else
            {
                _cprops.RemoveChain(tag);
                if (_currentParagraph == null)
                {
                    _currentParagraph = FactoryProperties.CreateParagraph(_cprops);
                }

                _currentParagraph.Add(new Chunk(img, 0, 0));
            }

            return;
        }

        EndElement("p");
        if (tag.Equals("h1", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h2", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h3", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h4", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h5", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("h6", StringComparison.OrdinalIgnoreCase))
        {
            if (!h.ContainsKey(ElementTags.SIZE))
            {
                var v = 7 - int.Parse(tag.Substring(1), CultureInfo.InvariantCulture);
                h[ElementTags.SIZE] = v.ToString(CultureInfo.InvariantCulture);
            }

            _cprops.AddToChain(tag, h);
            return;
        }

        if (tag.Equals(HtmlTags.UNORDEREDLIST, StringComparison.OrdinalIgnoreCase))
        {
            if (_pendingLi)
            {
                EndElement(HtmlTags.LISTITEM);
            }

            _skipText = true;
            _cprops.AddToChain(tag, h);
            var list = new List(false);
            try
            {
                list.IndentationLeft = float.Parse(_cprops["indent"], NumberFormatInfo.InvariantInfo);
            }
            catch
            {
                list.Autoindent = true;
            }

            list.SetListSymbol("\u2022");
            _stack.Push(list);
            return;
        }

        if (tag.Equals(HtmlTags.ORDEREDLIST, StringComparison.OrdinalIgnoreCase))
        {
            if (_pendingLi)
            {
                EndElement(HtmlTags.LISTITEM);
            }

            _skipText = true;
            _cprops.AddToChain(tag, h);
            var list = new List(true);
            try
            {
                list.IndentationLeft = float.Parse(_cprops["indent"], NumberFormatInfo.InvariantInfo);
            }
            catch
            {
                list.Autoindent = true;
            }

            _stack.Push(list);
            return;
        }

        if (tag.Equals(HtmlTags.LISTITEM, StringComparison.OrdinalIgnoreCase))
        {
            if (_pendingLi)
            {
                EndElement(HtmlTags.LISTITEM);
            }

            _skipText = false;
            _pendingLi = true;
            _cprops.AddToChain(tag, h);
            _stack.Push(FactoryProperties.CreateListItem(_cprops));
            return;
        }

        if (tag.Equals(HtmlTags.DIV, StringComparison.OrdinalIgnoreCase) ||
            tag.Equals(HtmlTags.BODY, StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("p", StringComparison.OrdinalIgnoreCase))
        {
            _cprops.AddToChain(tag, h);
            return;
        }

        if (tag.Equals(HtmlTags.PRE, StringComparison.OrdinalIgnoreCase))
        {
            if (!h.ContainsKey(ElementTags.FACE))
            {
                h[ElementTags.FACE] = "Courier";
            }

            _cprops.AddToChain(tag, h);
            _isPre = true;
            return;
        }

        if (tag.Equals("tr", StringComparison.OrdinalIgnoreCase))
        {
            if (_pendingTr)
            {
                EndElement("tr");
            }

            _skipText = true;
            _pendingTr = true;
            _cprops.AddToChain("tr", h);
            return;
        }

        if (tag.Equals("td", StringComparison.OrdinalIgnoreCase) ||
            tag.Equals("th", StringComparison.OrdinalIgnoreCase))
        {
            if (_pendingTd)
            {
                EndElement(tag);
            }

            _skipText = false;
            _pendingTd = true;
            _cprops.AddToChain("td", h);
            _stack.Push(new IncCell(tag, _cprops));
            return;
        }

        if (tag.Equals("table", StringComparison.OrdinalIgnoreCase))
        {
            _cprops.AddToChain("table", h);
            var table = new IncTable(h);
            _stack.Push(table);
            _tableState.Push(new[] { _pendingTr, _pendingTd });
            _pendingTr = _pendingTd = false;
            _skipText = true;
        }
    }

    public static List<IElement> ParseToList(TextReader reader, StyleSheet style) => ParseToList(reader, style, null);

    public static List<IElement> ParseToList(TextReader reader, StyleSheet style,
                                             INullValueDictionary<string, object> interfaceProps)
    {
        var worker = new HtmlWorker(null);
        if (style != null)
        {
            worker.Style = style;
        }

        worker.Document = worker;
        worker.InterfaceProps = interfaceProps;
        worker.ObjectList = new List<IElement>();
        worker.Parse(reader);
        return worker.ObjectList;
    }

    public void Parse(TextReader reader)
    {
        SimpleXmlParser.Parse(this, null, reader, true);
    }
}