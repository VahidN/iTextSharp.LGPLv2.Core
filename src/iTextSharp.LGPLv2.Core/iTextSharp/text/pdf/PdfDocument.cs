using System.util;
using System.util.collections;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.pdf.draw;
using iTextSharp.text.pdf.intern;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfDocument  is the class that is used by  PdfWriter
///     to translate a  Document  into a PDF with different pages.
///     A  PdfDocument  always listens to a  Document
///     and adds the Pdf representation of every  Element  that is
///     added to the  Document .
///     @see      com.lowagie.text.Document
///     @see      com.lowagie.text.DocListener
///     @see      PdfWriter
/// </summary>
public class PdfDocument : Document
{
    /// <summary>
    ///     PdfInfo  is the PDF InfoDictionary.
    ///     A document's trailer may contain a reference to an Info dictionary that provides information
    ///     about the document. This optional dictionary may contain one or more keys, whose values
    ///     should be strings.
    ///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
    ///     section 6.10 (page 120-121)
    /// </summary>
    /// <summary>
    ///     The characters to be applied the hanging punctuation.
    /// </summary>
    internal const string hangingPunctuation = ".,;:'";

    /// <summary>
    ///     Stores a list of document level JavaScript actions.
    /// </summary>
    private int _jsCounter;

    protected internal PdfDictionary AdditionalActions;

    /// <summary>
    ///     This represents the current alignment of the PDF Elements.
    /// </summary>
    protected internal int Alignment = Element.ALIGN_LEFT;

    /// <summary>
    ///     The current active  PdfAction  when processing an  Anchor .
    /// </summary>
    protected internal PdfAction AnchorAction;

    internal PdfAnnotationsImp AnnotationsImp;

    /// <summary>
    ///     This is the size of the several boxes that will be used in
    ///     the next page.
    /// </summary>
    protected INullValueDictionary<string, PdfRectangle> BoxSize = new NullValueDictionary<string, PdfRectangle>();

    protected internal PdfCollection collection;

    /// <summary>
    ///     This is the current  PdfOutline  in the hierarchy of outlines.
    /// </summary>
    protected internal PdfOutline CurrentOutline;

    protected internal INullValueDictionary<string, PdfObject> DocumentFileAttachment =
        new NullValueDictionary<string, PdfObject>();

    protected internal INullValueDictionary<string, PdfObject> DocumentLevelJs =
        new NullValueDictionary<string, PdfObject>();

    /// <summary>
    ///     The duration of the page
    /// </summary>
    protected int duration = -1;

    /// <summary>
    ///     Signals that OnOpenDocument should be called.
    /// </summary>
    protected internal bool FirstPageEvent = true;

    /// <summary>
    ///     This is the PdfContentByte object, containing the borders and other Graphics.
    /// </summary>
    protected internal PdfContentByte Graphics;

    /// <summary>
    ///     This is the position where the image ends.
    /// </summary>
    protected internal float ImageEnd = -1;

    /// <summary>
    ///     This is the image that could not be shown on a previous page.
    /// </summary>
    protected internal Image ImageWait;

    protected internal Indentation indentation = new();

    /// <summary>
    ///     some meta information about the Document.
    /// </summary>
    protected internal PdfInfo info = new();

    /// <summary>
    ///     Signals that onParagraph is valid (to avoid that a Chapter/Section title is treated as a Paragraph).
    ///     @since 2.1.2
    /// </summary>
    protected bool IsSectionTitle;

    /// <summary>
    ///     Holds the type of the last element, that has been added to the document.
    /// </summary>
    protected internal int LastElementType = -1;

    /// <summary>
    ///     This represents the leading of the lines.
    /// </summary>
    protected internal float leading;

    /// <summary>
    ///     Signals that the current leading has to be subtracted from a YMark object.
    ///     @since 2.1.2
    /// </summary>
    protected int LeadingCount;

    /// <summary>
    ///     The line that is currently being written.
    /// </summary>
    protected internal PdfLine Line;

    /// <summary>
    ///     The lines that are written until now.
    /// </summary>
    protected internal IList<PdfLine> Lines = new List<PdfLine>();

    /// <summary>
    ///     Stores the destinations keyed by name. Value is
    ///     Object[]{PdfAction,PdfIndirectReference,PdfDestintion} .
    /// </summary>
    protected internal OrderedTree LocalDestinations = new();

    protected int MarkPoint;

    /// <summary>
    ///     margin in y direction starting from the bottom. Will be valid in the next page
    /// </summary>
    protected float NextMarginBottom;

    /// <summary>
    ///     margin in x direction starting from the left. Will be valid in the next page
    /// </summary>
    protected float NextMarginLeft;

    /// <summary>
    ///     margin in x direction starting from the right. Will be valid in the next page
    /// </summary>
    protected float NextMarginRight;

    /// <summary>
    ///     margin in y direction starting from the top. Will be valid in the next page
    /// </summary>
    protected float NextMarginTop;

    /// <summary>
    ///     This is the size of the next page.
    /// </summary>
    protected Rectangle NextPageSize;

    protected internal PdfAction OpenActionAction;

    protected internal string OpenActionName;

    protected PdfDictionary PageAa;

    /// <summary>
    ///     This checks if the page is empty.
    /// </summary>
    protected internal bool pageEmpty = true;

    protected internal PdfPageLabels pageLabels;

    /// <summary>
    ///     This are the page resources of the current Page.
    /// </summary>
    protected internal PageResources pageResources;

    /// <summary>
    ///     This is the root outline of the document.
    /// </summary>
    protected internal PdfOutline rootOutline;

    /// <summary>
    ///     Holds value of property strictImageSequence.
    /// </summary>
    protected internal bool strictImageSequence;

    /// <summary>
    ///     This is the PdfContentByte object, containing the text.
    /// </summary>
    protected internal PdfContentByte Text;

    protected internal int TextEmptySize;

    /// <summary>
    ///     [U1] page sizes
    /// </summary>
    /// <summary>
    ///     This is the size of the several boxes of the current Page.
    /// </summary>
    protected INullValueDictionary<string, PdfRectangle> ThisBoxSize = new NullValueDictionary<string, PdfRectangle>();

    protected internal PdfIndirectReference Thumb;

    /// <summary>
    ///     The page transition
    /// </summary>
    protected PdfTransition transition;

    /// <summary>
    ///     Contains the Viewer preferences of this PDF document.
    /// </summary>
    protected PdfViewerPreferencesImp viewerPreferences = new();

    /// <summary>
    ///     The  PdfWriter .
    /// </summary>
    protected internal PdfWriter Writer;

    /// <summary>
    ///     [L3] DocListener interface
    /// </summary>
    /// <summary>
    ///     [C9] Metadata for the page
    /// </summary>
    /// <summary>
    ///     XMP Metadata for the page.
    /// </summary>
    protected byte[] xmpMetadata;

    /// <summary>
    ///     Constructs a new PDF document.
    ///     @throws DocumentException on error
    /// </summary>
    internal PdfDocument()
    {
        AddProducer();
        AddCreationDate();
    }

    /// <summary>
    ///     This is the current height of the document.
    /// </summary>
    public float CurrentHeight { get; private set; }

    /// <summary>
    ///     [C8] AcroForm
    /// </summary>
    /// <summary>
    ///     Gets the AcroForm object.
    /// </summary>
    /// <returns>the PdfAcroform object of the PdfDocument</returns>
    public PdfAcroForm AcroForm => AnnotationsImp.AcroForm;

    /// <summary>
    ///     Sets the collection dictionary.
    /// </summary>
    public PdfCollection Collection
    {
        set => collection = value;
    }

    /// <summary>
    ///     Changes the footer of this document.
    /// </summary>
    public override HeaderFooter Footer
    {
        set
        {
            if (Writer != null && Writer.IsPaused())
            {
                return;
            }

            base.Footer = value;
        }
    }

    /// <summary>
    ///     Changes the header of this document.
    /// </summary>
    public override HeaderFooter Header
    {
        set
        {
            if (Writer != null && Writer.IsPaused())
            {
                return;
            }

            base.Header = value;
        }
    }

    /// <summary>
    ///     [L0] ElementListener interface
    /// </summary>
    /// <summary>
    ///     Getter for the current leading.
    ///     @since   2.1.2
    /// </summary>
    /// <returns>the current leading</returns>
    public float Leading
    {
        get => leading;
        set => leading = value;
    }

    /// <summary>
    ///     Sets the page number.
    /// </summary>
    public override int PageCount
    {
        set
        {
            if (Writer != null && Writer.IsPaused())
            {
                return;
            }

            base.PageCount = value;
        }
    }

    /// <summary>
    ///     Gets the root outline. All the outlines must be created with a parent.
    ///     The first level is created with this outline.
    /// </summary>
    /// <returns>the root outline</returns>
    public PdfOutline RootOutline => rootOutline;

    /// <summary>
    ///     Use this method to set the XMP Metadata.
    /// </summary>
    public byte[] XmpMetadata
    {
        set => xmpMetadata = value;
    }

    internal Rectangle CropBoxSize
    {
        set => SetBoxSize("crop", value);
    }

    // negative values will indicate no duration
    /// <summary>
    ///     Sets the display duration for the page (for presentations)
    /// </summary>
    internal int Duration
    {
        set
        {
            if (value > 0)
            {
                duration = value;
            }
            else
            {
                duration = -1;
            }
        }
    }

    /// <summary>
    ///     Info Dictionary and Catalog
    /// </summary>
    /// <summary>
    ///     Gets the  PdfInfo -object.
    /// </summary>
    /// <returns> PdfInfo</returns>
    internal PdfInfo Info => info;

    /// <summary>
    ///     [U2] empty pages
    /// </summary>
    internal bool PageEmpty
    {
        set => pageEmpty = value;
    }

    /// <summary>
    ///     [C4] Page labels
    /// </summary>
    internal PdfPageLabels PageLabels
    {
        set => pageLabels = value;
    }

    /// <summary>
    ///     [M0] Page resources contain references to fonts, extgstate, images,...
    /// </summary>
    internal PageResources PageResources => pageResources;

    internal int SigFlags
    {
        set => AnnotationsImp.SigFlags = value;
    }

    /// <summary>
    ///     Setter for property strictImageSequence.
    /// </summary>
    internal bool StrictImageSequence
    {
        set => strictImageSequence = value;
        get => strictImageSequence;
    }

    /// <summary>
    ///     [U8] thumbnail images
    /// </summary>
    internal Image Thumbnail
    {
        set => Thumb = Writer.GetImageReference(Writer.AddDirectImageSimple(value));
    }

    /// <summary>
    ///     [U3] page actions
    /// </summary>
    /// <summary>
    ///     Sets the transition for the page
    /// </summary>
    internal PdfTransition Transition
    {
        set => transition = value;
    }

    /// <summary>
    ///     [C3] PdfViewerPreferences interface
    /// </summary>
    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#setViewerPreferences(int)
    /// </summary>
    internal int ViewerPreferences
    {
        set => viewerPreferences.ViewerPreferences = value;
    }

    protected internal float IndentBottom => GetBottom(indentation.indentBottom);

    protected internal float IndentLeft => GetLeft(indentation.indentLeft + indentation.ListIndentLeft +
                                                   indentation.ImageIndentLeft + indentation.SectionIndentLeft);

    protected internal float IndentRight
        => GetRight(indentation.indentRight + indentation.SectionIndentRight + indentation.ImageIndentRight);

    protected internal float IndentTop => GetTop(indentation.indentTop);

    /// <summary>
    ///     LISTENER METHODS START
    /// </summary>
    /// <summary>
    ///     Signals that an  Element  was added to the  Document .
    ///     @throws DocumentException when a document isn't open yet, or has been closed
    /// </summary>
    /// <param name="element">the element to add</param>
    /// <returns> true  if the element was added,  false  if not.</returns>
    public override bool Add(IElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (Writer != null && Writer.IsPaused())
        {
            return false;
        }

        switch (element.Type)
        {
            // Information (headers)
            case Element.HEADER:
                info.Addkey(((Meta)element).Name, ((Meta)element).Content);

                break;
            case Element.TITLE:
                info.AddTitle(((Meta)element).Content);

                break;
            case Element.SUBJECT:
                info.AddSubject(((Meta)element).Content);

                break;
            case Element.KEYWORDS:
                info.AddKeywords(((Meta)element).Content);

                break;
            case Element.AUTHOR:
                info.AddAuthor(((Meta)element).Content);

                break;
            case Element.CREATOR:
                info.AddCreator(((Meta)element).Content);

                break;
            case Element.PRODUCER:
                // you can not change the name of the producer
                info.AddProducer();

                break;
            case Element.CREATIONDATE:
                // you can not set the creation date, only reset it
                info.AddCreationDate();

                break;

            // content (text)
            case Element.CHUNK:
            {
                // if there isn't a current line available, we make one
                if (Line == null)
                {
                    CarriageReturn();
                }

                // we cast the element to a chunk
                var chunk = new PdfChunk((Chunk)element, AnchorAction);

                // we try to add the chunk to the line, until we succeed
                {
                    PdfChunk overflow;

                    while ((overflow = Line.Add(chunk)) != null)
                    {
                        CarriageReturn();
                        chunk = overflow;
                        chunk.TrimFirstSpace();
                    }
                }

                pageEmpty = false;

                if (chunk.IsAttribute(Chunk.NEWPAGE))
                {
                    NewPage();
                }

                break;
            }
            case Element.ANCHOR:
            {
                LeadingCount++;
                var anchor = (Anchor)element;
                var url = anchor.Reference;
                leading = anchor.Leading;

                if (url != null)
                {
                    AnchorAction = new PdfAction(url);
                }

                // we process the element
                element.Process(this);
                AnchorAction = null;
                LeadingCount--;

                break;
            }
            case Element.ANNOTATION:
            {
                if (Line == null)
                {
                    CarriageReturn();
                }

                var annot = (Annotation)element;
                var rect = new Rectangle(0, 0);

                if (Line != null)
                {
                    rect = new Rectangle(annot.GetLlx(IndentRight - Line.WidthLeft),
                        annot.GetLly(IndentTop - CurrentHeight), annot.GetUrx(IndentRight - Line.WidthLeft + 20),
                        annot.GetUry(IndentTop - CurrentHeight - 20));
                }

                var an = PdfAnnotationsImp.ConvertAnnotation(Writer, annot, rect);
                AnnotationsImp.AddPlainAnnotation(an);
                pageEmpty = false;

                break;
            }
            case Element.PHRASE:
            {
                LeadingCount++;

                // we cast the element to a phrase and set the leading of the document
                leading = ((Phrase)element).Leading;

                // we process the element
                element.Process(this);
                LeadingCount--;

                break;
            }
            case Element.PARAGRAPH:
            {
                LeadingCount++;

                // we cast the element to a paragraph
                var paragraph = (Paragraph)element;

                AddSpacing(paragraph.SpacingBefore, leading, paragraph.Font);

                // we adjust the parameters of the document
                Alignment = paragraph.Alignment;
                leading = paragraph.TotalLeading;

                CarriageReturn();

                // we don't want to make orphans/widows
                if (CurrentHeight + Line.Height + leading > IndentTop - IndentBottom)
                {
                    NewPage();
                }

                indentation.indentLeft += paragraph.IndentationLeft;
                indentation.indentRight += paragraph.IndentationRight;

                CarriageReturn();

                var pageEvent = Writer.PageEvent;

                if (pageEvent != null && !IsSectionTitle)
                {
                    pageEvent.OnParagraph(Writer, this, IndentTop - CurrentHeight);
                }

                // if a paragraph has to be kept together, we wrap it in a table object
                if (paragraph.KeepTogether)
                {
                    CarriageReturn();
                    var table = new PdfPTable(1);
                    table.WidthPercentage = 100f;
                    var cell = new PdfPCell();
                    cell.AddElement(paragraph);
                    cell.Border = Rectangle.NO_BORDER;
                    cell.Padding = 0;
                    table.AddCell(cell);
                    indentation.indentLeft -= paragraph.IndentationLeft;
                    indentation.indentRight -= paragraph.IndentationRight;
                    Add(table);
                    indentation.indentLeft += paragraph.IndentationLeft;
                    indentation.indentRight += paragraph.IndentationRight;
                }
                else
                {
                    Line.SetExtraIndent(paragraph.FirstLineIndent);
                    element.Process(this);
                    CarriageReturn();
                    AddSpacing(paragraph.SpacingAfter, paragraph.TotalLeading, paragraph.Font);
                }

                if (pageEvent != null && !IsSectionTitle)
                {
                    pageEvent.OnParagraphEnd(Writer, this, IndentTop - CurrentHeight);
                }

                Alignment = Element.ALIGN_LEFT;
                indentation.indentLeft -= paragraph.IndentationLeft;
                indentation.indentRight -= paragraph.IndentationRight;
                CarriageReturn();
                LeadingCount--;

                break;
            }
            case Element.SECTION:
            case Element.CHAPTER:
            {
                // Chapters and Sections only differ in their constructor
                // so we cast both to a Section
                var section = (Section)element;
                var pageEvent = Writer.PageEvent;

                var hasTitle = section.NotAddedYet && section.Title != null;

                // if the section is a chapter, we begin a new page
                if (section.TriggerNewPage)
                {
                    NewPage();
                }

                if (hasTitle)
                {
                    var fith = IndentTop - CurrentHeight;
                    var rotation = PageSize.Rotation;

                    if (rotation == 90 || rotation == 180)
                    {
                        fith = PageSize.Height - fith;
                    }

                    var destination = new PdfDestination(PdfDestination.FITH, fith);

                    while (CurrentOutline.Level >= section.Depth)
                    {
                        CurrentOutline = CurrentOutline.Parent;
                    }

                    var outline = new PdfOutline(CurrentOutline, destination, section.GetBookmarkTitle(),
                        section.BookmarkOpen);

                    CurrentOutline = outline;
                }

                // some values are set
                CarriageReturn();
                indentation.SectionIndentLeft += section.IndentationLeft;
                indentation.SectionIndentRight += section.IndentationRight;

                if (section.NotAddedYet && pageEvent != null)
                {
                    if (element.Type == Element.CHAPTER)
                    {
                        pageEvent.OnChapter(Writer, this, IndentTop - CurrentHeight, section.Title);
                    }
                    else
                    {
                        pageEvent.OnSection(Writer, this, IndentTop - CurrentHeight, section.Depth, section.Title);
                    }
                }

                // the title of the section (if any has to be printed)
                if (hasTitle)
                {
                    IsSectionTitle = true;
                    Add(section.Title);
                    IsSectionTitle = false;
                }

                indentation.SectionIndentLeft += section.Indentation;

                // we process the section
                element.Process(this);

                // some parameters are set back to normal again
                indentation.SectionIndentLeft -= section.IndentationLeft + section.Indentation;
                indentation.SectionIndentRight -= section.IndentationRight;

                if (section.ElementComplete && pageEvent != null)
                {
                    if (element.Type == Element.CHAPTER)
                    {
                        pageEvent.OnChapterEnd(Writer, this, IndentTop - CurrentHeight);
                    }
                    else
                    {
                        pageEvent.OnSectionEnd(Writer, this, IndentTop - CurrentHeight);
                    }
                }

                break;
            }
            case Element.LIST:
            {
                // we cast the element to a List
                var list = (List)element;

                if (list.Alignindent)
                {
                    list.NormalizeIndentation();
                }

                // we adjust the document
                indentation.ListIndentLeft += list.IndentationLeft;
                indentation.indentRight += list.IndentationRight;

                // we process the items in the list
                element.Process(this);

                // some parameters are set back to normal again
                indentation.ListIndentLeft -= list.IndentationLeft;
                indentation.indentRight -= list.IndentationRight;
                CarriageReturn();

                break;
            }
            case Element.LISTITEM:
            {
                LeadingCount++;

                // we cast the element to a ListItem
                var listItem = (ListItem)element;

                AddSpacing(listItem.SpacingBefore, leading, listItem.Font);

                // we adjust the document
                Alignment = listItem.Alignment;
                indentation.ListIndentLeft += listItem.IndentationLeft;
                indentation.indentRight += listItem.IndentationRight;
                leading = listItem.TotalLeading;
                CarriageReturn();

                // we prepare the current line to be able to show us the listsymbol
                Line.ListItem = listItem;

                // we process the item
                element.Process(this);

                AddSpacing(listItem.SpacingAfter, listItem.TotalLeading, listItem.Font);

                // if the last line is justified, it should be aligned to the left
                if (Line.HasToBeJustified())
                {
                    Line.ResetAlignment();
                }

                // some parameters are set back to normal again
                CarriageReturn();
                indentation.ListIndentLeft -= listItem.IndentationLeft;
                indentation.indentRight -= listItem.IndentationRight;
                LeadingCount--;

                break;
            }
            case Element.RECTANGLE:
            {
                var rectangle = (Rectangle)element;
                Graphics.Rectangle(rectangle);
                pageEmpty = false;

                break;
            }
            case Element.PTABLE:
            {
                var ptable = (PdfPTable)element;

                if (ptable.Size <= ptable.HeaderRows)
                {
                    break; //nothing to do
                }

                // before every table, we add a new line and flush all lines
                EnsureNewLine();
                FlushLines();

                AddPTable(ptable);
                pageEmpty = false;
                NewLine();

                break;
            }
            case Element.MULTI_COLUMN_TEXT:
            {
                EnsureNewLine();
                FlushLines();
                var multiText = (MultiColumnText)element;
                var height = multiText.Write(Writer.DirectContent, this, IndentTop - CurrentHeight);
                CurrentHeight += height;
                Text.MoveText(0, -1f * height);
                pageEmpty = false;

                break;
            }
            case Element.TABLE:
            {
                if (element is SimpleTable)
                {
                    var ptable = ((SimpleTable)element).CreatePdfPTable();

                    if (ptable.Size <= ptable.HeaderRows)
                    {
                        break; //nothing to do
                    }

                    // before every table, we add a new line and flush all lines
                    EnsureNewLine();
                    FlushLines();
                    AddPTable(ptable);
                    pageEmpty = false;

                    break;
                }

                if (element is Table)
                {
                    try
                    {
                        var ptable = ((Table)element).CreatePdfPTable();

                        if (ptable.Size <= ptable.HeaderRows)
                        {
                            break; //nothing to do
                        }

                        // before every table, we add a new line and flush all lines
                        EnsureNewLine();
                        FlushLines();
                        AddPTable(ptable);
                        pageEmpty = false;
                    }
                    catch (BadElementException)
                    {
                        // constructing the PdfTable
                        // Before the table, add a blank line using offset or default leading
                        var offset = ((Table)element).Offset;

                        if (float.IsNaN(offset))
                        {
                            offset = leading;
                        }

                        CarriageReturn();
                        Lines.Add(new PdfLine(IndentLeft, IndentRight, Alignment, offset));
                        CurrentHeight += offset;
                        addPdfTable((Table)element);
                    }
                }
                else
                {
                    return false;
                }

                break;
            }
            case Element.JPEG:
            case Element.JPEG2000:
            case Element.JBIG2:
            case Element.IMGRAW:
            case Element.IMGTEMPLATE:
            {
                //carriageReturn(); suggestion by Marc Campforts
                Add((Image)element);

                break;
            }
            case Element.YMARK:
            {
                var zh = (IDrawInterface)element;

                zh.Draw(Graphics, IndentLeft, IndentBottom, IndentRight, IndentTop,
                    IndentTop - CurrentHeight - (LeadingCount > 0 ? leading : 0));

                pageEmpty = false;

                break;
            }
            case Element.MARKED:
            {
                MarkedObject mo;

                if (element is MarkedSection)
                {
                    mo = ((MarkedSection)element).Title;

                    if (mo != null)
                    {
                        mo.Process(this);
                    }
                }

                mo = (MarkedObject)element;
                mo.Process(this);

                break;
            }
            default:
                return false;
        }

        LastElementType = element.Type;

        return true;
    }

    /// <summary>
    ///     Method added by Pelikan Stephan
    ///     @see com.lowagie.text.DocListener#clearTextWrap()
    /// </summary>
    public void ClearTextWrap()
    {
        var tmpHeight = ImageEnd - CurrentHeight;

        if (Line != null)
        {
            tmpHeight += Line.Height;
        }

        if (ImageEnd > -1 && tmpHeight > 0)
        {
            CarriageReturn();
            CurrentHeight += tmpHeight;
        }
    }

    /// <summary>
    ///     Closes the document.
    ///     Once all the content has been written in the body, you have to close
    ///     the body. After that nothing can be written to the body anymore.
    /// </summary>
    public override void Close()
    {
        if (IsDocumentClose)
        {
            return;
        }

        var wasImage = ImageWait != null;
        NewPage();

        if (ImageWait != null || wasImage)
        {
            NewPage();
        }

        if (AnnotationsImp.HasUnusedAnnotations())
        {
            throw new InvalidOperationException(
                "Not all annotations could be added to the document (the document doesn't have enough pages).");
        }

        var pageEvent = Writer.PageEvent;

        if (pageEvent != null)
        {
            pageEvent.OnCloseDocument(Writer, this);
        }

        base.Close();

        Writer.AddLocalDestinations(LocalDestinations);
        CalculateOutlineCount();
        WriteOutlines();

        Writer.Close();
    }

    /// <summary>
    ///     Gets the current vertical page position.
    ///     for elements that do not terminate the lines they've started because those lines will get
    ///     terminated.
    /// </summary>
    /// <param name="ensureNewLine">Tells whether a new line shall be enforced. This may cause side effects</param>
    /// <returns>The current vertical page position.</returns>
    public float GetVerticalPosition(bool ensureNewLine)
    {
        // ensuring that a new line has been started.
        if (ensureNewLine)
        {
            EnsureNewLine();
        }

        return Top - CurrentHeight - indentation.indentTop;
    }

    /// <summary>
    ///     [L2] DocListener interface
    /// </summary>
    /// <summary>
    ///     Makes a new page and sends it to the  PdfWriter .
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>a  bool </returns>
    public override bool NewPage()
    {
        LastElementType = -1;

        if (Writer == null || (Writer.DirectContent.Size == 0 && Writer.DirectContentUnder.Size == 0 &&
                               (pageEmpty || Writer.IsPaused())))
        {
            SetNewPageSizeAndMargins();

            return false;
        }

        if (!IsDocumentOpen || IsDocumentClose)
        {
            throw new InvalidOperationException("The document isn't open.");
        }

        var pageEvent = Writer.PageEvent;

        if (pageEvent != null)
        {
            pageEvent.OnEndPage(Writer, this);
        }

        //Added to inform any listeners that we are moving to a new page (added by David Freels)
        base.NewPage();

        // the following 2 lines were added by Pelikan Stephan
        indentation.ImageIndentLeft = 0;
        indentation.ImageIndentRight = 0;

        // we flush the arraylist with recently written lines
        FlushLines();

        // we prepare the elements of the page dictionary

        // [U1] page size and rotation
        var rotation = PageSize.Rotation;

        // [C10]
        if (Writer.IsPdfX())
        {
            var containsArtKey = ThisBoxSize.ContainsKey("art");
            var containsTrimKey = ThisBoxSize.ContainsKey("trim");

            if (containsArtKey && containsTrimKey)
            {
                throw new PdfXConformanceException("Only one of ArtBox or TrimBox can exist in the page.");
            }

            if (!containsArtKey && !containsTrimKey)
            {
                if (ThisBoxSize.TryGetValue("crop", out var crop))
                {
                    ThisBoxSize["trim"] = crop;
                }
                else
                {
                    ThisBoxSize["trim"] = new PdfRectangle(PageSize, PageSize.Rotation);
                }
            }
        }

        // [M1]
        pageResources.AddDefaultColorDiff(Writer.DefaultColorspace);

        if (Writer.RgbTransparencyBlending)
        {
            var dcs = new PdfDictionary();
            dcs.Put(PdfName.Cs, PdfName.Devicergb);
            pageResources.AddDefaultColorDiff(dcs);
        }

        var resources = pageResources.Resources;

        // we create the page dictionary

        var page = new PdfPage(new PdfRectangle(PageSize, rotation), ThisBoxSize, resources, rotation);
        page.Put(PdfName.Tabs, Writer.Tabs);
        page.Merge(Writer.PageDictionary);

        // we complete the page dictionary

        // [C9] if there is XMP data to add: add it
        if (xmpMetadata != null)
        {
            var xmp = new PdfStream(xmpMetadata);
            xmp.Put(PdfName.TYPE, PdfName.Metadata);
            xmp.Put(PdfName.Subtype, PdfName.Xml);
            var crypto = Writer.Encryption;

            if (crypto != null && !crypto.IsMetadataEncrypted())
            {
                var ar = new PdfArray();
                ar.Add(PdfName.Crypt);
                xmp.Put(PdfName.Filter, ar);
            }

            page.Put(PdfName.Metadata, Writer.AddToBody(xmp).IndirectReference);
        }

        // [U3] page actions: transition, duration, additional actions
        if (transition != null)
        {
            page.Put(PdfName.Trans, transition.TransitionDictionary);
            transition = null;
        }

        if (duration > 0)
        {
            page.Put(PdfName.Dur, new PdfNumber(duration));
            duration = 0;
        }

        if (PageAa != null)
        {
            page.Put(PdfName.Aa, Writer.AddToBody(PageAa).IndirectReference);
            PageAa = null;
        }

        // [U4] we add the thumbs
        if (Thumb != null)
        {
            page.Put(PdfName.Thumb, Thumb);
            Thumb = null;
        }

        // [U8] we check if the userunit is defined
        if (Writer.Userunit > 0f)
        {
            page.Put(PdfName.Userunit, new PdfNumber(Writer.Userunit));
        }

        // [C5] and [C8] we add the annotations
        if (AnnotationsImp.HasUnusedAnnotations())
        {
            var array = AnnotationsImp.RotateAnnotations(Writer, PageSize);

            if (array.Size != 0)
            {
                page.Put(PdfName.Annots, array);
            }
        }

        // [F12] we add tag info
        if (Writer.IsTagged())
        {
            page.Put(PdfName.Structparents, new PdfNumber(Writer.CurrentPageNumber - 1));
        }

        if (Text.Size > TextEmptySize)
        {
            Text.EndText();
        }
        else
        {
            Text = null;
        }

        Writer.Add(page, new PdfContents(Writer.DirectContentUnder, Graphics, Text, Writer.DirectContent, PageSize));

        // we initialize the new page
        InitPage();

        return true;
    }

    /// <summary>
    ///     Opens the document.
    ///     You have to open the document before you can begin to add content
    ///     to the body of the document.
    /// </summary>
    public override void Open()
    {
        if (!IsDocumentOpen)
        {
            base.Open();
            Writer.Open();
            rootOutline = new PdfOutline(Writer);
            CurrentOutline = rootOutline;
        }

        InitPage();
    }

    /// <summary>
    ///     Resets the footer of this document.
    /// </summary>
    public override void ResetFooter()
    {
        if (Writer != null && Writer.IsPaused())
        {
            return;
        }

        base.ResetFooter();
    }

    /// <summary>
    ///     Resets the header of this document.
    /// </summary>
    public override void ResetHeader()
    {
        if (Writer != null && Writer.IsPaused())
        {
            return;
        }

        base.ResetHeader();
    }

    /// <summary>
    ///     Sets the page number to 0.
    /// </summary>
    public override void ResetPageCount()
    {
        if (Writer != null && Writer.IsPaused())
        {
            return;
        }

        base.ResetPageCount();
    }

    /// <summary>
    ///     @see com.lowagie.text.DocListener#setMarginMirroring(bool)
    /// </summary>
    public override bool SetMarginMirroring(bool marginMirroring)
    {
        if (Writer != null && Writer.IsPaused())
        {
            return false;
        }

        return base.SetMarginMirroring(marginMirroring);
    }

    /// <summary>
    ///     [L6] DocListener interface
    /// </summary>
    /// <summary>
    ///     @see com.lowagie.text.DocListener#setMarginMirroring(boolean)
    ///     @since    2.1.6
    /// </summary>
    public override bool SetMarginMirroringTopBottom(bool marginMirroringTopBottom)
    {
        if (Writer != null && Writer.IsPaused())
        {
            return false;
        }

        return base.SetMarginMirroringTopBottom(marginMirroringTopBottom);
    }

    /// <summary>
    ///     Sets the margins.
    /// </summary>
    /// <param name="marginLeft">the margin on the left</param>
    /// <param name="marginRight">the margin on the right</param>
    /// <param name="marginTop">the margin on the top</param>
    /// <param name="marginBottom">the margin on the bottom</param>
    /// <returns>a  bool </returns>
    public override bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom)
    {
        if (Writer != null && Writer.IsPaused())
        {
            return false;
        }

        NextMarginLeft = marginLeft;
        NextMarginRight = marginRight;
        NextMarginTop = marginTop;
        NextMarginBottom = marginBottom;

        return true;
    }

    /// <summary>
    ///     Sets the pagesize.
    /// </summary>
    /// <param name="pageSize">the new pagesize</param>
    /// <returns> true  if the page size was set</returns>
    public override bool SetPageSize(Rectangle pageSize)
    {
        if (Writer != null && Writer.IsPaused())
        {
            return false;
        }

        NextPageSize = new Rectangle(pageSize);

        return true;
    }

    internal void AddAdditionalAction(PdfName actionType, PdfAction action)
    {
        if (AdditionalActions == null)
        {
            AdditionalActions = new PdfDictionary();
        }

        if (action == null)
        {
            AdditionalActions.Remove(actionType);
        }
        else
        {
            AdditionalActions.Put(actionType, action);
        }

        if (AdditionalActions.Size == 0)
        {
            AdditionalActions = null;
        }
    }

    internal void AddAnnotation(PdfAnnotation annot)
    {
        pageEmpty = false;
        AnnotationsImp.AddAnnotation(annot);
    }

    internal void AddCalculationOrder(PdfFormField formField)
        => AnnotationsImp.AddCalculationOrder(formField);

    internal void AddFileAttachment(string description, PdfFileSpecification fs)
    {
        if (description == null)
        {
            var desc = (PdfString)fs.Get(PdfName.Desc);

            if (desc == null)
            {
                description = "";
            }
            else
            {
                description = PdfEncodings.ConvertToString(desc.GetBytes(), null);
            }
        }

        fs.AddDescription(description, true);

        if (description.Length == 0)
        {
            description = "Unnamed";
        }

        var fn = PdfEncodings.ConvertToString(new PdfString(description, PdfObject.TEXT_UNICODE).GetBytes(), null);
        var k = 0;

        while (DocumentFileAttachment.ContainsKey(fn))
        {
            ++k;

            fn = PdfEncodings.ConvertToString(new PdfString(description + " " + k, PdfObject.TEXT_UNICODE).GetBytes(),
                null);
        }

        DocumentFileAttachment[fn] = fs.Reference;
    }

    internal void AddJavaScript(PdfAction js)
    {
        if (js.Get(PdfName.Js) == null)
        {
            throw new ArgumentException("Only JavaScript actions are allowed.");
        }

        DocumentLevelJs[_jsCounter.ToString(CultureInfo.InvariantCulture).PadLeft(16, '0')] =
            Writer.AddToBody(js).IndirectReference;

        _jsCounter++;
    }

    internal void AddJavaScript(string name, PdfAction js)
    {
        if (js.Get(PdfName.Js) == null)
        {
            throw new ArgumentException("Only JavaScript actions are allowed.");
        }

        DocumentLevelJs[name] = Writer.AddToBody(js).IndirectReference;
    }

    /// <summary>
    ///     [C1] outlines
    /// </summary>
    /// <summary>
    ///     Adds a named outline to the document .
    /// </summary>
    /// <param name="outline">the outline to be added</param>
    /// <param name="name">the name of this local destination</param>
    internal void AddOutline(PdfOutline outline, string name)
        => LocalDestination(name, outline.PdfDestination);

    /// <summary>
    ///     Adds a  PdfPTable  to the document.
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="ptable">the  PdfPTable  to be added to the document.</param>
    internal void AddPTable(PdfPTable ptable)
    {
        var ct = new ColumnText(Writer.DirectContent);

        // if the table prefers to be on a single page, and it wouldn't
        //fit on the current page, start a new page.
        if (ptable.KeepTogether && !FitsPage(ptable, 0f) && CurrentHeight > 0)
        {
            NewPage();
        }

        // add dummy paragraph if we aren't at the top of a page, so that
        // spacingBefore will be taken into account by ColumnText
        if (CurrentHeight > 0)
        {
            var p = new Paragraph();
            p.Leading = 0;
            ct.AddElement(p);
        }

        ct.AddElement(ptable);
        var he = ptable.HeadersInEvent;
        ptable.HeadersInEvent = true;
        var loop = 0;

        while (true)
        {
            ct.SetSimpleColumn(IndentLeft, IndentBottom, IndentRight, IndentTop - CurrentHeight);
            var status = ct.Go();

            if ((status & ColumnText.NO_MORE_TEXT) != 0)
            {
                Text.MoveText(0, ct.YLine - IndentTop + CurrentHeight);
                CurrentHeight = IndentTop - ct.YLine;

                break;
            }

            if ((IndentTop - CurrentHeight).ApproxEquals(ct.YLine))
            {
                ++loop;
            }
            else
            {
                loop = 0;
            }

            if (loop == 3)
            {
                Add(new Paragraph("ERROR: Infinite table loop"));

                break;
            }

            NewPage();
        }

        ptable.HeadersInEvent = he;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#addViewerPreference(com.lowagie.text.pdf.PdfName,
    ///     com.lowagie.text.pdf.PdfObject)
    /// </summary>
    internal void AddViewerPreference(PdfName key, PdfObject value)
        => viewerPreferences.AddViewerPreference(key, value);

    /// <summary>
    ///     CONSTRUCTING A PdfDocument/PdfWriter INSTANCE
    /// </summary>
    /// <summary>
    ///     Adds a  PdfWriter  to the  PdfDocument .
    ///     what is added to this document to an outputstream.
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="writer">the  PdfWriter  that writes everything</param>
    internal void AddWriter(PdfWriter writer)
    {
        if (Writer == null)
        {
            Writer = writer;
            AnnotationsImp = new PdfAnnotationsImp(writer);

            return;
        }

        throw new DocumentException("You can only add a writer to a PdfDocument once.");
    }

    internal void CalculateOutlineCount()
    {
        if (rootOutline.Kids.Count == 0)
        {
            return;
        }

        TraverseOutlineCount(rootOutline);
    }

    /// <summary>
    ///     [M4] Adding a PdfPTable
    /// </summary>
    internal bool FitsPage(PdfPTable table, float margin)
    {
        if (!table.LockedWidth)
        {
            var totalWidth = (IndentRight - IndentLeft) * table.WidthPercentage / 100;
            table.TotalWidth = totalWidth;
        }

        // ensuring that a new line has been started.
        EnsureNewLine();

        return table.TotalHeight + (CurrentHeight > 0 ? table.SpacingBefore : 0f) <=
               IndentTop - CurrentHeight - IndentBottom - margin;
    }

    /// <summary>
    ///     Returns the bottomvalue of a  Table  if it were added to this document.
    /// </summary>
    /// <param name="table">the table that may or may not be added to this document</param>
    /// <returns>a bottom value</returns>
    internal float GetBottom(Table table)
    {
        // constructing a PdfTable
        var tmp = new PdfTable(table, IndentLeft, IndentRight, IndentTop - CurrentHeight);

        return tmp.Bottom;
    }

    /// <summary>
    ///     Gives the size of a trim, art, crop or bleed box, or null if not defined.
    /// </summary>
    /// <param name="boxName">crop, trim, art or bleed</param>
    internal Rectangle GetBoxSize(string boxName)
    {
        var r = ThisBoxSize[boxName];

        if (r != null)
        {
            return r.Rectangle;
        }

        return null;
    }

    /// <summary>
    ///     Gets the  PdfCatalog -object.
    /// </summary>
    /// <param name="pages">an indirect reference to this document pages</param>
    /// <returns> PdfCatalog </returns>
    internal PdfCatalog GetCatalog(PdfIndirectReference pages)
    {
        var catalog = new PdfCatalog(pages, Writer);

        // [C1] outlines
        if (rootOutline.Kids.Count > 0)
        {
            catalog.Put(PdfName.Pagemode, PdfName.Useoutlines);
            catalog.Put(PdfName.Outlines, rootOutline.IndirectReference);
        }

        // [C2] version
        Writer.GetPdfVersion().AddToCatalog(catalog);

        // [C3] preferences
        viewerPreferences.AddToCatalog(catalog);

        // [C4] pagelabels
        if (pageLabels != null)
        {
            catalog.Put(PdfName.Pagelabels, pageLabels.GetDictionary(Writer));
        }

        // [C5] named objects
        catalog.AddNames(LocalDestinations, GetDocumentLevelJs(), DocumentFileAttachment, Writer);

        // [C6] actions
        if (OpenActionName != null)
        {
            var action = GetLocalGotoAction(OpenActionName);
            catalog.OpenAction = action;
        }
        else if (OpenActionAction != null)
        {
            catalog.OpenAction = OpenActionAction;
        }

        if (AdditionalActions != null)
        {
            catalog.AdditionalActions = AdditionalActions;
        }

        // [C7] portable collections
        if (collection != null)
        {
            catalog.Put(PdfName.Collection, collection);
        }

        // [C8] AcroForm
        if (AnnotationsImp.HasValidAcroForm())
        {
            catalog.Put(PdfName.Acroform, Writer.AddToBody(AnnotationsImp.AcroForm).IndirectReference);
        }

        return catalog;
    }

    internal INullValueDictionary<string, PdfObject> GetDocumentFileAttachment()
        => DocumentFileAttachment;

    internal INullValueDictionary<string, PdfObject> GetDocumentLevelJs()
        => DocumentLevelJs;

    internal PdfAction GetLocalGotoAction(string name)
    {
        PdfAction action;
        var obj = (object[])LocalDestinations[name];

        if (obj == null)
        {
            obj = new object[3];
        }

        if (obj[0] == null)
        {
            if (obj[1] == null)
            {
                obj[1] = Writer.PdfIndirectReference;
            }

            action = new PdfAction((PdfIndirectReference)obj[1]);
            obj[0] = action;
            LocalDestinations[name] = obj;
        }
        else
        {
            action = (PdfAction)obj[0];
        }

        return action;
    }

    /// <summary>
    ///     [F12] tagged PDF
    /// </summary>
    internal int GetMarkPoint()
        => MarkPoint;

    internal void IncMarkPoint()
        => ++MarkPoint;

    /// <summary>
    ///     The local destination to where a local goto with the same
    ///     name will jump to.
    ///     false  if a local destination with the same name
    ///     already existed
    /// </summary>
    /// <param name="name">the name of this local destination</param>
    /// <param name="destination">the  PdfDestination  with the jump coordinates</param>
    /// <returns> true  if the local destination was added,</returns>
    internal bool LocalDestination(string name, PdfDestination destination)
    {
        var obj = (object[])LocalDestinations[name];

        if (obj == null)
        {
            obj = new object[3];
        }

        if (obj[2] != null)
        {
            return false;
        }

        obj[2] = destination;
        LocalDestinations[name] = obj;
        destination.AddPage(Writer.CurrentPage);

        return true;
    }

    /// <summary>
    ///     Implements a link to other part of the document. The jump will
    ///     be made to a local destination with the same name, that must exist.
    /// </summary>
    /// <param name="name">the name for this link</param>
    /// <param name="llx">the lower left x corner of the activation area</param>
    /// <param name="lly">the lower left y corner of the activation area</param>
    /// <param name="urx">the upper right x corner of the activation area</param>
    /// <param name="ury">the upper right y corner of the activation area</param>
    internal void LocalGoto(string name, float llx, float lly, float urx, float ury)
    {
        var action = GetLocalGotoAction(name);
        AnnotationsImp.AddPlainAnnotation(new PdfAnnotation(Writer, llx, lly, urx, ury, action));
    }

    internal void OutlineTree(PdfOutline outline)
    {
        outline.IndirectReference = Writer.PdfIndirectReference;

        if (outline.Parent != null)
        {
            outline.Put(PdfName.Parent, outline.Parent.IndirectReference);
        }

        var kids = outline.Kids;
        var size = kids.Count;

        for (var k = 0; k < size; ++k)
        {
            OutlineTree(kids[k]);
        }

        for (var k = 0; k < size; ++k)
        {
            if (k > 0)
            {
                kids[k].Put(PdfName.Prev, kids[k - 1].IndirectReference);
            }

            if (k < size - 1)
            {
                kids[k].Put(PdfName.Next, kids[k + 1].IndirectReference);
            }
        }

        if (size > 0)
        {
            outline.Put(PdfName.First, kids[0].IndirectReference);
            outline.Put(PdfName.Last, kids[size - 1].IndirectReference);
        }

        for (var k = 0; k < size; ++k)
        {
            var kid = kids[k];
            Writer.AddToBody(kid, kid.IndirectReference);
        }
    }

    /// <summary>
    ///     [C5] named objects: local destinations, javascript, embedded files
    /// </summary>
    /// <summary>
    ///     Implements a link to another document.
    /// </summary>
    /// <param name="filename">the filename for the remote document</param>
    /// <param name="name">the name to jump to</param>
    /// <param name="llx">the lower left x corner of the activation area</param>
    /// <param name="lly">the lower left y corner of the activation area</param>
    /// <param name="urx">the upper right x corner of the activation area</param>
    /// <param name="ury">the upper right y corner of the activation area</param>
    internal void RemoteGoto(string filename, string name, float llx, float lly, float urx, float ury)
        => AnnotationsImp.AddPlainAnnotation(new PdfAnnotation(Writer, llx, lly, urx, ury,
            new PdfAction(filename, name)));

    /// <summary>
    ///     Implements a link to another document.
    /// </summary>
    /// <param name="filename">the filename for the remote document</param>
    /// <param name="page">the page to jump to</param>
    /// <param name="llx">the lower left x corner of the activation area</param>
    /// <param name="lly">the lower left y corner of the activation area</param>
    /// <param name="urx">the upper right x corner of the activation area</param>
    /// <param name="ury">the upper right y corner of the activation area</param>
    internal void RemoteGoto(string filename, int page, float llx, float lly, float urx, float ury)
        => AddAnnotation(new PdfAnnotation(Writer, llx, lly, urx, ury, new PdfAction(filename, page)));

    /// <summary>
    ///     Implements an action in an area.
    /// </summary>
    /// <param name="action">the  PdfAction </param>
    /// <param name="llx">the lower left x corner of the activation area</param>
    /// <param name="lly">the lower left y corner of the activation area</param>
    /// <param name="urx">the upper right x corner of the activation area</param>
    /// <param name="ury">the upper right y corner of the activation area</param>
    internal void SetAction(PdfAction action, float llx, float lly, float urx, float ury)
        => AddAnnotation(new PdfAnnotation(Writer, llx, lly, urx, ury, action));

    internal void SetBoxSize(string boxName, Rectangle size)
    {
        if (size == null)
        {
            BoxSize.Remove(boxName);
        }
        else
        {
            BoxSize[boxName] = new PdfRectangle(size);
        }
    }

    /// <summary>
    ///     [C6] document level actions
    /// </summary>
    internal void SetOpenAction(string name)
    {
        OpenActionName = name;
        OpenActionAction = null;
    }

    internal void SetOpenAction(PdfAction action)
    {
        OpenActionAction = action;
        OpenActionName = null;
    }

    internal void SetPageAction(PdfName actionType, PdfAction action)
    {
        if (PageAa == null)
        {
            PageAa = new PdfDictionary();
        }

        PageAa.Put(actionType, action);
    }

    internal static void TraverseOutlineCount(PdfOutline outline)
    {
        var kids = outline.Kids;
        var parent = outline.Parent;

        if (kids.Count == 0)
        {
            if (parent != null)
            {
                parent.Count = parent.Count + 1;
            }
        }
        else
        {
            for (var k = 0; k < kids.Count; ++k)
            {
                TraverseOutlineCount(kids[k]);
            }

            if (parent != null)
            {
                if (outline.Open)
                {
                    parent.Count = outline.Count + parent.Count + 1;
                }
                else
                {
                    parent.Count = parent.Count + 1;
                    outline.Count = -outline.Count;
                }
            }
        }
    }

    /// <summary>
    ///     Writes a text line to the document. It takes care of all the attributes.
    ///     Before entering the line position must have been established and the
    ///     text  argument must be in text object scope ( beginText() ).
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="line">the line to be written</param>
    /// <param name="text">the  PdfContentByte  where the text will be written to</param>
    /// <param name="graphics">the  PdfContentByte  where the graphics will be written to</param>
    /// <param name="currentValues">the current font and extra spacing values</param>
    /// <param name="ratio"></param>
    internal void WriteLineToContent(PdfLine line,
        PdfContentByte text,
        PdfContentByte graphics,
        object[] currentValues,
        float ratio)
    {
        var currentFont = (PdfFont)currentValues[0];
        var lastBaseFactor = (float)currentValues[1];

        //PdfChunk chunkz;
        int numberOfSpaces;
        int lineLen;
        bool isJustified;
        float hangingCorrection = 0;
        float hScale = 1;
        var lastHScale = float.NaN;
        float baseWordSpacing = 0;
        float baseCharacterSpacing = 0;
        float glueWidth = 0;

        numberOfSpaces = line.NumberOfSpaces;
        lineLen = line.GetLineLengthUtf32();

        // does the line need to be justified?
        isJustified = line.HasToBeJustified() && (numberOfSpaces != 0 || lineLen > 1);
        var separatorCount = line.GetSeparatorCount();

        if (separatorCount > 0)
        {
            glueWidth = line.WidthLeft / separatorCount;
        }
        else if (isJustified)
        {
            if (line.NewlineSplit && line.WidthLeft >= lastBaseFactor * (ratio * numberOfSpaces + lineLen - 1))
            {
                if (line.Rtl)
                {
                    text.MoveText(line.WidthLeft - lastBaseFactor * (ratio * numberOfSpaces + lineLen - 1), 0);
                }

                baseWordSpacing = ratio * lastBaseFactor;
                baseCharacterSpacing = lastBaseFactor;
            }
            else
            {
                var width = line.WidthLeft;
                var last = line.GetChunk(line.Size - 1);

                if (last != null)
                {
                    var s = last.ToString();
                    char c;

                    if (s.Length > 0 &&
                        hangingPunctuation.IndexOf((c = s[s.Length - 1]).ToString(), StringComparison.Ordinal) >= 0)
                    {
                        var oldWidth = width;
                        width += last.Font.Width(c) * 0.4f;
                        hangingCorrection = width - oldWidth;
                    }
                }

                var baseFactor = width / (ratio * numberOfSpaces + lineLen - 1);
                baseWordSpacing = ratio * baseFactor;
                baseCharacterSpacing = baseFactor;
                lastBaseFactor = baseFactor;
            }
        }

        var lastChunkStroke = line.LastStrokeChunk;
        var chunkStrokeIdx = 0;
        var xMarker = text.Xtlm;
        var baseXMarker = xMarker;
        var yMarker = text.Ytlm;
        var adjustMatrix = false;
        float tabPosition = 0;

        // looping over all the chunks in 1 line
        foreach (var chunk in line)
        {
            var color = chunk.Color;
            hScale = 1;

            if (chunkStrokeIdx <= lastChunkStroke)
            {
                float width;

                if (isJustified)
                {
                    width = chunk.GetWidthCorrected(baseCharacterSpacing, baseWordSpacing);
                }
                else
                {
                    width = chunk.Width;
                }

                if (chunk.IsStroked())
                {
                    var nextChunk = line.GetChunk(chunkStrokeIdx + 1);

                    if (chunk.IsSeparator())
                    {
                        width = glueWidth;
                        var sep = (object[])chunk.GetAttribute(Chunk.SEPARATOR);
                        var di = (IDrawInterface)sep[0];
                        var vertical = (bool)sep[1];
                        var fontSize = chunk.Font.Size;
                        var ascender = chunk.Font.Font.GetFontDescriptor(BaseFont.ASCENT, fontSize);
                        var descender = chunk.Font.Font.GetFontDescriptor(BaseFont.DESCENT, fontSize);

                        if (vertical)
                        {
                            di.Draw(graphics, baseXMarker, yMarker + descender, baseXMarker + line.OriginalWidth,
                                ascender - descender, yMarker);
                        }
                        else
                        {
                            di.Draw(graphics, xMarker, yMarker + descender, xMarker + width, ascender - descender,
                                yMarker);
                        }
                    }

                    if (chunk.IsTab())
                    {
                        var tab = (object[])chunk.GetAttribute(Chunk.TAB);
                        var di = (IDrawInterface)tab[0];
                        tabPosition = (float)tab[1] + (float)tab[3];
                        var fontSize = chunk.Font.Size;
                        var ascender = chunk.Font.Font.GetFontDescriptor(BaseFont.ASCENT, fontSize);
                        var descender = chunk.Font.Font.GetFontDescriptor(BaseFont.DESCENT, fontSize);

                        if (tabPosition > xMarker)
                        {
                            di.Draw(graphics, xMarker, yMarker + descender, tabPosition, ascender - descender, yMarker);
                        }

                        var tmp = xMarker;
                        xMarker = tabPosition;
                        tabPosition = tmp;
                    }

                    if (chunk.IsAttribute(Chunk.BACKGROUND))
                    {
                        var subtract = lastBaseFactor;

                        if (nextChunk != null && nextChunk.IsAttribute(Chunk.BACKGROUND))
                        {
                            subtract = 0;
                        }

                        if (nextChunk == null)
                        {
                            subtract += hangingCorrection;
                        }

                        var fontSize = chunk.Font.Size;
                        var ascender = chunk.Font.Font.GetFontDescriptor(BaseFont.ASCENT, fontSize);
                        var descender = chunk.Font.Font.GetFontDescriptor(BaseFont.DESCENT, fontSize);
                        var bgr = (object[])chunk.GetAttribute(Chunk.BACKGROUND);
                        graphics.SetColorFill((BaseColor)bgr[0]);
                        var extra = (float[])bgr[1];

                        graphics.Rectangle(xMarker - extra[0], yMarker + descender - extra[1] + chunk.TextRise,
                            width - subtract + extra[0] + extra[2], ascender - descender + extra[1] + extra[3]);

                        graphics.Fill();
                        graphics.SetGrayFill(0);
                    }

                    if (chunk.IsAttribute(Chunk.UNDERLINE))
                    {
                        var subtract = lastBaseFactor;

                        if (nextChunk != null && nextChunk.IsAttribute(Chunk.UNDERLINE))
                        {
                            subtract = 0;
                        }

                        if (nextChunk == null)
                        {
                            subtract += hangingCorrection;
                        }

                        var unders = (object[][])chunk.GetAttribute(Chunk.UNDERLINE);
                        BaseColor scolor = null;

                        for (var k = 0; k < unders.Length; ++k)
                        {
                            var obj = unders[k];
                            scolor = (BaseColor)obj[0];
                            var ps = (float[])obj[1];

                            if (scolor == null)
                            {
                                scolor = color;
                            }

                            if (scolor != null)
                            {
                                graphics.SetColorStroke(scolor);
                            }

                            var fsize = chunk.Font.Size;
                            graphics.SetLineWidth(ps[0] + fsize * ps[1]);
                            var shift = ps[2] + fsize * ps[3];
                            var cap2 = (int)ps[4];

                            if (cap2 != 0)
                            {
                                graphics.SetLineCap(cap2);
                            }

                            graphics.MoveTo(xMarker, yMarker + shift);
                            graphics.LineTo(xMarker + width - subtract, yMarker + shift);
                            graphics.Stroke();

                            if (scolor != null)
                            {
                                graphics.ResetGrayStroke();
                            }

                            if (cap2 != 0)
                            {
                                graphics.SetLineCap(0);
                            }
                        }

                        graphics.SetLineWidth(1);
                    }

                    if (chunk.IsAttribute(Chunk.ACTION))
                    {
                        var subtract = lastBaseFactor;

                        if (nextChunk != null && nextChunk.IsAttribute(Chunk.ACTION))
                        {
                            subtract = 0;
                        }

                        if (nextChunk == null)
                        {
                            subtract += hangingCorrection;
                        }

                        text.AddAnnotation(new PdfAnnotation(Writer, xMarker, yMarker, xMarker + width - subtract,
                            yMarker + chunk.Font.Size, (PdfAction)chunk.GetAttribute(Chunk.ACTION)));
                    }

                    if (chunk.IsAttribute(Chunk.REMOTEGOTO))
                    {
                        var subtract = lastBaseFactor;

                        if (nextChunk != null && nextChunk.IsAttribute(Chunk.REMOTEGOTO))
                        {
                            subtract = 0;
                        }

                        if (nextChunk == null)
                        {
                            subtract += hangingCorrection;
                        }

                        var obj = (object[])chunk.GetAttribute(Chunk.REMOTEGOTO);
                        var filename = (string)obj[0];

                        if (obj[1] is string)
                        {
                            RemoteGoto(filename, (string)obj[1], xMarker, yMarker, xMarker + width - subtract,
                                yMarker + chunk.Font.Size);
                        }
                        else
                        {
                            RemoteGoto(filename, (int)obj[1], xMarker, yMarker, xMarker + width - subtract,
                                yMarker + chunk.Font.Size);
                        }
                    }

                    if (chunk.IsAttribute(Chunk.LOCALGOTO))
                    {
                        var subtract = lastBaseFactor;

                        if (nextChunk != null && nextChunk.IsAttribute(Chunk.LOCALGOTO))
                        {
                            subtract = 0;
                        }

                        if (nextChunk == null)
                        {
                            subtract += hangingCorrection;
                        }

                        LocalGoto((string)chunk.GetAttribute(Chunk.LOCALGOTO), xMarker, yMarker,
                            xMarker + width - subtract, yMarker + chunk.Font.Size);
                    }

                    if (chunk.IsAttribute(Chunk.LOCALDESTINATION))
                    {
                        var subtract = lastBaseFactor;

                        if (nextChunk != null && nextChunk.IsAttribute(Chunk.LOCALDESTINATION))
                        {
                            subtract = 0;
                        }

                        if (nextChunk == null)
                        {
                            subtract += hangingCorrection;
                        }

                        LocalDestination((string)chunk.GetAttribute(Chunk.LOCALDESTINATION),
                            new PdfDestination(PdfDestination.XYZ, xMarker, yMarker + chunk.Font.Size, 0));
                    }

                    if (chunk.IsAttribute(Chunk.GENERICTAG))
                    {
                        var subtract = lastBaseFactor;

                        if (nextChunk != null && nextChunk.IsAttribute(Chunk.GENERICTAG))
                        {
                            subtract = 0;
                        }

                        if (nextChunk == null)
                        {
                            subtract += hangingCorrection;
                        }

                        var rect = new Rectangle(xMarker, yMarker, xMarker + width - subtract,
                            yMarker + chunk.Font.Size);

                        var pev = Writer.PageEvent;

                        if (pev != null)
                        {
                            pev.OnGenericTag(Writer, this, rect, (string)chunk.GetAttribute(Chunk.GENERICTAG));
                        }
                    }

                    if (chunk.IsAttribute(Chunk.PDFANNOTATION))
                    {
                        var subtract = lastBaseFactor;

                        if (nextChunk != null && nextChunk.IsAttribute(Chunk.PDFANNOTATION))
                        {
                            subtract = 0;
                        }

                        if (nextChunk == null)
                        {
                            subtract += hangingCorrection;
                        }

                        var fontSize = chunk.Font.Size;
                        var ascender = chunk.Font.Font.GetFontDescriptor(BaseFont.ASCENT, fontSize);
                        var descender = chunk.Font.Font.GetFontDescriptor(BaseFont.DESCENT, fontSize);

                        var annot = PdfAnnotation.ShallowDuplicate(
                            (PdfAnnotation)chunk.GetAttribute(Chunk.PDFANNOTATION));

                        annot.Put(PdfName.Rect,
                            new PdfRectangle(xMarker, yMarker + descender, xMarker + width - subtract,
                                yMarker + ascender));

                        text.AddAnnotation(annot);
                    }

                    var paramsx = (float[])chunk.GetAttribute(Chunk.SKEW);
                    var hs = chunk.GetAttribute(Chunk.HSCALE);

                    if (paramsx != null || hs != null)
                    {
                        float b = 0, c = 0;

                        if (paramsx != null)
                        {
                            b = paramsx[0];
                            c = paramsx[1];
                        }

                        if (hs != null)
                        {
                            hScale = (float)hs;
                        }

                        text.SetTextMatrix(hScale, b, c, 1, xMarker, yMarker);
                    }

                    if (chunk.IsImage())
                    {
                        var image = chunk.Image;
                        var matrix = image.Matrix;
                        matrix[Image.CX] = xMarker + chunk.ImageOffsetX - matrix[Image.CX];
                        matrix[Image.CY] = yMarker + chunk.ImageOffsetY - matrix[Image.CY];
                        graphics.AddImage(image, matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);
                        text.MoveText(xMarker + lastBaseFactor + image.ScaledWidth - text.Xtlm, 0);
                    }
                }

                xMarker += width;
                ++chunkStrokeIdx;
            }

            if (chunk.Font.CompareTo(currentFont) != 0)
            {
                currentFont = chunk.Font;
                text.SetFontAndSize(currentFont.Font, currentFont.Size);
            }

            float rise = 0;
            var textRender = (object[])chunk.GetAttribute(Chunk.TEXTRENDERMODE);
            var tr = 0;
            float strokeWidth = 1;
            BaseColor strokeColor = null;
            var fr = chunk.GetAttribute(Chunk.SUBSUPSCRIPT);

            if (textRender != null)
            {
                tr = (int)textRender[0] & 3;

                if (tr != PdfContentByte.TEXT_RENDER_MODE_FILL)
                {
                    text.SetTextRenderingMode(tr);
                }

                if (tr == PdfContentByte.TEXT_RENDER_MODE_STROKE || tr == PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE)
                {
                    strokeWidth = (float)textRender[1];

                    if (strokeWidth.ApproxNotEqual(1))
                    {
                        text.SetLineWidth(strokeWidth);
                    }

                    strokeColor = (BaseColor)textRender[2];

                    if (strokeColor == null)
                    {
                        strokeColor = color;
                    }

                    if (strokeColor != null)
                    {
                        text.SetColorStroke(strokeColor);
                    }
                }
            }

            if (fr != null)
            {
                rise = (float)fr;
            }

            if (color != null)
            {
                text.SetColorFill(color);
            }

            if (rise.ApproxNotEqual(0))
            {
                text.SetTextRise(rise);
            }

            if (chunk.IsImage())
            {
                adjustMatrix = true;
            }
            else if (chunk.IsHorizontalSeparator())
            {
                var array = new PdfTextArray();
                array.Add(-glueWidth * 1000f / chunk.Font.Size / hScale);
                text.ShowText(array);
            }
            else if (chunk.IsTab())
            {
                var array = new PdfTextArray();
                array.Add((tabPosition - xMarker) * 1000f / chunk.Font.Size / hScale);
                text.ShowText(array);
            }

            // If it is a CJK chunk or Unicode TTF we will have to simulate the
            // space adjustment.
            else if (isJustified && numberOfSpaces > 0 && chunk.IsSpecialEncoding())
            {
                if (hScale.ApproxNotEqual(lastHScale))
                {
                    lastHScale = hScale;
                    text.SetWordSpacing(baseWordSpacing / hScale);
                    text.SetCharacterSpacing(baseCharacterSpacing / hScale);
                }

                var s = chunk.ToString();
                var idx = s.IndexOf(" ", StringComparison.Ordinal);

                if (idx < 0)
                {
                    text.ShowText(s);
                }
                else
                {
                    var spaceCorrection = -baseWordSpacing * 1000f / chunk.Font.Size / hScale;
                    var textArray = new PdfTextArray(s.Substring(0, idx));
                    var lastIdx = idx;

                    while ((idx = s.IndexOf(" ", lastIdx + 1, StringComparison.Ordinal)) >= 0)
                    {
                        textArray.Add(spaceCorrection);
                        textArray.Add(s.Substring(lastIdx, idx - lastIdx));
                        lastIdx = idx;
                    }

                    textArray.Add(spaceCorrection);
                    textArray.Add(s.Substring(lastIdx));
                    text.ShowText(textArray);
                }
            }
            else
            {
                if (isJustified && hScale.ApproxNotEqual(lastHScale))
                {
                    lastHScale = hScale;
                    text.SetWordSpacing(baseWordSpacing / hScale);
                    text.SetCharacterSpacing(baseCharacterSpacing / hScale);
                }

                text.ShowText(chunk.ToString());
            }

            if (rise.ApproxNotEqual(0))
            {
                text.SetTextRise(0);
            }

            if (color != null)
            {
                text.ResetRgbColorFill();
            }

            if (tr != PdfContentByte.TEXT_RENDER_MODE_FILL)
            {
                text.SetTextRenderingMode(PdfContentByte.TEXT_RENDER_MODE_FILL);
            }

            if (strokeColor != null)
            {
                text.ResetRgbColorStroke();
            }

            if (strokeWidth.ApproxNotEqual(1))
            {
                text.SetLineWidth(1);
            }

            if (chunk.IsAttribute(Chunk.SKEW) || chunk.IsAttribute(Chunk.HSCALE))
            {
                adjustMatrix = true;
                text.SetTextMatrix(xMarker, yMarker);
            }
        }

        if (isJustified)
        {
            text.SetWordSpacing(0);
            text.SetCharacterSpacing(0);

            if (line.NewlineSplit)
            {
                lastBaseFactor = 0;
            }
        }

        if (adjustMatrix)
        {
            text.MoveText(baseXMarker - text.Xtlm, 0);
        }

        currentValues[0] = currentFont;
        currentValues[1] = lastBaseFactor;
    }

    internal void WriteOutlines()
    {
        if (rootOutline.Kids.Count == 0)
        {
            return;
        }

        OutlineTree(rootOutline);
        Writer.AddToBody(rootOutline, rootOutline.IndirectReference);
    }

    /// <summary>
    ///     Adds an image to the document.
    ///     @throws PdfException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="image">the  Image  to add</param>
    protected internal void Add(Image image)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        if (image.HasAbsolutePosition())
        {
            Graphics.AddImage(image);
            pageEmpty = false;

            return;
        }

        // if there isn't enough room for the image on this page, save it for the next page
        if (CurrentHeight.ApproxNotEqual(0) && IndentTop - CurrentHeight - image.ScaledHeight < IndentBottom)
        {
            if (!strictImageSequence && ImageWait == null)
            {
                ImageWait = image;

                return;
            }

            NewPage();

            if (CurrentHeight.ApproxNotEqual(0) && IndentTop - CurrentHeight - image.ScaledHeight < IndentBottom)
            {
                ImageWait = image;

                return;
            }
        }

        pageEmpty = false;

        // avoid endless loops
        if (image == ImageWait)
        {
            ImageWait = null;
        }

        var textwrap = (image.Alignment & Image.TEXTWRAP) == Image.TEXTWRAP &&
                       !((image.Alignment & Image.MIDDLE_ALIGN) == Image.MIDDLE_ALIGN);

        var underlying = (image.Alignment & Image.UNDERLYING) == Image.UNDERLYING;
        var diff = leading / 2;

        if (textwrap)
        {
            diff += leading;
        }

        var lowerleft = IndentTop - CurrentHeight - image.ScaledHeight - diff;
        var mt = image.Matrix;
        var startPosition = IndentLeft - mt[4];

        if ((image.Alignment & Image.RIGHT_ALIGN) == Image.RIGHT_ALIGN)
        {
            startPosition = IndentRight - image.ScaledWidth - mt[4];
        }

        if ((image.Alignment & Image.MIDDLE_ALIGN) == Image.MIDDLE_ALIGN)
        {
            startPosition = IndentLeft + (IndentRight - IndentLeft - image.ScaledWidth) / 2 - mt[4];
        }

        if (image.HasAbsoluteX())
        {
            startPosition = image.AbsoluteX;
        }

        if (textwrap)
        {
            if (ImageEnd < 0 || ImageEnd < CurrentHeight + image.ScaledHeight + diff)
            {
                ImageEnd = CurrentHeight + image.ScaledHeight + diff;
            }

            if ((image.Alignment & Image.RIGHT_ALIGN) == Image.RIGHT_ALIGN)
            {
                // indentation suggested by Pelikan Stephan
                indentation.ImageIndentRight += image.ScaledWidth + image.IndentationLeft;
            }
            else
            {
                // indentation suggested by Pelikan Stephan
                indentation.ImageIndentLeft += image.ScaledWidth + image.IndentationRight;
            }
        }
        else
        {
            if ((image.Alignment & Image.RIGHT_ALIGN) == Image.RIGHT_ALIGN)
            {
                startPosition -= image.IndentationRight;
            }
            else if ((image.Alignment & Image.MIDDLE_ALIGN) == Image.MIDDLE_ALIGN)
            {
                startPosition += image.IndentationLeft - image.IndentationRight;
            }
            else
            {
                startPosition -= image.IndentationRight;
            }
        }

        Graphics.AddImage(image, mt[0], mt[1], mt[2], mt[3], startPosition, lowerleft - mt[5]);

        if (!(textwrap || underlying))
        {
            CurrentHeight += image.ScaledHeight + diff;
            FlushLines();
            Text.MoveText(0, -(image.ScaledHeight + diff));
            NewLine();
        }
    }

    /// <summary>
    ///     Gets the indentation on the left side.
    /// </summary>
    /// <returns>a margin</returns>
    /// <summary>
    ///     Gets the indentation on the right side.
    /// </summary>
    /// <returns>a margin</returns>
    /// <summary>
    ///     Gets the indentation on the top side.
    /// </summary>
    /// <returns>a margin</returns>
    /// <summary>
    ///     Gets the indentation on the bottom side.
    /// </summary>
    /// <returns>a margin</returns>
    /// <summary>
    ///     Adds extra space.
    ///     This method should probably be rewritten.
    /// </summary>
    protected internal void AddSpacing(float extraspace, float oldleading, Font f)
    {
        if (f == null)
        {
            throw new ArgumentNullException(nameof(f));
        }

        if (extraspace.ApproxEquals(0))
        {
            return;
        }

        if (pageEmpty)
        {
            return;
        }

        if (CurrentHeight + Line.Height + leading > IndentTop - IndentBottom)
        {
            return;
        }

        leading = extraspace;
        CarriageReturn();

        if (f.IsUnderlined() || f.IsStrikethru())
        {
            f = new Font(f);
            var style = f.Style;
            style &= ~Font.UNDERLINE;
            style &= ~Font.STRIKETHRU;
            f.SetStyle(style);
        }

        var space = new Chunk(" ", f);
        space.Process(this);
        CarriageReturn();
        leading = oldleading;
    }

    protected internal void AnalyzeRow(IList<IList<PdfCell>> rows, RenderingContext ctx)
    {
        if (rows == null)
        {
            throw new ArgumentNullException(nameof(rows));
        }

        if (ctx == null)
        {
            throw new ArgumentNullException(nameof(ctx));
        }

        ctx.MaxCellBottom = IndentBottom;

        // determine whether Row(index) is in a rowspan
        var rowIndex = 0;

        var row = rows[rowIndex];
        var maxRowspan = 1;

        foreach (var cell in row)
        {
            maxRowspan = Math.Max(ctx.CurrentRowspan(cell), maxRowspan);
        }

        rowIndex += maxRowspan;

        var useTop = true;

        if (rowIndex == rows.Count)
        {
            rowIndex = rows.Count - 1;
            useTop = false;
        }

        if (rowIndex < 0 || rowIndex >= rows.Count)
        {
            return;
        }

        row = rows[rowIndex];

        foreach (var cell in row)
        {
            var cellRect = cell.Rectangle(ctx.Pagetop, IndentBottom);

            if (useTop)
            {
                ctx.MaxCellBottom = Math.Max(ctx.MaxCellBottom, cellRect.Top);
            }
            else
            {
                if (ctx.CurrentRowspan(cell) == 1)
                {
                    ctx.MaxCellBottom = Math.Max(ctx.MaxCellBottom, cellRect.Bottom);
                }
            }
        }
    }

    protected internal void CarriageReturn()
    {
        // the arraylist with lines may not be null
        if (Lines == null)
        {
            Lines = new List<PdfLine>();
        }

        // If the current line is not null
        if (Line != null)
        {
            // we check if the end of the page is reached (bugfix by Francois Gravel)
            if (CurrentHeight + Line.Height + leading < IndentTop - IndentBottom)
            {
                // if so nonempty lines are added and the heigt is augmented
                if (Line.Size > 0)
                {
                    CurrentHeight += Line.Height;
                    Lines.Add(Line);
                    pageEmpty = false;
                }
            }

            // if the end of the line is reached, we start a new page
            else
            {
                NewPage();
            }
        }

        if (ImageEnd > -1 && CurrentHeight > ImageEnd)
        {
            ImageEnd = -1;
            indentation.ImageIndentRight = 0;
            indentation.ImageIndentLeft = 0;
        }

        // a new current line is constructed
        Line = new PdfLine(IndentLeft, IndentRight, Alignment, leading);
    }

    protected internal static void ConsumeRowspan(IList<PdfCell> row, RenderingContext ctx)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row));
        }

        if (ctx == null)
        {
            throw new ArgumentNullException(nameof(ctx));
        }

        foreach (var c in row)
        {
            ctx.ConsumeRowspan(c);
        }
    }

    /// <summary>
    ///     [M5] header/footer
    /// </summary>
    protected internal void DoFooter()
    {
        if (footer == null)
        {
            return;
        }

        // Begin added by Edgar Leonardo Prieto Perilla
        // Avoid footer identation
        var tmpIndentLeft = indentation.indentLeft;
        var tmpIndentRight = indentation.indentRight;

        // Begin added: Bonf (Marc Schneider) 2003-07-29
        var tmpListIndentLeft = indentation.ListIndentLeft;
        var tmpImageIndentLeft = indentation.ImageIndentLeft;
        var tmpImageIndentRight = indentation.ImageIndentRight;

        // End added: Bonf (Marc Schneider) 2003-07-29

        indentation.indentLeft = indentation.indentRight = 0;

        // Begin added: Bonf (Marc Schneider) 2003-07-29
        indentation.ListIndentLeft = 0;
        indentation.ImageIndentLeft = 0;
        indentation.ImageIndentRight = 0;

        // End added: Bonf (Marc Schneider) 2003-07-29
        // End Added by Edgar Leonardo Prieto Perilla
        footer.PageNumber = PageNumber;
        leading = footer.Paragraph.TotalLeading;
        Add(footer.Paragraph);

        // adding the footer limits the height
        indentation.indentBottom = CurrentHeight;
        Text.MoveText(Left, IndentBottom);
        FlushLines();
        Text.MoveText(-Left, -Bottom);
        footer.Top = GetBottom(CurrentHeight);
        footer.Bottom = Bottom - 0.75f * leading;
        footer.Left = Left;
        footer.Right = Right;
        Graphics.Rectangle(footer);
        indentation.indentBottom = CurrentHeight + leading * 2;
        CurrentHeight = 0;

        // Begin added by Edgar Leonardo Prieto Perilla
        indentation.indentLeft = tmpIndentLeft;
        indentation.indentRight = tmpIndentRight;

        // Begin added: Bonf (Marc Schneider) 2003-07-29
        indentation.ListIndentLeft = tmpListIndentLeft;
        indentation.ImageIndentLeft = tmpImageIndentLeft;
        indentation.ImageIndentRight = tmpImageIndentRight;

        // End added: Bonf (Marc Schneider) 2003-07-29
        // End added by Edgar Leonardo Prieto Perilla
    }

    protected internal void DoHeader()
    {
        // if there is a header, the header = added
        if (header == null)
        {
            return;
        }

        // Begin added by Edgar Leonardo Prieto Perilla
        // Avoid header identation
        var tmpIndentLeft = indentation.indentLeft;
        var tmpIndentRight = indentation.indentRight;

        // Begin added: Bonf (Marc Schneider) 2003-07-29
        var tmpListIndentLeft = indentation.ListIndentLeft;
        var tmpImageIndentLeft = indentation.ImageIndentLeft;
        var tmpImageIndentRight = indentation.ImageIndentRight;

        // End added: Bonf (Marc Schneider) 2003-07-29
        indentation.indentLeft = indentation.indentRight = 0;

        //  Added: Bonf
        indentation.ListIndentLeft = 0;
        indentation.ImageIndentLeft = 0;
        indentation.ImageIndentRight = 0;

        // End added: Bonf
        // Begin added by Edgar Leonardo Prieto Perilla
        header.PageNumber = PageNumber;
        leading = header.Paragraph.TotalLeading;
        Text.MoveText(0, leading);
        Add(header.Paragraph);
        NewLine();
        indentation.indentTop = CurrentHeight - leading;
        header.Top = Top + leading;
        header.Bottom = IndentTop + leading * 2 / 3;
        header.Left = Left;
        header.Right = Right;
        Graphics.Rectangle(header);
        FlushLines();
        CurrentHeight = 0;

        // Begin added by Edgar Leonardo Prieto Perilla
        // Restore identation
        indentation.indentLeft = tmpIndentLeft;
        indentation.indentRight = tmpIndentRight;

        // Begin added: Bonf (Marc Schneider) 2003-07-29
        indentation.ListIndentLeft = tmpListIndentLeft;
        indentation.ImageIndentLeft = tmpImageIndentLeft;
        indentation.ImageIndentRight = tmpImageIndentRight;

        // End added: Bonf (Marc Schneider) 2003-07-29
        // End Added by Edgar Leonardo Prieto Perilla
    }

    /// <summary>
    ///     If the current line is not empty or null, it is added to the arraylist
    ///     of lines and a new empty line is added.
    ///     @throws DocumentException on error
    /// </summary>
    /// <summary>
    ///     Ensures that a new line has been started.
    /// </summary>
    protected internal void EnsureNewLine()
    {
        if (LastElementType == Element.PHRASE || LastElementType == Element.CHUNK)
        {
            NewLine();
            FlushLines();
        }
    }

    protected internal static IList<IList<PdfCell>> ExtractRows(IList<PdfCell> cells, RenderingContext ctx)
    {
        PdfCell cell;
        PdfCell previousCell = null;
        var rows = new List<IList<PdfCell>>();
        var rowCells = new List<PdfCell>();

        var iterator = new ListIterator<PdfCell>(cells);

        while (iterator.HasNext())
        {
            cell = iterator.Next();

            var isAdded = false;

            var isEndOfRow = !iterator.HasNext();
            var isCurrentCellPartOfRow = !iterator.HasNext();

            if (previousCell != null)
            {
                if (cell.Left <= previousCell.Left)
                {
                    isEndOfRow = true;
                    isCurrentCellPartOfRow = false;
                }
            }

            if (isCurrentCellPartOfRow)
            {
                rowCells.Add(cell);
                isAdded = true;
            }

            if (isEndOfRow)
            {
                if (rowCells.Count != 0)
                {
                    // add to rowlist
                    rows.Add(rowCells);
                }

                // start a new list for next line
                rowCells = new List<PdfCell>();
            }

            if (!isAdded)
            {
                rowCells.Add(cell);
            }

            previousCell = cell;
        }

        if (rowCells.Count != 0)
        {
            rows.Add(rowCells);
        }

        // fill row information with rowspan cells to get complete "scan lines"
        for (var i = rows.Count - 1; i >= 0; i--)
        {
            var row = rows[i];

            // iterator through row
            for (var j = 0; j < row.Count; j++)
            {
                var c = row[j];
                var rowspan = c.Rowspan;

                // fill in missing rowspan cells to complete "scan line"
                for (var k = 1; k < rowspan && rows.Count < i + k; k++)
                {
                    var spannedRow = rows[i + k];

                    if (spannedRow.Count > j)
                    {
                        spannedRow.Insert(j, c);
                    }
                }
            }
        }

        return rows;
    }

    /// <summary>
    ///     Writes all the lines to the text-object.
    ///     @throws DocumentException on error
    /// </summary>
    /// <returns>the displacement that was caused</returns>
    protected internal float FlushLines()
    {
        // checks if the ArrayList with the lines is not null
        if (Lines == null)
        {
            return 0;
        }

        // checks if a new Line has to be made.
        if (Line != null && Line.Size > 0)
        {
            Lines.Add(Line);
            Line = new PdfLine(IndentLeft, IndentRight, Alignment, leading);
        }

        // checks if the ArrayList with the lines is empty
        if (Lines.Count == 0)
        {
            return 0;
        }

        // initialisation of some parameters
        var currentValues = new object[2];
        PdfFont currentFont = null;
        float displacement = 0;

        currentValues[1] = (float)0;

        // looping over all the lines
        foreach (var l in Lines)
        {
            // this is a line in the loop

            var moveTextX = l.IndentLeft - IndentLeft + indentation.indentLeft + indentation.ListIndentLeft +
                            indentation.SectionIndentLeft;

            Text.MoveText(moveTextX, -l.Height);

            // is the line preceeded by a symbol?
            if (l.ListSymbol != null)
            {
                ColumnText.ShowTextAligned(Graphics, Element.ALIGN_LEFT, new Phrase(l.ListSymbol),
                    Text.Xtlm - l.ListIndent, Text.Ytlm, 0);
            }

            currentValues[0] = currentFont;

            WriteLineToContent(l, Text, Graphics, currentValues, Writer.SpaceCharRatio);

            currentFont = (PdfFont)currentValues[0];

            displacement += l.Height;
            Text.MoveText(-moveTextX, 0);
        }

        Lines = new List<PdfLine>();

        return displacement;
    }

    protected internal void InitPage()
    {
        // the pagenumber is incremented
        PageNumber++;

        // initialisation of some page objects
        AnnotationsImp.ResetAnnotations();
        pageResources = new PageResources();

        Writer.ResetContent();
        Graphics = new PdfContentByte(Writer);
        Text = new PdfContentByte(Writer);
        Text.Reset();
        Text.BeginText();
        TextEmptySize = Text.Size;

        MarkPoint = 0;
        SetNewPageSizeAndMargins();
        ImageEnd = -1;
        indentation.ImageIndentRight = 0;
        indentation.ImageIndentLeft = 0;
        indentation.indentBottom = 0;
        indentation.indentTop = 0;
        CurrentHeight = 0;

        // backgroundcolors, etc...
        ThisBoxSize = BoxSize.Clone();

        if (PageSize.BackgroundColor != null || PageSize.HasBorders() || PageSize.BorderColor != null)
        {
            Add(PageSize);
        }

        var oldleading = leading;
        var oldAlignment = Alignment;

        // if there is a footer, the footer is added
        DoFooter();

        // we move to the left/top position of the page
        Text.MoveText(Left, Top);
        DoHeader();
        pageEmpty = true;

        // if there is an image waiting to be drawn, draw it
        if (ImageWait != null)
        {
            Add(ImageWait);
            ImageWait = null;
        }

        leading = oldleading;
        Alignment = oldAlignment;
        CarriageReturn();

        var pageEvent = Writer.PageEvent;

        if (pageEvent != null)
        {
            if (FirstPageEvent)
            {
                pageEvent.OnOpenDocument(Writer, this);
            }

            pageEvent.OnStartPage(Writer, this);
        }

        FirstPageEvent = false;
    }

    protected internal static bool MayBeRemoved(IList<PdfCell> row)
    {
        if (row == null)
        {
            throw new ArgumentNullException(nameof(row));
        }

        var mayBeRemoved = true;

        foreach (var cell in row)
        {
            mayBeRemoved &= cell.MayBeRemoved();
        }

        return mayBeRemoved;
    }

    protected internal void NewLine()
    {
        LastElementType = -1;
        CarriageReturn();

        if (Lines != null && Lines.Count > 0)
        {
            Lines.Add(Line);
            CurrentHeight += Line.Height;
        }

        Line = new PdfLine(IndentLeft, IndentRight, Alignment, leading);
    }

    protected internal void RenderCells(RenderingContext ctx, IList<PdfCell> cells, bool hasToFit)
    {
        if (ctx == null)
        {
            throw new ArgumentNullException(nameof(ctx));
        }

        if (cells == null)
        {
            throw new ArgumentNullException(nameof(cells));
        }

        if (hasToFit)
        {
            foreach (var cell in cells)
            {
                if (!cell.Header)
                {
                    if (cell.Bottom < IndentBottom)
                    {
                        return;
                    }
                }
            }
        }

        foreach (var cell in cells)
        {
            if (!ctx.IsCellRenderedOnPage(cell, PageNumber))
            {
                float correction = 0;

                if (ctx.NumCellRendered(cell) >= 1)
                {
                    correction = 1.0f;
                }

                Lines = cell.GetLines(ctx.Pagetop, IndentBottom - correction);

                // if there is still text to render we render it
                if (Lines != null && Lines.Count > 0)
                {
                    // we write the text
                    var cellTop = cell.GetTop(ctx.Pagetop - ctx.OldHeight);
                    Text.MoveText(0, cellTop);
                    var cellDisplacement = FlushLines() - cellTop;

                    Text.MoveText(0, cellDisplacement);

                    if (ctx.OldHeight + cellDisplacement > CurrentHeight)
                    {
                        CurrentHeight = ctx.OldHeight + cellDisplacement;
                    }

                    ctx.CellRendered(cell, PageNumber);
                }

                var indentBottom = Math.Max(cell.Bottom, IndentBottom);

                var tableRect = ctx.Table.GetRectangle(ctx.Pagetop, IndentBottom);

                indentBottom = Math.Max(tableRect.Bottom, indentBottom);

                // we paint the borders of the cells
                var cellRect = cell.GetRectangle(tableRect.Top, indentBottom);

                //cellRect.Bottom = cellRect.Bottom;
                if (cellRect.Height > 0)
                {
                    ctx.LostTableBottom = indentBottom;
                    ctx.CellGraphics.Rectangle(cellRect);
                }

                // and additional graphics
                var images = cell.GetImages(ctx.Pagetop, IndentBottom);

                foreach (var image in images)
                {
                    Graphics.AddImage(image);
                }
            }
        }
    }

    protected internal void SetNewPageSizeAndMargins()
    {
        PageSize = NextPageSize;

        if (MarginMirroring && (PageNumber & 1) == 0)
        {
            RightMargin = NextMarginLeft;
            LeftMargin = NextMarginRight;
        }
        else
        {
            LeftMargin = NextMarginLeft;
            RightMargin = NextMarginRight;
        }

        if (MarginMirroringTopBottom && (PageNumber & 1) == 0)
        {
            TopMargin = NextMarginBottom;
            BottomMargin = NextMarginTop;
        }
        else
        {
            TopMargin = NextMarginTop;
            BottomMargin = NextMarginBottom;
        }
    }

    /// <summary>
    ///     Adds a new table to
    ///     @throws DocumentException
    /// </summary>
    /// <param name="t">Table to add. Rendered rows will be deleted after processing.</param>
    private void addPdfTable(Table t)
    {
        // before every table, we flush all lines
        FlushLines();

        var table = new PdfTable(t, IndentLeft, IndentRight, IndentTop - CurrentHeight);
        var ctx = new RenderingContext();
        ctx.Pagetop = IndentTop;
        ctx.OldHeight = CurrentHeight;
        ctx.CellGraphics = new PdfContentByte(Writer);
        ctx.RowspanMap = new NullValueDictionary<object, object>();
        ctx.Table = table;

        // initialisation of parameters
        PdfCell cell;

        // drawing the table
        var headercells = table.HeaderCells;
        var cells = table.Cells;
        var rows = ExtractRows(cells, ctx);
        var isContinue = false;

        while (cells.Count != 0)
        {
            // initialisation of some extra parameters;
            ctx.LostTableBottom = 0;

            // loop over the cells
            var cellsShown = false;

            // draw the cells (line by line)
            var iterator = new ListIterator<IList<PdfCell>>(rows);

            var atLeastOneFits = false;

            while (iterator.HasNext())
            {
                var row = iterator.Next();
                AnalyzeRow(rows, ctx);
                RenderCells(ctx, row, table.HasToFitPageCells() & atLeastOneFits);

                if (!MayBeRemoved(row))
                {
                    break;
                }

                ConsumeRowspan(row, ctx);
                iterator.Remove();
                atLeastOneFits = true;
            }

            //          compose cells array list for subsequent code
            cells.Clear();
            var opt = new NullValueDictionary<PdfCell, object>();

            foreach (var row in rows)
            {
                foreach (var cellp in row)
                {
                    if (!opt.ContainsKey(cellp))
                    {
                        cells.Add(cellp);
                        opt[cellp] = null;
                    }
                }
            }

            // we paint the graphics of the table after looping through all the cells
            var tablerec = new Rectangle(table);
            tablerec.Border = table.Border;
            tablerec.BorderWidth = table.BorderWidth;
            tablerec.BorderColor = table.BorderColor;
            tablerec.BackgroundColor = table.BackgroundColor;
            var under = Writer.DirectContentUnder;
            under.Rectangle(tablerec.GetRectangle(Top, IndentBottom));
            under.Add(ctx.CellGraphics);

            // bugfix by Gerald Fehringer: now again add the border for the table
            // since it might have been covered by cell backgrounds
            tablerec.BackgroundColor = null;
            tablerec = tablerec.GetRectangle(Top, IndentBottom);
            tablerec.Border = table.Border;
            under.Rectangle(tablerec);

            // end bugfix
            ctx.CellGraphics = new PdfContentByte(null);

            // if the table continues on the next page
            if (rows.Count != 0)
            {
                isContinue = true;
                Graphics.SetLineWidth(table.BorderWidth);

                if (cellsShown && (table.Border & Rectangle.BOTTOM_BORDER) == Rectangle.BOTTOM_BORDER)
                {
                    // Draw the bottom line

                    // the color is set to the color of the element
                    var tColor = table.BorderColor;

                    if (tColor != null)
                    {
                        Graphics.SetColorStroke(tColor);
                    }

                    Graphics.MoveTo(table.Left, Math.Max(table.Bottom, IndentBottom));
                    Graphics.LineTo(table.Right, Math.Max(table.Bottom, IndentBottom));
                    Graphics.Stroke();

                    if (tColor != null)
                    {
                        Graphics.ResetRgbColorStroke();
                    }
                }

                // old page
                pageEmpty = false;
                var difference = ctx.LostTableBottom;

                // new page
                NewPage();

                // G.F.: if something added in page event i.e. currentHeight > 0
                float heightCorrection = 0;
                var somethingAdded = false;

                if (CurrentHeight > 0)
                {
                    heightCorrection = 6;
                    CurrentHeight += heightCorrection;
                    somethingAdded = true;
                    NewLine();
                    FlushLines();
                    indentation.indentTop = CurrentHeight - leading;
                    CurrentHeight = 0;
                }
                else
                {
                    FlushLines();
                }

                // this part repeats the table headers (if any)
                var size = headercells.Count;

                if (size > 0)
                {
                    // this is the top of the headersection
                    cell = headercells[0];
                    var oldTop = cell.GetTop(0);

                    // loop over all the cells of the table header
                    for (var ii = 0; ii < size; ii++)
                    {
                        cell = headercells[ii];

                        // calculation of the new cellpositions
                        cell.Top = IndentTop - oldTop + cell.GetTop(0);
                        cell.Bottom = IndentTop - oldTop + cell.GetBottom(0);
                        ctx.Pagetop = cell.Bottom;

                        // we paint the borders of the cell
                        ctx.CellGraphics.Rectangle(cell.Rectangle(IndentTop, IndentBottom));

                        // we write the text of the cell
                        var images = cell.GetImages(IndentTop, IndentBottom);

                        foreach (var image in images)
                        {
                            cellsShown = true;
                            Graphics.AddImage(image);
                        }

                        Lines = cell.GetLines(IndentTop, IndentBottom);
                        var cellTop = cell.GetTop(IndentTop);
                        Text.MoveText(0, cellTop - heightCorrection);
                        var cellDisplacement = FlushLines() - cellTop + heightCorrection;
                        Text.MoveText(0, cellDisplacement);
                    }

                    CurrentHeight = IndentTop - ctx.Pagetop + table.Cellspacing;
                    Text.MoveText(0, ctx.Pagetop - IndentTop - CurrentHeight);
                }
                else
                {
                    if (somethingAdded)
                    {
                        ctx.Pagetop = IndentTop;
                        Text.MoveText(0, -table.Cellspacing);
                    }
                }

                ctx.OldHeight = CurrentHeight - heightCorrection;

                // calculating the new positions of the table and the cells
                size = Math.Min(cells.Count, table.Columns);
                var i = 0;

                while (i < size)
                {
                    cell = cells[i];

                    if (cell.GetTop(-table.Cellspacing) > ctx.LostTableBottom)
                    {
                        var newBottom = ctx.Pagetop - difference + cell.Bottom;
                        var neededHeight = cell.RemainingHeight;

                        if (newBottom > ctx.Pagetop - neededHeight)
                        {
                            difference += newBottom - (ctx.Pagetop - neededHeight);
                        }
                    }

                    i++;
                }

                size = cells.Count;
                table.Top = IndentTop;
                table.Bottom = ctx.Pagetop - difference + table.GetBottom(table.Cellspacing);

                for (i = 0; i < size; i++)
                {
                    cell = cells[i];
                    var newBottom = ctx.Pagetop - difference + cell.Bottom;
                    var newTop = ctx.Pagetop - difference + cell.GetTop(-table.Cellspacing);

                    if (newTop > IndentTop - CurrentHeight)
                    {
                        newTop = IndentTop - CurrentHeight;
                    }

                    cell.Top = newTop;
                    cell.Bottom = newBottom;
                }
            }
        }

        var tableHeight = table.Top - table.Bottom;

        // bugfix by Adauto Martins when have more than two tables and more than one page
        // If continuation of table in other page (bug report #1460051)
        if (isContinue)
        {
            CurrentHeight = tableHeight;
            Text.MoveText(0, -(tableHeight - ctx.OldHeight * 2));
        }
        else
        {
            CurrentHeight = ctx.OldHeight + tableHeight;
            Text.MoveText(0, -tableHeight);
        }

        pageEmpty = false;
    }

    /// <summary>
    ///     [L1] DocListener interface
    /// </summary>
    /// <summary>
    ///     [L4] DocListener interface
    /// </summary>
    /// <summary>
    ///     [L7] DocListener interface
    /// </summary>
    /// <summary>
    ///     [L8] DocListener interface
    /// </summary>
    /// <summary>
    ///     DOCLISTENER METHODS END
    /// </summary>
    /// <summary>
    ///     Initializes a page.
    ///     If the footer/header is set, it is printed.
    ///     @throws DocumentException on error
    /// </summary>
    /// <summary>
    ///     Adds the current line to the list of lines and also adds an empty line.
    ///     @throws DocumentException on error
    /// </summary>
    public class Indentation
    {
        /// <summary>
        ///     This is the indentation caused by an image on the left.
        /// </summary>
        internal float ImageIndentLeft;

        /// <summary>
        ///     This is the indentation caused by an image on the right.
        /// </summary>
        internal float ImageIndentRight;

        /// <summary>
        ///     This represents the current indentation of the PDF Elements on the bottom side.
        /// </summary>
        internal float indentBottom;

        /// <summary>
        ///     This represents the current indentation of the PDF Elements on the left side.
        /// </summary>
        internal float indentLeft;

        /// <summary>
        ///     This represents the current indentation of the PDF Elements on the right side.
        /// </summary>
        internal float indentRight;

        /// <summary>
        ///     This represents the current indentation of the PDF Elements on the top side.
        /// </summary>
        internal float indentTop;

        /// <summary>
        ///     This represents the current indentation of the PDF Elements on the left side.
        /// </summary>
        internal float ListIndentLeft;

        /// <summary>
        ///     Indentation to the left caused by a section.
        /// </summary>
        internal float SectionIndentLeft;

        /// <summary>
        ///     Indentation to the right caused by a section.
        /// </summary>
        internal float SectionIndentRight;
    }

    public class PdfInfo : PdfDictionary
    {
        /// <summary>
        ///     constructors
        /// </summary>
        /// <summary>
        ///     Construct a  PdfInfo -object.
        /// </summary>
        internal PdfInfo()
        {
            AddProducer();
            AddCreationDate();
        }

        /// <summary>
        ///     Constructs a  PdfInfo -object.
        /// </summary>
        /// <param name="author">name of the author of the document</param>
        /// <param name="title">title of the document</param>
        /// <param name="subject">subject of the document</param>
        internal PdfInfo(string author, string title, string subject)
        {
            AddTitle(title);
            AddSubject(subject);
            AddAuthor(author);
        }

        internal void AddAuthor(string author)
            => Put(PdfName.Author, new PdfString(author, TEXT_UNICODE));

        internal void AddCreationDate()
        {
            PdfString date = new PdfDate();
            Put(PdfName.Creationdate, date);
            Put(PdfName.Moddate, date);
        }

        internal void AddCreator(string creator)
            => Put(PdfName.Creator, new PdfString(creator, TEXT_UNICODE));

        /// <summary>
        ///     Adds the date of creation to the document.
        /// </summary>
        internal void Addkey(string key, string value)
        {
            if (key.Equals("Producer", StringComparison.Ordinal) ||
                key.Equals("CreationDate", StringComparison.Ordinal))
            {
                return;
            }

            Put(new PdfName(key), new PdfString(value, TEXT_UNICODE));
        }

        internal void AddKeywords(string keywords)
            => Put(PdfName.Keywords, new PdfString(keywords, TEXT_UNICODE));

        internal void AddProducer()

            // This line may only be changed by Bruno Lowagie or Paulo Soares
            => Put(PdfName.Producer, new PdfString(Version));

        // Do not edit the line above!
        internal void AddSubject(string subject)
            => Put(PdfName.Subject, new PdfString(subject, TEXT_UNICODE));

        internal void AddTitle(string title)
            => Put(PdfName.Title, new PdfString(title, TEXT_UNICODE));
    }

    /// <summary>
    ///     PdfCatalog  is the PDF Catalog-object.
    ///     The Catalog is a dictionary that is the root node of the document. It contains a reference
    ///     to the tree of pages contained in the document, a reference to the tree of objects representing
    ///     the document's outline, a reference to the document's article threads, and the list of named
    ///     destinations. In addition, the Catalog indicates whether the document's outline or thumbnail
    ///     page images should be displayed automatically when the document is viewed and wether some location
    ///     other than the first page should be shown when the document is opened.
    ///     In this class however, only the reference to the tree of pages is implemented.
    ///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
    ///     section 6.2 (page 67-71)
    /// </summary>
    internal class PdfCatalog : PdfDictionary
    {
        internal readonly PdfWriter Writer;

        /// <summary>
        ///     constructors
        /// </summary>
        /// <summary>
        ///     Constructs a  PdfCatalog .
        /// </summary>
        /// <param name="pages">an indirect reference to the root of the document's Pages tree.</param>
        /// <param name="writer">the writer the catalog applies to</param>
        internal PdfCatalog(PdfIndirectReference pages, PdfWriter writer) : base(Catalog)
        {
            Writer = writer;
            Put(PdfName.Pages, pages);
        }

        /// <summary>
        ///     Sets the document level additional actions.
        /// </summary>
        internal PdfDictionary AdditionalActions
        {
            set => Put(PdfName.Aa, Writer.AddToBody(value).IndirectReference);
        }

        internal PdfAction OpenAction
        {
            set => Put(PdfName.Openaction, value);
        }

        /// <summary>
        ///     Adds the names of the named destinations to the catalog.
        /// </summary>
        internal void AddNames(OrderedTree localDestinations,
            INullValueDictionary<string, PdfObject> documentLevelJs,
            INullValueDictionary<string, PdfObject> documentFileAttachment,
            PdfWriter writer)
        {
            if (localDestinations.Count == 0 && documentLevelJs.Count == 0 && documentFileAttachment.Count == 0)
            {
                return;
            }

            var names = new PdfDictionary();

            if (localDestinations.Count > 0)
            {
                var ar = new PdfArray();

                foreach (string name in localDestinations.Keys)
                {
                    var obj = (object[])localDestinations[name];

                    if (obj[2] == null) //no destination
                    {
                        continue;
                    }

                    var refi = (PdfIndirectReference)obj[1];
                    ar.Add(new PdfString(name, null));
                    ar.Add(refi);
                }

                if (ar.Size > 0)
                {
                    var dests = new PdfDictionary();
                    dests.Put(PdfName.Names, ar);
                    names.Put(PdfName.Dests, writer.AddToBody(dests).IndirectReference);
                }
            }

            if (documentLevelJs.Count > 0)
            {
                var tree = PdfNameTree.WriteTree(documentLevelJs, writer);
                names.Put(PdfName.Javascript, writer.AddToBody(tree).IndirectReference);
            }

            if (documentFileAttachment.Count > 0)
            {
                names.Put(PdfName.Embeddedfiles,
                    writer.AddToBody(PdfNameTree.WriteTree(documentFileAttachment, writer)).IndirectReference);
            }

            if (names.Size > 0)
            {
                Put(PdfName.Names, writer.AddToBody(names).IndirectReference);
            }
        }
    }

    /// <summary>
    ///     [M4'] Adding a Table
    /// </summary>
    protected internal class RenderingContext
    {
        internal readonly INullValueDictionary<object, object> PageMap = new NullValueDictionary<object, object>();
        internal PdfContentByte CellGraphics;
        internal float LostTableBottom;
        internal float MaxCellBottom;
        internal float OldHeight = -1;
        internal float Pagetop = -1;

        /// <summary>
        ///     internal float maxCellHeight;
        /// </summary>
        internal INullValueDictionary<object, object> RowspanMap = new NullValueDictionary<object, object>();

        /// <summary>
        ///     A PdfPTable
        /// </summary>
        public PdfTable Table;

        public int CellRendered(PdfCell cell, int pageNumber)
        {
            var i = PageMap[cell];

            if (i == null)
            {
                i = 1;
            }
            else
            {
                i = (int)i + 1;
            }

            PageMap[cell] = i;

            var seti = (NullValueDictionary<object, object>)PageMap[pageNumber];

            if (seti == null)
            {
                seti = new NullValueDictionary<object, object>();
                PageMap[pageNumber] = seti;
            }

            seti[cell] = null;

            return (int)i;
        }

        /// <summary>
        ///     Consumes the rowspan
        /// </summary>
        /// <param name="c"></param>
        /// <returns>a rowspan.</returns>
        public int ConsumeRowspan(PdfCell c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            if (c.Rowspan == 1)
            {
                return 1;
            }

            var i = RowspanMap[c];

            if (i == null)
            {
                i = c.Rowspan;
            }

            i = (int)i - 1;
            RowspanMap[c] = i;

            if ((int)i < 1)
            {
                return 1;
            }

            return (int)i;
        }

        /// <summary>
        ///     Looks at the current rowspan.
        /// </summary>
        /// <param name="c"></param>
        /// <returns>the current rowspan</returns>
        public int CurrentRowspan(PdfCell c)
        {
            if (c == null)
            {
                throw new ArgumentNullException(nameof(c));
            }

            var i = RowspanMap[c];

            if (i == null)
            {
                return c.Rowspan;
            }

            return (int)i;
        }

        public bool IsCellRenderedOnPage(PdfCell cell, int pageNumber)
        {
            var seti = (NullValueDictionary<object, object>)PageMap[pageNumber];

            if (seti != null)
            {
                return seti.ContainsKey(cell);
            }

            return false;
        }

        public int NumCellRendered(PdfCell cell)
        {
            var i = PageMap[cell];

            if (i == null)
            {
                i = 0;
            }

            return (int)i;
        }
    }
}