using iTextSharp.text.rtf.parser.ctrlwords;
using iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationStylesheetTable  handles data destined for the
///     Stylesheet Table destination
///     @author Howard Shank (hgshank@yahoo.com)
/// </summary>
public class RtfDestinationStylesheetTable : RtfDestination
{
    /// <summary>
    ///     Automatically adjust right indentation when docunent grid is defined
    /// </summary>
    private int _adustRightIndent;

    /// <summary>
    ///     Alignment
    /// </summary>
    /// <summary>
    ///     Alignment - page 85
    ///     \qc, \qj, \ql, \qr, \qd, \qkN, \qt
    /// </summary>
    private int _alignment = Element.ALIGN_LEFT;

    /// <summary>
    ///     Asian Typography
    /// </summary>
    /// <summary>
    ///     auto spacing betwee DBC and English
    /// </summary>
    private int _autoSpaceBetweenDbcEnglish;

    /// <summary>
    ///     auto spacing betwee DBC and numbers
    /// </summary>
    private int _autoSpaceBetweenDbcNumbers;

    private string _elementName = "";

    /// <summary>
    ///     Indentation
    /// </summary>
    /// <summary>
    ///     First line indentation.
    /// </summary>
    private int _firstLineIndent;

    /// <summary>
    /// </summary>
    /// <summary>
    ///     The RtfImportHeader to add color mappings to.
    /// </summary>
    private RtfImportMgr _importHeader;

    /// <summary>
    ///     Percentage of line occupied by Kashida justification (0 � low, 10 � medium, 20 � high).
    ///     \qkN
    /// </summary>
    private int _justificationPercentage;

    /// <summary>
    ///     Left indentation
    /// </summary>
    private int _leftIndent;

    /// <summary>
    ///     Mirror indents?
    /// </summary>
    private int _mirrorIndent;

    /// <summary>
    ///     No Character wrapping
    /// </summary>
    private int _noCharacterWrapping;

    /// <summary>
    ///     No overflow period and comma
    /// </summary>
    private int _noOverflowPeriodComma;

    /// <summary>
    ///     No Word wrapping
    /// </summary>
    private int _noWordWrapping;

    /// <summary>
    ///     Document Foratting Properties
    /// </summary>
    /// <summary>
    ///     Override orphan/widow control.
    /// </summary>
    private int _overrideWidowControl = -1;

    /// <summary>
    ///     Right indentation
    /// </summary>
    private int _rightIndent;

    private string _styleName = "";

    /// <summary>
    ///     RtfParagraphStyle  object for setting styleshee values
    ///     as they are parsed from the input.
    /// </summary>
    /// <summary>
    ///     private RtfParagraphStyle rtfParagraphStyle = null;
    /// </summary>
    /// <summary>
    ///     RTF Style number from stylesheet table.
    /// </summary>
    private int _styleNr;

    /// <summary>
    ///     What kind of style is this, Paragraph or Character or Table
    /// </summary>
    private int _styleType = RtfStyleTypes.PARAGRAPH;

    private string _type = "";

    public RtfDestinationStylesheetTable() : base(null)
    {
    }

    public RtfDestinationStylesheetTable(RtfParser parser, string type) : base(parser)
    {
        if (parser == null)
        {
            throw new ArgumentNullException(nameof(parser));
        }

        _importHeader = parser.GetImportManager();
        _type = type;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
    /// </summary>
    public override bool CloseDestination() => true;

    public static void CreateNewStyle()
    {
        //public RtfParagraphStyle(String styleName, String fontName, int fontSize, int fontStyle, Color fontColor)
        //this.rtfParagraphStyle = new RtfParagraphStyle();
    }

    /// <summary>
    ///     Get the right indent adjustment value
    /// </summary>
    /// <returns>the adustRightIndent value</returns>
    public int GetAdustRightIndent() => _adustRightIndent;

    /// <summary>
    ///     Get the alignment value.
    /// </summary>
    /// <returns>The alignment value.</returns>
    public int GetAlignment() => _alignment;

    /// <summary>
    ///     Get the auto space between DBC and English indicator.
    /// </summary>
    /// <returns>the autoSpaceBetweenDBCEnglish</returns>
    public int GetAutoSpaceBetweenDbcEnglish() => _autoSpaceBetweenDbcEnglish;

    /// <summary>
    ///     Get the auto space between DBC and Numbers indicator.
    /// </summary>
    /// <returns>the autoSpaceBetweenDBCNumbers</returns>
    public int GetAutoSpaceBetweenDbcNumbers() => _autoSpaceBetweenDbcNumbers;

    /// <summary>
    ///     Get the first line indent value.
    /// </summary>
    /// <returns>the firstLineIndent</returns>
    public int GetFirstLineIndent() => _firstLineIndent;

    /// <summary>
    ///     Get the left indent value
    /// </summary>
    /// <returns>the left indent</returns>
    public int GetIndent() => _leftIndent;

    /// <summary>
    ///     Get the justification percentage.
    /// </summary>
    /// <returns>The justification percentage value.</returns>
    public int GetJustificationPercentage() => _justificationPercentage;

    /// <summary>
    ///     Get the left indent value
    /// </summary>
    /// <returns>the leftIndent</returns>
    public int GetLeftIndent() => _leftIndent;

    /// <summary>
    ///     Get the value indicating if document has mirrored indents.
    /// </summary>
    /// <returns>the mirrorIndent</returns>
    public int GetMirrorIndent() => _mirrorIndent;

    /// <summary>
    ///     Get no character wrapping indicator.
    /// </summary>
    /// <returns>the noCharacterWrapping</returns>
    public int GetNoCharacterWrapping() => _noCharacterWrapping;

    /// <summary>
    ///     Get the no overflow period comma indicator.
    /// </summary>
    /// <returns>the noOverflowPeriodComma</returns>
    public int GetNoOverflowPeriodComma() => _noOverflowPeriodComma;

    /// <summary>
    ///     Get the no word wrapping indicator.
    /// </summary>
    /// <returns>the noWordWrapping</returns>
    public int GetNoWordWrapping() => _noWordWrapping;

    /// <summary>
    ///     Get the ovirride widow control value.
    /// </summary>
    /// <returns>the overrideWidowControl</returns>
    public int GetOverrideWidowControl() => _overrideWidowControl;

    /// <summary>
    ///     Get the right indent value.
    /// </summary>
    /// <returns>the rightIndent</returns>
    public int GetRightIndent() => _rightIndent;

    /// <summary>
    ///     Get this style number.
    /// </summary>
    /// <returns>the styleNr</returns>
    public int GetStyleNr() => _styleNr;

    /// <summary>
    ///     Get this style type.
    ///     For example Style, Character Style, etc.
    /// </summary>
    /// <returns>the styleType</returns>
    public int GetStyleType() => _styleType;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
    /// </summary>
    public override bool HandleCharacter(int ch)
    {
        _styleName += (char)ch;
        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
    /// </summary>
    public override bool HandleCloseGroup() => true;

    public override bool HandleControlWord(RtfCtrlWordData ctrlWordData)
    {
        if (ctrlWordData == null)
        {
            throw new ArgumentNullException(nameof(ctrlWordData));
        }

        var result = true;
        OnCtrlWord(ctrlWordData); // event handler

        if (RtfParser.IsImport())
        {
            // information
            if (ctrlWordData.CtrlWord.Equals("s", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("cs", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("ds", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("ts", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsrowd", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("keycode", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("shift", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("ctrl", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("alt", StringComparison.Ordinal))
            {
            }

            //cells
            if (ctrlWordData.CtrlWord.Equals("fn", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("additive", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("sbasedon", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("snext", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("sautoupd", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("shidden", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("slink", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("slocked", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("spersonal", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("scompose", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("sreply", StringComparison.Ordinal))
            {
            }
            /* FORMATTING */
            // brdrdef/parfmt/apoctl/tabdef/shading/chrfmt


            if (ctrlWordData.CtrlWord.Equals("styrsid", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("ssemihidden", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("sqformat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("spriority", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("sunhideused", StringComparison.Ordinal))
            {
            }

            /* TABLE STYLES */
            if (ctrlWordData.CtrlWord.Equals("tscellwidth", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellwidthfts", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellpaddt", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellpaddl", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellpaddr", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellpaddb", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellpaddft", StringComparison.Ordinal)) /*0-auto, 3-twips*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellpaddfl", StringComparison.Ordinal)) /*0-auto, 3-twips*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellpaddfr", StringComparison.Ordinal)) /*0-auto, 3-twips*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellpaddfb", StringComparison.Ordinal)) /*0-auto, 3-twips*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsvertalt", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsvertalc", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsvertalb", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsnowrap", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellcfpat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscellcbpat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgbdiag", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgfdiag", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgcross", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgdcross", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgdkcross ", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgdkdcross", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbghoriz", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgvert", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgdkhor", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbgdkvert", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbrdrt", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbrdrb", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbrdrl", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbrdrr", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbrdrh", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbrdrv", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbrdrdgl", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tsbrdrdgr", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscbandsh", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tscbandsv", StringComparison.Ordinal))
            {
            }
        }

        if (ctrlWordData.CtrlWordType == RtfCtrlWordType.FLAG ||
            ctrlWordData.CtrlWordType == RtfCtrlWordType.TOGGLE ||
            ctrlWordData.CtrlWordType == RtfCtrlWordType.VALUE)
        {
            RtfParser.GetState().Properties.SetProperty(ctrlWordData);
        }

        switch (RtfParser.GetConversionType())
        {
            case RtfParser.TYPE_IMPORT_FULL:
                result = true;
                break;
            case RtfParser.TYPE_IMPORT_FRAGMENT:
                result = true;
                break;
            case RtfParser.TYPE_CONVERT:
                result = true;
                break;
            default: // error because is should be an import or convert
                result = false;
                break;
        }

        return result;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
    /// </summary>
    public override bool HandleOpenGroup() => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
    /// </summary>
    public override bool HandleOpeningSubGroup() =>
        // TODO Auto-generated method stub
        false;

    /// <summary>
    ///     Set the right indent adjustment value
    /// </summary>
    /// <param name="adustRightIndent">the adustRightIndent to set</param>
    public void SetAdustRightIndent(int adustRightIndent)
    {
        _adustRightIndent = adustRightIndent;
    }

    /// <summary>
    ///     Set the alignment value from the parsed value.
    /// </summary>
    /// <param name="alignment">The alignment value.</param>
    /// <returns>The alignment value.</returns>
    public int SetAlignment(int alignment)
    {
        _alignment = alignment;
        return _alignment;
    }

    /// <summary>
    ///     Set the auto space between DBC and English indicator.
    /// </summary>
    /// <param name="autoSpaceBetweenDbcEnglish">the autoSpaceBetweenDBCEnglish to set</param>
    public void SetAutoSpaceBetweenDbcEnglish(int autoSpaceBetweenDbcEnglish)
    {
        _autoSpaceBetweenDbcEnglish = autoSpaceBetweenDbcEnglish;
    }

    /// <summary>
    ///     Set the auto space between DBC and Numbers indicator.
    /// </summary>
    /// <param name="autoSpaceBetweenDbcNumbers">the autoSpaceBetweenDBCNumbers to set</param>
    public void SetAutoSpaceBetweenDbcNumbers(int autoSpaceBetweenDbcNumbers)
    {
        _autoSpaceBetweenDbcNumbers = autoSpaceBetweenDbcNumbers;
    }

    public void SetElementName(string value)
    {
        _elementName = value;
    }

    /// <summary>
    ///     Set the first line indent value.
    /// </summary>
    /// <param name="firstLineIndent">the firstLineIndent to set</param>
    public void SetFirstLineIndent(int firstLineIndent)
    {
        _firstLineIndent = firstLineIndent;
    }

    /// <summary>
    ///     Set the left indent value from the value parsed.
    /// </summary>
    /// <param name="indent">the left indent value.</param>
    public void SetIndent(int indent)
    {
        _leftIndent = indent;
    }

    /// <summary>
    ///     Set the justification percentage from parsed value.
    /// </summary>
    /// <param name="percent">The justification percentage</param>
    /// <returns>The justification percentage</returns>
    public int SetJustificationPercentage(int percent)
    {
        _justificationPercentage = percent;
        return _justificationPercentage;
    }

    /// <summary>
    ///     Set the left indent value
    /// </summary>
    /// <param name="leftIndent">the leftIndent to set</param>
    public void SetLeftIndent(int leftIndent)
    {
        _leftIndent = leftIndent;
    }

    /// <summary>
    ///     Set the mirrored indent value from the parsed value.
    /// </summary>
    /// <param name="mirrorIndent">the mirrorIndent to set</param>
    public void SetMirrorIndent(int mirrorIndent)
    {
        _mirrorIndent = mirrorIndent;
    }

    /// <summary>
    ///     Set the no character wrapping indicator from parsed value
    /// </summary>
    /// <param name="noCharacterWrapping">the noCharacterWrapping to set</param>
    public void SetNoCharacterWrapping(int noCharacterWrapping)
    {
        _noCharacterWrapping = noCharacterWrapping;
    }

    /// <summary>
    ///     Set the no overflow period comma indicator from the parsed value.
    /// </summary>
    /// <param name="noOverflowPeriodComma">the noOverflowPeriodComma to set</param>
    public void SetNoOverflowPeriodComma(int noOverflowPeriodComma)
    {
        _noOverflowPeriodComma = noOverflowPeriodComma;
    }

    /// <summary>
    ///     Set the no word wrapping indicator from the parsed value.
    /// </summary>
    /// <param name="noWordWrapping">the noWordWrapping to set</param>
    public void SetNoWordWrapping(int noWordWrapping)
    {
        _noWordWrapping = noWordWrapping;
    }

    /// <summary>
    ///     Set the override widow control.
    /// </summary>
    /// <param name="overrideWidowControl">the overrideWidowControl to set</param>
    public void SetOverrideWidowControl(int overrideWidowControl)
    {
        _overrideWidowControl = overrideWidowControl;
    }

    public override void SetParser(RtfParser parser)
    {
        RtfParser = parser ?? throw new ArgumentNullException(nameof(parser));
        _importHeader = parser.GetImportManager();
    }

    /// <summary>
    ///     Set the right indent value.
    /// </summary>
    /// <param name="rightIndent">the rightIndent to set</param>
    public void SetRightIndent(int rightIndent)
    {
        _rightIndent = rightIndent;
    }

    /// <summary>
    ///     Set this style number from the parsed value.
    /// </summary>
    /// <param name="styleNr">the styleNr to set</param>
    public void SetStyleNr(int styleNr)
    {
        _styleNr = styleNr;
    }

    /// <summary>
    ///     Set the style type.
    /// </summary>
    /// <param name="styleType">the styleType to set</param>
    public void SetStyleType(int styleType)
    {
        _styleType = styleType;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#setToDefaults()
    /// </summary>
    public override void SetToDefaults()
    {
        _styleName = "";
        _styleNr = 0;
        _alignment = Element.ALIGN_LEFT;
        _justificationPercentage = 0;
        _firstLineIndent = 0;
        _leftIndent = 0;
        _rightIndent = 0;
        _adustRightIndent = 0;
        _mirrorIndent = 0;
        _overrideWidowControl = -1;
        _autoSpaceBetweenDbcEnglish = 0;
        _autoSpaceBetweenDbcNumbers = 0;
        _noCharacterWrapping = 0;
        _noWordWrapping = 0;
        _noOverflowPeriodComma = 0;
    }

    public void SetType(string value)
    {
        _type = value;
    }
}