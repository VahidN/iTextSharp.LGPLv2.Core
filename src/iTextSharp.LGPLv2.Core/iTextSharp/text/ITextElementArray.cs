namespace iTextSharp.text;

/// <summary>
///     Interface for a text element to which other objects can be added.
/// </summary>
public interface ITextElementArray : IElement
{
    /// <summary>
    ///     Adds an object to the TextElementArray.
    /// </summary>
    /// <param name="o">an object that has to be added</param>
    /// <returns>true if the addition succeeded; false otherwise</returns>
    bool Add(IElement o);
}