//-----------------------------------------------------------------------
// <copyright file="IntegerTheory.cs" company="Math.NET Project">
//    Copyright (c) 2002-2009, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Christoph Rüegg, http://christoph.ruegg.name
// </author>
// <product>
//    Math.NET Iridium, part of the Math.NET Project.
//    http://mathnet.opensourcedotnet.info
// </product>
// <license type="opensource" name="LGPL" version="2 or later">
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation; either version 2 of the License, or
//    any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// </license>
// <contribution>
//    StackOverflow.com, John D. Cook
// </contribution>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics.NumberTheory
{
    /// <summary>
    /// Number Theory for Integers
    /// </summary>
    public static class IntegerTheory
    {
        /// <summary>
        /// Find out whether the provided integer is an even number.
        /// </summary>
        /// <returns>True if and only if it is an even number.</returns>
        public static
        bool
        IsEven(
            int number
            )
        {
            return (number & 0x1) == 0x0;
        }

        /// <summary>
        /// Find out whether the provided integer is an odd number.
        /// </summary>
        /// <returns>True if and only if it is an odd number.</returns>
        public static
        bool
        IsOdd(
            int number
            )
        {
            return (number & 0x1) == 0x1;
        }

        /// <summary>
        /// Find out whether the provided integer is a perfect square, i.e. a square of an integer.
        /// </summary>
        /// <returns>True if and only if it is a perfect square.</returns>
        public static
        bool
        IsPerfectSquare(
            int number
            )
        {
            if(number < 0)
            {
                return false;
            }

            int lastHexDigit = number & 0xF;
            if(lastHexDigit > 9)
            {
                return false; // return immediately in 6 cases out of 16.
            }

            /*
             TODO (ruegg, 2009-01-02): Test/benchmark wherher the inverse test performs better:
             lastHexDigit != 2 && lastHexDigit != 3 && lastHexDigit != 5 && lastHexDigit != 6 && lastHexDigit != 7 && lastHexDigit != 8
             */
            if(lastHexDigit == 0 || lastHexDigit == 1 || lastHexDigit == 4 || lastHexDigit == 9)
            {
                int t = (int)Math.Floor(Math.Sqrt(number) + 0.5);
                return (t * t) == number;
            }

            return false;
        }
    }
}
