using System.Text;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     A Phrase is a series of Chunks.
/// </summary>
/// <remarks>
///     A Phrase has a main Font, but some chunks
///     within the phrase can have a Font that differs from the
///     main Font. All the Chunks in a Phrase
///     have the same leading.
/// </remarks>
/// <example>
///     // When no parameters are passed, the default leading = 16
///     Phrase phrase0 = new Phrase();
///     Phrase phrase1 = new Phrase("this is a phrase");
///     // In this example the leading is passed as a parameter
///     Phrase phrase2 = new Phrase(16, "this is a phrase with leading 16");
///     // When a Font is passed (explicitely or embedded in a chunk), the default leading = 1.5 * size of the font
///     Phrase phrase3 = new Phrase("this is a phrase with a red, normal font Courier, size 12",
///     FontFactory.GetFont(FontFactory.COURIER, 12, Font.NORMAL, new Color(255, 0, 0)));
///     Phrase phrase4 = new Phrase(new Chunk("this is a phrase"));
///     Phrase phrase5 = new Phrase(18, new Chunk("this is a phrase", FontFactory.GetFont(FontFactory.HELVETICA, 16,
///     Font.BOLD, new Color(255, 0, 0)));
/// </example>
public class Phrase : List<IElement>, ITextElementArray
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> This is the font of this phrase. </summary>
    protected Font font;

    /// <summary>
    ///     Null, unless the Phrase has to be hyphenated.
    ///     @since   2.1.2
    /// </summary>
    protected IHyphenationEvent hyphenation;

    /// <summary>This is the leading of this phrase.</summary>
    protected float leading = float.NaN;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a Phrase without specifying a leading.
    /// </summary>
    /// <overloads>
    ///     Has nine overloads.
    /// </overloads>
    public Phrase() : this(16)
    {
    }

    /// <summary>
    ///     Copy constructor for  Phrase .
    /// </summary>
    public Phrase(Phrase phrase)
    {
        if (phrase == null)
        {
            throw new ArgumentNullException(nameof(phrase));
        }

        AddAll(phrase);
        leading = phrase.Leading;
        font = phrase.Font;
        hyphenation = phrase.hyphenation;
    }

    /// <summary>
    ///     Constructs a Phrase with a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    public Phrase(float leading)
    {
        this.leading = leading;
        font = new Font();
    }

    /// <summary>
    ///     Constructs a Phrase with a certain Chunk.
    /// </summary>
    /// <param name="chunk">a Chunk</param>
    public Phrase(Chunk chunk)
    {
        if (chunk == null)
        {
            throw new ArgumentNullException(nameof(chunk));
        }

        base.Add(chunk);
        font = chunk.Font;
        hyphenation = chunk.GetHyphenation();
    }

    /// <summary>
    ///     Constructs a Phrase with a certain Chunk and a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="chunk">a Chunk</param>
    public Phrase(float leading, Chunk chunk)
    {
        if (chunk == null)
        {
            throw new ArgumentNullException(nameof(chunk));
        }

        this.leading = leading;
        base.Add(chunk);
        font = chunk.Font;
        hyphenation = chunk.GetHyphenation();
    }

    /// <summary>
    ///     Constructs a Phrase with a certain string.
    /// </summary>
    /// <param name="str">a string</param>
    public Phrase(string str) : this(float.NaN, str, new Font())
    {
    }

    /// <summary>
    ///     Constructs a Phrase with a certain string and a certain Font.
    /// </summary>
    /// <param name="str">a string</param>
    /// <param name="font">a Font</param>
    public Phrase(string str, Font font) : this(float.NaN, str, font)
    {
    }

    /// <summary>
    ///     Constructs a Phrase with a certain leading and a certain string.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="str">a string</param>
    public Phrase(float leading, string str) : this(leading, str, new Font())
    {
    }

    public Phrase(float leading, string str, Font font)
    {
        this.leading = leading;
        this.font = font;
        /* bugfix by August Detlefsen */
        if (!string.IsNullOrEmpty(str))
        {
            base.Add(new Chunk(str, font));
        }
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Constructs a Phrase that can be used in the static GetInstance() method.
    /// </summary>
    /// <param name="dummy">parameter</param>
    private Phrase(bool dummy)
    {
    }

    /// <summary>
    ///     Returns the content as a String object.
    ///     This method differs from toString because toString will return an ArrayList with the toString value of the Chunks
    ///     in this Phrase.
    /// </summary>
    public string Content
    {
        get
        {
            var buf = new StringBuilder();
            foreach (object obj in Chunks)
            {
                buf.Append(obj);
            }

            return buf.ToString();
        }
    }

    /// <summary>
    ///     Gets the font of the first Chunk that appears in this Phrase.
    /// </summary>
    /// <value>a Font</value>
    public Font Font
    {
        get => font;
        set => font = value;
    }

    /// <summary>
    ///     Setter/getter for the hyphenation.
    ///     @since   2.1.2
    /// </summary>
    public IHyphenationEvent Hyphenation
    {
        set => hyphenation = value;
        get => hyphenation;
    }

    /// <summary>
    ///     Gets/sets the leading of this phrase.
    /// </summary>
    /// <value>the linespacing</value>
    public virtual float Leading
    {
        get
        {
            if (float.IsNaN(leading) && font != null)
            {
                return font.GetCalculatedLeading(1.5f);
            }

            return leading;
        }

        set => leading = value;
    }

    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public virtual IList<Chunk> Chunks
    {
        get
        {
            var tmp = new List<Chunk>();
            foreach (var ele in this)
            {
                tmp.AddRange(ele.Chunks);
            }

            return tmp;
        }
    }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public virtual int Type => Element.PHRASE;

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
    ///     <see cref="iTextSharp.text.IElementListener" />.
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    public virtual bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            foreach (var ele in this)
            {
                listener.Add(ele);
            }

            return true;
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     overriding some of the ArrayList-methods
    /// </summary>
    /// <summary>
    ///     Adds a Chunk, Anchor or another Phrase
    ///     to this Phrase.
    /// </summary>
    /// <param name="o">an object of type Chunk, Anchor or Phrase</param>
    /// <returns>a bool</returns>
    public new virtual bool Add(IElement o)
    {
        if (o == null)
        {
            return false;
        }

        if (o is IRtfElementInterface)
        {
            base.Add(o);
            return true;
        }

        try
        {
            var element = o;
            switch (element.Type)
            {
                case Element.CHUNK:
                    return AddChunk((Chunk)o);
                case Element.PHRASE:
                case Element.PARAGRAPH:
                    var phrase = (Phrase)o;
                    var success = true;
                    foreach (var e in phrase)
                    {
                        if (e is Chunk)
                        {
                            success &= AddChunk((Chunk)e);
                        }
                        else
                        {
                            success &= Add(e);
                        }
                    }

                    return success;
                case Element.MARKED:
                case Element.ANCHOR:
                case Element.ANNOTATION:
                case Element.TABLE: // case added by David Freels
                case Element.PTABLE: // case added by Karen Vardanyan
                // This will only work for PDF!!! Not for RTF/HTML
                case Element.LIST:
                case Element.YMARK:
                    base.Add(o);
                    return true;
                default:
                    throw new InvalidOperationException(element.Type.ToString(CultureInfo.InvariantCulture));
            }
        }
        catch (Exception cce)
        {
            throw new InvalidOperationException("Insertion of illegal Element: " + cce.Message);
        }
    }

    /// <summary>
    ///     Gets a special kind of Phrase that changes some characters into corresponding symbols.
    /// </summary>
    /// <param name="str"></param>
    /// <returns>a newly constructed Phrase</returns>
    public static Phrase GetInstance(string str) => GetInstance(16, str, new Font());

    /// <summary>
    ///     Gets a special kind of Phrase that changes some characters into corresponding symbols.
    /// </summary>
    /// <param name="leading"></param>
    /// <param name="str"></param>
    /// <returns>a newly constructed Phrase</returns>
    public static Phrase GetInstance(int leading, string str) => GetInstance(leading, str, new Font());

    /// <summary>
    ///     Gets a special kind of Phrase that changes some characters into corresponding symbols.
    /// </summary>
    /// <param name="leading"></param>
    /// <param name="str"></param>
    /// <param name="font"></param>
    /// <returns>a newly constructed Phrase</returns>
    public static Phrase GetInstance(int leading, string str, Font font)
    {
        if (str == null)
        {
            throw new ArgumentNullException(nameof(str));
        }

        if (font == null)
        {
            throw new ArgumentNullException(nameof(font));
        }

        var p = new Phrase(true);
        p.Leading = leading;
        p.font = font;
        if (font.Family != Font.SYMBOL && font.Family != Font.ZAPFDINGBATS && font.BaseFont == null)
        {
            int index;
            while ((index = SpecialSymbol.Index(str)) > -1)
            {
                if (index > 0)
                {
                    var firstPart = str.Substring(0, index);
                    p.Add(new Chunk(firstPart, font));
                    str = str.Substring(index);
                }

                var symbol = new Font(Font.SYMBOL, font.Size, font.Style, font.Color);
                var buf = new StringBuilder();
                buf.Append(SpecialSymbol.GetCorrespondingSymbol(str[0]));
                str = str.Substring(1);
                while (SpecialSymbol.Index(str) == 0)
                {
                    buf.Append(SpecialSymbol.GetCorrespondingSymbol(str[0]));
                    str = str.Substring(1);
                }

                p.Add(new Chunk(buf.ToString(), symbol));
            }
        }

        if (!string.IsNullOrEmpty(str))
        {
            p.Add(new Chunk(str, font));
        }

        return p;
    }

    /// <summary>
    ///     Checks if a given tag corresponds with this object.
    /// </summary>
    /// <param name="tag">the given tag</param>
    /// <returns>true if the tag corresponds</returns>
    public static bool IsTag(string tag) => ElementTags.PHRASE.Equals(tag, StringComparison.Ordinal);

    /// <summary>
    ///     Adds a Chunk, an Anchor or another Phrase
    ///     to this Phrase.
    /// </summary>
    /// <param name="index">index at which the specified element is to be inserted</param>
    /// <param name="o">an object of type Chunk, Anchor, or Phrase</param>
    public virtual void Add(int index, object o)
    {
        if (o == null)
        {
            return;
        }

        try
        {
            var element = (IElement)o;
            if (element.Type == Element.CHUNK)
            {
                var chunk = (Chunk)element;
                if (!font.IsStandardFont())
                {
                    chunk.Font = font.Difference(chunk.Font);
                }

                if (hyphenation != null && chunk.GetHyphenation() == null && !chunk.IsEmpty())
                {
                    chunk.SetHyphenation(hyphenation);
                }

                Insert(index, chunk);
            }
            else if (element.Type == Element.PHRASE ||
                     element.Type == Element.ANCHOR ||
                     element.Type == Element.ANNOTATION ||
                     element.Type == Element.TABLE || // line added by David Freels
                     element.Type == Element.YMARK ||
                     element.Type == Element.MARKED)
            {
                Insert(index, element);
            }
            else
            {
                throw new InvalidOperationException(element.Type.ToString(CultureInfo.InvariantCulture));
            }
        }
        catch (Exception cce)
        {
            throw new InvalidOperationException("Insertion of illegal Element: " + cce.Message);
        }
    }

    /// <summary>
    ///     Adds a collection of Chunks
    ///     to this Phrase.
    /// </summary>
    /// <param name="collection">a collection of Chunks, Anchors and Phrases.</param>
    /// <returns>true if the action succeeded, false if not.</returns>
    public bool AddAll<T>(ICollection<T> collection) where T : IElement
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (collection.Count == 0)
        {
            return false;
        }

        foreach (var itm in collection)
        {
            Add(itm);
        }

        return true;
    }

    /// <summary>
    ///     Adds a Object to the Paragraph.
    /// </summary>
    /// <param name="obj">the object to add.</param>
    public void AddSpecial(IElement obj)
    {
        base.Add(obj);
    }

    public bool HasLeading()
    {
        if (float.IsNaN(leading))
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Checks is this Phrase contains no or 1 empty Chunk.
    /// </summary>
    /// <returns>
    ///     false if the Phrase
    ///     contains more than one or more non-emptyChunks.
    /// </returns>
    public bool IsEmpty()
    {
        switch (Count)
        {
            case 0:
                return true;
            case 1:
                var element = this[0];
                if (element.Type == Element.CHUNK && ((Chunk)element).IsEmpty())
                {
                    return true;
                }

                return false;
            default:
                return false;
        }
    }

    /// <summary>
    ///     Adds a Chunk.
    /// </summary>
    /// <remarks>
    ///     This method is a hack to solve a problem I had with phrases that were split between chunks
    ///     in the wrong place.
    /// </remarks>
    /// <param name="chunk">a Chunk</param>
    /// <returns>a bool</returns>
    protected bool AddChunk(Chunk chunk)
    {
        if (chunk == null)
        {
            throw new ArgumentNullException(nameof(chunk));
        }

        var f = chunk.Font;
        var c = chunk.Content;
        if (font != null && !font.IsStandardFont())
        {
            f = font.Difference(chunk.Font);
        }

        if (Count > 0 && !chunk.HasAttributes())
        {
            try
            {
                var previous = (Chunk)this[Count - 1];
                if (!previous.HasAttributes()
                    && (f == null
                        || f.CompareTo(previous.Font) == 0)
                    && previous.Font.CompareTo(f) == 0
                    && !"".Equals(previous.Content.Trim(), StringComparison.Ordinal)
                    && !"".Equals(c.Trim(), StringComparison.Ordinal))
                {
                    previous.Append(c);
                    return true;
                }
            }
            catch
            {
            }
        }

        var newChunk = new Chunk(c, f);
        newChunk.Attributes = chunk.Attributes;
        if (hyphenation != null && newChunk.GetHyphenation() == null && !newChunk.IsEmpty())
        {
            newChunk.SetHyphenation(hyphenation);
        }

        base.Add(newChunk);
        return true;
    }
}