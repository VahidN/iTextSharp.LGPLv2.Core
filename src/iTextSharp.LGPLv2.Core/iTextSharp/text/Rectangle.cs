using System.Text;
using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     A Rectangle is the representation of a geometric figure.
/// </summary>
public class Rectangle : Element, IElement
{
    /// <summary>
    ///     static membervariables (concerning the presence of borders)
    /// </summary>
    /// <summary> This represents one side of the border of the Rectangle. </summary>
    public const int BOTTOM_BORDER = 2;

    ///<summary> This represents a type of border. </summary>
    public const int BOX = TOP_BORDER + BOTTOM_BORDER + LEFT_BORDER + RIGHT_BORDER;

    ///<summary> This represents one side of the border of the Rectangle. </summary>
    public const int LEFT_BORDER = 4;

    ///<summary> This represents a rectangle without borders. </summary>
    public const int NO_BORDER = 0;

    ///<summary> This represents one side of the border of the Rectangle. </summary>
    public const int RIGHT_BORDER = 8;

    ///<summary> This represents one side of the border of the Rectangle. </summary>
    public const int TOP_BORDER = 1;

    ///<summary> This is the value that will be used as <VAR>undefined</VAR>. </summary>
    public const int UNDEFINED = -1;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> This is the color of the background of this rectangle. </summary>
    protected BaseColor backgroundColor;

    ///<summary> This represents the status of the 4 sides of the rectangle. </summary>
    protected int border = UNDEFINED;

    ///<summary> This is the color of the border of this rectangle. </summary>
    protected BaseColor borderColor;

    /// <summary>
    ///     The color of the bottom border of this rectangle.
    /// </summary>
    protected BaseColor borderColorBottom;

    /// <summary>
    ///     The color of the left border of this rectangle.
    /// </summary>
    protected BaseColor borderColorLeft;

    /// <summary>
    ///     The color of the right border of this rectangle.
    /// </summary>
    protected BaseColor borderColorRight;

    /// <summary>
    ///     The color of the top border of this rectangle.
    /// </summary>
    protected BaseColor borderColorTop;

    ///<summary> This is the width of the border around this rectangle. </summary>
    protected float borderWidth = UNDEFINED;

    /// <summary>
    ///     The width of the bottom border of this rectangle.
    /// </summary>
    protected float borderWidthBottom = UNDEFINED;

    /// <summary>
    ///     The width of the left border of this rectangle.
    /// </summary>
    protected float borderWidthLeft = UNDEFINED;

    /// <summary>
    ///     The width of the right border of this rectangle.
    /// </summary>
    protected float borderWidthRight = UNDEFINED;

    /// <summary>
    ///     The width of the top border of this rectangle.
    /// </summary>
    protected float borderWidthTop = UNDEFINED;

    ///<summary> the lower left x-coordinate. </summary>
    protected float Llx;

    ///<summary> the lower left y-coordinate. </summary>
    protected float Lly;

    ///<summary> This is the rotation value of this rectangle. </summary>
    protected int rotation;

    ///<summary> the upper right x-coordinate. </summary>
    protected float Urx;

    ///<summary> the upper right y-coordinate. </summary>
    protected float Ury;

    /// <summary>
    ///     Whether variable width borders are used.
    /// </summary>
    protected bool useVariableBorders;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a Rectangle-object.
    /// </summary>
    /// <param name="llx">lower left x</param>
    /// <param name="lly">lower left y</param>
    /// <param name="urx">upper right x</param>
    /// <param name="ury">upper right y</param>
    public Rectangle(float llx, float lly, float urx, float ury)
    {
        Llx = llx;
        Lly = lly;
        Urx = urx;
        Ury = ury;
    }

    /// <summary>
    ///     Constructs a Rectangle-object starting from the origin (0, 0).
    /// </summary>
    /// <param name="urx">upper right x</param>
    /// <param name="ury">upper right y</param>
    public Rectangle(float urx, float ury) : this(0, 0, urx, ury)
    {
    }

    /// <summary>
    ///     Constructs a Rectangle-object.
    /// </summary>
    /// <param name="rect">another Rectangle</param>
    public Rectangle(Rectangle rect) : this(rect?.Llx ?? throw new ArgumentNullException(nameof(rect)),
                                            rect.Lly, rect.Urx, rect.Ury)
    {
        CloneNonPositionParameters(rect);
    }

    /// <summary>
    ///     Gets the backgroundcolor.
    /// </summary>
    public virtual BaseColor BackgroundColor
    {
        get => backgroundColor;

        set => backgroundColor = value;
    }

    /// <summary>
    ///     Get/set the border
    /// </summary>
    /// <value>a int</value>
    public virtual int Border
    {
        get => border;

        set => border = value;
    }

    /// <summary>
    ///     Gets the color of the border.
    /// </summary>
    /// <returns>a value</returns>
    /// <summary>
    ///     Get/set the color of the border.
    /// </summary>
    /// <value>a Color</value>
    public virtual BaseColor BorderColor
    {
        get => borderColor;

        set => borderColor = value;
    }

    public virtual BaseColor BorderColorBottom
    {
        get
        {
            if (borderColorBottom == null)
            {
                return borderColor;
            }

            return borderColorBottom;
        }
        set => borderColorBottom = value;
    }

    public virtual BaseColor BorderColorLeft
    {
        get
        {
            if (borderColorLeft == null)
            {
                return borderColor;
            }

            return borderColorLeft;
        }
        set => borderColorLeft = value;
    }

    public virtual BaseColor BorderColorRight
    {
        get
        {
            if (borderColorRight == null)
            {
                return borderColor;
            }

            return borderColorRight;
        }
        set => borderColorRight = value;
    }

    public virtual BaseColor BorderColorTop
    {
        get
        {
            if (borderColorTop == null)
            {
                return borderColor;
            }

            return borderColorTop;
        }
        set => borderColorTop = value;
    }

    /// <summary>
    ///     Get/set the borderwidth.
    /// </summary>
    /// <value>a float</value>
    public virtual float BorderWidth
    {
        get => borderWidth;

        set => borderWidth = value;
    }

    public virtual float BorderWidthBottom
    {
        get => getVariableBorderWidth(borderWidthBottom, BOTTOM_BORDER);
        set
        {
            borderWidthBottom = value;
            updateBorderBasedOnWidth(value, BOTTOM_BORDER);
        }
    }

    public virtual float BorderWidthLeft
    {
        get => getVariableBorderWidth(borderWidthLeft, LEFT_BORDER);
        set
        {
            borderWidthLeft = value;
            updateBorderBasedOnWidth(value, LEFT_BORDER);
        }
    }

    public virtual float BorderWidthRight
    {
        get => getVariableBorderWidth(borderWidthRight, RIGHT_BORDER);
        set
        {
            borderWidthRight = value;
            updateBorderBasedOnWidth(value, RIGHT_BORDER);
        }
    }

    public virtual float BorderWidthTop
    {
        get => getVariableBorderWidth(borderWidthTop, TOP_BORDER);
        set
        {
            borderWidthTop = value;
            updateBorderBasedOnWidth(value, TOP_BORDER);
        }
    }

    /// <summary>
    ///     Get/set the lower left y-coordinate.
    /// </summary>
    /// <value>a float</value>
    public virtual float Bottom
    {
        get => Lly;
        set => Lly = value;
    }

    /// <summary>
    ///     Get/set the grayscale of the rectangle.
    /// </summary>
    /// <value>a float</value>
    public virtual float GrayFill
    {
        get
        {
            if (backgroundColor is GrayColor)
            {
                return ((GrayColor)backgroundColor).Gray;
            }

            return 0;
        }
        set => backgroundColor = new GrayColor(value);
    }

    /// <summary>
    ///     Returns the height of the rectangle.
    /// </summary>
    /// <value>a height</value>
    public float Height => Ury - Lly;

    /// <summary>
    ///     Get/set the lower left x-coordinate.
    /// </summary>
    /// <value>a float</value>
    public virtual float Left
    {
        get => Llx;

        set => Llx = value;
    }

    /// <summary>
    ///     methods to get the membervariables
    /// </summary>
    /// <summary>
    ///     Get/set the upper right x-coordinate.
    /// </summary>
    /// <value>a float</value>
    public virtual float Right
    {
        get => Urx;

        set => Urx = value;
    }

    /// <summary>
    ///     Returns the rotation
    /// </summary>
    /// <value>a int</value>
    public int Rotation => rotation;

    /// <summary>
    ///     Get/set the upper right y-coordinate.
    /// </summary>
    /// <value>a float</value>
    public virtual float Top
    {
        get => Ury;

        set => Ury = value;
    }

    /// <summary>
    ///     Sets a parameter indicating if the rectangle has variable borders
    ///     indication if the rectangle has variable borders
    /// </summary>
    public virtual bool UseVariableBorders
    {
        get => useVariableBorders;
        set => useVariableBorders = value;
    }

    /// <summary>
    ///     Returns the width of the rectangle.
    /// </summary>
    /// <value>a width</value>
    public virtual float Width
    {
        get => Urx - Llx;
        set => throw new InvalidOperationException("The width cannot be set.");
    }

    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public virtual IList<Chunk> Chunks => new List<Chunk>();

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public virtual int Type => RECTANGLE;

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public virtual bool IsNestable() => false;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     IElementListener.
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    public virtual bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            return listener.Add(this);
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     Updates the border flag for a side based on the specified width. A width
    ///     of 0 will disable the border on that side. Any other width enables it.
    ///     width of border
    ///     border side constant
    /// </summary>
    public override string ToString()
    {
        var buf = new StringBuilder("Rectangle: ");
        buf.Append(Width);
        buf.Append('x');
        buf.Append(Height);
        buf.Append(" (rot: ");
        buf.Append(rotation);
        buf.Append(" degrees)");
        return buf.ToString();
    }

    public virtual void CloneNonPositionParameters(Rectangle rect)
    {
        if (rect == null)
        {
            throw new ArgumentNullException(nameof(rect));
        }

        rotation = rect.rotation;
        border = rect.border;
        borderWidth = rect.borderWidth;
        borderColor = rect.borderColor;
        backgroundColor = rect.backgroundColor;
        borderColorLeft = rect.borderColorLeft;
        borderColorRight = rect.borderColorRight;
        borderColorTop = rect.borderColorTop;
        borderColorBottom = rect.borderColorBottom;
        borderWidthLeft = rect.borderWidthLeft;
        borderWidthRight = rect.borderWidthRight;
        borderWidthTop = rect.borderWidthTop;
        borderWidthBottom = rect.borderWidthBottom;
        useVariableBorders = rect.useVariableBorders;
    }


    /// <summary>
    ///     Disables the border on the specified side.
    ///     the side to disable. One of  LEFT, RIGHT, TOP, BOTTOM
    /// </summary>
    /// <param name="side"></param>
    public virtual void DisableBorderSide(int side)
    {
        if (border == UNDEFINED)
        {
            border = 0;
        }

        border &= ~side;
    }

    /// <summary>
    ///     methods to set the membervariables
    /// </summary>
    /// <summary>
    ///     Enables the border on the specified side.
    ///     the side to enable. One of  LEFT, RIGHT, TOP, BOTTOM
    /// </summary>
    /// <param name="side"></param>
    public virtual void EnableBorderSide(int side)
    {
        if (border == UNDEFINED)
        {
            border = 0;
        }

        border |= side;
    }

    /// <summary>
    ///     Returns the lower left y-coordinate, considering a given margin.
    /// </summary>
    /// <param name="margin">a margin</param>
    /// <returns>the lower left y-coordinate</returns>
    public virtual float GetBottom(float margin) => Lly + margin;

    /// <summary>
    ///     Returns the lower left x-coordinate, considering a given margin.
    /// </summary>
    /// <param name="margin">a margin</param>
    /// <returns>the lower left x-coordinate</returns>
    public virtual float GetLeft(float margin) => Llx + margin;

    /// <summary>
    ///     Gets a Rectangle that is altered to fit on the page.
    /// </summary>
    /// <param name="top">the top position</param>
    /// <param name="bottom">the bottom position</param>
    /// <returns>a Rectangle</returns>
    public Rectangle GetRectangle(float top, float bottom)
    {
        var tmp = new Rectangle(this);
        if (Top > top)
        {
            tmp.Top = top;
            tmp.Border = border - (border & TOP_BORDER);
        }

        if (Bottom < bottom)
        {
            tmp.Bottom = bottom;
            tmp.Border = border - (border & BOTTOM_BORDER);
        }

        return tmp;
    }

    /// <summary>
    ///     Returns the upper right x-coordinate, considering a given margin.
    /// </summary>
    /// <param name="margin">a margin</param>
    /// <returns>the upper right x-coordinate</returns>
    public virtual float GetRight(float margin) => Urx - margin;

    /// <summary>
    ///     Returns the upper right y-coordinate, considering a given margin.
    /// </summary>
    /// <param name="margin">a margin</param>
    /// <returns>the upper right y-coordinate</returns>
    public virtual float GetTop(float margin) => Ury - margin;

    /// <summary>
    ///     Indicates if the table has a some type of border.
    /// </summary>
    /// <param name="type">the type of border</param>
    /// <returns>a bool</returns>
    public bool HasBorder(int type)
    {
        if (border == UNDEFINED)
        {
            return false;
        }

        return (border & type) == type;
    }

    /// <summary>
    ///     Indicates if the table has borders.
    /// </summary>
    /// <returns>a bool</returns>
    public bool HasBorders()
    {
        switch (border)
        {
            case UNDEFINED:
            case NO_BORDER:
                return false;
            default:
                return borderWidth > 0 || borderWidthLeft > 0
                                       || borderWidthRight > 0 || borderWidthTop > 0 || borderWidthBottom > 0;
        }
    }

    /// <summary>
    ///     Switches lowerleft with upperright
    /// </summary>
    public virtual void Normalize()
    {
        if (Llx > Urx)
        {
            var a = Llx;
            Llx = Urx;
            Urx = a;
        }

        if (Lly > Ury)
        {
            var a = Lly;
            Lly = Ury;
            Ury = a;
        }
    }

    /// <summary>
    ///     implementation of the Element interface
    /// </summary>
    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Swaps the values of urx and ury and of lly and llx in order to rotate the rectangle.
    /// </summary>
    /// <returns>a Rectangle</returns>
    public Rectangle Rotate()
    {
        var rect = new Rectangle(Lly, Llx, Ury, Urx);
        rect.rotation = rotation + 90;
        rect.rotation %= 360;
        return rect;
    }

    public virtual void SoftCloneNonPositionParameters(Rectangle rect)
    {
        if (rect == null)
        {
            throw new ArgumentNullException(nameof(rect));
        }

        if (rect.rotation != 0)
        {
            rotation = rect.rotation;
        }

        if (rect.border != UNDEFINED)
        {
            border = rect.border;
        }

        if (rect.borderWidth.ApproxNotEqual(UNDEFINED))
        {
            borderWidth = rect.borderWidth;
        }

        if (rect.borderColor != null)
        {
            borderColor = rect.borderColor;
        }

        if (rect.backgroundColor != null)
        {
            backgroundColor = rect.backgroundColor;
        }

        if (rect.borderColorLeft != null)
        {
            borderColorLeft = rect.borderColorLeft;
        }

        if (rect.borderColorRight != null)
        {
            borderColorRight = rect.borderColorRight;
        }

        if (rect.borderColorTop != null)
        {
            borderColorTop = rect.borderColorTop;
        }

        if (rect.borderColorBottom != null)
        {
            borderColorBottom = rect.borderColorBottom;
        }

        if (rect.borderWidthLeft.ApproxNotEqual(UNDEFINED))
        {
            borderWidthLeft = rect.borderWidthLeft;
        }

        if (rect.borderWidthRight.ApproxNotEqual(UNDEFINED))
        {
            borderWidthRight = rect.borderWidthRight;
        }

        if (rect.borderWidthTop.ApproxNotEqual(UNDEFINED))
        {
            borderWidthTop = rect.borderWidthTop;
        }

        if (rect.borderWidthBottom.ApproxNotEqual(UNDEFINED))
        {
            borderWidthBottom = rect.borderWidthBottom;
        }

        if (useVariableBorders)
        {
            useVariableBorders = rect.useVariableBorders;
        }
    }

    private float getVariableBorderWidth(float variableWidthValue, int side)
    {
        if ((border & side) != 0)
        {
            return variableWidthValue.ApproxNotEqual(UNDEFINED) ? variableWidthValue : borderWidth;
        }

        return 0;
    }

    private void updateBorderBasedOnWidth(float width, int side)
    {
        useVariableBorders = true;
        if (width > 0)
        {
            EnableBorderSide(side);
        }
        else
        {
            DisableBorderSide(side);
        }
    }
}