using System.Drawing;

namespace iTextSharp.text.rtf.graphic;

/// <summary>
///     The RtfShapeProperty stores all shape properties that are
///     not handled by the RtfShape and RtfShapePosition.
///     There is a huge selection of properties that can be set. For
///     the most important properites there are constants for the
///     property name, for all others you must find the correct
///     property name in the RTF specification (version 1.6).
///     The following types of property values are supported:
///     long
///     double
///     bool
///     Color
///     int[]
///     Point[]
///     @version $Revision: 1.8 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfShapeProperty : RtfAddableElement
{
    /// <summary>
    ///     Property for defining the first adjust handle for shapes. Used
    ///     with the rounded rectangle. Requires a long value.
    /// </summary>
    public const string PROPERTY_ADJUST_VALUE = "adjustValue";

    /// <summary>
    ///     Property for defining the fill color of the shape. Requires a
    ///     Color value.
    /// </summary>
    public const string PROPERTY_FILL_COLOR = "fillColor";

    /// <summary>
    ///     Property for signalling a horizontal flip of the shape. Requires a
    ///     bool value.
    /// </summary>
    public const string PROPERTY_FLIP_H = "fFlipH";

    /// <summary>
    ///     Property for signalling a vertical flip of the shape. Requires a
    ///     bool value.
    /// </summary>
    public const string PROPERTY_FLIP_V = "fFlipV";

    /// <summary>
    ///     Property for defining the maximum vertical coordinate that is
    ///     visible. Requires a long value.
    /// </summary>
    public const string PROPERTY_GEO_BOTTOM = "geoBottom";

    /// <summary>
    ///     Property for defining the minimum horizontal coordinate that is
    ///     visible. Requires a long value.
    /// </summary>
    public const string PROPERTY_GEO_LEFT = "geoLeft";

    /// <summary>
    ///     Property for defining the maximum horizontal coordinate that is
    ///     visible. Requires a long value.
    /// </summary>
    public const string PROPERTY_GEO_RIGHT = "geoRight";

    /// <summary>
    ///     Property for defining the minimum vertical coordinate that is
    ///     visible. Requires a long value.
    /// </summary>
    public const string PROPERTY_GEO_TOP = "geoTop";

    /// <summary>
    ///     Property for defining an image.
    /// </summary>
    public const string PROPERTY_IMAGE = "pib";

    /// <summary>
    ///     Property for defining that the shape is in a table cell. Requires
    ///     a bool value.
    /// </summary>
    public const string PROPERTY_LAYOUT_IN_CELL = "fLayoutInCell";

    /// <summary>
    ///     Property for defining the line color of the shape. Requires a
    ///     Color value.
    /// </summary>
    public const string PROPERTY_LINE_COLOR = "lineColor";

    /// <summary>
    ///     Property for defining vertices in freeform shapes. Requires a
    ///     Point array as the value.
    /// </summary>
    public const string PROPERTY_VERTICIES = "pVerticies";

    /// <summary>
    ///     The stored value is either an int or a Point array.
    /// </summary>
    private const int PropertyTypeArray = 5;

    /// <summary>
    ///     The stored value is bool.
    /// </summary>
    private const int PropertyTypeBoolean = 2;

    /// <summary>
    ///     The stored value is a Color.
    /// </summary>
    private const int PropertyTypeColor = 4;

    /// <summary>
    ///     The stored value is a double.
    /// </summary>
    private const int PropertyTypeDouble = 3;

    /// <summary>
    ///     The stored value is an Image.
    /// </summary>
    private const int PropertyTypeImage = 6;

    /// <summary>
    ///     The stored value is a long.
    /// </summary>
    private const int PropertyTypeLong = 1;

    /// <summary>
    ///     The RtfShapeProperty name.
    /// </summary>
    private readonly string _name = "";

    /// <summary>
    ///     The value type.
    /// </summary>
    private readonly int _type;

    /// <summary>
    ///     The RtfShapeProperty value.
    /// </summary>
    private readonly object _value;

    /// <summary>
    ///     Constructs a RtfShapeProperty with a long value.
    /// </summary>
    /// <param name="name">The property name to use.</param>
    /// <param name="value">The long value to use.</param>
    public RtfShapeProperty(string name, long value)
    {
        _name = name;
        _value = value;
        _type = PropertyTypeLong;
    }

    /// <summary>
    ///     Constructs a RtfShapeProperty with a double value.
    /// </summary>
    /// <param name="name">The property name to use.</param>
    /// <param name="value">The double value to use.</param>
    public RtfShapeProperty(string name, double value)
    {
        _name = name;
        _value = value;
        _type = PropertyTypeDouble;
    }

    /// <summary>
    ///     Constructs a RtfShapeProperty with a bool value.
    /// </summary>
    /// <param name="name">The property name to use.</param>
    /// <param name="value">The bool value to use.</param>
    public RtfShapeProperty(string name, bool value)
    {
        _name = name;
        _value = value;
        _type = PropertyTypeBoolean;
    }

    /// <summary>
    ///     Constructs a RtfShapeProperty with a Color value.
    /// </summary>
    /// <param name="name">The property name to use.</param>
    /// <param name="value">The Color value to use.</param>
    public RtfShapeProperty(string name, BaseColor value)
    {
        _name = name;
        _value = value;
        _type = PropertyTypeColor;
    }

    /// <summary>
    ///     Constructs a RtfShapeProperty with an int array value.
    /// </summary>
    /// <param name="name">The property name to use.</param>
    /// <param name="value">The int array to use.</param>
    public RtfShapeProperty(string name, int[] value)
    {
        _name = name;
        _value = value;
        _type = PropertyTypeArray;
    }

    /// <summary>
    ///     Constructs a RtfShapeProperty with a Point array value.
    /// </summary>
    /// <param name="name">The property name to use.</param>
    /// <param name="value">The Point array to use.</param>
    public RtfShapeProperty(string name, Point[] value)
    {
        _name = name;
        _value = value;
        _type = PropertyTypeArray;
    }

    /// <summary>
    ///     Constructs a RtfShapeProperty with an Image value.
    /// </summary>
    /// <param name="name">The property name to use.</param>
    /// <param name="value">The Image to use.</param>
    public RtfShapeProperty(string name, Image value)
    {
        _name = name;
        _value = value;
        _type = PropertyTypeImage;
    }

    /// <summary>
    ///     Internaly used to create the RtfShape.
    /// </summary>
    /// <param name="name">The property name to use.</param>
    /// <param name="value">The property value to use.</param>
    private RtfShapeProperty(string name, object value)
    {
        _name = name;
        _value = value;
    }

    /// <summary>
    ///     Gets the name of this RtfShapeProperty.
    /// </summary>
    /// <returns>The name of this RtfShapeProperty.</returns>
    public string GetName() => _name;

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
        outp.Write(RtfElement.OpenGroup, 0, RtfElement.OpenGroup.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\sp"), 0, t.Length);
        outp.Write(RtfElement.OpenGroup, 0, RtfElement.OpenGroup.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\sn"), 0, t.Length);
        outp.Write(RtfElement.Delimiter, 0, RtfElement.Delimiter.Length);
        outp.Write(t = DocWriter.GetIsoBytes(_name), 0, t.Length);
        outp.Write(RtfElement.CloseGroup, 0, RtfElement.CloseGroup.Length);
        outp.Write(RtfElement.OpenGroup, 0, RtfElement.OpenGroup.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\sv"), 0, t.Length);
        outp.Write(RtfElement.Delimiter, 0, RtfElement.Delimiter.Length);
        switch (_type)
        {
            case PropertyTypeLong:
            case PropertyTypeDouble:
                outp.Write(t = DocWriter.GetIsoBytes(_value.ToString()), 0, t.Length);
                break;
            case PropertyTypeBoolean:
                if ((bool)_value)
                {
                    outp.Write(t = DocWriter.GetIsoBytes("1"), 0, t.Length);
                }
                else
                {
                    outp.Write(t = DocWriter.GetIsoBytes("0"), 0, t.Length);
                }

                break;
            case PropertyTypeColor:
                var color = (BaseColor)_value;
                outp.Write(t = IntToByteArray(color.R | (color.G << 8) | (color.B << 16)), 0, t.Length);
                break;
            case PropertyTypeArray:
                if (_value is int[])
                {
                    var values = (int[])_value;
                    outp.Write(t = DocWriter.GetIsoBytes("4;"), 0, t.Length);
                    outp.Write(t = IntToByteArray(values.Length), 0, t.Length);
                    outp.Write(RtfElement.CommaDelimiter, 0, RtfElement.CommaDelimiter.Length);
                    for (var i = 0; i < values.Length; i++)
                    {
                        outp.Write(t = IntToByteArray(values[i]), 0, t.Length);
                        if (i < values.Length - 1)
                        {
                            outp.Write(RtfElement.CommaDelimiter, 0, RtfElement.CommaDelimiter.Length);
                        }
                    }
                }
                else if (_value is Point[])
                {
                    var values = (Point[])_value;
                    outp.Write(t = DocWriter.GetIsoBytes("8;"), 0, t.Length);
                    outp.Write(t = IntToByteArray(values.Length), 0, t.Length);
                    outp.Write(RtfElement.CommaDelimiter, 0, RtfElement.CommaDelimiter.Length);
                    for (var i = 0; i < values.Length; i++)
                    {
                        outp.Write(t = DocWriter.GetIsoBytes("("), 0, t.Length);
                        outp.Write(t = IntToByteArray(values[i].X), 0, t.Length);
                        outp.Write(t = DocWriter.GetIsoBytes(","), 0, t.Length);
                        outp.Write(t = IntToByteArray(values[i].Y), 0, t.Length);
                        outp.Write(t = DocWriter.GetIsoBytes(")"), 0, t.Length);
                        if (i < values.Length - 1)
                        {
                            outp.Write(RtfElement.CommaDelimiter, 0, RtfElement.CommaDelimiter.Length);
                        }
                    }
                }

                break;
            case PropertyTypeImage:
                var image = (Image)_value;
                var img = new RtfImage(Doc, image);
                img.SetTopLevelElement(true);
                outp.Write(RtfElement.OpenGroup, 0, RtfElement.OpenGroup.Length);
                img.WriteContent(outp);
                outp.Write(RtfElement.CloseGroup, 0, RtfElement.CloseGroup.Length);
                break;
        }

        outp.Write(RtfElement.CloseGroup, 0, RtfElement.CloseGroup.Length);
        outp.Write(RtfElement.CloseGroup, 0, RtfElement.CloseGroup.Length);
    }
}