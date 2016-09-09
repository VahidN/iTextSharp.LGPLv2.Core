using System;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Generates barcodes in several formats: EAN13, EAN8, UPCA, UPCE,
    /// supplemental 2 and 5. The default parameters are:
    ///
    /// x = 0.8f;
    /// font = BaseFont.CreateFont("Helvetica", "winansi", false);
    /// size = 8;
    /// baseline = size;
    /// barHeight = size * 3;
    /// guardBars = true;
    /// codeType = EAN13;
    /// code = "";
    ///
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class BarcodeEan : Barcode
    {

        /// <summary>
        /// Marker for even parity.
        /// </summary>
        private const byte Even = 1;

        /// <summary>
        /// Marker for odd parity.
        /// </summary>
        private const byte Odd = 0;

        /// <summary>
        /// The total number of bars for EAN13.
        /// </summary>
        private const int TotalbarsEan13 = 11 + 12 * 4;

        /// <summary>
        /// The total number of bars for EAN8.
        /// </summary>
        private const int TotalbarsEan8 = 11 + 8 * 4;

        /// <summary>
        /// The total number of bars for supplemental 2.
        /// </summary>
        private const int TotalbarsSupp2 = 13;

        /// <summary>
        /// The total number of bars for supplemental 5.
        /// </summary>
        private const int TotalbarsSupp5 = 31;

        /// <summary>
        /// The total number of bars for UPCE.
        /// </summary>
        private const int TotalbarsUpce = 9 + 6 * 4;

        /// <summary>
        /// The basic bar widths.
        /// </summary>
        private static readonly byte[][] _bars =
        {
            new byte[] {3, 2, 1, 1}, // 0
            new byte[] {2, 2, 2, 1}, // 1
            new byte[] {2, 1, 2, 2}, // 2
            new byte[] {1, 4, 1, 1}, // 3
            new byte[] {1, 1, 3, 2}, // 4
            new byte[] {1, 2, 3, 1}, // 5
            new byte[] {1, 1, 1, 4}, // 6
            new byte[] {1, 3, 1, 2}, // 7
            new byte[] {1, 2, 1, 3}, // 8
            new byte[] {3, 1, 1, 2}  // 9
        };

        /// <summary>
        /// The bar positions that are guard bars.
        /// </summary>
        private static readonly int[] _guardEan13 = { 0, 2, 28, 30, 56, 58 };

        /// <summary>
        /// The bar positions that are guard bars.
        /// </summary>
        private static readonly int[] _guardEan8 = { 0, 2, 20, 22, 40, 42 };

        /// <summary>
        /// The bar positions that are guard bars.
        /// </summary>
        private static readonly int[] _guardEmpty = { };
        /// <summary>
        /// The bar positions that are guard bars.
        /// </summary>
        private static readonly int[] _guardUpca = { 0, 2, 4, 6, 28, 30, 52, 54, 56, 58 };
        /// <summary>
        /// The bar positions that are guard bars.
        /// </summary>
        private static readonly int[] _guardUpce = { 0, 2, 28, 30, 32 };
        /// <summary>
        /// Sequence of parities to be used with EAN13.
        /// </summary>
        private static readonly byte[][] _parity13 =
        {
            new[] {Odd, Odd,  Odd,  Odd,  Odd,  Odd},  // 0
            new[] {Odd, Odd,  Even, Odd,  Even, Even}, // 1
            new[] {Odd, Odd,  Even, Even, Odd,  Even}, // 2
            new[] {Odd, Odd,  Even, Even, Even, Odd},  // 3
            new[] {Odd, Even, Odd,  Odd,  Even, Even}, // 4
            new[] {Odd, Even, Even, Odd,  Odd,  Even}, // 5
            new[] {Odd, Even, Even, Even, Odd,  Odd},  // 6
            new[] {Odd, Even, Odd,  Even, Odd,  Even}, // 7
            new[] {Odd, Even, Odd,  Even, Even, Odd},  // 8
            new[] {Odd, Even, Even, Odd,  Even, Odd}   // 9
        };

        /// <summary>
        /// Sequence of parities to be used with supplemental 2.
        /// </summary>
        private static readonly byte[][] _parity2 =
        {
            new[] {Odd,  Odd},   // 0
            new[] {Odd,  Even},  // 1
            new[] {Even, Odd},   // 2
            new[] {Even, Even}   // 3
        };

        /// <summary>
        /// Sequence of parities to be used with supplemental 2.
        /// </summary>
        private static readonly byte[][] _parity5 =
        {
            new[] {Even, Even, Odd,  Odd,  Odd},  // 0
            new[] {Even, Odd,  Even, Odd,  Odd},  // 1
            new[] {Even, Odd,  Odd,  Even, Odd},  // 2
            new[] {Even, Odd,  Odd,  Odd,  Even}, // 3
            new[] {Odd,  Even, Even, Odd,  Odd},  // 4
            new[] {Odd,  Odd,  Even, Even, Odd},  // 5
            new[] {Odd,  Odd,  Odd,  Even, Even}, // 6
            new[] {Odd,  Even, Odd,  Even, Odd},  // 7
            new[] {Odd,  Even, Odd,  Odd,  Even}, // 8
            new[] {Odd,  Odd,  Even, Odd,  Even}  // 9
        };

        /// <summary>
        /// Sequence of parities to be used with UPCE.
        /// </summary>
        private static readonly byte[][] _paritye =
        {
            new[] {Even, Even, Even, Odd,  Odd,  Odd},  // 0
            new[] {Even, Even, Odd,  Even, Odd,  Odd},  // 1
            new[] {Even, Even, Odd,  Odd,  Even, Odd},  // 2
            new[] {Even, Even, Odd,  Odd,  Odd,  Even}, // 3
            new[] {Even, Odd,  Even, Even, Odd,  Odd},  // 4
            new[] {Even, Odd,  Odd,  Even, Even, Odd},  // 5
            new[] {Even, Odd,  Odd,  Odd,  Even, Even}, // 6
            new[] {Even, Odd,  Even, Odd,  Even, Odd},  // 7
            new[] {Even, Odd,  Even, Odd,  Odd,  Even}, // 8
            new[] {Even, Odd,  Odd,  Even, Odd,  Even}  // 9
        };

        /// <summary>
        /// The x coordinates to place the text.
        /// </summary>
        private static readonly float[] _textposEan13 = { 6.5f, 13.5f, 20.5f, 27.5f, 34.5f, 41.5f, 53.5f, 60.5f, 67.5f, 74.5f, 81.5f, 88.5f };
        /// <summary>
        /// The x coordinates to place the text.
        /// </summary>
        private static readonly float[] _textposEan8 = { 6.5f, 13.5f, 20.5f, 27.5f, 39.5f, 46.5f, 53.5f, 60.5f };
        /// <summary>
        /// Creates new BarcodeEAN
        /// </summary>
        public BarcodeEan()
        {
            x = 0.8f;
            font = BaseFont.CreateFont("Helvetica", "winansi", false);
            size = 8;
            baseline = size;
            barHeight = size * 3;
            guardBars = true;
            codeType = EAN13;
            code = "";
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
                float width = 0;
                float height = barHeight;
                if (font != null)
                {
                    if (baseline <= 0)
                        height += -baseline + size;
                    else
                        height += baseline - font.GetFontDescriptor(BaseFont.DESCENT, size);
                }
                switch (codeType)
                {
                    case EAN13:
                        width = x * (11 + 12 * 7);
                        if (font != null)
                        {
                            width += font.GetWidthPoint(code[0], size);
                        }
                        break;
                    case EAN8:
                        width = x * (11 + 8 * 7);
                        break;
                    case UPCA:
                        width = x * (11 + 12 * 7);
                        if (font != null)
                        {
                            width += font.GetWidthPoint(code[0], size) + font.GetWidthPoint(code[11], size);
                        }
                        break;
                    case UPCE:
                        width = x * (9 + 6 * 7);
                        if (font != null)
                        {
                            width += font.GetWidthPoint(code[0], size) + font.GetWidthPoint(code[7], size);
                        }
                        break;
                    case SUPP2:
                        width = x * (6 + 2 * 7);
                        break;
                    case SUPP5:
                        width = x * (4 + 5 * 7 + 4 * 2);
                        break;
                    default:
                        throw new ArgumentException("Invalid code type.");
                }
                return new Rectangle(width, height);
            }
        }

        /// <summary>
        /// Calculates the EAN parity character.
        /// </summary>
        /// <param name="code">the code</param>
        /// <returns>the parity character</returns>
        public static int CalculateEanParity(string code)
        {
            int mul = 3;
            int total = 0;
            for (int k = code.Length - 1; k >= 0; --k)
            {
                int n = code[k] - '0';
                total += mul * n;
                mul ^= 2;
            }
            return (10 - (total % 10)) % 10;
        }

        /// <summary>
        /// Converts an UPCA code into an UPCE code. If the code can not
        /// be converted a  null  is returned.
        /// code could not be converted
        /// </summary>
        /// <param name="text">the code to convert. It must have 12 numeric characters</param>
        /// <returns>the 8 converted digits or  null  if the</returns>
        public static string ConvertUpcAtoUpce(string text)
        {
            if (text.Length != 12 || !(text.StartsWith("0") || text.StartsWith("1")))
                return null;
            if (text.Substring(3, 3).Equals("000") || text.Substring(3, 3).Equals("100")
                || text.Substring(3, 3).Equals("200"))
            {
                if (text.Substring(6, 2).Equals("00"))
                    return text.Substring(0, 1) + text.Substring(1, 2) + text.Substring(8, 3) + text.Substring(3, 1) + text.Substring(11);
            }
            else if (text.Substring(4, 2).Equals("00"))
            {
                if (text.Substring(6, 3).Equals("000"))
                    return text.Substring(0, 1) + text.Substring(1, 3) + text.Substring(9, 2) + "3" + text.Substring(11);
            }
            else if (text.Substring(5, 1).Equals("0"))
            {
                if (text.Substring(6, 4).Equals("0000"))
                    return text.Substring(0, 1) + text.Substring(1, 4) + text.Substring(10, 1) + "4" + text.Substring(11);
            }
            else if (text[10] >= '5')
            {
                if (text.Substring(6, 4).Equals("0000"))
                    return text.Substring(0, 1) + text.Substring(1, 5) + text.Substring(10, 1) + text.Substring(11);
            }
            return null;
        }

        /// <summary>
        /// Creates the bars for the barcode EAN13 and UPCA.
        /// </summary>
        /// <param name="_code">the text with 13 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsEan13(string _code)
        {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TotalbarsEan13];
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            byte[] sequence = _parity13[code[0]];
            for (int k = 0; k < sequence.Length; ++k)
            {
                int c = code[k + 1];
                byte[] stripes = _bars[c];
                if (sequence[k] == Odd)
                {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else
                {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 7; k < 13; ++k)
            {
                int c = code[k];
                byte[] stripes = _bars[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }

        /// <summary>
        /// Creates the bars for the barcode EAN8.
        /// </summary>
        /// <param name="_code">the text with 8 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsEan8(string _code)
        {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TotalbarsEan8];
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 0; k < 4; ++k)
            {
                int c = code[k];
                byte[] stripes = _bars[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            for (int k = 4; k < 8; ++k)
            {
                int c = code[k];
                byte[] stripes = _bars[c];
                bars[pb++] = stripes[0];
                bars[pb++] = stripes[1];
                bars[pb++] = stripes[2];
                bars[pb++] = stripes[3];
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }

        /// <summary>
        /// Creates the bars for the barcode supplemental 2.
        /// </summary>
        /// <param name="_code">the text with 2 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsSupplemental2(string _code)
        {
            int[] code = new int[2];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TotalbarsSupp2];
            int pb = 0;
            int parity = (code[0] * 10 + code[1]) % 4;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 2;
            byte[] sequence = _parity2[parity];
            for (int k = 0; k < sequence.Length; ++k)
            {
                if (k == 1)
                {
                    bars[pb++] = 1;
                    bars[pb++] = 1;
                }
                int c = code[k];
                byte[] stripes = _bars[c];
                if (sequence[k] == Odd)
                {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else
                {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            return bars;
        }

        /// <summary>
        /// Creates the bars for the barcode supplemental 5.
        /// </summary>
        /// <param name="_code">the text with 5 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsSupplemental5(string _code)
        {
            int[] code = new int[5];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TotalbarsSupp5];
            int pb = 0;
            int parity = (((code[0] + code[2] + code[4]) * 3) + ((code[1] + code[3]) * 9)) % 10;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 2;
            byte[] sequence = _parity5[parity];
            for (int k = 0; k < sequence.Length; ++k)
            {
                if (k != 0)
                {
                    bars[pb++] = 1;
                    bars[pb++] = 1;
                }
                int c = code[k];
                byte[] stripes = _bars[c];
                if (sequence[k] == Odd)
                {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else
                {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            return bars;
        }

        /// <summary>
        /// Creates the bars for the barcode UPCE.
        /// </summary>
        /// <param name="_code">the text with 8 digits</param>
        /// <returns>the barcode</returns>
        public static byte[] GetBarsUpce(string _code)
        {
            int[] code = new int[_code.Length];
            for (int k = 0; k < code.Length; ++k)
                code[k] = _code[k] - '0';
            byte[] bars = new byte[TotalbarsUpce];
            bool flip = (code[0] != 0);
            int pb = 0;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            byte[] sequence = _paritye[code[code.Length - 1]];
            for (int k = 1; k < code.Length - 1; ++k)
            {
                int c = code[k];
                byte[] stripes = _bars[c];
                if (sequence[k - 1] == (flip ? Even : Odd))
                {
                    bars[pb++] = stripes[0];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[3];
                }
                else
                {
                    bars[pb++] = stripes[3];
                    bars[pb++] = stripes[2];
                    bars[pb++] = stripes[1];
                    bars[pb++] = stripes[0];
                }
            }
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            bars[pb++] = 1;
            return bars;
        }
        public override System.Drawing.Image CreateDrawingImage(System.Drawing.Color foreground, System.Drawing.Color background)
        {
            int width = 0;
            byte[] bars = null;
            switch (codeType)
            {
                case EAN13:
                    bars = GetBarsEan13(code);
                    width = 11 + 12 * 7;
                    break;
                case EAN8:
                    bars = GetBarsEan8(code);
                    width = 11 + 8 * 7;
                    break;
                case UPCA:
                    bars = GetBarsEan13("0" + code);
                    width = 11 + 12 * 7;
                    break;
                case UPCE:
                    bars = GetBarsUpce(code);
                    width = 9 + 6 * 7;
                    break;
                case SUPP2:
                    bars = GetBarsSupplemental2(code);
                    width = 6 + 2 * 7;
                    break;
                case SUPP5:
                    bars = GetBarsSupplemental5(code);
                    width = 4 + 5 * 7 + 4 * 2;
                    break;
                default:
                    throw new InvalidOperationException("Invalid code type.");
            }
            int height = (int)barHeight;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height);
            for (int h = 0; h < height; ++h)
            {
                bool print = true;
                int ptr = 0;
                for (int k = 0; k < bars.Length; ++k)
                {
                    int w = bars[k];
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
            Rectangle rect = BarcodeSize;
            float barStartX = 0;
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
            switch (codeType)
            {
                case EAN13:
                case UPCA:
                case UPCE:
                    if (font != null)
                        barStartX += font.GetWidthPoint(code[0], size);
                    break;
            }
            byte[] bars = null;
            int[] guard = _guardEmpty;
            switch (codeType)
            {
                case EAN13:
                    bars = GetBarsEan13(code);
                    guard = _guardEan13;
                    break;
                case EAN8:
                    bars = GetBarsEan8(code);
                    guard = _guardEan8;
                    break;
                case UPCA:
                    bars = GetBarsEan13($"0{code}");
                    guard = _guardUpca;
                    break;
                case UPCE:
                    bars = GetBarsUpce(code);
                    guard = _guardUpce;
                    break;
                case SUPP2:
                    bars = GetBarsSupplemental2(code);
                    break;
                case SUPP5:
                    bars = GetBarsSupplemental5(code);
                    break;
            }
            float keepBarX = barStartX;
            bool print = true;
            float gd = 0;
            if (font != null && baseline > 0 && guardBars)
            {
                gd = baseline / 2;
            }
            if (barColor != null)
                cb.SetColorFill(barColor);
            for (int k = 0; k < bars.Length; ++k)
            {
                float w = bars[k] * x;
                if (print)
                {
                    if (Array.BinarySearch(guard, k) >= 0)
                        cb.Rectangle(barStartX, barStartY - gd, w - inkSpreading, barHeight + gd);
                    else
                        cb.Rectangle(barStartX, barStartY, w - inkSpreading, barHeight);
                }
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
                switch (codeType)
                {
                    case EAN13:
                        cb.SetTextMatrix(0, textStartY);
                        cb.ShowText(code.Substring(0, 1));
                        for (int k = 1; k < 13; ++k)
                        {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = keepBarX + _textposEan13[k - 1] * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        break;
                    case EAN8:
                        for (int k = 0; k < 8; ++k)
                        {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = _textposEan8[k] * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        break;
                    case UPCA:
                        cb.SetTextMatrix(0, textStartY);
                        cb.ShowText(code.Substring(0, 1));
                        for (int k = 1; k < 11; ++k)
                        {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = keepBarX + _textposEan13[k] * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        cb.SetTextMatrix(keepBarX + x * (11 + 12 * 7), textStartY);
                        cb.ShowText(code.Substring(11, 1));
                        break;
                    case UPCE:
                        cb.SetTextMatrix(0, textStartY);
                        cb.ShowText(code.Substring(0, 1));
                        for (int k = 1; k < 7; ++k)
                        {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = keepBarX + _textposEan13[k - 1] * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        cb.SetTextMatrix(keepBarX + x * (9 + 6 * 7), textStartY);
                        cb.ShowText(code.Substring(7, 1));
                        break;
                    case SUPP2:
                    case SUPP5:
                        for (int k = 0; k < code.Length; ++k)
                        {
                            string c = code.Substring(k, 1);
                            float len = font.GetWidthPoint(c, size);
                            float pX = (7.5f + (9 * k)) * x - len / 2;
                            cb.SetTextMatrix(pX, textStartY);
                            cb.ShowText(c);
                        }
                        break;
                }
                cb.EndText();
            }
            return rect;
        }
    }
}
