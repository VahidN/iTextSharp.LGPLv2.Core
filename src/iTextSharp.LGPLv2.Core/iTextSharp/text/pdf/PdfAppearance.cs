using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Implements the appearance stream to be used with form fields..
/// </summary>
public class PdfAppearance : PdfTemplate
{
    public static INullValueDictionary<string, PdfName> StdFieldFontNames =
        new NullValueDictionary<string, PdfName>();

    static PdfAppearance()
    {
        StdFieldFontNames["Courier-BoldOblique"] = new PdfName("CoBO");
        StdFieldFontNames["Courier-Bold"] = new PdfName("CoBo");
        StdFieldFontNames["Courier-Oblique"] = new PdfName("CoOb");
        StdFieldFontNames["Courier"] = new PdfName("Cour");
        StdFieldFontNames["Helvetica-BoldOblique"] = new PdfName("HeBO");
        StdFieldFontNames["Helvetica-Bold"] = new PdfName("HeBo");
        StdFieldFontNames["Helvetica-Oblique"] = new PdfName("HeOb");
        StdFieldFontNames["Helvetica"] = PdfName.Helv;
        StdFieldFontNames["Symbol"] = new PdfName("Symb");
        StdFieldFontNames["Times-BoldItalic"] = new PdfName("TiBI");
        StdFieldFontNames["Times-Bold"] = new PdfName("TiBo");
        StdFieldFontNames["Times-Italic"] = new PdfName("TiIt");
        StdFieldFontNames["Times-Roman"] = new PdfName("TiRo");
        StdFieldFontNames["ZapfDingbats"] = PdfName.Zadb;
        StdFieldFontNames["HYSMyeongJo-Medium"] = new PdfName("HySm");
        StdFieldFontNames["HYGoThic-Medium"] = new PdfName("HyGo");
        StdFieldFontNames["HeiseiKakuGo-W5"] = new PdfName("KaGo");
        StdFieldFontNames["HeiseiMin-W3"] = new PdfName("KaMi");
        StdFieldFontNames["MHei-Medium"] = new PdfName("MHei");
        StdFieldFontNames["MSung-Light"] = new PdfName("MSun");
        StdFieldFontNames["STSong-Light"] = new PdfName("STSo");
        StdFieldFontNames["MSungStd-Light"] = new PdfName("MSun");
        StdFieldFontNames["STSongStd-Light"] = new PdfName("STSo");
        StdFieldFontNames["HYSMyeongJoStd-Medium"] = new PdfName("HySm");
        StdFieldFontNames["KozMinPro-Regular"] = new PdfName("KaMi");
    }

    /// <summary>
    ///     Creates a  PdfAppearance .
    /// </summary>
    internal PdfAppearance() => Separator = ' ';

    internal PdfAppearance(PdfIndirectReference iref) => ThisReference = iref;

    /// <summary>
    ///     Creates new PdfTemplate
    /// </summary>
    /// <param name="wr">the  PdfWriter </param>
    internal PdfAppearance(PdfWriter wr) : base(wr) => Separator = ' ';

    public override PdfContentByte Duplicate
    {
        get
        {
            var tpl = new PdfAppearance();
            tpl.Writer = Writer;
            tpl.Pdf = Pdf;
            tpl.ThisReference = ThisReference;
            tpl.pageResources = pageResources;
            tpl.BBox = new Rectangle(BBox);
            tpl.group = group;
            tpl.layer = layer;
            if (matrix != null)
            {
                tpl.matrix = new PdfArray(matrix);
            }

            tpl.Separator = Separator;
            return tpl;
        }
    }

    /// <summary>
    ///     Creates a new appearance to be used with form fields.
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="width">the bounding box width</param>
    /// <param name="height">the bounding box height</param>
    /// <returns>the appearance created</returns>
    public static PdfAppearance CreateAppearance(PdfWriter writer, float width, float height)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        return CreateAppearance(writer, width, height, null);
    }

    /// <summary>
    ///     Set the font and the size for the subsequent text writing.
    /// </summary>
    /// <param name="bf">the font</param>
    /// <param name="size">the font size in points</param>
    public override void SetFontAndSize(BaseFont bf, float size)
    {
        if (bf == null)
        {
            throw new ArgumentNullException(nameof(bf));
        }

        CheckWriter();
        State.size = size;
        if (bf.FontType == BaseFont.FONT_TYPE_DOCUMENT)
        {
            State.FontDetails = new FontDetails(null, ((DocumentFont)bf).IndirectReference, bf);
        }
        else
        {
            State.FontDetails = Writer.AddSimple(bf);
        }

        var psn = StdFieldFontNames[bf.PostscriptFontName];
        if (psn == null)
        {
            if (bf.Subset && bf.FontType == BaseFont.FONT_TYPE_TTUNI)
            {
                psn = State.FontDetails.FontName;
            }
            else
            {
                psn = new PdfName(bf.PostscriptFontName);
                State.FontDetails.Subset = false;
            }
        }

        var prs = PageResources;
        prs.AddFont(psn, State.FontDetails.IndirectReference);
        Content.Append(psn.GetBytes()).Append(' ').Append(size).Append(" Tf").Append_i(Separator);
    }

    internal static PdfAppearance CreateAppearance(PdfWriter writer, float width, float height, PdfName forcedName)
    {
        var template = new PdfAppearance(writer);
        template.Width = width;
        template.Height = height;
        writer.AddDirectTemplateSimple(template, forcedName);
        return template;
    }
}