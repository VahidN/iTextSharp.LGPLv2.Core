using System.Collections;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples
{
    [TestClass]
    public class Chapter15Tests
    {
        [TestMethod]
        public void Verify_OptionalContentActionExample_CanBeCreated()
        {
            var pdfFilePath = TestUtils.GetOutputFileName();
            var stream = new FileStream(pdfFilePath, FileMode.Create);

            // step 1
            var document = new Document();

            // step 2
            PdfWriter writer = PdfWriter.GetInstance(document, stream);
            writer.PdfVersion = PdfWriter.VERSION_1_5;
            // step 3
            document.AddAuthor(TestUtils.Author);
            document.Open();
            // step 4
            PdfLayer a1 = new PdfLayer("answer 1", writer);
            PdfLayer a2 = new PdfLayer("answer 2", writer);
            PdfLayer a3 = new PdfLayer("answer 3", writer);
            a1.On = false;
            a2.On = false;
            a3.On = false;

            BaseFont bf = BaseFont.CreateFont();
            PdfContentByte cb = writer.DirectContent;
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

            var stateOn = new ArrayList {"ON", a1, a2, a3};
            PdfAction actionOn = PdfAction.SetOcGstate(stateOn, true);
            var stateOff = new ArrayList {"OFF", a1, a2, a3};
            PdfAction actionOff = PdfAction.SetOcGstate(stateOff, true);
            var stateToggle = new ArrayList {"Toggle", a1, a2, a3};
            PdfAction actionToggle = PdfAction.SetOcGstate(stateToggle, true);
            Phrase p = new Phrase("Change the state of the answers:");
            Chunk on = new Chunk(" on ").SetAction(actionOn);
            p.Add(on);
            Chunk off = new Chunk("/ off ").SetAction(actionOff);
            p.Add(off);
            Chunk toggle = new Chunk("/ toggle").SetAction(actionToggle);
            p.Add(toggle);
            document.Add(p);

            document.Close();
            stream.Dispose();

            TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
        }
    }
}