using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace iTextSharp.text.pdf.security;

/// <summary>
///     Implementation of <see cref="IExternalSignature"/> that uses a .NET
///     <see cref="X509Certificate2"/> certificate with a private key.
///     This enables signing with certificates from the Windows certificate store
///     or any X509Certificate2 instance that has an accessible private RSA key.
/// </summary>
public class X509Certificate2Signature : IExternalSignature
{
    private readonly X509Certificate2 _certificate;
    private readonly string _hashAlgorithm;
    private readonly string _encryptionAlgorithm;

    /// <summary>
    ///     Creates a new instance using the specified certificate and hash algorithm.
    ///     The certificate must have an associated private RSA key.
    /// </summary>
    /// <param name="certificate">the X509Certificate2 with a private key</param>
    /// <param name="hashAlgorithm">the hash algorithm (e.g. "SHA-256")</param>
    public X509Certificate2Signature(X509Certificate2 certificate, string hashAlgorithm)
    {
        _certificate = certificate ?? throw new ArgumentNullException(nameof(certificate));
        _hashAlgorithm = DigestAlgorithms.GetDigest(DigestAlgorithms.GetAllowedDigests(
            hashAlgorithm ?? throw new ArgumentNullException(nameof(hashAlgorithm))));
        _encryptionAlgorithm = "RSA";
    }

    /// <summary>
    ///     Returns the hash algorithm (e.g. "SHA256").
    /// </summary>
    /// <returns>the hash algorithm name</returns>
    public string GetHashAlgorithm() => _hashAlgorithm;

    /// <summary>
    ///     Returns the encryption algorithm ("RSA").
    /// </summary>
    /// <returns>"RSA"</returns>
    public string GetEncryptionAlgorithm() => _encryptionAlgorithm;

    /// <summary>
    ///     Signs the message using the certificate's private RSA key.
    ///     The message is first hashed with the specified hash algorithm,
    ///     then the hash is signed with RSA PKCS#1 v1.5.
    /// </summary>
    /// <param name="message">the data to sign (typically DER-encoded authenticated attributes)</param>
    /// <returns>the RSA signature bytes</returns>
    public byte[] Sign(byte[] message)
    {
        var rsa = _certificate.GetRSAPrivateKey() ?? throw new InvalidOperationException(
                "The certificate does not have an associated RSA private key.");
        var hashAlgoName = ResolveHashAlgorithmName(_hashAlgorithm);
        return rsa.SignData(message, hashAlgoName, RSASignaturePadding.Pkcs1);
    }

    private static HashAlgorithmName ResolveHashAlgorithmName(string hashAlgorithm)
    {
        return hashAlgorithm switch
        {
            "SHA1" => HashAlgorithmName.SHA1,
            "SHA256" => HashAlgorithmName.SHA256,
            "SHA384" => HashAlgorithmName.SHA384,
            "SHA512" => HashAlgorithmName.SHA512,
            _ => HashAlgorithmName.SHA256,
        };
    }
}
