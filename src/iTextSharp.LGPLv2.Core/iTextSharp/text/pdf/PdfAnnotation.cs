using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfAnnotation  is a note that is associated with a page.
///     @see     PdfDictionary
/// </summary>
public class PdfAnnotation : PdfDictionary
{
    public const int FLAGS_HIDDEN = 2;
    public const int FLAGS_INVISIBLE = 1;
    public const int FLAGS_LOCKED = 128;
    public const int FLAGS_NOROTATE = 16;
    public const int FLAGS_NOVIEW = 32;
    public const int FLAGS_NOZOOM = 8;
    public const int FLAGS_PRINT = 4;
    public const int FLAGS_READONLY = 64;
    public const int FLAGS_TOGGLENOVIEW = 256;
    public const int MARKUP_HIGHLIGHT = 0;

    /// <summary>
    ///     attributevalue
    /// </summary>
    public const int MARKUP_SQUIGGLY = 3;

    public const int MARKUP_STRIKEOUT = 2;
    public const int MARKUP_UNDERLINE = 1;
    public static readonly PdfName AaBlur = PdfName.Bl;
    public static readonly PdfName AaDown = PdfName.D;
    public static readonly PdfName AaEnter = PdfName.E;
    public static readonly PdfName AaExit = PdfName.X;
    public static readonly PdfName AaFocus = PdfName.Fo;
    public static readonly PdfName AaJsChange = PdfName.V;
    public static readonly PdfName AaJsFormat = PdfName.F;
    public static readonly PdfName AaJsKey = PdfName.K;
    public static readonly PdfName AaJsOtherChange = PdfName.C;
    public static readonly PdfName AaUp = PdfName.U;
    public static readonly PdfName AppearanceDown = PdfName.D;
    public static readonly PdfName AppearanceNormal = PdfName.N;
    public static readonly PdfName AppearanceRollover = PdfName.R;
    public static readonly PdfName HighlightInvert = PdfName.I;
    public static readonly PdfName HighlightNone = PdfName.N;
    public static readonly PdfName HighlightOutline = PdfName.O;
    public static readonly PdfName HighlightPush = PdfName.P;
    public static readonly PdfName HighlightToggle = PdfName.T;

    /// <summary>
    ///     Holds value of property placeInPage.
    /// </summary>
    private int _placeInPage = -1;

    protected internal bool Annotation = true;
    protected internal bool Form;
    protected internal PdfIndirectReference Reference;
    protected internal INullValueDictionary<PdfTemplate, object> templates;

    /// <summary>
    ///     Holds value of property used.
    /// </summary>
    protected internal bool Used;

    protected internal PdfWriter Writer;

    /// <summary>
    ///     constructors
    /// </summary>
    public PdfAnnotation(PdfWriter writer, Rectangle rect)
    {
        Writer = writer;
        if (rect != null)
        {
            Put(PdfName.Rect, new PdfRectangle(rect));
        }
    }

    /// <summary>
    ///     Constructs a new  PdfAnnotation  of subtype text.
    /// </summary>
    public PdfAnnotation(PdfWriter writer, float llx, float lly, float urx, float ury, PdfString title,
                         PdfString content)
    {
        Writer = writer;
        Put(PdfName.Subtype, PdfName.Text);
        Put(PdfName.T, title);
        Put(PdfName.Rect, new PdfRectangle(llx, lly, urx, ury));
        Put(PdfName.Contents, content);
    }

    /// <summary>
    ///     Constructs a new  PdfAnnotation  of subtype link (Action).
    /// </summary>
    public PdfAnnotation(PdfWriter writer, float llx, float lly, float urx, float ury, PdfAction action)
    {
        Writer = writer;
        Put(PdfName.Subtype, PdfName.Link);
        Put(PdfName.Rect, new PdfRectangle(llx, lly, urx, ury));
        Put(PdfName.A, action);
        Put(PdfName.Border, new PdfBorderArray(0, 0, 0));
        Put(PdfName.C, new PdfColor(0x00, 0x00, 0xFF));
    }

    public PdfAction Action
    {
        set => Put(PdfName.A, value);
    }

    public string AppearanceState
    {
        set
        {
            if (value == null)
            {
                Remove(PdfName.As);
                return;
            }

            Put(PdfName.As, new PdfName(value));
        }
    }

    public PdfBorderArray Border
    {
        set => Put(PdfName.Border, value);
    }

    public PdfBorderDictionary BorderStyle
    {
        set => Put(PdfName.Bs, value);
    }

    public BaseColor Color
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Put(PdfName.C, new PdfColor(value));
        }
    }

    public PdfContentByte DefaultAppearanceString
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var b = value.InternalBuffer.ToByteArray();
            var len = b.Length;
            for (var k = 0; k < len; ++k)
            {
                if (b[k] == '\n')
                {
                    b[k] = 32;
                }
            }

            Put(PdfName.Da, new PdfString(b));
        }
    }

    public int Flags
    {
        set
        {
            if (value == 0)
            {
                Remove(PdfName.F);
            }
            else
            {
                Put(PdfName.F, new PdfNumber(value));
            }
        }
    }

    public PdfIndirectReference IndirectReference
    {
        get
        {
            if (Reference == null)
            {
                Reference = Writer.PdfIndirectReference;
            }

            return Reference;
        }
    }

    /// <summary>
    ///     Sets the layer this annotation belongs to.
    /// </summary>
    public IPdfOcg Layer
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Put(PdfName.Oc, value.Ref);
        }
    }

    public string MkAlternateCaption
    {
        set => Mk.Put(PdfName.Ac, new PdfString(value, TEXT_UNICODE));
    }

    public PdfTemplate MkAlternateIcon
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Mk.Put(PdfName.Ix, value.IndirectReference);
        }
    }

    public BaseColor MkBackgroundColor
    {
        set
        {
            if (value == null)
            {
                Mk.Remove(PdfName.Bg);
            }
            else
            {
                Mk.Put(PdfName.Bg, GetMkColor(value));
            }
        }
    }

    public BaseColor MkBorderColor
    {
        set
        {
            if (value == null)
            {
                Mk.Remove(PdfName.Bc);
            }
            else
            {
                Mk.Put(PdfName.Bc, GetMkColor(value));
            }
        }
    }

    public string MkNormalCaption
    {
        set => Mk.Put(PdfName.CA, new PdfString(value, TEXT_UNICODE));
    }

    public PdfTemplate MkNormalIcon
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Mk.Put(PdfName.I, value.IndirectReference);
        }
    }

    public string MkRolloverCaption
    {
        set => Mk.Put(PdfName.Rc, new PdfString(value, TEXT_UNICODE));
    }

    public PdfTemplate MkRolloverIcon
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Mk.Put(PdfName.Ri, value.IndirectReference);
        }
    }

    public int MkRotation
    {
        set => Mk.Put(PdfName.R, new PdfNumber(value));
    }

    public int MkTextPosition
    {
        set => Mk.Put(PdfName.Tp, new PdfNumber(value));
    }

    /// <summary>
    ///     Sets the name of the annotation.
    ///     With this name the annotation can be identified among
    ///     all the annotations on a page (it has to be unique).
    /// </summary>
    public string Name
    {
        set => Put(PdfName.Nm, new PdfString(value));
    }

    public new int Page
    {
        set => Put(PdfName.P, Writer.GetPageReference(value));
    }

    /// <summary>
    ///     Getter for property placeInPage.
    /// </summary>
    /// <returns>Value of property placeInPage.</returns>
    public int PlaceInPage
    {
        get => _placeInPage;

        set => _placeInPage = value;
    }

    public PdfAnnotation Popup
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Put(PdfName.Popup, value.IndirectReference);
            value.Put(PdfName.Parent, IndirectReference);
        }
    }

    public int Rotate
    {
        set => Put(PdfName.Rotate, new PdfNumber(value));
    }

    public INullValueDictionary<PdfTemplate, object> Templates => templates;

    public string Title
    {
        set
        {
            if (value == null)
            {
                Remove(PdfName.T);
                return;
            }

            Put(PdfName.T, new PdfString(value, TEXT_UNICODE));
        }
    }

    internal PdfDictionary Mk
    {
        get
        {
            var mk = (PdfDictionary)Get(PdfName.Mk);
            if (mk == null)
            {
                mk = new PdfDictionary();
                Put(PdfName.Mk, mk);
            }

            return mk;
        }
    }

    /// <summary>
    ///     Creates a file attachment annotation.
    ///     the file will be read from the disk
    ///     fileStore  is not  null
    ///     @throws IOException on error
    /// </summary>
    /// <param name="writer">the  PdfWriter </param>
    /// <param name="rect">the dimensions in the page of the annotation</param>
    /// <param name="contents">the file description</param>
    /// <param name="fileStore">an array with the file. If it's  null </param>
    /// <param name="file">the path to the file. It will only be used if</param>
    /// <param name="fileDisplay">the actual file name stored in the pdf</param>
    /// <returns>the annotation</returns>
    public static PdfAnnotation CreateFileAttachment(PdfWriter writer, Rectangle rect, string contents,
                                                     byte[] fileStore, string file, string fileDisplay) =>
        CreateFileAttachment(writer, rect, contents,
                             PdfFileSpecification.FileEmbedded(writer, file, fileDisplay, fileStore));

    /// <summary>
    ///     Creates a file attachment annotation
    ///     @throws IOException
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="rect"></param>
    /// <param name="contents"></param>
    /// <param name="fs"></param>
    /// <returns>the annotation</returns>
    public static PdfAnnotation CreateFileAttachment(PdfWriter writer, Rectangle rect, string contents,
                                                     PdfFileSpecification fs)
    {
        if (fs == null)
        {
            throw new ArgumentNullException(nameof(fs));
        }

        var annot = new PdfAnnotation(writer, rect);
        annot.Put(PdfName.Subtype, PdfName.Fileattachment);
        if (contents != null)
        {
            annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        }

        annot.Put(PdfName.Fs, fs.Reference);
        return annot;
    }

    public static PdfAnnotation CreateFreeText(PdfWriter writer, Rectangle rect, string contents,
                                               PdfContentByte defaultAppearance)
    {
        var annot = new PdfAnnotation(writer, rect);
        annot.Put(PdfName.Subtype, PdfName.Freetext);
        annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        annot.DefaultAppearanceString = defaultAppearance;
        return annot;
    }

    public static PdfAnnotation CreateInk(PdfWriter writer, Rectangle rect, string contents, float[][] inkList)
    {
        if (inkList == null)
        {
            throw new ArgumentNullException(nameof(inkList));
        }

        var annot = new PdfAnnotation(writer, rect);
        annot.Put(PdfName.Subtype, PdfName.Ink);
        annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        var outer = new PdfArray();
        for (var k = 0; k < inkList.Length; ++k)
        {
            var inner = new PdfArray();
            var deep = inkList[k];
            for (var j = 0; j < deep.Length; ++j)
            {
                inner.Add(new PdfNumber(deep[j]));
            }

            outer.Add(inner);
        }

        annot.Put(PdfName.Inklist, outer);
        return annot;
    }

    public static PdfAnnotation CreateLine(PdfWriter writer, Rectangle rect, string contents, float x1, float y1,
                                           float x2, float y2)
    {
        var annot = new PdfAnnotation(writer, rect);
        annot.Put(PdfName.Subtype, PdfName.Line);
        annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        var array = new PdfArray(new PdfNumber(x1));
        array.Add(new PdfNumber(y1));
        array.Add(new PdfNumber(x2));
        array.Add(new PdfNumber(y2));
        annot.Put(PdfName.L, array);
        return annot;
    }

    public static PdfAnnotation CreateLink(PdfWriter writer, Rectangle rect, PdfName highlight, PdfAction action)
    {
        var annot = CreateLink(writer, rect, highlight);
        annot.PutEx(PdfName.A, action);
        return annot;
    }

    public static PdfAnnotation CreateLink(PdfWriter writer, Rectangle rect, PdfName highlight, string namedDestination)
    {
        var annot = CreateLink(writer, rect, highlight);
        annot.Put(PdfName.Dest, new PdfString(namedDestination));
        return annot;
    }

    public static PdfAnnotation CreateLink(PdfWriter writer, Rectangle rect, PdfName highlight, int page,
                                           PdfDestination dest)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        var annot = CreateLink(writer, rect, highlight);
        var piref = writer.GetPageReference(page);
        dest.AddPage(piref);
        annot.Put(PdfName.Dest, dest);
        return annot;
    }

    public static PdfAnnotation CreateMarkup(PdfWriter writer, Rectangle rect, string contents, int type,
                                             float[] quadPoints)
    {
        if (quadPoints == null)
        {
            throw new ArgumentNullException(nameof(quadPoints));
        }

        var annot = new PdfAnnotation(writer, rect);
        var name = PdfName.Highlight;
        switch (type)
        {
            case MARKUP_UNDERLINE:
                name = PdfName.Underline;
                break;
            case MARKUP_STRIKEOUT:
                name = PdfName.Strikeout;
                break;
            case MARKUP_SQUIGGLY:
                name = PdfName.Squiggly;
                break;
        }

        annot.Put(PdfName.Subtype, name);
        annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        var array = new PdfArray();
        for (var k = 0; k < quadPoints.Length; ++k)
        {
            array.Add(new PdfNumber(quadPoints[k]));
        }

        annot.Put(PdfName.Quadpoints, array);
        return annot;
    }

    public static PdfAnnotation CreatePopup(PdfWriter writer, Rectangle rect, string contents, bool open)
    {
        var annot = new PdfAnnotation(writer, rect);
        annot.Put(PdfName.Subtype, PdfName.Popup);
        if (contents != null)
        {
            annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        }

        if (open)
        {
            annot.Put(PdfName.Open, PdfBoolean.Pdftrue);
        }

        return annot;
    }

    /// <summary>
    ///     Creates a screen PdfAnnotation
    ///     @throws IOException
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="rect"></param>
    /// <param name="clipTitle"></param>
    /// <param name="fs"></param>
    /// <param name="mimeType"></param>
    /// <param name="playOnDisplay"></param>
    /// <returns>a screen PdfAnnotation</returns>
    public static PdfAnnotation CreateScreen(PdfWriter writer, Rectangle rect, string clipTitle,
                                             PdfFileSpecification fs,
                                             string mimeType, bool playOnDisplay)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        var ann = new PdfAnnotation(writer, rect);
        ann.Put(PdfName.Subtype, PdfName.Screen);
        ann.Put(PdfName.F, new PdfNumber(FLAGS_PRINT));
        ann.Put(PdfName.TYPE, PdfName.Annot);
        ann.SetPage();
        var refi = ann.IndirectReference;
        var action = PdfAction.Rendition(clipTitle, fs, mimeType, refi);
        var actionRef = writer.AddToBody(action).IndirectReference;
        // for play on display add trigger event
        if (playOnDisplay)
        {
            var aa = new PdfDictionary();
            aa.Put(new PdfName("PV"), actionRef);
            ann.Put(PdfName.Aa, aa);
        }

        ann.Put(PdfName.A, actionRef);
        return ann;
    }

    public static PdfAnnotation CreateSquareCircle(PdfWriter writer, Rectangle rect, string contents, bool square)
    {
        var annot = new PdfAnnotation(writer, rect);
        if (square)
        {
            annot.Put(PdfName.Subtype, PdfName.Square);
        }
        else
        {
            annot.Put(PdfName.Subtype, PdfName.Circle);
        }

        annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        return annot;
    }

    public static PdfAnnotation CreateStamp(PdfWriter writer, Rectangle rect, string contents, string name)
    {
        var annot = new PdfAnnotation(writer, rect);
        annot.Put(PdfName.Subtype, PdfName.Stamp);
        annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        annot.Put(PdfName.Name, new PdfName(name));
        return annot;
    }

    public static PdfAnnotation CreateText(PdfWriter writer, Rectangle rect, string title, string contents, bool open,
                                           string icon)
    {
        var annot = new PdfAnnotation(writer, rect);
        annot.Put(PdfName.Subtype, PdfName.Text);
        if (title != null)
        {
            annot.Put(PdfName.T, new PdfString(title, TEXT_UNICODE));
        }

        if (contents != null)
        {
            annot.Put(PdfName.Contents, new PdfString(contents, TEXT_UNICODE));
        }

        if (open)
        {
            annot.Put(PdfName.Open, PdfBoolean.Pdftrue);
        }

        if (icon != null)
        {
            annot.Put(PdfName.Name, new PdfName(icon));
        }

        return annot;
    }

    public static PdfArray GetMkColor(BaseColor color)
    {
        if (color == null)
        {
            throw new ArgumentNullException(nameof(color));
        }

        var array = new PdfArray();
        var type = ExtendedColor.GetType(color);
        switch (type)
        {
            case ExtendedColor.TYPE_GRAY:
            {
                array.Add(new PdfNumber(((GrayColor)color).Gray));
                break;
            }
            case ExtendedColor.TYPE_CMYK:
            {
                var cmyk = (CmykColor)color;
                array.Add(new PdfNumber(cmyk.Cyan));
                array.Add(new PdfNumber(cmyk.Magenta));
                array.Add(new PdfNumber(cmyk.Yellow));
                array.Add(new PdfNumber(cmyk.Black));
                break;
            }
            case ExtendedColor.TYPE_SEPARATION:
            case ExtendedColor.TYPE_PATTERN:
            case ExtendedColor.TYPE_SHADING:
                throw new
                    InvalidOperationException("Separations, patterns and shadings are not allowed in MK dictionary.");
            default:
                array.Add(new PdfNumber(color.R / 255f));
                array.Add(new PdfNumber(color.G / 255f));
                array.Add(new PdfNumber(color.B / 255f));
                break;
        }

        return array;
    }

    public static PdfAnnotation ShallowDuplicate(PdfAnnotation annot)
    {
        if (annot == null)
        {
            throw new ArgumentNullException(nameof(annot));
        }

        PdfAnnotation dup;
        if (annot.IsForm())
        {
            dup = new PdfFormField(annot.Writer);
            var dupField = (PdfFormField)dup;
            var srcField = (PdfFormField)annot;
            dupField.parent = srcField.parent;
            dupField.kids = srcField.kids;
        }
        else
        {
            dup = new PdfAnnotation(annot.Writer, null);
        }

        dup.Merge(annot);
        dup.Form = annot.Form;
        dup.Annotation = annot.Annotation;
        dup.templates = annot.templates;
        return dup;
    }

    /// <summary>
    ///     Getter for property annotation.
    /// </summary>
    /// <returns>Value of property annotation.</returns>
    public bool IsAnnotation() => Annotation;

    /// <summary>
    ///     Getter for property form.
    /// </summary>
    /// <returns>Value of property form.</returns>
    public bool IsForm() => Form;

    public void SetAdditionalActions(PdfName key, PdfAction action)
    {
        PdfDictionary dic;
        var obj = Get(PdfName.Aa);
        if (obj != null && obj.IsDictionary())
        {
            dic = (PdfDictionary)obj;
        }
        else
        {
            dic = new PdfDictionary();
        }

        dic.Put(key, action);
        Put(PdfName.Aa, dic);
    }

    public void SetAppearance(PdfName ap, PdfTemplate template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        var dic = (PdfDictionary)Get(PdfName.Ap);
        if (dic == null)
        {
            dic = new PdfDictionary();
        }

        dic.Put(ap, template.IndirectReference);
        Put(PdfName.Ap, dic);
        if (!Form)
        {
            return;
        }

        if (templates == null)
        {
            templates = new NullValueDictionary<PdfTemplate, object>();
        }

        templates[template] = null;
    }

    public void SetAppearance(PdfName ap, string state, PdfTemplate template)
    {
        if (template == null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        var dicAp = (PdfDictionary)Get(PdfName.Ap);
        if (dicAp == null)
        {
            dicAp = new PdfDictionary();
        }

        PdfDictionary dic;
        var obj = dicAp.Get(ap);
        if (obj != null && obj.IsDictionary())
        {
            dic = (PdfDictionary)obj;
        }
        else
        {
            dic = new PdfDictionary();
        }

        dic.Put(new PdfName(state), template.IndirectReference);
        dicAp.Put(ap, dic);
        Put(PdfName.Ap, dicAp);
        if (!Form)
        {
            return;
        }

        if (templates == null)
        {
            templates = new NullValueDictionary<PdfTemplate, object>();
        }

        templates[template] = null;
    }

    /// <summary>
    ///     Sets the annotation's highlighting mode. The values can be
    ///     HIGHLIGHT_NONE ,  HIGHLIGHT_INVERT ,
    ///     HIGHLIGHT_OUTLINE  and  HIGHLIGHT_PUSH ;
    /// </summary>
    /// <param name="highlight">the annotation's highlighting mode</param>
    public void SetHighlighting(PdfName highlight)
    {
        if (highlight == null)
        {
            throw new ArgumentNullException(nameof(highlight));
        }

        if (highlight.Equals(HighlightInvert))
        {
            Remove(PdfName.H);
        }
        else
        {
            Put(PdfName.H, highlight);
        }
    }

    public void SetMkIconFit(PdfName scale, PdfName scalingType, float leftoverLeft, float leftoverBottom,
                             bool fitInBounds)
    {
        if (scale == null)
        {
            throw new ArgumentNullException(nameof(scale));
        }

        if (scalingType == null)
        {
            throw new ArgumentNullException(nameof(scalingType));
        }

        var dic = new PdfDictionary();
        if (!scale.Equals(PdfName.A))
        {
            dic.Put(PdfName.Sw, scale);
        }

        if (!scalingType.Equals(PdfName.P))
        {
            dic.Put(PdfName.S, scalingType);
        }

        if (leftoverLeft.ApproxNotEqual(0.5f) || leftoverBottom.ApproxNotEqual(0.5f))
        {
            var array = new PdfArray(new PdfNumber(leftoverLeft));
            array.Add(new PdfNumber(leftoverBottom));
            dic.Put(PdfName.A, array);
        }

        if (fitInBounds)
        {
            dic.Put(PdfName.Fb, PdfBoolean.Pdftrue);
        }

        Mk.Put(PdfName.If, dic);
    }

    public void SetPage()
    {
        Put(PdfName.P, Writer.CurrentPage);
    }

    public virtual void SetUsed()
    {
        Used = true;
    }

    internal virtual bool IsUsed() => Used;

    protected static PdfAnnotation CreateLink(PdfWriter writer, Rectangle rect, PdfName highlight)
    {
        if (highlight == null)
        {
            throw new ArgumentNullException(nameof(highlight));
        }

        var annot = new PdfAnnotation(writer, rect);
        annot.Put(PdfName.Subtype, PdfName.Link);
        if (!highlight.Equals(HighlightInvert))
        {
            annot.Put(PdfName.H, highlight);
        }

        return annot;
    }

    /// <summary>
    ///     This class processes links from imported pages so that they may be active. The following example code reads a group
    ///     of files and places them all on the output PDF, four pages in a single page, keeping the links active.
    ///     String[] files = new String[] {&quot;input1.pdf&quot;, &quot;input2.pdf&quot;};
    ///     String outputFile = &quot;output.pdf&quot;;
    ///     int firstPage=1;
    ///     Document document = new Document();
    ///     PdfWriter writer = PdfWriter.GetInstance(document, new FileOutputStream(outputFile));
    ///     document.SetPageSize(PageSize.A4);
    ///     float W = PageSize.A4.GetWidth() / 2;
    ///     float H = PageSize.A4.GetHeight() / 2;
    ///     document.Open();
    ///     PdfContentByte cb = writer.GetDirectContent();
    ///     for (int i = 0; i &lt; files.length; i++) {
    ///     PdfReader currentReader = new PdfReader(files[i]);
    ///     currentReader.ConsolidateNamedDestinations();
    ///     for (int page = 1; page &lt;= currentReader.GetNumberOfPages(); page++) {
    ///     PdfImportedPage importedPage = writer.GetImportedPage(currentReader, page);
    ///     float a = 0.5f;
    ///     float e = (page % 2 == 0) ? W : 0;
    ///     float f = (page % 4 == 1 || page % 4 == 2) ? H : 0;
    ///     ArrayList links = currentReader.GetLinks(page);
    ///     cb.AddTemplate(importedPage, a, 0, 0, a, e, f);
    ///     for (int j = 0; j &lt; links.Size(); j++) {
    ///     PdfAnnotation.PdfImportedLink link = (PdfAnnotation.PdfImportedLink)links.Get(j);
    ///     if (link.IsInternal()) {
    ///     int dPage = link.GetDestinationPage();
    ///     int newDestPage = (dPage-1)/4 + firstPage;
    ///     float ee = (dPage % 2 == 0) ? W : 0;
    ///     float ff = (dPage % 4 == 1 || dPage % 4 == 2) ? H : 0;
    ///     link.SetDestinationPage(newDestPage);
    ///     link.TransformDestination(a, 0, 0, a, ee, ff);
    ///     }
    ///     link.TransformRect(a, 0, 0, a, e, f);
    ///     writer.AddAnnotation(link.CreateAnnotation(writer));
    ///     }
    ///     if (page % 4 == 0)
    ///     document.NewPage();
    ///     }
    ///     if (i &lt; files.length - 1)
    ///     document.NewPage();
    ///     firstPage += (currentReader.GetNumberOfPages()+3)/4;
    ///     }
    ///     document.Close();
    /// </summary>
    public class PdfImportedLink
    {
        private readonly PdfArray _destination;
        private readonly INullValueDictionary<PdfName, PdfObject> _parameters;
        private float _llx, _lly, _urx, _ury;
        private int _newPage;

        internal PdfImportedLink(PdfDictionary annotation)
        {
            _parameters = annotation.HashMap.Clone();
            try
            {
                _destination = (PdfArray)_parameters[PdfName.Dest];
                _parameters.Remove(PdfName.Dest);
            }
            catch (Exception)
            {
                throw new ArgumentException("You have to consolidate the named destinations of your reader.");
            }

            if (_destination != null)
            {
                _destination = new PdfArray(_destination);
            }

            var rc = (PdfArray)_parameters[PdfName.Rect];
            _parameters.Remove(PdfName.Rect);
            _llx = rc.GetAsNumber(0).FloatValue;
            _lly = rc.GetAsNumber(1).FloatValue;
            _urx = rc.GetAsNumber(2).FloatValue;
            _ury = rc.GetAsNumber(3).FloatValue;
        }

        public PdfAnnotation CreateAnnotation(PdfWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var annotation = new PdfAnnotation(writer, new Rectangle(_llx, _lly, _urx, _ury));
            if (_newPage != 0)
            {
                var refi = writer.GetPageReference(_newPage);
                _destination[0] = refi;
            }

            if (_destination != null)
            {
                annotation.Put(PdfName.Dest, _destination);
            }

            foreach (var key in _parameters.Keys)
            {
                annotation.HashMap[key] = _parameters[key];
            }

            return annotation;
        }

        public int GetDestinationPage()
        {
            if (!IsInternal())
            {
                return 0;
            }

            // here destination is something like
            // [132 0 R, /XYZ, 29.3898, 731.864502, null]
            var refi = _destination.GetAsIndirectObject(0);

            var pr = (PrIndirectReference)refi;
            var r = pr.Reader;
            for (var i = 1; i <= r.NumberOfPages; i++)
            {
                var pp = r.GetPageOrigRef(i);
                if (pp.Generation == pr.Generation && pp.Number == pr.Number)
                {
                    return i;
                }
            }

            throw new ArgumentException("Page not found.");
        }

        public bool IsInternal() => _destination != null;

        public void SetDestinationPage(int newPage)
        {
            if (!IsInternal())
            {
                throw new ArgumentException("Cannot change destination of external link");
            }

            _newPage = newPage;
        }

        /// <summary>
        ///     Returns a String representation of the link.
        ///     @since	2.1.6
        /// </summary>
        /// <returns>String representation of the imported link</returns>
        public override string ToString()
        {
            var buf = new StringBuilder("Imported link: location [");
            buf.Append(_llx);
            buf.Append(' ');
            buf.Append(_lly);
            buf.Append(' ');
            buf.Append(_urx);
            buf.Append(' ');
            buf.Append(_ury);
            buf.Append("] destination ");
            buf.Append(_destination);
            buf.Append(" parameters ");
            buf.Append(_parameters);
            return buf.ToString();
        }

        public void TransformDestination(float a, float b, float c, float d, float e, float f)
        {
            if (!IsInternal())
            {
                throw new ArgumentException("Cannot change destination of external link");
            }

            if (_destination.GetAsName(1).Equals(PdfName.Xyz))
            {
                var x = _destination.GetAsNumber(2).FloatValue;
                var y = _destination.GetAsNumber(3).FloatValue;
                var xx = x * a + y * c + e;
                var yy = x * b + y * d + f;
                _destination.ArrayList[2] = new PdfNumber(xx);
                _destination.ArrayList[3] = new PdfNumber(yy);
            }
        }

        public void TransformRect(float a, float b, float c, float d, float e, float f)
        {
            var x = _llx * a + _lly * c + e;
            var y = _llx * b + _lly * d + f;
            _llx = x;
            _lly = y;
            x = _urx * a + _ury * c + e;
            y = _urx * b + _ury * d + f;
            _urx = x;
            _ury = y;
        }
    }
}