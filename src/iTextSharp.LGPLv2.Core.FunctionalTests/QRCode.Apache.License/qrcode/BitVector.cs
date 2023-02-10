using System;
using System.Text;

/*
 * Copyright 2008 ZXing authors
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
 * JAVAPORT: This should be combined with BitArray in the future, although that class is not yet
 * dynamically resizeable. This implementation is reasonable but there is a lot of function calling
 * in loops I'd like to get rid of.
 * 
 * @author satorux@google.com (Satoru Takabayashi) - creator
 * @author dswitkin@google.com (Daniel Switkin) - ported from C++
 */
public sealed class BitVector
{
    // For efficiency, start out with some room to work.
    private const int DEFAULT_SIZE_IN_BYTES = 32;
    private byte[] array;

    private int sizeInBits;

    public BitVector()
    {
        sizeInBits = 0;
        array = new byte[DEFAULT_SIZE_IN_BYTES];
    }

    // Return the bit value at "index".
    public int At(int index)
    {
        if (index < 0 || index >= sizeInBits)
        {
            throw new IndexOutOfRangeException("Bad index: " + index);
        }

        var value = array[index >> 3] & 0xff;
        return (value >> (7 - (index & 0x7))) & 1;
    }

    // Return the number of bits in the bit vector.
    public int Size() => sizeInBits;

    // Return the number of bytes in the bit vector.
    public int SizeInBytes() => (sizeInBits + 7) >> 3;

    // Append one bit to the bit vector.
    public void AppendBit(int bit)
    {
        if (!(bit == 0 || bit == 1))
        {
            throw new ArgumentException("Bad bit");
        }

        var numBitsInLastByte = sizeInBits & 0x7;
        // We'll expand array if we don't have bits in the last byte.
        if (numBitsInLastByte == 0)
        {
            AppendByte(0);
            sizeInBits -= 8;
        }

        // Modify the last byte.
        array[sizeInBits >> 3] |= (byte)(bit << (7 - numBitsInLastByte));
        ++sizeInBits;
    }

    // Append "numBits" bits in "value" to the bit vector.
    // REQUIRES: 0<= numBits <= 32.
    //
    // Examples:
    // - AppendBits(0x00, 1) adds 0.
    // - AppendBits(0x00, 4) adds 0000.
    // - AppendBits(0xff, 8) adds 11111111.
    public void AppendBits(int value, int numBits)
    {
        if (numBits < 0 || numBits > 32)
        {
            throw new ArgumentException("Num bits must be between 0 and 32");
        }

        var numBitsLeft = numBits;
        while (numBitsLeft > 0)
        {
            // Optimization for byte-oriented appending.
            if ((sizeInBits & 0x7) == 0 && numBitsLeft >= 8)
            {
                var newByte = (value >> (numBitsLeft - 8)) & 0xff;
                AppendByte(newByte);
                numBitsLeft -= 8;
            }
            else
            {
                var bit = (value >> (numBitsLeft - 1)) & 1;
                AppendBit(bit);
                --numBitsLeft;
            }
        }
    }

    // Append "bits".
    public void AppendBitVector(BitVector bits)
    {
        var size = bits.Size();
        for (var i = 0; i < size; ++i)
        {
            AppendBit(bits.At(i));
        }
    }

    // Modify the bit vector by XOR'ing with "other"
    public void Xor(BitVector other)
    {
        if (sizeInBits != other.Size())
        {
            throw new ArgumentException("BitVector sizes don't match");
        }

        var sizeInBytes = (sizeInBits + 7) >> 3;
        for (var i = 0; i < sizeInBytes; ++i)
        {
            // The last byte could be incomplete (i.e. not have 8 bits in
            // it) but there is no problem since 0 XOR 0 == 0.
            array[i] ^= other.array[i];
        }
    }

    // Return String like "01110111" for debugging.
    public override string ToString()
    {
        var result = new StringBuilder(sizeInBits);
        for (var i = 0; i < sizeInBits; ++i)
        {
            if (At(i) == 0)
            {
                result.Append('0');
            }
            else if (At(i) == 1)
            {
                result.Append('1');
            }
            else
            {
                throw new ArgumentException("Byte isn't 0 or 1");
            }
        }

        return result.ToString();
    }

    // Callers should not assume that array.length is the exact number of bytes needed to hold
    // sizeInBits - it will typically be larger for efficiency.
    public byte[] GetArray() => array;

    // Add a new byte to the end, possibly reallocating and doubling the size of the array if we've
    // run out of room.
    private void AppendByte(int value)
    {
        if (sizeInBits >> 3 == array.Length)
        {
            var newArray = new byte[array.Length << 1];
            Array.Copy(array, 0, newArray, 0, array.Length);
            array = newArray;
        }

        array[sizeInBits >> 3] = (byte)value;
        sizeInBits += 8;
    }
}