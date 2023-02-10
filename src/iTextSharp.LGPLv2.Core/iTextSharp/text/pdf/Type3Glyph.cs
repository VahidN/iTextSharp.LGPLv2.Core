namespace iTextSharp.text.pdf;

/// <summary>
///     The content where Type3 glyphs are written to.
/// </summary>
public sealed class Type3Glyph : PdfContentByte
{
    private bool _colorized;
    private PageResources _pageResources;

    internal Type3Glyph(PdfWriter writer, PageResources pageResources, float wx, float llx, float lly, float urx,
                        float ury, bool colorized) : base(writer)
    {
        _pageResources = pageResources;
        _colorized = colorized;
        if (colorized)
        {
            Content.Append(wx).Append(" 0 d0\n");
        }
        else
        {
            Content.Append(wx).Append(" 0 ").Append(llx).Append(' ').Append(lly).Append(' ').Append(urx).Append(' ')
                   .Append(ury).Append(" d1\n");
        }
    }

    private Type3Glyph() : base(null)
    {
    }

    internal override PageResources PageResources => _pageResources;

    public override void AddImage(Image image, float a, float b, float c, float d, float e, float f, bool inlineImage)
    {
        if (!_colorized && (!image.IsMask() || !(image.Bpc == 1 || image.Bpc > 0xff)))
        {
            throw new DocumentException("Not colorized Typed3 fonts only accept mask images.");
        }

        base.AddImage(image, a, b, c, d, e, f, inlineImage);
    }

    public PdfContentByte GetDuplicate()
    {
        var dup = new Type3Glyph();
        dup.Writer = Writer;
        dup.Pdf = Pdf;
        dup._pageResources = _pageResources;
        dup._colorized = _colorized;
        return dup;
    }
}