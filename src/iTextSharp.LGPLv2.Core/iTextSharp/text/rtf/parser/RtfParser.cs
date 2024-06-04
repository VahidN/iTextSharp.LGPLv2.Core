using System.Text;
using iTextSharp.text.rtf.direct;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.parser.ctrlwords;
using iTextSharp.text.rtf.parser.destinations;

namespace iTextSharp.text.rtf.parser;

/// <summary>
///     The RtfParser allows the importing of RTF documents or
///     RTF document fragments. The RTF document or fragment is tokenised,
///     font and color definitions corrected and then added to
///     the document being written.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfParser
{
    /// <summary>
    ///     Destination is normal. Text is processed.
    /// </summary>
    public const int DESTINATION_NORMAL = 0;

    /// <summary>
    ///     Destination is skipping. Text is ignored.
    /// </summary>
    public const int DESTINATION_SKIP = 1;

    public const int errAssertion = -6;

    public const int errBadTable = -5;

    public const int errCtrlWordNotFound = -8;

    // RTF table (sym or prop) invalid
    // Assertion failure
    public const int errEndOfFile = -7;

    public const int errInvalidHex = -4;

    /// <summary>
    ///     RTF parser error codes
    /// </summary>
    public const int errOK = 0;

    public const int errStackOverflow = -2;

    // Everything's fine!
    public const int errStackUnderflow = -1;

    // Unmatched '}'
    // Too many '{' -- memory exhausted
    public const int errUnmatchedBrace = -3;

    /// <summary>
    ///     ERRORS
    /// </summary>
    /// <summary>
    ///     Currently the parser is in an error state.
    /// </summary>
    public const int PARSER_ERROR = (0x8 << 28) | 0x0000;

    /// <summary>
    ///     The parser reached the end of the file.
    /// </summary>
    public const int PARSER_ERROR_EOF = PARSER_ERROR | 0x0001;

    /// <summary>
    ///     Currently a blipuid control word is being parsed.
    /// </summary>
    public const int PARSER_IN_BLIPUID = PARSER_IN_DOCUMENT | 0x000013;

    /// <summary>
    ///     Currently the RTF charset is being parsed.
    /// </summary>
    public const int PARSER_IN_CHARSET = PARSER_IN_HEADER | 0x000001;

    /// <summary>
    ///     Currently the RTF color table is being parsed.
    /// </summary>
    public const int PARSER_IN_COLOR_TABLE = PARSER_IN_HEADER | 0x000006;

    /// <summary>
    ///     Currently the RTF deffont is being parsed.
    /// </summary>
    public const int PARSER_IN_DEFFONT = PARSER_IN_HEADER | 0x000002;

    /// <summary>
    ///     Currently the RTF document content is being parsed.
    /// </summary>
    public const int PARSER_IN_DOCUMENT = (0x2 << 28) | 0x000000;

    /// <summary>
    ///     Currently the RTF filetbl is being parsed.
    /// </summary>
    public const int PARSER_IN_FILE_TABLE = PARSER_IN_HEADER | 0x000005;

    /// <summary>
    ///     Currently the RTF font table is being parsed.
    /// </summary>
    public const int PARSER_IN_FONT_TABLE = PARSER_IN_HEADER | 0x000003;

    /// <summary>
    ///     Currently a RTF font table info element is being parsed.
    /// </summary>
    public const int PARSER_IN_FONT_TABLE_INFO = PARSER_IN_HEADER | 0x000004;

    /// <summary>
    ///     Currently the RTF generator is being parsed.
    /// </summary>
    public const int PARSER_IN_GENERATOR = PARSER_IN_HEADER | 0x00000C;

    /// <summary>
    ///     Currently the RTF document header is being parsed.
    /// </summary>
    public const int PARSER_IN_HEADER = (0x0 << 28) | 0x000000;

    /// <summary>
    ///     Document state values
    /// </summary>
    /// <summary>
    ///     Currently the RTF info group is being parsed.
    /// </summary>
    public const int PARSER_IN_INFO_GROUP = PARSER_IN_DOCUMENT | 0x000001;

    /// <summary>
    ///     Currently the Latent Style and Formatting usage restrictions
    /// </summary>
    public const int PARSER_IN_LATENTSTYLES = PARSER_IN_HEADER | 0x000015;

    /// <summary>
    ///     Currently the RTF listtables is being parsed.
    /// </summary>
    public const int PARSER_IN_LIST_TABLE = PARSER_IN_HEADER | 0x000008;

    /// <summary>
    ///     Currently the RTF listtable override is being parsed.
    /// </summary>
    public const int PARSER_IN_LISTOVERRIDE_TABLE = PARSER_IN_HEADER | 0x000009;

    /// <summary>
    ///     Currently the RTF Old Properties.
    /// </summary>
    public const int PARSER_IN_OLDCPROPS = PARSER_IN_HEADER | 0x00000F;

    /// <summary>
    ///     Currently the RTF Old Properties.
    /// </summary>
    public const int PARSER_IN_OLDPPROPS = PARSER_IN_HEADER | 0x000010;

    /// <summary>
    ///     Currently the RTF Old Properties.
    /// </summary>
    public const int PARSER_IN_OLDSPROPS = PARSER_IN_HEADER | 0x000013;

    /// <summary>
    ///     Currently the RTF Old Properties.
    /// </summary>
    public const int PARSER_IN_OLDTPROPS = PARSER_IN_HEADER | 0x000012;

    public const int PARSER_IN_PARAGRAPH_GROUP_PROPERTIES = PARSER_IN_HEADER | 0x000016;

    /// <summary>
    ///     Currently the RTF Paragraph group properties Table (word 2002)
    /// </summary>
    public const int PARSER_IN_PARAGRAPH_TABLE = PARSER_IN_HEADER | 0x00000E;

    /// <summary>
    ///     Currently a picprop control word is being parsed.
    /// </summary>
    public const int PARSER_IN_PICPROP = PARSER_IN_DOCUMENT | 0x000012;

    /// <summary>
    ///     Currently a pict control word is being parsed.
    /// </summary>
    public const int PARSER_IN_PICT = PARSER_IN_DOCUMENT | 0x000011;

    /// <summary>
    ///     Currently the RTF User Protection Information.
    /// </summary>
    public const int PARSER_IN_PROT_USER_TABLE = PARSER_IN_HEADER | 0x000014;

    /// <summary>
    ///     Currently the RTF revtbl is being parsed.
    /// </summary>
    public const int PARSER_IN_REV_TABLE = PARSER_IN_HEADER | 0x00000A;

    /// <summary>
    ///     Currently the RTF rsidtable is being parsed.
    /// </summary>
    public const int PARSER_IN_RSID_TABLE = PARSER_IN_HEADER | 0x0000B;

    /// <summary>
    ///     Currently a shppict control word is being parsed.
    /// </summary>
    public const int PARSER_IN_SHPPICT = PARSER_IN_DOCUMENT | 0x000010;

    /// <summary>
    ///     Header state values
    /// </summary>
    /// <summary>
    ///     Currently the RTF  stylesheet is being parsed.
    /// </summary>
    public const int PARSER_IN_STYLESHEET = PARSER_IN_HEADER | 0x000007;

    /// <summary>
    ///     Currently the parser is in an unknown state.
    /// </summary>
    public const int PARSER_IN_UNKNOWN = PARSER_ERROR | 0x0FFFFFFF;

    /// <summary>
    ///     Bitmapping:
    ///     0111 1111 1111 1111 = Unkown state
    ///     0xxx xxxx xxxx xxxx = In Header
    ///     1xxx xxxx xxxx xxxx = In Document
    ///     2xxx xxxx xxxx xxxx = Reserved
    ///     4xxx xxxx xxxx xxxx = Other
    ///     8xxx xxxx xxxx xxxx = Errors
    /// </summary>
    public const int PARSER_IN_UPR = PARSER_IN_DOCUMENT | 0x000002;

    /// <summary>
    ///     other states
    /// </summary>
    /// <summary>
    ///     The parser is at the beginning or the end of the file.
    /// </summary>
    public const int PARSER_STARTSTOP = (0x4 << 28) | 0x0001;

    /// <summary>
    ///     The RtfTokeniser is currently reading binary stream.
    /// </summary>
    public const int TOKENISER_BINARY = 0x00000003;

    /// <summary>
    ///     The RtfTokeniser is currently reading hex data.
    /// </summary>
    public const int TOKENISER_HEX = 0x00000004;

    /// <summary>
    ///     The RtfTokeniser ignore result
    /// </summary>
    public const int TOKENISER_IGNORE_RESULT = 0x00000005;

    /// <summary>
    ///     The RtfTokeniser is in its ground state. Any token may follow.
    /// </summary>
    public const int TOKENISER_NORMAL = 0x00000000;

    /// <summary>
    ///     TOKENISE VARIABLES ///////////////////
    /// </summary>
    /// <summary>
    ///     State flags use 4/28 bitmask.
    ///     First 4 bits (nibble) indicates major state. Used for unknown and error
    ///     Last 28 bits indicates the value;
    /// </summary>
    /// <summary>
    ///     The last token parsed was a slash.
    /// </summary>
    public const int TOKENISER_SKIP_BYTES = 0x00000001;

    /// <summary>
    ///     The RtfTokeniser is currently tokenising a control word.
    /// </summary>
    public const int TOKENISER_SKIP_GROUP = 0x00000002;

    /// <summary>
    ///     The RtfTokeniser is currently in error state
    /// </summary>
    public const int TOKENISER_STATE_IN_ERROR = unchecked((int)0x80000000);

    // 1000 0000 0000 0000 0000 0000 0000 0000
    /// <summary>
    ///     The RtfTokeniser is currently in an unkown state
    /// </summary>
    public const int TOKENISER_STATE_IN_UNKOWN = unchecked((int)0xFF000000);

    /// <summary>
    ///     Conversion type is a conversion. This uses the document (not rtfDoc) to add
    ///     all the elements making it a different supported documents depending on the writer used.
    /// </summary>
    public const int TYPE_CONVERT = 2;

    /// <summary>
    ///     Conversion type is an import of a partial file/fragment. Uses direct content to add everything.
    /// </summary>
    public const int TYPE_IMPORT_FRAGMENT = 1;

    /// <summary>
    ///     Conversion type is an import. Uses direct content to add everything.
    ///     This is what the original import does.
    /// </summary>
    public const int TYPE_IMPORT_FULL = 0;

    /// <summary>
    ///     Conversion type to import a document into an element. i.e. Chapter, Section, Table Cell, etc.
    ///     @since 2.1.4
    /// </summary>
    public const int TYPE_IMPORT_INTO_ELEMENT = 3;

    //16
    //17
    //18
    //19
    /// <summary>
    ///     Conversion type is unknown
    /// </summary>
    public const int TYPE_UNIDENTIFIED = -1;

    /// <summary>
    ///     The  RtfCtrlWordListener .
    /// </summary>
    private readonly List<IEventListener> _listeners = new();

    /// <summary>
    ///     When the tokeniser is Binary.
    /// </summary>
    private long _binByteCount;

    /// <summary>
    ///     When the tokeniser is set to skip bytes, binSkipByteCount is the number of bytes to skip.
    /// </summary>
    private long _binSkipByteCount;

    /// <summary>
    ///     STATS VARIABLES ///////////////////
    /// </summary>
    /// <summary>
    ///     Total bytes read.
    /// </summary>
    private long _byteCount;

    /// <summary>
    ///     Total clear text characters processed.
    /// </summary>
    private long _characterCount;

    /// <summary>
    ///     Total } encountered as a close group token.
    /// </summary>
    private long _closeGroupCount;

    /// <summary>
    ///     Conversion type. Identifies if we are doing in import or a convert.
    /// </summary>
    private int _conversionType = TYPE_IMPORT_FULL;

    // RTF ended during an open group.
    // invalid hex character found in data
    // End of file reached while reading RTF
    // control word was not found
    /// <summary>
    ///     TOKENISE VARIABLES ///////////////////
    /// </summary>
    /// <summary>
    ///     Total control words processed.
    ///     Contains both known and unknown.
    ///     ctrlWordCount  should equal
    ///     ctrlWrodHandlecCount  +  ctrlWordNotHandledCount +  ctrlWordSkippedCount
    /// </summary>
    private long _ctrlWordCount;

    /// <summary>
    ///     Total control words recognized.
    /// </summary>
    private long _ctrlWordHandledCount;

    /// <summary>
    ///     Total control words not handled.
    /// </summary>
    private long _ctrlWordNotHandledCount;

    /// <summary>
    ///     Total control words skipped.
    /// </summary>
    private long _ctrlWordSkippedCount;

    /// <summary>
    ///     The current parser state.
    /// </summary>
    private RtfParserState _currentState;

    /// <summary>
    ///     The RtfDestinationMgr object to manage destinations.
    /// </summary>
    private RtfDestinationMgr _destinationMgr;

    /// <summary>
    ///     The current document group nesting level. Used for fragments.
    /// </summary>
    private int _docGroupLevel;

    /// <summary>
    ///     The iText document to add the RTF document to.
    /// </summary>
    private Document _document;

    /// <summary>
    ///     The iText element to add the RTF document to.
    ///     @since 2.1.3
    /// </summary>
    private IElement _elem;

    /// <summary>
    ///     End date as a date.
    /// </summary>
    private DateTime _endDate;

    /// <summary>
    ///     Stop time as a long.
    /// </summary>
    private long _endTime;

    /// <summary>
    ///     The current group nesting level.
    /// </summary>
    private int _groupLevel;

    /// <summary>
    ///     Total groups skipped. Includes { and } as a group.
    /// </summary>
    private long _groupSkippedCount;

    /// <summary>
    ///     The RtfImportHeader to store imported font and color mappings in.
    /// </summary>
    private RtfImportMgr _importMgr;

    /// <summary>
    ///     STATS VARIABLES ///////////////////
    /// </summary>
    /// <summary>
    ///     Last control word and parameter processed.
    /// </summary>
    private RtfCtrlWordData _lastCtrlWordParam;

    private bool _logAppend;
    private string _logFile;
    private bool _logging;

    /// <summary>
    ///     Total { encountered as an open group token.
    /// </summary>
    private long _openGroupCount;

    /// <summary>
    ///     The pushback reader to read the input stream.
    /// </summary>
    private PushbackStream _pbReader;

    /// <summary>
    ///     The RtfDocument to add the RTF document or fragment to.
    /// </summary>
    private RtfDocument _rtfDoc;

    /// <summary>
    ///     The RtfKeywords that creates and handles keywords that are implemented.
    /// </summary>
    private RtfCtrlWordMgr _rtfKeywordMgr;

    // 1111 0000 0000 0000 0000 0000 0000 0000
    /// <summary>
    ///     When the tokeniser is set to skip to next group, this is the group indentifier to return to.
    /// </summary>
    private int _skipGroupLevel;

    /// <summary>
    ///     Stack for saving states for groups
    /// </summary>
    private Stack<RtfParserState> _stackState;

    /// <summary>
    ///     Start date as a date.
    /// </summary>
    private DateTime _startDate;

    /// <summary>
    ///     Start time as a long.
    /// </summary>
    private long _startTime;

    /// <summary>
    ///     Constructor
    ///     @since 2.1.3
    /// </summary>
    /// <param name="doc"></param>
    public RtfParser(Document doc) => _document = doc;

    public static void OutputDebug(object doc, int groupLevel, string str)
    {
        Console.Out.WriteLine(str);
        if (doc == null)
        {
            return;
        }

        if (groupLevel < 0)
        {
            groupLevel = 0;
        }

        var spaces = new string(' ', groupLevel * 2);
        if (doc is RtfDocument)
        {
            ((RtfDocument)doc).Add(new RtfDirectContent("\n" + spaces + str));
        }
        else if (doc is Document)
        {
            try
            {
                ((Document)doc).Add(new RtfDirectContent("\n" + spaces + str));
            }
            catch (DocumentException)
            {
            }
        }
    }

    /// <summary>
    ///     Adds a  EventListener  to the  RtfCtrlWordMgr .
    ///     the new EventListener.
    /// </summary>
    /// <param name="listener"></param>
    public void AddListener(IEventListener listener)
    {
        _listeners.Add(listener);
    }

    /// <summary>
    ///     Converts an RTF document to an iText document.
    ///     Usage: Create a parser object and call this method with the input stream and the iText Document object
    ///     The Reader to read the RTF file from.
    ///     The iText document that the RTF file is to be added to.
    ///     @throws IOException
    ///     On I/O errors.
    /// </summary>
    /// <param name="readerIn"></param>
    /// <param name="doc"></param>
    public void ConvertRtfDocument(Stream readerIn, Document doc)
    {
        if (readerIn == null || doc == null)
        {
            return;
        }

        init(TYPE_CONVERT, null, readerIn, doc, null);
        SetCurrentDestination(RtfDestinationMgr.DESTINATION_DOCUMENT);
        _startDate = DateTime.Now;
        _startTime = _startDate.Ticks / 10000L;
        _groupLevel = 0;
        Tokenise();
        _endDate = DateTime.Now;
        _endTime = _endDate.Ticks / 10000L;
    }

    /// <summary>
    ///     Get the conversion type.
    ///     The type of the conversion. Import or Convert.
    /// </summary>
    /// <returns></returns>
    public int GetConversionType() => _conversionType;

    /// <summary>
    ///     Get the current destination object.
    /// </summary>
    /// <returns>The current state destination</returns>
    public RtfDestination GetCurrentDestination() => _currentState.Destination;

    /// <summary>
    ///     Get a destination from the map
    ///     @para destination The string destination.
    /// </summary>
    /// <returns>The destination object from the map</returns>
    public static RtfDestination GetDestination(string destination) => RtfDestinationMgr.GetDestination(destination);

    /// <summary>
    ///     Get the Document object.
    ///     Returns the object rtfDoc.
    /// </summary>
    /// <returns></returns>
    public Document GetDocument() => _document;

    /// <summary>
    ///     Helper method to indicate if this control word was a \* control word.
    /// </summary>
    /// <returns>true if it was a \* control word, otherwise false</returns>
    public bool GetExtendedDestination() => _currentState.IsExtendedDestination;

    /// <summary>
    ///     Get the RtfImportHeader object.
    ///     Returns the object importHeader.
    /// </summary>
    /// <returns></returns>
    public RtfImportMgr GetImportManager() => _importMgr;

    /// <summary>
    ///     Gets the current group level
    ///     The current group level value.
    /// </summary>
    /// <returns></returns>
    public int GetLevel() => _groupLevel;

    /// <summary>
    ///     Get the logfile name.
    /// </summary>
    /// <returns>the logFile</returns>
    public string GetLogFile() => _logFile;

    /// <summary>
    ///     Get the current state of the parser.
    ///     The current state of the parser.
    /// </summary>
    /// <returns></returns>
    public int GetParserState() => _currentState.ParserState;

    /// <summary>
    ///     Get the RTF Document object.
    ///     Returns the object rtfDoc.
    /// </summary>
    /// <returns></returns>
    public RtfDocument GetRtfDocument() => _rtfDoc;

    /// <summary>
    ///     Get the state of the parser.
    ///     The current RtfParserState state object.
    /// </summary>
    /// <returns></returns>
    public RtfParserState GetState() => _currentState;

    /// <summary>
    ///     Get the current state of the tokeniser.
    /// </summary>
    /// <returns>The current state of the tokeniser.</returns>
    public int GetTokeniserState() => _currentState.TokeniserState;

    /// <summary>
    ///     Handles text tokens. These are either handed on to the
    ///     appropriate destination handler.
    ///     The text token to handle.
    /// </summary>
    /// <param name="nextChar"></param>
    /// <returns>errOK if ok, other if an error occurred.</returns>
    public int HandleCharacter(int nextChar)
    {
        _characterCount++; // stats

        if (GetTokeniserState() == TOKENISER_SKIP_GROUP)
        {
            return errOK;
        }

        var handled = false;

        var dest = GetCurrentDestination();
        if (dest != null)
        {
            handled = dest.HandleCharacter(nextChar);
        }

        return errOK;
    }

    /// <summary>
    ///     Handles close group tokens. (})
    /// </summary>
    /// <returns>errOK if ok, other if an error occurred.</returns>
    public int HandleCloseGroup()
    {
        var result = errOK;
        _closeGroupCount++; // stats

        if (GetTokeniserState() != TOKENISER_SKIP_GROUP)
        {
            var dest = GetCurrentDestination();
            var handled = false;

            if (dest != null)
            {
                handled = dest.HandleCloseGroup();
            }
        }

        if (_stackState.Count > 0)
        {
            _currentState = _stackState.Pop();
        }
        else
        {
            result = errStackUnderflow;
        }

        _docGroupLevel--;
        _groupLevel--;

        if (GetTokeniserState() == TOKENISER_SKIP_GROUP && _groupLevel < _skipGroupLevel)
        {
            SetTokeniserState(TOKENISER_NORMAL);
        }

        return result;
    }

    /// <summary>
    ///     Handles control word tokens. Depending on the current
    ///     state a control word can lead to a state change. When
    ///     parsing the actual document contents, certain tabled
    ///     values are remapped. i.e. colors, fonts, styles, etc.
    /// </summary>
    /// <param name="ctrlWordData">The control word to handle.</param>
    /// <returns>errOK if ok, other if an error occurred.</returns>
    public int HandleCtrlWord(RtfCtrlWordData ctrlWordData)
    {
        var result = errOK;
        _ctrlWordCount++; // stats

        if (GetTokeniserState() == TOKENISER_SKIP_GROUP)
        {
            _ctrlWordSkippedCount++;
            return result;
        }

        //      RtfDestination dest = (RtfDestination)this.GetCurrentDestination();
        //      bool handled = false;
        //      if (dest != null) {
        //          handled = dest.HandleControlWord(ctrlWordData);
        //      }

        result = _rtfKeywordMgr.HandleKeyword(ctrlWordData, _groupLevel);

        if (result == errOK)
        {
            _ctrlWordHandledCount++;
        }
        else
        {
            _ctrlWordNotHandledCount++;
            result = errOK; // hack for now.
        }

        return result;
    }

    /// <summary>
    ///     Handles open group tokens. ({)
    /// </summary>
    /// <returns>errOK if ok, other if an error occurred.</returns>
    public int HandleOpenGroup()
    {
        var result = errOK;
        _openGroupCount++; // stats
        _groupLevel++; // current group level in tokeniser
        _docGroupLevel++; // current group level in document
        if (GetTokeniserState() == TOKENISER_SKIP_GROUP)
        {
            _groupSkippedCount++;
        }

        var dest = GetCurrentDestination();
        var handled = false;

        if (dest != null)
        {
            handled = dest.HandleOpeningSubGroup();
        }

        _stackState.Push(_currentState);
        _currentState = new RtfParserState(_currentState);
        // do not set this true until after the state is pushed
        // otherwise it inserts a { where one does not belong.
        _currentState.NewGroup = true;
        dest = GetCurrentDestination();

        if (dest != null)
        {
            handled = dest.HandleOpenGroup();
        }

        return result;
    }

    /// <summary>
    ///     READER *
    /// </summary>
    /// <summary>
    ///     Imports a complete RTF document.
    ///     The Reader to read the RTF document from.
    ///     The RtfDocument to add the imported document to.
    ///     @throws IOException On I/O errors.
    /// </summary>
    /// <param name="readerIn"></param>
    /// <param name="rtfDoc"></param>
    public void ImportRtfDocument(Stream readerIn, RtfDocument rtfDoc)
    {
        if (readerIn == null || rtfDoc == null)
        {
            return;
        }

        init(TYPE_IMPORT_FULL, rtfDoc, readerIn, _document, null);
        SetCurrentDestination(RtfDestinationMgr.DESTINATION_NULL);
        _startDate = DateTime.Now;
        _startTime = _startDate.Ticks / 10000L;
        _groupLevel = 0;
        try
        {
            Tokenise();
        }
        catch
        {
        }

        _endDate = DateTime.Now;
        _endTime = _endDate.Ticks / 10000L;
    }

    /// <summary>
    ///     Imports a complete RTF document into an Element, i.e. Chapter, section, Table Cell, etc.
    ///     The Reader to read the RTF document from.
    ///     The RtfDocument to add the imported document to.
    ///     @throws IOException On I/O errors.
    ///     @since 2.1.4
    /// </summary>
    /// <param name="elem">The Element the document is to be imported into.</param>
    /// <param name="readerIn"></param>
    /// <param name="rtfDoc"></param>
    public void ImportRtfDocumentIntoElement(IElement elem, Stream readerIn, RtfDocument rtfDoc)
    {
        if (readerIn == null || rtfDoc == null || elem == null)
        {
            return;
        }

        init(TYPE_IMPORT_INTO_ELEMENT, rtfDoc, readerIn, _document, elem);
        SetCurrentDestination(RtfDestinationMgr.DESTINATION_NULL);
        _startDate = DateTime.Now;
        _startTime = _startDate.Ticks / 10000L;
        _groupLevel = 0;
        try
        {
            Tokenise();
        }
        catch
        {
        }

        _endDate = DateTime.Now;
        _endTime = _endDate.Ticks / 10000L;
    }

    /// <summary>
    ///     Imports an RTF fragment.
    ///     The Reader to read the RTF fragment from.
    ///     The RTF document to add the RTF fragment to.
    ///     The RtfImportMappings defining font and color mappings for the fragment.
    ///     @throws IOException
    ///     On I/O errors.
    /// </summary>
    /// <param name="readerIn"></param>
    /// <param name="rtfDoc"></param>
    /// <param name="importMappings"></param>
    public void ImportRtfFragment(Stream readerIn, RtfDocument rtfDoc, RtfImportMappings importMappings)
    {
        //public void ImportRtfFragment2(Reader readerIn, RtfDocument rtfDoc, RtfImportMappings importMappings) throws IOException {
        if (readerIn == null || rtfDoc == null || importMappings == null)
        {
            return;
        }

        init(TYPE_IMPORT_FRAGMENT, rtfDoc, readerIn, null, null);
        handleImportMappings(importMappings);
        SetCurrentDestination(RtfDestinationMgr.DESTINATION_DOCUMENT);
        _groupLevel = 1;
        SetParserState(PARSER_IN_DOCUMENT);
        _startDate = DateTime.Now;
        _startTime = _startDate.Ticks / 10000L;
        Tokenise();
        _endDate = DateTime.Now;
        _endTime = _endDate.Ticks / 10000L;
    }

    /// <summary>
    ///     Helper method to determin if conversion is TYPE_CONVERT
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_CONVERT
    /// </summary>
    /// <returns>true if TYPE_CONVERT, otherwise false</returns>
    public bool IsConvert() => GetConversionType() == TYPE_CONVERT;

    /// <summary>
    ///     Helper method to determin if conversion is TYPE_IMPORT_FULL or TYPE_IMPORT_FRAGMENT
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FULL
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FRAGMENT
    /// </summary>
    /// <returns>true if TYPE_CONVERT, otherwise false</returns>
    public bool IsImport() => IsImportFull() || IsImportFragment();

    /// <summary>
    ///     Helper method to determin if conversion is TYPE_IMPORT_FRAGMENT
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FRAGMENT
    /// </summary>
    /// <returns>true if TYPE_CONVERT, otherwise false</returns>
    public bool IsImportFragment() => GetConversionType() == TYPE_IMPORT_FRAGMENT;

    /// <summary>
    ///     Helper method to determin if conversion is TYPE_IMPORT_FULL
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FULL
    /// </summary>
    /// <returns>true if TYPE_CONVERT, otherwise false</returns>
    public bool IsImportFull() => GetConversionType() == TYPE_IMPORT_FULL;

    /// <summary>
    /// </summary>
    /// <returns>the logAppend</returns>
    public bool IsLogAppend() => _logAppend;

    /// <summary>
    ///     Get flag indicating if logging is on or off.
    /// </summary>
    /// <returns>the logging</returns>
    public bool IsLogging() => _logging;

    /// <summary>
    ///     Helper method to determine if this is a new group.
    /// </summary>
    /// <returns>true if this is a new group, otherwise it returns false.</returns>
    public bool IsNewGroup() => _currentState.NewGroup;

    /// <summary>
    ///     listener methods
    /// </summary>
    /// <summary>
    ///     Removes a  EventListener  from the  RtfCtrlWordMgr .
    ///     the EventListener that has to be removed.
    /// </summary>
    /// <param name="listener"></param>
    public void RemoveListener(IEventListener listener)
    {
        _listeners.Remove(listener);
    }

    /// <summary>
    ///     Set the current destination object for the current state.
    /// </summary>
    /// <param name="destination">The destination value to set.</param>
    public bool SetCurrentDestination(string destination)
    {
        var dest = RtfDestinationMgr.GetDestination(destination);
        if (dest != null)
        {
            _currentState.Destination = dest;
            return false;
        }

        SetTokeniserStateSkipGroup();
        return false;
    }

    /// <summary>
    ///     Helper method to set the extended control word flag.
    /// </summary>
    /// <param name="value">Boolean to set the value to.</param>
    /// <returns>isExtendedDestination.</returns>
    public bool SetExtendedDestination(bool value)
    {
        _currentState.IsExtendedDestination = value;
        return _currentState.IsExtendedDestination;
    }

    /// <summary>
    /// </summary>
    /// <param name="logAppend">the logAppend to set</param>
    public void SetLogAppend(bool logAppend)
    {
        _logAppend = logAppend;
    }

    /// <summary>
    ///     Set the logFile name
    /// </summary>
    /// <param name="logFile">the logFile to set</param>
    public void SetLogFile(string logFile)
    {
        _logFile = logFile;
    }

    /// <summary>
    ///     Set the logFile name
    /// </summary>
    /// <param name="logFile">the logFile to set</param>
    /// <param name="logAppend"></param>
    public void SetLogFile(string logFile, bool logAppend)
    {
        _logFile = logFile;
        SetLogAppend(logAppend);
    }

    /// <summary>
    ///     Set flag indicating if logging is on or off
    /// </summary>
    /// <param name="logging"> true  to turn on logging,  false  to turn off logging.</param>
    public void SetLogging(bool logging)
    {
        _logging = logging;
    }

    /// <summary>
    ///     Helper method to set the new group flag
    /// </summary>
    /// <param name="value">The bool value to set the flag</param>
    /// <returns>The value of newGroup</returns>
    public bool SetNewGroup(bool value)
    {
        _currentState.NewGroup = value;
        return _currentState.NewGroup;
    }

    /// <summary>
    ///     DOCUMENT CONTROL METHODS
    ///     Handles -
    ///     handleOpenGroup:    Open groups     - '{'
    ///     handleCloseGroup:   Close groups    - '}'
    ///     handleCtrlWord:     Ctrl Words      - '\...'
    ///     handleCharacter:    Characters      - Plain Text, etc.
    /// </summary>
    /// <summary>
    ///     Set the state value of the parser.
    ///     The new state for the parser
    ///     The state of the parser.
    /// </summary>
    /// <param name="newState"></param>
    /// <returns></returns>
    public int SetParserState(int newState)
    {
        _currentState.ParserState = newState;
        return _currentState.ParserState;
    }

    /// <summary>
    ///     Sets the number of bytes to skip and the state of the tokeniser.
    ///     The numbere of bytes to skip in the file.
    /// </summary>
    /// <param name="numberOfBytesToSkip"></param>
    public void SetTokeniserSkipBytes(long numberOfBytesToSkip)
    {
        SetTokeniserState(TOKENISER_SKIP_BYTES);
        _binSkipByteCount = numberOfBytesToSkip;
    }

    /// <summary>
    ///     Set the current state of the tokeniser.
    /// </summary>
    /// <param name="value">The new state of the tokeniser.</param>
    /// <returns>The state of the tokeniser.</returns>
    public int SetTokeniserState(int value)
    {
        _currentState.TokeniserState = value;
        return _currentState.TokeniserState;
    }

    /// <summary>
    ///     Sets the number of binary bytes.
    ///     The number of binary bytes.
    /// </summary>
    /// <param name="binaryCount"></param>
    public void SetTokeniserStateBinary(int binaryCount)
    {
        SetTokeniserState(TOKENISER_BINARY);
        _binByteCount = binaryCount;
    }

    /// <summary>
    ///     Sets the number of binary bytes.
    ///     The number of binary bytes.
    /// </summary>
    /// <param name="binaryCount"></param>
    public void SetTokeniserStateBinary(long binaryCount)
    {
        SetTokeniserState(TOKENISER_BINARY);
        _binByteCount = binaryCount;
    }

    /// <summary>
    ///     Set the tokeniser state to skip to the end of the group.
    ///     Sets the state to TOKENISER_SKIP_GROUP and skipGroupLevel to the current group level.
    /// </summary>
    public void SetTokeniserStateNormal()
    {
        SetTokeniserState(TOKENISER_NORMAL);
    }

    /// <summary>
    ///     Set the tokeniser state to skip to the end of the group.
    ///     Sets the state to TOKENISER_SKIP_GROUP and skipGroupLevel to the current group level.
    /// </summary>
    public void SetTokeniserStateSkipGroup()
    {
        SetTokeniserState(TOKENISER_SKIP_GROUP);
        _skipGroupLevel = _groupLevel;
    }

    /// <summary>
    ///     Read through the input file and parse the data stream into tokens.
    ///     @throws IOException on IO error.
    /// </summary>
    public void Tokenise()
    {
        var errorCode = errOK; // error code
        var nextChar = 0;
        SetTokeniserState(TOKENISER_NORMAL); // set initial tokeniser state


        while ((nextChar = _pbReader.ReadByte()) != -1)
        {
            _byteCount++;

            if (GetTokeniserState() == TOKENISER_BINARY) // if we're parsing binary data, handle it directly
            {
                if ((errorCode = parseChar(nextChar)) != errOK)
                {
                    return;
                }
            }
            else
            {
                switch (nextChar)
                {
                    case '{': // scope delimiter - Open
                        HandleOpenGroup();
                        break;
                    case '}': // scope delimiter - Close
                        HandleCloseGroup();
                        break;
                    case '\n': // noise character
                    case '\r': // noise character
                        //                      if (this.IsImport()) {
                        //                          this.rtfDoc.Add(new RtfDirectContent(new String(nextChar)));
                        //                      }
                        break;
                    case '\\': // Control word start delimiter
                        if (parseCtrlWord(_pbReader) != errOK)
                        {
                            // TODO: Indicate some type of error
                            return;
                        }

                        break;
                    default:
                        if (_groupLevel == 0)
                        {
                            // BOMs
                            break;
                        }

                        if (GetTokeniserState() == TOKENISER_HEX)
                        {
                            var hexChars = new StringBuilder();
                            hexChars.Append(nextChar);
                            if ((nextChar = _pbReader.ReadByte()) == -1)
                            {
                                return;
                            }

                            _byteCount++;
                            hexChars.Append(nextChar);
                            try
                            {
                                nextChar = int.Parse(hexChars.ToString(), NumberStyles.HexNumber,
                                                     CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                return;
                            }

                            SetTokeniserState(TOKENISER_NORMAL);
                        }

                        if ((errorCode = parseChar(nextChar)) != errOK)
                        {
                            return; // some error occurred. we should send a
                            // real error
                        }

                        break;
                } // switch (nextChar[0])
            } // end if (this.GetTokeniserState() == TOKENISER_BINARY)

            //          if (groupLevel < 1 && this.IsImportFragment()) return; //return errOK;
            //          if (groupLevel < 0 && this.IsImportFull()) return; //return errStackUnderflow;
            //          if (groupLevel < 0 && this.IsConvert()) return; //return errStackUnderflow;
        } // end while (reader.Read(nextChar) != -1)

        var dest = GetCurrentDestination();
        if (dest != null)
        {
            dest.CloseDestination();
        }
    }

    /// <summary>
    ///     Initialize the statistics values.
    /// </summary>
    protected void Init_stats()
    {
        _byteCount = 0;
        _ctrlWordCount = 0;
        _openGroupCount = 0;
        _closeGroupCount = 0;
        _characterCount = 0;
        _ctrlWordHandledCount = 0;
        _ctrlWordNotHandledCount = 0;
        _ctrlWordSkippedCount = 0;
        _groupSkippedCount = 0;
        _startTime = 0;
        _endTime = 0;
        //startDate = null;
        //endDate = null;
    }

    /// <summary>
    ///     Imports the mappings defined in the RtfImportMappings into the
    ///     RtfImportHeader of this RtfParser2.
    ///     The RtfImportMappings to import.
    /// </summary>
    /// <param name="importMappings"></param>
    private void handleImportMappings(RtfImportMappings importMappings)
    {
        foreach (var fontNr in importMappings.GetFontMappings().Keys)
        {
            _importMgr.ImportFont(fontNr, importMappings.GetFontMappings()[fontNr]);
        }

        foreach (var colorNr in importMappings.GetColorMappings().Keys)
        {
            _importMgr.ImportColor(colorNr, importMappings.GetColorMappings()[colorNr]);
        }

        foreach (var listNr in importMappings.GetListMappings().Keys)
        {
            _importMgr.ImportList(listNr, importMappings.GetListMappings()[listNr]);
        }

        foreach (var stylesheetListNr in importMappings.GetStylesheetListMappings().Keys)
        {
            var style = importMappings.GetStylesheetListMappings()[stylesheetListNr];
            var list = new List();
            list.Add(new ListItem(style));
            _importMgr.ImportStylesheetList(stylesheetListNr, list);
        }
    }

    /// <summary>
    ///     Initialize the parser object values.
    /// </summary>
    /// <param name="type">Type of conversion or import</param>
    /// <param name="rtfDoc">The  RtfDocument </param>
    /// <param name="readerIn">The input stream</param>
    /// <param name="doc">The iText  Document </param>
    /// <param name="elem"></param>
    private void init(int type, RtfDocument rtfDoc, Stream readerIn, Document doc, IElement elem)
    {
        Init_stats();
        // initialize reader to a PushbackReader
        _pbReader = Init_Reader(readerIn);

        _conversionType = type;
        _rtfDoc = rtfDoc;
        _document = doc;
        _elem = elem;
        _currentState = new RtfParserState();
        _stackState = new Stack<RtfParserState>();
        SetParserState(PARSER_STARTSTOP);
        _importMgr = new RtfImportMgr(_rtfDoc, _document);

        // get destination Mgr
        _destinationMgr = RtfDestinationMgr.GetInstance(this);
        // set the parser
        RtfDestinationMgr.SetParser(this);


        // DEBUG INFO for timing and memory usage of RtfCtrlWordMgr object
        // create multiple new RtfCtrlWordMgr objects to check timing and memory usage
        //      System.Gc();
        //      long endTime = 0;
        //      Date endDate = null;
        //      long endFree = 0;
        //      DecimalFormat df = new DecimalFormat("#,##0");
        //      Date startDate = new Date();
        //      long startTime = System.CurrentTimeMillis();
        //      long startFree = Runtime.GetRuntime().FreeMemory();
        //      System.out.Println("1:");

        _rtfKeywordMgr = new RtfCtrlWordMgr(this, _pbReader); /////////DO NOT COMMENT OUT THIS LINE ///////////

        foreach (var listener in _listeners)
        {
            if (listener is IRtfCtrlWordListener)
            {
                _rtfKeywordMgr.AddRtfCtrlWordListener((IRtfCtrlWordListener)listener);
            }
        }
        //      endFree = Runtime.GetRuntime().FreeMemory();
        //      endTime = System.CurrentTimeMillis();
        //      endDate = new Date();
        //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
        //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
        //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
        //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
        //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
        //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
        //
        //      System.Gc();
        //      System.out.Println("2:");
        //      startDate = new Date();
        //      startTime = System.CurrentTimeMillis();
        //      startFree = Runtime.GetRuntime().FreeMemory();
        //      RtfCtrlWordMgr rtfKeywordMgr2 = new RtfCtrlWordMgr(this, this.pbReader);
        //      endFree = Runtime.GetRuntime().FreeMemory();
        //      endTime = System.CurrentTimeMillis();
        //      endDate = new Date();
        //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
        //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
        //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
        //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
        //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
        //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
        //
        //      System.Gc();
        //      System.out.Println("3:");
        //      startDate = new Date();
        //      startTime = System.CurrentTimeMillis();
        //      startFree = Runtime.GetRuntime().FreeMemory();
        //      RtfCtrlWordMgr rtfKeywordMgr3 = new RtfCtrlWordMgr(this, this.pbReader);
        //      endFree = Runtime.GetRuntime().FreeMemory();
        //      endTime = System.CurrentTimeMillis();
        //      endDate = new Date();
        //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
        //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
        //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
        //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
        //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
        //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
        //
        //      System.Gc();
        //      System.out.Println("4:");
        //      startDate = new Date();
        //      startTime = System.CurrentTimeMillis();
        //      startFree = Runtime.GetRuntime().FreeMemory();
        //      RtfCtrlWordMgr rtfKeywordMgr4 = new RtfCtrlWordMgr(this, this.pbReader);
        //      endFree = Runtime.GetRuntime().FreeMemory();
        //      endTime = System.CurrentTimeMillis();
        //      endDate = new Date();
        //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
        //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
        //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
        //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
        //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
        //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
        //
        //      System.Gc();
        //      System.out.Println("5:");
        //      startDate = new Date();
        //      startTime = System.CurrentTimeMillis();
        //      startFree = Runtime.GetRuntime().FreeMemory();
        //      RtfCtrlWordMgr rtfKeywordMgr5 = new RtfCtrlWordMgr(this, this.pbReader);
        //      endFree = Runtime.GetRuntime().FreeMemory();
        //      endTime = System.CurrentTimeMillis();
        //      endDate = new Date();
        //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
        //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
        //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
        //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
        //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
        //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
        //      System.Gc();
        //      System.out.Println("At ed:");
        //      startDate = new Date();
        //      startTime = System.CurrentTimeMillis();
        //      startFree = Runtime.GetRuntime().FreeMemory();
        //      //RtfCtrlWordMgr rtfKeywordMgr6 = new RtfCtrlWordMgr(this, this.pbReader);
        //      endFree = Runtime.GetRuntime().FreeMemory();
        //      endTime = System.CurrentTimeMillis();
        //      endDate = new Date();
        //      System.out.Println("RtfCtrlWordMgr start date: " + startDate.ToLocaleString());
        //      System.out.Println("RtfCtrlWordMgr end date  : " + endDate.ToLocaleString());
        //      System.out.Println("  Elapsed time    : " + Long.ToString(endTime - startTime) + " milliseconds.");
        //      System.out.Println("Begin Constructor RtfCtrlWordMgr , free mem is " + df.Format(startFree / 1024) + "k");
        //      System.out.Println("End Constructor RtfCtrlWordMgr , free mem is " + df.Format(endFree / 1024) + "k");
        //      System.out.Println("RtfCtrlWordMgr used approximately " + df.Format((startFree - endFree) / 1024) + "k");
    }

    /// <summary>
    ///     Casts the input reader to a PushbackReader or
    ///     creates a new PushbackReader from the Reader passed in.
    ///     The reader is also transformed into a BufferedReader if necessary.
    ///     The Reader object for the input file.
    ///     PushbackReader object
    /// </summary>
    /// <param name="readerIn"></param>
    /// <returns></returns>
    private static PushbackStream Init_Reader(Stream readerIn)
    {
        if (readerIn is PushbackStream)
        {
            return (PushbackStream)readerIn;
        }

        // return the proper reader object to the parser setup
        return new PushbackStream(readerIn);
    }

    /// <summary>
    ///     TOKENISER *
    /// </summary>
    /// <summary>
    ///     Process the character and send it to the current destination.
    ///     The character to process
    ///     Returns an error code or errOK if no error.
    /// </summary>
    /// <param name="nextChar"></param>
    /// <returns></returns>
    private int parseChar(int nextChar)
    {
        // figure out where to put the character
        // needs to handle group levels for parsing
        // examples
        /*
        * {\f3\froman\fcharset2\fprq2{\*\panose 05050102010706020507}Symbol;}
        * {\f7\fswiss\fcharset0\fprq2{\*\panose 020b0604020202030204}Helv{\*\falt Arial};} <- special case!!!!
        * {\f5\froman\fcharset0 Tahoma;}
        * {\f6\froman\fcharset0 Arial Black;}
        * {\info(\author name}{\company company name}}
        * ... document text ...
        */
        if (GetTokeniserState() == TOKENISER_BINARY && --_binByteCount <= 0)
        {
            SetTokeniserStateNormal();
        }

        if (GetTokeniserState() == TOKENISER_SKIP_BYTES && --_binSkipByteCount <= 0)
        {
            SetTokeniserStateNormal();
        }

        return HandleCharacter(nextChar);
    }

    /// <summary>
    ///     Parses a keyword and it's parameter if one exists
    ///     This is a pushback reader for file input.
    ///     Returns an error code or errOK if no error.
    ///     @throws IOException
    ///     Catch any file read problem.
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private int parseCtrlWord(PushbackStream reader)
    {
        var nextChar = 0;
        var result = errOK;

        if ((nextChar = reader.ReadByte()) == -1)
        {
            return errEndOfFile;
        }

        _byteCount++;

        var parsedCtrlWord = new StringBuilder();
        var parsedParam = new StringBuilder();
        var ctrlWordParam = new RtfCtrlWordData();

        if (!char.IsLetterOrDigit((char)nextChar))
        {
            parsedCtrlWord.Append((char)nextChar);
            ctrlWordParam.CtrlWord = parsedCtrlWord.ToString();
            result = HandleCtrlWord(ctrlWordParam);
            _lastCtrlWordParam = ctrlWordParam;
            return result;
        }

        //      for ( ; Character.IsLetter(nextChar[0]); reader.Read(nextChar) ) {
        //          parsedCtrlWord.Append(nextChar[0]);
        //      }
        do
        {
            parsedCtrlWord.Append((char)nextChar);
            //TODO: catch EOF
            nextChar = reader.ReadByte();
            _byteCount++;
        } while (char.IsLetter((char)nextChar));

        ctrlWordParam.CtrlWord = parsedCtrlWord.ToString();

        if ((char)nextChar == '-')
        {
            ctrlWordParam.IsNeg = true;
            if ((nextChar = reader.ReadByte()) == -1)
            {
                return errEndOfFile;
            }

            _byteCount++;
        }

        if (char.IsDigit((char)nextChar))
        {
            ctrlWordParam.HasParam = true;
            //          for ( ; Character.IsDigit(nextChar[0]); reader.Read(nextChar) ) {
            //              parsedParam.Append(nextChar[0]);
            //          }
            do
            {
                parsedParam.Append((char)nextChar);
                //TODO: catch EOF
                nextChar = reader.ReadByte();
                _byteCount++;
            } while (char.IsDigit((char)nextChar));

            ctrlWordParam.Param = parsedParam.ToString();
        }

        // push this character back into the stream
        if ((char)nextChar != ' ')
        {
            // || this.IsImport() ) {
            reader.Unread(nextChar);
        }

        result = HandleCtrlWord(ctrlWordParam);
        _lastCtrlWordParam = ctrlWordParam;
        return result;
    }
}