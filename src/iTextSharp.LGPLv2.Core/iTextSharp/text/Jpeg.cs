using System.Text;
using System.util;
using iTextSharp.LGPLv2.Core.System.NetUtils;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     An Jpeg is the representation of a graphic element (JPEG)
///     that has to be inserted into the document
/// </summary>
public class Jpeg : Image
{
    public const int M_APP0 = 0xE0;

    public const int M_APP2 = 0xE2;

    public const int M_APPE = 0xEE;

    /// <summary> This is a type of marker. </summary>
    public const int NOPARAM_MARKER = 2;

    /// <summary> This is a type of marker. </summary>
    public const int NOT_A_MARKER = -1;

    /// <summary> This is a type of marker. </summary>
    public const int UNSUPPORTED_MARKER = 1;

    /// <summary> This is a type of marker. </summary>
    public const int VALID_MARKER = 0;

    public static readonly byte[] JfifId = { 0x4A, 0x46, 0x49, 0x46, 0x00 };

    /// <summary> Jpeg markers without additional parameters. </summary>
    public static readonly int[] NoparamMarkers = { 0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0x01 };

    /// <summary> Unsupported Jpeg markers. </summary>
    public static readonly int[] UnsupportedMarkers =
        { 0xC3, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCD, 0xCE, 0xCF };

    /// <summary> Acceptable Jpeg markers. </summary>
    public static readonly int[] ValidMarkers = { 0xC0, 0xC1, 0xC2 };

    private byte[][] _icc;

    /// <summary>
    ///     Constructors
    /// </summary>
    /// <summary>
    ///     Construct a Jpeg-object, using a Image
    /// </summary>
    /// <param name="image">a Image</param>
    public Jpeg(Image image) : base(image)
    {
    }

    /// <summary>
    ///     Constructs a Jpeg-object, using an Uri.
    /// </summary>
    /// <remarks>
    ///     Deprecated, use Image.GetInstance(...) to create an Image
    /// </remarks>
    /// <param name="uri">the Uri where the image can be found</param>
    public Jpeg(Uri uri) : base(uri)
    {
        processParameters();
    }

    /// <summary>
    ///     Constructs a Jpeg-object from memory.
    /// </summary>
    /// <param name="img">the memory image</param>
    public Jpeg(byte[] img) : base((Uri)null)
    {
        rawData = img;
        originalData = img;
        processParameters();
    }

    /// <summary>
    ///     Constructs a Jpeg-object from memory.
    /// </summary>
    /// <param name="img">the memory image.</param>
    /// <param name="width">the width you want the image to have</param>
    /// <param name="height">the height you want the image to have</param>
    public Jpeg(byte[] img, float width, float height) : this(img)
    {
        scaledWidth = width;
        scaledHeight = height;
    }

    /// <summary>
    ///     private static methods
    /// </summary>
    /// <summary>
    ///     Reads a short from the Stream.
    /// </summary>
    /// <param name="istr">the Stream</param>
    /// <returns>an int</returns>
    private static int getShort(Stream istr) => (istr.ReadByte() << 8) + istr.ReadByte();

    /// <summary>
    ///     Reads an inverted short from the Stream.
    /// </summary>
    /// <param name="istr">the Stream</param>
    /// <returns>an int</returns>
    private static int getShortInverted(Stream istr) => (istr.ReadByte() + istr.ReadByte()) << 8;

    /// <summary>
    ///     Returns a type of marker.
    /// </summary>
    /// <param name="marker">an int</param>
    /// <returns>a type: VALID_MARKER, UNSUPPORTED_MARKER or NOPARAM_MARKER</returns>
    private static int markerType(int marker)
    {
        for (var i = 0; i < ValidMarkers.Length; i++)
        {
            if (marker == ValidMarkers[i])
            {
                return VALID_MARKER;
            }
        }

        for (var i = 0; i < NoparamMarkers.Length; i++)
        {
            if (marker == NoparamMarkers[i])
            {
                return NOPARAM_MARKER;
            }
        }

        for (var i = 0; i < UnsupportedMarkers.Length; i++)
        {
            if (marker == UnsupportedMarkers[i])
            {
                return UNSUPPORTED_MARKER;
            }
        }

        return NOT_A_MARKER;
    }

    /// <summary>
    ///     private methods
    /// </summary>
    /// <summary>
    ///     This method checks if the image is a valid JPEG and processes some parameters.
    /// </summary>
    private void processParameters()
    {
        type = JPEG;
        originalType = ORIGINAL_JPEG;
        Stream istr = null;
        try
        {
            string errorId;
            if (rawData == null)
            {
                istr = url.GetResponseStream();
                errorId = url.ToString();
            }
            else
            {
                istr = new MemoryStream(rawData);
                errorId = "Byte array";
            }

            if (istr.ReadByte() != 0xFF || istr.ReadByte() != 0xD8)
            {
                throw new BadElementException(errorId + " is not a valid JPEG-file.");
            }

            var firstPass = true;
            int len;
            while (true)
            {
                var v = istr.ReadByte();
                if (v < 0)
                {
                    throw new IOException("Premature EOF while reading JPG.");
                }

                if (v == 0xFF)
                {
                    var marker = istr.ReadByte();
                    if (firstPass && marker == M_APP0)
                    {
                        firstPass = false;
                        len = getShort(istr);
                        if (len < 16)
                        {
                            Utilities.Skip(istr, len - 2);
                            continue;
                        }

                        var bcomp = new byte[JfifId.Length];
                        var r = istr.Read(bcomp, 0, bcomp.Length);
                        if (r != bcomp.Length)
                        {
                            throw new BadElementException(errorId + " corrupted JFIF marker.");
                        }

                        var found = true;
                        for (var k = 0; k < bcomp.Length; ++k)
                        {
                            if (bcomp[k] != JfifId[k])
                            {
                                found = false;
                                break;
                            }
                        }

                        if (!found)
                        {
                            Utilities.Skip(istr, len - 2 - bcomp.Length);
                            continue;
                        }

                        Utilities.Skip(istr, 2);
                        var units = istr.ReadByte();
                        var dx = getShort(istr);
                        var dy = getShort(istr);
                        if (units == 1)
                        {
                            dpiX = dx;
                            dpiY = dy;
                        }
                        else if (units == 2)
                        {
                            dpiX = (int)(dx * 2.54f + 0.5f);
                            dpiY = (int)(dy * 2.54f + 0.5f);
                        }

                        Utilities.Skip(istr, len - 2 - bcomp.Length - 7);
                        continue;
                    }

                    if (marker == M_APPE)
                    {
                        len = getShort(istr) - 2;
                        var byteappe = new byte[len];
                        for (var k = 0; k < len; ++k)
                        {
                            byteappe[k] = (byte)istr.ReadByte();
                        }

                        if (byteappe.Length >= 12)
                        {
                            var appe = Encoding.ASCII.GetString(byteappe, 0, 5);
                            if (Util.EqualsIgnoreCase(appe, "adobe"))
                            {
                                Invert = true;
                            }
                        }

                        continue;
                    }

                    if (marker == M_APP2)
                    {
                        len = getShort(istr) - 2;
                        var byteapp2 = new byte[len];
                        for (var k = 0; k < len; ++k)
                        {
                            byteapp2[k] = (byte)istr.ReadByte();
                        }

                        if (byteapp2.Length >= 14)
                        {
                            var app2 = Encoding.ASCII.GetString(byteapp2, 0, 11);
                            if (app2.Equals("ICC_PROFILE", StringComparison.Ordinal))
                            {
                                var order = byteapp2[12] & 0xff;
                                var count = byteapp2[13] & 0xff;
                                if (_icc == null)
                                {
                                    _icc = new byte[count][];
                                }

                                _icc[order - 1] = byteapp2;
                            }
                        }

                        continue;
                    }

                    firstPass = false;
                    var markertype = markerType(marker);
                    if (markertype == VALID_MARKER)
                    {
                        Utilities.Skip(istr, 2);
                        if (istr.ReadByte() != 0x08)
                        {
                            throw new BadElementException(errorId + " must have 8 bits per component.");
                        }

                        scaledHeight = getShort(istr);
                        Top = scaledHeight;
                        scaledWidth = getShort(istr);
                        Right = scaledWidth;
                        colorspace = istr.ReadByte();
                        bpc = 8;
                        break;
                    }
                    else if (markertype == UNSUPPORTED_MARKER)
                    {
                        throw new BadElementException(errorId + ": unsupported JPEG marker: " + marker);
                    }
                    else if (markertype != NOPARAM_MARKER)
                    {
                        Utilities.Skip(istr, getShort(istr) - 2);
                    }
                }
            }
        }
        finally
        {
            if (istr != null)
            {
                istr.Dispose();
            }
        }

        plainWidth = Width;
        plainHeight = Height;
        if (_icc != null)
        {
            var total = 0;
            for (var k = 0; k < _icc.Length; ++k)
            {
                if (_icc[k] == null)
                {
                    _icc = null;
                    return;
                }

                total += _icc[k].Length - 14;
            }

            var ficc = new byte[total];
            total = 0;
            for (var k = 0; k < _icc.Length; ++k)
            {
                Array.Copy(_icc[k], 14, ficc, total, _icc[k].Length - 14);
                total += _icc[k].Length - 14;
            }

            try
            {
                var iccProf = IccProfile.GetInstance(ficc);
                TagIcc = iccProf;
            }
            catch
            {
            }

            _icc = null;
        }
    }
}