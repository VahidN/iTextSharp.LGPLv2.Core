using System.Text;

namespace iTextSharp.text;

/// <summary>
///     A RectangleReadOnly is the representation of a geometric figure.
///     It's the same as a Rectangle but immutable.
/// </summary>
public class RectangleReadOnly : Rectangle
{
    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a RectangleReadOnly-object.
    /// </summary>
    /// <param name="llx">lower left x</param>
    /// <param name="lly">lower left y</param>
    /// <param name="urx">upper right x</param>
    /// <param name="ury">upper right y</param>
    public RectangleReadOnly(float llx, float lly, float urx, float ury) : base(llx, lly, urx, ury)
    {
    }

    /// <summary>
    ///     Constructs a RectangleReadOnly-object starting from the origin (0, 0).
    /// </summary>
    /// <param name="urx">upper right x</param>
    /// <param name="ury">upper right y</param>
    public RectangleReadOnly(float urx, float ury) : base(0, 0, urx, ury)
    {
    }

    /// <summary>
    ///     Constructs a RectangleReadOnly-object.
    /// </summary>
    /// <param name="rect">another Rectangle</param>
    public RectangleReadOnly(Rectangle rect) : base(rect?.Left ?? throw new ArgumentNullException(nameof(rect)),
                                                    rect.Bottom, rect.Right, rect.Top)
    {
        base.CloneNonPositionParameters(rect);
    }

    /// <summary>
    ///     Gets the backgroundcolor.
    /// </summary>
    /// <returns>a value</returns>
    /// <summary>
    ///     Get/set the backgroundcolor.
    /// </summary>
    /// <value>a Color</value>
    public override BaseColor BackgroundColor
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Get/set the border
    /// </summary>
    /// <value>a int</value>
    public override int Border
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Gets the color of the border.
    /// </summary>
    /// <returns>a value</returns>
    /// <summary>
    ///     Get/set the color of the border.
    /// </summary>
    /// <value>a Color</value>
    public override BaseColor BorderColor
    {
        set => throwReadOnlyError();
    }

    public override BaseColor BorderColorBottom
    {
        set => throwReadOnlyError();
    }

    public override BaseColor BorderColorLeft
    {
        set => throwReadOnlyError();
    }

    public override BaseColor BorderColorRight
    {
        set => throwReadOnlyError();
    }

    public override BaseColor BorderColorTop
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Get/set the borderwidth.
    /// </summary>
    /// <value>a float</value>
    public override float BorderWidth
    {
        set => throwReadOnlyError();
    }

    public override float BorderWidthBottom
    {
        set => throwReadOnlyError();
    }

    public override float BorderWidthLeft
    {
        set => throwReadOnlyError();
    }

    public override float BorderWidthRight
    {
        set => throwReadOnlyError();
    }

    public override float BorderWidthTop
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Get/set the lower left y-coordinate.
    /// </summary>
    /// <value>a float</value>
    public override float Bottom
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Get/set the grayscale of the rectangle.
    /// </summary>
    /// <value>a float</value>
    public override float GrayFill
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Get/set the lower left x-coordinate.
    /// </summary>
    /// <value>a float</value>
    public override float Left
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     methods to get the membervariables
    /// </summary>
    /// <summary>
    ///     Get/set the upper right x-coordinate.
    /// </summary>
    /// <value>a float</value>
    public override float Right
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Get/set the upper right y-coordinate.
    /// </summary>
    /// <value>a float</value>
    public override float Top
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Sets a parameter indicating if the rectangle has variable borders
    ///     indication if the rectangle has variable borders
    /// </summary>
    public override bool UseVariableBorders
    {
        set => throwReadOnlyError();
    }

    /// <summary>
    ///     Copies all of the parameters from a  Rectangle  object
    ///     except the position.
    ///     Rectangle  to copy from
    /// </summary>
    /// <param name="rect"></param>
    public override void CloneNonPositionParameters(Rectangle rect)
    {
        throwReadOnlyError();
    }

    /// <summary>
    ///     Disables the border on the specified side.
    ///     the side to disable. One of  LEFT, RIGHT, TOP, BOTTOM
    /// </summary>
    /// <param name="side"></param>
    public override void DisableBorderSide(int side)
    {
        throwReadOnlyError();
    }

    /// <summary>
    ///     methods to set the membervariables
    /// </summary>
    /// <summary>
    ///     Enables the border on the specified side.
    ///     the side to enable. One of  LEFT, RIGHT, TOP, BOTTOM
    /// </summary>
    /// <param name="side"></param>
    public override void EnableBorderSide(int side)
    {
        throwReadOnlyError();
    }

    /// <summary>
    ///     Switches lowerleft with upperright
    /// </summary>
    public override void Normalize()
    {
        throwReadOnlyError();
    }

    public override void SoftCloneNonPositionParameters(Rectangle rect)
    {
        throwReadOnlyError();
    }

    /// <summary>
    ///     Copies all of the parameters from a  Rectangle  object
    ///     except the position.
    ///     Rectangle  to copy from
    /// </summary>
    public override string ToString()
    {
        var buf = new StringBuilder("RectangleReadOnly: ");
        buf.Append(Width);
        buf.Append('x');
        buf.Append(Height);
        buf.Append(" (rot: ");
        buf.Append(rotation);
        buf.Append(" degrees)");
        return buf.ToString();
    }

    private static void throwReadOnlyError()
    {
        throw new InvalidOperationException("RectangleReadOnly: this Rectangle is read only.");
    }
}