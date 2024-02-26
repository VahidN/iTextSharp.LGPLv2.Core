using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

internal abstract class PdfShape : VerticalPositionMark
{
    public const float DefaultShapeWidth = 4.2f;
    public const float DefaultCenterOffset = -4.2f;
    public const float DefaultStokeThickness = 0.8f;

    public static readonly BaseColor DarkBlue = new(0, 0, 139, 255);

    protected float DetalY;

    public PdfShape(float detalY = 0)
        => DetalY = detalY;

    public sealed override void Draw(PdfContentByte canvas, float llx, float lly, float urx, float ury, float y)
        => DrawShape(canvas, urx, y);

    public abstract void DrawShape(PdfContentByte canvas, float x0, float y0);
}

internal class GeoCheckMark : PdfShape
{
    private readonly float _detalX;
    private readonly float _detalY;

    public GeoCheckMark(float detalX, float detalY)
    {
        _detalX = detalX;
        _detalY = detalY;
    }

    public override void DrawShape(PdfContentByte canvas, float x0, float y0)
    {
        canvas.SaveState();

        var _x0 = x0 + _detalX;
        var _y0 = y0 + DefaultShapeWidth + _detalY;
        canvas.MoveTo(_x0, _y0);
        canvas.LineTo(_x0 + DefaultShapeWidth, _y0 - DefaultShapeWidth);
        canvas.LineTo(_x0 + DefaultShapeWidth * 2, _y0 + DefaultShapeWidth);
        canvas.LineTo(_x0 + DefaultShapeWidth, _y0 - DefaultShapeWidth / 2);
        canvas.FillStroke();
        canvas.Fill();

        canvas.RestoreState();
    }
}

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/140
/// </summary>
[TestClass]
public class Issue140
{
    [TestMethod]
    public void Test_Issue140_Verify_MissingMethodException_Works()
    {
        var outFile = TestUtils.GetOutputFileName();

        using (var fileStream = new FileStream(outFile, FileMode.Create))
        {
            using var pdfDoc = new Document(PageSize.A4, 15, 15, 15, 15);
            var wt = PdfWriter.GetInstance(pdfDoc, fileStream);

            pdfDoc.AddAuthor(TestUtils.Author);
            pdfDoc.Open();

            var rootTab = new PdfPTable(1)
            {
                WidthPercentage = 100,
                SplitLate = false,
                SplitRows = true
            };

            var cell = new PdfPCell();
            cell.AddElement(new GeoCheckMark(-22, -8));
            rootTab.AddCell(cell);
            pdfDoc.Add(rootTab);
        }

        TestUtils.VerifyPdfFileIsReadable(outFile);
    }
}