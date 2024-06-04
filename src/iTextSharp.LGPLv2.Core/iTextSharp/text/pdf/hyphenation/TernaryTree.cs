using System.Text;

namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     <h2>Ternary Search Tree</h2>
///     A ternary search tree is a hibrid between a binary tree and
///     a digital search tree (trie). Keys are limited to strings.
///     A data value of type char is stored in each leaf node.
///     It can be used as an index (or pointer) to the data.
///     Branches that only contain one key are compressed to one node
///     by storing a pointer to the trailer substring of the key.
///     This class is intended to serve as base class or helper class
///     to implement Dictionary collections or the like. Ternary trees
///     have some nice properties as the following: the tree can be
///     traversed in sorted order, partial matches (wildcard) can be
///     implemented, retrieval of all keys within a given distance
///     from the target, etc. The storage requirements are higher than
///     a binary tree but a lot less than a trie. Performance is
///     comparable with a hash table, sometimes it outperforms a hash
///     function (most of the time can determine a miss faster than a hash).
///     The main purpose of this java port is to serve as a base for
///     implementing TeX's hyphenation algorithm (see The TeXBook,
///     appendix H). Each language requires from 5000 to 15000 hyphenation
///     patterns which will be keys in this tree. The strings patterns
///     are usually small (from 2 to 5 characters), but each char in the
///     tree is stored in a node. Thus memory usage is the main concern.
///     We will sacrify 'elegance' to keep memory requirenments to the
///     minimum. Using java's char type as pointer (yes, I know pointer
///     it is a forbidden word in java) we can keep the size of the node
///     to be just 8 bytes (3 pointers and the data char). This gives
///     room for about 65000 nodes. In my tests the english patterns
///     took 7694 nodes and the german patterns 10055 nodes,
///     so I think we are safe.
///     All said, this is a map with strings as keys and char as value.
///     Pretty limited!. It can be extended to a general map by
///     using the string representation of an object and using the
///     char value as an index to an array that contains the object
///     values.
///     @author cav@uniscope.co.jp
/// </summary>
public class TernaryTree : ICloneable
{
    /// <summary>
    ///     We use 4 arrays to represent a node. I guess I should have created
    ///     a proper node class, but somehow Knuth's pascal code made me forget
    ///     we now have a portable language with memory management and
    ///     automatic garbage collection! And now is kind of late, furthermore,
    ///     if it ain't broken, don't fix it.
    /// </summary>
    protected static int BlockSize = 2048;

    /// <summary>
    ///     Pointer to equal branch and to data when this node is a string terminator.
    /// </summary>
    protected char[] Eq;

    protected char Freenode;

    /// <summary>
    ///     Pointer to high branch.
    /// </summary>
    protected char[] Hi;

    /// <summary>
    ///     This vector holds the trailing of the keys when the branch is compressed.
    /// </summary>
    protected CharVector Kv;

    protected int Length;

    /// <summary>
    ///     Pointer to low branch and to rest of the key when it is
    ///     stored directly in this node, we don't have unions in java!
    /// </summary>
    protected char[] Lo;

    protected char Root;

    /// <summary>
    ///     The character stored in this node: splitchar
    ///     Two special values are reserved:
    ///     0x0000 as string terminator
    ///     0xFFFF to indicate that the branch starting at
    ///     this node is compressed
    ///     This shouldn't be a problem if we give the usual semantics to
    ///     strings since 0xFFFF is garanteed not to be an Unicode character.
    /// </summary>
    protected char[] Sc;
    // number of items in tree

    // allocation size for arrays

    internal TernaryTree()
    {
        Init();
    }

    public Iterator Keys => new(this);

    public int Size => Length;

    public object Clone()
    {
        var t = new TernaryTree
                {
                    Lo = (char[])Lo.Clone(),
                    Hi = (char[])Hi.Clone(),
                    Eq = (char[])Eq.Clone(),
                    Sc = (char[])Sc.Clone(),
                    Kv = (CharVector)Kv.Clone(),
                    Root = Root,
                    Freenode = Freenode,
                    Length = Length,
                };

        return t;
    }

    /// <summary>
    ///     Compares 2 null terminated char arrays
    /// </summary>
    public static int Strcmp(char[] a, int startA, char[] b, int startB)
    {
        if (a == null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        if (b == null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        for (; a[startA] == b[startB]; startA++, startB++)
        {
            if (a[startA] == 0)
            {
                return 0;
            }
        }

        return a[startA] - b[startB];
    }

    /// <summary>
    ///     Compares a string with null terminated char array
    /// </summary>
    public static int Strcmp(string str, char[] a, int start)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (a == null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        int i, d, len = str.Length;
        for (i = 0; i < len; i++)
        {
            d = str[i] - a[start + i];
            if (d != 0)
            {
                return d;
            }

            if (a[start + i] == 0)
            {
                return d;
            }
        }

        if (a[start + i] != 0)
        {
            return -a[start + i];
        }

        return 0;
    }

    public static void Strcpy(char[] dst, int di, char[] src, int si)
    {
        if (dst == null)
        {
            throw new ArgumentNullException(nameof(dst));
        }

        if (src == null)
        {
            throw new ArgumentNullException(nameof(src));
        }

        while (src[si] != 0)
        {
            dst[di++] = src[si++];
        }

        dst[di] = (char)0;
    }

    public static int Strlen(char[] a, int start)
    {
        if (a == null)
        {
            throw new ArgumentNullException(nameof(a));
        }

        var len = 0;
        for (var i = start; i < a.Length && a[i] != 0; i++)
        {
            len++;
        }

        return len;
    }

    public static int Strlen(char[] a) => Strlen(a, 0);

    /// <summary>
    ///     Balance the tree for best search performance
    /// </summary>
    public void Balance()
    {
        // System.out.Print("Before root splitchar = "); System.out.Println(sc[root]);

        int i = 0, n = Length;
        var k = new string[n];
        var v = new char[n];
        var iter = new Iterator(this);
        while (iter.HasMoreElements())
        {
            v[i] = iter.Value;
            k[i++] = (string)iter.NextElement();
        }

        Init();
        InsertBalanced(k, v, 0, n);

        // With uniform letter distribution sc[root] should be around 'm'
        // System.out.Print("After root splitchar = "); System.out.Println(sc[root]);
    }

    public int Find(string key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var len = key.Length;
        var strkey = new char[len + 1];
        key.CopyTo(0, strkey, 0, len);
        strkey[len] = (char)0;

        return Find(strkey, 0);
    }

    public int Find(char[] key, int start)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        int d;
        var p = Root;
        var i = start;
        char c;

        while (p != 0)
        {
            if (Sc[p] == 0xFFFF)
            {
                if (Strcmp(key, i, Kv.Arr, Lo[p]) == 0)
                {
                    return Eq[p];
                }

                return -1;
            }

            c = key[i];
            d = c - Sc[p];
            if (d == 0)
            {
                if (c == 0)
                {
                    return Eq[p];
                }

                i++;
                p = Eq[p];
            }
            else if (d < 0)
            {
                p = Lo[p];
            }
            else
            {
                p = Hi[p];
            }
        }

        return -1;
    }

    /// <summary>
    ///     Branches are initially compressed, needing
    ///     one node per key plus the size of the string
    ///     key. They are decompressed as needed when
    ///     another key with same prefix
    ///     is inserted. This saves a lot of space,
    ///     specially for long keys.
    /// </summary>
    public void Insert(string key, char val)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        // make sure we have enough room in the arrays
        var len = key.Length
                  + 1; // maximum number of nodes that may be generated
        if (Freenode + len > Eq.Length)
        {
            redimNodeArrays(Eq.Length + BlockSize);
        }

        var strkey = new char[len--];
        key.CopyTo(0, strkey, 0, len);
        strkey[len] = (char)0;
        Root = insert(Root, strkey, 0, val);
    }

    public void Insert(char[] key, int start, char val)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var len = Strlen(key) + 1;
        if (Freenode + len > Eq.Length)
        {
            redimNodeArrays(Eq.Length + BlockSize);
        }

        Root = insert(Root, key, start, val);
    }

    public bool Knows(string key) => Find(key) >= 0;

    public virtual void PrintStats()
    {
        Console.Error.WriteLine("Number of keys = " + Length);
        Console.Error.WriteLine("Node count = " + Freenode);
        // Console.Error.WriteLine("Array length = " + int.ToString(eq.Length));
        Console.Error.WriteLine("Key Array length = "
                                + Kv.Length);

        /*
         * for (int i=0; i<kv.Length; i++)
         * if ( kv[i] != 0 )
         * System.out.Print(kv[i]);
         * else
         * System.out.Println("");
         * System.out.Println("Keys:");
         * for (Enumeration enum = Keys(); enum.HasMoreElements(); )
         * System.out.Println(enum.NextElement());
         */
    }

    /// <summary>
    ///     Each node stores a character (splitchar) which is part of
    ///     some Key(s). In a compressed branch (one that only contain
    ///     a single string key) the trailer of the key which is not
    ///     already in nodes is stored  externally in the kv array.
    ///     As items are inserted, key substrings decrease.
    ///     Some substrings may completely  disappear when the whole
    ///     branch is totally decompressed.
    ///     The tree is traversed to find the key substrings actually
    ///     used. In addition, duplicate substrings are removed using
    ///     a map (implemented with a TernaryTree!).
    /// </summary>
    public void TrimToSize()
    {
        // first balance the tree for best performance
        Balance();

        // redimension the node arrays
        redimNodeArrays(Freenode);

        // ok, compact kv array
        var kx = new CharVector();
        kx.Alloc(1);
        var map = new TernaryTree();
        compact(kx, map, Root);
        Kv = kx;
        Kv.TrimToSize();
    }

    protected void Init()
    {
        Root = (char)0;
        Freenode = (char)1;
        Length = 0;
        Lo = new char[BlockSize];
        Hi = new char[BlockSize];
        Eq = new char[BlockSize];
        Sc = new char[BlockSize];
        Kv = new CharVector();
    }

    /// <summary>
    ///     Recursively insert the median first and then the median of the
    ///     lower and upper halves, and so on in order to get a balanced
    ///     tree. The array of keys is assumed to be sorted in ascending
    ///     order.
    /// </summary>
    protected void InsertBalanced(string[] k, char[] v, int offset, int n)
    {
        if (k == null)
        {
            throw new ArgumentNullException(nameof(k));
        }

        if (v == null)
        {
            throw new ArgumentNullException(nameof(v));
        }

        if (n < 1)
        {
            return;
        }

        var m = n >> 1;

        Insert(k[m + offset], v[m + offset]);
        InsertBalanced(k, v, offset, m);

        InsertBalanced(k, v, offset + m + 1, n - m - 1);
    }

    private void compact(CharVector kx, TernaryTree map, char p)
    {
        int k;
        if (p == 0)
        {
            return;
        }

        if (Sc[p] == 0xFFFF)
        {
            k = map.Find(Kv.Arr, Lo[p]);
            if (k < 0)
            {
                k = kx.Alloc(Strlen(Kv.Arr, Lo[p]) + 1);
                Strcpy(kx.Arr, k, Kv.Arr, Lo[p]);
                map.Insert(kx.Arr, k, (char)k);
            }

            Lo[p] = (char)k;
        }
        else
        {
            compact(kx, map, Lo[p]);
            if (Sc[p] != 0)
            {
                compact(kx, map, Eq[p]);
            }

            compact(kx, map, Hi[p]);
        }
    }

    /// <summary>
    ///     The actual insertion function, recursive version.
    /// </summary>
    private char insert(char p, char[] key, int start, char val)
    {
        var len = Strlen(key, start);
        if (p == 0)
        {
            // this means there is no branch, this node will start a new branch.
            // Instead of doing that, we store the key somewhere else and create
            // only one node with a pointer to the key
            p = Freenode++;
            Eq[p] = val; // holds data
            Length++;
            Hi[p] = (char)0;
            if (len > 0)
            {
                Sc[p] = (char)0xFFFF; // indicates branch is compressed
                Lo[p] = (char)Kv.Alloc(len
                                       + 1); // use 'lo' to hold pointer to key
                Strcpy(Kv.Arr, Lo[p], key, start);
            }
            else
            {
                Sc[p] = (char)0;
                Lo[p] = (char)0;
            }

            return p;
        }

        if (Sc[p] == 0xFFFF)
        {
            // branch is compressed: need to decompress
            // this will generate garbage in the external key array
            // but we can do some garbage collection later
            var pp = Freenode++;
            Lo[pp] = Lo[p]; // previous pointer to key
            Eq[pp] = Eq[p]; // previous pointer to data
            Lo[p] = (char)0;
            if (len > 0)
            {
                Sc[p] = Kv[Lo[pp]];
                Eq[p] = pp;
                Lo[pp]++;
                if (Kv[Lo[pp]] == 0)
                {
                    // key completly decompressed leaving garbage in key array
                    Lo[pp] = (char)0;
                    Sc[pp] = (char)0;
                    Hi[pp] = (char)0;
                }
                else
                {
                    Sc[pp] =
                        (char)0xFFFF; // we only got first char of key, rest is still there
                }
            }
            else
            {
                // In this case we can save a node by swapping the new node
                // with the compressed node
                Sc[pp] = (char)0xFFFF;
                Hi[p] = pp;
                Sc[p] = (char)0;
                Eq[p] = val;
                Length++;
                return p;
            }
        }

        var s = key[start];
        if (s < Sc[p])
        {
            Lo[p] = insert(Lo[p], key, start, val);
        }
        else if (s == Sc[p])
        {
            if (s != 0)
            {
                Eq[p] = insert(Eq[p], key, start + 1, val);
            }
            else
            {
                // key already in tree, overwrite data
                Eq[p] = val;
            }
        }
        else
        {
            Hi[p] = insert(Hi[p], key, start, val);
        }

        return p;
    }

    /// <summary>
    ///     redimension the arrays
    /// </summary>
    private void redimNodeArrays(int newsize)
    {
        var len = newsize < Lo.Length ? newsize : Lo.Length;
        var na = new char[newsize];
        Array.Copy(Lo, 0, na, 0, len);
        Lo = na;
        na = new char[newsize];
        Array.Copy(Hi, 0, na, 0, len);
        Hi = na;
        na = new char[newsize];
        Array.Copy(Eq, 0, na, 0, len);
        Eq = na;
        na = new char[newsize];
        Array.Copy(Sc, 0, na, 0, len);
        Sc = na;
    }

    public class Iterator
    {
        /// <summary>
        ///     key stack implemented with a StringBuilder
        /// </summary>
        private readonly StringBuilder _ks;

        /// <summary>
        ///     Node stack
        /// </summary>
        private readonly Stack<Item> _ns;

        /// <summary>
        ///     TernaryTree parent
        /// </summary>
        private readonly TernaryTree _parent;

        /// <summary>
        ///     current node index
        /// </summary>
        private int _cur;

        /// <summary>
        ///     current key
        /// </summary>
        private string _curkey;

        public Iterator(TernaryTree parent)
        {
            _parent = parent;
            _cur = -1;
            _ns = new Stack<Item>();
            _ks = new StringBuilder();
            Rewind();
        }

        public char Value
        {
            get
            {
                if (_cur >= 0)
                {
                    return _parent.Eq[_cur];
                }

                return (char)0;
            }
        }

        public bool HasMoreElements() => _cur != -1;

        public object NextElement()
        {
            var res = _curkey;
            _cur = up();
            run();
            return res;
        }

        public void Rewind()
        {
            _ns.Clear();
            _ks.Length = 0;
            _cur = _parent.Root;
            run();
        }

        /// <summary>
        ///     traverse the tree to find next key
        /// </summary>
        private int run()
        {
            if (_cur == -1)
            {
                return -1;
            }

            var leaf = false;
            for (;;)
            {
                // first go down on low branch until leaf or compressed branch
                while (_cur != 0)
                {
                    if (_parent.Sc[_cur] == 0xFFFF)
                    {
                        leaf = true;
                        break;
                    }

                    _ns.Push(new Item((char)_cur, '\u0000'));
                    if (_parent.Sc[_cur] == 0)
                    {
                        leaf = true;
                        break;
                    }

                    _cur = _parent.Lo[_cur];
                }

                if (leaf)
                {
                    break;
                }

                // nothing found, go up one node and try again
                _cur = up();
                if (_cur == -1)
                {
                    return -1;
                }
            }

            // The current node should be a data node and
            // the key should be in the key stack (at least partially)
            var buf = new StringBuilder(_ks.ToString());
            if (_parent.Sc[_cur] == 0xFFFF)
            {
                int p = _parent.Lo[_cur];
                while (_parent.Kv[p] != 0)
                {
                    buf.Append(_parent.Kv[p++]);
                }
            }

            _curkey = buf.ToString();
            return 0;
        }

        /// <summary>
        ///     traverse upwards
        /// </summary>
        private int up()
        {
            var i = new Item();
            var res = 0;

            if (_ns.Count == 0)
            {
                return -1;
            }

            if (_cur != 0 && _parent.Sc[_cur] == 0)
            {
                return _parent.Lo[_cur];
            }

            var climb = true;

            while (climb)
            {
                i = _ns.Pop();
                i.Child++;
                switch (i.Child)
                {
                    case (char)1:
                        if (_parent.Sc[i.Parent] != 0)
                        {
                            res = _parent.Eq[i.Parent];
                            _ns.Push(i.Clone());
                            _ks.Append(_parent.Sc[i.Parent]);
                        }
                        else
                        {
                            i.Child++;
                            _ns.Push(i.Clone());
                            res = _parent.Hi[i.Parent];
                        }

                        climb = false;
                        break;

                    case (char)2:
                        res = _parent.Hi[i.Parent];
                        _ns.Push(i.Clone());
                        if (_ks.Length > 0)
                        {
                            _ks.Length = _ks.Length - 1; // pop
                        }

                        climb = false;
                        break;

                    default:
                        if (_ns.Count == 0)
                        {
                            return -1;
                        }

                        climb = true;
                        break;
                }
            }

            return res;
        }

        private class Item
        {
            internal readonly char Parent;
            internal char Child;

            public Item()
            {
                Parent = (char)0;
                Child = (char)0;
            }

            public Item(char p, char c)
            {
                Parent = p;
                Child = c;
            }

            public Item Clone() => new(Parent, Child);
        }
    }
}