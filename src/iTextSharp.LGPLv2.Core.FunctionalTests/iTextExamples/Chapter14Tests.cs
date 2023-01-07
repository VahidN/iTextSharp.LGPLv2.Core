using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter14Tests
{
    [TestMethod]
    public void Verify_PathConstructionAndPainting_CanBeCreated()
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
                var canvas = writer.DirectContent;
                // draw squares
                CreateSquares(canvas, 50, 720, 80, 20);
                ColumnText.ShowTextAligned(
                                           canvas, Element.ALIGN_LEFT,
                                           new
                                               Phrase("Methods MoveTo(), LineTo(), stroke(), closePathStroke(), Fill(), and closePathFill()"),
                                           50, 700, 0
                                          );
                // draw Bezier curves
                CreateBezierCurves(canvas, 70, 600, 80, 670, 140, 690, 160, 630, 160);
                ColumnText.ShowTextAligned(
                                           canvas, Element.ALIGN_LEFT,
                                           new Phrase("Different CurveTo() methods, followed by stroke()"),
                                           50, 580, 0
                                          );
                // draw stars and circles
                CreateStarsAndCircles(canvas, 50, 470, 40, 20);
                ColumnText.ShowTextAligned(
                                           canvas, Element.ALIGN_LEFT,
                                           new
                                               Phrase("Methods Fill(), eoFill(), NewPath(), FillStroke(), and EoFillStroke()"),
                                           50, 450, 0
                                          );
                // draw different shapes using convenience methods
                canvas.SaveState();
                canvas.SetColorStroke(new GrayColor(0.2f));
                canvas.SetColorFill(new GrayColor(0.9f));
                canvas.Arc(50, 270, 150, 330, 45, 270);
                canvas.Ellipse(170, 270, 270, 330);
                canvas.Circle(320, 300, 30);
                canvas.RoundRectangle(370, 270, 80, 60, 20);
                canvas.FillStroke();
                canvas.RestoreState();
                var rect = new Rectangle(470, 270, 550, 330)
                           {
                               BorderWidthBottom = 10,
                               BorderColorBottom = new GrayColor(0f),
                               BorderWidthLeft = 4,
                               BorderColorLeft = new GrayColor(0.9f),
                               BackgroundColor = new GrayColor(0.4f),
                           };
                canvas.Rectangle(rect);
                ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT,
                                           new Phrase("Convenience methods"), 50, 250, 0);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static void CreateCircle(PdfContentByte canvas, float x, float y, float r, bool clockwise)
    {
        var b = 0.5523f;
        if (clockwise)
        {
            canvas.MoveTo(x + r, y);
            canvas.CurveTo(x + r, y - r * b, x + r * b, y - r, x, y - r);
            canvas.CurveTo(x - r * b, y - r, x - r, y - r * b, x - r, y);
            canvas.CurveTo(x - r, y + r * b, x - r * b, y + r, x, y + r);
            canvas.CurveTo(x + r * b, y + r, x + r, y + r * b, x + r, y);
        }
        else
        {
            canvas.MoveTo(x + r, y);
            canvas.CurveTo(x + r, y + r * b, x + r * b, y + r, x, y + r);
            canvas.CurveTo(x - r * b, y + r, x - r, y + r * b, x - r, y);
            canvas.CurveTo(x - r, y - r * b, x - r * b, y - r, x, y - r);
            canvas.CurveTo(x + r * b, y - r, x + r, y - r * b, x + r, y);
        }
    }

    private static void CreateStar(PdfContentByte canvas, float x, float y)
    {
        canvas.MoveTo(x + 10, y);
        canvas.LineTo(x + 80, y + 60);
        canvas.LineTo(x, y + 60);
        canvas.LineTo(x + 70, y);
        canvas.LineTo(x + 40, y + 90);
        canvas.ClosePath();
    }

    private static void CreateStarsAndCircles(PdfContentByte canvas, float x, float y, float radius, float gutter)
    {
        canvas.SaveState();
        canvas.SetColorStroke(new GrayColor(0.2f));
        canvas.SetColorFill(new GrayColor(0.9f));
        CreateStar(canvas, x, y);
        CreateCircle(canvas, x + radius, y - 70, radius, true);
        CreateCircle(canvas, x + radius, y - 70, radius / 2, true);
        canvas.Fill();
        x += 2 * radius + gutter;
        CreateStar(canvas, x, y);
        CreateCircle(canvas, x + radius, y - 70, radius, true);
        CreateCircle(canvas, x + radius, y - 70, radius / 2, true);
        canvas.EoFill();
        x += 2 * radius + gutter;
        CreateStar(canvas, x, y);
        canvas.NewPath();
        CreateCircle(canvas, x + radius, y - 70, radius, true);
        CreateCircle(canvas, x + radius, y - 70, radius / 2, true);
        x += 2 * radius + gutter;
        CreateStar(canvas, x, y);
        CreateCircle(canvas, x + radius, y - 70, radius, true);
        CreateCircle(canvas, x + radius, y - 70, radius / 2, false);
        canvas.FillStroke();
        x += 2 * radius + gutter;
        CreateStar(canvas, x, y);
        CreateCircle(canvas, x + radius, y - 70, radius, true);
        CreateCircle(canvas, x + radius, y - 70, radius / 2, true);
        canvas.EoFillStroke();
        canvas.RestoreState();
    }

    private static void CreateBezierCurves(PdfContentByte cb, float x0, float y0,
                                           float x1, float y1, float x2, float y2, float x3,
                                           float y3, float distance)
    {
        cb.MoveTo(x0, y0);
        cb.LineTo(x1, y1);
        cb.MoveTo(x2, y2);
        cb.LineTo(x3, y3);
        cb.MoveTo(x0, y0);
        cb.CurveTo(x1, y1, x2, y2, x3, y3);
        x0 += distance;
        x1 += distance;
        x2 += distance;
        x3 += distance;
        cb.MoveTo(x2, y2);
        cb.LineTo(x3, y3);
        cb.MoveTo(x0, y0);
        cb.CurveTo(x2, y2, x3, y3);
        x0 += distance;
        x1 += distance;
        x2 += distance;
        x3 += distance;
        cb.MoveTo(x0, y0);
        cb.LineTo(x1, y1);
        cb.MoveTo(x0, y0);
        cb.CurveTo(x1, y1, x3, y3);
        cb.Stroke();
    }

    private static void CreateSquares(PdfContentByte canvas,
                                      float x, float y, float side, float gutter)
    {
        canvas.SaveState();
        canvas.SetColorStroke(new GrayColor(0.2f));
        canvas.SetColorFill(new GrayColor(0.9f));
        canvas.MoveTo(x, y);
        canvas.LineTo(x + side, y);
        canvas.LineTo(x + side, y + side);
        canvas.LineTo(x, y + side);
        canvas.Stroke();
        x = x + side + gutter;
        canvas.MoveTo(x, y);
        canvas.LineTo(x + side, y);
        canvas.LineTo(x + side, y + side);
        canvas.LineTo(x, y + side);
        canvas.ClosePathStroke();
        x = x + side + gutter;
        canvas.MoveTo(x, y);
        canvas.LineTo(x + side, y);
        canvas.LineTo(x + side, y + side);
        canvas.LineTo(x, y + side);
        canvas.Fill();
        x = x + side + gutter;
        canvas.MoveTo(x, y);
        canvas.LineTo(x + side, y);
        canvas.LineTo(x + side, y + side);
        canvas.LineTo(x, y + side);
        canvas.FillStroke();
        x = x + side + gutter;
        canvas.MoveTo(x, y);
        canvas.LineTo(x + side, y);
        canvas.LineTo(x + side, y + side);
        canvas.LineTo(x, y + side);
        canvas.ClosePathFillStroke();
        canvas.RestoreState();
    }
}