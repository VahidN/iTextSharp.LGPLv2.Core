using System.Text;

namespace iTextSharp.text.pdf;

public class CffFont
{
    internal static readonly string[] OperatorNames =
    {
        "version", "Notice", "FullName", "FamilyName",
        "Weight", "FontBBox", "BlueValues", "OtherBlues",
        "FamilyBlues", "FamilyOtherBlues", "StdHW", "StdVW",
        "UNKNOWN_12", "UniqueID", "XUID", "charset",
        "Encoding", "CharStrings", "Private", "Subrs",
        "defaultWidthX", "nominalWidthX", "UNKNOWN_22", "UNKNOWN_23",
        "UNKNOWN_24", "UNKNOWN_25", "UNKNOWN_26", "UNKNOWN_27",
        "UNKNOWN_28", "UNKNOWN_29", "UNKNOWN_30", "UNKNOWN_31",
        "Copyright", "isFixedPitch", "ItalicAngle", "UnderlinePosition",
        "UnderlineThickness", "PaintType", "CharstringType", "FontMatrix",
        "StrokeWidth", "BlueScale", "BlueShift", "BlueFuzz",
        "StemSnapH", "StemSnapV", "ForceBold", "UNKNOWN_12_15",
        "UNKNOWN_12_16", "LanguageGroup", "ExpansionFactor",
        "initialRandomSeed",
        "SyntheticBase", "PostScript", "BaseFontName", "BaseFontBlend",
        "UNKNOWN_12_24", "UNKNOWN_12_25", "UNKNOWN_12_26", "UNKNOWN_12_27",
        "UNKNOWN_12_28", "UNKNOWN_12_29", "ROS", "CIDFontVersion",
        "CIDFontRevision", "CIDFontType", "CIDCount", "UIDBase",
        "FDArray", "FDSelect", "FontName",
    };

    internal static readonly string[] StandardStrings =
    {
        // Automatically generated from Appendix A of the CFF specification; do
        // not edit. Size should be 391.
        ".notdef", "space", "exclam", "quotedbl", "numbersign", "dollar",
        "percent", "ampersand", "quoteright", "parenleft", "parenright",
        "asterisk", "plus", "comma", "hyphen", "period", "slash", "zero",
        "one",
        "two", "three", "four", "five", "six", "seven", "eight", "nine",
        "colon",
        "semicolon", "less", "equal", "greater", "question", "at", "A", "B",
        "C",
        "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q",
        "R",
        "S", "T", "U", "V", "W", "X", "Y", "Z", "bracketleft", "backslash",
        "bracketright", "asciicircum", "underscore", "quoteleft", "a", "b",
        "c",
        "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q",
        "r",
        "s", "t", "u", "v", "w", "x", "y", "z", "braceleft", "bar",
        "braceright",
        "asciitilde", "exclamdown", "cent", "sterling", "fraction", "yen",
        "florin", "section", "currency", "quotesingle", "quotedblleft",
        "guillemotleft", "guilsinglleft", "guilsinglright", "fi", "fl",
        "endash",
        "dagger", "daggerdbl", "periodcentered", "paragraph", "bullet",
        "quotesinglbase", "quotedblbase", "quotedblright", "guillemotright",
        "ellipsis", "perthousand", "questiondown", "grave", "acute",
        "circumflex",
        "tilde", "macron", "breve", "dotaccent", "dieresis", "ring",
        "cedilla",
        "hungarumlaut", "ogonek", "caron", "emdash", "AE", "ordfeminine",
        "Lslash",
        "Oslash", "OE", "ordmasculine", "ae", "dotlessi", "lslash", "oslash",
        "oe",
        "germandbls", "onesuperior", "logicalnot", "mu", "trademark", "Eth",
        "onehalf", "plusminus", "Thorn", "onequarter", "divide", "brokenbar",
        "degree", "thorn", "threequarters", "twosuperior", "registered",
        "minus",
        "eth", "multiply", "threesuperior", "copyright", "Aacute",
        "Acircumflex",
        "Adieresis", "Agrave", "Aring", "Atilde", "Ccedilla", "Eacute",
        "Ecircumflex", "Edieresis", "Egrave", "Iacute", "Icircumflex",
        "Idieresis",
        "Igrave", "Ntilde", "Oacute", "Ocircumflex", "Odieresis", "Ograve",
        "Otilde", "Scaron", "Uacute", "Ucircumflex", "Udieresis", "Ugrave",
        "Yacute", "Ydieresis", "Zcaron", "aacute", "acircumflex",
        "adieresis",
        "agrave", "aring", "atilde", "ccedilla", "eacute", "ecircumflex",
        "edieresis", "egrave", "iacute", "icircumflex", "idieresis",
        "igrave",
        "ntilde", "oacute", "ocircumflex", "odieresis", "ograve", "otilde",
        "scaron", "uacute", "ucircumflex", "udieresis", "ugrave", "yacute",
        "ydieresis", "zcaron", "exclamsmall", "Hungarumlautsmall",
        "dollaroldstyle", "dollarsuperior", "ampersandsmall", "Acutesmall",
        "parenleftsuperior", "parenrightsuperior", "twodotenleader",
        "onedotenleader", "zerooldstyle", "oneoldstyle", "twooldstyle",
        "threeoldstyle", "fouroldstyle", "fiveoldstyle", "sixoldstyle",
        "sevenoldstyle", "eightoldstyle", "nineoldstyle", "commasuperior",
        "threequartersemdash", "periodsuperior", "questionsmall",
        "asuperior",
        "bsuperior", "centsuperior", "dsuperior", "esuperior", "isuperior",
        "lsuperior", "msuperior", "nsuperior", "osuperior", "rsuperior",
        "ssuperior", "tsuperior", "ff", "ffi", "ffl", "parenleftinferior",
        "parenrightinferior", "Circumflexsmall", "hyphensuperior",
        "Gravesmall",
        "Asmall", "Bsmall", "Csmall", "Dsmall", "Esmall", "Fsmall", "Gsmall",
        "Hsmall", "Ismall", "Jsmall", "Ksmall", "Lsmall", "Msmall", "Nsmall",
        "Osmall", "Psmall", "Qsmall", "Rsmall", "Ssmall", "Tsmall", "Usmall",
        "Vsmall", "Wsmall", "Xsmall", "Ysmall", "Zsmall", "colonmonetary",
        "onefitted", "rupiah", "Tildesmall", "exclamdownsmall",
        "centoldstyle",
        "Lslashsmall", "Scaronsmall", "Zcaronsmall", "Dieresissmall",
        "Brevesmall",
        "Caronsmall", "Dotaccentsmall", "Macronsmall", "figuredash",
        "hypheninferior", "Ogoneksmall", "Ringsmall", "Cedillasmall",
        "questiondownsmall", "oneeighth", "threeeighths", "fiveeighths",
        "seveneighths", "onethird", "twothirds", "zerosuperior",
        "foursuperior",
        "fivesuperior", "sixsuperior", "sevensuperior", "eightsuperior",
        "ninesuperior", "zeroinferior", "oneinferior", "twoinferior",
        "threeinferior", "fourinferior", "fiveinferior", "sixinferior",
        "seveninferior", "eightinferior", "nineinferior", "centinferior",
        "dollarinferior", "periodinferior", "commainferior", "Agravesmall",
        "Aacutesmall", "Acircumflexsmall", "Atildesmall", "Adieresissmall",
        "Aringsmall", "AEsmall", "Ccedillasmall", "Egravesmall",
        "Eacutesmall",
        "Ecircumflexsmall", "Edieresissmall", "Igravesmall", "Iacutesmall",
        "Icircumflexsmall", "Idieresissmall", "Ethsmall", "Ntildesmall",
        "Ogravesmall", "Oacutesmall", "Ocircumflexsmall", "Otildesmall",
        "Odieresissmall", "OEsmall", "Oslashsmall", "Ugravesmall",
        "Uacutesmall",
        "Ucircumflexsmall", "Udieresissmall", "Yacutesmall", "Thornsmall",
        "Ydieresissmall", "001.000", "001.001", "001.002", "001.003",
        "Black",
        "Bold", "Book", "Light", "Medium", "Regular", "Roman", "Semibold",
    };

    private readonly int _offSize;

    protected int ArgCount;

    protected object[] Args = new object[48];

    /// <summary>
    ///     A random Access File or an array
    ///     (contributed by orly manor)
    /// </summary>
    protected RandomAccessFileOrArray Buf;

    /// <summary>
    ///     Changed from private to protected
    /// </summary>
    protected Font[] Fonts;

    protected int GsubrIndexOffset;

    protected int[] GsubrOffsets;

    protected string Key;

    protected int NameIndexOffset;

    protected int[] NameOffsets;

    internal int NextIndexOffset;

    protected int StringIndexOffset;

    protected int[] StringOffsets;

    protected int TopdictIndexOffset;

    protected int[] TopdictOffsets;

    public CffFont(RandomAccessFileOrArray inputbuffer)
    {
        //System.err.Println("CFF: nStdString = "+standardStrings.length);
        Buf = inputbuffer;
        Seek(0);

        int major, minor;
        major = GetCard8();
        minor = GetCard8();

        //System.err.Println("CFF Major-Minor = "+major+"-"+minor);

        int hdrSize = GetCard8();

        _offSize = GetCard8();

        //System.err.Println("offSize = "+offSize);

        //int count, indexOffSize, indexOffset, nextOffset;

        NameIndexOffset = hdrSize;
        NameOffsets = GetIndex(NameIndexOffset);
        TopdictIndexOffset = NameOffsets[NameOffsets.Length - 1];
        TopdictOffsets = GetIndex(TopdictIndexOffset);
        StringIndexOffset = TopdictOffsets[TopdictOffsets.Length - 1];
        StringOffsets = GetIndex(StringIndexOffset);
        GsubrIndexOffset = StringOffsets[StringOffsets.Length - 1];
        GsubrOffsets = GetIndex(GsubrIndexOffset);

        Fonts = new Font[NameOffsets.Length - 1];

        // now get the name index

        /*
        names             = new String[nfonts];
        privateOffset     = new int[nfonts];
        charsetOffset     = new int[nfonts];
        encodingOffset    = new int[nfonts];
        charstringsOffset = new int[nfonts];
        fdarrayOffset     = new int[nfonts];
        fdselectOffset    = new int[nfonts];
        */

        for (var j = 0; j < NameOffsets.Length - 1; j++)
        {
            Fonts[j] = new Font();
            Seek(NameOffsets[j]);
            Fonts[j].Name = "";
            for (var k = NameOffsets[j]; k < NameOffsets[j + 1]; k++)
            {
                Fonts[j].Name += GetCard8();
            }
            //System.err.Println("name["+j+"]=<"+fonts[j].name+">");
        }

        // string index

        //strings = new String[stringOffsets.length-1];
        /*
        System.err.Println("std strings = "+standardStrings.length);
        System.err.Println("fnt strings = "+(stringOffsets.length-1));
        for (char j=0; j<standardStrings.length+(stringOffsets.length-1); j++) {
            //Seek(stringOffsets[j]);
            //strings[j] = "";
            //for (int k=stringOffsets[j]; k<stringOffsets[j+1]; k++) {
            //    strings[j] += (char)getCard8();
            //}
            System.err.Println("j="+(int)j+" <? "+(standardStrings.length+(stringOffsets.length-1)));
            System.err.Println("strings["+(int)j+"]=<"+getString(j)+">");
        }
        */

        // top dict

        for (var j = 0; j < TopdictOffsets.Length - 1; j++)
        {
            Seek(TopdictOffsets[j]);
            while (GetPosition() < TopdictOffsets[j + 1])
            {
                GetDictItem();
                if (Key == "FullName")
                {
                    //System.err.Println("getting fullname sid = "+((Integer)args[0]).IntValue);
                    Fonts[j].FullName = GetString((char)(int)Args[0]);
                    //System.err.Println("got it");
                }
                else if (Key == "ROS")
                {
                    Fonts[j].IsCid = true;
                }
                else if (Key == "Private")
                {
                    Fonts[j].PrivateLength = (int)Args[0];
                    Fonts[j].PrivateOffset = (int)Args[1];
                }
                else if (Key == "charset")
                {
                    Fonts[j].CharsetOffset = (int)Args[0];
                }
                else if (Key == "Encoding")
                {
                    Fonts[j].EncodingOffset = (int)Args[0];
                    ReadEncoding(Fonts[j].EncodingOffset);
                }
                else if (Key == "CharStrings")
                {
                    Fonts[j].CharstringsOffset = (int)Args[0];
                    var p = GetPosition();
                    Fonts[j].CharstringsOffsets = GetIndex(Fonts[j].CharstringsOffset);
                    Seek(p);
                }
                else if (Key == "FDArray")
                {
                    Fonts[j].FdarrayOffset = (int)Args[0];
                }
                else if (Key == "FDSelect")
                {
                    Fonts[j].FdselectOffset = (int)Args[0];
                }
                else if (Key == "CharstringType")
                {
                    Fonts[j].CharstringType = (int)Args[0];
                }
            }

            // private dict
            if (Fonts[j].PrivateOffset >= 0)
            {
                //System.err.Println("PRIVATE::");
                Seek(Fonts[j].PrivateOffset);
                while (GetPosition() < Fonts[j].PrivateOffset + Fonts[j].PrivateLength)
                {
                    GetDictItem();
                    if (Key == "Subrs")
                        //Add the private offset to the lsubrs since the offset is
                        // relative to the begining of the PrivateDict
                    {
                        Fonts[j].PrivateSubrs = (int)Args[0] + Fonts[j].PrivateOffset;
                    }
                }
            }

            // fdarray index
            if (Fonts[j].FdarrayOffset >= 0)
            {
                var fdarrayOffsets = GetIndex(Fonts[j].FdarrayOffset);

                Fonts[j].FdprivateOffsets = new int[fdarrayOffsets.Length - 1];
                Fonts[j].FdprivateLengths = new int[fdarrayOffsets.Length - 1];

                //System.err.Println("FD Font::");

                for (var k = 0; k < fdarrayOffsets.Length - 1; k++)
                {
                    Seek(fdarrayOffsets[k]);
                    while (GetPosition() < fdarrayOffsets[k + 1])
                    {
                        GetDictItem();
                    }

                    if (Key == "Private")
                    {
                        Fonts[j].FdprivateLengths[k] = (int)Args[0];
                        Fonts[j].FdprivateOffsets[k] = (int)Args[1];
                    }
                }
            }
        }
        //System.err.Println("CFF: done");
    }

    public bool Exists(string fontName)
    {
        if (fontName == null)
        {
            throw new ArgumentNullException(nameof(fontName));
        }

        int j;
        for (j = 0; j < Fonts.Length; j++)
        {
            if (fontName.Equals(Fonts[j].Name, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    public byte[] GetCid(string fontName)
        //throws java.io.FileNotFoundException
    {
        if (fontName == null)
        {
            throw new ArgumentNullException(nameof(fontName));
        }

        int j;
        for (j = 0; j < Fonts.Length; j++)
        {
            if (fontName.Equals(Fonts[j].Name, StringComparison.Ordinal))
            {
                break;
            }
        }

        if (j == Fonts.Length)
        {
            return null;
        }

        List<Item> l = new();

        // copy the header

        Seek(0);

        int major = GetCard8();
        int minor = GetCard8();
        int hdrSize = GetCard8();
        int offSize = GetCard8();
        NextIndexOffset = hdrSize;

        l.Add(new RangeItem(Buf, 0, hdrSize));

        int nglyphs = -1, nstrings = -1;
        if (!Fonts[j].IsCid)
        {
            // count the glyphs
            Seek(Fonts[j].CharstringsOffset);
            nglyphs = GetCard16();
            Seek(StringIndexOffset);
            nstrings = GetCard16() + StandardStrings.Length;
            //System.err.Println("number of glyphs = "+nglyphs);
        }

        // create a name index

        l.Add(new UInt16Item((char)1)); // count
        l.Add(new UInt8Item((char)1)); // offSize
        l.Add(new UInt8Item((char)1)); // first offset
        l.Add(new UInt8Item((char)(1 + Fonts[j].Name.Length)));
        l.Add(new StringItem(Fonts[j].Name));

        // create the topdict Index


        l.Add(new UInt16Item((char)1)); // count
        l.Add(new UInt8Item((char)2)); // offSize
        l.Add(new UInt16Item((char)1)); // first offset
        OffsetItem topdictIndex1Ref = new IndexOffsetItem(2);
        l.Add(topdictIndex1Ref);
        var topdictBase = new IndexBaseItem();
        l.Add(topdictBase);

        /*
        int maxTopdictLen = (topdictOffsets[j+1]-topdictOffsets[j])
                            + 9*2 // at most 9 new keys
                            + 8*5 // 8 new integer arguments
                            + 3*2;// 3 new SID arguments
        */

        //int    topdictNext = 0;
        //byte[] topdict = new byte[maxTopdictLen];

        OffsetItem charsetRef = new DictOffsetItem();
        OffsetItem charstringsRef = new DictOffsetItem();
        OffsetItem fdarrayRef = new DictOffsetItem();
        OffsetItem fdselectRef = new DictOffsetItem();

        if (!Fonts[j].IsCid)
        {
            // create a ROS key
            l.Add(new DictNumberItem(nstrings));
            l.Add(new DictNumberItem(nstrings + 1));
            l.Add(new DictNumberItem(0));
            l.Add(new UInt8Item((char)12));
            l.Add(new UInt8Item((char)30));
            // create a CIDCount key
            l.Add(new DictNumberItem(nglyphs));
            l.Add(new UInt8Item((char)12));
            l.Add(new UInt8Item((char)34));
            // What about UIDBase (12,35)? Don't know what is it.
            // I don't think we need FontName; the font I looked at didn't have it.
        }

        // create an FDArray key
        l.Add(fdarrayRef);
        l.Add(new UInt8Item((char)12));
        l.Add(new UInt8Item((char)36));
        // create an FDSelect key
        l.Add(fdselectRef);
        l.Add(new UInt8Item((char)12));
        l.Add(new UInt8Item((char)37));
        // create an charset key
        l.Add(charsetRef);
        l.Add(new UInt8Item((char)15));
        // create a CharStrings key
        l.Add(charstringsRef);
        l.Add(new UInt8Item((char)17));

        Seek(TopdictOffsets[j]);
        while (GetPosition() < TopdictOffsets[j + 1])
        {
            var p1 = GetPosition();
            GetDictItem();
            var p2 = GetPosition();
            if (Key == "Encoding"
                || Key == "Private"
                || Key == "FDSelect"
                || Key == "FDArray"
                || Key == "charset"
                || Key == "CharStrings"
               )
            {
                // just drop them
            }
            else
            {
                l.Add(new RangeItem(Buf, p1, p2 - p1));
            }
        }

        l.Add(new IndexMarkerItem(topdictIndex1Ref, topdictBase));

        // Copy the string index and append new strings.
        // We need 3 more strings: Registry, Ordering, and a FontName for one FD.
        // The total length is at most "Adobe"+"Identity"+63 = 76

        if (Fonts[j].IsCid)
        {
            l.Add(GetEntireIndexRange(StringIndexOffset));
        }
        else
        {
            var fdFontName = Fonts[j].Name + "-OneRange";
            if (fdFontName.Length > 127)
            {
                fdFontName = fdFontName.Substring(0, 127);
            }

            var extraStrings = "Adobe" + "Identity" + fdFontName;

            var origStringsLen = StringOffsets[StringOffsets.Length - 1]
                                 - StringOffsets[0];
            var stringsBaseOffset = StringOffsets[0] - 1;

            byte stringsIndexOffSize;
            if (origStringsLen + extraStrings.Length <= 0xff)
            {
                stringsIndexOffSize = 1;
            }
            else if (origStringsLen + extraStrings.Length <= 0xffff)
            {
                stringsIndexOffSize = 2;
            }
            else if (origStringsLen + extraStrings.Length <= 0xffffff)
            {
                stringsIndexOffSize = 3;
            }
            else
            {
                stringsIndexOffSize = 4;
            }

            l.Add(new UInt16Item((char)(StringOffsets.Length - 1 + 3))); // count
            l.Add(new UInt8Item((char)stringsIndexOffSize)); // offSize
            for (var i = 0; i < StringOffsets.Length; i++)
            {
                l.Add(new IndexOffsetItem(stringsIndexOffSize,
                                          StringOffsets[i] - stringsBaseOffset));
            }

            var currentStringsOffset = StringOffsets[StringOffsets.Length - 1]
                                       - stringsBaseOffset;
            //l.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
            currentStringsOffset += "Adobe".Length;
            l.Add(new IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
            currentStringsOffset += "Identity".Length;
            l.Add(new IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
            currentStringsOffset += fdFontName.Length;
            l.Add(new IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));

            l.Add(new RangeItem(Buf, StringOffsets[0], origStringsLen));
            l.Add(new StringItem(extraStrings));
        }

        // copy the global subroutine index

        l.Add(GetEntireIndexRange(GsubrIndexOffset));

        // deal with fdarray, fdselect, and the font descriptors

        if (Fonts[j].IsCid)
        {
            // copy the FDArray, FDSelect, charset
        }
        else
        {
            // create FDSelect
            l.Add(new MarkerItem(fdselectRef));
            l.Add(new UInt8Item((char)3)); // format identifier
            l.Add(new UInt16Item((char)1)); // nRanges

            l.Add(new UInt16Item((char)0)); // Range[0].firstGlyph
            l.Add(new UInt8Item((char)0)); // Range[0].fd

            l.Add(new UInt16Item((char)nglyphs)); // sentinel

            // recreate a new charset
            // This format is suitable only for fonts without subsetting

            l.Add(new MarkerItem(charsetRef));
            l.Add(new UInt8Item((char)2)); // format identifier

            l.Add(new UInt16Item((char)1)); // first glyph in range (ignore .notdef)
            l.Add(new UInt16Item((char)(nglyphs - 1))); // nLeft
            // now all are covered, the data structure is complete.

            // create a font dict index (fdarray)

            l.Add(new MarkerItem(fdarrayRef));
            l.Add(new UInt16Item((char)1));
            l.Add(new UInt8Item((char)1)); // offSize
            l.Add(new UInt8Item((char)1)); // first offset

            OffsetItem privateIndex1Ref = new IndexOffsetItem(1);
            l.Add(privateIndex1Ref);
            var privateBase = new IndexBaseItem();
            l.Add(privateBase);

            // looking at the PS that acrobat generates from a PDF with
            // a CFF opentype font embeded with an identity-H encoding,
            // it seems that it does not need a FontName.
            //l.Add(new DictNumberItem((standardStrings.length+(stringOffsets.length-1)+2)));
            //l.Add(new UInt8Item((char)12));
            //l.Add(new UInt8Item((char)38)); // FontName

            l.Add(new DictNumberItem(Fonts[j].PrivateLength));
            OffsetItem privateRef = new DictOffsetItem();
            l.Add(privateRef);
            l.Add(new UInt8Item((char)18)); // Private

            l.Add(new IndexMarkerItem(privateIndex1Ref, privateBase));

            // copy the private index & local subroutines

            l.Add(new MarkerItem(privateRef));
            // copy the private dict and the local subroutines.
            // the length of the private dict seems to NOT include
            // the local subroutines.
            l.Add(new RangeItem(Buf, Fonts[j].PrivateOffset, Fonts[j].PrivateLength));
            if (Fonts[j].PrivateSubrs >= 0)
            {
                //System.err.Println("has subrs="+fonts[j].privateSubrs+" ,len="+fonts[j].privateLength);
                l.Add(GetEntireIndexRange(Fonts[j].PrivateSubrs));
            }
        }

        // copy the charstring index

        l.Add(new MarkerItem(charstringsRef));
        l.Add(GetEntireIndexRange(Fonts[j].CharstringsOffset));

        // now create the new CFF font

        var currentOffset = new int[1];
        currentOffset[0] = 0;

        foreach (var item in l)
        {
            item.Increment(currentOffset);
        }

        foreach (var item in l)
        {
            item.Xref();
        }

        var size = currentOffset[0];
        var b = new byte[size];

        foreach (var item in l)
        {
            item.Emit(b);
        }

        return b;
    }

    public string[] GetNames()
    {
        var names = new string[Fonts.Length];
        for (var i = 0; i < Fonts.Length; i++)
        {
            names[i] = Fonts[i].Name;
        }

        return names;
    }

    /// <summary>
    ///     private String[] strings;
    /// </summary>
    public string GetString(char sid)
    {
        if (sid < StandardStrings.Length)
        {
            return StandardStrings[sid];
        }

        if (sid >= StandardStrings.Length + (StringOffsets.Length - 1))
        {
            return null;
        }

        var j = sid - StandardStrings.Length;
        var p = GetPosition();
        Seek(StringOffsets[j]);
        var s = new StringBuilder();
        for (var k = StringOffsets[j]; k < StringOffsets[j + 1]; k++)
        {
            s.Append(GetCard8());
        }

        Seek(p);
        return s.ToString();
    }

    /// <summary>
    ///     get a single CID font. The PDF architecture (1.4)
    ///     supports 16-bit strings only with CID CFF fonts, not
    ///     in Type-1 CFF fonts, so we convert the font to CID if
    ///     it is in the Type-1 format.
    ///     Two other tasks that we need to do are to select
    ///     only a single font from the CFF package (this again is
    ///     a PDF restriction) and to subset the CharStrings glyph
    ///     description.
    /// </summary>
    public bool IsCid(string fontName)
    {
        if (fontName == null)
        {
            throw new ArgumentNullException(nameof(fontName));
        }

        int j;
        for (j = 0; j < Fonts.Length; j++)
        {
            if (fontName.Equals(Fonts[j].Name, StringComparison.Ordinal))
            {
                return Fonts[j].IsCid;
            }
        }

        return false;
    }

    internal char GetCard16() => Buf.ReadChar();

    internal char GetCard8()
    {
        var i = Buf.ReadByte();
        return (char)(i & 0xff);
    }

    /// <summary>
    ///     read the offsets in the next index
    /// </summary>
    /// <summary>
    ///     data structure, convert to global
    /// </summary>
    /// <summary>
    ///     offsets, and return them.
    /// </summary>
    /// <summary>
    ///     Sets the nextIndexOffset.
    /// </summary>
    internal int[] GetIndex(int nextIndexOffset)
    {
        int count, indexOffSize;

        Seek(nextIndexOffset);
        count = GetCard16();
        var offsets = new int[count + 1];

        if (count == 0)
        {
            offsets[0] = -1;
            nextIndexOffset += 2;
            return offsets;
        }

        indexOffSize = GetCard8();

        for (var j = 0; j <= count; j++)
        {
            //nextIndexOffset = ofset to relative segment
            offsets[j] = nextIndexOffset
                         //2-> count in the index header. 1->offset size in index header
                         + 2 + 1
                         //offset array size * offset size
                         + (count + 1) * indexOffSize
                         //???zero <-> one base
                         - 1
                         // read object offset relative to object array base
                         + GetOffset(indexOffSize);
        }

        //nextIndexOffset = offsets[count];
        return offsets;
    }

    internal int GetInt() => Buf.ReadInt();

    internal int GetOffset(int offSize)
    {
        var offset = 0;
        for (var i = 0; i < offSize; i++)
        {
            offset *= 256;
            offset += GetCard8();
        }

        return offset;
    }

    internal int GetPosition() => Buf.FilePointer;

    internal short GetShort() => Buf.ReadShort();

    internal void ReadEncoding(int nextIndexOffset)
    {
        int format;
        Seek(nextIndexOffset);
        format = GetCard8();
    }

    internal void Seek(int offset)
    {
        Buf.Seek(offset);
    }

    protected void GetDictItem()
    {
        for (var i = 0; i < ArgCount; i++)
        {
            Args[i] = null;
        }

        ArgCount = 0;
        Key = null;
        var gotKey = false;

        while (!gotKey)
        {
            var b0 = GetCard8();
            if (b0 == 29)
            {
                var item = GetInt();
                Args[ArgCount] = item;
                ArgCount++;
                //System.err.Println(item+" ");
                continue;
            }

            if (b0 == 28)
            {
                var item = GetShort();
                Args[ArgCount] = (int)item;
                ArgCount++;
                //System.err.Println(item+" ");
                continue;
            }

            if (b0 >= 32 && b0 <= 246)
            {
                var item = (sbyte)(b0 - 139);
                Args[ArgCount] = (int)item;
                ArgCount++;
                //System.err.Println(item+" ");
                continue;
            }

            if (b0 >= 247 && b0 <= 250)
            {
                var b1 = GetCard8();
                var item = (short)((b0 - 247) * 256 + b1 + 108);
                Args[ArgCount] = (int)item;
                ArgCount++;
                //System.err.Println(item+" ");
                continue;
            }

            if (b0 >= 251 && b0 <= 254)
            {
                var b1 = GetCard8();
                var item = (short)(-(b0 - 251) * 256 - b1 - 108);
                Args[ArgCount] = (int)item;
                ArgCount++;
                //System.err.Println(item+" ");
                continue;
            }

            if (b0 == 30)
            {
                var item = "";
                var done = false;
                var buffer = (char)0;
                byte avail = 0;
                var nibble = 0;
                while (!done)
                {
                    // get a nibble
                    if (avail == 0)
                    {
                        buffer = GetCard8();
                        avail = 2;
                    }

                    if (avail == 1)
                    {
                        nibble = buffer / 16;
                        avail--;
                    }

                    if (avail == 2)
                    {
                        nibble = buffer % 16;
                        avail--;
                    }

                    switch (nibble)
                    {
                        case 0xa:
                            item += ".";
                            break;
                        case 0xb:
                            item += "E";
                            break;
                        case 0xc:
                            item += "E-";
                            break;
                        case 0xe:
                            item += "-";
                            break;
                        case 0xf:
                            done = true;
                            break;
                        default:
                            if (nibble >= 0 && nibble <= 9)
                            {
                                item += nibble.ToString(CultureInfo.InvariantCulture);
                            }
                            else
                            {
                                item += "<NIBBLE ERROR: " + nibble + ">";
                                done = true;
                            }

                            break;
                    }
                }

                Args[ArgCount] = item;
                ArgCount++;
                //System.err.Println(" real=["+item+"]");
                continue;
            }

            if (b0 <= 21)
            {
                gotKey = true;
                if (b0 != 12)
                {
                    Key = OperatorNames[b0];
                }
                else
                {
                    Key = OperatorNames[32 + GetCard8()];
                }

                //for (int i=0; i<arg_count; i++)
                //  System.err.Print(args[i].ToString()+" ");
                //System.err.Println(key+" ;");
            }
        }
    }

    /// <summary>
    ///     List items for the linked list that builds the new CID font.
    /// </summary>
    protected virtual RangeItem GetEntireIndexRange(int indexOffset)
    {
        Seek(indexOffset);
        int count = GetCard16();
        if (count == 0)
        {
            return new RangeItem(Buf, indexOffset, 2);
        }

        int indexOffSize = GetCard8();
        Seek(indexOffset + 2 + 1 + count * indexOffSize);
        var size = GetOffset(indexOffSize) - 1;
        return new RangeItem(Buf, indexOffset,
                             2 + 1 + (count + 1) * indexOffSize + size);
    }

    protected internal class DictNumberItem : Item
    {
        public int Size = 5;
        public int Value;
        public DictNumberItem(int value) => Value = value;

        /// <summary>
        ///     this is imcomplete!
        /// </summary>
        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (Size == 5)
            {
                buffer[MyOffset] = 29;
                buffer[MyOffset + 1] = (byte)((Value >> 24) & 0xff);
                buffer[MyOffset + 2] = (byte)((Value >> 16) & 0xff);
                buffer[MyOffset + 3] = (byte)((Value >> 8) & 0xff);
                buffer[MyOffset + 4] = (byte)((Value >> 0) & 0xff);
            }
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += Size;
        }
    }

    /// <summary>
    ///     an unknown offset in a dictionary for the list.
    ///     We will fix up the offset later; for now, assume it's large.
    /// </summary>
    protected internal class DictOffsetItem : OffsetItem
    {
        public int Size;
        public DictOffsetItem() => Size = 5;

        /// <summary>
        ///     this is incomplete!
        /// </summary>
        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (Size == 5)
            {
                buffer[MyOffset] = 29;
                buffer[MyOffset + 1] = (byte)((Value >> 24) & 0xff);
                buffer[MyOffset + 2] = (byte)((Value >> 16) & 0xff);
                buffer[MyOffset + 3] = (byte)((Value >> 8) & 0xff);
                buffer[MyOffset + 4] = (byte)((Value >> 0) & 0xff);
            }
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += Size;
        }
    }

    /// <summary>
    ///     a utility that creates a range item for an entire index
    /// </summary>
    protected internal class Font
    {
        public int[] Charset;
        public int CharsetLength;
        public int CharsetOffset = -1;
        public int CharstringsOffset = -1;
        public int[] CharstringsOffsets;
        public int CharstringType = 2;
        public int EncodingOffset = -1;
        public int FdArrayCount;
        public int FdarrayOffset = -1;
        public int[] FdArrayOffsets;
        public int FdArrayOffsize;
        public int[] FdprivateLengths;
        public int[] FdprivateOffsets;
        public int[] FdprivateSubrs;
        public int[] FdSelect;
        public int FdSelectFormat;

        public int FdSelectLength;

        // only if CID
        public int FdselectOffset = -1;

        public string FullName;
        public bool IsCid;

        public string Name;

        // only if CID
        public int Nglyphs;

        public int Nstrings;
        public int PrivateLength = -1;

        public int PrivateOffset = -1; // only if not CID

        // only if not CID
        public int PrivateSubrs = -1;
        public int[] PrivateSubrsOffset;
        public int[][] PrivateSubrsOffsetsArray;
        public int[] SubrsOffsets;
    }

    protected internal class IndexBaseItem : Item
    {
    }

    protected internal class IndexMarkerItem : Item
    {
        private readonly IndexBaseItem _indexBase;
        private readonly OffsetItem _offItem;

        public IndexMarkerItem(OffsetItem offItem, IndexBaseItem indexBase)
        {
            _offItem = offItem;
            _indexBase = indexBase;
        }

        public override void Xref()
        {
            //System.err.Println("index marker item, base="+indexBase.myOffset+" my="+this.myOffset);
            _offItem.Set(MyOffset - _indexBase.MyOffset + 1);
        }
    }

    /// <summary>
    ///     An index-offset item for the list.
    ///     The size denotes the required size in the CFF. A positive
    ///     value means that we need a specific size in bytes (for offset arrays)
    ///     and a negative value means that this is a dict item that uses a
    ///     variable-size representation.
    /// </summary>
    protected internal class IndexOffsetItem : OffsetItem
    {
        public int Size;

        public IndexOffsetItem(int size, int value)
        {
            Size = size;
            Value = value;
        }

        public IndexOffsetItem(int size) => Size = size;

        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            var i = 0;
            switch (Size)
            {
                case 4:
                    buffer[MyOffset + i] = (byte)((Value >> 24) & 0xff);
                    i++;
                    goto case 3;
                case 3:
                    buffer[MyOffset + i] = (byte)((Value >> 16) & 0xff);
                    i++;
                    goto case 2;
                case 2:
                    buffer[MyOffset + i] = (byte)((Value >> 8) & 0xff);
                    i++;
                    goto case 1;
                case 1:
                    buffer[MyOffset + i] = (byte)((Value >> 0) & 0xff);
                    i++;
                    break;
            }
            /*
            int mask = 0xff;
            for (int i=size-1; i>=0; i--) {
                buffer[myOffset+i] = (byte) (value & mask);
                mask <<= 8;
            }
            */
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += Size;
        }
    }

    protected internal abstract class Item
    {
        protected internal int MyOffset = -1;

        /// <summary>
        ///     Emit the byte stream for this item.
        /// </summary>
        public virtual void Emit(byte[] buffer)
        {
        }

        /// <summary>
        ///     remember the current offset and increment by item's size in bytes.
        /// </summary>
        public virtual void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            MyOffset = currentOffset[0];
        }

        /// <summary>
        ///     Fix up cross references to this item (applies only to markers).
        /// </summary>
        public virtual void Xref()
        {
        }
    }

    protected internal class MarkerItem : Item
    {
        private readonly OffsetItem _p;
        public MarkerItem(OffsetItem pointerToMarker) => _p = pointerToMarker;

        public override void Xref()
        {
            _p.Set(MyOffset);
        }
    }

    protected internal abstract class OffsetItem : Item
    {
        public int Value;

        /// <summary>
        ///     set the value of an offset item that was initially unknown.
        ///     It will be fixed up latex by a call to xref on some marker.
        /// </summary>
        public void Set(int offset)
        {
            Value = offset;
        }
    }


    /// <summary>
    ///     A range item.
    /// </summary>
    protected internal class RangeItem : Item
    {
        private readonly RandomAccessFileOrArray _buf;
        public int Offset, Length;

        public RangeItem(RandomAccessFileOrArray buf, int offset, int length)
        {
            Offset = offset;
            Length = length;
            _buf = buf;
        }

        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            //System.err.Println("range emit offset "+offset+" size="+length);
            _buf.Seek(Offset);
            for (var i = MyOffset; i < MyOffset + Length; i++)
            {
                buffer[i] = _buf.ReadByte();
            }
            //System.err.Println("finished range emit");
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += Length;
        }
    }

    protected internal class StringItem : Item
    {
        public string S;
        public StringItem(string s) => S = s;

        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            for (var i = 0; i < S.Length; i++)
            {
                buffer[MyOffset + i] = (byte)(S[i] & 0xff);
            }
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += S.Length;
        }
    }

    /// <summary>
    ///     @author orly manor
    ///     TODO To change the template for this generated type comment go to
    ///     Window - Preferences - Java - Code Generation - Code and Comments
    /// </summary>
    protected internal class SubrMarkerItem : Item
    {
        private readonly IndexBaseItem _indexBase;
        private readonly OffsetItem _offItem;

        public SubrMarkerItem(OffsetItem offItem, IndexBaseItem indexBase)
        {
            _offItem = offItem;
            _indexBase = indexBase;
        }

        public override void Xref()
        {
            //System.err.Println("index marker item, base="+indexBase.myOffset+" my="+this.myOffset);
            _offItem.Set(MyOffset - _indexBase.MyOffset);
        }
    }

    /// <summary>
    ///     Card24 item.
    /// </summary>
    protected internal class UInt16Item : Item
    {
        public char Value;
        public UInt16Item(char value) => Value = value;

        /// <summary>
        ///     this is incomplete!
        /// </summary>
        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer[MyOffset + 0] = (byte)((Value >> 8) & 0xff);
            buffer[MyOffset + 1] = (byte)((Value >> 0) & 0xff);
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += 2;
        }
    }

    protected internal class UInt24Item : Item
    {
        public int Value;
        public UInt24Item(int value) => Value = value;

        /// <summary>
        ///     this is incomplete!
        /// </summary>
        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer[MyOffset + 0] = (byte)((Value >> 16) & 0xff);
            buffer[MyOffset + 1] = (byte)((Value >> 8) & 0xff);
            buffer[MyOffset + 2] = (byte)((Value >> 0) & 0xff);
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += 3;
        }
    }

    /// <summary>
    ///     Card32 item.
    /// </summary>
    protected internal class UInt32Item : Item
    {
        public int Value;
        public UInt32Item(int value) => Value = value;

        /// <summary>
        ///     this is incomplete!
        /// </summary>
        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer[MyOffset + 0] = (byte)((Value >> 24) & 0xff);
            buffer[MyOffset + 1] = (byte)((Value >> 16) & 0xff);
            buffer[MyOffset + 2] = (byte)((Value >> 8) & 0xff);
            buffer[MyOffset + 3] = (byte)((Value >> 0) & 0xff);
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += 4;
        }
    }

    /// <summary>
    ///     A SID or Card16 item.
    /// </summary>
    /// <summary>
    ///     A Card8 item.
    /// </summary>
    protected internal class UInt8Item : Item
    {
        public char Value;
        public UInt8Item(char value) => Value = value;

        /// <summary>
        ///     this is incomplete!
        /// </summary>
        public override void Emit(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            buffer[MyOffset + 0] = (byte)((Value >> 0) & 0xff);
        }

        public override void Increment(int[] currentOffset)
        {
            if (currentOffset == null)
            {
                throw new ArgumentNullException(nameof(currentOffset));
            }

            base.Increment(currentOffset);
            currentOffset[0] += 1;
        }
    }
}