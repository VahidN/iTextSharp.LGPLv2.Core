using System.util;
using System.util.collections;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.pdf.events;
using iTextSharp.text.pdf.interfaces;
using iTextSharp.text.pdf.intern;
using iTextSharp.text.xml.xmp;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
///     A  DocWriter  class for PDF.
///     When this  PdfWriter  is added
///     to a certain  PdfDocument , the PDF representation of every Element
///     added to this Document will be written to the outputstream.
/// </summary>
public class PdfWriter : DocWriter, IPdfViewerPreferences, IPdfEncryptionSettings, IPdfVersion, IPdfDocumentActions,
    IPdfPageActions, IPdfXConformance, IPdfRunDirection, IPdfAnnotations
{
    /// <summary>
    ///     The operation permitted when the document is opened with the user password
    ///     @since 2.0.7
    /// </summary>
    public const int ALLOW_ASSEMBLY = 1024;

    /// <summary>
    ///     The operation permitted when the document is opened with the user password
    ///     @since 2.0.7
    /// </summary>
    public const int ALLOW_COPY = 16;

    /// <summary>
    ///     The operation permitted when the document is opened with the user password
    ///     @since 2.0.7
    /// </summary>
    public const int ALLOW_DEGRADED_PRINTING = 4;

    /// <summary>
    ///     The operation permitted when the document is opened with the user password
    ///     @since 2.0.7
    /// </summary>
    public const int ALLOW_FILL_IN = 256;

    /// <summary>
    ///     The operation permitted when the document is opened with the user password
    ///     @since 2.0.7
    /// </summary>
    public const int ALLOW_MODIFY_ANNOTATIONS = 32;

    /// <summary>
    ///     The operation permitted when the document is opened with the user password
    ///     @since 2.0.7
    /// </summary>
    public const int ALLOW_MODIFY_CONTENTS = 8;

    /// <summary>
    ///     The operation permitted when the document is opened with the user password
    ///     @since 2.0.7
    /// </summary>
    public const int ALLOW_PRINTING = 4 + 2048;

    /// <summary>
    ///     permissions
    /// </summary>
    /// <summary>
    ///     The operation permitted when the document is opened with the user password
    ///     @since 2.0.7
    /// </summary>
    public const int ALLOW_SCREENREADERS = 512;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #ALLOW_ASSEMBLY} instead. Scheduled for removal at or after 2.2.0
    /// </summary>
    public const int AllowAssembly = ALLOW_ASSEMBLY;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #ALLOW_COPY} instead. Scheduled for removal at or after 2.2.0
    /// </summary>
    public const int AllowCopy = ALLOW_COPY;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #ALLOW_DEGRADED_PRINTING} instead. Scheduled for removal at or after
    ///     2.2.0
    /// </summary>
    public const int AllowDegradedPrinting = ALLOW_DEGRADED_PRINTING;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #ALLOW_FILL_IN} instead. Scheduled for removal at or after 2.2.0
    /// </summary>
    public const int AllowFillIn = ALLOW_FILL_IN;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #ALLOW_MODIFY_ANNOTATIONS} instead. Scheduled for removal at or after
    ///     2.2.0
    /// </summary>
    public const int AllowModifyAnnotations = ALLOW_MODIFY_ANNOTATIONS;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #ALLOW_MODIFY_CONTENTS} instead. Scheduled for removal at or after 2.2.0
    /// </summary>
    public const int AllowModifyContents = ALLOW_MODIFY_CONTENTS;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #ALLOW_PRINTING} instead. Scheduled for removal at or after 2.2.0
    /// </summary>
    public const int AllowPrinting = ALLOW_PRINTING;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #ALLOW_SCREENREADERS} instead. Scheduled for removal at or after 2.2.0
    /// </summary>
    public const int AllowScreenReaders = ALLOW_SCREENREADERS;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int CenterWindow = 1 << 16;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int DirectionL2R = 1 << 22;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int DirectionR2L = 1 << 23;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int DisplayDocTitle = 1 << 17;

    /// <summary>
    ///     Add this to the mode to keep the metadata in clear text
    /// </summary>
    public const int DO_NOT_ENCRYPT_METADATA = 8;

    /// <summary>
    ///     Add this to the mode to keep encrypt only the embedded files.
    ///     @since 2.1.3
    /// </summary>
    public const int EMBEDDED_FILES_ONLY = 24;

    /// <summary>
    ///     Type of encryption
    /// </summary>
    public const int ENCRYPTION_AES_128 = 2;

    /// <summary>
    ///     Type of encryption
    /// </summary>
    public const int ENCRYPTION_AES_256_V3 = 4;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int FitWindow = 1 << 15;

    /// <summary>
    ///     The highest generation number possible.
    ///     @since   iText 2.1.6
    /// </summary>
    public const int GENERATION_MAX = 65535;

    /// <summary>
    ///     INNER CLASSES
    /// </summary>
    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int HideMenubar = 1 << 13;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int HideToolbar = 1 << 12;

    /// <summary>
    ///     values for setting viewer preferences in iText versions older than 2.x
    /// </summary>
    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int HideWindowUI = 1 << 14;

    /// <summary>
    ///     Disable the inter-character spacing.
    /// </summary>
    public const float NO_SPACE_CHAR_RATIO = 10000000f;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int NonFullScreenPageModeUseNone = 1 << 18;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int NonFullScreenPageModeUseOC = 1 << 21;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int NonFullScreenPageModeUseOutlines = 1 << 19;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int NonFullScreenPageModeUseThumbs = 1 << 20;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageLayoutOneColumn = 2;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageLayoutSinglePage = 1;

    /// <summary>
    ///     page layout (section 13.1.1 of "iText in Action")
    /// </summary>
    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageLayoutTwoColumnLeft = 4;

    /// <summary>
    ///     [C3] PdfViewerPreferences interface
    /// </summary>
    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageLayoutTwoColumnRight = 8;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageLayoutTwoPageLeft = 16;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageLayoutTwoPageRight = 32;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageModeFullScreen = 512;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageModeUseAttachments = 2048;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageModeUseNone = 64;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageModeUseOC = 1024;

    /// <summary>
    ///     page mode (section 13.1.2 of "iText in Action")
    /// </summary>
    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageModeUseOutlines = 128;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PageModeUseThumbs = 256;

    /// <summary>
    ///     PDFA-1A level.
    /// </summary>
    public const int PDFA1A = 3;

    /// <summary>
    ///     PDFA-1B level.
    /// </summary>
    public const int PDFA1B = 4;

    /// <summary>
    ///     PDF/X level
    /// </summary>
    public const int PDFX1A2001 = 1;

    /// <summary>
    ///     PDF/X level
    /// </summary>
    public const int PDFX32002 = 2;

    /// <summary>
    ///     PDF/X level
    /// </summary>
    public const int PDFXNONE = 0;

    /// <summary>
    ///     A viewer preference
    /// </summary>
    public const int PrintScalingNone = 1 << 24;

    /// <summary>
    ///     Use the default run direction.
    /// </summary>
    public const int RUN_DIRECTION_DEFAULT = 0;

    /// <summary>
    ///     Use bidirectional reordering with left-to-right
    ///     preferential run direction.
    /// </summary>
    public const int RUN_DIRECTION_LTR = 2;

    /// <summary>
    ///     [U7] run direction (doesn't actually do anything)
    /// </summary>
    /// <summary>
    ///     Do not use bidirectional reordering.
    /// </summary>
    public const int RUN_DIRECTION_NO_BIDI = 1;

    /// <summary>
    ///     Use bidirectional reordering with right-to-left
    ///     preferential run direction.
    /// </summary>
    public const int RUN_DIRECTION_RTL = 3;

    /// <summary>
    ///     signature value
    /// </summary>
    public const int SIGNATURE_APPEND_ONLY = 2;

    /// <summary>
    ///     signature value
    /// </summary>
    public const int SIGNATURE_EXISTS = 1;

    /// <summary>
    ///     The default space-char ratio.
    /// </summary>
    public const float SPACE_CHAR_RATIO_DEFAULT = 2.5f;

    /// <summary>
    ///     Type of encryption
    /// </summary>
    public const int STANDARD_ENCRYPTION_128 = 1;

    /// <summary>
    ///     Type of encryption
    /// </summary>
    public const int STANDARD_ENCRYPTION_40 = 0;

    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #STANDARD_ENCRYPTION_128} instead. Scheduled for removal at or after
    ///     2.2.0
    /// </summary>
    public const bool STRENGTH128BITS = true;

    /// <summary>
    ///     Strength of the encryption (kept for historical reasons)
    /// </summary>
    /// <summary>
    ///     @deprecated As of iText 2.0.7, use {@link #STANDARD_ENCRYPTION_40} instead. Scheduled for removal at or after 2.2.0
    /// </summary>
    public const bool STRENGTH40BITS = false;

    /// <summary>
    ///     [C2] PdfVersion interface
    /// </summary>
    /// <summary>
    ///     possible PDF version (header)
    /// </summary>
    public const char VERSION_1_2 = '2';

    /// <summary>
    ///     possible PDF version (header)
    /// </summary>
    public const char VERSION_1_3 = '3';

    /// <summary>
    ///     possible PDF version (header)
    /// </summary>
    public const char VERSION_1_4 = '4';

    /// <summary>
    ///     possible PDF version (header)
    /// </summary>
    public const char VERSION_1_5 = '5';

    /// <summary>
    ///     possible PDF version (header)
    /// </summary>
    public const char VERSION_1_6 = '6';

    /// <summary>
    ///     possible PDF version (header)
    /// </summary>
    public const char VERSION_1_7 = '7';

    /// <summary>
    ///     types of encryption
    /// </summary>
    /// <summary>
    ///     Mask to separate the encryption type from the encryption mode.
    /// </summary>
    internal const int ENCRYPTION_MASK = 7;

    /// <summary>
    ///     action value
    /// </summary>
    public static readonly PdfName PageClose = PdfName.C;

    /// <summary>
    ///     action value
    /// </summary>
    public static readonly PdfName PageOpen = PdfName.O;

    /// <summary>
    ///     possible PDF version (catalog)
    /// </summary>
    public static readonly PdfName PdfVersion12 = new("1.2");

    /// <summary>
    ///     possible PDF version (catalog)
    /// </summary>
    public static readonly PdfName PdfVersion13 = new("1.3");

    /// <summary>
    ///     possible PDF version (catalog)
    /// </summary>
    public static readonly PdfName PdfVersion14 = new("1.4");

    /// <summary>
    ///     possible PDF version (catalog)
    /// </summary>
    public static readonly PdfName PdfVersion15 = new("1.5");

    /// <summary>
    ///     possible PDF version (catalog)
    /// </summary>
    public static readonly PdfName PdfVersion16 = new("1.6");

    /// <summary>
    ///     possible PDF version (catalog)
    /// </summary>
    public static readonly PdfName PdfVersion17 = new("1.7");

    /// <summary>
    ///     action value
    /// </summary>
    public static PdfName DidPrint = PdfName.Dp;

    /// <summary>
    ///     action value
    /// </summary>
    public static PdfName DidSave = PdfName.Ds;

    /// <summary>
    ///     action value
    /// </summary>
    public static PdfName DocumentClose = PdfName.Wc;

    /// <summary>
    ///     action value
    /// </summary>
    public static PdfName WillPrint = PdfName.Wp;

    /// <summary>
    ///     [C6] Actions (open and additional)
    /// </summary>
    /// <summary>
    ///     action value
    /// </summary>
    public static PdfName WillSave = PdfName.Ws;

    private static readonly float[] _gammaValues =
    {
        2.2f, 2.2f, 2.2f
    };

    private static readonly float[] _matrixValues =
    {
        0.4124f, 0.2126f, 0.0193f, 0.3576f, 0.7152f, 0.1192f, 0.1805f, 0.0722f, 0.9505f
    };

    private static readonly float[] _whitepointValues =
    {
        0.9505f, 1f, 1.089f
    };

    /// <summary>
    ///     This is the list with all the images in the document.
    /// </summary>
    private readonly NullValueDictionary<long, PdfName> _images = new();

    /// <summary>
    ///     [C10] PDFX Conformance
    /// </summary>
    /// <summary>
    ///     Stores the PDF/X level.
    /// </summary>
    private readonly PdfXConformanceImp _pdfxConformance = new();

    /// <summary>
    ///     The  PdfPageEvent  for this document.
    /// </summary>
    private IPdfPageEvent _pageEvent;

    /// <summary>
    ///     Holds value of property RGBTranparency.
    /// </summary>
    private bool _rgbTransparencyBlending;

    /// <summary>
    ///     [U6] space char ratio
    /// </summary>
    /// <summary>
    ///     The ratio between the extra word spacing and the extra character spacing.
    ///     Extra word spacing will grow  ratio  times more than extra character spacing.
    /// </summary>
    private float _spaceCharRatio = SPACE_CHAR_RATIO_DEFAULT;

    /// <summary>
    ///     [F12] tagged PDF
    /// </summary>
    /// <summary>
    ///     A flag indicating the presence of structure elements that contain user properties attributes.
    /// </summary>
    private bool _userProperties;

    /// <summary>
    ///     body of the PDF document
    /// </summary>
    protected internal PdfBody Body;

    /// <summary>
    ///     The color number counter for the colors in the document.
    /// </summary>
    protected int ColorNumber = 1;

    /// <summary>
    ///     The compression level of the content streams.
    ///     @since   2.1.3
    /// </summary>
    protected internal int compressionLevel = PdfStream.DEFAULT_COMPRESSION;

    /// <summary>
    ///     [F1] PdfEncryptionSettings interface
    /// </summary>
    /// <summary>
    ///     Contains the business logic for cryptography.
    /// </summary>
    protected PdfEncryption Crypto;

    /// <summary>
    ///     The current page number.
    /// </summary>
    protected int currentPageNumber = 1;

    protected PdfReaderInstance CurrentPdfReaderInstance;

    protected PdfDictionary defaultColorspace = new();

    /// <summary>
    ///     The direct content in this document.
    /// </summary>
    protected PdfContentByte directContent;

    /// <summary>
    ///     You should see Direct Content as a canvas on which you can draw
    ///     graphics and text. One canvas goes on top of the page (getDirectContent),
    ///     the other goes underneath (getDirectContentUnder).
    ///     You can always the same object throughout your document,
    ///     even if you have moved to a new page. Whatever you add on
    ///     the canvas will be displayed on top or under the current page.
    /// </summary>
    /// <summary>
    ///     The direct content under in this document.
    /// </summary>
    protected PdfContentByte directContentUnder;

    /// <summary>
    ///     The colors of this document
    /// </summary>
    protected INullValueDictionary<PdfSpotColor, ColorDetails> DocumentColors =
        new NullValueDictionary<PdfSpotColor, ColorDetails>();

    protected INullValueDictionary<PdfDictionary, PdfObject[]> DocumentExtGState =
        new NullValueDictionary<PdfDictionary, PdfObject[]>();

    /// <summary>
    ///     The fonts of this document
    /// </summary>
    protected INullValueDictionary<BaseFont, FontDetails> DocumentFonts =
        new NullValueDictionary<BaseFont, FontDetails>();

    protected INullValueDictionary<IPdfOcg, object> DocumentOcg = new NullValueDictionary<IPdfOcg, object>();

    /// <summary>
    ///     [F13] Optional Content Groups
    /// </summary>
    protected List<IPdfOcg> DocumentOcGorder = new();

    /// <summary>
    ///     The patterns of this document
    /// </summary>
    protected INullValueDictionary<PdfPatternPainter, PdfName> DocumentPatterns =
        new NullValueDictionary<PdfPatternPainter, PdfName>();

    protected INullValueDictionary<object, PdfObject[]> DocumentProperties =
        new NullValueDictionary<object, PdfObject[]>();

    protected INullValueDictionary<PdfShadingPattern, object> DocumentShadingPatterns =
        new NullValueDictionary<PdfShadingPattern, object>();

    protected INullValueDictionary<PdfShading, object> DocumentShadings = new NullValueDictionary<PdfShading, object>();

    protected INullValueDictionary<ColorDetails, ColorDetails> DocumentSpotPatterns =
        new NullValueDictionary<ColorDetails, ColorDetails>();

    /// <summary>
    ///     Holds value of property extraCatalog.
    /// </summary>
    protected internal PdfDictionary extraCatalog;

    /// <summary>
    ///     [F3] adding fonts
    /// </summary>
    /// <summary>
    ///     The font number counter for the fonts in the document.
    /// </summary>
    protected int FontNumber = 1;

    /// <summary>
    ///     The form XObjects in this document. The key is the xref and the value
    ///     is Object[]{PdfName, template}.
    /// </summary>
    protected INullValueDictionary<PdfIndirectReference, object[]> FormXObjects =
        new NullValueDictionary<PdfIndirectReference, object[]>();

    /// <summary>
    ///     [F4] adding (and releasing) form XObjects
    /// </summary>
    /// <summary>
    ///     The name counter for the form XObjects name.
    /// </summary>
    protected int FormXObjectsCounter = 1;

    /// <summary>
    ///     Holds value of property fullCompression.
    /// </summary>
    protected bool fullCompression;

    /// <summary>
    ///     A group attributes dictionary specifying the attributes
    ///     of the page�s page group for use in the transparent
    ///     imaging model
    /// </summary>
    protected PdfDictionary group;

    /// <summary>
    ///     Dictionary, containing all the images of the PDF document
    /// </summary>
    protected PdfDictionary ImageDictionary = new();

    protected INullValueDictionary<PdfReader, PdfReaderInstance> ImportedPages =
        new NullValueDictionary<PdfReader, PdfReaderInstance>();

    /// <summary>
    ///     A Hashtable with Stream objects containing JBIG2 Globals
    ///     @since 2.1.5
    /// </summary>
    protected INullValueDictionary<PdfStream, PdfIndirectReference> Jbig2Globals =
        new NullValueDictionary<PdfStream, PdfIndirectReference>();

    protected IList<INullValueDictionary<string, object>> NewBookmarks;

    protected PdfArray OcgLocked = new();

    protected PdfArray OcgRadioGroup = new();

    /// <summary>
    ///     The PdfIndirectReference to the pages.
    /// </summary>
    protected List<PdfIndirectReference> PageReferences = new();

    protected ColorDetails PatternColorspaceCmyk;

    protected ColorDetails PatternColorspaceGray;

    /// <summary>
    ///     [M2] spot patterns
    /// </summary>
    protected ColorDetails PatternColorspaceRgb;

    /// <summary>
    ///     [F7] document patterns
    /// </summary>
    /// <summary>
    ///     The patten number counter for the colors in the document.
    /// </summary>
    protected int PatternNumber = 1;

    /// <summary>
    ///     the PdfDocument instance
    /// </summary>
    /// <summary>
    ///     the pdfdocument object.
    /// </summary>
    protected internal PdfDocument Pdf;

    /// <summary>
    ///     Stores the version information for the header and the catalog.
    /// </summary>
    protected PdfVersionImp pdf_version = new();

    /// <summary>
    ///     A number refering to the previous Cross-Reference Table.
    /// </summary>
    protected int Prevxref = 0;

    /// <summary>
    ///     The root of the page tree.
    /// </summary>
    protected PdfPages Root;

    protected int runDirection = RUN_DIRECTION_NO_BIDI;

    protected PdfStructureTreeRoot structureTreeRoot;

    /// <summary>
    ///     The page root keeps the complete page tree of the document.
    ///     There's an entry in the Catalog that refers to the root
    ///     of the page tree, the page tree contains the references
    ///     to pages and other page trees.
    /// </summary>
    /// <summary>
    ///     The value of the Tabs entry in the page dictionary.
    ///     @since   2.1.5
    /// </summary>
    protected PdfName tabs;

    protected bool Tagged;

    protected float userunit;

    protected PdfOcProperties VOcProperties;

    /// <summary>
    ///     XMP Metadata for the document.
    /// </summary>
    protected byte[] xmpMetadata;

    /// <summary>
    ///     Constructs a  PdfWriter .
    /// </summary>
    protected PdfWriter()
        => Root = new PdfPages(this);

    protected PdfWriter(PdfDocument document, Stream os) : base(document, os)
    {
        Root = new PdfPages(this);
        Pdf = document;
        directContent = new PdfContentByte(this);
        directContentUnder = new PdfContentByte(this);
    }

    /// <summary>
    ///     This is the current height of the document.
    /// </summary>
    public float CurrentPageHeight => Pdf.CurrentHeight;

    /// <summary>
    ///     [C7] portable collections
    /// </summary>
    /// <summary>
    ///     Sets the Collection dictionary.
    /// </summary>
    public PdfCollection Collection
    {
        set
        {
            SetAtLeastPdfVersion(VERSION_1_7);
            Pdf.Collection = value;
        }
    }

    /// <summary>
    ///     Sets the compression level to be used for streams written by this writer.
    ///     @since   2.1.3
    /// </summary>
    public int CompressionLevel
    {
        set
        {
            if (compressionLevel < PdfStream.NO_COMPRESSION || compressionLevel > PdfStream.BEST_COMPRESSION)
            {
                compressionLevel = PdfStream.DEFAULT_COMPRESSION;
            }
            else
            {
                compressionLevel = value;
            }
        }
        get => compressionLevel;
    }

    /// <summary>
    ///     Sets the crop box. The crop box should not be rotated even if the
    ///     page is rotated. This change only takes effect in the next
    ///     page.
    /// </summary>
    public virtual Rectangle CropBoxSize
    {
        set => Pdf.CropBoxSize = value;
    }

    /// <summary>
    ///     Gets the current document size. This size only includes
    ///     the data already writen to the output stream, it does not
    ///     include templates or fonts. It is usefull if used with
    ///     freeReader()  when concatenating many documents
    ///     and an idea of the current size is needed.
    /// </summary>
    /// <returns>the approximate size without fonts or templates</returns>
    public int CurrentDocumentSize => Body.Offset + Body.Size * 20 + 0x48;

    public virtual int CurrentPageNumber => currentPageNumber;

    /// <summary>
    ///     [M1] Color settings
    /// </summary>
    /// <summary>
    ///     Gets the default colorspaces.
    /// </summary>
    /// <returns>the default colorspaces</returns>
    public PdfDictionary DefaultColorspace => defaultColorspace;

    /// <summary>
    ///     the PdfDirectContentByte instances
    /// </summary>
    /// <summary>
    ///     Use this method to get the direct content for this document.
    ///     There is only one direct content, multiple calls to this method
    ///     will allways retrieve the same object.
    /// </summary>
    /// <returns>the direct content</returns>
    public virtual PdfContentByte DirectContent
    {
        get
        {
            if (!open)
            {
                throw new InvalidOperationException("The document is not open.");
            }

            return directContent;
        }
    }

    /// <summary>
    ///     Use this method to get the direct content under for this document.
    ///     There is only one direct content, multiple calls to this method
    ///     will allways retrieve the same object.
    /// </summary>
    /// <returns>the direct content</returns>
    public virtual PdfContentByte DirectContentUnder
    {
        get
        {
            if (!open)
            {
                throw new InvalidOperationException("The document is not open.");
            }

            return directContentUnder;
        }
    }

    /// <summary>
    ///     Sets extra keys to the catalog.
    /// </summary>
    /// <returns>the catalog to change</returns>
    public PdfDictionary ExtraCatalog
    {
        get
        {
            if (extraCatalog == null)
            {
                extraCatalog = new PdfDictionary();
            }

            return extraCatalog;
        }
    }

    /// <summary>
    ///     [F2] compression
    /// </summary>
    /// <summary>
    ///     Gets the 1.5 compression status.
    /// </summary>
    /// <returns> true  if the 1.5 compression is on</returns>
    public bool FullCompression => fullCompression;

    /// <summary>
    ///     [U5] Transparency groups
    /// </summary>
    public PdfDictionary Group
    {
        get => group;
        set => group = value;
    }

    /// <summary>
    ///     Use this method to get the info dictionary if you want to
    ///     change it directly (add keys and values to the info dictionary).
    /// </summary>
    /// <returns>the info dictionary</returns>
    public PdfDictionary Info => ((PdfDocument)Document).Info;

    /// <summary>
    ///     Gets the <B>Optional Content Properties Dictionary</B>. Each call fills the dictionary with the current layer
    ///     state. It's advisable to only call this method right before close and do any modifications
    ///     at that time.
    /// </summary>
    /// <returns>the Optional Content Properties Dictionary</returns>
    public PdfOcProperties OcProperties
    {
        get
        {
            FillOcProperties(true);

            return VOcProperties;
        }
    }

    /// <summary>
    ///     Sets the bookmarks. The list structure is defined in
    ///     {@link SimpleBookmark}.
    /// </summary>
    public IList<INullValueDictionary<string, object>> Outlines
    {
        set => NewBookmarks = value;
    }

    /// <summary>
    ///     If you use SetPageEmpty(false), invoking NewPage() after a blank page will add a newPage.
    /// </summary>
    public bool PageEmpty
    {
        set => Pdf.PageEmpty = value;
    }

    /// <summary>
    ///     Page events are specific for iText, not for PDF.
    ///     Upon specific events (for instance when a page starts
    ///     or ends), the corresponing method in the page event
    ///     implementation that is added to the writer is invoked.
    /// </summary>
    /// <summary>
    ///     Gets the  PdfPageEvent  for this document or  null
    ///     if none is set.
    ///     if none is set
    /// </summary>
    /// <returns>the  PdfPageEvent  for this document or  null </returns>
    public IPdfPageEvent PageEvent
    {
        get => _pageEvent;
        set
        {
            if (value == null)
            {
                _pageEvent = null;
            }
            else if (_pageEvent == null)
            {
                _pageEvent = value;
            }
            else if (_pageEvent is PdfPageEventForwarder)
            {
                ((PdfPageEventForwarder)_pageEvent).AddPageEvent(value);
            }
            else
            {
                var forward = new PdfPageEventForwarder();
                forward.AddPageEvent(_pageEvent);
                forward.AddPageEvent(value);
                _pageEvent = forward;
            }
        }
    }

    /// <summary>
    ///     Use this method to add page labels
    /// </summary>
    public virtual PdfPageLabels PageLabels
    {
        set => Pdf.PageLabels = value;
    }

    /// <summary>
    ///     Gets the pagenumber of this document.
    ///     This number can be different from the real pagenumber,
    ///     if you have (re)set the page number previously.
    /// </summary>
    /// <returns>a page number</returns>
    public int PageNumber => Pdf.PageNumber;

    /// <summary>
    ///     Gives the size of the media box.
    /// </summary>
    /// <returns>a Rectangle</returns>
    public new Rectangle PageSize => Pdf.PageSize;

    /// <summary>
    ///     Use this method to set the XMP Metadata for each page.
    /// </summary>
    public byte[] PageXmpMetadata
    {
        set => Pdf.XmpMetadata = value;
    }

    /// <summary>
    ///     Gets a  PdfIndirectReference  for an object that
    ///     will be created in the future.
    /// </summary>
    /// <returns>the  PdfIndirectReference </returns>
    public PdfIndirectReference PdfIndirectReference => Body.PdfIndirectReference;

    /// <summary>
    ///     Sets the transparency blending colorspace to RGB. The default blending colorspace is
    ///     CMYK and will result in faded colors in the screen and in printing. Calling this method
    ///     will return the RGB colors to what is expected. The RGB blending will be applied to all subsequent pages
    ///     until other value is set.
    ///     Note that this is a generic solution that may not work in all cases.
    ///     to use the default blending colorspace
    /// </summary>
    public bool RgbTransparencyBlending
    {
        get => _rgbTransparencyBlending;
        set => _rgbTransparencyBlending = value;
    }

    /// <summary>
    ///     Use this method to get the root outline
    ///     and construct bookmarks.
    /// </summary>
    /// <returns>the root outline</returns>
    public PdfOutline RootOutline => directContent.RootOutline;

    /// <summary>
    ///     Sets the ratio between the extra word spacing and the extra character spacing
    ///     when the text is fully justified.
    ///     Extra word spacing will grow  spaceCharRatio  times more than extra character spacing.
    ///     If the ratio is  PdfWriter.NO_SPACE_CHAR_RATIO  then the extra character spacing
    ///     will be zero.
    /// </summary>
    public virtual float SpaceCharRatio
    {
        set
        {
            if (value < 0.001f)
            {
                _spaceCharRatio = 0.001f;
            }
            else
            {
                _spaceCharRatio = value;
            }
        }
        get => _spaceCharRatio;
    }

    /// <summary>
    ///     Sets the image sequence to follow the text in strict order.
    /// </summary>
    public bool StrictImageSequence
    {
        set => Pdf.StrictImageSequence = value;
        get => Pdf.StrictImageSequence;
    }

    /// <summary>
    ///     Gets the structure tree root. If the document is not marked for tagging it will return  null .
    /// </summary>
    /// <returns>the structure tree root</returns>
    public PdfStructureTreeRoot StructureTreeRoot
    {
        get
        {
            if (Tagged && structureTreeRoot == null)
            {
                structureTreeRoot = new PdfStructureTreeRoot(this);
            }

            return structureTreeRoot;
        }
    }

    /// <summary>
    ///     Sets the value for the Tabs entry in the page tree.
    ///     Since the Adobe Extensions Level 3, it can also be PdfName.A
    ///     or PdfName.W
    ///     @since	2.1.5
    /// </summary>
    public PdfName Tabs
    {
        get => tabs;
        set => tabs = value;
    }

    /// <summary>
    ///     Adds additional entries to the page dictionary.
    /// </summary>
    public PdfDictionary PageDictionary { set; get; } = new();

    /// <summary>
    ///     Sets the the thumbnail image for the current page.
    ///     @throws PdfException on error
    ///     @throws DocumentException or error
    /// </summary>
    public virtual Image Thumbnail
    {
        set => Pdf.Thumbnail = value;
    }

    /// <summary>
    ///     Sets the flag indicating the presence of structure elements that contain user properties attributes.
    /// </summary>
    public bool UserProperties
    {
        set => _userProperties = value;
        get => _userProperties;
    }

    /// <summary>
    ///     [U4] Thumbnail image
    /// </summary>
    /// <summary>
    ///     [U8] user units
    /// </summary>
    /// <summary>
    ///     A UserUnit is a value that defines the default user space unit.
    ///     The minimum UserUnit is 1 (1 unit = 1/72 inch).
    ///     The maximum UserUnit is 75,000.
    ///     Remark that this userunit only works starting with PDF1.6!
    /// </summary>
    public float Userunit
    {
        get => userunit;
        set
        {
            if (value < 1f || value > 75000f)
            {
                throw new DocumentException("UserUnit should be a value between 1 and 75000.");
            }

            userunit = value;
            SetAtLeastPdfVersion(VERSION_1_6);
        }
    }

    /// <summary>
    ///     [C9] Metadata
    /// </summary>
    /// <summary>
    ///     Sets XMP Metadata.
    /// </summary>
    public byte[] XmpMetadata
    {
        set => xmpMetadata = value;
        get => xmpMetadata;
    }

    internal virtual PdfIndirectReference CurrentPage => GetPageReference(currentPageNumber);

    /// <summary>
    ///     PDF Objects that have an impact on the PDF body
    /// </summary>
    internal PdfEncryption Encryption => Crypto;

    internal int IndirectReferenceNumber => Body.IndirectReferenceNumber;

    /// <summary>
    ///     Returns the outputStreamCounter.
    /// </summary>
    /// <returns>the outputStreamCounter</returns>
    internal new OutputStreamCounter Os => base.Os;

    /// <summary>
    ///     Gets the  PdfDocument  associated with this writer.
    /// </summary>
    /// <returns>the  PdfDocument </returns>
    internal PdfDocument PdfDocument => Pdf;

    public PdfAcroForm AcroForm => Pdf.AcroForm;

    /// <summary>
    ///     Set the signature flags.
    /// </summary>
    public virtual int SigFlags
    {
        set => Pdf.SigFlags = value;
    }

    /// <summary>
    ///     [C8] AcroForm
    /// </summary>
    /// <summary>
    ///     Gets the AcroForm object.
    /// </summary>
    /// <returns>the  PdfAcroForm </returns>
    /// <summary>
    ///     Adds a  PdfAnnotation  or a  PdfFormField
    ///     to the document. Only the top parent of a  PdfFormField
    ///     needs to be added.
    /// </summary>
    /// <param name="annot">the  PdfAnnotation  or the  PdfFormField  to add</param>
    public virtual void AddAnnotation(PdfAnnotation annot)
        => Pdf.AddAnnotation(annot);

    /// <summary>
    ///     Adds the  PdfAnnotation  to the calculation order
    ///     array.
    /// </summary>
    /// <param name="annot">the  PdfAnnotation  to be added</param>
    public virtual void AddCalculationOrder(PdfFormField annot)
        => Pdf.AddCalculationOrder(annot);

    /// <summary>
    ///     Additional-actions defining the actions to be taken in
    ///     response to various trigger events affecting the document
    ///     as a whole. The actions types allowed are:  DOCUMENT_CLOSE ,
    ///     WILL_SAVE ,  DID_SAVE ,  WILL_PRINT
    ///     and  DID_PRINT .
    ///     @throws PdfException on invalid action type
    /// </summary>
    /// <param name="actionType">the action type</param>
    /// <param name="action">the action to execute in response to the trigger</param>
    public virtual void SetAdditionalAction(PdfName actionType, PdfAction action)
    {
        if (actionType == null)
        {
            throw new ArgumentNullException(nameof(actionType));
        }

        if (!(actionType.Equals(DocumentClose) || actionType.Equals(WillSave) || actionType.Equals(DidSave) ||
              actionType.Equals(WillPrint) || actionType.Equals(DidPrint)))
        {
            throw new PdfException("Invalid additional action type: " + actionType);
        }

        Pdf.AddAdditionalAction(actionType, action);
    }

    /// <summary>
    ///     When the document opens it will jump to the destination with
    ///     this name.
    /// </summary>
    /// <param name="name">the name of the destination to jump to</param>
    public virtual void SetOpenAction(string name)
        => Pdf.SetOpenAction(name);

    /// <summary>
    ///     When the document opens this  action  will be
    ///     invoked.
    /// </summary>
    /// <param name="action">the action to be invoked</param>
    public virtual void SetOpenAction(PdfAction action)
        => Pdf.SetOpenAction(action);

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="encryptionType">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128
    ///     or ENCRYPTION_AES128.
    /// </param>
    public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, int encryptionType)
    {
        if (Pdf.IsOpen())
        {
            throw new DocumentException("Encryption can only be added before opening the document.");
        }

        Crypto = new PdfEncryption();
        Crypto.SetCryptoMode(encryptionType, 0);
        Crypto.SetupAllKeys(userPassword, ownerPassword, permissions);
    }

    /// <summary>
    ///     Sets the certificate encryption options for this document. An array of one or more public certificates
    ///     must be provided together with an array of the same size for the permissions for each certificate.
    ///     The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="certs">the public certificates to be used for the encryption</param>
    /// <param name="permissions">the user permissions for each of the certicates</param>
    /// <param name="encryptionType">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128
    ///     or ENCRYPTION_AES128.
    /// </param>
    public void SetEncryption(X509Certificate[] certs, int[] permissions, int encryptionType)
    {
        if (permissions == null)
        {
            throw new ArgumentNullException(nameof(permissions));
        }

        if (Pdf.IsOpen())
        {
            throw new DocumentException("Encryption can only be added before opening the document.");
        }

        Crypto = new PdfEncryption();

        if (certs != null)
        {
            for (var i = 0; i < certs.Length; i++)
            {
                Crypto.AddRecipient(certs[i], permissions[i]);
            }
        }

        Crypto.SetCryptoMode(encryptionType, 0);
        Crypto.GetEncryptionDictionary();
    }

    /// <summary>
    ///     Sets the display duration for the page (for presentations)
    /// </summary>
    public virtual int Duration
    {
        set => Pdf.Duration = value;
    }

    /// <summary>
    ///     Sets the transition for the page
    /// </summary>
    public virtual PdfTransition Transition
    {
        set => Pdf.Transition = value;
    }

    /// <summary>
    ///     User methods to change aspects of the page
    /// </summary>
    /// <summary>
    ///     [U2] take care of empty pages
    /// </summary>
    /// <summary>
    ///     [U3] page actions (open and close)
    /// </summary>
    /// <summary>
    ///     Sets the open and close page additional action.
    ///     or  PdfWriter.PAGE_CLOSE
    ///     @throws PdfException if the action type is invalid
    /// </summary>
    /// <param name="actionType">the action type. It can be  PdfWriter.PAGE_OPEN </param>
    /// <param name="action">the action to perform</param>
    public virtual void SetPageAction(PdfName actionType, PdfAction action)
    {
        if (actionType == null)
        {
            throw new ArgumentNullException(nameof(actionType));
        }

        if (!actionType.Equals(PageOpen) && !actionType.Equals(PageClose))
        {
            throw new PdfException("Invalid page additional action type: " + actionType);
        }

        Pdf.SetPageAction(actionType, action);
    }

    /// <summary>
    ///     Sets the run direction. This is only used as a placeholder
    ///     as it does not affect anything.
    /// </summary>
    public virtual int RunDirection
    {
        set
        {
            if (value < RUN_DIRECTION_NO_BIDI || value > RUN_DIRECTION_RTL)
            {
                throw new InvalidOperationException("Invalid run direction: " + value);
            }

            runDirection = value;
        }
        get => runDirection;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(char)
    /// </summary>
    public virtual char PdfVersion
    {
        set => pdf_version.PdfVersion = value;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfVersion#addDeveloperExtension(com.lowagie.text.pdf.PdfDeveloperExtension)
    ///     @since   2.1.6
    /// </summary>
    public void AddDeveloperExtension(PdfDeveloperExtension de)
        => pdf_version.AddDeveloperExtension(de);

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfVersion#setAtLeastPdfVersion(char)
    /// </summary>
    public void SetAtLeastPdfVersion(char version)
        => pdf_version.SetAtLeastPdfVersion(version);

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(com.lowagie.text.pdf.PdfName)
    /// </summary>
    public void SetPdfVersion(PdfName version)
        => pdf_version.SetPdfVersion(version);

    /// <summary>
    ///     Sets the viewer preferences as the sum of several constants.
    ///     @see PdfViewerPreferences#setViewerPreferences
    /// </summary>
    public virtual int ViewerPreferences
    {
        set => Pdf.ViewerPreferences = value;
    }

    /// <summary>
    ///     Adds a viewer preference
    ///     @see PdfViewerPreferences#addViewerPreference
    /// </summary>
    public virtual void AddViewerPreference(PdfName key, PdfObject value)
        => Pdf.AddViewerPreference(key, value);

    /// <summary>
    ///     Sets the PDFX conformance level. Allowed values are PDFX1A2001 and PDFX32002. It
    ///     must be called before opening the document.
    /// </summary>
    public int PdfxConformance
    {
        set
        {
            if (_pdfxConformance.PdfxConformance == value)
            {
                return;
            }

            if (Pdf.IsOpen())
            {
                throw new PdfXConformanceException("PDFX conformance can only be set before opening the document.");
            }

            if (Crypto != null)
            {
                throw new PdfXConformanceException("A PDFX conforming document cannot be encrypted.");
            }

            if (value == PDFA1A || value == PDFA1B)
            {
                PdfVersion = VERSION_1_4;
            }
            else if (value != PDFXNONE)
            {
                PdfVersion = VERSION_1_3;
            }

            _pdfxConformance.PdfxConformance = value;
        }
        get => _pdfxConformance.PdfxConformance;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfXConformance#isPdfX()
    /// </summary>
    public bool IsPdfX()
        => _pdfxConformance.IsPdfX();

    public static PdfWriter GetInstance(Document document, Stream os)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var pdf = new PdfDocument();
        document.AddDocListener(pdf);
        var writer = new PdfWriter(pdf, os);
        pdf.AddWriter(writer);

        return writer;
    }

    public static PdfWriter GetInstance(Document document, Stream os, IDocListener listener)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var pdf = new PdfDocument();
        pdf.AddDocListener(listener);
        document.AddDocListener(pdf);
        var writer = new PdfWriter(pdf, os);
        pdf.AddWriter(writer);

        return writer;
    }

    /// <summary>
    ///     Adds an image to the document but not to the page resources. It is used with
    ///     templates and  Document.Add(Image) .
    ///     @throws PdfException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="image">the  Image  to add</param>
    /// <returns>the name of the image added</returns>
    public PdfName AddDirectImageSimple(Image image)
        => AddDirectImageSimple(image, null);

    /// <summary>
    ///     Adds an image to the document but not to the page resources. It is used with
    ///     templates and  Document.Add(Image) .
    ///     a  PdfIndirectReference  or a  PRIndirectReference .
    ///     @throws PdfException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="image">the  Image  to add</param>
    /// <param name="fixedRef">the reference to used. It may be  null ,</param>
    /// <returns>the name of the image added</returns>
    public PdfName AddDirectImageSimple(Image image, PdfIndirectReference fixedRef)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        PdfName name;

        // if the images is already added, just retrieve the name
        if (_images.TryGetValue(image.MySerialId, out var image1))
        {
            name = image1;
        }

        // if it's a new image, add it to the document
        else
        {
            if (image.IsImgTemplate())
            {
                name = new PdfName("img" + _images.Count);

                if (image is ImgWmf wmf)
                {
                    wmf.ReadWmf(PdfTemplate.CreateTemplate(this, 0, 0));
                }
            }
            else
            {
                var dref = image.DirectReference;

                if (dref != null)
                {
                    var rname = new PdfName("img" + _images.Count);
                    _images[image.MySerialId] = rname;
                    ImageDictionary.Put(rname, dref);

                    return rname;
                }

                var maskImage = image.ImageMask;
                PdfIndirectReference maskRef = null;

                if (maskImage != null)
                {
                    var mname = _images[maskImage.MySerialId];
                    maskRef = GetImageReference(mname);
                }

                var i = new PdfImage(image, "img" + _images.Count, maskRef);

                if (image is ImgJbig2)
                {
                    var globals = ((ImgJbig2)image).GlobalBytes;

                    if (globals != null)
                    {
                        var decodeparms = new PdfDictionary();
                        decodeparms.Put(PdfName.Jbig2Globals, GetReferenceJbig2Globals(globals));
                        i.Put(PdfName.Decodeparms, decodeparms);
                    }
                }

                if (image.HasIccProfile())
                {
                    var icc = new PdfIccBased(image.TagIcc, image.CompressionLevel);
                    var iccRef = Add(icc);
                    var iccArray = new PdfArray();
                    iccArray.Add(PdfName.Iccbased);
                    iccArray.Add(iccRef);
                    var colorspace = i.GetAsArray(PdfName.Colorspace);

                    if (colorspace != null)
                    {
                        if (colorspace.Size > 1 && PdfName.Indexed.Equals(colorspace[0]))
                        {
                            colorspace[1] = iccArray;
                        }
                        else
                        {
                            i.Put(PdfName.Colorspace, iccArray);
                        }
                    }
                    else
                    {
                        i.Put(PdfName.Colorspace, iccArray);
                    }
                }

                Add(i, fixedRef);
                name = i.Name;
            }

            _images[image.MySerialId] = name;
        }

        return name;
    }

    /// <summary>
    ///     Adds a file attachment at the document level.
    ///     the file will be read from the disk
    ///     fileStore  is not  null
    ///     @throws IOException on error
    /// </summary>
    /// <param name="description">the file description</param>
    /// <param name="fileStore">an array with the file. If it's  null </param>
    /// <param name="file">the path to the file. It will only be used if</param>
    /// <param name="fileDisplay">the actual file name stored in the pdf</param>
    public virtual void AddFileAttachment(string description, byte[] fileStore, string file, string fileDisplay)
        => AddFileAttachment(description, PdfFileSpecification.FileEmbedded(this, file, fileDisplay, fileStore));

    /// <summary>
    ///     Adds a file attachment at the document level.
    /// </summary>
    /// <param name="description">the file description</param>
    /// <param name="fs">the file specification</param>
    public virtual void AddFileAttachment(string description, PdfFileSpecification fs)
    {
        if (fs == null)
        {
            throw new ArgumentNullException(nameof(fs));
        }

        Pdf.AddFileAttachment(description, fs);
    }

    /// <summary>
    ///     Adds a file attachment at the document level.
    /// </summary>
    /// <param name="fs">the file specification</param>
    public void AddFileAttachment(PdfFileSpecification fs)
    {
        if (fs == null)
        {
            throw new ArgumentNullException(nameof(fs));
        }

        Pdf.AddFileAttachment(null, fs);
    }

    /// <summary>
    ///     Use this method to add a JavaScript action at the document level.
    ///     When the document opens, all this JavaScript runs.
    /// </summary>
    /// <param name="js">The JavaScript action</param>
    public virtual void AddJavaScript(PdfAction js)
    {
        if (js == null)
        {
            throw new ArgumentNullException(nameof(js));
        }

        Pdf.AddJavaScript(js);
    }

    /// <summary>
    ///     [C4] Page labels
    /// </summary>
    /// <summary>
    ///     [C5] named objects: named destinations, javascript, embedded files
    /// </summary>
    /// <summary>
    ///     Adds a JavaScript action at the document level. When the document
    ///     opens all this JavaScript runs.
    ///     Acrobat JavaScript engine does not support unicode,
    ///     so this may or may not work for you
    /// </summary>
    /// <param name="code">the JavaScript code</param>
    /// <param name="unicode">select JavaScript unicode. Note that the internal</param>
    public virtual void AddJavaScript(string code, bool unicode)
        => AddJavaScript(PdfAction.JavaScript(code, this, unicode));

    /// <summary>
    ///     Adds a JavaScript action at the document level. When the document
    ///     opens all this JavaScript runs.
    /// </summary>
    /// <param name="code">the JavaScript code</param>
    public virtual void AddJavaScript(string code)
        => AddJavaScript(code, false);

    /// <summary>
    ///     Use this method to add a JavaScript action at the document level.
    ///     When the document opens, all this JavaScript runs.
    /// </summary>
    /// <param name="name">The name of the JS Action in the name tree</param>
    /// <param name="js">The JavaScript action</param>
    public void AddJavaScript(string name, PdfAction js)
    {
        if (js == null)
        {
            throw new ArgumentNullException(nameof(js));
        }

        Pdf.AddJavaScript(name, js);
    }

    /// <summary>
    ///     Use this method to add a JavaScript action at the document level.
    ///     When the document opens, all this JavaScript runs.
    ///     Acrobat JavaScript engine does not support unicode,
    ///     so this may or may not work for you
    /// </summary>
    /// <param name="name">The name of the JS Action in the name tree</param>
    /// <param name="code">the JavaScript code</param>
    /// <param name="unicode">select JavaScript unicode. Note that the internal</param>
    public void AddJavaScript(string name, string code, bool unicode)
        => AddJavaScript(name, PdfAction.JavaScript(code, this, unicode));

    /// <summary>
    ///     Use this method to adds a JavaScript action at the document level.
    ///     When the document opens, all this JavaScript runs.
    /// </summary>
    /// <param name="name">The name of the JS Action in the name tree</param>
    /// <param name="code">the JavaScript code</param>
    public void AddJavaScript(string name, string code)
        => AddJavaScript(name, code, false);

    /// <summary>
    ///     Sets a collection of optional content groups whose states are intended to follow
    ///     a "radio button" paradigm. That is, the state of at most one optional
    ///     content group in the array should be ON at a time: if one group is turned
    ///     ON, all others must be turned OFF.
    /// </summary>
    /// <param name="group">the radio group</param>
    public void AddOcgRadioGroup(IList<PdfLayer> group)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        var ar = new PdfArray();

        for (var k = 0; k < group.Count; ++k)
        {
            var layer = group[k];

            if (layer.Title == null)
            {
                ar.Add(layer.Ref);
            }
        }

        if (ar.Size == 0)
        {
            return;
        }

        OcgRadioGroup.Add(ar);
    }

    /// <summary>
    ///     Adds an object to the PDF body.
    ///     @throws IOException
    /// </summary>
    /// <param name="objecta"></param>
    /// <returns>a PdfIndirectObject</returns>
    public PdfIndirectObject AddToBody(PdfObject objecta)
    {
        var iobj = Body.Add(objecta);

        return iobj;
    }

    /// <summary>
    ///     Adds an object to the PDF body.
    ///     @throws IOException
    /// </summary>
    /// <param name="objecta"></param>
    /// <param name="inObjStm"></param>
    /// <returns>a PdfIndirectObject</returns>
    public PdfIndirectObject AddToBody(PdfObject objecta, bool inObjStm)
    {
        var iobj = Body.Add(objecta, inObjStm);

        return iobj;
    }

    /// <summary>
    ///     Adds an object to the PDF body.
    ///     @throws IOException
    /// </summary>
    /// <param name="objecta"></param>
    /// <param name="refa"></param>
    /// <returns>a PdfIndirectObject</returns>
    public PdfIndirectObject AddToBody(PdfObject objecta, PdfIndirectReference refa)
    {
        if (refa == null)
        {
            throw new ArgumentNullException(nameof(refa));
        }

        var iobj = Body.Add(objecta, refa);

        return iobj;
    }

    /// <summary>
    ///     Adds an object to the PDF body.
    ///     @throws IOException
    /// </summary>
    /// <param name="objecta"></param>
    /// <param name="refa"></param>
    /// <param name="inObjStm"></param>
    /// <returns>a PdfIndirectObject</returns>
    public PdfIndirectObject AddToBody(PdfObject objecta, PdfIndirectReference refa, bool inObjStm)
    {
        if (refa == null)
        {
            throw new ArgumentNullException(nameof(refa));
        }

        var iobj = Body.Add(objecta, refa, inObjStm);

        return iobj;
    }

    /// <summary>
    ///     Adds an object to the PDF body.
    ///     @throws IOException
    /// </summary>
    /// <param name="objecta"></param>
    /// <param name="refNumber"></param>
    /// <returns>a PdfIndirectObject</returns>
    public PdfIndirectObject AddToBody(PdfObject objecta, int refNumber)
    {
        var iobj = Body.Add(objecta, refNumber);

        return iobj;
    }

    /// <summary>
    ///     Adds an object to the PDF body.
    ///     @throws IOException
    /// </summary>
    /// <param name="objecta"></param>
    /// <param name="refNumber"></param>
    /// <param name="inObjStm"></param>
    /// <returns>a PdfIndirectObject</returns>
    public PdfIndirectObject AddToBody(PdfObject objecta, int refNumber, bool inObjStm)
    {
        if (objecta == null)
        {
            throw new ArgumentNullException(nameof(objecta));
        }

        var iobj = Body.Add(objecta, refNumber, inObjStm);

        return iobj;
    }

    /// <summary>
    ///     [M3] Images
    /// </summary>
    /// <summary>
    ///     Clears text wrapping around images (if applicable).
    ///     Method suggested by Pelikan Stephan
    ///     @throws DocumentException
    /// </summary>
    public void ClearTextWrap()
        => Pdf.ClearTextWrap();

    /// <summary>
    ///     Signals that the  Document  was closed and that no other
    ///     Elements  will be added.
    ///     The pages-tree is built and written to the outputstream.
    ///     A Catalog is constructed, as well as an Info-object,
    ///     the referencetable is composed and everything is written
    ///     to the outputstream embedded in a Trailer.
    /// </summary>
    public override void Close()
    {
        if (open)
        {
            if (currentPageNumber - 1 != PageReferences.Count)
            {
                throw new InvalidOperationException("The page " + PageReferences.Count +
                                                    " was requested but the document has only " +
                                                    (currentPageNumber - 1) + " pages.");
            }

            Pdf.Close();
            AddSharedObjectsToBody();

            // add the root to the body
            var rootRef = Root.WritePageTree();

            // make the catalog-object and add it to the body
            var catalog = GetCatalog(rootRef);

            // [C9] if there is XMP data to add: add it
            if (xmpMetadata != null)
            {
                var xmp = new PdfStream(xmpMetadata);
                xmp.Put(PdfName.TYPE, PdfName.Metadata);
                xmp.Put(PdfName.Subtype, PdfName.Xml);

                if (Crypto != null && !Crypto.IsMetadataEncrypted())
                {
                    var ar = new PdfArray();
                    ar.Add(PdfName.Crypt);
                    xmp.Put(PdfName.Filter, ar);
                }

                catalog.Put(PdfName.Metadata, Body.Add(xmp).IndirectReference);
            }

            // [C10] make pdfx conformant
            if (IsPdfX())
            {
                _pdfxConformance.CompleteInfoDictionary(Info);
                _pdfxConformance.CompleteExtraCatalog(ExtraCatalog);
            }

            // [C11] Output Intents
            if (extraCatalog != null)
            {
                catalog.MergeDifferent(extraCatalog);
            }

            WriteOutlines(catalog, false);

            // add the Catalog to the body
            var indirectCatalog = AddToBody(catalog, false);

            // add the info-object to the body
            var infoObj = AddToBody(Info, false);

            // [F1] encryption
            PdfIndirectReference encryption = null;
            PdfObject fileId = null;
            Body.FlushObjStm();

            if (Crypto != null)
            {
                var encryptionObject = AddToBody(Crypto.GetEncryptionDictionary(), false);
                encryption = encryptionObject.IndirectReference;
                fileId = Crypto.FileId;
            }
            else
            {
                fileId = PdfEncryption.CreateInfoId(PdfEncryption.CreateDocumentId());
            }

            // write the cross-reference table of the body
            Body.WriteCrossReferenceTable(base.Os, indirectCatalog.IndirectReference, infoObj.IndirectReference,
                encryption, fileId, Prevxref);

            // make the trailer
            // [F2] full compression
            if (fullCompression)
            {
                var tmp = GetIsoBytes("startxref\n");
                base.Os.Write(tmp, 0, tmp.Length);
                tmp = GetIsoBytes(Body.Offset.ToString(CultureInfo.InvariantCulture));
                base.Os.Write(tmp, 0, tmp.Length);
                tmp = GetIsoBytes("\n%%EOF\n");
                base.Os.Write(tmp, 0, tmp.Length);
            }
            else
            {
                var trailer = new PdfTrailer(Body.Size, Body.Offset, indirectCatalog.IndirectReference,
                    infoObj.IndirectReference, encryption, fileId, Prevxref);

                trailer.ToPdf(this, base.Os);
            }

            base.Close();
        }
    }

    /// <summary>
    ///     Creates XMP Metadata based on the metadata in the PdfDocument.
    /// </summary>
    public void CreateXmpMetadata()
        => XmpMetadata = createXmpMetadataBytes();

    /// <summary>
    ///     Checks if a  Table  fits the current page of the  PdfDocument .
    /// </summary>
    /// <param name="table">the table that has to be checked</param>
    /// <param name="margin">a certain margin</param>
    /// <returns> true  if the  Table  fits the page,  false  otherwise.</returns>
    public bool FitsPage(Table table, float margin)
        => Pdf.GetBottom(table) > Pdf.IndentBottom + margin;

    /// <summary>
    ///     [M4] Old table functionality; do we still need it?
    /// </summary>
    /// <summary>
    ///     Checks if a  Table  fits the current page of the  PdfDocument .
    /// </summary>
    /// <param name="table">the table that has to be checked</param>
    /// <returns> true  if the  Table  fits the page,  false  otherwise.</returns>
    public bool FitsPage(Table table)
        => FitsPage(table, 0);

    /// <summary>
    ///     Writes the reader to the document and frees the memory used by it.
    ///     The main use is when concatenating multiple documents to keep the
    ///     memory usage restricted to the current appending document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="reader">the  PdfReader  to free</param>
    public virtual void FreeReader(PdfReader reader)
    {
        CurrentPdfReaderInstance = ImportedPages[reader];

        if (CurrentPdfReaderInstance == null)
        {
            return;
        }

        CurrentPdfReaderInstance.WriteAllPages();
        CurrentPdfReaderInstance = null;
        ImportedPages.Remove(reader);
    }

    /// <summary>
    ///     Gives the size of a trim, art, crop or bleed box, or null if not defined.
    /// </summary>
    /// <param name="boxName">crop, trim, art or bleed</param>
    public Rectangle GetBoxSize(string boxName)
        => Pdf.GetBoxSize(boxName);

    /// <summary>
    ///     [F5] adding pages imported form other PDF documents
    /// </summary>
    /// <summary>
    ///     Gets a page from other PDF document. The page can be used as
    ///     any other PdfTemplate. Note that calling this method more than
    ///     once with the same parameters will retrieve the same object.
    /// </summary>
    /// <param name="reader">the PDF document where the page is</param>
    /// <param name="pageNumber">the page number. The first page is 1</param>
    /// <returns>the template representing the imported page</returns>
    public virtual PdfImportedPage GetImportedPage(PdfReader reader, int pageNumber)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var inst = ImportedPages[reader];

        if (inst == null)
        {
            inst = reader.GetPdfReaderInstance(this);
            ImportedPages[reader] = inst;
        }

        return inst.GetImportedPage(pageNumber);
    }

    /// <summary>
    ///     Use this method to get a reference to a page existing or not.
    ///     If the page does not exist yet the reference will be created
    ///     in advance. If on closing the document, a page number greater
    ///     than the total number of pages was requested, an exception
    ///     is thrown.
    /// </summary>
    /// <param name="page">the page number. The first page is 1</param>
    /// <returns>the reference to the page</returns>
    public virtual PdfIndirectReference GetPageReference(int page)
    {
        --page;

        if (page < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "The page numbers start at 1.");
        }

        PdfIndirectReference refa;

        if (page < PageReferences.Count)
        {
            refa = PageReferences[page];

            if (refa == null)
            {
                refa = Body.PdfIndirectReference;
                PageReferences[page] = refa;
            }
        }
        else
        {
            var empty = page - PageReferences.Count;

            for (var k = 0; k < empty; ++k)
            {
                PageReferences.Add(null);
            }

            refa = Body.PdfIndirectReference;
            PageReferences.Add(refa);
        }

        return refa;
    }

    /// <summary>
    ///     Use this method to get the current vertical page position.
    ///     for elements that do not terminate the lines they've started because those lines will get
    ///     terminated.
    /// </summary>
    /// <param name="ensureNewLine">Tells whether a new line shall be enforced. This may cause side effects</param>
    /// <returns>The current vertical page position.</returns>
    public float GetVerticalPosition(bool ensureNewLine)
        => Pdf.GetVerticalPosition(ensureNewLine);

    /// <summary>
    ///     Check if the document is marked for tagging.
    /// </summary>
    /// <returns> true  if the document is marked for tagging</returns>
    public bool IsTagged()
        => Tagged;

    /// <summary>
    ///     Use this method to lock an optional content group.
    ///     The state of a locked group cannot be changed through the user interface
    ///     of a viewer application. Producers can use this entry to prevent the visibility
    ///     of content that depends on these groups from being changed by users.
    ///     @since   2.1.2
    /// </summary>
    /// <param name="layer">the layer that needs to be added to the array of locked OCGs</param>
    public void LockLayer(PdfLayer layer)
    {
        if (layer == null)
        {
            throw new ArgumentNullException(nameof(layer));
        }

        OcgLocked.Add(layer.Ref);
    }

    /// <summary>
    ///     page events
    /// </summary>
    /// <summary>
    ///     Open en Close method + method that create the PDF
    /// </summary>
    /// <summary>
    ///     Signals that the  Document  has been opened and that
    ///     Elements  can be added.
    ///     When this method is called, the PDF-document header is
    ///     written to the outputstream.
    /// </summary>
    public override void Open()
    {
        base.Open();
        pdf_version.WriteHeader(base.Os);
        Body = new PdfBody(this);

        if (_pdfxConformance.IsPdfX32002())
        {
            var sec = new PdfDictionary();

            sec.Put(PdfName.Gamma, new PdfArray(_gammaValues));

            sec.Put(PdfName.Matrix, new PdfArray(_matrixValues));

            sec.Put(PdfName.Whitepoint, new PdfArray(_whitepointValues));

            var arr = new PdfArray(PdfName.Calrgb);
            arr.Add(sec);
            SetDefaultColorspace(PdfName.Defaultrgb, AddToBody(arr).IndirectReference);
        }
    }

    /// <summary>
    ///     Releases the memory used by a template by writing it to the output. The template
    ///     can still be added to any content but changes to the template itself won't have
    ///     any effect.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="tp">the template to release</param>
    public void ReleaseTemplate(PdfTemplate tp)
    {
        if (tp == null)
        {
            throw new ArgumentNullException(nameof(tp));
        }

        var refi = tp.IndirectReference;
        var objs = FormXObjects[refi];

        if (objs == null || objs[1] == null)
        {
            return;
        }

        var template = (PdfTemplate)objs[1];

        if (template.IndirectReference is PrIndirectReference)
        {
            return;
        }

        if (template.Type == PdfTemplate.TYPE_TEMPLATE)
        {
            AddToBody(template.GetFormXObject(compressionLevel), template.IndirectReference);
            objs[1] = null;
        }
    }

    /// <summary>
    ///     Use this method to reorder the pages in the document.
    ///     A  null  argument value only returns the number of pages to process.
    ///     It is advisable to issue a  Document.newPage()  before using this method.
    ///     same size as the number of pages.
    ///     @throws DocumentException if all the pages are not present in the array
    /// </summary>
    /// <param name="order">an array with the new page sequence. It must have the</param>
    /// <returns>the total number of pages</returns>
    public int ReorderPages(int[] order)
        => Root.ReorderPages(order);

    /// <summary>
    ///     [U1] page size
    /// </summary>
    /// <summary>
    ///     Sets the page box sizes. Allowed names are: "crop", "trim", "art" and "bleed".
    /// </summary>
    /// <param name="boxName">the box size</param>
    /// <param name="size">the size</param>
    public void SetBoxSize(string boxName, Rectangle size)
        => Pdf.SetBoxSize(boxName, size);

    /// <summary>
    ///     Miscellaneous topics
    /// </summary>
    /// <summary>
    ///     Sets the default colorspace that will be applied to all the document.
    ///     The colorspace is only applied if another colorspace with the same name
    ///     is not present in the content.
    ///     The colorspace is applied immediately when creating templates and at the page
    ///     end for the main document content.
    ///     or  PdfName.DEFAULTCMYK
    /// </summary>
    /// <param name="key">the name of the colorspace. It can be  PdfName.DEFAULTGRAY ,  PdfName.DEFAULTRGB </param>
    /// <param name="cs">the colorspace. A  null  or  PdfNull  removes any colorspace with the same name</param>
    public void SetDefaultColorspace(PdfName key, PdfObject cs)
    {
        if (cs == null || cs.IsNull())
        {
            defaultColorspace.Remove(key);
        }

        defaultColorspace.Put(key, cs);
    }

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    /// <param name="strength128Bits"> true  for 128 bit key length,  false  for 40 bit key length</param>
    public void SetEncryption(byte[] userPassword, byte[] ownerPassword, int permissions, bool strength128Bits)
        => SetEncryption(userPassword, ownerPassword, permissions,
            strength128Bits ? STANDARD_ENCRYPTION_128 : STANDARD_ENCRYPTION_40);

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="strength"> true  for 128 bit key length,  false  for 40 bit key length</param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    public void SetEncryption(bool strength, string userPassword, string ownerPassword, int permissions)
        => SetEncryption(GetIsoBytes(userPassword), GetIsoBytes(ownerPassword), permissions, strength);

    /// <summary>
    ///     Sets the encryption options for this document. The userPassword and the
    ///     ownerPassword can be null or have zero length. In this case the ownerPassword
    ///     is replaced by a random string. The open permissions for the document can be
    ///     AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
    ///     AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
    ///     The permissions can be combined by ORing them.
    ///     Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
    ///     @throws DocumentException if the document is already open
    /// </summary>
    /// <param name="encryptionType">
    ///     the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128
    ///     or ENCRYPTION_AES128.
    /// </param>
    /// <param name="userPassword">the user password. Can be null or empty</param>
    /// <param name="ownerPassword">the owner password. Can be null or empty</param>
    /// <param name="permissions">the user permissions</param>
    public void SetEncryption(int encryptionType, string userPassword, string ownerPassword, int permissions)
        => SetEncryption(GetIsoBytes(userPassword), GetIsoBytes(ownerPassword), permissions, encryptionType);

    /// <summary>
    ///     Sets the document's compression to the new 1.5 mode with object streams and xref
    ///     streams. It can be set at any time but once set it can't be unset.
    ///     If set before opening the document it will also set the pdf version to 1.5.
    /// </summary>
    public void SetFullCompression()
    {
        fullCompression = true;
        SetAtLeastPdfVersion(VERSION_1_5);
    }

    /// <summary>
    ///     PdfPages
    /// </summary>
    /// <summary>
    ///     Use this method to make sure the page tree has a lineair structure
    ///     (every leave is attached directly to the root).
    ///     Use this method to allow page reordering with method reorderPages.
    /// </summary>
    public void SetLinearPageMode()
        => Root.SetLinearMode(null);

    /// <summary>
    ///     Sets the values of the output intent dictionary. Null values are allowed to
    ///     suppress any key.
    ///     @throws IOException on error
    /// </summary>
    public void SetOutputIntents(string outputConditionIdentifier,
        string outputCondition,
        string registryName,
        string info,
        IccProfile colorProfile)
    {
        var outa = ExtraCatalog; //force the creation
        outa = new PdfDictionary(PdfName.Outputintent);

        if (outputCondition != null)
        {
            outa.Put(PdfName.Outputcondition, new PdfString(outputCondition, PdfObject.TEXT_UNICODE));
        }

        if (outputConditionIdentifier != null)
        {
            outa.Put(PdfName.Outputconditionidentifier,
                new PdfString(outputConditionIdentifier, PdfObject.TEXT_UNICODE));
        }

        if (registryName != null)
        {
            outa.Put(PdfName.Registryname, new PdfString(registryName, PdfObject.TEXT_UNICODE));
        }

        if (info != null)
        {
            outa.Put(PdfName.Info, new PdfString(info, PdfObject.TEXT_UNICODE));
        }

        if (colorProfile != null)
        {
            PdfStream stream = new PdfIccBased(colorProfile, compressionLevel);
            outa.Put(PdfName.Destoutputprofile, AddToBody(stream).IndirectReference);
        }

        PdfName intentSubtype;

        if (_pdfxConformance.IsPdfA1() || "PDFA/1".Equals(outputCondition, StringComparison.Ordinal))
        {
            intentSubtype = PdfName.GtsPdfa1;
        }
        else
        {
            intentSubtype = PdfName.GtsPdfx;
        }

        outa.Put(PdfName.S, intentSubtype);

        extraCatalog.Put(PdfName.Outputintents, new PdfArray(outa));
    }

    /// <summary>
    ///     [C11] Output intents
    /// </summary>
    /// <summary>
    ///     Sets the values of the output intent dictionary. Null values are allowed to
    ///     suppress any key.
    ///     Prefer the  ICC_Profile -based version of this method.
    ///     @since 1.x
    ///     @throws IOException
    /// </summary>
    /// <param name="outputConditionIdentifier">a value</param>
    /// <param name="outputCondition">a value, "PDFA/A" to force GTS_PDFA1, otherwise cued by pdfxConformance.</param>
    /// <param name="registryName">a value</param>
    /// <param name="info">a value</param>
    /// <param name="destOutputProfile">a value</param>
    public void SetOutputIntents(string outputConditionIdentifier,
        string outputCondition,
        string registryName,
        string info,
        byte[] destOutputProfile)
    {
        var colorProfile = destOutputProfile == null ? null : IccProfile.GetInstance(destOutputProfile);
        SetOutputIntents(outputConditionIdentifier, outputCondition, registryName, info, colorProfile);
    }

    /// <summary>
    ///     Copies the output intent dictionary from other document to this one.
    ///     dictionary,  false  to insert the dictionary if it exists
    ///     @throws IOException on error
    ///     otherwise
    /// </summary>
    /// <param name="reader">the other document</param>
    /// <param name="checkExistence"> true  to just check for the existence of a valid output intent</param>
    /// <returns> true  if the output intent dictionary exists,  false </returns>
    public bool SetOutputIntents(PdfReader reader, bool checkExistence)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var catalog = reader.Catalog;
        var outs = catalog.GetAsArray(PdfName.Outputintents);

        if (outs == null)
        {
            return false;
        }

        var arr = outs.ArrayList;

        if (outs.Size == 0)
        {
            return false;
        }

        var outa = outs.GetAsDict(0);
        var obj = PdfReader.GetPdfObject(outa.Get(PdfName.S));

        if (obj == null || !PdfName.GtsPdfx.Equals(obj))
        {
            return false;
        }

        if (checkExistence)
        {
            return true;
        }

        var stream = (PrStream)PdfReader.GetPdfObject(outa.Get(PdfName.Destoutputprofile));
        byte[] destProfile = null;

        if (stream != null)
        {
            destProfile = PdfReader.GetStreamBytes(stream);
        }

        SetOutputIntents(getNameString(outa, PdfName.Outputconditionidentifier),
            getNameString(outa, PdfName.Outputcondition), getNameString(outa, PdfName.Registryname),
            getNameString(outa, PdfName.Info), destProfile);

        return true;
    }

    /// <summary>
    ///     [F12] tagged PDF
    /// </summary>
    /// <summary>
    ///     Mark this document for tagging. It must be called before open.
    /// </summary>
    public void SetTagged()
    {
        if (open)
        {
            throw new ArgumentException("Tagging must be set before opening the document.");
        }

        Tagged = true;
    }

    /// <summary>
    ///     Adds some  PdfContents  to this Writer.
    ///     The document has to be open before you can begin to add content
    ///     to the body of the document.
    ///     @throws PdfException on error
    /// </summary>
    /// <param name="page">the  PdfPage  to add</param>
    /// <param name="contents">the  PdfContents  of the page</param>
    /// <returns>a  PdfIndirectReference </returns>
    internal virtual PdfIndirectReference Add(PdfPage page, PdfContents contents)
    {
        if (!open)
        {
            throw new PdfException("The document isn't open.");
        }

        PdfIndirectObject objecta;
        objecta = AddToBody(contents);
        page.Add(objecta.IndirectReference);

        if (group != null)
        {
            page.Put(PdfName.Group, group);
            group = null;
        }
        else if (_rgbTransparencyBlending)
        {
            var pp = new PdfDictionary();
            pp.Put(PdfName.TYPE, PdfName.Group);
            pp.Put(PdfName.S, PdfName.Transparency);
            pp.Put(PdfName.Cs, PdfName.Devicergb);
            page.Put(PdfName.Group, pp);
        }

        Root.AddPage(page);
        currentPageNumber++;

        return null;
    }

    /// <summary>
    ///     Writes a  PdfImage  to the outputstream.
    ///     @throws PdfException when a document isn't open yet, or has been closed
    /// </summary>
    /// <param name="pdfImage">the image to be added</param>
    /// <param name="fixedRef"></param>
    /// <returns>a  PdfIndirectReference  to the encapsulated image</returns>
    internal virtual PdfIndirectReference Add(PdfImage pdfImage, PdfIndirectReference fixedRef)
    {
        if (!ImageDictionary.Contains(pdfImage.Name))
        {
            PdfXConformanceImp.CheckPdfxConformance(this, PdfXConformanceImp.PDFXKEY_IMAGE, pdfImage);

            if (fixedRef is PrIndirectReference)
            {
                var r2 = (PrIndirectReference)fixedRef;
                fixedRef = new PdfIndirectReference(0, GetNewObjectNumber(r2.Reader, r2.Number, r2.Generation));
            }

            if (fixedRef == null)
            {
                fixedRef = AddToBody(pdfImage).IndirectReference;
            }
            else
            {
                AddToBody(pdfImage, fixedRef);
            }

            ImageDictionary.Put(pdfImage.Name, fixedRef);

            return fixedRef;
        }

        return (PdfIndirectReference)ImageDictionary.Get(pdfImage.Name);
    }

    internal virtual void AddAnnotation(PdfAnnotation annot, int page)
        => AddAnnotation(annot);

    /// <summary>
    ///     Adds a template to the document but not to the page resources.
    /// </summary>
    /// <param name="template">the template to add</param>
    /// <param name="forcedName">the template name, rather than a generated one. Can be null</param>
    /// <returns>the  PdfName  for this template</returns>
    internal PdfName AddDirectTemplateSimple(PdfTemplate template, PdfName forcedName)
    {
        var refa = template.IndirectReference;
        var obj = FormXObjects[refa];
        PdfName name = null;

        if (obj == null)
        {
            if (forcedName == null)
            {
                name = new PdfName("Xf" + FormXObjectsCounter);
                ++FormXObjectsCounter;
            }
            else
            {
                name = forcedName;
            }

            if (template.Type == PdfTemplate.TYPE_IMPORTED)
            {
                // If we got here from PdfCopy we'll have to fill importedPages
                var ip = (PdfImportedPage)template;
                var r = ip.PdfReaderInstance.Reader;

                if (!ImportedPages.ContainsKey(r))
                {
                    ImportedPages[r] = ip.PdfReaderInstance;
                }

                template = null;
            }

            FormXObjects[refa] = new object[]
            {
                name, template
            };
        }
        else
        {
            name = (PdfName)obj[0];
        }

        return name;
    }

    /// <summary>
    ///     A PDF file has 4 parts: a header, a body, a cross-reference table, and a trailer.
    ///     The body contains all the PDF objects that make up the PDF document.
    ///     Each element gets a reference (a set of numbers) and the byte position of
    ///     every object is stored in the cross-reference table.
    ///     Use these methods only if you know what you're doing.
    /// </summary>
    /// <summary>
    ///     Adds the local destinations to the body of the document.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="dest">the  Hashtable  containing the destinations</param>
    internal void AddLocalDestinations(OrderedTree dest)
    {
        foreach (string name in dest.Keys)
        {
            var obj = (object[])dest[name];
            var destination = (PdfDestination)obj[2];

            if (destination == null)
            {
                throw new InvalidOperationException("The name '" + name + "' has no local destination.");
            }

            if (obj[1] == null)
            {
                obj[1] = PdfIndirectReference;
            }

            AddToBody(destination, (PdfIndirectReference)obj[1]);
        }
    }

    /// <summary>
    ///     Adds a  BaseFont  to the document but not to the page resources.
    ///     It is used for templates.
    ///     and position 1 is an  PdfIndirectReference
    /// </summary>
    /// <param name="bf">the  BaseFont  to add</param>
    /// <returns>an  Object[]  where position 0 is a  PdfName </returns>
    internal FontDetails AddSimple(BaseFont bf)
    {
        if (bf.FontType == BaseFont.FONT_TYPE_DOCUMENT)
        {
            return new FontDetails(new PdfName("F" + FontNumber++), ((DocumentFont)bf).IndirectReference, bf);
        }

        var ret = DocumentFonts[bf];

        if (ret == null)
        {
            PdfXConformanceImp.CheckPdfxConformance(this, PdfXConformanceImp.PDFXKEY_FONT, bf);
            ret = new FontDetails(new PdfName("F" + FontNumber++), Body.PdfIndirectReference, bf);
            DocumentFonts[bf] = ret;
        }

        return ret;
    }

    /// <summary>
    ///     Adds a  SpotColor  to the document but not to the page resources.
    ///     and position 1 is an  PdfIndirectReference
    /// </summary>
    /// <param name="spc">the  SpotColor  to add</param>
    /// <returns>an  Object[]  where position 0 is a  PdfName </returns>
    internal ColorDetails AddSimple(PdfSpotColor spc)
    {
        var ret = DocumentColors[spc];

        if (ret == null)
        {
            ret = new ColorDetails(GetColorspaceName(), Body.PdfIndirectReference, spc);
            DocumentColors[spc] = ret;
        }

        return ret;
    }

    /// <summary>
    ///     [F10] extended graphics state (for instance for transparency)
    /// </summary>
    internal PdfObject[] AddSimpleExtGState(PdfDictionary gstate)
    {
        if (DocumentExtGState.TryGetValue(gstate, out var value))
        {
            return value;
        }

        PdfXConformanceImp.CheckPdfxConformance(this, PdfXConformanceImp.PDFXKEY_GSTATE, gstate);

        value = new PdfObject[]
        {
            new PdfName("GS" + (DocumentExtGState.Count + 1)), PdfIndirectReference
        };

        DocumentExtGState[gstate] = value;

        return value;
    }

    internal PdfName AddSimplePattern(PdfPatternPainter painter)
    {
        var name = DocumentPatterns[painter];

        if (name == null)
        {
            name = new PdfName("P" + PatternNumber);
            ++PatternNumber;
            DocumentPatterns[painter] = name;
        }

        return name;
    }

    internal ColorDetails AddSimplePatternColorspace(BaseColor color)
    {
        var type = ExtendedColor.GetType(color);

        if (type == ExtendedColor.TYPE_PATTERN || type == ExtendedColor.TYPE_SHADING)
        {
            throw new InvalidOperationException(
                "An uncolored tile pattern can not have another pattern or shading as color.");
        }

        switch (type)
        {
            case ExtendedColor.TYPE_RGB:
                if (PatternColorspaceRgb == null)
                {
                    PatternColorspaceRgb = new ColorDetails(GetColorspaceName(), Body.PdfIndirectReference, null);
                    var array = new PdfArray(PdfName.Pattern);
                    array.Add(PdfName.Devicergb);
                    AddToBody(array, PatternColorspaceRgb.IndirectReference);
                }

                return PatternColorspaceRgb;
            case ExtendedColor.TYPE_CMYK:
                if (PatternColorspaceCmyk == null)
                {
                    PatternColorspaceCmyk = new ColorDetails(GetColorspaceName(), Body.PdfIndirectReference, null);
                    var array = new PdfArray(PdfName.Pattern);
                    array.Add(PdfName.Devicecmyk);
                    AddToBody(array, PatternColorspaceCmyk.IndirectReference);
                }

                return PatternColorspaceCmyk;
            case ExtendedColor.TYPE_GRAY:
                if (PatternColorspaceGray == null)
                {
                    PatternColorspaceGray = new ColorDetails(GetColorspaceName(), Body.PdfIndirectReference, null);
                    var array = new PdfArray(PdfName.Pattern);
                    array.Add(PdfName.Devicegray);
                    AddToBody(array, PatternColorspaceGray.IndirectReference);
                }

                return PatternColorspaceGray;
            case ExtendedColor.TYPE_SEPARATION:
            {
                var details = AddSimple(((SpotColor)color).PdfSpotColor);
                var patternDetails = DocumentSpotPatterns[details];

                if (patternDetails == null)
                {
                    patternDetails = new ColorDetails(GetColorspaceName(), Body.PdfIndirectReference, null);
                    var array = new PdfArray(PdfName.Pattern);
                    array.Add(details.IndirectReference);
                    AddToBody(array, patternDetails.IndirectReference);
                    DocumentSpotPatterns[details] = patternDetails;
                }

                return patternDetails;
            }
            default:
                throw new InvalidOperationException("Invalid color type in PdfWriter.AddSimplePatternColorspace().");
        }
    }

    /// <summary>
    ///     [F11] adding properties (OCG, marked content)
    /// </summary>
    internal PdfObject[] AddSimpleProperty(object prop, PdfIndirectReference refi)
    {
        if (DocumentProperties.TryGetValue(prop, out var value))
        {
            return value;
        }

        if (prop is IPdfOcg)
        {
            PdfXConformanceImp.CheckPdfxConformance(this, PdfXConformanceImp.PDFXKEY_LAYER, null);
        }

        value = new PdfObject[]
        {
            new PdfName("Pr" + (DocumentProperties.Count + 1)), refi
        };

        DocumentProperties[prop] = value;

        return value;
    }

    /// <summary>
    ///     [F9] document shadings
    /// </summary>
    internal void AddSimpleShading(PdfShading shading)
    {
        if (!DocumentShadings.ContainsKey(shading))
        {
            DocumentShadings[shading] = null;
            shading.Name = DocumentShadings.Count;
        }
    }

    /// <summary>
    ///     [F8] shading patterns
    /// </summary>
    internal void AddSimpleShadingPattern(PdfShadingPattern shading)
    {
        if (!DocumentShadingPatterns.ContainsKey(shading))
        {
            shading.Name = PatternNumber;
            ++PatternNumber;
            DocumentShadingPatterns[shading] = null;
            AddSimpleShading(shading.Shading);
        }
    }

    internal void EliminateFontSubset(PdfDictionary fonts)
    {
        foreach (var ft in DocumentFonts.Values)
        {
            if (fonts.Get(ft.FontName) != null)
            {
                ft.Subset = false;
            }
        }
    }

    /// <summary>
    ///     [F6] spot colors
    /// </summary>
    internal PdfName GetColorspaceName()
        => new("CS" + ColorNumber++);

    /// <summary>
    ///     return the  PdfIndirectReference  to the image with a given name.
    /// </summary>
    /// <param name="name">the name of the image</param>
    /// <returns>a  PdfIndirectReference </returns>
    internal virtual PdfIndirectReference GetImageReference(PdfName name)
        => (PdfIndirectReference)ImageDictionary.Get(name);

    /// <summary>
    ///     Returns the version information.
    /// </summary>
    internal PdfVersionImp GetPdfVersion()
        => pdf_version;

    internal virtual RandomAccessFileOrArray GetReaderFile(PdfReader reader)
        => CurrentPdfReaderInstance.ReaderFile;

    internal bool PropertyExists(object prop)
        => DocumentProperties.ContainsKey(prop);

    internal void RegisterLayer(IPdfOcg layer)
    {
        PdfXConformanceImp.CheckPdfxConformance(this, PdfXConformanceImp.PDFXKEY_LAYER, null);

        if (layer is PdfLayer la)
        {
            if (la.Title == null)
            {
                if (!DocumentOcg.ContainsKey(la))
                {
                    DocumentOcg[la] = null;
                    DocumentOcGorder.Add(la);
                }
            }
            else
            {
                DocumentOcGorder.Add(la);
            }
        }
        else
        {
            throw new ArgumentException("Only PdfLayer is accepted.");
        }
    }

    /// <summary>
    ///     Resets all the direct contents to empty.
    ///     This happens when a new page is started.
    /// </summary>
    internal void ResetContent()
    {
        directContent.Reset();
        directContentUnder.Reset();
    }

    protected internal virtual int GetNewObjectNumber(PdfReader reader, int number, int generation)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (CurrentPdfReaderInstance == null)
        {
            CurrentPdfReaderInstance = reader.GetPdfReaderInstance(this);
        }

        return CurrentPdfReaderInstance.GetNewObjectNumber(number, generation);
    }

    /// <summary>
    ///     Gets an indirect reference to a JBIG2 Globals stream.
    ///     Adds the stream if it hasn't already been added to the writer.
    ///     @since  2.1.5
    /// </summary>
    /// <param name="content">a byte array that may already been added to the writer inside a stream object.</param>
    protected internal PdfIndirectReference GetReferenceJbig2Globals(byte[] content)
    {
        if (content == null)
        {
            return null;
        }

        foreach (var str in Jbig2Globals.Keys)
        {
            if (Arrays.AreEqual(content, str.GetBytes()))
            {
                return Jbig2Globals[str];
            }
        }

        var stream = new PdfStream(content);
        PdfIndirectObject refi;

        try
        {
            refi = AddToBody(stream);
        }
        catch (IOException)
        {
            return null;
        }

        Jbig2Globals[stream] = refi.IndirectReference;

        return refi.IndirectReference;
    }

    /// <summary>
    ///     [C1] Outlines (bookmarks)
    /// </summary>
    protected internal void WriteOutlines(PdfDictionary catalog, bool namedAsNames)
    {
        if (catalog == null)
        {
            throw new ArgumentNullException(nameof(catalog));
        }

        if (NewBookmarks == null || NewBookmarks.Count == 0)
        {
            return;
        }

        var top = new PdfDictionary();
        var topRef = PdfIndirectReference;
        var kids = SimpleBookmark.IterateOutlines(this, topRef, NewBookmarks, namedAsNames);
        top.Put(PdfName.First, (PdfIndirectReference)kids[0]);
        top.Put(PdfName.Last, (PdfIndirectReference)kids[1]);
        top.Put(PdfName.Count, new PdfNumber((int)kids[2]));
        AddToBody(top, topRef);
        catalog.Put(PdfName.Outlines, topRef);
    }

    protected virtual PdfIndirectReference Add(PdfIccBased icc)
    {
        PdfIndirectObject objecta;
        objecta = AddToBody(icc);

        return objecta.IndirectReference;
    }

    protected void AddSharedObjectsToBody()
    {
        // add the fonts
        foreach (var details in DocumentFonts.Values)
        {
            details.WriteFont(this);
        }

        // add the form XObjects
        foreach (var objs in FormXObjects.Values)
        {
            var template = (PdfTemplate)objs[1];

            if (template != null && template.IndirectReference is PrIndirectReference)
            {
                continue;
            }

            if (template != null && template.Type == PdfTemplate.TYPE_TEMPLATE)
            {
                AddToBody(template.GetFormXObject(compressionLevel), template.IndirectReference);
            }
        }

        // add all the dependencies in the imported pages
        foreach (var rd in ImportedPages.Values)
        {
            CurrentPdfReaderInstance = rd;
            CurrentPdfReaderInstance.WriteAllPages();
        }

        CurrentPdfReaderInstance = null;

        // add the color
        foreach (var color in DocumentColors.Values)
        {
            AddToBody(color.GetSpotColor(this), color.IndirectReference);
        }

        // add the pattern
        foreach (var pat in DocumentPatterns.Keys)
        {
            AddToBody(pat.GetPattern(compressionLevel), pat.IndirectReference);
        }

        // add the shading patterns
        foreach (var shadingPattern in DocumentShadingPatterns.Keys)
        {
            shadingPattern.AddToBody();
        }

        // add the shadings
        foreach (var shading in DocumentShadings.Keys)
        {
            shading.AddToBody();
        }

        // add the extgstate
        foreach (var entry in DocumentExtGState)
        {
            var gstate = entry.Key;
            var obj = entry.Value;
            AddToBody(gstate, (PdfIndirectReference)obj[1]);
        }

        // add the properties
        foreach (var entry in DocumentProperties)
        {
            var prop = entry.Key;
            var obj = entry.Value;

            if (prop is PdfLayerMembership)
            {
                var layer = (PdfLayerMembership)prop;
                AddToBody(layer.PdfObject, layer.Ref);
            }
            else if (prop is PdfDictionary && !(prop is PdfLayer))
            {
                AddToBody((PdfDictionary)prop, (PdfIndirectReference)obj[1]);
            }
        }

        foreach (var layer in DocumentOcg.Keys)
        {
            AddToBody(layer.PdfObject, layer.Ref);
        }
    }

    protected void FillOcProperties(bool erase)
    {
        if (VOcProperties == null)
        {
            VOcProperties = new PdfOcProperties();
        }

        if (erase)
        {
            VOcProperties.Remove(PdfName.Ocgs);
            VOcProperties.Remove(PdfName.D);
        }

        if (VOcProperties.Get(PdfName.Ocgs) == null)
        {
            var gr = new PdfArray();

            foreach (var layer in DocumentOcg.Keys)
            {
                gr.Add(layer.Ref);
            }

            VOcProperties.Put(PdfName.Ocgs, gr);
        }

        if (VOcProperties.Get(PdfName.D) != null)
        {
            return;
        }

        List<IPdfOcg> docOrder = new(DocumentOcGorder);

        for (var it = new ListIterator<IPdfOcg>(docOrder); it.HasNext();)
        {
            var layer = (PdfLayer)it.Next();

            if (layer.Parent != null)
            {
                it.Remove();
            }
        }

        var order = new PdfArray();

        foreach (PdfLayer layer in docOrder)
        {
            getOcgOrder(order, layer);
        }

        var d = new PdfDictionary();
        VOcProperties.Put(PdfName.D, d);
        d.Put(PdfName.Order, order);
        var grx = new PdfArray();

        foreach (PdfLayer layer in DocumentOcg.Keys)
        {
            if (!layer.On)
            {
                grx.Add(layer.Ref);
            }
        }

        if (grx.Size > 0)
        {
            d.Put(PdfName.OFF, grx);
        }

        if (OcgRadioGroup != null && OcgRadioGroup.Size > 0)
        {
            d.Put(PdfName.Rbgroups, OcgRadioGroup);
        }

        if (OcgLocked != null && OcgLocked.Size > 0)
        {
            d.Put(PdfName.Locked, OcgLocked);
        }

        addAsEvent(PdfName.View, PdfName.Zoom);
        addAsEvent(PdfName.View, PdfName.View);
        addAsEvent(PdfName.Print, PdfName.Print);
        addAsEvent(PdfName.Export, PdfName.Export);
        d.Put(PdfName.Listmode, PdfName.Visiblepages);
    }

    protected virtual PdfDictionary GetCatalog(PdfIndirectReference rootObj)
    {
        PdfDictionary catalog = Pdf.GetCatalog(rootObj);

        // [F12] tagged PDF
        if (Tagged)
        {
            StructureTreeRoot.BuildTree();
            catalog.Put(PdfName.Structtreeroot, structureTreeRoot.Reference);
            var mi = new PdfDictionary();
            mi.Put(PdfName.Marked, PdfBoolean.Pdftrue);

            if (_userProperties)
            {
                mi.Put(PdfName.Userproperties, PdfBoolean.Pdftrue);
            }

            catalog.Put(PdfName.Markinfo, mi);
        }

        // [F13] OCG
        if (DocumentOcg.Count != 0)
        {
            FillOcProperties(false);
            catalog.Put(PdfName.Ocproperties, VOcProperties);
        }

        return catalog;
    }

    private static string getNameString(PdfDictionary dic, PdfName key)
    {
        var obj = PdfReader.GetPdfObject(dic.Get(key));

        if (obj == null || !obj.IsString())
        {
            return null;
        }

        return ((PdfString)obj).ToUnicodeString();
    }

    private static void getOcgOrder(PdfArray order, PdfLayer layer)
    {
        if (!layer.OnPanel)
        {
            return;
        }

        if (layer.Title == null)
        {
            order.Add(layer.Ref);
        }

        var children = layer.Children;

        if (children == null)
        {
            return;
        }

        var kids = new PdfArray();

        if (layer.Title != null)
        {
            kids.Add(new PdfString(layer.Title, PdfObject.TEXT_UNICODE));
        }

        for (var k = 0; k < children.Count; ++k)
        {
            getOcgOrder(kids, children[k]);
        }

        if (kids.Size > 0)
        {
            order.Add(kids);
        }
    }

    private void addAsEvent(PdfName eventa, PdfName category)
    {
        var arr = new PdfArray();

        foreach (PdfLayer layer in DocumentOcg.Keys)
        {
            var usage = layer.GetAsDict(PdfName.Usage);

            if (usage != null && usage.Get(category) != null)
            {
                arr.Add(layer.Ref);
            }
        }

        if (arr.Size == 0)
        {
            return;
        }

        var d = (PdfDictionary)VOcProperties.Get(PdfName.D);
        var arras = (PdfArray)d.Get(PdfName.As);

        if (arras == null)
        {
            arras = new PdfArray();
            d.Put(PdfName.As, arras);
        }

        var asa = new PdfDictionary();
        asa.Put(PdfName.Event, eventa);
        asa.Put(PdfName.Category, new PdfArray(category));
        asa.Put(PdfName.Ocgs, arr);
        arras.Add(asa);
    }

    /// <summary>
    ///     The Catalog is also called the root object of the document.
    ///     Whereas the Cross-Reference maps the objects number with the
    ///     byte offset so that the viewer can find the objects, the
    ///     Catalog tells the viewer the numbers of the objects needed
    ///     to render the document.
    /// </summary>
    /// <summary>
    ///     Root data for the PDF document (used when composing the Catalog)
    /// </summary>
    /// <summary>
    /// </summary>
    /// <returns>an XmpMetadata byte array</returns>
    private byte[] createXmpMetadataBytes()
    {
        var baos = new MemoryStream();

        try
        {
            var xmp = new XmpWriter(baos, Pdf.Info, _pdfxConformance.PdfxConformance);
            xmp.Close();
        }
        catch (IOException)
        {
        }

        return baos.ToArray();
    }

    /// <summary>
    ///     This class generates the structure of a PDF document.
    ///     This class covers the third section of Chapter 5 in the 'Portable Document Format
    ///     Reference Manual version 1.3' (page 55-60). It contains the body of a PDF document
    ///     (section 5.14) and it can also generate a Cross-reference Table (section 5.15).
    ///     @see      PdfWriter
    ///     @see      PdfObject
    ///     @see      PdfIndirectObject
    /// </summary>
    public class PdfBody
    {
        /// <summary>
        ///     inner classes
        /// </summary>
        /// <summary>
        ///     PdfCrossReference  is an entry in the PDF Cross-Reference table.
        /// </summary>
        private const int Objsinstream = 200;

        private readonly PdfWriter _writer;

        /// <summary>
        ///     membervariables
        /// </summary>
        /// <summary>
        ///     array containing the cross-reference table of the normal objects.
        /// </summary>
        private readonly OrderedTree _xrefs;

        private int _currentObjNum;

        private ByteBuffer _index;

        private int _numObj;

        private int _refnum;

        private ByteBuffer _streamObjects;

        /// <summary>
        ///     Constructs a new  PdfBody .
        /// </summary>
        /// <param name="writer"></param>
        internal PdfBody(PdfWriter writer)
        {
            _xrefs = new OrderedTree();
            _xrefs[new PdfCrossReference(0, 0, GENERATION_MAX)] = null;
            Offset = writer.Os.Counter;
            _refnum = 1;
            _writer = writer;
        }

        internal int IndirectReferenceNumber
        {
            get
            {
                var n = _refnum++;
                _xrefs[new PdfCrossReference(n, 0, GENERATION_MAX)] = null;

                return n;
            }
        }

        /// <summary>
        ///     the current byteposition in the body.
        /// </summary>
        internal int Offset { get; private set; }

        internal PdfIndirectReference PdfIndirectReference => new(0, IndirectReferenceNumber);

        internal int Refnum
        {
            set => _refnum = value;
        }

        internal int Size => Math.Max(((PdfCrossReference)_xrefs.GetMaxKey()).Refnum + 1, _refnum);

        internal PdfIndirectObject Add(PdfObject objecta)
            => Add(objecta, IndirectReferenceNumber);

        /// <summary>
        ///     Adds a  PdfObject  to the body.
        ///     This methods creates a  PdfIndirectObject  with a
        ///     certain number, containing the given  PdfObject .
        ///     It also adds a  PdfCrossReference  for this object
        ///     to an  ArrayList  that will be used to build the
        ///     Cross-reference Table.
        ///     @throws IOException
        /// </summary>
        /// <returns>a  PdfIndirectObject </returns>
        internal PdfIndirectObject Add(PdfObject objecta, bool inObjStm)
            => Add(objecta, IndirectReferenceNumber, inObjStm);

        internal PdfIndirectObject Add(PdfObject objecta, PdfIndirectReference refa)
            => Add(objecta, refa.Number);

        /// <summary>
        ///     Gets a PdfIndirectReference for an object that will be created in the future.
        /// </summary>
        /// <returns>a PdfIndirectReference</returns>
        /// <summary>
        ///     Adds a  PdfObject  to the body given an already existing
        ///     PdfIndirectReference.
        ///     This methods creates a  PdfIndirectObject  with the number given by
        ///     ref , containing the given  PdfObject .
        ///     It also adds a  PdfCrossReference  for this object
        ///     to an  ArrayList  that will be used to build the
        ///     Cross-reference Table.
        ///     @throws IOException
        /// </summary>
        /// <returns>a  PdfIndirectObject </returns>
        internal PdfIndirectObject Add(PdfObject objecta, PdfIndirectReference refa, bool inObjStm)
            => Add(objecta, refa.Number, inObjStm);

        internal PdfIndirectObject Add(PdfObject objecta, int refNumber)
            => Add(objecta, refNumber, true); // to false

        internal PdfIndirectObject Add(PdfObject objecta, int refNumber, bool inObjStm)
        {
            if (inObjStm && objecta.CanBeInObjStm() && _writer.FullCompression)
            {
                var pxref = addToObjStm(objecta, refNumber);
                var indirect = new PdfIndirectObject(refNumber, objecta, _writer);
                _xrefs.Remove(pxref);
                _xrefs[pxref] = null;

                return indirect;
            }
            else
            {
                var indirect = new PdfIndirectObject(refNumber, objecta, _writer);
                var pxref = new PdfCrossReference(refNumber, Offset);
                _xrefs.Remove(pxref);
                _xrefs[pxref] = null;
                indirect.WriteTo(_writer.Os);
                Offset = _writer.Os.Counter;

                return indirect;
            }
        }

        internal void FlushObjStm()
        {
            if (_numObj == 0)
            {
                return;
            }

            var first = _index.Size;
            _index.Append(_streamObjects);
            var stream = new PdfStream(_index.ToByteArray());
            stream.FlateCompress(_writer.CompressionLevel);
            stream.Put(PdfName.TYPE, PdfName.Objstm);
            stream.Put(PdfName.N, new PdfNumber(_numObj));
            stream.Put(PdfName.First, new PdfNumber(first));
            Add(stream, _currentObjNum);
            _index = null;
            _streamObjects = null;
            _numObj = 0;
        }

        internal void WriteCrossReferenceTable(Stream os,
            PdfIndirectReference root,
            PdfIndirectReference info,
            PdfIndirectReference encryption,
            PdfObject fileId,
            int prevxref)
        {
            var refNumber = 0;

            if (_writer.FullCompression)
            {
                FlushObjStm();
                refNumber = IndirectReferenceNumber;
                _xrefs[new PdfCrossReference(refNumber, Offset)] = null;
            }

            var first = ((PdfCrossReference)_xrefs.GetMinKey()).Refnum;
            var len = 0;
            var sections = new List<int>();

            foreach (PdfCrossReference entry in _xrefs.Keys)
            {
                if (first + len == entry.Refnum)
                {
                    ++len;
                }
                else
                {
                    sections.Add(first);
                    sections.Add(len);
                    first = entry.Refnum;
                    len = 1;
                }
            }

            sections.Add(first);
            sections.Add(len);

            if (_writer.FullCompression)
            {
                var mid = 4;
                var mask = 0xff000000;

                for (; mid > 1; --mid)
                {
                    if ((mask & Offset) != 0)
                    {
                        break;
                    }

                    mask >>= 8;
                }

                var buf = new ByteBuffer();

                foreach (PdfCrossReference entry in _xrefs.Keys)
                {
                    entry.ToPdf(mid, buf);
                }

                var xr = new PdfStream(buf.ToByteArray());
                buf = null;
                xr.FlateCompress(_writer.CompressionLevel);
                xr.Put(PdfName.Size, new PdfNumber(Size));
                xr.Put(PdfName.Root, root);

                if (info != null)
                {
                    xr.Put(PdfName.Info, info);
                }

                if (encryption != null)
                {
                    xr.Put(PdfName.Encrypt, encryption);
                }

                if (fileId != null)
                {
                    xr.Put(PdfName.Id, fileId);
                }

                xr.Put(PdfName.W, new PdfArray(new[]
                {
                    1, mid, 2
                }));

                xr.Put(PdfName.TYPE, PdfName.Xref);
                var idx = new PdfArray();

                for (var k = 0; k < sections.Count; ++k)
                {
                    idx.Add(new PdfNumber(sections[k]));
                }

                xr.Put(PdfName.Index, idx);

                if (prevxref > 0)
                {
                    xr.Put(PdfName.Prev, new PdfNumber(prevxref));
                }

                var enc = _writer.Crypto;
                _writer.Crypto = null;
                var indirect = new PdfIndirectObject(refNumber, xr, _writer);
                indirect.WriteTo(_writer.Os);
                _writer.Crypto = enc;
            }
            else
            {
                var tmp = GetIsoBytes("xref\n");
                os.Write(tmp, 0, tmp.Length);
                var i = _xrefs.Keys;
                i.MoveNext();

                for (var k = 0; k < sections.Count; k += 2)
                {
                    first = sections[k];
                    len = sections[k + 1];
                    tmp = GetIsoBytes(first.ToString(CultureInfo.InvariantCulture));
                    os.Write(tmp, 0, tmp.Length);
                    os.WriteByte((byte)' ');
                    tmp = GetIsoBytes(len.ToString(CultureInfo.InvariantCulture));
                    os.Write(tmp, 0, tmp.Length);
                    os.WriteByte((byte)'\n');

                    while (len-- > 0)
                    {
                        ((PdfCrossReference)i.Current).ToPdf(os);
                        i.MoveNext();
                    }
                }
            }
        }

        /// <summary>
        ///     constructors
        /// </summary>
        /// <summary>
        ///     methods
        /// </summary>
        private PdfCrossReference addToObjStm(PdfObject obj, int nObj)
        {
            if (_numObj >= Objsinstream)
            {
                FlushObjStm();
            }

            if (_index == null)
            {
                _index = new ByteBuffer();
                _streamObjects = new ByteBuffer();
                _currentObjNum = IndirectReferenceNumber;
                _numObj = 0;
            }

            var p = _streamObjects.Size;
            var idx = _numObj++;
            var enc = _writer.Crypto;
            _writer.Crypto = null;
            obj.ToPdf(_writer, _streamObjects);
            _writer.Crypto = enc;
            _streamObjects.Append(' ');
            _index.Append(nObj).Append(' ').Append(p).Append(' ');

            return new PdfCrossReference(2, nObj, _currentObjNum, idx);
        }

        internal class PdfCrossReference : IComparable
        {
            /// <summary>
            ///     generation of the object.
            /// </summary>
            private readonly int _generation;

            /// <summary>
            ///     Byte offset in the PDF file.
            /// </summary>
            private readonly int _offset;

            /// <summary>
            ///     membervariables
            /// </summary>
            private readonly int _type;

            /// <summary>
            ///     constructors
            /// </summary>
            /// <summary>
            ///     Constructs a cross-reference element for a PdfIndirectObject.
            /// </summary>
            /// <param name="refnum"></param>
            /// <param name="offset">byte offset of the object</param>
            /// <param name="generation">generationnumber of the object</param>
            internal PdfCrossReference(int refnum, int offset, int generation)
            {
                _type = 0;
                _offset = offset;
                Refnum = refnum;
                _generation = generation;
            }

            /// <summary>
            ///     Constructs a cross-reference element for a PdfIndirectObject.
            /// </summary>
            /// <param name="refnum"></param>
            /// <param name="offset">byte offset of the object</param>
            internal PdfCrossReference(int refnum, int offset)
            {
                _type = 1;
                _offset = offset;
                Refnum = refnum;
                _generation = 0;
            }

            internal PdfCrossReference(int type, int refnum, int offset, int generation)
            {
                _type = type;
                _offset = offset;
                Refnum = refnum;
                _generation = generation;
            }

            internal int Refnum { get; }

            public int CompareTo(object o)
            {
                var other = (PdfCrossReference)o;

                return Refnum < other.Refnum ? -1 :
                    Refnum == other.Refnum ? 0 : 1;
            }

            /// <summary>
            ///     @see java.lang.Object#equals(java.lang.Object)
            /// </summary>
            public override bool Equals(object obj)
            {
                if (obj is PdfCrossReference)
                {
                    var other = (PdfCrossReference)obj;

                    return Refnum == other.Refnum;
                }

                return false;
            }

            public override int GetHashCode()
                => Refnum;

            public void ToPdf(Stream os)
            {
                var s1 = _offset.ToString(CultureInfo.InvariantCulture).PadLeft(10, '0');
                var s2 = _generation.ToString(CultureInfo.InvariantCulture).PadLeft(5, '0');
                var buf = new ByteBuffer(40);

                if (_generation == GENERATION_MAX)
                {
                    buf.Append(s1).Append(' ').Append(s2).Append(" f \n");
                }
                else
                {
                    buf.Append(s1).Append(' ').Append(s2).Append(" n \n");
                }

                os.Write(buf.Buffer, 0, buf.Size);
            }

            /// <summary>
            ///     Writes PDF syntax to the Stream
            ///     @throws IOException
            /// </summary>
            /// <param name="midSize"></param>
            /// <param name="os"></param>
            public void ToPdf(int midSize, Stream os)
            {
                os.WriteByte((byte)_type);

                while (--midSize >= 0)
                {
                    os.WriteByte((byte)((_offset >> (8 * midSize)) & 0xff));
                }

                os.WriteByte((byte)((_generation >> 8) & 0xff));
                os.WriteByte((byte)(_generation & 0xff));
            }
        }
    }

    /// <summary>
    ///     PdfTrailer  is the PDF Trailer object.
    ///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
    ///     section 5.16 (page 59-60).
    /// </summary>
    internal class PdfTrailer : PdfDictionary
    {
        /// <summary>
        ///     membervariables
        /// </summary>
        internal readonly int Offset;

        /// <summary>
        ///     constructors
        /// </summary>
        /// <summary>
        ///     Constructs a PDF-Trailer.
        /// </summary>
        /// <param name="size">the number of entries in the  PdfCrossReferenceTable </param>
        /// <param name="offset">offset of the  PdfCrossReferenceTable </param>
        /// <param name="root">an indirect reference to the root of the PDF document</param>
        /// <param name="info">an indirect reference to the info object of the PDF document</param>
        /// <param name="encryption"></param>
        /// <param name="fileId"></param>
        /// <param name="prevxref"></param>
        internal PdfTrailer(int size,
            int offset,
            PdfIndirectReference root,
            PdfIndirectReference info,
            PdfIndirectReference encryption,
            PdfObject fileId,
            int prevxref)
        {
            Offset = offset;
            Put(PdfName.Size, new PdfNumber(size));
            Put(PdfName.Root, root);

            if (info != null)
            {
                Put(PdfName.Info, info);
            }

            if (encryption != null)
            {
                Put(PdfName.Encrypt, encryption);
            }

            if (fileId != null)
            {
                Put(PdfName.Id, fileId);
            }

            if (prevxref > 0)
            {
                Put(PdfName.Prev, new PdfNumber(prevxref));
            }
        }

        /// <summary>
        ///     Returns the PDF representation of this  PdfObject .
        ///     @throws IOException
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="os"></param>
        public override void ToPdf(PdfWriter writer, Stream os)
        {
            var tmp = GetIsoBytes("trailer\n");
            os.Write(tmp, 0, tmp.Length);
            base.ToPdf(null, os);
            tmp = GetIsoBytes("\nstartxref\n");
            os.Write(tmp, 0, tmp.Length);
            tmp = GetIsoBytes(Offset.ToString(CultureInfo.InvariantCulture));
            os.Write(tmp, 0, tmp.Length);
            tmp = GetIsoBytes("\n%%EOF\n");
            os.Write(tmp, 0, tmp.Length);
        }
    }
}