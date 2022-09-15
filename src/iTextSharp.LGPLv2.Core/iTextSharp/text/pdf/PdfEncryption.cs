using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using iTextSharp.text.pdf.crypto;
using Org.BouncyCastle.X509;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// @author  Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class PdfEncryption
    {

        public const int AES_128 = 4;
        public const int STANDARD_ENCRYPTION_128 = 3;
        public const int STANDARD_ENCRYPTION_40 = 2;
        internal static readonly byte[] MetadataPad = { 255, 255, 255, 255 };

        internal static long Seq = DateTime.Now.Ticks + Environment.TickCount;

        internal byte[] DocumentId;

        /// <summary>
        /// Work area to prepare the object/generation bytes
        /// </summary>
        internal byte[] Extra = new byte[5];

        /// <summary>
        /// The encryption key for a particular object/generation
        /// </summary>
        internal byte[] Key;

        /// <summary>
        /// The encryption key length for a particular object/generation
        /// </summary>
        internal int KeySize;

        /// <summary>
        /// The global encryption key
        /// </summary>
        internal byte[] Mkey;

        /// <summary>
        /// The encryption key for the owner
        /// </summary>
        internal byte[] OwnerKey = new byte[32];

        internal int Permissions;

        /// <summary>
        /// The encryption key for the user
        /// </summary>
        internal byte[] UserKey = new byte[32];

        /// <summary>
        /// The public key security handler for certificate encryption
        /// </summary>
        protected PdfPublicKeySecurityHandler PublicKeyHandler;

        private static readonly byte[] _pad = {
        0x28, 0xBF, 0x4E, 0x5E, 0x4E, 0x75,
        0x8A, 0x41, 0x64, 0x00, 0x4E, 0x56,
        0xFF, 0xFA, 0x01, 0x08, 0x2E, 0x2E,
        0x00, 0xB6, 0xD0, 0x68, 0x3E, 0x80,
        0x2F, 0x0C, 0xA9, 0xFE, 0x64, 0x53,
        0x69, 0x7A};

        private static readonly byte[] _salt = { 0x73, 0x41, 0x6c, 0x54 };
        private readonly ArcfourEncryption _rc4 = new ArcfourEncryption();
        private int _cryptoMode;
        /// <summary>
        /// Indicates if the encryption is only necessary for embedded files.
        /// @since 2.1.3
        /// </summary>
        private bool _embeddedFilesOnly;

        private bool _encryptMetadata;
        /// <summary>
        /// The generic key length. It may be 40 or 128.
        /// </summary>
        private int _keyLength;

        private int _revision;
        public PdfEncryption()
        {
            PublicKeyHandler = new PdfPublicKeySecurityHandler();
        }

        public PdfEncryption(PdfEncryption enc) : this()
        {
            Mkey = (byte[])enc.Mkey.Clone();
            OwnerKey = (byte[])enc.OwnerKey.Clone();
            UserKey = (byte[])enc.UserKey.Clone();
            Permissions = enc.Permissions;
            if (enc.DocumentId != null)
                DocumentId = (byte[])enc.DocumentId.Clone();
            _revision = enc._revision;
            _keyLength = enc._keyLength;
            _encryptMetadata = enc._encryptMetadata;
            _embeddedFilesOnly = enc._embeddedFilesOnly;
            PublicKeyHandler = enc.PublicKeyHandler;
        }

        public PdfObject FileId
        {
            get
            {
                return CreateInfoId(DocumentId);
            }
        }

        public static byte[] CreateDocumentId()
        {
            long time = DateTime.Now.Ticks + Environment.TickCount;
            long mem = GC.GetTotalMemory(false);
            string s = time + "+" + mem + "+" + (Seq++);
            return MD5BouncyCastle.Create().ComputeHash(Encoding.ASCII.GetBytes(s));
        }

        public static PdfObject CreateInfoId(byte[] id)
        {
            ByteBuffer buf = new ByteBuffer(90);
            buf.Append('[').Append('<');
            for (int k = 0; k < 16; ++k)
                buf.AppendHex(id[k]);
            buf.Append('>').Append('<');
            id = CreateDocumentId();
            for (int k = 0; k < 16; ++k)
                buf.AppendHex(id[k]);
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
            if (_revision == AES_128)
                return (n & 0x7ffffff0) + 32;
            else
                return n;
        }

        public byte[] ComputeUserPassword(byte[] ownerPassword)
        {
            byte[] userPad = computeOwnerKey(OwnerKey, padPassword(ownerPassword));
            for (int i = 0; i < userPad.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < userPad.Length - i; j++)
                {
                    if (userPad[i + j] != _pad[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (!match) continue;
                byte[] userPassword = new byte[i];
                Array.Copy(userPad, 0, userPassword, 0, i);
                return userPassword;
            }
            return userPad;
        }

        public byte[] DecryptByteArray(byte[] b)
        {
            MemoryStream ba = new MemoryStream();
            StandardDecryption dec = GetDecryptor();
            byte[] b2 = dec.Update(b, 0, b.Length);
            if (b2 != null)
                ba.Write(b2, 0, b2.Length);
            b2 = dec.Finish();
            if (b2 != null)
                ba.Write(b2, 0, b2.Length);
            return ba.ToArray();
        }

        public byte[] EncryptByteArray(byte[] b)
        {
            MemoryStream ba = new MemoryStream();
            OutputStreamEncryption os2 = GetEncryptionStream(ba);
            os2.Write(b, 0, b.Length);
            os2.Finish();
            return ba.ToArray();
        }

        public int GetCryptoMode()
        {
            return _cryptoMode;
        }

        public StandardDecryption GetDecryptor()
        {
            return new StandardDecryption(Key, 0, KeySize, _revision);
        }

        public PdfDictionary GetEncryptionDictionary()
        {
            PdfDictionary dic = new PdfDictionary();

            if (PublicKeyHandler.GetRecipientsSize() > 0)
            {
                PdfArray recipients = null;

                dic.Put(PdfName.Filter, PdfName.Pubsec);
                dic.Put(PdfName.R, new PdfNumber(_revision));

                recipients = PublicKeyHandler.GetEncodedRecipients();

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

                    PdfDictionary stdcf = new PdfDictionary();
                    stdcf.Put(PdfName.Recipients, recipients);
                    if (!_encryptMetadata)
                        stdcf.Put(PdfName.Encryptmetadata, PdfBoolean.Pdffalse);

                    if (_revision == AES_128)
                        stdcf.Put(PdfName.Cfm, PdfName.Aesv2);
                    else
                        stdcf.Put(PdfName.Cfm, PdfName.V2);
                    PdfDictionary cf = new PdfDictionary();
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
                SHA1 sh = new SHA1CryptoServiceProvider();
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
                    byte[] seed = PublicKeyHandler.GetSeed();
                    sh.AppendData(seed, 0, seed.Length);
                    for (int i = 0; i < PublicKeyHandler.GetRecipientsSize(); i++)
                    {
                        var encodedRecipient = PublicKeyHandler.GetEncodedRecipient(i);
                        sh.AppendData(encodedRecipient, 0, encodedRecipient.Length);
                    }
                    if (!_encryptMetadata)
                        sh.AppendData(MetadataPad, 0, MetadataPad.Length);

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
                else
                {
                    if (!_encryptMetadata)
                        dic.Put(PdfName.Encryptmetadata, PdfBoolean.Pdffalse);
                    dic.Put(PdfName.R, new PdfNumber(AES_128));
                    dic.Put(PdfName.V, new PdfNumber(4));
                    dic.Put(PdfName.LENGTH, new PdfNumber(128));
                    PdfDictionary stdcf = new PdfDictionary();
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
                    if (_revision == AES_128)
                        stdcf.Put(PdfName.Cfm, PdfName.Aesv2);
                    else
                        stdcf.Put(PdfName.Cfm, PdfName.V2);
                    PdfDictionary cf = new PdfDictionary();
                    cf.Put(PdfName.Stdcf, stdcf);
                    dic.Put(PdfName.Cf, cf);
                }
            }
            return dic;
        }

        public OutputStreamEncryption GetEncryptionStream(Stream os)
        {
            return new OutputStreamEncryption(os, Key, 0, KeySize, _revision);
        }

        /// <summary>
        /// Indicates if only the embedded files have to be encrypted.
        /// @since   2.1.3
        /// </summary>
        /// <returns>if true only the embedded files will be encrypted</returns>
        public bool IsEmbeddedFilesOnly()
        {
            return _embeddedFilesOnly;
        }

        public bool IsMetadataEncrypted()
        {
            return _encryptMetadata;
        }

        public void SetCryptoMode(int mode, int kl)
        {
            _cryptoMode = mode;
            _encryptMetadata = (mode & PdfWriter.DO_NOT_ENCRYPT_METADATA) == 0;
            _embeddedFilesOnly = (mode & PdfWriter.EMBEDDED_FILES_ONLY) != 0;
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
                    if (kl > 0)
                        _keyLength = kl;
                    else
                        _keyLength = 128;
                    _revision = STANDARD_ENCRYPTION_128;
                    break;
                case PdfWriter.ENCRYPTION_AES_128:
                    _keyLength = 128;
                    _revision = AES_128;
                    break;
                default:
                    throw new ArgumentException("No valid encryption mode");
            }
        }

        public void SetHashKey(int number, int generation)
        {
            using (var md5 = MD5BouncyCastle.Create())
            {
                md5.Initialize();
                Extra[0] = (byte)number;
                Extra[1] = (byte)(number >> 8);
                Extra[2] = (byte)(number >> 16);
                Extra[3] = (byte)generation;
                Extra[4] = (byte)(generation >> 8);
                if ((Mkey != null) || (Extra != null) || (_salt != null))
                    throw new Exception("Null value in PDFEncryption method SetHashKey");
                md5.TransformBlock(Mkey, 0, Mkey.Length, Mkey, 0);
                md5.TransformBlock(Extra, 0, Extra.Length, Extra, 0);
                if (_revision == AES_128)
                    md5.TransformBlock(_salt, 0, _salt.Length, _salt, 0);

                md5.TransformFinalBlock(Extra, 0, 0);
                Key = md5.Hash;
            }

            KeySize = Mkey.Length + 5;
            if (KeySize > 16)
                KeySize = 16;
        }

        /// <summary>
        /// gets keylength and revision and uses revison to choose the initial values for permissions
        /// </summary>
        public void SetupAllKeys(byte[] userPassword, byte[] ownerPassword, int permissions)
        {
            if (ownerPassword == null || ownerPassword.Length == 0)
                ownerPassword = MD5BouncyCastle.Create().ComputeHash(CreateDocumentId());

            permissions |= (int)((_revision == STANDARD_ENCRYPTION_128 || _revision == AES_128) ? 0xfffff0c0 : 0xffffffc0);
            permissions &= unchecked((int)0xfffffffc);
            //PDF refrence 3.5.2 Standard Security Handler, Algorithum 3.3-1
            //If there is no owner password, use the user password instead.
            byte[] userPad = padPassword(userPassword);
            byte[] ownerPad = padPassword(ownerPassword);

            OwnerKey = computeOwnerKey(userPad, ownerPad);
            DocumentId = CreateDocumentId();
            setupByUserPad(DocumentId, userPad, OwnerKey, permissions);
        }

        public void SetupByEncryptionKey(byte[] key, int keylength)
        {
            Mkey = new byte[keylength / 8];
            Array.Copy(key, 0, Mkey, 0, Mkey.Length);
        }

        /// <summary>
        /// </summary>
        public void SetupByOwnerPassword(byte[] documentId, byte[] ownerPassword, byte[] userKey, byte[] ownerKey, int permissions)
        {
            setupByOwnerPad(documentId, padPassword(ownerPassword), userKey, ownerKey, permissions);
        }

        public void SetupByUserPassword(byte[] documentId, byte[] userPassword, byte[] ownerKey, int permissions)
        {
            setupByUserPad(documentId, padPassword(userPassword), ownerKey, permissions);
        }

        /// <summary>
        /// </summary>
        private byte[] computeOwnerKey(byte[] userPad, byte[] ownerPad)
        {
            byte[] ownerKey = new byte[32];
            var md5 = MD5BouncyCastle.Create();
            byte[] digest = md5.ComputeHash(ownerPad);
            if (_revision == STANDARD_ENCRYPTION_128 || _revision == AES_128)
            {
                byte[] mkey = new byte[_keyLength / 8];
                // only use for the input as many bit as the key consists of
                for (int k = 0; k < 50; ++k)
                    Array.Copy(md5.ComputeHash(digest), 0, digest, 0, mkey.Length);
                Array.Copy(userPad, 0, ownerKey, 0, 32);
                for (int i = 0; i < 20; ++i)
                {
                    for (int j = 0; j < mkey.Length; ++j)
                        mkey[j] = (byte)(digest[j] ^ i);
                    _rc4.PrepareArcfourKey(mkey);
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

        private byte[] padPassword(byte[] userPassword)
        {
            byte[] userPad = new byte[32];
            if (userPassword == null)
            {
                Array.Copy(_pad, 0, userPad, 0, 32);
            }
            else
            {
                Array.Copy(userPassword, 0, userPad, 0, Math.Min(userPassword.Length, 32));
                if (userPassword.Length < 32)
                    Array.Copy(_pad, 0, userPad, userPassword.Length, 32 - userPassword.Length);
            }

            return userPad;
        }
        private void setupByOwnerPad(byte[] documentId, byte[] ownerPad, byte[] userKey, byte[] ownerKey, int permissions)
        {
            byte[] userPad = computeOwnerKey(ownerKey, ownerPad); //userPad will be set in this.ownerKey
            setupGlobalEncryptionKey(documentId, userPad, ownerKey, permissions); //step 3
            setupUserKey();
        }

        /// <summary>
        /// </summary>
        private void setupByUserPad(byte[] documentId, byte[] userPad, byte[] ownerKey, int permissions)
        {
            setupGlobalEncryptionKey(documentId, userPad, ownerKey, permissions);
            setupUserKey();
        }

        /// <summary>
        /// ownerKey, documentID must be setuped
        /// </summary>
        private void setupGlobalEncryptionKey(byte[] documentId, byte[] userPad, byte[] ownerKey, int permissions)
        {
            DocumentId = documentId;
            OwnerKey = ownerKey;
            Permissions = permissions;
            // use variable keylength
            Mkey = new byte[_keyLength / 8];
            byte[] digest = new byte[Mkey.Length];

            //fixed by ujihara in order to follow PDF refrence
            using (var md5 = MD5BouncyCastle.Create())
            {
                md5.Initialize();
                md5.TransformBlock(userPad, 0, userPad.Length, userPad, 0);
                md5.TransformBlock(ownerKey, 0, ownerKey.Length, ownerKey, 0);

                byte[] ext = new byte[4];
                ext[0] = (byte)permissions;
                ext[1] = (byte)(permissions >> 8);
                ext[2] = (byte)(permissions >> 16);
                ext[3] = (byte)(permissions >> 24);
                md5.TransformBlock(ext, 0, 4, ext, 0);
                if (documentId != null)
                    md5.TransformBlock(documentId, 0, documentId.Length, documentId, 0);
                if (!_encryptMetadata)
                    md5.TransformBlock(MetadataPad, 0, MetadataPad.Length, MetadataPad, 0);
                md5.TransformFinalBlock(ext, 0, 0);

                Array.Copy(md5.Hash, 0, digest, 0, Mkey.Length);
            }


            // only use the really needed bits as input for the hash
            if (_revision == STANDARD_ENCRYPTION_128 || _revision == AES_128)
            {
                for (int k = 0; k < 50; ++k)
                {
                    using (var md5Hash = MD5BouncyCastle.Create())
                    {
                        Array.Copy(md5Hash.ComputeHash(digest), 0, digest, 0, Mkey.Length);
                    }
                }
            }
            Array.Copy(digest, 0, Mkey, 0, Mkey.Length);
        }

        /// <summary>
        /// mkey must be setuped
        /// </summary>
        /// <summary>
        /// use the revision to choose the setup method
        /// </summary>
        private void setupUserKey()
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
                for (int k = 16; k < 32; ++k)
                    UserKey[k] = 0;
                for (int i = 0; i < 20; ++i)
                {
                    for (int j = 0; j < Mkey.Length; ++j)
                        digest[j] = (byte)(Mkey[j] ^ i);
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
    }
}