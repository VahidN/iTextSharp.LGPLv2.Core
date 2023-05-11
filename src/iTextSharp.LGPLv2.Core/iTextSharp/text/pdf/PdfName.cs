using System.Reflection;
using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfName  is an object that can be used as a name in a PDF-file.
///     A name, like a string, is a sequence of characters. It must begin with a slash
///     followed by a sequence of ASCII characters in the range 32 through 136 except
///     %, (, ), [, ], &lt;, &gt;, {, }, / and #. Any character except 0x00 may be included
///     in a name by writing its twocharacter hex code, preceded by #. The maximum number
///     of characters in a name is 127.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.5 (page 39-40).
///     @see        PdfObject
///     @see        PdfDictionary
///     @see        BadPdfFormatException
/// </summary>
public class PdfName : PdfObject, IComparable
{
    /// <summary>
    ///     CLASS CONSTANTS (a variety of standard names used in PDF))
    /// </summary>
    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName _3D = new("3D");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName A = new("A");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Aa = new("AA");

    /// <summary>
    ///     A name
    ///     @since 2.1.5 renamed from ABSOLUTECALORIMETRIC
    /// </summary>
    public static readonly PdfName Absolutecolorimetric = new("AbsoluteColorimetric");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ac = new("AC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Acroform = new("AcroForm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Action = new("Action");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Activation = new("Activation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Adbe = new("ADBE");

    /// <summary>
    ///     a name used in PDF structure
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Actualtext = new("ActualText");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbePkcs7Detached = new("adbe.pkcs7.detached");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbePkcs7S4 = new("adbe.pkcs7.s4");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbePkcs7S5 = new("adbe.pkcs7.s5");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbePkcs7Sha1 = new("adbe.pkcs7.sha1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbeX509RsaSha1 = new("adbe.x509.rsa_sha1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdobePpklite = new("Adobe.PPKLite");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdobePpkms = new("Adobe.PPKMS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Aesv2 = new("AESV2");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ais = new("AIS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Allpages = new("AllPages");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Alt = new("Alt");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Alternate = new("Alternate");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Animation = new("Animation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Annot = new("Annot");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Annots = new("Annots");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Antialias = new("AntiAlias");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ap = new("AP");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Appdefault = new("AppDefault");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Art = new("Art");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Artbox = new("ArtBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ascent = new("Ascent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName As = new("AS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ascii85Decode = new("ASCII85Decode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Asciihexdecode = new("ASCIIHexDecode");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Asset = new("Asset");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Assets = new("Assets");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Authevent = new("AuthEvent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Author = new("Author");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName B = new("B");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Background = new("Background");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Baseencoding = new("BaseEncoding");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Basefont = new("BaseFont");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Baseversion = new("BaseVersion");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bbox = new("BBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bc = new("BC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bg = new("BG");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Bibentry = new("BibEntry");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bigfive = new("BigFive");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Binding = new("Binding");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Bindingmaterialname = new("BindingMaterialName");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bitspercomponent = new("BitsPerComponent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bitspersample = new("BitsPerSample");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bl = new("Bl");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Blackis1 = new("BlackIs1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Blackpoint = new("BlackPoint");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Blockquote = new("BlockQuote");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bleedbox = new("BleedBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Blinds = new("Blinds");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bm = new("BM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Border = new("Border");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bounds = new("Bounds");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Box = new("Box");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bs = new("BS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Btn = new("Btn");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Byterange = new("ByteRange");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName C = new("C");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName C0 = new("C0");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName C1 = new("C1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName CA = new("CA");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName CA_ = new("ca");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Calgray = new("CalGray");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Calrgb = new("CalRGB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Capheight = new("CapHeight");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Caption = new("Caption");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Catalog = new("Catalog");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Category = new("Category");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ccittfaxdecode = new("CCITTFaxDecode");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Center = new("Center");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Centerwindow = new("CenterWindow");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cert = new("Cert");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cf = new("CF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cfm = new("CFM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ch = new("Ch");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Charprocs = new("CharProcs");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ci = new("CI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cidfonttype0 = new("CIDFontType0");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cidfonttype2 = new("CIDFontType2");

    /// <summary>
    ///     A name
    ///     @since 2.0.7
    /// </summary>
    public static readonly PdfName Cidset = new("CIDSet");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cidsysteminfo = new("CIDSystemInfo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cidtogidmap = new("CIDToGIDMap");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Circle = new("Circle");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Cmd = new("CMD");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Co = new("CO");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Code = new("Code");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Colors = new("Colors");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Colorspace = new("ColorSpace");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collection = new("Collection");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionfield = new("CollectionField");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionitem = new("CollectionItem");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionschema = new("CollectionSchema");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionsort = new("CollectionSort");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionsubitem = new("CollectionSubitem");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Columns = new("Columns");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Condition = new("Condition");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Configuration = new("Configuration");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Configurations = new("Configurations");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Contactinfo = new("ContactInfo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName CONTENT = new("Content");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Contents = new("Contents");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Coords = new("Coords");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Count = new("Count");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName Courier = new("Courier");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName CourierBold = new("Courier-Bold");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName CourierOblique = new("Courier-Oblique");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName CourierBoldoblique = new("Courier-BoldOblique");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Creationdate = new("CreationDate");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Creator = new("Creator");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Creatorinfo = new("CreatorInfo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cropbox = new("CropBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Crypt = new("Crypt");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cs = new("CS");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Cuepoint = new("CuePoint");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Cuepoints = new("CuePoints");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName D = new("D");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Da = new("DA");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Data = new("Data");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dc = new("DC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dctdecode = new("DCTDecode");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Deactivation = new("Deactivation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Decode = new("Decode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Decodeparms = new("DecodeParms");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Default = new("Default");

    /// <summary>
    ///     A name
    ///     @since	2.1.5 renamed from DEFAULTCRYPTFILER
    /// </summary>
    public static readonly PdfName Defaultcryptfilter = new("DefaultCryptFilter");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Defaultcmyk = new("DefaultCMYK");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Defaultgray = new("DefaultGray");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Defaultrgb = new("DefaultRGB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Desc = new("Desc");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Descendantfonts = new("DescendantFonts");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Descent = new("Descent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dest = new("Dest");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Destoutputprofile = new("DestOutputProfile");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dests = new("Dests");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Devicegray = new("DeviceGray");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Devicergb = new("DeviceRGB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Devicecmyk = new("DeviceCMYK");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Di = new("Di");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Differences = new("Differences");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dissolve = new("Dissolve");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Direction = new("Direction");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Displaydoctitle = new("DisplayDocTitle");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Div = new("Div");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dm = new("Dm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Docmdp = new("DocMDP");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Docopen = new("DocOpen");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AESV3 = new("AESV3");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Document = new("Document");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Domain = new("Domain");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dp = new("DP");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dr = new("DR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ds = new("DS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dur = new("Dur");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Duplex = new("Duplex");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Duplexflipshortedge = new("DuplexFlipShortEdge");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Duplexfliplongedge = new("DuplexFlipLongEdge");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dv = new("DV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dw = new("DW");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName E = new("E");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Earlychange = new("EarlyChange");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName EF = new("EF");

    /// <summary>
    ///     A name
    ///     @since	2.1.3
    /// </summary>
    public static readonly PdfName Eff = new("EFF");

    /// <summary>
    ///     A name
    ///     @since	2.1.3
    /// </summary>
    public static readonly PdfName Efopen = new("EFOpen");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Embedded = new("Embedded");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Embeddedfile = new("EmbeddedFile");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Embeddedfiles = new("EmbeddedFiles");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encode = new("Encode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encodedbytealign = new("EncodedByteAlign");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encoding = new("Encoding");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encrypt = new("Encrypt");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encryptmetadata = new("EncryptMetadata");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Endofblock = new("EndOfBlock");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Endofline = new("EndOfLine");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Extend = new("Extend");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Extensions = new("Extensions");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Extensionlevel = new("ExtensionLevel");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Extgstate = new("ExtGState");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Export = new("Export");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Exportstate = new("ExportState");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Event = new("Event");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName F = new("F");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Far = new("Far");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fb = new("FB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fdecodeparms = new("FDecodeParms");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fdf = new("FDF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ff = new("Ff");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ffilter = new("FFilter");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fields = new("Fields");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Figure = new("Figure");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fileattachment = new("FileAttachment");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Filespec = new("Filespec");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Filter = new("Filter");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName First = new("First");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Firstchar = new("FirstChar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Firstpage = new("FirstPage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fit = new("Fit");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fith = new("FitH");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitv = new("FitV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitr = new("FitR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitb = new("FitB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitbh = new("FitBH");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitbv = new("FitBV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitwindow = new("FitWindow");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Flags = new("Flags");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Flash = new("Flash");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Flashvars = new("FlashVars");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Flatedecode = new("FlateDecode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fo = new("Fo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Font = new("Font");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontbbox = new("FontBBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontdescriptor = new("FontDescriptor");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontfile = new("FontFile");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontfile2 = new("FontFile2");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontfile3 = new("FontFile3");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontmatrix = new("FontMatrix");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontname = new("FontName");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Foreground = new("Foreground");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Form = new("Form");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Formtype = new("FormType");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Formula = new("Formula");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Freetext = new("FreeText");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Frm = new("FRM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fs = new("FS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ft = new("FT");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fullscreen = new("FullScreen");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Function = new("Function");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Functions = new("Functions");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Functiontype = new("FunctionType");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Gamma = new("Gamma");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Gbk = new("GBK");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Glitter = new("Glitter");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Goto = new("GoTo");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Gotoe = new("GoToE");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName OE = new("OE");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName UE = new("UE");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Gotor = new("GoToR");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Group = new("Group");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName GtsPdfa1 = new("GTS_PDFA1");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName GtsPdfx = new("GTS_PDFX");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName GtsPdfxversion = new("GTS_PDFXVersion");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName H = new("H");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H1 = new("H1");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H2 = new("H2");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H3 = new("H3");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H4 = new("H4");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H5 = new("H5");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H6 = new("H6");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Halign = new("HAlign");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Height = new("Height");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Helv = new("Helv");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName Helvetica = new("Helvetica");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName HelveticaBold = new("Helvetica-Bold");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName HelveticaOblique = new("Helvetica-Oblique");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName HelveticaBoldoblique = new("Helvetica-BoldOblique");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hid = new("Hid");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hide = new("Hide");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hidemenubar = new("HideMenubar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hidetoolbar = new("HideToolbar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hidewindowui = new("HideWindowUI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Highlight = new("Highlight");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Hoffset = new("HOffset");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName I = new("I");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Iccbased = new("ICCBased");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Id = new("ID");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Identity = new("Identity");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName If = new("IF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Image = new("Image");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Imageb = new("ImageB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Imagec = new("ImageC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Imagei = new("ImageI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Imagemask = new("ImageMask");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Index = new("Index");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Indexed = new("Indexed");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Info = new("Info");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ink = new("Ink");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Inklist = new("InkList");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Instances = new("Instances");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Importdata = new("ImportData");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Intent = new("Intent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Interpolate = new("Interpolate");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ismap = new("IsMap");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Irt = new("IRT");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Italicangle = new("ItalicAngle");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Itxt = new("ITXT");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ix = new("IX");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Javascript = new("JavaScript");

    /// <summary>
    ///     A name
    ///     @since	2.1.5
    /// </summary>
    public static readonly PdfName Jbig2Decode = new("JBIG2Decode");

    /// <summary>
    ///     A name
    ///     @since	2.1.5
    /// </summary>
    public static readonly PdfName Jbig2Globals = new("JBIG2Globals");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Jpxdecode = new("JPXDecode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Js = new("JS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName K = new("K");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Keywords = new("Keywords");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Kids = new("Kids");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName L = new("L");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName L2R = new("L2R");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lang = new("Lang");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Language = new("Language");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Last = new("Last");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lastchar = new("LastChar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lastpage = new("LastPage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Launch = new("Launch");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Lbl = new("Lbl");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Lbody = new("LBody");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName LENGTH = new("Length");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Length1 = new("Length1");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Li = new("LI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Limits = new("Limits");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Line = new("Line");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Linear = new("Linear");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Link = new("Link");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Listmode = new("ListMode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Location = new("Location");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lock = new("Lock");

    /// <summary>
    ///     A name
    ///     @since	2.1.2
    /// </summary>
    public static readonly PdfName Locked = new("Locked");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lzwdecode = new("LZWDecode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName M = new("M");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Material = new("Material");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Matrix = new("Matrix");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName MacExpertEncoding = new("MacExpertEncoding");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName MacRomanEncoding = new("MacRomanEncoding");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Marked = new("Marked");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Markinfo = new("MarkInfo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mask = new("Mask");

    /// <summary>
    ///     A name
    ///     @since	2.1.6 renamed from MAX
    /// </summary>
    public static readonly PdfName MaxLowerCase = new("max");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName MaxCamelCase = new("Max");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Maxlen = new("MaxLen");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mediabox = new("MediaBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mcid = new("MCID");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mcr = new("MCR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Metadata = new("Metadata");

    /// <summary>
    ///     A name
    ///     @since	2.1.6 renamed from MIN
    /// </summary>
    public static readonly PdfName MinLowerCase = new("min");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName MinCamelCase = new("Min");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mk = new("MK");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mmtype1 = new("MMType1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Moddate = new("ModDate");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N = new("N");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N0 = new("n0");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N1 = new("n1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N2 = new("n2");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N3 = new("n3");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N4 = new("n4");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Name = new("Name");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Named = new("Named");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Names = new("Names");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Navigation = new("Navigation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Navigationpane = new("NavigationPane");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Near = new("Near");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Needappearances = new("NeedAppearances");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Newwindow = new("NewWindow");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Next = new("Next");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Nextpage = new("NextPage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Nm = new("NM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName None = new("None");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Nonfullscreenpagemode = new("NonFullScreenPageMode");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Nonstruct = new("NonStruct");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Note = new("Note");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Numcopies = new("NumCopies");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Nums = new("Nums");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName O = new("O");

    /// <summary>
    ///     A name used with Document Structure
    ///     @since 2.1.5
    /// </summary>
    public static readonly PdfName Obj = new("Obj");

    /// <summary>
    ///     a name used with Doucment Structure
    ///     @since 2.1.5
    /// </summary>
    public static readonly PdfName Objr = new("OBJR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Objstm = new("ObjStm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Oc = new("OC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ocg = new("OCG");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ocgs = new("OCGs");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ocmd = new("OCMD");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ocproperties = new("OCProperties");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Off = new("Off");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName OFF = new("OFF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName On = new("ON");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Onecolumn = new("OneColumn");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Open = new("Open");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Openaction = new("OpenAction");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Op = new("OP");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Op_ = new("op");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Opm = new("OPM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Opt = new("Opt");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Order = new("Order");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ordering = new("Ordering");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Oscillating = new("Oscillating");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outlines = new("Outlines");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outputcondition = new("OutputCondition");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outputconditionidentifier = new("OutputConditionIdentifier");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outputintent = new("OutputIntent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outputintents = new("OutputIntents");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName P = new("P");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Page = new("Page");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pagelabels = new("PageLabels");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pagelayout = new("PageLayout");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pagemode = new("PageMode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pages = new("Pages");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Painttype = new("PaintType");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Panose = new("Panose");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Params = new("Params");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Parent = new("Parent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Parenttree = new("ParentTree");

    /// <summary>
    ///     A name used in defining Document Structure.
    ///     @since 2.1.5
    /// </summary>
    public static readonly PdfName Parenttreenextkey = new("ParentTreeNextKey");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Part = new("Part");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Passcontextclick = new("PassContextClick");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pattern = new("Pattern");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Patterntype = new("PatternType");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Pc = new("PC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pdf = new("PDF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pdfdocencoding = new("PDFDocEncoding");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Perceptual = new("Perceptual");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Perms = new("Perms");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pg = new("Pg");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Pi = new("PI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Picktraybypdfsize = new("PickTrayByPDFSize");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Playcount = new("PlayCount");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Po = new("PO");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Popup = new("Popup");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Position = new("Position");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Predictor = new("Predictor");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Preferred = new("Preferred");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Presentation = new("Presentation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Preserverb = new("PreserveRB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Prev = new("Prev");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Prevpage = new("PrevPage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Print = new("Print");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printarea = new("PrintArea");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printclip = new("PrintClip");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printpagerange = new("PrintPageRange");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printscaling = new("PrintScaling");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printstate = new("PrintState");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Private = new("Private");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Procset = new("ProcSet");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Producer = new("Producer");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Properties = new("Properties");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ps = new("PS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pubsec = new("Adobe.PubSec");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Pv = new("PV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Q = new("Q");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Quadpoints = new("QuadPoints");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Quote = new("Quote");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName R = new("R");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName R2L = new("R2L");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Range = new("Range");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rc = new("RC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rbgroups = new("RBGroups");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Reason = new("Reason");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Recipients = new("Recipients");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rect = new("Rect");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Reference = new("Reference");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Registry = new("Registry");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Registryname = new("RegistryName");

    /// <summary>
    ///     A name
    ///     @since	2.1.5 renamed from RELATIVECALORIMETRIC
    /// </summary>
    public static readonly PdfName Relativecolorimetric = new("RelativeColorimetric");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rendition = new("Rendition");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Resetform = new("ResetForm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Resources = new("Resources");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ri = new("RI");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmedia = new("RichMedia");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaactivation = new("RichMediaActivation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaanimation = new("RichMediaAnimation");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Richmediacommand = new("RichMediaCommand");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaconfiguration = new("RichMediaConfiguration");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediacontent = new("RichMediaContent");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediadeactivation = new("RichMediaDeactivation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaexecute = new("RichMediaExecute");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediainstance = new("RichMediaInstance");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaparams = new("RichMediaParams");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaposition = new("RichMediaPosition");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediapresentation = new("RichMediaPresentation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediasettings = new("RichMediaSettings");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediawindow = new("RichMediaWindow");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rolemap = new("RoleMap");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Root = new("Root");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rotate = new("Rotate");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rows = new("Rows");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Ruby = new("Ruby");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Runlengthdecode = new("RunLengthDecode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rv = new("RV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName S = new("S");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Saturation = new("Saturation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Schema = new("Schema");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Screen = new("Screen");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Scripts = new("Scripts");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sect = new("Sect");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Separation = new("Separation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Setocgstate = new("SetOCGState");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Settings = new("Settings");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Shading = new("Shading");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Shadingtype = new("ShadingType");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName ShiftJis = new("Shift-JIS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sig = new("Sig");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sigflags = new("SigFlags");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sigref = new("SigRef");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Simplex = new("Simplex");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Singlepage = new("SinglePage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Size = new("Size");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Smask = new("SMask");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sort = new("Sort");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Sound = new("Sound");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Span = new("Span");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Speed = new("Speed");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Split = new("Split");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Square = new("Square");

    /// <summary>
    ///     A name
    ///     @since 2.1.3
    /// </summary>
    public static readonly PdfName Squiggly = new("Squiggly");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName St = new("St");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Stamp = new("Stamp");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Standard = new("Standard");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName State = new("State");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Stdcf = new("StdCF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Stemv = new("StemV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Stmf = new("StmF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Strf = new("StrF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Strikeout = new("StrikeOut");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Structparent = new("StructParent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Structparents = new("StructParents");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Structtreeroot = new("StructTreeRoot");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Style = new("Style");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Subfilter = new("SubFilter");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Subject = new("Subject");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Submitform = new("SubmitForm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Subtype = new("Subtype");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Supplement = new("Supplement");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sv = new("SV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sw = new("SW");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName Symbol = new("Symbol");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName T = new("T");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Ta = new("TA");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Table = new("Table");

    /// <summary>
    ///     A name
    ///     @since	2.1.5
    /// </summary>
    public static readonly PdfName Tabs = new("Tabs");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Tbody = new("TBody");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Td = new("TD");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Text = new("Text");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Tfoot = new("TFoot");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Th = new("TH");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Thead = new("THead");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Thumb = new("Thumb");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Threads = new("Threads");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ti = new("TI");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Time = new("Time");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tilingtype = new("TilingType");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName TimesRoman = new("Times-Roman");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName TimesBold = new("Times-Bold");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName TimesItalic = new("Times-Italic");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName TimesBolditalic = new("Times-BoldItalic");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Title = new("Title");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tk = new("TK");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tm = new("TM");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Toc = new("TOC");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Toci = new("TOCI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Toggle = new("Toggle");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Toolbar = new("Toolbar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tounicode = new("ToUnicode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tp = new("TP");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Tablerow = new("TR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Trans = new("Trans");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Transformparams = new("TransformParams");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Transformmethod = new("TransformMethod");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Transparency = new("Transparency");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Transparent = new("Transparent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Trapped = new("Trapped");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Trimbox = new("TrimBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Truetype = new("TrueType");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tu = new("TU");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Twocolumnleft = new("TwoColumnLeft");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Twocolumnright = new("TwoColumnRight");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Twopageleft = new("TwoPageLeft");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Twopageright = new("TwoPageRight");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tx = new("Tx");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName TYPE = new("Type");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName TYPES = new("Types");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Type0 = new("Type0");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Type1 = new("Type1");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Type3 = new("Type3");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName U = new("U");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Uf = new("UF");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Uhc = new("UHC");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Underline = new("Underline");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ur = new("UR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ur3 = new("UR3");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Uri = new("URI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Url = new("URL");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Usage = new("Usage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Useattachments = new("UseAttachments");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Usenone = new("UseNone");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Useoc = new("UseOC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Useoutlines = new("UseOutlines");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName User = new("User");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Userproperties = new("UserProperties");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Userunit = new("UserUnit");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Usethumbs = new("UseThumbs");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName V = new("V");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName V2 = new("V2");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Valign = new("VAlign");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName VerisignPpkvs = new("VeriSign.PPKVS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Version = new("Version");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Video = new("Video");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName View = new("View");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Views = new("Views");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Viewarea = new("ViewArea");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Viewclip = new("ViewClip");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Viewerpreferences = new("ViewerPreferences");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Viewstate = new("ViewState");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Visiblepages = new("VisiblePages");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Voffset = new("VOffset");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName W = new("W");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName W2 = new("W2");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Warichu = new("Warichu");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Wc = new("WC");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Widget = new("Widget");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Width = new("Width");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Widths = new("Widths");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName Win = new("Win");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName WinAnsiEncoding = new("WinAnsiEncoding");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Window = new("Window");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Windowed = new("Windowed");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName Wipe = new("Wipe");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Whitepoint = new("WhitePoint");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Wp = new("WP");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName Ws = new("WS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName X = new("X");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Xa = new("XA");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Xd = new("XD");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xfa = new("XFA");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xml = new("XML");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xobject = new("XObject");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xstep = new("XStep");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xref = new("XRef");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xrefstm = new("XRefStm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xyz = new("XYZ");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ystep = new("YStep");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Zadb = new("ZaDb");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName Zapfdingbats = new("ZapfDingbats");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Zoom = new("Zoom");

    /// <summary>
    ///     map strings to all known static names
    ///     @since 2.1.6
    /// </summary>
    public static INullValueDictionary<string, PdfName> StaticNames;

    /// <summary>
    ///     Use reflection to cache all the static public final names so
    ///     future  PdfName  additions don't have to be "added twice".
    ///     A bit less efficient (around 50ms spent here on a 2.2ghz machine),
    ///     but Much Less error prone.
    ///     @since 2.1.6
    /// </summary>
    static PdfName()
    {
        var fields = typeof(PdfName).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
        StaticNames = new NullValueDictionary<string, PdfName>(fields.Length);
        try
        {
            for (var fldIdx = 0; fldIdx < fields.Length; ++fldIdx)
            {
                var curFld = fields[fldIdx];
                if (curFld.FieldType == typeof(PdfName))
                {
                    var name = (PdfName)curFld.GetValue(null);
                    StaticNames[DecodeName(name.ToString())] = name;
                }
            }
        }
        catch
        {
        }
    }

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a new  PdfName . The name length will be checked.
    /// </summary>
    /// <param name="name">the new name</param>
    public PdfName(string name) : this(name, true)
    {
    }

    /// <summary>
    ///     Constructs a new  PdfName .
    ///     have any length
    /// </summary>
    /// <param name="name">the new name</param>
    /// <param name="lengthCheck">if  true  check the lenght validity, if  false  the name can</param>
    public PdfName(string name, bool lengthCheck) : base(NAME)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        // The minimum number of characters in a name is 0, the maximum is 127 (the '/' not included)
        var length = name.Length;
        if (lengthCheck && length > 127)
        {
            throw new ArgumentException("The name '" + name + "' is too long (" + length + " characters).");
        }

        Bytes = EncodeName(name);
    }

    public PdfName(byte[] bytes) : base(NAME, bytes)
    {
    }

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Compares this object with the specified object for order.  Returns a
    ///     negative int, zero, or a positive int as this object is less
    ///     than, equal to, or greater than the specified object.
    ///     is less than, equal to, or greater than the specified object.
    ///     @throws Exception if the specified object's type prevents it
    ///     from being compared to this Object.
    /// </summary>
    /// <param name="obj">the Object to be compared.</param>
    /// <returns>a negative int, zero, or a positive int as this object</returns>
    public int CompareTo(object obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        var name = (PdfName)obj;

        var myBytes = Bytes;
        var objBytes = name.Bytes;
        var len = Math.Min(myBytes.Length, objBytes.Length);
        for (var i = 0; i < len; i++)
        {
            if (myBytes[i] > objBytes[i])
            {
                return 1;
            }

            if (myBytes[i] < objBytes[i])
            {
                return -1;
            }
        }

        if (myBytes.Length < objBytes.Length)
        {
            return -1;
        }

        if (myBytes.Length > objBytes.Length)
        {
            return 1;
        }

        return 0;
    }

    /// <summary>
    ///     Indicates whether some other object is "equal to" this one.
    ///     argument;  false  otherwise.
    /// </summary>
    /// <param name="obj">the reference object with which to compare.</param>
    /// <returns> true  if this object is the same as the obj</returns>
    public override bool Equals(object obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (obj is PdfName)
        {
            return CompareTo(obj) == 0;
        }

        return false;
    }

    /// <summary>
    ///     Returns a hash code value for the object. This method is
    ///     supported for the benefit of hashtables such as those provided by
    ///     java.util.Hashtable .
    /// </summary>
    /// <returns>a hash code value for this object.</returns>
    public override int GetHashCode()
    {
        var ptr = 0;
        var len = Bytes.Length;
        var h = 0;
        for (var i = 0; i < len; i++)
        {
            h = 31 * h + (Bytes[ptr++] & 0xff);
        }

        return h;
    }

    /// <summary>
    ///     Encodes a plain name given in the unescaped form "AB CD" into "/AB#20CD".
    ///     @since	2.1.5
    /// </summary>
    /// <param name="name">the name to encode</param>
    /// <returns>the encoded name</returns>
    public static byte[] EncodeName(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var length = name.Length;
        // every special character has to be substituted
        var pdfName = new ByteBuffer(length + 20);
        pdfName.Append('/');
        var chars = name.ToCharArray();
        char character;
        // loop over all the characters
        foreach (var cc in chars)
        {
            character = (char)(cc & 0xff);
            // special characters are escaped (reference manual p.39)
            switch (character)
            {
                case ' ':
                case '%':
                case '(':
                case ')':
                case '<':
                case '>':
                case '[':
                case ']':
                case '{':
                case '}':
                case '/':
                case '#':
                    pdfName.Append('#');
                    pdfName.Append(Convert.ToString(character, 16));
                    break;
                default:
                    if (character > 126 || character < 32)
                    {
                        pdfName.Append('#');
                        if (character < 16)
                        {
                            pdfName.Append('0');
                        }

                        pdfName.Append(Convert.ToString(character, 16));
                    }
                    else
                    {
                        pdfName.Append(character);
                    }

                    break;
            }
        }

        return pdfName.ToByteArray();
    }

    /// <summary>
    ///     Decodes an escaped name in the form "/AB#20CD" into "AB CD".
    /// </summary>
    /// <param name="name">the name to decode</param>
    /// <returns>the decoded name</returns>
    public static string DecodeName(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var buf = new StringBuilder();
        var len = name.Length;
        for (var k = 1; k < len; ++k)
        {
            var c = name[k];
            if (c == '#')
            {
                c = (char)((PrTokeniser.GetHex(name[k + 1]) << 4) + PrTokeniser.GetHex(name[k + 2]));
                k += 2;
            }

            buf.Append(c);
        }

        return buf.ToString();
    }
}