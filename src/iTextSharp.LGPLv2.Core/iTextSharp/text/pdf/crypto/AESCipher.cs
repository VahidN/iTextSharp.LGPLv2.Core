using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;

namespace iTextSharp.text.pdf.crypto
{
    /// <summary>
    /// Creates an AES Cipher with CBC and padding PKCS5/7.
    /// @author Paulo Soares (psoares@consiste.pt)
    /// </summary>
    public class AesCipher
    {
        private readonly PaddedBufferedBlockCipher _bp;

        /// <summary>
        /// Creates a new instance of AESCipher
        /// </summary>
        public AesCipher(bool forEncryption, byte[] key, byte[] iv)
        {
            IBlockCipher aes = new AesEngine();
            IBlockCipher cbc = new CbcBlockCipher(aes);
            _bp = new PaddedBufferedBlockCipher(cbc);
            KeyParameter kp = new KeyParameter(key);
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            _bp.Init(forEncryption, piv);
        }

        public byte[] Update(byte[] inp, int inpOff, int inpLen)
        {
            int neededLen = _bp.GetUpdateOutputSize(inpLen);
            byte[] outp = null;
            if (neededLen > 0)
                outp = new byte[neededLen];
            else
                neededLen = 0;
            _bp.ProcessBytes(inp, inpOff, inpLen, outp, 0);
            return outp;
        }

        public byte[] DoFinal()
        {
            int neededLen = _bp.GetOutputSize(0);
            byte[] outp = new byte[neededLen];
            int n = 0;
            try
            {
                n = _bp.DoFinal(outp, 0);
            }
            catch
            {
                return outp;
            }
            if (n != outp.Length)
            {
                byte[] outp2 = new byte[n];
                System.Array.Copy(outp, 0, outp2, 0, n);
                return outp2;
            }
            else
                return outp;
        }

    }
}