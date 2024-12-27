using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text.html.simpleparser;

/// <summary>
///     @author  psoares
/// </summary>
public class FactoryProperties
{
    public static INullValueDictionary<string, string> FollowTags = new NullValueDictionary<string, string>();

    static FactoryProperties()
    {
        FollowTags[key: "i"] = "i";
        FollowTags[key: "b"] = "b";
        FollowTags[key: "u"] = "u";
        FollowTags[key: "sub"] = "sub";
        FollowTags[key: "sup"] = "sup";
        FollowTags[key: "em"] = "i";
        FollowTags[key: "strong"] = "b";
        FollowTags[key: "s"] = "s";
        FollowTags[key: "strike"] = "s";
    }

    public FontFactoryImp FontImp { get; set; } = FontFactory.FontImp;

    public static ListItem CreateListItem(ChainedProperties props)
    {
        var p = new ListItem();
        CreateParagraph(p, props);

        return p;
    }

    public static void CreateParagraph(Paragraph p, ChainedProperties props)
    {
        if (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        var value = props[key: "align"];

        if (value != null)
        {
            if (Util.EqualsIgnoreCase(value, s2: "center"))
            {
                p.Alignment = Element.ALIGN_CENTER;
            }
            else if (Util.EqualsIgnoreCase(value, s2: "right"))
            {
                p.Alignment = Element.ALIGN_RIGHT;
            }
            else if (Util.EqualsIgnoreCase(value, s2: "justify"))
            {
                p.Alignment = Element.ALIGN_JUSTIFIED;
            }
        }

        p.Hyphenation = GetHyphenation(props);
        setParagraphLeading(p, props[key: "leading"]);
        value = props[key: "before"];

        if (value != null)
        {
            try
            {
                p.SpacingBefore = float.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            catch
            {
            }
        }

        value = props[key: "after"];

        if (value != null)
        {
            try
            {
                p.SpacingAfter = float.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            catch
            {
            }
        }

        value = props[key: "extraparaspace"];

        if (value != null)
        {
            try
            {
                p.ExtraParagraphSpace = float.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            catch
            {
            }
        }
    }

    public static Paragraph CreateParagraph(ChainedProperties props)
    {
        var p = new Paragraph();
        CreateParagraph(p, props);

        return p;
    }

    /// <summary>
    ///     Gets a HyphenationEvent based on the hyphenation entry in ChainedProperties.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="props">ChainedProperties</param>
    /// <returns>a HyphenationEvent</returns>
    public static IHyphenationEvent GetHyphenation(ChainedProperties props)
    {
        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        return GetHyphenation(props[key: "hyphenation"]);
    }

    /// <summary>
    ///     Gets a HyphenationEvent based on the hyphenation entry in a HashMap.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="props">a HashMap with properties</param>
    /// <returns>a HyphenationEvent</returns>
    public static IHyphenationEvent GetHyphenation(INullValueDictionary<string, string> props)
    {
        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        return GetHyphenation(props[key: "hyphenation"]);
    }

    /// <summary>
    ///     Gets a HyphenationEvent based on a String.
    ///     For instance "en_UK,3,2" returns new HyphenationAuto("en", "UK", 3, 2);
    ///     @since    2.1.2
    /// </summary>
    /// <param name="s">String, for instance "en_UK,2,2"</param>
    /// <returns>a HyphenationEvent</returns>
    public static IHyphenationEvent GetHyphenation(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }

        var lang = s;
        string country = null;
        var leftMin = 2;
        var rightMin = 2;

        var pos = s.IndexOf(value: '_', StringComparison.Ordinal);

        if (pos == -1)
        {
            return new HyphenationAuto(lang, country, leftMin, rightMin);
        }

        lang = s.Substring(startIndex: 0, pos);
        country = s.Substring(pos + 1);
        pos = country.IndexOf(value: ',', StringComparison.Ordinal);

        if (pos == -1)
        {
            return new HyphenationAuto(lang, country, leftMin, rightMin);
        }

        s = country.Substring(pos + 1);
        country = country.Substring(startIndex: 0, pos);
        pos = s.IndexOf(value: ',', StringComparison.Ordinal);

        if (pos == -1)
        {
            leftMin = int.Parse(s, CultureInfo.InvariantCulture);
        }
        else
        {
            leftMin = int.Parse(s.Substring(startIndex: 0, pos), CultureInfo.InvariantCulture);
            rightMin = int.Parse(s.Substring(pos + 1), CultureInfo.InvariantCulture);
        }

        return new HyphenationAuto(lang, country, leftMin, rightMin);
    }

    /// <summary>
    ///     This method isn't used by iText, but you can use it to analyze
    ///     the value of a style attribute inside a HashMap.
    ///     The different elements of the style attribute are added to the
    ///     HashMap as key-value pairs.
    ///     style. After this method is invoked, more keys could be added.
    /// </summary>
    /// <param name="h">that should have at least a key named</param>
    public static void InsertStyle(INullValueDictionary<string, string> h)
    {
        if (h == null)
        {
            throw new ArgumentNullException(nameof(h));
        }

        var style = h[key: "style"];

        if (style == null)
        {
            return;
        }

        var prop = Markup.ParseAttributes(style);

        foreach (var key in prop.Keys)
        {
            if (key.Equals(Markup.CSS_KEY_FONTFAMILY, StringComparison.OrdinalIgnoreCase))
            {
                h[key: "face"] = prop[key];
            }
            else if (key.Equals(Markup.CSS_KEY_FONTSIZE, StringComparison.OrdinalIgnoreCase))
            {
                h[key: "size"] = Markup.ParseLength(prop[key]).ToString(NumberFormatInfo.InvariantInfo) + "pt";
            }
            else if (key.Equals(Markup.CSS_KEY_FONTSTYLE, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToUpperInvariant();

                if (ss.Equals(value: "italic", StringComparison.OrdinalIgnoreCase) ||
                    ss.Equals(value: "oblique", StringComparison.OrdinalIgnoreCase))
                {
                    h[key: "i"] = null;
                }
            }
            else if (key.Equals(Markup.CSS_KEY_FONTWEIGHT, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToUpperInvariant();

                if (ss.Equals(value: "bold", StringComparison.OrdinalIgnoreCase) ||
                    ss.Equals(value: "700", StringComparison.OrdinalIgnoreCase) ||
                    ss.Equals(value: "800", StringComparison.OrdinalIgnoreCase) ||
                    ss.Equals(value: "900", StringComparison.OrdinalIgnoreCase))
                {
                    h[key: "b"] = null;
                }
            }
            else if (key.Equals(Markup.CSS_KEY_TEXTDECORATION, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToUpperInvariant();

                if (ss.Equals(Markup.CSS_VALUE_UNDERLINE, StringComparison.OrdinalIgnoreCase))
                {
                    h[key: "u"] = null;
                }
            }
            else if (key.Equals(Markup.CSS_KEY_COLOR, StringComparison.OrdinalIgnoreCase))
            {
                var c = Markup.DecodeColor(prop[key]);

                if (c != null)
                {
                    var hh = c.ToArgb() & 0xffffff;
                    var hs = "#" + hh.ToString(format: "X06", NumberFormatInfo.InvariantInfo);
                    h[key: "color"] = hs;
                }
            }
            else if (key.Equals(Markup.CSS_KEY_LINEHEIGHT, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim();
                var v = Markup.ParseLength(prop[key]);

                if (ss.EndsWith(value: '%'))
                {
                    v /= 100;
                    h[key: "leading"] = "0," + v.ToString(NumberFormatInfo.InvariantInfo);
                }
                else if (Util.EqualsIgnoreCase(s1: "normal", ss))
                {
                    h[key: "leading"] = "0,1.5";
                }
                else
                {
                    h[key: "leading"] = v.ToString(NumberFormatInfo.InvariantInfo) + ",0";
                }
            }
            else if (key.Equals(Markup.CSS_KEY_TEXTALIGN, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                h[key: "align"] = ss;
            }
        }
    }

    /// <summary>
    ///     New method contributed by Lubos Strapko
    ///     @since 2.1.3
    /// </summary>
    /// <param name="h"></param>
    /// <param name="cprops"></param>
    public static void InsertStyle(INullValueDictionary<string, string> h, ChainedProperties cprops)
    {
        if (h == null)
        {
            throw new ArgumentNullException(nameof(h));
        }

        if (cprops == null)
        {
            throw new ArgumentNullException(nameof(cprops));
        }

        var style = h[key: "style"];

        if (style == null)
        {
            return;
        }

        var prop = Markup.ParseAttributes(style);

        foreach (var key in prop.Keys)
        {
            if (key.Equals(Markup.CSS_KEY_FONTFAMILY, StringComparison.OrdinalIgnoreCase))
            {
                h[key: "face"] = prop[key];
            }
            else if (key.Equals(Markup.CSS_KEY_FONTSIZE, StringComparison.OrdinalIgnoreCase))
            {
                var actualFontSize = Markup.ParseLength(cprops[ElementTags.SIZE], Markup.DEFAULT_FONT_SIZE);

                if (actualFontSize <= 0f)
                {
                    actualFontSize = Markup.DEFAULT_FONT_SIZE;
                }

                h[ElementTags.SIZE] =
                    Markup.ParseLength(prop[key], actualFontSize).ToString(NumberFormatInfo.InvariantInfo) + "pt";
            }
            else if (key.Equals(Markup.CSS_KEY_FONTSTYLE, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToUpperInvariant();

                if (ss.Equals(value: "italic", StringComparison.OrdinalIgnoreCase) ||
                    ss.Equals(value: "oblique", StringComparison.OrdinalIgnoreCase))
                {
                    h[key: "i"] = null;
                }
            }
            else if (key.Equals(Markup.CSS_KEY_FONTWEIGHT, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToUpperInvariant();

                if (ss.Equals(value: "bold", StringComparison.OrdinalIgnoreCase) ||
                    ss.Equals(value: "700", StringComparison.OrdinalIgnoreCase) ||
                    ss.Equals(value: "800", StringComparison.OrdinalIgnoreCase) ||
                    ss.Equals(value: "900", StringComparison.OrdinalIgnoreCase))
                {
                    h[key: "b"] = null;
                }
            }
            else if (key.Equals(Markup.CSS_KEY_TEXTDECORATION, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToUpperInvariant();

                if (ss.Equals(Markup.CSS_VALUE_UNDERLINE, StringComparison.OrdinalIgnoreCase))
                {
                    h[key: "u"] = null;
                }
            }
            else if (key.Equals(Markup.CSS_KEY_COLOR, StringComparison.OrdinalIgnoreCase))
            {
                var c = Markup.DecodeColor(prop[key]);

                if (c != null)
                {
                    var hh = c.ToArgb() & 0xffffff;
                    var hs = "#" + hh.ToString(format: "X06", NumberFormatInfo.InvariantInfo);
                    h[key: "color"] = hs;
                }
            }
            else if (key.Equals(Markup.CSS_KEY_LINEHEIGHT, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim();
                var actualFontSize = Markup.ParseLength(cprops[ElementTags.SIZE], Markup.DEFAULT_FONT_SIZE);

                if (actualFontSize <= 0f)
                {
                    actualFontSize = Markup.DEFAULT_FONT_SIZE;
                }

                var v = Markup.ParseLength(prop[key], actualFontSize);

                if (ss.EndsWith(value: '%'))
                {
                    v /= 100;
                    h[key: "leading"] = "0," + v.ToString(NumberFormatInfo.InvariantInfo);
                }
                else if (Util.EqualsIgnoreCase(s1: "normal", ss))
                {
                    h[key: "leading"] = "0,1.5";
                }
                else
                {
                    h[key: "leading"] = v.ToString(NumberFormatInfo.InvariantInfo) + ",0";
                }
            }
            else if (key.Equals(Markup.CSS_KEY_TEXTALIGN, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                h[key: "align"] = ss;
            }
            else if (key.Equals(Markup.CSS_KEY_PADDINGLEFT, StringComparison.OrdinalIgnoreCase))
            {
                var ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                h[key: "indent"] = ss;
            }
        }
    }

    public static Chunk CreateChunk(string text, ChainedProperties props)
    {
        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        var font = GetFont(props);
        var size = font.Size;
        size /= 2;
        var ck = new Chunk(text, font);

        if (props.HasProperty(key: "sub"))
        {
            ck.SetTextRise(-size);
        }
        else if (props.HasProperty(key: "sup"))
        {
            ck.SetTextRise(size);
        }

        ck.SetHyphenation(GetHyphenation(props));

        return ck;
    }

    public static Font GetFont(ChainedProperties props)
    {
        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        var face = props[ElementTags.FACE];

        if (face != null)
        {
            var tok = new StringTokenizer(face, delim: ",");

            while (tok.HasMoreTokens())
            {
                face = tok.NextToken().Trim();

                if (face.StartsWith(value: '"'))
                {
                    face = face.Substring(startIndex: 1);
                }

                if (face.EndsWith(value: '"'))
                {
                    face = face.Substring(startIndex: 0, face.Length - 1);
                }

                if (FontFactoryImp.IsRegistered(face))
                {
                    break;
                }
            }
        }

        var style = 0;

        if (props.HasProperty(HtmlTags.I))
        {
            style |= Font.ITALIC;
        }

        if (props.HasProperty(HtmlTags.B))
        {
            style |= Font.BOLD;
        }

        if (props.HasProperty(HtmlTags.U))
        {
            style |= Font.UNDERLINE;
        }

        if (props.HasProperty(HtmlTags.S))
        {
            style |= Font.STRIKETHRU;
        }

        var value = props[ElementTags.SIZE];
        float size = 12;

        if (value != null)
        {
            if (value.EndsWith(value: "pt", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(startIndex: 0, value.Length - 2);
            }

            size = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        var color = Markup.DecodeColor(props[key: "color"]);
        var encoding = props[key: "encoding"];

        if (encoding == null)
        {
            encoding = BaseFont.WINANSI;
        }

        return FontFactoryImp.GetFont(face, encoding, embedded: true, size, style, color);
    }

    private static void setParagraphLeading(Paragraph p, string leading)
    {
        if (leading == null)
        {
            p.SetLeading(fixedLeading: 0, multipliedLeading: 1.5f);

            return;
        }

        try
        {
            var tk = new StringTokenizer(leading, delim: " ,");
            var v = tk.NextToken();
            var v1 = float.Parse(v, NumberFormatInfo.InvariantInfo);

            if (!tk.HasMoreTokens())
            {
                p.SetLeading(v1, multipliedLeading: 0);

                return;
            }

            v = tk.NextToken();
            var v2 = float.Parse(v, NumberFormatInfo.InvariantInfo);
            p.SetLeading(v1, v2);
        }
        catch
        {
            p.SetLeading(fixedLeading: 0, multipliedLeading: 1.5f);
        }
    }
}