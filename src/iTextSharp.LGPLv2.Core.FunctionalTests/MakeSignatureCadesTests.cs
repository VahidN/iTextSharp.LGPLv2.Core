namespace iTextSharp.LGPLv2.Core.FunctionalTests;

using iTextSharp.LGPLv2.Core.FunctionalTests.CryptoUtils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MakeSignatureCadesTests
{
    [TestMethod]
    public void Verify_CmsSignDetached_Basic()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = CreateSimplePdf();
        SignHelper(inputPdfPath, outputPdfPath, CryptoStandard.CMS);

        using var verifyReader = new PdfReader(outputPdfPath);
        var names = verifyReader.AcroFields.GetSignatureNames();
        Assert.AreEqual(1, names.Count);
        var pkcs7 = verifyReader.AcroFields.VerifySignature(names[0]);
        Assert.IsNotNull(pkcs7);
    }

    [TestMethod]
    public void Verify_CadesSignDetached_Basic()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = CreateSimplePdf();
        SignHelper(inputPdfPath, outputPdfPath, CryptoStandard.CADES);

        using var verifyReader = new PdfReader(outputPdfPath);
        var af = verifyReader.AcroFields;
        var sigNames = af.GetSignatureNames();
        Assert.AreEqual(1, sigNames.Count);

        var pkcs7 = af.VerifySignature(sigNames[0]);
        Assert.IsNotNull(pkcs7);
    }

    [TestMethod]
    public void Verify_CadesSignDetached_SubFilter()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = CreateSimplePdf();
        SignHelper(inputPdfPath, outputPdfPath, CryptoStandard.CADES);

        using var verifyReader = new PdfReader(outputPdfPath);
        var af = verifyReader.AcroFields;
        var sigDict = af.GetSignatureDictionary(af.GetSignatureNames()[0]);
        Assert.IsNotNull(sigDict);

        var subFilter = sigDict.Get(PdfName.Subfilter) as PdfName;
        Assert.IsNotNull(subFilter);
        Assert.AreEqual(PdfName.EtsiCadesDetached.ToString(), subFilter.ToString());
    }

    private static void SignHelper(string inputPdfPath, string outputPdfPath, CryptoStandard sigtype)
    {
        var stream = new global::System.IO.FileStream(
            outputPdfPath, global::System.IO.FileMode.Create, global::System.IO.FileAccess.Write);
        try
        {
            using var reader = new PdfReader(inputPdfPath);
            var stamper = PdfStamper.CreateSignature(reader, stream, '\0', null, true);
            try
            {
                var certs = PfxReader.ReadCertificate(
                    TestUtils.GetPfxPath("cert123.pfx"), "123");

                var sap = stamper.SignatureAppearance;
                sap.Reason = "Test";
                sap.Location = "Test";

                var extSig = new TestExternalSignature(
                    certs.PublicKey, certs.X509PrivateKeys);

                MakeSignature.SignDetached(sap, extSig, certs.X509PrivateKeys,
                    crlList: null, ocspClient: null, tsaClient: null,
                    estimatedSize: 0, sigtype);
            }
            finally
            {
                stamper.Close();
            }
        }
        finally
        {
            stream.Dispose();
        }
    }

    private static string CreateSimplePdf()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new global::System.IO.FileStream(
            pdfFilePath, global::System.IO.FileMode.Create))
        {
            using (var document = new Document(PageSize.A4))
            {
                var writer = PdfWriter.GetInstance(document, fileStream);
                document.AddAuthor(TestUtils.Author);
                document.Open();
                document.Add(new Paragraph("Test Document"));
            }
        }

        return pdfFilePath;
    }

    private sealed class TestExternalSignature : IExternalSignature
    {
        private readonly Org.BouncyCastle.Crypto.ICipherParameters _privKey;

        public TestExternalSignature(
            Org.BouncyCastle.Crypto.ICipherParameters privKey,
            Org.BouncyCastle.X509.X509Certificate[] chain)
        {
            _privKey = privKey;
        }

        public string GetHashAlgorithm() => DigestAlgorithms.Sha256;

        public string GetEncryptionAlgorithm() => "RSA";

        public byte[] Sign(byte[] message)
        {
            var signer = Org.BouncyCastle.Security.SignerUtilities.GetSigner("RSA");
            signer.Init(true, _privKey);
            signer.BlockUpdate(message, 0, message.Length);
            return signer.GenerateSignature();
        }
    }
}
