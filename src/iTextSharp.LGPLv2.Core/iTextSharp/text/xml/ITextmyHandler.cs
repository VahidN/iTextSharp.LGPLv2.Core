using System.util;

namespace iTextSharp.text.xml;

/// <summary>
///     The  iTextmyHandler -class maps several XHTML-tags to iText-objects.
/// </summary>
public class TextmyHandler : TextHandler
{
    /// <summary>
    ///     Constructs a new iTextHandler that will translate all the events
    ///     triggered by the parser to actions on the  Document -object.
    /// </summary>
    /// <param name="document">this is the document on which events must be triggered</param>
    /// <param name="myTags">a map of tags</param>
    public TextmyHandler(IDocListener document, TagMap myTags) : base(document, myTags)
    {
    }

    /// <summary>
    ///     This method gets called when a start tag is encountered.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="lname"></param>
    /// <param name="name">the name of the tag that is encountered</param>
    /// <param name="attrs">the list of attributes</param>
    public override void StartElement(string uri, string lname, string name, INullValueDictionary<string, string> attrs)
    {
        if (MyTags.TryGetValue(name, out var tag))
        {
            HandleStartingTags(tag.Tag, tag.GetAttributes(attrs));
        }
        else
        {
            var attributes = new Properties();
            if (attrs != null)
            {
                foreach (var key in attrs.Keys)
                {
                    attributes.Add(key, attrs[key]);
                }
            }

            HandleStartingTags(name, attributes);
        }
    }

    /// <summary>
    ///     This method gets called when an end tag is encountered.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="lname"></param>
    /// <param name="name">the name of the tag that ends</param>
    public override void EndElement(string uri, string lname, string name)
    {
        if (MyTags.TryGetValue(name, out var tag))
        {
            HandleEndingTags(tag.Tag);
        }
        else
        {
            HandleEndingTags(name);
        }
    }
}