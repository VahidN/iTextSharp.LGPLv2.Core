namespace iTextSharp.text.pdf.collection;

/// <summary>
///     @author blowagie
/// </summary>
public class PdfCollectionField : PdfDictionary
{
    /// <summary>
    ///     A possible type of collection field.
    /// </summary>
    public const int CREATIONDATE = 6;

    /// <summary>
    ///     A possible type of collection field.
    /// </summary>
    public const int DATE = 1;

    /// <summary>
    ///     A possible type of collection field.
    /// </summary>
    public const int DESC = 4;

    /// <summary>
    ///     A possible type of collection field.
    /// </summary>
    public const int FILENAME = 3;

    /// <summary>
    ///     A possible type of collection field.
    /// </summary>
    public const int MODDATE = 5;

    /// <summary>
    ///     A possible type of collection field.
    /// </summary>
    public new const int NUMBER = 2;

    /// <summary>
    ///     A possible type of collection field.
    /// </summary>
    public const int SIZE = 7;

    /// <summary>
    ///     A possible type of collection field.
    /// </summary>
    public const int TEXT = 0;

    /// <summary>
    ///     The type of the PDF collection field.
    /// </summary>
    protected internal int FieldType;

    /// <summary>
    ///     Creates a PdfCollectionField.
    /// </summary>
    /// <param name="name">the field name</param>
    /// <param name="type">the field type</param>
    public PdfCollectionField(string name, int type) : base(PdfName.Collectionfield)
    {
        Put(PdfName.N, new PdfString(name, TEXT_UNICODE));
        FieldType = type;
        switch (type)
        {
            default:
                Put(PdfName.Subtype, PdfName.S);
                break;
            case DATE:
                Put(PdfName.Subtype, PdfName.D);
                break;
            case NUMBER:
                Put(PdfName.Subtype, PdfName.N);
                break;
            case FILENAME:
                Put(PdfName.Subtype, PdfName.F);
                break;
            case DESC:
                Put(PdfName.Subtype, PdfName.Desc);
                break;
            case MODDATE:
                Put(PdfName.Subtype, PdfName.Moddate);
                break;
            case CREATIONDATE:
                Put(PdfName.Subtype, PdfName.Creationdate);
                break;
            case SIZE:
                Put(PdfName.Subtype, PdfName.Size);
                break;
        }
    }

    /// <summary>
    ///     Indication if the field value should be editable in the viewer.
    /// </summary>
    public bool Editable
    {
        set => Put(PdfName.E, new PdfBoolean(value));
    }

    /// <summary>
    ///     The relative order of the field name. Fields are sorted in ascending order.
    /// </summary>
    public int Order
    {
        set => Put(PdfName.O, new PdfNumber(value));
    }

    /// <summary>
    ///     Sets the initial visibility of the field.
    /// </summary>
    public bool Visible
    {
        set => Put(PdfName.V, new PdfBoolean(value));
    }

    /// <summary>
    ///     Returns a PdfObject that can be used as the value of a Collection Item.
    /// </summary>
    public PdfObject GetValue(string v)
    {
        switch (FieldType)
        {
            case TEXT:
                return new PdfString(v, TEXT_UNICODE);
            case DATE:
                return new PdfDate(PdfDate.Decode(v));
            case NUMBER:
                return new PdfNumber(v);
        }

        throw new InvalidOperationException($"{v} is not an acceptable value for the field {Get(PdfName.N)}");
    }

    /// <summary>
    ///     Checks if the type of the field is suitable for a Collection Item.
    /// </summary>
    public bool IsCollectionItem()
    {
        switch (FieldType)
        {
            case TEXT:
            case DATE:
            case NUMBER:
                return true;
            default:
                return false;
        }
    }
}