using System.util;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.xml.xmp;

/// <summary>
///     With this class you can create an Xmp Stream that can be used for adding
///     Metadata to a PDF Dictionary. Remark that this class doesn't cover the
///     complete XMP specification.
/// </summary>
public class XmpWriter
{
    /// <summary>
    ///     String used to fill the extra space.
    /// </summary>
    public const string EXTRASPACE =
        "                                                                                                   \n";

    /// <summary>
    ///     A possible charset for the XMP.
    /// </summary>
    public const string UTF16 = "UTF-16";

    /// <summary>
    ///     A possible charset for the XMP.
    /// </summary>
    public const string UTF16BE = "UTF-16BE";

    /// <summary>
    ///     A possible charset for the XMP.
    /// </summary>
    public const string UTF16LE = "UTF-16LE";

    /// <summary>
    ///     A possible charset for the XMP.
    /// </summary>
    public const string UTF8 = "UTF-8";

    /// <summary>
    ///     Processing Instruction required at the start of an XMP stream
    ///     @since iText 2.1.6
    /// </summary>
    public const string XPACKET_PI_BEGIN = "<?xpacket begin=\"\uFEFF\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>\n";

    /// <summary>
    ///     Processing Instruction required at the end of an XMP stream for XMP streams that are read only
    ///     @since iText 2.1.6
    /// </summary>
    public const string XPACKET_PI_END_R = "<?xpacket end=\"r\"?>";

    /// <summary>
    ///     Processing Instruction required at the end of an XMP stream for XMP streams that can be updated
    ///     @since iText 2.1.6
    /// </summary>
    public const string XPACKET_PI_END_W = "<?xpacket end=\"w\"?>";

    /// <summary>
    ///     The about string that goes into the rdf:Description tags.
    /// </summary>
    protected string about;

    /// <summary>
    ///     The end attribute.
    /// </summary>
    protected char End = 'w';

    /// <summary>
    ///     You can add some extra space in the XMP packet; 1 unit in this variable represents 100 spaces and a newline.
    /// </summary>
    protected int ExtraSpace;

    /// <summary>
    ///     The writer to which you can write bytes for the XMP stream.
    /// </summary>
    protected StreamWriter Writer;

    /// <summary>
    ///     Creates an XmpWriter.
    ///     @throws IOException
    /// </summary>
    /// <param name="os"></param>
    /// <param name="utfEncoding"></param>
    /// <param name="extraSpace"></param>
    public XmpWriter(Stream os, string utfEncoding, int extraSpace)
    {
        ExtraSpace = extraSpace;
        Writer = new StreamWriter(os, new EncodingNoPreamble(IanaEncodings.GetEncodingEncoding(utfEncoding)));
        Writer.Write(XPACKET_PI_BEGIN);
        Writer.Write("<x:xmpmeta xmlns:x=\"adobe:ns:meta/\">\n");
        Writer.Write("<rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\">\n");
        about = "";
    }

    /// <summary>
    ///     Creates an XmpWriter.
    ///     @throws IOException
    /// </summary>
    /// <param name="os"></param>
    public XmpWriter(Stream os) : this(os, UTF8, 20)
    {
    }

    /// <summary>
    ///     @throws IOException
    /// </summary>
    public XmpWriter(Stream os, PdfDictionary info, int pdfXConformance) : this(os)
    {
        if (info != null)
        {
            var dc = new DublinCoreSchema();
            var p = new PdfSchema();
            var basic = new XmpBasicSchema();
            PdfObject obj;
            foreach (var key in info.Keys)
            {
                obj = info.Get(key);
                if (obj == null)
                {
                    continue;
                }

                if (PdfName.Title.Equals(key))
                {
                    dc.AddTitle(((PdfString)obj).ToUnicodeString());
                }

                if (PdfName.Author.Equals(key))
                {
                    dc.AddAuthor(((PdfString)obj).ToUnicodeString());
                }

                if (PdfName.Subject.Equals(key))
                {
                    dc.AddSubject(((PdfString)obj).ToUnicodeString());
                    dc.AddDescription(((PdfString)obj).ToUnicodeString());
                }

                if (PdfName.Keywords.Equals(key))
                {
                    p.AddKeywords(((PdfString)obj).ToUnicodeString());
                }

                if (PdfName.Creator.Equals(key))
                {
                    basic.AddCreatorTool(((PdfString)obj).ToUnicodeString());
                }

                if (PdfName.Producer.Equals(key))
                {
                    p.AddProducer(((PdfString)obj).ToUnicodeString());
                }

                if (PdfName.Creationdate.Equals(key))
                {
                    basic.AddCreateDate(((PdfDate)obj).GetW3CDate());
                }

                if (PdfName.Moddate.Equals(key))
                {
                    basic.AddModDate(((PdfDate)obj).GetW3CDate());
                }
            }

            if (dc.Count > 0)
            {
                AddRdfDescription(dc);
            }

            if (p.Count > 0)
            {
                AddRdfDescription(p);
            }

            if (basic.Count > 0)
            {
                AddRdfDescription(basic);
            }

            if (pdfXConformance == PdfWriter.PDFA1A || pdfXConformance == PdfWriter.PDFA1B)
            {
                var a1 = new PdfA1Schema();
                if (pdfXConformance == PdfWriter.PDFA1A)
                {
                    a1.AddConformance("A");
                }
                else
                {
                    a1.AddConformance("B");
                }

                AddRdfDescription(a1);
            }
        }
    }

    /// <summary>
    ///     @throws IOException
    /// </summary>
    /// <param name="os"></param>
    /// <param name="info"></param>
    public XmpWriter(Stream os, INullValueDictionary<string, string> info) : this(os)
    {
        if (info != null)
        {
            var dc = new DublinCoreSchema();
            var p = new PdfSchema();
            var basic = new XmpBasicSchema();
            string value;
            foreach (var entry in info)
            {
                var key = entry.Key;
                value = entry.Value;
                if (value == null)
                {
                    continue;
                }

                if ("Title".Equals(key, StringComparison.Ordinal))
                {
                    dc.AddTitle(value);
                }

                if ("Author".Equals(key, StringComparison.Ordinal))
                {
                    dc.AddAuthor(value);
                }

                if ("Subject".Equals(key, StringComparison.Ordinal))
                {
                    dc.AddSubject(value);
                    dc.AddDescription(value);
                }

                if ("Keywords".Equals(key, StringComparison.Ordinal))
                {
                    p.AddKeywords(value);
                }

                if ("Creator".Equals(key, StringComparison.Ordinal))
                {
                    basic.AddCreatorTool(value);
                }

                if ("Producer".Equals(key, StringComparison.Ordinal))
                {
                    p.AddProducer(value);
                }

                if ("CreationDate".Equals(key, StringComparison.Ordinal))
                {
                    basic.AddCreateDate(PdfDate.GetW3CDate(value));
                }

                if ("ModDate".Equals(key, StringComparison.Ordinal))
                {
                    basic.AddModDate(PdfDate.GetW3CDate(value));
                }
            }

            if (dc.Count > 0)
            {
                AddRdfDescription(dc);
            }

            if (p.Count > 0)
            {
                AddRdfDescription(p);
            }

            if (basic.Count > 0)
            {
                AddRdfDescription(basic);
            }
        }
    }

    /// <summary>
    /// </summary>
    public string About
    {
        set => about = value;
    }

    /// <summary>
    ///     Adds an rdf:Description.
    ///     @throws IOException
    /// </summary>
    /// <param name="xmlns"></param>
    /// <param name="content"></param>
    public void AddRdfDescription(string xmlns, string content)
    {
        Writer.Write("<rdf:Description rdf:about=\"");
        Writer.Write(about);
        Writer.Write("\" ");
        Writer.Write(xmlns);
        Writer.Write(">");
        Writer.Write(content);
        Writer.Write("</rdf:Description>\n");
    }

    /// <summary>
    ///     Adds an rdf:Description.
    ///     @throws IOException
    /// </summary>
    /// <param name="s"></param>
    public void AddRdfDescription(XmpSchema s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        Writer.Write("<rdf:Description rdf:about=\"");
        Writer.Write(about);
        Writer.Write("\" ");
        Writer.Write(s.Xmlns);
        Writer.Write(">");
        Writer.Write(s.ToString());
        Writer.Write("</rdf:Description>\n");
    }

    /// <summary>
    ///     Flushes and closes the XmpWriter.
    ///     @throws IOException
    /// </summary>
    public void Close()
    {
        Writer.Write("</rdf:RDF>");
        Writer.Write("</x:xmpmeta>\n");
        for (var i = 0; i < ExtraSpace; i++)
        {
            Writer.Write(EXTRASPACE);
        }

        Writer.Write(End == 'r' ? XPACKET_PI_END_R : XPACKET_PI_END_W);
        Writer.Flush();
        Writer.Dispose();
    }

    /// <summary>
    ///     Sets the XMP to read-only
    /// </summary>
    public void SetReadOnly()
    {
        End = 'r';
    }
}