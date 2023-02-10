namespace iTextSharp.text.xml.xmp;

/// <summary>
///     An implementation of an XmpSchema.
/// </summary>
public class PdfSchema : XmpSchema
{
    /// <summary>
    ///     default namespace identifier
    /// </summary>
    public const string DEFAULT_XPATH_ID = "pdf";

    /// <summary>
    ///     default namespace uri
    /// </summary>
    public const string DEFAULT_XPATH_URI = "http://ns.adobe.com/pdf/1.3/";

    /// <summary>
    ///     Keywords.
    /// </summary>
    public const string KEYWORDS = "pdf:keywords";

    /// <summary>
    ///     The Producer.
    /// </summary>
    public const string PRODUCER = "pdf:Producer";

    /// <summary>
    ///     The PDF file version (for example: 1.0, 1.3, and so on).
    /// </summary>
    public const string VERSION = "pdf:PDFVersion";

    /// <summary>
    ///     @throws IOException
    /// </summary>
    public PdfSchema() : base("xmlns:" + DEFAULT_XPATH_ID + "=\"" + DEFAULT_XPATH_URI + "\"")
    {
        AddProducer(Document.Version);
    }

    /// <summary>
    ///     Adds keywords.
    /// </summary>
    /// <param name="keywords"></param>
    public void AddKeywords(string keywords)
    {
        this[KEYWORDS] = keywords;
    }

    /// <summary>
    ///     Adds the producer.
    /// </summary>
    /// <param name="producer"></param>
    public void AddProducer(string producer)
    {
        this[PRODUCER] = producer;
    }

    /// <summary>
    ///     Adds the version.
    /// </summary>
    /// <param name="version"></param>
    public void AddVersion(string version)
    {
        this[VERSION] = version;
    }
}