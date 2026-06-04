namespace iTextSharp.text.pdf.security;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

/// <summary>
///     Utility class providing digest algorithm constants and hash computation methods.
///     Maps OIDs to digest names and provides <c>Digest()</c> methods for streams and byte arrays.
/// </summary>
public static class DigestAlgorithms
{
    /// <summary>SHA-1 (PDF 1.3+)</summary>
    public const string Sha1 = "SHA-1";

    /// <summary>SHA-256 (PDF 1.6+)</summary>
    public const string Sha256 = "SHA-256";

    /// <summary>SHA-384 (PDF 1.7+)</summary>
    public const string Sha384 = "SHA-384";

    /// <summary>SHA-512 (PDF 1.7+)</summary>
    public const string Sha512 = "SHA-512";

    /// <summary>RIPEMD-160</summary>
    public const string Ripemd160 = "RIPEMD160";

    private static readonly Dictionary<string, string> _digestNames = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, string> _allowedDigests = new(StringComparer.OrdinalIgnoreCase);

    static DigestAlgorithms()
    {
        _digestNames["1.2.840.113549.2.5"] = "MD5";
        _digestNames["1.2.840.113549.2.2"] = "MD2";
        _digestNames["1.3.14.3.2.26"] = "SHA1";
        _digestNames["2.16.840.1.101.3.4.2.4"] = "SHA224";
        _digestNames["2.16.840.1.101.3.4.2.1"] = "SHA256";
        _digestNames["2.16.840.1.101.3.4.2.2"] = "SHA384";
        _digestNames["2.16.840.1.101.3.4.2.3"] = "SHA512";
        _digestNames["1.3.36.3.2.2"] = "RIPEMD128";
        _digestNames["1.3.36.3.2.1"] = "RIPEMD160";
        _digestNames["1.3.36.3.2.3"] = "RIPEMD256";
        _digestNames["1.2.840.113549.1.1.4"] = "MD5";
        _digestNames["1.2.840.113549.1.1.2"] = "MD2";
        _digestNames["1.2.840.113549.1.1.5"] = "SHA1";
        _digestNames["1.2.840.113549.1.1.14"] = "SHA224";
        _digestNames["1.2.840.113549.1.1.11"] = "SHA256";
        _digestNames["1.2.840.113549.1.1.12"] = "SHA384";
        _digestNames["1.2.840.113549.1.1.13"] = "SHA512";
        _digestNames["1.2.840.10040.4.3"] = "SHA1";
        _digestNames["2.16.840.1.101.3.4.3.1"] = "SHA224";
        _digestNames["2.16.840.1.101.3.4.3.2"] = "SHA256";
        _digestNames["2.16.840.1.101.3.4.3.3"] = "SHA384";
        _digestNames["2.16.840.1.101.3.4.3.4"] = "SHA512";
        _digestNames["1.3.36.3.3.1.3"] = "RIPEMD128";
        _digestNames["1.3.36.3.3.1.2"] = "RIPEMD160";
        _digestNames["1.3.36.3.3.1.4"] = "RIPEMD256";
        _digestNames["1.2.643.2.2.9"] = "GOST3411";

        _allowedDigests["MD2"] = "1.2.840.113549.2.2";
        _allowedDigests["MD-2"] = "1.2.840.113549.2.2";
        _allowedDigests["MD5"] = "1.2.840.113549.2.5";
        _allowedDigests["MD-5"] = "1.2.840.113549.2.5";
        _allowedDigests["SHA1"] = "1.3.14.3.2.26";
        _allowedDigests["SHA-1"] = "1.3.14.3.2.26";
        _allowedDigests["SHA224"] = "2.16.840.1.101.3.4.2.4";
        _allowedDigests["SHA-224"] = "2.16.840.1.101.3.4.2.4";
        _allowedDigests["SHA256"] = "2.16.840.1.101.3.4.2.1";
        _allowedDigests["SHA-256"] = "2.16.840.1.101.3.4.2.1";
        _allowedDigests["SHA384"] = "2.16.840.1.101.3.4.2.2";
        _allowedDigests["SHA-384"] = "2.16.840.1.101.3.4.2.2";
        _allowedDigests["SHA512"] = "2.16.840.1.101.3.4.2.3";
        _allowedDigests["SHA-512"] = "2.16.840.1.101.3.4.2.3";
        _allowedDigests["RIPEMD128"] = "1.3.36.3.2.2";
        _allowedDigests["RIPEMD-128"] = "1.3.36.3.2.2";
        _allowedDigests["RIPEMD160"] = "1.3.36.3.2.1";
        _allowedDigests["RIPEMD-160"] = "1.3.36.3.2.1";
        _allowedDigests["RIPEMD256"] = "1.3.36.3.2.3";
        _allowedDigests["RIPEMD-256"] = "1.3.36.3.2.3";
        _allowedDigests["GOST3411"] = "1.2.643.2.2.9";
    }

    /// <summary>
    ///     Returns a message digest instance for the given algorithm name.
    /// </summary>
    /// <param name="hashAlgorithm">the digest algorithm name (e.g. "SHA-256")</param>
    /// <returns>an <see cref="IDigest"/> instance</returns>
    public static IDigest GetMessageDigest(string hashAlgorithm) =>
        DigestUtilities.GetDigest(hashAlgorithm);

    /// <summary>
    ///     Returns a message digest instance for the given OID.
    /// </summary>
    /// <param name="digestOid">the digest OID</param>
    /// <returns>an <see cref="IDigest"/> instance</returns>
    public static IDigest GetMessageDigestFromOid(string digestOid) =>
        DigestUtilities.GetDigest(digestOid);

    /// <summary>
    ///     Computes the hash of a stream using the specified algorithm.
    /// </summary>
    /// <param name="data">the input stream</param>
    /// <param name="hashAlgorithm">the digest algorithm name</param>
    /// <returns>the hash bytes</returns>
    public static byte[] Digest(Stream data, string hashAlgorithm) =>
        Digest(data, GetMessageDigest(hashAlgorithm));

    /// <summary>
    ///     Computes the hash of a stream using the given digest instance.
    /// </summary>
    /// <param name="data">the input stream</param>
    /// <param name="messageDigest">the digest instance</param>
    /// <returns>the hash bytes</returns>
    public static byte[] Digest(Stream data, IDigest messageDigest)
    {
        var buf = new byte[8192];
        int n;
        while ((n = data.Read(buf, 0, buf.Length)) > 0)
        {
            messageDigest.BlockUpdate(buf, 0, n);
        }

        var result = new byte[messageDigest.GetDigestSize()];
        messageDigest.DoFinal(result, 0);
        return result;
    }

    /// <summary>
    ///     Computes the hash of a byte array segment.
    /// </summary>
    public static byte[] Digest(string algo, byte[] b, int offset, int len) =>
        Digest(DigestUtilities.GetDigest(algo), b, offset, len);

    /// <summary>
    ///     Computes the hash of a byte array.
    /// </summary>
    public static byte[] Digest(string algo, byte[] b) =>
        Digest(DigestUtilities.GetDigest(algo), b, 0, b.Length);

    /// <summary>
    ///     Computes the hash of a byte array segment using a digest instance.
    /// </summary>
    public static byte[] Digest(IDigest d, byte[] b, int offset, int len)
    {
        d.BlockUpdate(b, offset, len);
        var result = new byte[d.GetDigestSize()];
        d.DoFinal(result, 0);
        return result;
    }

    /// <summary>
    ///     Computes the hash of a byte array using a digest instance.
    /// </summary>
    public static byte[] Digest(IDigest d, byte[] b) =>
        Digest(d, b, 0, b.Length);

    /// <summary>
    ///     Returns the human-readable digest name for the given OID.
    /// </summary>
    /// <param name="oid">the digest OID</param>
    /// <returns>the digest name, or the OID itself if unknown</returns>
    public static string GetDigest(string oid) =>
        _digestNames.TryGetValue(oid, out var name) ? name : oid;

    /// <summary>
    ///     Returns the OID of a digest algorithm that is allowed in PDF,
    ///     or null if the algorithm is not allowed.
    /// </summary>
    /// <param name="name">the digest algorithm name</param>
    /// <returns>the OID, or null</returns>
    public static string GetAllowedDigests(string name)
    {
        _allowedDigests.TryGetValue(name, out var oid);
        return oid;
    }
}
