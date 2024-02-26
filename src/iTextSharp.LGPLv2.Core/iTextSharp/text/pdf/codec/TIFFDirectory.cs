using System.util;

namespace iTextSharp.text.pdf.codec;

/// <summary>
///     A class representing an Image File Directory (IFD) from a TIFF 6.0
///     stream.  The TIFF file format is described in more detail in the
///     comments for the TIFFDescriptor class.
///     A TIFF IFD consists of a set of TIFFField tags.  Methods are
///     provided to query the set of tags and to obtain the raw field
///     array.  In addition, convenience methods are provided for acquiring
///     the values of tags that contain a single value that fits into a
///     byte, int, long, float, or double.
///     Every TIFF file is made up of one or more public IFDs that are
///     joined in a linked list, rooted in the file header.  A file may
///     also contain so-called private IFDs that are referenced from
///     tag data and do not appear in the main list.
///     <b>
///         This class is not a committed part of the JAI API.  It may
///         be removed or changed in future releases of JAI.
///     </b>
///     @see TIFFField
/// </summary>
public class TiffDirectory
{
    private static readonly int[] _sizeOfType =
    {
        0, //  0 = n/a
        1, //  1 = byte
        1, //  2 = ascii
        2, //  3 = short
        4, //  4 = long
        8, //  5 = rational
        1, //  6 = sbyte
        1, //  7 = undefined
        2, //  8 = sshort
        4, //  9 = slong
        8, // 10 = srational
        4, // 11 = float
        8 // 12 = double
    };

    /// <summary>
    ///     A Hashtable indexing the fields by tag number.
    /// </summary>
    private readonly NullValueDictionary<int, int?> _fieldIndex = new();

    /// <summary>
    ///     A bool storing the endianness of the stream.
    /// </summary>
    private readonly bool _isBigEndian;

    /// <summary>
    ///     An array of TIFFFields.
    /// </summary>
    private TiffField[] _fields;

    /// <summary>
    ///     The offset of this IFD.
    /// </summary>
    private long _ifdOffset = 8;

    /// <summary>
    ///     The offset of the next IFD.
    /// </summary>
    private long _nextIfdOffset;

    /// <summary>
    ///     The number of entries in the IFD.
    /// </summary>
    private int _numEntries;

    /// <summary>
    ///     Constructs a TIFFDirectory from a SeekableStream.
    ///     The directory parameter specifies which directory to read from
    ///     the linked list present in the stream; directory 0 is normally
    ///     read but it is possible to store multiple images in a single
    ///     TIFF file by maintaing multiple directories.
    /// </summary>
    /// <param name="stream">a SeekableStream to read from.</param>
    /// <param name="directory">the index of the directory to read.</param>
    public TiffDirectory(RandomAccessFileOrArray stream, int directory)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        long globalSaveOffset = stream.FilePointer;
        long ifdOffset;

        // Read the TIFF header
        stream.Seek(0L);
        var endian = stream.ReadUnsignedShort();

        if (!isValidEndianTag(endian))
        {
            throw new InvalidOperationException("Bad endianness tag (not 0x4949 or 0x4d4d).");
        }

        _isBigEndian = endian == 0x4d4d;

        var magic = readUnsignedShort(stream);

        if (magic != 42)
        {
            throw new InvalidOperationException("Bad magic number, should be 42.");
        }

        // Get the initial ifd offset as an unsigned int (using a long)
        ifdOffset = readUnsignedInt(stream);

        for (var i = 0; i < directory; i++)
        {
            if (ifdOffset == 0L)
            {
                throw new InvalidOperationException("Directory number too large.");
            }

            stream.Seek(ifdOffset);
            var entries = readUnsignedShort(stream);
            stream.Skip(12 * entries);

            ifdOffset = readUnsignedInt(stream);
        }

        stream.Seek(ifdOffset);
        initialize(stream);
        stream.Seek(globalSaveOffset);
    }

    /// <summary>
    ///     Constructs a TIFFDirectory by reading a SeekableStream.
    ///     The ifd_offset parameter specifies the stream offset from which
    ///     to begin reading; this mechanism is sometimes used to store
    ///     private IFDs within a TIFF file that are not part of the normal
    ///     sequence of IFDs.
    ///     one at the current stream offset; zero indicates the IFD
    ///     at the current offset.
    /// </summary>
    /// <param name="stream">a SeekableStream to read from.</param>
    /// <param name="ifdOffset">the long byte offset of the directory.</param>
    /// <param name="directory">the index of the directory to read beyond the</param>
    public TiffDirectory(RandomAccessFileOrArray stream, long ifdOffset, int directory)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        long globalSaveOffset = stream.FilePointer;
        stream.Seek(0L);
        var endian = stream.ReadUnsignedShort();

        if (!isValidEndianTag(endian))
        {
            throw new InvalidOperationException("Bad endianness tag (not 0x4949 or 0x4d4d).");
        }

        _isBigEndian = endian == 0x4d4d;

        // Seek to the first IFD.
        stream.Seek(ifdOffset);

        // Seek to desired IFD if necessary.
        var dirNum = 0;

        while (dirNum < directory)
        {
            // Get the number of fields in the current IFD.
            var numEntries = readUnsignedShort(stream);

            // Skip to the next IFD offset value field.
            stream.Seek(ifdOffset + 12 * numEntries);

            // Read the offset to the next IFD beyond this one.
            ifdOffset = readUnsignedInt(stream);

            // Seek to the next IFD.
            stream.Seek(ifdOffset);

            // Increment the directory.
            dirNum++;
        }

        initialize(stream);
        stream.Seek(globalSaveOffset);
    }

    /// <summary>
    ///     The default constructor.
    /// </summary>
    private TiffDirectory()
    {
    }

    /// <summary>
    ///     Returns the number of image directories (subimages) stored in a
    ///     given TIFF file, represented by a  SeekableStream .
    /// </summary>
    public static int GetNumDirectories(RandomAccessFileOrArray stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        long pointer = stream.FilePointer; // Save stream pointer

        stream.Seek(0L);
        var endian = stream.ReadUnsignedShort();

        if (!isValidEndianTag(endian))
        {
            throw new InvalidOperationException("Bad endianness tag (not 0x4949 or 0x4d4d).");
        }

        var isBigEndian = endian == 0x4d4d;
        var magic = readUnsignedShort(stream, isBigEndian);

        if (magic != 42)
        {
            throw new InvalidOperationException("Bad magic number, should be 42.");
        }

        stream.Seek(4L);
        var offset = readUnsignedInt(stream, isBigEndian);

        var numDirectories = 0;

        while (offset != 0L)
        {
            ++numDirectories;

            // EOFException means IFD was probably not properly terminated.
            try
            {
                stream.Seek(offset);
                var entries = readUnsignedShort(stream, isBigEndian);
                stream.Skip(12 * entries);
                offset = readUnsignedInt(stream, isBigEndian);
            }
            catch (EndOfStreamException)
            {
                //numDirectories--;
                break;
            }
        }

        stream.Seek(pointer); // Reset stream pointer

        return numDirectories;
    }

    /// <summary>
    ///     Returns the value of a given tag as a TIFFField,
    ///     or null if the tag is not present.
    /// </summary>
    public TiffField GetField(int tag)
    {
        var i = _fieldIndex[tag];

        return i.HasValue ? _fields[i.Value] : null;
    }

    /// <summary>
    ///     Returns the value of a particular index of a given tag as a
    ///     byte.  The caller is responsible for ensuring that the tag is
    ///     present and has type TIFFField.TIFF_SBYTE, TIFF_BYTE, or
    ///     TIFF_UNDEFINED.
    /// </summary>
    public byte? GetFieldAsByte(int tag, int index)
    {
        var i = _fieldIndex[tag];

        if (!i.HasValue)
        {
            return null;
        }

        var b = _fields[i.Value].GetAsBytes();

        return b[index];
    }

    /// <summary>
    ///     Returns the value of index 0 of a given tag as a
    ///     byte.  The caller is responsible for ensuring that the tag is
    ///     present and has  type TIFFField.TIFF_SBYTE, TIFF_BYTE, or
    ///     TIFF_UNDEFINED.
    /// </summary>
    public byte? GetFieldAsByte(int tag)
        => GetFieldAsByte(tag, 0);

    /// <summary>
    ///     Returns the value of a particular index of a given tag as a
    ///     double.  The caller is responsible for ensuring that the tag is
    ///     present and has numeric type (all but TIFF_UNDEFINED and
    ///     TIFF_ASCII).
    /// </summary>
    public double? GetFieldAsDouble(int tag, int index)
    {
        var i = _fieldIndex[tag];

        if (!i.HasValue)
        {
            return null;
        }

        return _fields[i.Value].GetAsDouble(index);
    }

    /// <summary>
    ///     Returns the value of index 0 of a given tag as a double.  The
    ///     caller is responsible for ensuring that the tag is present and
    ///     has numeric type (all but TIFF_UNDEFINED and TIFF_ASCII).
    /// </summary>
    public double? GetFieldAsDouble(int tag)
        => GetFieldAsDouble(tag, 0);

    /// <summary>
    ///     Returns the value of a particular index of a given tag as a
    ///     float.  The caller is responsible for ensuring that the tag is
    ///     present and has numeric type (all but TIFF_UNDEFINED and
    ///     TIFF_ASCII).
    /// </summary>
    public float? GetFieldAsFloat(int tag, int index)
    {
        var i = _fieldIndex[tag];

        if (!i.HasValue)
        {
            return null;
        }

        return _fields[i.Value].GetAsFloat(index);
    }

    /// <summary>
    ///     Returns the value of index 0 of a given tag as a float.  The
    ///     caller is responsible for ensuring that the tag is present and
    ///     has numeric type (all but TIFF_UNDEFINED and TIFF_ASCII).
    /// </summary>
    public float? GetFieldAsFloat(int tag)
        => GetFieldAsFloat(tag, 0);

    /// <summary>
    ///     Returns the value of a particular index of a given tag as a
    ///     long.  The caller is responsible for ensuring that the tag is
    ///     present and has type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED,
    ///     TIFF_SHORT, TIFF_SSHORT, TIFF_SLONG or TIFF_LONG.
    /// </summary>
    public long? GetFieldAsLong(int tag, int index)
    {
        var i = _fieldIndex[tag];

        if (!i.HasValue)
        {
            return null;
        }

        return _fields[i.Value].GetAsLong(index);
    }

    /// <summary>
    ///     Returns the value of index 0 of a given tag as a
    ///     long.  The caller is responsible for ensuring that the tag is
    ///     present and has type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED,
    ///     TIFF_SHORT, TIFF_SSHORT, TIFF_SLONG or TIFF_LONG.
    /// </summary>
    public long? GetFieldAsLong(int tag)
        => GetFieldAsLong(tag, 0);

    /// <summary>
    ///     Returns an array of TIFFFields containing all the fields
    ///     in this directory.
    /// </summary>
    public TiffField[] GetFields()
        => _fields;

    /// <summary>
    ///     Returns the offset of the IFD corresponding to this
    ///     TIFFDirectory .
    /// </summary>
    public long GetIfdOffset()
        => _ifdOffset;

    /// <summary>
    ///     Returns the offset of the next IFD after the IFD corresponding to this
    ///     TIFFDirectory .
    /// </summary>
    public long GetNextIfdOffset()
        => _nextIfdOffset;

    /// <summary>
    ///     Returns the number of directory entries.
    /// </summary>
    public int GetNumEntries()
        => _numEntries;

    /// <summary>
    ///     Returns an ordered array of ints indicating the tag
    ///     values.
    /// </summary>
    public int[] GetTags()
    {
        var tags = new int[_fieldIndex.Count];
        _fieldIndex.Keys.CopyTo(tags, 0);

        return tags;
    }

    /// <summary>
    ///     Utilities
    /// </summary>
    /// <summary>
    ///     Returns a bool indicating whether the byte order used in the
    ///     the TIFF file is big-endian (i.e. whether the byte order is from
    ///     the most significant to the least significant)
    /// </summary>
    public bool IsBigEndian()
        => _isBigEndian;

    /// <summary>
    ///     Returns true if a tag appears in the directory.
    /// </summary>
    public bool IsTagPresent(int tag)
        => _fieldIndex.ContainsKey(tag);

    private static bool isValidEndianTag(int endian)
        => endian == 0x4949 || endian == 0x4d4d;

    private static long readUnsignedInt(RandomAccessFileOrArray stream, bool isBigEndian)
    {
        if (isBigEndian)
        {
            return stream.ReadUnsignedInt();
        }

        return stream.ReadUnsignedIntLe();
    }

    private static int readUnsignedShort(RandomAccessFileOrArray stream, bool isBigEndian)
    {
        if (isBigEndian)
        {
            return stream.ReadUnsignedShort();
        }

        return stream.ReadUnsignedShortLe();
    }

    private void initialize(RandomAccessFileOrArray stream)
    {
        var nextTagOffset = 0L;
        long maxOffset = stream.Length;
        int i, j;

        _ifdOffset = stream.FilePointer;

        _numEntries = readUnsignedShort(stream);
        _fields = new TiffField[_numEntries];

        for (i = 0; i < _numEntries && nextTagOffset < maxOffset; i++)
        {
            var tag = readUnsignedShort(stream);
            var type = readUnsignedShort(stream);
            var count = (int)readUnsignedInt(stream);
            var processTag = true;

            // The place to return to to read the next tag
            nextTagOffset = stream.FilePointer + 4;

            try
            {
                // If the tag data can't fit in 4 bytes, the next 4 bytes
                // contain the starting offset of the data
                if (count * _sizeOfType[type] > 4)
                {
                    var valueOffset = readUnsignedInt(stream);

                    // bounds check offset for EOF
                    if (valueOffset < maxOffset)
                    {
                        stream.Seek(valueOffset);
                    }
                    else
                    {
                        // bad offset pointer .. skip tag
                        processTag = false;
                    }
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                // if the data type is unknown we should skip this TIFF Field
                processTag = false;
            }

            if (processTag)
            {
                _fieldIndex[tag] = i;
                object obj = null;

                switch (type)
                {
                    case TiffField.TIFF_BYTE:
                    case TiffField.TIFF_SBYTE:
                    case TiffField.TIFF_UNDEFINED:
                    case TiffField.TIFF_ASCII:
                        var bvalues = new byte[count];
                        stream.ReadFully(bvalues, 0, count);

                        if (type == TiffField.TIFF_ASCII)
                        {
                            // Can be multiple strings
                            int index = 0, prevIndex = 0;
                            List<string> v = new();

                            while (index < count)
                            {
                                while (index < count && bvalues[index++] != 0)
                                {
                                    ;
                                }

                                // When we encountered zero, means one string has ended
                                var cht = new char[index - prevIndex];
                                Array.Copy(bvalues, prevIndex, cht, 0, index - prevIndex);
                                v.Add(new string(cht));
                                prevIndex = index;
                            }

                            count = v.Count;
                            var strings = new string[count];

                            for (var c = 0; c < count; c++)
                            {
                                strings[c] = v[c];
                            }

                            obj = strings;
                        }
                        else
                        {
                            obj = bvalues;
                        }

                        break;

                    case TiffField.TIFF_SHORT:
                        var cvalues = new char[count];

                        for (j = 0; j < count; j++)
                        {
                            cvalues[j] = (char)readUnsignedShort(stream);
                        }

                        obj = cvalues;

                        break;

                    case TiffField.TIFF_LONG:
                        var lvalues = new long[count];

                        for (j = 0; j < count; j++)
                        {
                            lvalues[j] = readUnsignedInt(stream);
                        }

                        obj = lvalues;

                        break;

                    case TiffField.TIFF_RATIONAL:
                        var llvalues = new long[count][];

                        for (j = 0; j < count; j++)
                        {
                            var v0 = readUnsignedInt(stream);
                            var v1 = readUnsignedInt(stream);

                            llvalues[j] = new[]
                            {
                                v0, v1
                            };
                        }

                        obj = llvalues;

                        break;

                    case TiffField.TIFF_SSHORT:
                        var svalues = new short[count];

                        for (j = 0; j < count; j++)
                        {
                            svalues[j] = readShort(stream);
                        }

                        obj = svalues;

                        break;

                    case TiffField.TIFF_SLONG:
                        var ivalues = new int[count];

                        for (j = 0; j < count; j++)
                        {
                            ivalues[j] = readInt(stream);
                        }

                        obj = ivalues;

                        break;

                    case TiffField.TIFF_SRATIONAL:
                        var iivalues = new int[count, 2];

                        for (j = 0; j < count; j++)
                        {
                            iivalues[j, 0] = readInt(stream);
                            iivalues[j, 1] = readInt(stream);
                        }

                        obj = iivalues;

                        break;

                    case TiffField.TIFF_FLOAT:
                        var fvalues = new float[count];

                        for (j = 0; j < count; j++)
                        {
                            fvalues[j] = readFloat(stream);
                        }

                        obj = fvalues;

                        break;

                    case TiffField.TIFF_DOUBLE:
                        var dvalues = new double[count];

                        for (j = 0; j < count; j++)
                        {
                            dvalues[j] = readDouble(stream);
                        }

                        obj = dvalues;

                        break;
                }

                _fields[i] = new TiffField(tag, type, count, obj);
            }

            stream.Seek(nextTagOffset);
        }

        // Read the offset of the next IFD.
        try
        {
            _nextIfdOffset = readUnsignedInt(stream);
        }
        catch
        {
            // broken tiffs may not have this pointer
            _nextIfdOffset = 0;
        }
    }

    /// <summary>
    ///     Methods to read primitive data types from the stream
    /// </summary>
    private double readDouble(RandomAccessFileOrArray stream)
    {
        if (_isBigEndian)
        {
            return stream.ReadDouble();
        }

        return stream.ReadDoubleLe();
    }

    private float readFloat(RandomAccessFileOrArray stream)
    {
        if (_isBigEndian)
        {
            return stream.ReadFloat();
        }

        return stream.ReadFloatLe();
    }

    private int readInt(RandomAccessFileOrArray stream)
    {
        if (_isBigEndian)
        {
            return stream.ReadInt();
        }

        return stream.ReadIntLe();
    }

    private long readLong(RandomAccessFileOrArray stream)
    {
        if (_isBigEndian)
        {
            return stream.ReadLong();
        }

        return stream.ReadLongLe();
    }

    private short readShort(RandomAccessFileOrArray stream)
    {
        if (_isBigEndian)
        {
            return stream.ReadShort();
        }

        return stream.ReadShortLe();
    }

    private long readUnsignedInt(RandomAccessFileOrArray stream)
    {
        if (_isBigEndian)
        {
            return stream.ReadUnsignedInt();
        }

        return stream.ReadUnsignedIntLe();
    }

    private int readUnsignedShort(RandomAccessFileOrArray stream)
    {
        if (_isBigEndian)
        {
            return stream.ReadUnsignedShort();
        }

        return stream.ReadUnsignedShortLe();
    }
}