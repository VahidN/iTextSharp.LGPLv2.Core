namespace iTextSharp.text.pdf.codec;

/// <summary>
///     Support for JBIG2 Images.
///     This class assumes that we are always embedding into a pdf.
///     @since 2.1.5
/// </summary>
public static class Jbig2Image
{
    /// <summary>
    ///     Gets a byte array that can be used as a /JBIG2Globals,
    ///     or null if not applicable to the given jbig2.
    /// </summary>
    /// <param name="ra">an random access file or array</param>
    /// <returns>a byte array</returns>
    public static byte[] GetGlobalSegment(RandomAccessFileOrArray ra)
    {
        try
        {
            var sr = new Jbig2SegmentReader(ra);
            sr.Read();
            return sr.GetGlobal(true);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     returns an Image representing the given page.
    /// </summary>
    /// <param name="ra">the file or array containing the image</param>
    /// <param name="page">the page number of the image</param>
    /// <returns>an Image object</returns>
    public static Image GetJbig2Image(RandomAccessFileOrArray ra, int page)
    {
        if (page < 1)
        {
            throw new ArgumentException("The page number must be >= 1.");
        }

        var sr = new Jbig2SegmentReader(ra);
        sr.Read();
        var p = sr.GetPage(page);
        Image img = new ImgJbig2(p.PageBitmapWidth, p.PageBitmapHeight, p.GetData(true), sr.GetGlobal(true));
        return img;
    }

    /// <summary>
    ///     Gets the number of pages in a JBIG2 image.
    /// </summary>
    /// <param name="ra">a random acces file array containing a JBIG2 image</param>
    /// <returns>the number of pages</returns>
    public static int GetNumberOfPages(RandomAccessFileOrArray ra)
    {
        var sr = new Jbig2SegmentReader(ra);
        sr.Read();
        return sr.NumberOfPages();
    }
}