using System.IO;
using System.Collections;
using iTextSharp.text.pdf.crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Security;


namespace iTextSharp.text.pdf
{

    /// <summary>
    /// @author Aiken Sam (aikensam@ieee.org)
    /// </summary>
    public class PdfPublicKeySecurityHandler
    {

        private const int SeedLength = 20;

        private readonly ArrayList _recipients;

        private readonly byte[] _seed;

        public PdfPublicKeySecurityHandler()
        {
            _seed = IvGenerator.GetIv(SeedLength);
            _recipients = new ArrayList();
        }


        public void AddRecipient(PdfPublicKeyRecipient recipient)
        {
            _recipients.Add(recipient);
        }

        public byte[] GetEncodedRecipient(int index)
        {
            //Certificate certificate = recipient.GetX509();
            PdfPublicKeyRecipient recipient = (PdfPublicKeyRecipient)_recipients[index];
            byte[] cms = recipient.Cms;

            if (cms != null) return cms;

            X509Certificate certificate = recipient.Certificate;
            int permission = recipient.Permission;//PdfWriter.AllowCopy | PdfWriter.AllowPrinting | PdfWriter.AllowScreenReaders | PdfWriter.AllowAssembly;
            int revision = 3;

            permission |= (int)(revision == 3 ? 0xfffff0c0 : 0xffffffc0);
            permission &= unchecked((int)0xfffffffc);
            permission += 1;

            byte[] pkcs7Input = new byte[24];

            byte one = (byte)(permission);
            byte two = (byte)(permission >> 8);
            byte three = (byte)(permission >> 16);
            byte four = (byte)(permission >> 24);

            System.Array.Copy(_seed, 0, pkcs7Input, 0, 20); // put this seed in the pkcs7 input

            pkcs7Input[20] = four;
            pkcs7Input[21] = three;
            pkcs7Input[22] = two;
            pkcs7Input[23] = one;

            Asn1Object obj = createDerForRecipient(pkcs7Input, certificate);

            MemoryStream baos = new MemoryStream();

            Asn1OutputStream k = Asn1OutputStream.Create(baos, Asn1Encodable.Der);

            k.WriteObject(obj);

            cms = baos.ToArray();

            recipient.Cms = cms;

            return cms;
        }

        public PdfArray GetEncodedRecipients()
        {
            PdfArray encodedRecipients = new PdfArray();
            byte[] cms = null;
            for (int i = 0; i < _recipients.Count; i++)
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

        public int GetRecipientsSize()
        {
            return _recipients.Count;
        }

        protected internal byte[] GetSeed()
        {
            return (byte[])_seed.Clone();
        }
        private KeyTransRecipientInfo computeRecipientInfo(X509Certificate x509Certificate, byte[] abyte0)
        {
            Asn1InputStream asn1Inputstream =
                new Asn1InputStream(new MemoryStream(x509Certificate.GetTbsCertificate()));
            TbsCertificateStructure tbscertificatestructure =
                TbsCertificateStructure.GetInstance(asn1Inputstream.ReadObject());
            AlgorithmIdentifier algorithmidentifier = tbscertificatestructure.SubjectPublicKeyInfo.AlgorithmID;
            Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber issuerandserialnumber =
                new Org.BouncyCastle.Asn1.Cms.IssuerAndSerialNumber(
                    tbscertificatestructure.Issuer,
                    tbscertificatestructure.SerialNumber.Value);
            IBufferedCipher cipher = CipherUtilities.GetCipher(algorithmidentifier.Algorithm);
            cipher.Init(true, x509Certificate.GetPublicKey());
            byte[] outp = new byte[10000];
            int len = cipher.DoFinal(abyte0, outp, 0);
            byte[] abyte1 = new byte[len];
            System.Array.Copy(outp, 0, abyte1, 0, len);
            DerOctetString deroctetstring = new DerOctetString(abyte1);
            RecipientIdentifier recipId = new RecipientIdentifier(issuerandserialnumber);
            return new KeyTransRecipientInfo(recipId, algorithmidentifier, deroctetstring);
        }

        private Asn1Object createDerForRecipient(byte[] inp, X509Certificate cert)
        {

            string s = "1.2.840.113549.3.2";

            byte[] outp = new byte[100];
            DerObjectIdentifier derob = new DerObjectIdentifier(s);
            byte[] keyp = IvGenerator.GetIv(16);
            IBufferedCipher cf = CipherUtilities.GetCipher(derob);
            KeyParameter kp = new KeyParameter(keyp);
            byte[] iv = IvGenerator.GetIv(cf.GetBlockSize());
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            cf.Init(true, piv);
            int len = cf.DoFinal(inp, outp, 0);

            byte[] abyte1 = new byte[len];
            System.Array.Copy(outp, 0, abyte1, 0, len);
            DerOctetString deroctetstring = new DerOctetString(abyte1);
            KeyTransRecipientInfo keytransrecipientinfo = computeRecipientInfo(cert, keyp);
            DerSet derset = new DerSet(new RecipientInfo(keytransrecipientinfo));
            Asn1EncodableVector ev = new Asn1EncodableVector();
            ev.Add(new DerInteger(58));
            ev.Add(new DerOctetString(iv));
            DerSequence seq = new DerSequence(ev);
            AlgorithmIdentifier algorithmidentifier = new AlgorithmIdentifier(derob, seq);
            EncryptedContentInfo encryptedcontentinfo =
                new EncryptedContentInfo(PkcsObjectIdentifiers.Data, algorithmidentifier, deroctetstring);

            EnvelopedData env = new EnvelopedData(null, derset, encryptedcontentinfo, (Asn1Set)null);
            Org.BouncyCastle.Asn1.Cms.ContentInfo contentinfo =
                new Org.BouncyCastle.Asn1.Cms.ContentInfo(PkcsObjectIdentifiers.EnvelopedData, env);
            return contentinfo.ToAsn1Object();
        }
    }
}