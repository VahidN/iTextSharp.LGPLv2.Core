using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf;

/// <summary>
///     The RtfWriter allows the creation of rtf documents via the iText system
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfWriter2 : DocWriter
{
    /// <summary>
    ///     The RtfDocument this RtfWriter is creating
    /// </summary>
    private RtfDocument _rtfDoc;

    /// <summary>
    ///     Constructs a new RtfWriter that listens to the specified Document and
    ///     writes its output to the Stream.
    /// </summary>
    /// <param name="doc">The Document that this RtfWriter listens to</param>
    /// <param name="os">The Stream to write to</param>
    protected RtfWriter2(Document doc, Stream os) : base(doc, os)
    {
        if (doc == null)
        {
            throw new ArgumentNullException(nameof(doc));
        }

        doc.AddDocListener(this);
        _rtfDoc = new RtfDocument();
    }

    /// <summary>
    ///     Sets the footer to use
    /// </summary>
    public override HeaderFooter Footer
    {
        set => _rtfDoc.GetDocumentHeader().SetFooter(value);
    }

    /// <summary>
    ///     Sets the header to use
    /// </summary>
    public override HeaderFooter Header
    {
        set => _rtfDoc.GetDocumentHeader().SetHeader(value);
    }

    /// <summary>
    ///     This method is not supported in the RtfWriter
    /// </summary>
    public override int PageCount
    {
        set { }
    }

    /// <summary>
    ///     Static method to generate RtfWriters
    /// </summary>
    /// <param name="doc">The Document that this RtfWriter listens to</param>
    /// <param name="os">The Stream to write to</param>
    /// <returns>The new RtfWriter</returns>
    public static RtfWriter2 GetInstance(Document doc, Stream os) => new(doc, os);

    /// <summary>
    ///     Adds an Element to the Document
    ///     @throws DocumentException
    /// </summary>
    /// <param name="element">The element to be added</param>
    /// <returns> false </returns>
    public override bool Add(IElement element)
    {
        if (pause)
        {
            return false;
        }

        var rtfElements = _rtfDoc.GetMapper().MapElement(element);
        if (rtfElements.Length != 0)
        {
            for (var i = 0; i < rtfElements.Length; i++)
            {
                if (rtfElements[i] != null)
                {
                    _rtfDoc.Add(rtfElements[i]);
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Closes the RtfDocument. This causes the document to be written
    ///     to the specified Stream
    /// </summary>
    public override void Close()
    {
        if (open)
        {
            _rtfDoc.WriteDocument(Os);
            base.Close();
            _rtfDoc = new RtfDocument();
        }
    }

    /// <summary>
    ///     Gets the RtfDocumentSettings that specify how the rtf document is generated.
    /// </summary>
    /// <returns>The current RtfDocumentSettings.</returns>
    public RtfDocumentSettings GetDocumentSettings() => _rtfDoc.GetDocumentSettings();

    /// <summary>
    ///     Adds the complete RTF document to the current RTF document being generated.
    ///     It will parse the font and color tables and correct the font and color references
    ///     so that the imported RTF document retains its formattings.
    ///     @throws IOException On errors reading the RTF document.
    ///     @throws DocumentException On errors adding to this RTF document.
    /// </summary>
    /// <param name="documentSource">The Stream to read the RTF document from.</param>
    public void ImportRtfDocument(Stream documentSource)
    {
        ImportRtfDocument(documentSource, null);
    }

    /// <summary>
    ///     Adds the complete RTF document to the current RTF document being generated.
    ///     It will parse the font and color tables and correct the font and color references
    ///     so that the imported RTF document retains its formattings.
    ///     Uses new RtfParser object.
    ///     @throws IOException
    ///     @throws DocumentException
    ///     @see com.lowagie.text.rtf.parser.RtfParser
    ///     @see com.lowagie.text.rtf.parser.RtfParser#importRtfDocument(Reader, RtfDocument)
    ///     @since 2.0.8
    ///     @author Howard Shank
    /// </summary>
    /// <param name="documentSource">The Stream to read the RTF document from.</param>
    /// <param name="events">The array of event listeners. May be null</param>
    public void ImportRtfDocument(Stream documentSource, IEventListener[] events)
    {
        if (!open)
        {
            throw new DocumentException("The document must be open to import RTF documents.");
        }

        var rtfImport = new RtfParser(Document);
        if (events != null)
        {
            for (var idx = 0; idx < events.Length; idx++)
            {
                rtfImport.AddListener(events[idx]);
            }
        }

        rtfImport.ImportRtfDocument(documentSource, _rtfDoc);
    }

    /// <summary>
    ///     Adds the complete RTF document to the current RTF element being generated.
    ///     It will parse the font and color tables and correct the font and color references
    ///     so that the imported RTF document retains its formattings.
    ///     @throws IOException On errors reading the RTF document.
    ///     @throws DocumentException On errors adding to this RTF document.
    ///     @since 2.1.4
    /// </summary>
    /// <param name="elem">The Element the RTF document is to be imported into.</param>
    /// <param name="documentSource">The Reader to read the RTF document from.</param>
    public void ImportRtfDocumentIntoElement(IElement elem, FileStream documentSource)
    {
        ImportRtfDocumentIntoElement(elem, documentSource, null);
    }

    /// <summary>
    ///     Adds the complete RTF document to the current RTF element being generated.
    ///     It will parse the font and color tables and correct the font and color references
    ///     so that the imported RTF document retains its formattings.
    ///     @throws IOException On errors reading the RTF document.
    ///     @throws DocumentException On errors adding to this RTF document.
    ///     @since 2.1.4
    /// </summary>
    /// <param name="elem">The Element the RTF document is to be imported into.</param>
    /// <param name="documentSource">The Reader to read the RTF document from.</param>
    /// <param name="events">The event array for listeners.</param>
    public void ImportRtfDocumentIntoElement(IElement elem, FileStream documentSource, IEventListener[] events)
    {
        var rtfImport = new RtfParser(Document);
        if (events != null)
        {
            for (var idx = 0; idx < events.Length; idx++)
            {
                rtfImport.AddListener(events[idx]);
            }
        }

        rtfImport.ImportRtfDocumentIntoElement(elem, documentSource, _rtfDoc);
    }

    /// <summary>
    ///     Adds a fragment of an RTF document to the current RTF document being generated.
    ///     Since this fragment doesn't contain font or color tables, all fonts and colors
    ///     are mapped to the default font and color. If the font and color mappings are
    ///     known, they can be specified via the mappings parameter.
    ///     @throws IOException On errors reading the RTF fragment.
    ///     @throws DocumentException On errors adding to this RTF fragment.
    /// </summary>
    /// <param name="documentSource">The Stream to read the RTF fragment from.</param>
    /// <param name="mappings">The RtfImportMappings that contain font and color mappings to apply to the fragment.</param>
    public void ImportRtfFragment(Stream documentSource, RtfImportMappings mappings)
    {
        ImportRtfFragment(documentSource, mappings, null);
    }

    /// <summary>
    ///     Adds a fragment of an RTF document to the current RTF document being generated.
    ///     Since this fragment doesn't contain font or color tables, all fonts and colors
    ///     are mapped to the default font and color. If the font and color mappings are
    ///     known, they can be specified via the mappings parameter.
    ///     Uses new RtfParser object.
    ///     @throws IOException On errors reading the RTF fragment.
    ///     @throws DocumentException On errors adding to this RTF fragment.
    ///     @see com.lowagie.text.rtf.parser.RtfImportMappings
    ///     @see com.lowagie.text.rtf.parser.RtfParser
    ///     @see com.lowagie.text.rtf.parser.RtfParser#importRtfFragment(Reader, RtfDocument,
    ///     com.lowagie.text.rtf.parser.RtfImportMappings)
    ///     @since 2.0.8
    ///     @author Howard Shank
    /// </summary>
    /// <param name="documentSource">The Stream to read the RTF fragment from.</param>
    /// <param name="mappings">The RtfImportMappings that contain font and color mappings to apply to the fragment.</param>
    /// <param name="events">The array of event listeners. May be null</param>
    public void ImportRtfFragment(Stream documentSource, RtfImportMappings mappings, IEventListener[] events)
    {
        if (!open)
        {
            throw new DocumentException("The document must be open to import RTF fragments.");
        }

        var rtfImport = new RtfParser(Document);
        if (events != null)
        {
            for (var idx = 0; idx < events.Length; idx++)
            {
                rtfImport.AddListener(events[idx]);
            }
        }

        rtfImport.ImportRtfFragment(documentSource, _rtfDoc, mappings);
    }

    /// <summary>
    ///     Adds a page break
    /// </summary>
    /// <returns> false </returns>
    public override bool NewPage()
    {
        _rtfDoc.Add(new RtfNewPage(_rtfDoc));
        return true;
    }

    /// <summary>
    ///     Opens the RtfDocument
    /// </summary>
    public override void Open()
    {
        base.Open();
        _rtfDoc.Open();
    }

    /// <summary>
    ///     Resets the footer
    /// </summary>
    public override void ResetFooter()
    {
        _rtfDoc.GetDocumentHeader().SetFooter(null);
    }

    /// <summary>
    ///     Resets the header
    /// </summary>
    public override void ResetHeader()
    {
        _rtfDoc.GetDocumentHeader().SetHeader(null);
    }

    /// <summary>
    ///     This method is not supported in the RtfWriter
    /// </summary>
    public override void ResetPageCount()
    {
    }

    /// <summary>
    ///     Whether to automagically generate table of contents entries when
    ///     adding Chapters or Sections.
    /// </summary>
    /// <param name="autogenerate">Whether to automatically generate TOC entries</param>
    public void SetAutogenerateTocEntries(bool autogenerate)
    {
        _rtfDoc.SetAutogenerateTocEntries(autogenerate);
    }

    /// <summary>
    ///     Sets the page margins
    /// </summary>
    /// <param name="marginLeft">The left margin</param>
    /// <param name="marginRight">The right margin</param>
    /// <param name="marginTop">The top margin</param>
    /// <param name="marginBottom">The bottom margin</param>
    /// <returns> false </returns>
    public override bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom)
    {
        _rtfDoc.GetDocumentHeader().GetPageSetting().SetMarginLeft((int)(marginLeft * RtfElement.TWIPS_FACTOR));
        _rtfDoc.GetDocumentHeader().GetPageSetting().SetMarginRight((int)(marginRight * RtfElement.TWIPS_FACTOR));
        _rtfDoc.GetDocumentHeader().GetPageSetting().SetMarginTop((int)(marginTop * RtfElement.TWIPS_FACTOR));
        _rtfDoc.GetDocumentHeader().GetPageSetting().SetMarginBottom((int)(marginBottom * RtfElement.TWIPS_FACTOR));
        return true;
    }

    /// <summary>
    ///     Sets the size of the page
    /// </summary>
    /// <param name="pageSize">A Rectangle representing the page</param>
    /// <returns> false </returns>
    public override bool SetPageSize(Rectangle pageSize)
    {
        _rtfDoc.GetDocumentHeader().GetPageSetting().SetPageSize(pageSize);
        return true;
    }
}