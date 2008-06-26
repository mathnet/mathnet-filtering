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

namespace MathNet.Numerics.Transformations
{
    /// <summary>
    /// FFT Convention
    /// </summary>
    [Flags]
    public enum TransformationConvention : int
    {
        // FLAGS:

        /// <summary>
        /// Only scale by 1/N in the inverse direction; No scaling in forward direction.
        /// </summary>
        AsymmetricScaling = 0x01,

        /// <summary>
        /// Inverse integrand exponent (forward: positive sign; inverse: negative sign).
        /// </summary>
        InverseExponent = 0x02,

        /// <summary>
        /// Don't scale at all (neither on forward nor on inverse transformation).
        /// </summary>
        NoScaling = 0x04,


        // USABILITY POINTERS:

        /// <summary>
        /// Universal; Symmetric scaling and common exponent (used in Maple).
        /// </summary>
        Default = 0,

        /// <summary>
        /// Only scale by 1/N in the inverse direction; No scaling in forward direction (used in Matlab). [= AsymmetricScaling]
        /// </summary>
        Matlab = AsymmetricScaling,

        /// <summary>
        /// Inverse integrand exponent; No scaling at all (used in all Numerical Recipes based implementations). [= InverseExponent | NoScaling]
        /// </summary>
        NumericalRecipes = InverseExponent | NoScaling
    }
}
