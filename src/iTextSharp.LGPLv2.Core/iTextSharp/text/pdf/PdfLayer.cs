namespace iTextSharp.text.pdf;

/// <summary>
///     An optional content group is a dictionary representing a collection of graphics
///     that can be made visible or invisible dynamically by users of viewer applications.
///     In iText they are referenced as layers.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfLayer : PdfDictionary, IPdfOcg
{
    /// <summary>
    ///     Holds value of property on.
    /// </summary>
    private bool _on = true;

    /// <summary>
    ///     Holds value of property onPanel.
    /// </summary>
    private bool _onPanel = true;

    protected IList<PdfLayer> children;
    protected PdfLayer parent;
    protected PdfIndirectReference Refi;
    protected string title;

    /// <summary>
    ///     Creates a new layer.
    /// </summary>
    /// <param name="name">the name of the layer</param>
    /// <param name="writer">the writer</param>
    public PdfLayer(string name, PdfWriter writer) : base(PdfName.Ocg)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        Name = name;
        Refi = writer.PdfIndirectReference;
        writer.RegisterLayer(this);
    }

    internal PdfLayer(string title) => this.title = title;

    /// <summary>
    ///     Gets the children layers.
    /// </summary>
    /// <returns>the children layers or  null  if the layer has no children</returns>
    public IList<PdfLayer> Children => children;

    /// <summary>
    ///     Specifies the recommended state for content in this
    ///     group when the document (or part of it) is saved by a viewer application to a format
    ///     that does not support optional content (for example, an earlier version of
    ///     PDF or a raster image format).
    /// </summary>
    public bool Export
    {
        set
        {
            var usage = Usage;
            var dic = new PdfDictionary();
            dic.Put(PdfName.Exportstate, value ? PdfName.On : PdfName.OFF);
            usage.Put(PdfName.Export, dic);
        }
    }

    /// <summary>
    ///     Sets the name of this layer.
    /// </summary>
    public string Name
    {
        set => Put(PdfName.Name, new PdfString(value, TEXT_UNICODE));
    }

    /// <summary>
    ///     Gets the initial visibility of the layer.
    /// </summary>
    /// <returns>the initial visibility of the layer</returns>
    public bool On
    {
        get => _on;
        set => _on = value;
    }

    /// <summary>
    ///     Gets the layer visibility in Acrobat's layer panel
    ///     Sets the visibility of the layer in Acrobat's layer panel. If  false
    ///     the layer cannot be directly manipulated by the user. Note that any children layers will
    ///     also be absent from the panel.
    /// </summary>
    public bool OnPanel
    {
        get => _onPanel;
        set => _onPanel = value;
    }

    /// <summary>
    ///     Gets the parent layer.
    /// </summary>
    /// <returns>the parent layer or  null  if the layer has no parent</returns>
    public PdfLayer Parent => parent;

    /// <summary>
    ///     Indicates that the group should be set to that state when the
    ///     document is opened in a viewer application.
    /// </summary>
    public bool View
    {
        set
        {
            var usage = Usage;
            var dic = new PdfDictionary();
            dic.Put(PdfName.Viewstate, value ? PdfName.On : PdfName.OFF);
            usage.Put(PdfName.View, dic);
        }
    }

    internal string Title => title;

    private PdfDictionary Usage
    {
        get
        {
            var usage = (PdfDictionary)Get(PdfName.Usage);
            if (usage == null)
            {
                usage = new PdfDictionary();
                Put(PdfName.Usage, usage);
            }

            return usage;
        }
    }

    /// <summary>
    ///     Gets the dictionary representing the layer. It just returns  this .
    /// </summary>
    /// <returns>the dictionary representing the layer</returns>
    public PdfObject PdfObject => this;

    /// <summary>
    ///     Gets the  PdfIndirectReference  that represents this layer.
    /// </summary>
    /// <returns>the  PdfIndirectReference  that represents this layer</returns>
    public PdfIndirectReference Ref
    {
        get => Refi;
        set => Refi = value;
    }

    /// <summary>
    ///     Creates a title layer. A title layer is not really a layer but a collection of layers
    ///     under the same title heading.
    /// </summary>
    /// <param name="title">the title text</param>
    /// <param name="writer">the  PdfWriter </param>
    /// <returns>the title layer</returns>
    public static PdfLayer CreateTitle(string title, PdfWriter writer)
    {
        if (title == null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        var layer = new PdfLayer(title);
        writer.RegisterLayer(layer);
        return layer;
    }

    /// <summary>
    ///     Adds a child layer. Nested layers can only have one parent.
    /// </summary>
    /// <param name="child">the child layer</param>
    public void AddChild(PdfLayer child)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        if (child.parent != null)
        {
            throw new ArgumentException("The layer '" + ((PdfString)child.Get(PdfName.Name)).ToUnicodeString() +
                                        "' already has a parent.");
        }

        child.parent = this;
        if (children == null)
        {
            children = new List<PdfLayer>();
        }

        children.Add(child);
    }

    /// <summary>
    ///     Used by the creating application to store application-specific
    ///     data associated with this optional content group.
    ///     values include but are not limited to <B>Artwork</B>, for graphic-design or publishing
    ///     applications, and <B>Technical</B>, for technical designs such as building plans or
    ///     schematics
    /// </summary>
    /// <param name="creator">a text string specifying the application that created the group</param>
    /// <param name="subtype">a string defining the type of content controlled by the group. Suggested</param>
    public void SetCreatorInfo(string creator, string subtype)
    {
        var usage = Usage;
        var dic = new PdfDictionary();
        dic.Put(PdfName.Creator, new PdfString(creator, TEXT_UNICODE));
        dic.Put(PdfName.Subtype, new PdfName(subtype));
        usage.Put(PdfName.Creatorinfo, dic);
    }

    /// <summary>
    ///     Specifies the language of the content controlled by this
    ///     optional content group
    ///     (for example, <B>es-MX</B> represents Mexican Spanish)
    ///     match between the system language and the language strings in all usage dictionaries
    /// </summary>
    /// <param name="lang">a language string which specifies a language and possibly a locale</param>
    /// <param name="preferred">used by viewer applications when there is a partial match but no exact</param>
    public void SetLanguage(string lang, bool preferred)
    {
        var usage = Usage;
        var dic = new PdfDictionary();
        dic.Put(PdfName.Lang, new PdfString(lang, TEXT_UNICODE));
        if (preferred)
        {
            dic.Put(PdfName.Preferred, PdfName.On);
        }

        usage.Put(PdfName.Language, dic);
    }

    /// <summary>
    ///     Specifies that the content in this group is intended for
    ///     use in printing
    ///     for example, <B>Trapping</B>, <B>PrintersMarks</B> and <B>Watermark</B>
    ///     set to that state when the document is printed from a viewer application
    /// </summary>
    /// <param name="subtype">a name specifying the kind of content controlled by the group;</param>
    /// <param name="printstate">indicates that the group should be</param>
    public void SetPrint(string subtype, bool printstate)
    {
        var usage = Usage;
        var dic = new PdfDictionary();
        dic.Put(PdfName.Subtype, new PdfName(subtype));
        dic.Put(PdfName.Printstate, printstate ? PdfName.On : PdfName.OFF);
        usage.Put(PdfName.Print, dic);
    }

    /// <summary>
    ///     Specifies a range of magnifications at which the content
    ///     in this optional content group is best viewed.
    ///     should be ON. A negative value will set the default to 0
    ///     should be ON. A negative value will set the largest possible magnification supported by the
    ///     viewer application
    /// </summary>
    /// <param name="min">the minimum recommended magnification factors at which the group</param>
    /// <param name="max">the maximum recommended magnification factor at which the group</param>
    public void SetZoom(float min, float max)
    {
        if (min <= 0 && max < 0)
        {
            return;
        }

        var usage = Usage;
        var dic = new PdfDictionary();
        if (min > 0)
        {
            dic.Put(PdfName.MinLowerCase, new PdfNumber(min));
        }

        if (max >= 0)
        {
            dic.Put(PdfName.MaxLowerCase, new PdfNumber(max));
        }

        usage.Put(PdfName.Zoom, dic);
    }
}