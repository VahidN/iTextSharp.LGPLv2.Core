using System.IO;
using System.Text;
using System.Collections;
using System.util;
using iTextSharp.text.xml.simpleparser;
using iTextSharp.text.html;

namespace iTextSharp.text.pdf.hyphenation
{
    /// <summary>
    /// Parses the xml hyphenation pattern.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class SimplePatternParser : ISimpleXmlDocHandler
    {
        internal const int ELEM_CLASSES = 1;
        internal const int ELEM_EXCEPTIONS = 2;
        internal const int ELEM_HYPHEN = 4;
        internal const int ELEM_PATTERNS = 3;
        internal IPatternConsumer Consumer;
        internal int CurrElement;
        internal ArrayList Exception;
        internal char HyphenChar;
        internal StringBuilder Token;

        /// <summary>
        /// Creates a new instance of PatternParser2
        /// </summary>
        public SimplePatternParser()
        {
            Token = new StringBuilder();
            HyphenChar = '-';    // default
        }

        public void EndDocument()
        {
        }

        public void EndElement(string tag)
        {
            if (Token.Length > 0)
            {
                string word = Token.ToString();
                switch (CurrElement)
                {
                    case ELEM_CLASSES:
                        Consumer.AddClass(word);
                        break;
                    case ELEM_EXCEPTIONS:
                        Exception.Add(word);
                        Exception = NormalizeException(Exception);
                        Consumer.AddException(GetExceptionWord(Exception),
                                            (ArrayList)Exception.Clone());
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

        public void Parse(Stream stream, IPatternConsumer consumer)
        {
            Consumer = consumer;
            try
            {
                SimpleXmlParser.Parse(this, stream);
            }
            finally
            {
                try { stream.Dispose(); } catch { }
            }
        }

        public void StartDocument()
        {
        }

        public void StartElement(string tag, Hashtable h)
        {
            if (tag.Equals("hyphen-char"))
            {
                string hh = (string)h["value"];
                if (hh != null && hh.Length == 1)
                {
                    HyphenChar = hh[0];
                }
            }
            else if (tag.Equals("classes"))
            {
                CurrElement = ELEM_CLASSES;
            }
            else if (tag.Equals("patterns"))
            {
                CurrElement = ELEM_PATTERNS;
            }
            else if (tag.Equals("exceptions"))
            {
                CurrElement = ELEM_EXCEPTIONS;
                Exception = new ArrayList();
            }
            else if (tag.Equals("hyphen"))
            {
                if (Token.Length > 0)
                {
                    Exception.Add(Token.ToString());
                }
                Exception.Add(new Hyphen((string)h[HtmlTags.PRE],
                                                (string)h["no"],
                                                (string)h["post"]));
                CurrElement = ELEM_HYPHEN;
            }
            Token.Length = 0;
        }

        public void Text(string str)
        {
            StringTokenizer tk = new StringTokenizer(str);
            while (tk.HasMoreTokens())
            {
                string word = tk.NextToken();
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
                                            (ArrayList)Exception.Clone());
                        Exception.Clear();
                        break;
                    case ELEM_PATTERNS:
                        Consumer.AddPattern(GetPattern(word),
                                            GetInterletterValues(word));
                        break;
                }
            }
        }

        protected static string GetInterletterValues(string pat)
        {
            StringBuilder il = new StringBuilder();
            string word = pat + "a";    // add dummy letter to serve as sentinel
            int len = word.Length;
            for (int i = 0; i < len; i++)
            {
                char c = word[i];
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
            StringBuilder pat = new StringBuilder();
            int len = word.Length;
            for (int i = 0; i < len; i++)
            {
                if (!char.IsDigit(word[i]))
                {
                    pat.Append(word[i]);
                }
            }
            return pat.ToString();
        }

        protected string GetExceptionWord(ArrayList ex)
        {
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < ex.Count; i++)
            {
                object item = ex[i];
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

        protected ArrayList NormalizeException(ArrayList ex)
        {
            ArrayList res = new ArrayList();
            for (int i = 0; i < ex.Count; i++)
            {
                object item = ex[i];
                if (item is string)
                {
                    string str = (string)item;
                    StringBuilder buf = new StringBuilder();
                    for (int j = 0; j < str.Length; j++)
                    {
                        char c = str[j];
                        if (c != HyphenChar)
                        {
                            buf.Append(c);
                        }
                        else
                        {
                            res.Add(buf.ToString());
                            buf.Length = 0;
                            char[] h = new char[1];
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
}
