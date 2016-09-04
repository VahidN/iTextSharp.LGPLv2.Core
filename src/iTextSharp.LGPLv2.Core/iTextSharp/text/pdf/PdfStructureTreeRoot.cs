using System.Collections;

namespace iTextSharp.text.pdf
{

    /// <summary>
    /// The structure tree root corresponds to the highest hierarchy level in a tagged PDF.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class PdfStructureTreeRoot : PdfDictionary
    {
        private readonly Hashtable _parentTree = new Hashtable();

        /// <summary>
        /// Holds value of property writer.
        /// </summary>
        private readonly PdfWriter _writer;

        /// <summary>
        /// Creates a new instance of PdfStructureTreeRoot
        /// </summary>
        internal PdfStructureTreeRoot(PdfWriter writer) : base(PdfName.Structtreeroot)
        {
            _writer = writer;
            Reference = writer.PdfIndirectReference;
        }

        /// <summary>
        /// Gets the reference this object will be written to.
        /// </summary>
        /// <returns>the reference this object will be written to</returns>
        public PdfIndirectReference Reference { get; }

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <returns>the writer</returns>
        public PdfWriter Writer
        {
            get
            {
                return _writer;
            }
        }

        /// <summary>
        /// Maps the user tags to the standard tags. The mapping will allow a standard application to make some sense of the tagged
        /// document whatever the user tags may be.
        /// </summary>
        /// <param name="used">the user tag</param>
        /// <param name="standard">the standard tag</param>
        public void MapRole(PdfName used, PdfName standard)
        {
            PdfDictionary rm = (PdfDictionary)Get(PdfName.Rolemap);
            if (rm == null)
            {
                rm = new PdfDictionary();
                Put(PdfName.Rolemap, rm);
            }
            rm.Put(used, standard);
        }

        internal void BuildTree()
        {
            Hashtable numTree = new Hashtable();
            foreach (int i in _parentTree.Keys)
            {
                PdfArray ar = (PdfArray)_parentTree[i];
                numTree[i] = _writer.AddToBody(ar).IndirectReference;
            }
            PdfDictionary dicTree = PdfNumberTree.WriteTree(numTree, _writer);
            if (dicTree != null)
                Put(PdfName.Parenttree, _writer.AddToBody(dicTree).IndirectReference);

            nodeProcess(this, Reference);
        }

        internal void SetPageMark(int page, PdfIndirectReference struc)
        {
            PdfArray ar = (PdfArray)_parentTree[page];
            if (ar == null)
            {
                ar = new PdfArray();
                _parentTree[page] = ar;
            }
            ar.Add(struc);
        }

        private void nodeProcess(PdfDictionary struc, PdfIndirectReference reference)
        {
            PdfObject obj = struc.Get(PdfName.K);
            if (obj != null && obj.IsArray() && !((PdfObject)((PdfArray)obj).ArrayList[0]).IsNumber())
            {
                PdfArray ar = (PdfArray)obj;
                ArrayList a = ar.ArrayList;
                for (int k = 0; k < a.Count; ++k)
                {
                    PdfStructureElement e = (PdfStructureElement)a[k];
                    a[k] = e.Reference;
                    nodeProcess(e, e.Reference);
                }
            }
            if (reference != null)
                _writer.AddToBody(struc, reference);
        }
    }
}