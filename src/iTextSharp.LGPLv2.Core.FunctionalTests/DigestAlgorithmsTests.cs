using System;
using System.IO;
using System.Text;
using iTextSharp.text.pdf.security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

[TestClass]
public class DigestAlgorithmsTests
{
    [TestMethod]
    public void Digest_Stream_Sha256_Empty()
    {
        using var ms = new MemoryStream();
        byte[] hash = DigestAlgorithms.Digest(ms, DigestAlgorithms.Sha256);
        Assert.HasCount(32, hash);
        // SHA-256 of empty data
        byte[] expected = HexToBytes("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
        CollectionAssert.AreEqual(expected, hash);
    }

    [TestMethod]
    public void Digest_Stream_Sha1_HelloWorld()
    {
        byte[] data = Encoding.UTF8.GetBytes("Hello World");
        using var ms = new MemoryStream(data);
        byte[] hash = DigestAlgorithms.Digest(ms, DigestAlgorithms.Sha1);
        byte[] expected = HexToBytes("0a4d55a8d778e5022fab701977c5d840bbc486d0");
        CollectionAssert.AreEqual(expected, hash);
    }

    [TestMethod]
    public void Digest_Stream_Sha256_HelloWorld()
    {
        byte[] data = Encoding.UTF8.GetBytes("Hello World");
        using var ms = new MemoryStream(data);
        byte[] hash = DigestAlgorithms.Digest(ms, DigestAlgorithms.Sha256);
        byte[] expected = HexToBytes("a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e");
        CollectionAssert.AreEqual(expected, hash);
    }

    [TestMethod]
    public void Digest_ByteArray_Sha512()
    {
        byte[] data = Encoding.UTF8.GetBytes("Test");
        byte[] hash = DigestAlgorithms.Digest(DigestAlgorithms.Sha512, data);
        Assert.HasCount(64, hash);
    }

    [TestMethod]
    public void GetMessageDigest_Sha256_ReturnsNonNull()
    {
        var digest = DigestAlgorithms.GetMessageDigest(DigestAlgorithms.Sha256);
        Assert.IsNotNull(digest);
        Assert.AreEqual(32, digest.GetDigestSize());
    }

    [TestMethod]
    public void GetDigest_KnownOid_ReturnsSha1()
    {
        string name = DigestAlgorithms.GetDigest("1.3.14.3.2.26");
        Assert.AreEqual("SHA1", name);
    }

    [TestMethod]
    public void GetDigest_UnknownOid_ReturnsOid()
    {
        string oid = "1.2.3.4.5.6.7.8.9";
        string name = DigestAlgorithms.GetDigest(oid);
        Assert.AreEqual(oid, name);
    }

    [TestMethod]
    public void GetAllowedDigests_Sha256_ReturnsOid()
    {
        string oid = DigestAlgorithms.GetAllowedDigests("SHA256");
        Assert.AreEqual("2.16.840.1.101.3.4.2.1", oid);
    }

    [TestMethod]
    public void GetAllowedDigests_Sha256_CaseInsensitive()
    {
        string oid = DigestAlgorithms.GetAllowedDigests("sha-256");
        Assert.AreEqual("2.16.840.1.101.3.4.2.1", oid);
    }

    [TestMethod]
    public void GetAllowedDigests_NotAllowed_ReturnsNull()
    {
        string oid = DigestAlgorithms.GetAllowedDigests("BLAKE2b");
        Assert.IsNull(oid);
    }

    private static byte[] HexToBytes(string hex)
    {
        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        return bytes;
    }
}
