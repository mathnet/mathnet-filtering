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

using System;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.RandomSources
{
    /// <summary>
    /// Represents a simple pseudo-random number generator.
    /// </summary>
    /// <remarks>
    /// The <see cref="SystemRandomSource"/> type internally uses an instance of the <see cref="System.Random"/> type 
    ///   to generate pseudo-random numbers.
    /// </remarks>
    public class SystemRandomSource :
        RandomSource
    {
        /// <summary>
        /// Stores an instance of <see cref="System.Random"/> type that is used to generate random numbers.
        /// </summary>
        Random _generator;

        /// <summary>
        /// Stores the used seed value.
        /// </summary>
        int _seed;

        /// <summary>
        /// Stores an <see cref="Int32"/> used to generate up to 31 random <see cref="Boolean"/> values.
        /// </summary>
        int _bitBuffer;

        /// <summary>
        /// Stores how many random <see cref="Boolean"/> values still can be generated from <see cref="_bitBuffer"/>.
        /// </summary>
        int _bitCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRandomSource"/> class, using a time-dependent default 
        ///   seed value.
        /// </summary>
        public
        SystemRandomSource()
            : this(Environment.TickCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemRandomSource"/> class, using the specified seed value.
        /// </summary>
        /// <param name="seed">
        /// A number used to calculate a starting value for the pseudo-random number sequence.
        /// If a negative number is specified, the absolute value of the number is used. 
        /// </param>
        public
        SystemRandomSource(
            int seed
            )
        {
            _seed = Math.Abs(seed);
            ResetGenerator();
        }

        /// <summary>
        /// Resets the <see cref="SystemRandomSource"/>, so that it produces the same pseudo-random number sequence again.
        /// </summary>
        void
        ResetGenerator()
        {
            // Create a new Random object using the same seed.
            _generator = new Random(_seed);

            // Reset helper variables used for generation of random bools.
            _bitBuffer = 0;
            _bitCount = 0;
        }
        
        /// <summary>
        /// Returns a nonnegative random number.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero and less than <see cref="Int32.MaxValue"/>.
        /// </returns>
        public override
        int
        Next()
        {
            return _generator.Next();
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
            return _generator.Next(maxValue);
        }

        /// <summary>
        /// Returns a random number within a specified range. 
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
            return _generator.Next(minValue, maxValue);
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
            return _generator.NextDouble();
        }

        /// <summary>
        /// Returns a nonnegative floating point random number less than the specified maximum.
        /// </summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. 
        /// <paramref name="maxValue"/> must be greater than or equal to zero. 
        /// </param>
        /// <returns>
        /// A double-precision floating point number greater than or equal to zero, and less than <paramref name="maxValue"/>; 
        ///   that is, the range of return values includes zero but not <paramref name="maxValue"/>. 
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
            if(maxValue < 0)
            {
                string message = string.Format(
                    Resources.ArgumentOutOfRangeGreaterEqual,
                    "maxValue",
                    "0.0"
                    );

                throw new ArgumentOutOfRangeException("maxValue", maxValue, message);
            }

            return _generator.NextDouble() * maxValue;
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

            return minValue + _generator.NextDouble() * range;
        }

        /// <summary>
        /// Returns a random Boolean value.
        /// </summary>
        /// <remarks>
        /// Buffers 31 random bits (1 int) for future calls, so a new random number is only generated every 31 calls.
        /// </remarks>
        /// <returns>A <see cref="Boolean"/> value.</returns>
        public override
        bool
        NextBoolean()
        {
            if(_bitCount == 0)
            {
                // Generate 31 more bits (1 int) and store it for future calls.
                _bitBuffer = _generator.Next();

                // Reset the bitCount and use rightmost bit of buffer to generate random bool.
                _bitCount = 30;
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
        /// Each element of the array of bytes is set to a random number greater than or equal to zero, and less than or 
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
            _generator.NextBytes(buffer);
        }

        /// <summary>
        /// Resets the <see cref="SystemRandomSource"/>, so that it produces the same pseudo-random number sequence again.
        /// </summary>
        public override
        void
        Reset()
        {
            ResetGenerator();
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="SystemRandomSource"/> can be reset, so that it produces the 
        ///   same pseudo-random number sequence again.
        /// </summary>
        public override bool CanReset
        {
            get { return true; }
        }
    }
}
