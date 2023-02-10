using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter10Tests
{
    [TestMethod]
    public void Verify_Barcodes_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document(new Rectangle(340, 842)))
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var cb = writer.DirectContent;

                // EAN 13
                document.Add(new Paragraph("Barcode EAN.UCC-13"));
                var codeEan = new BarcodeEan { Code = "4512345678906" };
                document.Add(new Paragraph("default:"));
                document.Add(codeEan.CreateImageWithBarcode(cb, null, null));
                codeEan.GuardBars = false;
                document.Add(new Paragraph("without guard bars:"));
                document.Add(codeEan.CreateImageWithBarcode(cb, null, null));
                codeEan.Baseline = -1f;
                codeEan.GuardBars = true;
                document.Add(new Paragraph("text above:"));
                document.Add(codeEan.CreateImageWithBarcode(cb, null, null));
                codeEan.Baseline = codeEan.Size;

                // UPC A
                document.Add(new Paragraph("Barcode UCC-12 (UPC-A)"));
                codeEan.CodeType = Barcode.UPCA;
                codeEan.Code = "785342304749";
                document.Add(codeEan.CreateImageWithBarcode(cb, null, null));

                // EAN 8
                document.Add(new Paragraph("Barcode EAN.UCC-8"));
                codeEan.CodeType = Barcode.EAN8;
                codeEan.BarHeight = codeEan.Size * 1.5f;
                codeEan.Code = "34569870";
                document.Add(codeEan.CreateImageWithBarcode(cb, null, null));

                // UPC E
                document.Add(new Paragraph("Barcode UPC-E"));
                codeEan.CodeType = Barcode.UPCE;
                codeEan.Code = "03456781";
                document.Add(codeEan.CreateImageWithBarcode(cb, null, null));
                codeEan.BarHeight = codeEan.Size * 3f;

                // EANSUPP
                document.Add(new Paragraph("Bookland"));
                document.Add(new Paragraph("ISBN 0-321-30474-8"));
                codeEan.CodeType = Barcode.EAN13;
                codeEan.Code = "9781935182610";
                var codeSupp = new BarcodeEan
                               {
                                   CodeType = Barcode.SUPP5,
                                   Code = "55999",
                                   Baseline = -2,
                               };
                var eanSupp = new BarcodeEansupp(codeEan, codeSupp);
                document.Add(eanSupp.CreateImageWithBarcode(cb, null, BaseColor.Blue));

                // CODE 128
                document.Add(new Paragraph("Barcode 128"));
                var code128 = new Barcode128 { Code = "0123456789 hello" };
                document.Add(code128.CreateImageWithBarcode(cb, null, null));
                code128.Code = "0123456789\uffffMy Raw Barcode (0 - 9)";
                code128.CodeType = Barcode.CODE128_RAW;
                document.Add(code128.CreateImageWithBarcode(cb, null, null));

                // Data for the barcode :
                var code402 = "24132399420058289";
                var code90 = "3700000050";
                var code421 = "422356";
                var data = new StringBuilder(code402);
                data.Append(Barcode128.FNC1);
                data.Append(code90);
                data.Append(Barcode128.FNC1);
                data.Append(code421);
                var shipBarCode = new Barcode128
                                  {
                                      X = 0.75f,
                                      N = 1.5f,
                                      Size = 10f,
                                      TextAlignment = Element.ALIGN_CENTER,
                                      Baseline = 10f,
                                      BarHeight = 50f,
                                      Code = data.ToString(),
                                  };
                document.Add(shipBarCode.CreateImageWithBarcode(
                                                                cb, BaseColor.Black, BaseColor.Blue
                                                               ));

                // it is composed of 3 blocks whith AI 01, 3101 and 10
                var uccEan128 = new Barcode128
                                {
                                    CodeType = Barcode.CODE128_UCC,
                                    Code = "(01)00000090311314(10)ABC123(15)060916",
                                };
                document.Add(uccEan128.CreateImageWithBarcode(
                                                              cb, BaseColor.Blue, BaseColor.Black
                                                             ));
                uccEan128.Code = "0191234567890121310100035510ABC123";
                document.Add(uccEan128.CreateImageWithBarcode(
                                                              cb, BaseColor.Blue, BaseColor.Red
                                                             ));
                uccEan128.Code = "(01)28880123456788";
                document.Add(uccEan128.CreateImageWithBarcode(
                                                              cb, BaseColor.Blue, BaseColor.Black
                                                             ));

                // INTER25
                document.Add(new Paragraph("Barcode Interleaved 2 of 5"));
                var code25 = new BarcodeInter25
                             {
                                 GenerateChecksum = true,
                                 Code = "41-1200076041-001",
                             };
                document.Add(code25.CreateImageWithBarcode(cb, null, null));
                code25.Code = "411200076041001";
                document.Add(code25.CreateImageWithBarcode(cb, null, null));
                code25.Code = "0611012345678";
                code25.ChecksumText = true;
                document.Add(code25.CreateImageWithBarcode(cb, null, null));

                // POSTNET
                document.Add(new Paragraph("Barcode Postnet"));
                var codePost = new BarcodePostnet();
                document.Add(new Paragraph("ZIP"));
                codePost.Code = "01234";
                document.Add(codePost.CreateImageWithBarcode(cb, null, null));
                document.Add(new Paragraph("ZIP+4"));
                codePost.Code = "012345678";
                document.Add(codePost.CreateImageWithBarcode(cb, null, null));
                document.Add(new Paragraph("ZIP+4 and dp"));
                codePost.Code = "01234567890";
                document.Add(codePost.CreateImageWithBarcode(cb, null, null));

                document.Add(new Paragraph("Barcode Planet"));
                var codePlanet = new BarcodePostnet
                                 {
                                     Code = "01234567890",
                                     CodeType = Barcode.PLANET,
                                 };
                document.Add(codePlanet.CreateImageWithBarcode(cb, null, null));

                // CODE 39
                document.Add(new Paragraph("Barcode 3 of 9"));
                var code39 = new Barcode39 { Code = "ITEXT IN ACTION" };
                document.Add(code39.CreateImageWithBarcode(cb, null, null));

                document.Add(new Paragraph("Barcode 3 of 9 extended"));
                var code39Ext = new Barcode39
                                {
                                    Code = "iText in Action",
                                    StartStopText = false,
                                    Extended = true,
                                };
                document.Add(code39Ext.CreateImageWithBarcode(cb, null, null));

                // CODABAR
                document.Add(new Paragraph("Codabar"));
                var codabar = new BarcodeCodabar
                              {
                                  Code = "A123A",
                                  StartStopText = true,
                              };
                document.Add(codabar.CreateImageWithBarcode(cb, null, null));

                // PDF417
                document.Add(new Paragraph("Barcode PDF417"));
                var pdf417 = new BarcodePdf417();
                var text = "Call me Ishmael. Some years ago--never mind how long "
                           + "precisely --having little or no money in my purse, and nothing "
                           + "particular to interest me on shore, I thought I would sail about "
                           + "a little and see the watery part of the world.";
                pdf417.SetText(text);
                var img = pdf417.GetImage();
                img.ScalePercent(50, 50 * pdf417.YHeight);
                document.Add(img);

                document.Add(new Paragraph("Barcode Datamatrix"));
                var datamatrix = new BarcodeDatamatrix();
                datamatrix.Generate(text);
                img = datamatrix.CreateImage();
                document.Add(img);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_ClippingPath_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var img = Image.GetInstance(TestUtils.GetImagePath("hitchcock.png"));
                var w = img.ScaledWidth;
                var h = img.ScaledHeight;
                var t = writer.DirectContent.CreateTemplate(850, 600);
                t.Ellipse(0, 0, 850, 600);
                t.Clip();
                t.NewPath();
                t.AddImage(img, w, 0, 0, h, 0, -600);
                var clipped = Image.GetInstance(t);
                clipped.ScalePercent(50);
                document.Add(clipped);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_ShadingPatternColor_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var canvas = writer.DirectContent;
                var axial = PdfShading.SimpleAxial(writer, 36, 716, 396,
                                                   788, BaseColor.Orange, BaseColor.Blue
                                                  );
                canvas.PaintShading(axial);
                document.NewPage();
                var radial = PdfShading.SimpleRadial(writer,
                                                     200, 700, 50, 300, 700, 100,
                                                     new BaseColor(0xFF, 0xF7, 0x94),
                                                     new BaseColor(0xF7, 0x8A, 0x6B),
                                                     false, false
                                                    );
                canvas.PaintShading(radial);

                var shading = new PdfShadingPattern(axial);
                ColorRectangle(canvas, new ShadingColor(shading), 150, 420, 126, 126);
                canvas.SetShadingFill(shading);
                canvas.Rectangle(300, 420, 126, 126);
                canvas.FillStroke();
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static void ColorRectangle(PdfContentByte canvas, BaseColor color,
                                       float x, float y, float width, float height)
    {
        canvas.SaveState();
        canvas.SetColorFill(color);
        canvas.Rectangle(x, y, width, height);
        canvas.FillStroke();
        canvas.RestoreState();
    }
}