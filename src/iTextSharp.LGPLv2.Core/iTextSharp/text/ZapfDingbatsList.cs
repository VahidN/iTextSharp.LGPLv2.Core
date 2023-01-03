namespace iTextSharp.text;

/// <summary>
///     A special-version of  LIST  whitch use zapfdingbats-letters.
///     @see com.lowagie.text.List
///     @author Michael Niedermair and Bruno Lowagie
/// </summary>
public class ZapfDingbatsList : List
{
    /// <summary>
    ///     char-number in zapfdingbats
    /// </summary>
    protected int Zn;

    /// <summary>
    ///     Creates a ZapfDingbatsList
    /// </summary>
    /// <param name="zn">a char-number</param>
    public ZapfDingbatsList(int zn) : base(true)
    {
        Zn = zn;
        var fontsize = symbol.Font.Size;
        symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
        postSymbol = " ";
    }

    /// <summary>
    ///     Creates a ZapfDingbatsList
    /// </summary>
    /// <param name="zn">a char-number</param>
    /// <param name="symbolIndent">indent</param>
    public ZapfDingbatsList(int zn, int symbolIndent) : base(true, symbolIndent)
    {
        Zn = zn;
        var fontsize = symbol.Font.Size;
        symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
        postSymbol = " ";
    }

    /// <summary>
    ///     set the char-number
    /// </summary>
    public int CharNumber
    {
        set => Zn = value;
        get => Zn;
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
            chunk.Append(((char)Zn).ToString());
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