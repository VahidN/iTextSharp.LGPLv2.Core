using System.Collections;
using System.Collections.Generic;
using iTextSharp.text.pdf;

namespace iTextSharp.text.html.simpleparser
{

    /// <summary>
    /// @author  psoares
    /// </summary>
    public class IncTable
    {
        private readonly Hashtable _props = new Hashtable();
        private ArrayList _cols;

        /// <summary>
        /// Creates a new instance of IncTable
        /// </summary>
        public IncTable(Hashtable props)
        {
            foreach (DictionaryEntry dc in props)
                _props[dc.Key] = dc.Value;
        }

        public ArrayList Rows { get; } = new ArrayList();

        public void AddCol(PdfPCell cell)
        {
            if (_cols == null)
                _cols = new ArrayList();
            _cols.Add(cell);
        }

        public void AddCols(ArrayList ncols)
        {
            if (_cols == null)
                _cols = new ArrayList(ncols);
            else
                _cols.AddRange(ncols);
        }

        public PdfPTable BuildTable()
        {
            if (Rows.Count == 0)
                return new PdfPTable(1);

            int ncol = 0;

            ArrayList c0 = (ArrayList)Rows[0];
            for (int k = 0; k < c0.Count; ++k)
            {
                ncol += ((PdfPCell)c0[k]).Colspan;
            }

            PdfPTable table = new PdfPTable(ncol);

            var widths = (string)_props["widths"];
            if (widths != null)
            {
                var intWidths = new List<int>();
                foreach (var widthElement in widths.Split(','))
                {
                    intWidths.Add(int.Parse(widthElement));
                }
                table.SetWidths(intWidths.ToArray());
            }

            string width = (string)_props["width"];
            if (width == null)
                table.WidthPercentage = 100;
            else
            {
                if (width.EndsWith("%"))
                    table.WidthPercentage = float.Parse(width.Substring(0, width.Length - 1), System.Globalization.NumberFormatInfo.InvariantInfo);
                else
                {
                    table.TotalWidth = float.Parse(width, System.Globalization.NumberFormatInfo.InvariantInfo);
                    table.LockedWidth = true;
                }
            }

            for (int row = 0; row < Rows.Count; ++row)
            {
                ArrayList col = (ArrayList)Rows[row];
                for (int k = 0; k < col.Count; ++k)
                {
                    table.AddCell((PdfPCell)col[k]);
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
}