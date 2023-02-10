using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter01Tests
{
    [TestMethod]
    public void Verify_Normal_PDF_File_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                var chunk = new Chunk("Test1");
                pdfDoc.Add(chunk);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorld_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph("Hello World!"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldColumn_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                // we set the compression to 0 so that we can read the PDF syntax
                writer.CompressionLevel = 0;
                // writes something to the direct content using a convenience method
                var hello = new Phrase("HelloWorldColumn");
                var canvas = writer.DirectContentUnder;
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT,
                                           hello, 36, 788, 0
                                          );
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldDirect_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var canvas = writer.DirectContentUnder;
                writer.CompressionLevel = 0;
                canvas.SaveState(); // q
                canvas.BeginText(); // BT
                canvas.MoveText(36, 788); // 36 788 Td
                canvas.SetFontAndSize(BaseFont.CreateFont(), 12); // /F1 12 Tf
                canvas.ShowText("HelloWorldDirect"); // (Hello World)Tj
                canvas.EndText(); // ET
                canvas.RestoreState(); // Q
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldLandscape1_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            // landscape format using the rotate() method
            using (var document = new Document(PageSize.Letter.Rotate()))
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph("HelloWorldLandscape1"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldLandscape2_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            // landscape format because width > height
            using (var document = new Document(new Rectangle(792, 612)))
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph("HelloWorldLandscape2"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldLetter_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            // Specifying the page size
            using (var document = new Document(PageSize.Letter))
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph("HelloWorldLetter"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldMaximum_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            // Specifying the page size
            using (var document = new Document(new Rectangle(14400, 14400)))
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // changes the user unit
                writer.Userunit = 75000f;
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph("HelloWorldMaximum"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldMemory_CanBeCreated()
    {
        using var ms = new MemoryStream();
        // step 1
        using (var document = new Document())
        {
            // step 2
            PdfWriter.GetInstance(document, ms);
            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            document.Add(new Paragraph("HelloWorldMemory"));
        }

        var pdfBytes = ms.ToArray();
        TestUtils.VerifyPdfFileIsReadable(pdfBytes);
    }

    [TestMethod]
    public void Verify_HelloWorldMirroredMargins_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                document.SetPageSize(PageSize.A5);
                document.SetMargins(36, 72, 108, 180);
                document.SetMarginMirroring(true);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph(
                                           "The left margin of this odd page is 36pt (0.5 inch); " +
                                           "the right margin 72pt (1 inch); " +
                                           "the top margin 108pt (1.5 inch); " +
                                           "the bottom margin 180pt (2.5 inch).")
                            );
                var paragraph = new Paragraph { Alignment = Element.ALIGN_JUSTIFIED };
                for (var i = 0; i < 20; i++)
                {
                    paragraph.Add(new Chunk("Hello World! Hello People! " +
                                            "Hello Sky! Hello Sun! Hello Moon! Hello Stars!"));
                }

                document.Add(paragraph);
                document.Add(new Paragraph(
                                           "The right margin of this even page is 36pt (0.5 inch); " +
                                           "the left margin 72pt (1 inch).")
                            );
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldMirroredMarginsTop_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            // Specifying the page size
            using (var document = new Document(PageSize.Letter))
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // setting page size, margins and mirrered margins
                document.SetPageSize(PageSize.A5);
                document.SetMargins(36, 72, 108, 180);
                document.SetMarginMirroringTopBottom(true);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph(
                                           "The left margin of this odd page is 36pt (0.5 inch); " +
                                           "the right margin 72pt (1 inch); " +
                                           "the top margin 108pt (1.5 inch); " +
                                           "the bottom margin 180pt (2.5 inch).")
                            );
                var paragraph = new Paragraph { Alignment = Element.ALIGN_JUSTIFIED };
                for (var i = 0; i < 20; i++)
                {
                    paragraph.Add(new Chunk("Hello World! Hello People! " +
                                            "Hello Sky! Hello Sun! Hello Moon! Hello Stars!"));
                }

                document.Add(paragraph);
                document.Add(new Paragraph("The top margin 180pt (2.5 inch)."));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldNarrow_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            // Using a custom page size
            var pagesize = new Rectangle(216f, 720f);
            using (var document = new Document(pagesize, 36f, 72f, 108f, 180f))
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph("Hello World! Hello People! " +
                                           "Hello Sky! Hello Sun! Hello Moon! Hello Stars!"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_HelloWorldVersion_1_7_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                // Creating a PDF 1.7 document
                var writer = PdfWriter.GetInstance(document, stream);
                writer.PdfVersion = PdfWriter.VERSION_1_7;
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(new Paragraph("HelloWorldVersion_1_7"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}