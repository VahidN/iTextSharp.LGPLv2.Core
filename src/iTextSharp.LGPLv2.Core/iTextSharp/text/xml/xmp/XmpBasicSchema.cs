namespace iTextSharp.text.xml.xmp;

/// <summary>
///     An implementation of an XmpSchema.
/// </summary>
public class XmpBasicSchema : XmpSchema
{
    /// <summary>
    ///     An unordered array specifying properties that were edited outside the authoring application. Each item should
    ///     contain a single namespace and XPath separated by one ASCII space (U+0020).
    /// </summary>
    public const string ADVISORY = "xmp:Advisory";

    /// <summary>
    ///     The base URL for relative URLs in the document content. If this document contains Internet links, and those links
    ///     are relative, they are relative to this base URL. This property provides a standard way for embedded relative URLs
    ///     to be interpreted by tools. Web authoring tools should set the value based on their notion of where URLs will be
    ///     interpreted.
    /// </summary>
    public const string BASEURL = "xmp:BaseURL";

    /// <summary>
    ///     The date and time the resource was originally created.
    /// </summary>
    public const string CREATEDATE = "xmp:CreateDate";

    /// <summary>
    ///     The name of the first known tool used to create the resource. If history is present in the metadata, this value
    ///     should be equivalent to that of xmpMM:History�s softwareAgent property.
    /// </summary>
    public const string CREATORTOOL = "xmp:CreatorTool";

    /// <summary>
    ///     default namespace identifier
    /// </summary>
    public const string DEFAULT_XPATH_ID = "xmp";

    /// <summary>
    ///     default namespace uri
    /// </summary>
    public const string DEFAULT_XPATH_URI = "http://ns.adobe.com/xap/1.0/";

    /// <summary>
    ///     An unordered array of text strings that unambiguously identify the resource within a given context.
    /// </summary>
    public const string IDENTIFIER = "xmp:Identifier";

    /// <summary>
    ///     The date and time that any metadata for this resource was last changed.
    /// </summary>
    public const string METADATADATE = "xmp:MetadataDate";

    /// <summary>
    ///     The date and time the resource was last modified.
    /// </summary>
    public const string MODIFYDATE = "xmp:ModifyDate";

    /// <summary>
    ///     A short informal name for the resource.
    /// </summary>
    public const string NICKNAME = "xmp:Nickname";

    /// <summary>
    ///     An alternative array of thumbnail images for a file, which can differ in characteristics such as size or image
    ///     encoding.
    /// </summary>
    public const string THUMBNAILS = "xmp:Thumbnails";

    /// <summary>
    ///     @throws IOException
    /// </summary>
    public XmpBasicSchema() : base($"xmlns:{DEFAULT_XPATH_ID}=\"{DEFAULT_XPATH_URI}\"")
    {
    }

    /// <summary>
    ///     Adds the creation date.
    /// </summary>
    /// <param name="date"></param>
    public void AddCreateDate(string date)
    {
        this[CREATEDATE] = date;
    }

    /// <summary>
    ///     Adds the creatortool.
    /// </summary>
    /// <param name="creator"></param>
    public void AddCreatorTool(string creator)
    {
        this[CREATORTOOL] = creator;
    }

    /// <summary>
    ///     Adds the identifier.
    /// </summary>
    /// <param name="id"></param>
    public void AddIdentifiers(string[] id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        var array = new XmpArray(XmpArray.UNORDERED);
        for (var i = 0; i < id.Length; i++)
        {
            array.Add(id[i]);
        }

        SetProperty(IDENTIFIER, array);
    }

    /// <summary>
    ///     Adds the meta data date.
    /// </summary>
    /// <param name="date"></param>
    public void AddMetaDataDate(string date)
    {
        this[METADATADATE] = date;
    }

    /// <summary>
    ///     Adds the modification date.
    /// </summary>
    /// <param name="date"></param>
    public void AddModDate(string date)
    {
        this[MODIFYDATE] = date;
    }

    /// <summary>
    ///     Adds the nickname.
    /// </summary>
    /// <param name="name"></param>
    public void AddNickname(string name)
    {
        this[NICKNAME] = name;
    }
}