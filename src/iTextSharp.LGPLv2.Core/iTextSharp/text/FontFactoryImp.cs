using System;
using System.IO;
using System.Collections;
using System.util;
using System.Globalization;
using iTextSharp.text.html;
using iTextSharp.text.pdf;

namespace iTextSharp.text
{
    /// <summary>
    /// If you are using True Type fonts, you can declare the paths of the different ttf- and ttc-files
    /// to this class first and then create fonts in your code using one of the getFont method
    /// without having to enter a path as parameter.
    /// </summary>
    public class FontFactoryImp
    {

        private static readonly string[] _ttFamilyOrder = {
            "3", "1", "1033",
            "3", "0", "1033",
            "1", "0", "0",
            "0", "3", "0"
        };

        /// <summary> This is a map of fontfamilies. </summary>
        private readonly Hashtable _fontFamilies = new Hashtable();

        /// <summary> This is a map of postscriptfontnames of True Type fonts and the path of their ttf- or ttc-file. </summary>
        private readonly Properties _trueTypeFonts = new Properties();
        /// <summary> This is the default value of the <VAR>embedded</VAR> variable. </summary>
        private bool _defaultEmbedding = BaseFont.NOT_EMBEDDED;

        /// <summary> This is the default encoding to use. </summary>
        private string _defaultEncoding = BaseFont.WINANSI;
        /// <summary> Creates new FontFactory </summary>
        public FontFactoryImp()
        {
            _trueTypeFonts.Add(FontFactory.COURIER.ToLower(CultureInfo.InvariantCulture), FontFactory.COURIER);
            _trueTypeFonts.Add(FontFactory.COURIER_BOLD.ToLower(CultureInfo.InvariantCulture), FontFactory.COURIER_BOLD);
            _trueTypeFonts.Add(FontFactory.COURIER_OBLIQUE.ToLower(CultureInfo.InvariantCulture), FontFactory.COURIER_OBLIQUE);
            _trueTypeFonts.Add(FontFactory.COURIER_BOLDOBLIQUE.ToLower(CultureInfo.InvariantCulture), FontFactory.COURIER_BOLDOBLIQUE);
            _trueTypeFonts.Add(FontFactory.HELVETICA.ToLower(CultureInfo.InvariantCulture), FontFactory.HELVETICA);
            _trueTypeFonts.Add(FontFactory.HELVETICA_BOLD.ToLower(CultureInfo.InvariantCulture), FontFactory.HELVETICA_BOLD);
            _trueTypeFonts.Add(FontFactory.HELVETICA_OBLIQUE.ToLower(CultureInfo.InvariantCulture), FontFactory.HELVETICA_OBLIQUE);
            _trueTypeFonts.Add(FontFactory.HELVETICA_BOLDOBLIQUE.ToLower(CultureInfo.InvariantCulture), FontFactory.HELVETICA_BOLDOBLIQUE);
            _trueTypeFonts.Add(FontFactory.SYMBOL.ToLower(CultureInfo.InvariantCulture), FontFactory.SYMBOL);
            _trueTypeFonts.Add(FontFactory.TIMES_ROMAN.ToLower(CultureInfo.InvariantCulture), FontFactory.TIMES_ROMAN);
            _trueTypeFonts.Add(FontFactory.TIMES_BOLD.ToLower(CultureInfo.InvariantCulture), FontFactory.TIMES_BOLD);
            _trueTypeFonts.Add(FontFactory.TIMES_ITALIC.ToLower(CultureInfo.InvariantCulture), FontFactory.TIMES_ITALIC);
            _trueTypeFonts.Add(FontFactory.TIMES_BOLDITALIC.ToLower(CultureInfo.InvariantCulture), FontFactory.TIMES_BOLDITALIC);
            _trueTypeFonts.Add(FontFactory.ZAPFDINGBATS.ToLower(CultureInfo.InvariantCulture), FontFactory.ZAPFDINGBATS);

            ArrayList tmp;
            tmp = new ArrayList
            {
                FontFactory.COURIER,
                FontFactory.COURIER_BOLD,
                FontFactory.COURIER_OBLIQUE,
                FontFactory.COURIER_BOLDOBLIQUE
            };
            _fontFamilies[FontFactory.COURIER.ToLower(CultureInfo.InvariantCulture)] = tmp;
            tmp = new ArrayList
            {
                FontFactory.HELVETICA,
                FontFactory.HELVETICA_BOLD,
                FontFactory.HELVETICA_OBLIQUE,
                FontFactory.HELVETICA_BOLDOBLIQUE
            };
            _fontFamilies[FontFactory.HELVETICA.ToLower(CultureInfo.InvariantCulture)] = tmp;
            tmp = new ArrayList {FontFactory.SYMBOL};
            _fontFamilies[FontFactory.SYMBOL.ToLower(CultureInfo.InvariantCulture)] = tmp;
            tmp = new ArrayList
            {
                FontFactory.TIMES_ROMAN,
                FontFactory.TIMES_BOLD,
                FontFactory.TIMES_ITALIC,
                FontFactory.TIMES_BOLDITALIC
            };
            _fontFamilies[FontFactory.TIMES.ToLower(CultureInfo.InvariantCulture)] = tmp;
            _fontFamilies[FontFactory.TIMES_ROMAN.ToLower(CultureInfo.InvariantCulture)] = tmp;
            tmp = new ArrayList {FontFactory.ZAPFDINGBATS};
            _fontFamilies[FontFactory.ZAPFDINGBATS.ToLower(CultureInfo.InvariantCulture)] = tmp;
        }

        public virtual bool DefaultEmbedding
        {
            get
            {
                return _defaultEmbedding;
            }
            set
            {
                _defaultEmbedding = value;
            }
        }

        public virtual string DefaultEncoding
        {
            get
            {
                return _defaultEncoding;
            }
            set
            {
                _defaultEncoding = value;
            }
        }

        /// <summary>
        /// Gets a set of registered font families.
        /// </summary>
        /// <value>a set of registered font families</value>
        public virtual ICollection RegisteredFamilies
        {
            get
            {
                return _fontFamilies.Keys;
            }
        }

        /// <summary>
        /// Gets a set of registered fontnames.
        /// </summary>
        /// <value>a set of registered fontnames</value>
        public virtual ICollection RegisteredFonts
        {
            get
            {
                return _trueTypeFonts.Keys;
            }
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color)
        {
            return GetFont(fontname, encoding, embedded, size, style, color, true);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <param name="cached">true if the font comes from the cache or is added to the cache if new, false if the font is always created new</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color, bool cached)
        {
            if (fontname == null) return new Font(Font.UNDEFINED, size, style, color);
            string lowercasefontname = fontname.ToLower(CultureInfo.InvariantCulture);
            ArrayList tmp = (ArrayList)_fontFamilies[lowercasefontname];
            if (tmp != null)
            {
                // some bugs were fixed here by Daniel Marczisovszky
                int fs = Font.NORMAL;
                bool found = false;
                int s = style == Font.UNDEFINED ? Font.NORMAL : style;
                foreach (string f in tmp)
                {
                    string lcf = f.ToLower(CultureInfo.InvariantCulture);
                    fs = Font.NORMAL;
                    if (lcf.ToLower(CultureInfo.InvariantCulture).IndexOf("bold") != -1) fs |= Font.BOLD;
                    if (lcf.ToLower(CultureInfo.InvariantCulture).IndexOf("italic") != -1 || lcf.ToLower(CultureInfo.InvariantCulture).IndexOf("oblique") != -1) fs |= Font.ITALIC;
                    if ((s & Font.BOLDITALIC) == fs)
                    {
                        fontname = f;
                        found = true;
                        break;
                    }
                }
                if (style != Font.UNDEFINED && found)
                {
                    style &= ~fs;
                }
            }
            BaseFont basefont = null;
            try
            {
                try
                {
                    // the font is a type 1 font or CJK font
                    basefont = BaseFont.CreateFont(fontname, encoding, embedded, cached, null, null, true);
                }
                catch (DocumentException)
                {
                }
                if (basefont == null)
                {
                    // the font is a true type font or an unknown font
                    fontname = _trueTypeFonts[fontname.ToLower(CultureInfo.InvariantCulture)];
                    // the font is not registered as truetype font
                    if (fontname == null) return new Font(Font.UNDEFINED, size, style, color);
                    // the font is registered as truetype font
                    basefont = BaseFont.CreateFont(fontname, encoding, embedded, cached, null, null);
                }
            }
            catch (DocumentException)
            {
                // this shouldn't happen
                throw;
            }
            catch (IOException)
            {
                // the font is registered as a true type font, but the path was wrong
                return new Font(Font.UNDEFINED, size, style, color);
            }
            catch
            {
                // null was entered as fontname and/or encoding
                return new Font(Font.UNDEFINED, size, style, color);
            }
            return new Font(basefont, size, style, color);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="attributes">the attributes of a Font object</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(Properties attributes)
        {
            string fontname = null;
            string encoding = _defaultEncoding;
            bool embedded = _defaultEmbedding;
            float size = Font.UNDEFINED;
            int style = Font.NORMAL;
            BaseColor color = null;
            string value = attributes[Markup.HTML_ATTR_STYLE];
            if (!string.IsNullOrEmpty(value))
            {
                Properties styleAttributes = Markup.ParseAttributes(value);
                if (styleAttributes.Count == 0)
                {
                    attributes.Add(Markup.HTML_ATTR_STYLE, value);
                }
                else
                {
                    fontname = styleAttributes[Markup.CSS_KEY_FONTFAMILY];
                    if (fontname != null)
                    {
                        string tmp;
                        while (fontname.IndexOf(',') != -1)
                        {
                            tmp = fontname.Substring(0, fontname.IndexOf(','));
                            if (IsRegistered(tmp))
                            {
                                fontname = tmp;
                            }
                            else
                            {
                                fontname = fontname.Substring(fontname.IndexOf(',') + 1);
                            }
                        }
                    }
                    if ((value = styleAttributes[Markup.CSS_KEY_FONTSIZE]) != null)
                    {
                        size = Markup.ParseLength(value);
                    }
                    if ((value = styleAttributes[Markup.CSS_KEY_FONTWEIGHT]) != null)
                    {
                        style |= Font.GetStyleValue(value);
                    }
                    if ((value = styleAttributes[Markup.CSS_KEY_FONTSTYLE]) != null)
                    {
                        style |= Font.GetStyleValue(value);
                    }
                    if ((value = styleAttributes[Markup.CSS_KEY_COLOR]) != null)
                    {
                        color = Markup.DecodeColor(value);
                    }
                    attributes.AddAll(styleAttributes);
                }
            }
            if ((value = attributes[ElementTags.ENCODING]) != null)
            {
                encoding = value;
            }
            if ("true".Equals(attributes[ElementTags.EMBEDDED]))
            {
                embedded = true;
            }
            if ((value = attributes[ElementTags.FONT]) != null)
            {
                fontname = value;
            }
            if ((value = attributes[ElementTags.SIZE]) != null)
            {
                size = float.Parse(value, NumberFormatInfo.InvariantInfo);
            }
            if ((value = attributes[Markup.HTML_ATTR_STYLE]) != null)
            {
                style |= Font.GetStyleValue(value);
            }
            if ((value = attributes[ElementTags.STYLE]) != null)
            {
                style |= Font.GetStyleValue(value);
            }
            string r = attributes[ElementTags.RED];
            string g = attributes[ElementTags.GREEN];
            string b = attributes[ElementTags.BLUE];
            if (r != null || g != null || b != null)
            {
                int red = 0;
                int green = 0;
                int blue = 0;
                if (r != null) red = int.Parse(r);
                if (g != null) green = int.Parse(g);
                if (b != null) blue = int.Parse(b);
                color = new BaseColor(red, green, blue);
            }
            else if ((value = attributes[ElementTags.COLOR]) != null)
            {
                color = Markup.DecodeColor(value);
            }
            if (fontname == null)
            {
                return GetFont(null, encoding, embedded, size, style, color);
            }
            return GetFont(fontname, encoding, embedded, size, style, color);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <returns>a Font object</returns>
        public Font GetFont(string fontname, string encoding, bool embedded, float size, int style)
        {
            return GetFont(fontname, encoding, embedded, size, style, null);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <param name="size">the size of this font</param>
        /// <returns></returns>
        public virtual Font GetFont(string fontname, string encoding, bool embedded, float size)
        {
            return GetFont(fontname, encoding, embedded, size, Font.UNDEFINED, null);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="embedded">true if the font is to be embedded in the PDF</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, string encoding, bool embedded)
        {
            return GetFont(fontname, encoding, embedded, Font.UNDEFINED, Font.UNDEFINED, null);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, string encoding, float size, int style, BaseColor color)
        {
            return GetFont(fontname, encoding, _defaultEmbedding, size, style, color);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, string encoding, float size, int style)
        {
            return GetFont(fontname, encoding, _defaultEmbedding, size, style, null);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <param name="size">the size of this font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, string encoding, float size)
        {
            return GetFont(fontname, encoding, _defaultEmbedding, size, Font.UNDEFINED, null);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="encoding">the encoding of the font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, string encoding)
        {
            return GetFont(fontname, encoding, _defaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, float size, int style, BaseColor color)
        {
            return GetFont(fontname, _defaultEncoding, _defaultEmbedding, size, style, color);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="color">the Color of this font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, float size, BaseColor color)
        {
            return GetFont(fontname, _defaultEncoding, _defaultEmbedding, size, Font.UNDEFINED, color);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="size">the size of this font</param>
        /// <param name="style">the style of this font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, float size, int style)
        {
            return GetFont(fontname, _defaultEncoding, _defaultEmbedding, size, style, null);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <param name="size">the size of this font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname, float size)
        {
            return GetFont(fontname, _defaultEncoding, _defaultEmbedding, size, Font.UNDEFINED, null);
        }

        /// <summary>
        /// Constructs a Font-object.
        /// </summary>
        /// <param name="fontname">the name of the font</param>
        /// <returns>a Font object</returns>
        public virtual Font GetFont(string fontname)
        {
            return GetFont(fontname, _defaultEncoding, _defaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);
        }

        /// <summary>
        /// Checks if a certain font is registered.
        /// </summary>
        /// <param name="fontname">the name of the font that has to be checked</param>
        /// <returns>true if the font is found</returns>
        public virtual bool IsRegistered(string fontname)
        {
            return _trueTypeFonts.ContainsKey(fontname.ToLower(CultureInfo.InvariantCulture));
        }

        public virtual void Register(Properties attributes)
        {
            string path;
            string alias = null;

            path = attributes.Remove("path");
            alias = attributes.Remove("alias");

            Register(path, alias);
        }

        /// <summary>
        /// Register a ttf- or a ttc-file.
        /// </summary>
        /// <param name="path">the path to a ttf- or ttc-file</param>
        public virtual void Register(string path)
        {
            Register(path, null);
        }

        /// <summary>
        /// Register a ttf- or a ttc-file and use an alias for the font contained in the ttf-file.
        /// </summary>
        /// <param name="path">the path to a ttf- or ttc-file</param>
        /// <param name="alias">the alias you want to use for the font</param>
        public virtual void Register(string path, string alias)
        {
            if (path.ToLower(CultureInfo.InvariantCulture).EndsWith(".ttf") || path.ToLower(CultureInfo.InvariantCulture).EndsWith(".otf") || path.ToLower(CultureInfo.InvariantCulture).IndexOf(".ttc,") > 0)
            {
                object[] allNames = BaseFont.GetAllFontNames(path, BaseFont.WINANSI, null);
                _trueTypeFonts.Add(((string)allNames[0]).ToLower(CultureInfo.InvariantCulture), path);
                if (alias != null)
                {
                    _trueTypeFonts.Add(alias.ToLower(CultureInfo.InvariantCulture), path);
                }
                // register all the font names with all the locales
                string[][] names = (string[][])allNames[2]; //full name
                for (int i = 0; i < names.Length; i++)
                {
                    _trueTypeFonts.Add(names[i][3].ToLower(CultureInfo.InvariantCulture), path);
                }
                string fullName = null;
                string familyName = null;
                names = (string[][])allNames[1]; //family name
                for (int k = 0; k < _ttFamilyOrder.Length; k += 3)
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (_ttFamilyOrder[k].Equals(names[i][0]) && _ttFamilyOrder[k + 1].Equals(names[i][1]) && _ttFamilyOrder[k + 2].Equals(names[i][2]))
                        {
                            familyName = names[i][3].ToLower(CultureInfo.InvariantCulture);
                            k = _ttFamilyOrder.Length;
                            break;
                        }
                    }
                }
                if (familyName != null)
                {
                    string lastName = "";
                    names = (string[][])allNames[2]; //full name
                    for (int i = 0; i < names.Length; i++)
                    {
                        for (int k = 0; k < _ttFamilyOrder.Length; k += 3)
                        {
                            if (_ttFamilyOrder[k].Equals(names[i][0]) && _ttFamilyOrder[k + 1].Equals(names[i][1]) && _ttFamilyOrder[k + 2].Equals(names[i][2]))
                            {
                                fullName = names[i][3];
                                if (fullName.Equals(lastName))
                                    continue;
                                lastName = fullName;
                                RegisterFamily(familyName, fullName, null);
                                break;
                            }
                        }
                    }
                }
            }
            else if (path.ToLower(CultureInfo.InvariantCulture).EndsWith(".ttc"))
            {
                string[] names = BaseFont.EnumerateTtcNames(path);
                for (int i = 0; i < names.Length; i++)
                {
                    Register(path + "," + i);
                }
            }
            else if (path.ToLower(CultureInfo.InvariantCulture).EndsWith(".afm") || path.ToLower(CultureInfo.InvariantCulture).EndsWith(".pfm"))
            {
                BaseFont bf = BaseFont.CreateFont(path, BaseFont.CP1252, false);
                string fullName = (bf.FullFontName[0][3]).ToLower(CultureInfo.InvariantCulture);
                string familyName = (bf.FamilyFontName[0][3]).ToLower(CultureInfo.InvariantCulture);
                string psName = bf.PostscriptFontName.ToLower(CultureInfo.InvariantCulture);
                RegisterFamily(familyName, fullName, null);
                _trueTypeFonts.Add(psName, path);
                _trueTypeFonts.Add(fullName, path);
            }
        }

        /// <summary>
        /// Register fonts in some probable directories. It usually works in Windows,
        /// Linux and Solaris.
        /// </summary>
        /// <returns>the number of fonts registered</returns>
        public virtual int RegisterDirectories()
        {
            int count = 0;
            count += RegisterDirectory("c:/windows/fonts");
            count += RegisterDirectory("c:/winnt/fonts");
            count += RegisterDirectory("d:/windows/fonts");
            count += RegisterDirectory("d:/winnt/fonts");
            count += RegisterDirectory("/usr/share/X11/fonts", true);
            count += RegisterDirectory("/usr/X/lib/X11/fonts", true);
            count += RegisterDirectory("/usr/openwin/lib/X11/fonts", true);
            count += RegisterDirectory("/usr/share/fonts", true);
            count += RegisterDirectory("/usr/X11R6/lib/X11/fonts", true);
            count += RegisterDirectory("/Library/Fonts");
            count += RegisterDirectory("/System/Library/Fonts");
            return count;
        }

        /// <summary>
        /// Register all the fonts in a directory.
        /// </summary>
        /// <param name="dir">the directory</param>
        /// <returns>the number of fonts registered</returns>
        public virtual int RegisterDirectory(string dir)
        {
            return RegisterDirectory(dir, false);
        }

        /// <summary>
        /// Register all the fonts in a directory and possibly its subdirectories.
        /// @since 2.1.2
        /// </summary>
        /// <param name="dir">the directory</param>
        /// <param name="scanSubdirectories">recursively scan subdirectories if  true</param>
        /// <returns>the number of fonts registered</returns>
        public int RegisterDirectory(string dir, bool scanSubdirectories)
        {
            int count = 0;
            try
            {
                if (!Directory.Exists(dir))
                    return 0;
                string[] files = Directory.GetFiles(dir);
                if (files == null)
                    return 0;
                for (int k = 0; k < files.Length; ++k)
                {
                    try
                    {
                        if (Directory.Exists(files[k]))
                        {
                            if (scanSubdirectories)
                            {
                                count += RegisterDirectory(Path.GetFullPath(files[k]), true);
                            }
                        }
                        else
                        {
                            string name = Path.GetFullPath(files[k]);
                            string suffix = name.Length < 4 ? null : name.Substring(name.Length - 4).ToLower(CultureInfo.InvariantCulture);
                            if (".afm".Equals(suffix) || ".pfm".Equals(suffix))
                            {
                                /* Only register Type 1 fonts with matching .pfb files */
                                string pfb = name.Substring(0, name.Length - 4) + ".pfb";
                                if (File.Exists(pfb))
                                {
                                    Register(name, null);
                                    ++count;
                                }
                            }
                            else if (".ttf".Equals(suffix) || ".otf".Equals(suffix) || ".ttc".Equals(suffix))
                            {
                                Register(name, null);
                                ++count;
                            }
                        }
                    }
                    catch
                    {
                        //empty on purpose
                    }
                }
            }
            catch
            {
                //empty on purpose
            }
            return count;
        }

        /// <summary>
        /// Register a font by giving explicitly the font family and name.
        /// </summary>
        /// <param name="familyName">the font family</param>
        /// <param name="fullName">the font name</param>
        /// <param name="path">the font path</param>
        public void RegisterFamily(string familyName, string fullName, string path)
        {
            if (path != null)
                _trueTypeFonts.Add(fullName, path);
            ArrayList tmp = (ArrayList)_fontFamilies[familyName];
            if (tmp == null)
            {
                tmp = new ArrayList();
                tmp.Add(fullName);
                _fontFamilies[familyName] = tmp;
            }
            else
            {
                int fullNameLength = fullName.Length;
                bool inserted = false;
                for (int j = 0; j < tmp.Count; ++j)
                {
                    if (((string)tmp[j]).Length >= fullNameLength)
                    {
                        tmp.Insert(j, fullName);
                        inserted = true;
                        break;
                    }
                }
                if (!inserted)
                    tmp.Add(fullName);
            }
        }
    }
}