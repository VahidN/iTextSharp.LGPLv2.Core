using System.Collections.Generic;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/42
/// </summary>
[TestClass]
public class Issue42
{
    [TestMethod]
    [Ignore]
    public void Verify_Issue42_CanBe_Processed()
    {
        var inPdfFile = TestUtils.GetPdfsPath("issue42.pdf");
        var reader = new PdfReader(inPdfFile);

        var content = reader.GetPageContent(1);
        var tokenizer = new PrTokeniser(new RandomAccessFileOrArray(content));

        var stringsList = new List<string>();
        while (tokenizer.NextToken())
        {
            if (tokenizer.TokenType == PrTokeniser.TK_STRING)
            {
                stringsList.Add(tokenizer.StringValue);
            }
        }

        reader.Close();
        Assert.IsTrue(stringsList.Contains("demonstration"));
    }
}