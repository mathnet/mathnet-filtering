//-----------------------------------------------------------------------
// <copyright file="EquidistantPolynomialInterpolation.cs" company="Math.NET Project">
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
// <contribution>
//    Numerical Recipes in C++, Second Edition [2003]
//    Handbook of Mathematical Functions [1965]
//    ALGLIB, Sergey Bochkanov
// </contribution>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MathNet.Numerics.Interpolation.Algorithms
{
    /// <summary>
    /// Barycentric Polynomial Interpolation where the given sample points are equidistant.
    /// </summary>
    /// <remarks>
    /// This algorithm neither supports differentiation nor integration.
    /// </remarks>
    public class EquidistantPolynomialInterpolation :
        IInterpolationMethod
    {
        BarycentricInterpolation _barycentric;

        /// <summary>
        /// Initializes a new instance of the EquidistantPolynomialInterpolation class.
        /// </summary>
        public
        EquidistantPolynomialInterpolation()
        {
            _barycentric = new BarycentricInterpolation();
        }

        /// <summary>
        /// True if the alorithm supports differentiation.
        /// </summary>
        /// <seealso cref="Differentiate"/>
        public bool SupportsDifferentiation
        {
            get { return _barycentric.SupportsDifferentiation; }
        }

        /// <summary>
        /// True if the alorithm supports integration.
        /// </summary>
        /// <seealso cref="Integrate"/>
        public bool SupportsIntegration
        {
            get { return _barycentric.SupportsIntegration; }
        }

        /// <summary>
        /// Initialize the interpolation method with the given equidistant samples in the interval [a,b].
        /// </summary>
        /// <param name="a">Left bound of the sample point interval.</param>
        /// <param name="b">Right bound of the sample point interval.</param>
        /// <param name="x">Values x(t) where t are equidistant over [a,b], i.e. x[i] = x(a+(b-a)*i/(n-1))</param>
        public
        void
        Init(
            double a,
            double b,
            IList<double> x
            )
        {
            if(null == x)
            {
                throw new ArgumentNullException("x");
            }

            if(x.Count < 1)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if(b <= a)
            {
                throw new ArgumentOutOfRangeException("b");
            }

            // construct equidistant points
            double[] t = new double[x.Count];
            t[0] = a;
            double step = (b - a) / (t.Length - 1);
            for(int i = 1; i < t.Length; i++)
            {
                t[i] = t[i - 1] + step;
            }

            // construct barycentric weights
            double[] w = new double[x.Count];
            w[0] = 1.0;
            for(int i = 1; i < w.Length; i++)
            {
                w[i] = -(w[i - 1] * (w.Length - i)) / i;
            }

            _barycentric.Init(t, x, w);
        }

        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated value x(t).</returns>
        public
        double
        Interpolate(
            double t
            )
        {
            return _barycentric.Interpolate(t);
        }

        /// <summary>
        /// Differentiate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <param name="first">Interpolated first derivative at point t.</param>
        /// <param name="second">Interpolated second derivative at point t.</param>
        /// <returns>Interpolated value x(t).</returns>
        public
        double
        Differentiate(
            double t,
            out double first,
            out double second
            )
        {
            return _barycentric.Differentiate(t, out first, out second);
        }

        /// <summary>
        /// Definite Integrate up to point t.
        /// </summary>
        /// <param name="t">Right bound of the integration interval [a,t].</param>
        /// <returns>Interpolated definite integeral over the interval [a,t].</returns>
        /// <seealso cref="SupportsIntegration"/>
        public
        double
        Integrate(
            double t
            )
        {
            return _barycentric.Integrate(t);
        }
    }
}
