namespace iTextSharp.text.pdf;

/// <summary>
///     A Rendition dictionary (pdf spec 1.5)
/// </summary>
public class PdfRendition : PdfDictionary
{
    /// <summary>
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fs"></param>
    /// <param name="mimeType"></param>
    public PdfRendition(string file, PdfFileSpecification fs, string mimeType)
    {
        if (fs == null)
        {
            throw new ArgumentNullException(nameof(fs));
        }

        Put(PdfName.S, new PdfName("MR"));
        Put(PdfName.N, new PdfString("Rendition for " + file));
        Put(PdfName.C, new PdfMediaClipData(file, fs, mimeType));
    }
}