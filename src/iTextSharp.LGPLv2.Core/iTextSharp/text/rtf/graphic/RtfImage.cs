using System.util;
using iTextSharp.LGPLv2.Core.System.NetUtils;
using iTextSharp.text.pdf.codec.wmf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.document.output;
using iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf.graphic;

/// <summary>
///     The RtfImage contains one image. Supported image types are jpeg, png, wmf, bmp.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Paulo Soares
/// </summary>
public class RtfImage : RtfElement
{
    /// <summary>
    ///     Constant for converting pixels to twips
    /// </summary>
    private const int PixelTwipsFactor = 15;

    /// <summary>
    ///     lookup table used for converting bytes to hex chars.
    /// </summary>
    public static readonly byte[] Byte2CharLut = new byte[512];

    /// <summary>
    ///     Constant for a picture
    /// </summary>
    private static readonly byte[] _picture = DocWriter.GetIsoBytes("\\pict");

    /// <summary>
    ///     "\bin" constant
    /// </summary>
    private static readonly byte[] _pictureBinaryData = DocWriter.GetIsoBytes("\\bin");

    /// <summary>
    ///     Constant for the shape/picture group
    /// </summary>
    private static readonly byte[] _pictureGroup = DocWriter.GetIsoBytes("\\*\\shppict");

    /// <summary>
    ///     Constant for the picture height
    /// </summary>
    private static readonly byte[] _pictureHeight = DocWriter.GetIsoBytes("\\pich");

    /// <summary>
    ///     Constant for a jpeg image
    /// </summary>
    private static readonly byte[] _pictureJpeg = DocWriter.GetIsoBytes("\\jpegblip");

    /// <summary>
    ///     Constant for a png image
    /// </summary>
    private static readonly byte[] _picturePng = DocWriter.GetIsoBytes("\\pngblip");

    /// <summary>
    ///     Constant for the picture height scale
    /// </summary>
    private static readonly byte[] _pictureScaledHeight = DocWriter.GetIsoBytes("\\pichgoal");

    /// <summary>
    ///     Constant for the picture width scale
    /// </summary>
    private static readonly byte[] _pictureScaledWidth = DocWriter.GetIsoBytes("\\picwgoal");

    /// <summary>
    ///     Constant for horizontal picture scaling
    /// </summary>
    private static readonly byte[] _pictureScaleX = DocWriter.GetIsoBytes("\\picscalex");

    /// <summary>
    ///     Constant for vertical picture scaling
    /// </summary>
    private static readonly byte[] _pictureScaleY = DocWriter.GetIsoBytes("\\picscaley");

    /// <summary>
    ///     Constant for the picture width
    /// </summary>
    private static readonly byte[] _pictureWidth = DocWriter.GetIsoBytes("\\picw");

    /// <summary>
    ///     Constant for a wmf image
    /// </summary>
    private static readonly byte[] _pictureWmf = DocWriter.GetIsoBytes("\\wmetafile8");

    /// <summary>
    ///     The height of this picutre
    /// </summary>
    private readonly float _height;

    /// <summary>
    ///     Binary image data.
    /// </summary>
    private readonly byte[][] _imageData;

    /// <summary>
    ///     The type of image this is.
    /// </summary>
    private readonly int _imageType;

    /// <summary>
    ///     The intended display height of this picture
    /// </summary>
    private readonly float _plainHeight;

    /// <summary>
    ///     The intended display width of this picture
    /// </summary>
    private readonly float _plainWidth;

    /// <summary>
    ///     The width of this picture
    /// </summary>
    private readonly float _width;

    /// <summary>
    ///     The alignment of this picture
    /// </summary>
    private int _alignment = Element.ALIGN_LEFT;

    /// <summary>
    ///     Whether this RtfImage is a top level element and should
    ///     be an extra paragraph.
    /// </summary>
    private bool _topLevelElement;

    //'0001020304050607 ... fafbfcfdfeff'
    static RtfImage()
    {
        var c = '0';
        for (var k = 0; k < 16; k++)
        {
            for (var x = 0; x < 16; x++)
            {
                Byte2CharLut[(k * 16 + x) * 2] = Byte2CharLut[(x * 16 + k) * 2 + 1] = (byte)c;
            }

            if (++c == ':')
            {
                c = 'a';
            }
        }
    }

    /// <summary>
    ///     Constructs a RtfImage for an Image.
    ///     @throws DocumentException If an error occured accessing the image content
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfImage belongs to</param>
    /// <param name="image">The Image that this RtfImage wraps</param>
    public RtfImage(RtfDocument doc, Image image) : base(doc)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        _imageType = image.OriginalType;
        if (!(_imageType == Image.ORIGINAL_JPEG || _imageType == Image.ORIGINAL_BMP
                                                || _imageType == Image.ORIGINAL_PNG ||
                                                _imageType == Image.ORIGINAL_WMF || _imageType == Image.ORIGINAL_GIF))
        {
            throw new DocumentException("Only BMP, PNG, WMF, GIF and JPEG images are supported by the RTF Writer");
        }

        _alignment = image.Alignment;
        _width = image.Width;
        _height = image.Height;
        _plainWidth = image.PlainWidth;
        _plainHeight = image.PlainHeight;
        _imageData = getImageData(image);
    }

    /// <summary>
    ///     Sets the alignment of this RtfImage. Uses the alignments from com.lowagie.text.Element.
    /// </summary>
    /// <param name="alignment">The alignment to use.</param>
    public void SetAlignment(int alignment)
    {
        _alignment = alignment;
    }

    /// <summary>
    ///     Set whether this RtfImage should behave like a top level element
    ///     and enclose itself in a paragraph.
    /// </summary>
    /// <param name="topLevelElement">Whether to behave like a top level element.</param>
    public void SetTopLevelElement(bool topLevelElement)
    {
        _topLevelElement = topLevelElement;
    }

    /// <summary>
    ///     Writes the RtfImage content
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        if (_topLevelElement)
        {
            outp.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
            switch (_alignment)
            {
                case Element.ALIGN_LEFT:
                    outp.Write(RtfParagraphStyle.AlignLeft, 0, RtfParagraphStyle.AlignLeft.Length);
                    break;
                case Element.ALIGN_RIGHT:
                    outp.Write(RtfParagraphStyle.AlignRight, 0, RtfParagraphStyle.AlignRight.Length);
                    break;
                case Element.ALIGN_CENTER:
                    outp.Write(RtfParagraphStyle.AlignCenter, 0, RtfParagraphStyle.AlignCenter.Length);
                    break;
                case Element.ALIGN_JUSTIFIED:
                    outp.Write(RtfParagraphStyle.AlignJustify, 0, RtfParagraphStyle.AlignJustify.Length);
                    break;
            }
        }

        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_pictureGroup, 0, _pictureGroup.Length);
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_picture, 0, _picture.Length);
        switch (_imageType)
        {
            case Image.ORIGINAL_JPEG:
                outp.Write(_pictureJpeg, 0, _pictureJpeg.Length);
                break;
            case Image.ORIGINAL_PNG:
            case Image.ORIGINAL_GIF:
                outp.Write(_picturePng, 0, _picturePng.Length);
                break;
            case Image.ORIGINAL_WMF:
            case Image.ORIGINAL_BMP:
                outp.Write(_pictureWmf, 0, _pictureWmf.Length);
                break;
        }

        outp.Write(_pictureWidth, 0, _pictureWidth.Length);
        outp.Write(t = IntToByteArray((int)_width), 0, t.Length);
        outp.Write(_pictureHeight, 0, _pictureHeight.Length);
        outp.Write(t = IntToByteArray((int)_height), 0, t.Length);
        if (Document.GetDocumentSettings().IsWriteImageScalingInformation())
        {
            outp.Write(_pictureScaleX, 0, _pictureScaleX.Length);
            outp.Write(t = IntToByteArray((int)(100 * _plainWidth / _width)), 0, t.Length);
            outp.Write(_pictureScaleY, 0, _pictureScaleY.Length);
            outp.Write(t = IntToByteArray((int)(100 * _plainHeight / _height)), 0, t.Length);
        }

        if (Document.GetDocumentSettings().IsImagePdfConformance())
        {
            outp.Write(_pictureScaledWidth, 0, _pictureScaledWidth.Length);
            outp.Write(t = IntToByteArray((int)(_plainWidth * TWIPS_FACTOR)), 0, t.Length);
            outp.Write(_pictureScaledHeight, 0, _pictureScaledHeight.Length);
            outp.Write(t = IntToByteArray((int)(_plainHeight * TWIPS_FACTOR)), 0, t.Length);
        }
        else
        {
            if (_width.ApproxNotEqual(_plainWidth) || _imageType == Image.ORIGINAL_BMP)
            {
                outp.Write(_pictureScaledWidth, 0, _pictureScaledWidth.Length);
                outp.Write(t = IntToByteArray((int)(_plainWidth * PixelTwipsFactor)), 0, t.Length);
            }

            if (_height.ApproxNotEqual(_plainHeight) || _imageType == Image.ORIGINAL_BMP)
            {
                outp.Write(_pictureScaledHeight, 0, _pictureScaledHeight.Length);
                outp.Write(t = IntToByteArray((int)(_plainHeight * PixelTwipsFactor)), 0, t.Length);
            }
        }

        if (Document.GetDocumentSettings().IsImageWrittenAsBinary())
        {
            //binary
            outp.WriteByte((byte)'\n');
            outp.Write(_pictureBinaryData, 0, _pictureBinaryData.Length);
            outp.Write(t = IntToByteArray(imageDataSize()), 0, t.Length);
            outp.Write(Delimiter, 0, Delimiter.Length);
            if (outp is RtfByteArrayBuffer)
            {
                ((RtfByteArrayBuffer)outp).Append(_imageData);
            }
            else
            {
                for (var k = 0; k < _imageData.Length; k++)
                {
                    outp.Write(_imageData[k], 0, _imageData[k].Length);
                }
            }
        }
        else
        {
            //hex encoded
            outp.Write(Delimiter, 0, Delimiter.Length);
            outp.WriteByte((byte)'\n');
            writeImageDataHexEncoded(outp);
        }

        outp.Write(CloseGroup, 0, CloseGroup.Length);
        outp.Write(CloseGroup, 0, CloseGroup.Length);
        if (_topLevelElement)
        {
            outp.Write(RtfParagraph.Paragraph, 0, RtfParagraph.Paragraph.Length);
        }

        outp.WriteByte((byte)'\n');
    }

    /// <summary>
    ///     Extracts the image data from the Image.
    ///     @throws DocumentException If an error occurs accessing the image content
    /// </summary>
    /// <param name="image">The image for which to extract the content</param>
    /// <returns>The raw image data, not formated</returns>
    private byte[][] getImageData(Image image)
    {
        var wmfPlaceableHeaderSize = 22;
        var bab = new RtfByteArrayBuffer();

        try
        {
            if (_imageType == Image.ORIGINAL_BMP)
            {
                bab.Append(MetaDo.WrapBmp(image));
            }
            else
            {
                var iod = image.OriginalData;
                if (iod == null)
                {
                    var imageIn = image.Url.GetResponseStream();
                    if (_imageType == Image.ORIGINAL_WMF)
                    {
                        //remove the placeable header first
                        for (var k = 0; k < wmfPlaceableHeaderSize; k++)
                        {
                            if (imageIn.ReadByte() < 0)
                            {
                                throw new IOException("while removing wmf placeable header");
                            }
                        }
                    }

                    bab.Write(imageIn);
                    imageIn.Dispose();
                }
                else
                {
                    if (_imageType == Image.ORIGINAL_WMF)
                    {
                        //remove the placeable header
                        bab.Write(iod, wmfPlaceableHeaderSize, iod.Length - wmfPlaceableHeaderSize);
                    }
                    else
                    {
                        bab.Append(iod);
                    }
                }
            }

            return bab.ToArrayArray();
        }
        catch (IOException ioe)
        {
            throw new DocumentException(ioe.Message);
        }
    }

    /// <summary>
    ///     Returns the image raw data size in bytes.
    /// </summary>
    /// <returns></returns>
    private int imageDataSize()
    {
        var size = 0;
        for (var k = 0; k < _imageData.Length; k++)
        {
            size += _imageData[k].Length;
        }

        return size;
    }

    /// <summary>
    ///     Writes the image data to the given buffer as hex encoded text.
    ///     @
    /// </summary>
    private void writeImageDataHexEncoded(Stream bab)
    {
        var cnt = 0;
        for (var k = 0; k < _imageData.Length; k++)
        {
            var chunk = _imageData[k];
            for (var x = 0; x < chunk.Length; x++)
            {
                bab.Write(Byte2CharLut, (chunk[x] & 0xff) * 2, 2);
                if (++cnt == 64)
                {
                    bab.WriteByte((byte)'\n');
                    cnt = 0;
                }
            }
        }

        if (cnt > 0)
        {
            bab.WriteByte((byte)'\n');
        }
    }
}