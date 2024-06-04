using System.Text;
using System.util;

namespace iTextSharp.text.html;

/// <summary>
///     A class that contains all the possible tagnames and their attributes.
/// </summary>
public static class Markup
{
    /// <summary>
    ///     iText specific
    /// </summary>
    /// <summary>
    ///     the CSS tag for background color
    /// </summary>
    public const string CSS_KEY_BGCOLOR = "background-color";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_BORDERCOLOR = "border-color";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_BORDERWIDTH = "border-width";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_BORDERWIDTHBOTTOM = "border-bottom-width";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_BORDERWIDTHLEFT = "border-left-width";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_BORDERWIDTHRIGHT = "border-right-width";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_BORDERWIDTHTOP = "border-top-width";

    /// <summary>
    ///     CSS keys
    /// </summary>
    /// <summary>
    ///     the CSS tag for text color
    /// </summary>
    public const string CSS_KEY_COLOR = "color";

    /// <summary>
    ///     CSS key that indicate the way something has to be displayed
    /// </summary>
    public const string CSS_KEY_DISPLAY = "display";

    /// <summary>
    ///     the CSS tag for the font family
    /// </summary>
    public const string CSS_KEY_FONTFAMILY = "font-family";

    /// <summary>
    ///     the CSS tag for the font size
    /// </summary>
    public const string CSS_KEY_FONTSIZE = "font-size";

    /// <summary>
    ///     the CSS tag for the font style
    /// </summary>
    public const string CSS_KEY_FONTSTYLE = "font-style";

    /// <summary>
    ///     the CSS tag for the font weight
    /// </summary>
    public const string CSS_KEY_FONTWEIGHT = "font-weight";

    /// <summary>
    ///     the CSS tag for text decorations
    /// </summary>
    public const string CSS_KEY_LINEHEIGHT = "line-height";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_MARGIN = "margin";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_MARGINBOTTOM = "margin-bottom";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_MARGINLEFT = "margin-left";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_MARGINRIGHT = "margin-right";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_MARGINTOP = "margin-top";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_PADDING = "padding";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_PADDINGBOTTOM = "padding-bottom";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_PADDINGLEFT = "padding-left";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_PADDINGRIGHT = "padding-right";

    /// <summary>
    ///     the CSS tag for the margin of an object
    /// </summary>
    public const string CSS_KEY_PADDINGTOP = "padding-top";

    /// <summary>
    ///     the CSS tag for adding a page break when the document is printed
    /// </summary>
    public const string CSS_KEY_PAGE_BREAK_AFTER = "page-break-after";

    /// <summary>
    ///     the CSS tag for adding a page break when the document is printed
    /// </summary>
    public const string CSS_KEY_PAGE_BREAK_BEFORE = "page-break-before";

    /// <summary>
    ///     the CSS tag for the horizontal alignment of an object
    /// </summary>
    public const string CSS_KEY_TEXTALIGN = "text-align";

    /// <summary>
    ///     the CSS tag for text decorations
    /// </summary>
    public const string CSS_KEY_TEXTDECORATION = "text-decoration";

    /// <summary>
    ///     the CSS tag for text decorations
    /// </summary>
    public const string CSS_KEY_VERTICALALIGN = "vertical-align";

    /// <summary>
    ///     the CSS tag for the visibility of objects
    /// </summary>
    public const string CSS_KEY_VISIBILITY = "visibility";

    /// <summary>
    ///     value for the CSS tag for adding a page break when the document is printed
    /// </summary>
    public const string CSS_VALUE_ALWAYS = "always";

    /// <summary>
    ///     CSS values
    /// </summary>
    /// <summary>
    ///     A possible value for the DISPLAY key
    /// </summary>
    public const string CSS_VALUE_BLOCK = "block";

    /// <summary>
    ///     a CSS value for text font weight
    /// </summary>
    public const string CSS_VALUE_BOLD = "bold";

    /// <summary>
    ///     the value if you want to hide objects.
    /// </summary>
    public const string CSS_VALUE_HIDDEN = "hidden";

    /// <summary>
    ///     A possible value for the DISPLAY key
    /// </summary>
    public const string CSS_VALUE_INLINE = "inline";

    /// <summary>
    ///     a CSS value for text font style
    /// </summary>
    public const string CSS_VALUE_ITALIC = "italic";

    /// <summary>
    ///     a CSS value for text decoration
    /// </summary>
    public const string CSS_VALUE_LINETHROUGH = "line-through";

    /// <summary>
    ///     A possible value for the DISPLAY key
    /// </summary>
    public const string CSS_VALUE_LISTITEM = "list-item";

    /// <summary>
    ///     a CSS value
    /// </summary>
    public const string CSS_VALUE_NONE = "none";

    /// <summary>
    ///     a CSS value
    /// </summary>
    public const string CSS_VALUE_NORMAL = "normal";

    /// <summary>
    ///     a CSS value for text font style
    /// </summary>
    public const string CSS_VALUE_OBLIQUE = "oblique";

    /// <summary>
    ///     A possible value for the DISPLAY key
    /// </summary>
    public const string CSS_VALUE_TABLE = "table";

    /// <summary>
    ///     A possible value for the DISPLAY key
    /// </summary>
    public const string CSS_VALUE_TABLECELL = "table-cell";

    /// <summary>
    ///     A possible value for the DISPLAY key
    /// </summary>
    public const string CSS_VALUE_TABLEROW = "table-row";

    /// <summary>
    ///     the CSS value for a horizontal alignment of an object
    /// </summary>
    public const string CSS_VALUE_TEXTALIGNCENTER = "center";

    /// <summary>
    ///     the CSS value for a horizontal alignment of an object
    /// </summary>
    public const string CSS_VALUE_TEXTALIGNJUSTIFY = "justify";

    /// <summary>
    ///     the CSS value for a horizontal alignment of an object
    /// </summary>
    public const string CSS_VALUE_TEXTALIGNLEFT = "left";

    /// <summary>
    ///     the CSS value for a horizontal alignment of an object
    /// </summary>
    public const string CSS_VALUE_TEXTALIGNRIGHT = "right";

    /// <summary>
    ///     a CSS value for text decoration
    /// </summary>
    public const string CSS_VALUE_UNDERLINE = "underline";

    /// <summary>
    ///     a default value for font-size
    ///     @since 2.1.3
    /// </summary>
    public const float DEFAULT_FONT_SIZE = 12f;

    /// <summary>
    ///     attribute for specifying externally defined CSS class
    /// </summary>
    public const string HTML_ATTR_CSS_CLASS = "class";

    /// <summary>
    ///     The ID attribute.
    /// </summary>
    public const string HTML_ATTR_CSS_ID = "id";

    /// <summary>
    ///     the height attribute.
    /// </summary>
    public const string HTML_ATTR_HEIGHT = "height";

    /// <summary>
    ///     HTML attributes
    /// </summary>
    /// <summary>
    ///     the hyperlink reference attribute.
    /// </summary>
    public const string HTML_ATTR_HREF = "href";

    /// <summary>
    ///     This is a possible HTML attribute for the LINK tag.
    /// </summary>
    public const string HTML_ATTR_REL = "rel";

    /// <summary>
    ///     This is used for inline css style information
    /// </summary>
    public const string HTML_ATTR_STYLE = "style";

    /// <summary>
    ///     This is a possible HTML attribute.
    /// </summary>
    public const string HTML_ATTR_STYLESHEET = "stylesheet";

    /// <summary>
    ///     This is a possible HTML attribute for the LINK tag.
    /// </summary>
    public const string HTML_ATTR_TYPE = "type";

    /// <summary>
    ///     the width attribute.
    /// </summary>
    public const string HTML_ATTR_WIDTH = "width";

    /// <summary>
    ///     the markup for the body part of a file
    /// </summary>
    public const string HTML_TAG_BODY = "body";

    /// <summary>
    ///     HTML tags
    /// </summary>
    /// <summary>
    ///     The DIV tag.
    /// </summary>
    public const string HTML_TAG_DIV = "div";

    /// <summary>
    ///     This is a possible HTML-tag.
    /// </summary>
    public const string HTML_TAG_LINK = "link";

    /// <summary>
    ///     The SPAN tag.
    /// </summary>
    public const string HTML_TAG_SPAN = "span";

    /// <summary>
    ///     This is a possible HTML attribute for the LINK tag.
    /// </summary>
    public const string HTML_VALUE_CSS = "text/css";

    /// <summary>
    ///     This is a possible value for the language attribute (SCRIPT tag).
    /// </summary>
    public const string HTML_VALUE_JAVASCRIPT = "text/javascript";

    /// <summary>
    ///     the key for any tag
    /// </summary>
    public const string ITEXT_TAG = "tag";

    /// <summary>
    ///     Converts a  Color  into a HTML representation of this  Color .
    /// </summary>
    /// <param name="s">the  Color  that has to be converted.</param>
    /// <returns>the HTML representation of this  Color </returns>
    public static BaseColor DecodeColor(string s)
    {
        if (s == null)
        {
            return null;
        }

        s = s.ToLower(CultureInfo.InvariantCulture).Trim();
        try
        {
            return WebColors.GetRgbColor(s);
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    ///     This method parses a string with attributes and returns a Properties object.
    /// </summary>
    /// <param name="str">a string of this form: 'key1="value1"; key2="value2";... keyN="valueN" '</param>
    /// <returns>a Properties object</returns>
    public static Properties ParseAttributes(string str)
    {
        var result = new Properties();
        if (str == null)
        {
            return result;
        }

        var keyValuePairs = new StringTokenizer(str, ";");
        StringTokenizer keyValuePair;
        string key;
        string value;
        while (keyValuePairs.HasMoreTokens())
        {
            keyValuePair = new StringTokenizer(keyValuePairs.NextToken(), ":");
            if (keyValuePair.HasMoreTokens())
            {
                key = keyValuePair.NextToken().Trim().Trim();
            }
            else
            {
                continue;
            }

            if (keyValuePair.HasMoreTokens())
            {
                value = keyValuePair.NextToken().Trim();
            }
            else
            {
                continue;
            }

            if (value.StartsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(1);
            }

            if (value.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(0, value.Length - 1);
            }

            result.Add(key.ToLower(CultureInfo.InvariantCulture), value);
        }

        return result;
    }

    /// <summary>
    ///     HTML values
    /// </summary>
    /// <summary>
    ///     Parses a length.
    /// </summary>
    /// <param name="str">a length in the form of an optional + or -, followed by a number and a unit.</param>
    /// <returns>a float</returns>
    public static float ParseLength(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        // TODO: Evaluate the effect of this.
        // It may change the default behavour of the methd if this is changed.
        // return ParseLength(string, Markup.DEFAULT_FONT_SIZE);
        var pos = 0;
        var length = str.Length;
        var ok = true;
        while (ok && pos < length)
        {
            switch (str[pos])
            {
                case '+':
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '.':
                    pos++;
                    break;
                default:
                    ok = false;
                    break;
            }
        }

        if (pos == 0)
        {
            return 0f;
        }

        if (pos == length)
        {
            return float.Parse(str, NumberFormatInfo.InvariantInfo);
        }

        var f = float.Parse(str.Substring(0, pos), NumberFormatInfo.InvariantInfo);
        str = str.Substring(pos);
        // inches
        if (str.StartsWith("in", StringComparison.OrdinalIgnoreCase))
        {
            return f * 72f;
        }

        // centimeters
        if (str.StartsWith("cm", StringComparison.OrdinalIgnoreCase))
        {
            return f / 2.54f * 72f;
        }

        // millimeters
        if (str.StartsWith("mm", StringComparison.OrdinalIgnoreCase))
        {
            return f / 25.4f * 72f;
        }

        // picas
        if (str.StartsWith("pc", StringComparison.OrdinalIgnoreCase))
        {
            return f * 12f;
        }

        // default: we assume the length was measured in points
        return f;
    }

    /// <summary>
    ///     New method contributed by: Lubos Strapko
    ///     @since 2.1.3
    /// </summary>
    public static float ParseLength(string str, float actualFontSize)
    {
        if (str == null)
        {
            return 0f;
        }

        var pos = 0;
        var length = str.Length;
        var ok = true;
        while (ok && pos < length)
        {
            switch (str[pos])
            {
                case '+':
                case '-':
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '.':
                    pos++;
                    break;
                default:
                    ok = false;
                    break;
            }
        }

        if (pos == 0)
        {
            return 0f;
        }

        if (pos == length)
        {
            return float.Parse(str, NumberFormatInfo.InvariantInfo);
        }

        var f = float.Parse(str.Substring(0, pos), NumberFormatInfo.InvariantInfo);
        str = str.Substring(pos);
        // inches
        if (str.StartsWith("in", StringComparison.OrdinalIgnoreCase))
        {
            return f * 72f;
        }

        // centimeters
        if (str.StartsWith("cm", StringComparison.OrdinalIgnoreCase))
        {
            return f / 2.54f * 72f;
        }

        // millimeters
        if (str.StartsWith("mm", StringComparison.OrdinalIgnoreCase))
        {
            return f / 25.4f * 72f;
        }

        // picas
        if (str.StartsWith("pc", StringComparison.OrdinalIgnoreCase))
        {
            return f * 12f;
        }

        // 1em is equal to the current font size
        if (str.StartsWith("em", StringComparison.OrdinalIgnoreCase))
        {
            return f * actualFontSize;
        }

        // one ex is the x-height of a font (x-height is usually about half the
        // font-size)
        if (str.StartsWith("ex", StringComparison.OrdinalIgnoreCase))
        {
            return f * actualFontSize / 2;
        }

        // default: we assume the length was measured in points
        return f;
    }

    /// <summary>
    ///     Removes the comments sections of a String.
    ///     the original String
    ///     the String that marks the start of a Comment section
    ///     the String that marks the end of a Comment section.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="startComment"></param>
    /// <param name="endComment"></param>
    /// <returns>the String stripped of its comment section</returns>
    public static string RemoveComment(string str, string startComment,
                                       string endComment)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (endComment == null)
        {
            throw new ArgumentNullException(nameof(endComment));
        }

        var result = new StringBuilder();
        var pos = 0;
        var end = endComment.Length;
        var start = str.IndexOf(startComment, pos, StringComparison.Ordinal);
        while (start > -1)
        {
            result.Append(str.Substring(pos, start - pos));
            pos = str.IndexOf(endComment, start, StringComparison.Ordinal) + end;
            start = str.IndexOf(startComment, pos, StringComparison.Ordinal);
        }

        result.Append(str.Substring(pos));
        return result.ToString();
    }
}