using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class PdfSmartCopyTests
{
    [TestMethod]
    public void Verify_Remove_Duplicate_Objects_Works()
    {
        var inputFile = CreateALargePdfFile();
        var outFile = TestUtils.GetOutputFileName();

        CompressPdfFileRemoveDuplicateObjects(inputFile, outFile);

        TestUtils.VerifyPdfFileIsReadable(outFile);
        Assert.IsTrue(new FileInfo(inputFile).Length > new FileInfo(outFile).Length);
    }

    private static void CompressPdfFileRemoveDuplicateObjects(string inputFile, string outFile)
    {
        using var fileStream = new FileStream(outFile, FileMode.Create);
        using var pdfDoc = new Document();
        var pdfSmartCopy = new PdfSmartCopy(pdfDoc, fileStream);
        pdfSmartCopy.SetFullCompression();

        pdfDoc.AddAuthor(TestUtils.Author);
        pdfDoc.Open();

        using var reader = new PdfReader(inputFile);

        var n = reader.NumberOfPages;
        for (var page = 0; page < n;)
        {
            pdfSmartCopy.AddPage(pdfSmartCopy.GetImportedPage(reader, ++page));
        }

        pdfSmartCopy.FreeReader(reader);
    }

    private string CreateALargePdfFile()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                var pdfWriter = PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                var table = new PdfPTable(new float[] { 1, 2 });
                // The same image has been added two times with new instances of it, instead of reusing its instance.
                // This will make the final output twice as big as it should be.
                var imagePath = TestUtils.GetImagePath("hitchcock.png");
                table.AddCell(Image.GetInstance(imagePath));
                table.AddCell(Image.GetInstance(imagePath));
                pdfDoc.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        return pdfFilePath;
    }
}