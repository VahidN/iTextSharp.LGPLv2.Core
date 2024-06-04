using iTextSharp.text.rtf.document;
using ST = iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.field;

/// <summary>
///     The RtfField class is an abstract base class for all rtf field functionality.
///     Subclasses only need to implement the two abstract methods writeFieldInstContent
///     and writeFieldResultContent. All other field functionality is handled by the
///     RtfField class.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author <a href="mailto:Dirk.Weigenand@smb-tec.com">Dirk Weigenand</a>
/// </summary>
public abstract class RtfField : Chunk, IRtfBasicElement
{
    /// <summary>
    ///     The factor to use for translating from iText to rtf measurments
    /// </summary>
    public const double TWIPS_FACTOR = 20;

    /// <summary>
    ///     Constant for the end of an rtf group
    /// </summary>
    public static readonly byte[] CloseGroup = { (byte)'}' };

    /// <summary>
    ///     Constant for a comma delimiter in rtf
    /// </summary>
    public static readonly byte[] CommaDelimiter = { (byte)';' };

    /// <summary>
    ///     Constant for a delimiter in rtf
    /// </summary>
    public static readonly byte[] Delimiter = { (byte)' ' };

    /// <summary>
    ///     Constant for the beginning of a rtf group
    /// </summary>
    public static readonly byte[] OpenGroup = { (byte)'{' };

    /// <summary>
    ///     Constant for an alt field
    /// </summary>
    private static readonly byte[] _fieldAltBytes = DocWriter.GetIsoBytes("\\fldalt");

    /// <summary>
    ///     Constant for a rtf field
    /// </summary>
    private static readonly byte[] _fieldBytes = DocWriter.GetIsoBytes("\\field");

    /// <summary>
    ///     Constant for a dirty field
    /// </summary>
    private static readonly byte[] _fieldDirtyBytes = DocWriter.GetIsoBytes("\\flddirty");

    /// <summary>
    ///     Constant for a edited field
    /// </summary>
    private static readonly byte[] _fieldEditBytes = DocWriter.GetIsoBytes("\\fldedit");

    /// <summary>
    ///     Constant for the field instructions
    /// </summary>
    private static readonly byte[] _fieldInstructionsBytes = DocWriter.GetIsoBytes("\\*\\fldinst");

    /// <summary>
    ///     Constant for a locked field
    /// </summary>
    private static readonly byte[] _fieldLockedBytes = DocWriter.GetIsoBytes("\\fldlock");

    /// <summary>
    ///     Constant for a private field
    /// </summary>
    private static readonly byte[] _fieldPrivateBytes = DocWriter.GetIsoBytes("\\fldpriv");

    /// <summary>
    ///     Constant for the field result
    /// </summary>
    private static readonly byte[] _fieldResultBytes = DocWriter.GetIsoBytes("\\fldrslt");

    /// <summary>
    ///     Is it an alt field
    /// </summary>
    private bool _fieldAlt;

    /// <summary>
    ///     Is the field dirty
    /// </summary>
    private bool _fieldDirty;

    /// <summary>
    ///     Is the field edited
    /// </summary>
    private bool _fieldEdit;

    /// <summary>
    ///     Is the field locked
    /// </summary>
    private bool _fieldLocked;

    /// <summary>
    ///     Is the field private
    /// </summary>
    private bool _fieldPrivate;

    /// <summary>
    ///     The RtfFont of this RtfField
    /// </summary>
    private ST.RtfFont _font;

    /// <summary>
    ///     Whether this RtfElement is in a header
    /// </summary>
    private bool _inHeader;

    /// <summary>
    ///     Whether this RtfField is in a table
    /// </summary>
    private bool _inTable;

    /// <summary>
    ///     The RtfDocument this RtfField belongs to
    /// </summary>
    protected RtfDocument Document;

    /// <summary>
    ///     Constructs a RtfField for a RtfDocument. This is not very usefull,
    ///     since the RtfField by itself does not do anything. Use one of the
    ///     subclasses instead.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfField belongs to.</param>
    protected RtfField(RtfDocument doc) : this(doc, new Font())
    {
    }

    /// <summary>
    ///     Constructs a RtfField for a RtfDocument. This is not very usefull,
    ///     since the RtfField by itself does not do anything. Use one of the
    ///     subclasses instead.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfField belongs to.</param>
    /// <param name="font">The Font this RtfField should use</param>
    protected RtfField(RtfDocument doc, Font font) : base("", font)
    {
        Document = doc;
        _font = new ST.RtfFont(Document, font);
    }

    public override Font Font
    {
        set
        {
            base.Font = value;
            _font = new ST.RtfFont(Document, value);
        }
    }

    /// <summary>
    ///     Sets whether this RtfField is in a header
    /// </summary>
    /// <param name="inHeader"> True  if this RtfField is in a header,  false  otherwise</param>
    public void SetInHeader(bool inHeader)
    {
        _inHeader = inHeader;
    }

    /// <summary>
    ///     Sets whether this RtfField is in a table
    /// </summary>
    /// <param name="inTable"> True  if this RtfField is in a table,  false  otherwise</param>
    public void SetInTable(bool inTable)
    {
        _inTable = inTable;
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfElement belongs to
    /// </summary>
    /// <param name="doc">The RtfDocument to use</param>
    public void SetRtfDocument(RtfDocument doc)
    {
        Document = doc;
        _font.SetRtfDocument(Document);
    }

    /// <summary>
    ///     Writes the field to the  OutputStream .
    /// </summary>
    public virtual void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        _font.WriteBegin(outp);
        writeFieldBegin(outp);
        writeFieldInstBegin(outp);
        WriteFieldInstContent(outp);
        writeFieldInstEnd(outp);
        writeFieldResultBegin(outp);
        WriteFieldResultContent(outp);
        writeFieldResultEnd(outp);
        writeFieldEnd(outp);
        _font.WriteEnd(outp);
    }

    /// <summary>
    ///     An RtfField is never empty.
    /// </summary>
    public override bool IsEmpty() => false;

    /// <summary>
    ///     Get whether this field is an alt field
    /// </summary>
    /// <returns>Returns whether this field is an alt field</returns>
    public bool IsFieldAlt() => _fieldAlt;

    /// <summary>
    ///     Get whether this field is dirty
    /// </summary>
    /// <returns>Returns whether this field is dirty</returns>
    public bool IsFieldDirty() => _fieldDirty;

    /// <summary>
    ///     Get whether this field is edited
    /// </summary>
    /// <returns>Returns whether this field is edited</returns>
    public bool IsFieldEdit() => _fieldEdit;

    /// <summary>
    ///     Get whether this field is locked
    /// </summary>
    /// <returns>Returns the fieldLocked.</returns>
    public bool IsFieldLocked() => _fieldLocked;

    /// <summary>
    ///     Get whether this field is private
    /// </summary>
    /// <returns>Returns the fieldPrivate.</returns>
    public bool IsFieldPrivate() => _fieldPrivate;

    /// <summary>
    ///     Gets whether this  RtfField  is in a header.
    /// </summary>
    /// <returns> True  if this  RtfField  is in a header,  false  otherwise</returns>
    public bool IsInHeader() => _inHeader;

    /// <summary>
    ///     Gets whether this  RtfField  is in a table.
    /// </summary>
    /// <returns> True  if this  RtfField  is in a table,  false  otherwise</returns>
    public bool IsInTable() => _inTable;

    /// <summary>
    ///     Set whether this field is an alt field
    /// </summary>
    /// <param name="fieldAlt">The value to use</param>
    public void SetFieldAlt(bool fieldAlt)
    {
        _fieldAlt = fieldAlt;
    }

    /// <summary>
    ///     Set whether this field is dirty
    /// </summary>
    /// <param name="fieldDirty">The value to use</param>
    public void SetFieldDirty(bool fieldDirty)
    {
        _fieldDirty = fieldDirty;
    }

    /// <summary>
    ///     Set whether this field is edited.
    /// </summary>
    /// <param name="fieldEdit">The value to use</param>
    public void SetFieldEdit(bool fieldEdit)
    {
        _fieldEdit = fieldEdit;
    }

    /// <summary>
    ///     Set whether this field is locked
    /// </summary>
    /// <param name="fieldLocked">The value to use</param>
    public void SetFieldLocked(bool fieldLocked)
    {
        _fieldLocked = fieldLocked;
    }

    /// <summary>
    ///     Set whether this field is private
    /// </summary>
    /// <param name="fieldPrivate">The value to use</param>
    public void SetFieldPrivate(bool fieldPrivate)
    {
        _fieldPrivate = fieldPrivate;
    }

    /// <summary>
    ///     Writes the content of the field instruction area. Override this
    ///     method in your subclasses.
    /// </summary>
    protected abstract void WriteFieldInstContent(Stream oupt);

    /// <summary>
    ///     Writes the content of the pre-calculated field result. Override this
    ///     method in your subclasses.
    /// </summary>
    protected abstract void WriteFieldResultContent(Stream oupt);

    /// <summary>
    ///     Writes the field beginning. Also writes field properties.
    ///     @throws IOException
    /// </summary>
    /// <returns>A byte array with the field beginning.</returns>
    private void writeFieldBegin(Stream result)
    {
        result.Write(OpenGroup, 0, OpenGroup.Length);
        result.Write(_fieldBytes, 0, _fieldBytes.Length);
        if (_fieldDirty)
        {
            result.Write(_fieldDirtyBytes, 0, _fieldDirtyBytes.Length);
        }

        if (_fieldEdit)
        {
            result.Write(_fieldEditBytes, 0, _fieldEditBytes.Length);
        }

        if (_fieldLocked)
        {
            result.Write(_fieldLockedBytes, 0, _fieldLockedBytes.Length);
        }

        if (_fieldPrivate)
        {
            result.Write(_fieldPrivateBytes, 0, _fieldPrivateBytes.Length);
        }
    }

    /// <summary>
    ///     Writes the end of the field
    /// </summary>
    private static void writeFieldEnd(Stream result)
    {
        result.Write(CloseGroup, 0, CloseGroup.Length);
    }

    /// <summary>
    ///     Writes the beginning of the field instruction area.
    ///     @throws IOException
    /// </summary>
    /// <returns>The beginning of the field instruction area</returns>
    private static void writeFieldInstBegin(Stream result)
    {
        result.Write(OpenGroup, 0, OpenGroup.Length);
        result.Write(_fieldInstructionsBytes, 0, _fieldInstructionsBytes.Length);
        result.Write(Delimiter, 0, Delimiter.Length);
    }

    /// <summary>
    ///     Writes the end of the field instruction area.
    /// </summary>
    private void writeFieldInstEnd(Stream result)
    {
        if (_fieldAlt)
        {
            result.Write(Delimiter, 0, Delimiter.Length);
            result.Write(_fieldAltBytes, 0, _fieldAltBytes.Length);
        }

        result.Write(CloseGroup, 0, CloseGroup.Length);
    }

    /// <summary>
    ///     Writes the beginning of the field result area
    /// </summary>
    private static void writeFieldResultBegin(Stream result)
    {
        result.Write(OpenGroup, 0, OpenGroup.Length);
        result.Write(_fieldResultBytes, 0, _fieldResultBytes.Length);
        result.Write(Delimiter, 0, Delimiter.Length);
    }

    /// <summary>
    ///     Writes the end of the field result area
    /// </summary>
    private static void writeFieldResultEnd(Stream result)
    {
        result.Write(Delimiter, 0, Delimiter.Length);
        result.Write(CloseGroup, 0, CloseGroup.Length);
    }
}