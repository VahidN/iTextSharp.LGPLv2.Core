namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     This class implements a simple char vector with access to the
///     underlying array.
///     @author Carlos Villegas
/// </summary>
public class CharVector : ICloneable
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

    public CharVector() : this(DEFAULT_BLOCK_SIZE)
    {
    }

    public CharVector(int capacity)
    {
        if (capacity > 0)
        {
            _blockSize = capacity;
        }
        else
        {
            _blockSize = DEFAULT_BLOCK_SIZE;
        }

        Arr = new char[_blockSize];
        _n = 0;
    }

    public CharVector(char[] a)
    {
        _blockSize = DEFAULT_BLOCK_SIZE;
        Arr = a ?? throw new ArgumentNullException(nameof(a));
        _n = a.Length;
    }

    public CharVector(char[] a, int capacity)
    {
        if (a == null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        if (capacity > 0)
        {
            _blockSize = capacity;
        }
        else
        {
            _blockSize = DEFAULT_BLOCK_SIZE;
        }

        Arr = a;
        _n = a.Length;
    }

    /// <summary>
    ///     The encapsulated array
    /// </summary>
    public char[] Arr { get; private set; }

    /// <summary>
    ///     returns current capacity of array
    /// </summary>
    public int Capacity => Arr.Length;

    /// <summary>
    ///     return number of items in array
    /// </summary>
    public int Length => _n;

    public char this[int index]
    {
        get => Arr[index];

        set => Arr[index] = value;
    }

    public object Clone()
    {
        var cv = new CharVector((char[])Arr.Clone(), _blockSize);
        cv._n = _n;
        return cv;
    }

    public int Alloc(int size)
    {
        var index = _n;
        var len = Arr.Length;
        if (_n + size >= len)
        {
            var aux = new char[len + _blockSize];
            Array.Copy(Arr, 0, aux, 0, len);
            Arr = aux;
        }

        _n += size;
        return index;
    }

    /// <summary>
    ///     Reset Vector but don't resize or clear elements
    /// </summary>
    public void Clear()
    {
        _n = 0;
    }

    public void TrimToSize()
    {
        if (_n < Arr.Length)
        {
            var aux = new char[_n];
            Array.Copy(Arr, 0, aux, 0, _n);
            Arr = aux;
        }
    }
}