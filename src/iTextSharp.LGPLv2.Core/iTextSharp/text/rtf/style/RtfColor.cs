using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.style;

/// <summary>
///     The RtfColor stores one rtf color value for a rtf document
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfColor : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     Constant for the end of one color entry
    /// </summary>
    private const byte Colon = (byte)';';

    /// <summary>
    ///     Constant for BLUE value
    /// </summary>
    private static readonly byte[] _colorBlueBytes = DocWriter.GetIsoBytes("\\blue");

    /// <summary>
    ///     Constant for GREEN value
    /// </summary>
    private static readonly byte[] _colorGreenBytes = DocWriter.GetIsoBytes("\\green");

    /// <summary>
    ///     Constant for the number of the colour in the list of colours
    /// </summary>
    private static readonly byte[] _colorNumberBytes = DocWriter.GetIsoBytes("\\cf");

    /// <summary>
    ///     Constant for RED value
    /// </summary>
    private static readonly byte[] _colorRedBytes = DocWriter.GetIsoBytes("\\red");

    /// <summary>
    ///     The blue value
    /// </summary>
    private readonly int _blue;

    /// <summary>
    ///     The green value
    /// </summary>
    private readonly int _green;

    /// <summary>
    ///     The red value
    /// </summary>
    private readonly int _red;

    /// <summary>
    ///     The number of the colour in the list of colours
    /// </summary>
    private int _colorNumber;

    /// <summary>
    ///     Constructs a RtfColor as a clone of an existing RtfColor
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfColor belongs to</param>
    /// <param name="col">The RtfColor to use as a base</param>
    public RtfColor(RtfDocument doc, RtfColor col) : base(doc)
    {
        if (col != null)
        {
            _red = col.GetRed();
            _green = col.GetGreen();
            _blue = col.GetBlue();
        }

        if (Document != null)
        {
            _colorNumber = Document.GetDocumentHeader().GetColorNumber(this);
        }
    }

    /// <summary>
    ///     Constructs a RtfColor based on the Color
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfColor belongs to</param>
    /// <param name="col">The Color to base this RtfColor on</param>
    public RtfColor(RtfDocument doc, BaseColor col) : base(doc)
    {
        if (col != null)
        {
            _red = col.R;
            _blue = col.B;
            _green = col.G;
        }

        if (Document != null)
        {
            _colorNumber = Document.GetDocumentHeader().GetColorNumber(this);
        }
    }

    /// <summary>
    ///     Constructs a RtfColor based on the red/green/blue values
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfColor belongs to</param>
    /// <param name="red">The red value to use</param>
    /// <param name="green">The green value to use</param>
    /// <param name="blue">The blue value to use</param>
    public RtfColor(RtfDocument doc, int red, int green, int blue) : base(doc)
    {
        _red = red;
        _blue = blue;
        _green = green;
        if (Document != null)
        {
            _colorNumber = Document.GetDocumentHeader().GetColorNumber(this);
        }
    }

    /// <summary>
    ///     Constructor only for use when initializing the RtfColorList
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfColor belongs to</param>
    /// <param name="red">The red value to use</param>
    /// <param name="green">The green value to use</param>
    /// <param name="blue">The blue value to use</param>
    /// <param name="colorNumber">The number of the colour in the colour list</param>
    protected internal RtfColor(RtfDocument doc, int red, int green, int blue, int colorNumber) : base(doc)
    {
        _red = red;
        _blue = blue;
        _green = green;
        _colorNumber = colorNumber;
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfColor belongs to
    /// </summary>
    /// <param name="doc">The RtfDocument to use</param>
    public override void SetRtfDocument(RtfDocument doc)
    {
        base.SetRtfDocument(doc);
        if (Document != null)
        {
            _colorNumber = Document.GetDocumentHeader().GetColorNumber(this);
        }
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Write the definition part of this RtfColor.
    /// </summary>
    public virtual void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(_colorRedBytes, 0, _colorRedBytes.Length);
        outp.Write(t = IntToByteArray(_red), 0, t.Length);
        outp.Write(_colorGreenBytes, 0, _colorGreenBytes.Length);
        outp.Write(t = IntToByteArray(_green), 0, t.Length);
        outp.Write(_colorBlueBytes, 0, _colorBlueBytes.Length);
        outp.Write(t = IntToByteArray(_blue), 0, t.Length);
        outp.WriteByte(Colon);
    }

    /// <summary>
    ///     Tests if this RtfColor is equal to another RtfColor.
    ///     false  otherwise.
    /// </summary>
    /// <param name="obj">another RtfColor</param>
    /// <returns> True  if red, green and blue values of the two colours match,</returns>
    public override bool Equals(object obj)
    {
        if (!(obj is RtfColor))
        {
            return false;
        }

        var color = (RtfColor)obj;
        return _red == color.GetRed() && _green == color.GetGreen() && _blue == color.GetBlue();
    }

    /// <summary>
    ///     Get the blue value of this RtfColor
    /// </summary>
    /// <returns>The blue value</returns>
    public int GetBlue() => _blue;

    /// <summary>
    ///     Gets the number of this RtfColor in the list of colours
    /// </summary>
    /// <returns>Returns the colorNumber.</returns>
    public int GetColorNumber() => _colorNumber;

    /// <summary>
    ///     Get the green value of this RtfColor
    /// </summary>
    /// <returns>The green value</returns>
    public int GetGreen() => _green;

    /// <summary>
    ///     Returns the hash code of this RtfColor. The hash code is
    ///     an integer with the lowest three bytes containing the values
    ///     of red, green and blue.
    /// </summary>
    /// <returns>The hash code of this RtfColor</returns>
    public override int GetHashCode() => (_red << 16) | (_green << 8) | _blue;

    /// <summary>
    ///     Get the red value of this RtfColor
    /// </summary>
    /// <returns>The red value</returns>
    public int GetRed() => _red;

    /// <summary>
    ///     Writes the beginning of this RtfColor
    /// </summary>
    public void WriteBegin(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        try
        {
            result.Write(_colorNumberBytes, 0, _colorNumberBytes.Length);
            result.Write(t = IntToByteArray(_colorNumber), 0, t.Length);
        }
        catch (IOException)
        {
        }
    }

    /// <summary>
    ///     Unused
    /// </summary>
    public static void WriteEnd(Stream result)
    {
    }
}