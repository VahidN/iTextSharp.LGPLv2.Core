using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec;
using iTextSharp.text.pdf.qrcode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/26
/// </summary>
[TestClass]
public class Issue26
{
    [TestMethod]
    public void Verify_Issue26_CanBe_Processed()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            using (var document = new Document())
            {
                PdfWriter.GetInstance(document, stream);
                document.AddAuthor(TestUtils.Author);
                document.Open();
                var qrcodeImage = CreateQrCodeImage("This is a text ...");

                //qrcodeImage.SetAbsolutePosition(10, 500);
                qrcodeImage.ScalePercent(200);
                document.Add(qrcodeImage);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static Image CreateQrCodeImage(string content)
    {
        var qrCodeWriter = new QRCodeWriter();
        var byteMatrix = qrCodeWriter.Encode(content, 1, 1, null);
        var width = byteMatrix.GetWidth();
        var height = byteMatrix.GetHeight();
        var stride = (width + 7) / 8;
        var bitMatrix = new byte[stride * height];
        var byteMatrixArray = byteMatrix.GetArray();
        for (var y = 0; y < height; ++y)
        {
            var line = byteMatrixArray[y];
            for (var x = 0; x < width; ++x)
            {
                if (line[x] != 0)
                {
                    var offset = stride * y + x / 8;
                    bitMatrix[offset] |= (byte)(0x80 >> (x % 8));
                }
            }
        }

        var encodedImage = Ccittg4Encoder.Compress(bitMatrix, byteMatrix.GetWidth(), byteMatrix.GetHeight());
        var qrcodeImage = Image.GetInstance(byteMatrix.GetWidth(), byteMatrix.GetHeight(), false, Element.CCITTG4,
                                            Element.CCITT_BLACKIS1, encodedImage, null);
        return qrcodeImage;
    }
}