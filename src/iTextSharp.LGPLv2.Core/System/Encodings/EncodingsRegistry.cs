using System.Text;

namespace iTextSharp.LGPLv2.Core.System.Encodings;

/// <summary>
/// </summary>
public static class EncodingsRegistry
{
    static EncodingsRegistry()
    {
#if !NET40
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
    }

    /// <summary>
    ///     Returns the encoding associated with the specified code page identifier.
    /// </summary>
    /// <param name="codepage">
    ///     The code page identifier of the preferred encoding. Possible values are listed in the Code Page
    ///     column of the table that appears in the System.Text.Encoding class topic.-or- 0 (zero), to use the default
    ///     encoding.
    /// </param>
    public static Encoding GetEncoding(int codepage) => Encoding.GetEncoding(codepage);

    /// <summary>
    ///     Returns the encoding associated with the specified code page name.
    /// </summary>
    /// <param name="name">
    ///     The code page name of the preferred encoding. Any value returned by the System.Text.Encoding.WebName
    ///     property is valid. Possible values are listed in the Name column of the table that appears in the
    ///     System.Text.Encoding class topic.
    /// </param>
    public static Encoding GetEncoding(string name) => Encoding.GetEncoding(name);
}