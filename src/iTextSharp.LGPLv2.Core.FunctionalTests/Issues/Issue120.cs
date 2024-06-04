using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/120
/// </summary>
[TestClass]
public class Issue129
{
    [TestMethod]
    public void Test_Issue120_Verify_Page_Split_On_Nested_PdfPTable_Works()
    {
        var tempFilename = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(tempFilename, FileMode.Create))
        {
            var document = new Document(PageSize.A4, 10, 10, 10, 10);

            var writer = PdfWriter.GetInstance(document, stream);

            document.AddCreator("TEST");
            document.Open();
            document.NewPage();

            var docTable = new PdfPTable(1);
            docTable.DefaultCell.Padding = 0;
            docTable.DefaultCell.PaddingTop = 15; // required to cause the bug in this case

            var innerTable = new PdfPTable(1);
            innerTable.DefaultCell.Padding = 3;

            for (var i = 0; i < 300; i++)
            {
                innerTable.AddCell(new Phrase($"test row {i}"));
            }

            docTable.AddCell(innerTable);

            document.Add(docTable);

            document.Close();
        }
    }
}