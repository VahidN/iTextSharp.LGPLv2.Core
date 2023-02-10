using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.iTextExamples;

[TestClass]
public class Chapter04Tests
{
    [TestMethod]
    public void Verify_CellHeights_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var table = new PdfPTable(2);
                // a long phrase
                var p = new Phrase(
                                   "Dr. iText or: How I Learned to Stop Worrying and Love PDF."
                                  );
                var cell = new PdfPCell(p);
                // the prhase is wrapped
                table.AddCell("wrap");
                cell.NoWrap = false;
                table.AddCell(cell);
                // the phrase isn't wrapped
                table.AddCell("no wrap");
                cell.NoWrap = true;
                table.AddCell(cell);
                // a long phrase with newlines
                p = new Phrase(
                               "Dr. iText or:\nHow I Learned to Stop Worrying\nand Love PDF.");
                cell = new PdfPCell(p);
                // the phrase fits the fixed height
                table.AddCell("fixed height (more than sufficient)");
                cell.FixedHeight = 72f;
                table.AddCell(cell);
                // the phrase doesn't fit the fixed height
                table.AddCell("fixed height (not sufficient)");
                cell.FixedHeight = 36f;
                table.AddCell(cell);
                // The minimum height is exceeded
                table.AddCell("minimum height");
                cell = new PdfPCell(new Phrase("Dr. iText")) { MinimumHeight = 36f };
                table.AddCell(cell);
                // The last row is extended
                table.ExtendLastRow = true;
                table.AddCell("extend last row");
                table.AddCell(cell);
                document.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_ColumnWidths_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var table = CreateTable1();
                document.Add(table);
                table = CreateTable2();
                table.SpacingBefore = 5;
                table.SpacingAfter = 5;
                document.Add(table);
                table = CreateTable3();
                document.Add(table);
                table = CreateTable4();
                table.SpacingBefore = 5;
                table.SpacingAfter = 5;
                document.Add(table);
                table = CreateTable5();
                document.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_MyFirstTable_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                document.Add(CreateFirstTable());
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_NestedTable_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var table = new PdfPTable(4);
                var nested1 = new PdfPTable(2);
                nested1.AddCell("1.1");
                nested1.AddCell("1.2");
                var nested2 = new PdfPTable(1);
                nested2.AddCell("12.1");
                nested2.AddCell("12.2");
                for (var k = 0; k < 16; ++k)
                {
                    if (k == 1)
                    {
                        table.AddCell(nested1);
                    }
                    else if (k == 12)
                    {
                        table.AddCell(new PdfPCell(nested2));
                    }
                    else
                    {
                        table.AddCell("cell " + k);
                    }
                }

                document.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_RotationAndColors_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var table = new PdfPTable(4);
                table.SetWidths(new[] { 1, 3, 3, 3 });
                table.WidthPercentage = 100;
                // row 1, cell 1
                var cell = new PdfPCell(new Phrase("COLOR"))
                           {
                               Rotation = 90,
                               VerticalAlignment = Element.ALIGN_TOP,
                           };
                table.AddCell(cell);
                // row 1, cell 2
                cell = new PdfPCell(new Phrase("red / no borders"))
                       {
                           Border = Rectangle.NO_BORDER,
                           BackgroundColor = BaseColor.Red,
                       };
                table.AddCell(cell);
                // row 1, cell 3
                cell = new PdfPCell(new Phrase("green / black bottom border"))
                       {
                           Border = Rectangle.BOTTOM_BORDER,
                           BorderColorBottom = BaseColor.Black,
                           BorderWidthBottom = 10f,
                           BackgroundColor = BaseColor.Green,
                       };
                table.AddCell(cell);
                // row 1, cell 4
                cell = new PdfPCell(new Phrase(
                                               "cyan / blue top border + padding"
                                              ))
                       {
                           Border = Rectangle.TOP_BORDER,
                           UseBorderPadding = true,
                           BorderWidthTop = 5f,
                           BorderColorTop = BaseColor.Blue,
                           BackgroundColor = BaseColor.Cyan,
                       };
                table.AddCell(cell);
                // row 2, cell 1
                cell = new PdfPCell(new Phrase("GRAY"))
                       {
                           Rotation = 90,
                           VerticalAlignment = Element.ALIGN_MIDDLE,
                       };
                table.AddCell(cell);
                // row 2, cell 2
                cell = new PdfPCell(new Phrase("0.6"))
                       {
                           Border = Rectangle.NO_BORDER,
                           GrayFill = 0.6f,
                       };
                table.AddCell(cell);
                // row 2, cell 3
                cell = new PdfPCell(new Phrase("0.75"))
                       {
                           Border = Rectangle.NO_BORDER,
                           GrayFill = 0.75f,
                       };
                table.AddCell(cell);
                // row 2, cell 4
                cell = new PdfPCell(new Phrase("0.9"))
                       {
                           Border = Rectangle.NO_BORDER,
                           GrayFill = 0.9f,
                       };
                table.AddCell(cell);
                // row 3, cell 1
                cell = new PdfPCell(new Phrase("BORDERS"))
                       {
                           Rotation = 90,
                           VerticalAlignment = Element.ALIGN_BOTTOM,
                       };
                table.AddCell(cell);
                // row 3, cell 2
                cell = new PdfPCell(new Phrase("different borders"))
                       {
                           BorderWidthLeft = 16f,
                           BorderWidthBottom = 12f,
                           BorderWidthRight = 8f,
                           BorderWidthTop = 4f,
                           BorderColorLeft = BaseColor.Red,
                           BorderColorBottom = BaseColor.Orange,
                           BorderColorRight = BaseColor.Yellow,
                           BorderColorTop = BaseColor.Green,
                       };
                table.AddCell(cell);
                // row 3, cell 3
                cell = new PdfPCell(new Phrase("with correct padding"))
                       {
                           UseBorderPadding = true,
                           BorderWidthLeft = 16f,
                           BorderWidthBottom = 12f,
                           BorderWidthRight = 8f,
                           BorderWidthTop = 4f,
                           BorderColorLeft = BaseColor.Red,
                           BorderColorBottom = BaseColor.Orange,
                           BorderColorRight = BaseColor.Yellow,
                           BorderColorTop = BaseColor.Green,
                       };
                table.AddCell(cell);
                // row 3, cell 4
                cell = new PdfPCell(new Phrase("red border"))
                       {
                           BorderWidth = 8f,
                           BorderColor = BaseColor.Red,
                       };
                table.AddCell(cell);
                document.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Spacing_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var table = new PdfPTable(2) { WidthPercentage = 100 };
                var p = new Phrase(
                                   "Dr. iText or: How I Learned to Stop Worrying " +
                                   "and Love the Portable Document Format."
                                  );
                var cell = new PdfPCell(p);
                table.AddCell("default leading / spacing");
                table.AddCell(cell);
                table.AddCell("absolute leading: 20");
                cell.SetLeading(20f, 0f);
                table.AddCell(cell);
                table.AddCell("absolute leading: 3; relative leading: 1.2");
                cell.SetLeading(3f, 1.2f);
                table.AddCell(cell);
                table.AddCell("absolute leading: 0; relative leading: 1.2");
                cell.SetLeading(0f, 1.2f);
                table.AddCell(cell);
                table.AddCell("no leading at all");
                cell.SetLeading(0f, 0f);
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(
                                               "Dr. iText or: How I Learned to Stop Worrying and Love PDF"));
                table.AddCell("padding 10");
                cell.Padding = 10;
                table.AddCell(cell);
                table.AddCell("padding 0");
                cell.Padding = 0;
                table.AddCell(cell);
                table.AddCell("different padding for left, right, top and bottom");
                cell.PaddingLeft = 20;
                cell.PaddingRight = 50;
                cell.PaddingTop = 0;
                cell.PaddingBottom = 5;
                table.AddCell(cell);
                p = new Phrase("iText in Action Second Edition");
                table.DefaultCell.Padding = 2;
                table.DefaultCell.UseAscender = false;
                table.DefaultCell.UseDescender = false;
                table.AddCell("padding 2; no ascender, no descender");
                table.AddCell(p);
                table.DefaultCell.UseAscender = true;
                table.DefaultCell.UseDescender = false;
                table.AddCell("padding 2; ascender, no descender");
                table.AddCell(p);
                table.DefaultCell.UseAscender = false;
                table.DefaultCell.UseDescender = true;
                table.AddCell("padding 2; descender, no ascender");
                table.AddCell(p);
                table.DefaultCell.UseAscender = true;
                table.DefaultCell.UseDescender = true;
                table.AddCell("padding 2; ascender and descender");
                cell.Padding = 2;
                cell.UseAscender = true;
                cell.UseDescender = true;
                table.AddCell(p);
                document.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_TableAlignment_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var table = CreateFirstTable();
                table.WidthPercentage = 50;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                document.Add(table);
                table.HorizontalAlignment = Element.ALIGN_CENTER;
                document.Add(table);
                table.HorizontalAlignment = Element.ALIGN_RIGHT;
                document.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_TableHeight_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                var table = CreateFirstTable();
                document.Add(new Paragraph(string.Format(
                                                         "Table height before document.Add(): {0}",
                                                         table.TotalHeight)
                                          ));
                document.Add(new Paragraph(
                                           string.Format("Height of the first row: {0}",
                                                         table.GetRowHeight(0))
                                          ));
                document.Add(table);
                document.Add(new Paragraph(string.Format(
                                                         "Table height after document.Add(): {0}",
                                                         table.TotalHeight
                                                        )));
                document.Add(new Paragraph(string.Format(
                                                         "Height of the first row: {0}",
                                                         table.GetRowHeight(0)
                                                        )));
                table = CreateFirstTable();
                document.Add(new Paragraph(string.Format(
                                                         "Table height before setTotalWidth(): {0}",
                                                         table.TotalHeight
                                                        )));
                document.Add(new Paragraph(string.Format(
                                                         "Height of the first row: {0}",
                                                         table.GetRowHeight(0)
                                                        )));
                table.TotalWidth = 50;
                table.LockedWidth = true;
                document.Add(new Paragraph(string.Format(
                                                         "Table height after setTotalWidth(): {0}",
                                                         table.TotalHeight
                                                        )));
                document.Add(new Paragraph(string.Format(
                                                         "Height of the first row: {0}",
                                                         table.GetRowHeight(0)
                                                        )));
                document.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_XMen_CanBeCreated()
    {
        var pdfFilePath = TestUtils.GetOutputFileName();
        using (var stream = new FileStream(pdfFilePath, FileMode.Create))
        {
            // step 1
            using (var document = new Document())
            {
                // step 2
                PdfWriter.GetInstance(document, stream);
                // step 3
                document.AddAuthor(TestUtils.Author);
                document.Open();
                // step 4
                // we'll use 4 images in this example
                Image[] img =
                {
                    Image.GetInstance(TestUtils.GetPosterPath("0120903.jpg")),
                    Image.GetInstance(TestUtils.GetPosterPath("0290334.jpg")),
                    Image.GetInstance(TestUtils.GetPosterPath("0376994.jpg")),
                    Image.GetInstance(TestUtils.GetPosterPath("0348150.jpg")),
                };
                // Creates a table with 6 columns
                var table = new PdfPTable(6) { WidthPercentage = 100 };
                // first movie
                table.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.DefaultCell.VerticalAlignment = Element.ALIGN_TOP;
                table.AddCell("X-Men");
                // we wrap he image in a PdfPCell
                var cell = new PdfPCell(img[0]);
                table.AddCell(cell);
                // second movie
                table.DefaultCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                table.AddCell("X2");
                // we wrap the image in a PdfPCell and let iText scale it
                cell = new PdfPCell(img[1], true);
                table.AddCell(cell);
                // third movie
                table.DefaultCell.VerticalAlignment = Element.ALIGN_BOTTOM;
                table.AddCell("X-Men: The Last Stand");
                // we add the image with addCell()
                table.AddCell(img[2]);
                // fourth movie
                table.AddCell("Superman Returns");
                cell = new PdfPCell();
                // we add it with addElement(); it can only take 50% of the width.
                img[3].WidthPercentage = 50;
                cell.AddElement(img[3]);
                table.AddCell(cell);
                // we complete the table (otherwise the last row won't be rendered)
                table.CompleteRow();
                document.Add(table);
            }
        }

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    private static PdfPTable CreateFirstTable()
    {
        // a table with three columns
        var table = new PdfPTable(3);
        // the cell object
        // we add a cell with colspan 3
        var cell = new PdfPCell(new Phrase("Cell with colspan 3")) { Colspan = 3 };
        table.AddCell(cell);
        // now we add a cell with rowspan 2
        cell = new PdfPCell(new Phrase("Cell with rowspan 2")) { Rowspan = 2 };
        table.AddCell(cell);
        // we add the four remaining cells with addCell()
        table.AddCell("row 1; cell 1");
        table.AddCell("row 1; cell 2");
        table.AddCell("row 2; cell 1");
        table.AddCell("row 2; cell 2");
        return table;
    }

    private static PdfPTable CreateTable1()
    {
        var table = new PdfPTable(3) { WidthPercentage = 288 / 5.23f };
        table.SetWidths(new[] { 2, 1, 1 });
        var cell = new PdfPCell(new Phrase("Table 1")) { Colspan = 3 };
        table.AddCell(cell);
        cell = new PdfPCell(new Phrase("Cell with rowspan 2")) { Rowspan = 2 };
        table.AddCell(cell);
        table.AddCell("row 1; cell 1");
        table.AddCell("row 1; cell 2");
        table.AddCell("row 2; cell 1");
        table.AddCell("row 2; cell 2");
        return table;
    }

    private static PdfPTable CreateTable2()
    {
        var table = new PdfPTable(3)
                    {
                        TotalWidth = 288,
                        LockedWidth = true,
                    };
        table.SetWidths(new float[] { 2, 1, 1 });
        var cell = new PdfPCell(new Phrase("Table 2")) { Colspan = 3 };
        table.AddCell(cell);
        cell = new PdfPCell(new Phrase("Cell with rowspan 2")) { Rowspan = 2 };
        table.AddCell(cell);
        table.AddCell("row 1; cell 1");
        table.AddCell("row 1; cell 2");
        table.AddCell("row 2; cell 1");
        table.AddCell("row 2; cell 2");
        return table;
    }

    private static PdfPTable CreateTable3()
    {
        var table = new PdfPTable(new float[] { 2, 1, 1 }) { WidthPercentage = 55.067f };
        var cell = new PdfPCell(new Phrase("Table 3")) { Colspan = 3 };
        table.AddCell(cell);
        cell = new PdfPCell(new Phrase("Cell with rowspan 2")) { Rowspan = 2 };
        table.AddCell(cell);
        table.AddCell("row 1; cell 1");
        table.AddCell("row 1; cell 2");
        table.AddCell("row 2; cell 1");
        table.AddCell("row 2; cell 2");
        return table;
    }

    private static PdfPTable CreateTable4()
    {
        var table = new PdfPTable(3);
        var rect = new Rectangle(523, 770);
        table.SetWidthPercentage(new float[] { 144, 72, 72 }, rect);
        var cell = new PdfPCell(new Phrase("Table 4")) { Colspan = 3 };
        table.AddCell(cell);
        cell = new PdfPCell(new Phrase("Cell with rowspan 2")) { Rowspan = 2 };
        table.AddCell(cell);
        table.AddCell("row 1; cell 1");
        table.AddCell("row 1; cell 2");
        table.AddCell("row 2; cell 1");
        table.AddCell("row 2; cell 2");
        return table;
    }

    private static PdfPTable CreateTable5()
    {
        var table = new PdfPTable(3);
        table.SetTotalWidth(new float[] { 144, 72, 72 });
        table.LockedWidth = true;
        var cell = new PdfPCell(new Phrase("Table 5")) { Colspan = 3 };
        table.AddCell(cell);
        cell = new PdfPCell(new Phrase("Cell with rowspan 2")) { Rowspan = 2 };
        table.AddCell(cell);
        table.AddCell("row 1; cell 1");
        table.AddCell("row 1; cell 2");
        table.AddCell("row 2; cell 1");
        table.AddCell("row 2; cell 2");
        return table;
    }
}