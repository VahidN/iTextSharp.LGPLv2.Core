using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Writes an FDF form.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class FdfWriter
{
    private static readonly byte[] _headerFdf = DocWriter.GetIsoBytes("%FDF-1.2\n%\u00e2\u00e3\u00cf\u00d3\n");
    private readonly INullValueDictionary<string, object> _fields = new NullValueDictionary<string, object>();

    /// <summary>
    ///     The PDF file associated with the FDF.
    /// </summary>
    private string _file;

    /// <summary>
    ///     Gets the PDF file name associated with the FDF.
    /// </summary>
    /// <returns>the PDF file name associated with the FDF</returns>
    public string File
    {
        get => _file;
        set => _file = value;
    }

    /// <summary>
    ///     Gets the field value.
    /// </summary>
    /// <param name="field">the field name</param>
    /// <returns>the field value or  null  if not found</returns>
    public string GetField(string field)
    {
        var map = _fields;
        var tk = new StringTokenizer(field, ".");

        if (!tk.HasMoreTokens())
        {
            return null;
        }

        while (true)
        {
            var s = tk.NextToken();
            var obj = map[s];

            if (obj == null)
            {
                return null;
            }

            if (tk.HasMoreTokens())
            {
                if (obj is INullValueDictionary<string, object>)
                {
                    map = (INullValueDictionary<string, object>)obj;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (obj is INullValueDictionary<string, object>)
                {
                    return null;
                }

                if (((PdfObject)obj).IsString())
                {
                    return ((PdfString)obj).ToUnicodeString();
                }

                return PdfName.DecodeName(obj.ToString());
            }
        }
    }

    /// <summary>
    ///     Gets all the fields. The map is keyed by the fully qualified
    ///     field name and the values are  PdfObject .
    /// </summary>
    /// <returns>a map with all the fields</returns>
    public INullValueDictionary<string, object> GetFields()
    {
        var values = new NullValueDictionary<string, object>();
        IterateFields(values, _fields, "");

        return values;
    }

    /// <summary>
    ///     Removes the field value.
    ///     false  otherwise
    /// </summary>
    /// <param name="field">the field name</param>
    /// <returns> true  if the field was found and removed,</returns>
    public bool RemoveField(string field)
    {
        var map = _fields;
        var tk = new StringTokenizer(field, ".");

        if (!tk.HasMoreTokens())
        {
            return false;
        }

        List<object> hist = new();

        while (true)
        {
            var s = tk.NextToken();
            var obj = map[s];

            if (obj == null)
            {
                return false;
            }

            hist.Add(map);
            hist.Add(s);

            if (tk.HasMoreTokens())
            {
                if (obj is INullValueDictionary<string, object>)
                {
                    map = (INullValueDictionary<string, object>)obj;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (obj is INullValueDictionary<string, object>)
                {
                    return false;
                }

                break;
            }
        }

        for (var k = hist.Count - 2; k >= 0; k -= 2)
        {
            map = (INullValueDictionary<string, object>)hist[k];
            var s = (string)hist[k + 1];
            map.Remove(s);

            if (map.Count > 0)
            {
                break;
            }
        }

        return true;
    }

    /// <summary>
    ///     Sets the field value as a  PDFAction .
    ///     For example, this method allows setting a form submit button action using {@link PdfAction#createSubmitForm(String,
    ///     Object[], int)}.
    ///     This method creates an  A  entry for the specified field in the underlying FDF file.
    ///     Method contributed by Philippe Laflamme (plaflamme)
    ///     false  if the name is incompatible with
    ///     an existing field
    ///     @since	2.1.5
    /// </summary>
    /// <param name="field">the fully qualified field name</param>
    /// <param name="action">the field's action</param>
    /// <returns> true  if the value was inserted,</returns>
    public bool SetFieldAsAction(string field, PdfAction action)
        => SetField(field, action);

    /// <summary>
    ///     Sets the field value as a name.
    ///     false  if the name is incompatible with
    ///     an existing field
    /// </summary>
    /// <param name="field">the fully qualified field name</param>
    /// <param name="value">the value</param>
    /// <returns> true  if the value was inserted,</returns>
    public bool SetFieldAsName(string field, string value)
        => SetField(field, new PdfName(value));

    /// <summary>
    ///     Sets the field value as a string.
    ///     false  if the name is incompatible with
    ///     an existing field
    /// </summary>
    /// <param name="field">the fully qualified field name</param>
    /// <param name="value">the value</param>
    /// <returns> true  if the value was inserted,</returns>
    public bool SetFieldAsString(string field, string value)
        => SetField(field, new PdfString(value, PdfObject.TEXT_UNICODE));

    /// <summary>
    ///     Sets all the fields from this  FdfReader
    /// </summary>
    /// <param name="fdf">the  FdfReader </param>
    public void SetFields(FdfReader fdf)
    {
        if (fdf == null)
        {
            throw new ArgumentNullException(nameof(fdf));
        }

        var map = fdf.Fields;

        foreach (var entry in map)
        {
            var key = entry.Key;
            var dic = entry.Value;
            var v = dic.Get(PdfName.V);

            if (v != null)
            {
                SetField(key, v);
            }

            v = dic.Get(PdfName.A); // (plaflamme)

            if (v != null)
            {
                SetField(key, v);
            }
        }
    }

    /// <summary>
    ///     Sets all the fields from this  PdfReader
    /// </summary>
    /// <param name="pdf">the  PdfReader </param>
    public void SetFields(PdfReader pdf)
    {
        if (pdf == null)
        {
            throw new ArgumentNullException(nameof(pdf));
        }

        SetFields(pdf.AcroFields);
    }

    /// <summary>
    ///     Sets all the fields from this  AcroFields
    /// </summary>
    /// <param name="af">the  AcroFields </param>
    public void SetFields(AcroFields af)
    {
        if (af == null)
        {
            throw new ArgumentNullException(nameof(af));
        }

        foreach (var entry in af.Fields)
        {
            var fn = entry.Key;
            var item = entry.Value;
            var dic = item.GetMerged(0);
            var v = PdfReader.GetPdfObjectRelease(dic.Get(PdfName.V));

            if (v == null)
            {
                continue;
            }

            var ft = PdfReader.GetPdfObjectRelease(dic.Get(PdfName.Ft));

            if (ft == null || PdfName.Sig.Equals(ft))
            {
                continue;
            }

            SetField(fn, v);
        }
    }

    /// <summary>
    ///     Writes the content to a stream.
    ///     @throws DocumentException on error
    ///     @throws IOException on error
    /// </summary>
    /// <param name="os">the stream</param>
    public void WriteTo(Stream os)
    {
        var wrt = new Wrt(os, this);
        wrt.WriteTo();
    }

    internal static void IterateFields(INullValueDictionary<string, object> values,
        INullValueDictionary<string, object> map,
        string name)
    {
        foreach (var entry in map)
        {
            var s = entry.Key;
            var obj = entry.Value;

            if (obj is INullValueDictionary<string, object> objects)
            {
                IterateFields(values, objects, name + "." + s);
            }
            else
            {
                values[(name + "." + s).Substring(1)] = obj;
            }
        }
    }

    internal bool SetField(string field, PdfObject value)
    {
        var map = _fields;
        var tk = new StringTokenizer(field, ".");

        if (!tk.HasMoreTokens())
        {
            return false;
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
                    return false;
                }
            }
            else
            {
                if (!(obj is NullValueDictionary<string, object>))
                {
                    map[s] = value;

                    return true;
                }

                return false;
            }
        }
    }

    internal class Wrt : PdfWriter
    {
        private readonly FdfWriter _fdf;

        internal Wrt(Stream os, FdfWriter fdf) : base(new PdfDocument(), os)
        {
            _fdf = fdf;
            ((DocWriter)this).Os.Write(_headerFdf, 0, _headerFdf.Length);
            Body = new PdfBody(this);
        }

        internal static PdfArray Calculate(INullValueDictionary<string, object> map)
        {
            var ar = new PdfArray();

            foreach (var entry in map)
            {
                var key = entry.Key;
                var v = entry.Value;
                var dic = new PdfDictionary();
                dic.Put(PdfName.T, new PdfString(key, PdfObject.TEXT_UNICODE));

                if (v is NullValueDictionary<string, object>)
                {
                    dic.Put(PdfName.Kids, Calculate((NullValueDictionary<string, object>)v));
                }
                else if (v is PdfAction)
                {
                    // (plaflamme)
                    dic.Put(PdfName.A, (PdfAction)v);
                }
                else
                {
                    dic.Put(PdfName.V, (PdfObject)v);
                }

                ar.Add(dic);
            }

            return ar;
        }

        internal void WriteTo()
        {
            var dic = new PdfDictionary();
            dic.Put(PdfName.Fields, Calculate(_fdf._fields));

            if (_fdf._file != null)
            {
                dic.Put(PdfName.F, new PdfString(_fdf._file, PdfObject.TEXT_UNICODE));
            }

            var fd = new PdfDictionary();
            fd.Put(PdfName.Fdf, dic);
            var refi = AddToBody(fd).IndirectReference;
            var b = GetIsoBytes("trailer\n");
            ((DocWriter)this).Os.Write(b, 0, b.Length);
            var trailer = new PdfDictionary();
            trailer.Put(PdfName.Root, refi);
            trailer.ToPdf(null, ((DocWriter)this).Os);
            b = GetIsoBytes("\n%%EOF\n");
            ((DocWriter)this).Os.Write(b, 0, b.Length);
            ((DocWriter)this).Os.Dispose();
        }
    }
}