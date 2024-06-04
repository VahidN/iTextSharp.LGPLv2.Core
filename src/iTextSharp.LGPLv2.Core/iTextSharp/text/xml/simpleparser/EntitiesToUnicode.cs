using System.Text;
using System.util;

namespace iTextSharp.text.xml.simpleparser;

/// <summary>
///     This class contains entities that can be used in an entity tag.
/// </summary>
public static class EntitiesToUnicode
{
    /// <summary>
    ///     This is a map that contains the names of entities and their unicode value.
    /// </summary>
    public static readonly INullValueDictionary<string, char> Map = new NullValueDictionary<string, char>();

    static EntitiesToUnicode()
    {
        Map["nbsp"] = '\u00a0'; // no-break space = non-breaking space, U+00A0 ISOnum
        Map["iexcl"] = '\u00a1'; // inverted exclamation mark, U+00A1 ISOnum
        Map["cent"] = '\u00a2'; // cent sign, U+00A2 ISOnum
        Map["pound"] = '\u00a3'; // pound sign, U+00A3 ISOnum
        Map["curren"] = '\u00a4'; // currency sign, U+00A4 ISOnum
        Map["yen"] = '\u00a5'; // yen sign = yuan sign, U+00A5 ISOnum
        Map["brvbar"] = '\u00a6'; // broken bar = broken vertical bar, U+00A6 ISOnum
        Map["sect"] = '\u00a7'; // section sign, U+00A7 ISOnum
        Map["uml"] = '\u00a8'; // diaeresis = spacing diaeresis, U+00A8 ISOdia
        Map["copy"] = '\u00a9'; // copyright sign, U+00A9 ISOnum
        Map["ordf"] = '\u00aa'; // feminine ordinal indicator, U+00AA ISOnum
        Map["laquo"] = '\u00ab'; // left-pointing double angle quotation mark = left pointing guillemet, U+00AB ISOnum
        Map["not"] = '\u00ac'; // not sign, U+00AC ISOnum
        Map["shy"] = '\u00ad'; // soft hyphen = discretionary hyphen, U+00AD ISOnum
        Map["reg"] = '\u00ae'; // registered sign = registered trade mark sign, U+00AE ISOnum
        Map["macr"] = '\u00af'; // macron = spacing macron = overline = APL overbar, U+00AF ISOdia
        Map["deg"] = '\u00b0'; // degree sign, U+00B0 ISOnum
        Map["plusmn"] = '\u00b1'; // plus-minus sign = plus-or-minus sign, U+00B1 ISOnum
        Map["sup2"] = '\u00b2'; // superscript two = superscript digit two = squared, U+00B2 ISOnum
        Map["sup3"] = '\u00b3'; // superscript three = superscript digit three = cubed, U+00B3 ISOnum
        Map["acute"] = '\u00b4'; // acute accent = spacing acute, U+00B4 ISOdia
        Map["micro"] = '\u00b5'; // micro sign, U+00B5 ISOnum
        Map["para"] = '\u00b6'; // pilcrow sign = paragraph sign, U+00B6 ISOnum
        Map["middot"] = '\u00b7'; // middle dot = Georgian comma = Greek middle dot, U+00B7 ISOnum
        Map["cedil"] = '\u00b8'; // cedilla = spacing cedilla, U+00B8 ISOdia
        Map["sup1"] = '\u00b9'; // superscript one = superscript digit one, U+00B9 ISOnum
        Map["ordm"] = '\u00ba'; // masculine ordinal indicator, U+00BA ISOnum
        Map["raquo"] = '\u00bb'; // right-pointing double angle quotation mark = right pointing guillemet, U+00BB ISOnum
        Map["frac14"] = '\u00bc'; // vulgar fraction one quarter = fraction one quarter, U+00BC ISOnum
        Map["frac12"] = '\u00bd'; // vulgar fraction one half = fraction one half, U+00BD ISOnum
        Map["frac34"] = '\u00be'; // vulgar fraction three quarters = fraction three quarters, U+00BE ISOnum
        Map["iquest"] = '\u00bf'; // inverted question mark = turned question mark, U+00BF ISOnum
        Map["Agrave"] = '\u00c0'; // latin capital letter A with grave = latin capital letter A grave, U+00C0 ISOlat1
        Map["Aacute"] = '\u00c1'; // latin capital letter A with acute, U+00C1 ISOlat1
        Map["Acirc"] = '\u00c2'; // latin capital letter A with circumflex, U+00C2 ISOlat1
        Map["Atilde"] = '\u00c3'; // latin capital letter A with tilde, U+00C3 ISOlat1
        Map["Auml"] = '\u00c4'; // latin capital letter A with diaeresis, U+00C4 ISOlat1
        Map["Aring"] = '\u00c5'; // latin capital letter A with ring above = latin capital letter A ring, U+00C5 ISOlat1
        Map["AElig"] = '\u00c6'; // latin capital letter AE = latin capital ligature AE, U+00C6 ISOlat1
        Map["Ccedil"] = '\u00c7'; // latin capital letter C with cedilla, U+00C7 ISOlat1
        Map["Egrave"] = '\u00c8'; // latin capital letter E with grave, U+00C8 ISOlat1
        Map["Eacute"] = '\u00c9'; // latin capital letter E with acute, U+00C9 ISOlat1
        Map["Ecirc"] = '\u00ca'; // latin capital letter E with circumflex, U+00CA ISOlat1
        Map["Euml"] = '\u00cb'; // latin capital letter E with diaeresis, U+00CB ISOlat1
        Map["Igrave"] = '\u00cc'; // latin capital letter I with grave, U+00CC ISOlat1
        Map["Iacute"] = '\u00cd'; // latin capital letter I with acute, U+00CD ISOlat1
        Map["Icirc"] = '\u00ce'; // latin capital letter I with circumflex, U+00CE ISOlat1
        Map["Iuml"] = '\u00cf'; // latin capital letter I with diaeresis, U+00CF ISOlat1
        Map["ETH"] = '\u00d0'; // latin capital letter ETH, U+00D0 ISOlat1
        Map["Ntilde"] = '\u00d1'; // latin capital letter N with tilde, U+00D1 ISOlat1
        Map["Ograve"] = '\u00d2'; // latin capital letter O with grave, U+00D2 ISOlat1
        Map["Oacute"] = '\u00d3'; // latin capital letter O with acute, U+00D3 ISOlat1
        Map["Ocirc"] = '\u00d4'; // latin capital letter O with circumflex, U+00D4 ISOlat1
        Map["Otilde"] = '\u00d5'; // latin capital letter O with tilde, U+00D5 ISOlat1
        Map["Ouml"] = '\u00d6'; // latin capital letter O with diaeresis, U+00D6 ISOlat1
        Map["times"] = '\u00d7'; // multiplication sign, U+00D7 ISOnum
        Map["Oslash"] = '\u00d8'; // latin capital letter O with stroke = latin capital letter O slash, U+00D8 ISOlat1
        Map["Ugrave"] = '\u00d9'; // latin capital letter U with grave, U+00D9 ISOlat1
        Map["Uacute"] = '\u00da'; // latin capital letter U with acute, U+00DA ISOlat1
        Map["Ucirc"] = '\u00db'; // latin capital letter U with circumflex, U+00DB ISOlat1
        Map["Uuml"] = '\u00dc'; // latin capital letter U with diaeresis, U+00DC ISOlat1
        Map["Yacute"] = '\u00dd'; // latin capital letter Y with acute, U+00DD ISOlat1
        Map["THORN"] = '\u00de'; // latin capital letter THORN, U+00DE ISOlat1
        Map["szlig"] = '\u00df'; // latin small letter sharp s = ess-zed, U+00DF ISOlat1
        Map["agrave"] = '\u00e0'; // latin small letter a with grave = latin small letter a grave, U+00E0 ISOlat1
        Map["aacute"] = '\u00e1'; // latin small letter a with acute, U+00E1 ISOlat1
        Map["acirc"] = '\u00e2'; // latin small letter a with circumflex, U+00E2 ISOlat1
        Map["atilde"] = '\u00e3'; // latin small letter a with tilde, U+00E3 ISOlat1
        Map["auml"] = '\u00e4'; // latin small letter a with diaeresis, U+00E4 ISOlat1
        Map["aring"] = '\u00e5'; // latin small letter a with ring above = latin small letter a ring, U+00E5 ISOlat1
        Map["aelig"] = '\u00e6'; // latin small letter ae = latin small ligature ae, U+00E6 ISOlat1
        Map["ccedil"] = '\u00e7'; // latin small letter c with cedilla, U+00E7 ISOlat1
        Map["egrave"] = '\u00e8'; // latin small letter e with grave, U+00E8 ISOlat1
        Map["eacute"] = '\u00e9'; // latin small letter e with acute, U+00E9 ISOlat1
        Map["ecirc"] = '\u00ea'; // latin small letter e with circumflex, U+00EA ISOlat1
        Map["euml"] = '\u00eb'; // latin small letter e with diaeresis, U+00EB ISOlat1
        Map["igrave"] = '\u00ec'; // latin small letter i with grave, U+00EC ISOlat1
        Map["iacute"] = '\u00ed'; // latin small letter i with acute, U+00ED ISOlat1
        Map["icirc"] = '\u00ee'; // latin small letter i with circumflex, U+00EE ISOlat1
        Map["iuml"] = '\u00ef'; // latin small letter i with diaeresis, U+00EF ISOlat1
        Map["eth"] = '\u00f0'; // latin small letter eth, U+00F0 ISOlat1
        Map["ntilde"] = '\u00f1'; // latin small letter n with tilde, U+00F1 ISOlat1
        Map["ograve"] = '\u00f2'; // latin small letter o with grave, U+00F2 ISOlat1
        Map["oacute"] = '\u00f3'; // latin small letter o with acute, U+00F3 ISOlat1
        Map["ocirc"] = '\u00f4'; // latin small letter o with circumflex, U+00F4 ISOlat1
        Map["otilde"] = '\u00f5'; // latin small letter o with tilde, U+00F5 ISOlat1
        Map["ouml"] = '\u00f6'; // latin small letter o with diaeresis, U+00F6 ISOlat1
        Map["divide"] = '\u00f7'; // division sign, U+00F7 ISOnum
        Map["oslash"] = '\u00f8'; // latin small letter o with stroke, = latin small letter o slash, U+00F8 ISOlat1
        Map["ugrave"] = '\u00f9'; // latin small letter u with grave, U+00F9 ISOlat1
        Map["uacute"] = '\u00fa'; // latin small letter u with acute, U+00FA ISOlat1
        Map["ucirc"] = '\u00fb'; // latin small letter u with circumflex, U+00FB ISOlat1
        Map["uuml"] = '\u00fc'; // latin small letter u with diaeresis, U+00FC ISOlat1
        Map["yacute"] = '\u00fd'; // latin small letter y with acute, U+00FD ISOlat1
        Map["thorn"] = '\u00fe'; // latin small letter thorn, U+00FE ISOlat1
        Map["yuml"] = '\u00ff'; // latin small letter y with diaeresis, U+00FF ISOlat1
        // Latin Extended-B
        Map["fnof"] = '\u0192'; // latin small f with hook = function = florin, U+0192 ISOtech
        // Greek
        Map["Alpha"] = '\u0391'; // greek capital letter alpha, U+0391
        Map["Beta"] = '\u0392'; // greek capital letter beta, U+0392
        Map["Gamma"] = '\u0393'; // greek capital letter gamma, U+0393 ISOgrk3
        Map["Delta"] = '\u0394'; // greek capital letter delta, U+0394 ISOgrk3
        Map["Epsilon"] = '\u0395'; // greek capital letter epsilon, U+0395
        Map["Zeta"] = '\u0396'; // greek capital letter zeta, U+0396
        Map["Eta"] = '\u0397'; // greek capital letter eta, U+0397
        Map["Theta"] = '\u0398'; // greek capital letter theta, U+0398 ISOgrk3
        Map["Iota"] = '\u0399'; // greek capital letter iota, U+0399
        Map["Kappa"] = '\u039a'; // greek capital letter kappa, U+039A
        Map["Lambda"] = '\u039b'; // greek capital letter lambda, U+039B ISOgrk3
        Map["Mu"] = '\u039c'; // greek capital letter mu, U+039C
        Map["Nu"] = '\u039d'; // greek capital letter nu, U+039D
        Map["Xi"] = '\u039e'; // greek capital letter xi, U+039E ISOgrk3
        Map["Omicron"] = '\u039f'; // greek capital letter omicron, U+039F
        Map["Pi"] = '\u03a0'; // greek capital letter pi, U+03A0 ISOgrk3
        Map["Rho"] = '\u03a1'; // greek capital letter rho, U+03A1
        // there is no Sigmaf, and no U+03A2 character either
        Map["Sigma"] = '\u03a3'; // greek capital letter sigma, U+03A3 ISOgrk3
        Map["Tau"] = '\u03a4'; // greek capital letter tau, U+03A4
        Map["Upsilon"] = '\u03a5'; // greek capital letter upsilon, U+03A5 ISOgrk3
        Map["Phi"] = '\u03a6'; // greek capital letter phi, U+03A6 ISOgrk3
        Map["Chi"] = '\u03a7'; // greek capital letter chi, U+03A7
        Map["Psi"] = '\u03a8'; // greek capital letter psi, U+03A8 ISOgrk3
        Map["Omega"] = '\u03a9'; // greek capital letter omega, U+03A9 ISOgrk3
        Map["alpha"] = '\u03b1'; // greek small letter alpha, U+03B1 ISOgrk3
        Map["beta"] = '\u03b2'; // greek small letter beta, U+03B2 ISOgrk3
        Map["gamma"] = '\u03b3'; // greek small letter gamma, U+03B3 ISOgrk3
        Map["delta"] = '\u03b4'; // greek small letter delta, U+03B4 ISOgrk3
        Map["epsilon"] = '\u03b5'; // greek small letter epsilon, U+03B5 ISOgrk3
        Map["zeta"] = '\u03b6'; // greek small letter zeta, U+03B6 ISOgrk3
        Map["eta"] = '\u03b7'; // greek small letter eta, U+03B7 ISOgrk3
        Map["theta"] = '\u03b8'; // greek small letter theta, U+03B8 ISOgrk3
        Map["iota"] = '\u03b9'; // greek small letter iota, U+03B9 ISOgrk3
        Map["kappa"] = '\u03ba'; // greek small letter kappa, U+03BA ISOgrk3
        Map["lambda"] = '\u03bb'; // greek small letter lambda, U+03BB ISOgrk3
        Map["mu"] = '\u03bc'; // greek small letter mu, U+03BC ISOgrk3
        Map["nu"] = '\u03bd'; // greek small letter nu, U+03BD ISOgrk3
        Map["xi"] = '\u03be'; // greek small letter xi, U+03BE ISOgrk3
        Map["omicron"] = '\u03bf'; // greek small letter omicron, U+03BF NEW
        Map["pi"] = '\u03c0'; // greek small letter pi, U+03C0 ISOgrk3
        Map["rho"] = '\u03c1'; // greek small letter rho, U+03C1 ISOgrk3
        Map["sigmaf"] = '\u03c2'; // greek small letter final sigma, U+03C2 ISOgrk3
        Map["sigma"] = '\u03c3'; // greek small letter sigma, U+03C3 ISOgrk3
        Map["tau"] = '\u03c4'; // greek small letter tau, U+03C4 ISOgrk3
        Map["upsilon"] = '\u03c5'; // greek small letter upsilon, U+03C5 ISOgrk3
        Map["phi"] = '\u03c6'; // greek small letter phi, U+03C6 ISOgrk3
        Map["chi"] = '\u03c7'; // greek small letter chi, U+03C7 ISOgrk3
        Map["psi"] = '\u03c8'; // greek small letter psi, U+03C8 ISOgrk3
        Map["omega"] = '\u03c9'; // greek small letter omega, U+03C9 ISOgrk3
        Map["thetasym"] = '\u03d1'; // greek small letter theta symbol, U+03D1 NEW
        Map["upsih"] = '\u03d2'; // greek upsilon with hook symbol, U+03D2 NEW
        Map["piv"] = '\u03d6'; // greek pi symbol, U+03D6 ISOgrk3
        // General Punctuation
        Map["bull"] = '\u2022'; // bullet = black small circle, U+2022 ISOpub
        // bullet is NOT the same as bullet operator, U+2219
        Map["hellip"] = '\u2026'; // horizontal ellipsis = three dot leader, U+2026 ISOpub
        Map["prime"] = '\u2032'; // prime = minutes = feet, U+2032 ISOtech
        Map["Prime"] = '\u2033'; // double prime = seconds = inches, U+2033 ISOtech
        Map["oline"] = '\u203e'; // overline = spacing overscore, U+203E NEW
        Map["frasl"] = '\u2044'; // fraction slash, U+2044 NEW
        // Letterlike Symbols
        Map["weierp"] = '\u2118'; // script capital P = power set = Weierstrass p, U+2118 ISOamso
        Map["image"] = '\u2111'; // blackletter capital I = imaginary part, U+2111 ISOamso
        Map["real"] = '\u211c'; // blackletter capital R = real part symbol, U+211C ISOamso
        Map["trade"] = '\u2122'; // trade mark sign, U+2122 ISOnum
        Map["alefsym"] = '\u2135'; // alef symbol = first transfinite cardinal, U+2135 NEW
        // alef symbol is NOT the same as hebrew letter alef,
        // U+05D0 although the same glyph could be used to depict both characters
        // Arrows
        Map["larr"] = '\u2190'; // leftwards arrow, U+2190 ISOnum
        Map["uarr"] = '\u2191'; // upwards arrow, U+2191 ISOnum
        Map["rarr"] = '\u2192'; // rightwards arrow, U+2192 ISOnum
        Map["darr"] = '\u2193'; // downwards arrow, U+2193 ISOnum
        Map["harr"] = '\u2194'; // left right arrow, U+2194 ISOamsa
        Map["crarr"] = '\u21b5'; // downwards arrow with corner leftwards = carriage return, U+21B5 NEW
        Map["lArr"] = '\u21d0'; // leftwards double arrow, U+21D0 ISOtech
        // ISO 10646 does not say that lArr is the same as the 'is implied by' arrow
        // but also does not have any other character for that function. So ? lArr can
        // be used for 'is implied by' as ISOtech suggests
        Map["uArr"] = '\u21d1'; // upwards double arrow, U+21D1 ISOamsa
        Map["rArr"] = '\u21d2'; // rightwards double arrow, U+21D2 ISOtech
        // ISO 10646 does not say this is the 'implies' character but does not have
        // another character with this function so ?
        // rArr can be used for 'implies' as ISOtech suggests
        Map["dArr"] = '\u21d3'; // downwards double arrow, U+21D3 ISOamsa
        Map["hArr"] = '\u21d4'; // left right double arrow, U+21D4 ISOamsa
        // Mathematical Operators
        Map["forall"] = '\u2200'; // for all, U+2200 ISOtech
        Map["part"] = '\u2202'; // partial differential, U+2202 ISOtech
        Map["exist"] = '\u2203'; // there exists, U+2203 ISOtech
        Map["empty"] = '\u2205'; // empty set = null set = diameter, U+2205 ISOamso
        Map["nabla"] = '\u2207'; // nabla = backward difference, U+2207 ISOtech
        Map["isin"] = '\u2208'; // element of, U+2208 ISOtech
        Map["notin"] = '\u2209'; // not an element of, U+2209 ISOtech
        Map["ni"] = '\u220b'; // contains as member, U+220B ISOtech
        // should there be a more memorable name than 'ni'?
        Map["prod"] = '\u220f'; // n-ary product = product sign, U+220F ISOamsb
        // prod is NOT the same character as U+03A0 'greek capital letter pi' though
        // the same glyph might be used for both
        Map["sum"] = '\u2211'; // n-ary sumation, U+2211 ISOamsb
        // sum is NOT the same character as U+03A3 'greek capital letter sigma'
        // though the same glyph might be used for both
        Map["minus"] = '\u2212'; // minus sign, U+2212 ISOtech
        Map["lowast"] = '\u2217'; // asterisk operator, U+2217 ISOtech
        Map["radic"] = '\u221a'; // square root = radical sign, U+221A ISOtech
        Map["prop"] = '\u221d'; // proportional to, U+221D ISOtech
        Map["infin"] = '\u221e'; // infinity, U+221E ISOtech
        Map["ang"] = '\u2220'; // angle, U+2220 ISOamso
        Map["and"] = '\u2227'; // logical and = wedge, U+2227 ISOtech
        Map["or"] = '\u2228'; // logical or = vee, U+2228 ISOtech
        Map["cap"] = '\u2229'; // intersection = cap, U+2229 ISOtech
        Map["cup"] = '\u222a'; // union = cup, U+222A ISOtech
        Map["int"] = '\u222b'; // integral, U+222B ISOtech
        Map["there4"] = '\u2234'; // therefore, U+2234 ISOtech
        Map["sim"] = '\u223c'; // tilde operator = varies with = similar to, U+223C ISOtech
        // tilde operator is NOT the same character as the tilde, U+007E,
        // although the same glyph might be used to represent both
        Map["cong"] = '\u2245'; // approximately equal to, U+2245 ISOtech
        Map["asymp"] = '\u2248'; // almost equal to = asymptotic to, U+2248 ISOamsr
        Map["ne"] = '\u2260'; // not equal to, U+2260 ISOtech
        Map["equiv"] = '\u2261'; // identical to, U+2261 ISOtech
        Map["le"] = '\u2264'; // less-than or equal to, U+2264 ISOtech
        Map["ge"] = '\u2265'; // greater-than or equal to, U+2265 ISOtech
        Map["sub"] = '\u2282'; // subset of, U+2282 ISOtech
        Map["sup"] = '\u2283'; // superset of, U+2283 ISOtech
        // note that nsup, 'not a superset of, U+2283' is not covered by the Symbol
        // font encoding and is not included. Should it be, for symmetry?
        // It is in ISOamsn
        Map["nsub"] = '\u2284'; // not a subset of, U+2284 ISOamsn
        Map["sube"] = '\u2286'; // subset of or equal to, U+2286 ISOtech
        Map["supe"] = '\u2287'; // superset of or equal to, U+2287 ISOtech
        Map["oplus"] = '\u2295'; // circled plus = direct sum, U+2295 ISOamsb
        Map["otimes"] = '\u2297'; // circled times = vector product, U+2297 ISOamsb
        Map["perp"] = '\u22a5'; // up tack = orthogonal to = perpendicular, U+22A5 ISOtech
        Map["sdot"] = '\u22c5'; // dot operator, U+22C5 ISOamsb
        // dot operator is NOT the same character as U+00B7 middle dot
        // Miscellaneous Technical
        Map["lceil"] = '\u2308'; // left ceiling = apl upstile, U+2308 ISOamsc
        Map["rceil"] = '\u2309'; // right ceiling, U+2309 ISOamsc
        Map["lfloor"] = '\u230a'; // left floor = apl downstile, U+230A ISOamsc
        Map["rfloor"] = '\u230b'; // right floor, U+230B ISOamsc
        Map["lang"] = '\u2329'; // left-pointing angle bracket = bra, U+2329 ISOtech
        // lang is NOT the same character as U+003C 'less than'
        // or U+2039 'single left-pointing angle quotation mark'
        Map["rang"] = '\u232a'; // right-pointing angle bracket = ket, U+232A ISOtech
        // rang is NOT the same character as U+003E 'greater than'
        // or U+203A 'single right-pointing angle quotation mark'
        // Geometric Shapes
        Map["loz"] = '\u25ca'; // lozenge, U+25CA ISOpub
        // Miscellaneous Symbols
        Map["spades"] = '\u2660'; // black spade suit, U+2660 ISOpub
        // black here seems to mean filled as opposed to hollow
        Map["clubs"] = '\u2663'; // black club suit = shamrock, U+2663 ISOpub
        Map["hearts"] = '\u2665'; // black heart suit = valentine, U+2665 ISOpub
        Map["diams"] = '\u2666'; // black diamond suit, U+2666 ISOpub
        // C0 Controls and Basic Latin
        Map["quot"] = '\u0022'; // quotation mark = APL quote, U+0022 ISOnum
        Map["amp"] = '\u0026'; // ampersand, U+0026 ISOnum
        Map["apos"] = '\'';
        Map["lt"] = '\u003c'; // less-than sign, U+003C ISOnum
        Map["gt"] = '\u003e'; // greater-than sign, U+003E ISOnum
        // Latin Extended-A
        Map["OElig"] = '\u0152'; // latin capital ligature OE, U+0152 ISOlat2
        Map["oelig"] = '\u0153'; // latin small ligature oe, U+0153 ISOlat2
        // ligature is a misnomer, this is a separate character in some languages
        Map["Scaron"] = '\u0160'; // latin capital letter S with caron, U+0160 ISOlat2
        Map["scaron"] = '\u0161'; // latin small letter s with caron, U+0161 ISOlat2
        Map["Yuml"] = '\u0178'; // latin capital letter Y with diaeresis, U+0178 ISOlat2
        // Spacing Modifier Letters
        Map["circ"] = '\u02c6'; // modifier letter circumflex accent, U+02C6 ISOpub
        Map["tilde"] = '\u02dc'; // small tilde, U+02DC ISOdia
        // General Punctuation
        Map["ensp"] = '\u2002'; // en space, U+2002 ISOpub
        Map["emsp"] = '\u2003'; // em space, U+2003 ISOpub
        Map["thinsp"] = '\u2009'; // thin space, U+2009 ISOpub
        Map["zwnj"] = '\u200c'; // zero width non-joiner, U+200C NEW RFC 2070
        Map["zwj"] = '\u200d'; // zero width joiner, U+200D NEW RFC 2070
        Map["lrm"] = '\u200e'; // left-to-right mark, U+200E NEW RFC 2070
        Map["rlm"] = '\u200f'; // right-to-left mark, U+200F NEW RFC 2070
        Map["ndash"] = '\u2013'; // en dash, U+2013 ISOpub
        Map["mdash"] = '\u2014'; // em dash, U+2014 ISOpub
        Map["lsquo"] = '\u2018'; // left single quotation mark, U+2018 ISOnum
        Map["rsquo"] = '\u2019'; // right single quotation mark, U+2019 ISOnum
        Map["sbquo"] = '\u201a'; // single low-9 quotation mark, U+201A NEW
        Map["ldquo"] = '\u201c'; // left double quotation mark, U+201C ISOnum
        Map["rdquo"] = '\u201d'; // right double quotation mark, U+201D ISOnum
        Map["bdquo"] = '\u201e'; // double low-9 quotation mark, U+201E NEW
        Map["dagger"] = '\u2020'; // dagger, U+2020 ISOpub
        Map["Dagger"] = '\u2021'; // double dagger, U+2021 ISOpub
        Map["permil"] = '\u2030'; // per mille sign, U+2030 ISOtech
        Map["lsaquo"] = '\u2039'; // single left-pointing angle quotation mark, U+2039 ISO proposed
        // lsaquo is proposed but not yet ISO standardized
        Map["rsaquo"] = '\u203a'; // single right-pointing angle quotation mark, U+203A ISO proposed
        // rsaquo is proposed but not yet ISO standardized
        Map["euro"] = '\u20ac'; // euro sign, U+20AC NEW
    }


    /// <summary>
    ///     Translates an entity to a unicode character.
    /// </summary>
    /// <param name="name">the name of the entity</param>
    /// <returns>the corresponding unicode character</returns>
    public static char DecodeEntity(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (name.StartsWith("#x", StringComparison.Ordinal))
        {
            try
            {
                return (char)int.Parse(name.Substring(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            }
            catch
            {
                return '\0';
            }
        }

        if (name.StartsWith("#", StringComparison.Ordinal))
        {
            try
            {
                return (char)int.Parse(name.Substring(1), CultureInfo.InvariantCulture);
            }
            catch
            {
                return '\0';
            }
        }

        object c = Map[name];
        if (c == null)
        {
            return '\0';
        }

        return (char)c;
    }

    /// <summary>
    ///     Translates a String with entities to a String without entities,
    ///     replacing the entity with the right (unicode) character.
    /// </summary>
    public static string DecodeString(string s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        var posAmp = s.IndexOf("&", StringComparison.Ordinal);
        if (posAmp == -1)
        {
            return s;
        }

        int posSc;
        int posA;
        var buf = new StringBuilder(s.Substring(0, posAmp));
        char replace;
        while (true)
        {
            posSc = s.IndexOf(";", posAmp, StringComparison.Ordinal);
            if (posSc == -1)
            {
                buf.Append(s.Substring(posAmp));
                return buf.ToString();
            }

            posA = s.IndexOf("&", posAmp + 1, StringComparison.Ordinal);
            while (posA != -1 && posA < posSc)
            {
                buf.Append(s.Substring(posAmp, posA - posAmp));
                posAmp = posA;
                posA = s.IndexOf("&", posAmp + 1, StringComparison.Ordinal);
            }

            replace = DecodeEntity(s.Substring(posAmp + 1, posSc - (posAmp + 1)));
            if (s.Length < posSc + 1)
            {
                return buf.ToString();
            }

            if (replace == '\0')
            {
                buf.Append(s.Substring(posAmp, posSc + 1 - posAmp));
            }
            else
            {
                buf.Append(replace);
            }

            posAmp = s.IndexOf("&", posSc, StringComparison.Ordinal);
            if (posAmp == -1)
            {
                buf.Append(s.Substring(posSc + 1));
                return buf.ToString();
            }

            buf.Append(s.Substring(posSc + 1, posAmp - (posSc + 1)));
        }
    }
}