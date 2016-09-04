using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples
{
    [TestClass]
    public class Chapter06Tests
    {
        [TestMethod]
        public void Verify_Concatenate_CanBeCreated()
        {
            var pdfFiles = new List<byte[]>
            {
                createSamplePdfFile("Hello World!"),
                createSamplePdfFile("Hello DNT!")
            };

            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            // step 1
            var document = new Document();

            // step 2
            var pdfCopy = new PdfCopy(document, stream);

            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();

            // step 4
            foreach (var pdf in pdfFiles)
            {
                var reader = new PdfReader(pdf);
                var n = reader.NumberOfPages;
                for (var page = 0; page < n;)
                {
                    pdfCopy.AddPage(pdfCopy.GetImportedPage(reader, ++page));
                }
                reader.Close();
            }

            document.Close();
            pdfCopy.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }

        [TestMethod]
        public void Verify_StampText_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            var pdfFile = createSamplePdfFile("Hello DNT!");
            var reader = new PdfReader(pdfFile);
            var stamper = new PdfStamper(reader, stream);

            var canvas = stamper.GetOverContent(1);
            ColumnText.ShowTextAligned(
              canvas,
              Element.ALIGN_LEFT,
              new Phrase("Hello people!"),
              36, 540, 0
            );

            reader.Close();
            stamper.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }

        private static byte[] createSamplePdfFile(string msg)
        {
            using (var stream = new MemoryStream())
            {
                var document = new Document();

                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph(msg));

                document.Close();
                return stream.ToArray();
            }
        }
    }
}