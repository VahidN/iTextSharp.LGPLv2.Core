using acryptohashnet;
using System;

namespace System.Security.Cryptography
{
    /// <summary>
    /// RFC1319: The MD2 Message-Digest Algorithm
    /// https://datatracker.ietf.org/doc/html/rfc1319
    /// </summary>
    public class MD2 : BlockHashAlgorithm
    {
        public static new MD2 Create() => new MD2();

        private static readonly byte[] Pi = new byte[]
        {
            041, 046, 067, 201,
            162, 216, 124, 001,
            061, 054, 084, 161,
            236, 240, 006, 019,

            098, 167, 005, 243,
            192, 199, 115, 140,
            152, 147, 043, 217,
            188, 076, 130, 202,

            030, 155, 087, 060,
            253, 212, 224, 022,
            103, 066, 111, 024,
            138, 023, 229, 018,

            190, 078, 196, 214,
            218, 158, 222, 073,
            160, 251, 245, 142,
            187, 047, 238, 122,

            169, 104, 121, 145,
            021, 178, 007, 063,
            148, 194, 016, 137,
            011, 034, 095, 033,

            128, 127, 093, 154,
            090, 144, 050, 039,
            053, 062, 204, 231,
            191, 247, 151, 003,

            255, 025, 048, 179,
            072, 165, 181, 209,
            215, 094, 146, 042,
            172, 086, 170, 198,

            079, 184, 056, 210,
            150, 164, 125, 182,
            118, 252, 107, 226,
            156, 116, 004, 241,

            069, 157, 112, 089,
            100, 113, 135, 032,
            134, 091, 207, 101,
            230, 045, 168, 002,

            027, 096, 037, 173,
            174, 176, 185, 246,
            028, 070, 097, 105,
            052, 064, 126, 015,

            085, 071, 163, 035,
            221, 081, 175, 058,
            195, 092, 249, 206,
            186, 197, 234, 038,

            044, 083, 013, 110,
            133, 040, 132, 009,
            211, 223, 205, 244,
            065, 129, 077, 082,

            106, 220, 055, 200,
            108, 193, 171, 250,
            036, 225, 123, 008,
            012, 189, 177, 074,

            120, 136, 149, 139,
            227, 099, 232, 109,
            233, 203, 213, 254,
            059, 000, 029, 057,

            242, 239, 183, 014,
            102, 088, 208, 228,
            166, 119, 114, 248,
            235, 117, 075, 010,

            049, 068, 080, 180,
            143, 237, 031, 026,
            219, 153, 141, 051,
            159, 017, 131, 020
        };

        private readonly byte[] state = new byte[16];

        private readonly byte[] buffer = new byte[48];

        private readonly byte[] finalBlock;

        private readonly byte[] checkSum;

        private int processedLength = 0;

        public MD2() : base(16)
        {
            HashSizeValue = 128;
            
            finalBlock = new byte[BlockSize];
            checkSum = new byte[BlockSize];
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();

            processedLength = 0;

            Array.Clear(finalBlock, 0, finalBlock.Length);
            Array.Clear(checkSum, 0, checkSum.Length);

            InitializeState();
        }

        protected override void ProcessBlock(byte[] array, int offset)
        {
            processedLength += BlockSize;

            // fill buffer

            Buffer.BlockCopy(state, 0, buffer, 0, 16);
            Buffer.BlockCopy(array, offset, buffer, 16, 16);
            for (int ii = 0; ii < 16; ii++)
            {
                buffer[ii + 32] = (byte)(state[ii] ^ array[offset + ii]);
            }

            // do 18 rounds

            uint t = 0;
            for (int ii = 0; ii < 18; ii++)
            {
                for (int jj = 0; jj < buffer.Length; jj++)
                {
                    buffer[jj] = (byte)(buffer[jj] ^ Pi[t]);
                    t = buffer[jj];
                }

                t = (uint)((t + ii) & 0xff); // % 256
            }

            Buffer.BlockCopy(buffer, 0, state, 0, 16);

            t = checkSum[15];
            for (int ii = 0; ii < checkSum.Length; ii++)
            {
                checkSum[ii] = (byte)(checkSum[ii] ^ Pi[array[offset + ii] ^ t]);
                t = checkSum[ii];
            }
        }

        protected override void ProcessFinalBlock(byte[] array, int offset, int length)
        {
            int messageLength = processedLength + length;

            Buffer.BlockCopy(array, offset, finalBlock, 0, length);

            // padding message
            for (int ii = offset + length; ii < BlockSize; ii++)
            {
                finalBlock[ii] = (byte)(16 - (messageLength & 0xf));
            }
            
            ProcessBlock(finalBlock, 0);
            ProcessBlock(checkSum, 0);
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

        private void InitializeState()
        {
            Array.Clear(state, 0, state.Length);
        }
    }

    public class MD2CryptoServiceProvider : MD2 { }
}
