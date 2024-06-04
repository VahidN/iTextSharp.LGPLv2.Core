using iTextSharp.text.rtf.document.output;
using iTextSharp.text.rtf.graphic;

namespace iTextSharp.text.rtf.document;

/// <summary>
///     The RtfDocument stores all document related data and also the main data stream.
///     INTERNAL CLASS - NOT TO BE USED DIRECTLY
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Todd Bush (Todd.Bush@canopysystems.com) [Tab support]
/// </summary>
public class RtfDocument : RtfElement
{
    private static readonly byte[] _fscBackslash = DocWriter.GetIsoBytes("\\");

    private static readonly byte[] _fscHexPrefix = DocWriter.GetIsoBytes("\\\'");

    private static readonly byte[] _fscLine = DocWriter.GetIsoBytes("\\line ");

    private static readonly byte[] _fscNewpage = DocWriter.GetIsoBytes("$newpage$");

    private static readonly byte[] _fscPagePar = DocWriter.GetIsoBytes("\\page\\par ");

    private static readonly byte[] _fscPar = DocWriter.GetIsoBytes("\\par ");

    private static readonly byte[] _fscTab = DocWriter.GetIsoBytes("\\tab ");

    private static readonly byte[] _fscUniPrefix = DocWriter.GetIsoBytes("\\u");

    private static readonly Random _random = new();

    /// <summary>
    ///     Constant for the Rtf document start
    /// </summary>
    private static readonly byte[] _rtfDocument = DocWriter.GetIsoBytes("\\rtf1");

    /// <summary>
    ///     The RtfDocumentHeader that handles all document header methods
    /// </summary>
    private readonly RtfDocumentHeader _documentHeader;

    /// <summary>
    ///     The RtfDocumentSettings for this RtfDocument.
    /// </summary>
    private readonly RtfDocumentSettings _documentSettings;

    /// <summary>
    ///     The RtfMapper to use in this RtfDocument
    /// </summary>
    private readonly RtfMapper _mapper;

    /// <summary>
    ///     Stores integers that have been generated as unique random numbers
    /// </summary>
    private readonly List<int> _previousRandomInts;

    /// <summary>
    ///     Whether to automatically generate TOC entries for Chapters and Sections. Defaults to false
    /// </summary>
    private bool _autogenerateTocEntries;

    /// <summary>
    ///     Stores the actual document data
    /// </summary>
    private IRtfDataCache _data;

    /// <summary>
    ///     The last RtfBasicElement that was added directly to the RtfDocument.
    /// </summary>
    private IRtfBasicElement _lastElementWritten;

    /// <summary>
    ///     The default constructor for a RtfDocument
    /// </summary>
    public RtfDocument() : base(null)
    {
        _data = new RtfMemoryCache();
        _mapper = new RtfMapper(this);
        _documentHeader = new RtfDocumentHeader(this);
        _documentHeader.Init();
        _previousRandomInts = new List<int>();
        _documentSettings = new RtfDocumentSettings(this);
    }

    /// <summary>
    ///     Adds an element to the rtf document
    /// </summary>
    /// <param name="element">The element to add</param>
    public void Add(IRtfBasicElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        try
        {
            if (element is RtfInfoElement)
            {
                _documentHeader.AddInfoElement((RtfInfoElement)element);
            }
            else
            {
                if (element is RtfImage)
                {
                    ((RtfImage)element).SetTopLevelElement(true);
                }

                element.WriteContent(_data.GetOutputStream());
                _lastElementWritten = element;
            }
        }
        catch (IOException)
        {
        }
    }

    /// <summary>
    ///     Writes the given string to the given {@link Stream} encoding the string characters.
    ///     @throws IOException
    /// </summary>
    /// <param name="outp">destination Stream</param>
    /// <param name="str">string to write</param>
    /// <param name="useHex">if  true  hex encoding characters is preferred to unicode encoding if possible</param>
    /// <param name="softLineBreaks">if  true  return characters are written as soft line breaks</param>
    public void FilterSpecialChar(Stream outp, string str, bool useHex, bool softLineBreaks)
    {
        if (outp == null)
        {
            throw new ArgumentException("null OutpuStream");
        }

        var alwaysUseUniCode = _documentSettings.IsAlwaysUseUnicode();
        if (str == null)
        {
            return;
        }

        var len = str.Length;
        if (len == 0)
        {
            return;
        }

        byte[] t = null;
        for (var k = 0; k < len; k++)
        {
            var c = str[k];
            if (c < 0x20)
            {
                //allow return and tab only
                if (c == '\n')
                {
                    outp.Write(t = softLineBreaks ? _fscLine : _fscPar, 0, t.Length);
                }
                else if (c == '\t')
                {
                    outp.Write(_fscTab, 0, _fscTab.Length);
                }
                else
                {
                    outp.WriteByte((byte)'?');
                }
            }
            else if (c == '\\' || c == '{' || c == '}')
            {
                //escape
                outp.Write(_fscBackslash, 0, _fscBackslash.Length);
                outp.WriteByte((byte)c);
            }
            else if (c == '$' && len - k >= _fscNewpage.Length && subMatch(str, k, _fscNewpage))
            {
                outp.Write(_fscPagePar, 0, _fscPagePar.Length);
                k += _fscNewpage.Length - 1;
            }
            else
            {
                if (c > 0xff || (c > 'z' && alwaysUseUniCode))
                {
                    if (useHex && c <= 0xff)
                    {
                        //encode as 2 char hex string
                        outp.Write(_fscHexPrefix, 0, _fscHexPrefix.Length);
                        outp.Write(RtfImage.Byte2CharLut, c * 2, 2);
                    }
                    else
                    {
                        //encode as decimal, signed short value
                        outp.Write(_fscUniPrefix, 0, _fscUniPrefix.Length);
                        var s = ((short)c).ToString(CultureInfo.InvariantCulture);
                        for (var x = 0; x < s.Length; x++)
                        {
                            outp.WriteByte((byte)s[x]);
                        }

                        outp.WriteByte((byte)'?');
                    }
                }
                else
                {
                    outp.WriteByte((byte)c);
                }
            }
        }
    }

    /// <summary>
    ///     Get whether to autmatically generate table of contents entries
    /// </summary>
    /// <returns>Wheter to automatically generate TOC entries</returns>
    public bool GetAutogenerateTocEntries() => _autogenerateTocEntries;

    /// <summary>
    ///     Gets the RtfDocumentHeader of this RtfDocument
    /// </summary>
    /// <returns>The RtfDocumentHeader of this RtfDocument</returns>
    public RtfDocumentHeader GetDocumentHeader() => _documentHeader;

    /// <summary>
    ///     Gets the RtfDocumentSettings that specify how the rtf document is generated.
    /// </summary>
    /// <returns>The current RtfDocumentSettings.</returns>
    public RtfDocumentSettings GetDocumentSettings() => _documentSettings;

    /// <summary>
    ///     Gets the last RtfBasicElement that was directly added to the RtfDocument.
    /// </summary>
    /// <returns>The last RtfBasicElement that was directly added to the RtfDocument.</returns>
    public IRtfBasicElement GetLastElementWritten() => _lastElementWritten;

    /// <summary>
    ///     Gets the RtfMapper object of this RtfDocument
    /// </summary>
    /// <returns>The RtfMapper</returns>
    public RtfMapper GetMapper() => _mapper;

    /// <summary>
    ///     Generates a random integer that is unique with respect to the document.
    /// </summary>
    /// <returns>A random int</returns>
    public int GetRandomInt()
    {
        int newInt;
        do
        {
            lock (_random)
            {
                newInt = _random.Next(int.MaxValue - 2);
            }
        } while (_previousRandomInts.Contains(newInt));

        _previousRandomInts.Add(newInt);
        return newInt;
    }

    /// <summary>
    ///     Opens the RtfDocument and initialises the data cache. If the data cache is
    ///     set to CACHE_DISK, but the cache cannot be initialised then the memory cache
    ///     is used.
    /// </summary>
    public void Open()
    {
        try
        {
            switch (_documentSettings.GetDataCacheStyle())
            {
                case RtfDataCache.CACHE_MEMORY_EFFICIENT:
                    _data = new RtfEfficientMemoryCache();
                    break;
                case RtfDataCache.CACHE_MEMORY:
                    _data = new RtfMemoryCache();
                    break;
                case RtfDataCache.CACHE_DISK:
                    _data = new RtfDiskCache();
                    break;
                default:
                    throw new ArgumentException("unknown");
            }
        }
        catch (IOException)
        {
            _data = new RtfMemoryCache();
        }
    }

    /// <summary>
    ///     Helper method outputs linebreak in document if debugging is turned on.
    ///     @throws IOException
    ///     @since 2.1.3
    /// </summary>
    /// <param name="result">the OutputStream to write the linebreak to.</param>
    public void OutputDebugLinebreak(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (GetDocumentSettings().IsOutputDebugLineBreaks())
        {
            result.WriteByte((byte)'\n');
        }
    }

    /// <summary>
    ///     Whether to automagically generate table of contents entries when
    ///     adding Chapters or Sections.
    /// </summary>
    /// <param name="autogenerate">Whether to automatically generate TOC entries</param>
    public void SetAutogenerateTocEntries(bool autogenerate)
    {
        _autogenerateTocEntries = autogenerate;
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the document
    /// </summary>
    /// <param name="outs">The  Stream  to write the RTF document to.</param>
    public void WriteDocument(Stream outs)
    {
        if (outs == null)
        {
            throw new ArgumentNullException(nameof(outs));
        }

        try
        {
            outs.Write(OpenGroup, 0, OpenGroup.Length);
            outs.Write(_rtfDocument, 0, _rtfDocument.Length);
            _documentHeader.WriteContent(outs);
            _data.WriteTo(outs);
            outs.Write(CloseGroup, 0, CloseGroup.Length);
        }
        catch (IOException)
        {
        }
    }

    /// <summary>
    ///     Returns  true  if  m.length  characters in  str , starting at offset  soff
    ///     match the bytes in the given array  m .
    /// </summary>
    /// <param name="str">the string to search for a match</param>
    /// <param name="soff">the starting offset in str</param>
    /// <param name="m">the array to match</param>
    /// <returns> true  if there is match</returns>
    private static bool subMatch(string str, int soff, byte[] m)
    {
        for (var k = 0; k < m.Length; k++)
        {
            if (str[soff++] != (char)m[k])
            {
                return false;
            }
        }

        return true;
    }
}