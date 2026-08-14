using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/298
/// </summary>
[TestClass]
public class Issue298
{
    [TestMethod]
    public void Test_Issue298_Verify_PDFA1B_CanBeCreated()
    {
        var outFile = TestUtils.GetOutputFileName();

        using (var fileStream = new FileStream(outFile, FileMode.Create))
        {
            using var pdfDoc = new Document(PageSize.A4, marginLeft: 15, marginRight: 15, marginTop: 15,
                marginBottom: 15);

            var writer = PdfWriter.GetInstance(pdfDoc, fileStream);
            writer.PdfxConformance = PdfWriter.PDFA1B;			
			writer.CreateXmpMetadata();

            pdfDoc.AddAuthor(TestUtils.Author);
            pdfDoc.Open();

            SetColorProfile(writer, File.ReadAllBytes(TestUtils.GetIccPath(fileName: "srgb.profile")));

            var font = TestUtils.GetUnicodeFont(fontName: "Liberation Sans", TestUtils.GetFontPath("LiberationSans-Regular.ttf"), size: 10,
                Font.NORMAL, BaseColor.Black);

            pdfDoc.Add(new Paragraph(str: "Vă rugăm să confirmați — Größe 12,5 m³", font));
        }

        TestUtils.VerifyPdfFileIsReadable(outFile);
    }

    /// <summary>
    ///     Sets PDF/A Conformance ColorProfile.
    /// </summary>
    public static void SetColorProfile(PdfWriter pdfWriter, byte[] profileBytes)
    {
        var pdfDictionary = new PdfDictionary(PdfName.Outputintent);
        pdfDictionary.Put(PdfName.Outputconditionidentifier, new PdfString(value: "sRGB IEC61966-2.1"));
        pdfDictionary.Put(PdfName.Info, new PdfString(value: "sRGB IEC61966-2.1"));
        pdfDictionary.Put(PdfName.S, PdfName.GtsPdfa1);

        var pdfICCBased = new PdfIccBased(IccProfile.GetInstance(profileBytes));
        pdfICCBased.Remove(PdfName.Alternate);
        pdfDictionary.Put(PdfName.Destoutputprofile, pdfWriter.AddToBody(pdfICCBased).IndirectReference);

        pdfWriter.ExtraCatalog.Put(PdfName.Outputintents, new PdfArray(pdfDictionary));
    }
	
    // PDF/A:
    // Encryption is not allowed.
    // Embedded files are not allowed.
    // All fonts must be embedded.
    // Transparent images are forbidden.	
}