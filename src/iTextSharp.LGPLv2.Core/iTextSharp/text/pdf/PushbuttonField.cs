using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Creates a pushbutton field. It supports all the text and icon alignments.
///     The icon may be an image or a template.
///     Example usage:
///     Document document = new Document(PageSize.A4, 50, 50, 50, 50);
///     PdfWriter writer = PdfWriter.GetInstance(document, new FileOutputStream("output.pdf"));
///     document.Open();
///     PdfContentByte cb = writer.GetDirectContent();
///     Image img = Image.GetInstance("image.png");
///     PushbuttonField bt = new PushbuttonField(writer, new Rectangle(100, 100, 200, 200), "Button1");
///     bt.SetText("My Caption");
///     bt.SetFontSize(0);
///     bt.SetImage(img);
///     bt.SetLayout(PushbuttonField.LAYOUT_ICON_TOP_LABEL_BOTTOM);
///     bt.SetBackgroundColor(Color.cyan);
///     bt.SetBorderStyle(PdfBorderDictionary.STYLE_SOLID);
///     bt.SetBorderColor(Color.red);
///     bt.SetBorderWidth(3);
///     PdfFormField ff = bt.GetField();
///     PdfAction ac = PdfAction.CreateSubmitForm("http://www.submit-site.com", null, 0);
///     ff.SetAction(ac);
///     writer.AddAnnotation(ff);
///     document.Close();
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PushbuttonField : BaseField
{
    /// <summary>
    ///     A layout option
    /// </summary>
    public const int LAYOUT_ICON_LEFT_LABEL_RIGHT = 5;

    /// <summary>
    ///     A layout option
    /// </summary>
    public const int LAYOUT_ICON_ONLY = 2;

    /// <summary>
    ///     A layout option
    /// </summary>
    public const int LAYOUT_ICON_TOP_LABEL_BOTTOM = 3;

    /// <summary>
    ///     A layout option
    /// </summary>
    public const int LAYOUT_LABEL_LEFT_ICON_RIGHT = 6;

    /// <summary>
    ///     A layout option
    /// </summary>
    public const int LAYOUT_LABEL_ONLY = 1;

    /// <summary>
    ///     A layout option
    /// </summary>
    public const int LAYOUT_LABEL_OVER_ICON = 7;

    /// <summary>
    ///     A layout option
    /// </summary>
    public const int LAYOUT_LABEL_TOP_ICON_BOTTOM = 4;

    /// <summary>
    ///     An icon scaling option
    /// </summary>
    public const int SCALE_ICON_ALWAYS = 1;

    /// <summary>
    ///     An icon scaling option
    /// </summary>
    public const int SCALE_ICON_IS_TOO_BIG = 3;

    /// <summary>
    ///     An icon scaling option
    /// </summary>
    public const int SCALE_ICON_IS_TOO_SMALL = 4;

    /// <summary>
    ///     An icon scaling option
    /// </summary>
    public const int SCALE_ICON_NEVER = 2;

    /// <summary>
    ///     Holds value of property iconFitToBounds.
    /// </summary>
    private bool _iconFitToBounds;

    /// <summary>
    ///     Holds value of property iconHorizontalAdjustment.
    /// </summary>
    private float _iconHorizontalAdjustment = 0.5f;

    /// <summary>
    ///     Holds value of property iconReference.
    /// </summary>
    private PrIndirectReference _iconReference;

    /// <summary>
    ///     Holds value of property iconVerticalAdjustment.
    /// </summary>
    private float _iconVerticalAdjustment = 0.5f;

    /// <summary>
    ///     Holds value of property image.
    /// </summary>
    private Image _image;

    /// <summary>
    ///     Holds value of property layout.
    /// </summary>
    private int _layout = LAYOUT_LABEL_ONLY;

    /// <summary>
    ///     Holds value of property proportionalIcon.
    /// </summary>
    private bool _proportionalIcon = true;

    /// <summary>
    ///     Holds value of property scaleIcon.
    /// </summary>
    private int _scaleIcon = SCALE_ICON_ALWAYS;

    /// <summary>
    ///     Holds value of property template.
    /// </summary>
    private PdfTemplate _template;

    private PdfTemplate _tp;

    /// <summary>
    ///     Creates a new instance of PushbuttonField
    ///     will be included in the field allowing it to be used as a kid field.
    /// </summary>
    /// <param name="writer">the document  PdfWriter </param>
    /// <param name="box">the field location and dimensions</param>
    /// <param name="fieldName">the field name. If  null  only the widget keys</param>
    public PushbuttonField(PdfWriter writer, Rectangle box, string fieldName) : base(writer, box, fieldName)
    {
    }

    /// <summary>
    ///     Gets the pushbutton field.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>the pushbutton field</returns>
    public PdfFormField Field
    {
        get
        {
            var field = PdfFormField.CreatePushButton(writer);
            field.SetWidget(box, PdfAnnotation.HighlightInvert);
            if (fieldName != null)
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
            var tpa = GetAppearance();
            field.SetAppearance(PdfAnnotation.AppearanceNormal, tpa);
            var da = (PdfAppearance)tpa.Duplicate;
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

            if (_tp != null)
            {
                field.MkNormalIcon = _tp;
            }

            field.MkTextPosition = _layout - 1;
            var scale = PdfName.A;
            if (_scaleIcon == SCALE_ICON_IS_TOO_BIG)
            {
                scale = PdfName.B;
            }
            else if (_scaleIcon == SCALE_ICON_IS_TOO_SMALL)
            {
                scale = PdfName.S;
            }
            else if (_scaleIcon == SCALE_ICON_NEVER)
            {
                scale = PdfName.N;
            }

            field.SetMkIconFit(scale, _proportionalIcon ? PdfName.P : PdfName.A, _iconHorizontalAdjustment,
                               _iconVerticalAdjustment, _iconFitToBounds);
            return field;
        }
    }

    /// <summary>
    ///     If  true  the icon will be scaled to fit fully within the bounds of the annotation,
    ///     if  false  the border width will be taken into account. The default
    ///     is  false .
    ///     if  false  the border width will be taken into account
    /// </summary>
    public bool IconFitToBounds
    {
        get => _iconFitToBounds;
        set => _iconFitToBounds = value;
    }

    /// <summary>
    ///     A number between 0 and 1 indicating the fraction of leftover space to allocate at the left of the icon.
    ///     A value of 0 positions the icon at the left of the annotation rectangle.
    ///     A value of 0.5 centers it within the rectangle. The default is 0.5.
    /// </summary>
    public float IconHorizontalAdjustment
    {
        get => _iconHorizontalAdjustment;
        set
        {
            _iconHorizontalAdjustment = value;
            if (_iconHorizontalAdjustment < 0)
            {
                _iconHorizontalAdjustment = 0;
            }
            else if (_iconHorizontalAdjustment > 1)
            {
                _iconHorizontalAdjustment = 1;
            }
        }
    }

    /// <summary>
    ///     Sets the reference to an existing icon.
    /// </summary>
    public PrIndirectReference IconReference
    {
        get => _iconReference;
        set => _iconReference = value;
    }

    /// <summary>
    ///     A number between 0 and 1 indicating the fraction of leftover space to allocate at the bottom of the icon.
    ///     A value of 0 positions the icon at the bottom of the annotation rectangle.
    ///     A value of 0.5 centers it within the rectangle. The default is 0.5.
    /// </summary>
    public float IconVerticalAdjustment
    {
        get => _iconVerticalAdjustment;
        set
        {
            _iconVerticalAdjustment = value;
            if (_iconVerticalAdjustment < 0)
            {
                _iconVerticalAdjustment = 0;
            }
            else if (_iconVerticalAdjustment > 1)
            {
                _iconVerticalAdjustment = 1;
            }
        }
    }

    /// <summary>
    ///     Sets the icon as an image.
    /// </summary>
    public Image Image
    {
        get => _image;
        set
        {
            _image = value;
            _template = null;
        }
    }

    /// <summary>
    ///     Sets the icon and label layout. Possible values are  LAYOUT_LABEL_ONLY ,
    ///     LAYOUT_ICON_ONLY ,  LAYOUT_ICON_TOP_LABEL_BOTTOM ,
    ///     LAYOUT_LABEL_TOP_ICON_BOTTOM ,  LAYOUT_ICON_LEFT_LABEL_RIGHT ,
    ///     LAYOUT_LABEL_LEFT_ICON_RIGHT  and  LAYOUT_LABEL_OVER_ICON .
    ///     The default is  LAYOUT_LABEL_ONLY .
    /// </summary>
    public int Layout
    {
        set
        {
            if (value < LAYOUT_LABEL_ONLY || value > LAYOUT_LABEL_OVER_ICON)
            {
                throw new ArgumentException("Layout out of bounds.");
            }

            _layout = value;
        }
        get => _layout;
    }

    /// <summary>
    ///     Sets the way the icon is scaled. If  true  the icon is scaled proportionally,
    ///     if  false  the scaling is done anamorphicaly.
    /// </summary>
    public bool ProportionalIcon
    {
        get => _proportionalIcon;
        set => _proportionalIcon = value;
    }

    /// <summary>
    ///     Sets the way the icon will be scaled. Possible values are
    ///     SCALE_ICON_ALWAYS ,  SCALE_ICON_NEVER ,
    ///     SCALE_ICON_IS_TOO_BIG  and  SCALE_ICON_IS_TOO_SMALL .
    ///     The default is  SCALE_ICON_ALWAYS .
    /// </summary>
    public int ScaleIcon
    {
        set
        {
            if (value < SCALE_ICON_ALWAYS || value > SCALE_ICON_IS_TOO_SMALL)
            {
                _scaleIcon = SCALE_ICON_ALWAYS;
            }
            else
            {
                _scaleIcon = value;
            }
        }
        get => _scaleIcon;
    }

    /// <summary>
    ///     Sets the icon as a template.
    /// </summary>
    public PdfTemplate Template
    {
        set
        {
            _template = value;
            _image = null;
        }
        get => _template;
    }

    /// <summary>
    ///     Gets the button appearance.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>the button appearance</returns>
    public PdfAppearance GetAppearance()
    {
        var app = GetBorderAppearance();
        var localBox = new Rectangle(app.BoundingBox);
        if (string.IsNullOrEmpty(text) && (_layout == LAYOUT_LABEL_ONLY ||
                                           (_image == null && _template == null && _iconReference == null)))
        {
            return app;
        }

        if (_layout == LAYOUT_ICON_ONLY && _image == null && _template == null && _iconReference == null)
        {
            return app;
        }

        var ufont = RealFont;
        var borderExtra = borderStyle == PdfBorderDictionary.STYLE_BEVELED ||
                          borderStyle == PdfBorderDictionary.STYLE_INSET;
        var h = localBox.Height - borderWidth * 2;
        var bw2 = borderWidth;
        if (borderExtra)
        {
            h -= borderWidth * 2;
            bw2 *= 2;
        }

        var offsetX = borderExtra ? 2 * borderWidth : borderWidth;
        offsetX = Math.Max(offsetX, 1);
        var offX = Math.Min(bw2, offsetX);
        _tp = null;
        var textX = float.NaN;
        float textY = 0;
        var fsize = fontSize;
        var wt = localBox.Width - 2 * offX - 2;
        var ht = localBox.Height - 2 * offX;
        var adj = _iconFitToBounds ? 0 : offX + 1;
        var nlayout = _layout;
        if (_image == null && _template == null && _iconReference == null)
        {
            nlayout = LAYOUT_LABEL_ONLY;
        }

        Rectangle iconBox = null;
        while (true)
        {
            switch (nlayout)
            {
                case LAYOUT_LABEL_ONLY:
                case LAYOUT_LABEL_OVER_ICON:
                    if (!string.IsNullOrEmpty(text) && wt > 0 && ht > 0)
                    {
                        fsize = calculateFontSize(wt, ht);
                        textX = (localBox.Width - ufont.GetWidthPoint(text, fsize)) / 2;
                        textY = (localBox.Height - ufont.GetFontDescriptor(BaseFont.ASCENT, fsize)) / 2;
                    }

                    goto case LAYOUT_ICON_ONLY;
                case LAYOUT_ICON_ONLY:
                    if (nlayout == LAYOUT_LABEL_OVER_ICON || nlayout == LAYOUT_ICON_ONLY)
                    {
                        iconBox = new Rectangle(localBox.Left + adj, localBox.Bottom + adj, localBox.Right - adj,
                                                localBox.Top - adj);
                    }

                    break;
                case LAYOUT_ICON_TOP_LABEL_BOTTOM:
                    if (string.IsNullOrEmpty(text) || wt <= 0 || ht <= 0)
                    {
                        nlayout = LAYOUT_ICON_ONLY;
                        continue;
                    }

                    var nht = localBox.Height * 0.35f - offX;
                    if (nht > 0)
                    {
                        fsize = calculateFontSize(wt, nht);
                    }
                    else
                    {
                        fsize = 4;
                    }

                    textX = (localBox.Width - ufont.GetWidthPoint(text, fsize)) / 2;
                    textY = offX - ufont.GetFontDescriptor(BaseFont.DESCENT, fsize);
                    iconBox = new Rectangle(localBox.Left + adj, textY + fsize, localBox.Right - adj,
                                            localBox.Top - adj);
                    break;
                case LAYOUT_LABEL_TOP_ICON_BOTTOM:
                    if (string.IsNullOrEmpty(text) || wt <= 0 || ht <= 0)
                    {
                        nlayout = LAYOUT_ICON_ONLY;
                        continue;
                    }

                    nht = localBox.Height * 0.35f - offX;
                    if (nht > 0)
                    {
                        fsize = calculateFontSize(wt, nht);
                    }
                    else
                    {
                        fsize = 4;
                    }

                    textX = (localBox.Width - ufont.GetWidthPoint(text, fsize)) / 2;
                    textY = localBox.Height - offX - fsize;
                    if (textY < offX)
                    {
                        textY = offX;
                    }

                    iconBox = new Rectangle(localBox.Left + adj, localBox.Bottom + adj, localBox.Right - adj,
                                            textY + ufont.GetFontDescriptor(BaseFont.DESCENT, fsize));
                    break;
                case LAYOUT_LABEL_LEFT_ICON_RIGHT:
                    if (string.IsNullOrEmpty(text) || wt <= 0 || ht <= 0)
                    {
                        nlayout = LAYOUT_ICON_ONLY;
                        continue;
                    }

                    var nw = localBox.Width * 0.35f - offX;
                    if (nw > 0)
                    {
                        fsize = calculateFontSize(wt, nw);
                    }
                    else
                    {
                        fsize = 4;
                    }

                    if (ufont.GetWidthPoint(text, fsize) >= wt)
                    {
                        nlayout = LAYOUT_LABEL_ONLY;
                        fsize = fontSize;
                        continue;
                    }

                    textX = offX + 1;
                    textY = (localBox.Height - ufont.GetFontDescriptor(BaseFont.ASCENT, fsize)) / 2;
                    iconBox = new Rectangle(textX + ufont.GetWidthPoint(text, fsize), localBox.Bottom + adj,
                                            localBox.Right - adj, localBox.Top - adj);
                    break;
                case LAYOUT_ICON_LEFT_LABEL_RIGHT:
                    if (string.IsNullOrEmpty(text) || wt <= 0 || ht <= 0)
                    {
                        nlayout = LAYOUT_ICON_ONLY;
                        continue;
                    }

                    nw = localBox.Width * 0.35f - offX;
                    if (nw > 0)
                    {
                        fsize = calculateFontSize(wt, nw);
                    }
                    else
                    {
                        fsize = 4;
                    }

                    if (ufont.GetWidthPoint(text, fsize) >= wt)
                    {
                        nlayout = LAYOUT_LABEL_ONLY;
                        fsize = fontSize;
                        continue;
                    }

                    textX = localBox.Width - ufont.GetWidthPoint(text, fsize) - offX - 1;
                    textY = (localBox.Height - ufont.GetFontDescriptor(BaseFont.ASCENT, fsize)) / 2;
                    iconBox = new Rectangle(localBox.Left + adj, localBox.Bottom + adj, textX - 1, localBox.Top - adj);
                    break;
            }

            break;
        }

        if (textY < localBox.Bottom + offX)
        {
            textY = localBox.Bottom + offX;
        }

        if (iconBox != null && (iconBox.Width <= 0 || iconBox.Height <= 0))
        {
            iconBox = null;
        }

        var haveIcon = false;
        float boundingBoxWidth = 0;
        float boundingBoxHeight = 0;
        PdfArray matrix = null;
        if (iconBox != null)
        {
            if (_image != null)
            {
                _tp = new PdfTemplate(writer);
                _tp.BoundingBox = new Rectangle(_image);
                writer.AddDirectTemplateSimple(_tp, PdfName.Frm);
                _tp.AddImage(_image, _image.Width, 0, 0, _image.Height, 0, 0);
                haveIcon = true;
                boundingBoxWidth = _tp.BoundingBox.Width;
                boundingBoxHeight = _tp.BoundingBox.Height;
            }
            else if (_template != null)
            {
                _tp = new PdfTemplate(writer);
                _tp.BoundingBox = new Rectangle(_template.Width, _template.Height);
                writer.AddDirectTemplateSimple(_tp, PdfName.Frm);
                _tp.AddTemplate(_template, _template.BoundingBox.Left, _template.BoundingBox.Bottom);
                haveIcon = true;
                boundingBoxWidth = _tp.BoundingBox.Width;
                boundingBoxHeight = _tp.BoundingBox.Height;
            }
            else if (_iconReference != null)
            {
                var dic = (PdfDictionary)PdfReader.GetPdfObject(_iconReference);
                if (dic != null)
                {
                    var r2 = PdfReader.GetNormalizedRectangle(dic.GetAsArray(PdfName.Bbox));
                    matrix = dic.GetAsArray(PdfName.Matrix);
                    haveIcon = true;
                    boundingBoxWidth = r2.Width;
                    boundingBoxHeight = r2.Height;
                }
            }
        }

        if (haveIcon)
        {
            var icx = iconBox.Width / boundingBoxWidth;
            var icy = iconBox.Height / boundingBoxHeight;
            if (_proportionalIcon)
            {
                switch (_scaleIcon)
                {
                    case SCALE_ICON_IS_TOO_BIG:
                        icx = Math.Min(icx, icy);
                        icx = Math.Min(icx, 1);
                        break;
                    case SCALE_ICON_IS_TOO_SMALL:
                        icx = Math.Min(icx, icy);
                        icx = Math.Max(icx, 1);
                        break;
                    case SCALE_ICON_NEVER:
                        icx = 1;
                        break;
                    default:
                        icx = Math.Min(icx, icy);
                        break;
                }

                icy = icx;
            }
            else
            {
                switch (_scaleIcon)
                {
                    case SCALE_ICON_IS_TOO_BIG:
                        icx = Math.Min(icx, 1);
                        icy = Math.Min(icy, 1);
                        break;
                    case SCALE_ICON_IS_TOO_SMALL:
                        icx = Math.Max(icx, 1);
                        icy = Math.Max(icy, 1);
                        break;
                    case SCALE_ICON_NEVER:
                        icx = icy = 1;
                        break;
                }
            }

            var xpos = iconBox.Left + (iconBox.Width - boundingBoxWidth * icx) * _iconHorizontalAdjustment;
            var ypos = iconBox.Bottom + (iconBox.Height - boundingBoxHeight * icy) * _iconVerticalAdjustment;
            app.SaveState();
            app.Rectangle(iconBox.Left, iconBox.Bottom, iconBox.Width, iconBox.Height);
            app.Clip();
            app.NewPath();
            if (_tp != null)
            {
                app.AddTemplate(_tp, icx, 0, 0, icy, xpos, ypos);
            }
            else
            {
                float cox = 0;
                float coy = 0;
                if (matrix != null && matrix.Size == 6)
                {
                    var nm = matrix.GetAsNumber(4);
                    if (nm != null)
                    {
                        cox = nm.FloatValue;
                    }

                    nm = matrix.GetAsNumber(5);
                    if (nm != null)
                    {
                        coy = nm.FloatValue;
                    }
                }

                app.AddTemplateReference(_iconReference, PdfName.Frm, icx, 0, 0, icy, xpos - cox * icx,
                                         ypos - coy * icy);
            }

            app.RestoreState();
        }

        if (!float.IsNaN(textX))
        {
            app.SaveState();
            app.Rectangle(offX, offX, localBox.Width - 2 * offX, localBox.Height - 2 * offX);
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
            app.SetTextMatrix(textX, textY);
            app.ShowText(text);
            app.EndText();
            app.RestoreState();
        }

        return app;
    }

    private float calculateFontSize(float w, float h)
    {
        var ufont = RealFont;
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
                fsize = w / bw;
            }

            var nfsize = h / (1 - ufont.GetFontDescriptor(BaseFont.DESCENT, 1));
            fsize = Math.Min(fsize, nfsize);
            if (fsize < 4)
            {
                fsize = 4;
            }
        }

        return fsize;
    }
}