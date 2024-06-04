namespace iTextSharp.text;

/// <summary>
///     Wrapper that allows to add properties to a Chapter/Section object.
///     Before iText 1.5 every 'basic building block' implemented the MarkupAttributes interface.
///     By setting attributes, you could add markup to the corresponding XML and/or HTML tag.
///     This functionality was hardly used by anyone, so it was removed, and replaced by
///     the MarkedObject functionality.
/// </summary>
public class MarkedSection : MarkedObject
{
    /// <summary>
    ///     This is the title of this section.
    /// </summary>
    protected MarkedObject title;

    /// <summary>
    ///     Creates a MarkedObject with a Section or Chapter object.
    /// </summary>
    /// <param name="section">the marked section</param>
    public MarkedSection(Section section)
    {
        if (section == null)
        {
            throw new ArgumentNullException(nameof(section));
        }

        if (section.Title != null)
        {
            title = new MarkedObject(section.Title);
            section.Title = null;
        }

        Element = section;
    }

    /// <summary>
    ///     Setter for property bookmarkOpen.
    ///     visible.
    /// </summary>
    public bool BookmarkOpen
    {
        set => ((Section)Element).BookmarkOpen = value;
    }

    /// <summary>
    ///     Sets the bookmark title. The bookmark title is the same as the section title but
    ///     can be changed with this method.
    /// </summary>
    public string BookmarkTitle
    {
        set => ((Section)Element).BookmarkTitle = value;
    }

    /// <summary>
    ///     Sets the indentation of the content of this  Section .
    /// </summary>
    public float Indentation
    {
        set => ((Section)Element).Indentation = value;
    }

    /// <summary>
    ///     Sets the indentation of this  Section  on the left side.
    /// </summary>
    public float IndentationLeft
    {
        set => ((Section)Element).IndentationLeft = value;
    }

    public float IndentationRight
    {
        set => ((Section)Element).IndentationRight = value;
    }

    /// <summary>
    ///     Sets the depth of the sectionnumbers that will be shown preceding the title.
    ///     If the numberdepth is 0, the sections will not be numbered. If the numberdepth
    ///     is 1, the section will be numbered with their own number. If the numberdepth is
    ///     higher (for instance x > 1), the numbers of x - 1 parents will be shown.
    /// </summary>
    public int NumberDepth
    {
        set => ((Section)Element).NumberDepth = value;
    }

    /// <summary>
    ///     Sets the title of this section.
    /// </summary>
    public MarkedObject Title
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Element is Paragraph)
            {
                title = value;
            }
        }
        get
        {
            var result = Section.ConstructTitle((Paragraph)title.Element, ((Section)Element).Numbers,
                                                ((Section)Element).NumberDepth, ((Section)Element).NumberStyle);
            var mo = new MarkedObject(result);
            mo.markupAttributes = title.MarkupAttributes;
            return mo;
        }
    }

    /// <summary>
    ///     Setter for property triggerNewPage.
    /// </summary>
    public bool TriggerNewPage
    {
        set => ((Section)Element).TriggerNewPage = value;
    }

    public void Add(int index, IElement o)
    {
        ((Section)Element).Add(index, o);
    }

    /// <summary>
    ///     Adds a  Paragraph ,  List ,  Table  or another  Section
    ///     to this  Section .
    ///     @throws  ClassCastException if the object is not a  Paragraph ,  List ,  Table  or  Section
    /// </summary>
    /// <param name="o">an object of type  Paragraph ,  List ,  Table  or another  Section </param>
    /// <returns>a bool</returns>
    public bool Add(IElement o) => ((Section)Element).Add(o);

    public bool AddAll(ICollection<IElement> collection) => ((Section)Element).AddAll(collection);

    public MarkedSection AddSection(float indentation, int numberDepth)
    {
        var section = ((Section)Element).AddMarkedSection();
        section.Indentation = indentation;
        section.NumberDepth = numberDepth;
        return section;
    }

    public MarkedSection AddSection(float indentation)
    {
        var section = ((Section)Element).AddMarkedSection();
        section.Indentation = indentation;
        return section;
    }

    /// <summary>
    ///     Adds a collection of  Element s
    ///     to this  Section .
    ///     @throws  ClassCastException if one of the objects isn't a  Paragraph ,  List ,  Table
    /// </summary>
    public MarkedSection AddSection(int numberDepth)
    {
        var section = ((Section)Element).AddMarkedSection();
        section.NumberDepth = numberDepth;
        return section;
    }

    /// <summary>
    ///     Creates a  Section , adds it to this  Section  and returns it.
    /// </summary>
    /// <returns>a new Section object</returns>
    public MarkedSection AddSection() => ((Section)Element).AddMarkedSection();

    /// <summary>
    ///     Adds a new page to the section.
    ///     @since    2.1.1
    /// </summary>
    public void NewPage()
    {
        ((Section)Element).NewPage();
    }

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     ElementListener .
    /// </summary>
    /// <param name="listener">an  ElementListener </param>
    /// <returns> true  if the element was processed successfully</returns>
    public override bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            foreach (var element in (Section)Element)
            {
                listener.Add(element);
            }

            return true;
        }
        catch (DocumentException)
        {
            return false;
        }
    }
}