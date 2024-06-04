using System.Text;
using System.util;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf;

/// <summary>
///     Bookmark processing in a simple way. It has some limitations, mainly the only
///     action types supported are GoTo, GoToR, URI and Launch.
///     The list structure is composed by a number of Hashtable, keyed by strings, one Hashtable
///     for each bookmark.
///     The element values are all strings with the exception of the key "Kids" that has
///     another list for the child bookmarks.
///     All the bookmarks have a "Title" with the
///     bookmark title and optionally a "Style" that can be "bold", "italic" or a
///     combination of both. They can also have a "Color" key with a value of three
///     floats separated by spaces. The key "Open" can have the values "true" or "false" and
///     signals the open status of the children. It's "true" by default.
///     The actions and the parameters can be:
///     "Action" = "GoTo" - "Page" | "Named"
///     "Page" = "3 XYZ 70 400 null" - page number followed by a destination (/XYZ is also accepted)
///     "Named" = "named_destination"
///     "Action" = "GoToR" - "Page" | "Named" | "NamedN", "File", ["NewWindow"]
///     "Page" = "3 XYZ 70 400 null" - page number followed by a destination (/XYZ is also accepted)
///     "Named" = "named_destination_as_a_string"
///     "NamedN" = "named_destination_as_a_name"
///     "File" - "the_file_to_open"
///     "NewWindow" - "true" or "false"
///     "Action" = "URI" - "URI"
///     "URI" = "http://sf.net" - URI to jump to
///     "Action" = "Launch" - "File"
///     "File" - "the_file_to_open_or_execute"
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public sealed class SimpleBookmark : ISimpleXmlDocHandler
{
    private readonly Stack<INullValueDictionary<string, object>> _attr = new();
    private List<INullValueDictionary<string, object>> _topList;

    /// <summary>
    ///     Creates a new instance of SimpleBookmark
    /// </summary>
    private SimpleBookmark()
    {
    }

    public void EndDocument()
    {
    }

    public void EndElement(string tag)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (tag.Equals("Bookmark", StringComparison.Ordinal))
        {
            if (_attr.Count == 0)
            {
                return;
            }

            throw new InvalidOperationException("Bookmark end tag out of place.");
        }

        if (!tag.Equals("Title", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Invalid end tag - " + tag);
        }

        var attributes = _attr.Pop();
        var title = (string)attributes["Title"];
        attributes["Title"] = title.Trim();
        var named = (string)attributes["Named"];

        if (named != null)
        {
            attributes["Named"] = UnEscapeBinaryString(named);
        }

        named = (string)attributes["NamedN"];

        if (named != null)
        {
            attributes["NamedN"] = UnEscapeBinaryString(named);
        }

        if (_attr.Count == 0)
        {
            _topList.Add(attributes);
        }
        else
        {
            var parent = _attr.Peek();
            var kids = (IList<INullValueDictionary<string, object>>)parent["Kids"];

            if (kids == null)
            {
                kids = new List<INullValueDictionary<string, object>>();
                parent["Kids"] = kids;
            }

            kids.Add(attributes);
        }
    }

    public void StartDocument()
    {
    }

    public void Text(string str)
    {
        if (_attr.Count == 0)
        {
            return;
        }

        var attributes = _attr.Peek();
        var title = (string)attributes["Title"];
        title += str;
        attributes["Title"] = title;
    }

    public void StartElement(string tag, INullValueDictionary<string, string> h)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (h == null)
        {
            throw new ArgumentNullException(nameof(h));
        }

        if (_topList == null)
        {
            if (tag.Equals("Bookmark", StringComparison.Ordinal))
            {
                _topList = new List<INullValueDictionary<string, object>>();

                return;
            }

            throw new InvalidOperationException("Root element is not Bookmark: " + tag);
        }

        if (!tag.Equals("Title", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Tag " + tag + " not allowed.");
        }

        var attributes = new NullValueDictionary<string, object>();

        foreach (var kv in h)
        {
            attributes[kv.Key] = kv.Value;
        }

        attributes["Title"] = "";
        attributes.Remove("Kids");
        _attr.Push(attributes);
    }

    /// <summary>
    ///     Removes the bookmark entries for a number of page ranges. The page ranges
    ///     consists of a number of pairs with the start/end page range. The page numbers
    ///     are inclusive.
    /// </summary>
    /// <param name="list">the bookmarks</param>
    /// <param name="pageRange">the page ranges, always in pairs.</param>
    public static void EliminatePages(IList<INullValueDictionary<string, object>> list, int[] pageRange)
    {
        if (pageRange == null)
        {
            throw new ArgumentNullException(nameof(pageRange));
        }

        if (list == null)
        {
            return;
        }

        for (var it = new ListIterator<INullValueDictionary<string, object>>(list); it.HasNext();)
        {
            var map = it.Next();
            var hit = false;

            if ("GoTo".Equals(map["Action"]))
            {
                var page = (string)map["Page"];

                if (page != null)
                {
                    page = page.Trim();
                    var idx = page.IndexOf(" ", StringComparison.Ordinal);
                    int pageNum;

                    if (idx < 0)
                    {
                        pageNum = int.Parse(page, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        pageNum = int.Parse(page.Substring(0, idx), CultureInfo.InvariantCulture);
                    }

                    var len = pageRange.Length & 0x7ffffffe;

                    for (var k = 0; k < len; k += 2)
                    {
                        if (pageNum >= pageRange[k] && pageNum <= pageRange[k + 1])
                        {
                            hit = true;

                            break;
                        }
                    }
                }
            }

            var kids = (IList<INullValueDictionary<string, object>>)map["Kids"];

            if (kids != null)
            {
                EliminatePages(kids, pageRange);

                if (kids.Count == 0)
                {
                    map.Remove("Kids");
                    kids = null;
                }
            }

            if (hit)
            {
                if (kids == null)
                {
                    it.Remove();
                }
                else
                {
                    map.Remove("Action");
                    map.Remove("Page");
                    map.Remove("Named");
                }
            }
        }
    }

    public static string EscapeBinaryString(string s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        var buf = new StringBuilder();
        var cc = s.ToCharArray();
        var len = cc.Length;

        for (var k = 0; k < len; ++k)
        {
            var c = cc[k];

            if (c < ' ')
            {
                buf.Append('\\');
                int v = c;
                var octal = "";

                do
                {
                    var x = v % 8;
                    octal = x + octal;
                    v /= 8;
                }
                while (v > 0);

                buf.Append(octal.PadLeft(3, '0'));
            }
            else if (c == '\\')
            {
                buf.Append("\\\\");
            }
            else
            {
                buf.Append(c);
            }
        }

        return buf.ToString();
    }

    /// <summary>
    ///     Exports the bookmarks to XML. The DTD for this XML is:
    ///     &lt;?xml version='1.0' encoding='UTF-8'?&gt;
    ///     &lt;!ELEMENT Title (#PCDATA|Title)*&gt;
    ///     &lt;!ATTLIST Title
    ///     Action CDATA #IMPLIED
    ///     Open CDATA #IMPLIED
    ///     Page CDATA #IMPLIED
    ///     URI CDATA #IMPLIED
    ///     File CDATA #IMPLIED
    ///     Named CDATA #IMPLIED
    ///     NamedN CDATA #IMPLIED
    ///     NewWindow CDATA #IMPLIED
    ///     Style CDATA #IMPLIED
    ///     Color CDATA #IMPLIED
    ///     &gt;
    ///     &lt;!ELEMENT Bookmark (Title)*&gt;
    ///     whatever the encoding
    ///     @throws IOException on error
    /// </summary>
    /// <param name="list">the bookmarks</param>
    /// <param name="outp">the export destination. The stream is not closed</param>
    /// <param name="encoding">the encoding according to IANA conventions</param>
    /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
    public static void ExportToXml(IList<INullValueDictionary<string, object>> list,
        Stream outp,
        string encoding,
        bool onlyAscii)
    {
        using var wrt = new StreamWriter(outp, IanaEncodings.GetEncodingEncoding(encoding));
        ExportToXml(list, wrt, encoding, onlyAscii);
    }

    /// <summary>
    ///     Exports the bookmarks to XML.
    ///     whatever the encoding
    ///     @throws IOException on error
    /// </summary>
    /// <param name="list">the bookmarks</param>
    /// <param name="wrt">the export destination. The writer is not closed</param>
    /// <param name="encoding">the encoding according to IANA conventions</param>
    /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
    public static void ExportToXml(IList<INullValueDictionary<string, object>> list,
        TextWriter wrt,
        string encoding,
        bool onlyAscii)
    {
        if (wrt == null)
        {
            throw new ArgumentNullException(nameof(wrt));
        }

        wrt.Write("<?xml version=\"1.0\" encoding=\"");
        wrt.Write(SimpleXmlParser.EscapeXml(encoding, onlyAscii));
        wrt.Write("\"?>\n<Bookmark>\n");
        ExportToXmlNode(list, wrt, 1, onlyAscii);
        wrt.Write("</Bookmark>\n");
        wrt.Flush();
    }

    /// <summary>
    ///     Exports the bookmarks to XML. Only of use if the generation is to be include in
    ///     some other XML document.
    ///     whatever the encoding
    ///     @throws IOException on error
    /// </summary>
    /// <param name="list">the bookmarks</param>
    /// <param name="outp">the export destination. The writer is not closed</param>
    /// <param name="indent">the indentation level. Pretty printing significant only</param>
    /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
    public static void ExportToXmlNode(IList<INullValueDictionary<string, object>> list,
        TextWriter outp,
        int indent,
        bool onlyAscii)
    {
        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }

        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        var dep = "";

        for (var k = 0; k < indent; ++k)
        {
            dep += "  ";
        }

        foreach (var map in list)
        {
            string title = null;
            outp.Write(dep);
            outp.Write("<Title ");
            IList<INullValueDictionary<string, object>> kids = null;

            foreach (var entry in map)
            {
                var key = entry.Key;

                if (key.Equals("Title", StringComparison.Ordinal))
                {
                    title = (string)entry.Value;

                    continue;
                }

                if (key.Equals("Kids", StringComparison.Ordinal))
                {
                    kids = (IList<INullValueDictionary<string, object>>)entry.Value;

                    continue;
                }

                outp.Write(key);
                outp.Write("=\"");
                var value = (string)entry.Value;

                if (key.Equals("Named", StringComparison.Ordinal) || key.Equals("NamedN", StringComparison.Ordinal))
                {
                    value = EscapeBinaryString(value);
                }

                outp.Write(SimpleXmlParser.EscapeXml(value, onlyAscii));
                outp.Write("\" ");
            }

            outp.Write(">");

            if (title == null)
            {
                title = "";
            }

            outp.Write(SimpleXmlParser.EscapeXml(title, onlyAscii));

            if (kids != null)
            {
                outp.Write("\n");
                ExportToXmlNode(kids, outp, indent + 1, onlyAscii);
                outp.Write(dep);
            }

            outp.Write("</Title>\n");
        }
    }

    /// <summary>
    ///     Gets a  List  with the bookmarks. It returns  null  if
    ///     the document doesn't have any bookmarks.
    ///     document doesn't have any
    /// </summary>
    /// <param name="reader">the document</param>
    /// <returns>a  List  with the bookmarks or  null  if the</returns>
    public static IList<INullValueDictionary<string, object>> GetBookmark(PdfReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var catalog = reader.Catalog;
        var obj = PdfReader.GetPdfObjectRelease(catalog.Get(PdfName.Outlines));

        if (obj == null || !obj.IsDictionary())
        {
            return null;
        }

        var outlines = (PdfDictionary)obj;
        var pages = new NullValueDictionary<int, int>();
        var numPages = reader.NumberOfPages;

        for (var k = 1; k <= numPages; ++k)
        {
            pages[reader.GetPageOrigRef(k).Number] = k;
            reader.ReleasePage(k);
        }

        return bookmarkDepth(reader, (PdfDictionary)PdfReader.GetPdfObjectRelease(outlines.Get(PdfName.First)), pages);
    }

    /// <summary>
    ///     Import the bookmarks from XML.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="inp">the XML source. The stream is not closed</param>
    /// <returns>the bookmarks</returns>
    public static List<INullValueDictionary<string, object>> ImportFromXml(Stream inp)
    {
        var book = new SimpleBookmark();
        SimpleXmlParser.Parse(book, inp);

        return book._topList;
    }

    /// <summary>
    ///     Import the bookmarks from XML.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="inp">the XML source. The reader is not closed</param>
    /// <returns>the bookmarks</returns>
    public static List<INullValueDictionary<string, object>> ImportFromXml(TextReader inp)
    {
        var book = new SimpleBookmark();
        SimpleXmlParser.Parse(book, inp);

        return book._topList;
    }

    public static object[] IterateOutlines(PdfWriter writer,
        PdfIndirectReference parent,
        IList<INullValueDictionary<string, object>> kids,
        bool namedAsNames)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (kids == null)
        {
            throw new ArgumentNullException(nameof(kids));
        }

        var refs = new PdfIndirectReference[kids.Count];

        for (var k = 0; k < refs.Length; ++k)
        {
            refs[k] = writer.PdfIndirectReference;
        }

        var ptr = 0;
        var count = 0;

        foreach (var map in kids)
        {
            object[] lower = null;
            var subKid = (IList<INullValueDictionary<string, object>>)map["Kids"];

            if (subKid != null && subKid.Count > 0)
            {
                lower = IterateOutlines(writer, refs[ptr], subKid, namedAsNames);
            }

            var outline = new PdfDictionary();
            ++count;

            if (lower != null)
            {
                outline.Put(PdfName.First, (PdfIndirectReference)lower[0]);
                outline.Put(PdfName.Last, (PdfIndirectReference)lower[1]);
                var n = (int)lower[2];

                if ("false".Equals(map["Open"]))
                {
                    outline.Put(PdfName.Count, new PdfNumber(-n));
                }
                else
                {
                    outline.Put(PdfName.Count, new PdfNumber(n));
                    count += n;
                }
            }

            outline.Put(PdfName.Parent, parent);

            if (ptr > 0)
            {
                outline.Put(PdfName.Prev, refs[ptr - 1]);
            }

            if (ptr < refs.Length - 1)
            {
                outline.Put(PdfName.Next, refs[ptr + 1]);
            }

            outline.Put(PdfName.Title, new PdfString((string)map["Title"], PdfObject.TEXT_UNICODE));
            var color = (string)map["Color"];

            if (color != null)
            {
                try
                {
                    var arr = new PdfArray();
                    var tk = new StringTokenizer(color);

                    for (var k = 0; k < 3; ++k)
                    {
                        var f = float.Parse(tk.NextToken(), NumberFormatInfo.InvariantInfo);

                        if (f < 0)
                        {
                            f = 0;
                        }

                        if (f > 1)
                        {
                            f = 1;
                        }

                        arr.Add(new PdfNumber(f));
                    }

                    outline.Put(PdfName.C, arr);
                }
                catch
                {
                } //in case it's malformed
            }

            var style = (string)map["Style"];

            if (style != null)
            {
                var bits = 0;

                if (style.IndexOf("italic", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    bits |= 1;
                }

                if (style.IndexOf("bold", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    bits |= 2;
                }

                if (bits != 0)
                {
                    outline.Put(PdfName.F, new PdfNumber(bits));
                }
            }

            CreateOutlineAction(outline, map, writer, namedAsNames);
            writer.AddToBody(outline, refs[ptr]);
            ++ptr;
        }

        return new object[]
        {
            refs[0], refs[refs.Length - 1], count
        };
    }

    /// <summary>
    ///     For the pages in range add the  pageShift  to the page number.
    ///     The page ranges
    ///     consists of a number of pairs with the start/end page range. The page numbers
    ///     are inclusive.
    ///     to include all the pages
    /// </summary>
    /// <param name="list">the bookmarks</param>
    /// <param name="pageShift">the number to add to the pages in range</param>
    /// <param name="pageRange">the page ranges, always in pairs. It can be  null </param>
    public static void ShiftPageNumbers(IList<INullValueDictionary<string, object>> list,
        int pageShift,
        int[] pageRange)
    {
        if (list == null)
        {
            return;
        }

        foreach (var map in list)
        {
            if ("GoTo".Equals(map["Action"]))
            {
                var page = (string)map["Page"];

                if (page != null)
                {
                    page = page.Trim();
                    var idx = page.IndexOf(" ", StringComparison.Ordinal);
                    int pageNum;

                    if (idx < 0)
                    {
                        pageNum = int.Parse(page, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        pageNum = int.Parse(page.Substring(0, idx), CultureInfo.InvariantCulture);
                    }

                    var hit = false;

                    if (pageRange == null)
                    {
                        hit = true;
                    }
                    else
                    {
                        var len = pageRange.Length & 0x7ffffffe;

                        for (var k = 0; k < len; k += 2)
                        {
                            if (pageNum >= pageRange[k] && pageNum <= pageRange[k + 1])
                            {
                                hit = true;

                                break;
                            }
                        }
                    }

                    if (hit)
                    {
                        if (idx < 0)
                        {
                            page = pageNum + pageShift + "";
                        }
                        else
                        {
                            page = pageNum + pageShift + page.Substring(idx);
                        }
                    }

                    map["Page"] = page;
                }
            }

            var kids = (IList<INullValueDictionary<string, object>>)map["Kids"];

            if (kids != null)
            {
                ShiftPageNumbers(kids, pageShift, pageRange);
            }
        }
    }

    public static string UnEscapeBinaryString(string s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        var buf = new StringBuilder();
        var cc = s.ToCharArray();
        var len = cc.Length;

        for (var k = 0; k < len; ++k)
        {
            var c = cc[k];

            if (c == '\\')
            {
                if (++k >= len)
                {
                    buf.Append('\\');

                    break;
                }

                c = cc[k];

                if (c >= '0' && c <= '7')
                {
                    var n = c - '0';
                    ++k;

                    for (var j = 0; j < 2 && k < len; ++j)
                    {
                        c = cc[k];

                        if (c >= '0' && c <= '7')
                        {
                            ++k;
                            n = n * 8 + c - '0';
                        }
                        else
                        {
                            break;
                        }
                    }

                    --k;
                    buf.Append((char)n);
                }
                else
                {
                    buf.Append(c);
                }
            }
            else
            {
                buf.Append(c);
            }
        }

        return buf.ToString();
    }

    internal static void CreateOutlineAction(PdfDictionary outline,
        INullValueDictionary<string, object> map,
        PdfWriter writer,
        bool namedAsNames)
    {
        try
        {
            var action = (string)map["Action"];

            if ("GoTo".Equals(action, StringComparison.Ordinal))
            {
                string p;

                if ((p = (string)map["Named"]) != null)
                {
                    if (namedAsNames)
                    {
                        outline.Put(PdfName.Dest, new PdfName(p));
                    }
                    else
                    {
                        outline.Put(PdfName.Dest, new PdfString(p, null));
                    }
                }
                else if ((p = (string)map["Page"]) != null)
                {
                    var ar = new PdfArray();
                    var tk = new StringTokenizer(p);
                    var n = int.Parse(tk.NextToken(), CultureInfo.InvariantCulture);
                    ar.Add(writer.GetPageReference(n));

                    if (!tk.HasMoreTokens())
                    {
                        ar.Add(PdfName.Xyz);

                        ar.Add(new float[]
                        {
                            0, 10000, 0
                        });
                    }
                    else
                    {
                        var fn = tk.NextToken();

                        if (fn.StartsWith("/", StringComparison.Ordinal))
                        {
                            fn = fn.Substring(1);
                        }

                        ar.Add(new PdfName(fn));

                        for (var k = 0; k < 4 && tk.HasMoreTokens(); ++k)
                        {
                            fn = tk.NextToken();

                            if (fn.Equals("null", StringComparison.Ordinal))
                            {
                                ar.Add(PdfNull.Pdfnull);
                            }
                            else
                            {
                                ar.Add(new PdfNumber(fn));
                            }
                        }
                    }

                    outline.Put(PdfName.Dest, ar);
                }
            }
            else if ("GoToR".Equals(action, StringComparison.Ordinal))
            {
                string p;
                var dic = new PdfDictionary();

                if ((p = (string)map["Named"]) != null)
                {
                    dic.Put(PdfName.D, new PdfString(p, null));
                }
                else if ((p = (string)map["NamedN"]) != null)
                {
                    dic.Put(PdfName.D, new PdfName(p));
                }
                else if ((p = (string)map["Page"]) != null)
                {
                    var ar = new PdfArray();
                    var tk = new StringTokenizer(p);
                    ar.Add(new PdfNumber(tk.NextToken()));

                    if (!tk.HasMoreTokens())
                    {
                        ar.Add(PdfName.Xyz);

                        ar.Add(new float[]
                        {
                            0, 10000, 0
                        });
                    }
                    else
                    {
                        var fn = tk.NextToken();

                        if (fn.StartsWith("/", StringComparison.Ordinal))
                        {
                            fn = fn.Substring(1);
                        }

                        ar.Add(new PdfName(fn));

                        for (var k = 0; k < 4 && tk.HasMoreTokens(); ++k)
                        {
                            fn = tk.NextToken();

                            if (fn.Equals("null", StringComparison.Ordinal))
                            {
                                ar.Add(PdfNull.Pdfnull);
                            }
                            else
                            {
                                ar.Add(new PdfNumber(fn));
                            }
                        }
                    }

                    dic.Put(PdfName.D, ar);
                }

                var file = (string)map["File"];

                if (dic.Size > 0 && file != null)
                {
                    dic.Put(PdfName.S, PdfName.Gotor);
                    dic.Put(PdfName.F, new PdfString(file));
                    var nw = (string)map["NewWindow"];

                    if (nw != null)
                    {
                        if (nw.Equals("true", StringComparison.Ordinal))
                        {
                            dic.Put(PdfName.Newwindow, PdfBoolean.Pdftrue);
                        }
                        else if (nw.Equals("false", StringComparison.Ordinal))
                        {
                            dic.Put(PdfName.Newwindow, PdfBoolean.Pdffalse);
                        }
                    }

                    outline.Put(PdfName.A, dic);
                }
            }
            else if ("URI".Equals(action, StringComparison.Ordinal))
            {
                var uri = (string)map["URI"];

                if (uri != null)
                {
                    var dic = new PdfDictionary();
                    dic.Put(PdfName.S, PdfName.Uri);
                    dic.Put(PdfName.Uri, new PdfString(uri));
                    outline.Put(PdfName.A, dic);
                }
            }
            else if ("Launch".Equals(action, StringComparison.Ordinal))
            {
                var file = (string)map["File"];

                if (file != null)
                {
                    var dic = new PdfDictionary();
                    dic.Put(PdfName.S, PdfName.Launch);
                    dic.Put(PdfName.F, new PdfString(file));
                    outline.Put(PdfName.A, dic);
                }
            }
        }
        catch
        {
            // empty on purpose
        }
    }

    private static List<INullValueDictionary<string, object>> bookmarkDepth(PdfReader reader,
        PdfDictionary outline,
        NullValueDictionary<int, int> pages)
    {
        var list = new List<INullValueDictionary<string, object>>();

        while (outline != null)
        {
            var map = new NullValueDictionary<string, object>();
            var title = (PdfString)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.Title));
            map["Title"] = title.ToUnicodeString();
            var color = (PdfArray)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.C));

            if (color != null && color.Size == 3)
            {
                var outp = new ByteBuffer();
                outp.Append(color.GetAsNumber(0).FloatValue).Append(' ');
                outp.Append(color.GetAsNumber(1).FloatValue).Append(' ');
                outp.Append(color.GetAsNumber(2).FloatValue);
                map["Color"] = PdfEncodings.ConvertToString(outp.ToByteArray(), null);
            }

            var style = (PdfNumber)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.F));

            if (style != null)
            {
                var f = style.IntValue;
                var s = "";

                if ((f & 1) != 0)
                {
                    s += "italic ";
                }

                if ((f & 2) != 0)
                {
                    s += "bold ";
                }

                s = s.Trim();

                if (s.Length != 0)
                {
                    map["Style"] = s;
                }
            }

            var count = (PdfNumber)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.Count));

            if (count != null && count.IntValue < 0)
            {
                map["Open"] = "false";
            }

            try
            {
                var dest = PdfReader.GetPdfObjectRelease(outline.Get(PdfName.Dest));

                if (dest != null)
                {
                    mapGotoBookmark(map, dest, pages); //changed by ujihara 2004-06-13
                }
                else
                {
                    var action = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.A));

                    if (action != null)
                    {
                        if (PdfName.Goto.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S))))
                        {
                            dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));

                            if (dest != null)
                            {
                                mapGotoBookmark(map, dest, pages);
                            }
                        }
                        else if (PdfName.Uri.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S))))
                        {
                            map["Action"] = "URI";

                            map["URI"] = ((PdfString)PdfReader.GetPdfObjectRelease(action.Get(PdfName.Uri)))
                                .ToUnicodeString();
                        }
                        else if (PdfName.Gotor.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S))))
                        {
                            dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));

                            if (dest != null)
                            {
                                if (dest.IsString())
                                {
                                    map["Named"] = dest.ToString();
                                }
                                else if (dest.IsName())
                                {
                                    map["NamedN"] = PdfName.DecodeName(dest.ToString());
                                }
                                else if (dest.IsArray())
                                {
                                    var arr = (PdfArray)dest;
                                    var s = new StringBuilder();
                                    s.Append(arr[0]);
                                    s.Append(' ').Append(arr[1]);

                                    for (var k = 2; k < arr.Size; ++k)
                                    {
                                        s.Append(' ').Append(arr[k]);
                                    }

                                    map["Page"] = s.ToString();
                                }
                            }

                            map["Action"] = "GoToR";
                            var file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.F));

                            if (file != null)
                            {
                                if (file.IsString())
                                {
                                    map["File"] = ((PdfString)file).ToUnicodeString();
                                }
                                else if (file.IsDictionary())
                                {
                                    file = PdfReader.GetPdfObject(((PdfDictionary)file).Get(PdfName.F));

                                    if (file.IsString())
                                    {
                                        map["File"] = ((PdfString)file).ToUnicodeString();
                                    }
                                }
                            }

                            var newWindow = PdfReader.GetPdfObjectRelease(action.Get(PdfName.Newwindow));

                            if (newWindow != null)
                            {
                                map["NewWindow"] = newWindow.ToString();
                            }
                        }
                        else if (PdfName.Launch.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S))))
                        {
                            map["Action"] = "Launch";
                            var file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.F));

                            if (file == null)
                            {
                                file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.Win));
                            }

                            if (file != null)
                            {
                                if (file.IsString())
                                {
                                    map["File"] = ((PdfString)file).ToUnicodeString();
                                }
                                else if (file.IsDictionary())
                                {
                                    file = PdfReader.GetPdfObjectRelease(((PdfDictionary)file).Get(PdfName.F));

                                    if (file.IsString())
                                    {
                                        map["File"] = ((PdfString)file).ToUnicodeString();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                //empty on purpose
            }

            var first = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.First));

            if (first != null)
            {
                map["Kids"] = bookmarkDepth(reader, first, pages);
            }

            list.Add(map);
            outline = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.Next));
        }

        return list;
    }

    /// <summary>
    ///     Gets number of indirect. If type of directed indirect is PAGES, it refers PAGE object through KIDS.
    ///     (Contributed by Kazuya Ujihara)
    ///     2004-06-13
    /// </summary>
    /// <param name="indirect"></param>
    private static int getNumber(PdfIndirectReference indirect)
    {
        var pdfObj = (PdfDictionary)PdfReader.GetPdfObjectRelease(indirect);

        if (pdfObj.Contains(PdfName.TYPE) && pdfObj.Get(PdfName.TYPE).Equals(PdfName.Pages) &&
            pdfObj.Contains(PdfName.Kids))
        {
            var kids = (PdfArray)pdfObj.Get(PdfName.Kids);
            indirect = (PdfIndirectReference)kids[0];
        }

        return indirect.Number;
    }

    private static string makeBookmarkParam(PdfArray dest, NullValueDictionary<int, int> pages)
    {
        var s = new StringBuilder();
        var obj = dest[0];

        if (obj.IsNumber())
        {
            s.Append(((PdfNumber)obj).IntValue + 1);
        }
        else
        {
            s.Append(pages[getNumber((PdfIndirectReference)obj)]); //changed by ujihara 2004-06-13
        }

        s.Append(' ').Append(dest[1].ToString().Substring(1));

        for (var k = 2; k < dest.Size; ++k)
        {
            s.Append(' ').Append(dest[k]);
        }

        return s.ToString();
    }

    private static void mapGotoBookmark(INullValueDictionary<string, object> map,
        PdfObject dest,
        NullValueDictionary<int, int> pages)
    {
        if (dest.IsString())
        {
            map["Named"] = dest.ToString();
        }
        else if (dest.IsName())
        {
            map["Named"] = PdfName.DecodeName(dest.ToString());
        }
        else if (dest.IsArray())
        {
            map["Page"] = makeBookmarkParam((PdfArray)dest, pages); //changed by ujihara 2004-06-13
        }

        map["Action"] = "GoTo";
    }
}