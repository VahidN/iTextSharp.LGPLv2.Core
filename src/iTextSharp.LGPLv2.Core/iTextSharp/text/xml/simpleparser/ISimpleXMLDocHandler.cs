using System.util;

namespace iTextSharp.text.xml.simpleparser;

/// <summary>
///     The handler for the events fired by  SimpleXMLParser .
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public interface ISimpleXmlDocHandler
{
    /// <summary>
    ///     Called when a start tag is found.
    /// </summary>
    /// <param name="tag">the tag name</param>
    /// <param name="h">the tag's attributes</param>
    void StartElement(string tag, INullValueDictionary<string, string> h);

    /// <summary>
    ///     Called when an end tag is found.
    /// </summary>
    /// <param name="tag">the tag name</param>
    void EndElement(string tag);

    /// <summary>
    ///     Called when the document starts to be parsed.
    /// </summary>
    void StartDocument();

    /// <summary>
    ///     Called after the document is parsed.
    /// </summary>
    void EndDocument();

    /// <summary>
    ///     Called when a text element is found.
    /// </summary>
    /// <param name="str">the text element, probably a fragment.</param>
    void Text(string str);
}