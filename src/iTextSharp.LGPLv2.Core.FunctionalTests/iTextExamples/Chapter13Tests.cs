using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples
{
    [TestClass]
    public class Chapter13Tests
    {
        [TestMethod]
        public void Verify_PdfXPdfA_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            // step 1
            var document = new Document();

            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PdfxConformance = PdfWriter.PDFX1A2001;
            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            const string arialTtf = @"c:\windows\fonts\arial.ttf";
            Font font = FontFactory.GetFont(
              arialTtf, BaseFont.CP1252, BaseFont.EMBEDDED, Font.UNDEFINED,
              Font.UNDEFINED, new CmykColor(255, 255, 0, 0)
            );
            document.Add(new Paragraph("Hello World", font));

            document.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }


        [TestMethod]
        public void Verify_PrintPreferencesExample_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            var document = new Document();

            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PdfVersion = PdfWriter.VERSION_1_5;
            writer.AddViewerPreference(PdfName.Printscaling, PdfName.None);
            writer.AddViewerPreference(PdfName.Numcopies, new PdfNumber(3));
            writer.AddViewerPreference(PdfName.Picktraybypdfsize, PdfBoolean.Pdftrue);
            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello World!"));

            document.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }
    }
}