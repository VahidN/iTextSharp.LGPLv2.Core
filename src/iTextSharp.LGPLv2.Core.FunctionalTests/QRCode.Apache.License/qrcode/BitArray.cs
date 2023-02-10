using System;
using System.Text;

/*
 * Copyright 2007 ZXing authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace iTextSharp.text.pdf.qrcode;

/**
 * <p>A simple, fast array of bits, represented compactly by an array of ints internally.</p>
 * @author Sean Owen
 */
public sealed class BitArray
{
    // TODO: I have changed these members to be public so ProGuard can inline Get() and Set(). Ideally
    // they'd be private and we'd use the -allowaccessmodification flag, but Dalvik rejects the
    // resulting binary at runtime on Android. If we find a solution to this, these should be changed
    // back to private.
    public int[] bits;
    public int size;

    public BitArray(int size)
    {
        if (size < 1)
        {
            throw new ArgumentException("size must be at least 1");
        }

        this.size = size;
        bits = MakeArray(size);
    }

    public int GetSize() => size;

    /**
     * @param i bit to get
     * @return true iff bit i is set
     */
    public bool Get(int i) => (bits[i >> 5] & (1 << (i & 0x1F))) != 0;

    /**
     * Sets bit i.
     * 
     * @param i bit to set
     */
    public void Set(int i)
    {
        bits[i >> 5] |= 1 << (i & 0x1F);
    }

    /**
     * Flips bit i.
     * 
     * @param i bit to set
     */
    public void Flip(int i)
    {
        bits[i >> 5] ^= 1 << (i & 0x1F);
    }

    /**
     * Sets a block of 32 bits, starting at bit i.
     * 
     * @param i first bit to set
     * @param newBits the new value of the next 32 bits. Note again that the least-significant bit
     * corresponds to bit i, the next-least-significant to i+1, and so on.
     */
    public void SetBulk(int i, int newBits)
    {
        bits[i >> 5] = newBits;
    }

    /**
         * Clears all bits (sets to false).
         */
    public void Clear()
    {
        var max = bits.Length;
        for (var i = 0; i < max; i++)
        {
            bits[i] = 0;
        }
    }

    /**
     * Efficient method to check if a range of bits is set, or not set.
     * 
     * @param start start of range, inclusive.
     * @param end end of range, exclusive
     * @param value if true, checks that bits in range are set, otherwise checks that they are not set
     * @return true iff all bits are set or not set in range, according to value argument
     * @throws IllegalArgumentException if end is less than or equal to start
     */
    public bool IsRange(int start, int end, bool value)
    {
        if (end < start)
        {
            throw new ArgumentException();
        }

        if (end == start)
        {
            return true; // empty range matches
        }

        end--; // will be easier to treat this as the last actually set bit -- inclusive    
        var firstInt = start >> 5;
        var lastInt = end >> 5;
        for (var i = firstInt; i <= lastInt; i++)
        {
            var firstBit = i > firstInt ? 0 : start & 0x1F;
            var lastBit = i < lastInt ? 31 : end & 0x1F;
            int mask;
            if (firstBit == 0 && lastBit == 31)
            {
                mask = -1;
            }
            else
            {
                mask = 0;
                for (var j = firstBit; j <= lastBit; j++)
                {
                    mask |= 1 << j;
                }
            }

            // Return false if we're looking for 1s and the masked bits[i] isn't all 1s (that is,
            // equals the mask, or we're looking for 0s and the masked portion is not all 0s
            if ((bits[i] & mask) != (value ? mask : 0))
            {
                return false;
            }
        }

        return true;
    }

    /**
     * @return underlying array of ints. The first element holds the first 32 bits, and the least
     * significant bit is bit 0.
     */
    public int[] GetBitArray() => bits;

    /**
         * Reverses all bits in the array.
         */
    public void Reverse()
    {
        var newBits = new int[bits.Length];
        var size = this.size;
        for (var i = 0; i < size; i++)
        {
            if (Get(size - i - 1))
            {
                newBits[i >> 5] |= 1 << (i & 0x1F);
            }
        }

        bits = newBits;
    }

    private static int[] MakeArray(int size)
    {
        var arraySize = size >> 5;
        if ((size & 0x1F) != 0)
        {
            arraySize++;
        }

        return new int[arraySize];
    }

    public override string ToString()
    {
        var result = new StringBuilder(size);
        for (var i = 0; i < size; i++)
        {
            if ((i & 0x07) == 0)
            {
                result.Append(' ');
            }

            result.Append(Get(i) ? 'X' : '.');
        }

        return result.ToString();
    }
}