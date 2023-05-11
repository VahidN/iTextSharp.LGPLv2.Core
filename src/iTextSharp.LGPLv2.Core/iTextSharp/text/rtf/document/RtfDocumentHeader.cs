using iTextSharp.text.rtf.document.output;
using iTextSharp.text.rtf.headerfooter;
using iTextSharp.text.rtf.list;
using iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.document;

/// <summary>
///     The RtfDocumentHeader contains all classes required for the generation of
///     the document header area.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Thomas Bickel (tmb99@inode.at)
/// </summary>
public class RtfDocumentHeader : RtfElement
{
    /// <summary>
    ///     Constant for facing pages
    /// </summary>
    private static readonly byte[] _facingPages = DocWriter.GetIsoBytes("\\facingp");

    /// <summary>
    ///     Constant for the title page
    /// </summary>
    private static readonly byte[] _titlePage = DocWriter.GetIsoBytes("\\titlepg");

    /// <summary>
    ///     The code page to use
    /// </summary>
    private RtfCodePage _codePage;

    /// <summary>
    ///     Stores all the colors used in the document
    /// </summary>
    private RtfColorList _colorList;

    /// <summary>
    ///     Stores all the fonts used in the document
    /// </summary>
    private RtfFontList _fontList;

    /// <summary>
    ///     The current RtfHeaderFooterGroup for the footer
    /// </summary>
    private HeaderFooter _footer;

    /// <summary>
    ///     Generator string in document
    /// </summary>
    private RtfGenerator _generator;

    /// <summary>
    ///     The current RtfHeaderFooterGroup for the header
    /// </summary>
    private HeaderFooter _header;

    /// <summary>
    ///     The information group with author/subject/keywords/title/producer/creationdate data
    /// </summary>
    private RtfInfoGroup _infoGroup;

    /// <summary>
    ///     Manages List tables
    /// </summary>
    private RtfListTable _listTable;

    /// <summary>
    ///     The page settings
    /// </summary>
    private RtfPageSetting _pageSetting;

    /// <summary>
    ///     The protection settings
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private RtfProtectionSetting _protectionSetting;

    /// <summary>
    ///     Stores all paragraph styles used in the document.
    /// </summary>
    private RtfStylesheetList _stylesheetList;

    /// <summary>
    ///     Constructs a RtfDocumentHeader for a RtfDocument
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfDocumentHeader belongs to</param>
    protected internal RtfDocumentHeader(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     Adds an RtfInfoElement to the list of RtfInfoElements
    /// </summary>
    /// <param name="rtfInfoElement">The RtfInfoElement to add</param>
    public void AddInfoElement(RtfInfoElement rtfInfoElement)
    {
        _infoGroup.Add(rtfInfoElement);
    }

    /// <summary>
    ///     Removes a RtfList from the list table
    /// </summary>
    /// <param name="list">The RtfList to remove</param>
    public void FreeListNumber(RtfList list)
    {
        _listTable.FreeListNumber(list);
    }

    /// <summary>
    ///     Gets the number of the specified RtfColor
    /// </summary>
    /// <param name="color">The RtfColor for which to get the number</param>
    /// <returns>The number of the color</returns>
    public int GetColorNumber(RtfColor color) => _colorList.GetColorNumber(color);

    /// <summary>
    ///     Gets the number of the specified RtfFont
    /// </summary>
    /// <param name="font">The RtfFont for which to get the number</param>
    /// <returns>The number of the font</returns>
    public int GetFontNumber(RtfFont font) => _fontList.GetFontNumber(font);

    /// <summary>
    ///     Gets the number of the specified RtfList
    /// </summary>
    /// <param name="list">The RtfList for which to get the number</param>
    /// <returns>The number of the list</returns>
    public int GetListNumber(RtfList list) => _listTable.GetListNumber(list);

    /// <summary>
    ///     Get the  RtfListTable  object.
    ///     @since 2.1.3
    /// </summary>
    /// <returns>the ListTable object.</returns>
    public RtfListTable GetListTable() => _listTable;

    /// <summary>
    ///     Gets the RtfPageSetting object of this RtfDocument
    /// </summary>
    /// <returns>The RtfPageSetting object</returns>
    public RtfPageSetting GetPageSetting() => _pageSetting;

    /// <summary>
    ///     Gets the RtfParagraphStyle with the given style name.
    /// </summary>
    /// <param name="styleName">The style name of the RtfParagraphStyle to get.</param>
    /// <returns>The RtfParagraphStyle with the given style name or null.</returns>
    public RtfParagraphStyle GetRtfParagraphStyle(string styleName) => _stylesheetList.GetRtfParagraphStyle(styleName);

    /// <summary>
    ///     Registers the RtfParagraphStyle for further use in the document.
    /// </summary>
    /// <param name="rtfParagraphStyle">The RtfParagraphStyle to register.</param>
    public void RegisterParagraphStyle(RtfParagraphStyle rtfParagraphStyle)
    {
        _stylesheetList.RegisterParagraphStyle(rtfParagraphStyle);
    }

    /// <summary>
    ///     Sets the current footer to use
    /// </summary>
    /// <param name="footer">The HeaderFooter to use as footer</param>
    public void SetFooter(HeaderFooter footer)
    {
        _footer = footer;
    }

    /// <summary>
    ///     Sets the current header to use
    /// </summary>
    /// <param name="header">The HeaderFooter to use as header</param>
    public void SetHeader(HeaderFooter header)
    {
        _header = header;
    }

    /// <summary>
    ///     Write the contents of the document header area.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        try
        {
            // This is so that all colour, font and similar information is processed once, before
            // the header section is written.
            WriteSectionDefinition(new RtfNilOutputStream());

            _codePage.WriteDefinition(outp);
            _fontList.WriteDefinition(outp);
            _colorList.WriteDefinition(outp);
            _stylesheetList.WriteDefinition(outp);
            _listTable.WriteDefinition(outp);
            _generator.WriteContent(outp);
            _infoGroup.WriteContent(outp);
            _protectionSetting.WriteDefinition(outp);
            _pageSetting.WriteDefinition(outp);

            WriteSectionDefinition(outp);
        }
        catch (IOException)
        {
        }
    }

    /// <summary>
    ///     Writes the section definition data
    /// </summary>
    /// <param name="result"></param>
    public void WriteSectionDefinition(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        try
        {
            var header = convertHeaderFooter(_header, RtfHeaderFooter.TYPE_HEADER);
            var footer = convertHeaderFooter(_footer, RtfHeaderFooter.TYPE_FOOTER);
            if (header.HasTitlePage() || footer.HasTitlePage())
            {
                result.Write(_titlePage, 0, _titlePage.Length);
                header.SetHasTitlePage();
                footer.SetHasTitlePage();
            }

            if (header.HasFacingPages() || footer.HasFacingPages())
            {
                result.Write(_facingPages, 0, _facingPages.Length);
                header.SetHasFacingPages();
                footer.SetHasFacingPages();
            }

            footer.WriteContent(result);
            header.WriteContent(result);
            _pageSetting.WriteSectionDefinition(result);
        }
        catch (IOException)
        {
        }
    }

    /// <summary>
    ///     Initialises the RtfDocumentHeader.
    /// </summary>
    protected internal void Init()
    {
        _codePage = new RtfCodePage(Document);
        _colorList = new RtfColorList(Document);
        _fontList = new RtfFontList(Document);
        _listTable = new RtfListTable(Document);
        _stylesheetList = new RtfStylesheetList(Document);
        _infoGroup = new RtfInfoGroup(Document);
        _protectionSetting = new RtfProtectionSetting(Document);
        _pageSetting = new RtfPageSetting(Document);
        _header = new RtfHeaderFooterGroup(Document, RtfHeaderFooter.TYPE_HEADER);
        _footer = new RtfHeaderFooterGroup(Document, RtfHeaderFooter.TYPE_FOOTER);
        _generator = new RtfGenerator(Document);
    }

    /// <summary>
    ///     Converts a HeaderFooter into a RtfHeaderFooterGroup. Depending on which class
    ///     the HeaderFooter is, the correct RtfHeaderFooterGroup is created.
    ///     @see com.lowagie.text.rtf.headerfooter.RtfHeaderFooter
    ///     @see com.lowagie.text.rtf.headerfooter.RtfHeaderFooterGroup
    /// </summary>
    /// <param name="hf">The HeaderFooter to convert.</param>
    /// <param name="type">Whether the conversion is being done on a footer or header</param>
    /// <returns>The converted RtfHeaderFooterGroup.</returns>
    private RtfHeaderFooterGroup convertHeaderFooter(HeaderFooter hf, int type)
    {
        if (hf != null)
        {
            if (hf is RtfHeaderFooterGroup)
            {
                return new RtfHeaderFooterGroup(Document, (RtfHeaderFooterGroup)hf, type);
            }

            if (hf is RtfHeaderFooter)
            {
                return new RtfHeaderFooterGroup(Document, (RtfHeaderFooter)hf, type);
            }

            return new RtfHeaderFooterGroup(Document, hf, type);
        }

        return new RtfHeaderFooterGroup(Document, type);
    }
}