using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace iTextSharp.text.pdf.crypto;

public static class AesCbcNoPadding
{
    public static byte[] ProcessBlock(
        bool forEncryption,
        byte[] key,
        byte[] inBuf,
        int inOff,
        int inLen,
        byte[] iv = null)
    {
        var aes = new AesEngine();
        var cbcBlockCipher = new CbcBlockCipher(aes);
        var keyParameter = new KeyParameter(key);

        if (iv is null)
        {
            cbcBlockCipher.Init(forEncryption, keyParameter);
        }
        else
        {
            var parametersWithIv = new ParametersWithIV(keyParameter, iv);
            cbcBlockCipher.Init(forEncryption, parametersWithIv);
        }

        if (inLen % cbcBlockCipher.GetBlockSize() != 0)
        {
            throw new ArgumentException($"{inLen} is not multiple of BlockSize.");
        }

        var output = new byte[inLen];
        var outOff = 0;
        while (inLen > 0)
        {
            cbcBlockCipher.ProcessBlock(inBuf, inOff, output, outOff);
            inLen -= cbcBlockCipher.GetBlockSize();
            outOff += cbcBlockCipher.GetBlockSize();
            inOff += cbcBlockCipher.GetBlockSize();
        }

        return output;
    }
}