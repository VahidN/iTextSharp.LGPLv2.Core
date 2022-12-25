using System;
using System.Collections;
using System.Globalization;
using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text.html.simpleparser
{

    /// <summary>
    /// @author  psoares
    /// </summary>
    public class FactoryProperties
    {

        public static Hashtable FollowTags = new Hashtable();

        static FactoryProperties()
        {
            FollowTags["i"] = "i";
            FollowTags["b"] = "b";
            FollowTags["u"] = "u";
            FollowTags["sub"] = "sub";
            FollowTags["sup"] = "sup";
            FollowTags["em"] = "i";
            FollowTags["strong"] = "b";
            FollowTags["s"] = "s";
            FollowTags["strike"] = "s";
        }

        public FontFactoryImp FontImp { get; set; } = FontFactory.FontImp;

        public static ListItem CreateListItem(ChainedProperties props)
        {
            ListItem p = new ListItem();
            CreateParagraph(p, props);
            return p;
        }

        public static void CreateParagraph(Paragraph p, ChainedProperties props)
        {
            string value = props["align"];
            if (value != null)
            {
                if (Util.EqualsIgnoreCase(value, "center"))
                    p.Alignment = Element.ALIGN_CENTER;
                else if (Util.EqualsIgnoreCase(value, "right"))
                    p.Alignment = Element.ALIGN_RIGHT;
                else if (Util.EqualsIgnoreCase(value, "justify"))
                    p.Alignment = Element.ALIGN_JUSTIFIED;
            }
            p.Hyphenation = GetHyphenation(props);
            setParagraphLeading(p, props["leading"]);
            value = props["before"];
            if (value != null)
            {
                try
                {
                    p.SpacingBefore = float.Parse(value, NumberFormatInfo.InvariantInfo);
                }
                catch { }
            }
            value = props["after"];
            if (value != null)
            {
                try
                {
                    p.SpacingAfter = float.Parse(value, NumberFormatInfo.InvariantInfo);
                }
                catch { }
            }
            value = props["extraparaspace"];
            if (value != null)
            {
                try
                {
                    p.ExtraParagraphSpace = float.Parse(value, NumberFormatInfo.InvariantInfo);
                }
                catch { }
            }
        }

        public static Paragraph CreateParagraph(ChainedProperties props)
        {
            Paragraph p = new Paragraph();
            CreateParagraph(p, props);
            return p;
        }

        /// <summary>
        /// Gets a HyphenationEvent based on the hyphenation entry in ChainedProperties.
        /// @since    2.1.2
        /// </summary>
        /// <param name="props">ChainedProperties</param>
        /// <returns>a HyphenationEvent</returns>
        public static IHyphenationEvent GetHyphenation(ChainedProperties props)
        {
            return GetHyphenation(props["hyphenation"]);
        }

        /// <summary>
        /// Gets a HyphenationEvent based on the hyphenation entry in a HashMap.
        /// @since    2.1.2
        /// </summary>
        /// <param name="props">a HashMap with properties</param>
        /// <returns>a HyphenationEvent</returns>
        public static IHyphenationEvent GetHyphenation(Hashtable props)
        {
            return GetHyphenation((string)props["hyphenation"]);
        }

        /// <summary>
        /// Gets a HyphenationEvent based on a String.
        /// For instance "en_UK,3,2" returns new HyphenationAuto("en", "UK", 3, 2);
        /// @since    2.1.2
        /// </summary>
        /// <param name="s">String, for instance "en_UK,2,2"</param>
        /// <returns>a HyphenationEvent</returns>
        public static IHyphenationEvent GetHyphenation(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            string lang = s;
            string country = null;
            int leftMin = 2;
            int rightMin = 2;

            int pos = s.IndexOf("_", StringComparison.Ordinal);
            if (pos == -1)
            {
                return new HyphenationAuto(lang, country, leftMin, rightMin);
            }
            lang = s.Substring(0, pos);
            country = s.Substring(pos + 1);
            pos = country.IndexOf(",", StringComparison.Ordinal);
            if (pos == -1)
            {
                return new HyphenationAuto(lang, country, leftMin, rightMin);
            }
            s = country.Substring(pos + 1);
            country = country.Substring(0, pos);
            pos = s.IndexOf(",", StringComparison.Ordinal);
            if (pos == -1)
            {
                leftMin = int.Parse(s, CultureInfo.InvariantCulture);
            }
            else
            {
                leftMin = int.Parse(s.Substring(0, pos), CultureInfo.InvariantCulture);
                rightMin = int.Parse(s.Substring(pos + 1), CultureInfo.InvariantCulture);
            }
            return new HyphenationAuto(lang, country, leftMin, rightMin);
        }

        /// <summary>
        /// This method isn't used by iText, but you can use it to analyze
        /// the value of a style attribute inside a HashMap.
        /// The different elements of the style attribute are added to the
        /// HashMap as key-value pairs.
        /// style. After this method is invoked, more keys could be added.
        /// </summary>
        /// <param name="h">that should have at least a key named</param>
        public static void InsertStyle(Hashtable h)
        {
            string style = (string)h["style"];
            if (style == null)
                return;
            Properties prop = Markup.ParseAttributes(style);
            foreach (string key in prop.Keys)
            {
                if (key.Equals(Markup.CSS_KEY_FONTFAMILY))
                {
                    h["face"] = prop[key];
                }
                else if (key.Equals(Markup.CSS_KEY_FONTSIZE))
                {
                    h["size"] = Markup.ParseLength(prop[key]).ToString(NumberFormatInfo.InvariantInfo) + "pt";
                }
                else if (key.Equals(Markup.CSS_KEY_FONTSTYLE))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals("italic") || ss.Equals("oblique"))
                        h["i"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_FONTWEIGHT))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals("bold") || ss.Equals("700") || ss.Equals("800") || ss.Equals("900"))
                        h["b"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_TEXTDECORATION))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals(Markup.CSS_VALUE_UNDERLINE))
                        h["u"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_COLOR))
                {
                    BaseColor c = Markup.DecodeColor(prop[key]);
                    if (c != null)
                    {
                        int hh = c.ToArgb() & 0xffffff;
                        string hs = "#" + hh.ToString("X06", NumberFormatInfo.InvariantInfo);
                        h["color"] = hs;
                    }
                }
                else if (key.Equals(Markup.CSS_KEY_LINEHEIGHT))
                {
                    string ss = prop[key].Trim();
                    float v = Markup.ParseLength(prop[key]);
                    if (ss.EndsWith("%"))
                    {
                        v /= 100;
                        h["leading"] = "0," + v.ToString(NumberFormatInfo.InvariantInfo);
                    }
                    else if (Util.EqualsIgnoreCase("normal", ss))
                    {
                        h["leading"] = "0,1.5";
                    }
                    else
                    {
                        h["leading"] = v.ToString(NumberFormatInfo.InvariantInfo) + ",0";
                    }
                }
                else if (key.Equals(Markup.CSS_KEY_TEXTALIGN))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    h["align"] = ss;
                }
            }
        }

        /// <summary>
        /// New method contributed by Lubos Strapko
        /// @since 2.1.3
        /// </summary>
        /// <param name="h"></param>
        /// <param name="cprops"></param>
        public static void InsertStyle(Hashtable h, ChainedProperties cprops)
        {
            string style = (string)h["style"];
            if (style == null)
                return;
            Properties prop = Markup.ParseAttributes(style);
            foreach (string key in prop.Keys)
            {
                if (key.Equals(Markup.CSS_KEY_FONTFAMILY))
                {
                    h["face"] = prop[key];
                }
                else if (key.Equals(Markup.CSS_KEY_FONTSIZE))
                {
                    float actualFontSize = Markup.ParseLength(cprops[ElementTags.SIZE], Markup.DEFAULT_FONT_SIZE);
                    if (actualFontSize <= 0f)
                        actualFontSize = Markup.DEFAULT_FONT_SIZE;
                    h[ElementTags.SIZE] = Markup.ParseLength(prop[key], actualFontSize).ToString(NumberFormatInfo.InvariantInfo) + "pt";
                }
                else if (key.Equals(Markup.CSS_KEY_FONTSTYLE))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals("italic") || ss.Equals("oblique"))
                        h["i"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_FONTWEIGHT))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals("bold") || ss.Equals("700") || ss.Equals("800") || ss.Equals("900"))
                        h["b"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_TEXTDECORATION))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    if (ss.Equals(Markup.CSS_VALUE_UNDERLINE))
                        h["u"] = null;
                }
                else if (key.Equals(Markup.CSS_KEY_COLOR))
                {
                    BaseColor c = Markup.DecodeColor(prop[key]);
                    if (c != null)
                    {
                        int hh = c.ToArgb() & 0xffffff;
                        string hs = "#" + hh.ToString("X06", NumberFormatInfo.InvariantInfo);
                        h["color"] = hs;
                    }
                }
                else if (key.Equals(Markup.CSS_KEY_LINEHEIGHT))
                {
                    string ss = prop[key].Trim();
                    float actualFontSize = Markup.ParseLength(cprops[ElementTags.SIZE], Markup.DEFAULT_FONT_SIZE);
                    if (actualFontSize <= 0f)
                        actualFontSize = Markup.DEFAULT_FONT_SIZE;
                    float v = Markup.ParseLength(prop[key], actualFontSize);
                    if (ss.EndsWith("%"))
                    {
                        v /= 100;
                        h["leading"] = "0," + v.ToString(NumberFormatInfo.InvariantInfo);
                    }
                    else if (Util.EqualsIgnoreCase("normal", ss))
                    {
                        h["leading"] = "0,1.5";
                    }
                    else
                    {
                        h["leading"] = v.ToString(NumberFormatInfo.InvariantInfo) + ",0";
                    }
                }
                else if (key.Equals(Markup.CSS_KEY_TEXTALIGN))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    h["align"] = ss;
                }
                else if (key.Equals(Markup.CSS_KEY_PADDINGLEFT))
                {
                    string ss = prop[key].Trim().ToLower(CultureInfo.InvariantCulture);
                    h["indent"] = ss;
                }
            }
        }

        public Chunk CreateChunk(string text, ChainedProperties props)
        {
            Font font = GetFont(props);
            float size = font.Size;
            size /= 2;
            Chunk ck = new Chunk(text, font);
            if (props.HasProperty("sub"))
                ck.SetTextRise(-size);
            else if (props.HasProperty("sup"))
                ck.SetTextRise(size);
            ck.SetHyphenation(GetHyphenation(props));
            return ck;
        }

        public Font GetFont(ChainedProperties props)
        {
            string face = props[ElementTags.FACE];
            if (face != null)
            {
                StringTokenizer tok = new StringTokenizer(face, ",");
                while (tok.HasMoreTokens())
                {
                    face = tok.NextToken().Trim();
                    if (face.StartsWith("\""))
                        face = face.Substring(1);
                    if (face.EndsWith("\""))
                        face = face.Substring(0, face.Length - 1);
                    if (FontImp.IsRegistered(face))
                        break;
                }
            }
            int style = 0;
            if (props.HasProperty(HtmlTags.I))
                style |= Font.ITALIC;
            if (props.HasProperty(HtmlTags.B))
                style |= Font.BOLD;
            if (props.HasProperty(HtmlTags.U))
                style |= Font.UNDERLINE;
            if (props.HasProperty(HtmlTags.S))
                style |= Font.STRIKETHRU;

            string value = props[ElementTags.SIZE];
            float size = 12;
            if (value != null)
                size = float.Parse(value, NumberFormatInfo.InvariantInfo);
            BaseColor color = Markup.DecodeColor(props["color"]);
            string encoding = props["encoding"];
            if (encoding == null)
                encoding = BaseFont.WINANSI;
            return FontImp.GetFont(face, encoding, true, size, style, color);
        }

        private static void setParagraphLeading(Paragraph p, string leading)
        {
            if (leading == null)
            {
                p.SetLeading(0, 1.5f);
                return;
            }
            try
            {
                StringTokenizer tk = new StringTokenizer(leading, " ,");
                string v = tk.NextToken();
                float v1 = float.Parse(v, NumberFormatInfo.InvariantInfo);
                if (!tk.HasMoreTokens())
                {
                    p.SetLeading(v1, 0);
                    return;
                }
                v = tk.NextToken();
                float v2 = float.Parse(v, NumberFormatInfo.InvariantInfo);
                p.SetLeading(v1, v2);
            }
            catch
            {
                p.SetLeading(0, 1.5f);
            }

        }
    }
}