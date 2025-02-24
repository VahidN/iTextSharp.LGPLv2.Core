namespace iTextSharp.text.xml.simpleparser;

/// <summary>
///     This class contains entities that can be used in an entity tag.
/// </summary>
public static class EntitiesToSymbol
{
    /// <summary>
    ///     This is a map that contains all possible id values of the entity tag
    ///     that can be translated to a character in font Symbol.
    /// </summary>
    private static readonly Dictionary<string, char> _map;

    static EntitiesToSymbol()
        => _map = new Dictionary<string, char>(StringComparer.Ordinal)
        {
            [key: "169"] = (char)227,
            [key: "172"] = (char)216,
            [key: "174"] = (char)210,
            [key: "177"] = (char)177,
            [key: "215"] = (char)180,
            [key: "247"] = (char)184,
            [key: "8230"] = (char)188,
            [key: "8242"] = (char)162,
            [key: "8243"] = (char)178,
            [key: "8260"] = (char)164,
            [key: "8364"] = (char)240,
            [key: "8465"] = (char)193,
            [key: "8472"] = (char)195,
            [key: "8476"] = (char)194,
            [key: "8482"] = (char)212,
            [key: "8501"] = (char)192,
            [key: "8592"] = (char)172,
            [key: "8593"] = (char)173,
            [key: "8594"] = (char)174,
            [key: "8595"] = (char)175,
            [key: "8596"] = (char)171,
            [key: "8629"] = (char)191,
            [key: "8656"] = (char)220,
            [key: "8657"] = (char)221,
            [key: "8658"] = (char)222,
            [key: "8659"] = (char)223,
            [key: "8660"] = (char)219,
            [key: "8704"] = (char)34,
            [key: "8706"] = (char)182,
            [key: "8707"] = (char)36,
            [key: "8709"] = (char)198,
            [key: "8711"] = (char)209,
            [key: "8712"] = (char)206,
            [key: "8713"] = (char)207,
            [key: "8717"] = (char)39,
            [key: "8719"] = (char)213,
            [key: "8721"] = (char)229,
            [key: "8722"] = (char)45,
            [key: "8727"] = (char)42,
            [key: "8729"] = (char)183,
            [key: "8730"] = (char)214,
            [key: "8733"] = (char)181,
            [key: "8734"] = (char)165,
            [key: "8736"] = (char)208,
            [key: "8743"] = (char)217,
            [key: "8744"] = (char)218,
            [key: "8745"] = (char)199,
            [key: "8746"] = (char)200,
            [key: "8747"] = (char)242,
            [key: "8756"] = (char)92,
            [key: "8764"] = (char)126,
            [key: "8773"] = (char)64,
            [key: "8776"] = (char)187,
            [key: "8800"] = (char)185,
            [key: "8801"] = (char)186,
            [key: "8804"] = (char)163,
            [key: "8805"] = (char)179,
            [key: "8834"] = (char)204,
            [key: "8835"] = (char)201,
            [key: "8836"] = (char)203,
            [key: "8838"] = (char)205,
            [key: "8839"] = (char)202,
            [key: "8853"] = (char)197,
            [key: "8855"] = (char)196,
            [key: "8869"] = (char)94,
            [key: "8901"] = (char)215,
            [key: "8992"] = (char)243,
            [key: "8993"] = (char)245,
            [key: "9001"] = (char)225,
            [key: "9002"] = (char)241,
            [key: "913"] = (char)65,
            [key: "914"] = (char)66,
            [key: "915"] = (char)71,
            [key: "916"] = (char)68,
            [key: "917"] = (char)69,
            [key: "918"] = (char)90,
            [key: "919"] = (char)72,
            [key: "920"] = (char)81,
            [key: "921"] = (char)73,
            [key: "922"] = (char)75,
            [key: "923"] = (char)76,
            [key: "924"] = (char)77,
            [key: "925"] = (char)78,
            [key: "926"] = (char)88,
            [key: "927"] = (char)79,
            [key: "928"] = (char)80,
            [key: "929"] = (char)82,
            [key: "931"] = (char)83,
            [key: "932"] = (char)84,
            [key: "933"] = (char)85,
            [key: "934"] = (char)70,
            [key: "935"] = (char)67,
            [key: "936"] = (char)89,
            [key: "937"] = (char)87,
            [key: "945"] = (char)97,
            [key: "946"] = (char)98,
            [key: "947"] = (char)103,
            [key: "948"] = (char)100,
            [key: "949"] = (char)101,
            [key: "950"] = (char)122,
            [key: "951"] = (char)104,
            [key: "952"] = (char)113,
            [key: "953"] = (char)105,
            [key: "954"] = (char)107,
            [key: "955"] = (char)108,
            [key: "956"] = (char)109,
            [key: "957"] = (char)110,
            [key: "958"] = (char)120,
            [key: "959"] = (char)111,
            [key: "960"] = (char)112,
            [key: "961"] = (char)114,
            [key: "962"] = (char)86,
            [key: "963"] = (char)115,
            [key: "964"] = (char)116,
            [key: "965"] = (char)117,
            [key: "966"] = (char)102,
            [key: "967"] = (char)99,
            [key: "9674"] = (char)224,
            [key: "968"] = (char)121,
            [key: "969"] = (char)119,
            [key: "977"] = (char)74,
            [key: "978"] = (char)161,
            [key: "981"] = (char)106,
            [key: "982"] = (char)118,
            [key: "9824"] = (char)170,
            [key: "9827"] = (char)167,
            [key: "9829"] = (char)169,
            [key: "9830"] = (char)168,
            [key: "Alpha"] = (char)65,
            [key: "Beta"] = (char)66,
            [key: "Chi"] = (char)67,
            [key: "Delta"] = (char)68,
            [key: "Epsilon"] = (char)69,
            [key: "Eta"] = (char)72,
            [key: "Gamma"] = (char)71,
            [key: "Iota"] = (char)73,
            [key: "Kappa"] = (char)75,
            [key: "Lambda"] = (char)76,
            [key: "Mu"] = (char)77,
            [key: "Nu"] = (char)78,
            [key: "Omega"] = (char)87,
            [key: "Omicron"] = (char)79,
            [key: "Phi"] = (char)70,
            [key: "Pi"] = (char)80,
            [key: "Prime"] = (char)178,
            [key: "Psi"] = (char)89,
            [key: "Rho"] = (char)82,
            [key: "Sigma"] = (char)83,
            [key: "Tau"] = (char)84,
            [key: "Theta"] = (char)81,
            [key: "Upsilon"] = (char)85,
            [key: "Xi"] = (char)88,
            [key: "Zeta"] = (char)90,
            [key: "alefsym"] = (char)192,
            [key: "alpha"] = (char)97,
            [key: "and"] = (char)217,
            [key: "ang"] = (char)208,
            [key: "asymp"] = (char)187,
            [key: "beta"] = (char)98,
            [key: "cap"] = (char)199,
            [key: "chi"] = (char)99,
            [key: "clubs"] = (char)167,
            [key: "cong"] = (char)64,
            [key: "copy"] = (char)211,
            [key: "crarr"] = (char)191,
            [key: "cup"] = (char)200,
            [key: "dArr"] = (char)223,
            [key: "darr"] = (char)175,
            [key: "delta"] = (char)100,
            [key: "diams"] = (char)168,
            [key: "divide"] = (char)184,
            [key: "empty"] = (char)198,
            [key: "epsilon"] = (char)101,
            [key: "equiv"] = (char)186,
            [key: "eta"] = (char)104,
            [key: "euro"] = (char)240,
            [key: "exist"] = (char)36,
            [key: "forall"] = (char)34,
            [key: "frasl"] = (char)164,
            [key: "gamma"] = (char)103,
            [key: "ge"] = (char)179,
            [key: "hArr"] = (char)219,
            [key: "harr"] = (char)171,
            [key: "hearts"] = (char)169,
            [key: "hellip"] = (char)188,
            [key: "horizontal arrow extender"] = (char)190,
            [key: "image"] = (char)193,
            [key: "infin"] = (char)165,
            [key: "int"] = (char)242,
            [key: "iota"] = (char)105,
            [key: "isin"] = (char)206,
            [key: "kappa"] = (char)107,
            [key: "lArr"] = (char)220,
            [key: "lambda"] = (char)108,
            [key: "lang"] = (char)225,
            [key: "large brace extender"] = (char)239,
            [key: "large integral extender"] = (char)244,
            [key: "large left brace (bottom)"] = (char)238,
            [key: "large left brace (middle)"] = (char)237,
            [key: "large left brace (top)"] = (char)236,
            [key: "large left bracket (bottom)"] = (char)235,
            [key: "large left bracket (extender)"] = (char)234,
            [key: "large left bracket (top)"] = (char)233,
            [key: "large left parenthesis (bottom)"] = (char)232,
            [key: "large left parenthesis (extender)"] = (char)231,
            [key: "large left parenthesis (top)"] = (char)230,
            [key: "large right brace (bottom)"] = (char)254,
            [key: "large right brace (middle)"] = (char)253,
            [key: "large right brace (top)"] = (char)252,
            [key: "large right bracket (bottom)"] = (char)251,
            [key: "large right bracket (extender)"] = (char)250,
            [key: "large right bracket (top)"] = (char)249,
            [key: "large right parenthesis (bottom)"] = (char)248,
            [key: "large right parenthesis (extender)"] = (char)247,
            [key: "large right parenthesis (top)"] = (char)246,
            [key: "larr"] = (char)172,
            [key: "le"] = (char)163,
            [key: "lowast"] = (char)42,
            [key: "loz"] = (char)224,
            [key: "minus"] = (char)45,
            [key: "mu"] = (char)109,
            [key: "nabla"] = (char)209,
            [key: "ne"] = (char)185,
            [key: "not"] = (char)216,
            [key: "notin"] = (char)207,
            [key: "nsub"] = (char)203,
            [key: "nu"] = (char)110,
            [key: "omega"] = (char)119,
            [key: "omicron"] = (char)111,
            [key: "oplus"] = (char)197,
            [key: "or"] = (char)218,
            [key: "otimes"] = (char)196,
            [key: "part"] = (char)182,
            [key: "perp"] = (char)94,
            [key: "phi"] = (char)102,
            [key: "pi"] = (char)112,
            [key: "piv"] = (char)118,
            [key: "plusmn"] = (char)177,
            [key: "prime"] = (char)162,
            [key: "prod"] = (char)213,
            [key: "prop"] = (char)181,
            [key: "psi"] = (char)121,
            [key: "rArr"] = (char)222,
            [key: "radic"] = (char)214,
            [key: "radical extender"] = (char)96,
            [key: "rang"] = (char)241,
            [key: "rarr"] = (char)174,
            [key: "real"] = (char)194,
            [key: "reg"] = (char)210,
            [key: "rho"] = (char)114,
            [key: "sdot"] = (char)215,
            [key: "sigma"] = (char)115,
            [key: "sigmaf"] = (char)86,
            [key: "sim"] = (char)126,
            [key: "spades"] = (char)170,
            [key: "sub"] = (char)204,
            [key: "sube"] = (char)205,
            [key: "sum"] = (char)229,
            [key: "sup"] = (char)201,
            [key: "supe"] = (char)202,
            [key: "tau"] = (char)116,
            [key: "there4"] = (char)92,
            [key: "theta"] = (char)113,
            [key: "thetasym"] = (char)74,
            [key: "times"] = (char)180,
            [key: "trade"] = (char)212,
            [key: "uArr"] = (char)221,
            [key: "uarr"] = (char)173,
            [key: "upsih"] = (char)161,
            [key: "upsilon"] = (char)117,
            [key: "vertical arrow extender"] = (char)189,
            [key: "weierp"] = (char)195,
            [key: "xi"] = (char)120,
            [key: "zeta"] = (char)122
        };

    /// <summary>
    ///     Gets a chunk with a symbol character.
    /// </summary>
    /// <param name="e">a symbol value (see Entities class: alfa is greek alfa,...)</param>
    /// <param name="font">the font if the symbol isn't found (otherwise Font.SYMBOL)</param>
    /// <returns>a Chunk</returns>
    public static Chunk Get(string e, Font font)
    {
        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        var s = GetCorrespondingSymbol(e);

        if (s == '\0')
        {
            try
            {
                return new Chunk("" + (char)int.Parse(e, CultureInfo.InvariantCulture), font);
            }
            catch (Exception)
            {
                return new Chunk(e, font);
            }
        }

        var symbol = new Font(Font.SYMBOL, font.Size, font.Style, font.Color);

        return new Chunk(s.ToString(), symbol);
    }

    /// <summary>
    ///     Looks for the corresponding symbol in the font Symbol.
    /// </summary>
    /// <param name="name">the name of the entity</param>
    /// <returns>the corresponding character in font Symbol</returns>
    public static char GetCorrespondingSymbol(string name)
    {
        if (_map.TryGetValue(name, out var symbol))
        {
            return symbol;
        }

        return '\0';
    }
}