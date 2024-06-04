using System;
using System.IO;
using System.Runtime.InteropServices;
using BitMiracle.LibTiff.Classic;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using Document = iTextSharp.text.Document;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class TiffImageTests
{
    private RandomAccessFileOrArray _s;

    [TestInitialize]
    public void TIFF_extra_samples()
    {
        var filename = Path.Combine(TestUtils.GetBaseDir(), "iTextExamples", "resources", "img", "sampleRGBA.tif");
        _s = new RandomAccessFileOrArray(filename);
    }

    [TestMethod]
    public void Verify_TIFF_extra_samples_CanBeRead()
    {
        var numberOfPages = TiffImage.GetNumberOfPages(_s);
        Assert.AreEqual(1, numberOfPages);

        var image = TiffImage.GetTiffImage(_s, 1);
        Assert.IsNotNull(image);
    }

    [TestMethod]
    public void Verify_TIFF_extra_samples_CanBeAddedToPdfDocument()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();

        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            var margin = 50f;

            using (var document = new Document())
            {
                var pageWidth = document.PageSize.Width - margin;
                var pageHeight = document.PageSize.Height - margin;
                var image = TiffImage.GetTiffImage(_s, 1);
                image.ScaleToFit(pageWidth, pageHeight);

                PdfWriter.GetInstance(document, stream);
                document.Open();
                document.AddAuthor(TestUtils.Author);
                document.Add(image);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_TIFF_Image_CanBeAddedToPdfDocument()
    {
        var fileName = Path.Combine(TestUtils.GetBaseDir(), "iTextExamples", "resources", "img", "test.tif");
        var pdfFilePath = TestUtils.GetOutputFileName();

        // It uses BitMiracle.LibTiff.NET package
        // https://github.com/BitMiracle/libtiff.net
        using (var tiff = Tiff.Open(fileName, "r"))
        {
            if (tiff == null)
            {
                throw new InvalidOperationException("Could not open the incoming image");
            }

            var skBitmap = ConvertToSKBitmap(tiff);
            var img = Image.GetInstance(skBitmap, SKEncodedImageFormat.Jpeg);

            img.ScaleAbsolute(img.Width, img.Height);
            img.SetAbsolutePosition(0, 0);
            img.SetDpi(img.DpiX, img.DpiY);

            using (var stream = new FileStream(pdfFilePath, FileMode.Create))
            {
                using (var document = new Document(img, 0, 0, 0, 0))
                {
                    var writer = PdfWriter.GetInstance(document, stream);
                    writer.CompressionLevel = PdfStream.NO_COMPRESSION;

                    document.Open();
                    document.AddAuthor(TestUtils.Author);
                    document.Add(img);
                }
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Multi_Page_TIFF_Image_CanBeAddedToPdfDocument()
    {
        var fileName = Path.Combine(TestUtils.GetBaseDir(), "iTextExamples", "resources", "img", "multipage.tif");
        var pdfFilePath = TestUtils.GetOutputFileName();

        // It uses BitMiracle.LibTiff.NET package
        // https://github.com/BitMiracle/libtiff.net
        using (var tiff = Tiff.Open(fileName, "r"))
        {
            if (tiff == null)
            {
                throw new InvalidOperationException("Could not open the incoming image");
            }

            using (var stream = new FileStream(pdfFilePath, FileMode.Create))
            {
                using (var document = new Document())
                {
                    var writer = PdfWriter.GetInstance(document, stream);
                    writer.CompressionLevel = PdfStream.NO_COMPRESSION;

                    document.Open();
                    document.AddAuthor(TestUtils.Author);

                    do
                    {
                        var skBitmap = ConvertToSKBitmap(tiff);
                        var img = Image.GetInstance(skBitmap, SKEncodedImageFormat.Jpeg);
                        img.ScaleAbsolute(img.Width, img.Height);
                        img.SetAbsolutePosition(0, 0);
                        img.SetDpi(img.DpiX, img.DpiY);

                        document.Add(img);
                        document.NewPage();
                    }
                    while (tiff.ReadDirectory());
                }
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static int CalculatePageNumbers(Tiff image)
    {
        var pageCount = 0;

        do
        {
            ++pageCount;
        }
        while (image.ReadDirectory());

        return pageCount;
    }

    // Convert a TIFF stream to a SKBitmap
    public static SKBitmap ConvertToSKBitmap(Tiff tiff)
    {
        // read the dimensions
        var width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
        var height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

        // create the bitmap
        var bitmap = new SKBitmap();
        var info = new SKImageInfo(width, height);

        // create the buffer that will hold the pixels
        var raster = new int[width * height];

        // get a pointer to the buffer, and give it to the bitmap
        var ptr = GCHandle.Alloc(raster, GCHandleType.Pinned);
        bitmap.InstallPixels(info, ptr.AddrOfPinnedObject(), info.RowBytes, null, null);

        // read the image into the memory buffer
        if (!tiff.ReadRGBAImageOriented(width, height, raster, Orientation.TOPLEFT))
        {
            throw new Exception("Not a valid TIF image.");
        }

        // swap the red and blue because SkiaSharp may differ from the tiff
        if (SKImageInfo.PlatformColorType == SKColorType.Bgra8888)
        {
            SKSwizzle.SwapRedBlue(ptr.AddrOfPinnedObject(), raster.Length);
        }

        return bitmap;
    }
}