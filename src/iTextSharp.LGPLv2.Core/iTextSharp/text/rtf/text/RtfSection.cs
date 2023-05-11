using System.Text;
using iTextSharp.text.rtf.document;
using FD = iTextSharp.text.rtf.field;

namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfSection wraps a Section element.
///     INTERNAL CLASS
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfSection : RtfElement
{
    /// <summary>
    ///     The sub-items of this RtfSection
    /// </summary>
    protected List<IRtfBasicElement> Items;

    /// <summary>
    ///     The title paragraph of this RtfSection
    /// </summary>
    protected RtfParagraph Title;

    /// <summary>
    ///     Constructs a RtfSection for a given Section. If the autogenerateTOCEntries
    ///     property of the RtfDocument is set and the title is not empty then a TOC entry
    ///     is generated for the title.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfSection belongs to</param>
    /// <param name="section">The Section this RtfSection is based on</param>
    public RtfSection(RtfDocument doc, Section section) : base(doc)
    {
        if (doc == null)
        {
            throw new ArgumentNullException(nameof(doc));
        }

        if (section == null)
        {
            throw new ArgumentNullException(nameof(section));
        }

        Items = new List<IRtfBasicElement>();
        try
        {
            if (section.Title != null)
            {
                Title = (RtfParagraph)doc.GetMapper().MapElement(section.Title)[0];
            }

            if (Document.GetAutogenerateTocEntries())
            {
                var titleText = new StringBuilder();
                foreach (var element in section.Title)
                {
                    if (element.Type == Element.CHUNK)
                    {
                        titleText.Append(((Chunk)element).Content);
                    }
                }

                if (titleText.ToString().Trim().Length > 0)
                {
                    var tocEntry = new FD.RtfTocEntry(titleText.ToString());
                    tocEntry.SetRtfDocument(Document);
                    Items.Add(tocEntry);
                }
            }

            foreach (var element in section)
            {
                var rtfElements = doc.GetMapper().MapElement(element);
                for (var i = 0; i < rtfElements.Length; i++)
                {
                    if (rtfElements[i] != null)
                    {
                        Items.Add(rtfElements[i]);
                    }
                }
            }

            updateIndentation(section.IndentationLeft, section.IndentationRight, section.Indentation);
        }
        catch (DocumentException)
        {
        }
    }

    /// <summary>
    ///     Sets whether this RtfSection is in a header. Sets the correct inTable setting for all
    ///     child elements.
    /// </summary>
    /// <param name="inHeader"> True  if this RtfSection is in a header,  false  otherwise</param>
    public override void SetInHeader(bool inHeader)
    {
        base.SetInHeader(inHeader);
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].SetInHeader(inHeader);
        }
    }

    /// <summary>
    ///     Sets whether this RtfSection is in a table. Sets the correct inTable setting for all
    ///     child elements.
    /// </summary>
    /// <param name="inTable"> True  if this RtfSection is in a table,  false  otherwise</param>
    public override void SetInTable(bool inTable)
    {
        base.SetInTable(inTable);
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].SetInTable(inTable);
        }
    }

    /// <summary>
    ///     Write this RtfSection and its contents
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(RtfParagraph.Paragraph, 0, RtfParagraph.Paragraph.Length);
        if (Title != null)
        {
            Title.WriteContent(outp);
        }

        foreach (var rbe in Items)
        {
            rbe.WriteContent(outp);
        }
    }

    /// <summary>
    ///     Updates the left, right and content indentation of all RtfParagraph and RtfSection
    ///     elements that this RtfSection contains.
    /// </summary>
    /// <param name="indentLeft">The left indentation to add.</param>
    /// <param name="indentRight">The right indentation to add.</param>
    /// <param name="indentContent">The content indentation to add.</param>
    private void updateIndentation(float indentLeft, float indentRight, float indentContent)
    {
        if (Title != null)
        {
            Title.SetIndentLeft((int)(Title.GetIndentLeft() + indentLeft * TWIPS_FACTOR));
            Title.SetIndentRight((int)(Title.GetIndentRight() + indentRight * TWIPS_FACTOR));
        }

        for (var i = 0; i < Items.Count; i++)
        {
            var rtfElement = Items[i];
            if (rtfElement is RtfSection)
            {
                ((RtfSection)rtfElement).updateIndentation(indentLeft + indentContent, indentRight, 0);
            }
            else if (rtfElement is RtfParagraph)
            {
                ((RtfParagraph)rtfElement).SetIndentLeft((int)(((RtfParagraph)rtfElement).GetIndentLeft() +
                                                               (indentLeft + indentContent) * TWIPS_FACTOR));
                ((RtfParagraph)rtfElement).SetIndentRight((int)(((RtfParagraph)rtfElement).GetIndentRight() +
                                                                indentRight * TWIPS_FACTOR));
            }
        }
    }
}