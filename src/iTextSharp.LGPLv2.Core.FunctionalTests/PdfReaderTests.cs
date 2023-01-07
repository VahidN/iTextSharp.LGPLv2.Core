using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class PdfReaderTests
{
    [TestMethod]
    public void Detect_Blank_Pages_In_Pdf()
    {
        // value where we can consider that this is a blank image
        // can be much higher or lower depending of what is considered as a blank page
        const int blankThreshold = 20;

        var pdfFile = CreateSamplePdfFile();
        using var reader = new PdfReader(pdfFile);

        var blankPages = 0;
        for (var pageNum = 1; pageNum <= reader.NumberOfPages; pageNum++)
        {
            // first check, examine the resource dictionary for /Font or /XObject keys.
            // If either are present -> not blank.
            var pageDict = reader.GetPageN(pageNum);
            var resDict = (PdfDictionary)pageDict.Get(PdfName.Resources);

            var hasFont = resDict.Get(PdfName.Font) != null;
            if (hasFont)
            {
                Console.WriteLine($"Page {pageNum} has font(s).");
                continue;
            }

            var hasImage = resDict.Get(PdfName.Xobject) != null;
            if (hasImage)
            {
                Console.WriteLine($"Page {pageNum} has image(s).");
                continue;
            }

            var content = reader.GetPageContent(pageNum);
            if (content.Length <= blankThreshold)
            {
                Console.WriteLine($"Page {pageNum} is blank");
                blankPages++;
            }
        }

        Assert.AreEqual(1, blankPages, $"{reader.NumberOfPages} page(s) with {blankPages} blank page(s).");
    }


    [TestMethod]
    public void Test_Extract_Text()
    {
        var pdfFile = CreateSamplePdfFile();
        using var reader = new PdfReader(pdfFile);
        var streamBytes = reader.GetPageContent(1);
        var tokenizer = new PrTokeniser(new RandomAccessFileOrArray(streamBytes));

        var stringsList = new List<string>();
        while (tokenizer.NextToken())
        {
            if (tokenizer.TokenType == PrTokeniser.TK_STRING)
            {
                stringsList.Add(tokenizer.StringValue);
            }
        }

        Assert.IsTrue(stringsList.Contains("Hello DNT!"));
    }

    private static byte[] CreateSamplePdfFile()
    {
        using var stream = new MemoryStream();
        using (var document = new Document())
        {
            // step 2
            var writer = PdfWriter.GetInstance(document, stream);
            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            document.Add(new Paragraph("Hello DNT!"));

            document.NewPage();
            // we don't add anything to this page: newPage() will be ignored
            document.Add(new Phrase(""));

            document.NewPage();
            writer.PageEmpty = false;
        }

        return stream.ToArray();
    }

    [TestMethod]
    public void Test_Draw_Text()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                var pdfWriter = PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                pdfDoc.Add(new Paragraph("Test"));

                var cb = pdfWriter.DirectContent;
                var bf = BaseFont.CreateFont();
                cb.BeginText();
                cb.SetFontAndSize(bf, 12);
                cb.MoveText(88.66f, 367);
                cb.ShowText("ld");
                cb.MoveText(-22f, 0);
                cb.ShowText("Wor");
                cb.MoveText(-15.33f, 0);
                cb.ShowText("llo");
                cb.MoveText(-15.33f, 0);
                cb.ShowText("He");
                cb.EndText();

                var tmp = cb.CreateTemplate(250, 25);
                tmp.BeginText();
                tmp.SetFontAndSize(bf, 12);
                tmp.MoveText(0, 7);
                tmp.ShowText("Hello People");
                tmp.EndText();
                cb.AddTemplate(tmp, 36, 343);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}