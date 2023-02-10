using System.Text;

namespace iTextSharp.LGPLv2.Core.System.Encodings;

/// <summary>
/// </summary>
public sealed class EncodingsRegistry
{
    /// <summary>
    ///     It's a lazy loaded thread-safe singleton.
    /// </summary>
    private static readonly Lazy<EncodingsRegistry> _instance =
        new(() => new EncodingsRegistry(), LazyThreadSafetyMode.ExecutionAndPublication);

    private EncodingsRegistry()
    {
#if !NET40
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
    }

    /// <summary>
    /// </summary>
    /// <exception cref="MemberAccessException">
    ///     The <see cref="T:System.Lazy`1" /> instance is initialized to use the default
    ///     constructor of the type that is being lazily initialized, and permissions to access the constructor are missing.
    /// </exception>
    /// <exception cref="MissingMemberException">
    ///     The <see cref="T:System.Lazy`1" /> instance is initialized to use the default
    ///     constructor of the type that is being lazily initialized, and that type does not have a public, parameterless
    ///     constructor.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     The initialization function tries to access
    ///     <see cref="P:System.Lazy`1.Value" /> on this instance.
    /// </exception>
    public static EncodingsRegistry Instance { get; } = _instance.Value;

    /// <summary>
    ///     Returns the encoding associated with the specified code page identifier.
    /// </summary>
    /// <param name="codepage">
    ///     The code page identifier of the preferred encoding. Possible values are listed in the Code Page
    ///     column of the table that appears in the System.Text.Encoding class topic.-or- 0 (zero), to use the default
    ///     encoding.
    /// </param>
    public Encoding GetEncoding(int codepage) => Encoding.GetEncoding(codepage);

    /// <summary>
    ///     Returns the encoding associated with the specified code page name.
    /// </summary>
    /// <param name="name">
    ///     The code page name of the preferred encoding. Any value returned by the System.Text.Encoding.WebName
    ///     property is valid. Possible values are listed in the Name column of the table that appears in the
    ///     System.Text.Encoding class topic.
    /// </param>
    public Encoding GetEncoding(string name) => Encoding.GetEncoding(name);
}