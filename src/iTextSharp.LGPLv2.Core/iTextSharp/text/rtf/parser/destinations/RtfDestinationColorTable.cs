using System.util;
using iTextSharp.text.rtf.parser.ctrlwords;
using iTextSharp.text.rtf.parser.enumerations;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationColorTable  handles data destined for the color table destination
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfDestinationColorTable : RtfDestination
{
    /// <summary>
    ///     The blue component of the current color being parsed.
    /// </summary>
    private int _blue = -1;

    /// <summary>
    ///     Color map object for conversions
    /// </summary>
    private INullValueDictionary<string, BaseColor> _colorMap;

    /// <summary>
    ///     The number of the current color being parsed.
    /// </summary>
    private int _colorNr;

    /// <summary>
    ///     Specifies the shade when specifying a theme color.
    ///     RTF control word cshade
    ///     0 - 255: 0 = full Shade(black), 255 = no shade.
    ///     Default value: 255
    ///     If shade is specified and is less than 255, ctint must equal 255.
    ///     cshade/ctint are mutually exclusive
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#ctint
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#themeColor
    /// </summary>
    private int _cshade = 255;

    /// <summary>
    ///     Color themes - Introduced Word 2007
    /// </summary>
    /// <summary>
    ///     Specifies the tint when specifying a theme color.
    ///     RTF control word ctint
    ///     0 - 255: 0 = full Tint(white), 255 = no tint.
    ///     Default value: 255
    ///     If tint is specified and is less than 255, cshade must equal 255.
    ///     ctint/cshade are mutually exclusive
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#cshade
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#themeColor
    /// </summary>
    private int _ctint = 255;

    /// <summary>
    ///     The green component of the current color being parsed.
    /// </summary>
    private int _green = -1;

    /// <summary>
    ///     The RtfImportHeader to add color mappings to.
    /// </summary>
    private RtfImportMgr _importHeader;

    /// <summary>
    ///     The red component of the current color being parsed.
    /// </summary>
    private int _red = -1;

    /// <summary>
    ///     Specifies the use of a theme color.
    ///     @see com.lowagie.text.rtf.parser.enumerations.RtfColorThemes
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#ctint
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#cshade
    /// </summary>
    private int _themeColor = RtfColorThemes.THEME_UNDEFINED;

    /// <summary>
    ///     Constructor.
    /// </summary>
    public RtfDestinationColorTable() : base(null)
    {
        _colorMap = new NullValueDictionary<string, BaseColor>();
        _colorNr = 0;
    }

    /// <summary>
    ///     Constructs a new RtfColorTableParser.
    /// </summary>
    public RtfDestinationColorTable(RtfParser parser) : base(parser)
    {
        if (parser == null)
        {
            throw new ArgumentNullException(nameof(parser));
        }

        _colorMap = new NullValueDictionary<string, BaseColor>();
        _colorNr = 0;
        _importHeader = parser.GetImportManager();
        SetToDefaults();
    }

    public override bool CloseDestination() => true;

    /// <summary>
    ///     conversion functions
    /// </summary>
    /// <summary>
    ///     Get the  Color  object that is mapped to the key.
    ///     *@return  Color  object from the map. null if key does not exist.
    /// </summary>
    /// <param name="key">The map number.</param>
    public BaseColor GetColor(string key) => _colorMap[key];

    public override bool HandleCharacter(int ch)
    {
        // color elements end with a semicolon (;)
        if ((char)ch == ';')
        {
            processColor();
        }

        return true;
    }

    public override bool HandleCloseGroup()
    {
        processColor();
        return true;
    }

    public override bool HandleControlWord(RtfCtrlWordData ctrlWordData)
    {
        if (ctrlWordData == null)
        {
            throw new ArgumentNullException(nameof(ctrlWordData));
        }

        if (ctrlWordData.CtrlWord.Equals("blue", StringComparison.Ordinal))
        {
            setBlue(ctrlWordData.IntValue());
        }

        if (ctrlWordData.CtrlWord.Equals("red", StringComparison.Ordinal))
        {
            setRed(ctrlWordData.IntValue());
        }

        if (ctrlWordData.CtrlWord.Equals("green", StringComparison.Ordinal))
        {
            setGreen(ctrlWordData.IntValue());
        }

        if (ctrlWordData.CtrlWord.Equals("cshade", StringComparison.Ordinal))
        {
            setShade(ctrlWordData.IntValue());
        }

        if (ctrlWordData.CtrlWord.Equals("ctint", StringComparison.Ordinal))
        {
            setTint(ctrlWordData.IntValue());
        }

        //if(ctrlWordData.ctrlWord.Equals("cmaindarkone")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("cmainlightone")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("cmaindarktwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("cmainlighttwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("caccentone")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("caccenttwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("caccentthree")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("caccentfour")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("caccentfive")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("caccentsix")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("chyperlink")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("cfollowedhyperlink")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("cbackgroundone")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("ctextone")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("cbacgroundtwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
        //if(ctrlWordData.ctrlWord.Equals("ctexttwo")) this.SetThemeColor(ctrlWordData.ctrlWord);
        return true;
    }

    public override bool HandleOpenGroup() => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
    /// </summary>
    public override bool HandleOpeningSubGroup() => true;

    public override void SetParser(RtfParser parser)
    {
        RtfParser = parser ?? throw new ArgumentNullException(nameof(parser));
        _colorMap = new NullValueDictionary<string, BaseColor>();
        _colorNr = 0;
        _importHeader = parser.GetImportManager();
        SetToDefaults();
    }

    /// <summary>
    ///     Set default values.
    /// </summary>
    public override void SetToDefaults()
    {
        _red = -1;
        _green = -1;
        _blue = -1;
        _ctint = 255;
        _cshade = 255;
        _themeColor = RtfColorThemes.THEME_UNDEFINED;
        // do not reset colorNr
    }

    /// <summary>
    ///     Processes the color triplet parsed from the document.
    ///     Add it to the import mapping so colors can be mapped when encountered
    ///     in the RTF import or conversion.
    /// </summary>
    private void processColor()
    {
        if (_red != -1 && _green != -1 && _blue != -1)
        {
            if (RtfParser.IsImport())
            {
                _importHeader.ImportColor(_colorNr.ToString(CultureInfo.InvariantCulture),
                                          new BaseColor(_red, _green, _blue));
            }

            if (RtfParser.IsConvert())
            {
                _colorMap[_colorNr.ToString(CultureInfo.InvariantCulture)] = new BaseColor(_red, _green, _blue);
            }
        }

        SetToDefaults();
        _colorNr++;
    }

    /// <summary>
    ///     Set the blue color value.
    /// </summary>
    /// <param name="value">Value to set blue to.</param>
    private void setBlue(int value)
    {
        if (value >= 0 && value <= 255)
        {
            _blue = value;
        }
    }

    /// <summary>
    ///     Set the green color value.
    /// </summary>
    /// <param name="value">Value to set green to.</param>
    private void setGreen(int value)
    {
        if (value >= 0 && value <= 255)
        {
            _green = value;
        }
    }

    /// <summary>
    ///     Set the red color to value.
    /// </summary>
    /// <param name="value">Value to set red to.</param>
    private void setRed(int value)
    {
        if (value >= 0 && value <= 255)
        {
            _red = value;
        }
    }

    /// <summary>
    ///     Set the shade value
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#cshade
    /// </summary>
    /// <param name="value">Value to set the shade to</param>
    private void setShade(int value)
    {
        if (value >= 0 && value <= 255)
        {
            _cshade = value;
            if (value >= 0 && value < 255)
            {
                _ctint = 255;
            }
        }
    }

    /// <summary>
    ///     Set the theme color value.
    ///     @see com.lowagie.text.rtf.parser.enumerations.RtfColorThemes
    /// </summary>
    /// <param name="value">Value to set the theme color to</param>
    private void setThemeColor(int value)
    {
        if (value >= RtfColorThemes.THEME_UNDEFINED && value <= RtfColorThemes.THEME_MAX)
        {
            _themeColor = value;
        }
        else
        {
            _themeColor = RtfColorThemes.THEME_UNDEFINED;
        }
    }

    /// <summary>
    ///     Set the tint value
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestinationColorTable#ctint
    /// </summary>
    /// <param name="value">Value to set the tint to</param>
    private void setTint(int value)
    {
        if (value >= 0 && value <= 255)
        {
            _ctint = value;
            if (value >= 0 && value < 255)
            {
                _cshade = 255;
            }
        }
    }
}