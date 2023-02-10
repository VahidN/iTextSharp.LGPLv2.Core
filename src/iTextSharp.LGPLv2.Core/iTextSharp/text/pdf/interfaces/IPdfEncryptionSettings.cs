using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf.interfaces;

/// <summary>
///     Encryption settings are described in section 3.5 (more specifically
///     section 3.5.2) of the PDF Reference 1.7.
///     They are explained in section 3.3.3 of the book 'iText in Action'.
///     The values of the different  preferences were originally stored
///     in class PdfWriter, but they have been moved to this separate interface
///     for reasons of convenience.
/// </summary>
public interface IPdfEncryptionSettings
{
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
    void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType);

    /// <summary>
    ///     Sets the certificate encryption options for this document. An array of one or more public certificates
    ///     must be provided together with an array of the same size for the permissions for each certificate.
    ///     The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="certs">the public certificates to be used for the encryption</param>
    /// <param name="permissions">the user permissions for each of the certicates</param>
    /// <param name="encryptionType">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128
    ///     or ENCRYPTION_AES128.
    /// </param>
    void SetEncryption(X509Certificate[] certs, int[] permissions, int encryptionType);
}