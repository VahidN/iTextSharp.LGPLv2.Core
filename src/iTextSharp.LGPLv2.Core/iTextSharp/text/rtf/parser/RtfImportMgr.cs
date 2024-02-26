using System.util;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.list;
using iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.parser;

/// <summary>
///     The RtfImportHeader stores the docment header information from
///     an RTF document that is being imported. Currently font and
///     color settings are stored. The RtfImportHeader maintains a mapping
///     from font and color numbers from the imported RTF document to
///     the RTF document that is the target of the import. This guarantees
///     that the merged document has the correct font and color settings.
///     It also handles other list based items that need mapping, for example
///     stylesheets and lists.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Howard Shank (hgshank@yahoo.com)
/// </summary>
public class RtfImportMgr
{
    /// <summary>
    ///     The Hashtable storing the color number mapings.
    /// </summary>
    private readonly NullValueDictionary<string, string> _importColorMapping;

    /// <summary>
    ///     TODO: Add list, stylesheet, info, etc. mappings
    /// </summary>
    /// <summary>
    ///     The Hashtable storing the font number mappings.
    /// </summary>
    private readonly NullValueDictionary<string, string> _importFontMapping;

    /// <summary>
    ///     The Hashtable storing the List number mapings.
    /// </summary>
    private readonly NullValueDictionary<string, string> _importListMapping;

    /// <summary>
    ///     The Hashtable storing the Stylesheet List number mapings.
    /// </summary>
    private readonly NullValueDictionary<string, string> _importStylesheetListMapping;

    /// <summary>
    ///     The RtfDocument to get font and color numbers from.
    /// </summary>
    private readonly RtfDocument _rtfDoc;

    /// <summary>
    ///     The Document.
    ///     Used for conversions, but not imports.
    /// </summary>
    private Document _doc;

    /// <summary>
    ///     Constructs a new RtfImportHeader.
    /// </summary>
    /// <param name="rtfDoc">The RtfDocument to get font and color numbers from.</param>
    /// <param name="doc"></param>
    public RtfImportMgr(RtfDocument rtfDoc, Document doc)
    {
        _rtfDoc = rtfDoc;
        _doc = doc;
        _importFontMapping = new NullValueDictionary<string, string>();
        _importColorMapping = new NullValueDictionary<string, string>();
        _importStylesheetListMapping = new NullValueDictionary<string, string>();
        _importListMapping = new NullValueDictionary<string, string>();
    }

    /// <summary>
    ///     Imports a color value. The color number for the color defined
    ///     by its red, green and blue values is determined and then the
    ///     resulting mapping is added.
    /// </summary>
    /// <param name="colorNr">The original color number.</param>
    /// <param name="color">The color to import.</param>
    public void ImportColor(string colorNr, BaseColor color)
    {
        var rtfColor = new RtfColor(_rtfDoc, color);
        _importColorMapping[colorNr] = rtfColor.GetColorNumber().ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Imports a font. The font name is looked up in the RtfDocumentHeader and
    ///     then the mapping from original font number to actual font number is added.
    /// </summary>
    /// <param name="fontNr">The original font number.</param>
    /// <param name="fontName">The font name to look up.</param>
    public bool ImportFont(string fontNr, string fontName)
    {
        var rtfFont = new RtfFont(fontName);
        rtfFont.SetRtfDocument(_rtfDoc);

        _importFontMapping[fontNr] =
            _rtfDoc.GetDocumentHeader().GetFontNumber(rtfFont).ToString(CultureInfo.InvariantCulture);

        return true;
    }

    /// <summary>
    ///     Imports a font. The font name is looked up in the RtfDocumentHeader and
    ///     then the mapping from original font number to actual font number is added.
    /// </summary>
    /// <param name="fontNr">The original font number.</param>
    /// <param name="fontName">The font name to look up.</param>
    /// <param name="charset">The characterset to use for the font.</param>
    public bool ImportFont(string fontNr, string fontName, int charset)
    {
        var rtfFont = new RtfFont(fontName);

        if (charset >= 0)
        {
            rtfFont.SetCharset(charset);
        }

        rtfFont.SetRtfDocument(_rtfDoc);

        _importFontMapping[fontNr] =
            _rtfDoc.GetDocumentHeader().GetFontNumber(rtfFont).ToString(CultureInfo.InvariantCulture);

        return true;
    }

    /// <summary>
    ///     Imports a font. The font name is looked up in the RtfDocumentHeader and
    ///     then the mapping from original font number to actual font number is added.
    /// </summary>
    /// <param name="fontNr">The original font number.</param>
    /// <param name="fontName">The font name to look up.</param>
    /// <param name="fontFamily"></param>
    /// <param name="charset">The characterset to use for the font.</param>
    public bool ImportFont(string fontNr, string fontName, string fontFamily, int charset)
    {
        var rtfFont = new RtfFont(fontName);

        if (charset >= 0)
        {
            rtfFont.SetCharset(charset);
        }

        if (!string.IsNullOrEmpty(fontFamily))
        {
            rtfFont.SetFamily(fontFamily);
        }

        rtfFont.SetRtfDocument(_rtfDoc);

        _importFontMapping[fontNr] =
            _rtfDoc.GetDocumentHeader().GetFontNumber(rtfFont).ToString(CultureInfo.InvariantCulture);

        return true;
    }

    /// <summary>
    ///     Imports a List value. The List number for the List defined
    ///     is determined and then the resulting mapping is added.
    /// </summary>
    public void ImportList(string origListNr, string newListNr)
        => _importListMapping[origListNr] = newListNr;

    /// <summary>
    ///     Imports a stylesheet list value. The stylesheet number for the stylesheet defined
    ///     is determined and then the resulting mapping is added.
    /// </summary>
    public bool ImportStylesheetList(string listNr, List listIn)
    {
        var rtfList = new RtfList(_rtfDoc, listIn);
        rtfList.SetRtfDocument(_rtfDoc);

        return true;
    }

    /// <summary>
    ///     Performs the mapping from the original font number to the actual font
    ///     number used in the RTF document. If the color number was not
    ///     seen during import (thus no mapping) then 0 is returned, guaranteeing
    ///     that the color number is always valid.
    /// </summary>
    /// <param name="colorNr">The color number to map.</param>
    /// <returns>The mapped color number</returns>
    public string MapColorNr(string colorNr)
    {
        if (_importColorMapping.TryGetValue(colorNr, out var nr))
        {
            return nr;
        }

        return "0";
    }

    /// <summary>
    ///     Performs the mapping from the original font number to the actual
    ///     font number in the resulting RTF document. If the font number was not
    ///     seen during import (thus no mapping) then 0 is returned, guaranteeing
    ///     that the font number is always valid.
    /// </summary>
    /// <param name="fontNr">The font number to map.</param>
    /// <returns>The mapped font number.</returns>
    public string MapFontNr(string fontNr)
    {
        if (_importFontMapping.TryGetValue(fontNr, out var nr))
        {
            return nr;
        }

        return "0";
    }

    /// <summary>
    ///     Performs the mapping from the original list number to the actual
    ///     list number in the resulting RTF document. If the list number was not
    ///     seen during import (thus no mapping) then 0 is returned, guaranteeing
    ///     that the list number is always valid.
    /// </summary>
    public string MapListNr(string listNr)
    {
        if (_importListMapping.TryGetValue(listNr, out var nr))
        {
            return nr;
        }

        return null;
    }

    /// <summary>
    ///     Performs the mapping from the original stylesheet number to the actual
    ///     stylesheet number in the resulting RTF document. If the stylesheet number was not
    ///     seen during import (thus no mapping) then 0 is returned, guaranteeing
    ///     that the stylesheet number is always valid.
    /// </summary>
    public string MapStylesheetListNr(string listNr)
    {
        if (_importStylesheetListMapping.TryGetValue(listNr, out var nr))
        {
            return nr;
        }

        return "0";
    }
}