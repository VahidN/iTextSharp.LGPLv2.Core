namespace iTextSharp.text.pdf;

/// <summary>
///     This is a node in a document logical structure. It may contain a mark point or it may contain
///     other nodes.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfStructureElement : PdfDictionary
{
    /// <summary>
    ///     Holds value of property kids.
    /// </summary>
    private readonly PdfStructureElement _parent;

    private readonly PdfStructureTreeRoot _top;

    /// <summary>
    ///     Holds value of property reference.
    /// </summary>
    private PdfIndirectReference _reference;

    /// <summary>
    ///     Creates a new instance of PdfStructureElement.
    /// </summary>
    /// <param name="parent">the parent of this node</param>
    /// <param name="structureType">the type of structure. It may be a standard type or a user type mapped by the role map</param>
    public PdfStructureElement(PdfStructureElement parent, PdfName structureType)
    {
        if (parent == null)
        {
            throw new ArgumentNullException(nameof(parent));
        }

        _top = parent._top;
        init(parent, structureType);
        _parent = parent;
        Put(PdfName.P, parent._reference);
    }

    /// <summary>
    ///     Creates a new instance of PdfStructureElement.
    /// </summary>
    /// <param name="parent">the parent of this node</param>
    /// <param name="structureType">the type of structure. It may be a standard type or a user type mapped by the role map</param>
    public PdfStructureElement(PdfStructureTreeRoot parent, PdfName structureType)
    {
        _top = parent ?? throw new ArgumentNullException(nameof(parent));
        init(parent, structureType);
        Put(PdfName.P, parent.Reference);
    }

    /// <summary>
    ///     Gets the parent of this node.
    /// </summary>
    /// <returns>the parent of this node</returns>
    public PdfDictionary Parent => _parent;

    /// <summary>
    ///     Gets the reference this object will be written to.
    /// </summary>
    /// <returns>the reference this object will be written to</returns>
    public PdfIndirectReference Reference => _reference;

    internal void SetPageMark(int page, int mark)
    {
        if (mark >= 0)
        {
            Put(PdfName.K, new PdfNumber(mark));
        }

        _top.SetPageMark(page, _reference);
    }

    private void init(PdfDictionary parent, PdfName structureType)
    {
        var kido = parent.Get(PdfName.K);
        PdfArray kids = null;
        if (kido != null && !kido.IsArray())
        {
            throw new ArgumentException("The parent has already another function.");
        }

        if (kido == null)
        {
            kids = new PdfArray();
            parent.Put(PdfName.K, kids);
        }
        else
        {
            kids = (PdfArray)kido;
        }

        kids.Add(this);
        Put(PdfName.S, structureType);
        _reference = _top.Writer.PdfIndirectReference;
    }
}