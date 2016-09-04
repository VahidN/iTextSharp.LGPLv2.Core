using System.IO;
using System.Collections;
using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.style
{
    /// <summary>
    /// The RtfFontList stores the list of fonts used in the rtf document. It also
    /// has methods for writing this list to the document
    /// @author Mark Hall (Mark.Hall@mail.room3b.eu)
    /// </summary>
    public class RtfFontList : RtfElement, IRtfExtendedElement
    {

        /// <summary>
        /// Constant for the font number
        /// </summary>
        public static readonly byte[] FontNumber = DocWriter.GetIsoBytes("\\f");

        /// <summary>
        /// Constant for the default font
        /// </summary>
        private static readonly byte[] _defaultFont = DocWriter.GetIsoBytes("\\deff");
        /// <summary>
        /// Constant for the font table
        /// </summary>
        private static readonly byte[] _fontTable = DocWriter.GetIsoBytes("\\fonttbl");
        /// <summary>
        /// The list of fonts
        /// </summary>
        private readonly ArrayList _fontList = new ArrayList();

        /// <summary>
        /// Creates a RtfFontList
        /// </summary>
        /// <param name="doc">The RtfDocument this RtfFontList belongs to</param>
        public RtfFontList(RtfDocument doc) : base(doc)
        {
            _fontList.Add(new RtfFont(Document, 0));
        }

        /// <summary>
        /// Gets the index of the font in the list of fonts. If the font does not
        /// exist in the list, it is added.
        /// </summary>
        /// <param name="font">The font to get the id for</param>
        /// <returns>The index of the font</returns>
        public int GetFontNumber(RtfFont font)
        {
            if (font is RtfParagraphStyle)
            {
                font = new RtfFont(Document, (RtfParagraphStyle)font);
            }
            int fontIndex = -1;
            for (int i = 0; i < _fontList.Count; i++)
            {
                if (_fontList[i].Equals(font))
                {
                    fontIndex = i;
                }
            }
            if (fontIndex == -1)
            {
                fontIndex = _fontList.Count;
                _fontList.Add(font);
            }
            return fontIndex;
        }

        /// <summary>
        /// unused
        /// </summary>
        public override void WriteContent(Stream outp)
        {
        }
        /// <summary>
        /// Writes the definition of the font list
        /// </summary>
        public virtual void WriteDefinition(Stream result)
        {
            byte[] t;
            result.Write(_defaultFont, 0, _defaultFont.Length);
            result.Write(t = IntToByteArray(0), 0, t.Length);
            result.Write(OpenGroup, 0, OpenGroup.Length);
            result.Write(_fontTable, 0, _fontTable.Length);
            for (int i = 0; i < _fontList.Count; i++)
            {
                result.Write(OpenGroup, 0, OpenGroup.Length);
                result.Write(FontNumber, 0, FontNumber.Length);
                result.Write(t = IntToByteArray(i), 0, t.Length);
                RtfFont rf = (RtfFont)_fontList[i];
                rf.WriteDefinition(result);
                result.Write(CommaDelimiter, 0, CommaDelimiter.Length);
                result.Write(CloseGroup, 0, CloseGroup.Length);
            }
            result.Write(CloseGroup, 0, CloseGroup.Length);
            Document.OutputDebugLinebreak(result);
        }
    }
}