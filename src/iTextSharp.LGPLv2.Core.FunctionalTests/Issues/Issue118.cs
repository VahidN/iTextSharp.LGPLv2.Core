using System.Collections.Generic;
using System.IO;
using System.util;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/118
/// </summary>
[TestClass]
public class Issue118
{
    [TestMethod]
    public void Test_Issue118_Verify_ExtraCatalog_Works()
    {
        var inPdfFile = TestUtils.GetPdfsPath("sample.pdf");
        var outPdfFile = TestUtils.GetOutputFileName();

        var newKey = new PdfName("Verify_ExtraCatalog_Works");
        var newString = new PdfString("Verify_ExtraCatalog_Works");

        using (var stream = new FileStream(outPdfFile, FileMode.Create))
        {
            using (var reader = new PdfReader(inPdfFile))
            {
                using (var stamper = new PdfStamper(reader, stream))
                {
                    stamper.Writer.ExtraCatalog.Put(newKey, newString);

                    var bookmarks = new List<INullValueDictionary<string, object>>
                                    {
                                        new NullValueDictionary<string, object>
                                        {
                                            { "Title", Path.GetFileName(inPdfFile) },
                                        },
                                    };
                    stamper.Outlines = bookmarks;
                    stamper.Writer.ExtraCatalog.Put(PdfName.Pagemode, PdfName.Useoutlines); // Show bookmarks

                    stamper.Writer.ExtraCatalog.Put(PdfName.Lang, new PdfString("EN"));
                }
            }
        }

        using (var reader = new PdfReader(outPdfFile))
        {
            var root = reader.Catalog;
            var pageMode = root.Get(PdfName.Pagemode);
            Assert.IsNotNull(pageMode);

            var langObject = root.Get(PdfName.Lang);
            Assert.IsNotNull(langObject);

            var newKeyObject = root.Get(newKey);
            Assert.IsNotNull(newKeyObject);
        }
    }
}