using System.Text;
using iTextSharp.text.rtf.direct;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.parser.ctrlwords;
using iTextSharp.text.rtf.parser.properties;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationDocument  handles data destined for the document destination
///     @author Howard Shank (hgshank@yahoo.com)
/// </summary>
public sealed class RtfDestinationDocument : RtfDestination, IRtfPropertyListener
{
    private static readonly List<string> _importIgnoredCtrlwords = new(new[]
                                                                       {
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
                                                                           "adeflangfe",
                                                                       }
                                                                      );

    private static List<string> _convertIgnoredCtrlwords = new(new[] { "rtf" });

    private StringBuilder _buffer;

    /// <summary>
    ///     Indicates the parser action. Import or Conversion.
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_UNIDENTIFIED
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_CONVERT
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FRAGMENT
    ///     @see com.lowagie.text.rtf.direct.RtfParser#TYPE_IMPORT_FULL
    /// </summary>
    private int _conversionType;

    /// <summary>
    ///     The iText Document object.
    ///     @see com.lowagie.text.Document
    /// </summary>
    private Document _doc;

    private Paragraph _iTextParagraph;

    /// <summary>
    ///     The RtfDocument object.
    ///     @see com.lowagie.text.rtf.document.RtfDocument
    /// </summary>
    private RtfDocument _rtfDoc;

    /// <summary>
    ///     Indicates the current table level being processed
    /// </summary>
    private int _tableLevel;

    public RtfDestinationDocument() : base(null)
    {
    }

    /// <summary>
    ///     Constructs a new  RtfDestinationDocument  using
    ///     the parameters to initialize the object.
    /// </summary>
    public RtfDestinationDocument(RtfParser parser) : base(parser)
    {
        if (parser == null)
        {
            throw new ArgumentNullException(nameof(parser));
        }

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
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.properties.RtfPropertyListener#afterChange(java.lang.String)
    /// </summary>
    public void AfterPropertyChange(string propertyName)
    {
        if (propertyName == null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        if (propertyName.StartsWith(RtfProperty.CHARACTER, StringComparison.Ordinal))
        {
        }
        else
        {
            if (propertyName.StartsWith(RtfProperty.PARAGRAPH, StringComparison.Ordinal))
            {
            }
            else
            {
                if (propertyName.StartsWith(RtfProperty.SECTION, StringComparison.Ordinal))
                {
                }
                else
                {
                    if (propertyName.StartsWith(RtfProperty.DOCUMENT, StringComparison.Ordinal))
                    {
                    }
                }
            }
        }
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.properties.RtfPropertyListener#beforeChange(java.lang.String)
    /// </summary>
    public void BeforePropertyChange(string propertyName)
    {
        if (propertyName == null)
        {
            throw new ArgumentNullException(nameof(propertyName));
        }

        // do we have any text to do anything with?
        // if not, then just return without action.
        if (_buffer.Length == 0)
        {
            return;
        }

        if (propertyName.StartsWith(RtfProperty.CHARACTER, StringComparison.Ordinal))
        {
            // this is a character change,
            // add a new chunck to the current paragraph using current character settings.
            var chunk = new Chunk();
            chunk.Append(_buffer.ToString());
            _buffer = new StringBuilder(255);
            var charProperties = RtfParser.GetState().Properties.GetProperties(RtfProperty.CHARACTER);
            var defFont = (string)charProperties[RtfProperty.CHARACTER_FONT];
            if (defFont == null)
            {
                defFont = "0";
            }

            var fontTable = (RtfDestinationFontTable)RtfParser.GetDestination("fonttbl");
            var currFont = fontTable.GetFont(defFont);
            var fs = Font.NORMAL;
            if (charProperties.ContainsKey(RtfProperty.CHARACTER_BOLD))
            {
                fs |= Font.BOLD;
            }

            if (charProperties.ContainsKey(RtfProperty.CHARACTER_ITALIC))
            {
                fs |= Font.ITALIC;
            }

            if (charProperties.ContainsKey(RtfProperty.CHARACTER_UNDERLINE))
            {
                fs |= Font.UNDERLINE;
            }

            var useFont = FontFactory.GetFont(currFont.Familyname, 12, fs, new BaseColor(0, 0, 0));


            chunk.Font = useFont;
            if (_iTextParagraph == null)
            {
                _iTextParagraph = new Paragraph();
            }

            _iTextParagraph.Add(chunk);
        }
        else
        {
            if (propertyName.StartsWith(RtfProperty.PARAGRAPH, StringComparison.Ordinal))
            {
                // this is a paragraph change. what do we do?
            }
            else
            {
                if (propertyName.StartsWith(RtfProperty.SECTION, StringComparison.Ordinal))
                {
                }
                else
                {
                    if (propertyName.StartsWith(RtfProperty.DOCUMENT, StringComparison.Ordinal))
                    {
                    }
                }
            }
        }
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
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
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
    /// </summary>
    public override bool HandleCharacter(int ch)
    {
        var result = true;
        OnCharacter(ch); // event handler

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
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
    /// </summary>
    public override bool HandleCloseGroup()
    {
        OnCloseGroup(); // event handler

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
                var chunk = new Chunk();
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
        if (ctrlWordData == null)
        {
            throw new ArgumentNullException(nameof(ctrlWordData));
        }

        var result = false;
        OnCtrlWord(ctrlWordData); // event handler

        if (RtfParser.IsImport())
        {
            // map font information
            if (ctrlWordData.CtrlWord.Equals("f", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapFontNr(ctrlWordData.Param);
            }

            // map color information
            //colors
            if (ctrlWordData.CtrlWord.Equals("cb", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals("cf", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            //cells
            if (ctrlWordData.CtrlWord.Equals("clcbpat", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals("clcbpatraw", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals("clcfpat", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals("clcfpatraw", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            //table rows
            if (ctrlWordData.CtrlWord.Equals("trcfpat", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals("trcbpat", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            //paragraph border
            if (ctrlWordData.CtrlWord.Equals("brdrcf", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            // map lists
            if (ctrlWordData.CtrlWord.Equals("ls", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapListNr(ctrlWordData.Param);
            }
        }


        if (RtfParser.IsConvert())
        {
            if (ctrlWordData.CtrlWord.Equals("par", StringComparison.Ordinal))
            {
                addParagraphToDocument();
            }

            // Set Font
            if (ctrlWordData.CtrlWord.Equals("f", StringComparison.Ordinal))
            {
            }

            // color information
            //colors
            if (ctrlWordData.CtrlWord.Equals("cb", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("cf", StringComparison.Ordinal))
            {
            }

            //cells
            if (ctrlWordData.CtrlWord.Equals("clcbpat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clcbpatraw", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clcfpat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clcfpatraw", StringComparison.Ordinal))
            {
            }

            //table rows
            if (ctrlWordData.CtrlWord.Equals("trcfpat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trcbpat", StringComparison.Ordinal))
            {
            }

            //paragraph border
            if (ctrlWordData.CtrlWord.Equals("brdrcf", StringComparison.Ordinal))
            {
            }

            /* TABLES */
            if (ctrlWordData.CtrlWord.Equals("trowd", StringComparison.Ordinal)) /*Beginning of row*/
            {
                _tableLevel++;
            }

            if (ctrlWordData.CtrlWord
                            .Equals("cell", StringComparison.Ordinal)) /*End of Cell Denotes the end of a table cell*/
            {
                //              String ctl = ctrlWordData.ctrlWord;
                //              System.out.Print("cell found");
            }

            if (ctrlWordData.CtrlWord.Equals("row", StringComparison.Ordinal)) /*End of row*/
            {
                _tableLevel++;
            }

            if (ctrlWordData.CtrlWord.Equals("lastrow", StringComparison.Ordinal)) /*Last row of the table*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("row", StringComparison.Ordinal)) /*End of row*/
            {
                _tableLevel++;
            }

            if (ctrlWordData.CtrlWord.Equals("irow", StringComparison.Ordinal)) /*param  is the row index of this row.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("irowband",
                                    StringComparison
                                        .Ordinal)) /*param is the row index of the row, adjusted to account for header rows. A header row has a value of -1.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tcelld", StringComparison.Ordinal)) /*Sets table cell defaults*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("nestcell",
                                             StringComparison.Ordinal)) /*Denotes the end of a nested cell.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("nestrow", StringComparison.Ordinal)) /*Denotes the end of a nested row*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("nesttableprops",
                                    StringComparison
                                        .Ordinal)) /*Defines the properties of a nested table. This is a destination control word*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("nonesttables",
                                    StringComparison
                                        .Ordinal)) /*Contains text for readers that do not understand nested tables. This destination should be ignored by readers that support nested tables.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trgaph",
                                             StringComparison
                                                 .Ordinal)) /*Half the space between the cells of a table row in twips.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("cellx",
                                    StringComparison
                                        .Ordinal)) /*param Defines the right boundary of a table cell, including its half of the space between cells.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("clmgf",
                                    StringComparison
                                        .Ordinal)) /*The first cell in a range of table cells to be merged.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("clmrg",
                                    StringComparison
                                        .Ordinal)) /*Contents of the table cell are merged with those of the preceding cell*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("clvmgf",
                                    StringComparison
                                        .Ordinal)) /*The first cell in a range of table cells to be vertically merged.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("clvmrg",
                                    StringComparison
                                        .Ordinal)) /*Contents of the table cell are vertically merged with those of the preceding cell*/
            {
            }

            /* TABLE: table row revision tracking */
            if (ctrlWordData.CtrlWord
                            .Equals("trauth",
                                    StringComparison
                                        .Ordinal)) /*With revision tracking enabled, this control word identifies the author of changes to a table row's properties. N refers to a value in the revision table*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("trdate",
                                    StringComparison
                                        .Ordinal)) /*With revision tracking enabled, this control word identifies the date of a revision*/
            {
            }

            /* TABLE: Autoformatting flags */
            if (ctrlWordData.CtrlWord.Equals("tbllkborder",
                                             StringComparison.Ordinal)) /*Flag sets table autoformat to format borders*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tbllkshading",
                                             StringComparison
                                                 .Ordinal)) /*Flag sets table autoformat to affect shading.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tbllkfont",
                                             StringComparison.Ordinal)) /*Flag sets table autoformat to affect font*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tbllkcolor",
                                             StringComparison.Ordinal)) /*Flag sets table autoformat to affect color*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tbllkbestfit",
                                             StringComparison.Ordinal)) /*Flag sets table autoformat to apply best fit*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("tbllkhdrrows",
                                    StringComparison
                                        .Ordinal)) /*Flag sets table autoformat to format the first (header) row*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tbllklastrow",
                                             StringComparison
                                                 .Ordinal)) /*Flag sets table autoformat to format the last row.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("tbllkhdrcols",
                                    StringComparison
                                        .Ordinal)) /*Flag sets table autoformat to format the first (header) column*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tbllklastcol",
                                             StringComparison
                                                 .Ordinal)) /*Flag sets table autoformat to format the last column*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("tbllknorowband",
                                    StringComparison
                                        .Ordinal)) /*Specifies row banding conditional formatting shall not be applied*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("tbllknocolband",
                                    StringComparison
                                        .Ordinal)) /*Specifies column banding conditional formatting shall not be applied.*/
            {
            }

            /* TABLE: Row Formatting */
            if (ctrlWordData.CtrlWord.Equals("taprtl", StringComparison.Ordinal)) /*Table direction is right to left*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trautofit", StringComparison.Ordinal)) /*param = AutoFit:
    0   No AutoFit (default).
    1   AutoFit is on for the row. Overridden by \clwWidthN and \trwWidthN in any table row.
    */
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("trhdr",
                                    StringComparison
                                        .Ordinal)) /*Table row header. This row should appear at the top of every page on which the current table appears*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("trkeep",
                                    StringComparison
                                        .Ordinal)) /*Keep table row together. This row cannot be split by a page break. This property is assumed to be off unless the control word is present*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trkeepfollow",
                                             StringComparison
                                                 .Ordinal)) /*Keep row in the same page as the following row.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("trleft",
                                    StringComparison
                                        .Ordinal)) /*Position in twips of the leftmost edge of the table with respect to the left edge of its column.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("trqc",
                                    StringComparison
                                        .Ordinal)) /*Centers a table row with respect to its containing column.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("trql",
                                    StringComparison
                                        .Ordinal)) /*Left-justifies a table row with respect to its containing column.*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("trqr",
                                    StringComparison
                                        .Ordinal)) /*Right-justifies a table row with respect to its containing column*/
            {
            }

            if (ctrlWordData.CtrlWord
                            .Equals("trrh",
                                    StringComparison
                                        .Ordinal)) /*Height of a table row in twips. When 0, the height is sufficient for all the text in the line; when positive, the height is guaranteed to be at least the specified height; when negative, the absolute value of the height is used, regardless of the height of the text in the line*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpaddb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpaddl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpaddr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpaddt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpaddfb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpaddfl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpaddfr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpaddft", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trspdl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trspdt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trspdb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trspdr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trspdfl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trspdft", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trspdfb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trspdfr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trwWidth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trftsWidth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trwWidthB", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trftsWidthB", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trftsWidthB", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trwWidthA", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trftsWidthA", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tblind", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tblindtype", StringComparison.Ordinal)) /* */
            {
            }

            /*TABLE: Row shading and Background Colors*/
            if (ctrlWordData.CtrlWord.Equals("trcbpat", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trcfpat", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trpat", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trshdng", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgbdiag", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgcross", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgdcross", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgdkbdiag", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgdkcross", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgdkdcross", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgdkfdiag", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgdkhor", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgdkvert", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgfdiag", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbghoriz", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbgvert", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Cell Formatting*/
            if (ctrlWordData.CtrlWord.Equals("clFitText", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clNoWrap", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clpadl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clpadt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clpadb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clpadr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clpadfl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clpadft", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clpadfb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clpadfr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clwWidth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clftsWidth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clhidemark", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Compared Table Cells */
            if (ctrlWordData.CtrlWord.Equals("clins", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("cldel", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clmrgd", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clmrgdr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clsplit", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clsplitr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clinsauth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clinsdttm", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("cldelauth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("cldeldttm", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clmrgdauth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clmrgddttm", StringComparison.Ordinal)) /* */
            {
            }

            /*TABLE: Position Wrapped Tables (The following properties must be the same for all rows in the table.)*/
            if (ctrlWordData.CtrlWord.Equals("tdfrmtxtLeft", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tdfrmtxtRight", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tdfrmtxtTop", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tdfrmtxtBottom", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tabsnoovrlp", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tphcol", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tphmrg", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tphpg", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposnegx", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposnegy", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposx", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposxc", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposxi", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposxl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposxo", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposxr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposy", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposyb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposyc", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposyil", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposyin", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposyout", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tposyt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tpvmrg", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tpvpara", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("tpvpg", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Bidirectional Controls */
            if (ctrlWordData.CtrlWord.Equals("rtlrow", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("ltrrow", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Row Borders */
            if (ctrlWordData.CtrlWord.Equals("trbrdrt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbrdrl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbrdrb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbrdrr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbrdrh", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("trbrdrv", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Cell Borders */
            if (ctrlWordData.CtrlWord.Equals("brdrnil", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clbrdrb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clbrdrt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clbrdrl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("clbrdrr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("cldglu", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals("cldgll", StringComparison.Ordinal)) /* */
            {
            }

            if (string.IsNullOrEmpty(ctrlWordData.CtrlWord)) /* */
            {
            }
        }

        if (ctrlWordData.CtrlWordType == RtfCtrlWordType.TOGGLE)
        {
            RtfParser.GetState().Properties.ToggleProperty(ctrlWordData); //ctrlWordData.specialHandler);
        }

        if (ctrlWordData.CtrlWordType == RtfCtrlWordType.FLAG ||
            ctrlWordData.CtrlWordType == RtfCtrlWordType.VALUE)
        {
            RtfParser.GetState().Properties
                     .SetProperty(ctrlWordData); //ctrlWordData.specialHandler, ctrlWordData.param);
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
            default: // error because is should be an import or convert
                result = false;
                break;
        }


        return result;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
    /// </summary>
    public override bool HandleOpenGroup()
    {
        OnOpenGroup(); // event handler

        if (RtfParser.IsImport())
        {
        }

        if (RtfParser.IsConvert())
        {
            if (_iTextParagraph == null)
            {
                _iTextParagraph = new Paragraph();
            }
        }

        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
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
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#setDefaults()
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
    ///     Write the accumulated buffer to the destination.
    ///     Used for direct content
    /// </summary>
    private void writeBuffer()
    {
        writeText(_buffer.ToString());
        SetToDefaults();
    }

    /// <summary>
    ///     Write the string value to the destiation.
    ///     Used for direct content
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