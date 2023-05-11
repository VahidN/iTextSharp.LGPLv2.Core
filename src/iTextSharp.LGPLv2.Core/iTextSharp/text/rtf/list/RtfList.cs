using iTextSharp.text.factories;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf.list;

/// <summary>
///     The RtfList stores one List. It also provides the methods to write the
///     list declaration and the list data.
///     @version $Id: RtfList.cs,v 1.18 2008/05/16 19:31:01 psoares33 Exp $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Thomas Bickel (tmb99@inode.at)
///     @author Felix Satyaputra (f_satyaputra@yahoo.co.uk)
/// </summary>
public class RtfList : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     List type of listhybrid
    ///     @since 2.1.3
    /// </summary>
    public const int LIST_TYPE_HYBRID = 2;

    /// <summary>
    ///     List type of NORMAL - no control word
    ///     @since 2.1.3
    /// </summary>
    public const int LIST_TYPE_NORMAL = 0;

    /// <summary>
    ///     List type of listsimple
    ///     @since 2.1.3
    /// </summary>
    public const int LIST_TYPE_SIMPLE = 1;

    /// <summary>
    ///     Constant for the list id
    ///     @since 2.1.3
    /// </summary>
    public static readonly byte[] ListId = DocWriter.GetIsoBytes("\\listid");

    /// <summary>
    ///     character properties
    /// </summary>
    /// <summary>
    ///     Constant for the list level value
    ///     @since 2.1.3
    /// </summary>
    public static readonly byte[] ListLevelNumber = DocWriter.GetIsoBytes("\\ilvl");

    /// <summary>
    ///     Constant for the list number
    ///     @since 2.1.3
    /// </summary>
    public static readonly byte[] ListNumber = DocWriter.GetIsoBytes("\\ls");

    /// <summary>
    ///     Constant for the old list number end
    ///     @since 2.1.3
    /// </summary>
    public static readonly byte[] ListNumberEnd = DocWriter.GetIsoBytes(".");

    /// <summary>
    ///     Constant for the old list text
    ///     @since 2.1.3
    /// </summary>
    public static readonly byte[] ListText = DocWriter.GetIsoBytes("\\listtext");

    /// <summary>
    ///     Constant for a tab character
    ///     @since 2.1.3
    /// </summary>
    public static readonly byte[] Tab = DocWriter.GetIsoBytes("\\tab");

    /// <summary>
    ///     Constant for the list
    /// </summary>
    private static readonly byte[] _list = DocWriter.GetIsoBytes("\\list");

    /// <summary>
    ///     Constant for the hybrid list
    /// </summary>
    private static readonly byte[] _listHybrid = DocWriter.GetIsoBytes("\\listhybrid");

    /// <summary>
    ///     Constant for the name of this list
    /// </summary>
    private static readonly byte[] _listName = DocWriter.GetIsoBytes("\\listname");

    /// <summary>
    ///     Constant to indicate if the list restarts at each section. Word 7 compatiblity
    /// </summary>
    private static readonly byte[] _listRestarthdn = DocWriter.GetIsoBytes("\\listrestarthdn");

    /// <summary>
    ///     Constant for the simple list
    /// </summary>
    private static readonly byte[] _listSimple = DocWriter.GetIsoBytes("\\listsimple");

    /// <summary>
    ///     Constant for the identifier of the style of this list. Mutually exclusive with \\liststylename
    /// </summary>
    private static readonly byte[] _listStyleid = DocWriter.GetIsoBytes("\\liststyleid");

    /// <summary>
    ///     Constant for the identifier of the style of this list. Mutually exclusive with \\liststyleid
    /// </summary>
    private static readonly byte[] _listStylename = DocWriter.GetIsoBytes("\\liststylename");

    /// <summary>
    ///     Constant for the list template id
    /// </summary>
    private static readonly byte[] _listTemplateId = DocWriter.GetIsoBytes("\\listtemplateid");

    /// <summary>
    ///     The subitems of this RtfList
    /// </summary>
    private readonly List<IRtfBasicElement> _items;

    /// <summary>
    ///     The list id
    /// </summary>
    private int _listId = -1;

    /// <summary>
    ///     The RtfList lists managed by this RtfListTable
    /// </summary>
    private List<RtfListLevel> _listLevels;

    /// <summary>
    ///     The list number of this RtfList
    /// </summary>
    private int _listNumber = -1;

    /// <summary>
    ///     This RtfList type
    /// </summary>
    private int _listType = LIST_TYPE_HYBRID;

    /// <summary>
    ///     The name of the list if it exists
    /// </summary>
    private string _name;

    /// <summary>
    ///     The parent list if there is one.
    /// </summary>
    private RtfList _parentList;
    /*  Normal list type */

    /*  Simple list type */

    /*  Hybrid list type */
    /// <summary>
    ///     Constructs an empty RtfList object.
    ///     @since 2.1.3
    /// </summary>
    public RtfList() : base(null)
    {
        CreateDefaultLevels();
    }

    /// <summary>
    ///     Constructs an empty RtfList object.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfList belongs to</param>
    public RtfList(RtfDocument doc) : base(doc)
    {
        CreateDefaultLevels();
        // get the list number or create a new one adding it to the table
        _listNumber = Document.GetDocumentHeader().GetListNumber(this);
    }

    /// <summary>
    ///     Constructs a new RtfList for the specified List.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfList belongs to</param>
    /// <param name="list">The List this RtfList is based on</param>
    public RtfList(RtfDocument doc, List list) : base(doc)
    {
        if (doc == null)
        {
            throw new ArgumentNullException(nameof(doc));
        }

        if (list == null)
        {
            throw new ArgumentNullException(nameof(list));
        }
        // setup the listlevels
        // Then, setup the list data below

        // setup 1 listlevel if it's a simple list
        // setup 9 if it's a regular list
        // setup 9 if it's a hybrid list (default)
        CreateDefaultLevels();

        _items = new List<IRtfBasicElement>(); // list content
        var ll = _listLevels[0];

        // get the list number or create a new one adding it to the table
        _listNumber = Document.GetDocumentHeader().GetListNumber(this);

        if (list.SymbolIndent > 0 && list.IndentationLeft > 0)
        {
            ll.SetFirstIndent((int)(list.SymbolIndent * TWIPS_FACTOR * -1));
            ll.SetLeftIndent((int)((list.IndentationLeft + list.SymbolIndent) * TWIPS_FACTOR));
        }
        else if (list.SymbolIndent > 0)
        {
            ll.SetFirstIndent((int)(list.SymbolIndent * TWIPS_FACTOR * -1));
            ll.SetLeftIndent((int)(list.SymbolIndent * TWIPS_FACTOR));
        }
        else if (list.IndentationLeft > 0)
        {
            ll.SetFirstIndent(0);
            ll.SetLeftIndent((int)(list.IndentationLeft * TWIPS_FACTOR));
        }
        else
        {
            ll.SetFirstIndent(0);
            ll.SetLeftIndent(0);
        }

        ll.SetRightIndent((int)(list.IndentationRight * TWIPS_FACTOR));
        ll.SetSymbolIndent((int)((list.SymbolIndent + list.IndentationLeft) * TWIPS_FACTOR));
        ll.CorrectIndentation();
        ll.SetTentative(false);

        if (list is RomanList)
        {
            if (list.Lowercase)
            {
                ll.SetListType(RtfListLevel.LIST_TYPE_LOWER_ROMAN);
            }
            else
            {
                ll.SetListType(RtfListLevel.LIST_TYPE_UPPER_ROMAN);
            }
        }
        else if (list.Numbered)
        {
            ll.SetListType(RtfListLevel.LIST_TYPE_NUMBERED);
        }
        else if (list.Lettered)
        {
            if (list.Lowercase)
            {
                ll.SetListType(RtfListLevel.LIST_TYPE_LOWER_LETTERS);
            }
            else
            {
                ll.SetListType(RtfListLevel.LIST_TYPE_UPPER_LETTERS);
            }
        }
        else
        {
            //          Paragraph p = new Paragraph();
            //          p.Add(new Chunk(list.GetPreSymbol()) );
            //          p.Add(list.GetSymbol());
            //          p.Add(new Chunk(list.GetPostSymbol()) );
            //          ll.SetBulletChunk(list.GetSymbol());
            ll.SetBulletCharacter(list.PreSymbol + list.Symbol.Content + list.PostSymbol);
            ll.SetListType(RtfListLevel.LIST_TYPE_BULLET);
        }

        // now setup the actual list contents.
        for (var i = 0; i < list.Items.Count; i++)
        {
            try
            {
                var element = list.Items[i];

                if (element.Type == Element.CHUNK)
                {
                    element = new ListItem((Chunk)element);
                }

                if (element is ListItem)
                {
                    ll.SetAlignment(((ListItem)element).Alignment);
                }

                var rtfElements = doc.GetMapper().MapElement(element);
                for (var j = 0; j < rtfElements.Length; j++)
                {
                    var rtfElement = rtfElements[j];
                    if (rtfElement is RtfList)
                    {
                        ((RtfList)rtfElement).SetParentList(this);
                    }
                    else if (rtfElement is RtfListItem)
                    {
                        ((RtfListItem)rtfElement).SetParent(ll);
                    }

                    ll.SetFontNumber(new RtfFont(Document,
                                                 new Font(Font.TIMES_ROMAN, 10, Font.NORMAL, new BaseColor(0, 0, 0))));
                    if (list.Symbol != null && list.Symbol.Font != null &&
                        !list.Symbol.Content.StartsWith("-", StringComparison.Ordinal) &&
                        list.Symbol.Content.Length > 0)
                    {
                        // only set this to bullet symbol is not default
                        ll.SetBulletFont(list.Symbol.Font);
                        ll.SetBulletCharacter(list.Symbol.Content.Substring(0, 1));
                    }
                    else if (list.Symbol != null && list.Symbol.Font != null)
                    {
                        ll.SetBulletFont(list.Symbol.Font);
                    }
                    else
                    {
                        ll.SetBulletFont(new Font(Font.SYMBOL, 10, Font.NORMAL, new BaseColor(0, 0, 0)));
                    }

                    _items.Add(rtfElement);
                }
            }
            catch (DocumentException)
            {
            }
        }
    }

    /// <summary>
    ///     Sets whether this RtfList is in a header. Sets the correct inTable setting for all
    ///     child elements.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="inHeader"> True  if this RtfList is in a header,  false  otherwise</param>
    public override void SetInHeader(bool inHeader)
    {
        base.SetInHeader(inHeader);
        for (var i = 0; i < _items.Count; i++)
        {
            _items[i].SetInHeader(inHeader);
        }
    }

    /// <summary>
    ///     Sets whether this RtfList is in a table. Sets the correct inTable setting for all
    ///     child elements.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="inTable"> True  if this RtfList is in a table,  false  otherwise</param>
    public override void SetInTable(bool inTable)
    {
        base.SetInTable(inTable);
        for (var i = 0; i < _items.Count; i++)
        {
            _items[i].SetInTable(inTable);
        }

        for (var i = 0; i < _listLevels.Count; i++)
        {
            _listLevels[i].SetInTable(inTable);
        }
    }

    /// <summary>
    ///     Writes the content of the RtfList
    ///     @since 2.1.3
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        if (!InTable)
        {
            outp.Write(OpenGroup, 0, OpenGroup.Length);
        }

        var itemNr = 0;
        if (_items != null)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                var thisRtfElement = (RtfElement)_items[i];
                //thisRtfElement.WriteContent(result);
                if (thisRtfElement is RtfListItem)
                {
                    itemNr++;
                    var rtfElement = (RtfListItem)thisRtfElement;
                    var listLevel = rtfElement.GetParent();
                    if (listLevel.GetListLevel() == 0)
                    {
                        CorrectIndentation();
                    }

                    if (i == 0)
                    {
                        listLevel.WriteListBeginning(outp);
                        WriteListNumbers(outp);
                    }

                    WriteListTextBlock(outp, itemNr, listLevel);

                    rtfElement.WriteContent(outp);

                    if (i < _items.Count - 1 || !InTable || listLevel.GetListType() > 0)
                    {
                        // TODO Fix no paragraph on last list item in tables
                        outp.Write(RtfParagraph.Paragraph, 0, RtfParagraph.Paragraph.Length);
                    }

                    Document.OutputDebugLinebreak(outp);
                }
                else if (thisRtfElement is RtfList)
                {
                    ((RtfList)thisRtfElement).WriteContent(outp);
                    //              ((RtfList)thisRtfElement).WriteListBeginning(result);
                    WriteListNumbers(outp);
                    Document.OutputDebugLinebreak(outp);
                }
            }
        }

        if (!InTable)
        {
            outp.Write(CloseGroup, 0, CloseGroup.Length);
            outp.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
        }
    }

    /// <summary>
    ///     Writes the definition part of this list level
    ///     @throws IOException
    ///     @since 2.1.3
    /// </summary>
    /// <param name="outp"></param>
    public void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_list, 0, _list.Length);
        outp.Write(_listTemplateId, 0, _listTemplateId.Length);
        outp.Write(t = IntToByteArray(Document.GetRandomInt()), 0, t.Length);

        var levelsToWrite = -1;

        switch (_listType)
        {
            case LIST_TYPE_NORMAL:
                levelsToWrite = _listLevels.Count;
                break;
            case LIST_TYPE_SIMPLE:
                outp.Write(_listSimple, 0, _listSimple.Length);
                outp.Write(t = IntToByteArray(1), 0, t.Length);
                levelsToWrite = 1;
                break;
            case LIST_TYPE_HYBRID:
                outp.Write(_listHybrid, 0, _listHybrid.Length);
                levelsToWrite = _listLevels.Count;
                break;
        }

        Document.OutputDebugLinebreak(outp);

        // TODO: Figure out hybrid because multi-level hybrid does not work.
        // Seems hybrid is mixed type all single level - Simple = single level
        // SIMPLE1/HYRBID
        // 1. Line 1
        // 2. Line 2
        // MULTI-LEVEL LISTS Are Simple0 - 9 levels (0-8) all single digit
        // 1. Line 1
        // 1.1. Line 1.1
        // 1.2. Line 1.2
        // 2. Line 2

        // write the listlevels here
        for (var i = 0; i < levelsToWrite; i++)
        {
            _listLevels[i].WriteDefinition(outp);
            Document.OutputDebugLinebreak(outp);
        }

        outp.Write(ListId, 0, ListId.Length);
        outp.Write(t = IntToByteArray(_listId), 0, t.Length);
        outp.Write(CloseGroup, 0, CloseGroup.Length);
        Document.OutputDebugLinebreak(outp);
        if (_items != null)
        {
            for (var i = 0; i < _items.Count; i++)
            {
                var rtfElement = (RtfElement)_items[i];
                if (rtfElement is RtfList)
                {
                    var rl = (RtfList)rtfElement;
                    rl.WriteDefinition(outp);
                    break;
                }

                if (rtfElement is RtfListItem)
                {
                    var rli = (RtfListItem)rtfElement;
                    if (rli.WriteDefinition(outp))
                    {
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Get the list ID number
    ///     @since 2.1.3
    /// </summary>
    /// <returns>this list id</returns>
    public int GetId() => _listId;

    /// <summary>
    ///     @since 2.1.3
    /// </summary>
    /// <returns>the list at the index</returns>
    public RtfListLevel GetListLevel(int index)
    {
        if (_listLevels != null)
        {
            return _listLevels[index];
        }

        return null;
    }

    /// <summary>
    ///     Gets the id of this list
    ///     @since 2.1.3
    /// </summary>
    /// <returns>Returns the list number.</returns>
    public int GetListNumber() => _listNumber;

    /// <summary>
    ///     @see RtfList#LIST_TYPE_NORMAL
    ///     @see RtfList#LIST_TYPE_SIMPLE
    ///     @see RtfList#LIST_TYPE_HYBRID
    ///     @since 2.1.3
    /// </summary>
    /// <returns>the listType</returns>
    public int GetListType() => _listType;

    /// <summary>
    ///     @since 2.1.3
    /// </summary>
    /// <returns>the name</returns>
    public string GetName() => _name;

    /// <summary>
    ///     @since 2.1.3
    /// </summary>
    /// <returns>the parentList</returns>
    public RtfList GetParentList() => _parentList;

    /// <summary>
    ///     Set the document.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="doc">The RtfDocument</param>
    public void SetDocument(RtfDocument doc)
    {
        Document = doc;
        // get the list number or create a new one adding it to the table
        _listNumber = Document.GetDocumentHeader().GetListNumber(this);
    }

    /// <summary>
    ///     Set the list ID number
    ///     @since 2.1.3
    /// </summary>
    /// <param name="id"></param>
    public void SetId(int id)
    {
        _listId = id;
    }

    /// <summary>
    ///     Sets the id of this list
    ///     @since 2.1.3
    /// </summary>
    /// <param name="listNumber">The list number to set.</param>
    public void SetListNumber(int listNumber)
    {
        _listNumber = listNumber;
    }

    /// <summary>
    ///     @see RtfList#LIST_TYPE_NORMAL
    ///     @see RtfList#LIST_TYPE_SIMPLE
    ///     @see RtfList#LIST_TYPE_HYBRID
    ///     @since 2.1.3
    /// </summary>
    /// <param name="listType">the listType to set</param>
    public void SetListType(int listType)
    {
        if (listType == LIST_TYPE_NORMAL ||
            listType == LIST_TYPE_SIMPLE ||
            listType == LIST_TYPE_HYBRID)
        {
            _listType = listType;
        }
        else
        {
            throw new ArgumentException("Invalid listType value.");
        }
    }

    /// <summary>
    ///     @since 2.1.3
    /// </summary>
    /// <param name="name">the name to set</param>
    public void SetName(string name)
    {
        _name = name;
    }

    /// <summary>
    ///     @since 2.1.3
    /// </summary>
    /// <param name="parentList">the parentList to set</param>
    public void SetParentList(RtfList parentList)
    {
        _parentList = parentList;
    }

    /// <summary>
    ///     Correct the indentation of this RtfList by adding left/first line indentation
    ///     from the parent RtfList. Also calls correctIndentation on all child RtfLists.
    ///     @since 2.1.3
    /// </summary>
    protected internal void CorrectIndentation()
    {
        // TODO: Fix
        //        if (this.parentList != null) {
        //            this.leftIndent = this.leftIndent + this.parentList.GetLeftIndent() + this.parentList.GetFirstIndent();
        //        }
        for (var i = 0; i < _items.Count; i++)
        {
            if (_items[i] is RtfList)
            {
                ((RtfList)_items[i]).CorrectIndentation();
            }
            else if (_items[i] is RtfListItem)
            {
                ((RtfListItem)_items[i]).CorrectIndentation();
            }
        }
    }

    /// <summary>
    ///     Create a default set of listlevels
    ///     @since 2.1.3
    /// </summary>
    protected void CreateDefaultLevels()
    {
        _listLevels = new List<RtfListLevel>(); // listlevels
        for (var i = 0; i <= 8; i++)
        {
            // create a list level
            var ll = new RtfListLevel(Document);
            ll.SetListType(RtfListLevel.LIST_TYPE_NUMBERED);
            ll.SetFirstIndent(0);
            ll.SetLeftIndent(0);
            ll.SetLevelTextNumber(i);
            ll.SetTentative(true);
            ll.CorrectIndentation();
            _listLevels.Add(ll);
        }
    }

    /// <summary>
    ///     Writes only the list number and list level number.
    ///     @throws IOException On i/o errors.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="result">The  Stream  to write to</param>
    protected void WriteListNumbers(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        result.Write(ListNumber, 0, ListNumber.Length);
        result.Write(t = IntToByteArray(_listNumber), 0, t.Length);
    }

    /// <summary>
    ///     @throws IOException
    ///     @since 2.1.3
    /// </summary>
    /// <param name="result"></param>
    /// <param name="itemNr"></param>
    /// <param name="listLevel"></param>
    protected void WriteListTextBlock(Stream result, int itemNr, RtfListLevel listLevel)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (listLevel == null)
        {
            throw new ArgumentNullException(nameof(listLevel));
        }

        byte[] t;
        result.Write(OpenGroup, 0, OpenGroup.Length);
        result.Write(ListText, 0, ListText.Length);
        result.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
        if (InTable)
        {
            result.Write(RtfPhrase.InTable, 0, RtfPhrase.InTable.Length);
        }

        result.Write(RtfFontList.FontNumber, 0, RtfFontList.FontNumber.Length);
        if (listLevel.GetListType() != RtfListLevel.LIST_TYPE_BULLET)
        {
            result.Write(t = IntToByteArray(listLevel.GetFontNumber().GetFontNumber()), 0, t.Length);
        }
        else
        {
            result.Write(t = IntToByteArray(listLevel.GetFontBullet().GetFontNumber()), 0, t.Length);
        }

        listLevel.WriteIndentation(result);
        result.Write(Delimiter, 0, Delimiter.Length);
        if (listLevel.GetListType() != RtfListLevel.LIST_TYPE_BULLET)
        {
            switch (listLevel.GetListType())
            {
                case RtfListLevel.LIST_TYPE_NUMBERED:
                    result.Write(t = IntToByteArray(itemNr), 0, t.Length);
                    break;
                case RtfListLevel.LIST_TYPE_UPPER_LETTERS:
                    result.Write(t = DocWriter.GetIsoBytes(RomanAlphabetFactory.GetUpperCaseString(itemNr)), 0,
                                 t.Length);
                    break;
                case RtfListLevel.LIST_TYPE_LOWER_LETTERS:
                    result.Write(t = DocWriter.GetIsoBytes(RomanAlphabetFactory.GetLowerCaseString(itemNr)), 0,
                                 t.Length);
                    break;
                case RtfListLevel.LIST_TYPE_UPPER_ROMAN:
                    result.Write(t = DocWriter.GetIsoBytes(RomanNumberFactory.GetUpperCaseString(itemNr)), 0, t.Length);
                    break;
                case RtfListLevel.LIST_TYPE_LOWER_ROMAN:
                    result.Write(t = DocWriter.GetIsoBytes(RomanNumberFactory.GetLowerCaseString(itemNr)), 0, t.Length);
                    break;
            }

            result.Write(ListNumberEnd, 0, ListNumberEnd.Length);
        }
        else
        {
            Document.FilterSpecialChar(result, listLevel.GetBulletCharacter(), true, false);
        }

        result.Write(Tab, 0, Tab.Length);
        result.Write(CloseGroup, 0, CloseGroup.Length);
    }
}