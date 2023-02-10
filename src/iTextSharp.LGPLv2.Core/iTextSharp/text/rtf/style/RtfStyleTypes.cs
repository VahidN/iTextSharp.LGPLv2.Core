namespace iTextSharp.text.rtf.style;

/// <summary>
///     RtfStyleTypes  contains the different types of Stylesheet entries
///     that exist in RTF.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.0.8
/// </summary>
public sealed class RtfStyleTypes
{
    /// <summary>
    ///     Indicates paragraph style.
    /// </summary>
    public const int PARAGRAPH = 0;

    /// <summary>
    ///     Indicates character style.
    /// </summary>
    public const int CHARACTER = 0;

    /// <summary>
    ///     Indicates section style.
    /// </summary>
    public const int SECTION = 2;

    /// <summary>
    ///     Indicates Table style.
    /// </summary>
    public const int TABLE = 3;

    /// <summary>
    ///     Indicates table definition style.
    /// </summary>
    public const int TABLE_STYLE_DEFINITION = 4;
}