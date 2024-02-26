using System.Text;
using System.util;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.pdf.intern;
using iTextSharp.text.xml.xmp;

namespace iTextSharp.text.pdf;

public class PdfStamperImp : PdfWriter
{
    internal readonly RandomAccessFileOrArray File;
    internal readonly NullValueDictionary<int, int> MyXref = new();

    /// <summary>
    ///     Integer(page number) -> PageStamp
    /// </summary>
    internal readonly INullValueDictionary<PdfDictionary, PageStamp> PagesToContent =
        new NullValueDictionary<PdfDictionary, PageStamp>();

    internal readonly PdfReader Reader;

    internal readonly INullValueDictionary<PdfReader, RandomAccessFileOrArray> Readers2File =
        new NullValueDictionary<PdfReader, RandomAccessFileOrArray>();

    internal readonly INullValueDictionary<PdfReader, NullValueDictionary<int, int>> Readers2Intrefs =
        new NullValueDictionary<PdfReader, NullValueDictionary<int, int>>();

    protected AcroFields acroFields;
    protected internal bool Append;
    internal bool Closed;

    protected bool FieldsAdded;

    protected INullValueDictionary<PdfTemplate, object> FieldTemplates = new NullValueDictionary<PdfTemplate, object>();

    protected bool Flat;

    protected bool FlatFreeText;

    protected int InitialXrefSize;

    protected NullValueDictionary<int, int> Marked;

    protected int[] NamePtr =
    {
        0
    };

    protected PdfAction OpenAction;

    protected INullValueDictionary<string, object> PartialFlattening = new NullValueDictionary<string, object>();

    protected int sigFlags;

    protected bool UseVp;

    protected PdfViewerPreferencesImp viewerPreferences = new();

    /// <summary>
    ///     Creates new PdfStamperImp.
    ///     document
    ///     @throws DocumentException on error
    ///     @throws IOException
    /// </summary>
    /// <param name="reader">the read PDF</param>
    /// <param name="os">the output destination</param>
    /// <param name="pdfVersion">the new pdf version or '\0' to keep the same version as the original</param>
    /// <param name="append"></param>
    internal PdfStamperImp(PdfReader reader, Stream os, char pdfVersion, bool append) : base(new PdfDocument(), os)
    {
        if (!reader.IsOpenedWithFullPermissions)
        {
            throw new BadPasswordException("PdfReader not opened with owner password");
        }

        if (reader.Tampered)
        {
            throw new DocumentException("The original document was reused. Read it again from file.");
        }

        reader.Tampered = true;
        Reader = reader;
        File = reader.SafeFile;
        Append = append;

        if (append)
        {
            if (reader.IsRebuilt())
            {
                throw new DocumentException(
                    "Append mode requires a document without errors even if recovery was possible.");
            }

            if (reader.IsEncrypted())
            {
                Crypto = new PdfEncryption(reader.Decrypt);
            }

            pdf_version.SetAppendmode(true);
            File.ReOpen();
            var buf = new byte[8192];
            int n;

            while ((n = File.Read(buf)) > 0)
            {
                ((DocWriter)this).Os.Write(buf, 0, n);
            }

            File.Close();
            Prevxref = reader.LastXref;
            reader.Appendable = true;
        }
        else
        {
            if (pdfVersion == 0)
            {
                PdfVersion = reader.PdfVersion;
            }
            else
            {
                PdfVersion = pdfVersion;
            }
        }

        Open();
        Pdf.AddWriter(this);

        if (append)
        {
            Body.Refnum = reader.XrefSize;
            Marked = new NullValueDictionary<int, int>();

            if (reader.IsNewXrefType())
            {
                fullCompression = true;
            }

            if (reader.IsHybridXref())
            {
                fullCompression = false;
            }
        }

        InitialXrefSize = reader.XrefSize;
    }

    public override PdfContentByte DirectContent
        => throw new InvalidOperationException("Use PdfStamper.GetUnderContent() or PdfStamper.GetOverContent()");

    public override PdfContentByte DirectContentUnder
        => throw new InvalidOperationException("Use PdfStamper.GetUnderContent() or PdfStamper.GetOverContent()");

    /// <summary>
    ///     Always throws an  UnsupportedOperationException .
    /// </summary>
    public override int Duration
    {
        set => throw new InvalidOperationException("Use the methods at Pdfstamper.");
    }

    /// <summary>
    ///     Set the signature flags.
    /// </summary>
    public override int SigFlags
    {
        set => sigFlags |= value;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfWriter#setThumbnail(com.lowagie.text.Image)
    /// </summary>
    public override Image Thumbnail
    {
        set => throw new InvalidOperationException("Use PdfStamper.Thumbnail");
    }

    /// <summary>
    ///     Always throws an  UnsupportedOperationException .
    /// </summary>
    public override PdfTransition Transition
    {
        set => throw new InvalidOperationException("Use the methods at Pdfstamper.");
    }

    /// <summary>
    ///     Sets the viewer preferences.
    ///     @see PdfWriter#setViewerPreferences(int)
    /// </summary>
    public override int ViewerPreferences
    {
        set
        {
            UseVp = true;
            viewerPreferences.ViewerPreferences = value;
        }
    }

    internal AcroFields AcroFields
    {
        get
        {
            if (acroFields == null)
            {
                acroFields = new AcroFields(Reader, this);
            }

            return acroFields;
        }
    }

    internal bool ContentWritten => Body.Size > 1;

    internal bool FormFlattening
    {
        set => Flat = value;
    }

    internal bool FreeTextFlattening
    {
        set => FlatFreeText = value;
    }

    /// <summary>
    ///     Holds value of property rotateContents.
    /// </summary>
    internal bool RotateContents { set; get; } = true;

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfWriter#addAnnotation(com.lowagie.text.pdf.PdfAnnotation)
    /// </summary>
    public override void AddAnnotation(PdfAnnotation annot)
        => throw new NotSupportedException("Unsupported in this context. Use PdfStamper.AddAnnotation()");

    /// <summary>
    ///     @throws IOException
    /// </summary>
    /// <param name="fdf"></param>
    public void AddComments(FdfReader fdf)
    {
        if (fdf == null)
        {
            throw new ArgumentNullException(nameof(fdf));
        }

        if (Readers2Intrefs.ContainsKey(fdf))
        {
            return;
        }

        var catalog = fdf.Catalog;
        catalog = catalog.GetAsDict(PdfName.Fdf);

        if (catalog == null)
        {
            return;
        }

        var annots = catalog.GetAsArray(PdfName.Annots);

        if (annots == null || annots.Size == 0)
        {
            return;
        }

        RegisterReader(fdf, false);
        var hits = new NullValueDictionary<int, int>();
        var irt = new NullValueDictionary<string, PdfObject>();
        List<PdfObject> an = new();

        for (var k = 0; k < annots.Size; ++k)
        {
            var obj = annots[k];
            var annot = (PdfDictionary)PdfReader.GetPdfObject(obj);
            var page = annot.GetAsNumber(PdfName.Page);

            if (page == null || page.IntValue >= Reader.NumberOfPages)
            {
                continue;
            }

            FindAllObjects(fdf, obj, hits);
            an.Add(obj);

            if (obj.Type == PdfObject.INDIRECT)
            {
                var nm = PdfReader.GetPdfObject(annot.Get(PdfName.Nm));

                if (nm != null && nm.Type == PdfObject.STRING)
                {
                    irt[nm.ToString()] = obj;
                }
            }
        }

        var arhits = hits.GetKeys();

        for (var k = 0; k < arhits.Count; ++k)
        {
            var n = arhits[k];
            var obj = fdf.GetPdfObject(n);

            if (obj.Type == PdfObject.DICTIONARY)
            {
                var str = PdfReader.GetPdfObject(((PdfDictionary)obj).Get(PdfName.Irt));

                if (str != null && str.Type == PdfObject.STRING)
                {
                    var i = irt[str.ToString()];

                    if (i != null)
                    {
                        var dic2 = new PdfDictionary();
                        dic2.Merge((PdfDictionary)obj);
                        dic2.Put(PdfName.Irt, i);
                        obj = dic2;
                    }
                }
            }

            AddToBody(obj, GetNewObjectNumber(fdf, n, 0));
        }

        for (var k = 0; k < an.Count; ++k)
        {
            var obj = an[k];
            var annot = (PdfDictionary)PdfReader.GetPdfObject(obj);
            var page = annot.GetAsNumber(PdfName.Page);
            var dic = Reader.GetPageN(page.IntValue + 1);
            var annotsp = (PdfArray)PdfReader.GetPdfObject(dic.Get(PdfName.Annots), dic);

            if (annotsp == null)
            {
                annotsp = new PdfArray();
                dic.Put(PdfName.Annots, annotsp);
                MarkUsed(dic);
            }

            MarkUsed(annotsp);
            annotsp.Add(obj);
        }
    }

    /// <summary>
    ///     Adds a viewer preference
    ///     @see PdfViewerPreferences#addViewerPreference
    /// </summary>
    public override void AddViewerPreference(PdfName key, PdfObject value)
    {
        UseVp = true;
        viewerPreferences.AddViewerPreference(key, value);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfWriter#getPageReference(int)
    /// </summary>
    public override PdfIndirectReference GetPageReference(int page)
    {
        PdfIndirectReference refP = Reader.GetPageOrigRef(page);

        if (refP == null)
        {
            throw new ArgumentException("Invalid page number " + page);
        }

        return refP;
    }

    /// <summary>
    ///     Gets the PdfLayer objects in an existing document as a Map
    ///     with the names/titles of the layers as keys.
    ///     @since    2.1.2
    /// </summary>
    /// <returns>a Map with all the PdfLayers in the document (and the name/title of the layer as key)</returns>
    public INullValueDictionary<string, PdfLayer> GetPdfLayers()
    {
        if (DocumentOcg.Count == 0)
        {
            ReadOcProperties();
        }

        var map = new NullValueDictionary<string, PdfLayer>();
        string key;

        foreach (PdfLayer layer in DocumentOcg.Keys)
        {
            if (layer.Title == null)
            {
                key = layer.GetAsString(PdfName.Name).ToString();
            }
            else
            {
                key = layer.Title;
            }

            if (map.ContainsKey(key))
            {
                var seq = 2;
                var tmp = key + "(" + seq + ")";

                while (map.ContainsKey(tmp))
                {
                    seq++;
                    tmp = key + "(" + seq + ")";
                }

                key = tmp;
            }

            map[key] = layer;
        }

        return map;
    }

    /// <summary>
    ///     @throws IOException
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="openFile"></param>
    public void RegisterReader(PdfReader reader, bool openFile)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        if (Readers2Intrefs.ContainsKey(reader))
        {
            return;
        }

        Readers2Intrefs[reader] = new NullValueDictionary<int, int>();

        if (openFile)
        {
            var raf = reader.SafeFile;
            Readers2File[reader] = raf;
            raf.ReOpen();
        }
    }

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
    public override void SetAdditionalAction(PdfName actionType, PdfAction action)
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

        var aa = Reader.Catalog.GetAsDict(PdfName.Aa);

        if (aa == null)
        {
            if (action == null)
            {
                return;
            }

            aa = new PdfDictionary();
            Reader.Catalog.Put(PdfName.Aa, aa);
        }

        MarkUsed(aa);

        if (action == null)
        {
            aa.Remove(actionType);
        }
        else
        {
            aa.Put(actionType, action);
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfWriter#setOpenAction(com.lowagie.text.pdf.PdfAction)
    /// </summary>
    public override void SetOpenAction(PdfAction action)
        => OpenAction = action;

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfWriter#setOpenAction(java.lang.String)
    /// </summary>
    public override void SetOpenAction(string name)
        => throw new InvalidOperationException("Open actions by name are not supported.");

    /// <summary>
    ///     Always throws an  UnsupportedOperationException .
    ///     @throws PdfException ignore
    ///     @see PdfStamper#setPageAction(PdfName, PdfAction, int)
    /// </summary>
    /// <param name="actionType">ignore</param>
    /// <param name="action">ignore</param>
    public override void SetPageAction(PdfName actionType, PdfAction action)
        => throw new InvalidOperationException("Use SetPageAction(PdfName actionType, PdfAction action, int page)");

    /// <summary>
    /// </summary>
    /// <param name="reader"></param>
    public void UnRegisterReader(PdfReader reader)
    {
        if (!Readers2Intrefs.ContainsKey(reader))
        {
            return;
        }

        Readers2Intrefs.Remove(reader);
        var raf = Readers2File[reader];

        if (raf == null)
        {
            return;
        }

        Readers2File.Remove(reader);

        try
        {
            raf.Close();
        }
        catch
        {
        }
    }

    internal static void FindAllObjects(PdfReader reader, PdfObject obj, NullValueDictionary<int, int> hits)
    {
        if (obj == null)
        {
            return;
        }

        switch (obj.Type)
        {
            case PdfObject.INDIRECT:
                var iref = (PrIndirectReference)obj;

                if (reader != iref.Reader)
                {
                    return;
                }

                if (hits.ContainsKey(iref.Number))
                {
                    return;
                }

                hits[iref.Number] = 1;
                FindAllObjects(reader, PdfReader.GetPdfObject(obj), hits);

                return;
            case PdfObject.ARRAY:
                var a = (PdfArray)obj;

                for (var k = 0; k < a.Size; ++k)
                {
                    FindAllObjects(reader, a[k], hits);
                }

                return;
            case PdfObject.DICTIONARY:
            case PdfObject.STREAM:
                var dic = (PdfDictionary)obj;

                foreach (var name in dic.Keys)
                {
                    FindAllObjects(reader, dic.Get(name), hits);
                }

                return;
        }
    }

    internal void AddAnnotation(PdfAnnotation annot, PdfDictionary pageN)
    {
        List<PdfAnnotation> allAnnots = new();

        if (annot.IsForm())
        {
            FieldsAdded = true;
            var afdummy = AcroFields;
            var field = (PdfFormField)annot;

            if (field.Parent != null)
            {
                return;
            }

            ExpandFields(field, allAnnots);
        }
        else
        {
            allAnnots.Add(annot);
        }

        for (var k = 0; k < allAnnots.Count; ++k)
        {
            annot = allAnnots[k];

            if (annot.PlaceInPage > 0)
            {
                pageN = Reader.GetPageN(annot.PlaceInPage);
            }

            if (annot.IsForm())
            {
                if (!annot.IsUsed())
                {
                    var templates = annot.Templates;

                    if (templates != null)
                    {
                        foreach (var tpl in templates.Keys)
                        {
                            FieldTemplates[tpl] = null;
                        }
                    }
                }

                var field = (PdfFormField)annot;

                if (field.Parent == null)
                {
                    AddDocumentField(field.IndirectReference);
                }
            }

            if (annot.IsAnnotation())
            {
                var pdfobj = PdfReader.GetPdfObject(pageN.Get(PdfName.Annots), pageN);
                PdfArray annots = null;

                if (pdfobj == null || !pdfobj.IsArray())
                {
                    annots = new PdfArray();
                    pageN.Put(PdfName.Annots, annots);
                    MarkUsed(pageN);
                }
                else
                {
                    annots = (PdfArray)pdfobj;
                }

                annots.Add(annot.IndirectReference);
                MarkUsed(annots);

                if (!annot.IsUsed())
                {
                    var rect = (PdfRectangle)annot.Get(PdfName.Rect);

                    if (rect != null && (rect.Left.ApproxNotEqual(0) || rect.Right.ApproxNotEqual(0) ||
                                         rect.Top.ApproxNotEqual(0) || rect.Bottom.ApproxNotEqual(0)))
                    {
                        var rotation = PdfReader.GetPageRotation(pageN);
                        var pageSize = PdfReader.GetPageSizeWithRotation(pageN);

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
                AddToBody(annot, annot.IndirectReference);
            }
        }
    }

    internal override void AddAnnotation(PdfAnnotation annot, int page)
    {
        annot.Page = page;
        AddAnnotation(annot, Reader.GetPageN(page));
    }

    internal void AddDocumentField(PdfIndirectReference refP)
    {
        var catalog = Reader.Catalog;
        var acroForm = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Acroform), catalog);

        if (acroForm == null)
        {
            acroForm = new PdfDictionary();
            catalog.Put(PdfName.Acroform, acroForm);
            MarkUsed(catalog);
        }

        var fields = (PdfArray)PdfReader.GetPdfObject(acroForm.Get(PdfName.Fields), acroForm);

        if (fields == null)
        {
            fields = new PdfArray();
            acroForm.Put(PdfName.Fields, fields);
            MarkUsed(acroForm);
        }

        if (!acroForm.Contains(PdfName.Da))
        {
            acroForm.Put(PdfName.Da, new PdfString("/Helv 0 Tf 0 g "));
            MarkUsed(acroForm);
        }

        fields.Add(refP);
        MarkUsed(fields);
    }

    internal void AddFieldResources()
    {
        if (FieldTemplates.Count == 0)
        {
            return;
        }

        var catalog = Reader.Catalog;
        var acroForm = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Acroform), catalog);

        if (acroForm == null)
        {
            acroForm = new PdfDictionary();
            catalog.Put(PdfName.Acroform, acroForm);
            MarkUsed(catalog);
        }

        var dr = (PdfDictionary)PdfReader.GetPdfObject(acroForm.Get(PdfName.Dr), acroForm);

        if (dr == null)
        {
            dr = new PdfDictionary();
            acroForm.Put(PdfName.Dr, dr);
            MarkUsed(acroForm);
        }

        MarkUsed(dr);

        foreach (var template in FieldTemplates.Keys)
        {
            PdfFormField.MergeResources(dr, (PdfDictionary)template.Resources, this);
        }

        //            if (dr.Get(PdfName.ENCODING) == null)
        //                dr.Put(PdfName.ENCODING, PdfName.WIN_ANSI_ENCODING);
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

        if (acroForm.Get(PdfName.Da) == null)
        {
            acroForm.Put(PdfName.Da, new PdfString("/Helv 0 Tf 0 g "));
            MarkUsed(acroForm);
        }
    }

    internal void AlterContents()
    {
        foreach (var ps in PagesToContent.Values)
        {
            var pageN = ps.PageN;
            MarkUsed(pageN);
            PdfArray ar = null;
            var content = PdfReader.GetPdfObject(pageN.Get(PdfName.Contents), pageN);

            if (content == null)
            {
                ar = new PdfArray();
                pageN.Put(PdfName.Contents, ar);
            }
            else if (content.IsArray())
            {
                ar = (PdfArray)content;
                MarkUsed(ar);
            }
            else if (content.IsStream())
            {
                ar = new PdfArray();
                ar.Add(pageN.Get(PdfName.Contents));
                pageN.Put(PdfName.Contents, ar);
            }
            else
            {
                ar = new PdfArray();
                pageN.Put(PdfName.Contents, ar);
            }

            var outP = new ByteBuffer();

            if (ps.Under != null)
            {
                outP.Append(PdfContents.Savestate);
                ApplyRotation(pageN, outP);
                outP.Append(ps.Under.InternalBuffer);
                outP.Append(PdfContents.Restorestate);
            }

            if (ps.Over != null)
            {
                outP.Append(PdfContents.Savestate);
            }

            var stream = new PdfStream(outP.ToByteArray());
            stream.FlateCompress(compressionLevel);
            ar.AddFirst(AddToBody(stream).IndirectReference);
            outP.Reset();

            if (ps.Over != null)
            {
                outP.Append(' ');
                outP.Append(PdfContents.Restorestate);
                var buf = ps.Over.InternalBuffer;
                outP.Append(buf.Buffer, 0, ps.ReplacePoint);
                outP.Append(PdfContents.Savestate);
                ApplyRotation(pageN, outP);
                outP.Append(buf.Buffer, ps.ReplacePoint, buf.Size - ps.ReplacePoint);
                outP.Append(PdfContents.Restorestate);
                stream = new PdfStream(outP.ToByteArray());
                stream.FlateCompress(compressionLevel);
                ar.Add(AddToBody(stream).IndirectReference);
            }

            AlterResources(ps);
        }
    }

    internal static void AlterResources(PageStamp ps)
        => ps.PageN.Put(PdfName.Resources, ps.PageResources.Resources);

    internal void ApplyRotation(PdfDictionary pageN, ByteBuffer outP)
    {
        if (!RotateContents)
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

    internal void Close(INullValueDictionary<string, string> moreInfo)
    {
        if (Closed)
        {
            return;
        }

        if (UseVp)
        {
            Reader.SetViewerPreferences(viewerPreferences);
            MarkUsed(Reader.Trailer.Get(PdfName.Root));
        }

        if (Flat)
        {
            FlatFields();
        }

        if (FlatFreeText)
        {
            flatFreeTextFields();
        }

        AddFieldResources();
        var catalog = Reader.Catalog;
        var pages = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Pages));
        pages.Put(PdfName.Itxt, new PdfString(Document.Release));
        MarkUsed(pages);
        var acroForm = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Acroform), Reader.Catalog);

        if (acroFields != null && acroFields.Xfa.Changed)
        {
            MarkUsed(acroForm);

            if (!Flat)
            {
                acroFields.Xfa.SetXfa(this);
            }
        }

        if (sigFlags != 0)
        {
            if (acroForm != null)
            {
                acroForm.Put(PdfName.Sigflags, new PdfNumber(sigFlags));
                MarkUsed(acroForm);
                MarkUsed(catalog);
            }
        }

        Closed = true;
        AddSharedObjectsToBody();
        SetOutlines();
        SetJavaScript();
        addFileAttachments();

        // [C11] Output Intents
        if (extraCatalog != null)
        {
            catalog.MergeDifferent(extraCatalog);
        }

        if (OpenAction != null)
        {
            catalog.Put(PdfName.Openaction, OpenAction);
        }

        if (Pdf.pageLabels != null)
        {
            catalog.Put(PdfName.Pagelabels, Pdf.pageLabels.GetDictionary(this));
        }

        // OCG
        if (DocumentOcg.Count > 0)
        {
            FillOcProperties(false);
            var ocdict = catalog.GetAsDict(PdfName.Ocproperties);

            if (ocdict == null)
            {
                Reader.Catalog.Put(PdfName.Ocproperties, OcProperties);
            }
            else
            {
                ocdict.Put(PdfName.Ocgs, OcProperties.Get(PdfName.Ocgs));
                var ddict = ocdict.GetAsDict(PdfName.D);

                if (ddict == null)
                {
                    ddict = new PdfDictionary();
                    ocdict.Put(PdfName.D, ddict);
                }

                ddict.Put(PdfName.Order, OcProperties.GetAsDict(PdfName.D).Get(PdfName.Order));
                ddict.Put(PdfName.Rbgroups, OcProperties.GetAsDict(PdfName.D).Get(PdfName.Rbgroups));
                ddict.Put(PdfName.OFF, OcProperties.GetAsDict(PdfName.D).Get(PdfName.OFF));
                ddict.Put(PdfName.As, OcProperties.GetAsDict(PdfName.D).Get(PdfName.As));
            }
        }

        // metadata
        var skipInfo = -1;
        var infoObj = Reader.Trailer.Get(PdfName.Info);
        PrIndirectReference iInfo;
        PdfDictionary oldInfo;

        if (infoObj is PdfIndirectReference)
        {
            iInfo = (PrIndirectReference)infoObj;
            oldInfo = (PdfDictionary)PdfReader.GetPdfObject(iInfo);
        }
        else
        {
            iInfo = null;
            oldInfo = (PdfDictionary)infoObj;
        }

        string producer = null;

        if (iInfo != null)
        {
            skipInfo = iInfo.Number;
        }

        if (oldInfo != null && oldInfo.Get(PdfName.Producer) != null)
        {
            producer = oldInfo.GetAsString(PdfName.Producer).ToString();
        }

        if (producer == null)
        {
            producer = Document.Version;
        }
        else if (producer.IndexOf(Document.Product, StringComparison.Ordinal) == -1)
        {
            var buf = new StringBuilder(producer);
            buf.Append("; modified using ");
            buf.Append(Document.Version);
            producer = buf.ToString();
        }

        // XMP
        byte[] altMetadata = null;
        var xmpo = PdfReader.GetPdfObject(catalog.Get(PdfName.Metadata));

        if (xmpo != null && xmpo.IsStream())
        {
            altMetadata = PdfReader.GetStreamBytesRaw((PrStream)xmpo);
            PdfReader.KillIndirect(catalog.Get(PdfName.Metadata));
        }

        if (xmpMetadata != null)
        {
            altMetadata = xmpMetadata;
        }

        // if there is XMP data to add: add it
        var date = new PdfDate();

        if (altMetadata != null)
        {
            PdfStream xmp;

            try
            {
                var xmpr = new XmpReader(altMetadata);

                if (!xmpr.Replace("http://ns.adobe.com/pdf/1.3/", "Producer", producer))
                {
                    xmpr.Add("rdf:Description", "http://ns.adobe.com/pdf/1.3/", "pdf:Producer", producer);
                }

                if (!xmpr.Replace("http://ns.adobe.com/xap/1.0/", "ModifyDate", date.GetW3CDate()))
                {
                    xmpr.Add("rdf:Description", "http://ns.adobe.com/xap/1.0/", "xmp:ModifyDate", date.GetW3CDate());
                }

                xmpr.Replace("http://ns.adobe.com/xap/1.0/", "MetadataDate", date.GetW3CDate());
                xmp = new PdfStream(xmpr.SerializeDoc());
            }
            catch
            {
                xmp = new PdfStream(altMetadata);
            }

            xmp.Put(PdfName.TYPE, PdfName.Metadata);
            xmp.Put(PdfName.Subtype, PdfName.Xml);

            if (Crypto != null && !Crypto.IsMetadataEncrypted())
            {
                var ar = new PdfArray();
                ar.Add(PdfName.Crypt);
                xmp.Put(PdfName.Filter, ar);
            }

            if (Append && xmpo != null)
            {
                Body.Add(xmp, xmpo.IndRef);
            }
            else
            {
                catalog.Put(PdfName.Metadata, Body.Add(xmp).IndirectReference);
                MarkUsed(catalog);
            }
        }

        try
        {
            File.ReOpen();
            AlterContents();
            var rootN = ((PrIndirectReference)Reader.trailer.Get(PdfName.Root)).Number;

            if (Append)
            {
                var keys = Marked.GetKeys();

                for (var k = 0; k < keys.Count; ++k)
                {
                    var j = keys[k];
                    var obj = Reader.GetPdfObjectRelease(j);

                    if (obj != null && skipInfo != j && j < InitialXrefSize)
                    {
                        AddToBody(obj, j, j != rootN);
                    }
                }

                for (var k = InitialXrefSize; k < Reader.XrefSize; ++k)
                {
                    var obj = Reader.GetPdfObject(k);

                    if (obj != null)
                    {
                        AddToBody(obj, GetNewObjectNumber(Reader, k, 0));
                    }
                }
            }
            else
            {
                for (var k = 1; k < Reader.XrefSize; ++k)
                {
                    var obj = Reader.GetPdfObjectRelease(k);

                    if (obj != null && skipInfo != k)
                    {
                        AddToBody(obj, GetNewObjectNumber(Reader, k, 0), k != rootN);
                    }
                }
            }
        }
        finally
        {
            try
            {
                File.Close();
            }
            catch
            {
                // empty on purpose
            }
        }

        PdfIndirectReference encryption = null;
        PdfObject fileId = null;

        if (Crypto != null)
        {
            if (Append)
            {
                encryption = Reader.GetCryptoRef();
            }
            else
            {
                var encryptionObject = AddToBody(Crypto.GetEncryptionDictionary(), false);
                encryption = encryptionObject.IndirectReference;
            }

            fileId = Crypto.FileId;
        }
        else
        {
            fileId = PdfEncryption.CreateInfoId(PdfEncryption.CreateDocumentId());
        }

        var iRoot = (PrIndirectReference)Reader.trailer.Get(PdfName.Root);
        var root = new PdfIndirectReference(0, GetNewObjectNumber(Reader, iRoot.Number, 0));
        PdfIndirectReference info = null;
        var newInfo = new PdfDictionary();

        if (oldInfo != null)
        {
            foreach (var key in oldInfo.Keys)
            {
                var value = PdfReader.GetPdfObject(oldInfo.Get(key));
                newInfo.Put(key, value);
            }
        }

        if (moreInfo != null)
        {
            foreach (var entry in moreInfo)
            {
                var keyName = new PdfName(entry.Key);
                var value = entry.Value;

                if (value == null)
                {
                    newInfo.Remove(keyName);
                }
                else
                {
                    newInfo.Put(keyName, new PdfString(value, PdfObject.TEXT_UNICODE));
                }
            }
        }

        newInfo.Put(PdfName.Moddate, date);
        newInfo.Put(PdfName.Producer, new PdfString(producer));

        if (Append)
        {
            if (iInfo == null)
            {
                info = AddToBody(newInfo, false).IndirectReference;
            }
            else
            {
                info = AddToBody(newInfo, iInfo.Number, false).IndirectReference;
            }
        }
        else
        {
            info = AddToBody(newInfo, false).IndirectReference;
        }

        // write the cross-reference table of the body
        Body.WriteCrossReferenceTable(((DocWriter)this).Os, root, info, encryption, fileId, Prevxref);

        if (fullCompression)
        {
            var tmp = GetIsoBytes("startxref\n");
            ((DocWriter)this).Os.Write(tmp, 0, tmp.Length);
            tmp = GetIsoBytes(Body.Offset.ToString(CultureInfo.InvariantCulture));
            ((DocWriter)this).Os.Write(tmp, 0, tmp.Length);
            tmp = GetIsoBytes("\n%%EOF\n");
            ((DocWriter)this).Os.Write(tmp, 0, tmp.Length);
        }
        else
        {
            var trailer = new PdfTrailer(Body.Size, Body.Offset, root, info, encryption, fileId, Prevxref);
            trailer.ToPdf(this, ((DocWriter)this).Os);
        }

        ((DocWriter)this).Os.Flush();

        if (CloseStream)
        {
            ((DocWriter)this).Os.Dispose();
        }

        Reader.Close();
    }

    internal void CorrectAcroFieldPages(int page)
    {
        if (acroFields == null)
        {
            return;
        }

        if (page > Reader.NumberOfPages)
        {
            return;
        }

        var fields = acroFields.Fields;

        foreach (var item in fields.Values)
        {
            for (var k = 0; k < item.Size; ++k)
            {
                var p = item.GetPage(k);

                if (p >= page)
                {
                    item.ForcePage(k, p + 1);
                }
            }
        }
    }

    internal void DeleteOutlines()
    {
        var catalog = Reader.Catalog;
        var outlines = (PrIndirectReference)catalog.Get(PdfName.Outlines);

        if (outlines == null)
        {
            return;
        }

        outlineTravel(outlines);
        PdfReader.KillIndirect(outlines);
        catalog.Remove(PdfName.Outlines);
        MarkUsed(catalog);
    }

    internal void EliminateAcroformObjects()
    {
        var acro = Reader.Catalog.Get(PdfName.Acroform);

        if (acro == null)
        {
            return;
        }

        var acrodic = (PdfDictionary)PdfReader.GetPdfObject(acro);
        Reader.KillXref(acrodic.Get(PdfName.Xfa));
        acrodic.Remove(PdfName.Xfa);
        var iFields = acrodic.Get(PdfName.Fields);

        if (iFields != null)
        {
            var kids = new PdfDictionary();
            kids.Put(PdfName.Kids, iFields);
            SweepKids(kids);
            PdfReader.KillIndirect(iFields);
            acrodic.Put(PdfName.Fields, new PdfArray());
        }

        //        PdfReader.KillIndirect(acro);
        //        reader.GetCatalog().Remove(PdfName.ACROFORM);
    }

    internal static void ExpandFields(PdfFormField field, IList<PdfAnnotation> allAnnots)
    {
        allAnnots.Add(field);
        var kids = field.Kids;

        if (kids != null)
        {
            for (var k = 0; k < kids.Count; ++k)
            {
                ExpandFields(kids[k], allAnnots);
            }
        }
    }

    internal void FlatFields()
    {
        if (Append)
        {
            throw new ArgumentException("Field flattening is not supported in append mode.");
        }

        var af = AcroFields;
        var fields = acroFields.Fields;

        if (FieldsAdded && PartialFlattening.Count == 0)
        {
            foreach (var obf in fields.Keys)
            {
                PartialFlattening[obf] = null;
            }
        }

        var acroForm = Reader.Catalog.GetAsDict(PdfName.Acroform);
        PdfArray acroFds = null;

        if (acroForm != null)
        {
            acroFds = (PdfArray)PdfReader.GetPdfObject(acroForm.Get(PdfName.Fields), acroForm);
        }

        foreach (var entry in fields)
        {
            var name = entry.Key;

            if (PartialFlattening.Count != 0 && !PartialFlattening.ContainsKey(name))
            {
                continue;
            }

            var item = entry.Value;

            for (var k = 0; k < item.Size; ++k)
            {
                var merged = item.GetMerged(k);
                var ff = merged.GetAsNumber(PdfName.F);
                var flags = 0;

                if (ff != null)
                {
                    flags = ff.IntValue;
                }

                var page = item.GetPage(k);
                var appDic = merged.GetAsDict(PdfName.Ap);

                if (appDic != null && (flags & PdfAnnotation.FLAGS_PRINT) != 0 &&
                    (flags & PdfAnnotation.FLAGS_HIDDEN) == 0)
                {
                    var obj = appDic.Get(PdfName.N);
                    PdfAppearance app = null;

                    if (obj != null)
                    {
                        var objReal = PdfReader.GetPdfObject(obj);

                        if (obj is PdfIndirectReference && !obj.IsIndirect())
                        {
                            app = new PdfAppearance((PdfIndirectReference)obj);
                        }
                        else if (objReal is PdfStream)
                        {
                            ((PdfDictionary)objReal).Put(PdfName.Subtype, PdfName.Form);
                            app = new PdfAppearance((PdfIndirectReference)obj);
                        }
                        else
                        {
                            if (objReal != null && objReal.IsDictionary())
                            {
                                var asP = merged.GetAsName(PdfName.As);

                                if (asP != null)
                                {
                                    var iref = (PdfIndirectReference)((PdfDictionary)objReal).Get(asP);

                                    if (iref != null)
                                    {
                                        app = new PdfAppearance(iref);

                                        if (iref.IsIndirect())
                                        {
                                            objReal = PdfReader.GetPdfObject(iref);
                                            ((PdfDictionary)objReal).Put(PdfName.Subtype, PdfName.Form);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (app != null)
                    {
                        var box = PdfReader.GetNormalizedRectangle(merged.GetAsArray(PdfName.Rect));
                        var cb = GetOverContent(page);
                        cb.SetLiteral("Q ");
                        cb.AddTemplate(app, box.Left, box.Bottom);
                        cb.SetLiteral("q ");
                    }
                }

                if (PartialFlattening.Count == 0)
                {
                    continue;
                }

                var pageDic = Reader.GetPageN(page);
                var annots = pageDic.GetAsArray(PdfName.Annots);

                if (annots == null)
                {
                    continue;
                }

                for (var idx = 0; idx < annots.Size; ++idx)
                {
                    var ran = annots[idx];

                    if (!ran.IsIndirect())
                    {
                        continue;
                    }

                    PdfObject ran2 = item.GetWidgetRef(k);

                    if (!ran2.IsIndirect())
                    {
                        continue;
                    }

                    if (((PrIndirectReference)ran).Number == ((PrIndirectReference)ran2).Number)
                    {
                        annots.Remove(idx--);
                        var wdref = (PrIndirectReference)ran2;

                        while (true)
                        {
                            var wd = (PdfDictionary)PdfReader.GetPdfObject(wdref);
                            var parentRef = (PrIndirectReference)wd.Get(PdfName.Parent);
                            PdfReader.KillIndirect(wdref);

                            if (parentRef == null)
                            {
                                // reached AcroForm
                                for (var fr = 0; fr < acroFds.Size; ++fr)
                                {
                                    var h = acroFds[fr];

                                    if (h.IsIndirect() && ((PrIndirectReference)h).Number == wdref.Number)
                                    {
                                        acroFds.Remove(fr);
                                        --fr;
                                    }
                                }

                                break;
                            }

                            var parent = (PdfDictionary)PdfReader.GetPdfObject(parentRef);
                            var kids = parent.GetAsArray(PdfName.Kids);

                            for (var fr = 0; fr < kids.Size; ++fr)
                            {
                                var h = kids[fr];

                                if (h.IsIndirect() && ((PrIndirectReference)h).Number == wdref.Number)
                                {
                                    kids.Remove(fr);
                                    --fr;
                                }
                            }

                            if (!kids.IsEmpty())
                            {
                                break;
                            }

                            wdref = parentRef;
                        }
                    }
                }

                if (annots.IsEmpty())
                {
                    PdfReader.KillIndirect(pageDic.Get(PdfName.Annots));
                    pageDic.Remove(PdfName.Annots);
                }
            }
        }

        if (!FieldsAdded && PartialFlattening.Count == 0)
        {
            for (var page = 1; page <= Reader.NumberOfPages; ++page)
            {
                var pageDic = Reader.GetPageN(page);
                var annots = pageDic.GetAsArray(PdfName.Annots);

                if (annots == null)
                {
                    continue;
                }

                for (var idx = 0; idx < annots.Size; ++idx)
                {
                    var annoto = annots.GetDirectObject(idx);

                    if (annoto is PdfIndirectReference && !annoto.IsIndirect())
                    {
                        continue;
                    }

                    if (!annoto.IsDictionary() || PdfName.Widget.Equals(((PdfDictionary)annoto).Get(PdfName.Subtype)))
                    {
                        annots.Remove(idx);
                        --idx;
                    }
                }

                if (annots.IsEmpty())
                {
                    PdfReader.KillIndirect(pageDic.Get(PdfName.Annots));
                    pageDic.Remove(PdfName.Annots);
                }
            }

            EliminateAcroformObjects();
        }
    }

    internal PdfContentByte GetOverContent(int pageNum)
    {
        if (pageNum < 1 || pageNum > Reader.NumberOfPages)
        {
            return null;
        }

        var ps = GetPageStamp(pageNum);

        if (ps.Over == null)
        {
            ps.Over = new StampContent(this, ps);
        }

        return ps.Over;
    }

    internal PageStamp GetPageStamp(int pageNum)
    {
        var pageN = Reader.GetPageN(pageNum);
        var ps = PagesToContent[pageN];

        if (ps == null)
        {
            ps = new PageStamp(this, Reader, pageN);
            PagesToContent[pageN] = ps;
        }

        return ps;
    }

    internal override RandomAccessFileOrArray GetReaderFile(PdfReader reader)
    {
#pragma warning disable CA1854
        if (Readers2Intrefs.ContainsKey(reader))
#pragma warning restore CA1854
        {
            var raf = Readers2File[reader];

            if (raf != null)
            {
                return raf;
            }

            return reader.SafeFile;
        }

        if (CurrentPdfReaderInstance == null)
        {
            return File;
        }

        return CurrentPdfReaderInstance.ReaderFile;
    }

    internal PdfContentByte GetUnderContent(int pageNum)
    {
        if (pageNum < 1 || pageNum > Reader.NumberOfPages)
        {
            return null;
        }

        var ps = GetPageStamp(pageNum);

        if (ps.Under == null)
        {
            ps.Under = new StampContent(this, ps);
        }

        return ps.Under;
    }

    internal void InsertPage(int pageNumber, Rectangle mediabox)
    {
        var media = new Rectangle(mediabox);
        var rotation = media.Rotation % 360;
        var page = new PdfDictionary(PdfName.Page);
        var resources = new PdfDictionary();
        var procset = new PdfArray();
        procset.Add(PdfName.Pdf);
        procset.Add(PdfName.Text);
        procset.Add(PdfName.Imageb);
        procset.Add(PdfName.Imagec);
        procset.Add(PdfName.Imagei);
        resources.Put(PdfName.Procset, procset);
        page.Put(PdfName.Resources, resources);
        page.Put(PdfName.Rotate, new PdfNumber(rotation));
        page.Put(PdfName.Mediabox, new PdfRectangle(media, rotation));
        var pref = Reader.AddPdfObject(page);
        PdfDictionary parent;
        PrIndirectReference parentRef;

        if (pageNumber > Reader.NumberOfPages)
        {
            var lastPage = Reader.GetPageNRelease(Reader.NumberOfPages);
            parentRef = (PrIndirectReference)lastPage.Get(PdfName.Parent);
            parentRef = new PrIndirectReference(Reader, parentRef.Number);
            parent = (PdfDictionary)PdfReader.GetPdfObject(parentRef);
            var kids = (PdfArray)PdfReader.GetPdfObject(parent.Get(PdfName.Kids), parent);
            kids.Add(pref);
            MarkUsed(kids);
            Reader.pageRefs.InsertPage(pageNumber, pref);
        }
        else
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var firstPage = Reader.GetPageN(pageNumber);
            var firstPageRef = Reader.GetPageOrigRef(pageNumber);
            Reader.ReleasePage(pageNumber);
            parentRef = (PrIndirectReference)firstPage.Get(PdfName.Parent);
            parentRef = new PrIndirectReference(Reader, parentRef.Number);
            parent = (PdfDictionary)PdfReader.GetPdfObject(parentRef);
            var kids = (PdfArray)PdfReader.GetPdfObject(parent.Get(PdfName.Kids), parent);
            var len = kids.Size;
            var num = firstPageRef.Number;

            for (var k = 0; k < len; ++k)
            {
                var cur = (PrIndirectReference)kids[k];

                if (num == cur.Number)
                {
                    kids.Add(k, pref);

                    break;
                }
            }

            if (len == kids.Size)
            {
                throw new InvalidOperationException("Internal inconsistence.");
            }

            MarkUsed(kids);
            Reader.pageRefs.InsertPage(pageNumber, pref);
            CorrectAcroFieldPages(pageNumber);
        }

        page.Put(PdfName.Parent, parentRef);

        while (parent != null)
        {
            MarkUsed(parent);
            var count = (PdfNumber)PdfReader.GetPdfObjectRelease(parent.Get(PdfName.Count));
            parent.Put(PdfName.Count, new PdfNumber(count.IntValue + 1));
            parent = parent.GetAsDict(PdfName.Parent);
        }
    }

    /// <summary>
    ///     Getter for property append.
    /// </summary>
    /// <returns>Value of property append.</returns>
    internal bool IsAppend()
        => Append;

    /// <summary>
    ///     Adds or replaces the Collection Dictionary in the Catalog.
    /// </summary>
    /// <param name="collection">the new collection dictionary.</param>
    internal void MakePackage(PdfCollection collection)
    {
        var catalog = Reader.Catalog;
        catalog.Put(PdfName.Collection, collection);
    }

    internal bool PartialFormFlattening(string name)
    {
        var af = AcroFields;

        if (acroFields.Xfa.XfaPresent)
        {
            throw new InvalidOperationException("Partial form flattening is not supported with XFA forms.");
        }

        if (!acroFields.Fields.ContainsKey(name))
        {
            return false;
        }

        PartialFlattening[name] = null;

        return true;
    }

    internal void ReplacePage(PdfReader r, int pageImported, int pageReplaced)
    {
        var pageN = Reader.GetPageN(pageReplaced);

        if (PagesToContent.ContainsKey(pageN))
        {
            throw new InvalidOperationException("This page cannot be replaced: new content was already added");
        }

        var p = GetImportedPage(r, pageImported);
        var dic2 = Reader.GetPageNRelease(pageReplaced);
        dic2.Remove(PdfName.Resources);
        dic2.Remove(PdfName.Contents);
        moveRectangle(dic2, r, pageImported, PdfName.Mediabox, "media");
        moveRectangle(dic2, r, pageImported, PdfName.Cropbox, "crop");
        moveRectangle(dic2, r, pageImported, PdfName.Trimbox, "trim");
        moveRectangle(dic2, r, pageImported, PdfName.Artbox, "art");
        moveRectangle(dic2, r, pageImported, PdfName.Bleedbox, "bleed");
        dic2.Put(PdfName.Rotate, new PdfNumber(r.GetPageRotation(pageImported)));
        var cb = GetOverContent(pageReplaced);
        cb.AddTemplate(p, 0, 0);
        var ps = PagesToContent[pageN];
        ps.ReplacePoint = ps.Over.InternalBuffer.Size;
    }

    /// <summary>
    ///     Sets the display duration for the page (for presentations)
    /// </summary>
    /// <param name="seconds">the number of seconds to display the page. A negative value removes the entry</param>
    /// <param name="page">the page where the duration will be applied. The first page is 1</param>
    internal void SetDuration(int seconds, int page)
    {
        var pg = Reader.GetPageN(page);

        if (seconds < 0)
        {
            pg.Remove(PdfName.Dur);
        }
        else
        {
            pg.Put(PdfName.Dur, new PdfNumber(seconds));
        }

        MarkUsed(pg);
    }

    internal void SetJavaScript()
    {
        var djs = Pdf.GetDocumentLevelJs();

        if (djs.Count == 0)
        {
            return;
        }

        var catalog = Reader.Catalog;
        var names = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Names), catalog);

        if (names == null)
        {
            names = new PdfDictionary();
            catalog.Put(PdfName.Names, names);
            MarkUsed(catalog);
        }

        MarkUsed(names);
        var tree = PdfNameTree.WriteTree(djs, this);
        names.Put(PdfName.Javascript, AddToBody(tree).IndirectReference);
    }

    internal void SetOutlines()
    {
        if (NewBookmarks == null)
        {
            return;
        }

        DeleteOutlines();

        if (NewBookmarks.Count == 0)
        {
            return;
        }

        var catalog = Reader.Catalog;
        var namedAsNames = catalog.Get(PdfName.Dests) != null;
        WriteOutlines(catalog, namedAsNames);
        MarkUsed(catalog);
    }

    /// <summary>
    ///     Sets the open and close page additional action.
    ///     or  PdfWriter.PAGE_CLOSE
    ///     @throws PdfException if the action type is invalid
    /// </summary>
    /// <param name="actionType">the action type. It can be  PdfWriter.PAGE_OPEN </param>
    /// <param name="action">the action to perform</param>
    /// <param name="page">the page where the action will be applied. The first page is 1</param>
    internal void SetPageAction(PdfName actionType, PdfAction action, int page)
    {
        if (!actionType.Equals(PageOpen) && !actionType.Equals(PageClose))
        {
            throw new PdfException("Invalid page additional action type: " + actionType);
        }

        var pg = Reader.GetPageN(page);
        var aa = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.Aa), pg);

        if (aa == null)
        {
            aa = new PdfDictionary();
            pg.Put(PdfName.Aa, aa);
            MarkUsed(pg);
        }

        aa.Put(actionType, action);
        MarkUsed(aa);
    }

    internal void SetThumbnail(Image image, int page)
    {
        var thumb = GetImageReference(AddDirectImageSimple(image));
        Reader.ResetReleasePage();
        var dic = Reader.GetPageN(page);
        dic.Put(PdfName.Thumb, thumb);
        Reader.ResetReleasePage();
    }

    /// <summary>
    ///     Sets the transition for the page
    /// </summary>
    /// <param name="transition">the transition object. A  null  removes the transition</param>
    /// <param name="page">the page where the transition will be applied. The first page is 1</param>
    internal void SetTransition(PdfTransition transition, int page)
    {
        var pg = Reader.GetPageN(page);

        if (transition == null)
        {
            pg.Remove(PdfName.Trans);
        }
        else
        {
            pg.Put(PdfName.Trans, transition.TransitionDictionary);
        }

        MarkUsed(pg);
    }

    internal static void SweepKids(PdfObject obj)
    {
        var oo = PdfReader.KillIndirect(obj);

        if (oo == null || !oo.IsDictionary())
        {
            return;
        }

        var dic = (PdfDictionary)oo;
        var kids = (PdfArray)PdfReader.KillIndirect(dic.Get(PdfName.Kids));

        if (kids == null)
        {
            return;
        }

        for (var k = 0; k < kids.Size; ++k)
        {
            SweepKids(kids[k]);
        }
    }

    protected internal override int GetNewObjectNumber(PdfReader reader, int number, int generation)
    {
        var refP = Readers2Intrefs[reader];

        if (refP != null)
        {
            var n = refP[number];

            if (n == 0)
            {
                n = IndirectReferenceNumber;
                refP[number] = n;
            }

            return n;
        }

        if (CurrentPdfReaderInstance == null)
        {
            if (Append && number < InitialXrefSize)
            {
                return number;
            }

            var n = MyXref[number];

            if (n == 0)
            {
                n = IndirectReferenceNumber;
                MyXref[number] = n;
            }

            return n;
        }

        return CurrentPdfReaderInstance.GetNewObjectNumber(number, generation);
    }

    protected internal void MarkUsed(PdfObject obj)
    {
        if (Append && obj != null)
        {
            PrIndirectReference refP = null;

            if (obj.Type == PdfObject.INDIRECT)
            {
                refP = (PrIndirectReference)obj;
            }
            else
            {
                refP = obj.IndRef;
            }

            if (refP != null)
            {
                Marked[refP.Number] = 1;
            }
        }
    }

    protected internal void MarkUsed(int num)
    {
        if (Append)
        {
            Marked[num] = 1;
        }
    }

    /// <summary>
    ///     Reads the OCProperties dictionary from the catalog of the existing document
    ///     and fills the documentOCG, documentOCGorder and OCGRadioGroup variables in PdfWriter.
    ///     Note that the original OCProperties of the existing document can contain more information.
    ///     @since    2.1.2
    /// </summary>
    protected void ReadOcProperties()
    {
        if (DocumentOcg.Count != 0)
        {
            return;
        }

        var dict = Reader.Catalog.GetAsDict(PdfName.Ocproperties);

        if (dict == null)
        {
            return;
        }

        var ocgs = dict.GetAsArray(PdfName.Ocgs);
        PdfIndirectReference refi;
        PdfLayer layer;
        var ocgmap = new NullValueDictionary<string, PdfLayer>();

        for (var i = ocgs.GetListIterator(); i.HasNext();)
        {
            refi = (PdfIndirectReference)i.Next();
            layer = new PdfLayer(null);
            layer.Ref = refi;
            layer.OnPanel = false;
            layer.Merge((PdfDictionary)PdfReader.GetPdfObject(refi));
            ocgmap[refi.ToString()] = layer;
        }

        var d = dict.GetAsDict(PdfName.D);
        var off = d.GetAsArray(PdfName.OFF);

        if (off != null)
        {
            for (var i = off.GetListIterator(); i.HasNext();)
            {
                refi = (PdfIndirectReference)i.Next();
                layer = ocgmap[refi.ToString()];
                layer.On = false;
            }
        }

        var order = d.GetAsArray(PdfName.Order);

        if (order != null)
        {
            addOrder(null, order, ocgmap);
        }

        foreach (var o in ocgmap.Values)
        {
            DocumentOcg[o] = null;
        }

        OcgRadioGroup = d.GetAsArray(PdfName.Rbgroups);
        OcgLocked = d.GetAsArray(PdfName.Locked);

        if (OcgLocked == null)
        {
            OcgLocked = new PdfArray();
        }
    }

    private static void moveRectangle(PdfDictionary dic2, PdfReader r, int pageImported, PdfName key, string name)
    {
        var m = r.GetBoxSize(pageImported, name);

        if (m == null)
        {
            dic2.Remove(key);
        }
        else
        {
            dic2.Put(key, new PdfRectangle(m));
        }
    }

    private void addFileAttachments()
    {
        var fs = Pdf.GetDocumentFileAttachment();

        if (fs.Count == 0)
        {
            return;
        }

        var catalog = Reader.Catalog;
        var names = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Names), catalog);

        if (names == null)
        {
            names = new PdfDictionary();
            catalog.Put(PdfName.Names, names);
            MarkUsed(catalog);
        }

        MarkUsed(names);
        var old = PdfNameTree.ReadTree((PdfDictionary)PdfReader.GetPdfObjectRelease(names.Get(PdfName.Embeddedfiles)));

        foreach (var entry in fs)
        {
            var name = entry.Key;
            var k = 0;
            var nn = name;

            while (old.ContainsKey(nn))
            {
                ++k;
                nn += " " + k;
            }

            old[nn] = entry.Value;
        }

        var tree = PdfNameTree.WriteTree(old, this);
        names.Put(PdfName.Embeddedfiles, AddToBody(tree).IndirectReference);
    }

    /// <summary>
    ///     Recursive method to reconstruct the documentOCGorder variable in the writer.
    ///     @since    2.1.2
    /// </summary>
    /// <param name="parent">a parent PdfLayer (can be null)</param>
    /// <param name="arr">an array possibly containing children for the parent PdfLayer</param>
    /// <param name="ocgmap">a Hashtable with indirect reference Strings as keys and PdfLayer objects as values.</param>
    private void addOrder(PdfLayer parent, PdfArray arr, INullValueDictionary<string, PdfLayer> ocgmap)
    {
        PdfObject obj;
        PdfLayer layer;

        for (var i = 0; i < arr.Size; i++)
        {
            obj = arr[i];

            if (obj.IsIndirect())
            {
                layer = ocgmap[obj.ToString()];
                layer.OnPanel = true;
                RegisterLayer(layer);

                if (parent != null)
                {
                    parent.AddChild(layer);
                }

                if (arr.Size > i + 1 && arr[i + 1].IsArray())
                {
                    i++;
                    addOrder(layer, (PdfArray)arr[i], ocgmap);
                }
            }
            else if (obj.IsArray())
            {
                var sub = (PdfArray)obj;

                if (sub.IsEmpty())
                {
                    return;
                }

                obj = sub[0];

                if (obj.IsString())
                {
                    layer = new PdfLayer(obj.ToString());
                    layer.OnPanel = true;
                    RegisterLayer(layer);

                    if (parent != null)
                    {
                        parent.AddChild(layer);
                    }

                    var array = new PdfArray();

                    for (var j = sub.GetListIterator(); j.HasNext();)
                    {
                        array.Add(j.Next());
                    }

                    addOrder(layer, array, ocgmap);
                }
                else
                {
                    addOrder(parent, (PdfArray)obj, ocgmap);
                }
            }
        }
    }

    private void flatFreeTextFields()
    {
        if (Append)
        {
            throw new ArgumentException("FreeText flattening is not supported in append mode.");
        }

        for (var page = 1; page <= Reader.NumberOfPages; ++page)
        {
            var pageDic = Reader.GetPageN(page);
            var annots = pageDic.GetAsArray(PdfName.Annots);

            if (annots == null)
            {
                continue;
            }

            for (var idx = 0; idx < annots.Size; ++idx)
            {
                var annoto = annots.GetDirectObject(idx);

                if (annoto is PdfIndirectReference && !annoto.IsIndirect())
                {
                    continue;
                }

                var annDic = (PdfDictionary)annoto;

                if (!((PdfName)annDic.Get(PdfName.Subtype)).Equals(PdfName.Freetext))
                {
                    continue;
                }

                var ff = annDic.GetAsNumber(PdfName.F);
                var flags = ff != null ? ff.IntValue : 0;

                if ((flags & PdfAnnotation.FLAGS_PRINT) != 0 && (flags & PdfAnnotation.FLAGS_HIDDEN) == 0)
                {
                    var obj1 = annDic.Get(PdfName.Ap);

                    if (obj1 == null)
                    {
                        continue;
                    }

                    var appDic = obj1 is PdfIndirectReference
                        ? (PdfDictionary)PdfReader.GetPdfObject(obj1)
                        : (PdfDictionary)obj1;

                    var obj = appDic.Get(PdfName.N);
                    PdfAppearance app = null;

                    if (obj != null)
                    {
                        var objReal = PdfReader.GetPdfObject(obj);

                        if (obj is PdfIndirectReference && !obj.IsIndirect())
                        {
                            app = new PdfAppearance((PdfIndirectReference)obj);
                        }
                        else if (objReal is PdfStream)
                        {
                            ((PdfDictionary)objReal).Put(PdfName.Subtype, PdfName.Form);
                            app = new PdfAppearance((PdfIndirectReference)obj);
                        }
                        else
                        {
                            if (objReal.IsDictionary())
                            {
                                var asP = appDic.GetAsName(PdfName.As);

                                if (asP != null)
                                {
                                    var iref = (PdfIndirectReference)((PdfDictionary)objReal).Get(asP);

                                    if (iref != null)
                                    {
                                        app = new PdfAppearance(iref);

                                        if (iref.IsIndirect())
                                        {
                                            objReal = PdfReader.GetPdfObject(iref);
                                            ((PdfDictionary)objReal).Put(PdfName.Subtype, PdfName.Form);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (app != null)
                    {
                        var box = PdfReader.GetNormalizedRectangle(annDic.GetAsArray(PdfName.Rect));
                        var cb = GetOverContent(page);
                        cb.SetLiteral("Q ");
                        cb.AddTemplate(app, box.Left, box.Bottom);
                        cb.SetLiteral("q ");
                    }
                }
            }

            for (var idx = 0; idx < annots.Size; ++idx)
            {
                var annot = annots.GetAsDict(idx);

                if (annot != null)
                {
                    if (PdfName.Freetext.Equals(annot.Get(PdfName.Subtype)))
                    {
                        annots.Remove(idx);
                        --idx;
                    }
                }
            }

            if (annots.IsEmpty())
            {
                PdfReader.KillIndirect(pageDic.Get(PdfName.Annots));
                pageDic.Remove(PdfName.Annots);
            }
        }
    }

    private static void outlineTravel(PrIndirectReference outline)
    {
        while (outline != null)
        {
            var outlineR = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline);
            var first = (PrIndirectReference)outlineR.Get(PdfName.First);

            if (first != null)
            {
                outlineTravel(first);
            }

            PdfReader.KillIndirect(outlineR.Get(PdfName.Dest));
            PdfReader.KillIndirect(outlineR.Get(PdfName.A));
            PdfReader.KillIndirect(outline);
            outline = (PrIndirectReference)outlineR.Get(PdfName.Next);
        }
    }

    internal class PageStamp
    {
        internal readonly PdfDictionary PageN;
        internal readonly PageResources PageResources;
        internal StampContent Over;
        internal int ReplacePoint;
        internal StampContent Under;

        internal PageStamp(PdfStamperImp stamper, PdfReader reader, PdfDictionary pageN)
        {
            PageN = pageN;
            PageResources = new PageResources();
            var resources = pageN.GetAsDict(PdfName.Resources);
            PageResources.SetOriginalResources(resources, stamper.NamePtr);
        }
    }
}