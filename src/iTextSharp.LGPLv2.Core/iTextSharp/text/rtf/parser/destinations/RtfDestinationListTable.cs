using iTextSharp.text.rtf.list;
using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.destinations
{

    /// <summary>
    ///  RtfDestinationListTable  handles data destined for the List Table destination
    /// @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    public class RtfDestinationListTable : RtfDestination
    {
        private int _currentLevel = -1;

        private RtfListLevel _currentListLevel;

        private int _currentListMappingNumber;

        private int _currentSubGroupCount;

        /// <summary>
        /// The RtfImportHeader to add List mappings to.
        /// </summary>
        private RtfImportMgr _importHeader;

        private RtfList _newList;

        public RtfDestinationListTable() : base(null)
        {
        }

        public RtfDestinationListTable(RtfParser parser) : base(parser)
        {
            _importHeader = parser.GetImportManager();
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
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
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
        /// </summary>
        public override bool HandleCharacter(int ch)
        {
            // TODO Auto-generated method stub
            return true;
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
        /// </summary>
        public override bool HandleCloseGroup()
        {
            _currentSubGroupCount--;
            if (_newList != null && _currentSubGroupCount == 0)
            {
                _importHeader.ImportList(_currentListMappingNumber.ToString(), _newList.GetListNumber().ToString());
                RtfParser.GetRtfDocument().Add(_newList);
            }
            return true;
        }

        public override bool HandleControlWord(RtfCtrlWordData ctrlWordData)
        {
            bool result = true;
            bool skipCtrlWord = false;

            if (RtfParser.IsImport())
            {
                skipCtrlWord = true;
                if (ctrlWordData.CtrlWord.Equals("listtable"))
                {
                    result = true;
                    _currentListMappingNumber = 0;

                }
                else
                    /* Picture info for icons/images for lists */
                    if (ctrlWordData.CtrlWord.Equals("listpicture"))/* DESTINATION */
                {
                    skipCtrlWord = true;
                    // this.rtfParser.SetTokeniserStateSkipGroup();
                    result = true;
                }
                else
                    /* list */
                    if (ctrlWordData.CtrlWord.Equals("list")) /* DESTINATION */
                {
                    skipCtrlWord = true;
                    _newList = new RtfList(RtfParser.GetRtfDocument());
                    _newList.SetListType(RtfList.LIST_TYPE_NORMAL); // set default
                    _currentLevel = -1;
                    _currentListMappingNumber++;
                    _currentSubGroupCount = 0;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("listtemplateid")) /* // List item*/
                {
                    // ignore this because it gets regenerated in every document
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("listsimple")) /* // List item*/
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
                else if (ctrlWordData.CtrlWord.Equals("listhybrid")) /* // List item*/
                {
                    _newList.SetListType(RtfList.LIST_TYPE_HYBRID);
                    skipCtrlWord = true;
                    result = true;
                    // this gets set internally. Don't think it should be imported
                }
                else if (ctrlWordData.CtrlWord.Equals("listrestarthdn")) /* // List item*/
                {
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("listid"))
                {    // List item cannot be between -1 and -5
                    // needs to be mapped for imports and is recreated
                    // we have the new id and the old id. Just add it to the mapping table here.
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("listname"))/* // List item*/
                {
                    _newList.SetName(ctrlWordData.Param);
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("liststyleid"))/* // List item*/
                {
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("liststylename"))/* // List item*/
                {
                    skipCtrlWord = true;
                    result = true;
                }
                else
                  /* listlevel */
                  if (ctrlWordData.CtrlWord.Equals("listlevel")) /* DESTINATION There are 1 or 9 listlevels per list */
                {
                    _currentLevel++;
                    _currentListLevel = _newList.GetListLevel(_currentLevel);
                    _currentListLevel.SetTentative(false);
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("leveljc"))
                { // listlevel item justify
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
                else if (ctrlWordData.CtrlWord.Equals("leveljcn"))
                { // listlevel item
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
                else if (ctrlWordData.CtrlWord.Equals("levelstartat"))
                {
                    _currentListLevel.SetListStartAt(ctrlWordData.IntValue());
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("lvltentative"))
                {
                    _currentListLevel.SetTentative(true);
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelold"))
                {
                    // old style. ignore
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelprev"))
                {
                    // old style. ignore
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelprevspace"))
                {
                    // old style. ignore
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelspace"))
                {
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelindent"))
                {
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("leveltext"))
                {/* FIX */
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelfollow"))
                {
                    _currentListLevel.SetLevelFollowValue(ctrlWordData.IntValue());
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levellegal"))
                {
                    _currentListLevel.SetLegal(ctrlWordData.Param == "1" ? true : false);
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelnorestart"))
                {
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("chrfmt"))
                {/* FIX */
                    // set an attribute pair
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelpicture"))
                {
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("li"))
                {
                    // set an attribute pair
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("fi"))
                {
                    // set an attribute pair
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("jclisttab"))
                {
                    // set an attribute pair
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("tx"))
                {
                    // set an attribute pair
                    skipCtrlWord = true;
                    result = true;
                }
                else
                  /* number */
                  if (ctrlWordData.CtrlWord.Equals("levelnfc")) /* old style */
                {
                    if (_currentListLevel.GetListType() == RtfListLevel.LIST_TYPE_UNKNOWN)
                    {
                        _currentListLevel.SetListType(ctrlWordData.IntValue() + RtfListLevel.LIST_TYPE_BASE);
                    }
                    skipCtrlWord = true;
                    result = true;
                }
                else if (ctrlWordData.CtrlWord.Equals("levelnfcn")) /* new style takes priority over levelnfc.*/
                {
                    _currentListLevel.SetListType(ctrlWordData.IntValue() + RtfListLevel.LIST_TYPE_BASE);
                    skipCtrlWord = true;
                    result = true;
                }
                else
                  /* level text */
                  if (ctrlWordData.CtrlWord.Equals("leveltemplateid"))
                {
                    // ignore. this value is regenerated in each document.
                    skipCtrlWord = true;
                    result = true;
                }
                else
                  /* levelnumber */
                  if (ctrlWordData.CtrlWord.Equals("levelnumbers"))
                {
                    skipCtrlWord = true;
                    result = true;
                }
            }

            if (RtfParser.IsConvert())
            {
                if (ctrlWordData.CtrlWord.Equals("shppict"))
                {
                    result = true;
                }
                if (ctrlWordData.CtrlWord.Equals("nonshppict"))
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
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
        /// </summary>
        public override bool HandleOpenGroup()
        {
            // TODO Auto-generated method stub
            return true;
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
        /// </summary>
        public override bool HandleOpeningSubGroup()
        {
            _currentSubGroupCount++;
            return true;
        }

        public override void SetParser(RtfParser parser)
        {
            RtfParser = parser;
            _importHeader = parser.GetImportManager();
            SetToDefaults();
        }
        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.parser.destinations.RtfDestination#setToDefaults()
        /// </summary>
        public override void SetToDefaults()
        {
            // TODO Auto-generated method stub

        }

    }
}