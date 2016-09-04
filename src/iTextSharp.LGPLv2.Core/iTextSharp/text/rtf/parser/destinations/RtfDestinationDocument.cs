using System.Collections;
using System.Text;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.direct;
using iTextSharp.text.rtf.parser.ctrlwords;
using iTextSharp.text.rtf.parser.properties;

namespace iTextSharp.text.rtf.parser.destinations
{
    /// <summary>
    ///  RtfDestinationDocument  handles data destined for the document destination
    /// @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    public sealed class RtfDestinationDocument : RtfDestination, IRtfPropertyListener
    {


        private static readonly ArrayList _importIgnoredCtrlwords = new ArrayList(new[]{
            "rtf",
            "ansicpg",
            "deff",
            "ansi",
            "mac",
            "pca",
            "pc",
            "stshfdbch",
            "stshfloch",
            "stshfhich",
            "stshfbi",
            "deflang",
            "deflangfe",
            "adeflang",
            "adeflangfe"
            }
                );

        private static ArrayList _convertIgnoredCtrlwords = new ArrayList(new[] { "rtf" });

        private StringBuilder _buffer;

        /// <summary>
        /// Indicates the parser action. Import or Conversion.
        /// @see com.lowagie.text.rtf.direct.RtfParser#TYPE_UNIDENTIFIED
        /// @see com.lowagie.text.rtf.direct.RtfParser#TYPE_CONVERT
        /// @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FRAGMENT
        /// @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FULL
        /// </summary>
        private int _conversionType;

        /// <summary>
        /// The iText Document object.
        /// @see com.lowagie.text.Document
        /// </summary>
        private Document _doc;

        private Paragraph _iTextParagraph;

        /// <summary>
        /// The RtfDocument object.
        /// @see com.lowagie.text.rtf.document.RtfDocument
        /// </summary>
        private RtfDocument _rtfDoc;
        /// <summary>
        /// Indicates the current table level being processed
        /// </summary>
        private int _tableLevel;
        public RtfDestinationDocument() : base(null)
        {
        }
        /// <summary>
        /// Constructs a new  RtfDestinationDocument  using
        /// the parameters to initialize the object.
        /// </summary>
        public RtfDestinationDocument(RtfParser parser) : base(parser)
        {
            _rtfDoc = parser.GetRtfDocument();
            _doc = parser.GetDocument();
            _conversionType = parser.GetConversionType();
            SetToDefaults();
            if (RtfParser.IsConvert())
            {
                RtfParser.GetState().Properties.AddRtfPropertyListener(this);
            }
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.parser.properties.RtfPropertyListener#afterChange(java.lang.String)
        /// </summary>
        public void AfterPropertyChange(string propertyName)
        {
            if (propertyName.StartsWith(RtfProperty.CHARACTER))
            {
            }
            else
            {
                if (propertyName.StartsWith(RtfProperty.PARAGRAPH))
                {
                }
                else
                {
                    if (propertyName.StartsWith(RtfProperty.SECTION))
                    {
                    }
                    else
                    {
                        if (propertyName.StartsWith(RtfProperty.DOCUMENT))
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.parser.properties.RtfPropertyListener#beforeChange(java.lang.String)
        /// </summary>
        public void BeforePropertyChange(string propertyName)
        {
            // do we have any text to do anything with?
            // if not, then just return without action.
            if (_buffer.Length == 0) return;

            if (propertyName.StartsWith(RtfProperty.CHARACTER))
            {
                // this is a character change,
                // add a new chunck to the current paragraph using current character settings.
                Chunk chunk = new Chunk();
                chunk.Append(_buffer.ToString());
                _buffer = new StringBuilder(255);
                Hashtable charProperties = RtfParser.GetState().Properties.GetProperties(RtfProperty.CHARACTER);
                string defFont = (string)charProperties[RtfProperty.CHARACTER_FONT];
                if (defFont == null) defFont = "0";
                RtfDestinationFontTable fontTable = (RtfDestinationFontTable)RtfParser.GetDestination("fonttbl");
                Font currFont = fontTable.GetFont(defFont);
                int fs = Font.NORMAL;
                if (charProperties.ContainsKey(RtfProperty.CHARACTER_BOLD)) fs |= Font.BOLD;
                if (charProperties.ContainsKey(RtfProperty.CHARACTER_ITALIC)) fs |= Font.ITALIC;
                if (charProperties.ContainsKey(RtfProperty.CHARACTER_UNDERLINE)) fs |= Font.UNDERLINE;
                Font useFont = FontFactory.GetFont(currFont.Familyname, 12, fs, new BaseColor(0, 0, 0));


                chunk.Font = useFont;
                if (_iTextParagraph == null) _iTextParagraph = new Paragraph();
                _iTextParagraph.Add(chunk);

            }
            else
            {
                if (propertyName.StartsWith(RtfProperty.PARAGRAPH))
                {
                    // this is a paragraph change. what do we do?
                }
                else
                {
                    if (propertyName.StartsWith(RtfProperty.SECTION))
                    {

                    }
                    else
                    {
                        if (propertyName.StartsWith(RtfProperty.DOCUMENT))
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
        /// </summary>
        public override bool CloseDestination()
        {
            if (RtfParser.IsImport())
            {
                if (_buffer.Length > 0)
                {
                    writeBuffer();
                }
            }

            RtfParser.GetState().Properties.RemoveRtfPropertyListener(this);

            return true;
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
        /// </summary>
        public override bool HandleCharacter(int ch)
        {
            bool result = true;
            OnCharacter(ch);   // event handler

            if (RtfParser.IsImport())
            {
                if (_buffer.Length > 254)
                {
                    writeBuffer();
                }
                _buffer.Append((char)ch);
            }
            if (RtfParser.IsConvert())
            {
                _buffer.Append((char)ch);
            }
            return result;
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
        /// </summary>
        public override bool HandleCloseGroup()
        {
            OnCloseGroup();    // event handler

            if (RtfParser.IsImport())
            {
                if (_buffer.Length > 0)
                {
                    writeBuffer();
                }
                writeText("}");
            }
            if (RtfParser.IsConvert())
            {
                if (_buffer.Length > 0 && _iTextParagraph == null)
                {
                    _iTextParagraph = new Paragraph();
                }
                if (_buffer.Length > 0)
                {
                    Chunk chunk = new Chunk();
                    chunk.Append(_buffer.ToString());
                    _iTextParagraph.Add(chunk);
                }
                if (_iTextParagraph != null)
                {
                    addParagraphToDocument();
                }
            }
            return true;
        }

        public override bool HandleControlWord(RtfCtrlWordData ctrlWordData)
        {
            bool result = false;
            OnCtrlWord(ctrlWordData);  // event handler

            if (RtfParser.IsImport())
            {
                // map font information
                if (ctrlWordData.CtrlWord.Equals("f")) { ctrlWordData.Param = RtfParser.GetImportManager().MapFontNr(ctrlWordData.Param); }

                // map color information
                //colors
                if (ctrlWordData.CtrlWord.Equals("cb")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                if (ctrlWordData.CtrlWord.Equals("cf")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                //cells
                if (ctrlWordData.CtrlWord.Equals("clcbpat")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                if (ctrlWordData.CtrlWord.Equals("clcbpatraw")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                if (ctrlWordData.CtrlWord.Equals("clcfpat")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                if (ctrlWordData.CtrlWord.Equals("clcfpatraw")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                //table rows
                if (ctrlWordData.CtrlWord.Equals("trcfpat")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                if (ctrlWordData.CtrlWord.Equals("trcbpat")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                //paragraph border
                if (ctrlWordData.CtrlWord.Equals("brdrcf")) { ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param); }
                // map lists
                if (ctrlWordData.CtrlWord.Equals("ls")) { ctrlWordData.Param = RtfParser.GetImportManager().MapListNr(ctrlWordData.Param); }
            }



            if (RtfParser.IsConvert())
            {
                if (ctrlWordData.CtrlWord.Equals("par")) { addParagraphToDocument(); }
                // Set Font
                if (ctrlWordData.CtrlWord.Equals("f")) { }

                // color information
                //colors
                if (ctrlWordData.CtrlWord.Equals("cb")) { }
                if (ctrlWordData.CtrlWord.Equals("cf")) { }
                //cells
                if (ctrlWordData.CtrlWord.Equals("clcbpat")) { }
                if (ctrlWordData.CtrlWord.Equals("clcbpatraw")) { }
                if (ctrlWordData.CtrlWord.Equals("clcfpat")) { }
                if (ctrlWordData.CtrlWord.Equals("clcfpatraw")) { }
                //table rows
                if (ctrlWordData.CtrlWord.Equals("trcfpat")) { }
                if (ctrlWordData.CtrlWord.Equals("trcbpat")) { }
                //paragraph border
                if (ctrlWordData.CtrlWord.Equals("brdrcf")) { }

                /* TABLES */
                if (ctrlWordData.CtrlWord.Equals("trowd")) /*Beginning of row*/ { _tableLevel++; }
                if (ctrlWordData.CtrlWord.Equals("cell")) /*End of Cell Denotes the end of a table cell*/
                {
                    //              String ctl = ctrlWordData.ctrlWord;
                    //              System.out.Print("cell found");
                }
                if (ctrlWordData.CtrlWord.Equals("row")) /*End of row*/ { _tableLevel++; }
                if (ctrlWordData.CtrlWord.Equals("lastrow")) /*Last row of the table*/ { }
                if (ctrlWordData.CtrlWord.Equals("row")) /*End of row*/ { _tableLevel++; }
                if (ctrlWordData.CtrlWord.Equals("irow")) /*param  is the row index of this row.*/ { }
                if (ctrlWordData.CtrlWord.Equals("irowband")) /*param is the row index of the row, adjusted to account for header rows. A header row has a value of -1.*/ { }
                if (ctrlWordData.CtrlWord.Equals("tcelld")) /*Sets table cell defaults*/ { }
                if (ctrlWordData.CtrlWord.Equals("nestcell")) /*Denotes the end of a nested cell.*/ { }
                if (ctrlWordData.CtrlWord.Equals("nestrow")) /*Denotes the end of a nested row*/ { }
                if (ctrlWordData.CtrlWord.Equals("nesttableprops")) /*Defines the properties of a nested table. This is a destination control word*/ { }
                if (ctrlWordData.CtrlWord.Equals("nonesttables")) /*Contains text for readers that do not understand nested tables. This destination should be ignored by readers that support nested tables.*/ { }
                if (ctrlWordData.CtrlWord.Equals("trgaph")) /*Half the space between the cells of a table row in twips.*/ { }
                if (ctrlWordData.CtrlWord.Equals("cellx")) /*param Defines the right boundary of a table cell, including its half of the space between cells.*/ { }
                if (ctrlWordData.CtrlWord.Equals("clmgf")) /*The first cell in a range of table cells to be merged.*/ { }
                if (ctrlWordData.CtrlWord.Equals("clmrg")) /*Contents of the table cell are merged with those of the preceding cell*/ { }
                if (ctrlWordData.CtrlWord.Equals("clvmgf")) /*The first cell in a range of table cells to be vertically merged.*/ { }
                if (ctrlWordData.CtrlWord.Equals("clvmrg")) /*Contents of the table cell are vertically merged with those of the preceding cell*/ { }
                /* TABLE: table row revision tracking */
                if (ctrlWordData.CtrlWord.Equals("trauth")) /*With revision tracking enabled, this control word identifies the author of changes to a table row's properties. N refers to a value in the revision table*/ { }
                if (ctrlWordData.CtrlWord.Equals("trdate")) /*With revision tracking enabled, this control word identifies the date of a revision*/ { }
                /* TABLE: Autoformatting flags */
                if (ctrlWordData.CtrlWord.Equals("tbllkborder")) /*Flag sets table autoformat to format borders*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllkshading")) /*Flag sets table autoformat to affect shading.*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllkfont")) /*Flag sets table autoformat to affect font*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllkcolor")) /*Flag sets table autoformat to affect color*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllkbestfit")) /*Flag sets table autoformat to apply best fit*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllkhdrrows")) /*Flag sets table autoformat to format the first (header) row*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllklastrow")) /*Flag sets table autoformat to format the last row.*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllkhdrcols")) /*Flag sets table autoformat to format the first (header) column*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllklastcol")) /*Flag sets table autoformat to format the last column*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllknorowband")) /*Specifies row banding conditional formatting shall not be applied*/ { }
                if (ctrlWordData.CtrlWord.Equals("tbllknocolband")) /*Specifies column banding conditional formatting shall not be applied.*/ { }
                /* TABLE: Row Formatting */
                if (ctrlWordData.CtrlWord.Equals("taprtl")) /*Table direction is right to left*/ { }
                if (ctrlWordData.CtrlWord.Equals("trautofit")) /*param = AutoFit:
    0   No AutoFit (default).
    1   AutoFit is on for the row. Overridden by \clwWidthN and \trwWidthN in any table row.
    */
                { }
                if (ctrlWordData.CtrlWord.Equals("trhdr")) /*Table row header. This row should appear at the top of every page on which the current table appears*/ { }
                if (ctrlWordData.CtrlWord.Equals("trkeep")) /*Keep table row together. This row cannot be split by a page break. This property is assumed to be off unless the control word is present*/ { }
                if (ctrlWordData.CtrlWord.Equals("trkeepfollow")) /*Keep row in the same page as the following row.*/ { }
                if (ctrlWordData.CtrlWord.Equals("trleft")) /*Position in twips of the leftmost edge of the table with respect to the left edge of its column.*/ { }
                if (ctrlWordData.CtrlWord.Equals("trqc")) /*Centers a table row with respect to its containing column.*/ { }
                if (ctrlWordData.CtrlWord.Equals("trql")) /*Left-justifies a table row with respect to its containing column.*/ { }
                if (ctrlWordData.CtrlWord.Equals("trqr")) /*Right-justifies a table row with respect to its containing column*/ { }
                if (ctrlWordData.CtrlWord.Equals("trrh")) /*Height of a table row in twips. When 0, the height is sufficient for all the text in the line; when positive, the height is guaranteed to be at least the specified height; when negative, the absolute value of the height is used, regardless of the height of the text in the line*/ { }
                if (ctrlWordData.CtrlWord.Equals("trpaddb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trpaddl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trpaddr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trpaddt")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trpaddfb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trpaddfl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trpaddfr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trpaddft")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trspdl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trspdt")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trspdb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trspdr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trspdfl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trspdft")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trspdfb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trspdfr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trwWidth")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trftsWidth")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trwWidthB")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trftsWidthB")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trftsWidthB")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trwWidthA")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trftsWidthA")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tblind")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tblindtype")) /* */ { }
                /*TABLE: Row shading and Background Colors*/
                if (ctrlWordData.CtrlWord.Equals("trcbpat")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trcfpat")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trpat")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trshdng")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgbdiag")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgcross")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgdcross")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgdkbdiag")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgdkcross")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgdkdcross")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgdkfdiag")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgdkhor")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgdkvert")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgfdiag")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbghoriz")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbgvert")) /* */ { }
                /* TABLE: Cell Formatting*/
                if (ctrlWordData.CtrlWord.Equals("clFitText")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clNoWrap")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clpadl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clpadt")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clpadb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clpadr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clpadfl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clpadft")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clpadfb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clpadfr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clwWidth")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clftsWidth")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clhidemark")) /* */ { }
                /* TABLE: Compared Table Cells */
                if (ctrlWordData.CtrlWord.Equals("clins")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("cldel")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clmrgd")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clmrgdr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clsplit")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clsplitr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clinsauth")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clinsdttm")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("cldelauth")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("cldeldttm")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clmrgdauth")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clmrgddttm")) /* */ { }
                /*TABLE: Position Wrapped Tables (The following properties must be the same for all rows in the table.)*/
                if (ctrlWordData.CtrlWord.Equals("tdfrmtxtLeft")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tdfrmtxtRight")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tdfrmtxtTop")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tdfrmtxtBottom")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tabsnoovrlp")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tphcol")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tphmrg")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tphpg")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposnegx")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposnegy")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposx")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposxc")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposxi")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposxl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposxo")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposxr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposy")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposyb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposyc")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposyil")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposyin")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposyout")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tposyt")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tpvmrg")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tpvpara")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("tpvpg")) /* */ { }
                /* TABLE: Bidirectional Controls */
                if (ctrlWordData.CtrlWord.Equals("rtlrow")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("ltrrow")) /* */ { }
                /* TABLE: Row Borders */
                if (ctrlWordData.CtrlWord.Equals("trbrdrt")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbrdrl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbrdrb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbrdrr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbrdrh")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("trbrdrv")) /* */ { }
                /* TABLE: Cell Borders */
                if (ctrlWordData.CtrlWord.Equals("brdrnil")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clbrdrb")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clbrdrt")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clbrdrl")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("clbrdrr")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("cldglu")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("cldgll")) /* */ { }
                if (ctrlWordData.CtrlWord.Equals("")) /* */ { }
            }
            if (ctrlWordData.CtrlWordType == RtfCtrlWordType.TOGGLE)
            {
                RtfParser.GetState().Properties.ToggleProperty(ctrlWordData);//ctrlWordData.specialHandler);
            }

            if (ctrlWordData.CtrlWordType == RtfCtrlWordType.FLAG ||
                    ctrlWordData.CtrlWordType == RtfCtrlWordType.VALUE)
            {
                RtfParser.GetState().Properties.SetProperty(ctrlWordData);//ctrlWordData.specialHandler, ctrlWordData.param);
            }

            switch (_conversionType)
            {
                case RtfParser.TYPE_IMPORT_FULL:
                    if (!_importIgnoredCtrlwords.Contains(ctrlWordData.CtrlWord))
                    {
                        writeBuffer();
                        writeText(ctrlWordData.ToString());
                    }
                    result = true;
                    break;
                case RtfParser.TYPE_IMPORT_FRAGMENT:
                    if (!_importIgnoredCtrlwords.Contains(ctrlWordData.CtrlWord))
                    {
                        writeBuffer();
                        writeText(ctrlWordData.ToString());
                    }
                    result = true;
                    break;
                case RtfParser.TYPE_CONVERT:
                    if (_importIgnoredCtrlwords.Contains(ctrlWordData.CtrlWord) == false)
                    {
                    }
                    result = true;
                    break;
                default:    // error because is should be an import or convert
                    result = false;
                    break;
            }




            return result;
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
        /// </summary>
        public override bool HandleOpenGroup()
        {
            OnOpenGroup(); // event handler

            if (RtfParser.IsImport())
            {
            }
            if (RtfParser.IsConvert())
            {
                if (_iTextParagraph == null) _iTextParagraph = new Paragraph();
            }
            return true;
        }

        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
        /// </summary>
        public override bool HandleOpeningSubGroup()
        {
            if (RtfParser.IsImport())
            {
                if (_buffer.Length > 0)
                {
                    writeBuffer();
                }
            }
            return true;
        }

        public override void SetParser(RtfParser parser)
        {
            RtfParser = parser;
            _rtfDoc = parser.GetRtfDocument();
            _doc = parser.GetDocument();
            _conversionType = parser.GetConversionType();
            SetToDefaults();
            if (RtfParser.IsConvert())
            {
                RtfParser.GetState().Properties.AddRtfPropertyListener(this);
            }
        }
        /// <summary>
        /// (non-Javadoc)
        /// @see com.lowagie.text.rtf.direct.RtfDestination#setDefaults()
        /// </summary>
        public override void SetToDefaults()
        {
            _buffer = new StringBuilder();
        }

        private void addParagraphToDocument()
        {
            if (_iTextParagraph != null)
            {
                try
                {
                    RtfParser.GetDocument().Add(_iTextParagraph);
                }
                catch
                {
                }
                _iTextParagraph = null;
            }
        }

        /// <summary>
        /// Write the accumulated buffer to the destination.
        /// Used for direct content
        /// </summary>
        private void writeBuffer()
        {
            writeText(_buffer.ToString());
            SetToDefaults();
        }
        /// <summary>
        /// Write the string value to the destiation.
        /// Used for direct content
        /// </summary>
        /// <param name="value"></param>
        private void writeText(string value)
        {
            if (RtfParser.IsNewGroup())
            {
                _rtfDoc.Add(new RtfDirectContent("{"));
                RtfParser.SetNewGroup(false);
            }
            if (value.Length > 0)
            {
                _rtfDoc.Add(new RtfDirectContent(value));
            }
        }
    }
}