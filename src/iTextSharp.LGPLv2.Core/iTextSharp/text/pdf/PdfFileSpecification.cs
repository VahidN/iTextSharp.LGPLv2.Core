using iTextSharp.LGPLv2.Core.System.NetUtils;
using iTextSharp.text.pdf.collection;

namespace iTextSharp.text.pdf;

/// <summary>
///     Specifies a file or an URL. The file can be extern or embedded.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfFileSpecification : PdfDictionary
{
    protected PdfIndirectReference Refi;
    protected PdfWriter Writer;

    /// <summary>
    ///     Creates a new instance of PdfFileSpecification. The static methods are preferred.
    /// </summary>
    public PdfFileSpecification() : base(PdfName.Filespec)
    {
    }

    /// <summary>
    ///     Sets the file name (the key /F) string as an hex representation
    ///     to support multi byte file names. The name must have the slash and
    ///     backslash escaped according to the file specification rules
    /// </summary>
    public byte[] MultiByteFileName
    {
        set => Put(PdfName.F, new PdfString(value).SetHexWriting(true));
    }

    /// <summary>
    ///     Gets the indirect reference to this file specification.
    ///     Multiple invocations will retrieve the same value.
    ///     @throws IOException on error
    /// </summary>
    /// <returns>the indirect reference</returns>
    public PdfIndirectReference Reference
    {
        get
        {
            if (Refi != null)
            {
                return Refi;
            }

            Refi = Writer.AddToBody(this).IndirectReference;
            return Refi;
        }
    }

    /// <summary>
    ///     Sets a flag that indicates whether an external file referenced by the file
    ///     specification is volatile. If the value is true, applications should never
    ///     cache a copy of the file.
    /// </summary>
    public bool Volatile
    {
        set => Put(PdfName.V, new PdfBoolean(value));
    }

    /// <summary>
    ///     Creates a file specification with the file embedded. The file may
    ///     come from the file system or from a byte array. The data is flate compressed.
    ///     it takes precedence over  filePath
    ///     @throws IOException on error
    /// </summary>
    /// <param name="writer">the  PdfWriter </param>
    /// <param name="filePath">the file path</param>
    /// <param name="fileDisplay">the file information that is presented to the user</param>
    /// <param name="fileStore">the byte array with the file. If it is not  null </param>
    /// <returns>the file specification</returns>
    public static PdfFileSpecification FileEmbedded(PdfWriter writer, string filePath, string fileDisplay,
                                                    byte[] fileStore) =>
        FileEmbedded(writer, filePath, fileDisplay, fileStore, PdfStream.BEST_COMPRESSION);

    /// <summary>
    ///     Creates a file specification with the file embedded. The file may
    ///     come from the file system or from a byte array. The data is flate compressed.
    ///     it takes precedence over  filePath
    ///     it takes precedence over  filePath
    ///     @throws IOException on error
    ///     @since    2.1.3
    /// </summary>
    /// <param name="writer">the  PdfWriter </param>
    /// <param name="filePath">the file path</param>
    /// <param name="fileDisplay">the file information that is presented to the user</param>
    /// <param name="fileStore">the byte array with the file. If it is not  null </param>
    /// <param name="compressionLevel">the compression level to be used for compressing the file</param>
    /// <returns>the file specification</returns>
    public static PdfFileSpecification FileEmbedded(PdfWriter writer, string filePath, string fileDisplay,
                                                    byte[] fileStore, int compressionLevel) =>
        FileEmbedded(writer, filePath, fileDisplay, fileStore, null, null, compressionLevel);

    /// <summary>
    ///     Creates a file specification with the file embedded. The file may
    ///     come from the file system or from a byte array.
    ///     it takes precedence over  filePath
    ///     from compression
    ///     @throws IOException on error
    /// </summary>
    /// <param name="writer">the  PdfWriter </param>
    /// <param name="filePath">the file path</param>
    /// <param name="fileDisplay">the file information that is presented to the user</param>
    /// <param name="fileStore">the byte array with the file. If it is not  null </param>
    /// <param name="compress">sets the compression on the data. Multimedia content will benefit little</param>
    /// <returns>the file specification</returns>
    public static PdfFileSpecification FileEmbedded(PdfWriter writer, string filePath, string fileDisplay,
                                                    byte[] fileStore, bool compress) =>
        FileEmbedded(writer, filePath, fileDisplay, fileStore, null, null,
                     compress ? PdfStream.BEST_COMPRESSION : PdfStream.NO_COMPRESSION);

    /// <summary>
    ///     Creates a file specification with the file embedded. The file may
    ///     come from the file system or from a byte array.
    ///     it takes precedence over  filePath
    ///     from compression
    ///     @throws IOException on error
    /// </summary>
    /// <param name="writer">the  PdfWriter </param>
    /// <param name="filePath">the file path</param>
    /// <param name="fileDisplay">the file information that is presented to the user</param>
    /// <param name="fileStore">the byte array with the file. If it is not  null </param>
    /// <param name="compress">sets the compression on the data. Multimedia content will benefit little</param>
    /// <param name="mimeType">the optional mimeType</param>
    /// <param name="fileParameter">the optional extra file parameters such as the creation or modification date</param>
    /// <returns>the file specification</returns>
    public static PdfFileSpecification FileEmbedded(PdfWriter writer, string filePath, string fileDisplay,
                                                    byte[] fileStore, bool compress, string mimeType,
                                                    PdfDictionary fileParameter) =>
        FileEmbedded(writer, filePath, fileDisplay, fileStore, null, null,
                     compress ? PdfStream.BEST_COMPRESSION : PdfStream.NO_COMPRESSION);

    /// <summary>
    ///     Creates a file specification with the file embedded. The file may
    ///     come from the file system or from a byte array.
    ///     it takes precedence over  filePath
    ///     @throws IOException on error
    ///     @since   2.1.3
    /// </summary>
    /// <param name="writer">the  PdfWriter </param>
    /// <param name="filePath">the file path</param>
    /// <param name="fileDisplay">the file information that is presented to the user</param>
    /// <param name="fileStore">the byte array with the file. If it is not  null </param>
    /// <param name="mimeType">the optional mimeType</param>
    /// <param name="fileParameter">the optional extra file parameters such as the creation or modification date</param>
    /// <param name="compressionLevel">the level of compression</param>
    /// <returns>the file specification</returns>
    public static PdfFileSpecification FileEmbedded(PdfWriter writer, string filePath, string fileDisplay,
                                                    byte[] fileStore, string mimeType, PdfDictionary fileParameter,
                                                    int compressionLevel)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (filePath == null)
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        var fs = new PdfFileSpecification();
        fs.Writer = writer;
        fs.Put(PdfName.F, new PdfString(fileDisplay));
        PdfEFStream stream;
        Stream inp = null;
        PdfIndirectReference refi;
        PdfIndirectReference refFileLength;
        try
        {
            refFileLength = writer.PdfIndirectReference;
            if (fileStore == null)
            {
                if (File.Exists(filePath))
                {
                    inp = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                }
                else
                {
                    if (filePath.StartsWith("file:/", StringComparison.OrdinalIgnoreCase) ||
                        filePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                        filePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        inp = filePath.GetResponseStream();
                    }
                    else
                    {
                        inp = BaseFont.GetResourceStream(filePath);
                        if (inp == null)
                        {
                            throw new IOException(filePath + " not found as file or resource.");
                        }
                    }
                }

                stream = new PdfEFStream(inp, writer);
            }
            else
            {
                stream = new PdfEFStream(fileStore);
            }

            stream.Put(PdfName.TYPE, PdfName.Embeddedfile);
            stream.FlateCompress(compressionLevel);
            stream.Put(PdfName.Params, refFileLength);
            if (mimeType != null)
            {
                stream.Put(PdfName.Subtype, new PdfName(mimeType));
            }

            refi = writer.AddToBody(stream).IndirectReference;
            if (fileStore == null)
            {
                stream.WriteLength();
            }

            var param = new PdfDictionary();
            if (fileParameter != null)
            {
                param.Merge(fileParameter);
            }

            param.Put(PdfName.Size, new PdfNumber(stream.RawLength));
            writer.AddToBody(param, refFileLength);
        }
        finally
        {
            if (inp != null)
            {
                try
                {
                    inp.Dispose();
                }
                catch
                {
                }
            }
        }

        var f = new PdfDictionary();
        f.Put(PdfName.F, refi);
        fs.Put(PdfName.EF, f);
        return fs;
    }

    /// <summary>
    ///     Creates a file specification for an external file.
    /// </summary>
    /// <param name="writer">the  PdfWriter </param>
    /// <param name="filePath">the file path</param>
    /// <returns>the file specification</returns>
    public static PdfFileSpecification FileExtern(PdfWriter writer, string filePath)
    {
        var fs = new PdfFileSpecification();
        fs.Writer = writer;
        fs.Put(PdfName.F, new PdfString(filePath));
        return fs;
    }

    /// <summary>
    ///     Creates a file specification of type URL.
    /// </summary>
    /// <param name="writer">the  PdfWriter </param>
    /// <param name="url">the URL</param>
    /// <returns>the file specification</returns>
    public static PdfFileSpecification Url(PdfWriter writer, string url)
    {
        var fs = new PdfFileSpecification();
        fs.Writer = writer;
        fs.Put(PdfName.Fs, PdfName.Url);
        fs.Put(PdfName.F, new PdfString(url));
        return fs;
    }

    /// <summary>
    ///     Adds the Collection item dictionary.
    /// </summary>
    public void AddCollectionItem(PdfCollectionItem ci)
    {
        Put(PdfName.Ci, ci);
    }

    /// <summary>
    ///     Adds a description for the file that is specified here.
    /// </summary>
    /// <param name="description">some text</param>
    /// <param name="unicode">if true, the text is added as a unicode string</param>
    public void AddDescription(string description, bool unicode)
    {
        Put(PdfName.Desc, new PdfString(description, unicode ? TEXT_UNICODE : TEXT_PDFDOCENCODING));
    }

    /// <summary>
    ///     Adds the unicode file name (the key /UF). This entry was introduced
    ///     in PDF 1.7. The filename must have the slash and backslash escaped
    ///     according to the file specification rules.
    /// </summary>
    /// <param name="filename">the filename</param>
    /// <param name="unicode">if true, the filename is UTF-16BE encoded; otherwise PDFDocEncoding is used;</param>
    public void SetUnicodeFileName(string filename, bool unicode)
    {
        Put(PdfName.Uf, new PdfString(filename, unicode ? TEXT_UNICODE : TEXT_PDFDOCENCODING));
    }
}