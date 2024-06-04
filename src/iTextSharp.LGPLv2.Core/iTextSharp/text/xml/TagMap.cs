using System.util;
using System.Xml;

namespace iTextSharp.text.xml;

/// <summary>
///     The  TagMap -class maps several XHTML-tags to iText-objects.
/// </summary>
public class TagMap : NullValueDictionary<string, XmlPeer>
{
    public TagMap()
    {
    }

    /// <summary>
    ///     Constructs a Tagmap object
    /// </summary>
    /// <param name="tagfile">the file of tags to parse</param>
    public TagMap(string tagfile)
    {
        Init(tagfile);
    }

    /// <summary>
    ///     Constructs a Tagmap object
    /// </summary>
    public TagMap(XmlDocument xTagfile)
    {
        Init(xTagfile);
    }

    /// <summary>
    ///     Parses the xml document
    /// </summary>
    protected void Init(XmlDocument xTagfile)
    {
        var a = new AttributeHandler(this);
        a.Parse(xTagfile);
    }

    /// <summary>
    ///     Parses the xml document
    /// </summary>
    /// <param name="tagfile"></param>
    protected void Init(string tagfile)
    {
        var a = new AttributeHandler(this);
        a.Parse(tagfile);
    }

    private class AttributeHandler : ParserBase
    {
        /// <summary> This is an attribute </summary>
        public const string ALIAS = "alias";

        /// <summary> This is a tag </summary>
        public const string ATTRIBUTE = "attribute";

        /// <summary> This is an attribute </summary>
        public const string CONTENT = "content";

        /// <summary> This is an attribute </summary>
        public const string NAME = "name";

        /// <summary> This is a tag </summary>
        public const string TAG = "tag";

        /// <summary> This is an attribute </summary>
        public const string VALUE = "value";

        /// <summary> This is the tagmap using the AttributeHandler </summary>
        private readonly INullValueDictionary<string, XmlPeer> _tagMap;

        /// <summary> This is the current peer. </summary>
        private XmlPeer _currentPeer;

        /// <summary>
        ///     Constructs a new SAXiTextHandler that will translate all the events
        ///     triggered by the parser to actions on the  Document -object.
        /// </summary>
        /// <param name="tagMap">A Hashtable containing XmlPeer-objects</param>
        public AttributeHandler(INullValueDictionary<string, XmlPeer> tagMap) => _tagMap = tagMap;

        /// <summary>
        ///     This method gets called when characters are encountered.
        /// </summary>
        /// <param name="content">an array of characters</param>
        /// <param name="start">the start position in the array</param>
        /// <param name="length">the number of characters to read from the array</param>
        public override void Characters(string content, int start, int length)
        {
            // do nothing
        }

        /// <summary>
        ///     This method gets called when an end tag is encountered.
        /// </summary>
        /// <param name="tag">the name of the tag that ends</param>
        /// <param name="lname"></param>
        /// <param name="name"></param>
        public override void EndElement(string tag, string lname, string name)
        {
            if (TAG.Equals(lname, StringComparison.Ordinal))
            {
                _tagMap.Add(_currentPeer.Alias, _currentPeer);
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
            // do nothing
        }

        /// <summary>
        ///     This method gets called when a start tag is encountered.
        /// </summary>
        /// <param name="tag">the name of the tag that is encountered</param>
        /// <param name="lname"></param>
        /// <param name="n"></param>
        /// <param name="attrs">the list of attributes</param>
        public override void StartElement(string tag, string lname, string n,
                                          INullValueDictionary<string, string> attrs)
        {
            var name = attrs[NAME];
            var alias = attrs[ALIAS];
            var value = attrs[VALUE];
            if (name != null)
            {
                if (TAG.Equals(lname, StringComparison.Ordinal))
                {
                    _currentPeer = new XmlPeer(name, alias);
                }
                else if (ATTRIBUTE.Equals(lname, StringComparison.Ordinal))
                {
                    if (alias != null)
                    {
                        _currentPeer.AddAlias(name, alias);
                    }

                    if (value != null)
                    {
                        _currentPeer.AddValue(name, value);
                    }
                }
            }

            value = attrs[CONTENT];
            if (value != null)
            {
                _currentPeer.Content = value;
            }
        }
    }
}