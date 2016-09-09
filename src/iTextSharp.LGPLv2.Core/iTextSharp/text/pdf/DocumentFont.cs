using System.Collections;

namespace iTextSharp.text.pdf
{
    /// <summary>
    ///
    /// </summary>
    public class DocumentFont : BaseFont
    {

        private static readonly string[] _cjkEncs = {"UniJIS-UCS2-H", "UniJIS-UCS2-H", "UniGB-UCS2-H", "UniCNS-UCS2-H",
            "UniCNS-UCS2-H", "UniKS-UCS2-H", "UniKS-UCS2-H", "UniCNS-UCS2-H", "UniGB-UCS2-H",
            "UniKS-UCS2-H", "UniJIS-UCS2-H"};

        private static readonly string[] _cjkEncs2 = {"UniCNS-UCS2-H", "UniGB-UCS2-H", "UniKS-UCS2-H", "UniJIS-UCS2-H",
            "UniCNS-UTF16-H", "UniGB-UTF16-H", "UniKS-UTF16-H", "UniJIS-UTF16-H"};

        private static readonly string[] _cjkNames = {"HeiseiMin-W3", "HeiseiKakuGo-W5", "STSong-Light", "MHei-Medium",
            "MSung-Light", "HYGoThic-Medium", "HYSMyeongJo-Medium", "MSungStd-Light", "STSongStd-Light",
            "HYSMyeongJoStd-Medium", "KozMinPro-Regular"};

        private static readonly string[] _cjkNames2 = { "MSungStd-Light", "STSongStd-Light", "HYSMyeongJoStd-Medium", "KozMinPro-Regular" };

        private static readonly int[] _stdEnc = {
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            32,33,34,35,36,37,38,8217,40,41,42,43,44,45,46,47,
            48,49,50,51,52,53,54,55,56,57,58,59,60,61,62,63,
            64,65,66,67,68,69,70,71,72,73,74,75,76,77,78,79,
            80,81,82,83,84,85,86,87,88,89,90,91,92,93,94,95,
            8216,97,98,99,100,101,102,103,104,105,106,107,108,109,110,111,
            112,113,114,115,116,117,118,119,120,121,122,123,124,125,126,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,161,162,163,8260,165,402,167,164,39,8220,171,8249,8250,64257,64258,
            0,8211,8224,8225,183,0,182,8226,8218,8222,8221,187,8230,8240,0,191,
            0,96,180,710,732,175,728,729,168,0,730,184,0,733,731,711,
            8212,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,198,0,170,0,0,0,0,321,216,338,186,0,0,0,0,
            0,230,0,0,0,305,0,0,322,248,339,223,0,0,0,0};

        private readonly BaseFont _cjkMirror;

        private readonly PdfDictionary _font;

        private readonly string _fontName;

        private readonly bool _isType0;

        /// <summary>
        /// code, [glyph, width]
        /// </summary>
        private readonly Hashtable _metrics = new Hashtable();
        private readonly PrIndirectReference _refFont;
        private float _ascender = 800;
        private float _capHeight = 700;
        private float _descender = -200;
        private IntHashtable _diffmap;
        private float _italicAngle;
        private float _llx = -50;
        private float _lly = -200;
        private float _urx = 100;
        private float _ury = 900;
        /// <summary>
        /// Creates a new instance of DocumentFont
        /// </summary>
        internal DocumentFont(PrIndirectReference refFont)
        {
            encoding = "";
            FontSpecific = false;
            _refFont = refFont;
            fontType = FONT_TYPE_DOCUMENT;
            _font = (PdfDictionary)PdfReader.GetPdfObject(refFont);
            _fontName = PdfName.DecodeName(_font.GetAsName(PdfName.Basefont).ToString());
            PdfName subType = _font.GetAsName(PdfName.Subtype);
            if (PdfName.Type1.Equals(subType) || PdfName.Truetype.Equals(subType))
                doType1Tt();
            else
            {
                for (int k = 0; k < _cjkNames.Length; ++k)
                {
                    if (_fontName.StartsWith(_cjkNames[k]))
                    {
                        _fontName = _cjkNames[k];
                        _cjkMirror = CreateFont(_fontName, _cjkEncs[k], false);
                        return;
                    }
                }
                string enc = PdfName.DecodeName(_font.GetAsName(PdfName.Encoding).ToString());
                for (int k = 0; k < _cjkEncs2.Length; ++k)
                {
                    if (enc.StartsWith(_cjkEncs2[k]))
                    {
                        if (k > 3)
                            k -= 4;
                        _cjkMirror = CreateFont(_cjkNames2[k], _cjkEncs2[k], false);
                        return;
                    }
                }
                if (PdfName.Type0.Equals(subType) && enc.Equals("Identity-H"))
                {
                    processType0(_font);
                    _isType0 = true;
                }
            }
        }

        /// <summary>
        /// Gets all the entries of the names-table. If it is a True Type font
        /// each array element will have {Name ID, Platform ID, Platform Encoding ID,
        /// Language ID, font name}. The interpretation of this values can be
        /// found in the Open Type specification, chapter 2, in the 'name' table.
        /// For the other fonts the array has a single element with {"4", "", "", "",
        /// font name}.
        /// </summary>
        /// <returns>the full name of the font</returns>
        public override string[][] AllNameEntries
        {
            get
            {
                return new[] { new[] { "4", "", "", "", _fontName } };
            }
        }

        /// <summary>
        /// Gets the family name of the font. If it is a True Type font
        /// each array element will have {Platform ID, Platform Encoding ID,
        /// Language ID, font name}. The interpretation of this values can be
        /// found in the Open Type specification, chapter 2, in the 'name' table.
        /// For the other fonts the array has a single element with {"", "", "",
        /// font name}.
        /// </summary>
        /// <returns>the family name of the font</returns>
        public override string[][] FamilyFontName
        {
            get
            {
                return FullFontName;
            }
        }

        /// <summary>
        /// Gets the full name of the font. If it is a True Type font
        /// each array element will have {Platform ID, Platform Encoding ID,
        /// Language ID, font name}. The interpretation of this values can be
        /// found in the Open Type specification, chapter 2, in the 'name' table.
        /// For the other fonts the array has a single element with {"", "", "",
        /// font name}.
        /// </summary>
        /// <returns>the full name of the font</returns>
        public override string[][] FullFontName
        {
            get
            {
                return new[] { new[] { "", "", "", _fontName } };
            }
        }

        /// <summary>
        /// Gets the postscript font name.
        /// </summary>
        /// <returns>the postscript font name</returns>
        public override string PostscriptFontName
        {
            get
            {
                return _fontName;
            }
            set
            {
            }
        }

        internal PdfIndirectReference IndirectReference
        {
            get
            {
                return _refFont;
            }
        }

        /// <summary>
        /// Exposes the unicode - > CID map that is constructed from the font's encoding
        /// @since 2.1.7
        /// </summary>
        /// <returns>the unicode to CID map</returns>
        internal IntHashtable Uni2Byte { get; } = new IntHashtable();

        public override bool CharExists(int c)
        {
            if (_cjkMirror != null)
                return _cjkMirror.CharExists(c);
            else if (_isType0)
            {
                return _metrics.ContainsKey(c);
            }
            else
                return base.CharExists(c);
        }

        public override int[] GetCharBBox(int c)
        {
            return null;
        }

        /// <summary>
        /// Gets the font parameter identified by  key . Valid values
        /// for  key  are  ASCENT ,  CAPHEIGHT ,  DESCENT ,
        ///  ITALICANGLE ,  BBOXLLX ,  BBOXLLY ,  BBOXURX
        /// and  BBOXURY .
        /// </summary>
        /// <param name="key">the parameter to be extracted</param>
        /// <param name="fontSize">the font size in points</param>
        /// <returns>the parameter in points</returns>
        public override float GetFontDescriptor(int key, float fontSize)
        {
            if (_cjkMirror != null)
                return _cjkMirror.GetFontDescriptor(key, fontSize);
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
            }
            return 0;
        }

        /// <summary>
        /// Always returns null.
        /// @since   2.1.3
        /// </summary>
        /// <returns>null</returns>
        public override PdfStream GetFullFontStream()
        {
            return null;
        }

        /// <summary>
        /// Gets the kerning between two Unicode chars.
        /// </summary>
        /// <param name="char1">the first char</param>
        /// <param name="char2">the second char</param>
        /// <returns>the kerning to be applied</returns>
        public override int GetKerning(int char1, int char2)
        {
            return 0;
        }

        /// <summary>
        /// Gets the width of a  char  in normalized 1000 units.
        /// </summary>
        /// <param name="char1">the unicode  char  to get the width of</param>
        /// <returns>the width in normalized 1000 units</returns>
        public override int GetWidth(int char1)
        {
            if (_cjkMirror != null)
                return _cjkMirror.GetWidth(char1);
            else if (_isType0)
            {
                int[] ws = (int[])_metrics[char1];
                if (ws != null)
                    return ws[1];
                else
                    return 0;
            }
            else
                return base.GetWidth(char1);
        }

        public override int GetWidth(string text)
        {
            if (_cjkMirror != null)
                return _cjkMirror.GetWidth(text);
            else if (_isType0)
            {
                char[] chars = text.ToCharArray();
                int len = chars.Length;
                int total = 0;
                for (int k = 0; k < len; ++k)
                {
                    int[] ws = (int[])_metrics[(int)chars[k]];
                    if (ws != null)
                        total += ws[1];
                }
                return total;
            }
            else
                return base.GetWidth(text);
        }

        /// <summary>
        /// Checks if the font has any kerning pairs.
        /// </summary>
        /// <returns> true  if the font has any kerning pairs</returns>
        public override bool HasKernPairs()
        {
            return false;
        }

        public override bool SetKerning(int char1, int char2, int kern)
        {
            return false;
        }

        internal override byte[] ConvertToBytes(string text)
        {
            if (_cjkMirror != null)
                return PdfEncodings.ConvertToBytes(text, CjkFont.CJK_ENCODING);
            else if (_isType0)
            {
                char[] chars = text.ToCharArray();
                int len = chars.Length;
                byte[] b = new byte[len * 2];
                int bptr = 0;
                for (int k = 0; k < len; ++k)
                {
                    int[] ws = (int[])_metrics[(int)chars[k]];
                    if (ws != null)
                    {
                        int g = ws[0];
                        b[bptr++] = (byte)(g / 256);
                        b[bptr++] = (byte)(g);
                    }
                }
                if (bptr == b.Length)
                    return b;
                else
                {
                    byte[] nb = new byte[bptr];
                    System.Array.Copy(b, 0, nb, 0, bptr);
                    return nb;
                }
            }
            else
            {
                char[] cc = text.ToCharArray();
                byte[] b = new byte[cc.Length];
                int ptr = 0;
                for (int k = 0; k < cc.Length; ++k)
                {
                    if (Uni2Byte.ContainsKey(cc[k]))
                        b[ptr++] = (byte)Uni2Byte[cc[k]];
                }
                if (ptr == b.Length)
                    return b;
                else
                {
                    byte[] b2 = new byte[ptr];
                    System.Array.Copy(b, 0, b2, 0, ptr);
                    return b2;
                }
            }
        }

        internal override byte[] ConvertToBytes(int char1)
        {
            if (_cjkMirror != null)
                return PdfEncodings.ConvertToBytes((char)char1, CjkFont.CJK_ENCODING);
            else if (_isType0)
            {
                int[] ws = (int[])_metrics[char1];
                if (ws != null)
                {
                    int g = ws[0];
                    return new[] { (byte)(g / 256), (byte)(g) };
                }
                else
                    return new byte[0];
            }
            else
            {
                if (Uni2Byte.ContainsKey(char1))
                    return new[] { (byte)Uni2Byte[char1] };
                else
                    return new byte[0];
            }
        }

        /// <summary>
        /// Gets the width from the font according to the Unicode char  c
        /// or the  name . If the  name  is null it's a symbolic font.
        /// </summary>
        /// <param name="c">the unicode char</param>
        /// <param name="name">the glyph name</param>
        /// <returns>the width of the char</returns>
        internal override int GetRawWidth(int c, string name)
        {
            return 0;
        }

        /// <summary>
        /// Outputs to the writer the font dictionaries and streams.
        /// @throws IOException on error
        /// @throws DocumentException error in generating the object
        /// </summary>
        /// <param name="writer">the writer for this document</param>
        /// <param name="refi">the font indirect reference</param>
        /// <param name="param">several parameters that depend on the font type</param>
        internal override void WriteFont(PdfWriter writer, PdfIndirectReference refi, object[] param)
        {
        }

        protected override int[] GetRawCharBBox(int c, string name)
        {
            return null;
        }

        private string decodeString(PdfString ps)
        {
            if (ps.IsHexWriting())
                return PdfEncodings.ConvertToString(ps.GetBytes(), "UnicodeBigUnmarked");
            else
                return ps.ToUnicodeString();
        }

        private void doType1Tt()
        {
            PdfObject enc = PdfReader.GetPdfObject(_font.Get(PdfName.Encoding));
            if (enc == null)
                fillEncoding(null);
            else
            {
                if (enc.IsName())
                    fillEncoding((PdfName)enc);
                else
                {
                    PdfDictionary encDic = (PdfDictionary)enc;
                    enc = PdfReader.GetPdfObject(encDic.Get(PdfName.Baseencoding));
                    if (enc == null)
                        fillEncoding(null);
                    else
                        fillEncoding((PdfName)enc);
                    PdfArray diffs = encDic.GetAsArray(PdfName.Differences);
                    if (diffs != null)
                    {
                        _diffmap = new IntHashtable();
                        int currentNumber = 0;
                        for (int k = 0; k < diffs.Size; ++k)
                        {
                            PdfObject obj = diffs[k];
                            if (obj.IsNumber())
                                currentNumber = ((PdfNumber)obj).IntValue;
                            else
                            {
                                int[] c = GlyphList.NameToUnicode(PdfName.DecodeName(((PdfName)obj).ToString()));
                                if (c != null && c.Length > 0)
                                {
                                    Uni2Byte[c[0]] = currentNumber;
                                    _diffmap[c[0]] = currentNumber;
                                }
                                ++currentNumber;
                            }
                        }
                    }
                }
            }
            PdfArray newWidths = _font.GetAsArray(PdfName.Widths);
            PdfNumber first = _font.GetAsNumber(PdfName.Firstchar);
            PdfNumber last = _font.GetAsNumber(PdfName.Lastchar);
            if (BuiltinFonts14.ContainsKey(_fontName))
            {
                BaseFont bf;
                bf = CreateFont(_fontName, WINANSI, false);
                int[] e = Uni2Byte.ToOrderedKeys();
                for (int k = 0; k < e.Length; ++k)
                {
                    int n = Uni2Byte[e[k]];
                    widths[n] = bf.GetRawWidth(n, GlyphList.UnicodeToName(e[k]));
                }
                if (_diffmap != null)
                { //widths for differences must override existing ones
                    e = _diffmap.ToOrderedKeys();
                    for (int k = 0; k < e.Length; ++k)
                    {
                        int n = _diffmap[e[k]];
                        widths[n] = bf.GetRawWidth(n, GlyphList.UnicodeToName(e[k]));
                    }
                    _diffmap = null;
                }
                _ascender = bf.GetFontDescriptor(ASCENT, 1000);
                _capHeight = bf.GetFontDescriptor(CAPHEIGHT, 1000);
                _descender = bf.GetFontDescriptor(DESCENT, 1000);
                _italicAngle = bf.GetFontDescriptor(ITALICANGLE, 1000);
                _llx = bf.GetFontDescriptor(BBOXLLX, 1000);
                _lly = bf.GetFontDescriptor(BBOXLLY, 1000);
                _urx = bf.GetFontDescriptor(BBOXURX, 1000);
                _ury = bf.GetFontDescriptor(BBOXURY, 1000);
            }
            if (first != null && last != null && newWidths != null)
            {
                int f = first.IntValue;
                for (int k = 0; k < newWidths.Size; ++k)
                {
                    widths[f + k] = newWidths.GetAsNumber(k).IntValue;
                }
            }
            fillFontDesc(_font.GetAsDict(PdfName.Fontdescriptor));
        }

        private void fillEncoding(PdfName encoding)
        {
            if (PdfName.MacRomanEncoding.Equals(encoding) || PdfName.WinAnsiEncoding.Equals(encoding))
            {
                byte[] b = new byte[256];
                for (int k = 0; k < 256; ++k)
                    b[k] = (byte)k;
                string enc = WINANSI;
                if (PdfName.MacRomanEncoding.Equals(encoding))
                    enc = MACROMAN;
                string cv = PdfEncodings.ConvertToString(b, enc);
                char[] arr = cv.ToCharArray();
                for (int k = 0; k < 256; ++k)
                {
                    Uni2Byte[arr[k]] = k;
                }
            }
            else
            {
                for (int k = 0; k < 256; ++k)
                {
                    Uni2Byte[_stdEnc[k]] = k;
                }
            }
        }

        private void fillFontDesc(PdfDictionary fontDesc)
        {
            if (fontDesc == null)
                return;
            PdfNumber v = fontDesc.GetAsNumber(PdfName.Ascent);
            if (v != null)
                _ascender = v.FloatValue;
            v = fontDesc.GetAsNumber(PdfName.Capheight);
            if (v != null)
                _capHeight = v.FloatValue;
            v = fontDesc.GetAsNumber(PdfName.Descent);
            if (v != null)
                _descender = v.FloatValue;
            v = fontDesc.GetAsNumber(PdfName.Italicangle);
            if (v != null)
                _italicAngle = v.FloatValue;
            PdfArray bbox = fontDesc.GetAsArray(PdfName.Fontbbox);
            if (bbox != null)
            {
                _llx = bbox.GetAsNumber(0).FloatValue;
                _lly = bbox.GetAsNumber(1).FloatValue;
                _urx = bbox.GetAsNumber(2).FloatValue;
                _ury = bbox.GetAsNumber(3).FloatValue;
                if (_llx > _urx)
                {
                    float t = _llx;
                    _llx = _urx;
                    _urx = t;
                }
                if (_lly > _ury)
                {
                    float t = _lly;
                    _lly = _ury;
                    _ury = t;
                }
            }
        }

        private void fillMetrics(byte[] touni, IntHashtable widths, int dw)
        {
            PdfContentParser ps = new PdfContentParser(new PrTokeniser(touni));
            PdfObject ob = null;
            PdfObject last = null;
            while ((ob = ps.ReadPrObject()) != null)
            {
                if (ob.Type == PdfContentParser.COMMAND_TYPE)
                {
                    if (ob.ToString().Equals("beginbfchar"))
                    {
                        int n = ((PdfNumber)last).IntValue;
                        for (int k = 0; k < n; ++k)
                        {
                            string cid = decodeString((PdfString)ps.ReadPrObject());
                            string uni = decodeString((PdfString)ps.ReadPrObject());
                            if (uni.Length == 1)
                            {
                                int cidc = cid[0];
                                int unic = uni[uni.Length - 1];
                                int w = dw;
                                if (widths.ContainsKey(cidc))
                                    w = widths[cidc];
                                _metrics[unic] = new[] { cidc, w };
                            }
                        }
                    }
                    else if (ob.ToString().Equals("beginbfrange"))
                    {
                        int n = ((PdfNumber)last).IntValue;
                        for (int k = 0; k < n; ++k)
                        {
                            string cid1 = decodeString((PdfString)ps.ReadPrObject());
                            string cid2 = decodeString((PdfString)ps.ReadPrObject());
                            int cid1C = cid1[0];
                            int cid2C = cid2[0];
                            PdfObject ob2 = ps.ReadPrObject();
                            if (ob2.IsString())
                            {
                                string uni = decodeString((PdfString)ob2);
                                if (uni.Length == 1)
                                {
                                    int unic = uni[uni.Length - 1];
                                    for (; cid1C <= cid2C; cid1C++, unic++)
                                    {
                                        int w = dw;
                                        if (widths.ContainsKey(cid1C))
                                            w = widths[cid1C];
                                        _metrics[unic] = new[] { cid1C, w };
                                    }
                                }
                            }
                            else
                            {
                                PdfArray a = (PdfArray)ob2;
                                for (int j = 0; j < a.Size; ++j, ++cid1C)
                                {
                                    string uni = decodeString(a.GetAsString(j));
                                    if (uni.Length == 1)
                                    {
                                        int unic = uni[uni.Length - 1];
                                        int w = dw;
                                        if (widths.ContainsKey(cid1C))
                                            w = widths[cid1C];
                                        _metrics[unic] = new[] { cid1C, w };
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    last = ob;
            }
        }

        private void processType0(PdfDictionary font)
        {
            PdfObject toUniObject = PdfReader.GetPdfObjectRelease(font.Get(PdfName.Tounicode));
            PdfArray df = (PdfArray)PdfReader.GetPdfObjectRelease(font.Get(PdfName.Descendantfonts));
            PdfDictionary cidft = (PdfDictionary)PdfReader.GetPdfObjectRelease(df[0]);
            PdfNumber dwo = (PdfNumber)PdfReader.GetPdfObjectRelease(cidft.Get(PdfName.Dw));
            int dw = 1000;
            if (dwo != null)
                dw = dwo.IntValue;
            IntHashtable localWidths = readWidths((PdfArray)PdfReader.GetPdfObjectRelease(cidft.Get(PdfName.W)));
            PdfDictionary fontDesc = (PdfDictionary)PdfReader.GetPdfObjectRelease(cidft.Get(PdfName.Fontdescriptor));
            fillFontDesc(fontDesc);
            if (toUniObject != null)
            {
                fillMetrics(PdfReader.GetStreamBytes((PrStream)toUniObject), localWidths, dw);
            }
        }

        private IntHashtable readWidths(PdfArray ws)
        {
            IntHashtable hh = new IntHashtable();
            if (ws == null)
                return hh;
            for (int k = 0; k < ws.Size; ++k)
            {
                int c1 = ((PdfNumber)PdfReader.GetPdfObjectRelease(ws[k])).IntValue;
                PdfObject obj = PdfReader.GetPdfObjectRelease(ws[++k]);
                if (obj.IsArray())
                {
                    PdfArray a2 = (PdfArray)obj;
                    for (int j = 0; j < a2.Size; ++j)
                    {
                        int c2 = ((PdfNumber)PdfReader.GetPdfObjectRelease(a2[j])).IntValue;
                        hh[c1++] = c2;
                    }
                }
                else
                {
                    int c2 = ((PdfNumber)obj).IntValue;
                    int w = ((PdfNumber)PdfReader.GetPdfObjectRelease(ws[++k])).IntValue;
                    for (; c1 <= c2; ++c1)
                        hh[c1] = w;
                }
            }
            return hh;
        }
    }
}
