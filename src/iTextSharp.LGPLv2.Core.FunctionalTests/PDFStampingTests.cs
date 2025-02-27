using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

public class StampRequestForm
{
    public IList<string> Lines { get; set; }

    public float LowerLeftX { get; set; }

    public float LowerLeftY { get; set; }

    public float UpperRightX { get; set; }

    public float UpperRightY { get; set; }

    public int RotationDegree { get; set; }
}

[TestClass]
public class PDFStampingTests
{
    [TestMethod]
    public void Verify_Metadata_Can_Be_Added_To_Stamped_File()
    {
        var outputFilePath = TestUtils.GetOutputFileName();

        using (var reader = new PdfReader(TestUtils.GetPdfsPath(fileName: "sample.pdf")))
        {
            using var outStream = new FileStream(outputFilePath, FileMode.Create);
            using var stamper = new PdfStamper(reader, outStream);
            stamper.AddAuthor(author: "Test");
            stamper.AddKeywords(keywords: "C#");
            stamper.AddTitle(title: "New doc");
            stamper.AddSubject(subject: "New doc");
            stamper.AddCreator(creator: "MyApp");

            stamper.ViewerPreferences = PdfWriter.PageModeUseOutlines;
            stamper.SetFullCompression();
        }

        using (var reader = new PdfReader(outputFilePath))
        {
            Assert.AreEqual(expected: "Test", reader.Info[key: "Author"]);
            Assert.AreEqual(expected: "New doc", reader.Info[key: "Title"]);
        }
    }

    [TestMethod]
    public void Verify_Stamped_File_CanBeCreated()
    {
        using var inputPdfStream = new FileStream(TestUtils.GetPdfsPath(fileName: "sample.pdf"), FileMode.Open);
        using var outStream = new FileStream(TestUtils.GetOutputFileName(), FileMode.Create);

        using var reader = new PdfReader(inputPdfStream);
        using var stamper = new PdfStamper(reader, outStream);
        var dc = stamper.GetOverContent(pageNum: 1);

        AddWaterMark(dc, new StampRequestForm
        {
            Lines = new[]
            {
                "This is a sample stamp", DateTime.Now.ToShortDateString(), "https://github.com/VahidN"
            },

            // position the stamp text near the top right. 
            LowerLeftX = 402,
            LowerLeftY = 600,
            UpperRightX = 575,
            UpperRightY = 900
        });

        Assert.IsNotNull(outStream);
    }

    private void AddWaterMark(PdfContentByte dc, StampRequestForm stampRequest)
    {
        dc.SaveState();

        dc.SetGState(new PdfGState
        {
            FillOpacity = 0.61f,
            StrokeOpacity = 0.61f
        });

        dc.SetColorFill(BaseColor.Red);
        dc.BeginText();
        dc.SetFontAndSize(BaseFont.CreateFont(), size: 7);
        var x = (stampRequest.LowerLeftX + stampRequest.UpperRightX) / 2;
        var y = (stampRequest.LowerLeftY + stampRequest.UpperRightY) / 2;

        var verticalSpaceBetweenLines = 15;

        foreach (var line in stampRequest.Lines)
        {
            dc.ShowTextAligned(Element.ALIGN_CENTER, line, x, y, stampRequest.RotationDegree);
            y -= verticalSpaceBetweenLines;
        }

        dc.EndText();
        dc.RestoreState();
    }
}