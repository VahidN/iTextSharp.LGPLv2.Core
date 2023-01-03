using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     Interface for customizing the split character.
/// </summary>
public interface ISplitCharacter
{
    /// <summary>
    ///     Returns  true  if the character can split a line. The splitting implementation
    ///     is free to look ahead or look behind characters to make a decision.
    /// </summary>
    /// <param name="start">the lower limit of  cc  inclusive</param>
    /// <param name="current">the pointer to the character in  cc </param>
    /// <param name="end">the upper limit of  cc  exclusive</param>
    /// <param name="cc">an array of characters at least  end  sized</param>
    /// <param name="ck">an array of  PdfChunk . The main use is to be able to call</param>
    /// <returns> true  if the Character(s) can split a line</returns>
    bool IsSplitCharacter(int start, int current, int end, char[] cc, PdfChunk[] ck);
}