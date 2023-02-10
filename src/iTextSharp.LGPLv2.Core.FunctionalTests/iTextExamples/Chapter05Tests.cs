using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter05Tests
{
    [TestMethod]
    public void Verify_Hero1_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            var rect = new Rectangle(-1192, -1685, 1192, 1685);
            using (var document = new Document(rect))
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var content = writer.DirectContent;
                var template = CreateTemplate(content, rect, 4);
                content.AddTemplate(template, -1192, -1685);
                content.MoveTo(-595, 0);
                content.LineTo(595, 0);
                content.MoveTo(0, -842);
                content.LineTo(0, 842);
                content.Stroke();
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_NewPage_CanBeCreated()
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
                document.Add(new Paragraph("This page will NOT be followed by a blank page!"));
                document.NewPage();
                // we don't add anything to this page: newPage() will be ignored
                document.NewPage();
                document.Add(new Paragraph("This page will be followed by a blank page!"));
                document.NewPage();
                writer.PageEmpty = false;
                document.NewPage();
                document.Add(new Paragraph("The previous page was a blank page!"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static PdfTemplate CreateTemplate(PdfContentByte content, Rectangle rect, int factor)
    {
        var template = content.CreateTemplate(rect.Width, rect.Height);
        template.ConcatCtm(factor, 0, 0, factor, 0, 0);

        var hero = TestUtils.GetTxtPath("hero.txt");
        if (!File.Exists(hero))
        {
            throw new FileNotFoundException($"{hero} NOT FOUND!");
        }

        var fi = new FileInfo(hero);
        using (var sr = fi.OpenText())
        {
            while (sr.Peek() >= 0)
            {
                template.SetLiteral((char)sr.Read());
            }
        }

        return template;
    }
}