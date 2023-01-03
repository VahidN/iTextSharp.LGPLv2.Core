namespace iTextSharp.text.pdf;

/// <summary>
///     Beginning with BaseVersion 1.7, the extensions dictionary lets developers
///     designate that a given document contains extensions to PDF. The presence
///     of the extension dictionary in a document indicates that it may contain
///     developer-specific PDF properties that extend a particular base version
///     of the PDF specification.
///     The extensions dictionary enables developers to identify their own extensions
///     relative to a base version of PDF. Additionally, the convention identifies
///     extension levels relative to that base version. The intent of this dictionary
///     is to enable developers of PDF-producing applications to identify company-specific
///     specifications (such as this one) that PDF-consuming applications use to
///     interpret the extensions.
///     @since   2.1.6
/// </summary>
public class PdfDeveloperExtension
{
    /// <summary>
    ///     An instance of this class for Adobe 1.7 Extension level 3.
    /// </summary>
    public static readonly PdfDeveloperExtension Adobe17Extensionlevel3 =
        new(PdfName.Adbe, PdfWriter.PdfVersion17, 3);

    /// <summary>
    ///     The base version.
    /// </summary>
    protected PdfName baseversion;

    /// <summary>
    ///     The extension level within the baseversion.
    /// </summary>
    protected int extensionLevel;

    /// <summary>
    ///     The prefix used in the Extensions dictionary added to the Catalog.
    /// </summary>
    protected PdfName prefix;

    /// <summary>
    ///     Creates a PdfDeveloperExtension object.
    /// </summary>
    /// <param name="prefix">the prefix referring to the developer</param>
    /// <param name="baseversion">the number of the base version</param>
    /// <param name="extensionLevel">the extension level within the baseverion.</param>
    public PdfDeveloperExtension(PdfName prefix, PdfName baseversion, int extensionLevel)
    {
        this.prefix = prefix;
        this.baseversion = baseversion;
        this.extensionLevel = extensionLevel;
    }

    /// <summary>
    ///     Gets the baseversion name.
    /// </summary>
    /// <returns>a PdfName</returns>
    public PdfName Baseversion => baseversion;

    /// <summary>
    ///     Gets the extension level within the baseversion.
    /// </summary>
    /// <returns>an integer</returns>
    public int ExtensionLevel => extensionLevel;

    /// <summary>
    ///     Gets the prefix name.
    /// </summary>
    /// <returns>a PdfName</returns>
    public PdfName Prefix => prefix;

    /// <summary>
    ///     Generations the developer extension dictionary corresponding
    ///     with the prefix.
    /// </summary>
    /// <returns>a PdfDictionary</returns>
    public PdfDictionary GetDeveloperExtensions()
    {
        var developerextensions = new PdfDictionary();
        developerextensions.Put(PdfName.Baseversion, baseversion);
        developerextensions.Put(PdfName.Extensionlevel, new PdfNumber(extensionLevel));
        return developerextensions;
    }
}