using System.IO;
using System.Xml;
using System.Text;

namespace iTextSharp.text.xml.xmp
{
    /// <summary>
    /// Reads an XMP stream into an org.w3c.dom.Document objects.
    /// Allows you to replace the contents of a specific tag.
    /// @since 2.1.3
    /// </summary>
    public class XmpReader
    {

        private readonly XmlDocument _domDocument;

        /// <summary>
        /// Constructs an XMP reader
        /// @throws ExceptionConverter
        /// @throws IOException
        /// @throws SAXException
        /// </summary>
        public XmpReader(byte[] bytes)
        {
            MemoryStream bout = new MemoryStream();
            bout.Write(bytes, 0, bytes.Length);
            bout.Seek(0, SeekOrigin.Begin);
            var xtr = XmlReader.Create(bout);
            _domDocument = new XmlDocument();
            _domDocument.PreserveWhitespace = true;
            _domDocument.Load(xtr);
        }

        /// <summary>
        /// Adds a tag.
        /// @since	2.1.6
        /// </summary>
        /// <returns>if the content was successfully added</returns>
        public bool Add(string parent, string namespaceUri, string localName, string value)
        {
            XmlNodeList nodes = _domDocument.GetElementsByTagName(parent);
            if (nodes.Count == 0)
                return false;
            XmlNode pNode;
            XmlNode node;
            for (int i = 0; i < nodes.Count; i++)
            {
                pNode = nodes[i];
                XmlAttributeCollection attrs = pNode.Attributes;
                for (int j = 0; j < attrs.Count; j++)
                {
                    node = attrs[j];
                    if (namespaceUri.Equals(node.Value))
                    {
                        node = _domDocument.CreateElement(localName);
                        node.AppendChild(_domDocument.CreateTextNode(value));
                        pNode.AppendChild(node);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Replaces the content of a tag.
        /// @since	2.1.6 the return type has changed from void to boolean
        /// </summary>
        /// <returns>if the content was successfully replaced</returns>
        public bool Replace(string namespaceUri, string localName, string value)
        {
            XmlNodeList nodes = _domDocument.GetElementsByTagName(localName, namespaceUri);
            XmlNode node;
            if (nodes.Count == 0)
                return false;
            for (int i = 0; i < nodes.Count; i++)
            {
                node = nodes[i];
                SetNodeText(_domDocument, node, value);
            }
            return true;
        }
        /// <summary>
        /// Writes the document to a byte array.
        /// </summary>
        public byte[] SerializeDoc()
        {
            using (var fout = new MemoryStream())
            {
                byte[] b = new UTF8Encoding(false).GetBytes(XmpWriter.XPACKET_PI_BEGIN);
                fout.Write(b, 0, b.Length);
                fout.Flush();
                XmlNodeList xmpmeta = _domDocument.GetElementsByTagName("x:xmpmeta");

                var xwSettings = new XmlWriterSettings
                {
                    Encoding = new UTF8Encoding(false),
                    IndentChars = "  ",
                    Indent = true,
                    NewLineChars = "\n",
                    OmitXmlDeclaration = true
                };
                var xw = XmlWriter.Create(fout, xwSettings);
                var xmlNode = xmpmeta[0];
                xmlNode.WriteContentTo(xw);
                xw.Flush();
                b = new UTF8Encoding(false).GetBytes(XmpWriter.XPACKET_PI_END_W);
                fout.Write(b, 0, b.Length);
                return fout.ToArray();
            }
        }

        /// <summary>
        /// Sets the text of this node. All the child's node are deleted and a new
        /// child text node is created.
        /// </summary>
        /// <param name="domDocument">the  Document  that contains the node</param>
        /// <param name="n">the  Node  to add the text to</param>
        /// <param name="value">the text to add</param>
        public bool SetNodeText(XmlDocument domDocument, XmlNode n, string value)
        {
            if (n == null)
                return false;
            XmlNode nc = null;
            while ((nc = n.FirstChild) != null)
            {
                n.RemoveChild(nc);
            }
            n.AppendChild(domDocument.CreateTextNode(value));
            return true;
        }
    }
}