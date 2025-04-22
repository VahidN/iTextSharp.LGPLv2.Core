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
        var outputFile = TestUtils.GetOutputFileName();

        var userPassword = "user";
        var ownerPassword = "owner";

        ReadEncryptedFileModifyItEncryptItAgain(outputFile, inputFile, userPassword, ownerPassword);
        ReadOutputEncryptedFile(outputFile, ownerPassword);
    }

    private static void ReadOutputEncryptedFile(string outputFile, string ownerPassword)
    {
        using var reader = new PdfReader(outputFile, ownerPassword);
        Assert.AreEqual(expected: 1, reader.NumberOfPages);
    }

    private static void ReadEncryptedFileModifyItEncryptItAgain(string outputFile,
        string inputFile,
        string userPassword,
        string ownerPassword)
    {
        using var outStream = new FileStream(outputFile, FileMode.Create);

        PdfReader.AllowOpenWithFullPermissions = true;
        using var pdfReader = new PdfReader(inputFile);
        using var pdfStamper = new PdfStamper(pdfReader, outStream);

        pdfStamper.SetEncryption(PdfWriter.ENCRYPTION_AES_256_V3, userPassword, ownerPassword,
            PdfWriter.ALLOW_PRINTING | PdfWriter.ALLOW_COPY);

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