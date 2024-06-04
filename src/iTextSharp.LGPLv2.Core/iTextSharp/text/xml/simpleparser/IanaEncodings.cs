using System.Text;
using System.util;
using iTextSharp.LGPLv2.Core.System.Encodings;

namespace iTextSharp.text.xml.simpleparser;

/// <summary>
///     Translates a IANA encoding name to a Java encoding.
/// </summary>
public static class IanaEncodings
{
    /// <summary>
    ///     The object that maps IANA to Java encodings.
    /// </summary>
    private static readonly NullValueDictionary<string, int> _map = new();

    static IanaEncodings()
    {
        // add IANA to .NET encoding mappings.
        _map["CP037"] = 37;
        _map["CSIBM037"] = 37;
        _map["EBCDIC-CP-CA"] = 37;
        _map["EBCDIC-CP-NL"] = 37;
        _map["EBCDIC-CP-US"] = 37;
        _map["EBCDIC-CP-WT"] = 37;
        _map["IBM037"] = 37;
        _map["CP437"] = 437;
        _map["CSPC8CODEPAGE437"] = 437;
        _map["IBM437"] = 437;
        _map["CP500"] = 500;
        _map["CSIBM500"] = 500;
        _map["EBCDIC-CP-BE"] = 500;
        _map["EBCDIC-CP-CH"] = 500;
        _map["IBM500"] = 500;
        _map["ASMO-708"] = 708;
        _map["DOS-720"] = 720;
        _map["IBM737"] = 737;
        _map["IBM775"] = 775;
        _map["CP850"] = 850;
        _map["IBM850"] = 850;
        _map["CP852"] = 852;
        _map["IBM852"] = 852;
        _map["CP855"] = 855;
        _map["IBM855"] = 855;
        _map["CP857"] = 857;
        _map["IBM857"] = 857;
        _map["CCSID00858"] = 858;
        _map["CP00858"] = 858;
        _map["CP858"] = 858;
        _map["IBM00858"] = 858;
        _map["PC-MULTILINGUAL-850+EURO"] = 858;
        _map["CP860"] = 860;
        _map["IBM860"] = 860;
        _map["CP861"] = 861;
        _map["IBM861"] = 861;
        _map["CP862"] = 862;
        _map["DOS-862"] = 862;
        _map["IBM862"] = 862;
        _map["CP863"] = 863;
        _map["IBM863"] = 863;
        _map["CP864"] = 864;
        _map["IBM864"] = 864;
        _map["CP865"] = 865;
        _map["IBM865"] = 865;
        _map["CP866"] = 866;
        _map["IBM866"] = 866;
        _map["CP869"] = 869;
        _map["IBM869"] = 869;
        _map["CP870"] = 870;
        _map["CSIBM870"] = 870;
        _map["EBCDIC-CP-ROECE"] = 870;
        _map["EBCDIC-CP-YU"] = 870;
        _map["IBM870"] = 870;
        _map["DOS-874"] = 874;
        _map["ISO-8859-11"] = 874;
        _map["MS874"] = 874;
        _map["TIS620"] = 874;
        _map["TIS-620"] = 874;
        _map["WINDOWS-874"] = 874;
        _map["CP875"] = 875;
        _map["CSSHIFTJIS"] = 932;
        _map["CSWINDOWS31J"] = 932;
        _map["MS932"] = 932;
        _map["MS_KANJI"] = 932;
        _map["SHIFT-JIS"] = 932;
        _map["SHIFT_JIS"] = 932;
        _map["SJIS"] = 932;
        _map["X-MS-CP932"] = 932;
        _map["X-SJIS"] = 932;
        _map["CHINESE"] = 936;
        _map["CN-GB"] = 936;
        _map["CSGB2312"] = 936;
        _map["CSGB231280"] = 936;
        _map["CSISO58GB231280"] = 936;
        _map["GB2312"] = 936;
        _map["GB2312-80"] = 936;
        _map["GB231280"] = 936;
        _map["GB_2312-80"] = 936;
        _map["GBK"] = 936;
        _map["ISO-IR-58"] = 936;
        _map["MS936"] = 936;
        _map["CSKSC56011987"] = 949;
        _map["ISO-IR-149"] = 949;
        _map["KOREAN"] = 949;
        _map["KS-C-5601"] = 949;
        _map["KS-C5601"] = 949;
        _map["KS_C_5601"] = 949;
        _map["KS_C_5601-1987"] = 949;
        _map["KS_C_5601-1989"] = 949;
        _map["KS_C_5601_1987"] = 949;
        _map["KSC5601"] = 949;
        _map["KSC_5601"] = 949;
        _map["MS949"] = 949;
        _map["BIG5"] = 950;
        _map["BIG5-HKSCS"] = 950;
        _map["CN-BIG5"] = 950;
        _map["CSBIG5"] = 950;
        _map["MS950"] = 950;
        _map["X-X-BIG5"] = 950;
        _map["CP1026"] = 1026;
        _map["CSIBM1026"] = 1026;
        _map["IBM1026"] = 1026;
        _map["IBM01047"] = 1047;
        _map["CCSID01140"] = 1140;
        _map["CP01140"] = 1140;
        _map["EBCDIC-US-37+EURO"] = 1140;
        _map["IBM01140"] = 1140;
        _map["CCSID01141"] = 1141;
        _map["CP01141"] = 1141;
        _map["EBCDIC-DE-273+EURO"] = 1141;
        _map["IBM01141"] = 1141;
        _map["CCSID01142"] = 1142;
        _map["CP01142"] = 1142;
        _map["EBCDIC-DK-277+EURO"] = 1142;
        _map["EBCDIC-NO-277+EURO"] = 1142;
        _map["IBM01142"] = 1142;
        _map["CCSID01143"] = 1143;
        _map["CP01143"] = 1143;
        _map["EBCDIC-FI-278+EURO"] = 1143;
        _map["EBCDIC-SE-278+EURO"] = 1143;
        _map["IBM01143"] = 1143;
        _map["CCSID01144"] = 1144;
        _map["CP01144"] = 1144;
        _map["EBCDIC-IT-280+EURO"] = 1144;
        _map["IBM01144"] = 1144;
        _map["CCSID01145"] = 1145;
        _map["CP01145"] = 1145;
        _map["EBCDIC-ES-284+EURO"] = 1145;
        _map["IBM01145"] = 1145;
        _map["CCSID01146"] = 1146;
        _map["CP01146"] = 1146;
        _map["EBCDIC-GB-285+EURO"] = 1146;
        _map["IBM01146"] = 1146;
        _map["CCSID01147"] = 1147;
        _map["CP01147"] = 1147;
        _map["EBCDIC-FR-297+EURO"] = 1147;
        _map["IBM01147"] = 1147;
        _map["CCSID01148"] = 1148;
        _map["CP01148"] = 1148;
        _map["EBCDIC-INTERNATIONAL-500+EURO"] = 1148;
        _map["IBM01148"] = 1148;
        _map["CCSID01149"] = 1149;
        _map["CP01149"] = 1149;
        _map["EBCDIC-IS-871+EURO"] = 1149;
        _map["IBM01149"] = 1149;
        _map["ISO-10646-UCS-2"] = 1200;
        _map["UCS-2"] = 1200;
        _map["UNICODE"] = 1200;
        _map["UTF-16"] = 1200;
        _map["UTF-16LE"] = 1200;
        _map["UNICODELITTLEUNMARKED"] = 1200;
        _map["UNICODELITTLE"] = 1200;
        _map["UNICODEFFFE"] = 1201;
        _map["UTF-16BE"] = 1201;
        _map["UNICODEBIGUNMARKED"] = 1201;
        _map["UNICODEBIG"] = 1201;
        _map["CP1250"] = 1250;
        _map["WINDOWS-1250"] = 1250;
        _map["X-CP1250"] = 1250;
        _map["CP1251"] = 1251;
        _map["WINDOWS-1251"] = 1251;
        _map["X-CP1251"] = 1251;
        _map["CP1252"] = 1252;
        _map["WINDOWS-1252"] = 1252;
        _map["X-ANSI"] = 1252;
        _map["CP1253"] = 1253;
        _map["WINDOWS-1253"] = 1253;
        _map["CP1254"] = 1254;
        _map["WINDOWS-1254"] = 1254;
        _map["CP1255"] = 1255;
        _map["WINDOWS-1255"] = 1255;
        _map["CP1256"] = 1256;
        _map["WINDOWS-1256"] = 1256;
        _map["CP1257"] = 1257;
        _map["WINDOWS-1257"] = 1257;
        _map["CP1258"] = 1258;
        _map["WINDOWS-1258"] = 1258;
        _map["JOHAB"] = 1361;
        _map["MACINTOSH"] = 10000;
        _map["MACROMAN"] = 10000;
        _map["X-MAC-JAPANESE"] = 10001;
        _map["X-MAC-CHINESETRAD"] = 10002;
        _map["X-MAC-KOREAN"] = 10003;
        _map["MACARABIC"] = 10004;
        _map["X-MAC-ARABIC"] = 10004;
        _map["MACHEBREW"] = 10005;
        _map["X-MAC-HEBREW"] = 10005;
        _map["MACGREEK"] = 10006;
        _map["X-MAC-GREEK"] = 10006;
        _map["MACCYRILLIC"] = 10007;
        _map["X-MAC-CYRILLIC"] = 10007;
        _map["X-MAC-CHINESESIMP"] = 10008;
        _map["MACROMANIA"] = 10010;
        _map["MACROMANIAN"] = 10010;
        _map["X-MAC-ROMANIAN"] = 10010;
        _map["MACUKRAINE"] = 10017;
        _map["MACUKRAINIAN"] = 10017;
        _map["X-MAC-UKRAINIAN"] = 10017;
        _map["MACTHAI"] = 10021;
        _map["X-MAC-THAI"] = 10021;
        _map["MACCENTRALEUROPE"] = 10029;
        _map["X-MAC-CE"] = 10029;
        _map["MACICELANDIC"] = 10079;
        _map["MACICELAND"] = 10079;
        _map["X-MAC-ICELANDIC"] = 10079;
        _map["MACTURKISH"] = 10081;
        _map["X-MAC-TURKISH"] = 10081;
        _map["MACCROATIAN"] = 10082;
        _map["X-MAC-CROATIAN"] = 10082;
        _map["X-CHINESE-CNS"] = 20000;
        _map["X-CP20001"] = 20001;
        _map["X-CHINESE-ETEN"] = 20002;
        _map["X-CP20003"] = 20003;
        _map["X-CP20004"] = 20004;
        _map["X-CP20005"] = 20005;
        _map["IRV"] = 20105;
        _map["X-IA5"] = 20105;
        _map["DIN_66003"] = 20106;
        _map["GERMAN"] = 20106;
        _map["X-IA5-GERMAN"] = 20106;
        _map["SEN_850200_B"] = 20107;
        _map["SWEDISH"] = 20107;
        _map["X-IA5-SWEDISH"] = 20107;
        _map["NORWEGIAN"] = 20108;
        _map["NS_4551-1"] = 20108;
        _map["X-IA5-NORWEGIAN"] = 20108;
        _map["ANSI_X3.4-1968"] = 20127;
        _map["ANSI_X3.4-1986"] = 20127;
        _map["ASCII"] = 20127;
        _map["CP367"] = 20127;
        _map["CSASCII"] = 20127;
        _map["IBM367"] = 20127;
        _map["ISO-IR-6"] = 20127;
        _map["ISO646-US"] = 20127;
        _map["ISO_646.IRV:1991"] = 20127;
        _map["US"] = 20127;
        _map["US-ASCII"] = 20127;
        _map["X-CP20261"] = 20261;
        _map["X-CP20269"] = 20269;
        _map["CP273"] = 20273;
        _map["CSIBM273"] = 20273;
        _map["IBM273"] = 20273;
        _map["CSIBM277"] = 20277;
        _map["EBCDIC-CP-DK"] = 20277;
        _map["EBCDIC-CP-NO"] = 20277;
        _map["IBM277"] = 20277;
        _map["CP278"] = 20278;
        _map["CSIBM278"] = 20278;
        _map["EBCDIC-CP-FI"] = 20278;
        _map["EBCDIC-CP-SE"] = 20278;
        _map["IBM278"] = 20278;
        _map["CP280"] = 20280;
        _map["CSIBM280"] = 20280;
        _map["EBCDIC-CP-IT"] = 20280;
        _map["IBM280"] = 20280;
        _map["CP284"] = 20284;
        _map["CSIBM284"] = 20284;
        _map["EBCDIC-CP-ES"] = 20284;
        _map["IBM284"] = 20284;
        _map["CP285"] = 20285;
        _map["CSIBM285"] = 20285;
        _map["EBCDIC-CP-GB"] = 20285;
        _map["IBM285"] = 20285;
        _map["CP290"] = 20290;
        _map["CSIBM290"] = 20290;
        _map["EBCDIC-JP-KANA"] = 20290;
        _map["IBM290"] = 20290;
        _map["CP297"] = 20297;
        _map["CSIBM297"] = 20297;
        _map["EBCDIC-CP-FR"] = 20297;
        _map["IBM297"] = 20297;
        _map["CP420"] = 20420;
        _map["CSIBM420"] = 20420;
        _map["EBCDIC-CP-AR1"] = 20420;
        _map["IBM420"] = 20420;
        _map["CP423"] = 20423;
        _map["CSIBM423"] = 20423;
        _map["EBCDIC-CP-GR"] = 20423;
        _map["IBM423"] = 20423;
        _map["CP424"] = 20424;
        _map["CSIBM424"] = 20424;
        _map["EBCDIC-CP-HE"] = 20424;
        _map["IBM424"] = 20424;
        _map["X-EBCDIC-KOREANEXTENDED"] = 20833;
        _map["CSIBMTHAI"] = 20838;
        _map["IBM-THAI"] = 20838;
        _map["CSKOI8R"] = 20866;
        _map["KOI"] = 20866;
        _map["KOI8"] = 20866;
        _map["KOI8-R"] = 20866;
        _map["KOI8R"] = 20866;
        _map["CP871"] = 20871;
        _map["CSIBM871"] = 20871;
        _map["EBCDIC-CP-IS"] = 20871;
        _map["IBM871"] = 20871;
        _map["CP880"] = 20880;
        _map["CSIBM880"] = 20880;
        _map["EBCDIC-CYRILLIC"] = 20880;
        _map["IBM880"] = 20880;
        _map["CP905"] = 20905;
        _map["CSIBM905"] = 20905;
        _map["EBCDIC-CP-TR"] = 20905;
        _map["IBM905"] = 20905;
        _map["CCSID00924"] = 20924;
        _map["CP00924"] = 20924;
        _map["EBCDIC-LATIN9--EURO"] = 20924;
        _map["IBM00924"] = 20924;
        _map["X-CP20936"] = 20936;
        _map["X-CP20949"] = 20949;
        _map["CP1025"] = 21025;
        _map["X-CP21027"] = 21027;
        _map["KOI8-RU"] = 21866;
        _map["KOI8-U"] = 21866;
        _map["CP819"] = 28591;
        _map["CSISOLATIN1"] = 28591;
        _map["IBM819"] = 28591;
        _map["ISO-8859-1"] = 28591;
        _map["ISO-IR-100"] = 28591;
        _map["ISO8859-1"] = 28591;
        _map["ISO_8859-1"] = 28591;
        _map["ISO_8859-1:1987"] = 28591;
        _map["L1"] = 28591;
        _map["LATIN1"] = 28591;
        _map["CSISOLATIN2"] = 28592;
        _map["ISO-8859-2"] = 28592;
        _map["ISO-IR-101"] = 28592;
        _map["ISO8859-2"] = 28592;
        _map["ISO_8859-2"] = 28592;
        _map["ISO_8859-2:1987"] = 28592;
        _map["L2"] = 28592;
        _map["LATIN2"] = 28592;
        _map["CSISOLATIN3"] = 28593;
        _map["ISO-8859-3"] = 28593;
        _map["ISO-IR-109"] = 28593;
        _map["ISO_8859-3"] = 28593;
        _map["ISO_8859-3:1988"] = 28593;
        _map["L3"] = 28593;
        _map["LATIN3"] = 28593;
        _map["CSISOLATIN4"] = 28594;
        _map["ISO-8859-4"] = 28594;
        _map["ISO-IR-110"] = 28594;
        _map["ISO_8859-4"] = 28594;
        _map["ISO_8859-4:1988"] = 28594;
        _map["L4"] = 28594;
        _map["LATIN4"] = 28594;
        _map["CSISOLATINCYRILLIC"] = 28595;
        _map["CYRILLIC"] = 28595;
        _map["ISO-8859-5"] = 28595;
        _map["ISO-IR-144"] = 28595;
        _map["ISO_8859-5"] = 28595;
        _map["ISO_8859-5:1988"] = 28595;
        _map["ARABIC"] = 28596;
        _map["CSISOLATINARABIC"] = 28596;
        _map["ECMA-114"] = 28596;
        _map["ISO-8859-6"] = 28596;
        _map["ISO-IR-127"] = 28596;
        _map["ISO_8859-6"] = 28596;
        _map["ISO_8859-6:1987"] = 28596;
        _map["CSISOLATINGREEK"] = 28597;
        _map["ECMA-118"] = 28597;
        _map["ELOT_928"] = 28597;
        _map["GREEK"] = 28597;
        _map["GREEK8"] = 28597;
        _map["ISO-8859-7"] = 28597;
        _map["ISO-IR-126"] = 28597;
        _map["ISO_8859-7"] = 28597;
        _map["ISO_8859-7:1987"] = 28597;
        _map["CSISOLATINHEBREW"] = 28598;
        _map["HEBREW"] = 28598;
        _map["ISO-8859-8"] = 28598;
        _map["ISO-IR-138"] = 28598;
        _map["ISO_8859-8"] = 28598;
        _map["ISO_8859-8:1988"] = 28598;
        _map["LOGICAL"] = 28598;
        _map["VISUAL"] = 28598;
        _map["CSISOLATIN5"] = 28599;
        _map["ISO-8859-9"] = 28599;
        _map["ISO-IR-148"] = 28599;
        _map["ISO_8859-9"] = 28599;
        _map["ISO_8859-9:1989"] = 28599;
        _map["L5"] = 28599;
        _map["LATIN5"] = 28599;
        _map["ISO-8859-13"] = 28603;
        _map["CSISOLATIN9"] = 28605;
        _map["ISO-8859-15"] = 28605;
        _map["ISO_8859-15"] = 28605;
        _map["L9"] = 28605;
        _map["LATIN9"] = 28605;
        _map["X-EUROPA"] = 29001;
        _map["ISO-8859-8-I"] = 38598;
        _map["ISO-2022-JP"] = 50220;
        _map["CSISO2022JP"] = 50221;
        _map["CSISO2022KR"] = 50225;
        _map["ISO-2022-KR"] = 50225;
        _map["ISO-2022-KR-7"] = 50225;
        _map["ISO-2022-KR-7BIT"] = 50225;
        _map["CP50227"] = 50227;
        _map["X-CP50227"] = 50227;
        _map["CP930"] = 50930;
        _map["X-EBCDIC-JAPANESEANDUSCANADA"] = 50931;
        _map["CP933"] = 50933;
        _map["CP935"] = 50935;
        _map["CP937"] = 50937;
        _map["CP939"] = 50939;
        _map["CSEUCPKDFMTJAPANESE"] = 51932;
        _map["EUC-JP"] = 51932;
        _map["EXTENDED_UNIX_CODE_PACKED_FORMAT_FOR_JAPANESE"] = 51932;
        _map["ISO-2022-JPEUC"] = 51932;
        _map["X-EUC"] = 51932;
        _map["X-EUC-JP"] = 51932;
        _map["EUC-CN"] = 51936;
        _map["X-EUC-CN"] = 51936;
        _map["CSEUCKR"] = 51949;
        _map["EUC-KR"] = 51949;
        _map["ISO-2022-KR-8"] = 51949;
        _map["ISO-2022-KR-8BIT"] = 51949;
        _map["HZ-GB-2312"] = 52936;
        _map["GB18030"] = 54936;
        _map["X-ISCII-DE"] = 57002;
        _map["X-ISCII-BE"] = 57003;
        _map["X-ISCII-TA"] = 57004;
        _map["X-ISCII-TE"] = 57005;
        _map["X-ISCII-AS"] = 57006;
        _map["X-ISCII-OR"] = 57007;
        _map["X-ISCII-KA"] = 57008;
        _map["X-ISCII-MA"] = 57009;
        _map["X-ISCII-GU"] = 57010;
        _map["X-ISCII-PA"] = 57011;
        _map["CSUNICODE11UTF7"] = 65000;
        _map["UNICODE-1-1-UTF-7"] = 65000;
        _map["UNICODE-2-0-UTF-7"] = 65000;
        _map["UTF-7"] = 65000;
        _map["X-UNICODE-1-1-UTF-7"] = 65000;
        _map["X-UNICODE-2-0-UTF-7"] = 65000;
        _map["UNICODE-1-1-UTF-8"] = 65001;
        _map["UNICODE-2-0-UTF-8"] = 65001;
        _map["UTF-8"] = 65001;
        _map["X-UNICODE-1-1-UTF-8"] = 65001;
        _map["X-UNICODE-2-0-UTF-8"] = 65001;
    }

    public static int GetEncodingNumber(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        object n = _map[name.ToUpper(CultureInfo.InvariantCulture)];

        if (n == null)
        {
            return 0;
        }

        return (int)n;
    }

    public static Encoding GetEncodingEncoding(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var nameU = name.ToUpper(CultureInfo.InvariantCulture);

        if (nameU.Equals("UNICODEBIGUNMARKED", StringComparison.Ordinal))
        {
            return new UnicodeEncoding(true, false);
        }

        if (nameU.Equals("UNICODEBIG", StringComparison.Ordinal))
        {
            return new UnicodeEncoding(true, true);
        }

        if (nameU.Equals("UNICODELITTLEUNMARKED", StringComparison.Ordinal))
        {
            return new UnicodeEncoding(false, false);
        }

        if (nameU.Equals("UNICODELITTLE", StringComparison.Ordinal))
        {
            return new UnicodeEncoding(false, true);
        }

        if (_map.TryGetValue(nameU, out var value))
        {
            return EncodingsRegistry.GetEncoding(value);
        }

        return EncodingsRegistry.GetEncoding(name);
    }
}