using System.util;
using iTextSharp.text.pdf.interfaces;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
///     Allows you to add one (or more) existing PDF document(s) to
///     create a new PDF and add the form of another PDF document to
///     this new PDF.
///     @since 2.1.5
/// </summary>
public class PdfCopyForms : IPdfViewerPreferences, IPdfEncryptionSettings
{
    /// <summary>
    ///     The class with the actual implementations.
    /// </summary>
    private readonly PdfCopyFormsImp _fc;

    /// <summary>
    ///     Creates a new instance.
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="os">the output stream</param>
    public PdfCopyForms(Stream os) => _fc = new PdfCopyFormsImp(os);

    /// <summary>
    ///     Gets the 1.5 compression status.
    /// </summary>
    /// <returns> true  if the 1.5 compression is on</returns>
    public bool FullCompression => _fc.FullCompression;

    /// <summary>
    ///     Sets the bookmarks. The list structure is defined in
    ///     SimpleBookmark# .
    /// </summary>
    public IList<INullValueDictionary<string, object>> Outlines
    {
        set => _fc.Outlines = value;
    }

    /// <summary>
    ///     Gets the underlying PdfWriter.
    /// </summary>
    /// <returns>the underlying PdfWriter</returns>
    public PdfWriter Writer => _fc;

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfEncryptionSettings#setEncryption(byte[], byte[], int, int)
    /// </summary>
    public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType)
    {
        _fc.SetEncryption(userPassword, ownerPassword, permissions, encryptionType);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfEncryptionSettings#setEncryption(java.security.cert.Certificate[], int[],
    ///     int)
    /// </summary>
    public void SetEncryption(X509Certificate[] certs, int[] permissions, int encryptionType)
    {
        _fc.SetEncryption(certs, permissions, encryptionType);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#setViewerPreferences(int)
    /// </summary>
    public int ViewerPreferences
    {
        set => _fc.ViewerPreferences = value;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#addViewerPreference(com.lowagie.text.pdf.PdfName,
    ///     com.lowagie.text.pdf.PdfObject)
    /// </summary>
    public void AddViewerPreference(PdfName key, PdfObject value)
    {
        _fc.AddViewerPreference(key, value);
    }

    /// <summary>
    ///     Concatenates a PDF document.
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="reader">the PDF document</param>
    public void AddDocument(PdfReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        _fc.AddDocument(reader);
    }

    /// <summary>
    ///     Concatenates a PDF document selecting the pages to keep. The pages are described as a
    ///     List  of  Integer . The page ordering can be changed but
    ///     no page repetitions are allowed.
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="reader">the PDF document</param>
    /// <param name="pagesToKeep">the pages to keep</param>
    public void AddDocument(PdfReader reader, ICollection<int> pagesToKeep)
    {
        _fc.AddDocument(reader, pagesToKeep);
    }

    /// <summary>
    ///     Concatenates a PDF document selecting the pages to keep. The pages are described as
    ///     ranges. The page ordering can be changed but
    ///     no page repetitions are allowed.
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="reader">the PDF document</param>
    /// <param name="ranges">the comma separated ranges as described in {@link SequenceList}</param>
    public void AddDocument(PdfReader reader, string ranges)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        _fc.AddDocument(reader, SequenceList.Expand(ranges, reader.NumberOfPages));
    }

    /// <summary>
    ///     Adds JavaScript to the global document
    /// </summary>
    /// <param name="js">the JavaScript</param>
    public void AddJavaScript(string js)
    {
        _fc.AddJavaScript(js, !PdfEncodings.IsPdfDocEncoding(js));
    }

    /// <summary>
    ///     Closes the output document.
    /// </summary>
    public void Close()
    {
        _fc.Close();
    }

    /// <summary>
    ///     Copies the form fields of this PDFDocument onto the PDF-Document which was added
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="reader">the PDF document</param>
    public void CopyDocumentFields(PdfReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        _fc.CopyDocumentFields(reader);
    }

    /// <summary>
    ///     Opens the document. This is usually not needed as addDocument() will do it
    ///     automatically.
    /// </summary>
    public void Open()
    {
        _fc.OpenDoc();
    }

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="strength128Bits"> true  for 128 bit key length,  false  for 40 bit key length</param>
    public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, bool strength128Bits)
    {
        _fc.SetEncryption(userPassword, ownerPassword, permissions,
                          strength128Bits ? PdfWriter.STANDARD_ENCRYPTION_128 : PdfWriter.STANDARD_ENCRYPTION_40);
    }

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="strength">true for 128 bit key length. false for 40 bit key length</param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    public void SetEncryption(bool strength, string userPassword, string ownerPassword, int permissions)
    {
        SetEncryption(DocWriter.GetIsoBytes(userPassword), DocWriter.GetIsoBytes(ownerPassword), permissions, strength);
    }

    /// <summary>
    ///     Sets the document's compression to the new 1.5 mode with object streams and xref
    ///     streams. It can be set at any time but once set it can't be unset.
    ///     If set before opening the document it will also set the pdf version to 1.5.
    /// </summary>
    public void SetFullCompression()
    {
        _fc.SetFullCompression();
    }
}