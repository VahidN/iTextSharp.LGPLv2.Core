namespace iTextSharp.text;

/// <summary>
///     The PageSize-object contains a number of read only rectangles representing the most common paper sizes.
/// </summary>
public static class PageSize
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>
    ///     This is the 11x17 format
    /// </summary>
    public static readonly Rectangle _11X17 = new RectangleReadOnly(792, 1224);

    /// <summary>
    ///     This is the a0 format
    /// </summary>
    public static readonly Rectangle A0 = new RectangleReadOnly(2384, 3370);

    /// <summary>
    ///     This is the a1 format
    /// </summary>
    public static readonly Rectangle A1 = new RectangleReadOnly(1684, 2384);

    /// <summary>
    ///     This is the a10 format
    /// </summary>
    public static readonly Rectangle A10 = new RectangleReadOnly(73, 105);

    /// <summary>
    ///     This is the a2 format
    /// </summary>
    public static readonly Rectangle A2 = new RectangleReadOnly(1191, 1684);

    /// <summary>
    ///     This is the a3 format
    /// </summary>
    public static readonly Rectangle A3 = new RectangleReadOnly(842, 1191);

    /// <summary>
    ///     This is the a4 format
    /// </summary>
    public static readonly Rectangle A4 = new RectangleReadOnly(595, 842);

    /// <summary>
    ///     This is the a5 format
    /// </summary>
    public static readonly Rectangle A5 = new RectangleReadOnly(420, 595);

    /// <summary>
    ///     This is the a6 format
    /// </summary>
    public static readonly Rectangle A6 = new RectangleReadOnly(297, 420);

    /// <summary>
    ///     This is the a7 format
    /// </summary>
    public static readonly Rectangle A7 = new RectangleReadOnly(210, 297);

    /// <summary>
    ///     This is the a8 format
    /// </summary>
    public static readonly Rectangle A8 = new RectangleReadOnly(148, 210);

    /// <summary>
    ///     This is the a9 format
    /// </summary>
    public static readonly Rectangle A9 = new RectangleReadOnly(105, 148);

    /// <summary>
    ///     This is the archA format
    /// </summary>
    public static readonly Rectangle ArchA = new RectangleReadOnly(648, 864);

    /// <summary>
    ///     This is the archB format
    /// </summary>
    public static readonly Rectangle ArchB = new RectangleReadOnly(864, 1296);

    /// <summary>
    ///     This is the archC format
    /// </summary>
    public static readonly Rectangle ArchC = new RectangleReadOnly(1296, 1728);

    /// <summary>
    ///     This is the archD format
    /// </summary>
    public static readonly Rectangle ArchD = new RectangleReadOnly(1728, 2592);

    /// <summary>
    ///     This is the archE format
    /// </summary>
    public static readonly Rectangle ArchE = new RectangleReadOnly(2592, 3456);

    /// <summary>
    ///     This is the b0 format
    /// </summary>
    public static readonly Rectangle B0 = new RectangleReadOnly(2834, 4008);

    /// <summary>
    ///     This is the b1 format
    /// </summary>
    public static readonly Rectangle B1 = new RectangleReadOnly(2004, 2834);

    /// <summary>
    ///     This is the b10 format
    /// </summary>
    public static readonly Rectangle B10 = new RectangleReadOnly(87, 124);

    /// <summary>
    ///     This is the b2 format
    /// </summary>
    public static readonly Rectangle B2 = new RectangleReadOnly(1417, 2004);

    /// <summary>
    ///     This is the b3 format
    /// </summary>
    public static readonly Rectangle B3 = new RectangleReadOnly(1000, 1417);

    /// <summary>
    ///     This is the b4 format
    /// </summary>
    public static readonly Rectangle B4 = new RectangleReadOnly(708, 1000);

    /// <summary>
    ///     This is the b5 format
    /// </summary>
    public static readonly Rectangle B5 = new RectangleReadOnly(498, 708);

    /// <summary>
    ///     This is the b6 format
    /// </summary>
    public static readonly Rectangle B6 = new RectangleReadOnly(354, 498);

    /// <summary>
    ///     This is the b7 format
    /// </summary>
    public static readonly Rectangle B7 = new RectangleReadOnly(249, 354);

    /// <summary>
    ///     This is the b8 format
    /// </summary>
    public static readonly Rectangle B8 = new RectangleReadOnly(175, 249);

    /// <summary>
    ///     This is the b9 format
    /// </summary>
    public static readonly Rectangle B9 = new RectangleReadOnly(124, 175);

    /// <summary>
    ///     This is the Crown Octavo format
    /// </summary>
    public static readonly Rectangle CrownOctavo = new RectangleReadOnly(348, 527);

    /// <summary>
    ///     This is the Crown Quarto format
    /// </summary>
    public static readonly Rectangle CrownQuarto = new RectangleReadOnly(535, 697);

    /// <summary>
    ///     This is the Demy Octavo format
    /// </summary>
    public static readonly Rectangle DemyOctavo = new RectangleReadOnly(391, 612);

    /// <summary>
    ///     This is the Demy Quarto format.
    /// </summary>
    public static readonly Rectangle DemyQuarto = new RectangleReadOnly(620, 782);

    /// <summary>
    ///     This is the executive format
    /// </summary>
    public static readonly Rectangle Executive = new RectangleReadOnly(522, 756);

    /// <summary>
    ///     This is the American Foolscap format
    /// </summary>
    public static readonly Rectangle Flsa = new RectangleReadOnly(612, 936);

    /// <summary>
    ///     This is the European Foolscap format
    /// </summary>
    public static readonly Rectangle Flse = new RectangleReadOnly(648, 936);

    /// <summary>
    ///     This is the halfletter format
    /// </summary>
    public static readonly Rectangle Halfletter = new RectangleReadOnly(396, 612);

    /// <summary>
    ///     This is the ISO 7810 ID-1 format (85.60 x 53.98 mm or 3.370 x 2.125 inch)
    /// </summary>
    public static readonly Rectangle Id1 = new RectangleReadOnly(242.65f, 153);

    /// <summary>
    ///     This is the ISO 7810 ID-2 format (A7 rotated)
    /// </summary>
    public static readonly Rectangle Id2 = new RectangleReadOnly(297, 210);

    /// <summary>
    ///     This is the ISO 7810 ID-3 format (B7 rotated)
    /// </summary>
    public static readonly Rectangle Id3 = new RectangleReadOnly(354, 249);

    /// <summary>
    ///     This is the Large Crown Octavo format
    /// </summary>
    public static readonly Rectangle LargeCrownOctavo = new RectangleReadOnly(365, 561);

    /// <summary>
    ///     This is the Large Crown Quarto format
    /// </summary>
    public static readonly Rectangle LargeCrownQuarto = new RectangleReadOnly(569, 731);

    /// <summary>
    ///     This is the ledger format
    /// </summary>
    public static readonly Rectangle Ledger = new RectangleReadOnly(1224, 792);

    /// <summary>
    ///     This is the legal format
    /// </summary>
    public static readonly Rectangle Legal = new RectangleReadOnly(612, 1008);

    /// <summary>
    ///     This is the letter format
    /// </summary>
    public static readonly Rectangle Letter = new RectangleReadOnly(612, 792);

    /// <summary>
    ///     This is the note format
    /// </summary>
    public static readonly Rectangle Note = new RectangleReadOnly(540, 720);

    /// <summary>
    ///     This is the Penguin large paparback format.
    /// </summary>
    public static readonly Rectangle PenguinLargePaperback = new RectangleReadOnly(365, 561);

    /// <summary>
    ///     This is the Pengiun small paperback format.
    /// </summary>
    public static readonly Rectangle PenguinSmallPaperback = new RectangleReadOnly(314, 513);

    /// <summary>
    ///     This is the postcard format
    /// </summary>
    public static readonly Rectangle Postcard = new RectangleReadOnly(283, 416);

    /// <summary>
    ///     This is the Royal Octavo format.
    /// </summary>
    public static readonly Rectangle RoyalOctavo = new RectangleReadOnly(442, 663);

    /// <summary>
    ///     This is the Royal Quarto format.
    /// </summary>
    public static readonly Rectangle RoyalQuarto = new RectangleReadOnly(671, 884);

    /// <summary>
    ///     This is the small paperback format.
    /// </summary>
    public static readonly Rectangle SmallPaperback = new RectangleReadOnly(314, 504);

    /// <summary>
    ///     This is the tabloid format
    /// </summary>
    public static readonly Rectangle Tabloid = new RectangleReadOnly(792, 1224);

    /// <summary>
    ///     This method returns a Rectangle based on a String.
    ///     Possible values are the the names of a constant in this class
    ///     (for instance "A4", "LETTER",...) or a value like "595 842"
    /// </summary>
    public static Rectangle GetRectangle(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        name = name.Trim().ToUpper(CultureInfo.InvariantCulture);
        var pos = name.IndexOf(" ", StringComparison.Ordinal);
        if (pos == -1)
        {
            try
            {
                return (Rectangle)typeof(PageSize).GetField(name).GetValue(null);
            }
            catch (Exception)
            {
                throw new ArgumentException("Can't find page size " + name);
            }
        }

        try
        {
            var width = name.Substring(0, pos);
            var height = name.Substring(pos + 1);
            return new Rectangle(float.Parse(width, NumberFormatInfo.InvariantInfo),
                                 float.Parse(height, NumberFormatInfo.InvariantInfo));
        }
        catch (Exception e)
        {
            throw new ArgumentException(name + " is not a valid page size format: " + e.Message);
        }
    }
}