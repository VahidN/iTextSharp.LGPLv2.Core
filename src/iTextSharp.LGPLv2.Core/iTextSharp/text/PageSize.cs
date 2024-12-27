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
    public static readonly Rectangle _11X17 = new RectangleReadOnly(urx: 792, ury: 1224);

    /// <summary>
    ///     This is the a0 format
    /// </summary>
    public static readonly Rectangle A0 = new RectangleReadOnly(urx: 2384, ury: 3370);

    /// <summary>
    ///     This is the a1 format
    /// </summary>
    public static readonly Rectangle A1 = new RectangleReadOnly(urx: 1684, ury: 2384);

    /// <summary>
    ///     This is the a10 format
    /// </summary>
    public static readonly Rectangle A10 = new RectangleReadOnly(urx: 73, ury: 105);

    /// <summary>
    ///     This is the a2 format
    /// </summary>
    public static readonly Rectangle A2 = new RectangleReadOnly(urx: 1191, ury: 1684);

    /// <summary>
    ///     This is the a3 format
    /// </summary>
    public static readonly Rectangle A3 = new RectangleReadOnly(urx: 842, ury: 1191);

    /// <summary>
    ///     This is the a4 format
    /// </summary>
    public static readonly Rectangle A4 = new RectangleReadOnly(urx: 595, ury: 842);

    /// <summary>
    ///     This is the a5 format
    /// </summary>
    public static readonly Rectangle A5 = new RectangleReadOnly(urx: 420, ury: 595);

    /// <summary>
    ///     This is the a6 format
    /// </summary>
    public static readonly Rectangle A6 = new RectangleReadOnly(urx: 297, ury: 420);

    /// <summary>
    ///     This is the a7 format
    /// </summary>
    public static readonly Rectangle A7 = new RectangleReadOnly(urx: 210, ury: 297);

    /// <summary>
    ///     This is the a8 format
    /// </summary>
    public static readonly Rectangle A8 = new RectangleReadOnly(urx: 148, ury: 210);

    /// <summary>
    ///     This is the a9 format
    /// </summary>
    public static readonly Rectangle A9 = new RectangleReadOnly(urx: 105, ury: 148);

    /// <summary>
    ///     This is the archA format
    /// </summary>
    public static readonly Rectangle ArchA = new RectangleReadOnly(urx: 648, ury: 864);

    /// <summary>
    ///     This is the archB format
    /// </summary>
    public static readonly Rectangle ArchB = new RectangleReadOnly(urx: 864, ury: 1296);

    /// <summary>
    ///     This is the archC format
    /// </summary>
    public static readonly Rectangle ArchC = new RectangleReadOnly(urx: 1296, ury: 1728);

    /// <summary>
    ///     This is the archD format
    /// </summary>
    public static readonly Rectangle ArchD = new RectangleReadOnly(urx: 1728, ury: 2592);

    /// <summary>
    ///     This is the archE format
    /// </summary>
    public static readonly Rectangle ArchE = new RectangleReadOnly(urx: 2592, ury: 3456);

    /// <summary>
    ///     This is the b0 format
    /// </summary>
    public static readonly Rectangle B0 = new RectangleReadOnly(urx: 2834, ury: 4008);

    /// <summary>
    ///     This is the b1 format
    /// </summary>
    public static readonly Rectangle B1 = new RectangleReadOnly(urx: 2004, ury: 2834);

    /// <summary>
    ///     This is the b10 format
    /// </summary>
    public static readonly Rectangle B10 = new RectangleReadOnly(urx: 87, ury: 124);

    /// <summary>
    ///     This is the b2 format
    /// </summary>
    public static readonly Rectangle B2 = new RectangleReadOnly(urx: 1417, ury: 2004);

    /// <summary>
    ///     This is the b3 format
    /// </summary>
    public static readonly Rectangle B3 = new RectangleReadOnly(urx: 1000, ury: 1417);

    /// <summary>
    ///     This is the b4 format
    /// </summary>
    public static readonly Rectangle B4 = new RectangleReadOnly(urx: 708, ury: 1000);

    /// <summary>
    ///     This is the b5 format
    /// </summary>
    public static readonly Rectangle B5 = new RectangleReadOnly(urx: 498, ury: 708);

    /// <summary>
    ///     This is the b6 format
    /// </summary>
    public static readonly Rectangle B6 = new RectangleReadOnly(urx: 354, ury: 498);

    /// <summary>
    ///     This is the b7 format
    /// </summary>
    public static readonly Rectangle B7 = new RectangleReadOnly(urx: 249, ury: 354);

    /// <summary>
    ///     This is the b8 format
    /// </summary>
    public static readonly Rectangle B8 = new RectangleReadOnly(urx: 175, ury: 249);

    /// <summary>
    ///     This is the b9 format
    /// </summary>
    public static readonly Rectangle B9 = new RectangleReadOnly(urx: 124, ury: 175);

    /// <summary>
    ///     This is the Crown Octavo format
    /// </summary>
    public static readonly Rectangle CrownOctavo = new RectangleReadOnly(urx: 348, ury: 527);

    /// <summary>
    ///     This is the Crown Quarto format
    /// </summary>
    public static readonly Rectangle CrownQuarto = new RectangleReadOnly(urx: 535, ury: 697);

    /// <summary>
    ///     This is the Demy Octavo format
    /// </summary>
    public static readonly Rectangle DemyOctavo = new RectangleReadOnly(urx: 391, ury: 612);

    /// <summary>
    ///     This is the Demy Quarto format.
    /// </summary>
    public static readonly Rectangle DemyQuarto = new RectangleReadOnly(urx: 620, ury: 782);

    /// <summary>
    ///     This is the executive format
    /// </summary>
    public static readonly Rectangle Executive = new RectangleReadOnly(urx: 522, ury: 756);

    /// <summary>
    ///     This is the American Foolscap format
    /// </summary>
    public static readonly Rectangle Flsa = new RectangleReadOnly(urx: 612, ury: 936);

    /// <summary>
    ///     This is the European Foolscap format
    /// </summary>
    public static readonly Rectangle Flse = new RectangleReadOnly(urx: 648, ury: 936);

    /// <summary>
    ///     This is the halfletter format
    /// </summary>
    public static readonly Rectangle Halfletter = new RectangleReadOnly(urx: 396, ury: 612);

    /// <summary>
    ///     This is the ISO 7810 ID-1 format (85.60 x 53.98 mm or 3.370 x 2.125 inch)
    /// </summary>
    public static readonly Rectangle Id1 = new RectangleReadOnly(urx: 242.65f, ury: 153);

    /// <summary>
    ///     This is the ISO 7810 ID-2 format (A7 rotated)
    /// </summary>
    public static readonly Rectangle Id2 = new RectangleReadOnly(urx: 297, ury: 210);

    /// <summary>
    ///     This is the ISO 7810 ID-3 format (B7 rotated)
    /// </summary>
    public static readonly Rectangle Id3 = new RectangleReadOnly(urx: 354, ury: 249);

    /// <summary>
    ///     This is the Large Crown Octavo format
    /// </summary>
    public static readonly Rectangle LargeCrownOctavo = new RectangleReadOnly(urx: 365, ury: 561);

    /// <summary>
    ///     This is the Large Crown Quarto format
    /// </summary>
    public static readonly Rectangle LargeCrownQuarto = new RectangleReadOnly(urx: 569, ury: 731);

    /// <summary>
    ///     This is the ledger format
    /// </summary>
    public static readonly Rectangle Ledger = new RectangleReadOnly(urx: 1224, ury: 792);

    /// <summary>
    ///     This is the legal format
    /// </summary>
    public static readonly Rectangle Legal = new RectangleReadOnly(urx: 612, ury: 1008);

    /// <summary>
    ///     This is the letter format
    /// </summary>
    public static readonly Rectangle Letter = new RectangleReadOnly(urx: 612, ury: 792);

    /// <summary>
    ///     This is the note format
    /// </summary>
    public static readonly Rectangle Note = new RectangleReadOnly(urx: 540, ury: 720);

    /// <summary>
    ///     This is the Penguin large paparback format.
    /// </summary>
    public static readonly Rectangle PenguinLargePaperback = new RectangleReadOnly(urx: 365, ury: 561);

    /// <summary>
    ///     This is the Pengiun small paperback format.
    /// </summary>
    public static readonly Rectangle PenguinSmallPaperback = new RectangleReadOnly(urx: 314, ury: 513);

    /// <summary>
    ///     This is the postcard format
    /// </summary>
    public static readonly Rectangle Postcard = new RectangleReadOnly(urx: 283, ury: 416);

    /// <summary>
    ///     This is the Royal Octavo format.
    /// </summary>
    public static readonly Rectangle RoyalOctavo = new RectangleReadOnly(urx: 442, ury: 663);

    /// <summary>
    ///     This is the Royal Quarto format.
    /// </summary>
    public static readonly Rectangle RoyalQuarto = new RectangleReadOnly(urx: 671, ury: 884);

    /// <summary>
    ///     This is the small paperback format.
    /// </summary>
    public static readonly Rectangle SmallPaperback = new RectangleReadOnly(urx: 314, ury: 504);

    /// <summary>
    ///     This is the tabloid format
    /// </summary>
    public static readonly Rectangle Tabloid = new RectangleReadOnly(urx: 792, ury: 1224);

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
        var pos = name.IndexOf(value: ' ', StringComparison.Ordinal);

        if (pos == -1)
        {
            try
            {
                return (Rectangle)typeof(PageSize).GetField(name).GetValue(obj: null);
            }
            catch (Exception)
            {
                throw new ArgumentException("Can't find page size " + name);
            }
        }

        try
        {
            var width = name.Substring(startIndex: 0, pos);
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