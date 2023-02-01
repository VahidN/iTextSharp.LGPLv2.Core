using System.IO;
using System.util;
using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BadPasswordException = iTextSharp.text.pdf.BadPasswordException;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/100
///     ported from https://github.com/LibrePDF/OpenPDF/pull/802
/// </summary>
[TestClass]
public class Issue100
{
    [TestMethod]
    public void Test_Issue100_ReadPwProtectedAES256_openPDFiss375()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "pwProtectedAES256_openPDFiss375.pdf");
        using var pdfReader = new PdfReader(resource);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void testReadDemo1Encrypted()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "Demo1_encrypted_.pdf");
        using var pdfReader = new PdfReader(resource);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadCopiedPositiveP()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "copied-positive-P.pdf");
        using var pdfReader = new PdfReader(resource);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadCR6InPwOwner4()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "c-r6-in-pw=owner4.pdf");
        using var pdfReader = new PdfReader(resource, "owner4".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(30, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncryptedHelloWorldR6PwHôtel()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "encrypted_hello_world_r6-pw=hôtel.pdf");
        //var ownerPassword = "hôtel".GetIsoBytes(); --> this method doesn't work for utf-8 strings!
        var ownerPassword = "hôtel"u8.ToArray();
        using var pdfReader = new PdfReader(resource, ownerPassword);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncryptedPositiveP()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "encrypted-positive-P.pdf");
        using var pdfReader = new PdfReader(resource);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncXiLongPassword()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "enc-XI-long-password=q....pdf");
        using var pdfReader =
            new PdfReader(resource,
                          "qwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcvbnmqwertyuiopasdfghjklzxcv"
                              .GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(30, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncXiR6V5OMaster_User()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "enc-XI-R6,V5,O=master.pdf");
        using var pdfReader = new PdfReader(resource);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(30, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncXiR6V5OMaster_Owner()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "enc-XI-R6,V5,O=master.pdf");
        using var pdfReader = new PdfReader(resource, "master".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(30, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidPdfException))]
    public void Test_Issue100_ReadEncXiR6V5UAttachmentEncryptedAttachments()
    {
        // in this test file only the embedded file is encrypted.
        var resource = TestUtils.GetIssuePdfsPath("issue100", "enc-XI-R6,V5,U=attachment,encrypted-attachments.pdf");
        using var pdfReader = new PdfReader(resource, "attachment".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncXiR6V5UViewAttachmentsCleartextMetadata()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "enc-XI-R6,V5,U=view,attachments,cleartext-metadata.pdf");
        using var pdfReader = new PdfReader(resource, "view".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(30, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncXiR6V5UViewOMaster_User()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "enc-XI-R6,V5,U=view,O=master.pdf");
        using var pdfReader = new PdfReader(resource, "view".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(30, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncXiR6V5UViewOMaster_Owner()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "enc-XI-R6,V5,U=view,O=master.pdf");
        using var pdfReader = new PdfReader(resource, "master".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(30, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadEncXiR6V5UWwwwwOWwwww()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "enc-XI-R6,V5,U=wwwww,O=wwwww.pdf");
        using var pdfReader = new PdfReader(resource, "wwwww".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(30, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadGraphEncryptedPwUser()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "graph-encrypted-pw=user.pdf");
        using var pdfReader = new PdfReader(resource, "user".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadIssue60101PwOwner()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "issue6010_1-pw=owner.pdf");
        using var pdfReader = new PdfReader(resource, "owner".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadIssue60102Pwæøå()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "issue6010_2-pw=æøå.pdf");
        //var ownerPassword = "æøå".GetIsoBytes(); --> this method doesn't work for utf-8 strings!
        var ownerPassword = "æøå"u8.ToArray();
        using var pdfReader = new PdfReader(resource, ownerPassword);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(10, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadMuPDFAes256R6UUserOOwner_User()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "MuPDF-AES256-R6-u=user-o=owner.pdf");
        using var pdfReader = new PdfReader(resource, "user".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadMuPDFAes256R6UUserOOwner_Owner()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "MuPDF-AES256-R6-u=user-o=owner.pdf");
        using var pdfReader = new PdfReader(resource, "owner".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    [ExpectedException(typeof(BadPasswordException))]
    public void Test_Issue100_ReadNonTrivialCryptFilter()
    {
        // in this test file only the embedded file is encrypted.
        var resource = TestUtils.GetIssuePdfsPath("issue100", "nontrivial-crypt-filter.pdf");
        using var pdfReader = new PdfReader(resource);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadPr65311PwAsdfasdf()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "pr6531_1-pw=asdfasdf.pdf");
        using var pdfReader = new PdfReader(resource, "asdfasdf".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadPr65312PwAsdfasdf()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "pr6531_2-pw=asdfasdf.pdf");
        using var pdfReader = new PdfReader(resource, "asdfasdf".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    [ExpectedException(typeof(BadPasswordException))]
    public void Test_Issue100_ReadUnfilterableWithCrypt()
    {
        // this test file only certain streams with Crypt filters are encrypted.
        var resource = TestUtils.GetIssuePdfsPath("issue100", "unfilterable-with-crypt.pdf");
        using var pdfReader = new PdfReader(resource);
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(1, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_ReadTHISISATEST_PWP()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "THISISATEST_PWP.pdf");
        using var pdfReader = new PdfReader(resource, "password".GetIsoBytes());
        Assert.IsTrue(pdfReader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.AreEqual(2, pdfReader.NumberOfPages,
                        "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_CreateSimplePdf()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var document = new Document(PageSize.A4))
            {
                var permissions = 0;
                permissions |= PdfWriter.AllowPrinting;
                permissions |= PdfWriter.AllowCopy;
                permissions |= PdfWriter.AllowScreenReaders;

                var writer = PdfWriter.GetInstance(document, fileStream);
                writer.SetEncryption("user".GetIsoBytes(), "owner".GetIsoBytes(), permissions,
                                     PdfWriter.ENCRYPTION_AES_256_V3);
                document.AddAuthor(TestUtils.Author);
                // step 3
                document.Open();
                // step 4
                document.Add(new Paragraph("Hello World"));
            }
        }

        using var reader = new PdfReader(pdfFilePath, "owner".GetIsoBytes());
        var author = reader.Info["Author"];
        Assert.AreEqual(TestUtils.Author, author);
        Assert.IsTrue(reader.IsEncrypted(), "PdfReader fails to report test file to be encrypted.");
        Assert.IsTrue(reader.IsOpenedWithFullPermissions, "PdfReader fails to recognize password as owner password.");
        Assert.AreEqual(1, reader.NumberOfPages, "PdfReader fails to report the correct number of pages");
    }

    [TestMethod]
    public void Test_Issue100_StampPwProtectedAES256_openPDFiss375()
    {
        var resource = TestUtils.GetIssuePdfsPath("issue100", "pwProtectedAES256_openPDFiss375.pdf");
        using var outStream = new FileStream(TestUtils.GetOutputFileName(), FileMode.Create);
        PdfReader.AllowOpenWithFullPermissions = true;
        using var pdfReader = new PdfReader(resource);
        using var pdfStamper = new PdfStamper(pdfReader, outStream);

        var box = pdfReader.GetPageSize(1);
        var canvas = pdfStamper.GetOverContent(1);
        canvas.SetRgbColorStroke(255, 0, 0);
        canvas.MoveTo(box.Left, box.Bottom);
        canvas.LineTo(box.Right, box.Top);
        canvas.MoveTo(box.Right, box.Bottom);
        canvas.LineTo(box.Left, box.Top);
        canvas.Stroke();
    }
}