using System.IO;
using System.Text;
using System.util;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter09Tests
{
    [TestMethod]
    public void Verify_Html_To_Pdf_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // create a StyleSheet
            var styles = new StyleSheet();
            styles.LoadTagStyle("ul", "indent", "10");
            styles.LoadTagStyle("li", "leading", "14");
            styles.LoadStyle("country", "i", "");
            styles.LoadStyle("country", "color", "#008080");
            styles.LoadStyle("director", "b", "");
            styles.LoadStyle("director", "color", "midnightblue");

            var props = new NullValueDictionary<string, object>
                        {
                            { "img_provider", new MyImageFactory() },
                        };

            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var objects = HtmlWorker.ParseToList(
                                                     new StringReader(CreateHtmlSnippet()),
                                                     styles,
                                                     props
                                                    );
                foreach (var element in objects)
                {
                    document.Add(element);
                }
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static string CreateHtmlSnippet()
    {
        var buf = new StringBuilder("<table width='500'>\n<tr>\n");
        buf.Append("\t<td><img src='");
        buf.Append("0092005");
        buf.Append(".jpg' /></td>\t<td>\n");
        buf.Append(CreateHtmlData());
        buf.Append("\t</ul>\n\t</td>\n</tr>\n</table>");
        return buf.ToString();
    }

    private static string CreateHtmlData()
    {
        var buf = new StringBuilder("\t<span class='title'>");
        buf.Append("MovieTitle");
        buf.Append("</span><br />\n");
        buf.Append("\t<ul>\n");

        buf.Append("\t\t<li class='country'>");
        buf.Append("country.Name");
        buf.Append("</li>\n");

        buf.Append("\t</ul>\n");
        buf.Append("\tYear: <i>");
        buf.Append("movie.Year");
        buf.Append(" minutes</i><br />\n");
        buf.Append("\tDuration: <i>");
        buf.Append("Duration");
        buf.Append(" minutes</i><br />\n");
        buf.Append("\t<ul>\n");

        buf.Append("\t\t<li><span class='director'>");
        buf.Append("director.Name");
        buf.Append(", ");
        buf.Append("director.GivenName");
        buf.Append("</span></li>\n");

        buf.Append("\t</ul>\n");
        return buf.ToString();
    }
}

public class MyImageFactory : IImageProvider
{
    public Image GetImage(string src,
                          INullValueDictionary<string, string> h,
                          ChainedProperties cprops,
                          IDocListener doc)
    {
        var fileName = Path.GetFileName(src);
        return Image.GetInstance(TestUtils.GetPosterPath(fileName));
    }
}