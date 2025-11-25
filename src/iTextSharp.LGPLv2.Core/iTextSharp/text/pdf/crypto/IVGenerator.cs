using System.Security.Cryptography;

namespace iTextSharp.text.pdf.crypto;

/// <summary>
///     An initialization vector generator for a CBC block encryption.
///     Uses cryptographically secure random number generation.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public static class IvGenerator
{
    /// <summary>
    ///     Gets a 16 byte random initialization vector.
    /// </summary>
    /// <returns>a 16 byte random initialization vector</returns>
    public static byte[] GetIv() => GetIv(16);

    /// <summary>
    ///     Gets a random initialization vector.
    /// </summary>
    /// <param name="len">the length of the initialization vector</param>
    /// <returns>a random initialization vector</returns>
    public static byte[] GetIv(int len)
    {
        var b = new byte[len];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(b);
        }
        return b;
    }
}