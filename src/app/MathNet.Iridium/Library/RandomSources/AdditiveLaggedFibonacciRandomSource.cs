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
#region Derived From: Copyright 2000-2001 Maurer
/* boost random/lagged_fibonacci.hpp header file
 *
 * Copyright Jens Maurer 2000-2001
 * Distributed under the Boost Software License, Version 1.0. (See
 * accompanying file LICENSE_1_0.txt or copy at
 * http://www.boost.org/LICENSE_1_0.txt)
 *
 * See http://www.boost.org for most recent version including documentation.
 *
 * $Id: lagged_fibonacci.hpp,v 1.28 2005/05/21 15:57:00 dgregor Exp $
 *
 * Revision history
 *  2001-02-18  moved to individual header files
 */
#endregion

using System;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.RandomSources
{
    /// <summary>
    /// Represents a Additive Lagged Fibonacci pseudo-random number generator.
    /// </summary>
    /// <remarks>
    /// The <see cref="AdditiveLaggedFibonacciRandomSource"/> type bases upon the implementation in the 
    ///   <a href="http://www.boost.org/libs/random/index.html">Boost Random Number Library</a>.
    /// It uses the modulus 2<sup>32</sup> and by default the "lags" 418 and 1279, which can be adjusted through the 
    ///   associated <see cref="ShortLag"/> and <see cref="LongLag"/> properties. Some popular pairs are presented on 
    ///   <a href="http://en.wikipedia.org/wiki/Lagged_Fibonacci_generator">Wikipedia - Lagged Fibonacci generator</a>.
    /// </remarks>
    public class AdditiveLaggedFibonacciRandomSource :
        RandomSource
    {
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
        /// Stores the short lag of the Lagged Fibonacci pseudo-random number generator.
        /// </summary>
        int _shortLag;

        /// <summary>
        /// Stores the long lag of the Lagged Fibonacci pseudo-random number generator.
        /// </summary>
        int _longLag;

        /// <summary>
        /// Stores an array of <see cref="_longLag"/> random numbers
        /// </summary>
        uint[] _x;

        /// <summary>
        /// Stores an index for the random number array element that will be accessed next.
        /// </summary>
        int _i;

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
        /// Initializes a new instance of the <see cref="AdditiveLaggedFibonacciRandomSource"/> class, using a time-dependent default 
        ///   seed value.
        /// </summary>
        public
        AdditiveLaggedFibonacciRandomSource()
            : this((uint)Math.Abs(Environment.TickCount))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveLaggedFibonacciRandomSource"/> class, using the specified seed value.
        /// </summary>
        /// <param name="seed">
        /// A number used to calculate a starting value for the pseudo-random number sequence.
        /// If a negative number is specified, the absolute value of the number is used. 
        /// </param>
        public
        AdditiveLaggedFibonacciRandomSource(
            int seed
            )
            : this((uint)Math.Abs(seed))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditiveLaggedFibonacciRandomSource"/> class, using the specified seed value.
        /// </summary>
        /// <param name="seed">
        /// An unsigned number used to calculate a starting value for the pseudo-random number sequence.
        /// </param>
        [CLSCompliant(false)]
        public
        AdditiveLaggedFibonacciRandomSource(
            uint seed
            )
        {
            _seed = seed;
            _shortLag = 418;
            _longLag = 1279;
            ResetGenerator();
        }

        /// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="ShortLag"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <see langword="true"/> if value is greater than 0; otherwise, <see langword="false"/>.
        /// </returns>
        public
        bool
        IsValidShortLag(
            int value
            )
        {
            return value > 0;
        }

        /// <summary>
        /// Determines whether the specified value is valid for parameter <see cref="LongLag"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>
        /// <see langword="true"/> if value is greater than <see cref="ShortLag"/>; otherwise, <see langword="false"/>.
        /// </returns>
        public
        bool
        IsValidLongLag(
            int value
            )
        {
            return value > _shortLag;
        }

        /// <summary>
        /// Resets the <see cref="AdditiveLaggedFibonacciRandomSource"/>,
        /// so that it produces the same pseudo-random number sequence again.
        /// </summary>
        void
        ResetGenerator()
        {
            MersenneTwisterRandomSource gen = new MersenneTwisterRandomSource(_seed);
            
            _x = new uint[_longLag];
            for(uint j = 0; j < _longLag; ++j)
            {
                _x[j] = gen.NextUInt();
            }

            _i = _longLag;

            // Reset helper variables used for generation of random bools.
            _bitBuffer = 0;
            _bitCount = 0;
        }

        /// <summary>
        /// Fills the array <see cref="_x"/> with <see cref="_longLag"/> new unsigned random numbers.
        /// </summary>
        /// <remarks>
        /// Generated random numbers are 32-bit unsigned integers greater than or equal to <see cref="UInt32.MinValue"/> 
        ///   and less than or equal to <see cref="UInt32.MaxValue"/>.
        /// </remarks>
        void
        Fill()
        {
            // two loops to avoid costly modulo operations
            for(int j = 0; j < _shortLag; ++j)
            {
                _x[j] = _x[j] + _x[j + (_longLag - _shortLag)];
            }

            for(int j = _shortLag; j < _longLag; ++j)
            {
                _x[j] = _x[j] + _x[j - _shortLag];
            }

            _i = 0;
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
            if(_i >= _longLag)
            {
                Fill();
            }

            return _x[_i++];
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
            if(_i >= _longLag)
            {
                Fill();
            }

            uint x = _x[_i++];
            return (int)(x >> 1);
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
            if(_i >= _longLag)
            {
                Fill();
            }

            uint x = _x[_i++];
            int result = (int)(x >> 1);

            // Exclude Int32.MaxValue from the range of return values.
            if(result == Int32.MaxValue)
            {
                return Next();
            }

            return result;
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
                string message = string.Format(null, Resources.ArgumentOutOfRangeGreaterEqual, "maxValue", "0");
                throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
            }

            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            if(_i >= _longLag)
            {
                Fill();
            }

            uint x = _x[_i++];

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return (int)((double)(int)(x >> 1) * IntToDoubleMultiplier * (double)maxValue);
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
                string message = string.Format(null, Resources.ArgumentOutOfRangeGreaterEqual, "maxValue", "minValue");
                throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
            }

            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            if(_i >= _longLag)
            {
                Fill();
            }

            uint x = _x[_i++];

            int range = maxValue - minValue;
            if(range < 0)
            {
                // The range is greater than Int32.MaxValue, so we have to use slower floating point arithmetic.
                // Also all 32 random bits (uint) have to be used which again is slower (See comment in NextDouble()).
                return minValue + (int)
                    ((double)x * UIntToDoubleMultiplier * ((double)maxValue - (double)minValue));
            }
            else
            {
                // 31 random bits (int) will suffice which allows us to shift and cast to an int before the first multiplication and gain better performance.
                // See comment in NextDouble().
                return minValue + (int)
                    ((double)(int)(x >> 1) * IntToDoubleMultiplier * (double)range);
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
            if(_i >= _longLag)
            {
                Fill();
            }

            uint x = _x[_i++];

            // Here a ~2x speed improvement is gained by computing a value that can be cast to an int 
            //   before casting to a double to perform the multiplication.
            // Casting a double from an int is a lot faster than from an uint and the extra shift operation 
            //   and cast to an int are very fast (the allocated bits remain the same), so overall there's 
            //   a significant performance improvement.
            return (double)(int)(x >> 1) * IntToDoubleMultiplier;
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
                string message = string.Format(null, Resources.ArgumentOutOfRangeGreaterEqual, "maxValue", "0.0");
                throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
            }

            // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
            if(_i >= _longLag)
            {
                Fill();
            }

            uint x = _x[_i++];

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return (double)(int)(x >> 1) * IntToDoubleMultiplier * maxValue;
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
        /// The range between <paramref name="minValue"/> and <paramref name="maxValue"/> must be less than
        ///   or equal to <see cref="Double.MaxValue"/>.
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
            if(_i >= _longLag)
            {
                Fill();
            }

            uint x = _x[_i++];

            // The shift operation and extra int cast before the first multiplication give better performance.
            // See comment in NextDouble().
            return minValue + (double)(int)(x >> 1) * IntToDoubleMultiplier * range;
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
                if(_i >= _longLag)
                {
                    Fill();
                }

                _bitBuffer = _x[_i++];

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

            // Fill the buffer with 4 bytes (1 uint) at a time.
            int i = 0;
            uint w;
            while(i < buffer.Length - 3)
            {
                // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
                if(_i >= _longLag)
                {
                    Fill();
                }

                w = _x[_i++];

                buffer[i++] = (byte)w;
                buffer[i++] = (byte)(w >> 8);
                buffer[i++] = (byte)(w >> 16);
                buffer[i++] = (byte)(w >> 24);
            }

            // Fill up any remaining bytes in the buffer.
            if(i < buffer.Length)
            {
                // Its faster to explicitly calculate the unsigned random number than simply call NextUInt().
                if(_i >= _longLag)
                {
                    Fill();
                }

                w = _x[_i++];

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
        }

        /// <summary>
        /// Resets the <see cref="AdditiveLaggedFibonacciRandomSource"/>,
        /// so that it produces the same pseudo-random number sequence again.
        /// </summary>
        public override
        void
        Reset()
        {
            ResetGenerator();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="AdditiveLaggedFibonacciRandomSource"/> can be reset,
        /// so that it produces the same pseudo-random number sequence again.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the short lag of the Lagged Fibonacci pseudo-random number generator.
        /// </summary>
        /// <remarks>Call <see cref="IsValidShortLag"/> to determine whether a value is valid and therefor assignable.</remarks>
        public int ShortLag
        {
            get
            {
                return _shortLag;
            }

            set
            {
                if(this.IsValidShortLag(value))
                {
                    _shortLag = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the long lag of the Lagged Fibonacci pseudo-random number generator.
        /// </summary>
        /// <remarks>Call <see cref="IsValidLongLag"/> to determine whether a value is valid and therefor assignable.</remarks>
        public int LongLag
        {
            get
            {
                return _longLag;
            }

            set
            {
                if(this.IsValidLongLag(value))
                {
                    _longLag = value;
                    this.Reset();
                }
            }
        }
    }
}
