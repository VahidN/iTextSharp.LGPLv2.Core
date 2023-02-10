using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter12Tests
{
    private readonly string _ownerPassword = "World";
    private readonly string _userPassword = "Hello";

    [TestMethod]
    public void Verify_EncryptionPdf_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        var enc1 = CreatePdf();
        File.WriteAllBytes(Path.ChangeExtension(pdfFilePath, ".STANDARD_ENCRYPTION_128.pdf"), enc1);

        var enc2 = DecryptPdf(enc1);
        File.WriteAllBytes(Path.ChangeExtension(pdfFilePath, ".Decrypt_STANDARD_ENCRYPTION_128.pdf"), enc2);

        var enc3 = EncryptPdf(enc2);
        File.WriteAllBytes(Path.ChangeExtension(pdfFilePath, ".ENCRYPTION_AES_128_DO_NOT_ENCRYPT_METADATA.pdf"), enc3);
    }

    private byte[] CreatePdf()
    {
        using var output = new MemoryStream();
        // step 1
        using (var document = new Document())
        {
            var permissions = 0;
            permissions |= PdfWriter.AllowPrinting;
            permissions |= PdfWriter.AllowCopy;
            permissions |= PdfWriter.AllowScreenReaders;

            var writer = PdfWriter.GetInstance(document, output);
            writer.SetEncryption(PdfWriter.STANDARD_ENCRYPTION_128, _userPassword, _ownerPassword, permissions);
            document.AddAuthor(TestUtils.Author);
            writer.CreateXmpMetadata();
            // step 3
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello World"));
        }

        return output.ToArray();
    }

    private byte[] DecryptPdf(byte[] src)
    {
        using var output = new MemoryStream();
        using (var reader = new PdfReader(src, DocWriter.GetIsoBytes(_ownerPassword)))
        {
            using (var stamper = new PdfStamper(reader, output))
            {
            }
        }

        return output.ToArray();
    }

    private byte[] EncryptPdf(byte[] src)
    {
        using var output = new MemoryStream();
        using (var reader = new PdfReader(src))
        {
            using (var stamper = new PdfStamper(reader, output))
            {
                stamper.SetEncryption(PdfWriter.ENCRYPTION_AES_128 | PdfWriter.DO_NOT_ENCRYPT_METADATA,
                                      _userPassword, _ownerPassword,
                                      PdfWriter.ALLOW_PRINTING);
            }
        }

        return output.ToArray();
    }
}