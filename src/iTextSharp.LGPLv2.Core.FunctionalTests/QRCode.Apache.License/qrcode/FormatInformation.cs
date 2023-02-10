
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
 * <p>
 *     Encapsulates a QR Code's format information, including the data mask used and
 *     error correction level.
 * </p>
 * @author Sean Owen
 * @see ErrorCorrectionLevel
 */
public sealed class FormatInformation
{
    private const int FORMAT_INFO_MASK_QR = 0x5412;

    /**
         * See ISO 18004:2006, Annex C, Table C.1
         */
    private static readonly int[][] FORMAT_INFO_DECODE_LOOKUP =
    {
        new[] { 0x5412, 0x00 },
        new[] { 0x5125, 0x01 },
        new[] { 0x5E7C, 0x02 },
        new[] { 0x5B4B, 0x03 },
        new[] { 0x45F9, 0x04 },
        new[] { 0x40CE, 0x05 },
        new[] { 0x4F97, 0x06 },
        new[] { 0x4AA0, 0x07 },
        new[] { 0x77C4, 0x08 },
        new[] { 0x72F3, 0x09 },
        new[] { 0x7DAA, 0x0A },
        new[] { 0x789D, 0x0B },
        new[] { 0x662F, 0x0C },
        new[] { 0x6318, 0x0D },
        new[] { 0x6C41, 0x0E },
        new[] { 0x6976, 0x0F },
        new[] { 0x1689, 0x10 },
        new[] { 0x13BE, 0x11 },
        new[] { 0x1CE7, 0x12 },
        new[] { 0x19D0, 0x13 },
        new[] { 0x0762, 0x14 },
        new[] { 0x0255, 0x15 },
        new[] { 0x0D0C, 0x16 },
        new[] { 0x083B, 0x17 },
        new[] { 0x355F, 0x18 },
        new[] { 0x3068, 0x19 },
        new[] { 0x3F31, 0x1A },
        new[] { 0x3A06, 0x1B },
        new[] { 0x24B4, 0x1C },
        new[] { 0x2183, 0x1D },
        new[] { 0x2EDA, 0x1E },
        new[] { 0x2BED, 0x1F },
    };

    /**
         * Offset i holds the number of 1 bits in the binary representation of i
         */
    private static readonly int[] BITS_SET_IN_HALF_BYTE =
        { 0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4 };

    private readonly byte dataMask;

    private readonly ErrorCorrectionLevel errorCorrectionLevel;

    private FormatInformation(int formatInfo)
    {
        // Bits 3,4
        errorCorrectionLevel = ErrorCorrectionLevel.ForBits((formatInfo >> 3) & 0x03);
        // Bottom 3 bits
        dataMask = (byte)(formatInfo & 0x07);
    }

    public static int NumBitsDiffering(int a, int b)
    {
        a ^= b; // a now has a 1 bit exactly where its bit differs with b's
        // Count bits set quickly with a series of lookups:
        return BITS_SET_IN_HALF_BYTE[a & 0x0F] +
               BITS_SET_IN_HALF_BYTE[(a >> 4) & 0x0F] +
               BITS_SET_IN_HALF_BYTE[(a >> 8) & 0x0F] +
               BITS_SET_IN_HALF_BYTE[(a >> 12) & 0x0F] +
               BITS_SET_IN_HALF_BYTE[(a >> 16) & 0x0F] +
               BITS_SET_IN_HALF_BYTE[(a >> 20) & 0x0F] +
               BITS_SET_IN_HALF_BYTE[(a >> 24) & 0x0F] +
               BITS_SET_IN_HALF_BYTE[(a >> 28) & 0x0F];
    }

    /**
     * @param maskedFormatInfo1 format info indicator, with mask still applied
     * @param maskedFormatInfo2 second copy of same info; both are checked at the same time
     * to establish best match
     * @return information about the format it specifies, or
     * <code>null</code>
     * if doesn't seem to match any known pattern
     */
    public static FormatInformation DecodeFormatInformation(int maskedFormatInfo1, int maskedFormatInfo2)
    {
        var formatInfo = DoDecodeFormatInformation(maskedFormatInfo1, maskedFormatInfo2);
        if (formatInfo != null)
        {
            return formatInfo;
        }

        // Should return null, but, some QR codes apparently
        // do not mask this info. Try again by actually masking the pattern
        // first
        return DoDecodeFormatInformation(maskedFormatInfo1 ^ FORMAT_INFO_MASK_QR,
                                         maskedFormatInfo2 ^ FORMAT_INFO_MASK_QR);
    }

    private static FormatInformation DoDecodeFormatInformation(int maskedFormatInfo1, int maskedFormatInfo2)
    {
        // Find the int in FORMAT_INFO_DECODE_LOOKUP with fewest bits differing
        var bestDifference = int.MaxValue;
        var bestFormatInfo = 0;
        for (var i = 0; i < FORMAT_INFO_DECODE_LOOKUP.GetLength(0); i++)
        {
            var decodeInfo = FORMAT_INFO_DECODE_LOOKUP[i];
            var targetInfo = decodeInfo[0];
            if (targetInfo == maskedFormatInfo1 || targetInfo == maskedFormatInfo2)
            {
                // Found an exact match
                return new FormatInformation(decodeInfo[1]);
            }

            var bitsDifference = NumBitsDiffering(maskedFormatInfo1, targetInfo);
            if (bitsDifference < bestDifference)
            {
                bestFormatInfo = decodeInfo[1];
                bestDifference = bitsDifference;
            }

            if (maskedFormatInfo1 != maskedFormatInfo2)
            {
                // also try the other option
                bitsDifference = NumBitsDiffering(maskedFormatInfo2, targetInfo);
                if (bitsDifference < bestDifference)
                {
                    bestFormatInfo = decodeInfo[1];
                    bestDifference = bitsDifference;
                }
            }
        }

        // Hamming distance of the 32 masked codes is 7, by construction, so <= 3 bits
        // differing means we found a match
        if (bestDifference <= 3)
        {
            return new FormatInformation(bestFormatInfo);
        }

        return null;
    }

    public ErrorCorrectionLevel GetErrorCorrectionLevel() => errorCorrectionLevel;

    public byte GetDataMask() => dataMask;

    public int HashCode() => (errorCorrectionLevel.Ordinal() << 3) | dataMask;

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object o)
    {
        if (!(o is FormatInformation))
        {
            return false;
        }

        var other = (FormatInformation)o;
        return errorCorrectionLevel == other.errorCorrectionLevel &&
               dataMask == other.dataMask;
    }
}