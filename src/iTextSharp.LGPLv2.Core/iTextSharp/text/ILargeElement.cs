namespace iTextSharp.text;

/// <summary>
///     Interface implemented by Element objects that can potentially consume
///     a lot of memory. Objects implementing the LargeElement interface can
///     be added to a Document more than once. If you have invoked setCompleted(false),
///     they will be added partially and the content that was added will be
///     removed until you've invoked setCompleted(true);
///     @since   iText 2.0.8
/// </summary>
public interface ILargeElement : IElement
{
    /// <summary>
    ///     If you invoke setCompleted(false), you indicate that the content
    ///     of the object isn't complete yet; it can be added to the document
    ///     partially, but more will follow. If you invoke setCompleted(true),
    ///     you indicate that you won't add any more data to the object.
    ///     @since   iText 2.0.8
    ///     adding the object to the document.
    /// </summary>
    bool ElementComplete { get; set; }

    /// <summary>
    ///     Flushes the content that has been added.
    /// </summary>
    void FlushContent();
}