using System.util;
using iTextSharp.text.pdf.interfaces;

namespace iTextSharp.text.pdf.intern;

/// <summary>
/// </summary>
public class PdfXConformanceImp : IPdfXConformance
{
    /// <summary>
    ///     A key for an aspect that can be checked for PDF/X Conformance.
    /// </summary>
    public const int PDFXKEY_CMYK = 2;

    /// <summary>
    ///     A key for an aspect that can be checked for PDF/X Conformance.
    /// </summary>
    public const int PDFXKEY_COLOR = 1;

    /// <summary>
    ///     A key for an aspect that can be checked for PDF/X Conformance.
    /// </summary>
    public const int PDFXKEY_FONT = 4;

    /// <summary>
    ///     A key for an aspect that can be checked for PDF/X Conformance.
    /// </summary>
    public const int PDFXKEY_GSTATE = 6;

    /// <summary>
    ///     A key for an aspect that can be checked for PDF/X Conformance.
    /// </summary>
    public const int PDFXKEY_IMAGE = 5;

    /// <summary>
    ///     A key for an aspect that can be checked for PDF/X Conformance.
    /// </summary>
    public const int PDFXKEY_LAYER = 7;

    /// <summary>
    ///     A key for an aspect that can be checked for PDF/X Conformance.
    /// </summary>
    public const int PDFXKEY_RGB = 3;

    /// <summary>
    ///     The value indicating if the PDF has to be in conformance with PDF/X.
    /// </summary>
    public int PdfxConformance { set; get; } = PdfWriter.PDFXNONE;

    /// <summary>
    ///     Checks if the PDF/X Conformance is necessary.
    /// </summary>
    /// <returns>true if the PDF has to be in conformance with any of the PDF/X specifications</returns>
    public bool IsPdfX() => PdfxConformance != PdfWriter.PDFXNONE;

    /// <summary>
    ///     Business logic that checks if a certain object is in conformance with PDF/X.
    /// </summary>
    /// <param name="writer">the writer that is supposed to write the PDF/X file</param>
    /// <param name="key">the type of PDF/X conformance that has to be checked</param>
    /// <param name="obj1">the object that is checked for conformance</param>
    public static void CheckPdfxConformance(PdfWriter writer, int key, object obj1)
    {
        if (writer == null || !writer.IsPdfX())
        {
            return;
        }

        var conf = writer.PdfxConformance;
        switch (key)
        {
            case PDFXKEY_COLOR:
                switch (conf)
                {
                    case PdfWriter.PDFX1A2001:
                        if (obj1 is ExtendedColor)
                        {
                            var ec = (ExtendedColor)obj1;
                            switch (ec.Type)
                            {
                                case ExtendedColor.TYPE_CMYK:
                                case ExtendedColor.TYPE_GRAY:
                                    return;
                                case ExtendedColor.TYPE_RGB:
                                    throw new PdfXConformanceException("Colorspace RGB is not allowed.");
                                case ExtendedColor.TYPE_SEPARATION:
                                    var sc = (SpotColor)ec;
                                    CheckPdfxConformance(writer, PDFXKEY_COLOR, sc.PdfSpotColor.AlternativeCs);
                                    break;
                                case ExtendedColor.TYPE_SHADING:
                                    var xc = (ShadingColor)ec;
                                    CheckPdfxConformance(writer, PDFXKEY_COLOR,
                                                         xc.PdfShadingPattern.Shading.ColorSpace);
                                    break;
                                case ExtendedColor.TYPE_PATTERN:
                                    var pc = (PatternColor)ec;
                                    CheckPdfxConformance(writer, PDFXKEY_COLOR, pc.Painter.DefaultColor);
                                    break;
                            }
                        }
                        else if (obj1 is BaseColor)
                        {
                            throw new PdfXConformanceException("Colorspace RGB is not allowed.");
                        }

                        break;
                }

                break;
            case PDFXKEY_CMYK:
                break;
            case PDFXKEY_RGB:
                if (conf == PdfWriter.PDFX1A2001)
                {
                    throw new PdfXConformanceException("Colorspace RGB is not allowed.");
                }

                break;
            case PDFXKEY_FONT:
                if (obj1 == null)
                {
                    throw new ArgumentNullException(nameof(obj1));
                }

                if (!((BaseFont)obj1).IsEmbedded())
                {
                    throw new PdfXConformanceException("All the fonts must be embedded. This one isn't: " +
                                                       ((BaseFont)obj1).PostscriptFontName);
                }

                break;
            case PDFXKEY_IMAGE:
                if (obj1 == null)
                {
                    throw new ArgumentNullException(nameof(obj1));
                }

                var image = (PdfImage)obj1;
                if (image.Get(PdfName.Smask) != null)
                {
                    throw new PdfXConformanceException("The /SMask key is not allowed in images.");
                }

                switch (conf)
                {
                    case PdfWriter.PDFX1A2001:
                        var cs = image.Get(PdfName.Colorspace);
                        if (cs == null)
                        {
                            return;
                        }

                        if (cs.IsName())
                        {
                            if (PdfName.Devicergb.Equals(cs))
                            {
                                throw new PdfXConformanceException("Colorspace RGB is not allowed.");
                            }
                        }
                        else if (cs.IsArray())
                        {
                            if (PdfName.Calrgb.Equals(((PdfArray)cs)[0]))
                            {
                                throw new PdfXConformanceException("Colorspace CalRGB is not allowed.");
                            }
                        }

                        break;
                }

                break;
            case PDFXKEY_GSTATE:
                if (obj1 == null)
                {
                    throw new ArgumentNullException(nameof(obj1));
                }

                var gs = (PdfDictionary)obj1;
                var obj = gs.Get(PdfName.Bm);
                if (obj != null && !PdfGState.BmNormal.Equals(obj) && !PdfGState.BmCompatible.Equals(obj))
                {
                    throw new PdfXConformanceException("Blend mode " + obj + " not allowed.");
                }

                obj = gs.Get(PdfName.CA);
                var v = 0.0;
                if (obj != null && (v = ((PdfNumber)obj).DoubleValue).ApproxNotEqual(1.0))
                {
                    throw new PdfXConformanceException("Transparency is not allowed: /CA = " + v);
                }

                obj = gs.Get(PdfName.CA_);
                v = 0.0;
                if (obj != null && (v = ((PdfNumber)obj).DoubleValue).ApproxNotEqual(1.0))
                {
                    throw new PdfXConformanceException("Transparency is not allowed: /ca = " + v);
                }

                break;
            case PDFXKEY_LAYER:
                throw new PdfXConformanceException("Layers are not allowed.");
        }
    }

    public void CompleteExtraCatalog(PdfDictionary extraCatalog)
    {
        if (extraCatalog == null)
        {
            throw new ArgumentNullException(nameof(extraCatalog));
        }

        if (IsPdfX() && !IsPdfA1())
        {
            if (extraCatalog.Get(PdfName.Outputintents) == null)
            {
                var outp = new PdfDictionary(PdfName.Outputintent);
                outp.Put(PdfName.Outputcondition, new PdfString("SWOP CGATS TR 001-1995"));
                outp.Put(PdfName.Outputconditionidentifier, new PdfString("CGATS TR 001"));
                outp.Put(PdfName.Registryname, new PdfString("http://www.color.org"));
                outp.Put(PdfName.Info, new PdfString(""));
                outp.Put(PdfName.S, PdfName.GtsPdfx);
                extraCatalog.Put(PdfName.Outputintents, new PdfArray(outp));
            }
        }
    }

    public void CompleteInfoDictionary(PdfDictionary info)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        if (IsPdfX() && !IsPdfA1())
        {
            if (info.Get(PdfName.GtsPdfxversion) == null)
            {
                if (IsPdfX1A2001())
                {
                    info.Put(PdfName.GtsPdfxversion, new PdfString("PDF/X-1:2001"));
                    info.Put(new PdfName("GTS_PDFXConformance"), new PdfString("PDF/X-1a:2001"));
                }
                else if (IsPdfX32002())
                {
                    info.Put(PdfName.GtsPdfxversion, new PdfString("PDF/X-3:2002"));
                }
            }

            if (info.Get(PdfName.Title) == null)
            {
                info.Put(PdfName.Title, new PdfString("Pdf document"));
            }

            if (info.Get(PdfName.Creator) == null)
            {
                info.Put(PdfName.Creator, new PdfString("Unknown"));
            }

            if (info.Get(PdfName.Trapped) == null)
            {
                info.Put(PdfName.Trapped, new PdfName("False"));
            }
        }
    }

    /// <summary>
    ///     Checks if the PDF has to be in conformance with PDFA1
    /// </summary>
    /// <returns>true of the PDF has to be in conformance with PDFA1</returns>
    public bool IsPdfA1() => PdfxConformance == PdfWriter.PDFA1A || PdfxConformance == PdfWriter.PDFA1B;

    /// <summary>
    ///     Checks if the PDF has to be in conformance with PDFA1A
    /// </summary>
    /// <returns>true of the PDF has to be in conformance with PDFA1A</returns>
    public bool IsPdfA1A() => PdfxConformance == PdfWriter.PDFA1A;

    /// <summary>
    ///     Checks if the PDF has to be in conformance with PDF/X-1a:2001
    /// </summary>
    /// <returns>true of the PDF has to be in conformance with PDF/X-1a:2001</returns>
    public bool IsPdfX1A2001() => PdfxConformance == PdfWriter.PDFX1A2001;

    /// <summary>
    ///     Checks if the PDF has to be in conformance with PDF/X-3:2002
    /// </summary>
    /// <returns>true of the PDF has to be in conformance with PDF/X-3:2002</returns>
    public bool IsPdfX32002() => PdfxConformance == PdfWriter.PDFX32002;
}