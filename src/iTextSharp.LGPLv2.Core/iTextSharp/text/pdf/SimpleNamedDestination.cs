using System;
using System.IO;
using System.Collections;
using System.Text;
using System.util;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public sealed class SimpleNamedDestination : ISimpleXmlDocHandler
    {

        private Hashtable _xmlLast;
        private Hashtable _xmlNames;

        private SimpleNamedDestination()
        {
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
                    ((int)c).ToString("", System.Globalization.CultureInfo.InvariantCulture);
                    string octal = "00" + Convert.ToString(c, 8);
                    buf.Append(octal.Substring(octal.Length - 3));
                }
                else if (c == '\\')
                    buf.Append("\\\\");
                else
                    buf.Append(c);
            }
            return buf.ToString();
        }

        /// <summary>
        /// Exports the destinations to XML. The DTD for this XML is:
        ///
        ///
        /// &lt;?xml version='1.0' encoding='UTF-8'?&gt;
        /// &lt;!ELEMENT Name (#PCDATA)&gt;
        /// &lt;!ATTLIST Name
        /// Page CDATA #IMPLIED
        /// &gt;
        /// &lt;!ELEMENT Destination (Name)*&gt;
        ///
        /// whatever the encoding
        /// @throws IOException on error
        /// </summary>
        /// <param name="names">the names</param>
        /// <param name="outp">the export destination. The stream is not closed</param>
        /// <param name="encoding">the encoding according to IANA conventions</param>
        /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
        public static void ExportToXml(Hashtable names, Stream outp, string encoding, bool onlyAscii)
        {
            StreamWriter wrt = new StreamWriter(outp, IanaEncodings.GetEncodingEncoding(encoding));
            ExportToXml(names, wrt, encoding, onlyAscii);
        }

        /// <summary>
        /// Exports the bookmarks to XML.
        /// whatever the encoding
        /// @throws IOException on error
        /// </summary>
        /// <param name="names">the names</param>
        /// <param name="wrt">the export destination. The writer is not closed</param>
        /// <param name="encoding">the encoding according to IANA conventions</param>
        /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true ,</param>
        public static void ExportToXml(Hashtable names, TextWriter wrt, string encoding, bool onlyAscii)
        {
            wrt.Write("<?xml version=\"1.0\" encoding=\"");
            wrt.Write(SimpleXmlParser.EscapeXml(encoding, onlyAscii));
            wrt.Write("\"?>\n<Destination>\n");
            foreach (string key in names.Keys)
            {
                string value = (string)names[key];
                wrt.Write("  <Name Page=\"");
                wrt.Write(SimpleXmlParser.EscapeXml(value, onlyAscii));
                wrt.Write("\">");
                wrt.Write(SimpleXmlParser.EscapeXml(EscapeBinaryString(key), onlyAscii));
                wrt.Write("</Name>\n");
            }
            wrt.Write("</Destination>\n");
            wrt.Flush();
        }

        public static Hashtable GetNamedDestination(PdfReader reader, bool fromNames)
        {
            IntHashtable pages = new IntHashtable();
            int numPages = reader.NumberOfPages;
            for (int k = 1; k <= numPages; ++k)
                pages[reader.GetPageOrigRef(k).Number] = k;
            Hashtable names = fromNames ? reader.GetNamedDestinationFromNames() : reader.GetNamedDestinationFromStrings();
            string[] keys = new string[names.Count];
            names.Keys.CopyTo(keys, 0);
            foreach (string name in keys)
            {
                PdfArray arr = (PdfArray)names[name];
                StringBuilder s = new StringBuilder();
                try
                {
                    s.Append(pages[(arr.GetAsIndirectObject(0)).Number]);
                    s.Append(' ').Append(arr[1].ToString().Substring(1));
                    for (int k = 2; k < arr.Size; ++k)
                        s.Append(' ').Append(arr[k]);
                    names[name] = s.ToString();
                }
                catch
                {
                    names.Remove(name);
                }
            }
            return names;
        }
        /// <summary>
        /// Import the names from XML.
        /// @throws IOException on error
        /// </summary>
        /// <param name="inp">the XML source. The stream is not closed</param>
        /// <returns>the names</returns>
        public static Hashtable ImportFromXml(Stream inp)
        {
            SimpleNamedDestination names = new SimpleNamedDestination();
            SimpleXmlParser.Parse(names, inp);
            return names._xmlNames;
        }

        /// <summary>
        /// Import the names from XML.
        /// @throws IOException on error
        /// </summary>
        /// <param name="inp">the XML source. The reader is not closed</param>
        /// <returns>the names</returns>
        public static Hashtable ImportFromXml(TextReader inp)
        {
            SimpleNamedDestination names = new SimpleNamedDestination();
            SimpleXmlParser.Parse(names, inp);
            return names._xmlNames;
        }

        public static PdfDictionary OutputNamedDestinationAsNames(Hashtable names, PdfWriter writer)
        {
            PdfDictionary dic = new PdfDictionary();
            foreach (string key in names.Keys)
            {
                try
                {
                    string value = (string)names[key];
                    PdfArray ar = CreateDestinationArray(value, writer);
                    PdfName kn = new PdfName(key);
                    dic.Put(kn, ar);
                }
                catch
                {
                    // empty on purpose
                }
            }
            return dic;
        }

        public static PdfDictionary OutputNamedDestinationAsStrings(Hashtable names, PdfWriter writer)
        {
            Hashtable n2 = new Hashtable();
            foreach (string key in names.Keys)
            {
                try
                {
                    string value = (string)names[key];
                    PdfArray ar = CreateDestinationArray(value, writer);
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
            if (tag.Equals("Destination"))
            {
                if (_xmlLast == null && _xmlNames != null)
                    return;
                else
                    throw new ArgumentException("Destination end tag out of place.");
            }
            if (!tag.Equals("Name"))
                throw new ArgumentException("Invalid end tag - " + tag);
            if (_xmlLast == null || _xmlNames == null)
                throw new ArgumentException("Name end tag out of place.");
            if (!_xmlLast.ContainsKey("Page"))
                throw new ArgumentException("Page attribute missing.");
            _xmlNames[UnEscapeBinaryString((string)_xmlLast["Name"])] = _xmlLast["Page"];
            _xmlLast = null;
        }

        public void StartDocument()
        {
        }

        public void StartElement(string tag, Hashtable h)
        {
            if (_xmlNames == null)
            {
                if (tag.Equals("Destination"))
                {
                    _xmlNames = new Hashtable();
                    return;
                }
                else
                    throw new ArgumentException("Root element is not Destination.");
            }
            if (!tag.Equals("Name"))
                throw new ArgumentException("Tag " + tag + " not allowed.");
            if (_xmlLast != null)
                throw new ArgumentException("Nested tags are not allowed.");
            _xmlLast = new Hashtable(h);
            _xmlLast["Name"] = "";
        }

        public void Text(string str)
        {
            if (_xmlLast == null)
                return;
            string name = (string)_xmlLast["Name"];
            name += str;
            _xmlLast["Name"] = name;
        }

        internal static PdfArray CreateDestinationArray(string value, PdfWriter writer)
        {
            PdfArray ar = new PdfArray();
            StringTokenizer tk = new StringTokenizer(value);
            int n = int.Parse(tk.NextToken(), CultureInfo.InvariantCulture);
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
            return ar;
        }
    }
}