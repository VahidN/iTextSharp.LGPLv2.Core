using System.util;
using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.style;

/// <summary>
///     The RtfStylesheetList stores the RtfParagraphStyles that are used in the document.
///     @version $Revision: 1.5 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfStylesheetList : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     The Hashtable containing the RtfParagraphStyles.
    /// </summary>
    private readonly NullValueDictionary<string, RtfParagraphStyle> _styleMap;

    /// <summary>
    ///     Whether the default settings have been loaded.
    /// </summary>
    private bool _defaultsLoaded;

    /// <summary>
    ///     Constructs a new RtfStylesheetList for the RtfDocument.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfStylesheetList belongs to.</param>
    public RtfStylesheetList(RtfDocument doc) : base(doc)
        => _styleMap = new NullValueDictionary<string, RtfParagraphStyle>();

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the definition of the stylesheet list.
    /// </summary>
    public virtual void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(t = DocWriter.GetIsoBytes("{"), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\stylesheet"), 0, t.Length);
        outp.Write(t = Delimiter, 0, t.Length);
        Document.OutputDebugLinebreak(outp);

        foreach (var rps in _styleMap.Values)
        {
            rps.WriteDefinition(outp);
        }

        outp.Write(t = DocWriter.GetIsoBytes("}"), 0, t.Length);
        Document.OutputDebugLinebreak(outp);
    }

    /// <summary>
    ///     Gets the RtfParagraphStyle with the given name. Makes sure that the defaults
    ///     have been loaded.
    /// </summary>
    /// <param name="styleName">The name of the RtfParagraphStyle to get.</param>
    /// <returns>The RtfParagraphStyle with the given name or null.</returns>
    public RtfParagraphStyle GetRtfParagraphStyle(string styleName)
    {
        if (!_defaultsLoaded)
        {
            registerDefaultStyles();
        }

        if (_styleMap.TryGetValue(styleName, out var style))
        {
            return style;
        }

        return null;
    }

    /// <summary>
    ///     Register a RtfParagraphStyle with this RtfStylesheetList.
    /// </summary>
    /// <param name="rtfParagraphStyle">The RtfParagraphStyle to add.</param>
    public void RegisterParagraphStyle(RtfParagraphStyle rtfParagraphStyle)
    {
        var tempStyle = new RtfParagraphStyle(Document, rtfParagraphStyle);
        tempStyle.HandleInheritance();
        tempStyle.SetStyleNumber(_styleMap.Count);
        _styleMap[tempStyle.GetStyleName()] = tempStyle;
    }

    /// <summary>
    ///     Registers all default styles. If styles with the given name have already been registered,
    ///     then they are NOT overwritten.
    /// </summary>
    private void registerDefaultStyles()
    {
        _defaultsLoaded = true;

        if (!_styleMap.ContainsKey(RtfParagraphStyle.StyleNormal.GetStyleName()))
        {
            RegisterParagraphStyle(RtfParagraphStyle.StyleNormal);
        }

        if (!_styleMap.ContainsKey(RtfParagraphStyle.StyleHeading1.GetStyleName()))
        {
            RegisterParagraphStyle(RtfParagraphStyle.StyleHeading1);
        }

        if (!_styleMap.ContainsKey(RtfParagraphStyle.StyleHeading2.GetStyleName()))
        {
            RegisterParagraphStyle(RtfParagraphStyle.StyleHeading2);
        }

        if (!_styleMap.ContainsKey(RtfParagraphStyle.StyleHeading3.GetStyleName()))
        {
            RegisterParagraphStyle(RtfParagraphStyle.StyleHeading3);
        }
    }
}