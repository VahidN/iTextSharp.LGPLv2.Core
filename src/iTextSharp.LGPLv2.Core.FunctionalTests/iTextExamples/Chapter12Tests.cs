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
        readonly byte[] _ownerPassword = Encoding.UTF8.GetBytes("World");
        readonly byte[] _userPassword = Encoding.UTF8.GetBytes("Hello");

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
            using (var ms = new MemoryStream())
            {
                // step 1
                var document = new Document();

                var permissions = 0;
                permissions |= PdfWriter.AllowPrinting;
                permissions |= PdfWriter.AllowCopy;
                permissions |= PdfWriter.AllowScreenReaders;

                var writer = PdfWriter.GetInstance(document, ms);
                writer.SetEncryption(
                  _userPassword, _ownerPassword,
                  permissions,
                  PdfWriter.STANDARD_ENCRYPTION_128
                );
                document.AddAuthor(TestUtils.Author);
                writer.CreateXmpMetadata();
                // step 3
                document.Open();
                // step 4
                document.Add(new Paragraph("Hello World"));

                document.Close();

                return ms.ToArray();
            }
        }

        private byte[] decryptPdf(byte[] src)
        {
            using (var ms = new MemoryStream())
            {
                var reader = new PdfReader(src, _ownerPassword);

                var stamper = new PdfStamper(reader, ms);
                stamper.Close();
                reader.Close();

                return ms.ToArray();
            }
        }

        private byte[] encryptPdf(byte[] src)
        {
            using (var ms = new MemoryStream())
            {
                var reader = new PdfReader(src);

                var stamper = new PdfStamper(reader, ms);

                stamper.SetEncryption(
                  _userPassword, _ownerPassword,
                  PdfWriter.ALLOW_PRINTING,
                  PdfWriter.ENCRYPTION_AES_128 | PdfWriter.DO_NOT_ENCRYPT_METADATA
                );

                stamper.Close();
                reader.Close();

                return ms.ToArray();
            }
        }
    }
}