namespace iTextSharp.text.pdf;

/// <summary>
///     Called by  Chunk  to hyphenate a word.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public interface IHyphenationEvent
{
    /// <summary>
    ///     Gets the second part of the hyphenated word. Must be called
    ///     after  getHyphenatedWordPre() .
    /// </summary>
    /// <returns>the second part of the hyphenated word</returns>
    string HyphenatedWordPost { get; }

    /// <summary>
    ///     Gets the hyphen symbol.
    /// </summary>
    /// <returns>the hyphen symbol</returns>
    string HyphenSymbol { get; }

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
    string GetHyphenatedWordPre(string word, BaseFont font, float fontSize, float remainingWidth);
}