using System;

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
 *     See ISO 18004:2006, 6.5.1. This enum encapsulates the four error correction levels
 *     defined by the QR code standard.
 * </p>
 * @author Sean Owen
 */
public sealed class ErrorCorrectionLevel
{
    /**
         * L = ~7% correction
         */
    public static readonly ErrorCorrectionLevel L = new(0, 0x01, "L");

    /**
         * M = ~15% correction
         */
    public static readonly ErrorCorrectionLevel M = new(1, 0x00, "M");

    /**
         * Q = ~25% correction
         */
    public static readonly ErrorCorrectionLevel Q = new(2, 0x03, "Q");

    /**
         * H = ~30% correction
         */
    public static readonly ErrorCorrectionLevel H = new(3, 0x02, "H");

    private static readonly ErrorCorrectionLevel[] FOR_BITS = { M, L, H, Q };
    private readonly int bits;
    private readonly string name;

    private readonly int ordinal;

    private ErrorCorrectionLevel(int ordinal, int bits, string name)
    {
        this.ordinal = ordinal;
        this.bits = bits;
        this.name = name;
    }

    public int Ordinal() => ordinal;

    public int GetBits() => bits;

    public string GetName() => name;

    public override string ToString() => name;

    /**
     * @param bits int containing the two bits encoding a QR Code's error correction level
     * @return {@link ErrorCorrectionLevel} representing the encoded error correction level
     */
    public static ErrorCorrectionLevel ForBits(int bits)
    {
        if (bits < 0 || bits >= FOR_BITS.Length)
        {
            throw new IndexOutOfRangeException();
        }

        return FOR_BITS[bits];
    }
}