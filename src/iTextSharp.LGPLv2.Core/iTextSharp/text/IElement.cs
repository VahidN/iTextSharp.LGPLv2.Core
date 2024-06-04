namespace iTextSharp.text;

/// <summary>
///     Interface for a text element.
/// </summary>
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