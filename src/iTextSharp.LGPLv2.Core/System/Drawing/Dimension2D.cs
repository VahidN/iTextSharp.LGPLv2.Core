namespace System.Drawing;

/// <summary>
///     The  Dimension2D  class is to encapsulate a width
///     and a height dimension.
/// </summary>
/// <remarks>
///     This class is only the abstract baseclass for all objects that
///     store a 2D dimension.
///     The actual storage representation of the sizes is left to
///     the subclass.
/// </remarks>
public abstract class Dimension2D : ICloneable
{
    /// <summary>
    ///     Returns the height of this  Dimension  in double
    ///     precision.
    /// </summary>
    /// <value>the height</value>
    public abstract double Height { get; }

    /// <summary>
    ///     Sets the size of this  Dimension2D  object to
    ///     match the specified size.
    /// </summary>
    /// <value>the size</value>
    public Dimension2D Size
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            SetSize(value.Width, value.Height);
        }
    }

    /// <summary>
    ///     Returns the width of this  Dimension  in double
    ///     precision.
    /// </summary>
    /// <value>the width</value>
    public abstract double Width { get; }

    /// <summary>
    ///     Creates a new object of the same class as this object.
    /// </summary>
    /// <returns>a clone of this instance</returns>
    public object Clone() => throw new NotSupportedException("not implemented");

    /// <summary>
    ///     Sets the size of this  Dimension  object to the
    ///     specified width and height.
    /// </summary>
    /// <param name="width">
    ///     the new width for the  Dimension
    ///     object
    /// </param>
    /// <param name="height">
    ///     the new height for the  Dimension
    ///     object
    /// </param>
    public abstract void SetSize(double width, double height);
}