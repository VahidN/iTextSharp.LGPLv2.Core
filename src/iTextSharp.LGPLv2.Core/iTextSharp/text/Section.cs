using System.Text;
using System.util;

namespace iTextSharp.text;

/// <summary>
///     A Section is a part of a Document containing
///     other Sections, Paragraphs, List
///     and/or Tables.
/// </summary>
/// <remarks>
///     You can not construct a Section yourself.
///     You will have to ask an instance of Section to the
///     Chapter or Section to which you want to
///     add the new Section.
/// </remarks>
/// <example>
///     Paragraph title2 = new Paragraph("This is Chapter 2", FontFactory.GetFont(FontFactory.HELVETICA, 18,
///     Font.BOLDITALIC, new Color(0, 0, 255)));
///     Chapter chapter2 = new Chapter(title2, 2);
///     Paragraph someText = new Paragraph("This is some text");
///     chapter2.Add(someText);
///     Paragraph title21 = new Paragraph("This is Section 1 in Chapter 2", FontFactory.GetFont(FontFactory.HELVETICA, 16,
///     Font.BOLD, new Color(255, 0, 0)));
///     Section section1 = chapter2.AddSection(title21);
///     Paragraph someSectionText = new Paragraph("This is some silly paragraph in a chapter and/or section. It contains
///     some text to test the functionality of Chapters and Section.");
///     section1.Add(someSectionText);
///     Paragraph title211 = new Paragraph("This is SubSection 1 in Section 1 in Chapter 2",
///     FontFactory.GetFont(FontFactory.HELVETICA, 14, Font.BOLD, new Color(255, 0, 0)));
///     Section section11 = section1.AddSection(40, title211, 2);
///     section11.Add(someSectionText); strong>
/// </example>
public class Section : List<IElement>, ITextElementArray, ILargeElement
{
    /// <summary>
    ///     constant
    /// </summary>
    /// <summary>
    ///     A possible number style. The default number style: "1.2.3."
    ///     @since   iText 2.0.8
    /// </summary>
    public const int NUMBERSTYLE_DOTTED = 0;

    /// <summary>
    ///     A possible number style. For instance: "1.2.3"
    ///     @since   iText 2.0.8
    /// </summary>
    public const int NUMBERSTYLE_DOTTED_WITHOUT_FINAL_DOT = 1;

    /// <summary>
    ///     Indicates if the Section was added completely to the document.
    ///     @since   iText 2.0.8
    /// </summary>
    protected bool addedCompletely;

    ///<summary> false if the bookmark children are not visible </summary>
    protected bool bookmarkOpen = true;

    /// <summary>
    ///     The bookmark title if different from the content title
    /// </summary>
    protected string bookmarkTitle;

    /// <summary>
    ///     Indicates if the Section will be complete once added to the document.
    ///     @since   iText 2.0.8
    /// </summary>
    protected bool Complete = true;

    ///<summary> The additional indentation of the content of this section. </summary>
    protected float indentation;

    ///<summary> The indentation of this section on the left side. </summary>
    protected float indentationLeft;

    ///<summary> The indentation of this section on the right side. </summary>
    protected float indentationRight;

    /// <summary>
    ///     Indicates if this is the first time the section was added.
    ///     @since   iText 2.0.8
    /// </summary>
    protected bool notAddedYet = true;

    ///<summary> This is the number of sectionnumbers that has to be shown before the section title. </summary>
    protected int numberDepth;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> This is the complete list of sectionnumbers of this section and the parents of this section. </summary>
    protected internal List<int> Numbers;

    /// <summary>
    ///     The style for sectionnumbers.
    ///     @since    iText 2.0.8
    /// </summary>
    protected int numberStyle = NUMBERSTYLE_DOTTED;

    ///<summary> This is the number of subsections. </summary>
    protected int Subsections;

    ///<summary> This is the title of this section. </summary>
    protected Paragraph title;

    /// <summary>
    ///     true if the section has to trigger a new page
    /// </summary>
    protected bool triggerNewPage;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a new Section.
    /// </summary>
    /// <overloads>
    ///     Has 2 overloads.
    /// </overloads>
    protected internal Section()
    {
        title = new Paragraph();
        numberDepth = 1;
    }

    /// <summary>
    ///     Constructs a new Section.
    /// </summary>
    /// <param name="title">a Paragraph</param>
    /// <param name="numberDepth">the numberDepth</param>
    protected internal Section(Paragraph title, int numberDepth)
    {
        this.numberDepth = numberDepth;
        this.title = title;
    }

    /// <summary>
    ///     private methods
    /// </summary>
    /// <summary>
    ///     Get/set the bookmark
    /// </summary>
    /// <value>a bool</value>
    public bool BookmarkOpen
    {
        get => bookmarkOpen;

        set => bookmarkOpen = value;
    }

    /// <summary>
    ///     Sets the bookmark title. The bookmark title is the same as the section title but
    ///     can be changed with this method.
    /// </summary>
    public string BookmarkTitle
    {
        set => bookmarkTitle = value;
    }

    /// <summary>
    ///     Returns the depth of this section.
    /// </summary>
    /// <value>the depth</value>
    public int Depth => Numbers.Count;

    /// <summary>
    ///     Get/set the indentation of the content of this Section.
    /// </summary>
    /// <value>the indentation</value>
    public float Indentation
    {
        get => indentation;

        set => indentation = value;
    }

    /// <summary>
    ///     Get/set the indentation of this Section on the left side.
    /// </summary>
    /// <value>the indentation</value>
    public float IndentationLeft
    {
        get => indentationLeft;

        set => indentationLeft = value;
    }

    /// <summary>
    ///     Get/set the indentation of this Section on the right side.
    /// </summary>
    /// <value>the indentation</value>
    public float IndentationRight
    {
        get => indentationRight;

        set => indentationRight = value;
    }

    /// <summary>
    ///     Indicates if this is the first time the section is added.
    ///     @since   iText2.0.8
    /// </summary>
    /// <returns>true if the section wasn't added yet</returns>
    public bool NotAddedYet
    {
        get => notAddedYet;
        set => notAddedYet = value;
    }

    /// <summary>
    ///     Get/set the numberdepth of this Section.
    /// </summary>
    /// <value>a int</value>
    public int NumberDepth
    {
        get => numberDepth;

        set => numberDepth = value;
    }

    /// <summary>
    ///     Sets the style for numbering sections.
    ///     Possible values are NUMBERSTYLE_DOTTED: 1.2.3. (the default)
    ///     or NUMBERSTYLE_DOTTED_WITHOUT_FINAL_DOT: 1.2.3
    ///     @since    iText 2.0.8
    /// </summary>
    public int NumberStyle
    {
        set => numberStyle = value;
        get => numberStyle;
    }

    /// <summary>
    ///     Get/set the title of this section
    /// </summary>
    /// <value>a Paragraph</value>
    public Paragraph Title
    {
        get => ConstructTitle(title, Numbers, numberDepth, numberStyle);

        set => title = value;
    }

    public virtual bool TriggerNewPage
    {
        get => triggerNewPage && notAddedYet;
        set => triggerNewPage = value;
    }

    /// <summary>
    ///     @see com.lowagie.text.LargeElement#isAddedCompletely()
    ///     @since   iText 2.0.8
    /// </summary>
    protected bool AddedCompletely
    {
        get => addedCompletely;
        set => addedCompletely = value;
    }

    /// <summary>
    ///     @since   iText 2.0.8
    ///     @see com.lowagie.text.LargeElement#isComplete()
    /// </summary>
    public bool ElementComplete
    {
        get => Complete;
        set => Complete = value;
    }

    /// <summary>
    ///     @since   iText 2.0.8
    ///     @see com.lowagie.text.LargeElement#flushContent()
    /// </summary>
    public void FlushContent()
    {
        NotAddedYet = false;
        title = null;
        for (var k = 0; k < Count; ++k)
        {
            var element = this[k];
            if (element is Section)
            {
                var s = (Section)element;
                if (!s.ElementComplete && Count == 1)
                {
                    s.FlushContent();
                    return;
                }

                s.AddedCompletely = true;
            }

            RemoveAt(k);
            --k;
        }
    }

    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public IList<Chunk> Chunks
    {
        get
        {
            var tmp = new List<Chunk>();
            foreach (var ele in this)
            {
                tmp.AddRange(ele.Chunks);
            }

            return tmp;
        }
    }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public virtual int Type => Element.SECTION;

    /// <summary>
    ///     overriding some of the ArrayList-methods
    /// </summary>
    /// <summary>
    ///     Adds a Paragraph, List, Table or another Section
    ///     to this Section.
    /// </summary>
    /// <param name="o">an object of type Paragraph, List, Table or another Section</param>
    /// <returns>a bool</returns>
    public new bool Add(IElement o)
    {
        if (o == null)
        {
            throw new ArgumentNullException(nameof(o));
        }

        try
        {
            var element = o;
            if (element.Type == Element.SECTION)
            {
                var section = (Section)o;
                section.setNumbers(++Subsections, Numbers);
                base.Add(section);
                return true;
            }

            if (o is MarkedSection && ((MarkedObject)o).Element.Type == Element.SECTION)
            {
                var mo = (MarkedSection)o;
                var section = (Section)mo.Element;
                section.setNumbers(++Subsections, Numbers);
                base.Add(mo);
                return true;
            }

            if (element.IsNestable())
            {
                base.Add(o);
                return true;
            }

            throw new InvalidOperationException(element.Type.ToString(CultureInfo.InvariantCulture));
        }
        catch (Exception cce)
        {
            throw new InvalidOperationException("Insertion of illegal Element: " + cce.Message);
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public virtual bool IsNestable() => false;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     IElementListener.
    /// </summary>
    /// <param name="listener">the IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    public bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            foreach (var ele in this)
            {
                listener.Add(ele);
            }

            return true;
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Constructs a Paragraph that will be used as title for a Section or Chapter.
    ///     @since    iText 2.0.8
    /// </summary>
    /// <param name="title">the title of the section</param>
    /// <param name="numbers">a list of sectionnumbers</param>
    /// <param name="numberDepth">how many numbers have to be shown</param>
    /// <param name="numberStyle">the numbering style</param>
    /// <returns>a Paragraph object</returns>
    public static Paragraph ConstructTitle(Paragraph title, IList<int> numbers, int numberDepth, int numberStyle)
    {
        if (numbers == null)
        {
            throw new ArgumentNullException(nameof(numbers));
        }

        if (title == null)
        {
            return null;
        }

        var depth = Math.Min(numbers.Count, numberDepth);
        if (depth < 1)
        {
            return title;
        }

        var buf = new StringBuilder(" ");
        for (var i = 0; i < depth; i++)
        {
            buf.Insert(0, ".");
            buf.Insert(0, numbers[i]);
        }

        if (numberStyle == NUMBERSTYLE_DOTTED_WITHOUT_FINAL_DOT)
        {
            buf.Remove(buf.Length - 2, 1);
        }

        var result = new Paragraph(title);
        result.Insert(0, new Chunk(buf.ToString(), title.Font));
        return result;
    }

    /// <summary>
    ///     Checks if a given tag corresponds with this object.
    /// </summary>
    /// <param name="tag">the given tag</param>
    /// <returns>true if the tag corresponds</returns>
    public static bool IsTag(string tag) => ElementTags.SECTION.Equals(tag, StringComparison.Ordinal);

    /// <summary>
    ///     Checks if a given tag corresponds with a title tag for this object.
    /// </summary>
    /// <param name="tag">the given tag</param>
    /// <returns>true if the tag corresponds</returns>
    public static bool IsTitle(string tag) => ElementTags.TITLE.Equals(tag, StringComparison.Ordinal);

    /// <summary>
    ///     Adds a Paragraph, List or Table
    ///     to this Section.
    /// </summary>
    /// <param name="index">index at which the specified element is to be inserted</param>
    /// <param name="o">an object of type Paragraph, List or Table</param>
    public void Add(int index, object o)
    {
        if (o == null)
        {
            throw new ArgumentNullException(nameof(o));
        }

        if (AddedCompletely)
        {
            throw new InvalidOperationException("This LargeElement has already been added to the Document.");
        }

        try
        {
            var element = (IElement)o;
            if (element.IsNestable())
            {
                Insert(index, element);
            }
            else
            {
                throw new InvalidOperationException(element.Type.ToString(CultureInfo.InvariantCulture));
            }
        }
        catch (Exception cce)
        {
            throw new InvalidOperationException("Insertion of illegal Element: " + cce.Message);
        }
    }

    /// <summary>
    ///     Adds a collection of Elements
    ///     to this Section.
    /// </summary>
    /// <param name="collection">a collection of Paragraphs, Lists and/or Tables</param>
    /// <returns>true if the action succeeded, false if not.</returns>
    public bool AddAll<T>(ICollection<T> collection) where T : IElement
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        foreach (var itm in collection)
        {
            Add(itm);
        }

        return true;
    }

    /// <summary>
    ///     Adds a marked section. For use in class MarkedSection only!
    /// </summary>
    public MarkedSection AddMarkedSection()
    {
        var section = new MarkedSection(new Section(null, numberDepth + 1));
        Add(section);
        return section;
    }

    /// <summary>
    ///     Creates a Section, adds it to this Section and returns it.
    /// </summary>
    /// <param name="indentation">the indentation of the new section</param>
    /// <param name="title">the title of the new section</param>
    /// <param name="numberDepth">the numberDepth of the section</param>
    /// <returns>the newly added Section</returns>
    public virtual Section AddSection(float indentation, Paragraph title, int numberDepth)
    {
        if (AddedCompletely)
        {
            throw new InvalidOperationException("This LargeElement has already been added to the Document.");
        }

        var section = new Section(title, numberDepth);
        section.Indentation = indentation;
        Add(section);
        return section;
    }

    /// <summary>
    ///     methods that return a Section
    /// </summary>
    /// <summary>
    ///     Creates a Section, adds it to this Section and returns it.
    /// </summary>
    /// <param name="indentation">the indentation of the new section</param>
    /// <param name="title">the title of the new section</param>
    /// <returns>the newly added Section</returns>
    public virtual Section AddSection(float indentation, Paragraph title) =>
        AddSection(indentation, title, numberDepth + 1);

    /// <summary>
    ///     Creates a Section, add it to this Section and returns it.
    /// </summary>
    /// <param name="title">the title of the new section</param>
    /// <param name="numberDepth">the numberDepth of the section</param>
    /// <returns>the newly added Section</returns>
    public virtual Section AddSection(Paragraph title, int numberDepth) => AddSection(0, title, numberDepth);

    /// <summary>
    ///     Creates a Section, adds it to this Section and returns it.
    /// </summary>
    /// <param name="title">the title of the new section</param>
    /// <returns>the newly added Section</returns>
    public virtual Section AddSection(Paragraph title) => AddSection(0, title, numberDepth + 1);

    /// <summary>
    ///     Adds a Section to this Section and returns it.
    /// </summary>
    /// <param name="indentation">the indentation of the new section</param>
    /// <param name="title">the title of the new section</param>
    /// <param name="numberDepth">the numberDepth of the section</param>
    /// <returns>the newly added Section</returns>
    public virtual Section AddSection(float indentation, string title, int numberDepth) =>
        AddSection(indentation, new Paragraph(title), numberDepth);

    /// <summary>
    ///     Adds a Section to this Section and returns it.
    /// </summary>
    /// <param name="title">the title of the new section</param>
    /// <param name="numberDepth">the numberDepth of the section</param>
    /// <returns>the newly added Section</returns>
    public virtual Section AddSection(string title, int numberDepth) => AddSection(new Paragraph(title), numberDepth);

    /// <summary>
    ///     Adds a Section to this Section and returns it.
    /// </summary>
    /// <param name="indentation">the indentation of the new section</param>
    /// <param name="title">the title of the new section</param>
    /// <returns>the newly added Section</returns>
    public virtual Section AddSection(float indentation, string title) => AddSection(indentation, new Paragraph(title));

    /// <summary>
    ///     Adds a Section to this Section and returns it.
    /// </summary>
    /// <param name="title">the title of the new section</param>
    /// <returns>the newly added Section</returns>
    public virtual Section AddSection(string title) => AddSection(new Paragraph(title));

    /// <summary>
    ///     Gets the bookmark title.
    /// </summary>
    /// <returns>the bookmark title</returns>
    public Paragraph GetBookmarkTitle()
    {
        if (bookmarkTitle == null)
        {
            return Title;
        }

        return new Paragraph(bookmarkTitle);
    }

    /// <summary>
    ///     Checks if this object is a Chapter.
    /// </summary>
    /// <returns>
    ///     true if it is a Chapter,
    ///     false if it is a Section
    /// </returns>
    public bool IsChapter() => Type == Element.CHAPTER;

    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Checks if this object is a Section.
    /// </summary>
    /// <returns>
    ///     true if it is a Section,
    ///     false if it is a Chapter.
    /// </returns>
    public bool IsSection() => Type == Element.SECTION;

    /// <summary>
    ///     Adds a new page to the section.
    ///     @since   2.1.1
    /// </summary>
    public void NewPage()
    {
        Add(Chunk.Nextpage);
    }

    /// <summary>
    ///     Alters the attributes of this Section.
    /// </summary>
    /// <param name="attributes">the attributes</param>
    public void Set(Properties attributes)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        string value;
        if ((value = attributes.Remove(ElementTags.NUMBERDEPTH)) != null)
        {
            NumberDepth = int.Parse(value, CultureInfo.InvariantCulture);
        }

        if ((value = attributes.Remove(ElementTags.INDENT)) != null)
        {
            Indentation = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        if ((value = attributes.Remove(ElementTags.INDENTATIONLEFT)) != null)
        {
            IndentationLeft = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }

        if ((value = attributes.Remove(ElementTags.INDENTATIONRIGHT)) != null)
        {
            IndentationRight = float.Parse(value, NumberFormatInfo.InvariantInfo);
        }
    }

    /// <summary>
    ///     Changes the Chapter number.
    /// </summary>
    public void SetChapterNumber(int number)
    {
        Numbers[Numbers.Count - 1] = number;
        foreach (var s in this)
        {
            if (s is Section)
            {
                ((Section)s).SetChapterNumber(number);
            }
        }
    }

    /// <summary>
    ///     Sets the number of this section.
    /// </summary>
    /// <param name="number">the number of this section</param>
    /// <param name="numbers">an ArrayList, containing the numbers of the Parent</param>
    private void setNumbers(int number, IList<int> numbers)
    {
        Numbers = new List<int>();
        Numbers.Add(number);
        Numbers.AddRange(numbers);
    }
}