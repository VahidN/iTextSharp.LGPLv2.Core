using System;
using System.Text;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Implements the code interleaved 2 of 5. The text can include
    /// non numeric characters that are printed but do not generate bars.
    /// The default parameters are:
    ///
    /// x = 0.8f;
    /// n = 2;
    /// font = BaseFont.CreateFont("Helvetica", "winansi", false);
    /// size = 8;
    /// baseline = size;
    /// barHeight = size * 3;
    /// textint= Element.ALIGN_CENTER;
    /// generateChecksum = false;
    /// checksumText = false;
    ///
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class BarcodeInter25 : Barcode
    {

        /// <summary>
        /// The bars to generate the code.
        /// </summary>
        private static readonly byte[][] _bars = {
         new byte[] {0,0,1,1,0},
         new byte[] {1,0,0,0,1},
         new byte[] {0,1,0,0,1},
         new byte[] {1,1,0,0,0},
         new byte[] {0,0,1,0,1},
         new byte[] {1,0,1,0,0},
         new byte[] {0,1,1,0,0},
         new byte[] {0,0,0,1,1},
         new byte[] {1,0,0,1,0},
         new byte[] {0,1,0,1,0}
    };

        /// <summary>
        /// Creates new BarcodeInter25
        /// </summary>
        public BarcodeInter25()
        {
            x = 0.8f;
            n = 2;
            font = BaseFont.CreateFont("Helvetica", "winansi", false);
            size = 8;
            baseline = size;
            barHeight = size * 3;
            textAlignment = Element.ALIGN_CENTER;
            generateChecksum = false;
            checksumText = false;
        }

        /// <summary>
        /// Gets the maximum area that the barcode and the text, if
        /// any, will occupy. The lower left corner is always (0, 0).
        /// </summary>
        /// <returns>the size the barcode occupies.</returns>
        public override Rectangle BarcodeSize
        {
            get
            {
                float fontX = 0;
                float fontY = 0;
                if (font != null)
                {
                    if (baseline > 0)
                        fontY = baseline - font.GetFontDescriptor(BaseFont.DESCENT, size);
                    else
                        fontY = -baseline + size;
                    string fullCode = code;
                    if (generateChecksum && checksumText)
                        fullCode += GetChecksum(fullCode);
                    fontX = font.GetWidthPoint(altText != null ? altText : fullCode, size);
                }
                string fCode = KeepNumbers(code);
                int len = fCode.Length;
                if (generateChecksum)
                    ++len;
                float fullWidth = len * (3 * x + 2 * x * n) + (6 + n) * x;
                fullWidth = Math.Max(fullWidth, fontX);
                float fullHeight = barHeight + fontY;
                return new Rectangle(fullWidth, fullHeight);
            }
        }

        /// <summary>
        /// Creates the bars for the barcode.
        /// </summary>
        /// <param name="text">the text. It can contain non numeric characters</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsInter25(string text)
        {
            text = KeepNumbers(text);
            if ((text.Length & 1) != 0)
                throw new ArgumentException("The text length must be even.");
            byte[] bars = new byte[text.Length * 5 + 7];
            int pb = 0;
            bars[pb++] = 0;
            bars[pb++] = 0;
            bars[pb++] = 0;
            bars[pb++] = 0;
            int len = text.Length / 2;
            for (int k = 0; k < len; ++k)
            {
                int c1 = text[k * 2] - '0';
                int c2 = text[k * 2 + 1] - '0';
                byte[] b1 = _bars[c1];
                byte[] b2 = _bars[c2];
                for (int j = 0; j < 5; ++j)
                {
                    bars[pb++] = b1[j];
                    bars[pb++] = b2[j];
                }
            }
            bars[pb++] = 1;
            bars[pb++] = 0;
            bars[pb++] = 0;
            return bars;
        }

        /// <summary>
        /// Calculates the checksum.
        /// </summary>
        /// <param name="text">the numeric text</param>
        /// <returns>the checksum</returns>
        public static char GetChecksum(string text)
        {
            int mul = 3;
            int total = 0;
            for (int k = text.Length - 1; k >= 0; --k)
            {
                int n = text[k] - '0';
                total += mul * n;
                mul ^= 2;
            }
            return (char)(((10 - (total % 10)) % 10) + '0');
        }

        /// <summary>
        /// Deletes all the non numeric characters from  text .
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns>a  string  with only numeric characters</returns>
        public static string KeepNumbers(string text)
        {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < text.Length; ++k)
            {
                char c = text[k];
                if (c >= '0' && c <= '9')
                    sb.Append(c);
            }
            return sb.ToString();
        }
        public override System.Drawing.Image CreateDrawingImage(System.Drawing.Color foreground, System.Drawing.Color background)
        {
            string bCode = KeepNumbers(code);
            if (generateChecksum)
                bCode += GetChecksum(bCode);
            int len = bCode.Length;
            int nn = (int)n;
            int fullWidth = len * (3 + 2 * nn) + (6 + nn);
            byte[] bars = GetBarsInter25(bCode);
            int height = (int)barHeight;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fullWidth, height);
            for (int h = 0; h < height; ++h)
            {
                bool print = true;
                int ptr = 0;
                for (int k = 0; k < bars.Length; ++k)
                {
                    int w = (bars[k] == 0 ? 1 : nn);
                    System.Drawing.Color c = background;
                    if (print)
                        c = foreground;
                    print = !print;
                    for (int j = 0; j < w; ++j)
                        bmp.SetPixel(ptr++, h, c);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Places the barcode in a  PdfContentByte . The
        /// barcode is always placed at coodinates (0, 0). Use the
        /// translation matrix to move it elsewhere.
        /// The bars and text are written in the following colors:
        ///
        ///
        ///    barColor
        ///    textColor
        ///   Result
        ///
        ///
        ///    null
        ///    null
        ///   bars and text painted with current fill color
        ///
        ///
        ///    barColor
        ///    null
        ///   bars and text painted with  barColor
        ///
        ///
        ///    null
        ///    textColor
        ///   bars painted with current color text painted with  textColor
        ///
        ///
        ///    barColor
        ///    textColor
        ///   bars painted with  barColor  text painted with  textColor
        ///
        ///
        /// </summary>
        /// <param name="cb">the  PdfContentByte  where the barcode will be placed</param>
        /// <param name="barColor">the color of the bars. It can be  null </param>
        /// <param name="textColor">the color of the text. It can be  null </param>
        /// <returns>the dimensions the barcode occupies</returns>
        public override Rectangle PlaceBarcode(PdfContentByte cb, BaseColor barColor, BaseColor textColor)
        {
            string fullCode = code;
            float fontX = 0;
            if (font != null)
            {
                if (generateChecksum && checksumText)
                    fullCode += GetChecksum(fullCode);
                fontX = font.GetWidthPoint(fullCode = altText != null ? altText : fullCode, size);
            }
            string bCode = KeepNumbers(code);
            if (generateChecksum)
                bCode += GetChecksum(bCode);
            int len = bCode.Length;
            float fullWidth = len * (3 * x + 2 * x * n) + (6 + n) * x;
            float barStartX = 0;
            float textStartX = 0;
            switch (textAlignment)
            {
                case Element.ALIGN_LEFT:
                    break;
                case Element.ALIGN_RIGHT:
                    if (fontX > fullWidth)
                        barStartX = fontX - fullWidth;
                    else
                        textStartX = fullWidth - fontX;
                    break;
                default:
                    if (fontX > fullWidth)
                        barStartX = (fontX - fullWidth) / 2;
                    else
                        textStartX = (fullWidth - fontX) / 2;
                    break;
            }
            float barStartY = 0;
            float textStartY = 0;
            if (font != null)
            {
                if (baseline <= 0)
                    textStartY = barHeight - baseline;
                else
                {
                    textStartY = -font.GetFontDescriptor(BaseFont.DESCENT, size);
                    barStartY = textStartY + baseline;
                }
            }
            byte[] bars = GetBarsInter25(bCode);
            bool print = true;
            if (barColor != null)
                cb.SetColorFill(barColor);
            for (int k = 0; k < bars.Length; ++k)
            {
                float w = (bars[k] == 0 ? x : x * n);
                if (print)
                    cb.Rectangle(barStartX, barStartY, w - inkSpreading, barHeight);
                print = !print;
                barStartX += w;
            }
            cb.Fill();
            if (font != null)
            {
                if (textColor != null)
                    cb.SetColorFill(textColor);
                cb.BeginText();
                cb.SetFontAndSize(font, size);
                cb.SetTextMatrix(textStartX, textStartY);
                cb.ShowText(fullCode);
                cb.EndText();
            }
            return BarcodeSize;
        }
    }
}
