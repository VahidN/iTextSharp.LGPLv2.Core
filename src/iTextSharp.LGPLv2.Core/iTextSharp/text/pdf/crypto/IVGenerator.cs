namespace iTextSharp.text.pdf.crypto;

/// <summary>
///     An initialization vector generator for a CBC block encryption. It's a random generator based on RC4.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public static class IvGenerator
{
    private static readonly ArcfourEncryption _rc4;

    static IvGenerator()
    {
        _rc4 = new ArcfourEncryption();
        var longBytes = new byte[8];
        var val = DateTime.Now.Ticks;
        for (var i = 0; i != 8; i++)
        {
            longBytes[i] = (byte)val;
            val = (long)((ulong)val >> 8);
        }

        _rc4.PrepareArcfourKey(longBytes);
    }

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
        lock (_rc4)
        {
            _rc4.EncryptArcfour(b);
        }

        return b;
    }
}