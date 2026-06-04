namespace iTextSharp.text.pdf.security;

using System.util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

/// <summary>
///     Provides static methods to sign PDF documents.
///     This is the high-level API for creating digital signatures,
///     equivalent to <c>MakeSignature</c> in iTextSharp 5.x.
///     @since    3.7.12
/// </summary>
public static class MakeSignature
{
    /// <summary>
    ///     Processes a CRL list and returns the CRL bytes.
    /// </summary>
    /// <param name="cert">a certificate, used if a CRL client needs to derive the CRL URL</param>
    /// <param name="crlList">a collection of <see cref="ICrlClient"/> implementations</param>
    /// <returns>a collection of CRL byte arrays, or null if empty</returns>
    public static ICollection<byte[]> ProcessCrl(X509Certificate cert, ICollection<ICrlClient> crlList)
    {
        if (crlList == null)
        {
            return null;
        }

        var crlBytes = new List<byte[]>();
        foreach (var cc in crlList)
        {
            if (cc == null)
            {
                continue;
            }

            var b = cc.GetEncoded(cert, null);
            if (b == null)
            {
                continue;
            }

            crlBytes.AddRange(b);
        }

        return crlBytes.Count == 0 ? null : crlBytes;
    }

    /// <summary>
    ///     Signs the document using detached mode (CMS).
    ///     The actual signing is performed by an external signature implementation
    ///     (e.g. smart card, HSM, or remote signing service).
    /// </summary>
    /// <param name="sap">the <see cref="PdfSignatureAppearance"/></param>
    /// <param name="externalSignature">the interface providing the actual signing</param>
    /// <param name="chain">the certificate chain</param>
    /// <param name="crlList">a collection of <see cref="ICrlClient"/> implementations, or null</param>
    /// <param name="ocspClient">the OCSP client, or null</param>
    /// <param name="tsaClient">the timestamp client, or null</param>
    /// <param name="estimatedSize">the reserved size for the signature; estimated if 0</param>
    /// <param name="sigtype">the signature standard (CMS or CADES)</param>
    public static void SignDetached(
        PdfSignatureAppearance sap,
        IExternalSignature externalSignature,
        ICollection<X509Certificate> chain,
        ICollection<ICrlClient> crlList,
        IOcspClient ocspClient,
        ITsaClient tsaClient,
        int estimatedSize,
        CryptoStandard sigtype)
    {
        var certList = new List<X509Certificate>(chain);
        ICollection<byte[]> crlBytes = null;
        var i = 0;
        while (crlBytes == null && i < certList.Count)
        {
            crlBytes = ProcessCrl(certList[i++], crlList);
        }

        if (estimatedSize == 0)
        {
            estimatedSize = 8192;
            if (crlBytes != null)
            {
                foreach (var element in crlBytes)
                {
                    estimatedSize += element.Length + 10;
                }
            }

            if (ocspClient != null)
            {
                estimatedSize += 4192;
            }

            if (tsaClient != null)
            {
                estimatedSize += 4192;
            }
        }

        sap.CertChain = certList.ToArray();

        if (sigtype == CryptoStandard.CADES)
        {
            sap.AddDeveloperExtension(PdfDeveloperExtension.Esic17Extensionlevel2);
        }

        var dic = new PdfSignature(
            PdfName.AdobePpklite,
            sigtype == CryptoStandard.CADES
                ? PdfName.EtsiCadesDetached
                : PdfName.AdbePkcs7Detached);
        dic.Reason = sap.Reason;
        dic.Location = sap.Location;
        dic.Contact = sap.Contact;
        dic.Date = new PdfDate(sap.SignDate);
        sap.CryptoDictionary = dic;

        var exc = new NullValueDictionary<PdfName, int>();
        exc[PdfName.Contents] = (estimatedSize * 2) + 2;
        sap.PreClose(exc);

        var hashAlgorithm = externalSignature.GetHashAlgorithm();
        var sgn = new PdfPkcs7(chain, hashAlgorithm, false);
        var messageDigest = DigestUtilities.GetDigest(hashAlgorithm);
        var data = sap.RangeStream;
        var hash = DigestAlgorithms.Digest(data, messageDigest);

        byte[] ocsp = null;
        if (chain.Count >= 2 && ocspClient != null)
        {
            ocsp = ocspClient.GetEncoded(certList[0], certList[1], null);
        }

        var sh = sgn.GetAuthenticatedAttributeBytes(hash, ocsp, crlBytes, sigtype);
        var extSignature = externalSignature.Sign(sh);
        sgn.SetExternalDigest(extSignature, null, externalSignature.GetEncryptionAlgorithm());

        var encodedSig = sgn.GetEncodedPkcs7(hash, tsaClient, ocsp, crlBytes, sigtype);

        if (estimatedSize < encodedSig.Length)
        {
            throw new IOException("Not enough space");
        }

        var paddedSig = new byte[estimatedSize];
        Array.Copy(encodedSig, 0, paddedSig, 0, encodedSig.Length);

        var dic2 = new PdfDictionary();
        dic2.Put(PdfName.Contents, new PdfString(paddedSig).SetHexWriting(true));
        sap.Close(dic2);
    }

    /// <summary>
    ///     Signs the document using an external container.
    ///     The signature is fully composed externally; only the final bytes
    ///     are injected into the PDF.
    /// </summary>
    /// <param name="sap">the <see cref="PdfSignatureAppearance"/></param>
    /// <param name="externalSignatureContainer">the interface providing the actual signing</param>
    /// <param name="estimatedSize">the reserved size for the signature</param>
    public static void SignExternalContainer(
        PdfSignatureAppearance sap,
        IExternalSignatureContainer externalSignatureContainer,
        int estimatedSize)
    {
        var dic = new PdfSignature(null, null);
        dic.Reason = sap.Reason;
        dic.Location = sap.Location;
        dic.Contact = sap.Contact;
        dic.Date = new PdfDate(sap.SignDate);
        externalSignatureContainer.ModifySigningDictionary(dic);
        sap.CryptoDictionary = dic;

        var exc = new NullValueDictionary<PdfName, int>();
        exc[PdfName.Contents] = (estimatedSize * 2) + 2;
        sap.PreClose(exc);

        var data = sap.RangeStream;
        var encodedSig = externalSignatureContainer.Sign(data);

        if (estimatedSize < encodedSig.Length)
        {
            throw new IOException("Not enough space");
        }

        var paddedSig = new byte[estimatedSize];
        Array.Copy(encodedSig, 0, paddedSig, 0, encodedSig.Length);

        var dic2 = new PdfDictionary();
        dic2.Put(PdfName.Contents, new PdfString(paddedSig).SetHexWriting(true));
        sap.Close(dic2);
    }

    /// <summary>
    ///     Signs a PDF where space was already reserved (deferred signing).
    ///     The field must be the last signature field in the document.
    /// </summary>
    /// <param name="reader">the original PDF</param>
    /// <param name="fieldName">the field to sign; must be the last field</param>
    /// <param name="outs">the output stream</param>
    /// <param name="externalSignatureContainer">the signature container producing the signature bytes</param>
    public static void SignDeferred(
        PdfReader reader,
        string fieldName,
        Stream outs,
        IExternalSignatureContainer externalSignatureContainer)
    {
        var af = reader.AcroFields;
        var v = af.GetSignatureDictionary(fieldName);
        if (v == null)
        {
            throw new DocumentException("No field");
        }

        if (!af.SignatureCoversWholeDocument(fieldName))
        {
            throw new DocumentException("Not the last signature");
        }

        var b = v.GetAsArray(PdfName.Byterange);
        var gaps = new long[b.Size];
        for (var k = 0; k < b.Size; ++k)
        {
            gaps[k] = b.GetAsNumber(k).LongValue;
        }

        if (b.Size != 4 || gaps[0] != 0)
        {
            throw new DocumentException("Single exclusion space supported");
        }

        var rf = reader.SafeFile;
        rf.ReOpen();

        try
        {
            // Copy up to the gap start
            CopyBytesFromFile(rf, 0, gaps[1] + 1, outs);

            // Let the external container sign the range stream
            var rangeStream = new SignedRangeStream(rf, gaps);
            var signedContent = externalSignatureContainer.Sign(rangeStream);

            var spaceAvailable = (int)(gaps[2] - gaps[1]) - 2;
            if ((spaceAvailable & 1) != 0)
            {
                throw new DocumentException("Gap is not a multiple of 2");
            }

            spaceAvailable /= 2;
            if (spaceAvailable < signedContent.Length)
            {
                throw new DocumentException("Not enough space");
            }

            var bb = new ByteBuffer(spaceAvailable * 2);
            foreach (var bi in signedContent)
            {
                bb.AppendHex(bi);
            }

            var remain = (spaceAvailable - signedContent.Length) * 2;
            for (var k = 0; k < remain; ++k)
            {
                bb.Append((byte)48);
            }

            bb.WriteTo(outs);

            // Copy from after the gap to the end
            CopyBytesFromFile(rf, gaps[2] - 1, gaps[3] + 1, outs);
        }
        finally
        {
            try
            {
                rf.Close();
            }
            catch
            {
                // Ignore close errors
            }
        }
    }

    private static void CopyBytesFromFile(RandomAccessFileOrArray rf, long offset, long length, Stream outs)
    {
        rf.Seek(offset);
        var buf = new byte[8192];
        var remaining = length;
        while (remaining > 0)
        {
            var toRead = (int)Math.Min(buf.Length, remaining);
            var read = rf.Read(buf, 0, toRead);
            if (read <= 0)
            {
                break;
            }

            outs.Write(buf, 0, read);
            remaining -= read;
        }
    }

    /// <summary>
    ///     A stream that reads from a file using byte range intervals,
    ///     skipping over the signature gap.
    /// </summary>
    private sealed class SignedRangeStream : Stream
    {
        private readonly RandomAccessFileOrArray _rf;
        private readonly long[] _gaps;
        private long _position;
        private int _gapIndex;

        public SignedRangeStream(RandomAccessFileOrArray rf, long[] gaps)
        {
            _rf = rf;
            _gaps = gaps;
            _position = gaps[0];
            _gapIndex = 0;
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => (_gaps[1] - _gaps[0]) + (_gaps[3] - _gaps[2]);

        public override long Position
        {
            get => _position;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_gapIndex >= _gaps.Length)
            {
                return 0;
            }

            var remaining = _gaps[_gapIndex + 1] - _position;
            if (remaining <= 0)
            {
                _gapIndex += 2;
                if (_gapIndex >= _gaps.Length)
                {
                    return 0;
                }

                _position = _gaps[_gapIndex];
                remaining = _gaps[_gapIndex + 1] - _position;
            }

            var toRead = (int)Math.Min(count, remaining);
            _rf.Seek(_position);
            var read = _rf.Read(buffer, offset, toRead);
            if (read > 0)
            {
                _position += read;
            }

            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
