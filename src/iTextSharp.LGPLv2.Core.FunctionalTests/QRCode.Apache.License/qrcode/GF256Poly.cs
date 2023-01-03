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
 * <p>
 *     Represents a polynomial whose coefficients are elements of GF(256).
 *     Instances of this class are immutable.
 * </p>
 * <p>
 *     Much credit is due to William Rucklidge since portions of this code are an indirect
 *     port of his C++ Reed-Solomon implementation.
 * </p>
 * @author Sean Owen
 */
internal sealed class GF256Poly
{
    private readonly int[] coefficients;

    private readonly GF256 field;

    /**
     * @param field the {@link GF256} instance representing the field to use
     * to perform computations
     * @param coefficients coefficients as ints representing elements of GF(256), arranged
     * from most significant (highest-power term) coefficient to least significant
     * @throws IllegalArgumentException if argument is null or empty,
     * or if leading coefficient is 0 and this is not a
     * constant polynomial (that is, it is not the monomial "0")
     */
    internal GF256Poly(GF256 field, int[] coefficients)
    {
        if (coefficients == null || coefficients.Length == 0)
        {
            throw new ArgumentException();
        }

        this.field = field;
        var coefficientsLength = coefficients.Length;
        if (coefficientsLength > 1 && coefficients[0] == 0)
        {
            // Leading term must be non-zero for anything except the constant polynomial "0"
            var firstNonZero = 1;
            while (firstNonZero < coefficientsLength && coefficients[firstNonZero] == 0)
            {
                firstNonZero++;
            }

            if (firstNonZero == coefficientsLength)
            {
                this.coefficients = field.GetZero().coefficients;
            }
            else
            {
                this.coefficients = new int[coefficientsLength - firstNonZero];
                Array.Copy(coefficients,
                           firstNonZero,
                           this.coefficients,
                           0,
                           this.coefficients.Length);
            }
        }
        else
        {
            this.coefficients = coefficients;
        }
    }

    internal int[] GetCoefficients() => coefficients;

    /**
         * @return degree of this polynomial
         */
    internal int GetDegree() => coefficients.Length - 1;

    /**
         * @return true iff this polynomial is the monomial "0"
         */
    internal bool IsZero() => coefficients[0] == 0;

    /**
         * @return coefficient of x^degree term in this polynomial
         */
    internal int GetCoefficient(int degree) => coefficients[coefficients.Length - 1 - degree];

    /**
         * @return evaluation of this polynomial at a given point
         */
    internal int EvaluateAt(int a)
    {
        if (a == 0)
        {
            // Just return the x^0 coefficient
            return GetCoefficient(0);
        }

        var size = coefficients.Length;
        if (a == 1)
        {
            // Just the sum of the coefficients
            var result2 = 0;
            for (var i = 0; i < size; i++)
            {
                result2 = GF256.AddOrSubtract(result2, coefficients[i]);
            }

            return result2;
        }

        var result = coefficients[0];
        for (var i = 1; i < size; i++)
        {
            result = GF256.AddOrSubtract(field.Multiply(a, result), coefficients[i]);
        }

        return result;
    }

    internal GF256Poly AddOrSubtract(GF256Poly other)
    {
        if (!field.Equals(other.field))
        {
            throw new ArgumentException("GF256Polys do not have same GF256 field");
        }

        if (IsZero())
        {
            return other;
        }

        if (other.IsZero())
        {
            return this;
        }

        var smallerCoefficients = coefficients;
        var largerCoefficients = other.coefficients;
        if (smallerCoefficients.Length > largerCoefficients.Length)
        {
            var temp = smallerCoefficients;
            smallerCoefficients = largerCoefficients;
            largerCoefficients = temp;
        }

        var sumDiff = new int[largerCoefficients.Length];
        var lengthDiff = largerCoefficients.Length - smallerCoefficients.Length;
        // Copy high-order terms only found in higher-degree polynomial's coefficients
        Array.Copy(largerCoefficients, 0, sumDiff, 0, lengthDiff);

        for (var i = lengthDiff; i < largerCoefficients.Length; i++)
        {
            sumDiff[i] = GF256.AddOrSubtract(smallerCoefficients[i - lengthDiff], largerCoefficients[i]);
        }

        return new GF256Poly(field, sumDiff);
    }

    internal GF256Poly Multiply(GF256Poly other)
    {
        if (!field.Equals(other.field))
        {
            throw new ArgumentException("GF256Polys do not have same GF256 field");
        }

        if (IsZero() || other.IsZero())
        {
            return field.GetZero();
        }

        var aCoefficients = coefficients;
        var aLength = aCoefficients.Length;
        var bCoefficients = other.coefficients;
        var bLength = bCoefficients.Length;
        var product = new int[aLength + bLength - 1];
        for (var i = 0; i < aLength; i++)
        {
            var aCoeff = aCoefficients[i];
            for (var j = 0; j < bLength; j++)
            {
                product[i + j] = GF256.AddOrSubtract(product[i + j],
                                                     field.Multiply(aCoeff, bCoefficients[j]));
            }
        }

        return new GF256Poly(field, product);
    }

    internal GF256Poly Multiply(int scalar)
    {
        if (scalar == 0)
        {
            return field.GetZero();
        }

        if (scalar == 1)
        {
            return this;
        }

        var size = coefficients.Length;
        var product = new int[size];
        for (var i = 0; i < size; i++)
        {
            product[i] = field.Multiply(coefficients[i], scalar);
        }

        return new GF256Poly(field, product);
    }

    internal GF256Poly MultiplyByMonomial(int degree, int coefficient)
    {
        if (degree < 0)
        {
            throw new ArgumentException();
        }

        if (coefficient == 0)
        {
            return field.GetZero();
        }

        var size = coefficients.Length;
        var product = new int[size + degree];
        for (var i = 0; i < size; i++)
        {
            product[i] = field.Multiply(coefficients[i], coefficient);
        }

        return new GF256Poly(field, product);
    }

    internal GF256Poly[] Divide(GF256Poly other)
    {
        if (!field.Equals(other.field))
        {
            throw new ArgumentException("GF256Polys do not have same GF256 field");
        }

        if (other.IsZero())
        {
            throw new DivideByZeroException("Divide by 0");
        }

        var quotient = field.GetZero();
        var remainder = this;

        var denominatorLeadingTerm = other.GetCoefficient(other.GetDegree());
        var inverseDenominatorLeadingTerm = field.Inverse(denominatorLeadingTerm);

        while (remainder.GetDegree() >= other.GetDegree() && !remainder.IsZero())
        {
            var degreeDifference = remainder.GetDegree() - other.GetDegree();
            var scale = field.Multiply(remainder.GetCoefficient(remainder.GetDegree()), inverseDenominatorLeadingTerm);
            var term = other.MultiplyByMonomial(degreeDifference, scale);
            var iterationQuotient = field.BuildMonomial(degreeDifference, scale);
            quotient = quotient.AddOrSubtract(iterationQuotient);
            remainder = remainder.AddOrSubtract(term);
        }

        return new[] { quotient, remainder };
    }

    public override string ToString()
    {
        var result = new StringBuilder(8 * GetDegree());
        for (var degree = GetDegree(); degree >= 0; degree--)
        {
            var coefficient = GetCoefficient(degree);
            if (coefficient != 0)
            {
                if (coefficient < 0)
                {
                    result.Append(" - ");
                    coefficient = -coefficient;
                }
                else
                {
                    if (result.Length > 0)
                    {
                        result.Append(" + ");
                    }
                }

                if (degree == 0 || coefficient != 1)
                {
                    var alphaPower = field.Log(coefficient);
                    if (alphaPower == 0)
                    {
                        result.Append('1');
                    }
                    else if (alphaPower == 1)
                    {
                        result.Append('a');
                    }
                    else
                    {
                        result.Append("a^");
                        result.Append(alphaPower);
                    }
                }

                if (degree != 0)
                {
                    if (degree == 1)
                    {
                        result.Append('x');
                    }
                    else
                    {
                        result.Append("x^");
                        result.Append(degree);
                    }
                }
            }
        }

        return result.ToString();
    }
}