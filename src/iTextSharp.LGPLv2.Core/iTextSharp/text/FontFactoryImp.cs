using System.Collections.Concurrent;
using System.util;
using iTextSharp.text.html;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     If you are using True Type fonts, you can declare the paths of the different ttf- and ttc-files
///     to this class first and then create fonts in your code using one of the getFont method
///     without having to enter a path as parameter.
///     It's a registry class and defined as a singleton to prevent misusing it.
/// </summary>
public sealed class FontFactoryImp
{
    private static readonly Lazy<FontFactoryImp> _instance =
        new(() => new FontFactoryImp(), LazyThreadSafetyMode.ExecutionAndPublication);

    private static readonly string[] _ttFamilyOrder =
    {
        "3", "1", "1033",
        "3", "0", "1033",
        "1", "0", "0",
        "0", "3", "0",
    };

    private static readonly ConcurrentDictionary<string, List<string>> _fontFamilies = new();
    private static readonly Properties _trueTypeFonts = new();
    private static readonly object _syncLock = new();

    private FontFactoryImp()
    {
        _trueTypeFonts.Add(FontFactory.COURIER.ToLower(CultureInfo.InvariantCulture), FontFactory.COURIER);
        _trueTypeFonts.Add(FontFactory.COURIER_BOLD.ToLower(CultureInfo.InvariantCulture), FontFactory.COURIER_BOLD);
        _trueTypeFonts.Add(FontFactory.COURIER_OBLIQUE.ToLower(CultureInfo.InvariantCulture),
                           FontFactory.COURIER_OBLIQUE);
        _trueTypeFonts.Add(FontFactory.COURIER_BOLDOBLIQUE.ToLower(CultureInfo.InvariantCulture),
                           FontFactory.COURIER_BOLDOBLIQUE);
        _trueTypeFonts.Add(FontFactory.HELVETICA.ToLower(CultureInfo.InvariantCulture), FontFactory.HELVETICA);
        _trueTypeFonts.Add(FontFactory.HELVETICA_BOLD.ToLower(CultureInfo.InvariantCulture),
                           FontFactory.HELVETICA_BOLD);
        _trueTypeFonts.Add(FontFactory.HELVETICA_OBLIQUE.ToLower(CultureInfo.InvariantCulture),
                           FontFactory.HELVETICA_OBLIQUE);
        _trueTypeFonts.Add(FontFactory.HELVETICA_BOLDOBLIQUE.ToLower(CultureInfo.InvariantCulture),
                           FontFactory.HELVETICA_BOLDOBLIQUE);
        _trueTypeFonts.Add(FontFactory.SYMBOL.ToLower(CultureInfo.InvariantCulture), FontFactory.SYMBOL);
        _trueTypeFonts.Add(FontFactory.TIMES_ROMAN.ToLower(CultureInfo.InvariantCulture), FontFactory.TIMES_ROMAN);
        _trueTypeFonts.Add(FontFactory.TIMES_BOLD.ToLower(CultureInfo.InvariantCulture), FontFactory.TIMES_BOLD);
        _trueTypeFonts.Add(FontFactory.TIMES_ITALIC.ToLower(CultureInfo.InvariantCulture), FontFactory.TIMES_ITALIC);
        _trueTypeFonts.Add(FontFactory.TIMES_BOLDITALIC.ToLower(CultureInfo.InvariantCulture),
                           FontFactory.TIMES_BOLDITALIC);
        _trueTypeFonts.Add(FontFactory.ZAPFDINGBATS.ToLower(CultureInfo.InvariantCulture), FontFactory.ZAPFDINGBATS);

        var tmp = new List<string>
                  {
                      FontFactory.COURIER,
                      FontFactory.COURIER_BOLD,
                      FontFactory.COURIER_OBLIQUE,
                      FontFactory.COURIER_BOLDOBLIQUE,
                  };
        _fontFamilies[FontFactory.COURIER.ToLower(CultureInfo.InvariantCulture)] = tmp;
        tmp = new List<string>
              {
                  FontFactory.HELVETICA,
                  FontFactory.HELVETICA_BOLD,
                  FontFactory.HELVETICA_OBLIQUE,
                  FontFactory.HELVETICA_BOLDOBLIQUE,
              };
        _fontFamilies[FontFactory.HELVETICA.ToLower(CultureInfo.InvariantCulture)] = tmp;
        tmp = new List<string> { FontFactory.SYMBOL };
        _fontFamilies[FontFactory.SYMBOL.ToLower(CultureInfo.InvariantCulture)] = tmp;
        tmp = new List<string>
              {
                  FontFactory.TIMES_ROMAN,
                  FontFactory.TIMES_BOLD,
                  FontFactory.TIMES_ITALIC,
                  FontFactory.TIMES_BOLDITALIC,
              };
        _fontFamilies[FontFactory.TIMES.ToLower(CultureInfo.InvariantCulture)] = tmp;
        _fontFamilies[FontFactory.TIMES_ROMAN.ToLower(CultureInfo.InvariantCulture)] = tmp;
        tmp = new List<string> { FontFactory.ZAPFDINGBATS };
        _fontFamilies[FontFactory.ZAPFDINGBATS.ToLower(CultureInfo.InvariantCulture)] = tmp;
    }

    /// <summary>
    ///     Gets an Instance of FontFactoryImp
    /// </summary>
    public static FontFactoryImp Instance => _instance.Value;

    /// <summary>
    ///     Gets or set DefaultEmbedding
    /// </summary>
    public bool DefaultEmbedding { set; get; } = BaseFont.NOT_EMBEDDED;

    /// <summary>
    ///     Gets or set DefaultEncoding
    /// </summary>
    public string DefaultEncoding { set; get; } = BaseFont.WINANSI;

    /// <summary>
    ///     Gets a set of registered font families.
    /// </summary>
    /// <value>a set of registered font families</value>
    public static ICollection<string> RegisteredFamilies => _fontFamilies.Keys;

    /// <summary>
    ///     Gets a set of registered fontnames.
    /// </summary>
    /// <value>a set of registered fontnames</value>
    public static ICollection<string> RegisteredFonts => _trueTypeFonts.Keys;

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <param name="color">the Color of this font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, string encoding, bool embedded, float size, int style,
                               BaseColor color) =>
        GetFont(fontname, encoding, embedded, size, style, color, true);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <param name="color">the Color of this font</param>
    /// <param name="cached">
    ///     true if the font comes from the cache or is added to the cache if new, false if the font is always
    ///     created new
    /// </param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color,
                               bool cached)
    {
        lock (_syncLock)
        {
            if (fontname == null)
            {
                return new Font(Font.UNDEFINED, size, style, color);
            }

            var lowercasefontname = fontname.ToLower(CultureInfo.InvariantCulture);
            if (_fontFamilies.TryGetValue(lowercasefontname, out var tmp))
            {
                // some bugs were fixed here by Daniel Marczisovszky
                var fs = Font.NORMAL;
                var found = false;
                var s = style == Font.UNDEFINED ? Font.NORMAL : style;
                foreach (var f in tmp)
                {
                    var lcf = f.ToLower(CultureInfo.InvariantCulture);
                    fs = Font.NORMAL;
                    if (lcf.IndexOf("bold", StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        fs |= Font.BOLD;
                    }

                    if (lcf.IndexOf("italic", StringComparison.OrdinalIgnoreCase) != -1 ||
                        lcf.IndexOf("oblique", StringComparison.OrdinalIgnoreCase) != -1)
                    {
                        fs |= Font.ITALIC;
                    }

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
                    if (fontname == null)
                    {
                        return new Font(Font.UNDEFINED, size, style, color);
                    }

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
    }

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="attributes">the attributes of a Font object</param>
    /// <returns>a Font object</returns>
    public Font GetFont(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        lock (_syncLock)
        {
            string fontname = null;
            var encoding = DefaultEncoding;
            var embedded = DefaultEmbedding;
            float size = Font.UNDEFINED;
            var style = Font.NORMAL;
            BaseColor color = null;
            var value = attributes[Markup.HTML_ATTR_STYLE];
            if (!string.IsNullOrEmpty(value))
            {
                var styleAttributes = Markup.ParseAttributes(value);
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
                        while (fontname.IndexOf(",", StringComparison.Ordinal) != -1)
                        {
                            tmp = fontname.Substring(0, fontname.IndexOf(",", StringComparison.Ordinal));
                            if (IsRegistered(tmp))
                            {
                                fontname = tmp;
                            }
                            else
                            {
                                fontname = fontname.Substring(fontname.IndexOf(",", StringComparison.Ordinal) + 1);
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

            if ("true".Equals(attributes[ElementTags.EMBEDDED], StringComparison.Ordinal))
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

            var r = attributes[ElementTags.RED];
            var g = attributes[ElementTags.GREEN];
            var b = attributes[ElementTags.BLUE];
            if (r != null || g != null || b != null)
            {
                var red = 0;
                var green = 0;
                var blue = 0;
                if (r != null)
                {
                    red = int.Parse(r, CultureInfo.InvariantCulture);
                }

                if (g != null)
                {
                    green = int.Parse(g, CultureInfo.InvariantCulture);
                }

                if (b != null)
                {
                    blue = int.Parse(b, CultureInfo.InvariantCulture);
                }

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
    }

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, string encoding, bool embedded, float size, int style) =>
        GetFont(fontname, encoding, embedded, size, style, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <param name="size">the size of this font</param>
    /// <returns></returns>
    public static Font GetFont(string fontname, string encoding, bool embedded, float size) =>
        GetFont(fontname, encoding, embedded, size, Font.UNDEFINED, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, string encoding, bool embedded) =>
        GetFont(fontname, encoding, embedded, Font.UNDEFINED, Font.UNDEFINED, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <param name="color">the Color of this font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname, string encoding, float size, int style, BaseColor color) =>
        GetFont(fontname, encoding, DefaultEmbedding, size, style, color);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname, string encoding, float size, int style) =>
        GetFont(fontname, encoding, DefaultEmbedding, size, style, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="size">the size of this font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname, string encoding, float size) =>
        GetFont(fontname, encoding, DefaultEmbedding, size, Font.UNDEFINED, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname, string encoding) =>
        GetFont(fontname, encoding, DefaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <param name="color">the Color of this font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname, float size, int style, BaseColor color) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, size, style, color);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="color">the Color of this font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname, float size, BaseColor color) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, size, Font.UNDEFINED, color);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname, float size, int style) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, size, style, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="size">the size of this font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname, float size) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, size, Font.UNDEFINED, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <returns>a Font object</returns>
    public Font GetFont(string fontname) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);

    /// <summary>
    ///     Checks if a certain font is registered.
    /// </summary>
    /// <param name="fontname">the name of the font that has to be checked</param>
    /// <returns>true if the font is found</returns>
    public static bool IsRegistered(string fontname)
    {
        if (fontname == null)
        {
            throw new ArgumentNullException(nameof(fontname));
        }

        lock (_syncLock)
        {
            return _trueTypeFonts.ContainsKey(fontname.ToLower(CultureInfo.InvariantCulture));
        }
    }

    public void Register(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        var path = attributes.Remove("path");
        var alias = attributes.Remove("alias");
        Register(path, alias);
    }

    /// <summary>
    ///     Register a ttf- or a ttc-file.
    /// </summary>
    /// <param name="path">the path to a ttf- or ttc-file</param>
    public void Register(string path)
    {
        Register(path, null);
    }

    /// <summary>
    ///     Register a ttf- or a ttc-file and use an alias for the font contained in the ttf-file.
    /// </summary>
    /// <param name="path">the path to a ttf- or ttc-file</param>
    /// <param name="alias">the alias you want to use for the font</param>
    public void Register(string path, string alias)
    {
        if (path == null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        lock (_syncLock)
        {
            if (path.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                path.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
                path.IndexOf(".ttc,", StringComparison.OrdinalIgnoreCase) > 0)
            {
                var allNames = BaseFont.GetAllFontNames(path, BaseFont.WINANSI, null);
                _trueTypeFonts.Add(((string)allNames[0]).ToLower(CultureInfo.InvariantCulture), path);
                if (alias != null)
                {
                    _trueTypeFonts.Add(alias.ToLower(CultureInfo.InvariantCulture), path);
                }

                // register all the font names with all the locales
                var names = (string[][])allNames[2]; //full name
                for (var i = 0; i < names.Length; i++)
                {
                    _trueTypeFonts.Add(names[i][3].ToLower(CultureInfo.InvariantCulture), path);
                }

                string fullName = null;
                string familyName = null;
                names = (string[][])allNames[1]; //family name
                for (var k = 0; k < _ttFamilyOrder.Length; k += 3)
                {
                    for (var i = 0; i < names.Length; i++)
                    {
                        if (_ttFamilyOrder[k].Equals(names[i][0], StringComparison.Ordinal) &&
                            _ttFamilyOrder[k + 1].Equals(names[i][1], StringComparison.Ordinal) &&
                            _ttFamilyOrder[k + 2].Equals(names[i][2], StringComparison.Ordinal))
                        {
                            familyName = names[i][3].ToLower(CultureInfo.InvariantCulture);
                            k = _ttFamilyOrder.Length;
                            break;
                        }
                    }
                }

                if (familyName != null)
                {
                    var lastName = "";
                    names = (string[][])allNames[2]; //full name
                    for (var i = 0; i < names.Length; i++)
                    {
                        for (var k = 0; k < _ttFamilyOrder.Length; k += 3)
                        {
                            if (_ttFamilyOrder[k].Equals(names[i][0], StringComparison.Ordinal) &&
                                _ttFamilyOrder[k + 1].Equals(names[i][1], StringComparison.Ordinal) &&
                                _ttFamilyOrder[k + 2].Equals(names[i][2], StringComparison.Ordinal))
                            {
                                fullName = names[i][3];
                                if (fullName.Equals(lastName, StringComparison.Ordinal))
                                {
                                    continue;
                                }

                                lastName = fullName;
                                RegisterFamily(familyName, fullName, null);
                                break;
                            }
                        }
                    }
                }
            }
            else if (path.ToLower(CultureInfo.InvariantCulture).EndsWith(".ttc", StringComparison.Ordinal))
            {
                var names = BaseFont.EnumerateTtcNames(path);
                for (var i = 0; i < names.Length; i++)
                {
                    Register(path + "," + i);
                }
            }
            else if (path.ToLower(CultureInfo.InvariantCulture).EndsWith(".afm", StringComparison.Ordinal) ||
                     path.ToLower(CultureInfo.InvariantCulture).EndsWith(".pfm", StringComparison.Ordinal))
            {
                var bf = BaseFont.CreateFont(path, BaseFont.CP1252, false);
                var fullName = bf.FullFontName[0][3].ToLower(CultureInfo.InvariantCulture);
                var familyName = bf.FamilyFontName[0][3].ToLower(CultureInfo.InvariantCulture);
                var psName = bf.PostscriptFontName.ToLower(CultureInfo.InvariantCulture);
                RegisterFamily(familyName, fullName, null);
                _trueTypeFonts.Add(psName, path);
                _trueTypeFonts.Add(fullName, path);
            }
        }
    }

    /// <summary>
    ///     Register fonts in some probable directories. It usually works in Windows,
    ///     Linux and Solaris.
    /// </summary>
    /// <returns>the number of fonts registered</returns>
    public int RegisterDirectories()
    {
        lock (_syncLock)
        {
            var count = 0;
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
    }

    /// <summary>
    ///     Register all the fonts in a directory.
    /// </summary>
    /// <param name="dir">the directory</param>
    /// <returns>the number of fonts registered</returns>
    public int RegisterDirectory(string dir) => RegisterDirectory(dir, false);

    /// <summary>
    ///     Register all the fonts in a directory and possibly its subdirectories.
    ///     @since 2.1.2
    /// </summary>
    /// <param name="dir">the directory</param>
    /// <param name="scanSubdirectories">recursively scan subdirectories if  true</param>
    /// <returns>the number of fonts registered</returns>
    public int RegisterDirectory(string dir, bool scanSubdirectories)
    {
        lock (_syncLock)
        {
            var count = 0;
            try
            {
                if (!Directory.Exists(dir))
                {
                    return 0;
                }

                var files = Directory.GetFiles(dir);
                if (files == null)
                {
                    return 0;
                }

                for (var k = 0; k < files.Length; ++k)
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
                            var name = Path.GetFullPath(files[k]);
                            var suffix = name.Length < 4
                                             ? null
                                             : name.Substring(name.Length - 4).ToLower(CultureInfo.InvariantCulture);
                            if (".afm".Equals(suffix, StringComparison.Ordinal) ||
                                ".pfm".Equals(suffix, StringComparison.Ordinal))
                            {
                                /* Only register Type 1 fonts with matching .pfb files */
                                var pfb = name.Substring(0, name.Length - 4) + ".pfb";
                                if (File.Exists(pfb))
                                {
                                    Register(name, null);
                                    ++count;
                                }
                            }
                            else if (".ttf".Equals(suffix, StringComparison.Ordinal) ||
                                     ".otf".Equals(suffix, StringComparison.Ordinal) ||
                                     ".ttc".Equals(suffix, StringComparison.Ordinal))
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
    }

    /// <summary>
    ///     Register a font by giving explicitly the font family and name.
    /// </summary>
    /// <param name="familyName">the font family</param>
    /// <param name="fullName">the font name</param>
    /// <param name="path">the font path</param>
    public static void RegisterFamily(string familyName, string fullName, string path)
    {
        if (fullName == null)
        {
            throw new ArgumentNullException(nameof(fullName));
        }

        lock (_syncLock)
        {
            if (path != null)
            {
                _trueTypeFonts.Add(fullName, path);
            }

            if (!_fontFamilies.TryGetValue(familyName, out var tmp))
            {
                tmp = new List<string>
                      {
                          fullName,
                      };
                _fontFamilies[familyName] = tmp;
            }
            else
            {
                var fullNameLength = fullName.Length;
                var inserted = false;
                for (var j = 0; j < tmp.Count; ++j)
                {
                    if (tmp[j].Length >= fullNameLength)
                    {
                        tmp.Insert(j, fullName);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                {
                    tmp.Add(fullName);
                }
            }
        }
    }
}