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
#region Derived From: Copyright 2002 Nishimura, Matsumoto
// A C-program for MT19937, with initialization improved 2002/1/26.
// Coded by Takuji Nishimura and Makoto Matsumoto.
//
// Before using, initialize the state by using init_genrand(seed)  
// or init_by_array(init_key, key_length).
//
// Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura,
// All rights reserved.                          
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
//
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
//
// 3. The names of its contributors may not be used to endorse or promote 
//    products derived from this software without specific prior written 
//    permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// Any feedback is very welcome.
// http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
// email: m-mat @ math.sci.hiroshima-u.ac.jp (remove space)
#endregion

using System;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.RandomSources
{
    /// <summary>
    /// Represents a Mersenne Twister pseudo-random number generator with period 2^19937-1.
    /// </summary>
    /// <remarks>
    /// The <see cref="MersenneTwisterRandomSource"/> type bases upon information and the implementation presented on the
    ///   <a href="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html">Mersenne Twister Home Page</a>.
    /// </remarks>
    public class MersenneTwisterRandomSource :
        RandomSource
    {
        /// <summary>
        /// Represents the number of unsigned random numbers generated at one time. This field is constant.
        /// </summary>
        /// <remarks>The value of this constant is 624.</remarks>
        const int N = 624;

        /// <summary>
        /// Represents a constant used for generation of unsigned random numbers. This field is constant.
        /// </summary>
        /// <remarks>The value of this constant is 397.</remarks>
        const int M = 397;

        /// <summary>
        /// Represents the constant vector a. This field is constant.
        /// </summary>
        /// <remarks>The value of this constant is 0x9908b0dfU.</remarks>
        const uint VectorA = 0x9908b0dfU;

        /// <summary>
        /// Represents the most significant w-r bits. This field is constant.
        /// </summary>
        /// <remarks>The value of this constant is 0x80000000.</remarks>
        const uint UpperMask = 0x80000000U;

        /// <summary>
        /// Represents the least significant r bits. This field is constant.
        /// </summary>
        /// <remarks>The value of this constant is 0x7fffffff.</remarks>
        const uint LowerMask = 0x7fffffffU;

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
        /// Stores the state vector array.
        /// </summary>
        uint[] _mt;

        /// <summary>
        /// Stores an index for the state vector array element that will be accessed next.
        /// </summary>
        uint _mti;

        /// <summary>
        /// Stores the used seed value.
        /// </summary>
        uint _seed;

        /// <summary>
        /// Stores the used seed array.
        /// </summary>
        uint[] _seedArray;

        /// <summary>
        /// Stores an <see cref="UInt32"/> used to generate up to 32 random <see cref="Boolean"/> values.
        /// </summary>
        uint _bitBuffer;

        /// <summary>
        /// Stores how many random <see cref="Boolean"/> values still can be generated from <see cref="_bitBuffer"/>.
        /// </summary>
        int _bitCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwisterRandomSource"/> class, using a time-dependent default 
        ///   seed value.
        /// </summary>
        public
        MersenneTwisterRandomSource()
            : this((uint)Math.Abs(Environment.TickCount))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwisterRandomSource"/> class, using the specified seed value.
        /// </summary>
        /// <param name="seed">
        /// A number used to calculate a starting value for the pseudo-random number sequence.
        /// If a negative number is specified, the absolute value of the number is used. 
        /// </param>
        public
        MersenneTwisterRandomSource(
            int seed
            )
            : this((uint)Math.Abs(seed))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwisterRandomSource"/> class, using the specified seed value.
        /// </summary>
        /// <param name="seed">
        /// An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        [CLSCompliant(false)]
        public
        MersenneTwisterRandomSource(
            uint seed
            )
        {
            _mt = new uint[N];
            _seed = seed;
            ResetGenerator();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwisterRandomSource"/> class, using the specified seed array.
        /// </summary>
        /// <param name="seedArray">
        /// An array of numbers used to calculate a starting values for the pseudo-random number sequence.
        /// If negative numbers are specified, the absolute values of them are used. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seedArray"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        public
        MersenneTwisterRandomSource(
            int[] seedArray
            )
        {
            if(seedArray == null)
            {
                string message = string.Format(
                    Resources.ArgumentNull,
                    "seedArray"
                    );

                throw new ArgumentNullException("seedArray", message);
            }

            _mt = new uint[N];
            _seed = 19650218U;

            _seedArray = new uint[seedArray.Length];
            for(int index = 0; index < seedArray.Length; index++)
            {
                _seedArray[index] = (uint)Math.Abs(seedArray[index]);
            }

            ResetGenerator();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwisterRandomSource"/> class, using the specified seed array.
        /// </summary>
        /// <param name="seedArray">
        /// An array of unsigned numbers used to calculate a starting values for the pseudo-random number sequence.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="seedArray"/> is NULL (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        [CLSCompliant(false)]
        public
        MersenneTwisterRandomSource(
            uint[] seedArray
            )
        {
            if(seedArray == null)
            {
                string message = string.Format(
                    Resources.ArgumentNull,
                    "seedArray"
                    );

                throw new ArgumentNullException("seedArray", message);
            }

            _mt = new uint[N];
            _seed = 19650218U;
            _seedArray = seedArray;
            ResetGenerator();
        }

        /// <summary>
        /// Resets the <see cref="MersenneTwisterRandomSource"/>,
        /// so that it produces the same pseudo-random number sequence again.
        /// </summary>
        void
        ResetGenerator()
        {
            _mt[0] = _seed & 0xffffffffU;
            for(_mti = 1; _mti < N; _mti++)
            {
                _mt[_mti] = (1812433253U * (_mt[_mti - 1] ^ (_mt[_mti - 1] >> 30)) + _mti);

                // See Knuth TAOCP Vol2. 3rd Ed. P.106 for multiplier.
                // In the previous versions, MSBs of the seed affect only MSBs of the array mt[].
                // 2002/01/09 modified by Makoto Matsumoto
            }

            // If the object was instantiated with a seed array do some further (re)initialisation.
            if(_seedArray != null)
            {
                ResetBySeedArray();
            }

            // Reset helper variables used for generation of random booleans.
            _bitBuffer = 0;
            _bitCount = 32;
        }

        /// <summary>
        /// Extends resetting of the <see cref="MersenneTwisterRandomSource"/> using the <see cref="_seedArray"/>.
        /// </summary>
        void
        ResetBySeedArray()
        {
            uint i = 1;
            uint j = 0;
            int k = (N > _seedArray.Length) ? N : _seedArray.Length;
            for(; k > 0; k--)
            {
                _mt[i] = (_mt[i] ^ ((_mt[i - 1] ^ (_mt[i - 1] >> 30)) * 1664525U)) + _seedArray[j] + j; // non linear
                i++;
                j++;

                if(i >= N)
                {
                    _mt[0] = _mt[N - 1];
                    i = 1;
                }

                if(j >= _seedArray.Length)
                {
                    j = 0;
                }
            }

            for(k = N - 1; k > 0; k--)
            {
                _mt[i] = (_mt[i] ^ ((_mt[i - 1] ^ (_mt[i - 1] >> 30)) * 1566083941U)) - i; // non linear
                i++;
                if(i >= N)
                {
                    _mt[0] = _mt[N - 1];
                    i = 1;
                }
            }

            _mt[0] = 0x80000000U; // MSB is 1; assuring non-0 initial array
        }

        /// <summary>
        /// Generates <see cref="MersenneTwisterRandomSource.N"/> unsigned random numbers.
        /// </summary>
        /// <remarks>
        /// Generated random numbers are 32-bit unsigned integers greater than or equal to <see cref="UInt32.MinValue"/> 
        ///   and less than or equal to <see cref="UInt32.MaxValue"/>.
        /// </remarks>
        void
        GenerateNUInts()
        {
            int kk;
            uint y;
            uint[] mag01 = new uint[2] { 0x0U, VectorA };

            for(kk = 0; kk < N - M; kk++)
            {
                y = (_mt[kk] & UpperMask) | (_mt[kk + 1] & LowerMask);
                _mt[kk] = _mt[kk + M] ^ (y >> 1) ^ mag01[y & 0x1U];
            }

            for(; kk < N - 1; kk++)
            {
                y = (_mt[kk] & UpperMask) | (_mt[kk + 1] & LowerMask);
                _mt[kk] = _mt[kk + (M - N)] ^ (y >> 1) ^ mag01[y & 0x1U];
            }

            y = (_mt[N - 1] & UpperMask) | (_mt[0] & LowerMask);
            _mt[N - 1] = _mt[M - 1] ^ (y >> 1) ^ mag01[y & 0x1U];

            _mti = 0;
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
            if(_mti >= N)
            {
                // generate N words at one time
                GenerateNUInts();
            }

            uint y = _mt[_mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            return (y ^ (y >> 18));
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
            if(_mti >= N)
            {
                // generate N words at one time
                GenerateNUInts();
            }

            uint y = _mt[_mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            return (int)(y >> 1);
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
            if(_mti >= N)
            {
                // generate N words at one time
                GenerateNUInts();
            }

            uint y = _mt[_mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            int result = (int)(y >> 1);

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
            if(_mti >= N)
            {
                // generate N words at one time
                GenerateNUInts();
            }

            uint y = _mt[_mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return (int)((double)(int)(y >> 1) * IntToDoubleMultiplier * (double)maxValue);
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
            if(_mti >= N)
            {
                // generate N words at one time
                GenerateNUInts();
            }

            uint y = _mt[_mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            int range = maxValue - minValue;
            if(range < 0)
            {
                // The range is greater than Int32.MaxValue, so we have to use slower floating point arithmetic.
                // Also all 32 random bits (uint) have to be used which again is slower (See comment in NextDouble()).
                return minValue + (int)
                    ((double)y * UIntToDoubleMultiplier * ((double)maxValue - (double)minValue));
            }
            else
            {
                // 31 random bits (int) will suffice which allows us to shift and cast to an int before the first multiplication and gain better performance.
                // See comment in NextDouble().
                return minValue + (int)((double)(int)(y >> 1) * IntToDoubleMultiplier * (double)range);
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
            if(_mti >= N)
            {
                // generate N words at one time
                GenerateNUInts();
            }

            uint y = _mt[_mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            // Here a ~2x speed improvement is gained by computing a value that can be cast to an int 
            //   before casting to a double to perform the multiplication.
            // Casting a double from an int is a lot faster than from an uint and the extra shift operation 
            //   and cast to an int are very fast (the allocated bits remain the same), so overall there's 
            //   a significant performance improvement.
            return (double)(int)(y >> 1) * IntToDoubleMultiplier;
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
            if(_mti >= N)
            {
                // generate N words at one time
                GenerateNUInts();
            }

            uint y = _mt[_mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return (double)(int)(y >> 1) * IntToDoubleMultiplier * maxValue;
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
            if(_mti >= N)
            {
                // generate N words at one time
                GenerateNUInts();
            }

            uint y = _mt[_mti++];

            // Tempering
            y ^= (y >> 11);
            y ^= (y << 7) & 0x9d2c5680U;
            y ^= (y << 15) & 0xefc60000U;
            y ^= (y >> 18);

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return minValue + (double)(int)(y >> 1) * IntToDoubleMultiplier * range;
        }

        /// <summary>
        /// Returns a random Boolean value.
        /// </summary>
        /// <remarks>
        /// Buffers 32 random bits (1 uint) for future calls, so a new random number is only generated every 32 calls.
        /// </remarks>
        /// <returns>A <see cref="Boolean"/> value.</returns>
        public override
        bool
        NextBoolean()
        {
            if(_bitCount == 32)
            {
                // Generate 32 more bits (1 uint) and store it for future calls.
                // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
                if(_mti >= N)
                {
                    // generate N words at one time
                    GenerateNUInts();
                }

                uint y = _mt[_mti++];

                // Tempering
                y ^= (y >> 11);
                y ^= (y << 7) & 0x9d2c5680U;
                y ^= (y << 15) & 0xefc60000U;
                _bitBuffer = (y ^ (y >> 18));

                // Reset the bitCount and use rightmost bit of buffer to generate random bool.
                _bitCount = 1;
                return (_bitBuffer & 0x1) == 1;
            }

            // Increase the bitCount and use rightmost bit of shifted buffer to generate random bool.
            _bitCount++;
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

            // Fill the buffer with 4 bytes (1 uint) at a time.
            int i = 0;
            uint y;
            while(i < buffer.Length - 3)
            {
                // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
                if(_mti >= N)
                {
                    // generate N words at one time
                    GenerateNUInts();
                }

                y = _mt[_mti++];

                // Tempering
                y ^= (y >> 11);
                y ^= (y << 7) & 0x9d2c5680U;
                y ^= (y << 15) & 0xefc60000U;
                y ^= (y >> 18);

                buffer[i++] = (byte)y;
                buffer[i++] = (byte)(y >> 8);
                buffer[i++] = (byte)(y >> 16);
                buffer[i++] = (byte)(y >> 24);
            }

            // Fill up any remaining bytes in the buffer.
            if(i < buffer.Length)
            {
                // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
                if(_mti >= N)
                {
                    // generate N words at one time
                    GenerateNUInts();
                }

                y = _mt[_mti++];

                // Tempering
                y ^= (y >> 11);
                y ^= (y << 7) & 0x9d2c5680U;
                y ^= (y << 15) & 0xefc60000U;
                y ^= (y >> 18);

                buffer[i++] = (byte)y;
                if(i < buffer.Length)
                {
                    buffer[i++] = (byte)(y >> 8);
                    if(i < buffer.Length)
                    {
                        buffer[i++] = (byte)(y >> 16);
                        if(i < buffer.Length)
                        {
                            buffer[i] = (byte)(y >> 24);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the <see cref="MersenneTwisterRandomSource"/>, so that it produces the same pseudo-random number sequence again.
        /// </summary>
        public override
        void
        Reset()
        {
            ResetGenerator();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MersenneTwisterRandomSource"/> can be reset, so that it produces the 
        ///   same pseudo-random number sequence again.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }
    }
}
