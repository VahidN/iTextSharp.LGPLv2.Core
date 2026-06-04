using System.IO;
using iTextSharp.LGPLv2.Core.FunctionalTests.CryptoUtils;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class MakeSignatureTests
{
    [TestMethod]
    public void Verify_SignDetached_Cms()
    {
        var (reader, pkcs7) = SignAndVerify(CryptoStandard.CMS);
        using (reader)
        {
            Assert.IsNotNull(pkcs7);
            Assert.HasCount(1, reader.AcroFields.GetSignatureNames());
        }
    }

    [TestMethod]
    public void Verify_SignDetached_Cades()
    {
        var (reader, pkcs7) = SignAndVerify(CryptoStandard.CADES);
        using (reader)
        {
            Assert.IsNotNull(pkcs7);
            var sigDict = reader.AcroFields.GetSignatureDictionary(
                reader.AcroFields.GetSignatureNames()[0]);
            var subFilter = sigDict.Get(PdfName.Subfilter) as PdfName;
            Assert.AreEqual(PdfName.EtsiCadesDetached.ToString(), subFilter.ToString());
        }
    }

    [TestMethod]
    public void Verify_SignExternalContainer()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = TestUtils.CreateSimplePdf();
        var certs = PfxReader.ReadCertificate(TestUtils.GetPfxPath("cert123.pfx"), "123");

        var stream = new FileStream(
            outputPdfPath, FileMode.Create, FileAccess.Write);
        try
        {
            using var reader = new PdfReader(inputPdfPath);
            var stamper = PdfStamper.CreateSignature(reader, stream, '\0', null, true);
            try
            {
                var sap = stamper.SignatureAppearance;
                sap.Reason = "External Container Test";
                var container = new PreSignedContainer(
                    certs.PublicKey, certs.X509PrivateKeys, "SHA-256");
                MakeSignature.SignExternalContainer(sap, container, estimatedSize: 8192);
            }
            finally { stamper.Close(); }
        }
        finally { stream.Dispose(); }

        using var verifyReader = new PdfReader(outputPdfPath);
        var names = verifyReader.AcroFields.GetSignatureNames();
        Assert.HasCount(1, names);
    }

    [TestMethod]
    public void Verify_SignDetached_Invisible()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = TestUtils.CreateSimplePdf();
        var certs = PfxReader.ReadCertificate(TestUtils.GetPfxPath("cert123.pfx"), "123");

        var stream = new FileStream(
            outputPdfPath, FileMode.Create, FileAccess.Write);
        try
        {
            using var reader = new PdfReader(inputPdfPath);
            var stamper = PdfStamper.CreateSignature(reader, stream, '\0', null, true);
            try
            {
                var sap = stamper.SignatureAppearance;
                sap.Reason = "Invisible";
                var extSig = new ExternalSignature(
                    certs.PublicKey, certs.X509PrivateKeys, "SHA-256", "RSA");
                MakeSignature.SignDetached(sap, extSig, certs.X509PrivateKeys,
                    crlList: null, ocspClient: null, tsaClient: null,
                    estimatedSize: 0, CryptoStandard.CMS);
            }
            finally { stamper.Close(); }
        }
        finally { stream.Dispose(); }

        using var verifyReader = new PdfReader(outputPdfPath);
        Assert.HasCount(1, verifyReader.AcroFields.GetSignatureNames());
    }

    [TestMethod]
    public void Verify_SignDetached_InsufficientSize_Throws()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = TestUtils.CreateSimplePdf();
        var certs = PfxReader.ReadCertificate(TestUtils.GetPfxPath("cert123.pfx"), "123");

        var stream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write);
        try
        {
            using var reader = new PdfReader(inputPdfPath);
            var stamper = PdfStamper.CreateSignature(reader, stream, '\0', null, true);
            try
            {
                var sap = stamper.SignatureAppearance;
                var extSig = new ExternalSignature(
                    certs.PublicKey, certs.X509PrivateKeys, "SHA-256", "RSA");
                try
                {
                    MakeSignature.SignDetached(sap, extSig, certs.X509PrivateKeys,
                        crlList: null, ocspClient: null, tsaClient: null,
                        estimatedSize: 10, CryptoStandard.CMS);
                    Assert.Fail("Expected IOException was not thrown");
                }
                catch (IOException ex)
                {
                    Assert.Contains("Not enough space", ex.Message);
                }
            }
            finally { stamper.Close(); }
        }
        finally { stream.Dispose(); }
    }

    [TestMethod]
    public void Verify_SignDetached_Metadata()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = TestUtils.CreateSimplePdf();
        var certs = PfxReader.ReadCertificate(TestUtils.GetPfxPath("cert123.pfx"), "123");

        var stream = new FileStream(
            outputPdfPath, FileMode.Create, FileAccess.Write);
        try
        {
            using var reader = new PdfReader(inputPdfPath);
            var stamper = PdfStamper.CreateSignature(reader, stream, '\0', null, true);
            try
            {
                var sap = stamper.SignatureAppearance;
                sap.Reason = "Metadata Test Reason";
                sap.Location = "Metadata Test Location";
                sap.Contact = "contact@test.com";

                var extSig = new ExternalSignature(
                    certs.PublicKey, certs.X509PrivateKeys, "SHA-256", "RSA");
                MakeSignature.SignDetached(sap, extSig, certs.X509PrivateKeys,
                    crlList: null, ocspClient: null, tsaClient: null,
                    estimatedSize: 0, CryptoStandard.CMS);
            }
            finally { stamper.Close(); }
        }
        finally { stream.Dispose(); }

        using var verifyReader = new PdfReader(outputPdfPath);
        var pkcs7 = verifyReader.AcroFields.VerifySignature(
            verifyReader.AcroFields.GetSignatureNames()[0]);
        Assert.IsNotNull(pkcs7);
        Assert.AreEqual("Metadata Test Reason", pkcs7.Reason);
        Assert.AreEqual("Metadata Test Location", pkcs7.Location);
    }

    [TestMethod]
    public void Verify_CreateSignature_Regression()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = TestUtils.CreateSimplePdf();
        using var stream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write);
        using var reader = new PdfReader(inputPdfPath);
        using var stamper = PdfStamper.CreateSignature(reader, stream, '\0', null, true);
        var certs = PfxReader.ReadCertificate(TestUtils.GetPfxPath("cert123.pfx"), "123");
        var sap = stamper.SignatureAppearance;
        sap.Reason = "Regression";
        sap.SetCrypto(certs.PublicKey, certs.X509PrivateKeys, null, PdfSignatureAppearance.SelfSigned);
    }

    private static (PdfReader Reader, PdfPkcs7 Pkcs7) SignAndVerify(CryptoStandard sigtype)
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = TestUtils.CreateSimplePdf();
        var certs = PfxReader.ReadCertificate(TestUtils.GetPfxPath("cert123.pfx"), "123");

        var stream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write);
        try
        {
            using var reader = new PdfReader(inputPdfPath);
            var stamper = PdfStamper.CreateSignature(reader, stream, '\0', null, true);
            try
            {
                var sap = stamper.SignatureAppearance;
                sap.Reason = "SignDetached Test";
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

        var verifyReader = new PdfReader(outputPdfPath);
        var names = verifyReader.AcroFields.GetSignatureNames();
        var pkcs7 = verifyReader.AcroFields.VerifySignature(names[0]);
        return (verifyReader, pkcs7);
    }
}
