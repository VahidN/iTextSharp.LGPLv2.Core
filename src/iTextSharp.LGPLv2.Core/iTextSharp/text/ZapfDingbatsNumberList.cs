namespace iTextSharp.text;

/// <summary>
///     A special-version of  LIST  whitch use zapfdingbats-numbers (1..10).
///     @see com.lowagie.text.List
///     @version 2003-06-22
///     @author Michael Niedermair
/// </summary>
public class ZapfDingbatsNumberList : List
{
    /// <summary>
    ///     which type
    /// </summary>
    protected int type;

    /// <summary>
    ///     Creates a ZapdDingbatsNumberList
    /// </summary>
    /// <param name="type">the type of list</param>
    public ZapfDingbatsNumberList(int type) : base(true)
    {
        this.type = type;
        var fontsize = symbol.Font.Size;
        symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
        postSymbol = " ";
    }

    /// <summary>
    ///     Creates a ZapdDingbatsNumberList
    /// </summary>
    /// <param name="type">the type of list</param>
    /// <param name="symbolIndent">indent</param>
    public ZapfDingbatsNumberList(int type, int symbolIndent) : base(true, symbolIndent)
    {
        this.type = type;
        var fontsize = symbol.Font.Size;
        symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
        postSymbol = " ";
    }

    /// <summary>
    ///     get the type
    /// </summary>
    /// <returns>char-number</returns>
    public int NumberType
    {
        get => type;
        set => type = value;
    }

    /// <summary>
    ///     Adds an  Object  to the  List .
    /// </summary>
    /// <param name="o">the object to add.</param>
    /// <returns>true if adding the object succeeded</returns>
    public override bool Add(IElement o)
    {
        if (o is ListItem)
        {
            var item = (ListItem)o;
            var chunk = new Chunk(preSymbol, symbol.Font);
            switch (type)
            {
                case 0:
                    chunk.Append(((char)(first + list.Count + 171)).ToString());
                    break;
                case 1:
                    chunk.Append(((char)(first + list.Count + 181)).ToString());
                    break;
                case 2:
                    chunk.Append(((char)(first + list.Count + 191)).ToString());
                    break;
                default:
                    chunk.Append(((char)(first + list.Count + 201)).ToString());
                    break;
            }

            chunk.Append(postSymbol);
            item.ListSymbol = chunk;
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
}