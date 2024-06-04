using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Creates a number tree.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public static class PdfNumberTree
{
    private const int LeafSize = 64;

    public static INullValueDictionary<int, PdfObject> ReadTree(PdfDictionary dic)
    {
        var items = new NullValueDictionary<int, PdfObject>();
        if (dic != null)
        {
            iterateItems(dic, items);
        }

        return items;
    }

    /// <summary>
    ///     Creates a number tree.
    ///     and the value is a  PdfObject .
    ///     @throws IOException on error
    /// </summary>
    /// <param name="items">the item of the number tree. The key is an  Integer </param>
    /// <param name="writer">the writer</param>
    /// <returns>the dictionary with the number tree.</returns>
    public static PdfDictionary WriteTree<T>(INullValueDictionary<int, T> items, PdfWriter writer) where T : PdfObject
    {
        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (items.Count == 0)
        {
            return null;
        }

        var numbers = new int[items.Count];
        items.Keys.CopyTo(numbers, 0);
        Array.Sort(numbers);
        if (numbers.Length <= LeafSize)
        {
            var dic = new PdfDictionary();
            var ar = new PdfArray();
            for (var k = 0; k < numbers.Length; ++k)
            {
                ar.Add(new PdfNumber(numbers[k]));
                ar.Add(items[numbers[k]]);
            }

            dic.Put(PdfName.Nums, ar);
            return dic;
        }

        var skip = LeafSize;
        var kids = new PdfIndirectReference[(numbers.Length + LeafSize - 1) / LeafSize];
        for (var k = 0; k < kids.Length; ++k)
        {
            var offset = k * LeafSize;
            var end = Math.Min(offset + LeafSize, numbers.Length);
            var dic = new PdfDictionary();
            var arr = new PdfArray();
            arr.Add(new PdfNumber(numbers[offset]));
            arr.Add(new PdfNumber(numbers[end - 1]));
            dic.Put(PdfName.Limits, arr);
            arr = new PdfArray();
            for (; offset < end; ++offset)
            {
                arr.Add(new PdfNumber(numbers[offset]));
                arr.Add(items[numbers[offset]]);
            }

            dic.Put(PdfName.Nums, arr);
            kids[k] = writer.AddToBody(dic).IndirectReference;
        }

        var top = kids.Length;
        while (true)
        {
            if (top <= LeafSize)
            {
                var arr = new PdfArray();
                for (var k = 0; k < top; ++k)
                {
                    arr.Add(kids[k]);
                }

                var dic = new PdfDictionary();
                dic.Put(PdfName.Kids, arr);
                return dic;
            }

            skip *= LeafSize;
            var tt = (numbers.Length + skip - 1) / skip;
            for (var k = 0; k < tt; ++k)
            {
                var offset = k * LeafSize;
                var end = Math.Min(offset + LeafSize, top);
                var dic = new PdfDictionary();
                var arr = new PdfArray();
                arr.Add(new PdfNumber(numbers[k * skip]));
                arr.Add(new PdfNumber(numbers[Math.Min((k + 1) * skip, numbers.Length) - 1]));
                dic.Put(PdfName.Limits, arr);
                arr = new PdfArray();
                for (; offset < end; ++offset)
                {
                    arr.Add(kids[offset]);
                }

                dic.Put(PdfName.Kids, arr);
                kids[k] = writer.AddToBody(dic).IndirectReference;
            }

            top = tt;
        }
    }

    private static void iterateItems(PdfDictionary dic, INullValueDictionary<int, PdfObject> items)
    {
        var nn = (PdfArray)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Nums));
        if (nn != null)
        {
            for (var k = 0; k < nn.Size; ++k)
            {
                var s = (PdfNumber)PdfReader.GetPdfObjectRelease(nn[k++]);
                items[s.IntValue] = nn[k];
            }
        }
        else if ((nn = (PdfArray)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Kids))) != null)
        {
            for (var k = 0; k < nn.Size; ++k)
            {
                var kid = (PdfDictionary)PdfReader.GetPdfObjectRelease(nn[k]);
                iterateItems(kid, items);
            }
        }
    }
}