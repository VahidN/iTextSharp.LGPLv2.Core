using System;
using System.IO;
using System.Collections;
using System.Text;
using System.util;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Bookmark processing in a simple way. It has some limitations, mainly the only
    /// action types supported are GoTo, GoToR, URI and Launch.
    ///
    /// The list structure is composed by a number of Hashtable, keyed by strings, one Hashtable
    /// for each bookmark.
    /// The element values are all strings with the exception of the key "Kids" that has
    /// another list for the child bookmarks.
    ///
    /// All the bookmarks have a "Title" with the
    /// bookmark title and optionally a "Style" that can be "bold", "italic" or a
    /// combination of both. They can also have a "Color" key with a value of three
    /// floats separated by spaces. The key "Open" can have the values "true" or "false" and
    /// signals the open status of the children. It's "true" by default.
    ///
    /// The actions and the parameters can be:
    ///
    ///  "Action" = "GoTo" - "Page" | "Named"
    ///
    ///  "Page" = "3 XYZ 70 400 null" - page number followed by a destination (/XYZ is also accepted)
    ///  "Named" = "named_destination"
    ///
    ///  "Action" = "GoToR" - "Page" | "Named" | "NamedN", "File", ["NewWindow"]
    ///
    ///  "Page" = "3 XYZ 70 400 null" - page number followed by a destination (/XYZ is also accepted)
    ///  "Named" = "named_destination_as_a_string"
    ///  "NamedN" = "named_destination_as_a_name"
    ///  "File" - "the_file_to_open"
    ///  "NewWindow" - "true" or "false"
    ///
    ///  "Action" = "URI" - "URI"
    ///
    ///  "URI" = "http://sf.net" - URI to jump to
    ///
    ///  "Action" = "Launch" - "File"
    ///
    ///  "File" - "the_file_to_open_or_execute"
    ///
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public sealed class SimpleBookmark : ISimpleXmlDocHandler
    {

        private readonly Stack _attr = new Stack();
        private ArrayList _topList;

        /// <summary>
        /// Creates a new instance of SimpleBookmark
        /// </summary>
        private SimpleBookmark()
        {
        }

        /// <summary>
        /// Removes the bookmark entries for a number of page ranges. The page ranges
        /// consists of a number of pairs with the start/end page range. The page numbers
        /// are inclusive.
        /// </summary>
        /// <param name="list">the bookmarks</param>
        /// <param name="pageRange">the page ranges, always in pairs.</param>
        public static void EliminatePages(ArrayList list, int[] pageRange)
        {
            if (list == null)
                return;

            for (ListIterator it = new ListIterator(list); it.HasNext();)
            {
                Hashtable map = (Hashtable)it.Next();
                bool hit = false;
                if ("GoTo".Equals(map["Action"]))
                {
                    string page = (string)map["Page"];
                    if (page != null)
                    {
                        page = page.Trim();
                        int idx = page.IndexOf(' ');
                        int pageNum;
                        if (idx < 0)
                            pageNum = int.Parse(page);
                        else
                            pageNum = int.Parse(page.Substring(0, idx));
                        int len = pageRange.Length & 0x7ffffffe;
                        for (int k = 0; k < len; k += 2)
                        {
                            if (pageNum >= pageRange[k] && pageNum <= pageRange[k + 1])
                            {
                                hit = true;
                                break;
                            }
                        }
                    }
                }
                ArrayList kids = (ArrayList)map["Kids"];
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
                        it.Remove();
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
            StringBuilder buf = new StringBuilder();
            char[] cc = s.ToCharArray();
            int len = cc.Length;
            for (int k = 0; k < len; ++k)
            {
                char c = cc[k];
                if (c < ' ')
                {
                    buf.Append('\\');
                    int v = c;
                    string octal = "";
                    do
                    {
                        int x = v % 8;
                        octal = x + octal;
                        v /= 8;
                    } while (v > 0);
                    buf.Append(octal.PadLeft(3, '0'));
                }
                else if (c == '\\')
                    buf.Append("\\\\");
                else
                    buf.Append(c);
            }
            return buf.ToString();
        }

        /// <summary>
        /// Exports the bookmarks to XML. The DTD for this XML is:
        ///
        ///
        /// &lt;?xml version='1.0' encoding='UTF-8'?&gt;
        /// &lt;!ELEMENT Title (#PCDATA|Title)*&gt;
        /// &lt;!ATTLIST Title
        /// Action CDATA #IMPLIED
        /// Open CDATA #IMPLIED
        /// Page CDATA #IMPLIED
        /// URI CDATA #IMPLIED
        /// File CDATA #IMPLIED
        /// Named CDATA #IMPLIED
        /// NamedN CDATA #IMPLIED
        /// NewWindow CDATA #IMPLIED
        /// Style CDATA #IMPLIED
        /// Color CDATA #IMPLIED
        /// &gt;
        /// &lt;!ELEMENT Bookmark (Title)*&gt;
        ///
        /// whatever the encoding
        /// @throws IOException on error
        /// </summary>
        /// <param name="list">the bookmarks</param>
        /// <param name="outp">the export destination. The stream is not closed</param>
        /// <param name="encoding">the encoding according to IANA conventions</param>
        /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
        public static void ExportToXml(ArrayList list, Stream outp, string encoding, bool onlyAscii)
        {
            StreamWriter wrt = new StreamWriter(outp, IanaEncodings.GetEncodingEncoding(encoding));
            ExportToXml(list, wrt, encoding, onlyAscii);
        }

        /// <summary>
        /// Exports the bookmarks to XML.
        /// whatever the encoding
        /// @throws IOException on error
        /// </summary>
        /// <param name="list">the bookmarks</param>
        /// <param name="wrt">the export destination. The writer is not closed</param>
        /// <param name="encoding">the encoding according to IANA conventions</param>
        /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
        public static void ExportToXml(ArrayList list, TextWriter wrt, string encoding, bool onlyAscii)
        {
            wrt.Write("<?xml version=\"1.0\" encoding=\"");
            wrt.Write(SimpleXmlParser.EscapeXml(encoding, onlyAscii));
            wrt.Write("\"?>\n<Bookmark>\n");
            ExportToXmlNode(list, wrt, 1, onlyAscii);
            wrt.Write("</Bookmark>\n");
            wrt.Flush();
        }

        /// <summary>
        /// Exports the bookmarks to XML. Only of use if the generation is to be include in
        /// some other XML document.
        /// whatever the encoding
        /// @throws IOException on error
        /// </summary>
        /// <param name="list">the bookmarks</param>
        /// <param name="outp">the export destination. The writer is not closed</param>
        /// <param name="indent">the indentation level. Pretty printing significant only</param>
        /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
        public static void ExportToXmlNode(ArrayList list, TextWriter outp, int indent, bool onlyAscii)
        {
            string dep = "";
            for (int k = 0; k < indent; ++k)
                dep += "  ";
            foreach (Hashtable map in list)
            {
                string title = null;
                outp.Write(dep);
                outp.Write("<Title ");
                ArrayList kids = null;
                foreach (DictionaryEntry entry in map)
                {
                    string key = (string)entry.Key;
                    if (key.Equals("Title"))
                    {
                        title = (string)entry.Value;
                        continue;
                    }
                    else if (key.Equals("Kids"))
                    {
                        kids = (ArrayList)entry.Value;
                        continue;
                    }
                    else
                    {
                        outp.Write(key);
                        outp.Write("=\"");
                        string value = (string)entry.Value;
                        if (key.Equals("Named") || key.Equals("NamedN"))
                            value = EscapeBinaryString(value);
                        outp.Write(SimpleXmlParser.EscapeXml(value, onlyAscii));
                        outp.Write("\" ");
                    }
                }
                outp.Write(">");
                if (title == null)
                    title = "";
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
        /// Gets a  List  with the bookmarks. It returns  null  if
        /// the document doesn't have any bookmarks.
        /// document doesn't have any
        /// </summary>
        /// <param name="reader">the document</param>
        /// <returns>a  List  with the bookmarks or  null  if the</returns>
        public static ArrayList GetBookmark(PdfReader reader)
        {
            PdfDictionary catalog = reader.Catalog;
            PdfObject obj = PdfReader.GetPdfObjectRelease(catalog.Get(PdfName.Outlines));
            if (obj == null || !obj.IsDictionary())
                return null;
            PdfDictionary outlines = (PdfDictionary)obj;
            IntHashtable pages = new IntHashtable();
            int numPages = reader.NumberOfPages;
            for (int k = 1; k <= numPages; ++k)
            {
                pages[reader.GetPageOrigRef(k).Number] = k;
                reader.ReleasePage(k);
            }
            return bookmarkDepth(reader, (PdfDictionary)PdfReader.GetPdfObjectRelease(outlines.Get(PdfName.First)), pages);
        }

        /// <summary>
        /// Import the bookmarks from XML.
        /// @throws IOException on error
        /// </summary>
        /// <param name="inp">the XML source. The stream is not closed</param>
        /// <returns>the bookmarks</returns>
        public static ArrayList ImportFromXml(Stream inp)
        {
            SimpleBookmark book = new SimpleBookmark();
            SimpleXmlParser.Parse(book, inp);
            return book._topList;
        }

        /// <summary>
        /// Import the bookmarks from XML.
        /// @throws IOException on error
        /// </summary>
        /// <param name="inp">the XML source. The reader is not closed</param>
        /// <returns>the bookmarks</returns>
        public static ArrayList ImportFromXml(TextReader inp)
        {
            SimpleBookmark book = new SimpleBookmark();
            SimpleXmlParser.Parse(book, inp);
            return book._topList;
        }

        public static object[] IterateOutlines(PdfWriter writer, PdfIndirectReference parent, ArrayList kids, bool namedAsNames)
        {
            PdfIndirectReference[] refs = new PdfIndirectReference[kids.Count];
            for (int k = 0; k < refs.Length; ++k)
                refs[k] = writer.PdfIndirectReference;
            int ptr = 0;
            int count = 0;
            foreach (Hashtable map in kids)
            {
                object[] lower = null;
                ArrayList subKid = (ArrayList)map["Kids"];
                if (subKid != null && subKid.Count > 0)
                    lower = IterateOutlines(writer, refs[ptr], subKid, namedAsNames);
                PdfDictionary outline = new PdfDictionary();
                ++count;
                if (lower != null)
                {
                    outline.Put(PdfName.First, (PdfIndirectReference)lower[0]);
                    outline.Put(PdfName.Last, (PdfIndirectReference)lower[1]);
                    int n = (int)lower[2];
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
                    outline.Put(PdfName.Prev, refs[ptr - 1]);
                if (ptr < refs.Length - 1)
                    outline.Put(PdfName.Next, refs[ptr + 1]);
                outline.Put(PdfName.Title, new PdfString((string)map["Title"], PdfObject.TEXT_UNICODE));
                string color = (string)map["Color"];
                if (color != null)
                {
                    try
                    {
                        PdfArray arr = new PdfArray();
                        StringTokenizer tk = new StringTokenizer(color);
                        for (int k = 0; k < 3; ++k)
                        {
                            float f = float.Parse(tk.NextToken(), System.Globalization.NumberFormatInfo.InvariantInfo);
                            if (f < 0) f = 0;
                            if (f > 1) f = 1;
                            arr.Add(new PdfNumber(f));
                        }
                        outline.Put(PdfName.C, arr);
                    }
                    catch { } //in case it's malformed
                }
                string style = (string)map["Style"];
                if (style != null)
                {
                    int bits = 0;
                    if (style.IndexOf("italic", StringComparison.OrdinalIgnoreCase) >= 0)
                        bits |= 1;
                    if (style.IndexOf("bold", StringComparison.OrdinalIgnoreCase) >= 0)
                        bits |= 2;
                    if (bits != 0)
                        outline.Put(PdfName.F, new PdfNumber(bits));
                }
                CreateOutlineAction(outline, map, writer, namedAsNames);
                writer.AddToBody(outline, refs[ptr]);
                ++ptr;
            }
            return new object[] { refs[0], refs[refs.Length - 1], count };
        }

        /// <summary>
        /// For the pages in range add the  pageShift  to the page number.
        /// The page ranges
        /// consists of a number of pairs with the start/end page range. The page numbers
        /// are inclusive.
        /// to include all the pages
        /// </summary>
        /// <param name="list">the bookmarks</param>
        /// <param name="pageShift">the number to add to the pages in range</param>
        /// <param name="pageRange">the page ranges, always in pairs. It can be  null </param>
        public static void ShiftPageNumbers(ArrayList list, int pageShift, int[] pageRange)
        {
            if (list == null)
                return;
            foreach (Hashtable map in list)
            {
                if ("GoTo".Equals(map["Action"]))
                {
                    string page = (string)map["Page"];
                    if (page != null)
                    {
                        page = page.Trim();
                        int idx = page.IndexOf(' ');
                        int pageNum;
                        if (idx < 0)
                            pageNum = int.Parse(page);
                        else
                            pageNum = int.Parse(page.Substring(0, idx));
                        bool hit = false;
                        if (pageRange == null)
                            hit = true;
                        else
                        {
                            int len = pageRange.Length & 0x7ffffffe;
                            for (int k = 0; k < len; k += 2)
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
                                page = (pageNum + pageShift) + "";
                            else
                                page = (pageNum + pageShift) + page.Substring(idx);
                        }
                        map["Page"] = page;
                    }
                }
                ArrayList kids = (ArrayList)map["Kids"];
                if (kids != null)
                    ShiftPageNumbers(kids, pageShift, pageRange);
            }
        }

        public static string UnEscapeBinaryString(string s)
        {
            StringBuilder buf = new StringBuilder();
            char[] cc = s.ToCharArray();
            int len = cc.Length;
            for (int k = 0; k < len; ++k)
            {
                char c = cc[k];
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
                        int n = c - '0';
                        ++k;
                        for (int j = 0; j < 2 && k < len; ++j)
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
                        buf.Append(c);
                }
                else
                    buf.Append(c);
            }
            return buf.ToString();
        }

        public void EndDocument()
        {
        }

        public void EndElement(string tag)
        {
            if (tag.Equals("Bookmark"))
            {
                if (_attr.Count == 0)
                    return;
                else
                    throw new Exception("Bookmark end tag out of place.");
            }
            if (!tag.Equals("Title"))
                throw new Exception("Invalid end tag - " + tag);
            Hashtable attributes = (Hashtable)_attr.Pop();
            string title = (string)attributes["Title"];
            attributes["Title"] = title.Trim();
            string named = (string)attributes["Named"];
            if (named != null)
                attributes["Named"] = UnEscapeBinaryString(named);
            named = (string)attributes["NamedN"];
            if (named != null)
                attributes["NamedN"] = UnEscapeBinaryString(named);
            if (_attr.Count == 0)
                _topList.Add(attributes);
            else
            {
                Hashtable parent = (Hashtable)_attr.Peek();
                ArrayList kids = (ArrayList)parent["Kids"];
                if (kids == null)
                {
                    kids = new ArrayList();
                    parent["Kids"] = kids;
                }
                kids.Add(attributes);
            }
        }

        public void StartDocument()
        {
        }

        public void StartElement(string tag, Hashtable h)
        {
            if (_topList == null)
            {
                if (tag.Equals("Bookmark"))
                {
                    _topList = new ArrayList();
                    return;
                }
                else
                    throw new Exception("Root element is not Bookmark: " + tag);
            }
            if (!tag.Equals("Title"))
                throw new Exception("Tag " + tag + " not allowed.");
            Hashtable attributes = new Hashtable(h);
            attributes["Title"] = "";
            attributes.Remove("Kids");
            _attr.Push(attributes);
        }

        public void Text(string str)
        {
            if (_attr.Count == 0)
                return;
            Hashtable attributes = (Hashtable)_attr.Peek();
            string title = (string)attributes["Title"];
            title += str;
            attributes["Title"] = title;
        }

        internal static void CreateOutlineAction(PdfDictionary outline, Hashtable map, PdfWriter writer, bool namedAsNames)
        {
            try
            {
                string action = (string)map["Action"];
                if ("GoTo".Equals(action))
                {
                    string p;
                    if ((p = (string)map["Named"]) != null)
                    {
                        if (namedAsNames)
                            outline.Put(PdfName.Dest, new PdfName(p));
                        else
                            outline.Put(PdfName.Dest, new PdfString(p, null));
                    }
                    else if ((p = (string)map["Page"]) != null)
                    {
                        PdfArray ar = new PdfArray();
                        StringTokenizer tk = new StringTokenizer(p);
                        int n = int.Parse(tk.NextToken());
                        ar.Add(writer.GetPageReference(n));
                        if (!tk.HasMoreTokens())
                        {
                            ar.Add(PdfName.Xyz);
                            ar.Add(new float[] { 0, 10000, 0 });
                        }
                        else
                        {
                            string fn = tk.NextToken();
                            if (fn.StartsWith("/"))
                                fn = fn.Substring(1);
                            ar.Add(new PdfName(fn));
                            for (int k = 0; k < 4 && tk.HasMoreTokens(); ++k)
                            {
                                fn = tk.NextToken();
                                if (fn.Equals("null"))
                                    ar.Add(PdfNull.Pdfnull);
                                else
                                    ar.Add(new PdfNumber(fn));
                            }
                        }
                        outline.Put(PdfName.Dest, ar);
                    }
                }
                else if ("GoToR".Equals(action))
                {
                    string p;
                    PdfDictionary dic = new PdfDictionary();
                    if ((p = (string)map["Named"]) != null)
                        dic.Put(PdfName.D, new PdfString(p, null));
                    else if ((p = (string)map["NamedN"]) != null)
                        dic.Put(PdfName.D, new PdfName(p));
                    else if ((p = (string)map["Page"]) != null)
                    {
                        PdfArray ar = new PdfArray();
                        StringTokenizer tk = new StringTokenizer(p);
                        ar.Add(new PdfNumber(tk.NextToken()));
                        if (!tk.HasMoreTokens())
                        {
                            ar.Add(PdfName.Xyz);
                            ar.Add(new float[] { 0, 10000, 0 });
                        }
                        else
                        {
                            string fn = tk.NextToken();
                            if (fn.StartsWith("/"))
                                fn = fn.Substring(1);
                            ar.Add(new PdfName(fn));
                            for (int k = 0; k < 4 && tk.HasMoreTokens(); ++k)
                            {
                                fn = tk.NextToken();
                                if (fn.Equals("null"))
                                    ar.Add(PdfNull.Pdfnull);
                                else
                                    ar.Add(new PdfNumber(fn));
                            }
                        }
                        dic.Put(PdfName.D, ar);
                    }
                    string file = (string)map["File"];
                    if (dic.Size > 0 && file != null)
                    {
                        dic.Put(PdfName.S, PdfName.Gotor);
                        dic.Put(PdfName.F, new PdfString(file));
                        string nw = (string)map["NewWindow"];
                        if (nw != null)
                        {
                            if (nw.Equals("true"))
                                dic.Put(PdfName.Newwindow, PdfBoolean.Pdftrue);
                            else if (nw.Equals("false"))
                                dic.Put(PdfName.Newwindow, PdfBoolean.Pdffalse);
                        }
                        outline.Put(PdfName.A, dic);
                    }
                }
                else if ("URI".Equals(action))
                {
                    string uri = (string)map["URI"];
                    if (uri != null)
                    {
                        PdfDictionary dic = new PdfDictionary();
                        dic.Put(PdfName.S, PdfName.Uri);
                        dic.Put(PdfName.Uri, new PdfString(uri));
                        outline.Put(PdfName.A, dic);
                    }
                }
                else if ("Launch".Equals(action))
                {
                    string file = (string)map["File"];
                    if (file != null)
                    {
                        PdfDictionary dic = new PdfDictionary();
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

        private static ArrayList bookmarkDepth(PdfReader reader, PdfDictionary outline, IntHashtable pages)
        {
            ArrayList list = new ArrayList();
            while (outline != null)
            {
                Hashtable map = new Hashtable();
                PdfString title = (PdfString)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.Title));
                map["Title"] = title.ToUnicodeString();
                PdfArray color = (PdfArray)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.C));
                if (color != null && color.Size == 3)
                {
                    ByteBuffer outp = new ByteBuffer();
                    outp.Append(color.GetAsNumber(0).FloatValue).Append(' ');
                    outp.Append(color.GetAsNumber(1).FloatValue).Append(' ');
                    outp.Append(color.GetAsNumber(2).FloatValue);
                    map["Color"] = PdfEncodings.ConvertToString(outp.ToByteArray(), null);
                }
                PdfNumber style = (PdfNumber)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.F));
                if (style != null)
                {
                    int f = style.IntValue;
                    string s = "";
                    if ((f & 1) != 0)
                        s += "italic ";
                    if ((f & 2) != 0)
                        s += "bold ";
                    s = s.Trim();
                    if (s.Length != 0)
                        map["Style"] = s;
                }
                PdfNumber count = (PdfNumber)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.Count));
                if (count != null && count.IntValue < 0)
                    map["Open"] = "false";
                try
                {
                    PdfObject dest = PdfReader.GetPdfObjectRelease(outline.Get(PdfName.Dest));
                    if (dest != null)
                    {
                        mapGotoBookmark(map, dest, pages); //changed by ujihara 2004-06-13
                    }
                    else
                    {
                        PdfDictionary action = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.A));
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
                                map["URI"] = ((PdfString)PdfReader.GetPdfObjectRelease(action.Get(PdfName.Uri))).ToUnicodeString();
                            }
                            else if (PdfName.Gotor.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S))))
                            {
                                dest = PdfReader.GetPdfObjectRelease(action.Get(PdfName.D));
                                if (dest != null)
                                {
                                    if (dest.IsString())
                                        map["Named"] = dest.ToString();
                                    else if (dest.IsName())
                                        map["NamedN"] = PdfName.DecodeName(dest.ToString());
                                    else if (dest.IsArray())
                                    {
                                        PdfArray arr = (PdfArray)dest;
                                        StringBuilder s = new StringBuilder();
                                        s.Append(arr[0]);
                                        s.Append(' ').Append(arr[1]);
                                        for (int k = 2; k < arr.Size; ++k)
                                            s.Append(' ').Append(arr[k]);
                                        map["Page"] = s.ToString();
                                    }
                                }
                                map["Action"] = "GoToR";
                                PdfObject file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.F));
                                if (file != null)
                                {
                                    if (file.IsString())
                                        map["File"] = ((PdfString)file).ToUnicodeString();
                                    else if (file.IsDictionary())
                                    {
                                        file = PdfReader.GetPdfObject(((PdfDictionary)file).Get(PdfName.F));
                                        if (file.IsString())
                                            map["File"] = ((PdfString)file).ToUnicodeString();
                                    }
                                }
                                PdfObject newWindow = PdfReader.GetPdfObjectRelease(action.Get(PdfName.Newwindow));
                                if (newWindow != null)
                                    map["NewWindow"] = newWindow.ToString();
                            }
                            else if (PdfName.Launch.Equals(PdfReader.GetPdfObjectRelease(action.Get(PdfName.S))))
                            {
                                map["Action"] = "Launch";
                                PdfObject file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.F));
                                if (file == null)
                                    file = PdfReader.GetPdfObjectRelease(action.Get(PdfName.Win));
                                if (file != null)
                                {
                                    if (file.IsString())
                                        map["File"] = ((PdfString)file).ToUnicodeString();
                                    else if (file.IsDictionary())
                                    {
                                        file = PdfReader.GetPdfObjectRelease(((PdfDictionary)file).Get(PdfName.F));
                                        if (file.IsString())
                                            map["File"] = ((PdfString)file).ToUnicodeString();
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
                PdfDictionary first = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline.Get(PdfName.First));
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
        /// Gets number of indirect. If type of directed indirect is PAGES, it refers PAGE object through KIDS.
        /// (Contributed by Kazuya Ujihara)
        /// 2004-06-13
        /// </summary>
        /// <param name="indirect"></param>
        private static int getNumber(PdfIndirectReference indirect)
        {
            PdfDictionary pdfObj = (PdfDictionary)PdfReader.GetPdfObjectRelease(indirect);
            if (pdfObj.Contains(PdfName.TYPE) && pdfObj.Get(PdfName.TYPE).Equals(PdfName.Pages) && pdfObj.Contains(PdfName.Kids))
            {
                PdfArray kids = (PdfArray)pdfObj.Get(PdfName.Kids);
                indirect = (PdfIndirectReference)kids[0];
            }
            return indirect.Number;
        }

        private static string makeBookmarkParam(PdfArray dest, IntHashtable pages)
        {
            StringBuilder s = new StringBuilder();
            PdfObject obj = dest[0];
            if (obj.IsNumber())
                s.Append(((PdfNumber)obj).IntValue + 1);
            else
                s.Append(pages[getNumber((PdfIndirectReference)obj)]); //changed by ujihara 2004-06-13
            s.Append(' ').Append(dest[1].ToString().Substring(1));
            for (int k = 2; k < dest.Size; ++k)
                s.Append(' ').Append(dest[k]);
            return s.ToString();
        }

        private static void mapGotoBookmark(Hashtable map, PdfObject dest, IntHashtable pages)
        {
            if (dest.IsString())
                map["Named"] = dest.ToString();
            else if (dest.IsName())
                map["Named"] = PdfName.DecodeName(dest.ToString());
            else if (dest.IsArray())
                map["Page"] = makeBookmarkParam((PdfArray)dest, pages); //changed by ujihara 2004-06-13
            map["Action"] = "GoTo";
        }
    }
}
