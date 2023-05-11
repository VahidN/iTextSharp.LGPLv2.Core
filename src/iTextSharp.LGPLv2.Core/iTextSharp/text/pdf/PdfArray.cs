using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfArray  is the PDF Array object.
///     An array is a sequence of PDF objects. An array may contain a mixture of object types.
///     An array is written as a left square bracket ([), followed by a sequence of objects,
///     followed by a right square bracket (]).
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.6 (page 40).
///     @see        PdfObject
/// </summary>
public class PdfArray : PdfObject
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     this is the actual array of PdfObjects
    /// </summary>
    protected List<PdfObject> arrayList;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs an empty  PdfArray -object.
    /// </summary>
    public PdfArray() : base(ARRAY) => arrayList = new List<PdfObject>();

    /// <summary>
    ///     Constructs an  PdfArray -object, containing 1  PdfObject .
    /// </summary>
    /// <param name="obj">a  PdfObject  that has to be added to the array</param>
    public PdfArray(PdfObject obj) : base(ARRAY)
    {
        arrayList = new List<PdfObject>();
        arrayList.Add(obj);
    }

    public PdfArray(float[] values) : base(ARRAY)
    {
        arrayList = new List<PdfObject>();
        Add(values);
    }

    public PdfArray(int[] values) : base(ARRAY)
    {
        arrayList = new List<PdfObject>();
        Add(values);
    }

    /// <summary>
    ///     Constructs a PdfArray with the elements of an ArrayList.
    ///     Throws a ClassCastException if the ArrayList contains something
    ///     that isn't a PdfObject.
    ///     @since 2.1.3
    /// </summary>
    /// <param name="l">an ArrayList with PdfObjects</param>
    public PdfArray(IList<PdfObject> l) : this()
    {
        if (l == null)
        {
            throw new ArgumentNullException(nameof(l));
        }

        foreach (var o in l)
        {
            Add(o);
        }
    }

    /// <summary>
    ///     Constructs an  PdfArray -object, containing all the  PdfObject s in a given  PdfArray .
    /// </summary>
    /// <param name="array">a  PdfArray  that has to be added to the array</param>
    public PdfArray(PdfArray array) : base(ARRAY)
    {
        if (array == null)
        {
            throw new ArgumentNullException(nameof(array));
        }

        arrayList = new List<PdfObject>(array.arrayList);
    }

    /// <summary>
    ///     methods overriding some methods in PdfObject
    /// </summary>
    /// <summary>
    ///     Returns the PDF representation of this  PdfArray .
    /// </summary>
    /// <returns>an array of  byte s</returns>

    public IList<PdfObject> ArrayList => arrayList;

    public int Size => arrayList.Count;

    /// <summary>
    ///     Overwrites a specified location of the array.
    ///     @throws IndexOutOfBoundsException if the specified position doesn't exist
    ///     @since 2.1.5
    /// </summary>
    /// <returns>the previous value</returns>
    public PdfObject this[int idx]
    {
        get => arrayList[idx];
        set => arrayList[idx] = value;
    }

    public virtual bool Add(PdfObject obj)
    {
        arrayList.Add(obj);
        return true;
    }

    /// <summary>
    ///     Adds a  PdfObject  to the  PdfArray .
    /// </summary>
    /// <returns> true </returns>
    public virtual bool Add(float[] values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        for (var k = 0; k < values.Length; ++k)
        {
            arrayList.Add(new PdfNumber(values[k]));
        }

        return true;
    }

    public virtual bool Add(int[] values)
    {
        if (values == null)
        {
            throw new ArgumentNullException(nameof(values));
        }

        for (var k = 0; k < values.Length; ++k)
        {
            arrayList.Add(new PdfNumber(values[k]));
        }

        return true;
    }

    /// <summary>
    ///     Inserts the specified element at the specified position.
    ///     Shifts the element currently at that position (if any) and
    ///     any subsequent elements to the right (adds one to their indices).
    ///     @throws IndexOutOfBoundsException if the specified index is larger than the
    ///     last position currently set, plus 1.
    ///     @since 2.1.5
    /// </summary>
    /// <param name="index">The index at which the specified element is to be inserted</param>
    /// <param name="element">The element to be inserted</param>
    public virtual void Add(int index, PdfObject element)
    {
        arrayList.Insert(index, element);
    }

    /// <summary>
    ///     Inserts a  PdfObject  at the beginning of the
    ///     PdfArray .
    ///     The  PdfObject  will be the first element, any other elements
    ///     will be shifted to the right (adds one to their indices).
    /// </summary>
    /// <param name="obj">The  PdfObject  to add</param>
    public virtual void AddFirst(PdfObject obj)
    {
        arrayList.Insert(0, obj);
    }

    public bool Contains(PdfObject obj) => arrayList.Contains(obj);

    public PdfArray GetAsArray(int idx)
    {
        PdfArray array = null;
        var orig = GetDirectObject(idx);
        if (orig != null && orig.IsArray())
        {
            array = (PdfArray)orig;
        }

        return array;
    }

    public PdfBoolean GetAsBoolean(int idx)
    {
        PdfBoolean b = null;
        var orig = GetDirectObject(idx);
        if (orig != null && orig.IsBoolean())
        {
            b = (PdfBoolean)orig;
        }

        return b;
    }

    /// <summary>
    ///     more of the same like PdfDictionary. (MAS 2/17/06)
    /// </summary>
    public PdfDictionary GetAsDict(int idx)
    {
        PdfDictionary dict = null;
        var orig = GetDirectObject(idx);
        if (orig != null && orig.IsDictionary())
        {
            dict = (PdfDictionary)orig;
        }

        return dict;
    }

    public PdfIndirectReference GetAsIndirectObject(int idx)
    {
        PdfIndirectReference refi = null;
        var orig = this[idx]; // not getDirect this time.
        if (orig != null && orig.IsIndirect())
        {
            refi = (PdfIndirectReference)orig;
        }

        return refi;
    }

    public PdfName GetAsName(int idx)
    {
        PdfName name = null;
        var orig = GetDirectObject(idx);
        if (orig != null && orig.IsName())
        {
            name = (PdfName)orig;
        }

        return name;
    }

    public PdfNumber GetAsNumber(int idx)
    {
        PdfNumber number = null;
        var orig = GetDirectObject(idx);
        if (orig != null && orig.IsNumber())
        {
            number = (PdfNumber)orig;
        }

        return number;
    }

    public PdfStream GetAsStream(int idx)
    {
        PdfStream stream = null;
        var orig = GetDirectObject(idx);
        if (orig != null && orig.IsStream())
        {
            stream = (PdfStream)orig;
        }

        return stream;
    }

    public PdfString GetAsString(int idx)
    {
        PdfString str = null;
        var orig = GetDirectObject(idx);
        if (orig != null && orig.IsString())
        {
            str = (PdfString)orig;
        }

        return str;
    }

    public PdfObject GetDirectObject(int idx) => PdfReader.GetPdfObject(this[idx]);

    /// <summary>
    ///     Checks if the  PdfArray  allready contains a certain  PdfObject .
    /// </summary>
    /// <returns> true </returns>
    public ListIterator<PdfObject> GetListIterator() => new(arrayList);

    /// <summary>
    ///     Returns an ArrayList containing  PdfObject s.
    /// </summary>
    /// <returns>an ArrayList</returns>
    /// <summary>
    ///     Returns the number of entries in the array.
    /// </summary>
    /// <returns>the size of the ArrayList</returns>
    /// <summary>
    ///     Returns  true  if the array is empty.
    ///     @since 2.1.5
    /// </summary>
    /// <returns> true  if the array is empty</returns>
    public bool IsEmpty() => arrayList.Count == 0;

    /// <summary>
    ///     ARRAY CONTENT METHODS
    /// </summary>
    /// <summary>
    ///     Remove the element at the specified position from the array.
    ///     Shifts any subsequent elements to the left (subtracts one from their
    ///     indices).
    ///     @throws IndexOutOfBoundsException the specified position doesn't exist
    ///     @since 2.1.5
    /// </summary>
    /// <param name="idx">The index of the element to be removed.</param>
    public PdfObject Remove(int idx)
    {
        var tmp = arrayList[idx];
        arrayList.RemoveAt(idx);
        return tmp;
    }

    public override void ToPdf(PdfWriter writer, Stream os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        os.WriteByte((byte)'[');
        var first = true;
        PdfObject obj = null;
        foreach (var obja in arrayList)
        {
            obj = obja == null ? PdfNull.Pdfnull : obja;
            type = obj.Type;
            if (!first && type != ARRAY && type != DICTIONARY && type != NAME && type != STRING)
            {
                os.WriteByte((byte)' ');
            }

            first = false;
            obj.ToPdf(writer, os);
        }

        os.WriteByte((byte)']');
    }

    /// <summary>
    ///     methods concerning the ArrayList-membervalue
    /// </summary>
    public override string ToString() => arrayList.ToString();
}