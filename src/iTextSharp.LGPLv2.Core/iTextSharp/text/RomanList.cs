using iTextSharp.text.factories;

namespace iTextSharp.text;

/// <summary>
///     A special-version of  LIST  which use roman-letters.
///     @see com.lowagie.text.List
///     @version 2003-06-22
///     @author Michael Niedermair
/// </summary>
public class RomanList : List
{
    /// <summary>
    ///     Initialization
    /// </summary>
    public RomanList() : base(numbered: true)
    {
    }

    /// <summary>
    ///     Initialization
    /// </summary>
    /// <param name="symbolIndent">indent</param>
    public RomanList(int symbolIndent) : base(numbered: true, symbolIndent)
    {
    }

    /// <summary>
    ///     Initialization
    /// </summary>
    /// <param name="romanlower">roman-char in lowercase</param>
    /// <param name="symbolIndent">indent</param>
    public RomanList(bool romanlower, int symbolIndent) : base(numbered: true, symbolIndent) => lowercase = romanlower;

    /// <summary>
    ///     Adds an  Object  to the  List .
    /// </summary>
    /// <param name="o">the object to add.</param>
    /// <returns>true if adding the object succeeded</returns>
    public override bool Add(IElement o)
    {
        if (o is ListItem listItem)
        {
            var chunk = new Chunk(preSymbol, symbol.Font);
            chunk.Append(RomanNumberFactory.GetString(first + list.Count, lowercase));
            chunk.Append(postSymbol);
            listItem.ListSymbol = chunk;
            listItem.SetIndentationLeft(symbolIndent, autoindent);
            listItem.IndentationRight = 0;
            list.Add(listItem);

            return true;
        }

        if (o is List element)
        {
            element.IndentationLeft += symbolIndent;
            first--;
            list.Add(element);

            return true;
        }

        return false;
    }
}