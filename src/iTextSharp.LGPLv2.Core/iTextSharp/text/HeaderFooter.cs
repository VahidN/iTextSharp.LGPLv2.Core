using System.util;

namespace iTextSharp.text;

/// <summary>
///     A HeaderFooter-object is a Rectangle with text
///     that can be put above and/or below every page.
/// </summary>
/// <example>
///     HeaderFooter header = new HeaderFooter(new Phrase("This is a header."), false);
///     HeaderFooter footer = new HeaderFooter(new Phrase("This is page "), new Phrase("."));
///     document.SetHeader(header);
///     document.SetFooter(footer);
/// </example>
public class HeaderFooter : Rectangle
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> Does the page contain a pagenumber? </summary>
    private readonly bool _numbered;

    /// <summary> This is the Phrase that comes after the pagenumber. </summary>
    private Phrase _after;

    /// <summary> This is alignment of the header/footer. </summary>
    private int _alignment;

    /// <summary> This is the Phrase that comes before the pagenumber. </summary>
    private Phrase _before;

    /// <summary> This is number of the page. </summary>
    private int _pageN;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a HeaderFooter-object.
    /// </summary>
    /// <param name="before">the Phrase before the pagenumber</param>
    /// <param name="after">the Phrase after the pagenumber</param>
    public HeaderFooter(Phrase before, Phrase after) : base(0, 0, 0, 0)
    {
        Border = TOP_BORDER + BOTTOM_BORDER;
        BorderWidth = 1;

        _numbered = true;
        _before = before;
        _after = after;
    }

    /// <summary>
    ///     Constructs a Header-object with a pagenumber at the end.
    /// </summary>
    /// <param name="before">the Phrase before the pagenumber</param>
    /// <param name="numbered">true if the page has to be numbered</param>
    public HeaderFooter(Phrase before, bool numbered) : base(0, 0, 0, 0)
    {
        Border = TOP_BORDER + BOTTOM_BORDER;
        BorderWidth = 1;

        _numbered = numbered;
        _before = before;
    }

    public HeaderFooter(Properties attributes) : base(0, 0, 0, 0)
    {
        if (attributes == null)
        {
            throw new ArgumentNullException(nameof(attributes));
        }

        string value;

        if ((value = attributes.Remove(ElementTags.NUMBERED)) != null)
        {
            _numbered = bool.Parse(value);
        }

        if ((value = attributes.Remove(ElementTags.ALIGN)) != null)
        {
            SetAlignment(value);
        }

        if ((value = attributes.Remove("border")) != null)
        {
            Border = int.Parse(value, CultureInfo.InvariantCulture);
        }
        else
        {
            Border = TOP_BORDER + BOTTOM_BORDER;
        }
    }

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Get/set the part that comes after the pageNumber.
    /// </summary>
    /// <value>a Phrase</value>
    public Phrase After
    {
        get => _after;

        set => _after = value;
    }

    /// <summary>
    ///     Sets the Element.
    /// </summary>
    /// <value>the new alignment</value>
    public int Alignment
    {
        set => _alignment = value;
        get => _alignment;
    }

    /// <summary>
    ///     Get/set the part that comes before the pageNumber.
    /// </summary>
    /// <value>a Phrase</value>
    public Phrase Before
    {
        get => _before;

        set => _before = value;
    }

    /// <summary>
    ///     Sets the page number.
    /// </summary>
    /// <value>the new page number</value>
    public int PageNumber
    {
        set => _pageN = value;
    }

    /// <summary>
    ///     Gets the Paragraph that can be used as header or footer.
    /// </summary>
    /// <returns>a Paragraph</returns>
    public Paragraph Paragraph
    {
        get
        {
            var paragraph = new Paragraph(_before.Leading);
            paragraph.Add(_before);
            if (_numbered)
            {
                paragraph.AddSpecial(new Chunk(_pageN.ToString(CultureInfo.InvariantCulture), _before.Font));
            }

            if (_after != null)
            {
                paragraph.AddSpecial(_after);
            }

            paragraph.Alignment = _alignment;
            return paragraph;
        }
    }

    /// <summary>
    ///     Checks if the HeaderFooter contains a page number.
    /// </summary>
    /// <returns>true if the page has to be numbered</returns>
    public bool IsNumbered() => _numbered;

    /// <summary>
    ///     Sets the alignment of this HeaderFooter.
    /// </summary>
    /// <param name="alignment">the new alignment as a string</param>
    public void SetAlignment(string alignment)
    {
        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_CENTER))
        {
            _alignment = ALIGN_CENTER;
            return;
        }

        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_RIGHT))
        {
            _alignment = ALIGN_RIGHT;
            return;
        }

        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED))
        {
            _alignment = ALIGN_JUSTIFIED;
            return;
        }

        _alignment = ALIGN_LEFT;
    }
}