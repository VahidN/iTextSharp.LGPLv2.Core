namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     This interface is used to connect the XML pattern file parser to
///     the hyphenation tree.
///     @author Carlos Villegas
/// </summary>
public interface IPatternConsumer
{
    /// <summary>
    ///     Add a character class.
    ///     A character class defines characters that are considered
    ///     equivalent for the purpose of hyphenation (e.g. "aA"). It
    ///     usually means to ignore case.
    /// </summary>
    /// <param name="chargroup">character group</param>
    void AddClass(string chargroup);

    /// <summary>
    ///     Add a hyphenation exception. An exception replaces the
    ///     result obtained by the algorithm for cases for which this
    ///     fails or the user wants to provide his own hyphenation.
    ///     A hyphenatedword is a vector of alternating String's and
    ///     {@link Hyphen Hyphen} instances
    /// </summary>
    void AddException(string word, List<object> hyphenatedword);

    /// <summary>
    ///     Add hyphenation patterns.
    ///     digit characters.
    /// </summary>
    /// <param name="pattern">the pattern</param>
    /// <param name="values">interletter values expressed as a string of</param>
    void AddPattern(string pattern, string values);
}