namespace iTextSharp.text.xml.xmp;

/// <summary>
///     An implementation of an XmpSchema.
/// </summary>
public class DublinCoreSchema : XmpSchema
{
    /// <summary>
    ///     External Contributors to the resource (other than the authors).
    /// </summary>
    public const string CONTRIBUTOR = "dc:contributor";

    /// <summary>
    ///     The extent or scope of the resource.
    /// </summary>
    public const string COVERAGE = "dc:coverage";

    /// <summary>
    ///     The authors of the resource (listed in order of precedence, if significant).
    /// </summary>
    public const string CREATOR = "dc:creator";

    /// <summary>
    ///     Date(s) that something interesting happened to the resource.
    /// </summary>
    public const string DATE = "dc:date";

    /// <summary>
    ///     default namespace identifier
    /// </summary>
    public const string DEFAULT_XPATH_ID = "dc";

    /// <summary>
    ///     default namespace uri
    /// </summary>
    public const string DEFAULT_XPATH_URI = "http://purl.org/dc/elements/1.1/";

    /// <summary>
    ///     A textual description of the content of the resource. Multiple values may be present for different languages.
    /// </summary>
    public const string DESCRIPTION = "dc:description";

    /// <summary>
    ///     The file format used when saving the resource. Tools and applications should set this property to the save format
    ///     of the data. It may include appropriate qualifiers.
    /// </summary>
    public const string FORMAT = "dc:format";

    /// <summary>
    ///     Unique identifier of the resource.
    /// </summary>
    public const string IDENTIFIER = "dc:identifier";

    /// <summary>
    ///     An unordered array specifying the languages used in the resource.
    /// </summary>
    public const string LANGUAGE = "dc:language";

    /// <summary>
    ///     Publishers.
    /// </summary>
    public const string PUBLISHER = "dc:publisher";

    /// <summary>
    ///     Relationships to other documents.
    /// </summary>
    public const string RELATION = "dc:relation";

    /// <summary>
    ///     Informal rights statement, selected by language.
    /// </summary>
    public const string RIGHTS = "dc:rights";

    /// <summary>
    ///     Unique identifier of the work from which this resource was derived.
    /// </summary>
    public const string SOURCE = "dc:source";

    /// <summary>
    ///     An unordered array of descriptive phrases or keywords that specify the topic of the content of the resource.
    /// </summary>
    public const string SUBJECT = "dc:subject";

    /// <summary>
    ///     The title of the document, or the name given to the resource. Typically, it will be a name by which the resource is
    ///     formally known.
    /// </summary>
    public const string TITLE = "dc:title";

    /// <summary>
    ///     A document type; for example, novel, poem, or working paper.
    /// </summary>
    public const string TYPE = "dc:type";

    /// <summary>
    ///     @throws IOException
    /// </summary>
    public DublinCoreSchema() : base($"xmlns:{DEFAULT_XPATH_ID}=\"{DEFAULT_XPATH_URI}\"") =>
        this[FORMAT] = "application/pdf";

    /// <summary>
    ///     Adds a single author.
    /// </summary>
    /// <param name="author"></param>
    public void AddAuthor(string author)
    {
        var array = new XmpArray(XmpArray.ORDERED);
        array.Add(author);
        SetProperty(CREATOR, array);
    }

    /// <summary>
    ///     Adds an array of authors.
    /// </summary>
    /// <param name="author"></param>
    public void AddAuthor(string[] author)
    {
        if (author == null)
        {
            throw new ArgumentNullException(nameof(author));
        }

        var array = new XmpArray(XmpArray.ORDERED);
        for (var i = 0; i < author.Length; i++)
        {
            array.Add(author[i]);
        }

        SetProperty(CREATOR, array);
    }

    /// <summary>
    ///     Adds a description.
    /// </summary>
    /// <param name="desc"></param>
    public void AddDescription(string desc)
    {
        var array = new XmpArray(XmpArray.ALTERNATIVE);
        array.Add(desc);
        SetProperty(DESCRIPTION, array);
    }

    /// <summary>
    ///     Adds a single publisher.
    /// </summary>
    /// <param name="publisher"></param>
    public void AddPublisher(string publisher)
    {
        var array = new XmpArray(XmpArray.ORDERED);
        array.Add(publisher);
        SetProperty(PUBLISHER, array);
    }

    /// <summary>
    ///     Adds an array of publishers.
    /// </summary>
    /// <param name="publisher"></param>
    public void AddPublisher(string[] publisher)
    {
        if (publisher == null)
        {
            throw new ArgumentNullException(nameof(publisher));
        }

        var array = new XmpArray(XmpArray.ORDERED);
        for (var i = 0; i < publisher.Length; i++)
        {
            array.Add(publisher[i]);
        }

        SetProperty(PUBLISHER, array);
    }

    /// <summary>
    ///     Adds a subject.
    /// </summary>
    /// <param name="subject">array of subjects</param>
    public void addSubject(string[] subject)
    {
        if (subject == null)
        {
            throw new ArgumentNullException(nameof(subject));
        }

        var array = new XmpArray(XmpArray.UNORDERED);
        for (var i = 0; i < subject.Length; i++)
        {
            array.Add(subject[i]);
        }

        SetProperty(SUBJECT, array);
    }

    /// <summary>
    ///     Adds a subject.
    /// </summary>
    /// <param name="subject"></param>
    public void AddSubject(string subject)
    {
        var array = new XmpArray(XmpArray.UNORDERED);
        array.Add(subject);
        SetProperty(SUBJECT, array);
    }

    /// <summary>
    ///     Adds a title.
    /// </summary>
    /// <param name="title"></param>
    public void AddTitle(string title)
    {
        var array = new XmpArray(XmpArray.ALTERNATIVE);
        array.Add(title);
        SetProperty(TITLE, array);
    }
}