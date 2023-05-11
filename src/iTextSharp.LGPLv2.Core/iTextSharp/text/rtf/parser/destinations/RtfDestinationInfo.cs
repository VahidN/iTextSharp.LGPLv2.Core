using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.parser.ctrlwords;

namespace iTextSharp.text.rtf.parser.destinations;

/// <summary>
///     RtfDestinationInfo  handles data destined for the info destination
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfDestinationInfo : RtfDestination
{
    private string _elementName = "";
    private string _text = "";


    public RtfDestinationInfo() : base(null)
    {
    }

    /// <summary>
    ///     Constructs a new RtfDestinationInfo.
    /// </summary>
    public RtfDestinationInfo(RtfParser parser, string elementname) : base(parser)
    {
        SetToDefaults();
        _elementName = elementname;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
    /// </summary>
    public override bool CloseDestination() => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(char[])
    /// </summary>
    public override bool HandleCharacter(int ch)
    {
        _text += (char)ch;
        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
    /// </summary>
    public override bool HandleCloseGroup()
    {
        if (_text.Length > 0)
        {
            var doc = RtfParser.GetDocument();
            if (doc != null)
            {
                if (_elementName.Equals("author", StringComparison.Ordinal))
                {
                    doc.AddAuthor(_text);
                }

                if (_elementName.Equals("title", StringComparison.Ordinal))
                {
                    doc.AddTitle(_text);
                }

                if (_elementName.Equals("subject", StringComparison.Ordinal))
                {
                    doc.AddSubject(_text);
                }
            }
            else
            {
                var rtfDoc = RtfParser.GetRtfDocument();
                if (rtfDoc != null)
                {
                    if (_elementName.Equals("author", StringComparison.Ordinal))
                    {
                        var meta = new Meta(_elementName, _text);
                        var elem = new RtfInfoElement(rtfDoc, meta);
                        rtfDoc.GetDocumentHeader().AddInfoElement(elem);
                    }

                    if (_elementName.Equals("title", StringComparison.Ordinal))
                    {
                        var meta = new Meta(_elementName, _text);
                        var elem = new RtfInfoElement(rtfDoc, meta);
                        rtfDoc.GetDocumentHeader().AddInfoElement(elem);
                    }

                    if (_elementName.Equals("subject", StringComparison.Ordinal))
                    {
                        var meta = new Meta(_elementName, _text);
                        var elem = new RtfInfoElement(rtfDoc, meta);
                        rtfDoc.GetDocumentHeader().AddInfoElement(elem);
                    }
                }
            }

            SetToDefaults();
        }

        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see
    ///     com.lowagie.text.rtf.parser.destinations.RtfDestination#handleControlWord(com.lowagie.text.rtf.parser.ctrlwords.RtfCtrlWordData)
    /// </summary>
    public override bool HandleControlWord(RtfCtrlWordData ctrlWordData)
    {
        if (ctrlWordData == null)
        {
            throw new ArgumentNullException(nameof(ctrlWordData));
        }

        _elementName = ctrlWordData.CtrlWord;
        return true;
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
    /// </summary>
    public override bool HandleOpenGroup() => true;

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
    /// </summary>
    public override bool HandleOpeningSubGroup() => true;

    public void SetElementName(string value)
    {
        _elementName = value;
    }

    public override void SetParser(RtfParser parser)
    {
        RtfParser = parser;
        SetToDefaults();
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.parser.destinations.RtfDestination#setToDefaults()
    /// </summary>
    public override void SetToDefaults()
    {
        _text = "";
    }
}