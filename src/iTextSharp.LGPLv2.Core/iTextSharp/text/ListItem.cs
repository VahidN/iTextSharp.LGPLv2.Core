namespace iTextSharp.text;

/// <summary>
///     A ListItem is a Paragraph
///     that can be added to a List.
/// </summary>
/// <example>
///     Example 1:
///     List list = new List(true, 20);
///     list.Add( new ListItem("First line") );
///     list.Add( new ListItem("The second line is longer to see what happens once the end of the line is reached. Will it
///     start on a new line?") );
///     list.Add( new ListItem("Third line") );
///     The result of this code looks like this:
///     First line
///     The second line is longer to see what happens once the end of the line is reached. Will it start on a new line?
///     Third line
///     Example 2:
///     List overview = new List(false, 10);
///     overview.Add( new ListItem("This is an item") );
///     overview.Add("This is another item");
///     The result of this code looks like this:
///     This is an item
///     This is another item
/// </example>
public class ListItem : Paragraph
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> this is the symbol that wil proceed the listitem. </summary>
    private Chunk _symbol;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a ListItem.
    /// </summary>
    public ListItem()
    {
    }

    /// <summary>
    ///     Constructs a ListItem with a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    public ListItem(float leading) : base(leading)
    {
    }

    /// <summary>
    ///     Constructs a ListItem with a certain Chunk.
    /// </summary>
    /// <param name="chunk">a Chunk</param>
    public ListItem(Chunk chunk) : base(chunk)
    {
    }

    /// <summary>
    ///     Constructs a ListItem with a certain string.
    /// </summary>
    /// <param name="str">a string</param>
    public ListItem(string str) : base(str)
    {
    }

    /// <summary>
    ///     Constructs a ListItem with a certain string
    ///     and a certain Font.
    /// </summary>
    /// <param name="str">a string</param>
    /// <param name="font">a string</param>
    public ListItem(string str, Font font) : base(str, font)
    {
    }

    /// <summary>
    ///     Constructs a ListItem with a certain Chunk
    ///     and a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="chunk">a Chunk</param>
    public ListItem(float leading, Chunk chunk) : base(leading, chunk)
    {
    }

    /// <summary>
    ///     Constructs a ListItem with a certain string
    ///     and a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="str">a string</param>
    public ListItem(float leading, string str) : base(leading, str)
    {
    }

    /// <summary>
    ///     Constructs a ListItem with a certain leading, string
    ///     and Font.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="str">a string</param>
    /// <param name="font">a Font</param>
    public ListItem(float leading, string str, Font font) : base(leading, str, font)
    {
    }

    /// <summary>
    ///     Constructs a ListItem with a certain Phrase.
    /// </summary>
    /// <param name="phrase">a Phrase</param>
    public ListItem(Phrase phrase) : base(phrase)
    {
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Get/set the listsymbol.
    /// </summary>
    /// <value>a Chunk</value>
    public Chunk ListSymbol
    {
        get => _symbol;

        set
        {
            if (_symbol == null)
            {
                _symbol = value;
                if (_symbol.Font.IsStandardFont())
                {
                    _symbol.Font = font;
                }
            }
        }
    }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public override int Type { get; } = Element.LISTITEM;

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Checks if a given tag corresponds with this object.
    /// </summary>
    /// <param name="tag">the given tag</param>
    /// <returns>true if the tag corresponds</returns>
    public new static bool IsTag(string tag) => ElementTags.LISTITEM.Equals(tag, StringComparison.Ordinal);

    /// <summary>
    ///     Sets the indentation of this paragraph on the left side.
    /// </summary>
    public void SetIndentationLeft(float indentation, bool autoindent)
    {
        if (autoindent)
        {
            IndentationLeft = ListSymbol.GetWidthPoint();
        }
        else
        {
            IndentationLeft = indentation;
        }
    }
}