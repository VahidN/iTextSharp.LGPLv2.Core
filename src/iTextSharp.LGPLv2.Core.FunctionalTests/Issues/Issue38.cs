using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/38
/// </summary>
[TestClass]
public class Issue38
{
    [TestMethod]
    public void Verify_Issue38_CanBe_Processed()
    {
        var pdfDoc = new Document(PageSize.A4);

        var pdfFilePath = TestUtils.GetOutputFileName();
        var fileStream = new FileStream(pdfFilePath, FileMode.Create);
        PdfWriter.GetInstance(pdfDoc, fileStream);

        pdfDoc.AddAuthor(TestUtils.Author);
        pdfDoc.Open();

        var swiss721BT = TestUtils.GetUnicodeFont("SWIS721 BT", TestUtils.GetFontPath("Swiss721BT.ttf"),
                                                  20, Font.NORMAL, BaseColor.Blue);

        var swiss721ThinBT = TestUtils.GetUnicodeFont("SWIS721 Th BT", TestUtils.GetFontPath("Swiss721ThinBT.ttf"),
                                                      20, Font.NORMAL, BaseColor.DarkGray);

        var text = new Phrase("Default font")
                   {
                       new Chunk("\nSwiss721BT", swiss721BT),
                       new Chunk("\nSwiss721ThinBT", swiss721ThinBT),
                   };

        pdfDoc.Add(text);

        pdfDoc.Close();
        fileStream.Dispose();

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}