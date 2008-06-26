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

namespace MathNet.Numerics.Distributions
{
    /// <summary>
    /// Continuous number generator, returning <see cref="Double"/> floating-point numbers.
    /// </summary>
    public interface IContinuousGenerator
    {
        /// <summary>
        /// Generates the next <see cref="Double"/> floating-point numbers.
        /// </summary>
        double NextDouble();

        /// <summary>
        /// True if the generator is reproducible, i.e. te same sequence can be generated again.
        /// </summary>
        bool CanReset
        {
            get;
        }

        /// <summary>
        /// Resets the number generator, so that it produces the same sequence again.
        /// </summary>
        void Reset();
    }
}
