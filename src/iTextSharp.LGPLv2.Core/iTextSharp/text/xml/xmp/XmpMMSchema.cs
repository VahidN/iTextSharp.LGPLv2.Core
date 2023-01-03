namespace iTextSharp.text.xml.xmp;

/// <summary>
///     An implementation of an XmpSchema.
/// </summary>
public class XmpMmSchema : XmpSchema
{
    /// <summary>
    ///     default namespace identifier
    /// </summary>
    public const string DEFAULT_XPATH_ID = "xmpMM";

    /// <summary>
    ///     default namespace uri
    /// </summary>
    public const string DEFAULT_XPATH_URI = "http://ns.adobe.com/xap/1.0/mm/";

    /// <summary>
    ///     A reference to the original document from which this one is derived. It is a minimal reference; missing components
    ///     can be assumed to be unchanged. For example, a new version might only need to specify the instance ID and version
    ///     number of the previous version, or a rendition might only need to specify the instance ID and rendition class of
    ///     the original.
    /// </summary>
    public const string DERIVEDFROM = "xmpMM:DerivedFrom";

    /// <summary>
    ///     The common identifier for all versions and renditions of a document.
    /// </summary>
    public const string DOCUMENTID = "xmpMM:DocumentID";

    /// <summary>
    ///     An ordered array of high-level user actions that resulted in this resource. It is intended to give human readers a
    ///     general indication of the steps taken to make the changes from the previous version to this one. The list should be
    ///     at an abstract level; it is not intended to be an exhaustive keystroke or other detailed history.
    /// </summary>
    public const string HISTORY = "xmpMM:History";

    /// <summary>
    ///     A reference to the document as it was prior to becoming managed. It is set when a managed document is introduced to
    ///     an asset management system that does not currently own it. It may or may not include references to different
    ///     management systems.
    /// </summary>
    public const string MANAGEDFROM = "xmpMM:ManagedFrom";

    /// <summary>
    ///     The name of the asset management system that manages this resource.
    /// </summary>
    public const string MANAGER = "xmpMM:Manager";

    /// <summary>
    ///     Specifies a particular variant of the asset management system. The format of this property is private to the
    ///     specific asset management system.
    /// </summary>
    public const string MANAGERVARIANT = "xmpMM:ManagerVariant";

    /// <summary>
    ///     A URI identifying the managed resource to the asset management system; the presence of this property is the formal
    ///     indication that this resource is managed. The form and content of this URI is private to the asset management
    ///     system.
    /// </summary>
    public const string MANAGETO = "xmpMM:ManageTo";

    /// <summary>
    ///     A URI that can be used to access information about the managed resource through a web browser. It might require a
    ///     custom browser plugin.
    /// </summary>
    public const string MANAGEUI = "xmpMM:ManageUI";

    /// <summary>
    ///     The rendition class name for this resource.
    /// </summary>
    public const string RENDITIONCLASS = "xmpMM:RenditionClass";

    /// <summary>
    ///     Can be used to provide additional rendition parameters that are too complex or verbose to encode in xmpMM:
    ///     RenditionClass.
    /// </summary>
    public const string RENDITIONPARAMS = "xmpMM:RenditionParams";

    /// <summary>
    ///     The document version identifier for this resource.
    /// </summary>
    public const string VERSIONID = "xmpMM:VersionID";

    /// <summary>
    ///     The version history associated with this resource.
    /// </summary>
    public const string VERSIONS = "xmpMM:Versions";

    /// <summary>
    ///     @throws IOException
    /// </summary>
    public XmpMmSchema() : base($"xmlns:{DEFAULT_XPATH_ID}=\"{DEFAULT_XPATH_URI}\"")
    {
    }
}