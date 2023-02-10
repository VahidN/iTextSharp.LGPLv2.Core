using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter11Tests
{
    [TestMethod]
    public void Verify_EncodingExample_CanBeCreated()
    {
        string[][] movies =
        {
            new[]
            {
                "Cp1252",
                "A Very long Engagement (France)",
                "directed by Jean-Pierre Jeunet",
                "Un long dimanche de fian\u00e7ailles",
            },
            new[]
            {
                "Cp1250",
                "No Man's Land (Bosnia-Herzegovina)",
                "Directed by Danis Tanovic",
                "Nikogar\u0161nja zemlja",
            },
            new[]
            {
                "Cp1251",
                "You I Love (Russia)",
                "directed by Olga Stolpovskaja and Dmitry Troitsky",
                "\u042f \u043b\u044e\u0431\u043b\u044e \u0442\u0435\u0431\u044f",
            },
            new[]
            {
                "Cp1253",
                "Brides (Greece)",
                "directed by Pantelis Voulgaris",
                "\u039d\u03cd\u03c6\u03b5\u03c2",
            },
        };


        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();

                // step 4
                var font = TestUtils.GetTahomaFontPath();
                for (var i = 0; i < 4; i++)
                {
                    var bf = BaseFont.CreateFont(font, movies[i][0], BaseFont.EMBEDDED);
                    document.Add(new Paragraph(
                                               $"Font: {bf.PostscriptFontName} with encoding: {bf.Encoding}"
                                              ));
                    document.Add(new Paragraph(movies[i][1]));
                    document.Add(new Paragraph(movies[i][2]));
                    document.Add(new Paragraph(movies[i][3], new Font(bf, 12)));
                    document.Add(Chunk.Newline);
                }
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Unicode_PDF_File_CanBeCreated_Using_Chunks()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                var pdfWriter = PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                var tahomaFont =
                    TestUtils.GetUnicodeFont("Tahoma", TestUtils.GetTahomaFontPath(), 10, Font.NORMAL, BaseColor.Black);

                var ct = new ColumnText(pdfWriter.DirectContent)
                         {
                             RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                         };
                ct.SetSimpleColumn(100, 100, 500, 800, 24, Element.ALIGN_RIGHT);

                var chunk = new Chunk("آزمایش", tahomaFont);

                ct.AddElement(chunk);
                ct.Go();
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Unicode_PDF_File_CanBeCreated_Using_Phrases()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                var fontFilePath = TestUtils.GetFontPath("IRANSans(FaNum)_Medium.ttf");
                var sansFont = TestUtils.GetUnicodeFont(fontFilePath, fontFilePath, 10, Font.NORMAL, BaseColor.Black);
                var table = new PdfPTable(1)
                            {
                                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                                ExtendLastRow = true,
                            };

                var pdfCell = new PdfPCell(new Phrase("آزمایش", sansFont))
                              {
                                  RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                              };

                table.AddCell(pdfCell);
                pdfDoc.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Unicode_PDF_File_CanBeCreated_Using_PdfTable()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                var tahomaFont =
                    TestUtils.GetUnicodeFont("Tahoma", TestUtils.GetTahomaFontPath(), 10, Font.NORMAL, BaseColor.Black);

                var table = new PdfPTable(1)
                            {
                                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                                ExtendLastRow = true,
                            };

                var pdfCell = new PdfPCell(new Phrase("آزمایش", tahomaFont))
                              {
                                  RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                              };

                table.AddCell(pdfCell);
                pdfDoc.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Chinese_PDF_File_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                var text = "These are the protagonists in 'Hero', a movie by Zhang Yimou:\n"
                           + "\u7121\u540d (Nameless), \u6b98\u528d (Broken Sword), "
                           + "\u98db\u96ea (Flying Snow), \u5982\u6708 (Moon), "
                           + "\u79e6\u738b (the King), and \u9577\u7a7a (Sky).";
                var chinese = "Chinese: \u5341\u950a\u57cb\u4f0f";

                var tahomaFont =
                    TestUtils.GetUnicodeFont("SimSun", TestUtils.GetSimSunFontPath(), 10, Font.NORMAL, BaseColor.Black);
                pdfDoc.Add(new Paragraph(text, tahomaFont));
                pdfDoc.Add(new Paragraph(chinese, tahomaFont));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Thai_PDF_File_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var fileStream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var pdfDoc = new Document(PageSize.A4))
            {
                PdfWriter.GetInstance(pdfDoc, fileStream);

                pdfDoc.AddAuthor(TestUtils.Author);
                pdfDoc.Open();

                var chinese = "ทดสอบ ภาษาไทย ฝีมือ พึ่ง พ่อ กู รู้ ดู กันต์ ก๋ยวเตี๋ยว";

                var tahomaFont =
                    TestUtils.GetUnicodeFont("THSarabunNew", TestUtils.GetThaiFontPath(), 20, Font.NORMAL,
                                             BaseColor.Black);
                pdfDoc.Add(new Paragraph(chinese, tahomaFont));
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}