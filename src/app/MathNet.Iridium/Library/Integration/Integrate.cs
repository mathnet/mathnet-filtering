//-----------------------------------------------------------------------
// <copyright file="Integrate.cs" company="Math.NET Project">
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
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace MathNet.Numerics.Integration
{
    using MathNet.Numerics.Integration.Algorithms;

    /// <summary>
    /// Numeric Integration (Quadrature).
    /// </summary>
    public static class Integrate
    {
        static DoubleExponentialTransformation det = new DoubleExponentialTransformation();

        /// <summary>
        /// Approximation of the definite interal of an analytic smooth function on a closed interval.
        /// </summary>
        /// <param name="f">The analytic smooth function to integrate.</param>
        /// <param name="intervalBegin">Where the interval starts, inclusive and finite.</param>
        /// <param name="intervalEnd">Where the interval stops, inclusive and finite.</param>
        /// <param name="targetAbsoluteError">The expected relative accuracy of the approximation.</param>
        public static
        double
        OnClosedInterval(
            CustomFunction f,
            double intervalBegin,
            double intervalEnd,
            double targetAbsoluteError)
        {
            return det.Integrate(
                f,
                intervalBegin,
                intervalEnd,
                targetAbsoluteError);
        }

        /// <summary>
        /// Approximation of the definite interal of an analytic smooth function on a closed interval.
        /// </summary>
        /// <param name="f">The analytic smooth function to integrate.</param>
        /// <param name="intervalBegin">Where the interval starts, inclusive and finite.</param>
        /// <param name="intervalEnd">Where the interval stops, inclusive and finite.</param>
        public static
        double
        OnClosedInterval(
            CustomFunction f,
            double intervalBegin,
            double intervalEnd)
        {
            return det.Integrate(
                f,
                intervalBegin,
                intervalEnd,
                1e-8);
        }
    }
}
