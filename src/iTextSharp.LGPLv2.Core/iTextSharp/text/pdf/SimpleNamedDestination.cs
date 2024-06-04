using System.Text;
using System.util;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf;

/// <summary>
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public sealed class SimpleNamedDestination : ISimpleXmlDocHandler
{
    private NullValueDictionary<string, string> _xmlLast;
    private INullValueDictionary<string, string> _xmlNames;

    private SimpleNamedDestination()
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

        if (tag.Equals("Destination", StringComparison.Ordinal))
        {
            if (_xmlLast == null && _xmlNames != null)
            {
                return;
            }

            throw new ArgumentException("Destination end tag out of place.");
        }

        if (!tag.Equals("Name", StringComparison.Ordinal))
        {
            throw new ArgumentException("Invalid end tag - " + tag);
        }

        if (_xmlLast == null || _xmlNames == null)
        {
            throw new ArgumentException("Name end tag out of place.");
        }

        if (!_xmlLast.TryGetValue("Page", out var pageValue))
        {
            throw new ArgumentException("Page attribute missing.");
        }

        _xmlNames[UnEscapeBinaryString(_xmlLast["Name"])] = pageValue;
        _xmlLast = null;
    }

    public void StartDocument()
    {
    }

    public void Text(string str)
    {
        if (_xmlLast == null)
        {
            return;
        }

        var name = _xmlLast["Name"];
        name += str;
        _xmlLast["Name"] = name;
    }

    public void StartElement(string tag, INullValueDictionary<string, string> h)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (_xmlNames == null)
        {
            if (tag.Equals("Destination", StringComparison.Ordinal))
            {
                _xmlNames = new NullValueDictionary<string, string>();

                return;
            }

            throw new ArgumentException("Root element is not Destination.");
        }

        if (!tag.Equals("Name", StringComparison.Ordinal))
        {
            throw new ArgumentException("Tag " + tag + " not allowed.");
        }

        if (_xmlLast != null)
        {
            throw new ArgumentException("Nested tags are not allowed.");
        }

        _xmlLast = new NullValueDictionary<string, string>(h);
        _xmlLast["Name"] = "";
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
                ((int)c).ToString("", CultureInfo.InvariantCulture);
                var octal = "00" + Convert.ToString(c, 8);
                buf.Append(octal.Substring(octal.Length - 3));
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
    ///     Exports the destinations to XML. The DTD for this XML is:
    ///     &lt;?xml version='1.0' encoding='UTF-8'?&gt;
    ///     &lt;!ELEMENT Name (#PCDATA)&gt;
    ///     &lt;!ATTLIST Name
    ///     Page CDATA #IMPLIED
    ///     &gt;
    ///     &lt;!ELEMENT Destination (Name)*&gt;
    ///     whatever the encoding
    ///     @throws IOException on error
    /// </summary>
    /// <param name="names">the names</param>
    /// <param name="outp">the export destination. The stream is not closed</param>
    /// <param name="encoding">the encoding according to IANA conventions</param>
    /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
    public static void ExportToXml(INullValueDictionary<string, string> names,
        Stream outp,
        string encoding,
        bool onlyAscii)
    {
        using var wrt = new StreamWriter(outp, IanaEncodings.GetEncodingEncoding(encoding));
        ExportToXml(names, wrt, encoding, onlyAscii);
    }

    /// <summary>
    ///     Exports the bookmarks to XML.
    ///     whatever the encoding
    ///     @throws IOException on error
    /// </summary>
    /// <param name="names">the names</param>
    /// <param name="wrt">the export destination. The writer is not closed</param>
    /// <param name="encoding">the encoding according to IANA conventions</param>
    /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
    public static void ExportToXml(INullValueDictionary<string, string> names,
        TextWriter wrt,
        string encoding,
        bool onlyAscii)
    {
        if (names == null)
        {
            throw new ArgumentNullException(nameof(names));
        }

        if (wrt == null)
        {
            throw new ArgumentNullException(nameof(wrt));
        }

        wrt.Write("<?xml version=\"1.0\" encoding=\"");
        wrt.Write(SimpleXmlParser.EscapeXml(encoding, onlyAscii));
        wrt.Write("\"?>\n<Destination>\n");

        foreach (var key in names.Keys)
        {
            var value = names[key];
            wrt.Write("  <Name Page=\"");
            wrt.Write(SimpleXmlParser.EscapeXml(value, onlyAscii));
            wrt.Write("\">");
            wrt.Write(SimpleXmlParser.EscapeXml(EscapeBinaryString(key), onlyAscii));
            wrt.Write("</Name>\n");
        }

        wrt.Write("</Destination>\n");
        wrt.Flush();
    }

    public static INullValueDictionary<string, string> GetNamedDestination(PdfReader reader, bool fromNames)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var pages = new NullValueDictionary<int, int>();
        var numPages = reader.NumberOfPages;

        for (var k = 1; k <= numPages; ++k)
        {
            pages[reader.GetPageOrigRef(k).Number] = k;
        }

        var names = fromNames ? reader.GetNamedDestinationFromNames() : reader.GetNamedDestinationFromStrings();
        var n2 = new NullValueDictionary<string, string>(names.Count);
        var keys = new string[names.Count];
        names.Keys.CopyTo(keys, 0);

        foreach (var name in keys)
        {
            var arr = (PdfArray)names[name];
            var s = new StringBuilder();

            try
            {
                s.Append(pages[arr.GetAsIndirectObject(0).Number]);
                s.Append(' ').Append(arr[1].ToString().Substring(1));

                for (var k = 2; k < arr.Size; ++k)
                {
                    s.Append(' ').Append(arr[k]);
                }

                n2[name] = s.ToString();
            }
            catch
            {
            }
        }

        return n2;
    }

    /// <summary>
    ///     Import the names from XML.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="inp">the XML source. The stream is not closed</param>
    /// <returns>the names</returns>
    public static INullValueDictionary<string, string> ImportFromXml(Stream inp)
    {
        var names = new SimpleNamedDestination();
        SimpleXmlParser.Parse(names, inp);

        return names._xmlNames;
    }

    /// <summary>
    ///     Import the names from XML.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="inp">the XML source. The reader is not closed</param>
    /// <returns>the names</returns>
    public static INullValueDictionary<string, string> ImportFromXml(TextReader inp)
    {
        var names = new SimpleNamedDestination();
        SimpleXmlParser.Parse(names, inp);

        return names._xmlNames;
    }

    public static PdfDictionary OutputNamedDestinationAsNames(INullValueDictionary<string, string> names,
        PdfWriter writer)
    {
        if (names == null)
        {
            throw new ArgumentNullException(nameof(names));
        }

        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        var dic = new PdfDictionary();

        foreach (var key in names.Keys)
        {
            try
            {
                var value = names[key];
                var ar = CreateDestinationArray(value, writer);
                var kn = new PdfName(key);
                dic.Put(kn, ar);
            }
            catch
            {
                // empty on purpose
            }
        }

        return dic;
    }

    public static PdfDictionary OutputNamedDestinationAsStrings(INullValueDictionary<string, string> names,
        PdfWriter writer)
    {
        if (names == null)
        {
            throw new ArgumentNullException(nameof(names));
        }

        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        var n2 = new NullValueDictionary<string, PdfObject>();

        foreach (var key in names.Keys)
        {
            try
            {
                var value = names[key];
                var ar = CreateDestinationArray(value, writer);
                n2[key] = writer.AddToBody(ar).IndirectReference;
            }
            catch
            {
                // empty on purpose
            }
        }

        return PdfNameTree.WriteTree(n2, writer);
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

    internal static PdfArray CreateDestinationArray(string value, PdfWriter writer)
    {
        var ar = new PdfArray();
        var tk = new StringTokenizer(value);
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

        return ar;
    }
}