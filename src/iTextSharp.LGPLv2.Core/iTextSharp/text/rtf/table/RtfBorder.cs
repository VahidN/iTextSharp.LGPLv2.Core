using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.table;

/// <summary>
///     The RtfBorder handle one row or cell border.
///     INTERNAL USE ONLY
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Unknown
/// </summary>
public class RtfBorder : RtfElement
{
    /// <summary>
    ///     Constant for a dashed border
    /// </summary>
    public const int BORDER_DASHED = 5;

    /// <summary>
    ///     Constant for a dot dash border
    /// </summary>
    public const int BORDER_DOT_DASH = 8;

    /// <summary>
    ///     Constant for a dot dot dash border
    /// </summary>
    public const int BORDER_DOT_DOT_DASH = 9;

    /// <summary>
    ///     Constant for a dotted border
    /// </summary>
    public const int BORDER_DOTTED = 4;

    /// <summary>
    ///     Constant for a double border
    /// </summary>
    public const int BORDER_DOUBLE = 7;

    /// <summary>
    ///     Constant for a double thick border
    /// </summary>
    public const int BORDER_DOUBLE_THICK = 2;

    /// <summary>
    ///     Constant for a double wavy border
    /// </summary>
    public const int BORDER_DOUBLE_WAVY = 21;

    /// <summary>
    ///     Constant for an embossed border
    /// </summary>
    public const int BORDER_EMBOSS = 23;

    /// <summary>
    ///     Constant for an engraved border
    /// </summary>
    public const int BORDER_ENGRAVE = 24;

    /// <summary>
    ///     Constant for a hairline border
    /// </summary>
    public const int BORDER_HAIRLINE = 6;

    /// <summary>
    ///     Constant for a border with no border
    /// </summary>
    public const int BORDER_NONE = 0;

    /// <summary>
    ///     Constant for a shadowed border
    /// </summary>
    public const int BORDER_SHADOWED = 3;

    /// <summary>
    ///     Constant for a single border
    /// </summary>
    public const int BORDER_SINGLE = 1;

    /// <summary>
    ///     Constant for a striped border
    /// </summary>
    public const int BORDER_STRIPED = 22;

    /// <summary>
    ///     Constant for a thick thin border
    /// </summary>
    public const int BORDER_THICK_THIN = 11;

    /// <summary>
    ///     Constant for a thick thin large border
    /// </summary>
    public const int BORDER_THICK_THIN_LARGE = 17;

    /// <summary>
    ///     Constant for a thick thin medium border
    /// </summary>
    public const int BORDER_THICK_THIN_MED = 14;

    /// <summary>
    ///     Constant for a thin thick border
    /// </summary>
    public const int BORDER_THIN_THICK = 12;

    /// <summary>
    ///     Constant for a thin thick large border
    /// </summary>
    public const int BORDER_THIN_THICK_LARGE = 18;

    /// <summary>
    ///     Constant for a thin thick medium border
    /// </summary>
    public const int BORDER_THIN_THICK_MED = 15;

    /// <summary>
    ///     Constant for a thin thick thin border
    /// </summary>
    public const int BORDER_THIN_THICK_THIN = 13;

    /// <summary>
    ///     Constant for a thin thick thin large border
    /// </summary>
    public const int BORDER_THIN_THICK_THIN_LARGE = 19;

    /// <summary>
    ///     Constant for a thin thick thin medium border
    /// </summary>
    public const int BORDER_THIN_THICK_THIN_MED = 16;

    /// <summary>
    ///     Constant for a triple border
    /// </summary>
    public const int BORDER_TRIPLE = 10;

    /// <summary>
    ///     Constant for a wavy border
    /// </summary>
    public const int BORDER_WAVY = 20;

    /// <summary>
    ///     Constant for a bottom border
    /// </summary>
    protected internal const int BOTTOM_BORDER = 8;

    /// <summary>
    ///     Constant for a box (left, top, right, bottom) border
    /// </summary>
    protected internal const int BOX_BORDER = 15;

    /// <summary>
    ///     Constant for a cell border
    /// </summary>
    protected internal const int CELL_BORDER = 2;

    /// <summary>
    ///     Constant for a horizontal line
    /// </summary>
    protected internal const int HORIZONTAL_BORDER = 32;

    /// <summary>
    ///     Constant for a left border
    /// </summary>
    protected internal const int LEFT_BORDER = 1;

    /// <summary>
    ///     This border is no border :-)
    /// </summary>
    protected internal const int NO_BORDER = 0;

    /// <summary>
    ///     Constant for a right border
    /// </summary>
    protected internal const int RIGHT_BORDER = 4;

    /// <summary>
    ///     Constant for a row border
    /// </summary>
    protected internal const int ROW_BORDER = 1;

    /// <summary>
    ///     Constant for a top border
    /// </summary>
    protected internal const int TOP_BORDER = 2;

    /// <summary>
    ///     Constant for a vertical line
    /// </summary>
    protected internal const int VERTICAL_BORDER = 16;

    /// <summary>
    ///     Constant for the border colour number
    /// </summary>
    protected internal static readonly byte[] BorderColorNumber = DocWriter.GetIsoBytes("\\brdrcf");

    /// <summary>
    ///     Constant for the dashed border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleDashed = DocWriter.GetIsoBytes("\\brdrdash");

    /// <summary>
    ///     Constant for the dot dash border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleDotDash = DocWriter.GetIsoBytes("\\brdrdashd");

    /// <summary>
    ///     Constant for the dot dot dash border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleDotDotDash = DocWriter.GetIsoBytes("\\brdrdashdd");

    /// <summary>
    ///     Constant for the dotted border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleDotted = DocWriter.GetIsoBytes("\\brdrdot");

    /// <summary>
    ///     Constant for the double border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleDouble = DocWriter.GetIsoBytes("\\brdrdb");

    /// <summary>
    ///     Constant for the double thick border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleDoubleThick = DocWriter.GetIsoBytes("\\brdrth");

    /// <summary>
    ///     Constant for the double wavy border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleDoubleWavy = DocWriter.GetIsoBytes("\\brdrwavydb");

    /// <summary>
    ///     Constant for the embossed border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleEmboss = DocWriter.GetIsoBytes("\\brdremboss");

    /// <summary>
    ///     Constant for the engraved border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleEngrave = DocWriter.GetIsoBytes("\\brdrengrave");

    /// <summary>
    ///     Constant for the hairline border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleHairline = DocWriter.GetIsoBytes("\\brdrhair");

    /// <summary>
    ///     Constant for the shadowed border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleShadowed = DocWriter.GetIsoBytes("\\brdrsh");

    /// <summary>
    ///     Constant for the single border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleSingle = DocWriter.GetIsoBytes("\\brdrs");

    /// <summary>
    ///     Constant for the striped border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleStriped = DocWriter.GetIsoBytes("\\brdrdashdotstr");

    /// <summary>
    ///     Constant for the thick thin border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThickThin = DocWriter.GetIsoBytes("\\brdrtnthsg");

    /// <summary>
    ///     Constant for the thick thin large border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThickThinLarge = DocWriter.GetIsoBytes("\\brdrtnthlg");

    /// <summary>
    ///     Constant for the thick thin medium border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThickThinMed = DocWriter.GetIsoBytes("\\brdrtnthmg");

    /// <summary>
    ///     Constant for the thin thick border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThinThick = DocWriter.GetIsoBytes("\\brdrthtnsg");

    /// <summary>
    ///     Constant for the thin thick large border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThinThickLarge = DocWriter.GetIsoBytes("\\brdrthtnlg");

    /// <summary>
    ///     Constant for the thin thick medium border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThinThickMed = DocWriter.GetIsoBytes("\\brdrthtnmg");

    /// <summary>
    ///     Constant for the thin thick thin border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThinThickThin = DocWriter.GetIsoBytes("\\brdrtnthtnsg");

    /// <summary>
    ///     Constant for the thin thick thin large border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThinThickThinLarge = DocWriter.GetIsoBytes("\\brdrtnthtnlg");

    /// <summary>
    ///     Constant for the thin thick thin medium border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleThinThickThinMed = DocWriter.GetIsoBytes("\\brdrtnthtnmg");

    /// <summary>
    ///     Constant for the triple border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleTriple = DocWriter.GetIsoBytes("\\brdrtriple");

    /// <summary>
    ///     Constant for the wavy border style
    /// </summary>
    protected internal static readonly byte[] BorderStyleWavy = DocWriter.GetIsoBytes("\\brdrwavy");

    /// <summary>
    ///     Constant for the border width
    /// </summary>
    protected internal static readonly byte[] BorderWidth = DocWriter.GetIsoBytes("\\brdrw");

    /// <summary>
    ///     Constant for the bottom cell border
    /// </summary>
    protected internal static readonly byte[] CellBorderBottom = DocWriter.GetIsoBytes("\\clbrdrb");

    /// <summary>
    ///     Constant for the left cell border
    /// </summary>
    protected internal static readonly byte[] CellBorderLeft = DocWriter.GetIsoBytes("\\clbrdrl");

    /// <summary>
    ///     Constant for the right cell border
    /// </summary>
    protected internal static readonly byte[] CellBorderRight = DocWriter.GetIsoBytes("\\clbrdrr");

    /// <summary>
    ///     Constant for the top cell border
    /// </summary>
    protected internal static readonly byte[] CellBorderTop = DocWriter.GetIsoBytes("\\clbrdrt");

    /// <summary>
    ///     Constant for the bottom row border
    /// </summary>
    protected internal static readonly byte[] RowBorderBottom = DocWriter.GetIsoBytes("\\trbrdrb");

    /// <summary>
    ///     Constant for the horizontal line
    /// </summary>
    protected internal static readonly byte[] RowBorderHorizontal = DocWriter.GetIsoBytes("\\trbrdrh");

    /// <summary>
    ///     Constant for the left row border
    /// </summary>
    protected internal static readonly byte[] RowBorderLeft = DocWriter.GetIsoBytes("\\trbrdrl");

    /// <summary>
    ///     Constant for the right row border
    /// </summary>
    protected internal static readonly byte[] RowBorderRight = DocWriter.GetIsoBytes("\\trbrdrr");

    /// <summary>
    ///     Constant for the top row border
    /// </summary>
    protected internal static readonly byte[] RowBorderTop = DocWriter.GetIsoBytes("\\trbrdrt");

    /// <summary>
    ///     Constant for the vertical line
    /// </summary>
    protected internal static readonly byte[] RowBorderVertical = DocWriter.GetIsoBytes("\\trbrdrv");

    /// <summary>
    ///     The colour of this RtfBorder
    /// </summary>
    private readonly RtfColor _borderColor;

    /// <summary>
    ///     The position of this RtfBorder
    /// </summary>
    private readonly int _borderPosition = NO_BORDER;

    /// <summary>
    ///     The style of this RtfBorder
    /// </summary>
    private readonly int _borderStyle = BORDER_NONE;

    /// <summary>
    ///     The type of this RtfBorder
    /// </summary>
    private readonly int _borderType = ROW_BORDER;

    /// <summary>
    ///     The width of this RtfBorder
    /// </summary>
    private readonly int _borderWidth = 20;

    /// <summary>
    ///     Makes a copy of the given RtfBorder
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfBorder belongs to</param>
    /// <param name="borderType">The border type of this RtfBorder</param>
    /// <param name="border">The RtfBorder to copy</param>
    protected internal RtfBorder(RtfDocument doc, int borderType, RtfBorder border) : base(doc)
    {
        if (border == null)
        {
            throw new ArgumentNullException(nameof(border));
        }

        _borderType = borderType;
        _borderPosition = border.GetBorderPosition();
        _borderStyle = border.GetBorderStyle();
        _borderWidth = border.GetBorderWidth();
        _borderColor = new RtfColor(Document, border.GetBorderColor());
    }

    /// <summary>
    ///     Constructs a RtfBorder
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfBorder belongs to</param>
    /// <param name="borderType">The type of border this RtfBorder is</param>
    /// <param name="borderPosition">The position of this RtfBorder</param>
    /// <param name="borderStyle">The style of this RtfBorder</param>
    /// <param name="borderWidth">The width of this RtfBorder</param>
    /// <param name="borderColor">The colour of this RtfBorder</param>
    protected internal RtfBorder(RtfDocument doc, int borderType, int borderPosition, int borderStyle,
                                 float borderWidth, BaseColor borderColor) : base(doc)
    {
        _borderType = borderType;
        _borderPosition = borderPosition;
        _borderStyle = borderStyle;
        _borderWidth = (int)Math.Min(borderWidth * TWIPS_FACTOR, 75);
        if (_borderWidth == 0)
        {
            _borderStyle = BORDER_NONE;
        }

        if (borderColor == null)
        {
            _borderColor = new RtfColor(Document, new BaseColor(0, 0, 0));
        }
        else
        {
            _borderColor = new RtfColor(Document, borderColor);
        }
    }

    /// <summary>
    ///     Writes the RtfBorder settings
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        if (_borderStyle == BORDER_NONE || _borderPosition == NO_BORDER || _borderWidth == 0)
        {
            return;
        }

        byte[] t;
        if (_borderType == ROW_BORDER)
        {
            switch (_borderPosition)
            {
                case LEFT_BORDER:
                    outp.Write(RowBorderLeft, 0, RowBorderLeft.Length);
                    break;
                case TOP_BORDER:
                    outp.Write(RowBorderTop, 0, RowBorderTop.Length);
                    break;
                case RIGHT_BORDER:
                    outp.Write(RowBorderRight, 0, RowBorderRight.Length);
                    break;
                case BOTTOM_BORDER:
                    outp.Write(RowBorderBottom, 0, RowBorderBottom.Length);
                    break;
                case HORIZONTAL_BORDER:
                    outp.Write(RowBorderHorizontal, 0, RowBorderHorizontal.Length);
                    break;
                case VERTICAL_BORDER:
                    outp.Write(RowBorderVertical, 0, RowBorderVertical.Length);
                    break;
                default:
                    return;
            }

            outp.Write(t = writeBorderStyle(), 0, t.Length);
            outp.Write(BorderWidth, 0, BorderWidth.Length);
            outp.Write(t = IntToByteArray(_borderWidth), 0, t.Length);
            outp.Write(BorderColorNumber, 0, BorderColorNumber.Length);
            outp.Write(t = IntToByteArray(_borderColor.GetColorNumber()), 0, t.Length);
            Document.OutputDebugLinebreak(outp);
        }
        else if (_borderType == CELL_BORDER)
        {
            switch (_borderPosition)
            {
                case LEFT_BORDER:
                    outp.Write(CellBorderLeft, 0, CellBorderLeft.Length);
                    break;
                case TOP_BORDER:
                    outp.Write(CellBorderTop, 0, CellBorderTop.Length);
                    break;
                case RIGHT_BORDER:
                    outp.Write(CellBorderRight, 0, CellBorderRight.Length);
                    break;
                case BOTTOM_BORDER:
                    outp.Write(CellBorderBottom, 0, CellBorderBottom.Length);
                    break;
                default:
                    return;
            }

            outp.Write(t = writeBorderStyle(), 0, t.Length);
            outp.Write(BorderWidth, 0, BorderWidth.Length);
            outp.Write(t = IntToByteArray(_borderWidth), 0, t.Length);
            outp.Write(BorderColorNumber, 0, BorderColorNumber.Length);
            outp.Write(t = IntToByteArray(_borderColor.GetColorNumber()), 0, t.Length);
            Document.OutputDebugLinebreak(outp);
        }
    }

    /// <summary>
    ///     Gets the colour of this RtfBorder
    /// </summary>
    /// <returns>Returns RtfColor of this RtfBorder</returns>
    protected RtfColor GetBorderColor() => _borderColor;

    /// <summary>
    ///     Gets the position of this RtfBorder
    /// </summary>
    /// <returns>Returns the position of this RtfBorder</returns>
    protected int GetBorderPosition() => _borderPosition;

    /// <summary>
    ///     Gets the style of this RtfBorder
    /// </summary>
    /// <returns>Returns the style of this RtfBorder</returns>
    protected int GetBorderStyle() => _borderStyle;

    /// <summary>
    ///     Gets the type of this RtfBorder
    /// </summary>
    /// <returns>Returns the type of this RtfBorder</returns>
    protected int GetBorderType() => _borderType;

    /// <summary>
    ///     Gets the width of this RtfBorder
    /// </summary>
    /// <returns>Returns the width of this RtfBorder</returns>
    protected int GetBorderWidth() => _borderWidth;

    /// <summary>
    ///     Writes the style of this RtfBorder
    /// </summary>
    /// <returns>A byte array containing the style of this RtfBorder</returns>
    private byte[] writeBorderStyle()
    {
        switch (_borderStyle)
        {
            case BORDER_NONE: return Array.Empty<byte>();
            case BORDER_SINGLE: return BorderStyleSingle;
            case BORDER_DOUBLE_THICK: return BorderStyleDoubleThick;
            case BORDER_SHADOWED: return BorderStyleShadowed;
            case BORDER_DOTTED: return BorderStyleDotted;
            case BORDER_DASHED: return BorderStyleDashed;
            case BORDER_HAIRLINE: return BorderStyleHairline;
            case BORDER_DOUBLE: return BorderStyleDouble;
            case BORDER_DOT_DASH: return BorderStyleDotDash;
            case BORDER_DOT_DOT_DASH: return BorderStyleDotDotDash;
            case BORDER_TRIPLE: return BorderStyleTriple;
            case BORDER_THICK_THIN: return BorderStyleThickThin;
            case BORDER_THIN_THICK: return BorderStyleThinThick;
            case BORDER_THIN_THICK_THIN: return BorderStyleThinThickThin;
            case BORDER_THICK_THIN_MED: return BorderStyleThickThinMed;
            case BORDER_THIN_THICK_MED: return BorderStyleThinThickMed;
            case BORDER_THIN_THICK_THIN_MED: return BorderStyleThinThickThinMed;
            case BORDER_THICK_THIN_LARGE: return BorderStyleThickThinLarge;
            case BORDER_THIN_THICK_LARGE: return BorderStyleThinThickLarge;
            case BORDER_THIN_THICK_THIN_LARGE: return BorderStyleThinThickThinLarge;
            case BORDER_WAVY: return BorderStyleWavy;
            case BORDER_DOUBLE_WAVY: return BorderStyleDoubleWavy;
            case BORDER_STRIPED: return BorderStyleStriped;
            case BORDER_EMBOSS: return BorderStyleEmboss;
            case BORDER_ENGRAVE: return BorderStyleEngrave;
            default: return BorderStyleSingle;
        }
    }
}