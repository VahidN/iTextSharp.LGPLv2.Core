using System.util;

namespace iTextSharp.text;

/// <summary>
///     Wrapper that allows to add properties to 'basic building block' objects.
///     Before iText 1.5 every 'basic building block' implemented the MarkupAttributes interface.
///     By setting attributes, you could add markup to the corresponding XML and/or HTML tag.
///     This functionality was hardly used by anyone, so it was removed, and replaced by
///     the MarkedObject functionality.
/// </summary>
public class MarkedObject : IElement
{
    /// <summary>
    ///     The element that is wrapped in a MarkedObject.
    /// </summary>
    protected internal IElement Element;

    /// <summary>
    ///     Contains extra markupAttributes
    /// </summary>
    protected internal Properties markupAttributes = new();

    /// <summary>
    ///     Creates a MarkedObject.
    /// </summary>
    public MarkedObject(IElement element) => Element = element;

    /// <summary>
    ///     This constructor is for internal use only.
    /// </summary>
    protected MarkedObject() => Element = null;

    /// <summary>
    /// </summary>
    /// <returns>the markupAttributes</returns>
    public virtual Properties MarkupAttributes => markupAttributes;

    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <returns>an  ArrayList </returns>
    public virtual IList<Chunk> Chunks => Element.Chunks;

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <returns>a type</returns>
    public virtual int Type => text.Element.MARKED;

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsNestable() => true;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     ElementListener .
    /// </summary>
    /// <param name="listener">an  ElementListener </param>
    /// <returns> true  if the element was processed successfully</returns>
    public virtual bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            return listener.Add(Element);
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    public virtual void SetMarkupAttribute(string key, string value)
    {
        markupAttributes.Add(key, value);
    }
}