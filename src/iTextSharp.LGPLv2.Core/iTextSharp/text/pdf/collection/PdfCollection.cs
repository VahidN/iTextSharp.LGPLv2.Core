namespace iTextSharp.text.pdf.collection;

/// <summary>
/// </summary>
public class PdfCollection : PdfDictionary
{
    /// <summary>
    ///     A type of PDF Collection
    /// </summary>
    public const int DETAILS = 0;

    /// <summary>
    ///     A type of PDF Collection
    /// </summary>
    public const int HIDDEN = 2;

    /// <summary>
    ///     A type of PDF Collection
    /// </summary>
    public const int TILE = 1;

    /// <summary>
    ///     Constructs a PDF Collection.
    /// </summary>
    /// <param name="type">the type of PDF collection.</param>
    public PdfCollection(int type) : base(PdfName.Collection)
    {
        switch (type)
        {
            case TILE:
                Put(PdfName.View, PdfName.T);
                break;
            case HIDDEN:
                Put(PdfName.View, PdfName.H);
                break;
            default:
                Put(PdfName.View, PdfName.D);
                break;
        }
    }

    /// <summary>
    ///     Identifies the document that will be initially presented
    ///     in the user interface.
    /// </summary>
    public string InitialDocument
    {
        set => Put(PdfName.D, new PdfString(value, null));
    }

    /// <summary>
    ///     Sets the Collection schema dictionary.
    /// </summary>
    public PdfCollectionSchema Schema
    {
        set => Put(PdfName.Schema, value);
        get => (PdfCollectionSchema)Get(PdfName.Schema);
    }

    /// <summary>
    ///     Sets the Collection sort dictionary.
    /// </summary>
    public PdfCollectionSort Sort
    {
        set => Put(PdfName.Sort, value);
    }
}