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
    public static readonly PdfName _3D = new(name: "3D");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName A = new(name: "A");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Aa = new(name: "AA");

    /// <summary>
    ///     A name
    ///     @since 2.1.5 renamed from ABSOLUTECALORIMETRIC
    /// </summary>
    public static readonly PdfName Absolutecolorimetric = new(name: "AbsoluteColorimetric");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ac = new(name: "AC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Acroform = new(name: "AcroForm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Action = new(name: "Action");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Activation = new(name: "Activation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Adbe = new(name: "ADBE");

    /// <summary>
    ///     a name used in PDF structure
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Actualtext = new(name: "ActualText");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbePkcs7Detached = new(name: "adbe.pkcs7.detached");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbePkcs7S4 = new(name: "adbe.pkcs7.s4");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbePkcs7S5 = new(name: "adbe.pkcs7.s5");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbePkcs7Sha1 = new(name: "adbe.pkcs7.sha1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdbeX509RsaSha1 = new(name: "adbe.x509.rsa_sha1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdobePpklite = new(name: "Adobe.PPKLite");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AdobePpkms = new(name: "Adobe.PPKMS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Aesv2 = new(name: "AESV2");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ais = new(name: "AIS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Allpages = new(name: "AllPages");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Alt = new(name: "Alt");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Alternate = new(name: "Alternate");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Animation = new(name: "Animation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Annot = new(name: "Annot");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Annots = new(name: "Annots");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Antialias = new(name: "AntiAlias");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ap = new(name: "AP");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Appdefault = new(name: "AppDefault");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Art = new(name: "Art");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Artbox = new(name: "ArtBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ascent = new(name: "Ascent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName As = new(name: "AS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ascii85Decode = new(name: "ASCII85Decode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Asciihexdecode = new(name: "ASCIIHexDecode");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Asset = new(name: "Asset");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Assets = new(name: "Assets");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Authevent = new(name: "AuthEvent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Author = new(name: "Author");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName B = new(name: "B");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Background = new(name: "Background");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Baseencoding = new(name: "BaseEncoding");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Basefont = new(name: "BaseFont");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Baseversion = new(name: "BaseVersion");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bbox = new(name: "BBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bc = new(name: "BC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bg = new(name: "BG");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Bibentry = new(name: "BibEntry");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bigfive = new(name: "BigFive");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Binding = new(name: "Binding");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Bindingmaterialname = new(name: "BindingMaterialName");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bitspercomponent = new(name: "BitsPerComponent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bitspersample = new(name: "BitsPerSample");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bl = new(name: "Bl");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Blackis1 = new(name: "BlackIs1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Blackpoint = new(name: "BlackPoint");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Blockquote = new(name: "BlockQuote");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bleedbox = new(name: "BleedBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Blinds = new(name: "Blinds");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bm = new(name: "BM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Border = new(name: "Border");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bounds = new(name: "Bounds");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Box = new(name: "Box");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Bs = new(name: "BS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Btn = new(name: "Btn");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Byterange = new(name: "ByteRange");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName C = new(name: "C");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName C0 = new(name: "C0");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName C1 = new(name: "C1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName CA = new(name: "CA");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName CA_ = new(name: "ca");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Calgray = new(name: "CalGray");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Calrgb = new(name: "CalRGB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Capheight = new(name: "CapHeight");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Caption = new(name: "Caption");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Catalog = new(name: "Catalog");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Category = new(name: "Category");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ccittfaxdecode = new(name: "CCITTFaxDecode");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Center = new(name: "Center");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Centerwindow = new(name: "CenterWindow");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cert = new(name: "Cert");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cf = new(name: "CF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cfm = new(name: "CFM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ch = new(name: "Ch");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Charprocs = new(name: "CharProcs");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ci = new(name: "CI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cidfonttype0 = new(name: "CIDFontType0");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cidfonttype2 = new(name: "CIDFontType2");

    /// <summary>
    ///     A name
    ///     @since 2.0.7
    /// </summary>
    public static readonly PdfName Cidset = new(name: "CIDSet");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cidsysteminfo = new(name: "CIDSystemInfo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cidtogidmap = new(name: "CIDToGIDMap");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Circle = new(name: "Circle");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Cmd = new(name: "CMD");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Co = new(name: "CO");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Code = new(name: "Code");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Colors = new(name: "Colors");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Colorspace = new(name: "ColorSpace");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collection = new(name: "Collection");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionfield = new(name: "CollectionField");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionitem = new(name: "CollectionItem");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionschema = new(name: "CollectionSchema");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionsort = new(name: "CollectionSort");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Collectionsubitem = new(name: "CollectionSubitem");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Columns = new(name: "Columns");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Condition = new(name: "Condition");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Configuration = new(name: "Configuration");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Configurations = new(name: "Configurations");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Contactinfo = new(name: "ContactInfo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName CONTENT = new(name: "Content");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Contents = new(name: "Contents");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Coords = new(name: "Coords");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Count = new(name: "Count");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName Courier = new(name: "Courier");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName CourierBold = new(name: "Courier-Bold");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName CourierOblique = new(name: "Courier-Oblique");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName CourierBoldoblique = new(name: "Courier-BoldOblique");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Creationdate = new(name: "CreationDate");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Creator = new(name: "Creator");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Creatorinfo = new(name: "CreatorInfo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cropbox = new(name: "CropBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Crypt = new(name: "Crypt");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Cs = new(name: "CS");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Cuepoint = new(name: "CuePoint");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Cuepoints = new(name: "CuePoints");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName D = new(name: "D");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Da = new(name: "DA");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Data = new(name: "Data");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dc = new(name: "DC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dctdecode = new(name: "DCTDecode");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Deactivation = new(name: "Deactivation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Decode = new(name: "Decode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Decodeparms = new(name: "DecodeParms");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Default = new(name: "Default");

    /// <summary>
    ///     A name
    ///     @since	2.1.5 renamed from DEFAULTCRYPTFILER
    /// </summary>
    public static readonly PdfName Defaultcryptfilter = new(name: "DefaultCryptFilter");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Defaultcmyk = new(name: "DefaultCMYK");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Defaultgray = new(name: "DefaultGray");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Defaultrgb = new(name: "DefaultRGB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Desc = new(name: "Desc");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Descendantfonts = new(name: "DescendantFonts");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Descent = new(name: "Descent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dest = new(name: "Dest");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Destoutputprofile = new(name: "DestOutputProfile");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dests = new(name: "Dests");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Devicegray = new(name: "DeviceGray");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Devicergb = new(name: "DeviceRGB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Devicecmyk = new(name: "DeviceCMYK");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Di = new(name: "Di");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Differences = new(name: "Differences");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dissolve = new(name: "Dissolve");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Direction = new(name: "Direction");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Displaydoctitle = new(name: "DisplayDocTitle");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Div = new(name: "Div");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dm = new(name: "Dm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Docmdp = new(name: "DocMDP");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Docopen = new(name: "DocOpen");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName AESV3 = new(name: "AESV3");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Document = new(name: "Document");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Domain = new(name: "Domain");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dp = new(name: "DP");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dr = new(name: "DR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ds = new(name: "DS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dur = new(name: "Dur");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Duplex = new(name: "Duplex");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Duplexflipshortedge = new(name: "DuplexFlipShortEdge");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Duplexfliplongedge = new(name: "DuplexFlipLongEdge");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dv = new(name: "DV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Dw = new(name: "DW");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName E = new(name: "E");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Earlychange = new(name: "EarlyChange");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName EF = new(name: "EF");

    /// <summary>
    ///     A name
    ///     @since	2.1.3
    /// </summary>
    public static readonly PdfName Eff = new(name: "EFF");

    /// <summary>
    ///     A name
    ///     @since	2.1.3
    /// </summary>
    public static readonly PdfName Efopen = new(name: "EFOpen");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Embedded = new(name: "Embedded");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Embeddedfile = new(name: "EmbeddedFile");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Embeddedfiles = new(name: "EmbeddedFiles");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encode = new(name: "Encode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encodedbytealign = new(name: "EncodedByteAlign");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encoding = new(name: "Encoding");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encrypt = new(name: "Encrypt");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Encryptmetadata = new(name: "EncryptMetadata");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Endofblock = new(name: "EndOfBlock");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Endofline = new(name: "EndOfLine");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Extend = new(name: "Extend");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Extensions = new(name: "Extensions");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Extensionlevel = new(name: "ExtensionLevel");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Extgstate = new(name: "ExtGState");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Export = new(name: "Export");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Exportstate = new(name: "ExportState");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Event = new(name: "Event");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName F = new(name: "F");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Far = new(name: "Far");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fb = new(name: "FB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fdecodeparms = new(name: "FDecodeParms");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fdf = new(name: "FDF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ff = new(name: "Ff");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ffilter = new(name: "FFilter");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fields = new(name: "Fields");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Figure = new(name: "Figure");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fileattachment = new(name: "FileAttachment");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Filespec = new(name: "Filespec");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Filter = new(name: "Filter");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName First = new(name: "First");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Firstchar = new(name: "FirstChar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Firstpage = new(name: "FirstPage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fit = new(name: "Fit");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fith = new(name: "FitH");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitv = new(name: "FitV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitr = new(name: "FitR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitb = new(name: "FitB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitbh = new(name: "FitBH");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitbv = new(name: "FitBV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fitwindow = new(name: "FitWindow");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Flags = new(name: "Flags");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Flash = new(name: "Flash");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Flashvars = new(name: "FlashVars");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Flatedecode = new(name: "FlateDecode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fo = new(name: "Fo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Font = new(name: "Font");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontbbox = new(name: "FontBBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontdescriptor = new(name: "FontDescriptor");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontfile = new(name: "FontFile");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontfile2 = new(name: "FontFile2");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontfile3 = new(name: "FontFile3");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontmatrix = new(name: "FontMatrix");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fontname = new(name: "FontName");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Foreground = new(name: "Foreground");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Form = new(name: "Form");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Formtype = new(name: "FormType");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Formula = new(name: "Formula");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Freetext = new(name: "FreeText");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Frm = new(name: "FRM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fs = new(name: "FS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ft = new(name: "FT");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Fullscreen = new(name: "FullScreen");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Function = new(name: "Function");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Functions = new(name: "Functions");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Functiontype = new(name: "FunctionType");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Gamma = new(name: "Gamma");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Gbk = new(name: "GBK");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Glitter = new(name: "Glitter");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Goto = new(name: "GoTo");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Gotoe = new(name: "GoToE");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName OE = new(name: "OE");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName UE = new(name: "UE");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Gotor = new(name: "GoToR");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Group = new(name: "Group");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName GtsPdfa1 = new(name: "GTS_PDFA1");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName GtsPdfx = new(name: "GTS_PDFX");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName GtsPdfxversion = new(name: "GTS_PDFXVersion");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName H = new(name: "H");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H1 = new(name: "H1");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H2 = new(name: "H2");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H3 = new(name: "H3");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H4 = new(name: "H4");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H5 = new(name: "H5");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName H6 = new(name: "H6");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Halign = new(name: "HAlign");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Height = new(name: "Height");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Helv = new(name: "Helv");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName Helvetica = new(name: "Helvetica");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName HelveticaBold = new(name: "Helvetica-Bold");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName HelveticaOblique = new(name: "Helvetica-Oblique");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName HelveticaBoldoblique = new(name: "Helvetica-BoldOblique");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hid = new(name: "Hid");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hide = new(name: "Hide");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hidemenubar = new(name: "HideMenubar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hidetoolbar = new(name: "HideToolbar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Hidewindowui = new(name: "HideWindowUI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Highlight = new(name: "Highlight");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Hoffset = new(name: "HOffset");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName I = new(name: "I");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Iccbased = new(name: "ICCBased");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Id = new(name: "ID");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Identity = new(name: "Identity");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName If = new(name: "IF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Image = new(name: "Image");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Imageb = new(name: "ImageB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Imagec = new(name: "ImageC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Imagei = new(name: "ImageI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Imagemask = new(name: "ImageMask");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Index = new(name: "Index");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Indexed = new(name: "Indexed");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Info = new(name: "Info");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ink = new(name: "Ink");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Inklist = new(name: "InkList");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Instances = new(name: "Instances");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Importdata = new(name: "ImportData");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Intent = new(name: "Intent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Interpolate = new(name: "Interpolate");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ismap = new(name: "IsMap");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Irt = new(name: "IRT");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Italicangle = new(name: "ItalicAngle");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Itxt = new(name: "ITXT");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ix = new(name: "IX");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Javascript = new(name: "JavaScript");

    /// <summary>
    ///     A name
    ///     @since	2.1.5
    /// </summary>
    public static readonly PdfName Jbig2Decode = new(name: "JBIG2Decode");

    /// <summary>
    ///     A name
    ///     @since	2.1.5
    /// </summary>
    public static readonly PdfName Jbig2Globals = new(name: "JBIG2Globals");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Jpxdecode = new(name: "JPXDecode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Js = new(name: "JS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName K = new(name: "K");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Keywords = new(name: "Keywords");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Kids = new(name: "Kids");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName L = new(name: "L");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName L2R = new(name: "L2R");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lang = new(name: "Lang");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Language = new(name: "Language");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Last = new(name: "Last");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lastchar = new(name: "LastChar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lastpage = new(name: "LastPage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Launch = new(name: "Launch");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Lbl = new(name: "Lbl");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Lbody = new(name: "LBody");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName LENGTH = new(name: "Length");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Length1 = new(name: "Length1");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Li = new(name: "LI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Limits = new(name: "Limits");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Line = new(name: "Line");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Linear = new(name: "Linear");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Link = new(name: "Link");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Listmode = new(name: "ListMode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Location = new(name: "Location");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lock = new(name: "Lock");

    /// <summary>
    ///     A name
    ///     @since	2.1.2
    /// </summary>
    public static readonly PdfName Locked = new(name: "Locked");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Lzwdecode = new(name: "LZWDecode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName M = new(name: "M");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Material = new(name: "Material");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Matrix = new(name: "Matrix");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName MacExpertEncoding = new(name: "MacExpertEncoding");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName MacRomanEncoding = new(name: "MacRomanEncoding");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Marked = new(name: "Marked");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Markinfo = new(name: "MarkInfo");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mask = new(name: "Mask");

    /// <summary>
    ///     A name
    ///     @since	2.1.6 renamed from MAX
    /// </summary>
    public static readonly PdfName MaxLowerCase = new(name: "max");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName MaxCamelCase = new(name: "Max");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Maxlen = new(name: "MaxLen");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mediabox = new(name: "MediaBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mcid = new(name: "MCID");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mcr = new(name: "MCR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Metadata = new(name: "Metadata");

    /// <summary>
    ///     A name
    ///     @since	2.1.6 renamed from MIN
    /// </summary>
    public static readonly PdfName MinLowerCase = new(name: "min");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName MinCamelCase = new(name: "Min");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mk = new(name: "MK");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Mmtype1 = new(name: "MMType1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Moddate = new(name: "ModDate");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N = new(name: "N");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N0 = new(name: "n0");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N1 = new(name: "n1");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N2 = new(name: "n2");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N3 = new(name: "n3");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName N4 = new(name: "n4");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Name = new(name: "Name");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Named = new(name: "Named");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Names = new(name: "Names");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Navigation = new(name: "Navigation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Navigationpane = new(name: "NavigationPane");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Near = new(name: "Near");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Needappearances = new(name: "NeedAppearances");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Newwindow = new(name: "NewWindow");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Next = new(name: "Next");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Nextpage = new(name: "NextPage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Nm = new(name: "NM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName None = new(name: "None");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Nonfullscreenpagemode = new(name: "NonFullScreenPageMode");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Nonstruct = new(name: "NonStruct");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Note = new(name: "Note");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Numcopies = new(name: "NumCopies");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Nums = new(name: "Nums");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName O = new(name: "O");

    /// <summary>
    ///     A name used with Document Structure
    ///     @since 2.1.5
    /// </summary>
    public static readonly PdfName Obj = new(name: "Obj");

    /// <summary>
    ///     a name used with Doucment Structure
    ///     @since 2.1.5
    /// </summary>
    public static readonly PdfName Objr = new(name: "OBJR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Objstm = new(name: "ObjStm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Oc = new(name: "OC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ocg = new(name: "OCG");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ocgs = new(name: "OCGs");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ocmd = new(name: "OCMD");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ocproperties = new(name: "OCProperties");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Off = new(name: "Off");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName OFF = new(name: "OFF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName On = new(name: "ON");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Onecolumn = new(name: "OneColumn");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Open = new(name: "Open");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Openaction = new(name: "OpenAction");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Op = new(name: "OP");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Op_ = new(name: "op");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Opm = new(name: "OPM");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Opt = new(name: "Opt");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Order = new(name: "Order");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ordering = new(name: "Ordering");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Oscillating = new(name: "Oscillating");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outlines = new(name: "Outlines");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outputcondition = new(name: "OutputCondition");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outputconditionidentifier = new(name: "OutputConditionIdentifier");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outputintent = new(name: "OutputIntent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Outputintents = new(name: "OutputIntents");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName P = new(name: "P");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Page = new(name: "Page");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pagelabels = new(name: "PageLabels");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pagelayout = new(name: "PageLayout");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pagemode = new(name: "PageMode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pages = new(name: "Pages");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Painttype = new(name: "PaintType");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Panose = new(name: "Panose");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Params = new(name: "Params");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Parent = new(name: "Parent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Parenttree = new(name: "ParentTree");

    /// <summary>
    ///     A name used in defining Document Structure.
    ///     @since 2.1.5
    /// </summary>
    public static readonly PdfName Parenttreenextkey = new(name: "ParentTreeNextKey");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Part = new(name: "Part");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Passcontextclick = new(name: "PassContextClick");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pattern = new(name: "Pattern");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Patterntype = new(name: "PatternType");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Pc = new(name: "PC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pdf = new(name: "PDF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pdfdocencoding = new(name: "PDFDocEncoding");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Perceptual = new(name: "Perceptual");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Perms = new(name: "Perms");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pg = new(name: "Pg");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Pi = new(name: "PI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Picktraybypdfsize = new(name: "PickTrayByPDFSize");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Playcount = new(name: "PlayCount");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Po = new(name: "PO");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Popup = new(name: "Popup");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Position = new(name: "Position");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Predictor = new(name: "Predictor");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Preferred = new(name: "Preferred");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Presentation = new(name: "Presentation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Preserverb = new(name: "PreserveRB");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Prev = new(name: "Prev");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Prevpage = new(name: "PrevPage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Print = new(name: "Print");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printarea = new(name: "PrintArea");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printclip = new(name: "PrintClip");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printpagerange = new(name: "PrintPageRange");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printscaling = new(name: "PrintScaling");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Printstate = new(name: "PrintState");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Private = new(name: "Private");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Procset = new(name: "ProcSet");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Producer = new(name: "Producer");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Properties = new(name: "Properties");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ps = new(name: "PS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Pubsec = new(name: "Adobe.PubSec");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Pv = new(name: "PV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Q = new(name: "Q");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Quadpoints = new(name: "QuadPoints");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Quote = new(name: "Quote");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName R = new(name: "R");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName R2L = new(name: "R2L");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Range = new(name: "Range");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rc = new(name: "RC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rbgroups = new(name: "RBGroups");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Reason = new(name: "Reason");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Recipients = new(name: "Recipients");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rect = new(name: "Rect");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Reference = new(name: "Reference");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Registry = new(name: "Registry");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Registryname = new(name: "RegistryName");

    /// <summary>
    ///     A name
    ///     @since	2.1.5 renamed from RELATIVECALORIMETRIC
    /// </summary>
    public static readonly PdfName Relativecolorimetric = new(name: "RelativeColorimetric");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rendition = new(name: "Rendition");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Resetform = new(name: "ResetForm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Resources = new(name: "Resources");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ri = new(name: "RI");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmedia = new(name: "RichMedia");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaactivation = new(name: "RichMediaActivation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaanimation = new(name: "RichMediaAnimation");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Richmediacommand = new(name: "RichMediaCommand");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaconfiguration = new(name: "RichMediaConfiguration");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediacontent = new(name: "RichMediaContent");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediadeactivation = new(name: "RichMediaDeactivation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaexecute = new(name: "RichMediaExecute");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediainstance = new(name: "RichMediaInstance");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaparams = new(name: "RichMediaParams");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediaposition = new(name: "RichMediaPosition");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediapresentation = new(name: "RichMediaPresentation");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediasettings = new(name: "RichMediaSettings");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Richmediawindow = new(name: "RichMediaWindow");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rolemap = new(name: "RoleMap");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Root = new(name: "Root");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rotate = new(name: "Rotate");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rows = new(name: "Rows");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Ruby = new(name: "Ruby");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Runlengthdecode = new(name: "RunLengthDecode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Rv = new(name: "RV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName S = new(name: "S");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Saturation = new(name: "Saturation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Schema = new(name: "Schema");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Screen = new(name: "Screen");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Scripts = new(name: "Scripts");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sect = new(name: "Sect");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Separation = new(name: "Separation");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Setocgstate = new(name: "SetOCGState");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Settings = new(name: "Settings");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Shading = new(name: "Shading");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Shadingtype = new(name: "ShadingType");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName ShiftJis = new(name: "Shift-JIS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sig = new(name: "Sig");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sigflags = new(name: "SigFlags");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sigref = new(name: "SigRef");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Simplex = new(name: "Simplex");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Singlepage = new(name: "SinglePage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Size = new(name: "Size");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Smask = new(name: "SMask");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sort = new(name: "Sort");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Sound = new(name: "Sound");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Span = new(name: "Span");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Speed = new(name: "Speed");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Split = new(name: "Split");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Square = new(name: "Square");

    /// <summary>
    ///     A name
    ///     @since 2.1.3
    /// </summary>
    public static readonly PdfName Squiggly = new(name: "Squiggly");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName St = new(name: "St");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Stamp = new(name: "Stamp");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Standard = new(name: "Standard");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName State = new(name: "State");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Stdcf = new(name: "StdCF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Stemv = new(name: "StemV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Stmf = new(name: "StmF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Strf = new(name: "StrF");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Strikeout = new(name: "StrikeOut");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Structparent = new(name: "StructParent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Structparents = new(name: "StructParents");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Structtreeroot = new(name: "StructTreeRoot");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Style = new(name: "Style");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Subfilter = new(name: "SubFilter");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Subject = new(name: "Subject");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Submitform = new(name: "SubmitForm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Subtype = new(name: "Subtype");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Supplement = new(name: "Supplement");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sv = new(name: "SV");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Sw = new(name: "SW");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName Symbol = new(name: "Symbol");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName T = new(name: "T");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Ta = new(name: "TA");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Table = new(name: "Table");

    /// <summary>
    ///     A name
    ///     @since	2.1.5
    /// </summary>
    public static readonly PdfName Tabs = new(name: "Tabs");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Tbody = new(name: "TBody");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Td = new(name: "TD");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Text = new(name: "Text");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Tfoot = new(name: "TFoot");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Th = new(name: "TH");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Thead = new(name: "THead");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Thumb = new(name: "Thumb");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Threads = new(name: "Threads");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ti = new(name: "TI");

    /// <summary>
    ///     A name
    ///     @since	2.1.6
    /// </summary>
    public static readonly PdfName Time = new(name: "Time");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tilingtype = new(name: "TilingType");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName TimesRoman = new(name: "Times-Roman");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName TimesBold = new(name: "Times-Bold");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName TimesItalic = new(name: "Times-Italic");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName TimesBolditalic = new(name: "Times-BoldItalic");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Title = new(name: "Title");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tk = new(name: "TK");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tm = new(name: "TM");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Toc = new(name: "TOC");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Toci = new(name: "TOCI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Toggle = new(name: "Toggle");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Toolbar = new(name: "Toolbar");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tounicode = new(name: "ToUnicode");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tp = new(name: "TP");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Tablerow = new(name: "TR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Trans = new(name: "Trans");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Transformparams = new(name: "TransformParams");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Transformmethod = new(name: "TransformMethod");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Transparency = new(name: "Transparency");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Transparent = new(name: "Transparent");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Trapped = new(name: "Trapped");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Trimbox = new(name: "TrimBox");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Truetype = new(name: "TrueType");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tu = new(name: "TU");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Twocolumnleft = new(name: "TwoColumnLeft");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Twocolumnright = new(name: "TwoColumnRight");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Twopageleft = new(name: "TwoPageLeft");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Twopageright = new(name: "TwoPageRight");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Tx = new(name: "Tx");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName TYPE = new(name: "Type");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName TYPES = new(name: "Types");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Type0 = new(name: "Type0");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Type1 = new(name: "Type1");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Type3 = new(name: "Type3");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName U = new(name: "U");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Uf = new(name: "UF");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Uhc = new(name: "UHC");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Underline = new(name: "Underline");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ur = new(name: "UR");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ur3 = new(name: "UR3");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Uri = new(name: "URI");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Url = new(name: "URL");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Usage = new(name: "Usage");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Useattachments = new(name: "UseAttachments");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Usenone = new(name: "UseNone");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Useoc = new(name: "UseOC");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Useoutlines = new(name: "UseOutlines");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName User = new(name: "User");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Userproperties = new(name: "UserProperties");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Userunit = new(name: "UserUnit");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Usethumbs = new(name: "UseThumbs");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName V = new(name: "V");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName V2 = new(name: "V2");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Valign = new(name: "VAlign");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName VerisignPpkvs = new(name: "VeriSign.PPKVS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Version = new(name: "Version");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Video = new(name: "Video");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName View = new(name: "View");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Views = new(name: "Views");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Viewarea = new(name: "ViewArea");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Viewclip = new(name: "ViewClip");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Viewerpreferences = new(name: "ViewerPreferences");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Viewstate = new(name: "ViewState");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Visiblepages = new(name: "VisiblePages");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Voffset = new(name: "VOffset");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName W = new(name: "W");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName W2 = new(name: "W2");

    /// <summary>
    ///     A name
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Warichu = new(name: "Warichu");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Wc = new(name: "WC");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Widget = new(name: "Widget");

    /// <summary>
    ///     A name of an attribute.
    /// </summary>
    public static readonly PdfName Width = new(name: "Width");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Widths = new(name: "Widths");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName Win = new(name: "Win");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName WinAnsiEncoding = new(name: "WinAnsiEncoding");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Window = new(name: "Window");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Windowed = new(name: "Windowed");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName Wipe = new(name: "Wipe");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Whitepoint = new(name: "WhitePoint");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Wp = new(name: "WP");

    /// <summary>
    ///     A name of an encoding
    /// </summary>
    public static readonly PdfName Ws = new(name: "WS");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName X = new(name: "X");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Xa = new(name: "XA");

    /// <summary>
    ///     A name.
    ///     @since 2.1.6
    /// </summary>
    public static readonly PdfName Xd = new(name: "XD");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xfa = new(name: "XFA");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xml = new(name: "XML");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xobject = new(name: "XObject");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xstep = new(name: "XStep");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xref = new(name: "XRef");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xrefstm = new(name: "XRefStm");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Xyz = new(name: "XYZ");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Ystep = new(name: "YStep");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Zadb = new(name: "ZaDb");

    /// <summary>
    ///     A name of a base 14 type 1 font
    /// </summary>
    public static readonly PdfName Zapfdingbats = new(name: "ZapfDingbats");

    /// <summary>
    ///     A name
    /// </summary>
    public static readonly PdfName Zoom = new(name: "Zoom");

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
                    var name = (PdfName)curFld.GetValue(obj: null);
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
    public PdfName(string name) : this(name, lengthCheck: true) => Key = name;

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

        Key = name;

        // The minimum number of characters in a name is 0, the maximum is 127 (the '/' not included)
        var length = name.Length;

        if (lengthCheck && length > 127)
        {
            throw new ArgumentException("The name '" + name + "' is too long (" + length + " characters).");
        }

        Bytes = EncodeName(name);
    }

    public PdfName(byte[] bytes) : base(NAME, bytes) => Key = PdfEncodings.ConvertToString(Bytes, encoding: null);

    /// <summary>
    ///     Name of this object
    /// </summary>
    public string Key { get; private set; }

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
        pdfName.Append(c: '/');
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
                    pdfName.Append(c: '#');
                    pdfName.Append(Convert.ToString(character, toBase: 16));

                    break;
                default:
                    if (character > 126 || character < 32)
                    {
                        pdfName.Append(c: '#');

                        if (character < 16)
                        {
                            pdfName.Append(c: '0');
                        }

                        pdfName.Append(Convert.ToString(character, toBase: 16));
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