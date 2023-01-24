using System.IO;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class FillAcrobatFormsTests
{
    [TestMethod]
    public void Verify_Correct_Rendering_Of_Text_Fields_CanBeCreated()
    {
        using var stream = new FileStream(TestUtils.GetOutputFileName(), FileMode.Create);
        using var reader = new PdfReader(TestUtils.GetPdfsPath("template.pdf"));
        using var stamper = new PdfStamper(reader, stream);

        var form = stamper.AcroFields;
        form.SetField("{NUMERO_DIAS_VISTA2}", "2039");
    }
}