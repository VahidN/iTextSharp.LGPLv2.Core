using iTextSharp.text.factories;

namespace iTextSharp.text;

/// <summary>
///     A special-version of  LIST  whitch use greek-letters.
///     @see com.lowagie.text.List
/// </summary>
public class GreekList : List
{
    /// <summary>
    ///     Initialization
    /// </summary>
    public GreekList() : base(true)
    {
        SetGreekFont();
    }

    /// <summary>
    ///     Initialisierung
    /// </summary>
    /// <param name="symbolIndent">indent</param>
    public GreekList(int symbolIndent) : base(true, symbolIndent)
    {
        SetGreekFont();
    }

    /// <summary>
    ///     Initialisierung
    /// </summary>
    /// <param name="greeklower">greek-char in lowercase</param>
    /// <param name="symbolIndent">indent</param>
    public GreekList(bool greeklower, int symbolIndent) : base(true, symbolIndent)
    {
        lowercase = greeklower;
        SetGreekFont();
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
            chunk.Append(GreekAlphabetFactory.GetString(first + list.Count, lowercase));
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

    /// <summary>
    ///     change the font to SYMBOL
    /// </summary>
    protected void SetGreekFont()
    {
        var fontsize = symbol.Font.Size;
        symbol.Font = FontFactory.GetFont(FontFactory.SYMBOL, fontsize, Font.NORMAL);
    }
}