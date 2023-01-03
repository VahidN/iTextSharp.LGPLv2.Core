namespace iTextSharp.text;

/// <summary>
///     Interface for a text element.
/// </summary>
/// <seealso cref="T:iTextSharp.text.Anchor" />
/// <seealso cref="T:iTextSharp.text.Cell" />
/// <seealso cref="T:iTextSharp.text.Chapter" />
/// <seealso cref="T:iTextSharp.text.Chunk" />
/// <seealso cref="T:iTextSharp.text.Gif" />
/// <seealso cref="T:iTextSharp.text.Graphic" />
/// <seealso cref="T:iTextSharp.text.Header" />
/// <seealso cref="T:iTextSharp.text.Image" />
/// <seealso cref="T:iTextSharp.text.Jpeg" />
/// <seealso cref="T:iTextSharp.text.List" />
/// <seealso cref="T:iTextSharp.text.ListItem" />
/// <seealso cref="T:iTextSharp.text.Meta" />
/// <seealso cref="T:iTextSharp.text.Paragraph" />
/// <seealso cref="T:iTextSharp.text.Phrase" />
/// <seealso cref="T:iTextSharp.text.Rectangle" />
/// <seealso cref="T:iTextSharp.text.Row" />
/// <seealso cref="T:iTextSharp.text.Section" />
/// <seealso cref="T:iTextSharp.text.Table" />
public interface IElement
{
    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    IList<Chunk> Chunks { get; }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    int Type { get; }

    bool IsContent();

    bool IsNestable();

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     IElementListener.
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    bool Process(IElementListener listener);

    /// <summary>
    ///     Gets the content of the text element.
    /// </summary>
    /// <returns>the content of the text element</returns>
    string ToString();
}