namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     This class implements a simple byte vector with access to the
///     underlying array.
///     @author Carlos Villegas
/// </summary>
public class ByteVector
{
    /// <summary>
    ///     Capacity increment size
    /// </summary>
    private const int DEFAULT_BLOCK_SIZE = 2048;

    private readonly int _blockSize;

    /// <summary>
    ///     Points to next free item
    /// </summary>
    private int _n;

    public ByteVector() : this(DEFAULT_BLOCK_SIZE)
    {
    }

    public ByteVector(int capacity)
    {
        if (capacity > 0)
        {
            _blockSize = capacity;
        }
        else
        {
            _blockSize = DEFAULT_BLOCK_SIZE;
        }

        Arr = new byte[_blockSize];
        _n = 0;
    }

    public ByteVector(byte[] a)
    {
        _blockSize = DEFAULT_BLOCK_SIZE;
        Arr = a;
        _n = 0;
    }

    public ByteVector(byte[] a, int capacity)
    {
        if (capacity > 0)
        {
            _blockSize = capacity;
        }
        else
        {
            _blockSize = DEFAULT_BLOCK_SIZE;
        }

        Arr = a;
        _n = 0;
    }

    /// <summary>
    ///     The encapsulated array
    /// </summary>
    public byte[] Arr { get; private set; }

    /// <summary>
    ///     returns current capacity of array
    /// </summary>
    public int Capacity => Arr.Length;

    /// <summary>
    ///     return number of items in array
    /// </summary>
    public int Length => _n;

    public byte this[int index]
    {
        get => Arr[index];

        set => Arr[index] = value;
    }

    /// <summary>
    ///     This is to implement memory allocation in the array. Like Malloc().
    /// </summary>
    public int Alloc(int size)
    {
        var index = _n;
        var len = Arr.Length;
        if (_n + size >= len)
        {
            var aux = new byte[len + _blockSize];
            Array.Copy(Arr, 0, aux, 0, len);
            Arr = aux;
        }

        _n += size;
        return index;
    }

    public void TrimToSize()
    {
        if (_n < Arr.Length)
        {
            var aux = new byte[_n];
            Array.Copy(Arr, 0, aux, 0, _n);
            Arr = aux;
        }
    }
}