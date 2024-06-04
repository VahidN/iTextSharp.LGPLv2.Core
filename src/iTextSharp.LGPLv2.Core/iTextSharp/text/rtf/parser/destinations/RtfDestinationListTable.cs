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

    public RtfDestinationListTable() : base(null)
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
    public override bool HandleCharacter(int ch) =>
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
            if (ctrlWordData.CtrlWord.Equals("listtable", StringComparison.Ordinal))
            {
                result = true;
                _currentListMappingNumber = 0;
            }
            else
                /* Picture info for icons/images for lists */
            if (ctrlWordData.CtrlWord.Equals("listpicture", StringComparison.Ordinal)) /* DESTINATION */
            {
                skipCtrlWord = true;
                // this.rtfParser.SetTokeniserStateSkipGroup();
                result = true;
            }
            else
                /* list */
            if (ctrlWordData.CtrlWord.Equals("list", StringComparison.Ordinal)) /* DESTINATION */
            {
                skipCtrlWord = true;
                _newList = new RtfList(RtfParser.GetRtfDocument());
                _newList.SetListType(RtfList.LIST_TYPE_NORMAL); // set default
                _currentLevel = -1;
                _currentListMappingNumber++;
                _currentSubGroupCount = 0;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("listtemplateid", StringComparison.Ordinal)) /* // List item*/
            {
                // ignore this because it gets regenerated in every document
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("listsimple", StringComparison.Ordinal)) /* // List item*/
            {
                // is value 0 or 1
                if (ctrlWordData.HasParam && ctrlWordData.Param == "1")
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
            else if (ctrlWordData.CtrlWord.Equals("listhybrid", StringComparison.Ordinal)) /* // List item*/
            {
                _newList.SetListType(RtfList.LIST_TYPE_HYBRID);
                skipCtrlWord = true;
                result = true;
                // this gets set internally. Don't think it should be imported
            }
            else if (ctrlWordData.CtrlWord.Equals("listrestarthdn", StringComparison.Ordinal)) /* // List item*/
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("listid", StringComparison.Ordinal))
            {
                // List item cannot be between -1 and -5
                // needs to be mapped for imports and is recreated
                // we have the new id and the old id. Just add it to the mapping table here.
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("listname", StringComparison.Ordinal)) /* // List item*/
            {
                _newList.SetName(ctrlWordData.Param);
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("liststyleid", StringComparison.Ordinal)) /* // List item*/
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("liststylename", StringComparison.Ordinal)) /* // List item*/
            {
                skipCtrlWord = true;
                result = true;
            }
            else
                /* listlevel */
            if (ctrlWordData.CtrlWord.Equals("listlevel",
                                             StringComparison
                                                 .Ordinal)) /* DESTINATION There are 1 or 9 listlevels per list */
            {
                _currentLevel++;
                _currentListLevel = _newList.GetListLevel(_currentLevel);
                _currentListLevel.SetTentative(false);
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("leveljc", StringComparison.Ordinal))
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
            else if (ctrlWordData.CtrlWord.Equals("leveljcn", StringComparison.Ordinal))
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
            else if (ctrlWordData.CtrlWord.Equals("levelstartat", StringComparison.Ordinal))
            {
                _currentListLevel.SetListStartAt(ctrlWordData.IntValue());
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("lvltentative", StringComparison.Ordinal))
            {
                _currentListLevel.SetTentative(true);
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelold", StringComparison.Ordinal))
            {
                // old style. ignore
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelprev", StringComparison.Ordinal))
            {
                // old style. ignore
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelprevspace", StringComparison.Ordinal))
            {
                // old style. ignore
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelspace", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelindent", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("leveltext", StringComparison.Ordinal))
            {
                /* FIX */
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelfollow", StringComparison.Ordinal))
            {
                _currentListLevel.SetLevelFollowValue(ctrlWordData.IntValue());
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levellegal", StringComparison.Ordinal))
            {
                _currentListLevel.SetLegal(ctrlWordData.Param == "1");
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelnorestart", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("chrfmt", StringComparison.Ordinal))
            {
                /* FIX */
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelpicture", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("li", StringComparison.Ordinal))
            {
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("fi", StringComparison.Ordinal))
            {
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("jclisttab", StringComparison.Ordinal))
            {
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("tx", StringComparison.Ordinal))
            {
                // set an attribute pair
                skipCtrlWord = true;
                result = true;
            }
            else
                /* number */
            if (ctrlWordData.CtrlWord.Equals("levelnfc", StringComparison.Ordinal)) /* old style */
            {
                if (_currentListLevel.GetListType() == RtfListLevel.LIST_TYPE_UNKNOWN)
                {
                    _currentListLevel.SetListType(ctrlWordData.IntValue() + RtfListLevel.LIST_TYPE_BASE);
                }

                skipCtrlWord = true;
                result = true;
            }
            else if (ctrlWordData.CtrlWord.Equals("levelnfcn",
                                                  StringComparison
                                                      .Ordinal)) /* new style takes priority over levelnfc.*/
            {
                _currentListLevel.SetListType(ctrlWordData.IntValue() + RtfListLevel.LIST_TYPE_BASE);
                skipCtrlWord = true;
                result = true;
            }
            else
                /* level text */
            if (ctrlWordData.CtrlWord.Equals("leveltemplateid", StringComparison.Ordinal))
            {
                // ignore. this value is regenerated in each document.
                skipCtrlWord = true;
                result = true;
            }
            else
                /* levelnumber */
            if (ctrlWordData.CtrlWord.Equals("levelnumbers", StringComparison.Ordinal))
            {
                skipCtrlWord = true;
                result = true;
            }
        }

        if (RtfParser.IsConvert())
        {
            if (ctrlWordData.CtrlWord.Equals("shppict", StringComparison.Ordinal))
            {
                result = true;
            }

            if (ctrlWordData.CtrlWord.Equals("nonshppict", StringComparison.Ordinal))
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
    public override bool HandleOpenGroup() =>
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