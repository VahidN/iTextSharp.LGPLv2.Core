using iTextSharp.text.pdf.hyphenation;

namespace iTextSharp.text.pdf;

/// <summary>
///     Hyphenates words automatically accordingly to the language and country.
///     The hyphenator engine was taken from FOP and uses the TEX patterns. If a language
///     is not provided and a TEX pattern for it exists, it can be easily adapted.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class HyphenationAuto : IHyphenationEvent
{
    /// <summary>
    ///     The hyphenator engine.
    /// </summary>
    protected Hyphenator Hyphenator;

    /// <summary>
    ///     The second part of the hyphenated word.
    /// </summary>
    protected string Post;

    /// <summary>
    ///     Creates a new hyphenation instance usable in  Chunk .
    /// </summary>
    /// <param name="lang">the language ("en" for english, for example)</param>
    /// <param name="country">the country ("GB" for Great-Britain or "none" for no country, for example)</param>
    /// <param name="leftMin">the minimun number of letters before the hyphen</param>
    /// <param name="rightMin">the minimun number of letters after the hyphen</param>
    public HyphenationAuto(string lang, string country, int leftMin, int rightMin) =>
        Hyphenator = new Hyphenator(lang, country, leftMin, rightMin);

    /// <summary>
    ///     Gets the second part of the hyphenated word. Must be called
    ///     after  getHyphenatedWordPre() .
    /// </summary>
    /// <returns>the second part of the hyphenated word</returns>
    public string HyphenatedWordPost => Post;

    /// <summary>
    ///     Gets the hyphen symbol.
    /// </summary>
    /// <returns>the hyphen symbol</returns>
    public string HyphenSymbol => "-";

    /// <summary>
    ///     Hyphenates a word and returns the first part of it. To get
    ///     the second part of the hyphenated word call  getHyphenatedWordPost() .
    ///     the hyphen symbol, if any
    /// </summary>
    /// <param name="word">the word to hyphenate</param>
    /// <param name="font">the font used by this word</param>
    /// <param name="fontSize">the font size used by this word</param>
    /// <param name="remainingWidth">the width available to fit this word in</param>
    /// <returns>the first part of the hyphenated word including</returns>
    public string GetHyphenatedWordPre(string word, BaseFont font, float fontSize, float remainingWidth)
    {
        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        Post = word;
        var hyphen = HyphenSymbol;
        var hyphenWidth = font.GetWidthPoint(hyphen, fontSize);
        if (hyphenWidth > remainingWidth)
        {
            return "";
        }

        var hyphenation = Hyphenator.Hyphenate(word);
        if (hyphenation == null)
        {
            return "";
        }

        var len = hyphenation.Length;
        int k;
        for (k = 0; k < len; ++k)
        {
            if (font.GetWidthPoint(hyphenation.GetPreHyphenText(k), fontSize) + hyphenWidth > remainingWidth)
            {
                break;
            }
        }

        --k;
        if (k < 0)
        {
            return "";
        }

        Post = hyphenation.GetPostHyphenText(k);
        return hyphenation.GetPreHyphenText(k) + hyphen;
    }
}