using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public abstract class BaseField
{
    /// <summary>
    ///     A medium border with 2 point width.
    /// </summary>
    public const float BORDER_WIDTH_MEDIUM = 2;

    /// <summary>
    ///     A thick border with 3 point width.
    /// </summary>
    public const float BORDER_WIDTH_THICK = 3;

    /// <summary>
    ///     A thin border with 1 point width.
    /// </summary>
    public const float BORDER_WIDTH_THIN = 1;

    /// <summary>
    ///     combo box flag.
    /// </summary>
    public const int COMB = PdfFormField.FF_COMB;

    /// <summary>
    ///     The field will not scroll (horizontally for single-line
    ///     fields, vertically for multiple-line fields) to accommodate more text
    ///     than will fit within its annotation rectangle. Once the field is full, no
    ///     further text will be accepted.
    /// </summary>
    public const int DO_NOT_SCROLL = PdfFormField.FF_DONOTSCROLL;

    /// <summary>
    ///     The text entered in the field will not be spell-checked.
    ///     This flag is meaningful only in text fields and in combo
    ///     fields with the  EDIT  flag set.
    /// </summary>
    public const int DO_NOT_SPELL_CHECK = PdfFormField.FF_DONOTSPELLCHECK;

    /// <summary>
    ///     If set the combo box includes an editable text box as well as a drop list; if
    ///     clear, it includes only a drop list.
    ///     This flag is only meaningful with combo fields.
    /// </summary>
    public const int EDIT = PdfFormField.FF_EDIT;

    /// <summary>
    ///     The text entered in the field represents the pathname of
    ///     a file whose contents are to be submitted as the value of the field.
    /// </summary>
    public const int FILE_SELECTION = PdfFormField.FF_FILESELECT;

    /// <summary>
    ///     The field is hidden.
    /// </summary>
    public const int HIDDEN = 1;

    /// <summary>
    ///     The field is hidden but is printable.
    /// </summary>
    public const int HIDDEN_BUT_PRINTABLE = 3;

    /// <summary>
    ///     The field may contain multiple lines of text.
    ///     This flag is only meaningful with text fields.
    /// </summary>
    public const int MULTILINE = PdfFormField.FF_MULTILINE;

    /// <summary>
    ///     The field is intended for entering a secure password that should
    ///     not be echoed visibly to the screen.
    /// </summary>
    public const int PASSWORD = PdfFormField.FF_PASSWORD;

    /// <summary>
    ///     The user may not change the value of the field.
    /// </summary>
    public const int READ_ONLY = PdfFormField.FF_READ_ONLY;

    /// <summary>
    ///     The field must have a value at the time it is exported by a submit-form
    ///     action.
    /// </summary>
    public const int REQUIRED = PdfFormField.FF_REQUIRED;

    /// <summary>
    ///     The field is visible.
    /// </summary>
    public const int VISIBLE = 0;

    /// <summary>
    ///     The field is visible but does not print.
    /// </summary>
    public const int VISIBLE_BUT_DOES_NOT_PRINT = 2;

    private static readonly NullValueDictionary<PdfName, int> _fieldKeys = new();
    protected int alignment = Element.ALIGN_LEFT;
    protected BaseColor backgroundColor;
    protected BaseColor borderColor;
    protected int borderStyle = PdfBorderDictionary.STYLE_SOLID;
    protected float borderWidth = BORDER_WIDTH_THIN;
    protected Rectangle box;

    /// <summary>
    ///     Holds value of property fieldName.
    /// </summary>
    protected string fieldName;

    protected BaseFont font;
    protected float fontSize;

    /// <summary>
    ///     Holds value of property maxCharacterLength.
    /// </summary>
    protected int maxCharacterLength;

    /// <summary>
    ///     Holds value of property options.
    /// </summary>
    protected int options;

    /// <summary>
    ///     Holds value of property rotation.
    /// </summary>
    protected int rotation;

    protected string text;
    protected BaseColor textColor;

    /// <summary>
    ///     Holds value of property visibility.
    /// </summary>
    protected int visibility;

    protected PdfWriter writer;

    static BaseField()
    {
        foreach (var entry in PdfCopyFieldsImp.FieldKeys)
        {
            _fieldKeys[entry.Key] = entry.Value;
        }

        _fieldKeys[PdfName.T] = 1;
    }

    /// <summary>
    ///     Creates a new  TextField .
    ///     will be included in the field allowing it to be used as a kid field.
    /// </summary>
    /// <param name="writer">the document  PdfWriter </param>
    /// <param name="box">the field location and dimensions</param>
    /// <param name="fieldName">the field name. If  null  only the widget keys</param>
    protected BaseField(PdfWriter writer, Rectangle box, string fieldName)
    {
        this.writer = writer;
        Box = box;
        this.fieldName = fieldName;
    }

    /// <summary>
    ///     Sets the text horizontal alignment. It can be  Element.ALIGN_LEFT ,
    ///     Element.ALIGN_CENTER  and  Element.ALIGN_RIGHT .
    /// </summary>
    public int Alignment
    {
        set => alignment = value;
        get => alignment;
    }

    /// <summary>
    ///     Sets the background color. Set to  null  for
    ///     transparent background.
    /// </summary>
    public BaseColor BackgroundColor
    {
        set => backgroundColor = value;
        get => backgroundColor;
    }

    /// <summary>
    ///     Sets the border color. Set to  null  to remove
    ///     the border.
    /// </summary>
    public BaseColor BorderColor
    {
        set => borderColor = value;
        get => borderColor;
    }

    /// <summary>
    ///     Sets the border style. The styles are found in  PdfBorderDictionary
    ///     and can be  STYLE_SOLID ,  STYLE_DASHED ,
    ///     STYLE_BEVELED ,  STYLE_INSET  and
    ///     STYLE_UNDERLINE .
    /// </summary>
    public int BorderStyle
    {
        set => borderStyle = value;
        get => borderStyle;
    }

    /// <summary>
    ///     Sets the border width in points. To eliminate the border
    ///     set the border color to  null .
    /// </summary>
    public float BorderWidth
    {
        set => borderWidth = value;
        get => borderWidth;
    }

    /// <summary>
    ///     Sets the field dimension and position.
    /// </summary>
    public Rectangle Box
    {
        set
        {
            if (value == null)
            {
                box = null;
            }
            else
            {
                box = new Rectangle(value);
                box.Normalize();
            }
        }
        get => box;
    }

    /// <summary>
    ///     Sets the field name.
    ///     will be included in the field allowing it to be used as a kid field.
    /// </summary>
    public string FieldName
    {
        set => fieldName = value;
        get => fieldName;
    }

    /// <summary>
    ///     Sets the text font. If  null  then Helvetica
    ///     will be used.
    /// </summary>
    public BaseFont Font
    {
        set => font = value;
        get => font;
    }

    /// <summary>
    ///     Sets the font size. If 0 then auto-sizing will be used but
    ///     only for text fields.
    /// </summary>
    public float FontSize
    {
        set => fontSize = value;
        get => fontSize;
    }

    /// <summary>
    ///     Sets the maximum length of the field�s text, in characters.
    ///     It is only meaningful for text fields.
    /// </summary>
    public int MaxCharacterLength
    {
        set => maxCharacterLength = value;
        get => maxCharacterLength;
    }

    /// <summary>
    ///     Sets the option flags. The option flags can be a combination by oring of
    ///     READ_ONLY ,  REQUIRED ,
    ///     MULTILINE ,  DO_NOT_SCROLL ,
    ///     PASSWORD ,  FILE_SELECTION ,
    ///     DO_NOT_SPELL_CHECK  and  EDIT .
    /// </summary>
    public int Options
    {
        set => options = value;
        get => options;
    }

    /// <summary>
    ///     Sets the field rotation. This value should be the same as
    ///     the page rotation where the field will be shown.
    /// </summary>
    public int Rotation
    {
        set
        {
            if (value % 90 != 0)
            {
                throw new ArgumentException("Rotation must be a multiple of 90.");
            }

            rotation = value % 360;

            if (rotation < 0)
            {
                rotation += 360;
            }
        }
        get => rotation;
    }

    /// <summary>
    ///     Sets the text for text fields.
    /// </summary>
    public string Text
    {
        set => text = value;
        get => text;
    }

    /// <summary>
    ///     Sets the text color. If  null  the color used
    ///     will be black.
    /// </summary>
    public BaseColor TextColor
    {
        set => textColor = value;
        get => textColor;
    }

    /// <summary>
    ///     Sets the field visibility flag. This flags can be one of
    ///     VISIBLE ,  HIDDEN ,  VISIBLE_BUT_DOES_NOT_PRINT
    ///     and  HIDDEN_BUT_PRINTABLE .
    /// </summary>
    public int Visibility
    {
        set => visibility = value;
        get => visibility;
    }

    public PdfWriter Writer
    {
        get => writer;
        set => writer = value;
    }

    protected BaseFont RealFont
    {
        get
        {
            if (font == null)
            {
                return BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, false);
            }

            return font;
        }
    }

    /// <summary>
    ///     Moves the field keys from  from  to  to . The moved keys
    ///     are removed from  from .
    /// </summary>
    /// <param name="from">the source</param>
    /// <param name="to">the destination. It may be  null </param>
    public static void MoveFields(PdfDictionary from, PdfDictionary to)
    {
        if (from == null)
        {
            throw new ArgumentNullException(nameof(from));
        }

        var keys = new PdfName[from.Size];

        foreach (var key in keys)
        {
            if (_fieldKeys.ContainsKey(key))
            {
                if (to != null)
                {
                    to.Put(key, from.Get(key));
                }

                from.Remove(key);
            }
        }
    }

    /// <summary>
    ///     Convenience method to set the field rotation the same as the
    ///     page rotation.
    /// </summary>
    /// <param name="page">the page</param>
    public void SetRotationFromPage(Rectangle page)
    {
        if (page == null)
        {
            throw new ArgumentNullException(nameof(page));
        }

        Rotation = page.Rotation;
    }

    protected static IList<string> BreakLines(List<string> breaks, BaseFont font, float fontSize, float width)
    {
        if (breaks == null)
        {
            throw new ArgumentNullException(nameof(breaks));
        }

        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        List<string> lines = new();
        var buf = new StringBuilder();

        for (var ck = 0; ck < breaks.Count; ++ck)
        {
            buf.Length = 0;
            float w = 0;
            var cs = breaks[ck].ToCharArray();
            var len = cs.Length;

            // 0 inline first, 1 inline, 2 spaces
            var state = 0;
            var lastspace = -1;
            var c = (char)0;
            var refk = 0;

            for (var k = 0; k < len; ++k)
            {
                c = cs[k];

                switch (state)
                {
                    case 0:
                        w += font.GetWidthPoint(c, fontSize);
                        buf.Append(c);

                        if (w > width)
                        {
                            w = 0;

                            if (buf.Length > 1)
                            {
                                --k;
                                buf.Length = buf.Length - 1;
                            }

                            lines.Add(buf.ToString());
                            buf.Length = 0;
                            refk = k;

                            if (c == ' ')
                            {
                                state = 2;
                            }
                            else
                            {
                                state = 1;
                            }
                        }
                        else
                        {
                            if (c != ' ')
                            {
                                state = 1;
                            }
                        }

                        break;
                    case 1:
                        w += font.GetWidthPoint(c, fontSize);
                        buf.Append(c);

                        if (c == ' ')
                        {
                            lastspace = k;
                        }

                        if (w > width)
                        {
                            w = 0;

                            if (lastspace >= 0)
                            {
                                k = lastspace;
                                buf.Length = lastspace - refk;
                                TrimRight(buf);
                                lines.Add(buf.ToString());
                                buf.Length = 0;
                                refk = k;
                                lastspace = -1;
                                state = 2;
                            }
                            else
                            {
                                if (buf.Length > 1)
                                {
                                    --k;
                                    buf.Length = buf.Length - 1;
                                }

                                lines.Add(buf.ToString());
                                buf.Length = 0;
                                refk = k;

                                if (c == ' ')
                                {
                                    state = 2;
                                }
                            }
                        }

                        break;
                    case 2:
                        if (c != ' ')
                        {
                            w = 0;
                            --k;
                            state = 1;
                        }

                        break;
                }
            }

            TrimRight(buf);
            lines.Add(buf.ToString());
        }

        return lines;
    }

    protected static IList<string> GetHardBreaks(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        List<string> arr = new();
        var cs = text.ToCharArray();
        var len = cs.Length;
        var buf = new StringBuilder();

        for (var k = 0; k < len; ++k)
        {
            var c = cs[k];

            if (c == '\r')
            {
                if (k + 1 < len && cs[k + 1] == '\n')
                {
                    ++k;
                }

                arr.Add(buf.ToString());
                buf = new StringBuilder();
            }
            else if (c == '\n')
            {
                arr.Add(buf.ToString());
                buf = new StringBuilder();
            }
            else
            {
                buf.Append(c);
            }
        }

        arr.Add(buf.ToString());

        return arr;
    }

    protected static void TrimRight(StringBuilder buf)
    {
        if (buf == null)
        {
            throw new ArgumentNullException(nameof(buf));
        }

        var len = buf.Length;

        while (true)
        {
            if (len == 0)
            {
                return;
            }

            if (buf[--len] != ' ')
            {
                return;
            }

            buf.Length = len;
        }
    }

    protected PdfAppearance GetBorderAppearance()
    {
        var app = PdfAppearance.CreateAppearance(writer, box.Width, box.Height);

        switch (rotation)
        {
            case 90:
                app.SetMatrix(0, 1, -1, 0, box.Height, 0);

                break;
            case 180:
                app.SetMatrix(-1, 0, 0, -1, box.Width, box.Height);

                break;
            case 270:
                app.SetMatrix(0, -1, 1, 0, 0, box.Width);

                break;
        }

        app.SaveState();

        // background
        if (backgroundColor != null)
        {
            app.SetColorFill(backgroundColor);
            app.Rectangle(0, 0, box.Width, box.Height);
            app.Fill();
        }

        // border
        if (borderStyle == PdfBorderDictionary.STYLE_UNDERLINE)
        {
            if (borderWidth.ApproxNotEqual(0) && borderColor != null)
            {
                app.SetColorStroke(borderColor);
                app.SetLineWidth(borderWidth);
                app.MoveTo(0, borderWidth / 2);
                app.LineTo(box.Width, borderWidth / 2);
                app.Stroke();
            }
        }
        else if (borderStyle == PdfBorderDictionary.STYLE_BEVELED)
        {
            if (borderWidth.ApproxNotEqual(0) && borderColor != null)
            {
                app.SetColorStroke(borderColor);
                app.SetLineWidth(borderWidth);
                app.Rectangle(borderWidth / 2, borderWidth / 2, box.Width - borderWidth, box.Height - borderWidth);
                app.Stroke();
            }

            // beveled
            var actual = backgroundColor;

            if (actual == null)
            {
                actual = BaseColor.White;
            }

            app.SetGrayFill(1);
            drawTopFrame(app);
            app.SetColorFill(actual.Darker());
            drawBottomFrame(app);
        }
        else if (borderStyle == PdfBorderDictionary.STYLE_INSET)
        {
            if (borderWidth.ApproxNotEqual(0) && borderColor != null)
            {
                app.SetColorStroke(borderColor);
                app.SetLineWidth(borderWidth);
                app.Rectangle(borderWidth / 2, borderWidth / 2, box.Width - borderWidth, box.Height - borderWidth);
                app.Stroke();
            }

            // inset
            app.SetGrayFill(0.5f);
            drawTopFrame(app);
            app.SetGrayFill(0.75f);
            drawBottomFrame(app);
        }
        else
        {
            if (borderWidth.ApproxNotEqual(0) && borderColor != null)
            {
                if (borderStyle == PdfBorderDictionary.STYLE_DASHED)
                {
                    app.SetLineDash(3, 0);
                }

                app.SetColorStroke(borderColor);
                app.SetLineWidth(borderWidth);
                app.Rectangle(borderWidth / 2, borderWidth / 2, box.Width - borderWidth, box.Height - borderWidth);
                app.Stroke();

                if ((options & COMB) != 0 && maxCharacterLength > 1)
                {
                    var step = box.Width / maxCharacterLength;
                    var yb = borderWidth / 2;
                    var yt = box.Height - borderWidth / 2;

                    for (var k = 1; k < maxCharacterLength; ++k)
                    {
                        var x = step * k;
                        app.MoveTo(x, yb);
                        app.LineTo(x, yt);
                    }

                    app.Stroke();
                }
            }
        }

        app.RestoreState();

        return app;
    }

    private void drawBottomFrame(PdfAppearance app)
    {
        app.MoveTo(borderWidth, borderWidth);
        app.LineTo(box.Width - borderWidth, borderWidth);
        app.LineTo(box.Width - borderWidth, box.Height - borderWidth);
        app.LineTo(box.Width - 2 * borderWidth, box.Height - 2 * borderWidth);
        app.LineTo(box.Width - 2 * borderWidth, 2 * borderWidth);
        app.LineTo(2 * borderWidth, 2 * borderWidth);
        app.LineTo(borderWidth, borderWidth);
        app.Fill();
    }

    private void drawTopFrame(PdfAppearance app)
    {
        app.MoveTo(borderWidth, borderWidth);
        app.LineTo(borderWidth, box.Height - borderWidth);
        app.LineTo(box.Width - borderWidth, box.Height - borderWidth);
        app.LineTo(box.Width - 2 * borderWidth, box.Height - 2 * borderWidth);
        app.LineTo(2 * borderWidth, box.Height - 2 * borderWidth);
        app.LineTo(2 * borderWidth, 2 * borderWidth);
        app.LineTo(borderWidth, borderWidth);
        app.Fill();
    }
}