using System;
using System.Collections;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Creates a number tree.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class PdfNumberTree
    {

        private const int LeafSize = 64;

        public static Hashtable ReadTree(PdfDictionary dic)
        {
            Hashtable items = new Hashtable();
            if (dic != null)
                iterateItems(dic, items);
            return items;
        }

        /// <summary>
        /// Creates a number tree.
        /// and the value is a  PdfObject .
        /// @throws IOException on error
        /// </summary>
        /// <param name="items">the item of the number tree. The key is an  Integer </param>
        /// <param name="writer">the writer</param>
        /// <returns>the dictionary with the number tree.</returns>
        public static PdfDictionary WriteTree(Hashtable items, PdfWriter writer)
        {
            if (items.Count == 0)
                return null;
            int[] numbers = new int[items.Count];
            items.Keys.CopyTo(numbers, 0);
            Array.Sort(numbers);
            if (numbers.Length <= LeafSize)
            {
                PdfDictionary dic = new PdfDictionary();
                PdfArray ar = new PdfArray();
                for (int k = 0; k < numbers.Length; ++k)
                {
                    ar.Add(new PdfNumber(numbers[k]));
                    ar.Add((PdfObject)items[numbers[k]]);
                }
                dic.Put(PdfName.Nums, ar);
                return dic;
            }
            int skip = LeafSize;
            PdfIndirectReference[] kids = new PdfIndirectReference[(numbers.Length + LeafSize - 1) / LeafSize];
            for (int k = 0; k < kids.Length; ++k)
            {
                int offset = k * LeafSize;
                int end = Math.Min(offset + LeafSize, numbers.Length);
                PdfDictionary dic = new PdfDictionary();
                PdfArray arr = new PdfArray();
                arr.Add(new PdfNumber(numbers[offset]));
                arr.Add(new PdfNumber(numbers[end - 1]));
                dic.Put(PdfName.Limits, arr);
                arr = new PdfArray();
                for (; offset < end; ++offset)
                {
                    arr.Add(new PdfNumber(numbers[offset]));
                    arr.Add((PdfObject)items[numbers[offset]]);
                }
                dic.Put(PdfName.Nums, arr);
                kids[k] = writer.AddToBody(dic).IndirectReference;
            }
            int top = kids.Length;
            while (true)
            {
                if (top <= LeafSize)
                {
                    PdfArray arr = new PdfArray();
                    for (int k = 0; k < top; ++k)
                        arr.Add(kids[k]);
                    PdfDictionary dic = new PdfDictionary();
                    dic.Put(PdfName.Kids, arr);
                    return dic;
                }
                skip *= LeafSize;
                int tt = (numbers.Length + skip - 1) / skip;
                for (int k = 0; k < tt; ++k)
                {
                    int offset = k * LeafSize;
                    int end = Math.Min(offset + LeafSize, top);
                    PdfDictionary dic = new PdfDictionary();
                    PdfArray arr = new PdfArray();
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

        private static void iterateItems(PdfDictionary dic, Hashtable items)
        {
            PdfArray nn = (PdfArray)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Nums));
            if (nn != null)
            {
                for (int k = 0; k < nn.Size; ++k)
                {
                    PdfNumber s = (PdfNumber)PdfReader.GetPdfObjectRelease(nn[k++]);
                    items[s.IntValue] = nn[k];
                }
            }
            else if ((nn = (PdfArray)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Kids))) != null)
            {
                for (int k = 0; k < nn.Size; ++k)
                {
                    PdfDictionary kid = (PdfDictionary)PdfReader.GetPdfObjectRelease(nn[k]);
                    iterateItems(kid, items);
                }
            }
        }
    }
}
