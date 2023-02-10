using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter15Tests
{
    [TestMethod]
    public void Verify_OptionalContentActionExample_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                var writer = PdfWriter.GetInstance(document, stream);
                writer.PdfVersion = PdfWriter.VERSION_1_5;
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var a1 = new PdfLayer("answer 1", writer);
                var a2 = new PdfLayer("answer 2", writer);
                var a3 = new PdfLayer("answer 3", writer);
                a1.On = false;
                a2.On = false;
                a3.On = false;

                var bf = BaseFont.CreateFont();
                var cb = writer.DirectContent;
                cb.BeginText();
                cb.SetFontAndSize(bf, 18);
                cb.ShowTextAligned(Element.ALIGN_LEFT,
                                   "Q1: Who is the director of the movie 'Paths of Glory'?", 50, 766, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT,
                                   "Q2: Who directed the movie 'Lawrence of Arabia'?", 50, 718, 0);
                cb.ShowTextAligned(Element.ALIGN_LEFT,
                                   "Q3: Who is the director of 'House of Flying Daggers'?", 50, 670, 0);
                cb.EndText();
                cb.SaveState();
                cb.SetRgbColorFill(0xFF, 0x00, 0x00);
                cb.BeginText();
                cb.BeginLayer(a1);
                cb.ShowTextAligned(Element.ALIGN_LEFT,
                                   "A1: Stanley Kubrick", 50, 742, 0);
                cb.EndLayer();
                cb.BeginLayer(a2);
                cb.ShowTextAligned(Element.ALIGN_LEFT,
                                   "A2: David Lean", 50, 694, 0);
                cb.EndLayer();
                cb.BeginLayer(a3);
                cb.ShowTextAligned(Element.ALIGN_LEFT,
                                   "A3: Zhang Yimou", 50, 646, 0);
                cb.EndLayer();
                cb.EndText();
                cb.RestoreState();

                var stateOn = new List<object> { "ON", a1, a2, a3 };
                var actionOn = PdfAction.SetOcGstate(stateOn, true);
                var stateOff = new List<object> { "OFF", a1, a2, a3 };
                var actionOff = PdfAction.SetOcGstate(stateOff, true);
                var stateToggle = new List<object> { "Toggle", a1, a2, a3 };
                var actionToggle = PdfAction.SetOcGstate(stateToggle, true);
                var p = new Phrase("Change the state of the answers:");
                var on = new Chunk(" on ").SetAction(actionOn);
                p.Add(on);
                var off = new Chunk("/ off ").SetAction(actionOff);
                p.Add(off);
                var toggle = new Chunk("/ toggle").SetAction(actionToggle);
                p.Add(toggle);
                document.Add(p);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}