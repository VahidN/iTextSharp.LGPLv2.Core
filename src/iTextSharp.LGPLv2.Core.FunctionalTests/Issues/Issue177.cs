using System.IO;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/177
/// </summary>
[TestClass]
public class Issue177
{
    [TestMethod]
    public void Test_Issue177_Verify_PdfReader_Works()
    {
        var inputFile = TestUtils.GetPdfsPath(fileName: "issue177.pdf");

        using var reader = new PdfReader(inputFile);
        Assert.AreEqual(expected: 1, reader.NumberOfPages);
    }

    [TestMethod]
    public void Test_Issue177_Verify_PdfStamper_Works()
    {
        var inputFile = TestUtils.GetPdfsPath(fileName: "issue177.pdf");
        using var outStream = new FileStream(TestUtils.GetOutputFileName(), FileMode.Create);
        // PdfReader.AllowOpenWithFullPermissions = true;
        using var pdfReader = new PdfReader(inputFile);
        using var pdfStamper = new PdfStamper(pdfReader, outStream);

        var box = pdfReader.GetPageSize(index: 1);
        var canvas = pdfStamper.GetOverContent(pageNum: 1);
        canvas.SetRgbColorStroke(red: 255, green: 0, blue: 0);
        canvas.MoveTo(box.Left, box.Bottom);
        canvas.LineTo(box.Right, box.Top);
        canvas.MoveTo(box.Right, box.Bottom);
        canvas.LineTo(box.Left, box.Top);
        canvas.Stroke();
    }
}