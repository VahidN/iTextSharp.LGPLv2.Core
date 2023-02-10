namespace iTextSharp.text;

/// <summary>
///     A Chapter is a special Section.
/// </summary>
/// <remarks>
///     A chapter number has to be created using a Paragraph as title
///     and an int as chapter number. The chapter number is shown be
///     default. If you don't want to see the chapter number, you have to set the
///     numberdepth to 0.
/// </remarks>
/// <example>
///     Paragraph title2 = new Paragraph("This is Chapter 2", FontFactory.GetFont(FontFactory.HELVETICA, 18,
///     Font.BOLDITALIC, new Color(0, 0, 255)));
///     Chapter chapter2 = new Chapter(title2, 2);
///     chapter2.SetNumberDepth(0);
///     Paragraph someText = new Paragraph("This is some text");
///     chapter2.Add(someText);
///     Paragraph title21 = new Paragraph("This is Section 1 in Chapter 2", FontFactory.GetFont(FontFactory.HELVETICA, 16,
///     Font.BOLD, new Color(255, 0, 0)));
///     Section section1 =  chapter2.AddSection(title21);
///     Paragraph someSectionText = new Paragraph("This is some silly paragraph in a chapter and/or section. It contains
///     some text to test the functionality of Chapters and Section.");
///     section1.Add(someSectionText);
/// </example>
public class Chapter : Section
{
    /// <summary>
    ///     Constructs a new  Chapter .
    /// </summary>
    /// <param name="number">the Chapter number</param>
    public Chapter(int number) : base(null, 1)
    {
        Numbers = new List<int>();
        Numbers.Add(number);
        triggerNewPage = true;
    }

    /// <summary>
    ///     Constructs a new Chapter.
    /// </summary>
    /// <param name="title">the Chapter title (as a Paragraph)</param>
    /// <param name="number">the Chapter number</param>
    /// <overoads>
    ///     Has three overloads.
    /// </overoads>
    public Chapter(Paragraph title, int number) : base(title, 1)
    {
        Numbers = new List<int>();
        Numbers.Add(number);
        triggerNewPage = true;
    }

    /// <summary>
    ///     Constructs a new Chapter.
    /// </summary>
    /// <param name="title">the Chapter title (as a string)</param>
    /// <param name="number">the Chapter number</param>
    /// <overoads>
    ///     Has three overloads.
    /// </overoads>
    public Chapter(string title, int number) : this(new Paragraph(title), number)
    {
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Gets the type of the text element.
    /// </summary>
    /// <value>a type</value>
    public override int Type { get; } = Element.CHAPTER;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public override bool IsNestable() => false;
}