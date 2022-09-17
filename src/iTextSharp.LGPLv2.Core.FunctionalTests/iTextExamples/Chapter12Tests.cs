using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples
{
    [TestClass]
    public class Chapter12Tests
    {
        private readonly string _ownerPassword = "World";
        private readonly string _userPassword = "Hello";

        [TestMethod]
        public void Verify_EncryptionPdf_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var enc1 = createPdf();
            File.WriteAllBytes(Path.ChangeExtension(pdfFilePath,".enc1.pdf"), enc1);

            var enc2 = decryptPdf(enc1);
            File.WriteAllBytes(Path.ChangeExtension(pdfFilePath, ".enc2.pdf"), enc2);

            var enc3 = encryptPdf(enc2);
            File.WriteAllBytes(Path.ChangeExtension(pdfFilePath, ".enc3.pdf"), enc3);
        }

        private byte[] createPdf()
        {
            using (var output = new MemoryStream())
            {
                // step 1
                var document = new Document();

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

                document.Close();

                return output.ToArray();
            }
        }

        private byte[] decryptPdf(byte[] src)
        {
            using (var output = new MemoryStream())
            {
                var reader = new PdfReader(src, DocWriter.GetIsoBytes(_ownerPassword));

                var stamper = new PdfStamper(reader, output);
                stamper.Close();
                reader.Close();

                return output.ToArray();
            }
        }

        private byte[] encryptPdf(byte[] src)
        {
            using (var output = new MemoryStream())
            {
                var reader = new PdfReader(src);
                var stamper = new PdfStamper(reader, output);

                stamper.SetEncryption(PdfWriter.ENCRYPTION_AES_128, _userPassword, _ownerPassword,
                                      PdfWriter.ALLOW_PRINTING);

                stamper.Close();                
                reader.Close();

                return output.ToArray();
            }
        }
    }
}