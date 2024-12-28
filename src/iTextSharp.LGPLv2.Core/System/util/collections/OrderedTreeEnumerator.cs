namespace System.util.collections;

public class OrderedTreeEnumerator : IEnumerator<object>
{
    // return in ascending order (true) or descending (false)
    private readonly bool _ascending;

    // return the keys
    private readonly bool _keys;

    private readonly OrderedTreeNode _sentinelNode;

    // the treap uses the stack to order the nodes
    private readonly Stack<OrderedTreeNode> _stack;

    private bool _pre = true;

    private OrderedTreeNode _tNode;

    private OrderedTreeEnumerator()
    {
    }

    /// <summary>
    ///     Determine order, walk the tree and push the nodes onto the stack
    /// </summary>
    public OrderedTreeEnumerator(OrderedTreeNode tNode, bool keys, bool ascending, OrderedTreeNode sentinelNode)
    {
        _sentinelNode = sentinelNode;
        _stack = new Stack<OrderedTreeNode>();
        _keys = keys;
        _ascending = ascending;
        _tNode = tNode;
        Reset();
    }

    /// <summary>
    ///     Key
    /// </summary>
    public virtual IComparable Key { get; set; }

    /// <summary>
    ///     Data
    /// </summary>
    public virtual object Value { get; set; }

    public virtual void Reset()
    {
        _pre = true;
        _stack.Clear();

        // use depth-first traversal to push nodes into stack
        // the lowest node will be at the top of the stack
        if (_ascending)
        {
            // find the lowest node
            while (_tNode != _sentinelNode)
            {
                _stack.Push(_tNode);
                _tNode = _tNode.Left;
            }
        }
        else
        {
            // the highest node will be at top of stack
            while (_tNode != _sentinelNode)
            {
                _stack.Push(_tNode);
                _tNode = _tNode.Right;
            }
        }
    }

    public virtual object Current
    {
        get
        {
            if (_pre)
            {
                throw new InvalidOperationException(message: "Current");
            }

            return _keys ? Key : Value;
        }
    }

    /// <summary>
    ///     MoveNext
    ///     For .NET compatibility
    /// </summary>
    public virtual bool MoveNext()
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

    public void Dispose()
    {
    }

    /// <summary>
    ///     HasMoreElements
    /// </summary>
    public virtual bool HasMoreElements() => _stack.Count > 0;

    /// <summary>
    ///     NextElement
    /// </summary>
    public virtual object NextElement()
    {
        if (_stack.Count == 0)
        {
            throw new InvalidOperationException(message: "Element not found");
        }

        // the top of stack will always have the next item
        // get top of stack but don't remove it as the next nodes in sequence
        // may be pushed onto the top
        // the stack will be popped after all the nodes have been returned
        var node = _stack.Peek(); //next node in sequence

        if (_ascending)
        {
            if (node.Right == _sentinelNode)
            {
                // yes, top node is lowest node in subtree - pop node off stack
                var tn = _stack.Pop();

                // peek at right node's parent
                // get rid of it if it has already been used
                while (HasMoreElements() && _stack.Peek().Right == tn)
                {
                    tn = _stack.Pop();
                }
            }
            else
            {
                // find the next items in the sequence
                // traverse to left; find lowest and push onto stack
                var tn = node.Right;

                while (tn != _sentinelNode)
                {
                    _stack.Push(tn);
                    tn = tn.Left;
                }
            }
        }
        else
        {
            // descending, same comments as above apply
            if (node.Left == _sentinelNode)
            {
                // walk the tree
                var tn = _stack.Pop();

                while (HasMoreElements() && _stack.Peek().Left == tn)
                {
                    tn = _stack.Pop();
                }
            }
            else
            {
                // determine next node in sequence
                // traverse to left subtree and find greatest node - push onto stack
                var tn = node.Left;

                while (tn != _sentinelNode)
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

    public virtual OrderedTreeEnumerator GetEnumerator() => this;
}