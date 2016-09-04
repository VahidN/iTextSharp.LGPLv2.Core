namespace iTextSharp.text.pdf
{
    /// <summary>
    /// The graphic state dictionary.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class PdfGState : PdfDictionary
    {
        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmColorburn = new PdfName("ColorBurn");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmColordodge = new PdfName("ColorDodge");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmCompatible = new PdfName("Compatible");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmDarken = new PdfName("Darken");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmDifference = new PdfName("Difference");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmExclusion = new PdfName("Exclusion");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmHardlight = new PdfName("HardLight");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmLighten = new PdfName("Lighten");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmMultiply = new PdfName("Multiply");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmNormal = new PdfName("Normal");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmOverlay = new PdfName("Overlay");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmScreen = new PdfName("Screen");

        /// <summary>
        /// A possible blend mode
        /// </summary>
        public static readonly PdfName BmSoftlight = new PdfName("SoftLight");

        /// <summary>
        /// The alpha source flag specifying whether the current soft mask
        /// and alpha constant are to be interpreted as shape values (true)
        /// or opacity values (false).
        /// </summary>
        public bool AlphaIsShape
        {
            set
            {
                Put(PdfName.Ais, value ? PdfBoolean.Pdftrue : PdfBoolean.Pdffalse);
            }
        }

        /// <summary>
        /// The current blend mode to be used in the transparent imaging model.
        /// </summary>
        public PdfName BlendMode
        {
            set
            {
                Put(PdfName.Bm, value);
            }
        }

        /// <summary>
        /// Sets the current stroking alpha constant, specifying the constant shape or
        /// constant opacity value to be used for nonstroking operations in the transparent
        /// imaging model.
        /// </summary>
        public float FillOpacity
        {
            set
            {
                Put(PdfName.CA_, new PdfNumber(value));
            }
        }

        /// <summary>
        /// Sets the flag whether to toggle knockout behavior for overprinted objects.
        /// </summary>
        public int OverPrintMode
        {
            set
            {
                Put(PdfName.Opm, new PdfNumber(value == 0 ? 0 : 1));
            }
        }

        /// <summary>
        /// Sets the flag whether to apply overprint for non stroking painting operations.
        /// </summary>
        public bool OverPrintNonStroking
        {
            set
            {
                Put(PdfName.Op_, value ? PdfBoolean.Pdftrue : PdfBoolean.Pdffalse);
            }
        }

        /// <summary>
        /// Sets the flag whether to apply overprint for stroking.
        /// </summary>
        public bool OverPrintStroking
        {
            set
            {
                Put(PdfName.Op, value ? PdfBoolean.Pdftrue : PdfBoolean.Pdffalse);
            }
        }

        /// <summary>
        /// Sets the current stroking alpha constant, specifying the constant shape or
        /// constant opacity value to be used for stroking operations in the transparent
        /// imaging model.
        /// </summary>
        public float StrokeOpacity
        {
            set
            {
                Put(PdfName.CA, new PdfNumber(value));
            }
        }

        /// <summary>
        /// Determines the behaviour of overlapping glyphs within a text object
        /// in the transparent imaging model.
        /// </summary>
        public bool TextKnockout
        {
            set
            {
                Put(PdfName.Tk, value ? PdfBoolean.Pdftrue : PdfBoolean.Pdffalse);
            }
        }
    }
}
