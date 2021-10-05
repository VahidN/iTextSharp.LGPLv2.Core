using System;
using System.IO;
using System.Collections;
using System.Text;
using System.util;
using iTextSharp.text.pdf.intern;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.xml.xmp;

namespace iTextSharp.text.pdf
{
    public class PdfStamperImp : PdfWriter
    {
        internal bool Closed;
        internal RandomAccessFileOrArray File;
        internal IntHashtable MyXref = new IntHashtable();
        /// <summary>
        /// Integer(page number) -> PageStamp
        /// </summary>
        internal Hashtable PagesToContent = new Hashtable();

        internal PdfReader Reader;
        internal Hashtable Readers2File = new Hashtable();
        internal Hashtable Readers2Intrefs = new Hashtable();
        protected internal bool Append;

        protected AcroFields acroFields;

        protected bool FieldsAdded;

        protected Hashtable FieldTemplates = new Hashtable();

        protected bool Flat;

        protected bool FlatFreeText;

        protected int InitialXrefSize;

        protected IntHashtable Marked;

        protected int[] NamePtr = { 0 };

        protected PdfAction OpenAction;

        protected Hashtable PartialFlattening = new Hashtable();

        protected int sigFlags;

        protected bool UseVp;

        protected PdfViewerPreferencesImp viewerPreferences = new PdfViewerPreferencesImp();

        /// <summary>
        /// Holds value of property rotateContents.
        /// </summary>
        private bool _rotateContents = true;
        /// <summary>
        /// Creates new PdfStamperImp.
        /// document
        /// @throws DocumentException on error
        /// @throws IOException
        /// </summary>
        /// <param name="reader">the read PDF</param>
        /// <param name="os">the output destination</param>
        /// <param name="pdfVersion">the new pdf version or '\0' to keep the same version as the original</param>
        /// <param name="append"></param>
        internal PdfStamperImp(PdfReader reader, Stream os, char pdfVersion, bool append) : base(new PdfDocument(), os)
        {
            if (!reader.IsOpenedWithFullPermissions)
                throw new BadPasswordException("PdfReader not opened with owner password");
            if (reader.Tampered)
                throw new DocumentException("The original document was reused. Read it again from file.");
            reader.Tampered = true;
            Reader = reader;
            File = reader.SafeFile;
            Append = append;
            if (append)
            {
                if (reader.IsRebuilt())
                    throw new DocumentException("Append mode requires a document without errors even if recovery was possible.");
                if (reader.IsEncrypted())
                    Crypto = new PdfEncryption(reader.Decrypt);
                pdf_version.SetAppendmode(true);
                File.ReOpen();
                byte[] buf = new byte[8192];
                int n;
                while ((n = File.Read(buf)) > 0)
                    ((DocWriter)this).Os.Write(buf, 0, n);
                File.Close();
                Prevxref = reader.LastXref;
                reader.Appendable = true;
            }
            else
            {
                if (pdfVersion == 0)
                    PdfVersion = reader.PdfVersion;
                else
                    PdfVersion = pdfVersion;
            }
            Open();
            Pdf.AddWriter(this);
            if (append)
            {
                Body.Refnum = reader.XrefSize;
                Marked = new IntHashtable();
                if (reader.IsNewXrefType())
                    fullCompression = true;
                if (reader.IsHybridXref())
                    fullCompression = false;
            }
            InitialXrefSize = reader.XrefSize;
        }

        public override PdfContentByte DirectContent
        {
            get
            {
                throw new InvalidOperationException("Use PdfStamper.GetUnderContent() or PdfStamper.GetOverContent()");
            }
        }

        public override PdfContentByte DirectContentUnder
        {
            get
            {
                throw new InvalidOperationException("Use PdfStamper.GetUnderContent() or PdfStamper.GetOverContent()");
            }
        }

        /// <summary>
        /// Always throws an  UnsupportedOperationException .
        /// </summary>
        public override int Duration
        {
            set
            {
                throw new InvalidOperationException("Use the methods at Pdfstamper.");
            }
        }

        /// <summary>
        /// Set the signature flags.
        /// </summary>
        public override int SigFlags
        {
            set
            {
                sigFlags |= value;
            }
        }

        /// <summary>
        /// @see com.lowagie.text.pdf.PdfWriter#setThumbnail(com.lowagie.text.Image)
        /// </summary>
        public override Image Thumbnail
        {
            set
            {
                throw new InvalidOperationException("Use PdfStamper.Thumbnail");
            }
        }

        /// <summary>
        /// Always throws an  UnsupportedOperationException .
        /// </summary>
        public override PdfTransition Transition
        {
            set
            {
                throw new InvalidOperationException("Use the methods at Pdfstamper.");
            }
        }

        /// <summary>
        /// Sets the viewer preferences.
        /// @see PdfWriter#setViewerPreferences(int)
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

        internal bool ContentWritten
        {
            get
            {
                return Body.Size > 1;
            }
        }

        internal bool FormFlattening
        {
            set
            {
                Flat = value;
            }
        }

        internal bool FreeTextFlattening
        {
            set
            {
                FlatFreeText = value;
            }
        }

        internal bool RotateContents
        {
            set
            {
                _rotateContents = value;
            }
            get
            {
                return _rotateContents;
            }
        }

        /// <summary>
        /// @see com.lowagie.text.pdf.PdfWriter#addAnnotation(com.lowagie.text.pdf.PdfAnnotation)
        /// </summary>
        public override void AddAnnotation(PdfAnnotation annot)
        {
            throw new Exception("Unsupported in this context. Use PdfStamper.AddAnnotation()");
        }

        /// <summary>
        /// @throws IOException
        /// </summary>
        /// <param name="fdf"></param>
        public void AddComments(FdfReader fdf)
        {
            if (Readers2Intrefs.ContainsKey(fdf))
                return;
            PdfDictionary catalog = fdf.Catalog;
            catalog = catalog.GetAsDict(PdfName.Fdf);
            if (catalog == null)
                return;
            PdfArray annots = catalog.GetAsArray(PdfName.Annots);
            if (annots == null || annots.Size == 0)
                return;
            RegisterReader(fdf, false);
            IntHashtable hits = new IntHashtable();
            Hashtable irt = new Hashtable();
            ArrayList an = new ArrayList();
            for (int k = 0; k < annots.Size; ++k)
            {
                PdfObject obj = annots[k];
                PdfDictionary annot = (PdfDictionary)PdfReader.GetPdfObject(obj);
                PdfNumber page = annot.GetAsNumber(PdfName.Page);
                if (page == null || page.IntValue >= Reader.NumberOfPages)
                    continue;
                FindAllObjects(fdf, obj, hits);
                an.Add(obj);
                if (obj.Type == PdfObject.INDIRECT)
                {
                    PdfObject nm = PdfReader.GetPdfObject(annot.Get(PdfName.Nm));
                    if (nm != null && nm.Type == PdfObject.STRING)
                        irt[nm.ToString()] = obj;
                }
            }
            int[] arhits = hits.GetKeys();
            for (int k = 0; k < arhits.Length; ++k)
            {
                int n = arhits[k];
                PdfObject obj = fdf.GetPdfObject(n);
                if (obj.Type == PdfObject.DICTIONARY)
                {
                    PdfObject str = PdfReader.GetPdfObject(((PdfDictionary)obj).Get(PdfName.Irt));
                    if (str != null && str.Type == PdfObject.STRING)
                    {
                        PdfObject i = (PdfObject)irt[str.ToString()];
                        if (i != null)
                        {
                            PdfDictionary dic2 = new PdfDictionary();
                            dic2.Merge((PdfDictionary)obj);
                            dic2.Put(PdfName.Irt, i);
                            obj = dic2;
                        }
                    }
                }
                AddToBody(obj, GetNewObjectNumber(fdf, n, 0));
            }
            for (int k = 0; k < an.Count; ++k)
            {
                PdfObject obj = (PdfObject)an[k];
                PdfDictionary annot = (PdfDictionary)PdfReader.GetPdfObject(obj);
                PdfNumber page = annot.GetAsNumber(PdfName.Page);
                PdfDictionary dic = Reader.GetPageN(page.IntValue + 1);
                PdfArray annotsp = (PdfArray)PdfReader.GetPdfObject(dic.Get(PdfName.Annots), dic);
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
        /// Adds a viewer preference
        /// @see PdfViewerPreferences#addViewerPreference
        /// </summary>
        public override void AddViewerPreference(PdfName key, PdfObject value)
        {
            UseVp = true;
            viewerPreferences.AddViewerPreference(key, value);
        }

        /// <summary>
        /// @see com.lowagie.text.pdf.PdfWriter#getPageReference(int)
        /// </summary>
        public override PdfIndirectReference GetPageReference(int page)
        {
            PdfIndirectReference refP = Reader.GetPageOrigRef(page);
            if (refP == null)
                throw new ArgumentException("Invalid page number " + page);
            return refP;
        }

        /// <summary>
        /// Gets the PdfLayer objects in an existing document as a Map
        /// with the names/titles of the layers as keys.
        /// @since    2.1.2
        /// </summary>
        /// <returns>a Map with all the PdfLayers in the document (and the name/title of the layer as key)</returns>
        public Hashtable GetPdfLayers()
        {
            if (DocumentOcg.Count == 0)
            {
                ReadOcProperties();
            }
            Hashtable map = new Hashtable();
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
                    int seq = 2;
                    string tmp = key + "(" + seq + ")";
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
        /// @throws IOException
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="openFile"></param>
        public void RegisterReader(PdfReader reader, bool openFile)
        {
            if (Readers2Intrefs.ContainsKey(reader))
                return;
            Readers2Intrefs[reader] = new IntHashtable();
            if (openFile)
            {
                RandomAccessFileOrArray raf = reader.SafeFile;
                Readers2File[reader] = raf;
                raf.ReOpen();
            }
        }

        /// <summary>
        /// Additional-actions defining the actions to be taken in
        /// response to various trigger events affecting the document
        /// as a whole. The actions types allowed are:  DOCUMENT_CLOSE ,
        ///  WILL_SAVE ,  DID_SAVE ,  WILL_PRINT
        /// and  DID_PRINT .
        /// @throws PdfException on invalid action type
        /// </summary>
        /// <param name="actionType">the action type</param>
        /// <param name="action">the action to execute in response to the trigger</param>
        public override void SetAdditionalAction(PdfName actionType, PdfAction action)
        {
            if (!(actionType.Equals(DocumentClose) ||
            actionType.Equals(WillSave) ||
            actionType.Equals(DidSave) ||
            actionType.Equals(WillPrint) ||
            actionType.Equals(DidPrint)))
            {
                throw new PdfException("Invalid additional action type: " + actionType);
            }
            PdfDictionary aa = Reader.Catalog.GetAsDict(PdfName.Aa);
            if (aa == null)
            {
                if (action == null)
                    return;
                aa = new PdfDictionary();
                Reader.Catalog.Put(PdfName.Aa, aa);
            }
            MarkUsed(aa);
            if (action == null)
                aa.Remove(actionType);
            else
                aa.Put(actionType, action);
        }

        /// <summary>
        /// @see com.lowagie.text.pdf.PdfWriter#setOpenAction(com.lowagie.text.pdf.PdfAction)
        /// </summary>
        public override void SetOpenAction(PdfAction action)
        {
            OpenAction = action;
        }

        /// <summary>
        /// @see com.lowagie.text.pdf.PdfWriter#setOpenAction(java.lang.String)
        /// </summary>
        public override void SetOpenAction(string name)
        {
            throw new InvalidOperationException("Open actions by name are not supported.");
        }

        /// <summary>
        /// Always throws an  UnsupportedOperationException .
        /// @throws PdfException ignore
        /// @see PdfStamper#setPageAction(PdfName, PdfAction, int)
        /// </summary>
        /// <param name="actionType">ignore</param>
        /// <param name="action">ignore</param>
        public override void SetPageAction(PdfName actionType, PdfAction action)
        {
            throw new InvalidOperationException("Use SetPageAction(PdfName actionType, PdfAction action, int page)");
        }

        /// <summary>
        /// </summary>
        /// <param name="reader"></param>
        public void UnRegisterReader(PdfReader reader)
        {
            if (!Readers2Intrefs.ContainsKey(reader))
                return;
            Readers2Intrefs.Remove(reader);
            RandomAccessFileOrArray raf = (RandomAccessFileOrArray)Readers2File[reader];
            if (raf == null)
                return;
            Readers2File.Remove(reader);
            try { raf.Close(); } catch { }
        }

        internal static void FindAllObjects(PdfReader reader, PdfObject obj, IntHashtable hits)
        {
            if (obj == null)
                return;
            switch (obj.Type)
            {
                case PdfObject.INDIRECT:
                    PrIndirectReference iref = (PrIndirectReference)obj;
                    if (reader != iref.Reader)
                        return;
                    if (hits.ContainsKey(iref.Number))
                        return;
                    hits[iref.Number] = 1;
                    FindAllObjects(reader, PdfReader.GetPdfObject(obj), hits);
                    return;
                case PdfObject.ARRAY:
                    PdfArray a = (PdfArray)obj;
                    for (int k = 0; k < a.Size; ++k)
                    {
                        FindAllObjects(reader, a[k], hits);
                    }
                    return;
                case PdfObject.DICTIONARY:
                case PdfObject.STREAM:
                    PdfDictionary dic = (PdfDictionary)obj;
                    foreach (PdfName name in dic.Keys)
                    {
                        FindAllObjects(reader, dic.Get(name), hits);
                    }
                    return;
            }
        }

        internal void AddAnnotation(PdfAnnotation annot, PdfDictionary pageN)
        {
            ArrayList allAnnots = new ArrayList();
            if (annot.IsForm())
            {
                FieldsAdded = true;
                AcroFields afdummy = AcroFields;
                PdfFormField field = (PdfFormField)annot;
                if (field.Parent != null)
                    return;
                ExpandFields(field, allAnnots);
            }
            else
                allAnnots.Add(annot);
            for (int k = 0; k < allAnnots.Count; ++k)
            {
                annot = (PdfAnnotation)allAnnots[k];
                if (annot.PlaceInPage > 0)
                    pageN = Reader.GetPageN(annot.PlaceInPage);
                if (annot.IsForm())
                {
                    if (!annot.IsUsed())
                    {
                        Hashtable templates = annot.Templates;
                        if (templates != null)
                        {
                            foreach (object tpl in templates.Keys)
                            {
                                FieldTemplates[tpl] = null;
                            }
                        }
                    }
                    PdfFormField field = (PdfFormField)annot;
                    if (field.Parent == null)
                        AddDocumentField(field.IndirectReference);
                }
                if (annot.IsAnnotation())
                {
                    PdfObject pdfobj = PdfReader.GetPdfObject(pageN.Get(PdfName.Annots), pageN);
                    PdfArray annots = null;
                    if (pdfobj == null || !pdfobj.IsArray())
                    {
                        annots = new PdfArray();
                        pageN.Put(PdfName.Annots, annots);
                        MarkUsed(pageN);
                    }
                    else
                        annots = (PdfArray)pdfobj;
                    annots.Add(annot.IndirectReference);
                    MarkUsed(annots);
                    if (!annot.IsUsed())
                    {
                        PdfRectangle rect = (PdfRectangle)annot.Get(PdfName.Rect);
                        if (rect != null && (rect.Left.ApproxNotEqual(0) || rect.Right.ApproxNotEqual(0) || rect.Top.ApproxNotEqual(0) || rect.Bottom.ApproxNotEqual(0)))
                        {
                            int rotation = Reader.GetPageRotation(pageN);
                            Rectangle pageSize = Reader.GetPageSizeWithRotation(pageN);
                            switch (rotation)
                            {
                                case 90:
                                    annot.Put(PdfName.Rect, new PdfRectangle(
                                        pageSize.Top - rect.Bottom,
                                        rect.Left,
                                        pageSize.Top - rect.Top,
                                        rect.Right));
                                    break;
                                case 180:
                                    annot.Put(PdfName.Rect, new PdfRectangle(
                                        pageSize.Right - rect.Left,
                                        pageSize.Top - rect.Bottom,
                                        pageSize.Right - rect.Right,
                                        pageSize.Top - rect.Top));
                                    break;
                                case 270:
                                    annot.Put(PdfName.Rect, new PdfRectangle(
                                        rect.Bottom,
                                        pageSize.Right - rect.Left,
                                        rect.Top,
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
            PdfDictionary catalog = Reader.Catalog;
            PdfDictionary acroForm = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Acroform), catalog);
            if (acroForm == null)
            {
                acroForm = new PdfDictionary();
                catalog.Put(PdfName.Acroform, acroForm);
                MarkUsed(catalog);
            }
            PdfArray fields = (PdfArray)PdfReader.GetPdfObject(acroForm.Get(PdfName.Fields), acroForm);
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
                return;
            PdfDictionary catalog = Reader.Catalog;
            PdfDictionary acroForm = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Acroform), catalog);
            if (acroForm == null)
            {
                acroForm = new PdfDictionary();
                catalog.Put(PdfName.Acroform, acroForm);
                MarkUsed(catalog);
            }
            PdfDictionary dr = (PdfDictionary)PdfReader.GetPdfObject(acroForm.Get(PdfName.Dr), acroForm);
            if (dr == null)
            {
                dr = new PdfDictionary();
                acroForm.Put(PdfName.Dr, dr);
                MarkUsed(acroForm);
            }
            MarkUsed(dr);
            foreach (PdfTemplate template in FieldTemplates.Keys)
            {
                PdfFormField.MergeResources(dr, (PdfDictionary)template.Resources, this);
            }
            //            if (dr.Get(PdfName.ENCODING) == null)
            //                dr.Put(PdfName.ENCODING, PdfName.WIN_ANSI_ENCODING);
            PdfDictionary fonts = dr.GetAsDict(PdfName.Font);
            if (fonts == null)
            {
                fonts = new PdfDictionary();
                dr.Put(PdfName.Font, fonts);
            }
            if (!fonts.Contains(PdfName.Helv))
            {
                PdfDictionary dic = new PdfDictionary(PdfName.Font);
                dic.Put(PdfName.Basefont, PdfName.Helvetica);
                dic.Put(PdfName.Encoding, PdfName.WinAnsiEncoding);
                dic.Put(PdfName.Name, PdfName.Helv);
                dic.Put(PdfName.Subtype, PdfName.Type1);
                fonts.Put(PdfName.Helv, AddToBody(dic).IndirectReference);
            }
            if (!fonts.Contains(PdfName.Zadb))
            {
                PdfDictionary dic = new PdfDictionary(PdfName.Font);
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
            foreach (PageStamp ps in PagesToContent.Values)
            {
                PdfDictionary pageN = ps.PageN;
                MarkUsed(pageN);
                PdfArray ar = null;
                PdfObject content = PdfReader.GetPdfObject(pageN.Get(PdfName.Contents), pageN);
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
                ByteBuffer outP = new ByteBuffer();
                if (ps.Under != null)
                {
                    outP.Append(PdfContents.Savestate);
                    ApplyRotation(pageN, outP);
                    outP.Append(ps.Under.InternalBuffer);
                    outP.Append(PdfContents.Restorestate);
                }
                if (ps.Over != null)
                    outP.Append(PdfContents.Savestate);
                PdfStream stream = new PdfStream(outP.ToByteArray());
                stream.FlateCompress(compressionLevel);
                ar.AddFirst(AddToBody(stream).IndirectReference);
                outP.Reset();
                if (ps.Over != null)
                {
                    outP.Append(' ');
                    outP.Append(PdfContents.Restorestate);
                    ByteBuffer buf = ps.Over.InternalBuffer;
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

        internal void AlterResources(PageStamp ps)
        {
            ps.PageN.Put(PdfName.Resources, ps.PageResources.Resources);
        }

        internal void ApplyRotation(PdfDictionary pageN, ByteBuffer outP)
        {
            if (!_rotateContents)
                return;
            Rectangle page = Reader.GetPageSizeWithRotation(pageN);
            int rotation = page.Rotation;
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

        internal void Close(Hashtable moreInfo)
        {
            if (Closed)
                return;
            if (UseVp)
            {
                Reader.SetViewerPreferences(viewerPreferences);
                MarkUsed(Reader.Trailer.Get(PdfName.Root));
            }
            if (Flat)
                FlatFields();
            if (FlatFreeText)
                flatFreeTextFields();
            AddFieldResources();
            PdfDictionary catalog = Reader.Catalog;
            PdfDictionary pages = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Pages));
            pages.Put(PdfName.Itxt, new PdfString(Document.Release));
            MarkUsed(pages);
            PdfDictionary acroForm = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Acroform), Reader.Catalog);
            if (acroFields != null && acroFields.Xfa.Changed)
            {
                MarkUsed(acroForm);
                if (!Flat)
                    acroFields.Xfa.SetXfa(this);
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
            if (OpenAction != null)
            {
                catalog.Put(PdfName.Openaction, OpenAction);
            }
            if (Pdf.pageLabels != null)
                catalog.Put(PdfName.Pagelabels, Pdf.pageLabels.GetDictionary(this));
            // OCG
            if (DocumentOcg.Count > 0)
            {
                FillOcProperties(false);
                PdfDictionary ocdict = catalog.GetAsDict(PdfName.Ocproperties);
                if (ocdict == null)
                {
                    Reader.Catalog.Put(PdfName.Ocproperties, OcProperties);
                }
                else
                {
                    ocdict.Put(PdfName.Ocgs, OcProperties.Get(PdfName.Ocgs));
                    PdfDictionary ddict = ocdict.GetAsDict(PdfName.D);
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
            int skipInfo = -1;
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
                skipInfo = iInfo.Number;
            if (oldInfo != null && oldInfo.Get(PdfName.Producer) != null)
                producer = oldInfo.GetAsString(PdfName.Producer).ToString();
            if (producer == null)
            {
                producer = Document.Version;
            }
            else if (producer.IndexOf(Document.Product, StringComparison.Ordinal) == -1)
            {
                StringBuilder buf = new StringBuilder(producer);
                buf.Append("; modified using ");
                buf.Append(Document.Version);
                producer = buf.ToString();
            }
            // XMP
            byte[] altMetadata = null;
            PdfObject xmpo = PdfReader.GetPdfObject(catalog.Get(PdfName.Metadata));
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
            PdfDate date = new PdfDate();
            if (altMetadata != null)
            {
                PdfStream xmp;
                try
                {
                    XmpReader xmpr = new XmpReader(altMetadata);
                    if (!xmpr.Replace("http://ns.adobe.com/pdf/1.3/", "Producer", producer))
                        xmpr.Add("rdf:Description", "http://ns.adobe.com/pdf/1.3/", "pdf:Producer", producer);
                    if (!xmpr.Replace("http://ns.adobe.com/xap/1.0/", "ModifyDate", date.GetW3CDate()))
                        xmpr.Add("rdf:Description", "http://ns.adobe.com/xap/1.0/", "xmp:ModifyDate", date.GetW3CDate());
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
                    PdfArray ar = new PdfArray();
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
                int rootN = ((PrIndirectReference)Reader.trailer.Get(PdfName.Root)).Number;
                if (Append)
                {
                    int[] keys = Marked.GetKeys();
                    for (int k = 0; k < keys.Length; ++k)
                    {
                        int j = keys[k];
                        PdfObject obj = Reader.GetPdfObjectRelease(j);
                        if (obj != null && skipInfo != j && j < InitialXrefSize)
                        {
                            AddToBody(obj, j, j != rootN);
                        }
                    }
                    for (int k = InitialXrefSize; k < Reader.XrefSize; ++k)
                    {
                        PdfObject obj = Reader.GetPdfObject(k);
                        if (obj != null)
                        {
                            AddToBody(obj, GetNewObjectNumber(Reader, k, 0));
                        }
                    }
                }
                else
                {
                    for (int k = 1; k < Reader.XrefSize; ++k)
                    {
                        PdfObject obj = Reader.GetPdfObjectRelease(k);
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
                    PdfIndirectObject encryptionObject = AddToBody(Crypto.GetEncryptionDictionary(), false);
                    encryption = encryptionObject.IndirectReference;
                }
                fileId = Crypto.FileId;
            }
            else
                fileId = PdfEncryption.CreateInfoId(PdfEncryption.CreateDocumentId());
            PrIndirectReference iRoot = (PrIndirectReference)Reader.trailer.Get(PdfName.Root);
            PdfIndirectReference root = new PdfIndirectReference(0, GetNewObjectNumber(Reader, iRoot.Number, 0));
            PdfIndirectReference info = null;
            PdfDictionary newInfo = new PdfDictionary();
            if (oldInfo != null)
            {
                foreach (PdfName key in oldInfo.Keys)
                {
                    PdfObject value = PdfReader.GetPdfObject(oldInfo.Get(key));
                    newInfo.Put(key, value);
                }
            }
            if (moreInfo != null)
            {
                foreach (DictionaryEntry entry in moreInfo)
                {
                    PdfName keyName = new PdfName((string)entry.Key);
                    string value = (string)entry.Value;
                    if (value == null)
                        newInfo.Remove(keyName);
                    else
                        newInfo.Put(keyName, new PdfString(value, PdfObject.TEXT_UNICODE));
                }
            }
            newInfo.Put(PdfName.Moddate, date);
            newInfo.Put(PdfName.Producer, new PdfString(producer));
            if (Append)
            {
                if (iInfo == null)
                    info = AddToBody(newInfo, false).IndirectReference;
                else
                    info = AddToBody(newInfo, iInfo.Number, false).IndirectReference;
            }
            else
            {
                info = AddToBody(newInfo, false).IndirectReference;
            }
            // write the cross-reference table of the body
            Body.WriteCrossReferenceTable(((DocWriter)this).Os, root, info, encryption, fileId, Prevxref);
            if (fullCompression)
            {
                byte[] tmp = GetIsoBytes("startxref\n");
                ((DocWriter)this).Os.Write(tmp, 0, tmp.Length);
                tmp = GetIsoBytes(Body.Offset.ToString());
                ((DocWriter)this).Os.Write(tmp, 0, tmp.Length);
                tmp = GetIsoBytes("\n%%EOF\n");
                ((DocWriter)this).Os.Write(tmp, 0, tmp.Length);
            }
            else
            {
                PdfTrailer trailer = new PdfTrailer(Body.Size,
                Body.Offset,
                root,
                info,
                encryption,
                fileId, Prevxref);
                trailer.ToPdf(this, ((DocWriter)this).Os);
            }
            ((DocWriter)this).Os.Flush();
            if (CloseStream)
                ((DocWriter)this).Os.Dispose();
            Reader.Close();
        }
        internal void CorrectAcroFieldPages(int page)
        {
            if (acroFields == null)
                return;
            if (page > Reader.NumberOfPages)
                return;
            Hashtable fields = acroFields.Fields;
            foreach (AcroFields.Item item in fields.Values)
            {
                for (int k = 0; k < item.Size; ++k)
                {
                    int p = item.GetPage(k);
                    if (p >= page)
                        item.ForcePage(k, p + 1);
                }
            }
        }

        internal void DeleteOutlines()
        {
            PdfDictionary catalog = Reader.Catalog;
            PrIndirectReference outlines = (PrIndirectReference)catalog.Get(PdfName.Outlines);
            if (outlines == null)
                return;
            outlineTravel(outlines);
            PdfReader.KillIndirect(outlines);
            catalog.Remove(PdfName.Outlines);
            MarkUsed(catalog);
        }

        internal void EliminateAcroformObjects()
        {
            PdfObject acro = Reader.Catalog.Get(PdfName.Acroform);
            if (acro == null)
                return;
            PdfDictionary acrodic = (PdfDictionary)PdfReader.GetPdfObject(acro);
            Reader.KillXref(acrodic.Get(PdfName.Xfa));
            acrodic.Remove(PdfName.Xfa);
            PdfObject iFields = acrodic.Get(PdfName.Fields);
            if (iFields != null)
            {
                PdfDictionary kids = new PdfDictionary();
                kids.Put(PdfName.Kids, iFields);
                SweepKids(kids);
                PdfReader.KillIndirect(iFields);
                acrodic.Put(PdfName.Fields, new PdfArray());
            }
            //        PdfReader.KillIndirect(acro);
            //        reader.GetCatalog().Remove(PdfName.ACROFORM);
        }

        internal void ExpandFields(PdfFormField field, ArrayList allAnnots)
        {
            allAnnots.Add(field);
            ArrayList kids = field.Kids;
            if (kids != null)
            {
                for (int k = 0; k < kids.Count; ++k)
                {
                    ExpandFields((PdfFormField)kids[k], allAnnots);
                }
            }
        }

        internal void FlatFields()
        {
            if (Append)
                throw new ArgumentException("Field flattening is not supported in append mode.");
            AcroFields af = AcroFields;
            Hashtable fields = acroFields.Fields;
            if (FieldsAdded && PartialFlattening.Count == 0)
            {
                foreach (object obf in fields.Keys)
                {
                    PartialFlattening[obf] = null;
                }
            }
            PdfDictionary acroForm = Reader.Catalog.GetAsDict(PdfName.Acroform);
            PdfArray acroFds = null;
            if (acroForm != null)
            {
                acroFds = (PdfArray)PdfReader.GetPdfObject(acroForm.Get(PdfName.Fields), acroForm);
            }
            foreach (DictionaryEntry entry in fields)
            {
                string name = (string)entry.Key;
                if (PartialFlattening.Count != 0 && !PartialFlattening.ContainsKey(name))
                    continue;
                AcroFields.Item item = (AcroFields.Item)entry.Value;
                for (int k = 0; k < item.Size; ++k)
                {
                    PdfDictionary merged = item.GetMerged(k);
                    PdfNumber ff = merged.GetAsNumber(PdfName.F);
                    int flags = 0;
                    if (ff != null)
                        flags = ff.IntValue;
                    int page = item.GetPage(k);
                    PdfDictionary appDic = merged.GetAsDict(PdfName.Ap);
                    if (appDic != null && (flags & PdfAnnotation.FLAGS_PRINT) != 0 && (flags & PdfAnnotation.FLAGS_HIDDEN) == 0)
                    {
                        PdfObject obj = appDic.Get(PdfName.N);
                        PdfAppearance app = null;
                        if (obj != null)
                        {
                            PdfObject objReal = PdfReader.GetPdfObject(obj);
                            if (obj is PdfIndirectReference && !obj.IsIndirect())
                                app = new PdfAppearance((PdfIndirectReference)obj);
                            else if (objReal is PdfStream)
                            {
                                ((PdfDictionary)objReal).Put(PdfName.Subtype, PdfName.Form);
                                app = new PdfAppearance((PdfIndirectReference)obj);
                            }
                            else
                            {
                                if (objReal != null && objReal.IsDictionary())
                                {
                                    PdfName asP = merged.GetAsName(PdfName.As);
                                    if (asP != null)
                                    {
                                        PdfIndirectReference iref = (PdfIndirectReference)((PdfDictionary)objReal).Get(asP);
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
                            Rectangle box = PdfReader.GetNormalizedRectangle(merged.GetAsArray(PdfName.Rect));
                            PdfContentByte cb = GetOverContent(page);
                            cb.SetLiteral("Q ");
                            cb.AddTemplate(app, box.Left, box.Bottom);
                            cb.SetLiteral("q ");
                        }
                    }
                    if (PartialFlattening.Count == 0)
                        continue;
                    PdfDictionary pageDic = Reader.GetPageN(page);
                    PdfArray annots = pageDic.GetAsArray(PdfName.Annots);
                    if (annots == null)
                        continue;
                    for (int idx = 0; idx < annots.Size; ++idx)
                    {
                        PdfObject ran = annots[idx];
                        if (!ran.IsIndirect())
                            continue;
                        PdfObject ran2 = item.GetWidgetRef(k);
                        if (!ran2.IsIndirect())
                            continue;
                        if (((PrIndirectReference)ran).Number == ((PrIndirectReference)ran2).Number)
                        {
                            annots.Remove(idx--);
                            PrIndirectReference wdref = (PrIndirectReference)ran2;
                            while (true)
                            {
                                PdfDictionary wd = (PdfDictionary)PdfReader.GetPdfObject(wdref);
                                PrIndirectReference parentRef = (PrIndirectReference)wd.Get(PdfName.Parent);
                                PdfReader.KillIndirect(wdref);
                                if (parentRef == null)
                                { // reached AcroForm
                                    for (int fr = 0; fr < acroFds.Size; ++fr)
                                    {
                                        PdfObject h = acroFds[fr];
                                        if (h.IsIndirect() && ((PrIndirectReference)h).Number == wdref.Number)
                                        {
                                            acroFds.Remove(fr);
                                            --fr;
                                        }
                                    }
                                    break;
                                }
                                PdfDictionary parent = (PdfDictionary)PdfReader.GetPdfObject(parentRef);
                                PdfArray kids = parent.GetAsArray(PdfName.Kids);
                                for (int fr = 0; fr < kids.Size; ++fr)
                                {
                                    PdfObject h = kids[fr];
                                    if (h.IsIndirect() && ((PrIndirectReference)h).Number == wdref.Number)
                                    {
                                        kids.Remove(fr);
                                        --fr;
                                    }
                                }
                                if (!kids.IsEmpty())
                                    break;
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
                for (int page = 1; page <= Reader.NumberOfPages; ++page)
                {
                    PdfDictionary pageDic = Reader.GetPageN(page);
                    PdfArray annots = pageDic.GetAsArray(PdfName.Annots);
                    if (annots == null)
                        continue;
                    for (int idx = 0; idx < annots.Size; ++idx)
                    {
                        PdfObject annoto = annots.GetDirectObject(idx);
                        if ((annoto is PdfIndirectReference) && !annoto.IsIndirect())
                            continue;
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
                return null;
            PageStamp ps = GetPageStamp(pageNum);
            if (ps.Over == null)
                ps.Over = new StampContent(this, ps);
            return ps.Over;
        }

        internal PageStamp GetPageStamp(int pageNum)
        {
            PdfDictionary pageN = Reader.GetPageN(pageNum);
            PageStamp ps = (PageStamp)PagesToContent[pageN];
            if (ps == null)
            {
                ps = new PageStamp(this, Reader, pageN);
                PagesToContent[pageN] = ps;
            }
            return ps;
        }

        internal override RandomAccessFileOrArray GetReaderFile(PdfReader reader)
        {
            if (Readers2Intrefs.ContainsKey(reader))
            {
                RandomAccessFileOrArray raf = (RandomAccessFileOrArray)Readers2File[reader];
                if (raf != null)
                    return raf;
                return reader.SafeFile;
            }
            if (CurrentPdfReaderInstance == null)
                return File;
            else
                return CurrentPdfReaderInstance.ReaderFile;
        }

        internal PdfContentByte GetUnderContent(int pageNum)
        {
            if (pageNum < 1 || pageNum > Reader.NumberOfPages)
                return null;
            PageStamp ps = GetPageStamp(pageNum);
            if (ps.Under == null)
                ps.Under = new StampContent(this, ps);
            return ps.Under;
        }

        internal void InsertPage(int pageNumber, Rectangle mediabox)
        {
            Rectangle media = new Rectangle(mediabox);
            int rotation = media.Rotation % 360;
            PdfDictionary page = new PdfDictionary(PdfName.Page);
            PdfDictionary resources = new PdfDictionary();
            PdfArray procset = new PdfArray();
            procset.Add(PdfName.Pdf);
            procset.Add(PdfName.Text);
            procset.Add(PdfName.Imageb);
            procset.Add(PdfName.Imagec);
            procset.Add(PdfName.Imagei);
            resources.Put(PdfName.Procset, procset);
            page.Put(PdfName.Resources, resources);
            page.Put(PdfName.Rotate, new PdfNumber(rotation));
            page.Put(PdfName.Mediabox, new PdfRectangle(media, rotation));
            PrIndirectReference pref = Reader.AddPdfObject(page);
            PdfDictionary parent;
            PrIndirectReference parentRef;
            if (pageNumber > Reader.NumberOfPages)
            {
                PdfDictionary lastPage = Reader.GetPageNRelease(Reader.NumberOfPages);
                parentRef = (PrIndirectReference)lastPage.Get(PdfName.Parent);
                parentRef = new PrIndirectReference(Reader, parentRef.Number);
                parent = (PdfDictionary)PdfReader.GetPdfObject(parentRef);
                PdfArray kids = (PdfArray)PdfReader.GetPdfObject(parent.Get(PdfName.Kids), parent);
                kids.Add(pref);
                MarkUsed(kids);
                Reader.pageRefs.InsertPage(pageNumber, pref);
            }
            else
            {
                if (pageNumber < 1)
                    pageNumber = 1;
                PdfDictionary firstPage = Reader.GetPageN(pageNumber);
                PrIndirectReference firstPageRef = Reader.GetPageOrigRef(pageNumber);
                Reader.ReleasePage(pageNumber);
                parentRef = (PrIndirectReference)firstPage.Get(PdfName.Parent);
                parentRef = new PrIndirectReference(Reader, parentRef.Number);
                parent = (PdfDictionary)PdfReader.GetPdfObject(parentRef);
                PdfArray kids = (PdfArray)PdfReader.GetPdfObject(parent.Get(PdfName.Kids), parent);
                int len = kids.Size;
                int num = firstPageRef.Number;
                for (int k = 0; k < len; ++k)
                {
                    PrIndirectReference cur = (PrIndirectReference)kids[k];
                    if (num == cur.Number)
                    {
                        kids.Add(k, pref);
                        break;
                    }
                }
                if (len == kids.Size)
                    throw new Exception("Internal inconsistence.");
                MarkUsed(kids);
                Reader.pageRefs.InsertPage(pageNumber, pref);
                CorrectAcroFieldPages(pageNumber);
            }
            page.Put(PdfName.Parent, parentRef);
            while (parent != null)
            {
                MarkUsed(parent);
                PdfNumber count = (PdfNumber)PdfReader.GetPdfObjectRelease(parent.Get(PdfName.Count));
                parent.Put(PdfName.Count, new PdfNumber(count.IntValue + 1));
                parent = parent.GetAsDict(PdfName.Parent);
            }
        }

        /// <summary>
        /// Getter for property append.
        /// </summary>
        /// <returns>Value of property append.</returns>
        internal bool IsAppend()
        {
            return Append;
        }

        /// <summary>
        /// Adds or replaces the Collection Dictionary in the Catalog.
        /// </summary>
        /// <param name="collection">the new collection dictionary.</param>
        internal void MakePackage(PdfCollection collection)
        {
            PdfDictionary catalog = Reader.Catalog;
            catalog.Put(PdfName.Collection, collection);
        }

        internal bool PartialFormFlattening(string name)
        {
            AcroFields af = AcroFields;
            if (acroFields.Xfa.XfaPresent)
                throw new InvalidOperationException("Partial form flattening is not supported with XFA forms.");
            if (!acroFields.Fields.ContainsKey(name))
                return false;
            PartialFlattening[name] = null;
            return true;
        }

        internal void ReplacePage(PdfReader r, int pageImported, int pageReplaced)
        {
            PdfDictionary pageN = Reader.GetPageN(pageReplaced);
            if (PagesToContent.ContainsKey(pageN))
                throw new InvalidOperationException("This page cannot be replaced: new content was already added");
            PdfImportedPage p = GetImportedPage(r, pageImported);
            PdfDictionary dic2 = Reader.GetPageNRelease(pageReplaced);
            dic2.Remove(PdfName.Resources);
            dic2.Remove(PdfName.Contents);
            moveRectangle(dic2, r, pageImported, PdfName.Mediabox, "media");
            moveRectangle(dic2, r, pageImported, PdfName.Cropbox, "crop");
            moveRectangle(dic2, r, pageImported, PdfName.Trimbox, "trim");
            moveRectangle(dic2, r, pageImported, PdfName.Artbox, "art");
            moveRectangle(dic2, r, pageImported, PdfName.Bleedbox, "bleed");
            dic2.Put(PdfName.Rotate, new PdfNumber(r.GetPageRotation(pageImported)));
            PdfContentByte cb = GetOverContent(pageReplaced);
            cb.AddTemplate(p, 0, 0);
            PageStamp ps = (PageStamp)PagesToContent[pageN];
            ps.ReplacePoint = ps.Over.InternalBuffer.Size;
        }

        /// <summary>
        /// Sets the display duration for the page (for presentations)
        /// </summary>
        /// <param name="seconds">the number of seconds to display the page. A negative value removes the entry</param>
        /// <param name="page">the page where the duration will be applied. The first page is 1</param>
        internal void SetDuration(int seconds, int page)
        {
            PdfDictionary pg = Reader.GetPageN(page);
            if (seconds < 0)
                pg.Remove(PdfName.Dur);
            else
                pg.Put(PdfName.Dur, new PdfNumber(seconds));
            MarkUsed(pg);
        }

        internal void SetJavaScript()
        {
            Hashtable djs = Pdf.GetDocumentLevelJs();
            if (djs.Count == 0)
                return;
            PdfDictionary catalog = Reader.Catalog;
            PdfDictionary names = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Names), catalog);
            if (names == null)
            {
                names = new PdfDictionary();
                catalog.Put(PdfName.Names, names);
                MarkUsed(catalog);
            }
            MarkUsed(names);
            PdfDictionary tree = PdfNameTree.WriteTree(djs, this);
            names.Put(PdfName.Javascript, AddToBody(tree).IndirectReference);
        }

        internal void SetOutlines()
        {
            if (NewBookmarks == null)
                return;
            DeleteOutlines();
            if (NewBookmarks.Count == 0)
                return;
            PdfDictionary catalog = Reader.Catalog;
            bool namedAsNames = (catalog.Get(PdfName.Dests) != null);
            WriteOutlines(catalog, namedAsNames);
            MarkUsed(catalog);
        }

        /// <summary>
        /// Sets the open and close page additional action.
        /// or  PdfWriter.PAGE_CLOSE
        /// @throws PdfException if the action type is invalid
        /// </summary>
        /// <param name="actionType">the action type. It can be  PdfWriter.PAGE_OPEN </param>
        /// <param name="action">the action to perform</param>
        /// <param name="page">the page where the action will be applied. The first page is 1</param>
        internal void SetPageAction(PdfName actionType, PdfAction action, int page)
        {
            if (!actionType.Equals(PageOpen) && !actionType.Equals(PageClose))
                throw new PdfException("Invalid page additional action type: " + actionType);
            PdfDictionary pg = Reader.GetPageN(page);
            PdfDictionary aa = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.Aa), pg);
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
            PdfIndirectReference thumb = GetImageReference(AddDirectImageSimple(image));
            Reader.ResetReleasePage();
            PdfDictionary dic = Reader.GetPageN(page);
            dic.Put(PdfName.Thumb, thumb);
            Reader.ResetReleasePage();
        }

        /// <summary>
        /// Sets the transition for the page
        /// </summary>
        /// <param name="transition">the transition object. A  null  removes the transition</param>
        /// <param name="page">the page where the transition will be applied. The first page is 1</param>
        internal void SetTransition(PdfTransition transition, int page)
        {
            PdfDictionary pg = Reader.GetPageN(page);
            if (transition == null)
                pg.Remove(PdfName.Trans);
            else
                pg.Put(PdfName.Trans, transition.TransitionDictionary);
            MarkUsed(pg);
        }

        internal void SweepKids(PdfObject obj)
        {
            PdfObject oo = PdfReader.KillIndirect(obj);
            if (oo == null || !oo.IsDictionary())
                return;
            PdfDictionary dic = (PdfDictionary)oo;
            PdfArray kids = (PdfArray)PdfReader.KillIndirect(dic.Get(PdfName.Kids));
            if (kids == null)
                return;
            for (int k = 0; k < kids.Size; ++k)
            {
                SweepKids(kids[k]);
            }
        }

        protected internal override int GetNewObjectNumber(PdfReader reader, int number, int generation)
        {
            IntHashtable refP = (IntHashtable)Readers2Intrefs[reader];
            if (refP != null)
            {
                int n = refP[number];
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
                    return number;
                int n = MyXref[number];
                if (n == 0)
                {
                    n = IndirectReferenceNumber;
                    MyXref[number] = n;
                }
                return n;
            }
            else
                return CurrentPdfReaderInstance.GetNewObjectNumber(number, generation);
        }
        protected internal void MarkUsed(PdfObject obj)
        {
            if (Append && obj != null)
            {
                PrIndirectReference refP = null;
                if (obj.Type == PdfObject.INDIRECT)
                    refP = (PrIndirectReference)obj;
                else
                    refP = obj.IndRef;
                if (refP != null)
                    Marked[refP.Number] = 1;
            }
        }

        protected internal void MarkUsed(int num)
        {
            if (Append)
                Marked[num] = 1;
        }

        /// <summary>
        /// Reads the OCProperties dictionary from the catalog of the existing document
        /// and fills the documentOCG, documentOCGorder and OCGRadioGroup variables in PdfWriter.
        /// Note that the original OCProperties of the existing document can contain more information.
        /// @since    2.1.2
        /// </summary>
        protected void ReadOcProperties()
        {
            if (DocumentOcg.Count != 0)
            {
                return;
            }
            PdfDictionary dict = Reader.Catalog.GetAsDict(PdfName.Ocproperties);
            if (dict == null)
            {
                return;
            }
            PdfArray ocgs = dict.GetAsArray(PdfName.Ocgs);
            PdfIndirectReference refi;
            PdfLayer layer;
            Hashtable ocgmap = new Hashtable();
            for (ListIterator i = ocgs.GetListIterator(); i.HasNext();)
            {
                refi = (PdfIndirectReference)i.Next();
                layer = new PdfLayer(null);
                layer.Ref = refi;
                layer.OnPanel = false;
                layer.Merge((PdfDictionary)PdfReader.GetPdfObject(refi));
                ocgmap[refi.ToString()] = layer;
            }
            PdfDictionary d = dict.GetAsDict(PdfName.D);
            PdfArray off = d.GetAsArray(PdfName.OFF);
            if (off != null)
            {
                for (ListIterator i = off.GetListIterator(); i.HasNext();)
                {
                    refi = (PdfIndirectReference)i.Next();
                    layer = (PdfLayer)ocgmap[refi.ToString()];
                    layer.On = false;
                }
            }
            PdfArray order = d.GetAsArray(PdfName.Order);
            if (order != null)
            {
                addOrder(null, order, ocgmap);
            }
            foreach (object o in ocgmap.Values)
                DocumentOcg[o] = null;
            OcgRadioGroup = d.GetAsArray(PdfName.Rbgroups);
            OcgLocked = d.GetAsArray(PdfName.Locked);
            if (OcgLocked == null)
                OcgLocked = new PdfArray();
        }

        private static void moveRectangle(PdfDictionary dic2, PdfReader r, int pageImported, PdfName key, string name)
        {
            Rectangle m = r.GetBoxSize(pageImported, name);
            if (m == null)
                dic2.Remove(key);
            else
                dic2.Put(key, new PdfRectangle(m));
        }
        void addFileAttachments()
        {
            Hashtable fs = Pdf.GetDocumentFileAttachment();
            if (fs.Count == 0)
                return;
            PdfDictionary catalog = Reader.Catalog;
            PdfDictionary names = (PdfDictionary)PdfReader.GetPdfObject(catalog.Get(PdfName.Names), catalog);
            if (names == null)
            {
                names = new PdfDictionary();
                catalog.Put(PdfName.Names, names);
                MarkUsed(catalog);
            }
            MarkUsed(names);
            Hashtable old = PdfNameTree.ReadTree((PdfDictionary)PdfReader.GetPdfObjectRelease(names.Get(PdfName.Embeddedfiles)));
            foreach (DictionaryEntry entry in fs)
            {
                string name = (string)entry.Key;
                int k = 0;
                string nn = name;
                while (old.ContainsKey(nn))
                {
                    ++k;
                    nn += " " + k;
                }
                old[nn] = entry.Value;
            }
            PdfDictionary tree = PdfNameTree.WriteTree(old, this);
            names.Put(PdfName.Embeddedfiles, AddToBody(tree).IndirectReference);
        }

        /// <summary>
        /// Recursive method to reconstruct the documentOCGorder variable in the writer.
        /// @since    2.1.2
        /// </summary>
        /// <param name="parent">a parent PdfLayer (can be null)</param>
        /// <param name="arr">an array possibly containing children for the parent PdfLayer</param>
        /// <param name="ocgmap">a Hashtable with indirect reference Strings as keys and PdfLayer objects as values.</param>
        private void addOrder(PdfLayer parent, PdfArray arr, Hashtable ocgmap)
        {
            PdfObject obj;
            PdfLayer layer;
            for (int i = 0; i < arr.Size; i++)
            {
                obj = arr[i];
                if (obj.IsIndirect())
                {
                    layer = (PdfLayer)ocgmap[obj.ToString()];
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
                    PdfArray sub = (PdfArray)obj;
                    if (sub.IsEmpty()) return;
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
                        PdfArray array = new PdfArray();
                        for (ListIterator j = sub.GetListIterator(); j.HasNext();)
                        {
                            array.Add((PdfObject)j.Next());
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
                throw new ArgumentException("FreeText flattening is not supported in append mode.");

            for (int page = 1; page <= Reader.NumberOfPages; ++page)
            {
                PdfDictionary pageDic = Reader.GetPageN(page);
                PdfArray annots = pageDic.GetAsArray(PdfName.Annots);
                if (annots == null)
                    continue;
                for (int idx = 0; idx < annots.Size; ++idx)
                {
                    PdfObject annoto = annots.GetDirectObject(idx);
                    if ((annoto is PdfIndirectReference) && !annoto.IsIndirect())
                        continue;

                    PdfDictionary annDic = (PdfDictionary)annoto;
                    if (!((PdfName)annDic.Get(PdfName.Subtype)).Equals(PdfName.Freetext))
                        continue;
                    PdfNumber ff = annDic.GetAsNumber(PdfName.F);
                    int flags = (ff != null) ? ff.IntValue : 0;

                    if ((flags & PdfAnnotation.FLAGS_PRINT) != 0 && (flags & PdfAnnotation.FLAGS_HIDDEN) == 0)
                    {
                        PdfObject obj1 = annDic.Get(PdfName.Ap);
                        if (obj1 == null)
                            continue;
                        PdfDictionary appDic = (obj1 is PdfIndirectReference) ?
                            (PdfDictionary)PdfReader.GetPdfObject(obj1) : (PdfDictionary)obj1;
                        PdfObject obj = appDic.Get(PdfName.N);
                        PdfAppearance app = null;
                        if (obj != null)
                        {
                            PdfObject objReal = PdfReader.GetPdfObject(obj);

                            if (obj is PdfIndirectReference && !obj.IsIndirect())
                                app = new PdfAppearance((PdfIndirectReference)obj);
                            else if (objReal is PdfStream)
                            {
                                ((PdfDictionary)objReal).Put(PdfName.Subtype, PdfName.Form);
                                app = new PdfAppearance((PdfIndirectReference)obj);
                            }
                            else
                            {
                                if (objReal.IsDictionary())
                                {
                                    PdfName asP = appDic.GetAsName(PdfName.As);
                                    if (asP != null)
                                    {
                                        PdfIndirectReference iref = (PdfIndirectReference)((PdfDictionary)objReal).Get(asP);
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
                            Rectangle box = PdfReader.GetNormalizedRectangle(annDic.GetAsArray(PdfName.Rect));
                            PdfContentByte cb = GetOverContent(page);
                            cb.SetLiteral("Q ");
                            cb.AddTemplate(app, box.Left, box.Bottom);
                            cb.SetLiteral("q ");
                        }
                    }
                }
                for (int idx = 0; idx < annots.Size; ++idx)
                {
                    PdfDictionary annot = annots.GetAsDict(idx);
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
        private void outlineTravel(PrIndirectReference outline)
        {
            while (outline != null)
            {
                PdfDictionary outlineR = (PdfDictionary)PdfReader.GetPdfObjectRelease(outline);
                PrIndirectReference first = (PrIndirectReference)outlineR.Get(PdfName.First);
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

            internal StampContent Over;
            internal PdfDictionary PageN;
            internal PageResources PageResources;
            internal int ReplacePoint;
            internal StampContent Under;
            internal PageStamp(PdfStamperImp stamper, PdfReader reader, PdfDictionary pageN)
            {
                PageN = pageN;
                PageResources = new PageResources();
                PdfDictionary resources = pageN.GetAsDict(PdfName.Resources);
                PageResources.SetOriginalResources(resources, stamper.NamePtr);
            }
        }
    }
}
