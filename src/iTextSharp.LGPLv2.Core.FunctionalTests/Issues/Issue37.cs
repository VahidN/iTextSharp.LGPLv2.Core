using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/37
/// </summary>
[TestClass]
public class Issue37
{
    [TestMethod]
    public void Verify_Issue37_CanBe_Processed()
    {
        var pdfDoc = new Document(PageSize.A4);

        var pdfFilePath = TestUtils.GetOutputFileName();
        var fileStream = new FileStream(pdfFilePath, FileMode.Create);
        var writer = PdfWriter.GetInstance(pdfDoc, fileStream);

        pdfDoc.AddAuthor(TestUtils.Author);
        pdfDoc.Open();

        var ct = new ColumnText(writer.DirectContent);

        var text = new Phrase("TEST paragraph\nAfter Newline");
        text.Add(new Chunk("\u00A0Test Test\u00A0"));
        text.Add(new Chunk("\nNew line"));

        ct.SetSimpleColumn(
                           text,
                           34, 750, 580, 317,
                           15,
                           Element.ALIGN_LEFT);
        ct.Go();

        pdfDoc.Close();
        fileStream.Dispose();

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}