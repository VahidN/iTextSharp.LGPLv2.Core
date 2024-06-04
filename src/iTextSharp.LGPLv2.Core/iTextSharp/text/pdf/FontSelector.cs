using System.Text;

namespace iTextSharp.text.pdf;

/// <summary>
///     Selects the appropriate fonts that contain the glyphs needed to
///     render text correctly. The fonts are checked in order until the
///     character is found.
///     The built in fonts "Symbol" and "ZapfDingbats", if used, have a special encoding
///     to allow the characters to be referred by Unicode.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class FontSelector
{
    protected List<Font> Fonts = new();

    /// <summary>
    ///     Adds a  Font  to be searched for valid characters.
    /// </summary>
    /// <param name="font">the  Font </param>
    public void AddFont(Font font)
    {
        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        if (font.BaseFont != null)
        {
            Fonts.Add(font);
            return;
        }

        var bf = font.GetCalculatedBaseFont(true);
        var f2 = new Font(bf, font.Size, font.CalculatedStyle, font.Color);
        Fonts.Add(f2);
    }

    /// <summary>
    ///     Process the text so that it will render with a combination of fonts
    ///     if needed.
    /// </summary>
    /// <param name="text">the text</param>
    /// <returns>a  Phrase  with one or more chunks</returns>
    public Phrase Process(string text)
    {
        if (text == null)
        {
            throw new ArgumentNullException(nameof(text));
        }

        var fsize = Fonts.Count;
        if (fsize == 0)
        {
            throw new ArgumentException("No font is defined.");
        }

        var cc = text.ToCharArray();
        var len = cc.Length;
        var sb = new StringBuilder();
        Font font = null;
        var lastidx = -1;
        var ret = new Phrase();
        for (var k = 0; k < len; ++k)
        {
            var c = cc[k];
            if (c == '\n' || c == '\r')
            {
                sb.Append(c);
                continue;
            }

            if (Utilities.IsSurrogatePair(cc, k))
            {
                var u = Utilities.ConvertToUtf32(cc, k);
                for (var f = 0; f < fsize; ++f)
                {
                    font = Fonts[f];
                    if (font.BaseFont.CharExists(u) ||
                        CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(u), 0) == UnicodeCategory.Format)
                    {
                        if (lastidx != f)
                        {
                            if (sb.Length > 0 && lastidx != -1)
                            {
                                var ck = new Chunk(sb.ToString(), Fonts[lastidx]);
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
                for (var f = 0; f < fsize; ++f)
                {
                    font = Fonts[f];
                    if (font.BaseFont.CharExists(c) ||
                        CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.Format)
                    {
                        if (lastidx != f)
                        {
                            if (sb.Length > 0 && lastidx != -1)
                            {
                                var ck = new Chunk(sb.ToString(), Fonts[lastidx]);
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
            var ck = new Chunk(sb.ToString(), Fonts[lastidx == -1 ? 0 : lastidx]);
            ret.Add(ck);
        }

        return ret;
    }
}