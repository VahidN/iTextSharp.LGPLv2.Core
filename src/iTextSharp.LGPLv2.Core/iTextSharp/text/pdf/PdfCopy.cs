using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Make copies of PDF documents. Documents can be edited after reading and
///     before writing them out.
///     @author Mark Thompson
/// </summary>
public class PdfCopy : PdfWriter
{
    /// <summary>
    ///     Holds value of property rotateContents.
    /// </summary>
    private bool _rotateContents = true;

    protected PdfIndirectReference acroForm;

    protected int CurrentObjectNum = 1;
    protected internal PdfArray FieldArray;

    protected internal INullValueDictionary<PdfTemplate, object> FieldTemplates;

    protected INullValueDictionary<PdfReader, INullValueDictionary<RefKey, IndirectReferences>> IndirectMap;

    protected INullValueDictionary<RefKey, IndirectReferences> Indirects;

    protected int[] NamePtr =
    {
        0
    };

    protected PdfReader Reader;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="document"></param>
    /// <param name="os">outputstream</param>
    public PdfCopy(Document document, Stream os) : base(new PdfDocument(), os)
    {
        if (document == null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        document.AddDocListener(Pdf);
        Pdf.AddWriter(this);
        IndirectMap = new NullValueDictionary<PdfReader, INullValueDictionary<RefKey, IndirectReferences>>();
    }

    /// <summary>
    ///     Checks if the content is automatically adjusted to compensate
    ///     the original page rotation.
    /// </summary>
    /// <returns>the auto-rotation status</returns>
    /// <summary>
    ///     Flags the content to be automatically adjusted to compensate
    ///     the original page rotation. The default is  true .
    ///     otherwise
    /// </summary>
    public bool RotateContents
    {
        set => _rotateContents = value;
        get => _rotateContents;
    }

    public override void AddAnnotation(PdfAnnotation annot)
    {
    }

    /// <summary>
    ///     Add an imported page to our output
    ///     @throws IOException, BadPdfFormatException
    /// </summary>
    /// <param name="iPage">an imported page</param>
    public void AddPage(PdfImportedPage iPage)
    {
        var pageNum = SetFromIPage(iPage);

        var thePage = Reader.GetPageN(pageNum);
        var origRef = Reader.GetPageOrigRef(pageNum);
        Reader.ReleasePage(pageNum);
        var key = new RefKey(origRef);
        PdfIndirectReference pageRef;
        var iRef = Indirects[key];

        if (iRef != null && !iRef.Copied)
        {
            PageReferences.Add(iRef.Ref);
            iRef.SetCopied();
        }

        pageRef = CurrentPage;

        if (iRef == null)
        {
            iRef = new IndirectReferences(pageRef);
            Indirects[key] = iRef;
        }

        iRef.SetCopied();
        var newPage = CopyDictionary(thePage);
        Root.AddPage(newPage);
        ++currentPageNumber;
    }

    /// <summary>
    ///     Adds a blank page.
    ///     @since	2.1.5
    /// </summary>
    /// <param name="rect">page dimension</param>
    /// <param name="rotation">rotation angle in degrees</param>
    public void AddPage(Rectangle rect, int rotation)
    {
        var mediabox = new PdfRectangle(rect, rotation);
        var resources = new PageResources();
        var page = new PdfPage(mediabox, new NullValueDictionary<string, PdfRectangle>(), resources.Resources, 0);
        page.Put(PdfName.Tabs, Tabs);
        Root.AddPage(page);
        ++currentPageNumber;
    }

    public override void Close()
    {
        if (open)
        {
            var ri = CurrentPdfReaderInstance;
            Pdf.Close();
            base.Close();

            if (ri != null)
            {
                try
                {
                    ri.Reader.Close();
                    ri.ReaderFile.Close();
                }
                catch (IOException)
                {
                    // empty on purpose
                }
            }
        }
    }

    /// <summary>
    ///     Copy the acroform for an input document. Note that you can only have one,
    ///     we make no effort to merge them.
    ///     @throws IOException, BadPdfFormatException
    /// </summary>
    /// <param name="reader">The reader of the input file that is being copied</param>
    public void CopyAcroForm(PdfReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        SetFromReader(reader);

        var catalog = reader.Catalog;
        PrIndirectReference hisRef = null;
        var o = catalog.Get(PdfName.Acroform);

        if (o != null && o.Type == PdfObject.INDIRECT)
        {
            hisRef = (PrIndirectReference)o;
        }

        if (hisRef == null)
        {
            return; // bugfix by John Engla
        }

        var key = new RefKey(hisRef);
        PdfIndirectReference myRef;
        var iRef = Indirects[key];

        if (iRef != null)
        {
            acroForm = myRef = iRef.Ref;
        }
        else
        {
            acroForm = myRef = Body.PdfIndirectReference;
            iRef = new IndirectReferences(myRef);
            Indirects[key] = iRef;
        }

        if (!iRef.Copied)
        {
            iRef.SetCopied();
            var theForm = CopyDictionary((PdfDictionary)PdfReader.GetPdfObject(hisRef));
            AddToBody(theForm, myRef);
        }
    }

    /// <summary>
    ///     Create a page stamp. New content and annotations, including new fields, are allowed.
    ///     The fields added cannot have parents in another pages. This method modifies the PdfReader instance.
    ///     The general usage to stamp something in a page is:
    ///     PdfImportedPage page = copy.getImportedPage(reader, 1);
    ///     PdfCopy.PageStamp ps = copy.createPageStamp(page);
    ///     ps.addAnnotation(PdfAnnotation.createText(copy, new Rectangle(50, 180, 70, 200), "Hello", "No Thanks", true,
    ///     "Comment"));
    ///     PdfContentByte under = ps.getUnderContent();
    ///     under.addImage(img);
    ///     PdfContentByte over = ps.getOverContent();
    ///     over.beginText();
    ///     over.setFontAndSize(bf, 18);
    ///     over.setTextMatrix(30, 30);
    ///     over.showText("total page " + totalPage);
    ///     over.endText();
    ///     ps.alterContents();
    ///     copy.addPage(page);
    /// </summary>
    /// <param name="iPage">an imported page</param>
    /// <returns>the  PageStamp </returns>
    public PageStamp CreatePageStamp(PdfImportedPage iPage)
    {
        if (iPage == null)
        {
            throw new ArgumentNullException(nameof(iPage));
        }

        var pageNum = iPage.PageNumber;
        var reader = iPage.PdfReaderInstance.Reader;
        var pageN = reader.GetPageN(pageNum);

        return new PageStamp(reader, pageN, this);
    }

    public override void FreeReader(PdfReader reader)
    {
        IndirectMap.Remove(reader);

        if (CurrentPdfReaderInstance != null)
        {
            if (CurrentPdfReaderInstance.Reader == reader)
            {
                try
                {
                    CurrentPdfReaderInstance.Reader.Close();
                    CurrentPdfReaderInstance.ReaderFile.Close();
                }
                catch (IOException)
                {
                    // empty on purpose
                }

                CurrentPdfReaderInstance = null;
            }
        }
    }

    /// <summary>
    ///     Grabs a page from the input document
    /// </summary>
    /// <param name="reader">the reader of the document</param>
    /// <param name="pageNumber">which page to get</param>
    /// <returns>the page</returns>
    public override PdfImportedPage GetImportedPage(PdfReader reader, int pageNumber)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (CurrentPdfReaderInstance != null)
        {
            if (CurrentPdfReaderInstance.Reader != reader)
            {
                try
                {
                    CurrentPdfReaderInstance.Reader.Close();
                    CurrentPdfReaderInstance.ReaderFile.Close();
                }
                catch (IOException)
                {
                    // empty on purpose
                }

                CurrentPdfReaderInstance = reader.GetPdfReaderInstance(this);
            }
        }
        else
        {
            CurrentPdfReaderInstance = reader.GetPdfReaderInstance(this);
        }

        return CurrentPdfReaderInstance.GetImportedPage(pageNumber);
    }

    /// <summary>
    ///     Signals that the  Document  was closed and that no other
    ///     Elements  will be added.
    ///     The pages-tree is built and written to the outputstream.
    ///     A Catalog is constructed, as well as an Info-object,
    ///     the referencetable is composed and everything is written
    ///     to the outputstream embedded in a Trailer.
    /// </summary>
    internal override PdfIndirectReference Add(PdfPage page, PdfContents contents)
        => null;

    /// <summary>
    ///     Translate a PRArray to a PdfArray. Also translate all of the objects contained
    ///     in it
    /// </summary>
    protected PdfArray CopyArray(PdfArray inp)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        var outp = new PdfArray();

        foreach (var value in inp.ArrayList)
        {
            outp.Add(CopyObject(value));
        }

        return outp;
    }

    /// <summary>
    ///     Translate a PRDictionary to a PdfDictionary. Also translate all of the
    ///     objects contained in it.
    /// </summary>
    protected PdfDictionary CopyDictionary(PdfDictionary inp)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        var outp = new PdfDictionary();
        var type = PdfReader.GetPdfObjectRelease(inp.Get(PdfName.TYPE));

        foreach (var key in inp.Keys)
        {
            var value = inp.Get(key);

            if (type != null && PdfName.Page.Equals(type))
            {
                if (!key.Equals(PdfName.B) && !key.Equals(PdfName.Parent))
                {
                    outp.Put(key, CopyObject(value));
                }
            }
            else
            {
                outp.Put(key, CopyObject(value));
            }
        }

        return outp;
    }

    /// <summary>
    ///     Translate a PRIndirectReference to a PdfIndirectReference
    ///     In addition, translates the object numbers, and copies the
    ///     referenced object to the output file.
    ///     NB: PRIndirectReferences (and PRIndirectObjects) really need to know what
    ///     file they came from, because each file has its own namespace. The translation
    ///     we do from their namespace to ours is *at best* heuristic, and guaranteed to
    ///     fail under some circumstances.
    /// </summary>
    protected virtual PdfIndirectReference CopyIndirect(PrIndirectReference inp)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        PdfIndirectReference theRef;
        var key = new RefKey(inp);
        var iRef = Indirects[key];

        if (iRef != null)
        {
            theRef = iRef.Ref;

            if (iRef.Copied)
            {
                return theRef;
            }
        }
        else
        {
            theRef = Body.PdfIndirectReference;
            iRef = new IndirectReferences(theRef);
            Indirects[key] = iRef;
        }

        var obj = PdfReader.GetPdfObjectRelease(inp);

        if (obj != null && obj.IsDictionary())
        {
            var type = PdfReader.GetPdfObjectRelease(((PdfDictionary)obj).Get(PdfName.TYPE));

            if (type != null && PdfName.Page.Equals(type))
            {
                return theRef;
            }
        }

        iRef.SetCopied();
        obj = CopyObject(obj);
        AddToBody(obj, theRef);

        return theRef;
    }

    /// <summary>
    ///     Translate a PR-object to a Pdf-object
    /// </summary>
    protected PdfObject CopyObject(PdfObject inp)
    {
        if (inp == null)
        {
            return PdfNull.Pdfnull;
        }

        switch (inp.Type)
        {
            case PdfObject.DICTIONARY:
                return CopyDictionary((PdfDictionary)inp);
            case PdfObject.INDIRECT:
                PdfObject obj = CopyIndirect((PrIndirectReference)inp);

                return obj ?? PdfNull.Pdfnull;
            case PdfObject.ARRAY:
                return CopyArray((PdfArray)inp);
            case PdfObject.NUMBER:
            case PdfObject.NAME:
            case PdfObject.STRING:
            case PdfObject.NULL:
            case PdfObject.BOOLEAN:
            case 0:
                return inp;
            case PdfObject.STREAM:
                return CopyStream((PrStream)inp);

            //                return in;
            default:
                if (inp.Type < 0)
                {
                    var lit = ((PdfLiteral)inp).ToString();

                    if (lit.Equals("true", StringComparison.Ordinal) || lit.Equals("false", StringComparison.Ordinal))
                    {
                        return new PdfBoolean(lit);
                    }

                    return new PdfLiteral(lit);
                }

                return null;
        }
    }

    /// <summary>
    ///     Translate a PRStream to a PdfStream. The data part copies itself.
    /// </summary>
    protected PdfStream CopyStream(PrStream inp)
    {
        if (inp == null)
        {
            throw new ArgumentNullException(nameof(inp));
        }

        var outp = new PrStream(inp, null);

        foreach (var key in inp.Keys)
        {
            var value = inp.Get(key);
            outp.Put(key, CopyObject(value));
        }

        return outp;
    }

    /// <summary>
    ///     the getCatalog method is part of PdfWriter.
    ///     we wrap this so that we can extend it
    /// </summary>
    protected override PdfDictionary GetCatalog(PdfIndirectReference rootObj)
    {
        PdfDictionary theCat = Pdf.GetCatalog(rootObj);

        if (FieldArray == null)
        {
            if (acroForm != null)
            {
                theCat.Put(PdfName.Acroform, acroForm);
            }
        }
        else
        {
            addFieldResources(theCat);
        }

        return theCat;
    }

    /// <summary>
    ///     convenience method. Given an importedpage, set our "globals"
    /// </summary>
    protected int SetFromIPage(PdfImportedPage iPage)
    {
        if (iPage == null)
        {
            throw new ArgumentNullException(nameof(iPage));
        }

        var pageNum = iPage.PageNumber;
        var inst = CurrentPdfReaderInstance = iPage.PdfReaderInstance;
        Reader = inst.Reader;
        SetFromReader(Reader);

        return pageNum;
    }

    /// <summary>
    ///     convenience method. Given a reader, set our "globals"
    /// </summary>
    protected void SetFromReader(PdfReader reader)
    {
        Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        Indirects = IndirectMap[reader];

        if (Indirects == null)
        {
            Indirects = new NullValueDictionary<RefKey, IndirectReferences>();
            IndirectMap[reader] = Indirects;
            var catalog = reader.Catalog;
            PrIndirectReference refi = null;
            var o = catalog.Get(PdfName.Acroform);

            if (o == null || o.Type != PdfObject.INDIRECT)
            {
                return;
            }

            refi = (PrIndirectReference)o;

            if (acroForm == null)
            {
                acroForm = Body.PdfIndirectReference;
            }

            Indirects[new RefKey(refi)] = new IndirectReferences(acroForm);
        }
    }

    private void addFieldResources(PdfDictionary catalog)
    {
        if (FieldArray == null)
        {
            return;
        }

        var localAcroForm = new PdfDictionary();
        catalog.Put(PdfName.Acroform, localAcroForm);
        localAcroForm.Put(PdfName.Fields, FieldArray);
        localAcroForm.Put(PdfName.Da, new PdfString("/Helv 0 Tf 0 g "));

        if (FieldTemplates.Count == 0)
        {
            return;
        }

        var dr = new PdfDictionary();
        localAcroForm.Put(PdfName.Dr, dr);

        foreach (var template in FieldTemplates.Keys)
        {
            PdfFormField.MergeResources(dr, (PdfDictionary)template.Resources);
        }

        var fonts = dr.GetAsDict(PdfName.Font);

        if (fonts == null)
        {
            fonts = new PdfDictionary();
            dr.Put(PdfName.Font, fonts);
        }

        if (!fonts.Contains(PdfName.Helv))
        {
            var dic = new PdfDictionary(PdfName.Font);
            dic.Put(PdfName.Basefont, PdfName.Helvetica);
            dic.Put(PdfName.Encoding, PdfName.WinAnsiEncoding);
            dic.Put(PdfName.Name, PdfName.Helv);
            dic.Put(PdfName.Subtype, PdfName.Type1);
            fonts.Put(PdfName.Helv, AddToBody(dic).IndirectReference);
        }

        if (!fonts.Contains(PdfName.Zadb))
        {
            var dic = new PdfDictionary(PdfName.Font);
            dic.Put(PdfName.Basefont, PdfName.Zapfdingbats);
            dic.Put(PdfName.Name, PdfName.Zadb);
            dic.Put(PdfName.Subtype, PdfName.Type1);
            fonts.Put(PdfName.Zadb, AddToBody(dic).IndirectReference);
        }
    }

    public class PageStamp
    {
        private readonly PdfCopy _cstp;
        private readonly PdfDictionary _pageN;
        private readonly PdfReader _reader;
        private StampContent _over;
        private PageResources _pageResources;
        private StampContent _under;

        internal PageStamp(PdfReader reader, PdfDictionary pageN, PdfCopy cstp)
        {
            _pageN = pageN;
            _reader = reader;
            _cstp = cstp;
        }

        public void AddAnnotation(PdfAnnotation annot)
        {
            if (annot == null)
            {
                throw new ArgumentNullException(nameof(annot));
            }

            var allAnnots = new List<PdfAnnotation>();

            if (annot.IsForm())
            {
                var field = (PdfFormField)annot;

                if (field.Parent != null)
                {
                    return;
                }

                expandFields(field, allAnnots);

                if (_cstp.FieldTemplates == null)
                {
                    _cstp.FieldTemplates = new NullValueDictionary<PdfTemplate, object>();
                }
            }
            else
            {
                allAnnots.Add(annot);
            }

            for (var k = 0; k < allAnnots.Count; ++k)
            {
                annot = allAnnots[k];

                if (annot.IsForm())
                {
                    if (!annot.IsUsed())
                    {
                        var templates = annot.Templates;

                        if (templates != null)
                        {
                            foreach (var tpl in templates.Keys)
                            {
                                _cstp.FieldTemplates[tpl] = null;
                            }
                        }
                    }

                    var field = (PdfFormField)annot;

                    if (field.Parent == null)
                    {
                        addDocumentField(field.IndirectReference);
                    }
                }

                if (annot.IsAnnotation())
                {
                    var pdfobj = PdfReader.GetPdfObject(_pageN.Get(PdfName.Annots), _pageN);
                    PdfArray annots = null;

                    if (pdfobj == null || !pdfobj.IsArray())
                    {
                        annots = new PdfArray();
                        _pageN.Put(PdfName.Annots, annots);
                    }
                    else
                    {
                        annots = (PdfArray)pdfobj;
                    }

                    annots.Add(annot.IndirectReference);

                    if (!annot.IsUsed())
                    {
                        var rect = (PdfRectangle)annot.Get(PdfName.Rect);

                        if (rect != null && (rect.Left.ApproxNotEqual(0) || rect.Right.ApproxNotEqual(0) ||
                                             rect.Top.ApproxNotEqual(0) || rect.Bottom.ApproxNotEqual(0)))
                        {
                            var rotation = PdfReader.GetPageRotation(_pageN);
                            var pageSize = PdfReader.GetPageSizeWithRotation(_pageN);

                            switch (rotation)
                            {
                                case 90:
                                    annot.Put(PdfName.Rect,
                                        new PdfRectangle(pageSize.Top - rect.Bottom, rect.Left, pageSize.Top - rect.Top,
                                            rect.Right));

                                    break;
                                case 180:
                                    annot.Put(PdfName.Rect,
                                        new PdfRectangle(pageSize.Right - rect.Left, pageSize.Top - rect.Bottom,
                                            pageSize.Right - rect.Right, pageSize.Top - rect.Top));

                                    break;
                                case 270:
                                    annot.Put(PdfName.Rect,
                                        new PdfRectangle(rect.Bottom, pageSize.Right - rect.Left, rect.Top,
                                            pageSize.Right - rect.Right));

                                    break;
                            }
                        }
                    }
                }

                if (!annot.IsUsed())
                {
                    annot.SetUsed();
                    _cstp.AddToBody(annot, annot.IndirectReference);
                }
            }
        }

        public void AlterContents()
        {
            if (_over == null && _under == null)
            {
                return;
            }

            PdfArray ar = null;
            var content = PdfReader.GetPdfObject(_pageN.Get(PdfName.Contents), _pageN);

            if (content == null)
            {
                ar = new PdfArray();
                _pageN.Put(PdfName.Contents, ar);
            }
            else if (content.IsArray())
            {
                ar = (PdfArray)content;
            }
            else if (content.IsStream())
            {
                ar = new PdfArray();
                ar.Add(_pageN.Get(PdfName.Contents));
                _pageN.Put(PdfName.Contents, ar);
            }
            else
            {
                ar = new PdfArray();
                _pageN.Put(PdfName.Contents, ar);
            }

            var outP = new ByteBuffer();

            if (_under != null)
            {
                outP.Append(PdfContents.Savestate);
                applyRotation(_pageN, outP);
                outP.Append(_under.InternalBuffer);
                outP.Append(PdfContents.Restorestate);
            }

            if (_over != null)
            {
                outP.Append(PdfContents.Savestate);
            }

            var stream = new PdfStream(outP.ToByteArray());
            stream.FlateCompress(_cstp.CompressionLevel);
            var ref1 = _cstp.AddToBody(stream).IndirectReference;
            ar.AddFirst(ref1);
            outP.Reset();

            if (_over != null)
            {
                outP.Append(' ');
                outP.Append(PdfContents.Restorestate);
                outP.Append(PdfContents.Savestate);
                applyRotation(_pageN, outP);
                outP.Append(_over.InternalBuffer);
                outP.Append(PdfContents.Restorestate);
                stream = new PdfStream(outP.ToByteArray());
                stream.FlateCompress(_cstp.CompressionLevel);
                ar.Add(_cstp.AddToBody(stream).IndirectReference);
            }

            _pageN.Put(PdfName.Resources, _pageResources.Resources);
        }

        public PdfContentByte GetOverContent()
        {
            if (_over == null)
            {
                if (_pageResources == null)
                {
                    _pageResources = new PageResources();
                    var resources = _pageN.GetAsDict(PdfName.Resources);
                    _pageResources.SetOriginalResources(resources, _cstp.NamePtr);
                }

                _over = new StampContent(_cstp, _pageResources);
            }

            return _over;
        }

        public PdfContentByte GetUnderContent()
        {
            if (_under == null)
            {
                if (_pageResources == null)
                {
                    _pageResources = new PageResources();
                    var resources = _pageN.GetAsDict(PdfName.Resources);
                    _pageResources.SetOriginalResources(resources, _cstp.NamePtr);
                }

                _under = new StampContent(_cstp, _pageResources);
            }

            return _under;
        }

        private void addDocumentField(PdfIndirectReference refi)
        {
            if (_cstp.FieldArray == null)
            {
                _cstp.FieldArray = new PdfArray();
            }

            _cstp.FieldArray.Add(refi);
        }

        private void applyRotation(PdfDictionary pageN, ByteBuffer outP)
        {
            if (!_cstp._rotateContents)
            {
                return;
            }

            var page = PdfReader.GetPageSizeWithRotation(pageN);
            var rotation = page.Rotation;

            switch (rotation)
            {
                case 90:
                    outP.Append(PdfContents.Rotate90);
                    outP.Append(page.Top);
                    outP.Append(' ').Append('0').Append(PdfContents.Rotatefinal);

                    break;
                case 180:
                    outP.Append(PdfContents.Rotate180);
                    outP.Append(page.Right);
                    outP.Append(' ');
                    outP.Append(page.Top);
                    outP.Append(PdfContents.Rotatefinal);

                    break;
                case 270:
                    outP.Append(PdfContents.Rotate270);
                    outP.Append('0').Append(' ');
                    outP.Append(page.Right);
                    outP.Append(PdfContents.Rotatefinal);

                    break;
            }
        }

        private static void expandFields(PdfFormField field, IList<PdfAnnotation> allAnnots)
        {
            allAnnots.Add(field);
            var kids = field.Kids;

            if (kids != null)
            {
                for (var k = 0; k < kids.Count; ++k)
                {
                    expandFields(kids[k], allAnnots);
                }
            }
        }
    }

    public class StampContent : PdfContentByte
    {
        private readonly PageResources _pageResources;

        /// <summary>
        ///     Creates a new instance of StampContent
        /// </summary>
        internal StampContent(PdfWriter writer, PageResources pageResources) : base(writer)
            => _pageResources = pageResources;

        /// <summary>
        ///     Gets a duplicate of this  PdfContentByte . All
        ///     the members are copied by reference but the buffer stays different.
        /// </summary>
        /// <returns>a copy of this  PdfContentByte </returns>
        public override PdfContentByte Duplicate => new StampContent(Writer, _pageResources);

        internal override PageResources PageResources => _pageResources;
    }

    /// <summary>
    ///     This class holds information about indirect references, since they are
    ///     renumbered by iText.
    /// </summary>
    public class IndirectReferences
    {
        public IndirectReferences(PdfIndirectReference refi)
        {
            Ref = refi;
            Copied = false;
        }

        public bool Copied { get; private set; }

        public PdfIndirectReference Ref { get; }

        public void SetCopied()
            => Copied = true;
    }

    /// <summary>
    ///     A key to allow us to hash indirect references
    /// </summary>
    protected class RefKey
    {
        internal readonly int Gen;
        internal readonly int Num;

        internal RefKey(int num, int gen)
        {
            Num = num;
            Gen = gen;
        }

        internal RefKey(PdfIndirectReference refi)
        {
            Num = refi.Number;
            Gen = refi.Generation;
        }

        internal RefKey(PrIndirectReference refi)
        {
            Num = refi.Number;
            Gen = refi.Generation;
        }

        public override bool Equals(object obj)
        {
            if (obj is not RefKey other)
            {
                return false;
            }

            return Num == other.Num && Gen == other.Gen;
        }

        public override int GetHashCode()
            => (Gen << 16) + Num;

        public override string ToString()
            => $"{Num} {Gen}";
    }
}