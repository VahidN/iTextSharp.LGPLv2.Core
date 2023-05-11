namespace iTextSharp.text.rtf.document;

/// <summary>
///     The RtfGenerator creates the (\*\generator ...} element.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public class RtfGenerator : RtfElement
{
    /// <summary>
    ///     Generator group starting tag
    /// </summary>
    private static readonly byte[] _generator = DocWriter.GetIsoBytes("\\*\\generator");

    /// <summary>
    ///     Constructs a  RtfGenerator  belonging to a RtfDocument
    /// </summary>
    /// <param name="doc">The  RtfDocument  this  RtfGenerator  belongs to</param>
    public RtfGenerator(RtfDocument doc) : base(doc)
    {
    }


    /// <summary>
    ///     Writes the RTF generator group.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_generator, 0, _generator.Length);
        outp.Write(Delimiter, 0, Delimiter.Length);
        byte[] t;
        outp.Write(t = DocWriter.GetIsoBytes(iTextSharp.text.Document.Version), 0, t.Length);
        outp.Write(CloseGroup, 0, CloseGroup.Length);
        Document.OutputDebugLinebreak(outp);
    }
}