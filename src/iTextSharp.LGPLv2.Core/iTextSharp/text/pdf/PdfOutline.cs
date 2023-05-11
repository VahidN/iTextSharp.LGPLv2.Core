using System.Text;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfOutline  is an object that represents a PDF outline entry.
///     An outline allows a user to access views of a document by name.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 6.7 (page 104-106)
///     @see     PdfDictionary
/// </summary>
public class PdfOutline : PdfDictionary
{
    /// <summary>
    ///     The  PdfAction  for this outline.
    /// </summary>
    private readonly PdfAction _action;

    /// <summary>
    ///     value of the <B>Destination</B>-key
    /// </summary>
    private readonly PdfDestination _destination;

    /// <summary>
    ///     Holds value of property color.
    /// </summary>
    private BaseColor _color;

    /// <summary>
    ///     Holds value of property open.
    /// </summary>
    private bool _open;

    /// <summary>
    ///     Holds value of property tag.
    /// </summary>
    private string _tag;

    /// <summary>
    ///     membervariables
    /// </summary>
    protected IList<PdfOutline> kids = new List<PdfOutline>();

    protected PdfWriter Writer;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfOutline .
    ///     This is the constructor for the  outlines object .
    /// </summary>
    public PdfOutline(PdfOutline parent, PdfAction action, string title) : this(parent, action, title, true)
    {
    }

    /// <summary>
    ///     Constructs a  PdfOutline .
    ///     This is the constructor for an  outline entry .
    /// </summary>
    /// <param name="parent">the parent of this outline item</param>
    /// <param name="action">the  PdfAction  for this outline item</param>
    /// <param name="title">the title of this outline item</param>
    /// <param name="open"> true  if the children are visible</param>
    public PdfOutline(PdfOutline parent, PdfAction action, string title, bool open)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        _action = action;
        InitOutline(parent, title, open);
    }

    public PdfOutline(PdfOutline parent, PdfDestination destination, string title) : this(parent, destination, title,
     true)
    {
    }

    /// <summary>
    ///     Constructs a  PdfOutline .
    ///     This is the constructor for an  outline entry .
    /// </summary>
    /// <param name="parent">the parent of this outline item</param>
    /// <param name="destination">the destination for this outline item</param>
    /// <param name="title">the title of this outline item</param>
    /// <param name="open"> true  if the children are visible</param>
    public PdfOutline(PdfOutline parent, PdfDestination destination, string title, bool open)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        _destination = destination;
        InitOutline(parent, title, open);
    }

    /// <summary>
    ///     Constructs a  PdfOutline .
    ///     This is the constructor for an  outline entry . The open mode is
    ///     true .
    /// </summary>
    /// <param name="parent">the parent of this outline item</param>
    /// <param name="action">the  PdfAction  for this outline item</param>
    /// <param name="title">the title of this outline item</param>
    public PdfOutline(PdfOutline parent, PdfAction action, PdfString title) : this(parent, action, title, true)
    {
    }

    /// <summary>
    ///     Constructs a  PdfOutline .
    ///     This is the constructor for an  outline entry .
    /// </summary>
    /// <param name="parent">the parent of this outline item</param>
    /// <param name="action">the  PdfAction  for this outline item</param>
    /// <param name="title">the title of this outline item</param>
    /// <param name="open"> true  if the children are visible</param>
    public PdfOutline(PdfOutline parent, PdfAction action, PdfString title, bool open) : this(parent,
                                                                                              action,
                                                                                              title?.ToString() ?? throw new ArgumentNullException(nameof(title)),
                                                                                              open)
    {
    }

    public PdfOutline(PdfOutline parent, PdfDestination destination, PdfString title) : this(parent, destination, title,
     true)
    {
    }

    /// <summary>
    ///     Constructs a  PdfOutline .
    ///     This is the constructor for an  outline entry .
    /// </summary>
    /// <param name="parent">the parent of this outline item</param>
    /// <param name="destination">the destination for this outline item</param>
    /// <param name="title">the title of this outline item</param>
    /// <param name="open"> true  if the children are visible</param>
    public PdfOutline(PdfOutline parent, PdfDestination destination, PdfString title, bool open) :
        this(parent, destination, title?.ToString() ?? throw new ArgumentNullException(nameof(title)), true)
    {
    }

    public PdfOutline(PdfOutline parent, PdfAction action, Paragraph title) : this(parent, action, title, true)
    {
    }

    /// <summary>
    ///     Constructs a  PdfOutline .
    ///     This is the constructor for an  outline entry .
    /// </summary>
    /// <param name="parent">the parent of this outline item</param>
    /// <param name="action">the  PdfAction  for this outline item</param>
    /// <param name="title">the title of this outline item</param>
    /// <param name="open"> true  if the children are visible</param>
    public PdfOutline(PdfOutline parent, PdfAction action, Paragraph title, bool open)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        if (title == null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        var buf = new StringBuilder();
        foreach (var chunk in title.Chunks)
        {
            buf.Append(chunk.Content);
        }

        _action = action;
        InitOutline(parent, buf.ToString(), open);
    }

    public PdfOutline(PdfOutline parent, PdfDestination destination, Paragraph title) : this(parent, destination, title,
     true)
    {
    }

    /// <summary>
    ///     Constructs a  PdfOutline .
    ///     This is the constructor for an  outline entry .
    /// </summary>
    /// <param name="parent">the parent of this outline item</param>
    /// <param name="destination">the destination for this outline item</param>
    /// <param name="title">the title of this outline item</param>
    /// <param name="open"> true  if the children are visible</param>
    public PdfOutline(PdfOutline parent, PdfDestination destination, Paragraph title, bool open)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        if (title == null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        var buf = new StringBuilder();
        foreach (var chunk in title.Chunks)
        {
            buf.Append(chunk.Content);
        }

        _destination = destination;
        InitOutline(parent, buf.ToString(), open);
    }

    internal PdfOutline(PdfWriter writer) : base(Outlines)
    {
        _open = true;
        Parent = null;
        Writer = writer;
    }

    /// <summary>
    ///     methods
    /// </summary>

    public BaseColor Color
    {
        get => _color;
        set => _color = value;
    }

    /// <summary>
    ///     the  PdfIndirectReference  of this object
    /// </summary>
    public PdfIndirectReference IndirectReference { get; set; }

    public IList<PdfOutline> Kids
    {
        get => kids;

        set => kids = value;
    }

    public int Level
    {
        get
        {
            if (Parent == null)
            {
                return 0;
            }

            return Parent.Level + 1;
        }
    }

    /// <summary>
    ///     Setter for property open.
    /// </summary>
    public bool Open
    {
        set => _open = value;
        get => _open;
    }

    /// <summary>
    ///     value of the <B>Parent</B>-key
    /// </summary>
    public PdfOutline Parent { get; private set; }

    /// <summary>
    ///     Gets the destination for this outline.
    /// </summary>
    /// <returns>the destination</returns>
    public PdfDestination PdfDestination => _destination;

    /// <summary>
    ///     Holds value of property style.
    /// </summary>
    public int Style { get; set; }

    /// <summary>
    ///     Getter for property tag.
    /// </summary>
    /// <returns>Value of property tag.</returns>
    public string Tag
    {
        get => _tag;

        set => _tag = value;
    }

    public string Title
    {
        get
        {
            var title = (PdfString)Get(PdfName.Title);
            return title.ToString();
        }

        set => Put(PdfName.Title, new PdfString(value, TEXT_UNICODE));
    }

    /// <summary>
    ///     value of the <B>Count</B>-key
    /// </summary>
    internal int Count { get; set; }

    public void AddKid(PdfOutline outline)
    {
        kids.Add(outline);
    }

    public bool SetDestinationPage(PdfIndirectReference pageReference)
    {
        if (_destination == null)
        {
            return false;
        }

        return _destination.AddPage(pageReference);
    }

    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (_color != null && !_color.Equals(BaseColor.Black))
        {
            Put(PdfName.C, new PdfArray(new[] { _color.R / 255f, _color.G / 255f, _color.B / 255f }));
        }

        var flag = 0;
        if ((Style & text.Font.BOLD) != 0)
        {
            flag |= 2;
        }

        if ((Style & text.Font.ITALIC) != 0)
        {
            flag |= 1;
        }

        if (flag != 0)
        {
            Put(PdfName.F, new PdfNumber(flag));
        }

        if (Parent != null)
        {
            Put(PdfName.Parent, Parent.IndirectReference);
        }

        if (_destination != null && _destination.HasPage())
        {
            Put(PdfName.Dest, _destination);
        }

        if (_action != null)
        {
            Put(PdfName.A, _action);
        }

        if (Count != 0)
        {
            Put(PdfName.Count, new PdfNumber(Count));
        }

        base.ToPdf(writer, os);
    }

    /// <summary>
    ///     Helper for the constructors.
    /// </summary>
    /// <param name="parent">the parent outline</param>
    /// <param name="title">the title for this outline</param>
    /// <param name="open"> true  if the children are visible</param>
    internal void InitOutline(PdfOutline parent, string title, bool open)
    {
        _open = open;
        Parent = parent;
        Writer = parent.Writer;
        Put(PdfName.Title, new PdfString(title, TEXT_UNICODE));
        parent.AddKid(this);
        if (_destination != null && !_destination.HasPage()) // bugfix Finn Bock
        {
            SetDestinationPage(Writer.CurrentPage);
        }
    }
}