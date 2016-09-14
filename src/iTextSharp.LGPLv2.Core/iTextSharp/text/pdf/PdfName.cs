using System;
using System.Text;
using System.Collections;
using System.Reflection;

namespace iTextSharp.text.pdf
{
    /// <summary>
    ///  PdfName  is an object that can be used as a name in a PDF-file.
    ///
    /// A name, like a string, is a sequence of characters. It must begin with a slash
    /// followed by a sequence of ASCII characters in the range 32 through 136 except
    /// %, (, ), [, ], &lt;, &gt;, {, }, / and #. Any character except 0x00 may be included
    /// in a name by writing its twocharacter hex code, preceded by #. The maximum number
    /// of characters in a name is 127.
    /// This object is described in the 'Portable Document Format Reference Manual version 1.3'
    /// section 4.5 (page 39-40).
    ///
    /// @see        PdfObject
    /// @see        PdfDictionary
    /// @see        BadPdfFormatException
    /// </summary>
    public class PdfName : PdfObject, IComparable
    {

        /// <summary>
        /// CLASS CONSTANTS (a variety of standard names used in PDF))
        /// </summary>
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName _3D = new PdfName("3D");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName A = new PdfName("A");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Aa = new PdfName("AA");
        /// <summary>
        /// A name
        /// @since 2.1.5 renamed from ABSOLUTECALORIMETRIC
        /// </summary>
        public static readonly PdfName Absolutecolorimetric = new PdfName("AbsoluteColorimetric");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ac = new PdfName("AC");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Acroform = new PdfName("AcroForm");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Action = new PdfName("Action");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Activation = new PdfName("Activation");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Adbe = new PdfName("ADBE");
        /// <summary>
        /// a name used in PDF structure
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Actualtext = new PdfName("ActualText");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName AdbePkcs7Detached = new PdfName("adbe.pkcs7.detached");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName AdbePkcs7S4 = new PdfName("adbe.pkcs7.s4");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName AdbePkcs7S5 = new PdfName("adbe.pkcs7.s5");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName AdbePkcs7Sha1 = new PdfName("adbe.pkcs7.sha1");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName AdbeX509RsaSha1 = new PdfName("adbe.x509.rsa_sha1");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName AdobePpklite = new PdfName("Adobe.PPKLite");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName AdobePpkms = new PdfName("Adobe.PPKMS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Aesv2 = new PdfName("AESV2");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ais = new PdfName("AIS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Allpages = new PdfName("AllPages");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Alt = new PdfName("Alt");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Alternate = new PdfName("Alternate");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Animation = new PdfName("Animation");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Annot = new PdfName("Annot");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Annots = new PdfName("Annots");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Antialias = new PdfName("AntiAlias");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ap = new PdfName("AP");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Appdefault = new PdfName("AppDefault");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Art = new PdfName("Art");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Artbox = new PdfName("ArtBox");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ascent = new PdfName("Ascent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName As = new PdfName("AS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ascii85Decode = new PdfName("ASCII85Decode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Asciihexdecode = new PdfName("ASCIIHexDecode");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Asset = new PdfName("Asset");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Assets = new PdfName("Assets");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Authevent = new PdfName("AuthEvent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Author = new PdfName("Author");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName B = new PdfName("B");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Background = new PdfName("Background");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Baseencoding = new PdfName("BaseEncoding");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Basefont = new PdfName("BaseFont");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Baseversion = new PdfName("BaseVersion");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bbox = new PdfName("BBox");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bc = new PdfName("BC");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bg = new PdfName("BG");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Bibentry = new PdfName("BibEntry");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bigfive = new PdfName("BigFive");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Binding = new PdfName("Binding");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Bindingmaterialname = new PdfName("BindingMaterialName");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bitspercomponent = new PdfName("BitsPerComponent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bitspersample = new PdfName("BitsPerSample");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bl = new PdfName("Bl");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Blackis1 = new PdfName("BlackIs1");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Blackpoint = new PdfName("BlackPoint");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Blockquote = new PdfName("BlockQuote");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bleedbox = new PdfName("BleedBox");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Blinds = new PdfName("Blinds");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bm = new PdfName("BM");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Border = new PdfName("Border");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bounds = new PdfName("Bounds");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Box = new PdfName("Box");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Bs = new PdfName("BS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Btn = new PdfName("Btn");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Byterange = new PdfName("ByteRange");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName C = new PdfName("C");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName C0 = new PdfName("C0");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName C1 = new PdfName("C1");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName CA = new PdfName("CA");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName CA_ = new PdfName("ca");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Calgray = new PdfName("CalGray");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Calrgb = new PdfName("CalRGB");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Capheight = new PdfName("CapHeight");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Caption = new PdfName("Caption");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Catalog = new PdfName("Catalog");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Category = new PdfName("Category");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ccittfaxdecode = new PdfName("CCITTFaxDecode");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Center = new PdfName("Center");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Centerwindow = new PdfName("CenterWindow");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cert = new PdfName("Cert");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cf = new PdfName("CF");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cfm = new PdfName("CFM");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ch = new PdfName("Ch");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Charprocs = new PdfName("CharProcs");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ci = new PdfName("CI");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cidfonttype0 = new PdfName("CIDFontType0");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cidfonttype2 = new PdfName("CIDFontType2");
        /// <summary>
        /// A name
        /// @since 2.0.7
        /// </summary>
        public static readonly PdfName Cidset = new PdfName("CIDSet");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cidsysteminfo = new PdfName("CIDSystemInfo");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cidtogidmap = new PdfName("CIDToGIDMap");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Circle = new PdfName("Circle");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Cmd = new PdfName("CMD");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Co = new PdfName("CO");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Code = new PdfName("Code");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Colors = new PdfName("Colors");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Colorspace = new PdfName("ColorSpace");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Collection = new PdfName("Collection");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Collectionfield = new PdfName("CollectionField");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Collectionitem = new PdfName("CollectionItem");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Collectionschema = new PdfName("CollectionSchema");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Collectionsort = new PdfName("CollectionSort");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Collectionsubitem = new PdfName("CollectionSubitem");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Columns = new PdfName("Columns");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Condition = new PdfName("Condition");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Configuration = new PdfName("Configuration");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Configurations = new PdfName("Configurations");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Contactinfo = new PdfName("ContactInfo");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName CONTENT = new PdfName("Content");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Contents = new PdfName("Contents");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Coords = new PdfName("Coords");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Count = new PdfName("Count");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName Courier = new PdfName("Courier");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName CourierBold = new PdfName("Courier-Bold");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName CourierOblique = new PdfName("Courier-Oblique");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName CourierBoldoblique = new PdfName("Courier-BoldOblique");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Creationdate = new PdfName("CreationDate");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Creator = new PdfName("Creator");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Creatorinfo = new PdfName("CreatorInfo");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cropbox = new PdfName("CropBox");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Crypt = new PdfName("Crypt");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Cs = new PdfName("CS");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Cuepoint = new PdfName("CuePoint");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Cuepoints = new PdfName("CuePoints");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName D = new PdfName("D");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Da = new PdfName("DA");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Data = new PdfName("Data");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dc = new PdfName("DC");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dctdecode = new PdfName("DCTDecode");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Deactivation = new PdfName("Deactivation");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Decode = new PdfName("Decode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Decodeparms = new PdfName("DecodeParms");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Default = new PdfName("Default");
        /// <summary>
        /// A name
        /// @since	2.1.5 renamed from DEFAULTCRYPTFILER
        /// </summary>
        public static readonly PdfName Defaultcryptfilter = new PdfName("DefaultCryptFilter");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Defaultcmyk = new PdfName("DefaultCMYK");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Defaultgray = new PdfName("DefaultGray");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Defaultrgb = new PdfName("DefaultRGB");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Desc = new PdfName("Desc");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Descendantfonts = new PdfName("DescendantFonts");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Descent = new PdfName("Descent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dest = new PdfName("Dest");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Destoutputprofile = new PdfName("DestOutputProfile");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dests = new PdfName("Dests");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Devicegray = new PdfName("DeviceGray");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Devicergb = new PdfName("DeviceRGB");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Devicecmyk = new PdfName("DeviceCMYK");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Di = new PdfName("Di");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Differences = new PdfName("Differences");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dissolve = new PdfName("Dissolve");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Direction = new PdfName("Direction");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Displaydoctitle = new PdfName("DisplayDocTitle");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Div = new PdfName("Div");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dm = new PdfName("Dm");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Docmdp = new PdfName("DocMDP");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Docopen = new PdfName("DocOpen");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Document = new PdfName("Document");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Domain = new PdfName("Domain");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dp = new PdfName("DP");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dr = new PdfName("DR");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ds = new PdfName("DS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dur = new PdfName("Dur");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Duplex = new PdfName("Duplex");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Duplexflipshortedge = new PdfName("DuplexFlipShortEdge");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Duplexfliplongedge = new PdfName("DuplexFlipLongEdge");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dv = new PdfName("DV");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Dw = new PdfName("DW");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName E = new PdfName("E");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Earlychange = new PdfName("EarlyChange");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName EF = new PdfName("EF");
        /// <summary>
        /// A name
        /// @since	2.1.3
        /// </summary>
        public static readonly PdfName Eff = new PdfName("EFF");
        /// <summary>
        /// A name
        /// @since	2.1.3
        /// </summary>
        public static readonly PdfName Efopen = new PdfName("EFOpen");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Embedded = new PdfName("Embedded");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Embeddedfile = new PdfName("EmbeddedFile");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Embeddedfiles = new PdfName("EmbeddedFiles");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Encode = new PdfName("Encode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Encodedbytealign = new PdfName("EncodedByteAlign");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Encoding = new PdfName("Encoding");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Encrypt = new PdfName("Encrypt");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Encryptmetadata = new PdfName("EncryptMetadata");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Endofblock = new PdfName("EndOfBlock");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Endofline = new PdfName("EndOfLine");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Extend = new PdfName("Extend");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Extensions = new PdfName("Extensions");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Extensionlevel = new PdfName("ExtensionLevel");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Extgstate = new PdfName("ExtGState");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Export = new PdfName("Export");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Exportstate = new PdfName("ExportState");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Event = new PdfName("Event");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName F = new PdfName("F");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Far = new PdfName("Far");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fb = new PdfName("FB");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fdecodeparms = new PdfName("FDecodeParms");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fdf = new PdfName("FDF");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ff = new PdfName("Ff");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ffilter = new PdfName("FFilter");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fields = new PdfName("Fields");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Figure = new PdfName("Figure");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fileattachment = new PdfName("FileAttachment");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Filespec = new PdfName("Filespec");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Filter = new PdfName("Filter");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName First = new PdfName("First");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Firstchar = new PdfName("FirstChar");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Firstpage = new PdfName("FirstPage");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fit = new PdfName("Fit");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fith = new PdfName("FitH");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fitv = new PdfName("FitV");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fitr = new PdfName("FitR");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fitb = new PdfName("FitB");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fitbh = new PdfName("FitBH");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fitbv = new PdfName("FitBV");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fitwindow = new PdfName("FitWindow");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Flags = new PdfName("Flags");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Flash = new PdfName("Flash");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Flashvars = new PdfName("FlashVars");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Flatedecode = new PdfName("FlateDecode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fo = new PdfName("Fo");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Font = new PdfName("Font");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fontbbox = new PdfName("FontBBox");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fontdescriptor = new PdfName("FontDescriptor");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fontfile = new PdfName("FontFile");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fontfile2 = new PdfName("FontFile2");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fontfile3 = new PdfName("FontFile3");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fontmatrix = new PdfName("FontMatrix");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fontname = new PdfName("FontName");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Foreground = new PdfName("Foreground");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Form = new PdfName("Form");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Formtype = new PdfName("FormType");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Formula = new PdfName("Formula");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Freetext = new PdfName("FreeText");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Frm = new PdfName("FRM");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fs = new PdfName("FS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ft = new PdfName("FT");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Fullscreen = new PdfName("FullScreen");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Function = new PdfName("Function");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Functions = new PdfName("Functions");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Functiontype = new PdfName("FunctionType");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Gamma = new PdfName("Gamma");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Gbk = new PdfName("GBK");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Glitter = new PdfName("Glitter");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Goto = new PdfName("GoTo");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Gotoe = new PdfName("GoToE");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Gotor = new PdfName("GoToR");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Group = new PdfName("Group");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName GtsPdfa1 = new PdfName("GTS_PDFA1");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName GtsPdfx = new PdfName("GTS_PDFX");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName GtsPdfxversion = new PdfName("GTS_PDFXVersion");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName H = new PdfName("H");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName H1 = new PdfName("H1");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName H2 = new PdfName("H2");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName H3 = new PdfName("H3");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName H4 = new PdfName("H4");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName H5 = new PdfName("H5");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName H6 = new PdfName("H6");

        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Halign = new PdfName("HAlign");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Height = new PdfName("Height");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Helv = new PdfName("Helv");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName Helvetica = new PdfName("Helvetica");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName HelveticaBold = new PdfName("Helvetica-Bold");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName HelveticaOblique = new PdfName("Helvetica-Oblique");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName HelveticaBoldoblique = new PdfName("Helvetica-BoldOblique");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Hid = new PdfName("Hid");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Hide = new PdfName("Hide");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Hidemenubar = new PdfName("HideMenubar");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Hidetoolbar = new PdfName("HideToolbar");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Hidewindowui = new PdfName("HideWindowUI");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Highlight = new PdfName("Highlight");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Hoffset = new PdfName("HOffset");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName I = new PdfName("I");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Iccbased = new PdfName("ICCBased");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Id = new PdfName("ID");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Identity = new PdfName("Identity");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName If = new PdfName("IF");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Image = new PdfName("Image");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Imageb = new PdfName("ImageB");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Imagec = new PdfName("ImageC");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Imagei = new PdfName("ImageI");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Imagemask = new PdfName("ImageMask");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Index = new PdfName("Index");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Indexed = new PdfName("Indexed");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Info = new PdfName("Info");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ink = new PdfName("Ink");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Inklist = new PdfName("InkList");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Instances = new PdfName("Instances");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Importdata = new PdfName("ImportData");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Intent = new PdfName("Intent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Interpolate = new PdfName("Interpolate");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ismap = new PdfName("IsMap");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Irt = new PdfName("IRT");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Italicangle = new PdfName("ItalicAngle");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Itxt = new PdfName("ITXT");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ix = new PdfName("IX");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Javascript = new PdfName("JavaScript");
        /// <summary>
        /// A name
        /// @since	2.1.5
        /// </summary>
        public static readonly PdfName Jbig2Decode = new PdfName("JBIG2Decode");
        /// <summary>
        /// A name
        /// @since	2.1.5
        /// </summary>
        public static readonly PdfName Jbig2Globals = new PdfName("JBIG2Globals");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Jpxdecode = new PdfName("JPXDecode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Js = new PdfName("JS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName K = new PdfName("K");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Keywords = new PdfName("Keywords");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Kids = new PdfName("Kids");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName L = new PdfName("L");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName L2R = new PdfName("L2R");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Lang = new PdfName("Lang");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Language = new PdfName("Language");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Last = new PdfName("Last");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Lastchar = new PdfName("LastChar");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Lastpage = new PdfName("LastPage");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Launch = new PdfName("Launch");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Lbl = new PdfName("Lbl");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Lbody = new PdfName("LBody");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName LENGTH = new PdfName("Length");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Length1 = new PdfName("Length1");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Li = new PdfName("LI");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Limits = new PdfName("Limits");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Line = new PdfName("Line");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Linear = new PdfName("Linear");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Link = new PdfName("Link");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Listmode = new PdfName("ListMode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Location = new PdfName("Location");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Lock = new PdfName("Lock");
        /// <summary>
        /// A name
        /// @since	2.1.2
        /// </summary>
        public static readonly PdfName Locked = new PdfName("Locked");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Lzwdecode = new PdfName("LZWDecode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName M = new PdfName("M");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Material = new PdfName("Material");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Matrix = new PdfName("Matrix");
        /// <summary>
        /// A name of an encoding
        /// </summary>
        public static readonly PdfName MacExpertEncoding = new PdfName("MacExpertEncoding");
        /// <summary>
        /// A name of an encoding
        /// </summary>
        public static readonly PdfName MacRomanEncoding = new PdfName("MacRomanEncoding");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Marked = new PdfName("Marked");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Markinfo = new PdfName("MarkInfo");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Mask = new PdfName("Mask");
        /// <summary>
        /// A name
        /// @since	2.1.6 renamed from MAX
        /// </summary>
        public static readonly PdfName MaxLowerCase = new PdfName("max");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName MaxCamelCase = new PdfName("Max");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Maxlen = new PdfName("MaxLen");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Mediabox = new PdfName("MediaBox");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Mcid = new PdfName("MCID");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Mcr = new PdfName("MCR");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Metadata = new PdfName("Metadata");
        /// <summary>
        /// A name
        /// @since	2.1.6 renamed from MIN
        /// </summary>
        public static readonly PdfName MinLowerCase = new PdfName("min");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName MinCamelCase = new PdfName("Min");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Mk = new PdfName("MK");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Mmtype1 = new PdfName("MMType1");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Moddate = new PdfName("ModDate");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName N = new PdfName("N");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName N0 = new PdfName("n0");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName N1 = new PdfName("n1");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName N2 = new PdfName("n2");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName N3 = new PdfName("n3");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName N4 = new PdfName("n4");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Name = new PdfName("Name");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Named = new PdfName("Named");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Names = new PdfName("Names");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Navigation = new PdfName("Navigation");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Navigationpane = new PdfName("NavigationPane");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Near = new PdfName("Near");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Needappearances = new PdfName("NeedAppearances");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Newwindow = new PdfName("NewWindow");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Next = new PdfName("Next");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Nextpage = new PdfName("NextPage");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Nm = new PdfName("NM");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName None = new PdfName("None");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Nonfullscreenpagemode = new PdfName("NonFullScreenPageMode");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Nonstruct = new PdfName("NonStruct");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Note = new PdfName("Note");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Numcopies = new PdfName("NumCopies");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Nums = new PdfName("Nums");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName O = new PdfName("O");
        /// <summary>
        /// A name used with Document Structure
        /// @since 2.1.5
        /// </summary>
        public static readonly PdfName Obj = new PdfName("Obj");
        /// <summary>
        /// a name used with Doucment Structure
        /// @since 2.1.5
        /// </summary>
        public static readonly PdfName Objr = new PdfName("OBJR");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Objstm = new PdfName("ObjStm");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Oc = new PdfName("OC");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ocg = new PdfName("OCG");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ocgs = new PdfName("OCGs");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ocmd = new PdfName("OCMD");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ocproperties = new PdfName("OCProperties");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Off = new PdfName("Off");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName OFF = new PdfName("OFF");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName On = new PdfName("ON");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Onecolumn = new PdfName("OneColumn");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Open = new PdfName("Open");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Openaction = new PdfName("OpenAction");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Op = new PdfName("OP");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Op_ = new PdfName("op");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Opm = new PdfName("OPM");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Opt = new PdfName("Opt");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Order = new PdfName("Order");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ordering = new PdfName("Ordering");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Oscillating = new PdfName("Oscillating");

        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Outlines = new PdfName("Outlines");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Outputcondition = new PdfName("OutputCondition");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Outputconditionidentifier = new PdfName("OutputConditionIdentifier");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Outputintent = new PdfName("OutputIntent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Outputintents = new PdfName("OutputIntents");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName P = new PdfName("P");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Page = new PdfName("Page");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pagelabels = new PdfName("PageLabels");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pagelayout = new PdfName("PageLayout");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pagemode = new PdfName("PageMode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pages = new PdfName("Pages");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Painttype = new PdfName("PaintType");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Panose = new PdfName("Panose");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Params = new PdfName("Params");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Parent = new PdfName("Parent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Parenttree = new PdfName("ParentTree");
        /// <summary>
        /// A name used in defining Document Structure.
        /// @since 2.1.5
        /// </summary>
        public static readonly PdfName Parenttreenextkey = new PdfName("ParentTreeNextKey");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Part = new PdfName("Part");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Passcontextclick = new PdfName("PassContextClick");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pattern = new PdfName("Pattern");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Patterntype = new PdfName("PatternType");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Pc = new PdfName("PC");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pdf = new PdfName("PDF");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pdfdocencoding = new PdfName("PDFDocEncoding");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Perceptual = new PdfName("Perceptual");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Perms = new PdfName("Perms");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pg = new PdfName("Pg");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Pi = new PdfName("PI");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Picktraybypdfsize = new PdfName("PickTrayByPDFSize");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Playcount = new PdfName("PlayCount");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Po = new PdfName("PO");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Popup = new PdfName("Popup");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Position = new PdfName("Position");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Predictor = new PdfName("Predictor");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Preferred = new PdfName("Preferred");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Presentation = new PdfName("Presentation");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Preserverb = new PdfName("PreserveRB");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Prev = new PdfName("Prev");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Prevpage = new PdfName("PrevPage");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Print = new PdfName("Print");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Printarea = new PdfName("PrintArea");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Printclip = new PdfName("PrintClip");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Printpagerange = new PdfName("PrintPageRange");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Printscaling = new PdfName("PrintScaling");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Printstate = new PdfName("PrintState");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Private = new PdfName("Private");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Procset = new PdfName("ProcSet");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Producer = new PdfName("Producer");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Properties = new PdfName("Properties");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ps = new PdfName("PS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Pubsec = new PdfName("Adobe.PubSec");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Pv = new PdfName("PV");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Q = new PdfName("Q");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Quadpoints = new PdfName("QuadPoints");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Quote = new PdfName("Quote");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName R = new PdfName("R");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName R2L = new PdfName("R2L");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Range = new PdfName("Range");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Rc = new PdfName("RC");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Rbgroups = new PdfName("RBGroups");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Reason = new PdfName("Reason");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Recipients = new PdfName("Recipients");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Rect = new PdfName("Rect");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Reference = new PdfName("Reference");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Registry = new PdfName("Registry");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Registryname = new PdfName("RegistryName");
        /// <summary>
        /// A name
        /// @since	2.1.5 renamed from RELATIVECALORIMETRIC
        /// </summary>
        public static readonly PdfName Relativecolorimetric = new PdfName("RelativeColorimetric");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Rendition = new PdfName("Rendition");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Resetform = new PdfName("ResetForm");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Resources = new PdfName("Resources");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ri = new PdfName("RI");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmedia = new PdfName("RichMedia");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediaactivation = new PdfName("RichMediaActivation");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediaanimation = new PdfName("RichMediaAnimation");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Richmediacommand = new PdfName("RichMediaCommand");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediaconfiguration = new PdfName("RichMediaConfiguration");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediacontent = new PdfName("RichMediaContent");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediadeactivation = new PdfName("RichMediaDeactivation");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediaexecute = new PdfName("RichMediaExecute");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediainstance = new PdfName("RichMediaInstance");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediaparams = new PdfName("RichMediaParams");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediaposition = new PdfName("RichMediaPosition");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediapresentation = new PdfName("RichMediaPresentation");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediasettings = new PdfName("RichMediaSettings");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Richmediawindow = new PdfName("RichMediaWindow");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Rolemap = new PdfName("RoleMap");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Root = new PdfName("Root");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Rotate = new PdfName("Rotate");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Rows = new PdfName("Rows");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Ruby = new PdfName("Ruby");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Runlengthdecode = new PdfName("RunLengthDecode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Rv = new PdfName("RV");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName S = new PdfName("S");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Saturation = new PdfName("Saturation");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Schema = new PdfName("Schema");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Screen = new PdfName("Screen");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Scripts = new PdfName("Scripts");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Sect = new PdfName("Sect");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Separation = new PdfName("Separation");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Setocgstate = new PdfName("SetOCGState");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Settings = new PdfName("Settings");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Shading = new PdfName("Shading");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Shadingtype = new PdfName("ShadingType");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName ShiftJis = new PdfName("Shift-JIS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Sig = new PdfName("Sig");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Sigflags = new PdfName("SigFlags");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Sigref = new PdfName("SigRef");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Simplex = new PdfName("Simplex");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Singlepage = new PdfName("SinglePage");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Size = new PdfName("Size");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Smask = new PdfName("SMask");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Sort = new PdfName("Sort");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Sound = new PdfName("Sound");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Span = new PdfName("Span");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Speed = new PdfName("Speed");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Split = new PdfName("Split");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Square = new PdfName("Square");
        /// <summary>
        /// A name
        /// @since 2.1.3
        /// </summary>
        public static readonly PdfName Squiggly = new PdfName("Squiggly");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName St = new PdfName("St");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Stamp = new PdfName("Stamp");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Standard = new PdfName("Standard");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName State = new PdfName("State");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Stdcf = new PdfName("StdCF");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Stemv = new PdfName("StemV");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Stmf = new PdfName("StmF");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Strf = new PdfName("StrF");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Strikeout = new PdfName("StrikeOut");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Structparent = new PdfName("StructParent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Structparents = new PdfName("StructParents");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Structtreeroot = new PdfName("StructTreeRoot");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Style = new PdfName("Style");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Subfilter = new PdfName("SubFilter");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Subject = new PdfName("Subject");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Submitform = new PdfName("SubmitForm");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Subtype = new PdfName("Subtype");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Supplement = new PdfName("Supplement");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Sv = new PdfName("SV");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Sw = new PdfName("SW");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName Symbol = new PdfName("Symbol");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName T = new PdfName("T");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Ta = new PdfName("TA");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Table = new PdfName("Table");
        /// <summary>
        /// A name
        /// @since	2.1.5
        /// </summary>
        public static readonly PdfName Tabs = new PdfName("Tabs");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Tbody = new PdfName("TBody");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Td = new PdfName("TD");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Text = new PdfName("Text");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Tfoot = new PdfName("TFoot");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Th = new PdfName("TH");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Thead = new PdfName("THead");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Thumb = new PdfName("Thumb");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Threads = new PdfName("Threads");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ti = new PdfName("TI");
        /// <summary>
        /// A name
        /// @since	2.1.6
        /// </summary>
        public static readonly PdfName Time = new PdfName("Time");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Tilingtype = new PdfName("TilingType");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName TimesRoman = new PdfName("Times-Roman");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName TimesBold = new PdfName("Times-Bold");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName TimesItalic = new PdfName("Times-Italic");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName TimesBolditalic = new PdfName("Times-BoldItalic");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Title = new PdfName("Title");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Tk = new PdfName("TK");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Tm = new PdfName("TM");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Toc = new PdfName("TOC");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Toci = new PdfName("TOCI");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Toggle = new PdfName("Toggle");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Toolbar = new PdfName("Toolbar");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Tounicode = new PdfName("ToUnicode");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Tp = new PdfName("TP");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Tablerow = new PdfName("TR");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Trans = new PdfName("Trans");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Transformparams = new PdfName("TransformParams");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Transformmethod = new PdfName("TransformMethod");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Transparency = new PdfName("Transparency");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Transparent = new PdfName("Transparent");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Trapped = new PdfName("Trapped");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Trimbox = new PdfName("TrimBox");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Truetype = new PdfName("TrueType");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Tu = new PdfName("TU");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Twocolumnleft = new PdfName("TwoColumnLeft");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Twocolumnright = new PdfName("TwoColumnRight");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Twopageleft = new PdfName("TwoPageLeft");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Twopageright = new PdfName("TwoPageRight");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Tx = new PdfName("Tx");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName TYPE = new PdfName("Type");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Type0 = new PdfName("Type0");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Type1 = new PdfName("Type1");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Type3 = new PdfName("Type3");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName U = new PdfName("U");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Uf = new PdfName("UF");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Uhc = new PdfName("UHC");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Underline = new PdfName("Underline");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ur = new PdfName("UR");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ur3 = new PdfName("UR3");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Uri = new PdfName("URI");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Url = new PdfName("URL");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Usage = new PdfName("Usage");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Useattachments = new PdfName("UseAttachments");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Usenone = new PdfName("UseNone");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Useoc = new PdfName("UseOC");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Useoutlines = new PdfName("UseOutlines");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName User = new PdfName("User");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Userproperties = new PdfName("UserProperties");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Userunit = new PdfName("UserUnit");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Usethumbs = new PdfName("UseThumbs");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName V = new PdfName("V");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName V2 = new PdfName("V2");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Valign = new PdfName("VAlign");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName VerisignPpkvs = new PdfName("VeriSign.PPKVS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Version = new PdfName("Version");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Video = new PdfName("Video");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName View = new PdfName("View");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Views = new PdfName("Views");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Viewarea = new PdfName("ViewArea");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Viewclip = new PdfName("ViewClip");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Viewerpreferences = new PdfName("ViewerPreferences");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Viewstate = new PdfName("ViewState");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Visiblepages = new PdfName("VisiblePages");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Voffset = new PdfName("VOffset");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName W = new PdfName("W");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName W2 = new PdfName("W2");
        /// <summary>
        /// A name
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Warichu = new PdfName("Warichu");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Wc = new PdfName("WC");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Widget = new PdfName("Widget");
        /// <summary>
        /// A name of an attribute.
        /// </summary>
        public static readonly PdfName Width = new PdfName("Width");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Widths = new PdfName("Widths");
        /// <summary>
        /// A name of an encoding
        /// </summary>
        public static readonly PdfName Win = new PdfName("Win");
        /// <summary>
        /// A name of an encoding
        /// </summary>
        public static readonly PdfName WinAnsiEncoding = new PdfName("WinAnsiEncoding");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Window = new PdfName("Window");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Windowed = new PdfName("Windowed");
        /// <summary>
        /// A name of an encoding
        /// </summary>
        public static readonly PdfName Wipe = new PdfName("Wipe");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Whitepoint = new PdfName("WhitePoint");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Wp = new PdfName("WP");
        /// <summary>
        /// A name of an encoding
        /// </summary>
        public static readonly PdfName Ws = new PdfName("WS");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName X = new PdfName("X");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Xa = new PdfName("XA");
        /// <summary>
        /// A name.
        /// @since 2.1.6
        /// </summary>
        public static readonly PdfName Xd = new PdfName("XD");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Xfa = new PdfName("XFA");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Xml = new PdfName("XML");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Xobject = new PdfName("XObject");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Xstep = new PdfName("XStep");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Xref = new PdfName("XRef");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Xrefstm = new PdfName("XRefStm");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Xyz = new PdfName("XYZ");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Ystep = new PdfName("YStep");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Zadb = new PdfName("ZaDb");
        /// <summary>
        /// A name of a base 14 type 1 font
        /// </summary>
        public static readonly PdfName Zapfdingbats = new PdfName("ZapfDingbats");
        /// <summary>
        /// A name
        /// </summary>
        public static readonly PdfName Zoom = new PdfName("Zoom");

        /// <summary>
        /// map strings to all known static names
        /// @since 2.1.6
        /// </summary>
        public static Hashtable StaticNames;

        /// <summary>
        /// Use reflection to cache all the static public final names so
        /// future  PdfName  additions don't have to be "added twice".
        /// A bit less efficient (around 50ms spent here on a 2.2ghz machine),
        /// but Much Less error prone.
        /// @since 2.1.6
        /// </summary>

        static PdfName()
        {
            FieldInfo[] fields = typeof(PdfName).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            StaticNames = new Hashtable(fields.Length);
            try
            {
                for (int fldIdx = 0; fldIdx < fields.Length; ++fldIdx)
                {
                    FieldInfo curFld = fields[fldIdx];
                    if (curFld.FieldType == typeof(PdfName))
                    {
                        PdfName name = (PdfName)curFld.GetValue(null);
                        StaticNames[DecodeName(name.ToString())] = name;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// constructors
        /// </summary>

        /// <summary>
        /// Constructs a new  PdfName . The name length will be checked.
        /// </summary>
        /// <param name="name">the new name</param>
        public PdfName(string name) : this(name, true)
        {
        }

        /// <summary>
        /// Constructs a new  PdfName .
        /// have any length
        /// </summary>
        /// <param name="name">the new name</param>
        /// <param name="lengthCheck">if  true  check the lenght validity, if  false  the name can</param>
        public PdfName(string name, bool lengthCheck) : base(NAME)
        {
            // The minimum number of characters in a name is 0, the maximum is 127 (the '/' not included)
            int length = name.Length;
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
        /// methods
        /// </summary>

        /// <summary>
        /// Compares this object with the specified object for order.  Returns a
        /// negative int, zero, or a positive int as this object is less
        /// than, equal to, or greater than the specified object.
        /// is less than, equal to, or greater than the specified object.
        /// @throws Exception if the specified object's type prevents it
        /// from being compared to this Object.
        /// </summary>
        /// <param name="obj">the Object to be compared.</param>
        /// <returns>a negative int, zero, or a positive int as this object</returns>
        public int CompareTo(object obj)
        {
            PdfName name = (PdfName)obj;

            byte[] myBytes = Bytes;
            byte[] objBytes = name.Bytes;
            int len = Math.Min(myBytes.Length, objBytes.Length);
            for (int i = 0; i < len; i++)
            {
                if (myBytes[i] > objBytes[i])
                    return 1;

                if (myBytes[i] < objBytes[i])
                    return -1;
            }
            if (myBytes.Length < objBytes.Length)
                return -1;
            if (myBytes.Length > objBytes.Length)
                return 1;
            return 0;
        }

        /// <summary>
        /// Indicates whether some other object is "equal to" this one.
        /// argument;  false  otherwise.
        /// </summary>
        /// <param name="obj">the reference object with which to compare.</param>
        /// <returns> true  if this object is the same as the obj</returns>
        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj is PdfName)
                return CompareTo(obj) == 0;
            return false;
        }

        /// <summary>
        /// Returns a hash code value for the object. This method is
        /// supported for the benefit of hashtables such as those provided by
        ///  java.util.Hashtable .
        /// </summary>
        /// <returns>a hash code value for this object.</returns>
        public override int GetHashCode()
        {
            int ptr = 0;
            int len = Bytes.Length;
            var h = 0;
            for (int i = 0; i < len; i++)
                h = 31 * h + (Bytes[ptr++] & 0xff);
            return h;
        }

        /// <summary>
        /// Encodes a plain name given in the unescaped form "AB CD" into "/AB#20CD".
        /// @since	2.1.5
        /// </summary>
        /// <param name="name">the name to encode</param>
        /// <returns>the encoded name</returns>
        public static byte[] EncodeName(string name)
        {
            int length = name.Length;
            // every special character has to be substituted
            ByteBuffer pdfName = new ByteBuffer(length + 20);
            pdfName.Append('/');
            char[] chars = name.ToCharArray();
            char character;
            // loop over all the characters
            foreach (char cc in chars)
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
                                pdfName.Append('0');
                            pdfName.Append(Convert.ToString(character, 16));
                        }
                        else
                            pdfName.Append(character);
                        break;
                }
            }
            return pdfName.ToByteArray();
        }

        /// <summary>
        /// Decodes an escaped name in the form "/AB#20CD" into "AB CD".
        /// </summary>
        /// <param name="name">the name to decode</param>
        /// <returns>the decoded name</returns>
        public static string DecodeName(string name)
        {
            StringBuilder buf = new StringBuilder();
            int len = name.Length;
            for (int k = 1; k < len; ++k)
            {
                char c = name[k];
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
}
