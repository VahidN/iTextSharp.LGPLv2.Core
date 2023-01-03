using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/39
/// </summary>
[TestClass]
public class Issue39
{
    [TestMethod]
    public void Verify_Issue39_CanBe_Processed()
    {
        var inPdfFile = TestUtils.GetPdfsPath("issue39.pdf");
        var outPdfFile = TestUtils.GetOutputFileName();
        addWatermark(inPdfFile, outPdfFile);
    }

    [TestMethod]
    public void Verify_Issue39_2_CanBe_Processed()
    {
        var inPdfFile = TestUtils.GetPdfsPath("issue39_2.pdf");
        var outPdfFile = TestUtils.GetOutputFileName();
        addWatermark(inPdfFile, outPdfFile);
    }

    private static void addWatermark(string inPdfFile, string outPdfFile)
    {
        var pdfReader = new PdfReader(inPdfFile);
        var outStream = new FileStream(outPdfFile, FileMode.Create);
        var pdfStamper = new PdfStamper(pdfReader, outStream);
        var total = pdfReader.NumberOfPages + 1;
        var pageSize = pdfReader.GetPageSize(1);
        var width = pageSize.Width;
        var height = pageSize.Height;

        //var font = TestUtils.GetUnicodeFont("Tahoma", TestUtils.GetTahomaFontPath(), 10, Font.BOLD, BaseColor.Black);
        //var font = TestUtils.GetUnicodeFont("FangSong", TestUtils.GetFontPath("simfang.ttf"), 10, Font.BOLD, BaseColor.Black);
        var font = BaseFont.CreateFont(TestUtils.GetFontPath("simfang.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        var gs = new PdfGState();
        for (var i = 1; i < total; i++)
        {
            var content = pdfStamper.GetOverContent(i);
            gs.FillOpacity = 0.3f;
            content.SetGState(gs);
            content.BeginText();
            content.SetColorFill(BaseColor.LightGray);
            content.SetFontAndSize(font, 100);
            content.SetTextMatrix(0, 0);
            content.ShowTextAligned(
                                    Element.ALIGN_CENTER,
                                    "WaterMark Name ...",
                                    width / 2 - 50,
                                    height / 2 - 50,
                                    55);
            content.EndText();
        }

        pdfStamper.Close();
        pdfReader.Close();
        outStream.Dispose();
    }
}