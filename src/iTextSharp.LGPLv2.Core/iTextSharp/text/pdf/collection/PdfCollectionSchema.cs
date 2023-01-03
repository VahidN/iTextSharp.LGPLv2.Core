namespace iTextSharp.text.pdf.collection;

public class PdfCollectionSchema : PdfDictionary
{
    /// <summary>
    ///     Creates a Collection Schema dictionary.
    /// </summary>
    public PdfCollectionSchema() : base(PdfName.Collectionschema)
    {
    }

    /// <summary>
    ///     Adds a Collection field to the Schema.
    /// </summary>
    /// <param name="name">the name of the collection field</param>
    /// <param name="field">a Collection Field</param>
    public void AddField(string name, PdfCollectionField field)
    {
        Put(new PdfName(name), field);
    }
}