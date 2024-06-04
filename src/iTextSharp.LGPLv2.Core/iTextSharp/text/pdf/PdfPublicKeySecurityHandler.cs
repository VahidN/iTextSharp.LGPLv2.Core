using iTextSharp.text.pdf.crypto;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using ContentInfo = Org.BouncyCastle.Asn1.Cms.ContentInfo;
using IssuerAndSerialNumber = Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber;

namespace iTextSharp.text.pdf;

/// <summary>
///     @author Aiken Sam (aikensam@ieee.org)
/// </summary>
public class PdfPublicKeySecurityHandler
{
    private const int SeedLength = 20;

    private readonly List<PdfPublicKeyRecipient> _recipients = new();

    private readonly byte[] _seed;

    public PdfPublicKeySecurityHandler() => _seed = IvGenerator.GetIv(SeedLength);

    public void AddRecipient(PdfPublicKeyRecipient recipient) => _recipients.Add(recipient);

    public byte[] GetEncodedRecipient(int index)
    {
        //Certificate certificate = recipient.GetX509();
        var recipient = _recipients[index];
        var cms = recipient.Cms;

        if (cms != null)
        {
            return cms;
        }

        var certificate = recipient.Certificate;

        var permission =
            recipient.Permission; //PdfWriter.AllowCopy | PdfWriter.AllowPrinting | PdfWriter.AllowScreenReaders | PdfWriter.AllowAssembly;

        permission |= unchecked((int)0xfffff0c0);
        permission &= unchecked((int)0xfffffffc);
        permission += 1;

        var pkcs7Input = new byte[24];

        var one = (byte)permission;
        var two = (byte)(permission >> 8);
        var three = (byte)(permission >> 16);
        var four = (byte)(permission >> 24);

        Array.Copy(_seed, 0, pkcs7Input, 0, 20); // put this seed in the pkcs7 input

        pkcs7Input[20] = four;
        pkcs7Input[21] = three;
        pkcs7Input[22] = two;
        pkcs7Input[23] = one;

        var obj = createDerForRecipient(pkcs7Input, certificate);
        using var baos = new MemoryStream();
        using var k = Asn1OutputStream.Create(baos, Asn1Encodable.Der);
        k.WriteObject(obj);
        cms = baos.ToArray();
        recipient.Cms = cms;

        return cms;
    }

    public PdfArray GetEncodedRecipients()
    {
        var encodedRecipients = new PdfArray();
        byte[] cms = null;

        for (var i = 0; i < _recipients.Count; i++)
        {
            try
            {
                cms = GetEncodedRecipient(i);
                encodedRecipients.Add(new PdfLiteral(PdfContentByte.EscapeString(cms)));
            }
            catch
            {
                encodedRecipients = null;
            }
        }

        return encodedRecipients;
    }

    public int GetRecipientsSize() => _recipients.Count;

    protected internal byte[] GetSeed() => (byte[])_seed.Clone();

    private static KeyTransRecipientInfo computeRecipientInfo(X509Certificate x509Certificate, byte[] abyte0)
    {
        using var memoryStream = new MemoryStream(x509Certificate.GetTbsCertificate());
        using var asn1Inputstream = new Asn1InputStream(memoryStream);
        var tbscertificatestructure = TbsCertificateStructure.GetInstance(asn1Inputstream.ReadObject());
        var algorithmidentifier = tbscertificatestructure.SubjectPublicKeyInfo.Algorithm;

        var issuerandserialnumber =
            new IssuerAndSerialNumber(tbscertificatestructure.Issuer, tbscertificatestructure.SerialNumber.Value);

        var cipher = CipherUtilities.GetCipher(algorithmidentifier.Algorithm);
        cipher.Init(true, x509Certificate.GetPublicKey());
        var outp = new byte[10000];
        var len = cipher.DoFinal(abyte0, outp, 0);
        var abyte1 = new byte[len];
        Array.Copy(outp, 0, abyte1, 0, len);
        var deroctetstring = new DerOctetString(abyte1);
        var recipId = new RecipientIdentifier(issuerandserialnumber);

        return new KeyTransRecipientInfo(recipId, algorithmidentifier, deroctetstring);
    }

    private static Asn1Object createDerForRecipient(byte[] inp, X509Certificate cert)
    {
        var s = "1.2.840.113549.3.2";

        var outp = new byte[100];
        var derob = new DerObjectIdentifier(s);
        var keyp = IvGenerator.GetIv(16);
        var cf = CipherUtilities.GetCipher(derob);
        var kp = new KeyParameter(keyp);
        var iv = IvGenerator.GetIv(cf.GetBlockSize());
        var piv = new ParametersWithIV(kp, iv);
        cf.Init(true, piv);
        var len = cf.DoFinal(inp, outp, 0);

        var abyte1 = new byte[len];
        Array.Copy(outp, 0, abyte1, 0, len);
        var deroctetstring = new DerOctetString(abyte1);
        var keytransrecipientinfo = computeRecipientInfo(cert, keyp);
        var derset = new DerSet(new RecipientInfo(keytransrecipientinfo));
        var ev = new Asn1EncodableVector();
        ev.Add(new DerInteger(58));
        ev.Add(new DerOctetString(iv));
        var seq = new DerSequence(ev);
        var algorithmidentifier = new AlgorithmIdentifier(derob, seq);

        var encryptedcontentinfo =
            new EncryptedContentInfo(PkcsObjectIdentifiers.Data, algorithmidentifier, deroctetstring);

        var env = new EnvelopedData(null, derset, encryptedcontentinfo, (Asn1Set)null);
        var contentinfo = new ContentInfo(PkcsObjectIdentifiers.EnvelopedData, env);

        return contentinfo.ToAsn1Object();
    }
}