using System.Text;

namespace iTextSharp.text.html;

/// <summary>
///     This class converts a  String  to the HTML-format of a String.
///     To convert the  String , each character is examined:
///     ASCII-characters from 000 till 031 are represented as &amp;#xxx;
///     (with xxx = the value of the character)
///     ASCII-characters from 032 t/m 127 are represented by the character itself, except for:
///     '\n' becomes &lt;BR&gt;\n
///     &quot; becomes &amp;quot;
///     &amp; becomes &amp;amp;
///     &lt; becomes &amp;lt;
///     &gt; becomes &amp;gt;
///     ASCII-characters from 128 till 255 are represented as &amp;#xxx;
///     (with xxx = the value of the character)
///     Example:
///     String htmlPresentation = HtmlEncoder.Encode("Marie-Th&#233;r&#232;se S&#248;rensen");
///     for more info: see O'Reilly; "HTML: The Definitive Guide" (page 164)
///     @author  mario.maccarini@rug.ac.be
/// </summary>
public static class HtmlEncoder
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     List with the HTML translation of all the characters.
    /// </summary>
    private static readonly string[] _htmlCode = new string[256];

    static HtmlEncoder()
    {
        for (var i = 0; i < 10; i++)
        {
            _htmlCode[i] = "&#00" + i + ";";
        }

        for (var i = 10; i < 32; i++)
        {
            _htmlCode[i] = "&#0" + i + ";";
        }

        for (var i = 32; i < 128; i++)
        {
            _htmlCode[i] = ((char)i).ToString();
        }

        // Special characters
        _htmlCode['\t'] = "\t";
        _htmlCode['\n'] = "<" + HtmlTags.NEWLINE + " />\n";
        _htmlCode['\"'] = "&quot;"; // double quote
        _htmlCode['&'] = "&amp;"; // ampersand
        _htmlCode['<'] = "&lt;"; // lower than
        _htmlCode['>'] = "&gt;"; // greater than

        for (var i = 128; i < 256; i++)
        {
            _htmlCode[i] = "&#" + i + ";";
        }
    }


    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Converts a  String  to the HTML-format of this  String .
    /// </summary>
    /// <param name="str">The  String  to convert</param>
    /// <returns>a  String </returns>
    public static string Encode(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        var n = str.Length;
        char character;
        var buffer = new StringBuilder();
        // loop over all the characters of the String.
        for (var i = 0; i < n; i++)
        {
            character = str[i];
            // the Htmlcode of these characters are added to a StringBuilder one by one
            if (character < 256)
            {
                buffer.Append(_htmlCode[character]);
            }
            else
            {
                // Improvement posted by Joachim Eyrich
                buffer.Append("&#").Append((int)character).Append(';');
            }
        }

        return buffer.ToString();
    }

    /// <summary>
    ///     Converts a  Color  into a HTML representation of this  Color .
    /// </summary>
    /// <param name="color">the  Color  that has to be converted.</param>
    /// <returns>the HTML representation of this <COLOR>Color</COLOR></returns>
    public static string Encode(BaseColor color)
    {
        if (color == null)
        {
            throw new ArgumentNullException(nameof(color));
        }

        var buffer = new StringBuilder("#");
        if (color.R < 16)
        {
            buffer.Append('0');
        }

        buffer.Append(Convert.ToString(color.R, 16));
        if (color.G < 16)
        {
            buffer.Append('0');
        }

        buffer.Append(Convert.ToString(color.G, 16));
        if (color.B < 16)
        {
            buffer.Append('0');
        }

        buffer.Append(Convert.ToString(color.B, 16));
        return buffer.ToString();
    }

    /// <summary>
    ///     Translates the alignment value.
    /// </summary>
    /// <param name="alignment">the alignment value</param>
    /// <returns>the translated value</returns>
    public static string GetAlignment(int alignment)
    {
        switch (alignment)
        {
            case Element.ALIGN_LEFT:
                return HtmlTags.ALIGN_LEFT;
            case Element.ALIGN_CENTER:
                return HtmlTags.ALIGN_CENTER;
            case Element.ALIGN_RIGHT:
                return HtmlTags.ALIGN_RIGHT;
            case Element.ALIGN_JUSTIFIED:
            case Element.ALIGN_JUSTIFIED_ALL:
                return HtmlTags.ALIGN_JUSTIFIED;
            case Element.ALIGN_TOP:
                return HtmlTags.ALIGN_TOP;
            case Element.ALIGN_MIDDLE:
                return HtmlTags.ALIGN_MIDDLE;
            case Element.ALIGN_BOTTOM:
                return HtmlTags.ALIGN_BOTTOM;
            case Element.ALIGN_BASELINE:
                return HtmlTags.ALIGN_BASELINE;
            default:
                return "";
        }
    }
}