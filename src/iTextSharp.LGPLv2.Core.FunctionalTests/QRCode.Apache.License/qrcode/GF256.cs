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
 *     This class contains utility methods for performing mathematical operations over
 *     the Galois Field GF(256). Operations use a given primitive polynomial in calculations.
 * </p>
 * <p>
 *     Throughout this package, elements of GF(256) are represented as an <code>int</code>
 *     for convenience and speed (but at the cost of memory).
 *     Only the bottom 8 bits are really used.
 * </p>
 * @author Sean Owen
 */
public sealed class GF256
{
    public static readonly GF256 QR_CODE_FIELD = new(0x011D); // x^8 + x^4 + x^3 + x^2 + 1
    public static readonly GF256 DATA_MATRIX_FIELD = new(0x012D); // x^8 + x^5 + x^3 + x^2 + 1

    private readonly int[] expTable;
    private readonly int[] logTable;
    private readonly GF256Poly one;
    private readonly GF256Poly zero;

    /**
     * Create a representation of GF(256) using the given primitive polynomial.
     * 
     * @param primitive irreducible polynomial whose coefficients are represented by
     * the bits of an int, where the least-significant bit represents the constant
     * coefficient
     */
    private GF256(int primitive)
    {
        expTable = new int[256];
        logTable = new int[256];
        var x = 1;
        for (var i = 0; i < 256; i++)
        {
            expTable[i] = x;
            x <<= 1; // x = x * 2; we're assuming the generator alpha is 2
            if (x >= 0x100)
            {
                x ^= primitive;
            }
        }

        for (var i = 0; i < 255; i++)
        {
            logTable[expTable[i]] = i;
        }

        // logTable[0] == 0 but this should never be used
        zero = new GF256Poly(this, new[] { 0 });
        one = new GF256Poly(this, new[] { 1 });
    }

    internal GF256Poly GetZero() => zero;

    internal GF256Poly GetOne() => one;

    /**
         * @return the monomial representing coefficient * x^degree
         */
    internal GF256Poly BuildMonomial(int degree, int coefficient)
    {
        if (degree < 0)
        {
            throw new ArgumentException();
        }

        if (coefficient == 0)
        {
            return zero;
        }

        var coefficients = new int[degree + 1];
        coefficients[0] = coefficient;
        return new GF256Poly(this, coefficients);
    }

    /**
     * Implements both addition and subtraction -- they are the same in GF(256).
     * 
     * @return sum/difference of a and b
     */
    internal static int AddOrSubtract(int a, int b) => a ^ b;

    /**
         * @return 2 to the power of a in GF(256)
         */
    internal int Exp(int a) => expTable[a];

    /**
         * @return base 2 log of a in GF(256)
         */
    internal int Log(int a)
    {
        if (a == 0)
        {
            throw new ArgumentException();
        }

        return logTable[a];
    }

    /**
         * @return multiplicative inverse of a
         */
    internal int Inverse(int a)
    {
        if (a == 0)
        {
            throw new ArithmeticException();
        }

        return expTable[255 - logTable[a]];
    }

    /**
     * @param a
     * @param b
     * @return product of a and b in GF(256)
     */
    internal int Multiply(int a, int b)
    {
        if (a == 0 || b == 0)
        {
            return 0;
        }

        if (a == 1)
        {
            return b;
        }

        if (b == 1)
        {
            return a;
        }

        return expTable[(logTable[a] + logTable[b]) % 255];
    }
}