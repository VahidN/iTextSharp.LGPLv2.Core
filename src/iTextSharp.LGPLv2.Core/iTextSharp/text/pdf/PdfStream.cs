using System.util.zlib;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfStream  is the Pdf stream object.
///     A stream, like a string, is a sequence of characters. However, an application can
///     read a small portion of a stream at a time, while a string must be read in its entirety.
///     For this reason, objects with potentially large amounts of data, such as images and
///     page descriptions, are represented as streams.
///     A stream consists of a dictionary that describes a sequence of characters, followed by
///     the keyword <B>stream</B>, followed by zero or more lines of characters, followed by
///     the keyword <B>endstream</B>.
///     All streams must be  PdfIndirectObject s. The stream dictionary must be a direct
///     object. The keyword <B>stream</B> that follows the stream dictionary should be followed by
///     a carriage return and linefeed or just a linefeed.
///     Remark: In this version only the FLATEDECODE-filter is supported.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.8 (page 41-53).
///     @see        PdfObject
///     @see        PdfDictionary
/// </summary>
public class PdfStream : PdfDictionary
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     A possible compression level.
    ///     @since   2.1.3
    /// </summary>
    public const int BEST_COMPRESSION = 9;

    /// <summary>
    ///     A possible compression level.
    ///     @since   2.1.3
    /// </summary>
    public const int BEST_SPEED = 1;

    /// <summary>
    ///     A possible compression level.
    ///     @since   2.1.3
    /// </summary>
    public const int DEFAULT_COMPRESSION = -1;

    /// <summary>
    ///     A possible compression level.
    ///     @since   2.1.3
    /// </summary>
    public const int NO_COMPRESSION = 0;

    internal static readonly byte[] Endstream = DocWriter.GetIsoBytes("\nendstream");

    internal static readonly int Sizestream;

    internal static readonly byte[] Startstream = DocWriter.GetIsoBytes("stream\n");

    /// <summary>
    ///     is the stream compressed?
    /// </summary>
    protected bool Compressed;

    /// <summary>
    ///     The level of compression.
    ///     @since   2.1.3
    /// </summary>
    protected int CompressionLevel = NO_COMPRESSION;

    protected Stream InputStream;
    protected int InputStreamLength = -1;
    protected PdfIndirectReference Iref;
    protected int rawLength;
    protected MemoryStream StreamBytes;
    protected PdfWriter Writer;

    /// <summary>
    ///     constructors
    /// </summary>
    static PdfStream()
        => Sizestream = Startstream.Length + Endstream.Length;

    /// <summary>
    ///     Constructs a  PdfStream -object.
    /// </summary>
    /// <param name="bytes">content of the new  PdfObject  as an array of  byte .</param>
    public PdfStream(byte[] bytes)
    {
        type = STREAM;
        Bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
        rawLength = bytes.Length;
        Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
    }

    /// <summary>
    ///     Creates an efficient stream. No temporary array is ever created. The  InputStream
    ///     is totally consumed but is not closed. The general usage is:
    ///     InputStream in = ...;
    ///     PdfStream stream = new PdfStream(in, writer);
    ///     stream.FlateCompress();
    ///     writer.AddToBody(stream);
    ///     stream.WriteLength();
    ///     in.Close();
    /// </summary>
    /// <param name="inputStream">the data to write to this stream</param>
    /// <param name="writer">the  PdfWriter  for this stream</param>
    public PdfStream(Stream inputStream, PdfWriter writer)
    {
        type = STREAM;
        InputStream = inputStream;
        Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        Iref = writer.PdfIndirectReference;
        Put(PdfName.LENGTH, Iref);
    }

    /// <summary>
    ///     Constructs a  PdfStream -object.
    /// </summary>
    protected PdfStream()
        => type = STREAM;

    /// <summary>
    ///     methods overriding some methods of PdfObject
    /// </summary>

    public int RawLength => rawLength;

    /// <summary>
    ///     Compresses the stream.
    /// </summary>
    public void FlateCompress()
        => FlateCompress(DEFAULT_COMPRESSION);

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Compresses the stream.
    ///     @since   2.1.3
    /// </summary>
    /// <param name="compressionLevel">the compression level (0 = best speed, 9 = best compression, -1 is default)</param>
    public void FlateCompress(int compressionLevel)
    {
        if (!Document.Compress)
        {
            return;
        }

        // check if the flateCompress-method has allready been
        if (Compressed)
        {
            return;
        }

        CompressionLevel = compressionLevel;

        if (InputStream != null)
        {
            Compressed = true;

            return;
        }

        // check if a filter allready exists
        var filter = PdfReader.GetPdfObject(Get(PdfName.Filter));

        if (filter != null)
        {
            if (filter.IsName())
            {
                if (PdfName.Flatedecode.Equals(filter))
                {
                    return;
                }
            }
            else if (filter.IsArray())
            {
                if (((PdfArray)filter).Contains(PdfName.Flatedecode))
                {
                    return;
                }
            }
            else
            {
                throw new PdfException("Stream could not be compressed: filter is not a name or array.");
            }
        }

        // compress
        var stream = new MemoryStream();
        var zip = new ZDeflaterOutputStream(stream, compressionLevel);

        if (StreamBytes != null)
        {
            StreamBytes.WriteTo(zip);
        }
        else
        {
            zip.Write(Bytes, 0, Bytes.Length);
        }

        //zip.Close();
        zip.Finish();

        // update the object
        StreamBytes = stream;
        Bytes = null;
        Put(PdfName.LENGTH, new PdfNumber(StreamBytes.Length));

        if (filter == null)
        {
            Put(PdfName.Filter, PdfName.Flatedecode);
        }
        else
        {
            var filters = new PdfArray(filter);
            filters.Add(PdfName.Flatedecode);
            Put(PdfName.Filter, filters);
        }

        Compressed = true;
    }

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

                    if (a.Size > 0 && PdfName.Crypt.Equals(a[0]))
                    {
                        crypto = null;
                    }
                }
            }
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

            if (crypto != null && !crypto.IsEmbeddedFilesOnly())
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
            if (crypto != null && !crypto.IsEmbeddedFilesOnly())
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
            else
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
        }

        os.Write(Endstream, 0, Endstream.Length);
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.PdfObject#toString()
    /// </summary>
    public override string ToString()
    {
        if (Get(PdfName.TYPE) == null)
        {
            return "Stream";
        }

        return "Stream of type: " + Get(PdfName.TYPE);
    }

    /// <summary>
    ///     Writes the data content to an  Stream .
    ///     @throws IOException on error
    /// </summary>
    /// <param name="os">the destination to write to</param>
    public void WriteContent(Stream os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        if (StreamBytes != null)
        {
            StreamBytes.WriteTo(os);
        }
        else if (Bytes != null)
        {
            os.Write(Bytes, 0, Bytes.Length);
        }
    }

    /// <summary>
    ///     Writes the stream length to the  PdfWriter .
    ///     This method must be called and can only be called if the contructor {@link #PdfStream(InputStream,PdfWriter)}
    ///     is used to create the stream.
    ///     @throws IOException on error
    ///     @see #PdfStream(InputStream,PdfWriter)
    /// </summary>
    public void WriteLength()
    {
        if (InputStream == null)
        {
            throw new PdfException(
                "WriteLength() can only be called in a contructed PdfStream(InputStream,PdfWriter).");
        }

        if (InputStreamLength == -1)
        {
            throw new PdfException("WriteLength() can only be called after output of the stream body.");
        }

        Writer.AddToBody(new PdfNumber(InputStreamLength), Iref, false);
    }

    protected virtual void SuperToPdf(PdfWriter writer, Stream os)
        => base.ToPdf(writer, os);
}