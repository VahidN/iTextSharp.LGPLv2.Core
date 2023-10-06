using System.Net;
using System.Text;
using System.util;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tsp;

namespace iTextSharp.text.pdf;

/// <summary>
///     Time Stamp Authority Client interface implementation using Bouncy Castle
///     org.bouncycastle.tsp package.
///     Created by Aiken Sam, 2006-11-15, refactored by Martin Brunecky, 07/15/2007
///     for ease of subclassing.
///     @since	2.1.6
/// </summary>
public class TsaClientBouncyCastle : ITsaClient
{
    /// <summary>
    ///     Estimate of the received time stamp token
    /// </summary>
    protected int TokSzEstimate;

    /// <summary>
    ///     TSA password
    /// </summary>
    protected string TsaPassword;

    /// <summary>
    ///     URL of the Time Stamp Authority
    /// </summary>
    protected string TsaUrl;

    /// <summary>
    ///     TSA Username
    /// </summary>
    protected string TsaUsername;

    /// <summary>
    ///     Creates an instance of a TSAClient that will use BouncyCastle.
    /// </summary>
    /// <param name="url">String - Time Stamp Authority URL (i.e. "http://tsatest1.digistamp.com/TSA")</param>
    public TsaClientBouncyCastle(string url) : this(url, null, null, 4096)
    {
    }

    /// <summary>
    ///     Creates an instance of a TSAClient that will use BouncyCastle.
    /// </summary>
    /// <param name="url">String - Time Stamp Authority URL (i.e. "http://tsatest1.digistamp.com/TSA")</param>
    /// <param name="username">String - user(account) name</param>
    /// <param name="password">String - password</param>
    public TsaClientBouncyCastle(string url, string username, string password) : this(url, username, password, 4096)
    {
    }

    /// <summary>
    ///     Constructor.
    ///     Note the token size estimate is updated by each call, as the token
    ///     size is not likely to change (as long as we call the same TSA using
    ///     the same imprint length).
    /// </summary>
    /// <param name="url">String - Time Stamp Authority URL (i.e. "http://tsatest1.digistamp.com/TSA")</param>
    /// <param name="username">String - user(account) name</param>
    /// <param name="password">String - password</param>
    /// <param name="tokSzEstimate">int - estimated size of received time stamp token (DER encoded)</param>
    public TsaClientBouncyCastle(string url, string username, string password, int tokSzEstimate)
    {
        TsaUrl = url;
        TsaUsername = username;
        TsaPassword = password;
        TokSzEstimate = tokSzEstimate;
    }

    /// <summary>
    ///     Get RFC 3161 timeStampToken.
    ///     Method may return null indicating that timestamp should be skipped.
    ///     @throws Exception - TSA request failed
    ///     @see com.lowagie.text.pdf.TSAClient#getTimeStampToken(com.lowagie.text.pdf.PdfPKCS7, byte[])
    /// </summary>
    /// <param name="caller">PdfPKCS7 - calling PdfPKCS7 instance (in case caller needs it)</param>
    /// <param name="imprint">byte[] - data imprint to be time-stamped</param>
    /// <returns>byte[] - encoded, TSA signed data of the timeStampToken</returns>
    public byte[] GetTimeStampToken(PdfPkcs7 caller, byte[] imprint) => GetTimeStampToken(imprint);

    /// <summary>
    ///     Get the token size estimate.
    ///     Returned value reflects the result of the last succesfull call, padded
    /// </summary>
    /// <returns>an estimate of the token size</returns>
    public int GetTokenSizeEstimate() => TokSzEstimate;

    /// <summary>
    ///     Get timestamp token - Bouncy Castle request encoding / decoding layer
    /// </summary>
    protected internal byte[] GetTimeStampToken(byte[] imprint)
    {
        byte[] respBytes = null;
        // Setup the time stamp request
        var tsqGenerator = new TimeStampRequestGenerator();
        tsqGenerator.SetCertReq(true);
        // tsqGenerator.setReqPolicy("1.3.6.1.4.1.601.10.3.1");
        var nonce = BigInteger.ValueOf(DateTime.Now.Ticks + Environment.TickCount);
        var request = tsqGenerator.Generate(X509ObjectIdentifiers.IdSha1.Id, imprint, nonce);
        var requestBytes = request.GetEncoded();

        // Call the communications layer
        respBytes = GetTsaResponse(requestBytes);

        // Handle the TSA response
        var response = new TimeStampResponse(respBytes);

        // validate communication level attributes (RFC 3161 PKIStatus)
        response.Validate(request);
        var failure = response.GetFailInfo();
        var value = failure == null ? 0 : failure.IntValue;
        if (value != 0)
        {
            // @todo: Translate value of 15 error codes defined by PKIFailureInfo to string
            throw new InvalidOperationException($"Invalid TSA \'{TsaUrl}\' response, code {value}");
        }
        // @todo: validate the time stap certificate chain (if we want
        //        assure we do not sign using an invalid timestamp).

        // extract just the time stamp token (removes communication status info)
        var tsToken = response.TimeStampToken;
        if (tsToken == null)
        {
            throw new
                InvalidOperationException($"TSA \'{TsaUrl}\' failed to return time stamp token: {response.GetStatusString()}");
        }

        var info = tsToken.TimeStampInfo; // to view details
        var encoded = tsToken.GetEncoded();

        // Update our token size estimate for the next call (padded to be safe)
        TokSzEstimate = encoded.Length + 32;
        return encoded;
    }

    /// <summary>
    ///     Get timestamp token - communications layer
    /// </summary>
    /// <returns>- byte[] - TSA response, raw bytes (RFC 3161 encoded)</returns>
    protected internal virtual byte[] GetTsaResponse(byte[] requestBytes)
    {
        if (requestBytes == null)
        {
            throw new ArgumentNullException(nameof(requestBytes));
        }

        var con = (HttpWebRequest)WebRequest.Create(TsaUrl);
        con.ContentType = "application/timestamp-query";
        con.Method = "POST";
        if (!string.IsNullOrEmpty(TsaUsername))
        {
            var authInfo = TsaUsername + ":" + TsaPassword;
            authInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(authInfo));
            con.Headers["Authorization"] = "Basic " + authInfo;
        }

#if NET40
            using var outp = con.GetRequestStream();
#else
        using var outp = con.GetRequestStreamAsync().Result;
#endif
        outp.Write(requestBytes, 0, requestBytes.Length);

#if NET40
            HttpWebResponse response = (HttpWebResponse)con.GetResponse();
#else
        using var response = (HttpWebResponse)con.GetResponseAsync().GetAwaiter().GetResult();
#endif
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new IOException("Invalid HTTP response: " + (int)response.StatusCode);
        }

        using var inp = response.GetResponseStream();
        var encoding = response.Headers["Content-Encoding"];

        using var baos = new MemoryStream();
        var buffer = new byte[1024];
        var bytesRead = 0;
        while ((bytesRead = inp.Read(buffer, 0, buffer.Length)) > 0)
        {
            baos.Write(buffer, 0, bytesRead);
        }

#if NET40
            response.Close();
#endif

        var respBytes = baos.ToArray();

        if (encoding != null && Util.EqualsIgnoreCase(encoding, "base64"))
        {
            respBytes = Convert.FromBase64String(Encoding.ASCII.GetString(respBytes));
        }

        return respBytes;
    }
}