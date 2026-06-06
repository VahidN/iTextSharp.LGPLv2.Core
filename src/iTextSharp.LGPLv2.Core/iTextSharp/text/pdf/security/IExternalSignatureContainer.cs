namespace iTextSharp.text.pdf.security;

using iTextSharp.text.pdf;

/// <summary>
///     Interface for externally composed signature containers (PKCS#7).
///     The container is fully built by an external component and then
///     injected into the PDF.
/// </summary>
public interface IExternalSignatureContainer
{
    /// <summary>
    ///     Produces the container with the signature bytes.
    /// </summary>
    /// <param name="data">the data to sign</param>
    /// <returns>a container with the signature (typically PKCS#7 bytes)</returns>
    byte[] Sign(Stream data);

    /// <summary>
    ///     Modifies the signature dictionary to suit the container.
    ///     At minimum, the PdfName.Filter and PdfName.SubFilter keys must be set.
    /// </summary>
    /// <param name="signDic">the signature dictionary to modify</param>
    void ModifySigningDictionary(PdfDictionary signDic);
}
