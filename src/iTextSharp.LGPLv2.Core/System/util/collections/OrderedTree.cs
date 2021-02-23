using System.Collections;

namespace System.util.collections
{
    public class OrderedTree
    {
        /// <summary>
        /// sentinelNode is convenient way of indicating a leaf node.
        /// </summary>
        [ThreadStatic]
        private static OrderedTreeNode sentinelNode;

        /// <summary>
        /// the number of nodes contained in the tree
        /// </summary>
        private int _intCount;
        /// <summary>
        /// the node that was last found; used to optimize searches
        /// </summary>
        private OrderedTreeNode _lastNodeFound;

        /// <summary>
        /// the tree
        /// </summary>
        private OrderedTreeNode _rbTree;

        public static OrderedTreeNode SentinelNode
        {
            get
            {
                if (sentinelNode == null)
                {
                    // set up the sentinel node. the sentinel node is the key to a successfull
                    // implementation and for understanding the red-black tree properties.
                    sentinelNode = new OrderedTreeNode();
                    sentinelNode.Left = sentinelNode.Right = sentinelNode;
                    sentinelNode.Parent = null;
                    sentinelNode.Color = OrderedTreeNode.BLACK;
                }
                return sentinelNode;
            }
        }

        public OrderedTree()
        {
            _rbTree = SentinelNode;
            _lastNodeFound = SentinelNode;
        }

        public int Count
        {
            get
            {
                return _intCount;
            }
        }

        ///<summary>
        /// Keys
        /// if(ascending is true, the keys will be returned in ascending order, else
        /// the keys will be returned in descending order.
        ///</summary>
        public OrderedTreeEnumerator Keys
        {
            get
            {
                return KeyElements(true);
            }
        }

        ///<summary>
        /// Values
        /// Provided for .NET compatibility.
        ///</summary>
        public OrderedTreeEnumerator Values
        {
            get
            {
                return Elements(true);
            }
        }

        public object this[IComparable key]
        {
            get
            {
                return GetData(key);
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("Key is null");

                int result = 0;
                OrderedTreeNode node = new OrderedTreeNode();
                OrderedTreeNode temp = _rbTree;

                while (temp != SentinelNode)
                {
                    node.Parent = temp;
                    result = key.CompareTo(temp.Key);
                    if (result == 0)
                    {
                        _lastNodeFound = temp;
                        temp.Data = value;
                        return;
                    }
                    if (result > 0)
                        temp = temp.Right;
                    else
                        temp = temp.Left;
                }

                node.Key = key;
                node.Data = value;
                node.Left = SentinelNode;
                node.Right = SentinelNode;

                if (node.Parent != null)
                {
                    result = node.Key.CompareTo(node.Parent.Key);
                    if (result > 0)
                        node.Parent.Right = node;
                    else
                        node.Parent.Left = node;
                }
                else
                    _rbTree = node;

                restoreAfterInsert(node);

                _lastNodeFound = node;

                _intCount = _intCount + 1;
            }
        }

        ///<summary>
        /// Add
        /// args: ByVal key As IComparable, ByVal data As Object
        /// key is object that implements IComparable interface
        /// performance tip: change to use use int type (such as the hashcode)
        ///</summary>
        public void Add(IComparable key, object data)
        {
            if (key == null)
                throw (new ArgumentNullException("Key is null"));

            // traverse tree - find where node belongs
            int result = 0;
            // create new node
            OrderedTreeNode node = new OrderedTreeNode();
            OrderedTreeNode temp = _rbTree; // grab the rbTree node of the tree

            while (temp != SentinelNode)
            {
                // find Parent
                node.Parent = temp;
                result = key.CompareTo(temp.Key);
                if (result == 0)
                    throw new ArgumentException("Key duplicated");
                if (result > 0)
                    temp = temp.Right;
                else
                    temp = temp.Left;
            }

            // setup node
            node.Key = key;
            node.Data = data;
            node.Left = SentinelNode;
            node.Right = SentinelNode;

            // insert node into tree starting at parent's location
            if (node.Parent != null)
            {
                result = node.Key.CompareTo(node.Parent.Key);
                if (result > 0)
                    node.Parent.Right = node;
                else
                    node.Parent.Left = node;
            }
            else
                _rbTree = node; // first node added

            restoreAfterInsert(node); // restore red-black properities

            _lastNodeFound = node;

            _intCount = _intCount + 1;
        }
        ///<summary>
        /// Clear
        /// Empties or clears the tree
        ///</summary>
        public void Clear()
        {
            _rbTree = SentinelNode;
            _intCount = 0;
        }

        public bool ContainsKey(IComparable key)
        {
            OrderedTreeNode treeNode = _rbTree; // begin at root
            int result = 0;
            // traverse tree until node is found
            while (treeNode != SentinelNode)
            {
                result = key.CompareTo(treeNode.Key);
                if (result == 0)
                {
                    _lastNodeFound = treeNode;
                    return true;
                }
                if (result < 0)
                    treeNode = treeNode.Left;
                else
                    treeNode = treeNode.Right;
            }
            return false;
        }

        ///<summary>
        /// Elements
        /// Returns an enumeration of the data objects.
        /// if(ascending is true, the objects will be returned in ascending order,
        /// else the objects will be returned in descending order.
        ///</summary>
        public OrderedTreeEnumerator Elements()
        {
            return Elements(true);
        }

        public OrderedTreeEnumerator Elements(bool ascending)
        {
            return new OrderedTreeEnumerator(_rbTree, false, ascending);
        }

        ///<summary>
        /// GetData
        /// Gets the data object associated with the specified key
        ///</summary>
        public object GetData(IComparable key)
        {
            if (key == null)
                throw new ArgumentNullException("Key is null");
            int result;

            OrderedTreeNode treeNode = _rbTree; // begin at root

            // traverse tree until node is found
            while (treeNode != SentinelNode)
            {
                result = key.CompareTo(treeNode.Key);
                if (result == 0)
                {
                    _lastNodeFound = treeNode;
                    return treeNode.Data;
                }
                if (result < 0)
                    treeNode = treeNode.Left;
                else
                    treeNode = treeNode.Right;
            }
            return null;
        }

        ///<summary>
        /// GetEnumerator
        /// return an enumerator that returns the tree nodes in order
        ///</summary>
        public OrderedTreeEnumerator GetEnumerator()
        {
            // elements is simply a generic name to refer to the
            // data objects the nodes contain
            return Elements(true);
        }

        ///<summary>
        /// GetMaxKey
        /// Returns the maximum key value
        ///</summary>
        public IComparable GetMaxKey()
        {
            OrderedTreeNode treeNode = _rbTree;

            if (treeNode == null || treeNode == SentinelNode)
                throw (new InvalidOperationException("Tree is empty"));

            // traverse to the extreme right to find the largest key
            while (treeNode.Right != SentinelNode)
                treeNode = treeNode.Right;

            _lastNodeFound = treeNode;

            return treeNode.Key;

        }

        ///<summary>
        /// GetMaxValue
        /// Returns the object having the maximum key
        ///</summary>
        public object GetMaxValue()
        {
            return GetData(GetMaxKey());
        }

        ///<summary>
        /// GetMinKey
        /// Returns the minimum key value
        ///</summary>
        public IComparable GetMinKey()
        {
            OrderedTreeNode treeNode = _rbTree;

            if (treeNode == null || treeNode == SentinelNode)
                throw (new InvalidOperationException("Tree is empty"));

            // traverse to the extreme left to find the smallest key
            while (treeNode.Left != SentinelNode)
                treeNode = treeNode.Left;

            _lastNodeFound = treeNode;

            return treeNode.Key;

        }

        ///<summary>
        /// GetMinValue
        /// Returns the object having the minimum key value
        ///</summary>
        public object GetMinValue()
        {
            return GetData(GetMinKey());
        }

        ///<summary>
        /// IsEmpty
        /// Is the tree empty?
        ///</summary>
        public bool IsEmpty()
        {
            return (_rbTree == null || _rbTree == SentinelNode);
        }

        public OrderedTreeEnumerator KeyElements(bool ascending)
        {
            return new OrderedTreeEnumerator(_rbTree, true, ascending);
        }

        ///<summary>
        /// Remove
        /// removes the key and data object (delete)
        ///</summary>
        public void Remove(IComparable key)
        {
            if (key == null)
                throw new ArgumentNullException("Key is null");

            // find node
            int result;
            OrderedTreeNode node;

            // see if node to be deleted was the last one found
            result = key.CompareTo(_lastNodeFound.Key);
            if (result == 0)
                node = _lastNodeFound;
            else
            {
                // not found, must search
                node = _rbTree;
                while (node != SentinelNode)
                {
                    result = key.CompareTo(node.Key);
                    if (result == 0)
                        break;
                    if (result < 0)
                        node = node.Left;
                    else
                        node = node.Right;
                }

                if (node == SentinelNode)
                    return; // key not found
            }

            delete(node);

            _intCount = _intCount - 1;
        }

        ///<summary>
        /// RemoveMax
        /// removes the node with the maximum key
        ///</summary>
        public void RemoveMax()
        {
            if (_rbTree == null || _rbTree == SentinelNode)
                return;
            Remove(GetMaxKey());
        }

        ///<summary>
        /// RemoveMin
        /// removes the node with the minimum key
        ///</summary>
        public void RemoveMin()
        {
            if (_rbTree == null || _rbTree == SentinelNode)
                return;
            Remove(GetMinKey());
        }

        ///<summary>
        /// RotateLeft
        /// Rebalance the tree by rotating the nodes to the left
        ///</summary>
        public void RotateLeft(OrderedTreeNode x)
        {
            // pushing node x down and to the Left to balance the tree. x's Right child (y)
            // replaces x (since y > x), and y's Left child becomes x's Right child
            // (since it's < y but > x).

            OrderedTreeNode y = x.Right; // get x's Right node, this becomes y

            // set x's Right link
            x.Right = y.Left; // y's Left child's becomes x's Right child

            // modify parents
            if (y.Left != SentinelNode)
                y.Left.Parent = x; // sets y's Left Parent to x

            if (y != SentinelNode)
                y.Parent = x.Parent; // set y's Parent to x's Parent

            if (x.Parent != null)
            {
                // determine which side of it's Parent x was on
                if (x == x.Parent.Left)
                    x.Parent.Left = y; // set Left Parent to y
                else
                    x.Parent.Right = y; // set Right Parent to y
            }
            else
                _rbTree = y; // at rbTree, set it to y

            // link x and y
            y.Left = x; // put x on y's Left
            if (x != SentinelNode) // set y as x's Parent
                x.Parent = y;
        }

        ///<summary>
        /// RotateRight
        /// Rebalance the tree by rotating the nodes to the right
        ///</summary>
        public void RotateRight(OrderedTreeNode x)
        {
            // pushing node x down and to the Right to balance the tree. x's Left child (y)
            // replaces x (since x < y), and y's Right child becomes x's Left child
            // (since it's < x but > y).

            OrderedTreeNode y = x.Left; // get x's Left node, this becomes y

            // set x's Right link
            x.Left = y.Right; // y's Right child becomes x's Left child

            // modify parents
            if (y.Right != SentinelNode)
                y.Right.Parent = x; // sets y's Right Parent to x

            if (y != SentinelNode)
                y.Parent = x.Parent; // set y's Parent to x's Parent

            if (x.Parent != null)
            { // null=rbTree, could also have used rbTree
                // determine which side of it's Parent x was on
                if (x == x.Parent.Right)
                    x.Parent.Right = y; // set Right Parent to y
                else
                    x.Parent.Left = y; // set Left Parent to y
            }
            else
                _rbTree = y; // at rbTree, set it to y

            // link x and y
            y.Right = x; // put x on y's Right
            if (x != SentinelNode) // set y as x's Parent
                x.Parent = y;
        }

        ///<summary>
        /// Delete
        /// Delete a node from the tree and restore red black properties
        ///</summary>
        private void delete(OrderedTreeNode z)
        {
            // A node to be deleted will be:
            // 1. a leaf with no children
            // 2. have one child
            // 3. have two children
            // If the deleted node is red, the red black properties still hold.
            // If the deleted node is black, the tree needs rebalancing

            OrderedTreeNode x = new OrderedTreeNode(); // work node to contain the replacement node
            OrderedTreeNode y; // work node

            // find the replacement node (the successor to x) - the node one with
            // at *most* one child.
            if (z.Left == SentinelNode || z.Right == SentinelNode)
                y = z; // node has sentinel as a child
            else
            {
                // z has two children, find replacement node which will
                // be the leftmost node greater than z
                y = z.Right; // traverse right subtree
                while (y.Left != SentinelNode) // to find next node in sequence
                    y = y.Left;
            }

            // at this point, y contains the replacement node. it's content will be copied
            // to the valules in the node to be deleted

            // x (y's only child) is the node that will be linked to y's old parent.
            if (y.Left != SentinelNode)
                x = y.Left;
            else
                x = y.Right;

            // replace x's parent with y's parent and
            // link x to proper subtree in parent
            // this removes y from the chain
            x.Parent = y.Parent;
            if (y.Parent != null)
                if (y == y.Parent.Left)
                    y.Parent.Left = x;
                else
                    y.Parent.Right = x;
            else
                _rbTree = x; // make x the root node

            // copy the values from y (the replacement node) to the node being deleted.
            // note: this effectively deletes the node.
            if (y != z)
            {
                z.Key = y.Key;
                z.Data = y.Data;
            }

            if (y.Color == OrderedTreeNode.BLACK)
                restoreAfterDelete(x);

            _lastNodeFound = SentinelNode;
        }

        ///<summary>
        /// RestoreAfterDelete
        /// Deletions from red-black trees may destroy the red-black
        /// properties. Examine the tree and restore. Rotations are normally
        /// required to restore it
        ///</summary>
        private void restoreAfterDelete(OrderedTreeNode x)
        {
            // maintain Red-Black tree balance after deleting node

            OrderedTreeNode y;

            while (x != _rbTree && x.Color == OrderedTreeNode.BLACK)
            {
                if (x == x.Parent.Left)
                { // determine sub tree from parent
                    y = x.Parent.Right; // y is x's sibling
                    if (y.Color == OrderedTreeNode.RED)
                    {
                        // x is black, y is red - make both black and rotate
                        y.Color = OrderedTreeNode.BLACK;
                        x.Parent.Color = OrderedTreeNode.RED;
                        RotateLeft(x.Parent);
                        y = x.Parent.Right;
                    }
                    if (y.Left.Color == OrderedTreeNode.BLACK &&
                        y.Right.Color == OrderedTreeNode.BLACK)
                    {
                        // children are both black
                        y.Color = OrderedTreeNode.RED; // change parent to red
                        x = x.Parent; // move up the tree
                    }
                    else
                    {
                        if (y.Right.Color == OrderedTreeNode.BLACK)
                        {
                            y.Left.Color = OrderedTreeNode.BLACK;
                            y.Color = OrderedTreeNode.RED;
                            RotateRight(y);
                            y = x.Parent.Right;
                        }
                        y.Color = x.Parent.Color;
                        x.Parent.Color = OrderedTreeNode.BLACK;
                        y.Right.Color = OrderedTreeNode.BLACK;
                        RotateLeft(x.Parent);
                        x = _rbTree;
                    }
                }
                else
                {
                    // right subtree - same as code above with right and left swapped
                    y = x.Parent.Left;
                    if (y.Color == OrderedTreeNode.RED)
                    {
                        y.Color = OrderedTreeNode.BLACK;
                        x.Parent.Color = OrderedTreeNode.RED;
                        RotateRight(x.Parent);
                        y = x.Parent.Left;
                    }
                    if (y.Right.Color == OrderedTreeNode.BLACK &&
                        y.Left.Color == OrderedTreeNode.BLACK)
                    {
                        y.Color = OrderedTreeNode.RED;
                        x = x.Parent;
                    }
                    else
                    {
                        if (y.Left.Color == OrderedTreeNode.BLACK)
                        {
                            y.Right.Color = OrderedTreeNode.BLACK;
                            y.Color = OrderedTreeNode.RED;
                            RotateLeft(y);
                            y = x.Parent.Left;
                        }
                        y.Color = x.Parent.Color;
                        x.Parent.Color = OrderedTreeNode.BLACK;
                        y.Left.Color = OrderedTreeNode.BLACK;
                        RotateRight(x.Parent);
                        x = _rbTree;
                    }
                }
            }
            x.Color = OrderedTreeNode.BLACK;
        }

        ///<summary>
        /// RestoreAfterInsert
        /// Additions to red-black trees usually destroy the red-black
        /// properties. Examine the tree and restore. Rotations are normally
        /// required to restore it
        ///</summary>
        private void restoreAfterInsert(OrderedTreeNode x)
        {
            // x and y are used as variable names for brevity, in a more formal
            // implementation, you should probably change the names

            OrderedTreeNode y;

            // maintain red-black tree properties after adding x
            while (x != _rbTree && x.Parent.Color == OrderedTreeNode.RED)
            {
                // Parent node is .Colored red;
                if (x.Parent == x.Parent.Parent.Left)
                { // determine traversal path
                  // is it on the Left or Right subtree?
                    y = x.Parent.Parent.Right; // get uncle
                    if (y != null && y.Color == OrderedTreeNode.RED)
                    {
                        // uncle is red; change x's Parent and uncle to black
                        x.Parent.Color = OrderedTreeNode.BLACK;
                        y.Color = OrderedTreeNode.BLACK;
                        // grandparent must be red. Why? Every red node that is not
                        // a leaf has only black children
                        x.Parent.Parent.Color = OrderedTreeNode.RED;
                        x = x.Parent.Parent; // continue loop with grandparent
                    }
                    else
                    {
                        // uncle is black; determine if x is greater than Parent
                        if (x == x.Parent.Right)
                        {
                            // yes, x is greater than Parent; rotate Left
                            // make x a Left child
                            x = x.Parent;
                            RotateLeft(x);
                        }
                        // no, x is less than Parent
                        x.Parent.Color = OrderedTreeNode.BLACK; // make Parent black
                        x.Parent.Parent.Color = OrderedTreeNode.RED; // make grandparent black
                        RotateRight(x.Parent.Parent); // rotate right
                    }
                }
                else
                {
                    // x's Parent is on the Right subtree
                    // this code is the same as above with "Left" and "Right" swapped
                    y = x.Parent.Parent.Left;
                    if (y != null && y.Color == OrderedTreeNode.RED)
                    {
                        x.Parent.Color = OrderedTreeNode.BLACK;
                        y.Color = OrderedTreeNode.BLACK;
                        x.Parent.Parent.Color = OrderedTreeNode.RED;
                        x = x.Parent.Parent;
                    }
                    else
                    {
                        if (x == x.Parent.Left)
                        {
                            x = x.Parent;
                            RotateRight(x);
                        }
                        x.Parent.Color = OrderedTreeNode.BLACK;
                        x.Parent.Parent.Color = OrderedTreeNode.RED;
                        RotateLeft(x.Parent.Parent);
                    }
                }
            }
            _rbTree.Color = OrderedTreeNode.BLACK; // rbTree should always be black
        }
    }

    public class OrderedTreeEnumerator : IEnumerator
    {
        /// <summary>
        /// return in ascending order (true) or descending (false)
        /// </summary>
        private readonly bool _ascending;

        /// <summary>
        /// return the keys
        /// </summary>
        private readonly bool _keys;

        /// <summary>
        /// the treap uses the stack to order the nodes
        /// </summary>
        private readonly Stack _stack;
        /// <summary>
        /// the data or value associated with the key
        /// </summary>
        private object _objValue;

        /// <summary>
        /// key
        /// </summary>
        private IComparable _ordKey;

        bool _pre = true;
        private OrderedTreeNode _tnode;
        ///<summary>
        /// Determine order, walk the tree and push the nodes onto the stack
        ///</summary>
        public OrderedTreeEnumerator(OrderedTreeNode tnode, bool keys, bool ascending)
        {

            _stack = new Stack();
            _keys = keys;
            _ascending = ascending;
            _tnode = tnode;
            Reset();
        }

        private OrderedTreeEnumerator()
        {
        }

        public object Current
        {
            get
            {
                if (_pre)
                    throw new InvalidOperationException("Current");
                return _keys ? Key : Value;
            }
        }

        ///<summary>
        ///Key
        ///</summary>
        public IComparable Key
        {
            get
            {
                return _ordKey;
            }

            set
            {
                _ordKey = value;
            }
        }
        ///<summary>
        ///Data
        ///</summary>
        public object Value
        {
            get
            {
                return _objValue;
            }

            set
            {
                _objValue = value;
            }
        }
        public OrderedTreeEnumerator GetEnumerator()
        {
            return this;
        }

        ///<summary>
        /// HasMoreElements
        ///</summary>
        public bool HasMoreElements()
        {
            return (_stack.Count > 0);
        }

        ///<summary>
        /// MoveNext
        /// For .NET compatibility
        ///</summary>
        public bool MoveNext()
        {
            if (HasMoreElements())
            {
                NextElement();
                _pre = false;
                return true;
            }
            _pre = true;
            return false;
        }

        ///<summary>
        /// NextElement
        ///</summary>
        public object NextElement()
        {
            if (_stack.Count == 0)

                throw new InvalidOperationException("Element not found");

            // the top of stack will always have the next item
            // get top of stack but don't remove it as the next nodes in sequence
            // may be pushed onto the top
            // the stack will be popped after all the nodes have been returned
            OrderedTreeNode node = (OrderedTreeNode)_stack.Peek(); //next node in sequence

            if (_ascending)
            {
                if (node.Right == OrderedTree.SentinelNode)
                {
                    // yes, top node is lowest node in subtree - pop node off stack
                    OrderedTreeNode tn = (OrderedTreeNode)_stack.Pop();
                    // peek at right node's parent
                    // get rid of it if it has already been used
                    while (HasMoreElements() && ((OrderedTreeNode)_stack.Peek()).Right == tn)
                        tn = (OrderedTreeNode)_stack.Pop();
                }
                else
                {
                    // find the next items in the sequence
                    // traverse to left; find lowest and push onto stack
                    OrderedTreeNode tn = node.Right;
                    while (tn != OrderedTree.SentinelNode)
                    {
                        _stack.Push(tn);
                        tn = tn.Left;
                    }
                }
            }
            else
            { // descending, same comments as above apply
                if (node.Left == OrderedTree.SentinelNode)
                {
                    // walk the tree
                    OrderedTreeNode tn = (OrderedTreeNode)_stack.Pop();
                    while (HasMoreElements() && ((OrderedTreeNode)_stack.Peek()).Left == tn)
                        tn = (OrderedTreeNode)_stack.Pop();
                }
                else
                {
                    // determine next node in sequence
                    // traverse to left subtree and find greatest node - push onto stack
                    OrderedTreeNode tn = node.Left;
                    while (tn != OrderedTree.SentinelNode)
                    {
                        _stack.Push(tn);
                        tn = tn.Right;
                    }
                }
            }

            // the following is for .NET compatibility (see MoveNext())
            Key = node.Key;
            Value = node.Data;
            // ******** testing only ********

            return _keys ? node.Key : node.Data;
        }

        public void Reset()
        {
            _pre = true;
            _stack.Clear();
            // use depth-first traversal to push nodes into stack
            // the lowest node will be at the top of the stack
            if (_ascending)
            {
                // find the lowest node
                while (_tnode != OrderedTree.SentinelNode)
                {
                    _stack.Push(_tnode);
                    _tnode = _tnode.Left;
                }
            }
            else
            {
                // the highest node will be at top of stack
                while (_tnode != OrderedTree.SentinelNode)
                {
                    _stack.Push(_tnode);
                    _tnode = _tnode.Right;
                }
            }
        }
    }

    public class OrderedTreeNode
    {
        public const bool BLACK = true;

        /// <summary>
        /// tree node colors
        /// </summary>
        public const bool RED = false;
        /// <summary>
        /// color - used to balance the tree
        /// </summary>
        private bool _intColor;

        /// <summary>
        /// the data or value associated with the key
        /// </summary>
        private object _objData;

        /// <summary>
        /// key provided by the calling class
        /// </summary>
        private IComparable _ordKey;
        /// <summary>
        /// left node
        /// </summary>
        private OrderedTreeNode _rbnLeft;
        /// <summary>
        /// parent node
        /// </summary>
        private OrderedTreeNode _rbnParent;

        /// <summary>
        /// right node
        /// </summary>
        private OrderedTreeNode _rbnRight;
        public OrderedTreeNode()
        {
            Color = RED;
        }

        ///<summary>
        ///Color
        ///</summary>
        public bool Color
        {
            get
            {
                return _intColor;
            }

            set
            {
                _intColor = value;
            }
        }

        ///<summary>
        ///Data
        ///</summary>
        public object Data
        {
            get
            {
                return _objData;
            }

            set
            {
                _objData = value;
            }
        }

        ///<summary>
        ///Key
        ///</summary>
        public IComparable Key
        {
            get
            {
                return _ordKey;
            }

            set
            {
                _ordKey = value;
            }
        }
        ///<summary>
        ///Left
        ///</summary>
        public OrderedTreeNode Left
        {
            get
            {
                return _rbnLeft;
            }

            set
            {
                _rbnLeft = value;
            }
        }
        public OrderedTreeNode Parent
        {
            get
            {
                return _rbnParent;
            }

            set
            {
                _rbnParent = value;
            }
        }

        ///<summary>
        /// Right
        ///</summary>
        public OrderedTreeNode Right
        {
            get
            {
                return _rbnRight;
            }

            set
            {
                _rbnRight = value;
            }
        }
    }
}
