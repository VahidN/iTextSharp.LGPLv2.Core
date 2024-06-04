using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     PdfTemplate that has to be inserted into the document
/// </summary>
public class ImgTemplate : Image
{
    /// <summary>
    ///     Creats an Image from a PdfTemplate.
    /// </summary>
    /// <param name="image">the Image</param>
    public ImgTemplate(Image image) : base(image)
    {
    }

    /// <summary>
    ///     Creats an Image from a PdfTemplate.
    /// </summary>
    /// <param name="template">the PdfTemplate</param>
    public ImgTemplate(PdfTemplate template) : base((Uri)null)
    {
        if (template == null)
        {
            throw new BadElementException("The template can not be null.");
        }

        if (template.Type == PdfTemplate.TYPE_PATTERN)
        {
            throw new BadElementException("A pattern can not be used as a template to create an image.");
        }

        type = IMGTEMPLATE;
        scaledHeight = template.Height;
        Top = scaledHeight;
        scaledWidth = template.Width;
        Right = scaledWidth;
        TemplateData = template;
        plainWidth = Width;
        plainHeight = Height;
    }
}