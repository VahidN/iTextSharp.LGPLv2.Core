using System.Text;
using System.util;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf;

/// <summary>
///     Supports fast encodings for winansi and PDFDocEncoding.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public static class PdfEncodings
{
    private const int CIDCHAR = 2;
    private const int CIDNONE = 0;
    private const int CIDRANGE = 1;

    /// <summary>
    ///     Assumes that '\\n' and '\\r\\n' are the newline sequences. It may not work for
    ///     all CJK encodings. To be used with LoadCmap().
    /// </summary>
    public static byte[][] CrlfCidNewline =
    {
        new[]
        {
            (byte)'\n'
        },
        new[]
        {
            (byte)'\r', (byte)'\n'
        }
    };

    internal static readonly INullValueDictionary<string, char[][]> Cmaps = new NullValueDictionary<string, char[][]>();

    internal static INullValueDictionary<string, IExtraEncoding> ExtraEncodings =
        new NullValueDictionary<string, IExtraEncoding>();

    internal static readonly NullValueDictionary<int, int> PdfEncoding = new();

    internal static readonly char[] PdfEncodingByteToChar =
    {
        (char)0, (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10, (char)11,
        (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20, (char)21, (char)22,
        (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30, (char)31, (char)32, (char)33,
        (char)34, (char)35, (char)36, (char)37, (char)38, (char)39, (char)40, (char)41, (char)42, (char)43, (char)44,
        (char)45, (char)46, (char)47, (char)48, (char)49, (char)50, (char)51, (char)52, (char)53, (char)54, (char)55,
        (char)56, (char)57, (char)58, (char)59, (char)60, (char)61, (char)62, (char)63, (char)64, (char)65, (char)66,
        (char)67, (char)68, (char)69, (char)70, (char)71, (char)72, (char)73, (char)74, (char)75, (char)76, (char)77,
        (char)78, (char)79, (char)80, (char)81, (char)82, (char)83, (char)84, (char)85, (char)86, (char)87, (char)88,
        (char)89, (char)90, (char)91, (char)92, (char)93, (char)94, (char)95, (char)96, (char)97, (char)98, (char)99,
        (char)100, (char)101, (char)102, (char)103, (char)104, (char)105, (char)106, (char)107, (char)108, (char)109,
        (char)110, (char)111, (char)112, (char)113, (char)114, (char)115, (char)116, (char)117, (char)118, (char)119,
        (char)120, (char)121, (char)122, (char)123, (char)124, (char)125, (char)126, (char)127, (char)0x2022,
        (char)0x2020, (char)0x2021, (char)0x2026, (char)0x2014, (char)0x2013, (char)0x0192, (char)0x2044, (char)0x2039,
        (char)0x203a, (char)0x2212, (char)0x2030, (char)0x201e, (char)0x201c, (char)0x201d, (char)0x2018, (char)0x2019,
        (char)0x201a, (char)0x2122, (char)0xfb01, (char)0xfb02, (char)0x0141, (char)0x0152, (char)0x0160, (char)0x0178,
        (char)0x017d, (char)0x0131, (char)0x0142, (char)0x0153, (char)0x0161, (char)0x017e, (char)65533, (char)0x20ac,
        (char)161, (char)162, (char)163, (char)164, (char)165, (char)166, (char)167, (char)168, (char)169, (char)170,
        (char)171, (char)172, (char)173, (char)174, (char)175, (char)176, (char)177, (char)178, (char)179, (char)180,
        (char)181, (char)182, (char)183, (char)184, (char)185, (char)186, (char)187, (char)188, (char)189, (char)190,
        (char)191, (char)192, (char)193, (char)194, (char)195, (char)196, (char)197, (char)198, (char)199, (char)200,
        (char)201, (char)202, (char)203, (char)204, (char)205, (char)206, (char)207, (char)208, (char)209, (char)210,
        (char)211, (char)212, (char)213, (char)214, (char)215, (char)216, (char)217, (char)218, (char)219, (char)220,
        (char)221, (char)222, (char)223, (char)224, (char)225, (char)226, (char)227, (char)228, (char)229, (char)230,
        (char)231, (char)232, (char)233, (char)234, (char)235, (char)236, (char)237, (char)238, (char)239, (char)240,
        (char)241, (char)242, (char)243, (char)244, (char)245, (char)246, (char)247, (char)248, (char)249, (char)250,
        (char)251, (char)252, (char)253, (char)254, (char)255
    };

    internal static readonly NullValueDictionary<int, int> Winansi = new();

    internal static readonly char[] WinansiByteToChar =
    {
        (char)0, (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10, (char)11,
        (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20, (char)21, (char)22,
        (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30, (char)31, (char)32, (char)33,
        (char)34, (char)35, (char)36, (char)37, (char)38, (char)39, (char)40, (char)41, (char)42, (char)43, (char)44,
        (char)45, (char)46, (char)47, (char)48, (char)49, (char)50, (char)51, (char)52, (char)53, (char)54, (char)55,
        (char)56, (char)57, (char)58, (char)59, (char)60, (char)61, (char)62, (char)63, (char)64, (char)65, (char)66,
        (char)67, (char)68, (char)69, (char)70, (char)71, (char)72, (char)73, (char)74, (char)75, (char)76, (char)77,
        (char)78, (char)79, (char)80, (char)81, (char)82, (char)83, (char)84, (char)85, (char)86, (char)87, (char)88,
        (char)89, (char)90, (char)91, (char)92, (char)93, (char)94, (char)95, (char)96, (char)97, (char)98, (char)99,
        (char)100, (char)101, (char)102, (char)103, (char)104, (char)105, (char)106, (char)107, (char)108, (char)109,
        (char)110, (char)111, (char)112, (char)113, (char)114, (char)115, (char)116, (char)117, (char)118, (char)119,
        (char)120, (char)121, (char)122, (char)123, (char)124, (char)125, (char)126, (char)127, (char)8364, (char)65533,
        (char)8218, (char)402, (char)8222, (char)8230, (char)8224, (char)8225, (char)710, (char)8240, (char)352,
        (char)8249, (char)338, (char)65533, (char)381, (char)65533, (char)65533, (char)8216, (char)8217, (char)8220,
        (char)8221, (char)8226, (char)8211, (char)8212, (char)732, (char)8482, (char)353, (char)8250, (char)339,
        (char)65533, (char)382, (char)376, (char)160, (char)161, (char)162, (char)163, (char)164, (char)165, (char)166,
        (char)167, (char)168, (char)169, (char)170, (char)171, (char)172, (char)173, (char)174, (char)175, (char)176,
        (char)177, (char)178, (char)179, (char)180, (char)181, (char)182, (char)183, (char)184, (char)185, (char)186,
        (char)187, (char)188, (char)189, (char)190, (char)191, (char)192, (char)193, (char)194, (char)195, (char)196,
        (char)197, (char)198, (char)199, (char)200, (char)201, (char)202, (char)203, (char)204, (char)205, (char)206,
        (char)207, (char)208, (char)209, (char)210, (char)211, (char)212, (char)213, (char)214, (char)215, (char)216,
        (char)217, (char)218, (char)219, (char)220, (char)221, (char)222, (char)223, (char)224, (char)225, (char)226,
        (char)227, (char)228, (char)229, (char)230, (char)231, (char)232, (char)233, (char)234, (char)235, (char)236,
        (char)237, (char)238, (char)239, (char)240, (char)241, (char)242, (char)243, (char)244, (char)245, (char)246,
        (char)247, (char)248, (char)249, (char)250, (char)251, (char)252, (char)253, (char)254, (char)255
    };

    static PdfEncodings()
    {
        for (var k = 128; k < 161; ++k)
        {
            var c = WinansiByteToChar[k];

            if (c != 65533)
            {
                Winansi[c] = k;
            }
        }

        for (var k = 128; k < 161; ++k)
        {
            var c = PdfEncodingByteToChar[k];

            if (c != 65533)
            {
                PdfEncoding[c] = k;
            }
        }

        AddExtraEncoding("Wingdings", new WingdingsConversion());
        AddExtraEncoding("Symbol", new SymbolConversion(true));
        AddExtraEncoding("ZapfDingbats", new SymbolConversion(false));
        AddExtraEncoding("SymbolTT", new SymbolTtConversion());
        AddExtraEncoding("Cp437", new Cp437Conversion());
    }

    /// <summary>
    ///     Adds an extra encoding.
    /// </summary>
    /// <param name="name">the name of the encoding. The encoding recognition is case insensitive</param>
    /// <param name="enc">the conversion class</param>
    public static void AddExtraEncoding(string name, IExtraEncoding enc)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        // This serializes concurrent updates
        var newEncodings = ExtraEncodings.Clone();
        newEncodings[name.ToLower(CultureInfo.InvariantCulture)] = enc;
        ExtraEncodings = newEncodings; // This swap does not require synchronization with reader
    }

    /// <summary>
    ///     Clears the CJK cmaps from the cache. If  name  is the
    ///     empty string then all the cache is cleared. Calling this method
    ///     has no consequences other than the need to reload the cmap
    ///     if needed.
    /// </summary>
    /// <param name="name">the name of the cmap to clear or all the cmaps if the empty string</param>
    public static void ClearCmap(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (name.Length == 0)
        {
            Cmaps.Clear();
        }
        else
        {
            Cmaps.Remove(name);
        }
    }

    /// <summary>
    ///     Converts a  byte  array encoded as  name
    ///     to a CID string. This is needed to reach some CJK characters
    ///     that don't exist in 16 bit Unicode.
    ///     The font to use this result must use the encoding "Identity-H"
    ///     or "Identity-V".
    ///     See ftp://ftp.oreilly.com/pub/examples/nutshell/cjkv/adobe/.
    /// </summary>
    /// <param name="name">the CJK encoding name</param>
    /// <param name="seq">the  byte  array to be decoded</param>
    /// <returns>the CID string</returns>
    public static string ConvertCmap(string name, byte[] seq)
    {
        if (seq == null)
        {
            throw new ArgumentNullException(nameof(seq));
        }

        return ConvertCmap(name, seq, 0, seq.Length);
    }

    /// <summary>
    ///     Converts a  byte  array encoded as  name
    ///     to a CID string. This is needed to reach some CJK characters
    ///     that don't exist in 16 bit Unicode.
    ///     The font to use this result must use the encoding "Identity-H"
    ///     or "Identity-V".
    ///     See ftp://ftp.oreilly.com/pub/examples/nutshell/cjkv/adobe/.
    /// </summary>
    /// <param name="name">the CJK encoding name</param>
    /// <param name="start">the start offset in the data</param>
    /// <param name="length">the number of bytes to convert</param>
    /// <param name="seq">the  byte  array to be decoded</param>
    /// <returns>the CID string</returns>
    public static string ConvertCmap(string name, byte[] seq, int start, int length)
    {
        if (seq == null)
        {
            throw new ArgumentNullException(nameof(seq));
        }

        char[][] planes = null;
        planes = Cmaps[name];

        if (planes == null)
        {
            planes = ReadCmap(name, (byte[][])null);
            Cmaps[name] = planes;
        }

        return DecodeSequence(seq, start, length, planes);
    }

    /// <summary>
    ///     Converts a  string  to a  byte  array according
    ///     to the font's encoding.
    /// </summary>
    /// <param name="text">the  string  to be converted</param>
    /// <param name="encoding"></param>
    /// <returns>an array of  byte  representing the conversion according to the font's encoding</returns>
    public static byte[] ConvertToBytes(string text, string encoding)
    {
        if (text == null)
        {
            return Array.Empty<byte>();
        }

        if (string.IsNullOrEmpty(encoding))
        {
            var len = text.Length;
            var b = new byte[len];

            for (var k = 0; k < len; ++k)
            {
                b[k] = (byte)text[k];
            }

            return b;
        }

        var extra = ExtraEncodings[encoding.ToLower(CultureInfo.InvariantCulture)];

        if (extra != null)
        {
            var b = extra.CharToByte(text, encoding);

            if (b != null)
            {
                return b;
            }
        }

        NullValueDictionary<int, int> hash = null;

        if (encoding.Equals(BaseFont.CP1252, StringComparison.Ordinal))
        {
            hash = Winansi;
        }
        else if (encoding.Equals(PdfObject.TEXT_PDFDOCENCODING, StringComparison.Ordinal))
        {
            hash = PdfEncoding;
        }

        if (hash != null)
        {
            var cc = text.ToCharArray();
            var len = cc.Length;
            var ptr = 0;
            var b = new byte[len];
            var c = 0;

            for (var k = 0; k < len; ++k)
            {
                var char1 = cc[k];

                if (char1 < 128 || (char1 > 160 && char1 <= 255))
                {
                    c = char1;
                }
                else
                {
                    c = hash[char1];
                }

                if (c != 0)
                {
                    b[ptr++] = (byte)c;
                }
            }

            if (ptr == len)
            {
                return b;
            }

            var b2 = new byte[ptr];
            Array.Copy(b, 0, b2, 0, ptr);

            return b2;
        }

        var encw = IanaEncodings.GetEncodingEncoding(encoding);
        var preamble = encw.GetPreamble();

        if (preamble.Length == 0)
        {
            return encw.GetBytes(text);
        }

        var encoded = encw.GetBytes(text);
        var total = new byte[encoded.Length + preamble.Length];
        Array.Copy(preamble, 0, total, 0, preamble.Length);
        Array.Copy(encoded, 0, total, preamble.Length, encoded.Length);

        return total;
    }

    /// <summary>
    ///     Converts a  String  to a  byte  array according
    ///     to the font's encoding.
    /// </summary>
    /// <param name="encoding">the encoding</param>
    /// <param name="char1">the  char  to be converted</param>
    /// <returns>an array of  byte  representing the conversion according to the font's encoding</returns>
    public static byte[] ConvertToBytes(char char1, string encoding)
    {
        if (string.IsNullOrEmpty(encoding))
        {
            return new[]
            {
                (byte)char1
            };
        }

        var extra = ExtraEncodings[encoding.ToLower(CultureInfo.InvariantCulture)];

        if (extra != null)
        {
            var b = extra.CharToByte(char1, encoding);

            if (b != null)
            {
                return b;
            }
        }

        NullValueDictionary<int, int> hash = null;

        if (encoding.Equals(BaseFont.WINANSI, StringComparison.Ordinal))
        {
            hash = Winansi;
        }
        else if (encoding.Equals(PdfObject.TEXT_PDFDOCENCODING, StringComparison.Ordinal))
        {
            hash = PdfEncoding;
        }

        if (hash != null)
        {
            var c = 0;

            if (char1 < 128 || (char1 > 160 && char1 <= 255))
            {
                c = char1;
            }
            else
            {
                c = hash[char1];
            }

            if (c != 0)
            {
                return new[]
                {
                    (byte)c
                };
            }

            return Array.Empty<byte>();
        }

        var encw = IanaEncodings.GetEncodingEncoding(encoding);
        var preamble = encw.GetPreamble();

        char[] text =
        {
            char1
        };

        if (preamble.Length == 0)
        {
            return encw.GetBytes(text);
        }

        var encoded = encw.GetBytes(text);
        var total = new byte[encoded.Length + preamble.Length];
        Array.Copy(preamble, 0, total, 0, preamble.Length);
        Array.Copy(encoded, 0, total, preamble.Length, encoded.Length);

        return total;
    }

    public static string ConvertToString(byte[] bytes, string encoding)
    {
        if (bytes == null)
        {
            return PdfObject.NOTHING;
        }

        if (string.IsNullOrEmpty(encoding))
        {
            var c = new char[bytes.Length];

            for (var k = 0; k < bytes.Length; ++k)
            {
                c[k] = (char)(bytes[k] & 0xff);
            }

            return new string(c);
        }

        var extra = ExtraEncodings[encoding.ToLower(CultureInfo.InvariantCulture)];

        if (extra != null)
        {
            var text = extra.ByteToChar(bytes, encoding);

            if (text != null)
            {
                return text;
            }
        }

        char[] ch = null;

        if (encoding.Equals(BaseFont.WINANSI, StringComparison.Ordinal))
        {
            ch = WinansiByteToChar;
        }
        else if (encoding.Equals(PdfObject.TEXT_PDFDOCENCODING, StringComparison.Ordinal))
        {
            ch = PdfEncodingByteToChar;
        }

        if (ch != null)
        {
            var len = bytes.Length;
            var c = new char[len];

            for (var k = 0; k < len; ++k)
            {
                c[k] = ch[bytes[k] & 0xff];
            }

            return new string(c);
        }

        var nameU = encoding.ToUpper(CultureInfo.InvariantCulture);
        var marker = false;
        var big = false;
        var offset = 0;

        if (bytes.Length >= 2)
        {
            if (bytes[0] == 254 && bytes[1] == 255)
            {
                marker = true;
                big = true;
                offset = 2;
            }
            else if (bytes[0] == 255 && bytes[1] == 254)
            {
                marker = true;
                big = false;
                offset = 2;
            }
        }

        UnicodeEncoding enc = null;

        if (nameU.Equals("UNICODEBIGUNMARKED", StringComparison.Ordinal) ||
            nameU.Equals("UNICODEBIG", StringComparison.Ordinal))
        {
            enc = new UnicodeEncoding(marker ? big : true, false);
        }

        if (nameU.Equals("UNICODELITTLEUNMARKED", StringComparison.Ordinal) ||
            nameU.Equals("UNICODELITTLE", StringComparison.Ordinal))
        {
            enc = new UnicodeEncoding(marker ? big : false, false);
        }

        if (enc != null)
        {
            return enc.GetString(bytes, offset, bytes.Length - offset);
        }

        return IanaEncodings.GetEncodingEncoding(encoding).GetString(bytes);
    }

    /// <summary>
    ///     Checks is  text  only has PdfDocEncoding characters.
    /// </summary>
    /// <param name="text">the  String  to test</param>
    /// <returns> true  if only PdfDocEncoding characters are present</returns>
    public static bool IsPdfDocEncoding(string text)
    {
        if (text == null)
        {
            return true;
        }

        var len = text.Length;

        for (var k = 0; k < len; ++k)
        {
            var char1 = text[k];

            if (char1 < 128 || (char1 > 160 && char1 <= 255))
            {
                continue;
            }

            if (!PdfEncoding.ContainsKey(char1))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///     Loads a CJK cmap to the cache with the option of associating
    ///     sequences to the newline.
    /// </summary>
    /// <param name="name">the CJK cmap name</param>
    /// <param name="newline">the sequences to be replaced bi a newline in the resulting CID. See  CRLF_CID_NEWLINE </param>
    public static void LoadCmap(string name, byte[][] newline)
    {
        char[][] planes = null;
        planes = Cmaps[name];

        if (planes == null)
        {
            planes = ReadCmap(name, newline);
            Cmaps[name] = planes;
        }
    }

    internal static void BreakLong(long n, int size, byte[] seqs)
    {
        for (var k = 0; k < size; ++k)
        {
            seqs[k] = (byte)(n >> ((size - 1 - k) * 8));
        }
    }

    internal static string DecodeSequence(byte[] seq, int start, int length, char[][] planes)
    {
        var buf = new StringBuilder();
        var end = start + length;
        var currentPlane = 0;

        for (var k = start; k < end; ++k)
        {
            var one = seq[k] & 0xff;
            var plane = planes[currentPlane];
            int cid = plane[one];

            if ((cid & 0x8000) == 0)
            {
                buf.Append((char)cid);
                currentPlane = 0;
            }
            else
            {
                currentPlane = cid & 0x7fff;
            }
        }

        return buf.ToString();
    }

    internal static void EncodeSequence(int size, byte[] seqs, char cid, IList<char[]> planes)
    {
        --size;
        var nextPlane = 0;
        char[] plane;

        for (var idx = 0; idx < size; ++idx)
        {
            plane = planes[nextPlane];
            var one = seqs[idx] & 0xff;
            var c = plane[one];

            if (c != 0 && (c & 0x8000) == 0)
            {
                throw new InvalidOperationException("Inconsistent mapping.");
            }

            if (c == 0)
            {
                planes.Add(new char[256]);
                c = (char)((planes.Count - 1) | 0x8000);
                plane[one] = c;
            }

            nextPlane = c & 0x7fff;
        }

        plane = planes[nextPlane];
        var ones = seqs[size] & 0xff;
        var cc = plane[ones];

        if ((cc & 0x8000) != 0)
        {
            throw new InvalidOperationException("Inconsistent mapping.");
        }

        plane[ones] = cid;
    }

    internal static void EncodeStream(Stream inp, IList<char[]> planes)
    {
        using var rd = new StreamReader(inp, Encoding.ASCII);
        string line = null;
        var state = CIDNONE;
        var seqs = new byte[7];

        while ((line = rd.ReadLine()) != null)
        {
            if (line.Length < 6)
            {
                continue;
            }

            switch (state)
            {
                case CIDNONE:
                {
                    if (line.IndexOf("begincidrange", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        state = CIDRANGE;
                    }
                    else if (line.IndexOf("begincidchar", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        state = CIDCHAR;
                    }
                    else if (line.IndexOf("usecmap", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var tk = new StringTokenizer(line);
                        var t = tk.NextToken();
                        ReadCmap(t.Substring(1), planes);
                    }

                    break;
                }
                case CIDRANGE:
                {
                    if (line.IndexOf("endcidrange", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        state = CIDNONE;

                        break;
                    }

                    var tk = new StringTokenizer(line);
                    var t = tk.NextToken();
                    var size = t.Length / 2 - 1;

                    var start = long.Parse(t.Substring(1, t.Length - 2), NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture);

                    t = tk.NextToken();

                    var end = long.Parse(t.Substring(1, t.Length - 2), NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture);

                    t = tk.NextToken();
                    var cid = int.Parse(t, CultureInfo.InvariantCulture);

                    for (var k = start; k <= end; ++k)
                    {
                        BreakLong(k, size, seqs);
                        EncodeSequence(size, seqs, (char)cid, planes);
                        ++cid;
                    }

                    break;
                }
                case CIDCHAR:
                {
                    if (line.IndexOf("endcidchar", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        state = CIDNONE;

                        break;
                    }

                    var tk = new StringTokenizer(line);
                    var t = tk.NextToken();
                    var size = t.Length / 2 - 1;

                    var start = long.Parse(t.Substring(1, t.Length - 2), NumberStyles.HexNumber,
                        CultureInfo.InvariantCulture);

                    t = tk.NextToken();
                    var cid = int.Parse(t, CultureInfo.InvariantCulture);
                    BreakLong(start, size, seqs);
                    EncodeSequence(size, seqs, (char)cid, planes);

                    break;
                }
            }
        }
    }

    internal static char[][] ReadCmap(string name, byte[][] newline)
    {
        List<char[]> planes = new();
        planes.Add(new char[256]);
        ReadCmap(name, planes);

        if (newline != null)
        {
            for (var k = 0; k < newline.Length; ++k)
            {
                EncodeSequence(newline[k].Length, newline[k], BaseFont.CID_NEWLINE, planes);
            }
        }

        var ret = new char[planes.Count][];
        planes.CopyTo(ret, 0);

        return ret;
    }

    internal static void ReadCmap(string name, IList<char[]> planes)
    {
        var fullName = BaseFont.RESOURCE_PATH + "cmaps." + name;
        using var inp = BaseFont.GetResourceStream(fullName);

        if (inp == null)
        {
            throw new IOException("The Cmap " + name + " was not found.");
        }

        EncodeStream(inp, planes);
    }

    private class Cp437Conversion : IExtraEncoding
    {
        private static readonly NullValueDictionary<int, int> _c2B = new();

        private static readonly char[] _table =
        {
            '\u00C7', '\u00FC', '\u00E9', '\u00E2', '\u00E4', '\u00E0', '\u00E5', '\u00E7', '\u00EA', '\u00EB',
            '\u00E8', '\u00EF', '\u00EE', '\u00EC', '\u00C4', '\u00C5', '\u00C9', '\u00E6', '\u00C6', '\u00F4',
            '\u00F6', '\u00F2', '\u00FB', '\u00F9', '\u00FF', '\u00D6', '\u00DC', '\u00A2', '\u00A3', '\u00A5',
            '\u20A7', '\u0192', '\u00E1', '\u00ED', '\u00F3', '\u00FA', '\u00F1', '\u00D1', '\u00AA', '\u00BA',
            '\u00BF', '\u2310', '\u00AC', '\u00BD', '\u00BC', '\u00A1', '\u00AB', '\u00BB', '\u2591', '\u2592',
            '\u2593', '\u2502', '\u2524', '\u2561', '\u2562', '\u2556', '\u2555', '\u2563', '\u2551', '\u2557',
            '\u255D', '\u255C', '\u255B', '\u2510', '\u2514', '\u2534', '\u252C', '\u251C', '\u2500', '\u253C',
            '\u255E', '\u255F', '\u255A', '\u2554', '\u2569', '\u2566', '\u2560', '\u2550', '\u256C', '\u2567',
            '\u2568', '\u2564', '\u2565', '\u2559', '\u2558', '\u2552', '\u2553', '\u256B', '\u256A', '\u2518',
            '\u250C', '\u2588', '\u2584', '\u258C', '\u2590', '\u2580', '\u03B1', '\u00DF', '\u0393', '\u03C0',
            '\u03A3', '\u03C3', '\u00B5', '\u03C4', '\u03A6', '\u0398', '\u03A9', '\u03B4', '\u221E', '\u03C6',
            '\u03B5', '\u2229', '\u2261', '\u00B1', '\u2265', '\u2264', '\u2320', '\u2321', '\u00F7', '\u2248',
            '\u00B0', '\u2219', '\u00B7', '\u221A', '\u207F', '\u00B2', '\u25A0', '\u00A0'
        };

        static Cp437Conversion()
        {
            for (var k = 0; k < _table.Length; ++k)
            {
                _c2B[_table[k]] = k + 128;
            }
        }

        public string ByteToChar(byte[] b, string encoding)
        {
            var len = b.Length;
            var cc = new char[len];
            var ptr = 0;

            for (var k = 0; k < len; ++k)
            {
                var c = b[k] & 0xff;

                if (c < ' ')
                {
                    continue;
                }

                if (c < 128)
                {
                    cc[ptr++] = (char)c;
                }
                else
                {
                    var v = _table[c - 128];
                    cc[ptr++] = v;
                }
            }

            return new string(cc, 0, ptr);
        }

        public byte[] CharToByte(string text, string encoding)
        {
            var cc = text.ToCharArray();
            var b = new byte[cc.Length];
            var ptr = 0;
            var len = cc.Length;

            for (var k = 0; k < len; ++k)
            {
                var c = cc[k];

                if (c < 128)
                {
                    b[ptr++] = (byte)c;
                }
                else
                {
                    var v = (byte)_c2B[c];

                    if (v != 0)
                    {
                        b[ptr++] = v;
                    }
                }
            }

            if (ptr == len)
            {
                return b;
            }

            var b2 = new byte[ptr];
            Array.Copy(b, 0, b2, 0, ptr);

            return b2;
        }

        public byte[] CharToByte(char char1, string encoding)
        {
            if (char1 < 128)
            {
                return new[]
                {
                    (byte)char1
                };
            }

            var v = (byte)_c2B[char1];

            if (v != 0)
            {
                return new[]
                {
                    v
                };
            }

            return Array.Empty<byte>();
        }
    }

    private class SymbolConversion : IExtraEncoding
    {
        private static readonly NullValueDictionary<int, int> _t1 = new();
        private static readonly NullValueDictionary<int, int> _t2 = new();

        private static readonly char[] _table1 =
        {
            ' ', '!', '\u2200', '#', '\u2203', '%', '&', '\u220b', '(', ')', '*', '+', ',', '-', '.', '/', '0', '1',
            '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', '\u2245', '\u0391', '\u0392',
            '\u03a7', '\u0394', '\u0395', '\u03a6', '\u0393', '\u0397', '\u0399', '\u03d1', '\u039a', '\u039b',
            '\u039c', '\u039d', '\u039f', '\u03a0', '\u0398', '\u03a1', '\u03a3', '\u03a4', '\u03a5', '\u03c2',
            '\u03a9', '\u039e', '\u03a8', '\u0396', '[', '\u2234', ']', '\u22a5', '_', '\u0305', '\u03b1', '\u03b2',
            '\u03c7', '\u03b4', '\u03b5', '\u03d5', '\u03b3', '\u03b7', '\u03b9', '\u03c6', '\u03ba', '\u03bb',
            '\u03bc', '\u03bd', '\u03bf', '\u03c0', '\u03b8', '\u03c1', '\u03c3', '\u03c4', '\u03c5', '\u03d6',
            '\u03c9', '\u03be', '\u03c8', '\u03b6', '{', '|', '}', '~', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\u20ac', '\u03d2', '\u2032', '\u2264', '\u2044', '\u221e',
            '\u0192', '\u2663', '\u2666', '\u2665', '\u2660', '\u2194', '\u2190', '\u2191', '\u2192', '\u2193',
            '\u00b0', '\u00b1', '\u2033', '\u2265', '\u00d7', '\u221d', '\u2202', '\u2022', '\u00f7', '\u2260',
            '\u2261', '\u2248', '\u2026', '\u2502', '\u2500', '\u21b5', '\u2135', '\u2111', '\u211c', '\u2118',
            '\u2297', '\u2295', '\u2205', '\u2229', '\u222a', '\u2283', '\u2287', '\u2284', '\u2282', '\u2286',
            '\u2208', '\u2209', '\u2220', '\u2207', '\u00ae', '\u00a9', '\u2122', '\u220f', '\u221a', '\u2022',
            '\u00ac', '\u2227', '\u2228', '\u21d4', '\u21d0', '\u21d1', '\u21d2', '\u21d3', '\u25ca', '\u2329', '\0',
            '\0', '\0', '\u2211', '\u239b', '\u239c', '\u239d', '\u23a1', '\u23a2', '\u23a3', '\u23a7', '\u23a8',
            '\u23a9', '\u23aa', '\0', '\u232a', '\u222b', '\u2320', '\u23ae', '\u2321', '\u239e', '\u239f', '\u23a0',
            '\u23a4', '\u23a5', '\u23a6', '\u23ab', '\u23ac', '\u23ad', '\0'
        };

        private static readonly char[] _table2 =
        {
            '\u0020', '\u2701', '\u2702', '\u2703', '\u2704', '\u260e', '\u2706', '\u2707', '\u2708', '\u2709',
            '\u261b', '\u261e', '\u270C', '\u270D', '\u270E', '\u270F', '\u2710', '\u2711', '\u2712', '\u2713',
            '\u2714', '\u2715', '\u2716', '\u2717', '\u2718', '\u2719', '\u271A', '\u271B', '\u271C', '\u271D',
            '\u271E', '\u271F', '\u2720', '\u2721', '\u2722', '\u2723', '\u2724', '\u2725', '\u2726', '\u2727',
            '\u2605', '\u2729', '\u272A', '\u272B', '\u272C', '\u272D', '\u272E', '\u272F', '\u2730', '\u2731',
            '\u2732', '\u2733', '\u2734', '\u2735', '\u2736', '\u2737', '\u2738', '\u2739', '\u273A', '\u273B',
            '\u273C', '\u273D', '\u273E', '\u273F', '\u2740', '\u2741', '\u2742', '\u2743', '\u2744', '\u2745',
            '\u2746', '\u2747', '\u2748', '\u2749', '\u274A', '\u274B', '\u25cf', '\u274D', '\u25a0', '\u274F',
            '\u2750', '\u2751', '\u2752', '\u25b2', '\u25bc', '\u25c6', '\u2756', '\u25d7', '\u2758', '\u2759',
            '\u275A', '\u275B', '\u275C', '\u275D', '\u275E', '\u0000', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0',
            '\0', '\0', '\0', '\0', '\0', '\0', '\u0000', '\u2761', '\u2762', '\u2763', '\u2764', '\u2765', '\u2766',
            '\u2767', '\u2663', '\u2666', '\u2665', '\u2660', '\u2460', '\u2461', '\u2462', '\u2463', '\u2464',
            '\u2465', '\u2466', '\u2467', '\u2468', '\u2469', '\u2776', '\u2777', '\u2778', '\u2779', '\u277A',
            '\u277B', '\u277C', '\u277D', '\u277E', '\u277F', '\u2780', '\u2781', '\u2782', '\u2783', '\u2784',
            '\u2785', '\u2786', '\u2787', '\u2788', '\u2789', '\u278A', '\u278B', '\u278C', '\u278D', '\u278E',
            '\u278F', '\u2790', '\u2791', '\u2792', '\u2793', '\u2794', '\u2192', '\u2194', '\u2195', '\u2798',
            '\u2799', '\u279A', '\u279B', '\u279C', '\u279D', '\u279E', '\u279F', '\u27A0', '\u27A1', '\u27A2',
            '\u27A3', '\u27A4', '\u27A5', '\u27A6', '\u27A7', '\u27A8', '\u27A9', '\u27AA', '\u27AB', '\u27AC',
            '\u27AD', '\u27AE', '\u27AF', '\u0000', '\u27B1', '\u27B2', '\u27B3', '\u27B4', '\u27B5', '\u27B6',
            '\u27B7', '\u27B8', '\u27B9', '\u27BA', '\u27BB', '\u27BC', '\u27BD', '\u27BE', '\u0000'
        };

        private readonly NullValueDictionary<int, int> _translation;

        static SymbolConversion()
        {
            for (var k = 0; k < _table1.Length; ++k)
            {
                int v = _table1[k];

                if (v != 0)
                {
                    _t1[v] = k + 32;
                }
            }

            for (var k = 0; k < _table2.Length; ++k)
            {
                int v = _table2[k];

                if (v != 0)
                {
                    _t2[v] = k + 32;
                }
            }
        }

        internal SymbolConversion(bool symbol)
        {
            if (symbol)
            {
                _translation = _t1;
            }
            else
            {
                _translation = _t2;
            }
        }

        public string ByteToChar(byte[] b, string encoding)
            => null;

        public byte[] CharToByte(string text, string encoding)
        {
            var cc = text.ToCharArray();
            var b = new byte[cc.Length];
            var ptr = 0;
            var len = cc.Length;

            for (var k = 0; k < len; ++k)
            {
                var c = cc[k];
                var v = (byte)_translation[c];

                if (v != 0)
                {
                    b[ptr++] = v;
                }
            }

            if (ptr == len)
            {
                return b;
            }

            var b2 = new byte[ptr];
            Array.Copy(b, 0, b2, 0, ptr);

            return b2;
        }

        public byte[] CharToByte(char char1, string encoding)
        {
            var v = (byte)_translation[char1];

            if (v != 0)
            {
                return new[]
                {
                    v
                };
            }

            return Array.Empty<byte>();
        }
    }

    private class SymbolTtConversion : IExtraEncoding
    {
        public string ByteToChar(byte[] b, string encoding)
            => null;

        public byte[] CharToByte(char char1, string encoding)
        {
            if ((char1 & 0xff00) == 0 || (char1 & 0xff00) == 0xf000)
            {
                return new[]
                {
                    (byte)char1
                };
            }

            return Array.Empty<byte>();
        }

        public byte[] CharToByte(string text, string encoding)
        {
            var ch = text.ToCharArray();
            var b = new byte[ch.Length];
            var ptr = 0;
            var len = ch.Length;

            for (var k = 0; k < len; ++k)
            {
                var c = ch[k];

                if ((c & 0xff00) == 0 || (c & 0xff00) == 0xf000)
                {
                    b[ptr++] = (byte)c;
                }
            }

            if (ptr == len)
            {
                return b;
            }

            var b2 = new byte[ptr];
            Array.Copy(b, 0, b2, 0, ptr);

            return b2;
        }
    }

    private class WingdingsConversion : IExtraEncoding
    {
        private static readonly byte[] _table =
        {
            0, 35, 34, 0, 0, 0, 41, 62, 81, 42, 0, 0, 65, 63, 0, 0, 0, 0, 0, 256 - 4, 0, 0, 0, 256 - 5, 0, 0, 0, 0, 0,
            0, 86, 0, 88, 89, 0, 0, 0, 0, 0, 0, 0, 0, 256 - 75, 0, 0, 0, 0, 0, 256 - 74, 0, 0, 0, 256 - 83, 256 - 81,
            256 - 84, 0, 0, 0, 0, 0, 0, 0, 0, 124, 123, 0, 0, 0, 84, 0, 0, 0, 0, 0, 0, 0, 0, 256 - 90, 0, 0, 0, 113,
            114, 0, 0, 0, 117, 0, 0, 0, 0, 0, 0, 125, 126, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 256 - 116, 256 - 115, 256 - 114, 256 - 113, 256 - 112, 256 - 111, 256 - 110, 256 - 109, 256 - 108,
            256 - 107, 256 - 127, 256 - 126, 256 - 125, 256 - 124, 256 - 123, 256 - 122, 256 - 121, 256 - 120,
            256 - 119, 256 - 118, 256 - 116, 256 - 115, 256 - 114, 256 - 113, 256 - 112, 256 - 111, 256 - 110,
            256 - 109, 256 - 108, 256 - 107, 256 - 24, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 256 - 24, 256 - 40, 0, 0,
            256 - 60, 256 - 58, 0, 0, 256 - 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 256 - 36, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        public string ByteToChar(byte[] b, string encoding)
            => null;

        public byte[] CharToByte(char char1, string encoding)
        {
            if (char1 == ' ')
            {
                return new[]
                {
                    (byte)char1
                };
            }

            if (char1 >= '\u2701' && char1 <= '\u27BE')
            {
                var v = _table[char1 - 0x2700];

                if (v != 0)
                {
                    return new[]
                    {
                        v
                    };
                }
            }

            return Array.Empty<byte>();
        }

        public byte[] CharToByte(string text, string encoding)
        {
            var cc = text.ToCharArray();
            var b = new byte[cc.Length];
            var ptr = 0;
            var len = cc.Length;

            for (var k = 0; k < len; ++k)
            {
                var c = cc[k];

                if (c == ' ')
                {
                    b[ptr++] = (byte)c;
                }
                else if (c >= '\u2701' && c <= '\u27BE')
                {
                    var v = _table[c - 0x2700];

                    if (v != 0)
                    {
                        b[ptr++] = v;
                    }
                }
            }

            if (ptr == len)
            {
                return b;
            }

            var b2 = new byte[ptr];
            Array.Copy(b, 0, b2, 0, ptr);

            return b2;
        }
    }
}