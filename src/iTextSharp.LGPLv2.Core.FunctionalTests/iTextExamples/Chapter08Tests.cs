using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter08Tests
{
    private readonly string[] _languages = { "English", "German", "French", "Spanish", "Dutch" };

    [TestMethod]
    public void Verify_RadioButtons_CanBeCreated()
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
                var cb = writer.DirectContent;
                var bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.NOT_EMBEDDED);

                var radiogroup = PdfFormField.CreateRadioButton(writer, true);
                radiogroup.FieldName = "language";
                var rect = new Rectangle(40, 806, 60, 788);
                for (var page = 0; page < _languages.Length;)
                {
                    var radio = new RadioCheckField(writer, rect, null, _languages[page])
                                {
                                    BackgroundColor = new GrayColor(0.8f),
                                };
                    var radiofield = radio.RadioField;
                    radiofield.PlaceInPage = ++page;
                    radiogroup.AddKid(radiofield);
                }

                writer.AddAnnotation(radiogroup);
                foreach (var lang in _languages)
                {
                    cb.BeginText();
                    cb.SetFontAndSize(bf, 18);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, lang, 70, 790, 0);
                    cb.EndText();
                    document.NewPage();
                }
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}