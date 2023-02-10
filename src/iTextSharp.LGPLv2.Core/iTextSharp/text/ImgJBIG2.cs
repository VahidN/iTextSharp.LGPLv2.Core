namespace iTextSharp.text;

/// <summary>
///     Support for JBIG2 images.
///     @since 2.1.5
/// </summary>
public class ImgJbig2 : Image
{
    /// <summary>
    ///     JBIG2 globals
    /// </summary>
    private readonly byte[] _global;

    /// <summary>
    ///     A unique hash
    /// </summary>
    private readonly byte[] _globalHash;

    /// <summary>
    ///     Empty constructor.
    /// </summary>
    public ImgJbig2() : base((Image)null)
    {
    }

    /// <summary>
    ///     Actual constructor for ImgJBIG2 images.
    /// </summary>
    /// <param name="width">the width of the image</param>
    /// <param name="height">the height of the image</param>
    /// <param name="data">the raw image data</param>
    /// <param name="globals">JBIG2 globals</param>
    public ImgJbig2(int width, int height, byte[] data, byte[] globals) : base((Uri)null)
    {
        type = JBIG2;
        originalType = ORIGINAL_JBIG2;
        scaledHeight = height;
        Top = scaledHeight;
        scaledWidth = width;
        Right = scaledWidth;
        bpc = 1;
        colorspace = 1;
        rawData = data;
        plainWidth = Width;
        plainHeight = Height;
        if (globals != null)
        {
            _global = globals;
            try
            {
                using (var md5 = MD5BouncyCastle.Create())
                {
                    _globalHash = md5.ComputeHash(_global);
                }
            }
            catch
            {
                //ignore
            }
        }
    }

    /// <summary>
    ///     Getter for the JBIG2 global data.
    /// </summary>
    /// <returns>an array of bytes</returns>
    public byte[] GlobalBytes => _global;

    /// <summary>
    ///     Getter for the unique hash.
    /// </summary>
    /// <returns>an array of bytes</returns>
    public byte[] GlobalHash => _globalHash;
}