using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter02Tests
{
    [TestMethod]
    public void Verify_RiverPhoenix_CanBeCreated()
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
                var tahomaFont =
                    TestUtils.GetUnicodeFont("Tahoma", TestUtils.GetTahomaFontPath(), 10, Font.BOLD, BaseColor.Black);
                document.Add(new Paragraph(
                                           "Movies featuring River Phoenix", tahomaFont
                                          ));
                document.Add(CreateParagraph(
                                             "My favorite movie featuring River Phoenix was ", "0092005"
                                            ));
                document.Add(CreateParagraph(
                                             "River Phoenix was nominated for an academy award for his role in ",
                                             "0096018"
                                            ));
                document.Add(CreateParagraph(
                                             "River Phoenix played the young Indiana Jones in ", "0097576"
                                            ));
                document.Add(CreateParagraph(
                                             "His best role was probably in ", "0102494"
                                            ));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static Paragraph CreateParagraph(string msg, string imdb)
    {
        var p = new Paragraph(msg);
        var imagePath = TestUtils.GetPosterPath($"{imdb}.jpg");
        var img = Image.GetInstance(imagePath);
        img.ScaleToFit(1000, 72);
        img.RotationDegrees = -30;
        p.Add(new Chunk(img, 0, -15, true));
        return p;
    }
}