using System;

namespace acryptohashnet
{
    public sealed class BigCounter
    {
        private readonly int sizeInBytes;
        private readonly uint[] array;

        public BigCounter(int sizeInBytes)
        {
            if (sizeInBytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes));
            }

            this.sizeInBytes = sizeInBytes;

            var sizeInUints = sizeInBytes / 4;
            if (sizeInBytes % 4 > 0)
            {
                sizeInUints += 1;
            }

            array = new uint[sizeInUints];
        }

        public byte[] GetBytes()
        {
            byte[] result = new byte[sizeInBytes];

            Buffer.BlockCopy(array, 0, result, 0, result.Length);

            return result;
        }

        public void Clear()
        {
            for(int ii = 0; ii < array.Length; ii++)
            {
                array[ii] = 0;
            }
        }

        public void Add(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Add((uint)value);
        }

        public void Add(uint value)
        {
            Add(0, value);
        }

        public void Add(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            Add(0, (uint)value);
            Add(1, (uint)(value >> 32));
        }

        public uint ToUInt32()
        {
            return array[0];
        }

        public ulong ToULong()
        {
            return (((ulong)array[1]) << 32) + array[0];
        }

        private void Add(int index, uint value)
        {
            if (index >= array.Length)
            {
                throw new OverflowException("Counter is not big enough");
            }

            var maxAllowedValue = uint.MaxValue - array[index];
            if (value > maxAllowedValue)
            {
                array[index] = value - (maxAllowedValue + 1);
                Add(index + 1, 1);
            }
            else
            {
                array[index] += value;
            }
        }
    }
}
