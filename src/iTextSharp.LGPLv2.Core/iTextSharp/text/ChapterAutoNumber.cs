namespace iTextSharp.text;

/// <summary>
///     Chapter with auto numbering.
///     @author Michael Niedermair
/// </summary>
public class ChapterAutoNumber : Chapter
{
    /// <summary>
    ///     Is the chapter number already set?
    ///     @since	2.1.4
    /// </summary>
    protected bool NumberSet;

    /// <summary>
    ///     Create a new object.
    /// </summary>
    /// <param name="para">the Chapter title (as a  Paragraph )</param>
    public ChapterAutoNumber(Paragraph para) : base(para, 0)
    {
    }

    /// <summary>
    ///     Create a new objet.
    /// </summary>
    /// <param name="title">the Chapter title (as a  String )</param>
    public ChapterAutoNumber(string title) : base(title, 0)
    {
    }

    /// <summary>
    ///     Create a new section for this chapter and ad it.
    /// </summary>
    /// <param name="title">the Section title (as a  String )</param>
    /// <returns>Returns the new section.</returns>
    public override Section AddSection(string title)
    {
        if (AddedCompletely)
        {
            throw new InvalidOperationException("This LargeElement has already been added to the Document.");
        }

        return AddSection(title, 2);
    }

    /// <summary>
    ///     Create a new section for this chapter and add it.
    /// </summary>
    /// <param name="title">the Section title (as a  Paragraph )</param>
    /// <returns>Returns the new section.</returns>
    public override Section AddSection(Paragraph title)
    {
        if (AddedCompletely)
        {
            throw new InvalidOperationException("This LargeElement has already been added to the Document.");
        }

        return AddSection(title, 2);
    }

    /// <summary>
    ///     Changes the Chapter number.
    ///     @since 2.1.4
    /// </summary>
    /// <param name="number">chapter number</param>
    public int SetAutomaticNumber(int number)
    {
        if (!NumberSet)
        {
            number++;
            SetChapterNumber(number);
            NumberSet = true;
        }

        return number;
    }
}