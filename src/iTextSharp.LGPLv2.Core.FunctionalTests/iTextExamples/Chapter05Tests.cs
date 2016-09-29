using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples
{
    [TestClass]
    public class Chapter05Tests
    {
        [TestMethod]
        public void Verify_Hero1_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            // step 1
            Rectangle rect = new Rectangle(-1192, -1685, 1192, 1685);
            var document = new Document(rect);

            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            PdfContentByte content = writer.DirectContent;
            PdfTemplate template = createTemplate(content, rect, 4);
            content.AddTemplate(template, -1192, -1685);
            content.MoveTo(-595, 0);
            content.LineTo(595, 0);
            content.MoveTo(0, -842);
            content.LineTo(0, 842);
            content.Stroke();

            document.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }

        [TestMethod]
        public void Verify_NewPage_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            // step 1
            var document = new Document();

            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
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

            document.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }

        private static PdfTemplate createTemplate(PdfContentByte content, Rectangle rect, int factor)
        {
            PdfTemplate template = content.CreateTemplate(rect.Width, rect.Height);
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
}