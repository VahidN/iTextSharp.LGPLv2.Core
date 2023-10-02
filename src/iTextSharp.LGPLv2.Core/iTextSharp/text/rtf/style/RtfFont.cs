using System.util;
using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.style;

/// <summary>
///     The RtfFont class stores one font for an rtf document. It extends Font,
///     so can be set as a font, to allow adding of fonts with arbitrary names.
///     BaseFont fontname handling contributed by Craig Fleming. Various fixes
///     Renaud Michel, Werner Daehn.
///     Version: $Id: RtfFont.cs,v 1.13 2008/05/16 19:31:11 psoares33 Exp $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Craig Fleming (rythos@rhana.dhs.org)
///     @author Renaud Michel (r.michel@immedia.be)
///     @author Werner Daehn (Werner.Daehn@BusinessObjects.com)
///     @author Lidong Liu (tmslld@gmail.com)
/// </summary>
public class RtfFont : Font, IRtfExtendedElement
{
    /// <summary>
    ///     Constant for a bold font
    /// </summary>
    public const int STYLE_BOLD = 1;

    /// <summary>
    ///     Constant for a double strikethrough font
    /// </summary>
    public const int STYLE_DOUBLE_STRIKETHROUGH = 16;

    /// <summary>
    ///     Constant for an embossed font
    /// </summary>
    public const int STYLE_EMBOSSED = 128;

    /// <summary>
    ///     Constant for an engraved font
    /// </summary>
    public const int STYLE_ENGRAVED = 256;

    /// <summary>
    ///     Constant for a font that hides the actual text.
    /// </summary>
    public const int STYLE_HIDDEN = 512;

    /// <summary>
    ///     Constant for an italic font
    /// </summary>
    public const int STYLE_ITALIC = 2;

    /// <summary>
    ///     Constant for a plain font
    /// </summary>
    public const int STYLE_NONE = 0;

    /// <summary>
    ///     Constant for an outlined font
    /// </summary>
    public const int STYLE_OUTLINE = 64;

    /// <summary>
    ///     Constant for a shadowed font
    /// </summary>
    public const int STYLE_SHADOW = 32;

    /// <summary>
    ///     Constant for a strikethrough font
    /// </summary>
    public const int STYLE_STRIKETHROUGH = 8;

    /// <summary>
    ///     Constant for an underlined font
    /// </summary>
    public const int STYLE_UNDERLINE = 4;

    /// <summary>
    ///     Constant for the font size
    /// </summary>
    public static readonly byte[] FontSize = DocWriter.GetIsoBytes("\\fs");

    /// <summary>
    ///     Constant for the bold flag
    /// </summary>
    private static readonly byte[] _fontBold = DocWriter.GetIsoBytes("\\b");

    /// <summary>
    ///     Constant for the charset
    /// </summary>
    private static readonly byte[] _fontCharset = DocWriter.GetIsoBytes("\\fcharset");

    /// <summary>
    ///     Constant for the double strikethrough flag
    /// </summary>
    private static readonly byte[] _fontDoubleStrikethrough = DocWriter.GetIsoBytes("\\striked");

    /// <summary>
    ///     Constant for the embossed flag
    /// </summary>
    private static readonly byte[] _fontEmbossed = DocWriter.GetIsoBytes("\\embo");

    /// <summary>
    ///     Constant for the engraved flag
    /// </summary>
    private static readonly byte[] _fontEngraved = DocWriter.GetIsoBytes("\\impr");

    /// <summary>
    ///     Constant for the font family to use ("froman")
    /// </summary>
    private static readonly byte[] _fontFamily = DocWriter.GetIsoBytes("\\froman");

    /// <summary>
    ///     Constant for hidden text flag
    /// </summary>
    private static readonly byte[] _fontHidden = DocWriter.GetIsoBytes("\\v");

    /// <summary>
    ///     Constant for the italic flag
    /// </summary>
    private static readonly byte[] _fontItalic = DocWriter.GetIsoBytes("\\i");

    /// <summary>
    ///     Constant for the outline flag
    /// </summary>
    private static readonly byte[] _fontOutline = DocWriter.GetIsoBytes("\\outl");

    /// <summary>
    ///     Constant for the shadow flag
    /// </summary>
    private static readonly byte[] _fontShadow = DocWriter.GetIsoBytes("\\shad");

    /// <summary>
    ///     Constant for the strikethrough flag
    /// </summary>
    private static readonly byte[] _fontStrikethrough = DocWriter.GetIsoBytes("\\strike");

    /// <summary>
    ///     Constant for the underline flag
    /// </summary>
    private static readonly byte[] _fontUnderline = DocWriter.GetIsoBytes("\\ul");

    /// <summary>
    ///     The character set to use for this font
    /// </summary>
    private int _charset;

    /// <summary>
    ///     The colour of this font
    /// </summary>
    private RtfColor _color;

    /// <summary>
    ///     The font name. Defaults to "Times New Roman"
    /// </summary>
    private string _fontName = "Times New Roman";

    /// <summary>
    ///     The number of this font
    /// </summary>
    private int _fontNumber;

    /// <summary>
    ///     The font size. Defaults to 10
    /// </summary>
    private int _fontSize = 10;

    /// <summary>
    ///     The font style. Defaults to STYLE_NONE
    /// </summary>
    private int _fontStyle = STYLE_NONE;

    /// <summary>
    ///     The RtfDocument this RtfFont belongs to.
    /// </summary>
    protected RtfDocument Document;

    /// <summary>
    ///     Constructs a RtfFont with the given font name and all other properties
    ///     at their default values.
    /// </summary>
    /// <param name="fontName">The font name to use</param>
    public RtfFont(string fontName) : base(UNDEFINED, UNDEFINED, UNDEFINED, null) => _fontName = fontName;

    /// <summary>
    ///     Constructs a RtfFont with the given font name and font size and all other
    ///     properties at their default values.
    /// </summary>
    /// <param name="fontName">The font name to use</param>
    /// <param name="size">The font size to use</param>
    public RtfFont(string fontName, float size) : base(UNDEFINED, size, UNDEFINED, null) => _fontName = fontName;

    /// <summary>
    ///     Constructs a RtfFont with the given font name, font size and font style and the
    ///     default color.
    /// </summary>
    /// <param name="fontName">The font name to use</param>
    /// <param name="size">The font size to use</param>
    /// <param name="style">The font style to use</param>
    public RtfFont(string fontName, float size, int style) : base(UNDEFINED, size, style, null) => _fontName = fontName;

    /// <summary>
    ///     Constructs a RtfFont with the given font name, font size, font style and
    ///     color.
    /// </summary>
    /// <param name="fontName">The font name to use</param>
    /// <param name="size">the font size to use</param>
    /// <param name="style">The font style to use</param>
    /// <param name="color">The font color to use</param>
    public RtfFont(string fontName, float size, int style, BaseColor color) : base(UNDEFINED, size, style, color) =>
        _fontName = fontName;

    /// <summary>
    ///     Constructs a RtfFont with the given font name, font size, font style, colour
    ///     and charset. This can be used when generating non latin-1 text.
    /// </summary>
    /// <param name="fontName">The font name to use</param>
    /// <param name="size">the font size to use</param>
    /// <param name="style">The font style to use</param>
    /// <param name="color">The font color to use</param>
    /// <param name="charset">The charset of the font content</param>
    public RtfFont(string fontName, float size, int style, BaseColor color, int charset) :
        this(fontName, size, style, color) => _charset = charset;

    /// <summary>
    ///     Constructs a RtfFont from a com.lowagie.text.Font
    /// </summary>
    /// <param name="doc">The RtfDocument this font appears in</param>
    /// <param name="font">The Font to use as a base</param>
    public RtfFont(RtfDocument doc, Font font)
    {
        Document = doc;
        if (font != null)
        {
            if (font is RtfFont)
            {
                _fontName = ((RtfFont)font).GetFontName();
                _charset = ((RtfFont)font).GetCharset();
            }
            else
            {
                setToDefaultFamily(font.Familyname);
            }

            if (font.BaseFont != null)
            {
                var fontNames = font.BaseFont.FullFontName;
                for (var i = 0; i < fontNames.Length; i++)
                {
                    if (fontNames[i][2].Equals("0", StringComparison.Ordinal))
                    {
                        _fontName = fontNames[i][3];
                        break;
                    }

                    if (fontNames[i][2].Equals("1033", StringComparison.Ordinal) ||
                        string.IsNullOrEmpty(fontNames[i][2]))
                    {
                        _fontName = fontNames[i][3];
                    }
                }
            }

            Size = font.Size;
            SetStyle(font.Style);
            Color = font.Color;
            if (Document != null)
            {
                _fontNumber = Document.GetDocumentHeader().GetFontNumber(this);
            }
        }

        if (Util.EqualsIgnoreCase(_fontName, "unknown"))
        {
            return;
        }

        if (Document != null)
        {
            SetRtfDocument(Document);
        }
    }

    /// <summary>
    ///     Special constructor for the default font
    /// </summary>
    /// <param name="doc">The RtfDocument this font appears in</param>
    /// <param name="fontNumber">The id of this font</param>
    protected internal RtfFont(RtfDocument doc, int fontNumber)
    {
        Document = doc;
        _fontNumber = fontNumber;
        _color = new RtfColor(doc, 0, 0, 0);
    }

    /// <summary>
    ///     @see com.lowagie.text.Font#setColor(Color)
    /// </summary>
    public override BaseColor Color
    {
        set
        {
            base.Color = value;
            if (value != null)
            {
                _color = new RtfColor(Document, value);
            }
            else
            {
                _color = null;
            }
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.Font#getFamilyname()
    /// </summary>
    public override string Familyname => _fontName;

    /// <summary>
    ///     @see com.lowagie.text.Font#setSize(float)
    /// </summary>
    public override float Size
    {
        set
        {
            base.Size = value;
            _fontSize = (int)Size;
        }
    }

    /// <summary>
    ///     Unused
    /// </summary>
    /// <param name="inHeader"></param>
    public void SetInHeader(bool inHeader)
    {
    }

    /// <summary>
    ///     Unused
    /// </summary>
    /// <param name="inTable"></param>
    public void SetInTable(bool inTable)
    {
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfFont belongs to
    /// </summary>
    /// <param name="doc">The RtfDocument to use</param>
    public void SetRtfDocument(RtfDocument doc)
    {
        Document = doc;
        if (Document != null)
        {
            _fontNumber = Document.GetDocumentHeader().GetFontNumber(this);
        }

        if (_color != null)
        {
            _color.SetRtfDocument(Document);
        }
    }

    /// <summary>
    ///     unused
    /// </summary>
    public virtual void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the font definition
    /// </summary>
    public virtual void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(_fontFamily, 0, _fontFamily.Length);
        outp.Write(_fontCharset, 0, _fontCharset.Length);
        outp.Write(t = IntToByteArray(_charset), 0, t.Length);
        outp.Write(RtfElement.Delimiter, 0, RtfElement.Delimiter.Length);
        Document.FilterSpecialChar(outp, _fontName, true, false);
    }

    /// <summary>
    ///     Compares this  RtfFont  to either a {@link com.lowagie.text.Font} or
    ///     an  RtfFont .
    ///     @since 2.1.0
    /// </summary>
    public override int CompareTo(object obj)
    {
        if (obj == null)
        {
            return -1;
        }

        if (obj is RtfFont)
        {
            if (!string.Equals(GetFontName(), ((RtfFont)obj).GetFontName(), StringComparison.Ordinal))
            {
                return 1;
            }

            return base.CompareTo(obj);
        }

        if (obj is Font)
        {
            return base.CompareTo(obj);
        }

        return -3;
    }

    /// <summary>
    ///     Replaces the attributes that are equal to <VAR>null</VAR> with
    ///     the attributes of a given font.
    /// </summary>
    /// <param name="font">The surrounding font</param>
    /// <returns>A RtfFont</returns>
    public override Font Difference(Font font)
    {
        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        var dFamilyname = font.Familyname;
        if (dFamilyname == null || string.IsNullOrEmpty(dFamilyname.Trim()) ||
            Util.EqualsIgnoreCase(dFamilyname.Trim(), "unknown"))
        {
            dFamilyname = _fontName;
        }

        var dSize = font.Size;
        if (dSize.ApproxEquals(UNDEFINED))
        {
            dSize = Size;
        }

        var dStyle = UNDEFINED;
        if (Style != UNDEFINED && font.Style != UNDEFINED)
        {
            dStyle = Style | font.Style;
        }
        else if (Style != UNDEFINED)
        {
            dStyle = Style;
        }
        else if (font.Style != UNDEFINED)
        {
            dStyle = font.Style;
        }

        var dColor = font.Color;
        if (dColor == null)
        {
            dColor = Color;
        }

        var dCharset = _charset;
        if (font is RtfFont)
        {
            dCharset = ((RtfFont)font).GetCharset();
        }

        return new RtfFont(dFamilyname, dSize, dStyle, dColor, dCharset);
    }

    /// <summary>
    ///     Tests for equality of RtfFonts. RtfFonts are equal if their fontName,
    ///     fontSize, fontStyle and fontSuperSubscript are equal
    /// </summary>
    /// <param name="obj">The RtfFont to compare with this RtfFont</param>
    /// <returns> True  if the RtfFonts are equal,  false  otherwise</returns>
    public override bool Equals(object obj)
    {
        if (!(obj is RtfFont))
        {
            return false;
        }

        var font = (RtfFont)obj;
        return _fontName.Equals(font.GetFontName(), StringComparison.Ordinal);
    }

    /// <summary>
    ///     Gets the charset used for constructing this RtfFont.
    /// </summary>
    /// <returns>The charset of this RtfFont.</returns>
    public int GetCharset() => _charset;

    /// <summary>
    ///     Gets the font name of this RtfFont
    /// </summary>
    /// <returns>The font name</returns>
    public string GetFontName() => _fontName;

    /// <summary>
    ///     Gets the font number of this RtfFont
    /// </summary>
    /// <returns>The font number</returns>
    public int GetFontNumber() => _fontNumber;

    /// <summary>
    ///     Gets the font size of this RtfFont
    /// </summary>
    /// <returns>The font size</returns>
    public int GetFontSize() => _fontSize;

    /// <summary>
    ///     Gets the font style of this RtfFont
    /// </summary>
    /// <returns>The font style</returns>
    public int GetFontStyle() => _fontStyle;

    /// <summary>
    ///     Returns the hash code of this RtfFont. The hash code is the hash code of the
    ///     string containing the font name + font size + "-" + the font style + "-" + the
    ///     font super/supscript value.
    /// </summary>
    /// <returns>The hash code of this RtfFont</returns>
    public override int GetHashCode() => (_fontName + _fontSize + "-" + _fontStyle).GetHashCode();

    /// <summary>
    ///     The  RtfFont  is never a standard font.
    ///     @since 2.1.0
    /// </summary>
    public override bool IsStandardFont() => false;

    /// <summary>
    ///     Sets the charset used for constructing this RtfFont.
    /// </summary>
    /// <param name="charset">The charset to use.</param>
    public void SetCharset(int charset)
    {
        _charset = charset;
    }

    /// <summary>
    ///     @see com.lowagie.text.Font#setColor(int, int, int)
    /// </summary>
    public override void SetColor(int red, int green, int blue)
    {
        base.SetColor(red, green, blue);
        _color = new RtfColor(Document, red, green, blue);
    }

    /// <summary>
    ///     @see com.lowagie.text.Font#setFamily(String)
    /// </summary>
    public override void SetFamily(string family)
    {
        base.SetFamily(family);
        setToDefaultFamily(family);
    }

    /// <summary>
    ///     Sets the font name of this RtfFont.
    /// </summary>
    /// <param name="fontName">The font name to use</param>
    public virtual void SetFontName(string fontName)
    {
        _fontName = fontName;
        if (Document != null)
        {
            _fontNumber = Document.GetDocumentHeader().GetFontNumber(this);
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.Font#setStyle(int)
    /// </summary>
    public override void SetStyle(int style)
    {
        base.SetStyle(style);
        _fontStyle = Style;
    }

    /// <summary>
    ///     @see com.lowagie.text.Font#setStyle(String)
    /// </summary>
    public override void SetStyle(string style)
    {
        base.SetStyle(style);
        _fontStyle = Style;
    }

    /// <summary>
    ///     Writes the font beginning
    /// </summary>
    /// <returns>A byte array with the font start data</returns>
    public virtual void WriteBegin(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        if (_fontNumber != UNDEFINED)
        {
            result.Write(RtfFontList.FontNumber, 0, RtfFontList.FontNumber.Length);
            result.Write(t = IntToByteArray(_fontNumber), 0, t.Length);
        }

        if (_fontSize != UNDEFINED)
        {
            result.Write(FontSize, 0, FontSize.Length);
            result.Write(t = IntToByteArray(_fontSize * 2), 0, t.Length);
        }

        if (_fontStyle != UNDEFINED)
        {
            if ((_fontStyle & STYLE_BOLD) == STYLE_BOLD)
            {
                result.Write(_fontBold, 0, _fontBold.Length);
            }

            if ((_fontStyle & STYLE_ITALIC) == STYLE_ITALIC)
            {
                result.Write(_fontItalic, 0, _fontItalic.Length);
            }

            if ((_fontStyle & STYLE_UNDERLINE) == STYLE_UNDERLINE)
            {
                result.Write(_fontUnderline, 0, _fontUnderline.Length);
            }

            if ((_fontStyle & STYLE_STRIKETHROUGH) == STYLE_STRIKETHROUGH)
            {
                result.Write(_fontStrikethrough, 0, _fontStrikethrough.Length);
            }

            if ((_fontStyle & STYLE_HIDDEN) == STYLE_HIDDEN)
            {
                result.Write(_fontHidden, 0, _fontHidden.Length);
            }

            if ((_fontStyle & STYLE_DOUBLE_STRIKETHROUGH) == STYLE_DOUBLE_STRIKETHROUGH)
            {
                result.Write(_fontDoubleStrikethrough, 0, _fontDoubleStrikethrough.Length);
                result.Write(t = IntToByteArray(1), 0, t.Length);
            }

            if ((_fontStyle & STYLE_SHADOW) == STYLE_SHADOW)
            {
                result.Write(_fontShadow, 0, _fontShadow.Length);
            }

            if ((_fontStyle & STYLE_OUTLINE) == STYLE_OUTLINE)
            {
                result.Write(_fontOutline, 0, _fontOutline.Length);
            }

            if ((_fontStyle & STYLE_EMBOSSED) == STYLE_EMBOSSED)
            {
                result.Write(_fontEmbossed, 0, _fontEmbossed.Length);
            }

            if ((_fontStyle & STYLE_ENGRAVED) == STYLE_ENGRAVED)
            {
                result.Write(_fontEngraved, 0, _fontEngraved.Length);
            }
        }

        if (_color != null)
        {
            _color.WriteBegin(result);
        }
    }

    /// <summary>
    ///     Write the font end
    /// </summary>
    public virtual void WriteEnd(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        if (_fontStyle != UNDEFINED)
        {
            if ((_fontStyle & STYLE_BOLD) == STYLE_BOLD)
            {
                result.Write(_fontBold, 0, _fontBold.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_ITALIC) == STYLE_ITALIC)
            {
                result.Write(_fontItalic, 0, _fontItalic.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_UNDERLINE) == STYLE_UNDERLINE)
            {
                result.Write(_fontUnderline, 0, _fontUnderline.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_STRIKETHROUGH) == STYLE_STRIKETHROUGH)
            {
                result.Write(_fontStrikethrough, 0, _fontStrikethrough.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_HIDDEN) == STYLE_HIDDEN)
            {
                result.Write(_fontHidden, 0, _fontHidden.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_DOUBLE_STRIKETHROUGH) == STYLE_DOUBLE_STRIKETHROUGH)
            {
                result.Write(_fontDoubleStrikethrough, 0, _fontDoubleStrikethrough.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_SHADOW) == STYLE_SHADOW)
            {
                result.Write(_fontShadow, 0, _fontShadow.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_OUTLINE) == STYLE_OUTLINE)
            {
                result.Write(_fontOutline, 0, _fontOutline.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_EMBOSSED) == STYLE_EMBOSSED)
            {
                result.Write(_fontEmbossed, 0, _fontEmbossed.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }

            if ((_fontStyle & STYLE_ENGRAVED) == STYLE_ENGRAVED)
            {
                result.Write(_fontEngraved, 0, _fontEngraved.Length);
                result.Write(t = IntToByteArray(0), 0, t.Length);
            }
        }
    }

    /// <summary>
    ///     Transforms an integer into its String representation and then returns the bytes
    ///     of that string.
    /// </summary>
    /// <param name="i">The integer to convert</param>
    /// <returns>A byte array representing the integer</returns>
    protected static byte[] IntToByteArray(int i) => DocWriter.GetIsoBytes(i.ToString(CultureInfo.InvariantCulture));

    /// <summary>
    ///     Sets the correct font name from the family name.
    /// </summary>
    /// <param name="familyname">The family name to set the name to.</param>
    private void setToDefaultFamily(string familyname)
    {
        switch (GetFamilyIndex(familyname))
        {
            case COURIER:
                _fontName = "Courier";
                break;
            case HELVETICA:
                _fontName = "Arial";
                break;
            case SYMBOL:
                _fontName = "Symbol";
                _charset = 2;
                break;
            case TIMES_ROMAN:
                _fontName = "Times New Roman";
                break;
            case ZAPFDINGBATS:
                _fontName = "Windings";
                break;
            default:
                _fontName = familyname;
                break;
        }
    }
}