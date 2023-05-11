namespace iTextSharp.text.rtf.document;

/// <summary>
///     Stores one information group element. Valid elements are
///     author, title, subject, keywords, producer and creationdate.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Thomas Bickel (tmb99@inode.at)
/// </summary>
public class RtfInfoElement : RtfElement
{
    /// <summary>
    ///     Constant for the author element
    /// </summary>
    private static readonly byte[] _infoAuthor = DocWriter.GetIsoBytes("\\author");

    /// <summary>
    ///     Constant for the creationdate element
    /// </summary>
    private static readonly byte[] _infoCreationDate = DocWriter.GetIsoBytes("\\creationdate");

    /// <summary>
    ///     Constant for the keywords element
    /// </summary>
    private static readonly byte[] _infoKeywords = DocWriter.GetIsoBytes("\\keywords");

    /// <summary>
    ///     Constant for the producer element
    /// </summary>
    private static readonly byte[] _infoProducer = DocWriter.GetIsoBytes("\\operator");

    /// <summary>
    ///     Constant for the subject element
    /// </summary>
    private static readonly byte[] _infoSubject = DocWriter.GetIsoBytes("\\subject");

    /// <summary>
    ///     Constant for the title element
    /// </summary>
    private static readonly byte[] _infoTitle = DocWriter.GetIsoBytes("\\title");

    /// <summary>
    ///     The content of this RtfInfoElement
    /// </summary>
    private readonly string _content = "";

    /// <summary>
    ///     The type of this RtfInfoElement. The values from Element.INFO_ELEMENT_NAME are used.
    /// </summary>
    private readonly int _infoType = -1;

    /// <summary>
    ///     Constructs a RtfInfoElement based on the given Meta object
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfInfoElement belongs to</param>
    /// <param name="meta">The Meta object this RtfInfoElement is based on</param>
    public RtfInfoElement(RtfDocument doc, Meta meta) : base(doc)
    {
        if (meta == null)
        {
            throw new ArgumentNullException(nameof(meta));
        }

        _infoType = meta.Type;
        _content = meta.Content;
    }

    /// <summary>
    ///     Writes the content of one RTF information element.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(OpenGroup, 0, OpenGroup.Length);
        switch (_infoType)
        {
            case Element.AUTHOR:
                outp.Write(_infoAuthor, 0, _infoAuthor.Length);
                break;
            case Element.SUBJECT:
                outp.Write(_infoSubject, 0, _infoSubject.Length);
                break;
            case Element.KEYWORDS:
                outp.Write(_infoKeywords, 0, _infoKeywords.Length);
                break;
            case Element.TITLE:
                outp.Write(_infoTitle, 0, _infoTitle.Length);
                break;
            case Element.PRODUCER:
                outp.Write(_infoProducer, 0, _infoProducer.Length);
                break;
            case Element.CREATIONDATE:
                outp.Write(_infoCreationDate, 0, _infoCreationDate.Length);
                break;
            default:
                outp.Write(_infoAuthor, 0, _infoAuthor.Length);
                break;
        }

        outp.Write(Delimiter, 0, Delimiter.Length);
        byte[] t;
        if (_infoType == Element.CREATIONDATE)
        {
            t = DocWriter.GetIsoBytes(convertDate(_content));
            outp.Write(t, 0, t.Length);
        }
        else
        {
            Document.FilterSpecialChar(outp, _content, false, false);
        }

        outp.Write(CloseGroup, 0, CloseGroup.Length);
    }

    /// <summary>
    ///     Converts a date from the format used by iText to the format required by
    ///     rtf. iText: EEE MMM dd HH:mm:ss zzz yyyy - rtf: \\'yr'yyyy\\'mo'MM\\'dy'dd\\'hr'HH\\'min'mm\\'sec'ss
    /// </summary>
    /// <param name="date">The date formated by iText</param>
    /// <returns>The date formated for rtf</returns>
    private static string convertDate(string date)
    {
        DateTime d;
        try
        {
            d = DateTime.Parse(date, CultureInfo.InvariantCulture);
        }
        catch
        {
            d = DateTime.Now;
        }

        return d.ToString("'\\\\yr'yyyy'\\\\mo'MM'\\\\dy'dd'\\\\hr'HH'\\\\min'mm'\\\\sec'ss",
                          CultureInfo.InvariantCulture);
    }
}