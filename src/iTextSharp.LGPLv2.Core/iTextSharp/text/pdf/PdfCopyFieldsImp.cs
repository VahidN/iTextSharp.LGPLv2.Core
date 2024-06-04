using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     @author  psoares
/// </summary>
internal class PdfCopyFieldsImp : PdfWriter
{
    private const int _zero = 0;
    protected internal static readonly INullValueDictionary<PdfName, int> FieldKeys = new NullValueDictionary<PdfName, int>();
    protected internal static readonly INullValueDictionary<PdfName, int> WidgetKeys = new NullValueDictionary<PdfName, int>();

    private static readonly PdfName _iTextTag = new("_iTextTag_");
    private readonly List<string> _calculationOrder = new();
    private List<object> _calculationOrderRefs;
    private bool _closing;
    private bool _hasSignature;
    private INullValueDictionary<PdfArray, List<int>> _tabOrder;
    internal readonly List<AcroFields> Fields = new();
    internal readonly INullValueDictionary<string, object> FieldTree = new NullValueDictionary<string, object>();
    internal RandomAccessFileOrArray File;
    internal PdfDictionary Form;
    internal readonly Document Nd;
    internal readonly List<PdfDictionary> PageDics = new();
    internal readonly List<PdfIndirectReference> PageRefs = new();

    internal readonly INullValueDictionary<PdfReader, NullValueDictionary<int, int>> Pages2Intrefs =
        new NullValueDictionary<PdfReader, NullValueDictionary<int, int>>();

    internal readonly List<PdfReader> Readers = new();

    internal readonly INullValueDictionary<PdfReader, NullValueDictionary<int, int>> Readers2Intrefs =
        new NullValueDictionary<PdfReader, NullValueDictionary<int, int>>();

    internal readonly PdfDictionary Resources = new();

    internal readonly INullValueDictionary<PdfReader, NullValueDictionary<int, int>> Visited =
        new NullValueDictionary<PdfReader, NullValueDictionary<int, int>>();

    static PdfCopyFieldsImp()
    {
        var one = 1;
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
        {
            PdfVersion = pdfVersion;
        }

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

    public override PdfIndirectReference GetPageReference(int page) => PageRefs[page - 1];

    public void OpenDoc()
    {
        if (!Nd.IsOpen())
        {
            Nd.Open();
        }
    }

    internal void AddDocument(PdfReader reader, ICollection<int> pagesToKeep)
    {
        if (!Readers2Intrefs.ContainsKey(reader) && reader.Tampered)
        {
            throw new DocumentException("The document was reused.");
        }

        reader = new PdfReader(reader);
        reader.SelectPages(pagesToKeep);
        if (reader.NumberOfPages == 0)
        {
            return;
        }

        reader.Tampered = false;
        AddDocument(reader);
    }

    internal void AddDocument(PdfReader reader)
    {
        if (!reader.IsOpenedWithFullPermissions)
        {
            throw new BadPasswordException("PdfReader not opened with owner password");
        }

        OpenDoc();
        if (Readers2Intrefs.ContainsKey(reader))
        {
            reader = new PdfReader(reader);
        }
        else
        {
            if (reader.Tampered)
            {
                throw new DocumentException("The document was reused.");
            }

            reader.ConsolidateNamedDestinations();
            reader.Tampered = true;
        }

        reader.ShuffleSubsetNames();
        Readers2Intrefs[reader] = new NullValueDictionary<int, int>();
        Readers.Add(reader);
        var len = reader.NumberOfPages;
        var refs = new NullValueDictionary<int, int>();
        for (var p = 1; p <= len; ++p)
        {
            refs[reader.GetPageOrigRef(p).Number] = 1;
            reader.ReleasePage(p);
        }

        Pages2Intrefs[reader] = refs;
        Visited[reader] = new NullValueDictionary<int, int>();
        Fields.Add(reader.AcroFields);
        UpdateCalculationOrder(reader);
    }

    internal static void AddPageOffsetToField(INullValueDictionary<string, AcroFields.Item> fd, int pageOffset)
    {
        if (pageOffset == 0)
        {
            return;
        }

        foreach (var item in fd.Values)
        {
            var page = item.Page;
            for (var k = 0; k < page.Count; ++k)
            {
                var p = item.GetPage(k);
                item.ForcePage(k, p + pageOffset);
            }
        }
    }

    internal void CreateWidgets(IList<object> list, AcroFields.Item item)
    {
        for (var k = 0; k < item.Size; ++k)
        {
            list.Add(item.GetPage(k));
            var merged = item.GetMerged(k);
            var dr = merged.Get(PdfName.Dr);
            if (dr != null)
            {
                PdfFormField.MergeResources(Resources, (PdfDictionary)PdfReader.GetPdfObject(dr));
            }

            var widget = new PdfDictionary();
            foreach (var key in merged.Keys)
            {
                if (WidgetKeys.ContainsKey(key))
                {
                    widget.Put(key, merged.Get(key));
                }
            }

            widget.Put(_iTextTag, new PdfNumber(item.GetTabOrder(k) + 1));
            list.Add(widget);
        }
    }

    internal override RandomAccessFileOrArray GetReaderFile(PdfReader reader) => File;

    internal void MergeField(string name, AcroFields.Item item)
    {
        var map = FieldTree;
        var tk = new StringTokenizer(name, ".");
        if (!tk.HasMoreTokens())
        {
            return;
        }

        while (true)
        {
            var s = tk.NextToken();
            var obj = map[s];
            if (tk.HasMoreTokens())
            {
                if (obj == null)
                {
                    obj = new NullValueDictionary<string, object>();
                    map[s] = obj;
                    map = (NullValueDictionary<string, object>)obj;
                    continue;
                }

                if (obj is NullValueDictionary<string, object>)
                {
                    map = (NullValueDictionary<string, object>)obj;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (obj is NullValueDictionary<string, object>)
                {
                    return;
                }

                var merged = item.GetMerged(0);
                if (obj == null)
                {
                    var field = new PdfDictionary();
                    if (PdfName.Sig.Equals(merged.Get(PdfName.Ft)))
                    {
                        _hasSignature = true;
                    }

                    foreach (var key in merged.Keys)
                    {
                        if (FieldKeys.ContainsKey(key))
                        {
                            field.Put(key, merged.Get(key));
                        }
                    }

                    List<object> list = new();
                    list.Add(field);
                    CreateWidgets(list, item);
                    map[s] = list;
                }
                else
                {
                    var list = (List<object>)obj;
                    var field = (PdfDictionary)list[0];
                    var type1 = (PdfName)field.Get(PdfName.Ft);
                    var type2 = (PdfName)merged.Get(PdfName.Ft);
                    if (type1 == null || !type1.Equals(type2))
                    {
                        return;
                    }

                    var flag1 = 0;
                    var f1 = field.Get(PdfName.Ff);
                    if (f1 != null && f1.IsNumber())
                    {
                        flag1 = ((PdfNumber)f1).IntValue;
                    }

                    var flag2 = 0;
                    var f2 = merged.Get(PdfName.Ff);
                    if (f2 != null && f2.IsNumber())
                    {
                        flag2 = ((PdfNumber)f2).IntValue;
                    }

                    if (type1.Equals(PdfName.Btn))
                    {
                        if (((flag1 ^ flag2) & PdfFormField.FF_PUSHBUTTON) != 0)
                        {
                            return;
                        }

                        if ((flag1 & PdfFormField.FF_PUSHBUTTON) == 0 && ((flag1 ^ flag2) & PdfFormField.FF_RADIO) != 0)
                        {
                            return;
                        }
                    }
                    else if (type1.Equals(PdfName.Ch))
                    {
                        if (((flag1 ^ flag2) & PdfFormField.FF_COMBO) != 0)
                        {
                            return;
                        }
                    }

                    CreateWidgets(list, item);
                }

                return;
            }
        }
    }

    internal virtual void MergeFields()
    {
        var pageOffset = 0;
        for (var k = 0; k < Fields.Count; ++k)
        {
            var fd = Fields[k].Fields;
            AddPageOffsetToField(fd, pageOffset);
            MergeWithMaster(fd);
            pageOffset += Readers[k].NumberOfPages;
        }
    }

    internal void MergeWithMaster(INullValueDictionary<string, AcroFields.Item> fd)
    {
        foreach (var entry in fd)
        {
            var name = entry.Key;
            MergeField(name, entry.Value);
        }
    }

    internal void Propagate(PdfObject obj, PdfIndirectReference refo, bool restricted)
    {
        if (obj == null)
        {
            return;
        }

        //        if (refo != null)
        //            AddToBody(obj, refo);
        if (obj is PdfIndirectReference)
        {
            return;
        }

        switch (obj.Type)
        {
            case PdfObject.DICTIONARY:
            case PdfObject.STREAM:
            {
                var dic = (PdfDictionary)obj;
                foreach (var key in dic.Keys)
                {
                    if (restricted && (key.Equals(PdfName.Parent) || key.Equals(PdfName.Kids)))
                    {
                        continue;
                    }

                    var ob = dic.Get(key);
                    if (ob != null && ob.IsIndirect())
                    {
                        var ind = (PrIndirectReference)ob;
                        if (!SetVisited(ind) && !IsPage(ind))
                        {
                            var refi = GetNewReference(ind);
                            Propagate(PdfReader.GetPdfObjectRelease(ind), refi, restricted);
                        }
                    }
                    else
                    {
                        Propagate(ob, null, restricted);
                    }
                }

                break;
            }
            case PdfObject.ARRAY:
            {
                //PdfArray arr = new PdfArray();
                for (var it = ((PdfArray)obj).GetListIterator(); it.HasNext();)
                {
                    var ob = it.Next();
                    if (ob != null && ob.IsIndirect())
                    {
                        var ind = (PrIndirectReference)ob;
                        if (!IsVisited(ind) && !IsPage(ind))
                        {
                            var refi = GetNewReference(ind);
                            Propagate(PdfReader.GetPdfObjectRelease(ind), refi, restricted);
                        }
                    }
                    else
                    {
                        Propagate(ob, null, restricted);
                    }
                }

                break;
            }
            case PdfObject.INDIRECT:
            {
                throw new InvalidOperationException("Reference pointing to reference.");
            }
        }
    }

    protected internal override int GetNewObjectNumber(PdfReader reader, int number, int generation)
    {
        var refs = Readers2Intrefs[reader];
        var n = refs[number];
        if (n == 0)
        {
            n = IndirectReferenceNumber;
            refs[number] = n;
        }

        return n;
    }

    /// <summary>
    ///     Checks if a reference refers to a page object.
    /// </summary>
    /// <param name="refi">the reference that needs to be checked</param>
    /// <returns>true is the reference refers to a page object.</returns>
    protected internal bool IsPage(PrIndirectReference refi)
    {
        var refs = Pages2Intrefs[refi.Reader];
        if (refs != null)
        {
            return refs.ContainsKey(refi.Number);
        }

        return false;
    }

    /// <summary>
    ///     Checks if a reference has already been "visited" in the copy process.
    /// </summary>
    /// <param name="refi">the reference that needs to be checked</param>
    /// <returns>true if the reference was already visited</returns>
    protected internal bool IsVisited(PrIndirectReference refi)
    {
        var refs = Visited[refi.Reader];
        if (refs != null)
        {
            return refs.ContainsKey(refi.Number);
        }

        return false;
    }

    protected internal bool IsVisited(PdfReader reader, int number, int generation)
    {
        var refs = Readers2Intrefs[reader];
        return refs.ContainsKey(number);
    }

    /// <summary>
    ///     Sets a reference to "visited" in the copy process.
    /// </summary>
    /// <param name="refi">the reference that needs to be set to "visited"</param>
    /// <returns>true if the reference was set to visited</returns>
    protected internal bool SetVisited(PrIndirectReference refi)
    {
        var refs = Visited[refi.Reader];
        if (refs != null)
        {
            var old = refs[refi.Number];
            refs[refi.Number] = 1;
            return old != 0;
        }

        return false;
    }

    protected internal void UpdateCalculationOrder(PdfReader reader)
    {
        var catalog = reader.Catalog;
        var acro = catalog.GetAsDict(PdfName.Acroform);
        if (acro == null)
        {
            return;
        }

        var co = acro.GetAsArray(PdfName.Co);
        if (co == null || co.Size == 0)
        {
            return;
        }

        var af = reader.AcroFields;
        for (var k = 0; k < co.Size; ++k)
        {
            var obj = co[k];
            if (obj == null || !obj.IsIndirect())
            {
                continue;
            }

            var name = getCoName(reader, (PrIndirectReference)obj);
            if (af.GetFieldItem(name) == null)
            {
                continue;
            }

            name = "." + name;
            if (_calculationOrder.Contains(name))
            {
                continue;
            }

            _calculationOrder.Add(name);
        }
    }

    protected PdfArray BranchForm(INullValueDictionary<string, object> level, PdfIndirectReference parent, string fname)
    {
        var arr = new PdfArray();
        foreach (var entry in level)
        {
            var name = entry.Key;
            var obj = entry.Value;
            var ind = PdfIndirectReference;
            var dic = new PdfDictionary();
            if (parent != null)
            {
                dic.Put(PdfName.Parent, parent);
            }

            dic.Put(PdfName.T, new PdfString(name, PdfObject.TEXT_UNICODE));
            var fname2 = $"{fname}.{name}";
            var coidx = _calculationOrder.IndexOf(fname2);
            if (coidx >= 0)
            {
                _calculationOrderRefs[coidx] = ind;
            }

            if (obj is INullValueDictionary<string, object>)
            {
                dic.Put(PdfName.Kids, BranchForm((INullValueDictionary<string, object>)obj, ind, fname2));
                arr.Add(ind);
                AddToBody(dic, ind);
            }
            else
            {
                var list = (List<object>)obj;
                dic.MergeDifferent((PdfDictionary)list[0]);
                if (list.Count == 3)
                {
                    dic.MergeDifferent((PdfDictionary)list[2]);
                    var page = (int)list[1];
                    var pageDic = PageDics[page - 1];
                    var annots = pageDic.GetAsArray(PdfName.Annots);
                    if (annots == null)
                    {
                        annots = new PdfArray();
                        pageDic.Put(PdfName.Annots, annots);
                    }

                    var nn = (PdfNumber)dic.Get(_iTextTag);
                    dic.Remove(_iTextTag);
                    adjustTabOrder(annots, ind, nn);
                }
                else
                {
                    var kids = new PdfArray();
                    for (var k = 1; k < list.Count; k += 2)
                    {
                        var page = (int)list[k];
                        var pageDic = PageDics[page - 1];
                        var annots = pageDic.GetAsArray(PdfName.Annots);
                        if (annots == null)
                        {
                            annots = new PdfArray();
                            pageDic.Put(PdfName.Annots, annots);
                        }

                        var widget = new PdfDictionary();
                        widget.Merge((PdfDictionary)list[k + 1]);
                        widget.Put(PdfName.Parent, ind);
                        var nn = (PdfNumber)widget.Get(_iTextTag);
                        widget.Remove(_iTextTag);
                        var wref = AddToBody(widget).IndirectReference;
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
        for (var k = 0; k < Readers.Count; ++k)
        {
            Readers[k].RemoveFields();
        }

        for (var r = 0; r < Readers.Count; ++r)
        {
            var reader = Readers[r];
            for (var page = 1; page <= reader.NumberOfPages; ++page)
            {
                PageRefs.Add(GetNewReference(reader.GetPageOrigRef(page)));
                PageDics.Add(reader.GetPageN(page));
            }
        }

        MergeFields();
        CreateAcroForms();
        for (var r = 0; r < Readers.Count; ++r)
        {
            var reader = Readers[r];
            for (var page = 1; page <= reader.NumberOfPages; ++page)
            {
                var dic = reader.GetPageN(page);
                var pageRef = GetNewReference(reader.GetPageOrigRef(page));
                var parent = Root.AddPageRef(pageRef);
                dic.Put(PdfName.Parent, parent);
                Propagate(dic, pageRef, false);
            }
        }

        foreach (var entry in Readers2Intrefs)
        {
            var reader = entry.Key;
            try
            {
                File = reader.SafeFile;
                File.ReOpen();
                var t = entry.Value;
                var keys = t.ToOrderedKeys();
                for (var k = 0; k < keys.Count; ++k)
                {
                    var refi = new PrIndirectReference(reader, keys[k]);
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
        {
            return;
        }

        Form = new PdfDictionary();
        Form.Put(PdfName.Dr, Resources);
        Propagate(Resources, null, false);
        Form.Put(PdfName.Da, new PdfString("/Helv 0 Tf 0 g "));
        _tabOrder = new NullValueDictionary<PdfArray, List<int>>();
        _calculationOrderRefs = new List<object>(_calculationOrder);
        Form.Put(PdfName.Fields, BranchForm(FieldTree, null, ""));
        if (_hasSignature)
        {
            Form.Put(PdfName.Sigflags, new PdfNumber(3));
        }

        var co = new PdfArray();
        for (var k = 0; k < _calculationOrderRefs.Count; ++k)
        {
            var obj = _calculationOrderRefs[k];
            if (obj is PdfIndirectReference)
            {
                co.Add((PdfIndirectReference)obj);
            }
        }

        if (co.Size > 0)
        {
            Form.Put(PdfName.Co, co);
        }
    }

    protected override PdfDictionary GetCatalog(PdfIndirectReference rootObj)
    {
        PdfDictionary cat = Pdf.GetCatalog(rootObj);
        if (Form != null)
        {
            var refi = AddToBody(Form).IndirectReference;
            cat.Put(PdfName.Acroform, refi);
        }

        return cat;
    }

    protected PdfIndirectReference GetNewReference(PrIndirectReference refi) =>
        new(0, GetNewObjectNumber(refi.Reader, refi.Number, 0));

    private static string getCoName(PdfReader reader, PrIndirectReference refi)
    {
        var name = "";
        while (refi != null)
        {
            var obj = PdfReader.GetPdfObject(refi);
            if (obj == null || obj.Type != PdfObject.DICTIONARY)
            {
                break;
            }

            var dic = (PdfDictionary)obj;
            var t = dic.GetAsString(PdfName.T);
            if (t != null)
            {
                name = t.ToUnicodeString() + "." + name;
            }

            refi = (PrIndirectReference)dic.Get(PdfName.Parent);
        }

        if (name.EndsWith(".", StringComparison.Ordinal))
        {
            name = name.Substring(0, name.Length - 1);
        }

        return name;
    }

    private void adjustTabOrder(PdfArray annots, PdfIndirectReference ind, PdfNumber nn)
    {
        var v = nn.IntValue;
        var t = _tabOrder[annots];
        if (t == null)
        {
            t = new List<int>();
            var size = annots.Size - 1;
            for (var k = 0; k < size; ++k)
            {
                t.Add(_zero);
            }

            t.Add(v);
            _tabOrder[annots] = t;
            annots.Add(ind);
        }
        else
        {
            var size = t.Count - 1;
            for (var k = size; k >= 0; --k)
            {
                if (t[k] <= v)
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