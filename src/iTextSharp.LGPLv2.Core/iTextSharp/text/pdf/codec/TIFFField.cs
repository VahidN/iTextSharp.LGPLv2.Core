namespace iTextSharp.text.pdf.codec;

/// <summary>
///     A class representing a field in a TIFF 6.0 Image File Directory.
///     The TIFF file format is described in more detail in the
///     comments for the TIFFDescriptor class.
///     A field in a TIFF Image File Directory (IFD).  A field is defined
///     as a sequence of values of identical data type.  TIFF 6.0 defines
///     12 data types, which are mapped internally onto the Java datatypes
///     byte, int, long, float, and double.
///     <b>
///         This class is not a committed part of the JAI API.  It may
///         be removed or changed in future releases of JAI.
///     </b>
///     @see TIFFDirectory
/// </summary>
public class TiffField : IComparable
{
    /// <summary>
    ///     Flag for null-terminated ASCII strings.
    /// </summary>
    public const int TIFF_ASCII = 2;

    /// <summary>
    ///     Flag for 8 bit unsigned integers.
    /// </summary>
    public const int TIFF_BYTE = 1;

    /// <summary>
    ///     Flag for 64 bit IEEE doubles.
    /// </summary>
    public const int TIFF_DOUBLE = 12;

    /// <summary>
    ///     Flag for 32 bit IEEE floats.
    /// </summary>
    public const int TIFF_FLOAT = 11;

    /// <summary>
    ///     Flag for 32 bit unsigned integers.
    /// </summary>
    public const int TIFF_LONG = 4;

    /// <summary>
    ///     Flag for pairs of 32 bit unsigned integers.
    /// </summary>
    public const int TIFF_RATIONAL = 5;

    /// <summary>
    ///     Flag for 8 bit signed integers.
    /// </summary>
    public const int TIFF_SBYTE = 6;

    /// <summary>
    ///     Flag for 16 bit unsigned integers.
    /// </summary>
    public const int TIFF_SHORT = 3;

    /// <summary>
    ///     Flag for 32 bit signed integers.
    /// </summary>
    public const int TIFF_SLONG = 9;

    /// <summary>
    ///     Flag for pairs of 32 bit signed integers.
    /// </summary>
    public const int TIFF_SRATIONAL = 10;

    /// <summary>
    ///     Flag for 16 bit signed integers.
    /// </summary>
    public const int TIFF_SSHORT = 8;

    /// <summary>
    ///     Flag for 8 bit uninterpreted bytes.
    /// </summary>
    public const int TIFF_UNDEFINED = 7;

    /// <summary>
    ///     The number of data items present in the field.
    /// </summary>
    private readonly int _count;

    /// <summary>
    ///     The field data.
    /// </summary>
    private readonly object _data;

    /// <summary>
    ///     The tag number.
    /// </summary>
    private readonly int _tag;

    /// <summary>
    ///     The tag type.
    /// </summary>
    private readonly int _type;

    /// <summary>
    ///     Constructs a TIFFField with arbitrary data.  The data
    ///     parameter must be an array of a Java type appropriate for the
    ///     type of the TIFF field.  Since there is no available 32-bit
    ///     unsigned datatype, long is used. The mapping between types is
    ///     as follows:
    ///     TIFF type     Java type
    ///     TIFF_BYTE          byte
    ///     TIFF_ASCII         String
    ///     TIFF_SHORT         char
    ///     TIFF_LONG          long
    ///     TIFF_RATIONAL      long[2]
    ///     TIFF_SBYTE         byte
    ///     TIFF_UNDEFINED     byte
    ///     TIFF_SSHORT        short
    ///     TIFF_SLONG         int
    ///     TIFF_SRATIONAL     int[2]
    ///     TIFF_FLOAT         float
    ///     TIFF_DOUBLE        double
    /// </summary>
    public TiffField(int tag, int type, int count, object data)
    {
        _tag = tag;
        _type = type;
        _count = count;
        _data = data;
    }

    /// <summary>
    ///     The default constructor.
    /// </summary>
    internal TiffField()
    {
    }

    /// <summary>
    ///     Compares this  TIFFField  with another
    ///     TIFFField  by comparing the tags.
    ///     <b>
    ///         Note: this class has a natural ordering that is inconsistent
    ///         with  equals() .
    ///     </b>
    ///     @throws IllegalArgumentException if the parameter is  null .
    ///     @throws ClassCastException if the parameter is not a
    ///     TIFFField .
    /// </summary>
    public int CompareTo(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var oTag = ((TiffField)obj).GetTag();

        if (_tag < oTag)
        {
            return -1;
        }

        if (_tag > oTag)
        {
            return 1;
        }

        return 0;
    }

    /// <summary>
    ///     Returns the data as an uninterpreted array of bytes.
    ///     The type of the field must be one of TIFF_BYTE, TIFF_SBYTE,
    ///     or TIFF_UNDEFINED;
    ///     For data in TIFF_BYTE format, the application must take
    ///     care when promoting the data to longer integral types
    ///     to avoid sign extension.
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_BYTE, TIFF_SBYTE, or TIFF_UNDEFINED.
    /// </summary>
    public byte[] GetAsBytes() => (byte[])_data;

    /// <summary>
    ///     Returns TIFF_SHORT data as an array of chars (unsigned 16-bit
    ///     integers).
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_SHORT.
    /// </summary>
    public char[] GetAsChars() => (char[])_data;

    /// <summary>
    ///     Returns data in any numerical format as a float.  Data in
    ///     TIFF_SRATIONAL or TIFF_RATIONAL format are evaluated by
    ///     dividing the numerator into the denominator using
    ///     double-precision arithmetic.
    ///     A ClassCastException will be thrown if the field is of
    ///     type TIFF_UNDEFINED or TIFF_ASCII.
    /// </summary>
    public double GetAsDouble(int index)
    {
        switch (_type)
        {
            case TIFF_BYTE:
                return ((byte[])_data)[index] & 0xff;
            case TIFF_SBYTE:
                return ((byte[])_data)[index];
            case TIFF_SHORT:
                return ((char[])_data)[index] & 0xffff;
            case TIFF_SSHORT:
                return ((short[])_data)[index];
            case TIFF_SLONG:
                return ((int[])_data)[index];
            case TIFF_LONG:
                return ((long[])_data)[index];
            case TIFF_FLOAT:
                return ((float[])_data)[index];
            case TIFF_DOUBLE:
                return ((double[])_data)[index];
            case TIFF_SRATIONAL:
                var ivalue = GetAsSRational(index);
                return (double)ivalue[0] / ivalue[1];
            case TIFF_RATIONAL:
                var lvalue = GetAsRational(index);
                return (double)lvalue[0] / lvalue[1];
            default:
                throw new InvalidCastException();
        }
    }

    /// <summary>
    ///     Returns TIFF_DOUBLE data as an array of doubles.
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_DOUBLE.
    /// </summary>
    public double[] GetAsDoubles() => (double[])_data;

    /// <summary>
    ///     Returns data in any numerical format as a float.  Data in
    ///     TIFF_SRATIONAL or TIFF_RATIONAL format are evaluated by
    ///     dividing the numerator into the denominator using
    ///     double-precision arithmetic and then truncating to single
    ///     precision.  Data in TIFF_SLONG, TIFF_LONG, or TIFF_DOUBLE
    ///     format may suffer from truncation.
    ///     A ClassCastException will be thrown if the field is
    ///     of type TIFF_UNDEFINED or TIFF_ASCII.
    /// </summary>
    public float GetAsFloat(int index)
    {
        switch (_type)
        {
            case TIFF_BYTE:
                return ((byte[])_data)[index] & 0xff;
            case TIFF_SBYTE:
                return ((byte[])_data)[index];
            case TIFF_SHORT:
                return ((char[])_data)[index] & 0xffff;
            case TIFF_SSHORT:
                return ((short[])_data)[index];
            case TIFF_SLONG:
                return ((int[])_data)[index];
            case TIFF_LONG:
                return ((long[])_data)[index];
            case TIFF_FLOAT:
                return ((float[])_data)[index];
            case TIFF_DOUBLE:
                return (float)((double[])_data)[index];
            case TIFF_SRATIONAL:
                var ivalue = GetAsSRational(index);
                return (float)((double)ivalue[0] / ivalue[1]);
            case TIFF_RATIONAL:
                var lvalue = GetAsRational(index);
                return (float)((double)lvalue[0] / lvalue[1]);
            default:
                throw new InvalidCastException();
        }
    }

    /// <summary>
    ///     Returns TIFF_FLOAT data as an array of floats.
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_FLOAT.
    /// </summary>
    public float[] GetAsFloats() => (float[])_data;

    /// <summary>
    ///     Returns data in TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
    ///     TIFF_SSHORT, or TIFF_SLONG format as an int.
    ///     TIFF_BYTE and TIFF_UNDEFINED data are treated as unsigned;
    ///     that is, no sign extension will take place and the returned
    ///     value will be in the range [0, 255].  TIFF_SBYTE data will
    ///     be returned in the range [-128, 127].
    ///     A ClassCastException will be thrown if the field is not of
    ///     type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
    ///     TIFF_SSHORT, or TIFF_SLONG.
    /// </summary>
    public int GetAsInt(int index)
    {
        switch (_type)
        {
            case TIFF_BYTE:
            case TIFF_UNDEFINED:
                return ((byte[])_data)[index] & 0xff;
            case TIFF_SBYTE:
                return ((byte[])_data)[index];
            case TIFF_SHORT:
                return ((char[])_data)[index] & 0xffff;
            case TIFF_SSHORT:
                return ((short[])_data)[index];
            case TIFF_SLONG:
                return ((int[])_data)[index];
            default:
                throw new InvalidCastException();
        }
    }

    /// <summary>
    ///     Returns TIFF_SLONG data as an array of ints (signed 32-bit
    ///     integers).
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_SLONG.
    /// </summary>
    public int[] GetAsInts() => (int[])_data;

    /// <summary>
    ///     Returns data in TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
    ///     TIFF_SSHORT, TIFF_SLONG, or TIFF_LONG format as a long.
    ///     TIFF_BYTE and TIFF_UNDEFINED data are treated as unsigned;
    ///     that is, no sign extension will take place and the returned
    ///     value will be in the range [0, 255].  TIFF_SBYTE data will
    ///     be returned in the range [-128, 127].
    ///     A ClassCastException will be thrown if the field is not of
    ///     type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED, TIFF_SHORT,
    ///     TIFF_SSHORT, TIFF_SLONG, or TIFF_LONG.
    /// </summary>
    public long GetAsLong(int index)
    {
        switch (_type)
        {
            case TIFF_BYTE:
            case TIFF_UNDEFINED:
                return ((byte[])_data)[index] & 0xff;
            case TIFF_SBYTE:
                return ((byte[])_data)[index];
            case TIFF_SHORT:
                return ((char[])_data)[index] & 0xffff;
            case TIFF_SSHORT:
                return ((short[])_data)[index];
            case TIFF_SLONG:
                return ((int[])_data)[index];
            case TIFF_LONG:
                return ((long[])_data)[index];
            default:
                throw new InvalidCastException();
        }
    }

    /// <summary>
    ///     Returns TIFF_LONG data as an array of longs (signed 64-bit
    ///     integers).
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_LONG.
    /// </summary>
    public long[] GetAsLongs() => (long[])_data;

    /// <summary>
    ///     Returns a TIFF_RATIONAL data item as a two-element array
    ///     of ints.
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_RATIONAL.
    /// </summary>
    public long[] GetAsRational(int index)
    {
        if (_type == TIFF_LONG)
        {
            return GetAsLongs();
        }

        return ((long[][])_data)[index];
    }

    /// <summary>
    ///     Returns TIFF_RATIONAL data as an array of 2-element arrays of longs.
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_RATTIONAL.
    /// </summary>
    public long[][] GetAsRationals() => (long[][])_data;

    /// <summary>
    ///     Returns TIFF_SSHORT data as an array of shorts (signed 16-bit
    ///     integers).
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_SSHORT.
    /// </summary>
    public short[] GetAsShorts() => (short[])_data;

    /// <summary>
    ///     Returns a TIFF_SRATIONAL data item as a two-element array
    ///     of ints.
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_SRATIONAL.
    /// </summary>
    public int[] GetAsSRational(int index) => ((int[][])_data)[index];

    /// <summary>
    ///     Returns TIFF_SRATIONAL data as an array of 2-element arrays of ints.
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_SRATIONAL.
    /// </summary>
    public int[][] GetAsSRationals() => (int[][])_data;

    /// <summary>
    ///     Returns a TIFF_ASCII data item as a String.
    ///     A ClassCastException will be thrown if the field is not
    ///     of type TIFF_ASCII.
    /// </summary>
    public string GetAsString(int index) => ((string[])_data)[index];

    /// <summary>
    ///     Returns the number of elements in the IFD.
    /// </summary>
    public int GetCount() => _count;

    /// <summary>
    ///     Returns the tag number, between 0 and 65535.
    /// </summary>
    public int GetTag() => _tag;

    /// <summary>
    ///     Returns the type of the data stored in the IFD.
    ///     For a TIFF6.0 file, the value will equal one of the
    ///     TIFF_ constants defined in this class.  For future
    ///     revisions of TIFF, higher values are possible.
    /// </summary>
    public new int GetType() => _type;
}