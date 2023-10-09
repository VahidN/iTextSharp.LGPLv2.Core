using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Instance of PdfReader in each output document.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfReaderInstance
{
    internal static readonly PdfLiteral Identitymatrix = new("[1 0 0 1 0 0]");
    internal static readonly PdfNumber One = new(1);
    internal readonly RandomAccessFileOrArray File;
    internal readonly INullValueDictionary<int, PdfImportedPage> ImportedPages = new NullValueDictionary<int, PdfImportedPage>();
    internal readonly int[] MyXref;
    internal List<int> NextRound = new();
    internal readonly PdfReader reader;
    internal readonly INullValueDictionary<int, object> Visited = new NullValueDictionary<int, object>();
    internal readonly PdfWriter Writer;

    internal PdfReaderInstance(PdfReader reader, PdfWriter writer)
    {
        this.reader = reader;
        Writer = writer;
        File = reader.SafeFile;
        MyXref = new int[reader.XrefSize];
    }

    internal PdfReader Reader => reader;

    internal RandomAccessFileOrArray ReaderFile => File;

    /// <summary>
    ///     Gets the content stream of a page as a PdfStream object.
    ///     @since   2.1.3 (the method already existed without param compressionLevel)
    /// </summary>
    /// <param name="pageNumber">the page of which you want the stream</param>
    /// <param name="compressionLevel">the compression level you want to apply to the stream</param>
    /// <returns>a PdfStream object</returns>
    internal PdfStream GetFormXObject(int pageNumber, int compressionLevel)
    {
        var page = reader.GetPageNRelease(pageNumber);
        var contents = PdfReader.GetPdfObjectRelease(page.Get(PdfName.Contents));
        var dic = new PdfDictionary();
        byte[] bout = null;
        if (contents != null)
        {
            if (contents.IsStream())
            {
                dic.Merge((PrStream)contents);
            }
            else
            {
                bout = reader.GetPageContent(pageNumber, File);
            }
        }
        else
        {
            bout = Array.Empty<byte>();
        }

        dic.Put(PdfName.Resources, PdfReader.GetPdfObjectRelease(page.Get(PdfName.Resources)));
        dic.Put(PdfName.TYPE, PdfName.Xobject);
        dic.Put(PdfName.Subtype, PdfName.Form);
        var impPage = ImportedPages[pageNumber];
        dic.Put(PdfName.Bbox, new PdfRectangle(impPage.BoundingBox));
        var matrix = impPage.Matrix;
        if (matrix == null)
        {
            dic.Put(PdfName.Matrix, Identitymatrix);
        }
        else
        {
            dic.Put(PdfName.Matrix, matrix);
        }

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
        {
            throw new ArgumentException("PdfReader not opened with owner password");
        }

        if (pageNumber < 1 || pageNumber > reader.NumberOfPages)
        {
            throw new ArgumentException("Invalid page number: " + pageNumber);
        }

        var pageT = ImportedPages[pageNumber];
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
        var obj = PdfReader.GetPdfObjectRelease(reader.GetPageNRelease(pageNumber).Get(PdfName.Resources));
        return obj;
    }

    internal void WriteAllPages()
    {
        try
        {
            File.ReOpen();
            foreach (var ip in ImportedPages.Values)
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
            var vec = NextRound;
            NextRound = new List<int>();
            for (var k = 0; k < vec.Count; ++k)
            {
                var i = vec[k];
                if (!Visited.ContainsKey(i))
                {
                    Visited[i] = null;
                    Writer.AddToBody(reader.GetPdfObjectRelease(i), MyXref[i]);
                }
            }
        }
    }
}