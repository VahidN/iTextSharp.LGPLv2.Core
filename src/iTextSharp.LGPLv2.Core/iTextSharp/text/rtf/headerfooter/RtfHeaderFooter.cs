using iTextSharp.text.rtf.document;
using FD = iTextSharp.text.rtf.field;

namespace iTextSharp.text.rtf.headerfooter;

/// <summary>
///     The RtfHeaderFooter represents one header or footer. This class can be used
///     directly.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfHeaderFooter : HeaderFooter, IRtfBasicElement
{
    /// <summary>
    ///     Constant for displaying the header/footer on all pages
    /// </summary>
    public const int DISPLAY_ALL_PAGES = 1;

    /// <summary>
    ///     Constant for displaying the header/footer on the first page
    /// </summary>
    public const int DISPLAY_FIRST_PAGE = 0;

    /// <summary>
    ///     Constant for displaying the header/footer on all left hand pages
    /// </summary>
    public const int DISPLAY_LEFT_PAGES = 2;

    /// <summary>
    ///     Constant for displaying the header/footer on all right hand pages
    /// </summary>
    public const int DISPLAY_RIGHT_PAGES = 4;

    /// <summary>
    ///     Constant for the footer type
    /// </summary>
    public const int TYPE_FOOTER = 2;

    /// <summary>
    ///     Constant for the header type
    /// </summary>
    public const int TYPE_HEADER = 1;

    /// <summary>
    ///     Constant for a footer on all pages
    /// </summary>
    private static readonly byte[] _footerAll = DocWriter.GetIsoBytes("\\footer");

    /// <summary>
    ///     Constant for a footer on the first page
    /// </summary>
    private static readonly byte[] _footerFirst = DocWriter.GetIsoBytes("\\footerf");

    /// <summary>
    ///     Constnat for a footer on the left hand pages
    /// </summary>
    private static readonly byte[] _footerLeft = DocWriter.GetIsoBytes("\\footerl");

    /// <summary>
    ///     Constant for a footer on the right hand pages
    /// </summary>
    private static readonly byte[] _footerRight = DocWriter.GetIsoBytes("\\footerr");

    /// <summary>
    ///     Constant for a header on all pages
    /// </summary>
    private static readonly byte[] _headerAll = DocWriter.GetIsoBytes("\\header");

    /// <summary>
    ///     Constant for a header on the first page
    /// </summary>
    private static readonly byte[] _headerFirst = DocWriter.GetIsoBytes("\\headerf");

    /// <summary>
    ///     Constant for a header on all left hand pages
    /// </summary>
    private static readonly byte[] _headerLeft = DocWriter.GetIsoBytes("\\headerl");

    /// <summary>
    ///     Constant for a header on all right hand pages
    /// </summary>
    private static readonly byte[] _headerRight = DocWriter.GetIsoBytes("\\headerr");

    /// <summary>
    ///     The content of this RtfHeaderFooter
    /// </summary>
    private readonly object[] _content;

    /// <summary>
    ///     The display location of this RtfHeaderFooter. DISPLAY_FIRST_PAGE,
    ///     DISPLAY_LEFT_PAGES, DISPLAY_RIGHT_PAGES or DISPLAY_ALL_PAGES
    /// </summary>
    private int _displayAt = DISPLAY_ALL_PAGES;

    /// <summary>
    ///     The RtfDocument this RtfHeaderFooter belongs to
    /// </summary>
    private RtfDocument _document;

    /// <summary>
    ///     The display type of this RtfHeaderFooter. TYPE_HEADER or TYPE_FOOTER
    /// </summary>
    private int _type = TYPE_HEADER;

    /// <summary>
    ///     Constructs a RtfHeaderFooter for any Element.
    /// </summary>
    /// <param name="element">The Element to display as content of this RtfHeaderFooter</param>
    public RtfHeaderFooter(IElement element) : this(new[] { element })
    {
    }

    /// <summary>
    ///     Constructs a RtfHeaderFooter for an array of Elements.
    /// </summary>
    /// <param name="elements">The Elements to display as the content of this RtfHeaderFooter.</param>
    public RtfHeaderFooter(IElement[] elements) : base(new Phrase(""), false)
    {
        if (elements == null)
        {
            throw new ArgumentNullException(nameof(elements));
        }

        _content = new object[elements.Length];
        for (var i = 0; i < elements.Length; i++)
        {
            _content[i] = elements[i];
        }
    }

    /// <summary>
    ///     Constructs a RtfHeaderFooter based on a HeaderFooter with a certain type and displayAt
    ///     location. For internal use only.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfHeaderFooter belongs to</param>
    /// <param name="headerFooter">The HeaderFooter to base this RtfHeaderFooter on</param>
    /// <param name="type">The type of RtfHeaderFooter</param>
    /// <param name="displayAt">The display location of this RtfHeaderFooter</param>
    protected internal RtfHeaderFooter(RtfDocument doc, HeaderFooter headerFooter, int type, int displayAt) :
        base(new Phrase(""), false)
    {
        if (headerFooter == null)
        {
            throw new ArgumentNullException(nameof(headerFooter));
        }

        _document = doc;
        _type = type;
        _displayAt = displayAt;
        var par = new Paragraph();
        par.Alignment = headerFooter.Alignment;
        if (headerFooter.Before != null)
        {
            par.Add(headerFooter.Before);
        }

        if (headerFooter.IsNumbered())
        {
            par.Add(new FD.RtfPageNumber(_document));
        }

        if (headerFooter.After != null)
        {
            par.Add(headerFooter.After);
        }

        try
        {
            _content = new object[1];
            if (_document != null)
            {
                _content[0] = _document.GetMapper().MapElement(par)[0];
                ((IRtfBasicElement)_content[0]).SetInHeader(true);
            }
            else
            {
                _content[0] = par;
            }
        }
        catch (DocumentException)
        {
        }
    }

    /// <summary>
    ///     Constructs a RtfHeaderFooter as a copy of an existing RtfHeaderFooter.
    ///     For internal use only.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfHeaderFooter belongs to</param>
    /// <param name="headerFooter">The RtfHeaderFooter to copy</param>
    /// <param name="displayAt">The display location of this RtfHeaderFooter</param>
    protected internal RtfHeaderFooter(RtfDocument doc, RtfHeaderFooter headerFooter, int displayAt) :
        base(new Phrase(""), false)
    {
        if (headerFooter == null)
        {
            throw new ArgumentNullException(nameof(headerFooter));
        }

        _document = doc;
        _content = headerFooter.getContent();
        _displayAt = displayAt;
        for (var i = 0; i < _content.Length; i++)
        {
            if (_content[i] is IElement)
            {
                try
                {
                    _content[i] = _document.GetMapper().MapElement((IElement)_content[i])[0];
                }
                catch (DocumentException)
                {
                }
            }

            if (_content[i] is IRtfBasicElement)
            {
                ((IRtfBasicElement)_content[i]).SetInHeader(true);
            }
        }
    }

    /// <summary>
    ///     Constructs a RtfHeaderFooter for a HeaderFooter.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfHeaderFooter belongs to</param>
    /// <param name="headerFooter">The HeaderFooter to base this RtfHeaderFooter on</param>
    protected internal RtfHeaderFooter(RtfDocument doc, HeaderFooter headerFooter) : base(new Phrase(""), false)
    {
        if (headerFooter == null)
        {
            throw new ArgumentNullException(nameof(headerFooter));
        }

        _document = doc ?? throw new ArgumentNullException(nameof(doc));
        var par = new Paragraph();
        par.Alignment = headerFooter.Alignment;
        if (headerFooter.Before != null)
        {
            par.Add(headerFooter.Before);
        }

        if (headerFooter.IsNumbered())
        {
            par.Add(new FD.RtfPageNumber(_document));
        }

        if (headerFooter.After != null)
        {
            par.Add(headerFooter.After);
        }

        try
        {
            _content = new object[1];
            _content[0] = doc.GetMapper().MapElement(par)[0];
            ((IRtfBasicElement)_content[0]).SetInHeader(true);
        }
        catch (DocumentException)
        {
        }
    }

    /// <summary>
    ///     Unused
    /// </summary>
    /// <param name="inHeader"></param>
    public void SetInHeader(bool inHeader)
    {
    }

    /// <summary>
    ///     Unused
    /// </summary>
    /// <param name="inTable"></param>
    public void SetInTable(bool inTable)
    {
    }

    /// <summary>
    ///     Sets the RtfDocument this RtfElement belongs to
    /// </summary>
    /// <param name="doc">The RtfDocument to use</param>
    public void SetRtfDocument(RtfDocument doc)
    {
        _document = doc;
        if (_document != null)
        {
            for (var i = 0; i < _content.Length; i++)
            {
                try
                {
                    if (_content[i] is Element)
                    {
                        _content[i] = _document.GetMapper().MapElement((IElement)_content[i])[0];
                        ((IRtfBasicElement)_content[i]).SetInHeader(true);
                    }
                    else if (_content[i] is IRtfBasicElement)
                    {
                        ((IRtfBasicElement)_content[i]).SetRtfDocument(_document);
                        ((IRtfBasicElement)_content[i]).SetInHeader(true);
                    }
                }
                catch (DocumentException)
                {
                }
            }
        }
    }

    /// <summary>
    ///     Write the content of this RtfHeaderFooter.
    /// </summary>
    public virtual void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(RtfElement.OpenGroup, 0, RtfElement.OpenGroup.Length);
        if (_type == TYPE_HEADER)
        {
            if (_displayAt == DISPLAY_ALL_PAGES)
            {
                outp.Write(_headerAll, 0, _headerAll.Length);
            }
            else if (_displayAt == DISPLAY_FIRST_PAGE)
            {
                outp.Write(_headerFirst, 0, _headerFirst.Length);
            }
            else if (_displayAt == DISPLAY_LEFT_PAGES)
            {
                outp.Write(_headerLeft, 0, _headerLeft.Length);
            }
            else if (_displayAt == DISPLAY_RIGHT_PAGES)
            {
                outp.Write(_headerRight, 0, _headerRight.Length);
            }
        }
        else
        {
            if (_displayAt == DISPLAY_ALL_PAGES)
            {
                outp.Write(_footerAll, 0, _footerAll.Length);
            }
            else if (_displayAt == DISPLAY_FIRST_PAGE)
            {
                outp.Write(_footerFirst, 0, _footerFirst.Length);
            }
            else if (_displayAt == DISPLAY_LEFT_PAGES)
            {
                outp.Write(_footerLeft, 0, _footerLeft.Length);
            }
            else if (_displayAt == DISPLAY_RIGHT_PAGES)
            {
                outp.Write(_footerRight, 0, _footerRight.Length);
            }
        }

        outp.Write(RtfElement.Delimiter, 0, RtfElement.Delimiter.Length);
        for (var i = 0; i < _content.Length; i++)
        {
            if (_content[i] is IRtfBasicElement)
            {
                var rbe = (IRtfBasicElement)_content[i];
                rbe.WriteContent(outp);
            }
        }

        outp.Write(RtfElement.CloseGroup, 0, RtfElement.CloseGroup.Length);
    }

    /// <summary>
    ///     Set the alignment of this RtfHeaderFooter. Passes the setting
    ///     on to the contained element.
    /// </summary>
    public void SetAlignment(int alignment)
    {
        Alignment = alignment;
        for (var i = 0; i < _content.Length; i++)
        {
            if (_content[i] is Paragraph)
            {
                ((Paragraph)_content[i]).Alignment = alignment;
            }
            else if (_content[i] is Table)
            {
                ((Table)_content[i]).Alignment = alignment;
            }
            else if (_content[i] is Image)
            {
                ((Image)_content[i]).Alignment = alignment;
            }
        }
    }

    /// <summary>
    ///     Sets the display location of this RtfHeaderFooter
    /// </summary>
    /// <param name="displayAt">The display location to use.</param>
    public void SetDisplayAt(int displayAt)
    {
        _displayAt = displayAt;
    }

    /// <summary>
    ///     Sets the type of this RtfHeaderFooter
    /// </summary>
    /// <param name="type">The type to use.</param>
    public void SetType(int type)
    {
        _type = type;
    }

    /// <summary>
    ///     Gets the content of this RtfHeaderFooter
    /// </summary>
    /// <returns>The content of this RtfHeaderFooter</returns>
    private object[] getContent() => _content;
}