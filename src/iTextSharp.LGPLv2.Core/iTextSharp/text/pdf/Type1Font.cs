using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Reads a Type1 font
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
internal class Type1Font : BaseFont
{
    /// <summary>
    ///     Types of records in a PFB file. ASCII is 1 and BINARY is 2.
    ///     They have to appear in the PFB file in this sequence.
    /// </summary>
    private static readonly int[] _pfbTypes =
    {
        1, 2, 1
    };

    /// <summary>
    ///     true  if this font is one of the 14 built in fonts.
    /// </summary>
    private readonly bool _builtinFont;

    /// <summary>
    ///     Represents the section CharMetrics in the AFM file. Each
    ///     value of this array contains a  Object[4]  with an
    ///     Integer, Integer, String and int[]. This is the code, width, name and char bbox.
    ///     The key is the name of the char and also an Integer with the char number.
    /// </summary>
    private readonly NullValueDictionary<object, object[]> _charMetrics = new();

    /// <summary>
    ///     The file in use.
    /// </summary>
    private readonly string _fileName;

    /// <summary>
    ///     Represents the section KernPairs in the AFM file. The key is
    ///     the name of the first character and the value is a  Object[]
    ///     with 2 elements for each kern pair. Position 0 is the name of
    ///     the second character and position 1 is the kerning distance. This is
    ///     repeated for all the pairs.
    /// </summary>
    private readonly INullValueDictionary<string, object[]> _kernPairs = new NullValueDictionary<string, object[]>();

    /// <summary>
    ///     The PFB file if the input was made with a  byte  array.
    /// </summary>
    protected readonly byte[] Pfb;

    /// <summary>
    ///     A variable.
    /// </summary>
    private int _ascender = 800;

    /// <summary>
    ///     A variable.
    /// </summary>
    private int _capHeight = 700;

    /// <summary>
    ///     The character set of the font.
    /// </summary>
    private string _characterSet;

    /// <summary>
    ///     A variable.
    /// </summary>
    private int _descender = -200;

    /// <summary>
    ///     The font's encoding name. This encoding is 'StandardEncoding' or
    ///     'AdobeStandardEncoding' for a font that can be totally encoded
    ///     according to the characters names. For all other names the
    ///     font is treated as symbolic.
    /// </summary>
    private string _encodingScheme = "FontSpecific";

    /// <summary>
    ///     The family name of the font.
    /// </summary>
    private string _familyName;

    /// <summary>
    ///     The Postscript font name.
    /// </summary>
    private string _fontName;

    /// <summary>
    ///     The full name of the font.
    /// </summary>
    private string _fullName;

    /// <summary>
    ///     true  if all the characters have the same
    ///     width.
    /// </summary>
    private bool _isFixedPitch;

    /// <summary>
    ///     The italic angle of the font, usually 0.0 or negative.
    /// </summary>
    private float _italicAngle;

    /// <summary>
    ///     The llx of the FontBox.
    /// </summary>
    private int _llx = -50;

    /// <summary>
    ///     The lly of the FontBox.
    /// </summary>
    private int _lly = -200;

    /// <summary>
    ///     A variable.
    /// </summary>
    private int _stdHw;

    /// <summary>
    ///     A variable.
    /// </summary>
    private int _stdVw = 80;

    /// <summary>
    ///     The underline position.
    /// </summary>
    private int _underlinePosition = -100;

    /// <summary>
    ///     The underline thickness.
    /// </summary>
    private int _underlineThickness = 50;

    /// <summary>
    ///     The lurx of the FontBox.
    /// </summary>
    private int _urx = 1000;

    /// <summary>
    ///     The ury of the FontBox.
    /// </summary>
    private int _ury = 900;

    /// <summary>
    ///     The weight of the font: normal, bold, etc.
    /// </summary>
    private string _weight = "";

    /// <summary>
    ///     A variable.
    /// </summary>
    private int _xHeight = 480;

    /// <summary>
    ///     Creates a new Type1 font.
    ///     @throws DocumentException the AFM file is invalid
    ///     @throws IOException the AFM file could not be read
    /// </summary>
    /// <param name="ttfAfm">the AFM file if the input is made with a  byte  array</param>
    /// <param name="pfb">the PFB file if the input is made with a  byte  array</param>
    /// <param name="afmFile">
    ///     the name of one of the 14 built-in fonts or the location of an AFM file. The file must end in
    ///     '.afm'
    /// </param>
    /// <param name="enc">the encoding to be applied to this font</param>
    /// <param name="emb">true if the font is to be embedded in the PDF</param>
    /// <param name="forceRead"></param>
    internal Type1Font(string afmFile, string enc, bool emb, byte[] ttfAfm, byte[] pfb, bool forceRead)
    {
        if (emb && ttfAfm != null && pfb == null)
        {
            throw new DocumentException("Two byte arrays are needed if the Type1 font is embedded.");
        }

        if (emb && ttfAfm != null)
        {
            Pfb = pfb;
        }

        encoding = enc;
        Embedded = emb;
        _fileName = afmFile;
        FontType = FONT_TYPE_T1;
        RandomAccessFileOrArray rf = null;
        Stream istr = null;

        if (BuiltinFonts14.ContainsKey(afmFile))
        {
            Embedded = false;
            _builtinFont = true;
            var buf = new byte[1024];

            try
            {
                istr = GetResourceStream(RESOURCE_PATH + afmFile + ".afm");

                if (istr == null)
                {
                    Console.Error.WriteLine(afmFile + " not found as resource.");

                    throw new DocumentException(afmFile + " not found as resource.");
                }

                using var ostr = new MemoryStream();

                while (true)
                {
                    var size = istr.Read(buf, 0, buf.Length);

                    if (size == 0)
                    {
                        break;
                    }

                    ostr.Write(buf, 0, size);
                }

                buf = ostr.ToArray();
            }
            finally
            {
                if (istr != null)
                {
                    try
                    {
                        istr.Dispose();
                    }
                    catch
                    {
                        // empty on purpose
                    }
                }
            }

            try
            {
                rf = new RandomAccessFileOrArray(buf);
                Process(rf);
            }
            finally
            {
                if (rf != null)
                {
                    try
                    {
                        rf.Close();
                    }
                    catch
                    {
                        // empty on purpose
                    }
                }
            }
        }
        else if (afmFile.EndsWith(".afm", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                if (ttfAfm == null)
                {
                    rf = new RandomAccessFileOrArray(afmFile, forceRead);
                }
                else
                {
                    rf = new RandomAccessFileOrArray(ttfAfm);
                }

                Process(rf);
            }
            finally
            {
                if (rf != null)
                {
                    try
                    {
                        rf.Close();
                    }
                    catch
                    {
                        // empty on purpose
                    }
                }
            }
        }
        else if (afmFile.EndsWith(".pfm", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var ba = new MemoryStream();

                if (ttfAfm == null)
                {
                    rf = new RandomAccessFileOrArray(afmFile, forceRead);
                }
                else
                {
                    rf = new RandomAccessFileOrArray(ttfAfm);
                }

                Pfm2Afm.Convert(rf, ba);
                rf.Close();
                rf = new RandomAccessFileOrArray(ba.ToArray());
                Process(rf);
            }
            finally
            {
                if (rf != null)
                {
                    try
                    {
                        rf.Close();
                    }
                    catch
                    {
                        // empty on purpose
                    }
                }
            }
        }
        else
        {
            throw new DocumentException(afmFile + " is not an AFM or PFM font file.");
        }

        _encodingScheme = _encodingScheme.Trim();

        if (_encodingScheme.Equals("AdobeStandardEncoding", StringComparison.Ordinal) ||
            _encodingScheme.Equals("StandardEncoding", StringComparison.Ordinal))
        {
            FontSpecific = false;
        }

        if (!encoding.StartsWith("#", StringComparison.Ordinal))
        {
            PdfEncodings.ConvertToBytes(" ", enc); // check if the encoding exists
        }

        CreateEncoding();
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
    public override string[][] AllNameEntries => new[]
    {
        new[]
        {
            "4", "", "", "", _fullName
        }
    };

    /// <summary>
    ///     Gets the family name of the font. If it is a True Type font
    ///     each array element will have {Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the family name of the font</returns>
    public override string[][] FamilyFontName => new[]
    {
        new[]
        {
            "", "", "", _familyName
        }
    };

    /// <summary>
    ///     Gets the full name of the font. If it is a True Type font
    ///     each array element will have {Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the full name of the font</returns>
    public override string[][] FullFontName => new[]
    {
        new[]
        {
            "", "", "", _fullName
        }
    };

    /// <summary>
    ///     Gets the postscript font name.
    /// </summary>
    /// <returns>the postscript font name</returns>
    public override string PostscriptFontName
    {
        get => _fontName;
        set => _fontName = value;
    }

    /// <summary>
    ///     Generates the font descriptor for this font or  null  if it is
    ///     one of the 14 built in fonts.
    /// </summary>
    /// <param name="fontStream">the indirect reference to a PdfStream containing the font or  null </param>
    /// <returns>the PdfDictionary containing the font descriptor or  null </returns>
    public PdfDictionary GetFontDescriptor(PdfIndirectReference fontStream)
    {
        if (_builtinFont)
        {
            return null;
        }

        var dic = new PdfDictionary(PdfName.Fontdescriptor);
        dic.Put(PdfName.Ascent, new PdfNumber(_ascender));
        dic.Put(PdfName.Capheight, new PdfNumber(_capHeight));
        dic.Put(PdfName.Descent, new PdfNumber(_descender));
        dic.Put(PdfName.Fontbbox, new PdfRectangle(_llx, _lly, _urx, _ury));
        dic.Put(PdfName.Fontname, new PdfName(_fontName));
        dic.Put(PdfName.Italicangle, new PdfNumber(_italicAngle));
        dic.Put(PdfName.Stemv, new PdfNumber(_stdVw));

        if (fontStream != null)
        {
            dic.Put(PdfName.Fontfile, fontStream);
        }

        var flags = 0;

        if (_isFixedPitch)
        {
            flags |= 1;
        }

        flags |= FontSpecific ? 4 : 32;

        if (_italicAngle < 0)
        {
            flags |= 64;
        }

        if (_fontName.IndexOf("Caps", StringComparison.Ordinal) >= 0 ||
            _fontName.EndsWith("SC", StringComparison.Ordinal))
        {
            flags |= 131072;
        }

        if (_weight.Equals("Bold", StringComparison.Ordinal))
        {
            flags |= 262144;
        }

        dic.Put(PdfName.Flags, new PdfNumber(flags));

        return dic;
    }

    /// <summary>
    ///     Gets the font parameter identified by  key . Valid values
    ///     for  key  are  ASCENT ,  CAPHEIGHT ,  DESCENT ,
    ///     ITALICANGLE ,  BBOXLLX ,  BBOXLLY ,  BBOXURX
    ///     and  BBOXURY .
    /// </summary>
    /// <param name="key">the parameter to be extracted</param>
    /// <param name="fontSize">the font size in points</param>
    /// <returns>the parameter in points</returns>
    public override float GetFontDescriptor(int key, float fontSize)
    {
        switch (key)
        {
            case AWT_ASCENT:
            case ASCENT:
                return _ascender * fontSize / 1000;
            case CAPHEIGHT:
                return _capHeight * fontSize / 1000;
            case AWT_DESCENT:
            case DESCENT:
                return _descender * fontSize / 1000;
            case ITALICANGLE:
                return _italicAngle;
            case BBOXLLX:
                return _llx * fontSize / 1000;
            case BBOXLLY:
                return _lly * fontSize / 1000;
            case BBOXURX:
                return _urx * fontSize / 1000;
            case BBOXURY:
                return _ury * fontSize / 1000;
            case AWT_LEADING:
                return 0;
            case AWT_MAXADVANCE:
                return (_urx - _llx) * fontSize / 1000;
            case UNDERLINE_POSITION:
                return _underlinePosition * fontSize / 1000;
            case UNDERLINE_THICKNESS:
                return _underlineThickness * fontSize / 1000;
        }

        return 0;
    }

    /// <summary>
    ///     If the embedded flag is  false  or if the font is
    ///     one of the 14 built in types, it returns  null ,
    ///     otherwise the font is read and output in a PdfStream object.
    ///     @throws DocumentException if there is an error reading the font
    /// </summary>
    /// <returns>the PdfStream containing the font or  null </returns>
    public override PdfStream GetFullFontStream()
    {
        if (_builtinFont || !Embedded)
        {
            return null;
        }

        RandomAccessFileOrArray rf = null;

        try
        {
            var filePfb = _fileName.Substring(0, _fileName.Length - 3) + "pfb";

            if (Pfb == null)
            {
                rf = new RandomAccessFileOrArray(filePfb, true);
            }
            else
            {
                rf = new RandomAccessFileOrArray(Pfb);
            }

            var fileLength = rf.Length;
            var st = new byte[fileLength - 18];
            var lengths = new int[3];
            var bytePtr = 0;

            for (var k = 0; k < 3; ++k)
            {
                if (rf.Read() != 0x80)
                {
                    throw new DocumentException("Start marker missing in " + filePfb);
                }

                if (rf.Read() != _pfbTypes[k])
                {
                    throw new DocumentException("Incorrect segment type in " + filePfb);
                }

                var size = rf.Read();
                size += rf.Read() << 8;
                size += rf.Read() << 16;
                size += rf.Read() << 24;
                lengths[k] = size;

                while (size != 0)
                {
                    var got = rf.Read(st, bytePtr, size);

                    if (got < 0)
                    {
                        throw new DocumentException("Premature end in " + filePfb);
                    }

                    bytePtr += got;
                    size -= got;
                }
            }

            return new StreamFont(st, lengths, compressionLevel);
        }
        finally
        {
            if (rf != null)
            {
                try
                {
                    rf.Close();
                }
                catch
                {
                    // empty on purpose
                }
            }
        }
    }

    /// <summary>
    ///     Gets the kerning between two Unicode characters. The characters
    ///     are converted to names and this names are used to find the kerning
    ///     pairs in the  Hashtable   KernPairs .
    /// </summary>
    /// <param name="char1">the first char</param>
    /// <param name="char2">the second char</param>
    /// <returns>the kerning to be applied</returns>
    public override int GetKerning(int char1, int char2)
    {
        var first = GlyphList.UnicodeToName(char1);

        if (first == null)
        {
            return 0;
        }

        var second = GlyphList.UnicodeToName(char2);

        if (second == null)
        {
            return 0;
        }

        var obj = _kernPairs[first];

        if (obj == null)
        {
            return 0;
        }

        for (var k = 0; k < obj.Length; k += 2)
        {
            if (second.Equals(obj[k]))
            {
                return (int)obj[k + 1];
            }
        }

        return 0;
    }

    /// <summary>
    ///     Checks if the font has any kerning pairs.
    /// </summary>
    /// <returns> true  if the font has any kerning pairs</returns>
    public override bool HasKernPairs()
        => _kernPairs.Count > 0;

    /// <summary>
    ///     Reads the font metrics
    ///     @throws DocumentException the AFM file is invalid
    ///     @throws IOException the AFM file could not be read
    /// </summary>
    /// <param name="rf">the AFM file</param>
    public void Process(RandomAccessFileOrArray rf)
    {
        string line;
        var isMetrics = false;

        while ((line = rf.ReadLine()) != null)
        {
            var tok = new StringTokenizer(line, " ,\n\r\t\f");

            if (!tok.HasMoreTokens())
            {
                continue;
            }

            var ident = tok.NextToken();

            if (ident.Equals("FontName", StringComparison.Ordinal))
            {
                _fontName = tok.NextToken("\u00ff").Substring(1);
            }
            else if (ident.Equals("FullName", StringComparison.Ordinal))
            {
                _fullName = tok.NextToken("\u00ff").Substring(1);
            }
            else if (ident.Equals("FamilyName", StringComparison.Ordinal))
            {
                _familyName = tok.NextToken("\u00ff").Substring(1);
            }
            else if (ident.Equals("Weight", StringComparison.Ordinal))
            {
                _weight = tok.NextToken("\u00ff").Substring(1);
            }
            else if (ident.Equals("ItalicAngle", StringComparison.Ordinal))
            {
                _italicAngle = float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("IsFixedPitch", StringComparison.Ordinal))
            {
                _isFixedPitch = tok.NextToken().Equals("true", StringComparison.Ordinal);
            }
            else if (ident.Equals("CharacterSet", StringComparison.Ordinal))
            {
                _characterSet = tok.NextToken("\u00ff").Substring(1);
            }
            else if (ident.Equals("FontBBox", StringComparison.Ordinal))
            {
                _llx = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
                _lly = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
                _urx = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
                _ury = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("UnderlinePosition", StringComparison.Ordinal))
            {
                _underlinePosition = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("UnderlineThickness", StringComparison.Ordinal))
            {
                _underlineThickness = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("EncodingScheme", StringComparison.Ordinal))
            {
                _encodingScheme = tok.NextToken("\u00ff").Substring(1);
            }
            else if (ident.Equals("CapHeight", StringComparison.Ordinal))
            {
                _capHeight = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("XHeight", StringComparison.Ordinal))
            {
                _xHeight = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("Ascender", StringComparison.Ordinal))
            {
                _ascender = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("Descender", StringComparison.Ordinal))
            {
                _descender = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("StdHW", StringComparison.Ordinal))
            {
                _stdHw = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("StdVW", StringComparison.Ordinal))
            {
                _stdVw = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
            }
            else if (ident.Equals("StartCharMetrics", StringComparison.Ordinal))
            {
                isMetrics = true;

                break;
            }
        }

        if (!isMetrics)
        {
            throw new DocumentException("Missing StartCharMetrics in " + _fileName);
        }

        while ((line = rf.ReadLine()) != null)
        {
            var tok = new StringTokenizer(line);

            if (!tok.HasMoreTokens())
            {
                continue;
            }

            var ident = tok.NextToken();

            if (ident.Equals("EndCharMetrics", StringComparison.Ordinal))
            {
                isMetrics = false;

                break;
            }

            var c = -1;
            var wx = 250;
            var n = "";
            int[] b = null;

            tok = new StringTokenizer(line, ";");

            while (tok.HasMoreTokens())
            {
                var tokc = new StringTokenizer(tok.NextToken());

                if (!tokc.HasMoreTokens())
                {
                    continue;
                }

                ident = tokc.NextToken();

                if (ident.Equals("C", StringComparison.Ordinal))
                {
                    c = int.Parse(tokc.NextToken(), CultureInfo.InvariantCulture);
                }
                else if (ident.Equals("WX", StringComparison.Ordinal))
                {
                    wx = (int)float.Parse(tokc.NextToken(), NumberFormatInfo.InvariantInfo);
                }
                else if (ident.Equals("N", StringComparison.Ordinal))
                {
                    n = tokc.NextToken();
                }
                else if (ident.Equals("B", StringComparison.Ordinal))
                {
                    b = new[]
                    {
                        int.Parse(tokc.NextToken(), CultureInfo.InvariantCulture),
                        int.Parse(tokc.NextToken(), CultureInfo.InvariantCulture),
                        int.Parse(tokc.NextToken(), CultureInfo.InvariantCulture),
                        int.Parse(tokc.NextToken(), CultureInfo.InvariantCulture)
                    };
                }
            }

            object[] metrics =
            {
                c, wx, n, b
            };

            if (c >= 0)
            {
                _charMetrics[c] = metrics;
            }

            _charMetrics[n] = metrics;
        }

        if (isMetrics)
        {
            throw new DocumentException("Missing EndCharMetrics in " + _fileName);
        }

        if (!_charMetrics.ContainsKey("nonbreakingspace"))
        {
            var space = _charMetrics["space"];

            if (space != null)
            {
                _charMetrics["nonbreakingspace"] = space;
            }
        }

        while ((line = rf.ReadLine()) != null)
        {
            var tok = new StringTokenizer(line);

            if (!tok.HasMoreTokens())
            {
                continue;
            }

            var ident = tok.NextToken();

            if (ident.Equals("EndFontMetrics", StringComparison.Ordinal))
            {
                return;
            }

            if (ident.Equals("StartKernPairs", StringComparison.Ordinal))
            {
                isMetrics = true;

                break;
            }
        }

        if (!isMetrics)
        {
            throw new DocumentException("Missing EndFontMetrics in " + _fileName);
        }

        while ((line = rf.ReadLine()) != null)
        {
            var tok = new StringTokenizer(line);

            if (!tok.HasMoreTokens())
            {
                continue;
            }

            var ident = tok.NextToken();

            if (ident.Equals("KPX", StringComparison.Ordinal))
            {
                var first = tok.NextToken();
                var second = tok.NextToken();
                var width = (int)float.Parse(tok.NextToken(), NumberFormatInfo.InvariantInfo);
                var relates = _kernPairs[first];

                if (relates == null)
                {
                    _kernPairs[first] = new object[]
                    {
                        second, width
                    };
                }
                else
                {
                    var n = relates.Length;
                    var relates2 = new object[n + 2];
                    Array.Copy(relates, 0, relates2, 0, n);
                    relates2[n] = second;
                    relates2[n + 1] = width;
                    _kernPairs[first] = relates2;
                }
            }
            else if (ident.Equals("EndKernPairs", StringComparison.Ordinal))
            {
                isMetrics = false;

                break;
            }
        }

        if (isMetrics)
        {
            throw new DocumentException("Missing EndKernPairs in " + _fileName);
        }

        rf.Close();
    }

    /// <summary>
    ///     Sets the kerning between two Unicode chars.
    /// </summary>
    /// <param name="char1">the first char</param>
    /// <param name="char2">the second char</param>
    /// <param name="kern">the kerning to apply in normalized 1000 units</param>
    /// <returns> true  if the kerning was applied,  false  otherwise</returns>
    public override bool SetKerning(int char1, int char2, int kern)
    {
        var first = GlyphList.UnicodeToName(char1);

        if (first == null)
        {
            return false;
        }

        var second = GlyphList.UnicodeToName(char2);

        if (second == null)
        {
            return false;
        }

        var obj = _kernPairs[first];

        if (obj == null)
        {
            obj = new object[]
            {
                second, kern
            };

            _kernPairs[first] = obj;

            return true;
        }

        for (var k = 0; k < obj.Length; k += 2)
        {
            if (second.Equals(obj[k]))
            {
                obj[k + 1] = kern;

                return true;
            }
        }

        var size = obj.Length;
        var obj2 = new object[size + 2];
        Array.Copy(obj, 0, obj2, 0, size);
        obj2[size] = second;
        obj2[size + 1] = kern;
        _kernPairs[first] = obj2;

        return true;
    }

    /// <summary>
    ///     Gets the width from the font according to the  name  or,
    ///     if the  name  is null, meaning it is a symbolic font,
    ///     the char  c .
    /// </summary>
    /// <param name="c">the char if the font is symbolic</param>
    /// <param name="name">the glyph name</param>
    /// <returns>the width of the char</returns>
    internal override int GetRawWidth(int c, string name)
    {
        object[] metrics;

        if (name == null)
        {
            // font specific
            metrics = _charMetrics[c];
        }
        else
        {
            if (name.Equals(".notdef", StringComparison.Ordinal))
            {
                return 0;
            }

            metrics = _charMetrics[name];
        }

        if (metrics != null)
        {
            return (int)metrics[1];
        }

        return 0;
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
        pobj = GetFullFontStream();

        if (pobj != null)
        {
            obj = writer.AddToBody(pobj);
            indFont = obj.IndirectReference;
        }

        pobj = GetFontDescriptor(indFont);

        if (pobj != null)
        {
            obj = writer.AddToBody(pobj);
            indFont = obj.IndirectReference;
        }

        pobj = getFontBaseType(indFont, firstChar, lastChar, shortTag);
        writer.AddToBody(pobj, piref);
    }

    protected override int[] GetRawCharBBox(int c, string name)
    {
        object[] metrics;

        if (name == null)
        {
            // font specific
            metrics = _charMetrics[c];
        }
        else
        {
            if (name.Equals(".notdef", StringComparison.Ordinal))
            {
                return null;
            }

            metrics = _charMetrics[name];
        }

        if (metrics != null)
        {
            return (int[])metrics[3];
        }

        return null;
    }

    /// <summary>
    ///     Generates the font dictionary for this font.
    /// </summary>
    /// <param name="firstChar">the first valid character</param>
    /// <param name="lastChar">the last valid character</param>
    /// <param name="shortTag">a 256 bytes long  byte  array where each unused byte is represented by 0</param>
    /// <param name="fontDescriptor">the indirect reference to a PdfDictionary containing the font descriptor or  null </param>
    /// <returns>the PdfDictionary containing the font dictionary</returns>
    private PdfDictionary getFontBaseType(PdfIndirectReference fontDescriptor,
        int firstChar,
        int lastChar,
        byte[] shortTag)
    {
        var dic = new PdfDictionary(PdfName.Font);
        dic.Put(PdfName.Subtype, PdfName.Type1);
        dic.Put(PdfName.Basefont, new PdfName(_fontName));

        var stdEncoding = encoding.Equals(CP1252, StringComparison.Ordinal) ||
                          encoding.Equals(MACROMAN, StringComparison.Ordinal);

        if (!FontSpecific || SpecialMap != null)
        {
            for (var k = firstChar; k <= lastChar; ++k)
            {
                if (!differences[k].Equals(notdef, StringComparison.Ordinal))
                {
                    firstChar = k;

                    break;
                }
            }

            if (stdEncoding)
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

        if (SpecialMap != null || forceWidthsOutput || !(_builtinFont && (FontSpecific || stdEncoding)))
        {
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
        }

        if (!_builtinFont && fontDescriptor != null)
        {
            dic.Put(PdfName.Fontdescriptor, fontDescriptor);
        }

        return dic;
    }
}