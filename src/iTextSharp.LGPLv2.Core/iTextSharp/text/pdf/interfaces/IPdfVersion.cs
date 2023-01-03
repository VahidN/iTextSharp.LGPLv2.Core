namespace iTextSharp.text.pdf.interfaces;

/// <summary>
///     The PDF version is described in the PDF Reference 1.7 p92
///     (about the PDF Header) and page 139 (the version entry in
///     the Catalog). You'll also find info about setting the version
///     in the book 'iText in Action' sections 2.1.3 (PDF Header)
///     and 3.3 (Version history).
/// </summary>
public interface IPdfVersion
{
    /// <summary>
    ///     If the PDF Header hasn't been written yet,
    ///     this changes the version as it will appear in the PDF Header.
    ///     If the PDF header was already written to the Stream,
    ///     this changes the version as it will appear in the Catalog.
    /// </summary>
    char PdfVersion { set; }

    /// <summary>
    ///     Adds a developer extension to the Extensions dictionary
    ///     in the Catalog.
    ///     @since    2.1.6
    /// </summary>
    /// <param name="de">an object that contains the extensions prefix and dictionary</param>
    void AddDeveloperExtension(PdfDeveloperExtension de);

    /// <summary>
    ///     If the PDF Header hasn't been written yet,
    ///     this changes the version as it will appear in the PDF Header,
    ///     but only if param refers to a higher version.
    ///     If the PDF header was already written to the Stream,
    ///     this changes the version as it will appear in the Catalog.
    /// </summary>
    /// <param name="version">a character representing the PDF version</param>
    void SetAtLeastPdfVersion(char version);

    /// <summary>
    ///     Sets the PDF version as it will appear in the Catalog.
    ///     Note that this only has effect if you use a later version
    ///     than the one that appears in the header; this method
    ///     ignores the parameter if you try to set a lower version.
    /// </summary>
    /// <param name="version">the PDF name that will be used for the Version key in the catalog</param>
    void SetPdfVersion(PdfName version);
}