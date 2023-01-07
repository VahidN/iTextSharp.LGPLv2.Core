using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter06Tests
{
    [TestMethod]
    public void Verify_Concatenate_CanBeCreated()
    {
        var pdfFiles = new List<byte[]>
                       {
                           CreateSamplePdfFile("Hello World!"),
                           CreateSamplePdfFile("Hello DNT!"),
                       };

        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                using (var pdfCopy = new PdfCopy(document, stream))
                {
                    // step 3
                    document.AddAuthor(TestUtils.Author);
                    document.Open();

                    // step 4
                    foreach (var pdf in pdfFiles)
                    {
                        using var reader = new PdfReader(pdf);
                        var n = reader.NumberOfPages;
                        for (var page = 0; page < n;)
                        {
                            pdfCopy.AddPage(pdfCopy.GetImportedPage(reader, ++page));
                        }
                    }
                }
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_StampText_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            var pdfFile = CreateSamplePdfFile("Hello DNT!");
            using (var reader = new PdfReader(pdfFile))
            {
                using (var stamper = new PdfStamper(reader, stream))
                {
                    var canvas = stamper.GetOverContent(1);
                    ColumnText.ShowTextAligned(
                                               canvas,
                                               Element.ALIGN_LEFT,
                                               new Phrase("Hello people!"),
                                               36, 540, 0
                                              );
                }
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static byte[] CreateSamplePdfFile(string msg)
    {
        using var stream = new MemoryStream();
        using (var document = new Document())
        {
            // step 2
            PdfWriter.GetInstance(document, stream);
            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            document.Add(new Paragraph(msg));
        }

        return stream.ToArray();
    }
}