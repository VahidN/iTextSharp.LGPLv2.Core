using System.util;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.pdf.interfaces;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
///     Applies extra content to the pages of a PDF document.
///     This extra content can be all the objects allowed in PdfContentByte
///     including pages from other Pdfs. The original PDF will keep
///     all the interactive elements including bookmarks, links and form fields.
///     It is also possible to change the field values and to
///     flatten them. New fields can be added but not flattened.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfStamper : IPdfViewerPreferences, IPdfEncryptionSettings, IDisposable
{
    private bool _hasSignature;

    /// <summary>
    ///     The writer
    /// </summary>
    protected PdfStamperImp Stamper;

    /// <summary>
    ///     Starts the process of adding extra content to an existing PDF
    ///     document.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the original document. It cannot be reused</param>
    /// <param name="os">the output stream</param>
    public PdfStamper(PdfReader reader, Stream os)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        Stamper = new PdfStamperImp(reader, os, '\0', false);
    }

    /// <summary>
    ///     Starts the process of adding extra content to an existing PDF
    ///     document.
    ///     document
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the original document. It cannot be reused</param>
    /// <param name="os">the output stream</param>
    /// <param name="pdfVersion">the new pdf version or '\0' to keep the same version as the original</param>
    public PdfStamper(PdfReader reader, Stream os, char pdfVersion)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        Stamper = new PdfStamperImp(reader, os, pdfVersion, false);
    }

    /// <summary>
    ///     Starts the process of adding extra content to an existing PDF
    ///     document, possibly as a new revision.
    ///     document
    ///     only useful for multiple signatures as nothing is gained in speed or memory
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the original document. It cannot be reused</param>
    /// <param name="os">the output stream</param>
    /// <param name="pdfVersion">the new pdf version or '\0' to keep the same version as the original</param>
    /// <param name="append">if  true  appends the document changes as a new revision. This is</param>
    public PdfStamper(PdfReader reader, Stream os, char pdfVersion, bool append)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        Stamper = new PdfStamperImp(reader, os, pdfVersion, append);
    }

    /// <summary>
    ///     Gets the  AcroFields  object that allows to get and set field values
    ///     and to merge FDF forms.
    /// </summary>
    /// <returns>the  AcroFields  object</returns>
    public AcroFields AcroFields => Stamper.AcroFields;

    /// <summary>
    ///     Determines if the fields are flattened on close. The fields added with
    ///     {@link #addAnnotation(PdfAnnotation,int)} will never be flattened.
    ///     to keep the fields
    /// </summary>
    public bool FormFlattening
    {
        set => Stamper.FormFlattening = value;
    }

    /// <summary>
    ///     Determines if the FreeText annotations are flattened on close.
    ///     (the default) to keep the FreeText annotations as active content.
    /// </summary>
    public bool FreeTextFlattening
    {
        set => Stamper.FreeTextFlattening = value;
    }

    /// <summary>
    ///     Gets the 1.5 compression status.
    /// </summary>
    /// <returns> true  if the 1.5 compression is on</returns>
    public bool FullCompression => Stamper.FullCompression;

    /// <summary>
    ///     Adds a JavaScript action at the document level. When the document
    ///     opens all this JavaScript runs. The existing JavaScript will be replaced.
    /// </summary>
    public string JavaScript
    {
        set => Stamper.AddJavaScript(value, !PdfEncodings.IsPdfDocEncoding(value));
    }

    /// <summary>
    ///     Gets the optional  String  map to add or change values in
    ///     the info dictionary.
    /// </summary>
    /// <returns>the map or  null </returns>
    /// <summary>
    ///     An optional  String  map to add or change values in
    ///     the info dictionary. Entries with  null
    ///     values delete the key in the original info dictionary
    /// </summary>
    public INullValueDictionary<string, string> MoreInfo { set; get; }

    /// <summary>
    ///     Sets the bookmarks. The list structure is defined in
    ///     {@link SimpleBookmark}.
    /// </summary>
    public IList<INullValueDictionary<string, object>> Outlines
    {
        set => Stamper.Outlines = value;
    }

    /// <summary>
    ///     Gets the underlying PdfReader.
    /// </summary>
    /// <returns>the underlying PdfReader</returns>
    public PdfReader Reader => Stamper.Reader;

    /// <summary>
    ///     Checks if the content is automatically adjusted to compensate
    ///     the original page rotation.
    /// </summary>
    /// <returns>the auto-rotation status</returns>
    /// <summary>
    ///     Flags the content to be automatically adjusted to compensate
    ///     the original page rotation. The default is  true .
    ///     otherwise
    /// </summary>
    public bool RotateContents
    {
        set => Stamper.RotateContents = value;
        get => Stamper.RotateContents;
    }

    /// <summary>
    ///     Gets the signing instance. The appearances and other parameters can the be set.
    /// </summary>
    /// <returns>the signing instance</returns>
    public PdfSignatureAppearance SignatureAppearance { get; private set; }

    /// <summary>
    ///     Gets the underlying PdfWriter.
    /// </summary>
    /// <returns>the underlying PdfWriter</returns>
    public PdfWriter Writer => Stamper;

    /// <summary>
    ///     Sets the XMP metadata.
    ///     @see PdfWriter#setXmpMetadata(byte[])
    /// </summary>
    public byte[] XmpMetadata
    {
        set => Stamper.XmpMetadata = value;
    }

    public void Dispose()
    {
        Close();
    }

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="encryptionType">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128
    ///     or ENCRYPTION_AES128.
    /// </param>
    public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType)
    {
        if (Stamper.IsAppend())
        {
            throw new DocumentException("Append mode does not support changing the encryption status.");
        }

        if (Stamper.ContentWritten)
        {
            throw new DocumentException("Content was already written to the output.");
        }

        Stamper.SetEncryption(userPassword, ownerPassword, permissions, encryptionType);
    }

    /// <summary>
    ///     Sets the certificate encryption options for this document. An array of one or more public certificates
    ///     must be provided together with an array of the same size for the permissions for each certificate.
    ///     The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     @throws DocumentException if the encryption was set too late
    /// </summary>
    /// <param name="certs">the public certificates to be used for the encryption</param>
    /// <param name="permissions">the user permissions for each of the certicates</param>
    /// <param name="encryptionType">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128
    ///     or ENCRYPTION_AES128.
    /// </param>
    public void SetEncryption(X509Certificate[] certs, int[] permissions, int encryptionType)
    {
        if (Stamper.IsAppend())
        {
            throw new DocumentException("Append mode does not support changing the encryption status.");
        }

        if (Stamper.ContentWritten)
        {
            throw new DocumentException("Content was already written to the output.");
        }

        Stamper.SetEncryption(certs, permissions, encryptionType);
    }

    /// <summary>
    ///     Sets the viewer preferences.
    ///     @see PdfViewerPreferences#setViewerPreferences(int)
    /// </summary>
    public virtual int ViewerPreferences
    {
        set => Stamper.ViewerPreferences = value;
    }

    public virtual void AddViewerPreference(PdfName key, PdfObject value)
    {
        Stamper.AddViewerPreference(key, value);
    }

    /// <summary>
    ///     Applies a digital signature to a document, possibly as a new revision, making
    ///     possible multiple signatures. The returned PdfStamper
    ///     can be used normally as the signature is only applied when closing.
    ///     A possible use for adding a signature without invalidating an existing one is:
    ///     KeyStore ks = KeyStore.getInstance("pkcs12");
    ///     ks.load(new FileInputStream("my_private_key.pfx"), "my_password".toCharArray());
    ///     String alias = (String)ks.aliases().nextElement();
    ///     PrivateKey key = (PrivateKey)ks.getKey(alias, "my_password".toCharArray());
    ///     Certificate[] chain = ks.getCertificateChain(alias);
    ///     PdfReader reader = new PdfReader("original.pdf");
    ///     FileOutputStream fout = new FileOutputStream("signed.pdf");
    ///     PdfStamper stp = PdfStamper.createSignature(reader, fout, '\0', new
    ///     File("/temp"), true);
    ///     PdfSignatureAppearance sap = stp.getSignatureAppearance();
    ///     sap.setCrypto(key, chain, null, PdfSignatureAppearance.WINCER_SIGNED);
    ///     sap.setReason("I'm the author");
    ///     sap.setLocation("Lisbon");
    ///     // comment next line to have an invisible signature
    ///     sap.setVisibleSignature(new Rectangle(100, 100, 200, 200), 1, null);
    ///     stp.close();
    ///     document
    ///     If it's a file it will be used directly. The file will be deleted on exit unless  os  is null.
    ///     In that case the document can be retrieved directly from the temporary file. If it's  null
    ///     no temporary file will be created and memory will be used
    ///     new revision thus not invalidating existing signatures
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the original document</param>
    /// <param name="os">the output stream or  null  to keep the document in the temporary file</param>
    /// <param name="pdfVersion">the new pdf version or '\0' to keep the same version as the original</param>
    /// <param name="tempFile">location of the temporary file. If it's a directory a temporary file will be created there.</param>
    /// <param name="append">if  true  the signature and all the other content will be added as a</param>
    /// <returns>a  PdfStamper </returns>
    public static PdfStamper CreateSignature(PdfReader reader, Stream os, char pdfVersion, string tempFile, bool append)
    {
        PdfStamper stp;
        if (tempFile == null)
        {
            var bout = new ByteBuffer();
            stp = new PdfStamper(reader, bout, pdfVersion, append);
            stp.SignatureAppearance = new PdfSignatureAppearance(stp.Stamper);
            stp.SignatureAppearance.Sigout = bout;
        }
        else
        {
            if (Directory.Exists(tempFile))
            {
                tempFile = Path.GetTempFileName();
            }

            var fout = new FileStream(tempFile, FileMode.Create, FileAccess.Write);
            stp = new PdfStamper(reader, fout, pdfVersion, append);
            stp.SignatureAppearance = new PdfSignatureAppearance(stp.Stamper);
            stp.SignatureAppearance.SetTempFile(tempFile);
        }

        stp.SignatureAppearance.Originalout = os;
        stp.SignatureAppearance.SetStamper(stp);
        stp._hasSignature = true;
        var catalog = reader.Catalog;
        var acroForm = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Acroform), catalog);
        if (acroForm != null)
        {
            acroForm.Remove(PdfName.Needappearances);
            stp.Stamper.MarkUsed(acroForm);
        }

        return stp;
    }

    /// <summary>
    ///     Applies a digital signature to a document. The returned PdfStamper
    ///     can be used normally as the signature is only applied when closing.
    ///     Note that the pdf is created in memory.
    ///     A possible use is:
    ///     KeyStore ks = KeyStore.getInstance("pkcs12");
    ///     ks.load(new FileInputStream("my_private_key.pfx"), "my_password".toCharArray());
    ///     String alias = (String)ks.aliases().nextElement();
    ///     PrivateKey key = (PrivateKey)ks.getKey(alias, "my_password".toCharArray());
    ///     Certificate[] chain = ks.getCertificateChain(alias);
    ///     PdfReader reader = new PdfReader("original.pdf");
    ///     FileOutputStream fout = new FileOutputStream("signed.pdf");
    ///     PdfStamper stp = PdfStamper.createSignature(reader, fout, '\0');
    ///     PdfSignatureAppearance sap = stp.getSignatureAppearance();
    ///     sap.setCrypto(key, chain, null, PdfSignatureAppearance.WINCER_SIGNED);
    ///     sap.setReason("I'm the author");
    ///     sap.setLocation("Lisbon");
    ///     // comment next line to have an invisible signature
    ///     sap.setVisibleSignature(new Rectangle(100, 100, 200, 200), 1, null);
    ///     stp.close();
    ///     document
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the original document</param>
    /// <param name="os">the output stream</param>
    /// <param name="pdfVersion">the new pdf version or '\0' to keep the same version as the original</param>
    /// <returns>a  PdfStamper </returns>
    public static PdfStamper CreateSignature(PdfReader reader, Stream os, char pdfVersion) =>
        CreateSignature(reader, os, pdfVersion, null, false);

    /// <summary>
    ///     Applies a digital signature to a document. The returned PdfStamper
    ///     can be used normally as the signature is only applied when closing.
    ///     A possible use is:
    ///     KeyStore ks = KeyStore.getInstance("pkcs12");
    ///     ks.load(new FileInputStream("my_private_key.pfx"), "my_password".toCharArray());
    ///     String alias = (String)ks.aliases().nextElement();
    ///     PrivateKey key = (PrivateKey)ks.getKey(alias, "my_password".toCharArray());
    ///     Certificate[] chain = ks.getCertificateChain(alias);
    ///     PdfReader reader = new PdfReader("original.pdf");
    ///     FileOutputStream fout = new FileOutputStream("signed.pdf");
    ///     PdfStamper stp = PdfStamper.createSignature(reader, fout, '\0', new File("/temp"));
    ///     PdfSignatureAppearance sap = stp.getSignatureAppearance();
    ///     sap.setCrypto(key, chain, null, PdfSignatureAppearance.WINCER_SIGNED);
    ///     sap.setReason("I'm the author");
    ///     sap.setLocation("Lisbon");
    ///     // comment next line to have an invisible signature
    ///     sap.setVisibleSignature(new Rectangle(100, 100, 200, 200), 1, null);
    ///     stp.close();
    ///     document
    ///     If it's a file it will be used directly. The file will be deleted on exit unless  os  is null.
    ///     In that case the document can be retrieved directly from the temporary file. If it's  null
    ///     no temporary file will be created and memory will be used
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the original document</param>
    /// <param name="os">the output stream or  null  to keep the document in the temporary file</param>
    /// <param name="pdfVersion">the new pdf version or '\0' to keep the same version as the original</param>
    /// <param name="tempFile">location of the temporary file. If it's a directory a temporary file will be created there.</param>
    /// <returns>a  PdfStamper </returns>
    public static PdfStamper CreateSignature(PdfReader reader, Stream os, char pdfVersion, string tempFile) =>
        CreateSignature(reader, os, pdfVersion, tempFile, false);

    /// <summary>
    ///     Adds an annotation of form field in a specific page. This page number
    ///     can be overridden with {@link PdfAnnotation#setPlaceInPage(int)}.
    /// </summary>
    /// <param name="annot">the annotation</param>
    /// <param name="page">the page</param>
    public void AddAnnotation(PdfAnnotation annot, int page)
    {
        Stamper.AddAnnotation(annot, page);
    }

    /// <summary>
    ///     Adds the comments present in an FDF file.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="fdf">the FDF file</param>
    public void AddComments(FdfReader fdf)
    {
        Stamper.AddComments(fdf);
    }

    /// <summary>
    ///     Adds a file attachment at the document level. Existing attachments will be kept.
    ///     the file will be read from the disk
    ///     fileStore  is not  null
    ///     @throws IOException on error
    /// </summary>
    /// <param name="description">the file description</param>
    /// <param name="fileStore">an array with the file. If it's  null </param>
    /// <param name="file">the path to the file. It will only be used if</param>
    /// <param name="fileDisplay">the actual file name stored in the pdf</param>
    public void AddFileAttachment(string description, byte[] fileStore, string file, string fileDisplay)
    {
        AddFileAttachment(description, PdfFileSpecification.FileEmbedded(Stamper, file, fileDisplay, fileStore));
    }

    /// <summary>
    ///     Adds a file attachment at the document level. Existing attachments will be kept.
    /// </summary>
    /// <param name="description">the file description</param>
    /// <param name="fs">the file specification</param>
    public void AddFileAttachment(string description, PdfFileSpecification fs)
    {
        Stamper.AddFileAttachment(description, fs);
    }

    /// <summary>
    ///     Adds an empty signature.
    ///     @since    2.1.4
    /// </summary>
    /// <param name="name">the name of the signature</param>
    /// <param name="page">the page number</param>
    /// <param name="llx">lower left x coordinate of the signature's position</param>
    /// <param name="lly">lower left y coordinate of the signature's position</param>
    /// <param name="urx">upper right x coordinate of the signature's position</param>
    /// <param name="ury">upper right y coordinate of the signature's position</param>
    /// <returns>a signature form field</returns>
    public PdfFormField AddSignature(string name, int page, float llx, float lly, float urx, float ury)
    {
        var acroForm = Stamper.AcroForm;
        var signature = PdfFormField.CreateSignature(Stamper);
        PdfAcroForm.SetSignatureParams(signature, name, llx, lly, urx, ury);
        acroForm.DrawSignatureAppearences(signature, llx, lly, urx, ury);
        AddAnnotation(signature, page);
        return signature;
    }

    /// <summary>
    ///     Closes the document. No more content can be written after the
    ///     document is closed.
    ///     If closing a signed document with an external signature the closing must be done
    ///     in the  PdfSignatureAppearance  instance.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    public void Close()
    {
        if (!_hasSignature)
        {
            Stamper.Close(MoreInfo);
            return;
        }

        SignatureAppearance.PreClose();
        var sig = SignatureAppearance.SigStandard;
        var lit = (PdfLiteral)sig.Get(PdfName.Contents);
        var totalBuf = (lit.PosLength - 2) / 2;
        var buf = new byte[8192];
        int n;
        var inp = SignatureAppearance.RangeStream;
        while ((n = inp.Read(buf, 0, buf.Length)) > 0)
        {
            sig.Signer.Update(buf, 0, n);
        }

        buf = new byte[totalBuf];
        var bsig = sig.SignerContents;
        Array.Copy(bsig, 0, buf, 0, bsig.Length);
        var str = new PdfString(buf);
        str.SetHexWriting(true);
        var dic = new PdfDictionary();
        dic.Put(PdfName.Contents, str);
        SignatureAppearance.Close(dic);
        Stamper.Reader.Close();
    }

    /// <summary>
    ///     Gets a page from other PDF document. Note that calling this method more than
    ///     once with the same parameters will retrieve the same object.
    /// </summary>
    /// <param name="reader">the PDF document where the page is</param>
    /// <param name="pageNumber">the page number. The first page is 1</param>
    /// <returns>the template representing the imported page</returns>
    public PdfImportedPage GetImportedPage(PdfReader reader, int pageNumber) =>
        Stamper.GetImportedPage(reader, pageNumber);

    /// <summary>
    ///     Gets a  PdfContentByte  to write over the page of
    ///     the original document.
    ///     the original document
    /// </summary>
    /// <param name="pageNum">the page number where the extra content is written</param>
    /// <returns>a  PdfContentByte  to write over the page of</returns>
    public PdfContentByte GetOverContent(int pageNum) => Stamper.GetOverContent(pageNum);

    /// <summary>
    ///     Gets the PdfLayer objects in an existing document as a Map
    ///     with the names/titles of the layers as keys.
    ///     @since    2.1.2
    /// </summary>
    /// <returns>a Map with all the PdfLayers in the document (and the name/title of the layer as key)</returns>
    public INullValueDictionary<string, PdfLayer> GetPdfLayers() => Stamper.GetPdfLayers();

    /// <summary>
    ///     Gets a  PdfContentByte  to write under the page of
    ///     the original document.
    ///     the original document
    /// </summary>
    /// <param name="pageNum">the page number where the extra content is written</param>
    /// <returns>a  PdfContentByte  to write under the page of</returns>
    public PdfContentByte GetUnderContent(int pageNum) => Stamper.GetUnderContent(pageNum);

    /// <summary>
    ///     Inserts a blank page. All the pages above and including  pageNumber  will
    ///     be shifted up. If  pageNumber  is bigger than the total number of pages
    ///     the new page will be the last one.
    /// </summary>
    /// <param name="pageNumber">the page number position where the new page will be inserted</param>
    /// <param name="mediabox">the size of the new page</param>
    public void InsertPage(int pageNumber, Rectangle mediabox)
    {
        Stamper.InsertPage(pageNumber, mediabox);
    }

    /// <summary>
    ///     This is the most simple way to change a PDF into a
    ///     portable collection. Choose one of the following names:
    ///     PdfName.D (detailed view)
    ///     PdfName.T (tiled view)
    ///     PdfName.H (hidden)
    ///     Pass this name as a parameter and your PDF will be
    ///     a portable collection with all the embedded and
    ///     attached files as entries.
    /// </summary>
    /// <param name="initialView">can be PdfName.D, PdfName.T or PdfName.H</param>
    public void MakePackage(PdfName initialView)
    {
        var collection = new PdfCollection(0);
        collection.Put(PdfName.View, initialView);
        Stamper.MakePackage(collection);
    }

    /// <summary>
    ///     Adds or replaces the Collection Dictionary in the Catalog.
    /// </summary>
    /// <param name="collection">the new collection dictionary.</param>
    public void MakePackage(PdfCollection collection)
    {
        Stamper.MakePackage(collection);
    }

    /// <summary>
    ///     Adds  name  to the list of fields that will be flattened on close,
    ///     all the other fields will remain. If this method is never called or is called
    ///     with invalid field names, all the fields will be flattened.
    ///     Calling  setFormFlattening(true)  is needed to have any kind of
    ///     flattening.
    /// </summary>
    /// <param name="name">the field name</param>
    /// <returns> true  if the field exists,  false  otherwise</returns>
    public bool PartialFormFlattening(string name) => Stamper.PartialFormFlattening(name);

    /// <summary>
    ///     Replaces a page from this document with a page from other document. Only the content
    ///     is replaced not the fields and annotations. This method must be called before
    ///     getOverContent() or getUndercontent() are called for the same page.
    /// </summary>
    /// <param name="r">the  PdfReader  from where the new page will be imported</param>
    /// <param name="pageImported">the page number of the imported page</param>
    /// <param name="pageReplaced">the page to replace in this document</param>
    public void ReplacePage(PdfReader r, int pageImported, int pageReplaced)
    {
        if (r == null)
        {
            throw new ArgumentNullException(nameof(r));
        }

        Stamper.ReplacePage(r, pageImported, pageReplaced);
    }

    /// <summary>
    ///     Sets the display duration for the page (for presentations)
    /// </summary>
    /// <param name="seconds">the number of seconds to display the page. A negative value removes the entry</param>
    /// <param name="page">the page where the duration will be applied. The first page is 1</param>
    public void SetDuration(int seconds, int page)
    {
        Stamper.SetDuration(seconds, page);
    }

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     @throws DocumentException if anything was already written to the output
    /// </summary>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="strength128Bits"> true  for 128 bit key length,  false  for 40 bit key length</param>
    public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, bool strength128Bits)
    {
        if (Stamper.Append)
        {
            throw new DocumentException("Append mode does not support changing the encryption status.");
        }

        if (Stamper.ContentWritten)
        {
            throw new DocumentException("Content was already written to the output.");
        }

        Stamper.SetEncryption(userPassword, ownerPassword, permissions,
                              strength128Bits ? PdfWriter.STANDARD_ENCRYPTION_128 : PdfWriter.STANDARD_ENCRYPTION_40);
    }

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     @throws DocumentException if anything was already written to the output
    /// </summary>
    /// <param name="strength"> true  for 128 bit key length,  false  for 40 bit key length</param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    public void SetEncryption(bool strength, string userPassword, string ownerPassword, int permissions)
    {
        SetEncryption(DocWriter.GetIsoBytes(userPassword), DocWriter.GetIsoBytes(ownerPassword), permissions, strength);
    }

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="encryptionType">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128
    ///     or ENCRYPTION_AES128.
    /// </param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    public void SetEncryption(int encryptionType, string userPassword, string ownerPassword, int permissions)
    {
        SetEncryption(DocWriter.GetIsoBytes(userPassword), DocWriter.GetIsoBytes(ownerPassword), permissions,
                      encryptionType);
    }

    /// <summary>
    ///     Sets the document's compression to the new 1.5 mode with object streams and xref
    ///     streams. It can be set at any time but once set it can't be unset.
    /// </summary>
    public void SetFullCompression()
    {
        if (Stamper.Append)
        {
            return;
        }

        Stamper.SetFullCompression();
    }

    /// <summary>
    ///     Sets the open and close page additional action.
    ///     or  PdfWriter.PAGE_CLOSE
    ///     @throws PdfException if the action type is invalid
    /// </summary>
    /// <param name="actionType">the action type. It can be  PdfWriter.PAGE_OPEN </param>
    /// <param name="action">the action to perform</param>
    /// <param name="page">the page where the action will be applied. The first page is 1</param>
    public void SetPageAction(PdfName actionType, PdfAction action, int page)
    {
        if (actionType == null)
        {
            throw new ArgumentNullException(nameof(actionType));
        }

        Stamper.SetPageAction(actionType, action, page);
    }

    /// <summary>
    ///     Sets the thumbnail image for a page.
    ///     @throws PdfException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="image">the image</param>
    /// <param name="page">the page</param>
    public void SetThumbnail(Image image, int page)
    {
        Stamper.SetThumbnail(image, page);
    }

    /// <summary>
    ///     Sets the transition for the page
    /// </summary>
    /// <param name="transition">the transition object. A  null  removes the transition</param>
    /// <param name="page">the page where the transition will be applied. The first page is 1</param>
    public void SetTransition(PdfTransition transition, int page)
    {
        Stamper.SetTransition(transition, page);
    }
}