namespace iTextSharp.text.pdf;

/// <summary>
///     PdfResources  is the PDF Resources-object.
///     The marking operations for drawing a page are stored in a stream that is the value of the
///     <B>Contents</B> key in the Page object's dictionary. Each marking context includes a list
///     of the named resources it uses. This resource list is stored as a dictionary that is the
///     value of the context's <B>Resources</B> key, and it serves two functions: it enumerates
///     the named resources in the contents stream, and it established the mapping from the names
///     to the objects used by the marking operations.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 7.5 (page 195-197).
///     @see     PdfResource
///     @see     PdfProcSet
///     @see     PdfFontDictionary
///     @see     PdfPage
/// </summary>
internal class PdfResources : PdfDictionary
{
    /// <summary>
    ///     methods
    /// </summary>
    internal void Add(PdfName key, PdfDictionary resource)
    {
        if (resource.Size == 0)
        {
            return;
        }

        var dic = GetAsDict(key);
        if (dic == null)
        {
            Put(key, resource);
        }
        else
        {
            dic.Merge(resource);
        }
    }
}