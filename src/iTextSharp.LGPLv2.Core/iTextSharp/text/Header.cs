using System.Text;

namespace iTextSharp.text;

/// <summary>
///     This is an Element that contains
///     some userdefined meta information about the document.
/// </summary>
/// <example>
///     Header header = new Header("inspired by", "William Shakespeare");
/// </example>
public class Header : Meta
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> This is the content of this chunk of text. </summary>
    private readonly StringBuilder _name;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a Header.
    /// </summary>
    /// <param name="name">the name of the meta-information</param>
    /// <param name="content">the content</param>
    public Header(string name, string content) : base(Element.HEADER, content) => _name = new StringBuilder(name);

    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Returns the name of the meta information.
    /// </summary>
    /// <value>a string</value>
    public override string Name => _name.ToString();
}