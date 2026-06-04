using System.IO;
using iTextSharp.LGPLv2.Core.FunctionalTests.CryptoUtils;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class MakeSignatureCadesTests
{
    [TestMethod]
    public void Verify_CadesSignDetached_Basic()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = TestUtils.CreateSimplePdf();
        SignHelper(inputPdfPath, outputPdfPath, CryptoStandard.CADES);

        using var verifyReader = new PdfReader(outputPdfPath);
        var sigNames = verifyReader.AcroFields.GetSignatureNames();
        Assert.HasCount(1, sigNames);
        var pkcs7 = verifyReader.AcroFields.VerifySignature(sigNames[0]);
        Assert.IsNotNull(pkcs7);
    }

    [TestMethod]
    public void Verify_CadesSignDetached_SubFilter()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = TestUtils.CreateSimplePdf();
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
        var stream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write);
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
                var extSig = new ExternalSignature(
                    certs.PublicKey, certs.X509PrivateKeys, "SHA-256", "RSA");
                MakeSignature.SignDetached(sap, extSig, certs.X509PrivateKeys,
                    crlList: null, ocspClient: null, tsaClient: null,
                    estimatedSize: 0, sigtype);
            }
            finally { stamper.Close(); }
        }
        finally { stream.Dispose(); }
    }
}
