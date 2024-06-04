using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
///     A signature dictionary representation for the standard filters.
/// </summary>
public abstract class PdfSigGenericPkcs : PdfSignature
{
    private string _digestEncryptionAlgorithm;
    private byte[] _externalDigest;
    private byte[] _externalRsAdata;

    /// <summary>
    ///     The hash algorith, for example "SHA1"
    /// </summary>
    protected string HashAlgorithm;

    /// <summary>
    ///     The subject name in the signing certificate (the element "CN")
    /// </summary>
    protected string name;

    /// <summary>
    ///     The class instance that calculates the PKCS#1 and PKCS#7
    /// </summary>
    protected PdfPkcs7 Pkcs;

    /// <summary>
    ///     Creates a generic standard filter.
    /// </summary>
    /// <param name="filter">the filter name</param>
    /// <param name="subFilter">the sub-filter name</param>
    protected PdfSigGenericPkcs(PdfName filter, PdfName subFilter) : base(filter, subFilter)
    {
    }

    /// <summary>
    ///     Gets the subject name in the signing certificate (the element "CN")
    /// </summary>
    /// <returns>the subject name in the signing certificate (the element "CN")</returns>
    public new string Name => name;

    /// <summary>
    ///     Gets the class instance that does the actual signing.
    /// </summary>
    /// <returns>the class instance that does the actual signing</returns>
    public PdfPkcs7 Signer => Pkcs;

    /// <summary>
    ///     Gets the signature content. This can be a PKCS#1 or a PKCS#7. It corresponds to
    ///     the /Contents key.
    /// </summary>
    /// <returns>the signature content</returns>
    public byte[] SignerContents
    {
        get
        {
            if (PdfName.AdbeX509RsaSha1.Equals(Get(PdfName.Subfilter)))
            {
                return Pkcs.GetEncodedPkcs1();
            }

            return Pkcs.GetEncodedPkcs7();
        }
    }

    /// <summary>
    ///     Sets the digest/signature to an external calculated value.
    ///     is also  null . If the  digest  is not  null
    ///     then it may be "RSA" or "DSA"
    /// </summary>
    /// <param name="digest">the digest. This is the actual signature</param>
    /// <param name="rsAdata">the extra data that goes into the data tag in PKCS#7</param>
    /// <param name="digestEncryptionAlgorithm">the encryption algorithm. It may must be  null  if the  digest </param>
    public void SetExternalDigest(byte[] digest, byte[] rsAdata, string digestEncryptionAlgorithm)
    {
        _externalDigest = digest;
        _externalRsAdata = rsAdata;
        _digestEncryptionAlgorithm = digestEncryptionAlgorithm;
    }

    /// <summary>
    ///     Sets the crypto information to sign.
    /// </summary>
    /// <param name="privKey">the private key</param>
    /// <param name="certChain">the certificate chain</param>
    /// <param name="crlList">the certificate revocation list. It can be  null </param>
    public void SetSignInfo(ICipherParameters privKey, X509Certificate[] certChain, object[] crlList)
    {
        Pkcs = new PdfPkcs7(privKey, certChain, crlList, HashAlgorithm,
                            PdfName.AdbePkcs7Sha1.Equals(Get(PdfName.Subfilter)));
        Pkcs.SetExternalDigest(_externalDigest, _externalRsAdata, _digestEncryptionAlgorithm);
        if (PdfName.AdbeX509RsaSha1.Equals(Get(PdfName.Subfilter)))
        {
            using var bout = new MemoryStream();
            for (var k = 0; k < certChain.Length; ++k)
            {
                var tmp = certChain[k].GetEncoded();
                bout.Write(tmp, 0, tmp.Length);
            }

            Cert = bout.ToArray();
            Contents = Pkcs.GetEncodedPkcs1();
        }
        else
        {
            Contents = Pkcs.GetEncodedPkcs7();
        }

        name = PdfPkcs7.GetSubjectFields(Pkcs.SigningCertificate).GetField("CN");
        if (name != null)
        {
            Put(PdfName.Name, new PdfString(name, TEXT_UNICODE));
        }

        Pkcs = new PdfPkcs7(privKey, certChain, crlList, HashAlgorithm,
                            PdfName.AdbePkcs7Sha1.Equals(Get(PdfName.Subfilter)));
        Pkcs.SetExternalDigest(_externalDigest, _externalRsAdata, _digestEncryptionAlgorithm);
    }

    /// <summary>
    ///     Creates a standard filter of the type self signed.
    /// </summary>
    public class PpkLite : PdfSigGenericPkcs
    {
        /// <summary>
        ///     The constructor for the default provider.
        /// </summary>
        public PpkLite() : base(PdfName.AdobePpklite, PdfName.AdbeX509RsaSha1)
        {
            HashAlgorithm = "SHA1";
            Put(PdfName.R, new PdfNumber(65541));
        }
    }

    /// <summary>
    ///     Creates a standard filter of the type Windows Certificate.
    /// </summary>
    public class Ppkms : PdfSigGenericPkcs
    {
        /// <summary>
        ///     The constructor for the default provider.
        /// </summary>
        public Ppkms() : base(PdfName.AdobePpkms, PdfName.AdbePkcs7Sha1) => HashAlgorithm = "SHA1";
    }

    /// <summary>
    ///     Creates a standard filter of the type VeriSign.
    /// </summary>
    public class VeriSign : PdfSigGenericPkcs
    {
        /// <summary>
        ///     The constructor for the default provider.
        /// </summary>
        public VeriSign() : base(PdfName.VerisignPpkvs, PdfName.AdbePkcs7Detached)
        {
            HashAlgorithm = "MD5";
            Put(PdfName.R, new PdfNumber(65537));
        }
    }
}