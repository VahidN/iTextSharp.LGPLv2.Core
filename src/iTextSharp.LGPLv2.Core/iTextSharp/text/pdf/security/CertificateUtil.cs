using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf.security;

/// <summary>
///     Utility class to retrieve information from certificates.
/// </summary>
public static class CertificateUtil
{
    private const string IdAdTimestamping = "1.3.6.1.5.5.7.48.0.3";

    /// <summary>
    ///     Gets the CRL distribution point URL from a certificate, if available.
    /// </summary>
    /// <param name="certificate">the BouncyCastle certificate to inspect</param>
    /// <returns>the CRL URL, or null</returns>
    public static string GetCrlUrl(X509Certificate certificate)
    {
        try
        {
            var extension = certificate.GetExtensionValue(X509Extensions.CrlDistributionPoints);
            if (extension is null)
            {
                return null;
            }

            var crlDistPoint = CrlDistPoint.GetInstance(
                Asn1Object.FromByteArray(extension.GetOctets()));
            if (crlDistPoint is null)
            {
                return null;
            }

            foreach (var dp in crlDistPoint.GetDistributionPoints())
            {
                var name = dp.DistributionPointName;
                if (name != null && name.Type == DistributionPointName.FullName)
                {
                    var generalNames = GeneralNames.GetInstance(name.Name);
                    foreach (var gn in generalNames.GetNames())
                    {
                        if (gn.TagNo == GeneralName.UniformResourceIdentifier)
                        {
                            return ((DerIA5String)gn.Name).GetString();
                        }
                    }
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     Gets the OCSP responder URL from a certificate's Authority Information Access
    ///     extension, if available.
    /// </summary>
    /// <param name="certificate">the BouncyCastle certificate to inspect</param>
    /// <returns>the OCSP URL, or null</returns>
    public static string GetOcspUrl(X509Certificate certificate)
    {
        try
        {
            return GetAccessDescriptionUrl(certificate, X509ObjectIdentifiers.OcspAccessMethod);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     Gets the Time-Stamp Authority URL from a certificate's Authority Information Access
    ///     extension, if available.
    /// </summary>
    /// <param name="certificate">the BouncyCastle certificate to inspect</param>
    /// <returns>the TSA URL, or null</returns>
    public static string GetTsaUrl(X509Certificate certificate)
    {
        try
        {
            return GetAccessDescriptionUrl(certificate, new DerObjectIdentifier(IdAdTimestamping));
        }
        catch
        {
            return null;
        }
    }

    private static string GetAccessDescriptionUrl(
        X509Certificate certificate,
        DerObjectIdentifier accessMethod)
    {
        var extension = certificate.GetExtensionValue(X509Extensions.AuthorityInfoAccess);
        if (extension is null)
        {
            return null;
        }

        var authInfoAccesss = AuthorityInformationAccess.GetInstance(
            Asn1Object.FromByteArray(extension.GetOctets()));
        if (authInfoAccesss is null)
        {
            return null;
        }

        foreach (var ad in authInfoAccesss.GetAccessDescriptions())
        {
            if (ad.AccessMethod.Equals(accessMethod) &&
                ad.AccessLocation.TagNo == GeneralName.UniformResourceIdentifier)
            {
                return ((DerIA5String)ad.AccessLocation.Name).GetString();
            }
        }

        return null;
    }
}