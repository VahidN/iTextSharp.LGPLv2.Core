namespace System.Drawing;

/// <summary>
///     The  Dimension  class encapsulates the width and
///     height of a component (in int precision) in a single object.
/// </summary>
/// <remarks>
///     The class is
///     associated with certain properties of components. Several methods
///     defined by the  Component  class and the
///     LayoutManager  interface return a  Dimension  object.
///     Normally the values of  width
///     and  height  are non-negative ints.
///     The constructors that allow you to create a dimension do
///     not prevent you from setting a negative value for these properties.
///     If the value of  width  or  height  is
///     negative, the behavior of some methods defined by other objects is
///     undefined.
/// </remarks>
public class Dimension : Dimension2D
{
    /// <summary>
    ///     Creates an instance of  Dimension  with a width
    ///     of zero and a height of zero.
    /// </summary>
    public Dimension() : this(0, 0)
    {
    }

    /// <summary>
    ///     Creates an instance of  Dimension  whose width
    ///     and height are the same as for the specified dimension.
    /// </summary>
    /// <param name="d">
    ///     the specified dimension for the
    ///     width  and
    ///     height  values.
    /// </param>
    public Dimension(Dimension d) : this(d?.width ?? throw new ArgumentNullException(nameof(d)), d.height)
    {
    }

    /// <summary>
    ///     Constructs a Dimension and initializes it to the specified width and
    ///     specified height.
    /// </summary>
    /// <param name="width">the specified width dimension</param>
    /// <param name="height">the specified height dimension</param>
    public Dimension(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    /// <summary>
    ///     The height dimension. Negative values can be used.
    /// </summary>
    public int height { set; get; }

    /// <summary>
    ///     The width dimension. Negative values can be used.
    /// </summary>
    public int width { set; get; }

    /// <summary>
    ///     Returns the height of this dimension in double precision.
    /// </summary>
    /// <value>the height</value>
    public override double Height => height;

    /// <summary>
    ///     Get/set the size of this  Dimension  object.
    /// </summary>
    /// <value>the size</value>
    public new Dimension Size
    {
        get => new(width, height);

        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            SetSize(value.width, value.height);
        }
    }

    /// <summary>
    ///     Returns the width of this dimension in double precision.
    /// </summary>
    /// <value>the width</value>
    public override double Width => width;

    /// <summary>
    ///     Checks whether two dimension objects have equal values.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (obj is Dimension)
        {
            var d = (Dimension)obj;
            return width == d.width && height == d.height;
        }

        return false;
    }

    /// <summary>
    ///     Returns the hash code for this Dimension.
    /// </summary>
    /// <returns>a hash code</returns>
    public override int GetHashCode()
    {
        var sum = width + height;
        return sum * (sum + 1) / 2 + width;
    }

    /// <summary>
    ///     Set the size of this Dimension object to the specified width
    ///     and height in double precision.
    /// </summary>
    /// <param name="width">the new width for the Dimension object</param>
    /// <param name="height">the new height for the Dimension object</param>
    public override void SetSize(double width, double height)
    {
        this.width = (int)Math.Ceiling(width);
        this.height = (int)Math.Ceiling(height);
    }

    /// <summary>
    ///     Set the size of this  Dimension  object
    ///     to the specified width and height.
    /// </summary>
    /// <param name="width">the new width for this  Dimension  object.</param>
    /// <param name="height">the new height for this  Dimension  object.</param>
    public void SetSize(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    /// <summary>
    ///     Returns a string representation of the values of this
    ///     Dimension  object's  height  and
    ///     width  fields.
    /// </summary>
    /// <remarks>
    ///     This method is intended to be used only
    ///     for debugging purposes, and the content and format of the returned
    ///     string may vary between implementations. The returned string may be
    ///     empty but may not be  null .
    /// </remarks>
    /// <returns>
    ///     a string representation of this  Dimension
    ///     object.
    /// </returns>
    public override string ToString() => $"{GetType().Name}[width={width},height={height}]";
}