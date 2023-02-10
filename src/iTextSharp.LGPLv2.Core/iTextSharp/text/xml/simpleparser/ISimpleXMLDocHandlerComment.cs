namespace iTextSharp.text.xml.simpleparser;

/// <summary>
///     The handler for the events fired by  SimpleXMLParser .
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public interface ISimpleXmlDocHandlerComment
{
    /// <summary>
    ///     Called when a comment is found.
    /// </summary>
    /// <param name="text">the comment text</param>
    void Comment(string text);
}