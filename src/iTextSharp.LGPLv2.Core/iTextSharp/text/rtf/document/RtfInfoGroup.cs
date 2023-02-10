namespace iTextSharp.text.rtf.document;

/// <summary>
///     The RtfInfoGroup stores information group elements.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Thomas Bickel (tmb99@inode.at)
/// </summary>
public class RtfInfoGroup : RtfElement
{
    /// <summary>
    ///     Information group starting tag
    /// </summary>
    private static readonly byte[] _infoGroup = DocWriter.GetIsoBytes("\\info");

    /// <summary>
    ///     Constant for the password element
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private static readonly byte[] _infoPassword = DocWriter.GetIsoBytes("\\*\\password");

    /// <summary>
    ///     The RtfInfoElements that belong to this RtfInfoGroup
    /// </summary>
    private readonly List<RtfInfoElement> _infoElements;

    /// <summary>
    ///     Constructs a RtfInfoGroup belonging to a RtfDocument
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfInfoGroup belongs to</param>
    public RtfInfoGroup(RtfDocument doc) : base(doc) => _infoElements = new List<RtfInfoElement>();

    /// <summary>
    ///     Adds an RtfInfoElement to the RtfInfoGroup
    /// </summary>
    /// <param name="infoElement">The RtfInfoElement to add</param>
    public void Add(RtfInfoElement infoElement)
    {
        _infoElements.Add(infoElement);
    }

    /// <summary>
    ///     Writes the RTF information group and its elements.
    /// </summary>
    public override void WriteContent(Stream result)
    {
        result.Write(OpenGroup, 0, OpenGroup.Length);
        result.Write(_infoGroup, 0, _infoGroup.Length);
        for (var i = 0; i < _infoElements.Count; i++)
        {
            var infoElement = _infoElements[i];
            infoElement.WriteContent(result);
        }

        // handle document protection
        if (Document.GetDocumentSettings().IsDocumentProtected())
        {
            byte[] t;
            result.Write(OpenGroup, 0, OpenGroup.Length);
            result.Write(_infoPassword, 0, _infoPassword.Length);
            result.Write(Delimiter, 0, Delimiter.Length);
            result.Write(t = Document.GetDocumentSettings().GetProtectionHashBytes(), 0, t.Length);
            result.Write(CloseGroup, 0, CloseGroup.Length);
        }

        result.Write(CloseGroup, 0, CloseGroup.Length);
        Document.OutputDebugLinebreak(result);
    }
}