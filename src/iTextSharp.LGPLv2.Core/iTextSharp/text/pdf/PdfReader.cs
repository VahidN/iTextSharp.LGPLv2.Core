using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.util;
using System.util.zlib;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf.interfaces;
using iTextSharp.text.pdf.intern;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
///     Reads a PDF document.
///     @author Paulo Soares (psoares@consiste.pt)
///     @author Kazuya Ujihara
/// </summary>
public class PdfReader : IPdfViewerPreferences, IDisposable
{
    private static readonly byte[] _endobj = PdfEncodings.ConvertToBytes("endobj", null);

    private static readonly byte[] _endstream = PdfEncodings.ConvertToBytes("endstream", null);

    private static readonly PdfName[] _pageInhCandidates =
    {
        PdfName.Mediabox, PdfName.Rotate, PdfName.Resources, PdfName.Cropbox
    };

    /// <summary>
    ///     Allows reading the pdf file without the owner password.
    /// </summary>
    public static bool AllowOpenWithFullPermissions = false;

    private readonly bool _partial;
    private readonly PdfViewerPreferencesImp _viewerPreferences = new();

    /// <summary>
    ///     Holds value of property appendable.
    /// </summary>
    private bool _appendable;

    private bool _bBailout;

    private PrIndirectReference _cryptoRef;
    private bool _encryptionError;
    private bool _hybridXref;
    private int _iPRObjectDepth;
    private int _lastXrefPartial = -1;
    private int _objGen;

    private int _objNum;

    //added by Aiken Sam for certificate decryption
    //added by Aiken Sam for certificate decryption
    private bool _ownerPasswordUsed;

    /// <summary>
    ///     Track how deeply nested the current object is, so
    /// </summary>
    /// <summary>
    ///     we know when to return an individual null or boolean, or
    /// </summary>
    /// <summary>
    ///     reuse one of the static ones.
    /// </summary>
    private int _readDepth;

    private PdfDictionary _rootPages;
    private List<PdfObject> _xrefObj;

    protected internal PrAcroForm acroForm;

    protected internal bool AcroFormParsed;

    protected internal PdfDictionary catalog;

    protected X509Certificate Certificate;

    //added by ujihara for decryption
    protected ICipherParameters CertificateKey;

    protected internal bool consolidateNamedDestinations;

    protected internal PdfEncryption decrypt;

    protected internal bool Encrypted;

    protected internal int eofPos;

    protected internal int FreeXref;

    protected internal int lastXref;

    protected internal bool NewXrefType;

    protected internal INullValueDictionary<int, NullValueDictionary<int, int>> ObjStmMark;

    protected internal NullValueDictionary<int, int> ObjStmToOffset;

    protected internal PageRefs pageRefs;

    protected internal byte[] Password;

    protected internal char pdfVersion;

    protected internal int PValue;

    protected internal bool Rebuilt;

    protected internal int RValue;

    protected internal bool SharedStreams = true;

    protected internal List<PdfString> Strings = new();

    protected internal bool tampered;

    protected internal PrTokeniser Tokens;

    protected internal PdfDictionary trailer;

    /// <summary>
    ///     Each xref pair is a position
    /// </summary>
    /// <summary>
    ///     type 0 -> -1, 0
    /// </summary>
    /// <summary>
    ///     type 1 -> offset, 0
    /// </summary>
    /// <summary>
    ///     type 2 -> index, obj num
    /// </summary>
    protected internal int[] Xref;

    protected internal List<int> xrefByteOffset = new();

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="filename">the file name of the document</param>
    public PdfReader(string filename) : this(filename, null)
    {
    }

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="filename">the file name of the document</param>
    /// <param name="ownerPassword">the password to read the document</param>
    public PdfReader(string filename, byte[] ownerPassword)
    {
        Password = ownerPassword;
        Tokens = new PrTokeniser(filename);
        ReadPdf();
    }

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="pdfIn">the byte array with the document</param>
    public PdfReader(byte[] pdfIn) : this(pdfIn, null)
    {
    }

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="pdfIn">the byte array with the document</param>
    /// <param name="ownerPassword">the password to read the document</param>
    public PdfReader(byte[] pdfIn, byte[] ownerPassword)
    {
        Password = ownerPassword;
        Tokens = new PrTokeniser(pdfIn);
        ReadPdf();
    }

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="filename">the file name of the document</param>
    /// <param name="certificate">the certificate to read the document</param>
    /// <param name="certificateKey">the private key of the certificate</param>
    public PdfReader(string filename, X509Certificate certificate, ICipherParameters certificateKey)
    {
        Certificate = certificate;
        CertificateKey = certificateKey;
        Tokens = new PrTokeniser(filename);
        ReadPdf();
    }

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="url">the Uri of the document</param>
    public PdfReader(Uri url) : this(url, null)
    {
    }

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="url">the Uri of the document</param>
    /// <param name="ownerPassword">the password to read the document</param>
    public PdfReader(Uri url, byte[] ownerPassword)
    {
        Password = ownerPassword;
        Tokens = new PrTokeniser(new RandomAccessFileOrArray(url));
        ReadPdf();
    }

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     end but is not closed
    ///     @throws IOException on error
    /// </summary>
    /// <param name="isp">the  InputStream  containing the document. The stream is read to the</param>
    /// <param name="ownerPassword">the password to read the document</param>
    /// <param name="forceRead">force the read of the entire stream, even it's a FileStream</param>
    public PdfReader(Stream isp, byte[] ownerPassword, bool forceRead = true)
    {
        Password = ownerPassword;
        Tokens = new PrTokeniser(new RandomAccessFileOrArray(isp, forceRead));
        ReadPdf();
    }

    /// <summary>
    ///     Reads and parses a PDF document.
    ///     end but is not closed
    ///     @throws IOException on error
    /// </summary>
    /// <param name="isp">the  InputStream  containing the document. The stream is read to the</param>
    /// <param name="forceRead">force the read of the entire stream, even it's a FileStream</param>
    public PdfReader(Stream isp, bool forceRead = true) : this(isp, null, forceRead)
    {
    }

    /// <summary>
    ///     Reads and parses a pdf document. Contrary to the other constructors only the xref is read
    ///     into memory. The reader is said to be working in "partial" mode as only parts of the pdf
    ///     are read as needed. The pdf is left open but may be closed at any time with
    ///     PdfReader.Close() , reopen is automatic.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="raf">the document location</param>
    /// <param name="ownerPassword">the password or  null  for no password</param>
    public PdfReader(RandomAccessFileOrArray raf, byte[] ownerPassword)
    {
        Password = ownerPassword;
        _partial = true;
        Tokens = new PrTokeniser(raf);
        ReadPdfPartial();
    }

    /// <summary>
    ///     Creates an independent duplicate.
    /// </summary>
    /// <param name="reader">the  PdfReader  to duplicate</param>
    public PdfReader(PdfReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        _appendable = reader._appendable;
        consolidateNamedDestinations = reader.consolidateNamedDestinations;
        Encrypted = reader.Encrypted;
        Rebuilt = reader.Rebuilt;
        SharedStreams = reader.SharedStreams;
        tampered = reader.tampered;
        Password = reader.Password;
        pdfVersion = reader.pdfVersion;
        eofPos = reader.eofPos;
        FreeXref = reader.FreeXref;
        lastXref = reader.lastXref;
        Tokens = new PrTokeniser(reader.Tokens.SafeFile);

        if (reader.decrypt != null)
        {
            decrypt = new PdfEncryption(reader.decrypt);
        }

        PValue = reader.PValue;
        RValue = reader.RValue;
        _xrefObj = new List<PdfObject>(reader._xrefObj);

        for (var k = 0; k < reader._xrefObj.Count; ++k)
        {
            _xrefObj[k] = DuplicatePdfObject(reader._xrefObj[k], this);
        }

        pageRefs = new PageRefs(reader.pageRefs, this);
        trailer = (PdfDictionary)DuplicatePdfObject(reader.trailer, this);
        catalog = trailer.GetAsDict(PdfName.Root);
        _rootPages = catalog.GetAsDict(PdfName.Pages);
        FileLength = reader.FileLength;
        _partial = reader._partial;
        _hybridXref = reader._hybridXref;
        ObjStmToOffset = reader.ObjStmToOffset;
        Xref = reader.Xref;
        _cryptoRef = (PrIndirectReference)DuplicatePdfObject(reader._cryptoRef, this);
        _ownerPasswordUsed = reader._ownerPasswordUsed;
    }

    /// <summary>
    ///     Reads and parses a pdf document.
    ///     It allows working with streams.
    /// </summary>
    public PdfReader(Stream isp, X509Certificate certificate, ICipherParameters certificateKey)
    {
        Certificate = certificate;
        CertificateKey = certificateKey;
        Tokens = new PrTokeniser(new RandomAccessFileOrArray(isp));
        ReadPdf();
    }

    protected internal PdfReader()
    {
    }

    /// <summary>
    ///     Gets a read-only version of  AcroFields .
    /// </summary>
    /// <returns>a read-only version of  AcroFields </returns>
    public AcroFields AcroFields => new(this, null);

    /// <summary>
    ///     Returns the document's acroform, if it has one.
    /// </summary>
    /// <returns>the document's acroform</returns>
    public PrAcroForm AcroForm
    {
        get
        {
            if (!AcroFormParsed)
            {
                AcroFormParsed = true;
                var form = catalog.Get(PdfName.Acroform);

                if (form != null)
                {
                    try
                    {
                        acroForm = new PrAcroForm(this);
                        acroForm.ReadAcroForm((PdfDictionary)GetPdfObject(form));
                    }
                    catch
                    {
                        acroForm = null;
                    }
                }
            }

            return acroForm;
        }
    }

    public bool Appendable
    {
        set
        {
            _appendable = value;

            if (_appendable)
            {
                GetPdfObject(trailer.Get(PdfName.Root));
            }
        }
        get => _appendable;
    }

    /// <summary>
    ///     Returns the document's catalog. This dictionary is not a copy,
    ///     any changes will be reflected in the catalog.
    /// </summary>
    /// <returns>the document's catalog</returns>
    public PdfDictionary Catalog => catalog;

    /// <summary>
    ///     Gets the byte address of the %%EOF marker.
    /// </summary>
    /// <returns>the byte address of the %%EOF marker</returns>
    public int EofPos => eofPos;

    /// <summary>
    ///     Getter for property fileLength.
    /// </summary>
    /// <returns>Value of property fileLength.</returns>
    public int FileLength { get; private set; }

    /// <summary>
    ///     Returns the content of the document information dictionary as a  Hashtable
    ///     of  String .
    /// </summary>
    /// <returns>content of the document information dictionary</returns>
    public INullValueDictionary<string, string> Info
    {
        get
        {
            var map = new NullValueDictionary<string, string>();
            var info = trailer.GetAsDict(PdfName.Info);

            if (info == null)
            {
                return map;
            }

            foreach (var key in info.Keys)
            {
                var obj = GetPdfObject(info.Get(key));

                if (obj == null)
                {
                    continue;
                }

                var value = obj.ToString();

                switch (obj.Type)
                {
                    case PdfObject.STRING:
                    {
                        value = ((PdfString)obj).ToUnicodeString();

                        break;
                    }
                    case PdfObject.NAME:
                    {
                        value = PdfName.DecodeName(value);

                        break;
                    }
                }

                map[PdfName.DecodeName(key.ToString())] = value;
            }

            return map;
        }
    }

    /// <summary>
    ///     Checks if the document was opened with the owner password so that the end application
    ///     can decide what level of access restrictions to apply. If the document is not encrypted
    ///     it will return  true .
    ///     false  if the document was opened with the user password
    /// </summary>
    /// <returns> true  if the document was opened with the owner password or if it's not encrypted,</returns>
    public bool IsOpenedWithFullPermissions => !Encrypted || _ownerPasswordUsed || AllowOpenWithFullPermissions;

    /// <summary>
    ///     Gets the global document JavaScript.
    ///     @throws IOException on error
    /// </summary>
    /// <returns>the global document JavaScript</returns>
    public string JavaScript
    {
        get
        {
            var rf = SafeFile;

            try
            {
                rf.ReOpen();

                return GetJavaScript(rf);
            }
            finally
            {
                try
                {
                    rf.Close();
                }
                catch
                {
                }
            }
        }
    }

    /// <summary>
    ///     Gets the byte address of the last xref table.
    /// </summary>
    /// <returns>the byte address of the last xref table</returns>
    public int LastXref => lastXref;

    /// <summary>
    ///     Gets the byte address of the all xref tables.
    /// </summary>
    /// <returns>the byte address of all the xref tables</returns>
    public IList<int> XrefByteOffset => xrefByteOffset;

    /// <summary>
    ///     Gets the XML metadata.
    ///     @throws IOException on error
    /// </summary>
    /// <returns>the XML metadata</returns>
    public byte[] Metadata
    {
        get
        {
            var obj = GetPdfObject(catalog.Get(PdfName.Metadata));

            if (!(obj is PrStream))
            {
                return null;
            }

            var rf = SafeFile;
            byte[] b = null;

            try
            {
                rf.ReOpen();
                b = GetStreamBytes((PrStream)obj, rf);
            }
            finally
            {
                try
                {
                    rf.Close();
                }
                catch
                {
                    // empty on purpose
                }
            }

            return b;
        }
    }

    /// <summary>
    ///     Gets the number of pages in the document.
    /// </summary>
    /// <returns>the number of pages in the document</returns>
    public int NumberOfPages => pageRefs.Size;

    /// <summary>
    ///     Gets the PDF version. Only the last version char is returned. For example
    ///     version 1.4 is returned as '4'.
    /// </summary>
    /// <returns>the PDF version</returns>
    public char PdfVersion => pdfVersion;

    /// <summary>
    ///     Gets the encryption permissions. It can be used directly in
    ///     PdfWriter.SetEncryption() .
    /// </summary>
    /// <returns>the encryption permissions</returns>
    public int Permissions => PValue;

    /// <summary>
    ///     Gets a new file instance of the original PDF
    ///     document.
    /// </summary>
    /// <returns>a new file instance of the original PDF document</returns>
    public RandomAccessFileOrArray SafeFile => Tokens.SafeFile;

    /// <summary>
    ///     Returns a bitset representing the PageMode and PageLayout viewer preferences.
    ///     Doesn't return any information about the ViewerPreferences dictionary.
    /// </summary>
    /// <returns>an int that contains the Viewer Preferences.</returns>
    public virtual int SimpleViewerPreferences
        => PdfViewerPreferencesImp.GetViewerPreferences(catalog).PageLayoutAndMode;

    /// <summary>
    ///     Sets the tampered state. A tampered PdfReader cannot be reused in PdfStamper.
    /// </summary>
    public bool Tampered
    {
        get => tampered;
        set
        {
            tampered = value;
            pageRefs.KeepPages();
        }
    }

    /// <summary>
    ///     Gets the trailer dictionary
    /// </summary>
    /// <returns>the trailer dictionary</returns>
    public PdfDictionary Trailer => trailer;

    /// <summary>
    ///     Gets the number of xref objects.
    /// </summary>
    /// <returns>the number of xref objects</returns>
    public int XrefSize => _xrefObj.Count;

    internal PdfEncryption Decrypt => decrypt;

    public void Dispose()
    {
        Close();

        // Ensure large pdf cleaned up before continuing
        _xrefObj.Clear();
    }

    /// <summary>
    ///     Sets the viewer preferences as the sum of several constants.
    ///     @see PdfViewerPreferences#setViewerPreferences
    /// </summary>
    public virtual int ViewerPreferences
    {
        set
        {
            _viewerPreferences.ViewerPreferences = value;
            SetViewerPreferences(_viewerPreferences);
        }
    }

    /// <summary>
    ///     Adds a viewer preference
    ///     @see PdfViewerPreferences#addViewerPreference
    /// </summary>
    /// <param name="key">a key for a viewer preference</param>
    /// <param name="value">a value for the viewer preference</param>
    public virtual void AddViewerPreference(PdfName key, PdfObject value)
    {
        _viewerPreferences.AddViewerPreference(key, value);
        SetViewerPreferences(_viewerPreferences);
    }

    /// <summary>
    ///     Decodes a stream that has the ASCII85Decode filter.
    /// </summary>
    /// <param name="inp">the input data</param>
    /// <returns>the decoded data</returns>
    public static byte[] Ascii85Decode(byte[] inp)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        using var outp = new MemoryStream();
        var state = 0;
        var chn = new int[5];

        for (var k = 0; k < inp.Length; ++k)
        {
            var ch = inp[k] & 0xff;

            if (ch == '~')
            {
                break;
            }

            if (PrTokeniser.IsWhitespace(ch))
            {
                continue;
            }

            if (ch == 'z' && state == 0)
            {
                outp.WriteByte(0);
                outp.WriteByte(0);
                outp.WriteByte(0);
                outp.WriteByte(0);

                continue;
            }

            if (ch < '!' || ch > 'u')
            {
                throw new ArgumentException("Illegal character in ASCII85Decode.");
            }

            chn[state] = ch - '!';
            ++state;

            if (state == 5)
            {
                state = 0;
                var rx = 0;

                for (var j = 0; j < 5; ++j)
                {
                    rx = rx * 85 + chn[j];
                }

                outp.WriteByte((byte)(rx >> 24));
                outp.WriteByte((byte)(rx >> 16));
                outp.WriteByte((byte)(rx >> 8));
                outp.WriteByte((byte)rx);
            }
        }

        var r = 0;

        // We'll ignore the next two lines for the sake of perpetuating broken PDFs
        //            if (state == 1)
        //                throw new ArgumentException("Illegal length in ASCII85Decode.");
        if (state == 2)
        {
            r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85 + 85 * 85 * 85 + 85 * 85 + 85;
            outp.WriteByte((byte)(r >> 24));
        }
        else if (state == 3)
        {
            r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85 + chn[2] * 85 * 85 + 85 * 85 + 85;
            outp.WriteByte((byte)(r >> 24));
            outp.WriteByte((byte)(r >> 16));
        }
        else if (state == 4)
        {
            r = chn[0] * 85 * 85 * 85 * 85 + chn[1] * 85 * 85 * 85 + chn[2] * 85 * 85 + chn[3] * 85 + 85;
            outp.WriteByte((byte)(r >> 24));
            outp.WriteByte((byte)(r >> 16));
            outp.WriteByte((byte)(r >> 8));
        }

        return outp.ToArray();
    }

    /// <summary>
    ///     Decodes a stream that has the ASCIIHexDecode filter.
    /// </summary>
    /// <param name="inp">the input data</param>
    /// <returns>the decoded data</returns>
    public static byte[] AsciiHexDecode(byte[] inp)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        using var outp = new MemoryStream();
        var first = true;
        var n1 = 0;

        for (var k = 0; k < inp.Length; ++k)
        {
            var ch = inp[k] & 0xff;

            if (ch == '>')
            {
                break;
            }

            if (PrTokeniser.IsWhitespace(ch))
            {
                continue;
            }

            var n = PrTokeniser.GetHex(ch);

            if (n == -1)
            {
                throw new ArgumentException("Illegal character in ASCIIHexDecode.");
            }

            if (first)
            {
                n1 = n;
            }
            else
            {
                outp.WriteByte((byte)((n1 << 4) + n));
            }

            first = !first;
        }

        if (!first)
        {
            outp.WriteByte((byte)(n1 << 4));
        }

        return outp.ToArray();
    }

    /// <summary>
    /// </summary>
    /// <param name="inp"></param>
    /// <param name="dicPar"></param>
    /// <returns>a byte array</returns>
    public static byte[] DecodePredictor(byte[] inp, PdfObject dicPar)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        if (dicPar == null || !dicPar.IsDictionary())
        {
            return inp;
        }

        var dic = (PdfDictionary)dicPar;
        var obj = GetPdfObject(dic.Get(PdfName.Predictor));

        if (obj == null || !obj.IsNumber())
        {
            return inp;
        }

        var predictor = ((PdfNumber)obj).IntValue;

        if (predictor < 10)
        {
            return inp;
        }

        var width = 1;
        obj = GetPdfObject(dic.Get(PdfName.Columns));

        if (obj != null && obj.IsNumber())
        {
            width = ((PdfNumber)obj).IntValue;
        }

        var colors = 1;
        obj = GetPdfObject(dic.Get(PdfName.Colors));

        if (obj != null && obj.IsNumber())
        {
            colors = ((PdfNumber)obj).IntValue;
        }

        var bpc = 8;
        obj = GetPdfObject(dic.Get(PdfName.Bitspercomponent));

        if (obj != null && obj.IsNumber())
        {
            bpc = ((PdfNumber)obj).IntValue;
        }

        using var dataStream = new MemoryStream(inp);
        using var fout = new MemoryStream(inp.Length);
        var bytesPerPixel = colors * bpc / 8;
        var bytesPerRow = (colors * width * bpc + 7) / 8;
        var curr = new byte[bytesPerRow];
        var prior = new byte[bytesPerRow];

        // Decode the (sub)image row-by-row
        while (true)
        {
            // Read the filter type byte and a row of data
            var filter = 0;

            try
            {
                filter = dataStream.ReadByte();

                if (filter < 0)
                {
                    return fout.ToArray();
                }

                var tot = 0;

                while (tot < bytesPerRow)
                {
                    var n = dataStream.Read(curr, tot, bytesPerRow - tot);

                    if (n <= 0)
                    {
                        return fout.ToArray();
                    }

                    tot += n;
                }
            }
            catch
            {
                return fout.ToArray();
            }

            switch (filter)
            {
                case 0: //PNG_FILTER_NONE
                    break;
                case 1: //PNG_FILTER_SUB
                    for (var i = bytesPerPixel; i < bytesPerRow; i++)
                    {
                        curr[i] += curr[i - bytesPerPixel];
                    }

                    break;
                case 2: //PNG_FILTER_UP
                    for (var i = 0; i < bytesPerRow; i++)
                    {
                        curr[i] += prior[i];
                    }

                    break;
                case 3: //PNG_FILTER_AVERAGE
                    for (var i = 0; i < bytesPerPixel; i++)
                    {
                        curr[i] += (byte)(prior[i] / 2);
                    }

                    for (var i = bytesPerPixel; i < bytesPerRow; i++)
                    {
                        curr[i] += (byte)(((curr[i - bytesPerPixel] & 0xff) + (prior[i] & 0xff)) / 2);
                    }

                    break;
                case 4: //PNG_FILTER_PAETH
                    for (var i = 0; i < bytesPerPixel; i++)
                    {
                        curr[i] += prior[i];
                    }

                    for (var i = bytesPerPixel; i < bytesPerRow; i++)
                    {
                        var a = curr[i - bytesPerPixel] & 0xff;
                        var b = prior[i] & 0xff;
                        var c = prior[i - bytesPerPixel] & 0xff;

                        var p = a + b - c;
                        var pa = Math.Abs(p - a);
                        var pb = Math.Abs(p - b);
                        var pc = Math.Abs(p - c);

                        int ret;

                        if (pa <= pb && pa <= pc)
                        {
                            ret = a;
                        }
                        else if (pb <= pc)
                        {
                            ret = b;
                        }
                        else
                        {
                            ret = c;
                        }

                        curr[i] += (byte)ret;
                    }

                    break;
                default:
                    // Error -- uknown filter type
                    throw new InvalidOperationException("PNG filter unknown.");
            }

            fout.Write(curr, 0, curr.Length);

            // Swap curr and prior
            var tmp = prior;
            prior = curr;
            curr = tmp;
        }
    }

    /// <summary>
    ///     Decodes a stream that has the FlateDecode filter.
    /// </summary>
    /// <param name="inp">the input data</param>
    /// <returns>the decoded data</returns>
    public static byte[] FlateDecode(byte[] inp)
    {
        var b = FlateDecode(inp, true);

        if (b == null)
        {
            return FlateDecode(inp, false);
        }

        return b;
    }

    /// <summary>
    ///     A helper to FlateDecode.
    ///     to try to read a corrupted stream
    /// </summary>
    /// <param name="inp">the input data</param>
    /// <param name="strict"> true  to read a correct stream.  false </param>
    /// <returns>the decoded data</returns>
    public static byte[] FlateDecode(byte[] inp, bool strict)
    {
        var stream = new MemoryStream(inp);
        var zip = new ZInflaterInputStream(stream);
        using var outp = new MemoryStream();
        var b = new byte[strict ? 4092 : 1];

        try
        {
            int n;

            while ((n = zip.Read(b, 0, b.Length)) > 0)
            {
                outp.Write(b, 0, n);
            }

            return outp.ToArray();
        }
        catch
        {
            if (strict)
            {
                return null;
            }

            return outp.ToArray();
        }
    }

    /// <summary>
    ///     Normalizes a  Rectangle  so that llx and lly are smaller than urx and ury.
    /// </summary>
    /// <param name="box">the original rectangle</param>
    /// <returns>a normalized  Rectangle </returns>
    public static Rectangle GetNormalizedRectangle(PdfArray box)
    {
        if (box == null)
        {
            throw new ArgumentNullException(nameof(box));
        }

        var llx = ((PdfNumber)GetPdfObjectRelease(box[0])).FloatValue;
        var lly = ((PdfNumber)GetPdfObjectRelease(box[1])).FloatValue;
        var urx = ((PdfNumber)GetPdfObjectRelease(box[2])).FloatValue;
        var ury = ((PdfNumber)GetPdfObjectRelease(box[3])).FloatValue;

        return new Rectangle(Math.Min(llx, urx), Math.Min(lly, ury), Math.Max(llx, urx), Math.Max(lly, ury));
    }

    /// <summary>
    ///     Reads a  PdfObject  resolving an indirect reference
    ///     if needed.
    /// </summary>
    /// <param name="obj">the  PdfObject  to read</param>
    /// <returns>the resolved  PdfObject </returns>
    public static PdfObject GetPdfObject(PdfObject obj)
    {
        if (obj == null)
        {
            return null;
        }

        if (!obj.IsIndirect())
        {
            return obj;
        }

        var refi = (PrIndirectReference)obj;
        var idx = refi.Number;
        var appendable = refi.Reader._appendable;
        obj = refi.Reader.GetPdfObject(idx);

        if (obj == null)
        {
            return null;
        }

        if (appendable)
        {
            switch (obj.Type)
            {
                case PdfObject.NULL:
                    obj = new PdfNull();

                    break;
                case PdfObject.BOOLEAN:
                    obj = new PdfBoolean(((PdfBoolean)obj).BooleanValue);

                    break;
                case PdfObject.NAME:
                    obj = new PdfName(obj.GetBytes());

                    break;
            }

            obj.IndRef = refi;
        }

        return obj;
    }

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parent"></param>
    /// <returns>a PdfObject</returns>
    public static PdfObject GetPdfObject(PdfObject obj, PdfObject parent)
    {
        if (obj == null)
        {
            return null;
        }

        if (!obj.IsIndirect())
        {
            PrIndirectReference refi = null;

            if (parent != null && (refi = parent.IndRef) != null && refi.Reader.Appendable)
            {
                switch (obj.Type)
                {
                    case PdfObject.NULL:
                        obj = new PdfNull();

                        break;
                    case PdfObject.BOOLEAN:
                        obj = new PdfBoolean(((PdfBoolean)obj).BooleanValue);

                        break;
                    case PdfObject.NAME:
                        obj = new PdfName(obj.GetBytes());

                        break;
                }

                obj.IndRef = refi;
            }

            return obj;
        }

        return GetPdfObject(obj);
    }

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>a PdfObject</returns>
    public static PdfObject GetPdfObjectRelease(PdfObject obj)
    {
        var obj2 = GetPdfObject(obj);
        ReleaseLastXrefPartial(obj);

        return obj2;
    }

    /// <summary>
    ///     Reads a  PdfObject  resolving an indirect reference
    ///     if needed. If the reader was opened in partial mode the object will be released
    ///     to save memory.
    /// </summary>
    /// <param name="obj">the  PdfObject  to read</param>
    /// <param name="parent"></param>
    /// <returns>a PdfObject</returns>
    public static PdfObject GetPdfObjectRelease(PdfObject obj, PdfObject parent)
    {
        var obj2 = GetPdfObject(obj, parent);
        ReleaseLastXrefPartial(obj);

        return obj2;
    }

    /// <summary>
    ///     Get the content from a stream applying the required filters.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="stream">the stream</param>
    /// <param name="file">the location where the stream is</param>
    /// <returns>the stream content</returns>
    public static byte[] GetStreamBytes(PrStream stream, RandomAccessFileOrArray file)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var filter = GetPdfObjectRelease(stream.Get(PdfName.Filter));
        var b = GetStreamBytesRaw(stream, file);
        IList<PdfObject> filters = new List<PdfObject>();

        if (filter != null)
        {
            if (filter.IsName())
            {
                filters.Add(filter);
            }
            else if (filter.IsArray())
            {
                filters = ((PdfArray)filter).ArrayList;
            }
        }

        IList<PdfObject> dp = new List<PdfObject>();
        var dpo = GetPdfObjectRelease(stream.Get(PdfName.Decodeparms));

        if (dpo == null || (!dpo.IsDictionary() && !dpo.IsArray()))
        {
            dpo = GetPdfObjectRelease(stream.Get(PdfName.Dp));
        }

        if (dpo != null)
        {
            if (dpo.IsDictionary())
            {
                dp.Add(dpo);
            }
            else if (dpo.IsArray())
            {
                dp = ((PdfArray)dpo).ArrayList;
            }
        }

        string name;

        for (var j = 0; j < filters.Count; ++j)
        {
            name = ((PdfName)GetPdfObjectRelease(filters[j])).ToString();

            if (name.Equals("/FlateDecode", StringComparison.Ordinal) || name.Equals("/Fl", StringComparison.Ordinal))
            {
                b = FlateDecode(b);
                PdfObject dicParam = null;

                if (j < dp.Count)
                {
                    dicParam = dp[j];
                    b = DecodePredictor(b, dicParam);
                }
            }
            else if (name.Equals("/ASCIIHexDecode", StringComparison.Ordinal) ||
                     name.Equals("/AHx", StringComparison.Ordinal))
            {
                b = AsciiHexDecode(b);
            }
            else if (name.Equals("/ASCII85Decode", StringComparison.Ordinal) ||
                     name.Equals("/A85", StringComparison.Ordinal))
            {
                b = Ascii85Decode(b);
            }
            else if (name.Equals("/LZWDecode", StringComparison.Ordinal))
            {
                b = LzwDecode(b);
                PdfObject dicParam = null;

                if (j < dp.Count)
                {
                    dicParam = dp[j];
                    b = DecodePredictor(b, dicParam);
                }
            }
            else if (name.Equals("/Crypt", StringComparison.Ordinal))
            {
            }
            else
            {
                throw new UnsupportedPdfException("The filter " + name + " is not supported.");
            }
        }

        return b;
    }

    /// <summary>
    ///     Get the content from a stream applying the required filters.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="stream">the stream</param>
    /// <returns>the stream content</returns>
    public static byte[] GetStreamBytes(PrStream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var rf = stream.Reader.SafeFile;

        try
        {
            rf.ReOpen();

            return GetStreamBytes(stream, rf);
        }
        finally
        {
            try
            {
                rf.Close();
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///     Get the content from a stream as it is without applying any filter.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="stream">the stream</param>
    /// <param name="file">the location where the stream is</param>
    /// <returns>the stream content</returns>
    public static byte[] GetStreamBytesRaw(PrStream stream, RandomAccessFileOrArray file)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (file == null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        var reader = stream.Reader;
        byte[] b;

        if (stream.Offset < 0)
        {
            b = stream.GetBytes();
        }
        else
        {
            b = new byte[stream.Length];
            file.Seek(stream.Offset);
            file.ReadFully(b);
            var decrypt = reader.Decrypt;

            if (decrypt != null)
            {
                var filter = GetPdfObjectRelease(stream.Get(PdfName.Filter));
                IList<PdfObject> filters = new List<PdfObject>();

                if (filter != null)
                {
                    if (filter.IsName())
                    {
                        filters.Add(filter);
                    }
                    else if (filter.IsArray())
                    {
                        filters = ((PdfArray)filter).ArrayList;
                    }
                }

                var skip = false;

                for (var k = 0; k < filters.Count; ++k)
                {
                    var obj = GetPdfObjectRelease(filters[k]);

                    if (obj != null && obj.ToString().Equals("/Crypt", StringComparison.Ordinal))
                    {
                        skip = true;

                        break;
                    }
                }

                if (!skip)
                {
                    decrypt.SetHashKey(stream.ObjNum, stream.ObjGen);
                    b = decrypt.DecryptByteArray(b);
                }
            }
        }

        return b;
    }

    /// <summary>
    ///     Get the content from a stream as it is without applying any filter.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="stream">the stream</param>
    /// <returns>the stream content</returns>
    public static byte[] GetStreamBytesRaw(PrStream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var rf = stream.Reader.SafeFile;

        try
        {
            rf.ReOpen();

            return GetStreamBytesRaw(stream, rf);
        }
        finally
        {
            try
            {
                rf.Close();
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///     Eliminates the reference to the object freeing the memory used by it and clearing
    ///     the xref entry.
    /// </summary>
    /// <param name="obj">the object. If it's an indirect reference it will be eliminated</param>
    /// <returns>the object or the already erased dereferenced object</returns>
    public static PdfObject KillIndirect(PdfObject obj)
    {
        if (obj == null || obj.IsNull())
        {
            return null;
        }

        var ret = GetPdfObjectRelease(obj);

        if (obj.IsIndirect())
        {
            var refi = (PrIndirectReference)obj;
            var reader = refi.Reader;
            var n = refi.Number;
            reader._xrefObj[n] = null;

            if (reader._partial)
            {
                reader.Xref[n * 2] = -1;
            }
        }

        return ret;
    }

    /// <summary>
    ///     Decodes a stream that has the LZWDecode filter.
    /// </summary>
    /// <param name="inp">the input data</param>
    /// <returns>the decoded data</returns>
    public static byte[] LzwDecode(byte[] inp)
    {
        var outp = new MemoryStream();
        var lzw = new LzwDecoder();
        lzw.Decode(inp, outp);

        return outp.ToArray();
    }

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    public static void ReleaseLastXrefPartial(PdfObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (!obj.IsIndirect())
        {
            return;
        }

        if (!(obj is PrIndirectReference))
        {
            return;
        }

        var refi = (PrIndirectReference)obj;
        var reader = refi.Reader;

        if (reader._partial && reader._lastXrefPartial != -1 && reader._lastXrefPartial == refi.Number)
        {
            reader._xrefObj[reader._lastXrefPartial] = null;
        }

        reader._lastXrefPartial = -1;
    }

    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>an indirect reference</returns>
    public PrIndirectReference AddPdfObject(PdfObject obj)
    {
        _xrefObj.Add(obj);

        return new PrIndirectReference(this, _xrefObj.Count - 1);
    }

    /// <summary>
    ///     Closes the reader
    /// </summary>
    public void Close()
    {
        if (!_partial)
        {
            return;
        }

        Tokens.Close();
    }

    public byte[] ComputeUserPassword()
    {
        if (!Encrypted || !_ownerPasswordUsed)
        {
            return null;
        }

        return decrypt.ComputeUserPassword(Password);
    }

    /// <summary>
    ///     Replaces all the local named links with the actual destinations.
    /// </summary>
    public void ConsolidateNamedDestinations()
    {
        if (consolidateNamedDestinations)
        {
            return;
        }

        consolidateNamedDestinations = true;
        var names = GetNamedDestination(true);

        if (names.Count == 0)
        {
            return;
        }

        for (var k = 1; k <= pageRefs.Size; ++k)
        {
            var page = pageRefs.GetPageN(k);
            PdfObject annotsRef;
            var annots = (PdfArray)GetPdfObject(annotsRef = page.Get(PdfName.Annots));
            var annotIdx = _lastXrefPartial;
            ReleaseLastXrefPartial();

            if (annots == null)
            {
                pageRefs.ReleasePage(k);

                continue;
            }

            var commitAnnots = false;

            for (var an = 0; an < annots.Size; ++an)
            {
                var objRef = annots[an];

                if (replaceNamedDestination(objRef, names) && !objRef.IsIndirect())
                {
                    commitAnnots = true;
                }
            }

            if (commitAnnots)
            {
                setXrefPartialObject(annotIdx, annots);
            }

            if (!commitAnnots || annotsRef.IsIndirect())
            {
                pageRefs.ReleasePage(k);
            }
        }

        var outlines = (PdfDictionary)GetPdfObjectRelease(catalog.Get(PdfName.Outlines));

        if (outlines == null)
        {
            return;
        }

        iterateBookmarks(outlines.Get(PdfName.First), names);
    }

    /// <summary>
    ///     Finds all the fonts not subset but embedded and marks them as subset.
    /// </summary>
    /// <returns>the number of fonts altered</returns>
    public int CreateFakeFontSubsets()
    {
        var total = 0;

        for (var k = 1; k < _xrefObj.Count; ++k)
        {
            var obj = GetPdfObjectRelease(k);

            if (obj == null || !obj.IsDictionary())
            {
                continue;
            }

            var dic = (PdfDictionary)obj;

            if (!ExistsName(dic, PdfName.TYPE, PdfName.Font))
            {
                continue;
            }

            if (ExistsName(dic, PdfName.Subtype, PdfName.Type1) || ExistsName(dic, PdfName.Subtype, PdfName.Mmtype1) ||
                ExistsName(dic, PdfName.Subtype, PdfName.Truetype))
            {
                var s = GetSubsetPrefix(dic);

                if (s != null)
                {
                    continue;
                }

                s = GetFontName(dic);

                if (s == null)
                {
                    continue;
                }

                var ns = BaseFont.CreateSubsetPrefix() + s;
                var fd = (PdfDictionary)GetPdfObjectRelease(dic.Get(PdfName.Fontdescriptor));

                if (fd == null)
                {
                    continue;
                }

                if (fd.Get(PdfName.Fontfile) == null && fd.Get(PdfName.Fontfile2) == null &&
                    fd.Get(PdfName.Fontfile3) == null)
                {
                    continue;
                }

                fd = dic.GetAsDict(PdfName.Fontdescriptor);
                var newName = new PdfName(ns);
                dic.Put(PdfName.Basefont, newName);
                fd.Put(PdfName.Fontname, newName);
                setXrefPartialObject(k, dic);
                ++total;
            }
        }

        return total;
    }

    /// <summary>
    /// </summary>
    /// <returns>the percentage of the cross reference table that has been read</returns>
    public double DumpPerc()
    {
        var total = 0;

        for (var k = 0; k < _xrefObj.Count; ++k)
        {
            if (_xrefObj[k] != null)
            {
                ++total;
            }
        }

        return total * 100.0 / _xrefObj.Count;
    }

    /// <summary>
    ///     Eliminates shared streams if they exist.
    /// </summary>
    public void EliminateSharedStreams()
    {
        if (!SharedStreams)
        {
            return;
        }

        SharedStreams = false;

        if (pageRefs.Size == 1)
        {
            return;
        }

        List<PrIndirectReference> newRefs = new();
        List<PrStream> newStreams = new();
        var visited = new NullValueDictionary<int, int>();

        for (var k = 1; k <= pageRefs.Size; ++k)
        {
            var page = pageRefs.GetPageN(k);

            if (page == null)
            {
                continue;
            }

            var contents = GetPdfObject(page.Get(PdfName.Contents));

            if (contents == null)
            {
                continue;
            }

            if (contents.IsStream())
            {
                var refi = (PrIndirectReference)page.Get(PdfName.Contents);

                if (visited.ContainsKey(refi.Number))
                {
                    // need to duplicate
                    newRefs.Add(refi);
                    newStreams.Add(new PrStream((PrStream)contents, null));
                }
                else
                {
                    visited[refi.Number] = 1;
                }
            }
            else if (contents.IsArray())
            {
                var array = (PdfArray)contents;

                for (var j = 0; j < array.Size; ++j)
                {
                    var refi = (PrIndirectReference)array[j];

                    if (visited.ContainsKey(refi.Number))
                    {
                        // need to duplicate
                        newRefs.Add(refi);
                        newStreams.Add(new PrStream((PrStream)GetPdfObject(refi), null));
                    }
                    else
                    {
                        visited[refi.Number] = 1;
                    }
                }
            }
        }

        if (newStreams.Count == 0)
        {
            return;
        }

        for (var k = 0; k < newStreams.Count; ++k)
        {
            _xrefObj.Add(newStreams[k]);
            var refi = newRefs[k];
            refi.SetNumber(_xrefObj.Count - 1, 0);
        }
    }

    /// <summary>
    ///     Gets the box size. Allowed names are: "crop", "trim", "art", "bleed" and "media".
    /// </summary>
    /// <param name="index">the page number. The first page is 1</param>
    /// <param name="boxName">the box name</param>
    /// <returns>the box rectangle or null</returns>
    public Rectangle GetBoxSize(int index, string boxName)
    {
        if (boxName == null)
        {
            throw new ArgumentNullException(nameof(boxName));
        }

        var page = pageRefs.GetPageNRelease(index);
        PdfArray box = null;

        if (boxName.Equals("trim", StringComparison.Ordinal))
        {
            box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.Trimbox));
        }
        else if (boxName.Equals("art", StringComparison.Ordinal))
        {
            box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.Artbox));
        }
        else if (boxName.Equals("bleed", StringComparison.Ordinal))
        {
            box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.Bleedbox));
        }
        else if (boxName.Equals("crop", StringComparison.Ordinal))
        {
            box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.Cropbox));
        }
        else if (boxName.Equals("media", StringComparison.Ordinal))
        {
            box = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.Mediabox));
        }

        if (box == null)
        {
            return null;
        }

        return GetNormalizedRectangle(box);
    }

    /// <summary>
    ///     Gets the certification level for this document. The return values can be  PdfSignatureAppearance.NOT_CERTIFIED ,
    ///     PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED ,
    ///     PdfSignatureAppearance.CERTIFIED_FORM_FILLING  and
    ///     PdfSignatureAppearance.CERTIFIED_FORM_FILLING_AND_ANNOTATIONS .
    ///     No signature validation is made, use the methods availabe for that in  AcroFields .
    /// </summary>
    /// <returns>gets the certification level for this document</returns>
    public int GetCertificationLevel()
    {
        var dic = catalog.GetAsDict(PdfName.Perms);

        if (dic == null)
        {
            return PdfSignatureAppearance.NOT_CERTIFIED;
        }

        dic = dic.GetAsDict(PdfName.Docmdp);

        if (dic == null)
        {
            return PdfSignatureAppearance.NOT_CERTIFIED;
        }

        var arr = dic.GetAsArray(PdfName.Reference);

        if (arr == null || arr.Size == 0)
        {
            return PdfSignatureAppearance.NOT_CERTIFIED;
        }

        dic = arr.GetAsDict(0);

        if (dic == null)
        {
            return PdfSignatureAppearance.NOT_CERTIFIED;
        }

        dic = dic.GetAsDict(PdfName.Transformparams);

        if (dic == null)
        {
            return PdfSignatureAppearance.NOT_CERTIFIED;
        }

        var p = dic.GetAsNumber(PdfName.P);

        if (p == null)
        {
            return PdfSignatureAppearance.NOT_CERTIFIED;
        }

        return p.IntValue;
    }

    /// <summary>
    ///     Gets the crop box without taking rotation into account. This
    ///     is the value of the /CropBox key. The crop box is the part
    ///     of the document to be displayed or printed. It usually is the same
    ///     as the media box but may be smaller. If the page doesn't have a crop
    ///     box the page size will be returned.
    /// </summary>
    /// <param name="index">the page number. The first page is 1</param>
    /// <returns>the crop box</returns>
    public Rectangle GetCropBox(int index)
    {
        var page = pageRefs.GetPageNRelease(index);
        var cropBox = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.Cropbox));

        if (cropBox == null)
        {
            return GetPageSize(page);
        }

        return GetNormalizedRectangle(cropBox);
    }

    public int GetCryptoMode()
    {
        if (decrypt == null)
        {
            return -1;
        }

        return decrypt.GetCryptoMode();
    }

    /// <summary>
    ///     Gets the global document JavaScript.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="file">the document file</param>
    /// <returns>the global document JavaScript</returns>
    public string GetJavaScript(RandomAccessFileOrArray file)
    {
        var names = (PdfDictionary)GetPdfObjectRelease(catalog.Get(PdfName.Names));

        if (names == null)
        {
            return null;
        }

        var js = (PdfDictionary)GetPdfObjectRelease(names.Get(PdfName.Javascript));

        if (js == null)
        {
            return null;
        }

        var jscript = PdfNameTree.ReadTree(js);
        var sortedNames = new string[jscript.Count];
        jscript.Keys.CopyTo(sortedNames, 0);
        Array.Sort(sortedNames);
        var buf = new StringBuilder();

        for (var k = 0; k < sortedNames.Length; ++k)
        {
            var j = (PdfDictionary)GetPdfObjectRelease((PdfIndirectReference)jscript[sortedNames[k]]);

            if (j == null)
            {
                continue;
            }

            var obj = GetPdfObjectRelease(j.Get(PdfName.Js));

            if (obj != null)
            {
                if (obj.IsString())
                {
                    buf.Append(((PdfString)obj).ToUnicodeString()).Append('\n');
                }
                else if (obj.IsStream())
                {
                    var bytes = GetStreamBytes((PrStream)obj, file);

                    if (bytes.Length >= 2 && bytes[0] == 254 && bytes[1] == 255)
                    {
                        buf.Append(PdfEncodings.ConvertToString(bytes, PdfObject.TEXT_UNICODE));
                    }
                    else
                    {
                        buf.Append(PdfEncodings.ConvertToString(bytes, PdfObject.TEXT_PDFDOCENCODING));
                    }

                    buf.Append('\n');
                }
            }
        }

        return buf.ToString();
    }

    public List<PdfAnnotation.PdfImportedLink> GetLinks(int page)
    {
        pageRefs.ResetReleasePage();
        List<PdfAnnotation.PdfImportedLink> result = new();
        var pageDic = pageRefs.GetPageN(page);

        if (pageDic.Get(PdfName.Annots) != null)
        {
            var annots = pageDic.GetAsArray(PdfName.Annots);

            for (var j = 0; j < annots.Size; ++j)
            {
                var annot = (PdfDictionary)GetPdfObjectRelease(annots[j]);

                if (PdfName.Link.Equals(annot.Get(PdfName.Subtype)))
                {
                    result.Add(new PdfAnnotation.PdfImportedLink(annot));
                }
            }
        }

        pageRefs.ReleasePage(page);
        pageRefs.ResetReleasePage();

        return result;
    }

    /// <summary>
    ///     Gets all the named destinations as an  Hashtable . The key is the name
    ///     and the value is the destinations array.
    /// </summary>
    /// <returns>gets all the named destinations</returns>
    public INullValueDictionary<object, PdfObject> GetNamedDestination() => GetNamedDestination(false);

    /// <summary>
    ///     Gets all the named destinations as an  HashMap . The key is the name
    ///     and the value is the destinations array.
    ///     @since   2.1.6
    /// </summary>
    /// <param name="keepNames">true if you want the keys to be real PdfNames instead of Strings</param>
    /// <returns>gets all the named destinations</returns>
    public INullValueDictionary<object, PdfObject> GetNamedDestination(bool keepNames)
    {
        var names = GetNamedDestinationFromNames(keepNames);
        var names2 = GetNamedDestinationFromStrings();

        foreach (var ie in names2)
        {
            names[ie.Key] = ie.Value;
        }

        return names;
    }

    /// <summary>
    ///     Gets the named destinations from the /Dests key in the catalog as an  Hashtable . The key is the name
    ///     and the value is the destinations array.
    /// </summary>
    /// <returns>gets the named destinations</returns>
    public INullValueDictionary<string, PdfObject> GetNamedDestinationFromNames()
    {
        var ret = new NullValueDictionary<string, PdfObject>();

        foreach (var s in GetNamedDestinationFromNames(false))
        {
            ret[(string)s.Key] = s.Value;
        }

        return ret;
    }

    /// <summary>
    ///     Gets the named destinations from the /Dests key in the catalog as an  HashMap . The key is the name
    ///     and the value is the destinations array.
    ///     @since   2.1.6
    /// </summary>
    /// <param name="keepNames">true if you want the keys to be real PdfNames instead of Strings</param>
    /// <returns>gets the named destinations</returns>
    public INullValueDictionary<object, PdfObject> GetNamedDestinationFromNames(bool keepNames)
    {
        var names = new NullValueDictionary<object, PdfObject>();

        if (catalog.Get(PdfName.Dests) != null)
        {
            var dic = (PdfDictionary)GetPdfObjectRelease(catalog.Get(PdfName.Dests));

            if (dic == null)
            {
                return names;
            }

            foreach (var key in dic.Keys)
            {
                var arr = getNameArray(dic.Get(key));

                if (arr == null)
                {
                    continue;
                }

                if (keepNames)
                {
                    names[key] = arr;
                }
                else
                {
                    var name = PdfName.DecodeName(key.ToString());
                    names[name] = arr;
                }
            }
        }

        return names;
    }

    /// <summary>
    ///     Gets the named destinations from the /Names key in the catalog as an  Hashtable . The key is the name
    ///     and the value is the destinations array.
    /// </summary>
    /// <returns>gets the named destinations</returns>
    public INullValueDictionary<string, PdfObject> GetNamedDestinationFromStrings()
    {
        if (catalog.Get(PdfName.Names) != null)
        {
            var dic = (PdfDictionary)GetPdfObjectRelease(catalog.Get(PdfName.Names));

            if (dic != null)
            {
                dic = (PdfDictionary)GetPdfObjectRelease(dic.Get(PdfName.Dests));

                if (dic != null)
                {
                    var names = PdfNameTree.ReadTree(dic);
                    var keys = new string[names.Count];
                    names.Keys.CopyTo(keys, 0);

                    foreach (var key in keys)
                    {
                        var arr = getNameArray(names[key]);

                        if (arr != null)
                        {
                            names[key] = arr;
                        }
                        else
                        {
                            names.Remove(key);
                        }
                    }

                    return names;
                }
            }
        }

        return new NullValueDictionary<string, PdfObject>();
    }

    /// <summary>
    ///     Gets the contents of the page.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="pageNum">the page number. 1 is the first</param>
    /// <param name="file">the location of the PDF document</param>
    /// <returns>the content</returns>
    public byte[] GetPageContent(int pageNum, RandomAccessFileOrArray file)
    {
        var page = GetPageNRelease(pageNum);

        if (page == null)
        {
            return null;
        }

        var contents = GetPdfObjectRelease(page.Get(PdfName.Contents));

        if (contents == null)
        {
            return Array.Empty<byte>();
        }

        if (contents.IsStream())
        {
            return GetStreamBytes((PrStream)contents, file);
        }

        if (contents.IsArray())
        {
            var array = (PdfArray)contents;
            using var bout = new MemoryStream();

            for (var k = 0; k < array.Size; ++k)
            {
                var item = GetPdfObjectRelease(array[k]);

                if (item == null || !item.IsStream())
                {
                    continue;
                }

                var b = GetStreamBytes((PrStream)item, file);
                bout.Write(b, 0, b.Length);

                if (k != array.Size - 1)
                {
                    bout.WriteByte((byte)'\n');
                }
            }

            return bout.ToArray();
        }

        return Array.Empty<byte>();
    }

    /// <summary>
    ///     Gets the contents of the page.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="pageNum">the page number. 1 is the first</param>
    /// <returns>the content</returns>
    public byte[] GetPageContent(int pageNum)
    {
        var rf = SafeFile;

        try
        {
            rf.ReOpen();

            return GetPageContent(pageNum, rf);
        }
        finally
        {
            try
            {
                rf.Close();
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///     Gets the dictionary that represents a page.
    /// </summary>
    /// <param name="pageNum">the page number. 1 is the first</param>
    /// <returns>the page dictionary</returns>
    public PdfDictionary GetPageN(int pageNum)
    {
        var dic = pageRefs.GetPageN(pageNum);

        if (dic == null)
        {
            return null;
        }

        if (_appendable)
        {
            dic.IndRef = pageRefs.GetPageOrigRef(pageNum);
        }

        return dic;
    }

    /// <summary>
    /// </summary>
    /// <param name="pageNum"></param>
    /// <returns>a Dictionary object</returns>
    public PdfDictionary GetPageNRelease(int pageNum)
    {
        var dic = GetPageN(pageNum);
        pageRefs.ReleasePage(pageNum);

        return dic;
    }

    /// <summary>
    ///     Gets the page reference to this page.
    /// </summary>
    /// <param name="pageNum">the page number. 1 is the first</param>
    /// <returns>the page reference</returns>
    public PrIndirectReference GetPageOrigRef(int pageNum) => pageRefs.GetPageOrigRef(pageNum);

    /// <summary>
    ///     Gets the page rotation. This value can be 0, 90, 180 or 270.
    /// </summary>
    /// <param name="index">the page number. The first page is 1</param>
    /// <returns>the page rotation</returns>
    public int GetPageRotation(int index) => GetPageRotation(pageRefs.GetPageNRelease(index));

    /// <summary>
    ///     Gets the page size without taking rotation into account. This
    ///     is the value of the /MediaBox key.
    /// </summary>
    /// <param name="index">the page number. The first page is 1</param>
    /// <returns>the page size</returns>
    public Rectangle GetPageSize(int index) => GetPageSize(pageRefs.GetPageNRelease(index));

    /// <summary>
    ///     Gets the page from a page dictionary
    /// </summary>
    /// <param name="page">the page dictionary</param>
    /// <returns>the page</returns>
    public static Rectangle GetPageSize(PdfDictionary page)
    {
        if (page == null)
        {
            throw new ArgumentNullException(nameof(page));
        }

        var mediaBox = page.GetAsArray(PdfName.Mediabox);

        return GetNormalizedRectangle(mediaBox);
    }

    /// <summary>
    ///     Gets the page size, taking rotation into account. This
    ///     is a  Rectangle  with the value of the /MediaBox and the /Rotate key.
    /// </summary>
    /// <param name="index">the page number. The first page is 1</param>
    /// <returns>a  Rectangle </returns>
    public Rectangle GetPageSizeWithRotation(int index) => GetPageSizeWithRotation(pageRefs.GetPageNRelease(index));

    /// <summary>
    ///     Gets the rotated page from a page dictionary.
    /// </summary>
    /// <param name="page">the page dictionary</param>
    /// <returns>the rotated page</returns>
    public static Rectangle GetPageSizeWithRotation(PdfDictionary page)
    {
        var rect = GetPageSize(page);
        var rotation = GetPageRotation(page);

        while (rotation > 0)
        {
            rect = rect.Rotate();
            rotation -= 90;
        }

        return rect;
    }

    /// <summary>
    /// </summary>
    /// <param name="idx"></param>
    /// <returns>aPdfObject</returns>
    public PdfObject GetPdfObject(int idx)
    {
        _lastXrefPartial = -1;

        if (idx < 0 || idx >= _xrefObj.Count)
        {
            return null;
        }

        var obj = _xrefObj[idx];

        if (!_partial || obj != null)
        {
            return obj;
        }

        if (idx * 2 >= Xref.Length)
        {
            return null;
        }

        obj = ReadSingleObject(idx);
        _lastXrefPartial = -1;

        if (obj != null)
        {
            _lastXrefPartial = idx;
        }

        return obj;
    }

    /// <summary>
    /// </summary>
    /// <param name="idx"></param>
    /// <returns>a PdfObject</returns>
    public PdfObject GetPdfObjectRelease(int idx)
    {
        var obj = GetPdfObject(idx);
        ReleaseLastXrefPartial();

        return obj;
    }

    /// <summary>
    ///     Returns  true  if the PDF has a 128 bit key encryption.
    /// </summary>
    /// <returns> true  if the PDF has a 128 bit key encryption</returns>
    public bool Is128Key() => RValue == 3;

    /// <summary>
    ///     Returns  true  if the PDF is encrypted.
    /// </summary>
    /// <returns> true  if the PDF is encrypted</returns>
    public bool IsEncrypted() => Encrypted;

    /// <summary>
    ///     Getter for property hybridXref.
    /// </summary>
    /// <returns>Value of property hybridXref.</returns>
    public bool IsHybridXref() => _hybridXref;

    public bool IsMetadataEncrypted()
    {
        if (decrypt == null)
        {
            return false;
        }

        return decrypt.IsMetadataEncrypted();
    }

    /// <summary>
    ///     Getter for property newXrefType.
    /// </summary>
    /// <returns>Value of property newXrefType.</returns>
    public bool IsNewXrefType() => NewXrefType;

    /// <summary>
    ///     Checks if the document had errors and was rebuilt.
    /// </summary>
    /// <returns>true if rebuilt.</returns>
    public bool IsRebuilt() => Rebuilt;

    /// <summary>
    /// </summary>
    public void ReleaseLastXrefPartial()
    {
        if (_partial && _lastXrefPartial != -1)
        {
            _xrefObj[_lastXrefPartial] = null;
            _lastXrefPartial = -1;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="pageNum"></param>
    public void ReleasePage(int pageNum) => pageRefs.ReleasePage(pageNum);

    /// <summary>
    ///     Removes all the annotations and fields from the document.
    /// </summary>
    public void RemoveAnnotations()
    {
        pageRefs.ResetReleasePage();

        for (var k = 1; k <= pageRefs.Size; ++k)
        {
            var page = pageRefs.GetPageN(k);

            if (page.Get(PdfName.Annots) == null)
            {
                pageRefs.ReleasePage(k);
            }
            else
            {
                page.Remove(PdfName.Annots);
            }
        }

        catalog.Remove(PdfName.Acroform);
        pageRefs.ResetReleasePage();
    }

    /// <summary>
    ///     Removes all the fields from the document.
    /// </summary>
    public void RemoveFields()
    {
        pageRefs.ResetReleasePage();

        for (var k = 1; k <= pageRefs.Size; ++k)
        {
            var page = pageRefs.GetPageN(k);
            var annots = page.GetAsArray(PdfName.Annots);

            if (annots == null)
            {
                pageRefs.ReleasePage(k);

                continue;
            }

            for (var j = 0; j < annots.Size; ++j)
            {
                var obj = GetPdfObjectRelease(annots[j]);

                if (obj == null || !obj.IsDictionary())
                {
                    continue;
                }

                var annot = (PdfDictionary)obj;

                if (PdfName.Widget.Equals(annot.Get(PdfName.Subtype)))
                {
                    annots.Remove(j--);
                }
            }

            if (annots.IsEmpty())
            {
                page.Remove(PdfName.Annots);
            }
            else
            {
                pageRefs.ReleasePage(k);
            }
        }

        catalog.Remove(PdfName.Acroform);
        pageRefs.ResetReleasePage();
    }

    /// <summary>
    ///     Removes all the unreachable objects.
    /// </summary>
    /// <returns>the number of indirect objects removed</returns>
    public int RemoveUnusedObjects()
    {
        var hits = new bool[_xrefObj.Count];
        RemoveUnusedNode(trailer, hits);
        var total = 0;

        if (_partial)
        {
            for (var k = 1; k < hits.Length; ++k)
            {
                if (!hits[k])
                {
                    Xref[k * 2] = -1;
                    Xref[k * 2 + 1] = 0;
                    _xrefObj[k] = null;
                    ++total;
                }
            }
        }
        else
        {
            for (var k = 1; k < hits.Length; ++k)
            {
                if (!hits[k])
                {
                    _xrefObj[k] = null;
                    ++total;
                }
            }
        }

        return total;
    }

    /// <summary>
    ///     Removes any usage rights that this PDF may have. Only Adobe can grant usage rights
    ///     and any PDF modification with iText will invalidate them. Invalidated usage rights may
    ///     confuse Acrobat and it's advisabe to remove them altogether.
    /// </summary>
    public void RemoveUsageRights()
    {
        var perms = catalog.GetAsDict(PdfName.Perms);

        if (perms == null)
        {
            return;
        }

        perms.Remove(PdfName.Ur);
        perms.Remove(PdfName.Ur3);

        if (perms.Size == 0)
        {
            catalog.Remove(PdfName.Perms);
        }
    }

    /// <summary>
    /// </summary>
    public void ResetLastXrefPartial() => _lastXrefPartial = -1;

    /// <summary>
    /// </summary>
    public void ResetReleasePage() => pageRefs.ResetReleasePage();

    /// <summary>
    ///     Selects the pages to keep in the document. The pages are described as
    ///     ranges. The page ordering can be changed but
    ///     no page repetitions are allowed. Note that it may be very slow in partial mode.
    /// </summary>
    /// <param name="ranges">the comma separated ranges as described in {@link SequenceList}</param>
    public void SelectPages(string ranges) => SelectPages(SequenceList.Expand(ranges, NumberOfPages));

    /// <summary>
    ///     Selects the pages to keep in the document. The pages are described as a
    ///     List  of  Integer . The page ordering can be changed but
    ///     no page repetitions are allowed. Note that it may be very slow in partial mode.
    /// </summary>
    /// <param name="pagesToKeep">the pages to keep in the document</param>
    public void SelectPages(ICollection<int> pagesToKeep)
    {
        if (pagesToKeep == null)
        {
            throw new ArgumentNullException(nameof(pagesToKeep));
        }

        pageRefs.SelectPages(pagesToKeep);
        RemoveUnusedObjects();
    }

    /// <summary>
    ///     Sets the contents of the page.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="content">the new page content</param>
    /// <param name="pageNum">the page number. 1 is the first</param>
    public void SetPageContent(int pageNum, byte[] content)
        => SetPageContent(pageNum, content, PdfStream.DEFAULT_COMPRESSION);

    /// <summary>
    ///     Sets the contents of the page.
    ///     @since   2.1.3   (the method already existed without param compressionLevel)
    /// </summary>
    /// <param name="content">the new page content</param>
    /// <param name="pageNum">the page number. 1 is the first</param>
    /// <param name="compressionLevel"></param>
    public void SetPageContent(int pageNum, byte[] content, int compressionLevel)
    {
        var page = GetPageN(pageNum);

        if (page == null)
        {
            return;
        }

        var contents = page.Get(PdfName.Contents);
        FreeXref = -1;
        KillXref(contents);

        if (FreeXref == -1)
        {
            _xrefObj.Add(null);
            FreeXref = _xrefObj.Count - 1;
        }

        page.Put(PdfName.Contents, new PrIndirectReference(this, FreeXref));
        _xrefObj[FreeXref] = new PrStream(this, content, compressionLevel);
    }

    /// <summary>
    ///     Finds all the font subsets and changes the prefixes to some
    ///     random values.
    /// </summary>
    /// <returns>the number of font subsets altered</returns>
    public int ShuffleSubsetNames()
    {
        var total = 0;

        for (var k = 1; k < _xrefObj.Count; ++k)
        {
            var obj = GetPdfObjectRelease(k);

            if (obj == null || !obj.IsDictionary())
            {
                continue;
            }

            var dic = (PdfDictionary)obj;

            if (!ExistsName(dic, PdfName.TYPE, PdfName.Font))
            {
                continue;
            }

            if (ExistsName(dic, PdfName.Subtype, PdfName.Type1) || ExistsName(dic, PdfName.Subtype, PdfName.Mmtype1) ||
                ExistsName(dic, PdfName.Subtype, PdfName.Truetype))
            {
                var s = GetSubsetPrefix(dic);

                if (s == null)
                {
                    continue;
                }

                var ns = BaseFont.CreateSubsetPrefix() + s.Substring(7);
                var newName = new PdfName(ns);
                dic.Put(PdfName.Basefont, newName);
                setXrefPartialObject(k, dic);
                ++total;
                var fd = dic.GetAsDict(PdfName.Fontdescriptor);

                if (fd == null)
                {
                    continue;
                }

                fd.Put(PdfName.Fontname, newName);
            }
            else if (ExistsName(dic, PdfName.Subtype, PdfName.Type0))
            {
                var s = GetSubsetPrefix(dic);
                var arr = dic.GetAsArray(PdfName.Descendantfonts);

                if (arr == null)
                {
                    continue;
                }

                if (arr.IsEmpty())
                {
                    continue;
                }

                var desc = arr.GetAsDict(0);
                var sde = GetSubsetPrefix(desc);

                if (sde == null)
                {
                    continue;
                }

                var ns = BaseFont.CreateSubsetPrefix();

                if (s != null)
                {
                    dic.Put(PdfName.Basefont, new PdfName(ns + s.Substring(7)));
                }

                setXrefPartialObject(k, dic);
                var newName = new PdfName(ns + sde.Substring(7));
                desc.Put(PdfName.Basefont, newName);
                ++total;
                var fd = desc.GetAsDict(PdfName.Fontdescriptor);

                if (fd == null)
                {
                    continue;
                }

                fd.Put(PdfName.Fontname, newName);
            }
        }

        return total;
    }

    internal static bool Equalsn(byte[] a1, byte[] a2)
    {
        var length = a2.Length;

        for (var k = 0; k < length; ++k)
        {
            if (a1[k] != a2[k])
            {
                return false;
            }
        }

        return true;
    }

    internal static bool ExistsName(PdfDictionary dic, PdfName key, PdfName value)
    {
        var type = GetPdfObjectRelease(dic.Get(key));

        if (type == null || !type.IsName())
        {
            return false;
        }

        var name = (PdfName)type;

        return name.Equals(value);
    }

    internal static string GetFontName(PdfDictionary dic)
    {
        if (dic == null)
        {
            return null;
        }

        var type = GetPdfObjectRelease(dic.Get(PdfName.Basefont));

        if (type == null || !type.IsName())
        {
            return null;
        }

        return PdfName.DecodeName(type.ToString());
    }

    internal static string GetSubsetPrefix(PdfDictionary dic)
    {
        if (dic == null)
        {
            return null;
        }

        var s = GetFontName(dic);

        if (s == null)
        {
            return null;
        }

        if (s.Length < 8 || s[6] != '+')
        {
            return null;
        }

        for (var k = 0; k < 6; ++k)
        {
            var c = s[k];

            if (c < 'A' || c > 'Z')
            {
                return null;
            }
        }

        return s;
    }

    internal PdfIndirectReference GetCryptoRef()
    {
        if (_cryptoRef == null)
        {
            return null;
        }

        return new PdfIndirectReference(0, _cryptoRef.Number, _cryptoRef.Generation);
    }

    internal static int GetPageRotation(PdfDictionary page)
    {
        var rotate = page.GetAsNumber(PdfName.Rotate);

        if (rotate == null)
        {
            return 0;
        }

        var n = rotate.IntValue;
        n %= 360;

        return n < 0 ? n + 360 : n;
    }

    internal virtual void SetViewerPreferences(PdfViewerPreferencesImp vp) => vp.AddToCatalog(catalog);

    protected internal static PdfDictionary DuplicatePdfDictionary(PdfDictionary original,
        PdfDictionary copy,
        PdfReader newReader)
    {
        if (original == null)
        {
            throw new ArgumentNullException(nameof(original));
        }

        if (copy == null)
        {
            copy = new PdfDictionary();
        }

        foreach (var key in original.Keys)
        {
            copy.Put(key, DuplicatePdfObject(original.Get(key), newReader));
        }

        return copy;
    }

    protected internal static PdfObject DuplicatePdfObject(PdfObject original, PdfReader newReader)
    {
        if (original == null)
        {
            return null;
        }

        switch (original.Type)
        {
            case PdfObject.DICTIONARY:
            {
                return DuplicatePdfDictionary((PdfDictionary)original, null, newReader);
            }
            case PdfObject.STREAM:
            {
                var org = (PrStream)original;
                var stream = new PrStream(org, null, newReader);
                DuplicatePdfDictionary(org, stream, newReader);

                return stream;
            }
            case PdfObject.ARRAY:
            {
                var arr = new PdfArray();

                for (var it = ((PdfArray)original).GetListIterator(); it.HasNext();)
                {
                    arr.Add(DuplicatePdfObject(it.Next(), newReader));
                }

                return arr;
            }
            case PdfObject.INDIRECT:
            {
                var org = (PrIndirectReference)original;

                return new PrIndirectReference(newReader, org.Number, org.Generation);
            }
            default:
                return original;
        }
    }

    protected internal PdfReaderInstance GetPdfReaderInstance(PdfWriter writer) => new(this, writer);

    protected internal void KillXref(PdfObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (obj is PdfIndirectReference && !obj.IsIndirect())
        {
            return;
        }

        switch (obj.Type)
        {
            case PdfObject.INDIRECT:
            {
                var xr = ((PrIndirectReference)obj).Number;
                obj = _xrefObj[xr];
                _xrefObj[xr] = null;
                FreeXref = xr;
                KillXref(obj);

                break;
            }
            case PdfObject.ARRAY:
            {
                var t = (PdfArray)obj;

                for (var i = 0; i < t.Size; ++i)
                {
                    KillXref(t[i]);
                }

                break;
            }
            case PdfObject.STREAM:
            case PdfObject.DICTIONARY:
            {
                var dic = (PdfDictionary)obj;

                foreach (var key in dic.Keys)
                {
                    KillXref(dic.Get(key));
                }

                break;
            }
        }
    }

    protected internal PdfArray ReadArray()
    {
        var array = new PdfArray();

        for (var i = 0; i < 2000000; i++) //Limit for bad files that cause loop
        {
            _iPRObjectDepth++;

            if (_iPRObjectDepth > 1000) //Check that recursion depth from ReadPrObject calls isn't too much
            {
                break;
            }

            var obj = ReadPrObject();
            _iPRObjectDepth--;
            var type = obj.Type;

            if (-type == PrTokeniser.TK_END_ARRAY)
            {
                break;
            }

            if (-type == PrTokeniser.TK_END_DIC)
            {
                Tokens.ThrowError("Unexpected '>>'");
            }

            array.Add(obj);
        }

        return array;
    }

    protected internal PdfDictionary ReadDictionary()
    {
        var dic = new PdfDictionary();

        for (var i = 0; i < 1000000; i++) //Limit for bad files that cause loop
        {
            i++;
            Tokens.NextValidToken();

            if (Tokens.TokenType == PrTokeniser.TK_END_DIC)
            {
                break;
            }

            if (Tokens.TokenType != PrTokeniser.TK_NAME)
            {
                Tokens.ThrowError("Dictionary key is not a name.");
            }

            var name = new PdfName(Tokens.StringValue, false);
            var obj = ReadPrObject();
            var type = obj.Type;

            if (-type == PrTokeniser.TK_END_DIC)
            {
                Tokens.ThrowError("Unexpected '>>'");
            }

            if (-type == PrTokeniser.TK_END_ARRAY)
            {
                Tokens.ThrowError("Unexpected ']'");
            }

            dic.Put(name, obj);
        }

        return dic;
    }

    protected internal void ReadDocObj()
    {
        List<PrStream> streams = new();
        _xrefObj = new List<PdfObject>(Xref.Length / 2);

        for (var k = 0; k < Xref.Length / 2; ++k)
        {
            _xrefObj.Add(null);
        }

        for (var k = 2; k < Xref.Length; k += 2)
        {
            var pos = Xref[k];

            if (pos <= 0 || Xref[k + 1] > 0)
            {
                continue;
            }

            Tokens.Seek(pos);
            Tokens.NextValidToken();

            if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
            {
                Tokens.ThrowError("Invalid object number.");
            }

            _objNum = Tokens.IntValue;
            Tokens.NextValidToken();

            if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
            {
                Tokens.ThrowError("Invalid generation number.");
            }

            _objGen = Tokens.IntValue;
            Tokens.NextValidToken();

            if (!Tokens.StringValue.Equals("obj", StringComparison.Ordinal))
            {
                Tokens.ThrowError("Token 'obj' expected.");
            }

            PdfObject obj;

            try
            {
                obj = ReadPrObject();

                if (obj.IsStream())
                {
                    streams.Add((PrStream)obj);
                }
            }
            catch
            {
                obj = null;
            }

            _xrefObj[k / 2] = obj;
        }

        for (var k = 0; k < streams.Count; ++k)
        {
            checkPrStreamLength(streams[k]);
        }

        readDecryptedDocObj();

        if (ObjStmMark != null)
        {
            foreach (var entry in ObjStmMark)
            {
                var n = entry.Key;
                var h = entry.Value;
                ReadObjStm((PrStream)_xrefObj[n], h);
                _xrefObj[n] = null;
            }

            ObjStmMark = null;
        }

        Xref = null;
    }

    protected internal void ReadDocObjPartial()
    {
        _xrefObj = new List<PdfObject>(Xref.Length / 2);

        for (var k = 0; k < Xref.Length / 2; ++k)
        {
            _xrefObj.Add(null);
        }

        readDecryptedDocObj();

        if (ObjStmToOffset != null)
        {
            var keys = ObjStmToOffset.GetKeys();

            for (var k = 0; k < keys.Count; ++k)
            {
                var n = keys[k];
                ObjStmToOffset[n] = Xref[n * 2];
                Xref[n * 2] = -1;
            }
        }
    }

    protected internal void ReadObjStm(PrStream stream, NullValueDictionary<int, int> map)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (map == null)
        {
            throw new ArgumentNullException(nameof(map));
        }

        var first = stream.GetAsNumber(PdfName.First).IntValue;
        var n = stream.GetAsNumber(PdfName.N).IntValue;
        var b = GetStreamBytes(stream, Tokens.File);
        var saveTokens = Tokens;
        Tokens = new PrTokeniser(b);

        try
        {
            var address = new int[n];
            var objNumber = new int[n];
            var ok = true;

            for (var k = 0; k < n; ++k)
            {
                ok = Tokens.NextToken();

                if (!ok)
                {
                    break;
                }

                if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
                {
                    ok = false;

                    break;
                }

                objNumber[k] = Tokens.IntValue;
                ok = Tokens.NextToken();

                if (!ok)
                {
                    break;
                }

                if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
                {
                    ok = false;

                    break;
                }

                address[k] = Tokens.IntValue + first;
            }

            if (!ok)
            {
                throw new InvalidPdfException("Error reading ObjStm");
            }

            for (var k = 0; k < n; ++k)
            {
                if (map.ContainsKey(k))
                {
                    Tokens.Seek(address[k]);
                    Tokens.NextToken();
                    PdfObject obj;

                    if (Tokens.TokenType == PrTokeniser.TK_NUMBER)
                    {
                        obj = new PdfNumber(Tokens.StringValue);
                    }
                    else
                    {
                        Tokens.Seek(address[k]);
                        obj = ReadPrObject();
                    }

                    _xrefObj[objNumber[k]] = obj;
                }
            }
        }
        finally
        {
            Tokens = saveTokens;
        }
    }

    protected internal PdfObject ReadOneObjStm(PrStream stream, int idx)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var first = stream.GetAsNumber(PdfName.First).IntValue;
        var b = GetStreamBytes(stream, Tokens.File);
        var saveTokens = Tokens;
        Tokens = new PrTokeniser(b);

        try
        {
            var address = 0;
            var ok = true;
            ++idx;

            for (var k = 0; k < idx; ++k)
            {
                ok = Tokens.NextToken();

                if (!ok)
                {
                    break;
                }

                if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
                {
                    ok = false;

                    break;
                }

                ok = Tokens.NextToken();

                if (!ok)
                {
                    break;
                }

                if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
                {
                    ok = false;

                    break;
                }

                address = Tokens.IntValue + first;
            }

            if (!ok)
            {
                throw new InvalidPdfException("Error reading ObjStm");
            }

            Tokens.Seek(address);

            return ReadPrObject();
        }
        finally
        {
            Tokens = saveTokens;
        }
    }

    protected internal void ReadPages()
    {
        catalog = trailer?.GetAsDict(PdfName.Root);

        if (catalog == null)
        {
            throw new InvalidPdfException("This invalid PDF file doesn't have a catalog.");
        }

        _rootPages = catalog.GetAsDict(PdfName.Pages);

        if (_rootPages == null || !HasRootPage())
        {
            throw new InvalidPdfException("This invalid PDF file doesn't have a page root.");
        }

        pageRefs = new PageRefs(this);
    }

    private bool HasRootPage()
    {
        var type = _rootPages.Get(PdfName.TYPE);
        var types = _rootPages.Get(PdfName.TYPES);
        
        return PdfName.Pages.Equals(type) || PdfName.Pages.Equals(types) || IsSimplifiedPageDictionaryRootPage();
    }

    private bool IsSimplifiedPageDictionaryRootPage() =>
        _rootPages.Get(PdfName.Kids) != null;

    protected internal virtual void ReadPdf()
    {
        try
        {
            FileLength = Tokens.File.Length;
            pdfVersion = Tokens.CheckPdfHeader();

            try
            {
                if (_bBailout == false)
                {
                    ReadXref();
                }
            }
            catch (Exception e)
            {
                try
                {
                    Rebuilt = true;
                    RebuildXref();
                    lastXref = -1;
                }
                catch (Exception ne)
                {
                    throw new InvalidPdfException("Rebuild failed: " + ne.Message + "; Original message: " + e.Message);
                }
            }

            try
            {
                if (_bBailout == false)
                {
                    ReadDocObj();
                }
            }
            catch (Exception ne)
            {
                if (ne is BadPasswordException)
                {
                    throw new BadPasswordException(ne.Message);
                }

                if (Rebuilt || _encryptionError)
                {
                    throw;
                }

                Rebuilt = true;
                Encrypted = false;
                RebuildXref();
                lastXref = -1;
                ReadDocObj();
            }

            Strings.Clear();
            ReadPages();
            EliminateSharedStreams();
            RemoveUnusedObjects();
        }
        finally
        {
            try
            {
                Tokens.Close();
            }
            catch
            {
                // empty on purpose
            }
        }
    }

    protected internal void ReadPdfPartial()
    {
        try
        {
            FileLength = Tokens.File.Length;
            pdfVersion = Tokens.CheckPdfHeader();

            try
            {
                if (_bBailout == false)
                {
                    ReadXref();
                }
            }
            catch (Exception e)
            {
                try
                {
                    Rebuilt = true;
                    RebuildXref();
                    lastXref = -1;
                }
                catch (Exception ne)
                {
                    throw new InvalidPdfException("Rebuild failed: " + ne.Message + "; Original message: " + e.Message);
                }
            }

            if (_bBailout == false)
            {
                ReadDocObjPartial();
                ReadPages();
            }
        }
        catch (IOException)
        {
            try
            {
                Tokens.Close();
            }
            catch
            {
            }

            throw;
        }
    }

    protected internal PdfObject ReadPrObject()
    {
        Tokens.NextValidToken();
        var type = Tokens.TokenType;

        switch (type)
        {
            case PrTokeniser.TK_START_DIC:
            {
                ++_readDepth;
                var dic = ReadDictionary();
                --_readDepth;
                var pos = Tokens.FilePointer;

                // be careful in the trailer. May not be a "next" token.
                bool hasNext;

                do
                {
                    hasNext = Tokens.NextToken();
                }
                while (hasNext && Tokens.TokenType == PrTokeniser.TK_COMMENT);

                if (hasNext && Tokens.StringValue.Equals("stream", StringComparison.Ordinal))
                {
                    //skip whitespaces
                    int ch;

                    do
                    {
                        ch = Tokens.Read();
                    }
                    while (ch == 32 || ch == 9 || ch == 0 || ch == 12);

                    if (ch != '\n')
                    {
                        ch = Tokens.Read();
                    }

                    if (ch != '\n')
                    {
                        Tokens.BackOnePosition(ch);
                    }

                    var stream = new PrStream(this, Tokens.FilePointer);
                    stream.Merge(dic);
                    stream.ObjNum = _objNum;
                    stream.ObjGen = _objGen;

                    return stream;
                }

                Tokens.Seek(pos);

                return dic;
            }
            case PrTokeniser.TK_START_ARRAY:
            {
                ++_readDepth;
                var arr = ReadArray();
                --_readDepth;

                return arr;
            }
            case PrTokeniser.TK_NUMBER:
                return new PdfNumber(Tokens.StringValue);
            case PrTokeniser.TK_STRING:
                var str = new PdfString(Tokens.StringValue, null).SetHexWriting(Tokens.IsHexString());
                str.SetObjNum(_objNum, _objGen);

                if (Strings != null)
                {
                    Strings.Add(str);
                }

                return str;
            case PrTokeniser.TK_NAME:
            {
                var cachedName = PdfName.StaticNames[Tokens.StringValue];

                if (_readDepth > 0 && cachedName != null)
                {
                    return cachedName;
                }

                // an indirect name (how odd...), or a non-standard one
                return new PdfName(Tokens.StringValue, false);
            }
            case PrTokeniser.TK_REF:
                var num = Tokens.Reference;
                var refi = new PrIndirectReference(this, num, Tokens.Generation);

                return refi;
            default:
                var sv = Tokens.StringValue;

                if ("null".Equals(sv, StringComparison.Ordinal))
                {
                    if (_readDepth == 0)
                    {
                        return new PdfNull();
                    } //else

                    return PdfNull.Pdfnull;
                }

                if ("true".Equals(sv, StringComparison.Ordinal))
                {
                    if (_readDepth == 0)
                    {
                        return new PdfBoolean(true);
                    } //else

                    return PdfBoolean.Pdftrue;
                }

                if ("false".Equals(sv, StringComparison.Ordinal))
                {
                    if (_readDepth == 0)
                    {
                        return new PdfBoolean(false);
                    } //else

                    return PdfBoolean.Pdffalse;
                }

                return new PdfLiteral(-type, Tokens.StringValue);
        }
    }

    protected internal PdfObject ReadSingleObject(int k)
    {
        Strings.Clear();
        var k2 = k * 2;
        var pos = Xref[k2];

        if (pos < 0)
        {
            return null;
        }

        if (Xref[k2 + 1] > 0)
        {
            pos = ObjStmToOffset[Xref[k2 + 1]];
        }

        if (pos == 0)
        {
            return null;
        }

        Tokens.Seek(pos);
        Tokens.NextValidToken();

        if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
        {
            Tokens.ThrowError("Invalid object number.");
        }

        _objNum = Tokens.IntValue;
        Tokens.NextValidToken();

        if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
        {
            Tokens.ThrowError("Invalid generation number.");
        }

        _objGen = Tokens.IntValue;
        Tokens.NextValidToken();

        if (!Tokens.StringValue.Equals("obj", StringComparison.Ordinal))
        {
            Tokens.ThrowError("Token 'obj' expected.");
        }

        PdfObject obj;

        try
        {
            obj = ReadPrObject();

            for (var j = 0; j < Strings.Count; ++j)
            {
                var str = Strings[j];
                str.Decrypt(this);
            }

            if (obj.IsStream())
            {
                checkPrStreamLength((PrStream)obj);
            }
        }
        catch
        {
            obj = null;
        }

        if (Xref[k2 + 1] > 0)
        {
            obj = ReadOneObjStm((PrStream)obj, Xref[k2]);
        }

        _xrefObj[k] = obj;

        return obj;
    }

    protected internal void ReadXref()
    {
        _hybridXref = false;
        NewXrefType = false;
        Tokens.Seek((int)Tokens.Startxref);
        Tokens.NextToken();

        if (!Tokens.StringValue.Equals("startxref", StringComparison.Ordinal))
        {
            throw new InvalidPdfException("startxref not found.");
        }

        Tokens.NextToken();

        if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
        {
            throw new InvalidPdfException("startxref is not followed by a number.");
        }

        var startxref = Tokens.IntValue;
        lastXref = startxref;
        eofPos = Tokens.FilePointer;

        try
        {
            if (ReadXRefStream(startxref))
            {
                NewXrefType = true;

                return;
            }
        }
        catch
        {
        }

        xrefByteOffset.Add(startxref);

        if (_bBailout == false)
        {
            Xref = null;
            Tokens.Seek(startxref);
            trailer = ReadXrefSection();
            var trailer2 = trailer;

            while (true)
            {
                var prev = (PdfNumber)trailer2.Get(PdfName.Prev);

                if (prev == null)
                {
                    break;
                }

                xrefByteOffset.Add(prev.IntValue);
                Tokens.Seek(prev.IntValue);
                trailer2 = ReadXrefSection();
            }
        }
    }

    protected internal PdfDictionary ReadXrefSection()
    {
        Tokens.NextValidToken();

        if (!Tokens.StringValue.Equals("xref", StringComparison.Ordinal))
        {
            Tokens.ThrowError("xref subsection not found");
        }

        var start = 0;
        var end = 0;
        var pos = 0;
        var gen = 0;

        while (true)
        {
            Tokens.NextValidToken();

            if (Tokens.StringValue.Equals("trailer", StringComparison.Ordinal))
            {
                break;
            }

            if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
            {
                Tokens.ThrowError("Object number of the first object in this xref subsection not found");
            }

            start = Tokens.IntValue;
            Tokens.NextValidToken();

            if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
            {
                Tokens.ThrowError("Number of entries in this xref subsection not found");
            }

            end = Tokens.IntValue + start;

            if (start == 1)
            {
                // fix incorrect start number
                var back = Tokens.FilePointer;
                Tokens.NextValidToken();
                pos = Tokens.IntValue;
                Tokens.NextValidToken();
                gen = Tokens.IntValue;

                if (pos == 0 && gen == PdfWriter.GENERATION_MAX)
                {
                    --start;
                    --end;
                }

                Tokens.Seek(back);
            }

            ensureXrefSize(end * 2);

            for (var k = start; k < end; ++k)
            {
                Tokens.NextValidToken();
                pos = Tokens.IntValue;
                Tokens.NextValidToken();
                gen = Tokens.IntValue;
                Tokens.NextValidToken();
                var p = k * 2;

                if (Tokens.StringValue.Equals("n", StringComparison.Ordinal))
                {
                    if (Xref[p] == 0 && Xref[p + 1] == 0)
                    {
                        //                        if (pos == 0)
                        //                            tokens.ThrowError("File position 0 cross-reference entry in this xref subsection");
                        Xref[p] = pos;
                    }
                }
                else if (Tokens.StringValue.Equals("f", StringComparison.Ordinal))
                {
                    if (Xref[p] == 0 && Xref[p + 1] == 0)
                    {
                        Xref[p] = -1;
                    }
                }
                else
                {
                    Tokens.ThrowError("Invalid cross-reference entry in this xref subsection");
                }
            }
        }

        var localTrailer = (PdfDictionary)ReadPrObject();
        var xrefSize = (PdfNumber)localTrailer.Get(PdfName.Size);
        ensureXrefSize(xrefSize.IntValue * 2);
        var xrs = localTrailer.Get(PdfName.Xrefstm);

        if (xrs != null && xrs.IsNumber())
        {
            var loc = ((PdfNumber)xrs).IntValue;

            try
            {
                ReadXRefStream(loc);
                NewXrefType = true;
                _hybridXref = true;
            }
            catch (IOException)
            {
                Xref = null;

                throw;
            }
        }

        return localTrailer;
    }

    protected internal bool ReadXRefStream(int ptr)
    {
        Tokens.Seek(ptr);
        var thisStream = 0;

        if (!Tokens.NextToken())
        {
            return false;
        }

        if (Tokens.TokenType != PrTokeniser.TK_NUMBER)
        {
            return false;
        }

        thisStream = Tokens.IntValue;

        if (!Tokens.NextToken() || Tokens.TokenType != PrTokeniser.TK_NUMBER)
        {
            return false;
        }

        if (!Tokens.NextToken() || !Tokens.StringValue.Equals("obj", StringComparison.Ordinal))
        {
            return false;
        }

        var objecto = ReadPrObject();
        PrStream stm = null;

        if (objecto.IsStream())
        {
            stm = (PrStream)objecto;

            if (!PdfName.Xref.Equals(stm.Get(PdfName.TYPE)))
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        if (trailer == null)
        {
            trailer = new PdfDictionary();
            trailer.Merge(stm);
        }

        stm.Length = ((PdfNumber)stm.Get(PdfName.LENGTH)).IntValue;
        var size = ((PdfNumber)stm.Get(PdfName.Size)).IntValue;
        PdfArray index;
        var obj = stm.Get(PdfName.Index);

        if (obj == null)
        {
            index = new PdfArray();

            index.Add(new[]
            {
                0, size
            });
        }
        else
        {
            index = (PdfArray)obj;
        }

        var w = (PdfArray)stm.Get(PdfName.W);
        xrefByteOffset.Add(ptr);
        var prev = -1;
        obj = stm.Get(PdfName.Prev);

        if (obj != null)
        {
            prev = ((PdfNumber)obj).IntValue;
        }

        // Each xref pair is a position
        // type 0 -> -1, 0
        // type 1 -> offset, 0
        // type 2 -> index, obj num
        ensureXrefSize(size * 2);

        if (ObjStmMark == null && !_partial)
        {
            ObjStmMark = new NullValueDictionary<int, NullValueDictionary<int, int>>();
        }

        if (ObjStmToOffset == null && _partial)
        {
            ObjStmToOffset = new NullValueDictionary<int, int>();
        }

        var b = GetStreamBytes(stm, Tokens.File);
        var bptr = 0;
        var wc = new int[3];

        for (var k = 0; k < 3; ++k)
        {
            wc[k] = w.GetAsNumber(k).IntValue;
        }

        for (var idx = 0; idx < index.Size; idx += 2)
        {
            var start = index.GetAsNumber(idx).IntValue;
            var length = index.GetAsNumber(idx + 1).IntValue;
            ensureXrefSize((start + length) * 2);

            while (length-- > 0)
            {
                var type = 1;

                if (wc[0] > 0)
                {
                    type = 0;

                    for (var k = 0; k < wc[0]; ++k)
                    {
                        type = (type << 8) + (b[bptr++] & 0xff);
                    }
                }

                var field2 = 0;

                for (var k = 0; k < wc[1]; ++k)
                {
                    field2 = (field2 << 8) + (b[bptr++] & 0xff);
                }

                var field3 = 0;

                for (var k = 0; k < wc[2]; ++k)
                {
                    field3 = (field3 << 8) + (b[bptr++] & 0xff);
                }

                var baseb = start * 2;

                if (Xref[baseb] == 0 && Xref[baseb + 1] == 0)
                {
                    switch (type)
                    {
                        case 0:
                            Xref[baseb] = -1;

                            break;
                        case 1:
                            Xref[baseb] = field2;

                            break;
                        case 2:
                            Xref[baseb] = field3;
                            Xref[baseb + 1] = field2;

                            if (_partial)
                            {
                                ObjStmToOffset[field2] = 0;
                            }
                            else
                            {
                                var seq = ObjStmMark[field2];

                                if (seq == null)
                                {
                                    seq = new NullValueDictionary<int, int>();
                                    seq[field3] = 1;
                                    ObjStmMark[field2] = seq;
                                }
                                else
                                {
                                    seq[field3] = 1;
                                }
                            }

                            break;
                    }
                }

                ++start;
            }
        }

        thisStream *= 2;

        if (thisStream < Xref.Length)
        {
            Xref[thisStream] = -1;
        }

        if (prev == -1)
        {
            return true;
        }

        //before we go on, let's make sure we haven't done this a number of times that indicates a problematic recursion loop
        if ((new StackTrace().GetFrames() ?? Array.Empty<StackFrame>()).Count(frame => frame.GetMethod().Name ==
                nameof(ReadXRefStream)) > 200)
        {
            _bBailout = true;

            throw new InvalidOperationException("Likely recursion loop issue.");
        }

        return ReadXRefStream(prev);
    }

    protected internal void RebuildXref()
    {
        _hybridXref = false;
        NewXrefType = false;
        Tokens.Seek(0);
        var xr = new int[1024][];
        var top = 0;
        trailer = null;
        var line = new byte[64];

        for (;;)
        {
            var pos = Tokens.FilePointer;

            if (!Tokens.ReadLineSegment(line))
            {
                break;
            }

            if (line[0] == 't')
            {
                var lineStr = PdfEncodings.ConvertToString(line, null);

                if (!lineStr.StartsWith("trailer", StringComparison.Ordinal))
                {
                    continue;
                }

                Tokens.Seek(pos);
                Tokens.NextToken();
                pos = Tokens.FilePointer;

                try
                {
                    var dic = (PdfDictionary)ReadPrObject();

                    if (dic.Get(PdfName.Root) != null)
                    {
                        trailer = dic;
                    }
                    else
                    {
                        Tokens.Seek(pos);
                    }
                }
                catch
                {
                    Tokens.Seek(pos);
                }
            }
            else if (line[0] >= '0' && line[0] <= '9')
            {
                var obj = PrTokeniser.CheckObjectStart(line);

                if (obj == null)
                {
                    continue;
                }

                var num = obj[0];
                var gen = obj[1];

                if (num >= xr.Length)
                {
                    var newLength = num * 2;
                    var xr2 = new int[newLength][];
                    Array.Copy(xr, 0, xr2, 0, top);
                    xr = xr2;
                }

                if (num >= top)
                {
                    top = num + 1;
                }

                if (xr[num] == null || gen >= xr[num][1])
                {
                    obj[0] = pos;
                    xr[num] = obj;
                }
            }
        }

        if (trailer == null)
        {
            throw new InvalidPdfException("trailer not found.");
        }

        Xref = new int[top * 2];

        for (var k = 0; k < top; ++k)
        {
            var obj = xr[k];

            if (obj != null)
            {
                Xref[k * 2] = obj[0];
            }
        }
    }

    protected internal void RemoveUnusedNode(PdfObject obj, bool[] hits)
    {
        if (hits == null)
        {
            throw new ArgumentNullException(nameof(hits));
        }

        var state = new Stack<object>();
        state.Push(obj);

        while (state.Count != 0)
        {
            var current = state.Pop();

            if (current == null)
            {
                continue;
            }

            IList<PdfObject> ar = null;
            PdfDictionary dic = null;
            PdfName[] keys = null;
            object[] objs = null;
            var idx = 0;

            if (current is PdfObject)
            {
                obj = (PdfObject)current;

                switch (obj.Type)
                {
                    case PdfObject.DICTIONARY:
                    case PdfObject.STREAM:
                        dic = (PdfDictionary)obj;
                        keys = new PdfName[dic.Size];
                        dic.Keys.CopyTo(keys, 0);

                        break;
                    case PdfObject.ARRAY:
                        ar = ((PdfArray)obj).ArrayList;

                        break;
                    case PdfObject.INDIRECT:
                        var refi = (PrIndirectReference)obj;
                        var num = refi.Number;

                        if (num > 0 && !hits[num])
                        {
                            hits[num] = true;
                            state.Push(GetPdfObjectRelease(refi));
                        }

                        continue;
                    default:
                        continue;
                }
            }
            else
            {
                objs = (object[])current;

                if (objs[0] is List<PdfObject>)
                {
                    ar = (List<PdfObject>)objs[0];
                    idx = (int)objs[1];
                }
                else
                {
                    keys = (PdfName[])objs[0];
                    dic = (PdfDictionary)objs[1];
                    idx = (int)objs[2];
                }
            }

            if (ar != null)
            {
                for (var k = idx; k < ar.Count; ++k)
                {
                    var v = ar[k];

                    if (v.IsIndirect())
                    {
                        var num = ((PrIndirectReference)v).Number;

                        if (num > 0 && (num >= _xrefObj.Count || (!_partial && _xrefObj[num] == null)))
                        {
                            ar[k] = PdfNull.Pdfnull;

                            continue;
                        }
                    }

                    if (objs == null)
                    {
                        state.Push(new object[]
                        {
                            ar, k + 1
                        });
                    }
                    else
                    {
                        objs[1] = k + 1;
                        state.Push(objs);
                    }

                    state.Push(v);

                    break;
                }
            }
            else
            {
                for (var k = idx; k < keys.Length; ++k)
                {
                    var key = keys[k];
                    var v = dic.Get(key);

                    if (v.IsIndirect())
                    {
                        var num = ((PrIndirectReference)v).Number;

                        if (num > 0 && (num >= _xrefObj.Count || (!_partial && _xrefObj[num] == null)))
                        {
                            dic.Put(key, PdfNull.Pdfnull);

                            continue;
                        }
                    }

                    if (objs == null)
                    {
                        state.Push(new object[]
                        {
                            keys, dic, k + 1
                        });
                    }
                    else
                    {
                        objs[2] = k + 1;
                        state.Push(objs);
                    }

                    state.Push(v);

                    break;
                }
            }
        }
    }

    private static PdfArray getNameArray(PdfObject obj)
    {
        if (obj == null)
        {
            return null;
        }

        obj = GetPdfObjectRelease(obj);

        if (obj == null)
        {
            return null;
        }

        if (obj.IsArray())
        {
            return (PdfArray)obj;
        }

        if (obj.IsDictionary())
        {
            var arr2 = GetPdfObjectRelease(((PdfDictionary)obj).Get(PdfName.D));

            if (arr2 != null && arr2.IsArray())
            {
                return (PdfArray)arr2;
            }
        }

        return null;
    }

    private void checkPrStreamLength(PrStream stream)
    {
        var fileLength = Tokens.Length;
        var start = stream.Offset;
        var calc = false;
        var streamLength = 0;
        var obj = GetPdfObjectRelease(stream.Get(PdfName.LENGTH));

        if (obj != null && obj.Type == PdfObject.NUMBER)
        {
            streamLength = ((PdfNumber)obj).IntValue;

            if (streamLength + start > fileLength - 20)
            {
                calc = true;
            }
            else
            {
                Tokens.Seek(start + streamLength);
                var line = Tokens.ReadString(20);

                if (!line.StartsWith("\nendstream", StringComparison.Ordinal) &&
                    !line.StartsWith("\r\nendstream", StringComparison.Ordinal) &&
                    !line.StartsWith("\rendstream", StringComparison.Ordinal) &&
                    !line.StartsWith("endstream", StringComparison.Ordinal))
                {
                    calc = true;
                }
            }
        }
        else
        {
            calc = true;
        }

        if (calc)
        {
            var tline = new byte[16];
            Tokens.Seek(start);

            while (true)
            {
                var pos = Tokens.FilePointer;

                if (!Tokens.ReadLineSegment(tline))
                {
                    break;
                }

                if (Equalsn(tline, _endstream))
                {
                    streamLength = pos - start;

                    break;
                }

                if (Equalsn(tline, _endobj))
                {
                    Tokens.Seek(pos - 16);
                    var s = Tokens.ReadString(16);
                    var index = s.IndexOf("endstream", StringComparison.OrdinalIgnoreCase);

                    if (index >= 0)
                    {
                        pos = pos - 16 + index;
                    }

                    streamLength = pos - start;

                    break;
                }
            }
        }

        stream.Length = streamLength;
    }

    private void ensureXrefSize(int size)
    {
        if (size == 0)
        {
            return;
        }

        if (Xref == null)
        {
            Xref = new int[size];
        }
        else
        {
            if (Xref.Length < size)
            {
                var xref2 = new int[size];
                Array.Copy(Xref, 0, xref2, 0, Xref.Length);
                Xref = xref2;
            }
        }
    }

    private static bool equalsArray(byte[] ar1, byte[] ar2, int size)
    {
        for (var k = 0; k < size; ++k)
        {
            if (ar1[k] != ar2[k])
            {
                return false;
            }
        }

        return true;
    }

    private void iterateBookmarks(PdfObject outlineRef, INullValueDictionary<object, PdfObject> names)
    {
        while (outlineRef != null)
        {
            replaceNamedDestination(outlineRef, names);
            var outline = (PdfDictionary)GetPdfObjectRelease(outlineRef);
            var first = outline.Get(PdfName.First);

            if (first != null)
            {
                iterateBookmarks(first, names);
            }

            outlineRef = outline.Get(PdfName.Next);
        }
    }

    /// <summary>
    ///     @throws IOException
    /// </summary>
    private void readDecryptedDocObj()
    {
        if (Encrypted)
        {
            return;
        }

        var encDic = trailer?.Get(PdfName.Encrypt);

        if (encDic == null || encDic.ToString().Equals("null", StringComparison.Ordinal))
        {
            return;
        }

        _encryptionError = true;
        byte[] encryptionKey = null;

        Encrypted = true;
        var enc = (PdfDictionary)GetPdfObject(encDic);

        string s;
        PdfObject o;

        var documentIDs = trailer.GetAsArray(PdfName.Id);
        byte[] documentId = null;

        if (documentIDs != null)
        {
            o = documentIDs[0];
            Strings.Remove((PdfString)o);
            s = o.ToString();
            documentId = DocWriter.GetIsoBytes(s);

            if (documentIDs.Size > 1)
            {
                Strings.Remove((PdfString)documentIDs[1]);
            }
        }

        // just in case we have a broken producer
        if (documentId == null)
        {
            documentId = Array.Empty<byte>();
        }

        byte[] uValue = null;
        byte[] oValue = null;
        var cryptoMode = PdfWriter.STANDARD_ENCRYPTION_40;
        var lengthValue = 0;

        var filter = GetPdfObjectRelease(enc.Get(PdfName.Filter));

        if (filter.Equals(PdfName.Standard))
        {
            s = enc.Get(PdfName.U).ToString();
            Strings.Remove((PdfString)enc.Get(PdfName.U));
            uValue = DocWriter.GetIsoBytes(s);
            s = enc.Get(PdfName.O).ToString();
            Strings.Remove((PdfString)enc.Get(PdfName.O));
            oValue = DocWriter.GetIsoBytes(s);

            o = enc.Get(PdfName.P);

            if (!o.IsNumber())
            {
                throw new InvalidPdfException($"Illegal P = {o} value.");
            }

            PValue = ((PdfNumber)o).IntValue;

            o = enc.Get(PdfName.R);

            if (!o.IsNumber())
            {
                throw new InvalidPdfException($"Illegal R = {o} value.");
            }

            RValue = ((PdfNumber)o).IntValue;

            switch (RValue)
            {
                case 2:
                    cryptoMode = PdfWriter.STANDARD_ENCRYPTION_40;

                    break;
                case 3:
                    o = enc.Get(PdfName.LENGTH);

                    if (!o.IsNumber())
                    {
                        throw new InvalidPdfException("Illegal Length value.");
                    }

                    lengthValue = ((PdfNumber)o).IntValue;

                    if (lengthValue > 128 || lengthValue < 40 || lengthValue % 8 != 0)
                    {
                        throw new InvalidPdfException("Illegal Length value.");
                    }

                    cryptoMode = PdfWriter.STANDARD_ENCRYPTION_128;

                    break;
                case 4:
                    var dic = (PdfDictionary)enc.Get(PdfName.Cf);

                    if (dic == null)
                    {
                        throw new InvalidPdfException("/CF not found (encryption)");
                    }

                    dic = (PdfDictionary)dic.Get(PdfName.Stdcf);

                    if (dic == null)
                    {
                        throw new InvalidPdfException("/StdCF not found (encryption)");
                    }

                    if (PdfName.V2.Equals(dic.Get(PdfName.Cfm)))
                    {
                        cryptoMode = PdfWriter.STANDARD_ENCRYPTION_128;
                    }
                    else if (PdfName.Aesv2.Equals(dic.Get(PdfName.Cfm)))
                    {
                        cryptoMode = PdfWriter.ENCRYPTION_AES_128;
                    }
                    else
                    {
                        throw new UnsupportedPdfException("No compatible encryption found");
                    }

                    var em = enc.Get(PdfName.Encryptmetadata);

                    if (em != null && em.ToString().Equals("false", StringComparison.Ordinal))
                    {
                        cryptoMode |= PdfWriter.DO_NOT_ENCRYPT_METADATA;
                    }

                    break;
                case 6:
                    cryptoMode = PdfWriter.ENCRYPTION_AES_256_V3;
                    em = enc.Get(PdfName.Encryptmetadata);

                    if (em != null && em.ToString().Equals("false", StringComparison.Ordinal))
                    {
                        cryptoMode |= PdfWriter.DO_NOT_ENCRYPT_METADATA;
                    }

                    break;
                default:
                    throw new UnsupportedPdfException("Unknown encryption type R = " + RValue);
            }
        }
        else if (filter.Equals(PdfName.Pubsec))
        {
            var foundRecipient = false;
            byte[] envelopedData = null;
            PdfArray recipients = null;

            o = enc.Get(PdfName.V);

            if (!o.IsNumber())
            {
                throw new InvalidPdfException("Illegal V value.");
            }

            var vValue = ((PdfNumber)o).IntValue;

            switch (vValue)
            {
                case 1:
                    cryptoMode = PdfWriter.STANDARD_ENCRYPTION_40;
                    lengthValue = 40;
                    recipients = (PdfArray)enc.Get(PdfName.Recipients);

                    break;
                case 2:
                    o = enc.Get(PdfName.LENGTH);

                    if (!o.IsNumber())
                    {
                        throw new InvalidPdfException("Illegal Length value.");
                    }

                    lengthValue = ((PdfNumber)o).IntValue;

                    if (lengthValue > 128 || lengthValue < 40 || lengthValue % 8 != 0)
                    {
                        throw new InvalidPdfException("Illegal Length value.");
                    }

                    cryptoMode = PdfWriter.STANDARD_ENCRYPTION_128;
                    recipients = (PdfArray)enc.Get(PdfName.Recipients);

                    break;
                case 4:
                    var dic = (PdfDictionary)enc.Get(PdfName.Cf);

                    if (dic == null)
                    {
                        throw new InvalidPdfException("/CF not found (encryption)");
                    }

                    dic = (PdfDictionary)dic.Get(PdfName.Defaultcryptfilter);

                    if (dic == null)
                    {
                        throw new InvalidPdfException("/DefaultCryptFilter not found (encryption)");
                    }

                    if (PdfName.V2.Equals(dic.Get(PdfName.Cfm)))
                    {
                        cryptoMode = PdfWriter.STANDARD_ENCRYPTION_128;
                        lengthValue = 128;
                    }
                    else if (PdfName.Aesv2.Equals(dic.Get(PdfName.Cfm)))
                    {
                        cryptoMode = PdfWriter.ENCRYPTION_AES_128;
                        lengthValue = 128;
                    }
                    else
                    {
                        throw new UnsupportedPdfException("No compatible encryption found");
                    }

                    var em = dic.Get(PdfName.Encryptmetadata);

                    if (em != null && em.ToString().Equals("false", StringComparison.Ordinal))
                    {
                        cryptoMode |= PdfWriter.DO_NOT_ENCRYPT_METADATA;
                    }

                    recipients = (PdfArray)dic.Get(PdfName.Recipients);

                    break;
                default:
                    throw new UnsupportedPdfException("Unknown encryption type V = " + RValue);
            }

            for (var i = 0; i < recipients.Size; i++)
            {
                var recipient = recipients[i];
                Strings.Remove((PdfString)recipient);

                CmsEnvelopedData data = null;
                data = new CmsEnvelopedData(recipient.GetBytes());

                foreach (var recipientInfo in data.GetRecipientInfos().GetRecipients())
                {
                    if (recipientInfo.RecipientID.Match(Certificate) && !foundRecipient)
                    {
                        envelopedData = recipientInfo.GetContent(CertificateKey);
                        foundRecipient = true;
                    }
                }
            }

            if (!foundRecipient || envelopedData == null)
            {
                throw new UnsupportedPdfException("Bad certificate and key.");
            }

#if NET40
            using (var sh = new SHA1CryptoServiceProvider())
            {
                sh.TransformBlock(envelopedData, 0, 20, envelopedData, 0);
                for (var i = 0; i < recipients.Size; i++)
                {
                    var encodedRecipient = recipients[i].GetBytes();
                    sh.TransformBlock(encodedRecipient, 0, encodedRecipient.Length, encodedRecipient, 0);
                }

                if ((cryptoMode & PdfWriter.DO_NOT_ENCRYPT_METADATA) != 0)
                {
                    sh.TransformBlock(PdfEncryption.MetadataPad, 0, PdfEncryption.MetadataPad.Length,
                                      PdfEncryption.MetadataPad, 0);
                }

                sh.TransformFinalBlock(envelopedData, 0, 0);
                encryptionKey = sh.Hash;
            }
#else
            using (var sh = IncrementalHash.CreateHash(HashAlgorithmName.SHA1))
            {
                sh.AppendData(envelopedData, 0, 20);

                for (var i = 0; i < recipients.Size; i++)
                {
                    var encodedRecipient = recipients[i].GetBytes();
                    sh.AppendData(encodedRecipient, 0, encodedRecipient.Length);
                }

                if ((cryptoMode & PdfWriter.DO_NOT_ENCRYPT_METADATA) != 0)
                {
                    sh.AppendData(PdfEncryption.MetadataPad, 0, PdfEncryption.MetadataPad.Length);
                }

                encryptionKey = sh.GetHashAndReset();
            }
#endif
        }

        decrypt = new PdfEncryption();
        decrypt.SetCryptoMode(cryptoMode, lengthValue);

        if (filter.Equals(PdfName.Standard))
        {
            if (RValue < 6)
            {
                //check by owner password
                decrypt.SetupByOwnerPassword(documentId, Password, uValue, oValue, PValue);

                if (!equalsArray(uValue, decrypt.UserKey, RValue == 3 || RValue == 4 ? 16 : 32))
                {
                    //check by user password
                    decrypt.SetupByUserPassword(documentId, Password, oValue, PValue);

                    if (!equalsArray(uValue, decrypt.UserKey, RValue == 3 || RValue == 4 ? 16 : 32))
                    {
                        throw new BadPasswordException("Bad user password");
                    }
                }
                else
                {
                    _ownerPasswordUsed = true;
                }
            }
            else
            {
                // implements Algorithm 2.A: Retrieving the file encryption key from an encrypted document in order to decrypt it (revision 6 and later) - ISO 32000-2 section 7.6.4.3.3
                s = enc.Get(PdfName.UE).ToString();
                Strings.Remove((PdfString)enc.Get(PdfName.UE));
                var ueValue = DocWriter.GetIsoBytes(s);
                s = enc.Get(PdfName.OE).ToString();
                Strings.Remove((PdfString)enc.Get(PdfName.OE));
                var oeValue = DocWriter.GetIsoBytes(s);
                s = enc.Get(PdfName.Perms).ToString();
                Strings.Remove((PdfString)enc.Get(PdfName.Perms));
                var permsValue = DocWriter.GetIsoBytes(s);

                // step b of Algorithm 2.A
                var password = Password;

                if (password == null)
                {
                    password = Array.Empty<byte>();
                }
                else if (password.Length > 127)
                {
                    password = password.CopyOf(127);
                }

                // According to ISO 32000-2 the uValue is expected to be 48 bytes in length.
                // Actual documents from the wild tend to have the uValue filled with zeroes
                // to a 127 bytes length. As input to computeHash for owner password related
                // operations, though, we must only use the 48 bytes.
                if (uValue != null && uValue.Length > 48)
                {
                    uValue = uValue.CopyOf(48);
                }

                // step c of Algorithm 2.A
                var hashAlg2B = PdfEncryption.HashAlg2B(password, oValue.CopyOfRange(32, 40), uValue);

                if (equalsArray(hashAlg2B, oValue, 32))
                {
                    // step d of Algorithm 2.A
                    decrypt.SetupByOwnerPassword(documentId, password, uValue, ueValue, oValue, oeValue, PValue);

                    // step f of Algorithm 2.A
                    if (decrypt.DecryptAndCheckPerms(permsValue))
                    {
                        _ownerPasswordUsed = true;
                    }
                }

                if (!_ownerPasswordUsed)
                {
                    // analog of step c of Algorithm 2.A for user password
                    hashAlg2B = PdfEncryption.HashAlg2B(password, uValue.CopyOfRange(32, 40), null);

                    if (!equalsArray(hashAlg2B, uValue, 32))
                    {
                        throw new BadPasswordException("Bad user password");
                    }

                    // step e of Algorithm 2.A
                    decrypt.SetupByUserPassword(documentId, password, uValue, ueValue, oValue, oeValue, PValue);

                    // step f of Algorithm 2.A
                    if (!decrypt.DecryptAndCheckPerms(permsValue))
                    {
                        throw new BadPasswordException("Bad user password");
                    }
                }

                PValue = decrypt.Permissions;
            }
        }
        else if (filter.Equals(PdfName.Pubsec))
        {
            decrypt.SetupByEncryptionKey(encryptionKey, lengthValue);
            _ownerPasswordUsed = true;
        }

        for (var k = 0; k < Strings.Count; ++k)
        {
            var str = Strings[k];
            str.Decrypt(this);
        }

        if (encDic.IsIndirect())
        {
            _cryptoRef = (PrIndirectReference)encDic;
            _xrefObj[_cryptoRef.Number] = null;
        }

        _encryptionError = false;
    }

    private bool replaceNamedDestination(PdfObject obj, INullValueDictionary<object, PdfObject> names)
    {
        obj = GetPdfObject(obj);
        var objIdx = _lastXrefPartial;
        ReleaseLastXrefPartial();

        if (obj != null && obj.IsDictionary())
        {
            var ob2 = GetPdfObjectRelease(((PdfDictionary)obj).Get(PdfName.Dest));
            object name = null;

            if (ob2 != null)
            {
                if (ob2.IsName())
                {
                    name = ob2;
                }
                else if (ob2.IsString())
                {
                    name = ob2.ToString();
                }

                if (name != null)
                {
                    var dest = (PdfArray)names[name];

                    if (dest != null)
                    {
                        ((PdfDictionary)obj).Put(PdfName.Dest, dest);
                        setXrefPartialObject(objIdx, obj);

                        return true;
                    }
                }
            }
            else if ((ob2 = GetPdfObject(((PdfDictionary)obj).Get(PdfName.A))) != null)
            {
                var obj2Idx = _lastXrefPartial;
                ReleaseLastXrefPartial();
                var dic = (PdfDictionary)ob2;
                var type = (PdfName)GetPdfObjectRelease(dic.Get(PdfName.S));

                if (PdfName.Goto.Equals(type))
                {
                    var ob3 = GetPdfObjectRelease(dic.Get(PdfName.D));

                    if (ob3 != null)
                    {
                        if (ob3.IsName())
                        {
                            name = ob3;
                        }
                        else if (ob3.IsString())
                        {
                            name = ob3.ToString();
                        }
                    }

                    if (name != null)
                    {
                        var dest = (PdfArray)names[name];

                        if (dest != null)
                        {
                            dic.Put(PdfName.D, dest);
                            setXrefPartialObject(obj2Idx, ob2);
                            setXrefPartialObject(objIdx, obj);

                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    private void setXrefPartialObject(int idx, PdfObject obj)
    {
        if (!_partial || idx < 0)
        {
            return;
        }

        _xrefObj[idx] = obj;
    }

    /// <summary>
    ///     Return all Signature Names from signatures that cover the whole document.
    ///     Empty in case no signature covers the whole document.
    /// </summary>
    /// <returns> An ArrayList filled with the Signature Names from signatures that covers the whole document.</returns>
    public IList<string> SignaturesCoverWholeDocument()
    {
        var signaturesCoverWholeDocument = new List<string>();
        var signatureNames = AcroFields.GetSignatureNames();

        if (signatureNames.Count <= 0)
        {
            throw new InvalidPdfException("No signatures found");
        }

        var signatureLastByte = new List<int>();

        for (var i = 0; i < signatureNames.Count; i++)
        {
            signatureNames[i] = AcroFields.GetTranslatedFieldName(signatureNames[i]);
            var nameDict = AcroFields.GetSignatureDictionary(signatureNames[i]);
            var byteRange = nameDict.GetAsArray(PdfName.Byterange);
            var offset = ((PdfNumber)byteRange[2]).IntValue;
            var lastByte = offset + ((PdfNumber)byteRange[3]).IntValue;
            signatureLastByte.Add(lastByte);
        }

        signatureLastByte.Sort(new SignaturesSorterComparator());

        if (xrefByteOffset.Count <= 0)
        {
            throw new InvalidPdfException("No references to xref found");
        }

        xrefByteOffset.Sort(new XrefsSorterComparator());

        for (var i = 0; i < xrefByteOffset.Count && i < signatureLastByte.Count; i++)
        {
            Tokens.Seek(xrefByteOffset[i]);
            Tokens.NextToken();

            if (Tokens.StringValue.Equals("xref", StringComparison.Ordinal))
            {
                while (true)
                {
                    if (!Tokens.NextToken() || !Tokens.TokenType.Equals(PrTokeniser.TK_NUMBER))
                    {
                        break;
                    }

                    if (!Tokens.NextToken() || !Tokens.TokenType.Equals(PrTokeniser.TK_NUMBER))
                    {
                        break;
                    }

                    var bytesSkip = Tokens.IntValue;
                    Tokens.Seek(Tokens.FilePointer + bytesSkip * 20);
                }

                if (!Tokens.StringValue.Equals("trailer", StringComparison.Ordinal))
                {
                    throw new InvalidPdfException("trailer not found");
                }

                while (true)
                {
                    if (Tokens.NextToken() && Tokens.StringValue.Equals("startxref", StringComparison.Ordinal))
                    {
                        break;
                    }
                }

                if (!Tokens.NextToken() || !Tokens.TokenType.Equals(PrTokeniser.TK_NUMBER))
                {
                    throw new InvalidPdfException("startxref byte position not found");
                }

                if (!Tokens.NextToken() || !string.IsNullOrEmpty(Tokens.StringValue))
                {
                    throw new InvalidPdfException("invalid data between startxref byte position and eof");
                }

                var eofPos = Tokens.FilePointer;

                if (eofPos > signatureLastByte[i])
                {
                    return signaturesCoverWholeDocument;
                }

                signaturesCoverWholeDocument.Add(signatureNames[i]);
            }
            else if (Tokens.TokenType == PrTokeniser.TK_NUMBER)
            {
                if (!Tokens.NextToken())
                {
                    throw new InvalidPdfException("object's generation number not found");
                }

                if (!Tokens.NextToken() || !Tokens.StringValue.Equals("obj", StringComparison.Ordinal))
                {
                    throw new InvalidPdfException("object's brilling not found");
                }

                var obj = ReadPrObject();
                PrStream stm;

                if (obj.IsStream())
                {
                    stm = (PrStream)obj;

                    if (!PdfName.Xref.Equals(stm.Get(PdfName.TYPE)))
                    {
                        throw new InvalidPdfException("object is not typed as XRef");
                    }
                }
                else
                {
                    throw new InvalidPdfException("invalid xref object");
                }

                var Length = ((PdfNumber)stm.Get(PdfName.LENGTH)).IntValue;
                Tokens.Seek(Tokens.FilePointer + Length);

                if (!Tokens.NextToken() || !Tokens.StringValue.Equals("endstream", StringComparison.Ordinal))
                {
                    throw new InvalidPdfException("endstream not found");
                }

                if (!Tokens.NextToken() || !Tokens.StringValue.Equals("endobj", StringComparison.Ordinal))
                {
                    throw new InvalidPdfException("endobj not found");
                }

                if (!Tokens.NextToken() || !Tokens.StringValue.Equals("startxref", StringComparison.Ordinal))
                {
                    throw new InvalidPdfException("startxref not found");
                }

                if (!Tokens.NextToken() || !Tokens.TokenType.Equals(PrTokeniser.TK_NUMBER))
                {
                    throw new InvalidPdfException("startxref byte position not found");
                }

                if (!Tokens.NextToken() || !string.IsNullOrEmpty(Tokens.StringValue))
                {
                    throw new InvalidPdfException("invalid data between startxref byte position and eof");
                }

                var eofPos = Tokens.FilePointer;

                if (eofPos > signatureLastByte[i])
                {
                    return signaturesCoverWholeDocument;
                }

                signaturesCoverWholeDocument.Add(signatureNames[i]);
            }
            else
            {
                throw new InvalidPdfException("xref object not found");
            }
        }

        return signaturesCoverWholeDocument;
    }

    public class SignaturesSorterComparator : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            var n1 = x;
            var n2 = y;

            return n2 - n1;
        }
    }

    public class XrefsSorterComparator : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            var n1 = x;
            var n2 = y;

            return n2 - n1;
        }
    }

    public class PageRefs
    {
        private readonly PdfReader _reader;
        private bool _keepPages;
        private int _lastPageRead = -1;
        private List<PdfDictionary> _pageInh;
        private List<PrIndirectReference> _refsn;
        private NullValueDictionary<int, int> _refsp;
        private int _sizep;

        internal PageRefs(PdfReader reader)
        {
            _reader = reader;

            if (reader._partial)
            {
                _refsp = new NullValueDictionary<int, int>();
                var npages = (PdfNumber)GetPdfObjectRelease(reader._rootPages.Get(PdfName.Count));
                _sizep = npages.IntValue;
            }
            else
            {
                ReadPages();
            }
        }

        internal PageRefs(PageRefs other, PdfReader reader)
        {
            _reader = reader;
            _sizep = other._sizep;

            if (other._refsn != null)
            {
                _refsn = new List<PrIndirectReference>(other._refsn);

                for (var k = 0; k < _refsn.Count; ++k)
                {
                    _refsn[k] = (PrIndirectReference)DuplicatePdfObject(_refsn[k], reader);
                }
            }
            else
            {
                _refsp = (NullValueDictionary<int, int>)other._refsp.Clone();
            }
        }

        internal int Size
        {
            get
            {
                if (_refsn != null)
                {
                    return _refsn.Count;
                }

                return _sizep;
            }
        }

        /// <summary>
        ///     Gets the dictionary that represents a page.
        /// </summary>
        /// <param name="pageNum">the page number. 1 is the first</param>
        /// <returns>the page dictionary</returns>
        public PdfDictionary GetPageN(int pageNum)
        {
            var refi = GetPageOrigRef(pageNum);

            return (PdfDictionary)GetPdfObject(refi);
        }

        /// <summary>
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns>a dictionary object</returns>
        public PdfDictionary GetPageNRelease(int pageNum)
        {
            var page = GetPageN(pageNum);
            ReleasePage(pageNum);

            return page;
        }

        /// <summary>
        ///     Gets the page reference to this page.
        /// </summary>
        /// <param name="pageNum">the page number. 1 is the first</param>
        /// <returns>the page reference</returns>
        public PrIndirectReference GetPageOrigRef(int pageNum)
        {
            --pageNum;

            if (pageNum < 0 || pageNum >= Size)
            {
                return null;
            }

            if (_refsn != null)
            {
                return _refsn[pageNum];
            }

            var n = _refsp[pageNum];

            if (n == 0)
            {
                var refi = GetSinglePage(pageNum);

                if (_reader._lastXrefPartial == -1)
                {
                    _lastPageRead = -1;
                }
                else
                {
                    _lastPageRead = pageNum;
                }

                _reader._lastXrefPartial = -1;
                _refsp[pageNum] = refi.Number;

                if (_keepPages)
                {
                    _lastPageRead = -1;
                }

                return refi;
            }

            if (_lastPageRead != pageNum)
            {
                _lastPageRead = -1;
            }

            if (_keepPages)
            {
                _lastPageRead = -1;
            }

            return new PrIndirectReference(_reader, n);
        }

        /// <summary>
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns>an indirect reference</returns>
        public PrIndirectReference GetPageOrigRefRelease(int pageNum)
        {
            var refi = GetPageOrigRef(pageNum);
            ReleasePage(pageNum);

            return refi;
        }

        /// <summary>
        /// </summary>
        /// <param name="pageNum"></param>
        public void ReleasePage(int pageNum)
        {
            if (_refsp == null)
            {
                return;
            }

            --pageNum;

            if (pageNum < 0 || pageNum >= Size)
            {
                return;
            }

            if (pageNum != _lastPageRead)
            {
                return;
            }

            _lastPageRead = -1;
            _reader._lastXrefPartial = _refsp[pageNum];
            _reader.ReleaseLastXrefPartial();
            _refsp.Remove(pageNum);
        }

        /// <summary>
        /// </summary>
        public void ResetReleasePage()
        {
            if (_refsp == null)
            {
                return;
            }

            _lastPageRead = -1;
        }

        internal void InsertPage(int pageNum, PrIndirectReference refi)
        {
            --pageNum;

            if (_refsn != null)
            {
                if (pageNum >= _refsn.Count)
                {
                    _refsn.Add(refi);
                }
                else
                {
                    _refsn.Insert(pageNum, refi);
                }
            }
            else
            {
                ++_sizep;
                _lastPageRead = -1;

                if (pageNum >= Size)
                {
                    _refsp[Size] = refi.Number;
                }
                else
                {
                    var refs2 = new NullValueDictionary<int, int>();

                    foreach (var entry in _refsp)
                    {
                        var p = entry.Key;
                        refs2[p >= pageNum ? p + 1 : p] = entry.Value;
                    }

                    refs2[pageNum] = refi.Number;
                    _refsp = refs2;
                }
            }
        }

        internal void KeepPages()
        {
            if (_refsp == null || _keepPages)
            {
                return;
            }

            _keepPages = true;
            _refsp.Clear();
        }

        internal void ReadPages()
        {
            if (_refsn != null)
            {
                return;
            }

            _refsp = null;
            _refsn = new List<PrIndirectReference>();
            _pageInh = new List<PdfDictionary>();
            iteratePages((PrIndirectReference)_reader.catalog.Get(PdfName.Pages), 0);
            _pageInh = null;
            _reader._rootPages.Put(PdfName.Count, new PdfNumber(_refsn.Count));
        }

        internal void ReReadPages()
        {
            _refsn = null;
            ReadPages();
        }

        internal void SelectPages(ICollection<int> pagesToKeep)
        {
            var pg = new NullValueDictionary<int, int>();
            var finalPages = new List<int>();
            var psize = Size;

            foreach (var p in pagesToKeep)
            {
                if (p >= 1 && p <= psize && !pg.ContainsKey(p))
                {
                    pg[p] = 1;
                    finalPages.Add(p);
                }
            }

            if (_reader._partial)
            {
                for (var k = 1; k <= psize; ++k)
                {
                    GetPageOrigRef(k);
                    ResetReleasePage();
                }
            }

            var parent = (PrIndirectReference)_reader.catalog.Get(PdfName.Pages);
            var topPages = (PdfDictionary)GetPdfObject(parent);
            var newPageRefs = new List<PrIndirectReference>(finalPages.Count);
            var kids = new PdfArray();

            foreach (var p in finalPages)
            {
                var pref = GetPageOrigRef(p);
                ResetReleasePage();
                kids.Add(pref);
                newPageRefs.Add(pref);
                GetPageN(p).Put(PdfName.Parent, parent);
            }

            var af = _reader.AcroFields;
            var removeFields = af.Fields.Count > 0;

            for (var k = 1; k <= psize; ++k)
            {
                if (!pg.ContainsKey(k))
                {
                    if (removeFields)
                    {
                        af.RemoveFieldsFromPage(k);
                    }

                    var pref = GetPageOrigRef(k);
                    var nref = pref.Number;
                    _reader._xrefObj[nref] = null;

                    if (_reader._partial)
                    {
                        _reader.Xref[nref * 2] = -1;
                        _reader.Xref[nref * 2 + 1] = 0;
                    }
                }
            }

            topPages.Put(PdfName.Count, new PdfNumber(finalPages.Count));
            topPages.Put(PdfName.Kids, kids);
            _refsp = null;
            _refsn = newPageRefs;
        }

        protected internal PrIndirectReference GetSinglePage(int n)
        {
            var acc = new PdfDictionary();
            var top = _reader._rootPages;
            var baseb = 0;

            while (true)
            {
                for (var k = 0; k < _pageInhCandidates.Length; ++k)
                {
                    var obj = top.Get(_pageInhCandidates[k]);

                    if (obj != null)
                    {
                        acc.Put(_pageInhCandidates[k], obj);
                    }
                }

                var kids = (PdfArray)GetPdfObjectRelease(top.Get(PdfName.Kids));

                for (var it = new ListIterator<PdfObject>(kids.ArrayList); it.HasNext();)
                {
                    var refi = (PrIndirectReference)it.Next();
                    var dic = (PdfDictionary)GetPdfObject(refi);
                    var last = _reader._lastXrefPartial;
                    var count = GetPdfObjectRelease(dic.Get(PdfName.Count));
                    _reader._lastXrefPartial = last;
                    var acn = 1;

                    if (count != null && count.Type == PdfObject.NUMBER)
                    {
                        acn = ((PdfNumber)count).IntValue;
                    }

                    if (n < baseb + acn)
                    {
                        if (count == null)
                        {
                            dic.MergeDifferent(acc);

                            return refi;
                        }

                        _reader.ReleaseLastXrefPartial();
                        top = dic;

                        break;
                    }

                    _reader.ReleaseLastXrefPartial();
                    baseb += acn;
                }
            }
        }

        private void iteratePages(PrIndirectReference rpage, int depth)
        {
            try
            {
                var page = (PdfDictionary)GetPdfObject(rpage);
                var kidsPr = page.GetAsArray(PdfName.Kids);

                if (kidsPr == null)
                {
                    page.Put(PdfName.TYPE, PdfName.Page);
                    var dic = _pageInh[_pageInh.Count - 1];

                    foreach (var key in dic.Keys)
                    {
                        if (page.Get(key) == null)
                        {
                            page.Put(key, dic.Get(key));
                        }
                    }

                    if (page.Get(PdfName.Mediabox) == null)
                    {
                        var arr = new PdfArray(new[]
                        {
                            0, 0, PageSize.Letter.Right, PageSize.Letter.Top
                        });

                        page.Put(PdfName.Mediabox, arr);
                    }

                    _refsn.Add(rpage);
                }
                else
                {
                    page.Put(PdfName.TYPE, PdfName.Pages);
                    pushPageAttributes(page);

                    for (var k = 0; k < kidsPr.Size; ++k)
                    {
                        var obj = kidsPr[k];

                        if (!obj.IsIndirect())
                        {
                            while (k < kidsPr.Size)
                            {
                                kidsPr.Remove(k);
                            }

                            break;
                        }

                        if (depth < 2000) //Depth limit for bad files that cause loop
                        {
                            iteratePages((PrIndirectReference)obj, depth + 1);
                        }
                    }

                    popPageAttributes();
                }
            }
            catch
            {
            }
        }

        private void popPageAttributes() => _pageInh.RemoveAt(_pageInh.Count - 1);

        private void pushPageAttributes(PdfDictionary nodePages)
        {
            var dic = new PdfDictionary();

            if (_pageInh.Count != 0)
            {
                dic.Merge(_pageInh[_pageInh.Count - 1]);
            }

            for (var k = 0; k < _pageInhCandidates.Length; ++k)
            {
                var obj = nodePages.Get(_pageInhCandidates[k]);

                if (obj != null)
                {
                    dic.Put(_pageInhCandidates[k], obj);
                }
            }

            _pageInh.Add(dic);
        }
    }
}
