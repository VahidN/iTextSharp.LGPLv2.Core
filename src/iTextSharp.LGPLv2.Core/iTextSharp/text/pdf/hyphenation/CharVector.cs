using System;

namespace iTextSharp.text.pdf.hyphenation
{
    /// <summary>
    /// This class implements a simple char vector with access to the
    /// underlying array.
    /// @author Carlos Villegas
    /// </summary>
    public class CharVector : ICloneable
    {
        /// <summary>
        /// Capacity increment size
        /// </summary>
        private static readonly int _defaultBlockSize = 2048;
        private readonly int _blockSize;

        /// <summary>
        /// The encapsulated array
        /// </summary>
        private char[] _array;

        /// <summary>
        /// Points to next free item
        /// </summary>
        private int _n;

        public CharVector() : this(_defaultBlockSize) { }

        public CharVector(int capacity)
        {
            if (capacity > 0)
                _blockSize = capacity;
            else
                _blockSize = _defaultBlockSize;
            _array = new char[_blockSize];
            _n = 0;
        }

        public CharVector(char[] a)
        {
            _blockSize = _defaultBlockSize;
            _array = a;
            _n = a.Length;
        }

        public CharVector(char[] a, int capacity)
        {
            if (capacity > 0)
                _blockSize = capacity;
            else
                _blockSize = _defaultBlockSize;
            _array = a;
            _n = a.Length;
        }

        public char[] Arr
        {
            get
            {
                return _array;
            }
        }

        /// <summary>
        /// returns current capacity of array
        /// </summary>
        public int Capacity
        {
            get
            {
                return _array.Length;
            }
        }

        /// <summary>
        /// return number of items in array
        /// </summary>
        public int Length
        {
            get
            {
                return _n;
            }
        }

        public char this[int index]
        {
            get
            {
                return _array[index];
            }

            set
            {
                _array[index] = value;
            }
        }

        public int Alloc(int size)
        {
            int index = _n;
            int len = _array.Length;
            if (_n + size >= len)
            {
                char[] aux = new char[len + _blockSize];
                Array.Copy(_array, 0, aux, 0, len);
                _array = aux;
            }
            _n += size;
            return index;
        }

        /// <summary>
        /// Reset Vector but don't resize or clear elements
        /// </summary>
        public void Clear()
        {
            _n = 0;
        }

        public object Clone()
        {
            CharVector cv = new CharVector((char[])_array.Clone(), _blockSize);
            cv._n = _n;
            return cv;
        }

        public void TrimToSize()
        {
            if (_n < _array.Length)
            {
                char[] aux = new char[_n];
                Array.Copy(_array, 0, aux, 0, _n);
                _array = aux;
            }
        }
    }
}
