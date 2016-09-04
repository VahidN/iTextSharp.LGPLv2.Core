using System.IO;
using iTextSharp.LGPLv2.Core.System.NetUtils;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.document.output;
using iTextSharp.text.rtf.text;
using iTextSharp.text.rtf.style;
using iTextSharp.text.pdf.codec.wmf;

namespace iTextSharp.text.rtf.graphic
{
    /// <summary>
    /// The RtfImage contains one image. Supported image types are jpeg, png, wmf, bmp.
    /// @version $Version:$
    /// @author Mark Hall (Mark.Hall@mail.room3b.eu)
    /// @author Paulo Soares
    /// </summary>
    public class RtfImage : RtfElement
    {

        /// <summary>
        /// lookup table used for converting bytes to hex chars.
        /// </summary>
        public static readonly byte[] Byte2CharLut = new byte[512];

        /// <summary>
        /// Constant for converting pixels to twips
        /// </summary>
        private const int PixelTwipsFactor = 15;

        /// <summary>
        /// Constant for a picture
        /// </summary>
        private static readonly byte[] _picture = DocWriter.GetIsoBytes("\\pict");

        /// <summary>
        /// "\bin" constant
        /// </summary>
        private static readonly byte[] _pictureBinaryData = DocWriter.GetIsoBytes("\\bin");

        /// <summary>
        /// Constant for the shape/picture group
        /// </summary>
        private static readonly byte[] _pictureGroup = DocWriter.GetIsoBytes("\\*\\shppict");
        /// <summary>
        /// Constant for the picture height
        /// </summary>
        private static readonly byte[] _pictureHeight = DocWriter.GetIsoBytes("\\pich");

        /// <summary>
        /// Constant for a jpeg image
        /// </summary>
        private static readonly byte[] _pictureJpeg = DocWriter.GetIsoBytes("\\jpegblip");
        /// <summary>
        /// Constant for a png image
        /// </summary>
        private static readonly byte[] _picturePng = DocWriter.GetIsoBytes("\\pngblip");
        /// <summary>
        /// Constant for the picture height scale
        /// </summary>
        private static readonly byte[] _pictureScaledHeight = DocWriter.GetIsoBytes("\\pichgoal");

        /// <summary>
        /// Constant for the picture width scale
        /// </summary>
        private static readonly byte[] _pictureScaledWidth = DocWriter.GetIsoBytes("\\picwgoal");

        /// <summary>
        /// Constant for horizontal picture scaling
        /// </summary>
        private static readonly byte[] _pictureScaleX = DocWriter.GetIsoBytes("\\picscalex");

        /// <summary>
        /// Constant for vertical picture scaling
        /// </summary>
        private static readonly byte[] _pictureScaleY = DocWriter.GetIsoBytes("\\picscaley");

        /// <summary>
        /// Constant for the picture width
        /// </summary>
        private static readonly byte[] _pictureWidth = DocWriter.GetIsoBytes("\\picw");

        /// <summary>
        /// Constant for a wmf image
        /// </summary>
        private static readonly byte[] _pictureWmf = DocWriter.GetIsoBytes("\\wmetafile8");
        /// <summary>
        /// The height of this picutre
        /// </summary>
        private readonly float _height;

        /// <summary>
        /// Binary image data.
        /// </summary>
        private readonly byte[][] _imageData;

        /// <summary>
        /// The type of image this is.
        /// </summary>
        private readonly int _imageType;
        /// <summary>
        /// The intended display height of this picture
        /// </summary>
        private readonly float _plainHeight;

        /// <summary>
        /// The intended display width of this picture
        /// </summary>
        private readonly float _plainWidth;

        /// <summary>
        /// The width of this picture
        /// </summary>
        private readonly float _width;

        /// <summary>
        /// The alignment of this picture
        /// </summary>
        private int _alignment = Element.ALIGN_LEFT;
        /// <summary>
        /// Whether this RtfImage is a top level element and should
        /// be an extra paragraph.
        /// </summary>
        private bool _topLevelElement;

        //'0001020304050607 ... fafbfcfdfeff'
        static RtfImage()
        {
            char c = '0';
            for (int k = 0; k < 16; k++)
            {
                for (int x = 0; x < 16; x++)
                {
                    Byte2CharLut[((k * 16) + x) * 2] = Byte2CharLut[(((x * 16) + k) * 2) + 1] = (byte)c;
                }
                if (++c == ':') c = 'a';
            }
        }

        /// <summary>
        /// Constructs a RtfImage for an Image.
        /// @throws DocumentException If an error occured accessing the image content
        /// </summary>
        /// <param name="doc">The RtfDocument this RtfImage belongs to</param>
        /// <param name="image">The Image that this RtfImage wraps</param>
        public RtfImage(RtfDocument doc, Image image) : base(doc)
        {
            _imageType = image.OriginalType;
            if (!(_imageType == Image.ORIGINAL_JPEG || _imageType == Image.ORIGINAL_BMP
                    || _imageType == Image.ORIGINAL_PNG || _imageType == Image.ORIGINAL_WMF || _imageType == Image.ORIGINAL_GIF))
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
        /// Sets the alignment of this RtfImage. Uses the alignments from com.lowagie.text.Element.
        /// </summary>
        /// <param name="alignment">The alignment to use.</param>
        public void SetAlignment(int alignment)
        {
            _alignment = alignment;
        }

        /// <summary>
        /// Set whether this RtfImage should behave like a top level element
        /// and enclose itself in a paragraph.
        /// </summary>
        /// <param name="topLevelElement">Whether to behave like a top level element.</param>
        public void SetTopLevelElement(bool topLevelElement)
        {
            _topLevelElement = topLevelElement;
        }

        /// <summary>
        /// Writes the RtfImage content
        /// </summary>
        public override void WriteContent(Stream result)
        {
            byte[] t;
            if (_topLevelElement)
            {
                result.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
                switch (_alignment)
                {
                    case Element.ALIGN_LEFT:
                        result.Write(RtfParagraphStyle.AlignLeft, 0, RtfParagraphStyle.AlignLeft.Length);
                        break;
                    case Element.ALIGN_RIGHT:
                        result.Write(RtfParagraphStyle.AlignRight, 0, RtfParagraphStyle.AlignRight.Length);
                        break;
                    case Element.ALIGN_CENTER:
                        result.Write(RtfParagraphStyle.AlignCenter, 0, RtfParagraphStyle.AlignCenter.Length);
                        break;
                    case Element.ALIGN_JUSTIFIED:
                        result.Write(RtfParagraphStyle.AlignJustify, 0, RtfParagraphStyle.AlignJustify.Length);
                        break;
                }
            }
            result.Write(OpenGroup, 0, OpenGroup.Length);
            result.Write(_pictureGroup, 0, _pictureGroup.Length);
            result.Write(OpenGroup, 0, OpenGroup.Length);
            result.Write(_picture, 0, _picture.Length);
            switch (_imageType)
            {
                case Image.ORIGINAL_JPEG:
                    result.Write(_pictureJpeg, 0, _pictureJpeg.Length);
                    break;
                case Image.ORIGINAL_PNG:
                case Image.ORIGINAL_GIF:
                    result.Write(_picturePng, 0, _picturePng.Length);
                    break;
                case Image.ORIGINAL_WMF:
                case Image.ORIGINAL_BMP:
                    result.Write(_pictureWmf, 0, _pictureWmf.Length);
                    break;
            }
            result.Write(_pictureWidth, 0, _pictureWidth.Length);
            result.Write(t = IntToByteArray((int)_width), 0, t.Length);
            result.Write(_pictureHeight, 0, _pictureHeight.Length);
            result.Write(t = IntToByteArray((int)_height), 0, t.Length);
            if (Document.GetDocumentSettings().IsWriteImageScalingInformation())
            {
                result.Write(_pictureScaleX, 0, _pictureScaleX.Length);
                result.Write(t = IntToByteArray((int)(100 * _plainWidth / _width)), 0, t.Length);
                result.Write(_pictureScaleY, 0, _pictureScaleY.Length);
                result.Write(t = IntToByteArray((int)(100 * _plainHeight / _height)), 0, t.Length);
            }
            if (Document.GetDocumentSettings().IsImagePdfConformance())
            {
                result.Write(_pictureScaledWidth, 0, _pictureScaledWidth.Length);
                result.Write(t = IntToByteArray((int)(_plainWidth * TWIPS_FACTOR)), 0, t.Length);
                result.Write(_pictureScaledHeight, 0, _pictureScaledHeight.Length);
                result.Write(t = IntToByteArray((int)(_plainHeight * TWIPS_FACTOR)), 0, t.Length);
            }
            else
            {
                if (_width != _plainWidth || _imageType == Image.ORIGINAL_BMP)
                {
                    result.Write(_pictureScaledWidth, 0, _pictureScaledWidth.Length);
                    result.Write(t = IntToByteArray((int)(_plainWidth * PixelTwipsFactor)), 0, t.Length);
                }
                if (_height != _plainHeight || _imageType == Image.ORIGINAL_BMP)
                {
                    result.Write(_pictureScaledHeight, 0, _pictureScaledHeight.Length);
                    result.Write(t = IntToByteArray((int)(_plainHeight * PixelTwipsFactor)), 0, t.Length);
                }
            }

            if (Document.GetDocumentSettings().IsImageWrittenAsBinary())
            {
                //binary
                result.WriteByte((byte)'\n');
                result.Write(_pictureBinaryData, 0, _pictureBinaryData.Length);
                result.Write(t = IntToByteArray(imageDataSize()), 0, t.Length);
                result.Write(Delimiter, 0, Delimiter.Length);
                if (result is RtfByteArrayBuffer)
                {
                    ((RtfByteArrayBuffer)result).Append(_imageData);
                }
                else
                {
                    for (int k = 0; k < _imageData.Length; k++)
                    {
                        result.Write(_imageData[k], 0, _imageData[k].Length);
                    }
                }
            }
            else
            {
                //hex encoded
                result.Write(Delimiter, 0, Delimiter.Length);
                result.WriteByte((byte)'\n');
                writeImageDataHexEncoded(result);
            }
            result.Write(CloseGroup, 0, CloseGroup.Length);
            result.Write(CloseGroup, 0, CloseGroup.Length);
            if (_topLevelElement)
            {
                result.Write(RtfParagraph.Paragraph, 0, RtfParagraph.Paragraph.Length);
            }
            result.WriteByte((byte)'\n');
        }

        /// <summary>
        /// Extracts the image data from the Image.
        /// @throws DocumentException If an error occurs accessing the image content
        /// </summary>
        /// <param name="image">The image for which to extract the content</param>
        /// <returns>The raw image data, not formated</returns>
        private byte[][] getImageData(Image image)
        {
            int wmfPlaceableHeaderSize = 22;
            RtfByteArrayBuffer bab = new RtfByteArrayBuffer();

            try
            {
                if (_imageType == Image.ORIGINAL_BMP)
                {
                    bab.Append(MetaDo.WrapBmp(image));
                }
                else
                {
                    byte[] iod = image.OriginalData;
                    if (iod == null)
                    {
                        Stream imageIn = image.Url.GetResponseStream();
                        if (_imageType == Image.ORIGINAL_WMF)
                        { //remove the placeable header first
                            for (int k = 0; k < wmfPlaceableHeaderSize; k++)
                            {
                                if (imageIn.ReadByte() < 0) throw (new IOException("while removing wmf placeable header"));
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
        /// Returns the image raw data size in bytes.
        /// </summary>
        /// <returns></returns>
        private int imageDataSize()
        {
            int size = 0;
            for (int k = 0; k < _imageData.Length; k++)
            {
                size += _imageData[k].Length;
            }
            return size;
        }

        /// <summary>
        /// Writes the image data to the given buffer as hex encoded text.
        /// @
        /// </summary>
        private void writeImageDataHexEncoded(Stream bab)
        {
            int cnt = 0;
            for (int k = 0; k < _imageData.Length; k++)
            {
                byte[] chunk = _imageData[k];
                for (int x = 0; x < chunk.Length; x++)
                {
                    bab.Write(Byte2CharLut, (chunk[x] & 0xff) * 2, 2);
                    if (++cnt == 64)
                    {
                        bab.WriteByte((byte)'\n');
                        cnt = 0;
                    }
                }
            }
            if (cnt > 0) bab.WriteByte((byte)'\n');
        }
    }
}