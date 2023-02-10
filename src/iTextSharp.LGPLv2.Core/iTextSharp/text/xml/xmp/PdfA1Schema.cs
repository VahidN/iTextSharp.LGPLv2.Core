namespace iTextSharp.text.xml.xmp;

/// <summary>
///     An implementation of an XmpSchema.
/// </summary>
public class PdfA1Schema : XmpSchema
{
    /// <summary>
    ///     Conformance, A or B.
    /// </summary>
    public const string CONFORMANCE = "pdfaid:conformance";

    /// <summary>
    ///     default namespace identifier
    /// </summary>
    public const string DEFAULT_XPATH_ID = "pdfaid";

    /// <summary>
    ///     default namespace uri
    /// </summary>
    public const string DEFAULT_XPATH_URI = "http://www.aiim.org/pdfa/ns/id/";

    /// <summary>
    ///     Part, always 1.
    /// </summary>
    public const string PART = "pdfaid:part";

    /// <summary>
    ///     @throws IOException
    /// </summary>
    public PdfA1Schema() : base($"xmlns:{DEFAULT_XPATH_ID}=\"{DEFAULT_XPATH_URI}\"")
    {
        AddPart("1");
    }

    /// <summary>
    ///     Adds the conformance.
    /// </summary>
    /// <param name="conformance"></param>
    public void AddConformance(string conformance)
    {
        this[CONFORMANCE] = conformance;
    }

    /// <summary>
    ///     Adds part.
    /// </summary>
    /// <param name="part"></param>
    public void AddPart(string part)
    {
        this[PART] = part;
    }
}