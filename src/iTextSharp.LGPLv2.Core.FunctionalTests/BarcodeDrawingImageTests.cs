using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class BarcodeDrawingImageTests
{
    [TestMethod]
    public void Verify_Barcode128_CreateDrawingImage_Produces_RawBitmap()
    {
        var barcode = new Barcode128
                      {
                          Code = "9781935182610",
                          CodeType = Barcode.CODE128,
                          BarHeight = 30f
                      };

        var bitmap = barcode.CreateDrawingImage(Color.Black, Color.White);

        Assert.IsNotNull(bitmap);
        Assert.IsTrue(bitmap.Width > 0);
        Assert.AreEqual(30, bitmap.Height);

        var pixels = bitmap.GetArgbPixels();
        Assert.AreEqual(bitmap.Width * bitmap.Height, pixels.Length);

        // The rendered barcode must contain both foreground (black) and background (white) pixels.
        var hasBlack = false;
        var hasWhite = false;
        for (var x = 0; x < bitmap.Width; x++)
        {
            var argb = bitmap.GetPixel(x, 0).ToArgb();
            if (argb == Color.Black.ToArgb())
            {
                hasBlack = true;
            }
            else if (argb == Color.White.ToArgb())
            {
                hasWhite = true;
            }
        }

        Assert.IsTrue(hasBlack, "Expected at least one foreground (black) pixel.");
        Assert.IsTrue(hasWhite, "Expected at least one background (white) pixel.");
    }

    [TestMethod]
    public void Verify_Barcode128_DrawingImage_CanBeConverted_To_PdfImage()
    {
        var barcode = new Barcode128
                      {
                          Code = "9781935182610",
                          CodeType = Barcode.CODE128,
                          BarHeight = 30f
                      };

        var bitmap = barcode.CreateDrawingImage(Color.Black, Color.White);
        var image = Image.GetInstance(bitmap);

        Assert.IsNotNull(image);
        Assert.IsTrue(image.Width > 0);
        Assert.IsTrue(image.Height > 0);
    }
}
