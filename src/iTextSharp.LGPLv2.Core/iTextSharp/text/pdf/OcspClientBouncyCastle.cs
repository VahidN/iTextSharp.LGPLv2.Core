using System.Net;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.Oiw;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
///     OcspClient implementation using BouncyCastle.
///     @author psoares
///     @since	2.1.6
/// </summary>
public class OcspClientBouncyCastle : IOcspClient
{
    /// <summary>
    ///     check certificate
    /// </summary>
    private readonly X509Certificate _checkCert;

    /// <summary>
    ///     root certificate
    /// </summary>
    private readonly X509Certificate _rootCert;

    /// <summary>
    ///     OCSP URL
    /// </summary>
    private readonly string _url;

    /// <summary>
    ///     Creates an instance of an OcspClient that will be using BouncyCastle.
    /// </summary>
    /// <param name="checkCert">check certificate</param>
    /// <param name="rootCert">root certificate</param>
    /// <param name="url">OCSP URL</param>
    public OcspClientBouncyCastle(X509Certificate checkCert, X509Certificate rootCert, string url)
    {
        _checkCert = checkCert;
        _rootCert = rootCert;
        _url = url;
    }

    /// <summary>
    ///     @see com.lowagie.text.pdf.OcspClient#getEncoded()
    /// </summary>
    /// <returns>	a byte array</returns>
    public byte[] GetEncoded()
    {
        var request = generateOcspRequest(_rootCert, _checkCert.SerialNumber);
        var array = request.GetEncoded();
        var con = (HttpWebRequest)WebRequest.Create(_url);
        con.ContentType = "application/ocsp-request";
        con.Accept = "application/ocsp-response";
        con.Method = "POST";
#if NET40
            using var outp = con.GetRequestStream();
#else
        using var outp = con.GetRequestStreamAsync().Result;
#endif
        outp.Write(array, 0, array.Length);

#if NET40
            HttpWebResponse response = (HttpWebResponse)con.GetResponse();
#else
        using var response = (HttpWebResponse)con.GetResponseAsync().GetAwaiter().GetResult();
#endif

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new IOException($"Invalid HTTP response: {(int)response.StatusCode}");
        }

        using var inp = response.GetResponseStream();
        var ocspResponse = new OcspResp(inp);
#if NET40
            response.Close();
#endif

        if (ocspResponse.Status != 0)
        {
            throw new IOException("Invalid status: " + ocspResponse.Status);
        }

        var basicResponse = (BasicOcspResp)ocspResponse.GetResponseObject();

        if (basicResponse != null)
        {
            var responses = basicResponse.Responses;

            if (responses.Length == 1)
            {
                var resp = responses[0];
                var status = resp.GetCertStatus();

                if (status == CertificateStatus.Good)
                {
                    return basicResponse.GetEncoded();
                }

                if (status is RevokedStatus)
                {
                    throw new IOException("OCSP Status is revoked!");
                }

                throw new IOException("OCSP Status is unknown!");
            }
        }

        return null;
    }

    /// <summary>
    ///     Generates an OCSP request using BouncyCastle.
    ///     @throws OCSPException
    ///     @throws IOException
    /// </summary>
    /// <param name="issuerCert">certificate of the issues</param>
    /// <param name="serialNumber">serial number</param>
    /// <returns>OCSP request</returns>
    private static OcspReq generateOcspRequest(X509Certificate issuerCert, BigInteger serialNumber)
    {
        // Generate the id for the certificate we are looking for
        var id = new CertificateID(
            new AlgorithmIdentifier(new DerObjectIdentifier(OiwObjectIdentifiers.IdSha1.Id), DerNull.Instance),
            issuerCert, serialNumber);

        // basic request generation with nonce
        var gen = new OcspReqGenerator();
        gen.AddRequest(id);

        // create details for nonce extension
        var oids = new List<DerObjectIdentifier>();
        var values = new List<X509Extension>();

        oids.Add(OcspObjectIdentifiers.PkixOcspNonce);

        values.Add(new X509Extension(false,
            new DerOctetString(new DerOctetString(PdfEncryption.CreateDocumentId()).GetEncoded())));

        gen.SetRequestExtensions(new X509Extensions(oids, values));

        return gen.Generate();
    }
}