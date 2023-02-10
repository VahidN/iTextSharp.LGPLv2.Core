namespace iTextSharp.text.pdf;

/// <summary>
///     The transparency group dictionary.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfTransparencyGroup : PdfDictionary
{
    /// <summary>
    ///     Constructs a transparencyGroup.
    /// </summary>
    public PdfTransparencyGroup()
    {
        Put(PdfName.S, PdfName.Transparency);
    }

    /// <summary>
    ///     Determining the initial backdrop against which its stack is composited.
    /// </summary>
    public bool Isolated
    {
        set
        {
            if (value)
            {
                Put(PdfName.I, PdfBoolean.Pdftrue);
            }
            else
            {
                Remove(PdfName.I);
            }
        }
    }

    /// <summary>
    ///     Determining whether the objects within the stack are composited with one another or only with the group's backdrop.
    /// </summary>
    public bool Knockout
    {
        set
        {
            if (value)
            {
                Put(PdfName.K, PdfBoolean.Pdftrue);
            }
            else
            {
                Remove(PdfName.K);
            }
        }
    }
}