using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfDictionary  is the Pdf dictionary object.
///     A dictionary is an associative table containing pairs of objects. The first element
///     of each pair is called the <I>key</I> and the second element is called the <I>value</I>.
///     Unlike dictionaries in the PostScript language, a key must be a  PdfName .
///     A value can be any kind of  PdfObject , including a dictionary. A dictionary is
///     generally used to collect and tie together the attributes of a complex object, with each
///     key-value pair specifying the name and value of an attribute.
///     A dictionary is represented by two left angle brackets , followed by a sequence of
///     key-value pairs, followed by two right angle brackets.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.7 (page 40-41).
///     @see        PdfObject
///     @see        PdfName
///     @see        BadPdfFormatException
/// </summary>
public class PdfDictionary : PdfObject
{
    /// <summary>
    ///     static membervariables (types of dictionary's)
    /// </summary>
    /// <summary>
    ///     This is a possible type of dictionary
    /// </summary>
    public static PdfName Catalog = PdfName.Catalog;

    /// <summary>
    ///     This is a possible type of dictionary
    /// </summary>
    public static PdfName Font = PdfName.Font;

    /// <summary>
    ///     This is a possible type of dictionary
    /// </summary>
    public static PdfName Outlines = PdfName.Outlines;

    /// <summary>
    ///     This is a possible type of dictionary
    /// </summary>
    public static PdfName Page = PdfName.Page;

    /// <summary>
    ///     This is a possible type of dictionary
    /// </summary>
    public static PdfName Pages = PdfName.Pages;

    /// <summary>
    ///     This is the type of this dictionary
    /// </summary>
    private readonly PdfName _dictionaryType;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     This is the hashmap that contains all the values and keys of the dictionary
    /// </summary>
    protected internal INullValueDictionary<PdfName, PdfObject> HashMap;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs an empty  PdfDictionary -object.
    /// </summary>
    public PdfDictionary() : base(DICTIONARY) => HashMap = new NullValueDictionary<PdfName, PdfObject>();

    /// <summary>
    ///     Constructs a  PdfDictionary -object of a certain type.
    /// </summary>
    /// <param name="type">a  PdfName </param>
    public PdfDictionary(PdfName type) : this()
    {
        _dictionaryType = type;
        Put(PdfName.TYPE, _dictionaryType);
    }

    /// <summary>
    ///     methods overriding some methods in PdfObject
    /// </summary>

    public ICollection<PdfName> Keys => HashMap.Keys;

    public int Size => HashMap.Count;

    public bool Contains(PdfName key) => HashMap.ContainsKey(key);

    public PdfObject Get(PdfName key) => HashMap[key];

    public PdfArray GetAsArray(PdfName key)
    {
        PdfArray array = null;
        var orig = GetDirectObject(key);
        if (orig != null && orig.IsArray())
        {
            array = (PdfArray)orig;
        }

        return array;
    }

    public PdfBoolean GetAsBoolean(PdfName key)
    {
        PdfBoolean b = null;
        var orig = GetDirectObject(key);
        if (orig != null && orig.IsBoolean())
        {
            b = (PdfBoolean)orig;
        }

        return b;
    }

    /// <summary>
    ///     All the getAs functions will return either null, or the specified object type
    ///     This function will automatically look up indirect references. There's one obvious
    ///     exception, the one that will only return an indirect reference.  All direct objects
    ///     come back as a null.
    ///     Mark A Storer (2/17/06)
    /// </summary>
    /// <param name="key"></param>
    /// <returns>the appropriate object in its final type, or null</returns>
    public PdfDictionary GetAsDict(PdfName key)
    {
        PdfDictionary dict = null;
        var orig = GetDirectObject(key);
        if (orig != null && orig.IsDictionary())
        {
            dict = (PdfDictionary)orig;
        }

        return dict;
    }

    public PdfIndirectReference GetAsIndirectObject(PdfName key)
    {
        PdfIndirectReference refi = null;
        var orig = Get(key); // not getDirect this time.
        if (orig != null && orig.IsIndirect())
        {
            refi = (PdfIndirectReference)orig;
        }

        return refi;
    }

    public PdfName GetAsName(PdfName key)
    {
        PdfName name = null;
        var orig = GetDirectObject(key);
        if (orig != null && orig.IsName())
        {
            name = (PdfName)orig;
        }

        return name;
    }

    public PdfNumber GetAsNumber(PdfName key)
    {
        PdfNumber number = null;
        var orig = GetDirectObject(key);
        if (orig != null && orig.IsNumber())
        {
            number = (PdfNumber)orig;
        }

        return number;
    }

    public PdfStream GetAsStream(PdfName key)
    {
        PdfStream stream = null;
        var orig = GetDirectObject(key);
        if (orig != null && orig.IsStream())
        {
            stream = (PdfStream)orig;
        }

        return stream;
    }

    public PdfString GetAsString(PdfName key)
    {
        PdfString str = null;
        var orig = GetDirectObject(key);
        if (orig != null && orig.IsString())
        {
            str = (PdfString)orig;
        }

        return str;
    }

    /// <summary>
    ///     This function behaves the same as 'get', but will never return an indirect reference,
    ///     it will always look such references up and return the actual object.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>null, or a non-indirect object</returns>
    public PdfObject GetDirectObject(PdfName key) => PdfReader.GetPdfObject(Get(key));

    public virtual IEnumerator<KeyValuePair<PdfName, PdfObject>> GetEnumerator() => HashMap.GetEnumerator();

    public bool IsCatalog() => Catalog.Equals(_dictionaryType);

    public bool IsFont() => Font.Equals(_dictionaryType);

    public bool IsOutlineTree() => Outlines.Equals(_dictionaryType);

    public bool IsPage() => Page.Equals(_dictionaryType);

    public bool IsPages() => Pages.Equals(_dictionaryType);

    /// <summary>
    ///     Checks if a  Dictionary  is of the type FONT.
    /// </summary>
    /// <returns> true  if it is,  false  if it isn't.</returns>
    /// <summary>
    ///     Checks if a  Dictionary  is of the type PAGE.
    /// </summary>
    /// <returns> true  if it is,  false  if it isn't.</returns>
    /// <summary>
    ///     Checks if a  Dictionary  is of the type PAGES.
    /// </summary>
    /// <returns> true  if it is,  false  if it isn't.</returns>
    /// <summary>
    ///     Checks if a  Dictionary  is of the type CATALOG.
    /// </summary>
    /// <returns> true  if it is,  false  if it isn't.</returns>
    /// <summary>
    ///     Checks if a  Dictionary  is of the type OUTLINES.
    /// </summary>
    /// <returns> true  if it is,  false  if it isn't.</returns>
    public void Merge(PdfDictionary other)
    {
        if (other is null)
        {
            return;
        }

        foreach (var key in other.HashMap.Keys)
        {
            HashMap[key] = other.HashMap[key];
        }
    }

    /// <summary>
    ///     methods concerning the type of Dictionary
    /// </summary>
    public void MergeDifferent(PdfDictionary other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        foreach (var key in other.HashMap.Keys)
        {
            if (!HashMap.ContainsKey(key))
            {
                HashMap[key] = other.HashMap[key];
            }
        }
    }

    public void Put(PdfName key, PdfObject value)
    {
        if (value == null || value.IsNull())
        {
            HashMap.Remove(key);
        }
        else
        {
            HashMap[key] = value;
        }
    }

    /// <summary>
    ///     Adds a  PdfObject  and its key to the  PdfDictionary .
    ///     If the value is null it does nothing.
    /// </summary>
    /// <param name="key">key of the entry (a  PdfName )</param>
    /// <param name="value">value of the entry (a  PdfObject )</param>
    public void PutEx(PdfName key, PdfObject value)
    {
        if (value == null)
        {
            return;
        }

        Put(key, value);
    }

    public void Remove(PdfName key)
    {
        HashMap.Remove(key);
    }

    /// <summary>
    ///     Returns the PDF representation of this  PdfDictionary .
    /// </summary>
    /// <returns>an array of  byte </returns>
    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        os.WriteByte((byte)'<');
        os.WriteByte((byte)'<');

        // loop over all the object-pairs in the Hashtable
        PdfObject value;
        foreach (var key in HashMap.Keys)
        {
            value = HashMap[key];
            key.ToPdf(writer, os);
            var localType = value.Type;
            if (localType != ARRAY && localType != DICTIONARY && localType != NAME && localType != STRING)
            {
                os.WriteByte((byte)' ');
            }

            value.ToPdf(writer, os);
        }

        os.WriteByte((byte)'>');
        os.WriteByte((byte)'>');
    }


    public override string ToString()
    {
        if (Get(PdfName.TYPE) == null)
        {
            return "Dictionary";
        }

        return "Dictionary of type: " + Get(PdfName.TYPE);
    }
}