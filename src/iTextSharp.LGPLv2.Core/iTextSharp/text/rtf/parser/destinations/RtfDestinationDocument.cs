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
        "rtf", "ansicpg", "deff", "ansi", "mac", "pca", "pc", "stshfdbch", "stshfloch", "stshfhich", "stshfbi",
        "deflang", "deflangfe", "adeflang", "adeflangfe"
    });

    private static List<string> _convertIgnoredCtrlwords = new(new[]
    {
        "rtf"
    });

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

    public RtfDestinationDocument() : base(parser: null)
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
            _buffer = new StringBuilder(capacity: 255);
            var charProperties = RtfParser.GetState().Properties.GetProperties(RtfProperty.CHARACTER);
            var defFont = (string)charProperties[RtfProperty.CHARACTER_FONT];

            if (defFont == null)
            {
                defFont = "0";
            }

            var fontTable = (RtfDestinationFontTable)RtfParser.GetDestination(destination: "fonttbl");
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

            var useFont = FontFactory.GetFont(currFont.Familyname, size: 12, fs,
                new BaseColor(red: 0, green: 0, blue: 0));

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

            writeText(value: "}");
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
            if (ctrlWordData.CtrlWord.Equals(value: "f", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapFontNr(ctrlWordData.Param);
            }

            // map color information
            //colors
            if (ctrlWordData.CtrlWord.Equals(value: "cb", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cf", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            //cells
            if (ctrlWordData.CtrlWord.Equals(value: "clcbpat", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clcbpatraw", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clcfpat", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clcfpatraw", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            //table rows
            if (ctrlWordData.CtrlWord.Equals(value: "trcfpat", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trcbpat", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            //paragraph border
            if (ctrlWordData.CtrlWord.Equals(value: "brdrcf", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapColorNr(ctrlWordData.Param);
            }

            // map lists
            if (ctrlWordData.CtrlWord.Equals(value: "ls", StringComparison.Ordinal))
            {
                ctrlWordData.Param = RtfParser.GetImportManager().MapListNr(ctrlWordData.Param);
            }
        }

        if (RtfParser.IsConvert())
        {
            if (ctrlWordData.CtrlWord.Equals(value: "par", StringComparison.Ordinal))
            {
                addParagraphToDocument();
            }

            // Set Font
            if (ctrlWordData.CtrlWord.Equals(value: "f", StringComparison.Ordinal))
            {
            }

            // color information
            //colors
            if (ctrlWordData.CtrlWord.Equals(value: "cb", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cf", StringComparison.Ordinal))
            {
            }

            //cells
            if (ctrlWordData.CtrlWord.Equals(value: "clcbpat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clcbpatraw", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clcfpat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clcfpatraw", StringComparison.Ordinal))
            {
            }

            //table rows
            if (ctrlWordData.CtrlWord.Equals(value: "trcfpat", StringComparison.Ordinal))
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trcbpat", StringComparison.Ordinal))
            {
            }

            //paragraph border
            if (ctrlWordData.CtrlWord.Equals(value: "brdrcf", StringComparison.Ordinal))
            {
            }

            /* TABLES */
            if (ctrlWordData.CtrlWord.Equals(value: "trowd", StringComparison.Ordinal)) /*Beginning of row*/
            {
                _tableLevel++;
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cell",
                    StringComparison.Ordinal)) /*End of Cell Denotes the end of a table cell*/
            {
                //              String ctl = ctrlWordData.ctrlWord;
                //              System.out.Print("cell found");
            }

            if (ctrlWordData.CtrlWord.Equals(value: "row", StringComparison.Ordinal)) /*End of row*/
            {
                _tableLevel++;
            }

            if (ctrlWordData.CtrlWord.Equals(value: "lastrow", StringComparison.Ordinal)) /*Last row of the table*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "row", StringComparison.Ordinal)) /*End of row*/
            {
                _tableLevel++;
            }

            if (ctrlWordData.CtrlWord.Equals(value: "irow",
                    StringComparison.Ordinal)) /*param  is the row index of this row.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "irowband",
                    StringComparison
                        .Ordinal)) /*param is the row index of the row, adjusted to account for header rows. A header row has a value of -1.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tcelld", StringComparison.Ordinal)) /*Sets table cell defaults*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "nestcell",
                    StringComparison.Ordinal)) /*Denotes the end of a nested cell.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "nestrow",
                    StringComparison.Ordinal)) /*Denotes the end of a nested row*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "nesttableprops",
                    StringComparison
                        .Ordinal)) /*Defines the properties of a nested table. This is a destination control word*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "nonesttables",
                    StringComparison
                        .Ordinal)) /*Contains text for readers that do not understand nested tables. This destination should be ignored by readers that support nested tables.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trgaph",
                    StringComparison.Ordinal)) /*Half the space between the cells of a table row in twips.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cellx",
                    StringComparison
                        .Ordinal)) /*param Defines the right boundary of a table cell, including its half of the space between cells.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clmgf",
                    StringComparison.Ordinal)) /*The first cell in a range of table cells to be merged.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clmrg",
                    StringComparison
                        .Ordinal)) /*Contents of the table cell are merged with those of the preceding cell*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clvmgf",
                    StringComparison.Ordinal)) /*The first cell in a range of table cells to be vertically merged.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clvmrg",
                    StringComparison
                        .Ordinal)) /*Contents of the table cell are vertically merged with those of the preceding cell*/
            {
            }

            /* TABLE: table row revision tracking */
            if (ctrlWordData.CtrlWord.Equals(value: "trauth",
                    StringComparison
                        .Ordinal)) /*With revision tracking enabled, this control word identifies the author of changes to a table row's properties. N refers to a value in the revision table*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trdate",
                    StringComparison
                        .Ordinal)) /*With revision tracking enabled, this control word identifies the date of a revision*/
            {
            }

            /* TABLE: Autoformatting flags */
            if (ctrlWordData.CtrlWord.Equals(value: "tbllkborder",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to format borders*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllkshading",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to affect shading.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllkfont",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to affect font*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllkcolor",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to affect color*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllkbestfit",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to apply best fit*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllkhdrrows",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to format the first (header) row*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllklastrow",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to format the last row.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllkhdrcols",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to format the first (header) column*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllklastcol",
                    StringComparison.Ordinal)) /*Flag sets table autoformat to format the last column*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllknorowband",
                    StringComparison.Ordinal)) /*Specifies row banding conditional formatting shall not be applied*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tbllknocolband",
                    StringComparison.Ordinal)) /*Specifies column banding conditional formatting shall not be applied.*/
            {
            }

            /* TABLE: Row Formatting */
            if (ctrlWordData.CtrlWord.Equals(value: "taprtl",
                    StringComparison.Ordinal)) /*Table direction is right to left*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trautofit", StringComparison.Ordinal)) /*param = AutoFit:
    0   No AutoFit (default).
    1   AutoFit is on for the row. Overridden by \clwWidthN and \trwWidthN in any table row.
    */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trhdr",
                    StringComparison
                        .Ordinal)) /*Table row header. This row should appear at the top of every page on which the current table appears*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trkeep",
                    StringComparison
                        .Ordinal)) /*Keep table row together. This row cannot be split by a page break. This property is assumed to be off unless the control word is present*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trkeepfollow",
                    StringComparison.Ordinal)) /*Keep row in the same page as the following row.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trleft",
                    StringComparison
                        .Ordinal)) /*Position in twips of the leftmost edge of the table with respect to the left edge of its column.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trqc",
                    StringComparison.Ordinal)) /*Centers a table row with respect to its containing column.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trql",
                    StringComparison.Ordinal)) /*Left-justifies a table row with respect to its containing column.*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trqr",
                    StringComparison.Ordinal)) /*Right-justifies a table row with respect to its containing column*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trrh",
                    StringComparison
                        .Ordinal)) /*Height of a table row in twips. When 0, the height is sufficient for all the text in the line; when positive, the height is guaranteed to be at least the specified height; when negative, the absolute value of the height is used, regardless of the height of the text in the line*/
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpaddb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpaddl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpaddr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpaddt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpaddfb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpaddfl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpaddfr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpaddft", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trspdl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trspdt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trspdb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trspdr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trspdfl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trspdft", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trspdfb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trspdfr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trwWidth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trftsWidth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trwWidthB", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trftsWidthB", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trftsWidthB", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trwWidthA", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trftsWidthA", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tblind", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tblindtype", StringComparison.Ordinal)) /* */
            {
            }

            /*TABLE: Row shading and Background Colors*/
            if (ctrlWordData.CtrlWord.Equals(value: "trcbpat", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trcfpat", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trpat", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trshdng", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgbdiag", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgcross", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgdcross", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgdkbdiag", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgdkcross", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgdkdcross", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgdkfdiag", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgdkhor", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgdkvert", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgfdiag", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbghoriz", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbgvert", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Cell Formatting*/
            if (ctrlWordData.CtrlWord.Equals(value: "clFitText", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clNoWrap", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clpadl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clpadt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clpadb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clpadr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clpadfl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clpadft", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clpadfb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clpadfr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clwWidth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clftsWidth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clhidemark", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Compared Table Cells */
            if (ctrlWordData.CtrlWord.Equals(value: "clins", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cldel", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clmrgd", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clmrgdr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clsplit", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clsplitr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clinsauth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clinsdttm", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cldelauth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cldeldttm", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clmrgdauth", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clmrgddttm", StringComparison.Ordinal)) /* */
            {
            }

            /*TABLE: Position Wrapped Tables (The following properties must be the same for all rows in the table.)*/
            if (ctrlWordData.CtrlWord.Equals(value: "tdfrmtxtLeft", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tdfrmtxtRight", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tdfrmtxtTop", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tdfrmtxtBottom", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tabsnoovrlp", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tphcol", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tphmrg", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tphpg", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposnegx", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposnegy", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposx", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposxc", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposxi", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposxl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposxo", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposxr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposy", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposyb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposyc", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposyil", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposyin", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposyout", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tposyt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tpvmrg", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tpvpara", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "tpvpg", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Bidirectional Controls */
            if (ctrlWordData.CtrlWord.Equals(value: "rtlrow", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "ltrrow", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Row Borders */
            if (ctrlWordData.CtrlWord.Equals(value: "trbrdrt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbrdrl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbrdrb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbrdrr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbrdrh", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "trbrdrv", StringComparison.Ordinal)) /* */
            {
            }

            /* TABLE: Cell Borders */
            if (ctrlWordData.CtrlWord.Equals(value: "brdrnil", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clbrdrb", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clbrdrt", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clbrdrl", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "clbrdrr", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cldglu", StringComparison.Ordinal)) /* */
            {
            }

            if (ctrlWordData.CtrlWord.Equals(value: "cldgll", StringComparison.Ordinal)) /* */
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

        if (ctrlWordData.CtrlWordType == RtfCtrlWordType.FLAG || ctrlWordData.CtrlWordType == RtfCtrlWordType.VALUE)
        {
            RtfParser.GetState()
                .Properties.SetProperty(ctrlWordData); //ctrlWordData.specialHandler, ctrlWordData.param);
        }

        switch (_conversionType)
        {
            case RtfParser.TYPE_IMPORT_FULL:
                if (!_importIgnoredCtrlwords.Contains(ctrlWordData.CtrlWord, StringComparer.Ordinal))
                {
                    writeBuffer();
                    writeText(ctrlWordData.ToString());
                }

                result = true;

                break;
            case RtfParser.TYPE_IMPORT_FRAGMENT:
                if (!_importIgnoredCtrlwords.Contains(ctrlWordData.CtrlWord, StringComparer.Ordinal))
                {
                    writeBuffer();
                    writeText(ctrlWordData.ToString());
                }

                result = true;

                break;
            case RtfParser.TYPE_CONVERT:
                if (!_importIgnoredCtrlwords.Contains(ctrlWordData.CtrlWord, StringComparer.Ordinal))
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
    public override void SetToDefaults() => _buffer = new StringBuilder();

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
            _rtfDoc.Add(new RtfDirectContent(directContent: "{"));
            RtfParser.SetNewGroup(value: false);
        }

        if (value.Length > 0)
        {
            _rtfDoc.Add(new RtfDirectContent(value));
        }
    }
}