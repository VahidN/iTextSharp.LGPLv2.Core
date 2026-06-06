namespace iTextSharp.text.pdf.security;

using Org.BouncyCastle.X509;

/// <summary>
///     Interface for obtaining Certificate Revocation Lists (CRLs)
///     to embed in PDF signatures.
/// </summary>
public interface ICrlClient
{
    /// <summary>
    ///     Gets a collection of byte arrays, each representing a CRL.
    /// </summary>
    /// <param name="checkCert">the certificate from which CRL URLs may be obtained</param>
    /// <param name="url">a CRL URL, or null to derive from the certificate</param>
    /// <returns>a collection of CRL byte arrays, or null/empty</returns>
    ICollection<byte[]> GetEncoded(X509Certificate checkCert, string url);
}
