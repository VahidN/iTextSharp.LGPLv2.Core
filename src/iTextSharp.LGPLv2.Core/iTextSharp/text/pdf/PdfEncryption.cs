using System.Security.Cryptography;
using System.Text;
using System.util;
using iTextSharp.text.pdf.crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf;

/// <summary>
///     @author  Paulo Soares (psoares@consiste.pt) and
///     some of it has been ported from https://github.com/LibrePDF/OpenPDF/pull/802
/// </summary>
public class PdfEncryption
{
    public const int AES_128 = 4;
    public const int STANDARD_ENCRYPTION_128 = 3;
    public const int AES_256_V3 = 6;
    public const int STANDARD_ENCRYPTION_40 = 2;
    internal static readonly byte[] MetadataPad = { 255, 255, 255, 255 };

    internal static long Seq = DateTime.Now.Ticks + Environment.TickCount;

    private static readonly byte[] _pad =
    {
        0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75,
        0x8A, 0x41, 0x64, 0x00, 0x4E, 0x56,
        0xFF, 0xFA, 0x01, 0x08, 0x2E, 0x2E,
        0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80,
        0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53,
        0x69, 0x7A,
    };

    private static readonly byte[] _salt = { 0x73, 0x41, 0x6c, 0x54 };
    private readonly ArcfourEncryption _rc4 = new();
    private int _cryptoMode;

    /// <summary>
    ///     Indicates if the encryption is only necessary for embedded files.
    ///     @since 2.1.3
    /// </summary>
    private bool _embeddedFilesOnly;

    private bool _encryptMetadata;

    /// <summary>
    ///     The generic key length. It may be 40 or 128.
    /// </summary>
    private int _keyLength;

    /**
     * Additional keys for AES_256_V3
     */
    private byte[] _oeKey;

    private byte[] _perms;

    private int _revision;

    private byte[] _ueKey;

    internal byte[] DocumentId;

    /// <summary>
    ///     Work area to prepare the object/generation bytes
    /// </summary>
    internal readonly byte[] Extra = new byte[5];

    /// <summary>
    ///     The encryption key for a particular object/generation
    /// </summary>
    internal byte[] Key;

    /// <summary>
    ///     The encryption key length for a particular object/generation
    /// </summary>
    internal int KeySize;

    /// <summary>
    ///     The global encryption key
    /// </summary>
    internal byte[] Mkey;

    /// <summary>
    ///     The encryption key for the owner
    /// </summary>
    internal byte[] OwnerKey = new byte[32];

    internal int Permissions;

    /// <summary>
    ///     The public key security handler for certificate encryption
    /// </summary>
    protected PdfPublicKeySecurityHandler PublicKeyHandler;

    /// <summary>
    ///     The encryption key for the user
    /// </summary>
    internal byte[] UserKey = new byte[32];

    public PdfEncryption() => PublicKeyHandler = new PdfPublicKeySecurityHandler();

    public PdfEncryption(PdfEncryption enc) : this()
    {
        if (enc == null)
        {
            throw new ArgumentNullException(nameof(enc));
        }

        KeySize = enc.KeySize;

        if (enc.Mkey != null)
        {
            Mkey = (byte[])enc.Mkey.Clone();
        }

        OwnerKey = (byte[])enc.OwnerKey.Clone();
        UserKey = (byte[])enc.UserKey.Clone();
        Permissions = enc.Permissions;
        if (enc.DocumentId != null)
        {
            DocumentId = (byte[])enc.DocumentId.Clone();
        }

        _revision = enc._revision;
        _keyLength = enc._keyLength;
        _encryptMetadata = enc._encryptMetadata;
        _embeddedFilesOnly = enc._embeddedFilesOnly;
        PublicKeyHandler = enc.PublicKeyHandler;

        if (enc._ueKey != null)
        {
            _ueKey = (byte[])enc._ueKey.Clone();
        }

        if (enc._oeKey != null)
        {
            _oeKey = (byte[])enc._oeKey.Clone();
        }

        if (enc._perms != null)
        {
            _perms = (byte[])enc._perms.Clone();
        }

        if (enc.Key != null)
        {
            Key = (byte[])enc.Key.Clone();
        }
    }

    public PdfObject FileId => CreateInfoId(DocumentId);

    public static byte[] CreateDocumentId()
    {
        var time = DateTime.Now.Ticks + Environment.TickCount;
        var mem = GC.GetTotalMemory(false);
        var s = $"{time}+{mem}+{Seq++}";
        return MD5BouncyCastle.Create().ComputeHash(Encoding.ASCII.GetBytes(s));
    }

    public static PdfObject CreateInfoId(byte[] id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id));
        }

        var buf = new ByteBuffer(90);
        buf.Append('[').Append('<');
        for (var k = 0; k < 16; ++k)
        {
            buf.AppendHex(id[k]);
        }

        buf.Append('>').Append('<');
        id = CreateDocumentId();
        for (var k = 0; k < 16; ++k)
        {
            buf.AppendHex(id[k]);
        }

        buf.Append('>').Append(']');
        return new PdfLiteral(buf.ToByteArray());
    }

    public void AddRecipient(X509Certificate cert, int permission)
    {
        DocumentId = CreateDocumentId();
        PublicKeyHandler.AddRecipient(new PdfPublicKeyRecipient(cert, permission));
    }

    public int CalculateStreamSize(int n)
    {
        if (_revision == AES_128 || _revision == AES_256_V3)
        {
            return (n & 0x7ffffff0) + 32;
        }

        return n;
    }

    public byte[] ComputeUserPassword(byte[] ownerPassword)
    {
        var userPad = ComputeOwnerKey(OwnerKey, PadPassword(ownerPassword));
        for (var i = 0; i < userPad.Length; i++)
        {
            var match = true;
            for (var j = 0; j < userPad.Length - i; j++)
            {
                if (userPad[i + j] != _pad[j])
                {
                    match = false;
                    break;
                }
            }

            if (!match)
            {
                continue;
            }

            var userPassword = new byte[i];
            Array.Copy(userPad, 0, userPassword, 0, i);
            return userPassword;
        }

        return userPad;
    }

    public byte[] DecryptByteArray(byte[] b)
    {
        if (b == null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        using var ba = new MemoryStream();
        var dec = GetDecryptor();
        var b2 = dec.Update(b, 0, b.Length);
        if (b2 != null)
        {
            ba.Write(b2, 0, b2.Length);
        }

        b2 = dec.Finish();
        if (b2 != null)
        {
            ba.Write(b2, 0, b2.Length);
        }

        return ba.ToArray();
    }

    public byte[] EncryptByteArray(byte[] b)
    {
        if (b == null)
        {
            throw new ArgumentNullException(nameof(b));
        }

        var ba = new MemoryStream();
        var os2 = GetEncryptionStream(ba);
        os2.Write(b, 0, b.Length);
        os2.Finish();
        return ba.ToArray();
    }

    public int GetCryptoMode() => _cryptoMode;

    public StandardDecryption GetDecryptor() => new(Key, 0, KeySize, _revision);

    public PdfDictionary GetEncryptionDictionary()
    {
        var dic = new PdfDictionary();

        if (PublicKeyHandler.GetRecipientsSize() > 0)
        {
            dic.Put(PdfName.Filter, PdfName.Pubsec);
            dic.Put(PdfName.R, new PdfNumber(_revision));

            var recipients = PublicKeyHandler.GetEncodedRecipients();

            if (_revision == STANDARD_ENCRYPTION_40)
            {
                dic.Put(PdfName.V, new PdfNumber(1));
                dic.Put(PdfName.Subfilter, PdfName.AdbePkcs7S4);
                dic.Put(PdfName.Recipients, recipients);
            }
            else if (_revision == STANDARD_ENCRYPTION_128 && _encryptMetadata)
            {
                dic.Put(PdfName.V, new PdfNumber(2));
                dic.Put(PdfName.LENGTH, new PdfNumber(128));
                dic.Put(PdfName.Subfilter, PdfName.AdbePkcs7S4);
                dic.Put(PdfName.Recipients, recipients);
            }
            else
            {
                dic.Put(PdfName.R, new PdfNumber(AES_128));
                dic.Put(PdfName.V, new PdfNumber(4));
                dic.Put(PdfName.Subfilter, PdfName.AdbePkcs7S5);

                var stdcf = new PdfDictionary();
                stdcf.Put(PdfName.Recipients, recipients);
                if (!_encryptMetadata)
                {
                    stdcf.Put(PdfName.Encryptmetadata, PdfBoolean.Pdffalse);
                }

                stdcf.Put(PdfName.Cfm, _revision == AES_128 ? PdfName.Aesv2 : PdfName.V2);

                var cf = new PdfDictionary();
                cf.Put(PdfName.Defaultcryptfilter, stdcf);
                dic.Put(PdfName.Cf, cf);
                if (_embeddedFilesOnly)
                {
                    dic.Put(PdfName.Eff, PdfName.Defaultcryptfilter);
                    dic.Put(PdfName.Strf, PdfName.Identity);
                    dic.Put(PdfName.Stmf, PdfName.Identity);
                }
                else
                {
                    dic.Put(PdfName.Strf, PdfName.Defaultcryptfilter);
                    dic.Put(PdfName.Stmf, PdfName.Defaultcryptfilter);
                }
            }

#if NET40
                using var sh = new SHA1CryptoServiceProvider();
                byte[] encodedRecipient = null;
                byte[] seed = PublicKeyHandler.GetSeed();
                sh.TransformBlock(seed, 0, seed.Length, seed, 0);
                for (int i = 0; i < PublicKeyHandler.GetRecipientsSize(); i++)
                {
                    encodedRecipient = PublicKeyHandler.GetEncodedRecipient(i);
                    sh.TransformBlock(encodedRecipient, 0, encodedRecipient.Length, encodedRecipient, 0);
                }
                if (!_encryptMetadata)
                    sh.TransformBlock(MetadataPad, 0, MetadataPad.Length, MetadataPad, 0);
                sh.TransformFinalBlock(seed, 0, 0);
                byte[] mdResult = sh.Hash;
#else
            byte[] mdResult;
            using (var sh = IncrementalHash.CreateHash(HashAlgorithmName.SHA1))
            {
                var seed = PublicKeyHandler.GetSeed();
                sh.AppendData(seed, 0, seed.Length);
                for (var i = 0; i < PublicKeyHandler.GetRecipientsSize(); i++)
                {
                    var encodedRecipient = PublicKeyHandler.GetEncodedRecipient(i);
                    sh.AppendData(encodedRecipient, 0, encodedRecipient.Length);
                }

                if (!_encryptMetadata)
                {
                    sh.AppendData(MetadataPad, 0, MetadataPad.Length);
                }

                mdResult = sh.GetHashAndReset();
            }
#endif

            SetupByEncryptionKey(mdResult, _keyLength);
        }
        else
        {
            dic.Put(PdfName.Filter, PdfName.Standard);
            dic.Put(PdfName.O, new PdfLiteral(PdfContentByte.EscapeString(OwnerKey)));
            dic.Put(PdfName.U, new PdfLiteral(PdfContentByte.EscapeString(UserKey)));
            dic.Put(PdfName.P, new PdfNumber(Permissions));
            dic.Put(PdfName.R, new PdfNumber(_revision));
            if (_revision == STANDARD_ENCRYPTION_40)
            {
                dic.Put(PdfName.V, new PdfNumber(1));
            }
            else if (_revision == STANDARD_ENCRYPTION_128 && _encryptMetadata)
            {
                dic.Put(PdfName.V, new PdfNumber(2));
                dic.Put(PdfName.LENGTH, new PdfNumber(128));
            }
            else if (_revision == STANDARD_ENCRYPTION_128 || _revision == AES_128)
            {
                if (!_encryptMetadata)
                {
                    dic.Put(PdfName.Encryptmetadata, PdfBoolean.Pdffalse);
                }

                dic.Put(PdfName.R, new PdfNumber(AES_128));
                dic.Put(PdfName.V, new PdfNumber(4));
                dic.Put(PdfName.LENGTH, new PdfNumber(128));
                var stdcf = new PdfDictionary();
                stdcf.Put(PdfName.LENGTH, new PdfNumber(16));
                if (_embeddedFilesOnly)
                {
                    stdcf.Put(PdfName.Authevent, PdfName.Efopen);
                    dic.Put(PdfName.Eff, PdfName.Stdcf);
                    dic.Put(PdfName.Strf, PdfName.Identity);
                    dic.Put(PdfName.Stmf, PdfName.Identity);
                }
                else
                {
                    stdcf.Put(PdfName.Authevent, PdfName.Docopen);
                    dic.Put(PdfName.Strf, PdfName.Stdcf);
                    dic.Put(PdfName.Stmf, PdfName.Stdcf);
                }

                stdcf.Put(PdfName.Cfm, _revision == AES_128 ? PdfName.Aesv2 : PdfName.V2);

                var cf = new PdfDictionary();
                cf.Put(PdfName.Stdcf, stdcf);
                dic.Put(PdfName.Cf, cf);
            }
            else if (_revision == AES_256_V3)
            {
                if (!_encryptMetadata)
                {
                    dic.Put(PdfName.Encryptmetadata, PdfBoolean.Pdffalse);
                }

                dic.Put(PdfName.V, new PdfNumber(5));
                dic.Put(PdfName.OE, new PdfLiteral(PdfContentByte.EscapeString(_oeKey)));
                dic.Put(PdfName.UE, new PdfLiteral(PdfContentByte.EscapeString(_ueKey)));
                dic.Put(PdfName.Perms, new PdfLiteral(PdfContentByte.EscapeString(_perms)));
                dic.Put(PdfName.LENGTH, new PdfNumber(256));
                var stdcf = new PdfDictionary();
                stdcf.Put(PdfName.LENGTH, new PdfNumber(32));
                if (_embeddedFilesOnly)
                {
                    stdcf.Put(PdfName.Authevent, PdfName.Efopen);
                    dic.Put(PdfName.Eff, PdfName.Stdcf);
                    dic.Put(PdfName.Strf, PdfName.Identity);
                    dic.Put(PdfName.Stmf, PdfName.Identity);
                }
                else
                {
                    stdcf.Put(PdfName.Authevent, PdfName.Docopen);
                    dic.Put(PdfName.Strf, PdfName.Stdcf);
                    dic.Put(PdfName.Stmf, PdfName.Stdcf);
                }

                stdcf.Put(PdfName.Cfm, PdfName.AESV3);
                var cf = new PdfDictionary();
                cf.Put(PdfName.Stdcf, stdcf);
                dic.Put(PdfName.Cf, cf);
            }
        }

        return dic;
    }

    public OutputStreamEncryption GetEncryptionStream(Stream os) => new(os, Key, 0, KeySize, _revision);

    /// <summary>
    ///     Indicates if only the embedded files have to be encrypted.
    ///     @since   2.1.3
    /// </summary>
    /// <returns>if true only the embedded files will be encrypted</returns>
    public bool IsEmbeddedFilesOnly() => _embeddedFilesOnly;

    public bool IsMetadataEncrypted() => _encryptMetadata;

    public void SetCryptoMode(int mode, int kl)
    {
        _cryptoMode = mode;
        _encryptMetadata = (mode & PdfWriter.DO_NOT_ENCRYPT_METADATA) != PdfWriter.DO_NOT_ENCRYPT_METADATA;
        _embeddedFilesOnly = (mode & PdfWriter.EMBEDDED_FILES_ONLY) == PdfWriter.EMBEDDED_FILES_ONLY;
        mode &= PdfWriter.ENCRYPTION_MASK;
        switch (mode)
        {
            case PdfWriter.STANDARD_ENCRYPTION_40:
                _encryptMetadata = true;
                _embeddedFilesOnly = false;
                _keyLength = 40;
                _revision = STANDARD_ENCRYPTION_40;
                break;
            case PdfWriter.STANDARD_ENCRYPTION_128:
                _embeddedFilesOnly = false;
                _keyLength = kl > 0 ? kl : 128;
                _revision = STANDARD_ENCRYPTION_128;
                break;
            case PdfWriter.ENCRYPTION_AES_128:
                _keyLength = 128;
                _revision = AES_128;
                break;
            case PdfWriter.ENCRYPTION_AES_256_V3:
                _keyLength = 256;
                KeySize = 32;
                _revision = AES_256_V3;
                break;

            default:
                throw new ArgumentException($"Not a valid encryption mode: {mode}", nameof(mode));
        }
    }

    public void SetHashKey(int number, int generation)
    {
        if (_revision >= AES_256_V3)
        {
            return;
        }

        using (var md5 = MD5BouncyCastle.Create())
        {
            md5.Initialize();
            Extra[0] = (byte)number;
            Extra[1] = (byte)(number >> 8);
            Extra[2] = (byte)(number >> 16);
            Extra[3] = (byte)generation;
            Extra[4] = (byte)(generation >> 8);
            md5.TransformBlock(Mkey, 0, Mkey.Length, Mkey, 0);
            md5.TransformBlock(Extra, 0, Extra.Length, Extra, 0);
            if (_revision == AES_128)
            {
                md5.TransformBlock(_salt, 0, _salt.Length, _salt, 0);
            }

            md5.TransformFinalBlock(Extra, 0, 0);
            Key = md5.Hash;
        }

        KeySize = Mkey.Length + 5;
        if (KeySize > 16)
        {
            KeySize = 16;
        }
    }

    /// <summary>
    ///     gets keylength and revision and uses revison to choose the initial values for permissions
    /// </summary>
    public void SetupAllKeys(byte[] userPassword, byte[] ownerPassword, int permissions)
    {
        if (ownerPassword == null || ownerPassword.Length == 0)
        {
            ownerPassword = MD5BouncyCastle.Create().ComputeHash(CreateDocumentId());
        }

        permissions |= (int)(_revision == STANDARD_ENCRYPTION_128 || _revision == AES_128 || _revision == AES_256_V3
                                 ? 0xfffff0c0
                                 : 0xffffffc0);
        permissions &= unchecked((int)0xfffffffc);
        Permissions = permissions;
        DocumentId = CreateDocumentId();

        if (_revision < AES_256_V3)
        {
            //PDF reference 3.5.2 Standard Security Handler, Algorithm 3.3-1
            //If there is no owner password, use the user password instead.
            var userPad = PadPassword(userPassword);
            var ownerPad = PadPassword(ownerPassword);
            OwnerKey = ComputeOwnerKey(userPad, ownerPad);
            SetupByUserPad(DocumentId, userPad, OwnerKey, permissions);
        }
        else
        {
            Key = IvGenerator.GetIv(32);
            KeySize = 32;
            ComputeUAndUeAlg8(userPassword);
            ComputeOAndOeAlg9(ownerPassword);
            ComputePermsAlg10(permissions);
        }
    }

    public void SetupByEncryptionKey(byte[] key, int keyLength)
    {
        Mkey = new byte[keyLength / 8];
        Array.Copy(key, 0, Mkey, 0, Mkey.Length);
    }

    /// <summary>
    /// </summary>
    public void SetupByOwnerPassword(byte[] documentId,
                                     byte[] ownerPassword,
                                     byte[] userKey,
                                     byte[] ownerKey,
                                     int permissions)
    {
        SetupByOwnerPad(documentId, PadPassword(ownerPassword), userKey, ownerKey, permissions);
    }

    public void SetupByUserPassword(byte[] documentId, byte[] userPassword, byte[] ownerKey, int permissions)
    {
        SetupByUserPad(documentId, PadPassword(userPassword), ownerKey, permissions);
    }

    /// <summary>
    /// </summary>
    private byte[] ComputeOwnerKey(byte[] userPad, byte[] ownerPad)
    {
        var ownerKey = new byte[32];
        var md5 = MD5BouncyCastle.Create();
        var digest = md5.ComputeHash(ownerPad);
        if (_revision == STANDARD_ENCRYPTION_128 || _revision == AES_128)
        {
            var mKey = new byte[_keyLength / 8];
            // only use for the input as many bit as the key consists of
            for (var k = 0; k < 50; ++k)
            {
                Array.Copy(md5.ComputeHash(digest), 0, digest, 0, mKey.Length);
            }

            Array.Copy(userPad, 0, ownerKey, 0, 32);
            for (var i = 0; i < 20; ++i)
            {
                for (var j = 0; j < mKey.Length; ++j)
                {
                    mKey[j] = (byte)(digest[j] ^ i);
                }

                _rc4.PrepareArcfourKey(mKey);
                _rc4.EncryptArcfour(ownerKey);
            }
        }
        else
        {
            _rc4.PrepareArcfourKey(digest, 0, 5);
            _rc4.EncryptArcfour(userPad, ownerKey);
        }

        return ownerKey;
    }

    private static byte[] PadPassword(byte[] userPassword)
    {
        var userPad = new byte[32];
        if (userPassword == null)
        {
            Array.Copy(_pad, 0, userPad, 0, 32);
        }
        else
        {
            Array.Copy(userPassword, 0, userPad, 0, Math.Min(userPassword.Length, 32));
            if (userPassword.Length < 32)
            {
                Array.Copy(_pad, 0, userPad, userPassword.Length, 32 - userPassword.Length);
            }
        }

        return userPad;
    }

    private void SetupByOwnerPad(byte[] documentId, byte[] ownerPad, byte[] userKey, byte[] ownerKey, int permissions)
    {
        var userPad = ComputeOwnerKey(ownerKey, ownerPad); //userPad will be set in this.ownerKey
        SetupGlobalEncryptionKey(documentId, userPad, ownerKey, permissions); //step 3
        SetupUserKey();
    }

    /// <summary>
    /// </summary>
    private void SetupByUserPad(byte[] documentId, byte[] userPad, byte[] ownerKey, int permissions)
    {
        SetupGlobalEncryptionKey(documentId, userPad, ownerKey, permissions);
        SetupUserKey();
    }

    /// <summary>
    ///     ownerKey, documentID must be setuped
    /// </summary>
    private void SetupGlobalEncryptionKey(byte[] documentId, byte[] userPad, byte[] ownerKey, int permissions)
    {
        DocumentId = documentId;
        OwnerKey = ownerKey;
        Permissions = permissions;
        // use variable keylength
        Mkey = new byte[_keyLength / 8];
        var digest = new byte[Mkey.Length];

        //fixed by ujihara in order to follow PDF refrence
        using (var md5 = MD5BouncyCastle.Create())
        {
            md5.Initialize();
            md5.TransformBlock(userPad, 0, userPad.Length, userPad, 0);
            md5.TransformBlock(ownerKey, 0, ownerKey.Length, ownerKey, 0);

            var ext = new byte[4];
            ext[0] = (byte)permissions;
            ext[1] = (byte)(permissions >> 8);
            ext[2] = (byte)(permissions >> 16);
            ext[3] = (byte)(permissions >> 24);
            md5.TransformBlock(ext, 0, 4, ext, 0);
            if (documentId != null)
            {
                md5.TransformBlock(documentId, 0, documentId.Length, documentId, 0);
            }

            if (!_encryptMetadata)
            {
                md5.TransformBlock(MetadataPad, 0, MetadataPad.Length, MetadataPad, 0);
            }

            md5.TransformFinalBlock(ext, 0, 0);

            Array.Copy(md5.Hash, 0, digest, 0, Mkey.Length);
        }


        // only use the really needed bits as input for the hash
        if (_revision == STANDARD_ENCRYPTION_128 || _revision == AES_128)
        {
            for (var k = 0; k < 50; ++k)
            {
                using var md5Hash = MD5BouncyCastle.Create();
                Array.Copy(md5Hash.ComputeHash(digest), 0, digest, 0, Mkey.Length);
            }
        }

        Array.Copy(digest, 0, Mkey, 0, Mkey.Length);
    }

    /// <summary>
    ///     mkey must be setuped
    /// </summary>
    /// <summary>
    ///     use the revision to choose the setup method
    /// </summary>
    private void SetupUserKey()
    {
        if (_revision == STANDARD_ENCRYPTION_128 || _revision == AES_128)
        {
            byte[] digest;
            using (var md5 = MD5BouncyCastle.Create())
            {
                md5.Initialize();
                md5.TransformBlock(_pad, 0, _pad.Length, _pad, 0);
                md5.TransformFinalBlock(DocumentId, 0, DocumentId.Length);
                digest = md5.Hash;
            }

            Array.Copy(digest, 0, UserKey, 0, 16);
            for (var k = 16; k < 32; ++k)
            {
                UserKey[k] = 0;
            }

            for (var i = 0; i < 20; ++i)
            {
                for (var j = 0; j < Mkey.Length; ++j)
                {
                    digest[j] = (byte)(Mkey[j] ^ i);
                }

                _rc4.PrepareArcfourKey(digest, 0, Mkey.Length);
                _rc4.EncryptArcfour(UserKey, 0, 16);
            }
        }
        else
        {
            _rc4.PrepareArcfourKey(Mkey);
            _rc4.EncryptArcfour(_pad, UserKey);
        }
    }

    //
    // AESv3 (AES256 according to ISO 32000-2) support
    //

    /// <summary>
    ///     implements step d of Algorithm 2.A: Retrieving the file encryption key from an encrypted document in order to
    ///     decrypt it (revision 6 and later) - ISO 32000-2 section 7.6.4.3.3
    /// </summary>
    public void SetupByOwnerPassword(byte[] documentId,
                                     byte[] ownerPassword,
                                     byte[] uValue,
                                     byte[] ueValue,
                                     byte[] oValue,
                                     byte[] oeValue,
                                     int permissions)
    {
        if (oeValue == null)
        {
            throw new ArgumentNullException(nameof(oeValue));
        }

        var result = HashAlg2B(ownerPassword, oValue.CopyOfRange(40, 48), uValue);
        Key = AesCbcNoPadding.ProcessBlock(false, result, oeValue, 0, oeValue.Length);
        OwnerKey = oValue;
        UserKey = uValue;
        DocumentId = documentId;
        Permissions = permissions;
    }

    /// <summary>
    ///     implements step e of Algorithm 2.A: Retrieving the file encryption key from an encrypted document in order to
    ///     decrypt it (revision 6 and later) - ISO 32000-2 section 7.6.4.3.3
    /// </summary>
    public void SetupByUserPassword(byte[] documentId,
                                    byte[] userPassword,
                                    byte[] uValue,
                                    byte[] ueValue,
                                    byte[] oValue,
                                    byte[] oeValue,
                                    int permissions)
    {
        if (ueValue == null)
        {
            throw new ArgumentNullException(nameof(ueValue));
        }

        var result = HashAlg2B(userPassword, uValue.CopyOfRange(40, 48), null);
        Key = AesCbcNoPadding.ProcessBlock(false, result, ueValue, 0, ueValue.Length);
        OwnerKey = oValue;
        UserKey = uValue;
        DocumentId = documentId;
        Permissions = permissions;
    }

    /// <summary>
    ///     implements step f of Algorithm 2.A: Retrieving the file encryption key from an encrypted document in order to
    ///     decrypt it (revision 6 and later) - ISO 32000-2 section 7.6.4.3.3
    /// </summary>
    public bool DecryptAndCheckPerms(byte[] permsValue)
    {
        if (permsValue == null)
        {
            throw new ArgumentNullException(nameof(permsValue));
        }

        var decPerms = AesCbcNoPadding.ProcessBlock(false, Key, permsValue, 0, permsValue.Length);
        Permissions = (decPerms[0] & 0xff) | ((decPerms[1] & 0xff) << 8)
                                           | ((decPerms[2] & 0xff) << 16) | ((decPerms[2] & 0xff) << 24);
        _encryptMetadata = decPerms[8] == (byte)'T';
        return decPerms[9] == (byte)'a' && decPerms[10] == (byte)'d' && decPerms[11] == (byte)'b';
    }

    /// <summary>
    ///     implements Algorithm 2.B: Computing a hash (revision 6 and later) - ISO 32000-2 section 7.6.4.3.4
    /// </summary>
    public static byte[] HashAlg2B(byte[] input, byte[] salt, byte[] userKey)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (salt == null)
        {
            throw new ArgumentNullException(nameof(salt));
        }

        var sha256 = DigestUtilities.GetDigest("SHA-256");
        var sha384 = DigestUtilities.GetDigest("SHA-384");
        var sha512 = DigestUtilities.GetDigest("SHA-512");

        userKey ??= Array.Empty<byte>();

        sha256.BlockUpdate(input, 0, input.Length);
        sha256.BlockUpdate(salt, 0, salt.Length);
        sha256.BlockUpdate(userKey, 0, userKey.Length);
        var k = new byte[sha256.GetDigestSize()];
        sha256.DoFinal(k, 0);

        for (int round = 0, lastEByte = 0; round < 64 || lastEByte > round - 32; round++)
        {
            var singleSequenceSize = input.Length + k.Length + userKey.Length;
            var k1 = new byte[singleSequenceSize * 64];
            Array.Copy(input, 0, k1, 0, input.Length);
            Array.Copy(k, 0, k1, input.Length, k.Length);
            Array.Copy(userKey, 0, k1, input.Length + k.Length, userKey.Length);
            for (var i = 1; i < 64; i++)
            {
                Array.Copy(k1, 0, k1, singleSequenceSize * i, singleSequenceSize);
            }

            var e = AesCbcNoPadding.ProcessBlock(true,
                                                 k.CopyOf(16),
                                                 k1,
                                                 0,
                                                 k1.Length,
                                                 k.CopyOfRange(16, 32));

            lastEByte = e[e.Length - 1] & 0xFF;

            var intValue = new BigInteger(1, e.CopyOf(16)).Remainder(BigInteger.ValueOf(3)).IntValue;
            switch (intValue)
            {
                case 0:
                    sha256.BlockUpdate(e, 0, e.Length);
                    k = new byte[sha256.GetDigestSize()];
                    sha256.DoFinal(k, 0);
                    break;
                case 1:
                    sha384.BlockUpdate(e, 0, e.Length);
                    k = new byte[sha384.GetDigestSize()];
                    sha384.DoFinal(k, 0);
                    break;
                case 2:
                    sha512.BlockUpdate(e, 0, e.Length);
                    k = new byte[sha512.GetDigestSize()];
                    sha512.DoFinal(k, 0);
                    break;
            }
        }

        return k.CopyOf(32);
    }

    /**
     * implements Algorithm 8: Computing the encryption dictionary’s U (user password) and
     * UE (user encryption) values (Security handlers of revision 6) - ISO 32000-2 section 7.6.4.4.7
     */
    private void ComputeUAndUeAlg8(byte[] userPassword)
    {
        if (userPassword == null)
        {
            userPassword = Array.Empty<byte>();
        }
        else if (userPassword.Length > 127)
        {
            userPassword = userPassword.CopyOf(127);
        }

        var userSalts = IvGenerator.GetIv(16);

        UserKey = new byte[48];
        Array.Copy(userSalts, 0, UserKey, 32, 16);
        var result = HashAlg2B(userPassword, userSalts.CopyOf(8), null);
        Array.Copy(result, 0, UserKey, 0, 32);

        result = HashAlg2B(userPassword, userSalts.CopyOfRange(8, 16), null);
        _ueKey = AesCbcNoPadding.ProcessBlock(true, result, Key, 0, KeySize);
    }

    /**
     * implements Algorithm 9: Computing the encryption dictionary’s O (owner password) and
     * OE (owner encryption) values (Security handlers of revision 6) - ISO 32000-2 section 7.6.4.4.8
     */
    private void ComputeOAndOeAlg9(byte[] ownerPassword)
    {
        if (ownerPassword == null)
        {
            ownerPassword = Array.Empty<byte>();
        }
        else if (ownerPassword.Length > 127)
        {
            ownerPassword = ownerPassword.CopyOf(127);
        }

        var ownerSalts = IvGenerator.GetIv(16);

        OwnerKey = new byte[48];
        Array.Copy(ownerSalts, 0, OwnerKey, 32, 16);
        var result = HashAlg2B(ownerPassword, ownerSalts.CopyOf(8), UserKey);
        Array.Copy(result, 0, OwnerKey, 0, 32);

        result = HashAlg2B(ownerPassword, ownerSalts.CopyOfRange(8, 16), UserKey);
        _oeKey = AesCbcNoPadding.ProcessBlock(true, result, Key, 0, KeySize);
    }

    /**
     * implements Algorithm 10: Computing the encryption dictionary’s Perms (permissions)
     * value (Security handlers of revision 6) - ISO 32000-2 section 7.6.4.4.9
     */
    private void ComputePermsAlg10(int permissions)
    {
        var rawPerms = new byte[16];
        rawPerms[0] = (byte)(permissions & 0xff);
        rawPerms[1] = (byte)((permissions & 0xff00) >> 8);
        rawPerms[2] = (byte)((permissions & 0xff0000) >> 16);
        rawPerms[3] = (byte)((permissions & 0xff000000) >> 24);
        rawPerms[4] = 0xff;
        rawPerms[5] = 0xff;
        rawPerms[6] = 0xff;
        rawPerms[7] = 0xff;
        rawPerms[8] = (byte)(_encryptMetadata ? 'T' : 'F');
        rawPerms[9] = (byte)'a';
        rawPerms[10] = (byte)'d';
        rawPerms[11] = (byte)'b';
        Array.Copy(IvGenerator.GetIv(4), 0, rawPerms, 12, 4);

        _perms = AesCbcNoPadding.ProcessBlock(true, Key, rawPerms, 0, rawPerms.Length);
    }
}