namespace iTextSharp.text.pdf.security;

/// <summary>
///     Interface for external signature implementations such as
///     smart cards, HSMs, or remote signing services.
/// </summary>
public interface IExternalSignature
{
    /// <summary>
    ///     Returns the hash algorithm used for signing (e.g. "SHA-256").
    /// </summary>
    /// <returns>the hash algorithm name</returns>
    string GetHashAlgorithm();

    /// <summary>
    ///     Returns the encryption algorithm used for signing (e.g. "RSA").
    /// </summary>
    /// <returns>the encryption algorithm name</returns>
    string GetEncryptionAlgorithm();

    /// <summary>
    ///     Signs the given message and returns the signature bytes.
    /// </summary>
    /// <param name="message">the data to sign</param>
    /// <returns>the signature bytes</returns>
    byte[] Sign(byte[] message);
}
