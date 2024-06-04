namespace iTextSharp.text.pdf;

/// <summary>
///     Implements PDF functions.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfFunction
{
    protected PdfDictionary Dictionary;
    protected PdfIndirectReference reference;
    protected PdfWriter Writer;

    /// <summary>
    ///     Creates new PdfFunction
    /// </summary>
    protected PdfFunction(PdfWriter writer) => Writer = writer;

    internal PdfIndirectReference Reference
    {
        get
        {
            if (reference == null)
            {
                reference = Writer.AddToBody(Dictionary).IndirectReference;
            }

            return reference;
        }
    }

    public static PdfFunction Type0(PdfWriter writer, float[] domain, float[] range, int[] size,
                                    int bitsPerSample, int order, float[] encode, float[] decode, byte[] stream)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        var func = new PdfFunction(writer);
        func.Dictionary = new PdfStream(stream);
        ((PdfStream)func.Dictionary).FlateCompress(writer.CompressionLevel);
        func.Dictionary.Put(PdfName.Functiontype, new PdfNumber(0));
        func.Dictionary.Put(PdfName.Domain, new PdfArray(domain));
        func.Dictionary.Put(PdfName.Range, new PdfArray(range));
        func.Dictionary.Put(PdfName.Size, new PdfArray(size));
        func.Dictionary.Put(PdfName.Bitspersample, new PdfNumber(bitsPerSample));
        if (order != 1)
        {
            func.Dictionary.Put(PdfName.Order, new PdfNumber(order));
        }

        if (encode != null)
        {
            func.Dictionary.Put(PdfName.Encode, new PdfArray(encode));
        }

        if (decode != null)
        {
            func.Dictionary.Put(PdfName.Decode, new PdfArray(decode));
        }

        return func;
    }

    public static PdfFunction Type2(PdfWriter writer, float[] domain, float[] range, float[] c0, float[] c1, float n)
    {
        var func = new PdfFunction(writer);
        func.Dictionary = new PdfDictionary();
        func.Dictionary.Put(PdfName.Functiontype, new PdfNumber(2));
        func.Dictionary.Put(PdfName.Domain, new PdfArray(domain));
        if (range != null)
        {
            func.Dictionary.Put(PdfName.Range, new PdfArray(range));
        }

        if (c0 != null)
        {
            func.Dictionary.Put(PdfName.C0, new PdfArray(c0));
        }

        if (c1 != null)
        {
            func.Dictionary.Put(PdfName.C1, new PdfArray(c1));
        }

        func.Dictionary.Put(PdfName.N, new PdfNumber(n));
        return func;
    }

    public static PdfFunction Type3(PdfWriter writer, float[] domain, float[] range, PdfFunction[] functions,
                                    float[] bounds, float[] encode)
    {
        if (functions == null)
        {
            throw new ArgumentNullException(nameof(functions));
        }

        var func = new PdfFunction(writer);
        func.Dictionary = new PdfDictionary();
        func.Dictionary.Put(PdfName.Functiontype, new PdfNumber(3));
        func.Dictionary.Put(PdfName.Domain, new PdfArray(domain));
        if (range != null)
        {
            func.Dictionary.Put(PdfName.Range, new PdfArray(range));
        }

        var array = new PdfArray();
        for (var k = 0; k < functions.Length; ++k)
        {
            array.Add(functions[k].Reference);
        }

        func.Dictionary.Put(PdfName.Functions, array);
        func.Dictionary.Put(PdfName.Bounds, new PdfArray(bounds));
        func.Dictionary.Put(PdfName.Encode, new PdfArray(encode));
        return func;
    }

    public static PdfFunction Type4(PdfWriter writer, float[] domain, float[] range, string postscript)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (postscript == null)
        {
            throw new ArgumentNullException(nameof(postscript));
        }

        var b = new byte[postscript.Length];
        for (var k = 0; k < b.Length; ++k)
        {
            b[k] = (byte)postscript[k];
        }

        var func = new PdfFunction(writer);
        func.Dictionary = new PdfStream(b);
        ((PdfStream)func.Dictionary).FlateCompress(writer.CompressionLevel);
        func.Dictionary.Put(PdfName.Functiontype, new PdfNumber(4));
        func.Dictionary.Put(PdfName.Domain, new PdfArray(domain));
        func.Dictionary.Put(PdfName.Range, new PdfArray(range));
        return func;
    }
}