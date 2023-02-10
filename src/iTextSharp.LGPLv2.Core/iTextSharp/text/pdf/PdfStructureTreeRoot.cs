using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     The structure tree root corresponds to the highest hierarchy level in a tagged PDF.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfStructureTreeRoot : PdfDictionary
{
    private readonly INullValueDictionary<int, PdfObject> _parentTree = new NullValueDictionary<int, PdfObject>();

    /// <summary>
    ///     Holds value of property writer.
    /// </summary>
    private readonly PdfWriter _writer;

    /// <summary>
    ///     Creates a new instance of PdfStructureTreeRoot
    /// </summary>
    internal PdfStructureTreeRoot(PdfWriter writer) : base(PdfName.Structtreeroot)
    {
        _writer = writer;
        Reference = writer.PdfIndirectReference;
    }

    /// <summary>
    ///     Gets the reference this object will be written to.
    /// </summary>
    /// <returns>the reference this object will be written to</returns>
    public PdfIndirectReference Reference { get; }

    /// <summary>
    ///     Gets the writer.
    /// </summary>
    /// <returns>the writer</returns>
    public PdfWriter Writer => _writer;

    /// <summary>
    ///     Maps the user tags to the standard tags. The mapping will allow a standard application to make some sense of the
    ///     tagged
    ///     document whatever the user tags may be.
    /// </summary>
    /// <param name="used">the user tag</param>
    /// <param name="standard">the standard tag</param>
    public void MapRole(PdfName used, PdfName standard)
    {
        var rm = (PdfDictionary)Get(PdfName.Rolemap);
        if (rm == null)
        {
            rm = new PdfDictionary();
            Put(PdfName.Rolemap, rm);
        }

        rm.Put(used, standard);
    }

    internal void BuildTree()
    {
        var numTree = new NullValueDictionary<int, PdfIndirectReference>();
        foreach (var i in _parentTree.Keys)
        {
            var ar = (PdfArray)_parentTree[i];
            numTree[i] = _writer.AddToBody(ar).IndirectReference;
        }

        var dicTree = PdfNumberTree.WriteTree(numTree, _writer);
        if (dicTree != null)
        {
            Put(PdfName.Parenttree, _writer.AddToBody(dicTree).IndirectReference);
        }

        nodeProcess(this, Reference);
    }

    internal void SetPageMark(int page, PdfIndirectReference struc)
    {
        var ar = (PdfArray)_parentTree[page];
        if (ar == null)
        {
            ar = new PdfArray();
            _parentTree[page] = ar;
        }

        ar.Add(struc);
    }

    private void nodeProcess(PdfDictionary struc, PdfIndirectReference reference)
    {
        var obj = struc.Get(PdfName.K);
        if (obj != null && obj.IsArray() && !((PdfArray)obj).ArrayList[0].IsNumber())
        {
            var ar = (PdfArray)obj;
            var a = ar.ArrayList;
            for (var k = 0; k < a.Count; ++k)
            {
                var e = (PdfStructureElement)a[k];
                a[k] = e.Reference;
                nodeProcess(e, e.Reference);
            }
        }

        if (reference != null)
        {
            _writer.AddToBody(struc, reference);
        }
    }
}