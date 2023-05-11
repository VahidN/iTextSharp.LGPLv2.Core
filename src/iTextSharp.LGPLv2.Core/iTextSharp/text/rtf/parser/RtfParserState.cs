using System.Text;
using iTextSharp.text.rtf.parser.destinations;
using iTextSharp.text.rtf.parser.properties;

namespace iTextSharp.text.rtf.parser;

/// <summary>
///     The  RtfParserState  contains the state information
///     for the parser. The current state object is pushed/popped in a stack
///     when a group change is made.
///     When an open group is encountered, the current state is copied and
///     then pushed on the top of the stack
///     When a close group is encountered, the current state is overwritten with
///     the popped value from the top of the stack
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfParserState
{
    /// <summary>
    ///     The current control word handler.
    /// </summary>
    public object CtrlWordHandler = null;

    /// <summary>
    ///     Stack containing control word handlers. There could be multiple
    ///     control words in a group.
    /// </summary>
    public Stack<object> CtrlWordHandlers;

    /// <summary>
    ///     The current destination.
    /// </summary>
    public RtfDestination Destination;

    /// <summary>
    ///     The control word set as the group handler.
    /// </summary>
    public object GroupHandler;

    /// <summary>
    ///     Flag indicating if this is an extended destination \* control word
    /// </summary>
    public bool IsExtendedDestination = false;

    /// <summary>
    ///     Flag to indicate if last token was an open group token '{'
    /// </summary>
    public bool NewGroup;

    /// <summary>
    ///     The parser state.
    /// </summary>
    public int ParserState = RtfParser.PARSER_IN_UNKNOWN;

    public RtfProperty Properties;

    /// <summary>
    ///     The parsed value for the current group/control word.
    /// </summary>
    public StringBuilder Text;

    /// <summary>
    ///     The tokeniser state.
    /// </summary>
    public int TokeniserState = RtfParser.TOKENISER_STATE_IN_UNKOWN;

    /// <summary>
    ///     Default constructor
    /// </summary>
    public RtfParserState()
    {
        Text = new StringBuilder();
        CtrlWordHandlers = new Stack<object>();
        Properties = new RtfProperty();
        Destination = RtfDestinationNull.GetInstance();
        NewGroup = false;
    }

    /// <summary>
    ///     Copy constructor
    /// </summary>
    /// <param name="orig">The object to copy</param>
    public RtfParserState(RtfParserState orig)
    {
        if (orig == null)
        {
            throw new ArgumentNullException(nameof(orig));
        }

        Properties = orig.Properties;
        ParserState = orig.ParserState;
        TokeniserState = orig.TokeniserState;
        GroupHandler = null;
        Destination = orig.Destination;
        Text = new StringBuilder();
        CtrlWordHandlers = new Stack<object>();
        Destination = orig.Destination;
        NewGroup = false;
    }
}