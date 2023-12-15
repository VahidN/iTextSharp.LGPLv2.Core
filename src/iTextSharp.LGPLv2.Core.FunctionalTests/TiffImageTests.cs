using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.codec;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
}