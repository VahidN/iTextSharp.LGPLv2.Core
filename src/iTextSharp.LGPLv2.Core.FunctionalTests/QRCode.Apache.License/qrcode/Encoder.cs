using System;
using System.Collections.Generic;
using System.Text;
using System.util;

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
 * @author satorux@google.com (Satoru Takabayashi) - creator
 * @author dswitkin@google.com (Daniel Switkin) - ported from C++
 */
public sealed class Encoder
{
    private const string DEFAULT_BYTE_MODE_ENCODING = "ISO-8859-1";

    // The original table is defined in the table 5 of JISX0510:2004 (p.19).
    private static readonly int[] ALPHANUMERIC_TABLE =
    {
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, // 0x00-0x0f
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, // 0x10-0x1f
        36, -1, -1, -1, 37, 38, -1, -1, -1, -1, 39, 40, -1, 41, 42,
        43, // 0x20-0x2f
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 44, -1, -1, -1, -1,
        -1, // 0x30-0x3f
        -1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23,
        24, // 0x40-0x4f
        25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1,
        -1, // 0x50-0x5f
    };

    private Encoder()
    {
    }

    // The mask penalty calculation is complicated.  See Table 21 of JISX0510:2004 (p.45) for details.
    // Basically it applies four rules and summate all penalties.
    private static int CalculateMaskPenalty(ByteMatrix matrix)
    {
        var penalty = 0;
        penalty += MaskUtil.ApplyMaskPenaltyRule1(matrix);
        penalty += MaskUtil.ApplyMaskPenaltyRule2(matrix);
        penalty += MaskUtil.ApplyMaskPenaltyRule3(matrix);
        penalty += MaskUtil.ApplyMaskPenaltyRule4(matrix);
        return penalty;
    }

    /**
     * Encode "bytes" with the error correction level "ecLevel". The encoding mode will be chosen
     * internally by ChooseMode(). On success, store the result in "qrCode".
     * 
     * We recommend you to use QRCode.EC_LEVEL_L (the lowest level) for
     * "getECLevel" since our primary use is to show QR code on desktop screens. We don't need very
     * strong error correction for this purpose.
     * 
     * Note that there is no way to encode bytes in MODE_KANJI. We might want to add EncodeWithMode()
     * with which clients can specify the encoding mode. For now, we don't need the functionality.
     */
    public static void Encode(string content, ErrorCorrectionLevel ecLevel, QRCode qrCode)
    {
        Encode(content, ecLevel, null, qrCode);
    }

    public static void Encode(string content,
                              ErrorCorrectionLevel ecLevel,
                              INullValueDictionary<EncodeHintType, object> hints,
                              QRCode qrCode)
    {
        string encoding = null;
        if (hints != null && hints.TryGetValue(EncodeHintType.CHARACTER_SET, out var hint))
        {
            encoding = (string)hint;
        }

        if (encoding == null)
        {
            encoding = DEFAULT_BYTE_MODE_ENCODING;
        }

        // Step 1: Choose the mode (encoding).
        var mode = ChooseMode(content, encoding);

        // Step 2: Append "bytes" into "dataBits" in appropriate encoding.
        var dataBits = new BitVector();
        AppendBytes(content, mode, dataBits, encoding);
        // Step 3: Initialize QR code that can contain "dataBits".
        var numInputBytes = dataBits.SizeInBytes();
        InitQRCode(numInputBytes, ecLevel, mode, qrCode);

        // Step 4: Build another bit vector that contains header and data.
        var headerAndDataBits = new BitVector();

        // Step 4.5: Append ECI message if applicable
        if (mode == Mode.BYTE && !DEFAULT_BYTE_MODE_ENCODING.Equals(encoding))
        {
            var eci = CharacterSetECI.GetCharacterSetECIByName(encoding);
            if (eci != null)
            {
                AppendECI(eci, headerAndDataBits);
            }
        }

        AppendModeInfo(mode, headerAndDataBits);

        var numLetters = mode.Equals(Mode.BYTE) ? dataBits.SizeInBytes() : content.Length;
        AppendLengthInfo(numLetters, qrCode.GetVersion(), mode, headerAndDataBits);
        headerAndDataBits.AppendBitVector(dataBits);

        // Step 5: Terminate the bits properly.
        TerminateBits(qrCode.GetNumDataBytes(), headerAndDataBits);

        // Step 6: Interleave data bits with error correction code.
        var finalBits = new BitVector();
        InterleaveWithECBytes(headerAndDataBits,
                              qrCode.GetNumTotalBytes(),
                              qrCode.GetNumDataBytes(),
                              qrCode.GetNumRSBlocks(),
                              finalBits);

        // Step 7: Choose the mask pattern and set to "qrCode".
        var matrix = new ByteMatrix(qrCode.GetMatrixWidth(), qrCode.GetMatrixWidth());
        qrCode.SetMaskPattern(ChooseMaskPattern(finalBits,
                                                qrCode.GetECLevel(),
                                                qrCode.GetVersion(),
                                                matrix));

        // Step 8.  Build the matrix and set it to "qrCode".
        MatrixUtil.BuildMatrix(finalBits,
                               qrCode.GetECLevel(),
                               qrCode.GetVersion(),
                               qrCode.GetMaskPattern(),
                               matrix);
        qrCode.SetMatrix(matrix);
        // Step 9.  Make sure we have a valid QR Code.
        if (!qrCode.IsValid())
        {
            throw new WriterException("Invalid QR code: " + qrCode);
        }
    }

    /**
     * @return the code point of the table used in alphanumeric mode or
     * -1 if there is no corresponding code in the table.
     */
    private static int GetAlphanumericCode(int code)
    {
        if (code < ALPHANUMERIC_TABLE.Length)
        {
            return ALPHANUMERIC_TABLE[code];
        }

        return -1;
    }

    public static Mode ChooseMode(string content) => ChooseMode(content, null);

    /**
     * Choose the best mode by examining the content. Note that 'encoding' is used as a hint;
     * if it is Shift_JIS, and the input is only double-byte Kanji, then we return {@link Mode#KANJI}.
     */
    public static Mode ChooseMode(string content, string encoding)
    {
        if ("Shift_JIS".Equals(encoding))
        {
            // Choose Kanji mode if all input are double-byte characters
            return IsOnlyDoubleByteKanji(content) ? Mode.KANJI : Mode.BYTE;
        }

        var hasNumeric = false;
        var hasAlphanumeric = false;
        for (var i = 0; i < content.Length; ++i)
        {
            var c = content[i];
            if (c >= '0' && c <= '9')
            {
                hasNumeric = true;
            }
            else if (GetAlphanumericCode(c) != -1)
            {
                hasAlphanumeric = true;
            }
            else
            {
                return Mode.BYTE;
            }
        }

        if (hasAlphanumeric)
        {
            return Mode.ALPHANUMERIC;
        }

        if (hasNumeric)
        {
            return Mode.NUMERIC;
        }

        return Mode.BYTE;
    }

    private static bool IsOnlyDoubleByteKanji(string content)
    {
        byte[] bytes;
        try
        {
            bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(content);
        }
        catch
        {
            return false;
        }

        var length = bytes.Length;
        if (length % 2 != 0)
        {
            return false;
        }

        for (var i = 0; i < length; i += 2)
        {
            var byte1 = bytes[i] & 0xFF;
            if ((byte1 < 0x81 || byte1 > 0x9F) && (byte1 < 0xE0 || byte1 > 0xEB))
            {
                return false;
            }
        }

        return true;
    }

    private static int ChooseMaskPattern(BitVector bits,
                                         ErrorCorrectionLevel ecLevel,
                                         int version,
                                         ByteMatrix matrix)
    {
        var minPenalty = int.MaxValue; // Lower penalty is better.
        var bestMaskPattern = -1;
        // We try all mask patterns to choose the best one.
        for (var maskPattern = 0; maskPattern < QRCode.NUM_MASK_PATTERNS; maskPattern++)
        {
            MatrixUtil.BuildMatrix(bits, ecLevel, version, maskPattern, matrix);
            var penalty = CalculateMaskPenalty(matrix);
            if (penalty < minPenalty)
            {
                minPenalty = penalty;
                bestMaskPattern = maskPattern;
            }
        }

        return bestMaskPattern;
    }

    /**
     * Initialize "qrCode" according to "numInputBytes", "ecLevel", and "mode". On success,
     * modify "qrCode".
     */
    private static void InitQRCode(int numInputBytes,
                                   ErrorCorrectionLevel ecLevel,
                                   Mode mode,
                                   QRCode qrCode)
    {
        qrCode.SetECLevel(ecLevel);
        qrCode.SetMode(mode);

        // In the following comments, we use numbers of Version 7-H.
        for (var versionNum = 1; versionNum <= 40; versionNum++)
        {
            var version = Version.GetVersionForNumber(versionNum);
            // numBytes = 196
            var numBytes = version.GetTotalCodewords();
            // getNumECBytes = 130
            var ecBlocks = version.GetECBlocksForLevel(ecLevel);
            var numEcBytes = ecBlocks.GetTotalECCodewords();
            // getNumRSBlocks = 5
            var numRSBlocks = ecBlocks.GetNumBlocks();
            // getNumDataBytes = 196 - 130 = 66
            var numDataBytes = numBytes - numEcBytes;
            // We want to choose the smallest version which can contain data of "numInputBytes" + some
            // extra bits for the header (mode info and length info). The header can be three bytes
            // (precisely 4 + 16 bits) at most. Hence we do +3 here.
            if (numDataBytes >= numInputBytes + 3)
            {
                // Yay, we found the proper rs block info!
                qrCode.SetVersion(versionNum);
                qrCode.SetNumTotalBytes(numBytes);
                qrCode.SetNumDataBytes(numDataBytes);
                qrCode.SetNumRSBlocks(numRSBlocks);
                // getNumECBytes = 196 - 66 = 130
                qrCode.SetNumECBytes(numEcBytes);
                // matrix width = 21 + 6 * 4 = 45
                qrCode.SetMatrixWidth(version.GetDimensionForVersion());
                return;
            }
        }

        throw new WriterException("Cannot find proper rs block info (input data too big?)");
    }

    /**
         * Terminate bits as described in 8.4.8 and 8.4.9 of JISX0510:2004 (p.24).
         */
    private static void TerminateBits(int numDataBytes, BitVector bits)
    {
        var capacity = numDataBytes << 3;
        if (bits.Size() > capacity)
        {
            throw new WriterException("data bits cannot fit in the QR Code" + bits.Size() + " > " +
                                      capacity);
        }

        // Append termination bits. See 8.4.8 of JISX0510:2004 (p.24) for details.
        // TODO: srowen says we can remove this for loop, since the 4 terminator bits are optional if
        // the last byte has less than 4 bits left. So it amounts to padding the last byte with zeroes
        // either way.
        for (var i = 0; i < 4 && bits.Size() < capacity; ++i)
        {
            bits.AppendBit(0);
        }

        var numBitsInLastByte = bits.Size() % 8;
        // If the last byte isn't 8-bit aligned, we'll add padding bits.
        if (numBitsInLastByte > 0)
        {
            var numPaddingBits = 8 - numBitsInLastByte;
            for (var i = 0; i < numPaddingBits; ++i)
            {
                bits.AppendBit(0);
            }
        }

        // Should be 8-bit aligned here.
        if (bits.Size() % 8 != 0)
        {
            throw new WriterException("Number of bits is not a multiple of 8");
        }

        // If we have more space, we'll fill the space with padding patterns defined in 8.4.9 (p.24).
        var numPaddingBytes = numDataBytes - bits.SizeInBytes();
        for (var i = 0; i < numPaddingBytes; ++i)
        {
            if (i % 2 == 0)
            {
                bits.AppendBits(0xec, 8);
            }
            else
            {
                bits.AppendBits(0x11, 8);
            }
        }

        if (bits.Size() != capacity)
        {
            throw new WriterException("Bits size does not equal capacity");
        }
    }

    /**
     * Get number of data bytes and number of error correction bytes for block id "blockID". Store
     * the result in "numDataBytesInBlock", and "numECBytesInBlock". See table 12 in 8.5.1 of
     * JISX0510:2004 (p.30)
     */
    private static void GetNumDataBytesAndNumECBytesForBlockID(int numTotalBytes,
                                                               int numDataBytes,
                                                               int numRSBlocks,
                                                               int blockID,
                                                               int[] numDataBytesInBlock,
                                                               int[] numECBytesInBlock)
    {
        if (blockID >= numRSBlocks)
        {
            throw new WriterException("Block ID too large");
        }

        // numRsBlocksInGroup2 = 196 % 5 = 1
        var numRsBlocksInGroup2 = numTotalBytes % numRSBlocks;
        // numRsBlocksInGroup1 = 5 - 1 = 4
        var numRsBlocksInGroup1 = numRSBlocks - numRsBlocksInGroup2;
        // numTotalBytesInGroup1 = 196 / 5 = 39
        var numTotalBytesInGroup1 = numTotalBytes / numRSBlocks;
        // numTotalBytesInGroup2 = 39 + 1 = 40
        var numTotalBytesInGroup2 = numTotalBytesInGroup1 + 1;
        // numDataBytesInGroup1 = 66 / 5 = 13
        var numDataBytesInGroup1 = numDataBytes / numRSBlocks;
        // numDataBytesInGroup2 = 13 + 1 = 14
        var numDataBytesInGroup2 = numDataBytesInGroup1 + 1;
        // numEcBytesInGroup1 = 39 - 13 = 26
        var numEcBytesInGroup1 = numTotalBytesInGroup1 - numDataBytesInGroup1;
        // numEcBytesInGroup2 = 40 - 14 = 26
        var numEcBytesInGroup2 = numTotalBytesInGroup2 - numDataBytesInGroup2;
        // Sanity checks.
        // 26 = 26
        if (numEcBytesInGroup1 != numEcBytesInGroup2)
        {
            throw new WriterException("EC bytes mismatch");
        }

        // 5 = 4 + 1.
        if (numRSBlocks != numRsBlocksInGroup1 + numRsBlocksInGroup2)
        {
            throw new WriterException("RS blocks mismatch");
        }

        // 196 = (13 + 26) * 4 + (14 + 26) * 1
        if (numTotalBytes !=
            (numDataBytesInGroup1 + numEcBytesInGroup1) *
            numRsBlocksInGroup1 +
            (numDataBytesInGroup2 + numEcBytesInGroup2) *
            numRsBlocksInGroup2)
        {
            throw new WriterException("Total bytes mismatch");
        }

        if (blockID < numRsBlocksInGroup1)
        {
            numDataBytesInBlock[0] = numDataBytesInGroup1;
            numECBytesInBlock[0] = numEcBytesInGroup1;
        }
        else
        {
            numDataBytesInBlock[0] = numDataBytesInGroup2;
            numECBytesInBlock[0] = numEcBytesInGroup2;
        }
    }

    /**
     * Interleave "bits" with corresponding error correction bytes. On success, store the result in
     * "result". The interleave rule is complicated. See 8.6 of JISX0510:2004 (p.37) for details.
     */
    private static void InterleaveWithECBytes(BitVector bits,
                                              int numTotalBytes,
                                              int numDataBytes,
                                              int numRSBlocks,
                                              BitVector result)
    {
        // "bits" must have "getNumDataBytes" bytes of data.
        if (bits.SizeInBytes() != numDataBytes)
        {
            throw new WriterException("Number of bits and data bytes does not match");
        }

        // Step 1.  Divide data bytes into blocks and generate error correction bytes for them. We'll
        // store the divided data bytes blocks and error correction bytes blocks into "blocks".
        var dataBytesOffset = 0;
        var maxNumDataBytes = 0;
        var maxNumEcBytes = 0;

        // Since, we know the number of reedsolmon blocks, we can initialize the vector with the number.
        var blocks = new List<BlockPair>(numRSBlocks);

        for (var i = 0; i < numRSBlocks; ++i)
        {
            var numDataBytesInBlock = new int[1];
            var numEcBytesInBlock = new int[1];
            GetNumDataBytesAndNumECBytesForBlockID(
                                                   numTotalBytes,
                                                   numDataBytes,
                                                   numRSBlocks,
                                                   i,
                                                   numDataBytesInBlock,
                                                   numEcBytesInBlock);

            var dataBytes = new ByteArray();
            dataBytes.Set(bits.GetArray(), dataBytesOffset, numDataBytesInBlock[0]);
            var ecBytes = GenerateECBytes(dataBytes, numEcBytesInBlock[0]);
            blocks.Add(new BlockPair(dataBytes, ecBytes));

            maxNumDataBytes = Math.Max(maxNumDataBytes, dataBytes.Size());
            maxNumEcBytes = Math.Max(maxNumEcBytes, ecBytes.Size());
            dataBytesOffset += numDataBytesInBlock[0];
        }

        if (numDataBytes != dataBytesOffset)
        {
            throw new WriterException("Data bytes does not match offset");
        }

        // First, place data blocks.
        for (var i = 0; i < maxNumDataBytes; ++i)
        {
            for (var j = 0; j < blocks.Count; ++j)
            {
                var dataBytes = blocks[j].GetDataBytes();
                if (i < dataBytes.Size())
                {
                    result.AppendBits(dataBytes.At(i), 8);
                }
            }
        }

        // Then, place error correction blocks.
        for (var i = 0; i < maxNumEcBytes; ++i)
        {
            for (var j = 0; j < blocks.Count; ++j)
            {
                var ecBytes = blocks[j].GetErrorCorrectionBytes();
                if (i < ecBytes.Size())
                {
                    result.AppendBits(ecBytes.At(i), 8);
                }
            }
        }

        if (numTotalBytes != result.SizeInBytes())
        {
            // Should be same.
            throw new WriterException("Interleaving error: " + numTotalBytes + " and " +
                                      result.SizeInBytes() + " differ.");
        }
    }

    private static ByteArray GenerateECBytes(ByteArray dataBytes, int numEcBytesInBlock)
    {
        var numDataBytes = dataBytes.Size();
        var toEncode = new int[numDataBytes + numEcBytesInBlock];
        for (var i = 0; i < numDataBytes; i++)
        {
            toEncode[i] = dataBytes.At(i);
        }

        new ReedSolomonEncoder(GF256.QR_CODE_FIELD).Encode(toEncode, numEcBytesInBlock);

        var ecBytes = new ByteArray(numEcBytesInBlock);
        for (var i = 0; i < numEcBytesInBlock; i++)
        {
            ecBytes.Set(i, toEncode[numDataBytes + i]);
        }

        return ecBytes;
    }

    /**
         * Append mode info. On success, store the result in "bits".
         */
    private static void AppendModeInfo(Mode mode, BitVector bits)
    {
        bits.AppendBits(mode.GetBits(), 4);
    }


    /**
         * Append length info. On success, store the result in "bits".
         */
    private static void AppendLengthInfo(int numLetters, int version, Mode mode, BitVector bits)
    {
        var numBits = mode.GetCharacterCountBits(Version.GetVersionForNumber(version));
        if (numLetters > (1 << numBits) - 1)
        {
            throw new WriterException(numLetters + "is bigger than" + ((1 << numBits) - 1));
        }

        bits.AppendBits(numLetters, numBits);
    }

    /**
         * Append "bytes" in "mode" mode (encoding) into "bits". On success, store the result in "bits".
         */
    private static void AppendBytes(string content, Mode mode, BitVector bits, string encoding)
    {
        if (mode.Equals(Mode.NUMERIC))
        {
            AppendNumericBytes(content, bits);
        }
        else if (mode.Equals(Mode.ALPHANUMERIC))
        {
            AppendAlphanumericBytes(content, bits);
        }
        else if (mode.Equals(Mode.BYTE))
        {
            Append8BitBytes(content, bits, encoding);
        }
        else if (mode.Equals(Mode.KANJI))
        {
            AppendKanjiBytes(content, bits);
        }
        else
        {
            throw new WriterException("Invalid mode: " + mode);
        }
    }

    private static void AppendNumericBytes(string content, BitVector bits)
    {
        var length = content.Length;
        var i = 0;
        while (i < length)
        {
            var num1 = content[i] - '0';
            if (i + 2 < length)
            {
                // Encode three numeric letters in ten bits.
                var num2 = content[i + 1] - '0';
                var num3 = content[i + 2] - '0';
                bits.AppendBits(num1 * 100 + num2 * 10 + num3, 10);
                i += 3;
            }
            else if (i + 1 < length)
            {
                // Encode two numeric letters in seven bits.
                var num2 = content[i + 1] - '0';
                bits.AppendBits(num1 * 10 + num2, 7);
                i += 2;
            }
            else
            {
                // Encode one numeric letter in four bits.
                bits.AppendBits(num1, 4);
                i++;
            }
        }
    }

    private static void AppendAlphanumericBytes(string content, BitVector bits)
    {
        var length = content.Length;
        var i = 0;
        while (i < length)
        {
            var code1 = GetAlphanumericCode(content[i]);
            if (code1 == -1)
            {
                throw new WriterException();
            }

            if (i + 1 < length)
            {
                var code2 = GetAlphanumericCode(content[i + 1]);
                if (code2 == -1)
                {
                    throw new WriterException();
                }

                // Encode two alphanumeric letters in 11 bits.
                bits.AppendBits(code1 * 45 + code2, 11);
                i += 2;
            }
            else
            {
                // Encode one alphanumeric letter in six bits.
                bits.AppendBits(code1, 6);
                i++;
            }
        }
    }

    private static void Append8BitBytes(string content, BitVector bits, string encoding)
    {
        byte[] bytes;
        try
        {
            bytes = Encoding.GetEncoding(encoding).GetBytes(content);
        }
        catch (Exception uee)
        {
            throw new WriterException(uee.Message);
        }

        for (var i = 0; i < bytes.Length; ++i)
        {
            bits.AppendBits(bytes[i], 8);
        }
    }

    private static void AppendKanjiBytes(string content, BitVector bits)
    {
        byte[] bytes;
        try
        {
            bytes = Encoding.GetEncoding("Shift_JIS").GetBytes(content);
        }
        catch (Exception uee)
        {
            throw new WriterException(uee.Message);
        }

        var length = bytes.Length;
        for (var i = 0; i < length; i += 2)
        {
            var byte1 = bytes[i] & 0xFF;
            var byte2 = bytes[i + 1] & 0xFF;
            var code = (byte1 << 8) | byte2;
            var subtracted = -1;
            if (code >= 0x8140 && code <= 0x9ffc)
            {
                subtracted = code - 0x8140;
            }
            else if (code >= 0xe040 && code <= 0xebbf)
            {
                subtracted = code - 0xc140;
            }

            if (subtracted == -1)
            {
                throw new WriterException("Invalid byte sequence");
            }

            var encoded = (subtracted >> 8) * 0xc0 + (subtracted & 0xff);
            bits.AppendBits(encoded, 13);
        }
    }

    private static void AppendECI(CharacterSetECI eci, BitVector bits)
    {
        bits.AppendBits(Mode.ECI.GetBits(), 4);
        // This is correct for values up to 127, which is all we need now.
        bits.AppendBits(eci.GetValue(), 8);
    }
}