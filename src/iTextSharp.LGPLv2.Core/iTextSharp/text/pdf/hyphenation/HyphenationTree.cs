using System.Text;
using System.util;

namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     This tree structure stores the hyphenation patterns in an efficient
///     way for fast lookup. It provides the provides the method to
///     hyphenate a word.
///     @author Carlos Villegas
/// </summary>
public class HyphenationTree : TernaryTree, IPatternConsumer
{
    /// <summary>
    ///     Temporary map to store interletter values on pattern loading.
    /// </summary>
    private TernaryTree _ivalues;

    /// <summary>
    ///     This map stores the character classes
    /// </summary>
    protected TernaryTree Classmap;

    /// <summary>
    ///     This map stores hyphenation exceptions
    /// </summary>
    protected INullValueDictionary<string, List<object>> Stoplist;

    /// <summary>
    ///     value space: stores the inteletter values
    /// </summary>
    protected ByteVector Vspace;

    public HyphenationTree()
    {
        Stoplist = new NullValueDictionary<string, List<object>>(23); // usually a small table
        Classmap = new TernaryTree();
        Vspace = new ByteVector();
        Vspace.Alloc(1); // this reserves index 0, which we don't use
    }

    /// <summary>
    ///     Add a character class to the tree. It is used by
    ///     {@link SimplePatternParser SimplePatternParser} as callback to
    ///     add character classes. Character classes define the
    ///     valid word characters for hyphenation. If a word contains
    ///     a character not defined in any of the classes, it is not hyphenated.
    ///     It also defines a way to normalize the characters in order
    ///     to compare them with the stored patterns. Usually pattern
    ///     files use only lower case characters, in this case a class
    ///     for letter 'a', for example, should be defined as "aA", the first
    ///     character being the normalization char.
    /// </summary>
    public void AddClass(string chargroup)
    {
        if (chargroup == null)
        {
            throw new ArgumentNullException(nameof(chargroup));
        }

        if (chargroup.Length > 0)
        {
            var equivChar = chargroup[0];
            var key = new char[2];
            key[1] = (char)0;
            for (var i = 0; i < chargroup.Length; i++)
            {
                key[0] = chargroup[i];
                Classmap.Insert(key, 0, equivChar);
            }
        }
    }

    /// <summary>
    ///     Add a pattern to the tree. Mainly, to be used by
    ///     {@link SimplePatternParser SimplePatternParser} class as callback to
    ///     add a pattern to the tree.
    ///     desirability and priority of hyphenating at a given point
    ///     within the pattern. It should contain only digit characters.
    ///     (i.e. '0' to '9').
    /// </summary>
    /// <param name="pattern">the hyphenation pattern</param>
    /// <param name="values">interletter weight values indicating the</param>
    public void AddPattern(string pattern, string values)
    {
        var k = _ivalues.Find(values);
        if (k <= 0)
        {
            k = PackValues(values);
            _ivalues.Insert(values, (char)k);
        }

        Insert(pattern, (char)k);
    }

    /// <summary>
    ///     Add an exception to the tree. It is used by
    ///     {@link SimplePatternParser SimplePatternParser} class as callback to
    ///     store the hyphenation exceptions.
    ///     {@link Hyphen hyphen} objects.
    /// </summary>
    /// <param name="word">normalized word</param>
    /// <param name="hyphenatedword">a vector of alternating strings and</param>
    public void AddException(string word, List<object> hyphenatedword)
    {
        Stoplist[word] = hyphenatedword;
    }

    /// <summary>
    ///     Packs the values by storing them in 4 bits, two values into a byte
    ///     Values range is from 0 to 9. We use zero as terminator,
    ///     so we'll add 1 to the value.
    ///     interletter values.
    ///     are stored.
    /// </summary>
    /// <param name="values">a string of digits from '0' to '9' representing the</param>
    /// <returns>the index into the vspace array where the packed values</returns>
    protected int PackValues(string values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        int i, n = values.Length;
        var m = (n & 1) == 1 ? (n >> 1) + 2 : (n >> 1) + 1;
        var offset = Vspace.Alloc(m);
        var va = Vspace.Arr;
        for (i = 0; i < n; i++)
        {
            var j = i >> 1;
            var v = (byte)((values[i] - '0' + 1) & 0x0f);
            if ((i & 1) == 1)
            {
                va[j + offset] = (byte)(va[j + offset] | v);
            }
            else
            {
                va[j + offset] = (byte)(v << 4); // big endian
            }
        }

        va[m - 1 + offset] = 0; // terminator
        return offset;
    }

    protected string UnpackValues(int k)
    {
        var buf = new StringBuilder();
        var v = Vspace[k++];
        while (v != 0)
        {
            var c = (char)((v >> 4) - 1 + '0');
            buf.Append(c);
            c = (char)(v & 0x0f);
            if (c == 0)
            {
                break;
            }

            c = (char)(c - 1 + '0');
            buf.Append(c);
            v = Vspace[k++];
        }

        return buf.ToString();
    }

    public void LoadSimplePatterns(Stream stream)
    {
        var pp = new SimplePatternParser();
        _ivalues = new TernaryTree();

        pp.Parse(stream, this);

        // patterns/values should be now in the tree
        // let's optimize a bit
        TrimToSize();
        Vspace.TrimToSize();
        Classmap.TrimToSize();

        // get rid of the auxiliary map
        _ivalues = null;
    }


    public string FindPattern(string pat)
    {
        var k = Find(pat);
        if (k >= 0)
        {
            return UnpackValues(k);
        }

        return "";
    }

    /// <summary>
    ///     String compare, returns 0 if equal or
    ///     t is a substring of s
    /// </summary>
    protected static int Hstrcmp(char[] s, int si, char[] t, int ti)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        if (t == null)
        {
            throw new ArgumentNullException(nameof(t));
        }

        for (; s[si] == t[ti]; si++, ti++)
        {
            if (s[si] == 0)
            {
                return 0;
            }
        }

        if (t[ti] == 0)
        {
            return 0;
        }

        return s[si] - t[ti];
    }

    protected byte[] GetValues(int k)
    {
        var buf = new StringBuilder();
        var v = Vspace[k++];
        while (v != 0)
        {
            var c = (char)((v >> 4) - 1);
            buf.Append(c);
            c = (char)(v & 0x0f);
            if (c == 0)
            {
                break;
            }

            c = (char)(c - 1);
            buf.Append(c);
            v = Vspace[k++];
        }

        var res = new byte[buf.Length];
        for (var i = 0; i < res.Length; i++)
        {
            res[i] = (byte)buf[i];
        }

        return res;
    }

    /// <summary>
    ///     Search for all possible partial matches of word starting
    ///     at index an update interletter values.
    ///     But it is done in an efficient way since the patterns are
    ///     stored in a ternary tree. In fact, this is the whole purpose
    ///     of having the tree: doing this search without having to test
    ///     every single pattern. The number of patterns for languages
    ///     such as English range from 4000 to 10000. Thus, doing thousands
    ///     of string comparisons for each word to hyphenate would be
    ///     really slow without the tree. The tradeoff is memory, but
    ///     using a ternary tree instead of a trie, almost halves the
    ///     the memory used by Lout or TeX. It's also faster than using
    ///     a hash table
    /// </summary>
    /// <param name="word">null terminated word to match</param>
    /// <param name="index">start index from word</param>
    /// <param name="il">interletter values array to update</param>
    protected void SearchPatterns(char[] word, int index, byte[] il)
    {
        if (word == null)
        {
            throw new ArgumentNullException(nameof(word));
        }

        if (il == null)
        {
            throw new ArgumentNullException(nameof(il));
        }

        byte[] values;
        var i = index;
        char p, q;
        var sp = word[i];
        p = Root;

        while (p > 0 && p < Sc.Length)
        {
            if (Sc[p] == 0xFFFF)
            {
                if (Hstrcmp(word, i, Kv.Arr, Lo[p]) == 0)
                {
                    values = GetValues(Eq[p]); // data pointer is in eq[]
                    var j = index;
                    for (var k = 0; k < values.Length; k++)
                    {
                        if (j < il.Length && values[k] > il[j])
                        {
                            il[j] = values[k];
                        }

                        j++;
                    }
                }

                return;
            }

            var d = sp - Sc[p];
            if (d == 0)
            {
                if (sp == 0)
                {
                    break;
                }

                sp = word[++i];
                p = Eq[p];
                q = p;

                // look for a pattern ending at this position by searching for
                // the null char ( splitchar == 0 )
                while (q > 0 && q < Sc.Length)
                {
                    if (Sc[q] == 0xFFFF)
                    {
                        // stop at compressed branch
                        break;
                    }

                    if (Sc[q] == 0)
                    {
                        values = GetValues(Eq[q]);
                        var j = index;
                        for (var k = 0; k < values.Length; k++)
                        {
                            if (j < il.Length && values[k] > il[j])
                            {
                                il[j] = values[k];
                            }

                            j++;
                        }

                        break;
                    }

                    q = Lo[q];

                    /*
                        * actually the code should be:
                        * q = sc[q] < 0 ? hi[q] : lo[q];
                        * but java chars are unsigned
                        */
                }
            }
            else
            {
                p = d < 0 ? Lo[p] : Hi[p];
            }
        }
    }

    /// <summary>
    ///     Hyphenate word and return a Hyphenation object.
    ///     before the hyphenation point.
    ///     the hyphenation point.
    ///     the hyphenated word or null if word is not hyphenated.
    /// </summary>
    /// <param name="word">the word to be hyphenated</param>
    /// <param name="remainCharCount">Minimum number of characters allowed</param>
    /// <param name="pushCharCount">Minimum number of characters allowed after</param>
    /// <returns>a {@link Hyphenation Hyphenation} object representing</returns>
    public Hyphenation Hyphenate(string word, int remainCharCount,
                                 int pushCharCount)
    {
        if (word == null)
        {
            throw new ArgumentNullException(nameof(word));
        }

        var w = word.ToCharArray();
        return Hyphenate(w, 0, w.Length, remainCharCount, pushCharCount);
    }

    /// <summary>
    ///     w = "****nnllllllnnn*****",
    ///     where n is a non-letter, l is a letter,
    ///     all n may be absent, the first n is at offset,
    ///     the first l is at offset + iIgnoreAtBeginning;
    ///     word = ".llllll.'\0'***",
    ///     where all l in w are copied into word.
    ///     In the first part of the routine len = w.length,
    ///     in the second part of the routine len = word.length.
    ///     Three indices are used:
    ///     Index(w), the index in w,
    ///     Index(word), the index in word,
    ///     Letterindex(word), the index in the letter part of word.
    ///     The following relations exist:
    ///     Index(w) = offset + i - 1
    ///     Index(word) = i - iIgnoreAtBeginning
    ///     Letterindex(word) = Index(word) - 1
    ///     (see first loop).
    ///     It follows that:
    ///     Index(w) - Index(word) = offset - 1 + iIgnoreAtBeginning
    ///     Index(w) = Letterindex(word) + offset + iIgnoreAtBeginning
    /// </summary>
    /// <summary>
    ///     Hyphenate word and return an array of hyphenation points.
    ///     before the hyphenation point.
    ///     the hyphenation point.
    ///     the hyphenated word or null if word is not hyphenated.
    /// </summary>
    /// <param name="w">char array that contains the word</param>
    /// <param name="offset">Offset to first character in word</param>
    /// <param name="len">Length of word</param>
    /// <param name="remainCharCount">Minimum number of characters allowed</param>
    /// <param name="pushCharCount">Minimum number of characters allowed after</param>
    /// <returns>a {@link Hyphenation Hyphenation} object representing</returns>
    public Hyphenation Hyphenate(char[] w, int offset, int len,
                                 int remainCharCount, int pushCharCount)
    {
        if (w == null)
        {
            throw new ArgumentNullException(nameof(w));
        }

        int i;
        var word = new char[len + 3];

        // normalize word
        var c = new char[2];
        var iIgnoreAtBeginning = 0;
        var iLength = len;
        var bEndOfLetters = false;
        for (i = 1; i <= len; i++)
        {
            c[0] = w[offset + i - 1];
            var nc = Classmap.Find(c, 0);
            if (nc < 0)
            {
                // found a non-letter character ...
                if (i == 1 + iIgnoreAtBeginning)
                {
                    // ... before any letter character
                    iIgnoreAtBeginning++;
                }
                else
                {
                    // ... after a letter character
                    bEndOfLetters = true;
                }

                iLength--;
            }
            else
            {
                if (!bEndOfLetters)
                {
                    word[i - iIgnoreAtBeginning] = (char)nc;
                }
                else
                {
                    return null;
                }
            }
        }

        len = iLength;
        if (len < remainCharCount + pushCharCount)
        {
            // word is too short to be hyphenated
            return null;
        }

        var result = new int[len + 1];
        var k = 0;

        // check exception list first
        var sw = new string(word, 1, len);
        if (Stoplist.TryGetValue(sw, out var hw))
        {
            // assume only simple hyphens (Hyphen.pre="-", Hyphen.post = Hyphen.no = null)
            var j = 0;
            for (i = 0; i < hw.Count; i++)
            {
                var o = hw[i];
                // j = Index(sw) = Letterindex(word)?
                // result[k] = corresponding Index(w)
                if (o is string)
                {
                    j += ((string)o).Length;
                    if (j >= remainCharCount && j < len - pushCharCount)
                    {
                        result[k++] = j + iIgnoreAtBeginning;
                    }
                }
            }
        }
        else
        {
            // use algorithm to get hyphenation points
            word[0] = '.'; // word start marker
            word[len + 1] = '.'; // word end marker
            word[len + 2] = (char)0; // null terminated
            var il = new byte[len + 3]; // initialized to zero
            for (i = 0; i < len + 1; i++)
            {
                SearchPatterns(word, i, il);
            }

            // hyphenation points are located where interletter value is odd
            // i is Letterindex(word),
            // i + 1 is Index(word),
            // result[k] = corresponding Index(w)
            for (i = 0; i < len; i++)
            {
                if ((il[i + 1] & 1) == 1 && i >= remainCharCount
                                         && i <= len - pushCharCount)
                {
                    result[k++] = i + iIgnoreAtBeginning;
                }
            }
        }


        if (k > 0)
        {
            // trim result array
            var res = new int[k];
            Array.Copy(result, 0, res, 0, k);
            return new Hyphenation(new string(w, offset, len), res);
        }

        return null;
    }

    public override void PrintStats()
    {
        Console.WriteLine("Value space size = " + Vspace.Length);
        base.PrintStats();
    }
}