using System.util;
using iTextSharp.LGPLv2.Core.System.Encodings;

namespace iTextSharp.text.pdf;

/// <summary>
///     Reads an FDF form and makes the fields available
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class FdfReader : PdfReader
{
    internal PdfName Encoding;
    internal INullValueDictionary<string, PdfDictionary> fields;
    internal string fileSpec;

    /// <summary>
    ///     Reads an FDF form.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="filename">the file name of the form</param>
    public FdfReader(string filename) : base(filename)
    {
    }

    /// <summary>
    ///     Reads an FDF form.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="pdfIn">the byte array with the form</param>
    public FdfReader(byte[] pdfIn) : base(pdfIn)
    {
    }

    /// <summary>
    ///     Reads an FDF form.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="url">the URL of the document</param>
    public FdfReader(Uri url) : base(url)
    {
    }

    /// <summary>
    ///     Reads an FDF form.
    ///     end but is not closed
    ///     @throws IOException on error
    /// </summary>
    /// <param name="isp">the  InputStream  containing the document. The stream is read to the</param>
    public FdfReader(Stream isp) : base(isp)
    {
    }

    /// <summary>
    ///     Gets all the fields. The map is keyed by the fully qualified
    ///     field name and the value is a merged  PdfDictionary
    ///     with the field content.
    /// </summary>
    /// <returns>all the fields</returns>
    public INullValueDictionary<string, PdfDictionary> Fields => fields;

    /// <summary>
    ///     Gets the PDF file specification contained in the FDF.
    /// </summary>
    /// <returns>the PDF file specification contained in the FDF</returns>
    public string FileSpec => fileSpec;

    /// <summary>
    ///     Gets the field dictionary.
    /// </summary>
    /// <param name="name">the fully qualified field name</param>
    /// <returns>the field dictionary</returns>
    public PdfDictionary GetField(string name) => fields[name];

    /// <summary>
    ///     Gets the field value or  null  if the field does not
    ///     exist or has no value defined.
    /// </summary>
    /// <param name="name">the fully qualified field name</param>
    /// <returns>the field value or  null </returns>
    public string GetFieldValue(string name)
    {
        var field = fields[name];
        if (field == null)
        {
            return null;
        }

        var v = GetPdfObject(field.Get(PdfName.V));
        if (v == null)
        {
            return null;
        }

        if (v.IsName())
        {
            return PdfName.DecodeName(((PdfName)v).ToString());
        }

        if (v.IsString())
        {
            var vs = (PdfString)v;
            if (Encoding == null || vs.Encoding != null)
            {
                return vs.ToUnicodeString();
            }

            var b = vs.GetBytes();
            if (b.Length >= 2 && b[0] == 254 && b[1] == 255)
            {
                return vs.ToUnicodeString();
            }

            try
            {
                if (Encoding.Equals(PdfName.ShiftJis))
                {
                    return EncodingsRegistry.GetEncoding(932).GetString(b);
                }

                if (Encoding.Equals(PdfName.Uhc))
                {
                    return EncodingsRegistry.GetEncoding(949).GetString(b);
                }

                if (Encoding.Equals(PdfName.Gbk))
                {
                    return EncodingsRegistry.GetEncoding(936).GetString(b);
                }

                if (Encoding.Equals(PdfName.Bigfive))
                {
                    return EncodingsRegistry.GetEncoding(950).GetString(b);
                }
            }
            catch
            {
            }

            return vs.ToUnicodeString();
        }

        return null;
    }

    protected internal override void ReadPdf()
    {
        fields = new NullValueDictionary<string, PdfDictionary>();
        try
        {
            Tokens.CheckFdfHeader();
            RebuildXref();
            ReadDocObj();
        }
        finally
        {
            try
            {
                Tokens.Close();
            }
            catch
            {
                // empty on purpose
            }
        }

        ReadFields();
    }

    protected virtual void KidNode(PdfDictionary merged, string name)
    {
        if (merged == null)
        {
            throw new ArgumentNullException(nameof(merged));
        }

        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var kids = merged.GetAsArray(PdfName.Kids);
        if (kids == null || kids.Size == 0)
        {
            if (name.Length > 0)
            {
                name = name.Substring(1);
            }

            fields[name] = merged;
        }
        else
        {
            merged.Remove(PdfName.Kids);
            for (var k = 0; k < kids.Size; ++k)
            {
                var dic = new PdfDictionary();
                dic.Merge(merged);
                var newDic = kids.GetAsDict(k);
                var t = newDic.GetAsString(PdfName.T);
                var newName = name;
                if (t != null)
                {
                    newName += "." + t.ToUnicodeString();
                }

                dic.Merge(newDic);
                dic.Remove(PdfName.T);
                KidNode(dic, newName);
            }
        }
    }

    protected virtual void ReadFields()
    {
        catalog = trailer.GetAsDict(PdfName.Root);
        var fdf = catalog.GetAsDict(PdfName.Fdf);
        if (fdf == null)
        {
            return;
        }

        var fs = fdf.GetAsString(PdfName.F);
        if (fs != null)
        {
            fileSpec = fs.ToUnicodeString();
        }

        var fld = fdf.GetAsArray(PdfName.Fields);
        if (fld == null)
        {
            return;
        }

        Encoding = fdf.GetAsName(PdfName.Encoding);
        var merged = new PdfDictionary();
        merged.Put(PdfName.Kids, fld);
        KidNode(merged, "");
    }
}