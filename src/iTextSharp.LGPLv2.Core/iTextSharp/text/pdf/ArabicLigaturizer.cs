using System.Text;

namespace iTextSharp.text.pdf;

/// <summary>
///     Shape arabic characters. This code was inspired by an LGPL'ed C library:
///     Pango ( see http://www.pango.com/ ). Note that the code of this is the
///     original work of Paulo Soares. Hence it is perfectly justifiable to distribute
///     it under the MPL.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public static class ArabicLigaturizer
{
    public const int ar_composedtashkeel = 0x4;

    public const int ar_lig = 0x8;

    public const int ar_nothing = 0x0;

    public const int ar_novowel = 0x1;

    /// <summary>
    ///     Digit type option: Use Arabic-Indic digits (U+0660...U+0669).
    /// </summary>
    public const int DIGIT_TYPE_AN = 0;

    /// <summary>
    ///     Digit type option: Use Eastern (Extended) Arabic-Indic digits (U+06f0...U+06f9).
    /// </summary>
    public const int DIGIT_TYPE_AN_EXTENDED = 0x100;

    /// <summary>
    ///     Bit mask for digit type options.
    /// </summary>
    public const int DIGIT_TYPE_MASK = '\u0100';

    /// <summary>
    ///     Digit shaping option: Replace Arabic-Indic digits by European digits (U+0030...U+0039).
    /// </summary>
    public const int DIGITS_AN2EN = 0x40;

    /// <summary>
    ///     Digit shaping option: Replace European digits (U+0030...U+0039) by Arabic-Indic digits.
    /// </summary>
    public const int DIGITS_EN2AN = 0x20;

    /// <summary>
    ///     Digit shaping option:
    ///     Replace European digits (U+0030...U+0039) by Arabic-Indic digits
    ///     if the most recent strongly directional character
    ///     is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
    ///     The initial state at the start of the text is assumed to be an Arabic,
    ///     letter, so European digits at the start of the text will change.
    ///     Compare to DIGITS_ALEN2AN_INT_LR.
    /// </summary>
    public const int DIGITS_EN2AN_INIT_AL = 0x80;

    /// <summary>
    ///     Digit shaping option:
    ///     Replace European digits (U+0030...U+0039) by Arabic-Indic digits
    ///     if the most recent strongly directional character
    ///     is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
    ///     The initial state at the start of the text is assumed to be not an Arabic,
    ///     letter, so European digits at the start of the text will not change.
    ///     Compare to DIGITS_ALEN2AN_INIT_AL.
    /// </summary>
    public const int DIGITS_EN2AN_INIT_LR = 0x60;

    /// <summary>
    ///     Bit mask for digit shaping options.
    /// </summary>
    public const int DIGITS_MASK = 0xe0;

    private const char Alef = '\u0627';

    private const char Alefhamza = '\u0623';

    private const char Alefhamzabelow = '\u0625';

    private const char Alefmadda = '\u0622';

    private const char Alefmaksura = '\u0649';

    private const char Damma = '\u064F';

    /// <summary>
    ///     Not a valid option value.
    /// </summary>
    private const int DigitsReserved = 0xa0;

    private const char Farsiyeh = '\u06CC';

    private const char Fatha = '\u064E';

    private const char Hamza = '\u0621';

    private const char Hamzaabove = '\u0654';

    private const char Hamzabelow = '\u0655';

    private const char Kasra = '\u0650';

    private const char Lam = '\u0644';

    private const char LamAlef = '\uFEFB';

    private const char LamAlefhamza = '\uFEF7';

    private const char LamAlefhamzabelow = '\uFEF9';

    private const char LamAlefmadda = '\uFEF5';

    private const char Madda = '\u0653';

    private const char Shadda = '\u0651';

    private const char Tatweel = '\u0640';

    private const char Waw = '\u0648';

    private const char Wawhamza = '\u0624';

    private const char Yeh = '\u064A';

    private const char Yehhamza = '\u0626';

    private const char Zwj = '\u200D';

    private static readonly char[][] _chartable =
    {
        new[] { '\u0621', '\uFE80' }, /* HAMZA */
        new[]
        {
            '\u0622', '\uFE81', '\uFE82',
        }, /* ALEF WITH MADDA ABOVE */
        new[]
        {
            '\u0623', '\uFE83', '\uFE84',
        }, /* ALEF WITH HAMZA ABOVE */
        new[] { '\u0624', '\uFE85', '\uFE86' }, /* WAW WITH HAMZA ABOVE */
        new[]
        {
            '\u0625', '\uFE87', '\uFE88',
        }, /* ALEF WITH HAMZA BELOW */
        new[]
        {
            '\u0626', '\uFE89', '\uFE8A', '\uFE8B', '\uFE8C',
        }, /* YEH WITH HAMZA ABOVE */
        new[] { '\u0627', '\uFE8D', '\uFE8E' }, /* ALEF */
        new[]
        {
            '\u0628', '\uFE8F', '\uFE90', '\uFE91', '\uFE92',
        }, /* BEH */
        new[] { '\u0629', '\uFE93', '\uFE94' }, /* TEH MARBUTA */
        new[]
        {
            '\u062A', '\uFE95', '\uFE96', '\uFE97', '\uFE98',
        }, /* TEH */
        new[]
        {
            '\u062B', '\uFE99', '\uFE9A', '\uFE9B', '\uFE9C',
        }, /* THEH */
        new[]
        {
            '\u062C', '\uFE9D', '\uFE9E', '\uFE9F', '\uFEA0',
        }, /* JEEM */
        new[]
        {
            '\u062D', '\uFEA1', '\uFEA2', '\uFEA3', '\uFEA4',
        }, /* HAH */
        new[]
        {
            '\u062E', '\uFEA5', '\uFEA6', '\uFEA7', '\uFEA8',
        }, /* KHAH */
        new[] { '\u062F', '\uFEA9', '\uFEAA' }, /* DAL */
        new[] { '\u0630', '\uFEAB', '\uFEAC' }, /* THAL */
        new[] { '\u0631', '\uFEAD', '\uFEAE' }, /* REH */
        new[] { '\u0632', '\uFEAF', '\uFEB0' }, /* ZAIN */
        new[]
        {
            '\u0633', '\uFEB1', '\uFEB2', '\uFEB3', '\uFEB4',
        }, /* SEEN */
        new[]
        {
            '\u0634', '\uFEB5', '\uFEB6', '\uFEB7', '\uFEB8',
        }, /* SHEEN */
        new[]
        {
            '\u0635', '\uFEB9', '\uFEBA', '\uFEBB', '\uFEBC',
        }, /* SAD */
        new[]
        {
            '\u0636', '\uFEBD', '\uFEBE', '\uFEBF', '\uFEC0',
        }, /* DAD */
        new[]
        {
            '\u0637', '\uFEC1', '\uFEC2', '\uFEC3', '\uFEC4',
        }, /* TAH */
        new[]
        {
            '\u0638', '\uFEC5', '\uFEC6', '\uFEC7', '\uFEC8',
        }, /* ZAH */
        new[]
        {
            '\u0639', '\uFEC9', '\uFECA', '\uFECB', '\uFECC',
        }, /* AIN */
        new[]
        {
            '\u063A', '\uFECD', '\uFECE', '\uFECF', '\uFED0',
        }, /* GHAIN */
        new[]
        {
            '\u0640', '\u0640', '\u0640', '\u0640', '\u0640',
        }, /* TATWEEL */
        new[]
        {
            '\u0641', '\uFED1', '\uFED2', '\uFED3', '\uFED4',
        }, /* FEH */
        new[]
        {
            '\u0642', '\uFED5', '\uFED6', '\uFED7', '\uFED8',
        }, /* QAF */
        new[]
        {
            '\u0643', '\uFED9', '\uFEDA', '\uFEDB', '\uFEDC',
        }, /* KAF */
        new[]
        {
            '\u0644', '\uFEDD', '\uFEDE', '\uFEDF', '\uFEE0',
        }, /* LAM */
        new[]
        {
            '\u0645', '\uFEE1', '\uFEE2', '\uFEE3', '\uFEE4',
        }, /* MEEM */
        new[]
        {
            '\u0646', '\uFEE5', '\uFEE6', '\uFEE7', '\uFEE8',
        }, /* NOON */
        new[]
        {
            '\u0647', '\uFEE9', '\uFEEA', '\uFEEB', '\uFEEC',
        }, /* HEH */
        new[] { '\u0648', '\uFEED', '\uFEEE' }, /* WAW */
        new[]
        {
            '\u0649', '\uFEEF', '\uFEF0', '\uFBE8', '\uFBE9',
        }, /* ALEF MAKSURA */
        new[]
        {
            '\u064A', '\uFEF1', '\uFEF2', '\uFEF3', '\uFEF4',
        }, /* YEH */
        new[] { '\u0671', '\uFB50', '\uFB51' }, /* ALEF WASLA */
        new[]
        {
            '\u0679', '\uFB66', '\uFB67', '\uFB68', '\uFB69',
        }, /* TTEH */
        new[]
        {
            '\u067A', '\uFB5E', '\uFB5F', '\uFB60', '\uFB61',
        }, /* TTEHEH */
        new[]
        {
            '\u067B', '\uFB52', '\uFB53', '\uFB54', '\uFB55',
        }, /* BEEH */
        new[]
        {
            '\u067E', '\uFB56', '\uFB57', '\uFB58', '\uFB59',
        }, /* PEH */
        new[]
        {
            '\u067F', '\uFB62', '\uFB63', '\uFB64', '\uFB65',
        }, /* TEHEH */
        new[]
        {
            '\u0680', '\uFB5A', '\uFB5B', '\uFB5C', '\uFB5D',
        }, /* BEHEH */
        new[]
        {
            '\u0683', '\uFB76', '\uFB77', '\uFB78', '\uFB79',
        }, /* NYEH */
        new[]
        {
            '\u0684', '\uFB72', '\uFB73', '\uFB74', '\uFB75',
        }, /* DYEH */
        new[]
        {
            '\u0686', '\uFB7A', '\uFB7B', '\uFB7C', '\uFB7D',
        }, /* TCHEH */
        new[]
        {
            '\u0687', '\uFB7E', '\uFB7F', '\uFB80', '\uFB81',
        }, /* TCHEHEH */
        new[] { '\u0688', '\uFB88', '\uFB89' }, /* DDAL */
        new[] { '\u068C', '\uFB84', '\uFB85' }, /* DAHAL */
        new[] { '\u068D', '\uFB82', '\uFB83' }, /* DDAHAL */
        new[] { '\u068E', '\uFB86', '\uFB87' }, /* DUL */
        new[] { '\u0691', '\uFB8C', '\uFB8D' }, /* RREH */
        new[] { '\u0698', '\uFB8A', '\uFB8B' }, /* JEH */
        new[]
        {
            '\u06A4', '\uFB6A', '\uFB6B', '\uFB6C', '\uFB6D',
        }, /* VEH */
        new[]
        {
            '\u06A6', '\uFB6E', '\uFB6F', '\uFB70', '\uFB71',
        }, /* PEHEH */
        new[]
        {
            '\u06A9', '\uFB8E', '\uFB8F', '\uFB90', '\uFB91',
        }, /* KEHEH */
        new[]
        {
            '\u06AD', '\uFBD3', '\uFBD4', '\uFBD5', '\uFBD6',
        }, /* NG */
        new[]
        {
            '\u06AF', '\uFB92', '\uFB93', '\uFB94', '\uFB95',
        }, /* GAF */
        new[]
        {
            '\u06B1', '\uFB9A', '\uFB9B', '\uFB9C', '\uFB9D',
        }, /* NGOEH */
        new[]
        {
            '\u06B3', '\uFB96', '\uFB97', '\uFB98', '\uFB99',
        }, /* GUEH */
        new[] { '\u06BA', '\uFB9E', '\uFB9F' }, /* NOON GHUNNA */
        new[]
        {
            '\u06BB', '\uFBA0', '\uFBA1', '\uFBA2', '\uFBA3',
        }, /* RNOON */
        new[]
        {
            '\u06BE', '\uFBAA', '\uFBAB', '\uFBAC', '\uFBAD',
        }, /* HEH DOACHASHMEE */
        new[] { '\u06C0', '\uFBA4', '\uFBA5' }, /* HEH WITH YEH ABOVE */
        new[]
        {
            '\u06C1', '\uFBA6', '\uFBA7', '\uFBA8', '\uFBA9',
        }, /* HEH GOAL */
        new[] { '\u06C5', '\uFBE0', '\uFBE1' }, /* KIRGHIZ OE */
        new[] { '\u06C6', '\uFBD9', '\uFBDA' }, /* OE */
        new[] { '\u06C7', '\uFBD7', '\uFBD8' }, /* U */
        new[] { '\u06C8', '\uFBDB', '\uFBDC' }, /* YU */
        new[] { '\u06C9', '\uFBE2', '\uFBE3' }, /* KIRGHIZ YU */
        new[] { '\u06CB', '\uFBDE', '\uFBDF' }, /* VE */
        new[]
        {
            '\u06CC', '\uFBFC', '\uFBFD', '\uFBFE', '\uFBFF',
        }, /* FARSI YEH */
        new[]
        {
            '\u06D0', '\uFBE4', '\uFBE5', '\uFBE6', '\uFBE7',
        }, /* E */
        new[] { '\u06D2', '\uFBAE', '\uFBAF' }, /* YEH BARREE */
        new[]
        {
            '\u06D3', '\uFBB0', '\uFBB1',
        }, /* YEH BARREE WITH HAMZA ABOVE */
    };

    internal static int Arabic_shape(char[] src, int srcoffset, int srclength, char[] dest, int destoffset,
                                     int destlength, int level)
    {
        var str = new char[srclength];
        for (var k = srclength + srcoffset - 1; k >= srcoffset; --k)
        {
            str[k - srcoffset] = src[k];
        }

        var str2 = new StringBuilder(srclength);
        Shape(str, str2, level);
        if ((level & (ar_composedtashkeel | ar_lig)) != 0)
        {
            Doublelig(str2, level);
        }

        //        string.Reverse();
        Array.Copy(str2.ToString().ToCharArray(), 0, dest, destoffset, str2.Length);
        return str2.Length;
    }

    /// <summary>
    ///     return len
    /// </summary>
    internal static void Doublelig(StringBuilder str, int level)
        /* Ok. We have presentation ligatures in our font. */
    {
        int len;
        var olen = len = str.Length;
        int j = 0, si = 1;
        char lapresult;

        while (si < olen)
        {
            lapresult = (char)0;
            if ((level & ar_composedtashkeel) != 0)
            {
                switch (str[j])
                {
                    case Shadda:
                        switch (str[si])
                        {
                            case Kasra:
                                lapresult = '\uFC62';
                                break;
                            case Fatha:
                                lapresult = '\uFC60';
                                break;
                            case Damma:
                                lapresult = '\uFC61';
                                break;
                            case '\u064C':
                                lapresult = '\uFC5E';
                                break;
                            case '\u064D':
                                lapresult = '\uFC5F';
                                break;
                        }

                        break;
                    case Kasra:
                        if (str[si] == Shadda)
                        {
                            lapresult = '\uFC62';
                        }

                        break;
                    case Fatha:
                        if (str[si] == Shadda)
                        {
                            lapresult = '\uFC60';
                        }

                        break;
                    case Damma:
                        if (str[si] == Shadda)
                        {
                            lapresult = '\uFC61';
                        }

                        break;
                }
            }

            if ((level & ar_lig) != 0)
            {
                switch (str[j])
                {
                    case '\uFEDF': /* LAM initial */
                        switch (str[si])
                        {
                            case '\uFE9E':
                                lapresult = '\uFC3F';
                                break; /* JEEM final */
                            case '\uFEA0':
                                lapresult = '\uFCC9';
                                break; /* JEEM medial */
                            case '\uFEA2':
                                lapresult = '\uFC40';
                                break; /* HAH final */
                            case '\uFEA4':
                                lapresult = '\uFCCA';
                                break; /* HAH medial */
                            case '\uFEA6':
                                lapresult = '\uFC41';
                                break; /* KHAH final */
                            case '\uFEA8':
                                lapresult = '\uFCCB';
                                break; /* KHAH medial */
                            case '\uFEE2':
                                lapresult = '\uFC42';
                                break; /* MEEM final */
                            case '\uFEE4':
                                lapresult = '\uFCCC';
                                break; /* MEEM medial */
                        }

                        break;
                    case '\uFE97': /* TEH inital */
                        switch (str[si])
                        {
                            case '\uFEA0':
                                lapresult = '\uFCA1';
                                break; /* JEEM medial */
                            case '\uFEA4':
                                lapresult = '\uFCA2';
                                break; /* HAH medial */
                            case '\uFEA8':
                                lapresult = '\uFCA3';
                                break; /* KHAH medial */
                        }

                        break;
                    case '\uFE91': /* BEH inital */
                        switch (str[si])
                        {
                            case '\uFEA0':
                                lapresult = '\uFC9C';
                                break; /* JEEM medial */
                            case '\uFEA4':
                                lapresult = '\uFC9D';
                                break; /* HAH medial */
                            case '\uFEA8':
                                lapresult = '\uFC9E';
                                break; /* KHAH medial */
                        }

                        break;
                    case '\uFEE7': /* NOON inital */
                        switch (str[si])
                        {
                            case '\uFEA0':
                                lapresult = '\uFCD2';
                                break; /* JEEM initial */
                            case '\uFEA4':
                                lapresult = '\uFCD3';
                                break; /* HAH medial */
                            case '\uFEA8':
                                lapresult = '\uFCD4';
                                break; /* KHAH medial */
                        }

                        break;

                    case '\uFEE8': /* NOON medial */
                        switch (str[si])
                        {
                            case '\uFEAE':
                                lapresult = '\uFC8A';
                                break; /* REH final  */
                            case '\uFEB0':
                                lapresult = '\uFC8B';
                                break; /* ZAIN final */
                        }

                        break;
                    case '\uFEE3': /* MEEM initial */
                        switch (str[si])
                        {
                            case '\uFEA0':
                                lapresult = '\uFCCE';
                                break; /* JEEM medial */
                            case '\uFEA4':
                                lapresult = '\uFCCF';
                                break; /* HAH medial */
                            case '\uFEA8':
                                lapresult = '\uFCD0';
                                break; /* KHAH medial */
                            case '\uFEE4':
                                lapresult = '\uFCD1';
                                break; /* MEEM medial */
                        }

                        break;

                    case '\uFED3': /* FEH initial */
                        switch (str[si])
                        {
                            case '\uFEF2':
                                lapresult = '\uFC32';
                                break; /* YEH final */
                        }

                        break;
                } /* end switch string[si] */
            }

            if (lapresult != 0)
            {
                str[j] = lapresult;
                len--;
                si++; /* jump over one character */
                /* we'll have to change this, too. */
            }
            else
            {
                j++;
                str[j] = str[si];
                si++;
            }
        }

        str.Length = len;
    }

    internal static void ProcessNumbers(char[] text, int offset, int length, int options)
    {
        var limit = offset + length;
        if ((options & DIGITS_MASK) != 0)
        {
            var digitBase = '\u0030'; // European digits
            switch (options & DIGIT_TYPE_MASK)
            {
                case DIGIT_TYPE_AN:
                    digitBase = '\u0660'; // Arabic-Indic digits
                    break;

                case DIGIT_TYPE_AN_EXTENDED:
                    digitBase = '\u06f0'; // Eastern Arabic-Indic digits (Persian and Urdu)
                    break;
            }

            switch (options & DIGITS_MASK)
            {
                case DIGITS_EN2AN:
                {
                    var digitDelta = digitBase - '\u0030';
                    for (var i = offset; i < limit; ++i)
                    {
                        var ch = text[i];
                        if (ch <= '\u0039' && ch >= '\u0030')
                        {
                            text[i] += (char)digitDelta;
                        }
                    }
                }
                    break;

                case DIGITS_AN2EN:
                {
                    var digitTop = (char)(digitBase + 9);
                    var digitDelta = '\u0030' - digitBase;
                    for (var i = offset; i < limit; ++i)
                    {
                        var ch = text[i];
                        if (ch <= digitTop && ch >= digitBase)
                        {
                            text[i] += (char)digitDelta;
                        }
                    }
                }
                    break;

                case DIGITS_EN2AN_INIT_LR:
                    ShapeToArabicDigitsWithContext(text, 0, length, digitBase, false);
                    break;

                case DIGITS_EN2AN_INIT_AL:
                    ShapeToArabicDigitsWithContext(text, 0, length, digitBase, true);
                    break;
            }
        }
    }

    internal static void Shape(char[] text, StringBuilder str, int level)
    {
        /* string is assumed to be empty and big enough.
        * text is the original text.
        * This routine does the basic arabic reshaping.
        * *len the number of non-null characters.
        *
        * Note: We have to unshape each character first!
        */
        int join;
        int which;
        char nextletter;

        var p = 0; /* initialize for output */
        var oldchar = new Charstruct();
        var curchar = new Charstruct();
        while (p < text.Length)
        {
            nextletter = text[p++];
            //nextletter = unshape (nextletter);

            join = ligature(nextletter, curchar);
            if (join == 0)
            {
                /* shape curchar */
                var nc = shapecount(nextletter);
                //(*len)++;
                if (nc == 1)
                {
                    which = 0; /* final or isolated */
                }
                else
                {
                    which = 2; /* medial or initial */
                }

                if (Connects_to_left(oldchar))
                {
                    which++;
                }

                which = which % curchar.Numshapes;
                curchar.Basechar = charshape(curchar.Basechar, which);

                /* get rid of oldchar */
                copycstostring(str, oldchar, level);
                oldchar = curchar; /* new values in oldchar */

                /* init new curchar */
                curchar = new Charstruct();
                curchar.Basechar = nextletter;
                curchar.Numshapes = nc;
                curchar.Lignum++;
                //          (*len) += unligature (&curchar, level);
            }
            else if (join == 1)
            {
            }
            //      else
            //        {
            //          (*len) += unligature (&curchar, level);
            //        }
            //      p = g_utf8_next_char (p);
        }

        /* Handle last char */
        if (Connects_to_left(oldchar))
        {
            which = 1;
        }
        else
        {
            which = 0;
        }

        which = which % curchar.Numshapes;
        curchar.Basechar = charshape(curchar.Basechar, which);

        /* get rid of oldchar */
        copycstostring(str, oldchar, level);
        copycstostring(str, curchar, level);
    }

    internal static void ShapeToArabicDigitsWithContext(char[] dest, int start, int length, char digitBase,
                                                        bool lastStrongWasAl)
    {
        digitBase -= '0'; // move common adjustment out of loop

        var limit = start + length;
        for (var i = start; i < limit; ++i)
        {
            var ch = dest[i];
            switch (BidiOrder.GetDirection(ch))
            {
                case BidiOrder.L:
                case BidiOrder.R:
                    lastStrongWasAl = false;
                    break;
                case BidiOrder.AL:
                    lastStrongWasAl = true;
                    break;
                case BidiOrder.EN:
                    if (lastStrongWasAl && ch <= '\u0039')
                    {
                        dest[i] = (char)(ch + digitBase);
                    }

                    break;
            }
        }
    }

    private static char charshape(char s, int which)
        /* which 0=isolated 1=final 2=initial 3=medial */
    {
        int l, r, m;
        if (s >= '\u0621' && s <= '\u06D3')
        {
            l = 0;
            r = _chartable.Length - 1;
            while (l <= r)
            {
                m = (l + r) / 2;
                if (s == _chartable[m][0])
                {
                    return _chartable[m][which + 1];
                }

                if (s < _chartable[m][0])
                {
                    r = m - 1;
                }
                else
                {
                    l = m + 1;
                }
            }
        }
        else if (s >= '\ufef5' && s <= '\ufefb')
        {
            return (char)(s + which);
        }

        return s;
    }

    private static bool Connects_to_left(Charstruct a) => a.Numshapes > 2;

    private static void copycstostring(StringBuilder str, Charstruct s, int level)
    {
        /* s is a shaped charstruct; i is the index into the string */
        if (s.Basechar == 0)
        {
            return;
        }

        str.Append(s.Basechar);
        s.Lignum--;
        if (s.Mark1 != 0)
        {
            if ((level & ar_novowel) == 0)
            {
                str.Append(s.Mark1);
                s.Lignum--;
            }
            else
            {
                s.Lignum--;
            }
        }

        if (s.Vowel != 0)
        {
            if ((level & ar_novowel) == 0)
            {
                str.Append(s.Vowel);
                s.Lignum--;
            }
            else
            {
                /* vowel elimination */
                s.Lignum--;
            }
        }
    }

    private static bool isVowel(char s) => (s >= '\u064B' && s <= '\u0655') || s == '\u0670';

    private static int ligature(char newchar, Charstruct oldchar)
    {
        /* 0 == no ligature possible; 1 == vowel; 2 == two chars; 3 == Lam+Alef */
        var retval = 0;

        if (oldchar.Basechar == 0)
        {
            return 0;
        }

        if (isVowel(newchar))
        {
            retval = 1;
            if (oldchar.Vowel != 0 && newchar != Shadda)
            {
                retval = 2; /* we eliminate the old vowel .. */
            }

            switch (newchar)
            {
                case Shadda:
                    if (oldchar.Mark1 == 0)
                    {
                        oldchar.Mark1 = Shadda;
                    }
                    else
                    {
                        return 0; /* no ligature possible */
                    }

                    break;
                case Hamzabelow:
                    switch (oldchar.Basechar)
                    {
                        case Alef:
                            oldchar.Basechar = Alefhamzabelow;
                            retval = 2;
                            break;
                        case LamAlef:
                            oldchar.Basechar = LamAlefhamzabelow;
                            retval = 2;
                            break;
                        default:
                            oldchar.Mark1 = Hamzabelow;
                            break;
                    }

                    break;
                case Hamzaabove:
                    switch (oldchar.Basechar)
                    {
                        case Alef:
                            oldchar.Basechar = Alefhamza;
                            retval = 2;
                            break;
                        case LamAlef:
                            oldchar.Basechar = LamAlefhamza;
                            retval = 2;
                            break;
                        case Waw:
                            oldchar.Basechar = Wawhamza;
                            retval = 2;
                            break;
                        case Yeh:
                        case Alefmaksura:
                        case Farsiyeh:
                            oldchar.Basechar = Yehhamza;
                            retval = 2;
                            break;
                        default: /* whatever sense this may make .. */
                            oldchar.Mark1 = Hamzaabove;
                            break;
                    }

                    break;
                case Madda:
                    switch (oldchar.Basechar)
                    {
                        case Alef:
                            oldchar.Basechar = Alefmadda;
                            retval = 2;
                            break;
                    }

                    break;
                default:
                    oldchar.Vowel = newchar;
                    break;
            }

            if (retval == 1)
            {
                oldchar.Lignum++;
            }

            return retval;
        }

        if (oldchar.Vowel != 0)
        {
            /* if we already joined a vowel, we can't join a Hamza */
            return 0;
        }

        switch (oldchar.Basechar)
        {
            case Lam:
                switch (newchar)
                {
                    case Alef:
                        oldchar.Basechar = LamAlef;
                        oldchar.Numshapes = 2;
                        retval = 3;
                        break;
                    case Alefhamza:
                        oldchar.Basechar = LamAlefhamza;
                        oldchar.Numshapes = 2;
                        retval = 3;
                        break;
                    case Alefhamzabelow:
                        oldchar.Basechar = LamAlefhamzabelow;
                        oldchar.Numshapes = 2;
                        retval = 3;
                        break;
                    case Alefmadda:
                        oldchar.Basechar = LamAlefmadda;
                        oldchar.Numshapes = 2;
                        retval = 3;
                        break;
                }

                break;
            case (char)0:
                oldchar.Basechar = newchar;
                oldchar.Numshapes = shapecount(newchar);
                retval = 1;
                break;
        }

        return retval;
    }

    private static int shapecount(char s)
    {
        int l, r, m;
        if (s >= '\u0621' && s <= '\u06D3' && !isVowel(s))
        {
            l = 0;
            r = _chartable.Length - 1;
            while (l <= r)
            {
                m = (l + r) / 2;
                if (s == _chartable[m][0])
                {
                    return _chartable[m].Length - 1;
                }

                if (s < _chartable[m][0])
                {
                    r = m - 1;
                }
                else
                {
                    l = m + 1;
                }
            }
        }
        else if (s == Zwj)
        {
            return 4;
        }

        return 1;
    }
    // '\u3f00'?

    private class Charstruct
    {
        internal char Basechar;
        internal int Lignum;
        internal char Mark1; /* has to be initialized to zero */
        internal int Numshapes = 1;

        internal char Vowel;
        /* is a ligature with lignum aditional characters */
    }
}