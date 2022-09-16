using System.IO;
using System.Collections;
using System.Net;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace iTextSharp.text.pdf
{

    /// <summary>
    /// OcspClient implementation using BouncyCastle.
    /// @author psoares
    /// @since	2.1.6
    /// </summary>
    public class OcspClientBouncyCastle : IOcspClient
    {
        /// <summary>
        /// check certificate
        /// </summary>
        private readonly X509Certificate _checkCert;

        /// <summary>
        /// root certificate
        /// </summary>
        private readonly X509Certificate _rootCert;
        /// <summary>
        /// OCSP URL
        /// </summary>
        private readonly string _url;

        /// <summary>
        /// Creates an instance of an OcspClient that will be using BouncyCastle.
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
        /// @see com.lowagie.text.pdf.OcspClient#getEncoded()
        /// </summary>
        /// <returns>	a byte array</returns>
        public byte[] GetEncoded()
        {
            OcspReq request = generateOcspRequest(_rootCert, _checkCert.SerialNumber);
            byte[] array = request.GetEncoded();
            ByteArrayContent content = new ByteArrayContent(array);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/ocsp-request");
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/ocsp-response"));

            HttpResponseMessage response = client.PostAsync(_url, content).Result;
            content.Dispose();

            if (response.StatusCode != HttpStatusCode.OK)
                throw new IOException($"Invalid HTTP response: {(int)response.StatusCode}");
            Stream inp = response.Content.ReadAsStreamAsync().Result;
            OcspResp ocspResponse = new OcspResp(inp);
            inp.Dispose();
            response.Dispose();

            if (ocspResponse.Status != 0)
                throw new IOException("Invalid status: " + ocspResponse.Status);
            BasicOcspResp basicResponse = (BasicOcspResp)ocspResponse.GetResponseObject();
            if (basicResponse != null)
            {
                SingleResp[] responses = basicResponse.Responses;
                if (responses.Length == 1)
                {
                    SingleResp resp = responses[0];
                    object status = resp.GetCertStatus();
                    if (status == CertificateStatus.Good)
                    {
                        return basicResponse.GetEncoded();
                    }
                    else if (status is RevokedStatus)
                    {
                        throw new IOException("OCSP Status is revoked!");
                    }
                    else
                    {
                        throw new IOException("OCSP Status is unknown!");
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Generates an OCSP request using BouncyCastle.
        /// @throws OCSPException
        /// @throws IOException
        /// </summary>
        /// <param name="issuerCert">certificate of the issues</param>
        /// <param name="serialNumber">serial number</param>
        /// <returns>OCSP request</returns>
        private static OcspReq generateOcspRequest(X509Certificate issuerCert, BigInteger serialNumber)
        {
            // Generate the id for the certificate we are looking for
            CertificateID id = new CertificateID(CertificateID.HashSha1, issuerCert, serialNumber);

            // basic request generation with nonce
            OcspReqGenerator gen = new OcspReqGenerator();

            gen.AddRequest(id);

            // create details for nonce extension
            var oids = new List<object>();
            var values = new List<object>();

            oids.Add(OcspObjectIdentifiers.PkixOcspNonce);
            values.Add(new X509Extension(false, new DerOctetString(new DerOctetString(PdfEncryption.CreateDocumentId()).GetEncoded())));

            gen.SetRequestExtensions(new X509Extensions(oids, values));

            return gen.Generate();
        }
    }
}