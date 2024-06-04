using System.util.zlib;

namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public class PrStream : PdfStream
{
    protected int length;
    protected int objGen;

    /// <summary>
    ///     added by ujihara for decryption
    /// </summary>
    protected int objNum;

    protected int offset;
    protected PdfReader reader;

    public PrStream(PrStream stream, PdfDictionary newDic)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        reader = stream.reader;
        offset = stream.offset;
        length = stream.Length;
        Compressed = stream.Compressed;
        CompressionLevel = stream.CompressionLevel;
        StreamBytes = stream.StreamBytes;
        Bytes = stream.Bytes;
        objNum = stream.objNum;
        objGen = stream.objGen;
        Merge(newDic ?? stream);
    }

    public PrStream(PrStream stream, PdfDictionary newDic, PdfReader reader) : this(stream, newDic) =>
        this.reader = reader;

    public PrStream(PdfReader reader, int offset)
    {
        this.reader = reader;
        this.offset = offset;
    }

    public PrStream(PdfReader reader, byte[] conts) : this(reader, conts, DEFAULT_COMPRESSION)
    {
    }

    /// <summary>
    ///     Creates a new PDF stream object that will replace a stream
    ///     in a existing PDF file.
    ///     @since   2.1.3 (replacing the existing constructor without param compressionLevel)
    /// </summary>
    /// <param name="reader">the reader that holds the existing PDF</param>
    /// <param name="conts">the new content</param>
    /// <param name="compressionLevel">the compression level for the content</param>
    public PrStream(PdfReader reader, byte[] conts, int compressionLevel)
    {
        if (conts == null)
        {
            throw new ArgumentNullException(nameof(conts));
        }

        this.reader = reader;
        offset = -1;
        if (Document.Compress)
        {
            var stream = new MemoryStream();
            var zip = new ZDeflaterOutputStream(stream, compressionLevel);
            zip.Write(conts, 0, conts.Length);
            zip.Close();
            Bytes = stream.ToArray();
            Put(PdfName.Filter, PdfName.Flatedecode);
        }
        else
        {
            Bytes = conts;
        }

        Length = Bytes.Length;
    }

    public new int Length
    {
        set
        {
            length = value;
            Put(PdfName.LENGTH, new PdfNumber(length));
        }
        get => length;
    }

    public int ObjGen
    {
        get => objGen;
        set => objGen = value;
    }

    public int ObjNum
    {
        get => objNum;
        set => objNum = value;
    }

    public int Offset => offset;

    public PdfReader Reader => reader;

    public new byte[] GetBytes() => Bytes;

    /// <summary>
    ///     Sets the data associated with the stream, either compressed or
    ///     uncompressed. Note that the data will never be compressed if
    ///     Document.compress is set to false.
    ///     @since   iText 2.1.1
    /// </summary>
    /// <param name="data">raw data, decrypted and uncompressed.</param>
    /// <param name="compress">true if you want the stream to be compresssed.</param>
    public void SetData(byte[] data, bool compress)
    {
        SetData(data, compress, DEFAULT_COMPRESSION);
    }

    /// <summary>
    ///     Sets the data associated with the stream, either compressed or
    ///     uncompressed. Note that the data will never be compressed if
    ///     Document.compress is set to false.
    ///     @since   iText 2.1.3
    /// </summary>
    /// <param name="data">raw data, decrypted and uncompressed.</param>
    /// <param name="compress">true if you want the stream to be compresssed.</param>
    /// <param name="compressionLevel">a value between -1 and 9 (ignored if compress == false)</param>
    public void SetData(byte[] data, bool compress, int compressionLevel)
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        Remove(PdfName.Filter);
        offset = -1;
        if (Document.Compress && compress)
        {
            var stream = new MemoryStream();
            var zip = new ZDeflaterOutputStream(stream, compressionLevel);
            zip.Write(data, 0, data.Length);
            zip.Close();
            Bytes = stream.ToArray();
            CompressionLevel = compressionLevel;
            Put(PdfName.Filter, PdfName.Flatedecode);
        }
        else
        {
            Bytes = data;
        }

        Length = Bytes.Length;
    }

    /// <summary>
    ///     Sets the data associated with the stream
    /// </summary>
    /// <param name="data">raw data, decrypted and uncompressed.</param>
    public void SetData(byte[] data)
    {
        SetData(data, true);
    }

    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        var b = PdfReader.GetStreamBytesRaw(this);
        PdfEncryption crypto = null;
        if (writer != null)
        {
            crypto = writer.Encryption;
        }

        var objLen = Get(PdfName.LENGTH);
        var nn = b.Length;
        if (crypto != null)
        {
            nn = crypto.CalculateStreamSize(nn);
        }

        Put(PdfName.LENGTH, new PdfNumber(nn));
        SuperToPdf(writer, os);
        Put(PdfName.LENGTH, objLen);
        os.Write(Startstream, 0, Startstream.Length);
        if (length > 0)
        {
            if (crypto != null && !crypto.IsEmbeddedFilesOnly())
            {
                b = crypto.EncryptByteArray(b);
            }

            os.Write(b, 0, b.Length);
        }

        os.Write(Endstream, 0, Endstream.Length);
    }
}