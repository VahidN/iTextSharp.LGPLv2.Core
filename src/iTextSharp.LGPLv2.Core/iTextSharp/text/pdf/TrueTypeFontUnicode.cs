using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Represents a True Type font with Unicode encoding. All the character
///     in the font can be used directly by using the encoding Identity-H or
///     Identity-V. This is the only way to represent some character sets such
///     as Thai.
///     @author  Paulo Soares (psoares@consiste.pt)
/// </summary>
internal class TrueTypeFontUnicode : TrueTypeFont, IComparer<int[]>
{
    private static readonly byte[] _rotbits = { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

    /// <summary>
    ///     true  if the encoding is vertical.
    /// </summary>
    private readonly bool _vertical;

    /// <summary>
    ///     Creates a new TrueType font addressed by Unicode characters. The font
    ///     will always be embedded.
    ///     The modifiers after the name are ignored.
    ///     @throws DocumentException the font is invalid
    ///     @throws IOException the font file could not be read
    /// </summary>
    /// <param name="ttFile">the location of the font on file. The file must end in '.ttf'.</param>
    /// <param name="enc">the encoding to be applied to this font</param>
    /// <param name="emb">true if the font is to be embedded in the PDF</param>
    /// <param name="ttfAfm">the font as a  byte  array</param>
    /// <param name="forceRead"></param>
    internal TrueTypeFontUnicode(string ttFile, string enc, bool emb, byte[] ttfAfm, bool forceRead)
    {
        var nameBase = GetBaseName(ttFile);
        var ttcName = GetTtcName(nameBase);
        if (nameBase.Length < ttFile.Length)
        {
            Style = ttFile.Substring(nameBase.Length);
        }

        encoding = enc;
        Embedded = emb;
        FileName = ttcName;
        TtcIndex = "";
        if (ttcName.Length < nameBase.Length)
        {
            TtcIndex = nameBase.Substring(ttcName.Length + 1);
        }

        FontType = FONT_TYPE_TTUNI;
        if ((FileName.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase) ||
             FileName.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) ||
             FileName.EndsWith(".ttc", StringComparison.OrdinalIgnoreCase)) &&
            (enc.Equals(IDENTITY_H, StringComparison.Ordinal) || enc.Equals(IDENTITY_V, StringComparison.Ordinal)) &&
            emb)
        {
            Process(ttfAfm, forceRead);
            if (Os2.FsType == 2)
            {
                throw new DocumentException(FileName + Style + " cannot be embedded due to licensing restrictions.");
            }

            // Sivan
            if ((Cmap31 == null && !FontSpecific) || (Cmap10 == null && FontSpecific))
            {
                directTextToByte = true;
            }

            //throw new DocumentException(fileName + " " + style + " does not contain an usable cmap.");
            if (FontSpecific)
            {
                FontSpecific = false;
                var tempEncoding = encoding;
                encoding = "";
                CreateEncoding();
                encoding = tempEncoding;
                FontSpecific = true;
            }
        }
        else
        {
            throw new DocumentException(FileName + " " + Style + " is not a TTF font file.");
        }

        _vertical = enc.EndsWith("V", StringComparison.Ordinal);
    }

    /// <summary>
    ///     The method used to sort the metrics array.
    /// </summary>
    /// <param name="o1">the first element</param>
    /// <param name="o2">the second element</param>
    /// <returns>the comparisation</returns>
    public int Compare(int[] o1, int[] o2)
    {
        var m1 = o1[0];
        var m2 = o2[0];
        if (m1 < m2)
        {
            return -1;
        }

        if (m1 == m2)
        {
            return 0;
        }

        return 1;
    }

    /// <summary>
    ///     Checks if a character exists in this font.
    ///     false  otherwise
    /// </summary>
    /// <param name="c">the character to check</param>
    /// <returns> true  if the character has a glyph,</returns>
    public override bool CharExists(int c) => GetMetricsTt(c) != null;

    public override int[] GetCharBBox(int c)
    {
        if (Bboxes == null)
        {
            return null;
        }

        var m = GetMetricsTt(c);
        if (m == null)
        {
            return null;
        }

        return Bboxes[m[0]];
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
            return new StreamFont(ReadCffFont(), "CIDFontType0C", compressionLevel);
        }

        return base.GetFullFontStream();
    }

    /// <summary>
    ///     Gets the glyph index and metrics for a character.
    /// </summary>
    /// <param name="c">the character</param>
    /// <returns>an  int  array with {glyph index, width}</returns>
    public override int[] GetMetricsTt(int c)
    {
        if (CmapExt != null)
        {
            return CmapExt[c];
        }

        INullValueDictionary<int, int[]> map = null;
        if (FontSpecific)
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

        if (FontSpecific)
        {
            if ((c & 0xffffff00) == 0 || (c & 0xffffff00) == 0xf000)
            {
                return map[c & 0xff];
            }

            return null;
        }

        return map[c];
    }

    /// <summary>
    ///     Gets the width of a  char  in normalized 1000 units.
    /// </summary>
    /// <param name="char1">the unicode  char  to get the width of</param>
    /// <returns>the width in normalized 1000 units</returns>
    public override int GetWidth(int char1)
    {
        if (_vertical)
        {
            return 1000;
        }

        if (FontSpecific)
        {
            if ((char1 & 0xff00) == 0 || (char1 & 0xff00) == 0xf000)
            {
                return GetRawWidth(char1 & 0xff, null);
            }

            return 0;
        }

        return GetRawWidth(char1, encoding);
    }

    /// <summary>
    ///     Gets the width of a  string  in normalized 1000 units.
    /// </summary>
    /// <param name="text">the  string  to get the witdth of</param>
    /// <returns>the width in normalized 1000 units</returns>
    public override int GetWidth(string text)
    {
        if (_vertical)
        {
            return text.Length * 1000;
        }

        var total = 0;
        if (FontSpecific)
        {
            var cc = text.ToCharArray();
            var len = cc.Length;
            for (var k = 0; k < len; ++k)
            {
                var c = cc[k];
                if ((c & 0xff00) == 0 || (c & 0xff00) == 0xf000)
                {
                    total += GetRawWidth(c & 0xff, null);
                }
            }
        }
        else
        {
            var len = text.Length;
            for (var k = 0; k < len; ++k)
            {
                if (Utilities.IsSurrogatePair(text, k))
                {
                    total += GetRawWidth(Utilities.ConvertToUtf32(text, k), encoding);
                    ++k;
                }
                else
                {
                    total += GetRawWidth(text[k], encoding);
                }
            }
        }

        return total;
    }

    /// <summary>
    ///     Sets the character advance.
    ///     false  otherwise
    /// </summary>
    /// <param name="c">the character</param>
    /// <param name="advance">the character advance normalized to 1000 units</param>
    /// <returns> true  if the advance was set,</returns>
    public override bool SetCharAdvance(int c, int advance)
    {
        var m = GetMetricsTt(c);
        if (m == null)
        {
            return false;
        }

        m[1] = advance;
        return true;
    }

    /// <summary>
    ///     Gets an hex string in the format "&lt;HHHH&gt;".
    /// </summary>
    /// <param name="n">the number</param>
    /// <returns>the hex string</returns>
    internal static string ToHex(int n)
    {
        if (n < 0x10000)
        {
            return "<" + Convert.ToString(n, 16).PadLeft(4, '0') + ">";
        }

        n -= 0x10000;
        var high = n / 0x400 + 0xd800;
        var low = n % 0x400 + 0xdc00;
        return "[<" + Convert.ToString(high, 16).PadLeft(4, '0') + Convert.ToString(low, 16).PadLeft(4, '0') + ">]";
    }

    /// <summary>
    ///     A forbidden operation. Will throw a null pointer exception.
    /// </summary>
    /// <param name="text">the text</param>
    /// <returns>always  null </returns>
    internal override byte[] ConvertToBytes(string text) => null;

    internal override byte[] ConvertToBytes(int char1) => null;

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
        var longTag = (INullValueDictionary<int, int[]>)parms[0];
        AddRangeUni(longTag, true, subset);
        var tmp = new List<int[]>();
        foreach (var o in longTag.Values)
        {
            tmp.Add(o);
        }

        var metrics = tmp.Where(m => m != null).ToArray();
        Array.Sort(metrics, this);
        PdfIndirectReference indFont = null;
        PdfObject pobj = null;
        PdfIndirectObject obj = null;
        PdfIndirectReference cidset = null;
        if (writer.PdfxConformance == PdfWriter.PDFA1A || writer.PdfxConformance == PdfWriter.PDFA1B)
        {
            PdfStream stream;
            if (metrics.Length == 0)
            {
                stream = new PdfStream(new[] { (byte)0x80 });
            }
            else
            {
                var top = metrics[metrics.Length - 1][0];
                var bt = new byte[top / 8 + 1];
                for (var k = 0; k < metrics.Length; ++k)
                {
                    var v = metrics[k][0];
                    bt[v / 8] |= _rotbits[v % 8];
                }

                stream = new PdfStream(bt);
                stream.FlateCompress(compressionLevel);
            }

            cidset = writer.AddToBody(stream).IndirectReference;
        }

        // sivan: cff
        if (Cff)
        {
            var b = ReadCffFont();
            if (subset || SubsetRanges != null)
            {
                var cffs = new CffFontSubset(new RandomAccessFileOrArray(b), longTag);
                b = cffs.Process(cffs.GetNames()[0]);
            }

            pobj = new StreamFont(b, "CIDFontType0C", compressionLevel);
            obj = writer.AddToBody(pobj);
            indFont = obj.IndirectReference;
        }
        else
        {
            byte[] b;
            if (subset || DirectoryOffset != 0)
            {
                var sb = new TrueTypeFontSubSet(FileName, new RandomAccessFileOrArray(Rf), longTag, DirectoryOffset,
                                                false, false);
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

        var subsetPrefix = "";
        if (subset)
        {
            subsetPrefix = CreateSubsetPrefix();
        }

        var dic = GetFontDescriptor(indFont, subsetPrefix, cidset);
        obj = writer.AddToBody(dic);
        indFont = obj.IndirectReference;

        pobj = getCidFontType2(indFont, subsetPrefix, metrics);
        obj = writer.AddToBody(pobj);
        indFont = obj.IndirectReference;

        pobj = getToUnicode(metrics);
        PdfIndirectReference toUnicodeRef = null;
        if (pobj != null)
        {
            obj = writer.AddToBody(pobj);
            toUnicodeRef = obj.IndirectReference;
        }

        pobj = getFontBaseType(indFont, subsetPrefix, toUnicodeRef);
        writer.AddToBody(pobj, piref);
    }

    /// <summary>
    ///     Generates the CIDFontTyte2 dictionary.
    /// </summary>
    /// <param name="fontDescriptor">the indirect reference to the font descriptor</param>
    /// <param name="subsetPrefix">the subset prefix</param>
    /// <param name="metrics">the horizontal width metrics</param>
    /// <returns>a stream</returns>
    private PdfDictionary getCidFontType2(PdfIndirectReference fontDescriptor, string subsetPrefix, object[] metrics)
    {
        var dic = new PdfDictionary(PdfName.Font);
        // sivan; cff
        if (Cff)
        {
            dic.Put(PdfName.Subtype, PdfName.Cidfonttype0);
            dic.Put(PdfName.Basefont, new PdfName(subsetPrefix + FontName + "-" + encoding));
        }
        else
        {
            dic.Put(PdfName.Subtype, PdfName.Cidfonttype2);
            dic.Put(PdfName.Basefont, new PdfName(subsetPrefix + FontName));
        }

        dic.Put(PdfName.Fontdescriptor, fontDescriptor);
        if (!Cff)
        {
            dic.Put(PdfName.Cidtogidmap, PdfName.Identity);
        }

        var cdic = new PdfDictionary();
        cdic.Put(PdfName.Registry, new PdfString("Adobe"));
        cdic.Put(PdfName.Ordering, new PdfString("Identity"));
        cdic.Put(PdfName.Supplement, new PdfNumber(0));
        dic.Put(PdfName.Cidsysteminfo, cdic);
        if (!_vertical)
        {
            dic.Put(PdfName.Dw, new PdfNumber(1000));
            var buf = new StringBuilder("[");
            var lastNumber = -10;
            var firstTime = true;
            for (var k = 0; k < metrics.Length; ++k)
            {
                var metric = (int[])metrics[k];
                if (metric[1] == 1000)
                {
                    continue;
                }

                var m = metric[0];
                if (m == lastNumber + 1)
                {
                    buf.Append(' ').Append(metric[1]);
                }
                else
                {
                    if (!firstTime)
                    {
                        buf.Append(']');
                    }

                    firstTime = false;
                    buf.Append(m).Append('[').Append(metric[1]);
                }

                lastNumber = m;
            }

            if (buf.Length > 1)
            {
                buf.Append("]]");
                dic.Put(PdfName.W, new PdfLiteral(buf.ToString()));
            }
        }

        return dic;
    }

    /// <summary>
    ///     Generates the font dictionary.
    /// </summary>
    /// <param name="descendant">the descendant dictionary</param>
    /// <param name="subsetPrefix">the subset prefix</param>
    /// <param name="toUnicode">the ToUnicode stream</param>
    /// <returns>the stream</returns>
    private PdfDictionary getFontBaseType(PdfIndirectReference descendant, string subsetPrefix,
                                          PdfIndirectReference toUnicode)
    {
        var dic = new PdfDictionary(PdfName.Font);

        dic.Put(PdfName.Subtype, PdfName.Type0);
        // The PDF Reference manual advises to add -encoding to CID font names
        if (Cff)
        {
            dic.Put(PdfName.Basefont, new PdfName(subsetPrefix + FontName + "-" + encoding));
        }
        else
        {
            dic.Put(PdfName.Basefont, new PdfName(subsetPrefix + FontName));
        }

        dic.Put(PdfName.Encoding, new PdfName(encoding));
        dic.Put(PdfName.Descendantfonts, new PdfArray(descendant));
        if (toUnicode != null)
        {
            dic.Put(PdfName.Tounicode, toUnicode);
        }

        return dic;
    }

    /// <summary>
    ///     Creates a ToUnicode CMap to allow copy and paste from Acrobat.
    ///     contains the Unicode code
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="metrics">metrics[0] contains the glyph index and metrics[2]</param>
    /// <returns>the stream representing this CMap or  null </returns>
    private PdfStream getToUnicode(object[] metrics)
    {
        if (metrics.Length == 0)
        {
            return null;
        }

        var buf = new StringBuilder(
                                    "/CIDInit /ProcSet findresource begin\n" +
                                    "12 dict begin\n" +
                                    "begincmap\n" +
                                    "/CIDSystemInfo\n" +
                                    "<< /Registry (TTX+0)\n" +
                                    "/Ordering (T42UV)\n" +
                                    "/Supplement 0\n" +
                                    ">> def\n" +
                                    "/CMapName /TTX+0 def\n" +
                                    "/CMapType 2 def\n" +
                                    "1 begincodespacerange\n" +
                                    "<0000><FFFF>\n" +
                                    "endcodespacerange\n");
        var size = 0;
        for (var k = 0; k < metrics.Length; ++k)
        {
            if (size == 0)
            {
                if (k != 0)
                {
                    buf.Append("endbfrange\n");
                }

                size = Math.Min(100, metrics.Length - k);
                buf.Append(size).Append(" beginbfrange\n");
            }

            --size;
            var metric = (int[])metrics[k];
            var fromTo = ToHex(metric[0]);
            buf.Append(fromTo).Append(fromTo).Append(ToHex(metric[2])).Append('\n');
        }

        buf.Append(
                   "endbfrange\n" +
                   "endcmap\n" +
                   "CMapName currentdict /CMap defineresource pop\n" +
                   "end end\n");
        var s = buf.ToString();
        var stream = new PdfStream(PdfEncodings.ConvertToBytes(s, null));
        stream.FlateCompress(compressionLevel);
        return stream;
    }
}