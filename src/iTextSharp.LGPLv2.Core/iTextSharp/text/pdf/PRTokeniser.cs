using System;
using System.IO;
using System.Text;

namespace iTextSharp.text.pdf
{

    /// <summary>
    /// @author  Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class PrTokeniser
    {

        public const int TK_COMMENT = 4;
        public const int TK_END_ARRAY = 6;
        public const int TK_END_DIC = 8;
        public const int TK_NAME = 3;
        public const int TK_NUMBER = 1;
        public const int TK_OTHER = 10;
        public const int TK_REF = 9;
        public const int TK_START_ARRAY = 5;
        public const int TK_START_DIC = 7;
        public const int TK_STRING = 2;
        internal const string EMPTY = "";


        protected RandomAccessFileOrArray file;
        protected int generation;
        protected bool HexString;
        protected int reference;
        protected string stringValue;
        protected int Type;
        public PrTokeniser(string filename)
        {
            file = new RandomAccessFileOrArray(filename);
        }

        public PrTokeniser(byte[] pdfIn)
        {
            file = new RandomAccessFileOrArray(pdfIn);
        }

        public PrTokeniser(RandomAccessFileOrArray file)
        {
            this.file = file;
        }

        public RandomAccessFileOrArray File
        {
            get
            {
                return file;
            }
        }

        public int FilePointer
        {
            get
            {
                return file.FilePointer;
            }
        }

        public int Generation
        {
            get
            {
                return generation;
            }
        }

        public int IntValue
        {
            get
            {
                return int.Parse(stringValue);
            }
        }

        public int Length
        {
            get
            {
                return file.Length;
            }
        }

        public int Reference
        {
            get
            {
                return reference;
            }
        }

        public RandomAccessFileOrArray SafeFile
        {
            get
            {
                return new RandomAccessFileOrArray(file);
            }
        }

        public int Startxref
        {
            get
            {
                int size = Math.Min(1024, file.Length);
                int pos = file.Length - size;
                file.Seek(pos);
                string str = ReadString(1024);
                int idx = str.LastIndexOf("startxref");
                if (idx < 0)
                    throw new IOException("PDF startxref not found.");
                return pos + idx;
            }
        }

        public string StringValue
        {
            get
            {
                return stringValue;
            }
        }

        public int TokenType
        {
            get
            {
                return Type;
            }
        }

        public static int[] CheckObjectStart(byte[] line)
        {
            try
            {
                PrTokeniser tk = new PrTokeniser(line);
                int num = 0;
                int gen = 0;
                if (!tk.NextToken() || tk.TokenType != TK_NUMBER)
                    return null;
                num = tk.IntValue;
                if (!tk.NextToken() || tk.TokenType != TK_NUMBER)
                    return null;
                gen = tk.IntValue;
                if (!tk.NextToken())
                    return null;
                if (!tk.StringValue.Equals("obj"))
                    return null;
                return new[] { num, gen };
            }
            catch
            {
            }
            return null;
        }

        public static int GetHex(int v)
        {
            if (v >= '0' && v <= '9')
                return v - '0';
            if (v >= 'A' && v <= 'F')
                return v - 'A' + 10;
            if (v >= 'a' && v <= 'f')
                return v - 'a' + 10;
            return -1;
        }

        public static bool IsDelimiter(int ch)
        {
            return (ch == '(' || ch == ')' || ch == '<' || ch == '>' || ch == '[' || ch == ']' || ch == '/' || ch == '%');
        }

        public static bool IsWhitespace(int ch)
        {
            return (ch == 0 || ch == 9 || ch == 10 || ch == 12 || ch == 13 || ch == 32);
        }

        public void BackOnePosition(int ch)
        {
            if (ch != -1)
                file.PushBack((byte)ch);
        }

        public void CheckFdfHeader()
        {
            file.StartOffset = 0;
            string str = ReadString(1024);
            int idx = str.IndexOf("%FDF-1.2");
            if (idx < 0)
                throw new IOException("FDF header signature not found.");
            file.StartOffset = idx;
        }

        public char CheckPdfHeader()
        {
            file.StartOffset = 0;
            string str = ReadString(1024);
            int idx = str.IndexOf("%PDF-");
            if (idx < 0)
                throw new IOException("PDF header signature not found.");
            file.StartOffset = idx;
            return str[idx + 7];
        }

        public void Close()
        {
            file.Close();
        }

        public bool IsHexString()
        {
            return HexString;
        }

        public bool NextToken()
        {
            int ch = 0;
            do
            {
                ch = file.Read();
            } while (ch != -1 && IsWhitespace(ch));
            if (ch == -1)
                return false;
            // Note:  We have to initialize stringValue here, after we've looked for the end of the stream,
            // to ensure that we don't lose the value of a token that might end exactly at the end
            // of the stream
            StringBuilder outBuf = null;
            stringValue = EMPTY;
            switch (ch)
            {
                case '[':
                    Type = TK_START_ARRAY;
                    break;
                case ']':
                    Type = TK_END_ARRAY;
                    break;
                case '/':
                    {
                        outBuf = new StringBuilder();
                        Type = TK_NAME;
                        while (true)
                        {
                            ch = file.Read();
                            if (ch == -1 || IsDelimiter(ch) || IsWhitespace(ch))
                                break;
                            if (ch == '#')
                            {
                                ch = (GetHex(file.Read()) << 4) + GetHex(file.Read());
                            }
                            outBuf.Append((char)ch);
                        }
                        BackOnePosition(ch);
                        break;
                    }
                case '>':
                    ch = file.Read();
                    if (ch != '>')
                        ThrowError("'>' not expected");
                    Type = TK_END_DIC;
                    break;
                case '<':
                    {
                        int v1 = file.Read();
                        if (v1 == '<')
                        {
                            Type = TK_START_DIC;
                            break;
                        }
                        outBuf = new StringBuilder();
                        Type = TK_STRING;
                        HexString = true;
                        int v2 = 0;
                        while (true)
                        {
                            while (IsWhitespace(v1))
                                v1 = file.Read();
                            if (v1 == '>')
                                break;
                            v1 = GetHex(v1);
                            if (v1 < 0)
                                break;
                            v2 = file.Read();
                            while (IsWhitespace(v2))
                                v2 = file.Read();
                            if (v2 == '>')
                            {
                                ch = v1 << 4;
                                outBuf.Append((char)ch);
                                break;
                            }
                            v2 = GetHex(v2);
                            if (v2 < 0)
                                break;
                            ch = (v1 << 4) + v2;
                            outBuf.Append((char)ch);
                            v1 = file.Read();
                        }
                        if (v1 < 0 || v2 < 0)
                            ThrowError("Error reading string");
                        break;
                    }
                case '%':
                    Type = TK_COMMENT;
                    do
                    {
                        ch = file.Read();
                    } while (ch != -1 && ch != '\r' && ch != '\n');
                    break;
                case '(':
                    {
                        outBuf = new StringBuilder();
                        Type = TK_STRING;
                        HexString = false;
                        int nesting = 0;
                        while (true)
                        {
                            ch = file.Read();
                            if (ch == -1)
                                break;
                            if (ch == '(')
                            {
                                ++nesting;
                            }
                            else if (ch == ')')
                            {
                                --nesting;
                            }
                            else if (ch == '\\')
                            {
                                bool lineBreak = false;
                                ch = file.Read();
                                switch (ch)
                                {
                                    case 'n':
                                        ch = '\n';
                                        break;
                                    case 'r':
                                        ch = '\r';
                                        break;
                                    case 't':
                                        ch = '\t';
                                        break;
                                    case 'b':
                                        ch = '\b';
                                        break;
                                    case 'f':
                                        ch = '\f';
                                        break;
                                    case '(':
                                    case ')':
                                    case '\\':
                                        break;
                                    case '\r':
                                        lineBreak = true;
                                        ch = file.Read();
                                        if (ch != '\n')
                                            BackOnePosition(ch);
                                        break;
                                    case '\n':
                                        lineBreak = true;
                                        break;
                                    default:
                                        {
                                            if (ch < '0' || ch > '7')
                                            {
                                                break;
                                            }
                                            int octal = ch - '0';
                                            ch = file.Read();
                                            if (ch < '0' || ch > '7')
                                            {
                                                BackOnePosition(ch);
                                                ch = octal;
                                                break;
                                            }
                                            octal = (octal << 3) + ch - '0';
                                            ch = file.Read();
                                            if (ch < '0' || ch > '7')
                                            {
                                                BackOnePosition(ch);
                                                ch = octal;
                                                break;
                                            }
                                            octal = (octal << 3) + ch - '0';
                                            ch = octal & 0xff;
                                            break;
                                        }
                                }
                                if (lineBreak)
                                    continue;
                                if (ch < 0)
                                    break;
                            }
                            else if (ch == '\r')
                            {
                                ch = file.Read();
                                if (ch < 0)
                                    break;
                                if (ch != '\n')
                                {
                                    BackOnePosition(ch);
                                    ch = '\n';
                                }
                            }
                            if (nesting == -1)
                                break;
                            outBuf.Append((char)ch);
                        }
                        if (ch == -1)
                            ThrowError("Error reading string");
                        break;
                    }
                default:
                    {
                        outBuf = new StringBuilder();
                        if (ch == '-' || ch == '+' || ch == '.' || (ch >= '0' && ch <= '9'))
                        {
                            Type = TK_NUMBER;
                            do
                            {
                                outBuf.Append((char)ch);
                                ch = file.Read();
                            } while (ch != -1 && ((ch >= '0' && ch <= '9') || ch == '.'));
                        }
                        else
                        {
                            Type = TK_OTHER;
                            do
                            {
                                outBuf.Append((char)ch);
                                ch = file.Read();
                            } while (ch != -1 && !IsDelimiter(ch) && !IsWhitespace(ch));
                        }
                        BackOnePosition(ch);
                        break;
                    }
            }
            if (outBuf != null)
                stringValue = outBuf.ToString();
            return true;
        }

        public void NextValidToken()
        {
            int level = 0;
            string n1 = null;
            string n2 = null;
            int ptr = 0;
            while (NextToken())
            {
                if (Type == TK_COMMENT)
                    continue;
                switch (level)
                {
                    case 0:
                        {
                            if (Type != TK_NUMBER)
                                return;
                            ptr = file.FilePointer;
                            n1 = stringValue;
                            ++level;
                            break;
                        }
                    case 1:
                        {
                            if (Type != TK_NUMBER)
                            {
                                file.Seek(ptr);
                                Type = TK_NUMBER;
                                stringValue = n1;
                                return;
                            }
                            n2 = stringValue;
                            ++level;
                            break;
                        }
                    default:
                        {
                            if (Type != TK_OTHER || !stringValue.Equals("R"))
                            {
                                file.Seek(ptr);
                                Type = TK_NUMBER;
                                stringValue = n1;
                                return;
                            }
                            Type = TK_REF;
                            reference = int.Parse(n1);
                            generation = int.Parse(n2);
                            return;
                        }
                }
            }
            // if we hit here, the file is either corrupt (stream ended unexpectedly),
            // or the last token ended exactly at the end of a stream.  This last
            // case can occur inside an Object Stream.
        }

        public int Read()
        {
            return file.Read();
        }

        public bool ReadLineSegment(byte[] input)
        {
            int c = -1;
            bool eol = false;
            int ptr = 0;
            int len = input.Length;
            // ssteward, pdftk-1.10, 040922:
            // skip initial whitespace; added this because PdfReader.RebuildXref()
            // assumes that line provided by readLineSegment does not have init. whitespace;
            if (ptr < len)
            {
                while (IsWhitespace((c = Read()))) ;
            }
            while (!eol && ptr < len)
            {
                switch (c)
                {
                    case -1:
                    case '\n':
                        eol = true;
                        break;
                    case '\r':
                        eol = true;
                        int cur = FilePointer;
                        if ((Read()) != '\n')
                        {
                            Seek(cur);
                        }
                        break;
                    default:
                        input[ptr++] = (byte)c;
                        break;
                }

                // break loop? do it before we Read() again
                if (eol || len <= ptr)
                {
                    break;
                }
                else
                {
                    c = Read();
                }
            }
            if (ptr >= len)
            {
                eol = false;
                while (!eol)
                {
                    switch (c = Read())
                    {
                        case -1:
                        case '\n':
                            eol = true;
                            break;
                        case '\r':
                            eol = true;
                            int cur = FilePointer;
                            if ((Read()) != '\n')
                            {
                                Seek(cur);
                            }
                            break;
                    }
                }
            }

            if ((c == -1) && (ptr == 0))
            {
                return false;
            }
            if (ptr + 2 <= len)
            {
                input[ptr++] = (byte)' ';
                input[ptr] = (byte)'X';
            }
            return true;
        }

        public string ReadString(int size)
        {
            StringBuilder buf = new StringBuilder();
            int ch;
            while ((size--) > 0)
            {
                ch = file.Read();
                if (ch == -1)
                    break;
                buf.Append((char)ch);
            }
            return buf.ToString();
        }

        public void Seek(int pos)
        {
            file.Seek(pos);
        }
        public void ThrowError(string error)
        {
            throw new IOException(error + " at file pointer " + file.FilePointer);
        }
    }
}