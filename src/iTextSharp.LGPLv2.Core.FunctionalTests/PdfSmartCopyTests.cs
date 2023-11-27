using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class PdfSmartCopyTests
{
    [TestMethod]
    public void Verify_Remove_Duplicate_Streams_Works()
    {
        var inputFile = CreateALargePdfFile();
        var outFile = TestUtils.GetOutputFileName();

        CompressPdfFileRemoveDuplicateObjects(inputFile, outFile);

        TestUtils.VerifyPdfFileIsReadable(outFile);
        Assert.IsTrue(new FileInfo(inputFile).Length > new FileInfo(outFile).Length);
    }

    [TestMethod]
    public void Verify_Remove_Duplicate_Dictionaries_Works()
    {
        var inputFile = CreatePdfFileWithEmbeddedFont();
        var outFile = TestUtils.GetOutputFileName();

        CompressMultiplePdfFilesRemoveDuplicateObjects(inputFile, outFile);

        TestUtils.VerifyPdfFileIsReadable(outFile);

        using var reader = new PdfReader(outFile);
        var fontCount = GetPdfObjects(reader)
            .OfType<PdfDictionary>()
            .Select(d => d.GetDirectObject(PdfName.TYPE))
            .Where(PdfName.Fontdescriptor.Equals)
            .Count();

        Assert.AreEqual(1, fontCount);
    }

    private static void CompressPdfFileRemoveDuplicateObjects(string inputFile, string outFile)
    {
        CompressMultiplePdfFilesRemoveDuplicateObjects(inputFile, outFile, 1);
    }

    private string CreateALargePdfFile()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                PdfWriter.GetInstance(pdfDoc, fileStream);

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

    private string CreatePdfFileWithEmbeddedFont()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                PdfWriter.GetInstance(pdfDoc, fileStream);
                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();
                
                var font =  TestUtils.GetUnicodeFont("Tahoma", TestUtils.GetTahomaFontPath(), 10, Font.NORMAL, BaseColor.Black);
                pdfDoc.Add(new Paragraph("Document with embedded font", font));
            }
        }
        
        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        return pdfFilePath;
    }
    
        
    private static void CompressMultiplePdfFilesRemoveDuplicateObjects(string inputFile, string outFile, int times = 10)
    {
        using var fileStream = new FileStream(outFile, FileMode.Create);
        using var pdfDoc = new Document();
        var pdfSmartCopy = new PdfSmartCopy(pdfDoc, fileStream);
        pdfSmartCopy.SetFullCompression();

        pdfDoc.AddAuthor(TestUtils.Author);
        pdfDoc.Open();

        // The same document has been added multiple times
        // This will cause duplicate dictionaries (ex: FontDescriptors)
        for (var i = 0; i < times; ++i)
        {
            using var reader = new PdfReader(inputFile);

            var n = reader.NumberOfPages;
            for (var page = 0; page < n;)
            {
                pdfSmartCopy.AddPage(pdfSmartCopy.GetImportedPage(reader, ++page));
            }

            pdfSmartCopy.FreeReader(reader);
        }
    }
    
    private IEnumerable<object> GetPdfObjects(PdfReader reader)
    {
        for (var idx = 0; idx < reader.XrefSize; ++idx)
            yield return reader.GetPdfObjectRelease(idx);
    }
}