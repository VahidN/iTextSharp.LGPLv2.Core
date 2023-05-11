using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text.html.simpleparser;

/// <summary>
///     @author  psoares
/// </summary>
public class IncTable : IElement
{
    private readonly INullValueDictionary<string, string> _props = new NullValueDictionary<string, string>();
    private List<PdfPCell> _cols;

    /// <summary>
    ///     Creates a new instance of IncTable
    /// </summary>
    public IncTable(INullValueDictionary<string, string> props)
    {
        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        foreach (var dc in props)
        {
            _props[dc.Key] = dc.Value;
        }
    }

    public IList<IList<PdfPCell>> Rows { get; } = new List<IList<PdfPCell>>();

    public virtual bool Process(IElementListener listener) => false;

    public virtual int Type => 0;

    public virtual bool IsContent() => false;

    public virtual bool IsNestable() => false;

    public virtual IList<Chunk> Chunks => null;

    public void AddCol(PdfPCell cell)
    {
        if (_cols == null)
        {
            _cols = new List<PdfPCell>();
        }

        _cols.Add(cell);
    }

    public void AddCols(IEnumerable<PdfPCell> ncols)
    {
        if (_cols == null)
        {
            _cols = new List<PdfPCell>(ncols);
        }
        else
        {
            _cols.AddRange(ncols);
        }
    }

    public PdfPTable BuildTable()
    {
        if (Rows.Count == 0)
        {
            return new PdfPTable(1);
        }

        var ncol = 0;

        var c0 = Rows[0];
        for (var k = 0; k < c0.Count; ++k)
        {
            ncol += c0[k].Colspan;
        }

        var table = new PdfPTable(ncol);

        var widths = _props["widths"];
        if (widths != null)
        {
            var intWidths = new List<int>();
            foreach (var widthElement in widths.Split(','))
            {
                intWidths.Add(int.Parse(widthElement, CultureInfo.InvariantCulture));
            }

            table.SetWidths(intWidths.ToArray());
        }

        var width = _props["width"];
        if (width == null)
        {
            table.WidthPercentage = 100;
        }
        else
        {
            if (width.EndsWith("%", StringComparison.OrdinalIgnoreCase))
            {
                table.WidthPercentage =
                    float.Parse(width.Substring(0, width.Length - 1), NumberFormatInfo.InvariantInfo);
            }
            else
            {
                table.TotalWidth = float.Parse(width, NumberFormatInfo.InvariantInfo);
                table.LockedWidth = true;
            }
        }

        for (var row = 0; row < Rows.Count; ++row)
        {
            var col = Rows[row];
            for (var k = 0; k < col.Count; ++k)
            {
                table.AddCell(col[k]);
            }
        }

        return table;
    }

    public void EndRow()
    {
        if (_cols != null)
        {
            _cols.Reverse();
            Rows.Add(_cols);
            _cols = null;
        }
    }
}