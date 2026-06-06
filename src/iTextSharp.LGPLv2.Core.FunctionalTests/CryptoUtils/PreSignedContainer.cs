namespace iTextSharp.LGPLv2.Core.FunctionalTests.CryptoUtils;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

/// <summary>
///     Reusable <see cref="IExternalSignatureContainer"/> for test scenarios.
///     Pre-builds a PKCS#7 container using the internal <see cref="PdfPkcs7"/>,
///     simulating an external signing service.
/// </summary>
public class PreSignedContainer : IExternalSignatureContainer
{
    private readonly ICipherParameters _privateKey;
    private readonly X509Certificate[] _chain;
    private readonly string _hashAlgorithm;

    /// <summary>
    ///     Creates a new instance.
    /// </summary>
    /// <param name="privateKey">the private key</param>
    /// <param name="chain">the certificate chain</param>
    /// <param name="hashAlgorithm">the hash algorithm (e.g. "SHA-256")</param>
    public PreSignedContainer(
        ICipherParameters privateKey,
        X509Certificate[] chain,
        string hashAlgorithm)
    {
        _privateKey = privateKey;
        _chain = chain;
        _hashAlgorithm = hashAlgorithm;
    }

    /// <inheritdoc />
    public void ModifySigningDictionary(PdfDictionary signDic)
    {
        signDic.Put(PdfName.Filter, PdfName.AdobePpklite);
        signDic.Put(PdfName.Subfilter, PdfName.AdbePkcs7Detached);
    }

    /// <inheritdoc />
    public byte[] Sign(global::System.IO.Stream data)
    {
        var sgn = new PdfPkcs7(_privateKey, _chain, null, _hashAlgorithm, false);
        var buf = new byte[8192];
        int n;
        while ((n = data.Read(buf, 0, buf.Length)) > 0)
        {
            sgn.Update(buf, 0, n);
        }

        return sgn.GetEncodedPkcs7();
    }
}
