namespace System.util.collections;

public class OrderedTreeNode
{
    // tree node colors
    public const bool RED = false;
    public const bool BLACK = true;

    public OrderedTreeNode() => Color = RED;

    /// <summary>
    ///     Key
    /// </summary>
    public virtual IComparable Key { get; set; }

    /// <summary>
    ///     Data
    /// </summary>
    public virtual object Data { get; set; }

    /// <summary>
    ///     Color
    /// </summary>
    public virtual bool Color { get; set; }

    /// <summary>
    ///     Left
    /// </summary>
    public virtual OrderedTreeNode Left { get; set; }

    /// <summary>
    ///     Right
    /// </summary>
    public virtual OrderedTreeNode Right { get; set; }

    public virtual OrderedTreeNode Parent { get; set; }
}