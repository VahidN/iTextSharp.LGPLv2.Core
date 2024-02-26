using System.util;

namespace iTextSharp.text.rtf.graphic;

/// <summary>
///     The RtfShape provides the interface for adding shapes to
///     the RTF document. This will only work for Word 97+, older
///     Word versions are not supported by this class.<br /><br />
///     Only very simple shapes are directly supported by the RtfShape.
///     For more complex shapes you will have to read the RTF
///     specification (iText follows the 1.6 specification) and add
///     the desired properties via the RtfShapeProperty.<br /><br />
///     One thing to keep in mind is that distances are not expressed
///     in the standard iText point, but in EMU where 1 inch = 914400 EMU
///     or 1 cm = 360000 EMU.
///     @version $Revision: 1.7 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfShape : RtfAddableElement
{
    /// <summary>
    ///     Constant for an arc shape.
    /// </summary>
    public const int SHAPE_ARC = 19;

    /// <summary>
    ///     Constant for a thick arrow.
    /// </summary>
    public const int SHAPE_ARROR_THICK = 14;

    /// <summary>
    ///     Constant for an arrow.
    /// </summary>
    public const int SHAPE_ARROW = 13;

    /// <summary>
    ///     Constant for a balloon shape.
    /// </summary>
    public const int SHAPE_BALLOON = 17;

    /// <summary>
    ///     Constant for a can shape.
    /// </summary>
    public const int SHAPE_CAN = 22;

    /// <summary>
    ///     Constant for a cube shape.
    /// </summary>
    public const int SHAPE_CUBE = 16;

    /// <summary>
    ///     Constant for a diamond.
    /// </summary>
    public const int SHAPE_DIAMOND = 4;

    /// <summary>
    ///     Constant for a donut shape.
    /// </summary>
    public const int SHAPE_DONUT = 23;

    /// <summary>
    ///     Constant for an ellipse. Use this to create circles.
    /// </summary>
    public const int SHAPE_ELLIPSE = 3;

    /// <summary>
    ///     Constant for a free form shape. The shape verticies must
    ///     be specified with an array of Point objects in a
    ///     RtfShapeProperty with the name PROPERTY_VERTICIES.
    /// </summary>
    public const int SHAPE_FREEFORM = 0;

    /// <summary>
    ///     Constant for a hexagon.
    /// </summary>
    public const int SHAPE_HEXAGON = 9;

    /// <summary>
    ///     Constant for a home plate style shape.
    /// </summary>
    public const int SHAPE_HOME_PLATE = 15;

    /// <summary>
    ///     Constant for a line shape.
    /// </summary>
    public const int SHAPE_LINE = 20;

    /// <summary>
    ///     Constant for an ocatagon.
    /// </summary>
    public const int SHAPE_OCTAGON = 10;

    /// <summary>
    ///     Constant for a parallelogram.
    /// </summary>
    public const int SHAPE_PARALLELOGRAM = 7;

    /// <summary>
    ///     Constant for a Picture Frame.
    /// </summary>
    public const int SHAPE_PICTURE_FRAME = 75;

    /// <summary>
    ///     Constant for a rectangle.
    /// </summary>
    public const int SHAPE_RECTANGLE = 1;

    /// <summary>
    ///     Constant for a rounded rectangle. The roundness is
    ///     set via a RtfShapeProperty with the name PROPERTY_ADJUST_VALUE.
    /// </summary>
    public const int SHAPE_ROUND_RECTANGLE = 2;

    /// <summary>
    ///     Constant for a seal shape.
    /// </summary>
    public const int SHAPE_SEAL = 18;

    /// <summary>
    ///     Constant for a star.
    /// </summary>
    public const int SHAPE_STAR = 12;

    /// <summary>
    ///     Constant for a trapezoid.
    /// </summary>
    public const int SHAPE_TRAPEZOID = 8;

    /// <summary>
    ///     Constant for a isoscelle triangle.
    /// </summary>
    public const int SHAPE_TRIANGLE_ISOSCELES = 5;

    /// <summary>
    ///     Constant for a right triangle.
    /// </summary>
    public const int SHAPE_TRIANGLE_RIGHT = 6;

    /// <summary>
    ///     Text is wrapped on the left and right side.
    /// </summary>
    public const int SHAPE_WRAP_BOTH = 2;

    /// <summary>
    ///     Text is wrapped on the largest side.
    /// </summary>
    public const int SHAPE_WRAP_LARGEST = 5;

    /// <summary>
    ///     Text is wrapped on the left side.
    /// </summary>
    public const int SHAPE_WRAP_LEFT = 3;

    /// <summary>
    ///     Text is not wrapped around the shape.
    /// </summary>
    public const int SHAPE_WRAP_NONE = 0;

    /// <summary>
    ///     Text is wrapped on the right side.
    /// </summary>
    public const int SHAPE_WRAP_RIGHT = 4;

    /// <summary>
    ///     Text is wrapped through the shape.
    /// </summary>
    public const int SHAPE_WRAP_THROUGH = 10;

    /// <summary>
    ///     Text is tightly wrapped on the left and right side.
    /// </summary>
    public const int SHAPE_WRAP_TIGHT_BOTH = 6;

    /// <summary>
    ///     Text is tightly wrapped on the largest side.
    /// </summary>
    public const int SHAPE_WRAP_TIGHT_LARGEST = 9;

    /// <summary>
    ///     Text is tightly wrapped on the left side.
    /// </summary>
    public const int SHAPE_WRAP_TIGHT_LEFT = 7;

    /// <summary>
    ///     Text is tightly wrapped on the right side.
    /// </summary>
    public const int SHAPE_WRAP_TIGHT_RIGHT = 8;

    /// <summary>
    ///     Text is wrapped to the top and bottom.
    /// </summary>
    public const int SHAPE_WRAP_TOP_BOTTOM = 1;

    /// <summary>
    ///     The RtfShapePosition that defines position settings for this RtfShape.
    /// </summary>
    private readonly RtfShapePosition _position;

    /// <summary>
    ///     A Hashtable with RtfShapePropertys that define further shape properties.
    /// </summary>
    private readonly NullValueDictionary<string, RtfShapeProperty> _properties;

    /// <summary>
    ///     The shape type.
    /// </summary>
    private readonly int _type;

    /// <summary>
    ///     The shape nr is a random unique id.
    /// </summary>
    private int _shapeNr;

    /// <summary>
    ///     Text that is contained in the shape.
    /// </summary>
    private string _shapeText = "";

    /// <summary>
    ///     The wrapping mode. Defaults to SHAPE_WRAP_NONE;
    /// </summary>
    private int _wrapping = SHAPE_WRAP_NONE;

    /// <summary>
    ///     Constructs a new RtfShape of a given shape at the given RtfShapePosition.
    /// </summary>
    /// <param name="type">The type of shape to create.</param>
    /// <param name="position">The RtfShapePosition to create this RtfShape at.</param>
    public RtfShape(int type, RtfShapePosition position)
    {
        _type = type;
        _position = position;
        _properties = new NullValueDictionary<string, RtfShapeProperty>();
    }

    /// <summary>
    ///     Sets a property.
    /// </summary>
    /// <param name="property">The property to set for this RtfShape.</param>
    public void SetProperty(RtfShapeProperty property)
    {
        if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        property.SetRtfDocument(Doc);
        _properties[property.GetName()] = property;
    }

    /// <summary>
    ///     Sets the text to display in this RtfShape.
    /// </summary>
    /// <param name="shapeText">The text to display.</param>
    public void SetShapeText(string shapeText)
        => _shapeText = shapeText;

    /// <summary>
    ///     Set the wrapping mode.
    /// </summary>
    /// <param name="wrapping">The wrapping mode to use for this RtfShape.</param>
    public void SetWrapping(int wrapping)
        => _wrapping = wrapping;

    /// <summary>
    ///     Writes the RtfShape. Some settings are automatically translated into
    ///     or require other properties and these are set first.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        _shapeNr = Doc.GetRandomInt();

        _properties["ShapeType"] = new RtfShapeProperty("ShapeType", _type);

        if (_position.IsShapeBelowText())
        {
            _properties["fBehindDocument"] = new RtfShapeProperty("fBehindDocument", true);
        }

        if (InTable)
        {
            _properties["fLayoutInCell"] = new RtfShapeProperty("fLayoutInCell", true);
        }

        if (_properties.ContainsKey("posh"))
        {
            _position.SetIgnoreXRelative(true);
        }

        if (_properties.ContainsKey("posv"))
        {
            _position.SetIgnoreYRelative(true);
        }

        byte[] t;
        outp.Write(RtfElement.OpenGroup, 0, RtfElement.OpenGroup.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\shp"), 0, t.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\shplid"), 0, t.Length);
        outp.Write(t = IntToByteArray(_shapeNr), 0, t.Length);
        _position.WriteContent(outp);

        switch (_wrapping)
        {
            case SHAPE_WRAP_NONE:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr3"), 0, t.Length);

                break;
            case SHAPE_WRAP_TOP_BOTTOM:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr1"), 0, t.Length);

                break;
            case SHAPE_WRAP_BOTH:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr2"), 0, t.Length);
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwrk0"), 0, t.Length);

                break;
            case SHAPE_WRAP_LEFT:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr2"), 0, t.Length);
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwrk1"), 0, t.Length);

                break;
            case SHAPE_WRAP_RIGHT:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr2"), 0, t.Length);
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwrk2"), 0, t.Length);

                break;
            case SHAPE_WRAP_LARGEST:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr2"), 0, t.Length);
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwrk3"), 0, t.Length);

                break;
            case SHAPE_WRAP_TIGHT_BOTH:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr4"), 0, t.Length);
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwrk0"), 0, t.Length);

                break;
            case SHAPE_WRAP_TIGHT_LEFT:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr4"), 0, t.Length);
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwrk1"), 0, t.Length);

                break;
            case SHAPE_WRAP_TIGHT_RIGHT:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr4"), 0, t.Length);
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwrk2"), 0, t.Length);

                break;
            case SHAPE_WRAP_TIGHT_LARGEST:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr4"), 0, t.Length);
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwrk3"), 0, t.Length);

                break;
            case SHAPE_WRAP_THROUGH:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr5"), 0, t.Length);

                break;
            default:
                outp.Write(t = DocWriter.GetIsoBytes("\\shpwr3"), 0, t.Length);

                break;
        }

        if (InHeader)
        {
            outp.Write(t = DocWriter.GetIsoBytes("\\shpfhdr1"), 0, t.Length);
        }

        if (Doc.GetDocumentSettings().IsOutputDebugLineBreaks())
        {
            outp.WriteByte((byte)'\n');
        }

        outp.Write(RtfElement.OpenGroup, 0, RtfElement.OpenGroup.Length);
        outp.Write(t = DocWriter.GetIsoBytes("\\*\\shpinst"), 0, t.Length);

        foreach (var rsp in _properties.Values)
        {
            rsp.WriteContent(outp);
        }

        if (!string.IsNullOrEmpty(_shapeText))
        {
            outp.Write(RtfElement.OpenGroup, 0, RtfElement.OpenGroup.Length);
            outp.Write(t = DocWriter.GetIsoBytes("\\shptxt"), 0, t.Length);
            outp.Write(RtfElement.Delimiter, 0, RtfElement.Delimiter.Length);
            outp.Write(t = DocWriter.GetIsoBytes(_shapeText), 0, t.Length);
            outp.Write(RtfElement.CloseGroup, 0, RtfElement.CloseGroup.Length);
        }

        outp.Write(RtfElement.CloseGroup, 0, RtfElement.CloseGroup.Length);
        Doc.OutputDebugLinebreak(outp);
        outp.Write(RtfElement.CloseGroup, 0, RtfElement.CloseGroup.Length);
    }
}