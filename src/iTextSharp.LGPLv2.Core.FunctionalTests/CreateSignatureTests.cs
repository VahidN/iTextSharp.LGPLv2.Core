using System.IO;
using System.util;
using iTextSharp.LGPLv2.Core.FunctionalTests.CryptoUtils;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class CreateSignatureTests
{
    private const string OwnerPassword = "owner";
    private const string UserPassword = "user";

    [TestMethod]
    public void Verify_CreateSignature()
    {
        var outputPdfPath = TestUtils.GetOutputFileName();
        var inputPdfPath = CreateSimplePdfFile();
        using var stream = new FileStream(outputPdfPath, FileMode.Create, FileAccess.Write);
        using var reader = new PdfReader(inputPdfPath, OwnerPassword.GetIsoBytes());
        using var stamper = PdfStamper.CreateSignature(reader, stream, '\0', null, true);
        var certs = PfxReader.ReadCertificate(TestUtils.GetPfxPath("cert123.pfx"), "123");
        SignWithoutTimeStampAuthority(stamper, certs.PublicKey, certs.X509PrivateKeys);
    }

    private string CreateSimplePdfFile()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var document = new Document(PageSize.A4))
            {
                var permissions = 0;
                permissions |= PdfWriter.AllowPrinting;
                permissions |= PdfWriter.AllowCopy;
                permissions |= PdfWriter.AllowScreenReaders;

                var writer = PdfWriter.GetInstance(document, fileStream);
                writer.SetEncryption(UserPassword.GetIsoBytes(), OwnerPassword.GetIsoBytes(), permissions,
                                     PdfWriter.ENCRYPTION_AES_256_V3);
                document.AddAuthor(TestUtils.Author);
                // step 3
                document.Open();
                // step 4
                document.Add(new Paragraph("Hello World"));
            }
        }

        return pdfFilePath;
    }

    private void SignWithoutTimeStampAuthority(PdfStamper stamper,
                                               ICipherParameters privKey,
                                               X509Certificate[] certChain)
    {
        var signAppearance = stamper.SignatureAppearance;
        signAppearance.Reason = "Reason ...";
        signAppearance.Contact = "Contact ...";
        signAppearance.Location = "Location ...";
        AddVisibleSignature(signAppearance, stamper);
        SignDetached(signAppearance, privKey, certChain);
    }

    private void AddVisibleSignature(PdfSignatureAppearance signAppearance,
                                     PdfStamper stamper)
    {
        signAppearance.Image = null;
        signAppearance.Layer2Text = "CustomText ...";
        signAppearance.RunDirection = PdfWriter.RUN_DIRECTION_LTR;
        // signAppearance.Layer2Font = ...
        var pageNumber = stamper.Reader.NumberOfPages;
        signAppearance.SetVisibleSignature(new Rectangle(0, 0, 100, 100), pageNumber, null);
    }

    private void SignDetached(PdfSignatureAppearance sap,
                              ICipherParameters privKey,
                              X509Certificate[] certChain)
    {
        sap.CertificationLevel = PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED;
        sap.SetCrypto(privKey, certChain, null, PdfSignatureAppearance.SelfSigned);
    }
}