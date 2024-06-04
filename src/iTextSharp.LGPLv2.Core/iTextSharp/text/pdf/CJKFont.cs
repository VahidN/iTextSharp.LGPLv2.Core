using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Creates a CJK font compatible with the fonts in the Adobe Asian font Pack.
///     @author  Paulo Soares (psoares@consiste.pt)
/// </summary>
internal class CjkFont : BaseFont
{
    /// <summary>
    ///     The encoding used in the PDF document for CJK fonts
    /// </summary>
    internal const string CJK_ENCODING = "UNICODEBIGUNMARKED";

    private const int Bracket = 1;
    private const int First = 0;
    private const int Serial = 2;
    private const int V1Y = 880;
    internal static readonly INullValueDictionary<string, char[]> AllCMaps = new NullValueDictionary<string, char[]>();

    internal static readonly INullValueDictionary<string, INullValueDictionary<string, object>> AllFonts =
        new NullValueDictionary<string, INullValueDictionary<string, object>>();

    internal static Properties CjkEncodings = new();
    internal static Properties CjkFonts = new();
    private static bool _propertiesLoaded;

    private readonly bool _cidDirect;

    /// <summary>
    ///     The CMap name associated with this font
    /// </summary>
    private readonly string _cMap;

    private readonly INullValueDictionary<string, object> _fontDesc;

    private readonly NullValueDictionary<int, int> _hMetrics;

    /// <summary>
    ///     The style modifier
    /// </summary>
    private readonly string _style = "";

    private readonly char[] _translationMap;

    private readonly bool _vertical;

    private readonly NullValueDictionary<int, int> _vMetrics;

    /// <summary>
    ///     The font name
    /// </summary>
    private string _fontName;

    /// <summary>
    ///     Creates a CJK font.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="fontName">the name of the font</param>
    /// <param name="enc">the encoding of the font</param>
    /// <param name="emb">always  false . CJK font and not embedded</param>
    internal CjkFont(string fontName, string enc, bool emb)
    {
        loadProperties();
        FontType = FONT_TYPE_CJK;
        var nameBase = GetBaseName(fontName);
        if (!IsCjkFont(nameBase, enc))
        {
            throw new DocumentException("Font '" + fontName + "' with '" + enc + "' encoding is not a CJK font.");
        }

        if (nameBase.Length < fontName.Length)
        {
            _style = fontName.Substring(nameBase.Length);
            fontName = nameBase;
        }

        _fontName = fontName;
        encoding = CJK_ENCODING;
        _vertical = enc.EndsWith("V", StringComparison.Ordinal);
        _cMap = enc;
        if (enc.StartsWith("Identity-", StringComparison.Ordinal))
        {
            _cidDirect = true;
            var s = CjkFonts[fontName];
            s = s.Substring(0, s.IndexOf("_", StringComparison.Ordinal));
            var c = AllCMaps[s];
            if (c == null)
            {
                c = ReadCMap(s);
                if (c == null)
                {
                    throw new DocumentException("The cmap " + s + " does not exist as a resource.");
                }

                c[CID_NEWLINE] = '\n';
                AllCMaps.Add(s, c);
            }

            _translationMap = c;
        }
        else
        {
            var c = AllCMaps[enc];
            if (c == null)
            {
                var s = CjkEncodings[enc];
                if (s == null)
                {
                    throw new DocumentException("The resource cjkencodings.properties does not contain the encoding " +
                                                enc);
                }

                var tk = new StringTokenizer(s);
                var nt = tk.NextToken();
                c = AllCMaps[nt];
                if (c == null)
                {
                    c = ReadCMap(nt);
                    AllCMaps.Add(nt, c);
                }

                if (tk.HasMoreTokens())
                {
                    var nt2 = tk.NextToken();
                    var m2 = ReadCMap(nt2);
                    for (var k = 0; k < 0x10000; ++k)
                    {
                        if (m2[k] == 0)
                        {
                            m2[k] = c[k];
                        }
                    }

                    AllCMaps.Add(enc, m2);
                    c = m2;
                }
            }

            _translationMap = c;
        }

        _fontDesc = AllFonts[fontName];
        if (_fontDesc == null)
        {
            _fontDesc = ReadFontProperties(fontName);
            AllFonts.Add(fontName, _fontDesc);
        }

        _hMetrics = (NullValueDictionary<int, int>)_fontDesc["W"];
        _vMetrics = (NullValueDictionary<int, int>)_fontDesc["W2"];
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
    public override string[][] AllNameEntries
    {
        get { return new[] { new[] { "4", "", "", "", _fontName } }; }
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
    public override string[][] FamilyFontName => FullFontName;

    /// <summary>
    ///     Gets the full name of the font. If it is a True Type font
    ///     each array element will have {Platform ID, Platform Encoding ID,
    ///     Language ID, font name}. The interpretation of this values can be
    ///     found in the Open Type specification, chapter 2, in the 'name' table.
    ///     For the other fonts the array has a single element with {"", "", "",
    ///     font name}.
    /// </summary>
    /// <returns>the full name of the font</returns>
    public override string[][] FullFontName
    {
        get { return new[] { new[] { "", "", "", _fontName } }; }
    }

    public override string PostscriptFontName
    {
        get => _fontName;
        set => _fontName = value;
    }

    /// <summary>
    ///     Checks if its a valid CJK font.
    /// </summary>
    /// <param name="fontName">the font name</param>
    /// <param name="enc">the encoding</param>
    /// <returns> true  if it is CJK font</returns>
    public static bool IsCjkFont(string fontName, string enc)
    {
        loadProperties();
        var encodings = CjkFonts[fontName];
        return encodings != null && (enc.Equals("Identity-H", StringComparison.Ordinal) ||
                                     enc.Equals("Identity-V", StringComparison.Ordinal) ||
                                     encodings.IndexOf($"_{enc}_", StringComparison.OrdinalIgnoreCase) >= 0);
    }

    public override bool CharExists(int c) => _translationMap[c] != 0;

    public override int[] GetCharBBox(int c) => null;

    public override int GetCidCode(int c)
    {
        if (_cidDirect)
        {
            return c;
        }

        return _translationMap[c];
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
            case AWT_ASCENT:
            case ASCENT:
                return getDescNumber("Ascent") * fontSize / 1000;
            case CAPHEIGHT:
                return getDescNumber("CapHeight") * fontSize / 1000;
            case AWT_DESCENT:
            case DESCENT:
                return getDescNumber("Descent") * fontSize / 1000;
            case ITALICANGLE:
                return getDescNumber("ItalicAngle");
            case BBOXLLX:
                return fontSize * getBBox(0) / 1000;
            case BBOXLLY:
                return fontSize * getBBox(1) / 1000;
            case BBOXURX:
                return fontSize * getBBox(2) / 1000;
            case BBOXURY:
                return fontSize * getBBox(3) / 1000;
            case AWT_LEADING:
                return 0;
            case AWT_MAXADVANCE:
                return fontSize * (getBBox(2) - getBBox(0)) / 1000;
        }

        return 0;
    }

    /// <summary>
    ///     You can't get the FontStream of a CJK font (CJK fonts are never embedded),
    ///     so this method always returns null.
    ///     @since   2.1.3
    /// </summary>
    /// <returns>null</returns>
    public override PdfStream GetFullFontStream() => null;

    public override int GetKerning(int char1, int char2) => 0;

    public override int GetUnicodeEquivalent(int c)
    {
        if (_cidDirect)
        {
            return _translationMap[c];
        }

        return c;
    }

    /// <summary>
    ///     Gets the width of a  char  in normalized 1000 units.
    /// </summary>
    /// <param name="char1">the unicode  char  to get the width of</param>
    /// <returns>the width in normalized 1000 units</returns>
    public override int GetWidth(int char1)
    {
        var c = char1;
        if (!_cidDirect)
        {
            c = _translationMap[c];
        }

        int v;
        if (_vertical)
        {
            v = _vMetrics[c];
        }
        else
        {
            v = _hMetrics[c];
        }

        if (v > 0)
        {
            return v;
        }

        return 1000;
    }

    public override int GetWidth(string text)
    {
        var total = 0;
        for (var k = 0; k < text.Length; ++k)
        {
            int c = text[k];
            if (!_cidDirect)
            {
                c = _translationMap[c];
            }

            int v;
            if (_vertical)
            {
                v = _vMetrics[c];
            }
            else
            {
                v = _hMetrics[c];
            }

            if (v > 0)
            {
                total += v;
            }
            else
            {
                total += 1000;
            }
        }

        return total;
    }

    public override bool HasKernPairs() => false;

    public override bool SetCharAdvance(int c, int advance) => false;

    public override bool SetKerning(int char1, int char2, int kern) => false;

    internal static string ConvertToHcidMetrics(IList<int> keys, NullValueDictionary<int, int> h)
    {
        if (keys.Count == 0)
        {
            return null;
        }

        var lastCid = 0;
        var lastValue = 0;
        int start;
        for (start = 0; start < keys.Count; ++start)
        {
            lastCid = keys[start];
            lastValue = h[lastCid];
            if (lastValue != 0)
            {
                ++start;
                break;
            }
        }

        if (lastValue == 0)
        {
            return null;
        }

        var buf = new StringBuilder();
        buf.Append('[');
        buf.Append(lastCid);
        var state = First;
        for (var k = start; k < keys.Count; ++k)
        {
            var cid = keys[k];
            var value = h[cid];
            if (value == 0)
            {
                continue;
            }

            switch (state)
            {
                case First:
                {
                    if (cid == lastCid + 1 && value == lastValue)
                    {
                        state = Serial;
                    }
                    else if (cid == lastCid + 1)
                    {
                        state = Bracket;
                        buf.Append('[').Append(lastValue);
                    }
                    else
                    {
                        buf.Append('[').Append(lastValue).Append(']').Append(cid);
                    }

                    break;
                }
                case Bracket:
                {
                    if (cid == lastCid + 1 && value == lastValue)
                    {
                        state = Serial;
                        buf.Append(']').Append(lastCid);
                    }
                    else if (cid == lastCid + 1)
                    {
                        buf.Append(' ').Append(lastValue);
                    }
                    else
                    {
                        state = First;
                        buf.Append(' ').Append(lastValue).Append(']').Append(cid);
                    }

                    break;
                }
                case Serial:
                {
                    if (cid != lastCid + 1 || value != lastValue)
                    {
                        buf.Append(' ').Append(lastCid).Append(' ').Append(lastValue).Append(' ').Append(cid);
                        state = First;
                    }

                    break;
                }
            }

            lastValue = value;
            lastCid = cid;
        }

        switch (state)
        {
            case First:
            {
                buf.Append('[').Append(lastValue).Append("]]");
                break;
            }
            case Bracket:
            {
                buf.Append(' ').Append(lastValue).Append("]]");
                break;
            }
            case Serial:
            {
                buf.Append(' ').Append(lastCid).Append(' ').Append(lastValue).Append(']');
                break;
            }
        }

        return buf.ToString();
    }

    internal static string ConvertToVcidMetrics(IList<int> keys,
                                                NullValueDictionary<int, int> v,
                                                NullValueDictionary<int, int> h)
    {
        if (keys.Count == 0)
        {
            return null;
        }

        var lastCid = 0;
        var lastValue = 0;
        var lastHValue = 0;
        int start;
        for (start = 0; start < keys.Count; ++start)
        {
            lastCid = keys[start];
            lastValue = v[lastCid];
            if (lastValue != 0)
            {
                ++start;
                break;
            }

            lastHValue = h[lastCid];
        }

        if (lastValue == 0)
        {
            return null;
        }

        if (lastHValue == 0)
        {
            lastHValue = 1000;
        }

        var buf = new StringBuilder();
        buf.Append('[');
        buf.Append(lastCid);
        var state = First;
        for (var k = start; k < keys.Count; ++k)
        {
            var cid = keys[k];
            var value = v[cid];
            if (value == 0)
            {
                continue;
            }

            var hValue = h[lastCid];
            if (hValue == 0)
            {
                hValue = 1000;
            }

            switch (state)
            {
                case First:
                {
                    if (cid == lastCid + 1 && value == lastValue && hValue == lastHValue)
                    {
                        state = Serial;
                    }
                    else
                    {
                        buf.Append(' ').Append(lastCid).Append(' ').Append(-lastValue).Append(' ')
                           .Append(lastHValue / 2).Append(' ').Append(V1Y).Append(' ').Append(cid);
                    }

                    break;
                }
                case Serial:
                {
                    if (cid != lastCid + 1 || value != lastValue || hValue != lastHValue)
                    {
                        buf.Append(' ').Append(lastCid).Append(' ').Append(-lastValue).Append(' ')
                           .Append(lastHValue / 2).Append(' ').Append(V1Y).Append(' ').Append(cid);
                        state = First;
                    }

                    break;
                }
            }

            lastValue = value;
            lastCid = cid;
            lastHValue = hValue;
        }

        buf.Append(' ').Append(lastCid).Append(' ').Append(-lastValue).Append(' ').Append(lastHValue / 2).Append(' ')
           .Append(V1Y).Append(" ]");
        return buf.ToString();
    }

    internal static NullValueDictionary<int, int> CreateMetric(string s)
    {
        var h = new NullValueDictionary<int, int>();
        var tk = new StringTokenizer(s);
        while (tk.HasMoreTokens())
        {
            var n1 = int.Parse(tk.NextToken(), CultureInfo.InvariantCulture);
            h[n1] = int.Parse(tk.NextToken(), CultureInfo.InvariantCulture);
        }

        return h;
    }

    internal static char[] ReadCMap(string name)
    {
        try
        {
            name = name + ".cmap";
            using var istr = GetResourceStream(RESOURCE_PATH + name);
            var c = new char[0x10000];
            for (var k = 0; k < 0x10000; ++k)
            {
                c[k] = (char)((istr.ReadByte() << 8) + istr.ReadByte());
            }

            return c;
        }
        catch
        {
            // empty on purpose
        }

        return null;
    }

    internal static INullValueDictionary<string, object> ReadFontProperties(string name)
    {
        name += ".properties";
        using var isp = GetResourceStream(RESOURCE_PATH + name);
        if (isp == null)
        {
            return null;
        }

        var p = new Properties();
        p.Load(isp);

        var w = CreateMetric(p["W"]);
        p.Remove("W");
        var w2 = CreateMetric(p["W2"]);
        p.Remove("W2");
        var map = new NullValueDictionary<string, object>();
        foreach (var key in p.Keys)
        {
            map[key] = p[key];
        }

        map["W"] = w;
        map["W2"] = w2;
        return map;
    }

    internal override int GetRawWidth(int c, string name) => 0;

    internal override void WriteFont(PdfWriter writer, PdfIndirectReference piref, object[] parms)
    {
        var cjkTag = (NullValueDictionary<int, int>)parms[0];
        PdfIndirectReference indFont = null;
        PdfObject pobj = null;
        PdfIndirectObject obj = null;
        pobj = getFontDescriptor();
        if (pobj != null)
        {
            obj = writer.AddToBody(pobj);
            indFont = obj.IndirectReference;
        }

        pobj = getCidFont(indFont, cjkTag);
        if (pobj != null)
        {
            obj = writer.AddToBody(pobj);
            indFont = obj.IndirectReference;
        }

        pobj = getFontBaseType(indFont);
        writer.AddToBody(pobj, piref);
    }

    protected override int[] GetRawCharBBox(int c, string name) => null;

    private static void loadProperties()
    {
        if (_propertiesLoaded)
        {
            return;
        }

        lock (AllFonts)
        {
            if (_propertiesLoaded)
            {
                return;
            }

            try
            {
                using var isp = GetResourceStream(RESOURCE_PATH + "cjkfonts.properties");
                if (isp != null)
                {
                    CjkFonts.Load(isp);

                    using var stream = GetResourceStream(RESOURCE_PATH + "cjkencodings.properties");
                    if (stream != null)
                    {
                        CjkEncodings.Load(stream);
                    }
                }
            }
            catch
            {
                CjkFonts = new Properties();
                CjkEncodings = new Properties();
            }

            _propertiesLoaded = true;
        }
    }

    private float getBBox(int idx)
    {
        var s = (string)_fontDesc["FontBBox"];
        var tk = new StringTokenizer(s, " []\r\n\t\f");
        var ret = tk.NextToken();
        for (var k = 0; k < idx; ++k)
        {
            ret = tk.NextToken();
        }

        return int.Parse(ret, CultureInfo.InvariantCulture);
    }

    private PdfDictionary getCidFont(PdfIndirectReference fontDescriptor, NullValueDictionary<int, int> cjkTag)
    {
        var dic = new PdfDictionary(PdfName.Font);
        dic.Put(PdfName.Subtype, PdfName.Cidfonttype0);
        dic.Put(PdfName.Basefont, new PdfName(_fontName + _style));
        dic.Put(PdfName.Fontdescriptor, fontDescriptor);
        var keys = cjkTag.ToOrderedKeys();
        var w = ConvertToHcidMetrics(keys, _hMetrics);
        if (w != null)
        {
            dic.Put(PdfName.W, new PdfLiteral(w));
        }

        if (_vertical)
        {
            w = ConvertToVcidMetrics(keys, _vMetrics, _hMetrics);
            if (w != null)
            {
                dic.Put(PdfName.W2, new PdfLiteral(w));
            }
        }
        else
        {
            dic.Put(PdfName.Dw, new PdfNumber(1000));
        }

        var cdic = new PdfDictionary();
        cdic.Put(PdfName.Registry, new PdfString((string)_fontDesc["Registry"], null));
        cdic.Put(PdfName.Ordering, new PdfString((string)_fontDesc["Ordering"], null));
        cdic.Put(PdfName.Supplement, new PdfLiteral((string)_fontDesc["Supplement"]));
        dic.Put(PdfName.Cidsysteminfo, cdic);
        return dic;
    }

    private float getDescNumber(string name) => int.Parse((string)_fontDesc[name], CultureInfo.InvariantCulture);

    private PdfDictionary getFontBaseType(PdfIndirectReference cidFont)
    {
        var dic = new PdfDictionary(PdfName.Font);
        dic.Put(PdfName.Subtype, PdfName.Type0);
        var name = _fontName;
        if (_style.Length > 0)
        {
            name += "-" + _style.Substring(1);
        }

        name += "-" + _cMap;
        dic.Put(PdfName.Basefont, new PdfName(name));
        dic.Put(PdfName.Encoding, new PdfName(_cMap));
        dic.Put(PdfName.Descendantfonts, new PdfArray(cidFont));
        return dic;
    }

    private PdfDictionary getFontDescriptor()
    {
        var dic = new PdfDictionary(PdfName.Fontdescriptor);
        dic.Put(PdfName.Ascent, new PdfLiteral((string)_fontDesc["Ascent"]));
        dic.Put(PdfName.Capheight, new PdfLiteral((string)_fontDesc["CapHeight"]));
        dic.Put(PdfName.Descent, new PdfLiteral((string)_fontDesc["Descent"]));
        dic.Put(PdfName.Flags, new PdfLiteral((string)_fontDesc["Flags"]));
        dic.Put(PdfName.Fontbbox, new PdfLiteral((string)_fontDesc["FontBBox"]));
        dic.Put(PdfName.Fontname, new PdfName(_fontName + _style));
        dic.Put(PdfName.Italicangle, new PdfLiteral((string)_fontDesc["ItalicAngle"]));
        dic.Put(PdfName.Stemv, new PdfLiteral((string)_fontDesc["StemV"]));
        var pdic = new PdfDictionary();
        pdic.Put(PdfName.Panose, new PdfString((string)_fontDesc["Panose"], null));
        dic.Put(PdfName.Style, pdic);
        return dic;
    }
}