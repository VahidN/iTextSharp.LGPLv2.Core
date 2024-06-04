using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Supports text, combo and list fields generating the correct appearances.
///     All the option in the Acrobat GUI are supported in an easy to use API.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class TextField : BaseField
{
    /// <summary>
    ///     Holds value of property choiceExports.
    /// </summary>
    private string[] _choiceExports;

    /// <summary>
    ///     Holds value of property choices.
    /// </summary>
    private string[] _choices;

    /// <summary>
    ///     Holds value of property choiceSelection.
    /// </summary>
    private int _choiceSelection;

    /// <summary>
    ///     Holds value of property defaultText.
    /// </summary>
    private string _defaultText;

    /// <summary>
    ///     Holds value of property extensionFont.
    /// </summary>
    private BaseFont _extensionFont;

    private float _extraMarginLeft;
    private float _extraMarginTop;

    /// <summary>
    ///     Holds value of property substitutionFonts.
    /// </summary>
    private IList<BaseFont> _substitutionFonts;

    /// <summary>
    ///     Creates a new  TextField .
    ///     will be included in the field allowing it to be used as a kid field.
    /// </summary>
    /// <param name="writer">the document  PdfWriter </param>
    /// <param name="box">the field location and dimensions</param>
    /// <param name="fieldName">the field name. If  null  only the widget keys</param>
    public TextField(PdfWriter writer, Rectangle box, string fieldName) : base(writer, box, fieldName)
    {
    }

    /// <summary>
    ///     Sets the export values in list/combo fields. If this array
    ///     is  null  then the choice values will also be used
    ///     as the export values.
    /// </summary>
    public string[] ChoiceExports
    {
        get => _choiceExports;
        set => _choiceExports = value;
    }

    /// <summary>
    ///     Sets the choices to be presented to the user in list/combo
    ///     fields.
    /// </summary>
    public string[] Choices
    {
        get => _choices;
        set => _choices = value;
    }

    /// <summary>
    ///     Sets the zero based index of the selected item.
    /// </summary>
    public int ChoiceSelection
    {
        get => _choiceSelection;
        set => _choiceSelection = value;
    }

    /// <summary>
    ///     Sets the default text. It is only meaningful for text fields.
    /// </summary>
    public string DefaultText
    {
        get => _defaultText;
        set => _defaultText = value;
    }

    /// <summary>
    ///     Sets the extensionFont. This font will be searched before the
    ///     substitution fonts. It may be  null .
    /// </summary>
    public BaseFont ExtensionFont
    {
        set => _extensionFont = value;
        get => _extensionFont;
    }

    /// <summary>
    ///     Sets a list of substitution fonts. The list is composed of  BaseFont  and can also be  null . The fonts in this
    ///     list will be used if the original
    ///     font doesn't contain the needed glyphs.
    /// </summary>
    public IList<BaseFont> SubstitutionFonts
    {
        set => _substitutionFonts = value;
        get => _substitutionFonts;
    }

    internal int TopFirst { get; private set; }

    /// <summary>
    ///     Obfuscates a password  String .
    ///     Every character is replaced by an asterisk (*).
    ///     @since   2.1.5
    /// </summary>
    /// <param name="text"></param>
    /// <returns>String</returns>
    public static string ObfuscatePassword(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        return new string('*', text.Length);
    }

    public static string RemoveCrlf(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (text.IndexOf("\n", StringComparison.Ordinal) >= 0 || text.IndexOf("\r", StringComparison.Ordinal) >= 0)
        {
            var p = text.ToCharArray();
            var sb = new StringBuilder(p.Length);
            for (var k = 0; k < p.Length; ++k)
            {
                var c = p[k];
                if (c == '\n')
                {
                    sb.Append(' ');
                }
                else if (c == '\r')
                {
                    sb.Append(' ');
                    if (k < p.Length - 1 && p[k + 1] == '\n')
                    {
                        ++k;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        return text;
    }

    /// <summary>
    ///     Get the  PdfAppearance  of a text or combo field
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>A  PdfAppearance </returns>
    public PdfAppearance GetAppearance()
    {
        var app = GetBorderAppearance();
        app.BeginVariableText();
        if (string.IsNullOrEmpty(text))
        {
            app.EndVariableText();
            return app;
        }

        var borderExtra = borderStyle == PdfBorderDictionary.STYLE_BEVELED ||
                          borderStyle == PdfBorderDictionary.STYLE_INSET;
        var h = box.Height - borderWidth * 2 - _extraMarginTop;
        var bw2 = borderWidth;
        if (borderExtra)
        {
            h -= borderWidth * 2;
            bw2 *= 2;
        }

        var offsetX = Math.Max(bw2, 1);
        var offX = Math.Min(bw2, offsetX);
        app.SaveState();
        app.Rectangle(offX, offX, box.Width - 2 * offX, box.Height - 2 * offX);
        app.Clip();
        app.NewPath();
        string ptext;
        if ((options & PASSWORD) != 0)
        {
            ptext = ObfuscatePassword(text);
        }
        else if ((options & MULTILINE) == 0)
        {
            ptext = RemoveCrlf(text);
        }
        else
        {
            ptext = text; //fixed by Kazuya Ujihara (ujihara.jp)
        }

        var ufont = RealFont;
        var fcolor = textColor == null ? GrayColor.Grayblack : textColor;
        var rtl = checkRtl(ptext) ? PdfWriter.RUN_DIRECTION_LTR : PdfWriter.RUN_DIRECTION_NO_BIDI;
        var usize = fontSize;
        var phrase = composePhrase(ptext, ufont, fcolor, usize);
        if ((options & MULTILINE) != 0)
        {
            var width = box.Width - 4 * offsetX - _extraMarginLeft;
            var factor = ufont.GetFontDescriptor(BaseFont.BBOXURY, 1) - ufont.GetFontDescriptor(BaseFont.BBOXLLY, 1);
            var ct = new ColumnText(null);
            if (usize.ApproxEquals(0))
            {
                usize = h / factor;
                if (usize > 4)
                {
                    if (usize > 12)
                    {
                        usize = 12;
                    }

                    var step = Math.Max((usize - 4) / 10, 0.2f);
                    ct.SetSimpleColumn(0, -h, width, 0);
                    ct.Alignment = alignment;
                    ct.RunDirection = rtl;
                    for (; usize > 4; usize -= step)
                    {
                        ct.YLine = 0;
                        changeFontSize(phrase, usize);
                        ct.SetText(phrase);
                        ct.Leading = factor * usize;
                        var status = ct.Go(true);
                        if ((status & ColumnText.NO_MORE_COLUMN) == 0)
                        {
                            break;
                        }
                    }
                }

                if (usize < 4)
                {
                    usize = 4;
                }
            }

            changeFontSize(phrase, usize);
            ct.Canvas = app;
            var leading = usize * factor;
            var offsetY = offsetX + h - ufont.GetFontDescriptor(BaseFont.BBOXURY, usize);
            ct.SetSimpleColumn(_extraMarginLeft + 2 * offsetX, -20000, box.Width - 2 * offsetX, offsetY + leading);
            ct.Leading = leading;
            ct.Alignment = alignment;
            ct.RunDirection = rtl;
            ct.SetText(phrase);
            ct.Go();
        }
        else
        {
            if (usize.ApproxEquals(0))
            {
                var maxCalculatedSize = h / (ufont.GetFontDescriptor(BaseFont.BBOXURX, 1) -
                                             ufont.GetFontDescriptor(BaseFont.BBOXLLY, 1));
                changeFontSize(phrase, 1);
                var wd = ColumnText.GetWidth(phrase, rtl, 0);
                if (wd.ApproxEquals(0))
                {
                    usize = maxCalculatedSize;
                }
                else
                {
                    usize = Math.Min(maxCalculatedSize, (box.Width - _extraMarginLeft - 4 * offsetX) / wd);
                }

                if (usize < 4)
                {
                    usize = 4;
                }
            }

            changeFontSize(phrase, usize);
            var offsetY = offX + (box.Height - 2 * offX - ufont.GetFontDescriptor(BaseFont.ASCENT, usize)) / 2;
            if (offsetY < offX)
            {
                offsetY = offX;
            }

            if (offsetY - offX < -ufont.GetFontDescriptor(BaseFont.DESCENT, usize))
            {
                var ny = -ufont.GetFontDescriptor(BaseFont.DESCENT, usize) + offX;
                var dy = box.Height - offX - ufont.GetFontDescriptor(BaseFont.ASCENT, usize);
                offsetY = Math.Min(ny, Math.Max(offsetY, dy));
            }

            if ((options & COMB) != 0 && maxCharacterLength > 0)
            {
                var textLen = Math.Min(maxCharacterLength, ptext.Length);
                var position = 0;
                if (alignment == Element.ALIGN_RIGHT)
                {
                    position = maxCharacterLength - textLen;
                }
                else if (alignment == Element.ALIGN_CENTER)
                {
                    position = (maxCharacterLength - textLen) / 2;
                }

                var step = (box.Width - _extraMarginLeft) / maxCharacterLength;
                var start = step / 2 + position * step;
                if (textColor == null)
                {
                    app.SetGrayFill(0);
                }
                else
                {
                    app.SetColorFill(textColor);
                }

                app.BeginText();
                foreach (Chunk ck in phrase)
                {
                    var bf = ck.Font.BaseFont;
                    app.SetFontAndSize(bf, usize);
                    var sb = ck.Append("");
                    for (var j = 0; j < sb.Length; ++j)
                    {
                        var c = sb.ToString(j, 1);
                        var wd = bf.GetWidthPoint(c, usize);
                        app.SetTextMatrix(_extraMarginLeft + start - wd / 2, offsetY - _extraMarginTop);
                        app.ShowText(c);
                        start += step;
                    }
                }

                app.EndText();
            }
            else
            {
                float x;
                switch (alignment)
                {
                    case Element.ALIGN_RIGHT:
                        x = _extraMarginLeft + box.Width - 2 * offsetX;
                        break;
                    case Element.ALIGN_CENTER:
                        x = _extraMarginLeft + box.Width / 2;
                        break;
                    default:
                        x = _extraMarginLeft + 2 * offsetX;
                        break;
                }

                ColumnText.ShowTextAligned(app, alignment, phrase, x, offsetY - _extraMarginTop, 0, rtl, 0);
            }
        }

        app.RestoreState();
        app.EndVariableText();
        return app;
    }

    /// <summary>
    ///     Gets a new combo field.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>a new combo field</returns>
    public PdfFormField GetComboField() => GetChoiceField(false);

    /// <summary>
    ///     Gets a new list field.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>a new list field</returns>
    public PdfFormField GetListField() => GetChoiceField(true);

    /// <summary>
    ///     Gets a new text field.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>a new text field</returns>
    public PdfFormField GetTextField()
    {
        if (maxCharacterLength <= 0)
        {
            options &= ~COMB;
        }

        if ((options & COMB) != 0)
        {
            options &= ~MULTILINE;
        }

        var field = PdfFormField.CreateTextField(writer, false, false, maxCharacterLength);
        field.SetWidget(box, PdfAnnotation.HighlightInvert);
        switch (alignment)
        {
            case Element.ALIGN_CENTER:
                field.Quadding = PdfFormField.Q_CENTER;
                break;
            case Element.ALIGN_RIGHT:
                field.Quadding = PdfFormField.Q_RIGHT;
                break;
        }

        if (rotation != 0)
        {
            field.MkRotation = rotation;
        }

        if (fieldName != null)
        {
            field.FieldName = fieldName;
            if (!"".Equals(text, StringComparison.Ordinal))
            {
                field.ValueAsString = text;
            }

            if (_defaultText != null)
            {
                field.DefaultValueAsString = _defaultText;
            }

            if ((options & READ_ONLY) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
            }

            if ((options & REQUIRED) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_REQUIRED);
            }

            if ((options & MULTILINE) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_MULTILINE);
            }

            if ((options & DO_NOT_SCROLL) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_DONOTSCROLL);
            }

            if ((options & PASSWORD) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_PASSWORD);
            }

            if ((options & FILE_SELECTION) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_FILESELECT);
            }

            if ((options & DO_NOT_SPELL_CHECK) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_DONOTSPELLCHECK);
            }

            if ((options & COMB) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_COMB);
            }
        }

        field.BorderStyle = new PdfBorderDictionary(borderWidth, borderStyle, new PdfDashPattern(3));
        var tp = GetAppearance();
        field.SetAppearance(PdfAnnotation.AppearanceNormal, tp);
        var da = (PdfAppearance)tp.Duplicate;
        da.SetFontAndSize(RealFont, fontSize);
        if (textColor == null)
        {
            da.SetGrayFill(0);
        }
        else
        {
            da.SetColorFill(textColor);
        }

        field.DefaultAppearanceString = da;
        if (borderColor != null)
        {
            field.MkBorderColor = borderColor;
        }

        if (backgroundColor != null)
        {
            field.MkBackgroundColor = backgroundColor;
        }

        switch (visibility)
        {
            case HIDDEN:
                field.Flags = PdfAnnotation.FLAGS_PRINT | PdfAnnotation.FLAGS_HIDDEN;
                break;
            case VISIBLE_BUT_DOES_NOT_PRINT:
                break;
            case HIDDEN_BUT_PRINTABLE:
                field.Flags = PdfAnnotation.FLAGS_PRINT | PdfAnnotation.FLAGS_NOVIEW;
                break;
            default:
                field.Flags = PdfAnnotation.FLAGS_PRINT;
                break;
        }

        return field;
    }

    /// <summary>
    ///     Sets extra margins in text fields to better mimic the Acrobat layout.
    /// </summary>
    /// <param name="extraMarginLeft">the extra marging left</param>
    /// <param name="extraMarginTop">the extra margin top</param>
    public void SetExtraMargin(float extraMarginLeft, float extraMarginTop)
    {
        _extraMarginLeft = extraMarginLeft;
        _extraMarginTop = extraMarginTop;
    }

    /// <summary>
    ///     Get the  PdfAppearance  of a list field
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>A  PdfAppearance </returns>
    internal PdfAppearance GetListAppearance()
    {
        var app = GetBorderAppearance();
        app.BeginVariableText();
        if (_choices == null || _choices.Length == 0)
        {
            app.EndVariableText();
            return app;
        }

        var topChoice = _choiceSelection;
        if (topChoice >= _choices.Length)
        {
            topChoice = _choices.Length - 1;
        }

        if (topChoice < 0)
        {
            topChoice = 0;
        }

        var ufont = RealFont;
        var usize = fontSize;
        if (usize.ApproxEquals(0))
        {
            usize = 12;
        }

        var borderExtra = borderStyle == PdfBorderDictionary.STYLE_BEVELED ||
                          borderStyle == PdfBorderDictionary.STYLE_INSET;
        var h = box.Height - borderWidth * 2;
        var offsetX = borderWidth;
        if (borderExtra)
        {
            h -= borderWidth * 2;
            offsetX *= 2;
        }

        var leading = ufont.GetFontDescriptor(BaseFont.BBOXURY, usize) -
                      ufont.GetFontDescriptor(BaseFont.BBOXLLY, usize);
        var maxFit = (int)(h / leading) + 1;
        var first = 0;
        var last = 0;
        last = topChoice + maxFit / 2 + 1;
        first = last - maxFit;
        if (first < 0)
        {
            last += first;
            first = 0;
        }

        //        first = topChoice;
        last = first + maxFit;
        if (last > _choices.Length)
        {
            last = _choices.Length;
        }

        TopFirst = first;
        app.SaveState();
        app.Rectangle(offsetX, offsetX, box.Width - 2 * offsetX, box.Height - 2 * offsetX);
        app.Clip();
        app.NewPath();
        var fcolor = textColor == null ? GrayColor.Grayblack : textColor;
        app.SetColorFill(new BaseColor(10, 36, 106));
        app.Rectangle(offsetX, offsetX + h - (topChoice - first + 1) * leading, box.Width - 2 * offsetX, leading);
        app.Fill();
        var xp = offsetX * 2;
        var yp = offsetX + h - ufont.GetFontDescriptor(BaseFont.BBOXURY, usize);
        for (var idx = first; idx < last; ++idx, yp -= leading)
        {
            var ptext = _choices[idx];
            var rtl = checkRtl(ptext) ? PdfWriter.RUN_DIRECTION_LTR : PdfWriter.RUN_DIRECTION_NO_BIDI;
            ptext = RemoveCrlf(ptext);
            var phrase = composePhrase(ptext, ufont, idx == topChoice ? GrayColor.Graywhite : fcolor, usize);
            ColumnText.ShowTextAligned(app, Element.ALIGN_LEFT, phrase, xp, yp, 0, rtl, 0);
        }

        app.RestoreState();
        app.EndVariableText();
        return app;
    }

    protected PdfFormField GetChoiceField(bool isList)
    {
        options &= ~MULTILINE & ~COMB;
        var uchoices = _choices;
        if (uchoices == null)
        {
            uchoices = Array.Empty<string>();
        }

        var topChoice = _choiceSelection;
        if (topChoice >= uchoices.Length)
        {
            topChoice = uchoices.Length - 1;
        }

        if (text == null)
        {
            text = ""; //fixed by Kazuya Ujihara (ujihara.jp)
        }

        if (topChoice >= 0)
        {
            text = uchoices[topChoice];
        }

        if (topChoice < 0)
        {
            topChoice = 0;
        }

        PdfFormField field = null;
        string[,] mix = null;
        if (_choiceExports == null)
        {
            if (isList)
            {
                field = PdfFormField.CreateList(writer, uchoices, topChoice);
            }
            else
            {
                field = PdfFormField.CreateCombo(writer, (options & EDIT) != 0, uchoices, topChoice);
            }
        }
        else
        {
            mix = new string[uchoices.Length, 2];
            for (var k = 0; k < mix.GetLength(0); ++k)
            {
                mix[k, 0] = mix[k, 1] = uchoices[k];
            }

            var top = Math.Min(uchoices.Length, _choiceExports.Length);
            for (var k = 0; k < top; ++k)
            {
                if (_choiceExports[k] != null)
                {
                    mix[k, 0] = _choiceExports[k];
                }
            }

            if (isList)
            {
                field = PdfFormField.CreateList(writer, mix, topChoice);
            }
            else
            {
                field = PdfFormField.CreateCombo(writer, (options & EDIT) != 0, mix, topChoice);
            }
        }

        field.SetWidget(box, PdfAnnotation.HighlightInvert);
        if (rotation != 0)
        {
            field.MkRotation = rotation;
        }

        if (fieldName != null)
        {
            field.FieldName = fieldName;
            if (uchoices.Length > 0)
            {
                if (mix != null)
                {
                    field.ValueAsString = mix[topChoice, 0];
                    field.DefaultValueAsString = mix[topChoice, 0];
                }
                else
                {
                    field.ValueAsString = text;
                    field.DefaultValueAsString = text;
                }
            }

            if ((options & READ_ONLY) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
            }

            if ((options & REQUIRED) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_REQUIRED);
            }

            if ((options & DO_NOT_SPELL_CHECK) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_DONOTSPELLCHECK);
            }
        }

        field.BorderStyle = new PdfBorderDictionary(borderWidth, borderStyle, new PdfDashPattern(3));
        PdfAppearance tp;
        if (isList)
        {
            tp = GetListAppearance();
            if (TopFirst > 0)
            {
                field.Put(PdfName.Ti, new PdfNumber(TopFirst));
            }
        }
        else
        {
            tp = GetAppearance();
        }

        field.SetAppearance(PdfAnnotation.AppearanceNormal, tp);
        var da = (PdfAppearance)tp.Duplicate;
        da.SetFontAndSize(RealFont, fontSize);
        if (textColor == null)
        {
            da.SetGrayFill(0);
        }
        else
        {
            da.SetColorFill(textColor);
        }

        field.DefaultAppearanceString = da;
        if (borderColor != null)
        {
            field.MkBorderColor = borderColor;
        }

        if (backgroundColor != null)
        {
            field.MkBackgroundColor = backgroundColor;
        }

        switch (visibility)
        {
            case HIDDEN:
                field.Flags = PdfAnnotation.FLAGS_PRINT | PdfAnnotation.FLAGS_HIDDEN;
                break;
            case VISIBLE_BUT_DOES_NOT_PRINT:
                break;
            case HIDDEN_BUT_PRINTABLE:
                field.Flags = PdfAnnotation.FLAGS_PRINT | PdfAnnotation.FLAGS_NOVIEW;
                break;
            default:
                field.Flags = PdfAnnotation.FLAGS_PRINT;
                break;
        }

        return field;
    }

    private static void changeFontSize(Phrase p, float size)
    {
        foreach (Chunk ck in p)
        {
            ck.Font.Size = size;
        }
    }

    private static bool checkRtl(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        var cc = text.ToCharArray();
        for (var k = 0; k < cc.Length; ++k)
        {
            int c = cc[k];
            if (c >= 0x590 && c < 0x0780)
            {
                return true;
            }
        }

        return false;
    }

    private Phrase composePhrase(string text, BaseFont ufont, BaseColor color, float fontSize)
    {
        Phrase phrase = null;
        if (_extensionFont == null && (_substitutionFonts == null || _substitutionFonts.Count == 0))
        {
            phrase = new Phrase(new Chunk(text, new Font(ufont, fontSize, 0, color)));
        }
        else
        {
            var fs = new FontSelector();
            fs.AddFont(new Font(ufont, fontSize, 0, color));
            if (_extensionFont != null)
            {
                fs.AddFont(new Font(_extensionFont, fontSize, 0, color));
            }

            if (_substitutionFonts != null)
            {
                foreach (var bf in _substitutionFonts)
                {
                    fs.AddFont(new Font(bf, fontSize, 0, color));
                }
            }

            phrase = fs.Process(text);
        }

        return phrase;
    }
}