using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class PdfVerifySignatureTests
{
    [TestMethod]
    public void Detect_Singature_Fields()
    {
        var filename = Path.Combine(TestUtils.GetBaseDir(), "iTextExamples", "resources", "pdfs", "issue_sig_fields.pdf");
        using var reader = new PdfReader(filename);
        var fields = reader.AcroFields;
        var names = fields.GetSignatureNames();
        Assert.AreEqual(1, names.Count);
        

    }
    
    [TestMethod]
    public void Detect_PKCS7()
    {
        var filename = Path.Combine(TestUtils.GetBaseDir(), "iTextExamples", "resources", "pdfs", "issue_sig_fields.pdf");
        using var reader = new PdfReader(filename);
        var fields = reader.AcroFields;
        var names = fields.GetSignatureNames();
        Assert.AreEqual(1, names.Count);

        var cover = fields.SignatureCoversWholeDocument(names[0]);
        Assert.IsTrue(cover);
        
        
        
        var pkcs7 = fields.VerifySignature(names[0]);
        Assert.IsNotNull(pkcs7);
        Assert.IsTrue(pkcs7.Verify());
    }


}