using System.Text;

namespace iTextSharp.text;

/// <summary>
///     This is an Element that contains
///     some meta information about the document.
/// </summary>
/// <remarks>
///     An object of type Meta can not be constructed by the user.
///     Userdefined meta information should be placed in a Header-object.
///     Meta is reserved for: Subject, Keywords, Author, Title, Producer
///     and Creationdate information.
/// </remarks>
public class Meta : IElement
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> This is the content of the Meta-information. </summary>
    private readonly StringBuilder _content;

    ///<summary> This is the type of Meta-information this object contains. </summary>
    private readonly int _type;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a Meta.
    /// </summary>
    /// <param name="type">the type of meta-information</param>
    /// <param name="content">the content</param>
    public Meta(int type, string content)
    {
        _type = type;
        _content = new StringBuilder(content);
    }

    /// <summary>
    ///     Constructs a Meta.
    /// </summary>
    /// <param name="tag">the tagname of the meta-information</param>
    /// <param name="content">the content</param>
    public Meta(string tag, string content)
    {
        _type = GetType(tag);
        _content = new StringBuilder(content);
    }

    /// <summary>
    ///     Returns the content of the meta information.
    /// </summary>
    /// <value>a string</value>
    public string Content => _content.ToString();

    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Returns the name of the meta information.
    /// </summary>
    /// <value>a string</value>
    public virtual string Name
    {
        get
        {
            switch (_type)
            {
                case Element.SUBJECT:
                    return ElementTags.SUBJECT;
                case Element.KEYWORDS:
                    return ElementTags.KEYWORDS;
                case Element.AUTHOR:
                    return ElementTags.AUTHOR;
                case Element.TITLE:
                    return ElementTags.TITLE;
                case Element.PRODUCER:
                    return ElementTags.PRODUCER;
                case Element.CREATIONDATE:
                    return ElementTags.CREATIONDATE;
                default:
                    return ElementTags.UNKNOWN;
            }
        }
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public IList<Chunk> Chunks => new List<Chunk>();

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public int Type => _type;

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => false;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsNestable() => false;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to a
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
            return listener.Add(this);
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Returns the name of the meta information.
    /// </summary>
    /// <param name="tag">name to match</param>
    /// <returns>a string</returns>
    public static int GetType(string tag)
    {
        if (ElementTags.SUBJECT.Equals(tag, StringComparison.Ordinal))
        {
            return Element.SUBJECT;
        }

        if (ElementTags.KEYWORDS.Equals(tag, StringComparison.Ordinal))
        {
            return Element.KEYWORDS;
        }

        if (ElementTags.AUTHOR.Equals(tag, StringComparison.Ordinal))
        {
            return Element.AUTHOR;
        }

        if (ElementTags.TITLE.Equals(tag, StringComparison.Ordinal))
        {
            return Element.TITLE;
        }

        if (ElementTags.PRODUCER.Equals(tag, StringComparison.Ordinal))
        {
            return Element.PRODUCER;
        }

        if (ElementTags.CREATIONDATE.Equals(tag, StringComparison.Ordinal))
        {
            return Element.CREATIONDATE;
        }

        return Element.HEADER;
    }

    /// <summary>
    ///     appends some text to this Meta.
    /// </summary>
    /// <param name="str">a string</param>
    /// <returns>a StringBuilder</returns>
    public StringBuilder Append(string str) => _content.Append(str);
}