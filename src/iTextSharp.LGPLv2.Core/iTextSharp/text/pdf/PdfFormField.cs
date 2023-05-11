namespace iTextSharp.text.pdf;

/// <summary>
///     Implements form fields.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfFormField : PdfAnnotation
{
    public const int FF_COMB = 16777216;
    public const int FF_COMBO = 131072;
    public const int FF_DONOTSCROLL = 8388608;
    public const int FF_DONOTSPELLCHECK = 4194304;
    public const int FF_EDIT = 262144;
    public const int FF_FILESELECT = 1048576;
    public const int FF_MULTILINE = 4096;
    public const int FF_MULTISELECT = 2097152;
    public const int FF_NO_EXPORT = 4;
    public const int FF_NO_TOGGLE_TO_OFF = 16384;
    public const int FF_PASSWORD = 8192;
    public const int FF_PUSHBUTTON = 65536;
    public const int FF_RADIO = 32768;
    public const int FF_RADIOSINUNISON = 1 << 25;
    public const int FF_READ_ONLY = 1;
    public const int FF_REQUIRED = 2;
    public const int MK_CAPTION_ABOVE = 3;
    public const int MK_CAPTION_BELOW = 2;
    public const int MK_CAPTION_LEFT = 5;
    public const int MK_CAPTION_OVERLAID = 6;
    public const int MK_CAPTION_RIGHT = 4;
    public const int MK_NO_CAPTION = 1;
    public const int MK_NO_ICON = 0;
    public const bool MULTILINE = true;
    public const bool PASSWORD = true;
    public const bool PLAINTEXT = false;
    public const int Q_CENTER = 1;
    public const int Q_LEFT = 0;
    public const int Q_RIGHT = 2;
    public const bool SINGLELINE = false;
    public static readonly PdfName IfScaleAlways = PdfName.A;
    public static readonly PdfName IfScaleAnamorphic = PdfName.A;
    public static readonly PdfName IfScaleBigger = PdfName.B;
    public static readonly PdfName IfScaleNever = PdfName.N;
    public static readonly PdfName IfScaleProportional = PdfName.P;
    public static readonly PdfName IfScaleSmaller = PdfName.S;
    public static PdfName[] MergeTarget = { PdfName.Font, PdfName.Xobject, PdfName.Colorspace, PdfName.Pattern };

    internal List<PdfFormField> kids;

    /// <summary>
    ///     Holds value of property parent.
    /// </summary>
    internal PdfFormField parent;

    /// <summary>
    ///     Constructs a new  PdfAnnotation  of subtype link (Action).
    /// </summary>
    public PdfFormField(PdfWriter writer, float llx, float lly, float urx, float ury, PdfAction action) :
        base(writer, llx, lly, urx, ury, action)
    {
        Put(PdfName.TYPE, PdfName.Annot);
        Put(PdfName.Subtype, PdfName.Widget);
        Annotation = true;
    }

    /// <summary>
    ///     Creates new PdfFormField
    /// </summary>
    internal PdfFormField(PdfWriter writer) : base(writer, null)
    {
        Form = true;
        Annotation = false;
    }

    public int Button
    {
        set
        {
            Put(PdfName.Ft, PdfName.Btn);
            if (value != 0)
            {
                Put(PdfName.Ff, new PdfNumber(value));
            }
        }
    }

    public string DefaultValueAsName
    {
        set => Put(PdfName.Dv, new PdfName(value));
    }

    public string DefaultValueAsString
    {
        set => Put(PdfName.Dv, new PdfString(value, TEXT_UNICODE));
    }

    public string FieldName
    {
        set
        {
            if (value != null)
            {
                Put(PdfName.T, new PdfString(value, TEXT_UNICODE));
            }
        }
    }

    public IList<PdfFormField> Kids => kids;

    public string MappingName
    {
        set => Put(PdfName.Tm, new PdfString(value, TEXT_UNICODE));
    }

    /// <summary>
    ///     Getter for property parent.
    /// </summary>
    /// <returns>Value of property parent.</returns>
    public PdfFormField Parent => parent;

    public int Quadding
    {
        set => Put(PdfName.Q, new PdfNumber(value));
    }

    public string UserName
    {
        set => Put(PdfName.Tu, new PdfString(value, TEXT_UNICODE));
    }

    public string ValueAsName
    {
        set => Put(PdfName.V, new PdfName(value));
    }

    public PdfSignature ValueAsSig
    {
        set => Put(PdfName.V, value);
    }

    public string ValueAsString
    {
        set => Put(PdfName.V, new PdfString(value, TEXT_UNICODE));
    }

    public static PdfFormField CreateCheckBox(PdfWriter writer) => CreateButton(writer, 0);

    public static PdfFormField CreateCombo(PdfWriter writer, bool edit, string[] options, int topIndex) =>
        CreateChoice(writer, FF_COMBO + (edit ? FF_EDIT : 0), ProcessOptions(options), topIndex);

    public static PdfFormField CreateCombo(PdfWriter writer, bool edit, string[,] options, int topIndex) =>
        CreateChoice(writer, FF_COMBO + (edit ? FF_EDIT : 0), ProcessOptions(options), topIndex);

    public static PdfFormField CreateEmpty(PdfWriter writer)
    {
        var field = new PdfFormField(writer);
        return field;
    }

    public static PdfFormField CreateList(PdfWriter writer, string[] options, int topIndex) =>
        CreateChoice(writer, 0, ProcessOptions(options), topIndex);

    public static PdfFormField CreateList(PdfWriter writer, string[,] options, int topIndex) =>
        CreateChoice(writer, 0, ProcessOptions(options), topIndex);

    public static PdfFormField CreatePushButton(PdfWriter writer) => CreateButton(writer, FF_PUSHBUTTON);

    public static PdfFormField CreateRadioButton(PdfWriter writer, bool noToggleToOff) =>
        CreateButton(writer, FF_RADIO + (noToggleToOff ? FF_NO_TOGGLE_TO_OFF : 0));

    public static PdfFormField CreateSignature(PdfWriter writer)
    {
        var field = new PdfFormField(writer);
        field.Put(PdfName.Ft, PdfName.Sig);
        return field;
    }

    public static PdfFormField CreateTextField(PdfWriter writer, bool multiline, bool password, int maxLen)
    {
        var field = new PdfFormField(writer);
        field.Put(PdfName.Ft, PdfName.Tx);
        var flags = multiline ? FF_MULTILINE : 0;
        flags += password ? FF_PASSWORD : 0;
        field.Put(PdfName.Ff, new PdfNumber(flags));
        if (maxLen > 0)
        {
            field.Put(PdfName.Maxlen, new PdfNumber(maxLen));
        }

        return field;
    }

    public void AddKid(PdfFormField field)
    {
        if (field == null)
        {
            throw new ArgumentNullException(nameof(field));
        }

        field.parent = this;
        if (kids == null)
        {
            kids = new List<PdfFormField>();
        }

        kids.Add(field);
    }

    public int SetFieldFlags(int flags)
    {
        var obj = (PdfNumber)Get(PdfName.Ff);
        int old;
        if (obj == null)
        {
            old = 0;
        }
        else
        {
            old = obj.IntValue;
        }

        var v = old | flags;
        Put(PdfName.Ff, new PdfNumber(v));
        return old;
    }

    public override void SetUsed()
    {
        Used = true;
        if (parent != null)
        {
            Put(PdfName.Parent, parent.IndirectReference);
        }

        if (kids != null)
        {
            var array = new PdfArray();
            for (var k = 0; k < kids.Count; ++k)
            {
                array.Add(kids[k].IndirectReference);
            }

            Put(PdfName.Kids, array);
        }

        if (templates == null)
        {
            return;
        }

        var dic = new PdfDictionary();
        foreach (var template in templates.Keys)
        {
            MergeResources(dic, (PdfDictionary)template.Resources);
        }

        Put(PdfName.Dr, dic);
    }

    public void SetWidget(Rectangle rect, PdfName highlight)
    {
        Put(PdfName.TYPE, PdfName.Annot);
        Put(PdfName.Subtype, PdfName.Widget);
        Put(PdfName.Rect, new PdfRectangle(rect));
        Annotation = true;
        if (highlight != null && !highlight.Equals(HighlightInvert))
        {
            Put(PdfName.H, highlight);
        }
    }

    internal static void MergeResources(PdfDictionary result, PdfDictionary source, PdfStamperImp writer)
    {
        PdfDictionary dic = null;
        PdfDictionary res = null;
        PdfName target = null;
        for (var k = 0; k < MergeTarget.Length; ++k)
        {
            target = MergeTarget[k];
            var pdfDict = source.GetAsDict(target);
            if ((dic = pdfDict) != null)
            {
                if ((res = (PdfDictionary)PdfReader.GetPdfObject(result.Get(target), result)) == null)
                {
                    res = new PdfDictionary();
                }

                res.MergeDifferent(dic);
                result.Put(target, res);
                if (writer != null)
                {
                    writer.MarkUsed(res);
                }
            }
        }
    }

    internal static void MergeResources(PdfDictionary result, PdfDictionary source)
    {
        MergeResources(result, source, null);
    }

    protected static PdfFormField CreateButton(PdfWriter writer, int flags)
    {
        var field = new PdfFormField(writer);
        field.Button = flags;
        return field;
    }

    protected static PdfFormField CreateChoice(PdfWriter writer, int flags, PdfArray options, int topIndex)
    {
        var field = new PdfFormField(writer);
        field.Put(PdfName.Ft, PdfName.Ch);
        field.Put(PdfName.Ff, new PdfNumber(flags));
        field.Put(PdfName.Opt, options);
        if (topIndex > 0)
        {
            field.Put(PdfName.Ti, new PdfNumber(topIndex));
        }

        return field;
    }

    protected static PdfArray ProcessOptions(string[] options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var array = new PdfArray();
        for (var k = 0; k < options.Length; ++k)
        {
            array.Add(new PdfString(options[k], TEXT_UNICODE));
        }

        return array;
    }

    protected static PdfArray ProcessOptions(string[,] options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var array = new PdfArray();
        for (var k = 0; k < options.GetLength(0); ++k)
        {
            var ar2 = new PdfArray(new PdfString(options[k, 0], TEXT_UNICODE));
            ar2.Add(new PdfString(options[k, 1], TEXT_UNICODE));
            array.Add(ar2);
        }

        return array;
    }
}