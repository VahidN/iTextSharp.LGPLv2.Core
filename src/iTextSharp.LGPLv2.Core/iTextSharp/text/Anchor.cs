namespace iTextSharp.text;

/// <summary>
///     An Anchor can be a reference or a destination of a reference.
/// </summary>
/// <remarks>
///     An Anchor is a special kind of Phrase
///     It is constructed in the same way.
/// </remarks>
public class Anchor : Phrase
{
    /// <summary>
    ///     This is the name of the Anchor.
    /// </summary>
    protected string name;

    /// <summary>
    ///     This is the reference of the Anchor.
    /// </summary>
    protected string reference;

    /// <summary>
    ///     constructors
    /// </summary>
    /// <summary>
    ///     Constructs an Anchor without specifying a leading.
    /// </summary>
    /// <overloads>
    ///     Has nine overloads.
    /// </overloads>
    public Anchor() : base(16)
    {
    }

    /// <summary>
    ///     Constructs an Anchor with a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    public Anchor(float leading) : base(leading)
    {
    }

    /// <summary>
    ///     Constructs an Anchor with a certain Chunk.
    /// </summary>
    /// <param name="chunk">a Chunk</param>
    public Anchor(Chunk chunk) : base(chunk)
    {
    }

    /// <summary>
    ///     Constructs an Anchor with a certain string.
    /// </summary>
    /// <param name="str">a string</param>
    public Anchor(string str) : base(str)
    {
    }

    /// <summary>
    ///     Constructs an Anchor with a certain string
    ///     and a certain Font.
    /// </summary>
    /// <param name="str">a string</param>
    /// <param name="font">a Font</param>
    public Anchor(string str, Font font) : base(str, font)
    {
    }

    /// <summary>
    ///     Constructs an Anchor with a certain Chunk
    ///     and a certain leading.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="chunk">a Chunk</param>
    public Anchor(float leading, Chunk chunk) : base(leading, chunk)
    {
    }

    /// <summary>
    ///     Constructs an Anchor with a certain leading
    ///     and a certain string.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="str">a string</param>
    public Anchor(float leading, string str) : base(leading, str)
    {
    }

    /// <summary>
    ///     Constructs an Anchor with a certain leading,
    ///     a certain string and a certain Font.
    /// </summary>
    /// <param name="leading">the leading</param>
    /// <param name="str">a string</param>
    /// <param name="font">a Font</param>
    public Anchor(float leading, string str, Font font) : base(leading, str, font)
    {
    }

    /// <summary>
    ///     Constructs an  Anchor  with a certain  Phrase .
    /// </summary>
    /// <param name="phrase">a  Phrase </param>
    public Anchor(Phrase phrase) : base(phrase)
    {
        var anchor = phrase as Anchor;
        if (anchor != null)
        {
            var a = anchor;
            Name = a.name;
            Reference = a.reference;
        }
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public override IList<Chunk> Chunks
    {
        get
        {
            var tmp = new List<Chunk>();
            var localDestination = reference != null && reference.StartsWith("#", StringComparison.Ordinal);
            var notGotoOk = true;
            foreach (Chunk chunk in this)
            {
                if (name != null && notGotoOk && !chunk.IsEmpty())
                {
                    chunk.SetLocalDestination(name);
                    notGotoOk = false;
                }

                if (localDestination)
                {
                    chunk.SetLocalGoto(reference.Substring(1));
                }
                else if (reference != null)
                {
                    chunk.SetAnchor(reference);
                }

                tmp.Add(chunk);
            }

            return tmp;
        }
    }

    /// <summary>
    ///     Name of this Anchor.
    /// </summary>
    public string Name
    {
        get => name;

        set => name = value;
    }

    /// <summary>
    ///     reference of this Anchor.
    /// </summary>
    public string Reference
    {
        get => reference;

        set => reference = value;
    }

    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public override int Type { get; } = Element.ANCHOR;

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     reference of this Anchor.
    /// </summary>
    /// <value>an Uri</value>
    public Uri Url
    {
        get
        {
            try
            {
                return new Uri(reference);
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was processed successfully</returns>
    public override bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            var localDestination = reference != null && reference.StartsWith("#", StringComparison.Ordinal);
            var notGotoOk = true;
            foreach (var chunk in Chunks)
            {
                if (name != null && notGotoOk && !chunk.IsEmpty())
                {
                    chunk.SetLocalDestination(name);
                    notGotoOk = false;
                }

                if (localDestination)
                {
                    chunk.SetLocalGoto(reference.Substring(1));
                }
                else if (reference != null)
                {
                    chunk.SetAnchor(reference);
                }

                listener.Add(chunk);
            }

            return true;
        }
        catch (DocumentException)
        {
            return false;
        }
    }
}