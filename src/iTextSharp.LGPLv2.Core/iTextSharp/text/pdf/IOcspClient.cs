namespace iTextSharp.text.pdf;

using Org.BouncyCastle.X509;

/// <summary>
///     Interface for the OCSP Client.
///     @since 2.1.6
/// </summary>
public interface IOcspClient
{
    /// <summary>
    ///     Gets an encoded OCSP response byte array.
    /// </summary>
    /// <param name="checkCert">the certificate to check</param>
    /// <param name="issuerCert">the issuer certificate</param>
    /// <param name="url">the OCSP service URL, or null</param>
    /// <returns>an encoded OCSP response, or null</returns>
    byte[] GetEncoded(X509Certificate checkCert, X509Certificate issuerCert, string url);
}