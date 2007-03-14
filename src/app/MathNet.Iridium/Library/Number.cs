#region Copyright 2006 Christoph Rüegg
//MathNet Numerics: Iridium, part of Math.NET
//Copyright (c) 2006, Christoph Daniel Rüegg (http://cdrnet.net/)
//This Math.NET package is available under the terms of the LGPL.

//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published 
//by the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public 
//License along with this program; if not, write to the Free Software
//Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct FloatingPoint64
    {
        [FieldOffset(0)]
        public System.Double float64;

        [FieldOffset(0)]
        public System.Int64 signed64;

        [FieldOffset(0)]
        public System.UInt64 unsigned64;
    }

    /// <summary>
    /// Helper functions for dealing with floating point numbers.
    /// </summary>
    public static class Number
    {
        public const double SmallestNumberGreaterThanZero = double.Epsilon;
        public static readonly double RelativeAccuracy = EpsilonOf(1.0);

        /// <summary>
        /// Evaluates the minimum distance to the next distinguishable number near the argument value.
        /// </summary>
        /// <returns>Relative Epsilon (positive double or NaN).</returns>
        /// <remarks>Evaluates the <b>negative</b> epsilon. The more common positive epsilon is equal to two times this negative epsilon.</remarks>
        public static double EpsilonOf(double value)
        {
            if(double.IsInfinity(value) || double.IsNaN(value))
                return double.NaN;

            long signed64 = BitConverter.DoubleToInt64Bits(value);
            if(signed64 == 0)
            {
                signed64++;
                return BitConverter.Int64BitsToDouble(signed64) - value;
            }
            else if(signed64-- < 0)
                return BitConverter.Int64BitsToDouble(signed64) - value;
            else
                return value - BitConverter.Int64BitsToDouble(signed64);
        }

        /// <summary>
        /// Increments a floating point number to the next bigger number representable by the data type.
        /// </summary>
        /// <remarks>
        /// The incrementation step length depends on the provided value.
        /// Increment(double.MaxValue) will return positive infinity.
        /// </remarks>
        public static double Increment(double value)
        {
            if(double.IsInfinity(value) || double.IsNaN(value))
                return value;

            long signed64 = BitConverter.DoubleToInt64Bits(value);
            if(signed64 < 0)
                signed64--;
            else
                signed64++;
            if(signed64 == -9223372036854775808) // = "-0", make it "+0"
                return 0;
            value = BitConverter.Int64BitsToDouble(signed64);
            return double.IsNaN(value) ? double.NaN : value;
        }

        /// <summary>
        /// Decrements a floating point number to the next smaller number representable by the data type.
        /// </summary>
        /// <remarks>
        /// The decrementation step length depends on the provided value.
        /// Decrement(double.MinValue) will return negative infinity.
        /// </remarks>
        public static double Decrement(double value)
        {
            if(double.IsInfinity(value) || double.IsNaN(value))
                return value;

            long signed64 = BitConverter.DoubleToInt64Bits(value);
            if(signed64 == 0)
                return -double.Epsilon;
            if(signed64 < 0)
                signed64++;
            else
                signed64--;
            value = BitConverter.Int64BitsToDouble(signed64);
            return double.IsNaN(value) ? double.NaN : value;
        }

        /// <summary>
        /// Evaluates the count of numbers between two double numbers
        /// </summary>
        /// <remarks>The second number is included in the number, thus two equal numbers evaluate to zero and two neighbour numbers evaluate to one. Therefore, what is returned is actually the count of numbers between plus 1.</remarks>
        [CLSCompliant(false)]
        public static ulong NumbersBetween(double a, double b)
        {
            if(double.IsNaN(a) || double.IsInfinity(a))
                throw new ArgumentException(Resources.ArgumentNotInfinityNaN, "a");
            if(double.IsNaN(b) || double.IsInfinity(b))
                throw new ArgumentException(Resources.ArgumentNotInfinityNaN, "b");

            ulong ua = ToLexicographicalOrderedUInt64(a);
            ulong ub = ToLexicographicalOrderedUInt64(b);

            return (a >= b) ? ua - ub : ub - ua;
        }

        [CLSCompliant(false)]
        public static ulong ToLexicographicalOrderedUInt64(double value)
        {
            long signed64 = BitConverter.DoubleToInt64Bits(value);
            ulong unsigned64 = (ulong)signed64;
            return (signed64 >= 0) ? unsigned64 : 0x8000000000000000 - unsigned64;
        }

        public static long ToLexicographicalOrderedInt64(double value)
        {
            long signed64 = BitConverter.DoubleToInt64Bits(value);
            return (signed64 >= 0) ? signed64 : (long)(0x8000000000000000 - (ulong)signed64);
        }

        [CLSCompliant(false)]
        public static ulong SignedMagnitudeToTwosComplementUInt64(long value)
        {
            return (value >= 0) ? (ulong)value : 0x8000000000000000 - (ulong)value;
        }

        public static long SignedMagnitudeToTwosComplementInt64(long value)
        {
            return (value >= 0) ? value : (long)(0x8000000000000000 - (ulong)value);
        }

        /// <param name="maxNumbersBetween">The maximum count of numbers between the two numbers plus one ([a,a] -> 0, [a,a+e] -> 1, [a,a+2e] -> 2, ...).</param>
        public static bool AlmostEqual(double a, double b, int maxNumbersBetween)
        {
            return AlmostEqual(a, b, (ulong)maxNumbersBetween);
        }
        
        /// <param name="maxNumbersBetween">The maximum count of numbers between the two numbers plus one ([a,a] -> 0, [a,a+e] -> 1, [a,a+2e] -> 2, ...).</param>
        public static bool AlmostEqual(double a, double b, ulong maxNumbersBetween)
        {
            if(maxNumbersBetween < 0)
                throw new ArgumentException(Resources.ArgumentNotNegative, "maxNumbersBetween");

            // NaN's should never equal to anything
            if(double.IsNaN(a) || double.IsNaN(b)) //(a != a || b != b)
                return false;

            if(a == b)
                return true;

            // false, if only one of them is infinity or they differ on the infinity sign
            if(double.IsInfinity(a) || double.IsInfinity(b))
                return false;

            ulong between = NumbersBetween(a, b);
            return between <= maxNumbersBetween;
        }
    }
}
