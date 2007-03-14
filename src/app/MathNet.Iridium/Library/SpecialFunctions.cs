#region Math.NET Iridium (LGPL) by Ruegg + Contributors
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2007, Christoph Rüegg,  http://christoph.ruegg.name
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

using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics
{
    /// <summary>
    /// Static DoublePrecision Special Functions Helper Class
    /// </summary>
    public static class Fn
    {
        /// <summary> Returns <code>sqrt(a<sup>2</sup> + b<sup>2</sup>)</code> 
        /// without underflow/overlow.</summary>
        public static double Hypot(double a, double b)
        {
            double r;

            if(Math.Abs(a) > Math.Abs(b))
            {
                r = b / a;
                r = Math.Abs(a) * Math.Sqrt(1 + r * r);
            }
            else if(b != 0)
            {
                r = a / b;
                r = Math.Abs(b) * Math.Sqrt(1 + r * r);
            }
            else r = 0.0;

            return r;
        }

        #region Base 2 Integer Exponentiation
        /// <summary>
        /// Raises 2 to the provided integer exponent (0 &lt;= exponent &lt; 31).
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static int IntPow2(int exponent)
        {
            if(exponent < 0 || exponent >= 31)
                throw new ArgumentOutOfRangeException("exponent");
            return 1 << exponent;
        }

        /// <summary>
        /// Evaluates the logarithm to base 2 of the provided integer value.
        /// </summary>
        public static int IntLog2(int x)
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
                                    return 0;
                                return 1;
                            }
                            return 2;
                        }
                        if(x <= 8)
                            return 3;
                        return 4;
                    }
                    if(x <= 64)
                    {
                        if(x <= 32)
                            return 5;
                        return 6;
                    }
                    if(x <= 128)
                        return 7;
                    return 8;
                }
                if(x <= 4096)
                {
                    if(x <= 1024)
                    {
                        if(x <= 512)
                            return 9;
                        return 10;
                    }
                    if(x <= 2048)
                        return 11;
                    return 12;
                }
                if(x <= 16384)
                {
                    if(x <= 8192)
                        return 13;
                    return 14;
                }
                if(x <= 32768)
                    return 15;
                return 16;
            }
            if(x <= 16777216)
            {
                if(x <= 1048576)
                {
                    if(x <= 262144)
                    {
                        if(x <= 131072)
                            return 17;
                        return 18;
                    }
                    if(x <= 524288)
                        return 19;
                    return 20;
                }
                if(x <= 4194304)
                {
                    if(x <= 2097152)
                        return 21;
                    return 22;
                }
                if(x <= 8388608)
                    return 23;
                return 24;
            }
            if(x <= 268435456)
            {
                if(x <= 67108864)
                {
                    if(x <= 33554432)
                        return 25;
                    return 26;
                }
                if(x <= 134217728)
                    return 27;
                return 28;
            }
            if(x <= 1073741824)
            {
                if(x <= 536870912)
                    return 29;
                return 30;
            }
            //	since int is unsigned it can never be higher than 2,147,483,647
            //	if( x <= 2147483648 )
            //		return	31;	
            //	return	32;	
            return 31;
        }

        /// <summary>
        /// Returns the smallest integer power of two bigger or equal to the value. 
        /// </summary>
        public static int CeilingToPowerOf2(int value)
        {
            return value <= 0 ? 0 : IntPow2(IntLog2(value));
        }

        /// <summary>
        /// Returns the biggest integer power of two smaller or equal to the value. 
        /// </summary>
        public static int FloorToPowerOf2(int value)
        {
            int log = IntLog2(value);
            int retHalf = log == 0 ? 0 : IntPow2(log - 1);
            return retHalf == value >> 1 ? value : retHalf;
        }
        #endregion

        #region Number Theory
        /// <summary>
        /// Returns the greatest common divisor of two integers using euclids algorithm.
        /// </summary>
        /// <returns>gcd(a,b)</returns>
        public static long Gcd(long a, long b)
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
        /// -> d == 9 && x == 1 && y == -2
        /// </code>
        /// The gcd of 45 and 18 is 9: 18 = 2*9, 45 = 5*9. 9 = 1*45 -2*18, therefore x=1 and y=-2.
        /// </example>
        public static long Gcd(long a, long b, out long x, out long y)
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
        public static long Lcm(long a, long b)
        {
            // TODO: Direct Implementation for preventing overflows.
            if(a == 0 && b == 0)
                return 0;
            return Math.Abs(a * b) / Gcd(a, b);
        }
        #endregion

        #region Factorial, Binomial Coefficient
        /// <summary>
        /// Returns the natural logarithm of the Factorial for a real value > 0.
        /// </summary>
        /// <returns>A value ln(value!) for value > 0</returns>
        public static double FactorialLn(int value)
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException("value", Resources.ArgumentPositive);
            if(value <= 1)
                return 0.0d;
            if(value >= FactorialLnCacheSize)
                return GammaLn(value + 1.0);
            if(factorialLnCache == null)
                factorialLnCache = new double[FactorialLnCacheSize];
            return factorialLnCache[value] != 0.0 ? factorialLnCache[value]
                : (factorialLnCache[value] = GammaLn(value + 1.0));
        }
        private static double[] factorialLnCache;
        private const int FactorialLnCacheSize = 2 * FactorialPrecompSize;

        /// <summary>
        /// Returns a factorial of an integer number (n!)
        /// </summary>
        /// <returns>A value value! for value > 0</returns>
        public static double Factorial(int value)
        {
            if(value < 0)
                throw new ArgumentOutOfRangeException("value", Resources.ArgumentPositive);
            if(value >= FactorialPrecompSize)
                return Math.Exp(GammaLn(value + 1.0));
            if(factorialPrecomp == null)
                factorialPrecomp = new double[FactorialPrecompSize] 
                    {1d, 1d, 2d, 6d, 24d, 120d, 720d, 5040d, 40320d, 362880d, 3628800d,
                        39916800d, 479001600d, 6227020800d, 87178291200d, 1307674368000d,
                        20922789888000d, 355687428096000d, 6402373705728000d,
                        121645100408832000d, 2432902008176640000d, 51090942171709440000d,
                        1124000727777607680000d, 25852016738884976640000d, 620448401733239439360000d,
                        15511210043330985984000000d, 403291461126605635584000000d,
                        10888869450418352160768000000d, 304888344611713860501504000000d,
                        8841761993739701954543616000000d, 265252859812191058636308480000000d,
                        8222838654177922817725562880000000d};
            return factorialPrecomp[value];
        }
        private static double[] factorialPrecomp;
        private const int FactorialPrecompSize = 32;

        /// <summary>
        /// Returns a binomial coefficient of n and k as a double precision number
        /// </summary>
        public static double BinomialCoefficient(int n, int k)
        {
            if(k < 0 || k > n)
                return 0.0;
            return Math.Floor(0.5 + Math.Exp(FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k)));
        }

        public static double BinomialCoefficientLn(int n, int k)
        {
            if(k < 0 || k > n)
                return 1.0;
            return FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k);
        }
        #endregion

        #region Gamma Functions
        /// <summary>
        /// Returns the natural logarithm of Gamma for a real value &gt; 0.
        /// </summary>
        /// <returns>A value ln|Gamma(value))| for value &gt; 0</returns>
        public static double GammaLn(double value)
        {
            double x, y, ser, temp;
            double[] coefficient = new double[]{76.18009172947146,-86.50535032941677,
												   24.01409824083091,-1.231739572450155,0.1208650973866179e-2,-0.5395239384953e-5};
            y = x = value;
            temp = x + 5.5;
            temp -= ((x + 0.5) * Math.Log(temp));
            ser = 1.000000000190015;
            for(int j = 0; j <= 5; j++)
                ser += (coefficient[j] / ++y);
            return -temp + Math.Log(2.50662827463100005 * ser / x);
        }

		/// <summary>
        /// Returns the regularized lower incomplete gamma function P(a,x) = 1/Gamma(a) * int(exp(-t)t^(a-1),t=0..x) for real a &gt; 0, x &gt; 0.
		/// </summary>
        public static double IncompleteGammaRegularized(double a, double x)
        {
            const int MaxIterations = 100;
            double eps = Number.RelativeAccuracy;
            double fpmin = Number.SmallestNumberGreaterThanZero / eps;

            if(a < 0.0 || x < 0.0)
                throw new ArgumentOutOfRangeException("a,x", Resources.ArgumentNotNegative);

            double gln = GammaLn(a);
            if(x < a + 1.0)
            {
                // Series Representation

                if(x <= 0.0) // Yes, I know we've already checked for x<0.0
                    return 0.0;
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
                            return sum * Math.Exp(-x + a * Math.Log(x) - gln);
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
                        d = fpmin;
                    c = b + an / c;
                    if(Math.Abs(c) < fpmin)
                        c = fpmin;
                    d = 1.0 / d;
                    double del = d * c;
                    h *= del;
                    if(Math.Abs(del - 1.0) <= eps)
                        return 1.0 - Math.Exp(-x + a * Math.Log(x) - gln) * h;
                }
            }
            throw new ArgumentException(Resources.ArgumentTooLargeForIterationLimit, "a");
        }
        #endregion

        #region Beta Functions
        /// <summary>
        /// Returns the Euler Beta function of real valued z > 0, w > 0. Beta(z,w) = Beta(w,z).
        /// </summary>
        public static double Beta(double z, double w)
        {
            return Math.Exp(GammaLn(z) + GammaLn(w) - GammaLn(z + w));
        }

        public static double BetaLn(double z, double w)
        {
            return GammaLn(z) + GammaLn(w) - GammaLn(z + w);
        }

        /// <summary>
        /// Returns the regularized lower incomplete beta function I_x(a,b) = 1/Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x) for real a &gt; 0, b &gt; 0, 1 &gt;= x &gt;= 0.
        /// </summary>
        public static double IncompleteBetaRegularized(double a, double b, double x)
        {
            if(a < 0.0 || b < 0.0)
                throw new ArgumentOutOfRangeException("a,b", Resources.ArgumentNotNegative);
            if(x < 0.0 || x > 1.0)
                throw new ArgumentOutOfRangeException("x", String.Format(Resources.ArgumentInIntervalXYInclusive,"0.0","1.0"));

            double bt = (x == 0.0 || x == 1.0) ? 0.0 : Math.Exp(GammaLn(a + b) - GammaLn(a) - GammaLn(b) + a * Math.Log(x) + b * Math.Log(1.0 - x));
            double betacf;

            // Continued fraction representation

            const int MaxIterations = 100;
            double eps = Number.RelativeAccuracy;
            double fpmin = Number.SmallestNumberGreaterThanZero / eps;

            double qab = a + b;
            double qap = a + 1.0;
            double qam = a - 1.0;
            double c = 1.0;
            double d = 1.0 - qab * x / qap;
            if(Math.Abs(d) < fpmin)
                d = fpmin;
            d = 1.0 / d;
            double h = d;
            for(int m = 1, m2 = 2; m <= MaxIterations; m++, m2 += 2)
            {
                double aa = m * (b - m) * x / ((qam + m2) * (a + m2));
                d = 1.0 + aa * d;
                if(Math.Abs(d) < fpmin)
                    d = fpmin;
                c = 1.0 + aa / c;
                if(Math.Abs(c) < fpmin)
                    c = fpmin;
                d = 1.0 / d;
                h *= d * c;
                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
                d = 1.0 + aa * d;
                if(Math.Abs(d) < fpmin)
                    d = fpmin;
                c = 1.0 + aa / c;
                if(Math.Abs(c) < fpmin)
                    c = fpmin;
                d = 1.0 / d;
                double del = d * c;
                h *= del;
                if(Math.Abs(del - 1.0) <= eps)
                {
                    if(x < (a + 1.0) / (a + b + 2.0))
                        return bt * h / a;
                    else
                        return 1.0 - bt * h / b;
                }
            }
            throw new ArgumentException(Resources.ArgumentTooLargeForIterationLimit, "a,b");
        }
        #endregion

        #region Error Functions
        /// <summary>
        /// Returns the error function erf(x) = 2/sqrt(pi) * int(exp(-t^2),t=0..x)
        /// </summary>
        public static double Erf(double x)
        {
            return x < 0.0 ? -IncompleteGammaRegularized(0.5, x * x)
                : IncompleteGammaRegularized(0.5, x * x);
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
        public static double ErfInverse(double x)
        {
            if(x < -1.0 || x > 1.0) throw new ArgumentOutOfRangeException(
                "p", x, String.Format(Resources.ArgumentInIntervalXYInclusive,"-1.0","1.0"));

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

        private static double[] erfinv_a = {-3.969683028665376e+01, 2.209460984245205e+02,
                         -2.759285104469687e+02, 1.383577518672690e+02,
                         -3.066479806614716e+01, 2.506628277459239e+00};

        private static double[] erfinv_b = {-5.447609879822406e+01, 1.615858368580409e+02,
                         -1.556989798598866e+02, 6.680131188771972e+01, -1.328068155288572e+01};

        private static double[] erfinv_c = {-7.784894002430293e-03, -3.223964580411365e-01,
                         -2.400758277161838e+00, -2.549732539343734e+00,
                         4.374664141464968e+00, 2.938163982698783e+00};

        private static double[] erfinv_d = {7.784695709041462e-03, 3.224671290700398e-01,
                         2.445134137142996e+00, 3.754408661907416e+00};
        #endregion
    }
}
