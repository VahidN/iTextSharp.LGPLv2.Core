using System.Security.Cryptography;
using System.Text;
using System.util;
using iTextSharp.LGPLv2.Core.System.Encodings;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tsp;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using AttributeTable = Org.BouncyCastle.Asn1.Cms.AttributeTable;
using ContentInfo = Org.BouncyCastle.Asn1.Cms.ContentInfo;

namespace iTextSharp.text.pdf;

/// <summary>
///     This class does all the processing related to signing and verifying a PKCS#7
///     signature.
///     It's based in code found at org.bouncycastle.
/// </summary>
public class PdfPkcs7
{
    private const string IdAdbeRevocation = "1.2.840.113583.1.1.8";
    private const string IdContentType = "1.2.840.113549.1.9.3";
    private const string IdDsa = "1.2.840.10040.4.1";
    private const string IdMessageDigest = "1.2.840.113549.1.9.4";
    private const string IdPkcs7Data = "1.2.840.113549.1.7.1";
    private const string IdPkcs7SignedData = "1.2.840.113549.1.7.2";
    private const string IdRsa = "1.2.840.113549.1.1.1";
    private const string IdSigningTime = "1.2.840.113549.1.9.5";

    private static readonly INullValueDictionary<string, string> _algorithmNames =
        new NullValueDictionary<string, string>();

    private static readonly INullValueDictionary<string, string> _allowedDigests =
        new NullValueDictionary<string, string>();

    private static readonly INullValueDictionary<string, string> _digestNames =
        new NullValueDictionary<string, string>();

    private readonly List<X509Certificate> _certs;
    private readonly string _digestAlgorithm;
    private readonly INullValueDictionary<string, object> _digestalgos = new NullValueDictionary<string, object>();
    private readonly byte[] _digestAttr;
    private readonly IDigest _messageDigest;

    private readonly ICipherParameters _privKey;
    private readonly ISigner _sig;
    private readonly byte[] _sigAttr;
    private byte[] _digest;
    private string _digestEncryptionAlgorithm;
    private byte[] _externalDigest;
    private byte[] _externalRsAdata;

    private byte[] _rsAdata;
    private List<X509Certificate> _signCerts;

    private bool _verified;
    private bool _verifyResult;

    static PdfPkcs7()
    {
        _digestNames["1.2.840.113549.2.5"] = "MD5";
        _digestNames["1.2.840.113549.2.2"] = "MD2";
        _digestNames["1.3.14.3.2.26"] = "SHA1";
        _digestNames["2.16.840.1.101.3.4.2.4"] = "SHA224";
        _digestNames["2.16.840.1.101.3.4.2.1"] = "SHA256";
        _digestNames["2.16.840.1.101.3.4.2.2"] = "SHA384";
        _digestNames["2.16.840.1.101.3.4.2.3"] = "SHA512";
        _digestNames["1.3.36.3.2.2"] = "RIPEMD128";
        _digestNames["1.3.36.3.2.1"] = "RIPEMD160";
        _digestNames["1.3.36.3.2.3"] = "RIPEMD256";
        _digestNames["1.2.840.113549.1.1.4"] = "MD5";
        _digestNames["1.2.840.113549.1.1.2"] = "MD2";
        _digestNames["1.2.840.113549.1.1.5"] = "SHA1";
        _digestNames["1.2.840.113549.1.1.14"] = "SHA224";
        _digestNames["1.2.840.113549.1.1.11"] = "SHA256";
        _digestNames["1.2.840.113549.1.1.12"] = "SHA384";
        _digestNames["1.2.840.113549.1.1.13"] = "SHA512";
        _digestNames["1.2.840.113549.2.5"] = "MD5";
        _digestNames["1.2.840.113549.2.2"] = "MD2";
        _digestNames["1.2.840.10040.4.3"] = "SHA1";
        _digestNames["2.16.840.1.101.3.4.3.1"] = "SHA224";
        _digestNames["2.16.840.1.101.3.4.3.2"] = "SHA256";
        _digestNames["2.16.840.1.101.3.4.3.3"] = "SHA384";
        _digestNames["2.16.840.1.101.3.4.3.4"] = "SHA512";
        _digestNames["1.3.36.3.3.1.3"] = "RIPEMD128";
        _digestNames["1.3.36.3.3.1.2"] = "RIPEMD160";
        _digestNames["1.3.36.3.3.1.4"] = "RIPEMD256";

        _algorithmNames["1.2.840.113549.1.1.1"] = "RSA";
        _algorithmNames["1.2.840.10040.4.1"] = "DSA";
        _algorithmNames["1.2.840.113549.1.1.2"] = "RSA";
        _algorithmNames["1.2.840.113549.1.1.4"] = "RSA";
        _algorithmNames["1.2.840.113549.1.1.5"] = "RSA";
        _algorithmNames["1.2.840.113549.1.1.14"] = "RSA";
        _algorithmNames["1.2.840.113549.1.1.11"] = "RSA";
        _algorithmNames["1.2.840.113549.1.1.12"] = "RSA";
        _algorithmNames["1.2.840.113549.1.1.13"] = "RSA";
        _algorithmNames["1.2.840.10040.4.3"] = "DSA";
        _algorithmNames["2.16.840.1.101.3.4.3.1"] = "DSA";
        _algorithmNames["2.16.840.1.101.3.4.3.2"] = "DSA";
        _algorithmNames["1.3.36.3.3.1.3"] = "RSA";
        _algorithmNames["1.3.36.3.3.1.2"] = "RSA";
        _algorithmNames["1.3.36.3.3.1.4"] = "RSA";

        _allowedDigests["MD5"] = "1.2.840.113549.2.5";
        _allowedDigests["MD2"] = "1.2.840.113549.2.2";
        _allowedDigests["SHA1"] = "1.3.14.3.2.26";
        _allowedDigests["SHA224"] = "2.16.840.1.101.3.4.2.4";
        _allowedDigests["SHA256"] = "2.16.840.1.101.3.4.2.1";
        _allowedDigests["SHA384"] = "2.16.840.1.101.3.4.2.2";
        _allowedDigests["SHA512"] = "2.16.840.1.101.3.4.2.3";
        _allowedDigests["MD-5"] = "1.2.840.113549.2.5";
        _allowedDigests["MD-2"] = "1.2.840.113549.2.2";
        _allowedDigests["SHA-1"] = "1.3.14.3.2.26";
        _allowedDigests["SHA-224"] = "2.16.840.1.101.3.4.2.4";
        _allowedDigests["SHA-256"] = "2.16.840.1.101.3.4.2.1";
        _allowedDigests["SHA-384"] = "2.16.840.1.101.3.4.2.2";
        _allowedDigests["SHA-512"] = "2.16.840.1.101.3.4.2.3";
        _allowedDigests["RIPEMD128"] = "1.3.36.3.2.2";
        _allowedDigests["RIPEMD-128"] = "1.3.36.3.2.2";
        _allowedDigests["RIPEMD160"] = "1.3.36.3.2.1";
        _allowedDigests["RIPEMD-160"] = "1.3.36.3.2.1";
        _allowedDigests["RIPEMD256"] = "1.3.36.3.2.3";
        _allowedDigests["RIPEMD-256"] = "1.3.36.3.2.3";
    }

    /// <summary>
    ///     Verifies a signature using the sub-filter adbe.x509.rsa_sha1.
    /// </summary>
    /// <param name="contentsKey">the /Contents key</param>
    /// <param name="certsKey">the /Cert key</param>
    public PdfPkcs7(byte[] contentsKey, byte[] certsKey)
    {
        var cf = new X509CertificateParser();
        _certs = new List<X509Certificate>();

        foreach (var cc in cf.ReadCertificates(certsKey))
        {
            _certs.Add(cc);
        }

        _signCerts = _certs;
        SigningCertificate = _certs[0];
        CrLs = new List<object>();
        using var memoryStream = new MemoryStream(contentsKey);
        using var inp = new Asn1InputStream(memoryStream);
        _digest = ((DerOctetString)inp.ReadObject()).GetOctets();
        _sig = SignerUtilities.GetSigner("SHA1withRSA");
        _sig.Init(false, SigningCertificate.GetPublicKey());
    }

    /// <summary>
    ///     Verifies a signature using the sub-filter adbe.pkcs7.detached or
    ///     adbe.pkcs7.sha1.
    ///     @throws SecurityException on error
    ///     @throws CRLException on error
    ///     @throws InvalidKeyException on error
    ///     @throws CertificateException on error
    ///     @throws NoSuchProviderException on error
    ///     @throws NoSuchAlgorithmException on error
    /// </summary>
    /// <param name="contentsKey">the /Contents key</param>
    public PdfPkcs7(byte[] contentsKey)
    {
        using var memoryStream = new MemoryStream(contentsKey);
        using var din = new Asn1InputStream(memoryStream);

        //
        // Basic checks to make sure it's a PKCS#7 SignedData Object
        //
        Asn1Object pkcs;

        try
        {
            pkcs = din.ReadObject();
        }
        catch
        {
            throw new ArgumentException("can't decode PKCS7SignedData object");
        }

        if (pkcs is not Asn1Sequence signedData)
        {
            throw new ArgumentException("Not a valid PKCS#7 object - not a sequence");
        }

        var objId = (DerObjectIdentifier)signedData[0];

        if (!objId.Id.Equals(IdPkcs7SignedData, StringComparison.Ordinal))
        {
            throw new ArgumentException("Not a valid PKCS#7 object - not signed data");
        }

        var content = (Asn1Sequence)((DerTaggedObject)signedData[1]).GetBaseObject();

        // the positions that we care are:
        //     0 - version
        //     1 - digestAlgorithms
        //     2 - possible ID_PKCS7_DATA
        //     (the certificates and crls are taken out by other means)
        //     last - signerInfos

        // the version
        Version = ((DerInteger)content[0]).Value.IntValue;

        // the digestAlgorithms
        _digestalgos = new NullValueDictionary<string, object>();
        var e = ((Asn1Set)content[1]).GetEnumerator();

        while (e.MoveNext())
        {
            var s = (Asn1Sequence)e.Current;
            var o = (DerObjectIdentifier)s[0];
            _digestalgos[o.Id] = null;
        }

        // the certificates and crls
        var cf = new X509CertificateParser();
        _certs = new List<X509Certificate>();

        foreach (var cc in cf.ReadCertificates(contentsKey))
        {
            _certs.Add(cc);
        }

        CrLs = new List<object>();

        // the possible ID_PKCS7_DATA
        var rsaData = (Asn1Sequence)content[2];

        if (rsaData.Count > 1)
        {
            var rsaDataContent = (DerOctetString)((DerTaggedObject)rsaData[1]).GetBaseObject();
            _rsAdata = rsaDataContent.GetOctets();
        }

        // the signerInfos
        var next = 3;

        while (content[next] is DerTaggedObject)
        {
            ++next;
        }

        var signerInfos = (Asn1Set)content[next];

        if (signerInfos.Count != 1)
        {
            throw new ArgumentException(
                "This PKCS#7 object has multiple SignerInfos - only one is supported at this time");
        }

        var signerInfo = (Asn1Sequence)signerInfos[0];

        // the positions that we care are
        //     0 - version
        //     1 - the signing certificate serial number
        //     2 - the digest algorithm
        //     3 or 4 - digestEncryptionAlgorithm
        //     4 or 5 - encryptedDigest
        SigningInfoVersion = ((DerInteger)signerInfo[0]).Value.IntValue;

        // Get the signing certificate
        var issuerAndSerialNumber = (Asn1Sequence)signerInfo[1];
        var serialNumber = ((DerInteger)issuerAndSerialNumber[1]).Value;

        foreach (var cert in _certs)
        {
            if (serialNumber.Equals(cert.SerialNumber))
            {
                SigningCertificate = cert;

                break;
            }
        }

        if (SigningCertificate == null)
        {
            throw new ArgumentException("Can't find signing certificate with serial " + serialNumber.ToString(16));
        }

        calcSignCertificateChain();
        _digestAlgorithm = ((DerObjectIdentifier)((Asn1Sequence)signerInfo[2])[0]).Id;
        next = 3;

        if (signerInfo[next] is Asn1TaggedObject tagsig)
        {
            var sseq = Asn1Set.GetInstance(tagsig, false);
            _sigAttr = sseq.GetEncoded(Asn1Encodable.Der);

            for (var k = 0; k < sseq.Count; ++k)
            {
                var seq2 = (Asn1Sequence)sseq[k];

                if (((DerObjectIdentifier)seq2[0]).Id.Equals(IdMessageDigest, StringComparison.Ordinal))
                {
                    var sset = (Asn1Set)seq2[1];
                    _digestAttr = ((DerOctetString)sset[0]).GetOctets();
                }
                else if (((DerObjectIdentifier)seq2[0]).Id.Equals(IdAdbeRevocation, StringComparison.Ordinal))
                {
                    var setout = (Asn1Set)seq2[1];
                    var seqout = (Asn1Sequence)setout[0];

                    for (var j = 0; j < seqout.Count; ++j)
                    {
                        var tg = (Asn1TaggedObject)seqout[j];

                        if (tg.TagNo != 1)
                        {
                            continue;
                        }

                        var seqin = (Asn1Sequence)tg.GetBaseObject();
                        findOcsp(seqin);
                    }
                }
            }

            if (_digestAttr == null)
            {
                throw new ArgumentException("Authenticated attribute is missing the digest.");
            }

            ++next;
        }

        _digestEncryptionAlgorithm = ((DerObjectIdentifier)((Asn1Sequence)signerInfo[next++])[0]).Id;
        _digest = ((DerOctetString)signerInfo[next++]).GetOctets();

        if (next < signerInfo.Count && signerInfo[next] is DerTaggedObject taggedObject)
        {
            var unat = Asn1Set.GetInstance(taggedObject, false);
            var attble = new AttributeTable(unat);
            var ts = attble[PkcsObjectIdentifiers.IdAASignatureTimeStampToken];

            if (ts != null)
            {
                var attributeValues = ts.AttrValues;
                var tokenSequence = Asn1Sequence.GetInstance(attributeValues[0]);
                var contentInfo = ContentInfo.GetInstance(tokenSequence);
                TimeStampToken = new TimeStampToken(contentInfo);
            }
        }

        if (_rsAdata != null || _digestAttr != null)
        {
            _messageDigest = GetHashClass();
        }

        _sig = SignerUtilities.GetSigner(GetDigestAlgorithm());
        _sig.Init(false, SigningCertificate.GetPublicKey());
    }

    /// <summary>
    ///     Generates a signature.
    ///     @throws SecurityException on error
    ///     @throws InvalidKeyException on error
    ///     @throws NoSuchProviderException on error
    ///     @throws NoSuchAlgorithmException on error
    /// </summary>
    /// <param name="privKey">the private key</param>
    /// <param name="certChain">the certificate chain</param>
    /// <param name="crlList">the certificate revocation list</param>
    /// <param name="hashAlgorithm">the hash algorithm</param>
    /// <param name="hasRsAdata"> true  if the sub-filter is adbe.pkcs7.sha1</param>
    public PdfPkcs7(ICipherParameters privKey,
        X509Certificate[] certChain,
        object[] crlList,
        string hashAlgorithm,
        bool hasRsAdata)
    {
        if (certChain == null)
        {
            throw new ArgumentNullException(nameof(certChain));
        }

        if (hashAlgorithm == null)
        {
            throw new ArgumentNullException(nameof(hashAlgorithm));
        }

        _privKey = privKey;

        _digestAlgorithm = _allowedDigests[hashAlgorithm.ToUpper(CultureInfo.InvariantCulture)];

        if (_digestAlgorithm == null)
        {
            throw new ArgumentException("Unknown Hash Algorithm " + hashAlgorithm);
        }

        Version = SigningInfoVersion = 1;
        _certs = new List<X509Certificate>();
        CrLs = new List<object>();
        _digestalgos = new NullValueDictionary<string, object>();
        _digestalgos[_digestAlgorithm] = null;

        //
        // Copy in the certificates and crls used to sign the private key.
        //
        SigningCertificate = certChain[0];

        for (var i = 0; i < certChain.Length; i++)
        {
            _certs.Add(certChain[i]);
        }

        //            if (crlList != null) {
        //                for (int i = 0;i < crlList.length;i++) {
        //                    crls.Add(crlList[i]);
        //                }
        //            }

        if (privKey != null)
        {
            //
            // Now we have private key, find out what the digestEncryptionAlgorithm is.
            //
            if (privKey is RsaKeyParameters)
            {
                _digestEncryptionAlgorithm = IdRsa;
            }
            else if (privKey is DsaKeyParameters)
            {
                _digestEncryptionAlgorithm = IdDsa;
            }
            else
            {
                throw new ArgumentException("Unknown Key Algorithm " + privKey);
            }
        }

        if (hasRsAdata)
        {
            _rsAdata = Array.Empty<byte>();
            _messageDigest = GetHashClass();
        }

        if (privKey != null)
        {
            _sig = SignerUtilities.GetSigner(GetDigestAlgorithm());
            _sig.Init(true, privKey);
        }
    }

    /// <summary>
    ///     Get all the X.509 certificates associated with this PKCS#7 object in no particular order.
    ///     Other certificates, from OCSP for example, will also be included.
    /// </summary>
    /// <returns>the X.509 certificates associated with this PKCS#7 object</returns>
    public X509Certificate[] Certificates
    {
        get
        {
            var c = new X509Certificate[_certs.Count];
            _certs.CopyTo(c);

            return c;
        }
    }

    /// <summary>
    ///     Get the X.509 certificate revocation lists associated with this PKCS#7 object
    /// </summary>
    /// <returns>the X.509 certificate revocation lists associated with this PKCS#7 object</returns>
    public List<object> CrLs { get; }

    /// <summary>
    ///     Holds value of property location.
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    ///     Gets the OCSP basic response if there is one.
    ///     @since    2.1.6
    /// </summary>
    /// <returns>the OCSP basic response or null</returns>
    public BasicOcspResp Ocsp { get; private set; }

    /// <summary>
    ///     Holds value of property reason.
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    ///     Get the X.509 sign certificate chain associated with this PKCS#7 object.
    ///     Only the certificates used for the main signature will be returned, with
    ///     the signing certificate first.
    ///     @since    2.1.6
    /// </summary>
    /// <returns>the X.509 certificates associated with this PKCS#7 object</returns>
    public X509Certificate[] SignCertificateChain
    {
        get
        {
            var ret = new X509Certificate[_signCerts.Count];
            _signCerts.CopyTo(ret);

            return ret;
        }
    }

    /// <summary>
    ///     Holds value of property signDate.
    /// </summary>
    public DateTime SignDate { get; set; }

    /// <summary>
    ///     Get the X.509 certificate actually used to sign the digest.
    /// </summary>
    /// <returns>the X.509 certificate actually used to sign the digest</returns>
    public X509Certificate SigningCertificate { get; }

    /// <summary>
    ///     Get the version of the PKCS#7 "SignerInfo" object. Always 1
    /// </summary>
    /// <returns>the version of the PKCS#7 "SignerInfo" object. Always 1</returns>
    public int SigningInfoVersion { get; }

    /// <summary>
    ///     Holds value of property signName.
    /// </summary>
    public string SignName { get; set; }

    /// <summary>
    ///     Gets the timestamp date
    ///     @since    2.1.6
    /// </summary>
    /// <returns>a date</returns>
    public DateTime TimeStampDate
    {
        get
        {
            if (TimeStampToken == null)
            {
                return DateTime.MaxValue;
            }

            return TimeStampToken.TimeStampInfo.GenTime;
        }
    }

    /// <summary>
    ///     Gets the timestamp token if there is one.
    ///     @since    2.1.6
    /// </summary>
    /// <returns>the timestamp token or null</returns>
    public TimeStampToken TimeStampToken { get; }

    /// <summary>
    ///     Get the version of the PKCS#7 object. Always 1
    /// </summary>
    /// <returns>the version of the PKCS#7 object. Always 1</returns>
    public int Version { get; }

    /// <summary>
    ///     Gets the algorithm name for a certain id.
    ///     @since    2.1.6
    /// </summary>
    /// <param name="oid">an id (for instance "1.2.840.113549.1.1.1")</param>
    /// <returns>an algorithm name (for instance "RSA")</returns>
    public static string GetAlgorithm(string oid)
    {
        var ret = _algorithmNames[oid];

        if (ret == null)
        {
            return oid;
        }

        return ret;
    }

    /// <summary>
    ///     Gets the digest name for a certain id
    ///     @since    2.1.6
    /// </summary>
    /// <param name="oid">an id (for instance "1.2.840.113549.2.5")</param>
    /// <returns>a digest name (for instance "MD5")</returns>
    public static string GetDigest(string oid)
    {
        var ret = _digestNames[oid];

        if (ret == null)
        {
            return oid;
        }

        return ret;
    }

    /// <summary>
    ///     Get the issuer fields from an X509 Certificate
    /// </summary>
    /// <param name="cert">an X509Certificate</param>
    /// <returns>an X509Name</returns>
    public static X509Name GetIssuerFields(X509Certificate cert)
    {
        if (cert == null)
        {
            throw new ArgumentNullException(nameof(cert));
        }

        return new X509Name((Asn1Sequence)getIssuer(cert.GetTbsCertificate()));
    }

    /// <summary>
    ///     Retrieves the OCSP URL from the given certificate.
    ///     @throws CertificateParsingException on error
    ///     @since    2.1.6
    /// </summary>
    /// <param name="certificate">the certificate</param>
    /// <returns>the URL or null</returns>
    public static string GetOcspurl(X509Certificate certificate)
    {
        if (certificate == null)
        {
            throw new ArgumentNullException(nameof(certificate));
        }

        try
        {
            var obj = getExtensionValue(certificate, X509Extensions.AuthorityInfoAccess.Id);

            if (obj == null)
            {
                return null;
            }

            var accessDescriptions = (Asn1Sequence)obj;

            for (var i = 0; i < accessDescriptions.Count; i++)
            {
                var accessDescription = (Asn1Sequence)accessDescriptions[i];

                if (accessDescription.Count != 2)
                {
                    continue;
                }

                if (accessDescription[0] is DerObjectIdentifier &&
                    ((DerObjectIdentifier)accessDescription[0]).Id.Equals("1.3.6.1.5.5.7.48.1",
                        StringComparison.Ordinal))
                {
                    var accessLocation = getStringFromGeneralName((Asn1Object)accessDescription[1]);

                    if (accessLocation == null)
                    {
                        return "";
                    }

                    return accessLocation;
                }
            }
        }
        catch
        {
        }

        return null;
    }

    /// <summary>
    ///     Get the subject fields from an X509 Certificate
    /// </summary>
    /// <param name="cert">an X509Certificate</param>
    /// <returns>an X509Name</returns>
    public static X509Name GetSubjectFields(X509Certificate cert)
    {
        if (cert == null)
        {
            throw new ArgumentNullException(nameof(cert));
        }

        return new X509Name((Asn1Sequence)getSubject(cert.GetTbsCertificate()));
    }

    /// <summary>
    ///     Verifies a single certificate.
    ///     if no error
    /// </summary>
    /// <param name="cert">the certificate to verify</param>
    /// <param name="crls">the certificate revocation list or  null </param>
    /// <param name="calendar">the date or  null  for the current date</param>
    /// <returns>a  String  with the error description or  null </returns>
    public static string VerifyCertificate(X509Certificate cert, object[] crls, DateTime calendar)
    {
        if (cert == null)
        {
            throw new ArgumentNullException(nameof(cert));
        }

        try
        {
            if (!cert.IsValid(calendar))
            {
                return "The certificate has expired or is not yet valid";
            }
        }
        catch (Exception e)
        {
            return e.ToString();
        }

        return null;
    }

    /// <summary>
    ///     Verifies a certificate chain against a KeyStore.
    ///     Object[]{cert,error}  where  cert  is the
    ///     failed certificate and  error  is the error message
    /// </summary>
    /// <param name="certs">the certificate chain</param>
    /// <param name="keystore">the  KeyStore </param>
    /// <param name="crls">the certificate revocation list or  null </param>
    /// <param name="calendar">the date or  null  for the current date</param>
    /// <returns> null  if the certificate chain could be validade or a</returns>
    public static object[] VerifyCertificates(X509Certificate[] certs,
        IList<X509Certificate> keystore,
        object[] crls,
        DateTime calendar)
    {
        if (certs == null)
        {
            throw new ArgumentNullException(nameof(certs));
        }

        if (keystore == null)
        {
            throw new ArgumentNullException(nameof(keystore));
        }

        for (var k = 0; k < certs.Length; ++k)
        {
            var cert = certs[k];
            var err = VerifyCertificate(cert, crls, calendar);

            if (err != null)
            {
                return new object[]
                {
                    cert, err
                };
            }

            foreach (var certStoreX509 in keystore)
            {
                try
                {
                    if (VerifyCertificate(certStoreX509, crls, calendar) != null)
                    {
                        continue;
                    }

                    try
                    {
                        cert.Verify(certStoreX509.GetPublicKey());

                        return null;
                    }
                    catch
                    {
                    }
                }
                catch
                {
                }
            }

            int j;

            for (j = 0; j < certs.Length; ++j)
            {
                if (j == k)
                {
                    continue;
                }

                var certNext = certs[j];

                try
                {
                    cert.Verify(certNext.GetPublicKey());

                    break;
                }
                catch
                {
                }
            }

            if (j == certs.Length)
            {
                return new object[]
                {
                    cert, "Cannot be verified against the KeyStore or the certificate chain"
                };
            }
        }

        return new object[]
        {
            null, "Invalid state. Possible circular certificate chain"
        };
    }

    /// <summary>
    ///     Loads the default root certificates at &lt;java.home&gt;/lib/security/cacerts
    ///     with the default provider.
    /// </summary>
    /// <returns>a  KeyStore </returns>
    /// <summary>
    ///     public static KeyStore LoadCacertsKeyStore() {
    /// </summary>
    /// <summary>
    ///     return LoadCacertsKeyStore(null);
    /// </summary>
    /// <summary>
    ///     }
    /// </summary>
    /// <summary>
    ///     Verifies an OCSP response against a KeyStore.
    ///     @since    2.1.6
    /// </summary>
    /// <param name="ocsp">the OCSP response</param>
    /// <param name="keystore">the  KeyStore </param>
    /// <returns> true  is a certificate was found</returns>
    public static bool VerifyOcspCertificates(BasicOcspResp ocsp, IList<X509Certificate> keystore)
    {
        if (ocsp == null)
        {
            throw new ArgumentNullException(nameof(ocsp));
        }

        if (keystore == null)
        {
            throw new ArgumentNullException(nameof(keystore));
        }

        try
        {
            foreach (var certStoreX509 in keystore)
            {
                try
                {
                    if (ocsp.Verify(certStoreX509.GetPublicKey()))
                    {
                        return true;
                    }
                }
                catch
                {
                }
            }
        }
        catch
        {
        }

        return false;
    }

    /// <summary>
    ///     Verifies a timestamp against a KeyStore.
    ///     @since    2.1.6
    /// </summary>
    /// <param name="ts">the timestamp</param>
    /// <param name="keystore">the  KeyStore </param>
    /// <returns> true  is a certificate was found</returns>
    public static bool VerifyTimestampCertificates(TimeStampToken ts, IList<X509Certificate> keystore)
    {
        if (ts == null)
        {
            throw new ArgumentNullException(nameof(ts));
        }

        if (keystore == null)
        {
            throw new ArgumentNullException(nameof(keystore));
        }

        try
        {
            foreach (var certStoreX509 in keystore)
            {
                try
                {
                    ts.Validate(certStoreX509);

                    return true;
                }
                catch
                {
                }
            }
        }
        catch
        {
        }

        return false;
    }

    /// <summary>
    ///     When using authenticatedAttributes the authentication process is different.
    ///     The document digest is generated and put inside the attribute. The signing is done over the DER encoded
    ///     authenticatedAttributes. This method provides that encoding and the parameters must be
    ///     exactly the same as in {@link #getEncodedPKCS7(byte[],Calendar)}.
    ///     A simple example:
    ///     Calendar cal = Calendar.GetInstance();
    ///     PdfPKCS7 pk7 = new PdfPKCS7(key, chain, null, "SHA1", null, false);
    ///     MessageDigest messageDigest = MessageDigest.GetInstance("SHA1");
    ///     byte buf[] = new byte[8192];
    ///     int n;
    ///     Stream inp = sap.GetRangeStream();
    ///     while ((n = inp.Read(buf)) &gt; 0) {
    ///     messageDigest.Update(buf, 0, n);
    ///     }
    ///     byte hash[] = messageDigest.Digest();
    ///     byte sh[] = pk7.GetAuthenticatedAttributeBytes(hash, cal);
    ///     pk7.Update(sh, 0, sh.length);
    ///     byte sg[] = pk7.GetEncodedPKCS7(hash, cal);
    /// </summary>
    /// <param name="secondDigest">the content digest</param>
    /// <param name="signingTime">the signing time</param>
    /// <param name="ocsp"></param>
    /// <returns>the byte array representation of the authenticatedAttributes ready to be signed</returns>
    public static byte[] GetAuthenticatedAttributeBytes(byte[] secondDigest, DateTime signingTime, byte[] ocsp)
        => getAuthenticatedAttributeSet(secondDigest, signingTime, ocsp).GetEncoded(Asn1Encodable.Der);

    /// <summary>
    ///     Get the algorithm used to calculate the message digest
    /// </summary>
    /// <returns>the algorithm used to calculate the message digest</returns>
    public string GetDigestAlgorithm()
    {
        var dea = GetAlgorithm(_digestEncryptionAlgorithm);

        if (dea == null)
        {
            dea = _digestEncryptionAlgorithm;
        }

        return GetHashAlgorithm() + "with" + dea;
    }

    /// <summary>
    ///     Gets the bytes for the PKCS#1 object.
    /// </summary>
    /// <returns>a byte array</returns>
    public byte[] GetEncodedPkcs1()
    {
        if (_externalDigest != null)
        {
            _digest = _externalDigest;
        }
        else
        {
            _digest = _sig.GenerateSignature();
        }

        using var bOut = new MemoryStream();
        using var dout = Asn1OutputStream.Create(bOut);
        dout.WriteObject(new DerOctetString(_digest));
        dout.Dispose();

        return bOut.ToArray();
    }

    /// <summary>
    ///     Gets the bytes for the PKCS7SignedData object.
    /// </summary>
    /// <returns>the bytes for the PKCS7SignedData object</returns>
    public byte[] GetEncodedPkcs7() => GetEncodedPkcs7(null, DateTime.Now, null, null);

    /// <summary>
    ///     Gets the bytes for the PKCS7SignedData object. Optionally the authenticatedAttributes
    ///     in the signerInfo can also be set. If either of the parameters is  null , none will be used.
    /// </summary>
    /// <param name="secondDigest">the digest in the authenticatedAttributes</param>
    /// <param name="signingTime">the signing time in the authenticatedAttributes</param>
    /// <returns>the bytes for the PKCS7SignedData object</returns>
    public byte[] GetEncodedPkcs7(byte[] secondDigest, DateTime signingTime)
        => GetEncodedPkcs7(secondDigest, signingTime, null, null);

    /// <summary>
    ///     Gets the bytes for the PKCS7SignedData object. Optionally the authenticatedAttributes
    ///     in the signerInfo can also be set, OR a time-stamp-authority client
    ///     may be provided.
    ///     @since   2.1.6
    /// </summary>
    /// <param name="secondDigest">the digest in the authenticatedAttributes</param>
    /// <param name="signingTime">the signing time in the authenticatedAttributes</param>
    /// <param name="tsaClient">TSAClient - null or an optional time stamp authority client</param>
    /// <param name="ocsp"></param>
    /// <returns>byte[] the bytes for the PKCS7SignedData object</returns>
    public byte[] GetEncodedPkcs7(byte[] secondDigest, DateTime signingTime, ITsaClient tsaClient, byte[] ocsp)
    {
        if (_externalDigest != null)
        {
            _digest = _externalDigest;

            if (_rsAdata != null)
            {
                _rsAdata = _externalRsAdata;
            }
        }
        else if (_externalRsAdata != null && _rsAdata != null)
        {
            _rsAdata = _externalRsAdata;
            _sig.BlockUpdate(_rsAdata, 0, _rsAdata.Length);
            _digest = _sig.GenerateSignature();
        }
        else
        {
            if (_rsAdata != null)
            {
                _rsAdata = new byte[_messageDigest.GetDigestSize()];
                _messageDigest.DoFinal(_rsAdata, 0);
                _sig.BlockUpdate(_rsAdata, 0, _rsAdata.Length);
            }

            _digest = _sig.GenerateSignature();
        }

        // Create the set of Hash algorithms
        var digestAlgorithms = new Asn1EncodableVector();

        foreach (var dal in _digestalgos.Keys)
        {
            var algos = new Asn1EncodableVector();
            algos.Add(new DerObjectIdentifier(dal));
            algos.Add(DerNull.Instance);
            digestAlgorithms.Add(new DerSequence(algos));
        }

        // Create the contentInfo.
        var v = new Asn1EncodableVector();
        v.Add(new DerObjectIdentifier(IdPkcs7Data));

        if (_rsAdata != null)
        {
            v.Add(new DerTaggedObject(0, new DerOctetString(_rsAdata)));
        }

        var contentinfo = new DerSequence(v);

        // Get all the certificates
        //
        v = new Asn1EncodableVector();

        foreach (var xcert in _certs)
        {
            using var memoryStream = new MemoryStream(xcert.GetEncoded());
            using var tempstream = new Asn1InputStream(memoryStream);
            v.Add(tempstream.ReadObject());
        }

        var dercertificates = new DerSet(v);

        // Create signerinfo structure.
        //
        var signerinfo = new Asn1EncodableVector();

        // Add the signerInfo version
        //
        signerinfo.Add(new DerInteger(SigningInfoVersion));

        v = new Asn1EncodableVector();
        v.Add(getIssuer(SigningCertificate.GetTbsCertificate()));
        v.Add(new DerInteger(SigningCertificate.SerialNumber));
        signerinfo.Add(new DerSequence(v));

        // Add the digestAlgorithm
        v = new Asn1EncodableVector();
        v.Add(new DerObjectIdentifier(_digestAlgorithm));
        v.Add(DerNull.Instance);
        signerinfo.Add(new DerSequence(v));

        // add the authenticated attribute if present
        if (secondDigest != null /*&& signingTime != null*/)
        {
            signerinfo.Add(new DerTaggedObject(false, 0,
                getAuthenticatedAttributeSet(secondDigest, signingTime, ocsp)));
        }

        // Add the digestEncryptionAlgorithm
        v = new Asn1EncodableVector();
        v.Add(new DerObjectIdentifier(_digestEncryptionAlgorithm));
        v.Add(DerNull.Instance);
        signerinfo.Add(new DerSequence(v));

        // Add the digest
        signerinfo.Add(new DerOctetString(_digest));

        // When requested, go get and add the timestamp. May throw an exception.
        // Added by Martin Brunecky, 07/12/2007 folowing Aiken Sam, 2006-11-15
        // Sam found Adobe expects time-stamped SHA1-1 of the encrypted digest
        if (tsaClient != null)
        {
            using var sha1 = SHA1.Create();
            var tsImprint = sha1.ComputeHash(_digest);
            var tsToken = tsaClient.GetTimeStampToken(this, tsImprint);

            if (tsToken != null)
            {
                var unauthAttributes = buildUnauthenticatedAttributes(tsToken);

                if (unauthAttributes != null)
                {
                    signerinfo.Add(new DerTaggedObject(false, 1, new DerSet(unauthAttributes)));
                }
            }
        }

        // Finally build the body out of all the components above
        var body = new Asn1EncodableVector();
        body.Add(new DerInteger(Version));
        body.Add(new DerSet(digestAlgorithms));
        body.Add(contentinfo);
        body.Add(new DerTaggedObject(false, 0, dercertificates));

        //                if (crls.Count > 0) {
        //                    v = new Asn1EncodableVector();
        //                    for (Iterator i = crls.Iterator();i.HasNext();) {
        //                        Asn1InputStream t = new Asn1InputStream(new ByteArrayInputStream((((X509CRL)i.Next()).GetEncoded())));
        //                        v.Add(t.ReadObject());
        //                    }
        //                    DERSet dercrls = new DERSet(v);
        //                    body.Add(new DERTaggedObject(false, 1, dercrls));
        //                }

        // Only allow one signerInfo
        body.Add(new DerSet(new DerSequence(signerinfo)));

        // Now we have the body, wrap it in it's PKCS7Signed shell
        // and return it
        //
        var whole = new Asn1EncodableVector();
        whole.Add(new DerObjectIdentifier(IdPkcs7SignedData));
        whole.Add(new DerTaggedObject(0, new DerSequence(body)));

        using var bOut = new MemoryStream();
        using var dout = Asn1OutputStream.Create(bOut);
        dout.WriteObject(new DerSequence(whole));

        return bOut.ToArray();
    }

    /// <summary>
    ///     Returns the algorithm.
    /// </summary>
    /// <returns>the digest algorithm</returns>
    public string GetHashAlgorithm() => GetDigest(_digestAlgorithm);

    /// <summary>
    ///     Checks if OCSP revocation refers to the document signing certificate.
    ///     @since    2.1.6
    /// </summary>
    /// <returns>true if it checks false otherwise</returns>
    public bool IsRevocationValid()
    {
        if (Ocsp == null)
        {
            return false;
        }

        if (_signCerts.Count < 2)
        {
            return false;
        }

        try
        {
            var cs = SignCertificateChain;
            var sr = Ocsp.Responses[0];
            var cid = sr.GetCertID();
            var sigcer = SigningCertificate;
            var isscer = cs[1];

            var tis = new CertificateID(
                new AlgorithmIdentifier(new DerObjectIdentifier(OiwObjectIdentifiers.IdSha1.Id), DerNull.Instance),
                isscer, sigcer.SerialNumber);

            return tis.Equals(cid);
        }
        catch
        {
        }

        return false;
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

        if (digestEncryptionAlgorithm != null)
        {
            if (digestEncryptionAlgorithm.Equals("RSA", StringComparison.Ordinal))
            {
                _digestEncryptionAlgorithm = IdRsa;
            }
            else if (digestEncryptionAlgorithm.Equals("DSA", StringComparison.Ordinal))
            {
                _digestEncryptionAlgorithm = IdDsa;
            }
            else
            {
                throw new ArgumentException("Unknown Key Algorithm " + digestEncryptionAlgorithm);
            }
        }
    }

    /// <summary>
    ///     Update the digest with the specified bytes. This method is used both for signing and verifying
    ///     @throws SignatureException on error
    /// </summary>
    /// <param name="buf">the data buffer</param>
    /// <param name="off">the offset in the data buffer</param>
    /// <param name="len">the data length</param>
    public void Update(byte[] buf, int off, int len)
    {
        if (_rsAdata != null || _digestAttr != null)
        {
            _messageDigest.BlockUpdate(buf, off, len);
        }
        else
        {
            _sig.BlockUpdate(buf, off, len);
        }
    }

    /// <summary>
    ///     Verify the digest.
    ///     @throws SignatureException on error
    /// </summary>
    /// <returns> true  if the signature checks out,  false  otherwise</returns>
    public bool Verify()
    {
        if (_verified)
        {
            return _verifyResult;
        }

        if (_sigAttr != null)
        {
            var msd = new byte[_messageDigest.GetDigestSize()];
            _sig.BlockUpdate(_sigAttr, 0, _sigAttr.Length);

            if (_rsAdata != null)
            {
                _messageDigest.DoFinal(msd, 0);
                _messageDigest.BlockUpdate(msd, 0, msd.Length);
            }

            _messageDigest.DoFinal(msd, 0);
            _verifyResult = Arrays.AreEqual(msd, _digestAttr) && _sig.VerifySignature(_digest);
        }
        else
        {
            if (_rsAdata != null)
            {
                var msd = new byte[_messageDigest.GetDigestSize()];
                _messageDigest.DoFinal(msd, 0);
                _sig.BlockUpdate(msd, 0, msd.Length);
            }

            _verifyResult = _sig.VerifySignature(_digest);
        }

        _verified = true;

        return _verifyResult;
    }

    /// <summary>
    ///     Checks if the timestamp refers to this document.
    ///     @throws java.security.NoSuchAlgorithmException on error
    ///     @since    2.1.6
    /// </summary>
    /// <returns>true if it checks false otherwise</returns>
    public bool VerifyTimestampImprint()
    {
        if (TimeStampToken == null)
        {
            return false;
        }

        var imprint = TimeStampToken.TimeStampInfo.TstInfo.MessageImprint;
        var md = SHA1.Create().ComputeHash(_digest);
        var imphashed = imprint.GetHashedMessage();
        var res = Arrays.AreEqual(md, imphashed);

        return res;
    }

    internal IDigest GetHashClass() => DigestUtilities.GetDigest(GetHashAlgorithm());

    private static Asn1Object getExtensionValue(X509Certificate cert, string oid)
    {
        var bytes = cert.GetExtensionValue(new DerObjectIdentifier(oid)).GetDerEncoded();

        if (bytes == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream(bytes);
        using var aIn = new Asn1InputStream(memoryStream);
        var octs = (Asn1OctetString)aIn.ReadObject();
        using var ain = new Asn1InputStream(new MemoryStream(octs.GetOctets()));

        return ain.ReadObject();
    }

    /// <summary>
    ///     Get the "issuer" from the TBSCertificate bytes that are passed in
    /// </summary>
    /// <param name="enc">a TBSCertificate in a byte array</param>
    /// <returns>a DERObject</returns>
    private static Asn1Object getIssuer(byte[] enc)
    {
        using var memoryStream = new MemoryStream(enc);
        using var inp = new Asn1InputStream(memoryStream);
        var seq = (Asn1Sequence)inp.ReadObject();

        return (Asn1Object)seq[seq[0] is DerTaggedObject ? 3 : 2];
    }

    private static string getStringFromGeneralName(Asn1Object names)
    {
        var taggedObject = (DerTaggedObject)names;

        return EncodingsRegistry.GetEncoding(1252)
            .GetString(Asn1OctetString.GetInstance(taggedObject, false).GetOctets());
    }

    /// <summary>
    ///     Get the "subject" from the TBSCertificate bytes that are passed in
    /// </summary>
    /// <param name="enc">A TBSCertificate in a byte array</param>
    /// <returns>a DERObject</returns>
    private static Asn1Object getSubject(byte[] enc)
    {
        using var memoryStream = new MemoryStream(enc);
        using var inp = new Asn1InputStream(memoryStream);
        var seq = (Asn1Sequence)inp.ReadObject();

        return (Asn1Object)seq[seq[0] is DerTaggedObject ? 5 : 4];
    }

    /// <summary>
    ///     Added by Aiken Sam, 2006-11-15, modifed by Martin Brunecky 07/12/2007
    ///     to start with the timeStampToken (signedData 1.2.840.113549.1.7.2).
    ///     Token is the TSA response without response status, which is usually
    ///     handled by the (vendor supplied) TSA request/response interface).
    ///     @throws IOException
    /// </summary>
    /// <param name="timeStampToken">byte[] - time stamp token, DER encoded signedData</param>
    /// <returns>ASN1EncodableVector</returns>
    private static Asn1EncodableVector buildUnauthenticatedAttributes(byte[] timeStampToken)
    {
        if (timeStampToken == null)
        {
            return null;
        }

        // @todo: move this together with the rest of the defintions
        var idTimeStampToken = "1.2.840.113549.1.9.16.2.14"; // RFC 3161 id-aa-timeStampToken

        using var memoryStream = new MemoryStream(timeStampToken);
        using var tempstream = new Asn1InputStream(memoryStream);
        var unauthAttributes = new Asn1EncodableVector();

        var v = new Asn1EncodableVector();
        v.Add(new DerObjectIdentifier(idTimeStampToken)); // id-aa-timeStampToken
        var seq = (Asn1Sequence)tempstream.ReadObject();
        v.Add(new DerSet(seq));

        unauthAttributes.Add(new DerSequence(v));

        return unauthAttributes;
    }

    private void calcSignCertificateChain()
    {
        var cc = new List<X509Certificate>();
        cc.Add(SigningCertificate);
        var oc = new List<X509Certificate>(_certs);

        for (var k = 0; k < oc.Count; ++k)
        {
            if (SigningCertificate.SerialNumber.Equals(oc[k].SerialNumber))
            {
                oc.RemoveAt(k);
                --k;
            }
        }

        var found = true;

        while (found)
        {
            var v = cc[cc.Count - 1];
            found = false;

            for (var k = 0; k < oc.Count; ++k)
            {
                try
                {
                    v.Verify(oc[k].GetPublicKey());
                    found = true;
                    cc.Add(oc[k]);
                    oc.RemoveAt(k);

                    break;
                }
                catch
                {
                }
            }
        }

        _signCerts = cc;
    }

    private void findOcsp(Asn1Sequence seq)
    {
        Ocsp = null;
        var ret = false;

        while (true)
        {
            if (seq[0] is DerObjectIdentifier &&
                ((DerObjectIdentifier)seq[0]).Id.Equals(OcspObjectIdentifiers.PkixOcspBasic.Id,
                    StringComparison.Ordinal))
            {
                break;
            }

            ret = true;

            for (var k = 0; k < seq.Count; ++k)
            {
                if (seq[k] is Asn1Sequence)
                {
                    seq = (Asn1Sequence)seq[0];
                    ret = false;

                    break;
                }

                if (seq[k] is Asn1TaggedObject tag)
                {
                    var asn1Object = tag.GetBaseObject();

                    if (asn1Object is Asn1Sequence sequence)
                    {
                        seq = sequence;
                        ret = false;

                        break;
                    }

                    return;
                }
            }

            if (ret)
            {
                return;
            }
        }

        var os = (DerOctetString)seq[1];
        using var inp = new Asn1InputStream(os.GetOctets());
        var resp = BasicOcspResponse.GetInstance(inp.ReadObject());
        Ocsp = new BasicOcspResp(resp);
    }

    private static DerSet getAuthenticatedAttributeSet(byte[] secondDigest, DateTime signingTime, byte[] ocsp)
    {
        var attribute = new Asn1EncodableVector();
        var v = new Asn1EncodableVector();
        v.Add(new DerObjectIdentifier(IdContentType));
        v.Add(new DerSet(new DerObjectIdentifier(IdPkcs7Data)));
        attribute.Add(new DerSequence(v));
        v = new Asn1EncodableVector();
        v.Add(new DerObjectIdentifier(IdSigningTime));
        v.Add(new DerSet(new DerUtcTime(signingTime, DateTimeFormatInfo.InvariantInfo.Calendar.TwoDigitYearMax)));
        attribute.Add(new DerSequence(v));
        v = new Asn1EncodableVector();
        v.Add(new DerObjectIdentifier(IdMessageDigest));
        v.Add(new DerSet(new DerOctetString(secondDigest)));
        attribute.Add(new DerSequence(v));

        if (ocsp != null)
        {
            v = new Asn1EncodableVector();
            v.Add(new DerObjectIdentifier(IdAdbeRevocation));
            var doctet = new DerOctetString(ocsp);
            var vo1 = new Asn1EncodableVector();
            var v2 = new Asn1EncodableVector();
            v2.Add(OcspObjectIdentifiers.PkixOcspBasic);
            v2.Add(doctet);
            var den = new DerEnumerated(0);
            var v3 = new Asn1EncodableVector();
            v3.Add(den);
            v3.Add(new DerTaggedObject(true, 0, new DerSequence(v2)));
            vo1.Add(new DerSequence(v3));
            v.Add(new DerSet(new DerSequence(new DerTaggedObject(true, 1, new DerSequence(vo1)))));
            attribute.Add(new DerSequence(v));
        }

        return new DerSet(attribute);
    }

    /// <summary>
    ///     a class that holds an X509 name
    /// </summary>
    public class X509Name
    {
        /// <summary>
        ///     country code - StringType(SIZE(2))
        /// </summary>
        public static DerObjectIdentifier C = new("2.5.4.6");

        /// <summary>
        ///     common name - StringType(SIZE(1..64))
        /// </summary>
        public static DerObjectIdentifier Cn = new("2.5.4.3");

        /// <summary>
        ///     object identifier
        /// </summary>
        public static DerObjectIdentifier Dc = new("0.9.2342.19200300.100.1.25");

        /// <summary>
        ///     A Hashtable with default symbols
        /// </summary>
        public static INullValueDictionary<Asn1Encodable, string> DefaultSymbols =
            new NullValueDictionary<Asn1Encodable, string>();

        /// <summary>
        ///     email address in Verisign certificates
        /// </summary>
        public static DerObjectIdentifier E = new("1.2.840.113549.1.9.1");

        /// <summary>
        ///     Email address (RSA PKCS#9 extension) - IA5String.
        ///     Note: if you're trying to be ultra orthodox, don't use this! It shouldn't be in here.
        /// </summary>
        public static DerObjectIdentifier EmailAddress = new("1.2.840.113549.1.9.1");

        /// <summary>
        ///     Naming attribute of type X520name
        /// </summary>
        public static DerObjectIdentifier Generation = new("2.5.4.44");

        /// <summary>
        ///     Naming attribute of type X520name
        /// </summary>
        public static DerObjectIdentifier Givenname = new("2.5.4.42");

        /// <summary>
        ///     Naming attribute of type X520name
        /// </summary>
        public static DerObjectIdentifier Initials = new("2.5.4.43");

        /// <summary>
        ///     locality name - StringType(SIZE(1..64))
        /// </summary>
        public static DerObjectIdentifier L = new("2.5.4.7");

        /// <summary>
        ///     organization - StringType(SIZE(1..64))
        /// </summary>
        public static DerObjectIdentifier O = new("2.5.4.10");

        /// <summary>
        ///     organizational unit name - StringType(SIZE(1..64))
        /// </summary>
        public static DerObjectIdentifier Ou = new("2.5.4.11");

        /// <summary>
        ///     device serial number name - StringType(SIZE(1..64))
        /// </summary>
        public static DerObjectIdentifier Sn = new("2.5.4.5");

        /// <summary>
        ///     state, or province name - StringType(SIZE(1..64))
        /// </summary>
        public static DerObjectIdentifier St = new("2.5.4.8");

        /// <summary>
        ///     Naming attribute of type X520name
        /// </summary>
        public static DerObjectIdentifier Surname = new("2.5.4.4");

        /// <summary>
        ///     Title
        /// </summary>
        public static DerObjectIdentifier T = new("2.5.4.12");

        /// <summary>
        ///     LDAP User id.
        /// </summary>
        public static DerObjectIdentifier Uid = new("0.9.2342.19200300.100.1.1");

        /// <summary>
        ///     Naming attribute of type X520name
        /// </summary>
        public static DerObjectIdentifier UniqueIdentifier = new("2.5.4.45");

        /// <summary>
        ///     A Hashtable with values
        /// </summary>
        public INullValueDictionary<string, IList<string>> Values = new NullValueDictionary<string, IList<string>>();

        static X509Name()
        {
            DefaultSymbols[C] = "C";
            DefaultSymbols[O] = "O";
            DefaultSymbols[T] = "T";
            DefaultSymbols[Ou] = "OU";
            DefaultSymbols[Cn] = "CN";
            DefaultSymbols[L] = "L";
            DefaultSymbols[St] = "ST";
            DefaultSymbols[Sn] = "SN";
            DefaultSymbols[EmailAddress] = "E";
            DefaultSymbols[Dc] = "DC";
            DefaultSymbols[Uid] = "UID";
            DefaultSymbols[Surname] = "SURNAME";
            DefaultSymbols[Givenname] = "GIVENNAME";
            DefaultSymbols[Initials] = "INITIALS";
            DefaultSymbols[Generation] = "GENERATION";
        }

        /// <summary>
        ///     Constructs an X509 name
        /// </summary>
        /// <param name="seq">an Asn1 Sequence</param>
        public X509Name(Asn1Sequence seq)
        {
            if (seq == null)
            {
                throw new ArgumentNullException(nameof(seq));
            }

            var e = seq.GetEnumerator();

            while (e.MoveNext())
            {
                var sett = (Asn1Set)e.Current;

                for (var i = 0; i < sett.Count; i++)
                {
                    var s = (Asn1Sequence)sett[i];
                    var id = DefaultSymbols[s[0]];

                    if (id == null)
                    {
                        continue;
                    }

                    var vs = Values[id];

                    if (vs == null)
                    {
                        vs = new List<string>();
                        Values[id] = vs;
                    }

                    vs.Add(((DerStringBase)s[1]).GetString());
                }
            }
        }

        /// <summary>
        ///     Constructs an X509 name
        /// </summary>
        /// <param name="dirName">a directory name</param>
        public X509Name(string dirName)
        {
            var nTok = new X509NameTokenizer(dirName);

            while (nTok.HasMoreTokens())
            {
                var token = nTok.NextToken();
                var index = token.IndexOf("=", StringComparison.Ordinal);

                if (index == -1)
                {
                    throw new ArgumentException("badly formated directory string");
                }

                var id = token.Substring(0, index).ToUpper(CultureInfo.InvariantCulture);
                var value = token.Substring(index + 1);
                var vs = Values[id];

                if (vs == null)
                {
                    vs = new List<string>();
                    Values[id] = vs;
                }

                vs.Add(value);
            }
        }

        public string GetField(string name)
        {
            var vs = Values[name];

            return vs == null ? null : vs[0];
        }

        /// <summary>
        ///     gets a field array from the values Hashmap
        /// </summary>
        /// <param name="name"></param>
        /// <returns>an ArrayList</returns>
        public IList<string> GetFieldArray(string name)
        {
            var vs = Values[name];

            return vs == null ? null : vs;
        }

        /// <summary>
        ///     getter for values
        /// </summary>
        /// <returns>a Hashtable with the fields of the X509 name</returns>
        public INullValueDictionary<string, IList<string>> GetFields() => Values;

        /// <summary>
        ///     @see java.lang.Object#toString()
        /// </summary>
        public override string ToString() => Values.ToString();
    }

    /// <summary>
    ///     class for breaking up an X500 Name into it's component tokens, ala
    ///     java.util.StringTokenizer. We need this class as some of the
    ///     lightweight Java environment don't support classes like
    ///     StringTokenizer.
    /// </summary>
    public class X509NameTokenizer
    {
        private readonly StringBuilder _buf = new();
        private readonly string _oid;
        private int _index;

        public X509NameTokenizer(string oid)
        {
            _oid = oid;
            _index = -1;
        }

        public bool HasMoreTokens() => _index != _oid.Length;

        public string NextToken()
        {
            if (_index == _oid.Length)
            {
                return null;
            }

            var end = _index + 1;
            var quoted = false;
            var escaped = false;

            _buf.Length = 0;

            while (end != _oid.Length)
            {
                var c = _oid[end];

                if (c == '"')
                {
                    if (!escaped)
                    {
                        quoted = !quoted;
                    }
                    else
                    {
                        _buf.Append(c);
                    }

                    escaped = false;
                }
                else
                {
                    if (escaped || quoted)
                    {
                        _buf.Append(c);
                        escaped = false;
                    }
                    else if (c == '\\')
                    {
                        escaped = true;
                    }
                    else if (c == ',')
                    {
                        break;
                    }
                    else
                    {
                        _buf.Append(c);
                    }
                }

                end++;
            }

            _index = end;

            return _buf.ToString().Trim();
        }
    }
}