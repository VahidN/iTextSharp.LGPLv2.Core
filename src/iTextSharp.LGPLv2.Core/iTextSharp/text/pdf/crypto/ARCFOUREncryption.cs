namespace iTextSharp.text.pdf.crypto;

public class ArcfourEncryption
{
    private readonly byte[] _state = new byte[256];
    private int _x;
    private int _y;

    public void EncryptArcfour(byte[] dataIn, int off, int len, byte[] dataOut, int offOut)
    {
        var length = len + off;
        byte tmp;
        for (var k = off; k < length; ++k)
        {
            _x = (_x + 1) & 255;
            _y = (_state[_x] + _y) & 255;
            tmp = _state[_x];
            _state[_x] = _state[_y];
            _state[_y] = tmp;
            dataOut[k - off + offOut] = (byte)(dataIn[k] ^ _state[(_state[_x] + _state[_y]) & 255]);
        }
    }

    public void EncryptArcfour(byte[] data, int off, int len)
    {
        EncryptArcfour(data, off, len, data, off);
    }

    public void EncryptArcfour(byte[] dataIn, byte[] dataOut)
    {
        EncryptArcfour(dataIn, 0, dataIn.Length, dataOut, 0);
    }

    public void EncryptArcfour(byte[] data)
    {
        EncryptArcfour(data, 0, data.Length, data, 0);
    }

    public void PrepareArcfourKey(byte[] key)
    {
        PrepareArcfourKey(key, 0, key.Length);
    }

    public void PrepareArcfourKey(byte[] key, int off, int len)
    {
        var index1 = 0;
        var index2 = 0;
        for (var k = 0; k < 256; ++k)
        {
            _state[k] = (byte)k;
        }

        _x = 0;
        _y = 0;
        byte tmp;
        for (var k = 0; k < 256; ++k)
        {
            index2 = (key[index1 + off] + _state[k] + index2) & 255;
            tmp = _state[k];
            _state[k] = _state[index2];
            _state[index2] = tmp;
            index1 = (index1 + 1) % len;
        }
    }
}