using System.util.zlib;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfContents  is a  PdfStream  containing the contents (text + graphics) of a  PdfPage .
/// </summary>
public class PdfContents : PdfStream
{
    internal static readonly byte[] Savestate = DocWriter.GetIsoBytes("q\n");
    internal static readonly byte[] Restorestate = DocWriter.GetIsoBytes("Q\n");
    internal static readonly byte[] Rotate90 = DocWriter.GetIsoBytes("0 1 -1 0 ");
    internal static readonly byte[] Rotate180 = DocWriter.GetIsoBytes("-1 0 0 -1 ");
    internal static readonly byte[] Rotate270 = DocWriter.GetIsoBytes("0 -1 1 0 ");
    internal static readonly byte[] Rotatefinal = DocWriter.GetIsoBytes(" cm\n");

    /// <summary>
    ///     Constructs a  PdfContents -object, containing text and general graphics.
    ///     @throws BadPdfFormatException on error
    /// </summary>
    /// <param name="under">the direct content that is under all others</param>
    /// <param name="content">the graphics in a page</param>
    /// <param name="text">the text in a page</param>
    /// <param name="secondContent">the direct content that is over all others</param>
    /// <param name="page"></param>
    internal PdfContents(PdfContentByte under, PdfContentByte content, PdfContentByte text,
                         PdfContentByte secondContent, Rectangle page)
    {
        Stream ostr = null;
        StreamBytes = new MemoryStream();
        if (Document.Compress)
        {
            Compressed = true;
            ostr = new ZDeflaterOutputStream(StreamBytes, text.PdfWriter.CompressionLevel);
        }
        else
        {
            ostr = StreamBytes;
        }

        var rotation = page.Rotation;
        byte[] tmp;
        switch (rotation)
        {
            case 90:
                ostr.Write(Rotate90, 0, Rotate90.Length);
                tmp = DocWriter.GetIsoBytes(ByteBuffer.FormatDouble(page.Top));
                ostr.Write(tmp, 0, tmp.Length);
                ostr.WriteByte((byte)' ');
                ostr.WriteByte((byte)'0');
                ostr.Write(Rotatefinal, 0, Rotatefinal.Length);
                break;
            case 180:
                ostr.Write(Rotate180, 0, Rotate180.Length);
                tmp = DocWriter.GetIsoBytes(ByteBuffer.FormatDouble(page.Right));
                ostr.Write(tmp, 0, tmp.Length);
                ostr.WriteByte((byte)' ');
                tmp = DocWriter.GetIsoBytes(ByteBuffer.FormatDouble(page.Top));
                ostr.Write(tmp, 0, tmp.Length);
                ostr.Write(Rotatefinal, 0, Rotatefinal.Length);
                break;
            case 270:
                ostr.Write(Rotate270, 0, Rotate270.Length);
                ostr.WriteByte((byte)'0');
                ostr.WriteByte((byte)' ');
                tmp = DocWriter.GetIsoBytes(ByteBuffer.FormatDouble(page.Right));
                ostr.Write(tmp, 0, tmp.Length);
                ostr.Write(Rotatefinal, 0, Rotatefinal.Length);
                break;
        }

        if (under.Size > 0)
        {
            ostr.Write(Savestate, 0, Savestate.Length);
            under.InternalBuffer.WriteTo(ostr);
            ostr.Write(Restorestate, 0, Restorestate.Length);
        }

        if (content.Size > 0)
        {
            ostr.Write(Savestate, 0, Savestate.Length);
            content.InternalBuffer.WriteTo(ostr);
            ostr.Write(Restorestate, 0, Restorestate.Length);
        }

        if (text != null)
        {
            ostr.Write(Savestate, 0, Savestate.Length);
            text.InternalBuffer.WriteTo(ostr);
            ostr.Write(Restorestate, 0, Restorestate.Length);
        }

        if (secondContent.Size > 0)
        {
            secondContent.InternalBuffer.WriteTo(ostr);
        }

        if (ostr is ZDeflaterOutputStream)
        {
            ((ZDeflaterOutputStream)ostr).Finish();
        }

        Put(PdfName.LENGTH, new PdfNumber(StreamBytes.Length));
        if (Compressed)
        {
            Put(PdfName.Filter, PdfName.Flatedecode);
        }
    }
}