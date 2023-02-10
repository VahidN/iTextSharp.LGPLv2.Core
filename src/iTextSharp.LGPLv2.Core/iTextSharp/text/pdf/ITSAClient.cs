namespace iTextSharp.text.pdf;

/// <summary>
///     Time Stamp Authority client (caller) interface.
///     Interface used by the PdfPKCS7 digital signature builder to call
///     Time Stamp Authority providing RFC 3161 compliant time stamp token.
///     @author Martin Brunecky, 07/17/2007
///     @since    2.1.6
/// </summary>
public interface ITsaClient
{
    /// <summary>
    ///     Get RFC 3161 timeStampToken.
    ///     Method may return null indicating that timestamp should be skipped.
    ///     @throws Exception - TSA request failed
    /// </summary>
    /// <param name="caller">PdfPKCS7 - calling PdfPKCS7 instance (in case caller needs it)</param>
    /// <param name="imprint">byte[] - data imprint to be time-stamped</param>
    /// <returns>byte[] - encoded, TSA signed data of the timeStampToken</returns>
    byte[] GetTimeStampToken(PdfPkcs7 caller, byte[] imprint);

    /// <summary>
    ///     Get the time stamp token size estimate.
    ///     Implementation must return value large enough to accomodate the entire token
    ///     returned by getTimeStampToken() _prior_ to actual getTimeStampToken() call.
    /// </summary>
    /// <returns>an estimate of the token size</returns>
    int GetTokenSizeEstimate();
}