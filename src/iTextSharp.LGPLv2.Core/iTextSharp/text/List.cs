using iTextSharp.text.factories;

namespace iTextSharp.text;

/// <summary>
///     A List contains several ListItems.
/// </summary>
/// <example>
///     Example 1:
///     List list = new List(true, 20);
///     list.Add(new ListItem("First line"));
///     list.Add(new ListItem("The second line is longer to see what happens once the end of the line is reached. Will it
///     start on a new line?"));
///     list.Add(new ListItem("Third line"));
///     The result of this code looks like this:
///     First line
///     The second line is longer to see what happens once the end of the line is reached. Will it start on a new line?
///     Third line
///     Example 2:
///     List overview = new List(false, 10);
///     overview.Add(new ListItem("This is an item"));
///     overview.Add("This is another item");
///     The result of this code looks like this:
///     This is an item
///     This is another item
/// </example>
public class List : ITextElementArray
{
    /// <summary>
    ///     a possible value for the lettered parameter
    /// </summary>
    public const bool ALPHABETICAL = true;

    /// <summary>
    ///     a possible value for the lettered parameter
    /// </summary>
    public const bool LOWERCASE = true;

    /// <summary>
    ///     a possible value for the lettered parameter
    /// </summary>
    public const bool NUMERICAL = false;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     a possible value for the numbered parameter
    /// </summary>
    public const bool ORDERED = true;

    /// <summary>
    ///     a possible value for the numbered parameter
    /// </summary>
    public const bool UNORDERED = false;

    /// <summary>
    ///     a possible value for the lettered parameter
    /// </summary>
    public const bool UPPERCASE = false;

    /// <summary>
    ///     Indicates if the indentation of all the items has to be aligned.
    /// </summary>
    protected bool alignindent;

    /// <summary>
    ///     Indicates if the indentation has to be set automatically.
    /// </summary>
    protected bool autoindent;

    /// <summary> This variable indicates the first number of a numbered list. </summary>
    protected int first = 1;

    /// <summary> The indentation of this list on the left side. </summary>
    protected float indentationLeft;

    /// <summary> The indentation of this list on the right side. </summary>
    protected float indentationRight;

    /// <summary>
    ///     Indicates if the listsymbols are numerical or alphabetical.
    /// </summary>
    protected bool lettered;

    /// <summary> This is the ArrayList containing the different ListItems. </summary>
    protected List<IElement> list = new();

    /// <summary>
    ///     Indicates if the listsymbols are lowercase or uppercase.
    /// </summary>
    protected bool lowercase;

    /// <summary>
    ///     Indicates if the list has to be numbered.
    /// </summary>
    protected bool numbered;

    /// <summary>
    ///     In case you are using numbered/lettered lists, this String is added after the number/letter.
    ///     @since   iText 2.1.1
    /// </summary>
    protected string postSymbol = ". ";

    /// <summary>
    ///     In case you are using numbered/lettered lists, this String is added before the number/letter.
    ///     @since   iText 2.1.1
    /// </summary>
    protected string preSymbol = "";

    /// <summary> This is the listsymbol of a list that is not numbered. </summary>
    protected Chunk symbol = new("-");

    /// <summary> The indentation of the listitems. </summary>
    protected float symbolIndent;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  List .
    /// </summary>
    public List() : this(false, false)
    {
    }

    /// <summary>
    ///     Constructs a  List  with a specific symbol indentation.
    ///     @since   iText 2.0.8
    /// </summary>
    /// <param name="symbolIndent">the symbol indentation</param>
    public List(float symbolIndent) => this.symbolIndent = symbolIndent;

    /// <summary>
    ///     Constructs a  List .
    /// </summary>
    /// <param name="numbered">a bool</param>
    public List(bool numbered) : this(numbered, false)
    {
    }

    /// <summary>
    ///     Constructs a  List .
    /// </summary>
    /// <param name="numbered">a bool</param>
    /// <param name="lettered">has the list to be 'numbered' with letters</param>
    public List(bool numbered, bool lettered)
    {
        this.numbered = numbered;
        this.lettered = lettered;
        autoindent = true;
        alignindent = true;
    }


    /// <summary>
    ///     Constructs a List.
    /// </summary>
    /// <remarks>
    ///     the parameter symbolIndent is important for instance when
    ///     generating PDF-documents; it indicates the indentation of the listsymbol.
    /// </remarks>
    /// <param name="numbered">a bool</param>
    /// <param name="symbolIndent">the indentation that has to be used for the listsymbol</param>
    public List(bool numbered, float symbolIndent) : this(numbered, false, symbolIndent)
    {
    }

    /// <summary>
    ///     Constructs a List.
    /// </summary>
    /// <param name="numbered">a bool</param>
    /// <param name="lettered">a bool</param>
    /// <param name="symbolIndent">the indentation that has to be used for the listsymbol</param>
    public List(bool numbered, bool lettered, float symbolIndent)
    {
        this.numbered = numbered;
        this.lettered = lettered;
        this.symbolIndent = symbolIndent;
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>

    public bool Alignindent
    {
        set => alignindent = value;
        get => alignindent;
    }

    public bool Autoindent
    {
        set => autoindent = value;
        get => autoindent;
    }

    /// <summary>
    ///     Get/set the first number
    /// </summary>
    /// <value>an int</value>
    public int First
    {
        get => first;

        set => first = value;
    }

    /// <summary>
    ///     Get/set the indentation of this paragraph on the left side.
    /// </summary>
    /// <value>the indentation</value>
    public float IndentationLeft
    {
        get => indentationLeft;

        set => indentationLeft = value;
    }

    /// <summary>
    ///     Get/set the indentation of this paragraph on the right side.
    /// </summary>
    /// <value>the indentation</value>
    public float IndentationRight
    {
        get => indentationRight;

        set => indentationRight = value;
    }

    /// <summary>
    ///     Gets all the items in the list.
    /// </summary>
    /// <value>an ArrayList containing ListItems</value>
    public IList<IElement> Items => list;

    public bool Lettered
    {
        set => lettered = value;
        get => lettered;
    }

    /// <summary>
    ///     Sets the symbol
    /// </summary>
    /// <value>a Chunk</value>
    public Chunk ListSymbol
    {
        set => symbol = value;
    }

    public bool Lowercase
    {
        set => lowercase = value;
        get => lowercase;
    }

    public bool Numbered
    {
        set => numbered = value;
        get => numbered;
    }

    /// <summary>
    ///     Sets the String that has to be added after a number or letter in the list symbol.
    ///     @since	iText 2.1.1
    /// </summary>
    public string PostSymbol
    {
        set => postSymbol = value;
        get => postSymbol;
    }

    /// <summary>
    ///     Sets the String that has to be added before a number or letter in the list symbol.
    ///     @since	iText 2.1.1
    /// </summary>
    public string PreSymbol
    {
        set => preSymbol = value;
        get => preSymbol;
    }

    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Gets the size of the list.
    /// </summary>
    /// <value>a size</value>
    public int Size => list.Count;

    /// <summary>
    ///     Get/set the symbol indentation.
    /// </summary>
    /// <value>a Chunk</value>
    public Chunk Symbol
    {
        get => symbol;
        set => symbol = value;
    }

    /// <summary>
    ///     Gets the symbol indentation.
    /// </summary>
    /// <value>the symbol indentation</value>
    public float SymbolIndent
    {
        set => symbolIndent = value;
        get => symbolIndent;
    }

    /// <summary>
    ///     Gets the leading of the first listitem.
    /// </summary>
    /// <value>a leading</value>
    public float TotalLeading
    {
        get
        {
            if (list.Count < 1)
            {
                return -1;
            }

            var item = (ListItem)list[0];
            return item.TotalLeading;
        }
    }

    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public IList<Chunk> Chunks
    {
        get
        {
            List<Chunk> tmp = new();
            foreach (var ele in list)
            {
                tmp.AddRange(ele.Chunks);
            }

            return tmp;
        }
    }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public int Type => Element.LIST;

    /// <summary>
    ///     Adds an Object to the List.
    /// </summary>
    /// <param name="o">the object to add</param>
    /// <returns>true is successful</returns>
    public virtual bool Add(IElement o)
    {
        if (o is ListItem)
        {
            var item = (ListItem)o;
            if (numbered || lettered)
            {
                var chunk = new Chunk(preSymbol, symbol.Font);
                var index = first + list.Count;
                if (lettered)
                {
                    chunk.Append(RomanAlphabetFactory.GetString(index, lowercase));
                }
                else
                {
                    chunk.Append(index.ToString(CultureInfo.InvariantCulture));
                }

                chunk.Append(postSymbol);
                item.ListSymbol = chunk;
            }
            else
            {
                item.ListSymbol = symbol;
            }

            item.SetIndentationLeft(symbolIndent, autoindent);
            item.IndentationRight = 0;
            list.Add(item);
            return true;
        }

        if (o is List)
        {
            var nested = (List)o;
            nested.IndentationLeft = nested.IndentationLeft + symbolIndent;
            first--;
            list.Add(nested);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsNestable() => true;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     IElementListener.
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    public bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            foreach (var ele in list)
            {
                listener.Add(ele);
            }

            return true;
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Returns the String that is after a number or letter in the list symbol.
    ///     @since	iText 2.1.1
    /// </summary>
    /// <returns>String that is after a number or letter in the list symbol</returns>
    public string GetPostSymbol() => postSymbol;

    /// <summary>
    ///     Returns  true  if the list is empty.
    /// </summary>
    /// <returns> true  if the list is empty</returns>
    public virtual bool IsEmpty() => list.Count == 0;

    /// <summary>
    ///     Makes sure all the items in the list have the same indentation.
    /// </summary>
    public void NormalizeIndentation()
    {
        float max = 0;
        foreach (var o in list)
        {
            if (o is ListItem)
            {
                max = Math.Max(max, ((ListItem)o).IndentationLeft);
            }
        }

        foreach (var o in list)
        {
            if (o is ListItem)
            {
                ((ListItem)o).IndentationLeft = max;
            }
        }
    }

    /// <summary>
    ///     Sets the listsymbol.
    /// </summary>
    /// <remarks>
    ///     This is a shortcut for SetListSymbol(Chunk symbol).
    /// </remarks>
    /// <param name="symbol">a string</param>
    public void SetListSymbol(string symbol)
    {
        this.symbol = new Chunk(symbol);
    }
}