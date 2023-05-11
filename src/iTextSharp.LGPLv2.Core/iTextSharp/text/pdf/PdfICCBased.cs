namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfICCBased  defines a ColorSpace
///     @see        PdfStream
/// </summary>
public class PdfIccBased : PdfStream
{
    /// <summary>
    ///     Creates an ICC stream.
    /// </summary>
    /// <param name="profile">an ICC profile</param>
    public PdfIccBased(IccProfile profile) : this(profile, DEFAULT_COMPRESSION)
    {
    }

    /// <summary>
    ///     Creates an ICC stream.
    ///     @since   2.1.3   (replacing the constructor without param compressionLevel)
    /// </summary>
    /// <param name="compressionLevel">the compressionLevel</param>
    /// <param name="profile">an ICC profile</param>
    public PdfIccBased(IccProfile profile, int compressionLevel)
    {
        if (profile == null)
        {
            throw new ArgumentNullException(nameof(profile));
        }

        var numberOfComponents = profile.NumComponents;
        switch (numberOfComponents)
        {
            case 1:
                Put(PdfName.Alternate, PdfName.Devicegray);
                break;
            case 3:
                Put(PdfName.Alternate, PdfName.Devicergb);
                break;
            case 4:
                Put(PdfName.Alternate, PdfName.Devicecmyk);
                break;
            default:
                throw new PdfException(numberOfComponents + " Component(s) is not supported in iText");
        }

        Put(PdfName.N, new PdfNumber(numberOfComponents));
        Bytes = profile.Data;
        FlateCompress(compressionLevel);
    }
}