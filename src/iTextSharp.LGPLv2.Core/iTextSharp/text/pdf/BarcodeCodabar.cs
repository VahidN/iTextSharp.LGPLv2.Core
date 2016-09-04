using System;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Implements the code codabar. The default parameters are:
    ///
    /// x = 0.8f;
    /// n = 2;
    /// font = BaseFont.CreateFont("Helvetica", "winansi", false);
    /// size = 8;
    /// baseline = size;
    /// barHeight = size * 3;
    /// textAlignment = Element.ALIGN_CENTER;
    /// generateChecksum = false;
    /// checksumText = false;
    /// startStopText = false;
    ///
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class BarcodeCodabar : Barcode
    {

        /// <summary>
        /// The index chars to  BARS .
        /// </summary>
        private const string Chars = "0123456789-$:/.+ABCD";

        private const int StartStopIdx = 16;

        /// <summary>
        /// The bars to generate the code.
        /// </summary>
        private static readonly byte[][] _bars = {
            new byte[]{0,0,0,0,0,1,1}, // 0
            new byte[]{0,0,0,0,1,1,0}, // 1
            new byte[]{0,0,0,1,0,0,1}, // 2
            new byte[]{1,1,0,0,0,0,0}, // 3
            new byte[]{0,0,1,0,0,1,0}, // 4
            new byte[]{1,0,0,0,0,1,0}, // 5
            new byte[]{0,1,0,0,0,0,1}, // 6
            new byte[]{0,1,0,0,1,0,0}, // 7
            new byte[]{0,1,1,0,0,0,0}, // 8
            new byte[]{1,0,0,1,0,0,0}, // 9
            new byte[]{0,0,0,1,1,0,0}, // -
            new byte[]{0,0,1,1,0,0,0}, // $
            new byte[]{1,0,0,0,1,0,1}, // :
            new byte[]{1,0,1,0,0,0,1}, // /
            new byte[]{1,0,1,0,1,0,0}, // .
            new byte[]{0,0,1,0,1,0,1}, // +
            new byte[]{0,0,1,1,0,1,0}, // a
            new byte[]{0,1,0,1,0,0,1}, // b
            new byte[]{0,0,0,1,0,1,1}, // c
            new byte[]{0,0,0,1,1,1,0}  // d
        };
        /// <summary>
        /// Creates a new BarcodeCodabar.
        /// </summary>
        public BarcodeCodabar()
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
            startStopText = false;
            codeType = CODABAR;
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
                string text = code;
                if (generateChecksum && checksumText)
                    text = CalculateChecksum(code);
                if (!startStopText)
                    text = text.Substring(1, text.Length - 2);
                if (font != null)
                {
                    if (baseline > 0)
                        fontY = baseline - font.GetFontDescriptor(BaseFont.DESCENT, size);
                    else
                        fontY = -baseline + size;
                    fontX = font.GetWidthPoint(altText != null ? altText : text, size);
                }
                text = code;
                if (generateChecksum)
                    text = CalculateChecksum(code);
                byte[] bars = GetBarsCodabar(text);
                int wide = 0;
                for (int k = 0; k < bars.Length; ++k)
                {
                    wide += bars[k];
                }
                int narrow = bars.Length - wide;
                float fullWidth = x * (narrow + wide * n);
                fullWidth = Math.Max(fullWidth, fontX);
                float fullHeight = barHeight + fontY;
                return new Rectangle(fullWidth, fullHeight);
            }
        }

        public static string CalculateChecksum(string code)
        {
            if (code.Length < 2)
                return code;
            string text = code.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            int sum = 0;
            int len = text.Length;
            for (int k = 0; k < len; ++k)
                sum += Chars.IndexOf(text[k]);
            sum = (sum + 15) / 16 * 16 - sum;
            return code.Substring(0, len - 1) + Chars[sum] + code.Substring(len - 1);
        }

        /// <summary>
        /// Creates the bars.
        /// </summary>
        /// <param name="text">the text to create the bars</param>
        /// <returns>the bars</returns>
        public static byte[] GetBarsCodabar(string text)
        {
            text = text.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            int len = text.Length;
            if (len < 2)
                throw new ArgumentException("Codabar must have at least a start and stop character.");
            if (Chars.IndexOf(text[0]) < StartStopIdx || Chars.IndexOf(text[len - 1]) < StartStopIdx)
                throw new ArgumentException("Codabar must have one of 'ABCD' as start/stop character.");
            byte[] bars = new byte[text.Length * 8 - 1];
            for (int k = 0; k < len; ++k)
            {
                int idx = Chars.IndexOf(text[k]);
                if (idx >= StartStopIdx && k > 0 && k < len - 1)
                    throw new ArgumentException("In codabar, start/stop characters are only allowed at the extremes.");
                if (idx < 0)
                    throw new ArgumentException("The character '" + text[k] + "' is illegal in codabar.");
                Array.Copy(_bars[idx], 0, bars, k * 8, 7);
            }
            return bars;
        }

        public override System.Drawing.Image CreateDrawingImage(System.Drawing.Color foreground, System.Drawing.Color background)
        {
            string fullCode = code;
            if (generateChecksum && checksumText)
                fullCode = CalculateChecksum(code);
            if (!startStopText)
                fullCode = fullCode.Substring(1, fullCode.Length - 2);
            byte[] bars = GetBarsCodabar(generateChecksum ? CalculateChecksum(code) : code);
            int wide = 0;
            for (int k = 0; k < bars.Length; ++k)
            {
                wide += bars[k];
            }
            int narrow = bars.Length - wide;
            int fullWidth = narrow + wide * (int)n;
            int height = (int)barHeight;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(fullWidth, height);
            for (int h = 0; h < height; ++h)
            {
                bool print = true;
                int ptr = 0;
                for (int k = 0; k < bars.Length; ++k)
                {
                    int w = (bars[k] == 0 ? 1 : (int)n);
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
            if (generateChecksum && checksumText)
                fullCode = CalculateChecksum(code);
            if (!startStopText)
                fullCode = fullCode.Substring(1, fullCode.Length - 2);
            float fontX = 0;
            if (font != null)
            {
                fontX = font.GetWidthPoint(fullCode = altText != null ? altText : fullCode, size);
            }
            byte[] bars = GetBarsCodabar(generateChecksum ? CalculateChecksum(code) : code);
            int wide = 0;
            for (int k = 0; k < bars.Length; ++k)
            {
                wide += bars[k];
            }
            int narrow = bars.Length - wide;
            float fullWidth = x * (narrow + wide * n);
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
