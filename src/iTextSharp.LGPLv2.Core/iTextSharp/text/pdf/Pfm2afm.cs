using System.Text;
using iTextSharp.LGPLv2.Core.System.Encodings;

namespace iTextSharp.text.pdf;

/// <summary>
///     Converts a PFM file into an AFM file.
/// </summary>
public sealed class Pfm2Afm
{
    private readonly Encoding _encoding;
    private readonly RandomAccessFileOrArray _inp;
    private readonly StreamWriter _outp;

    /// <summary>
    ///     Translate table from 1004 to psstd.  1004 is an extension of the
    ///     Windows translate table used in PM.
    /// </summary>
    private readonly int[] _win2PsStd =
    {
        0, 0, 0, 0, 197, 198, 199, 0, 202, 0, 205, 206, 207, 0, 0, 0, // 00
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // 10
        32, 33, 34, 35, 36, 37, 38, 169, 40, 41, 42, 43, 44, 45, 46, 47, // 20
        48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, // 30
        64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, // 40
        80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, // 50
        193, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110,
        111, // 60
        112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126,
        127, // 70
        0, 0, 184, 166, 185, 188, 178, 179, 195, 189, 0, 172, 234, 0, 0, 0, // 80
        0, 96, 0, 170, 186, 183, 177, 208, 196, 0, 0, 173, 250, 0, 0, 0, // 90
        0, 161, 162, 163, 168, 165, 0, 167, 200, 0, 227, 171, 0, 0, 0, 197, // A0
        0, 0, 0, 0, 194, 0, 182, 180, 203, 0, 235, 187, 0, 0, 0, 191, // B0
        0, 0, 0, 0, 0, 0, 225, 0, 0, 0, 0, 0, 0, 0, 0, 0, // C0
        0, 0, 0, 0, 0, 0, 0, 0, 233, 0, 0, 0, 0, 0, 0, 251, // D0
        0, 0, 0, 0, 0, 0, 241, 0, 0, 0, 0, 0, 0, 0, 0, 0, // E0
        0, 0, 0, 0, 0, 0, 0, 0, 249, 0, 0, 0, 0, 0, 0, 0, // F0
    };

    /// <summary>
    ///     Windows character names.  Give a name to the used locations
    ///     for when the all flag is specified.
    /// </summary>
    private readonly string[] _winChars =
    {
        "W00", /*   00    */
        "W01", /*   01    */
        "W02", /*   02    */
        "W03", /*   03    */
        "macron", /*   04    */
        "breve", /*   05    */
        "dotaccent", /*   06    */
        "W07", /*   07    */
        "ring", /*   08    */
        "W09", /*   09    */
        "W0a", /*   0a    */
        "W0b", /*   0b    */
        "W0c", /*   0c    */
        "W0d", /*   0d    */
        "W0e", /*   0e    */
        "W0f", /*   0f    */
        "hungarumlaut", /*   10    */
        "ogonek", /*   11    */
        "caron", /*   12    */
        "W13", /*   13    */
        "W14", /*   14    */
        "W15", /*   15    */
        "W16", /*   16    */
        "W17", /*   17    */
        "W18", /*   18    */
        "W19", /*   19    */
        "W1a", /*   1a    */
        "W1b", /*   1b    */
        "W1c", /*   1c    */
        "W1d", /*   1d    */
        "W1e", /*   1e    */
        "W1f", /*   1f    */
        "space", /*   20    */
        "exclam", /*   21    */
        "quotedbl", /*   22    */
        "numbersign", /*   23    */
        "dollar", /*   24    */
        "percent", /*   25    */
        "ampersand", /*   26    */
        "quotesingle", /*   27    */
        "parenleft", /*   28    */
        "parenright", /*   29    */
        "asterisk", /*   2A    */
        "plus", /*   2B    */
        "comma", /*   2C    */
        "hyphen", /*   2D    */
        "period", /*   2E    */
        "slash", /*   2F    */
        "zero", /*   30    */
        "one", /*   31    */
        "two", /*   32    */
        "three", /*   33    */
        "four", /*   34    */
        "five", /*   35    */
        "six", /*   36    */
        "seven", /*   37    */
        "eight", /*   38    */
        "nine", /*   39    */
        "colon", /*   3A    */
        "semicolon", /*   3B    */
        "less", /*   3C    */
        "equal", /*   3D    */
        "greater", /*   3E    */
        "question", /*   3F    */
        "at", /*   40    */
        "A", /*   41    */
        "B", /*   42    */
        "C", /*   43    */
        "D", /*   44    */
        "E", /*   45    */
        "F", /*   46    */
        "G", /*   47    */
        "H", /*   48    */
        "I", /*   49    */
        "J", /*   4A    */
        "K", /*   4B    */
        "L", /*   4C    */
        "M", /*   4D    */
        "N", /*   4E    */
        "O", /*   4F    */
        "P", /*   50    */
        "Q", /*   51    */
        "R", /*   52    */
        "S", /*   53    */
        "T", /*   54    */
        "U", /*   55    */
        "V", /*   56    */
        "W", /*   57    */
        "X", /*   58    */
        "Y", /*   59    */
        "Z", /*   5A    */
        "bracketleft", /*   5B    */
        "backslash", /*   5C    */
        "bracketright", /*   5D    */
        "asciicircum", /*   5E    */
        "underscore", /*   5F    */
        "grave", /*   60    */
        "a", /*   61    */
        "b", /*   62    */
        "c", /*   63    */
        "d", /*   64    */
        "e", /*   65    */
        "f", /*   66    */
        "g", /*   67    */
        "h", /*   68    */
        "i", /*   69    */
        "j", /*   6A    */
        "k", /*   6B    */
        "l", /*   6C    */
        "m", /*   6D    */
        "n", /*   6E    */
        "o", /*   6F    */
        "p", /*   70    */
        "q", /*   71    */
        "r", /*   72    */
        "s", /*   73    */
        "t", /*   74    */
        "u", /*   75    */
        "v", /*   76    */
        "w", /*   77    */
        "x", /*   78    */
        "y", /*   79    */
        "z", /*   7A    */
        "braceleft", /*   7B    */
        "bar", /*   7C    */
        "braceright", /*   7D    */
        "asciitilde", /*   7E    */
        "W7f", /*   7F    */
        "euro", /*   80    */
        "W81", /*   81    */
        "quotesinglbase", /*   82    */
        "florin", /*   83    */
        "quotedblbase", /*   84    */
        "ellipsis", /*   85    */
        "dagger", /*   86    */
        "daggerdbl", /*   87    */
        "circumflex", /*   88    */
        "perthousand", /*   89    */
        "Scaron", /*   8A    */
        "guilsinglleft", /*   8B    */
        "OE", /*   8C    */
        "W8d", /*   8D    */
        "Zcaron", /*   8E    */
        "W8f", /*   8F    */
        "W90", /*   90    */
        "quoteleft", /*   91    */
        "quoteright", /*   92    */
        "quotedblleft", /*   93    */
        "quotedblright", /*   94    */
        "bullet", /*   95    */
        "endash", /*   96    */
        "emdash", /*   97    */
        "tilde", /*   98    */
        "trademark", /*   99    */
        "scaron", /*   9A    */
        "guilsinglright", /*   9B    */
        "oe", /*   9C    */
        "W9d", /*   9D    */
        "zcaron", /*   9E    */
        "Ydieresis", /*   9F    */
        "reqspace", /*   A0    */
        "exclamdown", /*   A1    */
        "cent", /*   A2    */
        "sterling", /*   A3    */
        "currency", /*   A4    */
        "yen", /*   A5    */
        "brokenbar", /*   A6    */
        "section", /*   A7    */
        "dieresis", /*   A8    */
        "copyright", /*   A9    */
        "ordfeminine", /*   AA    */
        "guillemotleft", /*   AB    */
        "logicalnot", /*   AC    */
        "syllable", /*   AD    */
        "registered", /*   AE    */
        "macron", /*   AF    */
        "degree", /*   B0    */
        "plusminus", /*   B1    */
        "twosuperior", /*   B2    */
        "threesuperior", /*   B3    */
        "acute", /*   B4    */
        "mu", /*   B5    */
        "paragraph", /*   B6    */
        "periodcentered", /*   B7    */
        "cedilla", /*   B8    */
        "onesuperior", /*   B9    */
        "ordmasculine", /*   BA    */
        "guillemotright", /*   BB    */
        "onequarter", /*   BC    */
        "onehalf", /*   BD    */
        "threequarters", /*   BE    */
        "questiondown", /*   BF    */
        "Agrave", /*   C0    */
        "Aacute", /*   C1    */
        "Acircumflex", /*   C2    */
        "Atilde", /*   C3    */
        "Adieresis", /*   C4    */
        "Aring", /*   C5    */
        "AE", /*   C6    */
        "Ccedilla", /*   C7    */
        "Egrave", /*   C8    */
        "Eacute", /*   C9    */
        "Ecircumflex", /*   CA    */
        "Edieresis", /*   CB    */
        "Igrave", /*   CC    */
        "Iacute", /*   CD    */
        "Icircumflex", /*   CE    */
        "Idieresis", /*   CF    */
        "Eth", /*   D0    */
        "Ntilde", /*   D1    */
        "Ograve", /*   D2    */
        "Oacute", /*   D3    */
        "Ocircumflex", /*   D4    */
        "Otilde", /*   D5    */
        "Odieresis", /*   D6    */
        "multiply", /*   D7    */
        "Oslash", /*   D8    */
        "Ugrave", /*   D9    */
        "Uacute", /*   DA    */
        "Ucircumflex", /*   DB    */
        "Udieresis", /*   DC    */
        "Yacute", /*   DD    */
        "Thorn", /*   DE    */
        "germandbls", /*   DF    */
        "agrave", /*   E0    */
        "aacute", /*   E1    */
        "acircumflex", /*   E2    */
        "atilde", /*   E3    */
        "adieresis", /*   E4    */
        "aring", /*   E5    */
        "ae", /*   E6    */
        "ccedilla", /*   E7    */
        "egrave", /*   E8    */
        "eacute", /*   E9    */
        "ecircumflex", /*   EA    */
        "edieresis", /*   EB    */
        "igrave", /*   EC    */
        "iacute", /*   ED    */
        "icircumflex", /*   EE    */
        "idieresis", /*   EF    */
        "eth", /*   F0    */
        "ntilde", /*   F1    */
        "ograve", /*   F2    */
        "oacute", /*   F3    */
        "ocircumflex", /*   F4    */
        "otilde", /*   F5    */
        "odieresis", /*   F6    */
        "divide", /*   F7    */
        "oslash", /*   F8    */
        "ugrave", /*   F9    */
        "uacute", /*   FA    */
        "ucircumflex", /*   FB    */
        "udieresis", /*   FC    */
        "yacute", /*   FD    */
        "thorn", /*   FE    */
        "ydieresis", /*   FF    */
    };

    private short _ascender;

    private short _ascent;

    private short _avgwidth;

    private int _bitoff;

    private int _bits;

    private byte _brkchar;

    /// <summary>
    ///     Some metrics from the PostScript extension
    /// </summary>
    private short _capheight;

    private byte _charset;

    private int _chartab;

    private string _copyright;

    private byte _defchar;

    private short _descender;

    private int _device;

    private short _extleading;

    private short _extlen;

    private int _face;

    private int _firstchar;

    private int _fontname;

    private int _hLen;

    private short _horres;

    private short _intleading;

    private bool _isMono;

    private byte _italic;

    private int _kernpairs;

    private byte _kind;

    private int _lastchar;

    private short _maxwidth;

    private byte _overs;

    private short _pixheight;

    private short _pixwidth;

    private short _points;

    private int _psext;

    private int _res1;

    private int _res2;

    private short _type;

    private byte _uline;

    private short _verres;

    private short _vers;

    private short _weight;

    private short _widthby;

    /// <summary>
    ///     Character class.  This is a minor attempt to overcome the problem that
    ///     in the pfm file, all unused characters are given the width of space.
    ///     Note that this array isn't used in iText.
    /// </summary>
    private readonly int[] _winClass =
    {
        0, 0, 0, 0, 2, 2, 2, 0, 2, 0, 2, 2, 2, 0, 0, 0, /* 00 */
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* 10 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 20 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 30 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 40 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 50 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* 60 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, /* 70 */
        0, 0, 2, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, /* 80 */
        0, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2, /* 90 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* a0 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* b0 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* c0 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* d0 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* e0 */
        1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, /* f0 */
    };

    private short _xheight;

    /// <summary>
    ///     Creates a new instance of Pfm2afm
    /// </summary>
    private Pfm2Afm(RandomAccessFileOrArray inp, Stream outp)
    {
        _inp = inp;
        _encoding = EncodingsRegistry.GetEncoding(1252);
        _outp = new StreamWriter(outp, _encoding);
    }

    /// <summary>
    ///     Converts a PFM file into an AFM file.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="inp">the PFM file</param>
    /// <param name="outp">the AFM file</param>
    public static void Convert(RandomAccessFileOrArray inp, Stream outp)
    {
        var p = new Pfm2Afm(inp, outp);
        p.openpfm();
        p.putheader();
        p.putchartab();
        p.putkerntab();
        p.puttrailer();
        p._outp.Flush();
    }

    /// <summary>
    ///     public static void Main(String[] args) {
    ///     try {
    ///     RandomAccessFileOrArray inp = new RandomAccessFileOrArray(args[0]);
    ///     Stream outp = new FileOutputStream(args[1]);
    ///     Convert(inp, outp);
    ///     inp.Close();
    ///     outp.Close();
    ///     }
    ///     catch (Exception e) {
    ///     e.PrintStackTrace();
    ///     }
    ///     }
    /// </summary>
    private void openpfm()
    {
        _inp.Seek(0);
        _vers = _inp.ReadShortLe();
        _hLen = _inp.ReadIntLe();
        _copyright = readString(60);
        _type = _inp.ReadShortLe();
        _points = _inp.ReadShortLe();
        _verres = _inp.ReadShortLe();
        _horres = _inp.ReadShortLe();
        _ascent = _inp.ReadShortLe();
        _intleading = _inp.ReadShortLe();
        _extleading = _inp.ReadShortLe();
        _italic = (byte)_inp.Read();
        _uline = (byte)_inp.Read();
        _overs = (byte)_inp.Read();
        _weight = _inp.ReadShortLe();
        _charset = (byte)_inp.Read();
        _pixwidth = _inp.ReadShortLe();
        _pixheight = _inp.ReadShortLe();
        _kind = (byte)_inp.Read();
        _avgwidth = _inp.ReadShortLe();
        _maxwidth = _inp.ReadShortLe();
        _firstchar = _inp.Read();
        _lastchar = _inp.Read();
        _defchar = (byte)_inp.Read();
        _brkchar = (byte)_inp.Read();
        _widthby = _inp.ReadShortLe();
        _device = _inp.ReadIntLe();
        _face = _inp.ReadIntLe();
        _bits = _inp.ReadIntLe();
        _bitoff = _inp.ReadIntLe();
        _extlen = _inp.ReadShortLe();
        _psext = _inp.ReadIntLe();
        _chartab = _inp.ReadIntLe();
        _res1 = _inp.ReadIntLe();
        _kernpairs = _inp.ReadIntLe();
        _res2 = _inp.ReadIntLe();
        _fontname = _inp.ReadIntLe();
        if (_hLen != _inp.Length || _extlen != 30 || _fontname < 75 || _fontname > 512)
        {
            throw new IOException("Not a valid PFM file.");
        }

        _inp.Seek(_psext + 14);
        _capheight = _inp.ReadShortLe();
        _xheight = _inp.ReadShortLe();
        _ascender = _inp.ReadShortLe();
        _descender = _inp.ReadShortLe();
    }

    /// <summary>
    ///     Output a character entry
    /// </summary>
    private void outchar(int code, int width, string name)
    {
        _outp.Write("C ");
        outval(code);
        _outp.Write(" ; WX ");
        outval(width);
        if (name != null)
        {
            _outp.Write(" ; N ");
            _outp.Write(name);
        }

        _outp.Write(" ;\n");
    }

    private void outval(int n)
    {
        _outp.Write(' ');
        _outp.Write(n);
    }

    private void putchartab()
    {
        var count = _lastchar - _firstchar + 1;
        var ctabs = new int[count];
        _inp.Seek(_chartab);
        for (var k = 0; k < count; ++k)
        {
            ctabs[k] = _inp.ReadUnsignedShortLe();
        }

        var back = new int[256];
        if (_charset == 0)
        {
            for (var i = _firstchar; i <= _lastchar; ++i)
            {
                if (_win2PsStd[i] != 0)
                {
                    back[_win2PsStd[i]] = i;
                }
            }
        }

        /* Put outp the header */
        _outp.Write("StartCharMetrics");
        outval(count);
        _outp.Write('\n');

        /* Put outp all encoded chars */
        if (_charset != 0)
        {
            /*
            * If the charset is not the Windows standard, just put outp
            * unnamed entries.
            */
            for (var i = _firstchar; i <= _lastchar; i++)
            {
                if (ctabs[i - _firstchar] != 0)
                {
                    outchar(i, ctabs[i - _firstchar], null);
                }
            }
        }
        else
        {
            for (var i = 0; i < 256; i++)
            {
                var j = back[i];
                if (j != 0)
                {
                    outchar(i, ctabs[j - _firstchar], _winChars[j]);
                    ctabs[j - _firstchar] = 0;
                }
            }

            /* Put outp all non-encoded chars */
            for (var i = _firstchar; i <= _lastchar; i++)
            {
                if (ctabs[i - _firstchar] != 0)
                {
                    outchar(-1, ctabs[i - _firstchar], _winChars[i]);
                }
            }
        }

        /* Put outp the trailer */
        _outp.Write("EndCharMetrics\n");
    }

    private void putheader()
    {
        _outp.Write("StartFontMetrics 2.0\n");
        if (_copyright.Length > 0)
        {
            _outp.Write("Comment " + _copyright + '\n');
        }

        _outp.Write("FontName ");
        _inp.Seek(_fontname);
        var fname = readString();
        _outp.Write(fname);
        _outp.Write("\nEncodingScheme ");
        if (_charset != 0)
        {
            _outp.Write("FontSpecific\n");
        }
        else
        {
            _outp.Write("AdobeStandardEncoding\n");
        }

        /*
        * The .pfm is missing full name, so construct from font name by
        * changing the hyphen to a space.  This actually works inp a lot
        * of cases.
        */
        _outp.Write("FullName " + fname.Replace('-', ' '));
        if (_face != 0)
        {
            _inp.Seek(_face);
            _outp.Write("\nFamilyName " + readString());
        }

        _outp.Write("\nWeight ");
        if (_weight > 475 || fname.IndexOf("bold", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            _outp.Write("Bold");
        }
        else if ((_weight < 325 && _weight != 0) || fname.IndexOf("light", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            _outp.Write("Light");
        }
        else if (fname.IndexOf("black", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            _outp.Write("Black");
        }
        else
        {
            _outp.Write("Medium");
        }

        _outp.Write("\nItalicAngle ");
        if (_italic != 0 || fname.IndexOf("italic", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            _outp.Write("-12.00");
        }
        /* this is a typical value; something else may work better for a
        specific font */
        else
        {
            _outp.Write("0");
        }

        /*
        *  The mono flag inp the pfm actually indicates whether there is a
        *  table of font widths, not if they are all the same.
        */
        _outp.Write("\nIsFixedPitch ");
        if ((_kind & 1) == 0 || /* Flag for mono */
            _avgwidth == _maxwidth)
        {
            /* Avg width = max width */
            _outp.Write("true");
            _isMono = true;
        }
        else
        {
            _outp.Write("false");
            _isMono = false;
        }

        /*
        * The font bounding box is lost, but try to reconstruct it.
        * Much of this is just guess work.  The bounding box is required inp
        * the .afm, but is not used by the PM font installer.
        */
        _outp.Write("\nFontBBox");
        if (_isMono)
        {
            outval(-20); /* Just guess at left bounds */
        }
        else
        {
            outval(-100);
        }

        outval(-(_descender + 5)); /* Descender is given as positive value */
        outval(_maxwidth + 10);
        outval(_ascent + 5);

        /*
        * Give other metrics that were kept
        */
        _outp.Write("\nCapHeight");
        outval(_capheight);
        _outp.Write("\nXHeight");
        outval(_xheight);
        _outp.Write("\nDescender");
        outval(_descender);
        _outp.Write("\nAscender");
        outval(_ascender);
        _outp.Write('\n');
    }

    private void putkerntab()
    {
        if (_kernpairs == 0)
        {
            return;
        }

        _inp.Seek(_kernpairs);
        var count = _inp.ReadUnsignedShortLe();
        var nzero = 0;
        var kerns = new int[count * 3];
        for (var k = 0; k < kerns.Length;)
        {
            kerns[k++] = _inp.Read();
            kerns[k++] = _inp.Read();
            if ((kerns[k++] = _inp.ReadShortLe()) != 0)
            {
                ++nzero;
            }
        }

        if (nzero == 0)
        {
            return;
        }

        _outp.Write("StartKernData\nStartKernPairs");
        outval(nzero);
        _outp.Write('\n');
        for (var k = 0; k < kerns.Length; k += 3)
        {
            if (kerns[k + 2] != 0)
            {
                _outp.Write("KPX ");
                _outp.Write(_winChars[kerns[k]]);
                _outp.Write(' ');
                _outp.Write(_winChars[kerns[k + 1]]);
                outval(kerns[k + 2]);
                _outp.Write('\n');
            }
        }

        /* Put outp trailer */
        _outp.Write("EndKernPairs\nEndKernData\n");
    }

    private void puttrailer()
    {
        _outp.Write("EndFontMetrics\n");
    }

    private string readString(int n)
    {
        var b = new byte[n];
        _inp.ReadFully(b);
        int k;
        for (k = 0; k < b.Length; ++k)
        {
            if (b[k] == 0)
            {
                break;
            }
        }

        return _encoding.GetString(b, 0, k);
    }

    private string readString()
    {
        var buf = new StringBuilder();
        while (true)
        {
            var c = _inp.Read();
            if (c <= 0)
            {
                break;
            }

            buf.Append((char)c);
        }

        return buf.ToString();
    }
    /* Total length of .pfm file */
    /* Copyright string [60]*/
    /* 0=windows, otherwise nomap */
    /* Width for mono fonts */
    /* Lower bit off inp mono */
    /* Mono if avg=max width */
    /* Use to compute bounding box */
    /* First char inp table */
    /* Last char inp table */
    /* Face name */
    /* PostScript extension */
    /* Character width tables */
    /* Kerning pairs */
    /* Font name */

    /* Cap height */
    /* X height */
    /* Ascender */
    /* Descender (positive) */
}