using System.util;

namespace iTextSharp.text.rtf.parser;

/// <summary>
///     The RtfImportMappings make it possible to define font
///     and color mappings when using the RtfWriter2.importRtfFragment
///     method. This is necessary, because a RTF fragment does not
///     contain font or color information, just references to the
///     font and color tables.<br /><br />
///     The font mappings are fontNr -&gt; fontName and the color
///     mappigns are colorNr -&gt; Color.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Howard Shank (hgshank@yahoo.com)
/// </summary>
public class RtfImportMappings
{
    /// <summary>
    ///     The colorNr to Color mappings.
    /// </summary>
    private readonly INullValueDictionary<string, BaseColor> _colorMappings;

    /// <summary>
    ///     The fontNr to fontName mappings.
    /// </summary>
    private readonly INullValueDictionary<string, string> _fontMappings;

    /// <summary>
    ///     The listNr to List mappings.
    /// </summary>
    private readonly INullValueDictionary<string, string> _listMappings;

    /// <summary>
    ///     The sytlesheetListNr to Stylesheet mappings.
    /// </summary>
    private readonly INullValueDictionary<string, string> _stylesheetListMappings;

    /// <summary>
    ///     Constructs a new RtfImportMappings initialising the mappings.
    /// </summary>
    public RtfImportMappings()
    {
        _fontMappings = new NullValueDictionary<string, string>();
        _colorMappings = new NullValueDictionary<string, BaseColor>();
        _listMappings = new NullValueDictionary<string, string>();
        _stylesheetListMappings = new NullValueDictionary<string, string>();
    }

    /// <summary>
    ///     Add a color to the list of mappings.
    /// </summary>
    /// <param name="colorNr">The color number.</param>
    /// <param name="color">The Color.</param>
    public void AddColor(string colorNr, BaseColor color)
    {
        _colorMappings[colorNr] = color;
    }

    /// <summary>
    ///     Add a font to the list of mappings.
    /// </summary>
    /// <param name="fontNr">The font number.</param>
    /// <param name="fontName">The font name.</param>
    public void AddFont(string fontNr, string fontName)
    {
        _fontMappings[fontNr] = fontName;
    }

    /// <summary>
    ///     Add a List to the list of mappings.
    /// </summary>
    /// <param name="listNr">The List number.</param>
    /// <param name="list">The List.</param>
    public void AddList(string listNr, string list)
    {
        _listMappings[listNr] = list;
    }

    /// <summary>
    ///     Add a Stylesheet List to the list of mappings.
    /// </summary>
    /// <param name="stylesheetListNr">The Stylesheet List number.</param>
    /// <param name="list">The StylesheetList.</param>
    public void AddStylesheetList(string stylesheetListNr, string list)
    {
        _stylesheetListMappings[stylesheetListNr] = list;
    }

    /// <summary>
    ///     Gets the list of color mappings. String to Color.
    /// </summary>
    /// <returns>The color mappings.</returns>
    public INullValueDictionary<string, BaseColor> GetColorMappings() => _colorMappings;

    /// <summary>
    ///     Gets the list of font mappings. String to String.
    /// </summary>
    /// <returns>The font mappings.</returns>
    public INullValueDictionary<string, string> GetFontMappings() => _fontMappings;

    /// <summary>
    ///     Gets the list of List mappings.
    /// </summary>
    /// <returns>The List mappings.</returns>
    public INullValueDictionary<string, string> GetListMappings() => _listMappings;

    /// <summary>
    ///     Gets the list of Stylesheet mappings. .
    /// </summary>
    /// <returns>The Stylesheet List mappings.</returns>
    public INullValueDictionary<string, string> GetStylesheetListMappings() => _stylesheetListMappings;
}