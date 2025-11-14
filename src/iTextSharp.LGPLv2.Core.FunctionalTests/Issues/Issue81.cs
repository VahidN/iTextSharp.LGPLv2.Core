using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/81
/// </summary>
[TestClass]
public class Issue81
{
    [TestMethod]
    public void Test_Issue81_ReadPageCount()
        => Assert.Throws<InvalidPdfException>(() =>
        {
            var resource = TestUtils.GetPdfsPath(fileName: "issue81.pdf");
            using var pdfReader = new PdfReader(resource);

            Assert.AreEqual(expected: 1, pdfReader.NumberOfPages,
                message: "PdfReader fails to report the correct number of pages");
        });
}