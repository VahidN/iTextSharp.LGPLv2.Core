using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/17
/// </summary>
[TestClass]
public class Issue17
{
    [TestMethod]
    public void Verify_Issue17_CanBe_Processed()
    {
        var pdfDoc = new Document(PageSize.A4);

        var pdfFilePath = TestUtils.GetOutputFileName();
        var fileStream = new FileStream(pdfFilePath, FileMode.Create);
        PdfWriter.GetInstance(pdfDoc, fileStream);

        pdfDoc.AddAuthor(TestUtils.Author);
        pdfDoc.Open();

        var image = Image.GetInstance(TestUtils.GetImagePath("loa.jpg"));
        image.WidthPercentage = 60;
        image.Alignment = Element.ALIGN_RIGHT;

        var table = new PdfPTable(3);
        var cell = new PdfPCell( /*image: image, fit: false*/)
                   {
                       Colspan = 2,
                       //Border = 0,
                       HorizontalAlignment = Element.ALIGN_RIGHT,
                   };
        cell.AddElement(image);
        table.AddCell(cell);
        table.AddCell(new PdfPCell(new Phrase("Test...")));

        pdfDoc.Add(table);

        pdfDoc.Close();
        fileStream.Dispose();

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}