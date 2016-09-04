using System;
using System.Collections;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Instance of PdfReader in each output document.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class PdfReaderInstance
    {
        internal static PdfLiteral Identitymatrix = new PdfLiteral("[1 0 0 1 0 0]");
        internal static PdfNumber One = new PdfNumber(1);
        internal RandomAccessFileOrArray File;
        internal Hashtable ImportedPages = new Hashtable();
        internal int[] MyXref;
        internal ArrayList NextRound = new ArrayList();
        internal PdfReader reader;
        internal Hashtable Visited = new Hashtable();
        internal PdfWriter Writer;
        internal PdfReaderInstance(PdfReader reader, PdfWriter writer)
        {
            this.reader = reader;
            Writer = writer;
            File = reader.SafeFile;
            MyXref = new int[reader.XrefSize];
        }

        internal PdfReader Reader
        {
            get
            {
                return reader;
            }
        }

        internal RandomAccessFileOrArray ReaderFile
        {
            get
            {
                return File;
            }
        }

        /// <summary>
        /// Gets the content stream of a page as a PdfStream object.
        /// @since   2.1.3 (the method already existed without param compressionLevel)
        /// </summary>
        /// <param name="pageNumber">the page of which you want the stream</param>
        /// <param name="compressionLevel">the compression level you want to apply to the stream</param>
        /// <returns>a PdfStream object</returns>
        internal PdfStream GetFormXObject(int pageNumber, int compressionLevel)
        {
            PdfDictionary page = reader.GetPageNRelease(pageNumber);
            PdfObject contents = PdfReader.GetPdfObjectRelease(page.Get(PdfName.Contents));
            PdfDictionary dic = new PdfDictionary();
            byte[] bout = null;
            if (contents != null)
            {
                if (contents.IsStream())
                    dic.Merge((PrStream)contents);
                else
                    bout = reader.GetPageContent(pageNumber, File);
            }
            else
                bout = new byte[0];
            dic.Put(PdfName.Resources, PdfReader.GetPdfObjectRelease(page.Get(PdfName.Resources)));
            dic.Put(PdfName.TYPE, PdfName.Xobject);
            dic.Put(PdfName.Subtype, PdfName.Form);
            PdfImportedPage impPage = (PdfImportedPage)ImportedPages[pageNumber];
            dic.Put(PdfName.Bbox, new PdfRectangle(impPage.BoundingBox));
            PdfArray matrix = impPage.Matrix;
            if (matrix == null)
                dic.Put(PdfName.Matrix, Identitymatrix);
            else
                dic.Put(PdfName.Matrix, matrix);
            dic.Put(PdfName.Formtype, One);
            PrStream stream;
            if (bout == null)
            {
                stream = new PrStream((PrStream)contents, dic);
            }
            else
            {
                stream = new PrStream(reader, bout);
                stream.Merge(dic);
            }
            return stream;
        }

        internal PdfImportedPage GetImportedPage(int pageNumber)
        {
            if (!reader.IsOpenedWithFullPermissions)
                throw new ArgumentException("PdfReader not opened with owner password");
            if (pageNumber < 1 || pageNumber > reader.NumberOfPages)
                throw new ArgumentException("Invalid page number: " + pageNumber);
            PdfImportedPage pageT = (PdfImportedPage)ImportedPages[pageNumber];
            if (pageT == null)
            {
                pageT = new PdfImportedPage(this, Writer, pageNumber);
                ImportedPages[pageNumber] = pageT;
            }
            return pageT;
        }

        internal int GetNewObjectNumber(int number, int generation)
        {
            if (MyXref[number] == 0)
            {
                MyXref[number] = Writer.IndirectReferenceNumber;
                NextRound.Add(number);
            }
            return MyXref[number];
        }
        internal PdfObject GetResources(int pageNumber)
        {
            PdfObject obj = PdfReader.GetPdfObjectRelease(reader.GetPageNRelease(pageNumber).Get(PdfName.Resources));
            return obj;
        }
        internal void WriteAllPages()
        {
            try
            {
                File.ReOpen();
                foreach (PdfImportedPage ip in ImportedPages.Values)
                {
                    Writer.AddToBody(ip.GetFormXObject(Writer.CompressionLevel), ip.IndirectReference);
                }
                WriteAllVisited();
            }
            finally
            {
                try
                {
                    reader.Close();
                    File.Close();
                }
                catch
                {
                    //Empty on purpose
                }
            }
        }

        internal void WriteAllVisited()
        {
            while (NextRound.Count > 0)
            {
                ArrayList vec = NextRound;
                NextRound = new ArrayList();
                for (int k = 0; k < vec.Count; ++k)
                {
                    int i = (int)vec[k];
                    if (!Visited.ContainsKey(i))
                    {
                        Visited[i] = null;
                        Writer.AddToBody(reader.GetPdfObjectRelease(i), MyXref[i]);
                    }
                }
            }
        }
    }
}