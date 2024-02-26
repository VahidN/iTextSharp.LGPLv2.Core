using System.util;
using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.table;

/// <summary>
///     The RtfBorderGroup represents a collection of RtfBorders to use in a RtfCell
///     or RtfTable.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfBorderGroup : RtfElement
{
    /// <summary>
    ///     The borders in this RtfBorderGroup
    /// </summary>
    private readonly NullValueDictionary<int, RtfBorder> _borders;

    /// <summary>
    ///     The type of borders this RtfBorderGroup contains.
    ///     RtfBorder.ROW_BORDER or RtfBorder.CELL_BORDER
    /// </summary>
    private readonly int _borderType = RtfBorder.ROW_BORDER;

    /// <summary>
    ///     Constructs an empty RtfBorderGroup.
    /// </summary>
    public RtfBorderGroup() : base(null)
        => _borders = new NullValueDictionary<int, RtfBorder>();

    /// <summary>
    ///     Constructs a RtfBorderGroup with on border style for multiple borders.
    /// </summary>
    /// <param name="bordersToAdd">
    ///     The borders to add (Rectangle.LEFT, Rectangle.RIGHT, Rectangle.TOP, Rectangle.BOTTOM,
    ///     Rectangle.BOX)
    /// </param>
    /// <param name="borderStyle">The style of border to add (from RtfBorder)</param>
    /// <param name="borderWidth">The border width to use</param>
    /// <param name="borderColor">The border color to use</param>
    public RtfBorderGroup(int bordersToAdd, int borderStyle, float borderWidth, BaseColor borderColor) : base(null)
    {
        _borders = new NullValueDictionary<int, RtfBorder>();
        AddBorder(bordersToAdd, borderStyle, borderWidth, borderColor);
    }

    /// <summary>
    ///     Constructs a RtfBorderGroup based on another RtfBorderGroup.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfBorderGroup belongs to</param>
    /// <param name="borderType">The type of borders this RtfBorderGroup contains</param>
    /// <param name="borderGroup">The RtfBorderGroup to use as a base</param>
    protected internal RtfBorderGroup(RtfDocument doc, int borderType, RtfBorderGroup borderGroup) : base(doc)
    {
        _borders = new NullValueDictionary<int, RtfBorder>();
        _borderType = borderType;

        if (borderGroup != null)
        {
            foreach (var entry in borderGroup.GetBorders())
            {
                var borderPos = entry.Key;
                var border = entry.Value;
                _borders[borderPos] = new RtfBorder(Document, _borderType, border);
            }
        }
    }

    /// <summary>
    ///     Constructs a RtfBorderGroup with certain borders
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfBorderGroup belongs to</param>
    /// <param name="borderType">The type of borders this RtfBorderGroup contains</param>
    /// <param name="bordersToUse">
    ///     The borders to add (Rectangle.LEFT, Rectangle.RIGHT, Rectangle.TOP, Rectangle.BOTTOM,
    ///     Rectangle.BOX)
    /// </param>
    /// <param name="borderWidth">The border width to use</param>
    /// <param name="borderColor">The border color to use</param>
    protected internal RtfBorderGroup(RtfDocument doc,
        int borderType,
        int bordersToUse,
        float borderWidth,
        BaseColor borderColor) : base(doc)
    {
        _borderType = borderType;
        _borders = new NullValueDictionary<int, RtfBorder>();
        AddBorder(bordersToUse, RtfBorder.BORDER_SINGLE, borderWidth, borderColor);
    }

    /// <summary>
    ///     Adds borders to the RtfBorderGroup
    /// </summary>
    /// <param name="bordersToAdd">
    ///     The borders to add (Rectangle.LEFT, Rectangle.RIGHT, Rectangle.TOP, Rectangle.BOTTOM,
    ///     Rectangle.BOX)
    /// </param>
    /// <param name="borderStyle">The style of border to add (from RtfBorder)</param>
    /// <param name="borderWidth">The border width to use</param>
    /// <param name="borderColor">The border color to use</param>
    public void AddBorder(int bordersToAdd, int borderStyle, float borderWidth, BaseColor borderColor)
    {
        if ((bordersToAdd & Rectangle.LEFT_BORDER) == Rectangle.LEFT_BORDER)
        {
            setBorder(RtfBorder.LEFT_BORDER, borderStyle, borderWidth, borderColor);
        }

        if ((bordersToAdd & Rectangle.TOP_BORDER) == Rectangle.TOP_BORDER)
        {
            setBorder(RtfBorder.TOP_BORDER, borderStyle, borderWidth, borderColor);
        }

        if ((bordersToAdd & Rectangle.RIGHT_BORDER) == Rectangle.RIGHT_BORDER)
        {
            setBorder(RtfBorder.RIGHT_BORDER, borderStyle, borderWidth, borderColor);
        }

        if ((bordersToAdd & Rectangle.BOTTOM_BORDER) == Rectangle.BOTTOM_BORDER)
        {
            setBorder(RtfBorder.BOTTOM_BORDER, borderStyle, borderWidth, borderColor);
        }

        if ((bordersToAdd & Rectangle.BOX) == Rectangle.BOX && _borderType == RtfBorder.ROW_BORDER)
        {
            setBorder(RtfBorder.VERTICAL_BORDER, borderStyle, borderWidth, borderColor);
            setBorder(RtfBorder.HORIZONTAL_BORDER, borderStyle, borderWidth, borderColor);
        }
    }

    /// <summary>
    ///     Removes borders from the list of borders
    /// </summary>
    /// <param name="bordersToRemove">The borders to remove (from Rectangle)</param>
    public void RemoveBorder(int bordersToRemove)
    {
        if ((bordersToRemove & Rectangle.LEFT_BORDER) == Rectangle.LEFT_BORDER)
        {
            _borders.Remove(RtfBorder.LEFT_BORDER);
        }

        if ((bordersToRemove & Rectangle.TOP_BORDER) == Rectangle.TOP_BORDER)
        {
            _borders.Remove(RtfBorder.TOP_BORDER);
        }

        if ((bordersToRemove & Rectangle.RIGHT_BORDER) == Rectangle.RIGHT_BORDER)
        {
            _borders.Remove(RtfBorder.RIGHT_BORDER);
        }

        if ((bordersToRemove & Rectangle.BOTTOM_BORDER) == Rectangle.BOTTOM_BORDER)
        {
            _borders.Remove(RtfBorder.BOTTOM_BORDER);
        }

        if ((bordersToRemove & Rectangle.BOX) == Rectangle.BOX && _borderType == RtfBorder.ROW_BORDER)
        {
            _borders.Remove(RtfBorder.VERTICAL_BORDER);
            _borders.Remove(RtfBorder.HORIZONTAL_BORDER);
        }
    }

    /// <summary>
    ///     Writes the borders of this RtfBorderGroup
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        foreach (var rb in _borders.Values)
        {
            rb.WriteContent(outp);
        }
    }

    /// <summary>
    ///     Gets the RtfBorders of this RtfBorderGroup
    /// </summary>
    /// <returns>The RtfBorders of this RtfBorderGroup</returns>
    protected internal INullValueDictionary<int, RtfBorder> GetBorders()
        => _borders;

    /// <summary>
    ///     Sets a border in the Hashtable of borders
    /// </summary>
    /// <param name="borderPosition">The position of this RtfBorder</param>
    /// <param name="borderStyle">The type of borders this RtfBorderGroup contains</param>
    /// <param name="borderWidth">The border width to use</param>
    /// <param name="borderColor">The border color to use</param>
    private void setBorder(int borderPosition, int borderStyle, float borderWidth, BaseColor borderColor)
    {
        var border = new RtfBorder(Document, _borderType, borderPosition, borderStyle, borderWidth, borderColor);
        _borders[borderPosition] = border;
    }
}