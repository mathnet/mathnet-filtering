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

        /// <summary>
        /// Returns the natural logarithm of Gamma for a real value > 0
        /// </summary>
        /// <param name="xx">A real value for Gamma calculation</param>
        /// <returns>A value ln|Gamma(xx))| for xx > 0</returns>
        public static double GammaLn(double xx)
        {
            // TODO: check
            double x, y, ser, temp;
            double[] coefficient = new double[]{76.18009172947146,-86.50535032941677,
												   24.01409824083091,-1.231739572450155,0.1208650973866179e-2,-0.5395239384953e-5};
            int j;
            y = x = xx;
            temp = x + 5.5;
            temp -= ((x + 0.5) * Math.Log(temp));
            ser = 1.000000000190015;
            for(j = 0; j <= 5; j++)
                ser += (coefficient[j] / ++y);
            return -temp + Math.Log(2.50662827463100005 * ser / x);
        }

        /// <summary>
        /// Returns a factorial of an integer number (n!)
        /// </summary>
        /// <param name="n">The value to be factorialized</param>
        /// <returns>The double precision result</returns>
        public static double Factorial(int n)
        {
            // TODO: check
            int ntop = 4;
            double[] a = new double[32];
            a[0] = 1.0; a[1] = 1.0; a[2] = 2.0; a[3] = 6.0; a[4] = 24.0;
            int j;
            if(n < 0)
                throw new ArgumentException("Factorial expects a positive argument", "n");
            if(n > 32)
                return Math.Exp(GammaLn(n + 1.0));
            while(ntop < n)
            {
                j = ntop++;
                a[ntop] = a[j] * ntop;
            }
            return a[n];
        }

        /// <summary>
        /// Returns a binomial coefficient of n and k as a double precision number
        /// </summary>
        /// <param name="n"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static double BinomialCoefficient(int n, int k)
        {
            if(k < 0 || k > n)
                return 0;
            return Math.Floor(0.5 + Math.Exp(FactorialLn(n) - FactorialLn(k) - FactorialLn(n - k)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double FactorialLn(int n)
        {
            // TODO: check
            double[] a = new double[101];
            if(n < 0)
                throw new ArgumentException("Factorial expects a positive argument", "n");
            if(n <= 1)
                return 0.0d;
            if(n <= 100)
            {
                a[n] = GammaLn(n + 1.0d);
                return (a[n] == 0.0d) ? a[n] : (a[n]); // TODO: historic hulk?
            }
            else
            {
                return GammaLn(n + 1.0d);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="z"></param>
        /// <param name="w"></param>
        /// <returns></returns>
        public static double Beta(double z, double w)
        {
            return Math.Exp(GammaLn(z) + GammaLn(w) - GammaLn(z + w));
        }
    }
}
