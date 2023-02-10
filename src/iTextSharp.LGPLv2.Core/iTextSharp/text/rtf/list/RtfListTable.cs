using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.list;

/// <summary>
///     The RtfListTable manages all RtfLists in one RtfDocument. It also generates
///     the list and list override tables in the document header.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfListTable : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     Constant for the list override
    /// </summary>
    private static readonly byte[] _listOverride = DocWriter.GetIsoBytes("\\listoverride");

    /// <summary>
    ///     Constant for the list override count
    /// </summary>
    private static readonly byte[] _listOverrideCount = DocWriter.GetIsoBytes("\\listoverridecount");

    /// <summary>
    ///     Constant for the list override table
    /// </summary>
    private static readonly byte[] _listOverrideTable = DocWriter.GetIsoBytes("\\*\\listoverridetable");

    /// <summary>
    ///     Constant for the list table
    /// </summary>
    private static readonly byte[] _listTable = DocWriter.GetIsoBytes("\\*\\listtable");

    /// <summary>
    ///     The RtfLists managed by this RtfListTable
    /// </summary>
    private readonly List<RtfList> _lists;

    /// <summary>
    ///     The RtfPictureList lists managed by this RtfListTable
    /// </summary>
    private readonly List<RtfPictureList> _picturelists;

    /// <summary>
    ///     Constructs a RtfListTable for a RtfDocument
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfListTable belongs to</param>
    public RtfListTable(RtfDocument doc) : base(doc)
    {
        _lists = new List<RtfList>();
        _picturelists = new List<RtfPictureList>();
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the list and list override tables.
    /// </summary>
    public virtual void WriteDefinition(Stream result)
    {
        byte[] t;
        result.Write(OpenGroup, 0, OpenGroup.Length);
        result.Write(_listTable, 0, _listTable.Length);
        Document.OutputDebugLinebreak(result);

        for (var i = 0; i < _picturelists.Count; i++)
        {
            var l = _picturelists[i];
            //          l.SetID(document.GetRandomInt());
            l.WriteDefinition(result);
            Document.OutputDebugLinebreak(result);
        }

        for (var i = 0; i < _lists.Count; i++)
        {
            var l = _lists[i];
            l.SetId(Document.GetRandomInt());
            l.WriteDefinition(result);
            Document.OutputDebugLinebreak(result);
        }

        result.Write(CloseGroup, 0, CloseGroup.Length);
        Document.OutputDebugLinebreak(result);

        result.Write(OpenGroup, 0, OpenGroup.Length);
        result.Write(_listOverrideTable, 0, _listOverrideTable.Length);
        Document.OutputDebugLinebreak(result);

        // list override index values are 1-based, not 0.
        // valid list override index values \ls are 1 to 2000.
        // if there are more then 2000 lists, the result is undefined.
        for (var i = 0; i < _lists.Count; i++)
        {
            result.Write(OpenGroup, 0, OpenGroup.Length);
            result.Write(_listOverride, 0, _listOverride.Length);
            result.Write(RtfList.ListId, 0, RtfList.ListId.Length);
            result.Write(t = IntToByteArray(_lists[i].GetId()), 0, t.Length);
            result.Write(_listOverrideCount, 0, _listOverrideCount.Length);
            result.Write(t = IntToByteArray(0), 0, t.Length); // is this correct? Spec says valid values are 1 or 9.
            result.Write(RtfList.ListNumber, 0, RtfList.ListNumber.Length);
            result.Write(t = IntToByteArray(_lists[i].GetListNumber()), 0, t.Length);
            result.Write(CloseGroup, 0, CloseGroup.Length);
            Document.OutputDebugLinebreak(result);
        }

        result.Write(CloseGroup, 0, CloseGroup.Length);
        Document.OutputDebugLinebreak(result);
    }

    /// <summary>
    ///     Remove a RtfList from the list of RtfLists
    /// </summary>
    /// <param name="list">The RtfList to remove.</param>
    public void FreeListNumber(RtfList list)
    {
        var i = _lists.IndexOf(list);
        if (i >= 0)
        {
            _lists.RemoveAt(i);
        }
    }

    /// <summary>
    ///     Gets the id of the specified RtfList. If the RtfList is not yet in the
    ///     list of RtfLists, then it is added.
    /// </summary>
    /// <param name="list">The RtfList for which to get the id.</param>
    /// <returns>The id of the RtfList.</returns>
    public int GetListNumber(RtfList list)
    {
        if (_lists.Contains(list))
        {
            return _lists.IndexOf(list);
        }

        _lists.Add(list);
        return _lists.Count;
    }
}