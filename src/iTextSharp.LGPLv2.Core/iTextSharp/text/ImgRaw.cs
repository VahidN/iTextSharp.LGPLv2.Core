namespace iTextSharp.text;

/// <summary>
///     Raw Image data that has to be inserted into the document
/// </summary>
public class ImgRaw : Image
{
    public ImgRaw(Image image) : base(image)
    {
    }

    /// <summary>
    ///     Creats an Image in raw mode.
    /// </summary>
    /// <param name="width">the exact width of the image</param>
    /// <param name="height">the exact height of the image</param>
    /// <param name="components">1,3 or 4 for GrayScale, RGB and CMYK</param>
    /// <param name="bpc">bits per component. Must be 1,2,4 or 8</param>
    /// <param name="data">data the image data</param>
    public ImgRaw(int width, int height, int components, int bpc, byte[] data) : base((Uri)null)
    {
        type = IMGRAW;
        scaledHeight = height;
        Top = scaledHeight;
        scaledWidth = width;
        Right = scaledWidth;
        if (components != 1 && components != 3 && components != 4)
        {
            throw new BadElementException("Components must be 1, 3, or 4.");
        }

        if (bpc != 1 && bpc != 2 && bpc != 4 && bpc != 8)
        {
            throw new BadElementException("Bits-per-component must be 1, 2, 4, or 8.");
        }

        colorspace = components;
        this.bpc = bpc;
        rawData = data;
        plainWidth = Width;
        plainHeight = Height;
    }
}