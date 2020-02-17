using System.Collections;
using System.IO;
using iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests
{
    [TestClass]
    public class HtmlWorkerTests
    {
        [TestMethod]
        public void Verify_Base64_Images_CanBe_Processed()
        {
            var pdfDoc = new Document(PageSize.A4);

            var pdfFilePath = TestUtils.GetOutputFileName();
            var fileStream = new FileStream(pdfFilePath, FileMode.Create);
            PdfWriter.GetInstance(pdfDoc, fileStream);

            pdfDoc.AddAuthor(TestUtils.Author);
            pdfDoc.Open();

            FontFactory.Register(TestUtils.GetTahomaFontPath());


            var html = "<img alt='' src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAodJREFUeNpsk0tME1EUhv87UwlCREhRFpi4cGMMRrTE4MaoxBhAsDyMssFHfCQu3BlXGuNKNy5NmqALoqEEMJWCgEUjYojllSpofIUNBNqmIKU6OnQennunUxvgJF86957z/+d27hkGigMlDJfOAmV7AcYsKGqIZljRSvhNE+CMTwEtXmBy2gQb7mCQJUBKkTIQYtfJYCNMAxO9hzq5CYmFiWFY6ISE9VFLRedc1SONeqwf+uJLuKreNPI9nltbLG0orhpqUCM90DRVoEbJ5MSLho1MMg1O0bHOuyoD9crCcxL+xa0HqwL+rEQHsb/CW89reO1aAyEuq+yp+zXvg66rgng8LrDXSmwYpUc8dZkmDsJNL+NCeVVXbWK+O32cpJ7E6OgkwuEwrl8phaHrVsfYD+x03XTPjN3nzZnD0HGxvPppTSLcLwo0I4lldRFK8jdCoZBlJquAbBnr0BD9GUTRvubahclW5qDukqkpIqlodGQ1At3UxZXaIUvauqsyjBV+jZJEJ3s83HO5j+UWI7E6C4mp2EQCTixyV2CvbbKzNmN2zNfHtbzPM3p4FOy/M5CXtwsOKZmmsOi2IHMvyyFhJhgY4BqutQ/aRRstocEngZzswnQnO+x1lqTjy8hIgNdyDc+x5nomxrKJhpcSp2lSrx48WlZhGArynG5hsLLoE7/jQ59f0aR7ZBkdbf7U6Ge+mKYaBvdx8wwZXjtWvfswfTrp3Over29J8NAXYO1t/v/7csZA5U5/Q35nH+aKt8OMR2POPSUFOyRmorvje3BiCt4b9zBANTmwGvP/aMoZRluJbURB8APmnPlQliNLzk8flxbeh9Du8eId5bYQ2SnxH36b/wQYABNFRsIaESsTAAAAAElFTkSuQmCC' />";

            var styles = new StyleSheet();

            PdfPCell pdfCell = new PdfPCell
            {
                Border = 0,
                RunDirection = PdfWriter.RUN_DIRECTION_LTR
            };

            using (var reader = new StringReader(html))
            {
                var parsedHtmlElements = HtmlWorker.ParseToList(reader, styles);

                foreach (IElement htmlElement in parsedHtmlElements)
                {
                    pdfCell.AddElement(htmlElement);
                }
            }

            var table1 = new PdfPTable(1);
            table1.AddCell(pdfCell);
            pdfDoc.Add(table1);


            pdfDoc.Close();
            fileStream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }

        [TestMethod]
        public void Verify_Unicode_Html_To_Pdf_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            // create a StyleSheet
            var styles = new StyleSheet();
            // set the default font's properties
            styles.LoadTagStyle(HtmlTags.BODY, "encoding", "Identity-H");
            styles.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONT, "Tahoma");
            styles.LoadTagStyle(HtmlTags.BODY, "size", "16pt");


            FontFactory.Register(TestUtils.GetTahomaFontPath());

            var unicodeFontProvider = FontFactoryImp.Instance;
            unicodeFontProvider.DefaultEmbedding = BaseFont.EMBEDDED;
            unicodeFontProvider.DefaultEncoding = BaseFont.IDENTITY_H;

            var props = new Hashtable
            {
                { "img_provider", new MyImageFactory() },
                { "font_factory", unicodeFontProvider } // Always use Unicode fonts
            };

            // step 1
            var document = new Document();
            // step 2
            PdfWriter.GetInstance(document, stream);
            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            var objects = HtmlWorker.ParseToList(
                new StringReader(createHtmlSnippet()),
                styles,
                props
            );
            foreach (IElement element in objects)
            {
                document.Add(element);
            }

            document.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }

        [TestMethod]
        public void Verify_Html_To_Pdf_With_colspan_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            // create a StyleSheet
            var styles = new StyleSheet();
            // set the default font's properties
            styles.LoadTagStyle(HtmlTags.BODY, "encoding", "Identity-H");
            styles.LoadTagStyle(HtmlTags.BODY, HtmlTags.FONT, "Tahoma");
            styles.LoadTagStyle(HtmlTags.BODY, "size", "16pt");

            FontFactory.Register(TestUtils.GetTahomaFontPath());

            var unicodeFontProvider = FontFactoryImp.Instance;
            unicodeFontProvider.DefaultEmbedding = BaseFont.EMBEDDED;
            unicodeFontProvider.DefaultEncoding = BaseFont.IDENTITY_H;

            var props = new Hashtable
            {
                { "font_factory", unicodeFontProvider }
            };

            var document = new Document();
            PdfWriter.GetInstance(document, stream);
            document.AddAuthor(TestUtils.Author);
            document.Open();
            var objects = HtmlWorker.ParseToList(
                new StringReader(@"<table style='width: 100%;font-size:9pt;font-family:Tahoma;'>
 <tr>
    <td align='center' bgcolor='#00FF00' colspan='2'>Hello</td>
 </tr>
 <tr>
    <td align = 'right'> Month </td>
    <td align = 'right'> Savings </td>
 </tr>
 <tr>
   <td align = 'right'> January </td>
   <td align = 'right'>$100 </td>
 </tr>
</table>"),
                styles,
                props
            );
            foreach (IElement element in objects)
            {
                document.Add(element);
            }

            document.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }

        private string createHtmlSnippet()
        {
            return @"<b>This is a test</b>
                     <br/>
                     <span style='color:blue;font-size:20pt;font-family:tahoma;font-style:italic;font-weight:bold'>
                        <b>اين يك آزمايش است.<b/>
                    </span>";
        }
    }
}