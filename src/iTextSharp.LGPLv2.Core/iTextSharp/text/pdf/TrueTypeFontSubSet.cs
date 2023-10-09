using System.util;
using iTextSharp.LGPLv2.Core.System.Encodings;

namespace iTextSharp.text.pdf;

/// <summary>
///     Subsets a True Type font by removing the unneeded glyphs from
///     the font.
///     @author  Paulo Soares (psoares@consiste.pt)
/// </summary>
internal class TrueTypeFontSubSet
{
    internal const int Arg1And2AreWords = 1;

    internal const int HeadLocaFormatOffset = 51;

    internal const int MoreComponents = 32;

    internal const int TableChecksum = 0;

    internal const int TableLength = 2;

    internal const int TableOffset = 1;
    internal const int WeHaveAnXAndYScale = 64;
    internal const int WeHaveAScale = 8;
    internal const int WeHaveATwoByTwo = 128;

    internal static readonly int[] EntrySelectors = { 0, 0, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4 };

    internal static readonly string[] TableNamesCmap =
    {
        "cmap", "cvt ", "fpgm", "glyf", "head",
        "hhea", "hmtx", "loca", "maxp", "prep",
    };

    internal static readonly string[] TableNamesExtra =
    {
        "OS/2", "cmap", "cvt ", "fpgm", "glyf", "head",
        "hhea", "hmtx", "loca", "maxp", "name, prep",
    };

    internal static readonly string[] TableNamesSimple =
    {
        "cvt ", "fpgm", "glyf", "head",
        "hhea", "hmtx", "loca", "maxp", "prep",
    };


    protected readonly int DirectoryOffset;

    /// <summary>
    ///     The file name.
    /// </summary>
    protected readonly string FileName;

    protected int FontPtr;

    protected int GlyfTableRealSize;

    protected readonly List<int> GlyphsInList;

    protected readonly INullValueDictionary<int, int[]> GlyphsUsed;

    protected readonly bool IncludeCmap;

    protected readonly bool IncludeExtras;

    protected bool LocaShortTable;

    protected int[] LocaTable;

    protected int LocaTableRealSize;

    protected byte[] NewGlyfTable;

    protected int[] NewLocaTable;

    protected byte[] NewLocaTableOut;

    protected byte[] OutFont;

    /// <summary>
    ///     The file in use.
    /// </summary>
    protected readonly RandomAccessFileOrArray Rf;

    /// <summary>
    ///     Contains the location of the several tables. The key is the name of
    ///     the table and the value is an  int[3]  where position 0
    ///     is the checksum, position 1 is the offset from the start of the file
    ///     and position 2 is the length of the table.
    /// </summary>
    protected INullValueDictionary<string, int[]> TableDirectory;

    protected int TableGlyphOffset;

    /// <summary>
    ///     Creates a new TrueTypeFontSubSet
    /// </summary>
    /// <param name="directoryOffset">The offset from the start of the file to the table directory</param>
    /// <param name="fileName">the file name of the font</param>
    /// <param name="rf"></param>
    /// <param name="glyphsUsed">the glyphs used</param>
    /// <param name="includeCmap"> true  if the table cmap is to be included in the generated font</param>
    /// <param name="includeExtras"></param>
    internal TrueTypeFontSubSet(string fileName, RandomAccessFileOrArray rf,
                                INullValueDictionary<int, int[]> glyphsUsed,
                                int directoryOffset, bool includeCmap, bool includeExtras)
    {
        FileName = fileName;
        Rf = rf;
        GlyphsUsed = glyphsUsed;
        IncludeCmap = includeCmap;
        IncludeExtras = includeExtras;
        DirectoryOffset = directoryOffset;
        GlyphsInList = new List<int>(glyphsUsed.Keys);
    }

    /// <summary>
    ///     Does the actual work of subsetting the font.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>the subset font</returns>
    internal byte[] Process()
    {
        try
        {
            Rf.ReOpen();
            CreateTableDirectory();
            ReadLoca();
            FlatGlyphs();
            CreateNewGlyphTables();
            LocaTobytes();
            AssembleFont();
            return OutFont;
        }
        finally
        {
            try
            {
                Rf.Close();
            }
            catch
            {
                // empty on purpose
            }
        }
    }

    protected void AssembleFont()
    {
        int[] tableLocation;
        var fullFontSize = 0;
        string[] tableNames;
        if (IncludeExtras)
        {
            tableNames = TableNamesExtra;
        }
        else
        {
            if (IncludeCmap)
            {
                tableNames = TableNamesCmap;
            }
            else
            {
                tableNames = TableNamesSimple;
            }
        }

        var tablesUsed = 2;
        var len = 0;
        for (var k = 0; k < tableNames.Length; ++k)
        {
            var name = tableNames[k];
            if (name.Equals("glyf", StringComparison.Ordinal) ||
                name.Equals("loca", StringComparison.Ordinal))
            {
                continue;
            }

            tableLocation = TableDirectory[name];
            if (tableLocation == null)
            {
                continue;
            }

            ++tablesUsed;
            fullFontSize += (tableLocation[TableLength] + 3) & ~3;
        }

        fullFontSize += NewLocaTableOut.Length;
        fullFontSize += NewGlyfTable.Length;
        var iref = 16 * tablesUsed + 12;
        fullFontSize += iref;
        OutFont = new byte[fullFontSize];
        FontPtr = 0;
        WriteFontInt(0x00010000);
        WriteFontShort(tablesUsed);
        var selector = EntrySelectors[tablesUsed];
        WriteFontShort((1 << selector) * 16);
        WriteFontShort(selector);
        WriteFontShort((tablesUsed - (1 << selector)) * 16);
        for (var k = 0; k < tableNames.Length; ++k)
        {
            var name = tableNames[k];
            tableLocation = TableDirectory[name];
            if (tableLocation == null)
            {
                continue;
            }

            WriteFontString(name);
            if (name.Equals("glyf", StringComparison.Ordinal))
            {
                WriteFontInt(CalculateChecksum(NewGlyfTable));
                len = GlyfTableRealSize;
            }
            else if (name.Equals("loca", StringComparison.Ordinal))
            {
                WriteFontInt(CalculateChecksum(NewLocaTableOut));
                len = LocaTableRealSize;
            }
            else
            {
                WriteFontInt(tableLocation[TableChecksum]);
                len = tableLocation[TableLength];
            }

            WriteFontInt(iref);
            WriteFontInt(len);
            iref += (len + 3) & ~3;
        }

        for (var k = 0; k < tableNames.Length; ++k)
        {
            var name = tableNames[k];
            tableLocation = TableDirectory[name];
            if (tableLocation == null)
            {
                continue;
            }

            if (name.Equals("glyf", StringComparison.Ordinal))
            {
                Array.Copy(NewGlyfTable, 0, OutFont, FontPtr, NewGlyfTable.Length);
                FontPtr += NewGlyfTable.Length;
                NewGlyfTable = null;
            }
            else if (name.Equals("loca", StringComparison.Ordinal))
            {
                Array.Copy(NewLocaTableOut, 0, OutFont, FontPtr, NewLocaTableOut.Length);
                FontPtr += NewLocaTableOut.Length;
                NewLocaTableOut = null;
            }
            else
            {
                Rf.Seek(tableLocation[TableOffset]);
                Rf.ReadFully(OutFont, FontPtr, tableLocation[TableLength]);
                FontPtr += (tableLocation[TableLength] + 3) & ~3;
            }
        }
    }

    protected static int CalculateChecksum(byte[] b)
    {
        var len = b.Length / 4;
        var v0 = 0;
        var v1 = 0;
        var v2 = 0;
        var v3 = 0;
        var ptr = 0;
        for (var k = 0; k < len; ++k)
        {
            v3 += b[ptr++] & 0xff;
            v2 += b[ptr++] & 0xff;
            v1 += b[ptr++] & 0xff;
            v0 += b[ptr++] & 0xff;
        }

        return v0 + (v1 << 8) + (v2 << 16) + (v3 << 24);
    }

    protected void CheckGlyphComposite(int glyph)
    {
        var start = LocaTable[glyph];
        if (start == LocaTable[glyph + 1]) // no contour
        {
            return;
        }

        Rf.Seek(TableGlyphOffset + start);
        int numContours = Rf.ReadShort();
        if (numContours >= 0)
        {
            return;
        }

        Rf.SkipBytes(8);
        for (;;)
        {
            var flags = Rf.ReadUnsignedShort();
            var cGlyph = Rf.ReadUnsignedShort();
            if (!GlyphsUsed.ContainsKey(cGlyph))
            {
                GlyphsUsed[cGlyph] = null;
                GlyphsInList.Add(cGlyph);
            }

            if ((flags & MoreComponents) == 0)
            {
                return;
            }

            int skip;
            if ((flags & Arg1And2AreWords) != 0)
            {
                skip = 4;
            }
            else
            {
                skip = 2;
            }

            if ((flags & WeHaveAScale) != 0)
            {
                skip += 2;
            }
            else if ((flags & WeHaveAnXAndYScale) != 0)
            {
                skip += 4;
            }

            if ((flags & WeHaveATwoByTwo) != 0)
            {
                skip += 8;
            }

            Rf.SkipBytes(skip);
        }
    }

    protected void CreateNewGlyphTables()
    {
        NewLocaTable = new int[LocaTable.Length];
        var activeGlyphs = new int[GlyphsInList.Count];
        for (var k = 0; k < activeGlyphs.Length; ++k)
        {
            activeGlyphs[k] = GlyphsInList[k];
        }

        Array.Sort(activeGlyphs);
        var glyfSize = 0;
        for (var k = 0; k < activeGlyphs.Length; ++k)
        {
            var glyph = activeGlyphs[k];
            glyfSize += LocaTable[glyph + 1] - LocaTable[glyph];
        }

        GlyfTableRealSize = glyfSize;
        glyfSize = (glyfSize + 3) & ~3;
        NewGlyfTable = new byte[glyfSize];
        var glyfPtr = 0;
        var listGlyf = 0;
        for (var k = 0; k < NewLocaTable.Length; ++k)
        {
            NewLocaTable[k] = glyfPtr;
            if (listGlyf < activeGlyphs.Length && activeGlyphs[listGlyf] == k)
            {
                ++listGlyf;
                NewLocaTable[k] = glyfPtr;
                var start = LocaTable[k];
                var len = LocaTable[k + 1] - start;
                if (len > 0)
                {
                    Rf.Seek(TableGlyphOffset + start);
                    Rf.ReadFully(NewGlyfTable, glyfPtr, len);
                    glyfPtr += len;
                }
            }
        }
    }

    protected void CreateTableDirectory()
    {
        TableDirectory = new NullValueDictionary<string, int[]>();
        Rf.Seek(DirectoryOffset);
        var id = Rf.ReadInt();
        if (id != 0x00010000)
        {
            throw new DocumentException(FileName + " is not a true type file.");
        }

        var numTables = Rf.ReadUnsignedShort();
        Rf.SkipBytes(6);
        for (var k = 0; k < numTables; ++k)
        {
            var tag = ReadStandardString(4);
            var tableLocation = new int[3];
            tableLocation[TableChecksum] = Rf.ReadInt();
            tableLocation[TableOffset] = Rf.ReadInt();
            tableLocation[TableLength] = Rf.ReadInt();
            TableDirectory[tag] = tableLocation;
        }
    }

    protected void FlatGlyphs()
    {
        int[] tableLocation;
        tableLocation = TableDirectory["glyf"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'glyf' does not exist in " + FileName);
        }

        var glyph0 = 0;
        if (!GlyphsUsed.ContainsKey(glyph0))
        {
            GlyphsUsed[glyph0] = null;
            GlyphsInList.Add(glyph0);
        }

        TableGlyphOffset = tableLocation[TableOffset];
        for (var k = 0; k < GlyphsInList.Count; ++k)
        {
            var glyph = GlyphsInList[k];
            CheckGlyphComposite(glyph);
        }
    }

    protected void LocaTobytes()
    {
        if (LocaShortTable)
        {
            LocaTableRealSize = NewLocaTable.Length * 2;
        }
        else
        {
            LocaTableRealSize = NewLocaTable.Length * 4;
        }

        NewLocaTableOut = new byte[(LocaTableRealSize + 3) & ~3];
        OutFont = NewLocaTableOut;
        FontPtr = 0;
        for (var k = 0; k < NewLocaTable.Length; ++k)
        {
            if (LocaShortTable)
            {
                WriteFontShort(NewLocaTable[k] / 2);
            }
            else
            {
                WriteFontInt(NewLocaTable[k]);
            }
        }
    }

    protected void ReadLoca()
    {
        int[] tableLocation;
        tableLocation = TableDirectory["head"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'head' does not exist in " + FileName);
        }

        Rf.Seek(tableLocation[TableOffset] + HeadLocaFormatOffset);
        LocaShortTable = Rf.ReadUnsignedShort() == 0;
        tableLocation = TableDirectory["loca"];
        if (tableLocation == null)
        {
            throw new DocumentException("Table 'loca' does not exist in " + FileName);
        }

        Rf.Seek(tableLocation[TableOffset]);
        if (LocaShortTable)
        {
            var entries = tableLocation[TableLength] / 2;
            LocaTable = new int[entries];
            for (var k = 0; k < entries; ++k)
            {
                LocaTable[k] = Rf.ReadUnsignedShort() * 2;
            }
        }
        else
        {
            var entries = tableLocation[TableLength] / 4;
            LocaTable = new int[entries];
            for (var k = 0; k < entries; ++k)
            {
                LocaTable[k] = Rf.ReadInt();
            }
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

    protected void WriteFontInt(int n)
    {
        OutFont[FontPtr++] = (byte)(n >> 24);
        OutFont[FontPtr++] = (byte)(n >> 16);
        OutFont[FontPtr++] = (byte)(n >> 8);
        OutFont[FontPtr++] = (byte)n;
    }

    protected void WriteFontShort(int n)
    {
        OutFont[FontPtr++] = (byte)(n >> 8);
        OutFont[FontPtr++] = (byte)n;
    }

    protected void WriteFontString(string s)
    {
        var b = PdfEncodings.ConvertToBytes(s, BaseFont.WINANSI);
        Array.Copy(b, 0, OutFont, FontPtr, b.Length);
        FontPtr += b.Length;
    }
}