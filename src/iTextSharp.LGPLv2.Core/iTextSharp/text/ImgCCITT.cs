using iTextSharp.text.pdf.codec;

namespace iTextSharp.text;

/// <summary>
///     CCITT Image data that has to be inserted into the document
///     @see        Element
///     @see        Image
///     @author  Paulo Soares
/// </summary>
/// <summary>
///     CCITT Image data that has to be inserted into the document
/// </summary>
public class ImgCcitt : Image
{
    public ImgCcitt(Image image) : base(image)
    {
    }

    /// <summary>
    ///     Creats an Image in CCITT mode.
    /// </summary>
    /// <param name="width">the exact width of the image</param>
    /// <param name="height">the exact height of the image</param>
    /// <param name="reverseBits">
    ///     reverses the bits in data.
    ///     Bit 0 is swapped with bit 7 and so on
    /// </param>
    /// <param name="typeCcitt">
    ///     the type of compression in data. It can be
    ///     CCITTG4, CCITTG31D, CCITTG32D
    /// </param>
    /// <param name="parameters">
    ///     parameters associated with this stream. Possible values are
    ///     CCITT_BLACKIS1, CCITT_ENCODEDBYTEALIGN, CCITT_ENDOFLINE and CCITT_ENDOFBLOCK or a
    ///     combination of them
    /// </param>
    /// <param name="data">the image data</param>
    public ImgCcitt(int width, int height, bool reverseBits, int typeCcitt, int parameters, byte[] data) :
        base((Uri)null)
    {
        if (typeCcitt != CCITTG4 && typeCcitt != CCITTG3_1D && typeCcitt != CCITTG3_2D)
        {
            throw new BadElementException("The CCITT compression type must be CCITTG4, CCITTG3_1D or CCITTG3_2D");
        }

        if (reverseBits)
        {
            TiffFaxDecoder.ReverseBits(data);
        }

        type = IMGRAW;
        scaledHeight = height;
        Top = scaledHeight;
        scaledWidth = width;
        Right = scaledWidth;
        colorspace = parameters;
        bpc = typeCcitt;
        rawData = data;
        plainWidth = Width;
        plainHeight = Height;
    }
}