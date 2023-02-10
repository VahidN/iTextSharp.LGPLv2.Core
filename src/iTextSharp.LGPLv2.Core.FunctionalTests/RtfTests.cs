using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class RtfTests
{
    /// <summary>
    ///     The RtfWriter allows the creation of rtf documents via the iText system
    /// </summary>
    [TestMethod]
    public void Verify_PDF_To_RTF_File_CanBeCreated()
    {
        var rtfFilePath =
            Path.Combine(TestUtils.GetOutputFolder(), $"{nameof(Verify_PDF_To_RTF_File_CanBeCreated)}.rtf");

        using var fileStream = new FileStream(rtfFilePath, FileMode.Create);

        using var document = new Document();
        RtfWriter2.GetInstance(document, fileStream);

        document.AddAuthor(TestUtils.Author);
        document.Open();

        var font = FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD);
        var parTitle = new Paragraph("This is a new document", font);
        document.Add(parTitle);
    }

    /// <summary>
    ///     RTF to PDF using iTextSharp
    /// </summary>
    [TestMethod]
    public void Verify_RTF_To_PDF_File_CanBeCreated()
    {
        Verify_PDF_To_RTF_File_CanBeCreated(); // Create a sample .rtf file first

        var rtfFilePath =
            Path.Combine(TestUtils.GetOutputFolder(), $"{nameof(Verify_PDF_To_RTF_File_CanBeCreated)}.rtf");
        using var inputStream = new FileStream(rtfFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var pdfFilePath = TestUtils.GetOutputFileName();
        using var outputStream = new FileStream(pdfFilePath, FileMode.Create);

        using var document = new Document();
        PdfWriter.GetInstance(document, outputStream);

        document.Open();

        var parser = new RtfParser(null);
        parser.ConvertRtfDocument(inputStream, document);
    }
}