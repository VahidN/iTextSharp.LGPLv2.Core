using System.Text;

namespace iTextSharp.text.pdf;

/// <summary>
///     PdfIndirectReference  contains a reference to a  PdfIndirectObject .
///     Any object used as an element of an array or as a value in a dictionary may be specified
///     by either a direct object of an indirect reference. An <I>indirect reference</I> is a reference
///     to an indirect object, and consists of the indirect object's object number, generation number
///     and the <B>R</B> keyword.
///     This object is described in the 'Portable Document Format Reference Manual version 1.3'
///     section 4.11 (page 54).
///     @see        PdfObject
///     @see        PdfIndirectObject
/// </summary>
public class PdfIndirectReference : PdfObject
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     the generation number
    /// </summary>
    protected int generation;

    /// <summary>
    ///     the object number
    /// </summary>
    protected int number;

    /// <summary>
    ///     constructors
    /// </summary>
    internal PdfIndirectReference(int type, int number, int generation) :
        base(0, new StringBuilder().Append(number).Append(' ').Append(generation).Append(" R").ToString())
    {
        this.number = number;
        this.generation = generation;
    }

    internal PdfIndirectReference(int type, int number) : this(type, number, 0)
    {
    }

    protected PdfIndirectReference() : base(0)
    {
    }


    public int Generation => generation;

    public int Number => number;

    /// <summary>
    ///     Returns the generation of the object.
    /// </summary>
    /// <returns>a number.</returns>
    public override string ToString() =>
        new StringBuilder().Append(number).Append(' ').Append(generation).Append(" R").ToString();
}