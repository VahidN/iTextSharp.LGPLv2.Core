using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter03Tests
{
    [TestMethod]
    public void Verify_FestivalOpening_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document(PageSize.Postcard, 30, 30, 30, 30))
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                // Create and add a Paragraph
                var p = new Paragraph(
                                      "Foobar Film Festival",
                                      new Font(BaseFont.CreateFont(), 22)
                                     )
                        { Alignment = Element.ALIGN_CENTER };
                document.Add(p);
                // Create and add an Image
                var img = Image.GetInstance(TestUtils.GetImagePath("loa.jpg"));
                img.SetAbsolutePosition(
                                        (PageSize.Postcard.Width - img.ScaledWidth) / 2,
                                        (PageSize.Postcard.Height - img.ScaledHeight) / 2
                                       );
                document.Add(img);
                // Now we go to the next page
                document.NewPage();
                document.Add(p);
                document.Add(img);
                // Add text on top of the image
                var over = writer.DirectContent;
                over.SaveState();
                var sinus = (float)Math.Sin(Math.PI / 60);
                var cosinus = (float)Math.Cos(Math.PI / 60);
                var bf = BaseFont.CreateFont();
                over.BeginText();
                over.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE);
                over.SetLineWidth(1.5f);
                over.SetRgbColorStroke(0xFF, 0x00, 0x00);
                over.SetRgbColorFill(0xFF, 0xFF, 0xFF);
                over.SetFontAndSize(bf, 36);
                over.SetTextMatrix(cosinus, sinus, -sinus, cosinus, 50, 324);
                over.ShowText("SOLD OUT");
                over.EndText();
                over.RestoreState();
                // Add a rectangle under the image
                var under = writer.DirectContentUnder;
                under.SaveState();
                under.SetRgbColorFill(0xFF, 0xD7, 0x00);
                under.Rectangle(5, 5,
                                PageSize.Postcard.Width - 10,
                                PageSize.Postcard.Height - 10
                               );
                under.Fill();
                under.RestoreState();
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_GraphicsStateStack_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document(new Rectangle(200, 120)))
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var canvas = writer.DirectContent;
                // state 1:
                canvas.SetRgbColorFill(0xFF, 0x45, 0x00);
                // fill a rectangle in state 1
                canvas.Rectangle(10, 10, 60, 60);
                canvas.Fill();
                canvas.SaveState();
                // state 2;
                canvas.SetLineWidth(3);
                canvas.SetRgbColorFill(0x8B, 0x00, 0x00);
                // fill and stroke a rectangle in state 2
                canvas.Rectangle(40, 20, 60, 60);
                canvas.FillStroke();
                canvas.SaveState();
                // state 3:
                canvas.ConcatCtm(1, 0, 0.1f, 1, 0, 0);
                canvas.SetRgbColorStroke(0xFF, 0x45, 0x00);
                canvas.SetRgbColorFill(0xFF, 0xD7, 0x00);
                // fill and stroke a rectangle in state 3
                canvas.Rectangle(70, 30, 60, 60);
                canvas.FillStroke();
                canvas.RestoreState();
                // stroke a rectangle in state 2
                canvas.Rectangle(100, 40, 60, 60);
                canvas.Stroke();
                canvas.RestoreState();
                // fill and stroke a rectangle in state 1
                canvas.Rectangle(130, 50, 60, 60);
                canvas.FillStroke();
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_ImageDirect_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document(PageSize.Postcard, 30, 30, 30, 30))
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                writer.CompressionLevel = 0;
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var img = Image.GetInstance(TestUtils.GetImagePath("loa.jpg"));
                img.SetAbsolutePosition(
                                        (PageSize.Postcard.Width - img.ScaledWidth) / 2,
                                        (PageSize.Postcard.Height - img.ScaledHeight) / 2
                                       );
                writer.DirectContent.AddImage(img);
                var p = new Paragraph(
                                      "Foobar Film Festival",
                                      new Font(BaseFont.CreateFont(), 22)
                                     )
                        { Alignment = Element.ALIGN_CENTER };
                document.Add(p);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_ImageInline_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document(PageSize.Postcard, 30, 30, 30, 30))
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                writer.CompressionLevel = 0;
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var img = Image.GetInstance(TestUtils.GetImagePath("loa.jpg"));
                img.SetAbsolutePosition(
                                        (PageSize.Postcard.Width - img.ScaledWidth) / 2,
                                        (PageSize.Postcard.Height - img.ScaledHeight) / 2
                                       );
                writer.DirectContent.AddImage(img, true);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_ImageSkew_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document(PageSize.Postcard.Rotate()))
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                writer.CompressionLevel = 0;
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var img = Image.GetInstance(TestUtils.GetImagePath("loa.jpg"));
                // Add the image to the upper layer
                writer.DirectContent.AddImage(
                                              img,
                                              img.Width, 0, 0.35f * img.Height,
                                              0.65f * img.Height, 30, 30
                                             );
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}