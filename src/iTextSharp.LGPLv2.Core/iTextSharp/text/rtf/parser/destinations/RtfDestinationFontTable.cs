using System.util;
using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationFontTable  handles data destined for the font table destination
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public sealed class RtfDestinationFontTable : RtfDestination
{
    private const string CharsetDefault = "0";

    /// <summary>
    ///     \falt
    /// </summary>
    private const int SettingAlternate = 1;

    /// <summary>
    ///     \fontemb
    /// </summary>
    private const int SettingFontEmbed = 4;

    /// <summary>
    ///     \ffile
    /// </summary>
    private const int SettingFontFile = 5;

    /// <summary>
    ///     \fname
    /// </summary>
    private const int SettingFontname = 2;

    /// <summary>
    ///     state values
    /// </summary>
    /// <summary>
    ///     Normal
    /// </summary>
    private const int SettingNormal = 0;

    /// <summary>
    ///     \panose
    /// </summary>
    private const int SettingPanose = 3;

    /// <summary>
    ///     The \charset value
    /// </summary>
    private string _charset = "";

    /// <summary>
    ///     The \cpg value
    /// </summary>
    private string _cpg = "";

    /// <summary>
    ///     The \falt alternate font if primary font is not available.
    /// </summary>
    private string _falt = "";

    /// <summary>
    ///     The \falt alternate font if primary font is not available.
    /// </summary>
    /// <summary>
    ///     private String fontemb = "";
    /// </summary>
    /// <summary>
    ///     The \falt alternate font if primary font is not available.
    /// </summary>
    /// <summary>
    ///     private String fontType = "";
    /// </summary>
    /// <summary>
    ///     The \falt alternate font if primary font is not available.
    /// </summary>
    /// <summary>
    ///     private String fontFile = "";
    /// </summary>
    /// <summary>
    ///     The \falt alternate font if primary font is not available.
    /// </summary>
    /// <summary>
    ///     private String fontFileCpg = "";
    /// </summary>
    /// <summary>
    ///     The \fbias value
    /// </summary>
    private int _fbias;

    /// <summary>
    ///     The family of the font being parsed.
    /// </summary>
    private string _fontFamily = "";

    /// <summary>
    ///     Convert font mapping to  FontFactory  font objects.
    /// </summary>
    private INullValueDictionary<string, Font> _fontMap;

    /// <summary>
    ///     The \*\fname
    /// </summary>
    /// <summary>
    ///     private String nontaggedname = "";
    /// </summary>
    /// <summary>
    ///     The name of the font being parsed.
    /// </summary>
    private string _fontName = "";

    /// <summary>
    ///     The number of the font being parsed.
    /// </summary>
    private string _fontNr = "";

    /// <summary>
    ///     The \fprq
    /// </summary>
    private int _fprq;

    /// <summary>
    ///     The RtfImportHeader to add font mappings to.
    /// </summary>
    private RtfImportMgr _importHeader;

    /// <summary>
    ///     The \*\panose font matching value if primary font is not available.
    /// </summary>
    private string _panose = "";

    /// <summary>
    ///     state flag to handle different parsing of a font element
    /// </summary>
    private int _state;

    /// <summary>
    ///     The theme (Office 2007)
    /// </summary>
    private string _themeFont = "";

    /// <summary>
    ///     The \fnil, \fttruetype value
    /// </summary>
    private string _trueType = "";

    /// <summary>
    ///     Constructor
    /// </summary>
    public RtfDestinationFontTable() : base(null)
    {
    }

    /// <summary>
    ///     Constructs a new RtfFontTableParser.
    ///     @since 2.0.8
    /// </summary>
    public RtfDestinationFontTable(RtfParser parser) : base(parser)
    {
        init(true);
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
    ///     @since 2.0.8
    /// </summary>
    public override bool CloseDestination() => true;

    /// <summary>
    ///     Get a  Font  object from the font map object
    ///     @since 2.0.8
    /// </summary>
    /// <param name="key">The font number to get</param>
    /// <returns>The mapped  Font  object.</returns>
    public Font GetFont(string key) => _fontMap[key];

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(char[])
    ///     @since 2.0.8
    /// </summary>
    public override bool HandleCharacter(int ch)
    {
        switch (_state)
        {
            case SettingNormal:
                _fontName += (char)ch;
                break;
            case SettingAlternate:
                _falt += (char)ch;
                break;
            case SettingPanose:
                _panose += (char)ch;
                break;
            case SettingFontEmbed:
                break;
            case SettingFontFile:
                break;
            case SettingFontname:
                break;
        }

        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
    ///     @since 2.0.8
    /// </summary>
    public override bool HandleCloseGroup()
    {
        if (_state == SettingNormal)
        {
            processFont();
        }

        _state = SettingNormal;
        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see
    ///     com.lowagie.text.rtf.parser.destinations.RtfDestination#handleControlWord(com.lowagie.text.rtf.parser.ctrlwords.RtfCtrlWordData)
    ///     @since 2.0.8
    /// </summary>
    public override bool HandleControlWord(RtfCtrlWordData ctrlWordData)
    {
        var result = true;
        // just let fonttbl fall through and set last ctrl word object.

        if (ctrlWordData.CtrlWord.Equals("f", StringComparison.Ordinal))
        {
            SetFontNumber(ctrlWordData.Param);
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fcharset", StringComparison.Ordinal))
        {
            SetCharset(ctrlWordData.Param);
            result = true;
        }

        // font families
        if (ctrlWordData.CtrlWord.Equals("fnil", StringComparison.Ordinal))
        {
            SetFontFamily("roman");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("froman", StringComparison.Ordinal))
        {
            SetFontFamily("roman");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fswiss", StringComparison.Ordinal))
        {
            SetFontFamily("swiss");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fmodern", StringComparison.Ordinal))
        {
            SetFontFamily("modern");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fscript", StringComparison.Ordinal))
        {
            SetFontFamily("script");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fdecor", StringComparison.Ordinal))
        {
            SetFontFamily("decor");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("ftech", StringComparison.Ordinal))
        {
            SetFontFamily("tech");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fbidi", StringComparison.Ordinal))
        {
            SetFontFamily("bidi");
            result = true;
        }

        // pitch
        if (ctrlWordData.CtrlWord.Equals("fprq", StringComparison.Ordinal))
        {
            SetPitch(ctrlWordData.Param);
            result = true;
        }

        // bias
        if (ctrlWordData.CtrlWord.Equals("fbias", StringComparison.Ordinal))
        {
            SetBias(ctrlWordData.Param);
            result = true;
        }

        // theme font information
        if (ctrlWordData.CtrlWord.Equals("flomajor", StringComparison.Ordinal))
        {
            SetThemeFont("flomajor");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fhimajor", StringComparison.Ordinal))
        {
            SetThemeFont("fhimajor");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fdbmajor", StringComparison.Ordinal))
        {
            SetThemeFont("fdbmajor");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fbimajor", StringComparison.Ordinal))
        {
            SetThemeFont("fbimajor");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("flominor", StringComparison.Ordinal))
        {
            SetThemeFont("flominor");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fhiminor", StringComparison.Ordinal))
        {
            SetThemeFont("fhiminor");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fdbminor", StringComparison.Ordinal))
        {
            SetThemeFont("fdbminor");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fbiminor", StringComparison.Ordinal))
        {
            SetThemeFont("fbiminor");
            result = true;
        }

        // panose
        if (ctrlWordData.CtrlWord.Equals("panose", StringComparison.Ordinal))
        {
            _state = SettingPanose;
            result = true;
        }

        // \*\fname
        // <font name> #PCDATA
        if (ctrlWordData.CtrlWord.Equals("fname", StringComparison.Ordinal))
        {
            _state = SettingFontname;
            result = true;
        }

        // \*\falt
        if (ctrlWordData.CtrlWord.Equals("falt", StringComparison.Ordinal))
        {
            _state = SettingAlternate;
            result = true;
        }

        // \*\fontemb
        if (ctrlWordData.CtrlWord.Equals("fontemb", StringComparison.Ordinal))
        {
            _state = SettingFontEmbed;
            result = true;
        }

        // font type
        if (ctrlWordData.CtrlWord.Equals("ftnil", StringComparison.Ordinal))
        {
            SetTrueType("ftnil");
            result = true;
        }

        if (ctrlWordData.CtrlWord.Equals("fttruetype", StringComparison.Ordinal))
        {
            SetTrueType("fttruetype");
            result = true;
        }

        // \*\fontfile
        if (ctrlWordData.CtrlWord.Equals("fontemb", StringComparison.Ordinal))
        {
            _state = SettingFontFile;
            result = true;
        }

        // codepage
        if (ctrlWordData.CtrlWord.Equals("cpg", StringComparison.Ordinal))
        {
            SetCodePage(ctrlWordData.Param);
            result = true;
        }

        LastCtrlWord = ctrlWordData;
        return result;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
    ///     @since 2.0.8
    /// </summary>
    public override bool HandleOpenGroup() => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
    ///     @since 2.0.8
    /// </summary>
    public override bool HandleOpeningSubGroup() => true;

    /// <summary>
    ///     Set the font bias
    ///     @since 2.0.8
    /// </summary>
    /// <param name="value">Bias value</param>
    public void SetBias(string value)
    {
        _fbias = int.Parse(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Set the character-set to the parsed value.
    ///     @since 2.0.8
    /// </summary>
    /// <param name="charset">The charset value</param>
    public void SetCharset(string charset)
    {
        if (charset == null)
        {
            throw new ArgumentNullException(nameof(charset));
        }

        if (charset.Length == 0)
        {
            charset = "0";
        }

        _charset = charset;
    }

    /// <summary>
    ///     Set the code page
    ///     @since 2.0.8
    /// </summary>
    /// <param name="value">The code page value</param>
    public void SetCodePage(string value)
    {
        _cpg = value;
    }

    /// <summary>
    ///     Set the alternate font name.
    ///     @since 2.0.8
    /// </summary>
    /// <param name="fontAlternate">The falt font value</param>
    public void SetFontAlternate(string fontAlternate)
    {
        _falt = fontAlternate;
    }

    /// <summary>
    ///     Set the font family to the parsed value.
    ///     @since 2.0.8
    /// </summary>
    /// <param name="fontFamily">The font family.</param>
    public void SetFontFamily(string fontFamily)
    {
        _fontFamily = fontFamily;
    }

    /// <summary>
    ///     Set the font name to the parsed value.
    ///     @since 2.0.8
    /// </summary>
    /// <param name="fontName">The font name.</param>
    public void SetFontName(string fontName)
    {
        _fontName = fontName;
    }

    /// <summary>
    ///     Set the font number to the parsed value.
    ///     This is used for mapping fonts to the new font numbers
    ///     @since 2.0.8
    /// </summary>
    /// <param name="fontNr">The font number.</param>
    public void SetFontNumber(string fontNr)
    {
        _fontNr = fontNr;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#setParser(com.lowagie.text.rtf.parser.RtfParser)
    ///     @since 2.0.8
    /// </summary>
    public override void SetParser(RtfParser parser)
    {
        if (RtfParser != null && RtfParser.Equals(parser))
        {
            return;
        }

        RtfParser = parser;
        init(true);
    }

    /// <summary>
    ///     Set the font pitch
    ///     @since 2.0.8
    /// </summary>
    /// <param name="value">Pitch value</param>
    public void SetPitch(string value)
    {
        _fprq = int.Parse(value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Set the font theme
    ///     @since 2.0.8
    /// </summary>
    /// <param name="themeFont">Theme value</param>
    public void SetThemeFont(string themeFont)
    {
        _themeFont = themeFont;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#setDefaults()
    ///     @since 2.0.8
    /// </summary>
    public override void SetToDefaults()
    {
        _themeFont = "";
        _fontNr = "";
        _fontName = "";
        _fontFamily = "";

        _charset = "";
        _fprq = 0;
        _panose = "";
        //this.nontaggedname = "";
        _falt = "";
        //this.fontemb = "";
        //this.fontType = "";
        //this.fontFile = "";
        //this.fontFileCpg = "";
        _fbias = 0;
        _cpg = "";
        _trueType = "";
        _state = SettingNormal;
    }

    /// <summary>
    ///     Set the TrueTtype type
    ///     @since 2.0.8
    /// </summary>
    /// <param name="value">The type</param>
    public void SetTrueType(string value)
    {
        _trueType = value;
    }

    /// <summary>
    ///     Create a font via the  FontFactory
    ///     @since 2.0.8
    /// </summary>
    /// <param name="fontName">The font name to create</param>
    /// <returns>The created  Font  object</returns>
    private static Font createfont(string fontName)
    {
        Font f1 = null;
        var pos = -1;
        do
        {
            f1 = FontFactory.GetFont(fontName);

            if (f1.BaseFont != null)
            {
                break; // found a font, exit the do/while
            }

            pos = fontName.LastIndexOf(" ", StringComparison.Ordinal); // find the last space
            if (pos > 0)
            {
                fontName = fontName.Substring(0, pos); // truncate it to the last space
            }
        } while (pos > 0);

        return f1;
    }

    /// <summary>
    ///     Load system fonts into the static  FontFactory  object
    ///     @since 2.0.8
    /// </summary>
    private static void importSystemFonts()
    {
        FontFactory.RegisterDirectories();
    }

    /// <summary>
    ///     Initialize the object.
    ///     @since 2.0.8
    /// </summary>
    /// <param name="importFonts">true to import the fonts into the FontFactory, false do not load fonts</param>
    private void init(bool importFonts)
    {
        _fontMap = new NullValueDictionary<string, Font>();
        if (RtfParser != null)
        {
            _importHeader = RtfParser.GetImportManager();
        }

        SetToDefaults();
        if (importFonts)
        {
            importSystemFonts();
        }
    }

    /// <summary>
    ///     Process the font information that was parsed from the input.
    ///     @since 2.0.8
    /// </summary>
    private void processFont()
    {
        _fontName = _fontName.Trim();
        if (_fontName.Length == 0)
        {
            return;
        }

        if (_fontNr.Length == 0)
        {
            return;
        }

        if (_fontName.Length > 0 && _fontName.IndexOf(";", StringComparison.Ordinal) >= 0)
        {
            _fontName = _fontName.Substring(0, _fontName.IndexOf(";", StringComparison.Ordinal));
        }

        if (RtfParser.IsImport())
        {
            //TODO: If primary font fails, use the alternate
            //TODO: Problem: RtfFont defaults family to \froman and doesn't allow any other family.
            // if you set the family, it changes the font name and not the family in the Font.java class.

            //          if (this.fontFamily.Length() > 0) {
            //              if (this.importHeader.ImportFont(this.fontNr, this.fontName, this.fontFamily, Integer.ParseInt(this.charset)) == false) {
            //                  if (this.falt.Length() > 0) {
            //                      this.importHeader.ImportFont(this.fontNr, this.falt, this.fontFamily, Integer.ParseInt(this.charset));
            //                  }
            //              }
            //          } else {
            if (!_importHeader.ImportFont(_fontNr, _fontName,
                                          int.Parse(string.IsNullOrEmpty(_charset) ? CharsetDefault : _charset,
                                                    CultureInfo.InvariantCulture)))
            {
                if (_falt.Length > 0)
                {
                    _importHeader.ImportFont(_fontNr, _falt,
                                             int.Parse(string.IsNullOrEmpty(_charset) ? CharsetDefault : _charset,
                                                       CultureInfo.InvariantCulture));
                }
            }
            //          }
        }

        if (RtfParser.IsConvert())
        {
            // This could probably be written as a better font matching function

            var fName = _fontName; // work variable for trimming name if needed.
            var f1 = createfont(fName);
            if (f1.BaseFont == null && _falt.Length > 0)
            {
                f1 = createfont(_falt);
            }

            if (f1.BaseFont == null)
            {
                // Did not find a font, let's try a substring of the first name.
                if (FontFactory.COURIER.IndexOf(fName, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    f1 = FontFactory.GetFont(FontFactory.COURIER);
                }
                else if (FontFactory.HELVETICA.IndexOf(fName, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    f1 = FontFactory.GetFont(FontFactory.HELVETICA);
                }
                else if (FontFactory.TIMES.IndexOf(fName, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    f1 = FontFactory.GetFont(FontFactory.TIMES);
                }
                else if (FontFactory.SYMBOL.IndexOf(fName, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    f1 = FontFactory.GetFont(FontFactory.SYMBOL);
                }
                else if (FontFactory.ZAPFDINGBATS.IndexOf(fName, StringComparison.OrdinalIgnoreCase) > -1)
                {
                    f1 = FontFactory.GetFont(FontFactory.ZAPFDINGBATS);
                }
                else
                {
                    // we did not find a matching font in any form.
                    // default to HELVETICA for now.
                    f1 = FontFactory.GetFont(FontFactory.HELVETICA);
                }
            }

            _fontMap[_fontNr] = f1;
            //System.out.Println(f1.GetFamilyname());
        }

        SetToDefaults();
    }
}