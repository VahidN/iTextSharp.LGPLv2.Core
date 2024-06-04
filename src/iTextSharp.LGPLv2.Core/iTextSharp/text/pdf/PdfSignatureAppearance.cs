using System.Text;
using System.util;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
///     This class takes care of the cryptographic options and appearances that form a signature.
/// </summary>
public class PdfSignatureAppearance
{
    /// <summary>
    ///     Enumeration representing the different rendering options of a signature
    /// </summary>
    public enum SignatureRender
    {
        Description,
        NameAndDescription,
        GraphicAndDescription,
    }

    public const int CERTIFIED_FORM_FILLING = 2;

    public const int CERTIFIED_FORM_FILLING_AND_ANNOTATIONS = 3;

    public const int CERTIFIED_NO_CHANGES_ALLOWED = 1;

    public const int NOT_CERTIFIED = 0;

    /// <summary>
    ///     Commands to draw a yellow question mark in a stream content
    /// </summary>
    public const string questionMark =
        "% DSUnknown\n" +
        "q\n" +
        "1 G\n" +
        "1 g\n" +
        "0.1 0 0 0.1 9 0 cm\n" +
        "0 J 0 j 4 M []0 d\n" +
        "1 i \n" +
        "0 g\n" +
        "313 292 m\n" +
        "313 404 325 453 432 529 c\n" +
        "478 561 504 597 504 645 c\n" +
        "504 736 440 760 391 760 c\n" +
        "286 760 271 681 265 626 c\n" +
        "265 625 l\n" +
        "100 625 l\n" +
        "100 828 253 898 381 898 c\n" +
        "451 898 679 878 679 650 c\n" +
        "679 555 628 499 538 435 c\n" +
        "488 399 467 376 467 292 c\n" +
        "313 292 l\n" +
        "h\n" +
        "308 214 170 -164 re\n" +
        "f\n" +
        "0.44 G\n" +
        "1.2 w\n" +
        "1 1 0.4 rg\n" +
        "287 318 m\n" +
        "287 430 299 479 406 555 c\n" +
        "451 587 478 623 478 671 c\n" +
        "478 762 414 786 365 786 c\n" +
        "260 786 245 707 239 652 c\n" +
        "239 651 l\n" +
        "74 651 l\n" +
        "74 854 227 924 355 924 c\n" +
        "425 924 653 904 653 676 c\n" +
        "653 581 602 525 512 461 c\n" +
        "462 425 441 402 441 318 c\n" +
        "287 318 l\n" +
        "h\n" +
        "282 240 170 -164 re\n" +
        "B\n" +
        "Q\n";

    private const float Margin = 2;

    private const float TopSection = 0.3f;

    /// <summary>
    ///     The self signed filter.
    /// </summary>
    public static PdfName SelfSigned = PdfName.AdobePpklite;

    /// <summary>
    ///     The VeriSign filter.
    /// </summary>
    public static PdfName VerisignSigned = PdfName.VerisignPpkvs;

    /// <summary>
    ///     The Windows Certificate Security.
    /// </summary>
    public static PdfName WincerSigned = PdfName.AdobePpkms;

    private readonly PdfTemplate[] _app = new PdfTemplate[5];

    private readonly PdfStamperImp _writer;

    /// <summary>
    ///     Holds value of property acro6Layers.
    /// </summary>
    private bool _acro6Layers;

    private byte[] _bout;

    private int _boutLen;

    /// <summary>
    ///     Holds value of property contact.
    /// </summary>
    private string _contact;

    private string _digestEncryptionAlgorithm;

    private NullValueDictionary<PdfName, PdfLiteral> _exclusionLocations;

    private byte[] _externalDigest;

    private byte[] _externalRsAdata;

    private PdfTemplate _frm;

    /// <summary>
    ///     Holds value of property image.
    /// </summary>
    private Image _image;

    /// <summary>
    ///     Holds value of property imageScale.
    /// </summary>
    private float _imageScale;

    /// <summary>
    ///     Holds value of property layer2Font.
    /// </summary>
    private Font _layer2Font;

    /// <summary>
    ///     Holds value of property layer4Text.
    /// </summary>
    private string _layer4Text;

    private bool _newField;

    private bool _preClosed;

    private FileStream _raf;

    private int[] _range;

    /// <summary>
    ///     Holds value of property runDirection.
    /// </summary>
    private int _runDirection = PdfWriter.RUN_DIRECTION_NO_BIDI;

    /// <summary>
    ///     Holds value of property signatureEvent.
    /// </summary>
    private ISignatureEvent _signatureEvent;

    internal PdfSignatureAppearance(PdfStamperImp writer)
    {
        _writer = writer;
        SignDate = DateTime.Now;
        FieldName = GetNewSigName();
    }

    /// <summary>
    ///     Acrobat 6.0 and higher recomends that only layer n2 and n4 be present. This method sets that mode.
    /// </summary>
    public bool Acro6Layers
    {
        get => _acro6Layers;
        set => _acro6Layers = value;
    }

    /// <summary>
    ///     Gets the certificate chain.
    /// </summary>
    /// <returns>the certificate chain</returns>
    public X509Certificate[] CertChain { get; private set; }

    /// <summary>
    ///     Sets the document type to certified instead of simply signed.
    ///     CERTIFIED_FORM_FILLING  and  CERTIFIED_FORM_FILLING_AND_ANNOTATIONS
    /// </summary>
    public int CertificationLevel { get; set; } = NOT_CERTIFIED;

    /// <summary>
    ///     Sets the signing contact.
    /// </summary>
    public string Contact
    {
        get => _contact;
        set => _contact = value;
    }

    /// <summary>
    ///     Gets the certificate revocation list.
    /// </summary>
    /// <returns>the certificate revocation list</returns>
    public object[] CrlList { get; private set; }

    /// <summary>
    ///     Gets the user made signature dictionary. This is the dictionary at the /V key.
    /// </summary>
    /// <returns>the user made signature dictionary</returns>
    public PdfDictionary CryptoDictionary { get; set; }

    /// <summary>
    ///     Gets the field name.
    /// </summary>
    /// <returns>the field name</returns>
    public string FieldName { get; private set; }

    /// <summary>
    ///     Gets the filter used to sign the document.
    /// </summary>
    /// <returns>the filter used to sign the document</returns>
    public PdfName Filter { get; private set; }

    /// <summary>
    ///     Sets the background image for the layer 2.
    /// </summary>
    public Image Image
    {
        get => _image;
        set => _image = value;
    }

    /// <summary>
    ///     Sets the scaling to be applied to the background image. If it's zero the image
    ///     will fully fill the rectangle. If it's less than zero the image will fill the rectangle but
    ///     will keep the proportions. If it's greater than zero that scaling will be applied.
    ///     In any of the cases the image will always be centered. It's zero by default.
    /// </summary>
    public float ImageScale
    {
        get => _imageScale;
        set => _imageScale = value;
    }

    /// <summary>
    ///     Sets the n2 and n4 layer font. If the font size is zero, auto-fit will be used.
    /// </summary>
    public Font Layer2Font
    {
        get => _layer2Font;
        set => _layer2Font = value;
    }

    /// <summary>
    ///     Sets the signature text identifying the signer.
    ///     a standard description will be used
    /// </summary>
    public string Layer2Text { get; set; }

    /// <summary>
    ///     Sets the text identifying the signature status.
    ///     the description "Signature Not Verified" will be used
    /// </summary>
    public string Layer4Text
    {
        get => _layer4Text;
        set => _layer4Text = value;
    }

    /// <summary>
    ///     Sets the signing location.
    /// </summary>
    public string Location { get; set; }

    /// <summary>
    ///     Gets the page number of the field.
    /// </summary>
    /// <returns>the page number of the field</returns>
    public int Page { get; private set; } = 1;

    /// <summary>
    ///     Gets the rectangle that represent the position and dimension of the signature in the page.
    /// </summary>
    /// <returns>the rectangle that represent the position and dimension of the signature in the page</returns>
    public Rectangle PageRect { get; private set; }

    /// <summary>
    ///     Gets the private key.
    /// </summary>
    /// <returns>the private key</returns>
    public ICipherParameters PrivKey { get; private set; }

    /// <summary>
    ///     Gets the document bytes that are hashable when using external signatures. The general sequence is:
    ///     PreClose(), GetRangeStream() and Close().
    /// </summary>
    /// <returns>the document bytes that are hashable</returns>
    public Stream RangeStream => new FRangeStream(_raf, _bout, _range);

    /// <summary>
    ///     Sets the signing reason.
    /// </summary>
    public string Reason { get; set; }

    /// <summary>
    ///     Gets the rectangle representing the signature dimensions.
    ///     or have zero width or height for invisible signatures
    /// </summary>
    /// <returns>the rectangle representing the signature dimensions. It may be  null </returns>
    public Rectangle Rect { get; private set; }

    /// <summary>
    ///     Gets the rendering mode for this signature .
    /// </summary>
    /// <returns>the rectangle rendering mode for this signature.</returns>
    public SignatureRender Render { get; set; } = SignatureRender.Description;

    /// <summary>
    ///     Sets the run direction in the n2 and n4 layer.
    /// </summary>
    public int RunDirection
    {
        set
        {
            if (value < PdfWriter.RUN_DIRECTION_DEFAULT || value > PdfWriter.RUN_DIRECTION_RTL)
            {
                throw new ArgumentException("Invalid run direction: " + _runDirection);
            }

            _runDirection = value;
        }
        get => _runDirection;
    }

    /// <summary>
    ///     Sets the signature event to allow modification of the signature dictionary.
    /// </summary>
    public ISignatureEvent SignatureEvent
    {
        get => _signatureEvent;
        set => _signatureEvent = value;
    }

    /// <summary>
    ///     Sets the Image object to render when Render is set to SignatureRender.GraphicAndDescription
    ///     to SignatureRender.Description
    /// </summary>
    public Image SignatureGraphic { get; set; }

    /// <summary>
    ///     Gets the signature date.
    /// </summary>
    /// <returns>the signature date</returns>
    public DateTime SignDate { get; set; }

    /// <summary>
    ///     Gets the instance of the standard signature dictionary. This instance
    ///     is only available after pre close.
    ///     The main use is to insert external signatures.
    /// </summary>
    /// <returns>the instance of the standard signature dictionary</returns>
    public PdfSigGenericPkcs SigStandard { get; private set; }

    /// <summary>
    ///     Gets the  PdfStamper  associated with this instance.
    /// </summary>
    /// <returns>the  PdfStamper  associated with this instance</returns>
    public PdfStamper Stamper { get; private set; }

    /// <summary>
    ///     Gets the temporary file.
    /// </summary>
    /// <returns>the temporary file or  null  is the document is created in memory</returns>
    public string TempFile { get; private set; }

    internal Stream Originalout { get; set; }

    internal ByteBuffer Sigout { get; set; }

    /// <summary>
    ///     Fits the text to some rectangle adjusting the font size as needed.
    /// </summary>
    /// <param name="font">the font to use</param>
    /// <param name="text">the text</param>
    /// <param name="rect">the rectangle where the text must fit</param>
    /// <param name="maxFontSize">the maximum font size</param>
    /// <param name="runDirection">the run direction</param>
    /// <returns>the calculated font size that makes the text fit</returns>
    public static float FitText(Font font, string text, Rectangle rect, float maxFontSize, int runDirection)
    {
        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        if (rect == null)
        {
            throw new ArgumentNullException(nameof(rect));
        }

        ColumnText ct = null;
        var status = 0;
        if (maxFontSize <= 0)
        {
            var cr = 0;
            var lf = 0;
            var t = text.ToCharArray();
            for (var k = 0; k < t.Length; ++k)
            {
                if (t[k] == '\n')
                {
                    ++lf;
                }
                else if (t[k] == '\r')
                {
                    ++cr;
                }
            }

            var minLines = Math.Max(cr, lf) + 1;
            maxFontSize = Math.Abs(rect.Height) / minLines - 0.001f;
        }

        font.Size = maxFontSize;
        var ph = new Phrase(text, font);
        ct = new ColumnText(null);
        ct.SetSimpleColumn(ph, rect.Left, rect.Bottom, rect.Right, rect.Top, maxFontSize, Element.ALIGN_LEFT);
        ct.RunDirection = runDirection;
        status = ct.Go(true);
        if ((status & ColumnText.NO_MORE_TEXT) != 0)
        {
            return maxFontSize;
        }

        var precision = 0.1f;
        float min = 0;
        var max = maxFontSize;
        var size = maxFontSize;
        for (var k = 0; k < 50; ++k)
        {
            //just in case it doesn't converge
            size = (min + max) / 2;
            ct = new ColumnText(null);
            font.Size = size;
            ct.SetSimpleColumn(new Phrase(text, font), rect.Left, rect.Bottom, rect.Right, rect.Top, size,
                               Element.ALIGN_LEFT);
            ct.RunDirection = runDirection;
            status = ct.Go(true);
            if ((status & ColumnText.NO_MORE_TEXT) != 0)
            {
                if (max - min < size * precision)
                {
                    return size;
                }

                min = size;
            }
            else
            {
                max = size;
            }
        }

        return size;
    }

    /// <summary>
    ///     This is the last method to be called when using external signatures. The general sequence is:
    ///     PreClose(), GetDocumentBytes() and Close().
    ///     update  is a  PdfDictionary  that must have exactly the
    ///     same keys as the ones provided in {@link #preClose(Hashtable)}.
    ///     in {@link #preClose(Hashtable)}
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="update">a  PdfDictionary  with the key/value that will fill the holes defined</param>
    public void Close(PdfDictionary update)
    {
        if (update == null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        try
        {
            if (!_preClosed)
            {
                throw new DocumentException("preClose() must be called first.");
            }

            var bf = new ByteBuffer();
            foreach (var key in update.Keys)
            {
                var obj = update.Get(key);
                var lit = _exclusionLocations[key];
                if (lit == null)
                {
                    throw new ArgumentException("The key " + key + " didn't reserve space in PreClose().");
                }

                bf.Reset();
                obj.ToPdf(null, bf);
                if (bf.Size > lit.PosLength)
                {
                    throw new ArgumentException("The key " + key + " is too big. Is " + bf.Size + ", reserved " +
                                                lit.PosLength);
                }

                if (TempFile == null)
                {
                    Array.Copy(bf.Buffer, 0, _bout, lit.Position, bf.Size);
                }
                else
                {
                    _raf.Seek(lit.Position, SeekOrigin.Begin);
                    _raf.Write(bf.Buffer, 0, bf.Size);
                }
            }

            if (update.Size != _exclusionLocations.Count)
            {
                throw new ArgumentException("The update dictionary has less keys than required.");
            }

            if (TempFile == null)
            {
                Originalout.Write(_bout, 0, _boutLen);
            }
            else
            {
                if (Originalout != null)
                {
                    _raf.Seek(0, SeekOrigin.Begin);
                    var length = (int)_raf.Length;
                    var buf = new byte[8192];
                    while (length > 0)
                    {
                        var r = _raf.Read(buf, 0, Math.Min(buf.Length, length));
                        if (r < 0)
                        {
                            throw new EndOfStreamException("Unexpected EOF");
                        }

                        Originalout.Write(buf, 0, r);
                        length -= r;
                    }
                }
            }
        }
        finally
        {
            if (TempFile != null)
            {
                try
                {
                    _raf.Dispose();
                }
                catch
                {
                }

                if (Originalout != null)
                {
                    try
                    {
                        File.Delete(TempFile);
                    }
                    catch
                    {
                    }
                }
            }

            Originalout?.Seek(0, SeekOrigin.Begin);
        }
    }

    /// <summary>
    ///     Gets the main appearance layer.
    ///     Consult <A HREF="http://partners.adobe.com/asn/developer/pdfs/tn/PPKAppearances.pdf">PPKAppearances.pdf</A>
    ///     for further details.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <returns>the main appearance layer</returns>
    public PdfTemplate GetAppearance()
    {
        if (IsInvisible())
        {
            var t = new PdfTemplate(_writer);
            t.BoundingBox = new Rectangle(0, 0);
            _writer.AddDirectTemplateSimple(t, null);
            return t;
        }

        if (_app[0] == null)
        {
            var t = _app[0] = new PdfTemplate(_writer);
            t.BoundingBox = new Rectangle(100, 100);
            _writer.AddDirectTemplateSimple(t, new PdfName("n0"));
            t.SetLiteral("% DSBlank\n");
        }

        if (_app[1] == null && !_acro6Layers)
        {
            var t = _app[1] = new PdfTemplate(_writer);
            t.BoundingBox = new Rectangle(100, 100);
            _writer.AddDirectTemplateSimple(t, new PdfName("n1"));
            t.SetLiteral(questionMark);
        }

        if (_app[2] == null)
        {
            string text;
            if (Layer2Text == null)
            {
                var buf = new StringBuilder();
                buf.Append("Digitally signed by ").Append(PdfPkcs7.GetSubjectFields(CertChain[0]).GetField("CN"))
                   .Append('\n');
                buf.Append("Date: ").Append(SignDate.ToString("yyyy.MM.dd HH:mm:ss zzz", CultureInfo.InvariantCulture));
                if (Reason != null)
                {
                    buf.Append('\n').Append("Reason: ").Append(Reason);
                }

                if (Location != null)
                {
                    buf.Append('\n').Append("Location: ").Append(Location);
                }

                text = buf.ToString();
            }
            else
            {
                text = Layer2Text;
            }

            var t = _app[2] = new PdfTemplate(_writer);
            t.BoundingBox = Rect;
            _writer.AddDirectTemplateSimple(t, new PdfName("n2"));
            if (_image != null)
            {
                if (_imageScale.ApproxEquals(0))
                {
                    t.AddImage(_image, Rect.Width, 0, 0, Rect.Height, 0, 0);
                }
                else
                {
                    var usableScale = _imageScale;
                    if (_imageScale < 0)
                    {
                        usableScale = Math.Min(Rect.Width / _image.Width, Rect.Height / _image.Height);
                    }

                    var w = _image.Width * usableScale;
                    var h = _image.Height * usableScale;
                    var x = (Rect.Width - w) / 2;
                    var y = (Rect.Height - h) / 2;
                    t.AddImage(_image, w, 0, 0, h, x, y);
                }
            }

            Font font;
            if (_layer2Font == null)
            {
                font = new Font();
            }
            else
            {
                font = new Font(_layer2Font);
            }

            var size = font.Size;

            Rectangle dataRect = null;
            Rectangle signatureRect = null;

            if (Render == SignatureRender.NameAndDescription ||
                (Render == SignatureRender.GraphicAndDescription && SignatureGraphic != null))
            {
                // origin is the bottom-left
                signatureRect = new Rectangle(
                                              Margin,
                                              Margin,
                                              Rect.Width / 2 - Margin,
                                              Rect.Height - Margin);
                dataRect = new Rectangle(
                                         Rect.Width / 2 + Margin / 2,
                                         Margin,
                                         Rect.Width - Margin / 2,
                                         Rect.Height - Margin);

                if (Rect.Height > Rect.Width)
                {
                    signatureRect = new Rectangle(
                                                  Margin,
                                                  Rect.Height / 2,
                                                  Rect.Width - Margin,
                                                  Rect.Height);
                    dataRect = new Rectangle(
                                             Margin,
                                             Margin,
                                             Rect.Width - Margin,
                                             Rect.Height / 2 - Margin);
                }
            }
            else
            {
                dataRect = new Rectangle(
                                         Margin,
                                         Margin,
                                         Rect.Width - Margin,
                                         Rect.Height * (1 - TopSection) - Margin);
            }

            if (Render == SignatureRender.NameAndDescription)
            {
                var signedBy = PdfPkcs7.GetSubjectFields(CertChain[0]).GetField("CN");
                var sr2 = new Rectangle(signatureRect.Width - Margin, signatureRect.Height - Margin);
                var signedSize = FitText(font, signedBy, sr2, -1, _runDirection);

                var ct2 = new ColumnText(t);
                ct2.RunDirection = _runDirection;
                ct2.SetSimpleColumn(new Phrase(signedBy, font), signatureRect.Left, signatureRect.Bottom,
                                    signatureRect.Right, signatureRect.Top, signedSize, Element.ALIGN_LEFT);

                ct2.Go();
            }
            else if (Render == SignatureRender.GraphicAndDescription)
            {
                var ct2 = new ColumnText(t);
                ct2.RunDirection = _runDirection;
                ct2.SetSimpleColumn(signatureRect.Left, signatureRect.Bottom, signatureRect.Right, signatureRect.Top, 0,
                                    Element.ALIGN_RIGHT);

                var im = Image.GetInstance(SignatureGraphic);
                im.ScaleToFit(signatureRect.Width, signatureRect.Height);

                var p = new Paragraph();
                // must calculate the point to draw from to make image appear in middle of column
                float x = 0;
                // experimentation found this magic number to counteract Adobe's signature graphic, which
                // offsets the y co-ordinate by 15 units
                var y = -im.ScaledHeight + 15;

                x = x + (signatureRect.Width - im.ScaledWidth) / 2;
                y = y - (signatureRect.Height - im.ScaledHeight) / 2;
                p.Add(new Chunk(im, x + (signatureRect.Width - im.ScaledWidth) / 2, y, false));
                ct2.AddElement(p);
                ct2.Go();
            }

            if (size <= 0)
            {
                var sr = new Rectangle(dataRect.Width, dataRect.Height);
                size = FitText(font, text, sr, 12, _runDirection);
            }

            var ct = new ColumnText(t);
            ct.RunDirection = _runDirection;
            ct.SetSimpleColumn(new Phrase(text, font), dataRect.Left, dataRect.Bottom, dataRect.Right, dataRect.Top,
                               size, Element.ALIGN_LEFT);
            ct.Go();
        }

        if (_app[3] == null && !_acro6Layers)
        {
            var t = _app[3] = new PdfTemplate(_writer);
            t.BoundingBox = new Rectangle(100, 100);
            _writer.AddDirectTemplateSimple(t, new PdfName("n3"));
            t.SetLiteral("% DSBlank\n");
        }

        if (_app[4] == null && !_acro6Layers)
        {
            var t = _app[4] = new PdfTemplate(_writer);
            t.BoundingBox = new Rectangle(0, Rect.Height * (1 - TopSection), Rect.Right, Rect.Top);
            _writer.AddDirectTemplateSimple(t, new PdfName("n4"));
            Font font;
            if (_layer2Font == null)
            {
                font = new Font();
            }
            else
            {
                font = new Font(_layer2Font);
            }

            var size = font.Size;
            var text = "Signature Not Verified";
            if (_layer4Text != null)
            {
                text = _layer4Text;
            }

            var sr = new Rectangle(Rect.Width - 2 * Margin, Rect.Height * TopSection - 2 * Margin);
            size = FitText(font, text, sr, 15, _runDirection);
            var ct = new ColumnText(t);
            ct.RunDirection = _runDirection;
            ct.SetSimpleColumn(new Phrase(text, font), Margin, 0, Rect.Width - Margin, Rect.Height - Margin, size,
                               Element.ALIGN_LEFT);
            ct.Go();
        }

        var rotation = _writer.Reader.GetPageRotation(Page);
        var rotated = new Rectangle(Rect);
        var n = rotation;
        while (n > 0)
        {
            rotated = rotated.Rotate();
            n -= 90;
        }

        if (_frm == null)
        {
            _frm = new PdfTemplate(_writer);
            _frm.BoundingBox = rotated;
            _writer.AddDirectTemplateSimple(_frm, new PdfName("FRM"));
            var scale = Math.Min(Rect.Width, Rect.Height) * 0.9f;
            var x = (Rect.Width - scale) / 2;
            var y = (Rect.Height - scale) / 2;
            scale /= 100;
            if (rotation == 90)
            {
                _frm.ConcatCtm(0, 1, -1, 0, Rect.Height, 0);
            }
            else if (rotation == 180)
            {
                _frm.ConcatCtm(-1, 0, 0, -1, Rect.Width, Rect.Height);
            }
            else if (rotation == 270)
            {
                _frm.ConcatCtm(0, -1, 1, 0, 0, Rect.Width);
            }

            _frm.AddTemplate(_app[0], 0, 0);
            if (!_acro6Layers)
            {
                _frm.AddTemplate(_app[1], scale, 0, 0, scale, x, y);
            }

            _frm.AddTemplate(_app[2], 0, 0);
            if (!_acro6Layers)
            {
                _frm.AddTemplate(_app[3], scale, 0, 0, scale, x, y);
                _frm.AddTemplate(_app[4], 0, 0);
            }
        }

        var napp = new PdfTemplate(_writer);
        napp.BoundingBox = rotated;
        _writer.AddDirectTemplateSimple(napp, null);
        napp.AddTemplate(_frm, 0, 0);
        return napp;
    }

    /// <summary>
    ///     Gets the background image for the layer 2.
    /// </summary>
    /// <returns>the background image for the layer 2</returns>
    public Image GetImage() => _image;

    /// <summary>
    ///     Gets a template layer to create a signature appearance. The layers can go from 0 to 4.
    ///     Consult <A HREF="http://partners.adobe.com/asn/developer/pdfs/tn/PPKAppearances.pdf">PPKAppearances.pdf</A>
    ///     for further details.
    /// </summary>
    /// <param name="layer">the layer</param>
    /// <returns>a template</returns>
    public PdfTemplate GetLayer(int layer)
    {
        if (layer < 0 || layer >= _app.Length)
        {
            return null;
        }

        var t = _app[layer];
        if (t == null)
        {
            t = _app[layer] = new PdfTemplate(_writer);
            t.BoundingBox = Rect;
            _writer.AddDirectTemplateSimple(t, new PdfName("n" + layer));
        }

        return t;
    }

    /// <summary>
    ///     Gets a new signature fied name that doesn't clash with any existing name.
    /// </summary>
    /// <returns>a new signature fied name</returns>
    public string GetNewSigName()
    {
        var af = _writer.AcroFields;
        var name = "Signature";
        var step = 0;
        var found = false;
        while (!found)
        {
            ++step;
            var n1 = name + step;
            if (af.GetFieldItem(n1) != null)
            {
                continue;
            }

            n1 += ".";
            found = true;
            foreach (var fn in af.Fields.Keys)
            {
                if (fn.StartsWith(n1, StringComparison.Ordinal))
                {
                    found = false;
                    break;
                }
            }
        }

        name += step;
        return name;
    }

    /// <summary>
    ///     Gets the template that aggregates all appearance layers. This corresponds to the /FRM resource.
    ///     Consult <A HREF="http://partners.adobe.com/asn/developer/pdfs/tn/PPKAppearances.pdf">PPKAppearances.pdf</A>
    ///     for further details.
    /// </summary>
    /// <returns>the template that aggregates all appearance layers</returns>
    public PdfTemplate GetTopLayer()
    {
        if (_frm == null)
        {
            _frm = new PdfTemplate(_writer);
            _frm.BoundingBox = Rect;
            _writer.AddDirectTemplateSimple(_frm, new PdfName("FRM"));
        }

        return _frm;
    }

    /// <summary>
    ///     Gets the visibility status of the signature.
    /// </summary>
    /// <returns>the visibility status of the signature</returns>
    public bool IsInvisible() => Rect == null || Rect.Width.ApproxEquals(0) || Rect.Height.ApproxEquals(0);

    /// <summary>
    ///     Checks if a new field was created.
    ///     an existing field or if the signature is invisible
    /// </summary>
    /// <returns> true  if a new field was created,  false  if signing</returns>
    public bool IsNewField() => _newField;

    /// <summary>
    ///     Checks if the document is in the process of closing.
    ///     false  otherwise
    /// </summary>
    /// <returns> true  if the document is in the process of closing,</returns>
    public bool IsPreClosed() => _preClosed;

    /// <summary>
    ///     This is the first method to be called when using external signatures. The general sequence is:
    ///     PreClose(), GetDocumentBytes() and Close().
    ///     If calling PreClose() <B>dont't</B> call PdfStamper.Close().
    ///     No external signatures are allowed if this methos is called.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    public void PreClose()
    {
        PreClose(new NullValueDictionary<PdfName, int>());
    }

    /// <summary>
    ///     This is the first method to be called when using external signatures. The general sequence is:
    ///     PreClose(), GetDocumentBytes() and Close().
    ///     If calling PreClose() <B>dont't</B> call PdfStamper.Close().
    ///     If using an external signature  exclusionSizes  must contain at least
    ///     the  PdfName.CONTENTS  key with the size that it will take in the
    ///     document. Note that due to the hex string coding this size should be
    ///     byte_size*2+2.
    ///     calculation. The key is a  PdfName  and the value an
    ///     Integer . At least the  PdfName.CONTENTS  must be present
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="exclusionSizes">a  Hashtable  with names and sizes to be excluded in the signature</param>
    public void PreClose(INullValueDictionary<PdfName, int> exclusionSizes)
    {
        if (exclusionSizes == null)
        {
            throw new ArgumentNullException(nameof(exclusionSizes));
        }

        if (_preClosed)
        {
            throw new DocumentException("Document already pre closed.");
        }

        _preClosed = true;
        var af = _writer.AcroFields;
        var name = FieldName;
        var fieldExists = !(IsInvisible() || IsNewField());
        var refSig = _writer.PdfIndirectReference;
        _writer.SigFlags = 3;
        if (fieldExists)
        {
            var widget = af.GetFieldItem(name).GetWidget(0);
            _writer.MarkUsed(widget);
            widget.Put(PdfName.P, _writer.GetPageReference(Page));
            widget.Put(PdfName.V, refSig);
            var obj = PdfReader.GetPdfObjectRelease(widget.Get(PdfName.F));
            var flags = 0;
            if (obj != null && obj.IsNumber())
            {
                flags = ((PdfNumber)obj).IntValue;
            }

            flags |= PdfAnnotation.FLAGS_LOCKED;
            widget.Put(PdfName.F, new PdfNumber(flags));
            var ap = new PdfDictionary();
            ap.Put(PdfName.N, GetAppearance().IndirectReference);
            widget.Put(PdfName.Ap, ap);
        }
        else
        {
            var sigField = PdfFormField.CreateSignature(_writer);
            sigField.FieldName = name;
            sigField.Put(PdfName.V, refSig);
            sigField.Flags = PdfAnnotation.FLAGS_PRINT | PdfAnnotation.FLAGS_LOCKED;

            var pagen = Page;
            if (!IsInvisible())
            {
                sigField.SetWidget(PageRect, null);
            }
            else
            {
                sigField.SetWidget(new Rectangle(0, 0), null);
            }

            sigField.SetAppearance(PdfAnnotation.AppearanceNormal, GetAppearance());
            sigField.Page = pagen;
            _writer.AddAnnotation(sigField, pagen);
        }

        _exclusionLocations = new NullValueDictionary<PdfName, PdfLiteral>();
        if (CryptoDictionary == null)
        {
            if (PdfName.AdobePpklite.Equals(Filter))
            {
                SigStandard = new PdfSigGenericPkcs.PpkLite();
            }
            else if (PdfName.AdobePpkms.Equals(Filter))
            {
                SigStandard = new PdfSigGenericPkcs.Ppkms();
            }
            else if (PdfName.VerisignPpkvs.Equals(Filter))
            {
                SigStandard = new PdfSigGenericPkcs.VeriSign();
            }
            else
            {
                throw new ArgumentException("Unknown filter: " + Filter);
            }

            SigStandard.SetExternalDigest(_externalDigest, _externalRsAdata, _digestEncryptionAlgorithm);
            if (Reason != null)
            {
                SigStandard.Reason = Reason;
            }

            if (Location != null)
            {
                SigStandard.Location = Location;
            }

            if (Contact != null)
            {
                SigStandard.Contact = Contact;
            }

            SigStandard.Put(PdfName.M, new PdfDate(SignDate));
            SigStandard.SetSignInfo(PrivKey, CertChain, CrlList);
            var contents = (PdfString)SigStandard.Get(PdfName.Contents);
            var lit = new PdfLiteral((contents.ToString().Length + (PdfName.AdobePpklite.Equals(Filter) ? 0 : 64)) * 2 +
                                     2);
            _exclusionLocations[PdfName.Contents] = lit;
            SigStandard.Put(PdfName.Contents, lit);
            lit = new PdfLiteral(80);
            _exclusionLocations[PdfName.Byterange] = lit;
            SigStandard.Put(PdfName.Byterange, lit);
            if (CertificationLevel > 0)
            {
                addDocMdp(SigStandard);
            }

            if (_signatureEvent != null)
            {
                _signatureEvent.GetSignatureDictionary(SigStandard);
            }

            _writer.AddToBody(SigStandard, refSig, false);
        }
        else
        {
            var lit = new PdfLiteral(80);
            _exclusionLocations[PdfName.Byterange] = lit;
            CryptoDictionary.Put(PdfName.Byterange, lit);
            foreach (var entry in exclusionSizes)
            {
                var key = entry.Key;
                var v = entry.Value;
                lit = new PdfLiteral(v);
                _exclusionLocations[key] = lit;
                CryptoDictionary.Put(key, lit);
            }

            if (CertificationLevel > 0)
            {
                addDocMdp(CryptoDictionary);
            }

            if (_signatureEvent != null)
            {
                _signatureEvent.GetSignatureDictionary(CryptoDictionary);
            }

            _writer.AddToBody(CryptoDictionary, refSig, false);
        }

        if (CertificationLevel > 0)
        {
            // add DocMDP entry to root
            var docmdp = new PdfDictionary();
            docmdp.Put(new PdfName("DocMDP"), refSig);
            _writer.Reader.Catalog.Put(new PdfName("Perms"), docmdp);
        }

        _writer.Close(Stamper.MoreInfo);

        _range = new int[_exclusionLocations.Count * 2];
        var byteRangePosition = _exclusionLocations[PdfName.Byterange].Position;
        _exclusionLocations.Remove(PdfName.Byterange);
        var idx = 1;
        foreach (var lit in _exclusionLocations.Values)
        {
            var n = lit.Position;
            _range[idx++] = n;
            _range[idx++] = lit.PosLength + n;
        }

        Array.Sort(_range, 1, _range.Length - 2);
        for (var k = 3; k < _range.Length - 2; k += 2)
        {
            _range[k] -= _range[k - 1];
        }

        if (TempFile == null)
        {
            _bout = Sigout.Buffer;
            _boutLen = Sigout.Size;
            _range[_range.Length - 1] = _boutLen - _range[_range.Length - 2];
            var bf = new ByteBuffer();
            bf.Append('[');
            for (var k = 0; k < _range.Length; ++k)
            {
                bf.Append(_range[k]).Append(' ');
            }

            bf.Append(']');
            Array.Copy(bf.Buffer, 0, _bout, byteRangePosition, bf.Size);
        }
        else
        {
            try
            {
                _raf = new FileStream(TempFile, FileMode.Open, FileAccess.ReadWrite);
                var boutLen = (int)_raf.Length;
                _range[_range.Length - 1] = boutLen - _range[_range.Length - 2];
                var bf = new ByteBuffer();
                bf.Append('[');
                for (var k = 0; k < _range.Length; ++k)
                {
                    bf.Append(_range[k]).Append(' ');
                }

                bf.Append(']');
                _raf.Seek(byteRangePosition, SeekOrigin.Begin);
                _raf.Write(bf.Buffer, 0, bf.Size);
            }
            catch (IOException)
            {
                try
                {
                    _raf.Dispose();
                }
                catch
                {
                }

                try
                {
                    File.Delete(TempFile);
                }
                catch
                {
                }

                throw;
            }
        }
    }

    /// <summary>
    ///     Sets the cryptographic parameters.
    /// </summary>
    /// <param name="privKey">the private key</param>
    /// <param name="certChain">the certificate chain</param>
    /// <param name="crlList">the certificate revocation list. It may be  null </param>
    /// <param name="filter">the crytographic filter type. It can be SELF_SIGNED, VERISIGN_SIGNED or WINCER_SIGNED</param>
    public void SetCrypto(ICipherParameters privKey, X509Certificate[] certChain, object[] crlList, PdfName filter)
    {
        PrivKey = privKey;
        CertChain = certChain;
        CrlList = crlList;
        Filter = filter;
    }

    /// <summary>
    ///     Sets the digest/signature to an external calculated value.
    ///     is also  null . If the  digest  is not  null
    ///     then it may be "RSA" or "DSA"
    /// </summary>
    /// <param name="digest">the digest. This is the actual signature</param>
    /// <param name="rsAdata">the extra data that goes into the data tag in PKCS#7</param>
    /// <param name="digestEncryptionAlgorithm">the encryption algorithm. It may must be  null  if the  digest </param>
    public void SetExternalDigest(byte[] digest, byte[] rsAdata, string digestEncryptionAlgorithm)
    {
        _externalDigest = digest;
        _externalRsAdata = rsAdata;
        _digestEncryptionAlgorithm = digestEncryptionAlgorithm;
    }

    /// <summary>
    ///     Sets the signature to be visible. It creates a new visible signature field.
    /// </summary>
    /// <param name="pageRect">the position and dimension of the field in the page</param>
    /// <param name="page">the page to place the field. The fist page is 1</param>
    /// <param name="fieldName">the field name or  null  to generate automatically a new field name</param>
    public void SetVisibleSignature(Rectangle pageRect, int page, string fieldName)
    {
        if (fieldName != null)
        {
            if (fieldName.IndexOf(".", StringComparison.Ordinal) >= 0)
            {
                throw new ArgumentException("Field names cannot contain a dot.");
            }

            var af = _writer.AcroFields;
            var item = af.GetFieldItem(fieldName);
            if (item != null)
            {
                throw new ArgumentException("The field " + fieldName + " already exists.");
            }

            FieldName = fieldName;
        }

        if (page < 1 || page > _writer.Reader.NumberOfPages)
        {
            throw new ArgumentException("Invalid page number: " + page);
        }

        PageRect = new Rectangle(pageRect);
        PageRect.Normalize();
        Rect = new Rectangle(PageRect.Width, PageRect.Height);
        Page = page;
        _newField = true;
    }

    /// <summary>
    ///     Sets the signature to be visible. An empty signature field with the same name must already exist.
    /// </summary>
    /// <param name="fieldName">the existing empty signature field name</param>
    public void SetVisibleSignature(string fieldName)
    {
        var af = _writer.AcroFields;
        var item = af.GetFieldItem(fieldName);
        if (item == null)
        {
            throw new ArgumentException("The field " + fieldName + " does not exist.");
        }

        var merged = item.GetMerged(0);
        if (!PdfName.Sig.Equals(PdfReader.GetPdfObject(merged.Get(PdfName.Ft))))
        {
            throw new ArgumentException("The field " + fieldName + " is not a signature field.");
        }

        FieldName = fieldName;
        var r = merged.GetAsArray(PdfName.Rect);
        var llx = r.GetAsNumber(0).FloatValue;
        var lly = r.GetAsNumber(1).FloatValue;
        var urx = r.GetAsNumber(2).FloatValue;
        var ury = r.GetAsNumber(3).FloatValue;
        PageRect = new Rectangle(llx, lly, urx, ury);
        PageRect.Normalize();
        Page = item.GetPage(0);
        var rotation = _writer.Reader.GetPageRotation(Page);
        var pageSize = _writer.Reader.GetPageSizeWithRotation(Page);
        switch (rotation)
        {
            case 90:
                PageRect = new Rectangle(
                                         PageRect.Bottom,
                                         pageSize.Top - PageRect.Left,
                                         PageRect.Top,
                                         pageSize.Top - PageRect.Right);
                break;
            case 180:
                PageRect = new Rectangle(
                                         pageSize.Right - PageRect.Left,
                                         pageSize.Top - PageRect.Bottom,
                                         pageSize.Right - PageRect.Right,
                                         pageSize.Top - PageRect.Top);
                break;
            case 270:
                PageRect = new Rectangle(
                                         pageSize.Right - PageRect.Bottom,
                                         PageRect.Left,
                                         pageSize.Right - PageRect.Top,
                                         PageRect.Right);
                break;
        }

        if (rotation != 0)
        {
            PageRect.Normalize();
        }

        Rect = new Rectangle(PageRect.Width, PageRect.Height);
    }

    internal void SetStamper(PdfStamper stamper)
    {
        Stamper = stamper;
    }

    internal void SetTempFile(string tempFile)
    {
        TempFile = tempFile;
    }

    private void addDocMdp(PdfDictionary crypto)
    {
        var reference = new PdfDictionary();
        var transformParams = new PdfDictionary();
        transformParams.Put(PdfName.P, new PdfNumber(CertificationLevel));
        transformParams.Put(PdfName.V, new PdfName("1.2"));
        transformParams.Put(PdfName.TYPE, PdfName.Transformparams);
        reference.Put(PdfName.Transformmethod, PdfName.Docmdp);
        reference.Put(PdfName.TYPE, PdfName.Sigref);
        reference.Put(PdfName.Transformparams, transformParams);
        reference.Put(new PdfName("DigestValue"), new PdfString("aa"));
        var loc = new PdfArray();
        loc.Add(new PdfNumber(0));
        loc.Add(new PdfNumber(0));
        reference.Put(new PdfName("DigestLocation"), loc);
        reference.Put(new PdfName("DigestMethod"), new PdfName("MD5"));
        reference.Put(PdfName.Data, _writer.Reader.Trailer.Get(PdfName.Root));
        var types = new PdfArray();
        types.Add(reference);
        crypto.Put(PdfName.Reference, types);
    }

    /// <summary>
    ///     An interface to retrieve the signature dictionary for modification.
    /// </summary>
    public interface ISignatureEvent
    {
        /// <summary>
        ///     Allows modification of the signature dictionary.
        /// </summary>
        /// <param name="sig">the signature dictionary</param>
        void GetSignatureDictionary(PdfDictionary sig);
    }

    /// <summary>
    /// </summary>
    public class FRangeStream : Stream
    {
        private readonly byte[] _b = new byte[1];
        private readonly byte[] _bout;
        private readonly FileStream _raf;
        private readonly int[] _range;
        private int _rangePosition;

        internal FRangeStream(FileStream raf, byte[] bout, int[] range)
        {
            _raf = raf;
            _bout = bout;
            _range = range;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => 0;

        public override long Position
        {
            get => 0;
            set { }
        }

        public override void Flush()
        {
        }

        /// <summary>
        ///     @see java.io.Stream#read(byte[], int, int)
        /// </summary>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0 || offset > buffer.Length || count < 0 ||
                offset + count > buffer.Length || offset + count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count == 0)
            {
                return 0;
            }

            if (_rangePosition >= _range[_range.Length - 2] + _range[_range.Length - 1])
            {
                return -1;
            }

            for (var k = 0; k < _range.Length; k += 2)
            {
                var start = _range[k];
                var end = start + _range[k + 1];
                if (_rangePosition < start)
                {
                    _rangePosition = start;
                }

                if (_rangePosition >= start && _rangePosition < end)
                {
                    var lenf = Math.Min(count, end - _rangePosition);
                    if (_raf == null)
                    {
                        Array.Copy(_bout, _rangePosition, buffer, offset, lenf);
                    }
                    else
                    {
                        _raf.Seek(_rangePosition, SeekOrigin.Begin);
                        readFully(buffer, offset, lenf);
                    }

                    _rangePosition += lenf;
                    return lenf;
                }
            }

            return -1;
        }

        /// <summary>
        ///     @see java.io.Stream#read()
        /// </summary>
        public override int ReadByte()
        {
            var n = Read(_b, 0, 1);
            if (n != 1)
            {
                return -1;
            }

            return _b[0] & 0xff;
        }

        public override long Seek(long offset, SeekOrigin origin) => 0;

        public override void SetLength(long value)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        public override void WriteByte(byte value)
        {
        }

        private void readFully(byte[] b, int offset, int count)
        {
            while (count > 0)
            {
                var n = _raf.Read(b, offset, count);
                if (n <= 0)
                {
                    throw new IOException("Insufficient data.");
                }

                count -= n;
                offset += n;
            }
        }
    }
}