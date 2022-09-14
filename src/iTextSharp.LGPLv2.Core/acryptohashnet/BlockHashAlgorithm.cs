using System;
using System.Security.Cryptography;

namespace acryptohashnet
{
    /// <summary>
    /// Represents the base class from which all implementation of block hash algorithms must derive.
    /// </summary>
    public abstract class BlockHashAlgorithm : HashAlgorithm
    {
        private readonly byte[] lastBlock;

        private int lastBlockLength;

        /// <summary>
        /// Block hash algorithm ctor.
        /// </summary>
        /// <param name="blockSize">size of the block for algorithm in bytes</param>
        public BlockHashAlgorithm(int blockSize)
        {
            BlockSize = blockSize;

            lastBlock = new byte[BlockSize];
            lastBlockLength = 0;
        }

        /// <summary>
        /// Size of algorithm block in bytes.
        /// </summary>
        public int BlockSize { get; }

        /// <summary>
        /// Initialization algorithm variables.
        /// </summary>
        public override void Initialize()
        {
            Array.Clear(lastBlock, 0, lastBlock.Length);
            lastBlockLength = 0;
        }

        /// <summary>
        /// Processing block of bytes (size is @BlockSize), @array length must be >= than @offset + @BlockSize
        /// </summary>
        /// <param name="array">array of bytes</param>
        /// <param name="offset">offset from begin of block in @array</param>
        protected abstract void ProcessBlock(byte[] array, int offset);

        /// <summary>
        /// Processing final block of bytes (size is @length), @array length must be >= than @offset + @length
        /// </summary>
        /// <param name="array">array of bytes</param>
        /// <param name="offset">offset from begin of block in @array</param>
        /// <param name="length">length of final block</param>
        protected abstract void ProcessFinalBlock(byte[] array, int offset, int length);

        /// <summary>
        /// Resulting value of algorithm
        /// </summary>
        /// <value>byte array with hash value</value>
        protected abstract byte[] Result { get; }

        /// <summary>
        /// Main hash procedure.
        /// </summary>
        /// <param name="array">byte array</param>
        /// <param name="offset">offset in array</param>
        /// <param name="length">length of block for processing</param>
        protected override void HashCore(byte[] array, int offset, int length)
        {
            if (lastBlockLength > 0)
            {
                int lastBlockRemaining = BlockSize - lastBlockLength;
                if (length >= lastBlockRemaining)
                {
                    Buffer.BlockCopy(array, offset, lastBlock, lastBlockLength, lastBlockRemaining);

                    ProcessBlock(lastBlock, 0);
                    offset += lastBlockRemaining;
                    length -= lastBlockRemaining;

                    lastBlockLength = 0;
                }
            }

            while (length >= BlockSize)
            {
                ProcessBlock(array, offset);
                offset += BlockSize;
                length -= BlockSize;
            }

            if (length > 0)
            {
                Buffer.BlockCopy(array, offset, lastBlock, lastBlockLength, length);
                lastBlockLength += length;
            }
        }

        /// <summary>
        /// Hash final block.
        /// </summary>
        /// <returns>hash value</returns>
        protected override byte[] HashFinal()
        {
            if (lastBlockLength > lastBlock.Length)
            {
                throw new InvalidOperationException("lastBlockLength > lastBlock.Length");
            }

            ProcessFinalBlock(lastBlock, 0, lastBlockLength);
            return Result;
        }
    }
}
