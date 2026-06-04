namespace iTextSharp.LGPLv2.Core.FunctionalTests.CryptoUtils;

using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

/// <summary>
///     Reusable <see cref="IExternalSignature"/> implementation for test scenarios.
///     Uses BouncyCastle to sign locally, simulating an HSM or smart card.
/// </summary>
public class ExternalSignature : IExternalSignature
{
    private readonly ICipherParameters _privateKey;
    private readonly string _hashAlgorithm;
    private readonly string _encryptionAlgorithm;

    /// <summary>
    ///     Gets the certificate chain.
    /// </summary>
    public X509Certificate[] Chain { get; }

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="privateKey">the private key</param>
    /// <param name="chain">the certificate chain</param>
    /// <param name="hashAlgorithm">the hash algorithm (e.g. "SHA-256")</param>
    /// <param name="encryptionAlgorithm">the encryption algorithm (e.g. "RSA")</param>
    public ExternalSignature(
        ICipherParameters privateKey,
        X509Certificate[] chain,
        string hashAlgorithm,
        string encryptionAlgorithm)
    {
        _privateKey = privateKey;
        Chain = chain;
        _hashAlgorithm = hashAlgorithm;
        _encryptionAlgorithm = encryptionAlgorithm;
    }

    /// <inheritdoc />
    public string GetHashAlgorithm() => _hashAlgorithm;

    /// <inheritdoc />
    public string GetEncryptionAlgorithm() => _encryptionAlgorithm;

    /// <inheritdoc />
    public byte[] Sign(byte[] message)
    {
        var signer = SignerUtilities.GetSigner(_encryptionAlgorithm);
        signer.Init(true, _privateKey);
        signer.BlockUpdate(message, 0, message.Length);
        return signer.GenerateSignature();
    }
}
