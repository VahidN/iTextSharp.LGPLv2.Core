namespace iTextSharp.text
{
    /// <summary>
    /// A special-version of  LIST  whitch use zapfdingbats-letters.
    /// @see com.lowagie.text.List
    /// @author Michael Niedermair and Bruno Lowagie
    /// </summary>
    public class ZapfDingbatsList : List
    {
        /// <summary>
        /// char-number in zapfdingbats
        /// </summary>
        protected int Zn;

        /// <summary>
        /// Creates a ZapfDingbatsList
        /// </summary>
        /// <param name="zn">a char-number</param>
        public ZapfDingbatsList(int zn) : base(true)
        {
            Zn = zn;
            float fontsize = symbol.Font.Size;
            symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
            postSymbol = " ";
        }

        /// <summary>
        /// Creates a ZapfDingbatsList
        /// </summary>
        /// <param name="zn">a char-number</param>
        /// <param name="symbolIndent">indent</param>
        public ZapfDingbatsList(int zn, int symbolIndent) : base(true, symbolIndent)
        {
            Zn = zn;
            float fontsize = symbol.Font.Size;
            symbol.Font = FontFactory.GetFont(FontFactory.ZAPFDINGBATS, fontsize, Font.NORMAL);
            postSymbol = " ";
        }

        /// <summary>
        /// set the char-number
        /// </summary>
        public int CharNumber
        {
            set
            {
                Zn = value;
            }
            get
            {
                return Zn;
            }
        }

        /// <summary>
        /// Adds an  Object  to the  List .
        /// </summary>
        /// <param name="o">the object to add.</param>
        /// <returns>true if adding the object succeeded</returns>
        public override bool Add(object o)
        {
            if (o is ListItem)
            {
                ListItem item = (ListItem)o;
                Chunk chunk = new Chunk(preSymbol, symbol.Font);
                chunk.Append(((char)Zn).ToString());
                chunk.Append(postSymbol);
                item.ListSymbol = chunk;
                item.SetIndentationLeft(symbolIndent, autoindent);
                item.IndentationRight = 0;
                list.Add(item);
                return true;
            }
            else if (o is List)
            {
                List nested = (List)o;
                nested.IndentationLeft = nested.IndentationLeft + symbolIndent;
                first--;
                list.Add(nested);
                return true;
            }
            else if (o is string)
            {
                return Add(new ListItem((string)o));
            }
            return false;
        }
    }
}
