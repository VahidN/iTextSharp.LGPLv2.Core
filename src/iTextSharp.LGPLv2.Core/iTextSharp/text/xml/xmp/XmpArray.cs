using System.Text;

namespace iTextSharp.text.xml.xmp;

/// <summary>
///     StringBuilder to construct an XMP array.
/// </summary>
public class XmpArray : List<string>
{
    /// <summary>
    ///     An array with alternatives.
    /// </summary>
    public const string ALTERNATIVE = "rdf:Alt";

    /// <summary>
    ///     An array that is ordered.
    /// </summary>
    public const string ORDERED = "rdf:Seq";

    /// <summary>
    ///     An array that is unordered.
    /// </summary>
    public const string UNORDERED = "rdf:Bag";

    /// <summary>
    ///     the type of array.
    /// </summary>
    protected string Type;

    /// <summary>
    ///     Creates an XmpArray.
    /// </summary>
    /// <param name="type">the type of array: UNORDERED, ORDERED or ALTERNATIVE.</param>
    public XmpArray(string type) => Type = type;

    /// <summary>
    ///     Returns the String representation of the XmpArray.
    /// </summary>
    /// <returns>a String representation</returns>
    public override string ToString()
    {
        var buf = new StringBuilder("<");
        buf.Append(Type);
        buf.Append('>');
        foreach (var s in this)
        {
            buf.Append("<rdf:li>");
            buf.Append(XmpSchema.Escape(s));
            buf.Append("</rdf:li>");
        }

        buf.Append("</");
        buf.Append(Type);
        buf.Append('>');
        return buf.ToString();
    }
}