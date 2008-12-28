//-----------------------------------------------------------------------
// <copyright file="InterpolationMethod.cs" company="Math.NET Project">
//    Copyright (c) 2002-2008, Christoph Rüegg.
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
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using MathNet.Numerics;

namespace MathNet.Numerics.Interpolation
{
    /// <summary>
    /// Interpolation method
    /// </summary>
    public interface IInterpolationMethod
    {
        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated value x(t).</returns>
        /// <seealso cref="Differentiate"/>
        /// <seealso cref="Integrate"/>
        double
        Interpolate(
            double t
            );

        /// <summary>
        /// True if the alorithm supports differentiation.
        /// </summary>
        /// <seealso cref="Differentiate"/>
        bool SupportsDifferentiation
        {
            get;
        }

        /// <summary>
        /// Differentiate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <param name="first">Interpolated first derivative at point t.</param>
        /// <param name="second">Interpolated second derivative at point t.</param>
        /// <returns>Interpolated value x(t).</returns>
        /// <seealso cref="SupportsDifferentiation"/>
        double
        Differentiate(
            double t,
            out double first,
            out double second
            );

        /// <summary>
        /// True if the alorithm supports integration.
        /// </summary>
        /// <seealso cref="Integrate"/>
        bool SupportsIntegration
        {
            get;
        }

        /// <summary>
        /// Definite Integrate up to point t.
        /// </summary>
        /// <param name="t">Right bound of the integration interval [a,t].</param>
        /// <returns>Interpolated definite integeral over the interval [a,t].</returns>
        /// <seealso cref="SupportsIntegration"/>
        double
        Integrate(
            double t
            );
    }
}
