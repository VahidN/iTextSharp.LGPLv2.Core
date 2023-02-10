namespace iTextSharp.text.pdf.interfaces;

/// <summary>
/// </summary>
public interface IPdfXConformance
{
    /// <summary>
    ///     Sets the PDF/X conformance level.
    ///     Allowed values are PDFX1A2001, PDFX32002, PDFA1A and PDFA1B.
    ///     It must be called before opening the document.
    /// </summary>
    int PdfxConformance { set; get; }

    /// <summary>
    ///     Checks if the PDF/X Conformance is necessary.
    /// </summary>
    /// <returns>true if the PDF has to be in conformance with any of the PDF/X specifications</returns>
    bool IsPdfX();
}