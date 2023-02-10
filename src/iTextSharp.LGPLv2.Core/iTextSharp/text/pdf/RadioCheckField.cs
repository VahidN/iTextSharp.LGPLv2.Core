using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Creates a radio or a check field.
///     Example usage:
///     Document document = new Document(PageSize.A4, 50, 50, 50, 50);
///     PdfWriter writer = PdfWriter.GetInstance(document, new FileOutputStream("output.pdf"));
///     document.Open();
///     PdfContentByte cb = writer.GetDirectContent();
///     RadioCheckField bt = new RadioCheckField(writer, new Rectangle(100, 100, 200, 200), "radio", "v1");
///     bt.SetCheckType(RadioCheckField.TYPE_CIRCLE);
///     bt.SetBackgroundColor(Color.CYAN);
///     bt.SetBorderStyle(PdfBorderDictionary.STYLE_SOLID);
///     bt.SetBorderColor(Color.red);
///     bt.SetTextColor(Color.yellow);
///     bt.SetBorderWidth(BaseField.BORDER_WIDTH_THICK);
///     bt.SetChecked(false);
///     PdfFormField f1 = bt.GetRadioField();
///     bt.SetOnValue("v2");
///     bt.SetChecked(true);
///     bt.SetBox(new Rectangle(100, 300, 200, 400));
///     PdfFormField f2 = bt.GetRadioField();
///     bt.SetChecked(false);
///     PdfFormField top = bt.GetRadioGroup(true, false);
///     bt.SetOnValue("v3");
///     bt.SetBox(new Rectangle(100, 500, 200, 600));
///     PdfFormField f3 = bt.GetRadioField();
///     top.AddKid(f1);
///     top.AddKid(f2);
///     top.AddKid(f3);
///     writer.AddAnnotation(top);
///     bt = new RadioCheckField(writer, new Rectangle(300, 300, 400, 400), "check1", "Yes");
///     bt.SetCheckType(RadioCheckField.TYPE_CHECK);
///     bt.SetBorderWidth(BaseField.BORDER_WIDTH_THIN);
///     bt.SetBorderColor(Color.black);
///     bt.SetBackgroundColor(Color.white);
///     PdfFormField ck = bt.GetCheckField();
///     writer.AddAnnotation(ck);
///     document.Close();
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class RadioCheckField : BaseField
{
    /// <summary>
    ///     A field with the symbol check
    /// </summary>
    public const int TYPE_CHECK = 1;

    /// <summary>
    ///     A field with the symbol circle
    /// </summary>
    public const int TYPE_CIRCLE = 2;

    /// <summary>
    ///     A field with the symbol cross
    /// </summary>
    public const int TYPE_CROSS = 3;

    /// <summary>
    ///     A field with the symbol diamond
    /// </summary>
    public const int TYPE_DIAMOND = 4;

    /// <summary>
    ///     A field with the symbol square
    /// </summary>
    public const int TYPE_SQUARE = 5;

    /// <summary>
    ///     A field with the symbol star
    /// </summary>
    public const int TYPE_STAR = 6;

    private static readonly string[] _typeChars = { "4", "l", "8", "u", "n", "H" };

    /// <summary>
    ///     Holds value of property checkType.
    /// </summary>
    private int _checkType;

    /// <summary>
    ///     Holds value of property onValue.
    /// </summary>
    private string _onValue;

    /// <summary>
    ///     Holds value of property checked.
    /// </summary>
    private bool _vchecked;

    /// <summary>
    ///     Creates a new instance of RadioCheckField
    /// </summary>
    /// <param name="writer">the document  PdfWriter </param>
    /// <param name="box">the field location and dimensions</param>
    /// <param name="fieldName">the field name. It must not be  null </param>
    /// <param name="onValue">the value when the field is checked</param>
    public RadioCheckField(PdfWriter writer, Rectangle box, string fieldName, string onValue) :
        base(writer, box, fieldName)
    {
        OnValue = onValue;
        CheckType = TYPE_CIRCLE;
    }

    /// <summary>
    ///     Sets the state of the field to checked or unchecked.
    ///     and  false  for unchecked
    /// </summary>
    public bool Checked
    {
        get => _vchecked;
        set => _vchecked = value;
    }

    /// <summary>
    ///     Gets the check field.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>the check field</returns>
    public PdfFormField CheckField => GetField(false);

    /// <summary>
    ///     Sets the checked symbol. It can be
    ///     TYPE_CHECK ,
    ///     TYPE_CIRCLE ,
    ///     TYPE_CROSS ,
    ///     TYPE_DIAMOND ,
    ///     TYPE_SQUARE  and
    ///     TYPE_STAR .
    /// </summary>
    public int CheckType
    {
        get => _checkType;
        set
        {
            _checkType = value;
            if (_checkType < TYPE_CHECK || _checkType > TYPE_STAR)
            {
                _checkType = TYPE_CIRCLE;
            }

            Text = _typeChars[_checkType - 1];
            Font = BaseFont.CreateFont(BaseFont.ZAPFDINGBATS, BaseFont.WINANSI, false);
        }
    }

    /// <summary>
    ///     Sets the value when the field is checked.
    /// </summary>
    public string OnValue
    {
        get => _onValue;
        set => _onValue = value;
    }

    /// <summary>
    ///     Gets the radio field. It's only composed of the widget keys and must be used
    ///     with {@link #getRadioGroup(bool,bool)}.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>the radio field</returns>
    public PdfFormField RadioField => GetField(true);

    /// <summary>
    ///     Gets the field appearance.
    ///     for a check field
    ///     otherwise
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="isRadio"> true  for a radio field and  false </param>
    /// <param name="on"> true  for the checked state,  false </param>
    /// <returns>the appearance</returns>
    public PdfAppearance GetAppearance(bool isRadio, bool on)
    {
        if (isRadio && _checkType == TYPE_CIRCLE)
        {
            return GetAppearanceRadioCircle(on);
        }

        var app = GetBorderAppearance();
        if (!on)
        {
            return app;
        }

        var ufont = RealFont;
        var borderExtra = borderStyle == PdfBorderDictionary.STYLE_BEVELED ||
                          borderStyle == PdfBorderDictionary.STYLE_INSET;
        var h = box.Height - borderWidth * 2;
        var bw2 = borderWidth;
        if (borderExtra)
        {
            h -= borderWidth * 2;
            bw2 *= 2;
        }

        var offsetX = borderExtra ? 2 * borderWidth : borderWidth;
        offsetX = Math.Max(offsetX, 1);
        var offX = Math.Min(bw2, offsetX);
        var wt = box.Width - 2 * offX;
        var ht = box.Height - 2 * offX;
        var fsize = fontSize;
        if (fsize.ApproxEquals(0))
        {
            var bw = ufont.GetWidthPoint(text, 1);
            if (bw.ApproxEquals(0))
            {
                fsize = 12;
            }
            else
            {
                fsize = wt / bw;
            }

            var nfsize = h / ufont.GetFontDescriptor(BaseFont.ASCENT, 1);
            fsize = Math.Min(fsize, nfsize);
        }

        app.SaveState();
        app.Rectangle(offX, offX, wt, ht);
        app.Clip();
        app.NewPath();
        if (textColor == null)
        {
            app.ResetGrayFill();
        }
        else
        {
            app.SetColorFill(textColor);
        }

        app.BeginText();
        app.SetFontAndSize(ufont, fsize);
        app.SetTextMatrix((box.Width - ufont.GetWidthPoint(text, fsize)) / 2,
                          (box.Height - ufont.GetAscentPoint(text, fsize)) / 2);
        app.ShowText(text);
        app.EndText();
        app.RestoreState();
        return app;
    }

    /// <summary>
    ///     Gets the special field appearance for the radio circle.
    ///     otherwise
    /// </summary>
    /// <param name="on"> true  for the checked state,  false </param>
    /// <returns>the appearance</returns>
    public PdfAppearance GetAppearanceRadioCircle(bool on)
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

        var boxc = new Rectangle(app.BoundingBox);
        var cx = boxc.Width / 2;
        var cy = boxc.Height / 2;
        var r = (Math.Min(boxc.Width, boxc.Height) - borderWidth) / 2;
        if (r <= 0)
        {
            return app;
        }

        if (backgroundColor != null)
        {
            app.SetColorFill(backgroundColor);
            app.Circle(cx, cy, r + borderWidth / 2);
            app.Fill();
        }

        if (borderWidth > 0 && borderColor != null)
        {
            app.SetLineWidth(borderWidth);
            app.SetColorStroke(borderColor);
            app.Circle(cx, cy, r);
            app.Stroke();
        }

        if (on)
        {
            if (textColor == null)
            {
                app.ResetGrayFill();
            }
            else
            {
                app.SetColorFill(textColor);
            }

            app.Circle(cx, cy, r / 2);
            app.Fill();
        }

        return app;
    }

    /// <summary>
    ///     Gets a radio group. It's composed of the field specific keys, without the widget
    ///     ones. This field is to be used as a field aggregator with {@link PdfFormField#addKid(PdfFormField) AddKid()}.
    ///     times; clicking the currently selected button has no effect.
    ///     If  false , clicking
    ///     the selected button deselects it, leaving no button selected.
    ///     use the same value for the on state will turn on and off in unison; that is if
    ///     one is checked, they are all checked. If  false , the buttons are mutually exclusive
    ///     (the same behavior as HTML radio buttons)
    /// </summary>
    /// <param name="noToggleToOff">if  true , exactly one radio button must be selected at all</param>
    /// <param name="radiosInUnison">if  true , a group of radio buttons within a radio button field that</param>
    /// <returns>the radio group</returns>
    public PdfFormField GetRadioGroup(bool noToggleToOff, bool radiosInUnison)
    {
        var field = PdfFormField.CreateRadioButton(writer, noToggleToOff);
        if (radiosInUnison)
        {
            field.SetFieldFlags(PdfFormField.FF_RADIOSINUNISON);
        }

        field.FieldName = fieldName;
        if ((options & READ_ONLY) != 0)
        {
            field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
        }

        if ((options & REQUIRED) != 0)
        {
            field.SetFieldFlags(PdfFormField.FF_REQUIRED);
        }

        field.ValueAsName = _vchecked ? _onValue : "Off";
        return field;
    }

    /// <summary>
    ///     Gets a radio or check field.
    ///     a check field
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="isRadio"> true  to get a radio field,  false  to get</param>
    /// <returns>the field</returns>
    protected PdfFormField GetField(bool isRadio)
    {
        PdfFormField field = null;
        if (isRadio)
        {
            field = PdfFormField.CreateEmpty(writer);
        }
        else
        {
            field = PdfFormField.CreateCheckBox(writer);
        }

        field.SetWidget(box, PdfAnnotation.HighlightInvert);
        if (!isRadio)
        {
            field.FieldName = fieldName;
            if ((options & READ_ONLY) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_READ_ONLY);
            }

            if ((options & REQUIRED) != 0)
            {
                field.SetFieldFlags(PdfFormField.FF_REQUIRED);
            }

            field.ValueAsName = _vchecked ? _onValue : "Off";
        }

        if (text != null)
        {
            field.MkNormalCaption = text;
        }

        if (rotation != 0)
        {
            field.MkRotation = rotation;
        }

        field.BorderStyle = new PdfBorderDictionary(borderWidth, borderStyle, new PdfDashPattern(3));
        var tpon = GetAppearance(isRadio, true);
        var tpoff = GetAppearance(isRadio, false);
        field.SetAppearance(PdfAnnotation.AppearanceNormal, _onValue, tpon);
        field.SetAppearance(PdfAnnotation.AppearanceNormal, "Off", tpoff);
        field.AppearanceState = _vchecked ? _onValue : "Off";
        var da = (PdfAppearance)tpon.Duplicate;
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
}