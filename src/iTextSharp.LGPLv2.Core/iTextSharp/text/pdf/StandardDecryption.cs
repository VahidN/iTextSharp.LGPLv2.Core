namespace iTextSharp.text.pdf.crypto;

/// <summary>
/// </summary>
public class StandardDecryption
{
    private const int Aes128 = 4;
    private const int AES_256_V3 = 6;
    private readonly bool _aes;
    private readonly byte[] _iv = new byte[16];
    private readonly byte[] _key;
    private bool _initiated;
    private int _ivptr;
    protected ArcfourEncryption Arcfour;
    protected AesCipher Cipher;

    /// <summary>
    ///     Creates a new instance of StandardDecryption
    /// </summary>
    public StandardDecryption(byte[] key, int off, int len, int revision)
    {
        _aes = revision == Aes128 || revision == AES_256_V3;
        if (_aes)
        {
            _key = new byte[len];
            Array.Copy(key, off, _key, 0, len);
        }
        else
        {
            Arcfour = new ArcfourEncryption();
            Arcfour.PrepareArcfourKey(key, off, len);
        }
    }

    public byte[] Finish()
    {
        if (_aes && Cipher != null)
        {
            return Cipher.DoFinal();
        }

        return null;
    }

    public byte[] Update(byte[] b, int off, int len)
    {
        if (_aes)
        {
            if (_initiated)
            {
                return Cipher.Update(b, off, len);
            }

            var left = Math.Min(_iv.Length - _ivptr, len);
            Array.Copy(b, off, _iv, _ivptr, left);
            off += left;
            len -= left;
            _ivptr += left;
            if (_ivptr == _iv.Length)
            {
                Cipher = new AesCipher(false, _key, _iv);
                _initiated = true;
                if (len > 0)
                {
                    return Cipher.Update(b, off, len);
                }
            }

            return null;
        }

        var b2 = new byte[len];
        Arcfour.EncryptArcfour(b, off, len, b2, 0);
        return b2;
    }
}