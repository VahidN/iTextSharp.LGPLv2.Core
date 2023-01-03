namespace iTextSharp.text.pdf.interfaces;

public interface IPdfAnnotations
{
    /// <summary>
    ///     Use this methods to get the AcroForm object.
    ///     Use this method only if you know what you're doing
    /// </summary>
    /// <returns>the PdfAcroform object of the PdfDocument</returns>
    PdfAcroForm AcroForm { get; }

    /// <summary>
    ///     Use this method to set the signature flags.
    /// </summary>
    int SigFlags { set; }

    /// <summary>
    ///     Use this methods to add a  PdfAnnotation  or a  PdfFormField
    ///     to the document. Only the top parent of a  PdfFormField
    ///     needs to be added.
    /// </summary>
    /// <param name="annot">the  PdfAnnotation  or the  PdfFormField  to add</param>
    void AddAnnotation(PdfAnnotation annot);

    /// <summary>
    ///     Use this method to adds the  PdfAnnotation
    ///     to the calculation order array.
    /// </summary>
    /// <param name="annot">the  PdfAnnotation  to be added</param>
    void AddCalculationOrder(PdfFormField annot);
}