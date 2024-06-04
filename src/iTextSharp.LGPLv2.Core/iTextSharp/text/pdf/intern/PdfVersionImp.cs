using iTextSharp.text.pdf.interfaces;

namespace iTextSharp.text.pdf.intern;

/// <summary>
///     Stores the PDF version information,
///     knows how to write a PDF Header,
///     and how to add the version to the catalog (if necessary).
/// </summary>
public class PdfVersionImp : IPdfVersion
{
    /// <summary>
    ///     Contains different strings that are part of the header.
    /// </summary>
    public static readonly byte[][] Header =
    {
        DocWriter.GetIsoBytes("\n"),
        DocWriter.GetIsoBytes("%PDF-"),
        DocWriter.GetIsoBytes("\n%\u00e2\u00e3\u00cf\u00d3\n"),
    };

    /// <summary>
    ///     Indicates if we are working in append mode.
    /// </summary>
    protected bool Appendmode;

    /// <summary>
    ///     The version that will be written to the catalog.
    /// </summary>
    protected PdfName CatalogVersion;

    /// <summary>
    ///     The extensions dictionary.
    ///     @since	2.1.6
    /// </summary>
    protected PdfDictionary Extensions;

    /// <summary>
    ///     The version that was or will be written to the header.
    /// </summary>
    protected char HeaderVersion = PdfWriter.VERSION_1_4;

    /// <summary>
    ///     Indicates if the header was already written.
    /// </summary>
    protected bool HeaderWasWritten;

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(char)
    /// </summary>
    public char PdfVersion
    {
        set
        {
            if (HeaderWasWritten || Appendmode)
            {
                SetPdfVersion(GetVersionAsName(value));
            }
            else
            {
                HeaderVersion = value;
            }
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfVersion#addDeveloperExtension(com.lowagie.text.pdf.PdfDeveloperExtension)
    ///     @since   2.1.6
    /// </summary>
    public void AddDeveloperExtension(PdfDeveloperExtension de)
    {
        if (de == null)
        {
            throw new ArgumentNullException(nameof(de));
        }

        if (Extensions == null)
        {
            Extensions = new PdfDictionary();
        }
        else
        {
            var extension = Extensions.GetAsDict(de.Prefix);
            if (extension != null)
            {
                var diff = de.Baseversion.CompareTo(extension.GetAsName(PdfName.Baseversion));
                if (diff < 0)
                {
                    return;
                }

                diff = de.ExtensionLevel - extension.GetAsNumber(PdfName.Extensionlevel).IntValue;
                if (diff <= 0)
                {
                    return;
                }
            }
        }

        Extensions.Put(de.Prefix, de.GetDeveloperExtensions());
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfVersion#setAtLeastPdfVersion(char)
    /// </summary>
    public void SetAtLeastPdfVersion(char version)
    {
        if (version > HeaderVersion)
        {
            PdfVersion = version;
        }
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(com.lowagie.text.pdf.PdfName)
    /// </summary>
    public void SetPdfVersion(PdfName version)
    {
        if (CatalogVersion == null || CatalogVersion.CompareTo(version) < 0)
        {
            CatalogVersion = version;
        }
    }

    /// <summary>
    ///     Adds the version to the Catalog dictionary.
    /// </summary>
    public void AddToCatalog(PdfDictionary catalog)
    {
        if (catalog == null)
        {
            throw new ArgumentNullException(nameof(catalog));
        }

        if (CatalogVersion != null)
        {
            catalog.Put(PdfName.Version, CatalogVersion);
        }

        if (Extensions != null)
        {
            catalog.Put(PdfName.Extensions, Extensions);
        }
    }

    /// <summary>
    ///     Returns the version as a byte[].
    /// </summary>
    /// <param name="version">the version character</param>
    public static byte[] GetVersionAsByteArray(char version) =>
        DocWriter.GetIsoBytes(GetVersionAsName(version).ToString().Substring(1));

    /// <summary>
    ///     Returns the PDF version as a name.
    /// </summary>
    /// <param name="version">the version character.</param>
    public static PdfName GetVersionAsName(char version)
    {
        switch (version)
        {
            case PdfWriter.VERSION_1_2:
                return PdfWriter.PdfVersion12;
            case PdfWriter.VERSION_1_3:
                return PdfWriter.PdfVersion13;
            case PdfWriter.VERSION_1_4:
                return PdfWriter.PdfVersion14;
            case PdfWriter.VERSION_1_5:
                return PdfWriter.PdfVersion15;
            case PdfWriter.VERSION_1_6:
                return PdfWriter.PdfVersion16;
            case PdfWriter.VERSION_1_7:
                return PdfWriter.PdfVersion17;
            default:
                return PdfWriter.PdfVersion14;
        }
    }

    /// <summary>
    ///     Sets the append mode.
    /// </summary>
    public void SetAppendmode(bool appendmode)
    {
        Appendmode = appendmode;
    }

    /// <summary>
    ///     Writes the header to the OutputStreamCounter.
    ///     @throws IOException
    /// </summary>
    public void WriteHeader(OutputStreamCounter os)
    {
        if (os == null)
        {
            throw new ArgumentNullException(nameof(os));
        }

        if (Appendmode)
        {
            os.Write(Header[0], 0, Header[0].Length);
        }
        else
        {
            os.Write(Header[1], 0, Header[1].Length);
            os.Write(GetVersionAsByteArray(HeaderVersion), 0, GetVersionAsByteArray(HeaderVersion).Length);
            os.Write(Header[2], 0, Header[2].Length);
            HeaderWasWritten = true;
        }
    }
}