using System.Text;
using System.util;
using iTextSharp.LGPLv2.Core.System.Encodings;

namespace iTextSharp.text.xml.simpleparser;

/// <summary>
///     A simple XML and HTML parser.  This parser is, like the SAX parser,
///     an event based parser, but with much less functionality.
///     The parser can:
///     It recognizes the encoding used
///     It recognizes all the elements' start tags and end tags
///     It lists attributes, where attribute values can be enclosed in single or double quotes
///     It recognizes the  &lt;[CDATA[ ... ]]&gt;  construct
///     It recognizes the standard entities: &amp;amp;, &amp;lt;, &amp;gt;, &amp;quot;, and &amp;apos;, as well as numeric
///     entities
///     It maps lines ending in  \r\n  and  \r  to  \n  on input, in accordance with the XML Specification, Section 2.11
///     The code is based on http://www.javaworld.com/javaworld/javatips/javatip128/ with some extra
///     code from XERCES to recognize the encoding.
/// </summary>
public sealed class SimpleXmlParser
{
    private const int AttributeEqual = 13;

    private const int AttributeKey = 12;

    private const int AttributeValue = 14;

    private const int Cdata = 7;
    private const int Comment = 8;
    private const int Entity = 10;
    private const int ExaminTag = 3;

    private const int InClosetag = 5;

    private const int Pi = 9;

    private const int Quote = 11;

    private const int SingleTag = 6;

    private const int TagEncountered = 2;

    private const int TagExamined = 4;
    private const int Text = 1;

    /// <summary>
    ///     possible states
    /// </summary>
    private const int Unknown = 0;

    /// <summary>
    ///     the attribute key.
    /// </summary>
    internal string Attributekey;

    /// <summary>
    ///     current attributes
    /// </summary>
    internal INullValueDictionary<string, string> Attributes;

    /// <summary>
    ///     the attribute value.
    /// </summary>
    internal string Attributevalue;

    /// <summary>
    ///     The current character.
    /// </summary>
    internal int Character;

    /// <summary>
    ///     the column where the current character occurs
    /// </summary>
    internal int Columns;

    /// <summary>
    ///     The handler to which we are going to forward comments.
    /// </summary>
    internal readonly ISimpleXmlDocHandlerComment comment;

    /// <summary>
    ///     The handler to which we are going to forward document content
    /// </summary>
    internal readonly ISimpleXmlDocHandler Doc;

    /// <summary>
    ///     current entity
    /// </summary>
    internal readonly StringBuilder entity = new();

    /// <summary>
    ///     was the last character equivalent to a newline?
    /// </summary>
    internal bool Eol;

    /// <summary>
    ///     Are we parsing HTML?
    /// </summary>
    internal readonly bool Html;

    /// <summary>
    ///     the line we are currently reading
    /// </summary>
    internal int Lines = 1;

    /// <summary>
    ///     Keeps track of the number of tags that are open.
    /// </summary>
    internal int Nested;

    /// <summary>
    ///     A boolean indicating if the next character should be taken into account
    ///     if it's a space character. When nospace is false, the previous character
    ///     wasn't whitespace.
    ///     @since 2.1.5
    /// </summary>
    internal bool Nowhite;

    /// <summary>
    ///     The previous character.
    /// </summary>
    internal int PreviousCharacter = -1;

    /// <summary>
    ///     the quote character that was used to open the quote.
    /// </summary>
    internal int QuoteCharacter = '"';

    /// <summary>
    ///     the state stack
    /// </summary>
    internal readonly Stack<int> Stack;

    /// <summary>
    ///     the current state
    /// </summary>
    internal int State;

    /// <summary>
    ///     current tagname
    /// </summary>
    internal string Tag;

    /// <summary>
    ///     current text (whatever is encountered between tags)
    /// </summary>
    internal readonly StringBuilder text = new();

    /// <summary>
    ///     Creates a Simple XML parser object.
    ///     Call Go(BufferedReader) immediately after creation.
    /// </summary>
    private SimpleXmlParser(ISimpleXmlDocHandler doc, ISimpleXmlDocHandlerComment comment, bool html)
    {
        Doc = doc;
        this.comment = comment;
        Html = html;
        Stack = new Stack<int>();
        State = html ? Text : Unknown;
    }

    /// <summary>
    ///     Escapes a string with the appropriated XML codes.
    /// </summary>
    /// <param name="s">the string to be escaped</param>
    /// <param name="onlyAscii">codes above 127 will always be escaped with &amp;#nn; if  true </param>
    /// <returns>the escaped string</returns>
    public static string EscapeXml(string s, bool onlyAscii)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        var cc = s.ToCharArray();
        var len = cc.Length;
        var sb = new StringBuilder();
        for (var k = 0; k < len; ++k)
        {
            int c = cc[k];
            switch (c)
            {
                case '<':
                    sb.Append("&lt;");
                    break;
                case '>':
                    sb.Append("&gt;");
                    break;
                case '&':
                    sb.Append("&amp;");
                    break;
                case '"':
                    sb.Append("&quot;");
                    break;
                case '\'':
                    sb.Append("&apos;");
                    break;
                default:
                    if (c == 0x9 || c == 0xA || c == 0xD
                        || (c >= 0x20 && c <= 0xD7FF)
                        || (c >= 0xE000 && c <= 0xFFFD)
                        || (c >= 0x10000 && c <= 0x10FFFF))
                    {
                        if (onlyAscii && c > 127)
                        {
                            sb.Append("&#").Append(c).Append(';');
                        }
                        else
                        {
                            sb.Append((char)c);
                        }
                    }

                    break;
            }
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Parses the XML document firing the events to the handler.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="doc">the document handler</param>
    /// <param name="comment"></param>
    /// <param name="r">the document. The encoding is already resolved. The reader is not closed</param>
    /// <param name="html"></param>
    public static void Parse(ISimpleXmlDocHandler doc, ISimpleXmlDocHandlerComment comment, TextReader r, bool html)
    {
        if (r == null)
        {
            throw new ArgumentNullException(nameof(r));
        }

        var parser = new SimpleXmlParser(doc, comment, html);
        parser.go(r);
    }

    /// <summary>
    ///     Parses the XML document firing the events to the handler.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="doc">the document handler</param>
    /// <param name="inp">the document. The encoding is deduced from the stream. The stream is not closed</param>
    public static void Parse(ISimpleXmlDocHandler doc, Stream inp)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        var b4 = new byte[4];
        var count = inp.Read(b4, 0, b4.Length);
        if (count != 4)
        {
            throw new IOException("Insufficient length.");
        }

        var encoding = getEncodingName(b4);
        string decl = null;
        if (encoding.Equals("UTF-8", StringComparison.Ordinal))
        {
            var sb = new StringBuilder();
            int c;
            while ((c = inp.ReadByte()) != -1)
            {
                if (c == '>')
                {
                    break;
                }

                sb.Append((char)c);
            }

            decl = sb.ToString();
        }
        else if (encoding.Equals("CP037", StringComparison.Ordinal))
        {
            using var bi = new MemoryStream();
            int c;
            while ((c = inp.ReadByte()) != -1)
            {
                if (c == 0x6e) // that's '>' in ebcdic
                {
                    break;
                }

                bi.WriteByte((byte)c);
            }

            decl = EncodingsRegistry.GetEncoding(37).GetString(bi.ToArray()); //cp037 ebcdic
        }

        if (decl != null)
        {
            decl = getDeclaredEncoding(decl);
            if (decl != null)
            {
                encoding = decl;
            }
        }

        using var streamReader = new StreamReader(inp, IanaEncodings.GetEncodingEncoding(encoding));
        Parse(doc, streamReader);
    }

    public static void Parse(ISimpleXmlDocHandler doc, TextReader r)
    {
        Parse(doc, null, r, false);
    }

    private static string getDeclaredEncoding(string decl)
    {
        if (decl == null)
        {
            return null;
        }

        var idx = decl.IndexOf("encoding", StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
        {
            return null;
        }

        var idx1 = decl.IndexOf("\"", idx, StringComparison.Ordinal);
        var idx2 = decl.IndexOf("'", idx, StringComparison.Ordinal);
        if (idx1 == idx2)
        {
            return null;
        }

        if ((idx1 < 0 && idx2 > 0) || (idx2 > 0 && idx2 < idx1))
        {
            var idx3 = decl.IndexOf("'", idx2 + 1, StringComparison.Ordinal);
            if (idx3 < 0)
            {
                return null;
            }

            return decl.Substring(idx2 + 1, idx3 - (idx2 + 1));
        }

        if ((idx2 < 0 && idx1 > 0) || (idx1 > 0 && idx1 < idx2))
        {
            var idx3 = decl.IndexOf("\"", idx1 + 1, StringComparison.Ordinal);
            if (idx3 < 0)
            {
                return null;
            }

            return decl.Substring(idx1 + 1, idx3 - (idx1 + 1));
        }

        return null;
    }

    /// <summary>
    ///     Returns the IANA encoding name that is auto-detected from
    ///     the bytes specified, with the endian-ness of that encoding where appropriate.
    ///     (method found in org.apache.xerces.impl.XMLEntityManager, originaly published
    ///     by the Apache Software Foundation under the Apache Software License; now being
    ///     used in iText under the MPL)
    /// </summary>
    /// <param name="b4">The first four bytes of the input.</param>
    /// <returns>an IANA-encoding string</returns>
    private static string getEncodingName(byte[] b4)
    {
        // UTF-16, with BOM
        var b0 = b4[0] & 0xFF;
        var b1 = b4[1] & 0xFF;
        if (b0 == 0xFE && b1 == 0xFF)
        {
            // UTF-16, big-endian
            return "UTF-16BE";
        }

        if (b0 == 0xFF && b1 == 0xFE)
        {
            // UTF-16, little-endian
            return "UTF-16LE";
        }

        // UTF-8 with a BOM
        var b2 = b4[2] & 0xFF;
        if (b0 == 0xEF && b1 == 0xBB && b2 == 0xBF)
        {
            return "UTF-8";
        }

        // other encodings
        var b3 = b4[3] & 0xFF;
        if (b0 == 0x00 && b1 == 0x00 && b2 == 0x00 && b3 == 0x3C)
        {
            // UCS-4, big endian (1234)
            return "ISO-10646-UCS-4";
        }

        if (b0 == 0x3C && b1 == 0x00 && b2 == 0x00 && b3 == 0x00)
        {
            // UCS-4, little endian (4321)
            return "ISO-10646-UCS-4";
        }

        if (b0 == 0x00 && b1 == 0x00 && b2 == 0x3C && b3 == 0x00)
        {
            // UCS-4, unusual octet order (2143)
            // REVISIT: What should this be?
            return "ISO-10646-UCS-4";
        }

        if (b0 == 0x00 && b1 == 0x3C && b2 == 0x00 && b3 == 0x00)
        {
            // UCS-4, unusual octect order (3412)
            // REVISIT: What should this be?
            return "ISO-10646-UCS-4";
        }

        if (b0 == 0x00 && b1 == 0x3C && b2 == 0x00 && b3 == 0x3F)
        {
            // UTF-16, big-endian, no BOM
            // (or could turn out to be UCS-2...
            // REVISIT: What should this be?
            return "UTF-16BE";
        }

        if (b0 == 0x3C && b1 == 0x00 && b2 == 0x3F && b3 == 0x00)
        {
            // UTF-16, little-endian, no BOM
            // (or could turn out to be UCS-2...
            return "UTF-16LE";
        }

        if (b0 == 0x4C && b1 == 0x6F && b2 == 0xA7 && b3 == 0x94)
        {
            // EBCDIC
            // a la xerces1, return CP037 instead of EBCDIC here
            return "CP037";
        }

        // default encoding
        return "UTF-8";
    }

    /// <summary>
    ///     Sets the name of the tag.
    /// </summary>
    private void doTag()
    {
        if (Tag == null)
        {
            Tag = text.ToString();
        }

        if (Html)
        {
            Tag = Tag.ToLower(CultureInfo.InvariantCulture);
        }

        text.Length = 0;
    }

    /// <summary>
    ///     Flushes the text that is currently in the buffer.
    ///     The text can be ignored, added to the document
    ///     as content or as comment,... depending on the current state.
    /// </summary>
    private void flush()
    {
        switch (State)
        {
            case Text:
            case Cdata:
                if (text.Length > 0)
                {
                    Doc.Text(text.ToString());
                }

                break;
            case Comment:
                if (comment != null)
                {
                    comment.Comment(text.ToString());
                }

                break;
            case AttributeKey:
                Attributekey = text.ToString();
                if (Html)
                {
                    Attributekey = Attributekey.ToLower(CultureInfo.InvariantCulture);
                }

                break;
            case Quote:
            case AttributeValue:
                Attributevalue = text.ToString();
                Attributes[Attributekey] = Attributevalue;
                break;
        }

        text.Length = 0;
    }

    /// <summary>
    ///     Does the actual parsing. Perform this immediately
    ///     after creating the parser object.
    /// </summary>
    private void go(TextReader reader)
    {
        Doc.StartDocument();
        while (true)
        {
            // read a new character
            if (PreviousCharacter == -1)
            {
                Character = reader.Read();
            }
            // or re-examin the previous character
            else
            {
                Character = PreviousCharacter;
                PreviousCharacter = -1;
            }

            // the end of the file was reached
            if (Character == -1)
            {
                if (Html)
                {
                    if (Html && State == Text)
                    {
                        flush();
                    }

                    Doc.EndDocument();
                }
                else
                {
                    throwException("Missing end tag");
                }

                return;
            }

            // dealing with  \n and \r
            if (Character == '\n' && Eol)
            {
                Eol = false;
                continue;
            }

            if (Eol)
            {
                Eol = false;
            }
            else if (Character == '\n')
            {
                Lines++;
                Columns = 0;
            }
            else if (Character == '\r')
            {
                Eol = true;
                Character = '\n';
                Lines++;
                Columns = 0;
            }
            else
            {
                Columns++;
            }

            switch (State)
            {
                // we are in an unknown state before there's actual content
                case Unknown:
                    if (Character == '<')
                    {
                        saveState(Text);
                        State = TagEncountered;
                    }

                    break;
                // we can encounter any content
                case Text:
                    if (Character == '<')
                    {
                        flush();
                        saveState(State);
                        State = TagEncountered;
                    }
                    else if (Character == '&')
                    {
                        saveState(State);
                        entity.Length = 0;
                        State = Entity;
                    }
                    else if (char.IsWhiteSpace((char)Character))
                    {
                        if (Nowhite)
                        {
                            text.Append((char)Character);
                        }

                        Nowhite = false;
                    }
                    else
                    {
                        text.Append((char)Character);
                        Nowhite = true;
                    }

                    break;
                // we have just seen a < and are wondering what we are looking at
                // <foo>, </foo>, <!-- ... --->, etc.
                case TagEncountered:
                    initTag();
                    if (Character == '/')
                    {
                        State = InClosetag;
                    }
                    else if (Character == '?')
                    {
                        restoreState();
                        State = Pi;
                    }
                    else
                    {
                        text.Append((char)Character);
                        State = ExaminTag;
                    }

                    break;
                // we are processing something like this <foo ... >.
                // It could still be a <!-- ... --> or something.
                case ExaminTag:
                    if (Character == '>')
                    {
                        doTag();
                        processTag(true);
                        initTag();
                        State = restoreState();
                    }
                    else if (Character == '/')
                    {
                        State = SingleTag;
                    }
                    else if (Character == '-' && text.ToString().Equals("!-", StringComparison.Ordinal))
                    {
                        flush();
                        State = Comment;
                    }
                    else if (Character == '[' && text.ToString().Equals("![CDATA", StringComparison.Ordinal))
                    {
                        flush();
                        State = Cdata;
                    }
                    else if (Character == 'E' && text.ToString().Equals("!DOCTYP", StringComparison.Ordinal))
                    {
                        flush();
                        State = Pi;
                    }
                    else if (char.IsWhiteSpace((char)Character))
                    {
                        doTag();
                        State = TagExamined;
                    }
                    else
                    {
                        text.Append((char)Character);
                    }

                    break;
                // we know the name of the tag now.
                case TagExamined:
                    if (Character == '>')
                    {
                        processTag(true);
                        initTag();
                        State = restoreState();
                    }
                    else if (Character == '/')
                    {
                        State = SingleTag;
                    }
                    else if (char.IsWhiteSpace((char)Character))
                    {
                        // empty
                    }
                    else
                    {
                        text.Append((char)Character);
                        State = AttributeKey;
                    }

                    break;

                // we are processing a closing tag: e.g. </foo>
                case InClosetag:
                    if (Character == '>')
                    {
                        doTag();
                        processTag(false);
                        if (!Html && Nested == 0)
                        {
                            return;
                        }

                        State = restoreState();
                    }
                    else
                    {
                        if (!char.IsWhiteSpace((char)Character))
                        {
                            text.Append((char)Character);
                        }
                    }

                    break;

                // we have just seen something like this: <foo a="b"/
                // and are looking for the final >.
                case SingleTag:
                    if (Character != '>')
                    {
                        throwException($"Expected > for tag: <{Tag}/>");
                    }

                    doTag();
                    processTag(true);
                    processTag(false);
                    initTag();
                    if (!Html && Nested == 0)
                    {
                        Doc.EndDocument();
                        return;
                    }

                    State = restoreState();
                    break;

                // we are processing CDATA
                case Cdata:
                    if (Character == '>'
                        && text.ToString().EndsWith("]]", StringComparison.Ordinal))
                    {
                        text.Length = text.Length - 2;
                        flush();
                        State = restoreState();
                    }
                    else
                    {
                        text.Append((char)Character);
                    }

                    break;

                // we are processing a comment.  We are inside
                // the <!-- .... --> looking for the -->.
                case Comment:
                    if (Character == '>'
                        && text.ToString().EndsWith("--", StringComparison.Ordinal))
                    {
                        text.Length = text.Length - 2;
                        flush();
                        State = restoreState();
                    }
                    else
                    {
                        text.Append((char)Character);
                    }

                    break;

                // We are inside one of these <? ... ?> or one of these <!DOCTYPE ... >
                case Pi:
                    if (Character == '>')
                    {
                        State = restoreState();
                        if (State == Text)
                        {
                            State = Unknown;
                        }
                    }

                    break;

                // we are processing an entity, e.g. &lt;, &#187;, etc.
                case Entity:
                    if (Character == ';')
                    {
                        State = restoreState();
                        var cent = entity.ToString();
                        entity.Length = 0;
                        var ce = EntitiesToUnicode.DecodeEntity(cent);
                        if (ce == '\0')
                        {
                            text.Append('&').Append(cent).Append(';');
                        }
                        else
                        {
                            text.Append(ce);
                        }
                    }
                    else if ((Character != '#' && (Character < '0' || Character > '9') &&
                              (Character < 'a' || Character > 'z')
                              && (Character < 'A' || Character > 'Z')) || entity.Length >= 7)
                    {
                        State = restoreState();
                        PreviousCharacter = Character;
                        text.Append('&').Append(entity);
                        entity.Length = 0;
                    }
                    else
                    {
                        entity.Append((char)Character);
                    }

                    break;
                // We are processing the quoted right-hand side of an element's attribute.
                case Quote:
                    if (Html && QuoteCharacter == ' ' && Character == '>')
                    {
                        flush();
                        processTag(true);
                        initTag();
                        State = restoreState();
                    }
                    else if (Html && QuoteCharacter == ' ' && char.IsWhiteSpace((char)Character))
                    {
                        flush();
                        State = TagExamined;
                    }
                    else if (Html && QuoteCharacter == ' ')
                    {
                        text.Append((char)Character);
                    }
                    else if (Character == QuoteCharacter)
                    {
                        flush();
                        State = TagExamined;
                    }
                    else if (" \r\n\u0009".IndexOf(((char)Character).ToString(), StringComparison.Ordinal) >= 0)
                    {
                        text.Append(' ');
                    }
                    else if (Character == '&')
                    {
                        saveState(State);
                        State = Entity;
                        entity.Length = 0;
                    }
                    else
                    {
                        text.Append((char)Character);
                    }

                    break;

                case AttributeKey:
                    if (char.IsWhiteSpace((char)Character))
                    {
                        flush();
                        State = AttributeEqual;
                    }
                    else if (Character == '=')
                    {
                        flush();
                        State = AttributeValue;
                    }
                    else if (Html && Character == '>')
                    {
                        text.Length = 0;
                        processTag(true);
                        initTag();
                        State = restoreState();
                    }
                    else
                    {
                        text.Append((char)Character);
                    }

                    break;

                case AttributeEqual:
                    if (Character == '=')
                    {
                        State = AttributeValue;
                    }
                    else if (char.IsWhiteSpace((char)Character))
                    {
                        // empty
                    }
                    else if (Html && Character == '>')
                    {
                        text.Length = 0;
                        processTag(true);
                        initTag();
                        State = restoreState();
                    }
                    else if (Html && Character == '/')
                    {
                        flush();
                        State = SingleTag;
                    }
                    else if (Html)
                    {
                        flush();
                        text.Append((char)Character);
                        State = AttributeKey;
                    }
                    else
                    {
                        throwException("Error in attribute processing.");
                    }

                    break;

                case AttributeValue:
                    if (Character == '"' || Character == '\'')
                    {
                        QuoteCharacter = Character;
                        State = Quote;
                    }
                    else if (char.IsWhiteSpace((char)Character))
                    {
                        // empty
                    }
                    else if (Html && Character == '>')
                    {
                        flush();
                        processTag(true);
                        initTag();
                        State = restoreState();
                    }
                    else if (Html)
                    {
                        text.Append((char)Character);
                        QuoteCharacter = ' ';
                        State = Quote;
                    }
                    else
                    {
                        throwException("Error in attribute processing");
                    }

                    break;
            }
        }
    }

    /// <summary>
    ///     Initialized the tag name and attributes.
    /// </summary>
    private void initTag()
    {
        Tag = null;
        Attributes = new NullValueDictionary<string, string>();
    }

    /// <summary>
    ///     processes the tag.
    /// </summary>
    /// <param name="start">if true we are dealing with a tag that has just been opened; if false we are closing a tag.</param>
    private void processTag(bool start)
    {
        if (start)
        {
            Nested++;
            Doc.StartElement(Tag, Attributes);
        }
        else
        {
            Nested--;
            Doc.EndElement(Tag);
        }
    }

    /// <summary>
    ///     Gets a state from the stack
    /// </summary>
    /// <returns>the previous state</returns>
    private int restoreState()
    {
        if (Stack.Count != 0)
        {
            return Stack.Pop();
        }

        return Unknown;
    }

    /// <summary>
    ///     Adds a state to the stack.
    /// </summary>
    /// <param name="s">a state to add to the stack</param>
    private void saveState(int s)
    {
        Stack.Push(s);
    }

    /// <summary>
    ///     Throws an exception
    /// </summary>
    private void throwException(string s)
    {
        throw new IOException(s + " near line " + Lines + ", column " + Columns);
    }
}