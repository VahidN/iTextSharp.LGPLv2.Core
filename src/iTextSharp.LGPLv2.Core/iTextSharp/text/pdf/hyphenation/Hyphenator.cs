using System.util;

namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     This class is the main entry point to the hyphenation package.
///     You can use only the static methods or create an instance.
///     @author Carlos Villegas
/// </summary>
public class Hyphenator
{
    private const string DefaultHyphLocation = "iTextSharp.text.pdf.hyphenation.hyph.";

    private static readonly NullValueDictionary<string, HyphenationTree> _hyphenTrees = new();

    private HyphenationTree _hyphenTree;
    private int _pushCharCount = 2;
    private int _remainCharCount = 2;

    /// <summary>
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="country"></param>
    /// <param name="leftMin"></param>
    /// <param name="rightMin"></param>
    public Hyphenator(string lang, string country, int leftMin, int rightMin)
    {
        _hyphenTree = GetHyphenationTree(lang, country);
        _remainCharCount = leftMin;
        _pushCharCount = rightMin;
    }

    /// <summary>
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="country"></param>
    /// <returns>the hyphenation tree</returns>
    public static HyphenationTree GetHyphenationTree(string lang, string country)
    {
        var key = lang;

        // check whether the country code has been used
        if (country != null && !country.Equals("none", StringComparison.Ordinal))
        {
            key += "_" + country;
        }

        // first try to find it in the cache
        if (_hyphenTrees.TryGetValue(key, out var tree))
        {
            return tree;
        }

        if (_hyphenTrees.TryGetValue(lang, out var hyphenationTree))
        {
            return hyphenationTree;
        }

        var hTree = GetResourceHyphenationTree(key);

        //if (hTree == null)
        //    hTree = GetFileHyphenationTree(key);
        // put it into the pattern cache
        if (hTree != null)
        {
            _hyphenTrees[key] = hTree;
        }

        return hTree;
    }

    /// <summary>
    /// </summary>
    /// <param name="key"></param>
    /// <returns>a hyphenation tree</returns>
    public static HyphenationTree GetResourceHyphenationTree(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        try
        {
            var stream = BaseFont.GetResourceStream(DefaultHyphLocation + key + ".xml");

            if (stream == null && key.Length > 2)
            {
                stream = BaseFont.GetResourceStream(DefaultHyphLocation + key.Substring(0, 2) + ".xml");
            }

            if (stream == null)
            {
                return null;
            }

            var hTree = new HyphenationTree();
            hTree.LoadSimplePatterns(stream);

            return hTree;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="country"></param>
    /// <param name="word"></param>
    /// <param name="leftMin"></param>
    /// <param name="rightMin"></param>
    /// <returns>a hyphenation object</returns>
    public static Hyphenation Hyphenate(string lang, string country, string word, int leftMin, int rightMin)
    {
        var hTree = GetHyphenationTree(lang, country);

        if (hTree == null)
        {
            //log.Error("Error building hyphenation tree for language "
            //                       + lang);
            return null;
        }

        return hTree.Hyphenate(word, leftMin, rightMin);
    }

    /// <summary>
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="country"></param>
    /// <param name="word"></param>
    /// <param name="offset"></param>
    /// <param name="len"></param>
    /// <param name="leftMin"></param>
    /// <param name="rightMin"></param>
    /// <returns>a hyphenation object</returns>
    public static Hyphenation Hyphenate(string lang,
        string country,
        char[] word,
        int offset,
        int len,
        int leftMin,
        int rightMin)
    {
        var hTree = GetHyphenationTree(lang, country);

        if (hTree == null)
        {
            //log.Error("Error building hyphenation tree for language "
            //                       + lang);
            return null;
        }

        return hTree.Hyphenate(word, offset, len, leftMin, rightMin);
    }

    /// <summary>
    /// </summary>
    /// <param name="word"></param>
    /// <param name="offset"></param>
    /// <param name="len"></param>
    /// <returns>a hyphenation object</returns>
    public Hyphenation Hyphenate(char[] word, int offset, int len)
    {
        if (_hyphenTree == null)
        {
            return null;
        }

        return _hyphenTree.Hyphenate(word, offset, len, _remainCharCount, _pushCharCount);
    }

    /// <summary>
    /// </summary>
    /// <param name="word"></param>
    /// <returns>a hyphenation object</returns>
    public Hyphenation Hyphenate(string word)
    {
        if (_hyphenTree == null)
        {
            return null;
        }

        return _hyphenTree.Hyphenate(word, _remainCharCount, _pushCharCount);
    }

    /// <summary>
    /// </summary>
    /// <param name="lang"></param>
    /// <param name="country"></param>
    public void SetLanguage(string lang, string country)
        => _hyphenTree = GetHyphenationTree(lang, country);

    /// <summary>
    /// </summary>
    /// <param name="min"></param>
    public void SetMinPushCharCount(int min)
        => _pushCharCount = min;

    /// <summary>
    /// </summary>
    /// <param name="min"></param>
    public void SetMinRemainCharCount(int min)
        => _remainCharCount = min;
}