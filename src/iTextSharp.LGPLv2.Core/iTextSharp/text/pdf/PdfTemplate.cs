namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the form XObject.
/// </summary>
public class PdfTemplate : PdfContentByte
{
    public const int TYPE_IMPORTED = 2;
    public const int TYPE_PATTERN = 3;
    public const int TYPE_TEMPLATE = 1;

    /// <summary>
    ///     The bounding box of this template
    /// </summary>
    protected Rectangle BBox = new(0, 0);

    protected PdfTransparencyGroup group;
    protected IPdfOcg layer;
    protected PdfArray matrix;

    /// <summary>
    ///     The resources used by this template
    /// </summary>
    protected PageResources pageResources;

    /// <summary>
    ///     The indirect reference to this template
    /// </summary>
    protected PdfIndirectReference ThisReference;

    protected int type;

    /// <summary>
    ///     Creates a  PdfTemplate .
    /// </summary>
    internal PdfTemplate(PdfWriter wr) : base(wr)
    {
        type = TYPE_TEMPLATE;
        pageResources = new PageResources();
        pageResources.AddDefaultColor(wr.DefaultColorspace);
        ThisReference = Writer.PdfIndirectReference;
    }

    protected PdfTemplate() : base(null) => type = TYPE_TEMPLATE;

    public Rectangle BoundingBox
    {
        get => BBox;
        set => BBox = value;
    }

    public override PdfContentByte Duplicate
    {
        get
        {
            var tpl = new PdfTemplate();
            tpl.Writer = Writer;
            tpl.Pdf = Pdf;
            tpl.ThisReference = ThisReference;
            tpl.pageResources = pageResources;
            tpl.BBox = new Rectangle(BBox);
            tpl.group = group;
            tpl.layer = layer;
            if (matrix != null)
            {
                tpl.matrix = new PdfArray(matrix);
            }

            tpl.Separator = Separator;
            return tpl;
        }
    }

    public virtual PdfTransparencyGroup Group
    {
        get => group;
        set => group = value;
    }

    public float Height
    {
        get => BBox.Height;

        set
        {
            BBox.Bottom = 0;
            BBox.Top = value;
        }
    }

    public PdfIndirectReference IndirectReference
    {
        get
        {
            // uncomment the null check as soon as we're sure all examples still work
            if (ThisReference == null /* && writer != null */)
            {
                ThisReference = Writer.PdfIndirectReference;
            }

            return ThisReference;
        }
    }

    /// <summary>
    ///     Gets the bounding heigth of this template.
    /// </summary>
    /// <returns>heigth the bounding height</returns>
    /// <summary>
    ///     Gets the layer this template belongs to.
    /// </summary>
    /// <returns>the layer this template belongs to or  null  for no layer defined</returns>
    public IPdfOcg Layer
    {
        get => layer;
        set => layer = value;
    }

    /// <summary>
    ///     Gets a duplicate of this  PdfTemplate . All
    ///     the members are copied by reference but the buffer stays different.
    /// </summary>
    /// <returns>a copy of this  PdfTemplate </returns>
    public int Type => type;

    /// <summary>
    ///     Gets the bounding width of this template.
    /// </summary>
    /// <returns>width the bounding width</returns>
    public float Width
    {
        get => BBox.Width;

        set
        {
            BBox.Left = 0;
            BBox.Right = value;
        }
    }

    internal PdfArray Matrix => matrix;

    internal override PageResources PageResources => pageResources;

    internal virtual PdfObject Resources => PageResources.Resources;

    /// <summary>
    ///     Creates a new template.
    ///     Creates a new template that is nothing more than a form XObject. This template can be included
    ///     in this  PdfContentByte  or in another template. Templates are only written
    ///     to the output when the document is closed permitting things like showing text in the first page
    ///     that is only defined in the last page.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="width">the bounding box width</param>
    /// <param name="height">the bounding box height</param>
    /// <returns>the templated created</returns>
    public static PdfTemplate CreateTemplate(PdfWriter writer, float width, float height)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        return CreateTemplate(writer, width, height, null);
    }

    /// <summary>
    ///     Gets the indirect reference to this template.
    /// </summary>
    /// <returns>the indirect reference to this template</returns>
    public void BeginVariableText()
    {
        Content.Append("/Tx BMC ");
    }

    public void EndVariableText()
    {
        Content.Append("EMC ");
    }

    public void SetMatrix(float a, float b, float c, float d, float e, float f)
    {
        matrix = new PdfArray();
        matrix.Add(new PdfNumber(a));
        matrix.Add(new PdfNumber(b));
        matrix.Add(new PdfNumber(c));
        matrix.Add(new PdfNumber(d));
        matrix.Add(new PdfNumber(e));
        matrix.Add(new PdfNumber(f));
    }

    internal static PdfTemplate CreateTemplate(PdfWriter writer, float width, float height, PdfName forcedName)
    {
        var template = new PdfTemplate(writer);
        template.Width = width;
        template.Height = height;
        writer.AddDirectTemplateSimple(template, forcedName);
        return template;
    }

    /// <summary>
    ///     Constructs the resources used by this template.
    /// </summary>
    /// <returns>the resources used by this template</returns>
    /// <summary>
    ///     Gets the stream representing this template.
    ///     @since   2.1.3   (replacing the method without param compressionLevel)
    /// </summary>
    /// <param name="compressionLevel">the compressionLevel</param>
    /// <returns>the stream representing this template</returns>
    internal virtual PdfStream GetFormXObject(int compressionLevel) => new PdfFormXObject(this, compressionLevel);
}