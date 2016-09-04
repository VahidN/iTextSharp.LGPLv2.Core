using System.IO;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.text;
using iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.list
{
    /// <summary>
    /// The RtfListItem acts as a wrapper for a ListItem.
    /// @author Mark Hall (Mark.Hall@mail.room3b.eu)
    /// </summary>
    public class RtfListItem : RtfParagraph
    {
        /// <summary>
        /// Whether this RtfListItem contains further RtfLists.
        /// </summary>
        private bool _containsInnerList;

        private int _level;

        /// <summary>
        /// The RtfList this RtfListItem belongs to.
        /// </summary>
        private RtfListLevel _parentList;
        /// <summary>
        /// Constructs a RtfListItem for a ListItem belonging to a RtfDocument.
        /// </summary>
        /// <param name="doc">The RtfDocument this RtfListItem belongs to.</param>
        /// <param name="listItem">The ListItem this RtfListItem is based on.</param>
        public RtfListItem(RtfDocument doc, ListItem listItem) : base(doc, listItem)
        {
        }

        /// <summary>
        /// @since 2.1.3
        /// </summary>
        /// <returns>the level</returns>
        public int GetLevel()
        {
            return _level;
        }

        /// <summary>
        /// Set the parent RtfList.
        /// @since 2.1.3
        /// </summary>
        /// <returns>The parent RtfList to use.</returns>
        public RtfListLevel GetParent()
        {
            return _parentList;
        }

        /// <summary>
        /// Inherit the list settings from the parent list to RtfLists that
        /// are contained in this RtfListItem.
        /// </summary>
        /// <param name="listNumber">The list number to inherit.</param>
        /// <param name="listLevel">The list level to inherit.</param>
        public void InheritListSettings(int listNumber, int listLevel)
        {
            for (int i = 0; i < Chunks.Count; i++)
            {
                IRtfBasicElement rtfElement = (IRtfBasicElement)Chunks[i];
                if (rtfElement is RtfList)
                {
                    ((RtfList)rtfElement).SetListNumber(listNumber);
                    SetLevel(listLevel);
                }
            }
        }

        /// <summary>
        /// Gets whether this RtfListItem contains further RtfLists.
        /// </summary>
        /// <returns>Whether this RtfListItem contains further RtfLists.</returns>
        public bool IsContainsInnerList()
        {
            return _containsInnerList;
        }

        /// <summary>
        /// @since 2.1.3
        /// </summary>
        /// <param name="level">the level to set</param>
        public void SetLevel(int level)
        {
            _level = level;
        }

        /// <summary>
        /// Set the parent RtfList.
        /// </summary>
        /// <param name="parentList">The parent RtfList to use.</param>
        public void SetParent(RtfListLevel parentList)
        {
            _parentList = parentList;
        }

        /// <summary>
        /// Writes the content of this RtfListItem.
        /// </summary>
        public override void WriteContent(Stream result)
        {
            byte[] t;
            if (ParagraphStyle.GetSpacingBefore() > 0)
            {
                result.Write(RtfParagraphStyle.SpacingBefore, 0, RtfParagraphStyle.SpacingBefore.Length);
                result.Write(t = IntToByteArray(ParagraphStyle.GetSpacingBefore()), 0, t.Length);
            }
            if (ParagraphStyle.GetSpacingAfter() > 0)
            {
                result.Write(RtfParagraphStyle.SpacingAfter, 0, RtfParagraphStyle.SpacingAfter.Length);
                result.Write(t = IntToByteArray(ParagraphStyle.GetSpacingAfter()), 0, t.Length);
            }
            if (ParagraphStyle.GetLineLeading() > 0)
            {
                result.Write(LineSpacing, 0, LineSpacing.Length);
                result.Write(t = IntToByteArray(ParagraphStyle.GetLineLeading()), 0, t.Length);
            }
            for (int i = 0; i < Chunks.Count; i++)
            {
                IRtfBasicElement rtfElement = (IRtfBasicElement)Chunks[i];
                if (rtfElement is RtfChunk)
                {
                    ((RtfChunk)rtfElement).SetSoftLineBreaks(true);
                }
                else if (rtfElement is RtfList)
                {
                    result.Write(Paragraph, 0, Paragraph.Length);
                    _containsInnerList = true;
                }
                rtfElement.WriteContent(result);
                if (rtfElement is RtfList)
                {
                    switch (_parentList.GetLevelFollowValue())
                    {
                        case RtfListLevel.LIST_LEVEL_FOLLOW_NOTHING:
                            break;
                        case RtfListLevel.LIST_LEVEL_FOLLOW_TAB:
                            _parentList.WriteListBeginning(result);
                            result.Write(RtfList.Tab, 0, RtfList.Tab.Length);
                            break;
                        case RtfListLevel.LIST_LEVEL_FOLLOW_SPACE:
                            _parentList.WriteListBeginning(result);
                            result.Write(t = DocWriter.GetIsoBytes(" "), 0, t.Length);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Writes the definition of the first element in this RtfListItem that is
        /// an is {@link RtfList} to the given stream.
        /// If this item does not contain a {@link RtfList} element nothing is written
        /// and the method returns  false .
        /// @throws IOException
        /// @see {@link RtfList#writeDefinition(Stream)}
        /// </summary>
        /// <param name="outp">destination stream</param>
        /// <returns> true  if a RtfList definition was written,  false  otherwise</returns>
        public bool WriteDefinition(Stream outp)
        {
            for (int i = 0; i < Chunks.Count; i++)
            {
                IRtfBasicElement rtfElement = (IRtfBasicElement)Chunks[i];
                if (rtfElement is RtfList)
                {
                    RtfList rl = (RtfList)rtfElement;
                    rl.WriteDefinition(outp);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Correct the indentation of RtfLists in this RtfListItem by adding left/first line indentation
        /// from the parent RtfList. Also calls correctIndentation on all child RtfLists.
        /// </summary>
        protected internal void CorrectIndentation()
        {
            for (int i = 0; i < Chunks.Count; i++)
            {
                IRtfBasicElement rtfElement = (IRtfBasicElement)Chunks[i];
                if (rtfElement is RtfList)
                {
                    ((RtfList)rtfElement).CorrectIndentation();
                }
            }
        }
    }
}