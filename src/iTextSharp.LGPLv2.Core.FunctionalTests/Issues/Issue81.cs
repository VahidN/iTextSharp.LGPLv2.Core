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
    [ExpectedException(typeof(InvalidPdfException))]
    public void Test_Issue81_ReadPageCount()
    {
        var resource = TestUtils.GetPdfsPath("issue81.pdf");
        using var pdfReader = new PdfReader(resource);
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }
}