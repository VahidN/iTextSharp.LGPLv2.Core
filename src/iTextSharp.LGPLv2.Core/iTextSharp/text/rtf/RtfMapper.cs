using iTextSharp.text.pdf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.field;
using iTextSharp.text.rtf.graphic;
using iTextSharp.text.rtf.list;
using iTextSharp.text.rtf.table;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf;

/// <summary>
///     The RtfMapper provides mappings between com.lowagie.text.* classes
///     and the corresponding com.lowagie.text.rtf.** classes.
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfMapper
{
    /// <summary>
    ///     The RtfDocument this RtfMapper belongs to
    /// </summary>
    private readonly RtfDocument _rtfDoc;

    /// <summary>
    ///     Constructs a RtfMapper for a RtfDocument
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfMapper belongs to</param>
    public RtfMapper(RtfDocument doc) => _rtfDoc = doc;

    /// <summary>
    ///     Takes an Element subclass and returns an array of RtfBasicElement
    ///     subclasses, that contained the mapped RTF equivalent to the Element
    ///     passed in.
    ///     @throws DocumentException
    /// </summary>
    /// <param name="element">The Element to wrap</param>
    /// <returns>An array of RtfBasicElement wrapping the Element</returns>
    public IRtfBasicElement[] MapElement(IElement element)
    {
        if (element == null)
        {
            throw new ArgumentNullException(nameof(element));
        }

        var rtfElements = new List<IRtfBasicElement>();

        if (element is IRtfBasicElement)
        {
            var rtfElement = (IRtfBasicElement)element;
            rtfElement.SetRtfDocument(_rtfDoc);
            return new[] { rtfElement };
        }

        switch (element.Type)
        {
            case Element.CHUNK:
                var chunk = (Chunk)element;
                if (chunk.HasAttributes())
                {
                    if (chunk.Attributes.ContainsKey(Chunk.IMAGE))
                    {
                        rtfElements.Add(new RtfImage(_rtfDoc, chunk.GetImage()));
                    }
                    else if (chunk.Attributes.ContainsKey(Chunk.NEWPAGE))
                    {
                        rtfElements.Add(new RtfNewPage(_rtfDoc));
                    }
                    else if (chunk.Attributes.TryGetValue(Chunk.TAB, out var attribute))
                    {
                        var tabPos = (float)((object[])attribute)[1];
                        var tab = new RtfTab(tabPos, RtfTab.TAB_LEFT_ALIGN);
                        tab.SetRtfDocument(_rtfDoc);
                        rtfElements.Add(tab);
                        rtfElements.Add(new RtfChunk(_rtfDoc, new Chunk("\t")));
                    }
                    else
                    {
                        rtfElements.Add(new RtfChunk(_rtfDoc, (Chunk)element));
                    }
                }
                else
                {
                    rtfElements.Add(new RtfChunk(_rtfDoc, (Chunk)element));
                }

                break;
            case Element.PHRASE:
                rtfElements.Add(new RtfPhrase(_rtfDoc, (Phrase)element));
                break;
            case Element.PARAGRAPH:
                rtfElements.Add(new RtfParagraph(_rtfDoc, (Paragraph)element));
                break;
            case Element.ANCHOR:
                rtfElements.Add(new RtfAnchor(_rtfDoc, (Anchor)element));
                break;
            case Element.ANNOTATION:
                rtfElements.Add(new RtfAnnotation(_rtfDoc, (Annotation)element));
                break;
            case Element.IMGRAW:
            case Element.IMGTEMPLATE:
            case Element.JPEG:
                rtfElements.Add(new RtfImage(_rtfDoc, (Image)element));
                break;
            case Element.AUTHOR:
            case Element.SUBJECT:
            case Element.KEYWORDS:
            case Element.TITLE:
            case Element.PRODUCER:
            case Element.CREATIONDATE:
                rtfElements.Add(new RtfInfoElement(_rtfDoc, (Meta)element));
                break;
            case Element.LIST:
                rtfElements.Add(new RtfList(_rtfDoc, (List)element));
                break;
            case Element.LISTITEM:
                rtfElements.Add(new RtfListItem(_rtfDoc, (ListItem)element));
                break;
            case Element.SECTION:
                rtfElements.Add(new RtfSection(_rtfDoc, (Section)element));
                break;
            case Element.CHAPTER:
                rtfElements.Add(new RtfChapter(_rtfDoc, (Chapter)element));
                break;
            case Element.TABLE:
                try
                {
                    rtfElements.Add(new RtfTable(_rtfDoc, (Table)element));
                }
                catch (InvalidCastException)
                {
                    rtfElements.Add(new RtfTable(_rtfDoc, ((SimpleTable)element).CreateTable()));
                }

                break;
            case Element.PTABLE:
                try
                {
                    rtfElements.Add(new RtfTable(_rtfDoc, (PdfPTable)element));
                }
                catch (InvalidCastException)
                {
                    rtfElements.Add(new RtfTable(_rtfDoc, ((SimpleTable)element).CreateTable()));
                }

                break;
        }

        return rtfElements.ToArray();
    }
}