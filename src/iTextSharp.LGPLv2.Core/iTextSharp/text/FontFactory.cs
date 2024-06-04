using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     If you are using True Type fonts, you can declare the paths of the different ttf- and ttc-files
///     to this static class first and then create fonts in your code using one of the static getFont-method
///     without having to enter a path as parameter.
/// </summary>
public static class FontFactory
{
    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string COURIER = BaseFont.COURIER;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string COURIER_BOLD = BaseFont.COURIER_BOLD;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string COURIER_BOLDOBLIQUE = BaseFont.COURIER_BOLDOBLIQUE;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string COURIER_OBLIQUE = BaseFont.COURIER_OBLIQUE;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string HELVETICA = BaseFont.HELVETICA;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string HELVETICA_BOLD = BaseFont.HELVETICA_BOLD;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string HELVETICA_BOLDOBLIQUE = BaseFont.HELVETICA_BOLDOBLIQUE;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string HELVETICA_OBLIQUE = BaseFont.HELVETICA_OBLIQUE;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string SYMBOL = BaseFont.SYMBOL;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string TIMES = "Times";

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string TIMES_BOLD = BaseFont.TIMES_BOLD;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string TIMES_BOLDITALIC = BaseFont.TIMES_BOLDITALIC;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string TIMES_ITALIC = BaseFont.TIMES_ITALIC;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string TIMES_ROMAN = BaseFont.TIMES_ROMAN;

    /// <summary> This is a possible value of a base 14 type 1 font </summary>
    public const string ZAPFDINGBATS = BaseFont.ZAPFDINGBATS;

    /// <summary> This is the default value of the <VAR>embedded</VAR> variable. </summary>
    public static bool DefaultEmbedding { get; } = BaseFont.NOT_EMBEDDED;

    /// <summary> This is the default encoding to use. </summary>
    public static string DefaultEncoding { get; } = BaseFont.WINANSI;

    public static FontFactoryImp FontImp { set; get; } = FontFactoryImp.Instance;

    /// <summary>
    ///     Gets a set of registered font families.
    /// </summary>
    /// <value>a set of registered font families</value>
    public static ICollection<string> RegisteredFamilies => FontFactoryImp.RegisteredFamilies;

    /// <summary>
    ///     Gets a set of registered fontnames.
    /// </summary>
    /// <value>a set of registered fontnames</value>
    public static ICollection<string> RegisteredFonts => FontFactoryImp.RegisteredFonts;

    /// <summary>
    ///     Checks whether the given font is contained within the object
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <returns>true if font is contained within the object</returns>
    public static bool Contains(string fontname) => FontFactoryImp.IsRegistered(fontname);

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
    public static Font
        GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color) =>
        FontFactoryImp.GetFont(fontname, encoding, embedded, size, style, color);

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
                               bool cached) =>
        FontFactoryImp.GetFont(fontname, encoding, embedded, size, style, color, cached);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="attributes">the attributes of a Font object</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(Properties attributes)
    {
        FontImp.DefaultEmbedding = DefaultEmbedding;
        FontImp.DefaultEncoding = DefaultEncoding;
        return FontImp.GetFont(attributes);
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
    public static Font GetFont(string fontname, string encoding, float size, int style, BaseColor color) =>
        GetFont(fontname, encoding, DefaultEmbedding, size, style, color);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, string encoding, float size, int style) =>
        GetFont(fontname, encoding, DefaultEmbedding, size, style, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="size">the size of this font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, string encoding, float size) =>
        GetFont(fontname, encoding, DefaultEmbedding, size, Font.UNDEFINED, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, string encoding) =>
        GetFont(fontname, encoding, DefaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <param name="color">the Color of this font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, float size, int style, BaseColor color) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, size, style, color);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="color">the Color of this font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, float size, BaseColor color) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, size, Font.UNDEFINED, color);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="size">the size of this font</param>
    /// <param name="style">the style of this font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, float size, int style) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, size, style, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <param name="size">the size of this font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname, float size) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, size, Font.UNDEFINED, null);

    /// <summary>
    ///     Constructs a Font-object.
    /// </summary>
    /// <param name="fontname">the name of the font</param>
    /// <returns>a Font object</returns>
    public static Font GetFont(string fontname) =>
        GetFont(fontname, DefaultEncoding, DefaultEmbedding, Font.UNDEFINED, Font.UNDEFINED, null);

    /// <summary>
    ///     Checks if a certain font is registered.
    /// </summary>
    /// <param name="fontname">the name of the font that has to be checked</param>
    /// <returns>true if the font is found</returns>
    public static bool IsRegistered(string fontname) => FontFactoryImp.IsRegistered(fontname);

    public static void Register(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        string path;
        string alias = null;

        path = attributes.Remove("path");
        alias = attributes.Remove("alias");

        FontImp.Register(path, alias);
    }

    /// <summary>
    ///     Register a ttf- or a ttc-file.
    /// </summary>
    /// <param name="path">the path to a ttf- or ttc-file</param>
    public static void Register(string path)
    {
        Register(path, null);
    }

    /// <summary>
    ///     Register a ttf- or a ttc-file and use an alias for the font contained in the ttf-file.
    /// </summary>
    /// <param name="path">the path to a ttf- or ttc-file</param>
    /// <param name="alias">the alias you want to use for the font</param>
    public static void Register(string path, string alias)
    {
        FontImp.Register(path, alias);
    }

    /// <summary>
    ///     Register fonts in some probable directories. It usually works in Windows,
    ///     Linux and Solaris.
    /// </summary>
    /// <returns>the number of fonts registered</returns>
    public static int RegisterDirectories() => FontImp.RegisterDirectories();

    /// <summary>
    ///     Register all the fonts in a directory.
    /// </summary>
    /// <param name="dir">the directory</param>
    /// <returns>the number of fonts registered</returns>
    public static int RegisterDirectory(string dir) => FontImp.RegisterDirectory(dir);

    /// <summary>
    ///     Register all the fonts in a directory and possibly its subdirectories.
    ///     @since 2.1.2
    /// </summary>
    /// <param name="dir">the directory</param>
    /// <param name="scanSubdirectories">recursively scan subdirectories if  true</param>
    /// <returns>the number of fonts registered</returns>
    public static int RegisterDirectory(string dir, bool scanSubdirectories) =>
        FontImp.RegisterDirectory(dir, scanSubdirectories);

    /// <summary>
    ///     Register a font by giving explicitly the font family and name.
    /// </summary>
    /// <param name="familyName">the font family</param>
    /// <param name="fullName">the font name</param>
    /// <param name="path">the font path</param>
    public static void RegisterFamily(string familyName, string fullName, string path)
    {
        FontFactoryImp.RegisterFamily(familyName, fullName, path);
    }
}