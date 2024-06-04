namespace iTextSharp.text.pdf;

/// <summary>
///     Writes text vertically. Note that the naming is done according
///     to horizontal text although it referrs to vertical text.
///     A line with the alignment Element.LEFT_ALIGN will actually
///     be top aligned.
/// </summary>
public class VerticalText
{
    /// <summary>
    ///     Signals that there is no more column.
    /// </summary>
    public static int NoMoreColumn = 2;

    /// <summary>
    ///     Signals that there are no more text available.
    /// </summary>
    public static int NoMoreText = 1;

    /// <summary>
    ///     The column Element. Default is left Element.
    /// </summary>
    protected int alignment = Element.ALIGN_LEFT;

    /// <summary>
    ///     The chunks that form the text.
    /// </summary>
    protected List<PdfChunk> Chunks = new();

    /// <summary>
    ///     Marks the chunks to be eliminated when the line is written.
    /// </summary>
    protected int CurrentChunkMarker = -1;

    /// <summary>
    ///     The chunk created by the splitting.
    /// </summary>
    protected PdfChunk CurrentStandbyChunk;

    /// <summary>
    ///     The height of the text.
    /// </summary>
    protected float height;

    /// <summary>
    ///     The leading
    /// </summary>
    protected float leading;

    /// <summary>
    ///     The maximum number of vertical lines.
    /// </summary>
    protected int maxLines;

    /// <summary>
    ///     The chunk created by the splitting.
    /// </summary>
    protected string SplittedChunkText;

    /// <summary>
    ///     The X coordinate.
    /// </summary>
    protected float StartX;

    /// <summary>
    ///     The Y coordinate.
    /// </summary>
    protected float StartY;

    /// <summary>
    ///     The  PdfContent  where the text will be written to.
    /// </summary>
    protected PdfContentByte Text;

    /// <summary>
    ///     Creates new VerticalText
    ///     be a template.
    /// </summary>
    /// <param name="text">the place where the text will be written to. Can</param>
    public VerticalText(PdfContentByte text) => Text = text;

    /// <summary>
    ///     Gets the Element.
    /// </summary>
    /// <returns>the alignment</returns>
    public int Alignment
    {
        get => alignment;

        set => alignment = value;
    }

    /// <summary>
    ///     Gets the height of the line
    /// </summary>
    /// <returns>the height</returns>
    public float Height
    {
        get => height;

        set => height = value;
    }

    /// <summary>
    ///     Gets the separation between the vertical lines.
    /// </summary>
    /// <returns>the vertical line separation</returns>
    public float Leading
    {
        get => leading;

        set => leading = value;
    }

    /// <summary>
    ///     Gets the maximum number of available lines. This value will change
    ///     after each call to  go() .
    /// </summary>
    /// <returns>Value of property maxLines.</returns>
    public int MaxLines
    {
        get => maxLines;

        set => maxLines = value;
    }

    /// <summary>
    ///     Gets the X coordinate where the next line will be writen. This value will change
    ///     after each call to  go() .
    /// </summary>
    /// <returns>the X coordinate</returns>
    public float OriginX => StartX;

    /// <summary>
    ///     Gets the Y coordinate where the next line will be writen.
    /// </summary>
    /// <returns>the Y coordinate</returns>
    public float OriginY => StartY;

    /// <summary>
    ///     Adds a  Phrase  to the current text array.
    /// </summary>
    /// <param name="phrase">the text</param>
    public void AddText(Phrase phrase)
    {
        if (phrase == null)
        {
            throw new ArgumentNullException(nameof(phrase));
        }

        foreach (var c in phrase.Chunks)
        {
            Chunks.Add(new PdfChunk(c, null));
        }
    }

    /// <summary>
    ///     Adds a  Chunk  to the current text array.
    /// </summary>
    /// <param name="chunk">the text</param>
    public void AddText(Chunk chunk)
    {
        Chunks.Add(new PdfChunk(chunk, null));
    }

    /// <summary>
    ///     Outputs the lines to the document. It is equivalent to  go(false) .
    ///     and/or  NO_MORE_COLUMN
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>returns the result of the operation. It can be  NO_MORE_TEXT </returns>
    public int Go() => Go(false);

    /// <summary>
    ///     Outputs the lines to the document. The output can be simulated.
    ///     and/or  NO_MORE_COLUMN
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="simulate"> true  to simulate the writting to the document</param>
    /// <returns>returns the result of the operation. It can be  NO_MORE_TEXT </returns>
    public int Go(bool simulate)
    {
        var dirty = false;
        PdfContentByte graphics = null;
        if (Text != null)
        {
            graphics = Text.Duplicate;
        }
        else if (!simulate)
        {
            throw new InvalidOperationException("VerticalText.go with simulate==false and text==null.");
        }

        var status = 0;
        for (;;)
        {
            if (maxLines <= 0)
            {
                status = NoMoreColumn;
                if (Chunks.Count == 0)
                {
                    status |= NoMoreText;
                }

                break;
            }

            if (Chunks.Count == 0)
            {
                status = NoMoreText;
                break;
            }

            var line = CreateLine(height);
            if (!simulate && !dirty)
            {
                Text.BeginText();
                dirty = true;
            }

            ShortenChunkArray();
            if (!simulate)
            {
                Text.SetTextMatrix(StartX, StartY - line.IndentLeft);
                WriteLine(line, Text, graphics);
            }

            --maxLines;
            StartX -= leading;
        }

        if (dirty)
        {
            Text.EndText();
            Text.Add(graphics);
        }

        return status;
    }

    /// <summary>
    ///     Sets the new text origin.
    /// </summary>
    /// <param name="startX">the X coordinate</param>
    /// <param name="startY">the Y coordinate</param>
    public void SetOrigin(float startX, float startY)
    {
        StartX = startX;
        StartY = startY;
    }

    /// <summary>
    ///     Sets the layout.
    /// </summary>
    /// <param name="startX">the top right X line position</param>
    /// <param name="startY">the top right Y line position</param>
    /// <param name="height">the height of the lines</param>
    /// <param name="maxLines">the maximum number of lines</param>
    /// <param name="leading">the separation between the lines</param>
    public void SetVerticalLayout(float startX, float startY, float height, int maxLines, float leading)
    {
        StartX = startX;
        StartY = startY;
        this.height = height;
        this.maxLines = maxLines;
        Leading = leading;
    }

    internal static void WriteLine(PdfLine line, PdfContentByte text, PdfContentByte graphics)
    {
        PdfFont currentFont = null;
        foreach (var chunk in line)
        {
            if (chunk.Font.CompareTo(currentFont) != 0)
            {
                currentFont = chunk.Font;
                text.SetFontAndSize(currentFont.Font, currentFont.Size);
            }

            var color = chunk.Color;
            if (color != null)
            {
                text.SetColorFill(color);
            }

            text.ShowText(chunk.ToString());
            if (color != null)
            {
                text.ResetRgbColorFill();
            }
        }
    }

    /// <summary>
    ///     Creates a line from the chunk array.
    /// </summary>
    /// <param name="width">the width of the line</param>
    /// <returns>the line or null if no more chunks</returns>
    protected PdfLine CreateLine(float width)
    {
        if (Chunks.Count == 0)
        {
            return null;
        }

        SplittedChunkText = null;
        CurrentStandbyChunk = null;
        var line = new PdfLine(0, width, alignment, 0);
        string total;
        for (CurrentChunkMarker = 0; CurrentChunkMarker < Chunks.Count; ++CurrentChunkMarker)
        {
            var original = Chunks[CurrentChunkMarker];
            total = original.ToString();
            CurrentStandbyChunk = line.Add(original);
            if (CurrentStandbyChunk != null)
            {
                SplittedChunkText = original.ToString();
                original.Value = total;
                return line;
            }
        }

        return line;
    }

    /// <summary>
    ///     Normalizes the list of chunks when the line is accepted.
    /// </summary>
    protected void ShortenChunkArray()
    {
        if (CurrentChunkMarker < 0)
        {
            return;
        }

        if (CurrentChunkMarker >= Chunks.Count)
        {
            Chunks.Clear();
            return;
        }

        var split = Chunks[CurrentChunkMarker];
        split.Value = SplittedChunkText;
        Chunks[CurrentChunkMarker] = CurrentStandbyChunk;
        for (var j = CurrentChunkMarker - 1; j >= 0; --j)
        {
            Chunks.RemoveAt(j);
        }
    }
}