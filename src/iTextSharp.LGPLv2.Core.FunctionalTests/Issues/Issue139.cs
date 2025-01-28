using System.IO;
using System.util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/139
/// </summary>
[TestClass]
public class Issue139
{
    [TestMethod]
    public void Test_Issue139_Verify_PdfReader_Works()
    {
        var inputFile = TestUtils.GetPdfsPath(fileName: "issue139.pdf");

        using (var reader = new PdfReader(inputFile))
        {
            Assert.AreEqual(expected: 2, reader.NumberOfPages);
        }
    }

    [TestMethod]
    public void Test_Issue139_CreateSimple_AES_256_Pdf()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        var encryptionType = PdfWriter.ENCRYPTION_AES_256;
        CreateEncryptedPdfFile(pdfFilePath, encryptionType);
    }

    [TestMethod]
    public void Test_Issue139_CreateSimple_AES_128_Pdf()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        var encryptionType = PdfWriter.ENCRYPTION_AES_128;
        CreateEncryptedPdfFile(pdfFilePath, encryptionType);
    }

    [TestMethod]
    public void Test_Issue139_CreateSimple_AES_256_V3_Pdf()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        var encryptionType = PdfWriter.ENCRYPTION_AES_256_V3;
        CreateEncryptedPdfFile(pdfFilePath, encryptionType);
    }

    [TestMethod]
    public void Test_Issue139_CreateSimple_STANDARD_ENCRYPTION_128_Pdf()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        var encryptionType = PdfWriter.STANDARD_ENCRYPTION_128;
        CreateEncryptedPdfFile(pdfFilePath, encryptionType);
    }

    [TestMethod]
    public void Test_Issue139_CreateSimple_STANDARD_ENCRYPTION_40_Pdf()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        var encryptionType = PdfWriter.STANDARD_ENCRYPTION_40;
        CreateEncryptedPdfFile(pdfFilePath, encryptionType);
    }

    private static void CreateEncryptedPdfFile(string pdfFilePath, int encryptionType)
    {
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var document = new Document(PageSize.A4))
            {
                var permissions = 0;
                permissions |= PdfWriter.AllowPrinting;
                permissions |= PdfWriter.AllowCopy;
                permissions |= PdfWriter.AllowScreenReaders;

                var writer = PdfWriter.GetInstance(document, fileStream);

                writer.SetEncryption(encryptionType, userPassword: "user", ownerPassword: "owner", permissions);

                document.AddAuthor(TestUtils.Author);

                // step 3
                document.Open();

                // step 4
                document.Add(new Paragraph(str: "Hello World"));
            }
        }

        using var reader = new PdfReader(pdfFilePath, "owner".GetIsoBytes());
        Assert.IsTrue(reader.IsEncrypted(), message: "PdfReader fails to report test file to be encrypted.");

        Assert.IsTrue(reader.IsOpenedWithFullPermissions,
            message: "PdfReader fails to recognize password as owner password.");

        Assert.AreEqual(expected: 1, reader.NumberOfPages,
            message: "PdfReader fails to report the correct number of pages");

        var author = reader.Info[key: "Author"];
        Assert.AreEqual(TestUtils.Author, author);
    }
}