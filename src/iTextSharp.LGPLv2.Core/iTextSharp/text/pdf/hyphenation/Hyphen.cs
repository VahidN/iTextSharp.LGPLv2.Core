using System.Text;

namespace iTextSharp.text.pdf.hyphenation;

/// <summary>
///     This class represents a hyphen. A 'full' hyphen is made of 3 parts:
///     the pre-break text, post-break text and no-break. If no line-break
///     is generated at this position, the no-break text is used, otherwise,
///     pre-break and post-break are used. Typically, pre-break is equal to
///     the hyphen character and the others are empty. However, this general
///     scheme allows support for cases in some languages where words change
///     spelling if they're split across lines, like german's 'backen' which
///     hyphenates 'bak-ken'. BTW, this comes from TeX.
///     @author Carlos Villegas
/// </summary>
public class Hyphen
{
    public string NoBreak;
    public string PostBreak;
    public string PreBreak;

    internal Hyphen(string pre, string no, string post)
    {
        PreBreak = pre;
        NoBreak = no;
        PostBreak = post;
    }

    internal Hyphen(string pre)
    {
        PreBreak = pre;
        NoBreak = null;
        PostBreak = null;
    }

    public override string ToString()
    {
        if (NoBreak == null
            && PostBreak == null
            && PreBreak != null
            && PreBreak.Equals("-", StringComparison.Ordinal))
        {
            return "-";
        }

        var res = new StringBuilder("{");
        res.Append(PreBreak);
        res.Append("}{");
        res.Append(PostBreak);
        res.Append("}{");
        res.Append(NoBreak);
        res.Append('}');
        return res.ToString();
    }
}