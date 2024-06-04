namespace iTextSharp.text;

/// <summary>
/// </summary>
public static class SpecialSymbol
{
    /// <summary>
    ///     Gets a chunk with a symbol character.
    /// </summary>
    /// <param name="c">a character that has to be changed into a symbol</param>
    /// <param name="font">Font if there is no SYMBOL character corresponding with c</param>
    /// <returns>a SYMBOL version of a character</returns>
    public static Chunk Get(char c, Font font)
    {
        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        var greek = GetCorrespondingSymbol(c);
        if (greek == ' ')
        {
            return new Chunk(c.ToString(), font);
        }

        var symbol = new Font(Font.SYMBOL, font.Size, font.Style, font.Color);
        return new Chunk(greek.ToString(), symbol);
    }

    /// <summary>
    ///     Looks for the corresponding symbol in the font Symbol.
    /// </summary>
    /// <param name="c">the original ASCII-char</param>
    /// <returns>the corresponding symbol in font Symbol</returns>
    public static char GetCorrespondingSymbol(char c)
    {
        switch (c)
        {
            case (char)913:
                return 'A'; // ALFA
            case (char)914:
                return 'B'; // BETA
            case (char)915:
                return 'G'; // GAMMA
            case (char)916:
                return 'D'; // DELTA
            case (char)917:
                return 'E'; // EPSILON
            case (char)918:
                return 'Z'; // ZETA
            case (char)919:
                return 'H'; // ETA
            case (char)920:
                return 'Q'; // THETA
            case (char)921:
                return 'I'; // IOTA
            case (char)922:
                return 'K'; // KAPPA
            case (char)923:
                return 'L'; // LAMBDA
            case (char)924:
                return 'M'; // MU
            case (char)925:
                return 'N'; // NU
            case (char)926:
                return 'X'; // XI
            case (char)927:
                return 'O'; // OMICRON
            case (char)928:
                return 'P'; // PI
            case (char)929:
                return 'R'; // RHO
            case (char)931:
                return 'S'; // SIGMA
            case (char)932:
                return 'T'; // TAU
            case (char)933:
                return 'U'; // UPSILON
            case (char)934:
                return 'F'; // PHI
            case (char)935:
                return 'C'; // CHI
            case (char)936:
                return 'Y'; // PSI
            case (char)937:
                return 'W'; // OMEGA
            case (char)945:
                return 'a'; // alfa
            case (char)946:
                return 'b'; // beta
            case (char)947:
                return 'g'; // gamma
            case (char)948:
                return 'd'; // delta
            case (char)949:
                return 'e'; // epsilon
            case (char)950:
                return 'z'; // zeta
            case (char)951:
                return 'h'; // eta
            case (char)952:
                return 'q'; // theta
            case (char)953:
                return 'i'; // iota
            case (char)954:
                return 'k'; // kappa
            case (char)955:
                return 'l'; // lambda
            case (char)956:
                return 'm'; // mu
            case (char)957:
                return 'n'; // nu
            case (char)958:
                return 'x'; // xi
            case (char)959:
                return 'o'; // omicron
            case (char)960:
                return 'p'; // pi
            case (char)961:
                return 'r'; // rho
            case (char)962:
                return 'V'; // sigma
            case (char)963:
                return 's'; // sigma
            case (char)964:
                return 't'; // tau
            case (char)965:
                return 'u'; // upsilon
            case (char)966:
                return 'f'; // phi
            case (char)967:
                return 'c'; // chi
            case (char)968:
                return 'y'; // psi
            case (char)969:
                return 'w'; // omega
            default:
                return ' ';
        }
    }

    /// <summary>
    ///     Returns the first occurrence of a special symbol in a  String .
    /// </summary>
    /// <param name="str">a  String </param>
    /// <returns>an index of -1 if no special symbol was found</returns>
    public static int Index(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        var length = str.Length;
        for (var i = 0; i < length; i++)
        {
            if (GetCorrespondingSymbol(str[i]) != ' ')
            {
                return i;
            }
        }

        return -1;
    }
}