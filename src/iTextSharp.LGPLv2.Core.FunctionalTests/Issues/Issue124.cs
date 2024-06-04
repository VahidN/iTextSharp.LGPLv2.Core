using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/124
/// </summary>
[TestClass]
public class Issue124
{
    [TestMethod]
    public void Test_Issue124_Verify_PdfCopy_Works()
    {
        var inputFile = TestUtils.GetPdfsPath("issue124.pdf");
        var outFile = TestUtils.GetOutputFileName();

        using (var fileStream = new FileStream(outFile, FileMode.Create))
        {
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

        TestUtils.VerifyPdfFileIsReadable(outFile);
    }
}