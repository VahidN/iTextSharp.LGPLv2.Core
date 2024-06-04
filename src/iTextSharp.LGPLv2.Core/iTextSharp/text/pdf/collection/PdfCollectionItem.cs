namespace iTextSharp.text.pdf.collection;

public class PdfCollectionItem : PdfDictionary
{
    /// <summary>
    ///     The PdfCollectionSchema with the names and types of the items.
    /// </summary>
    internal readonly PdfCollectionSchema Schema;

    /// <summary>
    ///     Constructs a Collection Item that can be added to a PdfFileSpecification.
    /// </summary>
    public PdfCollectionItem(PdfCollectionSchema schema) : base(PdfName.Collectionitem) => Schema = schema;

    /// <summary>
    ///     Sets the value of the collection item.
    /// </summary>
    public void AddItem(string key, string value)
    {
        var fieldname = new PdfName(key);
        var field = (PdfCollectionField)Schema.Get(fieldname);
        Put(fieldname, field.GetValue(value));
    }

    /// <summary>
    ///     Sets the value of the collection item.
    /// </summary>
    public void AddItem(string key, PdfString value)
    {
        var fieldname = new PdfName(key);
        var field = (PdfCollectionField)Schema.Get(fieldname);
        if (field.FieldType == PdfCollectionField.TEXT)
        {
            Put(fieldname, value);
        }
    }

    /// <summary>
    ///     Sets the value of the collection item.
    /// </summary>
    public void AddItem(string key, PdfDate d)
    {
        var fieldname = new PdfName(key);
        var field = (PdfCollectionField)Schema.Get(fieldname);
        if (field.FieldType == PdfCollectionField.DATE)
        {
            Put(fieldname, d);
        }
    }

    /// <summary>
    ///     Sets the value of the collection item.
    /// </summary>
    public void AddItem(string key, PdfNumber n)
    {
        var fieldname = new PdfName(key);
        var field = (PdfCollectionField)Schema.Get(fieldname);
        if (field.FieldType == PdfCollectionField.NUMBER)
        {
            Put(fieldname, n);
        }
    }

    /// <summary>
    ///     Sets the value of the collection item.
    /// </summary>
    public void AddItem(string key, DateTime c)
    {
        AddItem(key, new PdfDate(c));
    }

    /// <summary>
    ///     Sets the value of the collection item.
    /// </summary>
    public void AddItem(string key, int i)
    {
        AddItem(key, new PdfNumber(i));
    }

    /// <summary>
    ///     Sets the value of the collection item.
    /// </summary>
    public void AddItem(string key, float f)
    {
        AddItem(key, new PdfNumber(f));
    }

    /// <summary>
    ///     Sets the value of the collection item.
    /// </summary>
    public void AddItem(string key, double d)
    {
        AddItem(key, new PdfNumber(d));
    }

    /// <summary>
    ///     Adds a prefix for the Collection item.
    ///     You can only use this method after you have set the value of the item.
    /// </summary>
    public void SetPrefix(string key, string prefix)
    {
        var fieldname = new PdfName(key);
        var o = Get(fieldname);
        if (o == null)
        {
            throw new InvalidOperationException("You must set a value before adding a prefix.");
        }

        var dict = new PdfDictionary(PdfName.Collectionsubitem);
        dict.Put(PdfName.D, o);
        dict.Put(PdfName.P, new PdfString(prefix, TEXT_UNICODE));
        Put(fieldname, dict);
    }
}