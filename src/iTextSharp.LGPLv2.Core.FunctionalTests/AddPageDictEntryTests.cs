using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class AddPageDictEntryTests
{
    [TestMethod]
    public void Verify_Equivalent_Of_AddPageDictEntry_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A6.Rotate()))
            {
                var pdfWriter = PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                pdfWriter.PageDictionary.Put(PdfName.Rotate, new PdfNumber(0));
                pdfDoc.Add(new Chunk("Rotate a page and its content"));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}