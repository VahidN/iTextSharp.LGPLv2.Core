using System;
using System.IO;
using System.Linq;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.CryptoUtils;

/// <summary>
///     A Personal Information Exchange File Reader
/// </summary>
public static class PfxReader
{
    /// <summary>
    ///     Reads A Personal Information Exchange File.
    /// </summary>
    /// <param name="pfxPath">Certificate file's path</param>
    /// <param name="pfxPassword">Certificate file's password</param>
    public static PfxData ReadCertificate(string pfxPath, string pfxPassword)
    {
        using var stream = new FileStream(pfxPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var pkcs12Store = new Pkcs12StoreBuilder().Build();
        pkcs12Store.Load(stream, pfxPassword.ToCharArray());
        var alias = FindThePublicKey(pkcs12Store);
        var asymmetricKeyParameter = pkcs12Store.GetKey(alias).Key;
        var chain = ConstructChain(pkcs12Store, alias);
        return new PfxData { X509PrivateKeys = chain, PublicKey = asymmetricKeyParameter };
    }

    private static X509Certificate[] ConstructChain(Pkcs12Store pkcs12Store, string alias)
    {
        var certificateChains = pkcs12Store.GetCertificateChain(alias);
        var chain = new X509Certificate[certificateChains.Length];

        for (var k = 0; k < certificateChains.Length; ++k)
        {
            chain[k] = certificateChains[k].Certificate;
        }

        return chain;
    }

    private static string FindThePublicKey(Pkcs12Store pkcs12Store)
    {
        var alias = string.Empty;
        foreach (var entry in pkcs12Store.Aliases.Where(entry => pkcs12Store.IsKeyEntry(entry) &&
                                                                 pkcs12Store.GetKey(entry).Key.IsPrivate))
        {
            alias = entry;
            break;
        }

        if (string.IsNullOrEmpty(alias))
        {
            throw new InvalidOperationException("Provided certificate is invalid.");
        }

        return alias;
    }
}