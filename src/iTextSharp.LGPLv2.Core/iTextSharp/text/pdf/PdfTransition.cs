namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public class PdfTransition
{
    /// <summary>
    ///     Vertical Blinds
    /// </summary>
    public const int BLINDH = 6;

    /// <summary>
    ///     Vertical Blinds
    /// </summary>
    public const int BLINDV = 5;

    /// <summary>
    ///     Bottom-Top Wipe
    /// </summary>
    public const int BTWIPE = 11;

    /// <summary>
    ///     Diagonal Glitter
    /// </summary>
    public const int DGLITTER = 16;

    /// <summary>
    ///     Dissolve
    /// </summary>
    public const int DISSOLVE = 13;

    /// <summary>
    ///     Inward Box
    /// </summary>
    public const int INBOX = 7;

    /// <summary>
    ///     Left-Right Glitter
    /// </summary>
    public const int LRGLITTER = 14;

    /// <summary>
    ///     Left-Right Wipe
    /// </summary>
    public const int LRWIPE = 9;

    /// <summary>
    ///     Outward Box
    /// </summary>
    public const int OUTBOX = 8;

    /// <summary>
    ///     Right-Left Wipe
    /// </summary>
    public const int RLWIPE = 10;

    /// <summary>
    ///     IN Horizontal Split
    /// </summary>
    public const int SPLITHIN = 4;

    /// <summary>
    ///     Out Horizontal Split
    /// </summary>
    public const int SPLITHOUT = 2;

    /// <summary>
    ///     In Vertical Split
    /// </summary>
    public const int SPLITVIN = 3;

    /// <summary>
    ///     Out Vertical Split
    /// </summary>
    public const int SPLITVOUT = 1;

    /// <summary>
    ///     Top-Bottom Glitter
    /// </summary>
    public const int TBGLITTER = 15;

    /// <summary>
    ///     Top-Bottom Wipe
    /// </summary>
    public const int TBWIPE = 12;

    /// <summary>
    ///     duration of the transition effect
    /// </summary>
    protected int duration;

    /// <summary>
    ///     type of the transition effect
    /// </summary>
    protected int type;

    /// <summary>
    ///     Constructs a  Transition .
    /// </summary>
    public PdfTransition() : this(BLINDH)
    {
    }

    /// <summary>
    ///     Constructs a  Transition .
    /// </summary>
    /// <param name="type">type of the transition effect</param>
    public PdfTransition(int type) : this(type, 1)
    {
    }

    /// <summary>
    ///     Constructs a  Transition .
    /// </summary>
    /// <param name="type">type of the transition effect</param>
    /// <param name="duration">duration of the transition effect</param>
    public PdfTransition(int type, int duration)
    {
        this.duration = duration;
        this.type = type;
    }


    public int Duration => duration;


    public PdfDictionary TransitionDictionary
    {
        get
        {
            var trans = new PdfDictionary(PdfName.Trans);
            switch (type)
            {
                case SPLITVOUT:
                    trans.Put(PdfName.S, PdfName.Split);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Dm, PdfName.V);
                    trans.Put(PdfName.M, PdfName.O);
                    break;
                case SPLITHOUT:
                    trans.Put(PdfName.S, PdfName.Split);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Dm, PdfName.H);
                    trans.Put(PdfName.M, PdfName.O);
                    break;
                case SPLITVIN:
                    trans.Put(PdfName.S, PdfName.Split);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Dm, PdfName.V);
                    trans.Put(PdfName.M, PdfName.I);
                    break;
                case SPLITHIN:
                    trans.Put(PdfName.S, PdfName.Split);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Dm, PdfName.H);
                    trans.Put(PdfName.M, PdfName.I);
                    break;
                case BLINDV:
                    trans.Put(PdfName.S, PdfName.Blinds);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Dm, PdfName.V);
                    break;
                case BLINDH:
                    trans.Put(PdfName.S, PdfName.Blinds);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Dm, PdfName.H);
                    break;
                case INBOX:
                    trans.Put(PdfName.S, PdfName.Box);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.M, PdfName.I);
                    break;
                case OUTBOX:
                    trans.Put(PdfName.S, PdfName.Box);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.M, PdfName.O);
                    break;
                case LRWIPE:
                    trans.Put(PdfName.S, PdfName.Wipe);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Di, new PdfNumber(0));
                    break;
                case RLWIPE:
                    trans.Put(PdfName.S, PdfName.Wipe);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Di, new PdfNumber(180));
                    break;
                case BTWIPE:
                    trans.Put(PdfName.S, PdfName.Wipe);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Di, new PdfNumber(90));
                    break;
                case TBWIPE:
                    trans.Put(PdfName.S, PdfName.Wipe);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Di, new PdfNumber(270));
                    break;
                case DISSOLVE:
                    trans.Put(PdfName.S, PdfName.Dissolve);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    break;
                case LRGLITTER:
                    trans.Put(PdfName.S, PdfName.Glitter);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Di, new PdfNumber(0));
                    break;
                case TBGLITTER:
                    trans.Put(PdfName.S, PdfName.Glitter);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Di, new PdfNumber(270));
                    break;
                case DGLITTER:
                    trans.Put(PdfName.S, PdfName.Glitter);
                    trans.Put(PdfName.D, new PdfNumber(duration));
                    trans.Put(PdfName.Di, new PdfNumber(315));
                    break;
            }

            return trans;
        }
    }

    public int Type => type;
}