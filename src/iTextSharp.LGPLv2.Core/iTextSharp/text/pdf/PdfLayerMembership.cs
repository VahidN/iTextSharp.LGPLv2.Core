using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Content typically belongs to a single optional content group,
///     and is visible when the group is <B>ON</B> and invisible when it is <B>OFF</B>. To express more
///     complex visibility policies, content should not declare itself to belong to an optional
///     content group directly, but rather to an optional content membership dictionary
///     represented by this class.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfLayerMembership : PdfDictionary, IPdfOcg
{
    /// <summary>
    ///     Visible only if all of the entries are <B>OFF</B>.
    /// </summary>
    public static readonly PdfName Alloff = new("AllOff");

    /// <summary>
    ///     Visible only if all of the entries are <B>ON</B>.
    /// </summary>
    public static readonly PdfName Allon = new("AllOn");

    /// <summary>
    ///     Visible if any of the entries are <B>OFF</B>.
    /// </summary>
    public static readonly PdfName Anyoff = new("AnyOff");

    /// <summary>
    ///     Visible if any of the entries are <B>ON</B>.
    /// </summary>
    public static readonly PdfName Anyon = new("AnyOn");

    internal readonly INullValueDictionary<PdfLayer, object> layers = new NullValueDictionary<PdfLayer, object>();
    internal readonly PdfArray Members = new();
    internal readonly PdfIndirectReference Refi;

    /// <summary>
    ///     Creates a new, empty, membership layer.
    /// </summary>
    /// <param name="writer">the writer</param>
    public PdfLayerMembership(PdfWriter writer) : base(PdfName.Ocmd)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        Put(PdfName.Ocgs, Members);
        Refi = writer.PdfIndirectReference;
    }

    /// <summary>
    ///     Gets the member layers.
    /// </summary>
    /// <returns>the member layers</returns>
    public ICollection<PdfLayer> Layers => layers.Keys;

    /// <summary>
    ///     Sets the visibility policy for content belonging to this
    ///     membership dictionary. Possible values are ALLON, ANYON, ANYOFF and ALLOFF.
    ///     The default value is ANYON.
    /// </summary>
    public PdfName VisibilityPolicy
    {
        set => Put(PdfName.P, value);
    }

    /// <summary>
    ///     Gets the dictionary representing the membership layer. It just returns  this .
    /// </summary>
    /// <returns>the dictionary representing the layer</returns>
    public PdfObject PdfObject => this;

    /// <summary>
    ///     Gets the  PdfIndirectReference  that represents this membership layer.
    /// </summary>
    /// <returns>the  PdfIndirectReference  that represents this layer</returns>
    public PdfIndirectReference Ref => Refi;

    /// <summary>
    ///     Adds a new member to the layer.
    /// </summary>
    /// <param name="layer">the new member to the layer</param>
    public void AddMember(PdfLayer layer)
    {
        if (layer == null)
        {
            throw new ArgumentNullException(nameof(layer));
        }

        if (!layers.ContainsKey(layer))
        {
            Members.Add(layer.Ref);
            layers[layer] = null;
        }
    }
}