using System.Collections;
using System.Text;
using System.util;

namespace iTextSharp.text.pdf.events
{

    /// <summary>
    /// Class for an index.
    /// @author Michael Niedermair
    /// </summary>
    public class IndexEvents : PdfPageEventHelper
    {
        /// <summary>
        /// the list for the index entry
        /// </summary>
        private readonly ArrayList _indexentry = new ArrayList();

        /// <summary>
        /// keeps the indextag with the pagenumber
        /// </summary>
        private readonly Hashtable _indextag = new Hashtable();

        /// <summary>
        /// Comparator for sorting the index
        /// </summary>
        private IComparer _comparator = new SortIndex();

        /// <summary>
        /// --------------------------------------------------------------------
        /// </summary>
        /// <summary>
        /// indexcounter
        /// </summary>
        private long _indexcounter;

        /// <summary>
        /// Create an index entry.
        /// </summary>
        /// <param name="text">The text for the Chunk.</param>
        /// <param name="in1">The first level.</param>
        /// <param name="in2">The second level.</param>
        /// <param name="in3">The third level.</param>
        /// <returns>Returns the Chunk.</returns>
        public Chunk Create(string text, string in1, string in2,
                string in3)
        {

            Chunk chunk = new Chunk(text);
            string tag = $"idx_{(_indexcounter++)}";
            chunk.SetGenericTag(tag);
            chunk.SetLocalDestination(tag);
            Entry entry = new Entry(in1, in2, in3, tag, this);
            _indexentry.Add(entry);
            return chunk;
        }

        /// <summary>
        /// Create an index entry.
        /// </summary>
        /// <param name="text">The text for the Chunk.</param>
        /// <param name="in1">The first level.</param>
        /// <returns>Returns the Chunk.</returns>
        public Chunk Create(string text, string in1)
        {
            return Create(text, in1, "", "");
        }

        /// <summary>
        /// Create an index entry.
        /// </summary>
        /// <param name="text">The text for the Chunk.</param>
        /// <param name="in1">The first level.</param>
        /// <param name="in2">The second level.</param>
        /// <returns>Returns the Chunk.</returns>
        public Chunk Create(string text, string in1, string in2)
        {
            return Create(text, in1, in2, "");
        }

        /// <summary>
        /// Create an index entry.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="in1">The first level.</param>
        /// <param name="in2">The second level.</param>
        /// <param name="in3">The third level.</param>
        public void Create(Chunk text, string in1, string in2,
                string in3)
        {

            string tag = $"idx_{(_indexcounter++)}";
            text.SetGenericTag(tag);
            text.SetLocalDestination(tag);
            Entry entry = new Entry(in1, in2, in3, tag, this);
            _indexentry.Add(entry);
        }

        /// <summary>
        /// Create an index entry.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="in1">The first level.</param>
        public void Create(Chunk text, string in1)
        {
            Create(text, in1, "", "");
        }

        /// <summary>
        /// Create an index entry.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="in1">The first level.</param>
        /// <param name="in2">The second level.</param>
        public void Create(Chunk text, string in1, string in2)
        {
            Create(text, in1, in2, "");
        }

        /// <summary>
        /// Returns the sorted list with the entries and the collected page numbers.
        /// </summary>
        /// <returns>Returns the sorted list with the entries and teh collected page numbers.</returns>
        public ArrayList GetSortedEntries()
        {

            Hashtable grouped = new Hashtable();

            for (int i = 0; i < _indexentry.Count; i++)
            {
                Entry e = (Entry)_indexentry[i];
                string key = e.GetKey();

                Entry master = (Entry)grouped[key];
                if (master != null)
                {
                    master.AddPageNumberAndTag(e.GetPageNumber(), e.GetTag());
                }
                else
                {
                    e.AddPageNumberAndTag(e.GetPageNumber(), e.GetTag());
                    grouped[key] = e;
                }
            }

            // copy to a list and sort it
            ArrayList sorted = new ArrayList(grouped.Values);
            sorted.Sort(0, sorted.Count, _comparator);
            return sorted;
        }

        /// <summary>
        /// All the text that is passed to this event, gets registered in the indexentry.
        /// @see com.lowagie.text.pdf.PdfPageEventHelper#onGenericTag(
        /// com.lowagie.text.pdf.PdfWriter, com.lowagie.text.Document,
        /// com.lowagie.text.Rectangle, java.lang.String)
        /// </summary>
        public override void OnGenericTag(PdfWriter writer, Document document,
                Rectangle rect, string text)
        {
            _indextag[text] = writer.PageNumber;
        }
        /// <summary>
        /// Set the comparator.
        /// </summary>
        /// <param name="aComparator">The comparator to set.</param>
        public void SetComparator(IComparer aComparator)
        {
            _comparator = aComparator;
        }

        /// <summary>
        /// --------------------------------------------------------------------
        /// </summary>
        /// <summary>
        /// Class for an index entry.
        ///
        /// In the first step, only in1, in2,in3 and tag are used.
        /// After the collections of the index entries, pagenumbers are used.
        ///
        /// </summary>
        public class Entry
        {

            /// <summary>
            /// first level
            /// </summary>
            private readonly string _in1;

            /// <summary>
            /// second level
            /// </summary>
            private readonly string _in2;

            /// <summary>
            /// third level
            /// </summary>
            private readonly string _in3;

            /// <summary>
            /// the lsit of all page numbers.
            /// </summary>
            private readonly ArrayList _pagenumbers = new ArrayList();

            private readonly IndexEvents _parent;

            /// <summary>
            /// the tag
            /// </summary>
            private readonly string _tag;
            /// <summary>
            /// the lsit of all tags.
            /// </summary>
            private readonly ArrayList _tags = new ArrayList();

            /// <summary>
            /// Create a new object.
            /// </summary>
            /// <param name="aIn1">The first level.</param>
            /// <param name="aIn2">The second level.</param>
            /// <param name="aIn3">The third level.</param>
            /// <param name="aTag">The tag.</param>
            /// <param name="parent"></param>
            public Entry(string aIn1, string aIn2, string aIn3,
                    string aTag, IndexEvents parent)
            {
                _in1 = aIn1;
                _in2 = aIn2;
                _in3 = aIn3;
                _tag = aTag;
                _parent = parent;
            }

            /// <summary>
            /// Add a pagenumber.
            /// </summary>
            /// <param name="number">The page number.</param>
            /// <param name="tag"></param>
            public void AddPageNumberAndTag(int number, string tag)
            {
                _pagenumbers.Add(number);
                _tags.Add(tag);
            }

            /// <summary>
            /// Returns the in1.
            /// </summary>
            /// <returns>Returns the in1.</returns>
            public string GetIn1()
            {
                return _in1;
            }

            /// <summary>
            /// Returns the in2.
            /// </summary>
            /// <returns>Returns the in2.</returns>
            public string GetIn2()
            {
                return _in2;
            }

            /// <summary>
            /// Returns the in3.
            /// </summary>
            /// <returns>Returns the in3.</returns>
            public string GetIn3()
            {
                return _in3;
            }

            /// <summary>
            /// Returns the key for the map-entry.
            /// </summary>
            /// <returns>Returns the key for the map-entry.</returns>
            public string GetKey()
            {
                return $"{_in1}!{_in2}!{_in3}";
            }

            /// <summary>
            /// Returns the pagenumer for this entry.
            /// </summary>
            /// <returns>Returns the pagenumer for this entry.</returns>
            public int GetPageNumber()
            {
                int rt = -1;
                object i = _parent._indextag[_tag];
                if (i != null)
                {
                    rt = (int)i;
                }
                return rt;
            }

            /// <summary>
            /// Returns the pagenumbers.
            /// </summary>
            /// <returns>Returns the pagenumbers.</returns>
            public ArrayList GetPagenumbers()
            {
                return _pagenumbers;
            }

            /// <summary>
            /// Returns the tag.
            /// </summary>
            /// <returns>Returns the tag.</returns>
            public string GetTag()
            {
                return _tag;
            }
            /// <summary>
            /// Returns the tags.
            /// </summary>
            /// <returns>Returns the tags.</returns>
            public ArrayList GetTags()
            {
                return _tags;
            }

            /// <summary>
            /// print the entry (only for test)
            /// </summary>
            /// <returns>the toString implementation of the entry</returns>
            public override string ToString()
            {
                StringBuilder buf = new StringBuilder();
                buf.Append(_in1).Append(' ');
                buf.Append(_in2).Append(' ');
                buf.Append(_in3).Append(' ');
                for (int i = 0; i < _pagenumbers.Count; i++)
                {
                    buf.Append(_pagenumbers[i]).Append(' ');
                }
                return buf.ToString();
            }
        }

        private class SortIndex : IComparer
        {

            public int Compare(object arg0, object arg1)
            {
                Entry en1 = (Entry)arg0;
                Entry en2 = (Entry)arg1;

                int rt = 0;
                if (en1.GetIn1() != null && en2.GetIn1() != null)
                {
                    if ((rt = Util.CompareToIgnoreCase(en1.GetIn1(), en2.GetIn1())) == 0)
                    {
                        // in1 equals
                        if (en1.GetIn2() != null && en2.GetIn2() != null)
                        {
                            if ((rt = Util.CompareToIgnoreCase(en1.GetIn2(), en2.GetIn2())) == 0)
                            {
                                // in2 equals
                                if (en1.GetIn3() != null && en2.GetIn3() != null)
                                {
                                    rt = Util.CompareToIgnoreCase(en1.GetIn3(), en2.GetIn3());
                                }
                            }
                        }
                    }
                }
                return rt;
            }
        }
    }
}