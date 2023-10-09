using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     This class captures an AcroForm on input. Basically, it extends Dictionary
///     by indexing the fields of an AcroForm
///     @author Mark Thompson
/// </summary>
public class PrAcroForm : PdfDictionary
{
    internal readonly INullValueDictionary<string, FieldInformation> FieldByName;

    internal readonly List<FieldInformation> fields;

    internal readonly PdfReader Reader;

    internal readonly List<PdfDictionary> Stack;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="reader">reader of the input file</param>
    public PrAcroForm(PdfReader reader)
    {
        Reader = reader;
        fields = new List<FieldInformation>();
        FieldByName = new NullValueDictionary<string, FieldInformation>();
        Stack = new List<PdfDictionary>();
    }

    public IList<FieldInformation> Fields => fields;

    /// <summary>
    ///     Number of fields found
    /// </summary>
    /// <returns>size</returns>
    public new int Size => fields.Count;

    public FieldInformation GetField(string name) => FieldByName[name];

    /// <summary>
    ///     Given the title (/T) of a reference, return the associated reference
    /// </summary>
    /// <param name="name">a string containing the path</param>
    /// <returns>a reference to the field, or null</returns>
    public PrIndirectReference GetRefByName(string name)
    {
        var fi = FieldByName[name];
        if (fi == null)
        {
            return null;
        }

        return fi.Ref;
    }

    /// <summary>
    ///     Read, and comprehend the acroform
    /// </summary>
    /// <param name="root">the docment root</param>
    public void ReadAcroForm(PdfDictionary root)
    {
        if (root == null)
        {
            return;
        }

        HashMap = root.HashMap;
        PushAttrib(root);
        var fieldlist = (PdfArray)PdfReader.GetPdfObjectRelease(root.Get(PdfName.Fields));
        IterateFields(fieldlist, null, null);
    }

    /// <summary>
    ///     After reading, we index all of the fields. Recursive.
    /// </summary>
    /// <param name="fieldlist">An array of fields</param>
    /// <param name="fieldDict">the last field dictionary we encountered (recursively)</param>
    /// <param name="title">the pathname of the field, up to this point or null</param>
    protected void IterateFields(PdfArray fieldlist, PrIndirectReference fieldDict, string title)
    {
        if (fieldlist == null)
        {
            throw new ArgumentNullException(nameof(fieldlist));
        }

        foreach (PrIndirectReference refi in fieldlist.ArrayList)
        {
            var dict = (PdfDictionary)PdfReader.GetPdfObjectRelease(refi);

            // if we are not a field dictionary, pass our parent's values
            var myFieldDict = fieldDict;
            var myTitle = title;
            var tField = (PdfString)dict.Get(PdfName.T);
            var isFieldDict = tField != null;

            if (isFieldDict)
            {
                myFieldDict = refi;
                if (title == null)
                {
                    myTitle = tField.ToString();
                }
                else
                {
                    myTitle = title + '.' + tField;
                }
            }

            var kids = (PdfArray)dict.Get(PdfName.Kids);
            if (kids != null)
            {
                PushAttrib(dict);
                IterateFields(kids, myFieldDict, myTitle);
                Stack.RemoveAt(Stack.Count - 1); // pop
            }
            else
            {
                // leaf node
                if (myFieldDict != null)
                {
                    var mergedDict = Stack[Stack.Count - 1];
                    if (isFieldDict)
                    {
                        mergedDict = MergeAttrib(mergedDict, dict);
                    }

                    mergedDict.Put(PdfName.T, new PdfString(myTitle));
                    var fi = new FieldInformation(myTitle, mergedDict, myFieldDict);
                    fields.Add(fi);
                    FieldByName[myTitle] = fi;
                }
            }
        }
    }

    /// <summary>
    ///     merge field attributes from two dictionaries
    /// </summary>
    /// <param name="parent">one dictionary</param>
    /// <param name="child">the other dictionary</param>
    /// <returns>a merged dictionary</returns>
    protected static PdfDictionary MergeAttrib(PdfDictionary parent, PdfDictionary child)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        var targ = new PdfDictionary();
        if (parent != null)
        {
            targ.Merge(parent);
        }

        foreach (var key in child.Keys)
        {
            if (key.Equals(PdfName.Dr) || key.Equals(PdfName.Da) ||
                key.Equals(PdfName.Q) || key.Equals(PdfName.Ff) ||
                key.Equals(PdfName.Dv) || key.Equals(PdfName.V)
                || key.Equals(PdfName.Ft)
                || key.Equals(PdfName.F))
            {
                targ.Put(key, child.Get(key));
            }
        }

        return targ;
    }

    /// <summary>
    ///     stack a level of dictionary. Merge in a dictionary from this level
    /// </summary>
    protected void PushAttrib(PdfDictionary dict)
    {
        PdfDictionary dic = null;
        if (Stack.Count != 0)
        {
            dic = Stack[Stack.Count - 1];
        }

        dic = MergeAttrib(dic, dict);
        Stack.Add(dic);
    }

    /// <summary>
    ///     This class holds the information for a single field
    /// </summary>
    public class FieldInformation
    {
        internal readonly PdfDictionary info;
        internal readonly string name;
        internal readonly PrIndirectReference Refi;

        internal FieldInformation(string name, PdfDictionary info, PrIndirectReference refi)
        {
            this.name = name;
            this.info = info;
            Refi = refi;
        }

        public PdfDictionary Info => info;

        public string Name => name;

        public PrIndirectReference Ref => Refi;
    }
}