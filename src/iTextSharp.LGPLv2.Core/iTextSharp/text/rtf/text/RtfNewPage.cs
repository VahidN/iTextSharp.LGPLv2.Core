using System.IO;
using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.text
{


    /// <summary>
    /// The RtfNewPage creates a new page. INTERNAL CLASS
    /// @version $Version:$
    /// @author Mark Hall (Mark.Hall@mail.room3b.eu)
    /// </summary>
    public class RtfNewPage : RtfElement
    {

        /// <summary>
        /// Constant for a new page
        /// </summary>
        public static readonly byte[] NewPage = DocWriter.GetIsoBytes("\\page");

        /// <summary>
        /// Constructs a RtfNewPage
        /// </summary>
        /// <param name="doc">The RtfDocument this RtfNewPage belongs to</param>
        public RtfNewPage(RtfDocument doc) : base(doc)
        {
        }

        /// <summary>
        /// Writes a new page
        /// </summary>
        public override void WriteContent(Stream result)
        {
            result.Write(NewPage, 0, NewPage.Length);
            result.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
        }
    }
}