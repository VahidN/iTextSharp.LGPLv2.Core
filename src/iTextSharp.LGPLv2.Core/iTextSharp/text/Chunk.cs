using System.Text;
using System.util;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

namespace iTextSharp.text;

/// <summary>
///     This is the smallest significant part of text that can be added to a document.
/// </summary>
/// <remarks>
///     Most elements can be divided in one or more Chunks.
///     A chunk is a string with a certain Font.
///     all other layoutparameters should be defined in the object to which
///     this chunk of text is added.
/// </remarks>
/// <example>
///     Chunk chunk = new Chunk("Hello world", FontFactory.GetFont(FontFactory.COURIER, 20, Font.ITALIC, new Color(255, 0,
///     0)));
///     document.Add(chunk);
/// </example>
public class Chunk : IElement
{
    /// <summary>
    ///     public static membervariables
    /// </summary>
    /// <summary> Key for Action. </summary>
    public const string ACTION = "ACTION";

    ///<summary> Key for background. </summary>
    public const string BACKGROUND = "BACKGROUND";

    ///<summary> Key for color. </summary>
    public const string COLOR = "COLOR";

    /// <summary>
    ///     keys used in PdfChunk
    /// </summary>
    /// <summary> Key for encoding. </summary>
    public const string ENCODING = "ENCODING";

    ///<summary> Key for generic tag. </summary>
    public const string GENERICTAG = "GENERICTAG";

    /// <summary>
    ///     Key for text horizontal scaling.
    /// </summary>
    public const string HSCALE = "HSCALE";

    ///<summary> Key for hyphenation. </summary>
    public const string HYPHENATION = "HYPHENATION";

    ///<summary> Key for image. </summary>
    public const string IMAGE = "IMAGE";

    ///<summary> Key for local destination. </summary>
    public const string LOCALDESTINATION = "LOCALDESTINATION";

    ///<summary> Key for local goto. </summary>
    public const string LOCALGOTO = "LOCALGOTO";

    ///<summary> Key for newpage. </summary>
    public const string NEWPAGE = "NEWPAGE";

    /// <summary>
    ///     The character stand in for an image or a separator.
    /// </summary>
    public const string OBJECT_REPLACEMENT_CHARACTER = "\ufffc";

    ///<summary> Key for annotation. </summary>
    public const string PDFANNOTATION = "PDFANNOTATION";

    ///<summary> Key for remote goto. </summary>
    public const string REMOTEGOTO = "REMOTEGOTO";

    /// <summary>
    ///     Key for drawInterface of the Separator.
    ///     @since   2.1.2
    /// </summary>
    public const string SEPARATOR = "SEPARATOR";

    ///<summary> Key for text skewing. </summary>
    public const string SKEW = "SKEW";

    ///<summary> Key for split character. </summary>
    public const string SPLITCHARACTER = "SPLITCHARACTER";

    ///<summary> Key for sub/basescript. </summary>
    public const string SUBSUPSCRIPT = "SUBSUPSCRIPT";

    /// <summary>
    ///     Key for drawInterface of the tab.
    ///     @since   2.1.2
    /// </summary>
    public const string TAB = "TAB";

    ///<summary> Key for text rendering mode.</summary>
    public const string TEXTRENDERMODE = "TEXTRENDERMODE";

    ///<summary> Key for underline. </summary>
    public const string UNDERLINE = "UNDERLINE";

    ///<summary> This is a Chunk containing a newline. </summary>
    public static readonly Chunk Newline = new("\n");

    /// <summary>
    ///     This is a Chunk containing a newpage.
    /// </summary>
    public static readonly Chunk Nextpage = new("");

    ///<summary> Contains some of the attributes for this Chunk. </summary>
    protected INullValueDictionary<string, object> attributes;

    ///<summary> This is the content of this chunk of text. </summary>
    protected StringBuilder content;

    /// <summary>
    ///     member variables
    /// </summary>
    /// <summary> This is the Font of this chunk of text. </summary>
    protected Font font;

    static Chunk()
    {
        Nextpage.SetNewPage();
    }

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Empty constructor.
    /// </summary>
    /// <overloads>
    ///     Has six overloads.
    /// </overloads>
    public Chunk()
    {
        content = new StringBuilder();
        font = new Font();
    }

    /// <summary>
    ///     A  Chunk  copy constructor.
    /// </summary>
    /// <param name="ck">the  Chunk  to be copied</param>
    public Chunk(Chunk ck)
    {
        if (ck == null)
        {
            throw new ArgumentNullException(nameof(ck));
        }

        if (ck.content != null)
        {
            content = new StringBuilder(ck.content.ToString());
        }

        if (ck.font != null)
        {
            font = new Font(ck.font);
        }
    }

    /// <summary>
    ///     Constructs a chunk of text with a certain content and a certain Font.
    /// </summary>
    /// <param name="content">the content</param>
    /// <param name="font">the font</param>
    public Chunk(string content, Font font)
    {
        this.content = new StringBuilder(content);
        this.font = font;
    }

    /// <summary>
    ///     Constructs a chunk of text with a certain content, without specifying a Font.
    /// </summary>
    /// <param name="content">the content</param>
    public Chunk(string content) : this(content, new Font())
    {
    }

    /// <summary>
    ///     Constructs a chunk of text with a char and a certain  Font .
    /// </summary>
    /// <param name="c">the content</param>
    /// <param name="font">the font</param>
    public Chunk(char c, Font font)
    {
        content = new StringBuilder();
        content.Append(c);
        this.font = font;
    }

    /// <summary>
    ///     Constructs a chunk of text with a char, without specifying a  Font .
    /// </summary>
    /// <param name="c">the content</param>
    public Chunk(char c) : this(c, new Font())
    {
    }

    /// <summary>
    ///     Constructs a chunk containing an Image.
    /// </summary>
    /// <param name="image">the image</param>
    /// <param name="offsetX">the image offset in the x direction</param>
    /// <param name="offsetY">the image offset in the y direction</param>
    public Chunk(Image image, float offsetX, float offsetY) : this(OBJECT_REPLACEMENT_CHARACTER, new Font())
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        var copyImage = Image.GetInstance(image);
        copyImage.SetAbsolutePosition(float.NaN, float.NaN);
        setAttribute(IMAGE, new object[] { copyImage, offsetX, offsetY, false });
    }

    /// <summary>
    ///     Creates a separator Chunk.
    ///     Note that separator chunks can't be used in combination with tab chunks!
    ///     @since   2.1.2
    /// </summary>
    /// <param name="separator">the drawInterface to use to draw the separator.</param>
    public Chunk(IDrawInterface separator) : this(separator, false)
    {
    }

    /// <summary>
    ///     Creates a separator Chunk.
    ///     Note that separator chunks can't be used in combination with tab chunks!
    ///     @since   2.1.2
    /// </summary>
    /// <param name="separator">the drawInterface to use to draw the separator.</param>
    /// <param name="vertical">true if this is a vertical separator</param>
    public Chunk(IDrawInterface separator, bool vertical) : this(OBJECT_REPLACEMENT_CHARACTER, new Font())
    {
        setAttribute(SEPARATOR, new object[] { separator, vertical });
    }

    /// <summary>
    ///     Creates a tab Chunk.
    ///     Note that separator chunks can't be used in combination with tab chunks!
    ///     @since   2.1.2
    /// </summary>
    /// <param name="separator">the drawInterface to use to draw the tab.</param>
    /// <param name="tabPosition">an X coordinate that will be used as start position for the next Chunk.</param>
    public Chunk(IDrawInterface separator, float tabPosition) : this(separator, tabPosition, false)
    {
    }

    /// <summary>
    ///     Creates a tab Chunk.
    ///     Note that separator chunks can't be used in combination with tab chunks!
    ///     @since   2.1.2
    /// </summary>
    /// <param name="separator">the drawInterface to use to draw the tab.</param>
    /// <param name="tabPosition">an X coordinate that will be used as start position for the next Chunk.</param>
    /// <param name="newline">if true, a newline will be added if the tabPosition has already been reached.</param>
    public Chunk(IDrawInterface separator, float tabPosition, bool newline) : this(OBJECT_REPLACEMENT_CHARACTER,
                                                                                   new Font())
    {
        if (tabPosition < 0)
        {
            throw new ArgumentException("A tab position may not be lower than 0; yours is " + tabPosition);
        }

        setAttribute(TAB, new object[] { separator, tabPosition, newline, 0 });
    }

    /// <summary>
    ///     Constructs a chunk containing an Image.
    /// </summary>
    /// <param name="image">the image</param>
    /// <param name="offsetX">the image offset in the x direction</param>
    /// <param name="offsetY">the image offset in the y direction</param>
    /// <param name="changeLeading">true if the leading has to be adapted to the image</param>
    public Chunk(Image image, float offsetX, float offsetY, bool changeLeading) : this(OBJECT_REPLACEMENT_CHARACTER,
                                                                                       new Font())
    {
        setAttribute(IMAGE, new object[] { image, offsetX, offsetY, changeLeading });
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Gets the attributes for this Chunk.
    /// </summary>
    /// <remarks>
    ///     It may be null.
    /// </remarks>
    /// <value>a Hashtable</value>
    public INullValueDictionary<string, object> Attributes
    {
        get => attributes;
        set => attributes = value;
    }

    /// <summary>
    ///     Returns the content of this Chunk.
    /// </summary>
    /// <value>a string</value>
    public virtual string Content => content.ToString();

    /// <summary>
    ///     Get/set the font of this Chunk.
    /// </summary>
    /// <value>a Font</value>
    public virtual Font Font
    {
        get => font;

        set => font = value;
    }

    /// <summary>
    ///     Gets the horizontal scaling.
    /// </summary>
    /// <returns>a percentage in float</returns>
    public float HorizontalScaling
    {
        get
        {
            if (attributes == null)
            {
                return 1f;
            }

            var f = attributes[HSCALE];
            if (f == null)
            {
                return 1f;
            }

            return (float)f;
        }
    }

    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public IList<Chunk> Chunks
    {
        get
        {
            var tmp = new List<Chunk>();
            tmp.Add(this);
            return tmp;
        }
    }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public int Type => Element.CHUNK;

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsNestable() => true;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     IElementListener.
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    public bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            return listener.Add(this);
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    public override string ToString() => content.ToString();

    /// <summary>
    ///     Checks if a given tag corresponds with this object.
    /// </summary>
    /// <param name="tag">the given tag</param>
    /// <returns>true if the tag corresponds</returns>
    public static bool IsTag(string tag) => ElementTags.CHUNK.Equals(tag, StringComparison.Ordinal);

    /// <summary>
    ///     appends some text to this Chunk.
    /// </summary>
    /// <param name="str">a string</param>
    /// <returns>a StringBuilder</returns>
    public StringBuilder Append(string str) => content.Append(str);

    /// <summary>
    ///     Returns the hyphenation (if present).
    ///     @since    2.1.2
    /// </summary>
    public IHyphenationEvent GetHyphenation()
    {
        if (attributes == null)
        {
            return null;
        }

        return (IHyphenationEvent)attributes[HYPHENATION];
    }

    /// <summary>
    ///     Returns the image.
    /// </summary>
    /// <value>an Image</value>
    public Image GetImage()
    {
        if (attributes == null)
        {
            return null;
        }

        var obj = (object[])attributes[IMAGE];
        if (obj == null)
        {
            return null;
        }

        return (Image)obj[0];
    }

    public float GetTextRise()
    {
        if (attributes != null && attributes.TryGetValue(SUBSUPSCRIPT, out var attribute))
        {
            return (float)attribute;
        }

        return 0.0f;
    }

    /// <summary>
    ///     Gets the width of the Chunk in points.
    /// </summary>
    /// <returns>a width in points</returns>
    public float GetWidthPoint()
    {
        if (GetImage() != null)
        {
            return GetImage().ScaledWidth;
        }

        return font.GetCalculatedBaseFont(true).GetWidthPoint(Content, font.CalculatedSize) * HorizontalScaling;
    }

    /// <summary>
    ///     Checks the attributes of this Chunk.
    /// </summary>
    /// <returns>false if there aren't any.</returns>
    public bool HasAttributes() => attributes != null;

    /// <summary>
    ///     Checks is this Chunk is empty.
    /// </summary>
    /// <returns>false if the Chunk contains other characters than space.</returns>
    public virtual bool IsEmpty() => content.ToString().Trim().Length == 0 &&
                                     content.ToString().IndexOf("\n", StringComparison.Ordinal) == -1 &&
                                     attributes == null;

    /// <summary>
    ///     Sets an action for this Chunk.
    /// </summary>
    /// <param name="action">the action</param>
    /// <returns>this Chunk</returns>
    public Chunk SetAction(PdfAction action) => setAttribute(ACTION, action);

    /// <summary>
    ///     Sets an anchor for this Chunk.
    /// </summary>
    /// <param name="url">the Uri to link to</param>
    /// <returns>this Chunk</returns>
    public Chunk SetAnchor(Uri url) => setAttribute(ACTION, new PdfAction(url));

    /// <summary>
    ///     Sets an anchor for this Chunk.
    /// </summary>
    /// <param name="url">the url to link to</param>
    /// <returns>this Chunk</returns>
    public Chunk SetAnchor(string url) => setAttribute(ACTION, new PdfAction(url));

    /// <summary>
    ///     Sets a generic annotation to this Chunk.
    /// </summary>
    /// <param name="annotation">the annotation</param>
    /// <returns>this Chunk</returns>
    public Chunk SetAnnotation(PdfAnnotation annotation) => setAttribute(PDFANNOTATION, annotation);

    /// <summary>
    ///     Sets the color of the background Chunk.
    /// </summary>
    /// <param name="color">the color of the background</param>
    /// <returns>this Chunk</returns>
    public Chunk SetBackground(BaseColor color) => SetBackground(color, 0, 0, 0, 0);

    /// <summary>
    ///     Sets the color and the size of the background  Chunk .
    /// </summary>
    /// <param name="color">the color of the background</param>
    /// <param name="extraLeft">increase the size of the rectangle in the left</param>
    /// <param name="extraBottom">increase the size of the rectangle in the bottom</param>
    /// <param name="extraRight">increase the size of the rectangle in the right</param>
    /// <param name="extraTop">increase the size of the rectangle in the top</param>
    /// <returns>this  Chunk </returns>
    public Chunk SetBackground(BaseColor color, float extraLeft, float extraBottom, float extraRight, float extraTop)
    {
        return setAttribute(BACKGROUND, new object[] { color, new[] { extraLeft, extraBottom, extraRight, extraTop } });
    }

    /// <summary>
    ///     Sets the generic tag Chunk.
    /// </summary>
    /// <remarks>
    ///     The text for this tag can be retrieved with PdfPageEvent.
    /// </remarks>
    /// <param name="text">the text for the tag</param>
    /// <returns>this Chunk</returns>
    public Chunk SetGenericTag(string text) => setAttribute(GENERICTAG, text);

    /// <summary>
    ///     Sets the text horizontal scaling. A value of 1 is normal and a value of 0.5f
    ///     shrinks the text to half it's width.
    /// </summary>
    /// <param name="scale">the horizontal scaling factor</param>
    /// <returns>this  Chunk </returns>
    public Chunk SetHorizontalScaling(float scale) => setAttribute(HSCALE, scale);

    /// <summary>
    ///     sets the hyphenation engine to this Chunk.
    /// </summary>
    /// <param name="hyphenation">the hyphenation engine</param>
    /// <returns>this Chunk</returns>
    public Chunk SetHyphenation(IHyphenationEvent hyphenation) => setAttribute(HYPHENATION, hyphenation);

    /// <summary>
    ///     Sets a local destination for this Chunk.
    /// </summary>
    /// <param name="name">the name for this destination</param>
    /// <returns>this Chunk</returns>
    public Chunk SetLocalDestination(string name) => setAttribute(LOCALDESTINATION, name);

    /// <summary>
    ///     Sets a local goto for this Chunk.
    /// </summary>
    /// <remarks>
    ///     There must be a local destination matching the name.
    /// </remarks>
    /// <param name="name">the name of the destination to go to</param>
    /// <returns>this Chunk</returns>
    public Chunk SetLocalGoto(string name) => setAttribute(LOCALGOTO, name);

    /// <summary>
    ///     Sets a new page tag.
    /// </summary>
    /// <returns>this Chunk</returns>
    public Chunk SetNewPage() => setAttribute(NEWPAGE, null);

    /// <summary>
    ///     Sets a goto for a remote destination for this Chunk.
    /// </summary>
    /// <param name="filename">the file name of the destination document</param>
    /// <param name="name">the name of the destination to go to</param>
    /// <returns>this Chunk</returns>
    public Chunk SetRemoteGoto(string filename, string name)
    {
        return setAttribute(REMOTEGOTO, new object[] { filename, name });
    }

    /// <summary>
    ///     Sets a goto for a remote destination for this Chunk.
    /// </summary>
    /// <param name="filename">the file name of the destination document</param>
    /// <param name="page">the page of the destination to go to. First page is 1</param>
    /// <returns>this Chunk</returns>
    public Chunk SetRemoteGoto(string filename, int page)
    {
        return setAttribute(REMOTEGOTO, new object[] { filename, page });
    }

    /// <summary>
    ///     Skews the text to simulate italic and other effects.
    ///     Try  alpha=0  and  beta=12 .
    /// </summary>
    /// <param name="alpha">the first angle in degrees</param>
    /// <param name="beta">the second angle in degrees</param>
    /// <returns>this  Chunk </returns>
    public Chunk SetSkew(float alpha, float beta)
    {
        alpha = (float)Math.Tan(alpha * Math.PI / 180);
        beta = (float)Math.Tan(beta * Math.PI / 180);
        return setAttribute(SKEW, new[] { alpha, beta });
    }

    /// <summary>
    ///     Sets the split characters.
    /// </summary>
    /// <param name="splitCharacter">the SplitCharacter interface</param>
    /// <returns>this Chunk</returns>
    public Chunk SetSplitCharacter(ISplitCharacter splitCharacter) => setAttribute(SPLITCHARACTER, splitCharacter);

    /// <summary>
    ///     Sets the text rendering mode. It can outline text, simulate bold and make
    ///     text invisible.
    ///     PdfContentByte.TEXT_RENDER_MODE_STROKE ,  PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE
    ///     and  PdfContentByte.TEXT_RENDER_MODE_INVISIBLE .
    ///     PdfContentByte.TEXT_RENDER_MODE_FILL_STROKE .
    /// </summary>
    /// <param name="mode">the text rendering mode. It can be  PdfContentByte.TEXT_RENDER_MODE_FILL ,</param>
    /// <param name="strokeWidth">the stroke line width for the modes  PdfContentByte.TEXT_RENDER_MODE_STROKE  and</param>
    /// <param name="strokeColor">the stroke color or  null  to follow the text color</param>
    /// <returns>this  Chunk </returns>
    public Chunk SetTextRenderMode(int mode, float strokeWidth, BaseColor strokeColor)
    {
        return setAttribute(TEXTRENDERMODE, new object[] { mode, strokeWidth, strokeColor });
    }

    /// <summary>
    ///     Sets the text displacement relative to the baseline. Positive values rise the text,
    ///     negative values lower the text.
    /// </summary>
    /// <remarks>
    ///     It can be used to implement sub/basescript.
    /// </remarks>
    /// <param name="rise">the displacement in points</param>
    /// <returns>this Chunk</returns>
    public Chunk SetTextRise(float rise) => setAttribute(SUBSUPSCRIPT, rise);

    /// <summary>
    ///     Sets an horizontal line that can be an underline or a strikethrough.
    ///     Actually, the line can be anywhere vertically and has always the
    ///     Chunk  width. Multiple call to this method will
    ///     produce multiple lines.
    /// </summary>
    /// <param name="thickness">the absolute thickness of the line</param>
    /// <param name="yPosition">the absolute y position relative to the baseline</param>
    /// <returns>this  Chunk </returns>
    public Chunk SetUnderline(float thickness, float yPosition) =>
        SetUnderline(null, thickness, 0f, yPosition, 0f, PdfContentByte.LINE_CAP_BUTT);

    /// <summary>
    ///     Sets an horizontal line that can be an underline or a strikethrough.
    ///     Actually, the line can be anywhere vertically and has always the
    ///     Chunk  width. Multiple call to this method will
    ///     produce multiple lines.
    ///     the text color
    ///     PdfContentByte.LINE_CAP_BUTT, PdfContentByte.LINE_CAP_ROUND and
    ///     PdfContentByte.LINE_CAP_PROJECTING_SQUARE
    /// </summary>
    /// <param name="color">the color of the line or  null  to follow</param>
    /// <param name="thickness">the absolute thickness of the line</param>
    /// <param name="thicknessMul">the thickness multiplication factor with the font size</param>
    /// <param name="yPosition">the absolute y position relative to the baseline</param>
    /// <param name="yPositionMul">the position multiplication factor with the font size</param>
    /// <param name="cap">the end line cap. Allowed values are</param>
    /// <returns>this  Chunk </returns>
    public Chunk SetUnderline(BaseColor color, float thickness, float thicknessMul, float yPosition, float yPositionMul,
                              int cap)
    {
        if (attributes == null)
        {
            attributes = new NullValueDictionary<string, object>();
        }

        object[] obj = { color, new[] { thickness, thicknessMul, yPosition, yPositionMul, cap } };
        var unders = Utilities.AddToArray((object[][])attributes[UNDERLINE], obj);
        return setAttribute(UNDERLINE, unders);
    }

    /// <summary>
    ///     Sets an arbitrary attribute.
    /// </summary>
    /// <param name="name">the key for the attribute</param>
    /// <param name="obj">the value of the attribute</param>
    /// <returns>this Chunk</returns>
    private Chunk setAttribute(string name, object obj)
    {
        if (attributes == null)
        {
            attributes = new NullValueDictionary<string, object>();
        }

        attributes[name] = obj;
        return this;
    }
}