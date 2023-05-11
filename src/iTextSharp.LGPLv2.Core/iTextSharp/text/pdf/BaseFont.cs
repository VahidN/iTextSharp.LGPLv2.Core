using System.Reflection;
using System.util;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf;

/// <summary>
///     Summary description for BaseFont.
/// </summary>
public abstract class BaseFont
{
    /// <summary>
    ///     The maximum height above the baseline reached by glyphs in this
    ///     font, excluding the height of glyphs for accented characters.
    /// </summary>
    public const int ASCENT = 1;

    /// <summary>
    ///     java.awt.Font property
    /// </summary>
    public const int AWT_ASCENT = 9;

    /// <summary>
    ///     java.awt.Font property
    /// </summary>
    public const int AWT_DESCENT = 10;

    /// <summary>
    ///     java.awt.Font property
    /// </summary>
    public const int AWT_LEADING = 11;

    /// <summary>
    ///     java.awt.Font property
    /// </summary>
    public const int AWT_MAXADVANCE = 12;

    /// <summary>
    ///     The lower left x glyph coordinate.
    /// </summary>
    public const int BBOXLLX = 5;

    /// <summary>
    ///     The lower left y glyph coordinate.
    /// </summary>
    public const int BBOXLLY = 6;

    /// <summary>
    ///     The upper right x glyph coordinate.
    /// </summary>
    public const int BBOXURX = 7;

    /// <summary>
    ///     The upper right y glyph coordinate.
    /// </summary>
    public const int BBOXURY = 8;

    /// <summary>
    ///     if the font has to be cached
    /// </summary>
    public const bool CACHED = true;

    /// <summary>
    ///     The y coordinate of the top of flat capital letters, measured from
    ///     the baseline.
    /// </summary>
    public const int CAPHEIGHT = 2;

    /// <summary>
    ///     The fake CID code that represents a newline.
    /// </summary>
    public const char CID_NEWLINE = '\u7fff';

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string COURIER = "Courier";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string COURIER_BOLD = "Courier-Bold";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string COURIER_BOLDOBLIQUE = "Courier-BoldOblique";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string COURIER_OBLIQUE = "Courier-Oblique";

    /// <summary>
    ///     A possible encoding.
    /// </summary>
    public const string CP1250 = "Cp1250";

    /// <summary>
    ///     A possible encoding.
    /// </summary>
    public const string CP1252 = "Cp1252";

    /// <summary>
    ///     A possible encoding.
    /// </summary>
    public const string CP1257 = "Cp1257";

    /// <summary>
    ///     The maximum depth below the baseline reached by glyphs in this
    ///     font. The value is a negative number.
    /// </summary>
    public const int DESCENT = 3;

    /// <summary>
    ///     if the font has to be embedded
    /// </summary>
    public const bool EMBEDDED = true;

    /// <summary>
    ///     The font is CJK.
    /// </summary>
    public const int FONT_TYPE_CJK = 2;

    /// <summary>
    ///     A font already inside the document.
    /// </summary>
    public const int FONT_TYPE_DOCUMENT = 4;

    /// <summary>
    ///     The font is Type 1.
    /// </summary>
    public const int FONT_TYPE_T1 = 0;

    /// <summary>
    ///     A Type3 font.
    /// </summary>
    public const int FONT_TYPE_T3 = 5;

    /// <summary>
    ///     The font is True Type with a standard encoding.
    /// </summary>
    public const int FONT_TYPE_TT = 1;

    /// <summary>
    ///     The font is True Type with a Unicode encoding.
    /// </summary>
    public const int FONT_TYPE_TTUNI = 3;

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string HELVETICA = "Helvetica";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string HELVETICA_BOLD = "Helvetica-Bold";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string HELVETICA_BOLDOBLIQUE = "Helvetica-BoldOblique";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string HELVETICA_OBLIQUE = "Helvetica-Oblique";

    /// <summary>
    ///     The Unicode encoding with horizontal writing.
    /// </summary>
    public const string IDENTITY_H = "Identity-H";

    /// <summary>
    ///     The Unicode encoding with vertical writing.
    /// </summary>
    public const string IDENTITY_V = "Identity-V";

    /// <summary>
    ///     The angle, expressed in degrees counterclockwise from the vertical,
    ///     of the dominant vertical strokes of the font. The value is
    ///     negative for fonts that slope to the right, as almost all italic fonts do.
    /// </summary>
    public const int ITALICANGLE = 4;

    /// <summary>
    ///     A possible encoding.
    /// </summary>
    public const string MACROMAN = "MacRoman";

    /// <summary>
    ///     if the font doesn't have to be cached
    /// </summary>
    public const bool NOT_CACHED = false;

    /// <summary>
    ///     if the font doesn't have to be embedded
    /// </summary>
    public const bool NOT_EMBEDDED = false;

    /// <summary>
    ///     a not defined character in a custom PDF encoding
    /// </summary>
    public const string notdef = ".notdef";

    /// <summary>
    ///     The path to the font resources.
    /// </summary>
    public const string RESOURCE_PATH = "iTextSharp.LGPLv2.Core.iTextSharp.text.pdf.fonts.";

    /// <summary>
    ///     The strikethrough position.
    /// </summary>
    public const int STRIKETHROUGH_POSITION = 15;

    /// <summary>
    ///     The strikethrough thickness.
    /// </summary>
    public const int STRIKETHROUGH_THICKNESS = 16;

    /// <summary>
    ///     The recommended vertical offset from the baseline for subscripts for this font. Usually a negative value.
    /// </summary>
    public const int SUBSCRIPT_OFFSET = 18;

    /// <summary>
    ///     The recommended vertical size for subscripts for this font.
    /// </summary>
    public const int SUBSCRIPT_SIZE = 17;

    /// <summary>
    ///     The recommended vertical offset from the baseline for superscripts for this font.
    /// </summary>
    public const int SUPERSCRIPT_OFFSET = 20;

    /// <summary>
    ///     The recommended vertical size for superscripts for this font.
    /// </summary>
    public const int SUPERSCRIPT_SIZE = 19;

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string SYMBOL = "Symbol";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string TIMES_BOLD = "Times-Bold";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string TIMES_BOLDITALIC = "Times-BoldItalic";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string TIMES_ITALIC = "Times-Italic";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string TIMES_ROMAN = "Times-Roman";

    /// <summary>
    ///     The underline position. Usually a negative value.
    /// </summary>
    public const int UNDERLINE_POSITION = 13;

    /// <summary>
    ///     The underline thickness.
    /// </summary>
    public const int UNDERLINE_THICKNESS = 14;

    /// <summary>
    ///     A possible encoding.
    /// </summary>
    public const string WINANSI = "Cp1252";

    /// <summary>
    ///     This is a possible value of a base 14 type 1 font
    /// </summary>
    public const string ZAPFDINGBATS = "ZapfDingbats";

    public static readonly int[] CharRangeArabic =
        { 0, 0x7f, 0x0600, 0x067f, 0x20a0, 0x20cf, 0xfb50, 0xfbff, 0xfe70, 0xfeff };

    public static readonly int[] CharRangeCyrillic = { 0, 0x7f, 0x0400, 0x052f, 0x2000, 0x206f, 0x20a0, 0x20cf };
    public static readonly int[] CharRangeHebrew = { 0, 0x7f, 0x0590, 0x05ff, 0x20a0, 0x20cf, 0xfb1d, 0xfb4f };
    public static readonly int[] CharRangeLatin = { 0, 0x17f, 0x2000, 0x206f, 0x20a0, 0x20cf, 0xfb00, 0xfb06 };

    protected internal static readonly List<object> ResourceSearch = new();

    /// <summary>
    ///     list of the 14 built in fonts.
    /// </summary>
    protected static readonly INullValueDictionary<string, PdfName> BuiltinFonts14 =
        new NullValueDictionary<string, PdfName>();

    /// <summary>
    ///     cache for the fonts already used.
    /// </summary>
    protected static readonly INullValueDictionary<string, BaseFont> FontCache =
        new NullValueDictionary<string, BaseFont>();

    private static readonly Random _random = new();

    protected int[][] CharBBoxes = new int[256][];

    /// <summary>
    ///     The compression level for the font stream.
    ///     @since   2.1.3
    /// </summary>
    protected int compressionLevel = PdfStream.DEFAULT_COMPRESSION;

    /// <summary>
    ///     encoding names
    /// </summary>
    protected string[] differences = new string[256];

    /// <summary>
    ///     Converts  char  directly to  byte
    ///     by casting.
    /// </summary>
    protected bool directTextToByte;

    /// <summary>
    ///     true if the font is to be embedded in the PDF
    /// </summary>
    protected bool Embedded;

    /// <summary>
    ///     encoding used with this font
    /// </summary>
    protected string encoding;

    protected bool FastWinansi;

    /// <summary>
    ///     true if the font must use its built in encoding. In that case the
    ///     encoding  is only used to map a char to the position inside
    ///     the font, not to the expected char name.
    /// </summary>
    protected bool FontSpecific = true;

    /// <summary>
    ///     The font type.
    /// </summary>
    internal int fontType;

    /// <summary>
    ///     Forces the output of the width array. Only matters for the 14
    ///     built-in fonts.
    /// </summary>
    protected bool forceWidthsOutput;

    /// <summary>
    ///     Custom encodings use this map to key the Unicode character
    ///     to the single byte code.
    /// </summary>
    protected NullValueDictionary<int, int> SpecialMap;

    /// <summary>
    ///     Indicates if all the glyphs and widths for that particular
    ///     encoding should be included in the document.
    /// </summary>
    protected bool subset = true;

    protected List<int[]> SubsetRanges;

    /// <summary>
    ///     same as differences but with the unicode codes
    /// </summary>
    protected char[] unicodeDifferences = new char[256];

    /// <summary>
    ///     table of characters widths for this encoding
    /// </summary>
    protected int[] widths = new int[256];

    static BaseFont()
    {
        BuiltinFonts14.Add(COURIER, PdfName.Courier);
        BuiltinFonts14.Add(COURIER_BOLD, PdfName.CourierBold);
        BuiltinFonts14.Add(COURIER_BOLDOBLIQUE, PdfName.CourierBoldoblique);
        BuiltinFonts14.Add(COURIER_OBLIQUE, PdfName.CourierOblique);
        BuiltinFonts14.Add(HELVETICA, PdfName.Helvetica);
        BuiltinFonts14.Add(HELVETICA_BOLD, PdfName.HelveticaBold);
        BuiltinFonts14.Add(HELVETICA_BOLDOBLIQUE, PdfName.HelveticaBoldoblique);
        BuiltinFonts14.Add(HELVETICA_OBLIQUE, PdfName.HelveticaOblique);
        BuiltinFonts14.Add(SYMBOL, PdfName.Symbol);
        BuiltinFonts14.Add(TIMES_ROMAN, PdfName.TimesRoman);
        BuiltinFonts14.Add(TIMES_BOLD, PdfName.TimesBold);
        BuiltinFonts14.Add(TIMES_BOLDITALIC, PdfName.TimesBolditalic);
        BuiltinFonts14.Add(TIMES_ITALIC, PdfName.TimesItalic);
        BuiltinFonts14.Add(ZAPFDINGBATS, PdfName.Zapfdingbats);
    }

    /// <summary>
    ///     Gets all the entries of the names-table. If it is a True Type font
    ///     each array element will have {Name ID, Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"4", "", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the full name of the font</returns>
    public abstract string[][] AllNameEntries { get; }

    /// <summary>
    ///     Gets the code pages supported by the font. This has only meaning
    ///     with True Type fonts.
    /// </summary>
    /// <returns>the code pages supported by the font</returns>
    public virtual string[] CodePagesSupported => Array.Empty<string>();

    /// <summary>
    ///     Sets the compression level to be used for the font streams.
    ///     @since 2.1.3
    /// </summary>
    public int CompressionLevel
    {
        set
        {
            if (value < PdfStream.NO_COMPRESSION || value > PdfStream.BEST_COMPRESSION)
            {
                compressionLevel = PdfStream.DEFAULT_COMPRESSION;
            }
            else
            {
                compressionLevel = value;
            }
        }
        get => compressionLevel;
    }

    /// <summary>
    ///     Gets the array with the names of the characters.
    /// </summary>
    /// <returns>the array with the names of the characters</returns>
    public string[] Differences => differences;

    /// <summary>
    ///     Sets the conversion of  char  directly to  byte
    ///     by casting. This is a low level feature to put the bytes directly in
    ///     the content stream without passing through string.GetBytes().
    /// </summary>
    public bool DirectTextToByte
    {
        set => directTextToByte = value;
        get => directTextToByte;
    }

    /// <summary>
    ///     Gets the encoding used to convert  string  into  byte[] .
    /// </summary>
    /// <returns>the encoding name</returns>
    public string Encoding => encoding;

    /// <summary>
    ///     Gets the family name of the font. If it is a True Type font
    ///     each array element will have {Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the family name of the font</returns>
    public abstract string[][] FamilyFontName { get; }

    /// <summary>
    ///     Gets the font type. The font types can be: FONT_TYPE_T1,
    ///     FONT_TYPE_TT, FONT_TYPE_CJK and FONT_TYPE_TTUNI.
    /// </summary>
    /// <returns>the font type</returns>
    public int FontType
    {
        get => fontType;

        set => fontType = value;
    }

    /// <summary>
    ///     Set to  true  to force the generation of the
    ///     widths array.
    ///     widths array
    /// </summary>
    public bool ForceWidthsOutput
    {
        set => forceWidthsOutput = value;
        get => forceWidthsOutput;
    }

    /// <summary>
    ///     Gets the full name of the font. If it is a True Type font
    ///     each array element will have {Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the full name of the font</returns>
    public abstract string[][] FullFontName { get; }

    /// <summary>
    ///     Gets the postscript font name.
    /// </summary>
    /// <returns>the postscript font name</returns>
    public abstract string PostscriptFontName { get; set; }

    /// <summary>
    ///     Indicates if all the glyphs and widths for that particular
    ///     encoding should be included in the document. When set to  true
    ///     only the glyphs used will be included in the font. When set to  false
    ///     and {@link #addSubsetRange(int[])} was not called the full font will be included
    ///     otherwise just the characters ranges will be included.
    /// </summary>
    public bool Subset
    {
        set => subset = value;
        get => subset;
    }

    /// <summary>
    ///     Gets the array with the unicode characters.
    /// </summary>
    /// <returns>the array with the unicode characters</returns>
    public char[] UnicodeDifferences => unicodeDifferences;

    /// <summary>
    ///     Gets the font width array.
    /// </summary>
    /// <returns>the font width array</returns>
    public int[] Widths => widths;

    public static void AddToResourceSearch(object obj)
    {
        if (obj is Assembly)
        {
            ResourceSearch.Add(obj);
        }
        else if (obj is string)
        {
            var f = (string)obj;
            if (Directory.Exists(f) || File.Exists(f))
            {
                ResourceSearch.Add(obj);
            }
        }
    }

    /// <summary>
    ///     Creates a new font. This will always be the default Helvetica font (not embedded).
    ///     This method is introduced because Helvetica is used in many examples.
    ///     @throws  IOException         This shouldn't occur ever
    ///     @throws  DocumentException   This shouldn't occur ever
    ///     @since   2.1.1
    /// </summary>
    /// <returns>a BaseFont object (Helvetica, Winansi, not embedded)</returns>
    public static BaseFont CreateFont() => CreateFont(HELVETICA, WINANSI, NOT_EMBEDDED);

    /// <summary>
    ///     Creates a new font. This font can be one of the 14 built in types,
    ///     a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
    ///     Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
    ///     appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
    ///     example would be "STSong-Light,Bold". Note that this modifiers do not work if
    ///     the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
    ///     This would get the second font (indexes start at 0), in this case "MS PGothic".
    ///     The fonts are cached and if they already exist they are extracted from the cache,
    ///     not parsed again.
    ///     Besides the common encodings described by name, custom encodings
    ///     can also be made. These encodings will only work for the single byte fonts
    ///     Type1 and TrueType. The encoding string starts with a '#'
    ///     followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
    ///     of hex values representing the Unicode codes that compose that encoding.
    ///     The "simple" encoding is recommended for TrueType fonts
    ///     as the "full" encoding risks not matching the character with the right glyph
    ///     if not done with care.
    ///     The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
    ///     described by non standard names like the Tex math fonts. Each group of three elements
    ///     compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the
    ///     Unicode character
    ///     used to access the glyph. The space must be assigned to character position 32 otherwise
    ///     text justification will not work.
    ///     Example for a "simple" encoding that includes the Unicode
    ///     character space, A, B and ecyrillic:
    ///     "# simple 32 0020 0041 0042 0454"
    ///     Example for a "full" encoding for a Type1 Tex font:
    ///     "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
    ///     This method calls:
    ///     createFont(name, encoding, embedded, true, null, null);
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <param name="name">the name of the font or its location on file</param>
    /// <param name="encoding">the encoding to be applied to this font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <returns>returns a new font. This font may come from the cache</returns>
    public static BaseFont CreateFont(string name, string encoding, bool embedded) =>
        CreateFont(name, encoding, embedded, true, null, null, false);

    /// <summary>
    ///     Creates a new font. This font can be one of the 14 built in types,
    ///     a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
    ///     Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
    ///     appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
    ///     example would be "STSong-Light,Bold". Note that this modifiers do not work if
    ///     the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
    ///     This would get the second font (indexes start at 0), in this case "MS PGothic".
    ///     The fonts are cached and if they already exist they are extracted from the cache,
    ///     not parsed again.
    ///     Besides the common encodings described by name, custom encodings
    ///     can also be made. These encodings will only work for the single byte fonts
    ///     Type1 and TrueType. The encoding string starts with a '#'
    ///     followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
    ///     of hex values representing the Unicode codes that compose that encoding.
    ///     The "simple" encoding is recommended for TrueType fonts
    ///     as the "full" encoding risks not matching the character with the right glyph
    ///     if not done with care.
    ///     The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
    ///     described by non standard names like the Tex math fonts. Each group of three elements
    ///     compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the
    ///     Unicode character
    ///     used to access the glyph. The space must be assigned to character position 32 otherwise
    ///     text justification will not work.
    ///     Example for a "simple" encoding that includes the Unicode
    ///     character space, A, B and ecyrillic:
    ///     "# simple 32 0020 0041 0042 0454"
    ///     Example for a "full" encoding for a Type1 Tex font:
    ///     "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
    ///     This method calls:
    ///     createFont(name, encoding, embedded, true, null, null);
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    ///     @since   2.1.5
    /// </summary>
    /// <param name="name">the name of the font or its location on file</param>
    /// <param name="encoding">the encoding to be applied to this font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <param name="forceRead">
    ///     in some cases (TrueTypeFont, Type1Font), the full font file will be read and kept in memory if
    ///     forceRead is true
    /// </param>
    /// <returns>returns a new font. This font may come from the cache</returns>
    public static BaseFont CreateFont(string name, string encoding, bool embedded, bool forceRead) =>
        CreateFont(name, encoding, embedded, true, null, null, forceRead);

    /// <summary>
    ///     Creates a new font. This font can be one of the 14 built in types,
    ///     a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
    ///     Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
    ///     appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
    ///     example would be "STSong-Light,Bold". Note that this modifiers do not work if
    ///     the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
    ///     This would get the second font (indexes start at 0), in this case "MS PGothic".
    ///     The fonts may or may not be cached depending on the flag  cached .
    ///     If the  byte  arrays are present the font will be
    ///     read from them instead of the name. A name is still required to identify
    ///     the font type.
    ///     Besides the common encodings described by name, custom encodings
    ///     can also be made. These encodings will only work for the single byte fonts
    ///     Type1 and TrueType. The encoding string starts with a '#'
    ///     followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
    ///     of hex values representing the Unicode codes that compose that encoding.
    ///     The "simple" encoding is recommended for TrueType fonts
    ///     as the "full" encoding risks not matching the character with the right glyph
    ///     if not done with care.
    ///     The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
    ///     described by non standard names like the Tex math fonts. Each group of three elements
    ///     compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the
    ///     Unicode character
    ///     used to access the glyph. The space must be assigned to character position 32 otherwise
    ///     text justification will not work.
    ///     Example for a "simple" encoding that includes the Unicode
    ///     character space, A, B and ecyrillic:
    ///     "# simple 32 0020 0041 0042 0454"
    ///     Example for a "full" encoding for a Type1 Tex font:
    ///     "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
    ///     the cache if new, false if the font is always created new
    ///     is true, otherwise it will always be created new
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    ///     @since   iText 0.80
    /// </summary>
    /// <param name="name">the name of the font or its location on file</param>
    /// <param name="encoding">the encoding to be applied to this font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <param name="cached">true if the font comes from the cache or is added to</param>
    /// <param name="ttfAfm">the true type font or the afm in a byte array</param>
    /// <param name="pfb">the pfb in a byte array</param>
    /// <returns>returns a new font. This font may come from the cache but only if cached</returns>
    public static BaseFont CreateFont(string name, string encoding, bool embedded, bool cached, byte[] ttfAfm,
                                      byte[] pfb) =>
        CreateFont(name, encoding, embedded, cached, ttfAfm, pfb, false);

    /// <summary>
    ///     Creates a new font. This font can be one of the 14 built in types,
    ///     a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
    ///     Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
    ///     appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
    ///     example would be "STSong-Light,Bold". Note that this modifiers do not work if
    ///     the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
    ///     This would get the second font (indexes start at 0), in this case "MS PGothic".
    ///     The fonts may or may not be cached depending on the flag  cached .
    ///     If the  byte  arrays are present the font will be
    ///     read from them instead of the name. A name is still required to identify
    ///     the font type.
    ///     Besides the common encodings described by name, custom encodings
    ///     can also be made. These encodings will only work for the single byte fonts
    ///     Type1 and TrueType. The encoding string starts with a '#'
    ///     followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
    ///     of hex values representing the Unicode codes that compose that encoding.
    ///     The "simple" encoding is recommended for TrueType fonts
    ///     as the "full" encoding risks not matching the character with the right glyph
    ///     if not done with care.
    ///     The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
    ///     described by non standard names like the Tex math fonts. Each group of three elements
    ///     compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the
    ///     Unicode character
    ///     used to access the glyph. The space must be assigned to character position 32 otherwise
    ///     text justification will not work.
    ///     Example for a "simple" encoding that includes the Unicode
    ///     character space, A, B and ecyrillic:
    ///     "# simple 32 0020 0041 0042 0454"
    ///     Example for a "full" encoding for a Type1 Tex font:
    ///     "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
    ///     the cache if new, false if the font is always created new
    ///     an exception if the font is not recognized. Note that even if true an exception may be thrown in some
    ///     circumstances.
    ///     This parameter is useful for FontFactory that may have to check many invalid font names before finding the right
    ///     one
    ///     is true, otherwise it will always be created new
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    ///     @since   2.0.3
    /// </summary>
    /// <param name="name">the name of the font or its location on file</param>
    /// <param name="encoding">the encoding to be applied to this font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <param name="cached">true if the font comes from the cache or is added to</param>
    /// <param name="ttfAfm">the true type font or the afm in a byte array</param>
    /// <param name="pfb">the pfb in a byte array</param>
    /// <param name="noThrow">
    ///     if true will not throw an exception if the font is not recognized and will return null, if false
    ///     will throw
    /// </param>
    /// <returns>returns a new font. This font may come from the cache but only if cached</returns>
    public static BaseFont CreateFont(string name, string encoding, bool embedded, bool cached, byte[] ttfAfm,
                                      byte[] pfb, bool noThrow) =>
        CreateFont(name, encoding, embedded, cached, ttfAfm, pfb, false, false);

    /// <summary>
    ///     Creates a new font. This font can be one of the 14 built in types,
    ///     a Type1 font referred to by an AFM or PFM file, a TrueType font (simple or collection) or a CJK font from the
    ///     Adobe Asian Font Pack. TrueType fonts and CJK fonts can have an optional style modifier
    ///     appended to the name. These modifiers are: Bold, Italic and BoldItalic. An
    ///     example would be "STSong-Light,Bold". Note that this modifiers do not work if
    ///     the font is embedded. Fonts in TrueType collections are addressed by index such as "msgothic.ttc,1".
    ///     This would get the second font (indexes start at 0), in this case "MS PGothic".
    ///     The fonts may or may not be cached depending on the flag  cached .
    ///     If the  byte  arrays are present the font will be
    ///     read from them instead of the name. A name is still required to identify
    ///     the font type.
    ///     Besides the common encodings described by name, custom encodings
    ///     can also be made. These encodings will only work for the single byte fonts
    ///     Type1 and TrueType. The encoding string starts with a '#'
    ///     followed by "simple" or "full". If "simple" there is a decimal for the first character position and then a list
    ///     of hex values representing the Unicode codes that compose that encoding.
    ///     The "simple" encoding is recommended for TrueType fonts
    ///     as the "full" encoding risks not matching the character with the right glyph
    ///     if not done with care.
    ///     The "full" encoding is specially aimed at Type1 fonts where the glyphs have to be
    ///     described by non standard names like the Tex math fonts. Each group of three elements
    ///     compose a code position: the one byte code order in decimal or as 'x' (x cannot be the space), the name and the
    ///     Unicode character
    ///     used to access the glyph. The space must be assigned to character position 32 otherwise
    ///     text justification will not work.
    ///     Example for a "simple" encoding that includes the Unicode
    ///     character space, A, B and ecyrillic:
    ///     "# simple 32 0020 0041 0042 0454"
    ///     Example for a "full" encoding for a Type1 Tex font:
    ///     "# full 'A' nottriangeqlleft 0041 'B' dividemultiply 0042 32 space 0020"
    ///     the cache if new, false if the font is always created new
    ///     an exception if the font is not recognized. Note that even if true an exception may be thrown in some
    ///     circumstances.
    ///     This parameter is useful for FontFactory that may have to check many invalid font names before finding the right
    ///     one
    ///     is true, otherwise it will always be created new
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    ///     @since   2.1.5
    /// </summary>
    /// <param name="name">the name of the font or its location on file</param>
    /// <param name="encoding">the encoding to be applied to this font</param>
    /// <param name="embedded">true if the font is to be embedded in the PDF</param>
    /// <param name="cached">true if the font comes from the cache or is added to</param>
    /// <param name="ttfAfm">the true type font or the afm in a byte array</param>
    /// <param name="pfb">the pfb in a byte array</param>
    /// <param name="noThrow">
    ///     if true will not throw an exception if the font is not recognized and will return null, if false
    ///     will throw
    /// </param>
    /// <param name="forceRead">
    ///     in some cases (TrueTypeFont, Type1Font), the full font file will be read and kept in memory if
    ///     forceRead is true
    /// </param>
    /// <returns>returns a new font. This font may come from the cache but only if cached</returns>
    public static BaseFont CreateFont(string name, string encoding, bool embedded, bool cached, byte[] ttfAfm,
                                      byte[] pfb, bool noThrow, bool forceRead)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var nameBase = GetBaseName(name);
        encoding = NormalizeEncoding(encoding);
        var isBuiltinFonts14 = BuiltinFonts14.ContainsKey(name);
        var isCjkFont = isBuiltinFonts14 ? false : CjkFont.IsCjkFont(nameBase, encoding);
        if (isBuiltinFonts14 || isCjkFont)
        {
            embedded = false;
        }
        else if (encoding.Equals(IDENTITY_H, StringComparison.Ordinal) ||
                 encoding.Equals(IDENTITY_V, StringComparison.Ordinal))
        {
            embedded = true;
        }

        BaseFont fontFound;
        BaseFont fontBuilt;
        var key = $"{name}\n{encoding}\n{embedded}";
        if (cached)
        {
            lock (FontCache)
            {
                fontFound = FontCache[key];
            }

            if (fontFound != null)
            {
                return fontFound;
            }
        }

        if (isBuiltinFonts14 || name.EndsWith(".afm", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith(".pfm", StringComparison.OrdinalIgnoreCase))
        {
            fontBuilt = new Type1Font(name, encoding, embedded, ttfAfm, pfb, forceRead);
            fontBuilt.FastWinansi = encoding.Equals(CP1252, StringComparison.Ordinal);
        }
        else if (nameBase.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
                 nameBase.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
                 nameBase.IndexOf(".ttc,", StringComparison.OrdinalIgnoreCase) > 0)
        {
            if (encoding.Equals(IDENTITY_H, StringComparison.Ordinal) ||
                encoding.Equals(IDENTITY_V, StringComparison.Ordinal))
            {
                fontBuilt = new TrueTypeFontUnicode(name, encoding, embedded, ttfAfm, forceRead);
            }
            else
            {
                fontBuilt = new TrueTypeFont(name, encoding, embedded, ttfAfm, false, forceRead);
                fontBuilt.FastWinansi = encoding.Equals(CP1252, StringComparison.Ordinal);
            }
        }
        else if (isCjkFont)
        {
            fontBuilt = new CjkFont(name, encoding, embedded);
        }
        else if (noThrow)
        {
            return null;
        }
        else
        {
            throw new DocumentException($"Font \'{name}\' with \'{encoding}\' is not recognized.");
        }

        if (cached)
        {
            lock (FontCache)
            {
                fontFound = FontCache[key];
                if (fontFound != null)
                {
                    return fontFound;
                }

                FontCache.Add(key, fontBuilt);

                var keyNormalized = $"{fontBuilt.PostscriptFontName}\n{encoding}\n{embedded}";
                if (!FontCache.ContainsKey(keyNormalized))
                {
                    FontCache.Add(keyNormalized, fontBuilt);

                    var keyNormalizedToLower =
                        $"{fontBuilt.PostscriptFontName.ToLowerInvariant()}\n{encoding}\n{embedded}";
                    if (!FontCache.ContainsKey(keyNormalizedToLower))
                    {
                        FontCache.Add(keyNormalizedToLower, fontBuilt);
                    }
                }
            }
        }

        return fontBuilt;
    }

    /// <summary>
    ///     Creates a font based on an existing document font. The created font font may not
    ///     behave as expected, depending on the encoding or subset.
    /// </summary>
    /// <param name="fontRef">the reference to the document font</param>
    /// <returns>the font</returns>
    public static BaseFont CreateFont(PrIndirectReference fontRef) => new DocumentFont(fontRef);

    /// <summary>
    ///     Enumerates the postscript font names present inside a
    ///     True Type Collection.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="ttcFile">the file name of the font</param>
    /// <returns>the postscript font names</returns>
    public static string[] EnumerateTtcNames(string ttcFile) => new EnumerateTtc(ttcFile).Names;

    /// <summary>
    ///     Enumerates the postscript font names present inside a
    ///     True Type Collection.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="ttcArray">the font as a  byte  array</param>
    /// <returns>the postscript font names</returns>
    public static string[] EnumerateTtcNames(byte[] ttcArray) => new EnumerateTtc(ttcArray).Names;

    /// <summary>
    ///     Gets all the names from the font. Only the required tables are read.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="name">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="ttfAfm">the true type font or the afm in a byte array</param>
    /// <returns>an array of Object[] built with {getPostscriptFontName(), GetFamilyFontName(), GetFullFontName()}</returns>
    public static object[] GetAllFontNames(string name, string encoding, byte[] ttfAfm)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var nameBase = GetBaseName(name);
        BaseFont fontBuilt = null;
        if (nameBase.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
            nameBase.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
            nameBase.IndexOf(".ttc,", StringComparison.OrdinalIgnoreCase) > 0)
        {
            fontBuilt = new TrueTypeFont(name, CP1252, false, ttfAfm, true, false);
        }
        else
        {
            fontBuilt = CreateFont(name, encoding, false, false, ttfAfm, null);
        }

        return new object[] { fontBuilt.PostscriptFontName, fontBuilt.FamilyFontName, fontBuilt.FullFontName };
    }

    /// <summary>
    ///     Gets all the entries of the namestable from the font. Only the required tables are read.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="name">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="ttfAfm">the true type font or the afm in a byte array</param>
    /// <returns>an array of Object[] built with {getPostscriptFontName(), getFamilyFontName(), getFullFontName()}</returns>
    public static string[][] GetAllNameEntries(string name, string encoding, byte[] ttfAfm)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var nameBase = GetBaseName(name);
        BaseFont fontBuilt = null;
        if (nameBase.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
            nameBase.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
            nameBase.IndexOf(".ttc,", StringComparison.OrdinalIgnoreCase) > 0)
        {
            fontBuilt = new TrueTypeFont(name, CP1252, false, ttfAfm, true, false);
        }
        else
        {
            fontBuilt = CreateFont(name, encoding, false, false, ttfAfm, null);
        }

        return fontBuilt.AllNameEntries;
    }

    /// <summary>
    ///     Gets a list of all document fonts. Each element of the  ArrayList
    ///     contains a  Object[]{String,PRIndirectReference}  with the font name
    ///     and the indirect reference to it.
    /// </summary>
    /// <param name="reader">the document where the fonts are to be listed from</param>
    /// <returns>the list of fonts and references</returns>
    public static List<object[]> GetDocumentFonts(PdfReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var hits = new NullValueDictionary<int, int>();
        List<object[]> fonts = new();
        var npages = reader.NumberOfPages;
        for (var k = 1; k <= npages; ++k)
        {
            recourseFonts(reader.GetPageN(k), hits, fonts, 1);
        }

        return fonts;
    }

    /// <summary>
    ///     Gets a list of the document fonts in a particular page. Each element of the  ArrayList
    ///     contains a  Object[]{String,PRIndirectReference}  with the font name
    ///     and the indirect reference to it.
    /// </summary>
    /// <param name="reader">the document where the fonts are to be listed from</param>
    /// <param name="page">the page to list the fonts from</param>
    /// <returns>the list of fonts and references</returns>
    public static List<object[]> GetDocumentFonts(PdfReader reader, int page)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var hits = new NullValueDictionary<int, int>();
        List<object[]> fonts = new();
        recourseFonts(reader.GetPageN(page), hits, fonts, 1);
        return fonts;
    }

    /// <summary>
    ///     Gets the full name of the font. If it is a True Type font
    ///     each array element will have {Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="name">the name of the font</param>
    /// <param name="encoding">the encoding of the font</param>
    /// <param name="ttfAfm">the true type font or the afm in a byte array</param>
    /// <returns>the full name of the font</returns>
    public static string[][] GetFullFontName(string name, string encoding, byte[] ttfAfm)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var nameBase = GetBaseName(name);
        BaseFont fontBuilt = null;
        if (nameBase.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
            nameBase.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
            nameBase.IndexOf(".ttc,", StringComparison.OrdinalIgnoreCase) > 0)
        {
            fontBuilt = new TrueTypeFont(name, CP1252, false, ttfAfm, true, false);
        }
        else
        {
            fontBuilt = CreateFont(name, encoding, false, false, ttfAfm, null);
        }

        return fontBuilt.FullFontName;
    }

    /// <summary>
    ///     Gets the font resources.
    ///     null  if not found
    /// </summary>
    /// <param name="key">the name of the resource</param>
    /// <returns>the  Stream  to get the resource or</returns>
    public static Stream GetResourceStream(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        Stream istr = null;
        // Try to use resource loader to load the properties file.
        try
        {
            var assembly = typeof(BaseFont).GetTypeInfo().Assembly;
            istr = assembly.GetManifestResourceStream(key);
        }
        catch
        {
        }

        if (istr != null)
        {
            return istr;
        }

        for (var k = 0; k < ResourceSearch.Count; ++k)
        {
            var obj = ResourceSearch[k];
            try
            {
                if (obj is Assembly)
                {
                    istr = ((Assembly)obj).GetManifestResourceStream(key);
                    if (istr != null)
                    {
                        return istr;
                    }
                }
                else if (obj is string)
                {
                    var dir = (string)obj;
                    try
                    {
                        var asm = Assembly.LoadFrom(dir);
                        istr = asm.GetManifestResourceStream(key);
                    }
                    catch
                    {
                    }

                    if (istr != null)
                    {
                        return istr;
                    }

                    var modkey = key.Replace('.', '/');
                    var fullPath = Path.Combine(dir, modkey);
                    if (File.Exists(fullPath))
                    {
                        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }

                    var idx = modkey.LastIndexOf("/", StringComparison.Ordinal);
                    if (idx >= 0)
                    {
                        modkey = modkey.Substring(0, idx) + "." + modkey.Substring(idx + 1);
                        fullPath = Path.Combine(dir, modkey);
                        if (File.Exists(fullPath))
                        {
                            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        return istr;
    }

    /// <summary>
    ///     Adds a character range when subsetting. The range is an  int  array
    ///     where the first element is the start range inclusive and the second element is the
    ///     end range inclusive. Several ranges are allowed in the same array.
    /// </summary>
    /// <param name="range">the character range</param>
    public void AddSubsetRange(int[] range)
    {
        if (SubsetRanges == null)
        {
            SubsetRanges = new List<int[]>();
        }

        SubsetRanges.Add(range);
    }

    /// <summary>
    ///     Checks if a character exists in this font.
    ///     false  otherwise
    /// </summary>
    /// <param name="c">the character to check</param>
    /// <returns> true  if the character has a glyph,</returns>
    public virtual bool CharExists(int c)
    {
        var b = ConvertToBytes(c);
        return b.Length > 0;
    }

    /// <summary>
    ///     iText expects Arabic Diactrics (tashkeel) to have zero advance but some fonts,
    ///     most notably those that come with Windows, like times.ttf, have non-zero
    ///     advance for those characters. This method makes those character to have zero
    ///     width advance and work correctly in the iText Arabic shaping and reordering
    ///     context.
    /// </summary>
    public void CorrectArabicAdvance()
    {
        for (var c = '\u064b'; c <= '\u0658'; ++c)
        {
            SetCharAdvance(c, 0);
        }

        SetCharAdvance('\u0670', 0);
        for (var c = '\u06d6'; c <= '\u06dc'; ++c)
        {
            SetCharAdvance(c, 0);
        }

        for (var c = '\u06df'; c <= '\u06e4'; ++c)
        {
            SetCharAdvance(c, 0);
        }

        for (var c = '\u06e7'; c <= '\u06e8'; ++c)
        {
            SetCharAdvance(c, 0);
        }

        for (var c = '\u06ea'; c <= '\u06ed'; ++c)
        {
            SetCharAdvance(c, 0);
        }
    }

    /// <summary>
    ///     Gets the ascent of a  String  in normalized 1000 units. The ascent will always be
    ///     greater than or equal to zero even if all the characters have a lower ascent.
    /// </summary>
    /// <param name="text">the  String  to get the ascent of</param>
    /// <returns>the ascent in normalized 1000 units</returns>
    public int GetAscent(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var max = 0;
        var chars = text.ToCharArray();
        for (var k = 0; k < chars.Length; ++k)
        {
            var bbox = GetCharBBox(chars[k]);
            if (bbox != null && bbox[3] > max)
            {
                max = bbox[3];
            }
        }

        return max;
    }

    /// <summary>
    ///     Gets the ascent of a  String  in points. The ascent will always be
    ///     greater than or equal to zero even if all the characters have a lower ascent.
    /// </summary>
    /// <param name="text">the  String  to get the ascent of</param>
    /// <param name="fontSize">the size of the font</param>
    /// <returns>the ascent in points</returns>
    public float GetAscentPoint(string text, float fontSize) => GetAscent(text) * 0.001f * fontSize;

    /// <summary>
    ///     Gets the smallest box enclosing the character contours. It will return
    ///     null  if the font has not the information or the character has no
    ///     contours, as in the case of the space, for example. Characters with no contours may
    ///     also return [0,0,0,0].
    ///     null
    /// </summary>
    /// <param name="c">the character to get the contour bounding box from</param>
    /// <returns>an array of four floats with the bounding box in the format [llx,lly,urx,ury] or</returns>
    public virtual int[] GetCharBBox(int c)
    {
        var b = ConvertToBytes(c);
        if (b.Length == 0)
        {
            return null;
        }

        return CharBBoxes[b[0] & 0xff];
    }

    /// <summary>
    ///     Gets the CID code given an Unicode.
    ///     It has only meaning with CJK fonts.
    /// </summary>
    /// <param name="c">the Unicode</param>
    /// <returns>the CID equivalent</returns>
    public virtual int GetCidCode(int c) => c;

    /// <summary>
    ///     Gets the descent of a  String  in normalized 1000 units. The descent will always be
    ///     less than or equal to zero even if all the characters have an higher descent.
    /// </summary>
    /// <param name="text">the  String  to get the descent of</param>
    /// <returns>the dexcent in normalized 1000 units</returns>
    public int GetDescent(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var min = 0;
        var chars = text.ToCharArray();
        for (var k = 0; k < chars.Length; ++k)
        {
            var bbox = GetCharBBox(chars[k]);
            if (bbox != null && bbox[1] < min)
            {
                min = bbox[1];
            }
        }

        return min;
    }

    /// <summary>
    ///     Gets the descent of a  String  in points. The descent will always be
    ///     less than or equal to zero even if all the characters have an higher descent.
    /// </summary>
    /// <param name="text">the  String  to get the descent of</param>
    /// <param name="fontSize">the size of the font</param>
    /// <returns>the dexcent in points</returns>
    public float GetDescentPoint(string text, float fontSize) => GetDescent(text) * 0.001f * fontSize;

    /// <summary>
    ///     Gets the font parameter identified by  key . Valid values
    ///     for  key  are  ASCENT ,  CAPHEIGHT ,  DESCENT ,
    ///     ITALICANGLE ,  BBOXLLX ,  BBOXLLY ,  BBOXURX
    ///     and  BBOXURY .
    /// </summary>
    /// <param name="key">the parameter to be extracted</param>
    /// <param name="fontSize">the font size in points</param>
    /// <returns>the parameter in points</returns>
    public abstract float GetFontDescriptor(int key, float fontSize);

    /// <summary>
    ///     Returns a PdfStream object with the full font program (if possible).
    ///     This method will return null for some types of fonts (CJKFont, Type3Font)
    ///     or if there is no font program available (standard Type 1 fonts).
    ///     @since   2.1.3
    /// </summary>
    /// <returns>a PdfStream with the font program</returns>
    public abstract PdfStream GetFullFontStream();

    /// <summary>
    ///     Gets the kerning between two Unicode chars.
    /// </summary>
    /// <param name="char1">the first char</param>
    /// <param name="char2">the second char</param>
    /// <returns>the kerning to be applied</returns>
    public abstract int GetKerning(int char1, int char2);

    /// <summary>
    ///     Gets the Unicode equivalent to a CID.
    ///     The (inexistent) CID FF00 is translated as '\n'.
    ///     It has only meaning with CJK fonts with Identity encoding.
    /// </summary>
    /// <param name="c">the CID code</param>
    /// <returns>the Unicode equivalent</returns>
    public virtual int GetUnicodeEquivalent(int c) => c;

    /// <summary>
    ///     Gets the width of a  char  in normalized 1000 units.
    /// </summary>
    /// <param name="char1">the unicode  char  to get the width of</param>
    /// <returns>the width in normalized 1000 units</returns>
    public virtual int GetWidth(int char1)
    {
        if (FastWinansi)
        {
            if (char1 < 128 || (char1 >= 160 && char1 <= 255))
            {
                return widths[char1];
            }

            return widths[PdfEncodings.Winansi[char1]];
        }

        var total = 0;
        var mbytes = ConvertToBytes((char)char1);
        for (var k = 0; k < mbytes.Length; ++k)
        {
            total += widths[0xff & mbytes[k]];
        }

        return total;
    }

    /// <summary>
    ///     Gets the width of a  string  in normalized 1000 units.
    /// </summary>
    /// <param name="text">the  string  to get the witdth of</param>
    /// <returns>the width in normalized 1000 units</returns>
    public virtual int GetWidth(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var total = 0;
        if (FastWinansi)
        {
            var len = text.Length;
            for (var k = 0; k < len; ++k)
            {
                var char1 = text[k];
                if (char1 < 128 || (char1 >= 160 && char1 <= 255))
                {
                    total += widths[char1];
                }
                else
                {
                    total += widths[PdfEncodings.Winansi[char1]];
                }
            }

            return total;
        }

        var mbytes = ConvertToBytes(text);
        for (var k = 0; k < mbytes.Length; ++k)
        {
            total += widths[0xff & mbytes[k]];
        }

        return total;
    }

    /// <summary>
    ///     Gets the width of a  string  in points.
    /// </summary>
    /// <param name="text">the  string  to get the witdth of</param>
    /// <param name="fontSize">the font size</param>
    /// <returns>the width in points</returns>
    public float GetWidthPoint(string text, float fontSize) => GetWidth(text) * 0.001f * fontSize;

    /// <summary>
    ///     Gets the width of a  char  in points.
    /// </summary>
    /// <param name="char1">the  char  to get the witdth of</param>
    /// <param name="fontSize">the font size</param>
    /// <returns>the width in points</returns>
    public float GetWidthPoint(int char1, float fontSize) => GetWidth(char1) * 0.001f * fontSize;

    /// <summary>
    ///     Gets the width of a  String  in points taking kerning
    ///     into account.
    /// </summary>
    /// <param name="text">the  String  to get the witdth of</param>
    /// <param name="fontSize">the font size</param>
    /// <returns>the width in points</returns>
    public float GetWidthPointKerned(string text, float fontSize)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var size = GetWidth(text) * 0.001f * fontSize;
        if (!HasKernPairs())
        {
            return size;
        }

        var len = text.Length - 1;
        var kern = 0;
        var c = text.ToCharArray();
        for (var k = 0; k < len; ++k)
        {
            kern += GetKerning(c[k], c[k + 1]);
        }

        return size + kern * 0.001f * fontSize;
    }

    /// <summary>
    ///     Checks if the font has any kerning pairs.
    /// </summary>
    /// <returns> true  if the font has any kerning pairs</returns>
    public abstract bool HasKernPairs();

    /// <summary>
    ///     Gets the embedded flag.
    /// </summary>
    /// <returns> true  if the font is embedded.</returns>
    public bool IsEmbedded() => Embedded;

    /// <summary>
    ///     Gets the symbolic flag of the font.
    /// </summary>
    /// <returns> true  if the font is symbolic</returns>
    public bool IsFontSpecific() => FontSpecific;

    /// <summary>
    ///     Sets the character advance.
    ///     false  otherwise
    /// </summary>
    /// <param name="c">the character</param>
    /// <param name="advance">the character advance normalized to 1000 units</param>
    /// <returns> true  if the advance was set,</returns>
    public virtual bool SetCharAdvance(int c, int advance)
    {
        var b = ConvertToBytes(c);
        if (b.Length == 0)
        {
            return false;
        }

        widths[0xff & b[0]] = advance;
        return true;
    }

    /// <summary>
    ///     Sets the kerning between two Unicode chars.
    /// </summary>
    /// <param name="char1">the first char</param>
    /// <param name="char2">the second char</param>
    /// <param name="kern">the kerning to apply in normalized 1000 units</param>
    /// <returns> true  if the kerning was applied,  false  otherwise</returns>
    public abstract bool SetKerning(int char1, int char2, int kern);

    /// <summary>
    ///     Creates a unique subset prefix to be added to the font name when the font is embedded and subset.
    /// </summary>
    /// <returns>the subset prefix</returns>
    internal static string CreateSubsetPrefix()
    {
        var s = new char[7];
        lock (_random)
        {
            for (var k = 0; k < 6; ++k)
            {
                s[k] = (char)_random.Next('A', 'Z' + 1);
            }
        }

        s[6] = '+';
        return new string(s);
    }

    /// <summary>
    ///     Converts a  string  to a  byte  array according
    ///     to the font's encoding.
    /// </summary>
    /// <param name="text">the  string  to be converted</param>
    /// <returns>an array of  byte  representing the conversion according to the font's encoding</returns>
    internal virtual byte[] ConvertToBytes(string text)
    {
        if (directTextToByte)
        {
            return PdfEncodings.ConvertToBytes(text, null);
        }

        if (SpecialMap != null)
        {
            var b = new byte[text.Length];
            var ptr = 0;
            var length = text.Length;
            for (var k = 0; k < length; ++k)
            {
                var c = text[k];
                if (SpecialMap.TryGetValue(c, out var value))
                {
                    b[ptr++] = (byte)value;
                }
            }

            if (ptr < length)
            {
                var b2 = new byte[ptr];
                Array.Copy(b, 0, b2, 0, ptr);
                return b2;
            }

            return b;
        }

        return PdfEncodings.ConvertToBytes(text, encoding);
    }

    /// <summary>
    ///     Converts a  char  to a  byte  array according
    ///     to the font's encoding.
    /// </summary>
    /// <param name="char1">the  String  to be converted</param>
    /// <returns>an array of  byte  representing the conversion according to the font's encoding</returns>
    internal virtual byte[] ConvertToBytes(int char1)
    {
        if (directTextToByte)
        {
            return PdfEncodings.ConvertToBytes((char)char1, null);
        }

        if (SpecialMap != null)
        {
            if (SpecialMap.TryGetValue(char1, out var value))
            {
                return new[] { (byte)value };
            }

            return Array.Empty<byte>();
        }

        return PdfEncodings.ConvertToBytes((char)char1, encoding);
    }

    /// <summary>
    ///     Gets the width from the font according to the Unicode char  c
    ///     or the  name . If the  name  is null it's a symbolic font.
    /// </summary>
    /// <param name="c">the unicode char</param>
    /// <param name="name">the glyph name</param>
    /// <returns>the width of the char</returns>
    internal abstract int GetRawWidth(int c, string name);

    /// <summary>
    ///     Gets the Unicode character corresponding to the byte output to the pdf stream.
    /// </summary>
    /// <param name="index">the byte index</param>
    /// <returns>the Unicode character</returns>
    internal char GetUnicodeDifferences(int index) => unicodeDifferences[index];

    /// <summary>
    ///     Outputs to the writer the font dictionaries and streams.
    ///     @throws IOException on error
    ///     @throws DocumentException error in generating the object
    /// </summary>
    /// <param name="writer">the writer for this document</param>
    /// <param name="piRef">the font indirect reference</param>
    /// <param name="oParams">several parameters that depend on the font type</param>
    internal abstract void WriteFont(PdfWriter writer, PdfIndirectReference piRef, object[] oParams);

    /// <summary>
    ///     Gets the name without the modifiers Bold, Italic or BoldItalic.
    /// </summary>
    /// <param name="name">the full name of the font</param>
    /// <returns>the name without the modifiers Bold, Italic or BoldItalic</returns>
    protected static string GetBaseName(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (name.EndsWith(",Bold", StringComparison.OrdinalIgnoreCase))
        {
            return name.Substring(0, name.Length - 5);
        }

        if (name.EndsWith(",Italic", StringComparison.OrdinalIgnoreCase))
        {
            return name.Substring(0, name.Length - 7);
        }

        if (name.EndsWith(",BoldItalic", StringComparison.OrdinalIgnoreCase))
        {
            return name.Substring(0, name.Length - 11);
        }

        return name;
    }

    /// <summary>
    ///     Normalize the encoding names. "winansi" is changed to "Cp1252" and
    ///     "macroman" is changed to "MacRoman".
    /// </summary>
    /// <param name="enc">the encoding to be normalized</param>
    /// <returns>the normalized encoding</returns>
    protected static string NormalizeEncoding(string enc)
    {
        if (string.IsNullOrEmpty(enc) || enc.Equals("winansi", StringComparison.Ordinal))
        {
            return CP1252;
        }

        if (enc.Equals("macroman", StringComparison.Ordinal))
        {
            return MACROMAN;
        }

        var n = IanaEncodings.GetEncodingNumber(enc);
        if (n == 1252)
        {
            return CP1252;
        }

        if (n == 10000)
        {
            return MACROMAN;
        }

        return enc;
    }

    /// <summary>
    ///     Creates the  widths  and the  differences  arrays
    ///     @throws UnsupportedEncodingException the encoding is not supported
    /// </summary>
    protected void CreateEncoding()
    {
        if (encoding.StartsWith("#", StringComparison.Ordinal))
        {
            SpecialMap = new NullValueDictionary<int, int>();
            var tok = new StringTokenizer(encoding.Substring(1), " ,\t\n\r\f");
            if (tok.NextToken().Equals("full", StringComparison.Ordinal))
            {
                while (tok.HasMoreTokens())
                {
                    var order = tok.NextToken();
                    var name = tok.NextToken();
                    var uni = (char)int.Parse(tok.NextToken(), NumberStyles.HexNumber,
                                              CultureInfo.InvariantCulture);
                    int orderK;
                    if (order.StartsWith("'", StringComparison.Ordinal))
                    {
                        orderK = order[1];
                    }
                    else
                    {
                        orderK = int.Parse(order, CultureInfo.InvariantCulture);
                    }

                    orderK %= 256;
                    SpecialMap[uni] = orderK;
                    differences[orderK] = name;
                    unicodeDifferences[orderK] = uni;
                    widths[orderK] = GetRawWidth(uni, name);
                    CharBBoxes[orderK] = GetRawCharBBox(uni, name);
                }
            }
            else
            {
                var k = 0;
                if (tok.HasMoreTokens())
                {
                    k = int.Parse(tok.NextToken(), CultureInfo.InvariantCulture);
                }

                while (tok.HasMoreTokens() && k < 256)
                {
                    var hex = tok.NextToken();
                    var uni = int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture) % 0x10000;
                    var name = GlyphList.UnicodeToName(uni);
                    if (name != null)
                    {
                        SpecialMap[uni] = k;
                        differences[k] = name;
                        unicodeDifferences[k] = (char)uni;
                        widths[k] = GetRawWidth(uni, name);
                        CharBBoxes[k] = GetRawCharBBox(uni, name);
                        ++k;
                    }
                }
            }

            for (var k = 0; k < 256; ++k)
            {
                if (differences[k] == null)
                {
                    differences[k] = notdef;
                }
            }
        }
        else if (FontSpecific)
        {
            for (var k = 0; k < 256; ++k)
            {
                widths[k] = GetRawWidth(k, null);
                CharBBoxes[k] = GetRawCharBBox(k, null);
            }
        }
        else
        {
            string s;
            string name;
            char c;
            var b = new byte[1];
            for (var k = 0; k < 256; ++k)
            {
                b[0] = (byte)k;
                s = PdfEncodings.ConvertToString(b, encoding);
                if (s.Length > 0)
                {
                    c = s[0];
                }
                else
                {
                    c = '?';
                }

                name = GlyphList.UnicodeToName(c);
                if (name == null)
                {
                    name = notdef;
                }

                differences[k] = name;
                UnicodeDifferences[k] = c;
                widths[k] = GetRawWidth(c, name);
                CharBBoxes[k] = GetRawCharBBox(c, name);
            }
        }
    }

    protected abstract int[] GetRawCharBBox(int c, string name);

    private static void addFont(PrIndirectReference fontRef, NullValueDictionary<int, int> hits, List<object[]> fonts)
    {
        var obj = PdfReader.GetPdfObject(fontRef);
        if (obj == null || !obj.IsDictionary())
        {
            return;
        }

        var font = (PdfDictionary)obj;
        var subtype = font.GetAsName(PdfName.Subtype);
        if (!PdfName.Type1.Equals(subtype) && !PdfName.Truetype.Equals(subtype))
        {
            return;
        }

        var name = font.GetAsName(PdfName.Basefont);
        fonts.Add(new object[] { PdfName.DecodeName(name.ToString()), fontRef });
        hits[fontRef.Number] = 1;
    }

    private static void recourseFonts(PdfDictionary page, NullValueDictionary<int, int> hits, List<object[]> fonts,
                                      int level)
    {
        ++level;
        if (level > 50) // in case we have an endless loop
        {
            return;
        }

        var resources = page.GetAsDict(PdfName.Resources);
        if (resources == null)
        {
            return;
        }

        var font = resources.GetAsDict(PdfName.Font);
        if (font != null)
        {
            foreach (var key in font.Keys)
            {
                var ft = font.Get(key);
                if (ft == null || !ft.IsIndirect())
                {
                    continue;
                }

                var hit = ((PrIndirectReference)ft).Number;
                if (hits.ContainsKey(hit))
                {
                    continue;
                }

                addFont((PrIndirectReference)ft, hits, fonts);
            }
        }

        var xobj = resources.GetAsDict(PdfName.Xobject);
        if (xobj != null)
        {
            foreach (var key in xobj.Keys)
            {
                recourseFonts(xobj.GetAsDict(key), hits, fonts, level);
            }
        }
    }

    /// <summary>
    ///     Generates the PDF stream with the Type1 and Truetype fonts returning
    ///     a PdfStream.
    /// </summary>
    internal class StreamFont : PdfStream
    {
        /// <summary>
        ///     Generates the PDF stream with the Type1 and Truetype fonts returning
        ///     a PdfStream.
        ///     @throws DocumentException error in the stream compression
        ///     @since   2.1.3 (replaces the constructor without param compressionLevel)
        /// </summary>
        /// <param name="contents">the content of the stream</param>
        /// <param name="lengths">an array of int that describes the several lengths of each part of the font</param>
        /// <param name="compressionLevel">the compression level of the Stream</param>
        internal StreamFont(byte[] contents, int[] lengths, int compressionLevel)
        {
            Bytes = contents;
            Put(PdfName.LENGTH, new PdfNumber(Bytes.Length));
            for (var k = 0; k < lengths.Length; ++k)
            {
                Put(new PdfName("Length" + (k + 1)), new PdfNumber(lengths[k]));
            }

            FlateCompress(compressionLevel);
        }

        /// <summary>
        ///     Generates the PDF stream for a font.
        ///     @throws DocumentException error in the stream compression
        ///     @since   2.1.3 (replaces the constructor without param compressionLevel)
        /// </summary>
        /// <param name="contents">the content of a stream</param>
        /// <param name="subType">the subtype of the font.</param>
        /// <param name="compressionLevel">the compression level of the Stream</param>
        internal StreamFont(byte[] contents, string subType, int compressionLevel)
        {
            Bytes = contents;
            Put(PdfName.LENGTH, new PdfNumber(Bytes.Length));
            if (subType != null)
            {
                Put(PdfName.Subtype, new PdfName(subType));
            }

            FlateCompress(compressionLevel);
        }
    }
}