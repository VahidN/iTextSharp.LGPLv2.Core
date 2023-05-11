using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfPages  is the PDF Pages-object.
///     The Pages of a document are accessible through a tree of nodes known as the Pages tree.
///     This tree defines the ordering of the pages in the document.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 6.3 (page 71-73)
///     @see        PdfPageElement
///     @see        PdfPage
/// </summary>
public class PdfPages
{
    private readonly List<PdfIndirectReference> _pages = new();
    private readonly List<PdfIndirectReference> _parents = new();
    private readonly PdfWriter _writer;
    private int _leafSize = 10;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a  PdfPages -object.
    /// </summary>
    internal PdfPages(PdfWriter writer) => _writer = writer;

    internal PdfIndirectReference TopParent { get; private set; }

    internal void AddPage(PdfDictionary page)
    {
        if (_pages.Count % _leafSize == 0)
        {
            _parents.Add(_writer.PdfIndirectReference);
        }

        var parent = _parents[_parents.Count - 1];
        page.Put(PdfName.Parent, parent);
        var current = _writer.CurrentPage;
        _writer.AddToBody(page, current);
        _pages.Add(current);
    }

    internal void AddPage(PdfIndirectReference page)
    {
        _pages.Add(page);
    }

    internal PdfIndirectReference AddPageRef(PdfIndirectReference pageRef)
    {
        if (_pages.Count % _leafSize == 0)
        {
            _parents.Add(_writer.PdfIndirectReference);
        }

        _pages.Add(pageRef);
        return _parents[_parents.Count - 1];
    }

    internal int ReorderPages(int[] order)
    {
        if (order == null)
        {
            return _pages.Count;
        }

        if (_parents.Count > 1)
        {
            throw new
                DocumentException("Page reordering requires a single parent in the page tree. Call PdfWriter.SetLinearMode() after open.");
        }

        if (order.Length != _pages.Count)
        {
            throw new DocumentException("Page reordering requires an array with the same size as the number of pages.");
        }

        var max = _pages.Count;
        var temp = new bool[max];
        for (var k = 0; k < max; ++k)
        {
            var p = order[k];
            if (p < 1 || p > max)
            {
                throw new DocumentException("Page reordering requires pages between 1 and " + max + ". Found " + p +
                                            ".");
            }

            if (temp[p - 1])
            {
                throw new DocumentException("Page reordering requires no page repetition. Page " + p + " is repeated.");
            }

            temp[p - 1] = true;
        }

        var copy = _pages.ToArray();
        for (var k = 0; k < max; ++k)
        {
            _pages[k] = copy[order[k] - 1];
        }

        return max;
    }

    internal void SetLinearMode(PdfIndirectReference topParent)
    {
        if (_parents.Count > 1)
        {
            throw new InvalidOperationException("Linear page mode can only be called with a single parent.");
        }

        if (topParent != null)
        {
            TopParent = topParent;
            _parents.Clear();
            _parents.Add(topParent);
        }

        _leafSize = 10000000;
    }

    /// <summary>
    ///     returns the top parent to include in the catalog
    /// </summary>
    internal PdfIndirectReference WritePageTree()
    {
        if (_pages.Count == 0)
        {
            throw new IOException("The document has no pages.");
        }

        var leaf = 1;
        var tParents = _parents;
        var tPages = _pages;
        List<PdfIndirectReference> nextParents = new();
        while (true)
        {
            leaf *= _leafSize;
            var stdCount = _leafSize;
            var rightCount = tPages.Count % _leafSize;
            if (rightCount == 0)
            {
                rightCount = _leafSize;
            }

            for (var p = 0; p < tParents.Count; ++p)
            {
                int count;
                var thisLeaf = leaf;
                if (p == tParents.Count - 1)
                {
                    count = rightCount;
                    thisLeaf = _pages.Count % leaf;
                    if (thisLeaf == 0)
                    {
                        thisLeaf = leaf;
                    }
                }
                else
                {
                    count = stdCount;
                }

                var top = new PdfDictionary(PdfName.Pages);
                top.Put(PdfName.Count, new PdfNumber(thisLeaf));
                var kids = new PdfArray();
                var intern = kids.ArrayList;
                intern.AddRange(tPages.GetRange(p * stdCount, count));
                top.Put(PdfName.Kids, kids);
                if (tParents.Count > 1)
                {
                    if (p % _leafSize == 0)
                    {
                        nextParents.Add(_writer.PdfIndirectReference);
                    }

                    top.Put(PdfName.Parent, nextParents[p / _leafSize]);
                }
                else
                {
                    top.Put(PdfName.Itxt, new PdfString(Document.Release));
                }

                _writer.AddToBody(top, tParents[p]);
            }

            if (tParents.Count == 1)
            {
                TopParent = tParents[0];
                return TopParent;
            }

            tPages = tParents;
            tParents = nextParents;
            nextParents = new List<PdfIndirectReference>();
        }
    }
}