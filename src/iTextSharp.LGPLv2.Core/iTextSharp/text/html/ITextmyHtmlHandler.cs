using System;
using System.Collections;
using System.Globalization;
using System.util;
using iTextSharp.text.xml;
using iTextSharp.text.pdf;

namespace iTextSharp.text.html
{

    /// <summary>
    /// The  Tags -class maps several XHTML-tags to iText-objects.
    /// </summary>
    public class TextmyHtmlHandler : TextHandler
    {

        /// <summary>
        /// These are the properties of the body section.
        /// </summary>
        private readonly Properties _bodyAttributes = new Properties();

        /// <summary>
        /// This is the status of the table border.
        /// </summary>
        private bool _tableBorder;

        /// <summary>
        /// Constructs a new SAXiTextHandler that will translate all the events
        /// triggered by the parser to actions on the  Document -object.
        /// </summary>
        /// <param name="document">this is the document on which events must be triggered</param>

        public TextmyHtmlHandler(IDocListener document) : base(document, new HtmlTagMap())
        {
        }

        public TextmyHtmlHandler(IDocListener document, BaseFont bf) : base(document, new HtmlTagMap(), bf)
        {
        }

        /// <summary>
        /// Constructs a new SAXiTextHandler that will translate all the events
        /// triggered by the parser to actions on the  Document -object.
        /// </summary>
        /// <param name="document">this is the document on which events must be triggered</param>
        /// <param name="htmlTags">a tagmap translating HTML tags to iText tags</param>

        public TextmyHtmlHandler(IDocListener document, Hashtable htmlTags) : base(document, htmlTags)
        {
        }

        /// <summary>
        /// This method gets called when a start tag is encountered.
        /// </summary>
        /// <param name="uri">the Uniform Resource Identifier</param>
        /// <param name="lname">the local name (without prefix), or the empty string if Namespace processing is not being performed.</param>
        /// <param name="name">the name of the tag that is encountered</param>
        public override void EndElement(string uri, string lname, string name)
        {
            //System.err.Println("End: " + name);
            name = name.ToLower(CultureInfo.InvariantCulture);
            if (ElementTags.PARAGRAPH.Equals(name))
            {
                Document.Add((IElement)Stack.Pop());
                return;
            }
            if (HtmlTagMap.IsHead(name))
            {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsTitle(name))
            {
                if (CurrentChunk != null)
                {
                    _bodyAttributes.Add(ElementTags.TITLE, CurrentChunk.Content);
                }
                return;
            }
            if (HtmlTagMap.IsMeta(name))
            {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsLink(name))
            {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsBody(name))
            {
                // we do nothing
                return;
            }
            if (MyTags.ContainsKey(name))
            {
                XmlPeer peer = (XmlPeer)MyTags[name];
                if (ElementTags.TABLE.Equals(peer.Tag))
                {
                    _tableBorder = false;
                }
                HandleEndingTags(peer.Tag);
                return;
            }
            // super.handleEndingTags is replaced with handleEndingTags
            // suggestion by Ken Auer
            HandleEndingTags(name);
        }

        public override void StartElement(string uri, string lname, string name, Hashtable attrs)
        {
            //System.err.Println("Start: " + name);

            // super.handleStartingTags is replaced with handleStartingTags
            // suggestion by Vu Ngoc Tan/Hop
            name = name.ToLower(CultureInfo.InvariantCulture);
            if (HtmlTagMap.IsHtml(name))
            {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsHead(name))
            {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsTitle(name))
            {
                // we do nothing
                return;
            }
            if (HtmlTagMap.IsMeta(name))
            {
                // we look if we can change the body attributes
                string meta = null;
                string content = null;
                if (attrs != null)
                {
                    foreach (string attribute in attrs.Keys)
                    {
                        if (Util.EqualsIgnoreCase(attribute, HtmlTags.CONTENT))
                            content = (string)attrs[attribute];
                        else if (Util.EqualsIgnoreCase(attribute, HtmlTags.NAME))
                            meta = (string)attrs[attribute];
                    }
                }
                if (meta != null && content != null)
                {
                    _bodyAttributes.Add(meta, content);
                }
                return;
            }
            if (HtmlTagMap.IsLink(name))
            {
                // we do nothing for the moment, in a later version we could extract the style sheet
                return;
            }
            if (HtmlTagMap.IsBody(name))
            {
                // maybe we could extract some info about the document: color, margins,...
                // but that's for a later version...
                XmlPeer peer = new XmlPeer(ElementTags.ITEXT, name);
                peer.AddAlias(ElementTags.TOP, HtmlTags.TOPMARGIN);
                peer.AddAlias(ElementTags.BOTTOM, HtmlTags.BOTTOMMARGIN);
                peer.AddAlias(ElementTags.RIGHT, HtmlTags.RIGHTMARGIN);
                peer.AddAlias(ElementTags.LEFT, HtmlTags.LEFTMARGIN);
                _bodyAttributes.AddAll(peer.GetAttributes(attrs));
                HandleStartingTags(peer.Tag, _bodyAttributes);
                return;
            }
            if (MyTags.ContainsKey(name))
            {
                XmlPeer peer = (XmlPeer)MyTags[name];
                if (ElementTags.TABLE.Equals(peer.Tag) || ElementTags.CELL.Equals(peer.Tag))
                {
                    Properties p = peer.GetAttributes(attrs);
                    string value;
                    if (ElementTags.TABLE.Equals(peer.Tag) && (value = p[ElementTags.BORDERWIDTH]) != null)
                    {
                        if (float.Parse(value, NumberFormatInfo.InvariantInfo) > 0)
                        {
                            _tableBorder = true;
                        }
                    }
                    if (_tableBorder)
                    {
                        p.Add(ElementTags.LEFT, "true");
                        p.Add(ElementTags.RIGHT, "true");
                        p.Add(ElementTags.TOP, "true");
                        p.Add(ElementTags.BOTTOM, "true");
                    }
                    HandleStartingTags(peer.Tag, p);
                    return;
                }
                HandleStartingTags(peer.Tag, peer.GetAttributes(attrs));
                return;
            }
            Properties attributes = new Properties();
            if (attrs != null)
            {
                foreach (string attribute in attrs.Keys)
                {
                    attributes.Add(attribute.ToLower(CultureInfo.InvariantCulture), ((string)attrs[attribute]).ToLower(CultureInfo.InvariantCulture));
                }
            }
            HandleStartingTags(name, attributes);
        }
    }
}