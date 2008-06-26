#region Math.NET Iridium (LGPL) by Ruegg + Contributors
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//
// Contribution: Fn.IntLog2 by Ben Houston, http://www.exocortex.org
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion
#region Some algorithms based on: Copyright 2000 Moshier, Bochkanov
// Cephes Math Library
// Copyright by Stephen L. Moshier
//
// Contributors:
//    * Sergey Bochkanov (ALGLIB project). Translation from C to
//      pseudocode.
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are
// met:
//
// - Redistributions of source code must retain the above copyright
//   notice, this list of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright
//   notice, this list of conditions and the following disclaimer listed
//   in this license in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the copyright holders nor the names of its
//   contributors may be used to endorse or promote products derived from
//   this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics.Properties;

namespace MathNet.Numerics
{
    /// <summary>
    /// Double-precision special functions toolkit.
    /// </summary>
    public static class Fn
    {

        /// <summary> Returns <code>sqrt(a<sup>2</sup> + b<sup>2</sup>)</code> 
        /// without underflow/overflow.</summary>
        public static
        double
        Hypot(
            double a,
            double b
            )
        {
            if(Math.Abs(a) > Math.Abs(b))
            {
                double r = b / a;
                return Math.Abs(a) * Math.Sqrt(1 + r * r);
            }

            if(!Number.AlmostZero(b))
            {
                double r = a / b;
                return Math.Abs(b) * Math.Sqrt(1 + r * r);
            }

            return 0d;
        }

        /// <summary>
        /// Integer Power
        /// </summary>
        [CLSCompliant(false)]
        public static
        long
        IntPow(
            long radix,
            uint exponent
            )
        {
            // TODO: investigate for a better solution
            return (long)Math.Round(Math.Pow(radix, exponent));
        }

        #region Base 2 Integer Exponentiation

        /// <summary>
        /// Raises 2 to the provided integer exponent (0 &lt;= exponent &lt; 31).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static
        int
        IntPow2(
            int exponent
            )
        {
            if(exponent < 0 || exponent >= 31)
            {
                throw new ArgumentOutOfRangeException("exponent");
            }

            return 1 << exponent;
        }

        /// <summary>
        /// Evaluates the logarithm to base 2 of the provided integer value.
        /// </summary>
        public static
        int
        IntLog2(
            int x
            )
        {
            if(x <= 65536)
            {
                if(x <= 256)
                {
                    if(x <= 16)
                    {
                        if(x <= 4)
                        {
                            if(x <= 2)
                            {
                                if(x <= 1)
                                {
                                    return 0;
                                }

                                return 1;
                            }

                            return 2;
                        }

                        if(x <= 8)
                        {
                            return 3;
                        }

                        return 4;
                    }

                    if(x <= 64)
                    {
                        if(x <= 32)
                        {
                            return 5;
                        }

                        return 6;
                    }

                    if(x <= 128)
                    {
                        return 7;
                    }

                    return 8;
                }

                if(x <= 4096)
                {
                    if(x <= 1024)
                    {
                        if(x <= 512)
                        {
                            return 9;
                        }

                        return 10;
                    }

                    if(x <= 2048)
                    {
                        return 11;
                    }

                    return 12;
                }

                if(x <= 16384)
                {
                    if(x <= 8192)
                    {
                        return 13;
                    }

                    return 14;
                }

                if(x <= 32768)
                {
                    return 15;
                }

                return 16;
            }

            if(x <= 16777216)
            {
                if(x <= 1048576)
                {
                    if(x <= 262144)
                    {
                        if(x <= 131072)
                        {
                            return 17;
                        }

                        return 18;
                    }

                    if(x <= 524288)
                    {
                        return 19;
                    }

                    return 20;
                }

                if(x <= 4194304)
                {
                    if(x <= 2097152)
                    {
                        return 21;
                    }

                    return 22;
                }

                if(x <= 8388608)
                {
                    return 23;
                }

                return 24;
            }

            if(x <= 268435456)
            {
                if(x <= 67108864)
                {
                    if(x <= 33554432)
                    {
                        return 25;
                    }

                    return 26;
                }

                if(x <= 134217728)
                {
                    return 27;
                }

                return 28;
            }

            if(x <= 1073741824)
            {
                if(x <= 536870912)
                {
                    return 29;
                }

                return 30;
            }

            // since int is unsigned it can never be higher than 2,147,483,647
            return 31;
        }

        /// <summary>
        /// Returns the smallest integer power of two bigger or equal to the value. 
        /// </summary>
        public static
        int
        CeilingToPowerOf2(
            int value
            )
        {
            return value <= 0
                ? 0
                : IntPow2(IntLog2(value));
        }

        /// <summary>
        /// Returns the biggest integer power of two smaller or equal to the value. 
        /// </summary>
        public static
        int
        FloorToPowerOf2(
            int value
            )
        {
            int log = IntLog2(value);

            int retHalf = log == 0
                ? 0
                : IntPow2(log - 1);

            return retHalf == value >> 1
                ? value
                : retHalf;
        }

        #endregion

        #region Number Theory

        /// <summary>
        /// Returns the greatest common divisor of two integers using euclids algorithm.
        /// </summary>
        /// <returns>gcd(a,b)</returns>
        public static
        long
        Gcd(
            long a,
            long b
            )
        {
            long rem;

            while(b != 0)
            {
                rem = a % b;
                a = b;
                b = rem;
            }

            return Math.Abs(a);
        }

        /// <summary>
        /// Computes the extended greatest common divisor, such that a*x + b*y = gcd(a,b).
        /// </summary>
        /// <returns>gcd(a,b)</returns>
        /// <example>
        /// <code>
        /// long x,y,d;
        /// d = Fn.Gcd(45,18,out x, out y);
        /// -> d == 9 &amp;&amp; x == 1 &amp;&amp; y == -2
        /// </code>
        /// The gcd of 45 and 18 is 9: 18 = 2*9, 45 = 5*9. 9 = 1*45 -2*18, therefore x=1 and y=-2.
        /// </example>
        public static
        long
        Gcd(
            long a,
            long b,
            out long x,
            out long y
            )
        {
            long rem, quot, tmp;
            long mp = 1, np = 0, m = 0, n = 1;

            while(b != 0)
            {
                quot = a / b;
                rem = a % b;
                a = b;
                b = rem;

                tmp = m;
                m = mp - quot * m;
                mp = tmp;

                tmp = n;
                n = np - quot * n;
                np = tmp;
            }

            if(a >= 0)
            {
                x = mp;
                y = np;
                return a;
            }
            else
            {
                x = -mp;
                y = -np;
                return -a;
            }
        }

        /// <summary>
        /// Returns the least common multiple of two integers using euclids algorithm.
        /// </summary>
        /// <returns>lcm(a,b)</returns>
        public static
        long
        Lcm(
            long a,
            long b
            )
        {
            // TODO: Direct Implementation for preventing overflows.
            if(a == 0 && b == 0)
            {
                return 0;
            }

            return Math.Abs(a * b) / Gcd(a, b);
        }

        #endregion

        #region Sinc

        /// <summary>
        /// Normalized Sinc (sinus cardinalis) Function.
        /// </summary>
        /// <remarks>sinc(x) = sin(pi * x) / (pi * x)</remarks>
        public static
        double
        Sinc(
            double x
            )
        {
            if(double.IsNaN(x))
            {
                return double.NaN;
            }

            if(double.IsInfinity(x))
            {
                return 0.0;
            }

            double a = Math.PI * x;
            double sinc = Math.Sin(a) / a;

            if(double.IsInfinity(sinc) || double.IsNaN(sinc))
            {
                return 1.0;
            }

            return sinc;
        }

        #endregion

        #region Factorial, Binomial Coefficient

        /// <summary>
        /// Returns the natural logarithm of the factorial (n!) for an integer value > 0.
        /// </summary>
        /// <returns>A value ln(value!) for value > 0</returns>
        public static
        double
        FactorialLn(
            int value
            )
        {
            if(value < 0)
            {
                throw new ArgumentOutOfRangeException("value", Resources.ArgumentPositive);
            }

            if(value <= 1)
            {
                return 0.0d;
            }

            if(value >= FactorialLnCacheSize)
            {
                return GammaLn(value + 1.0);
            }

            if(factorialLnCache == null)
            {
                factorialLnCache = new double[FactorialLnCacheSize];
            }

            return factorialLnCache[value] != 0.0
                ? factorialLnCache[value]
                : (factorialLnCache[value] = GammaLn(value + 1.0));
        }

        static double[] factorialLnCache;
        const int FactorialLnCacheSize = 2 * FactorialPrecompSize;

        /// <summary>
        /// Returns the factorial (n!) of an integer number > 0. Consider using <see cref="FactorialLn"/> instead.
        /// </summary>
        /// <returns>A value value! for value > 0</returns>
        /// <remarks>
        /// If you need to multiply or divide various such factorials, consider
        /// using the logarithmic version <see cref="FactorialLn"/> instead
        /// so you can add instead of multiply and subtract instead of divide, and
        /// then exponentiate the result using <see cref="System.Math.Exp"/>.
        /// This will also completely circumvent the problem that factorials
        /// easily become very large.
        /// </remarks>
        public static
        double
        Factorial(
            int value
            )
        {
            if(value < 0)
            {
                throw new ArgumentOutOfRangeException("value", Resources.ArgumentPositive);
            }

            if(value >= FactorialPrecompSize)
            {
                return Math.Exp(GammaLn(value + 1.0));
            }

            return factorialPrecomp[value];
        }

        #region Precomputed Static Array
        const int FactorialPrecompSize = 32;
        static double[] factorialPrecomp = new double[] {
            1d,
            1d,
            2d,
            6d,
            24d,
            120d,
            720d,
            5040d,
            40320d,
            362880d,
            3628800d,
            39916800d,
            479001600d,
            6227020800d,
            87178291200d,
            1307674368000d,
            20922789888000d,
            355687428096000d,
            6402373705728000d,
            121645100408832000d,
            2432902008176640000d,
            51090942171709440000d,
            1124000727777607680000d,
            25852016738884976640000d,
            620448401733239439360000d,
            15511210043330985984000000d,
            403291461126605635584000000d,
            10888869450418352160768000000d,
            304888344611713860501504000000d,
            8841761993739701954543616000000d,
            265252859812191058636308480000000d,
            8222838654177922817725562880000000d
        };
        #endregion

        /// <summary>
        /// Returns the binomial coefficient of n and k as a double precision number.
        /// </summary>
        /// <remarks>
        /// If you need to multiply or divide various such coefficients, consider
        /// using the logarithmic version <see cref="BinomialCoefficientLn"/> instead
        /// so you can add instead of multiply and subtract instead of divide, and
        /// then exponentiate the result using <see cref="System.Math.Exp"/>.
        /// </remarks>
        public static
        double
        BinomialCoefficient(
            int n,
            int k
            )
        {
            if(k < 0 || n < 0 || k > n)
            {
                return 0.0;
            }

            return Math.Floor(0.5 + Math.Exp(FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k)));
        }

        /// <summary>
        /// Returns the natural logarithm of the binomial coefficient of n and k as a double precision number.
        /// </summary>
        public static
        double
        BinomialCoefficientLn(
            int n,
            int k
            )
        {
            if(k < 0 || n < 0 || k > n)
            {
                return 1.0;
            }

            return FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k);
        }

        #endregion

        #region Gamma Functions

        /// <summary>
        /// Returns the natural logarithm of Gamma for a real value &gt; 0.
        /// </summary>
        /// <returns>A value ln|Gamma(value))| for value &gt; 0</returns>
        public static
        double
        GammaLn(
            double value
            )
        {
            double x, y, ser, temp;
            double[] coefficient = new double[]{
                76.18009172947146,
                -86.50532032941677,
                24.01409824083091,
                -1.231739572450155,
                0.1208650973866179e-2,
                -0.5395239384953e-5
                };

            y = x = value;
            temp = x + 5.5;
            temp -= ((x + 0.5) * Math.Log(temp));
            ser = 1.000000000190015;

            for(int j = 0; j <= 5; j++)
            {
                ser += (coefficient[j] / ++y);
            }

            return -temp + Math.Log(2.5066282746310005 * ser / x);
        }

        /// <summary>
        /// Returns the gamma function for real values (except at 0, -1, -2, ...).
        /// For numeric stability, consider to use GammaLn for positive values.
        /// </summary>
        /// <returns>A value Gamma(value) for value != 0,-1,-2,...</returns>
        public static
        double
        Gamma(
            double value
            )
        {
            if(value > 0.0)
            {
                return Math.Exp(GammaLn(value));
            }

            double reflection = 1.0 - value;
            double s = Math.Sin(Math.PI * reflection);

            if(Number.AlmostEqual(0.0, s))
            {
                return double.NaN; // singularity, undefined
            }

            return Math.PI / (s * Math.Exp(GammaLn(reflection)));
        }

        /// <summary>
        /// Obsolete. Please use GammaRegularized instead, with the same parameters (method was renamed).
        /// </summary>
        [Obsolete("Renamed to GammaRegularized; Hence please migrate to GammaRegularized.")]
        public static
        double
        IncompleteGammaRegularized(
            double a,
            double x
            )
        {
            return GammaRegularized(a, x);
        }

        /// <summary>
        /// Returns the regularized lower incomplete gamma function
        /// P(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
        /// </summary>
        public static
        double
        GammaRegularized(
            double a,
            double x
            )
        {
            const int MaxIterations = 100;
            double eps = Number.RelativeAccuracy;
            double fpmin = Number.SmallestNumberGreaterThanZero / eps;

            if(a < 0.0 || x < 0.0)
            {
                throw new ArgumentOutOfRangeException("a,x", Resources.ArgumentNotNegative);
            }

            double gln = GammaLn(a);
            if(x < a + 1.0)
            {
                // Series Representation

                if(x <= 0.0)
                {
                    // Yes, I know we've already checked for x<0.0

                    return 0.0;
                }
                else
                {
                    double ap = a;
                    double del, sum = del = 1.0 / a;

                    for(int n = 0; n < MaxIterations; n++)
                    {
                        ++ap;
                        del *= x / ap;
                        sum += del;

                        if(Math.Abs(del) < Math.Abs(sum) * eps)
                        {
                            return sum * Math.Exp(-x + a * Math.Log(x) - gln);
                        }
                    }
                }
            }
            else
            {
                // Continued fraction representation

                double b = x + 1.0 - a;
                double c = 1.0 / fpmin;
                double d = 1.0 / b;
                double h = d;

                for(int i = 1; i <= MaxIterations; i++)
                {
                    double an = -i * (i - a);
                    b += 2.0;
                    d = an * d + b;

                    if(Math.Abs(d) < fpmin)
                    {
                        d = fpmin;
                    }

                    c = b + an / c;

                    if(Math.Abs(c) < fpmin)
                    {
                        c = fpmin;
                    }

                    d = 1.0 / d;
                    double del = d * c;
                    h *= del;

                    if(Math.Abs(del - 1.0) <= eps)
                    {
                        return 1.0 - Math.Exp(-x + a * Math.Log(x) - gln) * h;
                    }
                }
            }

            throw new ArgumentException(Resources.ArgumentTooLargeForIterationLimit, "a");
        }

        #endregion

        #region Digamma Functions

        /// <summary>
        /// Returns the digamma (psi) function of real values (except at 0, -1, -2, ...).
        /// Digamma is the logarithmic derivative of the <see cref="Gamma"/> function.
        /// </summary>
        public static
        double
        Digamma(
            double x
            )
        {
            double y = 0;
            double nz = 0.0;
            bool negative = (x <= 0);

            if(negative)
            {
                double q = x;
                double p = Math.Floor(q);
                negative = true;

                if(Number.AlmostEqual(p, q))
                {
                    return double.NaN; // singularity, undefined
                }

                nz = q - p;

                if(nz != 0.5)
                {
                    if(nz > 0.5)
                    {
                        p = p + 1.0;
                        nz = q - p;
                    }

                    nz = Math.PI / Math.Tan(Math.PI * nz);
                }
                else
                {
                    nz = 0.0;
                }

                x = 1.0 - x;
            }

            if((x <= 10.0) && (x == Math.Floor(x)))
            {
                y = 0.0;
                int n = (int)Math.Floor(x);

                for(int i = 1; i <= n - 1; i++)
                {
                    y = y + 1.0 / i;
                }

                y = y - Constants.EulerGamma;
            }
            else
            {
                double s = x;
                double w = 0.0;

                while(s < 10.0)
                {
                    w = w + 1.0 / s;
                    s = s + 1.0;
                }

                if(s < 1.0e17)
                {
                    double z = 1.0 / (s * s);
                    double polv = 8.33333333333333333333e-2;
                    polv = polv * z - 2.10927960927960927961e-2;
                    polv = polv * z + 7.57575757575757575758e-3;
                    polv = polv * z - 4.16666666666666666667e-3;
                    polv = polv * z + 3.96825396825396825397e-3;
                    polv = polv * z - 8.33333333333333333333e-3;
                    polv = polv * z + 8.33333333333333333333e-2;
                    y = z * polv;
                }
                else
                {
                    y = 0.0;
                }

                y = Math.Log(s) - 0.5 / s - y - w;
            }

            if(negative)
            {
                return y - nz;
            }

            return y;
        }

        #endregion

        #region Beta Functions

        /// <summary>
        /// Returns the Euler Beta function of real valued z > 0, w > 0.
        /// Beta(z,w) = Beta(w,z).
        /// </summary>
        public static
        double
        Beta(
            double z,
            double w
            )
        {
            return Math.Exp(GammaLn(z) + GammaLn(w) - GammaLn(z + w));
        }

        /// <summary>
        /// Returns the natural logarithm of the Euler Beta function of real valued z > 0, w > 0.
        /// BetaLn(z,w) = BetaLn(w,z).
        /// </summary>
        public static
        double
        BetaLn(
            double z,
            double w
            )
        {
            return GammaLn(z) + GammaLn(w) - GammaLn(z + w);
        }

        /// <summary>
        /// Obsolete. Please use BetaRegularized instead, with the same parameters (method was renamed).
        /// </summary>
        [Obsolete("Renamed to BetaRegularized; Hence please migrate to BetaRegularized.")]
        public static
        double
        IncompleteBetaRegularized(
            double a,
            double b,
            double x
            )
        {
            return BetaRegularized(a, b, x);
        }

        /// <summary>
        /// Returns the regularized lower incomplete beta function
        /// I_x(a,b) = 1/Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a &gt; 0, b &gt; 0, 1 &gt;= x &gt;= 0.
        /// </summary>
        public static
        double
        BetaRegularized(
            double a,
            double b,
            double x
            )
        {
            if(a < 0.0 || b < 0.0)
            {
                throw new ArgumentOutOfRangeException("a,b", Resources.ArgumentNotNegative);
            }

            if(x < 0.0 || x > 1.0)
            {
                throw new ArgumentOutOfRangeException("x", String.Format(Resources.ArgumentInIntervalXYInclusive, "0.0", "1.0"));
            }

            double bt = (x == 0.0 || x == 1.0)
                ? 0.0
                : Math.Exp(GammaLn(a + b) - GammaLn(a) - GammaLn(b) + a * Math.Log(x) + b * Math.Log(1.0 - x));

            bool symmetryTransformation = (x >= (a + 1.0) / (a + b + 2.0));

            // Continued fraction representation

            const int MaxIterations = 100;
            double eps = Number.RelativeAccuracy;
            double fpmin = Number.SmallestNumberGreaterThanZero / eps;

            if(symmetryTransformation)
            {
                x = 1.0 - x;
                double swap = a;
                a = b;
                b = swap;
            }

            double qab = a + b;
            double qap = a + 1.0;
            double qam = a - 1.0;
            double c = 1.0;
            double d = 1.0 - qab * x / qap;

            if(Math.Abs(d) < fpmin)
            {
                d = fpmin;
            }

            d = 1.0 / d;
            double h = d;

            for(int m = 1, m2 = 2; m <= MaxIterations; m++, m2 += 2)
            {
                double aa = m * (b - m) * x / ((qam + m2) * (a + m2));
                d = 1.0 + aa * d;

                if(Math.Abs(d) < fpmin)
                {
                    d = fpmin;
                }

                c = 1.0 + aa / c;
                if(Math.Abs(c) < fpmin)
                {
                    c = fpmin;
                }

                d = 1.0 / d;
                h *= d * c;
                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
                d = 1.0 + aa * d;

                if(Math.Abs(d) < fpmin)
                {
                    d = fpmin;
                }

                c = 1.0 + aa / c;

                if(Math.Abs(c) < fpmin)
                {
                    c = fpmin;
                }

                d = 1.0 / d;
                double del = d * c;
                h *= del;

                if(Math.Abs(del - 1.0) <= eps)
                {
                    if(symmetryTransformation)
                    {
                        return 1.0 - bt * h / a;
                    }

                    return bt * h / a;
                }
            }

            throw new ArgumentException(Resources.ArgumentTooLargeForIterationLimit, "a,b");
        }

        #endregion

        #region Error Functions

        /// <summary>
        /// Returns the error function erf(x) = 2/sqrt(pi) * int(exp(-t^2),t=0..x)
        /// </summary>
        public static
        double
        Erf(
            double x
            )
        {
            if(double.IsNegativeInfinity(x))
            {
                return -1.0;
            }

            if(double.IsPositiveInfinity(x))
            {
                return 1.0;
            }

            return x < 0.0
                ? -GammaRegularized(0.5, x * x)
                : GammaRegularized(0.5, x * x);
        }

        /// <summary>
        /// Returns the inverse error function erf^-1(x).
        /// </summary>
        /// <remarks>
        /// <p>The algorithm uses a minimax approximation by rational functions
        /// and the result has a relative error whose absolute value is less
        /// than 1.15e-9.</p>
        /// 
        /// <p>See the page <see href="http://home.online.no/~pjacklam/notes/invnorm/"/>
        /// for more details.</p>
        /// </remarks>
        public static
        double
        ErfInverse(
            double x
            )
        {
            if(x < -1.0 || x > 1.0)
            {
                throw new ArgumentOutOfRangeException("p", x, String.Format(Resources.ArgumentInIntervalXYInclusive, "-1.0", "1.0"));
            }

            x = 0.5 * (x + 1.0);

            // Define break-points.
            double plow = 0.02425;
            double phigh = 1 - plow;

            double q;

            // Rational approximation for lower region:
            if(x < plow)
            {
                q = Math.Sqrt(-2 * Math.Log(x));
                return (((((erfinv_c[0] * q + erfinv_c[1]) * q + erfinv_c[2]) * q + erfinv_c[3]) * q + erfinv_c[4]) * q + erfinv_c[5]) /
                    ((((erfinv_d[0] * q + erfinv_d[1]) * q + erfinv_d[2]) * q + erfinv_d[3]) * q + 1)
                    * Constants.Sqrt1_2;
            }

            // Rational approximation for upper region:
            if(phigh < x)
            {
                q = Math.Sqrt(-2 * Math.Log(1 - x));
                return -(((((erfinv_c[0] * q + erfinv_c[1]) * q + erfinv_c[2]) * q + erfinv_c[3]) * q + erfinv_c[4]) * q + erfinv_c[5]) /
                    ((((erfinv_d[0] * q + erfinv_d[1]) * q + erfinv_d[2]) * q + erfinv_d[3]) * q + 1)
                    * Constants.Sqrt1_2;
            }

            // Rational approximation for central region:
            q = x - 0.5;
            double r = q * q;
            return (((((erfinv_a[0] * r + erfinv_a[1]) * r + erfinv_a[2]) * r + erfinv_a[3]) * r + erfinv_a[4]) * r + erfinv_a[5]) * q /
                (((((erfinv_b[0] * r + erfinv_b[1]) * r + erfinv_b[2]) * r + erfinv_b[3]) * r + erfinv_b[4]) * r + 1)
                * Constants.Sqrt1_2;
        }

        private static double[] erfinv_a = {
            -3.969683028665376e+01, 2.209460984245205e+02,
            -2.759285104469687e+02, 1.383577518672690e+02,
            -3.066479806614716e+01, 2.506628277459239e+00
            };

        private static double[] erfinv_b = {
            -5.447609879822406e+01, 1.615858368580409e+02,
            -1.556989798598866e+02, 6.680131188771972e+01,
            -1.328068155288572e+01
            };

        private static double[] erfinv_c = {
            -7.784894002430293e-03, -3.223964580411365e-01,
            -2.400758277161838e+00, -2.549732539343734e+00,
            4.374664141464968e+00, 2.938163982698783e+00
            };

        private static double[] erfinv_d = {
            7.784695709041462e-03, 3.224671290700398e-01,
            2.445134137142996e+00, 3.754408661907416e+00
            };

        #endregion

        #region Harmonic Numbers

        /// <summary>
        /// Evaluates the n-th harmonic number Hn = sum(1/k,k=1..n).
        /// </summary>
        /// <param name="n">n >= 0</param>
        /// <remarks>
        /// See <a http="http://en.wikipedia.org/wiki/Harmonic_Number">Wikipedia - Harmonic Number</a>
        /// </remarks>
        public static
        double
        HarmonicNumber(
            int n
            )
        {
            if(n < 0)
            {
                throw new ArgumentOutOfRangeException("n", Resources.ArgumentNotNegative);
            }

            if(n >= HarmonicPrecompSize)
            {
                double n2 = n * n;
                double n4 = n2 * n2;
                return Constants.EulerGamma
                    + Math.Log(n)
                    + 0.5 / n
                    - 1.0 / (12.0 * n2)
                    + 1.0 / (120.0 * n4);
            }

            return harmonicPrecomp[n];
        }

        #region Precomputed Static Array
        const int HarmonicPrecompSize = 32;
        static double[] harmonicPrecomp = new double[] {
            0.0,
            1.0,
            1.5,
            1.833333333333333333333333,
            2.083333333333333333333333,
            2.283333333333333333333333,
            2.45,
            2.592857142857142857142857,
            2.717857142857142857142857,
            2.828968253968253968253968,
            2.928968253968253968253968,
            3.019877344877344877344877,
            3.103210678210678210678211,
            3.180133755133755133755134,
            3.251562326562326562326562,
            3.318228993228993228993229,
            3.380728993228993228993229,
            3.439552522640757934875582,
            3.495108078196313490431137,
            3.547739657143681911483769,
            3.597739657143681911483769,
            3.645358704762729530531388,
            3.690813250217274985076843,
            3.734291511086840202468147,
            3.775958177753506869134814,
            3.815958177753506869134814,
            3.854419716215045330673275,
            3.891456753252082367710312,
            3.927171038966368081996027,
            3.961653797587057737168440,
            3.994987130920391070501774,
            4.027245195436520102759838
        };
        #endregion

        #endregion
    }
}
