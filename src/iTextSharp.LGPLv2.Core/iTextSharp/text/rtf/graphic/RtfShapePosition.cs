namespace iTextSharp.text.rtf.graphic;

/// <summary>
///     The RtfShapePosition stores position and ordering
///     information for one RtfShape.
///     @version $Revision: 1.6 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfShapePosition : RtfAddableElement
{
    /// <summary>
    ///     Constant for horizontal positioning relative to the column.
    /// </summary>
    public const int POSITION_X_RELATIVE_COLUMN = 2;

    /// <summary>
    ///     Constant for horizontal positioning relative to the margin.
    /// </summary>
    public const int POSITION_X_RELATIVE_MARGIN = 1;

    /// <summary>
    ///     Constant for horizontal positioning relative to the page.
    /// </summary>
    public const int POSITION_X_RELATIVE_PAGE = 0;

    /// <summary>
    ///     Constant for vertical positioning relative to the margin.
    /// </summary>
    public const int POSITION_Y_RELATIVE_MARGIN = 1;

    /// <summary>
    ///     Constant for vertical positioning relative to the page.
    /// </summary>
    public const int POSITION_Y_RELATIVE_PAGE = 0;

    /// <summary>
    ///     Constant for vertical positioning relative to the paragraph.
    /// </summary>
    public const int POSITION_Y_RELATIVE_PARAGRAPH = 2;

    /// <summary>
    ///     The bottom coordinate of this RtfShapePosition.
    /// </summary>
    private readonly int _bottom;

    /// <summary>
    ///     The left coordinate of this RtfShapePosition.
    /// </summary>
    private readonly int _left;

    /// <summary>
    ///     The right coordinate of this RtfShapePosition.
    /// </summary>
    private readonly int _right;

    /// <summary>
    ///     The top coordinate of this RtfShapePosition.
    /// </summary>
    private readonly int _top;

    /// <summary>
    ///     Whether to ignore the horizontal relative position.
    /// </summary>
    private bool _ignoreXRelative;

    /// <summary>
    ///     Whether to ignore the vertical relative position.
    /// </summary>
    private bool _ignoreYRelative;

    /// <summary>
    ///     Whether the shape is below the text.
    /// </summary>
    private bool _shapeBelowText;

    /// <summary>
    ///     The horizontal relative position.
    /// </summary>
    private int _xRelativePos = POSITION_X_RELATIVE_PAGE;

    /// <summary>
    ///     The vertical relative position.
    /// </summary>
    private int _yRelativePos = POSITION_Y_RELATIVE_PAGE;

    /// <summary>
    ///     The z order of this RtfShapePosition.
    /// </summary>
    private int _zOrder;

    /// <summary>
    ///     Constructs a new RtfShapePosition with the four bounding coordinates.
    /// </summary>
    /// <param name="top">The top coordinate.</param>
    /// <param name="left">The left coordinate.</param>
    /// <param name="right">The right coordinate.</param>
    /// <param name="bottom">The bottom coordinate.</param>
    public RtfShapePosition(int top, int left, int right, int bottom)
    {
        _top = top;
        _left = left;
        _right = right;
        _bottom = bottom;
    }

    /// <summary>
    ///     Gets whether the shape is below the text.
    /// </summary>
    /// <returns> True  if the shape is below,  false  if the text is below.</returns>
    public bool IsShapeBelowText() => _shapeBelowText;

    /// <summary>
    ///     Sets whether the shape is below the text.
    /// </summary>
    /// <param name="shapeBelowText"> True  if the shape is below,  false  if the text is below.</param>
    public void SetShapeBelowText(bool shapeBelowText)
    {
        _shapeBelowText = shapeBelowText;
    }

    /// <summary>
    ///     Sets the relative horizontal position. Use one of the constants
    ///     provided in this class.
    /// </summary>
    /// <param name="relativePos">The relative horizontal position to use.</param>
    public void SetXRelativePos(int relativePos)
    {
        _xRelativePos = relativePos;
    }

    /// <summary>
    ///     Sets the relative vertical position. Use one of the constants
    ///     provides in this class.
    /// </summary>
    /// <param name="relativePos">The relative vertical position to use.</param>
    public void SetYRelativePos(int relativePos)
    {
        _yRelativePos = relativePos;
    }

    /// <summary>
    ///     Sets the z order to use.
    /// </summary>
    /// <param name="order">The z order to use.</param>
    public void SetZOrder(int order)
    {
        _zOrder = order;
    }

    /// <summary>
    ///     Write this RtfShapePosition.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(t = DocWriter.GetIsoBytes("\\shpleft"), 0, t.Length);
        outp.Write(t = IntToByteArray(_left), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\shptop"), 0, t.Length);
        outp.Write(t = IntToByteArray(_top), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\shpright"), 0, t.Length);
        outp.Write(t = IntToByteArray(_right), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\shpbottom"), 0, t.Length);
        outp.Write(t = IntToByteArray(_bottom), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\shpz"), 0, t.Length);
        outp.Write(t = IntToByteArray(_zOrder), 0, t.Length);
        switch (_xRelativePos)
        {
            case POSITION_X_RELATIVE_PAGE:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpbxpage"), 0, t.Length);
                break;
            case POSITION_X_RELATIVE_MARGIN:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpbxmargin"), 0, t.Length);
                break;
            case POSITION_X_RELATIVE_COLUMN:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpbxcolumn"), 0, t.Length);
                break;
        }

        if (_ignoreXRelative)
        {
            outp.Write(t = DocWriter.GetIsoBytes("\\shpbxignore"), 0, t.Length);
        }

        switch (_yRelativePos)
        {
            case POSITION_Y_RELATIVE_PAGE:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpbypage"), 0, t.Length);
                break;
            case POSITION_Y_RELATIVE_MARGIN:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpbymargin"), 0, t.Length);
                break;
            case POSITION_Y_RELATIVE_PARAGRAPH:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpbypara"), 0, t.Length);
                break;
        }

        if (_ignoreYRelative)
        {
            outp.Write(t = DocWriter.GetIsoBytes("\\shpbyignore"), 0, t.Length);
        }

        if (_shapeBelowText)
        {
            outp.Write(t = DocWriter.GetIsoBytes("\\shpfblwtxt1"), 0, t.Length);
        }
        else
        {
            outp.Write(t = DocWriter.GetIsoBytes("\\shpfblwtxt0"), 0, t.Length);
        }
    }

    /// <summary>
    ///     Set whether to ignore the horizontal relative position.
    /// </summary>
    /// <param name="ignoreXRelative"> True  to ignore the horizontal relative position,  false  otherwise.</param>
    protected internal void SetIgnoreXRelative(bool ignoreXRelative)
    {
        _ignoreXRelative = ignoreXRelative;
    }

    /// <summary>
    ///     Set whether to ignore the vertical relative position.
    /// </summary>
    /// <param name="ignoreYRelative"> True  to ignore the vertical relative position,  false  otherwise.</param>
    protected internal void SetIgnoreYRelative(bool ignoreYRelative)
    {
        _ignoreYRelative = ignoreYRelative;
    }
}