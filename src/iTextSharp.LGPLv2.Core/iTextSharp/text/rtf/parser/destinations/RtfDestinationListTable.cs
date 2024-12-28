using iTextSharp.text.rtf.list;
using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationListTable  handles data destined for the List Table destination
///     @author Howard Shank (hgshank@yahoo.com)
/// </summary>
public class RtfDestinationListTable : RtfDestination
{
    private int _currentLevel = -1;

    private RtfListLevel _currentListLevel;

    private int _currentListMappingNumber;

    private int _currentSubGroupCount;

    /// <summary>
    ///     The RtfImportHeader to add List mappings to.
    /// </summary>
    private RtfImportMgr _importHeader;

    private RtfList _newList;

    public RtfDestinationListTable() : base(parser: null)
    {
    }

    public RtfDestinationListTable(RtfParser parser) : base(parser)
    {
        if (parser == null)
        {
            throw new ArgumentNullException(nameof(parser));
        }

        _importHeader = parser.GetImportManager();
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
    /// </summary>
    public override bool CloseDestination()
    {
        if (_newList != null)
        {
            RtfParser.GetRtfDocument().Add(_newList);
        }

        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
    /// </summary>
    public override bool HandleCharacter(int ch)
        =>

            // TODO Auto-generated method stub
            true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
    /// </summary>
    public override bool HandleCloseGroup()
    {
        _currentSubGroupCount--;

        if (_newList != null && _currentSubGroupCount == 0)
        {
            _importHeader.ImportList(_currentListMappingNumber.ToString(CultureInfo.InvariantCulture),
                _newList.GetListNumber().ToString(CultureInfo.InvariantCulture));

            RtfParser.GetRtfDocument().Add(_newList);
        }

        return true;
    }

    public override bool HandleControlWord(RtfCtrlWordData ctrlWordData)
    {
        if (ctrlWordData == null)
        {
            throw new ArgumentNullException(nameof(ctrlWordData));
        }

        var result = true;
        var skipCtrlWord = false;

        if (RtfParser.IsImport())
        {
            skipCtrlWord = true;

            if (ctrlWordData.CtrlWord.Equals(value: "listtable", StringComparison.Ordinal))
            {
                result = true;
                _currentListMappingNumber = 0;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "listpicture", StringComparison.Ordinal)) /* DESTINATION */
            {
                skipCtrlWord = true;

                // this.rtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "list", StringComparison.Ordinal)) /* DESTINATION */
            {
                skipCtrlWord = true;
                _newList = new RtfList(RtfParser.GetRtfDocument());
                _newList.SetListType(RtfList.LIST_TYPE_NORMAL); // set default
                _currentLevel = -1;
                _currentListMappingNumber++;
                _currentSubGroupCount = 0;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "listtemplateid", StringComparison.Ordinal)) /* // List item*/
            {
                // ignore this because it gets regenerated in every document
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "listsimple", StringComparison.Ordinal)) /* // List item*/
            {
                // is value 0 or 1
                if (ctrlWordData.HasParam && string.Equals(ctrlWordData.Param, b: "1", StringComparison.Ordinal))
                {
                    _newList.SetListType(RtfList.LIST_TYPE_SIMPLE);
                }
                else
                {
                    _newList.SetListType(RtfList.LIST_TYPE_NORMAL);
                }

                skipCtrlWord = true;
                result = true;

                // this gets set internally. Don't think it should be imported
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "listhybrid", StringComparison.Ordinal)) /* // List item*/
            {
                _newList.SetListType(RtfList.LIST_TYPE_HYBRID);
                skipCtrlWord = true;
                result = true;

                // this gets set internally. Don't think it should be imported
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "listrestarthdn", StringComparison.Ordinal)) /* // List item*/
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "listid", StringComparison.Ordinal))
            {
                // List item cannot be between -1 and -5
                // needs to be mapped for imports and is recreated
                // we have the new id and the old id. Just add it to the mapping table here.
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "listname", StringComparison.Ordinal)) /* // List item*/
            {
                _newList.SetName(ctrlWordData.Param);
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "liststyleid", StringComparison.Ordinal)) /* // List item*/
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "liststylename", StringComparison.Ordinal)) /* // List item*/
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "listlevel",
                         StringComparison.Ordinal)) /* DESTINATION There are 1 or 9 listlevels per list */
            {
                _currentLevel++;
                _currentListLevel = _newList.GetListLevel(_currentLevel);
                _currentListLevel.SetTentative(isTentative: false);
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "leveljc", StringComparison.Ordinal))
            {
                // listlevel item justify
                // this is the old number. Only use it if the current type is not set
                if (_currentListLevel.GetAlignment() == RtfListLevel.LIST_TYPE_UNKNOWN)
                {
                    switch (ctrlWordData.IntValue())
                    {
                        case 0:
                            _currentListLevel.SetAlignment(Element.ALIGN_LEFT);

                            break;
                        case 1:
                            _currentListLevel.SetAlignment(Element.ALIGN_CENTER);

                            break;
                        case 2:
                            _currentListLevel.SetAlignment(Element.ALIGN_RIGHT);

                            break;
                    }
                }

                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "leveljcn", StringComparison.Ordinal))
            {
                // listlevel item
                //justify
                // if this exists, use it and it overrides the old setting
                switch (ctrlWordData.IntValue())
                {
                    case 0:
                        _currentListLevel.SetAlignment(Element.ALIGN_LEFT);

                        break;
                    case 1:
                        _currentListLevel.SetAlignment(Element.ALIGN_CENTER);

                        break;
                    case 2:
                        _currentListLevel.SetAlignment(Element.ALIGN_RIGHT);

                        break;
                }

                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelstartat", StringComparison.Ordinal))
            {
                _currentListLevel.SetListStartAt(ctrlWordData.IntValue());
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "lvltentative", StringComparison.Ordinal))
            {
                _currentListLevel.SetTentative(isTentative: true);
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelold", StringComparison.Ordinal))
            {
                // old style. ignore
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelprev", StringComparison.Ordinal))
            {
                // old style. ignore
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelprevspace", StringComparison.Ordinal))
            {
                // old style. ignore
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelspace", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelindent", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "leveltext", StringComparison.Ordinal))
            {
                /* FIX */
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelfollow", StringComparison.Ordinal))
            {
                _currentListLevel.SetLevelFollowValue(ctrlWordData.IntValue());
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levellegal", StringComparison.Ordinal))
            {
                _currentListLevel.SetLegal(string.Equals(ctrlWordData.Param, b: "1", StringComparison.Ordinal));
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelnorestart", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "chrfmt", StringComparison.Ordinal))
            {
                /* FIX */
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelpicture", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "li", StringComparison.Ordinal))
            {
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "fi", StringComparison.Ordinal))
            {
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "jclisttab", StringComparison.Ordinal))
            {
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "tx", StringComparison.Ordinal))
            {
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelnfc", StringComparison.Ordinal)) /* old style */
            {
                if (_currentListLevel.GetListType() == RtfListLevel.LIST_TYPE_UNKNOWN)
                {
                    _currentListLevel.SetListType(ctrlWordData.IntValue() + RtfListLevel.LIST_TYPE_BASE);
                }

                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelnfcn",
                         StringComparison.Ordinal)) /* new style takes priority over levelnfc.*/
            {
                _currentListLevel.SetListType(ctrlWordData.IntValue() + RtfListLevel.LIST_TYPE_BASE);
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "leveltemplateid", StringComparison.Ordinal)) /* level text */
            {
                // ignore. this value is regenerated in each document.
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals(value: "levelnumbers", StringComparison.Ordinal)) /* levelnumber */
            {
                skipCtrlWord = true;
                result = true;
            }
        }

        if (RtfParser.IsConvert())
        {
            if (ctrlWordData.CtrlWord.Equals(value: "shppict", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals(value: "nonshppict", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                RtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }
        }

        if (!skipCtrlWord)
        {
            switch (RtfParser.GetConversionType())
            {
                case RtfParser.TYPE_IMPORT_FULL:
                    // WriteBuffer();
                    // WriteText(ctrlWordData.ToString());
                    result = true;

                    break;
                case RtfParser.TYPE_IMPORT_FRAGMENT:
                    // WriteBuffer();
                    // WriteText(ctrlWordData.ToString());
                    result = true;

                    break;
                case RtfParser.TYPE_CONVERT:
                    result = true;

                    break;
                default: // error because is should be an import or convert
                    result = false;

                    break;
            }
        }

        return result;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
    /// </summary>
    public override bool HandleOpenGroup()
        =>

            // TODO Auto-generated method stub
            true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
    /// </summary>
    public override bool HandleOpeningSubGroup()
    {
        _currentSubGroupCount++;

        return true;
    }

    public override void SetParser(RtfParser parser)
    {
        RtfParser = parser ?? throw new ArgumentNullException(nameof(parser));
        _importHeader = parser.GetImportManager();
        SetToDefaults();
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#setToDefaults()
    /// </summary>
    public override void SetToDefaults()
    {
        // TODO Auto-generated method stub
    }
}