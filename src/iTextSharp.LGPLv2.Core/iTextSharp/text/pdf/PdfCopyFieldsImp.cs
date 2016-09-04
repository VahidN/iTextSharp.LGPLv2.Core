using System;
using System.Collections;
using System.IO;
using System.util;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// @author  psoares
    /// </summary>
    internal class PdfCopyFieldsImp : PdfWriter
    {
        internal ArrayList Fields = new ArrayList();
        internal Hashtable FieldTree = new Hashtable();
        internal RandomAccessFileOrArray File;
        internal PdfDictionary Form;
        internal Document Nd;
        internal ArrayList PageDics = new ArrayList();
        internal ArrayList PageRefs = new ArrayList();
        internal Hashtable Pages2Intrefs = new Hashtable();
        internal ArrayList Readers = new ArrayList();
        internal Hashtable Readers2Intrefs = new Hashtable();
        internal PdfDictionary Resources = new PdfDictionary();
        internal Hashtable Visited = new Hashtable();

        protected internal static Hashtable FieldKeys = new Hashtable();
        protected internal static Hashtable WidgetKeys = new Hashtable();

        private static readonly PdfName _iTextTag = new PdfName("_iTextTag_");
        private static readonly object _zero = 0;
        private readonly ArrayList _calculationOrder = new ArrayList();
        private ArrayList _calculationOrderRefs;
        bool _closing;
        private bool _hasSignature;
        private Hashtable _tabOrder;
        static PdfCopyFieldsImp()
        {
            object one = 1;
            WidgetKeys[PdfName.Subtype] = one;
            WidgetKeys[PdfName.Contents] = one;
            WidgetKeys[PdfName.Rect] = one;
            WidgetKeys[PdfName.Nm] = one;
            WidgetKeys[PdfName.M] = one;
            WidgetKeys[PdfName.F] = one;
            WidgetKeys[PdfName.Bs] = one;
            WidgetKeys[PdfName.Border] = one;
            WidgetKeys[PdfName.Ap] = one;
            WidgetKeys[PdfName.As] = one;
            WidgetKeys[PdfName.C] = one;
            WidgetKeys[PdfName.A] = one;
            WidgetKeys[PdfName.Structparent] = one;
            WidgetKeys[PdfName.Oc] = one;
            WidgetKeys[PdfName.H] = one;
            WidgetKeys[PdfName.Mk] = one;
            WidgetKeys[PdfName.Da] = one;
            WidgetKeys[PdfName.Q] = one;
            FieldKeys[PdfName.Aa] = one;
            FieldKeys[PdfName.Ft] = one;
            FieldKeys[PdfName.Tu] = one;
            FieldKeys[PdfName.Tm] = one;
            FieldKeys[PdfName.Ff] = one;
            FieldKeys[PdfName.V] = one;
            FieldKeys[PdfName.Dv] = one;
            FieldKeys[PdfName.Ds] = one;
            FieldKeys[PdfName.Rv] = one;
            FieldKeys[PdfName.Opt] = one;
            FieldKeys[PdfName.Maxlen] = one;
            FieldKeys[PdfName.Ti] = one;
            FieldKeys[PdfName.I] = one;
            FieldKeys[PdfName.Lock] = one;
            FieldKeys[PdfName.Sv] = one;
        }

        internal PdfCopyFieldsImp(Stream os) : this(os, '\0')
        {
        }

        internal PdfCopyFieldsImp(Stream os, char pdfVersion) : base(new PdfDocument(), os)
        {
            Pdf.AddWriter(this);
            if (pdfVersion != 0)
                PdfVersion = pdfVersion;
            Nd = new Document();
            Nd.AddDocListener(Pdf);
        }

        public override void Close()
        {
            if (_closing)
            {
                base.Close();
                return;
            }
            _closing = true;
            CloseIt();
        }

        public override PdfIndirectReference GetPageReference(int page)
        {
            return (PdfIndirectReference)PageRefs[page - 1];
        }

        public void OpenDoc()
        {
            if (!Nd.IsOpen())
                Nd.Open();
        }

        internal void AddDocument(PdfReader reader, ArrayList pagesToKeep)
        {
            if (!Readers2Intrefs.ContainsKey(reader) && reader.Tampered)
                throw new DocumentException("The document was reused.");
            reader = new PdfReader(reader);
            reader.SelectPages(pagesToKeep);
            if (reader.NumberOfPages == 0)
                return;
            reader.Tampered = false;
            AddDocument(reader);
        }

        internal void AddDocument(PdfReader reader)
        {
            if (!reader.IsOpenedWithFullPermissions)
                throw new BadPasswordException("PdfReader not opened with owner password");
            OpenDoc();
            if (Readers2Intrefs.ContainsKey(reader))
            {
                reader = new PdfReader(reader);
            }
            else
            {
                if (reader.Tampered)
                    throw new DocumentException("The document was reused.");
                reader.ConsolidateNamedDestinations();
                reader.Tampered = true;
            }
            reader.ShuffleSubsetNames();
            Readers2Intrefs[reader] = new IntHashtable();
            Readers.Add(reader);
            int len = reader.NumberOfPages;
            IntHashtable refs = new IntHashtable();
            for (int p = 1; p <= len; ++p)
            {
                refs[reader.GetPageOrigRef(p).Number] = 1;
                reader.ReleasePage(p);
            }
            Pages2Intrefs[reader] = refs;
            Visited[reader] = new IntHashtable();
            Fields.Add(reader.AcroFields);
            UpdateCalculationOrder(reader);
        }

        internal void AddPageOffsetToField(Hashtable fd, int pageOffset)
        {
            if (pageOffset == 0)
                return;
            foreach (AcroFields.Item item in fd.Values)
            {
                ArrayList page = item.Page;
                for (int k = 0; k < page.Count; ++k)
                {
                    int p = item.GetPage(k);
                    item.ForcePage(k, p + pageOffset);
                }
            }
        }

        internal void CreateWidgets(ArrayList list, AcroFields.Item item)
        {
            for (int k = 0; k < item.Size; ++k)
            {
                list.Add(item.GetPage(k));
                PdfDictionary merged = item.GetMerged(k);
                PdfObject dr = merged.Get(PdfName.Dr);
                if (dr != null)
                    PdfFormField.MergeResources(Resources, (PdfDictionary)PdfReader.GetPdfObject(dr));
                PdfDictionary widget = new PdfDictionary();
                foreach (PdfName key in merged.Keys)
                {
                    if (WidgetKeys.ContainsKey(key))
                        widget.Put(key, merged.Get(key));
                }
                widget.Put(_iTextTag, new PdfNumber(item.GetTabOrder(k) + 1));
                list.Add(widget);
            }
        }

        internal override RandomAccessFileOrArray GetReaderFile(PdfReader reader)
        {
            return File;
        }

        internal void MergeField(string name, AcroFields.Item item)
        {
            Hashtable map = FieldTree;
            StringTokenizer tk = new StringTokenizer(name, ".");
            if (!tk.HasMoreTokens())
                return;
            while (true)
            {
                string s = tk.NextToken();
                object obj = map[s];
                if (tk.HasMoreTokens())
                {
                    if (obj == null)
                    {
                        obj = new Hashtable();
                        map[s] = obj;
                        map = (Hashtable)obj;
                        continue;
                    }
                    else if (obj is Hashtable)
                        map = (Hashtable)obj;
                    else
                        return;
                }
                else
                {
                    if (obj is Hashtable)
                        return;
                    PdfDictionary merged = item.GetMerged(0);
                    if (obj == null)
                    {
                        PdfDictionary field = new PdfDictionary();
                        if (PdfName.Sig.Equals(merged.Get(PdfName.Ft)))
                            _hasSignature = true;
                        foreach (PdfName key in merged.Keys)
                        {
                            if (FieldKeys.ContainsKey(key))
                                field.Put(key, merged.Get(key));
                        }
                        ArrayList list = new ArrayList();
                        list.Add(field);
                        CreateWidgets(list, item);
                        map[s] = list;
                    }
                    else
                    {
                        ArrayList list = (ArrayList)obj;
                        PdfDictionary field = (PdfDictionary)list[0];
                        PdfName type1 = (PdfName)field.Get(PdfName.Ft);
                        PdfName type2 = (PdfName)merged.Get(PdfName.Ft);
                        if (type1 == null || !type1.Equals(type2))
                            return;
                        int flag1 = 0;
                        PdfObject f1 = field.Get(PdfName.Ff);
                        if (f1 != null && f1.IsNumber())
                            flag1 = ((PdfNumber)f1).IntValue;
                        int flag2 = 0;
                        PdfObject f2 = merged.Get(PdfName.Ff);
                        if (f2 != null && f2.IsNumber())
                            flag2 = ((PdfNumber)f2).IntValue;
                        if (type1.Equals(PdfName.Btn))
                        {
                            if (((flag1 ^ flag2) & PdfFormField.FF_PUSHBUTTON) != 0)
                                return;
                            if ((flag1 & PdfFormField.FF_PUSHBUTTON) == 0 && ((flag1 ^ flag2) & PdfFormField.FF_RADIO) != 0)
                                return;
                        }
                        else if (type1.Equals(PdfName.Ch))
                        {
                            if (((flag1 ^ flag2) & PdfFormField.FF_COMBO) != 0)
                                return;
                        }
                        CreateWidgets(list, item);
                    }
                    return;
                }
            }
        }

        internal virtual void MergeFields()
        {
            int pageOffset = 0;
            for (int k = 0; k < Fields.Count; ++k)
            {
                Hashtable fd = ((AcroFields)Fields[k]).Fields;
                AddPageOffsetToField(fd, pageOffset);
                MergeWithMaster(fd);
                pageOffset += ((PdfReader)Readers[k]).NumberOfPages;
            }
        }

        internal void MergeWithMaster(Hashtable fd)
        {
            foreach (DictionaryEntry entry in fd)
            {
                string name = (string)entry.Key;
                MergeField(name, (AcroFields.Item)entry.Value);
            }
        }

        internal void Propagate(PdfObject obj, PdfIndirectReference refo, bool restricted)
        {
            if (obj == null)
                return;
            //        if (refo != null)
            //            AddToBody(obj, refo);
            if (obj is PdfIndirectReference)
                return;
            switch (obj.Type)
            {
                case PdfObject.DICTIONARY:
                case PdfObject.STREAM:
                    {
                        PdfDictionary dic = (PdfDictionary)obj;
                        foreach (PdfName key in dic.Keys)
                        {
                            if (restricted && (key.Equals(PdfName.Parent) || key.Equals(PdfName.Kids)))
                                continue;
                            PdfObject ob = dic.Get(key);
                            if (ob != null && ob.IsIndirect())
                            {
                                PrIndirectReference ind = (PrIndirectReference)ob;
                                if (!SetVisited(ind) && !IsPage(ind))
                                {
                                    PdfIndirectReference refi = GetNewReference(ind);
                                    Propagate(PdfReader.GetPdfObjectRelease(ind), refi, restricted);
                                }
                            }
                            else
                                Propagate(ob, null, restricted);
                        }
                        break;
                    }
                case PdfObject.ARRAY:
                    {
                        //PdfArray arr = new PdfArray();
                        for (ListIterator it = ((PdfArray)obj).GetListIterator(); it.HasNext();)
                        {
                            PdfObject ob = (PdfObject)it.Next();
                            if (ob != null && ob.IsIndirect())
                            {
                                PrIndirectReference ind = (PrIndirectReference)ob;
                                if (!IsVisited(ind) && !IsPage(ind))
                                {
                                    PdfIndirectReference refi = GetNewReference(ind);
                                    Propagate(PdfReader.GetPdfObjectRelease(ind), refi, restricted);
                                }
                            }
                            else
                                Propagate(ob, null, restricted);
                        }
                        break;
                    }
                case PdfObject.INDIRECT:
                    {
                        throw new Exception("Reference pointing to reference.");
                    }
            }
        }

        protected internal override int GetNewObjectNumber(PdfReader reader, int number, int generation)
        {
            IntHashtable refs = (IntHashtable)Readers2Intrefs[reader];
            int n = refs[number];
            if (n == 0)
            {
                n = IndirectReferenceNumber;
                refs[number] = n;
            }
            return n;
        }

        /// <summary>
        /// Checks if a reference refers to a page object.
        /// </summary>
        /// <param name="refi">the reference that needs to be checked</param>
        /// <returns>true is the reference refers to a page object.</returns>
        protected internal bool IsPage(PrIndirectReference refi)
        {
            IntHashtable refs = (IntHashtable)Pages2Intrefs[refi.Reader];
            if (refs != null)
                return refs.ContainsKey(refi.Number);
            else
                return false;
        }

        /// <summary>
        /// Checks if a reference has already been "visited" in the copy process.
        /// </summary>
        /// <param name="refi">the reference that needs to be checked</param>
        /// <returns>true if the reference was already visited</returns>
        protected internal bool IsVisited(PrIndirectReference refi)
        {
            IntHashtable refs = (IntHashtable)Visited[refi.Reader];
            if (refs != null)
                return refs.ContainsKey(refi.Number);
            else
                return false;
        }

        protected internal bool IsVisited(PdfReader reader, int number, int generation)
        {
            IntHashtable refs = (IntHashtable)Readers2Intrefs[reader];
            return refs.ContainsKey(number);
        }

        /// <summary>
        /// Sets a reference to "visited" in the copy process.
        /// </summary>
        /// <param name="refi">the reference that needs to be set to "visited"</param>
        /// <returns>true if the reference was set to visited</returns>
        protected internal bool SetVisited(PrIndirectReference refi)
        {
            IntHashtable refs = (IntHashtable)Visited[refi.Reader];
            if (refs != null)
            {
                int old = refs[refi.Number];
                refs[refi.Number] = 1;
                return (old != 0);
            }
            else
                return false;
        }

        protected internal void UpdateCalculationOrder(PdfReader reader)
        {
            PdfDictionary catalog = reader.Catalog;
            PdfDictionary acro = catalog.GetAsDict(PdfName.Acroform);
            if (acro == null)
                return;
            PdfArray co = acro.GetAsArray(PdfName.Co);
            if (co == null || co.Size == 0)
                return;
            AcroFields af = reader.AcroFields;
            for (int k = 0; k < co.Size; ++k)
            {
                PdfObject obj = co[k];
                if (obj == null || !obj.IsIndirect())
                    continue;
                string name = getCoName(reader, (PrIndirectReference)obj);
                if (af.GetFieldItem(name) == null)
                    continue;
                name = "." + name;
                if (_calculationOrder.Contains(name))
                    continue;
                _calculationOrder.Add(name);
            }
        }

        protected PdfArray BranchForm(Hashtable level, PdfIndirectReference parent, string fname)
        {
            PdfArray arr = new PdfArray();
            foreach (DictionaryEntry entry in level)
            {
                string name = (string)entry.Key;
                object obj = entry.Value;
                PdfIndirectReference ind = PdfIndirectReference;
                PdfDictionary dic = new PdfDictionary();
                if (parent != null)
                    dic.Put(PdfName.Parent, parent);
                dic.Put(PdfName.T, new PdfString(name, PdfObject.TEXT_UNICODE));
                string fname2 = fname + "." + name;
                int coidx = _calculationOrder.IndexOf(fname2);
                if (coidx >= 0)
                    _calculationOrderRefs[coidx] = ind;
                if (obj is Hashtable)
                {
                    dic.Put(PdfName.Kids, BranchForm((Hashtable)obj, ind, fname2));
                    arr.Add(ind);
                    AddToBody(dic, ind);
                }
                else
                {
                    ArrayList list = (ArrayList)obj;
                    dic.MergeDifferent((PdfDictionary)list[0]);
                    if (list.Count == 3)
                    {
                        dic.MergeDifferent((PdfDictionary)list[2]);
                        int page = (int)list[1];
                        PdfDictionary pageDic = (PdfDictionary)PageDics[page - 1];
                        PdfArray annots = pageDic.GetAsArray(PdfName.Annots);
                        if (annots == null)
                        {
                            annots = new PdfArray();
                            pageDic.Put(PdfName.Annots, annots);
                        }
                        PdfNumber nn = (PdfNumber)dic.Get(_iTextTag);
                        dic.Remove(_iTextTag);
                        adjustTabOrder(annots, ind, nn);
                    }
                    else
                    {
                        PdfArray kids = new PdfArray();
                        for (int k = 1; k < list.Count; k += 2)
                        {
                            int page = (int)list[k];
                            PdfDictionary pageDic = (PdfDictionary)PageDics[page - 1];
                            PdfArray annots = pageDic.GetAsArray(PdfName.Annots);
                            if (annots == null)
                            {
                                annots = new PdfArray();
                                pageDic.Put(PdfName.Annots, annots);
                            }
                            PdfDictionary widget = new PdfDictionary();
                            widget.Merge((PdfDictionary)list[k + 1]);
                            widget.Put(PdfName.Parent, ind);
                            PdfNumber nn = (PdfNumber)widget.Get(_iTextTag);
                            widget.Remove(_iTextTag);
                            PdfIndirectReference wref = AddToBody(widget).IndirectReference;
                            adjustTabOrder(annots, wref, nn);
                            kids.Add(wref);
                            Propagate(widget, null, false);
                        }
                        dic.Put(PdfName.Kids, kids);
                    }
                    arr.Add(ind);
                    AddToBody(dic, ind);
                    Propagate(dic, null, false);
                }
            }
            return arr;
        }

        protected void CloseIt()
        {
            for (int k = 0; k < Readers.Count; ++k)
            {
                ((PdfReader)Readers[k]).RemoveFields();
            }
            for (int r = 0; r < Readers.Count; ++r)
            {
                PdfReader reader = (PdfReader)Readers[r];
                for (int page = 1; page <= reader.NumberOfPages; ++page)
                {
                    PageRefs.Add(GetNewReference(reader.GetPageOrigRef(page)));
                    PageDics.Add(reader.GetPageN(page));
                }
            }
            MergeFields();
            CreateAcroForms();
            for (int r = 0; r < Readers.Count; ++r)
            {
                PdfReader reader = (PdfReader)Readers[r];
                for (int page = 1; page <= reader.NumberOfPages; ++page)
                {
                    PdfDictionary dic = reader.GetPageN(page);
                    PdfIndirectReference pageRef = GetNewReference(reader.GetPageOrigRef(page));
                    PdfIndirectReference parent = Root.AddPageRef(pageRef);
                    dic.Put(PdfName.Parent, parent);
                    Propagate(dic, pageRef, false);
                }
            }
            foreach (DictionaryEntry entry in Readers2Intrefs)
            {
                PdfReader reader = (PdfReader)entry.Key;
                try
                {
                    File = reader.SafeFile;
                    File.ReOpen();
                    IntHashtable t = (IntHashtable)entry.Value;
                    int[] keys = t.ToOrderedKeys();
                    for (int k = 0; k < keys.Length; ++k)
                    {
                        PrIndirectReference refi = new PrIndirectReference(reader, keys[k]);
                        AddToBody(PdfReader.GetPdfObjectRelease(refi), t[keys[k]]);
                    }
                }
                finally
                {
                    try
                    {
                        File.Close();
                        reader.Close();
                    }
                    catch
                    {
                        // empty on purpose
                    }
                }
            }
            Pdf.Close();
        }

        protected void CreateAcroForms()
        {
            if (FieldTree.Count == 0)
                return;
            Form = new PdfDictionary();
            Form.Put(PdfName.Dr, Resources);
            Propagate(Resources, null, false);
            Form.Put(PdfName.Da, new PdfString("/Helv 0 Tf 0 g "));
            _tabOrder = new Hashtable();
            _calculationOrderRefs = new ArrayList(_calculationOrder);
            Form.Put(PdfName.Fields, BranchForm(FieldTree, null, ""));
            if (_hasSignature)
                Form.Put(PdfName.Sigflags, new PdfNumber(3));
            PdfArray co = new PdfArray();
            for (int k = 0; k < _calculationOrderRefs.Count; ++k)
            {
                object obj = _calculationOrderRefs[k];
                if (obj is PdfIndirectReference)
                    co.Add((PdfIndirectReference)obj);
            }
            if (co.Size > 0)
                Form.Put(PdfName.Co, co);
        }

        protected override PdfDictionary GetCatalog(PdfIndirectReference rootObj)
        {
            PdfDictionary cat = Pdf.GetCatalog(rootObj);
            if (Form != null)
            {
                PdfIndirectReference refi = AddToBody(Form).IndirectReference;
                cat.Put(PdfName.Acroform, refi);
            }
            return cat;
        }

        protected PdfIndirectReference GetNewReference(PrIndirectReference refi)
        {
            return new PdfIndirectReference(0, GetNewObjectNumber(refi.Reader, refi.Number, 0));
        }

        private static string getCoName(PdfReader reader, PrIndirectReference refi)
        {
            string name = "";
            while (refi != null)
            {
                PdfObject obj = PdfReader.GetPdfObject(refi);
                if (obj == null || obj.Type != PdfObject.DICTIONARY)
                    break;
                PdfDictionary dic = (PdfDictionary)obj;
                PdfString t = dic.GetAsString(PdfName.T);
                if (t != null)
                {
                    name = t.ToUnicodeString() + "." + name;
                }
                refi = (PrIndirectReference)dic.Get(PdfName.Parent);
            }
            if (name.EndsWith("."))
                name = name.Substring(0, name.Length - 1);
            return name;
        }
        private void adjustTabOrder(PdfArray annots, PdfIndirectReference ind, PdfNumber nn)
        {
            int v = nn.IntValue;
            ArrayList t = (ArrayList)_tabOrder[annots];
            if (t == null)
            {
                t = new ArrayList();
                int size = annots.Size - 1;
                for (int k = 0; k < size; ++k)
                {
                    t.Add(_zero);
                }
                t.Add(v);
                _tabOrder[annots] = t;
                annots.Add(ind);
            }
            else
            {
                int size = t.Count - 1;
                for (int k = size; k >= 0; --k)
                {
                    if ((int)t[k] <= v)
                    {
                        t.Insert(k + 1, v);
                        annots.Add(k + 1, ind);
                        size = -2;
                        break;
                    }
                }
                if (size != -2)
                {
                    t.Insert(0, v);
                    annots.Add(0, ind);
                }
            }
        }
    }
}
