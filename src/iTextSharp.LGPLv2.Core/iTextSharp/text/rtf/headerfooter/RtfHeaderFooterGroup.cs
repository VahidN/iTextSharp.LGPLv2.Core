using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.headerfooter;

/// <summary>
///     The RtfHeaderFooterGroup holds 0 - 3 RtfHeaderFooters that create a group
///     of headers or footers.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfHeaderFooterGroup : HeaderFooter, IRtfBasicElement
{
    /// <summary>
    ///     This RtfHeaderFooterGroup contains two or three RtfHeaderFooter objects
    /// </summary>
    private const int ModeMultiple = 2;

    /// <summary>
    ///     This RtfHeaderFooterGroup contains no RtfHeaderFooter objects
    /// </summary>
    private const int ModeNone = 0;

    /// <summary>
    ///     This RtfHeaderFooterGroup contains one RtfHeaderFooter object
    /// </summary>
    private const int ModeSingle = 1;

    /// <summary>
    ///     The RtfDocument this RtfHeaderFooterGroup belongs to
    /// </summary>
    private RtfDocument _document;

    /// <summary>
    ///     The RtfHeaderFooter for all pages
    /// </summary>
    private RtfHeaderFooter _headerAll;

    /// <summary>
    ///     The RtfHeaderFooter for the first page
    /// </summary>
    private RtfHeaderFooter _headerFirst;

    /// <summary>
    ///     The RtfHeaderFooter for the left hand pages
    /// </summary>
    private RtfHeaderFooter _headerLeft;

    /// <summary>
    ///     The RtfHeaderFooter for the right hand pages
    /// </summary>
    private RtfHeaderFooter _headerRight;

    /// <summary>
    ///     The current mode of this RtfHeaderFooterGroup. Defaults to MODE_NONE
    /// </summary>
    private int _mode = ModeNone;

    /// <summary>
    ///     The current type of this RtfHeaderFooterGroup. Defaults to RtfHeaderFooter.TYPE_HEADER
    /// </summary>
    private int _type = RtfHeaderFooter.TYPE_HEADER;

    /// <summary>
    ///     Constructs a RtfHeaderGroup to which you add headers/footers using
    ///     via the setHeaderFooter method.
    /// </summary>
    public RtfHeaderFooterGroup() : base(new Phrase(""), false) => _mode = ModeNone;

    /// <summary>
    ///     Constructs a certain type of RtfHeaderFooterGroup. RtfHeaderFooter.TYPE_HEADER
    ///     and RtfHeaderFooter.TYPE_FOOTER are valid values for type.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfHeaderFooter belongs to</param>
    /// <param name="type">The type of RtfHeaderFooterGroup to create</param>
    public RtfHeaderFooterGroup(RtfDocument doc, int type) : base(new Phrase(""), false)
    {
        _document = doc;
        _type = type;
    }

    /// <summary>
    ///     Constructs a RtfHeaderFooterGroup by copying the content of the original
    ///     RtfHeaderFooterGroup
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfHeaderFooter belongs to</param>
    /// <param name="headerFooter">The RtfHeaderFooterGroup to copy</param>
    /// <param name="type">The type of RtfHeaderFooterGroup to create</param>
    public RtfHeaderFooterGroup(RtfDocument doc, RtfHeaderFooterGroup headerFooter, int type) :
        base(new Phrase(""), false)
    {
        if (headerFooter == null)
        {
            throw new ArgumentNullException(nameof(headerFooter));
        }

        _document = doc;
        _mode = headerFooter.GetMode();
        _type = type;
        if (headerFooter.GetHeaderAll() != null)
        {
            _headerAll = new RtfHeaderFooter(_document, headerFooter.GetHeaderAll(), RtfHeaderFooter.DISPLAY_ALL_PAGES);
        }

        if (headerFooter.GetHeaderFirst() != null)
        {
            _headerFirst = new RtfHeaderFooter(_document, headerFooter.GetHeaderFirst(),
                                               RtfHeaderFooter.DISPLAY_FIRST_PAGE);
        }

        if (headerFooter.GetHeaderLeft() != null)
        {
            _headerLeft = new RtfHeaderFooter(_document, headerFooter.GetHeaderLeft(),
                                              RtfHeaderFooter.DISPLAY_LEFT_PAGES);
        }

        if (headerFooter.GetHeaderRight() != null)
        {
            _headerRight = new RtfHeaderFooter(_document, headerFooter.GetHeaderRight(),
                                               RtfHeaderFooter.DISPLAY_RIGHT_PAGES);
        }

        SetType(_type);
    }

    /// <summary>
    ///     Constructs a RtfHeaderFooterGroup for a certain RtfHeaderFooter.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfHeaderFooter belongs to</param>
    /// <param name="headerFooter">The RtfHeaderFooter to display</param>
    /// <param name="type">The typ of RtfHeaderFooterGroup to create</param>
    public RtfHeaderFooterGroup(RtfDocument doc, RtfHeaderFooter headerFooter, int type) : base(new Phrase(""), false)
    {
        _document = doc;
        _type = type;
        _mode = ModeSingle;
        _headerAll = new RtfHeaderFooter(doc, headerFooter, RtfHeaderFooter.DISPLAY_ALL_PAGES);
        _headerAll.SetType(_type);
    }

    /// <summary>
    ///     Constructs a RtfHeaderGroup for a certain HeaderFooter
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfHeaderFooter belongs to</param>
    /// <param name="headerFooter">The HeaderFooter to display</param>
    /// <param name="type">The typ of RtfHeaderFooterGroup to create</param>
    public RtfHeaderFooterGroup(RtfDocument doc, HeaderFooter headerFooter, int type) : base(new Phrase(""), false)
    {
        _document = doc;
        _type = type;
        _mode = ModeSingle;
        _headerAll = new RtfHeaderFooter(doc, headerFooter, type, RtfHeaderFooter.DISPLAY_ALL_PAGES);
        _headerAll.SetType(_type);
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
        if (_headerAll != null)
        {
            _headerAll.SetRtfDocument(_document);
        }

        if (_headerFirst != null)
        {
            _headerFirst.SetRtfDocument(_document);
        }

        if (_headerLeft != null)
        {
            _headerLeft.SetRtfDocument(_document);
        }

        if (_headerRight != null)
        {
            _headerRight.SetRtfDocument(_document);
        }
    }

    /// <summary>
    ///     Write the content of this RtfHeaderFooterGroup.
    /// </summary>
    public virtual void WriteContent(Stream outp)
    {
        if (_mode == ModeSingle)
        {
            _headerAll.WriteContent(outp);
        }
        else if (_mode == ModeMultiple)
        {
            if (_headerFirst != null)
            {
                _headerFirst.WriteContent(outp);
            }

            if (_headerLeft != null)
            {
                _headerLeft.WriteContent(outp);
            }

            if (_headerRight != null)
            {
                _headerRight.WriteContent(outp);
            }

            if (_headerAll != null)
            {
                _headerAll.WriteContent(outp);
            }
        }
    }

    /// <summary>
    ///     Get whether this RtfHeaderFooterGroup has facing pages
    /// </summary>
    /// <returns>Whether this RtfHeaderFooterGroup has facing pages</returns>
    public bool HasFacingPages() => _headerLeft != null || _headerRight != null;

    /// <summary>
    ///     Get whether this RtfHeaderFooterGroup has a titlepage
    /// </summary>
    /// <returns>Whether this RtfHeaderFooterGroup has a titlepage</returns>
    public bool HasTitlePage() => _headerFirst != null;

    /// <summary>
    ///     Set that this RtfHeaderFooterGroup should have facing pages. If only
    ///     a header / footer for all pages exists, then it will be copied to the left
    ///     and right pages aswell.
    /// </summary>
    public void SetHasFacingPages()
    {
        if (_mode == ModeSingle)
        {
            _mode = ModeMultiple;
            _headerLeft = new RtfHeaderFooter(_document, _headerAll, RtfHeaderFooter.DISPLAY_LEFT_PAGES);
            _headerLeft.SetType(_type);
            _headerRight = new RtfHeaderFooter(_document, _headerAll, RtfHeaderFooter.DISPLAY_RIGHT_PAGES);
            _headerRight.SetType(_type);
            _headerAll = null;
        }
        else if (_mode == ModeMultiple)
        {
            if (_headerLeft == null && _headerAll != null)
            {
                _headerLeft = new RtfHeaderFooter(_document, _headerAll, RtfHeaderFooter.DISPLAY_LEFT_PAGES);
                _headerLeft.SetType(_type);
            }

            if (_headerRight == null && _headerAll != null)
            {
                _headerRight = new RtfHeaderFooter(_document, _headerAll, RtfHeaderFooter.DISPLAY_RIGHT_PAGES);
                _headerRight.SetType(_type);
            }

            _headerAll = null;
        }
    }

    /// <summary>
    ///     Set that this RtfHeaderFooterGroup should have a title page. If only
    ///     a header / footer for all pages exists, then it will be copied to the
    ///     first page aswell.
    /// </summary>
    public void SetHasTitlePage()
    {
        if (_mode == ModeSingle)
        {
            _mode = ModeMultiple;
            _headerFirst = new RtfHeaderFooter(_document, _headerAll, RtfHeaderFooter.DISPLAY_FIRST_PAGE);
            _headerFirst.SetType(_type);
        }
    }

    /// <summary>
    ///     Set a RtfHeaderFooter to be displayed at a certain position
    /// </summary>
    /// <param name="headerFooter">The RtfHeaderFooter to display</param>
    /// <param name="displayAt">The display location to use</param>
    public void SetHeaderFooter(RtfHeaderFooter headerFooter, int displayAt)
    {
        if (headerFooter == null)
        {
            throw new ArgumentNullException(nameof(headerFooter));
        }

        _mode = ModeMultiple;
        headerFooter.SetRtfDocument(_document);
        headerFooter.SetType(_type);
        headerFooter.SetDisplayAt(displayAt);
        switch (displayAt)
        {
            case RtfHeaderFooter.DISPLAY_ALL_PAGES:
                _headerAll = headerFooter;
                break;
            case RtfHeaderFooter.DISPLAY_FIRST_PAGE:
                _headerFirst = headerFooter;
                break;
            case RtfHeaderFooter.DISPLAY_LEFT_PAGES:
                _headerLeft = headerFooter;
                break;
            case RtfHeaderFooter.DISPLAY_RIGHT_PAGES:
                _headerRight = headerFooter;
                break;
        }
    }

    /// <summary>
    ///     Set a HeaderFooter to be displayed at a certain position
    /// </summary>
    /// <param name="headerFooter">The HeaderFooter to set</param>
    /// <param name="displayAt">The display location to use</param>
    public void SetHeaderFooter(HeaderFooter headerFooter, int displayAt)
    {
        _mode = ModeMultiple;
        switch (displayAt)
        {
            case RtfHeaderFooter.DISPLAY_ALL_PAGES:
                _headerAll = new RtfHeaderFooter(_document, headerFooter, _type, displayAt);
                break;
            case RtfHeaderFooter.DISPLAY_FIRST_PAGE:
                _headerFirst = new RtfHeaderFooter(_document, headerFooter, _type, displayAt);
                break;
            case RtfHeaderFooter.DISPLAY_LEFT_PAGES:
                _headerLeft = new RtfHeaderFooter(_document, headerFooter, _type, displayAt);
                break;
            case RtfHeaderFooter.DISPLAY_RIGHT_PAGES:
                _headerRight = new RtfHeaderFooter(_document, headerFooter, _type, displayAt);
                break;
        }
    }

    /// <summary>
    ///     Set the type of this RtfHeaderFooterGroup. RtfHeaderFooter.TYPE_HEADER
    ///     or RtfHeaderFooter.TYPE_FOOTER. Also sets the type for all RtfHeaderFooters
    ///     of this RtfHeaderFooterGroup.
    /// </summary>
    /// <param name="type">The type to use</param>
    public void SetType(int type)
    {
        _type = type;
        if (_headerAll != null)
        {
            _headerAll.SetType(_type);
        }

        if (_headerFirst != null)
        {
            _headerFirst.SetType(_type);
        }

        if (_headerLeft != null)
        {
            _headerLeft.SetType(_type);
        }

        if (_headerRight != null)
        {
            _headerRight.SetType(_type);
        }
    }

    /// <summary>
    ///     Gets the RtfHeaderFooter for all pages
    /// </summary>
    /// <returns>The RtfHeaderFooter for all pages</returns>
    protected RtfHeaderFooter GetHeaderAll() => _headerAll;

    /// <summary>
    ///     Gets the RtfHeaderFooter for the title page
    /// </summary>
    /// <returns>The RtfHeaderFooter for the title page</returns>
    protected RtfHeaderFooter GetHeaderFirst() => _headerFirst;

    /// <summary>
    ///     Gets the RtfHeaderFooter for all left hand pages
    /// </summary>
    /// <returns>The RtfHeaderFooter for all left hand pages</returns>
    protected RtfHeaderFooter GetHeaderLeft() => _headerLeft;

    /// <summary>
    ///     Gets the RtfHeaderFooter for all right hand pages
    /// </summary>
    /// <returns>The RtfHeaderFooter for all right hand pages</returns>
    protected RtfHeaderFooter GetHeaderRight() => _headerRight;

    /// <summary>
    ///     Gets the mode of this RtfHeaderFooterGroup
    /// </summary>
    /// <returns>The mode of this RtfHeaderFooterGroup</returns>
    protected int GetMode() => _mode;
}