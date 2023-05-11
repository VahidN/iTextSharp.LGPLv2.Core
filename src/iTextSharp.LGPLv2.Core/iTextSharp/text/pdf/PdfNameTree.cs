using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Creates a name tree.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public static class PdfNameTree
{
    private const int LeafSize = 64;

    public static INullValueDictionary<string, PdfObject> ReadTree(PdfDictionary dic)
    {
        var items = new NullValueDictionary<string, PdfObject>();
        if (dic != null)
        {
            iterateItems(dic, items);
        }

        return items;
    }

    /// <summary>
    ///     Creates a name tree.
    ///     and the value is a  PdfObject . Note that although the
    ///     keys are strings only the lower byte is used and no check is made for chars
    ///     with the same lower byte and different upper byte. This will generate a wrong
    ///     tree name.
    ///     @throws IOException on error
    ///     generally pointed to by the key /Dests, for example
    /// </summary>
    /// <param name="items">the item of the name tree. The key is a  String </param>
    /// <param name="writer">the writer</param>
    /// <returns>the dictionary with the name tree. This dictionary is the one</returns>
    public static PdfDictionary WriteTree(INullValueDictionary<string, PdfObject> items, PdfWriter writer)
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

        var names = new string[items.Count];
        items.Keys.CopyTo(names, 0);
        Array.Sort(names);
        if (names.Length <= LeafSize)
        {
            var dic = new PdfDictionary();
            var ar = new PdfArray();
            for (var k = 0; k < names.Length; ++k)
            {
                ar.Add(new PdfString(names[k], null));
                ar.Add(items[names[k]]);
            }

            dic.Put(PdfName.Names, ar);
            return dic;
        }

        var skip = LeafSize;
        var kids = new PdfIndirectReference[(names.Length + LeafSize - 1) / LeafSize];
        for (var k = 0; k < kids.Length; ++k)
        {
            var offset = k * LeafSize;
            var end = Math.Min(offset + LeafSize, names.Length);
            var dic = new PdfDictionary();
            var arr = new PdfArray();
            arr.Add(new PdfString(names[offset], null));
            arr.Add(new PdfString(names[end - 1], null));
            dic.Put(PdfName.Limits, arr);
            arr = new PdfArray();
            for (; offset < end; ++offset)
            {
                arr.Add(new PdfString(names[offset], null));
                arr.Add(items[names[offset]]);
            }

            dic.Put(PdfName.Names, arr);
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
            var tt = (names.Length + skip - 1) / skip;
            for (var k = 0; k < tt; ++k)
            {
                var offset = k * LeafSize;
                var end = Math.Min(offset + LeafSize, top);
                var dic = new PdfDictionary();
                var arr = new PdfArray();
                arr.Add(new PdfString(names[k * skip], null));
                arr.Add(new PdfString(names[Math.Min((k + 1) * skip, names.Length) - 1], null));
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

    private static void iterateItems(PdfDictionary dic, INullValueDictionary<string, PdfObject> items)
    {
        var nn = (PdfArray)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Names));
        if (nn != null)
        {
            for (var k = 0; k < nn.Size; ++k)
            {
                var s = (PdfString)PdfReader.GetPdfObjectRelease(nn[k++]);
                items[PdfEncodings.ConvertToString(s.GetBytes(), null)] = nn[k];
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