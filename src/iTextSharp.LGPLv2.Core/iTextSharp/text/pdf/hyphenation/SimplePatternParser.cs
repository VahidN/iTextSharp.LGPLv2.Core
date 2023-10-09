using System.Text;
using System.util;
using iTextSharp.text.html;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     Parses the xml hyphenation pattern.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class SimplePatternParser : ISimpleXmlDocHandler
{
    internal const int ELEM_CLASSES = 1;
    internal const int ELEM_EXCEPTIONS = 2;
    internal const int ELEM_HYPHEN = 4;
    internal const int ELEM_PATTERNS = 3;
    internal IPatternConsumer Consumer;
    internal int CurrElement;
    internal List<object> Exception;
    internal char HyphenChar;
    internal readonly StringBuilder Token;

    /// <summary>
    ///     Creates a new instance of PatternParser2
    /// </summary>
    public SimplePatternParser()
    {
        Token = new StringBuilder();
        HyphenChar = '-'; // default
    }

    public void EndDocument()
    {
    }

    public void EndElement(string tag)
    {
        if (Token.Length > 0)
        {
            var word = Token.ToString();
            switch (CurrElement)
            {
                case ELEM_CLASSES:
                    Consumer.AddClass(word);
                    break;
                case ELEM_EXCEPTIONS:
                    Exception.Add(word);
                    Exception = NormalizeException(Exception);
                    Consumer.AddException(GetExceptionWord(Exception),
                                          new List<object>(Exception));
                    break;
                case ELEM_PATTERNS:
                    Consumer.AddPattern(GetPattern(word),
                                        GetInterletterValues(word));
                    break;
                case ELEM_HYPHEN:
                    // nothing to do
                    break;
            }

            if (CurrElement != ELEM_HYPHEN)
            {
                Token.Length = 0;
            }
        }

        if (CurrElement == ELEM_HYPHEN)
        {
            CurrElement = ELEM_EXCEPTIONS;
        }
        else
        {
            CurrElement = 0;
        }
    }

    public void StartDocument()
    {
    }

    public void Text(string str)
    {
        var tk = new StringTokenizer(str);
        while (tk.HasMoreTokens())
        {
            var word = tk.NextToken();
            // System.out.Println("\"" + word + "\"");
            switch (CurrElement)
            {
                case ELEM_CLASSES:
                    Consumer.AddClass(word);
                    break;
                case ELEM_EXCEPTIONS:
                    Exception.Add(word);
                    Exception = NormalizeException(Exception);
                    Consumer.AddException(GetExceptionWord(Exception),
                                          new List<object>(Exception));
                    Exception.Clear();
                    break;
                case ELEM_PATTERNS:
                    Consumer.AddPattern(GetPattern(word),
                                        GetInterletterValues(word));
                    break;
            }
        }
    }

    public void StartElement(string tag, INullValueDictionary<string, string> h)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (h == null)
        {
            throw new ArgumentNullException(nameof(h));
        }

        if (tag.Equals("hyphen-char", StringComparison.Ordinal))
        {
            var hh = h["value"];
            if (hh != null && hh.Length == 1)
            {
                HyphenChar = hh[0];
            }
        }
        else if (tag.Equals("classes", StringComparison.Ordinal))
        {
            CurrElement = ELEM_CLASSES;
        }
        else if (tag.Equals("patterns", StringComparison.Ordinal))
        {
            CurrElement = ELEM_PATTERNS;
        }
        else if (tag.Equals("exceptions", StringComparison.Ordinal))
        {
            CurrElement = ELEM_EXCEPTIONS;
            Exception = new List<object>();
        }
        else if (tag.Equals("hyphen", StringComparison.Ordinal))
        {
            if (Token.Length > 0)
            {
                Exception.Add(Token.ToString());
            }

            Exception.Add(new Hyphen(h[HtmlTags.PRE],
                                     h["no"],
                                     h["post"]));
            CurrElement = ELEM_HYPHEN;
        }

        Token.Length = 0;
    }

    public void Parse(Stream stream, IPatternConsumer consumer)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        Consumer = consumer;
        try
        {
            SimpleXmlParser.Parse(this, stream);
        }
        finally
        {
            try
            {
                stream.Dispose();
            }
            catch
            {
            }
        }
    }

    protected static string GetInterletterValues(string pat)
    {
        var il = new StringBuilder();
        var word = pat + "a"; // add dummy letter to serve as sentinel
        var len = word.Length;
        for (var i = 0; i < len; i++)
        {
            var c = word[i];
            if (char.IsDigit(c))
            {
                il.Append(c);
                i++;
            }
            else
            {
                il.Append('0');
            }
        }

        return il.ToString();
    }

    protected static string GetPattern(string word)
    {
        if (word == null)
        {
            throw new ArgumentNullException(nameof(word));
        }

        var pat = new StringBuilder();
        var len = word.Length;
        for (var i = 0; i < len; i++)
        {
            if (!char.IsDigit(word[i]))
            {
                pat.Append(word[i]);
            }
        }

        return pat.ToString();
    }

    protected static string GetExceptionWord(List<object> ex)
    {
        if (ex == null)
        {
            throw new ArgumentNullException(nameof(ex));
        }

        var res = new StringBuilder();
        for (var i = 0; i < ex.Count; i++)
        {
            var item = ex[i];
            if (item is string)
            {
                res.Append((string)item);
            }
            else
            {
                if (((Hyphen)item).NoBreak != null)
                {
                    res.Append(((Hyphen)item).NoBreak);
                }
            }
        }

        return res.ToString();
    }

    protected List<object> NormalizeException(List<object> ex)
    {
        if (ex == null)
        {
            throw new ArgumentNullException(nameof(ex));
        }

        var res = new List<object>();
        for (var i = 0; i < ex.Count; i++)
        {
            var item = ex[i];
            if (item is string)
            {
                var str = (string)item;
                var buf = new StringBuilder();
                for (var j = 0; j < str.Length; j++)
                {
                    var c = str[j];
                    if (c != HyphenChar)
                    {
                        buf.Append(c);
                    }
                    else
                    {
                        res.Add(buf.ToString());
                        buf.Length = 0;
                        var h = new char[1];
                        h[0] = HyphenChar;
                        // we use here hyphenChar which is not necessarily
                        // the one to be printed
                        res.Add(new Hyphen(new string(h), null, null));
                    }
                }

                if (buf.Length > 0)
                {
                    res.Add(buf.ToString());
                }
            }
            else
            {
                res.Add(item);
            }
        }

        return res;
    }
}