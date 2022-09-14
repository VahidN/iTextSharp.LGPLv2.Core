using acryptohashnet;
using System;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography
{
    /// <summary>
    /// RFC1320: The MD4 Message-Digest Algorithm
    /// https://datatracker.ietf.org/doc/html/rfc1320
    /// </summary>
    public class MD4 : BlockHashAlgorithm
    {
        public static new MD4 Create() => new MD4();

        private static readonly uint[] Constants = new uint[]
        {
            0x00000000,
            0x5a827999, // [2 ^ 30 * sqrt(2)]
            0x6ed9eba1, // [2 ^ 30 * sqrt(3)]
        };

        private readonly uint[] state = new uint[4];

        private readonly uint[] buffer = new uint[16];

        private readonly byte[] finalBlock;

        private readonly BigCounter processedLength = new BigCounter(8);

        public MD4() : base(64)
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

            uint k0 = Constants[0];
            uint k1 = Constants[1];
            uint k2 = Constants[2];

            uint a = state[0];
            uint b = state[1];
            uint c = state[2];
            uint d = state[3];

            // Round 1
            for (int ii = 0; ii < 16; ii += 4)
            {
                a += buffer[ii + 0] + k0 + F(b, c, d);
                a = Bits.RotateLeft(a, 3);

                d += buffer[ii + 1] + k0 + F(a, b, c);
                d = Bits.RotateLeft(d, 7);

                c += buffer[ii + 2] + k0 + F(d, a, b);
                c = Bits.RotateLeft(c, 11);

                b += buffer[ii + 3] + k0 + F(c, d, a);
                b = Bits.RotateLeft(b, 19);
            }

            // Round 2
            for (int ii = 16, jj = 0; ii < 32; ii += 4, jj++)
            {
                a += buffer[jj + 00] + k1 + G(b, c, d);
                a = Bits.RotateLeft(a, 3);

                d += buffer[jj + 04] + k1 + G(a, b, c);
                d = Bits.RotateLeft(d, 5);

                c += buffer[jj + 08] + k1 + G(d, a, b);
                c = Bits.RotateLeft(c, 9);

                b += buffer[jj + 12] + k1 + G(c, d, a);
                b = Bits.RotateLeft(b, 13);
            }

            // Round 3
            for (int ii = 32, jj = 0; ii < 48; ii += 4, jj++)
            {
                int index = (jj << 1) + -3 * (jj >> 1); // jj * 2 + (jj / 2) * (-3);

                a += buffer[index + 00] + k2 + H(b, c, d);
                a = Bits.RotateLeft(a, 3);

                d += buffer[index + 08] + k2 + H(a, b, c);
                d = Bits.RotateLeft(d, 9);

                c += buffer[index + 04] + k2 + H(d, a, b);
                c = Bits.RotateLeft(c, 11);

                b += buffer[index + 12] + k2 + H(c, d, a);
                b = Bits.RotateLeft(b, 15);
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
            return (x & y) | (x & z) | (y & z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint H(uint x, uint y, uint z)
        {
            return x ^ y ^ z;
        }

        private void InitializeState()
        {
            state[0] = 0x67452301;
            state[1] = 0xefcdab89;
            state[2] = 0x98badcfe;
            state[3] = 0x10325476;
        }
    }

    public class MD4CryptoServiceProvider : MD4 { }
}
