using System.IO;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/285
/// </summary>
[TestClass]
public class Issue285
{
    [TestMethod]
    public void Test_Issue285_Verify_PdfReader_Works()
    {
        var inputFile = TestUtils.GetPdfsPath(fileName: "issue285.pdf");

        using var reader = new PdfReader(inputFile);
        Assert.AreEqual(expected: 1, reader.NumberOfPages);
    }
}