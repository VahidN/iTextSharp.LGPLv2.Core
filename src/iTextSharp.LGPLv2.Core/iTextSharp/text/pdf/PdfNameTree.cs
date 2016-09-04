using System;
using System.Collections;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Creates a name tree.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class PdfNameTree
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
        /// Creates a name tree.
        /// and the value is a  PdfObject . Note that although the
        /// keys are strings only the lower byte is used and no check is made for chars
        /// with the same lower byte and different upper byte. This will generate a wrong
        /// tree name.
        /// @throws IOException on error
        /// generally pointed to by the key /Dests, for example
        /// </summary>
        /// <param name="items">the item of the name tree. The key is a  String </param>
        /// <param name="writer">the writer</param>
        /// <returns>the dictionary with the name tree. This dictionary is the one</returns>
        public static PdfDictionary WriteTree(Hashtable items, PdfWriter writer)
        {
            if (items.Count == 0)
                return null;
            string[] names = new string[items.Count];
            items.Keys.CopyTo(names, 0);
            Array.Sort(names);
            if (names.Length <= LeafSize)
            {
                PdfDictionary dic = new PdfDictionary();
                PdfArray ar = new PdfArray();
                for (int k = 0; k < names.Length; ++k)
                {
                    ar.Add(new PdfString(names[k], null));
                    ar.Add((PdfObject)items[names[k]]);
                }
                dic.Put(PdfName.Names, ar);
                return dic;
            }
            int skip = LeafSize;
            PdfIndirectReference[] kids = new PdfIndirectReference[(names.Length + LeafSize - 1) / LeafSize];
            for (int k = 0; k < kids.Length; ++k)
            {
                int offset = k * LeafSize;
                int end = Math.Min(offset + LeafSize, names.Length);
                PdfDictionary dic = new PdfDictionary();
                PdfArray arr = new PdfArray();
                arr.Add(new PdfString(names[offset], null));
                arr.Add(new PdfString(names[end - 1], null));
                dic.Put(PdfName.Limits, arr);
                arr = new PdfArray();
                for (; offset < end; ++offset)
                {
                    arr.Add(new PdfString(names[offset], null));
                    arr.Add((PdfObject)items[names[offset]]);
                }
                dic.Put(PdfName.Names, arr);
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
                int tt = (names.Length + skip - 1) / skip;
                for (int k = 0; k < tt; ++k)
                {
                    int offset = k * LeafSize;
                    int end = Math.Min(offset + LeafSize, top);
                    PdfDictionary dic = new PdfDictionary();
                    PdfArray arr = new PdfArray();
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

        private static void iterateItems(PdfDictionary dic, Hashtable items)
        {
            PdfArray nn = (PdfArray)PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Names));
            if (nn != null)
            {
                for (int k = 0; k < nn.Size; ++k)
                {
                    PdfString s = (PdfString)PdfReader.GetPdfObjectRelease(nn[k++]);
                    items[PdfEncodings.ConvertToString(s.GetBytes(), null)] = nn[k];
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
