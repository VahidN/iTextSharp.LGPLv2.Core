using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Processes XFA forms.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class XfaForm
    {
        public const string XFA_DATA_SCHEMA = "http://www.xfa.org/schema/xfa-data/1.0/";
        private XmlDocument _domDocument;
        private XmlNode _templateNode;

        /// <summary>
        /// An empty constructor to build on.
        /// </summary>
        public XfaForm()
        {
        }

        /// <summary>
        /// A constructor from a  PdfReader . It basically does everything
        /// from finding the XFA stream to the XML parsing.
        /// @throws java.io.IOException on error
        /// @throws javax.xml.parsers.ParserConfigurationException on error
        /// @throws org.xml.sax.SAXException on error
        /// </summary>
        /// <param name="reader">the reader</param>
        public XfaForm(PdfReader reader)
        {
            Reader = reader;
            PdfObject xfa = GetXfaObject(reader);
            if (xfa == null)
            {
                XfaPresent = false;
                return;
            }
            XfaPresent = true;
            MemoryStream bout = new MemoryStream();
            if (xfa.IsArray())
            {
                PdfArray ar = (PdfArray)xfa;
                for (int k = 1; k < ar.Size; k += 2)
                {
                    PdfObject ob = ar.GetDirectObject(k);
                    if (ob is PrStream)
                    {
                        byte[] b = PdfReader.GetStreamBytes((PrStream)ob);
                        bout.Write(b, 0, b.Length);
                    }
                }
            }
            else if (xfa is PrStream)
            {
                byte[] b = PdfReader.GetStreamBytes((PrStream)xfa);
                bout.Write(b, 0, b.Length);
            }
            bout.Seek(0, SeekOrigin.Begin);
            XmlReader xtr = XmlReader.Create(bout);
            _domDocument = new XmlDocument();
            _domDocument.PreserveWhitespace = true;
            _domDocument.Load(xtr);
            extractNodes();
        }

        /// <summary>
        /// Gets the class that contains the "classic" fields processing.
        /// </summary>
        /// <returns>the class that contains the "classic" fields processing</returns>
        public AcroFieldsSearch AcroFieldsSom { get; set; }

        /// <summary>
        /// Checks if this XFA form was changed.
        /// </summary>
        /// <returns> true  if this XFA form was changed</returns>
        public bool Changed { get; set; }

        /// <summary>
        /// Gets the  Node  that corresponds to the datasets part.
        /// </summary>
        /// <returns>the  Node  that corresponds to the datasets part</returns>
        public XmlNode DatasetsNode { get; private set; }

        /// <summary>
        /// Gets the class that contains the datasets processing section of the XFA.
        /// </summary>
        /// <returns>the class that contains the datasets processing section of the XFA</returns>
        public Xml2SomDatasets DatasetsSom { get; set; }

        /// <summary>
        /// Gets the top level DOM document.
        /// </summary>
        /// <returns>the top level DOM document</returns>
        public XmlDocument DomDocument
        {
            get
            {
                return _domDocument;
            }
            set
            {
                _domDocument = value;
                extractNodes();
            }
        }

        /// <summary>
        /// Sets the  PdfReader  to be used by this instance.
        /// </summary>
        public PdfReader Reader { set; get; }

        /// <summary>
        /// Gets the class that contains the template processing section of the XFA.
        /// </summary>
        /// <returns>the class that contains the template processing section of the XFA</returns>
        public Xml2SomTemplate TemplateSom { get; set; }

        /// <summary>
        /// Returns  true  if it is a XFA form.
        /// </summary>
        /// <returns> true  if it is a XFA form</returns>
        public bool XfaPresent { get; set; }

        /// <summary>
        /// Gets all the text contained in the child nodes of this node.
        /// </summary>
        /// <param name="n">the  Node </param>
        /// <returns>the text found or "" if no text was found</returns>
        public static string GetNodeText(XmlNode n)
        {
            if (n == null)
                return "";
            return getNodeText(n, "");

        }

        /// <summary>
        /// Return the XFA Object, could be an array, could be a Stream.
        /// Returns null f no XFA Object is present.
        /// @since    2.1.3
        /// </summary>
        /// <param name="reader">a PdfReader instance</param>
        /// <returns>the XFA object</returns>
        public static PdfObject GetXfaObject(PdfReader reader)
        {
            PdfDictionary af = (PdfDictionary)PdfReader.GetPdfObjectRelease(reader.Catalog.Get(PdfName.Acroform));
            if (af == null)
            {
                return null;
            }
            return PdfReader.GetPdfObjectRelease(af.Get(PdfName.Xfa));
        }
        /// <summary>
        /// Serializes a XML document to a byte array.
        /// @throws java.io.IOException on error
        /// </summary>
        /// <param name="n">the XML document</param>
        /// <returns>the serialized XML document</returns>
        public static byte[] SerializeDoc(XmlNode n)
        {
            MemoryStream fout = new MemoryStream();
            var xwSettings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false)
            };
            var xw = XmlWriter.Create(fout, xwSettings);
            n.WriteContentTo(xw);
            xw.Dispose();
            return fout.ToArray();
        }

        /// <summary>
        /// Sets the XFA key from a byte array. The old XFA is erased.
        /// @throws java.io.IOException on error
        /// </summary>
        /// <param name="form">the data</param>
        /// <param name="reader">the reader</param>
        /// <param name="writer">the writer</param>
        public static void SetXfa(XfaForm form, PdfReader reader, PdfWriter writer)
        {
            PdfDictionary af = (PdfDictionary)PdfReader.GetPdfObjectRelease(reader.Catalog.Get(PdfName.Acroform));
            if (af == null)
            {
                return;
            }
            PdfObject xfa = GetXfaObject(reader);
            if (xfa.IsArray())
            {
                PdfArray ar = (PdfArray)xfa;
                int t = -1;
                int d = -1;
                for (int k = 0; k < ar.Size; k += 2)
                {
                    PdfString s = ar.GetAsString(k);
                    if ("template".Equals(s.ToString()))
                    {
                        t = k + 1;
                    }
                    if ("datasets".Equals(s.ToString()))
                    {
                        d = k + 1;
                    }
                }
                if (t > -1 && d > -1)
                {
                    reader.KillXref(ar.GetAsIndirectObject(t));
                    reader.KillXref(ar.GetAsIndirectObject(d));
                    PdfStream tStream = new PdfStream(SerializeDoc(form._templateNode));
                    tStream.FlateCompress(writer.CompressionLevel);
                    ar[t] = writer.AddToBody(tStream).IndirectReference;
                    PdfStream dStream = new PdfStream(SerializeDoc(form.DatasetsNode));
                    dStream.FlateCompress(writer.CompressionLevel);
                    ar[d] = writer.AddToBody(dStream).IndirectReference;
                    af.Put(PdfName.Xfa, new PdfArray(ar));
                    return;
                }
            }
            reader.KillXref(af.Get(PdfName.Xfa));
            PdfStream str = new PdfStream(SerializeDoc(form._domDocument));
            str.FlateCompress(writer.CompressionLevel);
            PdfIndirectReference refe = writer.AddToBody(str).IndirectReference;
            af.Put(PdfName.Xfa, refe);
        }

        /// <summary>
        /// Finds the complete SOM name contained in the datasets section from a
        /// possibly partial name.
        /// </summary>
        /// <param name="name">the complete or partial name</param>
        /// <returns>the complete name or  null  if not found</returns>
        public string FindDatasetsName(string name)
        {
            if (DatasetsSom.Name2Node.ContainsKey(name))
                return name;
            return DatasetsSom.InverseSearchGlobal(Xml2Som.SplitParts(name));
        }

        /// <summary>
        /// Finds the  Node  contained in the datasets section from a
        /// possibly partial name.
        /// </summary>
        /// <param name="name">the complete or partial name</param>
        /// <returns>the  Node  or  null  if not found</returns>
        public XmlNode FindDatasetsNode(string name)
        {
            if (name == null)
                return null;
            name = FindDatasetsName(name);
            if (name == null)
                return null;
            return (XmlNode)DatasetsSom.Name2Node[name];
        }

        /// <summary>
        /// Finds the complete field name contained in the "classic" forms from a partial
        /// name.
        /// </summary>
        /// <param name="name">the complete or partial name</param>
        /// <param name="af">the fields</param>
        /// <returns>the complete name or  null  if not found</returns>
        public string FindFieldName(string name, AcroFields af)
        {
            Hashtable items = af.Fields;
            if (items.ContainsKey(name))
                return name;
            if (AcroFieldsSom == null)
            {
                if (items.Count == 0 && XfaPresent)
                    AcroFieldsSom = new AcroFieldsSearch(DatasetsSom.Name2Node.Keys);
                else
                    AcroFieldsSom = new AcroFieldsSearch(items.Keys);
            }
            if (AcroFieldsSom.AcroShort2LongName.ContainsKey(name))
                return (string)AcroFieldsSom.AcroShort2LongName[name];
            return AcroFieldsSom.InverseSearchGlobal(Xml2Som.SplitParts(name));
        }

        /// <summary>
        /// Sets the text of this node. All the child's node are deleted and a new
        /// child text node is created.
        /// </summary>
        /// <param name="n">the  Node  to add the text to</param>
        /// <param name="text">the text to add</param>
        public void SetNodeText(XmlNode n, string text)
        {
            if (n == null)
                return;
            XmlNode nc = null;
            while ((nc = n.FirstChild) != null)
            {
                n.RemoveChild(nc);
            }
            n.Attributes.RemoveNamedItem("dataNode", XFA_DATA_SCHEMA);
            n.AppendChild(_domDocument.CreateTextNode(text));
            Changed = true;
        }

        /// <summary>
        /// Sets the XFA key from the instance data. The old XFA is erased.
        /// @throws java.io.IOException on error
        /// </summary>
        /// <param name="writer">the writer</param>
        public void SetXfa(PdfWriter writer)
        {
            SetXfa(this, Reader, writer);
        }

        private static string getNodeText(XmlNode n, string name)
        {
            XmlNode n2 = n.FirstChild;
            while (n2 != null)
            {
                if (n2.NodeType == XmlNodeType.Element)
                {
                    name = getNodeText(n2, name);
                }
                else if (n2.NodeType == XmlNodeType.Text)
                {
                    name += n2.Value;
                }
                n2 = n2.NextSibling;
            }
            return name;
        }

        /// <summary>
        /// Extracts the nodes from the domDocument.
        /// @since    2.1.5
        /// </summary>
        private void extractNodes()
        {
            XmlNode n = _domDocument.FirstChild;
            while (n.NodeType != XmlNodeType.Element || n.ChildNodes.Count == 0)
                n = n.NextSibling;
            n = n.FirstChild;
            while (n != null)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    string s = n.LocalName;
                    if (s.Equals("template"))
                    {
                        _templateNode = n;
                        TemplateSom = new Xml2SomTemplate(n);
                    }
                    else if (s.Equals("datasets"))
                    {
                        DatasetsNode = n;
                        DatasetsSom = new Xml2SomDatasets(n.FirstChild);
                    }
                }
                n = n.NextSibling;
            }
        }
        /// <summary>
        /// A class to process "classic" fields.
        /// </summary>
        public class AcroFieldsSearch : Xml2Som
        {
            /// <summary>
            /// Creates a new instance from a Collection with the full names.
            /// </summary>
            /// <param name="items">the Collection</param>
            public AcroFieldsSearch(ICollection items)
            {
                inverseSearch = new Hashtable();
                AcroShort2LongName = new Hashtable();
                foreach (string itemName in items)
                {
                    string itemShort = GetShortName(itemName);
                    AcroShort2LongName[itemShort] = itemName;
                    InverseSearchAdd(inverseSearch, SplitParts(itemShort), itemName);
                }
            }

            /// <summary>
            /// Gets the mapping from short names to long names. A long
            /// name may contain the #subform name part.
            /// </summary>
            /// <returns>the mapping from short names to long names</returns>
            public Hashtable AcroShort2LongName { get; set; }
        }

        /// <summary>
        /// A structure to store each part of a SOM name and link it to the next part
        /// beginning from the lower hierarchie.
        /// </summary>
        public class InverseStore
        {
            protected internal ArrayList Follow = new ArrayList();
            protected internal ArrayList Part = new ArrayList();
            /// <summary>
            /// Gets the full name by traversing the hiearchie using only the
            /// index 0.
            /// </summary>
            /// <returns>the full name</returns>
            public string DefaultName
            {
                get
                {
                    InverseStore store = this;
                    while (true)
                    {
                        object obj = store.Follow[0];
                        if (obj is string)
                            return (string)obj;
                        store = (InverseStore)obj;
                    }
                }
            }

            /// <summary>
            /// Search the current node for a similar name. A similar name starts
            /// with the same name but has a differnt index. For example, "detail[3]"
            /// is similar to "detail[9]". The main use is to discard names that
            /// correspond to out of bounds records.
            /// </summary>
            /// <param name="name">the name to search</param>
            /// <returns> true  if a similitude was found</returns>
            public bool IsSimilar(string name)
            {
                int idx = name.IndexOf('[');
                name = name.Substring(0, idx + 1);
                foreach (string n in Part)
                {
                    if (n.StartsWith(name))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Another stack implementation. The main use is to facilitate
        /// the porting to other languages.
        /// </summary>
        public class Stack2 : ArrayList
        {
            /// <summary>
            /// Tests if this stack is empty.
            /// </summary>
            /// <returns> true  if and only if this stack contains no items;  false  otherwise</returns>
            public bool Empty()
            {
                return Count == 0;
            }

            /// <summary>
            /// Looks at the object at the top of this stack without removing it from the stack.
            /// </summary>
            /// <returns>the object at the top of this stack</returns>
            public object Peek()
            {
                if (Count == 0)
                    throw new InvalidOperationException();
                return this[Count - 1];
            }

            /// <summary>
            /// Removes the object at the top of this stack and returns that object as the value of this function.
            /// </summary>
            /// <returns>the object at the top of this stack</returns>
            public object Pop()
            {
                if (Count == 0)
                    throw new InvalidOperationException();
                object ret = this[Count - 1];
                RemoveAt(Count - 1);
                return ret;
            }

            /// <summary>
            /// Pushes an item onto the top of this stack.
            /// </summary>
            /// <param name="item">the item to be pushed onto this stack</param>
            /// <returns>the  item  argument</returns>
            public object Push(object item)
            {
                Add(item);
                return item;
            }
        }

        /// <summary>
        /// A class for some basic SOM processing.
        /// </summary>
        public class Xml2Som
        {
            /// <summary>
            /// A temporary store for the repetition count.
            /// </summary>
            protected int Anform;

            /// <summary>
            /// The data to do a search from the bottom hierarchie.
            /// </summary>
            protected Hashtable inverseSearch;

            /// <summary>
            /// The mapping of full names to nodes.
            /// </summary>
            protected Hashtable name2Node;

            /// <summary>
            /// The order the names appear in the XML, depth first.
            /// </summary>
            protected ArrayList order;
            /// <summary>
            /// A stack to be used when parsing.
            /// </summary>
            protected Stack2 Stack;
            /// <summary>
            /// Gets the data to do a search from the bottom hierarchie.
            /// </summary>
            /// <returns>the data to do a search from the bottom hierarchie</returns>
            public Hashtable InverseSearch
            {
                get
                {
                    return inverseSearch;
                }
                set
                {
                    inverseSearch = value;
                }
            }

            /// <summary>
            /// Gets the mapping of full names to nodes.
            /// </summary>
            /// <returns>the mapping of full names to nodes</returns>
            public Hashtable Name2Node
            {
                get
                {
                    return name2Node;
                }
                set
                {
                    name2Node = value;
                }
            }

            /// <summary>
            /// Gets the order the names appear in the XML, depth first.
            /// </summary>
            /// <returns>the order the names appear in the XML, depth first</returns>
            public ArrayList Order
            {
                get
                {
                    return order;
                }
                set
                {
                    order = value;
                }
            }

            /// <summary>
            /// Escapes a SOM string fragment replacing "." with "\.".
            /// </summary>
            /// <param name="s">the unescaped string</param>
            /// <returns>the escaped string</returns>
            public static string EscapeSom(string s)
            {
                int idx = s.IndexOf('.');
                if (idx < 0)
                    return s;
                StringBuilder sb = new StringBuilder();
                int last = 0;
                while (idx >= 0)
                {
                    sb.Append(s.Substring(last, idx - last));
                    sb.Append('\\');
                    last = idx;
                    idx = s.IndexOf('.', idx + 1);
                }
                sb.Append(s.Substring(last));
                return sb.ToString();
            }

            /// <summary>
            /// Gets the name with the  #subform  removed.
            /// </summary>
            /// <param name="s">the long name</param>
            /// <returns>the short name</returns>
            public static string GetShortName(string s)
            {
                int idx = s.IndexOf(".#subform[", StringComparison.OrdinalIgnoreCase);
                if (idx < 0)
                    return s;
                int last = 0;
                StringBuilder sb = new StringBuilder();
                while (idx >= 0)
                {
                    sb.Append(s.Substring(last, idx - last));
                    idx = s.IndexOf("]", idx + 10, StringComparison.Ordinal);
                    if (idx < 0)
                        return sb.ToString();
                    last = idx + 1;
                    idx = s.IndexOf(".#subform[", last, StringComparison.OrdinalIgnoreCase);
                }
                sb.Append(s.Substring(last));
                return sb.ToString();
            }

            /// <summary>
            /// Adds a SOM name to the search node chain.
            /// </summary>
            /// <param name="inverseSearch">the start point</param>
            /// <param name="stack">the stack with the separeted SOM parts</param>
            /// <param name="unstack">the full name</param>
            public static void InverseSearchAdd(Hashtable inverseSearch, Stack2 stack, string unstack)
            {
                string last = (string)stack.Peek();
                InverseStore store = (InverseStore)inverseSearch[last];
                if (store == null)
                {
                    store = new InverseStore();
                    inverseSearch[last] = store;
                }
                for (int k = stack.Count - 2; k >= 0; --k)
                {
                    last = (string)stack[k];
                    InverseStore store2;
                    int idx = store.Part.IndexOf(last);
                    if (idx < 0)
                    {
                        store.Part.Add(last);
                        store2 = new InverseStore();
                        store.Follow.Add(store2);
                    }
                    else
                        store2 = (InverseStore)store.Follow[idx];
                    store = store2;
                }
                store.Part.Add("");
                store.Follow.Add(unstack);
            }

            /// <summary>
            /// Splits a SOM name in the individual parts.
            /// </summary>
            /// <param name="name">the full SOM name</param>
            /// <returns>the split name</returns>
            public static Stack2 SplitParts(string name)
            {
                while (name.StartsWith("."))
                    name = name.Substring(1);
                Stack2 parts = new Stack2();
                int last = 0;
                int pos = 0;
                string part;
                while (true)
                {
                    pos = last;
                    while (true)
                    {
                        pos = name.IndexOf('.', pos);
                        if (pos < 0)
                            break;
                        if (name[pos - 1] == '\\')
                            ++pos;
                        else
                            break;
                    }
                    if (pos < 0)
                        break;
                    part = name.Substring(last, pos - last);
                    if (!part.EndsWith("]"))
                        part += "[0]";
                    parts.Add(part);
                    last = pos + 1;
                }
                part = name.Substring(last);
                if (!part.EndsWith("]"))
                    part += "[0]";
                parts.Add(part);
                return parts;
            }

            /// <summary>
            /// Unescapes a SOM string fragment replacing "\." with ".".
            /// </summary>
            /// <param name="s">the escaped string</param>
            /// <returns>the unescaped string</returns>
            public static string UnescapeSom(string s)
            {
                int idx = s.IndexOf('\\');
                if (idx < 0)
                    return s;
                StringBuilder sb = new StringBuilder();
                int last = 0;
                while (idx >= 0)
                {
                    sb.Append(s.Substring(last, idx - last));
                    last = idx + 1;
                    idx = s.IndexOf('\\', idx + 1);
                }
                sb.Append(s.Substring(last));
                return sb.ToString();
            }

            /// <summary>
            /// Adds a SOM name to the search node chain.
            /// </summary>
            /// <param name="unstack">the SOM name</param>
            public void InverseSearchAdd(string unstack)
            {
                InverseSearchAdd(inverseSearch, Stack, unstack);
            }

            /// <summary>
            /// Searchs the SOM hiearchie from the bottom.
            /// </summary>
            /// <param name="parts">the SOM parts</param>
            /// <returns>the full name or  null  if not found</returns>
            public string InverseSearchGlobal(ArrayList parts)
            {
                if (parts.Count == 0)
                    return null;
                InverseStore store = (InverseStore)inverseSearch[parts[parts.Count - 1]];
                if (store == null)
                    return null;
                for (int k = parts.Count - 2; k >= 0; --k)
                {
                    string part = (string)parts[k];
                    int idx = store.Part.IndexOf(part);
                    if (idx < 0)
                    {
                        if (store.IsSimilar(part))
                            return null;
                        return store.DefaultName;
                    }
                    store = (InverseStore)store.Follow[idx];
                }
                return store.DefaultName;
            }

            /// <summary>
            /// Outputs the stack as the sequence of elements separated
            /// by '.'.
            /// </summary>
            /// <returns>the stack as the sequence of elements separated by '.'</returns>
            protected string PrintStack()
            {
                if (Stack.Empty())
                    return "";
                StringBuilder s = new StringBuilder();
                foreach (string part in Stack)
                    s.Append('.').Append(part);
                return s.ToString(1, s.Length - 1);
            }
        }

        /// <summary>
        /// Processes the datasets section in the XFA form.
        /// </summary>
        public class Xml2SomDatasets : Xml2Som
        {
            /// <summary>
            /// Creates a new instance from the datasets node. This expects
            /// not the datasets but the data node that comes below.
            /// </summary>
            /// <param name="n">the datasets node</param>
            public Xml2SomDatasets(XmlNode n)
            {
                order = new ArrayList();
                name2Node = new Hashtable();
                Stack = new Stack2();
                Anform = 0;
                inverseSearch = new Hashtable();
                processDatasetsInternal(n);
            }

            /// <summary>
            /// Inserts a new  Node  that will match the short name.
            /// </summary>
            /// <param name="n">the datasets top  Node </param>
            /// <param name="shortName">the short name</param>
            /// <returns>the new  Node  of the inserted name</returns>
            public XmlNode InsertNode(XmlNode n, string shortName)
            {
                Stack2 stack = SplitParts(shortName);
                XmlDocument doc = n.OwnerDocument;
                XmlNode n2 = null;
                n = n.FirstChild;
                for (int k = 0; k < stack.Count; ++k)
                {
                    string part = (string)stack[k];
                    int idx = part.LastIndexOf('[');
                    string name = part.Substring(0, idx);
                    idx = int.Parse(part.Substring(idx + 1, part.Length - idx - 2));
                    int found = -1;
                    for (n2 = n.FirstChild; n2 != null; n2 = n2.NextSibling)
                    {
                        if (n2.NodeType == XmlNodeType.Element)
                        {
                            string s = EscapeSom(n2.LocalName);
                            if (s.Equals(name))
                            {
                                ++found;
                                if (found == idx)
                                    break;
                            }
                        }
                    }
                    for (; found < idx; ++found)
                    {
                        n2 = doc.CreateElement(name);
                        n2 = n.AppendChild(n2);
                        XmlNode attr = doc.CreateNode(XmlNodeType.Attribute, "dataNode", XFA_DATA_SCHEMA);
                        attr.Value = "dataGroup";
                        n2.Attributes.SetNamedItem(attr);
                    }
                    n = n2;
                }
                InverseSearchAdd(inverseSearch, stack, shortName);
                name2Node[shortName] = n2;
                order.Add(shortName);
                return n2;
            }

            private static bool hasChildren(XmlNode n)
            {
                XmlNode dataNodeN = n.Attributes.GetNamedItem("dataNode", XFA_DATA_SCHEMA);
                if (dataNodeN != null)
                {
                    string dataNode = dataNodeN.Value;
                    if ("dataGroup".Equals(dataNode))
                        return true;
                    else if ("dataValue".Equals(dataNode))
                        return false;
                }
                if (!n.HasChildNodes)
                    return false;
                XmlNode n2 = n.FirstChild;
                while (n2 != null)
                {
                    if (n2.NodeType == XmlNodeType.Element)
                    {
                        return true;
                    }
                    n2 = n2.NextSibling;
                }
                return false;
            }

            private void processDatasetsInternal(XmlNode n)
            {
                Hashtable ss = new Hashtable();
                XmlNode n2 = n.FirstChild;
                while (n2 != null)
                {
                    if (n2.NodeType == XmlNodeType.Element)
                    {
                        string s = EscapeSom(n2.LocalName);
                        int i;
                        if (ss[s] == null)
                            i = 0;
                        else
                            i = (int)ss[s] + 1;
                        ss[s] = i;
                        if (hasChildren(n2))
                        {
                            Stack.Push(s + "[" + i + "]");
                            processDatasetsInternal(n2);
                            Stack.Pop();
                        }
                        else
                        {
                            Stack.Push(s + "[" + i + "]");
                            string unstack = PrintStack();
                            order.Add(unstack);
                            InverseSearchAdd(unstack);
                            name2Node[unstack] = n2;
                            Stack.Pop();
                        }
                    }
                    n2 = n2.NextSibling;
                }
            }
        }
        /// <summary>
        /// Processes the template section in the XFA form.
        /// </summary>
        public class Xml2SomTemplate : Xml2Som
        {
            private int _templateLevel;

            /// <summary>
            /// Creates a new instance from the datasets node.
            /// </summary>
            /// <param name="n">the template node</param>
            public Xml2SomTemplate(XmlNode n)
            {
                order = new ArrayList();
                name2Node = new Hashtable();
                Stack = new Stack2();
                Anform = 0;
                _templateLevel = 0;
                inverseSearch = new Hashtable();
                processTemplate(n, null);
            }

            /// <summary>
            ///  true  if it's a dynamic form;  false
            /// if it's a static form.
            /// if it's a static form
            /// </summary>
            /// <returns> true  if it's a dynamic form;  false </returns>
            public bool DynamicForm { get; set; }

            /// <summary>
            /// Gets the field type as described in the  template  section of the XFA.
            /// </summary>
            /// <param name="s">the exact template name</param>
            /// <returns>the field type or  null  if not found</returns>
            public string GetFieldType(string s)
            {
                XmlNode n = (XmlNode)name2Node[s];
                if (n == null)
                    return null;
                if (n.LocalName.Equals("exclGroup"))
                    return "exclGroup";
                XmlNode ui = n.FirstChild;
                while (ui != null)
                {
                    if (ui.NodeType == XmlNodeType.Element && ui.LocalName.Equals("ui"))
                    {
                        break;
                    }
                    ui = ui.NextSibling;
                }
                if (ui == null)
                    return null;
                XmlNode type = ui.FirstChild;
                while (type != null)
                {
                    if (type.NodeType == XmlNodeType.Element && !(type.LocalName.Equals("extras") && type.LocalName.Equals("picture")))
                    {
                        return type.LocalName;
                    }
                    type = type.NextSibling;
                }
                return null;
            }

            private void processTemplate(XmlNode n, Hashtable ff)
            {
                if (ff == null)
                    ff = new Hashtable();
                Hashtable ss = new Hashtable();
                XmlNode n2 = n.FirstChild;
                while (n2 != null)
                {
                    if (n2.NodeType == XmlNodeType.Element)
                    {
                        string s = n2.LocalName;
                        if (s.Equals("subform"))
                        {
                            XmlNode name = n2.Attributes.GetNamedItem("name");
                            string nn = "#subform";
                            bool annon = true;
                            if (name != null)
                            {
                                nn = EscapeSom(name.Value);
                                annon = false;
                            }
                            int i;
                            if (annon)
                            {
                                i = Anform;
                                ++Anform;
                            }
                            else
                            {
                                if (ss[nn] == null)
                                    i = 0;
                                else
                                    i = (int)ss[nn] + 1;
                                ss[nn] = i;
                            }
                            Stack.Push(nn + "[" + i + "]");
                            ++_templateLevel;
                            if (annon)
                                processTemplate(n2, ff);
                            else
                                processTemplate(n2, null);
                            --_templateLevel;
                            Stack.Pop();
                        }
                        else if (s.Equals("field") || s.Equals("exclGroup"))
                        {
                            XmlNode name = n2.Attributes.GetNamedItem("name");
                            if (name != null)
                            {
                                string nn = EscapeSom(name.Value);
                                int i;
                                if (ff[nn] == null)
                                    i = 0;
                                else
                                    i = (int)ff[nn] + 1;
                                ff[nn] = i;
                                Stack.Push(nn + "[" + i + "]");
                                string unstack = PrintStack();
                                order.Add(unstack);
                                InverseSearchAdd(unstack);
                                name2Node[unstack] = n2;
                                Stack.Pop();
                            }
                        }
                        else if (!DynamicForm && _templateLevel > 0 && s.Equals("occur"))
                        {
                            int initial = 1;
                            int min = 1;
                            int max = 1;
                            XmlNode a = n2.Attributes.GetNamedItem("initial");
                            if (a != null)
                                try { initial = int.Parse(a.Value.Trim()); } catch { };
                            a = n2.Attributes.GetNamedItem("min");
                            if (a != null)
                                try { min = int.Parse(a.Value.Trim()); } catch { };
                            a = n2.Attributes.GetNamedItem("max");
                            if (a != null)
                                try { max = int.Parse(a.Value.Trim()); } catch { };
                            if (initial != min || min != max)
                                DynamicForm = true;
                        }
                    }
                    n2 = n2.NextSibling;
                }
            }
        }
    }
}