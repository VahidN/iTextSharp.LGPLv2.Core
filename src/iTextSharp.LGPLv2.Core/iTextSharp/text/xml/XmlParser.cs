using System.Xml;
using System.Collections;

namespace iTextSharp.text.xml
{

    /// <summary>
    /// This class can be used to parse an XML file.
    /// </summary>
    public class XmlParser
    {

        /// <summary> This is the instance of the parser. </summary>
        protected TextHandler Parser;

        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="xDoc"></param>
        public static void Parse(IDocListener document, XmlDocument xDoc)
        {
            XmlParser p = new XmlParser();
            p.Go(document, xDoc);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        public static void Parse(IDocListener document, string file)
        {
            XmlParser p = new XmlParser();
            p.Go(document, file);
        }

        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        public static void Parse(IDocListener document, XmlReader reader)
        {
            XmlParser p = new XmlParser();
            p.Go(document, reader);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        public static void Parse(IDocListener document, XmlDocument xDoc, XmlDocument xTagmap)
        {
            XmlParser p = new XmlParser();
            p.Go(document, xDoc, xTagmap);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public static void Parse(IDocListener document, string file, string tagmap)
        {
            XmlParser p = new XmlParser();
            p.Go(document, file, tagmap);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public static void Parse(IDocListener document, string file, Hashtable tagmap)
        {
            XmlParser p = new XmlParser();
            p.Go(document, file, tagmap);
        }

        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="tagmap"></param>
        public static void Parse(IDocListener document, XmlReader reader, string tagmap)
        {
            XmlParser p = new XmlParser();
            p.Go(document, reader, tagmap);
        }

        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="tagmap"></param>
        public static void Parse(IDocListener document, XmlReader reader, Hashtable tagmap)
        {
            XmlParser p = new XmlParser();
            p.Go(document, reader, tagmap);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        public virtual void Go(IDocListener document, XmlDocument xDoc)
        {
            Parser = new TextHandler(document);
            Parser.Parse(xDoc);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        public virtual void Go(IDocListener document, string file)
        {
            Parser = new TextHandler(document);
            Parser.Parse(file);
        }

        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        public virtual void Go(IDocListener document, XmlReader reader)
        {
            Parser = new TextHandler(document);
            Parser.Parse(reader);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        public virtual void Go(IDocListener document, XmlDocument xDoc, XmlDocument xTagmap)
        {
            Parser = new TextmyHandler(document, new TagMap(xTagmap));
            Parser.Parse(xDoc);
        }

        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="tagmap"></param>
        public virtual void Go(IDocListener document, XmlReader reader, string tagmap)
        {
            Parser = new TextmyHandler(document, new TagMap(tagmap));
            Parser.Parse(reader);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public virtual void Go(IDocListener document, string file, string tagmap)
        {
            Parser = new TextmyHandler(document, new TagMap(tagmap));
            Parser.Parse(file);
        }

        /// <summary>
        /// Parses a given file.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="file"></param>
        /// <param name="tagmap"></param>
        public virtual void Go(IDocListener document, string file, Hashtable tagmap)
        {
            Parser = new TextmyHandler(document, tagmap);
            Parser.Parse(file);
        }

        /// <summary>
        /// Parses a given XmlTextReader.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="reader"></param>
        /// <param name="tagmap"></param>
        public virtual void Go(IDocListener document, XmlReader reader, Hashtable tagmap)
        {
            Parser = new TextmyHandler(document, tagmap);
            Parser.Parse(reader);
        }
    }
}