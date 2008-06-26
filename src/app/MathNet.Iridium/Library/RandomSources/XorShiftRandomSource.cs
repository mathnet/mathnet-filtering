#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
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
#region Derived From: Copyright 2006 Troschütz
/* 
 * Derived from the Troschuetz.Random Class Library,
 * Copyright © 2006 Stefan Troschütz (stefan@troschuetz.de)
 * 
 * Troschuetz.Random is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA 
 */
#endregion
#region Derived From: Copyright 2005 Green
// A fast random number generator for .NET
// Colin Green, January 2005
//
// September 4th 2005
// Added NextBytesUnsafe() - commented out by default.
// Fixed bug in Reinitialise() - y,z and w variables were not being reset.
//
// Key points:
// 1) Based on a simple and fast xor-shift pseudo random number generator (RNG) specified in: 
// Marsaglia, George. (2003). Xorshift RNGs.
// http://www.jstatsoft.org/v08/i14/xorshift.pdf
//
// This particular implementation of xorshift has a period of 2^128-1. See the above paper to see
// how this can be easily extended if you need a longer period. At the time of writing I could find no 
// information on the period of System.Random for comparison.
//
// 2) Faster than System.Random. Up to 15x faster, depending on which methods are called.
//
// 3) Direct replacement for System.Random. This class implements all of the methods that System.Random 
// does plus some additional methods. The like named methods are functionally equivalent.
//
// 4) Allows fast re-initialisation with a seed, unlike System.Random which accepts a seed at construction
// time which then executes a relatively expensive initialisation routine. This provides a vast speed improvement
// if you need to reset the pseudo-random number sequence many times, e.g. if you want to re-generate the same
// sequence many times. An alternative might be to cache random numbers in an array, but that approach is limited
// by memory capacity and the fact that you may also want a large number of different sequences cached. Each sequence
// can each be represented by a single seed value (int) when using FastRandom.
//
// Notes.
// A further performance improvement can be obtained by declaring local variables as static, thus avoiding 
// re-allocation of variables on each call. However care should be taken if multiple instances of
// FastRandom are in use or if being used in a multi-threaded environment.
#endregion

using System;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.RandomSources
{
    /// <summary>
    /// Represents a xorshift pseudo-random number generator with period 2^128-1.
    /// </summary>
    /// <remarks>
    /// The <see cref="XorShiftRandomSource"/> type bases upon the implementation presented in the CP article
    ///   "<a href="http://www.codeproject.com/csharp/fastrandom.asp">A fast equivalent for System.Random</a>"
    ///   and the theoretical background on xorshift random number generators published by George Marsaglia 
    ///   in this paper "<a href="http://www.jstatsoft.org/v08/i14/xorshift.pdf">Xorshift RNGs</a>".
    /// </remarks>
    public class XorShiftRandomSource :
        RandomSource
    {
        /// <summary>
        /// Represents the seed for the <see cref="_y"/> variable. This field is constant.
        /// </summary>
        /// <remarks>The value of this constant is 362436069.</remarks>
        const uint SeedY = 362436069;

        /// <summary>
        /// Represents the seed for the <see cref="_z"/> variable. This field is constant.
        /// </summary>
        /// <remarks>The value of this constant is 521288629.</remarks>
        const uint SeedZ = 521288629;

        /// <summary>
        /// Represents the seed for the <see cref="_w"/> variable. This field is constant.
        /// </summary>
        /// <remarks>The value of this constant is 88675123.</remarks>
        const uint SeedW = 88675123;

        /// <summary>
        /// Represents the multiplier that computes a double-precision floating point number greater than or equal to 0.0 
        ///   and less than 1.0 when it gets applied to a nonnegative 32-bit signed integer.
        /// </summary>
        const double IntToDoubleMultiplier = 1.0 / ((double)int.MaxValue + 1.0);

        /// <summary>
        /// Represents the multiplier that computes a double-precision floating point number greater than or equal to 0.0 
        ///   and less than 1.0  when it gets applied to a 32-bit unsigned integer.
        /// </summary>
        const double UIntToDoubleMultiplier = 1.0 / ((double)uint.MaxValue + 1.0);

        /// <summary>
        /// Stores the last but three unsigned random number. 
        /// </summary>
        uint _x;

        /// <summary>
        /// Stores the last but two unsigned random number. 
        /// </summary>
        uint _y;

        /// <summary>
        /// Stores the last but one unsigned random number. 
        /// </summary>
        uint _z;

        /// <summary>
        /// Stores the last generated unsigned random number. 
        /// </summary>
        uint _w;

        /// <summary>
        /// Stores the used seed value.
        /// </summary>
        uint _seed;

        /// <summary>
        /// Stores an <see cref="UInt32"/> used to generate up to 32 random <see cref="Boolean"/> values.
        /// </summary>
        uint _bitBuffer;

        /// <summary>
        /// Stores how many random <see cref="Boolean"/> values still can be generated from <see cref="_bitBuffer"/>.
        /// </summary>
        int _bitCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="XorShiftRandomSource"/> class, using a time-dependent default 
        ///   seed value.
        /// </summary>
        public
        XorShiftRandomSource()
            : this((uint)Math.Abs(Environment.TickCount))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorShiftRandomSource"/> class, using the specified seed value.
        /// </summary>
        /// <param name="seed">
        /// A number used to calculate a starting value for the pseudo-random number sequence.
        /// If a negative number is specified, the absolute value of the number is used. 
        /// </param>
        public
        XorShiftRandomSource(
            int seed
            )
            : this((uint)Math.Abs(seed))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XorShiftRandomSource"/> class, using the specified seed value.
        /// </summary>
        /// <param name="seed">
        /// An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        [CLSCompliant(false)]
        public
        XorShiftRandomSource(
            uint seed
            )
        {
            _seed = seed;
            ResetGenerator();
        }

        /// <summary>
        /// Resets the <see cref="XorShiftRandomSource"/>,
        /// so that it produces the same pseudo-random number sequence again.
        /// </summary>
        void
        ResetGenerator()
        {
            // "The seed set for xor128 is four 32-bit integers x,y,z,w not all 0, ..." (George Marsaglia)
            // To meet that requirement the y, z, w seeds are constant values greater 0.
            _x = _seed;
            _y = SeedY;
            _z = SeedZ;
            _w = SeedW;

            // Reset helper variables used for generation of random bools.
            _bitBuffer = 0;
            _bitCount = 0;
        }

        /// <summary>
        /// Returns an unsigned random number.
        /// </summary>
        /// <returns>
        /// A 32-bit unsigned integer greater than or equal to <see cref="UInt32.MinValue"/> and 
        ///   less than or equal to <see cref="UInt32.MaxValue"/>.
        /// </returns>
        [CLSCompliant(false)]
        public
        uint
        NextUInt()
        {
            uint t = (_x ^ (_x << 11));
            _x = _y;
            _y = _z;
            _z = _w;
            return (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));
        }

        /// <summary>
        /// Returns a nonnegative random number less than or equal to <see cref="Int32.MaxValue"/>.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to 0, and less than or equal to <see cref="Int32.MaxValue"/>; 
        ///   that is, the range of return values includes 0 and <paramref name="Int32.MaxValue"/>.
        /// </returns>
        public
        int
        NextInclusiveMaxValue()
        {
            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            uint t = (_x ^ (_x << 11));
            _x = _y;
            _y = _z;
            _z = _w;
            uint w = (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));

            return (int)(w >> 1);
        }

        /// <summary>
        /// Returns a nonnegative random number less than <see cref="Int32.MaxValue"/>.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to 0, and less than <see cref="Int32.MaxValue"/>; that is, 
        ///   the range of return values includes 0 but not <paramref name="Int32.MaxValue"/>.
        /// </returns>
        public override
        int
        Next()
        {
            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            uint t = (_x ^ (_x << 11));
            _x = _y;
            _y = _z;
            _z = _w;
            uint w = (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));

            int result = (int)(w >> 1);

            // Exclude Int32.MaxValue from the range of return values.
            if(result == Int32.MaxValue)
            {
                return Next();
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. 
        /// <paramref name="maxValue"/> must be greater than or equal to 0. 
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to 0, and less than <paramref name="maxValue"/>; that is, 
        ///   the range of return values includes 0 but not <paramref name="maxValue"/>. 
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxValue"/> is less than 0. 
        /// </exception>
        public override
        int
        Next(
            int maxValue
            )
        {
            if(maxValue < 0)
            {
                string message = string.Format(
                    Resources.ArgumentOutOfRangeGreaterEqual,
                    "maxValue",
                    "0"
                    );

                throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
            }

            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            uint t = (_x ^ (_x << 11));
            _x = _y;
            _y = _z;
            _z = _w;
            uint w = (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return (int)((double)(int)(w >> 1) * IntToDoubleMultiplier * (double)maxValue);
        }

        /// <summary>
        /// Returns a random number within the specified range. 
        /// </summary>
        /// <param name="minValue">
        /// The inclusive lower bound of the random number to be generated. 
        /// </param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. 
        /// <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>. 
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to <paramref name="minValue"/>, and less than 
        ///   <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but 
        ///   not <paramref name="maxValue"/>. 
        /// If <paramref name="minValue"/> equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.  
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
        /// </exception>
        public override
        int
        Next(
            int minValue,
            int maxValue
            )
        {
            if(minValue > maxValue)
            {
                string message = string.Format(
                    Resources.ArgumentOutOfRangeGreaterEqual,
                   "maxValue",
                   "minValue"
                   );

                throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
            }

            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            uint t = (_x ^ (_x << 11));
            _x = _y;
            _y = _z;
            _z = _w;
            uint w = (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));

            int range = maxValue - minValue;
            if(range < 0)
            {
                // The range is greater than Int32.MaxValue, so we have to use slower floating point arithmetic.
                // Also all 32 random bits (uint) have to be used which again is slower (See comment in NextDouble()).
                return minValue + (int)
                    ((double)w * UIntToDoubleMultiplier * ((double)maxValue - (double)minValue));
            }
            else
            {
                // 31 random bits (int) will suffice which allows us to shift and cast to an int before the first multiplication and gain better performance.
                // See comment in NextDouble().
                return minValue + (int)((double)(int)(w >> 1) * IntToDoubleMultiplier * (double)range);
            }
        }

        /// <summary>
        /// Returns a nonnegative floating point random number less than 1.0.
        /// </summary>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than 1.0; that is, 
        ///   the range of return values includes 0.0 but not 1.0.
        /// </returns>
        public override
        double
        NextDouble()
        {
            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            uint t = (_x ^ (_x << 11));
            _x = _y;
            _y = _z;
            _z = _w;
            uint w = (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));

            // Here a ~2x speed improvement is gained by computing a value that can be cast to an int 
            //   before casting to a double to perform the multiplication.
            // Casting a double from an int is a lot faster than from an uint and the extra shift operation 
            //   and cast to an int are very fast (the allocated bits remain the same), so overall there's 
            //   a significant performance improvement.
            return (double)(int)(w >> 1) * IntToDoubleMultiplier;
        }

        /// <summary>
        /// Returns a nonnegative floating point random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. 
        /// <paramref name="maxValue"/> must be greater than or equal to 0.0. 
        /// </param>
        /// <returns>
        /// A double-precision floating point number greater than or equal to 0.0, and less than <paramref name="maxValue"/>; 
        ///   that is, the range of return values includes 0 but not <paramref name="maxValue"/>. 
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxValue"/> is less than 0. 
        /// </exception>
        public override
        double
        NextDouble(
            double maxValue
            )
        {
            if(maxValue < 0.0)
            {
                string message = string.Format(
                    Resources.ArgumentOutOfRangeGreaterEqual,
                    "maxValue",
                    "0.0"
                    );

                throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
            }

            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            uint t = (_x ^ (_x << 11));
            _x = _y;
            _y = _z;
            _z = _w;
            uint w = (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return (double)(int)(w >> 1) * IntToDoubleMultiplier * maxValue;
        }

        /// <summary>
        /// Returns a floating point random number within the specified range. 
        /// </summary>
        /// <param name="minValue">
        /// The inclusive lower bound of the random number to be generated. 
        /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> must be less than or equal to
        ///   <see cref="Double.MaxValue"/>
        /// </param>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. 
        /// <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.
        /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> must be less than or equal to
        ///   <see cref="Double.MaxValue"/>.
        /// </param>
        /// <returns>
        /// A double-precision floating point number greater than or equal to <paramref name="minValue"/>, and less than 
        ///   <paramref name="maxValue"/>; that is, the range of return values includes <paramref name="minValue"/> but 
        ///   not <paramref name="maxValue"/>. 
        /// If <paramref name="minValue"/> equals <paramref name="maxValue"/>, <paramref name="minValue"/> is returned.  
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> is greater than
        ///   <see cref="Double.MaxValue"/>.
        /// </exception>
        public override
        double
        NextDouble(
            double minValue,
            double maxValue
            )
        {
            if(minValue > maxValue)
            {
                string message = string.Format(
                    Resources.ArgumentOutOfRangeGreaterEqual,
                    "maxValue",
                    "minValue"
                    );

                throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
            }

            double range = maxValue - minValue;

            if(range == double.PositiveInfinity)
            {
                string message = string.Format(
                    Resources.ArgumentRangeLessEqual,
                    "minValue",
                    "maxValue",
                    "Double.MaxValue"
                    );

                throw new ArgumentException(message);
            }

            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            uint t = (_x ^ (_x << 11));
            _x = _y;
            _y = _z;
            _z = _w;
            uint w = (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return minValue + (double)(int)(w >> 1) * IntToDoubleMultiplier * range;
        }

        /// <summary>
        /// Returns a random Boolean value.
        /// </summary>
        /// <remarks>
        /// <remarks>
        /// Buffers 32 random bits (1 uint) for future calls, so a new random number is only generated every 32 calls.
        /// </remarks>
        /// </remarks>
        /// <returns>A <see cref="Boolean"/> value.</returns>
        public override
        bool
        NextBoolean()
        {
            if(_bitCount == 0)
            {
                // Generate 32 more bits (1 uint) and store it for future calls.
                // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
                uint t = (_x ^ (_x << 11));
                _x = _y;
                _y = _z;
                _z = _w;
                _bitBuffer = (_w = (_w ^ (_w >> 19)) ^ (t ^ (t >> 8)));

                // Reset the bitCount and use rightmost bit of buffer to generate random bool.
                _bitCount = 31;
                return (_bitBuffer & 0x1) == 1;
            }

            // Decrease the bitCount and use rightmost bit of shifted buffer to generate random bool.
            _bitCount--;
            return ((_bitBuffer >>= 1) & 0x1) == 1;
        }

        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers. 
        /// </summary>
        /// <remarks>
        /// Each element of the array of bytes is set to a random number greater than or equal to 0, and less than or 
        ///   equal to <see cref="Byte.MaxValue"/>.
        /// </remarks>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="buffer"/> is a null reference (<see langword="Nothing"/> in Visual Basic). 
        /// </exception>
        public override
        void
        NextBytes(
            byte[] buffer
            )
        {
            if(buffer == null)
            {
                string message = string.Format(
                    Resources.ArgumentNull,
                    "buffer"
                    );

                throw new ArgumentNullException("buffer", message);
            }

            // Use local copies of x,y,z and w for better performance.
            uint x = _x;
            uint y = _y;
            uint z = _z;
            uint w = _w;

            // Fill the buffer with 4 bytes (1 uint) at a time.
            int i = 0;
            uint t;
            while(i < buffer.Length - 3)
            {
                // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
                t = (x ^ (x << 11));
                x = y;
                y = z;
                z = w;
                w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                buffer[i++] = (byte)w;
                buffer[i++] = (byte)(w >> 8);
                buffer[i++] = (byte)(w >> 16);
                buffer[i++] = (byte)(w >> 24);
            }

            // Fill up any remaining bytes in the buffer.
            if(i < buffer.Length)
            {
                // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
                t = (x ^ (x << 11));
                x = y;
                y = z;
                z = w;
                w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));

                buffer[i++] = (byte)w;
                if(i < buffer.Length)
                {
                    buffer[i++] = (byte)(w >> 8);
                    if(i < buffer.Length)
                    {
                        buffer[i++] = (byte)(w >> 16);
                        if(i < buffer.Length)
                        {
                            buffer[i] = (byte)(w >> 24);
                        }
                    }
                }
            }

            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }

        /// <summary>
        /// Resets the <see cref="XorShiftRandomSource"/>, so that it produces the same pseudo-random number sequence again.
        /// </summary>
        public override
        void
        Reset()
        {
            ResetGenerator();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="XorShiftRandomSource"/> can be reset, so that it produces the 
        ///   same pseudo-random number sequence again.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }
    }
}
