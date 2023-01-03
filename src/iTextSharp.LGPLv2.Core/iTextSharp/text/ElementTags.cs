using System.util;

namespace iTextSharp.text;

/// <summary>
///     A class that contains all the possible tagnames and their attributes.
/// </summary>
public static class ElementTags
{
    /// <summary> attribute of the image tag </summary>
    public const string ABSOLUTEX = "absolutex";

    /// <summary> attribute of the image tag </summary>
    public const string ABSOLUTEY = "absolutey";

    /// <summary> attribute of paragraph/image/table tag </summary>
    public const string ALIGN = "align";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_BASELINE = "Baseline";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_BOTTOM = "Bottom";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_CENTER = "Center";

    /// <summary>
    ///     a possible list attribute
    /// </summary>
    public const string ALIGN_INDENTATION_ITEMS = "alignindent";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_JUSTIFIED = "Justify";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_JUSTIFIED_ALL = "JustifyAll";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_LEFT = "Left";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_MIDDLE = "Middle";

    /// <summary>
    ///     alignment attribute values
    /// </summary>
    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_RIGHT = "Right";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string ALIGN_TOP = "Top";

    /// <summary> attribute of the image tag </summary>
    public const string ALT = "alt";

    /// <summary> the anchor tag </summary>
    public const string ANCHOR = "anchor";

    /// <summary> the annotation tag </summary>
    public const string ANNOTATION = "annotation";

    /// <summary> attribute of the annotation tag </summary>
    public const string APPLICATION = "application";

    /// <summary> attribute of the root tag </summary>
    public const string AUTHOR = "author";

    /// <summary>
    ///     a possible list attribute
    /// </summary>
    public const string AUTO_INDENT_ITEMS = "autoindent";

    /// <summary> attribute of the table/cell tag </summary>
    public const string BACKGROUNDCOLOR = "backgroundcolor";

    /// <summary> attribute of the table/cell tag </summary>
    public const string BGBLUE = "bgblue";

    /// <summary> attribute of the table/cell tag </summary>
    public const string BGGREEN = "bggreen";

    /// <summary> attribute of the table/cell tag </summary>
    public const string BGRED = "bgred";

    /// <summary> attribute of the chunk/table/cell tag </summary>
    public const string BLUE = "blue";

    /// <summary> the image tag </summary>
    public const string BOOKMARKOPEN = "bookmarkopen";

    /// <summary> attribute of the table/cell tag </summary>
    public const string BORDERCOLOR = "bordercolor";

    /// <summary> attribute of the table/cell tag </summary>
    public const string BORDERWIDTH = "borderwidth";

    /// <summary> attribute of the table/cell tag </summary>
    public const string BOTTOM = "bottom";

    /// <summary> the cell tag </summary>
    public const string CELL = "cell";

    /// <summary> attribute of the table tag </summary>
    public const string CELLPADDING = "cellpadding";

    /// <summary> attribute of the table tag </summary>
    public const string CELLSFITPAGE = "cellsfitpage";

    /// <summary> attribute of the table tag </summary>
    public const string CELLSPACING = "cellspacing";

    /// <summary> the chapter tag </summary>
    public const string CHAPTER = "chapter";

    /// <summary> the chunk tag </summary>
    public const string CHUNK = "chunk";

    /// <summary> attribute of the chunk/table/cell tag </summary>
    public const string COLOR = "color";

    /// <summary> attribute of the cell tag </summary>
    public const string COLSPAN = "colspan";

    /// <summary> attribute of the table tag </summary>
    public const string COLUMNS = "columns";

    /// <summary> attribute of the annotation tag </summary>
    public const string CONTENT = "content";

    /// <summary> attribute of the table tag </summary>
    public const string CONVERT2PDFP = "convert2pdfp";

    /// <summary> attribute of the root tag </summary>
    public const string CREATIONDATE = "creationdate";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string DEFAULT = "Default";

    /// <summary> attribute of the annotation tag </summary>
    public const string DEFAULTDIR = "defaultdir";

    /// <summary> attribute of section/chapter tag </summary>
    public const string DEPTH = "depth";

    /// <summary> attribute of the annotation tag </summary>
    public const string DESTINATION = "destination";

    /// <summary> attribute of the chunk tag </summary>
    public const string EMBEDDED = "embedded";

    /// <summary> attribute of the chunk tag </summary>
    public const string ENCODING = "encoding";

    /// <summary> the chunk tag </summary>
    public const string ENTITY = "entity";

    /// <summary>
    ///     a possible list attribute
    ///     @since 2.1.3
    /// </summary>
    public const string FACE = "face";

    /// <summary> attribute of the annotation tag </summary>
    public const string FILE = "file";

    /// <summary> attribute of list tag </summary>
    public const string FIRST = "first";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string FONT = "font";

    /// <summary> attribute of the cell tag </summary>
    public const string FOOTER = "footer";

    /// <summary> attribute of the table/cell tag </summary>
    public const string GRAYFILL = "grayfill";

    /// <summary> attribute of the chunk/table/cell tag </summary>
    public const string GREEN = "green";

    /// <summary> attribute of the cell tag </summary>
    public const string HEADER = "header";

    /// <summary> attribute of the cell tag </summary>
    public const string HORIZONTALALIGN = "horizontalalign";

    /// <summary> the possible value of a tag </summary>
    public const string HORIZONTALRULE = "horizontalrule";

    /// <summary> the chunk tag </summary>
    public const string ID = "id";

    /// <summary> the chunk tag </summary>
    public const string IGNORE = "ignore";

    /// <summary> the image tag </summary>
    public const string IMAGE = "image";

    /// <summary> attribute of section/chapter tag </summary>
    public const string INDENT = "indent";

    /// <summary> attribute of list tag </summary>
    public const string INDENTATIONLEFT = "indentationleft";

    /// <summary> attribute of list tag </summary>
    public const string INDENTATIONRIGHT = "indentationright";

    /// <summary> the root tag. </summary>
    public const string ITEXT = "itext";

    /// <summary> attribute of paragraph </summary>
    public const string KEEPTOGETHER = "keeptogether";

    /// <summary> attribute of the root tag </summary>
    public const string KEYWORDS = "keywords";

    /// <summary> attribute of the table tag </summary>
    public const string LASTHEADERROW = "lastHeaderRow";

    /// <summary> attribute of phrase/paragraph/cell tag </summary>
    public const string LEADING = "leading";

    /// <summary> attribute of chapter/section/paragraph/table/cell tag </summary>
    public const string LEFT = "left";

    /// <summary> attribute of the list tag </summary>
    public const string LETTERED = "lettered";

    /// <summary> the list tag </summary>
    public const string LIST = "list";

    /// <summary> the listitem tag </summary>
    public const string LISTITEM = "listitem";

    /// <summary> attribute of list tag </summary>
    public const string LISTSYMBOL = "listsymbol";

    /// <summary> attribute of the annotation tag </summary>
    public const string LLX = "llx";

    /// <summary> attribute of the annotation tag </summary>
    public const string LLY = "lly";

    /// <summary>
    ///     a possible list attribute
    /// </summary>
    public const string LOWERCASE = "lowercase";

    /// <summary> attribute of anchor tag </summary>
    public const string NAME = "name";

    /// <summary> attribute of the annotation tag </summary>
    public const string NAMED = "named";

    /// <summary> the newpage tag </summary>
    public const string NEWLINE = "newline";

    /// <summary> the newpage tag </summary>
    public const string NEWPAGE = "newpage";

    /// <summary> attribute of the cell tag </summary>
    public const string NOWRAP = "nowrap";

    /// <summary> attribute of section/chapter tag </summary>
    public const string NUMBER = "number";

    /// <summary> attribute of section/chapter tag </summary>
    public const string NUMBERDEPTH = "numberdepth";

    /// <summary> attribute of list tag </summary>
    public const string NUMBERED = "numbered";

    /// <summary> attribute of the table tag </summary>
    public const string OFFSET = "offset";

    /// <summary> attribute of the annotation tag </summary>
    public const string OPERATION = "operation";

    /// <summary>
    ///     the possible value of a tag
    /// </summary>
    public const string ORIENTATION = "orientation";

    /// <summary> attribute of the annotation tag </summary>
    public const string PAGE = "page";

    /// <summary>
    ///     the possible value of a tag
    /// </summary>
    public const string PAGE_SIZE = "pagesize";

    /// <summary> the paragraph tag </summary>
    public const string PARAGRAPH = "paragraph";

    /// <summary> attribute of the annotation tag </summary>
    public const string PARAMETERS = "parameters";

    /// <summary> the phrase tag </summary>
    public const string PHRASE = "phrase";

    /// <summary> attribute of the image tag </summary>
    public const string PLAINHEIGHT = "plainheight";

    /// <summary> attribute of the image tag </summary>
    public const string PLAINWIDTH = "plainwidth";

    /// <summary> attribute of the root tag </summary>
    public const string PRODUCER = "producer";

    /// <summary>
    ///     Chunks
    /// </summary>
    /// <summary> attribute of the chunk/table/cell tag </summary>
    public const string RED = "red";

    /// <summary>
    ///     Phrases, Anchors, Lists and Paragraphs
    /// </summary>
    /// <summary> attribute of anchor tag </summary>
    public const string REFERENCE = "reference";

    /// <summary> attribute of chapter/section/paragraph/table/cell tag </summary>
    public const string RIGHT = "right";

    /// <summary> attribute of the image tag </summary>
    public const string ROTATION = "rotation";

    /// <summary> the cell tag </summary>
    public const string ROW = "row";

    /// <summary> attribute of the cell tag </summary>
    public const string ROWSPAN = "rowspan";

    /// <summary> attribute of the image tag </summary>
    public const string SCALEDHEIGHT = "scaledheight";

    /// <summary> attribute of the image tag </summary>
    public const string SCALEDWIDTH = "scaledwidth";

    /// <summary>
    ///     Chapters and Sections
    /// </summary>
    /// <summary> the section tag </summary>
    public const string SECTION = "section";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string SIZE = "size";

    /// <summary>
    ///     attribute of the image or iframe tag
    ///     @since 2.1.3
    /// </summary>
    public const string SRC = "src";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string STYLE = "fontstyle";

    /// <summary> attribute of the root tag </summary>
    public const string SUBJECT = "subject";

    /// <summary> attribute of list tag </summary>
    public const string SYMBOLINDENT = "symbolindent";

    /// <summary> the table tag </summary>
    public const string TABLE = "table";

    /// <summary> attribute of the table tag </summary>
    public const string TABLEFITSPAGE = "tablefitspage";

    /// <summary> attribute of the image tag </summary>
    public const string TEXTWRAP = "textwrap";

    /// <summary> attribute of the root and annotation tag (also a special tag within a chapter or section) </summary>
    public const string TITLE = "title";

    /// <summary> attribute of the table/cell tag </summary>
    public const string TOP = "top";

    /// <summary> attribute of the image tag </summary>
    public const string UNDERLYING = "underlying";

    /// <summary> the possible value of an alignment attribute </summary>
    public const string UNKNOWN = "unknown";

    /// <summary>
    ///     Misc
    /// </summary>
    /// <summary> attribute of the image and annotation tag </summary>
    public const string URL = "url";

    /// <summary> attribute of the annotation tag </summary>
    public const string URX = "urx";

    /// <summary> attribute of the annotation tag </summary>
    public const string URY = "ury";

    /// <summary> attribute of the cell tag </summary>
    public const string VERTICALALIGN = "verticalalign";

    /// <summary> attribute of the table/cell tag </summary>
    public const string WIDTH = "width";

    /// <summary>
    ///     tables/cells
    /// </summary>
    /// <summary> attribute of the table tag </summary>
    public const string WIDTHS = "widths";

    /// <summary> attribute of the chunk tag </summary>
    public static readonly string Generictag = Chunk.GENERICTAG.ToLower(CultureInfo.InvariantCulture);

    /// <summary> attribute of the chunk tag </summary>
    public static readonly string Localdestination = Chunk.LOCALDESTINATION.ToLower(CultureInfo.InvariantCulture);

    /// <summary> attribute of the chunk tag </summary>
    public static readonly string Localgoto = Chunk.LOCALGOTO.ToLower(CultureInfo.InvariantCulture);

    /// <summary> attribute of the chunk tag </summary>
    public static readonly string Remotegoto = Chunk.REMOTEGOTO.ToLower(CultureInfo.InvariantCulture);

    /// <summary> attribute of the chunk tag </summary>
    public static readonly string Subsupscript = Chunk.SUBSUPSCRIPT.ToLower(CultureInfo.InvariantCulture);

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Translates a String value to an alignment value.
    ///     (written by Norman Richards, integrated into iText by Bruno)
    /// </summary>
    /// <param name="alignment">value (one of the ALIGN_ constants of the Element interface)</param>
    public static int AlignmentValue(string alignment)
    {
        if (alignment == null)
        {
            return Element.ALIGN_UNDEFINED;
        }

        if (Util.EqualsIgnoreCase(ALIGN_CENTER, alignment))
        {
            return Element.ALIGN_CENTER;
        }

        if (Util.EqualsIgnoreCase(ALIGN_LEFT, alignment))
        {
            return Element.ALIGN_LEFT;
        }

        if (Util.EqualsIgnoreCase(ALIGN_RIGHT, alignment))
        {
            return Element.ALIGN_RIGHT;
        }

        if (Util.EqualsIgnoreCase(ALIGN_JUSTIFIED, alignment))
        {
            return Element.ALIGN_JUSTIFIED;
        }

        if (Util.EqualsIgnoreCase(ALIGN_JUSTIFIED_ALL, alignment))
        {
            return Element.ALIGN_JUSTIFIED_ALL;
        }

        if (Util.EqualsIgnoreCase(ALIGN_TOP, alignment))
        {
            return Element.ALIGN_TOP;
        }

        if (Util.EqualsIgnoreCase(ALIGN_MIDDLE, alignment))
        {
            return Element.ALIGN_MIDDLE;
        }

        if (Util.EqualsIgnoreCase(ALIGN_BOTTOM, alignment))
        {
            return Element.ALIGN_BOTTOM;
        }

        if (Util.EqualsIgnoreCase(ALIGN_BASELINE, alignment))
        {
            return Element.ALIGN_BASELINE;
        }

        return Element.ALIGN_UNDEFINED;
    }

    /// <summary>
    ///     Translates the alignment value to a String value.
    /// </summary>
    /// <param name="alignment">the alignment value</param>
    /// <returns>the translated value</returns>
    public static string GetAlignment(int alignment)
    {
        switch (alignment)
        {
            case Element.ALIGN_LEFT:
                return ALIGN_LEFT;
            case Element.ALIGN_CENTER:
                return ALIGN_CENTER;
            case Element.ALIGN_RIGHT:
                return ALIGN_RIGHT;
            case Element.ALIGN_JUSTIFIED:
            case Element.ALIGN_JUSTIFIED_ALL:
                return ALIGN_JUSTIFIED;
            case Element.ALIGN_TOP:
                return ALIGN_TOP;
            case Element.ALIGN_MIDDLE:
                return ALIGN_MIDDLE;
            case Element.ALIGN_BOTTOM:
                return ALIGN_BOTTOM;
            case Element.ALIGN_BASELINE:
                return ALIGN_BASELINE;
            default:
                return DEFAULT;
        }
    }
}