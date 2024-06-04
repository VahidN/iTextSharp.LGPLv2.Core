using iTextSharp.LGPLv2.Core.System.Encodings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests;

/// <summary>
///     .NET Core doesn't support all of the encoding types.
/// </summary>
[TestClass]
public class EncodingTests
{
    [TestMethod]
    public void Test1250IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1250);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }

    [TestMethod]
    public void Test1251IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1251);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }

    [TestMethod]
    public void Test1252IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1252);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }

    [TestMethod]
    public void Test1253IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1253);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }

    [TestMethod]
    public void Test1254IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1254);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }

    [TestMethod]
    public void Test1255IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1255);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }

    [TestMethod]
    public void Test1256IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1256);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }

    [TestMethod]
    public void Test1257IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1257);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }

    [TestMethod]
    public void Test1258IsAvailable()
    {
        var encoding = EncodingsRegistry.GetEncoding(1258);
        var bytes = encoding.GetBytes("Text");
        Assert.IsNotNull(bytes);
    }
}