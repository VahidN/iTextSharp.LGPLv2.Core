using System.Text;
using System.util;
using iTextSharp.LGPLv2.Core.System.Encodings;

namespace iTextSharp.text.pdf;

/// <summary>
///     Reads a Truetype font
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
internal class TrueTypeFont : BaseFont
{
    /// <summary>
    ///     The code pages possible for a True Type font.
    /// </summary>
    internal static readonly string[] CodePages =
    {
        "1252 Latin 1",
        "1250 Latin 2: Eastern Europe",
        "1251 Cyrillic",
        "1253 Greek",
        "1254 Turkish",
        "1255 Hebrew",
        "1256 Arabic",
        "1257 Windows Baltic",
        "1258 Vietnamese",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        "874 Thai",
        "932 JIS/Japan",
        "936 Chinese: Simplified chars--PRC and Singapore",
        "949 Korean Wansung",
        "950 Chinese: Traditional chars--Taiwan and Hong Kong",
        "1361 Korean Johab",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        "Macintosh Character Set (US Roman)",
        "OEM Character Set",
        "Symbol Character Set",
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        "869 IBM Greek",
        "866 MS-DOS Russian",
        "865 MS-DOS Nordic",
        "864 Arabic",
        "863 MS-DOS Canadian French",
        "862 Hebrew",
        "861 MS-DOS Icelandic",
        "860 MS-DOS Portuguese",
        "857 IBM Turkish",
        "855 IBM Cyrillic; primarily Russian",
        "852 Latin 2",
        "775 MS-DOS Baltic",
        "737 Greek; former 437 G",
        "708 Arabic; ASMO 708",
        "850 WE/Latin 1",
        "437 US",
    };

    /// <summary>
    ///     All the names auf the Names-Table
    /// </summary>
    protected string[][] allNameEntries;

    protected int[][] Bboxes;
    protected bool Cff;
    protected int CffLength;
    protected int CffOffset;

    /// <summary>
    ///     The map containing the code information for the table 'cmap', encoding 1.0.
    ///     The key is the code and the value is an  int[2]  where position 0
    ///     is the glyph number and position 1 is the glyph width normalized to 1000
    ///     units.
    /// </summary>
    protected INullValueDictionary<int, int[]> Cmap10;

    /// <summary>
    ///     The map containing the code information for the table 'cmap', encoding 3.1
    ///     in Unicode.
    ///     The key is the code and the value is an  int [2] where position 0
    ///     is the glyph number and position 1 is the glyph width normalized to 1000
    ///     units.
    /// </summary>
    protected INullValueDictionary<int, int[]> Cmap31;

    /// <summary>
    ///     By James for unicode Ext.B
    /// </summary>
    protected INullValueDictionary<int, int[]> CmapExt;

    /// <summary>
    ///     The offset from the start of the file to the table directory.
    ///     It is 0 for TTF and may vary for TTC depending on the chosen font.
    /// </summary>
    protected int DirectoryOffset;

    /// <summary>
    ///     The family name of the font
    /// </summary>
    protected string[][] FamilyName;

    /// <summary>
    ///     The file name.
    /// </summary>
    public string FileName;

    /// <summary>
    ///     The font name.
    ///     This name is usually extracted from the table 'name' with
    ///     the 'Name ID' 6.
    /// </summary>
    protected string FontName;

    /// <summary>
    ///     The full name of the font
    /// </summary>
    protected string[][] FullName;

    /// <summary>
    ///     The width of the glyphs. This is essentially the content of table
    ///     'hmtx' normalized to 1000 units.
    /// </summary>
    protected int[] GlyphWidths;

    /// <summary>
    ///     The content of table 'head'.
    /// </summary>
    protected readonly FontHeader Head = new();

    /// <summary>
    ///     The content of table 'hhea'.
    /// </summary>
    protected readonly HorizontalHeader Hhea = new();

    /// <summary>
    ///     true  if all the glyphs have the same width.
    /// </summary>
    protected bool IsFixedPitch;

    /// <summary>
    ///     The italic angle. It is usually extracted from the 'post' table or in it's
    ///     absence with the code:
    ///     -Math.Atan2(hhea.caretSlopeRun, hhea.caretSlopeRise) * 180 / Math.PI
    /// </summary>
    protected double ItalicAngle;

    protected readonly bool JustNames;

    /// <summary>
    ///     The map containing the kerning information. It represents the content of
    ///     table 'kern'. The key is an  Integer  where the top 16 bits
    ///     are the glyph number for the first character and the lower 16 bits are the
    ///     glyph number for the second character. The value is the amount of kerning in
    ///     normalized 1000 units as an  Integer . This value is usually negative.
    /// </summary>
    protected readonly NullValueDictionary<int, int> Kerning = new();

    /// <summary>
    ///     The content of table 'OS/2'.
    /// </summary>
    protected readonly WindowsMetrics Os2 = new();

    /// <summary>
    ///     The file in use.
    /// </summary>
    protected RandomAccessFileOrArray Rf;

    /// <summary>
    ///     The style modifier
    /// </summary>
    protected string Style = "";

    /// <summary>
    ///     Contains the location of the several tables. The key is the name of
    ///     the table and the value is an  int[2]  where position 0
    ///     is the offset from the start of the file and position 1 is the length
    ///     of the table.
    /// </summary>
    protected INullValueDictionary<string, int[]> Tables;

    /// <summary>
    ///     The index for the TTC font. It is an empty  string  for a
    ///     TTF file.
    /// </summary>
    protected string TtcIndex;

    protected int UnderlinePosition;

    protected int UnderlineThickness;

    /// <summary>
    ///     Creates a new TrueType font.
    ///     '.ttc' but can have modifiers after the name
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <param name="ttFile">the location of the font on file. The file must end in '.ttf' or</param>
    /// <param name="enc">the encoding to be applied to this font</param>
    /// <param name="emb">true if the font is to be embedded in the PDF</param>
    /// <param name="ttfAfm">the font as a  byte  array</param>
    /// <param name="justNames"></param>
    /// <param name="forceRead"></param>
    internal TrueTypeFont(string ttFile, string enc, bool emb, byte[] ttfAfm, bool justNames, bool forceRead)
    {
        JustNames = justNames;
        var nameBase = GetBaseName(ttFile);
        var ttcName = GetTtcName(nameBase);
        if (nameBase.Length < ttFile.Length)
        {
            Style = ttFile.Substring(nameBase.Length);
        }

        encoding = enc;
        Embedded = emb;
        FileName = ttcName;
        FontType = FONT_TYPE_TT;
        TtcIndex = "";
        if (ttcName.Length < nameBase.Length)
        {
            TtcIndex = nameBase.Substring(ttcName.Length + 1);
        }

        if (FileName.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
            FileName.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
            FileName.EndsWith(".ttc", StringComparison.OrdinalIgnoreCase))
        {
            Process(ttfAfm, forceRead);
            if (!justNames && Embedded && Os2.FsType == 2)
            {
                throw new DocumentException(FileName + Style + " cannot be embedded due to licensing restrictions.");
            }
        }
        else
        {
            throw new DocumentException(FileName + Style + " is not a TTF, OTF or TTC font file.");
        }

        if (!encoding.StartsWith("#", StringComparison.Ordinal))
        {
            PdfEncodings.ConvertToBytes(" ", enc); // check if the encoding exists
        }

        CreateEncoding();
    }

    /// <summary>
    ///     This constructor is present to allow extending the class.
    /// </summary>
    protected TrueTypeFont()
    {
    }

    /// <summary>
    ///     Gets all the entries of the Names-Table. If it is a True Type font
    ///     each array element will have {Name ID, Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the full name of the font</returns>
    public override string[][] AllNameEntries => allNameEntries;

    /// <summary>
    ///     Gets the code pages supported by the font.
    /// </summary>
    /// <returns>the code pages supported by the font</returns>
    public override string[] CodePagesSupported
    {
        get
        {
            var cp = ((long)Os2.UlCodePageRange2 << 32) + (Os2.UlCodePageRange1 & 0xffffffffL);
            var count = 0;
            long bit = 1;
            for (var k = 0; k < 64; ++k)
            {
                if ((cp & bit) != 0 && CodePages[k] != null)
                {
                    ++count;
                }

                bit <<= 1;
            }

            var ret = new string[count];
            count = 0;
            bit = 1;
            for (var k = 0; k < 64; ++k)
            {
                if ((cp & bit) != 0 && CodePages[k] != null)
                {
                    ret[count++] = CodePages[k];
                }

                bit <<= 1;
            }

            return ret;
        }
    }

    /// <summary>
    ///     Gets the family name of the font. If it is a True Type font
    ///     each array element will have {Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the family name of the font</returns>
    public override string[][] FamilyFontName => FamilyName;

    /// <summary>
    ///     Gets the full name of the font. If it is a True Type font
    ///     each array element will have {Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the full name of the font</returns>
    public override string[][] FullFontName => FullName;

    /// <summary>
    ///     Gets the postscript font name.
    /// </summary>
    /// <returns>the postscript font name</returns>
    public override string PostscriptFontName
    {
        get => FontName;
        set => FontName = value;
    }

    /// <summary>
    ///     Gets the Postscript font name.
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <returns>the Postscript font name</returns>
    internal string BaseFont
    {
        get
        {
            int[] tableLocation;
            tableLocation = Tables["name"];
            if (tableLocation == null)
            {
                throw new DocumentException("Table 'name' does not exist in " + FileName + Style);
            }

            Rf.Seek(tableLocation[0] + 2);
            var numRecords = Rf.ReadUnsignedShort();
            var startOfStorage = Rf.ReadUnsignedShort();
            for (var k = 0; k < numRecords; ++k)
            {
                var platformId = Rf.ReadUnsignedShort();
                var platformEncodingId = Rf.ReadUnsignedShort();
                var languageId = Rf.ReadUnsignedShort();
                var nameId = Rf.ReadUnsignedShort();
                var length = Rf.ReadUnsignedShort();
                var offset = Rf.ReadUnsignedShort();
                if (nameId == 6)
                {
                    Rf.Seek(tableLocation[0] + startOfStorage + offset);
                    if (platformId == 0 || platformId == 3)
                    {
                        return ReadUnicodeString(length);
                    }

                    return ReadStandardString(length);
                }
            }

            var file = new FileInfo(FileName);
            return file.Name.Replace(' ', '-');
        }
    }

    /// <summary>
    ///     Gets the font parameter identified by  key . Valid values
    ///     for  key  are  ASCENT ,  CAPHEIGHT ,  DESCENT
    ///     and  ITALICANGLE .
    /// </summary>
    /// <param name="key">the parameter to be extracted</param>
    /// <param name="fontSize">the font size in points</param>
    /// <returns>the parameter in points</returns>
    public override float GetFontDescriptor(int key, float fontSize)
    {
        switch (key)
        {
            case ASCENT:
                return Os2.STypoAscender * fontSize / Head.UnitsPerEm;
            case CAPHEIGHT:
                return Os2.SCapHeight * fontSize / Head.UnitsPerEm;
            case DESCENT:
                return Os2.STypoDescender * fontSize / Head.UnitsPerEm;
            case ITALICANGLE:
                return (float)ItalicAngle;
            case BBOXLLX:
                return fontSize * Head.XMin / Head.UnitsPerEm;
            case BBOXLLY:
                return fontSize * Head.YMin / Head.UnitsPerEm;
            case BBOXURX:
                return fontSize * Head.XMax / Head.UnitsPerEm;
            case BBOXURY:
                return fontSize * Head.YMax / Head.UnitsPerEm;
            case AWT_ASCENT:
                return fontSize * Hhea.Ascender / Head.UnitsPerEm;
            case AWT_DESCENT:
                return fontSize * Hhea.Descender / Head.UnitsPerEm;
            case AWT_LEADING:
                return fontSize * Hhea.LineGap / Head.UnitsPerEm;
            case AWT_MAXADVANCE:
                return fontSize * Hhea.AdvanceWidthMax / Head.UnitsPerEm;
            case UNDERLINE_POSITION:
                return (UnderlinePosition - (float)UnderlineThickness / 2) * fontSize / Head.UnitsPerEm;
            case UNDERLINE_THICKNESS:
                return UnderlineThickness * fontSize / Head.UnitsPerEm;
            case STRIKETHROUGH_POSITION:
                return Os2.YStrikeoutPosition * fontSize / Head.UnitsPerEm;
            case STRIKETHROUGH_THICKNESS:
                return Os2.YStrikeoutSize * fontSize / Head.UnitsPerEm;
            case SUBSCRIPT_SIZE:
                return Os2.YSubscriptYSize * fontSize / Head.UnitsPerEm;
            case SUBSCRIPT_OFFSET:
                return -Os2.YSubscriptYOffset * fontSize / Head.UnitsPerEm;
            case SUPERSCRIPT_SIZE:
                return Os2.YSuperscriptYSize * fontSize / Head.UnitsPerEm;
            case SUPERSCRIPT_OFFSET:
                return Os2.YSuperscriptYOffset * fontSize / Head.UnitsPerEm;
        }

        return 0;
    }

    /// <summary>
    ///     Returns a PdfStream object with the full font program.
    ///     @since   2.1.3
    /// </summary>
    /// <returns>a PdfStream with the font program</returns>
    public override PdfStream GetFullFontStream()
    {
        if (Cff)
        {
            return new StreamFont(ReadCffFont(), "Type1C", compressionLevel);
        }

        var b = GetFullFont();
        int[] lengths = { b.Length };
        return new StreamFont(b, lengths, compressionLevel);
    }

    /// <summary>
    ///     Gets the kerning between two Unicode chars.
    /// </summary>
    /// <param name="char1">the first char</param>
    /// <param name="char2">the second char</param>
    /// <returns>the kerning to be applied</returns>
    public override int GetKerning(int char1, int char2)
    {
        var metrics = GetMetricsTt(char1);
        if (metrics == null)
        {
            return 0;
        }

        var c1 = metrics[0];
        metrics = GetMetricsTt(char2);
        if (metrics == null)
        {
            return 0;
        }

        var c2 = metrics[0];
        return Kerning[(c1 << 16) + c2];
    }

    /// <summary>
    ///     Gets the glyph index and metrics for a character.
    /// </summary>
    /// <param name="c">the character</param>
    /// <returns>an  int  array with {glyph index, width}</returns>
    public virtual int[] GetMetricsTt(int c)
    {
        if (CmapExt != null)
        {
            return CmapExt[c];
        }

        if (!FontSpecific && Cmap31 != null)
        {
            return Cmap31[c];
        }

        if (FontSpecific && Cmap10 != null)
        {
            return Cmap10[c];
        }

        if (Cmap31 != null)
        {
            return Cmap31[c];
        }

        if (Cmap10 != null)
        {
            return Cmap10[c];
        }

        return null;
    }

    /// <summary>
    ///     Checks if the font has any kerning pairs.
    /// </summary>
    /// <returns> true  if the font has any kerning pairs</returns>
    public override bool HasKernPairs() => Kerning.Size > 0;

    /// <summary>
    ///     Sets the kerning between two Unicode chars.
    /// </summary>
    /// <param name="char1">the first char</param>
    /// <param name="char2">the second char</param>
    /// <param name="kern">the kerning to apply in normalized 1000 units</param>
    /// <returns> true  if the kerning was applied,  false  otherwise</returns>
    public override bool SetKerning(int char1, int char2, int kern)
    {
        var metrics = GetMetricsTt(char1);
        if (metrics == null)
        {
            return false;
        }

        var c1 = metrics[0];
        metrics = GetMetricsTt(char2);
        if (metrics == null)
        {
            return false;
        }

        var c2 = metrics[0];
        Kerning[(c1 << 16) + c2] = kern;
        return true;
    }

    internal void CheckCff()
    {
        int[] tableLocation;
        tableLocation = Tables["CFF "];
        if (tableLocation != null)
        {
            Cff = true;
            CffOffset = tableLocation[0];
            CffLength = tableLocation[1];
        }
    }

    /// <summary>
    ///     Reads the tables 'head', 'hhea', 'OS/2' and 'post' filling several variables.
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    /// </summary>
    internal void FillTables()
    {
        int[] tableLocation;
        tableLocation = Tables["head"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'head' does not exist in " + FileName + Style);
        }

        Rf.Seek(tableLocation[0] + 16);
        Head.Flags = Rf.ReadUnsignedShort();
        Head.UnitsPerEm = Rf.ReadUnsignedShort();
        Rf.SkipBytes(16);
        Head.XMin = Rf.ReadShort();
        Head.YMin = Rf.ReadShort();
        Head.XMax = Rf.ReadShort();
        Head.YMax = Rf.ReadShort();
        Head.MacStyle = Rf.ReadUnsignedShort();

        tableLocation = Tables["hhea"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'hhea' does not exist " + FileName + Style);
        }

        Rf.Seek(tableLocation[0] + 4);
        Hhea.Ascender = Rf.ReadShort();
        Hhea.Descender = Rf.ReadShort();
        Hhea.LineGap = Rf.ReadShort();
        Hhea.AdvanceWidthMax = Rf.ReadUnsignedShort();
        Hhea.MinLeftSideBearing = Rf.ReadShort();
        Hhea.MinRightSideBearing = Rf.ReadShort();
        Hhea.XMaxExtent = Rf.ReadShort();
        Hhea.CaretSlopeRise = Rf.ReadShort();
        Hhea.CaretSlopeRun = Rf.ReadShort();
        Rf.SkipBytes(12);
        Hhea.NumberOfHMetrics = Rf.ReadUnsignedShort();

        tableLocation = Tables["OS/2"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'OS/2' does not exist in " + FileName + Style);
        }

        Rf.Seek(tableLocation[0]);
        var version = Rf.ReadUnsignedShort();
        Os2.XAvgCharWidth = Rf.ReadShort();
        Os2.UsWeightClass = Rf.ReadUnsignedShort();
        Os2.UsWidthClass = Rf.ReadUnsignedShort();
        Os2.FsType = Rf.ReadShort();
        Os2.YSubscriptXSize = Rf.ReadShort();
        Os2.YSubscriptYSize = Rf.ReadShort();
        Os2.YSubscriptXOffset = Rf.ReadShort();
        Os2.YSubscriptYOffset = Rf.ReadShort();
        Os2.YSuperscriptXSize = Rf.ReadShort();
        Os2.YSuperscriptYSize = Rf.ReadShort();
        Os2.YSuperscriptXOffset = Rf.ReadShort();
        Os2.YSuperscriptYOffset = Rf.ReadShort();
        Os2.YStrikeoutSize = Rf.ReadShort();
        Os2.YStrikeoutPosition = Rf.ReadShort();
        Os2.SFamilyClass = Rf.ReadShort();
        Rf.ReadFully(Os2.Panose);
        Rf.SkipBytes(16);
        Rf.ReadFully(Os2.AchVendId);
        Os2.FsSelection = Rf.ReadUnsignedShort();
        Os2.UsFirstCharIndex = Rf.ReadUnsignedShort();
        Os2.UsLastCharIndex = Rf.ReadUnsignedShort();
        Os2.STypoAscender = Rf.ReadShort();
        Os2.STypoDescender = Rf.ReadShort();
        if (Os2.STypoDescender > 0)
        {
            Os2.STypoDescender = (short)-Os2.STypoDescender;
        }

        Os2.STypoLineGap = Rf.ReadShort();
        Os2.UsWinAscent = Rf.ReadUnsignedShort();
        Os2.UsWinDescent = Rf.ReadUnsignedShort();
        Os2.UlCodePageRange1 = 0;
        Os2.UlCodePageRange2 = 0;
        if (version > 0)
        {
            Os2.UlCodePageRange1 = Rf.ReadInt();
            Os2.UlCodePageRange2 = Rf.ReadInt();
        }

        if (version > 1)
        {
            Rf.SkipBytes(2);
            Os2.SCapHeight = Rf.ReadShort();
        }
        else
        {
            Os2.SCapHeight = (int)(0.7 * Head.UnitsPerEm);
        }

        tableLocation = Tables["post"];
        if (tableLocation == null)
        {
            ItalicAngle = -Math.Atan2(Hhea.CaretSlopeRun, Hhea.CaretSlopeRise) * 180 / Math.PI;
            return;
        }

        Rf.Seek(tableLocation[0] + 4);
        var mantissa = Rf.ReadShort();
        var fraction = Rf.ReadUnsignedShort();
        ItalicAngle = mantissa + fraction / 16384.0;
        UnderlinePosition = Rf.ReadShort();
        UnderlineThickness = Rf.ReadShort();
        IsFixedPitch = Rf.ReadInt() != 0;
    }

    /// <summary>
    ///     Extracts all the names of the names-Table
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    internal string[][] GetAllNames()
    {
        int[] tableLocation;
        tableLocation = Tables["name"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'name' does not exist in " + FileName + Style);
        }

        Rf.Seek(tableLocation[0] + 2);
        var numRecords = Rf.ReadUnsignedShort();
        var startOfStorage = Rf.ReadUnsignedShort();
        List<string[]> names = new();
        for (var k = 0; k < numRecords; ++k)
        {
            var platformId = Rf.ReadUnsignedShort();
            var platformEncodingId = Rf.ReadUnsignedShort();
            var languageId = Rf.ReadUnsignedShort();
            var nameId = Rf.ReadUnsignedShort();
            var length = Rf.ReadUnsignedShort();
            var offset = Rf.ReadUnsignedShort();
            var pos = Rf.FilePointer;
            Rf.Seek(tableLocation[0] + startOfStorage + offset);
            string name;
            if (platformId == 0 || platformId == 3 || (platformId == 2 && platformEncodingId == 1))
            {
                name = ReadUnicodeString(length);
            }
            else
            {
                name = ReadStandardString(length);
            }

            names.Add(new[]
                      {
                          nameId.ToString(CultureInfo.InvariantCulture),
                          platformId.ToString(CultureInfo.InvariantCulture),
                          platformEncodingId.ToString(CultureInfo.InvariantCulture),
                          languageId.ToString(CultureInfo.InvariantCulture),
                          name,
                      });
            Rf.Seek(pos);
        }

        var thisName = new string[names.Count][];
        for (var k = 0; k < names.Count; ++k)
        {
            thisName[k] = names[k];
        }

        return thisName;
    }

    /// <summary>
    ///     Extracts the names of the font in all the languages available.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="id">the name id to retrieve</param>
    internal string[][] GetNames(int id)
    {
        int[] tableLocation;
        tableLocation = Tables["name"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'name' does not exist in " + FileName + Style);
        }

        Rf.Seek(tableLocation[0] + 2);
        var numRecords = Rf.ReadUnsignedShort();
        var startOfStorage = Rf.ReadUnsignedShort();
        List<string[]> names = new();
        for (var k = 0; k < numRecords; ++k)
        {
            var platformId = Rf.ReadUnsignedShort();
            var platformEncodingId = Rf.ReadUnsignedShort();
            var languageId = Rf.ReadUnsignedShort();
            var nameId = Rf.ReadUnsignedShort();
            var length = Rf.ReadUnsignedShort();
            var offset = Rf.ReadUnsignedShort();
            if (nameId == id)
            {
                var pos = Rf.FilePointer;
                Rf.Seek(tableLocation[0] + startOfStorage + offset);
                string name;
                if (platformId == 0 || platformId == 3 || (platformId == 2 && platformEncodingId == 1))
                {
                    name = ReadUnicodeString(length);
                }
                else
                {
                    name = ReadStandardString(length);
                }

                names.Add(new[]
                          {
                              platformId.ToString(CultureInfo.InvariantCulture),
                              platformEncodingId.ToString(CultureInfo.InvariantCulture),
                              languageId.ToString(CultureInfo.InvariantCulture),
                              name,
                          });
                Rf.Seek(pos);
            }
        }

        var thisName = new string[names.Count][];
        for (var k = 0; k < names.Count; ++k)
        {
            thisName[k] = names[k];
        }

        return thisName;
    }

    /// <summary>
    ///     Gets the width from the font according to the unicode char  c .
    ///     If the  name  is null it's a symbolic font.
    /// </summary>
    /// <param name="c">the unicode char</param>
    /// <param name="name">the glyph name</param>
    /// <returns>the width of the char</returns>
    internal override int GetRawWidth(int c, string name)
    {
        var metric = GetMetricsTt(c);
        if (metric == null)
        {
            return 0;
        }

        return metric[1];
    }

    /// <summary>
    ///     Reads the font data.
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <param name="ttfAfm">the font as a  byte  array, possibly  null </param>
    /// <param name="preload"></param>
    internal void Process(byte[] ttfAfm, bool preload)
    {
        Tables = new NullValueDictionary<string, int[]>();

        try
        {
            if (ttfAfm == null)
            {
                Rf = new RandomAccessFileOrArray(FileName, preload);
            }
            else
            {
                Rf = new RandomAccessFileOrArray(ttfAfm);
            }

            if (TtcIndex.Length > 0)
            {
                var dirIdx = int.Parse(TtcIndex, CultureInfo.InvariantCulture);
                if (dirIdx < 0)
                {
                    throw new DocumentException("The font index for " + FileName + " must be positive.");
                }

                var mainTag = ReadStandardString(4);
                if (!mainTag.Equals("ttcf", StringComparison.Ordinal))
                {
                    throw new DocumentException(FileName + " is not a valid TTC file.");
                }

                Rf.SkipBytes(4);
                var dirCount = Rf.ReadInt();
                if (dirIdx >= dirCount)
                {
                    throw new DocumentException("The font index for " + FileName + " must be between 0 and " +
                                                (dirCount - 1) + ". It was " + dirIdx + ".");
                }

                Rf.SkipBytes(dirIdx * 4);
                DirectoryOffset = Rf.ReadInt();
            }

            Rf.Seek(DirectoryOffset);
            var ttId = Rf.ReadInt();
            if (ttId != 0x00010000 && ttId != 0x4F54544F)
            {
                throw new DocumentException(FileName + " is not a valid TTF or OTF file.");
            }

            var numTables = Rf.ReadUnsignedShort();
            Rf.SkipBytes(6);
            for (var k = 0; k < numTables; ++k)
            {
                var tag = ReadStandardString(4);
                Rf.SkipBytes(4);
                var tableLocation = new int[2];
                tableLocation[0] = Rf.ReadInt();
                tableLocation[1] = Rf.ReadInt();
                Tables[tag] = tableLocation;
            }

            CheckCff();
            FontName = BaseFont;
            FullName = GetNames(4); //full name
            FamilyName = GetNames(1); //family name
            allNameEntries = GetAllNames();
            if (!JustNames)
            {
                FillTables();
                ReadGlyphWidths();
                ReadCMaps();
                ReadKerning();
                readBbox();
                GlyphWidths = null;
            }
        }
        finally
        {
            if (Rf != null)
            {
                Rf.Close();
                if (!Embedded)
                {
                    Rf = null;
                }
            }
        }
    }

    /// <summary>
    ///     Reads the several maps from the table 'cmap'. The maps of interest are 1.0 for symbolic
    ///     fonts and 3.1 for all others. A symbolic font is defined as having the map 3.0.
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    /// </summary>
    internal void ReadCMaps()
    {
        int[] tableLocation;
        tableLocation = Tables["cmap"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'cmap' does not exist in " + FileName + Style);
        }

        Rf.Seek(tableLocation[0]);
        Rf.SkipBytes(2);
        var numTables = Rf.ReadUnsignedShort();
        FontSpecific = false;
        var map10 = 0;
        var map31 = 0;
        var map30 = 0;

        //add by james for cmap Ext.b
        var mapExt = 0;

        for (var k = 0; k < numTables; ++k)
        {
            var platId = Rf.ReadUnsignedShort();
            var platSpecId = Rf.ReadUnsignedShort();
            var offset = Rf.ReadInt();
            if (platId == 3 && platSpecId == 0)
            {
                FontSpecific = true;
                map30 = offset;
            }
            else if (platId == 3 && platSpecId == 1)
            {
                map31 = offset;
            }
            else if (platId == 3 && platSpecId == 10)
            {
                mapExt = offset;
            }

            if (platId == 1 && platSpecId == 0)
            {
                map10 = offset;
            }
        }

        if (map10 > 0)
        {
            Rf.Seek(tableLocation[0] + map10);
            var format = Rf.ReadUnsignedShort();
            switch (format)
            {
                case 0:
                    Cmap10 = ReadFormat0();
                    break;
                case 4:
                    Cmap10 = ReadFormat4();
                    break;
                case 6:
                    Cmap10 = ReadFormat6();
                    break;
            }
        }

        if (map31 > 0)
        {
            Rf.Seek(tableLocation[0] + map31);
            var format = Rf.ReadUnsignedShort();
            if (format == 4)
            {
                Cmap31 = ReadFormat4();
            }
        }

        if (map30 > 0)
        {
            Rf.Seek(tableLocation[0] + map30);
            var format = Rf.ReadUnsignedShort();
            if (format == 4)
            {
                Cmap10 = ReadFormat4();
            }
        }

        if (mapExt > 0)
        {
            Rf.Seek(tableLocation[0] + mapExt);
            var format = Rf.ReadUnsignedShort();
            switch (format)
            {
                case 0:
                    CmapExt = ReadFormat0();
                    break;
                case 4:
                    CmapExt = ReadFormat4();
                    break;
                case 6:
                    CmapExt = ReadFormat6();
                    break;
                case 12:
                    CmapExt = ReadFormat12();
                    break;
            }
        }
    }

    /// <summary>
    ///     The information in the maps of the table 'cmap' is coded in several formats.
    ///     Format 0 is the Apple standard character to glyph index mapping table.
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <returns>a  Hashtable  representing this map</returns>
    internal INullValueDictionary<int, int[]> ReadFormat0()
    {
        var h = new NullValueDictionary<int, int[]>();
        Rf.SkipBytes(4);
        for (var k = 0; k < 256; ++k)
        {
            var r = new int[2];
            r[0] = Rf.ReadUnsignedByte();
            r[1] = GetGlyphWidth(r[0]);
            h[k] = r;
        }

        return h;
    }

    internal INullValueDictionary<int, int[]> ReadFormat12()
    {
        var h = new NullValueDictionary<int, int[]>();
        Rf.SkipBytes(2);
        var tableLenght = Rf.ReadInt();
        Rf.SkipBytes(4);
        var nGroups = Rf.ReadInt();
        for (var k = 0; k < nGroups; k++)
        {
            var startCharCode = Rf.ReadInt();
            var endCharCode = Rf.ReadInt();
            var startGlyphId = Rf.ReadInt();
            for (var i = startCharCode; i <= endCharCode; i++)
            {
                var r = new int[2];
                r[0] = startGlyphId;
                r[1] = GetGlyphWidth(r[0]);
                h[i] = r;
                startGlyphId++;
            }
        }

        return h;
    }

    /// <summary>
    ///     The information in the maps of the table 'cmap' is coded in several formats.
    ///     Format 4 is the Microsoft standard character to glyph index mapping table.
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <returns>a  Hashtable  representing this map</returns>
    internal INullValueDictionary<int, int[]> ReadFormat4()
    {
        var h = new NullValueDictionary<int, int[]>();
        var tableLenght = Rf.ReadUnsignedShort();
        Rf.SkipBytes(2);
        var segCount = Rf.ReadUnsignedShort() / 2;
        Rf.SkipBytes(6);
        var endCount = new int[segCount];
        for (var k = 0; k < segCount; ++k)
        {
            endCount[k] = Rf.ReadUnsignedShort();
        }

        Rf.SkipBytes(2);
        var startCount = new int[segCount];
        for (var k = 0; k < segCount; ++k)
        {
            startCount[k] = Rf.ReadUnsignedShort();
        }

        var idDelta = new int[segCount];
        for (var k = 0; k < segCount; ++k)
        {
            idDelta[k] = Rf.ReadUnsignedShort();
        }

        var idRo = new int[segCount];
        for (var k = 0; k < segCount; ++k)
        {
            idRo[k] = Rf.ReadUnsignedShort();
        }

        var glyphId = new int[tableLenght / 2 - 8 - segCount * 4];
        for (var k = 0; k < glyphId.Length; ++k)
        {
            glyphId[k] = Rf.ReadUnsignedShort();
        }

        for (var k = 0; k < segCount; ++k)
        {
            int glyph;
            for (var j = startCount[k]; j <= endCount[k] && j != 0xFFFF; ++j)
            {
                if (idRo[k] == 0)
                {
                    glyph = (j + idDelta[k]) & 0xFFFF;
                }
                else
                {
                    var idx = k + idRo[k] / 2 - segCount + j - startCount[k];
                    if (idx >= glyphId.Length)
                    {
                        continue;
                    }

                    glyph = (glyphId[idx] + idDelta[k]) & 0xFFFF;
                }

                var r = new int[2];
                r[0] = glyph;
                r[1] = GetGlyphWidth(r[0]);
                h[FontSpecific ? (j & 0xff00) == 0xf000 ? j & 0xff : j : j] = r;
            }
        }

        return h;
    }

    /// <summary>
    ///     The information in the maps of the table 'cmap' is coded in several formats.
    ///     Format 6 is a trimmed table mapping. It is similar to format 0 but can have
    ///     less than 256 entries.
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <returns>a  Hashtable  representing this map</returns>
    internal INullValueDictionary<int, int[]> ReadFormat6()
    {
        var h = new NullValueDictionary<int, int[]>();
        Rf.SkipBytes(4);
        var startCode = Rf.ReadUnsignedShort();
        var codeCount = Rf.ReadUnsignedShort();
        for (var k = 0; k < codeCount; ++k)
        {
            var r = new int[2];
            r[0] = Rf.ReadUnsignedShort();
            r[1] = GetGlyphWidth(r[0]);
            h[k + startCode] = r;
        }

        return h;
    }

    /// <summary>
    ///     Reads the kerning information from the 'kern' table.
    ///     @throws IOException the font file could not be read
    /// </summary>
    internal void ReadKerning()
    {
        int[] tableLocation;
        tableLocation = Tables["kern"];
        if (tableLocation == null)
        {
            return;
        }

        Rf.Seek(tableLocation[0] + 2);
        var nTables = Rf.ReadUnsignedShort();
        var checkpoint = tableLocation[0] + 4;
        var length = 0;
        for (var k = 0; k < nTables; ++k)
        {
            checkpoint += length;
            Rf.Seek(checkpoint);
            Rf.SkipBytes(2);
            length = Rf.ReadUnsignedShort();
            var coverage = Rf.ReadUnsignedShort();
            if ((coverage & 0xfff7) == 0x0001)
            {
                var nPairs = Rf.ReadUnsignedShort();
                Rf.SkipBytes(6);
                for (var j = 0; j < nPairs; ++j)
                {
                    var pair = Rf.ReadInt();
                    var value = Rf.ReadShort() * 1000 / Head.UnitsPerEm;
                    Kerning[pair] = value;
                }
            }
        }
    }

    /// <summary>
    ///     Outputs to the writer the font dictionaries and streams.
    ///     @throws IOException on error
    ///     @throws DocumentException error in generating the object
    /// </summary>
    /// <param name="writer">the writer for this document</param>
    /// <param name="piref">the font indirect reference</param>
    /// <param name="parms">several parameters that depend on the font type</param>
    internal override void WriteFont(PdfWriter writer, PdfIndirectReference piref, object[] parms)
    {
        var firstChar = (int)parms[0];
        var lastChar = (int)parms[1];
        var shortTag = (byte[])parms[2];
        var subsetp = (bool)parms[3] && subset;
        if (!subsetp)
        {
            firstChar = 0;
            lastChar = shortTag.Length - 1;
            for (var k = 0; k < shortTag.Length; ++k)
            {
                shortTag[k] = 1;
            }
        }

        PdfIndirectReference indFont = null;
        PdfObject pobj = null;
        PdfIndirectObject obj = null;
        var subsetPrefix = "";
        if (Embedded)
        {
            if (Cff)
            {
                pobj = new StreamFont(ReadCffFont(), "Type1C", compressionLevel);
                obj = writer.AddToBody(pobj);
                indFont = obj.IndirectReference;
            }
            else
            {
                if (subsetp)
                {
                    subsetPrefix = CreateSubsetPrefix();
                }

                var glyphs = new NullValueDictionary<int, int[]>();
                for (var k = firstChar; k <= lastChar; ++k)
                {
                    if (shortTag[k] != 0)
                    {
                        int[] metrics = null;
                        if (SpecialMap != null)
                        {
                            var cd = GlyphList.NameToUnicode(differences[k]);
                            if (cd != null)
                            {
                                metrics = GetMetricsTt(cd[0]);
                            }
                        }
                        else
                        {
                            if (FontSpecific)
                            {
                                metrics = GetMetricsTt(k);
                            }
                            else
                            {
                                metrics = GetMetricsTt(unicodeDifferences[k]);
                            }
                        }

                        if (metrics != null)
                        {
                            glyphs[metrics[0]] = null;
                        }
                    }
                }

                AddRangeUni(glyphs, false, subsetp);
                byte[] b = null;
                if (subsetp || DirectoryOffset != 0 || SubsetRanges != null)
                {
                    var sb = new TrueTypeFontSubSet(FileName,
                                                    new RandomAccessFileOrArray(Rf),
                                                    glyphs,
                                                    DirectoryOffset,
                                                    true,
                                                    !subsetp);
                    b = sb.Process();
                }
                else
                {
                    b = GetFullFont();
                }

                int[] lengths = { b.Length };
                pobj = new StreamFont(b, lengths, compressionLevel);
                obj = writer.AddToBody(pobj);
                indFont = obj.IndirectReference;
            }
        }

        pobj = GetFontDescriptor(indFont, subsetPrefix, null);
        if (pobj != null)
        {
            obj = writer.AddToBody(pobj);
            indFont = obj.IndirectReference;
        }

        pobj = GetFontBaseType(indFont, subsetPrefix, firstChar, lastChar, shortTag);
        writer.AddToBody(pobj, piref);
    }

    /// <summary>
    ///     If this font file is using the Compact Font File Format, then this method
    ///     will return the raw bytes needed for the font stream. If this method is
    ///     ever made public: make sure to add a test if (cff == true).
    ///     @since   2.1.3
    /// </summary>
    /// <returns>a byte array</returns>
    protected internal byte[] ReadCffFont()
    {
        var rf2 = new RandomAccessFileOrArray(Rf);
        var b = new byte[CffLength];
        try
        {
            rf2.ReOpen();
            rf2.Seek(CffOffset);
            rf2.ReadFully(b);
        }
        finally
        {
            try
            {
                rf2.Close();
            }
            catch
            {
                // empty on purpose
            }
        }

        return b;
    }

    protected static int[] CompactRanges(List<int[]> ranges)
    {
        List<int[]> simp = new();
        for (var k = 0; k < ranges.Count; ++k)
        {
            var r = ranges[k];
            for (var j = 0; j < r.Length; j += 2)
            {
                simp.Add(new[] { Math.Max(0, Math.Min(r[j], r[j + 1])), Math.Min(0xffff, Math.Max(r[j], r[j + 1])) });
            }
        }

        for (var k1 = 0; k1 < simp.Count - 1; ++k1)
        {
            for (var k2 = k1 + 1; k2 < simp.Count; ++k2)
            {
                var r1 = simp[k1];
                var r2 = simp[k2];
                if ((r1[0] >= r2[0] && r1[0] <= r2[1]) || (r1[1] >= r2[0] && r1[0] <= r2[1]))
                {
                    r1[0] = Math.Min(r1[0], r2[0]);
                    r1[1] = Math.Max(r1[1], r2[1]);
                    simp.RemoveAt(k2);
                    --k2;
                }
            }
        }

        var s = new int[simp.Count * 2];
        for (var k = 0; k < simp.Count; ++k)
        {
            var r = simp[k];
            s[k * 2] = r[0];
            s[k * 2 + 1] = r[1];
        }

        return s;
    }

    /// <summary>
    ///     Gets the name from a composed TTC file name.
    ///     If I have for input "myfont.ttc,2" the return will
    ///     be "myfont.ttc".
    /// </summary>
    /// <param name="name">the full name</param>
    /// <returns>the simple file name</returns>
    protected static string GetTtcName(string name)
    {
        var idx = name.IndexOf(".ttc,", StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            return name;
        }

        return name.Substring(0, idx + 4);
    }

    protected void AddRangeUni(INullValueDictionary<int, int[]> longTag, bool includeMetrics, bool subsetp)
    {
        if (!subsetp && (SubsetRanges != null || DirectoryOffset > 0))
        {
            var rg = SubsetRanges == null && DirectoryOffset > 0 ? new[] { 0, 0xffff } : CompactRanges(SubsetRanges);
            INullValueDictionary<int, int[]> usemap;
            if (!FontSpecific && Cmap31 != null)
            {
                usemap = Cmap31;
            }
            else if (FontSpecific && Cmap10 != null)
            {
                usemap = Cmap10;
            }
            else if (Cmap31 != null)
            {
                usemap = Cmap31;
            }
            else
            {
                usemap = Cmap10;
            }

            foreach (var e in usemap)
            {
                var v = e.Value;
                var gi = v[0];
                if (longTag.ContainsKey(gi))
                {
                    continue;
                }

                var c = e.Key;
                var skip = true;
                for (var k = 0; k < rg.Length; k += 2)
                {
                    if (c >= rg[k] && c <= rg[k + 1])
                    {
                        skip = false;
                        break;
                    }
                }

                if (!skip)
                {
                    longTag[gi] = includeMetrics ? new[] { v[0], v[1], c } : null;
                }
            }
        }
    }

    /// <summary>
    ///     Generates the font dictionary for this font.
    ///     @throws DocumentException if there is an error
    /// </summary>
    /// <param name="subsetPrefix">the subset prefx</param>
    /// <param name="firstChar">the first valid character</param>
    /// <param name="lastChar">the last valid character</param>
    /// <param name="shortTag">a 256 bytes long  byte  array where each unused byte is represented by 0</param>
    /// <param name="fontDescriptor">the indirect reference to a PdfDictionary containing the font descriptor or  null </param>
    /// <returns>the PdfDictionary containing the font dictionary</returns>
    protected PdfDictionary GetFontBaseType(PdfIndirectReference fontDescriptor,
                                            string subsetPrefix,
                                            int firstChar,
                                            int lastChar,
                                            byte[] shortTag)
    {
        var dic = new PdfDictionary(PdfName.Font);
        if (Cff)
        {
            dic.Put(PdfName.Subtype, PdfName.Type1);
            dic.Put(PdfName.Basefont, new PdfName(FontName + Style));
        }
        else
        {
            dic.Put(PdfName.Subtype, PdfName.Truetype);
            dic.Put(PdfName.Basefont, new PdfName(subsetPrefix + FontName + Style));
        }

        dic.Put(PdfName.Basefont, new PdfName(subsetPrefix + FontName + Style));
        if (!FontSpecific)
        {
            for (var k = firstChar; k <= lastChar; ++k)
            {
                if (!differences[k].Equals(notdef, StringComparison.Ordinal))
                {
                    firstChar = k;
                    break;
                }
            }

            if (encoding.Equals(CP1252, StringComparison.Ordinal) ||
                encoding.Equals(MACROMAN, StringComparison.Ordinal))
            {
                dic.Put(PdfName.Encoding,
                        encoding.Equals(CP1252, StringComparison.Ordinal)
                            ? PdfName.WinAnsiEncoding
                            : PdfName.MacRomanEncoding);
            }
            else
            {
                var enc = new PdfDictionary(PdfName.Encoding);
                var dif = new PdfArray();
                var gap = true;
                for (var k = firstChar; k <= lastChar; ++k)
                {
                    if (shortTag[k] != 0)
                    {
                        if (gap)
                        {
                            dif.Add(new PdfNumber(k));
                            gap = false;
                        }

                        dif.Add(new PdfName(differences[k]));
                    }
                    else
                    {
                        gap = true;
                    }
                }

                enc.Put(PdfName.Differences, dif);
                dic.Put(PdfName.Encoding, enc);
            }
        }

        dic.Put(PdfName.Firstchar, new PdfNumber(firstChar));
        dic.Put(PdfName.Lastchar, new PdfNumber(lastChar));
        var wd = new PdfArray();
        for (var k = firstChar; k <= lastChar; ++k)
        {
            if (shortTag[k] == 0)
            {
                wd.Add(new PdfNumber(0));
            }
            else
            {
                wd.Add(new PdfNumber(widths[k]));
            }
        }

        dic.Put(PdfName.Widths, wd);
        if (fontDescriptor != null)
        {
            dic.Put(PdfName.Fontdescriptor, fontDescriptor);
        }

        return dic;
    }

    /// <summary>
    ///     Generates the font descriptor for this font.
    ///     @throws DocumentException if there is an error
    /// </summary>
    /// <param name="subsetPrefix">the subset prefix</param>
    /// <param name="fontStream">the indirect reference to a PdfStream containing the font or  null </param>
    /// <param name="cidset"></param>
    /// <returns>the PdfDictionary containing the font descriptor or  null </returns>
    protected PdfDictionary GetFontDescriptor(PdfIndirectReference fontStream,
                                              string subsetPrefix,
                                              PdfIndirectReference cidset)
    {
        var dic = new PdfDictionary(PdfName.Fontdescriptor);
        dic.Put(PdfName.Ascent, new PdfNumber(Os2.STypoAscender * 1000 / Head.UnitsPerEm));
        dic.Put(PdfName.Capheight, new PdfNumber(Os2.SCapHeight * 1000 / Head.UnitsPerEm));
        dic.Put(PdfName.Descent, new PdfNumber(Os2.STypoDescender * 1000 / Head.UnitsPerEm));
        dic.Put(PdfName.Fontbbox,
                new PdfRectangle((float)Head.XMin * 1000 / Head.UnitsPerEm,
                                 (float)Head.YMin * 1000 / Head.UnitsPerEm,
                                 (float)Head.XMax * 1000 / Head.UnitsPerEm,
                                 (float)Head.YMax * 1000 / Head.UnitsPerEm));
        if (cidset != null)
        {
            dic.Put(PdfName.Cidset, cidset);
        }

        if (Cff)
        {
            if (encoding.StartsWith("Identity-", StringComparison.Ordinal))
            {
                dic.Put(PdfName.Fontname, new PdfName(subsetPrefix + FontName + "-" + encoding));
            }
            else
            {
                dic.Put(PdfName.Fontname, new PdfName(subsetPrefix + FontName + Style));
            }
        }
        else
        {
            dic.Put(PdfName.Fontname, new PdfName(subsetPrefix + FontName + Style));
        }

        dic.Put(PdfName.Italicangle, new PdfNumber(ItalicAngle));
        dic.Put(PdfName.Stemv, new PdfNumber(80));
        if (fontStream != null)
        {
            if (Cff)
            {
                dic.Put(PdfName.Fontfile3, fontStream);
            }
            else
            {
                dic.Put(PdfName.Fontfile2, fontStream);
            }
        }

        var flags = 0;
        if (IsFixedPitch)
        {
            flags |= 1;
        }

        flags |= FontSpecific ? 4 : 32;
        if ((Head.MacStyle & 2) != 0)
        {
            flags |= 64;
        }

        if ((Head.MacStyle & 1) != 0)
        {
            flags |= 262144;
        }

        dic.Put(PdfName.Flags, new PdfNumber(flags));

        return dic;
    }

    protected byte[] GetFullFont()
    {
        RandomAccessFileOrArray rf2 = null;
        try
        {
            rf2 = new RandomAccessFileOrArray(Rf);
            rf2.ReOpen();
            var b = new byte[rf2.Length];
            rf2.ReadFully(b);
            return b;
        }
        finally
        {
            try
            {
                if (rf2 != null)
                {
                    rf2.Close();
                }
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///     Gets a glyph width.
    /// </summary>
    /// <param name="glyph">the glyph to get the width of</param>
    /// <returns>the width of the glyph in normalized 1000 units</returns>
    protected int GetGlyphWidth(int glyph)
    {
        if (glyph >= GlyphWidths.Length)
        {
            glyph = GlyphWidths.Length - 1;
        }

        return GlyphWidths[glyph];
    }

    protected override int[] GetRawCharBBox(int c, string name)
    {
        INullValueDictionary<int, int[]> map = null;
        if (name == null || Cmap31 == null)
        {
            map = Cmap10;
        }
        else
        {
            map = Cmap31;
        }

        if (map == null)
        {
            return null;
        }

        var metric = map[c];
        if (metric == null || Bboxes == null)
        {
            return null;
        }

        return Bboxes[metric[0]];
    }

    /// <summary>
    ///     Reads the glyphs widths. The widths are extracted from the table 'hmtx'.
    ///     The glyphs are normalized to 1000 units.
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    /// </summary>
    protected void ReadGlyphWidths()
    {
        int[] tableLocation;
        tableLocation = Tables["hmtx"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'hmtx' does not exist in " + FileName + Style);
        }

        Rf.Seek(tableLocation[0]);
        GlyphWidths = new int[Hhea.NumberOfHMetrics];
        for (var k = 0; k < Hhea.NumberOfHMetrics; ++k)
        {
            GlyphWidths[k] = Rf.ReadUnsignedShort() * 1000 / Head.UnitsPerEm;
            Rf.ReadUnsignedShort();
        }
    }

    /// <summary>
    ///     Reads a  string  from the font file as bytes using the Cp1252
    ///     encoding.
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <param name="length">the length of bytes to read</param>
    /// <returns>the  string  read</returns>
    protected string ReadStandardString(int length)
    {
        var buf = new byte[length];
        Rf.ReadFully(buf);
        return EncodingsRegistry.GetEncoding(1252).GetString(buf);
    }

    /// <summary>
    ///     Reads a Unicode  string  from the font file. Each character is
    ///     represented by two bytes.
    ///     characters
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <param name="length">the length of bytes to read. The  string  will have  length /2</param>
    /// <returns>the  string  read</returns>
    protected string ReadUnicodeString(int length)
    {
        var buf = new StringBuilder();
        length /= 2;
        for (var k = 0; k < length; ++k)
        {
            buf.Append(Rf.ReadChar());
        }

        return buf.ToString();
    }

    private void readBbox()
    {
        int[] tableLocation;
        tableLocation = Tables["head"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'head' does not exist in " + FileName + Style);
        }

        Rf.Seek(tableLocation[0] + TrueTypeFontSubSet.HeadLocaFormatOffset);
        var locaShortTable = Rf.ReadUnsignedShort() == 0;
        tableLocation = Tables["loca"];
        if (tableLocation == null)
        {
            return;
        }

        Rf.Seek(tableLocation[0]);
        int[] locaTable;
        if (locaShortTable)
        {
            var entries = tableLocation[1] / 2;
            locaTable = new int[entries];
            for (var k = 0; k < entries; ++k)
            {
                locaTable[k] = Rf.ReadUnsignedShort() * 2;
            }
        }
        else
        {
            var entries = tableLocation[1] / 4;
            locaTable = new int[entries];
            for (var k = 0; k < entries; ++k)
            {
                locaTable[k] = Rf.ReadInt();
            }
        }

        tableLocation = Tables["glyf"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'glyf' does not exist in " + FileName + Style);
        }

        var tableGlyphOffset = tableLocation[0];
        Bboxes = new int[locaTable.Length - 1][];
        for (var glyph = 0; glyph < locaTable.Length - 1; ++glyph)
        {
            var start = locaTable[glyph];
            if (start != locaTable[glyph + 1])
            {
                Rf.Seek(tableGlyphOffset + start + 2);
                Bboxes[glyph] = new[]
                                {
                                    Rf.ReadShort() * 1000 / Head.UnitsPerEm,
                                    Rf.ReadShort() * 1000 / Head.UnitsPerEm,
                                    Rf.ReadShort() * 1000 / Head.UnitsPerEm,
                                    Rf.ReadShort() * 1000 / Head.UnitsPerEm,
                                };
            }
        }
    }

    /// <summary>
    ///     The components of table 'head'.
    /// </summary>
    protected class FontHeader
    {
        /// <summary>
        ///     A variable.
        /// </summary>
        internal int Flags;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int MacStyle;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UnitsPerEm;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short XMax;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short XMin;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YMax;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YMin;
    }

    /// <summary>
    ///     The components of table 'hhea'.
    /// </summary>
    protected class HorizontalHeader
    {
        /// <summary>
        ///     A variable.
        /// </summary>
        internal int AdvanceWidthMax;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short Ascender;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short CaretSlopeRise;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short CaretSlopeRun;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short Descender;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short LineGap;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short MinLeftSideBearing;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short MinRightSideBearing;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int NumberOfHMetrics;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short XMaxExtent;
    }

    /// <summary>
    ///     The components of table 'OS/2'.
    /// </summary>
    protected class WindowsMetrics
    {
        /// <summary>
        ///     A variable.
        /// </summary>
        internal readonly byte[] AchVendId = new byte[4];

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int FsSelection;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short FsType;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal readonly byte[] Panose = new byte[10];

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int SCapHeight;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short SFamilyClass;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short STypoAscender;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short STypoDescender;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short STypoLineGap;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UlCodePageRange1;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UlCodePageRange2;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UsFirstCharIndex;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UsLastCharIndex;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UsWeightClass;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UsWidthClass;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UsWinAscent;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal int UsWinDescent;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short XAvgCharWidth;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YStrikeoutPosition;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YStrikeoutSize;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YSubscriptXOffset;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YSubscriptXSize;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YSubscriptYOffset;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YSubscriptYSize;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YSuperscriptXOffset;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YSuperscriptXSize;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YSuperscriptYOffset;

        /// <summary>
        ///     A variable.
        /// </summary>
        internal short YSuperscriptYSize;
    }
}