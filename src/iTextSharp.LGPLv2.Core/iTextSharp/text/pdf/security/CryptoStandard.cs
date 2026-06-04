namespace iTextSharp.text.pdf.security;

/// <summary>
///     Digital signature standard.
///     Specifies the type of signature to create.
/// </summary>
public enum CryptoStandard
{
    /// <summary>
    ///     Cryptographic Message Syntax (CMS).
    ///     Compatible with PDF 1.x. Uses sub-filter adbe.pkcs7.detached.
    /// </summary>
    CMS,

    /// <summary>
    ///     CMS Advanced Electronic Signatures (CAdES).
    ///     PAdES-BES compatible. Uses sub-filter ETSI.CAdES.detached.
    /// </summary>
    CADES,
}
