using acryptohashnet;
using System;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography
{
    /// <summary>
    /// RFC1321: The MD5 Message-Digest Algorithm
    /// https://datatracker.ietf.org/doc/html/rfc1321
    /// </summary>
    public class MD5 : BlockHashAlgorithm
    {
        public static new MD5 Create() => new();

        private static readonly uint[] Constants = new uint[]
        {
            // round 1
            0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
            0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
            0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
            0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
            // round 2
            0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
            0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
            0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
            0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
            // round 3
            0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
            0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
            0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
            0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
            // round 4
            0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
            0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
            0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
            0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
        };

        private readonly BigCounter processedLength = new BigCounter(8);

        private readonly uint[] state = new uint[4];

        private readonly uint[] buffer = new uint[16];

        private readonly byte[] finalBlock;

        public MD5() : base(64)
        {
            HashSizeValue = 128;

            finalBlock = new byte[BlockSize];
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            processedLength.Clear();

            Array.Clear(finalBlock, 0, finalBlock.Length);

            InitializeState();
        }

        protected override void ProcessBlock(byte[] array, int offset)
        {
            processedLength.Add(BlockSize << 3); // * 8

            // Fill buffer for transformations
            Buffer.BlockCopy(array, offset, buffer, 0, BlockSize);

            uint a = state[0];
            uint b = state[1];
            uint c = state[2];
            uint d = state[3];

            int index;
            // Round 1
            for (index = 0; index < 16; index += 4)
            {
                a += buffer[index + 0] + Constants[index + 0] + F(b, c, d);
                a = Bits.RotateLeft(a, 7);
                a += b;

                d += buffer[index + 1] + Constants[index + 1] + F(a, b, c);
                d = Bits.RotateLeft(d, 12);
                d += a;

                c += buffer[index + 2] + Constants[index + 2] + F(d, a, b);
                c = Bits.RotateLeft(c, 17);
                c += d;

                b += buffer[index + 3] + Constants[index + 3] + F(c, d, a);
                b = Bits.RotateLeft(b, 22);
                b += c;
            }

            // Round 2
            for (index = 16; index < 32; index += 4)
            {
                a += buffer[((index + 0) * 5 + 1) & 0xf] + Constants[index + 0] + G(b, c, d);
                a = Bits.RotateLeft(a, 5);
                a += b;

                d += buffer[((index + 1) * 5 + 1) & 0xf] + Constants[index + 1] + G(a, b, c);
                d = Bits.RotateLeft(d, 9);
                d += a;

                c += buffer[((index + 2) * 5 + 1) & 0xf] + Constants[index + 2] + G(d, a, b);
                c = Bits.RotateLeft(c, 14);
                c += d;

                b += buffer[((index + 3) * 5 + 1) & 0xf] + Constants[index + 3] + G(c, d, a);
                b = Bits.RotateLeft(b, 20);
                b += c;
            }

            // Round 3
            for (index = 32; index < 48; index += 4)
            {
                a += buffer[((index + 0) * 3 + 5) & 0xf] + Constants[index + 0] + H(b, c, d);
                a = Bits.RotateLeft(a, 4);
                a += b;

                d += buffer[((index + 1) * 3 + 5) & 0xf] + Constants[index + 1] + H(a, b, c);
                d = Bits.RotateLeft(d, 11);
                d += a;

                c += buffer[((index + 2) * 3 + 5) & 0xf] + Constants[index + 2] + H(d, a, b);
                c = Bits.RotateLeft(c, 16);
                c += d;

                b += buffer[((index + 3) * 3 + 5) & 0xf] + Constants[index + 3] + H(c, d, a);
                b = Bits.RotateLeft(b, 23);
                b += c;
            }

            // Round 4
            for (index = 48; index < 64; index += 4)
            {
                a += buffer[((index + 0) * 7 + 0) & 0xf] + Constants[index + 0] + I(b, c, d);
                a = Bits.RotateLeft(a, 6);
                a += b;

                d += buffer[((index + 1) * 7 + 0) & 0xf] + Constants[index + 1] + I(a, b, c);
                d = Bits.RotateLeft(d, 10);
                d += a;

                c += buffer[((index + 2) * 7 + 0) & 0xf] + Constants[index + 2] + I(d, a, b);
                c = Bits.RotateLeft(c, 15);
                c += d;

                b += buffer[((index + 3) * 7 + 0) & 0xf] + Constants[index + 3] + I(c, d, a);
                b = Bits.RotateLeft(b, 21);
                b += c;
            }

            // The end
            state[0] += a;
            state[1] += b;
            state[2] += c;
            state[3] += d;
        }

        protected override void ProcessFinalBlock(byte[] array, int offset, int length)
        {
            processedLength.Add(length << 3); // * 8

            byte[] messageLength = processedLength.GetBytes();

            Buffer.BlockCopy(array, offset, finalBlock, 0, length);

            // padding message with 100..000 bits
            finalBlock[length] = 0x80;

            int endOffset = BlockSize - 8;
            if (length >= endOffset)
            {
                ProcessBlock(finalBlock, 0);

                Array.Clear(finalBlock, 0, finalBlock.Length);
            }

            for (int ii = 0; ii < 8; ii++)
            {
                finalBlock[endOffset + ii] = messageLength[ii];
            }

            // Processing of last block
            ProcessBlock(finalBlock, 0);
        }

        protected override byte[] Result
        {
            get
            {
                byte[] result = new byte[16];

                Buffer.BlockCopy(state, 0, result, 0, result.Length);

                return result;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint F(uint x, uint y, uint z)
        {
            return (x & y) | (~x & z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint G(uint x, uint y, uint z)
        {
            return (x & z) | (y & ~z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint H(uint x, uint y, uint z)
        {
            return x ^ y ^ z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint I(uint x, uint y, uint z)
        {
            return y ^ (x | ~ z);
        }

        private void InitializeState()
        {
            state[0] = 0x67452301;
            state[1] = 0xefcdab89;
            state[2] = 0x98badcfe;
            state[3] = 0x10325476;
        }
    }

    public class MD5CryptoServiceProvider : MD5 {}
}