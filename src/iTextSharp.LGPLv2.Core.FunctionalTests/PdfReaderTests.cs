using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests
{
    [TestClass]
    public class PdfReaderTests
    {
        [TestMethod]
        public void Detect_Blank_Pages_In_Pdf()
        {
            // value where we can consider that this is a blank image
            // can be much higher or lower depending of what is considered as a blank page
            const int blankThreshold = 20;

            var pdfFile = createSamplePdfFile();
            var reader = new PdfReader(pdfFile);

            var blankPages = 0;
            for (var pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++)
            {
                // first check, examine the resource dictionary for /Font or /XObject keys.
                // If either are present -> not blank.
                var pageDict = reader.GetPageN(pageNum);
                var resDict = (PdfDictionary)pageDict.Get(PdfName.Resources);

                var hasFont = resDict.Get(PdfName.Font) != null;
                if (hasFont)
                {
                    Console.WriteLine($"Page {pageNum} has font(s).");
                    continue;
                }

                var hasImage = resDict.Get(PdfName.Xobject) != null;
                if (hasImage)
                {
                    Console.WriteLine($"Page {pageNum} has image(s).");
                    continue;
                }

                var content = reader.GetPageContent(pageNum);
                if (content.Length <= blankThreshold)
                {
                    Console.WriteLine($"Page {pageNum} is blank");
                    blankPages++;
                }
            }

            reader.Close();

            Assert.AreEqual(expected: 1, actual: blankPages, message: $"{reader.NumberOfPages} page(s) with {blankPages} blank page(s).");
        }

        private static byte[] createSamplePdfFile()
        {
            using (var stream = new MemoryStream())
            {
                var document = new Document();

                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph("Hello DNT!"));

                document.NewPage();
                // we don't add anything to this page: newPage() will be ignored
                document.Add(new Phrase(""));

                document.NewPage();
                writer.PageEmpty = false;

                document.Close();
                return stream.ToArray();
            }
        }
    }
}