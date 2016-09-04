using System.Collections;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Each font in the document will have an instance of this class
    /// where the characters used will be represented.
    /// @author  Paulo Soares (psoares@consiste.pt)
    /// </summary>
    internal class FontDetails
    {

        /// <summary>
        /// Indicates if all the glyphs and widths for that particular
        /// encoding should be included in the document.
        /// </summary>
        protected bool subset = true;

        /// <summary>
        /// The font
        /// </summary>
        readonly BaseFont _baseFont;

        readonly CjkFont _cjkFont;

        readonly IntHashtable _cjkTag;

        /// <summary>
        /// The font name that appears in the document body stream
        /// </summary>
        readonly PdfName _fontName;

        /// <summary>
        /// The font type
        /// </summary>
        readonly int _fontType;

        /// <summary>
        /// The indirect reference to this font
        /// </summary>
        readonly PdfIndirectReference _indirectReference;
        /// <summary>
        /// The map used with double byte encodings. The key is Int(glyph) and the
        /// value is int[]{glyph, width, Unicode code}
        /// </summary>
        readonly Hashtable _longTag;

        /// <summary>
        /// The array used with single byte encodings
        /// </summary>
        readonly byte[] _shortTag;

        /// <summary>
        ///  true  if the font is symbolic
        /// </summary>
        readonly bool _symbolic;

        /// <summary>
        /// The font if its an instance of  TrueTypeFontUnicode
        /// </summary>
        readonly TrueTypeFontUnicode _ttu;
        /// <summary>
        /// Each font used in a document has an instance of this class.
        /// This class stores the characters used in the document and other
        /// specifics unique to the current working document.
        /// </summary>
        /// <param name="fontName">the font name</param>
        /// <param name="indirectReference">the indirect reference to the font</param>
        /// <param name="baseFont">the  BaseFont </param>
        internal FontDetails(PdfName fontName, PdfIndirectReference indirectReference, BaseFont baseFont)
        {
            _fontName = fontName;
            _indirectReference = indirectReference;
            _baseFont = baseFont;
            _fontType = baseFont.FontType;
            switch (_fontType)
            {
                case BaseFont.FONT_TYPE_T1:
                case BaseFont.FONT_TYPE_TT:
                    _shortTag = new byte[256];
                    break;
                case BaseFont.FONT_TYPE_CJK:
                    _cjkTag = new IntHashtable();
                    _cjkFont = (CjkFont)baseFont;
                    break;
                case BaseFont.FONT_TYPE_TTUNI:
                    _longTag = new Hashtable();
                    _ttu = (TrueTypeFontUnicode)baseFont;
                    _symbolic = baseFont.IsFontSpecific();
                    break;
            }
        }

        /// <summary>
        /// Indicates if all the glyphs and widths for that particular
        /// encoding should be included in the document. Set to  false
        /// to include all.
        /// </summary>
        public bool Subset
        {
            set
            {
                subset = value;
            }
            get
            {
                return subset;
            }
        }

        /// <summary>
        /// Gets the  BaseFont  of this font.
        /// </summary>
        /// <returns>the  BaseFont  of this font</returns>
        internal BaseFont BaseFont
        {
            get
            {
                return _baseFont;
            }
        }

        /// <summary>
        /// Gets the font name as it appears in the document body.
        /// </summary>
        /// <returns>the font name</returns>
        internal PdfName FontName
        {
            get
            {
                return _fontName;
            }
        }

        /// <summary>
        /// Gets the indirect reference to this font.
        /// </summary>
        /// <returns>the indirect reference to this font</returns>
        internal PdfIndirectReference IndirectReference
        {
            get
            {
                return _indirectReference;
            }
        }
        /// <summary>
        /// Converts the text into bytes to be placed in the document.
        /// The conversion is done according to the font and the encoding and the characters
        /// used are stored.
        /// </summary>
        /// <param name="text">the text to convert</param>
        /// <returns>the conversion</returns>
        internal byte[] ConvertToBytes(string text)
        {
            byte[] b = null;
            switch (_fontType)
            {
                case BaseFont.FONT_TYPE_T3:
                    return _baseFont.ConvertToBytes(text);
                case BaseFont.FONT_TYPE_T1:
                case BaseFont.FONT_TYPE_TT:
                    {
                        b = _baseFont.ConvertToBytes(text);
                        int len = b.Length;
                        for (int k = 0; k < len; ++k)
                            _shortTag[b[k] & 0xff] = 1;
                        break;
                    }
                case BaseFont.FONT_TYPE_CJK:
                    {
                        int len = text.Length;
                        for (int k = 0; k < len; ++k)
                            _cjkTag[_cjkFont.GetCidCode(text[k])] = 0;
                        b = _baseFont.ConvertToBytes(text);
                        break;
                    }
                case BaseFont.FONT_TYPE_DOCUMENT:
                    {
                        b = _baseFont.ConvertToBytes(text);
                        break;
                    }
                case BaseFont.FONT_TYPE_TTUNI:
                    {
                        int len = text.Length;
                        int[] metrics = null;
                        char[] glyph = new char[len];
                        int i = 0;
                        if (_symbolic)
                        {
                            b = PdfEncodings.ConvertToBytes(text, "symboltt");
                            len = b.Length;
                            for (int k = 0; k < len; ++k)
                            {
                                metrics = _ttu.GetMetricsTt(b[k] & 0xff);
                                if (metrics == null)
                                    continue;
                                _longTag[metrics[0]] = new[] { metrics[0], metrics[1], _ttu.GetUnicodeDifferences(b[k] & 0xff) };
                                glyph[i++] = (char)metrics[0];
                            }
                        }
                        else
                        {
                            for (int k = 0; k < len; ++k)
                            {
                                int val;
                                if (Utilities.IsSurrogatePair(text, k))
                                {
                                    val = Utilities.ConvertToUtf32(text, k);
                                    k++;
                                }
                                else
                                {
                                    val = text[k];
                                }
                                metrics = _ttu.GetMetricsTt(val);
                                if (metrics == null)
                                    continue;
                                int m0 = metrics[0];
                                int gl = m0;
                                if (!_longTag.ContainsKey(gl))
                                    _longTag[gl] = new[] { m0, metrics[1], val };
                                glyph[i++] = (char)m0;
                            }
                        }
                        string s = new string(glyph, 0, i);
                        b = PdfEncodings.ConvertToBytes(s, CjkFont.CJK_ENCODING);
                        break;
                    }
            }
            return b;
        }

        /// <summary>
        /// Writes the font definition to the document.
        /// </summary>
        /// <param name="writer">the  PdfWriter  of this document</param>
        internal void WriteFont(PdfWriter writer)
        {
            switch (_fontType)
            {
                case BaseFont.FONT_TYPE_T3:
                    _baseFont.WriteFont(writer, _indirectReference, null);
                    break;
                case BaseFont.FONT_TYPE_T1:
                case BaseFont.FONT_TYPE_TT:
                    {
                        int firstChar;
                        int lastChar;
                        for (firstChar = 0; firstChar < 256; ++firstChar)
                        {
                            if (_shortTag[firstChar] != 0)
                                break;
                        }
                        for (lastChar = 255; lastChar >= firstChar; --lastChar)
                        {
                            if (_shortTag[lastChar] != 0)
                                break;
                        }
                        if (firstChar > 255)
                        {
                            firstChar = 255;
                            lastChar = 255;
                        }
                        _baseFont.WriteFont(writer, _indirectReference, new object[] { firstChar, lastChar, _shortTag, subset });
                        break;
                    }
                case BaseFont.FONT_TYPE_CJK:
                    _baseFont.WriteFont(writer, _indirectReference, new object[] { _cjkTag });
                    break;
                case BaseFont.FONT_TYPE_TTUNI:
                    _baseFont.WriteFont(writer, _indirectReference, new object[] { _longTag, subset });
                    break;
            }
        }
    }
}