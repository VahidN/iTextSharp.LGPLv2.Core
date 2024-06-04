using System.util;
using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.properties;

/// <summary>
///     RtfProperty  handles document, paragraph, etc. property values
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfProperty
{
    public const string CHARACTER = "character.";

    /// <summary>
    ///     character properties
    /// </summary>
    public const string CHARACTER_BOLD = CHARACTER + "bold";

    public const string CHARACTER_FONT = CHARACTER + "font";
    public const string CHARACTER_ITALIC = CHARACTER + "italic";
    public const string CHARACTER_SIZE = CHARACTER + "size";
    public const string CHARACTER_STYLE = CHARACTER + "style";
    public const string CHARACTER_UNDERLINE = CHARACTER + "underline";

    /// <summary>
    ///     property groups
    /// </summary>
    public const string COLOR = "color.";

    public const string COLOR_BG = COLOR + "bg";

    /// <summary>
    ///     color properties
    /// </summary>
    public const string COLOR_FG = COLOR + "fg";

    public const string DOCUMENT = "document.";
    public const string DOCUMENT_DEFAULT_FONT_NUMER = DOCUMENT + "defaultFontNumber";
    public const string DOCUMENT_ENABLE_FACING_PAGES = DOCUMENT + "enableFacingPages";
    public const string DOCUMENT_MARGIN_BOTTOM_TWIPS = DOCUMENT + "marginBottomTwips";
    public const string DOCUMENT_MARGIN_LEFT_TWIPS = DOCUMENT + "marginLeftTwips";
    public const string DOCUMENT_MARGIN_RIGHT_TWIPS = DOCUMENT + "marginRightTwips";
    public const string DOCUMENT_MARGIN_TOP_TWIPS = DOCUMENT + "marginTopTwips";
    public const string DOCUMENT_PAGE_HEIGHT_TWIPS = DOCUMENT + "pageHeightTwips";
    public const string DOCUMENT_PAGE_NUMBER_START = DOCUMENT + "pageNumberStart";
    public const string DOCUMENT_PAGE_ORIENTATION = DOCUMENT + "pageOrientation";
    public const string DOCUMENT_PAGE_WIDTH_TWIPS = DOCUMENT + "pageWidthTwips";

    /// <summary>
    ///     Justify center
    /// </summary>
    public const int JUSTIFY_CENTER = 2;

    /// <summary>
    ///     Justify full
    /// </summary>
    public const int JUSTIFY_FULL = 3;

    //Color Object
    //Color Object
    /// <summary>
    ///     paragraph properties
    /// </summary>
    /// <summary>
    ///     Justify left
    /// </summary>
    public const int JUSTIFY_LEFT = 0;

    /// <summary>
    ///     Justify right
    /// </summary>
    public const int JUSTIFY_RIGHT = 1;

    public const int OFF = 0;
    public const int ON = 1;

    /// <summary>
    ///     Landscape orientation
    /// </summary>
    public const string PAGE_LANDSCAPE = "1";

    /// <summary>
    ///     document properties
    /// </summary>
    /// <summary>
    ///     Portrait orientation
    /// </summary>
    public const string PAGE_PORTRAIT = "0";

    public const string PARAGRAPH = "paragraph.";
    public const string PARAGRAPH_BORDER = PARAGRAPH + "border";

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_BOTTOM = 1;

    public const string PARAGRAPH_BORDER_CELL = PARAGRAPH + "borderCell";

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_DIAGONAL_UL_LR = 16;

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_DIAGONAL_UR_LL = 32;

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_LEFT = 4;

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_NIL = 0;

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_RIGHT = 8;

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_TABLE_HORIZONTAL = 64;

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_TABLE_VERTICAL = 128;

    /// <summary>
    ///     possible border settting
    /// </summary>
    public const int PARAGRAPH_BORDER_TOP = 2;

    public const string PARAGRAPH_INDENT_FIRST_LINE = PARAGRAPH + "indentFirstLine";

    public const string PARAGRAPH_INDENT_LEFT = PARAGRAPH + "indentLeft";

    //  twips
    public const string PARAGRAPH_INDENT_RIGHT = PARAGRAPH + "indentRight";

    // twips
    // twips
    public const string PARAGRAPH_JUSTIFICATION = PARAGRAPH + "justification";

    /// <summary>
    ///     section properties
    /// </summary>
    /// <summary>
    ///     Decimal number format
    /// </summary>
    public const int PGN_DECIMAL = 0;

    /// <summary>
    ///     Lowercase Letter
    /// </summary>
    public const int PGN_LETTER_LOWERCASE = 4;

    /// <summary>
    ///     Uppercase Letter
    /// </summary>
    public const int PGN_LETTER_UPPERCASE = 3;

    /// <summary>
    ///     Lowercase Roman Numeral
    /// </summary>
    public const int PGN_ROMAN_NUMERAL_LOWERCASE = 2;

    /// <summary>
    ///     Uppercase Roman Numeral
    /// </summary>
    public const int PGN_ROMAN_NUMERAL_UPPERCASE = 1;

    /// <summary>
    ///     Section Break Column break
    /// </summary>
    public const int SBK_COLUMN = 1;

    /// <summary>
    ///     Section Break Even page break
    /// </summary>
    public const int SBK_EVEN = 2;

    /// <summary>
    ///     Section Break None
    /// </summary>
    public const int SBK_NONE = 0;

    /// <summary>
    ///     Section Break Odd page break
    /// </summary>
    public const int SBK_ODD = 3;

    /// <summary>
    ///     Section Break Page break
    /// </summary>
    public const int SBK_PAGE = 4;

    public const string SECTION = "section.";
    public const string SECTION_BREAK_TYPE = SECTION + "SectionBreakType";
    public const string SECTION_NUMBER_OF_COLUMNS = SECTION + "numberOfColumns";
    public const string SECTION_PAGE_NUMBER_FORMAT = SECTION + "pageNumberFormat";
    public const string SECTION_PAGE_NUMBER_POSITION_X = SECTION + "pageNumberPositionX";
    public const string SECTION_PAGE_NUMBER_POSITION_Y = SECTION + "pageNumberPositionY";

    /// <summary>
    ///     The  RtfPropertyListener .
    /// </summary>
    private readonly List<IRtfPropertyListener> _listeners = new();

    private bool _modifiedCharacter;
    private bool _modifiedDocument;
    private bool _modifiedParagraph;
    private bool _modifiedSection;

    /// <summary>
    ///     Properties for this RtfProperty object
    /// </summary>
    protected INullValueDictionary<string, object> Properties = new NullValueDictionary<string, object>();

    /// <summary>
    ///     Adds a  RtfPropertyListener  to the  RtfProperty .
    ///     the new RtfPropertyListener.
    /// </summary>
    /// <param name="listener"></param>
    public void AddRtfPropertyListener(IRtfPropertyListener listener)
    {
        _listeners.Add(listener);
    }

    public void AfterChange(string propertyName)
    {
        if (propertyName == null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        // call listener for all
        foreach (var listener in _listeners)
        {
            listener.AfterPropertyChange(propertyName);
        }

        if (propertyName.StartsWith(CHARACTER, StringComparison.Ordinal))
        {
            // call listener for character chane
        }
        else
        {
            if (propertyName.StartsWith(PARAGRAPH, StringComparison.Ordinal))
            {
                // call listener for paragraph change
            }
            else
            {
                if (propertyName.StartsWith(SECTION, StringComparison.Ordinal))
                {
                    // call listener for section change
                }
                else
                {
                    if (propertyName.StartsWith(DOCUMENT, StringComparison.Ordinal))
                    {
                        // call listener for document change
                    }
                }
            }
        }
    }

    public void BeforeChange(string propertyName)
    {
        if (propertyName == null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        // call listener for all
        foreach (var listener in _listeners)
        {
            listener.BeforePropertyChange(propertyName);
        }

        if (propertyName.StartsWith(CHARACTER, StringComparison.Ordinal))
        {
            // call listener for character chane
        }
        else
        {
            if (propertyName.StartsWith(PARAGRAPH, StringComparison.Ordinal))
            {
                // call listener for paragraph change
            }
            else
            {
                if (propertyName.StartsWith(SECTION, StringComparison.Ordinal))
                {
                    // call listener for section change
                }
                else
                {
                    if (propertyName.StartsWith(DOCUMENT, StringComparison.Ordinal))
                    {
                        // call listener for document change
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Get a group of properties.
    /// </summary>
    /// <param name="propertyGroup">The group name to obtain.</param>
    /// <returns>Properties object with requested values.</returns>
    public INullValueDictionary<string, object> GetProperties(string propertyGroup)
    {
        var props = new NullValueDictionary<string, object>();
        if (Properties.Count != 0)
        {
            //properties.get
            foreach (var key in Properties.Keys)
            {
                if (key.StartsWith(propertyGroup, StringComparison.Ordinal))
                {
                    props[key] = Properties[key];
                }
            }
        }

        return props;
    }

    /// <summary>
    ///     Get the value of the property identified by the parameter.
    /// </summary>
    /// <param name="propertyName">String containing the property name to get</param>
    /// <returns>Property Object requested or null if not found in map.</returns>
    public object GetProperty(string propertyName) => Properties[propertyName];

    /// <summary>
    /// </summary>
    /// <returns>the modified</returns>
    public bool IsModified() => _modifiedCharacter || _modifiedParagraph || _modifiedSection || _modifiedDocument;

    /// <summary>
    /// </summary>
    /// <returns>the modifiedCharacter</returns>
    public bool IsModifiedCharacter() => _modifiedCharacter;

    /// <summary>
    /// </summary>
    /// <returns>the modifiedDocument</returns>
    public bool IsModifiedDocument() => _modifiedDocument;

    /// <summary>
    /// </summary>
    /// <returns>the modifiedParagraph</returns>
    public bool IsModifiedParagraph() => _modifiedParagraph;

    /// <summary>
    /// </summary>
    /// <returns>the modifiedSection</returns>
    public bool IsModifiedSection() => _modifiedSection;

    /// <summary>
    ///     Removes a  RtfPropertyListener  from the  RtfProperty .
    ///     the new RtfPropertyListener.
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveRtfPropertyListener(IRtfPropertyListener listener)
    {
        _listeners.Remove(listener);
    }

    /// <summary>
    /// </summary>
    /// <param name="propertyName">the propertyName that is modified</param>
    /// <param name="modified">the modified to set</param>
    public void SetModified(string propertyName, bool modified)
    {
        if (propertyName == null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (propertyName.StartsWith(CHARACTER, StringComparison.Ordinal))
        {
            SetModifiedCharacter(modified);
        }
        else
        {
            if (propertyName.StartsWith(PARAGRAPH, StringComparison.Ordinal))
            {
                SetModifiedParagraph(modified);
            }
            else
            {
                if (propertyName.StartsWith(SECTION, StringComparison.Ordinal))
                {
                    SetModifiedSection(modified);
                }
                else
                {
                    if (propertyName.StartsWith(DOCUMENT, StringComparison.Ordinal))
                    {
                        SetModifiedDocument(modified);
                    }
                }
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="modifiedCharacter">the modifiedCharacter to set</param>
    public void SetModifiedCharacter(bool modifiedCharacter)
    {
        _modifiedCharacter = modifiedCharacter;
    }

    /// <summary>
    /// </summary>
    /// <param name="modifiedDocument">the modifiedDocument to set</param>
    public void SetModifiedDocument(bool modifiedDocument)
    {
        _modifiedDocument = modifiedDocument;
    }

    /// <summary>
    /// </summary>
    /// <param name="modifiedParagraph">the modifiedParagraph to set</param>
    public void SetModifiedParagraph(bool modifiedParagraph)
    {
        _modifiedParagraph = modifiedParagraph;
    }

    /// <summary>
    /// </summary>
    /// <param name="modifiedSection">the modifiedSection to set</param>
    public void SetModifiedSection(bool modifiedSection)
    {
        _modifiedSection = modifiedSection;
    }

    /// <summary>
    ///     Set the value of the property identified by the parameter.
    /// </summary>
    /// <param name="ctrlWordData">The controlword with the name to set</param>
    /// <returns> true  for handled or  false  if  propertyName  or  propertyValue  is  null </returns>
    public bool SetProperty(RtfCtrlWordData ctrlWordData)
    {
        if (ctrlWordData == null)
        {
            throw new ArgumentNullException(nameof(ctrlWordData));
        }

        //String propertyName, Object propertyValueNew) {
        var propertyName = ctrlWordData.SpecialHandler;
        object propertyValueNew = ctrlWordData.Param;
        // depending on the control word, set mulitiple or reset settings, etc.
        //if pard then reset settings
        //
        setProperty(propertyName, propertyValueNew);
        return true;
    }

    /// <summary>
    ///     Set all property objects to default values.
    ///     @since 2.0.8
    /// </summary>
    public void SetToDefault()
    {
        SetToDefault(COLOR);
        SetToDefault(CHARACTER);
        SetToDefault(PARAGRAPH);
        SetToDefault(SECTION);
        SetToDefault(DOCUMENT);
    }

    /// <summary>
    ///     Set individual property group to default values.
    ///     @since 2.0.8
    /// </summary>
    /// <param name="propertyGroup"> String  name of the property group to set to default.</param>
    public void SetToDefault(string propertyGroup)
    {
        if (COLOR.Equals(propertyGroup, StringComparison.Ordinal))
        {
            setProperty(COLOR_FG, new BaseColor(0, 0, 0));
            setProperty(COLOR_BG, new BaseColor(255, 255, 255));
            return;
        }

        if (CHARACTER.Equals(propertyGroup, StringComparison.Ordinal))
        {
            setProperty(CHARACTER_BOLD, 0);
            setProperty(CHARACTER_UNDERLINE, 0);
            setProperty(CHARACTER_ITALIC, 0);
            setProperty(CHARACTER_SIZE, 24); // 1/2 pt sizes
            setProperty(CHARACTER_FONT, 0);
            return;
        }

        if (PARAGRAPH.Equals(propertyGroup, StringComparison.Ordinal))
        {
            setProperty(PARAGRAPH_INDENT_LEFT, 0);
            setProperty(PARAGRAPH_INDENT_RIGHT, 0);
            setProperty(PARAGRAPH_INDENT_FIRST_LINE, 0);
            setProperty(PARAGRAPH_JUSTIFICATION, JUSTIFY_LEFT);
            setProperty(PARAGRAPH_BORDER, PARAGRAPH_BORDER_NIL);
            setProperty(PARAGRAPH_BORDER_CELL, PARAGRAPH_BORDER_NIL);
            return;
        }

        if (SECTION.Equals(propertyGroup, StringComparison.Ordinal))
        {
            setProperty(SECTION_NUMBER_OF_COLUMNS, 0);
            setProperty(SECTION_BREAK_TYPE, SBK_NONE);
            setProperty(SECTION_PAGE_NUMBER_POSITION_X, 0);
            setProperty(SECTION_PAGE_NUMBER_POSITION_Y, 0);
            setProperty(SECTION_PAGE_NUMBER_FORMAT, PGN_DECIMAL);
            return;
        }

        if (DOCUMENT.Equals(propertyGroup, StringComparison.Ordinal))
        {
            setProperty(DOCUMENT_PAGE_WIDTH_TWIPS, 12240);
            setProperty(DOCUMENT_PAGE_HEIGHT_TWIPS, 15480);
            setProperty(DOCUMENT_MARGIN_LEFT_TWIPS, 1800);
            setProperty(DOCUMENT_MARGIN_TOP_TWIPS, 1440);
            setProperty(DOCUMENT_MARGIN_RIGHT_TWIPS, 1800);
            setProperty(DOCUMENT_MARGIN_BOTTOM_TWIPS, 1440);
            setProperty(DOCUMENT_PAGE_NUMBER_START, 1);
            setProperty(DOCUMENT_ENABLE_FACING_PAGES, 1);
            setProperty(DOCUMENT_PAGE_ORIENTATION, PAGE_PORTRAIT);
            setProperty(DOCUMENT_DEFAULT_FONT_NUMER, 0);
        }
    }


    /// <summary>
    ///     Toggle the value of the property identified by the  RtfCtrlWordData.specialHandler  parameter.
    ///     Toggle values are assumed to be integer values per the RTF spec with a value of 0=off or 1=on.
    /// </summary>
    /// <param name="ctrlWordData">The property name to set</param>
    /// <returns> true  for handled or  false  if  propertyName  is  null  or <i>blank</i></returns>
    public bool ToggleProperty(RtfCtrlWordData ctrlWordData)
    {
        if (ctrlWordData == null)
        {
            throw new ArgumentNullException(nameof(ctrlWordData));
        }
        //String propertyName) {

        var propertyName = ctrlWordData.SpecialHandler;

        if (string.IsNullOrEmpty(propertyName))
        {
            return false;
        }

        var propertyValue = GetProperty(propertyName);
        if (propertyValue == null)
        {
            propertyValue = ON;
        }
        else
        {
            if (propertyValue is int)
            {
                var value = (int)propertyValue;
                if (value != 0)
                {
                    removeProperty(propertyName);
                }

                return true;
            }

            if (propertyValue is long)
            {
                var value = (long)propertyValue;
                if (value != 0)
                {
                    removeProperty(propertyName);
                }

                return true;
            }
        }

        setProperty(propertyName, propertyValue);
        return true;
    }

    /// <summary>
    ///     Add the value of the property identified by the parameter.
    /// </summary>
    /// <param name="propertyName">The property name to set</param>
    /// <param name="propertyValue">The object to set the property value to</param>
    /// <returns> true  for handled or  false  if  propertyName  is  null </returns>
    private bool addToProperty(string propertyName, int propertyValue)
    {
        if (propertyName == null)
        {
            return false;
        }

        var value = (int)Properties[propertyName];
        if ((value | propertyValue) == value)
        {
            return true;
        }

        value |= propertyValue;
        BeforeChange(propertyName);
        Properties[propertyName] = value;
        AfterChange(propertyName);
        SetModified(propertyName, true);
        return true;
    }

    /// <summary>
    ///     Add the value of the property identified by the parameter.
    /// </summary>
    /// <param name="propertyName">The property name to set</param>
    /// <param name="propertyValue">The object to set the property value to</param>
    /// <returns> true  for handled or  false  if  propertyName  is  null </returns>
    private bool addToProperty(string propertyName, long propertyValue)
    {
        if (propertyName == null)
        {
            return false;
        }

        var value = (long)Properties[propertyName];
        if ((value | propertyValue) == value)
        {
            return true;
        }

        value |= propertyValue;
        BeforeChange(propertyName);
        Properties[propertyName] = value;
        AfterChange(propertyName);
        SetModified(propertyName, true);
        return true;
    }

    private bool removeProperty(string propertyName)
    {
        if (propertyName == null)
        {
            return false;
        }

        if (Properties.ContainsKey(propertyName))
        {
            BeforeChange(propertyName);
            Properties.Remove(propertyName);
            AfterChange(propertyName);
            SetModified(propertyName, true);
        }

        return true;
    }

    /// <summary>
    ///     Set the value of the property identified by the parameter.
    /// </summary>
    /// <param name="propertyName">The property name to set</param>
    /// <param name="propertyValueNew">The object to set the property value to</param>
    /// <returns> true  for handled or  false  if  propertyName  or  propertyValue  is  null </returns>
    private bool setProperty(string propertyName, object propertyValueNew)
    {
        if (propertyName == null || propertyValueNew == null)
        {
            return false;
        }

        var propertyValueOld = GetProperty(propertyName);
        if (propertyValueOld is int && propertyValueNew is int)
        {
            var valueOld = (int)propertyValueOld;
            var valueNew = (int)propertyValueNew;
            if (valueOld == valueNew)
            {
                return true;
            }
        }
        else
        {
            if (propertyValueOld is long && propertyValueNew is long)
            {
                var valueOld = (long)propertyValueOld;
                var valueNew = (long)propertyValueNew;
                if (valueOld == valueNew)
                {
                    return true;
                }
            }
        }

        BeforeChange(propertyName);
        Properties[propertyName] = propertyValueNew;
        AfterChange(propertyName);
        SetModified(propertyName, true);
        return true;
    }

    /// <summary>
    ///     Set the value of the property identified by the parameter.
    /// </summary>
    /// <param name="propertyName">The property name to set</param>
    /// <param name="propertyValueNew">The object to set the property value to</param>
    /// <returns> true  for handled or  false  if  propertyName  is  null </returns>
    private bool setProperty(string propertyName, int propertyValueNew)
    {
        if (propertyName == null)
        {
            return false;
        }

        var propertyValueOld = GetProperty(propertyName);
        if (propertyValueOld is int)
        {
            var valueOld = (int)propertyValueOld;
            if (valueOld == propertyValueNew)
            {
                return true;
            }
        }

        BeforeChange(propertyName);
        Properties[propertyName] = propertyValueNew;
        AfterChange(propertyName);
        SetModified(propertyName, true);
        return true;
    }

    /// <summary>
    ///     Set the value of the property identified by the parameter.
    /// </summary>
    /// <param name="propertyName">The property name to set</param>
    /// <param name="propertyValueNew">The object to set the property value to</param>
    /// <returns> true  for handled or  false  if  propertyName  is  null </returns>
    private bool setProperty(string propertyName, long propertyValueNew)
    {
        if (propertyName == null)
        {
            return false;
        }

        var propertyValueOld = GetProperty(propertyName);
        if (propertyValueOld is long)
        {
            var valueOld = (long)propertyValueOld;
            if (valueOld == propertyValueNew)
            {
                return true;
            }
        }

        BeforeChange(propertyName);
        Properties[propertyName] = propertyValueNew;
        AfterChange(propertyName);
        SetModified(propertyName, true);
        return true;
    }
}