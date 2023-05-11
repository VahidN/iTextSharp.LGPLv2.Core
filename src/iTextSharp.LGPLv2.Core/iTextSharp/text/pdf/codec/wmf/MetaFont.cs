using System.Text;
using iTextSharp.LGPLv2.Core.System.Encodings;

namespace iTextSharp.text.pdf.codec.wmf;

public class MetaFont : MetaObject
{
    internal const int BOLDTHRESHOLD = 600;

    internal const int DEFAULT_PITCH = 0;

    internal const int ETO_CLIPPED = 4;

    internal const int ETO_OPAQUE = 2;

    internal const int FF_DECORATIVE = 5;

    internal const int FF_DONTCARE = 0;

    internal const int FF_MODERN = 3;

    internal const int FF_ROMAN = 1;

    internal const int FF_SCRIPT = 4;

    internal const int FF_SWISS = 2;

    internal const int FIXED_PITCH = 1;

    internal const int MARKER_BOLD = 1;

    internal const int MARKER_COURIER = 0;

    internal const int MARKER_HELVETICA = 4;

    internal const int MARKER_ITALIC = 2;

    internal const int MARKER_SYMBOL = 12;

    internal const int MARKER_TIMES = 8;

    internal const int nameSize = 32;

    internal const int VARIABLE_PITCH = 2;

    private static readonly string[] _fontNames =
    {
        "Courier", "Courier-Bold", "Courier-Oblique",
        "Courier-BoldOblique",
        "Helvetica", "Helvetica-Bold", "Helvetica-Oblique",
        "Helvetica-BoldOblique",
        "Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic",
        "Symbol", "ZapfDingbats",
    };

    private int _bold;
    private int _charset;
    private string _faceName = "arial";
    private BaseFont _font;
    private int _height;
    private int _italic;
    private int _pitchAndFamily;
    private bool _strikeout;
    private bool _underline;

    public MetaFont() => Type = META_FONT;

    public float Angle { get; private set; }

    public BaseFont Font
    {
        get
        {
            if (_font != null)
            {
                return _font;
            }

            var ff2 = FontFactory.GetFont(_faceName, BaseFont.CP1252, true, 10,
                                          (_italic != 0 ? text.Font.ITALIC : 0) | (_bold != 0 ? text.Font.BOLD : 0));
            _font = ff2.BaseFont;
            if (_font != null)
            {
                return _font;
            }

            string fontName;
            if (_faceName.IndexOf("courier", StringComparison.OrdinalIgnoreCase) != -1 ||
                _faceName.IndexOf("terminal", StringComparison.OrdinalIgnoreCase) != -1
                || _faceName.IndexOf("fixedsys", StringComparison.OrdinalIgnoreCase) != -1)
            {
                fontName = _fontNames[MARKER_COURIER + _italic + _bold];
            }
            else if (_faceName.IndexOf("ms sans serif", StringComparison.OrdinalIgnoreCase) != -1 ||
                     _faceName.IndexOf("arial", StringComparison.OrdinalIgnoreCase) != -1
                     || _faceName.IndexOf("system", StringComparison.OrdinalIgnoreCase) != -1)
            {
                fontName = _fontNames[MARKER_HELVETICA + _italic + _bold];
            }
            else if (_faceName.IndexOf("arial black", StringComparison.OrdinalIgnoreCase) != -1)
            {
                fontName = _fontNames[MARKER_HELVETICA + _italic + MARKER_BOLD];
            }
            else if (_faceName.IndexOf("times", StringComparison.OrdinalIgnoreCase) != -1 ||
                     _faceName.IndexOf("ms serif", StringComparison.OrdinalIgnoreCase) != -1
                     || _faceName.IndexOf("roman", StringComparison.OrdinalIgnoreCase) != -1)
            {
                fontName = _fontNames[MARKER_TIMES + _italic + _bold];
            }
            else if (_faceName.IndexOf("symbol", StringComparison.OrdinalIgnoreCase) != -1)
            {
                fontName = _fontNames[MARKER_SYMBOL];
            }
            else
            {
                var pitch = _pitchAndFamily & 3;
                var family = (_pitchAndFamily >> 4) & 7;
                switch (family)
                {
                    case FF_MODERN:
                        fontName = _fontNames[MARKER_COURIER + _italic + _bold];
                        break;
                    case FF_ROMAN:
                        fontName = _fontNames[MARKER_TIMES + _italic + _bold];
                        break;
                    case FF_SWISS:
                    case FF_SCRIPT:
                    case FF_DECORATIVE:
                        fontName = _fontNames[MARKER_HELVETICA + _italic + _bold];
                        break;
                    default:
                    {
                        switch (pitch)
                        {
                            case FIXED_PITCH:
                                fontName = _fontNames[MARKER_COURIER + _italic + _bold];
                                break;
                            default:
                                fontName = _fontNames[MARKER_HELVETICA + _italic + _bold];
                                break;
                        }

                        break;
                    }
                }
            }

            _font = BaseFont.CreateFont(fontName, BaseFont.CP1252, false);

            return _font;
        }
    }

    public float GetFontSize(MetaState state)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        return Math.Abs(state.TransformY(_height) - state.TransformY(0)) * Document.WmfFontCorrection;
    }

    public void Init(InputMeta meta)
    {
        if (meta == null)
        {
            throw new ArgumentNullException(nameof(meta));
        }

        _height = Math.Abs(meta.ReadShort());
        meta.Skip(2);
        Angle = (float)(meta.ReadShort() / 1800.0 * Math.PI);
        meta.Skip(2);
        _bold = meta.ReadShort() >= BOLDTHRESHOLD ? MARKER_BOLD : 0;
        _italic = meta.ReadByte() != 0 ? MARKER_ITALIC : 0;
        _underline = meta.ReadByte() != 0;
        _strikeout = meta.ReadByte() != 0;
        _charset = meta.ReadByte();
        meta.Skip(3);
        _pitchAndFamily = meta.ReadByte();
        var name = new byte[nameSize];
        int k;
        for (k = 0; k < nameSize; ++k)
        {
            var c = meta.ReadByte();
            if (c == 0)
            {
                break;
            }

            name[k] = (byte)c;
        }

        try
        {
            _faceName = EncodingsRegistry.GetEncoding(1252).GetString(name, 0, k);
        }
        catch
        {
            _faceName = Encoding.ASCII.GetString(name, 0, k);
        }

        _faceName = _faceName.ToLower(CultureInfo.InvariantCulture);
    }

    public bool IsStrikeout() => _strikeout;

    public bool IsUnderline() => _underline;
}