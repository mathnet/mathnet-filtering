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

using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics
{
    /// <summary>
    /// A generic vector of two values, useful e.g. to return two values
    /// in a function without using out-parameters.
    /// </summary>
    public struct Tuple<TFirst, TSecond> : IEquatable<Tuple<TFirst, TSecond>>
        where TFirst : IEquatable<TFirst>
        where TSecond : IEquatable<TSecond>
    {
        private readonly TFirst _first;
        private readonly TSecond _second;

        /// <summary>
        /// Construct a tuple
        /// </summary>
        /// <param name="first">The first tuple value</param>
        /// <param name="second">The second tuple value</param>
        public Tuple(TFirst first, TSecond second)
        {
            _first = first;
            _second = second;
        }

        /// <summary>
        /// The first tuple value
        /// </summary>
        public TFirst First
        {
            get { return _first; }
        }

        /// <summary>
        /// The second tuple value
        /// </summary>
        public TSecond Second
        {
            get { return _second; }
        }

        /// <summary>
        /// True if the the first values of both tuples match and the second valus of both tuples match.
        /// </summary>
        /// <param name="other">The other tuple to compare with.</param>
        public bool Equals(Tuple<TFirst, TSecond> other)
        {
            return _first.Equals(other.First) && _second.Equals(other.Second);
        }
    }
}
