using System.Reflection;

namespace iTextSharp.text;

/// <summary>
///     A generic Document class.
/// </summary>
/// <remarks>
///     All kinds of Text-elements can be added to a HTMLDocument.
///     The Document signals all the listeners when an element
///     has been added.
///     Once a document is created you can add some meta information.
///     You can also set the headers/footers.
///     You have to open the document before you can write content.
///     You can only write content (no more meta-formation!) once a document is opened.
///     When you change the header/footer on a certain page, this will be effective starting on the next page.
///     Ater closing the document, every listener (as well as its OutputStream) is closed too.
/// </remarks>
/// <example>
///     // creation of the document with a certain size and certain margins
///     Document document = new Document(PageSize.A4, 50, 50, 50, 50);
///     try {
///     // creation of the different writers
///     HtmlWriter.GetInstance( document , System.out);
///     PdfWriter.GetInstance( document , new FileOutputStream("text.pdf"));
///     // we add some meta information to the document
///     document.AddAuthor("Bruno Lowagie");
///     document.AddSubject("This is the result of a Test.");
///     // we define a header and a footer
///     HeaderFooter header = new HeaderFooter(new Phrase("This is a header."), false);
///     HeaderFooter footer = new HeaderFooter(new Phrase("This is page "), new Phrase("."));
///     footer.SetAlignment(Element.ALIGN_CENTER);
///     document.SetHeader(header);
///     document.SetFooter(footer);
///     // we open the document for writing
///     document.Open();
///     document.Add(new Paragraph("Hello world"));
///     }
///     catch (DocumentException de) {
///     Console.Error.WriteLine(de.Message);
///     }
///     document.Close();
/// </example>
public class Document : IDocListener
{
    ///<summary> Allows the pdf documents to be produced without compression for debugging purposes. </summary>
    public static readonly bool Compress = true;

    ///<summary> Scales the WMF font size. The default value is 0.86.  </summary>
    public static readonly float WmfFontCorrection = 0.86f;

    ///<summary> The IDocListener. </summary>
    private readonly List<IDocListener> _listeners = new();

    /// <summary>
    ///     This is a chapter number in case ChapterAutoNumber is used.
    /// </summary>
    protected int Chapternumber;

    ///<summary> This is the textual part of the footer </summary>
    protected HeaderFooter footer;

    ///<summary> This is the textual part of a Page; it can contain a header </summary>
    protected HeaderFooter header;

    ///<summary> Has the document allready been closed? </summary>
    protected bool IsDocumentClose;

    ///<summary> Is the document open or not? </summary>
    protected bool IsDocumentOpen;

    protected bool MarginMirroring;

    /// <summary>
    ///     mirroring of the top/bottom margins
    ///     @since	2.1.6
    /// </summary>
    protected bool MarginMirroringTopBottom;

    /// <summary>
    ///     membervariables concerning the layout
    /// </summary>
    /// <summary>
    ///     headers, footers
    /// </summary>
    /// <summary>
    ///     Constructs a new Document-object.
    /// </summary>
    /// <overloads>
    ///     Has three overloads.
    /// </overloads>
    public Document() : this(text.PageSize.A4)
    {
    }

    /// <summary>
    ///     Constructs a new Document-object.
    /// </summary>
    /// <param name="pageSize">the pageSize</param>
    public Document(Rectangle pageSize) : this(pageSize, 36, 36, 36, 36)
    {
    }

    /// <summary>
    ///     Constructs a new Document-object.
    /// </summary>
    /// <param name="pageSize">the pageSize</param>
    /// <param name="marginLeft">the margin on the left</param>
    /// <param name="marginRight">the margin on the right</param>
    /// <param name="marginTop">the margin on the top</param>
    /// <param name="marginBottom">the margin on the bottom</param>
    public Document(Rectangle pageSize, float marginLeft, float marginRight, float marginTop, float marginBottom)
    {
        PageSize = pageSize;
        LeftMargin = marginLeft;
        RightMargin = marginRight;
        TopMargin = marginTop;
        BottomMargin = marginBottom;
    }

    /// <summary>
    ///     listener methods
    /// </summary>
    /// <summary>
    ///     Gets the product name.
    /// </summary>
    public static string Product { get; } = "iTextSharp.LGPLv2.Core";

    /// <summary>
    ///     Gets the release number.
    /// </summary>
    public static string Release { get; } = typeof(Document).GetTypeInfo().Assembly.GetName().Version.ToString();

    /// <summary>
    ///     Returns the lower left y-coordinate.
    /// </summary>
    /// <value>the lower left y-coordinate.</value>
    public float Bottom => PageSize.GetBottom(BottomMargin);

    /// <summary>
    ///     Returns the bottom margin.
    /// </summary>
    /// <value>the bottom margin</value>
    public float BottomMargin { get; protected set; }

    /// <summary>
    ///     Gets the style class of the HTML body tag
    /// </summary>
    /// <value>the style class of the HTML body tag</value>
    public string HtmlStyleClass { get; set; }

    /// <summary>
    ///     Gets the JavaScript onLoad command.
    /// </summary>
    /// <value>the JavaScript onLoad command.</value>
    public string JavaScriptOnLoad { get; set; }

    /// <summary>
    ///     Gets the JavaScript onUnLoad command.
    /// </summary>
    /// <value>the JavaScript onUnLoad command</value>
    public string JavaScriptOnUnLoad { get; set; }

    /// <summary>
    ///     Returns the lower left x-coordinate.
    /// </summary>
    /// <value>the lower left x-coordinate</value>
    public float Left => PageSize.GetLeft(LeftMargin);

    /// <summary>
    ///     Returns the left margin.
    /// </summary>
    /// <value>the left margin</value>
    public float LeftMargin { get; protected set; }

    /// <summary>
    ///     Returns the current page number.
    /// </summary>
    /// <value>an int</value>
    public int PageNumber { get; protected set; }

    /// <summary>
    ///     Gets the pagesize.
    /// </summary>
    /// <value>the page size</value>
    public Rectangle PageSize { get; protected set; }

    /// <summary>
    ///     Returns the upper right x-coordinate.
    /// </summary>
    /// <value>the upper right x-coordinate.</value>
    public float Right => PageSize.GetRight(RightMargin);

    /// <summary>
    ///     methods to get the layout of the document.
    /// </summary>
    /// <summary>
    ///     Return the right margin.
    /// </summary>
    /// <value>the right margin</value>
    public float RightMargin { get; protected set; }

    /// <summary>
    ///     Returns the upper right y-coordinate.
    /// </summary>
    /// <value>the upper right y-coordinate.</value>
    public float Top => PageSize.GetTop(TopMargin);

    /// <summary>
    ///     Returns the top margin.
    /// </summary>
    /// <value>the top margin</value>
    public float TopMargin { get; protected set; }

    /// <summary>
    ///     Gets the iText version.
    /// </summary>
    /// <value>iText version</value>
    public static string Version => $"{Product} {Release}";

    /// <summary>
    ///     Changes the footer of this document.
    /// </summary>
    /// <value>a HeaderFooter</value>
    public virtual HeaderFooter Footer
    {
        set
        {
            footer = value;
            foreach (var listener in _listeners)
            {
                listener.Footer = value;
            }
        }
    }

    /// <summary>
    ///     Changes the header of this document.
    /// </summary>
    /// <value>a HeaderFooter</value>
    public virtual HeaderFooter Header
    {
        set
        {
            header = value;
            foreach (var listener in _listeners)
            {
                listener.Header = value;
            }
        }
    }

    /// <summary>
    ///     Sets the page number.
    /// </summary>
    /// <value>an int</value>
    public virtual int PageCount
    {
        set
        {
            PageNumber = value;
            foreach (var listener in _listeners)
            {
                listener.PageCount = value;
            }
        }
    }

    /// <summary>
    ///     Adds an Element to the Document.
    /// </summary>
    /// <param name="element">the Element to add</param>
    /// <returns>true if the element was added, false if not</returns>
    public virtual bool Add(IElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        if (IsDocumentClose)
        {
            throw new DocumentException("The document has been closed. You can't add any Elements.");
        }

        if (!IsDocumentOpen && element.IsContent())
        {
            throw new DocumentException("The document is not open yet; you can only add Meta information.");
        }

        var success = false;
        var number = element as ChapterAutoNumber;
        if (number != null)
        {
            Chapternumber = number.SetAutomaticNumber(Chapternumber);
        }

        foreach (var listener in _listeners)
        {
            success |= listener.Add(element);
        }

        var largeElement = element as ILargeElement;
        if (largeElement != null)
        {
            var e = largeElement;
            if (!e.ElementComplete)
            {
                e.FlushContent();
            }
        }

        return success;
    }

    /// <summary>
    ///     Closes the document.
    /// </summary>
    /// <remarks>
    ///     Once all the content has been written in the body, you have to close
    ///     the body. After that nothing can be written to the body anymore.
    /// </remarks>
    public virtual void Close()
    {
        if (!IsDocumentClose)
        {
            IsDocumentOpen = false;
            IsDocumentClose = true;
        }

        foreach (var listener in _listeners)
        {
            listener.Close();
        }
    }

    /// <summary>
    ///     Signals that an new page has to be started.
    /// </summary>
    /// <returns>true if the page was added, false if not.</returns>
    public virtual bool NewPage()
    {
        if (!IsDocumentOpen || IsDocumentClose)
        {
            return false;
        }

        foreach (var listener in _listeners)
        {
            listener.NewPage();
        }

        return true;
    }

    /// <summary>
    ///     methods implementing the IDocListener interface
    /// </summary>
    /// <summary>
    ///     Opens the document.
    /// </summary>
    /// <remarks>
    ///     Once the document is opened, you can't write any Header- or Meta-information
    ///     anymore. You have to open the document before you can begin to add content
    ///     to the body of the document.
    /// </remarks>
    public virtual void Open()
    {
        if (!IsDocumentClose)
        {
            IsDocumentOpen = true;
        }

        foreach (var listener in _listeners)
        {
            listener.SetPageSize(PageSize);
            listener.SetMargins(LeftMargin, RightMargin, TopMargin, BottomMargin);
            listener.Open();
        }
    }

    /// <summary>
    ///     Resets the footer of this document.
    /// </summary>
    public virtual void ResetFooter()
    {
        footer = null;
        foreach (var listener in _listeners)
        {
            listener.ResetFooter();
        }
    }

    /// <summary>
    ///     Resets the header of this document.
    /// </summary>
    public virtual void ResetHeader()
    {
        header = null;
        foreach (var listener in _listeners)
        {
            listener.ResetHeader();
        }
    }

    /// <summary>
    ///     Sets the page number to 0.
    /// </summary>
    public virtual void ResetPageCount()
    {
        PageNumber = 0;
        foreach (var listener in _listeners)
        {
            listener.ResetPageCount();
        }
    }

    /// <summary>
    ///     Set the margin mirroring. It will mirror right/left margins for odd/even pages.
    ///     Note: it will not work with {@link Table}.
    ///     true  to mirror the margins
    /// </summary>
    /// <param name="marginMirroring"></param>
    /// <returns>always  true </returns>
    public virtual bool SetMarginMirroring(bool marginMirroring)
    {
        MarginMirroring = marginMirroring;
        foreach (var listener in _listeners)
        {
            listener.SetMarginMirroring(marginMirroring);
        }

        return true;
    }

    /// <summary>
    ///     Set the margin mirroring. It will mirror top/bottom margins for odd/even pages.
    ///     Note: it will not work with {@link Table}.
    ///     true  to mirror the margins
    ///     @since	2.1.6
    /// </summary>
    /// <param name="marginMirroringTopBottom"></param>
    /// <returns>always  true </returns>
    public virtual bool SetMarginMirroringTopBottom(bool marginMirroringTopBottom)
    {
        MarginMirroringTopBottom = marginMirroringTopBottom;
        foreach (var listener in _listeners)
        {
            listener.SetMarginMirroringTopBottom(marginMirroringTopBottom);
        }

        return true;
    }

    /// <summary>
    ///     Sets the margins.
    /// </summary>
    /// <param name="marginLeft">the margin on the left</param>
    /// <param name="marginRight">the margin on the right</param>
    /// <param name="marginTop">the margin on the top</param>
    /// <param name="marginBottom">the margin on the bottom</param>
    /// <returns></returns>
    public virtual bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom)
    {
        LeftMargin = marginLeft;
        RightMargin = marginRight;
        TopMargin = marginTop;
        BottomMargin = marginBottom;
        foreach (var listener in _listeners)
        {
            listener.SetMargins(marginLeft, marginRight, marginTop, marginBottom);
        }

        return true;
    }

    /// <summary>
    ///     Sets the pagesize.
    /// </summary>
    /// <param name="pageSize">the new pagesize</param>
    /// <returns>a bool</returns>
    public virtual bool SetPageSize(Rectangle pageSize)
    {
        PageSize = pageSize;
        foreach (var listener in _listeners)
        {
            listener.SetPageSize(pageSize);
        }

        return true;
    }

    public void Dispose()
    {
        if (IsOpen())
        {
            Close();
        }
    }

    /// <summary>
    ///     Adds the author to a Document.
    /// </summary>
    /// <param name="author">the name of the author</param>
    /// <returns>true if successful, false otherwise</returns>
    public bool AddAuthor(string author) => Add(new Meta(Element.AUTHOR, author));

    /// <summary>
    ///     Adds the current date and time to a Document.
    /// </summary>
    /// <returns>true if successful, false otherwise</returns>
    public bool AddCreationDate() =>
        Add(new Meta(Element.CREATIONDATE,
                     DateTime.Now.ToString("ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture)));

    /// <summary>
    ///     Adds the creator to a Document.
    /// </summary>
    /// <param name="creator">the name of the creator</param>
    /// <returns>true if successful, false otherwise</returns>
    public bool AddCreator(string creator) => Add(new Meta(Element.CREATOR, creator));

    /// <summary>
    ///     Adds a IDocListener to the Document.
    /// </summary>
    /// <param name="listener">the new IDocListener</param>
    public void AddDocListener(IDocListener listener)
    {
        _listeners.Add(listener);
    }

    /// <summary>
    ///     Adds a user defined header to the document.
    /// </summary>
    /// <param name="name">the name of the header</param>
    /// <param name="content">the content of the header</param>
    /// <returns>true if successful, false otherwise</returns>
    public bool AddHeader(string name, string content) => Add(new Header(name, content));

    /// <summary>
    ///     Adds the keywords to a Document.
    /// </summary>
    /// <param name="keywords">keywords to add</param>
    /// <returns>true if successful, false otherwise</returns>
    public bool AddKeywords(string keywords) => Add(new Meta(Element.KEYWORDS, keywords));

    /// <summary>
    ///     Adds the producer to a Document.
    /// </summary>
    /// <returns>true if successful, false otherwise</returns>
    public bool AddProducer() => Add(new Meta(Element.PRODUCER, Version));

    /// <summary>
    ///     Adds the subject to a Document.
    /// </summary>
    /// <param name="subject">the subject</param>
    /// <returns>true if successful, false otherwise</returns>
    public bool AddSubject(string subject) => Add(new Meta(Element.SUBJECT, subject));

    /// <summary>
    ///     methods concerning the header or some meta information
    /// </summary>
    /// <summary>
    ///     Adds the title to a Document.
    /// </summary>
    /// <param name="title">the title</param>
    /// <returns>true if successful, false otherwise</returns>
    public bool AddTitle(string title) => Add(new Meta(Element.TITLE, title));

    /// <summary>
    ///     Returns the lower left y-coordinate, considering a given margin.
    /// </summary>
    /// <param name="margin">a margin</param>
    /// <returns>the lower left y-coordinate</returns>
    public float GetBottom(float margin) => PageSize.GetBottom(BottomMargin + margin);

    /// <summary>
    ///     Returns the lower left x-coordinate considering a given margin.
    /// </summary>
    /// <param name="margin">a margin</param>
    /// <returns>the lower left x-coordinate</returns>
    public float GetLeft(float margin) => PageSize.GetLeft(LeftMargin + margin);

    /// <summary>
    ///     Returns the upper right x-coordinate, considering a given margin.
    /// </summary>
    /// <param name="margin">a margin</param>
    /// <returns>the upper right x-coordinate</returns>
    public float GetRight(float margin) => PageSize.GetRight(RightMargin + margin);

    /// <summary>
    ///     Returns the upper right y-coordinate, considering a given margin.
    /// </summary>
    /// <param name="margin">a margin</param>
    /// <returns>the upper right y-coordinate</returns>
    public float GetTop(float margin) => PageSize.GetTop(TopMargin + margin);

    /// <summary>
    ///     Gets the margin mirroring flag.
    /// </summary>
    /// <returns>the margin mirroring flag</returns>
    public bool IsMarginMirroring() => MarginMirroring;

    /// <summary>
    ///     Checks if the document is open.
    /// </summary>
    /// <returns>true if the document is open</returns>
    public bool IsOpen() => IsDocumentOpen;

    /// <summary>
    ///     Removes a IDocListener from the Document.
    /// </summary>
    /// <param name="listener">the IDocListener that has to be removed.</param>
    public void RemoveIDocListener(IDocListener listener)
    {
        _listeners.Remove(listener);
    }
}