namespace iTextSharp.text.pdf;

/// <summary>
///     A Hashtable that uses ints as the keys.
/// </summary>
public class IntHashtable
{
    /// The total number of entries in the hash table.
    private int _count;

    /// The load factor for the hashtable.
    private float _loadFactor;

    /// The hash table data.
    private IntHashtableEntry[] _table;

    /// Rehashes the table when count exceeds this threshold.
    private int _threshold;

    /// <summary>
    /// </summary>
    /// <param name="initialCapacity"></param>
    /// <param name="loadFactor"></param>
    public IntHashtable(int initialCapacity, float loadFactor)
    {
        if (initialCapacity <= 0 || loadFactor <= 0.0)
        {
            throw new ArgumentException();
        }

        _loadFactor = loadFactor;
        _table = new IntHashtableEntry[initialCapacity];
        _threshold = (int)(initialCapacity * loadFactor);
    }

    /// Constructs a new, empty hashtable with the specified initial
    /// <summary>
    ///     capacity.
    /// </summary>
    /// <summary>
    ///     @param initialCapacity the initial number of buckets
    /// </summary>
    public IntHashtable(int initialCapacity) : this(initialCapacity, 0.75f)
    {
    }

    /// <summary>
    /// </summary>
    public IntHashtable() : this(101, 0.75f)
    {
    }

    /// Returns the number of elements contained in the hashtable.
    public int Size => _count;

    /// <summary>
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public int this[int key]
    {
        get
        {
            var tab = _table;
            var hash = key;
            var index = (hash & 0x7FFFFFFF) % tab.Length;
            for (var e = tab[index]; e != null; e = e.Next)
            {
                if (e.Hash == hash && e.key == key)
                {
                    return e.value;
                }
            }

            return 0;
        }

        set
        {
            var tab = _table;
            var hash = key;
            var index = (hash & 0x7FFFFFFF) % tab.Length;
            for (var e = tab[index]; e != null; e = e.Next)
            {
                if (e.Hash == hash && e.key == key)
                {
                    e.value = value;
                    return;
                }
            }

            if (_count >= _threshold)
            {
                Rehash();
                this[key] = value;
                return;
            }

            var en = new IntHashtableEntry();
            en.Hash = hash;
            en.key = key;
            en.value = value;
            en.Next = tab[index];
            tab[index] = en;
            ++_count;
        }
    }

    /// Clears the hash table so that it has no more elements in it.
    public void Clear()
    {
        var tab = _table;
        for (var index = tab.Length; --index >= 0;)
        {
            tab[index] = null;
        }

        _count = 0;
    }

    public IntHashtable Clone()
    {
        var t = new IntHashtable();
        t._count = _count;
        t._loadFactor = _loadFactor;
        t._threshold = _threshold;
        t._table = new IntHashtableEntry[_table.Length];
        for (var i = _table.Length; i-- > 0;)
        {
            t._table[i] = _table[i] != null
                              ? _table[i].Clone()
                              : null;
        }

        return t;
    }

    /// Returns true if the specified object is an element of the hashtable.
    /// <summary>
    ///     This operation is more expensive than the ContainsKey() method.
    /// </summary>
    /// <summary>
    ///     @param value the value that we are looking for
    /// </summary>
    /// <summary>
    ///     @exception NullPointerException If the value being searched
    /// </summary>
    /// <summary>
    ///     for is equal to null.
    /// </summary>
    /// <summary>
    ///     @see IntHashtable#containsKey
    /// </summary>
    public bool Contains(int value)
    {
        var tab = _table;
        for (var i = tab.Length; i-- > 0;)
        {
            for (var e = tab[i]; e != null; e = e.Next)
            {
                if (e.value == value)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// Returns true if the collection contains an element for the key.
    /// <summary>
    ///     @param key the key that we are looking for
    /// </summary>
    /// <summary>
    ///     @see IntHashtable#contains
    /// </summary>
    public bool ContainsKey(int key)
    {
        var tab = _table;
        var hash = key;
        var index = (hash & 0x7FFFFFFF) % tab.Length;
        for (var e = tab[index]; e != null; e = e.Next)
        {
            if (e.Hash == hash && e.key == key)
            {
                return true;
            }
        }

        return false;
    }

    public IntHashtableIterator GetEntryIterator() => new IntHashtableIterator(_table);

    public int[] GetKeys()
    {
        var res = new int[_count];
        var ptr = 0;
        var index = _table.Length;
        IntHashtableEntry entry = null;
        while (true)
        {
            if (entry == null)
            {
                while (index-- > 0 && (entry = _table[index]) == null)
                {
                    ;
                }
            }

            if (entry == null)
            {
                break;
            }

            var e = entry;
            entry = e.Next;
            res[ptr++] = e.key;
        }

        return res;
    }

    /// Returns true if the hashtable contains no elements.
    public bool IsEmpty() => _count == 0;

    /// Removes the element corresponding to the key. Does nothing if the
    /// <summary>
    ///     key is not present.
    /// </summary>
    /// <summary>
    ///     @param key the key that needs to be removed
    /// </summary>
    /// <summary>
    ///     @return the value of key, or null if the key was not found.
    /// </summary>
    public int Remove(int key)
    {
        var tab = _table;
        var hash = key;
        var index = (hash & 0x7FFFFFFF) % tab.Length;
        for (IntHashtableEntry e = tab[index], prev = null; e != null; prev = e, e = e.Next)
        {
            if (e.Hash == hash && e.key == key)
            {
                if (prev != null)
                {
                    prev.Next = e.Next;
                }
                else
                {
                    tab[index] = e.Next;
                }

                --_count;
                return e.value;
            }
        }

        return 0;
    }

    public int[] ToOrderedKeys()
    {
        var res = GetKeys();
        Array.Sort(res);
        return res;
    }

    /// Rehashes the content of the table into a bigger table.
    /// <summary>
    ///     This method is called automatically when the hashtable's
    /// </summary>
    /// <summary>
    ///     size exceeds the threshold.
    /// </summary>
    protected void Rehash()
    {
        var oldCapacity = _table.Length;
        var oldTable = _table;

        var newCapacity = oldCapacity * 2 + 1;
        var newTable = new IntHashtableEntry[newCapacity];

        _threshold = (int)(newCapacity * _loadFactor);
        _table = newTable;

        for (var i = oldCapacity; i-- > 0;)
        {
            for (var old = oldTable[i]; old != null;)
            {
                var e = old;
                old = old.Next;

                var index = (e.Hash & 0x7FFFFFFF) % newCapacity;
                e.Next = newTable[index];
                newTable[index] = e;
            }
        }
    }

    public class IntHashtableEntry
    {
        internal int Hash;
        internal int key;
        internal IntHashtableEntry Next;
        internal int value;

        public int Key => key;

        public int Value => value;

        protected internal IntHashtableEntry Clone()
        {
            var entry = new IntHashtableEntry();
            entry.Hash = Hash;
            entry.key = key;
            entry.value = value;
            entry.Next = Next != null ? Next.Clone() : null;
            return entry;
        }
    }

    public class IntHashtableIterator
    {
        private readonly IntHashtableEntry[] _table;

        private IntHashtableEntry _entry;

        /// <summary>
        ///     boolean keys;
        /// </summary>
        private int _index;

        internal IntHashtableIterator(IntHashtableEntry[] table)
        {
            _table = table;
            _index = table.Length;
        }

        public bool HasNext()
        {
            if (_entry != null)
            {
                return true;
            }

            while (_index-- > 0)
            {
                if ((_entry = _table[_index]) != null)
                {
                    return true;
                }
            }

            return false;
        }

        public IntHashtableEntry Next()
        {
            if (_entry == null)
            {
                while (_index-- > 0 && (_entry = _table[_index]) == null)
                {
                    ;
                }
            }

            if (_entry != null)
            {
                var e = _entry;
                _entry = e.Next;
                return e;
            }

            throw new InvalidOperationException("IntHashtableIterator");
        }
    }
}