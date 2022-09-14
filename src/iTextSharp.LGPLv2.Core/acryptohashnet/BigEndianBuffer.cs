using System;

namespace acryptohashnet
{
    public static class BigEndianBuffer
    {
        public static void BlockCopy(uint[] src, int srcOffset, byte[] dst, int dstOffset, int bytesCount)
        {
            if (srcOffset > src.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(srcOffset));
            }

            if (dstOffset > dst.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            }

            if (dstOffset + bytesCount > dst.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            }

            int uintsCount = bytesCount >> 2; // arg / 4
            int bytesRemaining = bytesCount & 0x3; // arg % 4

            if (srcOffset + uintsCount + (bytesRemaining > 0 ? 1 : 0) > src.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesCount));
            }

            var srcIndex = srcOffset;
            var dstIndex = dstOffset;
            while (dstIndex < dstOffset + bytesCount)
            {
                dst[dstIndex] =     (byte)(src[srcIndex] >> 24);
                dst[dstIndex + 1] = (byte)(src[srcIndex] >> 16);
                dst[dstIndex + 2] = (byte)(src[srcIndex] >> 8);
                dst[dstIndex + 3] = (byte)src[srcIndex];

                srcIndex += 1;
                dstIndex += 4;
            }

            for (int ii = 0; ii < bytesRemaining; ii++)
            {
                dst[dstIndex + ii] = (byte)(src[srcIndex] >> (24 - (ii << 3)));
            }
        }

        public static void BlockCopy(byte[] src, int srcOffset, uint[] dst, int dstOffset, int bytesCount)
        {
            if (srcOffset > src.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(srcOffset));
            }

            if (srcOffset + bytesCount > src.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesCount));
            }

            if (dstOffset > dst.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            }

            int uintsCount = bytesCount >> 2; // arg / 4
            int bytesRemaining = bytesCount & 0x3; // arg % 4

            if (dstOffset + uintsCount + (bytesRemaining > 0 ? 1 : 0) > dst.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesCount));
            }

            var srcIndex = srcOffset;
            var dstIndex = dstOffset;
            while (srcIndex < srcOffset + bytesCount)
            {
                dst[dstIndex] =  (uint)src[srcIndex + 0] << 24
                    | (uint)src[srcIndex + 1] << 16
                    | (uint)src[srcIndex + 2] << 8
                    | (uint)src[srcIndex + 3];

                srcIndex += 4;
                dstIndex += 1;
            }

            if (bytesRemaining > 0)
            {
                dst[dstIndex] = 0;
                for (int ii = 0; ii < bytesRemaining; ii++)
                {
                    dst[dstIndex] |= (uint)(src[srcIndex + ii]) << (24 - (ii << 3));
                }
            }
        }

        public static void BlockCopy(ulong[] src, int srcOffset, byte[] dst, int dstOffset, int bytesCount)
        {
            if (srcOffset > src.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(srcOffset));
            }

            if (dstOffset > dst.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            }

            if (dstOffset + bytesCount > dst.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesCount));
            }

            int ulongsCount = bytesCount >> 3; // arg / 8
            int bytesRemaining = bytesCount & 0x7; // arg % 8

            if (srcOffset + ulongsCount + (bytesRemaining > 0 ? 1 : 0) > src.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesCount));
            }

            var srcIndex = srcOffset;
            var dstIndex = dstOffset;
            while (dstIndex < dstOffset + bytesCount)
            {
                dst[dstIndex + 0] = (byte)(src[srcIndex] >> 56);
                dst[dstIndex + 1] = (byte)(src[srcIndex] >> 48);
                dst[dstIndex + 2] = (byte)(src[srcIndex] >> 40);
                dst[dstIndex + 3] = (byte)(src[srcIndex] >> 32);
                dst[dstIndex + 4] = (byte)(src[srcIndex] >> 24);
                dst[dstIndex + 5] = (byte)(src[srcIndex] >> 16);
                dst[dstIndex + 6] = (byte)(src[srcIndex] >> 8);
                dst[dstIndex + 7] = (byte)src[srcIndex];

                srcIndex += 1;
                dstIndex += 8;
            }

            for (int ii = 0; ii < bytesRemaining; ii++)
            {                
                dst[dstIndex + ii] = (byte)(src[srcIndex] >> (56 - (ii << 3)));
            }
        }

        public static void BlockCopy(byte[] src, int srcOffset, ulong[] dst, int dstOffset, int bytesCount)
        {
            if (srcOffset > src.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(srcOffset));
            }

            if (srcOffset + bytesCount > src.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesCount));
            }

            if (dstOffset > dst.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(dstOffset));
            }

            int ulongsCount = bytesCount >> 3; // arg / 8
            int bytesRemaining = bytesCount & 0x7; // arg % 8

            if (dstOffset + ulongsCount + (bytesRemaining > 0 ? 1 : 0) > dst.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesCount));
            }

            var srcIndex = srcOffset;
            var dstIndex = dstOffset;
            while (srcIndex < srcOffset + bytesCount)
            {
                dst[dstIndex] = (ulong)src[srcIndex + 0] << 56
                    | (ulong)src[srcIndex + 1] << 48
                    | (ulong)src[srcIndex + 2] << 40
                    | (ulong)src[srcIndex + 3] << 32
                    | (ulong)src[srcIndex + 4] << 24
                    | (ulong)src[srcIndex + 5] << 16
                    | (ulong)src[srcIndex + 6] << 8
                    | (ulong)src[srcIndex + 7];

                srcIndex += 8;
                dstIndex += 1;
            }

            if (bytesRemaining > 0)
            {
                dst[dstIndex] = 0;
                for (int ii = 0; ii < bytesRemaining; ii++)
                {
                    dst[dstIndex] |= (ulong)(src[srcIndex + ii]) << (56 - (ii << 3));
                }
            }
        }
    }
}
