using System.IO;
using System.util;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/24
/// </summary>
[TestClass]
public class Issue24
{
    [TestMethod]
    public void Verify_Issue24_CanBe_Processed()
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

        var props = new NullValueDictionary<string, object>
                    {
                        { "font_factory", unicodeFontProvider },
                    };

        var document = new Document();
        PdfWriter.GetInstance(document, stream);
        document.AddAuthor(TestUtils.Author);
        document.Open();
        var objects = HtmlWorker.ParseToList(
                                             new StringReader(@"<b>This is a test</b>
                     <br/>
                     <span style='color:blue;font-size:20pt;font-family:tahoma;font-style:italic;font-weight:bold'>
                        <b>Hi<b/>
                    </span>"),
                                             styles,
                                             props
                                            );
        foreach (var element in objects)
        {
            document.Add(element);
        }

        document.Close();
        stream.Dispose();

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}