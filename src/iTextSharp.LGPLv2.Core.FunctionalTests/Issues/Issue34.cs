using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/34
/// </summary>
[TestClass]
public class Issue34
{
    [TestMethod]
    public void Verify_Issue34_Create_Font_CanBe_Processed()
    {
        var pdfDoc = new Document(PageSize.A4);

        var pdfFilePath = TestUtils.GetOutputFileName();
        var fileStream = new FileStream(pdfFilePath, FileMode.Create);
        PdfWriter.GetInstance(pdfDoc, fileStream);

        pdfDoc.AddAuthor(TestUtils.Author);
        pdfDoc.Open();

        // STSong-Light requires this language pack http://ardownload.adobe.com/pub/adobe/reader/win/AcrobatDC/misc/FontPack1900820071_XtdAlf_Lang_DC.msi
        var baseFont = BaseFont.CreateFont("STSong-Light", "UniGB-UCS2-H", BaseFont.NOT_EMBEDDED);
        var font = new Font(baseFont, 12);

        pdfDoc.Add(new Paragraph(font.BaseFont.PostscriptFontName, font));
        pdfDoc.Add(new Paragraph("你好", font));

        pdfDoc.Close();
        fileStream.Dispose();

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Issue34_Fill_Form1_CanBe_Processed()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        var stream = new FileStream(pdfFilePath, FileMode.Create);

        var path = TestUtils.GetPdfsPath("issue16.pdf");
        var reader = new PdfReader(path);
        var stamper = new PdfStamper(reader, stream);

        var form = stamper.AcroFields;

        var baseFont = BaseFont.CreateFont("STSong-Light", "UniGB-UCS2-H", BaseFont.NOT_EMBEDDED);
        form.AddSubstitutionFont(baseFont);

        foreach (var field in form.Fields)
        {
            Console.WriteLine(field.Key);
        }

        form.SetField("Text Field0", "你好");

        stamper.Close();
        reader.Close();
        stream.Dispose();
    }

    [TestMethod]
    public void Verify_Issue34_Fill_Form2_CanBe_Processed()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        var stream = new FileStream(pdfFilePath, FileMode.Create);

        var path = TestUtils.GetPdfsPath("SamplePDFForm.pdf");
        var reader = new PdfReader(path);
        var stamper = new PdfStamper(reader, stream);

        var form = stamper.AcroFields;

        var baseFont = BaseFont.CreateFont("STSong-Light", "UniGB-UCS2-H", BaseFont.NOT_EMBEDDED);
        form.AddSubstitutionFont(baseFont);

        foreach (var field in form.Fields)
        {
            Console.WriteLine(field.Key);
        }

        form.SetField("First Name", "你好");
        form.SetField("Last Name", "你好");
        form.SetField("Awesome Checkbox", true ? "Yes" : "Off");

        // set this if you want the result PDF to not be editable.
        stamper.FormFlattening = true;

        stamper.Close();
        reader.Close();
        stream.Dispose();
    }
}