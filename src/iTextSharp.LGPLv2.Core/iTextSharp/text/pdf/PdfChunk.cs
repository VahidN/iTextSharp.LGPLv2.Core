using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfChunk  is the PDF translation of a  Chunk .
///     A  PdfChunk  is a  PdfString  in a certain
///     PdfFont  and  Color .
///     @see     PdfString
///     @see     PdfFont
///     @see     iTextSharp.text.Chunk
///     @see     iTextSharp.text.Font
/// </summary>
public class PdfChunk
{
    private const float ItalicAngle = 0.21256f;

    /// <summary>
    ///     The allowed attributes in variable  attributes .
    /// </summary>
    private static readonly NullValueDictionary<string, object> _keysAttributes = new();

    /// <summary>
    ///     The allowed attributes in variable  noStroke .
    /// </summary>
    private static readonly NullValueDictionary<string, object> _keysNoStroke = new();

    private static readonly char[] _singleSpace =
    {
        ' '
    };

    private static readonly PdfChunk[] _thisChunk = new PdfChunk[1];

    /// <summary>
    ///     Metric attributes.
    ///     This attributes require the mesurement of characters widths when rendering
    ///     such as underline.
    /// </summary>
    protected INullValueDictionary<string, object> Attributes = new NullValueDictionary<string, object>();

    protected BaseFont BaseFont;

    /// <summary>
    ///     Indicates if the height and offset of the Image has to be taken into account
    /// </summary>
    protected bool changeLeading;

    /// <summary>
    ///     The encoding.
    /// </summary>
    protected string encoding = BaseFont.WINANSI;

    /// <summary>
    ///     The font for this  PdfChunk .
    /// </summary>
    protected PdfFont font;

    /// <summary>
    ///     The image in this  PdfChunk , if it has one
    /// </summary>
    protected Image image;

    /// <summary>
    ///     true  if the chunk split was cause by a newline.
    /// </summary>
    protected bool NewlineSplit;

    /// <summary>
    ///     Non metric attributes.
    ///     This attributes do not require the mesurement of characters widths when rendering
    ///     such as Color.
    /// </summary>
    protected INullValueDictionary<string, object> NoStroke = new NullValueDictionary<string, object>();

    /// <summary>
    ///     The offset in the x direction for the image
    /// </summary>
    protected float OffsetX;

    /// <summary>
    ///     The offset in the y direction for the image
    /// </summary>
    protected float OffsetY;

    protected ISplitCharacter SplitCharacter;

    /// <summary>
    ///     The value of this object.
    /// </summary>
    protected string value = PdfObject.NOTHING;

    static PdfChunk()
    {
        _keysAttributes.Add(Chunk.ACTION, null);
        _keysAttributes.Add(Chunk.UNDERLINE, null);
        _keysAttributes.Add(Chunk.REMOTEGOTO, null);
        _keysAttributes.Add(Chunk.LOCALGOTO, null);
        _keysAttributes.Add(Chunk.LOCALDESTINATION, null);
        _keysAttributes.Add(Chunk.GENERICTAG, null);
        _keysAttributes.Add(Chunk.NEWPAGE, null);
        _keysAttributes.Add(Chunk.IMAGE, null);
        _keysAttributes.Add(Chunk.BACKGROUND, null);
        _keysAttributes.Add(Chunk.PDFANNOTATION, null);
        _keysAttributes.Add(Chunk.SKEW, null);
        _keysAttributes.Add(Chunk.HSCALE, null);
        _keysAttributes.Add(Chunk.SEPARATOR, null);
        _keysAttributes.Add(Chunk.TAB, null);
        _keysNoStroke.Add(Chunk.SUBSUPSCRIPT, null);
        _keysNoStroke.Add(Chunk.SPLITCHARACTER, null);
        _keysNoStroke.Add(Chunk.HYPHENATION, null);
        _keysNoStroke.Add(Chunk.TEXTRENDERMODE, null);
    }

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfChunk -object.
    /// </summary>
    public PdfChunk(string str, PdfChunk other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        _thisChunk[0] = this;
        value = str;
        font = other.font;
        Attributes = other.Attributes;
        NoStroke = other.NoStroke;
        BaseFont = other.BaseFont;
        var obj = (object[])Attributes[Chunk.IMAGE];

        if (obj == null)
        {
            image = null;
        }
        else
        {
            image = (Image)obj[0];
            OffsetX = (float)obj[1];
            OffsetY = (float)obj[2];
            changeLeading = (bool)obj[3];
        }

        encoding = font.Font.Encoding;
        SplitCharacter = (ISplitCharacter)NoStroke[Chunk.SPLITCHARACTER];

        if (SplitCharacter == null)
        {
            SplitCharacter = DefaultSplitCharacter.Default;
        }
    }

    /// <summary>
    ///     Constructs a  PdfChunk -object.
    /// </summary>
    /// <param name="chunk">the original  Chunk -object</param>
    /// <param name="action">the  PdfAction  if the  Chunk  comes from an  Anchor </param>
    public PdfChunk(Chunk chunk, PdfAction action)
    {
        if (chunk == null)
        {
            throw new ArgumentNullException(nameof(chunk));
        }

        _thisChunk[0] = this;
        value = chunk.Content;

        var f = chunk.Font;
        var size = f.Size;

        if (size.ApproxEquals(text.Font.UNDEFINED))
        {
            size = 12;
        }

        BaseFont = f.BaseFont;
        var bf = f.BaseFont;
        var style = f.Style;

        if (style == text.Font.UNDEFINED)
        {
            style = text.Font.NORMAL;
        }

        if (BaseFont == null)
        {
            // translation of the font-family to a PDF font-family
            BaseFont = f.GetCalculatedBaseFont(false);
        }
        else
        {
            // bold simulation
            if ((style & text.Font.BOLD) != 0)
            {
                Attributes[Chunk.TEXTRENDERMODE] = new object[]
                {
                    PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE, size / 30f, null
                };
            }

            // italic simulation
            if ((style & text.Font.ITALIC) != 0)
            {
                Attributes[Chunk.SKEW] = new[]
                {
                    0, ItalicAngle
                };
            }
        }

        font = new PdfFont(BaseFont, size);

        // other style possibilities
        var attr = chunk.Attributes;

        if (attr != null)
        {
            foreach (var entry in attr)
            {
                var name = entry.Key;

                if (_keysAttributes.ContainsKey(name))
                {
                    Attributes[name] = entry.Value;
                }
                else if (_keysNoStroke.ContainsKey(name))
                {
                    NoStroke[name] = entry.Value;
                }
            }

            if ("".Equals(attr[Chunk.GENERICTAG]))
            {
                Attributes[Chunk.GENERICTAG] = chunk.Content;
            }
        }

        if (f.IsUnderlined())
        {
            object[] obj =
            {
                null, new[]
                {
                    0, 1f / 15, 0, -1f / 3, 0
                }
            };

            var unders = Utilities.AddToArray((object[][])Attributes[Chunk.UNDERLINE], obj);
            Attributes[Chunk.UNDERLINE] = unders;
        }

        if (f.IsStrikethru())
        {
            object[] obj =
            {
                null, new[]
                {
                    0, 1f / 15, 0, 1f / 3, 0
                }
            };

            var unders = Utilities.AddToArray((object[][])Attributes[Chunk.UNDERLINE], obj);
            Attributes[Chunk.UNDERLINE] = unders;
        }

        if (action != null)
        {
            Attributes[Chunk.ACTION] = action;
        }

        // the color can't be stored in a PdfFont
        NoStroke[Chunk.COLOR] = f.Color;
        NoStroke[Chunk.ENCODING] = font.Font.Encoding;
        var obj2 = (object[])Attributes[Chunk.IMAGE];

        if (obj2 == null)
        {
            image = null;
        }
        else
        {
            Attributes.Remove(Chunk.HSCALE); // images are scaled in other ways
            image = (Image)obj2[0];
            OffsetX = (float)obj2[1];
            OffsetY = (float)obj2[2];
            changeLeading = (bool)obj2[3];
        }

        font.Image = image;
        var hs = Attributes[Chunk.HSCALE];

        if (hs != null)
        {
            font.HorizontalScaling = (float)hs;
        }

        encoding = font.Font.Encoding;
        SplitCharacter = (ISplitCharacter)NoStroke[Chunk.SPLITCHARACTER];

        if (SplitCharacter == null)
        {
            SplitCharacter = DefaultSplitCharacter.Default;
        }
    }

    /// <summary>
    ///     methods
    /// </summary>

    public bool ChangeLeading => changeLeading;

    /// <summary>
    ///     Gets the text displacement relatiev to the baseline.
    /// </summary>
    /// <returns>a displacement in points</returns>
    public float TextRise
    {
        get
        {
            var f = GetAttribute(Chunk.SUBSUPSCRIPT);

            if (f != null)
            {
                return (float)f;
            }

            return 0.0f;
        }
    }

    public BaseColor Color => (BaseColor)NoStroke[Chunk.COLOR];

    public string Encoding => encoding;

    public PdfFont Font => font;

    public Image Image => image;

    public float ImageOffsetX
    {
        get => OffsetX;

        set => OffsetX = value;
    }

    public float ImageOffsetY
    {
        get => OffsetY;

        set => OffsetY = value;
    }

    /// <summary>
    ///     Gets the encoding of this string.
    /// </summary>
    /// <returns>a  string </returns>
    public int Length => value.Length;

    public int LengthUtf32
    {
        get
        {
            if (!BaseFont.IDENTITY_H.Equals(encoding, StringComparison.Ordinal))
            {
                return value.Length;
            }

            var total = 0;
            var len = value.Length;

            for (var k = 0; k < len; ++k)
            {
                if (Utilities.IsSurrogateHigh(value[k]))
                {
                    ++k;
                }

                ++total;
            }

            return total;
        }
    }

    public string Value
    {
        set => this.value = value;
    }

    public float Width => font.Width(value);

    public static bool NoPrint(int c)
        => (c >= 0x200b && c <= 0x200f) || (c >= 0x202a && c <= 0x202e);

    /// <summary>
    ///     Gets the Unicode equivalent to a CID.
    ///     The (inexistent) CID FF00 is translated as '\n'.
    ///     It has only meaning with CJK fonts with Identity encoding.
    /// </summary>
    /// <param name="c">the CID code</param>
    /// <returns>the Unicode equivalent</returns>
    public int GetUnicodeEquivalent(int c)
        => BaseFont.GetUnicodeEquivalent(c);

    public float GetWidthCorrected(float charSpacing, float wordSpacing)
    {
        if (image != null)
        {
            return image.ScaledWidth + charSpacing;
        }

        var numberOfSpaces = 0;
        var idx = -1;

        while ((idx = value.IndexOf(" ", idx + 1, StringComparison.Ordinal)) >= 0)
        {
            ++numberOfSpaces;
        }

        return Width + (value.Length * charSpacing + numberOfSpaces * wordSpacing);
    }

    public bool IsNewlineSplit()
        => NewlineSplit;

    /// <summary>
    ///     Gets the image in the  PdfChunk .
    /// </summary>
    /// <returns>the image or  null </returns>
    /// <summary>
    ///     Gets the image offset in the x direction
    /// </summary>
    /// <returns>the image offset in the x direction</returns>
    /// <summary>
    ///     Gets the image offset in the y direction
    /// </summary>
    /// <returns>Gets the image offset in the y direction</returns>
    /// <summary>
    ///     sets the value.
    /// </summary>
    public override string ToString()
        => value;

    public float TrimFirstSpace()
    {
        var ft = font.Font;

        if (ft.FontType == BaseFont.FONT_TYPE_CJK && ft.GetUnicodeEquivalent(' ') != ' ')
        {
            if (value.Length > 1 && value.StartsWith("\u0001", StringComparison.Ordinal))
            {
                value = value.Substring(1);

                return font.Width('\u0001');
            }
        }
        else
        {
            if (value.Length > 1 && value.StartsWith(" ", StringComparison.Ordinal))
            {
                value = value.Substring(1);

                return font.Width(' ');
            }
        }

        return 0;
    }

    public float TrimLastSpace()
    {
        var ft = font.Font;

        if (ft.FontType == BaseFont.FONT_TYPE_CJK && ft.GetUnicodeEquivalent(' ') != ' ')
        {
            if (value.Length > 1 && value.EndsWith("\u0001", StringComparison.Ordinal))
            {
                value = value.Substring(0, value.Length - 1);

                return font.Width('\u0001');
            }
        }
        else
        {
            if (value.Length > 1 && value.EndsWith(" ", StringComparison.Ordinal))
            {
                value = value.Substring(0, value.Length - 1);

                return font.Width(' ');
            }
        }

        return 0;
    }

    /// <summary>
    ///     Correction for the tab position based on the left starting position.
    ///     @since   2.1.2
    /// </summary>
    /// <param name="newValue">the new value for the left X.</param>
    public void AdjustLeft(float newValue)
    {
        var o = (object[])Attributes[Chunk.TAB];

        if (o != null)
        {
            Attributes[Chunk.TAB] = new[]
            {
                o[0], o[1], o[2], newValue
            };
        }
    }

    public object GetAttribute(string name)
    {
        if (Attributes.TryGetValue(name, out var attribute))
        {
            return attribute;
        }

        return NoStroke[name];
    }

    public float GetCharWidth(int c)
    {
        if (NoPrint(c))
        {
            return 0;
        }

        return font.Width(c);
    }

    public bool IsAttribute(string name)
    {
        if (Attributes.ContainsKey(name))
        {
            return true;
        }

        return NoStroke.ContainsKey(name);
    }

    public bool IsExtSplitCharacter(int start, int current, int end, char[] cc, PdfChunk[] ck)
        => SplitCharacter.IsSplitCharacter(start, current, end, cc, ck);

    /// <summary>
    ///     Checks if this  PdfChunk  is a horizontal Separator Chunk.
    ///     @since   2.1.2
    /// </summary>
    /// <returns>true if this chunk is a horizontal separator.</returns>
    public bool IsHorizontalSeparator()
    {
        if (IsAttribute(Chunk.SEPARATOR))
        {
            var o = (object[])GetAttribute(Chunk.SEPARATOR);

            return !(bool)o[1];
        }

        return false;
    }

    public bool IsImage()
        => image != null;

    /// <summary>
    ///     Checks if this  PdfChunk  is a Separator Chunk.
    ///     @since   2.1.2
    /// </summary>
    /// <returns>true if this chunk is a separator.</returns>
    public bool IsSeparator()
        => IsAttribute(Chunk.SEPARATOR);

    public bool IsSpecialEncoding()
        => encoding.Equals(CjkFont.CJK_ENCODING, StringComparison.Ordinal) ||
           encoding.Equals(BaseFont.IDENTITY_H, StringComparison.Ordinal);

    public bool IsStroked()
        => Attributes.Count > 0;

    /// <summary>
    ///     Checks if this  PdfChunk  is a tab Chunk.
    ///     @since   2.1.2
    /// </summary>
    /// <returns>true if this chunk is a separator.</returns>
    public bool IsTab()
        => IsAttribute(Chunk.TAB);

    public PdfChunk Split(float width)
    {
        NewlineSplit = false;

        if (image != null)
        {
            if (image.ScaledWidth > width)
            {
                var pc = new PdfChunk(Chunk.OBJECT_REPLACEMENT_CHARACTER, this);
                value = "";
                Attributes = new NullValueDictionary<string, object>();
                image = null;
                font = PdfFont.DefaultFont;

                return pc;
            }

            return null;
        }

        var hyphenationEvent = (IHyphenationEvent)NoStroke[Chunk.HYPHENATION];
        var currentPosition = 0;
        var splitPosition = -1;
        float currentWidth = 0;

        // loop over all the characters of a string
        // or until the totalWidth is reached
        var lastSpace = -1;
        float lastSpaceWidth = 0;
        var length = value.Length;
        var valueArray = value.ToCharArray();
        var character = (char)0;
        var ft = font.Font;
        var surrogate = false;

        if (ft.FontType == BaseFont.FONT_TYPE_CJK && ft.GetUnicodeEquivalent(' ') != ' ')
        {
            while (currentPosition < length)
            {
                // the width of every character is added to the currentWidth
                var cidChar = valueArray[currentPosition];
                character = (char)ft.GetUnicodeEquivalent(cidChar);

                // if a newLine or carriageReturn is encountered
                if (character == '\n')
                {
                    NewlineSplit = true;
                    var returnValue = value.Substring(currentPosition + 1);
                    value = value.Substring(0, currentPosition);

                    if (value.Length < 1)
                    {
                        value = "\u0001";
                    }

                    var pc = new PdfChunk(returnValue, this);

                    return pc;
                }

                currentWidth += font.Width(cidChar);

                if (character == ' ')
                {
                    lastSpace = currentPosition + 1;
                    lastSpaceWidth = currentWidth;
                }

                if (currentWidth > width)
                {
                    break;
                }

                // if a split-character is encountered, the splitPosition is altered
                if (SplitCharacter.IsSplitCharacter(0, currentPosition, length, valueArray, _thisChunk))
                {
                    splitPosition = currentPosition + 1;
                }

                currentPosition++;
            }
        }
        else
        {
            while (currentPosition < length)
            {
                // the width of every character is added to the currentWidth
                character = valueArray[currentPosition];

                // if a newLine or carriageReturn is encountered
                if (character == '\r' || character == '\n')
                {
                    NewlineSplit = true;
                    var inc = 1;

                    if (character == '\r' && currentPosition + 1 < length && valueArray[currentPosition + 1] == '\n')
                    {
                        inc = 2;
                    }

                    var returnValue = value.Substring(currentPosition + inc);
                    value = value.Substring(0, currentPosition);

                    if (value.Length < 1)
                    {
                        value = " ";
                    }

                    var pc = new PdfChunk(returnValue, this);

                    return pc;
                }

                surrogate = Utilities.IsSurrogatePair(valueArray, currentPosition);

                if (surrogate)
                {
                    currentWidth += font.Width(Utilities.ConvertToUtf32(valueArray[currentPosition],
                        valueArray[currentPosition + 1]));
                }
                else
                {
                    currentWidth += font.Width(character);
                }

                if (character == ' ')
                {
                    lastSpace = currentPosition + 1;
                    lastSpaceWidth = currentWidth;
                }

                if (surrogate)
                {
                    currentPosition++;
                }

                if (currentWidth > width)
                {
                    break;
                }

                // if a split-character is encountered, the splitPosition is altered
                if (SplitCharacter.IsSplitCharacter(0, currentPosition, length, valueArray, null))
                {
                    splitPosition = currentPosition + 1;
                }

                currentPosition++;
            }
        }

        // if all the characters fit in the total width, null is returned (there is no overflow)
        if (currentPosition == length)
        {
            return null;
        }

        // otherwise, the string has to be truncated
        if (splitPosition < 0)
        {
            var returnValue = value;
            value = "";
            var pc = new PdfChunk(returnValue, this);

            return pc;
        }

        if (lastSpace > splitPosition && SplitCharacter.IsSplitCharacter(0, 0, 1, _singleSpace, null))
        {
            splitPosition = lastSpace;
        }

        if (hyphenationEvent != null && lastSpace >= 0 && lastSpace < currentPosition)
        {
            var wordIdx = GetWord(value, lastSpace);

            if (wordIdx > lastSpace)
            {
                var pre = hyphenationEvent.GetHyphenatedWordPre(value.Substring(lastSpace, wordIdx - lastSpace),
                    font.Font, font.Size, width - lastSpaceWidth);

                var post = hyphenationEvent.HyphenatedWordPost;

                if (pre.Length > 0)
                {
                    var returnValue = post + value.Substring(wordIdx);
                    value = Trim(value.Substring(0, lastSpace) + pre);
                    var pc = new PdfChunk(returnValue, this);

                    return pc;
                }
            }
        }

        var retVal = value.Substring(splitPosition);
        value = Trim(value.Substring(0, splitPosition));
        var tmp = new PdfChunk(retVal, this);

        return tmp;
    }

    public string Trim(string str)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        var ft = font.Font;

        if (ft.FontType == BaseFont.FONT_TYPE_CJK && ft.GetUnicodeEquivalent(' ') != ' ')
        {
            while (str.EndsWith("\u0001", StringComparison.Ordinal))
            {
                str = str.Substring(0, str.Length - 1);
            }
        }
        else
        {
            while (str.EndsWith(" ", StringComparison.Ordinal) || str.EndsWith("\t", StringComparison.Ordinal))
            {
                str = str.Substring(0, str.Length - 1);
            }
        }

        return str;
    }

    public PdfChunk Truncate(float width)
    {
        if (image != null)
        {
            if (image.ScaledWidth > width)
            {
                var pc = new PdfChunk("", this);
                value = "";
                Attributes.Remove(Chunk.IMAGE);
                image = null;
                font = PdfFont.DefaultFont;

                return pc;
            }

            return null;
        }

        var currentPosition = 0;
        float currentWidth = 0;

        // it's no use trying to split if there isn't even enough place for a space
        if (width < font.Width())
        {
            var returnValue = value.Substring(1);
            value = value.Substring(0, 1);
            var pc = new PdfChunk(returnValue, this);

            return pc;
        }

        // loop over all the characters of a string
        // or until the totalWidth is reached
        var length = value.Length;
        var surrogate = false;

        while (currentPosition < length)
        {
            // the width of every character is added to the currentWidth
            surrogate = Utilities.IsSurrogatePair(value, currentPosition);

            if (surrogate)
            {
                currentWidth += font.Width(Utilities.ConvertToUtf32(value, currentPosition));
            }
            else
            {
                currentWidth += font.Width(value[currentPosition]);
            }

            if (currentWidth > width)
            {
                break;
            }

            if (surrogate)
            {
                currentPosition++;
            }

            currentPosition++;
        }

        // if all the characters fit in the total width, null is returned (there is no overflow)
        if (currentPosition == length)
        {
            return null;
        }

        // otherwise, the string has to be truncated
        //currentPosition -= 2;
        // we have to chop off minimum 1 character from the chunk
        if (currentPosition == 0)
        {
            currentPosition = 1;

            if (surrogate)
            {
                ++currentPosition;
            }
        }

        var retVal = value.Substring(currentPosition);
        value = value.Substring(0, currentPosition);
        var tmp = new PdfChunk(retVal, this);

        return tmp;
    }

    protected static int GetWord(string text, int start)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var len = text.Length;

        while (start < len)
        {
            if (!char.IsLetter(text[start]))
            {
                break;
            }

            ++start;
        }

        return start;
    }
}