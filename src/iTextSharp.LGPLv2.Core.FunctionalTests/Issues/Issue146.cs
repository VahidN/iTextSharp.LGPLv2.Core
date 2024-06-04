using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/146
/// </summary>
[TestClass]
public class Issue146
{
    [TestMethod]
    public void Test_Issue146_Verify_PdfReader_Works()
    {
        var inputFile = TestUtils.GetPdfsPath("issue146.pdf");

        using (var reader = new PdfReader(inputFile))
        {
            Assert.AreEqual(1, reader.NumberOfPages);
        }

        using (var pdfReader = new PdfReader(new RandomAccessFileOrArray(inputFile), null))
        {
            Assert.AreEqual(1, pdfReader.NumberOfPages);
        }
    }
}