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

namespace MathNet.Numerics.Interpolation
{
    /// <summary>
    /// Left and right boundary conditions.
    /// </summary>
    public enum SplineBoundaryCondition
    {
        /// <summary>
        /// Natural Boundary (Zero second derivative).
        /// </summary>
        Natural = 0,

        /// <summary>
        /// Parabolically Terminated boundary.
        /// </summary>
        ParabolicallyTerminated,

        /// <summary>
        /// Fixed first derivative at the boundary.
        /// </summary>
        FirstDerivative,

        /// <summary>
        /// Fixed second derivative at the boundary.
        /// </summary>
        SecondDerivative
    }
}
