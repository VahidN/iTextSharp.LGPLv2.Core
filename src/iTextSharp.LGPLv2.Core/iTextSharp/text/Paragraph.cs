using System.util;

namespace iTextSharp.text;

/// <summary>
///     A Paragraph is a series of Chunks and/or Phrases.
/// </summary>
/// <remarks>
///     A Paragraph has the same qualities of a Phrase, but also
///     some additional layout-parameters:
///     the indentation
///     the alignment of the text
/// </remarks>
/// <example>
///     Paragraph p = new Paragraph("This is a paragraph",
///     FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLDITALIC, new Color(0, 0, 255)));
/// </example>
public class Paragraph : Phrase
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> The alignment of the text. </summary>
    protected int alignment = Element.ALIGN_UNDEFINED;

    ///<summary> The indentation of this paragraph on the left side. </summary>
    protected float indentationLeft;

    ///<summary> The indentation of this paragraph on the right side. </summary>
    protected float indentationRight;

    ///<summary> Does the paragraph has to be kept together on 1 page. </summary>
    protected bool Keeptogether;

    /// <summary>
    ///     The text leading that is multiplied by the biggest font size in the line.
    /// </summary>
    protected float multipliedLeading;

    /// <summary>
    ///     The spacing after the paragraph.
    /// </summary>
    protected float spacingAfter;

    /// <summary>
    ///     The spacing before the paragraph.
    /// </summary>
    protected float spacingBefore;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs a Paragraph.
    /// </summary>
    public Paragraph()
    {
    }

    /// <summary>
    ///     Constructs a Paragraph with a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    public Paragraph(float leading) : base(leading)
    {
    }

    /// <summary>
    ///     Constructs a Paragraph with a certain Chunk.
    /// </summary>
    /// <param name="chunk">a Chunk</param>
    public Paragraph(Chunk chunk) : base(chunk)
    {
    }

    /// <summary>
    ///     Constructs a Paragraph with a certain Chunk
    ///     and a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="chunk">a Chunk</param>
    public Paragraph(float leading, Chunk chunk) : base(leading, chunk)
    {
    }

    /// <summary>
    ///     Constructs a Paragraph with a certain string.
    /// </summary>
    /// <param name="str">a string</param>
    public Paragraph(string str) : base(str)
    {
    }

    /// <summary>
    ///     Constructs a Paragraph with a certain string
    ///     and a certain Font.
    /// </summary>
    /// <param name="str">a string</param>
    /// <param name="font">a Font</param>
    public Paragraph(string str, Font font) : base(str, font)
    {
    }

    /// <summary>
    ///     Constructs a Paragraph with a certain string
    ///     and a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="str">a string</param>
    public Paragraph(float leading, string str) : base(leading, str)
    {
    }

    /// <summary>
    ///     Constructs a Paragraph with a certain leading, string
    ///     and Font.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="str">a string</param>
    /// <param name="font">a Font</param>
    public Paragraph(float leading, string str, Font font) : base(leading, str, font)
    {
    }

    /// <summary>
    ///     Constructs a Paragraph with a certain Phrase.
    /// </summary>
    /// <param name="phrase">a Phrase</param>
    public Paragraph(Phrase phrase) : base(phrase)
    {
        if (phrase is Paragraph)
        {
            var p = (Paragraph)phrase;
            Alignment = p.Alignment;
            ExtraParagraphSpace = p.ExtraParagraphSpace;
            FirstLineIndent = p.FirstLineIndent;
            IndentationLeft = p.IndentationLeft;
            IndentationRight = p.IndentationRight;
            SpacingAfter = p.SpacingAfter;
            SpacingBefore = p.SpacingBefore;
        }
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Get/set the alignment of this paragraph.
    /// </summary>
    /// <value>a integer</value>
    public int Alignment
    {
        get => alignment;
        set => alignment = value;
    }

    /// <summary>
    ///     Holds value of property extraParagraphSpace.
    /// </summary>
    public float ExtraParagraphSpace { get; set; }

    /// <summary>
    ///     Holds value of property firstLineIndent.
    /// </summary>
    public float FirstLineIndent { get; set; }

    /// <summary>
    ///     Get/set the indentation of this paragraph on the left side.
    /// </summary>
    /// <value>a float</value>
    public float IndentationLeft
    {
        get => indentationLeft;

        set => indentationLeft = value;
    }

    /// <summary>
    ///     Get/set the indentation of this paragraph on the right side.
    /// </summary>
    /// <value>a float</value>
    public float IndentationRight
    {
        get => indentationRight;

        set => indentationRight = value;
    }

    /// <summary>
    ///     Set/get if this paragraph has to be kept together on one page.
    /// </summary>
    /// <value>a bool</value>
    public bool KeepTogether
    {
        get => Keeptogether;
        set => Keeptogether = value;
    }

    public override float Leading
    {
        set
        {
            leading = value;
            multipliedLeading = 0;
        }
    }

    /// <summary>
    ///     Sets the variable leading. The resultant leading will be
    ///     multipliedLeading*maxFontSize where maxFontSize is the
    ///     size of the bigest font in the line.
    /// </summary>
    public float MultipliedLeading
    {
        get => multipliedLeading;
        set
        {
            leading = 0;
            multipliedLeading = value;
        }
    }

    public float SpacingAfter
    {
        get => spacingAfter;
        set => spacingAfter = value;
    }

    public float SpacingBefore
    {
        get => spacingBefore;
        set => spacingBefore = value;
    }

    /// <summary>
    ///     Gets the total leading.
    ///     This method is based on the assumption that the
    ///     font of the Paragraph is the font of all the elements
    ///     that make part of the paragraph. This isn't necessarily
    ///     true.
    /// </summary>
    /// <returns>the total leading (fixed and multiplied)</returns>
    public float TotalLeading
    {
        get
        {
            var m = font == null ? Font.DEFAULTSIZE * multipliedLeading : font.GetCalculatedLeading(multipliedLeading);
            if (m > 0 && !HasLeading())
            {
                return m;
            }

            return Leading + m;
        }
    }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public override int Type => Element.PARAGRAPH;

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Checks if a given tag corresponds with this object.
    /// </summary>
    /// <param name="tag">the given tag</param>
    /// <returns>true if the tag corresponds</returns>
    public new static bool IsTag(string tag) => ElementTags.PARAGRAPH.Equals(tag, StringComparison.Ordinal);

    /// <summary>
    ///     Adds an Object to the Paragraph.
    /// </summary>
    /// <param name="o">the object to add</param>
    /// <returns>a bool</returns>
    public override bool Add(IElement o)
    {
        if (o is List)
        {
            var list = (List)o;
            list.IndentationLeft = list.IndentationLeft + indentationLeft;
            list.IndentationRight = indentationRight;
            base.Add(list);
            return true;
        }

        if (o is Image)
        {
            AddSpecial((Image)o);
            return true;
        }

        if (o is Paragraph)
        {
            base.Add(o);
            var chunks = Chunks;
            if (chunks.Count > 0)
            {
                var tmp = chunks[chunks.Count - 1];
                base.Add(new Chunk("\n", tmp.Font));
            }
            else
            {
                base.Add(Chunk.Newline);
            }

            return true;
        }

        base.Add(o);
        return true;
    }

    /// <summary>
    ///     setting the membervariables
    /// </summary>
    /// <summary>
    ///     Sets the alignment of this paragraph.
    /// </summary>
    /// <param name="alignment">the new alignment as a string</param>
    public void SetAlignment(string alignment)
    {
        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_CENTER))
        {
            this.alignment = Element.ALIGN_CENTER;
            return;
        }

        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_RIGHT))
        {
            this.alignment = Element.ALIGN_RIGHT;
            return;
        }

        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED))
        {
            this.alignment = Element.ALIGN_JUSTIFIED;
            return;
        }

        if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED_ALL))
        {
            this.alignment = Element.ALIGN_JUSTIFIED_ALL;
            return;
        }

        this.alignment = Element.ALIGN_LEFT;
    }

    /// <summary>
    ///     Sets the leading fixed and variable. The resultant leading will be
    ///     fixedLeading+multipliedLeading*maxFontSize where maxFontSize is the
    ///     size of the bigest font in the line.
    /// </summary>
    /// <param name="fixedLeading">the fixed leading</param>
    /// <param name="multipliedLeading">the variable leading</param>
    public void SetLeading(float fixedLeading, float multipliedLeading)
    {
        leading = fixedLeading;
        this.multipliedLeading = multipliedLeading;
    }
}