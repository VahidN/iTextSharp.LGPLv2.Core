using iTextSharp.text.pdf.crypto;

namespace iTextSharp.text.pdf;

public class OutputStreamEncryption : Stream
{
    protected ArcfourEncryption Arcfour;
    protected AesCipher Cipher;
    protected Stream Outc;
    private const int Aes128 = 4;
    private const int AES_256_V3 = 6;
    private readonly bool _aes;
    private readonly byte[] _buf = new byte[1];
    private bool _finished;

    public OutputStreamEncryption(Stream outc, byte[] key, int off, int len, int revision)
    {
        Outc = outc;
        _aes = revision == Aes128 || revision == AES_256_V3;
        if (_aes)
        {
            var iv = IvGenerator.GetIv();
            var nkey = new byte[len];
            Array.Copy(key, off, nkey, 0, len);
            Cipher = new AesCipher(true, nkey, iv);
            Write(iv, 0, iv.Length);
        }
        else
        {
            Arcfour = new ArcfourEncryption();
            Arcfour.PrepareArcfourKey(key, off, len);
        }
    }

    public OutputStreamEncryption(Stream outc, byte[] key, int revision) :
        this(outc, key, 0, key?.Length ?? throw new ArgumentNullException(nameof(key)), revision)
    {
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

#if NETSTANDARD1_3
        public void Close()
#else
    public override void Close()
#endif
    {
        Finish();
    }

    public void Finish()
    {
        if (!_finished)
        {
            _finished = true;
            if (_aes)
            {
                var b = Cipher.DoFinal();
                Outc.Write(b, 0, b.Length);
            }
        }
    }

    public override void Flush()
    {
        Outc.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (_aes)
        {
            var b2 = Cipher.Update(buffer, offset, count);
            if (b2 == null || b2.Length == 0)
            {
                return;
            }

            Outc.Write(b2, 0, b2.Length);
        }
        else
        {
            var b2 = new byte[Math.Min(count, 4192)];
            while (count > 0)
            {
                var sz = Math.Min(count, b2.Length);
                Arcfour.EncryptArcfour(buffer, offset, sz, b2, 0);
                Outc.Write(b2, 0, sz);
                count -= sz;
                offset += sz;
            }
        }
    }

    public override void WriteByte(byte value)
    {
        _buf[0] = value;
        Write(_buf, 0, 1);
    }
}