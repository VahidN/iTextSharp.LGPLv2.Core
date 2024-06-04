namespace iTextSharp.text.pdf.collection;

/// <summary>
/// </summary>
public class PdfCollectionSort : PdfDictionary
{
    /// <summary>
    ///     Constructs a PDF Collection Sort Dictionary.
    /// </summary>
    /// <param name="key">the key of the field that will be used to sort entries</param>
    public PdfCollectionSort(string key) : base(PdfName.Collectionsort)
    {
        Put(PdfName.S, new PdfName(key));
    }

    /// <summary>
    ///     Constructs a PDF Collection Sort Dictionary.
    /// </summary>
    /// <param name="keys">the keys of the fields that will be used to sort entries</param>
    public PdfCollectionSort(string[] keys) : base(PdfName.Collectionsort)
    {
        if (keys == null)
        {
            throw new ArgumentNullException(nameof(keys));
        }

        var array = new PdfArray();
        for (var i = 0; i < keys.Length; i++)
        {
            array.Add(new PdfName(keys[i]));
        }

        Put(PdfName.S, array);
    }

    /// <summary>
    ///     Defines the sort order of the field (ascending or descending).
    /// </summary>
    /// <param name="ascending">true is the default, use false for descending order</param>
    public void SetSortOrder(bool ascending)
    {
        var o = Get(PdfName.S);
        if (o is PdfName)
        {
            Put(PdfName.A, new PdfBoolean(ascending));
        }
        else
        {
            throw new InvalidOperationException("You have to define a bool array for this collection sort dictionary.");
        }
    }

    /// <summary>
    ///     Defines the sort order of the field (ascending or descending).
    /// </summary>
    /// <param name="ascending">an array with every element corresponding with a name of a field.</param>
    public void SetSortOrder(bool[] ascending)
    {
        if (ascending == null)
        {
            throw new ArgumentNullException(nameof(ascending));
        }

        var o = Get(PdfName.S);
        if (o is PdfArray)
        {
            if (((PdfArray)o).Size != ascending.Length)
            {
                throw new
                    InvalidOperationException("The number of booleans in this array doesn't correspond with the number of fields.");
            }

            var array = new PdfArray();
            for (var i = 0; i < ascending.Length; i++)
            {
                array.Add(new PdfBoolean(ascending[i]));
            }

            Put(PdfName.A, array);
        }
        else
        {
            throw new InvalidOperationException("You need a single bool for this collection sort dictionary.");
        }
    }
}