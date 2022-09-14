using System.Runtime.CompilerServices;

namespace acryptohashnet
{
    internal static class Bits
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateLeft(this uint x, int n)
        {
            return x << n | x >> 32 - n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateLeft(this ulong x, int n)
        {
            return x << n | x >> 32 - n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RotateRight(this uint x, int n)
        {
            return x >> n | x << 32 - n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RotateRight(this ulong x, int n)
        {
            return x >> n | x << 64 - n;
        }
    }
}
