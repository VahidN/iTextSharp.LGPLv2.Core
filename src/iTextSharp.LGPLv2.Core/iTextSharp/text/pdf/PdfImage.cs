using iTextSharp.LGPLv2.Core.System.NetUtils;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfImage  is a  PdfStream  containing an image- Dictionary  and -stream.
/// </summary>
public class PdfImage : PdfStream
{
    internal const int TRANSFERSIZE = 4096;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     This is the  PdfName  of the image.
    /// </summary>
    protected PdfName name;

    /// <summary>
    ///     constructor
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfImage -object.
    ///     @throws BadPdfFormatException on error
    /// </summary>
    public PdfImage(Image image, string name, PdfIndirectReference maskRef)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        this.name = new PdfName(name);
        Put(PdfName.TYPE, PdfName.Xobject);
        Put(PdfName.Subtype, PdfName.Image);
        Put(PdfName.Width, new PdfNumber(image.Width));
        Put(PdfName.Height, new PdfNumber(image.Height));
        if (image.Layer != null)
        {
            Put(PdfName.Oc, image.Layer.Ref);
        }

        if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 0xff))
        {
            Put(PdfName.Imagemask, PdfBoolean.Pdftrue);
        }

        if (maskRef != null)
        {
            if (image.Smask)
            {
                Put(PdfName.Smask, maskRef);
            }
            else
            {
                Put(PdfName.Mask, maskRef);
            }
        }

        if (image.IsMask() && image.Inverted)
        {
            Put(PdfName.Decode, new PdfLiteral("[1 0]"));
        }

        if (image.Interpolation)
        {
            Put(PdfName.Interpolate, PdfBoolean.Pdftrue);
        }

        Stream isp = null;
        try
        {
            // Raw Image data
            if (image.IsImgRaw())
            {
                // will also have the CCITT parameters
                var colorspace = image.Colorspace;
                var transparency = image.Transparency;
                if (transparency != null && !image.IsMask() && maskRef == null)
                {
                    var s = "[";
                    for (var k = 0; k < transparency.Length; ++k)
                    {
                        s += transparency[k] + " ";
                    }

                    s += "]";
                    Put(PdfName.Mask, new PdfLiteral(s));
                }

                Bytes = image.RawData;
                Put(PdfName.LENGTH, new PdfNumber(Bytes.Length));
                var bpc = image.Bpc;
                if (bpc > 0xff)
                {
                    if (!image.IsMask())
                    {
                        Put(PdfName.Colorspace, PdfName.Devicegray);
                    }

                    Put(PdfName.Bitspercomponent, new PdfNumber(1));
                    Put(PdfName.Filter, PdfName.Ccittfaxdecode);
                    var k = bpc - Element.CCITTG3_1D;
                    var decodeparms = new PdfDictionary();
                    if (k != 0)
                    {
                        decodeparms.Put(PdfName.K, new PdfNumber(k));
                    }

                    if ((colorspace & Element.CCITT_BLACKIS1) != 0)
                    {
                        decodeparms.Put(PdfName.Blackis1, PdfBoolean.Pdftrue);
                    }

                    if ((colorspace & Element.CCITT_ENCODEDBYTEALIGN) != 0)
                    {
                        decodeparms.Put(PdfName.Encodedbytealign, PdfBoolean.Pdftrue);
                    }

                    if ((colorspace & Element.CCITT_ENDOFLINE) != 0)
                    {
                        decodeparms.Put(PdfName.Endofline, PdfBoolean.Pdftrue);
                    }

                    if ((colorspace & Element.CCITT_ENDOFBLOCK) != 0)
                    {
                        decodeparms.Put(PdfName.Endofblock, PdfBoolean.Pdffalse);
                    }

                    decodeparms.Put(PdfName.Columns, new PdfNumber(image.Width));
                    decodeparms.Put(PdfName.Rows, new PdfNumber(image.Height));
                    Put(PdfName.Decodeparms, decodeparms);
                }
                else
                {
                    switch (colorspace)
                    {
                        case 1:
                            Put(PdfName.Colorspace, PdfName.Devicegray);
                            if (image.Inverted)
                            {
                                Put(PdfName.Decode, new PdfLiteral("[1 0]"));
                            }

                            break;
                        case 3:
                            Put(PdfName.Colorspace, PdfName.Devicergb);
                            if (image.Inverted)
                            {
                                Put(PdfName.Decode, new PdfLiteral("[1 0 1 0 1 0]"));
                            }

                            break;
                        case 4:
                        default:
                            Put(PdfName.Colorspace, PdfName.Devicecmyk);
                            if (image.Inverted)
                            {
                                Put(PdfName.Decode, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                            }

                            break;
                    }

                    var additional = image.Additional;
                    if (additional != null)
                    {
                        Merge(additional);
                    }

                    if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 8))
                    {
                        Remove(PdfName.Colorspace);
                    }

                    Put(PdfName.Bitspercomponent, new PdfNumber(image.Bpc));
                    if (image.Deflated)
                    {
                        Put(PdfName.Filter, PdfName.Flatedecode);
                    }
                    else
                    {
                        FlateCompress(image.CompressionLevel);
                    }
                }

                return;
            }

            // GIF, JPEG or PNG
            string errorId;
            if (image.RawData == null)
            {
                isp = image.Url.GetResponseStream();
                errorId = image.Url.ToString();
            }
            else
            {
                isp = new MemoryStream(image.RawData);
                errorId = "Byte array";
            }

            switch (image.Type)
            {
                case Element.JPEG:
                    Put(PdfName.Filter, PdfName.Dctdecode);
                    switch (image.Colorspace)
                    {
                        case 1:
                            Put(PdfName.Colorspace, PdfName.Devicegray);
                            break;
                        case 3:
                            Put(PdfName.Colorspace, PdfName.Devicergb);
                            break;
                        default:
                            Put(PdfName.Colorspace, PdfName.Devicecmyk);
                            if (image.Inverted)
                            {
                                Put(PdfName.Decode, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                            }

                            break;
                    }

                    Put(PdfName.Bitspercomponent, new PdfNumber(8));
                    if (image.RawData != null)
                    {
                        Bytes = image.RawData;
                        Put(PdfName.LENGTH, new PdfNumber(Bytes.Length));
                        return;
                    }

                    StreamBytes = new MemoryStream();
                    TransferBytes(isp, StreamBytes, -1);
                    break;
                case Element.JPEG2000:
                    Put(PdfName.Filter, PdfName.Jpxdecode);
                    if (image.Colorspace > 0)
                    {
                        switch (image.Colorspace)
                        {
                            case 1:
                                Put(PdfName.Colorspace, PdfName.Devicegray);
                                break;
                            case 3:
                                Put(PdfName.Colorspace, PdfName.Devicergb);
                                break;
                            default:
                                Put(PdfName.Colorspace, PdfName.Devicecmyk);
                                break;
                        }

                        Put(PdfName.Bitspercomponent, new PdfNumber(image.Bpc));
                    }

                    if (image.RawData != null)
                    {
                        Bytes = image.RawData;
                        Put(PdfName.LENGTH, new PdfNumber(Bytes.Length));
                        return;
                    }

                    StreamBytes = new MemoryStream();
                    TransferBytes(isp, StreamBytes, -1);
                    break;
                case Element.JBIG2:
                    Put(PdfName.Filter, PdfName.Jbig2Decode);
                    Put(PdfName.Colorspace, PdfName.Devicegray);
                    Put(PdfName.Bitspercomponent, new PdfNumber(1));
                    if (image.RawData != null)
                    {
                        Bytes = image.RawData;
                        Put(PdfName.LENGTH, new PdfNumber(Bytes.Length));
                        return;
                    }

                    StreamBytes = new MemoryStream();
                    TransferBytes(isp, StreamBytes, -1);
                    break;
                default:
                    throw new IOException(errorId + " is an unknown Image format.");
            }

            Put(PdfName.LENGTH, new PdfNumber(StreamBytes.Length));
        }
        finally
        {
            if (isp != null)
            {
                try
                {
                    isp.Dispose();
                }
                catch
                {
                    // empty on purpose
                }
            }
        }
    }

    /// <summary>
    ///     Returns the  PdfName  of the image.
    /// </summary>
    /// <returns>the name</returns>

    public PdfName Name => name;

    internal static void TransferBytes(Stream inp, Stream outp, int len)
    {
        var buffer = new byte[TRANSFERSIZE];
        if (len < 0)
        {
            len = 0x7ffffff;
        }

        int size;
        while (len != 0)
        {
            size = inp.Read(buffer, 0, Math.Min(len, TRANSFERSIZE));
            if (size <= 0)
            {
                return;
            }

            outp.Write(buffer, 0, size);
            len -= size;
        }
    }

    protected void ImportAll(PdfImage dup)
    {
        if (dup == null)
        {
            throw new ArgumentNullException(nameof(dup));
        }

        name = dup.name;
        Compressed = dup.Compressed;
        CompressionLevel = dup.CompressionLevel;
        StreamBytes = dup.StreamBytes;
        Bytes = dup.Bytes;
        HashMap = dup.HashMap;
    }
}