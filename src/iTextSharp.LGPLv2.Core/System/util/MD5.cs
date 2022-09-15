using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace iTextSharp
{
    /// <summary>
    /// RFC1321: The MD5 Message-Digest Algorithm
    /// https://datatracker.ietf.org/doc/html/rfc1321
    /// </summary>
    public sealed class MD5BouncyCastle : HashAlgorithm
    {
#if NET40
        public static new HashAlgorithm Create() =>	MD5.Create();
#else	
        public static new HashAlgorithm Create() =>
            string.Equals(System.Runtime.InteropServices.RuntimeInformation.OSDescription, "Browser", StringComparison.OrdinalIgnoreCase) ? 
			new MD5BouncyCastle() : 
			MD5.Create();
#endif			
			
        private MD5BouncyCastle() { }

        private MD5Digest _digestInternal = new();

        public override void Initialize() {}

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _digestInternal.BlockUpdate(array, ibStart, cbSize);
        }

        protected override byte[] HashFinal()
        {
            byte[] output = new byte[_digestInternal.GetByteLength()];
            _digestInternal.DoFinal(output, 0);
            return output;
        }
    }
}