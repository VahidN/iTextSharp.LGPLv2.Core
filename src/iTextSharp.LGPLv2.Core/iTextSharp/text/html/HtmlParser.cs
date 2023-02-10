using System.Xml;
using iTextSharp.text.xml;

namespace iTextSharp.text.html;

/// <summary>
///     This class can be used to parse an XML file.
/// </summary>
public class HtmlParser : XmlParser
{
    /// <summary>
    ///     Parses a given file.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="xDoc"></param>
    public new static void Parse(IDocListener document, XmlDocument xDoc)
    {
        var p = new HtmlParser();
        p.Go(document, xDoc);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="file"></param>
    public new static void Parse(IDocListener document, string file)
    {
        var p = new HtmlParser();
        p.Go(document, file);
    }

    /// <summary>
    ///     Parses a given XmlTextReader.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="reader"></param>
    public new static void Parse(IDocListener document, XmlReader reader)
    {
        var p = new HtmlParser();
        p.Go(document, reader);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    public new static void Parse(IDocListener document, XmlDocument xDoc, XmlDocument xTagmap)
    {
        var p = new HtmlParser();
        p.Go(document, xDoc, xTagmap);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="file"></param>
    /// <param name="tagmap"></param>
    public new static void Parse(IDocListener document, string file, string tagmap)
    {
        var p = new HtmlParser();
        p.Go(document, file, tagmap);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="file"></param>
    /// <param name="tagmap"></param>
    public new static void Parse(IDocListener document, string file, TagMap tagmap)
    {
        var p = new HtmlParser();
        p.Go(document, file, tagmap);
    }

    /// <summary>
    ///     Parses a given XmlTextReader.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="reader"></param>
    /// <param name="tagmap"></param>
    public new static void Parse(IDocListener document, XmlReader reader, string tagmap)
    {
        var p = new HtmlParser();
        p.Go(document, reader, tagmap);
    }

    /// <summary>
    ///     Parses a given XmlTextReader.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="reader"></param>
    /// <param name="tagmap"></param>
    public new static void Parse(IDocListener document, XmlReader reader, TagMap tagmap)
    {
        var p = new HtmlParser();
        p.Go(document, reader, tagmap);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    public override void Go(IDocListener document, XmlDocument xDoc)
    {
        Parser = new TextmyHtmlHandler(document);
        Parser.Parse(xDoc);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="file"></param>
    public override void Go(IDocListener document, string file)
    {
        Parser = new TextmyHtmlHandler(document);
        Parser.Parse(file);
    }

    /// <summary>
    ///     Parses a given XmlTextReader.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="reader"></param>
    public override void Go(IDocListener document, XmlReader reader)
    {
        Parser = new TextmyHtmlHandler(document);
        Parser.Parse(reader);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    public override void Go(IDocListener document, XmlDocument xDoc, XmlDocument xTagmap)
    {
        Parser = new TextmyHtmlHandler(document, new TagMap(xTagmap));
        Parser.Parse(xDoc);
    }

    /// <summary>
    ///     Parses a given XmlTextReader.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="reader"></param>
    /// <param name="tagmap"></param>
    public override void Go(IDocListener document, XmlReader reader, string tagmap)
    {
        Parser = new TextmyHtmlHandler(document, new TagMap(tagmap));
        Parser.Parse(reader);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="file"></param>
    /// <param name="tagmap"></param>
    public override void Go(IDocListener document, string file, string tagmap)
    {
        Parser = new TextmyHtmlHandler(document, new TagMap(tagmap));
        Parser.Parse(file);
    }

    /// <summary>
    ///     Parses a given file.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="file"></param>
    /// <param name="tagmap"></param>
    public override void Go(IDocListener document, string file, TagMap tagmap)
    {
        Parser = new TextmyHtmlHandler(document, tagmap);
        Parser.Parse(file);
    }

    /// <summary>
    ///     Parses a given XmlTextReader.
    /// </summary>
    /// <param name="document"></param>
    /// <param name="reader"></param>
    /// <param name="tagmap"></param>
    public override void Go(IDocListener document, XmlReader reader, TagMap tagmap)
    {
        Parser = new TextmyHtmlHandler(document, tagmap);
        Parser.Parse(reader);
    }
}