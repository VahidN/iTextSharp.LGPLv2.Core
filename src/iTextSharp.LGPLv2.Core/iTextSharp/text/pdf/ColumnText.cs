using System.util;
using iTextSharp.text.pdf.draw;

namespace iTextSharp.text.pdf;

/// <summary>
///     Formats text in a columnwise form. The text is bound
///     on the left and on the right by a sequence of lines. This allows the column
///     to have any shape, not only rectangular.
///     Several parameters can be set like the first paragraph line indent and
///     extra space between paragraphs.
///     A call to the method  go  will return one of the following
///     situations: the column ended or the text ended.
///     I the column ended, a new column definition can be loaded with the method
///     setColumns  and the method  go  can be called again.
///     If the text ended, more text can be loaded with  addText
///     and the method  go  can be called again.
///     The only limitation is that one or more complete paragraphs must be loaded
///     each time.
///     Full bidirectional reordering is supported. If the run direction is
///     PdfWriter.RUN_DIRECTION_RTL  the meaning of the horizontal
///     alignments and margins is mirrored.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class ColumnText
{
    /// <summary>
    ///     Compose the tashkeel in the ligatures.
    /// </summary>
    public const int AR_COMPOSEDTASHKEEL = ArabicLigaturizer.ar_composedtashkeel;

    /// <summary>
    ///     Do some extra double ligatures.
    /// </summary>
    public const int AR_LIG = ArabicLigaturizer.ar_lig;

    /// <summary>
    ///     Digit type option: Use Arabic-Indic digits (U+0660...U+0669).
    /// </summary>
    public const int DIGIT_TYPE_AN = ArabicLigaturizer.DIGIT_TYPE_AN;

    /// <summary>
    ///     Digit type option: Use Eastern (Extended) Arabic-Indic digits (U+06f0...U+06f9).
    /// </summary>
    public const int DIGIT_TYPE_AN_EXTENDED = ArabicLigaturizer.DIGIT_TYPE_AN_EXTENDED;

    /// <summary>
    ///     Digit shaping option: Replace Arabic-Indic digits by European digits (U+0030...U+0039).
    /// </summary>
    public const int DIGITS_AN2EN = ArabicLigaturizer.DIGITS_AN2EN;

    /// <summary>
    ///     Digit shaping option: Replace European digits (U+0030...U+0039) by Arabic-Indic digits.
    /// </summary>
    public const int DIGITS_EN2AN = ArabicLigaturizer.DIGITS_EN2AN;

    /// <summary>
    ///     Digit shaping option:
    ///     Replace European digits (U+0030...U+0039) by Arabic-Indic digits
    ///     if the most recent strongly directional character
    ///     is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
    ///     The initial state at the start of the text is assumed to be an Arabic,
    ///     letter, so European digits at the start of the text will change.
    ///     Compare to DIGITS_ALEN2AN_INT_LR.
    /// </summary>
    public const int DIGITS_EN2AN_INIT_AL = ArabicLigaturizer.DIGITS_EN2AN_INIT_AL;

    /// <summary>
    ///     Digit shaping option:
    ///     Replace European digits (U+0030...U+0039) by Arabic-Indic digits
    ///     if the most recent strongly directional character
    ///     is an Arabic letter (its Bidi direction value is RIGHT_TO_LEFT_ARABIC).
    ///     The initial state at the start of the text is assumed to be not an Arabic,
    ///     letter, so European digits at the start of the text will not change.
    ///     Compare to DIGITS_ALEN2AN_INIT_AL.
    /// </summary>
    public const int DIGITS_EN2AN_INIT_LR = ArabicLigaturizer.DIGITS_EN2AN_INIT_LR;

    /// <summary>
    ///     Signals that there is no more column.
    /// </summary>
    public const int NO_MORE_COLUMN = 2;

    /// <summary>
    ///     Signals that there is no more text available.
    /// </summary>
    public const int NO_MORE_TEXT = 1;

    /// <summary>
    ///     The line cannot fit this column position.
    /// </summary>
    protected const int LINE_STATUS_NOLINE = 2;

    /// <summary>
    ///     The line is out the column limits.
    /// </summary>
    protected const int LINE_STATUS_OFFLIMITS = 1;

    /// <summary>
    ///     The column is valid.
    /// </summary>
    protected const int LINE_STATUS_OK = 0;

    public static float GlobalSpaceCharRatio = 0;

    /// <summary>
    ///     Holds value of property arabicOptions.
    /// </summary>
    private int _arabicOptions;

    /// <summary>
    ///     Holds value of property filledWidth.
    /// </summary>
    private float _filledWidth;

    private float _firstLineY;

    private bool _firstLineYDone;

    private bool _lastWasNewline = true;

    /// <summary>
    ///     Holds value of property linesWritten.
    /// </summary>
    private int _linesWritten;

    /// <summary>
    ///     Holds value of property spaceCharRatio.
    /// </summary>
    private float _spaceCharRatio = GlobalSpaceCharRatio;

    private bool _splittedRow;

    /// <summary>
    ///     if true, first line height is adjusted so that the max ascender touches the top
    /// </summary>
    private bool _useAscender;

    /// <summary>
    ///     The column Element. Default is left Element.
    /// </summary>
    protected int alignment = Element.ALIGN_LEFT;

    /// <summary>
    ///     Eliminate the arabic vowels
    /// </summary>
    public int ArNovowel = ArabicLigaturizer.ar_novowel;

    /// <summary>
    ///     The chunks that form the text.
    /// </summary>
    /// <summary>
    ///     protected ArrayList chunks = new ArrayList();
    /// </summary>
    protected BidiLine BidiLine;

    /// <summary>
    ///     The  PdfContent  where the text will be written to.
    /// </summary>
    protected PdfContentByte canvas;

    protected PdfContentByte[] canvases;
    protected bool Composite;
    protected ColumnText CompositeColumn;
    protected internal IList<IElement> CompositeElements;

    /// <summary>
    ///     The leading for the current line.
    /// </summary>
    protected float CurrentLeading = 16;

    protected float descender;

    /// <summary>
    ///     The extra space between paragraphs.
    /// </summary>
    protected float extraParagraphSpace;

    /// <summary>
    ///     The fixed text leading.
    /// </summary>
    protected float FixedLeading = 16;

    /// <summary>
    ///     The following paragraph lines indent.
    /// </summary>
    protected float followingIndent;

    /// <summary>
    ///     The first paragraph line indent.
    /// </summary>
    protected float indent;

    /// <summary>
    ///     The left column bound.
    /// </summary>
    protected IList<float[]> LeftWall;

    protected float LeftX;

    /// <summary>
    ///     The line status when trying to fit a line to a column.
    /// </summary>
    protected int LineStatus;

    protected int ListIdx;

    /// <summary>
    ///     Upper bound of the column.
    /// </summary>
    protected float MaxY;

    /// <summary>
    ///     Lower bound of the column.
    /// </summary>
    protected float MinY;

    /// <summary>
    ///     The text leading that is multiplied by the biggest font size in the line.
    /// </summary>
    protected float multipliedLeading;

    protected bool RectangularMode;

    /// <summary>
    ///     The width of the line when the column is defined as a simple rectangle.
    /// </summary>
    protected float RectangularWidth = -1;

    /// <summary>
    ///     The right paragraph lines indent.
    /// </summary>
    protected float rightIndent;

    /// <summary>
    ///     The right column bound.
    /// </summary>
    protected IList<float[]> RightWall;

    protected float RightX;
    protected int runDirection = PdfWriter.RUN_DIRECTION_DEFAULT;
    protected Phrase WaitPhrase;

    /// <summary>
    ///     The current y line location. Text will be written at this line minus the leading.
    /// </summary>
    protected float yLine;

    /// <summary>
    ///     Creates a  ColumnText .
    ///     be a template.
    /// </summary>
    /// <param name="canvas">the place where the text will be written to. Can</param>
    public ColumnText(PdfContentByte canvas) => this.canvas = canvas;

    /// <summary>
    ///     Sets the first line adjustment. Some objects have properties, like spacing before, that
    ///     behave differently if the object is the first to be written after go() or not. The first line adjustment is
    ///     true  by default but can be changed if several objects are to be placed one
    ///     after the other in the same column calling go() several times.
    /// </summary>
    public bool AdjustFirstLine { set; get; } = true;

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
    ///     Sets the arabic shaping options. The option can be AR_NOVOWEL,
    ///     AR_COMPOSEDTASHKEEL and AR_LIG.
    /// </summary>
    public int ArabicOptions
    {
        set => _arabicOptions = value;
        get => _arabicOptions;
    }

    /// <summary>
    ///     Sets the canvas.
    /// </summary>
    public PdfContentByte Canvas
    {
        set
        {
            canvas = value;
            canvases = null;
            if (CompositeColumn != null)
            {
                CompositeColumn.Canvas = value;
            }
        }
        get => canvas;
    }

    /// <summary>
    ///     Sets the canvases.
    /// </summary>
    public PdfContentByte[] Canvases
    {
        set
        {
            canvases = value;
            canvas = canvases[PdfPTable.TEXTCANVAS];
            if (CompositeColumn != null)
            {
                CompositeColumn.Canvases = canvases;
            }
        }
        get => canvases;
    }

    /// <summary>
    ///     Gets the biggest descender value of the last line written.
    /// </summary>
    /// <returns>the biggest descender value of the last line written</returns>
    public float Descender => descender;

    /// <summary>
    ///     Sets the extra space between paragraphs.
    /// </summary>
    /// <returns>the extra space between paragraphs</returns>
    public float ExtraParagraphSpace
    {
        get => extraParagraphSpace;

        set => extraParagraphSpace = value;
    }

    /// <summary>
    ///     Sets the real width used by the largest line. Only used to set it
    ///     to zero to start another measurement.
    /// </summary>
    public float FilledWidth
    {
        set => _filledWidth = value;
        get => _filledWidth;
    }

    /// <summary>
    ///     Gets the following paragraph lines indent.
    /// </summary>
    /// <returns>the indent</returns>
    public float FollowingIndent
    {
        get => followingIndent;

        set
        {
            followingIndent = value;
            _lastWasNewline = true;
        }
    }

    /// <summary>
    ///     Gets the first paragraph line indent.
    /// </summary>
    /// <returns>the indent</returns>
    public float Indent
    {
        get => indent;

        set
        {
            indent = value;
            _lastWasNewline = true;
        }
    }

    /// <summary>
    ///     Gets the fixed leading
    /// </summary>
    /// <returns>the leading</returns>
    public float Leading
    {
        get => FixedLeading;

        set
        {
            FixedLeading = value;
            multipliedLeading = 0;
        }
    }

    /// <summary>
    ///     Gets the number of lines written.
    /// </summary>
    /// <returns>the number of lines written</returns>
    public int LinesWritten => _linesWritten;

    /// <summary>
    ///     Gets the variable leading
    /// </summary>
    /// <returns>the leading</returns>
    public float MultipliedLeading => multipliedLeading;

    /// <summary>
    ///     Gets the right paragraph lines indent.
    /// </summary>
    /// <returns>the indent</returns>
    public float RightIndent
    {
        get => rightIndent;

        set
        {
            rightIndent = value;
            _lastWasNewline = true;
        }
    }

    /// <summary>
    ///     Gets the run direction.
    /// </summary>
    /// <returns>the run direction</returns>
    public int RunDirection
    {
        get => runDirection;

        set
        {
            if (value < PdfWriter.RUN_DIRECTION_DEFAULT || value > PdfWriter.RUN_DIRECTION_RTL)
            {
                throw new InvalidOperationException("Invalid run direction: " + value);
            }

            runDirection = value;
        }
    }

    /// <summary>
    ///     Gets the space/character extra spacing ratio for
    ///     fully justified text.
    /// </summary>
    /// <returns>the space/character extra spacing ratio</returns>
    public float SpaceCharRatio
    {
        get => _spaceCharRatio;

        set => _spaceCharRatio = value;
    }

    /// <summary>
    ///     Enables/Disables adjustment of first line height based on max ascender.
    /// </summary>
    public bool UseAscender
    {
        set => _useAscender = value;
        get => _useAscender;
    }

    /// <summary>
    ///     Gets the yLine.
    /// </summary>
    /// <returns>the yLine</returns>
    public float YLine
    {
        get => yLine;

        set => yLine = value;
    }

    /// <summary>
    ///     Creates an independent duplicated of the instance  org .
    /// </summary>
    /// <param name="org">the original  ColumnText </param>
    /// <returns>the duplicated</returns>
    public static ColumnText Duplicate(ColumnText org)
    {
        var ct = new ColumnText(null);
        ct.SetACopy(org);
        return ct;
    }

    /// <summary>
    ///     Gets the width that the line will occupy after writing.
    ///     Only the width of the first line is returned.
    /// </summary>
    /// <param name="phrase">the  Phrase  containing the line</param>
    /// <param name="runDirection">the run direction</param>
    /// <param name="arabicOptions">the options for the arabic shaping</param>
    /// <returns>the width of the line</returns>
    public static float GetWidth(Phrase phrase, int runDirection, int arabicOptions)
    {
        var ct = new ColumnText(null);
        ct.AddText(phrase);
        ct.addWaitingPhrase();
        var line = ct.BidiLine.ProcessLine(0, 20000, Element.ALIGN_LEFT, runDirection, arabicOptions);
        if (line == null)
        {
            return 0;
        }

        return 20000 - line.WidthLeft;
    }

    /// <summary>
    ///     Gets the width that the line will occupy after writing.
    ///     Only the width of the first line is returned.
    /// </summary>
    /// <param name="phrase">the  Phrase  containing the line</param>
    /// <returns>the width of the line</returns>
    public static float GetWidth(Phrase phrase) => GetWidth(phrase, PdfWriter.RUN_DIRECTION_NO_BIDI, 0);

    /// <summary>
    ///     Checks the status variable and looks if there's still some text.
    /// </summary>
    public static bool HasMoreText(int status) => (status & NO_MORE_TEXT) == 0;

    /// <summary>
    ///     Shows a line of text. Only the first line is written.
    /// </summary>
    /// <param name="canvas">where the text is to be written to</param>
    /// <param name="alignment">the alignment. It is not influenced by the run direction</param>
    /// <param name="phrase">the  Phrase  with the text</param>
    /// <param name="x">the x reference position</param>
    /// <param name="y">the y reference position</param>
    /// <param name="rotation">the rotation to be applied in degrees counterclockwise</param>
    /// <param name="runDirection">the run direction</param>
    /// <param name="arabicOptions">the options for the arabic shaping</param>
    public static void ShowTextAligned(PdfContentByte canvas,
                                       int alignment,
                                       Phrase phrase,
                                       float x,
                                       float y,
                                       float rotation,
                                       int runDirection,
                                       int arabicOptions)
    {
        if (canvas == null)
        {
            throw new ArgumentNullException(nameof(canvas));
        }

        if (alignment != Element.ALIGN_LEFT && alignment != Element.ALIGN_CENTER
                                            && alignment != Element.ALIGN_RIGHT)
        {
            alignment = Element.ALIGN_LEFT;
        }

        canvas.SaveState();
        var ct = new ColumnText(canvas);
        float lly = -1;
        float ury = 2;
        float llx;
        float urx;
        switch (alignment)
        {
            case Element.ALIGN_LEFT:
                llx = 0;
                urx = 20000;
                break;
            case Element.ALIGN_RIGHT:
                llx = -20000;
                urx = 0;
                break;
            default:
                llx = -20000;
                urx = 20000;
                break;
        }

        if (rotation.ApproxEquals(0))
        {
            llx += x;
            lly += y;
            urx += x;
            ury += y;
        }
        else
        {
            var alpha = rotation * Math.PI / 180.0;
            var cos = (float)Math.Cos(alpha);
            var sin = (float)Math.Sin(alpha);
            canvas.ConcatCtm(cos, sin, -sin, cos, x, y);
        }

        ct.SetSimpleColumn(phrase, llx, lly, urx, ury, 2, alignment);
        if (runDirection == PdfWriter.RUN_DIRECTION_RTL)
        {
            if (alignment == Element.ALIGN_LEFT)
            {
                alignment = Element.ALIGN_RIGHT;
            }
            else if (alignment == Element.ALIGN_RIGHT)
            {
                alignment = Element.ALIGN_LEFT;
            }
        }

        ct.Alignment = alignment;
        ct.ArabicOptions = arabicOptions;
        ct.RunDirection = runDirection;
        ct.Go();
        canvas.RestoreState();
    }

    /// <summary>
    ///     Shows a line of text. Only the first line is written.
    /// </summary>
    /// <param name="canvas">where the text is to be written to</param>
    /// <param name="alignment">the alignment</param>
    /// <param name="phrase">the  Phrase  with the text</param>
    /// <param name="x">the x reference position</param>
    /// <param name="y">the y reference position</param>
    /// <param name="rotation">the rotation to be applied in degrees counterclockwise</param>
    public static void ShowTextAligned(PdfContentByte canvas,
                                       int alignment,
                                       Phrase phrase,
                                       float x,
                                       float y,
                                       float rotation)
    {
        ShowTextAligned(canvas, alignment, phrase, x, y, rotation, PdfWriter.RUN_DIRECTION_NO_BIDI, 0);
    }

    /// <summary>
    ///     Adds an element. Elements supported are  Paragraph ,
    ///     List ,  PdfPTable ,  Image  and
    ///     Graphic .
    ///     It removes all the text placed with  addText() .
    /// </summary>
    /// <param name="element">the  Element </param>
    public void AddElement(IElement element)
    {
        if (element == null)
        {
            return;
        }

        if (element is Image)
        {
            var img = (Image)element;
            var t = new PdfPTable(1);
            var w = img.WidthPercentage;
            if (w.ApproxEquals(0))
            {
                t.TotalWidth = img.ScaledWidth;
                t.LockedWidth = true;
            }
            else
            {
                t.WidthPercentage = w;
            }

            t.SpacingAfter = img.SpacingAfter;
            t.SpacingBefore = img.SpacingBefore;
            switch (img.Alignment)
            {
                case Image.LEFT_ALIGN:
                    t.HorizontalAlignment = Element.ALIGN_LEFT;
                    break;
                case Image.RIGHT_ALIGN:
                    t.HorizontalAlignment = Element.ALIGN_RIGHT;
                    break;
                default:
                    t.HorizontalAlignment = Element.ALIGN_CENTER;
                    break;
            }

            var c = new PdfPCell(img, true);
            c.Padding = 0;
            c.Border = img.Border;
            c.BorderColor = img.BorderColor;
            c.BorderWidth = img.BorderWidth;
            c.BackgroundColor = img.BackgroundColor;
            t.AddCell(c);
            element = t;
        }

        if (element.Type == Element.CHUNK)
        {
            element = new Paragraph((Chunk)element);
        }
        else if (element.Type == Element.PHRASE)
        {
            element = new Paragraph((Phrase)element);
        }

        if (element is SimpleTable)
        {
            try
            {
                element = ((SimpleTable)element).CreatePdfPTable();
            }
            catch (DocumentException)
            {
                throw new ArgumentException("Element not allowed.");
            }
        }
        else if (element.Type != Element.PARAGRAPH && element.Type != Element.LIST && element.Type != Element.PTABLE &&
                 element.Type != Element.YMARK)
        {
            throw new ArgumentException("Element not allowed.");
        }

        if (!Composite)
        {
            Composite = true;
            CompositeElements = new List<IElement>();
            BidiLine = null;
            WaitPhrase = null;
        }

        CompositeElements.Add(element);
    }

    /// <summary>
    ///     Adds a  Phrase  to the current text array.
    /// </summary>
    /// <param name="phrase">the text</param>
    public void AddText(Phrase phrase)
    {
        if (phrase == null || Composite)
        {
            return;
        }

        addWaitingPhrase();
        if (BidiLine == null)
        {
            WaitPhrase = phrase;
            return;
        }

        foreach (var c in phrase.Chunks)
        {
            BidiLine.AddChunk(new PdfChunk(c, null));
        }
    }

    /// <summary>
    ///     Adds a  Chunk  to the current text array.
    ///     Will not have any effect if AddElement() was called before.
    /// </summary>
    /// <param name="chunk">the text</param>
    public void AddText(Chunk chunk)
    {
        if (chunk == null || Composite)
        {
            return;
        }

        AddText(new Phrase(chunk));
    }

    /// <summary>
    ///     Clears the chunk array. A call to  go()  will always return
    ///     NO_MORE_TEXT.
    /// </summary>
    public void ClearChunks()
    {
        if (BidiLine != null)
        {
            BidiLine.ClearChunks();
        }
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
        if (Composite)
        {
            return GoComposite(simulate);
        }

        addWaitingPhrase();
        if (BidiLine == null)
        {
            return NO_MORE_TEXT;
        }

        descender = 0;
        _linesWritten = 0;
        var dirty = false;
        var ratio = _spaceCharRatio;
        var currentValues = new object[2];
        PdfFont currentFont = null;
        var lastBaseFactor = 0F;
        currentValues[1] = lastBaseFactor;
        PdfDocument pdf = null;
        PdfContentByte graphics = null;
        PdfContentByte text = null;
        _firstLineY = float.NaN;
        var localRunDirection = PdfWriter.RUN_DIRECTION_NO_BIDI;
        if (runDirection != PdfWriter.RUN_DIRECTION_DEFAULT)
        {
            localRunDirection = runDirection;
        }

        if (canvas != null)
        {
            graphics = canvas;
            pdf = canvas.PdfDocument;
            text = canvas.Duplicate;
        }
        else if (!simulate)
        {
            throw new InvalidOperationException("ColumnText.go with simulate==false and text==null.");
        }

        if (!simulate)
        {
            if (ratio.ApproxEquals(GlobalSpaceCharRatio))
            {
                ratio = text.PdfWriter.SpaceCharRatio;
            }
            else if (ratio < 0.001f)
            {
                ratio = 0.001f;
            }
        }

        float firstIndent = 0;
        PdfLine line;
        float x1;
        var status = 0;
        while (true)
        {
            firstIndent = _lastWasNewline ? indent : followingIndent; //
            if (RectangularMode)
            {
                if (RectangularWidth <= firstIndent + rightIndent)
                {
                    status = NO_MORE_COLUMN;
                    if (BidiLine.IsEmpty())
                    {
                        status |= NO_MORE_TEXT;
                    }

                    break;
                }

                if (BidiLine.IsEmpty())
                {
                    status = NO_MORE_TEXT;
                    break;
                }

                line = BidiLine.ProcessLine(LeftX,
                                            RectangularWidth - firstIndent - rightIndent,
                                            alignment,
                                            localRunDirection,
                                            _arabicOptions);
                if (line == null)
                {
                    status = NO_MORE_TEXT;
                    break;
                }

                var maxSize = line.GetMaxSize();
                if (UseAscender && float.IsNaN(_firstLineY))
                {
                    CurrentLeading = line.Ascender;
                }
                else
                {
                    CurrentLeading = Math.Max(FixedLeading + maxSize[0] * multipliedLeading, maxSize[1]);
                }

                if (yLine > MaxY || yLine - CurrentLeading < MinY)
                {
                    status = NO_MORE_COLUMN;
                    BidiLine.Restore();
                    break;
                }

                yLine -= CurrentLeading;
                if (!simulate && !dirty)
                {
                    text.BeginText();
                    dirty = true;
                }

                if (float.IsNaN(_firstLineY))
                {
                    _firstLineY = yLine;
                }

                UpdateFilledWidth(RectangularWidth - line.WidthLeft);
                x1 = LeftX;
            }
            else
            {
                var yTemp = yLine;
                var xx = FindLimitsTwoLines();
                if (xx == null)
                {
                    status = NO_MORE_COLUMN;
                    if (BidiLine.IsEmpty())
                    {
                        status |= NO_MORE_TEXT;
                    }

                    yLine = yTemp;
                    break;
                }

                if (BidiLine.IsEmpty())
                {
                    status = NO_MORE_TEXT;
                    yLine = yTemp;
                    break;
                }

                x1 = Math.Max(xx[0], xx[2]);
                var x2 = Math.Min(xx[1], xx[3]);
                if (x2 - x1 <= firstIndent + rightIndent)
                {
                    continue;
                }

                if (!simulate && !dirty)
                {
                    text.BeginText();
                    dirty = true;
                }

                line = BidiLine.ProcessLine(x1,
                                            x2 - x1 - firstIndent - rightIndent,
                                            alignment,
                                            localRunDirection,
                                            _arabicOptions);
                if (line == null)
                {
                    status = NO_MORE_TEXT;
                    yLine = yTemp;
                    break;
                }
            }

            if (!simulate)
            {
                currentValues[0] = currentFont;
                text.SetTextMatrix(x1 + (line.Rtl ? rightIndent : firstIndent) + line.IndentLeft, yLine);
                pdf.WriteLineToContent(line, text, graphics, currentValues, ratio);
                currentFont = (PdfFont)currentValues[0];
            }

            _lastWasNewline = line.NewlineSplit;
            yLine -= line.NewlineSplit ? extraParagraphSpace : 0;
            ++_linesWritten;
            descender = line.Descender;
        }

        if (dirty)
        {
            text.EndText();
            canvas.Add(text);
        }

        return status;
    }

    /// <summary>
    ///     Makes this instance an independent copy of  org .
    /// </summary>
    /// <param name="org">the original  ColumnText </param>
    /// <returns>itself</returns>
    public ColumnText SetACopy(ColumnText org)
    {
        if (org == null)
        {
            throw new ArgumentNullException(nameof(org));
        }

        SetSimpleVars(org);
        if (org.BidiLine != null)
        {
            BidiLine = new BidiLine(org.BidiLine);
        }

        return this;
    }

    /// <summary>
    ///     Sets the columns bounds. Each column bound is described by a
    ///     float[]  with the line points [x1,y1,x2,y2,...].
    ///     The array must have at least 4 elements.
    /// </summary>
    /// <param name="leftLine">the left column bound</param>
    /// <param name="rightLine">the right column bound</param>
    public void SetColumns(float[] leftLine, float[] rightLine)
    {
        if (leftLine == null)
        {
            throw new ArgumentNullException(nameof(leftLine));
        }

        MaxY = -10e20f;
        MinY = 10e20f;
        YLine = Math.Max(leftLine[1], leftLine[leftLine.Length - 1]);
        RightWall = ConvertColumn(rightLine);
        LeftWall = ConvertColumn(leftLine);
        RectangularWidth = -1;
        RectangularMode = false;
    }

    /// <summary>
    ///     Sets the leading fixed and variable. The resultant leading will be
    ///     fixedLeading+multipliedLeading*maxFontSize where maxFontSize is the
    ///     size of the bigest font in the line.
    /// </summary>
    /// <param name="fixedLeading">the fixed leading</param>
    /// <param name="multipliedLeading">the variable leading</param>
    public void SetLeading(float fixedLeading, float multipliedLeading)
    {
        FixedLeading = fixedLeading;
        this.multipliedLeading = multipliedLeading;
    }

    /// <summary>
    ///     Simplified method for rectangular columns.
    /// </summary>
    /// <param name="phrase">a  Phrase </param>
    /// <param name="llx">the lower left x corner</param>
    /// <param name="lly">the lower left y corner</param>
    /// <param name="urx">the upper right x corner</param>
    /// <param name="ury">the upper right y corner</param>
    /// <param name="leading">the leading</param>
    /// <param name="alignment">the column alignment</param>
    public void SetSimpleColumn(Phrase phrase, float llx, float lly, float urx, float ury, float leading, int alignment)
    {
        AddText(phrase);
        SetSimpleColumn(llx, lly, urx, ury, leading, alignment);
    }

    /// <summary>
    ///     Simplified method for rectangular columns.
    /// </summary>
    /// <param name="llx">the lower left x corner</param>
    /// <param name="lly">the lower left y corner</param>
    /// <param name="urx">the upper right x corner</param>
    /// <param name="ury">the upper right y corner</param>
    /// <param name="leading">the leading</param>
    /// <param name="alignment">the column alignment</param>
    public void SetSimpleColumn(float llx, float lly, float urx, float ury, float leading, int alignment)
    {
        Leading = leading;
        this.alignment = alignment;
        SetSimpleColumn(llx, lly, urx, ury);
    }

    /// <summary>
    ///     Simplified method for rectangular columns.
    /// </summary>
    /// <param name="llx"></param>
    /// <param name="lly"></param>
    /// <param name="urx"></param>
    /// <param name="ury"></param>
    public void SetSimpleColumn(float llx, float lly, float urx, float ury)
    {
        LeftX = Math.Min(llx, urx);
        MaxY = Math.Max(lly, ury);
        MinY = Math.Min(lly, ury);
        RightX = Math.Max(llx, urx);
        yLine = MaxY;
        RectangularWidth = RightX - LeftX;
        if (RectangularWidth < 0)
        {
            RectangularWidth = 0;
        }

        RectangularMode = true;
    }

    /// <summary>
    ///     Replaces the current text array with this  Phrase .
    ///     Anything added previously with AddElement() is lost.
    /// </summary>
    /// <param name="phrase">the text</param>
    public void SetText(Phrase phrase)
    {
        BidiLine = null;
        Composite = false;
        CompositeColumn = null;
        CompositeElements = null;
        ListIdx = 0;
        _splittedRow = false;
        WaitPhrase = phrase;
    }

    /// <summary>
    ///     Replaces the  filledWidth  if greater than the existing one.
    /// </summary>
    /// <param name="w">the new  filledWidth  if greater than the existing one</param>
    public void UpdateFilledWidth(float w)
    {
        if (w > _filledWidth)
        {
            _filledWidth = w;
        }
    }

    /// <summary>
    ///     Checks if the element has a height of 0.
    ///     @since 2.1.2
    /// </summary>
    /// <returns>true or false</returns>
    public bool ZeroHeightElement() =>
        Composite && CompositeElements.Count != 0 && CompositeElements[0].Type == Element.YMARK;

    protected internal void SetSimpleVars(ColumnText org)
    {
        if (org == null)
        {
            throw new ArgumentNullException(nameof(org));
        }

        MaxY = org.MaxY;
        MinY = org.MinY;
        alignment = org.alignment;
        LeftWall = null;
        if (org.LeftWall != null)
        {
            LeftWall = new List<float[]>(org.LeftWall);
        }

        RightWall = null;
        if (org.RightWall != null)
        {
            RightWall = new List<float[]>(org.RightWall);
        }

        yLine = org.yLine;
        CurrentLeading = org.CurrentLeading;
        FixedLeading = org.FixedLeading;
        multipliedLeading = org.multipliedLeading;
        canvas = org.canvas;
        canvases = org.canvases;
        LineStatus = org.LineStatus;
        indent = org.indent;
        followingIndent = org.followingIndent;
        rightIndent = org.rightIndent;
        extraParagraphSpace = org.extraParagraphSpace;
        RectangularWidth = org.RectangularWidth;
        RectangularMode = org.RectangularMode;
        _spaceCharRatio = org._spaceCharRatio;
        _lastWasNewline = org._lastWasNewline;
        _linesWritten = org._linesWritten;
        _arabicOptions = org._arabicOptions;
        runDirection = org.runDirection;
        descender = org.descender;
        Composite = org.Composite;
        _splittedRow = org._splittedRow;
        if (org.Composite)
        {
            CompositeElements = new List<IElement>(org.CompositeElements);
            if (_splittedRow)
            {
                var table = (PdfPTable)CompositeElements[0];
                CompositeElements[0] = new PdfPTable(table);
            }

            if (org.CompositeColumn != null)
            {
                CompositeColumn = Duplicate(org.CompositeColumn);
            }
        }

        ListIdx = org.ListIdx;
        _firstLineY = org._firstLineY;
        LeftX = org.LeftX;
        RightX = org.RightX;
        _firstLineYDone = org._firstLineYDone;
        WaitPhrase = org.WaitPhrase;
        _useAscender = org._useAscender;
        _filledWidth = org._filledWidth;
        AdjustFirstLine = org.AdjustFirstLine;
    }

    /// <summary>
    ///     Converts a sequence of lines representing one of the column bounds into
    ///     an internal format.
    ///     Each array element will contain a  float[4]  representing
    ///     the line x = ax + b.
    /// </summary>
    /// <param name="cLine">the column array</param>
    /// <returns>the converted array</returns>
    protected IList<float[]> ConvertColumn(float[] cLine)
    {
        if (cLine == null)
        {
            throw new ArgumentNullException(nameof(cLine));
        }

        if (cLine.Length < 4)
        {
            throw new InvalidOperationException("No valid column line found.");
        }

        List<float[]> cc = new();
        for (var k = 0; k < cLine.Length - 2; k += 2)
        {
            var x1 = cLine[k];
            var y1 = cLine[k + 1];
            var x2 = cLine[k + 2];
            var y2 = cLine[k + 3];
            if (y1.ApproxEquals(y2))
            {
                continue;
            }

            // x = ay + b
            var a = (x1 - x2) / (y1 - y2);
            var b = x1 - a * y1;
            var r = new float[4];
            r[0] = Math.Min(y1, y2);
            r[1] = Math.Max(y1, y2);
            r[2] = a;
            r[3] = b;
            cc.Add(r);
            MaxY = Math.Max(MaxY, r[1]);
            MinY = Math.Min(MinY, r[0]);
        }

        if (cc.Count == 0)
        {
            throw new InvalidOperationException("No valid column line found.");
        }

        return cc;
    }

    /// <summary>
    ///     Finds the intersection between the  yLine  and the two
    ///     column bounds. It will set the  lineStatus  apropriatly.
    /// </summary>
    /// <returns>a  float[2] with the x coordinates of the intersection</returns>
    protected float[] FindLimitsOneLine()
    {
        var x1 = FindLimitsPoint(LeftWall);
        if (LineStatus == LINE_STATUS_OFFLIMITS || LineStatus == LINE_STATUS_NOLINE)
        {
            return null;
        }

        var x2 = FindLimitsPoint(RightWall);
        if (LineStatus == LINE_STATUS_NOLINE)
        {
            return null;
        }

        return new[] { x1, x2 };
    }

    /// <summary>
    ///     Finds the intersection between the  yLine  and the column. It will
    ///     set the  lineStatus  apropriatly.
    /// </summary>
    /// <param name="wall">the column to intersect</param>
    /// <returns>the x coordinate of the intersection</returns>
    protected float FindLimitsPoint(IList<float[]> wall)
    {
        if (wall == null)
        {
            throw new ArgumentNullException(nameof(wall));
        }

        LineStatus = LINE_STATUS_OK;
        if (yLine < MinY || yLine > MaxY)
        {
            LineStatus = LINE_STATUS_OFFLIMITS;
            return 0;
        }

        for (var k = 0; k < wall.Count; ++k)
        {
            var r = wall[k];
            if (yLine < r[0] || yLine > r[1])
            {
                continue;
            }

            return r[2] * yLine + r[3];
        }

        LineStatus = LINE_STATUS_NOLINE;
        return 0;
    }

    /// <summary>
    ///     Finds the intersection between the  yLine ,
    ///     the  yLine-leading and the two
    ///     column bounds. It will set the  lineStatus  apropriatly.
    /// </summary>
    /// <returns>a  float[4] with the x coordinates of the intersection</returns>
    protected float[] FindLimitsTwoLines()
    {
        var repeat = false;
        for (;;)
        {
            if (repeat && CurrentLeading.ApproxEquals(0))
            {
                return null;
            }

            repeat = true;
            var x1 = FindLimitsOneLine();
            if (LineStatus == LINE_STATUS_OFFLIMITS)
            {
                return null;
            }

            yLine -= CurrentLeading;
            if (LineStatus == LINE_STATUS_NOLINE)
            {
                continue;
            }

            var x2 = FindLimitsOneLine();
            if (LineStatus == LINE_STATUS_OFFLIMITS)
            {
                return null;
            }

            if (LineStatus == LINE_STATUS_NOLINE)
            {
                yLine -= CurrentLeading;
                continue;
            }

            if (x1[0] >= x2[1] || x2[0] >= x1[1])
            {
                continue;
            }

            return new[] { x1[0], x1[1], x2[0], x2[1] };
        }
    }

    protected int GoComposite(bool simulate)
    {
        if (!RectangularMode)
        {
            throw new DocumentException("Irregular columns are not supported in composite mode.");
        }

        _linesWritten = 0;
        descender = 0;
        var firstPass = AdjustFirstLine;
        main_loop:
        while (true)
        {
            if (CompositeElements.Count == 0)
            {
                return NO_MORE_TEXT;
            }

            var element = CompositeElements[0];
            if (element.Type == Element.PARAGRAPH)
            {
                var para = (Paragraph)element;
                var status = 0;
                for (var keep = 0; keep < 2; ++keep)
                {
                    var lastY = yLine;
                    var createHere = false;
                    if (CompositeColumn == null)
                    {
                        CompositeColumn = new ColumnText(canvas);
                        CompositeColumn.UseAscender = firstPass && _useAscender;
                        CompositeColumn.Alignment = para.Alignment;
                        CompositeColumn.Indent = para.IndentationLeft + para.FirstLineIndent;
                        CompositeColumn.ExtraParagraphSpace = para.ExtraParagraphSpace;
                        CompositeColumn.FollowingIndent = para.IndentationLeft;
                        CompositeColumn.RightIndent = para.IndentationRight;
                        CompositeColumn.SetLeading(para.Leading, para.MultipliedLeading);
                        CompositeColumn.RunDirection = runDirection;
                        CompositeColumn.ArabicOptions = _arabicOptions;
                        CompositeColumn.SpaceCharRatio = _spaceCharRatio;
                        CompositeColumn.AddText(para);
                        if (!firstPass)
                        {
                            yLine -= para.SpacingBefore;
                        }

                        createHere = true;
                    }

                    CompositeColumn.LeftX = LeftX;
                    CompositeColumn.RightX = RightX;
                    CompositeColumn.yLine = yLine;
                    CompositeColumn.RectangularWidth = RectangularWidth;
                    CompositeColumn.RectangularMode = RectangularMode;
                    CompositeColumn.MinY = MinY;
                    CompositeColumn.MaxY = MaxY;
                    var keepCandidate = para.KeepTogether && createHere && !firstPass;
                    status = CompositeColumn.Go(simulate || (keepCandidate && keep == 0));
                    UpdateFilledWidth(CompositeColumn._filledWidth);
                    if ((status & NO_MORE_TEXT) == 0 && keepCandidate)
                    {
                        CompositeColumn = null;
                        yLine = lastY;
                        return NO_MORE_COLUMN;
                    }

                    if (simulate || !keepCandidate)
                    {
                        break;
                    }

                    if (keep == 0)
                    {
                        CompositeColumn = null;
                        yLine = lastY;
                    }
                }

                firstPass = false;
                yLine = CompositeColumn.yLine;
                _linesWritten += CompositeColumn._linesWritten;
                descender = CompositeColumn.descender;
                if ((status & NO_MORE_TEXT) != 0)
                {
                    CompositeColumn = null;
                    CompositeElements.RemoveAt(0);
                    yLine -= para.SpacingAfter;
                }

                if ((status & NO_MORE_COLUMN) != 0)
                {
                    return NO_MORE_COLUMN;
                }
            }
            else if (element.Type == Element.LIST)
            {
                var list = (List)element;
                var items = list.Items;
                ListItem item = null;
                var listIndentation = list.IndentationLeft;
                var count = 0;
                var stack = new Stack<object[]>();
                for (var k = 0; k < items.Count; ++k)
                {
                    var obj = items[k];
                    if (obj is ListItem)
                    {
                        if (count == ListIdx)
                        {
                            item = (ListItem)obj;
                            break;
                        }

                        ++count;
                    }
                    else if (obj is List)
                    {
                        stack.Push(new object[] { list, k, listIndentation });
                        list = (List)obj;
                        items = list.Items;
                        listIndentation += list.IndentationLeft;
                        k = -1;
                        continue;
                    }

                    if (k == items.Count - 1)
                    {
                        if (stack.Count > 0)
                        {
                            var objs = stack.Pop();
                            list = (List)objs[0];
                            items = list.Items;
                            k = (int)objs[1];
                            listIndentation = (float)objs[2];
                        }
                    }
                }

                var status = 0;
                for (var keep = 0; keep < 2; ++keep)
                {
                    var lastY = yLine;
                    var createHere = false;
                    if (CompositeColumn == null)
                    {
                        if (item == null)
                        {
                            ListIdx = 0;
                            CompositeElements.RemoveAt(0);
                            goto main_loop;
                        }

                        CompositeColumn = new ColumnText(canvas);

                        CompositeColumn.UseAscender = firstPass && _useAscender;
                        CompositeColumn.Alignment = item.Alignment;
                        CompositeColumn.Indent = item.IndentationLeft + listIndentation + item.FirstLineIndent;
                        CompositeColumn.ExtraParagraphSpace = item.ExtraParagraphSpace;
                        CompositeColumn.FollowingIndent = CompositeColumn.Indent;
                        CompositeColumn.RightIndent = item.IndentationRight + list.IndentationRight;
                        CompositeColumn.SetLeading(item.Leading, item.MultipliedLeading);
                        CompositeColumn.RunDirection = runDirection;
                        CompositeColumn.ArabicOptions = _arabicOptions;
                        CompositeColumn.SpaceCharRatio = _spaceCharRatio;
                        CompositeColumn.AddText(item);
                        if (!firstPass)
                        {
                            yLine -= item.SpacingBefore;
                        }

                        createHere = true;
                    }

                    CompositeColumn.LeftX = LeftX;
                    CompositeColumn.RightX = RightX;
                    CompositeColumn.yLine = yLine;
                    CompositeColumn.RectangularWidth = RectangularWidth;
                    CompositeColumn.RectangularMode = RectangularMode;
                    CompositeColumn.MinY = MinY;
                    CompositeColumn.MaxY = MaxY;
                    var keepCandidate = item.KeepTogether && createHere && !firstPass;
                    status = CompositeColumn.Go(simulate || (keepCandidate && keep == 0));
                    UpdateFilledWidth(CompositeColumn._filledWidth);
                    if ((status & NO_MORE_TEXT) == 0 && keepCandidate)
                    {
                        CompositeColumn = null;
                        yLine = lastY;
                        return NO_MORE_COLUMN;
                    }

                    if (simulate || !keepCandidate)
                    {
                        break;
                    }

                    if (keep == 0)
                    {
                        CompositeColumn = null;
                        yLine = lastY;
                    }
                }

                firstPass = false;
                yLine = CompositeColumn.yLine;
                _linesWritten += CompositeColumn._linesWritten;
                descender = CompositeColumn.descender;
                if (!float.IsNaN(CompositeColumn._firstLineY) && !CompositeColumn._firstLineYDone)
                {
                    if (!simulate)
                    {
                        ShowTextAligned(canvas,
                                        Element.ALIGN_LEFT,
                                        new Phrase(item.ListSymbol),
                                        CompositeColumn.LeftX + listIndentation,
                                        CompositeColumn._firstLineY,
                                        0);
                    }

                    CompositeColumn._firstLineYDone = true;
                }

                if ((status & NO_MORE_TEXT) != 0)
                {
                    CompositeColumn = null;
                    ++ListIdx;
                    yLine -= item.SpacingAfter;
                }

                if ((status & NO_MORE_COLUMN) != 0)
                {
                    return NO_MORE_COLUMN;
                }
            }
            else if (element.Type == Element.PTABLE)
            {
                // don't write anything in the current column if there's no more space available
                if (yLine < MinY || yLine > MaxY)
                {
                    return NO_MORE_COLUMN;
                }

                // get the PdfPTable element
                var table = (PdfPTable)element;

                // we ignore tables without a body
                if (table.Size <= table.HeaderRows)
                {
                    CompositeElements.RemoveAt(0);
                    continue;
                }

                // offsets
                var yTemp = yLine;
                if (!firstPass && ListIdx == 0)
                {
                    yTemp -= table.SpacingBefore;
                }

                var yLineWrite = yTemp;

                // don't write anything in the current column if there's no more space available
                if (yTemp < MinY || yTemp > MaxY)
                {
                    return NO_MORE_COLUMN;
                }

                // coordinates
                CurrentLeading = 0;
                var x1 = LeftX;
                float tableWidth;
                if (table.LockedWidth)
                {
                    tableWidth = table.TotalWidth;
                    UpdateFilledWidth(tableWidth);
                }
                else
                {
                    tableWidth = RectangularWidth * table.WidthPercentage / 100f;
                    table.TotalWidth = tableWidth;
                }

                // how many header rows are real header rows; how many are footer rows?
                var headerRows = table.HeaderRows;
                var footerRows = table.FooterRows;
                if (footerRows > headerRows)
                {
                    footerRows = headerRows;
                }

                var realHeaderRows = headerRows - footerRows;
                var headerHeight = table.HeaderHeight;
                var footerHeight = table.FooterHeight;

                // make sure the header and footer fit on the page
                var skipHeader = table.SkipFirstHeader && ListIdx <= realHeaderRows &&
                                 (table.ElementComplete || ListIdx != realHeaderRows);
                if (!skipHeader)
                {
                    yTemp -= headerHeight;
                    if (yTemp < MinY || yTemp > MaxY)
                    {
                        if (firstPass)
                        {
                            CompositeElements.RemoveAt(0);
                            continue;
                        }

                        return NO_MORE_COLUMN;
                    }
                }

                // how many real rows (not header or footer rows) fit on a page?
                int k;
                if (ListIdx < headerRows)
                {
                    ListIdx = headerRows;
                }

                if (!table.ElementComplete)
                {
                    yTemp -= footerHeight;
                }

                for (k = ListIdx; k < table.Size; ++k)
                {
                    var rowHeight = table.GetRowHeight(k);
                    if (yTemp - rowHeight < MinY)
                    {
                        break;
                    }

                    yTemp -= rowHeight;
                }

                if (!table.ElementComplete)
                {
                    yTemp += footerHeight;
                }

                // either k is the first row that doesn't fit on the page (break);
                if (k < table.Size)
                {
                    if (table.SplitRows && (!table.SplitLate || (k == ListIdx && firstPass)))
                    {
                        if (!_splittedRow)
                        {
                            _splittedRow = true;
                            table = new PdfPTable(table);
                            CompositeElements[0] = table;
                            var rows = table.Rows;
                            for (var i = headerRows; i < ListIdx; ++i)
                            {
                                rows[i] = null;
                            }
                        }

                        var h = yTemp - MinY;
                        var newRow = table.GetRow(k).SplitRow(table, k, h);
                        if (newRow == null)
                        {
                            if (k == ListIdx)
                            {
                                return NO_MORE_COLUMN;
                            }
                        }
                        else
                        {
                            yTemp = MinY;
                            table.Rows.Insert(++k, newRow);
                        }
                    }
                    else if (!table.SplitRows && k == ListIdx && firstPass)
                    {
                        CompositeElements.RemoveAt(0);
                        _splittedRow = false;
                        continue;
                    }
                    else if (k == ListIdx && !firstPass && (!table.SplitRows || table.SplitLate) &&
                             (table.FooterRows == 0 || table.ElementComplete))
                    {
                        return NO_MORE_COLUMN;
                    }
                }

                // or k is the number of rows in the table (for loop was done).
                firstPass = false;
                // we draw the table (for real now)
                if (!simulate)
                {
                    // set the alignment
                    switch (table.HorizontalAlignment)
                    {
                        case Element.ALIGN_LEFT:
                            break;
                        case Element.ALIGN_RIGHT:
                            x1 += RectangularWidth - tableWidth;
                            break;
                        default:
                            x1 += (RectangularWidth - tableWidth) / 2f;
                            break;
                    }

                    // copy the rows that fit on the page in a new table nt
                    var nt = PdfPTable.ShallowCopy(table);
                    var sub = nt.Rows;

                    // first we add the real header rows (if necessary)
                    if (!skipHeader && realHeaderRows > 0)
                    {
                        sub.AddRange(table.GetRows(0, realHeaderRows));
                    }
                    else
                    {
                        nt.HeaderRows = footerRows;
                    }

                    // then we add the real content
                    sub.AddRange(table.GetRows(ListIdx, k));
                    // if k < table.size(), we must indicate that the new table is complete;
                    // otherwise no footers will be added (because iText thinks the table continues on the same page)
                    var showFooter = !table.SkipLastFooter;
                    if (k < table.Size)
                    {
                        nt.ElementComplete = true;
                        showFooter = true;
                    }

                    // we add the footer rows if necessary (not for incomplete tables)
                    for (var j = 0; j < footerRows && nt.ElementComplete && showFooter; ++j)
                    {
                        sub.Add(table.GetRow(j + realHeaderRows));
                    }

                    // we need a correction if the last row needs to be extended
                    float rowHeight = 0;
                    var index = sub.Count - 1;
                    if (showFooter)
                    {
                        index -= footerRows;
                    }

                    var last = sub[index];
                    if (table.ExtendLastRow)
                    {
                        rowHeight = last.MaxHeights;
                        last.MaxHeights = yTemp - MinY + rowHeight;
                        yTemp = MinY;
                    }

                    // now we render the rows of the new table
                    if (canvases != null)
                    {
                        nt.WriteSelectedRows(0, -1, x1, yLineWrite, canvases);
                    }
                    else
                    {
                        nt.WriteSelectedRows(0, -1, x1, yLineWrite, canvas);
                    }

                    if (table.ExtendLastRow)
                    {
                        last.MaxHeights = rowHeight;
                    }
                }
                else if (table.ExtendLastRow && MinY > PdfPRow.BOTTOM_LIMIT)
                {
                    yTemp = MinY;
                }

                yLine = yTemp;
                if (!(skipHeader || table.ElementComplete))
                {
                    yLine += footerHeight;
                }

                if (k >= table.Size)
                {
                    yLine -= table.SpacingAfter;
                    CompositeElements.RemoveAt(0);
                    _splittedRow = false;
                    ListIdx = 0;
                }
                else
                {
                    if (_splittedRow)
                    {
                        var rows = table.Rows;
                        for (var i = ListIdx; i < k; ++i)
                        {
                            rows[i] = null;
                        }
                    }

                    ListIdx = k;
                    return NO_MORE_COLUMN;
                }
            }
            else if (element.Type == Element.YMARK)
            {
                if (!simulate)
                {
                    var zh = (IDrawInterface)element;
                    zh.Draw(canvas, LeftX, MinY, RightX, MaxY, yLine);
                }

                CompositeElements.RemoveAt(0);
            }
            else
            {
                CompositeElements.RemoveAt(0);
            }
        }
    }

    private void addWaitingPhrase()
    {
        if (BidiLine == null && WaitPhrase != null)
        {
            BidiLine = new BidiLine();
            foreach (var ck in WaitPhrase.Chunks)
            {
                BidiLine.AddChunk(new PdfChunk(ck, null));
            }

            WaitPhrase = null;
        }
    }
}