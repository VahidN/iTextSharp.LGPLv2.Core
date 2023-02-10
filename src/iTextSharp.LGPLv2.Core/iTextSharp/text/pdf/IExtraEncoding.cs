namespace iTextSharp.text.pdf;

/// <summary>
///     Classes implementing this interface can create custom encodings or
///     replace existing ones. It is used in the context of  PdfEncoding .
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public interface IExtraEncoding
{
    /// <summary>
    ///     Converts a byte array to an Unicode string according to some encoding.
    ///     supports more than one encoding.
    /// </summary>
    /// <param name="b">the input byte array</param>
    /// <param name="encoding">the requested encoding. It's mainly of use if the same class</param>
    /// <returns>the conversion or  null  if no conversion is supported</returns>
    string ByteToChar(byte[] b, string encoding);

    /// <summary>
    ///     Converts an Unicode string to a byte array according to some encoding.
    ///     supports more than one encoding.
    /// </summary>
    /// <param name="text">the Unicode string</param>
    /// <param name="encoding">the requested encoding. It's mainly of use if the same class</param>
    /// <returns>the conversion or  null  if no conversion is supported</returns>
    byte[] CharToByte(string text, string encoding);

    /// <summary>
    ///     Converts an Unicode char to a byte array according to some encoding.
    ///     supports more than one encoding.
    /// </summary>
    /// <param name="char1">the Unicode char</param>
    /// <param name="encoding">the requested encoding. It's mainly of use if the same class</param>
    /// <returns>the conversion or  null  if no conversion is supported</returns>
    byte[] CharToByte(char char1, string encoding);
}