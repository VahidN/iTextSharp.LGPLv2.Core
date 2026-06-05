namespace iTextSharp.text.pdf;

/// <summary>
///     Interface for the OCSP Client.
///     @since 2.1.6
/// </summary>
public interface IOcspClient
{
    /// <summary>
    ///     Gets an encoded byte array.
    /// </summary>
    /// <returns>a byte array</returns>
    byte[] GetEncoded();
}