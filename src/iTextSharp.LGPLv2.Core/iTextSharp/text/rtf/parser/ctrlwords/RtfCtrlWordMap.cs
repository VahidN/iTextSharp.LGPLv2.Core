using System.util;
using iTextSharp.text.rtf.parser.properties;

namespace iTextSharp.text.rtf.parser.ctrlwords;

/// <summary>
///     RtfCtrlWords  handles the creation of the control word wiring.
///     It is a class containing the hash map of the control words (key)
///     and their associated class (value).
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
internal sealed class RtfCtrlWordMap
{
    /// <summary>
    ///     1810 control words in Spec v1.9. might be a few more for other apps that implement
    /// </summary>
    /// <summary>
    ///     additional control words such as exchange, outlook, etc.
    /// </summary>
    /// <summary>
    ///     1810/.9(loadfactor) = 2011.111111...
    /// </summary>
    /// <summary>
    ///     set approximate initial size to initial count / load factor.
    /// </summary>
    /// <summary>
    ///     Hashtable default size is 16. Load Factor .75
    /// </summary>
    /// <summary>
    ///     Control Word Hashtable mapping object.
    /// </summary>
    private readonly NullValueDictionary<string, RtfCtrlWordHandler> _ctrlWords = new();

    /// <summary>
    ///     Constructor
    ///     @since 2.0.8
    /// </summary>
    /// <param name="rtfParser">The parser object.</param>
    public RtfCtrlWordMap(RtfParser rtfParser)
    {
        /*
         * Parameters:
         * RtfParser rtfParser
         * String ctrlWord
         * int defaultParameterValue
         * bool passDefaultParameterValue
         * RtfCtrlWordType ctrlWordType
         * String prefix
         * String suffix
         * String specialHandler =
         *  If TOGGLE then the property name as String
         *  If FLAG then the property name as String
         *  If VALUE then the property name as String
         *  If SYMBOL then the character to use for substitution as String
         *  If DESTINATION|DESTINATION_EX then the RtfDestination class name as String
         */
        //starwriter
        _ctrlWords["aftnnrlc"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnrlc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgdsctbl"] = new RtfCtrlWordHandler(rtfParser, "pgdsctbl", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["pgdsc"] =
            new RtfCtrlWordHandler(rtfParser, "pgdsc", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgdscuse"] =
            new RtfCtrlWordHandler(rtfParser, "pgdscuse", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgdscnxt"] =
            new RtfCtrlWordHandler(rtfParser, "pgdscnxt", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgdscno"] = new RtfCtrlWordHandler(rtfParser, "pgdsctbl", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        //office
        _ctrlWords["'"] = new RtfCtrlWordHandler(rtfParser, "'", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", "'");
        _ctrlWords["*"] = new RtfCtrlWordHandler(rtfParser, "*", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", "*");
        _ctrlWords["-"] = new RtfCtrlWordHandler(rtfParser, "-", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", "-");
        _ctrlWords[":"] = new RtfCtrlWordHandler(rtfParser, ":", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", ":");

        _ctrlWords["ApplyBrkRules"] =
            new RtfCtrlWordHandler(rtfParser, "ApplyBrkRules", 0, false, RtfCtrlWordType.FLAG, "\\", " ",
                null); // "ApplyBrkRules",

        _ctrlWords["\\"] = new RtfCtrlWordHandler(rtfParser, "\\", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", "\\");
        _ctrlWords["_"] = new RtfCtrlWordHandler(rtfParser, "_", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", "_");

        _ctrlWords["ab"] =
            new RtfCtrlWordHandler(rtfParser, "ab", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null); //"ab",

        _ctrlWords["absh"] =
            new RtfCtrlWordHandler(rtfParser, "absh", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null); //"absh",

        _ctrlWords["abslock"] =
            new RtfCtrlWordHandler(rtfParser, "abslock", 0, false, RtfCtrlWordType.FLAG, "", " ", null); //"abslock",

        _ctrlWords["absnoovrlp"] =
            new RtfCtrlWordHandler(rtfParser, "absnoovrlp", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ",
                null); //"absnoovrlp",

        _ctrlWords["absw"] =
            new RtfCtrlWordHandler(rtfParser, "absw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null); //"absw",

        _ctrlWords["acaps"] =
            new RtfCtrlWordHandler(rtfParser, "acaps", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null); //"acaps",

        _ctrlWords["acccircle"] =
            new RtfCtrlWordHandler(rtfParser, "acccircle", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ",
                null); //"acccircle",

        _ctrlWords["acccomma"] =
            new RtfCtrlWordHandler(rtfParser, "acccomma", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["accdot"] =
            new RtfCtrlWordHandler(rtfParser, "accdot", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["accnone"] =
            new RtfCtrlWordHandler(rtfParser, "accnone", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["accunderdot"] =
            new RtfCtrlWordHandler(rtfParser, "accunderdot", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["acf"] = new RtfCtrlWordHandler(rtfParser, "acf", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["additive"] =
            new RtfCtrlWordHandler(rtfParser, "additive", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["adeflang"] =
            new RtfCtrlWordHandler(rtfParser, "adeflang", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["adjustright"] =
            new RtfCtrlWordHandler(rtfParser, "adjustright", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["adn"] = new RtfCtrlWordHandler(rtfParser, "adn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["aenddoc"] =
            new RtfCtrlWordHandler(rtfParser, "aenddoc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aendnotes"] =
            new RtfCtrlWordHandler(rtfParser, "aendnotes", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aexpnd"] =
            new RtfCtrlWordHandler(rtfParser, "aexpnd", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["af"] = new RtfCtrlWordHandler(rtfParser, "af", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["afelev"] =
            new RtfCtrlWordHandler(rtfParser, "afelev", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["affixed"] =
            new RtfCtrlWordHandler(rtfParser, "affixed", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["afs"] = new RtfCtrlWordHandler(rtfParser, "afs", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["aftnbj"] =
            new RtfCtrlWordHandler(rtfParser, "aftnbj", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftncn"] = new RtfCtrlWordHandler(rtfParser, "aftncn", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["aftnnalc"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnalc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnar"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnauc"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnauc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnchi"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnchi", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnchosung"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnchosung", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnncnum"] =
            new RtfCtrlWordHandler(rtfParser, "aftnncnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnndbar"] =
            new RtfCtrlWordHandler(rtfParser, "aftnndbar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnndbnum"] =
            new RtfCtrlWordHandler(rtfParser, "aftnndbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnndbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "aftnndbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnndbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "aftnndbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnndbnumt"] =
            new RtfCtrlWordHandler(rtfParser, "aftnndbnumt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnganada"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnganada", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnngbnum"] =
            new RtfCtrlWordHandler(rtfParser, "aftnngbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnngbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "aftnngbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnngbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "aftnngbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnngbnuml"] =
            new RtfCtrlWordHandler(rtfParser, "aftnngbnuml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnrlc"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnrlc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnruc"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnruc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnzodiac"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnzodiac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnzodiacd"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnzodiacd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnnzodiacl"] =
            new RtfCtrlWordHandler(rtfParser, "aftnnzodiacl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnrestart"] =
            new RtfCtrlWordHandler(rtfParser, "aftnrestart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnrstcont"] =
            new RtfCtrlWordHandler(rtfParser, "aftnrstcont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aftnsep"] = new RtfCtrlWordHandler(rtfParser, "aftnsep", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["aftnsepc"] = new RtfCtrlWordHandler(rtfParser, "aftnsepc", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["aftnstart"] =
            new RtfCtrlWordHandler(rtfParser, "aftnstart", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["aftntj"] =
            new RtfCtrlWordHandler(rtfParser, "aftntj", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ai"] = new RtfCtrlWordHandler(rtfParser, "ai", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["alang"] =
            new RtfCtrlWordHandler(rtfParser, "alang", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["allowfieldendsel"] = new RtfCtrlWordHandler(rtfParser, "allowfieldendsel", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["allprot"] =
            new RtfCtrlWordHandler(rtfParser, "allprot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["alntblind"] =
            new RtfCtrlWordHandler(rtfParser, "alntblind", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["alt"] = new RtfCtrlWordHandler(rtfParser, "alt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["animtext"] =
            new RtfCtrlWordHandler(rtfParser, "animtext", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["annotation"] = new RtfCtrlWordHandler(rtfParser, "annotation", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["annotprot"] =
            new RtfCtrlWordHandler(rtfParser, "annotprot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ansi"] = new RtfCtrlWordHandler(rtfParser, "ansi", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ansicpg"] =
            new RtfCtrlWordHandler(rtfParser, "ansicpg", 1252, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["aoutl"] =
            new RtfCtrlWordHandler(rtfParser, "aoutl", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ascaps"] =
            new RtfCtrlWordHandler(rtfParser, "ascaps", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ashad"] =
            new RtfCtrlWordHandler(rtfParser, "ashad", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["asianbrkrule"] =
            new RtfCtrlWordHandler(rtfParser, "asianbrkrule", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["aspalpha"] =
            new RtfCtrlWordHandler(rtfParser, "aspalpha", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["aspnum"] =
            new RtfCtrlWordHandler(rtfParser, "aspnum", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["astrike"] =
            new RtfCtrlWordHandler(rtfParser, "astrike", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["atnauthor"] = new RtfCtrlWordHandler(rtfParser, "atnauthor", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["atndate"] = new RtfCtrlWordHandler(rtfParser, "atndate", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["atnicn"] = new RtfCtrlWordHandler(rtfParser, "atnicn", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["atnid"] = new RtfCtrlWordHandler(rtfParser, "atnid", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["atnparent"] = new RtfCtrlWordHandler(rtfParser, "atnparent", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["atnref"] = new RtfCtrlWordHandler(rtfParser, "atnref", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["atntime"] = new RtfCtrlWordHandler(rtfParser, "atntime", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["atrfend"] = new RtfCtrlWordHandler(rtfParser, "atrfend", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["atrfstart"] = new RtfCtrlWordHandler(rtfParser, "atrfstart", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["aul"] = new RtfCtrlWordHandler(rtfParser, "aul", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["auld"] =
            new RtfCtrlWordHandler(rtfParser, "auld", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["auldb"] =
            new RtfCtrlWordHandler(rtfParser, "auldb", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["aulnone"] =
            new RtfCtrlWordHandler(rtfParser, "aulnone", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["aulw"] =
            new RtfCtrlWordHandler(rtfParser, "aulw", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["aup"] = new RtfCtrlWordHandler(rtfParser, "aup", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["author"] = new RtfCtrlWordHandler(rtfParser, "author", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationInfo");

        _ctrlWords["autofmtoverride"] = new RtfCtrlWordHandler(rtfParser, "autofmtoverride", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["b"] = new RtfCtrlWordHandler(rtfParser, "b", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ",
            RtfProperty.CHARACTER_BOLD);

        _ctrlWords["background"] = new RtfCtrlWordHandler(rtfParser, "background", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["bdbfhdr"] =
            new RtfCtrlWordHandler(rtfParser, "bdbfhdr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bdrrlswsix"] =
            new RtfCtrlWordHandler(rtfParser, "bdrrlswsix", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "bgbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgcross"] =
            new RtfCtrlWordHandler(rtfParser, "bgcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgdcross"] =
            new RtfCtrlWordHandler(rtfParser, "bgdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgdkbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "bgdkbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgdkcross"] =
            new RtfCtrlWordHandler(rtfParser, "bgdkcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgdkdcross"] =
            new RtfCtrlWordHandler(rtfParser, "bgdkdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgdkfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "bgdkfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgdkhoriz"] =
            new RtfCtrlWordHandler(rtfParser, "bgdkhoriz", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgdkvert"] =
            new RtfCtrlWordHandler(rtfParser, "bgdkvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "bgfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bghoriz"] =
            new RtfCtrlWordHandler(rtfParser, "bghoriz", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bgvert"] =
            new RtfCtrlWordHandler(rtfParser, "bgvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bin"] = new RtfCtrlWordHandler(rtfParser, "bin", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["binfsxn"] =
            new RtfCtrlWordHandler(rtfParser, "binfsxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["binsxn"] =
            new RtfCtrlWordHandler(rtfParser, "binsxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["bkmkcolf"] =
            new RtfCtrlWordHandler(rtfParser, "bkmkcolf", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["bkmkcoll"] =
            new RtfCtrlWordHandler(rtfParser, "bkmkcoll", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["bkmkend"] = new RtfCtrlWordHandler(rtfParser, "bkmkend", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["bkmkpub"] =
            new RtfCtrlWordHandler(rtfParser, "bkmkpub", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bkmkstart"] = new RtfCtrlWordHandler(rtfParser, "bkmkstart", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["bliptag"] =
            new RtfCtrlWordHandler(rtfParser, "bliptag", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["blipuid"] = new RtfCtrlWordHandler(rtfParser, "blipuid", 0, false, RtfCtrlWordType.VALUE, "\\*\\",
            " ", "RtfDestinationShppict"); //"RtfDestinationBlipuid";

        _ctrlWords["blipupi"] = new RtfCtrlWordHandler(rtfParser, "blipupi", 0, true, RtfCtrlWordType.VALUE, "\\", " ",
            "RtfDestinationShppict");

        _ctrlWords["blue"] = new RtfCtrlWordHandler(rtfParser, "blue", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["bookfold"] =
            new RtfCtrlWordHandler(rtfParser, "bookfold", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bookfoldrev"] =
            new RtfCtrlWordHandler(rtfParser, "bookfoldrev", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["bookfoldsheets"] = new RtfCtrlWordHandler(rtfParser, "bookfoldsheets", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["box"] = new RtfCtrlWordHandler(rtfParser, "box", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrart"] =
            new RtfCtrlWordHandler(rtfParser, "brdrart", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["brdrb"] =
            new RtfCtrlWordHandler(rtfParser, "brdrb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrbar"] =
            new RtfCtrlWordHandler(rtfParser, "brdrbar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrbtw"] =
            new RtfCtrlWordHandler(rtfParser, "brdrbtw", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrcf"] =
            new RtfCtrlWordHandler(rtfParser, "brdrcf", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["brdrdash"] =
            new RtfCtrlWordHandler(rtfParser, "brdrdash", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrdashd"] =
            new RtfCtrlWordHandler(rtfParser, "brdrdashd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrdashdd"] =
            new RtfCtrlWordHandler(rtfParser, "brdrdashdd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrdashdotstr"] = new RtfCtrlWordHandler(rtfParser, "brdrdashdotstr", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrdashsm"] =
            new RtfCtrlWordHandler(rtfParser, "brdrdashsm", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrdb"] =
            new RtfCtrlWordHandler(rtfParser, "brdrdb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrdot"] =
            new RtfCtrlWordHandler(rtfParser, "brdrdot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdremboss"] =
            new RtfCtrlWordHandler(rtfParser, "brdremboss", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrengrave"] =
            new RtfCtrlWordHandler(rtfParser, "brdrengrave", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrframe"] =
            new RtfCtrlWordHandler(rtfParser, "brdrframe", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrhair"] =
            new RtfCtrlWordHandler(rtfParser, "brdrhair", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrinset"] =
            new RtfCtrlWordHandler(rtfParser, "brdrinset", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrl"] =
            new RtfCtrlWordHandler(rtfParser, "brdrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrnil"] =
            new RtfCtrlWordHandler(rtfParser, "brdrnil", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrnone"] =
            new RtfCtrlWordHandler(rtfParser, "brdrnone", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["brdroutset"] =
            new RtfCtrlWordHandler(rtfParser, "brdroutset", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrr"] =
            new RtfCtrlWordHandler(rtfParser, "brdrr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrs"] =
            new RtfCtrlWordHandler(rtfParser, "brdrs", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrsh"] =
            new RtfCtrlWordHandler(rtfParser, "brdrsh", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrt"] =
            new RtfCtrlWordHandler(rtfParser, "brdrt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrtbl"] =
            new RtfCtrlWordHandler(rtfParser, "brdrtbl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrth"] =
            new RtfCtrlWordHandler(rtfParser, "brdrth", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrthtnlg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrthtnlg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrthtnmg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrthtnmg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrthtnsg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrthtnsg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrtnthlg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrtnthlg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrtnthmg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrtnthmg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrtnthsg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrtnthsg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrtnthtnlg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrtnthtnlg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrtnthtnmg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrtnthtnmg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrtnthtnsg"] =
            new RtfCtrlWordHandler(rtfParser, "brdrtnthtnsg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrtriple"] =
            new RtfCtrlWordHandler(rtfParser, "brdrtriple", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrw"] =
            new RtfCtrlWordHandler(rtfParser, "brdrw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["brdrwavy"] =
            new RtfCtrlWordHandler(rtfParser, "brdrwavy", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brdrwavydb"] =
            new RtfCtrlWordHandler(rtfParser, "brdrwavydb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brkfrm"] =
            new RtfCtrlWordHandler(rtfParser, "brkfrm", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["brsp"] = new RtfCtrlWordHandler(rtfParser, "brsp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["bullet"] =
            new RtfCtrlWordHandler(rtfParser, "bullet", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", "\0x149");

        _ctrlWords["buptim"] = new RtfCtrlWordHandler(rtfParser, "buptim", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["bxe"] = new RtfCtrlWordHandler(rtfParser, "bxe", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["caccentfive"] =
            new RtfCtrlWordHandler(rtfParser, "caccentfive", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["caccentfour"] =
            new RtfCtrlWordHandler(rtfParser, "caccentfour", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["caccentone"] =
            new RtfCtrlWordHandler(rtfParser, "caccentone", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["caccentsix"] =
            new RtfCtrlWordHandler(rtfParser, "caccentsix", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["caccentthree"] =
            new RtfCtrlWordHandler(rtfParser, "caccentthree", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["caccenttwo"] =
            new RtfCtrlWordHandler(rtfParser, "caccenttwo", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cachedcolbal"] =
            new RtfCtrlWordHandler(rtfParser, "cachedcolbal", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["caps"] =
            new RtfCtrlWordHandler(rtfParser, "caps", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["category"] = new RtfCtrlWordHandler(rtfParser, "category", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["cb"] = new RtfCtrlWordHandler(rtfParser, "cb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cbackgroundone"] = new RtfCtrlWordHandler(rtfParser, "cbackgroundone", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cbackgroundtwo"] = new RtfCtrlWordHandler(rtfParser, "cbackgroundtwo", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cbpat"] =
            new RtfCtrlWordHandler(rtfParser, "cbpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cchs"] = new RtfCtrlWordHandler(rtfParser, "cchs", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cell"] =
            new RtfCtrlWordHandler(rtfParser, "cell", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["cellx"] =
            new RtfCtrlWordHandler(rtfParser, "cellx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cf"] = new RtfCtrlWordHandler(rtfParser, "cf", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cfollowedhyperlink"] = new RtfCtrlWordHandler(rtfParser, "cfollowedhyperlink", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cfpat"] =
            new RtfCtrlWordHandler(rtfParser, "cfpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cgrid"] =
            new RtfCtrlWordHandler(rtfParser, "cgrid", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["charrsid"] =
            new RtfCtrlWordHandler(rtfParser, "charrsid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["charscalex"] =
            new RtfCtrlWordHandler(rtfParser, "charscalex", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["chatn"] =
            new RtfCtrlWordHandler(rtfParser, "chatn", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chbgbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "chbgbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgcross"] =
            new RtfCtrlWordHandler(rtfParser, "chbgcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgdcross"] =
            new RtfCtrlWordHandler(rtfParser, "chbgdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgdkbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "chbgdkbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgdkcross"] =
            new RtfCtrlWordHandler(rtfParser, "chbgdkcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgdkdcross"] =
            new RtfCtrlWordHandler(rtfParser, "chbgdkdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgdkfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "chbgdkfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgdkhoriz"] =
            new RtfCtrlWordHandler(rtfParser, "chbgdkhoriz", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgdkvert"] =
            new RtfCtrlWordHandler(rtfParser, "chbgdkvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "chbgfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbghoriz"] =
            new RtfCtrlWordHandler(rtfParser, "chbghoriz", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbgvert"] =
            new RtfCtrlWordHandler(rtfParser, "chbgvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chbrdr"] =
            new RtfCtrlWordHandler(rtfParser, "chbrdr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["chcbpat"] =
            new RtfCtrlWordHandler(rtfParser, "chcbpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["chcfpat"] =
            new RtfCtrlWordHandler(rtfParser, "chcfpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["chdate"] =
            new RtfCtrlWordHandler(rtfParser, "chdate", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chdpa"] =
            new RtfCtrlWordHandler(rtfParser, "chdpa", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chdpl"] =
            new RtfCtrlWordHandler(rtfParser, "chdpl", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chftn"] =
            new RtfCtrlWordHandler(rtfParser, "chftn", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chftnsep"] =
            new RtfCtrlWordHandler(rtfParser, "chftnsep", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chftnsepc"] =
            new RtfCtrlWordHandler(rtfParser, "chftnsepc", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chpgn"] =
            new RtfCtrlWordHandler(rtfParser, "chpgn", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chshdng"] =
            new RtfCtrlWordHandler(rtfParser, "chshdng", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["chtime"] =
            new RtfCtrlWordHandler(rtfParser, "chtime", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["chyperlink"] =
            new RtfCtrlWordHandler(rtfParser, "chyperlink", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clFitText"] =
            new RtfCtrlWordHandler(rtfParser, "clFitText", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clNoWrap"] =
            new RtfCtrlWordHandler(rtfParser, "clNoWrap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "clbgbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgcross"] =
            new RtfCtrlWordHandler(rtfParser, "clbgcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgdcross"] =
            new RtfCtrlWordHandler(rtfParser, "clbgdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgdkbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "clbgdkbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgdkcross"] =
            new RtfCtrlWordHandler(rtfParser, "clbgdkcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgdkdcross"] =
            new RtfCtrlWordHandler(rtfParser, "clbgdkdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgdkfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "clbgdkfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgdkhor"] =
            new RtfCtrlWordHandler(rtfParser, "clbgdkhor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgdkvert"] =
            new RtfCtrlWordHandler(rtfParser, "clbgdkvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "clbgfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbghoriz"] =
            new RtfCtrlWordHandler(rtfParser, "clbghoriz", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbgvert"] =
            new RtfCtrlWordHandler(rtfParser, "clbgvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbrdrb"] =
            new RtfCtrlWordHandler(rtfParser, "clbrdrb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbrdrl"] =
            new RtfCtrlWordHandler(rtfParser, "clbrdrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbrdrr"] =
            new RtfCtrlWordHandler(rtfParser, "clbrdrr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clbrdrt"] =
            new RtfCtrlWordHandler(rtfParser, "clbrdrt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clcbpat"] =
            new RtfCtrlWordHandler(rtfParser, "clcbpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clcbpatraw"] =
            new RtfCtrlWordHandler(rtfParser, "clcbpatraw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clcfpat"] =
            new RtfCtrlWordHandler(rtfParser, "clcfpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clcfpatraw"] =
            new RtfCtrlWordHandler(rtfParser, "clcfpatraw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cldel"] =
            new RtfCtrlWordHandler(rtfParser, "cldel", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cldelauth"] =
            new RtfCtrlWordHandler(rtfParser, "cldelauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cldeldttm"] =
            new RtfCtrlWordHandler(rtfParser, "cldeldttm", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cldgll"] =
            new RtfCtrlWordHandler(rtfParser, "cldgll", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cldglu"] =
            new RtfCtrlWordHandler(rtfParser, "cldglu", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clftsWidth"] =
            new RtfCtrlWordHandler(rtfParser, "clftsWidth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clhidemark"] =
            new RtfCtrlWordHandler(rtfParser, "clhidemark", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clins"] =
            new RtfCtrlWordHandler(rtfParser, "clins", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clinsauth"] =
            new RtfCtrlWordHandler(rtfParser, "clinsauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clinsdttm"] =
            new RtfCtrlWordHandler(rtfParser, "clinsdttm", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clmgf"] =
            new RtfCtrlWordHandler(rtfParser, "clmgf", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clmrg"] =
            new RtfCtrlWordHandler(rtfParser, "clmrg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clmrgd"] =
            new RtfCtrlWordHandler(rtfParser, "clmrgd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clmrgdauth"] =
            new RtfCtrlWordHandler(rtfParser, "clmrgdauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clmrgddttm"] =
            new RtfCtrlWordHandler(rtfParser, "clmrgddttm", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clmrgdr"] =
            new RtfCtrlWordHandler(rtfParser, "clmrgdr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clpadb"] =
            new RtfCtrlWordHandler(rtfParser, "clpadb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clpadfb"] =
            new RtfCtrlWordHandler(rtfParser, "clpadfb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clpadfl"] =
            new RtfCtrlWordHandler(rtfParser, "clpadfl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clpadfr"] =
            new RtfCtrlWordHandler(rtfParser, "clpadfr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clpadft"] =
            new RtfCtrlWordHandler(rtfParser, "clpadft", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clpadl"] =
            new RtfCtrlWordHandler(rtfParser, "clpadl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clpadr"] =
            new RtfCtrlWordHandler(rtfParser, "clpadr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clpadt"] =
            new RtfCtrlWordHandler(rtfParser, "clpadt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clshdng"] =
            new RtfCtrlWordHandler(rtfParser, "clshdng", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clshdngraw"] =
            new RtfCtrlWordHandler(rtfParser, "clshdngraw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["clshdrawnil"] =
            new RtfCtrlWordHandler(rtfParser, "clshdrawnil", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clsplit"] =
            new RtfCtrlWordHandler(rtfParser, "clsplit", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clsplitr"] =
            new RtfCtrlWordHandler(rtfParser, "clsplitr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cltxbtlr"] =
            new RtfCtrlWordHandler(rtfParser, "cltxbtlr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cltxlrtb"] =
            new RtfCtrlWordHandler(rtfParser, "cltxlrtb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cltxlrtbv"] =
            new RtfCtrlWordHandler(rtfParser, "cltxlrtbv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cltxtbrl"] =
            new RtfCtrlWordHandler(rtfParser, "cltxtbrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cltxtbrlv"] =
            new RtfCtrlWordHandler(rtfParser, "cltxtbrlv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clvertalb"] =
            new RtfCtrlWordHandler(rtfParser, "clvertalb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clvertalc"] =
            new RtfCtrlWordHandler(rtfParser, "clvertalc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clvertalt"] =
            new RtfCtrlWordHandler(rtfParser, "clvertalt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clvmgf"] =
            new RtfCtrlWordHandler(rtfParser, "clvmgf", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clvmrg"] =
            new RtfCtrlWordHandler(rtfParser, "clvmrg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["clwWidth"] =
            new RtfCtrlWordHandler(rtfParser, "clwWidth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cmaindarkone"] =
            new RtfCtrlWordHandler(rtfParser, "cmaindarkone", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cmaindarktwo"] =
            new RtfCtrlWordHandler(rtfParser, "cmaindarktwo", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cmainlightone"] =
            new RtfCtrlWordHandler(rtfParser, "cmainlightone", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cmainlighttwo"] =
            new RtfCtrlWordHandler(rtfParser, "cmainlighttwo", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["collapsed"] =
            new RtfCtrlWordHandler(rtfParser, "collapsed", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["colno"] =
            new RtfCtrlWordHandler(rtfParser, "colno", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["colorschememapping"] = new RtfCtrlWordHandler(rtfParser, "colorschememapping", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", null);

        _ctrlWords["colortbl"] = new RtfCtrlWordHandler(rtfParser, "colortbl", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationColorTable");

        _ctrlWords["cols"] = new RtfCtrlWordHandler(rtfParser, "cols", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["colsr"] =
            new RtfCtrlWordHandler(rtfParser, "colsr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["colsx"] =
            new RtfCtrlWordHandler(rtfParser, "colsx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["column"] =
            new RtfCtrlWordHandler(rtfParser, "column", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["colw"] = new RtfCtrlWordHandler(rtfParser, "colw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["comment"] = new RtfCtrlWordHandler(rtfParser, "comment", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationInfo");

        _ctrlWords["company"] = new RtfCtrlWordHandler(rtfParser, "company", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationInfo");

        _ctrlWords["contextualspace"] = new RtfCtrlWordHandler(rtfParser, "contextualspace", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["cpg"] = new RtfCtrlWordHandler(rtfParser, "cpg", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["crauth"] =
            new RtfCtrlWordHandler(rtfParser, "crauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["crdate"] =
            new RtfCtrlWordHandler(rtfParser, "crdate", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["creatim"] = new RtfCtrlWordHandler(rtfParser, "creatim", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationNull");

        _ctrlWords["cs"] = new RtfCtrlWordHandler(rtfParser, "cs", 0, true, RtfCtrlWordType.VALUE, "\\*\\", " ", null);

        _ctrlWords["cshade"] =
            new RtfCtrlWordHandler(rtfParser, "cshade", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ctextone"] =
            new RtfCtrlWordHandler(rtfParser, "ctextone", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ctexttwo"] =
            new RtfCtrlWordHandler(rtfParser, "ctexttwo", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ctint"] =
            new RtfCtrlWordHandler(rtfParser, "ctint", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ctrl"] = new RtfCtrlWordHandler(rtfParser, "ctrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["cts"] = new RtfCtrlWordHandler(rtfParser, "cts", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["cufi"] = new RtfCtrlWordHandler(rtfParser, "cufi", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["culi"] = new RtfCtrlWordHandler(rtfParser, "culi", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["curi"] = new RtfCtrlWordHandler(rtfParser, "curi", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["cvmme"] =
            new RtfCtrlWordHandler(rtfParser, "cvmme", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["datafield"] = new RtfCtrlWordHandler(rtfParser, "datafield", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["datastore"] = new RtfCtrlWordHandler(rtfParser, "datastore", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["date"] = new RtfCtrlWordHandler(rtfParser, "date", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["dbch"] = new RtfCtrlWordHandler(rtfParser, "dbch", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["defchp"] = new RtfCtrlWordHandler(rtfParser, "defchp", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["deff"] = new RtfCtrlWordHandler(rtfParser, "deff", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["defformat"] =
            new RtfCtrlWordHandler(rtfParser, "defformat", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["deflang"] =
            new RtfCtrlWordHandler(rtfParser, "deflang", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["deflangfe"] =
            new RtfCtrlWordHandler(rtfParser, "deflangfe", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["defpap"] = new RtfCtrlWordHandler(rtfParser, "defpap", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["defshp"] =
            new RtfCtrlWordHandler(rtfParser, "defshp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["deftab"] =
            new RtfCtrlWordHandler(rtfParser, "deftab", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["deleted"] =
            new RtfCtrlWordHandler(rtfParser, "deleted", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["delrsid"] =
            new RtfCtrlWordHandler(rtfParser, "delrsid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dfrauth"] =
            new RtfCtrlWordHandler(rtfParser, "dfrauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dfrdate"] =
            new RtfCtrlWordHandler(rtfParser, "dfrdate", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dfrmtxtx"] =
            new RtfCtrlWordHandler(rtfParser, "dfrmtxtx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dfrmtxty"] =
            new RtfCtrlWordHandler(rtfParser, "dfrmtxty", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dfrstart"] =
            new RtfCtrlWordHandler(rtfParser, "dfrstart", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dfrstop"] =
            new RtfCtrlWordHandler(rtfParser, "dfrstop", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dfrxst"] =
            new RtfCtrlWordHandler(rtfParser, "dfrxst", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dghorigin"] =
            new RtfCtrlWordHandler(rtfParser, "dghorigin", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dghshow"] =
            new RtfCtrlWordHandler(rtfParser, "dghshow", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dghspace"] =
            new RtfCtrlWordHandler(rtfParser, "dghspace", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dgmargin"] =
            new RtfCtrlWordHandler(rtfParser, "dgmargin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dgsnap"] =
            new RtfCtrlWordHandler(rtfParser, "dgsnap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dgvorigin"] =
            new RtfCtrlWordHandler(rtfParser, "dgvorigin", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dgvshow"] =
            new RtfCtrlWordHandler(rtfParser, "dgvshow", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dgvspace"] =
            new RtfCtrlWordHandler(rtfParser, "dgvspace", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dibitmap"] =
            new RtfCtrlWordHandler(rtfParser, "dibitmap", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dn"] = new RtfCtrlWordHandler(rtfParser, "dn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dntblnsbdb"] =
            new RtfCtrlWordHandler(rtfParser, "dntblnsbdb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["do"] = new RtfCtrlWordHandler(rtfParser, "do", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["dobxcolumn"] =
            new RtfCtrlWordHandler(rtfParser, "dobxcolumn", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dobxmargin"] =
            new RtfCtrlWordHandler(rtfParser, "dobxmargin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dobxpage"] =
            new RtfCtrlWordHandler(rtfParser, "dobxpage", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dobymargin"] =
            new RtfCtrlWordHandler(rtfParser, "dobymargin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dobypage"] =
            new RtfCtrlWordHandler(rtfParser, "dobypage", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dobypara"] =
            new RtfCtrlWordHandler(rtfParser, "dobypara", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["doccomm"] = new RtfCtrlWordHandler(rtfParser, "doccomm", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["doctemp"] =
            new RtfCtrlWordHandler(rtfParser, "doctemp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["doctype"] =
            new RtfCtrlWordHandler(rtfParser, "doctype", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["docvar"] = new RtfCtrlWordHandler(rtfParser, "docvar", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["dodhgt"] =
            new RtfCtrlWordHandler(rtfParser, "dodhgt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dolock"] =
            new RtfCtrlWordHandler(rtfParser, "dolock", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["donotembedlingdata"] = new RtfCtrlWordHandler(rtfParser, "donotembedlingdata", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["donotembedsysfont"] = new RtfCtrlWordHandler(rtfParser, "donotembedsysfont", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["donotshowcomments"] = new RtfCtrlWordHandler(rtfParser, "donotshowcomments", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["donotshowinsdel"] = new RtfCtrlWordHandler(rtfParser, "donotshowinsdel", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["donotshowmarkup"] = new RtfCtrlWordHandler(rtfParser, "donotshowmarkup", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["donotshowprops"] = new RtfCtrlWordHandler(rtfParser, "donotshowprops", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpaendhol"] =
            new RtfCtrlWordHandler(rtfParser, "dpaendhol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpaendl"] =
            new RtfCtrlWordHandler(rtfParser, "dpaendl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpaendsol"] =
            new RtfCtrlWordHandler(rtfParser, "dpaendsol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpaendw"] =
            new RtfCtrlWordHandler(rtfParser, "dpaendw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dparc"] =
            new RtfCtrlWordHandler(rtfParser, "dparc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dparcflipx"] =
            new RtfCtrlWordHandler(rtfParser, "dparcflipx", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dparcflipy"] =
            new RtfCtrlWordHandler(rtfParser, "dparcflipy", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpastarthol"] =
            new RtfCtrlWordHandler(rtfParser, "dpastarthol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpastartl"] =
            new RtfCtrlWordHandler(rtfParser, "dpastartl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpastartsol"] =
            new RtfCtrlWordHandler(rtfParser, "dpastartsol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpastartw"] =
            new RtfCtrlWordHandler(rtfParser, "dpastartw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpcallout"] =
            new RtfCtrlWordHandler(rtfParser, "dpcallout", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcoa"] =
            new RtfCtrlWordHandler(rtfParser, "dpcoa", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpcoaccent"] =
            new RtfCtrlWordHandler(rtfParser, "dpcoaccent", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcobestfit"] =
            new RtfCtrlWordHandler(rtfParser, "dpcobestfit", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcoborder"] =
            new RtfCtrlWordHandler(rtfParser, "dpcoborder", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcodabs"] =
            new RtfCtrlWordHandler(rtfParser, "dpcodabs", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpcodbottom"] =
            new RtfCtrlWordHandler(rtfParser, "dpcodbottom", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcodcenter"] =
            new RtfCtrlWordHandler(rtfParser, "dpcodcenter", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcodescent"] =
            new RtfCtrlWordHandler(rtfParser, "dpcodescent", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpcodtop"] =
            new RtfCtrlWordHandler(rtfParser, "dpcodtop", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcolength"] =
            new RtfCtrlWordHandler(rtfParser, "dpcolength", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpcominusx"] =
            new RtfCtrlWordHandler(rtfParser, "dpcominusx", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcominusy"] =
            new RtfCtrlWordHandler(rtfParser, "dpcominusy", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcooffset"] =
            new RtfCtrlWordHandler(rtfParser, "dpcooffset", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpcosmarta"] =
            new RtfCtrlWordHandler(rtfParser, "dpcosmarta", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcotdouble"] =
            new RtfCtrlWordHandler(rtfParser, "dpcotdouble", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcotright"] =
            new RtfCtrlWordHandler(rtfParser, "dpcotright", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcotsingle"] =
            new RtfCtrlWordHandler(rtfParser, "dpcotsingle", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcottriple"] =
            new RtfCtrlWordHandler(rtfParser, "dpcottriple", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpcount"] =
            new RtfCtrlWordHandler(rtfParser, "dpcount", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpellipse"] =
            new RtfCtrlWordHandler(rtfParser, "dpellipse", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpendgroup"] =
            new RtfCtrlWordHandler(rtfParser, "dpendgroup", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpfillbgcb"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillbgcb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpfillbgcg"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillbgcg", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpfillbgcr"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillbgcr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpfillbggray"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillbggray", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpfillbgpal"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillbgpal", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpfillfgcb"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillfgcb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpfillfgcg"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillfgcg", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpfillfgcr"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillfgcr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpfillfggray"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillfggray", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpfillfgpal"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillfgpal", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpfillpat"] =
            new RtfCtrlWordHandler(rtfParser, "dpfillpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpgroup"] =
            new RtfCtrlWordHandler(rtfParser, "dpgroup", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpline"] =
            new RtfCtrlWordHandler(rtfParser, "dpline", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dplinecob"] =
            new RtfCtrlWordHandler(rtfParser, "dplinecob", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dplinecog"] =
            new RtfCtrlWordHandler(rtfParser, "dplinecog", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dplinecor"] =
            new RtfCtrlWordHandler(rtfParser, "dplinecor", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dplinedado"] =
            new RtfCtrlWordHandler(rtfParser, "dplinedado", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dplinedadodo"] =
            new RtfCtrlWordHandler(rtfParser, "dplinedadodo", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dplinedash"] =
            new RtfCtrlWordHandler(rtfParser, "dplinedash", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dplinedot"] =
            new RtfCtrlWordHandler(rtfParser, "dplinedot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dplinegray"] =
            new RtfCtrlWordHandler(rtfParser, "dplinegray", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dplinehollow"] =
            new RtfCtrlWordHandler(rtfParser, "dplinehollow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dplinepal"] =
            new RtfCtrlWordHandler(rtfParser, "dplinepal", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dplinesolid"] =
            new RtfCtrlWordHandler(rtfParser, "dplinesolid", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dplinew"] =
            new RtfCtrlWordHandler(rtfParser, "dplinew", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dppolycount"] =
            new RtfCtrlWordHandler(rtfParser, "dppolycount", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dppolygon"] =
            new RtfCtrlWordHandler(rtfParser, "dppolygon", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dppolyline"] =
            new RtfCtrlWordHandler(rtfParser, "dppolyline", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpptx"] =
            new RtfCtrlWordHandler(rtfParser, "dpptx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dppty"] =
            new RtfCtrlWordHandler(rtfParser, "dppty", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dprect"] =
            new RtfCtrlWordHandler(rtfParser, "dprect", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dproundr"] =
            new RtfCtrlWordHandler(rtfParser, "dproundr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpshadow"] =
            new RtfCtrlWordHandler(rtfParser, "dpshadow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpshadx"] =
            new RtfCtrlWordHandler(rtfParser, "dpshadx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpshady"] =
            new RtfCtrlWordHandler(rtfParser, "dpshady", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dptxbtlr"] =
            new RtfCtrlWordHandler(rtfParser, "dptxbtlr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dptxbx"] =
            new RtfCtrlWordHandler(rtfParser, "dptxbx", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dptxbxmar"] =
            new RtfCtrlWordHandler(rtfParser, "dptxbxmar", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dptxbxtext"] = new RtfCtrlWordHandler(rtfParser, "dptxbxtext", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", null);

        _ctrlWords["dptxlrtb"] =
            new RtfCtrlWordHandler(rtfParser, "dptxlrtb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dptxlrtbv"] =
            new RtfCtrlWordHandler(rtfParser, "dptxlrtbv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dptxtbrl"] =
            new RtfCtrlWordHandler(rtfParser, "dptxtbrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dptxtbrlv"] =
            new RtfCtrlWordHandler(rtfParser, "dptxtbrlv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["dpx"] = new RtfCtrlWordHandler(rtfParser, "dpx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpxsize"] =
            new RtfCtrlWordHandler(rtfParser, "dpxsize", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpy"] = new RtfCtrlWordHandler(rtfParser, "dpy", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dpysize"] =
            new RtfCtrlWordHandler(rtfParser, "dpysize", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dropcapli"] =
            new RtfCtrlWordHandler(rtfParser, "dropcapli", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dropcapt"] =
            new RtfCtrlWordHandler(rtfParser, "dropcapt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ds"] = new RtfCtrlWordHandler(rtfParser, "ds", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dxfrtext"] =
            new RtfCtrlWordHandler(rtfParser, "dxfrtext", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["dy"] = new RtfCtrlWordHandler(rtfParser, "dy", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ebcend"] =
            new RtfCtrlWordHandler(rtfParser, "ebcend", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ", null);

        _ctrlWords["ebcstart"] =
            new RtfCtrlWordHandler(rtfParser, "ebcstart", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ", null);

        _ctrlWords["edmins"] =
            new RtfCtrlWordHandler(rtfParser, "edmins", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["embo"] =
            new RtfCtrlWordHandler(rtfParser, "embo", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["emdash"] =
            new RtfCtrlWordHandler(rtfParser, "emdash", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", "\0x151");

        _ctrlWords["emfblip"] =
            new RtfCtrlWordHandler(rtfParser, "emfblip", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["emspace"] =
            new RtfCtrlWordHandler(rtfParser, "emspace", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["endash"] =
            new RtfCtrlWordHandler(rtfParser, "endash", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", "\0x150");

        _ctrlWords["enddoc"] =
            new RtfCtrlWordHandler(rtfParser, "enddoc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["endnhere"] =
            new RtfCtrlWordHandler(rtfParser, "endnhere", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["endnotes"] =
            new RtfCtrlWordHandler(rtfParser, "endnotes", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["enforceprot"] =
            new RtfCtrlWordHandler(rtfParser, "enforceprot", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["enspace"] =
            new RtfCtrlWordHandler(rtfParser, "enspace", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["expnd"] =
            new RtfCtrlWordHandler(rtfParser, "expnd", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["expndtw"] =
            new RtfCtrlWordHandler(rtfParser, "expndtw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["expshrtn"] =
            new RtfCtrlWordHandler(rtfParser, "expshrtn", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["f"] = new RtfCtrlWordHandler(rtfParser, "f", 0, true, RtfCtrlWordType.VALUE, "\\", " ",
            RtfProperty.CHARACTER_FONT);

        _ctrlWords["faauto"] =
            new RtfCtrlWordHandler(rtfParser, "faauto", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["facenter"] =
            new RtfCtrlWordHandler(rtfParser, "facenter", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["facingp"] =
            new RtfCtrlWordHandler(rtfParser, "facingp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["factoidname"] = new RtfCtrlWordHandler(rtfParser, "factoidname", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", null);

        _ctrlWords["fafixed"] =
            new RtfCtrlWordHandler(rtfParser, "fafixed", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fahang"] =
            new RtfCtrlWordHandler(rtfParser, "fahang", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["falt"] = new RtfCtrlWordHandler(rtfParser, "falt", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationFontTable"); //"RtfDestinationAlternateFont";

        _ctrlWords["faroman"] =
            new RtfCtrlWordHandler(rtfParser, "faroman", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["favar"] =
            new RtfCtrlWordHandler(rtfParser, "favar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fbias"] =
            new RtfCtrlWordHandler(rtfParser, "fbias", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fbidi"] =
            new RtfCtrlWordHandler(rtfParser, "fbidi", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fbimajor"] =
            new RtfCtrlWordHandler(rtfParser, "fbimajor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fbiminor"] =
            new RtfCtrlWordHandler(rtfParser, "fbiminor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fchars"] = new RtfCtrlWordHandler(rtfParser, "fchars", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", null);

        _ctrlWords["fcharset"] =
            new RtfCtrlWordHandler(rtfParser, "fcharset", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fdbmajor"] =
            new RtfCtrlWordHandler(rtfParser, "fdbmajor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fdbminor"] =
            new RtfCtrlWordHandler(rtfParser, "fdbminor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fdecor"] =
            new RtfCtrlWordHandler(rtfParser, "fdecor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["felnbrelev"] =
            new RtfCtrlWordHandler(rtfParser, "felnbrelev", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fet"] = new RtfCtrlWordHandler(rtfParser, "fet", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fetch"] =
            new RtfCtrlWordHandler(rtfParser, "fetch", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ffdefres"] =
            new RtfCtrlWordHandler(rtfParser, "ffdefres", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffdeftext"] = new RtfCtrlWordHandler(rtfParser, "ffdeftext", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["ffentrymcr"] = new RtfCtrlWordHandler(rtfParser, "ffentrymcr", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["ffexitmcr"] = new RtfCtrlWordHandler(rtfParser, "ffexitmcr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["ffformat"] = new RtfCtrlWordHandler(rtfParser, "ffformat", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["ffhaslistbox"] =
            new RtfCtrlWordHandler(rtfParser, "ffhaslistbox", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffhelptext"] = new RtfCtrlWordHandler(rtfParser, "ffhelptext", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["ffhps"] =
            new RtfCtrlWordHandler(rtfParser, "ffhps", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffl"] = new RtfCtrlWordHandler(rtfParser, "ffl", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["ffmaxlen"] =
            new RtfCtrlWordHandler(rtfParser, "ffmaxlen", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffname"] = new RtfCtrlWordHandler(rtfParser, "ffname", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["ffownhelp"] =
            new RtfCtrlWordHandler(rtfParser, "ffownhelp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffownstat"] =
            new RtfCtrlWordHandler(rtfParser, "ffownstat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffprot"] =
            new RtfCtrlWordHandler(rtfParser, "ffprot", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffrecalc"] =
            new RtfCtrlWordHandler(rtfParser, "ffrecalc", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffres"] =
            new RtfCtrlWordHandler(rtfParser, "ffres", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffsize"] =
            new RtfCtrlWordHandler(rtfParser, "ffsize", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ffstattext"] = new RtfCtrlWordHandler(rtfParser, "ffstattext", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["fftype"] =
            new RtfCtrlWordHandler(rtfParser, "fftype", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fftypetxt"] =
            new RtfCtrlWordHandler(rtfParser, "fftypetxt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fhimajor"] =
            new RtfCtrlWordHandler(rtfParser, "fhimajor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fhiminor"] =
            new RtfCtrlWordHandler(rtfParser, "fhiminor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fi"] = new RtfCtrlWordHandler(rtfParser, "fi", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["fid"] = new RtfCtrlWordHandler(rtfParser, "fid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["field"] = new RtfCtrlWordHandler(rtfParser, "field", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["file"] = new RtfCtrlWordHandler(rtfParser, "file", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["filetbl"] = new RtfCtrlWordHandler(rtfParser, "filetbl", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["fittext"] =
            new RtfCtrlWordHandler(rtfParser, "fittext", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fjgothic"] =
            new RtfCtrlWordHandler(rtfParser, "fjgothic", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fjminchou"] =
            new RtfCtrlWordHandler(rtfParser, "fjminchou", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fldalt"] =
            new RtfCtrlWordHandler(rtfParser, "fldalt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["flddirty"] =
            new RtfCtrlWordHandler(rtfParser, "flddirty", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fldedit"] =
            new RtfCtrlWordHandler(rtfParser, "fldedit", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fldinst"] = new RtfCtrlWordHandler(rtfParser, "fldinst", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["fldlock"] =
            new RtfCtrlWordHandler(rtfParser, "fldlock", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fldpriv"] =
            new RtfCtrlWordHandler(rtfParser, "fldpriv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fldrslt"] = new RtfCtrlWordHandler(rtfParser, "fldrslt", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["fldtype"] = new RtfCtrlWordHandler(rtfParser, "fldtype", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["flomajor"] =
            new RtfCtrlWordHandler(rtfParser, "flomajor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["flominor"] =
            new RtfCtrlWordHandler(rtfParser, "flominor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fmodern"] =
            new RtfCtrlWordHandler(rtfParser, "fmodern", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fn"] = new RtfCtrlWordHandler(rtfParser, "fn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fname"] = new RtfCtrlWordHandler(rtfParser, "fname", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["fnetwork"] =
            new RtfCtrlWordHandler(rtfParser, "fnetwork", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fnil"] = new RtfCtrlWordHandler(rtfParser, "fnil", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fnonfilesys"] =
            new RtfCtrlWordHandler(rtfParser, "fnonfilesys", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fontemb"] = new RtfCtrlWordHandler(rtfParser, "fontemb", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["fontfile"] = new RtfCtrlWordHandler(rtfParser, "fontfile", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["fonttbl"] = new RtfCtrlWordHandler(rtfParser, "fonttbl", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationFontTable");

        _ctrlWords["footer"] = new RtfCtrlWordHandler(rtfParser, "footer", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["footerf"] = new RtfCtrlWordHandler(rtfParser, "footerf", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["footerl"] =
            new RtfCtrlWordHandler(rtfParser, "footerl", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ", null);

        _ctrlWords["footerr"] =
            new RtfCtrlWordHandler(rtfParser, "footerr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["footery"] =
            new RtfCtrlWordHandler(rtfParser, "footery", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["footnote"] = new RtfCtrlWordHandler(rtfParser, "footnote", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["forceupgrade"] =
            new RtfCtrlWordHandler(rtfParser, "forceupgrade", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["formdisp"] =
            new RtfCtrlWordHandler(rtfParser, "formdisp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["formfield"] = new RtfCtrlWordHandler(rtfParser, "formfield", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["formprot"] =
            new RtfCtrlWordHandler(rtfParser, "formprot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["formshade"] =
            new RtfCtrlWordHandler(rtfParser, "formshade", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fosnum"] =
            new RtfCtrlWordHandler(rtfParser, "fosnum", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fprq"] = new RtfCtrlWordHandler(rtfParser, "fprq", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fracwidth"] =
            new RtfCtrlWordHandler(rtfParser, "fracwidth", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["frelative"] =
            new RtfCtrlWordHandler(rtfParser, "frelative", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["frmtxbtlr"] =
            new RtfCtrlWordHandler(rtfParser, "frmtxbtlr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["frmtxlrtb"] =
            new RtfCtrlWordHandler(rtfParser, "frmtxlrtb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["frmtxlrtbv"] =
            new RtfCtrlWordHandler(rtfParser, "frmtxlrtbv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["frmtxtbrl"] =
            new RtfCtrlWordHandler(rtfParser, "frmtxtbrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["frmtxtbrlv"] =
            new RtfCtrlWordHandler(rtfParser, "frmtxtbrlv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["froman"] =
            new RtfCtrlWordHandler(rtfParser, "froman", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fromhtml"] =
            new RtfCtrlWordHandler(rtfParser, "fromhtml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fromtext"] =
            new RtfCtrlWordHandler(rtfParser, "fromtext", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fs"] = new RtfCtrlWordHandler(rtfParser, "fs", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["fscript"] =
            new RtfCtrlWordHandler(rtfParser, "fscript", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fswiss"] =
            new RtfCtrlWordHandler(rtfParser, "fswiss", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftech"] =
            new RtfCtrlWordHandler(rtfParser, "ftech", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnalt"] =
            new RtfCtrlWordHandler(rtfParser, "ftnalt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnbj"] =
            new RtfCtrlWordHandler(rtfParser, "ftnbj", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftncn"] = new RtfCtrlWordHandler(rtfParser, "ftncn", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["ftnil"] =
            new RtfCtrlWordHandler(rtfParser, "ftnil", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnlytwnine"] =
            new RtfCtrlWordHandler(rtfParser, "ftnlytwnine", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnalc"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnalc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnar"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnauc"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnauc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnchi"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnchi", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnchosung"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnchosung", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnncnum"] =
            new RtfCtrlWordHandler(rtfParser, "ftnncnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnndbar"] =
            new RtfCtrlWordHandler(rtfParser, "ftnndbar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnndbnum"] =
            new RtfCtrlWordHandler(rtfParser, "ftnndbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnndbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "ftnndbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnndbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "ftnndbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnndbnumt"] =
            new RtfCtrlWordHandler(rtfParser, "ftnndbnumt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnganada"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnganada", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnngbnum"] =
            new RtfCtrlWordHandler(rtfParser, "ftnngbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnngbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "ftnngbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnngbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "ftnngbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnngbnuml"] =
            new RtfCtrlWordHandler(rtfParser, "ftnngbnuml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnrlc"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnrlc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnruc"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnruc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnzodiac"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnzodiac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnzodiacd"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnzodiacd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnnzodiacl"] =
            new RtfCtrlWordHandler(rtfParser, "ftnnzodiacl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnrestart"] =
            new RtfCtrlWordHandler(rtfParser, "ftnrestart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnrstcont"] =
            new RtfCtrlWordHandler(rtfParser, "ftnrstcont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnrstpg"] =
            new RtfCtrlWordHandler(rtfParser, "ftnrstpg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ftnsep"] = new RtfCtrlWordHandler(rtfParser, "ftnsep", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["ftnsepc"] = new RtfCtrlWordHandler(rtfParser, "ftnsepc", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["ftnstart"] =
            new RtfCtrlWordHandler(rtfParser, "ftnstart", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ftntj"] =
            new RtfCtrlWordHandler(rtfParser, "ftntj", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fttruetype"] =
            new RtfCtrlWordHandler(rtfParser, "fttruetype", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fvaliddos"] =
            new RtfCtrlWordHandler(rtfParser, "fvaliddos", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fvalidhpfs"] =
            new RtfCtrlWordHandler(rtfParser, "fvalidhpfs", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fvalidmac"] =
            new RtfCtrlWordHandler(rtfParser, "fvalidmac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["fvalidntfs"] =
            new RtfCtrlWordHandler(rtfParser, "fvalidntfs", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["g"] = new RtfCtrlWordHandler(rtfParser, "g", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["gcw"] = new RtfCtrlWordHandler(rtfParser, "gcw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["generator"] = new RtfCtrlWordHandler(rtfParser, "generator", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationNull");

        _ctrlWords["green"] =
            new RtfCtrlWordHandler(rtfParser, "green", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["gridtbl"] = new RtfCtrlWordHandler(rtfParser, "gridtbl", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["gutter"] =
            new RtfCtrlWordHandler(rtfParser, "gutter", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["gutterprl"] =
            new RtfCtrlWordHandler(rtfParser, "gutterprl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["guttersxn"] =
            new RtfCtrlWordHandler(rtfParser, "guttersxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["header"] = new RtfCtrlWordHandler(rtfParser, "header", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["headerf"] = new RtfCtrlWordHandler(rtfParser, "headerf", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["headerl"] = new RtfCtrlWordHandler(rtfParser, "headerl", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["headerr"] = new RtfCtrlWordHandler(rtfParser, "headerr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["headery"] =
            new RtfCtrlWordHandler(rtfParser, "headery", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hich"] = new RtfCtrlWordHandler(rtfParser, "hich", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["highlight"] =
            new RtfCtrlWordHandler(rtfParser, "highlight", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hlfr"] = new RtfCtrlWordHandler(rtfParser, "hlfr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hlinkbase"] =
            new RtfCtrlWordHandler(rtfParser, "hlinkbase", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hlloc"] =
            new RtfCtrlWordHandler(rtfParser, "hlloc", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hlsrc"] =
            new RtfCtrlWordHandler(rtfParser, "hlsrc", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["horzdoc"] =
            new RtfCtrlWordHandler(rtfParser, "horzdoc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["horzsect"] =
            new RtfCtrlWordHandler(rtfParser, "horzsect", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["horzvert"] =
            new RtfCtrlWordHandler(rtfParser, "horzvert", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hr"] = new RtfCtrlWordHandler(rtfParser, "hr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hsv"] = new RtfCtrlWordHandler(rtfParser, "hsv", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["htmautsp"] =
            new RtfCtrlWordHandler(rtfParser, "htmautsp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["htmlbase"] =
            new RtfCtrlWordHandler(rtfParser, "htmlbase", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["htmlrtf"] =
            new RtfCtrlWordHandler(rtfParser, "htmlrtf", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["htmltag"] = new RtfCtrlWordHandler(rtfParser, "htmltag", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["hwelev"] =
            new RtfCtrlWordHandler(rtfParser, "hwelev", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["hyphauto"] =
            new RtfCtrlWordHandler(rtfParser, "hyphauto", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["hyphcaps"] =
            new RtfCtrlWordHandler(rtfParser, "hyphcaps", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["hyphconsec"] =
            new RtfCtrlWordHandler(rtfParser, "hyphconsec", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hyphhotz"] =
            new RtfCtrlWordHandler(rtfParser, "hyphhotz", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["hyphpar"] =
            new RtfCtrlWordHandler(rtfParser, "hyphpar", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["i"] = new RtfCtrlWordHandler(rtfParser, "i", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);
        _ctrlWords["id"] = new RtfCtrlWordHandler(rtfParser, "id", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ignoremixedcontent"] = new RtfCtrlWordHandler(rtfParser, "ignoremixedcontent", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ilfomacatclnup"] = new RtfCtrlWordHandler(rtfParser, "ilfomacatclnup", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ilvl"] = new RtfCtrlWordHandler(rtfParser, "ilvl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["impr"] =
            new RtfCtrlWordHandler(rtfParser, "impr", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["indmirror"] =
            new RtfCtrlWordHandler(rtfParser, "indmirror", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["indrlsweleven"] =
            new RtfCtrlWordHandler(rtfParser, "indrlsweleven", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["info"] = new RtfCtrlWordHandler(rtfParser, "info", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationInfo");

        _ctrlWords["insrsid"] =
            new RtfCtrlWordHandler(rtfParser, "insrsid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["intbl"] =
            new RtfCtrlWordHandler(rtfParser, "intbl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ipgp"] = new RtfCtrlWordHandler(rtfParser, "ipgp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["irow"] = new RtfCtrlWordHandler(rtfParser, "irow", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["irowband"] =
            new RtfCtrlWordHandler(rtfParser, "irowband", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["itap"] = new RtfCtrlWordHandler(rtfParser, "itap", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["ixe"] = new RtfCtrlWordHandler(rtfParser, "ixe", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["jclisttab"] =
            new RtfCtrlWordHandler(rtfParser, "jclisttab", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["jcompress"] =
            new RtfCtrlWordHandler(rtfParser, "jcompress", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["jexpand"] =
            new RtfCtrlWordHandler(rtfParser, "jexpand", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["jis"] = new RtfCtrlWordHandler(rtfParser, "jis", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["jpegblip"] =
            new RtfCtrlWordHandler(rtfParser, "jpegblip", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["jsksu"] =
            new RtfCtrlWordHandler(rtfParser, "jsksu", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["keep"] = new RtfCtrlWordHandler(rtfParser, "keep", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["keepn"] =
            new RtfCtrlWordHandler(rtfParser, "keepn", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["kerning"] =
            new RtfCtrlWordHandler(rtfParser, "kerning", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["keycode"] = new RtfCtrlWordHandler(rtfParser, "keycode", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["keywords"] = new RtfCtrlWordHandler(rtfParser, "keywords", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationInfo");

        _ctrlWords["krnprsnet"] =
            new RtfCtrlWordHandler(rtfParser, "krnprsnet", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ksulang"] =
            new RtfCtrlWordHandler(rtfParser, "ksulang", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["landscape"] =
            new RtfCtrlWordHandler(rtfParser, "landscape", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lang"] = new RtfCtrlWordHandler(rtfParser, "lang", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["langfe"] =
            new RtfCtrlWordHandler(rtfParser, "langfe", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["langfenp"] =
            new RtfCtrlWordHandler(rtfParser, "langfenp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["langnp"] =
            new RtfCtrlWordHandler(rtfParser, "langnp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lastrow"] =
            new RtfCtrlWordHandler(rtfParser, "lastrow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["latentstyles"] = new RtfCtrlWordHandler(rtfParser, "latentstyles", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["lbr"] = new RtfCtrlWordHandler(rtfParser, "lbr", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["lchars"] = new RtfCtrlWordHandler(rtfParser, "lchars", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["ldblquote"] = new RtfCtrlWordHandler(rtfParser, "ldblquote", 0, false, RtfCtrlWordType.SYMBOL, "\\",
            " ", "\0x147");

        _ctrlWords["level"] =
            new RtfCtrlWordHandler(rtfParser, "level", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelfollow"] =
            new RtfCtrlWordHandler(rtfParser, "levelfollow", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelindent"] =
            new RtfCtrlWordHandler(rtfParser, "levelindent", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["leveljc"] =
            new RtfCtrlWordHandler(rtfParser, "leveljc", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["leveljcn"] =
            new RtfCtrlWordHandler(rtfParser, "leveljcn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levellegal"] =
            new RtfCtrlWordHandler(rtfParser, "levellegal", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelnfc"] =
            new RtfCtrlWordHandler(rtfParser, "levelnfc", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelnfcn"] =
            new RtfCtrlWordHandler(rtfParser, "levelnfcn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelnorestart"] = new RtfCtrlWordHandler(rtfParser, "levelnorestart", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelnumbers"] =
            new RtfCtrlWordHandler(rtfParser, "levelnumbers", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelold"] =
            new RtfCtrlWordHandler(rtfParser, "levelold", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelpicture"] =
            new RtfCtrlWordHandler(rtfParser, "levelpicture", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelprev"] =
            new RtfCtrlWordHandler(rtfParser, "levelprev", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelprevspace"] = new RtfCtrlWordHandler(rtfParser, "levelprevspace", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelspace"] =
            new RtfCtrlWordHandler(rtfParser, "levelspace", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["levelstartat"] =
            new RtfCtrlWordHandler(rtfParser, "levelstartat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["leveltemplateid"] = new RtfCtrlWordHandler(rtfParser, "leveltemplateid", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["leveltext"] =
            new RtfCtrlWordHandler(rtfParser, "leveltext", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["li"] = new RtfCtrlWordHandler(rtfParser, "li", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["lin"] = new RtfCtrlWordHandler(rtfParser, "lin", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["line"] =
            new RtfCtrlWordHandler(rtfParser, "line", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["linebetcol"] =
            new RtfCtrlWordHandler(rtfParser, "linebetcol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["linecont"] =
            new RtfCtrlWordHandler(rtfParser, "linecont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["linemod"] =
            new RtfCtrlWordHandler(rtfParser, "linemod", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lineppage"] =
            new RtfCtrlWordHandler(rtfParser, "lineppage", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["linerestart"] =
            new RtfCtrlWordHandler(rtfParser, "linerestart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["linestart"] =
            new RtfCtrlWordHandler(rtfParser, "linestart", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["linestarts"] =
            new RtfCtrlWordHandler(rtfParser, "linestarts", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["linex"] =
            new RtfCtrlWordHandler(rtfParser, "linex", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["linkself"] =
            new RtfCtrlWordHandler(rtfParser, "linkself", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["linkstyles"] =
            new RtfCtrlWordHandler(rtfParser, "linkstyles", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["linktoquery"] =
            new RtfCtrlWordHandler(rtfParser, "linktoquery", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["linkval"] =
            new RtfCtrlWordHandler(rtfParser, "linkval", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lisa"] = new RtfCtrlWordHandler(rtfParser, "lisa", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["lisb"] = new RtfCtrlWordHandler(rtfParser, "lisb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["list"] =
            new RtfCtrlWordHandler(rtfParser, "list", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listlevel"] =
            new RtfCtrlWordHandler(rtfParser, "listlevel", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listhybrid"] =
            new RtfCtrlWordHandler(rtfParser, "listhybrid", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["listid"] =
            new RtfCtrlWordHandler(rtfParser, "listid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listname"] = new RtfCtrlWordHandler(rtfParser, "listname", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationNull");

        _ctrlWords["listoverride"] = new RtfCtrlWordHandler(rtfParser, "listoverride", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationNull");

        _ctrlWords["listoverridecount"] = new RtfCtrlWordHandler(rtfParser, "listoverridecount", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listoverrideformat"] = new RtfCtrlWordHandler(rtfParser, "listoverrideformat", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listoverridestart"] = new RtfCtrlWordHandler(rtfParser, "listoverridestart", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listoverridestartat"] = new RtfCtrlWordHandler(rtfParser, "listoverridestartat", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["listoverridetable"] = new RtfCtrlWordHandler(rtfParser, "listoverridetable", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationNull");

        _ctrlWords["listpicture"] = new RtfCtrlWordHandler(rtfParser, "listpicture", 0, true,
            RtfCtrlWordType.DESTINATION, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["listrestarthdn"] = new RtfCtrlWordHandler(rtfParser, "listrestarthdn", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listsimple"] =
            new RtfCtrlWordHandler(rtfParser, "listsimple", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["liststyleid"] =
            new RtfCtrlWordHandler(rtfParser, "liststyleid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["liststylename"] =
            new RtfCtrlWordHandler(rtfParser, "liststylename", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listtable"] = new RtfCtrlWordHandler(rtfParser, "listtable", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationListTable");

        _ctrlWords["listtemplateid"] = new RtfCtrlWordHandler(rtfParser, "listtemplateid", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["listtext"] = new RtfCtrlWordHandler(rtfParser, "listtext", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationNull");

        _ctrlWords["lnbrkrule"] =
            new RtfCtrlWordHandler(rtfParser, "lnbrkrule", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lndscpsxn"] =
            new RtfCtrlWordHandler(rtfParser, "lndscpsxn", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lnongrid"] =
            new RtfCtrlWordHandler(rtfParser, "lnongrid", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["loch"] = new RtfCtrlWordHandler(rtfParser, "loch", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lquote"] =
            new RtfCtrlWordHandler(rtfParser, "lquote", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", "\0x145");

        _ctrlWords["ls"] = new RtfCtrlWordHandler(rtfParser, "ls", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdlocked"] =
            new RtfCtrlWordHandler(rtfParser, "lsdlocked", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdlockeddef"] =
            new RtfCtrlWordHandler(rtfParser, "lsdlockeddef", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdlockedexcept"] = new RtfCtrlWordHandler(rtfParser, "lsdlockedexcept", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["lsdpriority"] =
            new RtfCtrlWordHandler(rtfParser, "lsdpriority", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdprioritydef"] = new RtfCtrlWordHandler(rtfParser, "lsdprioritydef", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdqformat"] =
            new RtfCtrlWordHandler(rtfParser, "lsdqformat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdqformatdef"] =
            new RtfCtrlWordHandler(rtfParser, "lsdqformatdef", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdsemihidden"] =
            new RtfCtrlWordHandler(rtfParser, "lsdsemihidden", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdsemihiddendef"] = new RtfCtrlWordHandler(rtfParser, "lsdsemihiddendef", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdstimax"] =
            new RtfCtrlWordHandler(rtfParser, "lsdstimax", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdunhideused"] =
            new RtfCtrlWordHandler(rtfParser, "lsdunhideused", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["lsdunhideuseddef"] = new RtfCtrlWordHandler(rtfParser, "lsdunhideuseddef", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ltrch"] =
            new RtfCtrlWordHandler(rtfParser, "ltrch", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ltrdoc"] =
            new RtfCtrlWordHandler(rtfParser, "ltrdoc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ltrmark"] =
            new RtfCtrlWordHandler(rtfParser, "ltrmark", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["ltrpar"] =
            new RtfCtrlWordHandler(rtfParser, "ltrpar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ltrrow"] =
            new RtfCtrlWordHandler(rtfParser, "ltrrow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ltrsect"] =
            new RtfCtrlWordHandler(rtfParser, "ltrsect", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lvltentative"] =
            new RtfCtrlWordHandler(rtfParser, "lvltentative", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lytcalctblwd"] =
            new RtfCtrlWordHandler(rtfParser, "lytcalctblwd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lytexcttp"] =
            new RtfCtrlWordHandler(rtfParser, "lytexcttp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lytprtmet"] =
            new RtfCtrlWordHandler(rtfParser, "lytprtmet", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["lyttblrtgr"] =
            new RtfCtrlWordHandler(rtfParser, "lyttblrtgr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mac"] = new RtfCtrlWordHandler(rtfParser, "mac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["macc"] = new RtfCtrlWordHandler(rtfParser, "macc", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["maccpr"] = new RtfCtrlWordHandler(rtfParser, "maccpr", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["macpict"] =
            new RtfCtrlWordHandler(rtfParser, "macpict", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mailmerge"] = new RtfCtrlWordHandler(rtfParser, "mailmerge", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["makebackup"] =
            new RtfCtrlWordHandler(rtfParser, "makebackup", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["maln"] = new RtfCtrlWordHandler(rtfParser, "maln", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["malnscr"] = new RtfCtrlWordHandler(rtfParser, "malnscr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["manager"] = new RtfCtrlWordHandler(rtfParser, "manager", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationInfo");

        _ctrlWords["margb"] =
            new RtfCtrlWordHandler(rtfParser, "margb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["margbsxn"] =
            new RtfCtrlWordHandler(rtfParser, "margbsxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["margl"] =
            new RtfCtrlWordHandler(rtfParser, "margl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["marglsxn"] =
            new RtfCtrlWordHandler(rtfParser, "marglsxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["margmirror"] =
            new RtfCtrlWordHandler(rtfParser, "margmirror", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["margmirsxn"] =
            new RtfCtrlWordHandler(rtfParser, "margmirsxn", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["margpr"] = new RtfCtrlWordHandler(rtfParser, "margpr", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["margr"] =
            new RtfCtrlWordHandler(rtfParser, "margr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["margrsxn"] =
            new RtfCtrlWordHandler(rtfParser, "margrsxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["margsz"] =
            new RtfCtrlWordHandler(rtfParser, "margsz", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["margt"] =
            new RtfCtrlWordHandler(rtfParser, "margt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["margtsxn"] =
            new RtfCtrlWordHandler(rtfParser, "margtsxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mbar"] = new RtfCtrlWordHandler(rtfParser, "mbar", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mbarpr"] = new RtfCtrlWordHandler(rtfParser, "mbarpr", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mbasejc"] = new RtfCtrlWordHandler(rtfParser, "mbasejc", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mbegchr"] = new RtfCtrlWordHandler(rtfParser, "mbegchr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mborderbox"] = new RtfCtrlWordHandler(rtfParser, "mborderbox", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mborderboxpr"] = new RtfCtrlWordHandler(rtfParser, "mborderboxpr", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mbox"] = new RtfCtrlWordHandler(rtfParser, "mbox", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mboxpr"] = new RtfCtrlWordHandler(rtfParser, "mboxpr", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mbrk"] = new RtfCtrlWordHandler(rtfParser, "mbrk", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mbrkbin"] =
            new RtfCtrlWordHandler(rtfParser, "mbrkbin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mbrkbinsub"] =
            new RtfCtrlWordHandler(rtfParser, "mbrkbinsub", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mcgp"] = new RtfCtrlWordHandler(rtfParser, "mcgp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mcgprule"] =
            new RtfCtrlWordHandler(rtfParser, "mcgprule", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mchr"] = new RtfCtrlWordHandler(rtfParser, "mchr", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mcount"] = new RtfCtrlWordHandler(rtfParser, "mcount", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mcsp"] = new RtfCtrlWordHandler(rtfParser, "mcsp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mctrlpr"] = new RtfCtrlWordHandler(rtfParser, "mctrlpr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["md"] = new RtfCtrlWordHandler(rtfParser, "md", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mdefjc"] =
            new RtfCtrlWordHandler(rtfParser, "mdefjc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mdeg"] = new RtfCtrlWordHandler(rtfParser, "mdeg", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mdeghide"] = new RtfCtrlWordHandler(rtfParser, "mdeghide", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mden"] = new RtfCtrlWordHandler(rtfParser, "mden", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mdiff"] = new RtfCtrlWordHandler(rtfParser, "mdiff", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mdispdef"] =
            new RtfCtrlWordHandler(rtfParser, "mdispdef", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mdpr"] = new RtfCtrlWordHandler(rtfParser, "mdpr", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["me"] = new RtfCtrlWordHandler(rtfParser, "me", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mendchr"] = new RtfCtrlWordHandler(rtfParser, "mendchr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["meqarr"] = new RtfCtrlWordHandler(rtfParser, "meqarr", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["meqarrpr"] = new RtfCtrlWordHandler(rtfParser, "meqarrpr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mf"] = new RtfCtrlWordHandler(rtfParser, "mf", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mfname"] = new RtfCtrlWordHandler(rtfParser, "mfname", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mfpr"] = new RtfCtrlWordHandler(rtfParser, "mfpr", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mfunc"] = new RtfCtrlWordHandler(rtfParser, "mfunc", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mfuncpr"] = new RtfCtrlWordHandler(rtfParser, "mfuncpr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mgroupchr"] = new RtfCtrlWordHandler(rtfParser, "mgroupchr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mgroupchrpr"] = new RtfCtrlWordHandler(rtfParser, "mgroupchrpr", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mgrow"] = new RtfCtrlWordHandler(rtfParser, "mgrow", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mhidebot"] = new RtfCtrlWordHandler(rtfParser, "mhidebot", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mhideleft"] = new RtfCtrlWordHandler(rtfParser, "mhideleft", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mhideright"] = new RtfCtrlWordHandler(rtfParser, "mhideright", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mhidetop"] = new RtfCtrlWordHandler(rtfParser, "mhidetop", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mhtmltag"] = new RtfCtrlWordHandler(rtfParser, "mhtmltag", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["min"] = new RtfCtrlWordHandler(rtfParser, "min", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mintersp"] =
            new RtfCtrlWordHandler(rtfParser, "mintersp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mintlim"] = new RtfCtrlWordHandler(rtfParser, "mintlim", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mintrasp"] = new RtfCtrlWordHandler(rtfParser, "mintrasp", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mjc"] = new RtfCtrlWordHandler(rtfParser, "mjc", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mlim"] = new RtfCtrlWordHandler(rtfParser, "mlim", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mlimloc"] = new RtfCtrlWordHandler(rtfParser, "mlimloc", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mlimlow"] = new RtfCtrlWordHandler(rtfParser, "mlimlow", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mlimlowpr"] = new RtfCtrlWordHandler(rtfParser, "mlimlowpr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mlimupp"] = new RtfCtrlWordHandler(rtfParser, "mlimupp", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mlimupppr"] = new RtfCtrlWordHandler(rtfParser, "mlimupppr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mlit"] = new RtfCtrlWordHandler(rtfParser, "mlit", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mlmargin"] =
            new RtfCtrlWordHandler(rtfParser, "mlmargin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mm"] = new RtfCtrlWordHandler(rtfParser, "mm", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mmaddfieldname"] = new RtfCtrlWordHandler(rtfParser, "mmaddfieldname", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmath"] = new RtfCtrlWordHandler(rtfParser, "mmath", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mmathfont"] =
            new RtfCtrlWordHandler(rtfParser, "mmathfont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmathpara"] = new RtfCtrlWordHandler(rtfParser, "mmathpara", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmathpict"] = new RtfCtrlWordHandler(rtfParser, "mmathpict", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmathpr"] = new RtfCtrlWordHandler(rtfParser, "mmathpr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmattach"] =
            new RtfCtrlWordHandler(rtfParser, "mmattach", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmaxdist"] = new RtfCtrlWordHandler(rtfParser, "mmaxdist", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmblanklines"] =
            new RtfCtrlWordHandler(rtfParser, "mmblanklines", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmblanklinks"] =
            new RtfCtrlWordHandler(rtfParser, "mmblanklinks", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmc"] = new RtfCtrlWordHandler(rtfParser, "mmc", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mmcjc"] = new RtfCtrlWordHandler(rtfParser, "mmcjc", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mmconnectstrdata"] = new RtfCtrlWordHandler(rtfParser, "mmconnectstrdata", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", null);

        _ctrlWords["mmcpr"] = new RtfCtrlWordHandler(rtfParser, "mmcpr", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mmcs"] = new RtfCtrlWordHandler(rtfParser, "mmcs", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mmdatasource"] = new RtfCtrlWordHandler(rtfParser, "mmdatasource", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmdatatypeaccess"] = new RtfCtrlWordHandler(rtfParser, "mmdatatypeaccess", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdatatypeexcel"] = new RtfCtrlWordHandler(rtfParser, "mmdatatypeexcel", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdatatypefile"] = new RtfCtrlWordHandler(rtfParser, "mmdatatypefile", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdatatypeodbc"] = new RtfCtrlWordHandler(rtfParser, "mmdatatypeodbc", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdatatypeodso"] = new RtfCtrlWordHandler(rtfParser, "mmdatatypeodso", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdatatypeqt"] =
            new RtfCtrlWordHandler(rtfParser, "mmdatatypeqt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdefaultStructuredQueryLanguage"] = new RtfCtrlWordHandler(rtfParser,
            "mmdefaultStructuredQueryLanguage", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdestemail"] =
            new RtfCtrlWordHandler(rtfParser, "mmdestemail", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdestfax"] =
            new RtfCtrlWordHandler(rtfParser, "mmdestfax", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdestnewdoc"] =
            new RtfCtrlWordHandler(rtfParser, "mmdestnewdoc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmdestprinter"] =
            new RtfCtrlWordHandler(rtfParser, "mmdestprinter", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmerrors"] =
            new RtfCtrlWordHandler(rtfParser, "mmerrors", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmfttypeaddress"] = new RtfCtrlWordHandler(rtfParser, "mmfttypeaddress", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmfttypebarcode"] = new RtfCtrlWordHandler(rtfParser, "mmfttypebarcode", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmfttypedbcolumn"] = new RtfCtrlWordHandler(rtfParser, "mmfttypedbcolumn", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmfttypemapped"] = new RtfCtrlWordHandler(rtfParser, "mmfttypemapped", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmfttypenull"] =
            new RtfCtrlWordHandler(rtfParser, "mmfttypenull", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmfttypesalutation"] = new RtfCtrlWordHandler(rtfParser, "mmfttypesalutation", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmheadersource"] = new RtfCtrlWordHandler(rtfParser, "mmheadersource", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmjdsotype"] =
            new RtfCtrlWordHandler(rtfParser, "mmjdsotype", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmlinktoquery"] =
            new RtfCtrlWordHandler(rtfParser, "mmlinktoquery", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmmailsubject"] = new RtfCtrlWordHandler(rtfParser, "mmmailsubject", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmmaintypecatalog"] = new RtfCtrlWordHandler(rtfParser, "mmmaintypecatalog", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmmaintypeemail"] = new RtfCtrlWordHandler(rtfParser, "mmmaintypeemail", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmmaintypeenvelopes"] = new RtfCtrlWordHandler(rtfParser, "mmmaintypeenvelopes", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmmaintypefax"] =
            new RtfCtrlWordHandler(rtfParser, "mmmaintypefax", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmmaintypelabels"] = new RtfCtrlWordHandler(rtfParser, "mmmaintypelabels", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmmaintypeletters"] = new RtfCtrlWordHandler(rtfParser, "mmmaintypeletters", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mmodso"] = new RtfCtrlWordHandler(rtfParser, "mmodso", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsoactive"] =
            new RtfCtrlWordHandler(rtfParser, "mmodsoactive", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmodsocoldelim"] = new RtfCtrlWordHandler(rtfParser, "mmodsocoldelim", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmodsocolumn"] =
            new RtfCtrlWordHandler(rtfParser, "mmodsocolumn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmodsodynaddr"] =
            new RtfCtrlWordHandler(rtfParser, "mmodsodynaddr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmodsofhdr"] =
            new RtfCtrlWordHandler(rtfParser, "mmodsofhdr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmodsofilter"] = new RtfCtrlWordHandler(rtfParser, "mmodsofilter", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsofldmpdata"] = new RtfCtrlWordHandler(rtfParser, "mmodsofldmpdata", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsofmcolumn"] = new RtfCtrlWordHandler(rtfParser, "mmodsofmcolumn", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmodsohash"] =
            new RtfCtrlWordHandler(rtfParser, "mmodsohash", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmodsolid"] =
            new RtfCtrlWordHandler(rtfParser, "mmodsolid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmodsomappedname"] = new RtfCtrlWordHandler(rtfParser, "mmodsomappedname", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsoname"] = new RtfCtrlWordHandler(rtfParser, "mmodsoname", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", null);

        _ctrlWords["mmodsorecipdata"] = new RtfCtrlWordHandler(rtfParser, "mmodsorecipdata", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsosort"] = new RtfCtrlWordHandler(rtfParser, "mmodsosort", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsosrc"] = new RtfCtrlWordHandler(rtfParser, "mmodsosrc", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsotable"] = new RtfCtrlWordHandler(rtfParser, "mmodsotable", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsoudldata"] = new RtfCtrlWordHandler(rtfParser, "mmodsoudldata", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmodsouniquetag"] = new RtfCtrlWordHandler(rtfParser, "mmodsouniquetag", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmpr"] = new RtfCtrlWordHandler(rtfParser, "mmpr", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mmquery"] = new RtfCtrlWordHandler(rtfParser, "mmquery", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mmr"] = new RtfCtrlWordHandler(rtfParser, "mmr", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mmreccur"] =
            new RtfCtrlWordHandler(rtfParser, "mmreccur", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mmshowdata"] =
            new RtfCtrlWordHandler(rtfParser, "mmshowdata", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mnary"] = new RtfCtrlWordHandler(rtfParser, "mnary", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mnarylim"] =
            new RtfCtrlWordHandler(rtfParser, "mnarylim", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mnarypr"] = new RtfCtrlWordHandler(rtfParser, "mnarypr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mnobreak"] = new RtfCtrlWordHandler(rtfParser, "mnobreak", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mnor"] = new RtfCtrlWordHandler(rtfParser, "mnor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mnum"] = new RtfCtrlWordHandler(rtfParser, "mnum", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mo"] = new RtfCtrlWordHandler(rtfParser, "mo", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mobjdist"] = new RtfCtrlWordHandler(rtfParser, "mobjdist", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["momath"] = new RtfCtrlWordHandler(rtfParser, "momath", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["momathpara"] = new RtfCtrlWordHandler(rtfParser, "momathpara", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["momathparapr"] = new RtfCtrlWordHandler(rtfParser, "momathparapr", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mopemu"] = new RtfCtrlWordHandler(rtfParser, "mopemu", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mphant"] = new RtfCtrlWordHandler(rtfParser, "mphant", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mphantpr"] = new RtfCtrlWordHandler(rtfParser, "mphantpr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mplchide"] = new RtfCtrlWordHandler(rtfParser, "mplchide", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mpos"] = new RtfCtrlWordHandler(rtfParser, "mpos", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mpostsp"] =
            new RtfCtrlWordHandler(rtfParser, "mpostsp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mpresp"] =
            new RtfCtrlWordHandler(rtfParser, "mpresp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mr"] = new RtfCtrlWordHandler(rtfParser, "mr", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mrad"] = new RtfCtrlWordHandler(rtfParser, "mrad", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mradpr"] = new RtfCtrlWordHandler(rtfParser, "mradpr", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mrmargin"] =
            new RtfCtrlWordHandler(rtfParser, "mrmargin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mrpr"] = new RtfCtrlWordHandler(rtfParser, "mrpr", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["mrsp"] = new RtfCtrlWordHandler(rtfParser, "mrsp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mrsprule"] =
            new RtfCtrlWordHandler(rtfParser, "mrsprule", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mscr"] = new RtfCtrlWordHandler(rtfParser, "mscr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["msepchr"] = new RtfCtrlWordHandler(rtfParser, "msepchr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mshow"] = new RtfCtrlWordHandler(rtfParser, "mshow", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mshp"] = new RtfCtrlWordHandler(rtfParser, "mshp", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["msize"] = new RtfCtrlWordHandler(rtfParser, "msize", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["msmallfrac"] =
            new RtfCtrlWordHandler(rtfParser, "msmallfrac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["msmcap"] =
            new RtfCtrlWordHandler(rtfParser, "msmcap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mspre"] = new RtfCtrlWordHandler(rtfParser, "mspre", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["msprepr"] = new RtfCtrlWordHandler(rtfParser, "msprepr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mssub"] = new RtfCtrlWordHandler(rtfParser, "mssub", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mssubpr"] = new RtfCtrlWordHandler(rtfParser, "mssubpr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mssubsup"] = new RtfCtrlWordHandler(rtfParser, "mssubsup", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mssubsuppr"] = new RtfCtrlWordHandler(rtfParser, "mssubsuppr", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mssup"] = new RtfCtrlWordHandler(rtfParser, "mssup", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mssuppr"] = new RtfCtrlWordHandler(rtfParser, "mssuppr", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mstrikebltr"] = new RtfCtrlWordHandler(rtfParser, "mstrikebltr", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mstrikeh"] = new RtfCtrlWordHandler(rtfParser, "mstrikeh", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mstriketlbr"] = new RtfCtrlWordHandler(rtfParser, "mstriketlbr", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mstrikev"] = new RtfCtrlWordHandler(rtfParser, "mstrikev", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["msty"] = new RtfCtrlWordHandler(rtfParser, "msty", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["msub"] = new RtfCtrlWordHandler(rtfParser, "msub", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["msubhide"] = new RtfCtrlWordHandler(rtfParser, "msubhide", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["msup"] = new RtfCtrlWordHandler(rtfParser, "msup", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["msuphide"] = new RtfCtrlWordHandler(rtfParser, "msuphide", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mt"] = new RtfCtrlWordHandler(rtfParser, "mt", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mtext"] = new RtfCtrlWordHandler(rtfParser, "mtext", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mtransp"] = new RtfCtrlWordHandler(rtfParser, "mtransp", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mtype"] = new RtfCtrlWordHandler(rtfParser, "mtype", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["mvauth"] =
            new RtfCtrlWordHandler(rtfParser, "mvauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mvdate"] =
            new RtfCtrlWordHandler(rtfParser, "mvdate", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mvertjc"] = new RtfCtrlWordHandler(rtfParser, "mvertjc", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mvf"] = new RtfCtrlWordHandler(rtfParser, "mvf", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mvfmf"] = new RtfCtrlWordHandler(rtfParser, "mvfmf", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mvfml"] = new RtfCtrlWordHandler(rtfParser, "mvfml", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mvt"] = new RtfCtrlWordHandler(rtfParser, "mvt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mvtof"] = new RtfCtrlWordHandler(rtfParser, "mvtof", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mvtol"] = new RtfCtrlWordHandler(rtfParser, "mvtol", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["mwrapindent"] =
            new RtfCtrlWordHandler(rtfParser, "mwrapindent", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mwrapindet"] =
            new RtfCtrlWordHandler(rtfParser, "mwrapindet", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["mwrapright"] =
            new RtfCtrlWordHandler(rtfParser, "mwrapright", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["mzeroasc"] = new RtfCtrlWordHandler(rtfParser, "mzeroasc", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mzerodesc"] = new RtfCtrlWordHandler(rtfParser, "mzerodesc", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["mzerowid"] = new RtfCtrlWordHandler(rtfParser, "mzerowid", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["nestcell"] =
            new RtfCtrlWordHandler(rtfParser, "nestcell", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["nestrow"] =
            new RtfCtrlWordHandler(rtfParser, "nestrow", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["nesttableprops"] = new RtfCtrlWordHandler(rtfParser, "nesttableprops", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["newtblstyruls"] =
            new RtfCtrlWordHandler(rtfParser, "newtblstyruls", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nextfile"] = new RtfCtrlWordHandler(rtfParser, "nextfile", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["noafcnsttbl"] =
            new RtfCtrlWordHandler(rtfParser, "noafcnsttbl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nobrkwrptbl"] =
            new RtfCtrlWordHandler(rtfParser, "nobrkwrptbl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nocolbal"] =
            new RtfCtrlWordHandler(rtfParser, "nocolbal", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nocompatoptions"] = new RtfCtrlWordHandler(rtfParser, "nocompatoptions", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nocwrap"] =
            new RtfCtrlWordHandler(rtfParser, "nocwrap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nocxsptable"] =
            new RtfCtrlWordHandler(rtfParser, "nocxsptable", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["noextrasprl"] =
            new RtfCtrlWordHandler(rtfParser, "noextrasprl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nofchars"] =
            new RtfCtrlWordHandler(rtfParser, "nofchars", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["nofcharsws"] =
            new RtfCtrlWordHandler(rtfParser, "nofcharsws", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["nofeaturethrottle"] = new RtfCtrlWordHandler(rtfParser, "nofeaturethrottle", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nofpages"] =
            new RtfCtrlWordHandler(rtfParser, "nofpages", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["nofwords"] =
            new RtfCtrlWordHandler(rtfParser, "nofwords", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["nogrowautofit"] =
            new RtfCtrlWordHandler(rtfParser, "nogrowautofit", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["noindnmbrts"] =
            new RtfCtrlWordHandler(rtfParser, "noindnmbrts", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nojkernpunct"] =
            new RtfCtrlWordHandler(rtfParser, "nojkernpunct", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nolead"] =
            new RtfCtrlWordHandler(rtfParser, "nolead", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["noline"] =
            new RtfCtrlWordHandler(rtfParser, "noline", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nolnhtadjtbl"] =
            new RtfCtrlWordHandler(rtfParser, "nolnhtadjtbl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nonesttables"] = new RtfCtrlWordHandler(rtfParser, "nonesttables", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["nonshppict"] = new RtfCtrlWordHandler(rtfParser, "nonshppict", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationNull");

        _ctrlWords["nooverflow"] =
            new RtfCtrlWordHandler(rtfParser, "nooverflow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["noproof"] =
            new RtfCtrlWordHandler(rtfParser, "noproof", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["noqfpromote"] =
            new RtfCtrlWordHandler(rtfParser, "noqfpromote", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nosectexpand"] =
            new RtfCtrlWordHandler(rtfParser, "nosectexpand", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nosnaplinegrid"] = new RtfCtrlWordHandler(rtfParser, "nosnaplinegrid", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nospaceforul"] =
            new RtfCtrlWordHandler(rtfParser, "nospaceforul", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nosupersub"] =
            new RtfCtrlWordHandler(rtfParser, "nosupersub", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["notabind"] =
            new RtfCtrlWordHandler(rtfParser, "notabind", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["notbrkcnstfrctbl"] = new RtfCtrlWordHandler(rtfParser, "notbrkcnstfrctbl", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["notcvasp"] =
            new RtfCtrlWordHandler(rtfParser, "notcvasp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["notvatxbx"] =
            new RtfCtrlWordHandler(rtfParser, "notvatxbx", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nouicompat"] =
            new RtfCtrlWordHandler(rtfParser, "nouicompat", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["noultrlspc"] =
            new RtfCtrlWordHandler(rtfParser, "noultrlspc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nowidctlpar"] =
            new RtfCtrlWordHandler(rtfParser, "nowidctlpar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nowrap"] =
            new RtfCtrlWordHandler(rtfParser, "nowrap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["nowwrap"] =
            new RtfCtrlWordHandler(rtfParser, "nowwrap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["noxlattoyen"] =
            new RtfCtrlWordHandler(rtfParser, "noxlattoyen", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objalias"] = new RtfCtrlWordHandler(rtfParser, "objalias", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["objalign"] =
            new RtfCtrlWordHandler(rtfParser, "objalign", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objattph"] =
            new RtfCtrlWordHandler(rtfParser, "objattph", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objautlink"] =
            new RtfCtrlWordHandler(rtfParser, "objautlink", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objclass"] = new RtfCtrlWordHandler(rtfParser, "objclass", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["objcropb"] =
            new RtfCtrlWordHandler(rtfParser, "objcropb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objcropl"] =
            new RtfCtrlWordHandler(rtfParser, "objcropl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objcropr"] =
            new RtfCtrlWordHandler(rtfParser, "objcropr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objcropt"] =
            new RtfCtrlWordHandler(rtfParser, "objcropt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objdata"] = new RtfCtrlWordHandler(rtfParser, "objdata", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["object"] = new RtfCtrlWordHandler(rtfParser, "object", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["objemb"] =
            new RtfCtrlWordHandler(rtfParser, "objemb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objh"] = new RtfCtrlWordHandler(rtfParser, "objh", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objhtml"] =
            new RtfCtrlWordHandler(rtfParser, "objhtml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objicemb"] =
            new RtfCtrlWordHandler(rtfParser, "objicemb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objlink"] =
            new RtfCtrlWordHandler(rtfParser, "objlink", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objlock"] =
            new RtfCtrlWordHandler(rtfParser, "objlock", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objname"] = new RtfCtrlWordHandler(rtfParser, "objname", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["objocx"] =
            new RtfCtrlWordHandler(rtfParser, "objocx", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objpub"] =
            new RtfCtrlWordHandler(rtfParser, "objpub", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objscalex"] =
            new RtfCtrlWordHandler(rtfParser, "objscalex", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objscaley"] =
            new RtfCtrlWordHandler(rtfParser, "objscaley", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objsect"] = new RtfCtrlWordHandler(rtfParser, "objsect", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["objsetsize"] =
            new RtfCtrlWordHandler(rtfParser, "objsetsize", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objsub"] =
            new RtfCtrlWordHandler(rtfParser, "objsub", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objtime"] = new RtfCtrlWordHandler(rtfParser, "objtime", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["objtransy"] =
            new RtfCtrlWordHandler(rtfParser, "objtransy", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["objupdate"] =
            new RtfCtrlWordHandler(rtfParser, "objupdate", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["objw"] = new RtfCtrlWordHandler(rtfParser, "objw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["oldas"] =
            new RtfCtrlWordHandler(rtfParser, "oldas", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["oldcprops"] = new RtfCtrlWordHandler(rtfParser, "oldcprops", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["oldlinewrap"] =
            new RtfCtrlWordHandler(rtfParser, "oldlinewrap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["oldpprops"] = new RtfCtrlWordHandler(rtfParser, "oldpprops", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["oldsprops"] = new RtfCtrlWordHandler(rtfParser, "oldsprops", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["oldtprops"] = new RtfCtrlWordHandler(rtfParser, "oldtprops", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["oleclsid"] = new RtfCtrlWordHandler(rtfParser, "oleclsid", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["operator"] = new RtfCtrlWordHandler(rtfParser, "operator", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationInfo");

        _ctrlWords["otblrul"] =
            new RtfCtrlWordHandler(rtfParser, "otblrul", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["outl"] =
            new RtfCtrlWordHandler(rtfParser, "outl", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["outlinelevel"] =
            new RtfCtrlWordHandler(rtfParser, "outlinelevel", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["overlay"] =
            new RtfCtrlWordHandler(rtfParser, "overlay", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["page"] =
            new RtfCtrlWordHandler(rtfParser, "page", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["pagebb"] =
            new RtfCtrlWordHandler(rtfParser, "pagebb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["panose"] = new RtfCtrlWordHandler(rtfParser, "panose", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationNull");

        _ctrlWords["paperh"] =
            new RtfCtrlWordHandler(rtfParser, "paperh", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["paperw"] =
            new RtfCtrlWordHandler(rtfParser, "paperw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["par"] = new RtfCtrlWordHandler(rtfParser, "par", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", "\n");

        _ctrlWords["pararsid"] =
            new RtfCtrlWordHandler(rtfParser, "pararsid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pard"] = new RtfCtrlWordHandler(rtfParser, "pard", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["passwordhash"] = new RtfCtrlWordHandler(rtfParser, "passwordhash", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["pc"] = new RtfCtrlWordHandler(rtfParser, "pc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["pca"] = new RtfCtrlWordHandler(rtfParser, "pca", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgbrdrb"] =
            new RtfCtrlWordHandler(rtfParser, "pgbrdrb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgbrdrfoot"] =
            new RtfCtrlWordHandler(rtfParser, "pgbrdrfoot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgbrdrhead"] =
            new RtfCtrlWordHandler(rtfParser, "pgbrdrhead", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgbrdrl"] =
            new RtfCtrlWordHandler(rtfParser, "pgbrdrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgbrdropt"] =
            new RtfCtrlWordHandler(rtfParser, "pgbrdropt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgbrdrr"] =
            new RtfCtrlWordHandler(rtfParser, "pgbrdrr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgbrdrsnap"] =
            new RtfCtrlWordHandler(rtfParser, "pgbrdrsnap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgbrdrt"] =
            new RtfCtrlWordHandler(rtfParser, "pgbrdrt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pghsxn"] =
            new RtfCtrlWordHandler(rtfParser, "pghsxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgnbidia"] =
            new RtfCtrlWordHandler(rtfParser, "pgnbidia", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnbidib"] =
            new RtfCtrlWordHandler(rtfParser, "pgnbidib", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnchosung"] =
            new RtfCtrlWordHandler(rtfParser, "pgnchosung", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgncnum"] =
            new RtfCtrlWordHandler(rtfParser, "pgncnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgncont"] =
            new RtfCtrlWordHandler(rtfParser, "pgncont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgndbnum"] =
            new RtfCtrlWordHandler(rtfParser, "pgndbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgndbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "pgndbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgndbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "pgndbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgndbnumt"] =
            new RtfCtrlWordHandler(rtfParser, "pgndbnumt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgndec"] =
            new RtfCtrlWordHandler(rtfParser, "pgndec", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgndecd"] =
            new RtfCtrlWordHandler(rtfParser, "pgndecd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnganada"] =
            new RtfCtrlWordHandler(rtfParser, "pgnganada", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgngbnum"] =
            new RtfCtrlWordHandler(rtfParser, "pgngbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgngbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "pgngbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgngbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "pgngbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgngbnuml"] =
            new RtfCtrlWordHandler(rtfParser, "pgngbnuml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhindia"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhindia", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhindib"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhindib", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhindic"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhindic", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhindid"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhindid", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhn"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgnhnsc"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhnsc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhnsh"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhnsh", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhnsm"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhnsm", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhnsn"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhnsn", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnhnsp"] =
            new RtfCtrlWordHandler(rtfParser, "pgnhnsp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnid"] =
            new RtfCtrlWordHandler(rtfParser, "pgnid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgnlcltr"] =
            new RtfCtrlWordHandler(rtfParser, "pgnlcltr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnlcrm"] =
            new RtfCtrlWordHandler(rtfParser, "pgnlcrm", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnrestart"] =
            new RtfCtrlWordHandler(rtfParser, "pgnrestart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnstart"] =
            new RtfCtrlWordHandler(rtfParser, "pgnstart", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgnstarts"] =
            new RtfCtrlWordHandler(rtfParser, "pgnstarts", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgnthaia"] =
            new RtfCtrlWordHandler(rtfParser, "pgnthaia", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnthaib"] =
            new RtfCtrlWordHandler(rtfParser, "pgnthaib", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnthaic"] =
            new RtfCtrlWordHandler(rtfParser, "pgnthaic", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnucltr"] =
            new RtfCtrlWordHandler(rtfParser, "pgnucltr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnucrm"] =
            new RtfCtrlWordHandler(rtfParser, "pgnucrm", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnvieta"] =
            new RtfCtrlWordHandler(rtfParser, "pgnvieta", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnx"] = new RtfCtrlWordHandler(rtfParser, "pgnx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["pgny"] = new RtfCtrlWordHandler(rtfParser, "pgny", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pgnzodiac"] =
            new RtfCtrlWordHandler(rtfParser, "pgnzodiac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnzodiacd"] =
            new RtfCtrlWordHandler(rtfParser, "pgnzodiacd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgnzodiacl"] =
            new RtfCtrlWordHandler(rtfParser, "pgnzodiacl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pgp"] = new RtfCtrlWordHandler(rtfParser, "pgp", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationNull");

        _ctrlWords["pgptbl"] = new RtfCtrlWordHandler(rtfParser, "pgptbl", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationNull");

        _ctrlWords["pgwsxn"] =
            new RtfCtrlWordHandler(rtfParser, "pgwsxn", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["phcol"] =
            new RtfCtrlWordHandler(rtfParser, "phcol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["phmrg"] =
            new RtfCtrlWordHandler(rtfParser, "phmrg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["phnthaia"] =
            new RtfCtrlWordHandler(rtfParser, "phnthaia", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["phpg"] = new RtfCtrlWordHandler(rtfParser, "phpg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["picbmp"] =
            new RtfCtrlWordHandler(rtfParser, "picbmp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["picbpp"] =
            new RtfCtrlWordHandler(rtfParser, "picbpp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["piccropb"] =
            new RtfCtrlWordHandler(rtfParser, "piccropb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["piccropl"] =
            new RtfCtrlWordHandler(rtfParser, "piccropl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["piccropr"] =
            new RtfCtrlWordHandler(rtfParser, "piccropr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["piccropt"] =
            new RtfCtrlWordHandler(rtfParser, "piccropt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pich"] = new RtfCtrlWordHandler(rtfParser, "pich", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pichgoal"] =
            new RtfCtrlWordHandler(rtfParser, "pichgoal", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["picprop"] = new RtfCtrlWordHandler(rtfParser, "picprop", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationShppict");

        _ctrlWords["picscaled"] =
            new RtfCtrlWordHandler(rtfParser, "picscaled", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["picscalex"] =
            new RtfCtrlWordHandler(rtfParser, "picscalex", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["picscaley"] =
            new RtfCtrlWordHandler(rtfParser, "picscaley", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pict"] = new RtfCtrlWordHandler(rtfParser, "pict", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationShppict");

        _ctrlWords["picw"] = new RtfCtrlWordHandler(rtfParser, "picw", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["picwgoal"] =
            new RtfCtrlWordHandler(rtfParser, "picwgoal", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pindtabqc"] =
            new RtfCtrlWordHandler(rtfParser, "pindtabqc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pindtabql"] =
            new RtfCtrlWordHandler(rtfParser, "pindtabql", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pindtabqr"] =
            new RtfCtrlWordHandler(rtfParser, "pindtabqr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["plain"] =
            new RtfCtrlWordHandler(rtfParser, "plain", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pmartabqc"] =
            new RtfCtrlWordHandler(rtfParser, "pmartabqc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pmartabql"] =
            new RtfCtrlWordHandler(rtfParser, "pmartabql", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pmartabqr"] =
            new RtfCtrlWordHandler(rtfParser, "pmartabqr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pmmetafile"] =
            new RtfCtrlWordHandler(rtfParser, "pmmetafile", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pn"] = new RtfCtrlWordHandler(rtfParser, "pn", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["pnacross"] =
            new RtfCtrlWordHandler(rtfParser, "pnacross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnaiu"] =
            new RtfCtrlWordHandler(rtfParser, "pnaiu", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnaiud"] =
            new RtfCtrlWordHandler(rtfParser, "pnaiud", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnaiueo"] =
            new RtfCtrlWordHandler(rtfParser, "pnaiueo", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnaiueod"] =
            new RtfCtrlWordHandler(rtfParser, "pnaiueod", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnb"] = new RtfCtrlWordHandler(rtfParser, "pnb", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["pnbidia"] =
            new RtfCtrlWordHandler(rtfParser, "pnbidia", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnbidib"] =
            new RtfCtrlWordHandler(rtfParser, "pnbidib", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pncaps"] =
            new RtfCtrlWordHandler(rtfParser, "pncaps", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["pncard"] =
            new RtfCtrlWordHandler(rtfParser, "pncard", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pncf"] = new RtfCtrlWordHandler(rtfParser, "pncf", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnchosung"] =
            new RtfCtrlWordHandler(rtfParser, "pnchosung", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pncnum"] =
            new RtfCtrlWordHandler(rtfParser, "pncnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pndbnum"] =
            new RtfCtrlWordHandler(rtfParser, "pndbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pndbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "pndbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pndbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "pndbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pndbnuml"] =
            new RtfCtrlWordHandler(rtfParser, "pndbnuml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pndbnumt"] =
            new RtfCtrlWordHandler(rtfParser, "pndbnumt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pndec"] =
            new RtfCtrlWordHandler(rtfParser, "pndec", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pndecd"] =
            new RtfCtrlWordHandler(rtfParser, "pndecd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnf"] = new RtfCtrlWordHandler(rtfParser, "pnf", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["pnfs"] = new RtfCtrlWordHandler(rtfParser, "pnfs", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnganada"] =
            new RtfCtrlWordHandler(rtfParser, "pnganada", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pngblip"] =
            new RtfCtrlWordHandler(rtfParser, "pngblip", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pngbnum"] =
            new RtfCtrlWordHandler(rtfParser, "pngbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pngbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "pngbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pngbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "pngbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pngbnuml"] =
            new RtfCtrlWordHandler(rtfParser, "pngbnuml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnhang"] =
            new RtfCtrlWordHandler(rtfParser, "pnhang", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pni"] = new RtfCtrlWordHandler(rtfParser, "pni", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["pnindent"] =
            new RtfCtrlWordHandler(rtfParser, "pnindent", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pniroha"] =
            new RtfCtrlWordHandler(rtfParser, "pniroha", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnirohad"] =
            new RtfCtrlWordHandler(rtfParser, "pnirohad", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnlcltr"] =
            new RtfCtrlWordHandler(rtfParser, "pnlcltr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnlcrm"] =
            new RtfCtrlWordHandler(rtfParser, "pnlcrm", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnlvl"] =
            new RtfCtrlWordHandler(rtfParser, "pnlvl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnlvlblt"] =
            new RtfCtrlWordHandler(rtfParser, "pnlvlblt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnlvlbody"] =
            new RtfCtrlWordHandler(rtfParser, "pnlvlbody", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnlvlcont"] =
            new RtfCtrlWordHandler(rtfParser, "pnlvlcont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnnumonce"] =
            new RtfCtrlWordHandler(rtfParser, "pnnumonce", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnord"] =
            new RtfCtrlWordHandler(rtfParser, "pnord", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnordt"] =
            new RtfCtrlWordHandler(rtfParser, "pnordt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnprev"] =
            new RtfCtrlWordHandler(rtfParser, "pnprev", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnqc"] = new RtfCtrlWordHandler(rtfParser, "pnqc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["pnql"] = new RtfCtrlWordHandler(rtfParser, "pnql", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["pnqr"] = new RtfCtrlWordHandler(rtfParser, "pnqr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnrauth"] =
            new RtfCtrlWordHandler(rtfParser, "pnrauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnrdate"] =
            new RtfCtrlWordHandler(rtfParser, "pnrdate", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnrestart"] =
            new RtfCtrlWordHandler(rtfParser, "pnrestart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnrnfc"] =
            new RtfCtrlWordHandler(rtfParser, "pnrnfc", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnrnot"] =
            new RtfCtrlWordHandler(rtfParser, "pnrnot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnrpnbr"] =
            new RtfCtrlWordHandler(rtfParser, "pnrpnbr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnrrgb"] =
            new RtfCtrlWordHandler(rtfParser, "pnrrgb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnrstart"] =
            new RtfCtrlWordHandler(rtfParser, "pnrstart", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnrstop"] =
            new RtfCtrlWordHandler(rtfParser, "pnrstop", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnrxst"] =
            new RtfCtrlWordHandler(rtfParser, "pnrxst", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnscaps"] =
            new RtfCtrlWordHandler(rtfParser, "pnscaps", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["pnseclvl"] = new RtfCtrlWordHandler(rtfParser, "pnseclvl", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["pnsp"] = new RtfCtrlWordHandler(rtfParser, "pnsp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnstart"] =
            new RtfCtrlWordHandler(rtfParser, "pnstart", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["pnstrike"] =
            new RtfCtrlWordHandler(rtfParser, "pnstrike", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["pntext"] = new RtfCtrlWordHandler(rtfParser, "pntext", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["pntxta"] = new RtfCtrlWordHandler(rtfParser, "pntxta", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["pntxtb"] = new RtfCtrlWordHandler(rtfParser, "pntxtb", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["pnucltr"] =
            new RtfCtrlWordHandler(rtfParser, "pnucltr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnucrm"] =
            new RtfCtrlWordHandler(rtfParser, "pnucrm", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnul"] =
            new RtfCtrlWordHandler(rtfParser, "pnul", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["pnuld"] =
            new RtfCtrlWordHandler(rtfParser, "pnuld", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnuldash"] =
            new RtfCtrlWordHandler(rtfParser, "pnuldash", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnuldashd"] =
            new RtfCtrlWordHandler(rtfParser, "pnuldashd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnuldashdd"] =
            new RtfCtrlWordHandler(rtfParser, "pnuldashdd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnuldb"] =
            new RtfCtrlWordHandler(rtfParser, "pnuldb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnulhair"] =
            new RtfCtrlWordHandler(rtfParser, "pnulhair", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnulnone"] =
            new RtfCtrlWordHandler(rtfParser, "pnulnone", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnulth"] =
            new RtfCtrlWordHandler(rtfParser, "pnulth", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnulw"] =
            new RtfCtrlWordHandler(rtfParser, "pnulw", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnulwave"] =
            new RtfCtrlWordHandler(rtfParser, "pnulwave", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnzodiac"] =
            new RtfCtrlWordHandler(rtfParser, "pnzodiac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnzodiacd"] =
            new RtfCtrlWordHandler(rtfParser, "pnzodiacd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pnzodiacl"] =
            new RtfCtrlWordHandler(rtfParser, "pnzodiacl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posnegx"] =
            new RtfCtrlWordHandler(rtfParser, "posnegx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["posnegy"] =
            new RtfCtrlWordHandler(rtfParser, "posnegy", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["posx"] = new RtfCtrlWordHandler(rtfParser, "posx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["posxc"] =
            new RtfCtrlWordHandler(rtfParser, "posxc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posxi"] =
            new RtfCtrlWordHandler(rtfParser, "posxi", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posxl"] =
            new RtfCtrlWordHandler(rtfParser, "posxl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posxo"] =
            new RtfCtrlWordHandler(rtfParser, "posxo", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posxr"] =
            new RtfCtrlWordHandler(rtfParser, "posxr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posy"] = new RtfCtrlWordHandler(rtfParser, "posy", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["posyb"] =
            new RtfCtrlWordHandler(rtfParser, "posyb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posyc"] =
            new RtfCtrlWordHandler(rtfParser, "posyc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posyil"] =
            new RtfCtrlWordHandler(rtfParser, "posyil", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posyin"] =
            new RtfCtrlWordHandler(rtfParser, "posyin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posyout"] =
            new RtfCtrlWordHandler(rtfParser, "posyout", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["posyt"] =
            new RtfCtrlWordHandler(rtfParser, "posyt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["prcolbl"] =
            new RtfCtrlWordHandler(rtfParser, "prcolbl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["printdata"] =
            new RtfCtrlWordHandler(rtfParser, "printdata", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["printim"] = new RtfCtrlWordHandler(rtfParser, "printim", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationNull");

        _ctrlWords["private"] = new RtfCtrlWordHandler(rtfParser, "private", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["propname"] =
            new RtfCtrlWordHandler(rtfParser, "propname", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["proptype"] =
            new RtfCtrlWordHandler(rtfParser, "proptype", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["protend"] = new RtfCtrlWordHandler(rtfParser, "protend", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["protlevel"] =
            new RtfCtrlWordHandler(rtfParser, "protlevel", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["protstart"] = new RtfCtrlWordHandler(rtfParser, "protstart", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["protusertbl"] = new RtfCtrlWordHandler(rtfParser, "protusertbl", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["psover"] =
            new RtfCtrlWordHandler(rtfParser, "psover", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["psz"] = new RtfCtrlWordHandler(rtfParser, "psz", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ptabldot"] =
            new RtfCtrlWordHandler(rtfParser, "ptabldot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ptablmdot"] =
            new RtfCtrlWordHandler(rtfParser, "ptablmdot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ptablminus"] =
            new RtfCtrlWordHandler(rtfParser, "ptablminus", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ptablnone"] =
            new RtfCtrlWordHandler(rtfParser, "ptablnone", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ptabluscore"] =
            new RtfCtrlWordHandler(rtfParser, "ptabluscore", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pubauto"] =
            new RtfCtrlWordHandler(rtfParser, "pubauto", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pvmrg"] =
            new RtfCtrlWordHandler(rtfParser, "pvmrg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pvpara"] =
            new RtfCtrlWordHandler(rtfParser, "pvpara", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pvpg"] = new RtfCtrlWordHandler(rtfParser, "pvpg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["pwd"] = new RtfCtrlWordHandler(rtfParser, "pwd", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["pxe"] = new RtfCtrlWordHandler(rtfParser, "pxe", 0, false, RtfCtrlWordType.DESTINATION_EX, "\\*\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["qc"] = new RtfCtrlWordHandler(rtfParser, "qc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["qd"] = new RtfCtrlWordHandler(rtfParser, "qd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["qj"] = new RtfCtrlWordHandler(rtfParser, "qj", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["qk"] = new RtfCtrlWordHandler(rtfParser, "qk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["ql"] = new RtfCtrlWordHandler(rtfParser, "ql", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["qmspace"] =
            new RtfCtrlWordHandler(rtfParser, "qmspace", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["qr"] = new RtfCtrlWordHandler(rtfParser, "qr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["qt"] = new RtfCtrlWordHandler(rtfParser, "qt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawbgdkbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "rawbgdkbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "rawclbgbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgcross"] =
            new RtfCtrlWordHandler(rtfParser, "rawclbgcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgdcross"] =
            new RtfCtrlWordHandler(rtfParser, "rawclbgdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgdkbdiag"] = new RtfCtrlWordHandler(rtfParser, "rawclbgdkbdiag", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgdkcross"] = new RtfCtrlWordHandler(rtfParser, "rawclbgdkcross", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgdkdcross"] = new RtfCtrlWordHandler(rtfParser, "rawclbgdkdcross", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgdkfdiag"] = new RtfCtrlWordHandler(rtfParser, "rawclbgdkfdiag", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgdkhor"] =
            new RtfCtrlWordHandler(rtfParser, "rawclbgdkhor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgdkvert"] =
            new RtfCtrlWordHandler(rtfParser, "rawclbgdkvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "rawclbgfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbghoriz"] =
            new RtfCtrlWordHandler(rtfParser, "rawclbghoriz", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rawclbgvert"] =
            new RtfCtrlWordHandler(rtfParser, "rawclbgvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rdblquote"] = new RtfCtrlWordHandler(rtfParser, "rdblquote", 0, false, RtfCtrlWordType.SYMBOL, "\\",
            " ", "\0x148");

        _ctrlWords["readonlyrecommended"] = new RtfCtrlWordHandler(rtfParser, "readonlyrecommended", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["readprot"] =
            new RtfCtrlWordHandler(rtfParser, "readprot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["red"] = new RtfCtrlWordHandler(rtfParser, "red", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["relyonvml"] =
            new RtfCtrlWordHandler(rtfParser, "relyonvml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rempersonalinfo"] = new RtfCtrlWordHandler(rtfParser, "rempersonalinfo", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["result"] = new RtfCtrlWordHandler(rtfParser, "result", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["revauth"] =
            new RtfCtrlWordHandler(rtfParser, "revauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["revauthdel"] =
            new RtfCtrlWordHandler(rtfParser, "revauthdel", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["revbar"] =
            new RtfCtrlWordHandler(rtfParser, "revbar", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["revdttm"] =
            new RtfCtrlWordHandler(rtfParser, "revdttm", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["revdttmdel"] =
            new RtfCtrlWordHandler(rtfParser, "revdttmdel", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["revised"] =
            new RtfCtrlWordHandler(rtfParser, "revised", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["revisions"] =
            new RtfCtrlWordHandler(rtfParser, "revisions", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["revprop"] =
            new RtfCtrlWordHandler(rtfParser, "revprop", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["revprot"] =
            new RtfCtrlWordHandler(rtfParser, "revprot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["revtbl"] = new RtfCtrlWordHandler(rtfParser, "revtbl", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationNull");

        _ctrlWords["revtim"] = new RtfCtrlWordHandler(rtfParser, "revtim", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationNull");

        _ctrlWords["ri"] = new RtfCtrlWordHandler(rtfParser, "ri", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["rin"] = new RtfCtrlWordHandler(rtfParser, "rin", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["row"] = new RtfCtrlWordHandler(rtfParser, "row", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["rquote"] =
            new RtfCtrlWordHandler(rtfParser, "rquote", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", "\0x146");

        _ctrlWords["rsid"] = new RtfCtrlWordHandler(rtfParser, "rsid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["rsidroot"] =
            new RtfCtrlWordHandler(rtfParser, "rsidroot", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["rsidtbl"] = new RtfCtrlWordHandler(rtfParser, "rsidtbl", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationNull");

        _ctrlWords["rsltbmp"] =
            new RtfCtrlWordHandler(rtfParser, "rsltbmp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rslthtml"] =
            new RtfCtrlWordHandler(rtfParser, "rslthtml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rsltmerge"] =
            new RtfCtrlWordHandler(rtfParser, "rsltmerge", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rsltpict"] =
            new RtfCtrlWordHandler(rtfParser, "rsltpict", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rsltrtf"] =
            new RtfCtrlWordHandler(rtfParser, "rsltrtf", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rslttxt"] =
            new RtfCtrlWordHandler(rtfParser, "rslttxt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rtf"] = new RtfCtrlWordHandler(rtfParser, "rtf", 1, true, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["rtlch"] =
            new RtfCtrlWordHandler(rtfParser, "rtlch", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rtldoc"] =
            new RtfCtrlWordHandler(rtfParser, "rtldoc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rtlgutter"] =
            new RtfCtrlWordHandler(rtfParser, "rtlgutter", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rtlmark"] =
            new RtfCtrlWordHandler(rtfParser, "rtlmark", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["rtlpar"] =
            new RtfCtrlWordHandler(rtfParser, "rtlpar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rtlrow"] =
            new RtfCtrlWordHandler(rtfParser, "rtlrow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rtlsect"] =
            new RtfCtrlWordHandler(rtfParser, "rtlsect", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["rxe"] = new RtfCtrlWordHandler(rtfParser, "rxe", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["s"] = new RtfCtrlWordHandler(rtfParser, "s", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["sa"] = new RtfCtrlWordHandler(rtfParser, "sa", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["saauto"] =
            new RtfCtrlWordHandler(rtfParser, "saauto", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["saftnnalc"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnalc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnar"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnauc"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnauc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnchi"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnchi", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnchosung"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnchosung", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnncnum"] =
            new RtfCtrlWordHandler(rtfParser, "saftnncnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnndbar"] =
            new RtfCtrlWordHandler(rtfParser, "saftnndbar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnndbnum"] =
            new RtfCtrlWordHandler(rtfParser, "saftnndbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnndbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "saftnndbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnndbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "saftnndbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnndbnumt"] =
            new RtfCtrlWordHandler(rtfParser, "saftnndbnumt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnganada"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnganada", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnngbnum"] =
            new RtfCtrlWordHandler(rtfParser, "saftnngbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnngbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "saftnngbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnngbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "saftnngbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnngbnuml"] =
            new RtfCtrlWordHandler(rtfParser, "saftnngbnuml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnrlc"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnrlc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnruc"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnruc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnzodiac"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnzodiac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnzodiacd"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnzodiacd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnnzodiacl"] =
            new RtfCtrlWordHandler(rtfParser, "saftnnzodiacl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnrestart"] =
            new RtfCtrlWordHandler(rtfParser, "saftnrestart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnrstcont"] =
            new RtfCtrlWordHandler(rtfParser, "saftnrstcont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saftnstart"] =
            new RtfCtrlWordHandler(rtfParser, "saftnstart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sautoupd"] =
            new RtfCtrlWordHandler(rtfParser, "sautoupd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saveinvalidxml"] = new RtfCtrlWordHandler(rtfParser, "saveinvalidxml", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["saveprevpict"] =
            new RtfCtrlWordHandler(rtfParser, "saveprevpict", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sb"] = new RtfCtrlWordHandler(rtfParser, "sb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sbasedon"] =
            new RtfCtrlWordHandler(rtfParser, "sbasedon", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sbauto"] =
            new RtfCtrlWordHandler(rtfParser, "sbauto", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["sbkcol"] = new RtfCtrlWordHandler(rtfParser, "sbkcol", RtfProperty.SBK_COLUMN, false,
            RtfCtrlWordType.FLAG, "\\", " ", RtfProperty.SECTION_BREAK_TYPE);

        _ctrlWords["sbkeven"] = new RtfCtrlWordHandler(rtfParser, "sbkeven", RtfProperty.SBK_EVEN, false,
            RtfCtrlWordType.FLAG, "\\", " ", RtfProperty.SECTION_BREAK_TYPE);

        _ctrlWords["sbknone"] = new RtfCtrlWordHandler(rtfParser, "sbknone", RtfProperty.SBK_NONE, false,
            RtfCtrlWordType.FLAG, "\\", " ", RtfProperty.SECTION_BREAK_TYPE);

        _ctrlWords["sbkodd"] = new RtfCtrlWordHandler(rtfParser, "sbkodd", RtfProperty.SBK_ODD, false,
            RtfCtrlWordType.FLAG, "\\", " ", RtfProperty.SECTION_BREAK_TYPE);

        _ctrlWords["sbkpage"] = new RtfCtrlWordHandler(rtfParser, "sbkpage", RtfProperty.SBK_PAGE, false,
            RtfCtrlWordType.FLAG, "\\", " ", RtfProperty.SECTION_BREAK_TYPE);

        _ctrlWords["sbys"] = new RtfCtrlWordHandler(rtfParser, "sbys", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["scaps"] =
            new RtfCtrlWordHandler(rtfParser, "scaps", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["scompose"] =
            new RtfCtrlWordHandler(rtfParser, "scompose", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sec"] = new RtfCtrlWordHandler(rtfParser, "sec", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sect"] =
            new RtfCtrlWordHandler(rtfParser, "sect", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["sectd"] =
            new RtfCtrlWordHandler(rtfParser, "sectd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sectdefaultcl"] = new RtfCtrlWordHandler(rtfParser, "sectdefaultcl", 0, false,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sectexpand"] =
            new RtfCtrlWordHandler(rtfParser, "sectexpand", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sectlinegrid"] =
            new RtfCtrlWordHandler(rtfParser, "sectlinegrid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sectnum"] =
            new RtfCtrlWordHandler(rtfParser, "sectnum", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["sectrsid"] =
            new RtfCtrlWordHandler(rtfParser, "sectrsid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sectspecifycl"] =
            new RtfCtrlWordHandler(rtfParser, "sectspecifycl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sectspecifygen"] = new RtfCtrlWordHandler(rtfParser, "sectspecifygen", 0, false,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sectspecifyl"] =
            new RtfCtrlWordHandler(rtfParser, "sectspecifyl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sectunlocked"] =
            new RtfCtrlWordHandler(rtfParser, "sectunlocked", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnbj"] =
            new RtfCtrlWordHandler(rtfParser, "sftnbj", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnalc"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnalc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnar"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnauc"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnauc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnchi"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnchi", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnchosung"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnchosung", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnncnum"] =
            new RtfCtrlWordHandler(rtfParser, "sftnncnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnndbar"] =
            new RtfCtrlWordHandler(rtfParser, "sftnndbar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnndbnum"] =
            new RtfCtrlWordHandler(rtfParser, "sftnndbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnndbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "sftnndbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnndbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "sftnndbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnndbnumt"] =
            new RtfCtrlWordHandler(rtfParser, "sftnndbnumt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnganada"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnganada", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnngbnum"] =
            new RtfCtrlWordHandler(rtfParser, "sftnngbnum", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnngbnumd"] =
            new RtfCtrlWordHandler(rtfParser, "sftnngbnumd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnngbnumk"] =
            new RtfCtrlWordHandler(rtfParser, "sftnngbnumk", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnngbnuml"] =
            new RtfCtrlWordHandler(rtfParser, "sftnngbnuml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnrlc"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnrlc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnruc"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnruc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnzodiac"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnzodiac", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnzodiacd"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnzodiacd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnnzodiacl"] =
            new RtfCtrlWordHandler(rtfParser, "sftnnzodiacl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnrestart"] =
            new RtfCtrlWordHandler(rtfParser, "sftnrestart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnrstcont"] =
            new RtfCtrlWordHandler(rtfParser, "sftnrstcont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnrstpg"] =
            new RtfCtrlWordHandler(rtfParser, "sftnrstpg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftnstart"] =
            new RtfCtrlWordHandler(rtfParser, "sftnstart", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sftntj"] =
            new RtfCtrlWordHandler(rtfParser, "sftntj", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shad"] =
            new RtfCtrlWordHandler(rtfParser, "shad", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["shading"] =
            new RtfCtrlWordHandler(rtfParser, "shading", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shidden"] =
            new RtfCtrlWordHandler(rtfParser, "shidden", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shift"] =
            new RtfCtrlWordHandler(rtfParser, "shift", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["showplaceholdtext"] = new RtfCtrlWordHandler(rtfParser, "showplaceholdtext", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["showxmlerrors"] =
            new RtfCtrlWordHandler(rtfParser, "showxmlerrors", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shp"] = new RtfCtrlWordHandler(rtfParser, "shp", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["shpbottom"] =
            new RtfCtrlWordHandler(rtfParser, "shpbottom", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shpbxcolumn"] =
            new RtfCtrlWordHandler(rtfParser, "shpbxcolumn", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shpbxignore"] =
            new RtfCtrlWordHandler(rtfParser, "shpbxignore", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shpbxmargin"] =
            new RtfCtrlWordHandler(rtfParser, "shpbxmargin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shpbxpage"] =
            new RtfCtrlWordHandler(rtfParser, "shpbxpage", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shpbyignore"] =
            new RtfCtrlWordHandler(rtfParser, "shpbyignore", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shpbymargin"] =
            new RtfCtrlWordHandler(rtfParser, "shpbymargin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shpbypage"] =
            new RtfCtrlWordHandler(rtfParser, "shpbypage", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shpbypara"] =
            new RtfCtrlWordHandler(rtfParser, "shpbypara", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shpfblwtxt"] =
            new RtfCtrlWordHandler(rtfParser, "shpfblwtxt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shpfhdr"] =
            new RtfCtrlWordHandler(rtfParser, "shpfhdr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shpgrp"] =
            new RtfCtrlWordHandler(rtfParser, "shpgrp", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shpinst"] = new RtfCtrlWordHandler(rtfParser, "shpinst", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["shpleft"] =
            new RtfCtrlWordHandler(rtfParser, "shpleft", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shplid"] =
            new RtfCtrlWordHandler(rtfParser, "shplid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shplockanchor"] =
            new RtfCtrlWordHandler(rtfParser, "shplockanchor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["shppict"] = new RtfCtrlWordHandler(rtfParser, "shppict", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationShppict"); //"RtfDestinationShppict";

        _ctrlWords["shpright"] =
            new RtfCtrlWordHandler(rtfParser, "shpright", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shprslt"] = new RtfCtrlWordHandler(rtfParser, "shprslt", 0, true, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationNull");

        _ctrlWords["shptop"] =
            new RtfCtrlWordHandler(rtfParser, "shptop", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shptxt"] =
            new RtfCtrlWordHandler(rtfParser, "shptxt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shpwr"] =
            new RtfCtrlWordHandler(rtfParser, "shpwr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shpwrk"] =
            new RtfCtrlWordHandler(rtfParser, "shpwrk", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["shpz"] = new RtfCtrlWordHandler(rtfParser, "shpz", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["sl"] = new RtfCtrlWordHandler(rtfParser, "sl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["slink"] =
            new RtfCtrlWordHandler(rtfParser, "slink", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["slmult"] =
            new RtfCtrlWordHandler(rtfParser, "slmult", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["slocked"] =
            new RtfCtrlWordHandler(rtfParser, "slocked", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sn"] = new RtfCtrlWordHandler(rtfParser, "sn", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["snapgridtocell"] = new RtfCtrlWordHandler(rtfParser, "snapgridtocell", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["snaptogridincell"] = new RtfCtrlWordHandler(rtfParser, "snaptogridincell", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["snext"] =
            new RtfCtrlWordHandler(rtfParser, "snext", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["softcol"] =
            new RtfCtrlWordHandler(rtfParser, "softcol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["softlheight"] =
            new RtfCtrlWordHandler(rtfParser, "softlheight", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["softline"] =
            new RtfCtrlWordHandler(rtfParser, "softline", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["softpage"] =
            new RtfCtrlWordHandler(rtfParser, "softpage", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sp"] = new RtfCtrlWordHandler(rtfParser, "sp", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["spersonal"] =
            new RtfCtrlWordHandler(rtfParser, "spersonal", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["spltpgpar"] =
            new RtfCtrlWordHandler(rtfParser, "spltpgpar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["splytwnine"] =
            new RtfCtrlWordHandler(rtfParser, "splytwnine", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["spp"] = new RtfCtrlWordHandler(rtfParser, "spp", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["spriority"] =
            new RtfCtrlWordHandler(rtfParser, "spriority", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sprsbsp"] =
            new RtfCtrlWordHandler(rtfParser, "sprsbsp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sprslnsp"] =
            new RtfCtrlWordHandler(rtfParser, "sprslnsp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sprsspbf"] =
            new RtfCtrlWordHandler(rtfParser, "sprsspbf", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sprstsm"] =
            new RtfCtrlWordHandler(rtfParser, "sprstsm", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sprstsp"] =
            new RtfCtrlWordHandler(rtfParser, "sprstsp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["spv"] = new RtfCtrlWordHandler(rtfParser, "spv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sqformat"] =
            new RtfCtrlWordHandler(rtfParser, "sqformat", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sreply"] =
            new RtfCtrlWordHandler(rtfParser, "sreply", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ssemihidden"] =
            new RtfCtrlWordHandler(rtfParser, "ssemihidden", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["staticval"] =
            new RtfCtrlWordHandler(rtfParser, "staticval", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["stextflow"] =
            new RtfCtrlWordHandler(rtfParser, "stextflow", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["strike"] =
            new RtfCtrlWordHandler(rtfParser, "strike", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["striked1"] =
            new RtfCtrlWordHandler(rtfParser, "striked1", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["stshfbi"] =
            new RtfCtrlWordHandler(rtfParser, "stshfbi", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["stshfdbch"] =
            new RtfCtrlWordHandler(rtfParser, "stshfdbch", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["stshfhich"] =
            new RtfCtrlWordHandler(rtfParser, "stshfhich", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["stshfloch"] =
            new RtfCtrlWordHandler(rtfParser, "stshfloch", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["stylelock"] =
            new RtfCtrlWordHandler(rtfParser, "stylelock", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["stylelockbackcomp"] = new RtfCtrlWordHandler(rtfParser, "stylelockbackcomp", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["stylelockenforced"] = new RtfCtrlWordHandler(rtfParser, "stylelockenforced", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["stylelockqfset"] = new RtfCtrlWordHandler(rtfParser, "stylelockqfset", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["stylelocktheme"] = new RtfCtrlWordHandler(rtfParser, "stylelocktheme", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["stylesheet"] = new RtfCtrlWordHandler(rtfParser, "stylesheet", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationStylesheetTable");

        _ctrlWords["stylesortmethod"] = new RtfCtrlWordHandler(rtfParser, "stylesortmethod", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["styrsid"] =
            new RtfCtrlWordHandler(rtfParser, "styrsid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["sub"] = new RtfCtrlWordHandler(rtfParser, "sub", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["subdocument"] =
            new RtfCtrlWordHandler(rtfParser, "subdocument", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["subfontbysize"] =
            new RtfCtrlWordHandler(rtfParser, "subfontbysize", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["subject"] = new RtfCtrlWordHandler(rtfParser, "subject", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationInfo");

        _ctrlWords["sunhideused"] =
            new RtfCtrlWordHandler(rtfParser, "sunhideused", 0, true, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["super"] =
            new RtfCtrlWordHandler(rtfParser, "super", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["sv"] = new RtfCtrlWordHandler(rtfParser, "sv", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["svb"] = new RtfCtrlWordHandler(rtfParser, "svb", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["swpbdr"] =
            new RtfCtrlWordHandler(rtfParser, "swpbdr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tab"] = new RtfCtrlWordHandler(rtfParser, "tab", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["tabsnoovrlp"] =
            new RtfCtrlWordHandler(rtfParser, "tabsnoovrlp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["taprtl"] =
            new RtfCtrlWordHandler(rtfParser, "taprtl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tb"] = new RtfCtrlWordHandler(rtfParser, "tb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tblind"] =
            new RtfCtrlWordHandler(rtfParser, "tblind", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tblindtype"] =
            new RtfCtrlWordHandler(rtfParser, "tblindtype", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tbllkbestfit"] =
            new RtfCtrlWordHandler(rtfParser, "tbllkbestfit", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllkborder"] =
            new RtfCtrlWordHandler(rtfParser, "tbllkborder", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllkcolor"] =
            new RtfCtrlWordHandler(rtfParser, "tbllkcolor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllkfont"] =
            new RtfCtrlWordHandler(rtfParser, "tbllkfont", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllkhdrcols"] =
            new RtfCtrlWordHandler(rtfParser, "tbllkhdrcols", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllkhdrrows"] =
            new RtfCtrlWordHandler(rtfParser, "tbllkhdrrows", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllklastcol"] =
            new RtfCtrlWordHandler(rtfParser, "tbllklastcol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllklastrow"] =
            new RtfCtrlWordHandler(rtfParser, "tbllklastrow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllknocolband"] = new RtfCtrlWordHandler(rtfParser, "tbllknocolband", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllknorowband"] = new RtfCtrlWordHandler(rtfParser, "tbllknorowband", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tbllkshading"] =
            new RtfCtrlWordHandler(rtfParser, "tbllkshading", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tblrsid"] =
            new RtfCtrlWordHandler(rtfParser, "tblrsid", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tc"] = new RtfCtrlWordHandler(rtfParser, "tc", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["tcelld"] =
            new RtfCtrlWordHandler(rtfParser, "tcelld", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tcf"] = new RtfCtrlWordHandler(rtfParser, "tcf", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["tcl"] = new RtfCtrlWordHandler(rtfParser, "tcl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["tcn"] = new RtfCtrlWordHandler(rtfParser, "tcn", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tdfrmtxtBottom"] = new RtfCtrlWordHandler(rtfParser, "tdfrmtxtBottom", 0, true,
            RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tdfrmtxtLeft"] =
            new RtfCtrlWordHandler(rtfParser, "tdfrmtxtLeft", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tdfrmtxtRight"] =
            new RtfCtrlWordHandler(rtfParser, "tdfrmtxtRight", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tdfrmtxtTop"] =
            new RtfCtrlWordHandler(rtfParser, "tdfrmtxtTop", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["template"] = new RtfCtrlWordHandler(rtfParser, "template", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["themedata"] = new RtfCtrlWordHandler(rtfParser, "themedata", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["themelang"] =
            new RtfCtrlWordHandler(rtfParser, "themelang", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["themelangcs"] =
            new RtfCtrlWordHandler(rtfParser, "themelangcs", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["themelangfe"] =
            new RtfCtrlWordHandler(rtfParser, "themelangfe", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["time"] = new RtfCtrlWordHandler(rtfParser, "time", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["title"] = new RtfCtrlWordHandler(rtfParser, "title", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationInfo");

        _ctrlWords["titlepg"] =
            new RtfCtrlWordHandler(rtfParser, "titlepg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tldot"] =
            new RtfCtrlWordHandler(rtfParser, "tldot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tleq"] = new RtfCtrlWordHandler(rtfParser, "tleq", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tlhyph"] =
            new RtfCtrlWordHandler(rtfParser, "tlhyph", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tlmdot"] =
            new RtfCtrlWordHandler(rtfParser, "tlmdot", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tlth"] = new RtfCtrlWordHandler(rtfParser, "tlth", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["tlul"] = new RtfCtrlWordHandler(rtfParser, "tlul", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["toplinepunct"] =
            new RtfCtrlWordHandler(rtfParser, "toplinepunct", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tphcol"] =
            new RtfCtrlWordHandler(rtfParser, "tphcol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tphmrg"] =
            new RtfCtrlWordHandler(rtfParser, "tphmrg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tphpg"] =
            new RtfCtrlWordHandler(rtfParser, "tphpg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposnegx"] =
            new RtfCtrlWordHandler(rtfParser, "tposnegx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tposnegy"] =
            new RtfCtrlWordHandler(rtfParser, "tposnegy", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tposx"] =
            new RtfCtrlWordHandler(rtfParser, "tposx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tposxc"] =
            new RtfCtrlWordHandler(rtfParser, "tposxc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposxi"] =
            new RtfCtrlWordHandler(rtfParser, "tposxi", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposxl"] =
            new RtfCtrlWordHandler(rtfParser, "tposxl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposxo"] =
            new RtfCtrlWordHandler(rtfParser, "tposxo", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposxr"] =
            new RtfCtrlWordHandler(rtfParser, "tposxr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposy"] =
            new RtfCtrlWordHandler(rtfParser, "tposy", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposyb"] =
            new RtfCtrlWordHandler(rtfParser, "tposyb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposyc"] =
            new RtfCtrlWordHandler(rtfParser, "tposyc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposyil"] =
            new RtfCtrlWordHandler(rtfParser, "tposyil", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposyin"] =
            new RtfCtrlWordHandler(rtfParser, "tposyin", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposyout"] =
            new RtfCtrlWordHandler(rtfParser, "tposyout", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposyoutv"] =
            new RtfCtrlWordHandler(rtfParser, "tposyoutv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tposyt"] =
            new RtfCtrlWordHandler(rtfParser, "tposyt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tpvmrg"] =
            new RtfCtrlWordHandler(rtfParser, "tpvmrg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tpvpara"] =
            new RtfCtrlWordHandler(rtfParser, "tpvpara", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tpvpg"] =
            new RtfCtrlWordHandler(rtfParser, "tpvpg", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tqc"] = new RtfCtrlWordHandler(rtfParser, "tqc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tqdec"] =
            new RtfCtrlWordHandler(rtfParser, "tqdec", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tqr"] = new RtfCtrlWordHandler(rtfParser, "tqr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trackformatting"] = new RtfCtrlWordHandler(rtfParser, "trackformatting", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trackmoves"] =
            new RtfCtrlWordHandler(rtfParser, "trackmoves", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["transmf"] =
            new RtfCtrlWordHandler(rtfParser, "transmf", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trauth"] =
            new RtfCtrlWordHandler(rtfParser, "trauth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trautofit"] =
            new RtfCtrlWordHandler(rtfParser, "trautofit", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["trbgbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "trbgbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgcross"] =
            new RtfCtrlWordHandler(rtfParser, "trbgcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgdcross"] =
            new RtfCtrlWordHandler(rtfParser, "trbgdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgdkbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "trbgdkbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgdkcross"] =
            new RtfCtrlWordHandler(rtfParser, "trbgdkcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgdkdcross"] =
            new RtfCtrlWordHandler(rtfParser, "trbgdkdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgdkfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "trbgdkfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgdkhor"] =
            new RtfCtrlWordHandler(rtfParser, "trbgdkhor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgdkvert"] =
            new RtfCtrlWordHandler(rtfParser, "trbgdkvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "trbgfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbghoriz"] =
            new RtfCtrlWordHandler(rtfParser, "trbghoriz", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbgvert"] =
            new RtfCtrlWordHandler(rtfParser, "trbgvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbrdrb"] =
            new RtfCtrlWordHandler(rtfParser, "trbrdrb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbrdrh"] =
            new RtfCtrlWordHandler(rtfParser, "trbrdrh", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbrdrl"] =
            new RtfCtrlWordHandler(rtfParser, "trbrdrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbrdrr"] =
            new RtfCtrlWordHandler(rtfParser, "trbrdrr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbrdrt"] =
            new RtfCtrlWordHandler(rtfParser, "trbrdrt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trbrdrv"] =
            new RtfCtrlWordHandler(rtfParser, "trbrdrv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trcbpat"] =
            new RtfCtrlWordHandler(rtfParser, "trcbpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trcfpat"] =
            new RtfCtrlWordHandler(rtfParser, "trcfpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trdate"] =
            new RtfCtrlWordHandler(rtfParser, "trdate", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trftsWidth"] =
            new RtfCtrlWordHandler(rtfParser, "trftsWidth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trftsWidthA"] =
            new RtfCtrlWordHandler(rtfParser, "trftsWidthA", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trftsWidthB"] =
            new RtfCtrlWordHandler(rtfParser, "trftsWidthB", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trgaph"] =
            new RtfCtrlWordHandler(rtfParser, "trgaph", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trhdr"] =
            new RtfCtrlWordHandler(rtfParser, "trhdr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trkeep"] =
            new RtfCtrlWordHandler(rtfParser, "trkeep", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trkeepfollow"] =
            new RtfCtrlWordHandler(rtfParser, "trkeepfollow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trleft"] =
            new RtfCtrlWordHandler(rtfParser, "trleft", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trowd"] =
            new RtfCtrlWordHandler(rtfParser, "trowd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trpaddb"] =
            new RtfCtrlWordHandler(rtfParser, "trpaddb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trpaddfb"] =
            new RtfCtrlWordHandler(rtfParser, "trpaddfb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trpaddfl"] =
            new RtfCtrlWordHandler(rtfParser, "trpaddfl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trpaddfr"] =
            new RtfCtrlWordHandler(rtfParser, "trpaddfr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trpaddft"] =
            new RtfCtrlWordHandler(rtfParser, "trpaddft", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trpaddl"] =
            new RtfCtrlWordHandler(rtfParser, "trpaddl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trpaddr"] =
            new RtfCtrlWordHandler(rtfParser, "trpaddr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trpaddt"] =
            new RtfCtrlWordHandler(rtfParser, "trpaddt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trpat"] =
            new RtfCtrlWordHandler(rtfParser, "trpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trqc"] = new RtfCtrlWordHandler(rtfParser, "trqc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["trql"] = new RtfCtrlWordHandler(rtfParser, "trql", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["trqr"] = new RtfCtrlWordHandler(rtfParser, "trqr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);
        _ctrlWords["trrh"] = new RtfCtrlWordHandler(rtfParser, "trrh", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trshdng"] =
            new RtfCtrlWordHandler(rtfParser, "trshdng", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trspdb"] =
            new RtfCtrlWordHandler(rtfParser, "trspdb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trspdfb"] =
            new RtfCtrlWordHandler(rtfParser, "trspdfb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trspdfl"] =
            new RtfCtrlWordHandler(rtfParser, "trspdfl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trspdfr"] =
            new RtfCtrlWordHandler(rtfParser, "trspdfr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trspdft"] =
            new RtfCtrlWordHandler(rtfParser, "trspdft", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trspdl"] =
            new RtfCtrlWordHandler(rtfParser, "trspdl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trspdr"] =
            new RtfCtrlWordHandler(rtfParser, "trspdr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trspdt"] =
            new RtfCtrlWordHandler(rtfParser, "trspdt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["truncatefontheight"] = new RtfCtrlWordHandler(rtfParser, "truncatefontheight", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["truncex"] =
            new RtfCtrlWordHandler(rtfParser, "truncex", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["trwWidth"] =
            new RtfCtrlWordHandler(rtfParser, "trwWidth", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trwWidthA"] =
            new RtfCtrlWordHandler(rtfParser, "trwWidthA", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["trwWidthB"] =
            new RtfCtrlWordHandler(rtfParser, "trwWidthB", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ts"] = new RtfCtrlWordHandler(rtfParser, "ts", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tsbgbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgcross"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgdcross"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgdkbdiag"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgdkbdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgdkcross"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgdkcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgdkdcross"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgdkdcross", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgdkfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgdkfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgdkhor"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgdkhor", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgdkvert"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgdkvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgfdiag"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgfdiag", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbghoriz"] =
            new RtfCtrlWordHandler(rtfParser, "tsbghoriz", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbgvert"] =
            new RtfCtrlWordHandler(rtfParser, "tsbgvert", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbrdrb"] =
            new RtfCtrlWordHandler(rtfParser, "tsbrdrb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbrdrdgl"] =
            new RtfCtrlWordHandler(rtfParser, "tsbrdrdgl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbrdrdgr"] =
            new RtfCtrlWordHandler(rtfParser, "tsbrdrdgr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbrdrh"] =
            new RtfCtrlWordHandler(rtfParser, "tsbrdrh", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbrdrl"] =
            new RtfCtrlWordHandler(rtfParser, "tsbrdrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbrdrr"] =
            new RtfCtrlWordHandler(rtfParser, "tsbrdrr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbrdrt"] =
            new RtfCtrlWordHandler(rtfParser, "tsbrdrt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsbrdrv"] =
            new RtfCtrlWordHandler(rtfParser, "tsbrdrv", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscbandhorzeven"] = new RtfCtrlWordHandler(rtfParser, "tscbandhorzeven", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscbandhorzodd"] = new RtfCtrlWordHandler(rtfParser, "tscbandhorzodd", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscbandsh"] =
            new RtfCtrlWordHandler(rtfParser, "tscbandsh", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscbandsv"] =
            new RtfCtrlWordHandler(rtfParser, "tscbandsv", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscbandverteven"] = new RtfCtrlWordHandler(rtfParser, "tscbandverteven", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscbandvertodd"] = new RtfCtrlWordHandler(rtfParser, "tscbandvertodd", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscellcbpat"] =
            new RtfCtrlWordHandler(rtfParser, "tscellcbpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellcfpat"] =
            new RtfCtrlWordHandler(rtfParser, "tscellcfpat", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpaddb"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpaddb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpaddfb"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpaddfb", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpaddfl"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpaddfl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpaddfr"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpaddfr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpaddft"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpaddft", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpaddl"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpaddl", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpaddr"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpaddr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpaddt"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpaddt", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellpct"] =
            new RtfCtrlWordHandler(rtfParser, "tscellpct", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["tscellwidth"] =
            new RtfCtrlWordHandler(rtfParser, "tscellwidth", 0, true, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscellwidthfts"] =
            new RtfCtrlWordHandler(rtfParser, "tscellwidthfts", 0, true, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscfirstcol"] =
            new RtfCtrlWordHandler(rtfParser, "tscfirstcol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscfirstrow"] =
            new RtfCtrlWordHandler(rtfParser, "tscfirstrow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsclastcol"] =
            new RtfCtrlWordHandler(rtfParser, "tsclastcol", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsclastrow"] =
            new RtfCtrlWordHandler(rtfParser, "tsclastrow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscnecell"] =
            new RtfCtrlWordHandler(rtfParser, "tscnecell", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscnwcell"] =
            new RtfCtrlWordHandler(rtfParser, "tscnwcell", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscsecell"] =
            new RtfCtrlWordHandler(rtfParser, "tscsecell", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tscswcell"] =
            new RtfCtrlWordHandler(rtfParser, "tscswcell", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsd"] = new RtfCtrlWordHandler(rtfParser, "tsd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsnowrap"] =
            new RtfCtrlWordHandler(rtfParser, "tsnowrap", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsrowd"] =
            new RtfCtrlWordHandler(rtfParser, "tsrowd", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsvertalb"] =
            new RtfCtrlWordHandler(rtfParser, "tsvertalb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsvertalc"] =
            new RtfCtrlWordHandler(rtfParser, "tsvertalc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tsvertalt"] =
            new RtfCtrlWordHandler(rtfParser, "tsvertalt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["twoonone"] =
            new RtfCtrlWordHandler(rtfParser, "twoonone", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["tx"] = new RtfCtrlWordHandler(rtfParser, "tx", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["txbxtwalways"] =
            new RtfCtrlWordHandler(rtfParser, "txbxtwalways", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["txbxtwfirst"] =
            new RtfCtrlWordHandler(rtfParser, "txbxtwfirst", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["txbxtwfirstlast"] = new RtfCtrlWordHandler(rtfParser, "txbxtwfirstlast", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["txbxtwlast"] =
            new RtfCtrlWordHandler(rtfParser, "txbxtwlast", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["txbxtwno"] =
            new RtfCtrlWordHandler(rtfParser, "txbxtwno", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["txe"] = new RtfCtrlWordHandler(rtfParser, "txe", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["u"] = new RtfCtrlWordHandler(rtfParser, "u", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["uc"] = new RtfCtrlWordHandler(rtfParser, "uc", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["ud"] = new RtfCtrlWordHandler(rtfParser, "ud", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["ul"] = new RtfCtrlWordHandler(rtfParser, "ul", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);
        _ctrlWords["ulc"] = new RtfCtrlWordHandler(rtfParser, "ulc", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["uld"] = new RtfCtrlWordHandler(rtfParser, "uld", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["uldash"] =
            new RtfCtrlWordHandler(rtfParser, "uldash", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["uldashd"] =
            new RtfCtrlWordHandler(rtfParser, "uldashd", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["uldashdd"] =
            new RtfCtrlWordHandler(rtfParser, "uldashdd", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["uldb"] =
            new RtfCtrlWordHandler(rtfParser, "uldb", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulhair"] =
            new RtfCtrlWordHandler(rtfParser, "ulhair", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulhwave"] =
            new RtfCtrlWordHandler(rtfParser, "ulhwave", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulldash"] =
            new RtfCtrlWordHandler(rtfParser, "ulldash", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulnone"] =
            new RtfCtrlWordHandler(rtfParser, "ulnone", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ulth"] =
            new RtfCtrlWordHandler(rtfParser, "ulth", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulthd"] =
            new RtfCtrlWordHandler(rtfParser, "ulthd", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulthdash"] =
            new RtfCtrlWordHandler(rtfParser, "ulthdash", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulthdashd"] =
            new RtfCtrlWordHandler(rtfParser, "ulthdashd", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulthdashdd"] =
            new RtfCtrlWordHandler(rtfParser, "ulthdashdd", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulthldash"] =
            new RtfCtrlWordHandler(rtfParser, "ulthldash", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ululdbwave"] =
            new RtfCtrlWordHandler(rtfParser, "ululdbwave", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["ulw"] = new RtfCtrlWordHandler(rtfParser, "ulw", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["ulwave"] =
            new RtfCtrlWordHandler(rtfParser, "ulwave", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["up"] = new RtfCtrlWordHandler(rtfParser, "up", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["upr"] = new RtfCtrlWordHandler(rtfParser, "upr", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["urtf"] = new RtfCtrlWordHandler(rtfParser, "urtf", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["useltbaln"] =
            new RtfCtrlWordHandler(rtfParser, "useltbaln", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["usenormstyforlist"] = new RtfCtrlWordHandler(rtfParser, "usenormstyforlist", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["userprops"] = new RtfCtrlWordHandler(rtfParser, "userprops", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", null);

        _ctrlWords["usexform"] =
            new RtfCtrlWordHandler(rtfParser, "usexform", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["utinl"] =
            new RtfCtrlWordHandler(rtfParser, "utinl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["v"] = new RtfCtrlWordHandler(rtfParser, "v", 0, false, RtfCtrlWordType.TOGGLE, "\\", " ", null);

        _ctrlWords["validatexml"] =
            new RtfCtrlWordHandler(rtfParser, "validatexml", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["vern"] = new RtfCtrlWordHandler(rtfParser, "vern", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["version"] =
            new RtfCtrlWordHandler(rtfParser, "version", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["vertalb"] =
            new RtfCtrlWordHandler(rtfParser, "vertalb", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["vertalc"] =
            new RtfCtrlWordHandler(rtfParser, "vertalc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["vertalj"] =
            new RtfCtrlWordHandler(rtfParser, "vertalj", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["vertalt"] =
            new RtfCtrlWordHandler(rtfParser, "vertalt", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["vertdoc"] =
            new RtfCtrlWordHandler(rtfParser, "vertdoc", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["vertsect"] =
            new RtfCtrlWordHandler(rtfParser, "vertsect", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["viewbksp"] =
            new RtfCtrlWordHandler(rtfParser, "viewbksp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["viewkind"] =
            new RtfCtrlWordHandler(rtfParser, "viewkind", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["viewnobound"] =
            new RtfCtrlWordHandler(rtfParser, "viewnobound", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["viewscale"] =
            new RtfCtrlWordHandler(rtfParser, "viewscale", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["viewzk"] =
            new RtfCtrlWordHandler(rtfParser, "viewzk", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["wbitmap"] =
            new RtfCtrlWordHandler(rtfParser, "wbitmap", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["wbmbitspixel"] =
            new RtfCtrlWordHandler(rtfParser, "wbmbitspixel", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["wbmplanes"] =
            new RtfCtrlWordHandler(rtfParser, "wbmplanes", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["wbmwidthbytes"] =
            new RtfCtrlWordHandler(rtfParser, "wbmwidthbytes", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["webhidden"] =
            new RtfCtrlWordHandler(rtfParser, "webhidden", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wgrffmtfilter"] = new RtfCtrlWordHandler(rtfParser, "wgrffmtfilter", 0, false,
            RtfCtrlWordType.VALUE, "\\*\\", " ", null);

        _ctrlWords["widctlpar"] =
            new RtfCtrlWordHandler(rtfParser, "widctlpar", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["widowctrl"] =
            new RtfCtrlWordHandler(rtfParser, "widowctrl", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["windowcaption"] =
            new RtfCtrlWordHandler(rtfParser, "windowcaption", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["wmetafile"] =
            new RtfCtrlWordHandler(rtfParser, "wmetafile", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["wpeqn"] =
            new RtfCtrlWordHandler(rtfParser, "wpeqn", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wpjst"] =
            new RtfCtrlWordHandler(rtfParser, "wpjst", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wpsp"] = new RtfCtrlWordHandler(rtfParser, "wpsp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wptab"] =
            new RtfCtrlWordHandler(rtfParser, "wptab", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wraparound"] =
            new RtfCtrlWordHandler(rtfParser, "wraparound", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wrapdefault"] =
            new RtfCtrlWordHandler(rtfParser, "wrapdefault", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wrapthrough"] =
            new RtfCtrlWordHandler(rtfParser, "wrapthrough", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wraptight"] =
            new RtfCtrlWordHandler(rtfParser, "wraptight", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["wraptrsp"] =
            new RtfCtrlWordHandler(rtfParser, "wraptrsp", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["writereservhash"] = new RtfCtrlWordHandler(rtfParser, "writereservhash", 0, false,
            RtfCtrlWordType.DESTINATION_EX, "\\*\\", " ", null);

        _ctrlWords["wrppunct"] =
            new RtfCtrlWordHandler(rtfParser, "wrppunct", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["xe"] = new RtfCtrlWordHandler(rtfParser, "xe", 0, false, RtfCtrlWordType.DESTINATION, "\\", " ",
            "RtfDestinationDocument");

        _ctrlWords["xef"] = new RtfCtrlWordHandler(rtfParser, "xef", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["xform"] = new RtfCtrlWordHandler(rtfParser, "xform", 0, false, RtfCtrlWordType.DESTINATION, "\\",
            " ", "RtfDestinationDocument");

        _ctrlWords["xmlattr"] =
            new RtfCtrlWordHandler(rtfParser, "xmlattr", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["xmlattrname"] = new RtfCtrlWordHandler(rtfParser, "xmlattrname", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["xmlattrns"] =
            new RtfCtrlWordHandler(rtfParser, "xmlattrns", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["xmlattrvalue"] = new RtfCtrlWordHandler(rtfParser, "xmlattrvalue", 0, false,
            RtfCtrlWordType.DESTINATION, "\\", " ", "RtfDestinationDocument");

        _ctrlWords["xmlclose"] = new RtfCtrlWordHandler(rtfParser, "xmlclose", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["xmlname"] = new RtfCtrlWordHandler(rtfParser, "xmlname", 0, false, RtfCtrlWordType.DESTINATION,
            "\\", " ", "RtfDestinationDocument");

        _ctrlWords["xmlns"] =
            new RtfCtrlWordHandler(rtfParser, "xmlns", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["xmlnstbl"] = new RtfCtrlWordHandler(rtfParser, "xmlnstbl", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["xmlopen"] = new RtfCtrlWordHandler(rtfParser, "xmlopen", 0, false, RtfCtrlWordType.DESTINATION_EX,
            "\\*\\", " ", "RtfDestinationDocument");

        _ctrlWords["xmlsdttcell"] =
            new RtfCtrlWordHandler(rtfParser, "xmlsdttcell", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["xmlsdttpara"] =
            new RtfCtrlWordHandler(rtfParser, "xmlsdttpara", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["xmlsdttregular"] = new RtfCtrlWordHandler(rtfParser, "xmlsdttregular", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["xmlsdttrow"] =
            new RtfCtrlWordHandler(rtfParser, "xmlsdttrow", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["xmlsdttunknown"] = new RtfCtrlWordHandler(rtfParser, "xmlsdttunknown", 0, false,
            RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["yr"] = new RtfCtrlWordHandler(rtfParser, "yr", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["yts"] = new RtfCtrlWordHandler(rtfParser, "yts", 0, true, RtfCtrlWordType.VALUE, "\\", " ", null);
        _ctrlWords["yxe"] = new RtfCtrlWordHandler(rtfParser, "yxe", 0, false, RtfCtrlWordType.FLAG, "\\", " ", null);

        _ctrlWords["zwbo"] =
            new RtfCtrlWordHandler(rtfParser, "zwbo", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["zwj"] = new RtfCtrlWordHandler(rtfParser, "zwj", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["zwnbo"] =
            new RtfCtrlWordHandler(rtfParser, "zwnbo", 0, false, RtfCtrlWordType.SYMBOL, "\\", " ", null);

        _ctrlWords["zwnj"] =
            new RtfCtrlWordHandler(rtfParser, "zwnj", 0, false, RtfCtrlWordType.VALUE, "\\", " ", null);

        _ctrlWords["{"] = new RtfCtrlWordHandler(rtfParser, "{", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", "{");
        _ctrlWords["|"] = new RtfCtrlWordHandler(rtfParser, "|", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", "|");
        _ctrlWords["}"] = new RtfCtrlWordHandler(rtfParser, "}", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", "}");
        _ctrlWords["~"] = new RtfCtrlWordHandler(rtfParser, "~", 0, false, RtfCtrlWordType.SYMBOL, "\\", "", "~");

        _ctrlWords["unknown"] =
            new RtfCtrlWordHandler(rtfParser, "unknown", 0, false, RtfCtrlWordType.UNIDENTIFIED, "\\", " ", null);
    }

    /// <summary>
    ///     Get the Hashtable object containing the control words.
    ///     Initializes the instance if this is the first instantiation
    ///     of RtfCtrlWords class.
    ///     @since 2.0.8
    /// </summary>
    public RtfCtrlWordHandler GetCtrlWordHandler(string ctrlWord)
    {
        try
        {
            if (_ctrlWords.TryGetValue(ctrlWord, out var handler))
            {
                // add 1 to known control words
                return handler;
            }

            // add 1 to unknown control words
            return _ctrlWords["unknown"];
        }
        catch
        {
        }

        return null;
    }
}