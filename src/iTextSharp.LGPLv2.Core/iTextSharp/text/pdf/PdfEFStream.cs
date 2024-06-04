using System.util.zlib;

namespace iTextSharp.text.pdf;

/// <summary>
///     Extends PdfStream and should be used to create Streams for Embedded Files
///     (file attachments).
///     @since	2.1.3
/// </summary>
public class PdfEFStream : PdfStream
{
    /// <summary>
    ///     Creates a Stream object using an InputStream and a PdfWriter object
    /// </summary>
    /// <param name="inp">that will be read to get the Stream object</param>
    /// <param name="writer">to which the stream will be added</param>
    public PdfEFStream(Stream inp, PdfWriter writer) : base(inp, writer)
    {
    }

    /// <summary>
    ///     Creates a Stream object using a byte array
    /// </summary>
    /// <param name="fileStore">for the stream</param>
    public PdfEFStream(byte[] fileStore) : base(fileStore)
    {
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfDictionary#toPdf(com.lowagie.text.pdf.PdfWriter, java.io.OutputStream)
    /// </summary>
    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        if (InputStream != null && Compressed)
        {
            Put(PdfName.Filter, PdfName.Flatedecode);
        }

        PdfEncryption crypto = null;

        if (writer != null)
        {
            crypto = writer.Encryption;
        }

        if (crypto != null)
        {
            var filter = Get(PdfName.Filter);

            if (filter != null)
            {
                if (PdfName.Crypt.Equals(filter))
                {
                    crypto = null;
                }
                else if (filter.IsArray())
                {
                    var a = (PdfArray)filter;

                    if (!a.IsEmpty() && PdfName.Crypt.Equals(a[0]))
                    {
                        crypto = null;
                    }
                }
            }
        }

        if (crypto != null && crypto.IsEmbeddedFilesOnly())
        {
            var filter = new PdfArray();
            var decodeparms = new PdfArray();
            var crypt = new PdfDictionary();
            crypt.Put(PdfName.Name, PdfName.Stdcf);
            filter.Add(PdfName.Crypt);
            decodeparms.Add(crypt);

            if (Compressed)
            {
                filter.Add(PdfName.Flatedecode);
                decodeparms.Add(new PdfNull());
            }

            Put(PdfName.Filter, filter);
            Put(PdfName.Decodeparms, decodeparms);
        }

        var nn = Get(PdfName.LENGTH);

        if (crypto != null && nn != null && nn.IsNumber())
        {
            var sz = ((PdfNumber)nn).IntValue;
            Put(PdfName.LENGTH, new PdfNumber(crypto.CalculateStreamSize(sz)));
            SuperToPdf(writer, os);
            Put(PdfName.LENGTH, nn);
        }
        else
        {
            SuperToPdf(writer, os);
        }

        os.Write(Startstream, 0, Startstream.Length);

        if (InputStream != null)
        {
            rawLength = 0;
            ZDeflaterOutputStream def = null;
            var osc = new OutputStreamCounter(os);
            OutputStreamEncryption ose = null;
            Stream fout = osc;

            if (crypto != null)
            {
                ose = crypto.GetEncryptionStream(fout);
                fout = ose;
            }

            if (Compressed)
            {
                def = new ZDeflaterOutputStream(fout, CompressionLevel);
                fout = def;
            }

            var buf = new byte[4192];

            while (true)
            {
                var n = InputStream.Read(buf, 0, buf.Length);

                if (n <= 0)
                {
                    break;
                }

                fout.Write(buf, 0, n);
                rawLength += n;
            }

            if (def != null)
            {
                def.Finish();
            }

            if (ose != null)
            {
                ose.Finish();
            }

            InputStreamLength = osc.Counter;
        }
        else
        {
            if (crypto == null)
            {
                if (StreamBytes != null)
                {
                    StreamBytes.WriteTo(os);
                }
                else
                {
                    os.Write(Bytes, 0, Bytes.Length);
                }
            }
            else
            {
                byte[] b;

                if (StreamBytes != null)
                {
                    b = crypto.EncryptByteArray(StreamBytes.ToArray());
                }
                else
                {
                    b = crypto.EncryptByteArray(Bytes);
                }

                os.Write(b, 0, b.Length);
            }
        }

        os.Write(Endstream, 0, Endstream.Length);
    }
}