using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfLine  defines an array with  PdfChunk -objects
///     that fit into 1 line.
/// </summary>
public class PdfLine
{
    /// <summary>
    ///     The alignment of the line.
    /// </summary>
    protected internal int Alignment;

    /// <summary>
    ///     The heigth of the line.
    /// </summary>
    protected internal float height;

    protected internal bool IsRtl;

    /// <summary>
    ///     The left indentation of the line.
    /// </summary>
    protected internal float Left;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     The arraylist containing the chunks.
    /// </summary>
    protected internal IList<PdfChunk> Line;

    /// <summary>
    ///     The listsymbol (if necessary).
    /// </summary>
    protected internal Chunk listSymbol;

    /// <summary>
    ///     true  if the chunk splitting was caused by a newline.
    /// </summary>
    protected internal bool newlineSplit;

    /// <summary>
    ///     The original width.
    /// </summary>
    protected internal float originalWidth;

    /// <summary>
    ///     The listsymbol (if necessary).
    /// </summary>
    protected internal float SymbolIndent;

    /// <summary>
    ///     The width of the line.
    /// </summary>
    protected internal float Width;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a new  PdfLine -object.
    /// </summary>
    /// <param name="left">the limit of the line at the left</param>
    /// <param name="right">the limit of the line at the right</param>
    /// <param name="alignment">the alignment of the line</param>
    /// <param name="height">the height of the line</param>
    internal PdfLine(float left, float right, int alignment, float height)
    {
        Left = left;
        Width = right - left;
        originalWidth = Width;
        Alignment = alignment;
        this.height = height;
        Line = new List<PdfChunk>();
    }

    /// <summary>
    ///     Creates a PdfLine object.
    /// </summary>
    /// <param name="left">the left offset</param>
    /// <param name="originalWidth">the original width of the line</param>
    /// <param name="remainingWidth">bigger than 0 if the line isn't completely filled</param>
    /// <param name="alignment">the alignment of the line</param>
    /// <param name="newlineSplit">was the line splitted (or does the paragraph end with this line)</param>
    /// <param name="line">an array of PdfChunk objects</param>
    /// <param name="isRtl">do you have to read the line from Right to Left?</param>
    internal PdfLine(float left, float originalWidth, float remainingWidth,
                     int alignment, bool newlineSplit, IList<PdfChunk> line, bool isRtl)
    {
        Left = left;
        this.originalWidth = originalWidth;
        Width = remainingWidth;
        Alignment = alignment;
        Line = line;
        this.newlineSplit = newlineSplit;
        IsRtl = isRtl;
    }

    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Returns the number of chunks in the line.
    /// </summary>
    /// <returns>a value</returns>

    public int Size => Line.Count;

    /// <summary>
    ///     Returns the height of the line.
    /// </summary>
    /// <returns>a value</returns>

    internal float Height => height;

    /// <summary>
    ///     Returns the left indentation of the line taking the alignment of the line into account.
    /// </summary>
    /// <returns>a value</returns>

    internal float IndentLeft
    {
        get
        {
            if (IsRtl)
            {
                switch (Alignment)
                {
                    case Element.ALIGN_LEFT:
                        return Left + Width;
                    case Element.ALIGN_CENTER:
                        return Left + Width / 2f;
                    default:
                        return Left;
                }
            }

            if (GetSeparatorCount() == 0)
            {
                switch (Alignment)
                {
                    case Element.ALIGN_RIGHT:
                        return Left + Width;
                    case Element.ALIGN_CENTER:
                        return Left + Width / 2f;
                }
            }

            return Left;
        }
    }

    /// <summary>
    ///     Returns the width that is left, after a maximum of characters is added to the line.
    /// </summary>
    /// <returns>a value</returns>

    internal float WidthLeft => Width;

    /// <summary>
    ///     Returns the number of space-characters in this line.
    /// </summary>
    /// <returns>a value</returns>

    internal int NumberOfSpaces
    {
        get
        {
            var str = ToString();
            var length = str.Length;
            var numberOfSpaces = 0;
            for (var i = 0; i < length; i++)
            {
                if (str[i] == ' ')
                {
                    numberOfSpaces++;
                }
            }

            return numberOfSpaces;
        }
    }

    /// <summary>
    ///     Sets the listsymbol of this line.
    ///     This is only necessary for the first line of a  ListItem .
    /// </summary>
    public ListItem ListItem
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            listSymbol = value.ListSymbol;
            SymbolIndent = value.IndentationLeft;
        }
    }

    /// <summary>
    ///     Returns the listsymbol of this line.
    /// </summary>
    /// <returns>a  PdfChunk  if the line has a listsymbol;  null  otherwise</returns>

    public Chunk ListSymbol => listSymbol;

    /// <summary>
    ///     Return the indentation needed to show the listsymbol.
    /// </summary>
    /// <returns>a value</returns>

    public float ListIndent => SymbolIndent;

    /// <summary>
    ///     Checks if a newline caused the line split.
    /// </summary>
    /// <returns> true  if a newline caused the line split</returns>
    public bool NewlineSplit => newlineSplit && Alignment != Element.ALIGN_JUSTIFIED_ALL;

    /// <summary>
    ///     Gets the index of the last  PdfChunk  with metric attributes
    /// </summary>
    /// <returns>the last  PdfChunk  with metric attributes</returns>
    public int LastStrokeChunk
    {
        get
        {
            var lastIdx = Line.Count - 1;
            for (; lastIdx >= 0; --lastIdx)
            {
                var chunk = Line[lastIdx];
                if (chunk.IsStroked())
                {
                    break;
                }
            }

            return lastIdx;
        }
    }

    /// <summary>
    ///     Gets the original width of the line.
    /// </summary>
    /// <returns>the original width of the line</returns>
    public float OriginalWidth => originalWidth;

    internal bool Rtl => IsRtl;

    /// <summary>
    ///     Gets the maximum size of the ascender for all the fonts used
    ///     in this line.
    /// </summary>
    /// <returns>maximum size of all the ascenders used in this line</returns>
    public float Ascender
    {
        get
        {
            float ascender = 0;
            foreach (var ck in Line)
            {
                if (ck.IsImage())
                {
                    ascender = Math.Max(ascender, ck.Image.ScaledHeight + ck.ImageOffsetY);
                }
                else
                {
                    var font = ck.Font;
                    ascender = Math.Max(ascender, font.Font.GetFontDescriptor(BaseFont.ASCENT, font.Size));
                }
            }

            return ascender;
        }
    }

    /// <summary>
    ///     Gets the biggest descender for all the fonts used
    ///     in this line.  Note that this is a negative number.
    /// </summary>
    /// <returns>maximum size of all the ascenders used in this line</returns>
    public float Descender
    {
        get
        {
            float descender = 0;
            foreach (var ck in Line)
            {
                if (ck.IsImage())
                {
                    descender = Math.Min(descender, ck.ImageOffsetY);
                }
                else
                {
                    var font = ck.Font;
                    descender = Math.Min(descender, font.Font.GetFontDescriptor(BaseFont.DESCENT, font.Size));
                }
            }

            return descender;
        }
    }

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Adds a  PdfChunk  to the  PdfLine .
    ///     a  PdfChunk  containing the part of the chunk that could
    ///     not be added is returned
    /// </summary>
    /// <param name="chunk">the  PdfChunk  to add</param>
    /// <returns> null  if the chunk could be added completely; if not</returns>
    internal PdfChunk Add(PdfChunk chunk)
    {
        // nothing happens if the chunk is null.
        if (chunk == null || string.IsNullOrEmpty(chunk.ToString()))
        {
            return null;
        }

        // we split the chunk to be added
        var overflow = chunk.Split(Width);
        newlineSplit = chunk.IsNewlineSplit() || overflow == null;
        //        if (chunk.IsNewlineSplit() && alignment == Element.ALIGN_JUSTIFIED)
        //            alignment = Element.ALIGN_LEFT;
        if (chunk.IsTab())
        {
            var tab = (object[])chunk.GetAttribute(Chunk.TAB);
            var tabPosition = (float)tab[1];
            var newline = (bool)tab[2];
            if (newline && tabPosition < originalWidth - Width)
            {
                return chunk;
            }

            Width = originalWidth - tabPosition;
            chunk.AdjustLeft(Left);
            addToLine(chunk);
        }
        // if the length of the chunk > 0 we add it to the line
        else if (chunk.Length > 0 || chunk.IsImage())
        {
            if (overflow != null)
            {
                chunk.TrimLastSpace();
            }

            Width -= chunk.Width;
            addToLine(chunk);
        }

        // if the length == 0 and there were no other chunks added to the line yet,
        // we risk to end up in an endless loop trying endlessly to add the same chunk
        else if (Line.Count < 1)
        {
            chunk = overflow;
            overflow = chunk.Truncate(Width);
            Width -= chunk.Width;
            if (chunk.Length > 0)
            {
                addToLine(chunk);
                return overflow;
            }
            // if the chunck couldn't even be truncated, we add everything, so be it

            if (overflow != null)
            {
                addToLine(chunk);
            }

            return null;
        }
        else
        {
            Width += Line[Line.Count - 1].TrimLastSpace();
        }

        return overflow;
    }

    private void addToLine(PdfChunk chunk)
    {
        if (chunk.ChangeLeading && chunk.IsImage())
        {
            var f = chunk.Image.ScaledHeight + chunk.ImageOffsetY + chunk.Image.BorderWidthTop;
            if (f > height)
            {
                height = f;
            }
        }

        Line.Add(chunk);
    }

    /// <summary>
    ///     Returns an iterator of  PdfChunk s.
    /// </summary>
    /// <returns>an  Iterator </returns>
    public IEnumerator<PdfChunk> GetEnumerator() => Line.GetEnumerator();

    /// <summary>
    ///     Checks if this line has to be justified.
    /// </summary>
    /// <returns> true  if the alignment equals <VAR>ALIGN_JUSTIFIED</VAR> and there is some width left.</returns>
    public bool HasToBeJustified() =>
        (Alignment == Element.ALIGN_JUSTIFIED || Alignment == Element.ALIGN_JUSTIFIED_ALL) && Width.ApproxNotEqual(0);

    /// <summary>
    ///     Resets the alignment of this line.
    ///     The alignment of the last line of for instance a  Paragraph
    ///     that has to be justified, has to be reset to <VAR>ALIGN_LEFT</VAR>.
    /// </summary>
    public void ResetAlignment()
    {
        if (Alignment == Element.ALIGN_JUSTIFIED)
        {
            Alignment = Element.ALIGN_LEFT;
        }
    }

    /// <summary>
    ///     Adds extra indentation to the left (for Paragraph.setFirstLineIndent).
    /// </summary>
    internal void SetExtraIndent(float extra)
    {
        Left += extra;
        Width -= extra;
    }

    /// <summary>
    ///     Get the string representation of what is in this line.
    /// </summary>
    /// <returns>a  string </returns>
    public override string ToString()
    {
        var tmp = new StringBuilder();
        foreach (var c in Line)
        {
            tmp.Append(c);
        }

        return tmp.ToString();
    }

    public int GetLineLengthUtf32()
    {
        var total = 0;
        foreach (var c in Line)
        {
            total += c.LengthUtf32;
        }

        return total;
    }

    /// <summary>
    ///     Gets a  PdfChunk  by index.
    /// </summary>
    /// <param name="idx">the index</param>
    /// <returns>the  PdfChunk  or null if beyond the array</returns>
    public PdfChunk GetChunk(int idx)
    {
        if (idx < 0 || idx >= Line.Count)
        {
            return null;
        }

        return Line[idx];
    }

    /// <summary>
    ///     Gets the difference between the "normal" leading and the maximum
    ///     size (for instance when there are images in the chunk).
    ///     @since    2.1.5
    /// </summary>
    /// <returns>an extra leading for images</returns>
    internal float[] GetMaxSize()
    {
        float normalLeading = 0;
        float imageLeading = -10000;
        PdfChunk chunk;
        for (var k = 0; k < Line.Count; ++k)
        {
            chunk = Line[k];
            if (!chunk.IsImage())
            {
                normalLeading = Math.Max(chunk.Font.Size, normalLeading);
            }
            else
            {
                imageLeading = Math.Max(chunk.Image.ScaledHeight + chunk.ImageOffsetY, imageLeading);
            }
        }

        return new[] { normalLeading, imageLeading };
    }

    /// <summary>
    ///     Gets the number of separators in the line.
    ///     @since   2.1.2
    /// </summary>
    /// <returns>the number of separators in the line</returns>
    internal int GetSeparatorCount()
    {
        var s = 0;
        foreach (var ck in Line)
        {
            if (ck.IsTab())
            {
                return 0;
            }

            if (ck.IsHorizontalSeparator())
            {
                s++;
            }
        }

        return s;
    }

    public float GetWidthCorrected(float charSpacing, float wordSpacing)
    {
        float total = 0;
        for (var k = 0; k < Line.Count; ++k)
        {
            var ck = Line[k];
            total += ck.GetWidthCorrected(charSpacing, wordSpacing);
        }

        return total;
    }
}