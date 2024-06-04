namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public class StampContent : PdfContentByte
{
    internal readonly PageResources pageResources;
    internal readonly PdfStamperImp.PageStamp Ps;

    /// <summary>
    ///     Creates a new instance of StampContent
    /// </summary>
    internal StampContent(PdfStamperImp stamper, PdfStamperImp.PageStamp ps) : base(stamper)
    {
        Ps = ps;
        pageResources = ps.PageResources;
    }

    /// <summary>
    ///     Gets a duplicate of this  PdfContentByte . All
    ///     the members are copied by reference but the buffer stays different.
    /// </summary>
    /// <returns>a copy of this  PdfContentByte </returns>
    public override PdfContentByte Duplicate => new StampContent((PdfStamperImp)Writer, Ps);

    internal override PageResources PageResources => pageResources;

    public override void SetAction(PdfAction action, float llx, float lly, float urx, float ury)
    {
        ((PdfStamperImp)Writer).AddAnnotation(new PdfAnnotation(Writer, llx, lly, urx, ury, action), Ps.PageN);
    }

    internal override void AddAnnotation(PdfAnnotation annot)
    {
        ((PdfStamperImp)Writer).AddAnnotation(annot, Ps.PageN);
    }
}