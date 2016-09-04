using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples
{
    [TestClass]
    public class Chapter07Tests
    {
        [TestMethod]
        public void Verify_NamedActions_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            // Create a reader
            var pdfFile = createSamplePdfFile();
            var reader = new PdfReader(pdfFile);

            // Create a stamper
            var stamper = new PdfStamper(reader, stream);

            var table = createTableWithNamedActions();

            // Add the table to each page
            for (var i = 0; i < reader.NumberOfPages;)
            {
                var canvas = stamper.GetOverContent(++i);
                table.WriteSelectedRows(0, -1, 696, 36, canvas);
            }

            reader.Close();
            stamper.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }

        private static byte[] createSamplePdfFile()
        {
            using (var stream = new MemoryStream())
            {
                // step 1
                var document = new Document(PageSize.A4);

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

                document.Close();

                return stream.ToArray();
            }
        }

        private static PdfPTable createTableWithNamedActions()
        {
            Font symbol = new Font(Font.SYMBOL, 20);
            PdfPTable table = new PdfPTable(4);
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            Chunk first = new Chunk(((char) 220).ToString(), symbol);
            first.SetAction(new PdfAction(PdfAction.FIRSTPAGE));
            table.AddCell(new Phrase(first));
            Chunk previous = new Chunk(((char) 172).ToString(), symbol);
            previous.SetAction(new PdfAction(PdfAction.PREVPAGE));
            table.AddCell(new Phrase(previous));
            Chunk next = new Chunk(((char) 174).ToString(), symbol);
            next.SetAction(new PdfAction(PdfAction.NEXTPAGE));
            table.AddCell(new Phrase(next));
            Chunk last = new Chunk(((char) 222).ToString(), symbol);
            last.SetAction(new PdfAction(PdfAction.LASTPAGE));
            table.AddCell(new Phrase(last));
            table.TotalWidth = 120;
            return table;
        }
    }
}