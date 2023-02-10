namespace iTextSharp.text.pdf;

/// <summary>
///     The interface common to all layer types.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public interface IPdfOcg
{
    /// <summary>
    ///     Gets the object representing the layer.
    /// </summary>
    /// <returns>the object representing the layer</returns>
    PdfObject PdfObject { get; }

    /// <summary>
    ///     Gets the  PdfIndirectReference  that represents this layer.
    /// </summary>
    /// <returns>the  PdfIndirectReference  that represents this layer</returns>
    PdfIndirectReference Ref { get; }
}