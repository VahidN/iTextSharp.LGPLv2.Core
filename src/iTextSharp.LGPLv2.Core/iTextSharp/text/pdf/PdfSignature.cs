namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the signature dictionary.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfSignature : PdfDictionary
{
    /// <summary>
    ///     Creates new PdfSignature
    /// </summary>
    public PdfSignature(PdfName filter, PdfName subFilter) : base(PdfName.Sig)
    {
        Put(PdfName.Filter, filter);
        Put(PdfName.Subfilter, subFilter);
    }

    public int[] ByteRange
    {
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var array = new PdfArray();
            for (var k = 0; k < value.Length; ++k)
            {
                array.Add(new PdfNumber(value[k]));
            }

            Put(PdfName.Byterange, array);
        }
    }

    public byte[] Cert
    {
        set => Put(PdfName.Cert, new PdfString(value));
    }

    public string Contact
    {
        set => Put(PdfName.Contactinfo, new PdfString(value, TEXT_UNICODE));
    }

    public byte[] Contents
    {
        set => Put(PdfName.Contents, new PdfString(value).SetHexWriting(true));
    }

    public PdfDate Date
    {
        set => Put(PdfName.M, value);
    }

    public string Location
    {
        set => Put(PdfName.Location, new PdfString(value, TEXT_UNICODE));
    }

    public string Name
    {
        set => Put(PdfName.Name, new PdfString(value, TEXT_UNICODE));
    }

    public string Reason
    {
        set => Put(PdfName.Reason, new PdfString(value, TEXT_UNICODE));
    }
}