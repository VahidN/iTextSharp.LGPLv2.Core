using System;
using System.Collections;
using System.Text;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Selects the appropriate fonts that contain the glyphs needed to
    /// render text correctly. The fonts are checked in order until the
    /// character is found.
    ///
    /// The built in fonts "Symbol" and "ZapfDingbats", if used, have a special encoding
    /// to allow the characters to be referred by Unicode.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class FontSelector
    {

        protected ArrayList Fonts = new ArrayList();

        /// <summary>
        /// Adds a  Font  to be searched for valid characters.
        /// </summary>
        /// <param name="font">the  Font </param>
        public void AddFont(Font font)
        {
            if (font.BaseFont != null)
            {
                Fonts.Add(font);
                return;
            }
            BaseFont bf = font.GetCalculatedBaseFont(true);
            Font f2 = new Font(bf, font.Size, font.CalculatedStyle, font.Color);
            Fonts.Add(f2);
        }

        /// <summary>
        /// Process the text so that it will render with a combination of fonts
        /// if needed.
        /// </summary>
        /// <param name="text">the text</param>
        /// <returns>a  Phrase  with one or more chunks</returns>
        public Phrase Process(string text)
        {
            int fsize = Fonts.Count;
            if (fsize == 0)
                throw new ArgumentException("No font is defined.");
            char[] cc = text.ToCharArray();
            int len = cc.Length;
            StringBuilder sb = new StringBuilder();
            Font font = null;
            int lastidx = -1;
            Phrase ret = new Phrase();
            for (int k = 0; k < len; ++k)
            {
                char c = cc[k];
                if (c == '\n' || c == '\r')
                {
                    sb.Append(c);
                    continue;
                }
                if (Utilities.IsSurrogatePair(cc, k))
                {
                    int u = Utilities.ConvertToUtf32(cc, k);
                    for (int f = 0; f < fsize; ++f)
                    {
                        font = (Font)Fonts[f];
                        if (font.BaseFont.CharExists(u))
                        {
                            if (lastidx != f)
                            {
                                if (sb.Length > 0 && lastidx != -1)
                                {
                                    Chunk ck = new Chunk(sb.ToString(), (Font)Fonts[lastidx]);
                                    ret.Add(ck);
                                    sb.Length = 0;
                                }
                                lastidx = f;
                            }
                            sb.Append(c);
                            sb.Append(cc[++k]);
                            break;
                        }
                    }
                }
                else
                {
                    for (int f = 0; f < fsize; ++f)
                    {
                        font = (Font)Fonts[f];
                        if (font.BaseFont.CharExists(c))
                        {
                            if (lastidx != f)
                            {
                                if (sb.Length > 0 && lastidx != -1)
                                {
                                    Chunk ck = new Chunk(sb.ToString(), (Font)Fonts[lastidx]);
                                    ret.Add(ck);
                                    sb.Length = 0;
                                }
                                lastidx = f;
                            }
                            sb.Append(c);
                            break;
                        }
                    }
                }
            }
            if (sb.Length > 0)
            {
                Chunk ck = new Chunk(sb.ToString(), (Font)Fonts[lastidx == -1 ? 0 : lastidx]);
                ret.Add(ck);
            }
            return ret;
        }
    }
}