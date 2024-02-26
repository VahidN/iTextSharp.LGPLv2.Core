using System.util;
using System.Xml;

namespace iTextSharp.text.pdf;

/// <summary>
///     Query and change fields in existing documents either by method
///     calls or by FDF merging.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class AcroFields
{
    public const int DA_COLOR = 2;
    public const int DA_FONT = 0;
    public const int DA_SIZE = 1;

    /// <summary>
    ///     A field type.
    /// </summary>
    public const int FIELD_TYPE_CHECKBOX = 2;

    /// <summary>
    ///     A field type.
    /// </summary>
    public const int FIELD_TYPE_COMBO = 6;

    /// <summary>
    ///     A field type.
    /// </summary>
    public const int FIELD_TYPE_LIST = 5;

    /// <summary>
    ///     A field type invalid or not found.
    /// </summary>
    public const int FIELD_TYPE_NONE = 0;

    /// <summary>
    ///     A field type.
    /// </summary>
    public const int FIELD_TYPE_PUSHBUTTON = 1;

    /// <summary>
    ///     A field type.
    /// </summary>
    public const int FIELD_TYPE_RADIOBUTTON = 3;

    /// <summary>
    ///     A field type.
    /// </summary>
    public const int FIELD_TYPE_SIGNATURE = 7;

    /// <summary>
    ///     A field type.
    /// </summary>
    public const int FIELD_TYPE_TEXT = 4;

    private static readonly PdfName[] _buttonRemove =
    {
        PdfName.Mk, PdfName.F, PdfName.Ff, PdfName.Q, PdfName.Bs, PdfName.Border
    };

    private static readonly INullValueDictionary<string, string[]> _stdFieldFontNames =
        new NullValueDictionary<string, string[]>();

    private readonly bool _append;
    private readonly NullValueDictionary<int, BaseFont> _extensionFonts = new();
    private readonly INullValueDictionary<string, BaseFont> _localFonts = new NullValueDictionary<string, BaseFont>();
    internal readonly PdfReader Reader;
    internal readonly PdfWriter Writer;
    private float _extraMarginLeft;
    private float _extraMarginTop;

    /// <summary>
    ///     Holds value of property fieldCache.
    /// </summary>
    private INullValueDictionary<string, TextField> _fieldCache;

    /// <summary>
    ///     Holds value of property generateAppearances.
    /// </summary>
    private bool _generateAppearances = true;

    private bool _lastWasString;
    private NullValueDictionary<string, int[]> _sigNames;
    private int _topFirst;
    private int _totalRevisions;

    internal INullValueDictionary<string, Item> fields;

    static AcroFields()
    {
        _stdFieldFontNames["CoBO"] = new[]
        {
            "Courier-BoldOblique"
        };

        _stdFieldFontNames["CoBo"] = new[]
        {
            "Courier-Bold"
        };

        _stdFieldFontNames["CoOb"] = new[]
        {
            "Courier-Oblique"
        };

        _stdFieldFontNames["Cour"] = new[]
        {
            "Courier"
        };

        _stdFieldFontNames["HeBO"] = new[]
        {
            "Helvetica-BoldOblique"
        };

        _stdFieldFontNames["HeBo"] = new[]
        {
            "Helvetica-Bold"
        };

        _stdFieldFontNames["HeOb"] = new[]
        {
            "Helvetica-Oblique"
        };

        _stdFieldFontNames["Helv"] = new[]
        {
            "Helvetica"
        };

        _stdFieldFontNames["Symb"] = new[]
        {
            "Symbol"
        };

        _stdFieldFontNames["TiBI"] = new[]
        {
            "Times-BoldItalic"
        };

        _stdFieldFontNames["TiBo"] = new[]
        {
            "Times-Bold"
        };

        _stdFieldFontNames["TiIt"] = new[]
        {
            "Times-Italic"
        };

        _stdFieldFontNames["TiRo"] = new[]
        {
            "Times-Roman"
        };

        _stdFieldFontNames["ZaDb"] = new[]
        {
            "ZapfDingbats"
        };

        _stdFieldFontNames["HySm"] = new[]
        {
            "HYSMyeongJo-Medium", "UniKS-UCS2-H"
        };

        _stdFieldFontNames["HyGo"] = new[]
        {
            "HYGoThic-Medium", "UniKS-UCS2-H"
        };

        _stdFieldFontNames["KaGo"] = new[]
        {
            "HeiseiKakuGo-W5", "UniKS-UCS2-H"
        };

        _stdFieldFontNames["KaMi"] = new[]
        {
            "HeiseiMin-W3", "UniJIS-UCS2-H"
        };

        _stdFieldFontNames["MHei"] = new[]
        {
            "MHei-Medium", "UniCNS-UCS2-H"
        };

        _stdFieldFontNames["MSun"] = new[]
        {
            "MSung-Light", "UniCNS-UCS2-H"
        };

        _stdFieldFontNames["STSo"] = new[]
        {
            "STSong-Light", "UniGB-UCS2-H"
        };
    }

    internal AcroFields(PdfReader reader, PdfWriter writer)
    {
        Reader = reader;
        Writer = writer;
        Xfa = new XfaForm(reader);

        if (writer is PdfStamperImp)
        {
            _append = ((PdfStamperImp)writer).Append;
        }

        Fill();
    }

    /// <summary>
    ///     Sets a cache for field appearances. Parsing the existing PDF to
    ///     create a new TextField is time expensive. For those tasks that repeatedly
    ///     fill the same PDF with different field values the use of the cache has dramatic
    ///     speed advantages. An example usage:
    ///     String pdfFile = ...;// the pdf file used as template
    ///     ArrayList xfdfFiles = ...;// the xfdf file names
    ///     ArrayList pdfOutFiles = ...;// the output file names, one for each element in xpdfFiles
    ///     Hashtable cache = new Hashtable();// the appearances cache
    ///     PdfReader originalReader = new PdfReader(pdfFile);
    ///     for (int k = 0; k &lt; xfdfFiles.Size(); ++k) {
    ///     PdfReader reader = new PdfReader(originalReader);
    ///     XfdfReader xfdf = new XfdfReader((String)xfdfFiles.Get(k));
    ///     PdfStamper stp = new PdfStamper(reader, new FileOutputStream((String)pdfOutFiles.Get(k)));
    ///     AcroFields af = stp.GetAcroFields();
    ///     af.SetFieldCache(cache);
    ///     af.SetFields(xfdf);
    ///     stp.Close();
    ///     }
    /// </summary>
    public INullValueDictionary<string, TextField> FieldCache
    {
        set => _fieldCache = value;
        get => _fieldCache;
    }

    /// <summary>
    ///     Gets all the fields. The fields are keyed by the fully qualified field name and
    ///     the value is an instance of  AcroFields.Item .
    /// </summary>
    /// <returns>all the fields</returns>
    public INullValueDictionary<string, Item> Fields => fields;

    /// <summary>
    ///     Sets the option to generate appearances. Not generating apperances
    ///     will speed-up form filling but the results can be
    ///     unexpected in Acrobat. Don't use it unless your environment is well
    ///     controlled. The default is  true .
    /// </summary>
    public bool GenerateAppearances
    {
        set
        {
            _generateAppearances = value;
            var top = Reader.Catalog.GetAsDict(PdfName.Acroform);

            if (_generateAppearances)
            {
                top.Remove(PdfName.Needappearances);
            }
            else
            {
                top.Put(PdfName.Needappearances, PdfBoolean.Pdftrue);
            }
        }
        get => _generateAppearances;
    }

    /// <summary>
    ///     Sets a list of substitution fonts. The list is composed of  BaseFont  and can also be  null . The fonts in this
    ///     list will be used if the original
    ///     font doesn't contain the needed glyphs.
    /// </summary>
    public IList<BaseFont> SubstitutionFonts { set; get; }

    /// <summary>
    ///     Gets the total number of revisions this document has.
    /// </summary>
    /// <returns>the total number of revisions</returns>
    public int TotalRevisions
    {
        get
        {
            findSignatureNames();

            return _totalRevisions;
        }
    }

    /// <summary>
    ///     Gets the XFA form processor.
    /// </summary>
    /// <returns>the XFA form processor</returns>
    public XfaForm Xfa { get; }

    public static object[] SplitDAelements(string da)
    {
        var tk = new PrTokeniser(PdfEncodings.ConvertToBytes(da, null));
        var stack = new List<string>();
        var ret = new object[3];

        while (tk.NextToken())
        {
            if (tk.TokenType == PrTokeniser.TK_COMMENT)
            {
                continue;
            }

            if (tk.TokenType == PrTokeniser.TK_OTHER)
            {
                var oper = tk.StringValue;

                if (oper.Equals("Tf", StringComparison.Ordinal))
                {
                    if (stack.Count >= 2)
                    {
                        ret[DA_FONT] = stack[stack.Count - 2];
                        ret[DA_SIZE] = float.Parse(stack[stack.Count - 1], NumberFormatInfo.InvariantInfo);
                    }
                }
                else if (oper.Equals("g", StringComparison.Ordinal))
                {
                    if (stack.Count >= 1)
                    {
                        var gray = float.Parse(stack[stack.Count - 1], NumberFormatInfo.InvariantInfo);

                        if (gray.ApproxNotEqual(0))
                        {
                            ret[DA_COLOR] = new GrayColor(gray);
                        }
                    }
                }
                else if (oper.Equals("rg", StringComparison.Ordinal))
                {
                    if (stack.Count >= 3)
                    {
                        var red = float.Parse(stack[stack.Count - 3], NumberFormatInfo.InvariantInfo);
                        var green = float.Parse(stack[stack.Count - 2], NumberFormatInfo.InvariantInfo);
                        var blue = float.Parse(stack[stack.Count - 1], NumberFormatInfo.InvariantInfo);
                        ret[DA_COLOR] = new BaseColor(red, green, blue);
                    }
                }
                else if (oper.Equals("k", StringComparison.Ordinal))
                {
                    if (stack.Count >= 4)
                    {
                        var cyan = float.Parse(stack[stack.Count - 4], NumberFormatInfo.InvariantInfo);
                        var magenta = float.Parse(stack[stack.Count - 3], NumberFormatInfo.InvariantInfo);
                        var yellow = float.Parse(stack[stack.Count - 2], NumberFormatInfo.InvariantInfo);
                        var black = float.Parse(stack[stack.Count - 1], NumberFormatInfo.InvariantInfo);
                        ret[DA_COLOR] = new CmykColor(cyan, magenta, yellow, black);
                    }
                }

                stack.Clear();
            }
            else
            {
                stack.Add(tk.StringValue);
            }
        }

        return ret;
    }

    /// <summary>
    ///     Adds a substitution font to the list. The fonts in this list will be used if the original
    ///     font doesn't contain the needed glyphs.
    /// </summary>
    /// <param name="font">the font</param>
    public void AddSubstitutionFont(BaseFont font)
    {
        if (SubstitutionFonts == null)
        {
            SubstitutionFonts = new List<BaseFont>();
        }

        SubstitutionFonts.Add(font);
    }

    public void DecodeGenericDictionary(PdfDictionary merged, BaseField tx)
    {
        if (merged == null)
        {
            throw new ArgumentNullException(nameof(merged));
        }

        if (tx == null)
        {
            throw new ArgumentNullException(nameof(tx));
        }

        var flags = 0;

        // the text size and color
        var da = merged.GetAsString(PdfName.Da);

        if (da != null)
        {
            var dab = SplitDAelements(da.ToUnicodeString());

            if (dab[DA_SIZE] != null)
            {
                tx.FontSize = (float)dab[DA_SIZE];
            }

            if (dab[DA_COLOR] != null)
            {
                tx.TextColor = (BaseColor)dab[DA_COLOR];
            }

            if (dab[DA_FONT] != null)
            {
                var font = merged.GetAsDict(PdfName.Dr);

                if (font != null)
                {
                    font = font.GetAsDict(PdfName.Font);

                    if (font != null)
                    {
                        var po = font.Get(new PdfName((string)dab[DA_FONT]));

                        if (po != null && po.Type == PdfObject.INDIRECT)
                        {
                            var por = (PrIndirectReference)po;
                            BaseFont bp = new DocumentFont((PrIndirectReference)po);
                            tx.Font = bp;
                            var porkey = por.Number;
                            var porf = _extensionFonts[porkey];

                            if (porf == null)
                            {
                                if (!_extensionFonts.ContainsKey(porkey))
                                {
                                    var fo = (PdfDictionary)PdfReader.GetPdfObject(po);
                                    var fd = fo.GetAsDict(PdfName.Fontdescriptor);

                                    if (fd != null)
                                    {
                                        var prs = (PrStream)PdfReader.GetPdfObject(fd.Get(PdfName.Fontfile2));

                                        if (prs == null)
                                        {
                                            prs = (PrStream)PdfReader.GetPdfObject(fd.Get(PdfName.Fontfile3));
                                        }

                                        if (prs == null)
                                        {
                                            _extensionFonts[porkey] = null;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                porf = BaseFont.CreateFont("font.ttf", BaseFont.IDENTITY_H, true, false,
                                                    PdfReader.GetStreamBytes(prs), null);
                                            }
                                            catch
                                            {
                                            }

                                            _extensionFonts[porkey] = porf;
                                        }
                                    }
                                }
                            }

                            if (tx is TextField)
                            {
                                ((TextField)tx).ExtensionFont = porf;
                            }
                        }
                        else
                        {
                            var bf = _localFonts[(string)dab[DA_FONT]];

                            if (bf == null)
                            {
                                var fn = _stdFieldFontNames[(string)dab[DA_FONT]];

                                if (fn != null)
                                {
                                    try
                                    {
                                        var enc = "winansi";

                                        if (fn.Length > 1)
                                        {
                                            enc = fn[1];
                                        }

                                        bf = BaseFont.CreateFont(fn[0], enc, false);
                                        tx.Font = bf;
                                    }
                                    catch
                                    {
                                        // empty
                                    }
                                }
                            }
                            else
                            {
                                tx.Font = bf;
                            }
                        }
                    }
                }
            }
        }

        //rotation, border and backgound color
        var mk = merged.GetAsDict(PdfName.Mk);

        if (mk != null)
        {
            var ar = mk.GetAsArray(PdfName.Bc);
            var border = GetMkColor(ar);
            tx.BorderColor = border;

            if (border != null)
            {
                tx.BorderWidth = 1;
            }

            ar = mk.GetAsArray(PdfName.Bg);
            tx.BackgroundColor = GetMkColor(ar);
            var rotation = mk.GetAsNumber(PdfName.R);

            if (rotation != null)
            {
                tx.Rotation = rotation.IntValue;
            }
        }

        //flags
        var nfl = merged.GetAsNumber(PdfName.F);
        flags = 0;
        tx.Visibility = BaseField.VISIBLE_BUT_DOES_NOT_PRINT;

        if (nfl != null)
        {
            flags = nfl.IntValue;

            if ((flags & PdfAnnotation.FLAGS_PRINT) != 0 && (flags & PdfAnnotation.FLAGS_HIDDEN) != 0)
            {
                tx.Visibility = BaseField.HIDDEN;
            }
            else if ((flags & PdfAnnotation.FLAGS_PRINT) != 0 && (flags & PdfAnnotation.FLAGS_NOVIEW) != 0)
            {
                tx.Visibility = BaseField.HIDDEN_BUT_PRINTABLE;
            }
            else if ((flags & PdfAnnotation.FLAGS_PRINT) != 0)
            {
                tx.Visibility = BaseField.VISIBLE;
            }
        }

        //multiline
        nfl = merged.GetAsNumber(PdfName.Ff);
        flags = 0;

        if (nfl != null)
        {
            flags = nfl.IntValue;
        }

        tx.Options = flags;

        if ((flags & PdfFormField.FF_COMB) != 0)
        {
            var maxLen = merged.GetAsNumber(PdfName.Maxlen);
            var len = 0;

            if (maxLen != null)
            {
                len = maxLen.IntValue;
            }

            tx.MaxCharacterLength = len;
        }

        //alignment
        nfl = merged.GetAsNumber(PdfName.Q);

        if (nfl != null)
        {
            if (nfl.IntValue == PdfFormField.Q_CENTER)
            {
                tx.Alignment = Element.ALIGN_CENTER;
            }
            else if (nfl.IntValue == PdfFormField.Q_RIGHT)
            {
                tx.Alignment = Element.ALIGN_RIGHT;
            }
        }

        //border styles
        var bs = merged.GetAsDict(PdfName.Bs);

        if (bs != null)
        {
            var w = bs.GetAsNumber(PdfName.W);

            if (w != null)
            {
                tx.BorderWidth = w.FloatValue;
            }

            var s = bs.GetAsName(PdfName.S);

            if (PdfName.D.Equals(s))
            {
                tx.BorderStyle = PdfBorderDictionary.STYLE_DASHED;
            }
            else if (PdfName.B.Equals(s))
            {
                tx.BorderStyle = PdfBorderDictionary.STYLE_BEVELED;
            }
            else if (PdfName.I.Equals(s))
            {
                tx.BorderStyle = PdfBorderDictionary.STYLE_INSET;
            }
            else if (PdfName.U.Equals(s))
            {
                tx.BorderStyle = PdfBorderDictionary.STYLE_UNDERLINE;
            }
        }
        else
        {
            var bd = merged.GetAsArray(PdfName.Border);

            if (bd != null)
            {
                if (bd.Size >= 3)
                {
                    tx.BorderWidth = bd.GetAsNumber(2).FloatValue;
                }

                if (bd.Size >= 4)
                {
                    tx.BorderStyle = PdfBorderDictionary.STYLE_DASHED;
                }
            }
        }
    }

    /// <summary>
    ///     Export the fields as a FDF.
    /// </summary>
    /// <param name="writer">the FDF writer</param>
    public void ExportAsFdf(FdfWriter writer)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        foreach (var entry in fields)
        {
            var item = entry.Value;
            var name = entry.Key;
            var v = item.GetMerged(0).Get(PdfName.V);

            if (v == null)
            {
                continue;
            }

            var value = GetField(name);

            if (_lastWasString)
            {
                writer.SetFieldAsString(name, value);
            }
            else
            {
                writer.SetFieldAsName(name, value);
            }
        }
    }

    /// <summary>
    ///     Extracts a revision from the document.
    ///     it's not a signature field
    ///     @throws IOException on error
    /// </summary>
    /// <param name="field">the signature field name</param>
    /// <returns>an  Stream  covering the revision. Returns  null  if</returns>
    public Stream ExtractRevision(string field)
    {
        findSignatureNames();
        field = GetTranslatedFieldName(field);

        if (!_sigNames.TryGetValue(field, out var value))
        {
            return null;
        }

        var length = value[0];
        var raf = Reader.SafeFile;
        raf.ReOpen();
        raf.Seek(0);

        return new RevisionStream(raf, length);
    }

    /// <summary>
    ///     Gets the list of appearance names. Use it to get the names allowed
    ///     with radio and checkbox fields. If the /Opt key exists the values will
    ///     also be included. The name 'Off' may also be valid
    ///     even if not returned in the list.
    /// </summary>
    /// <param name="fieldName">the fully qualified field name</param>
    /// <returns>the list of names or  null  if the field does not exist</returns>
    public string[] GetAppearanceStates(string fieldName)
    {
        var fd = fields[fieldName];

        if (fd == null)
        {
            return null;
        }

        var names = new NullValueDictionary<string, object>();
        var vals = fd.GetValue(0);
        var stringOpt = vals.GetAsString(PdfName.Opt);

        if (stringOpt != null)
        {
            names[stringOpt.ToUnicodeString()] = null;
        }
        else
        {
            var arrayOpt = vals.GetAsArray(PdfName.Opt);

            if (arrayOpt != null)
            {
                for (var k = 0; k < arrayOpt.Size; ++k)
                {
                    var valStr = arrayOpt.GetAsString(k);

                    if (valStr != null)
                    {
                        names[valStr.ToUnicodeString()] = null;
                    }
                }
            }
        }

        for (var k = 0; k < fd.Size; ++k)
        {
            var dic = fd.GetWidget(k);
            dic = dic.GetAsDict(PdfName.Ap);

            if (dic == null)
            {
                continue;
            }

            dic = dic.GetAsDict(PdfName.N);

            if (dic == null)
            {
                continue;
            }

            foreach (var pname in dic.Keys)
            {
                var name = PdfName.DecodeName(pname.ToString());
                names[name] = null;
            }
        }

        var outs = new string[names.Count];
        names.Keys.CopyTo(outs, 0);

        return outs;
    }

    /// <summary>
    ///     Gets the field names that have blank signatures.
    /// </summary>
    /// <returns>the field names that have blank signatures</returns>
    public IList<string> GetBlankSignatureNames()
    {
        findSignatureNames();
        var sigs = new List<string>();

        foreach (var entry in fields)
        {
            var item = entry.Value;
            var merged = item.GetMerged(0);

            if (!PdfName.Sig.Equals(merged.GetAsName(PdfName.Ft)))
            {
                continue;
            }

            if (_sigNames.ContainsKey(entry.Key))
            {
                continue;
            }

            sigs.Add(entry.Key);
        }

        return sigs;
    }

    /// <summary>
    ///     Gets the field value.
    /// </summary>
    /// <param name="name">the fully qualified field name</param>
    /// <returns>the field value</returns>
    public string GetField(string name)
    {
        if (Xfa.XfaPresent)
        {
            name = Xfa.FindFieldName(name, this);

            if (name == null)
            {
                return null;
            }

            name = XfaForm.Xml2Som.GetShortName(name);

            return XfaForm.GetNodeText(Xfa.FindDatasetsNode(name));
        }

        var item = fields[name];

        if (item == null)
        {
            return null;
        }

        _lastWasString = false;
        var mergedDict = item.GetMerged(0);

        // Jose A. Rodriguez posted a fix to the mailing list (May 11, 2009)
        // explaining that the value can also be a stream value
        // the fix was made against an old iText version. Bruno adapted it.
        var v = PdfReader.GetPdfObject(mergedDict.Get(PdfName.V));

        if (v == null)
        {
            return "";
        }

        if (v is PrStream)
        {
            var valBytes = PdfReader.GetStreamBytes((PrStream)v);

            return PdfEncodings.ConvertToString(valBytes, BaseFont.WINANSI);
        }

        var type = mergedDict.GetAsName(PdfName.Ft);

        if (PdfName.Btn.Equals(type))
        {
            var ff = mergedDict.GetAsNumber(PdfName.Ff);
            var flags = 0;

            if (ff != null)
            {
                flags = ff.IntValue;
            }

            if ((flags & PdfFormField.FF_PUSHBUTTON) != 0)
            {
                return "";
            }

            var value = "";

            if (v is PdfName)
            {
                value = PdfName.DecodeName(v.ToString());
            }
            else if (v is PdfString)
            {
                value = ((PdfString)v).ToUnicodeString();
            }

            var opts = item.GetValue(0).GetAsArray(PdfName.Opt);

            if (opts != null)
            {
                var idx = 0;

                try
                {
                    idx = int.Parse(value, CultureInfo.InvariantCulture);
                    var ps = opts.GetAsString(idx);
                    value = ps.ToUnicodeString();
                    _lastWasString = true;
                }
                catch
                {
                }
            }

            return value;
        }

        if (v is PdfString)
        {
            _lastWasString = true;

            return ((PdfString)v).ToUnicodeString();
        }

        if (v is PdfName)
        {
            return PdfName.DecodeName(v.ToString());
        }

        return "";
    }

    /// <summary>
    ///     Gets the field structure.
    ///     does not exist
    /// </summary>
    /// <param name="name">the name of the field</param>
    /// <returns>the field structure or  null  if the field</returns>
    public Item GetFieldItem(string name)
    {
        if (Xfa.XfaPresent)
        {
            name = Xfa.FindFieldName(name, this);

            if (name == null)
            {
                return null;
            }
        }

        return fields[name];
    }

    /// <summary>
    ///     Gets the field box positions in the document. The return is an array of  float
    ///     multiple of 5. For each of this groups the values are: [page, llx, lly, urx,
    ///     ury]. The coordinates have the page rotation in consideration.
    /// </summary>
    /// <param name="name">the field name</param>
    /// <returns>the positions or  null  if field does not exist</returns>
    public float[] GetFieldPositions(string name)
    {
        var item = GetFieldItem(name);

        if (item == null)
        {
            return null;
        }

        var ret = new float[item.Size * 5];
        var ptr = 0;

        for (var k = 0; k < item.Size; ++k)
        {
            try
            {
                var wd = item.GetWidget(k);
                var rect = wd.GetAsArray(PdfName.Rect);

                if (rect == null)
                {
                    continue;
                }

                var r = PdfReader.GetNormalizedRectangle(rect);
                var page = item.GetPage(k);
                var rotation = Reader.GetPageRotation(page);
                ret[ptr++] = page;

                if (rotation != 0)
                {
                    var pageSize = Reader.GetPageSize(page);

                    switch (rotation)
                    {
                        case 270:
                            r = new Rectangle(pageSize.Top - r.Bottom, r.Left, pageSize.Top - r.Top, r.Right);

                            break;
                        case 180:
                            r = new Rectangle(pageSize.Right - r.Left, pageSize.Top - r.Bottom,
                                pageSize.Right - r.Right, pageSize.Top - r.Top);

                            break;
                        case 90:
                            r = new Rectangle(r.Bottom, pageSize.Right - r.Left, r.Top, pageSize.Right - r.Right);

                            break;
                    }

                    r.Normalize();
                }

                ret[ptr++] = r.Left;
                ret[ptr++] = r.Bottom;
                ret[ptr++] = r.Right;
                ret[ptr++] = r.Top;
            }
            catch
            {
                // empty on purpose
            }
        }

        if (ptr < ret.Length)
        {
            var ret2 = new float[ptr];
            Array.Copy(ret, 0, ret2, 0, ptr);

            return ret2;
        }

        return ret;
    }

    /// <summary>
    ///     Gets the field type. The type can be one of:  FIELD_TYPE_PUSHBUTTON ,
    ///     FIELD_TYPE_CHECKBOX ,  FIELD_TYPE_RADIOBUTTON ,
    ///     FIELD_TYPE_TEXT ,  FIELD_TYPE_LIST ,
    ///     FIELD_TYPE_COMBO  or  FIELD_TYPE_SIGNATURE .
    ///     If the field does not exist or is invalid it returns
    ///     FIELD_TYPE_NONE .
    /// </summary>
    /// <param name="fieldName">the field name</param>
    /// <returns>the field type</returns>
    public int GetFieldType(string fieldName)
    {
        var fd = GetFieldItem(fieldName);

        if (fd == null)
        {
            return FIELD_TYPE_NONE;
        }

        var merged = fd.GetMerged(0);
        var type = merged.GetAsName(PdfName.Ft);

        if (type == null)
        {
            return FIELD_TYPE_NONE;
        }

        var ff = 0;
        var ffo = merged.GetAsNumber(PdfName.Ff);

        if (ffo != null)
        {
            ff = ffo.IntValue;
        }

        if (PdfName.Btn.Equals(type))
        {
            if ((ff & PdfFormField.FF_PUSHBUTTON) != 0)
            {
                return FIELD_TYPE_PUSHBUTTON;
            }

            if ((ff & PdfFormField.FF_RADIO) != 0)
            {
                return FIELD_TYPE_RADIOBUTTON;
            }

            return FIELD_TYPE_CHECKBOX;
        }

        if (PdfName.Tx.Equals(type))
        {
            return FIELD_TYPE_TEXT;
        }

        if (PdfName.Ch.Equals(type))
        {
            if ((ff & PdfFormField.FF_COMBO) != 0)
            {
                return FIELD_TYPE_COMBO;
            }

            return FIELD_TYPE_LIST;
        }

        if (PdfName.Sig.Equals(type))
        {
            return FIELD_TYPE_SIGNATURE;
        }

        return FIELD_TYPE_NONE;
    }

    /// <summary>
    ///     Gets the list of display option values from fields of type list or combo.
    ///     If the field doesn't exist or the field type is not list or combo it will return
    ///     null .
    /// </summary>
    /// <param name="fieldName">the field name</param>
    /// <returns>the list of export option values from fields of type list or combo</returns>
    public string[] GetListOptionDisplay(string fieldName)
        => getListOption(fieldName, 1);

    /// <summary>
    ///     Gets the list of export option values from fields of type list or combo.
    ///     If the field doesn't exist or the field type is not list or combo it will return
    ///     null .
    /// </summary>
    /// <param name="fieldName">the field name</param>
    /// <returns>the list of export option values from fields of type list or combo</returns>
    public string[] GetListOptionExport(string fieldName)
        => getListOption(fieldName, 0);

    /// <summary>
    ///     Gets the field values of a Choice field.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="name">the fully qualified field name</param>
    /// <returns>the field value</returns>
    public string[] GetListSelection(string name)
    {
        var s = GetField(name);

        var ret = s == null
            ? Array.Empty<string>()
            : new[]
            {
                s
            };

        var item = fields[name];

        if (item == null)
        {
            return ret;
        }

        var values = item.GetMerged(0).GetAsArray(PdfName.I);

        if (values == null)
        {
            return ret;
        }

        ret = new string[values.Size];
        var options = GetListOptionExport(name);
        var idx = 0;

        foreach (PdfNumber n in values.ArrayList)
        {
            ret[idx++] = options[n.IntValue];
        }

        return ret;
    }

    /// <summary>
    ///     Creates a new pushbutton from an existing field. If there are several pushbuttons with the same name
    ///     only the first one is used. This pushbutton can be changed and be used to replace
    ///     an existing one, with the same name or other name, as long is it is in the same document. To replace an existing
    ///     pushbutton
    ///     call {@link #replacePushbuttonField(String,PdfFormField)}.
    /// </summary>
    /// <param name="field">the field name that should be a pushbutton</param>
    /// <returns>a new pushbutton or  null  if the field is not a pushbutton</returns>
    public PushbuttonField GetNewPushbuttonFromField(string field)
        => GetNewPushbuttonFromField(field, 0);

    /// <summary>
    ///     Creates a new pushbutton from an existing field. This pushbutton can be changed and be used to replace
    ///     an existing one, with the same name or other name, as long is it is in the same document. To replace an existing
    ///     pushbutton
    ///     call {@link #replacePushbuttonField(String,PdfFormField,int)}.
    /// </summary>
    /// <param name="field">the field name that should be a pushbutton</param>
    /// <param name="order">the field order in fields with same name</param>
    /// <returns>a new pushbutton or  null  if the field is not a pushbutton</returns>
    public PushbuttonField GetNewPushbuttonFromField(string field, int order)
    {
        if (GetFieldType(field) != FIELD_TYPE_PUSHBUTTON)
        {
            return null;
        }

        var item = GetFieldItem(field);

        if (order >= item.Size)
        {
            return null;
        }

        var posi = order * 5;
        var pos = GetFieldPositions(field);
        var box = new Rectangle(pos[posi + 1], pos[posi + 2], pos[posi + 3], pos[posi + 4]);
        var newButton = new PushbuttonField(Writer, box, null);
        var dic = item.GetMerged(order);
        DecodeGenericDictionary(dic, newButton);
        var mk = dic.GetAsDict(PdfName.Mk);

        if (mk != null)
        {
            var text = mk.GetAsString(PdfName.CA);

            if (text != null)
            {
                newButton.Text = text.ToUnicodeString();
            }

            var tp = mk.GetAsNumber(PdfName.Tp);

            if (tp != null)
            {
                newButton.Layout = tp.IntValue + 1;
            }

            var ifit = mk.GetAsDict(PdfName.If);

            if (ifit != null)
            {
                var sw = ifit.GetAsName(PdfName.Sw);

                if (sw != null)
                {
                    var scale = PushbuttonField.SCALE_ICON_ALWAYS;

                    if (sw.Equals(PdfName.B))
                    {
                        scale = PushbuttonField.SCALE_ICON_IS_TOO_BIG;
                    }
                    else if (sw.Equals(PdfName.S))
                    {
                        scale = PushbuttonField.SCALE_ICON_IS_TOO_SMALL;
                    }
                    else if (sw.Equals(PdfName.N))
                    {
                        scale = PushbuttonField.SCALE_ICON_NEVER;
                    }

                    newButton.ScaleIcon = scale;
                }

                sw = ifit.GetAsName(PdfName.S);

                if (sw != null)
                {
                    if (sw.Equals(PdfName.A))
                    {
                        newButton.ProportionalIcon = false;
                    }
                }

                var aj = ifit.GetAsArray(PdfName.A);

                if (aj != null && aj.Size == 2)
                {
                    var left = aj.GetAsNumber(0).FloatValue;
                    var bottom = aj.GetAsNumber(1).FloatValue;
                    newButton.IconHorizontalAdjustment = left;
                    newButton.IconVerticalAdjustment = bottom;
                }

                var fb = ifit.GetAsBoolean(PdfName.Fb);

                if (fb != null && fb.BooleanValue)
                {
                    newButton.IconFitToBounds = true;
                }
            }

            var i = mk.Get(PdfName.I);

            if (i != null && i.IsIndirect())
            {
                newButton.IconReference = (PrIndirectReference)i;
            }
        }

        return newButton;
    }

    /// <summary>
    ///     Gets this  field  revision.
    /// </summary>
    /// <param name="field">the signature field name</param>
    /// <returns>the revision or zero if it's not a signature field</returns>
    public int GetRevision(string field)
    {
        findSignatureNames();
        field = GetTranslatedFieldName(field);

        if (!_sigNames.TryGetValue(field, out var value))
        {
            return 0;
        }

        return value[1];
    }

    /// <summary>
    ///     Gets the signature dictionary, the one keyed by /V.
    ///     a signature
    /// </summary>
    /// <param name="name">the field name</param>
    /// <returns>the signature dictionary keyed by /V or  null  if the field is not</returns>
    public PdfDictionary GetSignatureDictionary(string name)
    {
        findSignatureNames();
        name = GetTranslatedFieldName(name);

        if (!_sigNames.ContainsKey(name))
        {
            return null;
        }

        var item = fields[name];
        var merged = item.GetMerged(0);

        return merged.GetAsDict(PdfName.V);
    }

    /// <summary>
    ///     Gets the field names that have signatures and are signed.
    /// </summary>
    /// <returns>the field names that have signatures and are signed</returns>
    public IList<string> GetSignatureNames()
    {
        findSignatureNames();

        return new List<string>(_sigNames.Keys);
    }

    /// <summary>
    ///     Gets the long XFA translated name.
    /// </summary>
    /// <param name="name">the name of the field</param>
    /// <returns>the long field name</returns>
    public string GetTranslatedFieldName(string name)
    {
        if (Xfa.XfaPresent)
        {
            var namex = Xfa.FindFieldName(name, this);

            if (namex != null)
            {
                name = namex;
            }
        }

        return name;
    }

    /// <summary>
    ///     Merges an XML data structure into this form.
    ///     @throws java.io.IOException on error
    ///     @throws com.lowagie.text.DocumentException o error
    /// </summary>
    /// <param name="n">the top node of the data structure</param>
    public void MergeXfaData(XmlNode n)
    {
        var data = new XfaForm.Xml2SomDatasets(n);

        foreach (var name in data.Order)
        {
            var text = XfaForm.GetNodeText(data.Name2Node[name]);
            SetField(name, text);
        }
    }

    /// <summary>
    ///     Gets the field by name, and positions.
    /// </summary>
    /// <param name="name">the name of the field</param>
    /// <returns>arraylist with value and positions</returns>
    public List<object> GetFieldAndPositions(string name)
    {
        var al = new List<object>
        {
            GetField(name),
            GetFieldPositions(name)
        };

        return al;
    }

    /// <summary>
    ///     Regenerates the field appearance.
    ///     This is usefull when you change a field property, but not its value,
    ///     for instance form.SetFieldProperty("f", "bgcolor", Color.BLUE, null);
    ///     This won't have any effect, unless you use RegenerateField("f") after changing
    ///     the property.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    ///     false  otherwise
    /// </summary>
    /// <param name="name">the fully qualified field name or the partial name in the case of XFA forms</param>
    /// <returns> true  if the field was found and changed,</returns>
    public bool RegenerateField(string name)
    {
        var value = GetField(name);

        return SetField(name, value, value);
    }

    /// <summary>
    ///     Removes a field from the document. If page equals -1 all the fields with this
    ///     name  are removed from the document otherwise only the fields in
    ///     that particular page are removed.
    /// </summary>
    /// <param name="name">the field name</param>
    /// <param name="page">the page to remove the field from or -1 to remove it from all the pages</param>
    /// <returns> true  if the field exists,  false otherwise </returns>
    public bool RemoveField(string name, int page)
    {
        var item = GetFieldItem(name);

        if (item == null)
        {
            return false;
        }

        var acroForm = (PdfDictionary)PdfReader.GetPdfObject(Reader.Catalog.Get(PdfName.Acroform), Reader.Catalog);

        if (acroForm == null)
        {
            return false;
        }

        var arrayf = acroForm.GetAsArray(PdfName.Fields);

        if (arrayf == null)
        {
            return false;
        }

        for (var k = 0; k < item.Size; ++k)
        {
            var pageV = item.GetPage(k);

            if (page != -1 && page != pageV)
            {
                continue;
            }

            var refi = item.GetWidgetRef(k);
            var wd = item.GetWidget(k);
            var pageDic = Reader.GetPageN(pageV);
            var annots = pageDic.GetAsArray(PdfName.Annots);

            if (annots != null)
            {
                if (removeRefFromArray(annots, refi) == 0)
                {
                    pageDic.Remove(PdfName.Annots);
                    markUsed(pageDic);
                }
                else
                {
                    markUsed(annots);
                }
            }

            PdfReader.KillIndirect(refi);
            var kid = refi;

            while ((refi = wd.GetAsIndirectObject(PdfName.Parent)) != null)
            {
                wd = wd.GetAsDict(PdfName.Parent);
                var kids = wd.GetAsArray(PdfName.Kids);

                if (removeRefFromArray(kids, kid) != 0)
                {
                    break;
                }

                kid = refi;
                PdfReader.KillIndirect(refi);
            }

            if (refi == null)
            {
                removeRefFromArray(arrayf, kid);
                markUsed(arrayf);
            }

            if (page != -1)
            {
                item.Remove(k);
                --k;
            }
        }

        if (page == -1 || item.Size == 0)
        {
            fields.Remove(name);
        }

        return true;
    }

    /// <summary>
    ///     Removes a field from the document.
    /// </summary>
    /// <param name="name">the field name</param>
    /// <returns> true  if the field exists,  false otherwise </returns>
    public bool RemoveField(string name)
        => RemoveField(name, -1);

    /// <summary>
    ///     Removes all the fields from  page .
    /// </summary>
    /// <param name="page">the page to remove the fields from</param>
    /// <returns> true  if any field was removed,  false otherwise </returns>
    public bool RemoveFieldsFromPage(int page)
    {
        if (page < 1)
        {
            return false;
        }

        var names = new string[fields.Count];
        fields.Keys.CopyTo(names, 0);
        var found = false;

        for (var k = 0; k < names.Length; ++k)
        {
            var fr = RemoveField(names[k], page);
            found = found || fr;
        }

        return found;
    }

    /// <summary>
    ///     Renames a field. Only the last part of the name can be renamed. For example,
    ///     if the original field is "ab.cd.ef" only the "ef" part can be renamed.
    ///     otherwise
    /// </summary>
    /// <param name="oldName">the old field name</param>
    /// <param name="newName">the new field name</param>
    /// <returns> true  if the renaming was successful,  false </returns>
    public bool RenameField(string oldName, string newName)
    {
        if (oldName == null)
        {
            throw new ArgumentNullException(nameof(oldName));
        }

        if (newName == null)
        {
            throw new ArgumentNullException(nameof(newName));
        }

        var idx1 = oldName.LastIndexOf(".", StringComparison.Ordinal) + 1;
        var idx2 = newName.LastIndexOf(".", StringComparison.Ordinal) + 1;

        if (idx1 != idx2)
        {
            return false;
        }

        if (!oldName.Substring(0, idx1).Equals(newName.Substring(0, idx2), StringComparison.Ordinal))
        {
            return false;
        }

        if (fields.ContainsKey(newName))
        {
            return false;
        }

        var item = fields[oldName];

        if (item == null)
        {
            return false;
        }

        newName = newName.Substring(idx2);
        var ss = new PdfString(newName, PdfObject.TEXT_UNICODE);
        item.WriteToAll(PdfName.T, ss, Item.WRITE_VALUE | Item.WRITE_MERGED);
        item.MarkUsed(this, Item.WRITE_VALUE);
        fields.Remove(oldName);
        fields[newName] = item;

        return true;
    }

    /// <summary>
    ///     Replaces the first field with a new pushbutton. The pushbutton can be created with
    ///     {@link #getNewPushbuttonFromField(String)} from the same document or it can be a
    ///     generic PdfFormField of the type pushbutton.
    ///     was not a pushbutton
    /// </summary>
    /// <param name="field">the field name</param>
    /// <param name="button">the  PdfFormField  representing the pushbutton</param>
    /// <returns> true  if the field was replaced,  false  if the field</returns>
    public bool ReplacePushbuttonField(string field, PdfFormField button)
        => ReplacePushbuttonField(field, button, 0);

    /// <summary>
    ///     Replaces the designated field with a new pushbutton. The pushbutton can be created with
    ///     {@link #getNewPushbuttonFromField(String,int)} from the same document or it can be a
    ///     generic PdfFormField of the type pushbutton.
    ///     was not a pushbutton
    /// </summary>
    /// <param name="field">the field name</param>
    /// <param name="button">the  PdfFormField  representing the pushbutton</param>
    /// <param name="order">the field order in fields with same name</param>
    /// <returns> true  if the field was replaced,  false  if the field</returns>
    public bool ReplacePushbuttonField(string field, PdfFormField button, int order)
    {
        if (button == null)
        {
            throw new ArgumentNullException(nameof(button));
        }

        if (GetFieldType(field) != FIELD_TYPE_PUSHBUTTON)
        {
            return false;
        }

        var item = GetFieldItem(field);

        if (order >= item.Size)
        {
            return false;
        }

        var merged = item.GetMerged(order);
        var values = item.GetValue(order);
        var widgets = item.GetWidget(order);

        for (var k = 0; k < _buttonRemove.Length; ++k)
        {
            merged.Remove(_buttonRemove[k]);
            values.Remove(_buttonRemove[k]);
            widgets.Remove(_buttonRemove[k]);
        }

        foreach (var key in button.Keys)
        {
            if (key.Equals(PdfName.T) || key.Equals(PdfName.Rect))
            {
                continue;
            }

            if (key.Equals(PdfName.Ff))
            {
                values.Put(key, button.Get(key));
            }
            else
            {
                widgets.Put(key, button.Get(key));
            }

            merged.Put(key, button.Get(key));
        }

        return true;
    }

    /// <summary>
    ///     Sets extra margins in text fields to better mimic the Acrobat layout.
    /// </summary>
    /// <param name="extraMarginLeft">the extra marging left</param>
    /// <param name="extraMarginTop">the extra margin top</param>
    public void SetExtraMargin(float extraMarginLeft, float extraMarginTop)
    {
        _extraMarginLeft = extraMarginLeft;
        _extraMarginTop = extraMarginTop;
    }

    /// <summary>
    ///     Sets the field value.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    ///     false  otherwise
    /// </summary>
    /// <param name="name">the fully qualified field name or the partial name in the case of XFA forms</param>
    /// <param name="value">the field value</param>
    /// <returns> true  if the field was found and changed,</returns>
    public bool SetField(string name, string value)
        => SetField(name, value, null);

    /// <summary>
    ///     Sets the field value and the display string. The display string
    ///     is used to build the appearance in the cases where the value
    ///     is modified by Acrobat with JavaScript and the algorithm is
    ///     known.
    ///     the  value  parameter will be used
    ///     false  otherwise
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="name">the fully qualified field name or the partial name in the case of XFA forms</param>
    /// <param name="value">the field value</param>
    /// <param name="display">the string that is used for the appearance. If  null </param>
    /// <returns> true  if the field was found and changed,</returns>
    public bool SetField(string name, string value, string display)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (Writer == null)
        {
            throw new DocumentException("This AcroFields instance is read-only.");
        }

        if (Xfa.XfaPresent)
        {
            name = Xfa.FindFieldName(name, this);

            if (name == null)
            {
                return false;
            }

            var shortName = XfaForm.Xml2Som.GetShortName(name);
            var xn = Xfa.FindDatasetsNode(shortName);

            if (xn == null)
            {
                xn = Xfa.DatasetsSom.InsertNode(Xfa.DatasetsNode, shortName);
            }

            Xfa.SetNodeText(xn, value);
        }

        var item = fields[name];

        if (item == null)
        {
            return false;
        }

        var merged = item.GetMerged(0);
        var type = merged.GetAsName(PdfName.Ft);

        if (PdfName.Tx.Equals(type))
        {
            var maxLen = merged.GetAsNumber(PdfName.Maxlen);
            var len = 0;

            if (maxLen != null)
            {
                len = maxLen.IntValue;
            }

            if (len > 0)
            {
                value = value.Substring(0, Math.Min(len, value.Length));
            }
        }

        if (display == null)
        {
            display = value;
        }

        if (PdfName.Tx.Equals(type) || PdfName.Ch.Equals(type))
        {
            var v = new PdfString(value, PdfObject.TEXT_UNICODE);

            for (var idx = 0; idx < item.Size; ++idx)
            {
                var valueDic = item.GetValue(idx);
                valueDic.Put(PdfName.V, v);
                valueDic.Remove(PdfName.I);
                markUsed(valueDic);
                merged = item.GetMerged(idx);
                merged.Remove(PdfName.I);
                merged.Put(PdfName.V, v);
                var widget = item.GetWidget(idx);

                if (_generateAppearances)
                {
                    var app = GetAppearance(merged, display, name);

                    if (PdfName.Ch.Equals(type))
                    {
                        var n = new PdfNumber(_topFirst);
                        widget.Put(PdfName.Ti, n);
                        merged.Put(PdfName.Ti, n);
                    }

                    var appDic = widget.GetAsDict(PdfName.Ap);

                    if (appDic == null)
                    {
                        appDic = new PdfDictionary();
                        widget.Put(PdfName.Ap, appDic);
                        merged.Put(PdfName.Ap, appDic);
                    }

                    appDic.Put(PdfName.N, app.IndirectReference);
                    Writer.ReleaseTemplate(app);
                }
                else
                {
                    widget.Remove(PdfName.Ap);
                    merged.Remove(PdfName.Ap);
                }

                markUsed(widget);
            }

            return true;
        }

        if (PdfName.Btn.Equals(type))
        {
            var ff = item.GetMerged(0).GetAsNumber(PdfName.Ff);
            var flags = 0;

            if (ff != null)
            {
                flags = ff.IntValue;
            }

            if ((flags & PdfFormField.FF_PUSHBUTTON) != 0)
            {
                //we'll assume that the value is an image in base64
                Image img;

                try
                {
                    img = Image.GetInstance(Convert.FromBase64String(value));
                }
                catch
                {
                    return false;
                }

                var pb = GetNewPushbuttonFromField(name);
                pb.Image = img;
                ReplacePushbuttonField(name, pb.Field);

                return true;
            }

            var v = new PdfName(value);
            var lopt = new List<string>();
            var opts = item.GetValue(0).GetAsArray(PdfName.Opt);

            if (opts != null)
            {
                for (var k = 0; k < opts.Size; ++k)
                {
                    var valStr = opts.GetAsString(k);

                    if (valStr != null)
                    {
                        lopt.Add(valStr.ToUnicodeString());
                    }
                    else
                    {
                        lopt.Add(null);
                    }
                }
            }

            var vidx = lopt.IndexOf(value);
            PdfName valt = null;
            PdfName vt;

            if (vidx >= 0)
            {
                vt = valt = new PdfName(vidx.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                vt = v;
            }

            for (var idx = 0; idx < item.Size; ++idx)
            {
                merged = item.GetMerged(idx);
                var widget = item.GetWidget(idx);
                var valDict = item.GetValue(idx);
                markUsed(item.GetValue(idx));

                if (valt != null)
                {
                    var ps = new PdfString(value, PdfObject.TEXT_UNICODE);
                    valDict.Put(PdfName.V, ps);
                    merged.Put(PdfName.V, ps);
                }
                else
                {
                    valDict.Put(PdfName.V, v);
                    merged.Put(PdfName.V, v);
                }

                markUsed(widget);

                if (IsInAp(widget, vt))
                {
                    merged.Put(PdfName.As, vt);
                    widget.Put(PdfName.As, vt);
                }
                else
                {
                    merged.Put(PdfName.As, PdfName.Off);
                    widget.Put(PdfName.As, PdfName.Off);
                }
            }

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Sets a field property. Valid property names are:
    ///     textfont - sets the text font. The value for this entry is a  BaseFont .
    ///     textcolor - sets the text color. The value for this entry is a  java.awt.Color .
    ///     textsize - sets the text size. The value for this entry is a  Float .
    ///     bgcolor - sets the background color. The value for this entry is a  java.awt.Color .
    ///     If  null  removes the background.
    ///     bordercolor - sets the border color. The value for this entry is a  java.awt.Color .
    ///     If  null  removes the border.
    ///     Set to  null  to process all
    /// </summary>
    /// <param name="field">the field name</param>
    /// <param name="name">the property name</param>
    /// <param name="value">the property value</param>
    /// <param name="inst">an array of  int  indexing into  AcroField.Item.merged  elements to process.</param>
    /// <returns> true  if the property exists,  false  otherwise</returns>
    public bool SetFieldProperty(string field, string name, object value, int[] inst)
    {
        if (Writer == null)
        {
            throw new InvalidOperationException("This AcroFields instance is read-only.");
        }

        var item = fields[field];

        if (item == null)
        {
            return false;
        }

        var hit = new InstHit(inst);
        PdfDictionary merged;
        PdfString da;

        if (Util.EqualsIgnoreCase(name, "textfont"))
        {
            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    merged = item.GetMerged(k);
                    da = merged.GetAsString(PdfName.Da);
                    var dr = merged.GetAsDict(PdfName.Dr);

                    if (da != null && dr != null)
                    {
                        var dao = SplitDAelements(da.ToUnicodeString());
                        var cb = new PdfAppearance();

                        if (dao[DA_FONT] != null)
                        {
                            if (value == null)
                            {
                                throw new ArgumentNullException(nameof(value));
                            }

                            var bf = (BaseFont)value;
                            var psn = PdfAppearance.StdFieldFontNames[bf.PostscriptFontName];

                            if (psn == null)
                            {
                                psn = new PdfName(bf.PostscriptFontName);
                            }

                            var fonts = dr.GetAsDict(PdfName.Font);

                            if (fonts == null)
                            {
                                fonts = new PdfDictionary();
                                dr.Put(PdfName.Font, fonts);
                            }

                            var fref = (PdfIndirectReference)fonts.Get(psn);
                            var top = Reader.Catalog.GetAsDict(PdfName.Acroform);
                            markUsed(top);
                            dr = top.GetAsDict(PdfName.Dr);

                            if (dr == null)
                            {
                                dr = new PdfDictionary();
                                top.Put(PdfName.Dr, dr);
                            }

                            markUsed(dr);
                            var fontsTop = dr.GetAsDict(PdfName.Font);

                            if (fontsTop == null)
                            {
                                fontsTop = new PdfDictionary();
                                dr.Put(PdfName.Font, fontsTop);
                            }

                            markUsed(fontsTop);
                            var frefTop = (PdfIndirectReference)fontsTop.Get(psn);

                            if (frefTop != null)
                            {
                                if (fref == null)
                                {
                                    fonts.Put(psn, frefTop);
                                }
                            }
                            else if (fref == null)
                            {
                                FontDetails fd;

                                if (bf.FontType == BaseFont.FONT_TYPE_DOCUMENT)
                                {
                                    fd = new FontDetails(null, ((DocumentFont)bf).IndirectReference, bf);
                                }
                                else
                                {
                                    bf.Subset = false;
                                    fd = Writer.AddSimple(bf);
                                    _localFonts[psn.ToString().Substring(1)] = bf;
                                }

                                fontsTop.Put(psn, fd.IndirectReference);
                                fonts.Put(psn, fd.IndirectReference);
                            }

                            var buf = cb.InternalBuffer;
                            buf.Append(psn.GetBytes()).Append(' ').Append((float)dao[DA_SIZE]).Append(" Tf ");

                            if (dao[DA_COLOR] != null)
                            {
                                cb.SetColorFill((BaseColor)dao[DA_COLOR]);
                            }

                            var s = new PdfString(cb.ToString());
                            item.GetMerged(k).Put(PdfName.Da, s);
                            item.GetWidget(k).Put(PdfName.Da, s);
                            markUsed(item.GetWidget(k));
                        }
                    }
                }
            }
        }
        else if (Util.EqualsIgnoreCase(name, "textcolor"))
        {
            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    merged = item.GetMerged(k);
                    da = merged.GetAsString(PdfName.Da);

                    if (da != null)
                    {
                        var dao = SplitDAelements(da.ToUnicodeString());
                        var cb = new PdfAppearance();

                        if (dao[DA_FONT] != null)
                        {
                            var buf = cb.InternalBuffer;

                            buf.Append(new PdfName((string)dao[DA_FONT]).GetBytes()).Append(' ')
                                .Append((float)dao[DA_SIZE]).Append(" Tf ");

                            cb.SetColorFill((BaseColor)value);
                            var s = new PdfString(cb.ToString());
                            item.GetMerged(k).Put(PdfName.Da, s);
                            item.GetWidget(k).Put(PdfName.Da, s);
                            markUsed(item.GetWidget(k));
                        }
                    }
                }
            }
        }
        else if (Util.EqualsIgnoreCase(name, "textsize"))
        {
            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    merged = item.GetMerged(k);
                    da = merged.GetAsString(PdfName.Da);

                    if (da != null)
                    {
                        var dao = SplitDAelements(da.ToUnicodeString());
                        var cb = new PdfAppearance();

                        if (dao[DA_FONT] != null)
                        {
                            var buf = cb.InternalBuffer;

                            buf.Append(new PdfName((string)dao[DA_FONT]).GetBytes()).Append(' ').Append((float)value)
                                .Append(" Tf ");

                            if (dao[DA_COLOR] != null)
                            {
                                cb.SetColorFill((BaseColor)dao[DA_COLOR]);
                            }

                            var s = new PdfString(cb.ToString());
                            item.GetMerged(k).Put(PdfName.Da, s);
                            item.GetWidget(k).Put(PdfName.Da, s);
                            markUsed(item.GetWidget(k));
                        }
                    }
                }
            }
        }
        else if (Util.EqualsIgnoreCase(name, "bgcolor") || Util.EqualsIgnoreCase(name, "bordercolor"))
        {
            var dname = Util.EqualsIgnoreCase(name, "bgcolor") ? PdfName.Bg : PdfName.Bc;

            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    merged = item.GetMerged(k);
                    var mk = merged.GetAsDict(PdfName.Mk);

                    if (mk == null)
                    {
                        if (value == null)
                        {
                            return true;
                        }

                        mk = new PdfDictionary();
                        item.GetMerged(k).Put(PdfName.Mk, mk);
                        item.GetWidget(k).Put(PdfName.Mk, mk);
                        markUsed(item.GetWidget(k));
                    }
                    else
                    {
                        markUsed(mk);
                    }

                    if (value == null)
                    {
                        mk.Remove(dname);
                    }
                    else
                    {
                        mk.Put(dname, PdfAnnotation.GetMkColor((BaseColor)value));
                    }
                }
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Sets a field property. Valid property names are:
    ///     flags - a set of flags specifying various characteristics of the field�s widget annotation.
    ///     The value of this entry replaces that of the F entry in the form�s corresponding annotation dictionary.
    ///     setflags - a set of flags to be set (turned on) in the F entry of the form�s corresponding
    ///     widget annotation dictionary. Bits equal to 1 cause the corresponding bits in F to be set to 1.
    ///     clrflags - a set of flags to be cleared (turned off) in the F entry of the form�s corresponding
    ///     widget annotation dictionary. Bits equal to 1 cause the corresponding
    ///     bits in F to be set to 0.
    ///     fflags - a set of flags specifying various characteristics of the field. The value
    ///     of this entry replaces that of the Ff entry in the form�s corresponding field dictionary.
    ///     setfflags - a set of flags to be set (turned on) in the Ff entry of the form�s corresponding
    ///     field dictionary. Bits equal to 1 cause the corresponding bits in Ff to be set to 1.
    ///     clrfflags - a set of flags to be cleared (turned off) in the Ff entry of the form�s corresponding
    ///     field dictionary. Bits equal to 1 cause the corresponding bits in Ff
    ///     to be set to 0.
    ///     Set to  null  to process all
    /// </summary>
    /// <param name="field">the field name</param>
    /// <param name="name">the property name</param>
    /// <param name="value">the property value</param>
    /// <param name="inst">an array of  int  indexing into  AcroField.Item.merged  elements to process.</param>
    /// <returns> true  if the property exists,  false  otherwise</returns>
    public bool SetFieldProperty(string field, string name, int value, int[] inst)
    {
        if (Writer == null)
        {
            throw new InvalidOperationException("This AcroFields instance is read-only.");
        }

        var item = fields[field];

        if (item == null)
        {
            return false;
        }

        var hit = new InstHit(inst);

        if (Util.EqualsIgnoreCase(name, "flags"))
        {
            var num = new PdfNumber(value);

            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    item.GetMerged(k).Put(PdfName.F, num);
                    item.GetWidget(k).Put(PdfName.F, num);
                    markUsed(item.GetWidget(k));
                }
            }
        }
        else if (Util.EqualsIgnoreCase(name, "setflags"))
        {
            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    var num = item.GetWidget(k).GetAsNumber(PdfName.F);
                    var val = 0;

                    if (num != null)
                    {
                        val = num.IntValue;
                    }

                    num = new PdfNumber(val | value);
                    item.GetMerged(k).Put(PdfName.F, num);
                    item.GetWidget(k).Put(PdfName.F, num);
                    markUsed(item.GetWidget(k));
                }
            }
        }
        else if (Util.EqualsIgnoreCase(name, "clrflags"))
        {
            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    var widget = item.GetWidget(k);
                    var num = widget.GetAsNumber(PdfName.F);
                    var val = 0;

                    if (num != null)
                    {
                        val = num.IntValue;
                    }

                    num = new PdfNumber(val & ~value);
                    item.GetMerged(k).Put(PdfName.F, num);
                    widget.Put(PdfName.F, num);
                    markUsed(widget);
                }
            }
        }
        else if (Util.EqualsIgnoreCase(name, "fflags"))
        {
            var num = new PdfNumber(value);

            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    item.GetMerged(k).Put(PdfName.Ff, num);
                    item.GetValue(k).Put(PdfName.Ff, num);
                    markUsed(item.GetValue(k));
                }
            }
        }
        else if (Util.EqualsIgnoreCase(name, "setfflags"))
        {
            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    var valDict = item.GetValue(k);
                    var num = valDict.GetAsNumber(PdfName.Ff);
                    var val = 0;

                    if (num != null)
                    {
                        val = num.IntValue;
                    }

                    num = new PdfNumber(val | value);
                    item.GetMerged(k).Put(PdfName.Ff, num);
                    valDict.Put(PdfName.Ff, num);
                    markUsed(valDict);
                }
            }
        }
        else if (Util.EqualsIgnoreCase(name, "clrfflags"))
        {
            for (var k = 0; k < item.Size; ++k)
            {
                if (hit.IsHit(k))
                {
                    var valDict = item.GetValue(k);
                    var num = valDict.GetAsNumber(PdfName.Ff);
                    var val = 0;

                    if (num != null)
                    {
                        val = num.IntValue;
                    }

                    num = new PdfNumber(val & ~value);
                    item.GetMerged(k).Put(PdfName.Ff, num);
                    valDict.Put(PdfName.Ff, num);
                    markUsed(valDict);
                }
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Sets the fields by FDF merging.
    ///     @throws IOException on error
    ///     @throws DocumentException on error
    /// </summary>
    /// <param name="fdf">the FDF form</param>
    public void SetFields(FdfReader fdf)
    {
        if (fdf == null)
        {
            throw new ArgumentNullException(nameof(fdf));
        }

        var fd = fdf.Fields;

        foreach (var f in fd.Keys)
        {
            var v = fdf.GetFieldValue(f);

            if (v != null)
            {
                SetField(f, v);
            }
        }
    }

    public void SetFields(XfdfReader xfdf)
    {
        if (xfdf == null)
        {
            throw new ArgumentNullException(nameof(xfdf));
        }

        var fd = xfdf.Fields;

        foreach (var f in fd.Keys)
        {
            var v = xfdf.GetFieldValue(f);

            if (v != null)
            {
                SetField(f, v);
            }

            var l = xfdf.GetListValues(f);

            if (l != null)
            {
                var ar = l.ToArray();
                SetListSelection(v, ar);
            }
        }
    }

    /// <summary>
    ///     Sets the option list for fields of type list or combo. One of  exportValues
    ///     or  displayValues  may be  null  but not both. This method will only
    ///     set the list but will not set the value or appearance. For that, calling  setField()
    ///     is required.
    ///     An example:
    ///     PdfReader pdf = new PdfReader("input.pdf");
    ///     PdfStamper stp = new PdfStamper(pdf, new FileOutputStream("output.pdf"));
    ///     AcroFields af = stp.GetAcroFields();
    ///     af.SetListOption("ComboBox", new String[]{"a", "b", "c"}, new String[]{"first", "second", "third"});
    ///     af.SetField("ComboBox", "b");
    ///     stp.Close();
    /// </summary>
    /// <param name="fieldName">the field name</param>
    /// <param name="exportValues">the export values</param>
    /// <param name="displayValues">the display values</param>
    /// <returns> true  if the operation succeeded,  false  otherwise</returns>
    public bool SetListOption(string fieldName, string[] exportValues, string[] displayValues)
    {
        if (exportValues == null && displayValues == null)
        {
            return false;
        }

        if (exportValues != null && displayValues != null && exportValues.Length != displayValues.Length)
        {
            throw new ArgumentException("The export and the display array must have the same size.");
        }

        var ftype = GetFieldType(fieldName);

        if (ftype != FIELD_TYPE_COMBO && ftype != FIELD_TYPE_LIST)
        {
            return false;
        }

        var fd = fields[fieldName];
        string[] sing = null;

        if (exportValues == null && displayValues != null)
        {
            sing = displayValues;
        }
        else if (exportValues != null && displayValues == null)
        {
            sing = exportValues;
        }

        var opt = new PdfArray();

        if (sing != null)
        {
            for (var k = 0; k < sing.Length; ++k)
            {
                opt.Add(new PdfString(sing[k], PdfObject.TEXT_UNICODE));
            }
        }
        else
        {
            if (exportValues != null && displayValues != null)
            {
                for (var k = 0; k < exportValues.Length; ++k)
                {
                    var a = new PdfArray();
                    a.Add(new PdfString(exportValues[k], PdfObject.TEXT_UNICODE));
                    a.Add(new PdfString(displayValues[k], PdfObject.TEXT_UNICODE));
                    opt.Add(a);
                }
            }
        }

        fd.WriteToAll(PdfName.Opt, opt, Item.WRITE_VALUE | Item.WRITE_MERGED);

        return true;
    }

    /// <summary>
    ///     Sets different values in a list selection.
    ///     No appearance is generated yet; nor does the code check if multiple select is allowed.
    ///     @since 2.1.4
    /// </summary>
    /// <param name="name">the name of the field</param>
    /// <param name="value">an array with values that need to be selected</param>
    /// <returns>true only if the field value was changed</returns>
    public bool SetListSelection(string name, string[] value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var item = GetFieldItem(name);

        if (item == null)
        {
            return false;
        }

        var type = item.GetMerged(0).GetAsName(PdfName.Ft);

        if (!PdfName.Ch.Equals(type))
        {
            return false;
        }

        var options = GetListOptionExport(name);
        var array = new PdfArray();

        for (var i = 0; i < value.Length; i++)
        {
            for (var j = 0; j < options.Length; j++)
            {
                if (options[j].Equals(value[i], StringComparison.Ordinal))
                {
                    array.Add(new PdfNumber(j));
                }
            }
        }

        item.WriteToAll(PdfName.I, array, Item.WRITE_MERGED | Item.WRITE_VALUE);
        item.WriteToAll(PdfName.V, null, Item.WRITE_MERGED | Item.WRITE_VALUE);
        item.WriteToAll(PdfName.Ap, null, Item.WRITE_MERGED | Item.WRITE_WIDGET);
        item.MarkUsed(this, Item.WRITE_VALUE | Item.WRITE_WIDGET);

        return true;
    }

    /// <summary>
    ///     Checks is the signature covers the entire document or just part of it.
    ///     false  otherwise
    /// </summary>
    /// <param name="name">the signature field name</param>
    /// <returns> true  if the signature covers the entire document,</returns>
    public bool SignatureCoversWholeDocument(string name)
    {
        findSignatureNames();
        name = GetTranslatedFieldName(name);

        return _sigNames.TryGetValue(name, out var value) && value[0] == Reader.FileLength;
    }

    /// <summary>
    ///     Verifies a signature. An example usage is:
    ///     KeyStore kall = PdfPKCS7.LoadCacertsKeyStore();
    ///     PdfReader reader = new PdfReader("my_signed_doc.pdf");
    ///     AcroFields af = reader.GetAcroFields();
    ///     ArrayList names = af.GetSignatureNames();
    ///     for (int k = 0; k &lt; names.Size(); ++k) {
    ///     String name = (String)names.Get(k);
    ///     System.out.Println("Signature name: " + name);
    ///     System.out.Println("Signature covers whole document: " + af.SignatureCoversWholeDocument(name));
    ///     PdfPKCS7 pk = af.VerifySignature(name);
    ///     Calendar cal = pk.GetSignDate();
    ///     Certificate pkc[] = pk.GetCertificates();
    ///     System.out.Println("Subject: " + PdfPKCS7.GetSubjectFields(pk.GetSigningCertificate()));
    ///     System.out.Println("Document modified: " + !pk.Verify());
    ///     Object fails[] = PdfPKCS7.VerifyCertificates(pkc, kall, null, cal);
    ///     if (fails == null)
    ///     System.out.Println("Certificates verified against the KeyStore");
    ///     else
    ///     System.out.Println("Certificate failed: " + fails[1]);
    ///     }
    /// </summary>
    /// <param name="name">the signature field name</param>
    /// <returns>a  PdfPKCS7  class to continue the verification</returns>
    public PdfPkcs7 VerifySignature(string name)
    {
        var v = GetSignatureDictionary(name);

        if (v == null)
        {
            return null;
        }

        var sub = v.GetAsName(PdfName.Subfilter);
        var contents = v.GetAsString(PdfName.Contents);
        PdfPkcs7 pk = null;

        if (sub.Equals(PdfName.AdbeX509RsaSha1))
        {
            var cert = v.GetAsString(PdfName.Cert);
            pk = new PdfPkcs7(contents.GetOriginalBytes(), cert.GetBytes());
        }
        else
        {
            pk = new PdfPkcs7(contents.GetOriginalBytes());
        }

        updateByteRange(pk, v);
        var str = v.GetAsString(PdfName.M);

        if (str != null)
        {
            pk.SignDate = PdfDate.Decode(str.ToString());
        }

        var obj = PdfReader.GetPdfObject(v.Get(PdfName.Name));

        if (obj != null)
        {
            if (obj.IsString())
            {
                pk.SignName = ((PdfString)obj).ToUnicodeString();
            }
            else if (obj.IsName())
            {
                pk.SignName = PdfName.DecodeName(obj.ToString());
            }
        }

        str = v.GetAsString(PdfName.Reason);

        if (str != null)
        {
            pk.Reason = str.ToUnicodeString();
        }

        str = v.GetAsString(PdfName.Location);

        if (str != null)
        {
            pk.Location = str.ToUnicodeString();
        }

        return pk;
    }

    internal void Fill()
    {
        fields = new NullValueDictionary<string, Item>();
        var top = (PdfDictionary)PdfReader.GetPdfObjectRelease(Reader.Catalog.Get(PdfName.Acroform));

        if (top == null)
        {
            return;
        }

        var arrfds = (PdfArray)PdfReader.GetPdfObjectRelease(top.Get(PdfName.Fields));

        if (arrfds == null || arrfds.Size == 0)
        {
            return;
        }

        for (var k = 1; k <= Reader.NumberOfPages; ++k)
        {
            var page = Reader.GetPageNRelease(k);
            var annots = (PdfArray)PdfReader.GetPdfObjectRelease(page.Get(PdfName.Annots), page);

            if (annots == null)
            {
                continue;
            }

            for (var j = 0; j < annots.Size; ++j)
            {
                var annot = annots.GetAsDict(j);

                if (annot == null)
                {
                    PdfReader.ReleaseLastXrefPartial(annots.GetAsIndirectObject(j));

                    continue;
                }

                if (!PdfName.Widget.Equals(annot.GetAsName(PdfName.Subtype)))
                {
                    PdfReader.ReleaseLastXrefPartial(annots.GetAsIndirectObject(j));

                    continue;
                }

                var widget = annot;
                var dic = new PdfDictionary();
                dic.Merge(annot);
                var name = "";
                PdfDictionary value = null;
                PdfObject lastV = null;

                while (annot != null)
                {
                    dic.MergeDifferent(annot);
                    var t = annot.GetAsString(PdfName.T);

                    if (t != null)
                    {
                        name = t.ToUnicodeString() + "." + name;
                    }

                    if (lastV == null && annot.Get(PdfName.V) != null)
                    {
                        lastV = PdfReader.GetPdfObjectRelease(annot.Get(PdfName.V));
                    }

                    if (value == null && t != null)
                    {
                        value = annot;

                        if (annot.Get(PdfName.V) == null && lastV != null)
                        {
                            value.Put(PdfName.V, lastV);
                        }
                    }

                    annot = annot.GetAsDict(PdfName.Parent);
                }

                if (name.Length > 0)
                {
                    name = name.Substring(0, name.Length - 1);
                }

                var item = fields[name];

                if (item == null)
                {
                    item = new Item();
                    fields[name] = item;
                }

                if (value == null)
                {
                    item.AddValue(widget);
                }
                else
                {
                    item.AddValue(value);
                }

                item.AddWidget(widget);
                item.AddWidgetRef(annots.GetAsIndirectObject(j)); // must be a reference
                dic.MergeDifferent(top);
                item.AddMerged(dic);
                item.AddPage(k);
                item.AddTabOrder(j);
            }
        }

        // some tools produce invisible signatures without an entry in the page annotation array
        // look for a single level annotation
        var sigFlags = top.GetAsNumber(PdfName.Sigflags);

        if (sigFlags == null || (sigFlags.IntValue & 1) != 1)
        {
            return;
        }

        for (var j = 0; j < arrfds.Size; ++j)
        {
            var annot = arrfds.GetAsDict(j);

            if (annot == null)
            {
                PdfReader.ReleaseLastXrefPartial(arrfds.GetAsIndirectObject(j));

                continue;
            }

            if (!PdfName.Widget.Equals(annot.GetAsName(PdfName.Subtype)))
            {
                PdfReader.ReleaseLastXrefPartial(arrfds.GetAsIndirectObject(j));

                continue;
            }

            var kids = (PdfArray)PdfReader.GetPdfObjectRelease(annot.Get(PdfName.Kids));

            if (kids != null)
            {
                continue;
            }

            var dic = new PdfDictionary();
            dic.Merge(annot);
            var t = annot.GetAsString(PdfName.T);

            if (t == null)
            {
                continue;
            }

            var name = t.ToUnicodeString();

            if (fields.ContainsKey(name))
            {
                continue;
            }

            var item = new Item();
            fields[name] = item;
            item.AddValue(dic);
            item.AddWidget(dic);
            item.AddWidgetRef(arrfds.GetAsIndirectObject(j)); // must be a reference
            item.AddMerged(dic);
            item.AddPage(-1);
            item.AddTabOrder(-1);
        }
    }

    internal PdfAppearance GetAppearance(PdfDictionary merged, string text, string fieldName)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        _topFirst = 0;
        TextField tx = null;

        if (_fieldCache == null || !_fieldCache.TryGetValue(fieldName, out var fcValue))
        {
            tx = new TextField(Writer, null, null);
            tx.SetExtraMargin(_extraMarginLeft, _extraMarginTop);
            tx.BorderWidth = 0;
            tx.SubstitutionFonts = SubstitutionFonts;
            DecodeGenericDictionary(merged, tx);

            //rect
            var rect = merged.GetAsArray(PdfName.Rect);
            var box = PdfReader.GetNormalizedRectangle(rect);

            if (tx.Rotation == 90 || tx.Rotation == 270)
            {
                box = box.Rotate();
            }

            tx.Box = box;

            if (_fieldCache != null)
            {
                _fieldCache[fieldName] = tx;
            }
        }
        else
        {
            tx = fcValue;
            tx.Writer = Writer;
        }

        var fieldType = merged.GetAsName(PdfName.Ft);

        if (PdfName.Tx.Equals(fieldType))
        {
            tx.Text = text;

            return tx.GetAppearance();
        }

        if (!PdfName.Ch.Equals(fieldType))
        {
            throw new DocumentException("An appearance was requested without a variable text field.");
        }

        var opt = merged.GetAsArray(PdfName.Opt);
        var flags = 0;
        var nfl = merged.GetAsNumber(PdfName.Ff);

        if (nfl != null)
        {
            flags = nfl.IntValue;
        }

        if ((flags & PdfFormField.FF_COMBO) != 0 && opt == null)
        {
            tx.Text = text;

            return tx.GetAppearance();
        }

        if (opt != null)
        {
            var choices = new string[opt.Size];
            var choicesExp = new string[opt.Size];

            for (var k = 0; k < opt.Size; ++k)
            {
                var obj = opt[k];

                if (obj.IsString())
                {
                    choices[k] = choicesExp[k] = ((PdfString)obj).ToUnicodeString();
                }
                else
                {
                    var a = (PdfArray)obj;
                    choicesExp[k] = a.GetAsString(0).ToUnicodeString();
                    choices[k] = a.GetAsString(1).ToUnicodeString();
                }
            }

            if ((flags & PdfFormField.FF_COMBO) != 0)
            {
                for (var k = 0; k < choices.Length; ++k)
                {
                    if (text.Equals(choicesExp[k], StringComparison.Ordinal))
                    {
                        text = choices[k];

                        break;
                    }
                }

                tx.Text = text;

                return tx.GetAppearance();
            }

            var idx = 0;

            for (var k = 0; k < choicesExp.Length; ++k)
            {
                if (text.Equals(choicesExp[k], StringComparison.Ordinal))
                {
                    idx = k;

                    break;
                }
            }

            tx.Choices = choices;
            tx.ChoiceExports = choicesExp;
            tx.ChoiceSelection = idx;
        }

        var app = tx.GetListAppearance();
        _topFirst = tx.TopFirst;

        return app;
    }

    internal static BaseColor GetMkColor(PdfArray ar)
    {
        if (ar == null)
        {
            return null;
        }

        switch (ar.Size)
        {
            case 1:
                return new GrayColor(ar.GetAsNumber(0).FloatValue);
            case 3:
                return new BaseColor(ExtendedColor.Normalize(ar.GetAsNumber(0).FloatValue),
                    ExtendedColor.Normalize(ar.GetAsNumber(1).FloatValue),
                    ExtendedColor.Normalize(ar.GetAsNumber(2).FloatValue));
            case 4:
                return new CmykColor(ar.GetAsNumber(0).FloatValue, ar.GetAsNumber(1).FloatValue,
                    ar.GetAsNumber(2).FloatValue, ar.GetAsNumber(3).FloatValue);
            default:
                return null;
        }
    }

    internal static bool IsInAp(PdfDictionary dic, PdfName check)
    {
        var appDic = dic.GetAsDict(PdfName.Ap);

        if (appDic == null)
        {
            return false;
        }

        var nDic = appDic.GetAsDict(PdfName.N);

        return nDic != null && nDic.Get(check) != null;
    }

    private void findSignatureNames()
    {
        if (_sigNames != null)
        {
            return;
        }

        _sigNames = new NullValueDictionary<string, int[]>();
        var sorter = new List<object[]>();

        foreach (var entry in fields)
        {
            var item = entry.Value;
            var merged = item.GetMerged(0);

            if (!PdfName.Sig.Equals(merged.Get(PdfName.Ft)))
            {
                continue;
            }

            var v = merged.GetAsDict(PdfName.V);

            if (v == null)
            {
                continue;
            }

            var contents = v.GetAsString(PdfName.Contents);

            if (contents == null)
            {
                continue;
            }

            var ro = v.GetAsArray(PdfName.Byterange);

            if (ro == null)
            {
                continue;
            }

            var rangeSize = ro.Size;

            if (rangeSize < 2)
            {
                continue;
            }

            var length = ro.GetAsNumber(rangeSize - 1).IntValue + ro.GetAsNumber(rangeSize - 2).IntValue;

            sorter.Add(new object[]
            {
                entry.Key, new[]
                {
                    length, 0
                }
            });
        }

        sorter.Sort(new SorterComparator());

        if (sorter.Count > 0)
        {
            if (((int[])sorter[sorter.Count - 1][1])[0] == Reader.FileLength)
            {
                _totalRevisions = sorter.Count;
            }
            else
            {
                _totalRevisions = sorter.Count + 1;
            }

            for (var k = 0; k < sorter.Count; ++k)
            {
                var objs = sorter[k];
                var name = (string)objs[0];
                var p = (int[])objs[1];
                p[1] = k + 1;
                _sigNames[name] = p;
            }
        }
    }

    private string[] getListOption(string fieldName, int idx)
    {
        var fd = GetFieldItem(fieldName);

        if (fd == null)
        {
            return null;
        }

        var ar = fd.GetMerged(0).GetAsArray(PdfName.Opt);

        if (ar == null)
        {
            return null;
        }

        var ret = new string[ar.Size];

        for (var k = 0; k < ar.Size; ++k)
        {
            var obj = ar.GetDirectObject(k);

            try
            {
                if (obj.IsArray())
                {
                    obj = ((PdfArray)obj).GetDirectObject(idx);
                }

                if (obj.IsString())
                {
                    ret[k] = ((PdfString)obj).ToUnicodeString();
                }
                else
                {
                    ret[k] = obj.ToString();
                }
            }
            catch
            {
                ret[k] = "";
            }
        }

        return ret;
    }

    private void markUsed(PdfObject obj)
    {
        if (!_append)
        {
            return;
        }

        ((PdfStamperImp)Writer).MarkUsed(obj);
    }

    private static int removeRefFromArray(PdfArray array, PdfObject refo)
    {
        if (refo == null || !refo.IsIndirect())
        {
            return array.Size;
        }

        var refi = (PdfIndirectReference)refo;

        for (var j = 0; j < array.Size; ++j)
        {
            var obj = array[j];

            if (!obj.IsIndirect())
            {
                continue;
            }

            if (((PdfIndirectReference)obj).Number == refi.Number)
            {
                array.Remove(j--);
            }
        }

        return array.Size;
    }

    private void updateByteRange(PdfPkcs7 pkcs7, PdfDictionary v)
    {
        var b = v.GetAsArray(PdfName.Byterange);
        var rf = Reader.SafeFile;

        try
        {
            rf.ReOpen();
            var buf = new byte[8192];

            for (var k = 0; k < b.Size; ++k)
            {
                var start = b.GetAsNumber(k).IntValue;
                var length = b.GetAsNumber(++k).IntValue;
                rf.Seek(start);

                while (length > 0)
                {
                    var rd = rf.Read(buf, 0, Math.Min(length, buf.Length));

                    if (rd <= 0)
                    {
                        break;
                    }

                    length -= rd;
                    pkcs7.Update(buf, 0, rd);
                }
            }
        }
        finally
        {
            try
            {
                rf.Close();
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///     The field representations for retrieval and modification.
    /// </summary>
    public class Item
    {
        /// <summary>
        ///     writeToAll  constant.
        ///     @since 2.1.5
        /// </summary>
        public const int WRITE_MERGED = 1;

        /// <summary>
        ///     writeToAll  and  markUsed  constant.
        ///     @since 2.1.5
        /// </summary>
        public const int WRITE_VALUE = 4;

        /// <summary>
        ///     writeToAll  and  markUsed  constant.
        ///     @since 2.1.5
        /// </summary>
        public const int WRITE_WIDGET = 2;

        /// <summary>
        ///     An array of  PdfDictionary  with all the field
        ///     and widget tags merged.
        ///     @deprecated (will remove 'public' in the future)
        /// </summary>
        public List<PdfDictionary> Merged = new();

        /// <summary>
        ///     An array of  Integer  with the page numbers where
        ///     the widgets are displayed.
        ///     @deprecated (will remove 'public' in the future)
        /// </summary>
        public List<int> Page = new();

        /// <summary>
        ///     An array of  Integer  with the tab order of the field in the page.
        ///     @deprecated (will remove 'public' in the future)
        /// </summary>
        public List<int> TabOrder = new();

        /// <summary>
        ///     An array of  PdfDictionary  where the value tag /V
        ///     is present.
        ///     @deprecated (will remove 'public' in the future)
        /// </summary>
        public List<PdfDictionary> Values = new();

        /// <summary>
        ///     An array of  PdfDictionary  with the widget references.
        ///     @deprecated (will remove 'public' in the future)
        /// </summary>
        public List<PdfIndirectReference> WidgetRefs = new();

        /// <summary>
        ///     An array of  PdfDictionary  with the widgets.
        ///     @deprecated (will remove 'public' in the future)
        /// </summary>
        public List<PdfDictionary> Widgets = new();

        /// <summary>
        ///     Preferred method of determining the number of instances
        ///     of a given field.
        ///     @since 2.1.5
        /// </summary>
        /// <returns>number of instances</returns>
        public int Size => Values.Count;

        /// <summary>
        ///     Retrieve the merged dictionary for the given instance.  The merged
        ///     dictionary contains all the keys present in parent fields, though they
        ///     may have been overwritten (or modified?) by children.
        ///     Example: a merged radio field dict will contain /V
        ///     @since 2.1.5
        /// </summary>
        /// <param name="idx">instance index</param>
        /// <returns>the merged dictionary for the given instance</returns>
        public PdfDictionary GetMerged(int idx)
            => Merged[idx];

        /// <summary>
        ///     Retrieve the page number of the given instance
        ///     @since 2.1.5
        /// </summary>
        /// <param name="idx"></param>
        /// <returns>remember, pages are "1-indexed", not "0-indexed" like field instances.</returns>
        public int GetPage(int idx)
            => Page[idx];

        /// <summary>
        ///     Gets the tabOrder.
        ///     @since 2.1.5
        /// </summary>
        /// <param name="idx"></param>
        /// <returns>tab index of the given field instance</returns>
        public int GetTabOrder(int idx)
            => TabOrder[idx];

        /// <summary>
        ///     Retrieve the value dictionary of the given instance
        ///     @since 2.1.5
        /// </summary>
        /// <param name="idx">instance index</param>
        /// <returns>dictionary storing this instance's value. It may be shared across instances.</returns>
        public PdfDictionary GetValue(int idx)
            => Values[idx];

        /// <summary>
        ///     Retrieve the widget dictionary of the given instance
        ///     @since 2.1.5
        /// </summary>
        /// <param name="idx">instance index</param>
        /// <returns>The dictionary found in the appropriate page's Annot array.</returns>
        public PdfDictionary GetWidget(int idx)
            => Widgets[idx];

        /// <summary>
        ///     Retrieve the reference to the given instance
        ///     @since 2.1.5
        /// </summary>
        /// <param name="idx">instance index</param>
        /// <returns>reference to the given field instance</returns>
        public PdfIndirectReference GetWidgetRef(int idx)
            => WidgetRefs[idx];

        /// <summary>
        ///     Mark all the item dictionaries used matching the given flags
        ///     @since 2.1.5
        /// </summary>
        /// <param name="parentFields"></param>
        /// <param name="writeFlags">WRITE_MERGED is ignored</param>
        public void MarkUsed(AcroFields parentFields, int writeFlags)
        {
            if (parentFields == null)
            {
                throw new ArgumentNullException(nameof(parentFields));
            }

            if ((writeFlags & WRITE_VALUE) != 0)
            {
                for (var i = 0; i < Size; ++i)
                {
                    parentFields.markUsed(GetValue(i));
                }
            }

            if ((writeFlags & WRITE_WIDGET) != 0)
            {
                for (var i = 0; i < Size; ++i)
                {
                    parentFields.markUsed(GetWidget(i));
                }
            }
        }

        /// <summary>
        ///     This function writes the given key/value pair to all the instances
        ///     of merged, widget, and/or value, depending on the  writeFlags  setting
        ///     @since 2.1.5
        /// </summary>
        /// <param name="key">you'll never guess what this is for.</param>
        /// <param name="value">if value is null, the key will be removed</param>
        /// <param name="writeFlags">ORed together WRITE_* flags</param>
        public void WriteToAll(PdfName key, PdfObject value, int writeFlags)
        {
            int i;
            PdfDictionary curDict = null;

            if ((writeFlags & WRITE_MERGED) != 0)
            {
                for (i = 0; i < Merged.Count; ++i)
                {
                    curDict = GetMerged(i);
                    curDict.Put(key, value);
                }
            }

            if ((writeFlags & WRITE_WIDGET) != 0)
            {
                for (i = 0; i < Widgets.Count; ++i)
                {
                    curDict = GetWidget(i);
                    curDict.Put(key, value);
                }
            }

            if ((writeFlags & WRITE_VALUE) != 0)
            {
                for (i = 0; i < Values.Count; ++i)
                {
                    curDict = GetValue(i);
                    curDict.Put(key, value);
                }
            }
        }

        /// <summary>
        ///     Adds a merged dictionary to this Item.
        ///     @since 2.1.5
        /// </summary>
        /// <param name="mergeDict"></param>
        internal void AddMerged(PdfDictionary mergeDict)
            => Merged.Add(mergeDict);

        /// <summary>
        ///     Adds a page to the current Item.
        ///     @since 2.1.5
        /// </summary>
        /// <param name="pg"></param>
        internal void AddPage(int pg)
            => Page.Add(pg);

        /// <summary>
        ///     Adds a tab order value to this Item.
        ///     @since 2.1.5
        /// </summary>
        /// <param name="order"></param>
        internal void AddTabOrder(int order)
            => TabOrder.Add(order);

        /// <summary>
        ///     Add a value dict to this Item
        ///     @since 2.1.5
        /// </summary>
        /// <param name="value">new value dictionary</param>
        internal void AddValue(PdfDictionary value)
            => Values.Add(value);

        /// <summary>
        ///     Add a widget dict to this Item
        ///     @since 2.1.5
        /// </summary>
        /// <param name="widget"></param>
        internal void AddWidget(PdfDictionary widget)
            => Widgets.Add(widget);

        /// <summary>
        ///     Add a widget ref to this Item
        ///     @since 2.1.5
        /// </summary>
        /// <param name="widgRef"></param>
        internal void AddWidgetRef(PdfIndirectReference widgRef)
            => WidgetRefs.Add(widgRef);

        /// <summary>
        ///     forces a page value into the Item.
        ///     @since 2.1.5
        /// </summary>
        internal void ForcePage(int idx, int pg)
            => Page[idx] = pg;

        /// <summary>
        ///     Remove the given instance from this item.  It is possible to
        ///     remove all instances using this function.
        ///     @since 2.1.5
        /// </summary>
        /// <param name="killIdx"></param>
        internal void Remove(int killIdx)
        {
            Values.RemoveAt(killIdx);
            Widgets.RemoveAt(killIdx);
            WidgetRefs.RemoveAt(killIdx);
            Merged.RemoveAt(killIdx);
            Page.RemoveAt(killIdx);
            TabOrder.RemoveAt(killIdx);
        }
    }

    public class RevisionStream : Stream
    {
        private readonly byte[] _b = new byte[1];
        private readonly int _length;
        private readonly RandomAccessFileOrArray _raf;
        private bool _closed;
        private int _rangePosition;

        internal RevisionStream(RandomAccessFileOrArray raf, int length)
        {
            _raf = raf;
            _length = length;
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

#if NETSTANDARD1_3
            public void Close()
#else
        public override void Close()
#endif
        {
            if (!_closed)
            {
                _raf.Close();
                _closed = true;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0 || offset > buffer.Length || count < 0 || offset + count > buffer.Length ||
                offset + count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset));
            }

            if (count == 0)
            {
                return 0;
            }

            if (_rangePosition >= _length)
            {
                Close();

                return -1;
            }

            var elen = Math.Min(count, _length - _rangePosition);
            _raf.ReadFully(buffer, offset, elen);
            _rangePosition += elen;

            return elen;
        }

        public override int ReadByte()
        {
            var n = Read(_b, 0, 1);

            if (n != 1)
            {
                return -1;
            }

            return _b[0] & 0xff;
        }

        public override long Seek(long offset, SeekOrigin origin)
            => 0;

        public override void SetLength(long value)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
        }
    }

    private class InstHit
    {
        private readonly NullValueDictionary<int, int> _hits;

        public InstHit(int[] inst)
        {
            if (inst == null)
            {
                return;
            }

            _hits = new NullValueDictionary<int, int>();

            for (var k = 0; k < inst.Length; ++k)
            {
                _hits[inst[k]] = 1;
            }
        }

        public bool IsHit(int n)
        {
            if (_hits == null)
            {
                return true;
            }

            return _hits.ContainsKey(n);
        }
    }

    private class SorterComparator : IComparer<object[]>
    {
        public int Compare(object[] o1, object[] o2)
        {
            var n1 = ((int[])o1[1])[0];
            var n2 = ((int[])o2[1])[0];

            return n1 - n2;
        }
    }
}