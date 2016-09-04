using System.IO;


namespace iTextSharp.text.rtf.document
{
    /// <summary>
    /// The RtfGenerator creates the (\*\generator ...} element.
    /// @author Howard Shank (hgshank@yahoo.com)
    /// @since 2.0.8
    /// </summary>
    public class RtfGenerator : RtfElement
    {
        /// <summary>
        /// Generator group starting tag
        /// </summary>
        private static readonly byte[] _generator = DocWriter.GetIsoBytes("\\*\\generator");

        /// <summary>
        /// Constructs a  RtfGenerator  belonging to a RtfDocument
        /// </summary>
        /// <param name="doc">The  RtfDocument  this  RtfGenerator  belongs to</param>
        public RtfGenerator(RtfDocument doc) : base(doc)
        {
        }


        /// <summary>
        /// Writes the RTF generator group.
        /// </summary>
        public override void WriteContent(Stream result)
        {
            result.Write(OpenGroup, 0, OpenGroup.Length);
            result.Write(_generator, 0, _generator.Length);
            result.Write(Delimiter, 0, Delimiter.Length);
            byte[] t;
            result.Write(t = DocWriter.GetIsoBytes(iTextSharp.text.Document.Version), 0, t.Length);
            result.Write(CloseGroup, 0, CloseGroup.Length);
            Document.OutputDebugLinebreak(result);
        }
    }
}
