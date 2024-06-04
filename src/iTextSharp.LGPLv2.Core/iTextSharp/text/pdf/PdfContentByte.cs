using System.Text;
using System.util;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf.intern;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfContentByte  is an object containing the user positioned
///     text and graphic contents of a page. It knows how to apply the proper
///     font encoding.
/// </summary>
public class PdfContentByte
{
    /// <summary>
    ///     The alignement is center
    /// </summary>
    public const int ALIGN_CENTER = Element.ALIGN_CENTER;

    /// <summary>
    ///     The alignement is left
    /// </summary>
    public const int ALIGN_LEFT = Element.ALIGN_LEFT;

    /// <summary>
    ///     The alignement is right
    /// </summary>
    public const int ALIGN_RIGHT = Element.ALIGN_RIGHT;

    /// <summary>
    ///     A possible line cap value
    /// </summary>
    public const int LINE_CAP_BUTT = 0;

    /// <summary>
    ///     A possible line cap value
    /// </summary>
    public const int LINE_CAP_PROJECTING_SQUARE = 2;

    /// <summary>
    ///     A possible line cap value
    /// </summary>
    public const int LINE_CAP_ROUND = 1;

    /// <summary>
    ///     A possible line join value
    /// </summary>
    public const int LINE_JOIN_BEVEL = 2;

    /// <summary>
    ///     A possible line join value
    /// </summary>
    public const int LINE_JOIN_MITER = 0;

    /// <summary>
    ///     A possible line join value
    /// </summary>
    public const int LINE_JOIN_ROUND = 1;

    /// <summary>
    ///     A possible text rendering value
    /// </summary>
    public const int TEXT_RENDER_MODE_CLIP = 7;

    /// <summary>
    ///     A possible text rendering value
    /// </summary>
    public const int TEXT_RENDER_MODE_FILL = 0;

    /// <summary>
    ///     A possible text rendering value
    /// </summary>
    public const int TEXT_RENDER_MODE_FILL_CLIP = 4;

    /// <summary>
    ///     A possible text rendering value
    /// </summary>
    public const int TEXT_RENDER_MODE_FILL_STROKE = 2;

    /// <summary>
    ///     A possible text rendering value
    /// </summary>
    public const int TEXT_RENDER_MODE_FILL_STROKE_CLIP = 6;

    /// <summary>
    ///     A possible text rendering value
    /// </summary>
    public const int TEXT_RENDER_MODE_INVISIBLE = 3;

    /// <summary>
    ///     A possible text rendering value
    /// </summary>
    public const int TEXT_RENDER_MODE_STROKE = 1;

    /// <summary>
    ///     A possible text rendering value
    /// </summary>
    public const int TEXT_RENDER_MODE_STROKE_CLIP = 5;

    private static readonly INullValueDictionary<PdfName, string> _abrev = new NullValueDictionary<PdfName, string>();

    private static readonly float[] _unitRect =
    {
        0, 0, 0, 1, 1, 0, 1, 1
    };

    private bool _inText;

    private int _mcDepth;

    /// <summary>
    ///     This is the actual content
    /// </summary>
    protected ByteBuffer Content = new();

    /// <summary>
    ///     The list were we save/restore the layer depth
    /// </summary>
    protected List<int> LayerDepth;

    /// <summary>
    ///     This is the PdfDocument
    /// </summary>
    protected PdfDocument Pdf;

    /// <summary>
    ///     The separator between commands.
    /// </summary>
    protected int Separator = '\n';

    /// <summary>
    ///     This is the GraphicState in use
    /// </summary>
    protected GraphicState State = new();

    /// <summary>
    ///     The list were we save/restore the state
    /// </summary>
    protected List<GraphicState> StateList = new();

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     This is the writer
    /// </summary>
    protected PdfWriter Writer;

    static PdfContentByte()
    {
        _abrev[PdfName.Bitspercomponent] = "/BPC ";
        _abrev[PdfName.Colorspace] = "/CS ";
        _abrev[PdfName.Decode] = "/D ";
        _abrev[PdfName.Decodeparms] = "/DP ";
        _abrev[PdfName.Filter] = "/F ";
        _abrev[PdfName.Height] = "/H ";
        _abrev[PdfName.Imagemask] = "/IM ";
        _abrev[PdfName.Intent] = "/Intent ";
        _abrev[PdfName.Interpolate] = "/I ";
        _abrev[PdfName.Width] = "/W ";
    }

    public PdfContentByte(PdfWriter wr)
    {
        if (wr != null)
        {
            Writer = wr;
            Pdf = Writer.PdfDocument;
        }
    }

    /// <summary>
    ///     Gets the current character spacing.
    /// </summary>
    /// <returns>the current character spacing</returns>
    public float CharacterSpacing => State.CharSpace;

    /// <summary>
    ///     Gets a duplicate of this  PdfContentByte . All
    ///     the members are copied by reference but the buffer stays different.
    /// </summary>
    /// <returns>a copy of this  PdfContentByte </returns>
    public virtual PdfContentByte Duplicate => new(Writer);

    /// <summary>
    ///     Gets the current character spacing.
    /// </summary>
    /// <returns>the current character spacing</returns>
    public float HorizontalScaling => State.Scale;

    /// <summary>
    ///     Gets the internal buffer.
    /// </summary>
    /// <returns>the internal buffer</returns>
    public ByteBuffer InternalBuffer => Content;

    /// <summary>
    ///     Gets the current text leading.
    /// </summary>
    /// <returns>the current text leading</returns>
    public float Leading => State.leading;

    /// <summary>
    ///     Gets the  PdfDocument  in use by this object.
    /// </summary>
    /// <returns>the  PdfDocument  in use by this object</returns>
    public PdfDocument PdfDocument => Pdf;

    /// <summary>
    ///     Gets the  PdfWriter  in use by this object.
    /// </summary>
    /// <returns>the  PdfWriter  in use by this object</returns>
    public PdfWriter PdfWriter => Writer;

    /// <summary>
    ///     Gets the root outline.
    /// </summary>
    /// <returns>the root outline</returns>
    public PdfOutline RootOutline
    {
        get
        {
            CheckWriter();

            return Pdf.RootOutline;
        }
    }

    /// <summary>
    ///     Gets the current word spacing.
    /// </summary>
    /// <returns>the current word spacing</returns>
    public float WordSpacing => State.WordSpace;

    /// <summary>
    ///     Gets the x position of the text line matrix.
    /// </summary>
    /// <returns>the x position of the text line matrix</returns>
    public float Xtlm => State.XTlm;

    /// <summary>
    ///     Gets the y position of the text line matrix.
    /// </summary>
    /// <returns>the y position of the text line matrix</returns>
    public float Ytlm => State.YTlm;

    internal virtual PageResources PageResources => Pdf.PageResources;

    /// <summary>
    ///     Gets the size of this content.
    /// </summary>
    /// <returns>the size of the content</returns>
    internal int Size => Content.Size;

    /// <summary>
    ///     Generates an array of bezier curves to draw an arc.
    ///     (x1, y1) and (x2, y2) are the corners of the enclosing rectangle.
    ///     Angles, measured in degrees, start with 0 to the right (the positive X
    ///     axis) and increase counter-clockwise.  The arc extends from startAng
    ///     to startAng+extent.  I.e. startAng=0 and extent=180 yields an openside-down
    ///     semi-circle.
    ///     The resulting coordinates are of the form float[]{x1,y1,x2,y2,x3,y3, x4,y4}
    ///     such that the curve goes from (x1, y1) to (x4, y4) with (x2, y2) and
    ///     (x3, y3) as their respective Bezier control points.
    ///     Note: this code was taken from ReportLab (www.reportlab.com), an excelent
    ///     PDF generator for Python.
    /// </summary>
    /// <param name="x1">a corner of the enclosing rectangle</param>
    /// <param name="y1">a corner of the enclosing rectangle</param>
    /// <param name="x2">a corner of the enclosing rectangle</param>
    /// <param name="y2">a corner of the enclosing rectangle</param>
    /// <param name="startAng">starting angle in degrees</param>
    /// <param name="extent">angle extent in degrees</param>
    /// <returns>a list of float[] with the bezier curves</returns>
    public static IList<double[]> BezierArc(float x1, float y1, float x2, float y2, float startAng, float extent)
    {
        float tmp;

        if (x1 > x2)
        {
            tmp = x1;
            x1 = x2;
            x2 = tmp;
        }

        if (y2 > y1)
        {
            tmp = y1;
            y1 = y2;
            y2 = tmp;
        }

        float fragAngle;
        int nfrag;

        if (Math.Abs(extent) <= 90f)
        {
            fragAngle = extent;
            nfrag = 1;
        }
        else
        {
            nfrag = (int)Math.Ceiling(Math.Abs(extent) / 90f);
            fragAngle = extent / nfrag;
        }

        var xCen = (x1 + x2) / 2f;
        var yCen = (y1 + y2) / 2f;
        var rx = (x2 - x1) / 2f;
        var ry = (y2 - y1) / 2f;
        var halfAng = (float)(fragAngle * Math.PI / 360.0);
        var kappa = (float)Math.Abs(4.0 / 3.0 * (1.0 - Math.Cos(halfAng)) / Math.Sin(halfAng));
        List<double[]> pointList = new();

        for (var i = 0; i < nfrag; ++i)
        {
            var theta0 = (float)((startAng + i * fragAngle) * Math.PI / 180.0);
            var theta1 = (float)((startAng + (i + 1) * fragAngle) * Math.PI / 180.0);
            var cos0 = (float)Math.Cos(theta0);
            var cos1 = (float)Math.Cos(theta1);
            var sin0 = (float)Math.Sin(theta0);
            var sin1 = (float)Math.Sin(theta1);

            if (fragAngle > 0f)
            {
                pointList.Add(new double[]
                {
                    xCen + rx * cos0, yCen - ry * sin0, xCen + rx * (cos0 - kappa * sin0),
                    yCen - ry * (sin0 + kappa * cos0), xCen + rx * (cos1 + kappa * sin1),
                    yCen - ry * (sin1 - kappa * cos1), xCen + rx * cos1, yCen - ry * sin1
                });
            }
            else
            {
                pointList.Add(new double[]
                {
                    xCen + rx * cos0, yCen - ry * sin0, xCen + rx * (cos0 + kappa * sin0),
                    yCen - ry * (sin0 - kappa * cos0), xCen + rx * (cos1 - kappa * sin1),
                    yCen - ry * (sin1 + kappa * cos1), xCen + rx * cos1, yCen - ry * sin1
                });
            }
        }

        return pointList;
    }

    /// <summary>
    ///     Constructs a kern array for a text in a certain font
    /// </summary>
    /// <param name="text">the text</param>
    /// <param name="font">the font</param>
    /// <returns>a PdfTextArray</returns>
    public static PdfTextArray GetKernArray(string text, BaseFont font)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        var pa = new PdfTextArray();
        var acc = new StringBuilder();
        var len = text.Length - 1;
        var c = text.ToCharArray();

        if (len >= 0)
        {
            acc.Append(c, 0, 1);
        }

        for (var k = 0; k < len; ++k)
        {
            var c2 = c[k + 1];
            var kern = font.GetKerning(c[k], c2);

            if (kern == 0)
            {
                acc.Append(c2);
            }
            else
            {
                pa.Add(acc.ToString());
                acc.Length = 0;
                acc.Append(c, k + 1, 1);
                pa.Add(-kern);
            }
        }

        pa.Add(acc.ToString());

        return pa;
    }

    public void Add(PdfContentByte other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        if (other.Writer != null && Writer != other.Writer)
        {
            throw new InvalidOperationException("Inconsistent writers. Are you mixing two documents?");
        }

        Content.Append(other.Content);
    }

    /// <summary>
    ///     Adds an  Image  to the page. The  Image  must have
    ///     absolute positioning.
    ///     @throws DocumentException if the  Image  does not have absolute positioning
    /// </summary>
    /// <param name="image">the  Image  object</param>
    public virtual void AddImage(Image image)
        => AddImage(image, false);

    /// <summary>
    ///     Adds an  Image  to the page. The  Image  must have
    ///     absolute positioning. The image can be placed inline.
    ///     @throws DocumentException if the  Image  does not have absolute positioning
    /// </summary>
    /// <param name="image">the  Image  object</param>
    /// <param name="inlineImage"> true  to place this image inline,  false  otherwise</param>
    public virtual void AddImage(Image image, bool inlineImage)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        if (!image.HasAbsolutePosition())
        {
            throw new DocumentException("The image must have absolute positioning.");
        }

        var matrix = image.Matrix;
        matrix[Image.CX] = image.AbsoluteX - matrix[Image.CX];
        matrix[Image.CY] = image.AbsoluteY - matrix[Image.CY];
        AddImage(image, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5], inlineImage);
    }

    /// <summary>
    ///     Adds an  Image  to the page. The positioning of the  Image
    ///     is done with the transformation matrix. To position an  image  at (x,y)
    ///     use AddImage(image, image_width, 0, 0, image_height, x, y).
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="image">the  Image  object</param>
    /// <param name="a">an element of the transformation matrix</param>
    /// <param name="b">an element of the transformation matrix</param>
    /// <param name="c">an element of the transformation matrix</param>
    /// <param name="d">an element of the transformation matrix</param>
    /// <param name="e">an element of the transformation matrix</param>
    /// <param name="f">an element of the transformation matrix</param>
    public virtual void AddImage(Image image, float a, float b, float c, float d, float e, float f)
        => AddImage(image, a, b, c, d, e, f, false);

    /// <summary>
    ///     Adds an  Image  to the page. The positioning of the  Image
    ///     is done with the transformation matrix. To position an  image  at (x,y)
    ///     use AddImage(image, image_width, 0, 0, image_height, x, y). The image can be placed inline.
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="image">the  Image  object</param>
    /// <param name="a">an element of the transformation matrix</param>
    /// <param name="b">an element of the transformation matrix</param>
    /// <param name="c">an element of the transformation matrix</param>
    /// <param name="d">an element of the transformation matrix</param>
    /// <param name="e">an element of the transformation matrix</param>
    /// <param name="f">an element of the transformation matrix</param>
    /// <param name="inlineImage"> true  to place this image inline,  false  otherwise</param>
    public virtual void AddImage(Image image, float a, float b, float c, float d, float e, float f, bool inlineImage)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        if (image.Layer != null)
        {
            BeginLayer(image.Layer);
        }

        if (image.IsImgTemplate())
        {
            Writer.AddDirectImageSimple(image);
            var template = image.TemplateData;
            var w = template.Width;
            var h = template.Height;
            AddTemplate(template, a / w, b / w, c / h, d / h, e, f);
        }
        else
        {
            Content.Append("q ");
            Content.Append(a).Append(' ');
            Content.Append(b).Append(' ');
            Content.Append(c).Append(' ');
            Content.Append(d).Append(' ');
            Content.Append(e).Append(' ');
            Content.Append(f).Append(" cm");

            if (inlineImage)
            {
                Content.Append("\nBI\n");
                var pimage = new PdfImage(image, "", null);

                if (image is ImgJbig2)
                {
                    var globals = ((ImgJbig2)image).GlobalBytes;

                    if (globals != null)
                    {
                        var decodeparms = new PdfDictionary();
                        decodeparms.Put(PdfName.Jbig2Globals, Writer.GetReferenceJbig2Globals(globals));
                        pimage.Put(PdfName.Decodeparms, decodeparms);
                    }
                }

                foreach (var key in pimage.Keys)
                {
                    var value = pimage.Get(key);
                    var s = _abrev[key];

                    if (s == null)
                    {
                        continue;
                    }

                    Content.Append(s);
                    var check = true;

                    if (key.Equals(PdfName.Colorspace) && value.IsArray())
                    {
                        var ar = (PdfArray)value;

                        if (ar.Size == 4 && PdfName.Indexed.Equals(ar.GetAsName(0)) && ar[1].IsName() &&
                            ar[2].IsNumber() && ar[3].IsString())
                        {
                            check = false;
                        }
                    }

                    if (check && key.Equals(PdfName.Colorspace) && !value.IsName())
                    {
                        var cs = Writer.GetColorspaceName();
                        var prs = PageResources;
                        prs.AddColor(cs, Writer.AddToBody(value).IndirectReference);
                        value = cs;
                    }

                    value.ToPdf(null, Content);
                    Content.Append('\n');
                }

                Content.Append("ID\n");
                pimage.WriteContent(Content);
                Content.Append("\nEI\nQ").Append_i(Separator);
            }
            else
            {
                PdfName name;
                var prs = PageResources;
                var maskImage = image.ImageMask;

                if (maskImage != null)
                {
                    name = Writer.AddDirectImageSimple(maskImage);
                    prs.AddXObject(name, Writer.GetImageReference(name));
                }

                name = Writer.AddDirectImageSimple(image);
                name = prs.AddXObject(name, Writer.GetImageReference(name));
                Content.Append(' ').Append(name.GetBytes()).Append(" Do Q").Append_i(Separator);
            }
        }

        if (image.HasBorders())
        {
            SaveState();
            var w = image.Width;
            var h = image.Height;
            ConcatCtm(a / w, b / w, c / h, d / h, e, f);
            Rectangle(image);
            RestoreState();
        }

        if (image.Layer != null)
        {
            EndLayer();
        }

        var annot = image.Annotation;

        if (annot == null)
        {
            return;
        }

        var r = new float[_unitRect.Length];

        for (var k = 0; k < _unitRect.Length; k += 2)
        {
            r[k] = a * _unitRect[k] + c * _unitRect[k + 1] + e;
            r[k + 1] = b * _unitRect[k] + d * _unitRect[k + 1] + f;
        }

        var llx = r[0];
        var lly = r[1];
        var urx = llx;
        var ury = lly;

        for (var k = 2; k < r.Length; k += 2)
        {
            llx = Math.Min(llx, r[k]);
            lly = Math.Min(lly, r[k + 1]);
            urx = Math.Max(urx, r[k]);
            ury = Math.Max(ury, r[k + 1]);
        }

        annot = new Annotation(annot);
        annot.SetDimensions(llx, lly, urx, ury);
        var an = PdfAnnotationsImp.ConvertAnnotation(Writer, annot, new Rectangle(llx, lly, urx, ury));

        if (an == null)
        {
            return;
        }

        AddAnnotation(an);
    }

    /// <summary>
    ///     Adds a named outline to the document.
    /// </summary>
    /// <param name="outline">the outline</param>
    /// <param name="name">the name for the local destination</param>
    public void AddOutline(PdfOutline outline, string name)
    {
        if (outline == null)
        {
            throw new ArgumentNullException(nameof(outline));
        }

        CheckWriter();
        Pdf.AddOutline(outline, name);
    }

    /// <summary>
    ///     Adds a PostScript XObject to this content.
    /// </summary>
    /// <param name="psobject">the object</param>
    public void AddPsxObject(PdfPsxObject psobject)
    {
        if (psobject == null)
        {
            throw new ArgumentNullException(nameof(psobject));
        }

        CheckWriter();
        var name = Writer.AddDirectTemplateSimple(psobject, null);
        var prs = PageResources;
        name = prs.AddXObject(name, psobject.IndirectReference);
        Content.Append(name.GetBytes()).Append(" Do").Append_i(Separator);
    }

    /// <summary>
    ///     Adds a template to this content.
    /// </summary>
    /// <param name="pdfTemplate">the template</param>
    /// <param name="a">an element of the transformation matrix</param>
    /// <param name="b">an element of the transformation matrix</param>
    /// <param name="c">an element of the transformation matrix</param>
    /// <param name="d">an element of the transformation matrix</param>
    /// <param name="e">an element of the transformation matrix</param>
    /// <param name="f">an element of the transformation matrix</param>
    public virtual void AddTemplate(PdfTemplate pdfTemplate, float a, float b, float c, float d, float e, float f)
    {
        if (pdfTemplate == null)
        {
            throw new ArgumentNullException(nameof(pdfTemplate));
        }

        CheckWriter();
        CheckNoPattern(pdfTemplate);
        var name = Writer.AddDirectTemplateSimple(pdfTemplate, null);
        var prs = PageResources;
        name = prs.AddXObject(name, pdfTemplate.IndirectReference);
        Content.Append("q ");
        Content.Append(a).Append(' ');
        Content.Append(b).Append(' ');
        Content.Append(c).Append(' ');
        Content.Append(d).Append(' ');
        Content.Append(e).Append(' ');
        Content.Append(f).Append(" cm ");
        Content.Append(name.GetBytes()).Append(" Do Q").Append_i(Separator);
    }

    /// <summary>
    ///     Adds a template to this content.
    /// </summary>
    /// <param name="pdfTemplate">the template</param>
    /// <param name="x">the x location of this template</param>
    /// <param name="y">the y location of this template</param>
    public void AddTemplate(PdfTemplate pdfTemplate, float x, float y)
        => AddTemplate(pdfTemplate, 1, 0, 0, 1, x, y);

    /// <summary>
    ///     Draws a partial ellipse inscribed within the rectangle x1,y1,x2,y2,
    ///     starting at startAng degrees and covering extent degrees. Angles
    ///     start with 0 to the right (+x) and increase counter-clockwise.
    /// </summary>
    /// <param name="x1">a corner of the enclosing rectangle</param>
    /// <param name="y1">a corner of the enclosing rectangle</param>
    /// <param name="x2">a corner of the enclosing rectangle</param>
    /// <param name="y2">a corner of the enclosing rectangle</param>
    /// <param name="startAng">starting angle in degrees</param>
    /// <param name="extent">angle extent in degrees</param>
    public void Arc(float x1, float y1, float x2, float y2, float startAng, float extent)
    {
        var ar = BezierArc(x1, y1, x2, y2, startAng, extent);

        if (ar.Count == 0)
        {
            return;
        }

        var pt = ar[0];
        MoveTo(pt[0], pt[1]);

        for (var k = 0; k < ar.Count; ++k)
        {
            pt = ar[k];
            CurveTo(pt[2], pt[3], pt[4], pt[5], pt[6], pt[7]);
        }
    }

    /// <summary>
    ///     Begins a graphic block whose visibility is controled by the  layer .
    ///     Blocks can be nested. Each block must be terminated by an {@link #endLayer()}.
    ///     Note that nested layers with {@link PdfLayer#addChild(PdfLayer)} only require a single
    ///     call to this method and a single call to {@link #endLayer()}; all the nesting control
    ///     is built in.
    /// </summary>
    /// <param name="layer">the layer</param>
    public void BeginLayer(IPdfOcg layer)
    {
        if (layer is PdfLayer && ((PdfLayer)layer).Title != null)
        {
            throw new ArgumentException("A title is not a layer");
        }

        if (LayerDepth == null)
        {
            LayerDepth = new List<int>();
        }

        if (layer is PdfLayerMembership)
        {
            LayerDepth.Add(1);
            beginLayer2(layer);

            return;
        }

        var n = 0;
        var la = (PdfLayer)layer;

        while (la != null)
        {
            if (la.Title == null)
            {
                beginLayer2(la);
                ++n;
            }

            la = la.Parent;
        }

        LayerDepth.Add(n);
    }

    /// <summary>
    ///     Begins a marked content sequence. This sequence will be tagged with the structure  struc .
    ///     The same structure can be used several times to connect text that belongs to the same logical segment
    ///     but is in a different location, like the same paragraph crossing to another page, for example.
    /// </summary>
    /// <param name="struc">the tagging structure</param>
    public void BeginMarkedContentSequence(PdfStructureElement struc)
    {
        if (struc == null)
        {
            throw new ArgumentNullException(nameof(struc));
        }

        var obj = struc.Get(PdfName.K);
        var mark = Pdf.GetMarkPoint();

        if (obj != null)
        {
            PdfArray ar = null;

            if (obj.IsNumber())
            {
                ar = new PdfArray();
                ar.Add(obj);
                struc.Put(PdfName.K, ar);
            }
            else if (obj.IsArray())
            {
                ar = (PdfArray)obj;

                if (!ar[0].IsNumber())
                {
                    throw new ArgumentException("The structure has kids.");
                }
            }
            else
            {
                throw new ArgumentException("Unknown object at /K " + obj.GetType());
            }

            var dic = new PdfDictionary(PdfName.Mcr);
            dic.Put(PdfName.Pg, Writer.CurrentPage);
            dic.Put(PdfName.Mcid, new PdfNumber(mark));
            ar.Add(dic);
            struc.SetPageMark(Writer.PageNumber - 1, -1);
        }
        else
        {
            struc.SetPageMark(Writer.PageNumber - 1, mark);
            struc.Put(PdfName.Pg, Writer.CurrentPage);
        }

        Pdf.IncMarkPoint();
        _mcDepth++;

        Content.Append(struc.Get(PdfName.S).GetBytes()).Append(" <</MCID ").Append(mark).Append(">> BDC")
            .Append_i(Separator);
    }

    /// <summary>
    ///     Begins a marked content sequence. If property is  null  the mark will be of the type
    ///     BMC  otherwise it will be  BDC .
    ///     to include the property in the resource dictionary with the possibility of reusing
    /// </summary>
    /// <param name="tag">the tag</param>
    /// <param name="property">the property</param>
    /// <param name="inline"> true  to include the property in the content or  false </param>
    public void BeginMarkedContentSequence(PdfName tag, PdfDictionary property, bool inline)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (property == null)
        {
            Content.Append(tag.GetBytes()).Append(" BMC").Append_i(Separator);

            return;
        }

        Content.Append(tag.GetBytes()).Append(' ');

        if (inline)
        {
            property.ToPdf(Writer, Content);
        }
        else
        {
            PdfObject[] objs;

            if (Writer.PropertyExists(property))
            {
                objs = Writer.AddSimpleProperty(property, null);
            }
            else
            {
                objs = Writer.AddSimpleProperty(property, Writer.PdfIndirectReference);
            }

            var name = (PdfName)objs[0];
            var prs = PageResources;
            name = prs.AddProperty(name, (PdfIndirectReference)objs[1]);
            Content.Append(name.GetBytes());
        }

        Content.Append(" BDC").Append_i(Separator);
        ++_mcDepth;
    }

    /// <summary>
    ///     This is just a shorthand to  beginMarkedContentSequence(tag, null, false) .
    /// </summary>
    /// <param name="tag">the tag</param>
    public void BeginMarkedContentSequence(PdfName tag)
        => BeginMarkedContentSequence(tag, null, false);

    /// <summary>
    ///     Starts the writing of text.
    /// </summary>
    public void BeginText()
    {
        if (_inText)
        {
            throw new IllegalPdfSyntaxException("Unbalanced begin/end text operators.");
        }

        _inText = true;
        State.XTlm = 0;
        State.YTlm = 0;
        Content.Append("BT").Append_i(Separator);
    }

    /// <summary>
    ///     Draws a circle. The endpoint will (x+r, y).
    /// </summary>
    /// <param name="x">x center of circle</param>
    /// <param name="y">y center of circle</param>
    /// <param name="r">radius of circle</param>
    public void Circle(float x, float y, float r)
    {
        var b = 0.5523f;
        MoveTo(x + r, y);
        CurveTo(x + r, y + r * b, x + r * b, y + r, x, y + r);
        CurveTo(x - r * b, y + r, x - r, y + r * b, x - r, y);
        CurveTo(x - r, y - r * b, x - r * b, y - r, x, y - r);
        CurveTo(x + r * b, y - r, x + r, y - r * b, x + r, y);
    }

    public void Clip()
        => Content.Append('W').Append_i(Separator);

    public void ClosePath()
        => Content.Append('h').Append_i(Separator);

    public void ClosePathEoFillStroke()
        => Content.Append("b*").Append_i(Separator);

    public void ClosePathFillStroke()
        => Content.Append('b').Append_i(Separator);

    public void ClosePathStroke()
        => Content.Append('s').Append_i(Separator);

    /// <summary>
    ///     Concatenate a matrix to the current transformation matrix.
    /// </summary>
    /// <param name="a">an element of the transformation matrix</param>
    /// <param name="b">an element of the transformation matrix</param>
    /// <param name="c">an element of the transformation matrix</param>
    /// <param name="d">an element of the transformation matrix</param>
    /// <param name="e">an element of the transformation matrix</param>
    /// <param name="f">an element of the transformation matrix</param>
    public void ConcatCtm(float a, float b, float c, float d, float e, float f)
    {
        Content.Append(a).Append(' ').Append(b).Append(' ').Append(c).Append(' ');
        Content.Append(d).Append(' ').Append(e).Append(' ').Append(f).Append(" cm").Append_i(Separator);
    }

    /// <summary>
    ///     Creates a new appearance to be used with form fields.
    /// </summary>
    /// <param name="width">the bounding box width</param>
    /// <param name="height">the bounding box height</param>
    /// <returns>the appearance created</returns>
    public PdfAppearance CreateAppearance(float width, float height)
        => CreateAppearance(width, height, null);

    /// <summary>
    ///     Create a new colored tiling pattern.
    ///     May be either positive or negative, but not zero.
    ///     May be either positive or negative, but not zero.
    /// </summary>
    /// <param name="width">the width of the pattern</param>
    /// <param name="height">the height of the pattern</param>
    /// <param name="xstep">the desired horizontal spacing between pattern cells.</param>
    /// <param name="ystep">the desired vertical spacing between pattern cells.</param>
    /// <returns>the  PdfPatternPainter  where the pattern will be created</returns>
    public PdfPatternPainter CreatePattern(float width, float height, float xstep, float ystep)
    {
        CheckWriter();

        if (xstep.ApproxEquals(0.0f) || ystep.ApproxEquals(0.0f))
        {
            throw new InvalidOperationException("XStep or YStep can not be ZERO.");
        }

        var painter = new PdfPatternPainter(Writer);
        painter.Width = width;
        painter.Height = height;
        painter.XStep = xstep;
        painter.YStep = ystep;
        Writer.AddSimplePattern(painter);

        return painter;
    }

    /// <summary>
    ///     Create a new colored tiling pattern. Variables xstep and ystep are set to the same values
    ///     of width and height.
    /// </summary>
    /// <param name="width">the width of the pattern</param>
    /// <param name="height">the height of the pattern</param>
    /// <returns>the  PdfPatternPainter  where the pattern will be created</returns>
    public PdfPatternPainter CreatePattern(float width, float height)
        => CreatePattern(width, height, width, height);

    /// <summary>
    ///     Create a new uncolored tiling pattern.
    ///     May be either positive or negative, but not zero.
    ///     May be either positive or negative, but not zero.
    /// </summary>
    /// <param name="width">the width of the pattern</param>
    /// <param name="height">the height of the pattern</param>
    /// <param name="xstep">the desired horizontal spacing between pattern cells.</param>
    /// <param name="ystep">the desired vertical spacing between pattern cells.</param>
    /// <param name="color">the default color. Can be  null </param>
    /// <returns>the  PdfPatternPainter  where the pattern will be created</returns>
    public PdfPatternPainter CreatePattern(float width, float height, float xstep, float ystep, BaseColor color)
    {
        CheckWriter();

        if (xstep.ApproxEquals(0.0f) || ystep.ApproxEquals(0.0f))
        {
            throw new InvalidOperationException("XStep or YStep can not be ZERO.");
        }

        var painter = new PdfPatternPainter(Writer, color);
        painter.Width = width;
        painter.Height = height;
        painter.XStep = xstep;
        painter.YStep = ystep;
        Writer.AddSimplePattern(painter);

        return painter;
    }

    /// <summary>
    ///     Create a new uncolored tiling pattern.
    ///     Variables xstep and ystep are set to the same values
    ///     of width and height.
    /// </summary>
    /// <param name="width">the width of the pattern</param>
    /// <param name="height">the height of the pattern</param>
    /// <param name="color">the default color. Can be  null </param>
    /// <returns>the  PdfPatternPainter  where the pattern will be created</returns>
    public PdfPatternPainter CreatePattern(float width, float height, BaseColor color)
        => CreatePattern(width, height, width, height, color);

    /// <summary>
    ///     Creates a new template.
    ///     Creates a new template that is nothing more than a form XObject. This template can be included
    ///     in this  PdfContentByte  or in another template. Templates are only written
    ///     to the output when the document is closed permitting things like showing text in the first page
    ///     that is only defined in the last page.
    /// </summary>
    /// <param name="width">the bounding box width</param>
    /// <param name="height">the bounding box height</param>
    /// <returns>the templated created</returns>
    public PdfTemplate CreateTemplate(float width, float height)
        => CreateTemplate(width, height, null);

    public void CurveFromTo(float x1, float y1, float x3, float y3)
        => Content.Append(x1).Append(' ').Append(y1).Append(' ').Append(x3).Append(' ').Append(y3).Append(" y")
            .Append_i(Separator);

    public void CurveTo(double x1, double y1, double x2, double y2, double x3, double y3)
        => Content.Append(x1).Append(' ').Append(y1).Append(' ').Append(x2).Append(' ').Append(y2).Append(' ')
            .Append(x3).Append(' ').Append(y3).Append(" c").Append_i(Separator);

    public void CurveTo(float x1, float y1, float x2, float y2, float x3, float y3)
        => CurveTo(x1, y1, x2, y2, x3, (double)y3);

    public void CurveTo(float x2, float y2, float x3, float y3)
        => Content.Append(x2).Append(' ').Append(y2).Append(' ').Append(x3).Append(' ').Append(y3).Append(" v")
            .Append_i(Separator);

    public void DrawButton(float llx, float lly, float urx, float ury, string text, BaseFont bf, float size)
    {
        if (llx > urx)
        {
            var x = llx;
            llx = urx;
            urx = x;
        }

        if (lly > ury)
        {
            var y = lly;
            lly = ury;
            ury = y;
        }

        // black rectangle not filled
        SetColorStroke(new BaseColor(0x00, 0x00, 0x00));
        SetLineWidth(1);
        SetLineCap(0);
        Rectangle(llx, lly, urx - llx, ury - lly);
        Stroke();

        // silver rectangle filled
        SetLineWidth(1);
        SetLineCap(0);
        SetColorFill(new BaseColor(0xC0, 0xC0, 0xC0));
        Rectangle(llx + 0.5f, lly + 0.5f, urx - llx - 1f, ury - lly - 1f);
        Fill();

        // white lines
        SetColorStroke(new BaseColor(0xFF, 0xFF, 0xFF));
        SetLineWidth(1);
        SetLineCap(0);
        MoveTo(llx + 1f, lly + 1f);
        LineTo(llx + 1f, ury - 1f);
        LineTo(urx - 1f, ury - 1f);
        Stroke();

        // dark grey lines
        SetColorStroke(new BaseColor(0xA0, 0xA0, 0xA0));
        SetLineWidth(1);
        SetLineCap(0);
        MoveTo(llx + 1f, lly + 1f);
        LineTo(urx - 1f, lly + 1f);
        LineTo(urx - 1f, ury - 1f);
        Stroke();

        // text
        ResetRgbColorFill();
        BeginText();
        SetFontAndSize(bf, size);
        ShowTextAligned(ALIGN_CENTER, text, llx + (urx - llx) / 2, lly + (ury - lly - size) / 2, 0);
        EndText();
    }

    public void DrawRadioField(float llx, float lly, float urx, float ury, bool on)
    {
        if (llx > urx)
        {
            var x = llx;
            llx = urx;
            urx = x;
        }

        if (lly > ury)
        {
            var y = lly;
            lly = ury;
            ury = y;
        }

        // silver circle
        SetLineWidth(1);
        SetLineCap(1);
        SetColorStroke(new BaseColor(0xC0, 0xC0, 0xC0));
        Arc(llx + 1f, lly + 1f, urx - 1f, ury - 1f, 0f, 360f);
        Stroke();

        // gray circle-segment
        SetLineWidth(1);
        SetLineCap(1);
        SetColorStroke(new BaseColor(0xA0, 0xA0, 0xA0));
        Arc(llx + 0.5f, lly + 0.5f, urx - 0.5f, ury - 0.5f, 45, 180);
        Stroke();

        // black circle-segment
        SetLineWidth(1);
        SetLineCap(1);
        SetColorStroke(new BaseColor(0x00, 0x00, 0x00));
        Arc(llx + 1.5f, lly + 1.5f, urx - 1.5f, ury - 1.5f, 45, 180);
        Stroke();

        if (on)
        {
            // gray circle
            SetLineWidth(1);
            SetLineCap(1);
            SetColorFill(new BaseColor(0x00, 0x00, 0x00));
            Arc(llx + 4f, lly + 4f, urx - 4f, ury - 4f, 0, 360);
            Fill();
        }
    }

    public void DrawTextField(float llx, float lly, float urx, float ury)
    {
        if (llx > urx)
        {
            var x = llx;
            llx = urx;
            urx = x;
        }

        if (lly > ury)
        {
            var y = lly;
            lly = ury;
            ury = y;
        }

        // silver rectangle not filled
        SetColorStroke(new BaseColor(0xC0, 0xC0, 0xC0));
        SetLineWidth(1);
        SetLineCap(0);
        Rectangle(llx, lly, urx - llx, ury - lly);
        Stroke();

        // white rectangle filled
        SetLineWidth(1);
        SetLineCap(0);
        SetColorFill(new BaseColor(0xFF, 0xFF, 0xFF));
        Rectangle(llx + 0.5f, lly + 0.5f, urx - llx - 1f, ury - lly - 1f);
        Fill();

        // silver lines
        SetColorStroke(new BaseColor(0xC0, 0xC0, 0xC0));
        SetLineWidth(1);
        SetLineCap(0);
        MoveTo(llx + 1f, lly + 1.5f);
        LineTo(urx - 1.5f, lly + 1.5f);
        LineTo(urx - 1.5f, ury - 1f);
        Stroke();

        // gray lines
        SetColorStroke(new BaseColor(0xA0, 0xA0, 0xA0));
        SetLineWidth(1);
        SetLineCap(0);
        MoveTo(llx + 1f, lly + 1);
        LineTo(llx + 1f, ury - 1f);
        LineTo(urx - 1f, ury - 1f);
        Stroke();

        // black lines
        SetColorStroke(new BaseColor(0x00, 0x00, 0x00));
        SetLineWidth(1);
        SetLineCap(0);
        MoveTo(llx + 2f, lly + 2f);
        LineTo(llx + 2f, ury - 2f);
        LineTo(urx - 2f, ury - 2f);
        Stroke();
    }

    /// <summary>
    ///     Draws an ellipse inscribed within the rectangle x1,y1,x2,y2.
    /// </summary>
    /// <param name="x1">a corner of the enclosing rectangle</param>
    /// <param name="y1">a corner of the enclosing rectangle</param>
    /// <param name="x2">a corner of the enclosing rectangle</param>
    /// <param name="y2">a corner of the enclosing rectangle</param>
    public void Ellipse(float x1, float y1, float x2, float y2)
        => Arc(x1, y1, x2, y2, 0f, 360f);

    /// <summary>
    ///     Ends a layer controled graphic block. It will end the most recent open block.
    /// </summary>
    public void EndLayer()
    {
        var n = 1;

        if (LayerDepth != null && LayerDepth.Count > 0)
        {
            n = LayerDepth[LayerDepth.Count - 1];
            LayerDepth.RemoveAt(LayerDepth.Count - 1);
        }
        else
        {
            throw new IllegalPdfSyntaxException("Unbalanced layer operators.");
        }

        while (n-- > 0)
        {
            Content.Append("EMC").Append_i(Separator);
        }
    }

    /// <summary>
    ///     Ends a marked content sequence
    /// </summary>
    public void EndMarkedContentSequence()
    {
        if (_mcDepth == 0)
        {
            throw new IllegalPdfSyntaxException("Unbalanced begin/end marked content operators.");
        }

        --_mcDepth;
        Content.Append("EMC").Append_i(Separator);
    }

    /// <summary>
    ///     Ends the writing of text and makes the current font invalid.
    /// </summary>
    public void EndText()
    {
        if (!_inText)
        {
            throw new IllegalPdfSyntaxException("Unbalanced begin/end text operators.");
        }

        _inText = false;
        Content.Append("ET").Append_i(Separator);
    }

    public void EoClip()
        => Content.Append("W*").Append_i(Separator);

    public void EoFill()
        => Content.Append("f*").Append_i(Separator);

    public void EoFillStroke()
        => Content.Append("B*").Append_i(Separator);

    public void Fill()
        => Content.Append('f').Append_i(Separator);

    public void FillStroke()
        => Content.Append('B').Append_i(Separator);

    public float GetEffectiveStringWidth(string text, bool kerned)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var bf = State.FontDetails.BaseFont;

        float w;

        if (kerned)
        {
            w = bf.GetWidthPointKerned(text, State.size);
        }
        else
        {
            w = bf.GetWidthPoint(text, State.size);
        }

        if (State.CharSpace.ApproxNotEqual(0.0f) && text.Length > 1)
        {
            w += State.CharSpace * (text.Length - 1);
        }

        var ft = bf.FontType;

        if (State.WordSpace.ApproxNotEqual(0.0f) &&
            (ft == BaseFont.FONT_TYPE_T1 || ft == BaseFont.FONT_TYPE_TT || ft == BaseFont.FONT_TYPE_T3))
        {
            for (var i = 0; i < text.Length - 1; i++)
            {
                if (text[i] == ' ')
                {
                    w += State.WordSpace;
                }
            }
        }

        if (State.Scale.ApproxNotEqual(100.0f))
        {
            w = w * State.Scale / 100.0f;
        }

        //System.out.Println("String width = " + Float.ToString(w));
        return w;
    }

    public void LineTo(double x, double y)
        => Content.Append(x).Append(' ').Append(y).Append(" l").Append_i(Separator);

    public void LineTo(float x, float y)
        => LineTo(x, (double)y);

    /// <summary>
    ///     The local destination to where a local goto with the same
    ///     name will jump.
    ///     false  if a local destination with the same name
    ///     already exists
    /// </summary>
    /// <param name="name">the name of this local destination</param>
    /// <param name="destination">the  PdfDestination  with the jump coordinates</param>
    /// <returns> true  if the local destination was added,</returns>
    public bool LocalDestination(string name, PdfDestination destination)
    {
        if (destination == null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        return Pdf.LocalDestination(name, destination);
    }

    /// <summary>
    ///     Implements a link to other part of the document. The jump will
    ///     be made to a local destination with the same name, that must exist.
    /// </summary>
    /// <param name="name">the name for this link</param>
    /// <param name="llx">the lower left x corner of the activation area</param>
    /// <param name="lly">the lower left y corner of the activation area</param>
    /// <param name="urx">the upper right x corner of the activation area</param>
    /// <param name="ury">the upper right y corner of the activation area</param>
    public void LocalGoto(string name, float llx, float lly, float urx, float ury)
        => Pdf.LocalGoto(name, llx, lly, urx, ury);

    /// <summary>
    ///     Moves to the start of the next line, offset from the start of the current line.
    /// </summary>
    /// <param name="x">x-coordinate of the new current point</param>
    /// <param name="y">y-coordinate of the new current point</param>
    public void MoveText(float x, float y)
    {
        State.XTlm += x;
        State.YTlm += y;
        Content.Append(x).Append(' ').Append(y).Append(" Td").Append_i(Separator);
    }

    /// <summary>
    ///     Moves to the start of the next line, offset from the start of the current line.
    ///     As a side effect, this sets the leading parameter in the text state.
    /// </summary>
    /// <param name="x">offset of the new current point</param>
    /// <param name="y">y-coordinate of the new current point</param>
    public void MoveTextWithLeading(float x, float y)
    {
        State.XTlm += x;
        State.YTlm += y;
        State.leading = -y;
        Content.Append(x).Append(' ').Append(y).Append(" TD").Append_i(Separator);
    }

    public void MoveTo(double x, double y)
        => Content.Append(x).Append(' ').Append(y).Append(" m").Append_i(Separator);

    /// <summary>
    ///     Moves to the next line and shows  text .
    /// </summary>
    /// <param name="text">the text to write</param>
    public void NewlineShowText(string text)
    {
        State.YTlm -= State.leading;
        showText2(text);
        Content.Append('\'').Append_i(Separator);
    }

    /// <summary>
    ///     Moves to the next line and shows text string, using the given values of the character and word spacing parameters.
    /// </summary>
    /// <param name="wordSpacing">a parameter</param>
    /// <param name="charSpacing">a parameter</param>
    /// <param name="text">the text to write</param>
    public void NewlineShowText(float wordSpacing, float charSpacing, string text)
    {
        State.YTlm -= State.leading;
        Content.Append(wordSpacing).Append(' ').Append(charSpacing);
        showText2(text);
        Content.Append("\"").Append_i(Separator);

        // The " operator sets charSpace and wordSpace into graphics state
        // (cfr PDF reference v1.6, table 5.6)
        State.CharSpace = charSpacing;
        State.WordSpace = wordSpacing;
    }

    /// <summary>
    ///     Moves to the start of the next line.
    /// </summary>
    public void NewlineText()
    {
        State.YTlm -= State.leading;
        Content.Append("T*").Append_i(Separator);
    }

    public void NewPath()
        => Content.Append('n').Append_i(Separator);

    /// <summary>
    ///     Paints using a shading object.
    /// </summary>
    /// <param name="shading">the shading object</param>
    public virtual void PaintShading(PdfShading shading)
    {
        if (shading == null)
        {
            throw new ArgumentNullException(nameof(shading));
        }

        Writer.AddSimpleShading(shading);
        var prs = PageResources;
        var name = prs.AddShading(shading.ShadingName, shading.ShadingReference);
        Content.Append(name.GetBytes()).Append(" sh").Append_i(Separator);
        var details = shading.ColorDetails;

        if (details != null)
        {
            prs.AddColor(details.ColorName, details.IndirectReference);
        }
    }

    /// <summary>
    ///     Paints using a shading pattern.
    /// </summary>
    /// <param name="shading">the shading pattern</param>
    public virtual void PaintShading(PdfShadingPattern shading)
    {
        if (shading == null)
        {
            throw new ArgumentNullException(nameof(shading));
        }

        PaintShading(shading.Shading);
    }

    public void Rectangle(float x, float y, float w, float h)
        => Content.Append(x).Append(' ').Append(y).Append(' ').Append(w).Append(' ').Append(h).Append(" re")
            .Append_i(Separator);

    public void Rectangle(Rectangle rectangle)
    {
        if (rectangle == null)
        {
            throw new ArgumentNullException(nameof(rectangle));
        }

        // the coordinates of the border are retrieved
        var x1 = rectangle.Left;
        var y1 = rectangle.Bottom;
        var x2 = rectangle.Right;
        var y2 = rectangle.Top;

        // the backgroundcolor is set
        var background = rectangle.BackgroundColor;

        if (background != null)
        {
            SetColorFill(background);
            Rectangle(x1, y1, x2 - x1, y2 - y1);
            Fill();
            ResetRgbColorFill();
        }

        // if the element hasn't got any borders, nothing is added
        if (!rectangle.HasBorders())
        {
            return;
        }

        // if any of the individual border colors are set
        // we draw the borders all around using the
        // different colors
        if (rectangle.UseVariableBorders)
        {
            VariableRectangle(rectangle);
        }
        else
        {
            // the width is set to the width of the element
            if (rectangle.BorderWidth.ApproxNotEqual(text.Rectangle.UNDEFINED))
            {
                SetLineWidth(rectangle.BorderWidth);
            }

            // the color is set to the color of the element
            var color = rectangle.BorderColor;

            if (color != null)
            {
                SetColorStroke(color);
            }

            // if the box is a rectangle, it is added as a rectangle
            if (rectangle.HasBorder(text.Rectangle.BOX))
            {
                Rectangle(x1, y1, x2 - x1, y2 - y1);
            }

            // if the border isn't a rectangle, the different sides are added apart
            else
            {
                if (rectangle.HasBorder(text.Rectangle.RIGHT_BORDER))
                {
                    MoveTo(x2, y1);
                    LineTo(x2, y2);
                }

                if (rectangle.HasBorder(text.Rectangle.LEFT_BORDER))
                {
                    MoveTo(x1, y1);
                    LineTo(x1, y2);
                }

                if (rectangle.HasBorder(text.Rectangle.BOTTOM_BORDER))
                {
                    MoveTo(x1, y1);
                    LineTo(x2, y1);
                }

                if (rectangle.HasBorder(text.Rectangle.TOP_BORDER))
                {
                    MoveTo(x1, y2);
                    LineTo(x2, y2);
                }
            }

            Stroke();

            if (color != null)
            {
                ResetRgbColorStroke();
            }
        }
    }

    /// <summary>
    ///     Implements a link to another document.
    /// </summary>
    /// <param name="filename">the filename for the remote document</param>
    /// <param name="name">the name to jump to</param>
    /// <param name="llx">the lower left x corner of the activation area</param>
    /// <param name="lly">the lower left y corner of the activation area</param>
    /// <param name="urx">the upper right x corner of the activation area</param>
    /// <param name="ury">the upper right y corner of the activation area</param>
    public static void RemoteGoto(string filename, string name, float llx, float lly, float urx, float ury)
        => RemoteGoto(filename, name, llx, lly, urx, ury);

    /// <summary>
    ///     Implements a link to another document.
    /// </summary>
    /// <param name="filename">the filename for the remote document</param>
    /// <param name="page">the page to jump to</param>
    /// <param name="llx">the lower left x corner of the activation area</param>
    /// <param name="lly">the lower left y corner of the activation area</param>
    /// <param name="urx">the upper right x corner of the activation area</param>
    /// <param name="ury">the upper right y corner of the activation area</param>
    public void RemoteGoto(string filename, int page, float llx, float lly, float urx, float ury)
        => Pdf.RemoteGoto(filename, page, llx, lly, urx, ury);

    /// <summary>
    ///     Closes the path and strokes it.
    /// </summary>
    /// <summary>
    ///     Fills the path, using the non-zero winding number rule to determine the region to fill.
    /// </summary>
    /// <summary>
    ///     Fills the path, using the even-odd rule to determine the region to fill.
    /// </summary>
    /// <summary>
    ///     Fills the path using the non-zero winding number rule to determine the region to fill and strokes it.
    /// </summary>
    /// <summary>
    ///     Closes the path, fills it using the non-zero winding number rule to determine the region to fill and strokes it.
    /// </summary>
    /// <summary>
    ///     Fills the path, using the even-odd rule to determine the region to fill and strokes it.
    /// </summary>
    /// <summary>
    ///     Closes the path, fills it using the even-odd rule to determine the region to fill and strokes it.
    /// </summary>
    /// <summary>
    ///     Makes this  PdfContentByte  empty.
    ///     Calls  reset( true )
    /// </summary>
    public void Reset()
        => Reset(true);

    /// <summary>
    ///     Makes this  PdfContentByte  empty.
    ///     @since 2.1.6
    /// </summary>
    /// <param name="validateContent">will call  sanityCheck()  if true.</param>
    public void Reset(bool validateContent)
    {
        Content.Reset();

        if (validateContent)
        {
            SanityCheck();
        }

        State = new GraphicState();
    }

    public virtual void ResetCmykColorFill()
        => Content.Append("0 0 0 1 k").Append_i(Separator);

    public virtual void ResetCmykColorStroke()
        => Content.Append("0 0 0 1 K").Append_i(Separator);

    public virtual void ResetGrayFill()
        => Content.Append("0 g").Append_i(Separator);

    public virtual void ResetGrayStroke()
        => Content.Append("0 G").Append_i(Separator);

    public virtual void ResetRgbColorFill()
        => Content.Append("0 g").Append_i(Separator);

    public virtual void ResetRgbColorStroke()
        => Content.Append("0 G").Append_i(Separator);

    /// <summary>
    ///     Restores the graphic state.  saveState  and
    ///     restoreState  must be balanced.
    /// </summary>
    public void RestoreState()
    {
        Content.Append('Q').Append_i(Separator);
        var idx = StateList.Count - 1;

        if (idx < 0)
        {
            throw new IllegalPdfSyntaxException("Unbalanced save/restore state operators.");
        }

        State = StateList[idx];
        StateList.RemoveAt(idx);
    }

    /// <summary>
    ///     Adds a round rectangle to the current path.
    /// </summary>
    /// <param name="x">x-coordinate of the starting point</param>
    /// <param name="y">y-coordinate of the starting point</param>
    /// <param name="w">width</param>
    /// <param name="h">height</param>
    /// <param name="r">radius of the arc corner</param>
    public void RoundRectangle(float x, float y, float w, float h, float r)
    {
        if (w < 0)
        {
            x += w;
            w = -w;
        }

        if (h < 0)
        {
            y += h;
            h = -h;
        }

        if (r < 0)
        {
            r = -r;
        }

        var b = 0.4477f;
        MoveTo(x + r, y);
        LineTo(x + w - r, y);
        CurveTo(x + w - r * b, y, x + w, y + r * b, x + w, y + r);
        LineTo(x + w, y + h - r);
        CurveTo(x + w, y + h - r * b, x + w - r * b, y + h, x + w - r, y + h);
        LineTo(x + r, y + h);
        CurveTo(x + r * b, y + h, x, y + h - r * b, x, y + h - r);
        LineTo(x, y + r);
        CurveTo(x, y + r * b, x + r * b, y, x + r, y);
    }

    /// <summary>
    ///     Checks for any dangling state: Mismatched save/restore state, begin/end text,
    ///     begin/end layer, or begin/end marked content sequence.
    ///     If found, this function will throw.  This function is called automatically
    ///     during a reset() (from Document.newPage() for example), and before writing
    ///     itself out in toPdf().
    ///     One possible cause: not calling myPdfGraphics2D.dispose() will leave dangling
    ///     saveState() calls.
    ///     @since 2.1.6
    ///     @throws IllegalPdfSyntaxException (a runtime exception)
    /// </summary>
    public void SanityCheck()
    {
        if (_mcDepth != 0)
        {
            throw new IllegalPdfSyntaxException("Unbalanced marked content operators.");
        }

        if (_inText)
        {
            throw new IllegalPdfSyntaxException("Unbalanced begin/end text operators.");
        }

        if (LayerDepth != null && LayerDepth.Count > 0)
        {
            throw new IllegalPdfSyntaxException("Unbalanced layer operators.");
        }

        if (StateList.Count > 0)
        {
            throw new IllegalPdfSyntaxException("Unbalanced save/restore state operators.");
        }
    }

    /// <summary>
    ///     Saves the graphic state.  saveState  and
    ///     restoreState  must be balanced.
    /// </summary>
    public void SaveState()
    {
        Content.Append('q').Append_i(Separator);
        StateList.Add(new GraphicState(State));
    }

    /// <summary>
    ///     Implements an action in an area.
    /// </summary>
    /// <param name="action">the  PdfAction </param>
    /// <param name="llx">the lower left x corner of the activation area</param>
    /// <param name="lly">the lower left y corner of the activation area</param>
    /// <param name="urx">the upper right x corner of the activation area</param>
    /// <param name="ury">the upper right y corner of the activation area</param>
    public virtual void SetAction(PdfAction action, float llx, float lly, float urx, float ury)
        => Pdf.SetAction(action, llx, lly, urx, ury);

    /// <summary>
    ///     Sets the character spacing parameter.
    /// </summary>
    /// <param name="value">a parameter</param>
    public void SetCharacterSpacing(float value)
    {
        State.CharSpace = value;
        Content.Append(value).Append(" Tc").Append_i(Separator);
    }

    public virtual void SetCmykColorFill(int cyan, int magenta, int yellow, int black)
    {
        Content.Append((float)(cyan & 0xFF) / 0xFF);
        Content.Append(' ');
        Content.Append((float)(magenta & 0xFF) / 0xFF);
        Content.Append(' ');
        Content.Append((float)(yellow & 0xFF) / 0xFF);
        Content.Append(' ');
        Content.Append((float)(black & 0xFF) / 0xFF);
        Content.Append(" k").Append_i(Separator);
    }

    public virtual void SetCmykColorFillF(float cyan, float magenta, float yellow, float black)
    {
        helperCmyk(cyan, magenta, yellow, black);
        Content.Append(" k").Append_i(Separator);
    }

    public virtual void SetCmykColorStroke(int cyan, int magenta, int yellow, int black)
    {
        Content.Append((float)(cyan & 0xFF) / 0xFF);
        Content.Append(' ');
        Content.Append((float)(magenta & 0xFF) / 0xFF);
        Content.Append(' ');
        Content.Append((float)(yellow & 0xFF) / 0xFF);
        Content.Append(' ');
        Content.Append((float)(black & 0xFF) / 0xFF);
        Content.Append(" K").Append_i(Separator);
    }

    public virtual void SetCmykColorStrokeF(float cyan, float magenta, float yellow, float black)
    {
        helperCmyk(cyan, magenta, yellow, black);
        Content.Append(" K").Append_i(Separator);
    }

    /// <summary>
    ///     Sets the fill color.  color  can be an
    ///     ExtendedColor .
    /// </summary>
    /// <param name="value">the color</param>
    public virtual void SetColorFill(BaseColor value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        PdfXConformanceImp.CheckPdfxConformance(Writer, PdfXConformanceImp.PDFXKEY_COLOR, value);
        var type = ExtendedColor.GetType(value);

        switch (type)
        {
            case ExtendedColor.TYPE_GRAY:
            {
                SetGrayFill(((GrayColor)value).Gray);

                break;
            }
            case ExtendedColor.TYPE_CMYK:
            {
                var cmyk = (CmykColor)value;
                SetCmykColorFillF(cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black);

                break;
            }
            case ExtendedColor.TYPE_SEPARATION:
            {
                var spot = (SpotColor)value;
                SetColorFill(spot.PdfSpotColor, spot.Tint);

                break;
            }
            case ExtendedColor.TYPE_PATTERN:
            {
                var pat = (PatternColor)value;
                SetPatternFill(pat.Painter);

                break;
            }
            case ExtendedColor.TYPE_SHADING:
            {
                var shading = (ShadingColor)value;
                SetShadingFill(shading.PdfShadingPattern);

                break;
            }
            default:
                SetRgbColorFill(value.R, value.G, value.B);

                break;
        }
    }

    /// <summary>
    ///     Sets the fill color to a spot color.
    ///     is 100% color
    /// </summary>
    /// <param name="sp">the spot color</param>
    /// <param name="tint">the tint for the spot color. 0 is no color and 1</param>
    public virtual void SetColorFill(PdfSpotColor sp, float tint)
    {
        CheckWriter();
        State.ColorDetails = Writer.AddSimple(sp);
        var prs = PageResources;
        var name = State.ColorDetails.ColorName;
        name = prs.AddColor(name, State.ColorDetails.IndirectReference);
        Content.Append(name.GetBytes()).Append(" cs ").Append(tint).Append(" scn").Append_i(Separator);
    }

    /// <summary>
    ///     Sets the stroke color.  color  can be an
    ///     ExtendedColor .
    /// </summary>
    /// <param name="value">the color</param>
    public virtual void SetColorStroke(BaseColor value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        PdfXConformanceImp.CheckPdfxConformance(Writer, PdfXConformanceImp.PDFXKEY_COLOR, value);
        var type = ExtendedColor.GetType(value);

        switch (type)
        {
            case ExtendedColor.TYPE_GRAY:
            {
                SetGrayStroke(((GrayColor)value).Gray);

                break;
            }
            case ExtendedColor.TYPE_CMYK:
            {
                var cmyk = (CmykColor)value;
                SetCmykColorStrokeF(cmyk.Cyan, cmyk.Magenta, cmyk.Yellow, cmyk.Black);

                break;
            }
            case ExtendedColor.TYPE_SEPARATION:
            {
                var spot = (SpotColor)value;
                SetColorStroke(spot.PdfSpotColor, spot.Tint);

                break;
            }
            case ExtendedColor.TYPE_PATTERN:
            {
                var pat = (PatternColor)value;
                SetPatternStroke(pat.Painter);

                break;
            }
            case ExtendedColor.TYPE_SHADING:
            {
                var shading = (ShadingColor)value;
                SetShadingStroke(shading.PdfShadingPattern);

                break;
            }
            default:
                SetRgbColorStroke(value.R, value.G, value.B);

                break;
        }
    }

    /// <summary>
    ///     Sets the stroke color to a spot color.
    ///     is 100% color
    /// </summary>
    /// <param name="sp">the spot color</param>
    /// <param name="tint">the tint for the spot color. 0 is no color and 1</param>
    public virtual void SetColorStroke(PdfSpotColor sp, float tint)
    {
        CheckWriter();
        State.ColorDetails = Writer.AddSimple(sp);
        var prs = PageResources;
        var name = State.ColorDetails.ColorName;
        name = prs.AddColor(name, State.ColorDetails.IndirectReference);
        Content.Append(name.GetBytes()).Append(" CS ").Append(tint).Append(" SCN").Append_i(Separator);
    }

    /// <summary>
    ///     Sets the default colorspace.
    ///     or  PdfName.DEFAULTCMYK
    /// </summary>
    /// <param name="name">the name of the colorspace. It can be  PdfName.DEFAULTGRAY ,  PdfName.DEFAULTRGB </param>
    /// <param name="obj">the colorspace. A  null  or  PdfNull  removes any colorspace with the same name</param>
    public virtual void SetDefaultColorspace(PdfName name, PdfObject obj)
    {
        var prs = PageResources;
        prs.AddDefaultColor(name, obj);
    }

    public void SetFlatness(float value)
    {
        if (value >= 0 && value <= 100)
        {
            Content.Append(value).Append(" i").Append_i(Separator);
        }
    }

    /// <summary>
    ///     Set the font and the size for the subsequent text writing.
    /// </summary>
    /// <param name="bf">the font</param>
    /// <param name="size">the font size in points</param>
    public virtual void SetFontAndSize(BaseFont bf, float size)
    {
        if (bf == null)
        {
            throw new ArgumentNullException(nameof(bf));
        }

        CheckWriter();

        if (size < 0.0001f && size > -0.0001f)
        {
            throw new ArgumentException("Font size too small: " + size);
        }

        State.size = size;
        State.FontDetails = Writer.AddSimple(bf);
        var prs = PageResources;
        var name = State.FontDetails.FontName;
        name = prs.AddFont(name, State.FontDetails.IndirectReference);
        Content.Append(name.GetBytes()).Append(' ').Append(size).Append(" Tf").Append_i(Separator);
    }

    public virtual void SetGrayFill(float value)
        => Content.Append(value).Append(" g").Append_i(Separator);

    public virtual void SetGrayStroke(float value)
        => Content.Append(value).Append(" G").Append_i(Separator);

    /// <summary>
    ///     Draws a TextField.
    /// </summary>
    /// <summary>
    ///     Draws a TextField.
    /// </summary>
    /// <summary>
    ///     Draws a button.
    /// </summary>
    /// <summary>
    ///     Sets the graphic state
    /// </summary>
    /// <param name="gstate">the graphic state</param>
    public void SetGState(PdfGState gstate)
    {
        var obj = Writer.AddSimpleExtGState(gstate);
        var prs = PageResources;
        var name = prs.AddExtGState((PdfName)obj[0], (PdfIndirectReference)obj[1]);
        Content.Append(name.GetBytes()).Append(" gs").Append_i(Separator);
    }

    /// <summary>
    ///     Sets the horizontal scaling parameter.
    /// </summary>
    /// <param name="value">a parameter</param>
    public void SetHorizontalScaling(float value)
    {
        State.Scale = value;
        Content.Append(value).Append(" Tz").Append_i(Separator);
    }

    /// <summary>
    ///     Adds the content of another  PdfContent -object to this object.
    /// </summary>
    /// <param name="v">another  PdfByteContent -object</param>
    public void SetLeading(float v)
    {
        State.leading = v;
        Content.Append(v).Append(" TL").Append_i(Separator);
    }

    public void SetLineCap(int value)
    {
        if (value >= 0 && value <= 2)
        {
            Content.Append(value).Append(" J").Append_i(Separator);
        }
    }

    public void SetLineDash(float value)
        => Content.Append("[] ").Append(value).Append(" d").Append_i(Separator);

    public void SetLineDash(float unitsOn, float phase)
        => Content.Append('[').Append(unitsOn).Append("] ").Append(phase).Append(" d").Append_i(Separator);

    public void SetLineDash(float unitsOn, float unitsOff, float phase)
        => Content.Append('[').Append(unitsOn).Append(' ').Append(unitsOff).Append("] ").Append(phase).Append(" d")
            .Append_i(Separator);

    public void SetLineDash(float[] array, float phase)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        Content.Append('[');

        for (var i = 0; i < array.Length; i++)
        {
            Content.Append(array[i]);

            if (i < array.Length - 1)
            {
                Content.Append(' ');
            }
        }

        Content.Append("] ").Append(phase).Append(" d").Append_i(Separator);
    }

    public void SetLineJoin(int value)
    {
        if (value >= 0 && value <= 2)
        {
            Content.Append(value).Append(" j").Append_i(Separator);
        }
    }

    public void SetLineWidth(float value)
        => Content.Append(value).Append(" w").Append_i(Separator);

    /// <summary>
    ///     Outputs a  string  directly to the content.
    /// </summary>
    /// <param name="s">the  string </param>
    public void SetLiteral(string s)
        => Content.Append(s);

    /// <summary>
    ///     Outputs a  char  directly to the content.
    /// </summary>
    /// <param name="c">the  char </param>
    public void SetLiteral(char c)
        => Content.Append(c);

    /// <summary>
    ///     Outputs a  float  directly to the content.
    /// </summary>
    /// <param name="n">the  float </param>
    public void SetLiteral(float n)
        => Content.Append(n);

    public void SetMiterLimit(float value)
    {
        if (value > 1)
        {
            Content.Append(value).Append(" M").Append_i(Separator);
        }
    }

    /// <summary>
    ///     Sets the fill color to a pattern. The pattern can be
    ///     colored or uncolored.
    /// </summary>
    /// <param name="p">the pattern</param>
    public virtual void SetPatternFill(PdfPatternPainter p)
    {
        if (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        if (p.IsStencil())
        {
            SetPatternFill(p, p.DefaultColor);

            return;
        }

        CheckWriter();
        var prs = PageResources;
        var name = Writer.AddSimplePattern(p);
        name = prs.AddPattern(name, p.IndirectReference);

        Content.Append(PdfName.Pattern.GetBytes()).Append(" cs ").Append(name.GetBytes()).Append(" scn")
            .Append_i(Separator);
    }

    /// <summary>
    ///     Sets the fill color to an uncolored pattern.
    /// </summary>
    /// <param name="p">the pattern</param>
    /// <param name="color">the color of the pattern</param>
    public virtual void SetPatternFill(PdfPatternPainter p, BaseColor color)
    {
        if (color == null)
        {
            throw new ArgumentNullException(nameof(color));
        }

        if (ExtendedColor.GetType(color) == ExtendedColor.TYPE_SEPARATION)
        {
            SetPatternFill(p, color, ((SpotColor)color).Tint);
        }
        else
        {
            SetPatternFill(p, color, 0);
        }
    }

    /// <summary>
    ///     Sets the fill color to an uncolored pattern.
    /// </summary>
    /// <param name="p">the pattern</param>
    /// <param name="color">the color of the pattern</param>
    /// <param name="tint">the tint if the color is a spot color, ignored otherwise</param>
    public virtual void SetPatternFill(PdfPatternPainter p, BaseColor color, float tint)
    {
        if (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        if (color == null)
        {
            throw new ArgumentNullException(nameof(color));
        }

        CheckWriter();

        if (!p.IsStencil())
        {
            throw new InvalidOperationException("An uncolored pattern was expected.");
        }

        var prs = PageResources;
        var name = Writer.AddSimplePattern(p);
        name = prs.AddPattern(name, p.IndirectReference);
        var csDetail = Writer.AddSimplePatternColorspace(color);
        var cName = prs.AddColor(csDetail.ColorName, csDetail.IndirectReference);
        Content.Append(cName.GetBytes()).Append(" cs").Append_i(Separator);
        OutputColorNumbers(color, tint);
        Content.Append(' ').Append(name.GetBytes()).Append(" scn").Append_i(Separator);
    }

    /// <summary>
    ///     Sets the stroke color to an uncolored pattern.
    /// </summary>
    /// <param name="p">the pattern</param>
    /// <param name="color">the color of the pattern</param>
    public virtual void SetPatternStroke(PdfPatternPainter p, BaseColor color)
    {
        if (color == null)
        {
            throw new ArgumentNullException(nameof(color));
        }

        if (ExtendedColor.GetType(color) == ExtendedColor.TYPE_SEPARATION)
        {
            SetPatternStroke(p, color, ((SpotColor)color).Tint);
        }
        else
        {
            SetPatternStroke(p, color, 0);
        }
    }

    /// <summary>
    ///     Sets the stroke color to an uncolored pattern.
    /// </summary>
    /// <param name="p">the pattern</param>
    /// <param name="color">the color of the pattern</param>
    /// <param name="tint">the tint if the color is a spot color, ignored otherwise</param>
    public virtual void SetPatternStroke(PdfPatternPainter p, BaseColor color, float tint)
    {
        if (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        if (color == null)
        {
            throw new ArgumentNullException(nameof(color));
        }

        CheckWriter();

        if (!p.IsStencil())
        {
            throw new InvalidOperationException("An uncolored pattern was expected.");
        }

        var prs = PageResources;
        var name = Writer.AddSimplePattern(p);
        name = prs.AddPattern(name, p.IndirectReference);
        var csDetail = Writer.AddSimplePatternColorspace(color);
        var cName = prs.AddColor(csDetail.ColorName, csDetail.IndirectReference);
        Content.Append(cName.GetBytes()).Append(" CS").Append_i(Separator);
        OutputColorNumbers(color, tint);
        Content.Append(' ').Append(name.GetBytes()).Append(" SCN").Append_i(Separator);
    }

    /// <summary>
    ///     Sets the stroke color to a pattern. The pattern can be
    ///     colored or uncolored.
    /// </summary>
    /// <param name="p">the pattern</param>
    public virtual void SetPatternStroke(PdfPatternPainter p)
    {
        if (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        if (p.IsStencil())
        {
            SetPatternStroke(p, p.DefaultColor);

            return;
        }

        CheckWriter();
        var prs = PageResources;
        var name = Writer.AddSimplePattern(p);
        name = prs.AddPattern(name, p.IndirectReference);

        Content.Append(PdfName.Pattern.GetBytes()).Append(" CS ").Append(name.GetBytes()).Append(" SCN")
            .Append_i(Separator);
    }

    public virtual void SetRgbColorFill(int red, int green, int blue)
    {
        helperRgb((float)(red & 0xFF) / 0xFF, (float)(green & 0xFF) / 0xFF, (float)(blue & 0xFF) / 0xFF);
        Content.Append(" rg").Append_i(Separator);
    }

    public virtual void SetRgbColorFillF(float red, float green, float blue)
    {
        helperRgb(red, green, blue);
        Content.Append(" rg").Append_i(Separator);
    }

    public virtual void SetRgbColorStroke(int red, int green, int blue)
    {
        helperRgb((float)(red & 0xFF) / 0xFF, (float)(green & 0xFF) / 0xFF, (float)(blue & 0xFF) / 0xFF);
        Content.Append(" RG").Append_i(Separator);
    }

    public virtual void SetRgbColorStrokeF(float red, float green, float blue)
    {
        helperRgb(red, green, blue);
        Content.Append(" RG").Append_i(Separator);
    }

    /// <summary>
    ///     Sets the shading fill pattern.
    /// </summary>
    /// <param name="shading">the shading pattern</param>
    public virtual void SetShadingFill(PdfShadingPattern shading)
    {
        if (shading == null)
        {
            throw new ArgumentNullException(nameof(shading));
        }

        Writer.AddSimpleShadingPattern(shading);
        var prs = PageResources;
        var name = prs.AddPattern(shading.PatternName, shading.PatternReference);

        Content.Append(PdfName.Pattern.GetBytes()).Append(" cs ").Append(name.GetBytes()).Append(" scn")
            .Append_i(Separator);

        var details = shading.ColorDetails;

        if (details != null)
        {
            prs.AddColor(details.ColorName, details.IndirectReference);
        }
    }

    /// <summary>
    ///     Sets the shading stroke pattern
    /// </summary>
    /// <param name="shading">the shading pattern</param>
    public virtual void SetShadingStroke(PdfShadingPattern shading)
    {
        if (shading == null)
        {
            throw new ArgumentNullException(nameof(shading));
        }

        Writer.AddSimpleShadingPattern(shading);
        var prs = PageResources;
        var name = prs.AddPattern(shading.PatternName, shading.PatternReference);

        Content.Append(PdfName.Pattern.GetBytes()).Append(" CS ").Append(name.GetBytes()).Append(" SCN")
            .Append_i(Separator);

        var details = shading.ColorDetails;

        if (details != null)
        {
            prs.AddColor(details.ColorName, details.IndirectReference);
        }
    }

    /// <summary>
    ///     Changes the text matrix.
    ///     Remark: this operation also initializes the current point position.
    /// </summary>
    /// <param name="a">operand 1,1 in the matrix</param>
    /// <param name="b">operand 1,2 in the matrix</param>
    /// <param name="c">operand 2,1 in the matrix</param>
    /// <param name="d">operand 2,2 in the matrix</param>
    /// <param name="x">operand 3,1 in the matrix</param>
    /// <param name="y">operand 3,2 in the matrix</param>
    public void SetTextMatrix(float a, float b, float c, float d, float x, float y)
    {
        State.XTlm = x;
        State.YTlm = y;

        Content.Append(a).Append(' ').Append(b).Append_i(' ').Append(c).Append_i(' ').Append(d).Append_i(' ').Append(x)
            .Append_i(' ').Append(y).Append(" Tm").Append_i(Separator);
    }

    /// <summary>
    ///     Changes the text matrix. The first four parameters are {1,0,0,1}.
    ///     Remark: this operation also initializes the current point position.
    /// </summary>
    /// <param name="x">operand 3,1 in the matrix</param>
    /// <param name="y">operand 3,2 in the matrix</param>
    public void SetTextMatrix(float x, float y)
        => SetTextMatrix(1, 0, 0, 1, x, y);

    /// <summary>
    ///     Sets the text rendering parameter.
    /// </summary>
    /// <param name="value">a parameter</param>
    public void SetTextRenderingMode(int value)
        => Content.Append(value).Append(" Tr").Append_i(Separator);

    /// <summary>
    ///     Sets the text rise parameter.
    ///     This allows to write text in subscript or basescript mode.
    /// </summary>
    /// <param name="value">a parameter</param>
    public void SetTextRise(float value)
        => Content.Append(value).Append(" Ts").Append_i(Separator);

    /// <summary>
    ///     Sets the word spacing parameter.
    /// </summary>
    public void SetWordSpacing(float value)
    {
        State.WordSpace = value;
        Content.Append(value).Append(" Tw").Append_i(Separator);
    }

    /// <summary>
    ///     Shows the  text .
    /// </summary>
    /// <param name="text">the text to write</param>
    public void ShowText(string text)
    {
        showText2(text);
        Content.Append("Tj").Append_i(Separator);
    }

    /// <summary>
    ///     Show an array of text.
    /// </summary>
    /// <param name="text">array of text</param>
    public void ShowText(PdfTextArray text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (State.FontDetails == null)
        {
            throw new InvalidOperationException("Font and size must be set before writing any text");
        }

        Content.Append('[');
        var arrayList = text.ArrayList;
        var lastWasNumber = false;

        for (var k = 0; k < arrayList.Count; ++k)
        {
            var obj = arrayList[k];

            if (obj is string)
            {
                showText2((string)obj);
                lastWasNumber = false;
            }
            else
            {
                if (lastWasNumber)
                {
                    Content.Append(' ');
                }
                else
                {
                    lastWasNumber = true;
                }

                Content.Append((float)obj);
            }
        }

        Content.Append("]TJ").Append_i(Separator);
    }

    /// <summary>
    ///     Shows text right, left or center aligned with rotation.
    /// </summary>
    /// <param name="alignment">the alignment can be ALIGN_CENTER, ALIGN_RIGHT or ALIGN_LEFT</param>
    /// <param name="text">the text to show</param>
    /// <param name="x">the x pivot position</param>
    /// <param name="y">the y pivot position</param>
    /// <param name="rotation">the rotation to be applied in degrees counterclockwise</param>
    public void ShowTextAligned(int alignment, string text, float x, float y, float rotation)
        => showTextAligned(alignment, text, x, y, rotation, false);

    /// <summary>
    ///     Shows text kerned right, left or center aligned with rotation.
    /// </summary>
    /// <param name="alignment">the alignment can be ALIGN_CENTER, ALIGN_RIGHT or ALIGN_LEFT</param>
    /// <param name="text">the text to show</param>
    /// <param name="x">the x pivot position</param>
    /// <param name="y">the y pivot position</param>
    /// <param name="rotation">the rotation to be applied in degrees counterclockwise</param>
    public void ShowTextAlignedKerned(int alignment, string text, float x, float y, float rotation)
        => showTextAligned(alignment, text, x, y, rotation, true);

    /// <summary>
    ///     Shows the  text  kerned.
    /// </summary>
    /// <param name="text">the text to write</param>
    public void ShowTextKerned(string text)
    {
        if (State.FontDetails == null)
        {
            throw new InvalidOperationException("Font and size must be set before writing any text");
        }

        var bf = State.FontDetails.BaseFont;

        if (bf.HasKernPairs())
        {
            ShowText(GetKernArray(text, bf));
        }
        else
        {
            ShowText(text);
        }
    }

    public void Stroke()
        => Content.Append('S').Append_i(Separator);

    public byte[] ToPdf(PdfWriter writer)
    {
        SanityCheck();

        return Content.ToByteArray();
    }

    public override string ToString()
        => Content.ToString();

    /// <summary>
    ///     Adds a variable width border to the current path.
    ///     Only use if {@link com.lowagie.text.Rectangle#isUseVariableBorders() Rectangle.isUseVariableBorders}
    ///     = true.
    /// </summary>
    /// <param name="rect">a  Rectangle </param>
    public void VariableRectangle(Rectangle rect)
    {
        if (rect == null)
        {
            throw new ArgumentNullException(nameof(rect));
        }

        var t = rect.Top;
        var b = rect.Bottom;
        var r = rect.Right;
        var l = rect.Left;
        var wt = rect.BorderWidthTop;
        var wb = rect.BorderWidthBottom;
        var wr = rect.BorderWidthRight;
        var wl = rect.BorderWidthLeft;
        var ct = rect.BorderColorTop;
        var cb = rect.BorderColorBottom;
        var cr = rect.BorderColorRight;
        var cl = rect.BorderColorLeft;
        SaveState();
        SetLineCap(LINE_CAP_BUTT);
        SetLineJoin(LINE_JOIN_MITER);
        float clw = 0;
        var cdef = false;
        BaseColor ccol = null;
        var cdefi = false;
        BaseColor cfil = null;

        // draw top
        if (wt > 0)
        {
            SetLineWidth(clw = wt);
            cdef = true;

            if (ct == null)
            {
                ResetRgbColorStroke();
            }
            else
            {
                SetColorStroke(ct);
            }

            ccol = ct;
            MoveTo(l, t - wt / 2f);
            LineTo(r, t - wt / 2f);
            Stroke();
        }

        // Draw bottom
        if (wb > 0)
        {
            if (wb.ApproxNotEqual(clw))
            {
                SetLineWidth(clw = wb);
            }

            if (!cdef || !compareColors(ccol, cb))
            {
                cdef = true;

                if (cb == null)
                {
                    ResetRgbColorStroke();
                }
                else
                {
                    SetColorStroke(cb);
                }

                ccol = cb;
            }

            MoveTo(r, b + wb / 2f);
            LineTo(l, b + wb / 2f);
            Stroke();
        }

        // Draw right
        if (wr > 0)
        {
            if (wr.ApproxNotEqual(clw))
            {
                SetLineWidth(clw = wr);
            }

            if (!cdef || !compareColors(ccol, cr))
            {
                cdef = true;

                if (cr == null)
                {
                    ResetRgbColorStroke();
                }
                else
                {
                    SetColorStroke(cr);
                }

                ccol = cr;
            }

            var bt = compareColors(ct, cr);
            var bb = compareColors(cb, cr);
            MoveTo(r - wr / 2f, bt ? t : t - wt);
            LineTo(r - wr / 2f, bb ? b : b + wb);
            Stroke();

            if (!bt || !bb)
            {
                cdefi = true;

                if (cr == null)
                {
                    ResetRgbColorFill();
                }
                else
                {
                    SetColorFill(cr);
                }

                cfil = cr;

                if (!bt)
                {
                    MoveTo(r, t);
                    LineTo(r, t - wt);
                    LineTo(r - wr, t - wt);
                    Fill();
                }

                if (!bb)
                {
                    MoveTo(r, b);
                    LineTo(r, b + wb);
                    LineTo(r - wr, b + wb);
                    Fill();
                }
            }
        }

        // Draw Left
        if (wl > 0)
        {
            if (wl.ApproxNotEqual(clw))
            {
                SetLineWidth(wl);
            }

            if (!cdef || !compareColors(ccol, cl))
            {
                if (cl == null)
                {
                    ResetRgbColorStroke();
                }
                else
                {
                    SetColorStroke(cl);
                }
            }

            var bt = compareColors(ct, cl);
            var bb = compareColors(cb, cl);
            MoveTo(l + wl / 2f, bt ? t : t - wt);
            LineTo(l + wl / 2f, bb ? b : b + wb);
            Stroke();

            if (!bt || !bb)
            {
                if (!cdefi || !compareColors(cfil, cl))
                {
                    if (cl == null)
                    {
                        ResetRgbColorFill();
                    }
                    else
                    {
                        SetColorFill(cl);
                    }
                }

                if (!bt)
                {
                    MoveTo(l, t);
                    LineTo(l, t - wt);
                    LineTo(l + wl, t - wt);
                    Fill();
                }

                if (!bb)
                {
                    MoveTo(l, b);
                    LineTo(l, b + wb);
                    LineTo(l + wl, b + wb);
                    Fill();
                }
            }
        }

        RestoreState();
    }

    /// <summary>
    ///     Escapes a  byte  array according to the PDF conventions.
    /// </summary>
    /// <param name="b">the  byte  array to escape</param>
    /// <returns>an escaped  byte  array</returns>
    internal static byte[] EscapeString(byte[] b)
    {
        var content = new ByteBuffer();
        EscapeString(b, content);

        return content.ToByteArray();
    }

    /// <summary>
    ///     Escapes a  byte  array according to the PDF conventions.
    /// </summary>
    /// <param name="b">the  byte  array to escape</param>
    /// <param name="content"></param>
    internal static void EscapeString(byte[] b, ByteBuffer content)
    {
        content.Append_i('(');

        for (var k = 0; k < b.Length; ++k)
        {
            var c = b[k];

            switch ((int)c)
            {
                case '\r':
                    content.Append("\\r");

                    break;
                case '\n':
                    content.Append("\\n");

                    break;
                case '\t':
                    content.Append("\\t");

                    break;
                case '\b':
                    content.Append("\\b");

                    break;
                case '\f':
                    content.Append("\\f");

                    break;
                case '(':
                case ')':
                case '\\':
                    content.Append_i('\\').Append_i(c);

                    break;
                default:
                    content.Append_i(c);

                    break;
            }
        }

        content.Append(')');
    }

    internal virtual void AddAnnotation(PdfAnnotation annot)
        => Writer.AddAnnotation(annot);

    internal void AddTemplateReference(PdfIndirectReference template,
        PdfName name,
        float a,
        float b,
        float c,
        float d,
        float e,
        float f)
    {
        CheckWriter();
        var prs = PageResources;
        name = prs.AddXObject(name, template);
        Content.Append("q ");
        Content.Append(a).Append(' ');
        Content.Append(b).Append(' ');
        Content.Append(c).Append(' ');
        Content.Append(d).Append(' ');
        Content.Append(e).Append(' ');
        Content.Append(f).Append(" cm ");
        Content.Append(name.GetBytes()).Append(" Do Q").Append_i(Separator);
    }

    /// <summary>
    ///     Throws an error if it is a pattern.
    /// </summary>
    /// <param name="t">the object to check</param>
    internal static void CheckNoPattern(PdfTemplate t)
    {
        if (t.Type == PdfTemplate.TYPE_PATTERN)
        {
            throw new ArgumentException("Invalid use of a pattern. A template was expected.");
        }
    }

    internal PdfAppearance CreateAppearance(float width, float height, PdfName forcedName)
    {
        CheckWriter();
        var template = new PdfAppearance(Writer);
        template.Width = width;
        template.Height = height;
        Writer.AddDirectTemplateSimple(template, forcedName);

        return template;
    }

    internal PdfTemplate CreateTemplate(float width, float height, PdfName forcedName)
    {
        CheckWriter();
        var template = new PdfTemplate(Writer);
        template.Width = width;
        template.Height = height;
        Writer.AddDirectTemplateSimple(template, forcedName);

        return template;
    }

    /// <summary>
    ///     Outputs the color values to the content.
    /// </summary>
    /// <param name="color">The color</param>
    /// <param name="tint">the tint if it is a spot color, ignored otherwise</param>
    internal void OutputColorNumbers(BaseColor color, float tint)
    {
        PdfXConformanceImp.CheckPdfxConformance(Writer, PdfXConformanceImp.PDFXKEY_COLOR, color);
        var type = ExtendedColor.GetType(color);

        switch (type)
        {
            case ExtendedColor.TYPE_RGB:
                Content.Append((float)color.R / 0xFF);
                Content.Append(' ');
                Content.Append((float)color.G / 0xFF);
                Content.Append(' ');
                Content.Append((float)color.B / 0xFF);

                break;
            case ExtendedColor.TYPE_GRAY:
                Content.Append(((GrayColor)color).Gray);

                break;
            case ExtendedColor.TYPE_CMYK:
            {
                var cmyk = (CmykColor)color;
                Content.Append(cmyk.Cyan).Append(' ').Append(cmyk.Magenta);
                Content.Append(' ').Append(cmyk.Yellow).Append(' ').Append(cmyk.Black);

                break;
            }
            case ExtendedColor.TYPE_SEPARATION:
                Content.Append(tint);

                break;
            default:
                throw new InvalidOperationException("Invalid color type.");
        }
    }

    /// <summary>
    ///     Check if we have a valid PdfWriter.
    /// </summary>
    protected virtual void CheckWriter()
    {
        if (Writer == null)
        {
            throw new ArgumentNullException("The writer in PdfContentByte is null.");
        }
    }

    private void beginLayer2(IPdfOcg layer)
    {
        var name = (PdfName)Writer.AddSimpleProperty(layer, layer.Ref)[0];
        var prs = PageResources;
        name = prs.AddProperty(name, layer.Ref);
        Content.Append("/OC ").Append(name.GetBytes()).Append(" BDC").Append_i(Separator);
    }

    private static bool compareColors(BaseColor c1, BaseColor c2)
    {
        if (c1 == null && c2 == null)
        {
            return true;
        }

        if (c1 == null || c2 == null)
        {
            return false;
        }

        if (c1 is ExtendedColor)
        {
            return c1.Equals(c2);
        }

        return c2.Equals(c1);
    }

    /// <summary>
    ///     Helper to validate and write the CMYK color components.
    /// </summary>
    /// <param name="cyan">the intensity of cyan. A value between 0 and 1</param>
    /// <param name="magenta">the intensity of magenta. A value between 0 and 1</param>
    /// <param name="yellow">the intensity of yellow. A value between 0 and 1</param>
    /// <param name="black">the intensity of black. A value between 0 and 1</param>
    private void helperCmyk(float cyan, float magenta, float yellow, float black)
    {
        if (cyan < 0)
        {
            cyan = 0.0f;
        }
        else if (cyan > 1.0f)
        {
            cyan = 1.0f;
        }

        if (magenta < 0)
        {
            magenta = 0.0f;
        }
        else if (magenta > 1.0f)
        {
            magenta = 1.0f;
        }

        if (yellow < 0)
        {
            yellow = 0.0f;
        }
        else if (yellow > 1.0f)
        {
            yellow = 1.0f;
        }

        if (black < 0)
        {
            black = 0.0f;
        }
        else if (black > 1.0f)
        {
            black = 1.0f;
        }

        Content.Append(cyan).Append(' ').Append(magenta).Append(' ').Append(yellow).Append(' ').Append(black);
    }

    /// <summary>
    ///     Helper to validate and write the RGB color components
    /// </summary>
    /// <param name="red">the intensity of red. A value between 0 and 1</param>
    /// <param name="green">the intensity of green. A value between 0 and 1</param>
    /// <param name="blue">the intensity of blue. A value between 0 and 1</param>
    private void helperRgb(float red, float green, float blue)
    {
        PdfXConformanceImp.CheckPdfxConformance(Writer, PdfXConformanceImp.PDFXKEY_RGB, null);

        if (red < 0)
        {
            red = 0.0f;
        }
        else if (red > 1.0f)
        {
            red = 1.0f;
        }

        if (green < 0)
        {
            green = 0.0f;
        }
        else if (green > 1.0f)
        {
            green = 1.0f;
        }

        if (blue < 0)
        {
            blue = 0.0f;
        }
        else if (blue > 1.0f)
        {
            blue = 1.0f;
        }

        Content.Append(red).Append(' ').Append(green).Append(' ').Append(blue);
    }

    /// <summary>
    ///     A helper to insert into the content stream the  text
    ///     converted to bytes according to the font's encoding.
    /// </summary>
    /// <param name="text">the text to write</param>
    private void showText2(string text)
    {
        if (State.FontDetails == null)
        {
            throw new InvalidOperationException("Font and size must be set before writing any text");
        }

        var b = State.FontDetails.ConvertToBytes(text);
        EscapeString(b, Content);
    }

    /// <summary>
    ///     constructors
    /// </summary>
    private void showTextAligned(int alignment, string text, float x, float y, float rotation, bool kerned)
    {
        if (State.FontDetails == null)
        {
            throw new InvalidOperationException("Font and size must be set before writing any text");
        }

        if (rotation.ApproxEquals(0))
        {
            switch (alignment)
            {
                case ALIGN_CENTER:
                    x -= GetEffectiveStringWidth(text, kerned) / 2;

                    break;
                case ALIGN_RIGHT:
                    x -= GetEffectiveStringWidth(text, kerned);

                    break;
            }

            SetTextMatrix(x, y);

            if (kerned)
            {
                ShowTextKerned(text);
            }
            else
            {
                ShowText(text);
            }
        }
        else
        {
            var alpha = rotation * Math.PI / 180.0;
            var cos = (float)Math.Cos(alpha);
            var sin = (float)Math.Sin(alpha);
            float len;

            switch (alignment)
            {
                case ALIGN_CENTER:
                    len = GetEffectiveStringWidth(text, kerned) / 2;
                    x -= len * cos;
                    y -= len * sin;

                    break;
                case ALIGN_RIGHT:
                    len = GetEffectiveStringWidth(text, kerned);
                    x -= len * cos;
                    y -= len * sin;

                    break;
            }

            SetTextMatrix(cos, sin, -sin, cos, x, y);

            if (kerned)
            {
                ShowTextKerned(text);
            }
            else
            {
                ShowText(text);
            }

            SetTextMatrix(0f, 0f);
        }
    }

    /// <summary>
    ///     This class keeps the graphic state of the current page
    /// </summary>
    public class GraphicState
    {
        /// <summary>
        ///     The current character spacing
        /// </summary>
        protected internal float CharSpace;

        /// <summary>
        ///     This is the color in use
        /// </summary>
        internal ColorDetails ColorDetails;

        /// <summary>
        ///     This is the font in use
        /// </summary>
        internal FontDetails FontDetails;

        /// <summary>
        ///     The current text leading.
        /// </summary>
        protected internal float leading;

        /// <summary>
        ///     The current horizontal scaling
        /// </summary>
        protected internal float Scale = 100;

        /// <summary>
        ///     This is the font size in use
        /// </summary>
        internal float size;

        /// <summary>
        ///     The current word spacing
        /// </summary>
        protected internal float WordSpace;

        /// <summary>
        ///     The x position of the text line matrix.
        /// </summary>
        protected internal float XTlm;

        /// <summary>
        ///     The y position of the text line matrix.
        /// </summary>
        protected internal float YTlm;

        internal GraphicState()
        {
        }

        internal GraphicState(GraphicState cp)
        {
            FontDetails = cp.FontDetails;
            ColorDetails = cp.ColorDetails;
            size = cp.size;
            XTlm = cp.XTlm;
            YTlm = cp.YTlm;
            leading = cp.leading;
            Scale = cp.Scale;
            CharSpace = cp.CharSpace;
            WordSpace = cp.WordSpace;
        }
    }
}