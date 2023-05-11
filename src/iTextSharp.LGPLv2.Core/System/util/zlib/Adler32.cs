namespace System.util.zlib;

internal sealed class Adler32
{
    /// <summary>
    ///     largest prime smaller than 65536
    /// </summary>
    private const int Base = 65521;

    /// <summary>
    ///     NMAX is the largest n such that 255n(n+1)/2 + (n+1)(BASE-1)
    /// </summary>
    private const int Nmax = 5552;

    internal static long adler32(long adler, byte[] buf, int index, int len)
    {
        if (buf == null)
        {
            return 1L;
        }

        var s1 = adler & 0xffff;
        var s2 = (adler >> 16) & 0xffff;
        int k;

        while (len > 0)
        {
            k = len < Nmax ? len : Nmax;
            len -= k;
            while (k >= 16)
            {
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                s1 += buf[index++] & 0xff;
                s2 += s1;
                k -= 16;
            }

            if (k != 0)
            {
                do
                {
                    s1 += buf[index++] & 0xff;
                    s2 += s1;
                } while (--k != 0);
            }

            s1 %= Base;
            s2 %= Base;
        }

        return (s2 << 16) | s1;
    }
}