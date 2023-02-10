using System.util;
using iTextSharp.text.xml;

namespace iTextSharp.text.html;

/// <summary>
///     The  Tags -class maps several XHTML-tags to iText-objects.
/// </summary>
public class HtmlTagMap : TagMap
{
    /// <summary>
    ///     Constructs an HtmlTagMap.
    /// </summary>
    public HtmlTagMap()
    {
        HtmlPeer peer;

        peer = new HtmlPeer(ElementTags.ITEXT, HtmlTags.HTML);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.SPAN);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.CHUNK, HtmlTags.CHUNK);
        peer.AddAlias(ElementTags.FONT, HtmlTags.FONT);
        peer.AddAlias(ElementTags.SIZE, HtmlTags.SIZE);
        peer.AddAlias(ElementTags.COLOR, HtmlTags.COLOR);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.ANCHOR, HtmlTags.ANCHOR);
        peer.AddAlias(ElementTags.NAME, HtmlTags.NAME);
        peer.AddAlias(ElementTags.REFERENCE, HtmlTags.REFERENCE);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.PARAGRAPH);
        peer.AddAlias(ElementTags.ALIGN, HtmlTags.ALIGN);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.DIV);
        peer.AddAlias(ElementTags.ALIGN, HtmlTags.ALIGN);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[0]);
        peer.AddValue(ElementTags.SIZE, "20");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[1]);
        peer.AddValue(ElementTags.SIZE, "18");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[2]);
        peer.AddValue(ElementTags.SIZE, "16");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[3]);
        peer.AddValue(ElementTags.SIZE, "14");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[4]);
        peer.AddValue(ElementTags.SIZE, "12");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PARAGRAPH, HtmlTags.H[5]);
        peer.AddValue(ElementTags.SIZE, "10");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.LIST, HtmlTags.ORDEREDLIST);
        peer.AddValue(ElementTags.NUMBERED, "true");
        peer.AddValue(ElementTags.SYMBOLINDENT, "20");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.LIST, HtmlTags.UNORDEREDLIST);
        peer.AddValue(ElementTags.NUMBERED, "false");
        peer.AddValue(ElementTags.SYMBOLINDENT, "20");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.LISTITEM, HtmlTags.LISTITEM);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.I);
        peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_ITALIC);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.EM);
        peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_ITALIC);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.B);
        peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_BOLD);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.STRONG);
        peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_BOLD);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.S);
        peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_LINETHROUGH);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.CODE);
        peer.AddValue(ElementTags.FONT, FontFactory.COURIER);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.VAR);
        peer.AddValue(ElementTags.FONT, FontFactory.COURIER);
        peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_ITALIC);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.PHRASE, HtmlTags.U);
        peer.AddValue(ElementTags.STYLE, Markup.CSS_VALUE_UNDERLINE);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.CHUNK, HtmlTags.SUP);
        peer.AddValue(ElementTags.Subsupscript, "6.0");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.CHUNK, HtmlTags.SUB);
        peer.AddValue(ElementTags.Subsupscript, "-6.0");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.HORIZONTALRULE, HtmlTags.HORIZONTALRULE);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.TABLE, HtmlTags.TABLE);
        peer.AddAlias(ElementTags.WIDTH, HtmlTags.WIDTH);
        peer.AddAlias(ElementTags.BACKGROUNDCOLOR, HtmlTags.BACKGROUNDCOLOR);
        peer.AddAlias(ElementTags.BORDERCOLOR, HtmlTags.BORDERCOLOR);
        peer.AddAlias(ElementTags.COLUMNS, HtmlTags.COLUMNS);
        peer.AddAlias(ElementTags.CELLPADDING, HtmlTags.CELLPADDING);
        peer.AddAlias(ElementTags.CELLSPACING, HtmlTags.CELLSPACING);
        peer.AddAlias(ElementTags.BORDERWIDTH, HtmlTags.BORDERWIDTH);
        peer.AddAlias(ElementTags.ALIGN, HtmlTags.ALIGN);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.ROW, HtmlTags.ROW);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.CELL, HtmlTags.CELL);
        peer.AddAlias(ElementTags.WIDTH, HtmlTags.WIDTH);
        peer.AddAlias(ElementTags.BACKGROUNDCOLOR, HtmlTags.BACKGROUNDCOLOR);
        peer.AddAlias(ElementTags.BORDERCOLOR, HtmlTags.BORDERCOLOR);
        peer.AddAlias(ElementTags.COLSPAN, HtmlTags.COLSPAN);
        peer.AddAlias(ElementTags.ROWSPAN, HtmlTags.ROWSPAN);
        peer.AddAlias(ElementTags.NOWRAP, HtmlTags.NOWRAP);
        peer.AddAlias(ElementTags.HORIZONTALALIGN, HtmlTags.HORIZONTALALIGN);
        peer.AddAlias(ElementTags.VERTICALALIGN, HtmlTags.VERTICALALIGN);
        peer.AddValue(ElementTags.HEADER, "false");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.CELL, HtmlTags.HEADERCELL);
        peer.AddAlias(ElementTags.WIDTH, HtmlTags.WIDTH);
        peer.AddAlias(ElementTags.BACKGROUNDCOLOR, HtmlTags.BACKGROUNDCOLOR);
        peer.AddAlias(ElementTags.BORDERCOLOR, HtmlTags.BORDERCOLOR);
        peer.AddAlias(ElementTags.COLSPAN, HtmlTags.COLSPAN);
        peer.AddAlias(ElementTags.ROWSPAN, HtmlTags.ROWSPAN);
        peer.AddAlias(ElementTags.NOWRAP, HtmlTags.NOWRAP);
        peer.AddAlias(ElementTags.HORIZONTALALIGN, HtmlTags.HORIZONTALALIGN);
        peer.AddAlias(ElementTags.VERTICALALIGN, HtmlTags.VERTICALALIGN);
        peer.AddValue(ElementTags.HEADER, "true");
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.IMAGE, HtmlTags.IMAGE);
        peer.AddAlias(ElementTags.URL, ElementTags.SRC); // contributed by Lubos Strapko
        peer.AddAlias(ElementTags.ALT, HtmlTags.ALT);
        peer.AddAlias(ElementTags.PLAINWIDTH, HtmlTags.PLAINWIDTH);
        peer.AddAlias(ElementTags.PLAINHEIGHT, HtmlTags.PLAINHEIGHT);
        this[peer.Alias] = peer;

        peer = new HtmlPeer(ElementTags.NEWLINE, HtmlTags.NEWLINE);
        this[peer.Alias] = peer;
    }

    /// <summary>
    ///     Checks if this is the root tag.
    /// </summary>
    /// <param name="tag">a tagvalue</param>
    /// <returns>true if tag is BODY or body</returns>
    public static bool IsBody(string tag) => Util.EqualsIgnoreCase(HtmlTags.BODY, tag);

    /// <summary>
    ///     Checks if this is the head tag.
    /// </summary>
    /// <param name="tag">a tagvalue</param>
    /// <returns>true if tag is HEAD or head</returns>
    public static bool IsHead(string tag) => Util.EqualsIgnoreCase(HtmlTags.HEAD, tag);

    /// <summary>
    ///     Checks if this is the root tag.
    /// </summary>
    /// <param name="tag">a tagvalue</param>
    /// <returns>true if tag is HTML or html</returns>
    public static bool IsHtml(string tag) => Util.EqualsIgnoreCase(HtmlTags.HTML, tag);

    /// <summary>
    ///     Checks if this is the linl tag.
    /// </summary>
    /// <param name="tag">a tagvalue</param>
    /// <returns>true if tag is LINK or link</returns>
    public static bool IsLink(string tag) => Util.EqualsIgnoreCase(HtmlTags.LINK, tag);

    /// <summary>
    ///     Checks if this is the meta tag.
    /// </summary>
    /// <param name="tag">a tagvalue</param>
    /// <returns>true if tag is META or meta</returns>
    public static bool IsMeta(string tag) => Util.EqualsIgnoreCase(HtmlTags.META, tag);

    /// <summary>
    ///     Checks if this is a special tag.
    /// </summary>
    /// <param name="tag">a tagvalue</param>
    /// <returns>true if tag is a HTML, HEAD, META, LINK or BODY tag (case insensitive)</returns>
    public static bool IsSpecialTag(string tag) =>
        IsHtml(tag) || IsHead(tag) || IsMeta(tag) || IsLink(tag) || IsBody(tag);

    /// <summary>
    ///     Checks if this is the title tag.
    /// </summary>
    /// <param name="tag">a tagvalue</param>
    /// <returns>true if tag is TITLE or title</returns>
    public static bool IsTitle(string tag) => Util.EqualsIgnoreCase(HtmlTags.TITLE, tag);
}